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
    
    public partial class GRUPO_HORARIO_ESTATUS
    {
        public GRUPO_HORARIO_ESTATUS()
        {
            this.GRUPO_HORARIO = new HashSet<GRUPO_HORARIO>();
        }
    
        public short ID_ESTATUS { get; set; }
        public string DESCR { get; set; }
    
        public virtual ICollection<GRUPO_HORARIO> GRUPO_HORARIO { get; set; }
    }
}
