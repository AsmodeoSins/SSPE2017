using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cEstancia
    {
        public int Edificio { get; set; }
        public int Sector { get; set; }
        public string Celda { get; set; }
        public string EdificioDescr { get; set; }
        public string SectorDescr { get; set; }
        public int NumeroLista { get; set; }
        public int CantidadCamas { get; set; }
        public int Pagina { get; set; }
    }
}
