using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cDecomisoCustodioVisitante
    {
        public int Id_Decomiso { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string FechaRegistro { get; set; }
        public string Sexo { get; set; }
        public string TipoVisitante { get; set; }
        public string Estatus { get; set; }
        public string Discapacitado { get; set; }
    }
}
