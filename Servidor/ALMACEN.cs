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
    
    public partial class ALMACEN
    {
        public ALMACEN()
        {
            this.ALMACEN1 = new HashSet<ALMACEN>();
            this.ORDEN_COMPRA_DETALLE = new HashSet<ORDEN_COMPRA_DETALLE>();
            this.CALENDARIZAR_ENTREGA = new HashSet<CALENDARIZAR_ENTREGA>();
            this.ALMACEN_INVENTARIO = new HashSet<ALMACEN_INVENTARIO>();
            this.MOVIMIENTO = new HashSet<MOVIMIENTO>();
            this.TRASPASO = new HashSet<TRASPASO>();
            this.REQUISICION_CENTRO = new HashSet<REQUISICION_CENTRO>();
            this.INCIDENCIA = new HashSet<INCIDENCIA>();
        }
    
        public short ID_ALMACEN { get; set; }
        public short ID_CENTRO { get; set; }
        public string DESCRIPCION { get; set; }
        public string ACTIVO { get; set; }
        public Nullable<short> ID_PRODUCTO_TIPO { get; set; }
        public Nullable<short> ALMACEN_SUPERIOR { get; set; }
    
        public virtual ICollection<ALMACEN> ALMACEN1 { get; set; }
        public virtual ALMACEN ALMACEN2 { get; set; }
        public virtual ALMACEN_TIPO_CAT ALMACEN_TIPO_CAT { get; set; }
        public virtual ICollection<ORDEN_COMPRA_DETALLE> ORDEN_COMPRA_DETALLE { get; set; }
        public virtual ICollection<CALENDARIZAR_ENTREGA> CALENDARIZAR_ENTREGA { get; set; }
        public virtual ICollection<ALMACEN_INVENTARIO> ALMACEN_INVENTARIO { get; set; }
        public virtual ICollection<MOVIMIENTO> MOVIMIENTO { get; set; }
        public virtual ICollection<TRASPASO> TRASPASO { get; set; }
        public virtual ICollection<REQUISICION_CENTRO> REQUISICION_CENTRO { get; set; }
        public virtual CENTRO CENTRO { get; set; }
        public virtual ICollection<INCIDENCIA> INCIDENCIA { get; set; }
    }
}