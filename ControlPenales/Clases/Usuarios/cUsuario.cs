using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;

namespace ControlPenales
{
    public class cUsuarioExtendida
    {
        public USUARIO Usuario { get; set; }

        public string NOMBRE_COMPLETO { get; set; }

        //    get
        //    {
        //        //return Usuario.PERSONA.NOMBRE + (string.IsNullOrWhiteSpace(Usuario.PERSONA.PATERNO) ? string.Empty : " " + Usuario.PERSONA.PATERNO) + 
        //        //    (string.IsNullOrWhiteSpace(string.IsNullOrWhiteSpace(Usuario.PERSONA.MATERNO)?string.Empty: " " + Usuario.PERSONA.MATERNO));
        //    }
        //}

        public int ID_EMPLEADO { get; set; }
    }
}
