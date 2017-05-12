using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor.ModelosExtendidos
{
    public class DOCUMENTO_SISTEMA
    {
        private string mensaje = string.Empty;
        public string MENSAJE
        {
            get { return mensaje; }
            set { mensaje = value; }
        }

        private byte[] documento = null;
        public byte[] DOCUMENTO
        {
            get { return documento; }
            set { documento = value; }
        }

        private DateTime? registro_fec = null;
        public DateTime? REGISTRO_FEC
        {
            get { return registro_fec; }
            set { registro_fec = value; }
        }

        private short? formato_documento;
        public short? FORMATO_DOCUMENTO
        {
            get { return formato_documento; }
            set { formato_documento = value; }
        }

        private string nuc;
        public string NUC
        {
            get { return nuc; }
            set { nuc = value; }
        }

        private CAUSA_PENAL causa_penal;
        public CAUSA_PENAL CAUSA_PENAL
        {
            get { return causa_penal; }
            set { causa_penal = value; }
        }

        private short? cp_anio;
        public short? CP_ANIO
        {
            get { return cp_anio; }
            set { cp_anio = value; }
        }
        private int? cp_folio;
        public int? CP_FOLIO
        {
            get { return cp_folio; }
            set { cp_folio = value; }
        }
    }
}
