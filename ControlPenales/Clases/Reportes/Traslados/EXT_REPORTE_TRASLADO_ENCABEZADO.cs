using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class EXT_REPORTE_TRASLADO_ENCABEZADO
    {
        public string CENTRO_ORIGEN { get; set; }
        public DateTime FEC_TRASLADO { get; set; }
        public byte[] LOGO_BC { get; set; }
        public string TITULO { get; set; }
        public string RANGO_FECHAS { get; set; }
    }
}
