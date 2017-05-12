using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class TratamientoHistorial
    {
        public string MEDICAMENTO { get; set; }
        public string FORMULA_MEDICA { get; set; }
        public string PRESENTACION { get; set; }
        public decimal? CANTIDAD { get; set; }
        public string UNIDAD_MEDIDA { get; set; }
        public decimal? DURACION { get; set; }
        public bool MANANA { get; set; }
        public bool TARDE { get; set; }
        public bool NOCHE { get; set; }
        public string OBSERVACIONES { get; set; }
    }
}
