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
    
    public partial class EXCARCELACION_DESTINO
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public int ID_CONSEC { get; set; }
        public Nullable<short> ID_JUZGADO { get; set; }
        public Nullable<short> ID_TIPO_DOC { get; set; }
        public byte[] DOCUMENTO { get; set; }
        public Nullable<short> ID_FORMATO { get; set; }
        public string ID_ESTATUS { get; set; }
        public string FOLIO_DOC { get; set; }
        public int ID_DESTINO { get; set; }
        public Nullable<short> CAU_ID_CENTRO { get; set; }
        public Nullable<short> CAU_ID_ANIO { get; set; }
        public Nullable<int> CAU_ID_IMPUTADO { get; set; }
        public Nullable<short> CAU_ID_INGRESO { get; set; }
        public Nullable<short> CAU_ID_CAUSA_PENAL { get; set; }
        public Nullable<short> CAN_ID_MOTIVO { get; set; }
        public string CANCELADO_OBSERVA { get; set; }
        public string CAUSA_PENAL_TEXTO { get; set; }
        public string CANCELADO_TIPO { get; set; }
        public Nullable<int> ID_INTERSOL { get; set; }
        public Nullable<short> ID_CENTRO_UBI { get; set; }
    
        public virtual CAUSA_PENAL CAUSA_PENAL { get; set; }
        public virtual EXCARCELACION_CANCELA_MOTIVO EXCARCELACION_CANCELA_MOTIVO { get; set; }
        public virtual EXCARCELACION_TIPO_DOCTO EXCARCELACION_TIPO_DOCTO { get; set; }
        public virtual EXCARCELACION_DESTINO_ESTATUS EXCARCELACION_DESTINO_ESTATUS { get; set; }
        public virtual FORMATO_DOCUMENTO FORMATO_DOCUMENTO { get; set; }
        public virtual JUZGADO JUZGADO { get; set; }
        public virtual EXCARCELACION EXCARCELACION { get; set; }
        public virtual INTERCONSULTA_SOLICITUD INTERCONSULTA_SOLICITUD { get; set; }
    }
}