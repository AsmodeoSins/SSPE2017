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
    
    public partial class HOJA_ENFERMERIA_ULCERA
    {
        public decimal ID_HOJA_ULCERA { get; set; }
        public short ID_CENTRO_UBI { get; set; }
        public Nullable<decimal> ID_HOJAENF { get; set; }
        public Nullable<decimal> ID_HOSPITA { get; set; }
        public Nullable<short> ID_REGION { get; set; }
        public string ID_USUARIO { get; set; }
        public Nullable<System.DateTime> FECHA_REGISTRO { get; set; }
        public string DESC { get; set; }
    
        public virtual ANATOMIA_TOPOGRAFICA ANATOMIA_TOPOGRAFICA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual HOJA_ENFERMERIA HOJA_ENFERMERIA { get; set; }
    }
}
