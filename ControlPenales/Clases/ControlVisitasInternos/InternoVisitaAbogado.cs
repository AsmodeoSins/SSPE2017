using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class InternoVisitaAbogado
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public short ID_IMPUTADO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public short TIPO_VISITANTE { get; set; }
        public string NOMBRE_VISITANTE { get; set; }
        public bool ACTIVA { get; set; }
    }
}
