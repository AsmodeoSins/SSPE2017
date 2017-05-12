using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GESAL.Clases.Misc
{
    public class TipoDocumento
    {
        public short ID_TIPO_VISITA { get; set; }
        public short ID_TIPO_DOCUMENTO { get; set; }
        public string DESCR { get; set; }
        public bool DIGITALIZADO { get; set; }
    }
}
