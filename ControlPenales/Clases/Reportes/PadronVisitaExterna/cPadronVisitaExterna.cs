using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cPadronVisitaExterna
    {
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string NIP { get; set; }
        public string NumeroVisita { get; set; }
        public string TipoVisitante { get; set; }
        public string FechaAlta { get; set; }
        public string Correo { get; set; }
        public string Discapacidad { get; set; }
        public string Telefono { get; set; }
        public string Estatus { get; set; }
        public string Observaciones { get; set; }
        public byte[] Foto { get; set; }
    }
}
