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
    
    public partial class ESTATUS_VISITA
    {
        public ESTATUS_VISITA()
        {
            this.VISITANTE = new HashSet<VISITANTE>();
            this.ABOGADO = new HashSet<ABOGADO>();
            this.ABOGADO_INGRESO = new HashSet<ABOGADO_INGRESO>();
            this.ABOGADO_CAUSA_PENAL = new HashSet<ABOGADO_CAUSA_PENAL>();
            this.VISITANTE_INGRESO = new HashSet<VISITANTE_INGRESO>();
        }
    
        public short ID_ESTATUS_VISITA { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<VISITANTE> VISITANTE { get; set; }
        public virtual ICollection<ABOGADO> ABOGADO { get; set; }
        public virtual ICollection<ABOGADO_INGRESO> ABOGADO_INGRESO { get; set; }
        public virtual ICollection<ABOGADO_CAUSA_PENAL> ABOGADO_CAUSA_PENAL { get; set; }
        public virtual ICollection<VISITANTE_INGRESO> VISITANTE_INGRESO { get; set; }
    }
}
