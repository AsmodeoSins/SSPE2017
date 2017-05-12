using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cTiempoTramiteVisita
    {
        public DateTime Fecha { get; set; }
        public string UPaterno { get; set; }
        public string UMaterno { get; set; }
        public string UNombre { get; set; }
        public string VPaterno { get; set; }
        public string VMaterno { get; set; }
        public string VNombre { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public double? Tiempo { get; set; }
        public TimeSpan aux { get; set; }
        public string Diferencia { get; set; }
    }

    public class cTiempoTramiteVisitaPromedio
    {
        public string Promedio { get; set; }
    }
}
