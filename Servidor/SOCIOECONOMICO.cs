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
    
    public partial class SOCIOECONOMICO
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public Nullable<decimal> SALARIO { get; set; }
        public string DICTAMEN { get; set; }
        public Nullable<System.DateTime> DICTAMEN_FEC { get; set; }
    
        public virtual SOCIOE_GPOFAMPRI SOCIOE_GPOFAMPRI { get; set; }
        public virtual SOCIOE_GPOFAMSEC SOCIOE_GPOFAMSEC { get; set; }
        public virtual INGRESO INGRESO { get; set; }
    }
}
