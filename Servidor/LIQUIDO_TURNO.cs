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
    
    public partial class LIQUIDO_TURNO
    {
        public LIQUIDO_TURNO()
        {
            this.LIQUIDO_HORA = new HashSet<LIQUIDO_HORA>();
            this.HOJA_ENFERMERIA_SOLUCION = new HashSet<HOJA_ENFERMERIA_SOLUCION>();
            this.HOJA_ENFERMERIA = new HashSet<HOJA_ENFERMERIA>();
        }
    
        public decimal ID_LIQTURNO { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<LIQUIDO_HORA> LIQUIDO_HORA { get; set; }
        public virtual ICollection<HOJA_ENFERMERIA_SOLUCION> HOJA_ENFERMERIA_SOLUCION { get; set; }
        public virtual ICollection<HOJA_ENFERMERIA> HOJA_ENFERMERIA { get; set; }
    }
}
