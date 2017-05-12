using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cCeldasSector
    {
        public short centro { set; get; }
        public short edificio { set; get; }
        public short sector { set; get; }
        public string celda { set; get; }
        private bool seleccione { set; get; }
       
    }
}
