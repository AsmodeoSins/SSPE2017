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
    
    public partial class VISITA_EDIFICIO
    {
        public short ID_CENTRO { get; set; }
        public short ID_EDIFICIO { get; set; }
        public short ID_SECTOR { get; set; }
        public string CELDA_INICIO { get; set; }
        public string CELDA_FINAL { get; set; }
        public Nullable<short> ID_TIPO_VISITA { get; set; }
        public Nullable<short> DIA { get; set; }
        public short ID_CONSEC { get; set; }
        public Nullable<System.DateTime> FECHA_ALTA { get; set; }
        public string ESTATUS { get; set; }
        public string HORA_INI { get; set; }
        public string HORA_FIN { get; set; }
        public Nullable<short> ID_AREA { get; set; }
    
        public virtual AREA AREA { get; set; }
        public virtual CELDA CELDA { get; set; }
        public virtual CELDA CELDA1 { get; set; }
        public virtual EDIFICIO EDIFICIO { get; set; }
        public virtual SECTOR SECTOR { get; set; }
        public virtual TIPO_VISITA TIPO_VISITA { get; set; }
        public virtual VISITA_DIA VISITA_DIA { get; set; }
    }
}
