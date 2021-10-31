using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ffacsa.Modules.Cotizacion
{
    public class CotizacionEnc
    {
        public string CardCode { get; set; }
        public string SlpCode { get; set; }
        public string NIT { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public int DocNum { get; set; }
        public string DocDate { get; set; }
        public string DocStatus { get; set; }
        public string CodBodega { get; set; }
        public string DirEntrega { get; set; }
        public string NomPrestatario { get; set; }
        public string TelPrestatario { get; set; }
        public bool EnviadoFFACSA { get; set; }
        public int DocEntry { get; set; }

        public List<CotizacionDet> CotizacionDetalle = new List<CotizacionDet>();
        public List<CotizacionDatosEntrega> CotizacionEntrega = new List<CotizacionDatosEntrega>();


    }
}