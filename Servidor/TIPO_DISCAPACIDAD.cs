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
    
    public partial class TIPO_DISCAPACIDAD
    {
        public TIPO_DISCAPACIDAD()
        {
            this.EMI_ENFERMEDAD = new HashSet<EMI_ENFERMEDAD>();
            this.PERSONA = new HashSet<PERSONA>();
            this.INGRESO = new HashSet<INGRESO>();
            this.PROCESO_LIBERTAD = new HashSet<PROCESO_LIBERTAD>();
        }
    
        public short ID_TIPO_DISCAPACIDAD { get; set; }
        public string DESCR { get; set; }
        public string HUELLA { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<EMI_ENFERMEDAD> EMI_ENFERMEDAD { get; set; }
        public virtual ICollection<PERSONA> PERSONA { get; set; }
        public virtual ICollection<INGRESO> INGRESO { get; set; }
        public virtual ICollection<PROCESO_LIBERTAD> PROCESO_LIBERTAD { get; set; }
    }
}