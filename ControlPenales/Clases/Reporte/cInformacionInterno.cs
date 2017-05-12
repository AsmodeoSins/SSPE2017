using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cInformacionInterno
    {
        public byte[] Foto{set; get;}
        public string Nombre {set; get;}
        public string Expediente {set; get;}
        public string Ubicacion { set; get; }
        public string Fecha { set; get; }
    }
}
