using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cReporteGrupoParticipante
    {
        public string Grupo { set; get; }
        public string Eje { set; get; }
        public string Programa { set; get; }
        public string Actividad { set; get; }
        public string Responsable { set; get; }
    }

    public class cReporteGrupoParticipanteInterno
    {
        public string Expediente { set; get; }
        public string Nombre { set; get; }
        public string Ubicacion { set; get; }
    }
}
