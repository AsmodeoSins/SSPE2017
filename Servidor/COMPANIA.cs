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
    
    public partial class COMPANIA
    {
        public COMPANIA()
        {
            this.DECOMISO_OBJETO = new HashSet<DECOMISO_OBJETO>();
        }
    
        public short ID_COMPANIA { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
        public Nullable<short> ID_OBJETO_TIPO { get; set; }
    
        public virtual ICollection<DECOMISO_OBJETO> DECOMISO_OBJETO { get; set; }
        public virtual OBJETO_TIPO OBJETO_TIPO { get; set; }
    }
}
