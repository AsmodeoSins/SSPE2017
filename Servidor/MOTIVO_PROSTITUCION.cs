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
    
    public partial class MOTIVO_PROSTITUCION
    {
        public MOTIVO_PROSTITUCION()
        {
            this.EMI_HPS = new HashSet<EMI_HPS>();
        }
    
        public short ID_MOTIVO { get; set; }
        public string DESCR { get; set; }
    
        public virtual ICollection<EMI_HPS> EMI_HPS { get; set; }
    }
}