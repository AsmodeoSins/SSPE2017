using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_V_INVENTARIO_PRODUCTO_B
    {
        public short ID_ALMACEN { get; set; }
        public string DESCR { get; set; }
        public int ID_PRODUCTO { get; set; }
        public string NOMBRE { get; set; }
        public decimal? ID_UNIDAD_MEDIDA { get; set; }
        public string UNIDAD_MEDIDA { get; set; }
        public decimal? ID_PRESENTACION { get; set; }
        public string PRESENTACION { get; set; }
        public Nullable<decimal> CANTIDAD { get; set; }
        public Nullable<decimal> TRANSITO { get; set; }
        public Nullable<decimal> TRASPASO { get; set; }
    }
}
