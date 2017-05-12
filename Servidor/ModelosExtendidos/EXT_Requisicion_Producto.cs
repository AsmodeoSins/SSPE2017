using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_Requisicion_Producto
    {
        public int ID_REQUISICION { get; set; }
        public int ID_PRODUCTO { get; set; }
        public int? CANTIDAD { get; set; }
        public string NOMBRE_PRODUCTO { get; set; }
        public bool IS_SELECTED { get; set; }
    }
}
