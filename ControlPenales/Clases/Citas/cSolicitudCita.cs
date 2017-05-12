using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using System.Windows;
namespace ControlPenales
{
    public class cSolicitudCita
    {
        public ATENCION_INGRESO ATENCION_INGRESO { get; set; }
        private short max_atenciones;
        public short MAX_ATENCIONES
        {
            get { return max_atenciones; }
            set { max_atenciones = value; }
        }
        private int cant_solicitudes_atendidas;
        public int CANT_SOLICITUDES_ATENDIDAS 
        {
            get { return cant_solicitudes_atendidas; }
            set { cant_solicitudes_atendidas = value; }
        }

        public bool ALCANZO_TOPE_ATENCIONES
        {
            get
            {
                if (cant_solicitudes_atendidas >= max_atenciones)
                    return true;
                else
                    return false;
            }
        }

        public IEnumerable<ATENCION_CITA> SOLICITUDES_ATENDIDAS
        {
            get
            {
                if (ATENCION_INGRESO.INGRESO.ATENCION_CITA.Where(w => w.ATENCION_SOLICITUD != null && w.ATENCION_SOLICITUD.ID_TECNICA == ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TECNICA && w.ATENCION_SOLICITUD.ID_CENTRO == ATENCION_INGRESO.ATENCION_SOLICITUD.ID_CENTRO
                                           && w.CITA_FECHA_HORA.Value.Month == ATENCION_INGRESO.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.Month && w.CITA_FECHA_HORA.Value.Year == ATENCION_INGRESO.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.Year).Any())
                    return ATENCION_INGRESO.INGRESO.ATENCION_CITA.Where(w => w.ATENCION_SOLICITUD != null && w.ATENCION_SOLICITUD.ID_TECNICA == ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TECNICA && w.ATENCION_SOLICITUD.ID_CENTRO == ATENCION_INGRESO.ID_CENTRO_UBI
                                           && w.CITA_FECHA_HORA.Value.Month == ATENCION_INGRESO.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.Month && w.CITA_FECHA_HORA.Value.Year == ATENCION_INGRESO.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.Year);
                else
                    return null;
            }
        }

        public string TITULO_SOL_ATENDIDAS
        {
            get { return "SOLICITUDES AGENDADAS: " + cant_solicitudes_atendidas; }
        }

        public Visibility IsGrisSolicitudesVisible
        {
            get
            {
                if (cant_solicitudes_atendidas > 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
    }
}
