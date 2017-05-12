using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_Almacen_Tipo_Cat
    {
        public short ID_ALMACEN_TIPO { get; set; }
        public string DESCR { get; set; }
        public string ACTIVO { get; set; }
        public bool IS_SELECTED { get; set; }
        public string ID_ALMACEN_GRUPO { get; set; }
    }
}
