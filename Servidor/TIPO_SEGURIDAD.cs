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
    
    public partial class TIPO_SEGURIDAD
    {
        public TIPO_SEGURIDAD()
        {
            this.CELDA = new HashSet<CELDA>();
            this.INGRESO = new HashSet<INGRESO>();
        }
    
        public string ID_TIPO_SEGURIDAD { get; set; }
        public string DESCR { get; set; }
    
        public virtual ICollection<CELDA> CELDA { get; set; }
        public virtual ICollection<INGRESO> INGRESO { get; set; }
    }
}
