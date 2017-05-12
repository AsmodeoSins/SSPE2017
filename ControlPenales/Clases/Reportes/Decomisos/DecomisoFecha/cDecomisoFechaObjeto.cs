using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cDecomisoFechaObjeto
    {
        public int Id_Decomiso { get; set; }
        public int Id_Tipo_Objeto { get; set; }
        public int Id_Consec { get; set; }
        public string TipoObjeto { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
    }
}
