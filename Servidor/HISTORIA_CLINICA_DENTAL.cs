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
    
    public partial class HISTORIA_CLINICA_DENTAL
    {
        public HISTORIA_CLINICA_DENTAL()
        {
            this.HISTORIA_CLINICA_DENTAL_DOCUME = new HashSet<HISTORIA_CLINICA_DENTAL_DOCUME>();
            this.ODONTOGRAMA_INICIAL = new HashSet<ODONTOGRAMA_INICIAL>();
        }
    
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_CONSEC { get; set; }
        public string ID_USUARIO { get; set; }
        public Nullable<System.DateTime> REGISTRO_FEC { get; set; }
        public string COMPLICACIONES { get; set; }
        public string HEMORRAGIA { get; set; }
        public string REACCION_ANESTESICO { get; set; }
        public string EXP_BUC_LABIOS { get; set; }
        public string EXP_BUC_LENGUA { get; set; }
        public string EXP_BUC_MUCOSA_NASAL { get; set; }
        public string EXP_BUC_AMIGDALAS { get; set; }
        public string EXP_BUC_PISO_BOCA { get; set; }
        public string EXP_BUC_PALADAR_DURO { get; set; }
        public string EXP_BUC_PALADAR_BLANCO { get; set; }
        public string EXP_BUC_CARRILLOS { get; set; }
        public string EXP_BUC_FRENILLOS { get; set; }
        public string EXP_BUC_OTROS { get; set; }
        public string DIENTES_CARIES { get; set; }
        public string DIENTES_FLUOROSIS { get; set; }
        public string DIENTES_ANOM_FORMA { get; set; }
        public string DIENTES_ANOM_FORMA_OBS { get; set; }
        public string DIENTES_ANOM_TAMANO { get; set; }
        public string DIENTES_ANOM_TAMANO_OBS { get; set; }
        public string DIENTES_HIPOPLASIA { get; set; }
        public string DIENTES_HIPOPLASIA_OBS { get; set; }
        public string DIENTES_OTROS { get; set; }
        public string ART_TEMP_DOLOR { get; set; }
        public string ART_TEMP_DOLOR_OBS { get; set; }
        public string ART_TEMP_RIGIDEZ { get; set; }
        public string ART_TEMP_RIGIDEZ_OBS { get; set; }
        public string ART_TEMP_CHASQUIDOS { get; set; }
        public string ART_TEMP_CHASQUIDOS_OBS { get; set; }
        public string ART_TEMP_CANSANCIO { get; set; }
        public string ART_TEMP_CANSANCIO_OBS { get; set; }
        public string ENCIAS_COLORACION { get; set; }
        public string ENCIAS_FORMA { get; set; }
        public string ENCIAS_TEXTURA { get; set; }
        public string BRUXISMO { get; set; }
        public string BRUXISMO_DOLOR { get; set; }
        public string TENSION_ARTERIAL { get; set; }
        public string TEMPERATURA { get; set; }
        public string FRECUENCIA_CARDIAC { get; set; }
        public string FRECUENCIA_RESPIRA { get; set; }
        public string PESO { get; set; }
        public string GLICEMIA { get; set; }
        public string USUARIO_ENFERMERA { get; set; }
        public string ALERGICO_MEDICAMENTO { get; set; }
        public string ALERGICO_MEDICAMENTO_CUAL { get; set; }
        public string AMENAZA_ABORTO { get; set; }
        public string LACTANDO { get; set; }
        public string ESTATURA { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual USUARIO USUARIO { get; set; }
        public virtual ICollection<HISTORIA_CLINICA_DENTAL_DOCUME> HISTORIA_CLINICA_DENTAL_DOCUME { get; set; }
        public virtual INGRESO INGRESO { get; set; }
        public virtual ICollection<ODONTOGRAMA_INICIAL> ODONTOGRAMA_INICIAL { get; set; }
    }
}
