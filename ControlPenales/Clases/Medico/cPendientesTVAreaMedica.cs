using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public class cPendientesTVAreaMedica:ViewModelBase
    {
        private string tipo = "";
        public string TIPO 
        {
            get { return tipo; }
            set { tipo = value; RaisePropertyChanged("TIPO"); }
        }
        private TV_CITA_MEDICA tv_cita_medica = null;
        public TV_CITA_MEDICA TV_CITA_MEDICA 
        {
            get { return tv_cita_medica; }
            set { tv_cita_medica = value; RaisePropertyChanged("TV_CITA_MEDICA"); }
        }
        private TV_CANALIZACION tv_canalizacion = null;
        public TV_CANALIZACION TV_CANALIZACION 
        {
            get { return tv_canalizacion; }
            set { tv_canalizacion = value; RaisePropertyChanged("TV_CANALIZACION"); }
        }
        private TV_INTERCONSULTA_SOLICITUD tv_interconsulta_solicitud = null;
        public TV_INTERCONSULTA_SOLICITUD TV_INTERCONSULTA_SOLICITUD 
        {
            get { return tv_interconsulta_solicitud; }
            set { tv_interconsulta_solicitud = value; RaisePropertyChanged("TV_INTERCONSULTA_SOLICITUD"); }
        }
        private string estatus = string.Empty;
        public string ESTATUS
        {
            get { return estatus; }
            set { estatus = value; RaisePropertyChanged("ESTATUS"); }
        }

        public List<TV_CANALIZACION_ESPECIALIDAD> TV_CANALIZACION_ESPECIALIDAD_FILTRADOS
        {
            get
            {
                return TV_CANALIZACION != null && TV_CANALIZACION.TV_CANALIZACION_ESPECIALIDAD != null && TV_CANALIZACION.TV_CANALIZACION_ESPECIALIDAD.Count() > 0 ?
                    new List<TV_CANALIZACION_ESPECIALIDAD>(TV_CANALIZACION.TV_CANALIZACION_ESPECIALIDAD.Where(w => w.ID_TV_MEDICO_ESTATUS == "PE")) : new List<TV_CANALIZACION_ESPECIALIDAD>();
            }
        }

        public List<TV_CANALIZACION_SERV_AUX> TV_CANALIZACION_SERV_AUX_FILTRADOS
        {
            get
            {
                return TV_CANALIZACION != null && TV_CANALIZACION.TV_CANALIZACION_SERV_AUX != null && TV_CANALIZACION.TV_CANALIZACION_SERV_AUX.Count() > 0 ?
                    new List<TV_CANALIZACION_SERV_AUX>(TV_CANALIZACION.TV_CANALIZACION_SERV_AUX.Where(w => w.ID_TV_MEDICO_ESTATUS == "PE")) : new List<TV_CANALIZACION_SERV_AUX>();
            }
        }

        
        public Visibility TV_CITA_PROC_MED_VISIBLE
        {
            get { return TV_CITA_MEDICA != null && TV_CITA_MEDICA.TV_PROC_ATENCION_MEDICA_PROG != null && TV_CITA_MEDICA.TV_PROC_ATENCION_MEDICA_PROG.Count() > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility TV_CANALIZACION_ESPECIALIDAD_VISIBLE
        {
            get 
            {
                var temp = TV_CANALIZACION_ESPECIALIDAD_FILTRADOS;
                return temp!=null && temp.Count()>0 ? Visibility.Visible : Visibility.Collapsed; 
            }
        }

        public Visibility TV_CANALIZACION_SERV_AUX_VISIBLE
        {
            get
            {
                var temp = TV_CANALIZACION_SERV_AUX_FILTRADOS;
                return temp != null && temp.Count() > 0 ? Visibility.Visible : Visibility.Collapsed; 
            }
        }

        public string ESPECIALIDAD_LABEL
        {
            get { return TV_INTERCONSULTA_SOLICITUD!=null && TV_INTERCONSULTA_SOLICITUD.ID_ESPECIALIDAD.HasValue ? "Especialidad: " + TV_INTERCONSULTA_SOLICITUD.ESPECIALIDAD.DESCR : string.Empty; }
        }

        public Visibility IS_ESPECIALIDAD_VISIBLE
        {
            get { return TV_INTERCONSULTA_SOLICITUD != null && TV_INTERCONSULTA_SOLICITUD.ID_ESPECIALIDAD.HasValue ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility IS_SERV_AUX_INT_VISIBILITY
        {
            get { return TV_INTERCONSULTA_SOLICITUD != null && TV_INTERCONSULTA_SOLICITUD.TV_SERVICIO_AUX_INTERCONSULTA != null && TV_INTERCONSULTA_SOLICITUD.TV_SERVICIO_AUX_INTERCONSULTA.Count() > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility IS_INTERCONSULTA_INTERNA_VISIBILITY
        {
            get { return TV_INTERCONSULTA_SOLICITUD != null && TV_INTERCONSULTA_SOLICITUD.ID_INTER == 1 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility IS_INTERCONSULTA_EXTERNA_VISIBILITY
        {
            get { return TV_INTERCONSULTA_SOLICITUD != null && TV_INTERCONSULTA_SOLICITUD.ID_INTER == 2 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public TV_HOJA_REFERENCIA_MEDICA TV_HOJA_REFERENCIA_MEDICA_REG
        {
            get
            {
                return TV_INTERCONSULTA_SOLICITUD != null && TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA != null && TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.Count() > 0 ?
                    TV_INTERCONSULTA_SOLICITUD.TV_HOJA_REFERENCIA_MEDICA.First() : null;
            }
        }

        public TV_SOL_INTERCONSULTA_INTERNA TV_SOL_INTERCONSULTA_INTERNA_REG
        {
            get
            {
                return TV_INTERCONSULTA_SOLICITUD != null && TV_INTERCONSULTA_SOLICITUD.TV_SOL_INTERCONSULTA_INTERNA != null && TV_INTERCONSULTA_SOLICITUD.TV_SOL_INTERCONSULTA_INTERNA.Count() > 0 ?
                    TV_INTERCONSULTA_SOLICITUD.TV_SOL_INTERCONSULTA_INTERNA.First() : null;
            }
        }
    }
}
