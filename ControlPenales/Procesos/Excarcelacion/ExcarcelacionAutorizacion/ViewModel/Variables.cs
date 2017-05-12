
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using SSP.Servidor.ModelosExtendidos;

namespace ControlPenales
{
    partial class ExcarcelacionAutorizacionViewModel : ValidationViewModelBase
    {
        #region Variables de Validaciones
        private bool isFechaIniValida = true;
        public bool IsFechaIniValida
        {
            get { return isFechaIniValida; }
            set { isFechaIniValida = value; RaisePropertyChanged("IsFechaIniValida"); }
        }
        #endregion
        #region Variables Privadas
        private IQueryable<PROCESO_USUARIO> permisos;
        private string[] estatus_habilitados = null;
        private enum MODO_CANCELACION
        {
            GLOBAL = 1,
            INDIVIDUAL = 2
        }

        private MODO_CANCELACION _modo_cancelacion;
        #endregion

        #region Buscar Excarcelaciones
        private short? anioBuscarExc = null;
        public short? AnioBuscarExc
        {
            get { return anioBuscarExc; }
            set { anioBuscarExc = value; OnPropertyChanged("AnioBuscarExc"); }
        }
        private short? folioBuscarExc;
        public short? FolioBuscarExc
        {
            get { return folioBuscarExc; }
            set { folioBuscarExc = value; OnPropertyChanged("FolioBuscarExc"); }
        }

        private string nombreBuscarExc = string.Empty;
        public string NombreBuscarExc
        {
            get { return nombreBuscarExc; }
            set { nombreBuscarExc = value; OnPropertyChanged("NombreBuscarExc"); }
        }

        private string apellidoPaternoBuscarExc = string.Empty;
        public string ApellidoPaternoBuscarExc
        {
            get { return apellidoPaternoBuscarExc; }
            set { apellidoPaternoBuscarExc = value; OnPropertyChanged("ApellidoPaternoBuscarExc"); }
        }

        private string apellidoMaternoBuscarExc = string.Empty;
        public string ApellidoMaternoBuscarExc
        {
            get { return apellidoMaternoBuscarExc; }
            set { apellidoMaternoBuscarExc = value; OnPropertyChanged("ApellidoMaternoBuscarExc"); }
        }

        private ObservableCollection<EXCARCELACION_TIPO> excarcelacion_TiposBuscar;
        public ObservableCollection<EXCARCELACION_TIPO> Excarcelacion_TiposBuscar
        {
            get { return excarcelacion_TiposBuscar; }
            set { excarcelacion_TiposBuscar = value; RaisePropertyChanged("Excarcelacion_TiposBuscar"); }
        }

        private short selectedExc_TipoBuscarValue;
        public short SelectedExc_TipoBuscarValue
        {
            get { return selectedExc_TipoBuscarValue; }
            set { selectedExc_TipoBuscarValue = value; RaisePropertyChanged("SelectedExc_TipoBuscarValue"); }
        }

        private ObservableCollection<EXCARCELACION_ESTATUS> excarcelacion_EstatusBuscar;
        public ObservableCollection<EXCARCELACION_ESTATUS> Excarcelacion_EstatusBuscar
        {
            get { return excarcelacion_EstatusBuscar; }
            set { excarcelacion_EstatusBuscar = value; RaisePropertyChanged("Excarcelacion_EstatusBuscar"); }
        }

        private string selectedExcarcelacion_EstatusBuscarValue;
        public string SelectedExcarcelacion_EstatusBuscarValue
        {
            get { return selectedExcarcelacion_EstatusBuscarValue; }
            set { selectedExcarcelacion_EstatusBuscarValue = value; RaisePropertyChanged("SelectedExcarcelacion_EstatusBuscarValue"); }
        }

        private DateTime? fechaInicialBuscarExc;
        public DateTime? FechaInicialBuscarExc
        {
            get { return fechaInicialBuscarExc; }
            set { fechaInicialBuscarExc = value; RaisePropertyChanged("FechaInicialBuscarExc"); }
        }

        private DateTime? fechaFinalBuscarExc;
        public DateTime? FechaFinalBuscarExc
        {
            get { return fechaFinalBuscarExc; }
            set { fechaFinalBuscarExc = value; RaisePropertyChanged("FechaFinalBuscarExc"); }
        }
        #endregion

        #region Variables para Habilitar/Deshabilitar Controles
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        #endregion

        #region Variables para visualizar Controles
        private Visibility isExcarcelacionDestinosVisible = Visibility.Collapsed;
        public Visibility IsExcarcelacionDestinosVisible
        {
            get { return isExcarcelacionDestinosVisible; }
            set { isExcarcelacionDestinosVisible = value; OnPropertyChanged("IsExcarcelacionDestinosVisible"); }
        }
        #endregion

        #region Generales
        private ObservableCollection<EXCARCELACION> listaExcarcelaciones;
        public ObservableCollection<EXCARCELACION> ListaExcarcelaciones
        {
            get { return listaExcarcelaciones; }
            set { listaExcarcelaciones = value; RaisePropertyChanged("ListaExcarcelaciones"); }
        }

        private EXCARCELACION selectedExcarcelacion;
        public EXCARCELACION SelectedExcarcelacion
        {
            get { return selectedExcarcelacion; }
            set { selectedExcarcelacion = value; RaisePropertyChanged("SelectedExcarcelacion"); }
        }

        private ObservableCollection<CT_EXCARCELACION_DESTINO> listaExcarcelacionDestinos;
        public ObservableCollection<CT_EXCARCELACION_DESTINO> ListaExcarcelacionDestinos
        {
            get { return listaExcarcelacionDestinos; }
            set { listaExcarcelacionDestinos = value; OnPropertyChanged("ListaExcarcelacionDestinos"); }
        }

        private CT_EXCARCELACION_DESTINO selectedExcarcelacionDestino;
        public CT_EXCARCELACION_DESTINO SelectedExcarcelacionDestino
        {
            get { return selectedExcarcelacionDestino; }
            set { selectedExcarcelacionDestino = value; OnPropertyChanged("SelectedExcarcelacionDestino"); }
        }

        private string headerDestinosExcarcelacion;
        public string HeaderDestinosExcarcelacion
        {
            get { return headerDestinosExcarcelacion; }
            set { headerDestinosExcarcelacion = value; OnPropertyChanged("HeaderDestinosExcarcelacion"); }
        }
        #endregion

        #region Motivo de Cancelacion
        private ObservableCollection<EXCARCELACION_CANCELA_MOTIVO> cancelacion_Motivos;
        public ObservableCollection<EXCARCELACION_CANCELA_MOTIVO> Cancelacion_Motivos
        {
            get { return cancelacion_Motivos; }
            set { cancelacion_Motivos = value; OnPropertyChanged("Cancelacion_Motivos"); }
        }

        private short selectedCancelacion_MotivoValue;
        public short SelectedCancelacion_MotivoValue
        {
            get { return selectedCancelacion_MotivoValue; }
            set { selectedCancelacion_MotivoValue = value; OnPropertyChanged("SelectedCancelacion_MotivoValue"); }
        }

        private string cancelacion_Observacion = string.Empty;
        public string Cancelacion_Observacion
        {
            get { return cancelacion_Observacion; }
            set { cancelacion_Observacion = value; OnPropertyChanged("Cancelacion_Observacion"); }
        }
        #endregion

        DateTime _FechaServer = Fechas.GetFechaDateServer;
        public DateTime FechaServer
        {
            get { return _FechaServer; }
            set
            {
                _FechaServer = value;
                OnPropertyChanged("FechaServer");
            }
        }
    }

}
