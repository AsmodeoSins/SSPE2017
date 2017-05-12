using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cDecomisoFecha
    {
        public int Id_Decomiso { get; set; }
        public DateTime FechaEvento { get; set; }
        public string NumeroOficio { get; set; }
        public string Turno { get; set; }
        public string GrupoTactico { get; set; }
        public string Ubicacion { get; set; }
        public string Area { get; set; }
        public string Edificio { get; set; }
        public string Sector { get; set; }
        public string Celda { get; set; }
    }
}
