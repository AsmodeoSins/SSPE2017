using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class cPertenenciasReporte
    {
        public string Titulo { get; set; }
        public string Expediente { get; set; }
        public string Nombre { get; set; }
        public string FechaRegistro { get; set; }
        public string Texto1 { get; set; }
        public string Texto2 { get; set; }
        public string Texto3 { get; set; }
        public string PersonasAutorizadas { get; set; }
        public byte[] Header { get; set; }
        public byte[] Foto { get; set; }
    }
}
