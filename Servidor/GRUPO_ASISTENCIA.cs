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
    
    public partial class GRUPO_ASISTENCIA
    {
        public short ID_CENTRO { get; set; }
        public short ID_TIPO_PROGRAMA { get; set; }
        public short ID_ACTIVIDAD { get; set; }
        public short ID_GRUPO { get; set; }
        public short ID_GRUPO_HORARIO { get; set; }
        public short ID_CONSEC { get; set; }
        public Nullable<System.DateTime> FEC_REGISTRO { get; set; }
        public Nullable<short> ASISTENCIA { get; set; }
        public Nullable<int> EMPALME { get; set; }
        public Nullable<decimal> EMP_COORDINACION { get; set; }
        public Nullable<decimal> EMP_APROBADO { get; set; }
        public Nullable<System.DateTime> EMP_FECHA { get; set; }
        public Nullable<short> ESTATUS { get; set; }
    
        public virtual GRUPO_PARTICIPANTE GRUPO_PARTICIPANTE { get; set; }
        public virtual GRUPO_ASISTENCIA_ESTATUS GRUPO_ASISTENCIA_ESTATUS { get; set; }
        public virtual GRUPO_HORARIO GRUPO_HORARIO { get; set; }
    }
}
