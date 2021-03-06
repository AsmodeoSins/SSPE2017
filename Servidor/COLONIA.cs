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
    
    public partial class COLONIA
    {
        public COLONIA()
        {
            this.IMPUTADO_PADRES = new HashSet<IMPUTADO_PADRES>();
            this.VISITA_AUTORIZADA = new HashSet<VISITA_AUTORIZADA>();
            this.PFF_TRABAJO_SOCIAL = new HashSet<PFF_TRABAJO_SOCIAL>();
            this.PERSONA = new HashSet<PERSONA>();
            this.INGRESO = new HashSet<INGRESO>();
            this.UNIDAD_RECEPTORA = new HashSet<UNIDAD_RECEPTORA>();
            this.PROCESO_LIBERTAD = new HashSet<PROCESO_LIBERTAD>();
        }
    
        public int ID_COLONIA { get; set; }
        public Nullable<short> ID_ENTIDAD { get; set; }
        public Nullable<short> ID_MUNICIPIO { get; set; }
        public string DESCR { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual MUNICIPIO MUNICIPIO { get; set; }
        public virtual ICollection<IMPUTADO_PADRES> IMPUTADO_PADRES { get; set; }
        public virtual ICollection<VISITA_AUTORIZADA> VISITA_AUTORIZADA { get; set; }
        public virtual ICollection<PFF_TRABAJO_SOCIAL> PFF_TRABAJO_SOCIAL { get; set; }
        public virtual ICollection<PERSONA> PERSONA { get; set; }
        public virtual ICollection<INGRESO> INGRESO { get; set; }
        public virtual ICollection<UNIDAD_RECEPTORA> UNIDAD_RECEPTORA { get; set; }
        public virtual ICollection<PROCESO_LIBERTAD> PROCESO_LIBERTAD { get; set; }
    }
}
