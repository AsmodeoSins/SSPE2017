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
    
    public partial class ATENCION_CITA_INCIDENCIA
    {
        public int ID_CITA { get; set; }
        public short ID_ACMOTIVO { get; set; }
        public short ID_CONSEC { get; set; }
        public string ID_USUARIO { get; set; }
        public Nullable<System.DateTime> ATENCION_ORIGINAL_FEC { get; set; }
        public string OBSERV { get; set; }
        public Nullable<System.DateTime> REGISTRO_FEC { get; set; }
        public short ID_CENTRO_UBI { get; set; }
    
        public virtual ATENCION_CITA_IN_MOTIVO ATENCION_CITA_IN_MOTIVO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual ATENCION_CITA ATENCION_CITA { get; set; }
    }
}
