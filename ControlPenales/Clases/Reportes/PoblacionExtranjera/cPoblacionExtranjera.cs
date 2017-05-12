using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cPoblacionExtranjera
    {
        public string Pais { get; set; }
        public string Causa { get; set; }
        public int ComunProcesado { get; set; }
        public int SinFueroSentenciado { get; set; }
        public int SinFueroIndiciado { get; set; }
        public int SinFueroProcesado { get; set; }
    }
}
