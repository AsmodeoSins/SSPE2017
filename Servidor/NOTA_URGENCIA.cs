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
    
    public partial class NOTA_URGENCIA
    {
        public int ID_ATENCION_MEDICA { get; set; }
        public string MOTIVO_CONSULTA { get; set; }
        public string ESTADO_MENTAL { get; set; }
        public string DESTINO_DESPUES_UR { get; set; }
        public string PROCEDIMIENTO_AREA { get; set; }
        public Nullable<System.DateTime> REGISTRO_FEC { get; set; }
        public short ID_CENTRO_UBI { get; set; }
    
        public virtual ATENCION_MEDICA ATENCION_MEDICA { get; set; }
    }
}
