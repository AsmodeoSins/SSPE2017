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
    
    public partial class PFC_V_PELIGROSIDAD
    {
        public PFC_V_PELIGROSIDAD()
        {
            this.TRASLADO_INTERNACIONAL = new HashSet<TRASLADO_INTERNACIONAL>();
            this.TRASLADO_ISLAS = new HashSet<TRASLADO_ISLAS>();
            this.TRASLADO_NACIONAL = new HashSet<TRASLADO_NACIONAL>();
            this.PFC_V_CRIMINODIAGNOSTICO = new HashSet<PFC_V_CRIMINODIAGNOSTICO>();
        }
    
        public short ID_PELIGROSIDAD { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<TRASLADO_INTERNACIONAL> TRASLADO_INTERNACIONAL { get; set; }
        public virtual ICollection<TRASLADO_ISLAS> TRASLADO_ISLAS { get; set; }
        public virtual ICollection<TRASLADO_NACIONAL> TRASLADO_NACIONAL { get; set; }
        public virtual ICollection<PFC_V_CRIMINODIAGNOSTICO> PFC_V_CRIMINODIAGNOSTICO { get; set; }
    }
}