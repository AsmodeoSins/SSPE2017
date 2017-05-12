using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cInternos
    {
       public cInternos()
        {

        }

        public cInternos(TimeSpan? HoraDeInicio = null, TimeSpan? HoraDeFin = null)
        {
            Hora = HoraDeInicio;
            HoraFin = HoraDeFin;
        }

        #region [VARIABLES]
        public string LlaveInterno { get; set; }
        public int? NIP { get; set; }
        public short? IdArea { get; set; }
        public short IdIngreso { get; set; }
        public int? IdAduana { get; set; }// NECESARIO PARA SABER SI TERMINO SU VISITA
        public string Expediente { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Ubicacion { get; set; }// UBICACION ACTUAL DEL INTERNO
        public string Estancia { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaTermino { get; set; }
        public TimeSpan? Hora { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public string Area { get; set; }
        public string Actividad { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int IdImputado { get; set; }
        public short Anio { get; set; }
        public short Centro { get; set; }
        public int Id_Grupo { get; set; }
        public int? Empalme { get; set; } //SECUENCIAL DEL EMPALME!
        public decimal? EmpalmeAprobado { get; set; }
        public decimal? EmpalmeCoordinacion { get; set; }
        public short? Prioridad { get; set; }
        public bool Comparado { get; set; }
        public bool TrasladoInterno { get; set; }
        public bool ExcarcelacionInterno { get; set; }
        public bool CitaMedica { get; set; }
        public bool BooleanToRowTraslado { get; set; }// EN ROJO SI INTERNO ES TRUE PARA TRASLADO
        public bool BooleanToRowExcarcelacion { get; set; }// EN AMARILLO SI INTERNO ES TRUE PARA EXCARCELACION
        public bool BooleanToRowCitaMedica { get; set; } // EN CAFE SI INTERNO ES TRUE PARA CITA MEDICA
        public bool BooleanToRowVisitaLegal { get; set; } // EN VERDE SI INTERNO ES TRUE PARA VISITA LEGAL
        public bool BooleanToRowVisitaFamiliar { get; set; } // EN PURPLE SI INTERNO ES TRUE PARA VISITA FAMILIAR
        public bool BooleanToRowVisitaIntima { get; set; } // EN MAGENTA SI INTERNO ES TRUE PARA VISITA INTIMA
        public bool YardaInterno { get; set; }
        public short Tolerancia = Parametro.TOLERANCIA_ACTIVIDAD_EDIFICIO;
        public INGRESO Ingreso { get; set; }
        public YARDA Yarda { get; set; }
        public string ubicacion_estancia = Parametro.UBICACION_ESTANCIA;
        public string ubicacion_transito = Parametro.UBICACION_TRANSITO;
        #endregion

    }
}
