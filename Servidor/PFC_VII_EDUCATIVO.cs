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
    
    public partial class PFC_VII_EDUCATIVO
    {
        public PFC_VII_EDUCATIVO()
        {
            this.PFC_VII_ESCOLARIDAD_ANTERIOR = new HashSet<PFC_VII_ESCOLARIDAD_ANTERIOR>();
            this.PFC_VII_ACTIVIDAD = new HashSet<PFC_VII_ACTIVIDAD>();
        }
    
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_ESTUDIO { get; set; }
        public Nullable<short> P3_DICTAMEN { get; set; }
        public string P4_MOTIVACION_DICTAMEN { get; set; }
        public Nullable<System.DateTime> ESTUDIO_FEC { get; set; }
        public string COORDINADOR { get; set; }
        public string ELABORO { get; set; }
    
        public virtual PERSONALIDAD_FUERO_COMUN PERSONALIDAD_FUERO_COMUN { get; set; }
        public virtual ICollection<PFC_VII_ESCOLARIDAD_ANTERIOR> PFC_VII_ESCOLARIDAD_ANTERIOR { get; set; }
        public virtual ICollection<PFC_VII_ACTIVIDAD> PFC_VII_ACTIVIDAD { get; set; }
    }
}
