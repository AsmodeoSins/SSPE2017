using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cReporteProcesoLibertad
    {
        public string NOMBRE { set; get; }
        public string PATERNO { set; get; }
        public string MATERNO { set; get; }
        public string EDAD { set; get; }
        public string FECHA { set; get; }
        public byte[] FOTO { set; get; }

        public string COMPLEXION { set; get; }
        public string COLOR_PIEL { set; get; }
        public string ESTATURA { set; get; }
        public string PESO { set; get; }
        public string CABELLO_COLOR { set; get; }
        public string CABELLO_FORMA { set; get; }

        public string CAUSA_PENAL { set; get; }
        public string JUZGADO { set; get; }
        public string DELITO { set; get; }

    }

    public class cReporteProcesoLibertadMedidas
    {
        public string OBLIGACIONES { set; get; }
    }

    public class cReporteProcesoLibertadHuellas
    {
        public byte[] PULGAR { set; get; }
        public byte[] INDICE { set; get; }
        public byte[] ANULAR { set; get; }
        public byte[] MEDIO { set; get; }
        public byte[] MENIQUE { set; get; }
    }

    public class cReporteProcesoLibertadLogos
    {
        public byte[] LOGO1 { set; get; }
        public byte[] LOGO2 { set; get; }
    }
    
}
