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
    
    public partial class CORRESPONDENCIA
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_CONSEC { get; set; }
        public string REMITENTE { get; set; }
        public Nullable<System.DateTime> RECEPCION_FEC { get; set; }
        public Nullable<int> ID_DEPOSITANTE { get; set; }
        public Nullable<int> ID_EMPLEADO { get; set; }
        public string OBSERV { get; set; }
        public Nullable<System.DateTime> ENTREGA_FEC { get; set; }
        public string CONFIRMACION_RECIBIDO { get; set; }
    
        public virtual PERSONA PERSONA { get; set; }
        public virtual PERSONA PERSONA1 { get; set; }
        public virtual INGRESO INGRESO { get; set; }
    }
}
