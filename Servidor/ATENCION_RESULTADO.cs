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
    
    public partial class ATENCION_RESULTADO
    {
        public int ID_CITA { get; set; }
        public string RESULTADO { get; set; }
        public short ID_CENTRO_UBI { get; set; }
    
        public virtual ATENCION_CITA ATENCION_CITA { get; set; }
    }
}
