using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cImputados
    {
        public string centro {set;get;}
        public string anio { set; get; }
        public string folio { set; get; }
        public string paterno { set; get; }
        public string materno { set; get; }
        public string nombre { set; get; }
        public string control { set; get; }
        public IMPUTADO imputado { set; get; }
    }
}
