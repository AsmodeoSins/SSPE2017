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
    
    public partial class TIPO_FILIACION
    {
        public TIPO_FILIACION()
        {
            this.IMPUTADO_FILIACION = new HashSet<IMPUTADO_FILIACION>();
            this.PFF_FILIACION = new HashSet<PFF_FILIACION>();
        }
    
        public short ID_MEDIA_FILIACION { get; set; }
        public short ID_TIPO_FILIACION { get; set; }
        public string DESCR { get; set; }
        public Nullable<short> PM { get; set; }
        public Nullable<short> ID_VIEJO { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<IMPUTADO_FILIACION> IMPUTADO_FILIACION { get; set; }
        public virtual MEDIA_FILIACION MEDIA_FILIACION { get; set; }
        public virtual ICollection<PFF_FILIACION> PFF_FILIACION { get; set; }
    }
}
