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


using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DotNetNuke.Services.Exceptions;
using Oracle.DataAccess.Client;
using System.Collections;

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
    public partial class Edit : CotizacionModuleBase
    {
        public static CotizacionEnc cotizacion;
        decimal precioSinIva = 0;
        int priceList;
        string verificarEstado;
        string estatusCoti;
        string fileName = "";
        string WhsCode;
        string CardFName;
       // string BodegaEntrega;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                if (!IsPostBack)
                {
                    cotizacion = new CotizacionEnc();
                    ASPxTextBoxBuscarCotizacion.Text = Request.QueryString["DocNum"];
                    int DocNumEditar = int.Parse(ASPxTextBoxBuscarCotizacion.Value.ToString());

                    buscoCotiEditar(DocNumEditar);

                    //PrepareDownload();
                    cargarAdjuntos(DocNumEditar);


                    if (DotNetNuke.Framework.AJAX.IsEnabled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterPostBackControl(Button1);
                        DotNetNuke.Framework.AJAX.RegisterPostBackControl(buttonVale);
                        DotNetNuke.Framework.AJAX.RegisterPostBackControl(ASPxGridViewAdjuntos);
                    }
                    

                    /*VERIFICO ROL SI PUEDE O NO AUTORIZAR*/
                    string strConnSAP = ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString;
                    SqlConnection conn = new SqlConnection(strConnSAP);

                    DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                    int userId = userInfo.UserID;
                    string userMail = userInfo.Email;
                    bool isInternal = userInfo.IsInRole("ffacsaInterno");
                    bool isExternal = userInfo.IsInRole("ffacsaExterno");
                    bool isInternalMayoreo = userInfo.IsInRole("ffacsaMayoreo");


                    //priceList = -1;

                    string strUserSql = $"Select T0.CardCode, T0.CardName, T0.ListNum, T0.SlpCode, T0.AddID, T1.Name, T1.Address, T1.Tel1, T1.E_MailL, T1.Position " +
                                            $"From OCRD T0 Inner Join OCPR T1 On T0.CardCode = T1.CardCode Where T1.E_MailL = '{userMail}'";


                    SqlCommand userCmm = new SqlCommand(strUserSql, conn);
                    SqlDataReader reader;

                    //string Cliente = "";
                    //string NombreContacto = "";
                    string Posicion = "";
                    try
                    {
                        conn.Open();
                        reader = userCmm.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            //Cliente = reader.GetString(0);
                            //NombreContacto = reader.GetString(5);
                            Posicion = reader.GetString(9);
                        }
                        conn.Close();
                    }
                    catch (Exception exc) //Module failed to load
                    {
                        Exceptions.ProcessModuleLoadException(this, exc);
                    }

                    if (Posicion == "Administrador" || Posicion == "Asistente Administrativa" || Posicion == "Asistente Administrativo")
                    {
                        ASPxMenuActualizar.Items.FindByName("buttonAutorizar").Enabled = true;
                    }
                    else
                    {
                        ASPxMenuActualizar.Items.FindByName("buttonAutorizar").Enabled = false;
                    }


                        /*--------------VERIFICO ESTADO------------*/

                    int DocNum = int.Parse(ASPxTextBoxBuscarCotizacion.Text);
                    string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                    OracleConnection cnn = new OracleConnection(strConn);
                    OracleCommand cmdEstadoCoti = new OracleCommand($"Select Comments, DocStatus From T06_OQUT Where DocNum = {DocNum}", cnn);
                    OracleCommand VerificoDatosEntrega = new OracleCommand($"Select Nombres From T06_OQUT_DATOS_ENTREGA Where DocNum_Ref = {DocNum}", cnn);

                    cnn.Open();
                    OracleDataReader dataReader;
                    OracleDataReader dataReader2;
                    dataReader = cmdEstadoCoti.ExecuteReader();
                    dataReader2 = VerificoDatosEntrega.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {

                            verificarEstado = dataReader.GetString(0);
                            estatusCoti = dataReader.GetString(1);
                            
                        }
                    }

                    dataReader.Close();

                    if (dataReader2.HasRows)
                    {
                        ASPxMenuActualizar.Items.FindByName("buttonDatosEntrega").Enabled = false;
                    }

                    
                    dataReader2.Close();

                    cnn.Close();
                    /*------------------------TERMINA VERIFICACION ---------------------------*/
                    if (verificarEstado == "REALIZADO EN WEB - AUTORIZADO POR HABITAT" && estatusCoti == "O")
                    {
                        ASPxMenuActualizar.Items.FindByName("buttonActualizar").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonDatosEntrega").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonEnviar").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonAutorizar").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonRechazar").Enabled = false;
                        ASPxTextBoxEstadoCoti.Text = "LISTO PARA FACTURAR";
                    }
                    else
                    {
                        if (estatusCoti == "O")
                        {
                            ASPxTextBoxEstadoCoti.Text = "PENDIENTE DE VALIDACIÓN";
                            ASPxMenuActualizar.Items.FindByName("buttonEnviar").Enabled = true;
                        }
                    }
                     if(estatusCoti == "I")
                     {
                        ASPxMenuActualizar.Items.FindByName("buttonActualizar").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonEnviar").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonAutorizar").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonRechazar").Enabled = false;
                        ASPxTextBoxEstadoCoti.Text = "REVISANDO EN FFACSA";
                     }

                    if (estatusCoti == "R")
                    {
                        ASPxMenuActualizar.Items.FindByName("buttonActualizar").Enabled = true;
                        ASPxMenuActualizar.Items.FindByName("buttonEnviar").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonAutorizar").Enabled = true;
                        ASPxTextBoxEstadoCoti.Text = "VALIDADO POR FFACSA";
                    }
                    if (estatusCoti == "C")
                    {
                        ASPxMenuActualizar.Items.FindByName("buttonActualizar").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonDatosEntrega").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonEnviar").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonAutorizar").Enabled = false;
                        ASPxMenuActualizar.Items.FindByName("buttonRechazar").Enabled = false;
                        ASPxTextBoxEstadoCoti.Text = "FACTURADO";
                    }

                    //if (estatusCoti == "C")
                    //{
                    //    ASPxMenuActualizar.Items.FindByName("buttonActualizar").Enabled = false;
                    //    ASPxMenuActualizar.Items.FindByName("buttonAutorizar").Enabled = false;
                    //    ASPxMenuActualizar.Items.FindByName("buttonEnviar").Enabled = false;
                    //    ASPxTextBoxEstadoCoti.Text = "ENVIADO A FFACSA";
                    //}


                }

                prepareData();
                
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cargarAdjuntos(int DocNum)

        {
            string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
            OracleConnection cnn1 = new OracleConnection(strConn);
            OracleCommand command = new OracleCommand($"Select Nom_Archivo, DocNum, Id_Doc From T06_OQUT_DOCUMENTOS Where DocNum =  '{DocNum}' order by id_doc", cnn1);
            DataTable dt = new DataTable();
            cnn1.Open();
            using (OracleDataAdapter a = new OracleDataAdapter(command))
            {
                a.Fill(dt);

            }
            cnn1.Close();

            ASPxGridViewAdjuntos.DataSource = dt;
            ASPxGridViewAdjuntos.DataBind();

        }


        private void prepareData()
        {
            string strConn = ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(strConn);

            DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
            int userId = userInfo.UserID;
            string userMail = userInfo.Email;
           
            priceList = -1;

            string strUserSql = "Select T0.ListNum, T0.U_Whscode, T0.CardFName " +
                                    $"From OCRD T0 Inner Join OCPR T1 On T0.CardCode = T1.CardCode Where T1.E_MailL = '{userMail}'";
            

            SqlCommand userCmm = new SqlCommand(strUserSql, cnn);
            SqlDataReader reader;

            try
            {
                cnn.Open();
                reader = userCmm.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    priceList = reader.GetInt16(0);
                    WhsCode = reader.GetString(1);
                    CardFName = reader.GetString(2);
                }
                else
                    priceList = 1;

                reader.Close();

               
                string strItemSql = "select T0.ItemCode, concat(T0.ItemName,'(',T0.U_ItemMarca,')') ItemName, T0.U_ItemCategoria, ROUND(T1.Price * 1.12, 2) Price " +
                     "from OITM T0 " +
                    "inner join ITM1 T1 on T0.ItemCode = T1.ItemCode " +
                    "where T0.ItemCode < 'Z' and T0.validFor = 'Y' and T0.SellItem = 'Y' " +
                    $"and T1.PriceList = {priceList} " +
                    "and T1.Price > 0.00 and T0.QryGroup3 = 'Y'";

                
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

        private void PrepareDownload()
        {
            int id = int.Parse(ASPxTextBoxBuscarCotizacion.Text);
            //string fileName = "";
            string strOra = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
            using (OracleConnection con = new OracleConnection(strOra))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "select ID_DOC, NOM_ARCHIVO from T06_OQUT_DOCUMENTOS where DOCNUM=:DocNum";
                    cmd.Parameters.Add(":DocNum", id);
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            sdr.Read();
                            {
                                fileName = sdr["NOM_ARCHIVO"].ToString();
                            }
                        }
                    }
                    con.Close();
                }
                /*
                if (fileName.Equals(""))
                {
                    fileDownLoad.Visible = false;
                    fileUpload.Visible = true;
                }
                else
                {
                    buttonVale.Text = $"Descargar Vale: {fileName}";
                    buttonVale.CommandArgument = ASPxTextBoxBuscarCotizacion.Text;

                    fileDownLoad.Visible = true;
                    fileUpload.Visible = false;
                }
                */
            }
        }

        protected void Upload(object sender, EventArgs e)
        {
            try
            {
               
                    if (File1.PostedFile.FileName.Equals(""))
                    {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "No ha seleccionado un Archivo para Cargar", "Error de Carga", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                    
                    }

                    string filename = Path.GetFileName(File1.PostedFile.FileName);
                    string contentType = File1.PostedFile.ContentType;
                    int idDoc = 0;
                    using (Stream fs = File1.PostedFile.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            byte[] bytes = br.ReadBytes((Int32)fs.Length);
                            string strOra = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                            using (Oracle.DataAccess.Client.OracleConnection cnn = new Oracle.DataAccess.Client.OracleConnection(strOra))
                            {
                                string strSql = "P06_INSERTA_DOC_COTIZACION_WEB";
                                using (Oracle.DataAccess.Client.OracleCommand cmd = new Oracle.DataAccess.Client.OracleCommand(strSql))
                                {
                                    cmd.Connection = cnn;
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    cmd.Parameters.Add(":P_DOCNUM", ASPxTextBoxBuscarCotizacion.Text);
                                    cmd.Parameters.Add(":P_NOM_ARCHIVO", filename);
                                    cmd.Parameters.Add(":P_TIPO_CONTENIDO", contentType);
                                    //cmd.Parameters.Add(":P_DOCUMENTO", bytes);
                                    cmd.Parameters.Add(":P_DOCUMENTO", OracleDbType.Blob, bytes.Length, bytes, ParameterDirection.Input);
                                    cmd.Parameters.Add(":P_ID_DOC", Oracle.DataAccess.Client.OracleDbType.Int32, idDoc, ParameterDirection.InputOutput);
                                    cnn.Open();
                                    cmd.ExecuteNonQuery();
                                    cnn.Close();
                                }
                            }
                        }
                    }
                Response.Redirect(ModuleContext.NavigateUrl(PortalSettings.ActiveTab.TabID, "Edit", false, "mid=" + ModuleId.ToString(), "DocNum=" + ASPxTextBoxBuscarCotizacion.Text));
                
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void buttonVale_Click(object sender, EventArgs e)
        {
            int id = int.Parse((sender as ASPxButton).CommandArgument);
            byte[] bytes;
            string fileName, contentType;
            string strOra = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
            using (OracleConnection con = new OracleConnection(strOra))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "select NOM_ARCHIVO, DOCUMENTO, TIPO_CONTENIDO from T06_OQUT_DOCUMENTOS where DOCNUM=:DocNum";
                    cmd.Parameters.Add(":DocNum", id);
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        sdr.Read();

                        bytes = (byte[])sdr["DOCUMENTO"];
                        contentType = sdr["TIPO_CONTENIDO"].ToString();
                        fileName = sdr["NOM_ARCHIVO"].ToString();
                    }
                    con.Close();
                }
            }
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.ContentType = contentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        private void descargaAdjunto(int idDoc)
        {
            byte[] bytes;
            //int idDoc = ASPxGridViewAdjuntos.rowsFocusedRowIndex
            string fileName, contentType;
            string strOra = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
            using (OracleConnection con = new OracleConnection(strOra))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = "select NOM_ARCHIVO, DOCUMENTO, TIPO_CONTENIDO from T06_OQUT_DOCUMENTOS where Id_doc=:idDoc";
                    cmd.Parameters.Add(":idDoc", idDoc);
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        sdr.Read();

                        bytes = (byte[])sdr["DOCUMENTO"];
                        contentType = sdr["TIPO_CONTENIDO"].ToString();
                        fileName = sdr["NOM_ARCHIVO"].ToString();
                    }
                    con.Close();
                }
            }
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.ContentType = contentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }


        protected void glCategory_Load(object sender, EventArgs e)
        {
            (sender as ASPxGridLookup).GridView.Width = new Unit(500, UnitType.Pixel);


        }
        //--------------------------------------
        protected void grid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            CotizacionDet detalle = new CotizacionDet();
            detalle.LineNum = cotizacion.CotizacionDetalle.Count + 1;
            detalle.ItemCode = e.NewValues["ItemCode"].ToString();
            detalle.Quantity = int.Parse(e.NewValues["Quantity"].ToString());


            string articulo = detalle.ItemCode;

            string strConn = ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString;
            string strDatos = $"Select ROUND(T1.Price * 1.12, 2) Price, T0.ItemName From OITM T0 Inner Join ITM1 T1 On T0.ItemCode = T1.ItemCode Where T0.ItemCode = '{ articulo }' And T1.PriceList = {priceList}";
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

                   // decimal precioArt = dataReader.GetDecimal(0);

                    decimal precioConIva = dataReader.GetDecimal(0);
                    precioSinIva = Convert.ToDecimal(precioConIva / 1.12M);

                    detalle.Price = dataReader.GetDecimal(0);
                    // detalle.Price = precioArt * iva;
                    detalle.Dscription = dataReader.GetString(1);

                    detalle.GTotal = int.Parse(e.NewValues["Quantity"].ToString()) * detalle.Price;

                    detalle.LineTotal = precioSinIva;

                    detalle.LineStatus = 1;
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
            string strDatos = $"Select ROUND(T1.Price * 1.12, 2) Price, T0.ItemName From OITM T0 Inner Join ITM1 T1 On T0.ItemCode = T1.ItemCode Where T0.ItemCode = '{ articulo }' And T1.PriceList = {priceList}";
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

        private void limpioDatos()
        {
            ASPxTextBoxPlantilla.Text = "";
            ASPxTextBoxCliente.Text = "";
            ASPxTextBoxVendedor.Text = "";
            ASPxTextBoxNIT.Text = "";
            ASPxTextBoxContacto.Text = "";
            ASPxTextBoxDireccion.Text = "";
            ASPxTextBoxTelefono.Text = "";
            ASPxTextBoxEmail.Text = "";
            ASPxTextBoxFecha.Text = "";
           // ASPxTextBoxEstado.Text = "";
            ASPxTextBoxPrestatario.Text = "";
            ASPxTextBoxDirEntrega.Text = "";
            ASPxTextBoxTelPrestatario.Text = "";
            ASPxTextBoxDPI1.Text = "";
            ASPxTextBoxDPI2.Text = "";
            ASPxTextBoxPersonaEntrega1.Text = "";
            ASPxTextBoxPersonaEntrega2.Text = "";
            ASPxTextBoxObservaciones1.Text = "";
            ASPxTextBoxObservaciones2.Text = "";


            //gridCotizacion.Columns.Clear();
        }

        private void buscoCotiEditar(int DocNum)
        {
            buscoDatosCotizacionEnc(DocNum);
            buscoDatosCotizacionDet(DocNum);

           //if  cotizacion.EnviadoFFACSA == true
        }

        private void buscoDatosCotizacionEnc(int DocNumBuscar)
        {
            
            try
            {
                
              //  int numCotiBuscar = Convert.ToInt32(ASPxTextBoxBuscarCotizacion.Value.ToString());

                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);
                OracleCommand command = new OracleCommand($"Select A.CardCode || '-' || B.CardName Cliente, A.SlpCode || '-' || C.SlpName Vendedor, A.U_FacNit, A.U_FacNom, NVL(A.U_Direccion,' ') Direccion, NVL(A.U_Telefonos,' ') Telefono, NVL(A.Email_Contacto_Cliente, ' ') Email, A.DocNum, A.DocDate, A.DocStatus, NVL(A.Nom_Prestatario, ' ') Nom_Prestatario, NVL(A.U_DireccionEntrega, ' ') U_DireccionEntrega, NVL(A.Tel_Prestatario, ' ') Tel_Prestatario " +
                                                           $"From T06_OQUT A, T06_OCRD B, OSLP C  Where A.DocNum = {DocNumBuscar} " + 
                                                           "AND A.CardCode = B.CardCode AND A.SlpCode = C.SlpCode", cnn);

                OracleCommand command2 = new OracleCommand( $"Select linenum, dpi, nombres, observaciones From T06_OQUT_DATOS_ENTREGA Where LineNum = 1 and DocNum_Ref = {DocNumBuscar}", cnn);
                OracleCommand command3 = new OracleCommand($"Select linenum, dpi, nombres, observaciones From T06_OQUT_DATOS_ENTREGA Where LineNum = 2 and DocNum_Ref = {DocNumBuscar}", cnn);

                cnn.Open();
                OracleDataReader dataReader;
                OracleDataReader dataReader2;
                OracleDataReader dataReader3;
                dataReader = command.ExecuteReader();
                dataReader2 = command2.ExecuteReader();
                dataReader3 = command3.ExecuteReader();
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
                    string NomPrestatario = dataReader.GetString(10);
                    string DirEntrega = dataReader.GetString(11);
                    string TelPrestatario = dataReader.GetString(12);

                   // string DocStatusOld = DocStatus;

                    //string DocNumFin = Convert.ToString(DocNum);
                    ASPxTextBoxCliente.Text = CardCode;
                    ASPxTextBoxVendedor.Text  = SlpCode;
                    ASPxTextBoxNIT.Text = U_FacNit;
                    ASPxTextBoxContacto.Text = U_FacNom;
                    ASPxTextBoxDireccion.Text = U_Direccion;
                    ASPxTextBoxTelefono.Text = U_Telefonos;
                    ASPxTextBoxEmail.Text = Email_Contacto_Cliente;
                    ASPxTextBoxBuscarCotizacion.Value = DocNum;
                    ASPxTextBoxFecha.Value = DocDate;
                    ASPxTextBoxDocStatusOld.Text = DocStatus;
                    ASPxTextBoxPrestatario.Text = NomPrestatario;
                    ASPxTextBoxDirEntrega.Text = DirEntrega;
                    ASPxTextBoxTelPrestatario.Text = TelPrestatario;
                }
                else
                {
                    ASPxTextBoxCliente.Text = "";
                    ASPxTextBoxVendedor.Value = "";
                    ASPxTextBoxNIT.Text = "";
                    ASPxTextBoxContacto.Text = "";
                    ASPxTextBoxDireccion.Text = "";
                    ASPxTextBoxTelefono.Text = "";
                    ASPxTextBoxEmail.Text = "";
                    ASPxTextBoxBuscarCotizacion.Text = "";
                    ASPxTextBoxFecha.Text = "";
                  //  ASPxTextBoxEstado.Text = "";
                    ASPxTextBoxPrestatario.Text = "";
                    ASPxTextBoxDirEntrega.Text = "";
                    ASPxTextBoxTelPrestatario.Text = "";
                }
                dataReader.Close();

                if (dataReader2.HasRows)
                {
                    dataReader2.Read();

                    ASPxTextBoxDPI1.Text = dataReader2.GetString(1);
                    ASPxTextBoxPersonaEntrega1.Text = dataReader2.GetString(2);
                    ASPxTextBoxObservaciones1.Text = dataReader2.GetString(3);

                }
                dataReader2.Close();

                if (dataReader3.HasRows)
                {
                    dataReader3.Read();
                    ASPxTextBoxDPI2.Text = dataReader3.GetString(1);
                    ASPxTextBoxPersonaEntrega2.Text = dataReader3.GetString(2);
                    ASPxTextBoxObservaciones2.Text = dataReader3.GetString(3);
                      
                }

                dataReader3.Close();
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
           // detalle.LineNum = cotizacion.CotizacionDetalle.Count;
            //.ItemCode = e.NewValues["ItemCode"].ToString();
            //detalle.Dscription = e.NewValues["Dscription"].ToString();
            //detalle.Price = decimal.Parse(e.NewValues["Price"].ToString());
            //detalle.Quantity = int.Parse(e.NewValues["Quantity"].ToString());


            // string articulo = detalle.ItemCode;

            string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
            // string strPrecio = $"Select Price From ITM1 Where PriceList = 2 And ItemCode = '{articulo}'";
            string strDatos = $"Select T0.DocEntry, T1.LineNum, T1.ItemCode, T1.Dscription,  T1.Price, T1.Quantity, T1.GTotal From T06_OQUT T0, T06_QUT1 T1 Where T0.DocEntry = T1.DOcEntry And DocNum = {DocEntry} Order By LineNum Asc";
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
       
        /*---------------- PROCEDIMIENTO PARA GUARDAR LA COTIZACION EN LA TABLA T06_PLANTILLA_COTIZACION ---------------*/
        private void guardaPlantillaCoti()
        {
            string Solucion = ASPxTextBoxPlantilla.Value.ToString();


            if (Solucion == "")
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Ingrese un Nombre para la Solución", " -- Error al guardar", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
            }

            string nom_plantilla = ASPxTextBoxPlantilla.Text;
            int DocNumBase = int.Parse(ASPxTextBoxBuscarCotizacion.Value.ToString());
            int Cod_Plantilla = 0;



            try
            {
                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);
                string sqlInsert = "P06_INSERTA_PLANTILLA_COTIZACION";

                OracleCommand cmdInsert = new OracleCommand(sqlInsert, cnn);
                cmdInsert.CommandType = CommandType.StoredProcedure;
                // cmdInsert.CommandType = CommandType.Text;


               
                OracleParameter pNomPlatilla = new OracleParameter();
                pNomPlatilla.DbType = DbType.String;
                pNomPlatilla.Value = nom_plantilla;
                pNomPlatilla.ParameterName = ":Nom_Plantilla";

                OracleParameter pDocNumBase = new OracleParameter();
                pDocNumBase.DbType = DbType.Int32;
                pDocNumBase.Value = DocNumBase;
                pDocNumBase.ParameterName = ":DocNumBase";

                OracleParameter pCodPlantilla = new OracleParameter();
                pCodPlantilla.DbType = DbType.Int32;
                pCodPlantilla.Value = Cod_Plantilla;
                pCodPlantilla.ParameterName = ":Cod_Plantilla";


                cmdInsert.Parameters.Add(pNomPlatilla);
                cmdInsert.Parameters.Add(pDocNumBase);
                cmdInsert.Parameters.Add(pCodPlantilla);



                cnn.Open();


                using (var transaction = cnn.BeginTransaction())
                {
                    // var nDocEntry = cmdInsert.ExecuteScalar();
                    cmdInsert.ExecuteNonQuery();
                    //cmdInsert.ExecuteScalar();
                    //cotizacion.DocEntry = nDocEntry;

                    Cod_Plantilla = int.Parse(cmdInsert.Parameters[":Cod_Plantilla"].Value.ToString());

                    
                    transaction.Commit();
                }

                cnn.Close();

                cmdInsert.Dispose();
                limpioDatos();
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Solución Guardada ", " -- Puede generar nuevas cotizaciones a partir de esta ", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            /*------------------------------*/
        }

       
        protected void ASPxMenuActualizar_ItemClick(object source, MenuItemEventArgs e)
        {
            
            if (e.Item.Name.Equals("buttonActualizar"))
            {
                decimal DocTotal = 0;
                foreach (CotizacionDet linea in cotizacion.CotizacionDetalle)
                {
                    DocTotal += Convert.ToDecimal(linea.GTotal.ToString());

                }

                int DocNumActualiza = int.Parse(ASPxTextBoxBuscarCotizacion.Text.ToString());
                /*LLAMO PROCEDIMIENTO*/
                actualizoCotizacion(DocTotal);

                cotizacion.CotizacionDetalle.Clear();
                gridCotizacion.DataBind();

               
                actualizoDatosEntrega(DocNumActualiza);
                limpioDatos();  
                                    
            }

            if (e.Item.Name.Equals("buttonDatosEntrega"))
            {
                int DocNumDatosEntrega = int.Parse(ASPxTextBoxBuscarCotizacion.Text.ToString());
                guardoDatosEntrega(DocNumDatosEntrega);
            }

            if (e.Item.Name.Equals("buttonGuardarPlantilla"))
            {
                string NomSolucion = ASPxTextBoxPlantilla.Value.ToString();

                if (NomSolucion == "" )
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Ingrese un Nombre para la Solución", " -- Error al guardar", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                }
               
                guardaPlantillaCoti();
                cotizacion.CotizacionDetalle.Clear();
                gridCotizacion.DataBind();

            }

            if (e.Item.Name.Equals("buttonEnviar"))
            {
                int DocNum = int.Parse(ASPxTextBoxBuscarCotizacion.Value.ToString());
                string DocStatusOld = ASPxTextBoxDocStatusOld.Value.ToString();

                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);
                OracleCommand cmdLineNum = new OracleCommand($"Select NVL(Max(LineNum),0) + 1 From T06_HISTORIA_ESTADOS_COTIZACION Where DocNum = {DocNum}", cnn);
                OracleCommand cmdCorrelativo = new OracleCommand($"Select NVL(Max(Correlativo),0) + 1 From T06_HISTORIA_ESTADOS_COTIZACION", cnn);

                cnn.Open();
                OracleDataReader dataReader;
                OracleDataReader dataReader2;

                dataReader = cmdLineNum.ExecuteReader();
                dataReader2 = cmdCorrelativo.ExecuteReader();
                dataReader.Read();
                dataReader2.Read();

                int LineNum = int.Parse(dataReader.GetValue(0).ToString());
                int Correlativo = int.Parse(dataReader2.GetValue(0).ToString());

                cnn.Close();

                enviarCotiFFACSA(DocNum);
                envioCorreoNotificacionNuevo(DocNum);
                insertoHistoricoEstados(DocNum, DocStatusOld, Correlativo, LineNum);


            }

            if (e.Item.Name.Equals("buttonAutorizar"))
            {
                string autorizaHabitat = "";
                try
                {
                    
                    string strConnSAP = ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString;
                    SqlConnection conn = new SqlConnection(strConnSAP);
                    DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                    int userId = userInfo.UserID;
                    string userMail = userInfo.Email;

                    string strUserAutoriza = $"Select T1.Name " +
                                                $"From OCRD T0 Inner Join OCPR T1 On T0.CardCode = T1.CardCode Where T1.E_MailL = '{userMail}'";

                    SqlCommand userCmm = new SqlCommand(strUserAutoriza, conn);
                    SqlDataReader reader;
                    conn.Open();
                    reader = userCmm.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        autorizaHabitat = reader.GetString(0);
                    }
                    conn.Close();

                    int DocNumAutoriza = int.Parse(ASPxTextBoxBuscarCotizacion.Text.ToString());
                    autorizaCotizacion(DocNumAutoriza, autorizaHabitat);
                    envioCorreoNotificacionCotiAutorizada(DocNumAutoriza);
                    
                    cotizacion.CotizacionDetalle.Clear();
                    gridCotizacion.DataBind();

                }
                catch (Exception exc) //Module failed to load
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
                
            }
            
            if(e.Item.Name.Equals("buttonRechazar"))
            {
                int DocNumAnular = int.Parse(ASPxTextBoxBuscarCotizacion.Text.ToString());
                anularCotizacion(DocNumAnular);
            }

            if (e.Item.Name.Equals("buttonImprimir"))
            {
                Response.Redirect(ModuleContext.NavigateUrl(PortalSettings.ActiveTab.TabID, "Print", false, "mid=" + ModuleId.ToString(), "DocNum=" + ASPxTextBoxBuscarCotizacion.Text));
            }

            if (e.Item.Name.Equals("buttonCerrar"))
            {
                Response.Redirect(ModuleContext.NavigateUrl(PortalSettings.ActiveTab.TabID, "", false, "mid=" + ModuleId.ToString()));

                cotizacion.CotizacionDetalle.Clear();
                gridCotizacion.DataBind();
                limpioDatos();
            }

            cotizacion.CotizacionDetalle.Clear();
            gridCotizacion.DataBind();

        }
        /* ACTUALIZO LOS DATOS DE ENTREGA */
        private void actualizoDatosEntrega(int DocNumRef)
        {

        }

        /********************* GUARDAR DATOS DE LA ENTREGA ******/
        private void guardoDatosEntrega(int DocNumRef)
        {
            int correlativo = 0;
            string nombre1 = ASPxTextBoxPersonaEntrega1.Text;
            string dpi1 = ASPxTextBoxDPI1.Text;
            string observaciones1 = ASPxTextBoxObservaciones1.Text;
            int LineNum1 = 1;

            string nombre2 = ASPxTextBoxPersonaEntrega2.Text;
            string dpi2 = ASPxTextBoxDPI2.Text;
            string observaciones2 = ASPxTextBoxObservaciones2.Text;
            int LineNum2 = 2;

            string strConnEntrega = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
            OracleConnection cnn = new OracleConnection(strConnEntrega);
            string sqlInsertEntrega = "P06_INSERTA_DATOS_ENTREGA_COTI";

            OracleCommand cmdInsertDatosEntrega = new OracleCommand(sqlInsertEntrega, cnn);
            cmdInsertDatosEntrega.CommandType = CommandType.StoredProcedure;

            OracleParameter PDocNumRef = new OracleParameter();
            PDocNumRef.DbType = DbType.Int32;
            PDocNumRef.Value = DocNumRef;
            PDocNumRef.ParameterName = ":DocNumRef";

            OracleParameter pNomPersona1 = new OracleParameter();
            pNomPersona1.DbType = DbType.String;
            pNomPersona1.Value = nombre1;
            pNomPersona1.ParameterName = ":NomPersona";

            OracleParameter pDPI1 = new OracleParameter();
            pDPI1.DbType = DbType.String;
            pDPI1.Value = dpi1;
            pDPI1.ParameterName = ":DPI";

            OracleParameter pObservaciones = new OracleParameter();
            pObservaciones.DbType = DbType.String;
            pObservaciones.Value = observaciones1;
            pObservaciones.ParameterName = ":Observaciones";

            OracleParameter pLineNum = new OracleParameter();
            pLineNum.DbType = DbType.Int32;
            pLineNum.Value = LineNum1;
            pLineNum.ParameterName = ":LineNum";

            OracleParameter pCorrelativo = new OracleParameter();
            pCorrelativo.DbType = DbType.Int32;
            pCorrelativo.Value = correlativo;
            pCorrelativo.ParameterName = ":Correlativo";

            cmdInsertDatosEntrega.Parameters.Add(PDocNumRef);
            cmdInsertDatosEntrega.Parameters.Add(pNomPersona1);
            cmdInsertDatosEntrega.Parameters.Add(pDPI1);
            cmdInsertDatosEntrega.Parameters.Add(pObservaciones);
            cmdInsertDatosEntrega.Parameters.Add(pLineNum);
            cmdInsertDatosEntrega.Parameters.Add(pCorrelativo);

            cnn.Open();

            using (var transaction = cnn.BeginTransaction())
            {
                // var nDocEntry = cmdInsert.ExecuteScalar();
                cmdInsertDatosEntrega.ExecuteNonQuery();
                //cmdInsert.ExecuteScalar();
                //cotizacion.DocEntry = nDocEntry;

                correlativo = int.Parse(cmdInsertDatosEntrega.Parameters[":Correlativo"].Value.ToString());

                transaction.Commit();
            }

            cnn.Close();

            cmdInsertDatosEntrega.Dispose();

            /*INGRESO SEGUNDA FILA DE CONTACTOS DE ENTEGA*/
            string strConnEntrega1 = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
            OracleConnection cnn1 = new OracleConnection(strConnEntrega1);
            string sqlInsertEntrega1 = "P06_INSERTA_DATOS_ENTREGA_COTI";

            OracleCommand cmdInsertDatosEntrega1 = new OracleCommand(sqlInsertEntrega1, cnn1);
            cmdInsertDatosEntrega1.CommandType = CommandType.StoredProcedure;
            // cmdInsert.CommandType = CommandType.Text;

            int DocNumReff = int.Parse(ASPxTextBoxBuscarCotizacion.Text.ToString());

            OracleParameter PDocNumRef1 = new OracleParameter();
            PDocNumRef1.DbType = DbType.Int32;
            PDocNumRef1.Value = DocNumReff;
            PDocNumRef1.ParameterName = ":DocNumRef";

            OracleParameter pNomPersona2 = new OracleParameter();
            pNomPersona2.DbType = DbType.String;
            pNomPersona2.Value = nombre2;
            pNomPersona2.ParameterName = ":NomPersona";

            OracleParameter pDPI2 = new OracleParameter();
            pDPI2.DbType = DbType.String;
            pDPI2.Value = dpi2;
            pDPI2.ParameterName = ":DPI";

            
            OracleParameter pObservaciones2 = new OracleParameter();
            pObservaciones2.DbType = DbType.String;
            pObservaciones2.Value = observaciones2;
            pObservaciones2.ParameterName = ":Observaciones";

            OracleParameter pLineNum2 = new OracleParameter();
            pLineNum2.DbType = DbType.Int32;
            pLineNum2.Value = LineNum2;
            pLineNum2.ParameterName = ":LineNum";

            OracleParameter pCorrelativo2 = new OracleParameter();
            pCorrelativo2.DbType = DbType.Int32;
            pCorrelativo2.Value = correlativo;
            pCorrelativo2.ParameterName = ":Correlativo";

            cmdInsertDatosEntrega1.Parameters.Add(PDocNumRef1);
            cmdInsertDatosEntrega1.Parameters.Add(pNomPersona2);
            cmdInsertDatosEntrega1.Parameters.Add(pDPI2);
            cmdInsertDatosEntrega1.Parameters.Add(pObservaciones2);
            cmdInsertDatosEntrega1.Parameters.Add(pLineNum2);
            cmdInsertDatosEntrega1.Parameters.Add(pCorrelativo2);

            cnn1.Open();

            using (var transaction1 = cnn1.BeginTransaction())
            {
                // var nDocEntry = cmdInsert.ExecuteScalar();
                cmdInsertDatosEntrega1.ExecuteNonQuery();
                //cmdInsert.ExecuteScalar();
                //cotizacion.DocEntry = nDocEntry;

                correlativo = int.Parse(cmdInsertDatosEntrega1.Parameters[":Correlativo"].Value.ToString());

                transaction1.Commit();
            }

            cnn1.Close();

            cmdInsertDatosEntrega1.Dispose();
            limpioDatos();
        }

        /*Se envia correo al presionar el botón enviar a ffacsa desde EDITAR*/
        private void envioCorreoNotificacionNuevo(int DocNum)
        {
            string msgSubject;
            string msgBody;
            string msgText;


            //string url = DotNetNuke.Services.Url.FriendlyUrl.FriendlyUrlProvider.Instance.ResolveClientUrl;


            string server = DotNetNuke.Entities.Host.Host.SMTPServer;
            string authentication = DotNetNuke.Entities.Host.Host.SMTPAuthentication;
            string password = DotNetNuke.Entities.Host.Host.SMTPPassword;
            string username = DotNetNuke.Entities.Host.Host.SMTPUsername;

            string url = DotNetNuke.Common.Globals.NavigateURL();

            msgSubject = "Creación de Cotización -- Correo enviado desde Portal FFACSA";

            msgText = "";
            msgText += "<table width=750>";
            msgText += "<tr>";
            msgText += $"<td>Se ha creado o actualizado una cotización de HABITAT <strong>{CardFName}</strong> desde el Portal Web , número <strong>{DocNum}</strong>  {url} -- Verifique en POS...</td>";
            msgText += "</tr>";
            msgText += "</table>";

            string openEmailFormatting = "";
            openEmailFormatting += "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
            openEmailFormatting += "<HTML><HEAD><TITLE></TITLE>";
            openEmailFormatting += "<META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
            openEmailFormatting += "</HEAD><BODY>";

            string closeEmailFormatting = "</BODY></HTML>";

            msgBody = openEmailFormatting + msgText + closeEmailFormatting;

            DotNetNuke.Services.Mail.Mail.SendMail("soporteweb@ffacsa.com", "ginay@ffacsa.com", "", "",
                DotNetNuke.Services.Mail.MailPriority.Normal, msgSubject,
                DotNetNuke.Services.Mail.MailFormat.Html, System.Text.Encoding.UTF8, msgBody, "",
                server, authentication, username, password);
        }

        private void envioCorreoNotificacionCotiAutorizada(int DocNum)
        {
            string msgSubject;
            string msgBody;
            string msgText;


            string server = DotNetNuke.Entities.Host.Host.SMTPServer;
            string authentication = DotNetNuke.Entities.Host.Host.SMTPAuthentication;
            string password = DotNetNuke.Entities.Host.Host.SMTPPassword;
            string username = DotNetNuke.Entities.Host.Host.SMTPUsername;

            string url = DotNetNuke.Common.Globals.NavigateURL();

            msgSubject = "Autorización de Cotización -- Correo enviado desde Portal FFACSA";

            msgText = "";
            msgText += "<table width=750>";
            msgText += "<tr>";
            msgText += $"<td>Se ha autorizado la cotización, número <strong>{DocNum}</strong> de HABITAT <strong>{CardFName}</strong> desde el Portal Web {url} -- Verifique en POS...</td>";
            msgText += "</tr>";
            msgText += "</table>";

            string openEmailFormatting = "";
            openEmailFormatting += "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
            openEmailFormatting += "<HTML><HEAD><TITLE></TITLE>";
            openEmailFormatting += "<META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
            openEmailFormatting += "</HEAD><BODY>";

            string closeEmailFormatting = "</BODY></HTML>";

            msgBody = openEmailFormatting + msgText + closeEmailFormatting;

            DotNetNuke.Services.Mail.Mail.SendMail("soporteweb@ffacsa.com", "ginay@ffacsa.com", "", "",
                DotNetNuke.Services.Mail.MailPriority.Normal, msgSubject,
                DotNetNuke.Services.Mail.MailFormat.Html, System.Text.Encoding.UTF8, msgBody, "",
                server, authentication, username, password);
        }

        private void anularCotizacion(int DocNum)
        {
            try
            {

                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);

                string sqlAnulaCoti = $"Update T06_OQUT SET DocStatus = :DocStatus, Comments = :Comments Where DocNum = {DocNum}";
                OracleCommand cmdAnulacion = new OracleCommand(sqlAnulaCoti, cnn);
                cmdAnulacion.CommandType = CommandType.Text;

                cnn.Open();

                using (var transaction = cnn.BeginTransaction())
                {


                    string DocStatus = "A";
                    string Comments = "REALIZADO EN WEB - RECHAZADO POR HABITAT";

                    OracleParameter pDocStatus = new OracleParameter();
                    pDocStatus.DbType = DbType.String;
                    pDocStatus.Value = DocStatus;
                    pDocStatus.ParameterName = ":DocStatus";

                    OracleParameter pComments = new OracleParameter();
                    pComments.DbType = DbType.String;
                    pComments.Value = Comments;
                    pComments.ParameterName = ":Comments";

                    cmdAnulacion.Parameters.Add(pDocStatus);
                    cmdAnulacion.Parameters.Add(pComments);

                    cmdAnulacion.ExecuteNonQuery();

                    transaction.Commit();
                }
                cnn.Close();

                cmdAnulacion.Dispose();
                cotizacion.CotizacionDetalle.Clear();
                limpioDatos();
                gridCotizacion.DataBind();

               
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Cotización Rechazada con Éxito", " -- Se rechazó la Cotización", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);

            }
        }

        private void autorizaCotizacion(int DocNum, string autorizaHabitat)
        {

            try
            {

                // id = int.Parse(ASPxTextBoxNumDocumento.Text);
                //string fileName = "";
                string strOra = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                using (OracleConnection con = new OracleConnection(strOra))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.CommandText = "select NOM_ARCHIVO from T06_OQUT_DOCUMENTOS where DOCNUM=:DocNum";
                        cmd.Parameters.Add(":DocNum", DocNum);
                        cmd.Connection = con;
                        con.Open();
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            if (sdr.HasRows)
                            {
                                sdr.Read();
                                {
                                    fileName = sdr["NOM_ARCHIVO"].ToString();
                                }
                            }
                        }

                        string sqlAutorizaHabitat = $"Update T06_OQUT Set autoriza_habitat = '{autorizaHabitat}' Where DocNum = {DocNum}";
                        OracleCommand cmdAutorizaHabitat = new OracleCommand(sqlAutorizaHabitat, con);

                        cmdAutorizaHabitat.ExecuteNonQuery();

                        con.Close();
                    }
                    if (fileName.Equals(""))
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "No se ha cargado ninguna Orden de Compra", "Error de Carga", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                        return;
                    }

                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Cotización Autorizada con Éxito ", " -- Listo para Facturar", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);

            }

            try
            {

                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);

                string sqlAutorizaCoti = $"Update T06_OQUT SET Comments = :CommentsAutoriza, DocStatus = :DocStatus Where DocNum = {DocNum}";
                OracleCommand cmdAutoriza = new OracleCommand(sqlAutorizaCoti, cnn);
                cmdAutoriza.CommandType = CommandType.Text;

                cnn.Open();

                using (var transaction = cnn.BeginTransaction())
                {


                    string comentarioAutorizado = "REALIZADO EN WEB - AUTORIZADO POR HABITAT";

                    OracleParameter pCommentsAutoriza = new OracleParameter();
                    pCommentsAutoriza.DbType = DbType.String;
                    pCommentsAutoriza.Value = comentarioAutorizado;
                    pCommentsAutoriza.ParameterName = ":CommentsAutoriza";

                    OracleParameter pDocStatus = new OracleParameter();
                    pDocStatus.DbType = DbType.String;
                    pDocStatus.Value = "O";
                    pDocStatus.ParameterName = ":DocStatus";

                    cmdAutoriza.Parameters.Add(pCommentsAutoriza);
                    cmdAutoriza.Parameters.Add(pDocStatus);

                    cmdAutoriza.ExecuteNonQuery();

                    transaction.Commit();
                }
                cnn.Close();

                cmdAutoriza.Dispose();
                cotizacion.CotizacionDetalle.Clear();
                limpioDatos();
                gridCotizacion.DataBind();

                ASPxMenuActualizar.Items.FindByName("buttonActualizar").Enabled = false;
                ASPxMenuActualizar.Items.FindByName("buttonEnviar").Enabled = false;
                ASPxMenuActualizar.Items.FindByName("buttonAutorizar").Enabled = false;
                ASPxMenuActualizar.Items.FindByName("buttonRechazar").Enabled = false;
                ASPxTextBoxEstadoCoti.Text = "LISTO PARA FACTURAR";

                

              //  DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Cotización Autorizada", "Autorizado con Éxito", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);

            }
        }

        private void actualizoCotizacion(decimal DocTotal)
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

            /*-------------- ENCABEZADO DE LA COTIZACION -----------------------*/
          
            cotizacion.NIT = ASPxTextBoxNIT.Value.ToString();
            cotizacion.Nombre = ASPxTextBoxContacto.Value.ToString();
            cotizacion.Direccion = ASPxTextBoxDireccion.Value.ToString();
            cotizacion.Telefono = ASPxTextBoxTelefono.Value.ToString();
            cotizacion.Email = ASPxTextBoxEmail.Value.ToString();
            cotizacion.NomPrestatario = ASPxTextBoxPrestatario.Text;
            cotizacion.DirEntrega = ASPxTextBoxDirEntrega.Text;
            cotizacion.TelPrestatario = ASPxTextBoxTelPrestatario.Text;

           // cotizacion.CodBodega = bodegaCotiza;
            cotizacion.DocNum = int.Parse(ASPxTextBoxBuscarCotizacion.Value.ToString());

            string NIT = cotizacion.NIT;
            string Nombre = cotizacion.Nombre;
            string Direccion = cotizacion.Direccion;
            string Telefono = cotizacion.Telefono;
            string Email = cotizacion.Email;
            string DocDate = cotizacion.DocDate;
            string DocStatus = cotizacion.DocStatus;
           // string Bodega = bodegaCotiza;
            int DocEntry = cotizacion.DocNum;
            decimal total = DocTotal;
            string Prestatario = cotizacion.NomPrestatario;
            string DireccionEntrega = cotizacion.DirEntrega;
            string TelPrestatario = cotizacion.TelPrestatario;



            try
            {

                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);
                string sqlInsert = "P06_ACTUALIZA_COTIZACION_WEB";

                OracleCommand cmdInsert = new OracleCommand(sqlInsert, cnn);
                cmdInsert.CommandType = CommandType.StoredProcedure;

               
                /*encabezado*/


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
                
                OracleParameter pDocTotal = new OracleParameter();
                pDocTotal.DbType = DbType.Decimal;
                pDocTotal.Value = total;
                pDocTotal.ParameterName = ":DocTotal";

                OracleParameter pNomPrestatario = new OracleParameter();
                pNomPrestatario.DbType = DbType.String;
                pNomPrestatario.Value = Prestatario;
                pNomPrestatario.ParameterName = ":NomPrestatario";

                OracleParameter pDirEntrega = new OracleParameter();
                pDirEntrega.DbType = DbType.String;
                pDirEntrega.Value = DireccionEntrega;
                pDirEntrega.ParameterName = ":DirEntrega";

                OracleParameter pTelPrestatario = new OracleParameter();
                pTelPrestatario.DbType = DbType.String;
                pTelPrestatario.Value = TelPrestatario;
                pTelPrestatario.ParameterName = ":TelPrestatario";

                OracleParameter pDocEntry = new OracleParameter();
                pDocEntry.DbType = DbType.Int32;
                pDocEntry.Value = DocEntry;
                pDocEntry.ParameterName = ":DocEntry";

                cmdInsert.Parameters.Add(u_facnit);
                cmdInsert.Parameters.Add(u_facnom);
                cmdInsert.Parameters.Add(u_direccion);
                cmdInsert.Parameters.Add(u_telefonos);
                cmdInsert.Parameters.Add(u_email);
                cmdInsert.Parameters.Add(pDocTotal);
                cmdInsert.Parameters.Add(pNomPrestatario);
                cmdInsert.Parameters.Add(pDirEntrega);
                cmdInsert.Parameters.Add(pTelPrestatario);
                cmdInsert.Parameters.Add(pDocEntry);


                cnn.Open();


                using (var transaction = cnn.BeginTransaction())
                {
                   
                    cmdInsert.ExecuteNonQuery();

                    foreach (CotizacionDet linea in cotizacion.CotizacionDetalle)
                    {

                        if (linea.LineStatus == 1) //Verifico si es Nueva Linea
                        {
                            string sqlDetalle = "insert into T06_QUT1 (DocEntry,LineNum, ItemCode, Dscription, Price, Quantity, WhsCode, Bodega_Entrega, Precio_Descto, LineTotal, U_TipoIva, Currency, VatPrcnt, VatSumSy, TaxCode, LineType, U_Descuento_ventas, Descto_Automatico, U_Descuento_Fact, GTOTAL ) ";
                            sqlDetalle = sqlDetalle + "values (:DocEntry,:LineNum, :ItemCode, :Dscription, :Price, :Quantity, :WhsCode, :BodegaEntrega, :PrecioDescuento, :LineTotal, :U_TipoIva, 'QTZ', 0.12, 1.12, 'IVA', 'B', 0 , 'S', 0, :GTotal)";
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

                            cmdDetalle.Parameters[":DocEntry"].Value = DocEntry;
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

                        if (linea.LineStatus == 2) //Verifico si es actulización de líneas
                        {
                            string sqlDetalleActualizado = $"Update T06_QUT1 SET ItemCode = :ItemCode, Dscription = :Dscription, Price = :Price, Quantity = :Quantity, GTotal = :GTotal Where DocEntry = {DocEntry} and LineNum = {linea.LineNum}";
                            OracleCommand cmdDetalleActualiza = new OracleCommand(sqlDetalleActualizado, cnn);


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

                            OracleParameter pGTotal = new OracleParameter();
                            pGTotal.DbType = DbType.Decimal;
                            pGTotal.ParameterName = ":GTotal";


                            cmdDetalleActualiza.Parameters.Add(pItemCode);
                            cmdDetalleActualiza.Parameters.Add(pDscription);
                            cmdDetalleActualiza.Parameters.Add(pPrice);
                            cmdDetalleActualiza.Parameters.Add(pQuantity);
                            cmdDetalleActualiza.Parameters.Add(pGTotal);

                            cmdDetalleActualiza.Parameters[":ItemCode"].Value = linea.ItemCode.ToString();
                            cmdDetalleActualiza.Parameters[":Dscription"].Value = linea.Dscription.ToString();
                            cmdDetalleActualiza.Parameters[":Price"].Value = Convert.ToDecimal(linea.Price.ToString());
                            cmdDetalleActualiza.Parameters[":Quantity"].Value = Convert.ToDecimal(linea.Quantity.ToString());
                            cmdDetalleActualiza.Parameters[":GTotal"].Value = Convert.ToDecimal(linea.GTotal.ToString());

                            cmdDetalleActualiza.ExecuteNonQuery();
                        }
                        if (linea.LineStatus == 3) //Verifico si están eliminando la línea
                        {

                        }
                        
                    }

                    transaction.Commit();
                }

                cnn.Close();

                cmdInsert.Dispose();
              
                cotizacion.CotizacionDetalle.Clear();
                gridCotizacion.DataBind();

                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Cotización Actualizada Correctamente", " - Actualizado", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void enviarCotiFFACSA(int DocNum)
        {
            //Se cambia a status "O"
                        
            string DocStatus = "I";
            string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
            OracleConnection cnn = new OracleConnection(strConn);

            try
            {               
               cnn.Open();
               string sqlActualizoEstado = $"Update T06_OQUT Set DocStatus = '{DocStatus}' Where DocNum = {DocNum}";
               OracleCommand cmdEstado = new OracleCommand(sqlActualizoEstado, cnn);

                cmdEstado.ExecuteNonQuery();
               
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
            limpioDatos();
            ASPxTextBoxEstadoCoti.Text = "ENVIADO A FFACSA";

            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Enviado a FFACSA para revisión, no podrá modificarla hasta ser revisado ", " -- Enviado al asesor de ventas FFACSA", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
        }
        /*HISTORICO DE ESTADOS*/
        private void insertoHistoricoEstados(int DocNum, string DocStatusOld, int Correlativo, int LineNum)
        {
            //int Correlativo = 0;
            string CodEstadoN = "I";

            DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
            string UsuEnvFF = userInfo.Email;
            try
            {
                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);
                string sqlInsert = "P06_INSERTA_CAMBIOS_ESTADOS_COTI_WEB";


                OracleCommand cmdInsert = new OracleCommand(sqlInsert, cnn);
                cmdInsert.CommandType = CommandType.StoredProcedure;

                OracleParameter pCorrelativo = new OracleParameter();
                pCorrelativo.DbType = DbType.Int32;
                pCorrelativo.Value = Correlativo;
                pCorrelativo.ParameterName = ":Correlativo";

                OracleParameter pLineNum = new OracleParameter();
                pLineNum.DbType = DbType.Int32;
                pLineNum.Value = LineNum;
                pLineNum.ParameterName = ":LineNum";

                OracleParameter pDocNum = new OracleParameter();
                pDocNum.DbType = DbType.Int32;
                pDocNum.Value = DocNum;
                pDocNum.ParameterName = ":DocNum";

                OracleParameter pDocStatusOld = new OracleParameter();
                pDocStatusOld.DbType = DbType.String;
                pDocStatusOld.Value = DocStatusOld;
                pDocStatusOld.ParameterName = ":CodEstadoOld";

                OracleParameter pCodEstadoN = new OracleParameter();
                pCodEstadoN.DbType = DbType.String;
                pCodEstadoN.Value = CodEstadoN;
                pCodEstadoN.ParameterName = ":CodEstadoN";

                OracleParameter pUsuEnvFF = new OracleParameter();
                pUsuEnvFF.DbType = DbType.String;
                pUsuEnvFF.Value = UsuEnvFF;
                pUsuEnvFF.ParameterName = ":UsuEnvFF";

                cmdInsert.Parameters.Add(pCorrelativo);
                cmdInsert.Parameters.Add(pLineNum);
                cmdInsert.Parameters.Add(pDocNum);
                cmdInsert.Parameters.Add(pDocStatusOld);
                cmdInsert.Parameters.Add(pCodEstadoN);
                cmdInsert.Parameters.Add(pUsuEnvFF);

                cnn.Open();

                using (var transaction = cnn.BeginTransaction())
                {
                    // var nDocEntry = cmdInsert.ExecuteScalar();
                    cmdInsert.ExecuteNonQuery();
                    //cmdInsert.ExecuteScalar();
                    //cotizacion.DocEntry = nDocEntry;

                    //Correlativo = int.Parse(cmdInsert.Parameters[":Correlativo"].Value.ToString());

                    
                    transaction.Commit();
                }

                cnn.Close();
                cmdInsert.Dispose();
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Histórico guardado", "Guardado con exito", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);

            }
        }

        protected void gridCotizacion_ToolbarItemClick(object source, DevExpress.Web.Data.ASPxGridViewToolbarItemClickEventArgs e)
        {
            int Cotizacion = int.Parse(ASPxTextBoxBuscarCotizacion.Value.ToString());
            gridCotizacion.SettingsExport.FileName = $"Cotizacion Numero {Cotizacion}";
        }

        protected void buttonEditRow_Click(object sender, EventArgs e)
        {
            int id = int.Parse(ASPxGridViewAdjuntos.GetRowValues(ASPxGridViewAdjuntos.FocusedRowIndex, "ID_DOC").ToString());

            //int idDoc = int.Parse(ASPxGridViewAdjuntos.FocusedRowIndex.ToString());
           // DevExpress.Web.ASPxButton button = sender as DevExpress.Web.ASPxButton;

           // int idDoc = int.Parse(ASPxGridView.GetRowValues().ToString());

            //Response.Redirect(ModuleContext.NavigateUrl(PortalSettings.ActiveTab.TabID, "Edit", false, "mid=" + ModuleId.ToString(), "DocNum=" + button.Text));
            descargaAdjunto(id);   
        }
    }
}