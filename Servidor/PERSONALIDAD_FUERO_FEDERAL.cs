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
    
    public partial class PERSONALIDAD_FUERO_FEDERAL
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_ESTUDIO { get; set; }
    
        public virtual PFF_ACTA_CONSEJO_TECNICO PFF_ACTA_CONSEJO_TECNICO { get; set; }
        public virtual PFF_FICHA_IDENTIFICACION PFF_FICHA_IDENTIFICACION { get; set; }
        public virtual PFF_ESTUDIO_PSICOLOGICO PFF_ESTUDIO_PSICOLOGICO { get; set; }
        public virtual PFF_CRIMINOLOGICO PFF_CRIMINOLOGICO { get; set; }
        public virtual PFF_VIGILANCIA PFF_VIGILANCIA { get; set; }
        public virtual PFF_CAPACITACION PFF_CAPACITACION { get; set; }
        public virtual PFF_ESTUDIO_MEDICO PFF_ESTUDIO_MEDICO { get; set; }
        public virtual PFF_TRABAJO_SOCIAL PFF_TRABAJO_SOCIAL { get; set; }
        public virtual PERSONALIDAD PERSONALIDAD { get; set; }
        public virtual PFF_ACTIVIDAD PFF_ACTIVIDAD { get; set; }
    }
}
