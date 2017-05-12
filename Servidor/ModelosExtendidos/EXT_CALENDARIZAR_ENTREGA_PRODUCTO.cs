using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_CALENDARIZAR_ENTREGA_PRODUCTO
    {
        public int ID_CALENDARIZACION_ENTREGA { get; set; }
        public int ID_PRODUCTO { get; set; }
        public Nullable<decimal> CANTIDAD { get; set; }

        public bool IsEditable { get; set; }

        public INCIDENCIA_TIPO INCIDENCIA_TIPO
        {
            get; set;
        }

        public string INCIDENCIA_OBSERVACIONES
        {
            get;
            set;
        }

        public DateTime? FechaRecalendarizacion
        {
            get;
            set;
        }

        public string ESTATUS
        {
            get;
            set;
        }
    }
}
