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
    
    public partial class expediente
    {
        public expediente()
        {
            this.expediente_ingreso = new HashSet<expediente_ingreso>();
        }
    
        public int id_expediente { get; set; }
        public Nullable<int> anio { get; set; }
        public Nullable<int> folio { get; set; }
        public string causa_penal { get; set; }
        public Nullable<int> id_imputado { get; set; }
        public Nullable<int> id_cereso { get; set; }
        public string imagen { get; set; }
    
        public virtual cereso cereso { get; set; }
        public virtual imputado imputado { get; set; }
        public virtual ICollection<expediente_ingreso> expediente_ingreso { get; set; }
    }
}
