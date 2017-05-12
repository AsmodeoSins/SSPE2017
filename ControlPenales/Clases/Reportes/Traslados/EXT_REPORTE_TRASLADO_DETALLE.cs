using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class EXT_REPORTE_TRASLADO_DETALLE
    {
        public DateTime FEC_TRASLADO { get; set; }
        public string EXPEDIENTE { get; set; }
        public string NOMBRECOMPLETO { get; set; }
        public string CERESO_DESTINO { get; set; }
        public string MOTIVO_TRASLADO { get; set; }
        public string UBICACION { get; set; }
        public string MUNICIPIO_DESTINO { get; set; }
        public string TIPO_MOVIMIENTO { get; set; }
    }
}
