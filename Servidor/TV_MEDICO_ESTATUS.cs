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
    
    public partial class TV_MEDICO_ESTATUS
    {
        public TV_MEDICO_ESTATUS()
        {
            this.TV_CANALIZACION = new HashSet<TV_CANALIZACION>();
            this.TV_CANALIZACION_ESPECIALIDAD = new HashSet<TV_CANALIZACION_ESPECIALIDAD>();
            this.TV_CITA_MEDICA = new HashSet<TV_CITA_MEDICA>();
            this.TV_CANALIZACION_SERV_AUX = new HashSet<TV_CANALIZACION_SERV_AUX>();
            this.TV_INTERCONSULTA_SOLICITUD = new HashSet<TV_INTERCONSULTA_SOLICITUD>();
        }
    
        public string ID_TV_MEDICO_ESTATUS { get; set; }
        public string TV_MEDICO_ESTATUS1 { get; set; }
    
        public virtual ICollection<TV_CANALIZACION> TV_CANALIZACION { get; set; }
        public virtual ICollection<TV_CANALIZACION_ESPECIALIDAD> TV_CANALIZACION_ESPECIALIDAD { get; set; }
        public virtual ICollection<TV_CITA_MEDICA> TV_CITA_MEDICA { get; set; }
        public virtual ICollection<TV_CANALIZACION_SERV_AUX> TV_CANALIZACION_SERV_AUX { get; set; }
        public virtual ICollection<TV_INTERCONSULTA_SOLICITUD> TV_INTERCONSULTA_SOLICITUD { get; set; }
    }
}
