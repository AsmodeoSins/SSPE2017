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
    
    public partial class DELITO
    {
        public DELITO()
        {
            this.EMI_INGRESO_ANTERIOR = new HashSet<EMI_INGRESO_ANTERIOR>();
            this.MODALIDAD_DELITO = new HashSet<MODALIDAD_DELITO>();
            this.INGRESO = new HashSet<INGRESO>();
        }
    
        public string ID_FUERO { get; set; }
        public long ID_DELITO { get; set; }
        public string DESCR { get; set; }
        public Nullable<short> PM { get; set; }
        public Nullable<short> ID_TITULO { get; set; }
        public Nullable<short> ID_GRUPO_DELITO { get; set; }
        public string ARTICULO { get; set; }
        public string DETALLE { get; set; }
        public string GRAVE { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<EMI_INGRESO_ANTERIOR> EMI_INGRESO_ANTERIOR { get; set; }
        public virtual DELITO_GRUPO DELITO_GRUPO { get; set; }
        public virtual ICollection<MODALIDAD_DELITO> MODALIDAD_DELITO { get; set; }
        public virtual ICollection<INGRESO> INGRESO { get; set; }
    }
}
