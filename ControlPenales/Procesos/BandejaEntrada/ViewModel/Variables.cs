using ControlPenales;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class BandejaEntradaViewModel 
    {
        #region Bandeja
        private List<cNotificacion> notificaciones;
        public List<cNotificacion> Notificaciones
        {
            get { return notificaciones; }
            set { notificaciones = value; OnPropertyChanged("Notificaciones"); }
        }

        private string noNotificaciones;
        public string NoNotificaciones
        {
            get { return noNotificaciones; }
            set { noNotificaciones = value; OnPropertyChanged("NoNotificaciones"); }
        }

        private ObservableCollection<Mensajes> lstMensaje;
        public ObservableCollection<Mensajes> LstMensaje
        {
            get { return lstMensaje; }
            set { 
                lstMensaje = value; 
                OnPropertyChanged("LstMensaje"); }
        }

        private Mensajes selectedMensaje;
        public Mensajes SelectedMensaje
        {
            get { return selectedMensaje; }
            set { selectedMensaje = value;
                if (value != null)
                {
                    if (value.UsuarioMensaje != null)
                    {
                        if (value.UsuarioMensaje.MENSAJE != null)
                        {
                            if (value.UsuarioMensaje.MENSAJE.ID_INTER_DOCTO != null)
                                DocumentoVisible = true;
                            else
                                DocumentoVisible = false;
                        }
                    }
                }
                OnPropertyChanged("SelectedMensaje"); }
        }

        private bool popAbierto = false;
        public bool PopAbierto
        {
            get { return popAbierto; }
            set { popAbierto = value; OnPropertyChanged("PopAbierto"); }
        }

        private int VNoDias = Parametro.DIAS_BANDEJA;
        #endregion

        #region Ordenar
        private short selectedVer = 1;
        public short SelectedVer
        {
            get { return selectedVer; }
            set { selectedVer = value;
                PopulateListado();
                OnPropertyChanged("SelectedVer"); }
        }

        private short selectedOrganizar = 1;
        public short SelectedOrganizar
        {
            get { return selectedOrganizar; }
            set { selectedOrganizar = value;
                PopulateListado();
                OnPropertyChanged("SelectedOrganizar"); }
        }
        #endregion

        #region Ver Documento
        private bool documentoVisible = false;
        public bool DocumentoVisible
        {
            get { return documentoVisible; }
            set { documentoVisible = value; OnPropertyChanged("DocumentoVisible"); }
        }
        #endregion

        #region Filtros
        private string buscar;
        public string Buscar
        {
            get { return buscar; }
            set { buscar = value; OnPropertyChanged("Buscar"); }
        }

        private DateTime? fechaInicio;
        public DateTime? FechaInicio
        {
            get { return fechaInicio; }
            set { fechaInicio = value; OnPropertyChanged("FechaInicio"); }
        }

        private DateTime? fechaFin;
        public DateTime? FechaFin
        {
            get { return fechaFin; }
            set { fechaFin = value; OnPropertyChanged("FechaFin"); }
        }
        #endregion

        #region Pantalla
        private Visibility mResultados = Visibility.Collapsed;
        public Visibility MResultados
        {
            get { return mResultados; }
            set { mResultados = value; OnPropertyChanged("MResultados"); }
        }
        #endregion

        #region Tiempo
        private System.Timers.Timer aTimer;
        #endregion
    }
}
