using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class EXT_DELITOS
    {
        private List<CAUSA_PENAL_DELITO> delitos;
        public List<CAUSA_PENAL_DELITO> DELITOS
        {
            get { return delitos; }
            set { delitos = value; }
        }
        public string DELITOS_DESCR
        {
            get
            {
                StringBuilder strbuilder = new StringBuilder();
                foreach(var item in DELITOS)
                {
                    if (strbuilder.Length > 0)
                        strbuilder.Append(", ");
                    strbuilder.Append(item.DESCR_DELITO);
                    
                }
                return strbuilder.ToString();
            }
        }
        private string cp_folio;
        public string CP_FOLIO
        {
            get { return cp_folio; }
            set { cp_folio = value; }
        }
        private string cp_anio;
        public string CP_ANIO
        {
            get { return cp_anio; }
            set { cp_anio = value; }
        }
        private string cp_bis;
        public string CP_BIS
        {
            get { return cp_bis; }
            set { cp_bis = value; }
        }
        public string CAUSA_PENAL
        {
            get 
            {
                if (!string.IsNullOrWhiteSpace(cp_bis))
                    return CP_ANIO + "/" + CP_FOLIO + "-" + CP_BIS;
                else
                    return CP_ANIO + "/" + CP_FOLIO;
            }
        }
    }
}
