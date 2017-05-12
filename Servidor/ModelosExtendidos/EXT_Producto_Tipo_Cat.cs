using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_Producto_Tipo_Cat
    {
        public short ID_PRODUCTO_TIPO { get; set; }
        public string DESCR { get; set; }
        public string ACTIVO { get; set; }
        public bool IS_SELECTED { get; set; }
    }
}
