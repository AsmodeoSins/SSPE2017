using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor
{
    public partial class V_ALMACEN_INVENTARIO:ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
