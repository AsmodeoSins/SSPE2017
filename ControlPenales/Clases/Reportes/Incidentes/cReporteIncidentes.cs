using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cReporteIncidentes
    {
        public string INCIDENTE { get; set; }
        public int COMUN_PROCESADO { get; set; }
        public int COMUN_SENTENCIADO { get; set; }
        public int SIN_FUERO_IMPUTADO { get; set; }
        public int SIN_FUERO_PROCESADO { get; set; }
    }

    public class cReporteIncidenteGrafica
    {
        public string TIPO { get; set; }
        public int VALOR { get; set; }
    }
}
