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
    
    public partial class EMI_SITUACION_JURIDICA
    {
        public int ID_EMI { get; set; }
        public short ID_EMI_CONS { get; set; }
        public string VERSION_DELITO_INTERNO { get; set; }
        public string MENOR_PERIODO_LIBRE_REING { get; set; }
        public string MAYOR_PERIODO_LIBRE_REING { get; set; }
        public string PRACT_ESTUDIOS { get; set; }
        public string CUANDO_PRACT_ESTUDIOS { get; set; }
        public string DESEA_TRASLADO { get; set; }
        public string DONDE_DESEA_TRASLADO { get; set; }
        public string MOTIVO_DESEA_TRASLADO { get; set; }
        public Nullable<short> MENOR_PERIODO_LIBRE_REING2 { get; set; }
        public Nullable<short> MAYOR_PERIODO_LIBRE_REING2 { get; set; }
    
        public virtual EMI EMI { get; set; }
    }
}
