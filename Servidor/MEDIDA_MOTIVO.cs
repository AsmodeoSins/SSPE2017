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
    
    public partial class MEDIDA_MOTIVO
    {
        public MEDIDA_MOTIVO()
        {
            this.MEDIDA_LIBERTAD_ESTATUS = new HashSet<MEDIDA_LIBERTAD_ESTATUS>();
        }
    
        public short ID_MOTIVO { get; set; }
        public short ID_ESTATUS { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual MEDIDA_ESTATUS MEDIDA_ESTATUS { get; set; }
        public virtual ICollection<MEDIDA_LIBERTAD_ESTATUS> MEDIDA_LIBERTAD_ESTATUS { get; set; }
    }
}
