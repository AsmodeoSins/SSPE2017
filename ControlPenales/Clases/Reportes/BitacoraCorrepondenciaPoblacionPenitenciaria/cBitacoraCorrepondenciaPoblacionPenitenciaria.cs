using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cBitacoraCorrepondenciaPoblacionPenitenciaria
    {
        public int Id_Anio { get; set; }
        public int Id_Imputado { get; set; }
        public string Nombre_Ingreso { get; set; }
        public string Paterno_Ingreso { get; set; }
        public string Materno_Ingreso { get; set; }
        public string Nombre_Remitente { get; set; }
        public string Paterno_Remitente { get; set; }
        public string Materno_Remitente { get; set; }
        public string Nombre_Depositante { get; set; }
        public string Paterno_Depositante { get; set; }
        public string Materno_Depositante { get; set; }
        public string FechaDeposito { get; set; }
        public string EdificioDescr { get; set; }
        public string SectorDescr { get; set; }
        public string Celda { get; set; }
        public string Observaciones { get; set; }
        public string LoginEntrega { get; set; }
        public string FechaEntrega { get; set; }
    }
}
