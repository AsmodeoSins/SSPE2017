using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_Orden_Compra_Detalle
    {
        public int ID_ORDEN_COMPRA { get; set; }
        public short ID_ORDEN_COMPRA_DET { get; set; }
        public int ID_PRODUCTO { get; set; }
        public Nullable<int> CANTIDAD_ORDEN { get; set; }
        public Nullable<decimal> PRECIO_COMPRA { get; set; }
        public Nullable<int> CANTIDAD_ENTREGADA { get; set; }
        public Nullable<int> DIFERENCIA { get; set; }
        public Nullable<short> ID_ALMACEN { get; set; }
        public Nullable<int> CANTIDAD_TRANSITO { get; set; }
        public int ID_REQUISICION_CENTRO { get; set; }
    }
}
