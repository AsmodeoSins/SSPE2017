
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cHistorialAtencionCitas
    {
        public string Fecha { get; set; }
        public string Atencion { get; set; }
    }

    public class cHistorialAtencionCitasGenerales
    {
        public string Expediente { get; set; }
        public string Nombre { get; set; }
        public byte[] Foto { get; set; }
    }
}
