using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cInternoGafeteBrazalete
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public bool SELECCIONADO { get; set; }
        public byte[] FOTOINGRESO { get; set; }
        public byte[] FOTOCENTRO { get; set; }
    }
}
