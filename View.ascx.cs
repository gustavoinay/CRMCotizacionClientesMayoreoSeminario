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
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;

using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;
using System.Data.SqlClient;

namespace ffacsa.Modules.Cotizacion
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from CotizacionModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : CotizacionModuleBase, IActionable
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            string strConnSAP = ConfigurationManager.ConnectionStrings["SAPDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(strConnSAP);

            DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
            int userId = userInfo.UserID;
            string userMail = userInfo.Email;
            bool isInternal = userInfo.IsInRole("ffacsaInterno");
            bool isExternal = userInfo.IsInRole("ffacsaExterno");
            bool isInternalMayoreo = userInfo.IsInRole("ffacsaMayoreo");


            //priceList = -1;

            string strUserSql = $"Select T0.CardCode, T0.CardName, T0.ListNum, T0.SlpCode, T0.AddID, T1.Name, T1.Address, T1.Tel1, T1.E_MailL, T1.Position " +
                                    $"From OCRD T0 Inner Join OCPR T1 On T0.CardCode = T1.CardCode Where T1.E_MailL = '{userMail}'";

           
            SqlCommand userCmm = new SqlCommand(strUserSql, cnn);
            SqlDataReader reader;

            string Cliente = "";
            string NombreContacto = "";
            string Posicion = "";
            try
            {
                cnn.Open();
                reader = userCmm.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    Cliente = reader.GetString(0);
                    NombreContacto = reader.GetString(5);
                    Posicion = reader.GetString(9);
                }
                cnn.Close();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            leerCotizaciones(Cliente, NombreContacto, Posicion);

        }

        private void  leerCotizaciones(string Cliente, string NombreContacto, string Posicion)
        {
            try
            {
                if (Posicion == "Administrador" || Posicion == "Asistente Administrativa" || Posicion == "Asistente Administrativo")
                {
                    string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                    OracleConnection cnn1 = new OracleConnection(strConn);
                    OracleCommand command = new OracleCommand($"Select DocNum, DocDate, CardCode, CardName, U_FacNit, U_FacNom, U_Telefonos, Nom_Prestatario, DocTotal, Des_Estado, NumFactura, Numero_Electronico, Comments " +
                                                                $"From V06_COTI_FACTURA Where CardCode = '{Cliente}'", cnn1);
                    DataTable dt = new DataTable();
                    cnn1.Open();
                    using (OracleDataAdapter a = new OracleDataAdapter(command))
                    {
                        a.Fill(dt);

                    }
                    cnn1.Close();

                    ASPxGridView1.DataSource = dt;
                    ASPxGridView1.DataBind();
                }
                else
                {
                    string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                    OracleConnection cnn1 = new OracleConnection(strConn);
                    OracleCommand command = new OracleCommand($"Select DocNum, DocDate, CardCode, CardName, U_FacNit, U_FacNom, U_Telefonos, Nom_Prestatario, DocTotal, Des_Estado, NumFactura, Numero_Electronico, Comments " +
                                                                $"From V06_COTI_FACTURA Where CardCode = '{Cliente}' And  U_FacNom = '{NombreContacto}'", cnn1);

                    DataTable dt = new DataTable();
                    cnn1.Open();
                    using (OracleDataAdapter a = new OracleDataAdapter(command))
                    {
                        a.Fill(dt);

                    }
                    cnn1.Close();

                    ASPxGridView1.DataSource = dt;
                    ASPxGridView1.DataBind();
                }


            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                    {
                        {
                            GetNextActionID(), Localization.GetString("EditModule", LocalResourceFile), "", "", "",
                            EditUrl(), false, SecurityAccessLevel.Edit, true, false
                        }
                    };
                return actions;
            }
        }
        protected void ASPxMenu1_ItemClick(object source, DevExpress.Web.MenuItemEventArgs e)
        {
            if (e.Item.Name == "buttonNew")
            {
                Response.Redirect(ModuleContext.NavigateUrl(PortalSettings.ActiveTab.TabID, "New", false, "mid=" + ModuleId.ToString()));
            }

            if (e.Item.Name.Equals("buttonExito"))
            {
                string msgSubject;
                string msgBody;
                string msgText;

                string server = DotNetNuke.Entities.Host.Host.SMTPServer;
                string authentication = DotNetNuke.Entities.Host.Host.SMTPAuthentication;
                string password = DotNetNuke.Entities.Host.Host.SMTPPassword;
                string username = DotNetNuke.Entities.Host.Host.SMTPUsername;

                msgSubject = "Correo enviado desde Portal FFACSA";

                msgText = "";
                msgText += "<table width=625>";
                msgText += "<tr>";
                msgText += "<td>Email Body Text Goes Here...</td>";
                msgText += "</tr>";
                msgText += "</table>";

                string openEmailFormatting = "";
                openEmailFormatting += "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
                openEmailFormatting += "<HTML><HEAD><TITLE></TITLE>";
                openEmailFormatting += "<META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
                openEmailFormatting += "</HEAD><BODY>";

                string closeEmailFormatting = "</BODY></HTML>";

                msgBody = openEmailFormatting + msgText + closeEmailFormatting;

                DotNetNuke.Services.Mail.Mail.SendMail("afuentes@ffacsa.com", "ginay@ffacsa.com", "afuentes@ffacsa.com", "",
                    DotNetNuke.Services.Mail.MailPriority.Normal, msgSubject,
                    DotNetNuke.Services.Mail.MailFormat.Html, System.Text.Encoding.UTF8, msgBody, "",
                    server, authentication, username, password);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "toastr_message", "toastr.success('Mensage de Exito','Exito')", true);
            }
            
        }

        protected void buttonEditRow_Click(object sender, EventArgs e)
        {
            DevExpress.Web.ASPxButton button = sender as DevExpress.Web.ASPxButton;

            Response.Redirect(ModuleContext.NavigateUrl(PortalSettings.ActiveTab.TabID, "Edit", false, "mid=" + ModuleId.ToString(), "DocNum=" + button.Text));
        }

    }
}