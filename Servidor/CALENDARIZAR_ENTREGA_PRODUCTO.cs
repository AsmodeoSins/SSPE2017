//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SSP.Servidor
{
    using System;
    using System.Collections.Generic;
    
    public partial class CALENDARIZAR_ENTREGA_PRODUCTO
    {
        public CALENDARIZAR_ENTREGA_PRODUCTO()
        {
            this.CALENDARIZAR_ENTREGA_PRODUCTO1 = new HashSet<CALENDARIZAR_ENTREGA_PRODUCTO>();
        }
    
        public int ID_CALENDARIZACION_ENTREGA { get; set; }
        public int ID_PRODUCTO { get; set; }
        public int ID_CONSEC { get; set; }
        public Nullable<decimal> CANTIDAD { get; set; }
        public Nullable<int> CAL_ID_CALENDARIZACION_ENTREGA { get; set; }
        public Nullable<int> CAL_ID_CONSEC { get; set; }
        public string ESTATUS { get; set; }
        public Nullable<int> ID_INCIDENCIA { get; set; }
    
        public virtual CALENDARIZAR_ENTREGA CALENDARIZAR_ENTREGA { get; set; }
        public virtual ICollection<CALENDARIZAR_ENTREGA_PRODUCTO> CALENDARIZAR_ENTREGA_PRODUCTO1 { get; set; }
        public virtual CALENDARIZAR_ENTREGA_PRODUCTO CALENDARIZAR_ENTREGA_PRODUCTO2 { get; set; }
        public virtual CALENDARIZAR_EP_ESTATUS CALENDARIZAR_EP_ESTATUS { get; set; }
        public virtual INCIDENCIA_PRODUCTO INCIDENCIA_PRODUCTO { get; set; }
        public virtual PRODUCTO PRODUCTO { get; set; }
    }
}