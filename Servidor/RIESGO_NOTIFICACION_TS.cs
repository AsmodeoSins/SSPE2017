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
    
    public partial class RIESGO_NOTIFICACION_TS
    {
        public RIESGO_NOTIFICACION_TS()
        {
            this.NOTIFICACION_TS = new HashSet<NOTIFICACION_TS>();
        }
    
        public decimal ID_RIESGOS { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<NOTIFICACION_TS> NOTIFICACION_TS { get; set; }
    }
}
