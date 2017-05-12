using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases
{
    public class IngresoAinterior
    {
        public bool? Seleccione {get; set;}
        public EMISOR Emisor { get; set; }
        public DELITO Delito { get; set; }
        public string PerioroReclusion { get; set; }
        public string Sanciones { get; set; }
        public short? IdCentro { get; set; }
        public short? IdAnio { get; set; }
        public decimal? IdImputado { get; set; }
        public short? IdIngreso { get; set; }
    }
}
