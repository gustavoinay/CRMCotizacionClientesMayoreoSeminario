using DotNetNuke.Services.Exceptions;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ffacsa.Modules.Cotizacion
{
    public partial class Print : CotizacionModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int DocNum = int.Parse(Request.QueryString["DocNum"]);

                DataSet ds = new DataSet();

                string strConn = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
                OracleConnection cnn = new OracleConnection(strConn);
                OracleDataAdapter data = new OracleDataAdapter();

                string textSQL = $"Select * From V06_REPORTE_HABITAT Where DocNum = {DocNum}";

                cnn.Open();
                data.SelectCommand = new OracleCommand();
                data.SelectCommand.CommandText = textSQL;
                data.SelectCommand.CommandType = CommandType.Text;
                data.SelectCommand.Connection = cnn;

                data.Fill(ds);
                cnn.Close();


                // Cargar el reporte
                CotizacionHabitat reporte = new CotizacionHabitat();

                // Asignarle los datos del objeto al DataSource del Reporte
                reporte.DataSource = ds;

                ASPxWebDocumentViewer1.OpenReport(reporte);                

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}