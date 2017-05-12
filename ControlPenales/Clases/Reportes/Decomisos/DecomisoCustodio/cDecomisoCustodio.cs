using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cDecomisoCustodio
    {
        public int Id_Decomiso { get; set; }
        public int Id_Tipo_Empleado { get; set; }
        public int Id_Persona { get; set; }
        public int Id_Grupo_Tactico { get; set; }
        public string Grupo_Tactico { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
    }
}
