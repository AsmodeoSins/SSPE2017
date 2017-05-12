using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cControlVisita
    {
        public string Hora_Ingreso { get; set; }
        public string Hora_Salida { get; set; }
        public int Id_Persona { get; set; }
        public string Nombre_Visitante { get; set; }
        public string Paterno_Visitante { get; set; }
        public string Materno_Visitante { get; set; }
        public short Centro { get; set; }
        public short Id_Anio { get; set; }
        public int Id_Imputado { get; set; }
        public string Nombre_Ingreso { get; set; }
        public string Paterno_Ingreso { get; set; }
        public string Materno_Ingreso { get; set; }
        public string Ubicacion_Ingreso { get; set; }
        public string Usuario { get; set; }
        public string DioSalida { get; set; }
        public string Tipo_Visita { get; set; }
        public string Categoria { get; set; }
        public string Intima { get; set; }
    }

    public class cControlVisitaTotales
    {
        public int VisitantesIngresaron { get; set; }
        public int VisitantesSalieron { get; set; }
        public int InternosVisitados { get; set; }
        public int VisitanteIntimaIngresaron { get; set; }
        public int VisitanteIntimaSalieron { get; set; }
    }

    public class cControlVisitas
    {
        public DateTime ENTRADA_FEC { get; set; }
        public int ID_PERSONA { get; set; }
        public string VISITA_NOMBRE { get; set; }
        public string VISITA_PATERNO { get; set; }
        public string VISITA_MATERNO { get; set; }
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public string INTERNO_NOMBRE { get; set; }
        public string INTERNO_PATERNO { get; set; }
        public string INTERNO_MATERNO { get; set; }
        public DateTime SALIDA_FEC { get; set; }
        public int ID_TIPO_PERSONA { get; set; }
        public string TIPO_VISITA { get; set; }
        public string INTIMA { get; set; }
        public string EDIFICIO { get; set; }
        public string SECTOR { get; set; }
        public int CELDA { get; set; }
        public int CAMA { get; set; }
    }

}
