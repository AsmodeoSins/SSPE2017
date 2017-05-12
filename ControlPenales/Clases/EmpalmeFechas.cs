using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class EmpalmeFechas
    {

        public short NoIngreso{ get; set;}

        public string CausaPenal{ get; set;}

        public DateTime? FechaInicio{ get; set;}

        public DateTime? FechaFin { get; set; }

        public short? Anios { get; set; }

        public short? Meses { get; set; }

        public short? Dias { get; set; }
 
    }
}
