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
    
    public partial class PARAMETRO
    {
        public short ID_CENTRO { get; set; }
        public string ID_CLAVE { get; set; }
        public string VALOR { get; set; }
        public byte[] CONTENIDO { get; set; }
        public Nullable<int> VALOR_NUM { get; set; }
    
        public virtual PARAMETRO_CLAVE PARAMETRO_CLAVE { get; set; }
    }
}
