using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cImputadoAbogado
    {
        public int ID_ABOGADO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string NOMBRE_COMPLETO
        {
            get
            {
                var _temp = NOMBRE;
                if (!string.IsNullOrWhiteSpace(PATERNO))
                    _temp += " " + PATERNO;
                if (!string.IsNullOrWhiteSpace(MATERNO))
                    _temp += " " + MATERNO;
                return _temp;
            }
        }
        public string TITULO { get; set; }
        public int? TITULAR { get; set; }
        public int TITULAR_ORDEN { get; set; } //este campo trae el valor del titular o en caso de no tener TITULAR el ID_ABOGADO
        public int TITULO_ORDEN { get; set; } //este campo ordena para que el titular traiga el valor de 1, y el colaborador traiga el valor de 2.
    }
}
