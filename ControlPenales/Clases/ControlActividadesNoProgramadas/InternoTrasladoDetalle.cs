using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases.ControlActividadesNoProgramadas
{
    public class InternoTrasladoDetalle
    {
        public short Centro { get; set; }
        public short Anio { get; set; }
        public int IdImputado { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string Estatus { get; set; }
        public bool EnSalidaCentro { get; set; }
    }
}
