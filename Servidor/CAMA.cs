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
    
    public partial class CAMA
    {
        public CAMA()
        {
            this.INGRESO_UBICACION_ANT = new HashSet<INGRESO_UBICACION_ANT>();
            this.INGRESO = new HashSet<INGRESO>();
        }
    
        public short ID_CENTRO { get; set; }
        public short ID_EDIFICIO { get; set; }
        public short ID_SECTOR { get; set; }
        public string ID_CELDA { get; set; }
        public short ID_CAMA { get; set; }
        public string ESTATUS { get; set; }
        public string DESCR { get; set; }
    
        public virtual CELDA CELDA { get; set; }
        public virtual SECTOR_OBSERVACION_CELDA SECTOR_OBSERVACION_CELDA { get; set; }
        public virtual ICollection<INGRESO_UBICACION_ANT> INGRESO_UBICACION_ANT { get; set; }
        public virtual ICollection<INGRESO> INGRESO { get; set; }
    }
}