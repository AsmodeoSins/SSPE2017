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
    
    public partial class LIBERADO_SUSTANCIA
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_CONSEC { get; set; }
        public Nullable<short> ID_DROGA { get; set; }
        public Nullable<short> ID_FRECUENCIA { get; set; }
        public Nullable<short> EDAD_INICIO { get; set; }
        public Nullable<int> CANTIDAD { get; set; }
        public Nullable<System.DateTime> ULTIMO_CONSUMO_FEC { get; set; }
    
        public virtual DROGA DROGA { get; set; }
        public virtual DROGA_FRECUENCIA DROGA_FRECUENCIA { get; set; }
        public virtual LIBERADO LIBERADO { get; set; }
    }
}
