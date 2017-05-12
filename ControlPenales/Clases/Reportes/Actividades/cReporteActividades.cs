using System;
using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cReporteActividades
    {
        public string TipoPrograma { set; get; }
        public string TipoActividad { set; get; }
        public int ComunProcesado { set; get; }
        public int ComunSentenciado { set; get; }
        public int FederalProcesado { set; get; }
        public int SinFueroImputado { set; get; }
        public int SinFueroIndiciado { set; get; }
        public int SinFueroProcesado { set; get; }
        public int SinFueroSentenciado { set; get; }
    }

    public class cReporteActividadesGrafica
    {
        public string TipoPrograma { set; get; }
        public string ClasificacionJuridica { set; get; }
        public string Color { set; get; }
        public int Total { set; get; }
    }

    public class cReporteActividadesTitulos
    {
        public string Titulo { set; get; }
        public string Color { set; get; }
    }

}
