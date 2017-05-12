using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cVisitanteTramite
    {

        public int Anio { get; set; }
        public int Id_Imputado { get; set; }
        public string Nombre_Ingreso { get; set; }
        public string Paterno_Ingreso { get; set; }
        public string Materno_Ingreso { get; set; }
        public string Nombre_Visitante { get; set; }
        public string Paterno_Visitante { get; set; }
        public string Materno_Visitante { get; set; }
        public int NumeroVisita { get; set; }
        public int DiasTramite { get; set; }
        public string Parentesco { get; set; }
    }
}
