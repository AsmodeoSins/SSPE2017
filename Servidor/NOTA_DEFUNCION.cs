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
    
    public partial class NOTA_DEFUNCION
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public Nullable<System.DateTime> FECHA_DECESO { get; set; }
        public string LUGAR { get; set; }
        public Nullable<int> ID_ENFERMEDAD { get; set; }
        public string HECHOS { get; set; }
        public Nullable<int> ID_EMPLEADO_COORDINADOR_MED { get; set; }
        public byte[] TARJETA_DECESO { get; set; }
        public Nullable<System.DateTime> FECHA_REGISTRO { get; set; }
        public string USUARIO_REGISTRO { get; set; }
    
        public virtual EMPLEADO EMPLEADO { get; set; }
        public virtual ENFERMEDAD ENFERMEDAD { get; set; }
        public virtual INGRESO INGRESO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
