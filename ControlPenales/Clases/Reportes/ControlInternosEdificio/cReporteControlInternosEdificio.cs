using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class ReporteControlInternosEdificio
    {
        public string Expediente { get; set; }
        public string Nombre { get; set; }
        public string Ubicacion { get; set; }
        public string UbicacionActual { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public string AutorizaSalida { get; set; }
        public string Area { get; set; }
        public string MotivoSalida { get; set; }
        public int TotalSalida { get; set; }
        public int TotalEntrada { get; set; }
        public int TotalPendiente { get; set; }
        public int EstatusUbicacion { get; set; }
    }

    public class cControlInternosEdificio
    {
        public string EXPEDIENTE { get; set; }
        public string NOMBRE { get; set; }
        public string UBICACION { get; set; }
        public string FECHA { get; set; }
        public string AREA { get; set; }
        public DateTime? SALIDA { get; set; }
        public DateTime? ENTRADA { get; set; }
        public string MOTIVO { get; set; }
    }
}
