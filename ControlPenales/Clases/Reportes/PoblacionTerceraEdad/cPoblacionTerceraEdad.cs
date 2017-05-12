using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cPoblacionTerceraEdad
    {
        public string Rango { get; set; }
        public string Edad { get; set; }
        public string Causa { get; set; }
        public int ComunImpMasc { get; set; }
        public int ComunIndicMasc { get; set; }
        public int ComunProcMasc { get; set; }
        public int ComunSentMasc { get; set; }
        public int FederalProcFem { get; set; }
        public int FederalProcMasc { get; set; }
        public int FederalSentMasc { get; set; }
        public int FederalSentFem { get; set; }
        public int SinFueroProcMasc { get; set; }
        public int Orden { get; set; }
    }
}
