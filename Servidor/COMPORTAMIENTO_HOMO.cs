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
    
    public partial class COMPORTAMIENTO_HOMO
    {
        public COMPORTAMIENTO_HOMO()
        {
            this.EMI_HPS = new HashSet<EMI_HPS>();
        }
    
        public short ID_HOMO { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<EMI_HPS> EMI_HPS { get; set; }
    }
}
