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
    
    public partial class MEDIA_FILIACION
    {
        public MEDIA_FILIACION()
        {
            this.TIPO_FILIACION = new HashSet<TIPO_FILIACION>();
        }
    
        public short ID_MEDIA_FILIACION { get; set; }
        public string DESCR { get; set; }
        public Nullable<short> PM { get; set; }
        public Nullable<short> ID_VIEJO { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<TIPO_FILIACION> TIPO_FILIACION { get; set; }
    }
}
