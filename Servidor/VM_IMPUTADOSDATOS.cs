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
    
    public partial class VM_IMPUTADOSDATOS
    {
        public long EXPEDIENTEID { get; set; }
        public int PERSONAFISICAID { get; set; }
        public Nullable<short> TIPOIDENTIFICACIONID { get; set; }
        public string NUMEROIDENTIFICACION { get; set; }
        public string NOMBRE { get; set; }
        public string PRIMERAPELLIDO { get; set; }
        public string SEGUNDOAPELLIDO { get; set; }
        public byte[] FOTO { get; set; }
        public Nullable<short> NACIONALIDADID { get; set; }
        public Nullable<short> ESTADOORIGENID { get; set; }
        public Nullable<short> MUNICIPIOORIGENID { get; set; }
        public string TELEFONO { get; set; }
        public Nullable<System.DateTime> FECHANACIMIENTO { get; set; }
        public Nullable<short> EDADCANTIDAD { get; set; }
        public string EDADTIEMPO { get; set; }
        public Nullable<short> ESTADOCIVILID { get; set; }
        public string SEXO { get; set; }
    }
}
