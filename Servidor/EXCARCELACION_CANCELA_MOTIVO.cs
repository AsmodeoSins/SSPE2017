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
    
    public partial class EXCARCELACION_CANCELA_MOTIVO
    {
        public EXCARCELACION_CANCELA_MOTIVO()
        {
            this.EXCARCELACION_DESTINO = new HashSet<EXCARCELACION_DESTINO>();
        }
    
        public short ID_EXCARCELACION_CANCELA { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<EXCARCELACION_DESTINO> EXCARCELACION_DESTINO { get; set; }
    }
}
