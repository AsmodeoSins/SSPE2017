using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cReporteListadoGeneralDelito
    {
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public string Expediente { get; set; }
        public string NombreCompleto { get; set; }
        public string Ingreso { get; set; }
        public string Alias { get; set; }
        public string DelitoIngreso { get; set; }
        public string Fuero { get; set; }
        public string Situacion { get; set; }
        public string Ubicacion { get; set; }
        public byte[] Foto { get; set; }

        public DateTime? FechaIngreso { get; set; }
        public short? Edad { get; set; }
    }
}
