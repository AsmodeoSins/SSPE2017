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
    
    public partial class EMI_SANCION_DISCIPLINARIA
    {
        public int ID_EMI { get; set; }
        public short ID_EMI_CONS { get; set; }
        public int ID_SANCIONES_DISCIPLINARIAS { get; set; }
        public string MOTIVO_PROCESO { get; set; }
        public Nullable<short> CANTIDAD_PARTICIPACION { get; set; }
        public string TIEMPO_CASTIGO_SANCION_PROCESO { get; set; }
        public string NUEVO_PROCESO { get; set; }
    
        public virtual EMI EMI { get; set; }
    }
}
