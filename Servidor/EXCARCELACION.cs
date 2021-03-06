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
    
    public partial class EXCARCELACION
    {
        public EXCARCELACION()
        {
            this.EXCARCELACION_DESTINO = new HashSet<EXCARCELACION_DESTINO>();
            this.NOTA_EGRESO = new HashSet<NOTA_EGRESO>();
        }
    
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public int ID_CONSEC { get; set; }
        public string ID_USUARIO { get; set; }
        public Nullable<short> ID_TIPO_EX { get; set; }
        public string ID_ESTATUS { get; set; }
        public Nullable<System.DateTime> REGISTRO_FEC { get; set; }
        public Nullable<System.DateTime> SALIDA_FEC { get; set; }
        public Nullable<System.DateTime> RETORNO_FEC { get; set; }
        public string OBSERVACION { get; set; }
        public Nullable<decimal> CERTIFICADO_MEDICO { get; set; }
        public Nullable<System.DateTime> PROGRAMADO_FEC { get; set; }
        public string CANCELADO_TIPO { get; set; }
        public Nullable<int> CERT_MEDICO_SALIDA { get; set; }
        public Nullable<int> CERT_MEDICO_RETORNO { get; set; }
        public string RESPONSABLE { get; set; }
        public Nullable<short> ID_INCIDENCIA_TRASLADO { get; set; }
        public string INCIDENCIA_OBSERVACION { get; set; }
        public Nullable<short> ID_INCIDENCIA_TRASLADO_RETORNO { get; set; }
        public string INCIDENCIA_OBSERVACION_RETORNO { get; set; }
        public Nullable<short> ID_CENTRO_UBI { get; set; }
    
        public virtual ATENCION_MEDICA ATENCION_MEDICA { get; set; }
        public virtual ATENCION_MEDICA ATENCION_MEDICA1 { get; set; }
        public virtual EXCARCELACION_ESTATUS EXCARCELACION_ESTATUS { get; set; }
        public virtual ICollection<EXCARCELACION_DESTINO> EXCARCELACION_DESTINO { get; set; }
        public virtual EXCARCELACION_TRANSITO EXCARCELACION_TRANSITO { get; set; }
        public virtual EXCARCELACION_TIPO EXCARCELACION_TIPO { get; set; }
        public virtual INGRESO INGRESO { get; set; }
        public virtual TRASLADO_INCIDENCIA_TIPO TRASLADO_INCIDENCIA_TIPO { get; set; }
        public virtual TRASLADO_INCIDENCIA_TIPO TRASLADO_INCIDENCIA_TIPO1 { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual ICollection<NOTA_EGRESO> NOTA_EGRESO { get; set; }
    }
}
