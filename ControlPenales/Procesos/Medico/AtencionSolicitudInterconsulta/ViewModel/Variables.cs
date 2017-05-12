using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class AtencionSolicitudInterconsultaViewModel
    {
        #region Variables privadas
        private int otro_hospital = Parametro.ID_HOSPITAL_OTROS;
        private bool isfechacitarequerida = false;
        private short?[] estatus_administrativos_inactivos = null;
        private INTERCONSULTA_SOLICITUD selectedInterconsulta_solicitud;
        private enumModo modoVistaModelo = enumModo.INSERCION;
        private IQueryable<PROCESO_USUARIO> permisos;
        private bool _permisos_editar = false;
        private bool _permisos_agregar = false;
        private enumTipoServicioInterconsulta tipoServicioInterconsultaSeleccionado;
        private short? id_atencion_tipo = null;
        #endregion
        #region Variables para habilitar controles
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; RaisePropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuAgregarEnabled = false;
        public bool MenuAgregarEnabled
        {
            get { return menuAgregarEnabled; }
            set { menuAgregarEnabled = value; RaisePropertyChanged("MenuAgregarEnabled"); }
        }
        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; RaisePropertyChanged("MenuBuscarEnabled"); }
        }
        private bool menuEditarEnabled = false;
        public bool MenuEditarEnabled
        {
            get { return menuEditarEnabled; }
            set { menuEditarEnabled = value; RaisePropertyChanged("MenuEditarEnabled"); }
        }

        private bool eliminarMenuEnabled = false;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; RaisePropertyChanged("EliminarMenuEnabled"); }
        }
        private bool menuLimpiarEnabled;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set { menuLimpiarEnabled = value; RaisePropertyChanged("MenuLimpiarEnabled"); }
        }

        private bool gridsSolicitudesEnabled = true;
        public bool GridsSolicitudesEnabled
        {
            get { return gridsSolicitudesEnabled; }
            set { gridsSolicitudesEnabled = value; RaisePropertyChanged("GridsSolicitudesEnabled"); }
        }
        #endregion
        #region Datos de Solicitud

        private ObservableCollection<EXT_ESPECIALIDAD> lstExtEspecialidad;
        public ObservableCollection<EXT_ESPECIALIDAD> LstExtEspecialidad
        {
            get { return lstExtEspecialidad; }
            set { lstExtEspecialidad = value; RaisePropertyChanged("LstExtEspecialidad"); }
        }

        private EXT_ESPECIALIDAD selectedExtEspecialidad = null;
        public EXT_ESPECIALIDAD SelectedExtEspecialidad
        {
            get { return selectedExtEspecialidad; }
            set
            {
                selectedExtEspecialidad = value;
                RaisePropertyChanged("SelectedExtEspecialidad");
                if (value != null)
                    SelectedServAuxCanalizacion = null;
                
            }
        }


        private List<INTERCONSULTA_TIPO> lstExtEspecialidadInterconsulta_Tipo;

        private List<INTERCONSULTA_NIVEL_PRIORIDAD> lstExtEspecialidadNvlPrioridades;

        private bool isInterconsultaEnabled = false;
        public bool IsInterconsultaEnabled
        {
            get { return isInterconsultaEnabled; }
            set { isInterconsultaEnabled = value; RaisePropertyChanged("IsInterconsultaEnabled"); }
        }

        private ObservableCollection<INTERCONSULTA_TIPO> lstInterconsultaTipo;

        public ObservableCollection<INTERCONSULTA_TIPO> LstInterconsultaTipo
        {
            get { return lstInterconsultaTipo; }
            set { lstInterconsultaTipo = value; RaisePropertyChanged("LstInterconsultaTipo"); }
        }

        private short selectedInterconsultaTipo = -1;
        public short SelectedInterconsultaTipo
        {
            get { return selectedInterconsultaTipo; }
            set { selectedInterconsultaTipo = value; OnPropertyValidateChanged("SelectedInterconsultaTipo"); }
        }

        private ObservableCollection<INTERCONSULTA_NIVEL_PRIORIDAD> lstNvlPrioridades;
        public ObservableCollection<INTERCONSULTA_NIVEL_PRIORIDAD> LstNvlPrioridades
        {
            get { return lstNvlPrioridades; }
            set { lstNvlPrioridades = value; RaisePropertyChanged("LstNvlPrioridades"); }
        }

        private short selectedNvlPrioridad = -1;
        public short SelectedNvlPrioridad
        {
            get { return selectedNvlPrioridad; }
            set { selectedNvlPrioridad = value; OnPropertyValidateChanged("SelectedNvlPrioridad"); }
        }

        private ObservableCollection<INTERCONSULTA_ATENCION_TIPO> lstInterconsultaAtencionTipo;
        public ObservableCollection<INTERCONSULTA_ATENCION_TIPO> LstInterconsultaAtencionTipo
        {
            get { return lstInterconsultaAtencionTipo; }
            set { lstInterconsultaAtencionTipo = value; RaisePropertyChanged("LstInterconsultaTipo"); }
        }

        private short selectedInterconsultaAtencionTipo;
        public short SelectedInterconsultaAtencionTipo
        {
            get { return selectedInterconsultaAtencionTipo; }
            set { selectedInterconsultaAtencionTipo = value; OnPropertyValidateChanged("SelectedInterconsultaAtencionTipo"); }
        }

        private ObservableCollection<ESPECIALIDAD> lstEspecialidad;
        public ObservableCollection<ESPECIALIDAD> LstEspecialidad
        {
            get { return lstEspecialidad; }
            set { lstEspecialidad = value; RaisePropertyChanged("LstEspecialidad"); }
        }

        private short selectedEspecialidad = -1;
        public short SelectedEspecialidad
        {
            get { return selectedEspecialidad; }
            set { selectedEspecialidad = value; OnPropertyValidateChanged("SelectedEspecialidad"); }
        }

        private ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT> lstTipoServAux;
        public ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT> LstTipoServAux
        {
            get { return lstTipoServAux; }
            set { lstTipoServAux = value; RaisePropertyChanged("LstTipoServAux"); }
        }

        private short selectedTipoServAux = -1;
        public short SelectedTipoServAux
        {
            get { return selectedTipoServAux; }
            set { selectedTipoServAux = value; OnPropertyValidateChanged("SelectedTipoServAux"); }
        }

        private ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT> lstSubtipoServAux;
        public ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT> LstSubtipoServAux
        {
            get { return lstSubtipoServAux; }
            set { lstSubtipoServAux = value; RaisePropertyChanged("LstSubtipoServAux"); }
        }

        private short selectedSubtipoServAux = -1;
        public short SelectedSubtipoServAux
        {
            get { return selectedSubtipoServAux; }
            set { selectedSubtipoServAux = value; OnPropertyValidateChanged("SelectedSubtipoServAux"); }
        }

        private ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> lstServAux;
        public ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> LstServAux
        {
            get { return lstServAux; }
            set { lstServAux = value; RaisePropertyChanged("LstServAux"); }
        }


        private ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> lstServAuxSeleccionados;
        public ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> LstServAuxSeleccionados
        {
            get { return lstServAuxSeleccionados; }
            set { lstServAuxSeleccionados = value; OnPropertyValidateChanged("LstServAuxSeleccionados"); }
        }

        private EXT_SERV_AUX_DIAGNOSTICO selectedServAuxCanalizacion = null;
        public EXT_SERV_AUX_DIAGNOSTICO SelectedServAuxCanalizacion
        {
            get { return selectedServAuxCanalizacion; }
            set 
            { 
                selectedServAuxCanalizacion = value;
                RaisePropertyChanged("SelectedServAuxCanalizacion");
                if (value != null)
                    SelectedExtEspecialidad = null;
            }
        }

        private bool isServAuxSeleccionadosValid = false;
        public bool IsServAuxSeleccionadosValid
        {
            get { return isServAuxSeleccionadosValid; }
            set { isServAuxSeleccionadosValid = value; RaisePropertyChanged("IsServAuxSeleccionadosValid"); }
        }

        private ObservableCollection<EXT_INTERCONSULTA_SOLICITUD> lstCanalizacionInterconsultas = null;
        public ObservableCollection<EXT_INTERCONSULTA_SOLICITUD> LstCanalizacionInterconsultas
        {
            get { return lstCanalizacionInterconsultas; }
            set { lstCanalizacionInterconsultas = value; RaisePropertyChanged("LstCanalizacionInterconsultas"); }
        }

        private EXT_INTERCONSULTA_SOLICITUD selectedCanalizacionInterconsulta = null;
        public EXT_INTERCONSULTA_SOLICITUD SelectedCanalizacionInterconsulta
        { 
            get { return selectedCanalizacionInterconsulta; }
            set { selectedCanalizacionInterconsulta = value; RaisePropertyChanged("SelectedCanalizacionInterconsulta"); }
        }

        #region Datos de hoja de referencia médica
        private ObservableCollection<HOSPITAL> lstHospitales;
        public ObservableCollection<HOSPITAL> LstHospitales
        {
            get { return lstHospitales; }
            set { lstHospitales = value; RaisePropertyChanged("LstHospitales"); }
        }

        private short selectedRefMedHospital;
        public short SelectedRefMedHospital
        {
            get { return selectedRefMedHospital; }
            set { selectedRefMedHospital = value; OnPropertyValidateChanged("SelectedRefMedHospital"); }
        }

        private string textRefMedExpHGT = string.Empty;
        public string TextRefMedExpHGT
        {
            get { return textRefMedExpHGT; }
            set { textRefMedExpHGT = value; OnPropertyValidateChanged("TextRefMedExpHGT"); }
        }

        private ObservableCollection<CITA_TIPO> lstTipoCitas;
        public ObservableCollection<CITA_TIPO> LstTipoCitas
        {
            get { return lstTipoCitas; }
            set { lstTipoCitas = value; RaisePropertyChanged("LstTipoCitas"); }
        }

        private short selectedRefMedTipoCitaValue = -1;
        public short SelectedRefMedTipoCitaValue
        {
            get { return selectedRefMedTipoCitaValue; }
            set { selectedRefMedTipoCitaValue = value; OnPropertyValidateChanged("SelectedRefMedTipoCitaValue"); }

        }

        private string textRefMedMotivo = string.Empty;
        public string TextRefMedMotivo
        {
            get { return textRefMedMotivo; }
            set { textRefMedMotivo = value; OnPropertyValidateChanged("TextRefMedMotivo"); }
        }

        private string textRefMedObservaciones = string.Empty;
        public string TextRefMedObservaciones
        {
            get { return textRefMedObservaciones; }
            set { textRefMedObservaciones = value; OnPropertyValidateChanged("TextRefMedObservaciones"); }
        }

        private string textRefMedOtroHospital = string.Empty;
        public string TextRefMedOtroHospital
        {
            get { return textRefMedOtroHospital; }
            set { textRefMedOtroHospital = value; OnPropertyValidateChanged("TextRefMedOtroHospital"); }
        }

        private DateTime? fechaCita = null;
        public DateTime? FechaCita
        {
            get { return fechaCita; }
            set
            {
                fechaCita = value;
                OnPropertyValidateChanged("FechaCita");
                if (!isfechacitarequerida)
                    IsFechaCitaValid = true;
                else if (isfechacitarequerida && !value.HasValue)
                    IsFechaCitaValid = false;
                else
                    IsFechaCitaValid = true;
            }
        }

        private bool isFechaCitaValid = false;
        public bool IsFechaCitaValid
        {
            get { return isFechaCitaValid; }
            set { isFechaCitaValid = value; RaisePropertyChanged("IsFechaCitaValid"); }
        }

        private ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> lstSerAuxHojaRef = null;
        public ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> LstSerAuxHojaRef
        {
            get { return lstSerAuxHojaRef; }
            set { lstSerAuxHojaRef = value; RaisePropertyChanged("LstSerAuxHojaRef"); }
        }

        private string hRefEspecialidad = string.Empty;
        public string HRefEspecialidad
        {
            get { return hRefEspecialidad; }
            set { hRefEspecialidad = value; RaisePropertyChanged("HRefEspecialidad"); }
        }
        #endregion

        #region Datos de interconsulta interna
        private ObservableCollection<CENTRO> lstCentros;
        public ObservableCollection<CENTRO> LstCentros
        {
            get { return lstCentros; }
            set { lstCentros = value; OnPropertyChanged("LstCentros"); }
        }

        private short selectedSolIntCentro = -1;
        public short SelectedSolIntCentro
        {
            get { return selectedSolIntCentro; }
            set { selectedSolIntCentro = value; OnPropertyValidateChanged("SelectedSolIntCentro"); }
        }

        private string textSolIntMotivo = string.Empty;
        public string TextSolIntMotivo
        {
            get { return textSolIntMotivo; }
            set { textSolIntMotivo = value; OnPropertyValidateChanged("TextSolIntMotivo"); }
        }
        #endregion

        #region Busqueda de canalizaciones
        private short? anioBuscarCanal;
        public short? AnioBuscarCanal
        {
            get { return anioBuscarCanal; }
            set { anioBuscarCanal = value; RaisePropertyChanged("AnioBuscarCanal"); }
        }

        private int? folioBuscarCanal;
        public int? FolioBuscarCanal
        {
            get { return folioBuscarCanal; }
            set { folioBuscarCanal = value; RaisePropertyChanged("FolioBuscarCanal"); }
        }

        private string nombreBuscarCanal = string.Empty;
        public string NombreBuscarCanal
        {
            get { return nombreBuscarCanal; }
            set { nombreBuscarCanal = value; RaisePropertyChanged("NombreBuscarCanal"); }
        }

        private string apellidoPaternoBuscarCanal;
        public string ApellidoPaternoBuscarCanal
        {
            get { return apellidoPaternoBuscarCanal; }
            set { apellidoPaternoBuscarCanal = value; RaisePropertyChanged("ApellidoPaternoBuscarCanal"); }
        }

        private string apellidoMaternoBuscarCanal = string.Empty;
        public string ApellidoMaternoBuscarCanal
        {
            get { return apellidoMaternoBuscarCanal; }
            set { apellidoMaternoBuscarCanal = value; RaisePropertyChanged("ApellidoMaternoBuscarCanal"); }
        }

        private DateTime? fechaInicialBuscarCanal = null;
        public DateTime? FechaInicialBuscarCanal
        {
            get { return fechaInicialBuscarCanal; }
            set { fechaInicialBuscarCanal = value; RaisePropertyChanged("FechaInicialBuscarCanal"); }
        }

        private DateTime? fechaFinalBuscarCanal = null;
        public DateTime? FechaFinalBuscarCanal
        {
            get { return fechaFinalBuscarCanal; }
            set { fechaFinalBuscarCanal = value; RaisePropertyChanged("FechaFinalBuscarCanal"); }
        }


        private ObservableCollection<CANALIZACION> lstCanalizacionesBusqueda;
        public ObservableCollection<CANALIZACION> LstCanalizacionesBusqueda
        {
            get { return lstCanalizacionesBusqueda; }
            set { lstCanalizacionesBusqueda = value; RaisePropertyChanged("LstCanalizacionesBusqueda"); }
        }

        private CANALIZACION selectedCanalizacionBusqueda;
        public CANALIZACION SelectedCanalizacionBusqueda
        {
            get { return selectedCanalizacionBusqueda; }
            set { selectedCanalizacionBusqueda = value; RaisePropertyChanged("SelectedCanalizacionBusqueda"); }
        }

        private ObservableCollection<ATENCION_TIPO> lstAtencion_TipoCanalBuscar = null;
        public ObservableCollection<ATENCION_TIPO> LstAtencion_TipoCanalBuscar
        {
            get { return lstAtencion_TipoCanalBuscar; }
            set { lstAtencion_TipoCanalBuscar = value; RaisePropertyChanged("LstAtencion_TipoCanalBuscar"); }
        }

        private short selectedAtencion_TipoCanalBuscarValue = -1;
        public short SelectedAtencion_TipoCanalBuscarValue
        {
            get { return selectedAtencion_TipoCanalBuscarValue; }
            set { selectedAtencion_TipoCanalBuscarValue = value; RaisePropertyChanged("SelectedAtencion_TipoCanalBuscarValue"); }
        }

        private string textTituloBuscarCanalizacion = "Buscar canalizaciones pendientes de interconsulta";

        public string TextTituloBuscarCanalizacion
        {
            get { return textTituloBuscarCanalizacion; }
        }

        private CANALIZACION selectedCanalizacion;

        //private ObservableCollection<ATENCION_TIPO> lstAtencion_Tipo = null;
        //public ObservableCollection<ATENCION_TIPO> LstAtencion_Tipo
        //{
        //    get { return lstAtencion_Tipo; }
        //    set { lstAtencion_Tipo = value; RaisePropertyChanged("LstAtencion_Tipo"); }
        //}

        //private short selectedAtencion_TipoValue = -1;
        //public short SelectedAtencion_TipoValue
        //{
        //    get { return selectedAtencion_TipoValue; }
        //    set { selectedAtencion_TipoValue = value; RaisePropertyChanged("SelectedAtencion_TipoValue"); }
        //}

        #region Validaciones
        private bool isFechaIniBusquedaCanalValida = true;
        public bool IsFechaIniBusquedaCanalValida
        {
            get { return isFechaIniBusquedaCanalValida; }
            set { isFechaIniBusquedaCanalValida = value; RaisePropertyChanged("IsFechaIniBusquedaCanalValida"); }
        }

        //private bool isFechaIniBusquedaValida = true;
        //public bool IsFechaIniBusquedaValida
        //{
        //    get { return isFechaIniBusquedaValida; }
        //    set { isFechaIniBusquedaValida = value; RaisePropertyChanged("IsFechaIniBusquedaValida"); }
        //}
        #endregion


        #endregion

        #region Datos Imputado
        private string textAnioImputado = string.Empty;
        public string TextAnioImputado
        {
            get { return textAnioImputado; }
            set { textAnioImputado = value; RaisePropertyChanged("TextAnioImputado"); }
        }

        private string textFolioImputado = string.Empty;
        public string TextFolioImputado
        {
            get { return textFolioImputado; }
            set { textFolioImputado = value; RaisePropertyChanged("TextFolioImputado"); }
        }

        private string textPaternoImputado = string.Empty;
        public string TextPaternoImputado
        {
            get { return textPaternoImputado; }
            set { textPaternoImputado = value; RaisePropertyChanged("TextPaternoImputado"); }
        }

        private string textMaternoImputado = string.Empty;
        public string TextMaternoImputado
        {
            get { return textMaternoImputado; }
            set { textMaternoImputado = value; RaisePropertyChanged("TextMaternoImputado"); }
        }

        private string textNombreImputado = string.Empty;
        public string TextNombreImputado
        {
            get { return textNombreImputado; }
            set { textNombreImputado = value; RaisePropertyChanged("TextNombreImputado"); }
        }

        private string textSexoImputado = string.Empty;
        public string TextSexoImputado
        {
            get { return textSexoImputado; }
            set { textSexoImputado = value; RaisePropertyChanged("TextSexoImputado"); }
        }

        private string textEdadImputado = string.Empty;
        public string TextEdadImputado
        {
            get { return textEdadImputado; }
            set { textEdadImputado = value; RaisePropertyChanged("TextEdadImputado"); }
        }

        private string textFechaNacImputado;
        public string TextFechaNacImputado
        {
            get { return textFechaNacImputado; }
            set { textFechaNacImputado = value; RaisePropertyChanged("TextFechaNacImputado"); }
        }


        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set
            {
                imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
            }
        }
        #endregion


        #region busqueda de hojas de referencia medica
        private ObservableCollection<HOJA_REFERENCIA_MEDICA> lstHojaReferenciaMedicaMem;
        public ObservableCollection<HOJA_REFERENCIA_MEDICA> LstHojaReferenciaMedicaMem
        {
            get { return lstHojaReferenciaMedicaMem; }
            set { lstHojaReferenciaMedicaMem = value; RaisePropertyChanged("LstHojaReferenciaMedicaMem"); }
        }

        private HOJA_REFERENCIA_MEDICA selectedHojaReferenciaMedicaMemBuscar;
        public HOJA_REFERENCIA_MEDICA SelectedHojaReferenciaMedicaMemBuscar
        {
            get { return selectedHojaReferenciaMedicaMemBuscar; }
            set { selectedHojaReferenciaMedicaMemBuscar = value; RaisePropertyChanged("SelectedHojaReferenciaMedicaMemBuscar"); }
        }

        private HOJA_REFERENCIA_MEDICA selectedHojaReferenciaMedicaMem;
        #endregion
        #endregion
        #region Visibilidad de Controles
        private Visibility isEspecialidadVisible = Visibility.Collapsed;
        public Visibility IsEspecialidadVisible
        {
            get { return isEspecialidadVisible; }
            set { isEspecialidadVisible = value; RaisePropertyChanged("IsEspecialidadVisible"); }
        }

        private Visibility isServicioAuxiliarHojaRefVisible = Visibility.Collapsed;
        public Visibility IsServicioAuxiliarHojaRefVisible
        {
            get { return isServicioAuxiliarHojaRefVisible; }
            set { isServicioAuxiliarHojaRefVisible = value; RaisePropertyChanged("IsServicioAuxiliarHojaRefVisible"); }
        }

        private Visibility isReferenciaVisible = Visibility.Collapsed;
        public Visibility IsReferenciaVisible
        {
            get { return isReferenciaVisible; }
            set { isReferenciaVisible = value; RaisePropertyChanged("IsReferenciaVisible"); }
        }

        private Visibility isSolicitudInternaVisible = Visibility.Collapsed;

        public Visibility IsSolicitudInternaVisible
        {
            get { return isSolicitudInternaVisible; }
            set { isSolicitudInternaVisible = value; RaisePropertyChanged("IsSolicitudInternaVisible"); }
        }

        private Visibility isCertificadoMedico = Visibility.Collapsed;
        public Visibility IsCertificadoMedico
        {
            get { return isCertificadoMedico; }
            set { isCertificadoMedico = value; RaisePropertyChanged("IsCertificadoMedico"); }
        }

        private Visibility isOtroHospitalSelected = Visibility.Collapsed;
        public Visibility IsOtroHospitalSelected
        {
            get { return isOtroHospitalSelected; }
            set { isOtroHospitalSelected = value; RaisePropertyChanged("IsOtroHospitalSelected"); }
        }

        public Visibility exploracionFisicaVisible = Visibility.Collapsed;
        public Visibility ExploracionFisicaVisible
        {
            get { return exploracionFisicaVisible; }
            set { exploracionFisicaVisible = value; RaisePropertyChanged("ExploracionFisicaVisible"); }
        }

        private Visibility isEspecialidadHojaRefVisible = Visibility.Collapsed;
        public Visibility IsEspecialidadHojaRefVisible
        {
            get { return isEspecialidadHojaRefVisible; }
            set { isEspecialidadHojaRefVisible = value; RaisePropertyChanged("IsEspecialidadHojaRefVisible"); }
        }

        private Visibility isCanalizacionVisible = Visibility.Collapsed;
        public Visibility IsCanalizacionVisible
        {
            get { return isCanalizacionVisible; }
            set { isCanalizacionVisible = value; RaisePropertyChanged("IsCanalizacionVisible"); }
        }

        #endregion

        #region Tab Control
        private short selectedTabIndex = 0;

        public short SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set { selectedTabIndex = value; RaisePropertyChanged("SelectedTabIndex"); }
        }

        private Visibility selectedTabIndexTipografia = 0;
        public Visibility SelectedTabIndexTipografia
        {
            get { return selectedTabIndexTipografia; }
            set { selectedTabIndexTipografia = value; RaisePropertyChanged("private Visibility SelectedTabIndexTipografia"); }
        }
        #endregion
        #region Nota Medica
        #region Signos Vitales
        private string textNMPeso = string.Empty;
        public string TextNMPeso
        {
            get { return textNMPeso; }
            set { textNMPeso = value; RaisePropertyChanged("TextNMPeso"); }
        }

        private string textNMTalla = string.Empty;
        public string TextNMTalla
        {
            get { return textNMTalla; }
            set { textNMTalla = value; RaisePropertyChanged("TextNMTalla"); }
        }

        private string textNMTensionArterial1 = string.Empty;
        public string TextNMTensionArterial1
        {
            get { return textNMTensionArterial1; }
            set { textNMTensionArterial1 = value; RaisePropertyChanged("TextNMTensionArterial1"); }
        }

        private string textNMTensionArterial2 = string.Empty;
        public string TextNMTensionArterial2
        {
            get { return textNMTensionArterial2; }
            set { textNMTensionArterial2 = value; RaisePropertyChanged("TextNMTensionArterial2"); }
        }

        private string textNMFrecuenciaCardiaca = string.Empty;
        public string TextNMFrecuenciaCardiaca
        {
            get { return textNMFrecuenciaCardiaca; }
            set { textNMFrecuenciaCardiaca = value; RaisePropertyChanged("TextNMFrecuenciaCardiaca"); }
        }

        private string textNMFrecuenciaRespira = string.Empty;
        public string TextNMFrecuenciaRespira
        {
            get { return textNMFrecuenciaRespira; }
            set { textNMFrecuenciaRespira = value; RaisePropertyChanged("TextNMFrecuenciaRespira"); }
        }

        private string textNMTemperatura = string.Empty;
        public string TextNMTemperatura
        {
            get { return textNMTemperatura; }
            set { textNMTemperatura = value; RaisePropertyChanged("TextNMTemperatura"); }
        }

        private string textNMObservacionesSignosVitales = string.Empty;
        public string TextNMObservacionesSignosVitales
        {
            get { return textNMObservacionesSignosVitales; }
            set { textNMObservacionesSignosVitales = value; RaisePropertyChanged("TextNMObservacionesSignosVitales"); }
        }
        #endregion
        #region Topografia
        private List<CheckBox> _ListCheckBoxDorso;
        public List<CheckBox> ListCheckBoxDorso
        {
            get { return _ListCheckBoxDorso; }
            set { _ListCheckBoxDorso = value; }
        }
        private List<CheckBox> _CheckBoxFrente;
        public List<CheckBox> CheckBoxFrente
        {
            get { return _CheckBoxFrente; }
            set { _CheckBoxFrente = value; }
        }

        private ObservableCollection<LESION> lstLesiones = null;
        public ObservableCollection<LESION> LstLesiones
        {
            get { return lstLesiones; }
            set { lstLesiones = value; RaisePropertyChanged("LstLesiones"); }
        }

        #endregion

        #region Diagnostico
        private string text_Pronostico_Descr = string.Empty;
        public string Text_Pronostico_Descr
        {
            get { return text_Pronostico_Descr; }
            set { text_Pronostico_Descr = value; RaisePropertyChanged("Text_Pronostico_Descr"); }
        }

        private string exploracionFisica = string.Empty;
        public string ExploracionFisica
        {
            get { return exploracionFisica; }
            set { exploracionFisica = value; RaisePropertyChanged("ExploracionFisica"); }
        }

        private ObservableCollection<NOTA_MEDICA_ENFERMEDAD> lstEnfermedades = null;
        public ObservableCollection<NOTA_MEDICA_ENFERMEDAD> LstEnfermedades
        {
            get { return lstEnfermedades; }
            set { lstEnfermedades = value; RaisePropertyChanged("LstEnfermedades"); }
        }

        private bool checkedHospitalizacion = false;
        public bool CheckedHospitalizacion
        {
            get { return checkedHospitalizacion; }
            set { checkedHospitalizacion = value; RaisePropertyChanged("CheckedHospitalizacion"); }
        }

        private bool checkedPeligroVida = false;
        public bool CheckedPeligroVida
        {
            get { return checkedPeligroVida; }
            set { checkedPeligroVida = value; RaisePropertyChanged("CheckedPeligroVida"); }
        }

        private bool checked15DiasSanar = false;
        public bool Checked15DiasSanar
        {
            get { return checked15DiasSanar; }
            set { checked15DiasSanar = value; RaisePropertyChanged("Checked15DiasSanar"); }
        }
        #endregion

        #region Tratamiento
        private ObservableCollection<RecetaMedica> receta_Medica = null;
        public ObservableCollection<RecetaMedica> Receta_Medica
        {
            get { return receta_Medica; }
            set { receta_Medica = value; RaisePropertyChanged("Receta_Medica"); }
        }
        private bool elementosDisponibles = false;
        public bool ElementosDisponibles
        {
            get { return elementosDisponibles; }
        }

        private ObservableCollection<DietaMedica> lstDietas = null;
        public ObservableCollection<DietaMedica> LstDietas
        {
            get { return lstDietas; }
            set { lstDietas = value; RaisePropertyChanged("LstDietas"); }
        }

        private string textObservacionesConclusionesCertificado = string.Empty;
        public string TextObservacionesConclusionesCertificado
        {
            get { return textObservacionesConclusionesCertificado; }
            set { textObservacionesConclusionesCertificado = value; RaisePropertyChanged("TextObservacionesConclusionesCertificado"); }
        }
        #endregion
        #endregion

        private DateTime fechaServer = Fechas.GetFechaDateServer;
        public DateTime FechaServer
        {
            get { return fechaServer; }
            set { fechaServer = value; RaisePropertyChanged("FechaServer"); }
        }

        private DateTime? fechaMinima = Fechas.GetFechaDateServer;
        public DateTime? FechaMinima
        {
            get { return fechaMinima; }
            set { fechaMinima = value; RaisePropertyChanged("FechaMinima"); }
        }

        #region Busqueda de Solicitudes de Interconsulta
        private ObservableCollection<INTERCONSULTA_TIPO> lstInterconsulta_TiposBuscar = null;
        public ObservableCollection<INTERCONSULTA_TIPO> LstInterconsulta_TiposBuscar
        {
            get { return lstInterconsulta_TiposBuscar; }
            set { lstInterconsulta_TiposBuscar = value; RaisePropertyChanged("LstInterconsulta_TiposBuscar"); }
        }

        private short selectedInter_TipoBuscarValue = -1;
        public short SelectedInter_TipoBuscarValue
        {
            get { return selectedInter_TipoBuscarValue; }
            set { selectedInter_TipoBuscarValue = value; RaisePropertyChanged("SelectedInter_TipoBuscarValue"); }
        }

        private short? anioBuscarInter = null;
        public short? AnioBuscarInter
        {
            get { return anioBuscarInter; }
            set { anioBuscarInter = value; RaisePropertyChanged("AnioBuscarInter"); }
        }

        private short? folioBuscarInter = null;
        public short? FolioBuscarInter
        {
            get { return folioBuscarInter; }
            set { folioBuscarInter = value; RaisePropertyChanged("FolioBuscarInter"); }
        }

        private string nombreBuscarInter = string.Empty;
        public string NombreBuscarInter
        {
            get { return nombreBuscarInter; }
            set { nombreBuscarInter = value; RaisePropertyChanged("NombreBuscarInter"); }
        }

        private string apellidoPaternoBuscarInter = string.Empty;
        public string ApellidoPaternoBuscarInter
        {
            get { return apellidoPaternoBuscarInter; }
            set { apellidoPaternoBuscarInter = value; RaisePropertyChanged("ApellidoPaternoBuscarInter"); }
        }

        private string apellidoMaternoBuscarInter = string.Empty;
        public string ApellidoMaternoBuscarInter
        {
            get { return apellidoMaternoBuscarInter; }
            set { apellidoMaternoBuscarInter = value; RaisePropertyChanged("ApellidoMaternoBuscarInter"); }
        }

        private DateTime? fechaInicialBuscarInter = null;
        public DateTime? FechaInicialBuscarInter
        {
            get { return fechaInicialBuscarInter; }
            set { fechaInicialBuscarInter = value; RaisePropertyChanged("FechaInicialBuscarInter"); }
        }

        private DateTime? fechaFinalBuscarInter = null;
        public DateTime? FechaFinalBuscarInter
        {
            get { return fechaFinalBuscarInter; }
            set { fechaFinalBuscarInter = value; RaisePropertyChanged("FechaFinalBuscarInter"); }
        }

        private ObservableCollection<INTERCONSULTA_SOLICITUD> listaInterconsultasBusqueda;
        public ObservableCollection<INTERCONSULTA_SOLICITUD> ListaInterconsultasBusqueda
        {
            get { return listaInterconsultasBusqueda; }
            set { listaInterconsultasBusqueda = value; RaisePropertyChanged("ListaInterconsultasBusqueda"); }
        }

        private INTERCONSULTA_SOLICITUD selectedInterconsultaBusqueda = null;
        public INTERCONSULTA_SOLICITUD SelectedInterconsultaBusqueda
        {
            get { return selectedInterconsultaBusqueda; }
            set { selectedInterconsultaBusqueda = value; RaisePropertyChanged("SelectedInterconsultaBusqueda"); }
        }

        private ObservableCollection<ATENCION_TIPO> lstAtencion_TipoBuscar = null;
        public ObservableCollection<ATENCION_TIPO> LstAtencion_TipoBuscar
        {
            get { return lstAtencion_TipoBuscar; }
            set { lstAtencion_TipoBuscar = value; RaisePropertyChanged("LstAtencion_TipoBuscar"); }
        }

        private short selectedAtencion_TipoBuscarValue = -1;
        public short SelectedAtencion_TipoBuscarValue
        {
            get { return selectedAtencion_TipoBuscarValue; }
            set { selectedAtencion_TipoBuscarValue = value; RaisePropertyChanged("SelectedAtencion_TipoBuscarValue"); }
        }

        #region Validaciones Busqueda Solicitudes
        private bool isFechaIniBusquedaSolValida = true;
        public bool IsFechaIniBusquedaSolValida
        {
            get { return isFechaIniBusquedaSolValida; }
            set { isFechaIniBusquedaSolValida = value; RaisePropertyChanged("IsFechaIniBusquedaSolValida"); }
        }
        #endregion
        #endregion

        

        private bool isModoInsercion = true;
        public bool IsModoInsercion
        {
            get { return isModoInsercion; }
            set { isModoInsercion = value; RaisePropertyChanged("IsModoInsercion"); }
        }


    }
}
