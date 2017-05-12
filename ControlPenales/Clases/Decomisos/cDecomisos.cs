using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases
{
    public class cDecomisos
    {
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Tipo { get; set; }
        public byte[] ImagenVisitante { get; set; }
        public DECOMISO Decomiso { get; set; }
    }

    public class cReporteDecomisoInvolucrados
    {
        public string Fecha { get; set; }
        public string Involucrado { get; set; }
        public string Control { get; set; }
        public string Nombre { get; set; }
        public string Droga { get; set; }
        public string Cantidad { get; set; }
        public string Presentacion { get; set; }
        public string Dosis { get; set; }
    }
}
