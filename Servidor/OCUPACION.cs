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
    
    public partial class OCUPACION
    {
        public OCUPACION()
        {
            this.EMI_GRUPO_FAMILIAR = new HashSet<EMI_GRUPO_FAMILIAR>();
            this.EMI_ULTIMOS_EMPLEOS = new HashSet<EMI_ULTIMOS_EMPLEOS>();
            this.LIBERADO_VIVE = new HashSet<LIBERADO_VIVE>();
            this.PRS_APOYO_ECONOMICO = new HashSet<PRS_APOYO_ECONOMICO>();
            this.PRS_NUCLEO_FAMILIAR = new HashSet<PRS_NUCLEO_FAMILIAR>();
            this.PFF_CAPACITACION = new HashSet<PFF_CAPACITACION>();
            this.PFF_ESTUDIO_MEDICO = new HashSet<PFF_ESTUDIO_MEDICO>();
            this.PFF_ESTUDIO_MEDICO1 = new HashSet<PFF_ESTUDIO_MEDICO>();
            this.PFF_TRABAJO_SOCIAL = new HashSet<PFF_TRABAJO_SOCIAL>();
            this.LIBERADO = new HashSet<LIBERADO>();
            this.PRS_ENTREVISTA_INICIAL = new HashSet<PRS_ENTREVISTA_INICIAL>();
            this.INGRESO = new HashSet<INGRESO>();
            this.PROCESO_LIBERTAD = new HashSet<PROCESO_LIBERTAD>();
        }
    
        public short ID_OCUPACION { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<EMI_GRUPO_FAMILIAR> EMI_GRUPO_FAMILIAR { get; set; }
        public virtual ICollection<EMI_ULTIMOS_EMPLEOS> EMI_ULTIMOS_EMPLEOS { get; set; }
        public virtual ICollection<LIBERADO_VIVE> LIBERADO_VIVE { get; set; }
        public virtual ICollection<PRS_APOYO_ECONOMICO> PRS_APOYO_ECONOMICO { get; set; }
        public virtual ICollection<PRS_NUCLEO_FAMILIAR> PRS_NUCLEO_FAMILIAR { get; set; }
        public virtual ICollection<PFF_CAPACITACION> PFF_CAPACITACION { get; set; }
        public virtual ICollection<PFF_ESTUDIO_MEDICO> PFF_ESTUDIO_MEDICO { get; set; }
        public virtual ICollection<PFF_ESTUDIO_MEDICO> PFF_ESTUDIO_MEDICO1 { get; set; }
        public virtual ICollection<PFF_TRABAJO_SOCIAL> PFF_TRABAJO_SOCIAL { get; set; }
        public virtual ICollection<LIBERADO> LIBERADO { get; set; }
        public virtual ICollection<PRS_ENTREVISTA_INICIAL> PRS_ENTREVISTA_INICIAL { get; set; }
        public virtual ICollection<INGRESO> INGRESO { get; set; }
        public virtual ICollection<PROCESO_LIBERTAD> PROCESO_LIBERTAD { get; set; }
    }
}
