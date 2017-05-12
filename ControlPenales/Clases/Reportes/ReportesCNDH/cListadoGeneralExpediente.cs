using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cListadoGeneralExpediente
    {
        public int Anio { get; set; }
        public int Id_Imputado { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Clasificacion { get; set; }
        public string Fuero { get; set; }
        public string Edificio { get; set; }
        public string Sector { get; set; }
        public string Celda { get; set; }
    }
}
