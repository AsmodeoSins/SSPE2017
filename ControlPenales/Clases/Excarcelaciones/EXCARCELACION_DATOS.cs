using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class EXCARCELACION_DATOS
    {
        public EXCARCELACION SELECTED_EXCARCELACION { get; set; }
        public DateTime PROGRAMADO_FEC { get; set; }
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string NOMBRE { get; set; }
        public string DESTINO { get; set; }
        public CAMA CAMA { get; set; }
        public CAUSA_PENAL CAUSA_PENAL { get; set; }
    }
}
