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
    
    public partial class ABOGADO_TITULAR
    {
        public int ID_ABOGADO { get; set; }
        public int ID_ABOGADO_TITULAR { get; set; }
        public string ESTATUS { get; set; }
        public Nullable<System.DateTime> MOVIMIENTO_FEC { get; set; }
    
        public virtual ABOGADO ABOGADO { get; set; }
        public virtual ABOGADO ABOGADO1 { get; set; }
    }
}
