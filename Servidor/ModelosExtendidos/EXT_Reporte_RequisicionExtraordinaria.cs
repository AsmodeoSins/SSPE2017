using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_Reporte_RequisicionExtraordinaria
    {
        public string NOMBRE { get; set; }
        public string DESCRIPCION { get; set; }
        public string PRESENTACION { get; set; }
        public string UNIDAD_MEDIDA { get; set; }
        public int? CANTIDAD { get; set; }
        public string CATEGORIA { get; set; }
    }
}
