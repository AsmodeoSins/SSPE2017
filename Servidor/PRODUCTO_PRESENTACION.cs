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
    
    public partial class PRODUCTO_PRESENTACION
    {
        public int ID_PRODUCTO { get; set; }
        public short ID_PRESENTACION { get; set; }
        public Nullable<decimal> CANTIDAD { get; set; }
    
        public virtual PRESENTACION PRESENTACION { get; set; }
        public virtual PRODUCTO PRODUCTO { get; set; }
    }
}
