using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlPenales
{
    partial class SolicitudInterconsultaViewModel
    {
        #region Variables privadas
        private short?[] estatus_administrativos_inactivos = null;
        private CANALIZACION selectedCanalizacion;
        private enumModo modoVistaModelo = enumModo.INSERCION;
        private IQueryable<PROCESO_USUARIO> permisos;
        private bool _permisos_editar = false;
        private bool _permisos_agregar = false;
        
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

        private bool menuAtencionEnabled = false;
        public bool MenuAtencionEnabled
        {
            get { return menuAtencionEnabled; }
            set { menuAtencionEnabled = value; RaisePropertyChanged("MenuAtencionEnabled"); }
        }
        #endregion
        #region Datos de Solicitud

        private bool isServAuxSeleccionadosValid = false;
        public bool IsServAuxSeleccionadosValid
        {
            get { return isServAuxSeleccionadosValid; }
            set { isServAuxSeleccionadosValid = value; RaisePropertyChanged("IsServAuxSeleccionadosValid"); }
        }
        private bool isEspecialidadesValid = false;
        public bool IsEspecialidadesValid
        {
            get { return isEspecialidadesValid; }
            set { isEspecialidadesValid = value; RaisePropertyChanged("IsEspecialidadesValid"); }
        }

        private ObservableCollection<INTERCONSULTA_ATENCION_TIPO> lstInterconsultaTipo_Atencion;
        public ObservableCollection<INTERCONSULTA_ATENCION_TIPO> LstInterconsultaTipo_Atencion
        {
            get { return lstInterconsultaTipo_Atencion; }
            set { lstInterconsultaTipo_Atencion = value; RaisePropertyChanged("LstInterconsultaTipo_Atencion"); }
        }

        private ObservableCollection<EXT_ESPECIALIDAD> lstExtEspecialidad;
        public ObservableCollection<EXT_ESPECIALIDAD> LstExtEspecialidad
        {
            get { return lstExtEspecialidad; }
            set { lstExtEspecialidad = value; RaisePropertyChanged("LstExtEspecialidad"); }
        }

        private List<INTERCONSULTA_TIPO> lstExtEspecialidadInterconsulta_Tipo;

        private List<INTERCONSULTA_NIVEL_PRIORIDAD> lstExtEspecialidadNvlPrioridades;

        private bool isInterconsultaEnabled = false;
        public bool IsInterconsultaEnabled
        {
            get { return isInterconsultaEnabled; }
            set { isInterconsultaEnabled = value; RaisePropertyChanged("IsInterconsultaEnabled"); }
        }



        private short selectedInterconsultaTipoAtencion;
        public short SelectedInterconsultaTipoAtencion
        {
            get { return selectedInterconsultaTipoAtencion; }
            set { selectedInterconsultaTipoAtencion = value; OnPropertyValidateChanged("SelectedInterconsultaTipoAtencion"); }
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

 
 
        #region Busqueda de notas medicas
        private short? anioBuscarNotaMedica;
        public short? AnioBuscarNotaMedica
        {
            get { return anioBuscarNotaMedica; }
            set { anioBuscarNotaMedica = value; RaisePropertyChanged("AnioBuscarNotaMedica"); }
        }

        private int? folioBuscarNotaMedica;
        public int? FolioBuscarNotaMedica
        {
            get { return folioBuscarNotaMedica; }
            set { folioBuscarNotaMedica = value; RaisePropertyChanged("FolioBuscarNotaMedica"); }
        }

        private string nombreBuscarNotaMedica = string.Empty;
        public string NombreBuscarNotaMedica
        {
            get { return nombreBuscarNotaMedica; }
            set { nombreBuscarNotaMedica = value; RaisePropertyChanged("NombreBuscarNotaMedica"); }
        }

        private string apellidoPaternoBuscarNotaMedica;
        public string ApellidoPaternoBuscarNotaMedica
        {
            get { return apellidoPaternoBuscarNotaMedica; }
            set { apellidoPaternoBuscarNotaMedica = value; RaisePropertyChanged("ApellidoPaternoBuscarNotaMedica"); }
        }

        private string apellidoMaternoBuscarNotaMed = string.Empty;
        public string ApellidoMaternoBuscarNotaMed
        {
            get { return apellidoMaternoBuscarNotaMed; }
            set { apellidoMaternoBuscarNotaMed = value; RaisePropertyChanged("ApellidoMaternoBuscarNotaMed"); }
        }

        private DateTime? fechaInicialNotaMed = null;
        public DateTime? FechaInicialNotaMed
        {
            get { return fechaInicialNotaMed; }
            set { fechaInicialNotaMed = value; RaisePropertyChanged("FechaInicialNotaMed"); }
        }

        private DateTime? fechaFinalBuscarNotaMed = null;
        public DateTime? FechaFinalBuscarNotaMed
        {
            get { return fechaFinalBuscarNotaMed; }
            set { fechaFinalBuscarNotaMed = value; RaisePropertyChanged("FechaFinalBuscarNotaMed"); }
        }


        private ObservableCollection<NOTA_MEDICA> lstNotasMedicasBusqueda;
        public ObservableCollection<NOTA_MEDICA> LstNotasMedicasBusqueda
        {
            get { return lstNotasMedicasBusqueda; }
            set { lstNotasMedicasBusqueda = value; RaisePropertyChanged("LstNotasMedicasBusqueda"); }
        }

        private NOTA_MEDICA selectedNotaMedicaBusqueda;
        public NOTA_MEDICA SelectedNotaMedicaBusqueda
        {
            get { return selectedNotaMedicaBusqueda; }
            set { selectedNotaMedicaBusqueda = value; RaisePropertyChanged("SelectedNotaMedicaBusqueda"); }
        }

        private string textTituloBuscarNotaMedica = "Buscar Notas Médicas pendientes de Solicitud de Interconsulta";

        public string TextTituloBuscarNotaMedica
        {
            get { return textTituloBuscarNotaMedica; }
        }

        private NOTA_MEDICA selectedNotaMedica;

        private ObservableCollection<ATENCION_TIPO> lstAtencion_Tipo = null;
        public ObservableCollection<ATENCION_TIPO> LstAtencion_Tipo
        {
            get { return lstAtencion_Tipo; }
            set { lstAtencion_Tipo = value; RaisePropertyChanged("LstAtencion_Tipo"); }
        }

        private short selectedAtencion_TipoValue = -1;
        public short SelectedAtencion_TipoValue
        {
            get { return selectedAtencion_TipoValue; }
            set { selectedAtencion_TipoValue = value; RaisePropertyChanged("SelectedAtencion_TipoValue"); }
        }

        #region Validaciones
        private bool isFechaIniBusquedaValida = true;
        public bool IsFechaIniBusquedaValida
        {
            get { return isFechaIniBusquedaValida; }
            set { isFechaIniBusquedaValida = value; RaisePropertyChanged("IsFechaIniBusquedaValida"); }
        }
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
        #endregion
        #region Visibilidad de Controles
        private Visibility isCertificadoMedico = Visibility.Collapsed;
        public Visibility IsCertificadoMedico
        {
            get { return isCertificadoMedico; }
            set { isCertificadoMedico = value; RaisePropertyChanged("IsCertificadoMedico"); }
        }

        public Visibility exploracionFisicaVisible = Visibility.Collapsed;
        public Visibility ExploracionFisicaVisible
        {
            get { return exploracionFisicaVisible; }
            set { exploracionFisicaVisible = value; RaisePropertyChanged("ExploracionFisicaVisible"); }
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

        #region Huellas
        private Visibility _ShowPopUp = Visibility.Hidden;
        public Visibility ShowPopUp
        {
            get { return _ShowPopUp; }
            set
            {
                _ShowPopUp = value;
                OnPropertyChanged("ShowPopUp");
            }
        }
        private Visibility _ShowFingerPrint = Visibility.Hidden;
        public Visibility ShowFingerPrint
        {
            get { return _ShowFingerPrint; }
            set
            {
                _ShowFingerPrint = value;
                OnPropertyChanged("ShowFingerPrint");
            }
        }
        private Visibility _ShowLine = Visibility.Visible;
        public Visibility ShowLine
        {
            get { return _ShowLine; }
            set
            {
                _ShowLine = value;
                OnPropertyChanged("ShowLine");
            }
        }
        private Visibility _ShowOk = Visibility.Hidden;
        public Visibility ShowOk
        {
            get { return _ShowOk; }
            set
            {
                _ShowOk = value;
                OnPropertyChanged("ShowOk");
            }
        }
        IList<PlantillaBiometrico> HuellasCapturadas;
        private ImageSource _GuardaHuella;
        public ImageSource GuardaHuella
        {
            get { return _GuardaHuella; }
            set
            {
                _GuardaHuella = value;
                OnPropertyChanged("GuardaHuella");
            }
        }

        private string _ScannerMessage;
        public string ScannerMessage
        {
            get { return _ScannerMessage; }
            set
            {
                _ScannerMessage = value.ToUpper();
                OnPropertyChanged("ScannerMessage");
            }
        }

        private bool BanderaHuella;
        private BusquedaHuella _WindowBusqueda;
        public BusquedaHuella WindowBusqueda
        {
            get { return _WindowBusqueda; }
            set { _WindowBusqueda = value; }
        }
        private bool ParametroRequiereGuardarHuellas;

        private bool codigoEnabled = false;
        public bool CodigoEnabled
        {
            get { return codigoEnabled; }
            set { codigoEnabled = value; OnPropertyChanged("CodigoEnabled"); }
        }

        private bool nombreReadOnly = true;
        public bool NombreReadOnly
        {
            get { return nombreReadOnly; }
            set { nombreReadOnly = value; OnPropertyChanged("NombreReadOnly"); }
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
    }
}


