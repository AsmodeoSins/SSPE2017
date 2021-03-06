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
    
    public partial class TIPO_REFERENCIA
    {
        public TIPO_REFERENCIA()
        {
            this.ACOMPANANTE = new HashSet<ACOMPANANTE>();
            this.EMI_ANTECEDENTE_FAMILIAR_DROGA = new HashSet<EMI_ANTECEDENTE_FAMILIAR_DROGA>();
            this.EMI_FACTORES_SOCIO_FAMILIARES = new HashSet<EMI_FACTORES_SOCIO_FAMILIARES>();
            this.EMI_FICHA_IDENTIFICACION = new HashSet<EMI_FICHA_IDENTIFICACION>();
            this.EMI_GRUPO_FAMILIAR = new HashSet<EMI_GRUPO_FAMILIAR>();
            this.FAMILIAR_RESPONSABLE = new HashSet<FAMILIAR_RESPONSABLE>();
            this.LIBERADO_VIVE = new HashSet<LIBERADO_VIVE>();
            this.VISITA_AUTORIZADA = new HashSet<VISITA_AUTORIZADA>();
            this.PRS_NUCLEO_FAMILIAR = new HashSet<PRS_NUCLEO_FAMILIAR>();
            this.PFC_VI_COMUNICACION = new HashSet<PFC_VI_COMUNICACION>();
            this.HISTORIA_CLINICA_FAMILIAR = new HashSet<HISTORIA_CLINICA_FAMILIAR>();
            this.PFF_TRABAJO_SOCIAL = new HashSet<PFF_TRABAJO_SOCIAL>();
            this.EMI_ANTECEDENTE_FAM_CON_DEL = new HashSet<EMI_ANTECEDENTE_FAM_CON_DEL>();
            this.TRASLADO_INTERNACIONAL_VISITA = new HashSet<TRASLADO_INTERNACIONAL_VISITA>();
            this.LIBERADO = new HashSet<LIBERADO>();
            this.PRS_ENTREVISTA_INICIAL = new HashSet<PRS_ENTREVISTA_INICIAL>();
            this.PRS_REPORTE_PSICOLOGICO = new HashSet<PRS_REPORTE_PSICOLOGICO>();
            this.PRS_VISITA_DOMICILIARIA = new HashSet<PRS_VISITA_DOMICILIARIA>();
            this.VISITANTE_INGRESO = new HashSet<VISITANTE_INGRESO>();
            this.MEDIDA_PERSONA = new HashSet<MEDIDA_PERSONA>();
        }
    
        public short ID_TIPO_REFERENCIA { get; set; }
        public string DESCR { get; set; }
        public Nullable<short> PM { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<ACOMPANANTE> ACOMPANANTE { get; set; }
        public virtual ICollection<EMI_ANTECEDENTE_FAMILIAR_DROGA> EMI_ANTECEDENTE_FAMILIAR_DROGA { get; set; }
        public virtual ICollection<EMI_FACTORES_SOCIO_FAMILIARES> EMI_FACTORES_SOCIO_FAMILIARES { get; set; }
        public virtual ICollection<EMI_FICHA_IDENTIFICACION> EMI_FICHA_IDENTIFICACION { get; set; }
        public virtual ICollection<EMI_GRUPO_FAMILIAR> EMI_GRUPO_FAMILIAR { get; set; }
        public virtual ICollection<FAMILIAR_RESPONSABLE> FAMILIAR_RESPONSABLE { get; set; }
        public virtual ICollection<LIBERADO_VIVE> LIBERADO_VIVE { get; set; }
        public virtual ICollection<VISITA_AUTORIZADA> VISITA_AUTORIZADA { get; set; }
        public virtual ICollection<PRS_NUCLEO_FAMILIAR> PRS_NUCLEO_FAMILIAR { get; set; }
        public virtual ICollection<PFC_VI_COMUNICACION> PFC_VI_COMUNICACION { get; set; }
        public virtual ICollection<HISTORIA_CLINICA_FAMILIAR> HISTORIA_CLINICA_FAMILIAR { get; set; }
        public virtual ICollection<PFF_TRABAJO_SOCIAL> PFF_TRABAJO_SOCIAL { get; set; }
        public virtual ICollection<EMI_ANTECEDENTE_FAM_CON_DEL> EMI_ANTECEDENTE_FAM_CON_DEL { get; set; }
        public virtual ICollection<TRASLADO_INTERNACIONAL_VISITA> TRASLADO_INTERNACIONAL_VISITA { get; set; }
        public virtual ICollection<LIBERADO> LIBERADO { get; set; }
        public virtual ICollection<PRS_ENTREVISTA_INICIAL> PRS_ENTREVISTA_INICIAL { get; set; }
        public virtual ICollection<PRS_REPORTE_PSICOLOGICO> PRS_REPORTE_PSICOLOGICO { get; set; }
        public virtual ICollection<PRS_VISITA_DOMICILIARIA> PRS_VISITA_DOMICILIARIA { get; set; }
        public virtual ICollection<VISITANTE_INGRESO> VISITANTE_INGRESO { get; set; }
        public virtual ICollection<MEDIDA_PERSONA> MEDIDA_PERSONA { get; set; }
    }
}
