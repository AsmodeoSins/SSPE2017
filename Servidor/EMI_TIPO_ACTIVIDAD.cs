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
    
    public partial class EMI_TIPO_ACTIVIDAD
    {
        public EMI_TIPO_ACTIVIDAD()
        {
            this.EMI_ACTIVIDAD = new HashSet<EMI_ACTIVIDAD>();
        }
    
        public short ID_EMI_ACTIVIDAD { get; set; }
        public string DESCR { get; set; }
    
        public virtual ICollection<EMI_ACTIVIDAD> EMI_ACTIVIDAD { get; set; }
    }
}
