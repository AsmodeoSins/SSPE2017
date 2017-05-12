using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GESAL.Clases.Extendidas
{
    public class EXT_RECEPCION_PRODUCTO_DETALLE:ICloneable
    {
        public int ID_PRODUCTO { get; set; }
        public decimal RECIBIDO { get; set; }
        public int LOTE { get; set; }
        public DateTime? FECHA_CADUCIDAD { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
