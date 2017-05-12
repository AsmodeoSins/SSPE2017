using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ControlPenales
{
    partial class NotaEvolucionViewModel
    {
        #region Busqueda
        private string _LabelNipCodigo = "NIP";
        public string LabelNipCodigo
        {
            get { return _LabelNipCodigo; }
            set { _LabelNipCodigo = value; OnPropertyChanged("LabelNipCodigo"); }
        }
        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }
        private bool crearNuevoExpedienteEnabled;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }
        private string apellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return apellidoPaternoBuscar; }
            set { apellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
        }

        private string apellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return apellidoMaternoBuscar; }
            set { apellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscar"); }
        }

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set
            {
                nombreBuscar = value; OnPropertyChanged("NombreBuscar");
            }
        }

        private int? anioBuscar;
        public int? AnioBuscar
        {
            get { return anioBuscar; }
            set { anioBuscar = value; OnPropertyChanged("AnioBuscar"); }
        }

        private int? folioBuscar;
        public int? FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value; OnPropertyChanged("FolioBuscar"); }
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

        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set
            {
                imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
            }
        }

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private bool emptyIngresoVisible = true;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                OnPropertyChanged("ListExpediente");
            }
        }

        private IMPUTADO InputadoInterno { get; set; }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    SelectIngreso = null;
                    EmptyIngresoVisible = !(selectExpediente.INGRESO.Count > 0);

                    //OBTENEMOS FOTO DE FRENTE
                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        else
                            ImagenImputado = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenImputado = new Imagenes().getImagenPerson();
                }
                else
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    EmptyIngresoVisible = true;
                }
                OnPropertyChanged("SelectExpediente");
            }
        }
        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (value == null)
                {
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                    return;
                }
                SelectIngresoEnabled = !Parametro.ESTATUS_ADMINISTRATIVO_INACT.Contains(value.ID_ESTATUS_ADMINISTRATIVO);
                ImagenImputado = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                    value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO
                : new Imagenes().getImagenPerson();
                if (value.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    OnPropertyChanged("SelectIngreso");
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
            }
        }

        private bool selectIngresoEnabled;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }
        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value; OnPropertyChanged("SelectedIngreso"); }
        }

        private bool _ElementosDisponibles = false;
        public bool ElementosDisponibles
        {
            get { return _ElementosDisponibles; }
            set { _ElementosDisponibles = value; OnPropertyChanged("ElementosDisponibles"); }
        }


        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        #endregion

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set { pInsertar = value; }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set { pEditar = value; }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
                //if (value)
                //MenuBuscarEnabled = value;
            }
        }

        private bool _MenuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return _MenuBuscarEnabled; }
            set { _MenuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }

        private bool _CamposBusquedaEnabled = true;
        public bool CamposBusquedaEnabled
        {
            get { return _CamposBusquedaEnabled; }
            set { _CamposBusquedaEnabled = value; OnPropertyChanged("CamposBusquedaEnabled"); }
        }


        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set { pImprimir = value; }
        }
        #endregion

        #region Progreso de Validaciones

        private bool _BHuellasEnabled = false;
        public bool BHuellasEnabled
        {
            get { return _BHuellasEnabled; }
            set { _BHuellasEnabled = value; OnPropertyChanged("BHuellasEnabled"); }
        }

        private string _ProgresoValidaciones = "Realice comparación biométrica y por gafete para avanzar PROGRESO ACTUAL 0 / 2";
        public string ProgresoValidaciones
        {
            get { return _ProgresoValidaciones; }
            set { _ProgresoValidaciones = value; OnPropertyChanged("ProgresoValidaciones"); }
        }

        private int _MaxValidaciones = 2;
        public int MaxValidaciones
        {
            get { return _MaxValidaciones; }
            set { _MaxValidaciones = value; OnPropertyChanged("MaxValidaciones"); }
        }

        private int _ProgesoActual = 0;
        public int ProgesoActual
        {
            get { return _ProgesoActual; }
            set { _ProgesoActual = value; OnPropertyChanged("ProgesoActual"); }
        }


        private bool _IsPlanimetriaValidada = false;
        public bool IsPlanimetriaValidada
        {
            get { return _IsPlanimetriaValidada; }
            set { _IsPlanimetriaValidada = value; OnPropertyChanged("IsPlanimetriaValidada"); }
        }

        private bool _IsBusquedaSimpleValidada = false;
        public bool IsBusquedaSimpleValidada
        {
            get { return _IsBusquedaSimpleValidada; }
            set { _IsBusquedaSimpleValidada = value; OnPropertyChanged("IsBusquedaSimpleValidada"); }
        }


        #endregion

        //private IList<ResultadoBusquedaBiometrico> _ListResultado;
        //public IList<ResultadoBusquedaBiometrico> ListResultado
        //{
        //    get { return _ListResultado; }
        //    set
        //    {
        //        _ListResultado = value;
        //        OnPropertyChanged("ListResultado");
        //    }
        //}
        private string _TextEstatura;
        public string TextEstatura
        {
            get { return _TextEstatura; }
            set { _TextEstatura = value; OnPropertyChanged("TextEstatura"); }
        }
        private Visibility _EstaturaVisible;
        public Visibility EstaturaVisible
        {
            get { return _EstaturaVisible; }
            set { _EstaturaVisible = value; OnPropertyChanged("EstaturaVisible"); }
        }
        private short _ObservacionesSignosVitalesColumn;
        public short ObservacionesSignosVitalesColumn
        {
            get { return _ObservacionesSignosVitalesColumn; }
            set { _ObservacionesSignosVitalesColumn = value; OnPropertyChanged("ObservacionesSignosVitalesColumn"); }
        }
        private int? idPersona;
        private BuscarPorHuellaYNipView HuellaWindow;
        private bool _BuscarReadOnly = true;
        public bool BuscarReadOnly
        {
            get { return _BuscarReadOnly; }
            set { _BuscarReadOnly = value; OnPropertyChanged("BuscarReadOnly"); }
        }
        private bool _DietasEnabled = true;
        public bool DietasEnabled
        {
            get { return _DietasEnabled; }
            set { _DietasEnabled = value; OnPropertyChanged("DietasEnabled"); }
        }
        private bool _MenuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return _MenuGuardarEnabled; }
            set { _MenuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }
        private bool LeyendoHuellas { get; set; }

        #region [Huellas]

        //private enumTipoBiometrico? _DD_Dedo;
        //public enumTipoBiometrico? DD_Dedo
        //{
        //    get { return _DD_Dedo; }
        //    set { _DD_Dedo = value; }
        //}

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

        private Brush _PulgarDerecho;
        public Brush PulgarDerecho
        {
            get { return _PulgarDerecho; }
            set
            {
                _PulgarDerecho = value;
                RaisePropertyChanged("PulgarDerecho");
            }
        }

        private ImageSource _PulgarDerechoBMP;
        public ImageSource PulgarDerechoBMP
        {
            get { return _PulgarDerechoBMP; }
            set
            {
                _PulgarDerechoBMP = value;
                RaisePropertyChanged("PulgarDerechoBMP");
            }
        }

        private Brush _IndiceDerecho;
        public Brush IndiceDerecho
        {
            get { return _IndiceDerecho; }
            set
            {
                _IndiceDerecho = value;
                OnPropertyChanged("IndiceDerecho");
            }
        }

        private Brush _MedioDerecho;
        public Brush MedioDerecho
        {
            get { return _MedioDerecho; }
            set
            {
                _MedioDerecho = value;
                OnPropertyChanged("MedioDerecho");
            }
        }

        private Brush _AnularDerecho;
        public Brush AnularDerecho
        {
            get { return _AnularDerecho; }
            set
            {
                _AnularDerecho = value;
                OnPropertyChanged("AnularDerecho");
            }
        }

        private Brush _MeñiqueDerecho;
        public Brush MeñiqueDerecho
        {
            get { return _MeñiqueDerecho; }
            set
            {
                _MeñiqueDerecho = value;
                OnPropertyChanged("MeñiqueDerecho");
            }
        }

        private Brush _PulgarIzquierdo;
        public Brush PulgarIzquierdo
        {
            get { return _PulgarIzquierdo; }
            set
            {
                _PulgarIzquierdo = value;
                OnPropertyChanged("PulgarIzquierdo");
            }
        }

        private Brush _IndiceIzquierdo;
        public Brush IndiceIzquierdo
        {
            get { return _IndiceIzquierdo; }
            set
            {
                _IndiceIzquierdo = value;
                OnPropertyChanged("IndiceIzquierdo");
            }
        }

        private Brush _MedioIzquierdo;
        public Brush MedioIzquierdo
        {
            get { return _MedioIzquierdo; }
            set
            {
                _MedioIzquierdo = value;
                OnPropertyChanged("MedioIzquierdo");
            }
        }

        private Brush _AnularIzquierdo;
        public Brush AnularIzquierdo
        {
            get { return _AnularIzquierdo; }
            set
            {
                _AnularIzquierdo = value;
                OnPropertyChanged("AnularIzquierdo");
            }
        }

        private Brush _MeñiqueIzquierdo;
        public Brush MeñiqueIzquierdo
        {
            get { return _MeñiqueIzquierdo; }
            set
            {
                _MeñiqueIzquierdo = value;
                OnPropertyChanged("MeñiqueIzquierdo");
            }
        }
        #endregion

        private string textNombre;
        public string TextNombre
        {
            get { return textNombre; }
            set { textNombre = value; OnPropertyChanged("TextNombre"); }
        }
        private string textPaterno;
        public string TextPaterno
        {
            get { return textPaterno; }
            set { textPaterno = value; OnPropertyChanged("TextPaterno"); }
        }
        private string textMaterno;
        public string TextMaterno
        {
            get { return textMaterno; }
            set { textMaterno = value; OnPropertyChanged("TextMaterno"); }
        }

        private IMPUTADO imputado;
        public IMPUTADO Imputado
        {
            get { return imputado; }
            set
            {
                imputado = value;
                OnPropertyChanged("Imputado");
            }
        }

        #region Datos del Paciente
        private string _NombrePaciente;
        public string NombrePaciente
        {
            get { return _NombrePaciente; }
            set { _NombrePaciente = value; OnPropertyChanged("NombrePaciente"); }
        }

        private string _TextEdad;
        public string TextEdad
        {
            get { return _TextEdad; }
            set { _TextEdad = value; OnPropertyChanged("TextEdad"); }
        }
        private string _Sexo;
        public string TextSexo
        {
            get { return _Sexo; }
            set { _Sexo = value; OnPropertyChanged("TextSexo"); }
        }

        private DateTime? _FechaYHora;
        public DateTime? FechaYHora
        {
            get { return _FechaYHora; }
            set { _FechaYHora = value; OnPropertyChanged("FechaYHora"); }
        }

        private string _Edad;
        public string Edad
        {
            get { return _Edad; }
            set { _Edad = value; OnPropertyChanged("Edad"); }
        }


        #endregion

        #region Enabled en tabs
        private bool _IsSignosVitalesVisible = false;
        public bool IsSignosVitalesVisible
        {
            get { return _IsSignosVitalesVisible; }
            set { _IsSignosVitalesVisible = value; OnPropertyChanged("IsSignosVitalesVisible"); }
        }

        private bool _IsDiagnosticoEnabled = false;
        public bool IsDiagnosticoEnabled
        {
            get { return _IsDiagnosticoEnabled; }
            set { _IsDiagnosticoEnabled = value; OnPropertyChanged("IsDiagnosticoEnabled"); }
        }

        private bool _AtencionTipoEnabled = false;
        public bool AtencionTipoEnabled
        {
            get { return _AtencionTipoEnabled; }
            set { _AtencionTipoEnabled = value; OnPropertyChanged("AtencionTipoEnabled"); }
        }

        private bool _IsDentista = false;
        public bool IsDentista
        {
            get { return _IsDentista; }
            set
            {
                _IsDentista = value;
                AtencionTipoEnabled = IsMedico && value;
            }
        }

        private bool _IsMedico = false;
        public bool IsMedico
        {
            get { return _IsMedico; }
            set
            {
                _IsMedico = value;
                AtencionTipoEnabled = IsDentista && value;
            }
        }

        private bool _IsEnfermero = false;
        public bool IsEnfermero
        {
            get { return _IsEnfermero; }
            set { _IsEnfermero = value; }
        }

        private bool _IsAmbos = false; //se agrega este campo para validar en casos extra ordinarios que un usuario tenga ambos roles dentro del sistema
        public bool IsAmbos
        {
            get { return _IsAmbos; }
            set { _IsAmbos = value; }
        }



        #endregion

        #region Signos Vitales
        private string _TextUltimaHoraSignos;
        public string TextUltimaHoraSignos
        {
            get { return _TextUltimaHoraSignos; }
            set { _TextUltimaHoraSignos = value; OnPropertyChanged("TextUltimaHoraSignos"); }
        }
        private string _TextUltimaFechaSignos;
        public string TextUltimaFechaSignos
        {
            get { return _TextUltimaFechaSignos; }
            set { _TextUltimaFechaSignos = value; OnPropertyChanged("TextUltimaFechaSignos"); }
        }
        private string _TextLabelTallaGlicemia;
        public string TextLabelTallaGlicemia
        {
            get { return _TextLabelTallaGlicemia; }
            set { _TextLabelTallaGlicemia = value; OnPropertyChanged("TextLabelTallaGlicemia"); }
        }
        private Visibility _ObsSignosVitalesVisible = Visibility.Visible;
        public Visibility ObsSignosVitalesVisible
        {
            get { return _ObsSignosVitalesVisible; }
            set { _ObsSignosVitalesVisible = value; OnPropertyChanged("ObsSignosVitalesVisible"); }
        }
        private bool _TallaEnabled;
        public bool TallaEnabled
        {
            get { return _TallaEnabled; }
            set { _TallaEnabled = value; OnPropertyChanged("TallaEnabled"); }
        }
        private bool _PesoEnabled;
        public bool PesoEnabled
        {
            get { return _PesoEnabled; }
            set { _PesoEnabled = value; OnPropertyChanged("PesoEnabled"); }
        }
        private bool _SignosVitalesReadOnly;
        public bool SignosVitalesReadOnly
        {
            get { return _SignosVitalesReadOnly; }
            set { _SignosVitalesReadOnly = value; OnPropertyChanged("SignosVitalesReadOnly"); }
        }
        private bool _SignosVitalesExpandidos;
        public bool SignosVitalesExpandidos
        {
            get { return _SignosVitalesExpandidos; }
            set { _SignosVitalesExpandidos = value; OnPropertyChanged("SignosVitalesExpandidos"); }
        }
        private short? _IdAtencionMedica;
        public short? IdAtencionMedica
        {
            get { return _IdAtencionMedica; }
            set { _IdAtencionMedica = value; OnPropertyChanged("IdAtencionMedica"); }
        }
        private int? _IdResponsableSignosVitales;
        public int? IdResponsableSignosVitales
        {
            get { return _IdResponsableSignosVitales; }
            set { _IdResponsableSignosVitales = value; OnPropertyChanged("IdResponsableSignosVitales"); }
        }
        private string _TensionArterial1;
        public string TextTensionArterial1
        {
            get { return _TensionArterial1; }
            set
            {
                _TensionArterial1 = value;
                if (!string.IsNullOrEmpty(value))
                    TensionArterial = value + "/" + TextTensionArterial2;
                OnPropertyValidateChanged("TextTensionArterial1");
            }
        }
        private string _TensionArterial2;
        public string TextTensionArterial2
        {
            get { return _TensionArterial2; }
            set
            {
                _TensionArterial2 = value;
                if (!string.IsNullOrEmpty(value))
                    TensionArterial = TextTensionArterial1 + "/" + value;
                OnPropertyValidateChanged("TextTensionArterial2");
            }
        }
        private string _TensionArterial;
        public string TensionArterial
        {
            get { return _TensionArterial; }
            set { _TensionArterial = value; OnPropertyValidateChanged("TensionArterial"); }
        }
        private string _FrecuenciaCardiaca;
        public string TextFrecuenciaCardiaca
        {
            get { return _FrecuenciaCardiaca; }
            set { _FrecuenciaCardiaca = value; OnPropertyValidateChanged("TextFrecuenciaCardiaca"); }
        }
        private string _Temperatura;
        public string TextTemperatura
        {
            get { return _Temperatura; }
            set { _Temperatura = value; OnPropertyValidateChanged("TextTemperatura"); }
        }
        private string _Peso;
        public string TextPeso
        {
            get { return _Peso; }
            set
            {
                _Peso = value;
                OnPropertyValidateChanged("TextPeso");
            }
        }
        private string _Glucemia;
        public string TextGlucemia
        {
            get { return _Glucemia; }
            set { _Glucemia = value; OnPropertyValidateChanged("TextGlucemia"); }
        }
        private string _Talla;
        public string TextTalla
        {
            get { return _Talla; }
            set { _Talla = value; OnPropertyValidateChanged("TextTalla"); }
        }
        private string _FrecuenciaRespira;
        public string TextFrecuenciaRespira
        {
            get { return _FrecuenciaRespira; }
            set { _FrecuenciaRespira = value; OnPropertyValidateChanged("TextFrecuenciaRespira"); }
        }
        private string _ObservacionesSignosVitales;
        public string ObservacionesSignosVitales
        {
            get { return _ObservacionesSignosVitales; }
            set { _ObservacionesSignosVitales = value; OnPropertyValidateChanged("ObservacionesSignosVitales"); }
        }
        private string _ResponsableSignosVitales;
        public string ResponsableSignosVitales
        {
            get { return _ResponsableSignosVitales; }
            set { _ResponsableSignosVitales = value; OnPropertyChanged("ResponsableSignosVitales"); }
        }
        #endregion

        #region Nota Medica
        private int? _IdResponsableNotaMedica;
        public int? IdResponsableNotaMedica
        {
            get { return _IdResponsableNotaMedica; }
            set { _IdResponsableNotaMedica = value; OnPropertyChanged("IdResponsableNotaMedica"); }
        }

        private string _ResponsableNotaMedica;
        public string ResponsableNotaMedica
        {
            get { return _ResponsableNotaMedica; }
            set { _ResponsableNotaMedica = value; OnPropertyChanged("ResponsableNotaMedica"); }
        }

        private int _IdAtencionMedicaNotaMedica;
        public int IdAtencionMedicaNotaMedica
        {
            get { return _IdAtencionMedicaNotaMedica; }
            set { _IdAtencionMedicaNotaMedica = value; OnPropertyChanged("IdAtencionMedicaNotaMedica"); }
        }

        private ObservableCollection<PRONOSTICO> listPronostico;
        public ObservableCollection<PRONOSTICO> ListPronostico
        {
            get { return listPronostico; }
            set { listPronostico = value; RaisePropertyChanged("ListPronostico"); }
        }

        private short? selectedPronostico = -1;
        public short? SelectPronostico
        {
            get { return selectedPronostico; }
            set { selectedPronostico = value; RaisePropertyChanged("SelectPronostico"); }
        }

        //private string _Pronostico;
        //public string Pronostico
        //{
        //    get { return _Pronostico; }
        //    set { _Pronostico = value; OnPropertyValidateChanged("Pronostico"); }
        //}

        private string _ResultadoServTrat;
        public string ResultadoServTrat
        {
            get { return _ResultadoServTrat; }
            set { _ResultadoServTrat = value; OnPropertyValidateChanged("ResultadoServTrat"); }
        }

        private string _Diagnostico;
        public string Diagnostico
        {
            get { return _Diagnostico; }
            set { _Diagnostico = value; OnPropertyValidateChanged("Diagnostico"); }
        }

        private string _PlanEstudioTrat;
        public string PlanEstudioTrat
        {
            get { return _PlanEstudioTrat; }
            set { _PlanEstudioTrat = value; OnPropertyValidateChanged("PlanEstudioTrat"); }
        }

        private string _ExploracionFisica;
        public string TextExploracionFisica
        {
            get { return _ExploracionFisica; }
            set { _ExploracionFisica = value; OnPropertyValidateChanged("TextExploracionFisica"); }
        }

        private string _TextEstudios;
        public string TextEstudios
        {
            get { return _TextEstudios; }
            set { _TextEstudios = value; OnPropertyChanged("TextEstudios"); }
        }

        private string _ResumenInterr;
        public string TextResumenInterrogatorio
        {
            get { return _ResumenInterr; }
            set { _ResumenInterr = value; OnPropertyValidateChanged("TextResumenInterrogatorio"); }
        }

        private string _ResultadoServAux;
        public string ResultadoServAux
        {
            get { return _ResultadoServAux; }
            set { _ResultadoServAux = value; OnPropertyValidateChanged("ResultadoServAux"); }
        }

        #endregion

        private int _MaxLength20;
        public int MaxLength20
        {
            get { return _MaxLength20; }
            set { _MaxLength20 = value; OnPropertyChanged("MaxLength20"); }
        }
        private int _MaxLength500;
        public int MaxLength500
        {
            get { return _MaxLength500; }
            set { _MaxLength500 = value; OnPropertyChanged("MaxLength500"); }
        }
        private NOTA_SIGNOS_VITALES _SelectedSignosVitales;
        public NOTA_SIGNOS_VITALES SelectedSignosVitales
        {
            get { return _SelectedSignosVitales; }
            set { _SelectedSignosVitales = value; }
        }
        private NOTA_MEDICA _SelectedNotaMedica;
        public NOTA_MEDICA SelectedNotaMedica
        {
            get { return _SelectedNotaMedica; }
            set { _SelectedNotaMedica = value; OnPropertyChanged("SelectedNotaMedica"); }
        }
        private short? _TabSelected;
        public short? TabSelected
        {
            get { return _TabSelected; }
            set
            {
                _TabSelected = value;
                if (value.HasValue)
                {
                    switch (value)
                    {
                        case (short)enumNotaMedica.NOTA_EVOLUCION:
                            break;
                        case (short)enumNotaMedica.NOTA_INTERCONSULTA:
                            break;
                        case (short)enumNotaMedica.NOTA_POST_OP:
                            break;
                        case (short)enumNotaMedica.NOTA_PRE_ANEST:
                            break;
                        case (short)enumNotaMedica.NOTA_PRE_OP:
                            break;
                        case (short)enumNotaMedica.NOTA_TRASLADO:
                            break;
                        case (short)enumNotaMedica.NOTA_URGENCIA:
                            break;

                    }
                }
                OnPropertyChanged("TabSelected");
            }
        }

        #region Nota Urgencia
        private bool _TabDatosNotaUrgencia;
        public bool TabDatosNotaUrgencia
        {
            get { return _TabDatosNotaUrgencia; }
            set { _TabDatosNotaUrgencia = value; OnPropertyChanged("TabDatosNotaUrgencia"); }
        }
        private string _TextUrgenciaMotivo;
        public string TextUrgenciaMotivo
        {
            get { return _TextUrgenciaMotivo; }
            set { _TextUrgenciaMotivo = value; OnPropertyValidateChanged("TextUrgenciaMotivo"); }
        }
        private string _TextUrgenciaEstadoMental;
        public string TextUrgenciaEstadoMental
        {
            get { return _TextUrgenciaEstadoMental; }
            set { _TextUrgenciaEstadoMental = value; OnPropertyValidateChanged("TextUrgenciaEstadoMental"); }
        }
        private string _TextUrgenciaDestino;
        public string TextUrgenciaDestino
        {
            get { return _TextUrgenciaDestino; }
            set { _TextUrgenciaDestino = value; OnPropertyValidateChanged("TextUrgenciaDestino"); }
        }
        private string _TextUrgenciaProcedimiento;
        public string TextUrgenciaProcedimiento
        {
            get { return _TextUrgenciaProcedimiento; }
            set { _TextUrgenciaProcedimiento = value; OnPropertyValidateChanged("TextUrgenciaProcedimiento"); }
        }
        #endregion

        #region Nota Evolucion
        private bool _TabDatosNotaEvolucion;
        public bool TabDatosNotaEvolucion
        {
            get { return _TabDatosNotaEvolucion; }
            set { _TabDatosNotaEvolucion = value; OnPropertyChanged("TabDatosNotaEvolucion"); }
        }
        private string _TextEvolucion;
        public string TextEvolucion
        {
            get { return _TextEvolucion; }
            set { _TextEvolucion = value; OnPropertyValidateChanged("TextEvolucion"); }
        }
        private string _TextActualizacion;
        public string TextActualizacion
        {
            get { return _TextActualizacion; }
            set { _TextActualizacion = value; OnPropertyValidateChanged("TextActualizacion"); }
        }
        private string _TextExistencia;
        public string TextExistencia
        {
            get { return _TextExistencia; }
            set { _TextExistencia = value; OnPropertyValidateChanged("TextExistencia"); }
        }
        #endregion

        #region Nota Traslado
        private bool _TabDatosNotaTraslado;
        public bool TabDatosNotaTraslado
        {
            get { return _TabDatosNotaTraslado; }
            set { _TabDatosNotaTraslado = value; OnPropertyChanged("TabDatosNotaTraslado"); }
        }
        private string _TextTrasladoMotivo;
        public string TextTrasladoMotivo
        {
            get { return _TextTrasladoMotivo; }
            set { _TextTrasladoMotivo = value; OnPropertyValidateChanged("TextTrasladoMotivo"); }
        }
        private string _TextTrasladoEnvia;
        public string TextTrasladoEnvia
        {
            get { return _TextTrasladoEnvia; }
            set { _TextTrasladoEnvia = value; OnPropertyValidateChanged("TextTrasladoEnvia"); }
        }
        private string _TextTrasladoReceptor;
        public string TextTrasladoReceptor
        {
            get { return _TextTrasladoReceptor; }
            set { _TextTrasladoReceptor = value; OnPropertyValidateChanged("TextTrasladoReceptor"); }
        }
        #endregion

        #region Nota Interconsulta
        private bool _TabDatosNotaInterconsulta;
        public bool TabDatosNotaInterconsulta
        {
            get { return _TabDatosNotaInterconsulta; }
            set { _TabDatosNotaInterconsulta = value; OnPropertyChanged("TabDatosNotaInterconsulta"); }
        }
        private string _TextInterconsultaCriterio;
        public string TextInterconsultaCriterio
        {
            get { return _TextInterconsultaCriterio; }
            set { _TextInterconsultaCriterio = value; OnPropertyValidateChanged("TextInterconsultaCriterio"); }
        }
        private string _TextInterconsultaSugerencias;
        public string TextInterconsultaSugerencias
        {
            get { return _TextInterconsultaSugerencias; }
            set { _TextInterconsultaSugerencias = value; OnPropertyValidateChanged("TextInterconsultaSugerencias"); }
        }
        private string _TextInterconsultaTratamiento;
        public string TextInterconsultaTratamiento
        {
            get { return _TextInterconsultaTratamiento; }
            set { _TextInterconsultaTratamiento = value; OnPropertyValidateChanged("TextInterconsultaTratamiento"); }
        }
        private string _TextInterconsultaMotivo;
        public string TextInterconsultaMotivo
        {
            get { return _TextInterconsultaMotivo; }
            set { _TextInterconsultaMotivo = value; OnPropertyValidateChanged("TextInterconsultaMotivo"); }
        }
        #endregion

        #region Nota Pre Operatoria
        private bool _TabDatosNotaPreOp;
        public bool TabDatosNotaPreOp
        {
            get { return _TabDatosNotaPreOp; }
            set { _TabDatosNotaPreOp = value; OnPropertyChanged("TabDatosNotaPreOp"); }
        }
        private DateTime? _TextPreOpFecha;
        public DateTime? TextPreOpFecha
        {
            get { return _TextPreOpFecha; }
            set { _TextPreOpFecha = value; OnPropertyValidateChanged("TextPreOpFecha"); }
        }
        private string _TextPreOpDiagnostico;
        public string TextPreOpDiagnostico
        {
            get { return _TextPreOpDiagnostico; }
            set { _TextPreOpDiagnostico = value; OnPropertyValidateChanged("TextPreOpDiagnostico"); }
        }
        private string _TextPreOpPlan;
        public string TextPreOpPlan
        {
            get { return _TextPreOpPlan; }
            set { _TextPreOpPlan = value; OnPropertyValidateChanged("TextPreOpPlan"); }
        }
        private string _TextPreOpRiesgo;
        public string TextPreOpRiesgo
        {
            get { return _TextPreOpRiesgo; }
            set { _TextPreOpRiesgo = value; OnPropertyValidateChanged("TextPreOpRiesgo"); }
        }
        private string _TextPreOpCuidados;
        public string TextPreOpCuidados
        {
            get { return _TextPreOpCuidados; }
            set { _TextPreOpCuidados = value; OnPropertyValidateChanged("TextPreOpCuidados"); }
        }
        private string _TextPreOpPlanTerapeutico;
        public string TextPreOpPlanTerapeutico
        {
            get { return _TextPreOpPlanTerapeutico; }
            set { _TextPreOpPlanTerapeutico = value; OnPropertyValidateChanged("TextPreOpPlanTerapeutico"); }
        }
        #endregion

        #region Nota Pre Anestesica
        private bool _TabdatosNotaPreAnest;
        public bool TabdatosNotaPreAnest
        {
            get { return _TabdatosNotaPreAnest; }
            set { _TabdatosNotaPreAnest = value; OnPropertyChanged("TabdatosNotaPreAnest"); }
        }
        private string _TextPreAnestEvolucion;
        public string TextPreAnestEvolucion
        {
            get { return _TextPreAnestEvolucion; }
            set { _TextPreAnestEvolucion = value; OnPropertyValidateChanged("TextPreAnestEvolucion"); }
        }
        private string _TextPreAnestTipo;
        public string TextPreAnestTipo
        {
            get { return _TextPreAnestTipo; }
            set { _TextPreAnestTipo = value; OnPropertyValidateChanged("TextPreAnestTipo"); }
        }
        private string _TextPreAnestRiesgo;
        public string TextPreAnestRiesgo
        {
            get { return _TextPreAnestRiesgo; }
            set { _TextPreAnestRiesgo = value; OnPropertyValidateChanged("TextPreAnestRiesgo"); }
        }
        #endregion

        #region Nota Post Operatorio
        private bool _TabDatosNotaPostOp;
        public bool TabDatosNotaPostOp
        {
            get { return _TabDatosNotaPostOp; }
            set { _TabDatosNotaPostOp = value; OnPropertyChanged("TabDatosNotaPostOp"); }
        }
        private string _TextPostOpPlaneada;
        public string TextPostOpPlaneada
        {
            get { return _TextPostOpPlaneada; }
            set { _TextPostOpPlaneada = value; OnPropertyValidateChanged("TextPostOpPlaneada"); }
        }
        private string _TextPostOpRealizada;
        public string TextPostOpRealizada
        {
            get { return _TextPostOpRealizada; }
            set { _TextPostOpRealizada = value; OnPropertyValidateChanged("TextPostOpRealizada"); }
        }
        private string _TextPostOpDiagnosticoPostOp;
        public string TextPostOpDiagnosticoPostOp
        {
            get { return _TextPostOpDiagnosticoPostOp; }
            set { _TextPostOpDiagnosticoPostOp = value; OnPropertyValidateChanged("TextPostOpDiagnosticoPostOp"); }
        }
        private string _TextPostOpTecnicaQuirurgica;
        public string TextPostOpTecnicaQuirurgica
        {
            get { return _TextPostOpTecnicaQuirurgica; }
            set { _TextPostOpTecnicaQuirurgica = value; OnPropertyValidateChanged("TextPostOpTecnicaQuirurgica"); }
        }
        private string _TextPostOpHallazgosTransoperatorios;
        public string TextPostOpHallazgosTransoperatorios
        {
            get { return _TextPostOpHallazgosTransoperatorios; }
            set { _TextPostOpHallazgosTransoperatorios = value; OnPropertyValidateChanged("TextPostOpHallazgosTransoperatorios"); }
        }
        private string _TextPostOpGasasCompresas;
        public string TextPostOpGasasCompresas
        {
            get { return _TextPostOpGasasCompresas; }
            set { _TextPostOpGasasCompresas = value; OnPropertyValidateChanged("TextPostOpGasasCompresas"); }
        }
        private string _TextPostOpIncidentes;
        public string TextPostOpIncidentes
        {
            get { return _TextPostOpIncidentes; }
            set { _TextPostOpIncidentes = value; OnPropertyValidateChanged("TextPostOpIncidentes"); }
        }
        private string _TextPostOpAccidentes;
        public string TextPostOpAccidentes
        {
            get { return _TextPostOpAccidentes; }
            set { _TextPostOpAccidentes = value; OnPropertyValidateChanged("TextPostOpAccidentes"); }
        }
        private string _TextPostOpSangrado;
        public string TextPostOpSangrado
        {
            get { return _TextPostOpSangrado; }
            set { _TextPostOpSangrado = value; OnPropertyValidateChanged("TextPostOpSangrado"); }
        }
        private string _TextPostOpInterpretacion;
        public string TextPostOpInterpretacion
        {
            get { return _TextPostOpInterpretacion; }
            set { _TextPostOpInterpretacion = value; OnPropertyValidateChanged("TextPostOpInterpretacion"); }
        }
        private string _TextPostOpEstadoInmediato;
        public string TextPostOpEstadoInmediato
        {
            get { return _TextPostOpEstadoInmediato; }
            set { _TextPostOpEstadoInmediato = value; OnPropertyValidateChanged("TextPostOpEstadoInmediato"); }
        }
        private string _TextPostOpTratamientoInmediato;
        public string TextPostOpTratamientoInmediato
        {
            get { return _TextPostOpTratamientoInmediato; }
            set { _TextPostOpTratamientoInmediato = value; OnPropertyValidateChanged("TextPostOpTratamientoInmediato"); }
        }
        private string _TextPostOpPiezasBiopsias;
        public string TextPostOpPiezasBiopsias
        {
            get { return _TextPostOpPiezasBiopsias; }
            set { _TextPostOpPiezasBiopsias = value; OnPropertyValidateChanged("TextPostOpPiezasBiopsias"); }
        }
        #endregion

        private bool _AgregarProcedimientoMedicoEnabled;
        public bool AgregarProcedimientoMedicoEnabled
        {
            get { return _AgregarProcedimientoMedicoEnabled; }
            set { _AgregarProcedimientoMedicoEnabled = value; OnPropertyChanged("AgregarProcedimientoMedicoEnabled"); }
        }
        private PROC_MED SelectProcMedEnCitaParaAgendarAux;
        private PROC_MED _SelectProcMedEnCitaParaAgendar;
        public PROC_MED SelectProcMedEnCitaParaAgendar
        {
            get { return _SelectProcMedEnCitaParaAgendar; }
            set
            {
                _SelectProcMedEnCitaParaAgendar = value;
                if (value != null ? value.ID_PROCMED != -1 : false)
                {
                    if (ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED != null ? a.PROC_MED.PROC_MATERIAL != null ? a.PROC_MED.ID_PROCMED == value.ID_PROCMED : false : false))
                    {
                        EliminarProcedimientoMedicoEnabled = true;
                        AgregarProcedimientoMedicoEnabled = false;
                    }
                    else
                    {
                        EliminarProcedimientoMedicoEnabled = false;
                        AgregarProcedimientoMedicoEnabled = true;
                    }
                    SelectProcMedEnCitaParaAgendarAux = new PROC_MED
                    {
                        DESCR = value.DESCR,
                        ESTATUS = value.ESTATUS,
                        ID_PROCMED = value.ID_PROCMED,
                        ID_PROCMED_SUBTIPO = value.ID_PROCMED_SUBTIPO,
                        PROC_MED_SUBTIPO = value.PROC_MED_SUBTIPO,
                        PROC_ATENCION_MEDICA = value.PROC_ATENCION_MEDICA,
                        PROC_MATERIAL = value.PROC_MATERIAL
                    }; ;
                }
                else
                {
                    //SelectProcMedEnCitaParaAgendarAux = null;
                    EliminarProcedimientoMedicoEnabled = false;
                    AgregarProcedimientoMedicoEnabled = false;
                }
                OnPropertyChanged("SelectProcMedEnCitaParaAgendar");
            }
        }
        private ObservableCollection<PROC_MED> _ListProcedimientosMedicosEnCitaParaAgregar;
        public ObservableCollection<PROC_MED> ListProcedimientosMedicosEnCitaParaAgregar
        {
            get { return _ListProcedimientosMedicosEnCitaParaAgregar; }
            set { _ListProcedimientosMedicosEnCitaParaAgregar = value; OnPropertyChanged("ListProcedimientosMedicosEnCitaParaAgregar"); }
        }
        private bool _TipoNotaEnabled = false;
        public bool TipoNotaEnabled
        {
            get { return _TipoNotaEnabled; }
            set { _TipoNotaEnabled = value; OnPropertyChanged("TipoNotaEnabled"); }
        }
        private ObservableCollection<ATENCION_SERVICIO> ListTipoServicioAux;
        private ObservableCollection<ATENCION_SERVICIO> _ListTipoServicio;
        public ObservableCollection<ATENCION_SERVICIO> ListTipoServicio
        {
            get { return _ListTipoServicio; }
            set { _ListTipoServicio = value; OnPropertyChanged("ListTipoServicio"); }
        }
        private ObservableCollection<ATENCION_TIPO> _ListAtencionTipo;
        public ObservableCollection<ATENCION_TIPO> ListAtencionTipo
        {
            get { return _ListAtencionTipo; }
            set { _ListAtencionTipo = value; OnPropertyChanged("ListAtencionTipo"); }
        }
        private bool BuscarAgenda = false;
        private bool _TipoNotaSeleccionada;
        public bool TipoNotaSeleccionada
        {
            get { return _TipoNotaSeleccionada; }
            set { _TipoNotaSeleccionada = value; OnPropertyChanged("TipoNotaSeleccionada"); }
        }
        private string _SelectedValueTipoAtencion;
        public string SelectedValueTipoAtencion
        {
            get { return _SelectedValueTipoAtencion; }
            set
            {
                _SelectedValueTipoAtencion = value;
                OnPropertyChanged("SelectedValueTipoAtencion");
            }
        }
        private ATENCION_SERVICIO _SelectTipoServicio;
        public ATENCION_SERVICIO SelectTipoServicio
        {
            get { return _SelectTipoServicio; }
            set
            {
                _SelectTipoServicio = value;
                OnPropertyChanged("SelectTipoServicio");
            }
        }
        private string _TextContador;
        public string TextContador
        {
            get { return _TextContador; }
            set { _TextContador = value; OnPropertyChanged("TextContador"); }
        }
        private DateTime FechaActualizacion;

        #region Timer
        //private DispatcherTimer timer;
        //private DateTime FechaActualizacion;
        private string hora;
        private string minuto;
        private string segundo;
        private string _actualizacion;
        public string Actualizacion
        {
            get { return _actualizacion; }
            set
            {
                if (value != null)
                {
                    _actualizacion = value;
                    OnPropertyChanged("Actualizacion");
                }
            }
        }
        private short tMinuto = 1;
        private short tSegundo = 0;
        private System.Windows.Threading.DispatcherTimer timer;
        #endregion

        private ATENCION_TIPO _SelectTipoAtencion;
        public ATENCION_TIPO SelectTipoAtencion
        {
            get { return _SelectTipoAtencion; }
            set
            {
                //TipoNotaVisible = value > 0 ? Visibility.Visible : Visibility.Collapsed;
                _SelectTipoAtencion = value;
                if (value != null ? value.ID_TIPO_ATENCION != -1 : false)
                {
                    //CargarDatosTipoServicio(value.ID_TIPO_ATENCION);
                }
                OnPropertyChanged("SelectTipoAtencion");
            }
        }

        //private async void ValidarTipoNota(short value)
        //{
        //    SwitchTipoNota(value);
        //    if (value != -1)
        //    {
        //        switch (value)
        //        {
        //            case (short)enumTipoNota.NOTA_URGENCIA:
        //            case (short)enumTipoNota.NOTA_INTERCONSULTA:
        //            //case (short)enumTipoNota.NOTA_EVOLUCION:
        //            case (short)enumTipoNota.NOTA_PREANESTESICA:
        //            case (short)enumTipoNota.NOTA_PREOPERATORIA:
        //            case (short)enumTipoNota.NOTA_TRASLADO:
        //            case (short)enumTipoNota.NOTA_POSTOPERAORIO:
        //                TipoNotaSeleccionada = true;
        //                _SelectTipoAtencion = value;
        //                break;
        //            default:
        //                await System.Threading.Tasks.TaskEx.Delay(150);
        //                TipoNotaSeleccionada = false;
        //                SelectTipoAtencion = -1;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        TipoNotaSeleccionada = false;
        //        _SelectTipoAtencion = value;
        //    }
        //}
        //private void SwitchTipoNota(short value)
        //{
        //    HideNotas();
        //    switch (value)
        //    {
        //        case (short)enumTipoNota.NOTA_URGENCIA:
        //            UrgenciaVisible = Visibility.Visible;
        //            break;
        //        case (short)enumTipoNota.NOTA_EVOLUCION:
        //            EvolucionVisible = Visibility.Visible;
        //            break;
        //        case (short)enumTipoNota.NOTA_INTERCONSULTA:
        //            InterconsultaVisible = Visibility.Visible;
        //            break;
        //        case (short)enumTipoNota.NOTA_POSTOPERAORIO:
        //            PostOpVisible = Visibility.Visible;
        //            break;
        //        case (short)enumTipoNota.NOTA_PREANESTESICA:
        //            PreAnestVisible = Visibility.Visible;
        //            break;
        //        case (short)enumTipoNota.NOTA_PREOPERATORIA:
        //            PreOpVisible = Visibility.Visible;
        //            break;
        //        case (short)enumTipoNota.NOTA_TRASLADO:
        //            TrasladoVisible = Visibility.Visible;
        //            break;
        //        default:
        //            break;
        //    }
        //}

        private Visibility _ExploracionFisicaVisible = Visibility.Collapsed;
        public Visibility ExploracionFisicaVisible
        {
            get { return _ExploracionFisicaVisible; }
            set { _ExploracionFisicaVisible = value; OnPropertyChanged("ExploracionFisicaVisible"); }
        }
        private Visibility _BotonRegresarVisible = Visibility.Collapsed;
        public Visibility BotonRegresarVisible
        {
            get { return _BotonRegresarVisible; }
            set { _BotonRegresarVisible = value; OnPropertyChanged("BotonRegresarVisible"); }
        }
        private Visibility _CertificadoMedicoIngresoVisible = Visibility.Collapsed;
        public Visibility CertificadoMedicoIngresoVisible
        {
            get { return _CertificadoMedicoIngresoVisible; }
            set { _CertificadoMedicoIngresoVisible = value; OnPropertyChanged("CertificadoMedicoIngresoVisible"); }
        }
        private Visibility _TabMedicoVisibles = Visibility.Collapsed;
        public Visibility TabMedicoVisibles
        {
            get { return _TabMedicoVisibles; }
            set { _TabMedicoVisibles = value; OnPropertyChanged("TabMedicoVisibles"); }
        }

        #region DATOS IMPUTADO
        private int? _TextAnioImputado;
        public int? TextAnioImputado
        {
            get { return _TextAnioImputado; }
            set { _TextAnioImputado = value; OnPropertyChanged("TextAnioImputado"); }
        }
        private int? _TextFolioImputado;
        public int? TextFolioImputado
        {
            get { return _TextFolioImputado; }
            set { _TextFolioImputado = value; OnPropertyChanged("TextFolioImputado"); }
        }
        private string _TextPaternoImputado;
        public string TextPaternoImputado
        {
            get { return _TextPaternoImputado; }
            set { _TextPaternoImputado = value; OnPropertyChanged("TextPaternoImputado"); }
        }
        private string _TextMaternoImputado;
        public string TextMaternoImputado
        {
            get { return _TextMaternoImputado; }
            set { _TextMaternoImputado = value; OnPropertyChanged("TextMaternoImputado"); }
        }
        private string _TextNombreImputado;
        public string TextNombreImputado
        {
            get { return _TextNombreImputado; }
            set { _TextNombreImputado = value; OnPropertyChanged("TextNombreImputado"); }
        }
        private byte[] _FotoIngreso = new Imagenes().getImagenPerson();
        public byte[] FotoIngreso
        {
            get { return _FotoIngreso; }
            set { _FotoIngreso = value; OnPropertyChanged("FotoIngreso"); }
        }
        private string _TextFecha;
        public string TextFechaRegistro
        {
            get { return _TextFecha; }
            set { _TextFecha = value; OnPropertyChanged("TextFechaRegistro"); }
        }
        private string _TextHora;
        public string TextHoraRegistro
        {
            get { return _TextHora; }
            set { _TextHora = value; OnPropertyChanged("TextHoraRegistro"); }
        }
        #endregion

        private Visibility _UrgenciaVisible = Visibility.Collapsed;
        public Visibility UrgenciaVisible
        {
            get { return _UrgenciaVisible; }
            set { _UrgenciaVisible = value; OnPropertyChanged("UrgenciaVisible"); }
        }
        private Visibility _EvolucionVisible = Visibility.Collapsed;
        public Visibility EvolucionVisible
        {
            get { return _EvolucionVisible; }
            set { _EvolucionVisible = value; OnPropertyChanged("EvolucionVisible"); }
        }
        private Visibility _TrasladoVisible = Visibility.Collapsed;
        public Visibility TrasladoVisible
        {
            get { return _TrasladoVisible; }
            set { _TrasladoVisible = value; OnPropertyChanged("TrasladoVisible"); }
        }
        private Visibility _InterconsultaVisible = Visibility.Collapsed;
        public Visibility InterconsultaVisible
        {
            get { return _InterconsultaVisible; }
            set { _InterconsultaVisible = value; OnPropertyChanged("InterconsultaVisible"); }
        }
        private Visibility _PreOpVisible = Visibility.Collapsed;
        public Visibility PreOpVisible
        {
            get { return _PreOpVisible; }
            set { _PreOpVisible = value; OnPropertyChanged("PreOpVisible"); }
        }
        private Visibility _PreAnestVisible = Visibility.Collapsed;
        public Visibility PreAnestVisible
        {
            get { return _PreAnestVisible; }
            set { _PreAnestVisible = value; OnPropertyChanged("PreAnestVisible"); }
        }
        private Visibility _PostOpVisible = Visibility.Collapsed;
        public Visibility PostOpVisible
        {
            get { return _PostOpVisible; }
            set { _PostOpVisible = value; OnPropertyChanged("PostOpVisible"); }
        }
        private ATENCION_CITA SelectedCita;
        private string _TextBuscarConsultaMedica;
        public string TextBuscarConsultaMedica
        {
            get { return _TextBuscarConsultaMedica; }
            set { _TextBuscarConsultaMedica = value; OnPropertyChanged("TextBuscarConsultaMedica"); }
        }
        private ObservableCollection<ATENCION_CITA> ListConsultasMedicasAux;
        private ObservableCollection<ATENCION_CITA> _ListConsultasMedicas;
        public ObservableCollection<ATENCION_CITA> ListConsultasMedicas
        {
            get { return _ListConsultasMedicas; }
            set { _ListConsultasMedicas = value; OnPropertyChanged("ListConsultasMedicas"); }
        }
        private ATENCION_CITA _SelectConsultaMedica;
        public ATENCION_CITA SelectConsultaMedica
        {
            get { return _SelectConsultaMedica; }
            set { _SelectConsultaMedica = value; OnPropertyChanged("SelectConsultaMedica"); }
        }
        private bool _EmptyBuscarConsultasMedicasVisible = false;
        public bool EmptyBuscarConsultasMedicasVisible
        {
            get { return _EmptyBuscarConsultasMedicasVisible; }
            set { _EmptyBuscarConsultasMedicasVisible = value; OnPropertyChanged("EmptyBuscarConsultasMedicasVisible"); }
        }
        private CertificadoMedicoIngresoView CertificadoView;
        private ObservableCollection<PRODUCTO_UNIDAD_MEDIDA> _ListUnidadMedida;
        public ObservableCollection<PRODUCTO_UNIDAD_MEDIDA> ListUnidadMedida
        {
            get { return _ListUnidadMedida; }
            set { _ListUnidadMedida = value; OnPropertyChanged("ListUnidadMedida"); }
        }
        private ObservableCollection<DietaMedica> _ListDietasAux;
        public ObservableCollection<DietaMedica> ListDietasAux
        {
            get { return _ListDietasAux; }
            set { _ListDietasAux = value; OnPropertyChanged("ListDietasAux"); }
        }
        private ObservableCollection<DietaMedica> _ListDietas;
        public ObservableCollection<DietaMedica> ListDietas
        {
            get { return _ListDietas; }
            set { _ListDietas = value; OnPropertyChanged("ListDietas"); }
        }

        #region ENFERMEDAD
        private bool _RecetaEnabled = true;
        public bool RecetaEnabled
        {
            get { return _RecetaEnabled; }
            set { _RecetaEnabled = value; OnPropertyChanged("RecetaEnabled"); }
        }
        private ControlPenales.Controls.AutoCompleteTextBox _AutoCompleteProcsMeds;
        public ControlPenales.Controls.AutoCompleteTextBox AutoCompleteProcsMeds
        {
            get { return _AutoCompleteProcsMeds; }
            set { _AutoCompleteProcsMeds = value; }
        }
        private ControlPenales.Controls.AutoCompleteTextBox _AutoCompleteReceta;
        public ControlPenales.Controls.AutoCompleteTextBox AutoCompleteReceta
        {
            get { return _AutoCompleteReceta; }
            set { _AutoCompleteReceta = value; }
        }
        private ControlPenales.Controls.AutoCompleteTextBox _AutoCompleteTB;
        public ControlPenales.Controls.AutoCompleteTextBox AutoCompleteTB
        {
            get { return _AutoCompleteTB; }
            set { _AutoCompleteTB = value; }
        }
        private RecetaMedica _SelectReceta;
        public RecetaMedica SelectReceta
        {
            get { return _SelectReceta; }
            set { _SelectReceta = value; OnPropertyChanged("SelectReceta"); }
        }
        private List<RecetaMedica> ListRecetasCanceladas;
        private List<RecetaMedica> ListRecetasPorAgregar;
        private ObservableCollection<RecetaMedica> _ListRecetas;
        public ObservableCollection<RecetaMedica> ListRecetas
        {
            get { return _ListRecetas; }
            set { _ListRecetas = value; OnPropertyChanged("ListRecetas"); }
        }
        private ListBox _AutoCompleteProcsMedsLB;
        public ListBox AutoCompleteProcsMedsLB
        {
            get { return _AutoCompleteProcsMedsLB; }
            set { _AutoCompleteProcsMedsLB = value; }
        }
        private ListBox _AutoCompleteRecetaLB;
        public ListBox AutoCompleteRecetaLB
        {
            get { return _AutoCompleteRecetaLB; }
            set { _AutoCompleteRecetaLB = value; }
        }
        private ListBox _AutoComplete;
        public ListBox AutoCompleteLB
        {
            get { return _AutoComplete; }
            set { _AutoComplete = value; }
        }
        private ObservableCollection<NOTA_MEDICA_ENFERMEDAD> _ListEnfermedades;
        public ObservableCollection<NOTA_MEDICA_ENFERMEDAD> ListEnfermedades
        {
            get { return _ListEnfermedades; }
            set { _ListEnfermedades = value; OnPropertyChanged("ListEnfermedades"); }
        }
        private NOTA_MEDICA_ENFERMEDAD _SelectEnfermedad;
        public NOTA_MEDICA_ENFERMEDAD SelectEnfermedad
        {
            get { return _SelectEnfermedad; }
            set { _SelectEnfermedad = value; OnPropertyChanged("SelectEnfermedad"); }
        }
        private ObservableCollection<PatologicosMedicos> lstPatologicos;
        public ObservableCollection<PatologicosMedicos> LstPatologicos
        {
            get { return lstPatologicos; }
            set { lstPatologicos = value; OnPropertyChanged("LstPatologicos"); }
        }
        private PatologicosMedicos _SelectedPatologico;
        public PatologicosMedicos SelectedPatologico
        {
            get { return _SelectedPatologico; }
            set { _SelectedPatologico = value; OnPropertyChanged("SelectedPatologico"); }
        }
        private string _TituloHeaderExpandirDescripcion;
        public string TituloHeaderExpandirDescripcion
        {
            get { return _TituloHeaderExpandirDescripcion; }
            set { _TituloHeaderExpandirDescripcion = value; OnPropertyChanged("TituloHeaderExpandirDescripcion"); }
        }
        private string _TextAmpliarDescripcion;
        public string TextAmpliarDescripcion
        {
            get { return _TextAmpliarDescripcion; }
            set { _TextAmpliarDescripcion = value; OnPropertyChanged("TextAmpliarDescripcion"); }
        }
        private bool RecetaHistoria;
        private bool _HistoriaClinicaEnabled = false;
        public bool HistoriaClinicaEnabled
        {
            get { return _HistoriaClinicaEnabled; }
            set { _HistoriaClinicaEnabled = value; OnPropertyChanged("HistoriaClinicaEnabled"); }
        }
        private ObservableCollection<HistoriaclinicaPatologica> ListCondensadoPatologicosAux;
        private ObservableCollection<HistoriaclinicaPatologica> lstCondensadoPatologicos;
        public ObservableCollection<HistoriaclinicaPatologica> LstCondensadoPatologicos
        {
            get { return lstCondensadoPatologicos; }
            set { lstCondensadoPatologicos = value; OnPropertyChanged("LstCondensadoPatologicos"); }
        }
        private HistoriaclinicaPatologica _SelectedCondensadoPato;
        public HistoriaclinicaPatologica SelectedCondensadoPato
        {
            get { return _SelectedCondensadoPato; }
            set { _SelectedCondensadoPato = value; OnPropertyChanged("SelectedCondensadoPato"); }
        }
        #endregion

        #region CERTIFICADO
        private Visibility _ObservacionesTratamientoVisible;
        public Visibility ObservacionesTratamientoVisible
        {
            get { return _ObservacionesTratamientoVisible; }
            set { _ObservacionesTratamientoVisible = value; OnPropertyChanged("ObservacionesTratamientoVisible"); }
        }
        private List<RadioButton> _ListRadioButonsDorso;
        public List<RadioButton> ListRadioButonsDorso
        {
            get { return _ListRadioButonsDorso; }
            set { _ListRadioButonsDorso = value; }
        }
        private List<RadioButton> _ListRadioButonsFrente;
        public List<RadioButton> ListRadioButonsFrente
        {
            get { return _ListRadioButonsFrente; }
            set { _ListRadioButonsFrente = value; }
        }
        private string _TextAntecedentesPatologicos;
        public string TextAntecedentesPatologicos
        {
            get { return _TextAntecedentesPatologicos; }
            set { _TextAntecedentesPatologicos = value; OnPropertyValidateChanged("TextAntecedentesPatologicos"); }
        }
        private string _TextToxicomanias;
        public string TextToxicomanias
        {
            get { return _TextToxicomanias; }
            set { _TextToxicomanias = value; OnPropertyValidateChanged("TextToxicomanias"); }
        }
        private string _TextPadecimientoYTratamientoActual;
        public string TextPadecimientoYTratamientoActual
        {
            get { return _TextPadecimientoYTratamientoActual; }
            set { _TextPadecimientoYTratamientoActual = value; OnPropertyValidateChanged("TextPadecimientoYTratamientoActual"); }
        }
        private string _TextSeDetecto;
        public string TextSeDetecto
        {
            get { return _TextSeDetecto; }
            set { _TextSeDetecto = value; OnPropertyValidateChanged("TextSeDetecto"); }
        }
        private string _TextDiagnosticoCertificado;
        public string TextDiagnosticoCertificado
        {
            get { return _TextDiagnosticoCertificado; }
            set { _TextDiagnosticoCertificado = value; OnPropertyValidateChanged("TextDiagnosticoCertificado"); }
        }
        private string _TextPlanTerapeuticoCertificado;
        public string TextPlanTerapeuticoCertificado
        {
            get { return _TextPlanTerapeuticoCertificado; }
            set { _TextPlanTerapeuticoCertificado = value; OnPropertyValidateChanged("TextPlanTerapeuticoCertificado"); }
        }
        private string _TextObservacionesConclusionesCertificado;
        public string TextObservacionesConclusionesCertificado
        {
            get { return _TextObservacionesConclusionesCertificado; }
            set { _TextObservacionesConclusionesCertificado = value; OnPropertyValidateChanged("TextObservacionesConclusionesCertificado"); }
        }
        private bool _CheckedToxicomanias;
        public bool CheckedToxicomanias
        {
            get { return _CheckedToxicomanias; }
            set { _CheckedToxicomanias = value; OnPropertyValidateChanged("CheckedToxicomanias"); }
        }
        private bool _SeguimientoEnabled;
        public bool SeguimientoEnabled
        {
            get { return _SeguimientoEnabled; }
            set { _SeguimientoEnabled = value; OnPropertyChanged("SeguimientoEnabled"); }
        }
        private bool _CheckedSeguimiento = false;
        public bool CheckedSeguimiento
        {
            get { return _CheckedSeguimiento; }
            set
            {
                _CheckedSeguimiento = value;
                OnPropertyValidateChanged("CheckedSeguimiento");
                SeguimientoEnabled = value;
                StrFechaSeguimiento = string.Empty;
                AtencionCitaSeguimiento = null;
            }
        }
        private bool _CheckedHospitalizacion;
        public bool CheckedHospitalizacion
        {
            get { return _CheckedHospitalizacion; }
            set { _CheckedHospitalizacion = value; OnPropertyValidateChanged("CheckedHospitalizacion"); }
        }
        private bool _CheckedPeligroVida;
        public bool CheckedPeligroVida
        {
            get { return _CheckedPeligroVida; }
            set { _CheckedPeligroVida = value; OnPropertyValidateChanged("CheckedPeligroVida"); }
        }
        private bool _Checked15DiasSanar;
        public bool Checked15DiasSanar
        {
            get { return _Checked15DiasSanar; }
            set { _Checked15DiasSanar = value; OnPropertyValidateChanged("Checked15DiasSanar"); }
        }
        private bool _TabFrente;
        public bool TabFrente
        {
            get { return _TabFrente; }
            set { _TabFrente = value; OnPropertyChanged("TabFrente"); }
        }
        private bool _TabDorso;
        public bool TabDorso
        {
            get { return _TabDorso; }
            set { _TabDorso = value; OnPropertyChanged("TabDorso"); }
        }

        #region LESIONES
        private bool _BotonLesionEnabled;
        public bool BotonLesionEnabled
        {
            get { return _BotonLesionEnabled; }
            set { _BotonLesionEnabled = value; OnPropertyChanged("BotonLesionEnabled"); }
        }
        private short? _SelectRegion;
        public short? SelectRegion
        {
            get { return _SelectRegion; }
            set
            {
                _SelectRegion = value;
                BotonLesionEnabled = !string.IsNullOrEmpty(TextDescripcionLesion) && (value.HasValue ? value.Value > 0 : false);
                OnPropertyChanged("SelectRegion");
            }
        }
        private ObservableCollection<LesionesCustom> _ListLesiones;
        public ObservableCollection<LesionesCustom> ListLesiones
        {
            get { return _ListLesiones; }
            set { _ListLesiones = value; OnPropertyChanged("ListLesiones"); }
        }
        private LesionesCustom _SelectLesion;
        public LesionesCustom SelectLesion
        {
            get { return _SelectLesion; }
            set { _SelectLesion = value; OnPropertyChanged("SelectLesion"); }
        }
        private LesionesCustom _SelectLesionEliminar;
        public LesionesCustom SelectLesionEliminar
        {
            get { return _SelectLesionEliminar; }
            set { _SelectLesionEliminar = value; OnPropertyChanged("SelectLesionEliminar"); }
        }
        private string _TextDescripcionLesion;
        public string TextDescripcionLesion
        {
            get { return _TextDescripcionLesion; }
            set
            {
                _TextDescripcionLesion = value;
                BotonLesionEnabled = !string.IsNullOrEmpty(value) && (SelectRegion.HasValue ? SelectRegion.Value > 0 : false);
                OnPropertyChanged("TextDescripcionLesion");
            }
        }
        #endregion

        #endregion

        #region BUSQUEDA HUELLA
        private string _TextNIPCartaCompromiso;
        public string TextNIPCartaCompromiso
        {
            get { return _TextNIPCartaCompromiso; }
            set { _TextNIPCartaCompromiso = value; OnPropertyChanged("TextNIPCartaCompromiso"); }
        }
        private CartaCompromisoAceptaView CartaView;
        private bool HuellaCompromiso = false;
        private bool CartaCompromiso = false;
        private bool Conectado;
        public enumTipoPersona BuscarPor = enumTipoPersona.IMPUTADO;
        private Visibility _ShowCapturar = Visibility.Collapsed;
        public Visibility ShowCapturar
        {
            get { return _ShowCapturar; }
            set
            {
                _ShowCapturar = value;
                OnPropertyChanged("ShowCapturar");
            }
        }
        private Visibility _ShowContinuar = Visibility.Collapsed;
        public Visibility ShowContinuar
        {
            get { return _ShowContinuar; }
            set
            {
                _ShowContinuar = value;
                OnPropertyChanged("ShowContinuar");
            }
        }
        private Visibility _ShowLoading = Visibility.Collapsed;
        public Visibility ShowLoading
        {
            get { return _ShowLoading; }
            set
            {
                _ShowLoading = value;
                OnPropertyChanged("ShowLoading");
            }
        }
        private bool isKeepSearching { get; set; }
        private bool GuardandoHuellas { get; set; }
        private bool CancelKeepSearching { get; set; }
        private bool _GuardarHuellas { get; set; }
        public IList<PlantillaBiometrico> HuellasCapturadas { get; set; }
        private string _CabeceraBusqueda;
        public string CabeceraBusqueda
        {
            get { return _CabeceraBusqueda; }
            set
            {
                _CabeceraBusqueda = value;
                OnPropertyChanged("CabeceraBusqueda");
            }
        }
        private string _CabeceraFoto;
        public string CabeceraFoto
        {
            get { return _CabeceraFoto; }
            set
            {
                _CabeceraFoto = value;
                OnPropertyChanged("CabeceraFoto");
            }
        }
        private System.Windows.Media.Brush _ColorMessage;
        public System.Windows.Media.Brush ColorMessage
        {
            get { return _ColorMessage; }
            set
            {
                _ColorMessage = value;
                RaisePropertyChanged("ColorMessage");
            }
        }
        private enumTipoBiometrico? _DD_Dedo = enumTipoBiometrico.INDICE_DERECHO;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set
            {
                LimpiarCampos();
                _DD_Dedo = value;
                OnPropertyChanged("DD_Dedo");
            }
        }
        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set
            {
                aceptarBusquedaHuellaFocus = value;
                OnPropertyChanged("AceptarBusquedaHuellaFocus");
            }
        }
        private IList<ResultadoBusquedaBiometrico> _ListResultado;
        public IList<ResultadoBusquedaBiometrico> ListResultado
        {
            get { return _ListResultado; }
            set
            {
                _ListResultado = value;
                var bk = SelectRegistro;
                OnPropertyChanged("ListResultado");
                if (CancelKeepSearching)
                    SelectRegistro = bk;
            }
        }
        private ResultadoBusquedaBiometrico _SelectRegistro;
        public ResultadoBusquedaBiometrico SelectRegistro
        {
            get { return _SelectRegistro; }
            set
            {
                _SelectRegistro = value;
                FotoRegistro = value == null ? new Imagenes().getImagenPerson() : new Imagenes().ConvertBitmapToByte((BitmapSource)value.Foto);
                OnPropertyChanged("SelectRegistro");
            }
        }
        private byte[] _FotoRegistro = new Imagenes().getImagenPerson();
        public byte[] FotoRegistro
        {
            get { return _FotoRegistro; }
            set { _FotoRegistro = value; OnPropertyChanged("FotoRegistro"); }
        }
        private string _TextNipBusqueda;
        public string TextNipBusqueda
        {
            get { return _TextNipBusqueda; }
            set { _TextNipBusqueda = value; OnPropertyChanged("TextNipBusqueda"); }
        }
        #endregion

        #region LISTA DE IMPUTADOS
        private bool CitaNoExistente = false;
        private bool RetornoExcarcelacion = false;
        private bool AtencionSeleccionada = false;
        private ATENCION_CITA _SelectProcedimientoMedicoImputado;
        public ATENCION_CITA SelectProcedimientoMedicoImputado
        {
            get { return _SelectProcedimientoMedicoImputado; }
            set { _SelectProcedimientoMedicoImputado = value; OnPropertyChanged("SelectProcedimientoMedicoImputado"); }
        }
        private ATENCION_MEDICA _SelectAtencionMedica;
        public ATENCION_MEDICA SelectAtencionMedica
        {
            get { return _SelectAtencionMedica; }
            set { _SelectAtencionMedica = value; }
        }
        private ATENCION_MEDICA SelectAtencionMedicaAux;
        private ATENCION_CITA _SelectCitaMedicaImputado;
        public ATENCION_CITA SelectCitaMedicaImputado
        {
            get { return _SelectCitaMedicaImputado; }
            set { _SelectCitaMedicaImputado = value; }
        }
        private EXCARCELACION SelectExcarcelacionImputado;
        private TRASLADO_DETALLE SelectTrasladoImputado;
        private ATENCION_MEDICA SelectUrgenciaImputado;
        private ATENCION_MEDICA SelectSancionImputado;
        private INGRESO SelectIngresoConsulta;
        //private short _TabImputadosIndex;
        //public short TabImputadosIndex
        //{
        //    get { return _TabImputadosIndex; }
        //    set
        //    {
        //        _TabImputadosIndex = value;
        //        switch (value)
        //        {
        //            case 0:
        //                if (ListCitados != null)
        //                    ListImputados = new ObservableCollection<IMPUTADO>(ListCitados.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO));
        //                break;
        //            case 1:
        //                if (ListExcarcelados != null)
        //                    ListImputados = new ObservableCollection<IMPUTADO>(ListExcarcelados.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO));
        //                break;
        //            case 2:
        //                if (ListTrasladados != null)
        //                    ListImputados = new ObservableCollection<IMPUTADO>(ListTrasladados.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO));
        //                break;
        //            case 3:
        //                if (ListNuevosIngresos != null)
        //                    ListImputados = new ObservableCollection<IMPUTADO>(ListNuevosIngresos.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO));
        //                break;
        //        }
        //        OnPropertyChanged("TabImputadosIndex");
        //    }
        //}
        /*private bool _TabCitasMedicas = false;
        public bool TabCitasMedicas
        {
            get { return _TabCitasMedicas; }
            set
            {
                _TabCitasMedicas = value;
                ListImputados = new ObservableCollection<IMPUTADO>(ListCitados.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO));
                OnPropertyChanged("TabCitasMedicas");
            }
        }
        private bool _TabExcarcelaciones;
        public bool TabExcarcelaciones
        {
            get { return _TabExcarcelaciones; }
            set
            {
                _TabExcarcelaciones = value;
                ListImputados = new ObservableCollection<IMPUTADO>(ListExcarcelados.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO));
                OnPropertyChanged("TabExcarcelaciones");
            }
        }
        private bool _TabTraslados;
        public bool TabTraslados
        {
            get { return _TabTraslados; }
            set
            {
                _TabTraslados = value;
                ListImputados = new ObservableCollection<IMPUTADO>(ListTrasladados.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO));
                OnPropertyChanged("TabTraslados");
            }
        }
        private bool _TabNuevoIngreso;
        public bool TabNuevoIngreso
        {
            get { return _TabNuevoIngreso; }
            set
            {
                _TabNuevoIngreso = value;
                ListImputados = new ObservableCollection<IMPUTADO>(ListNuevosIngresos.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO));
                OnPropertyChanged("TabNuevoIngreso");
            }
        }*/
        private ObservableCollection<IMPUTADO> _ListImputados;
        public ObservableCollection<IMPUTADO> ListImputados
        {
            get { return _ListImputados; }
            set { _ListImputados = value; OnPropertyChanged("ListImputados"); }
        }
        private ObservableCollection<HOSPITALIZACION> _ListHospitalizados;
        public ObservableCollection<HOSPITALIZACION> ListHospitalizados
        {
            get { return _ListHospitalizados; }
            set { _ListHospitalizados = value; OnPropertyChanged("ListHospitalizados"); }
        }
        private Visibility _ProcedimientosMedicosListaVisible = Visibility.Visible;
        public Visibility ProcedimientosMedicosListaVisible
        {
            get { return _ProcedimientosMedicosListaVisible; }
            set { _ProcedimientosMedicosListaVisible = value; OnPropertyChanged("ProcedimientosMedicosListaVisible"); }
        }
        private Visibility _CitasVisible = Visibility.Visible;
        public Visibility CitasVisible
        {
            get { return _CitasVisible; }
            set { _CitasVisible = value; OnPropertyChanged("CitasVisible"); }
        }
        private Visibility _UrgenciasVisible = Visibility.Visible;
        public Visibility UrgenciasVisible
        {
            get { return _UrgenciasVisible; }
            set { _UrgenciasVisible = value; OnPropertyChanged("UrgenciasVisible"); }
        }
        private Visibility _SancionesVisible = Visibility.Visible;
        public Visibility SancionesVisible
        {
            get { return _SancionesVisible; }
            set { _SancionesVisible = value; OnPropertyChanged("SancionesVisible"); }
        }
        private Visibility _SignosVitalesVisible = Visibility.Visible;
        public Visibility SignosVitalesVisible
        {
            get { return _SignosVitalesVisible; }
            set { _SignosVitalesVisible = value; OnPropertyChanged("SignosVitalesVisible"); }
        }
        private Visibility _DiagnosticoVisible = Visibility.Visible;
        public Visibility DiagnosticoVisible
        {
            get { return _DiagnosticoVisible; }
            set { _DiagnosticoVisible = value; OnPropertyChanged("DiagnosticoVisible"); }
        }
        private Visibility _TratamientoVisible = Visibility.Visible;
        public Visibility TratamientoVisible
        {
            get { return _TratamientoVisible; }
            set { _TratamientoVisible = value; OnPropertyChanged("TratamientoVisible"); }
        }
        private Visibility _ResumenInterrogatorioVisible = Visibility.Visible;
        public Visibility ResumenInterrogatorioVisible
        {
            get { return _ResumenInterrogatorioVisible; }
            set { _ResumenInterrogatorioVisible = value; }
        }
        private Visibility _EstudiosVisible = Visibility.Visible;
        public Visibility EstudiosVisible
        {
            get { return _EstudiosVisible; }
            set { _EstudiosVisible = value; }
        }
        private Visibility _PlanSeguimientoVisible = Visibility.Visible;
        public Visibility PlanSeguimientoVisible
        {
            get { return _PlanSeguimientoVisible; }
            set { _PlanSeguimientoVisible = value; }
        }
        private Visibility _NotaMedicaVisible = Visibility.Collapsed;
        public Visibility NotaMedicaVisible
        {
            get { return _NotaMedicaVisible; }
            set { _NotaMedicaVisible = value; OnPropertyChanged("NotaMedicaVisible"); }
        }
        private Visibility _ListaImputadosVisible = Visibility.Visible;
        public Visibility ListaImputadosVisible
        {
            get { return _ListaImputadosVisible; }
            set { _ListaImputadosVisible = value; OnPropertyChanged("ListaImputadosVisible"); }
        }
        private System.Timers.Timer aTimer;
        #endregion

        #region MUJERES
        private Visibility _MujeresVisible = Visibility.Collapsed;
        public Visibility MujeresVisible
        {
            get { return _MujeresVisible; }
            set { _MujeresVisible = value; OnPropertyChanged("MujeresVisible"); }
        }
        #endregion

        #region VARIABLES AUXILIARES
        private RECETA_MEDICA RecetaMedica = new RECETA_MEDICA();
        private List<RECETA_MEDICA_DETALLE> RecetaMedicaDetalle = new List<RECETA_MEDICA_DETALLE>();
        private NOTA_SIGNOS_VITALES SignosVitales = new NOTA_SIGNOS_VITALES();
        private NOTA_MEDICA NotaMedica = new NOTA_MEDICA();
        private ATENCION_CITA AtencionCita = new ATENCION_CITA();
        private EXCARCELACION Excarcelacion = new EXCARCELACION();
        private TRASLADO_DETALLE TrasladoDetalle = new TRASLADO_DETALLE();
        private INGRESO NuevoIngreso = new INGRESO();
        private NOTA_URGENCIA NotaUrgencia = new NOTA_URGENCIA();
        private NOTA_INTERCONSULTA NotaInterconsulta = new NOTA_INTERCONSULTA();
        private NOTA_REFERENCIA_TR NotaTraslado = new NOTA_REFERENCIA_TR();
        private NOTA_PRE_OPERATORI NotaPreOperatoria = new NOTA_PRE_OPERATORI();
        private NOTA_PRE_ANESTECIC NotaPreAnestecica = new NOTA_PRE_ANESTECIC();
        private NOTA_POST_OPERATOR NotaPostOperatoria = new NOTA_POST_OPERATOR();
        //private NOTA_EVOLUCION NotaEvolucion = new NOTA_EVOLUCION();
        private List<NOTA_MEDICA_ENFERMEDAD> Enfermedades = new List<NOTA_MEDICA_ENFERMEDAD>();
        private CERTIFICADO_MEDICO certificado = new CERTIFICADO_MEDICO();
        private List<LESION> lesiones = new List<LESION>();
        private List<DIETA> dietas = new List<DIETA>();
        private List<HISTORIA_CLINICA_PATOLOGICOS> patologicos = new List<HISTORIA_CLINICA_PATOLOGICOS>();
        private List<PROC_ATENCION_MEDICA_PROG_INCI> ProcedimientosIncidencias = new List<PROC_ATENCION_MEDICA_PROG_INCI>();
        private ATENCION_MEDICA AtencionMedica = new ATENCION_MEDICA();
        #endregion

        #region AGENDA
        private string _ProcedimientoMedicoPorAgendar;
        public string ProcedimientoMedicoPorAgendar
        {
            get { return _ProcedimientoMedicoPorAgendar; }
            set { _ProcedimientoMedicoPorAgendar = value; OnPropertyChanged("ProcedimientoMedicoPorAgendar"); }
        }
        private string _ProcedimientosMedicosSeleccionadosAgenda;
        public string ProcedimientosMedicosSeleccionadosAgenda
        {
            get { return _ProcedimientosMedicosSeleccionadosAgenda; }
            set { _ProcedimientosMedicosSeleccionadosAgenda = value; OnPropertyChanged("ProcedimientosMedicosSeleccionadosAgenda"); }
        }
        //private ObservableCollection<PROC_MED> _ListProcedimientosMedicosParaAgendar;
        //public ObservableCollection<PROC_MED> ListProcedimientosMedicosParaAgendar
        //{
        //    get { return _ListProcedimientosMedicosParaAgendar; }
        //    set { _ListProcedimientosMedicosParaAgendar = value; OnPropertyChanged("ListProcedimientosMedicosParaAgendar"); }
        //}
        private PROC_MED _SelectProcMedCalendario;
        public PROC_MED SelectProcMedCalendario
        {
            get { return _SelectProcMedCalendario; }
            set { _SelectProcMedCalendario = value; OnPropertyChanged("SelectProcMedCalendario"); }
        }
        private Visibility _AgregarProcedimientoMedicoLayoutVisible = Visibility.Visible;
        public Visibility AgregarProcedimientoMedicoLayoutVisible
        {
            get { return _AgregarProcedimientoMedicoLayoutVisible; }
            set { _AgregarProcedimientoMedicoLayoutVisible = value; OnPropertyChanged("AgregarProcedimientoMedicoLayoutVisible"); }
        }
        private Visibility _AgregarHoraLayoutVisible = Visibility.Collapsed;
        public Visibility AgregarHoraLayoutVisible
        {
            get { return _AgregarHoraLayoutVisible; }
            set { _AgregarHoraLayoutVisible = value; OnPropertyChanged("AgregarHoraLayoutVisible"); }
        }
        private bool _EmpleadosEnAgendaEnabled = false;
        public bool EmpleadosEnAgendaEnabled
        {
            get { return _EmpleadosEnAgendaEnabled; }
            set { _EmpleadosEnAgendaEnabled = value; OnPropertyChanged("EmpleadosEnAgendaEnabled"); }
        }
        private ATENCION_CITA AtencionCitaSeguimiento;
        private AgendarCitaConCalendarioView _AgendaView;
        public AgendarCitaConCalendarioView AgendaView
        {
            get { return _AgendaView; }
            set { _AgendaView = value; }
        }
        private ObservableCollection<Appointment> lstAgenda;
        public ObservableCollection<Appointment> LstAgenda
        {
            get { return lstAgenda; }
            set
            {
                lstAgenda = value;
                OnPropertyChanged("LstAgenda");
            }
        }
        private DateTime? selectedDateCalendar;
        public DateTime? SelectedDateCalendar
        {
            get { return selectedDateCalendar; }
            set { selectedDateCalendar = value; RaisePropertyChanged("SelectedDateCalendar"); }
        }

        private DateTime? selectedDateBusqueda = DateTime.Now;
        public DateTime? SelectedDateBusqueda
        {
            get { return selectedDateBusqueda; }
            set { selectedDateBusqueda = value; RaisePropertyChanged("SelectedDateBusqueda"); }
        }

        private DateTime? fHoy = Fechas.GetFechaDateServer;
        private bool agregarHora = false;
        public bool AgregarHora
        {
            get { return agregarHora; }
            set { agregarHora = value; OnPropertyChanged("AgregarHora"); }
        }
        private DateTime? aFecha;
        public DateTime? AFecha
        {
            get { return aFecha; }
            set
            {
                aFecha = value;
                if (value.HasValue)
                {
                    var hoy = Fechas.GetFechaDateServer;
                    var now = new DateTime(hoy.Year, hoy.Month, hoy.Day);
                    if (value >= now)
                        AFechaValid = true;
                    else
                    {
                        AFechaValid = false;
                        AFechaMensaje = "La fecha a agendar debe ser igual o mayor al dia de hoy.";
                    }
                }
                else
                {
                    AFechaMensaje = "La fecha es requerida..";
                    AFechaValid = false;
                }
                OnPropertyChanged("AFecha");
            }
        }
        private bool aFechaValid = false;
        public bool AFechaValid
        {
            get { return aFechaValid; }
            set { aFechaValid = value; OnPropertyChanged("AFechaValid"); }
        }
        private DateTime? aHoraI;
        public DateTime? AHoraI
        {
            get { return aHoraI; }
            set
            {
                aHoraI = value;
                if (!value.HasValue)
                    AHorasValid = false;
                else
                    if (value >= AHoraF)
                        AHorasValid = false;
                    else
                        AHorasValid = true;
                OnPropertyChanged("AHoraI");
            }
        }
        private DateTime? aHoraF;
        public DateTime? AHoraF
        {
            get { return aHoraF; }
            set
            {
                aHoraF = value;
                if (!value.HasValue)
                    AHorasValid = false;
                else
                    if (!AHoraI.HasValue)
                        AHorasValid = false;
                    else
                        if (value <= AHoraI)
                            AHorasValid = false;
                        else
                            AHorasValid = true;

                OnPropertyChanged("AHoraF");
            }
        }
        private bool aHorasValid = false;
        public bool AHorasValid
        {
            get { return aHorasValid; }
            set { aHorasValid = value; OnPropertyChanged("AHorasValid"); }

        }
        private string aFechaMensaje = "La fecha es requerida.";
        public string AFechaMensaje
        {
            get { return aFechaMensaje; }
            set { aFechaMensaje = value; OnPropertyChanged("AFechaMensaje"); }
        }
        private string mensajeError;
        public string MensajeError
        {
            get { return mensajeError; }
            set
            {
                mensajeError = value;
                OnPropertyChanged("MensajeError");
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Task.Factory.StartNew(async () => { await TaskEx.Delay(4000); MensajeError = string.Empty; });
                }));
            }
        }
        private RangeEnabledObservableCollection<cSolicitudCita> lstSolicitudes;
        public RangeEnabledObservableCollection<cSolicitudCita> LstSolicitudes
        {
            get { return lstSolicitudes; }
            set
            {
                lstSolicitudes = value;
                OnPropertyChanged("LstSolicitudes");
            }
        }
        private Visibility emptySolicitudes = Visibility.Visible;
        public Visibility EmptySolicitudes
        {
            get { return emptySolicitudes; }
            set { emptySolicitudes = value; OnPropertyChanged("EmptySolicitudes"); }
        }
        private short? cAnio;
        public short? CAnio
        {
            get { return cAnio; }
            set { cAnio = value; OnPropertyChanged("CAnio"); }
        }
        private int? cFolio;
        public int? CFolio
        {
            get { return cFolio; }
            set { cFolio = value; OnPropertyChanged("CFolio"); }
        }
        private string cNombre;
        public string CNombre
        {
            get { return cNombre; }
            set { cNombre = value; OnPropertyChanged("CNombre"); }
        }
        private string cPaterno;
        public string CPaterno
        {
            get { return cPaterno; }
            set { cPaterno = value; OnPropertyChanged("CPaterno"); }
        }
        private string cMaterno;
        public string CMaterno
        {
            get { return cMaterno; }
            set { cMaterno = value; OnPropertyChanged("CMaterno"); }
        }
        private DateTime? cFecha;
        public DateTime? CFecha
        {
            get { return cFecha; }
            set { cFecha = value; OnPropertyChanged("CFecha"); }
        }
        private short? cArea = -1;
        public short? CArea
        {
            get { return cArea; }
            set { cArea = value; OnPropertyChanged("CArea"); }
        }
        private short? cEstatus = (short)enumSolicitudCita.SOLICITADA;
        public short? CEstatus
        {
            get { return cEstatus; }
            set { cEstatus = value; OnPropertyChanged("CEstatus"); }
        }
        private Visibility cMenuVisible = Visibility.Collapsed;
        public Visibility CMenuVisible
        {
            get { return cMenuVisible; }
            set { cMenuVisible = value; OnPropertyChanged("CMenuVisible"); }
        }
        private List<short> areas_tecnicas_rol = new List<short>();
        private bool SeguirCargandoIngresos { get; set; }

        private byte[] sImagen = new Imagenes().getImagenPerson();
        public byte[] SImagen
        {
            get { return sImagen; }
            set { sImagen = value; OnPropertyChanged("SImagen"); }
        }
        private List<cUsuarioExtendida> lstEmpleados = new List<cUsuarioExtendida>();
        public List<cUsuarioExtendida> LstEmpleados
        {
            get { return lstEmpleados; }
            set { lstEmpleados = value; OnPropertyChanged("LstEmpleados"); }
        }
        private cUsuarioExtendida selectedEmpleadoValue;
        public cUsuarioExtendida SelectedEmpleadoValue
        {
            get { return selectedEmpleadoValue; }
            set
            {
                BuscarAgenda = (value != null ? selectedEmpleadoValue != null ? selectedEmpleadoValue.ID_EMPLEADO == value.ID_EMPLEADO : false : false);
                selectedEmpleadoValue = value;
                RaisePropertyChanged("SelectedEmpleadoValue");
            }
        }
        private bool isEmpleadoEnabled = true;
        public bool IsEmpleadoEnabled
        {
            get { return isEmpleadoEnabled; }
            set { isEmpleadoEnabled = value; RaisePropertyChanged("IsEmpleadoEnabled"); }
        }
        private cUsuarioExtendida _empleado = null;
        private string tituloAgenda = string.Empty;
        public string TituloAgenda
        {
            get { return tituloAgenda; }
            set { tituloAgenda = value; RaisePropertyChanged("TituloAgenda"); }
        }
        private bool isAgendarCitaEnabled = true;
        public bool IsAgendarCitaEnabled
        {
            get { return isAgendarCitaEnabled; }
            set { isAgendarCitaEnabled = value; RaisePropertyChanged("IsAgendarCitaEnabled"); }
        }
        private cSolicitudCita selectedSolicitud;
        public cSolicitudCita SelectedSolicitud
        {
            get { return selectedSolicitud; }
            set
            {
                selectedSolicitud = value;
                if (value != null)
                {
                    if (value.ATENCION_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        SImagen = value.ATENCION_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        SImagen = new Imagenes().getImagenPerson();

                    if (value.ATENCION_INGRESO.ESTATUS == (short)enumSolicitudCita.SOLICITADA)
                        CMenuVisible = Visibility.Visible;
                    else
                        CMenuVisible = Visibility.Collapsed;
                }
                else
                {
                    SImagen = new Imagenes().getImagenPerson();
                    CMenuVisible = Visibility.Collapsed;
                }
                OnPropertyChanged("SelectedSolicitud");
            }
        }

        private string strFechaSeguimiento = string.Empty;
        public string StrFechaSeguimiento
        {
            get { return strFechaSeguimiento; }
            set { strFechaSeguimiento = value; RaisePropertyChanged("StrFechaSeguimiento"); }
        }
        #endregion

        #region DENTAL
        private ObservableCollection<DENTAL_TIPO_TRATAMIENTO> _ListTipoTratamiento;
        public ObservableCollection<DENTAL_TIPO_TRATAMIENTO> ListTipoTratamiento
        {
            get { return _ListTipoTratamiento; }
            set { _ListTipoTratamiento = value; OnPropertyChanged("ListTipoTratamiento"); }
        }
        private DENTAL_TIPO_TRATAMIENTO _SelectTipoTratamiento;
        public DENTAL_TIPO_TRATAMIENTO SelectTipoTratamiento
        {
            get { return _SelectTipoTratamiento; }
            set
            {
                _SelectTipoTratamiento = value;
                if (value != null ? value.ID_TIPO_TRATA > 0 : false)
                    ListTratamientoOdonto = new ObservableCollection<DENTAL_TRATAMIENTO>(new cTratamientoDental().ObtenerXTipo(value.ID_TIPO_TRATA));
                else
                    ListTratamientoOdonto = new ObservableCollection<DENTAL_TRATAMIENTO>();
                ListTratamientoOdonto.Insert(0, new DENTAL_TRATAMIENTO { ID_TRATA = -1, DESCR = "SELECCIONE" });
                SelectTratamientoOdonto = ListTratamientoOdonto.First(f => f.ID_TRATA == -1);
                OnPropertyChanged("SelectTipoTratamiento");
            }
        }
        private ObservableCollection<DENTAL_TRATAMIENTO> _ListTratamientoOdonto;
        public ObservableCollection<DENTAL_TRATAMIENTO> ListTratamientoOdonto
        {
            get { return _ListTratamientoOdonto; }
            set { _ListTratamientoOdonto = value; OnPropertyChanged("ListTratamientoOdonto"); }
        }
        private DENTAL_TRATAMIENTO _SelectTratamientoOdonto;
        public DENTAL_TRATAMIENTO SelectTratamientoOdonto
        {
            get { return _SelectTratamientoOdonto; }
            set { _SelectTratamientoOdonto = value; OnPropertyChanged("SelectTratamientoOdonto"); }
        }
        private Visibility _TratamientoDentalVisible = Visibility.Collapsed;
        public Visibility TratamientoDentalVisible
        {
            get { return _TratamientoDentalVisible; }
            set { _TratamientoDentalVisible = value; OnPropertyChanged("TratamientoDentalVisible"); }
        }
        private bool _TextDentalEnable = true;
        public bool TextDentalEnable
        {
            get { return _TextDentalEnable; }
            set { _TextDentalEnable = value; OnPropertyChanged("TextDentalEnable"); }
        }
        private List<CustomOdontograma> ListOdontograma;
        private string _TextTratamientoDental;
        public string TextTratamientoDental
        {
            get { return _TextTratamientoDental; }
            set { _TextTratamientoDental = value; OnPropertyChanged("TextTratamientoDental"); }
        }
        private CustomTratamientoDental _SelectTratamientoDental;
        public CustomTratamientoDental SelectTratamientoDental
        {
            get { return _SelectTratamientoDental; }
            set { _SelectTratamientoDental = value; OnPropertyChanged("SelectTratamientoDental"); }
        }
        private List<CustomTratamientoDental> _ListTratamientoDental;
        public List<CustomTratamientoDental> ListTratamientoDental
        {
            get { return _ListTratamientoDental; }
            set { _ListTratamientoDental = value; OnPropertyChanged("ListTratamientoDental"); }
        }
        private List<CheckBox> ListCheckBoxOdontograma;
        #endregion

        #region MEDICO
        private Visibility _CambiarMedicoVisible = Visibility.Collapsed;
        public Visibility CambiarMedicoVisible
        {
            get { return _CambiarMedicoVisible; }
            set { _CambiarMedicoVisible = value; OnPropertyChanged("CambiarMedicoVisible"); }
        }
        private bool _CheckMedicoEnable = false;
        public bool CheckMedicoEnable
        {
            get { return _CheckMedicoEnable; }
            set { _CheckMedicoEnable = value; OnPropertyChanged("CheckMedicoEnable"); }
        }
        private bool _MedicoEnabled = false;
        public bool MedicoEnabled
        {
            get { return _MedicoEnabled; }
            set { _MedicoEnabled = value; OnPropertyChanged("MedicoEnabled"); }
        }
        private ObservableCollection<EMPLEADO> _ListMedicos;
        public ObservableCollection<EMPLEADO> ListMedicos
        {
            get { return _ListMedicos; }
            set { _ListMedicos = value; OnPropertyChanged("ListMedicos"); }
        }
        private EMPLEADO _SelectMedicoItem;
        public EMPLEADO SelectMedicoItem
        {
            get { return _SelectMedicoItem; }
            set { _SelectMedicoItem = value; OnPropertyChanged("SelectMedicoItem"); }
        }
        private int _SelectMedico;
        public int SelectMedico
        {
            get { return _SelectMedico; }
            set { _SelectMedico = value; OnPropertyChanged("SelectMedico"); }
        }
        private bool _CheckCambiarMedico = true;
        public bool CheckCambiarMedico
        {
            get { return _CheckCambiarMedico; }
            set
            {
                _CheckCambiarMedico = value;
                if (CheckMedicoEnable)
                    MedicoEnabled = value;
                if (!value)
                {
                    ListMedicos = ListMedicos ?? new ObservableCollection<EMPLEADO>();
                    if (SelectCitaMedicaImputado != null)
                        SelectMedico = string.IsNullOrEmpty(SelectCitaMedicaImputado.ID_USUARIO) ?
                            ((IsMedico || IsDentista) ? new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData().First(f => f.ID_USUARIO.Contains(GlobalVar.gUsr) ? f.ID_PERSONA.HasValue : false).ID_PERSONA.Value : -1)
                        : ListMedicos.Any(f => f.USUARIO.Any(a => a.ID_USUARIO.Contains(SelectCitaMedicaImputado.ID_USUARIO.Trim()))) ?
                            ListMedicos.First(f => f.USUARIO.Any(a => a.ID_USUARIO.Contains(SelectCitaMedicaImputado.ID_USUARIO.Trim()))).ID_EMPLEADO
                        : ((IsMedico || IsDentista) ? new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData().First(f => f.ID_USUARIO.Contains(GlobalVar.gUsr) ? f.ID_PERSONA.HasValue : false).ID_PERSONA.Value : -1);
                }
                OnPropertyChanged("CheckCambiarMedico");
            }
        }
        #endregion

        private bool _GuardarAgendaEnabled;
        public bool GuardarAgendaEnabled
        {
            get { return _GuardarAgendaEnabled; }
            set { _GuardarAgendaEnabled = value; OnPropertyChanged("GuardarAgendaEnabled"); }
        }
        private bool _SolicitaInterconsultaCheck = false;
        public bool SolicitaInterconsultaCheck
        {
            get { return _SolicitaInterconsultaCheck; }
            set { _SolicitaInterconsultaCheck = value; OnPropertyChanged("SolicitaInterconsultaCheck"); }
        }
        private bool _MenuArchivosEnabled = false;
        public bool MenuArchivosEnabled
        {
            get { return _MenuArchivosEnabled; }
            set { _MenuArchivosEnabled = value; OnPropertyChanged("MenuArchivosEnabled"); }
        }
        private bool _MenuHClinicaEnabled = false;
        public bool MenuHClinicaEnabled
        {
            get { return _MenuHClinicaEnabled; }
            set { _MenuHClinicaEnabled = value; OnPropertyChanged("MenuHClinicaEnabled"); }
        }
        private bool _MenuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return _MenuFichaEnabled; }
            set { _MenuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        private CustomCitasProcedimientosMedicos _SelectProcedimientoMedicoEnCitaEnMemoria;
        public CustomCitasProcedimientosMedicos SelectProcedimientoMedicoEnCitaEnMemoria
        {
            get { return _SelectProcedimientoMedicoEnCitaEnMemoria; }
            set { _SelectProcedimientoMedicoEnCitaEnMemoria = value; OnPropertyChanged("SelectProcedimientoMedicoEnCitaEnMemoria"); }
        }
        private ObservableCollection<CustomCitasProcedimientosMedicos> _ProcedimientosMedicosEnCitaEnMemoria;
        public ObservableCollection<CustomCitasProcedimientosMedicos> ProcedimientosMedicosEnCitaEnMemoria
        {
            get { return _ProcedimientosMedicosEnCitaEnMemoria; }
            set { _ProcedimientosMedicosEnCitaEnMemoria = value; OnPropertyChanged("ProcedimientosMedicosEnCitaEnMemoria"); }
        }

        #region PROCEDIMIENTOS MEDICOS
        private CancelarProcedimientoMedicoView CancelarProcMedWindow;
        private string _TextObservacionesCancelarProcMed;
        public string TextObservacionesCancelarProcMed
        {
            get { return _TextObservacionesCancelarProcMed; }
            set { _TextObservacionesCancelarProcMed = value; OnPropertyChanged("TextObservacionesCancelarProcMed"); }
        }
        private ATENCION_CITA_IN_MOTIVO _SelectMotivoCancelarProcMed;
        public ATENCION_CITA_IN_MOTIVO SelectMotivoCancelarProcMed
        {
            get { return _SelectMotivoCancelarProcMed; }
            set { _SelectMotivoCancelarProcMed = value; OnPropertyChanged("SelectMotivoCancelarProcMed"); }
        }
        private ObservableCollection<ATENCION_CITA_IN_MOTIVO> _ListMotivosCancelarProcMed;
        public ObservableCollection<ATENCION_CITA_IN_MOTIVO> ListMotivosCancelarProcMed
        {
            get { return _ListMotivosCancelarProcMed; }
            set { _ListMotivosCancelarProcMed = value; OnPropertyChanged("ListMotivosCancelarProcMed"); }
        }
        private ObservableCollection<PROC_MED_SUBTIPO> _ListProcedimientoSubtipo;
        public ObservableCollection<PROC_MED_SUBTIPO> ListProcedimientoSubtipo
        {
            get { return _ListProcedimientoSubtipo; }
            set { _ListProcedimientoSubtipo = value; OnPropertyChanged("ListProcedimientoSubtipo"); }
        }
        private ObservableCollection<PROC_MED> ListProcedimientoMedicoHijoAux;
        private ObservableCollection<PROC_MED> _ListProcedimientoMedicoHijo;
        public ObservableCollection<PROC_MED> ListProcedimientoMedicoHijo
        {
            get { return _ListProcedimientoMedicoHijo; }
            set { _ListProcedimientoMedicoHijo = value; OnPropertyChanged("ListProcedimientoMedicoHijo"); }
        }
        private PROC_MED_SUBTIPO _SelectProcedimientoSubtipo;
        public PROC_MED_SUBTIPO SelectProcedimientoSubtipo
        {
            get { return _SelectProcedimientoSubtipo; }
            set
            {
                _SelectProcedimientoSubtipo = value;
                if (value != null)
                {
                    ListProcedimientoMedicoHijo = value.ID_PROCMED_SUBTIPO == -1 ? new ObservableCollection<PROC_MED>() : new ObservableCollection<PROC_MED>(ListProcedimientoMedicoHijoAux.Where(w => w.ID_PROCMED_SUBTIPO == value.ID_PROCMED_SUBTIPO));
                    ListProcedimientoMedicoHijo.Insert(0, new PROC_MED
                    {
                        DESCR = "SELECCIONE",
                        ID_PROCMED = -1,
                    });
                    SelectProcedimientoMedicoHijo = ListProcedimientoMedicoHijo.First(f => f.ID_PROCMED == -1);
                }
                OnPropertyChanged("SelectProcedimientoSubtipo");
            }
        }
        private PROC_MED _SelectProcedimientoMedicoHijo;
        public PROC_MED SelectProcedimientoMedicoHijo
        {
            get { return _SelectProcedimientoMedicoHijo; }
            set { _SelectProcedimientoMedicoHijo = value; OnPropertyChanged("SelectProcedimientoMedicoHijo"); }
        }
        private CustomProcedimientosMedicosCitados _SelectProcedimientoMedicoAgendado;
        public CustomProcedimientosMedicosCitados SelectProcedimientoMedicoAgendado
        {
            get { return _SelectProcedimientoMedicoAgendado; }
            set { _SelectProcedimientoMedicoAgendado = value; OnPropertyChanged("SelectProcedimientoMedicoAgendado"); }
        }
        private Visibility _ProcedimientoMedicoSeleccionadoVisible = Visibility.Collapsed;
        public Visibility ProcedimientoMedicoSeleccionadoVisible
        {
            get { return _ProcedimientoMedicoSeleccionadoVisible; }
            set { _ProcedimientoMedicoSeleccionadoVisible = value; OnPropertyChanged("ProcedimientoMedicoSeleccionadoVisible"); }
        }
        private ObservableCollection<CustomProcedimientosMedicosCitados> _ListProcedimientosMedicosEnCita;
        public ObservableCollection<CustomProcedimientosMedicosCitados> ListProcedimientosMedicosEnCita
        {
            get { return _ListProcedimientosMedicosEnCita; }
            set { _ListProcedimientosMedicosEnCita = value; OnPropertyChanged("ListProcedimientosMedicosEnCita"); }
        }
        private Visibility _ProcedimientosMedicosVisible = Visibility.Collapsed;
        public Visibility ProcedimientosMedicosVisible
        {
            get { return _ProcedimientosMedicosVisible; }
            set { _ProcedimientosMedicosVisible = value; OnPropertyChanged("ProcedimientosMedicosVisible"); }
        }
        private ObservableCollection<USUARIO> _ListEnfermeros;
        public ObservableCollection<USUARIO> ListEnfermeros
        {
            get { return _ListEnfermeros; }
            set { _ListEnfermeros = value; OnPropertyChanged("ListEnfermeros"); }
        }
        private ObservableCollection<PROC_MED> _ListProcedimientosMedicosCalendario;
        public ObservableCollection<PROC_MED> ListProcedimientosMedicosCalendario
        {
            get { return _ListProcedimientosMedicosCalendario; }
            set { _ListProcedimientosMedicosCalendario = value; }
        }
        private ObservableCollection<PROC_MED> _ListProcedimientosMedicos;
        public ObservableCollection<PROC_MED> ListProcedimientosMedicos
        {
            get { return _ListProcedimientosMedicos; }
            set { _ListProcedimientosMedicos = value; OnPropertyChanged("ListProcedimientosMedicos"); }
        }
        private PROC_MED _SelectProcedimientoMedico;
        public PROC_MED SelectProcedimientoMedico
        {
            get { return _SelectProcedimientoMedico; }
            set { _SelectProcedimientoMedico = value; OnPropertyChanged("SelectProcedimientoMedico"); }
        }
        private ObservableCollection<CustomProcedimientosMedicosSeleccionados> _ListProcMedsSeleccionados;
        public ObservableCollection<CustomProcedimientosMedicosSeleccionados> ListProcMedsSeleccionados
        {
            get { return _ListProcMedsSeleccionados; }
            set { _ListProcMedsSeleccionados = value; OnPropertyChanged("ListProcMedsSeleccionados"); }
        }
        private CustomProcedimientosMedicosSeleccionados ProcedimientoMedicoParaAgenda;
        private bool ProcedimientoMedico = false;
        private CustomProcedimientosMedicosSeleccionados _SelectProcMedSeleccionado;
        public CustomProcedimientosMedicosSeleccionados SelectProcMedSeleccionado
        {
            get { return _SelectProcMedSeleccionado; }
            set
            {
                _SelectProcMedSeleccionado = value;
                if (value != null)
                {
                    EliminarProcedimientoMedicoEnabled = !(value.ID_ATENCION_MEDICA.HasValue ? value.ID_CENTRO_UBI.HasValue : false);
                }
                else
                    EliminarProcedimientoMedicoEnabled = false;
                OnPropertyChanged("SelectProcMedSeleccionado");
            }
        }
        #endregion

        #region PARAMETROS
        private string[] ParametroNotaMedicaTipoServicio;
        private short?[] ParametroEstatusInactivos;
        private byte[] ParametroLogoEstado;
        private byte[] ParametroReporteLogo2;
        private byte[] ParametroCuerpoDorso;
        private byte[] ParametroCuerpoFrente;
        private byte[] ParametroImagenZonaCorporal;
        private string ParametroEstatusCitaSinAtender;
        private string ParametroEstatusCitaConcluido;
        private string ParametroEstatusCitaAtendiendo;
        private short ParametroSolicitudAtencionPorMes;
        private bool ParametroGuardarHuellaEnBusquedaPadronVisita;
        #endregion

        #region ARCHIVOS MEDICOS
        private ArchivosMedicosView _ArchivosMedicos;
        public ArchivosMedicosView ArchivosMedicos
        {
            get { return _ArchivosMedicos; }
            set { _ArchivosMedicos = value; }
        }
        private ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT> lstTipoServAux;
        public ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT> LstTipoServAux
        {
            get { return lstTipoServAux; }
            set { lstTipoServAux = value; OnPropertyChanged("LstTipoServAux"); }
        }
        private short _SelectedTipoServAux = -1;
        public short SelectedTipoServAux
        {
            get { return _SelectedTipoServAux; }
            set { _SelectedTipoServAux = value; OnPropertyChanged("SelectedTipoServAux"); }
        }
        private ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT> _LstSubTipoServAux;
        public ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT> LstSubTipoServAux
        {
            get { return _LstSubTipoServAux; }
            set { _LstSubTipoServAux = value; OnPropertyChanged("LstSubTipoServAux"); }
        }
        private short _SelectedSubTipoServAux = -1;
        public short SelectedSubTipoServAux
        {
            get { return _SelectedSubTipoServAux; }
            set { _SelectedSubTipoServAux = value; OnPropertyChanged("SelectedSubTipoServAux"); }
        }
        private ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> lstDiagnosticosPrincipal;
        public ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> LstDiagnosticosPrincipal
        {
            get { return lstDiagnosticosPrincipal; }
            set { lstDiagnosticosPrincipal = value; OnPropertyChanged("LstDiagnosticosPrincipal"); }
        }
        private int _SelectedDiagnPrincipal = -1;
        public int SelectedDiagnPrincipal
        {
            get { return _SelectedDiagnPrincipal; }
            set { _SelectedDiagnPrincipal = value; OnPropertyChanged("SelectedDiagnPrincipal"); }
        }
        private DateTime? _FechaInicioBusquedaResultServ = Fechas.GetFechaDateServer;
        public DateTime? FechaInicioBusquedaResultServ
        {
            get { return _FechaInicioBusquedaResultServ; }
            set
            {
                _FechaInicioBusquedaResultServ = value;
                if (value.HasValue)//avisa ala fecha de fin que ha cambiado
                {
                    FechaFinBusquedaResultServ = value;
                }
                OnPropertyChanged("FechaInicioBusquedaResultServ");
            }
        }
        private DateTime? _FechaFinBusquedaResultServ = Fechas.GetFechaDateServer;
        public DateTime? FechaFinBusquedaResultServ
        {
            get { return _FechaFinBusquedaResultServ; }
            set { _FechaFinBusquedaResultServ = value; OnPropertyChanged("FechaFinBusquedaResultServ"); }
        }
        private CustomGridSinBytes _SelectedResultadoSinArchivo;
        public CustomGridSinBytes SelectedResultadoSinArchivo
        {
            get { return _SelectedResultadoSinArchivo; }
            set { _SelectedResultadoSinArchivo = value; OnPropertyChanged("SelectedResultadoSinArchivo"); }
        }
        private ObservableCollection<CustomGridSinBytes> lstCustomizadaSinArchivos;//Lista que solo carga los datos SIN LOS BYTES (para ahorrar memoria)
        public ObservableCollection<CustomGridSinBytes> LstCustomizadaSinArchivos
        {
            get { return lstCustomizadaSinArchivos; }
            set { lstCustomizadaSinArchivos = value; OnPropertyChanged("LstCustomizadaSinArchivos"); }
        }
        private short selectedSubtipoServAux = -1;
        public short SelectedSubtipoServAux
        {
            get { return selectedSubtipoServAux; }
            set { selectedSubtipoServAux = value; OnPropertyValidateChanged("SelectedSubtipoServAux"); }
        }
        private short? _SelectedIngresoBusquedas;
        public short? SelectedIngresoBusquedas
        {
            get { return _SelectedIngresoBusquedas; }
            set { _SelectedIngresoBusquedas = value; OnPropertyChanged("SelectedIngresoBusquedas"); }
        }
        private bool _EmptyResultados = true;
        public bool EmptyResultados
        {
            get { return _EmptyResultados; }
            set { _EmptyResultados = value; OnPropertyChanged("EmptyResultados"); }
        }
        #endregion

        #region EVOLUCION
        private bool _InterconsultaEnabled;
        public bool InterconsultaEnabled
        {
            get { return _InterconsultaEnabled; }
            set { _InterconsultaEnabled = value; OnPropertyChanged("InterconsultaEnabled"); }
        }
        private bool Preguntando = true;
        private bool _EliminarProcedimientoMedicoEnabled;
        public bool EliminarProcedimientoMedicoEnabled
        {
            get { return _EliminarProcedimientoMedicoEnabled; }
            set
            {
                _EliminarProcedimientoMedicoEnabled = value;
                OnPropertyChanged("EliminarProcedimientoMedicoEnabled");
            }
        }
        private int _LengthResumenInterrogatorio = 500;
        public int LengthResumenInterrogatorio
        {
            get { return _LengthResumenInterrogatorio; }
            set { _LengthResumenInterrogatorio = value; OnPropertyChanged("LengthResumenInterrogatorio"); }
        }
        private int _LengthEstudios = 500;
        public int LengthEstudios
        {
            get { return _LengthEstudios; }
            set { _LengthEstudios = value; OnPropertyChanged("LengthEstudios"); }
        }
        private int _LengthExploracionFisica = 500;
        public int LengthExploracionFisica
        {
            get { return _LengthExploracionFisica; }
            set { _LengthExploracionFisica = value; OnPropertyChanged("LengthExploracionFisica"); }
        }
        private string _LabelExploracionFisica;
        public string LabelExploracionFisica
        {
            get { return _LabelExploracionFisica; }
            set { _LabelExploracionFisica = value; OnPropertyChanged("LabelExploracionFisica"); }
        }
        private string _LabelResumenInterrogatorio;
        public string LabelResumenInterrogatorio
        {
            get { return _LabelResumenInterrogatorio; }
            set { _LabelResumenInterrogatorio = value; OnPropertyChanged("LabelResumenInterrogatorio"); }
        }
        private string _LabelEstudios;
        public string LabelEstudios
        {
            get { return _LabelEstudios; }
            set { _LabelEstudios = value; OnPropertyChanged("LabelEstudios"); }
        }
        private bool NuevaNota;
        private bool AgregarNota;
        private ATENCION_MEDICA NotaEvolucion;
        //private HOSPITALIZACION SelectHospitalizacion;
        private HOSPITALIZACION _SelectHospitalizado;
        public HOSPITALIZACION SelectHospitalizado
        {
            get { return _SelectHospitalizado; }
            set { _SelectHospitalizado = value; OnPropertyChanged("SelectHospitalizado"); }
        }
        private RecetaMedica _SelectRecetaHistorial;
        public RecetaMedica SelectRecetaHistorial
        {
            get { return _SelectRecetaHistorial; }
            set { _SelectRecetaHistorial = value; OnPropertyChanged("SelectRecetaHistorial"); }
        }
        private ObservableCollection<RecetaMedica> _ListHistorialRecetas;
        public ObservableCollection<RecetaMedica> ListHistorialRecetas
        {
            get { return _ListHistorialRecetas; }
            set { _ListHistorialRecetas = value; OnPropertyChanged("ListHistorialRecetas"); }
        }
        private ObservableCollection<NOTA_MEDICA_DIETA> _ListHistorialDietas;
        public ObservableCollection<NOTA_MEDICA_DIETA> ListHistorialDietas
        {
            get { return _ListHistorialDietas; }
            set { _ListHistorialDietas = value; OnPropertyChanged("ListHistorialDietas"); }
        }
        //private CustomProcedimientosMedicosSeleccionados SelectHistorialProcMed;
        private ObservableCollection<CustomProcedimientosMedicosSeleccionados> _ListHistorialProcMeds;
        public ObservableCollection<CustomProcedimientosMedicosSeleccionados> ListHistorialProcMeds
        {
            get { return _ListHistorialProcMeds; }
            set { _ListHistorialProcMeds = value; OnPropertyChanged("ListHistorialProcMeds"); }
        }
        #endregion

        #region MUJERES
        private HISTORIA_CLINICA_GINECO_OBSTRE MujeresAuxiliar;
        private HISTORIA_CLINICA_GINECO_OBSTRE _Mujeres;
        public HISTORIA_CLINICA_GINECO_OBSTRE Mujeres
        {
            get { return _Mujeres; }
            set { _Mujeres = value; OnPropertyChanged("Mujeres"); }
        }
        private bool _MujeresEnabled;
        public bool MujeresEnabled
        {
            get { return _MujeresEnabled; }
            set { _MujeresEnabled = value; OnPropertyChanged("MujeresEnabled"); }
        }
        private bool _MenarquiaEnabled;
        public bool MenarquiaEnabled
        {
            get { return _MenarquiaEnabled; }
            set { _MenarquiaEnabled = value; OnPropertyChanged("MenarquiaEnabled"); }
        }
        private bool _IsEnabledControlP;
        public bool IsEnabledControlP
        {
            get { return _IsEnabledControlP; }
            set { _IsEnabledControlP = value; OnPropertyChanged("IsEnabledControlP"); }
        }
        private bool NotaMujeres;
        private short? _CheckMenarquia; //HISTORIA_CLINICA.MU_MENARQUIA
        public short? CheckMenarquia
        {
            get { return _CheckMenarquia; }
            set { _CheckMenarquia = value; OnPropertyChanged("CheckMenarquia"); }
        }
        private short? _TextEmbarazos; //HC_GINECO.EMABARAZO
        public short? TextEmbarazos
        {
            get { return _TextEmbarazos; }
            set { _TextEmbarazos = value; OnPropertyChanged("TextEmbarazos"); }
        }
        private short? _TextAbortos;//HC_GINECO.PARTO
        public short? TextAbortos
        {
            get { return _TextAbortos; }
            set { _TextAbortos = value; OnPropertyChanged("TextAbortos"); }
        }
        private short? _TextPartos;//HC_GINECO.PARTO
        public short? TextPartos
        {
            get { return _TextPartos; }
            set { _TextPartos = value; OnPropertyChanged("TextPartos"); }
        }
        private short? _TextCesareas; //HC_GINECO.CESAREA
        public short? TextCesareas
        {
            get { return _TextCesareas; }
            set { _TextCesareas = value; OnPropertyChanged("TextCesareas"); }
        }
        private short _ControlPreNatal = -1; //HC_GINECO.CONTROL_PRENATAL // cambiar de tipo de dato
        public short ControlPreNatal
        {
            get { return _ControlPreNatal; }
            set
            {
                _ControlPreNatal = value;
                IsEnabledControlP = false;
                if (value == 0)
                {
                    SelectControlPreNatal = -1;
                    IsEnabledControlP = true;
                }
                else if (value == 1)
                    SelectControlPreNatal = -1;
                OnPropertyChanged("ControlPreNatal");
            }
        }
        private short? _SelectControlPreNatal;
        public short? SelectControlPreNatal
        {
            get { return _SelectControlPreNatal; }
            set { _SelectControlPreNatal = value; OnPropertyChanged("SelectControlPreNatal"); }
        }
        private ObservableCollection<CONTROL_PRENATAL> _ListControlPrenatal;
        public ObservableCollection<CONTROL_PRENATAL> ListControlPrenatal
        {
            get { return _ListControlPrenatal; }
            set { _ListControlPrenatal = value; OnPropertyChanged("ListControlPrenatal"); }
        }
        private string _TextAniosRitmo; //HC_GINECO.ANIOS_RITMO
        public string TextAniosRitmo
        {
            get { return _TextAniosRitmo; }
            set { _TextAniosRitmo = value; OnPropertyChanged("TextAniosRitmo"); }
        }
        private string _TextDeformacionesOrganicas; //HC_GINECO.DEFORMACION
        public string TextDeformacionesOrganicas
        {
            get { return _TextDeformacionesOrganicas; }
            set { _TextDeformacionesOrganicas = value; OnPropertyChanged("TextDeformacionesOrganicas"); }
        }
        private string _TextIntegridadFisica;
        public string TextIntegridadFisica
        {
            get { return _TextIntegridadFisica; }
            set { _TextIntegridadFisica = value; OnPropertyChanged("TextIntegridadFisica"); }
        }
        private DateTime? _FechaUltimaMenstruacion; //HC_GINECO.ULTIMA_MENSTRUACION_FEC
        public DateTime? FechaUltimaMenstruacion
        {
            get { return _FechaUltimaMenstruacion; }
            set { _FechaUltimaMenstruacion = value; OnPropertyChanged("FechaUltimaMenstruacion"); }
        }
        private DateTime? _FechaProbParto; //HC_GINECO.FECHA_PROBABLE_PARTO
        public DateTime? FechaProbParto
        {
            get { return _FechaProbParto; }
            set { _FechaProbParto = value; OnPropertyChanged("FechaProbParto"); }
        }
        #endregion
    }
}
