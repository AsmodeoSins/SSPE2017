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
    
    public partial class UNIDAD_RECEPTORA
    {
        public UNIDAD_RECEPTORA()
        {
            this.AGENDA_ACTIVIDAD_LIBERTAD = new HashSet<AGENDA_ACTIVIDAD_LIBERTAD>();
        }
    
        public decimal ID_UNIDAD_RECEPTORA { get; set; }
        public string NOMBRE { get; set; }
        public string DESCRIPCION { get; set; }
        public Nullable<short> ID_ENTIDAD { get; set; }
        public Nullable<short> ID_MUNICIPIO { get; set; }
        public Nullable<int> ID_COLONIA { get; set; }
        public string CALLE_DIRECCION { get; set; }
        public string NUM_INT_DIRECCION { get; set; }
        public string NUM_EXT_DIRECCION { get; set; }
        public string CP_DIRECCION { get; set; }
        public Nullable<long> TELEFONO { get; set; }
        public string ESTATUS { get; set; }
    
        public virtual ICollection<AGENDA_ACTIVIDAD_LIBERTAD> AGENDA_ACTIVIDAD_LIBERTAD { get; set; }
        public virtual COLONIA COLONIA { get; set; }
        public virtual MUNICIPIO MUNICIPIO { get; set; }
    }
}