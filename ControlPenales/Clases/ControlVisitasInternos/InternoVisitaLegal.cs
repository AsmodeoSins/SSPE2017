using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class InternoVisitaLegal
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public short ID_IMPUTADO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public bool PERMITIR { get; set; }
        public bool HABILITAR { get; set; }
    }
}
