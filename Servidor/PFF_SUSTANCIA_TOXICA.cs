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
    
    public partial class PFF_SUSTANCIA_TOXICA
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_ESTUDIO { get; set; }
        public short ID_DROGA { get; set; }
        public int CANTIDAD { get; set; }
        public string PERIODICIDAD { get; set; }
        public Nullable<short> EDAD_INICIO { get; set; }
    
        public virtual DROGA DROGA { get; set; }
        public virtual PFF_ESTUDIO_MEDICO PFF_ESTUDIO_MEDICO { get; set; }
    }
}