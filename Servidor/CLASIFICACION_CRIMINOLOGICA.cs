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
    
    public partial class CLASIFICACION_CRIMINOLOGICA
    {
        public CLASIFICACION_CRIMINOLOGICA()
        {
            this.EMI_CLAS_CRIMINOLOGICA = new HashSet<EMI_CLAS_CRIMINOLOGICA>();
        }
    
        public short ID_CLAS { get; set; }
        public string DESCR { get; set; }
    
        public virtual ICollection<EMI_CLAS_CRIMINOLOGICA> EMI_CLAS_CRIMINOLOGICA { get; set; }
    }
}