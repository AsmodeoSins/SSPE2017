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
    
    public partial class AGENDA_ACTIVIDAD_ESTATUS
    {
        public AGENDA_ACTIVIDAD_ESTATUS()
        {
            this.AGENDA_ACTIVIDAD_LIBERTAD = new HashSet<AGENDA_ACTIVIDAD_LIBERTAD>();
        }
    
        public decimal ID_ESTATUS { get; set; }
        public string DESCR { get; set; }
    
        public virtual ICollection<AGENDA_ACTIVIDAD_LIBERTAD> AGENDA_ACTIVIDAD_LIBERTAD { get; set; }
    }
}
