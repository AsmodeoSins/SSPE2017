using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_Requisicion_Centro_Producto
    {
        public short? ID_ALMACEN { get; set; }
        public int ID_REQUISICION { get; set; }
        public int ID_PRODUCTO { get; set; }
        public int? CANTIDAD { get; set; }
        public string CENTRO { get; set; }
        public decimal? PRECIO { get; set; }
    }
}
