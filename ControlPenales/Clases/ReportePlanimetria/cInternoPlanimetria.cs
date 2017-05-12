using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class cInternoPlanimetria
    {
        public string Centro { get; set; }
        public string Anio { get; set; }
        public string Folio { get; set; }
        public string APaterno { get; set; }
        public string AMaterno { get; set; }
        public string Nombre { get; set; }
        public int IdClasificacion { get; set; }
        public string Estancia { get; set; }
        public string Cama { get; set; }
        public byte[] Foto { get; set; }
    }
}
