using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_CALENDARIZAR_ENTREGA
    {
        public int ID_CALENDARIZACION_ENTREGA { get; set; }
        public Nullable<int> ID_ORDEN_COMPRA { get; set; }
        public Nullable<short> ID_ALMACEN { get; set; }
        public Nullable<System.DateTime> FEC_PACTADA { get; set; }
        public string ESTATUS { get; set; }
        public Nullable<int> ID_ENTRADA { get; set; }
        public string ID_USUARIO { get; set; }
        public Nullable<System.DateTime> FECHA { get; set; }
        public Nullable<int> ID_INCIDENCIA { get; set; }

        public List<EXT_CALENDARIZAR_ENTREGA_PRODUCTO> CALENDARIZAR_ENTREGA_PRODUCTO { get; set; }
    }
}
