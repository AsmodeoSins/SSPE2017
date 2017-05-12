using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cPoblacionInternos
    {
        public string Genero { get; set; }
        public string Fuero { get; set; }
        public string ClasificacionJuridica { get; set; }
        public int Id_Imputado { get; set; }
        public int Id_Anio { get; set; }
        public int Id_Centro { get; set; }
    }
}