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
    
    public partial class GRUPO_PARTICIPANTE_ESTATUS
    {
        public GRUPO_PARTICIPANTE_ESTATUS()
        {
            this.GRUPO_PARTICIPANTE = new HashSet<GRUPO_PARTICIPANTE>();
            this.GRUPO_PARTICIPANTE_CANCELADO = new HashSet<GRUPO_PARTICIPANTE_CANCELADO>();
        }
    
        public short ID_ESTATUS { get; set; }
        public string DESCR { get; set; }
    
        public virtual ICollection<GRUPO_PARTICIPANTE> GRUPO_PARTICIPANTE { get; set; }
        public virtual ICollection<GRUPO_PARTICIPANTE_CANCELADO> GRUPO_PARTICIPANTE_CANCELADO { get; set; }
    }
}
