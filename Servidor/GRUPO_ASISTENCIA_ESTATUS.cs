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
    
    public partial class GRUPO_ASISTENCIA_ESTATUS
    {
        public GRUPO_ASISTENCIA_ESTATUS()
        {
            this.GRUPO_ASISTENCIA = new HashSet<GRUPO_ASISTENCIA>();
        }
    
        public short ID_ESTATUS { get; set; }
        public string DESCR { get; set; }
    
        public virtual ICollection<GRUPO_ASISTENCIA> GRUPO_ASISTENCIA { get; set; }
    }
}
