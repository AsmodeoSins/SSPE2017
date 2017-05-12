using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public class ReporteKardexInterno
    {
        public string Interno { get; set; }
        public string Expediente { get; set; }
        public string Ingreso { get; set; }
        public string Ubicacion { get; set; }
        public string FechaIngreso { get; set; }
        public string Planimetria { get; set; }
        public string Avance { get; set; }
        public string HorasTratamiento { get; set; }
        public string Estatus { get; set; }
        public string HistorialSanciones { get; set; }
    }

    public class ReporteKardexInternoActividades
    {
        public string Eje { get; set; }
        public string Programa { get; set; }
        public string Actividad { get; set; }
        public string Grupo { get; set; }
        public string ID_Grupo { get; set; }
        public string Inicio { get; set; }
        public string Fin { get; set; }
        public string Asistencia { get; set; }
        public string Nota_Tecnica { get; set; }
        public string Acreditado { get; set; }
    }

    public class ReporteKardexInternoHorario
    {
        public string EJE { get; set; }
        public string PROGRAMA { get; set; }
        public string ACTIVIDAD { get; set; }
        public string GRUPO { get; set; }
        public string ID_Grupo { get; set; }
        public string FECHA { get; set; }
        public string HORARIO { get; set; }
        public bool? ASISTENCIA { get; set; }
        public string INICIO { get; set; }
        public string FIN { get; set; }
        public string nASISTENCIA { get; set; }
        public string FALTAS { get; set; }
        public string JUSTIFICADAS { get; set; }

        public string AREA { get; set; }

        public string ESTATUS { get; set; }
        public DateTime? FechaHorario { get; set; }
        public Visibility ShowCheck { get; set; }
        public Visibility ShowLabel { get; set; }
    }

    public class ReporteKardexHorasEmpalmadas
    {
        public string Dia_Header { get; set; }

        public string EJE { get; set; }
        public string PROGRAMA { get; set; }
        public string ACTIVIDAD { get; set; }
        public string GRUPO { get; set; }
        public string Horario { get; set; }
        public bool Elegida { get; set; }
    }

    public class ReporteKardexSanciones
    {
        public string EJE { get; set; }
        public string PROGRAMA { get; set; }
        public string ACTIVIDAD { get; set; }
        public string GRUPO { get; set; }

        public string RESPONSABLE { get; set; }
        public string SOLICITUD_FECHA { get; set; }
        public string RESPUESTA_FECHA { get; set; }
        public string MOTIVO { get; set; }
        public string RESPUESTA { get; set; }
        public string ESTATUS { get; set; }
    }
}
