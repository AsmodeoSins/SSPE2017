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
    
    public partial class GRUPO_VULNERABLE
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_CONSEC { get; set; }
        public short ID_SECTOR_CLAS { get; set; }
        public short ID_GRUPO_CONSEC { get; set; }
        public Nullable<System.DateTime> REGISTRO_FEC { get; set; }
        public Nullable<System.DateTime> BAJA_FEC { get; set; }
        public string MOMENTO_DETECCION { get; set; }
    
        public virtual SECTOR_CLASIFICACION SECTOR_CLASIFICACION { get; set; }
        public virtual HISTORIA_CLINICA HISTORIA_CLINICA { get; set; }
    }
}
