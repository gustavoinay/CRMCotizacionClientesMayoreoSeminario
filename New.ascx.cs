/*
' Copyright (c) 2019  ffacsa.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/


using DevExpress.Web;
using DotNetNuke.Services.Exceptions;
using Oracle.DataAccess.Client;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace ffacsa.Modules.Cotizacion
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The EditCotizacion class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from CotizacionModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class New : CotizacionModuleBase
    {
        public static CotizacionEnc cotizacion;
        public Plantillas listaPlantillas;
        int priceList;
        string CodBodega;
        string whscode;
        decimal precioSinIva;

        protected void Page_Load(object sender, EventArgs e)
        {
            //string bodegaRegistroCotizacion = "";
            try
            {
                if(!IsPostBack)
                {
                    cotizacion = new CotizacionEnc();
                   
                    cargoPlantillas();
                }

                

                prepareData();
                
                //ASPxTextBox2.Text = Request.QueryString["DocNum"];
                //ASPxTextBox3.Text = Request.QueryString["mid"];


            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cargoPlantillas()
        {

            ASPxComboBoxPlantilla.Items.Add("<Seleccione una Solución..>");
            try
            {
                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);
                OracleCommand cmdPlantilla = new OracleCommand($"Select Cod_Plantilla, Nom_Plantilla, DocNum_Base From T06_PLANTILLA_COTIZACION Order by Cod_Plantilla Asc", cnn);

                cnn.Open();
                OracleDataReader dataReader;
                dataReader = cmdPlantilla.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        ListaPlantillas plantilla = new ListaPlantillas();

                        plantilla.Cod_Plantilla = int.Parse(dataReader.GetValue(0).ToString());
                        plantilla.Nom_Plantilla = dataReader.GetString(1);
                        plantilla.DocNumBase = int.Parse(dataReader.GetValue(2).ToString());

                        //  listaPlantillas.listarPlantillas.Add(plantilla);
                        ASPxComboBoxPlantilla.Items.Add(plantilla.Nom_Plantilla);
                    }
                }

                dataReader.Close();

                cnn.Close();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void prepareData()
        {
            string strConn = ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(strConn);
            
            DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
            int userId = userInfo.UserID;
            string userMail = userInfo.Email;
            bool isInternal = userInfo.IsInRole("ffacsaInterno");
            bool isExternal = userInfo.IsInRole("ffacsaExterno");

            
            priceList = -1;

            string strUserSql = "Select T0.CardCode, T0.CardName, T0.ListNum, LEFT(T0.U_Whscode,2) Bodega, T0.U_Whscode, T0.SlpCode, T0.AddID, T1.Name, T1.Address, T1.Tel1, T1.E_MailL " +
                                    $"From OCRD T0 Inner Join OCPR T1 On T0.CardCode = T1.CardCode Where T1.E_MailL = '{userMail}'";

            string strSlpSql = "Select SlpCode, SlpName From OSLP Where " +
                               "SlpCode IN(select SlpCode from OCRD inner join OCPR on OCRD.CardCode = OCPR.CardCode " +
                               $"and OCPR.E_MailL = '{userMail}')";

            SqlCommand slpCmm = new SqlCommand( strSlpSql, cnn);
            SqlCommand userCmm = new SqlCommand(strUserSql, cnn);
            SqlDataReader reader;

            try
            {
                cnn.Open();
                reader = userCmm.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    priceList = reader.GetInt16(2);
                    CodBodega = reader.GetString(3);
                    whscode = reader.GetString(4);
                }
                else
                    priceList = 1;

                reader.Close();

                string strItemSql = "select T0.ItemCode, concat(T0.ItemName,'(',T0.U_ItemMarca,')') ItemName, T0.U_ItemCategoria, T2.OnHand, ROUND(T1.Price * 1.12, 2) Price " +
                                        $"from OITM T0 " +
                                        $"inner join ITM1 T1 on T0.ItemCode = T1.ItemCode " +
                                        $"inner join OITW T2 on T0.ItemCode = T2.ItemCode " +
                                        $"where T0.ItemCode < 'Z' and T0.validFor = 'Y' and T0.SellItem = 'Y' " +
                                        $"and T1.PriceList = {priceList} " +
                                        $"and T2.WhsCode = '{whscode}' " +
                                        $"and T1.Price > 0.00 and T0.QryGroup3 = 'Y'";

                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();

                using (SqlDataAdapter a = new SqlDataAdapter(userCmm))
                {
                    a.Fill(dt);
                }
                using (SqlDataAdapter b = new SqlDataAdapter(slpCmm))
                {
                    b.Fill(dt2);
                }
                
                //llena datos del cliente
                ASPxGridLookup1.DataSource = dt;
                ASPxGridLookup1.DataBind();
                //llena datos del vendedor
                ASPxGridLookup2.DataSource = dt2;
                ASPxGridLookup2.DataBind();
                //Asocia datos del detalle con la Lista

                gridCotizacion.DataSource = cotizacion.CotizacionDetalle;
                gridCotizacion.DataBind();

                itemsData.ConnectionString = strConn;
                itemsData.SelectCommand = strItemSql;
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
        }

        protected void glCategory_Load(object sender, EventArgs e)
        {
            (sender as ASPxGridLookup).GridView.Width = new Unit(500, UnitType.Pixel);

            
        }

        protected void grid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            CotizacionDet detalle = new CotizacionDet();
            
            detalle.LineNum = cotizacion.CotizacionDetalle.Count + 1;
            detalle.ItemCode = e.NewValues["ItemCode"].ToString();
            detalle.Quantity = int.Parse(e.NewValues["Quantity"].ToString());
            

            string articulo = detalle.ItemCode;
            //decimal existencia;

            string strConn = ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString;
            string strDatos = $"Select ROUND(T1.Price * 1.12, 2) Price, T0.ItemName, T2.OnHand " +
                                $"From OITM T0 Inner Join ITM1 T1 On T0.ItemCode = T1.ItemCode Inner join OITW T2 on T0.ItemCode = T2.ItemCode Where T0.ItemCode = '{ articulo }' And T1.PriceList = {priceList} and T2.WhsCode = '{whscode}'";
            SqlConnection cnn = new SqlConnection(strConn);
            SqlCommand command = new SqlCommand(strDatos, cnn);

            try
            {
                cnn.Open();
                SqlDataReader dataReader;
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    //decimal art = Convert.ToDecimal(dataReader.GetString(0));

                   decimal precioConIva = dataReader.GetDecimal(0);
                   precioSinIva = Convert.ToDecimal(precioConIva / 1.12M);

                    detalle.Price = dataReader.GetDecimal(0);
                   // detalle.Price = precioArt * iva;
                    detalle.Dscription = dataReader.GetString(1);

                    detalle.GTotal = int.Parse(e.NewValues["Quantity"].ToString()) * detalle.Price;

                    detalle.LineTotal = precioSinIva;

                   // detalle.OnHand = dataReader.GetDecimal(2);

                    //detalle.LineTotal = int.Parse(e.NewValues["Quantity"].ToString()) * decimal.Parse(e.NewValues["Price"].ToString());

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
            }

            gridCotizacion.CancelEdit();
            e.Cancel = true;
            cotizacion.CotizacionDetalle.Add(detalle);
        }

        protected void grid_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            int row = cotizacion.CotizacionDetalle.FindIndex(d => d.LineNum == int.Parse(e.Keys[0].ToString()));


            cotizacion.CotizacionDetalle[row].LineStatus = 2;
            cotizacion.CotizacionDetalle[row].ItemCode = e.NewValues["ItemCode"].ToString();
            cotizacion.CotizacionDetalle[row].Quantity = int.Parse(e.NewValues["Quantity"].ToString());
           // cotizacion.CotizacionDetalle[row].GTotal = int.Parse(e.NewValues["Quantity"].ToString()) * cotizacion.CotizacionDetalle[row].Price;
            //cotizacion.CotizacionDetalle[row].LineTotal = int.Parse(e.NewValues["Quantity"].ToString()) * decimal.Parse(e.NewValues["Price"].ToString());

            
            gridCotizacion.CancelEdit();
            e.Cancel = true;
                       

            string articulo = cotizacion.CotizacionDetalle[row].ItemCode;

            string strConn = ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString;
            string strDatos = $"Select ROUND(T1.Price * 1.12, 2) Price, T0.ItemName, T2.OnHand " +
                                $"From OITM T0 Inner Join ITM1 T1 On T0.ItemCode = T1.ItemCode Inner join OITW T2 on T0.ItemCode = T2.ItemCode Where T0.ItemCode = '{ articulo }' And T1.PriceList = {priceList} and T2.WhsCode = '{whscode}'";
            SqlConnection cnn = new SqlConnection(strConn);
            SqlCommand command = new SqlCommand(strDatos, cnn);

            try
            {
                cnn.Open();
                SqlDataReader dataReader;
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    //decimal art = Convert.ToDecimal(dataReader.GetString(0));

                    decimal precioConIva = dataReader.GetDecimal(0);
                    decimal precioSinIva = Convert.ToDecimal(precioConIva / 1.12M);

                    cotizacion.CotizacionDetalle[row].Price = dataReader.GetDecimal(0);
                    // detalle.Price = precioArt * iva;cotizacion.CotizacionDetalle[row].Price
                    cotizacion.CotizacionDetalle[row].Dscription = dataReader.GetString(1);

                    cotizacion.CotizacionDetalle[row].GTotal = int.Parse(e.NewValues["Quantity"].ToString()) * cotizacion.CotizacionDetalle[row].Price;

                    cotizacion.CotizacionDetalle[row].LineTotal = precioSinIva;
                   // cotizacion.CotizacionDetalle[row].OnHand = dataReader.GetDecimal(2);

                    //detalle.LineTotal = int.Parse(e.NewValues["Quantity"].ToString()) * decimal.Parse(e.NewValues["Price"].ToString());

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
            }

            gridCotizacion.CancelEdit();
            e.Cancel = true;
        }

        protected void gridCotizacion_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {

            int row = cotizacion.CotizacionDetalle.FindIndex(d => d.LineNum == int.Parse(e.Keys[0].ToString()));

            cotizacion.CotizacionDetalle.RemoveAt(row);
            cotizacion.CotizacionDetalle[row].LineStatus = 3;

            gridCotizacion.CancelEdit();
            e.Cancel = true;           
            
        }

        protected void ASPxGridLookup1_ValueChanged(object sender, EventArgs e)
        {
            string CardCode = ASPxGridLookup1.Value.ToString();

            DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
            int userId = userInfo.UserID;
            string userMail = userInfo.Email;
            //bool isInternal = userInfo.IsInRole("ffacsaInterno");
            //bool isExternal = userInfo.IsInRole("ffacsaExterno");


            string strConn = ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString;

            string strCommando = "Select T0.CardCode, T0.CardName, T0.ListNum, T0.SlpCode, ISNULL(T0.AddID, ''), T1.Name, ISNULL(T1.Address, ''), ISNULL(T1.Tel1, ''), T1.E_MailL " +
                                   $"From OCRD T0 Inner Join OCPR T1 On T0.CardCode = T1.CardCode Where T1.E_MailL = '{userMail}'";

            //string strCommando = $"Select T0.AddID, T0.CardName, ISNULL(T0.Address, ''), ISNULL(T0.Phone1,''), T0.E_Mail, T0.SlpCode From OCRD T0 Inner Join OCPR T1 On T0.CardCode = T1.CardCode " +
            //                                        $"Where T0.CardCode = '{CardCode}'";
            SqlConnection cnn = new SqlConnection(strConn);
            SqlCommand command3 = new SqlCommand(strCommando , cnn);
            try
            {

                cnn.Open();
                SqlDataReader dataReader;
                dataReader = command3.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    string cardname = dataReader.GetString(1);
                    int slpcode = dataReader.GetInt32(3);
                    string nit = dataReader.GetString(4);
                    string contacto = dataReader.GetString(5);
                    string address = dataReader.GetString(6);
                    string telefono = dataReader.GetString(7);
                    string email = dataReader.GetString(8);
                    string fecha = DateTime.Now.ToShortDateString();
                   
                    ASPxTextBoxNIT.Text = nit;
                    ASPxTextBoxContacto.Text = contacto;
                    ASPxTextBoxDireccion.Text = address;
                    ASPxTextBoxTelefono.Text = telefono;
                    ASPxTextBoxEmail.Text = email;
                    ASPxGridLookup2.Value = slpcode;
                    ASPxTextBoxFecha.Text = fecha;
                    ASPxTextBoxEstado.Text = "P";
                }
                else
                {
                    ASPxTextBoxNIT.Text = "";
                    ASPxTextBoxContacto.Text = "";
                    ASPxTextBoxDireccion.Text = "";
                    ASPxTextBoxTelefono.Text = "";
                    ASPxTextBoxEmail.Text = "";
                }
                dataReader.Close();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            finally
            {
                if( cnn.State == ConnectionState.Open )
                    cnn.Close();
            }

        }

        protected void MenuNuevaCoti_ItemClick(object source, MenuItemEventArgs e)
        {
            if (e.Item.Name.Equals("buttonGuardar"))
            {

                //actualizoCotizacion(DocNum);
                decimal DocTotal = 0;
                foreach (CotizacionDet linea in cotizacion.CotizacionDetalle)
                {
                    DocTotal += Convert.ToDecimal(linea.GTotal.ToString());
                
                }

                //recuperoCardName();
                guardoCotizacion(DocTotal);
                CotizacionEnc datos = cotizacion;
                int codigoVendedor = int.Parse(datos.SlpCode);

                agregoMasDatosCoti(codigoVendedor);
               
                
            }

            if (e.Item.Name.Equals("buttonAutorizar"))
            {
                
            }

            if (e.Item.Name.Equals("buttonCerrar"))
            {
                Response.Redirect(ModuleContext.NavigateUrl(PortalSettings.ActiveTab.TabID, "", false, "mid=" + ModuleId.ToString()));
            }

        }

        

        private void agregoMasDatosCoti(int slpCode)
        {
            // Se agrega el nombre del vendedor en base a su código
            try
            {
                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);
                OracleCommand cmdDocNum = new OracleCommand($"Select Max(DocNum) DocNum From T06_OQUT", cnn);

                cnn.Open();
                OracleDataReader dataReader;
                dataReader = cmdDocNum.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    int DocNumActualizar = int.Parse(dataReader.GetValue(0).ToString());
     
                    string sqlAgregoDatos = $"Update T06_OQUT Set U_SlpName = (Select SlpName From OSLP Where SlpCode = {slpCode}) Where DocEntry = {DocNumActualizar}";
                    OracleCommand cmdAgregar = new OracleCommand(sqlAgregoDatos, cnn);

                    cmdAgregar.ExecuteNonQuery();
                }

                dataReader.Close();

                cnn.Close();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }


        }

        private void limpioDatos()
        {

            ASPxGridLookup1.Text = "";
            ASPxGridLookup2.Text = "";
            ASPxTextBoxNIT.Text = "";
            ASPxTextBoxContacto.Text = "";
            ASPxTextBoxDireccion.Text = "";
            ASPxTextBoxTelefono.Text = "";
            ASPxTextBoxEmail.Text = "";
            ASPxTextBoxFecha.Text = "";
            ASPxTextBoxPrestatario.Text = "";
            ASPxTextBoxDirEntrega.Text = "";
            ASPxTextBoxTelPrestatario.Text = "";
            
        }

        private void recuperoCardName()
        {
            
           
            try
            {
                cotizacion.CardCode = ASPxGridLookup1.Value.ToString();
                string CardCode = cotizacion.CardCode;

                string strConnection = ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString;
                string strCardName = $"Select CardName From OCRD Where CardCode = {CardCode}";
                SqlConnection conn = new SqlConnection(strConnection);
                SqlCommand cmdCardName = new SqlCommand(strCardName, conn);

                conn.Open();
                SqlDataReader dataReader;
                dataReader = cmdCardName.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    string CardName = dataReader.GetString(0);
                }
                dataReader.Close();
                conn.Close();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void guardoCotizacion(decimal doctotal)
        {
            
            //DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
            //int userId = userInfo.UserID;
            //string userMail = userInfo.Email;
            ////bool isInternal = userInfo.IsInRole("ffacsaInterno");
            ////bool isExternal = userInfo.IsInRole("ffacsaExterno");

            //string bodegaCotiza = "";
            //string bodegaDetalle = "";
            //if (userMail.ToString() == "egarcia@habitatguate.org" || userMail.ToString() == "mjcastro@habitatguate.org")
            //{
            //    bodegaCotiza = "14";
            //    bodegaDetalle = "1401";               
            //}

            //if (ASPxGridLookup1.Value.ToString() == "" || int.Parse(ASPxGridLookup2.Value.ToString()) == 0 || ASPxTextBoxNIT.Value.ToString() == "" || ASPxTextBoxContacto.Value.ToString() == "" || ASPxTextBoxDireccion.Value.ToString() == "" || ASPxTextBoxTelefono.Value.ToString() == "" || ASPxTextBoxEmail.Value.ToString() == "" || ASPxTextBoxFecha.Value.ToString() == "" || ASPxTextBoxEstado.Value.ToString() == "" || bodegaCotiza == "")
            //{
            //    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Faltan Datos en La Cotización", "No se pudo Guardar", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            //}

            /*------------------------- ENCABEZADO -----------------------------*/
            cotizacion.CardCode = ASPxGridLookup1.Value.ToString();
            cotizacion.SlpCode = ASPxGridLookup2.Value.ToString();
            cotizacion.NIT = ASPxTextBoxNIT.Value.ToString();
            cotizacion.Nombre = ASPxTextBoxContacto.Value.ToString();
            cotizacion.Direccion = ASPxTextBoxDireccion.Text.ToString();
            cotizacion.Telefono = ASPxTextBoxTelefono.Value.ToString();
            cotizacion.Email = ASPxTextBoxEmail.Value.ToString();
            cotizacion.DocDate = ASPxTextBoxFecha.Value.ToString();
            //cotizacion.DocDate = aspxtext
            cotizacion.DocStatus = ASPxTextBoxEstado.Value.ToString();
            cotizacion.CodBodega = CodBodega;
            cotizacion.NomPrestatario = ASPxTextBoxPrestatario.Text;
            cotizacion.DirEntrega = ASPxTextBoxDirEntrega.Text;
            cotizacion.TelPrestatario = ASPxTextBoxTelPrestatario.Text;

            
            string CardCode = cotizacion.CardCode;
            string CardName = cotizacion.Nombre;
            int SlpCode = Convert.ToInt32(cotizacion.SlpCode);
            string NIT = cotizacion.NIT;
            string Nombre = cotizacion.Nombre;
            string Direccion = cotizacion.Direccion;
            string Telefono = cotizacion.Telefono;
            string Email = cotizacion.Email;
            //DateTime DocDate = DateTime.Parse(cotizacion.DocDate);
            DateTime DocDate = DateTime.Now;
            string DocStatus = cotizacion.DocStatus;
            string Bodega = CodBodega;
            string WhsCode = whscode;
            int intDocEntry = 0;
            string pindicator = "valor de p";
            string u_docfiscal = "N";
            decimal total = doctotal;

            /*Datos del prestatario*/

            string NombrePrestatario = ASPxTextBoxPrestatario.Text;
            string DireccionEntrega = ASPxTextBoxDirEntrega.Text;
            string TelPrestatario = ASPxTextBoxTelPrestatario.Text;

            /*Datos especificos del vale*/

            string TipoSolucion = ASPxTextBoxTipoSolucion.Text;
            string Trimestre = ASPxTextBoxTrimestre.Text;
            string SupervisorConstruccion = ASPxTextBoxSupervisorConstruccion.Text;
            string TelSupervisor = ASPxTextBoxTelSupervisor.Text;


            if (CardCode == "" || SlpCode == 0 || NIT == "" || Nombre == "" || Direccion == "" || Telefono == "" || Email == "" || NombrePrestatario == "" || DireccionEntrega == "" || TelPrestatario == "")
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Faltan Datos en La Cotización", " -- No se pudo Guardar", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
            else
            {

                try
                {

                    string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                    OracleConnection cnn = new OracleConnection(strConn);
                    string sqlInsert = "P06_INSERTA_COTIZACION_WEB";

                    OracleCommand cmdInsert = new OracleCommand(sqlInsert, cnn);
                    cmdInsert.CommandType = CommandType.StoredProcedure;

                    /*Detalle de la cotización*/
                    string sqlDetalle = "insert into T06_QUT1 (DocEntry,LineNum, ItemCode, Dscription, Price, Quantity, WhsCode, Bodega_Entrega, Precio_Descto, LineTotal, U_TipoIva, Currency, VatPrcnt, VatSumSy, TaxCode, U_Descuento_ventas, Descto_Automatico, U_Descuento_Fact, GTOTAL ) ";
                    sqlDetalle = sqlDetalle + "values (:DocEntry,:LineNum, :ItemCode, :Dscription, :Price, :Quantity, :WhsCode, :BodegaEntrega, :PrecioDescuento, :LineTotal, :U_TipoIva, 'QTZ', 0.12, 1.12, 'IVA', 0 , 'S', 0, :GTotal)";
                    OracleCommand cmdDetalle = new OracleCommand(sqlDetalle, cnn);

                    OracleParameter detalleDocEntry = new OracleParameter();
                    detalleDocEntry.DbType = DbType.Int32;
                    detalleDocEntry.ParameterName = ":DocEntry";

                    OracleParameter pLineNum = new OracleParameter();
                    pLineNum.DbType = DbType.Int32;
                    pLineNum.ParameterName = ":LineNum";

                    OracleParameter pItemCode = new OracleParameter();
                    pItemCode.DbType = DbType.String;
                    pItemCode.ParameterName = ":ItemCode";

                    OracleParameter pDscription = new OracleParameter();
                    pDscription.DbType = DbType.String;
                    pDscription.ParameterName = ":Dscription";

                    OracleParameter pPrice = new OracleParameter();
                    pPrice.DbType = DbType.Decimal;
                    pPrice.ParameterName = ":Price";

                    OracleParameter pQuantity = new OracleParameter();
                    pQuantity.DbType = DbType.Decimal;
                    pQuantity.ParameterName = ":Quantity";

                    OracleParameter pWhsCode = new OracleParameter();
                    pWhsCode.DbType = DbType.String;
                    pWhsCode.ParameterName = ":WhsCode";

                    OracleParameter pBodegaEntrega = new OracleParameter();
                    pBodegaEntrega.DbType = DbType.String;
                    pBodegaEntrega.ParameterName = ":BodegaEntrega";

                    OracleParameter pPreciodescuento = new OracleParameter();
                    pPreciodescuento.DbType = DbType.Decimal;
                    pPreciodescuento.ParameterName = ":PrecioDescuento";

                    OracleParameter pLineTotal = new OracleParameter();
                    pLineTotal.DbType = DbType.Decimal;
                    pLineTotal.Value = precioSinIva;
                    pLineTotal.ParameterName = ":LineTotal";

                    OracleParameter pUTipoIva = new OracleParameter();
                    pUTipoIva.DbType = DbType.String;
                    pUTipoIva.Value = "X";
                    pUTipoIva.ParameterName = ":U_TipoIva";

                    OracleParameter pGTotal = new OracleParameter();
                    pGTotal.DbType = DbType.Decimal;
                    pGTotal.ParameterName = ":GTotal";

                    cmdDetalle.Parameters.Add(detalleDocEntry);
                    cmdDetalle.Parameters.Add(pLineNum);
                    cmdDetalle.Parameters.Add(pItemCode);
                    cmdDetalle.Parameters.Add(pDscription);
                    cmdDetalle.Parameters.Add(pPrice);
                    cmdDetalle.Parameters.Add(pQuantity);
                    cmdDetalle.Parameters.Add(pWhsCode);
                    cmdDetalle.Parameters.Add(pBodegaEntrega);
                    cmdDetalle.Parameters.Add(pPreciodescuento);
                    cmdDetalle.Parameters.Add(pLineTotal);
                    cmdDetalle.Parameters.Add(pUTipoIva);
                    cmdDetalle.Parameters.Add(pGTotal);

                    /*--------------------Finaliza Detalle Cotizacion -----------------*/

                    /*encabezado*/
                    OracleParameter codCliente = new OracleParameter();
                    codCliente.DbType = DbType.String;
                    codCliente.Value = CardCode;
                    codCliente.ParameterName = ":CardCode";

                    OracleParameter nomCliente = new OracleParameter();
                    nomCliente.DbType = DbType.String;
                    nomCliente.Value = CardName;
                    nomCliente.ParameterName = ":CardName";

                    OracleParameter codVendedor = new OracleParameter();
                    codVendedor.DbType = DbType.Int32;
                    codVendedor.Value = SlpCode;
                    codVendedor.ParameterName = ":SlpCode";

                    OracleParameter u_facnit = new OracleParameter();
                    u_facnit.DbType = DbType.String;
                    u_facnit.Value = NIT;
                    u_facnit.ParameterName = ":NIT";


                    OracleParameter u_facnom = new OracleParameter();
                    u_facnom.DbType = DbType.String;
                    u_facnom.Value = Nombre;
                    u_facnom.ParameterName = ":Nombre";

                    OracleParameter u_direccion = new OracleParameter();
                    u_direccion.DbType = DbType.String;
                    u_direccion.Value = Direccion;
                    u_direccion.ParameterName = ":Direccion";

                    OracleParameter u_telefonos = new OracleParameter();
                    u_telefonos.DbType = DbType.String;
                    u_telefonos.Value = Telefono;
                    u_telefonos.ParameterName = ":Telefono";

                    OracleParameter u_email = new OracleParameter();
                    u_email.DbType = DbType.String;
                    u_email.Value = Email;
                    u_email.ParameterName = ":Email";

                    OracleParameter FechaDoc = new OracleParameter();
                    FechaDoc.DbType = DbType.DateTime;
                    FechaDoc.Value = DocDate;
                    FechaDoc.ParameterName = ":DocDate";

                    // Se envía con status G, de Guardado

                    //OracleParameter Status = new OracleParameter();
                    //Status.DbType = DbType.String;
                    //Status.Value = DocStatus;
                    //Status.ParameterName = ":DocStatus";

                    OracleParameter indicator = new OracleParameter();
                    indicator.DbType = DbType.String;
                    indicator.Value = pindicator;
                    indicator.ParameterName = ":Pindicator";

                    OracleParameter docfiscal = new OracleParameter();
                    docfiscal.DbType = DbType.String;
                    docfiscal.Value = u_docfiscal;
                    docfiscal.ParameterName = ":U_DocFiscal";

                    OracleParameter pDocTotal = new OracleParameter();
                    pDocTotal.DbType = DbType.Decimal;
                    pDocTotal.Value = total;
                    pDocTotal.ParameterName = ":DocTotal";

                    OracleParameter pListNum = new OracleParameter();
                    pListNum.DbType = DbType.Int32;
                    pListNum.Value = priceList;
                    pListNum.ParameterName = ":ListNum";

                    OracleParameter pcodBodega = new OracleParameter();
                    pcodBodega.DbType = DbType.String;
                    pcodBodega.Value = CodBodega;
                    pcodBodega.ParameterName = ":CodBodega";

                    OracleParameter pNomPrestatario = new OracleParameter();
                    pNomPrestatario.DbType = DbType.String;
                    pNomPrestatario.Value = NombrePrestatario;
                    pNomPrestatario.ParameterName = ":NomPrestatario";

                    OracleParameter pDirEntrega = new OracleParameter();
                    pDirEntrega.DbType = DbType.String;
                    pDirEntrega.Value = DireccionEntrega;
                    pDirEntrega.ParameterName = ":DirEntrega";

                    OracleParameter pTelPrestatario = new OracleParameter();
                    pTelPrestatario.DbType = DbType.String;
                    pTelPrestatario.Value = TelPrestatario;
                    pTelPrestatario.ParameterName = ":TelPrestatario";

                    OracleParameter pTipoSolucion = new OracleParameter();
                    pTipoSolucion.DbType = DbType.String;
                    pTipoSolucion.Value = TipoSolucion;
                    pTipoSolucion.ParameterName = ":TipoSolucion";

                    OracleParameter pTrimestre = new OracleParameter();
                    pTrimestre.DbType = DbType.String;
                    pTrimestre.Value = Trimestre;
                    pTrimestre.ParameterName = ":Trimestre";

                    OracleParameter pSupervisor = new OracleParameter();
                    pSupervisor.DbType = DbType.String;
                    pSupervisor.Value = SupervisorConstruccion;
                    pSupervisor.ParameterName = ":Supervisor";

                    OracleParameter pTelSupervisor = new OracleParameter();
                    pTelSupervisor.DbType = DbType.String;
                    pTelSupervisor.Value = TelSupervisor;
                    pTelSupervisor.ParameterName = ":TelSupervisor";

                    OracleParameter pContacto = new OracleParameter();
                    pContacto.DbType = DbType.String;
                    pContacto.Value = Nombre;
                    pContacto.ParameterName = ":Contacto";

                    OracleParameter nuevoDocEntry = new OracleParameter();
                    nuevoDocEntry.DbType = DbType.Int32;
                    nuevoDocEntry.Direction = ParameterDirection.InputOutput;
                    nuevoDocEntry.Value = intDocEntry;
                    nuevoDocEntry.ParameterName = ":DocEntry";



                    cmdInsert.Parameters.Add(codCliente);
                    cmdInsert.Parameters.Add(nomCliente);
                    cmdInsert.Parameters.Add(codVendedor);
                    cmdInsert.Parameters.Add(u_facnit);
                    cmdInsert.Parameters.Add(u_facnom);
                    cmdInsert.Parameters.Add(u_direccion);
                    cmdInsert.Parameters.Add(u_telefonos);
                    cmdInsert.Parameters.Add(u_email);
                    cmdInsert.Parameters.Add(FechaDoc);
                    //cmdInsert.Parameters.Add(Status);
                    cmdInsert.Parameters.Add(indicator);
                    cmdInsert.Parameters.Add(docfiscal);
                    cmdInsert.Parameters.Add(pDocTotal);
                    cmdInsert.Parameters.Add(pListNum);
                    cmdInsert.Parameters.Add(pcodBodega);
                    cmdInsert.Parameters.Add(pNomPrestatario);
                    cmdInsert.Parameters.Add(pDirEntrega);
                    cmdInsert.Parameters.Add(pTelPrestatario);
                    cmdInsert.Parameters.Add(pTipoSolucion);
                    cmdInsert.Parameters.Add(pTrimestre);
                    cmdInsert.Parameters.Add(pSupervisor);
                    cmdInsert.Parameters.Add(pTelSupervisor);
                    cmdInsert.Parameters.Add(pContacto);
                    cmdInsert.Parameters.Add(nuevoDocEntry);


                    cnn.Open();


                    using (var transaction = cnn.BeginTransaction())
                    {
                        // var nDocEntry = cmdInsert.ExecuteScalar();
                        cmdInsert.ExecuteNonQuery();
                        //cmdInsert.ExecuteScalar();
                        //cotizacion.DocEntry = nDocEntry;

                        intDocEntry = int.Parse(cmdInsert.Parameters[":DocEntry"].Value.ToString());


                        foreach (CotizacionDet linea in cotizacion.CotizacionDetalle)
                        {
                            cmdDetalle.Parameters[":DocEntry"].Value = intDocEntry;
                            cmdDetalle.Parameters[":LineNum"].Value = Convert.ToInt32(linea.LineNum.ToString());
                            cmdDetalle.Parameters[":ItemCode"].Value = linea.ItemCode.ToString();
                            cmdDetalle.Parameters[":Dscription"].Value = linea.Dscription.ToString();
                            cmdDetalle.Parameters[":Price"].Value = Convert.ToDecimal(linea.Price.ToString());
                            cmdDetalle.Parameters[":Quantity"].Value = Convert.ToDecimal(linea.Quantity.ToString());
                            cmdDetalle.Parameters[":GTotal"].Value = Convert.ToDecimal(linea.GTotal.ToString());
                            cmdDetalle.Parameters[":WhsCode"].Value = WhsCode.ToString();
                            cmdDetalle.Parameters[":BodegaEntrega"].Value = WhsCode.ToString();
                            cmdDetalle.Parameters[":PrecioDescuento"].Value = Convert.ToDecimal(linea.Price.ToString());

                            cmdDetalle.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }

                    cnn.Close();

                    cmdInsert.Dispose();
                    cmdDetalle.Dispose();

                    cotizacion.CotizacionDetalle.Clear();
                    gridCotizacion.DataBind();

                    limpioDatos();

                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Cotización Guardada con éxito", "Documento Guardado", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                    //return;
                    
                }
                catch (Exception exc) //Module failed to load
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }

                

            }
        }


        protected void ASPxButtonBuscaPlantilla_Click(object sender, EventArgs e)
        {
            string nom_plantilla = ASPxComboBoxPlantilla.Text.ToString();
            try
            {
                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);
                OracleCommand cmdPlantilla = new OracleCommand($"Select DocNum_Base From T06_PLANTILLA_COTIZACION where NOM_PLANTILLA = '{nom_plantilla}'", cnn);

                cnn.Open();
                OracleDataReader dataReader;
                dataReader = cmdPlantilla.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    int DocNum_Base = int.Parse(dataReader.GetValue(0).ToString());

                    //buscarPlantillaGuardada(DocNum_Base);
                    buscoDatosCotizacionEnc(DocNum_Base);
                    buscoDatosCotizacionDet(DocNum_Base);
                }
                // string Nom_Plantilla = ASPxComboBoxPlantilla.Text.ToString();
                dataReader.Close();
                cnn.Close();

            }

            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void buscoDatosCotizacionEnc(int DocNumBuscar)
        {
            
            
            try
            {
                
                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);
                OracleCommand command = new OracleCommand($"Select A.CardCode || '-' || B.CardName Cliente, A.SlpCode || '-' || C.SlpName Vendedor, A.U_FacNit, A.U_FacNom, NVL(A.U_Direccion,' ') Direccion, NVL(A.U_Telefonos,' ') Telefono, NVL(A.Email_Contacto_Cliente, ' ') Email, A.DocNum, A.DocDate, A.DocStatus " +
                                                           $"From T06_OQUT A, T06_OCRD B, OSLP C  Where A.DocNum = {DocNumBuscar} " +
                                                           "AND A.CardCode = B.CardCode AND A.SlpCode = C.SlpCode", cnn);

                cnn.Open();
                OracleDataReader dataReader;
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    string CardCode = dataReader.GetString(0);
                    string SlpCode = dataReader.GetString(1);
                    string U_FacNit = dataReader.GetString(2);
                    string U_FacNom = dataReader.GetString(3);
                    string U_Direccion = dataReader.GetString(4);
                    string U_Telefonos = dataReader.GetString(5);
                    string Email_Contacto_Cliente = dataReader.GetString(6);
                    int DocNum = int.Parse(dataReader.GetValue(7).ToString());
                    DateTime DocDate = dataReader.GetDateTime(8);
                    string DocStatus = dataReader.GetString(9);

                    string DocNumFin = Convert.ToString(DocNum);
                    //ASPxTextBoxCliente.Text = CardCode;
                    ASPxGridLookup1.Value = CardCode;
                    //ASPxTextBoxVendedor.Text = SlpCode;
                    ASPxGridLookup2.Value = SlpCode;
                    ASPxTextBoxNIT.Text = U_FacNit;
                    ASPxTextBoxContacto.Text = U_FacNom;
                    ASPxTextBoxDireccion.Text = U_Direccion;
                    ASPxTextBoxTelefono.Text = U_Telefonos;
                    ASPxTextBoxEmail.Text = Email_Contacto_Cliente;
                    ASPxTextBoxNumDocumento.Value = DocNum;
                    ASPxTextBoxFecha.Value = DocDate;
                    ASPxTextBoxEstado.Text = DocStatus;
                }
                else
                {
                   // ASPxTextBoxCliente.Text = "";
                    //ASPxTextBoxVendedor.Value = "";
                    ASPxTextBoxNIT.Text = "";
                    ASPxTextBoxContacto.Text = "";
                    ASPxTextBoxDireccion.Text = "";
                    ASPxTextBoxTelefono.Text = "";
                    ASPxTextBoxEmail.Text = "";
                    ASPxTextBoxNumDocumento.Text = "";
                    ASPxTextBoxFecha.Text = "";
                    ASPxTextBoxEstado.Text = "";
                }
                dataReader.Close();
                cnn.Close();
                

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void buscoDatosCotizacionDet(int DocEntry)
        {
            cotizacion.CotizacionDetalle.Clear();
            CotizacionDet detalle = new CotizacionDet();
            
            string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
            // string strPrecio = $"Select Price From ITM1 Where PriceList = 2 And ItemCode = '{articulo}'";
            string strDatos = $"select TQ.DOCENTRY, TQ.LINENUM, TQ.ITEMCODE, T1.ITEMNAME, ROUND(T2.PRICE * 1.12, 2) , TQ.Quantity, TQ.Quantity *  T2.Price * 1.12 GTotal from T06_OQUT T0, T06_QUT1 TQ,OITM T1, ITM1 T2 where T0.DocEntry = TQ.DocEntry " +
                               $"and TQ.ITEMCODE = T1.ITEMCODE and TQ.ITEMCODE = T2.ITEMCODE and T2.PriceList = {priceList} and DocNum = {DocEntry} order by TQ.LINENUM";
            OracleConnection cnn = new OracleConnection(strConn);
            OracleCommand commandDet = new OracleCommand(strDatos, cnn);

            // decimal iva = 1.12M;
            try
            {
                cnn.Open();
                OracleDataReader dataReader;
                dataReader = commandDet.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        CotizacionDet c = new CotizacionDet();

                        c.DocEntry = int.Parse(dataReader.GetValue(0).ToString());
                        c.LineNum = int.Parse(dataReader.GetValue(1).ToString());
                        c.ItemCode = dataReader.GetString(2);
                        c.Dscription = dataReader.GetString(3);
                        c.Price = decimal.Parse(dataReader.GetValue(4).ToString());
                        c.Quantity = decimal.Parse(dataReader.GetValue(5).ToString());
                        c.GTotal = decimal.Parse(dataReader.GetValue(6).ToString());

                        cotizacion.CotizacionDetalle.Add(c);
                    }
                }
                gridCotizacion.DataBind();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                    cnn.Close();
            }
            gridCotizacion.CancelEdit();
        }

    }
}