using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cReosPeligrosos
    {
        public string Cereso { get; set; }
        public int Id_Anio { get; set; }
        public int Id_Imputado { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public int Id_Ingreso { get; set; }
        public string Activo { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string ClasificacionJuridica { get; set; }
        public string Delito { get; set; }
    }
}
