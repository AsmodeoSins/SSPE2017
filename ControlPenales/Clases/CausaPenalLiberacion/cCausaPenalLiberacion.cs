using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cCausaPenalLiberacion
    {
        public CAUSA_PENAL CausaPenal {set;get;}
        public short? Anio {set;get;}
        public int? Folio { set; get; }
        public string Juzgado { set; get; }
        public DateTime? Fecha { set; get; }
        private string Estatus { set; get; }
    }
}
