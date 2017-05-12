using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;

namespace ControlPenales
{
    public class CT_EXCARCELACION_DESTINO
    {
        public int? ID_CONSEC { get; set; }
        public short? ID_DESTINO { get; set; }
        public string DESTINO { get; set; }
        public byte[] DOCUMENTO { get; set; }
        public short? FORMATO_DOCUMENTO { get; set; }
        public short? TIPO_DOCUMENTO { get; set; }
        public string FOLIO { get; set; }
        public string ESTATUS { get; set; }
        public CAUSA_PENAL CAUSA_PENAL { get; set; }
        public short? ID_CANCELACION_MOTIVO { get; set; }
        public string CANCELACION_OBSERVACION { get; set; }
        public string CAUSA_PENAL_TEXTO { get; set; }
        public byte[] CERTIFICADO_MEDICO { get; set; }
        public bool CERTIFICADO_MEDICO_ENABLED { get; set; }
        public int? ID_INTERC { get; set; }
        public DateTime? FechaInterconsultaDestino { get; set; }

        //para la vista ExcarcelacionesCancelarView
        public DateTime FECHA_EXCARCELACION { get; set; }

        public short? ID_CENTRO_UBI { get; set; }
    }
}
