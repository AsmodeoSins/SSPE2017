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
    
    public partial class PERSONALIDAD_TIPO_ESTUDIO
    {
        public PERSONALIDAD_TIPO_ESTUDIO()
        {
            this.PERSONALIDAD_DETALLE = new HashSet<PERSONALIDAD_DETALLE>();
        }
    
        public short ID_TIPO { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<PERSONALIDAD_DETALLE> PERSONALIDAD_DETALLE { get; set; }
    }
}
