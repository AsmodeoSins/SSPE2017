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
    
    public partial class DELITO_GRUPO
    {
        public DELITO_GRUPO()
        {
            this.DELITO = new HashSet<DELITO>();
        }
    
        public string ID_FUERO { get; set; }
        public short ID_TITULO { get; set; }
        public short ID_GRUPO_DELITO { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<DELITO> DELITO { get; set; }
        public virtual DELITO_TITULO DELITO_TITULO { get; set; }
    }
}
