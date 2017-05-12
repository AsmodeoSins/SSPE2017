using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases.ControlActividadesNoProgramadas
{
    public class InternoExcarcelacion
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string NOMBRE { get; set; }
        public bool PERMITIR { get; set; }
        public bool HABILITAR { get; set; }
    }
}
