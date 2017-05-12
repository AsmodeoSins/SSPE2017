using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cInternoAbogado
    {
        public int Anio { get; set; }
        public int Id_Imputado { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }


        public int Id_Abogado { get; set; }
        public string Nombre_Abogado { get; set; }
        public string Paterno_Abogado { get; set; }
        public string Materno_Abogado { get; set; }
        public string Estatus { get; set; }
        public bool esTitular { get; set; }
        public string Observaciones { get; set; }
    }
}
