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
    
    public partial class TIPO_DISPOSICION
    {
        public TIPO_DISPOSICION()
        {
            this.INGRESO = new HashSet<INGRESO>();
        }
    
        public short ID_DISPOSICION { get; set; }
        public string DESCR { get; set; }
    
        public virtual ICollection<INGRESO> INGRESO { get; set; }
    }
}
