using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ffacsa.Modules.Cotizacion
{
    public class CotizacionDet
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
        public decimal GTotal { get; set; }
        public int LineStatus { get; set; }
        public string whscode { get; set; }
        public decimal OnHand { get; set; }
    }
}