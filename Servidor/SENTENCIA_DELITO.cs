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
    
    public partial class SENTENCIA_DELITO
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_SENTENCIA { get; set; }
        public short ID_CAUSA_PENAL { get; set; }
        public long ID_DELITO { get; set; }
        public string ID_FUERO { get; set; }
        public Nullable<short> ID_MODALIDAD { get; set; }
        public Nullable<short> ID_TIPO_DELITO { get; set; }
        public string DESCR_DELITO { get; set; }
        public string CANTIDAD { get; set; }
        public string OBJETO { get; set; }
        public short ID_CONS { get; set; }
    
        public virtual SENTENCIA SENTENCIA { get; set; }
        public virtual MODALIDAD_DELITO MODALIDAD_DELITO { get; set; }
        public virtual TIPO_DELITO TIPO_DELITO { get; set; }
    }
}
