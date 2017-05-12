using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cLiberacionBiometrica
    {
        public LIBERACION Liberacion {set;get;}
        public INGRESO Ingreso { set; get; }
        public IMPUTADO Imputado { set; get; }
        public byte[] ImagenEgreso { set; get;}
    }
}
