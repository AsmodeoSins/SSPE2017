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
    
    public partial class INGRESO_DELITO
    {
        public INGRESO_DELITO()
        {
            this.EMI_ANTECEDENTE_FAM_CON_DEL = new HashSet<EMI_ANTECEDENTE_FAM_CON_DEL>();
            this.INGRESO = new HashSet<INGRESO>();
        }
    
        public short ID_INGRESO_DELITO { get; set; }
        public string DESCR { get; set; }
        public string ID_FUERO { get; set; }
    
        public virtual ICollection<EMI_ANTECEDENTE_FAM_CON_DEL> EMI_ANTECEDENTE_FAM_CON_DEL { get; set; }
        public virtual ICollection<INGRESO> INGRESO { get; set; }
    }
}