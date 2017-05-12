using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class PadronEmpleados
    {
        public string nombre { get; set; }
        public string domicilio { get; set; }
        public string num_visitante { get; set; }
        public string estado { get; set; }
        public string municipio { get; set; }
        public string tel { get; set; }
        public string sexo { get; set; }
        public string fechanac { get; set; }
        public string rfc { get; set; }
        public string curp { get; set; }

        public string fecha_alta { get; set; }
        public string fecha_reg { get; set; }
        public string estatus_visita { get; set; }
        public string observaciones { get; set; }
        public byte[] imagen_frente { set; get; }
    }
}
