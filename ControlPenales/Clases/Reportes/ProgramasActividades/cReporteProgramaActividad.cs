using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cReporteProgramaActividad
    {
        public string TIPO_PROGRAMA { get; set; }
        public string TIPO_ACTIVIDAD { get; set; }
        public short FEMENINO { get; set; }
        public short MASCULINO { get; set; }
        public short NO_DEFINIDO { get; set; }
        public short TOTAL { get; set; }
    }

    public class cPlantillaPersonalTecnico
    {
        public string DEPARTAMENTO { get; set; }
        public string ROL { get; set; }
        public short TOTAL { get; set; }
    }
}
