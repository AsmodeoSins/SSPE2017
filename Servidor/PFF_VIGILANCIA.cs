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
    
    public partial class PFF_VIGILANCIA
    {
        public PFF_VIGILANCIA()
        {
            this.PFF_CORRECTIVO = new HashSet<PFF_CORRECTIVO>();
        }
    
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_ESTUDIO { get; set; }
        public string NOMBRE { get; set; }
        public Nullable<System.DateTime> FECHA_INGRESO { get; set; }
        public string CENTRO_DONDE_PROCEDE { get; set; }
        public string CONDUCTA { get; set; }
        public string MOTIVO_TRASLADO { get; set; }
        public string CONDUCTA_SUPERIORES { get; set; }
        public string RELACION_COMPANEROS { get; set; }
        public string DESCRIPCION_CONDUCTA { get; set; }
        public string HIGIENE_PERSONAL { get; set; }
        public string HIGIENE_CELDA { get; set; }
        public string VISITA_RECIBE { get; set; }
        public string VISITA_FRECUENCIA { get; set; }
        public string VISITA_QUIENES { get; set; }
        public string CONDUCTA_FAMILIA { get; set; }
        public string ESTIMULOS_BUENA_CONDUCTA { get; set; }
        public string CONDUCTA_GENERAL { get; set; }
        public string CONCLUSIONES { get; set; }
        public string LUGAR { get; set; }
        public Nullable<System.DateTime> FECHA { get; set; }
        public string DIRECTOR_CENTRO { get; set; }
        public string JEFE_VIGILANCIA { get; set; }
    
        public virtual ICollection<PFF_CORRECTIVO> PFF_CORRECTIVO { get; set; }
        public virtual PERSONALIDAD_FUERO_FEDERAL PERSONALIDAD_FUERO_FEDERAL { get; set; }
    }
}
