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
    
    public partial class DECOMISO
    {
        public DECOMISO()
        {
            this.DECOMISO_INGRESO = new HashSet<DECOMISO_INGRESO>();
            this.DECOMISO_OBJETO = new HashSet<DECOMISO_OBJETO>();
            this.DECOMISO_PERSONA = new HashSet<DECOMISO_PERSONA>();
        }
    
        public int ID_DECOMISO { get; set; }
        public Nullable<short> ID_CENTRO { get; set; }
        public Nullable<short> ID_EDIFICIO { get; set; }
        public Nullable<short> ID_SECTOR { get; set; }
        public string ID_CELDA { get; set; }
        public Nullable<short> ID_AREA { get; set; }
        public Nullable<short> ID_GRUPO_TACTICO { get; set; }
        public Nullable<short> ID_TURNO { get; set; }
        public string RESUMEN { get; set; }
        public Nullable<System.DateTime> EVENTO_FEC { get; set; }
        public Nullable<System.DateTime> INFORME_FEC { get; set; }
        public string OFICIO { get; set; }
        public string ID_USUARIO { get; set; }
        public Nullable<System.DateTime> SYSDATE_FEC { get; set; }
        public string OFICIO_SEGURIDAD { get; set; }
        public string JEFE_TURNO { get; set; }
        public string COMANDANTE { get; set; }
        public string OFICIO_COMAN1 { get; set; }
        public string OFICIO_COMAN2 { get; set; }
    
        public virtual AREA AREA { get; set; }
        public virtual CELDA CELDA { get; set; }
        public virtual ICollection<DECOMISO_INGRESO> DECOMISO_INGRESO { get; set; }
        public virtual ICollection<DECOMISO_OBJETO> DECOMISO_OBJETO { get; set; }
        public virtual ICollection<DECOMISO_PERSONA> DECOMISO_PERSONA { get; set; }
        public virtual TURNO TURNO { get; set; }
        public virtual GRUPO_TACTICO GRUPO_TACTICO { get; set; }
    }
}
