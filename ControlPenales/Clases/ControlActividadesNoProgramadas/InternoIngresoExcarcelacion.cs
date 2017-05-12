using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases.ControlActividadesNoProgramadas
{
    public class InternoIngresoExcarcelacion
    {
        public short Id_Centro { get; set; }
        public short Id_Anio { get; set; }
        public int Id_Imputado { get; set; }
        public short Id_Ingreso { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public bool EnSalidaDeCentro { get; set; }
    }
}
