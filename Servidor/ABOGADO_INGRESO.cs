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
    
    public partial class ABOGADO_INGRESO
    {
        public ABOGADO_INGRESO()
        {
            this.ABOGADO_CAUSA_PENAL = new HashSet<ABOGADO_CAUSA_PENAL>();
            this.ABOGADO_ING_DOCTO = new HashSet<ABOGADO_ING_DOCTO>();
        }
    
        public int ID_ABOGADO { get; set; }
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public string ID_ABOGADO_TITULO { get; set; }
        public Nullable<System.DateTime> CAPTURA_FEC { get; set; }
        public string OBSERV { get; set; }
        public Nullable<short> ID_ESTATUS_VISITA { get; set; }
        public string ADMINISTRATIVO { get; set; }
        public int ID_ABOGADO_TITULAR { get; set; }
    
        public virtual ABOGADO ABOGADO { get; set; }
        public virtual ABOGADO_TITULO ABOGADO_TITULO { get; set; }
        public virtual ESTATUS_VISITA ESTATUS_VISITA { get; set; }
        public virtual ICollection<ABOGADO_CAUSA_PENAL> ABOGADO_CAUSA_PENAL { get; set; }
        public virtual ICollection<ABOGADO_ING_DOCTO> ABOGADO_ING_DOCTO { get; set; }
        public virtual INGRESO INGRESO { get; set; }
    }
}
