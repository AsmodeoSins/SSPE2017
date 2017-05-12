using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class ReporteHorarioGrupo
    {
        public string ID_GRUPO { get; set; }

        public string Eje { get; set; }
        public string Programa { get; set; }
        public string Actividad { get; set; }
        public string Grupo { get; set; }
        public string Area { get; set; }
        public DateTime? dthora_inicio { get; set; }
        public string hora_inicio { get; set; }
        public string hora_termino { get; set; }
        public string Estatus { get; set; }
    }
}
