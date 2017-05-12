using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class ReporteComandancia
    {
        public string nombre { set; get; }
        public string puesto { set; get; }
        public string presente_mensaje { set; get; }
        public string imputado_comun { set; get; }
        public string indiciado_comun { set; get; }
        public string indiciado_federal { set; get; }

        public string indiciado_sinfuero { set; get; }
        public string procesado_comun { set; get; }
        public string procesado_federal { set; get; }
        public string procesado_sinfuero { set; get; }

        public string sentenciado_comun { set; get; }
        public string sentenciado_federal { set; get; }
        public string total { set; get; }
        public string nombre_usuario { set; get; }
        public string sexo { set; get; }
        public string fecha { set; get; }
    }
}
