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
    
    public partial class ORDEN_COMPRA_ESTATUS
    {
        public ORDEN_COMPRA_ESTATUS()
        {
            this.ORDEN_COMPRA = new HashSet<ORDEN_COMPRA>();
        }
    
        public string ID_OC_ESTATUS { get; set; }
        public string DESCR { get; set; }
    
        public virtual ICollection<ORDEN_COMPRA> ORDEN_COMPRA { get; set; }
    }
}