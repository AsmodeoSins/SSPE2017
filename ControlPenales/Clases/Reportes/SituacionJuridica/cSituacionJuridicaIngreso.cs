using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cSituacionJuridicaIngreso
    {
        public short Id_Anio { get; set; }
        public int Id_Imputado { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Alias { get; set; }
        public string Apodos { get; set; }
        public bool TieneAlias { get; set; }
        public bool TieneApodos { get; set; }
        public string Situacion_Juridica { get; set; }
    }
}
