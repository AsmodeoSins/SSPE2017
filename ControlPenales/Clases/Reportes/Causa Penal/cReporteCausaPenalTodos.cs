using System;
using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cReporteCausaPenalTodos
    {
        public int ID_CENTRO { set; get; }
        public int ID_ANIO { set; get; }
        public int ID_IMPUTADO { set; get; }
        public int ID_INGRESO { set; get; }
        public string NOMBRE { set; get; }
        public string PATERNO { set; get; }
        public string MATERNO { set; get; }
        public string CLASIFICACION_JURIDICA { set; get; }
    }

    public class cReporteCausaPenalTodosDetalle
    {
        public int ID_CENTRO { set; get; }
        public int ID_ANIO { set; get; }
        public int ID_IMPUTADO { set; get; }
        public int ID_INGRESO { set; get; }
        public int CP_ANIO { set; get; }
        public int CP_FOLIO { set; get; }
        public string FUERO { set; get; }
        public string JUZGADO { set; get; }
        public int SETENCIA_ANIO { set; get; }
        public int SENTENCIA_MES { set; get; }
        public int SENTENCIA_DIA { set; get; }
        public DateTime FEC_INICIO_COMPURGACION { set; get; }
        public int SENTENCIA_DIAS { set; get; }
        public int SENTENCIA_DIAS_COMPURGADO { set; get; }
        public double SENTENCIA_DIAS_PORCENTAJE { set; get; }
        public string DELITOS { set; get; }
    }
}
