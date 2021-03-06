//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Datos
{
    using System;
    using System.Collections.Generic;
    
    public partial class imputado
    {
        public imputado()
        {
            this.expedientes = new HashSet<expediente>();
        }
    
        public int id_imputado { get; set; }
        public string nombre { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }
        public string alias { get; set; }
        public Nullable<System.DateTime> fecha_nacimiento { get; set; }
        public string sexo { get; set; }
        public string estado_civil { get; set; }
        public string ciudad_nacimiento { get; set; }
        public string pais_nacimiento { get; set; }
        public string nacionalidad { get; set; }
        public string etnia { get; set; }
        public string religion { get; set; }
        public Nullable<int> anios_bc { get; set; }
        public Nullable<int> meses_bc { get; set; }
        public Nullable<int> dias_bc { get; set; }
        public string domicilio { get; set; }
        public string municipio { get; set; }
        public string estado { get; set; }
    
        public virtual ICollection<expediente> expedientes { get; set; }
    }
}
