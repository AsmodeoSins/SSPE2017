using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class ReporteListaGruposActivos
    {
        public string Eje { set; get; }
        public string Programa { set; get; }
        public string Actividad { set; get; }
    }

    public class ReporteDetalleGruposActivos
    {
        public string Grupo { set; get; }
        public string Responsable { set; get; }
        public string Fecha_Inicio { set; get; }
        public string Fecha_Fin { set; get; }
        public string Departamento { set; get; }
        public string Recurrencia { set; get; }
    }
}
