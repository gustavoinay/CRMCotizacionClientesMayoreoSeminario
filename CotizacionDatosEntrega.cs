using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ffacsa.Modules.Cotizacion
{
    public class CotizacionDatosEntrega
    {
        public int DocNumRef { get; set; }
        public string Nombres { get; set; }
        public string DPI { get; set; }
        public string observaciones { get; set; }

       // public List<CotizacionDatosEntrega> CotizacionEntrega = new List<CotizacionDatosEntrega>();
    }
    //public List<CotizacionDatosEntrega> datosEntrega = new List<CotizacionDatosEntrega>();
}