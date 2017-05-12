using System;
using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cReporteCausaPenal
    {
        public int ID_ANIO { set; get; }
        public int ID_IMPUTADO { set; get; }
        public string NOMBRE { set; get; }
        public string PATERNO { set; get; }
        public string MATERNO { set; get; }
        public int ANIO_GOBIERNO { set; get; }
        public string FOLIO_GOBIERNO { set; get; }
        public DateTime? FECHA_INICIO { set; get; }
        public int? S_ANIO { set; get; }
        public int? S_MES { set; get; }
        public int? S_DIA { set; get; }
        public int? C_ANIO { set; get; }
        public int? C_MES { set; get; }
        public int? C_DIA { set; get; }
        public int? A_ANIO { set; get; }
        public int? A_MES { set; get; }
        public int? A_DIA { set; get; }
        public int? PC_ANIO { set; get; }
        public int? PC_MES { set; get; }
        public int? PC_DIA { set; get; }
        public int ID_UB_CENTRO { set; get; }
    }
}
