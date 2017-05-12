using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cReporteEscalaRiesgo
    {
        public string FECHA_LUGAR { set; get; }
        public string NOMBRE { set; get; }
        public string NUC { set; get; }
        public string EVALUADOR { set; get; }
        public string FECHA { set; get; }
    }

    public class cReporteEscalaRiesgoDetalle
    {
        public string GRUPO { set; get; }
        public string DESCRIPCION{ set; get; }
        public short VALOR { set; get; }
        public short SELECCION { set; get; }
    }

    public class cReporteEscalaRiesgoResultado
    {
        public short? A1 { set; get; }
        public short? B1 { set; get; }
        public short? C1 { set; get; }
        public short? D1 { set; get; }
        public short? E1 { set; get; }
        public short? F1 { set; get; }
        public short? G1 { set; get; }
        public short? H1 { set; get; }
        public short? I1 { set; get; }
        public short? J1 { set; get; }
        public short? TOTAL { set; get; }
    }

    public class cReporteEscalaRiesgoNivel
    {
        public string NIVEL { set; get; }
        public int EN_RANGO { set; get; }
    }

    
}
