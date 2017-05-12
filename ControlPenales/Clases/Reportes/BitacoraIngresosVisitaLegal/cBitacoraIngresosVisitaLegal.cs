using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cBitacoraIngresosVisitaLegal
    {
        public string FechaHoraEntrada { set; get; }
        public string FechaHoraSalida { set; get; }
        public string Expediente { set; get; }
        public string Interno { set; get; }
        public string Abogado { set; get; }
    }


    public class cBitacoraVisitaFamiliar
    {
        public string HoraIngreso { set; get; }
        public string NoVisitante { set; get; }
        public string Visitante { set; get; }
        public string Expendiente { set; get; }
        public string Interno { set; get; }
        public string Ubicacion { set; get; }
        public string AutorizoEntrada { set; get; }
        public string HoraSalida { set; get; }
        public string AutorizoSalida { set; get; }
        public string TipoVisita { set; get; }

    }
}
