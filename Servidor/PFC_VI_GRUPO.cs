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
    
    public partial class PFC_VI_GRUPO
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_ESTUDIO { get; set; }
        public short ID_ACTIVIDAD { get; set; }
        public short ID_TIPO_PROGRAMA { get; set; }
        public string CONGREGACION { get; set; }
        public string PERIODO { get; set; }
        public string OBSERVACIONES { get; set; }
    
        public virtual ACTIVIDAD ACTIVIDAD { get; set; }
        public virtual PFC_VI_SOCIO_FAMILIAR PFC_VI_SOCIO_FAMILIAR { get; set; }
        public virtual TIPO_PROGRAMA TIPO_PROGRAMA { get; set; }
    }
}
