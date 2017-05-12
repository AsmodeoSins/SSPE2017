using ControlPenales.Clases.ControlProgramas;
using ControlPenales.Clases;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Threading;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Controlador.Catalogo.Justicia;
using ControlPenales.BiometricoServiceReference;
using System.Threading.Tasks;
using ControlPenales.Clases.ControlInternos;
using System.Collections.ObjectModel;
using System.IO;

namespace ControlPenales.ControlInternosEdificio.ViewModel
{
    public class LeerInternosEdificioViewModel : FingerPrintScanner
    {
        #region [VARIABLES]
        private short? IdEdificio;
        private short? IdSector;
        #endregion

        #region [CONSTRUCTOR]
        public LeerInternosEdificioViewModel(enumTipoPersona tipobusqueda, bool? set442 = null, bool GuardarHuellas = false, short? IdEdificio = 0, short? IdSector = 0)
        {
            ListaResultadoRequerido = new List<ResultadoBusquedaBiometrico>();
            BuscarPor = tipobusqueda;
            Conectado = set442.HasValue ? set442.Value : false;
            ShowCapturar = set442.HasValue ? set442.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
            _GuardarHuellas = GuardarHuellas;
            this.IdEdificio = IdEdificio;
            this.IdSector = IdSector;
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            switch (tipobusqueda)
            {
                case enumTipoPersona.IMPUTADO:
                    CabeceraBusqueda = "Datos del Interno";
                    CabeceraFoto = "Foto del Interno";
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region[PARAMETROS]
        private short?[] ParametroEstatusInactivos;
        #endregion

        #region [PROPIEDADES]
        public Imagenes ImagenPlaceHolder { get; set; }
        private bool Conectado;
        private bool _GuardarHuellas { get; set; }
        private string _CabeceraBusqueda;
        public IList<PlantillaBiometrico> HuellasCapturadas { get; set; }

        public string CabeceraBusqueda
        {
            get { return _CabeceraBusqueda; }
            set { _CabeceraBusqueda = value; OnPropertyChanged("CabeceraBusqueda"); }
        }

        private string _cabeceraFoto;
        public string CabeceraFoto
        {
            get { return _cabeceraFoto; }
            set { _cabeceraFoto = value; OnPropertyChanged("CabeceraFoto"); }
        }

        private Visibility _showCapturar = Visibility.Collapsed;
        public Visibility ShowCapturar
        {
            get { return _showCapturar; }
            set { _showCapturar = value; OnPropertyChanged("ShowCapturar"); }
        }

        private bool _modoAsistencia;
        public bool ModoAsistencia
        {
            get { return _modoAsistencia; }
            set { _modoAsistencia = value; OnPropertyChanged("ModoAsistencia"); }
        }

        private Visibility _showLine;
        public Visibility ShowLine
        {
            get { return _showLine; }
            set { _showLine = value; OnPropertyChanged("ShowLine"); }
        }

        private Visibility _showImagenInterno;
        public Visibility ShowImagenInterno
        {
            get { return _showImagenInterno; }
            set { _showImagenInterno = value; OnPropertyChanged("ShowImagenInterno"); }
        }

        //IMAGEN INTERNO
        private byte[] _imagenInterno;
        public byte[] ImagenInterno
        {
            get { return _imagenInterno; }
            set { _imagenInterno = value; OnPropertyChanged("ImagenInterno"); }
        }

        private AREA _area;
        public AREA Area
        {
            get { return _area; }
            set { _area = value; OnPropertyChanged("Area"); }
        }

        private string _checkMark;
        public string MarkCheck
        {
            get { return _checkMark; }
            set { _checkMark = value; OnPropertyChanged("MarkCheck"); }
        }

        private bool _imagenHuellaVisible;
        public bool ImagenHuellaVisible
        {
            get { return _imagenHuellaVisible; }
            set { _imagenHuellaVisible = value; OnPropertyChanged("ImagenHuellaVisible"); }
        }

        private Visibility _capturaNIPVisible;
        public Visibility CapturaNIPVisible
        {
            get { return _capturaNIPVisible; }
            set { _capturaNIPVisible = value; OnPropertyChanged("CapturaNIPVisible"); }
        }

        private string _buscarNIP;
        public string BuscarNIP
        {
            get { return _buscarNIP; }
            set { _buscarNIP = value; OnPropertyChanged("BuscarNIP"); }
        }

        private Visibility _capturaHuellaVisible;
        public Visibility CapturaHuellaVisible
        {
            get { return _capturaHuellaVisible; }
            set { _capturaHuellaVisible = value; OnPropertyChanged("CapturaHuellaVisible"); }
        }

        private string _imagen;
        public string Imagen
        {
            get { return _imagen; }
            set { _imagen = value; OnPropertyChanged("Imagen"); }
        }

        private List<InternosRequeridos> _listaInternos;
        public List<InternosRequeridos> ListaInternos
        {
            get { return _listaInternos; }
            set { _listaInternos = value; OnPropertyChanged("ListaInternos"); }
        }

        private ObservableCollection<InternosAusentes> _listaInternosAusentes;
        public ObservableCollection<InternosAusentes> ListaInternosAusentes
        {
            get { return _listaInternosAusentes; }
            set { _listaInternosAusentes = value; OnPropertyChanged("ListaInternosAusentes"); }
        }

        private Window WindowEnrolamientoInternos { get; set; }

        private ResultadoBusquedaBiometrico _selectRegistro;
        public ResultadoBusquedaBiometrico SelectRegistro
        {
            get { return _selectRegistro; }
            set { _selectRegistro = value; OnPropertyChanged("SelectRegistro"); }
        }

        private bool _isSucceed = false;
        public bool IsSucceed
        {
            get { return _isSucceed; }
            set { _isSucceed = value; OnPropertyChanged("IsSucceed"); }
        }

        private bool LeyendoHuellas { get; set; }

        private System.Windows.Media.Brush _colorAprobacion;
        public System.Windows.Media.Brush ColorAprobacion
        {
            get { return _colorAprobacion; }
            set { _colorAprobacion = value; RaisePropertyChanged("ColorAprobacion"); }
        }

        private System.Windows.Media.Brush _colorAprobacionNIP;
        public System.Windows.Media.Brush ColorNIPAprobacion
        {
            get { return _colorAprobacionNIP; }
            set { _colorAprobacionNIP = value; RaisePropertyChanged("ColorNIPAprobacion"); }
        }

        private List<InternosRequeridos> _listaRequeridosInternos;
        public List<InternosRequeridos> ListaRequeridosInternos
        {
            get { return _listaRequeridosInternos; }
            set { _listaRequeridosInternos = value; RaisePropertyChanged("ListaRequeridosInternos"); }
        }

        //LISTA BIOMETRICA DEL INTERNO
        private List<ResultadoBusquedaBiometrico> _listResultado;
        public List<ResultadoBusquedaBiometrico> ListResultado
        {
            get { return _listResultado; }
            set { _listResultado = value; OnPropertyChanged("ListResultado"); }
        }

        private List<ResultadoBusquedaBiometrico> _listaResultadoRequerido;
        public List<ResultadoBusquedaBiometrico> ListaResultadoRequerido
        {
            get { return _listaResultadoRequerido; }
            set { _listaResultadoRequerido = value; OnPropertyChanged("ListaResultadoRequerido"); }
        }

        private ObservableCollection<InternosAusentes> _listaAusentesInternos;
        public ObservableCollection<InternosAusentes> ListaAusentesInternos
        {
            get { return _listaAusentesInternos; }
            set { _listaAusentesInternos = value; RaisePropertyChanged("ListaAusentesInternos"); }
        }

        public enumTipoPersona BuscarPor { get; set; }

        private enumTipoBiometrico? _dD_Dedo = enumTipoBiometrico.INDICE_DERECHO;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _dD_Dedo; }
            set
            {
                if (value != enumTipoBiometrico.INDICE_DERECHO)
                {
                    LimpiarCampos();
                }
                _dD_Dedo = value;
                OnPropertyChanged("DD_Dedo");
            }
        }

        private bool _aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return _aceptarBusquedaHuellaFocus; }
            set { _aceptarBusquedaHuellaFocus = value; OnPropertyChanged("AceptarBusquedaHuellaFocus"); }
        }

        private System.Windows.Media.Brush _colorMessage;
        public System.Windows.Media.Brush ColorMessage
        {
            get { return _colorMessage; }
            set { _colorMessage = value; RaisePropertyChanged("ColorMessage"); }
        }

        private Visibility _showContinuar = Visibility.Collapsed;
        public Visibility ShowContinuar
        {
            get { return _showContinuar; }
            set { _showContinuar = value; OnPropertyChanged("ShowContinuar"); }
        }

        private Visibility _showLoading = Visibility.Collapsed;
        public Visibility ShowLoading
        {
            get { return _showLoading; }
            set { _showLoading = value; OnPropertyChanged("ShowLoading"); }
        }

        private bool _modoHuella;
        public bool ModoHuella
        {
            get { return _modoHuella; }
            set
            {
                ModoHuellaHabilitado = false;
                _modoHuella = value;
                OnPropertyChanged("ModoHuella");
            }
        }

        private bool _modoHuellaHabilitado;
        public bool ModoHuellaHabilitado
        {
            get { return _modoHuellaHabilitado; }
            set { _modoHuellaHabilitado = value; OnPropertyChanged("ModoHuellaHabilitado"); }
        }

        private bool _asistenciaNIPSelect;
        public bool AsistenciaNIPSelect
        {
            get { return _asistenciaNIPSelect; }
            set
            {
                if (value == true)
                {
                    ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                }
                _asistenciaNIPSelect = value;
                OnPropertyChanged("AsistenciaNIPSelect");
            }
        }

        private bool _asistenciaBiometricaSelect = true;
        public bool AsistenciaBiometricaSelect
        {
            get { return _asistenciaBiometricaSelect; }
            set
            {
                if (value)
                {
                    BuscarNIP = string.Empty;
                }
                _asistenciaBiometricaSelect = value;
                OnPropertyChanged("AsistenciaBiometricaSelect");
            }
        }

        private bool _nipCapturaVisible;
        public bool NIPCapturaVisible
        {
            get { return _nipCapturaVisible; }
            set { _nipCapturaVisible = value; OnPropertyChanged("NIPCapturaVisible"); }
        }

        private string _textoRegistroEdificio;
        public string TextoRegistroEdificio
        {
            get { return _textoRegistroEdificio; }
            set { _textoRegistroEdificio = value; OnPropertyChanged("TextoRegistroEdificio"); }
        }

        private bool CancelKeepSearching { get; set; }
        private bool GuardandoHuellas { get; set; }
        private bool isKeepSearching { get; set; }
        private string NipInterno { get; set; }

        private SolidColorBrush _fondoBackSpaceNIP;
        public SolidColorBrush FondoBackSpaceNIP
        {
            get { return _fondoBackSpaceNIP; }
            set { _fondoBackSpaceNIP = value; OnPropertyChanged("FondoBackSpaceNIP"); }
        }

        private SolidColorBrush _fondoLimpiarNIP;
        public SolidColorBrush FondoLimpiarNIP
        {
            get { return _fondoLimpiarNIP; }
            set { _fondoLimpiarNIP = value; OnPropertyChanged("FondoLimpiarNIP"); }
        }
        #endregion

        #region [COMMANDOS]
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand _onClickBuscarNip;
        public ICommand OnClickBuscarNip
        {
            get { return _onClickBuscarNip ?? (_onClickBuscarNip = new RelayCommand(ClickBuscarNip)); }
        }

        private ICommand _nipEnter;
        public ICommand NIPEnter
        {
            get { return _nipEnter ?? (_nipEnter = new RelayCommand(ClickEnterNIP)); }
        }

        public ICommand WindowLoading
        {
            get { return new DelegateCommand<LeerInternosEdificio>(OnLoad); }
        }

        public ICommand WindowUnloading
        {
            get { return new DelegateCommand<LeerInternosEdificio>(UnLoad); }
        }

        public ICommand CommandAceptar
        {
            get { return new DelegateCommand<Window>(Aceptar); }
        }

        private ICommand _buttonMouseEnter;
        public ICommand ButtonMouseEnter
        {
            get { return _buttonMouseEnter ?? (_buttonMouseEnter = new RelayCommand(MouseEnterSwitch)); }
        }

        private ICommand _buttonMouseLeave;
        public ICommand ButtonMouseLeave
        {
            get { return _buttonMouseLeave ?? (_buttonMouseLeave = new RelayCommand(MouseLeaveSwitch)); }
        }

        private string _mensajeAprobacionNIP;
        public string MensajeNipAprobacion
        {
            get { return _mensajeAprobacionNIP; }
            set
            {
                _mensajeAprobacionNIP = value;
                OnPropertyChanged("MensajeNipAprobacion");
            }
        }
        #endregion

        #region [METODOS EVENTOS]
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                //NIP interno
                case "OpenCloseFlyout":
                    if (NIPCapturaVisible)
                    {
                        NIPCapturaVisible = false;
                    }
                    else
                    {
                        NIPCapturaVisible = true;
                    }
                    break;
                case "terminarEnrolamiento":
                    LeerInternos.GetWindow(WindowEnrolamientoInternos).Close();
                    break;
                case "backspace":
                    if (BuscarNIP != null)
                    {
                        if (BuscarNIP.Length > 0)
                            BuscarNIP = BuscarNIP.Substring(0, BuscarNIP.Length - 1);
                    }
                    break;
                case "limpiarNIP":
                    BuscarNIP = "";
                    break;
                case "0":
                    BuscarNIP += "0";
                    break;
                case "1":
                    BuscarNIP += "1";
                    break;
                case "2":
                    BuscarNIP += "2";
                    break;
                case "3":
                    BuscarNIP += "3";
                    break;
                case "4":
                    BuscarNIP += "4";
                    break;
                case "5":
                    BuscarNIP += "5";
                    break;
                case "6":
                    BuscarNIP += "6";
                    break;
                case "7":
                    BuscarNIP += "7";
                    break;
                case "8":
                    BuscarNIP += "8";
                    break;
                case "9":
                    BuscarNIP += "9";
                    break;
            }
        }

        private void MouseEnterSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    break;
            }
        }

        private void MouseLeaveSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Color.FromRgb(0, 128, 0));
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Color.FromRgb(220, 20, 60));
                    break;
            }
        }
        #endregion

        #region [METODOS]
        private void ClickEnterNIP(Object obj)
        {
            if (obj != null)
            {
                if (!ModoHuella)
                {
                    BuscarNIP = ((System.Windows.Controls.TextBox)(obj)).Text;
                    ListResultado = null;
                    CompararNIPAsistenciaInterno(BuscarNIP);
                }
                else
                {
                    BuscarNIP = ((System.Windows.Controls.TextBox)(obj)).Text;
                    ListResultado = null;
                    CompararNIPAsistenciaAusente(BuscarNIP);
                }
            }
        }

        private void ClickBuscarNip(Object obj)
        {
            if (obj != null)
            {
                if (!ModoHuella)
                {
                    ListResultado = null;
                    CompararNIPAsistenciaInterno(BuscarNIP);
                }
                else
                {
                    ListResultado = null;
                    CompararNIPAsistenciaAusente(BuscarNIP);
                }
            }
        }

        private void LimpiarCampos()
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ScannerMessage = "Capture Huella";
                ColorMessage = new SolidColorBrush(Colors.Green);
                AceptarBusquedaHuellaFocus = true;
            }));
            SelectRegistro = null;
            PropertyImage = null;
        }

        private void OnLoad(LeerInternosEdificio Window)
        {
            ParametroEstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
            #region [Huellas Digitales]
            var myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 185;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.3));
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.SetTargetName(myDoubleAnimation, "Ln");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
            var myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Begin(Window.Ln);
            ColorNIPAprobacion = new SolidColorBrush(Colors.DarkBlue);
            MarkCheck = "🔍";
            ImagenPlaceHolder = new Imagenes();
            ImagenInterno = ImagenPlaceHolder.getImagenPerson();
            #endregion

            Window.Closed += (s, e) =>
            {
                try
                {
                    if (OnProgress == null)
                        return;

                    if (!IsSucceed)
                        SelectRegistro = null;

                    OnProgress.Abort();
                    CancelCaptureAndCloseReader(OnCaptured);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar búsqueda", ex);
                }
            };
            if (CurrentReader != null)
            {
                CurrentReader.Dispose();
                CurrentReader = null;
            }
            CurrentReader = Readers[0];

            if (CurrentReader == null)
                return;

            if (!OpenReader())
                Window.Close();

            if (!StartCaptureAsync(OnCaptured))
                Window.Close();

            OnProgress = new Thread(() => InvokeDelegate(Window));
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ScannerMessage = "Capture Huella";
                MensajeNipAprobacion = "Capture NIP";
                ColorMessage = new SolidColorBrush(Colors.Green);
                ColorNIPAprobacion = new SolidColorBrush(Colors.DarkBlue);
            }));
            GuardandoHuellas = true;
        }

        //NIP PARA DAR ENTRADA A LOS INTERNOS (AUSENTES)
        private async void CompararNIPAsistenciaAusente(string nip)
        {
            try
            {
                ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                var interno_ausente = new cIngreso().ObtenerPorNIP(nip);

                if (interno_ausente != null)
                {
                    var imputado_ausente = interno_ausente.IMPUTADO;
                    var InternoAusente = new cIngresoUbicacion();
                    var Autorizado = new cIngresoUbicacion().ObtenerTodos().Where(w => w.ID_ANIO == imputado_ausente.ID_ANIO && w.ID_CENTRO == imputado_ausente.ID_CENTRO && w.ID_IMPUTADO == imputado_ausente.ID_IMPUTADO
                        && w.ID_INGRESO == interno_ausente.ID_INGRESO && w.ESTATUS != 0).OrderByDescending(o => o.ID_CONSEC).FirstOrDefault();
                    var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());

                    if (Autorizado != null)
                    {
                        ListResultado.Add(new ResultadoBusquedaBiometrico()
                        {
                            IdCentro = imputado_ausente.ID_CENTRO,
                            IdAnio = imputado_ausente.ID_ANIO,
                            IdImputado = imputado_ausente.ID_IMPUTADO,
                            Nombre = string.IsNullOrEmpty(imputado_ausente.NOMBRE) ? string.Empty : imputado_ausente.NOMBRE.TrimEnd(),
                            APaterno = string.IsNullOrEmpty(imputado_ausente.PATERNO) ? string.Empty : imputado_ausente.PATERNO.TrimEnd(),
                            AMaterno = string.IsNullOrEmpty(imputado_ausente.MATERNO) ? string.Empty : imputado_ausente.MATERNO.TrimEnd(),
                            Expediente = imputado_ausente.ID_ANIO + "/" + imputado_ausente.ID_IMPUTADO,
                            ASISTENCIA = true
                        });
                        ColorNIPAprobacion = new SolidColorBrush(Colors.Green);
                        MensajeNipAprobacion = "Encontrado";
                        NIPCapturaVisible = false;
                        ImagenInterno = new cIngresoBiometrico().Obtener((short)imputado_ausente.ID_ANIO, (short)imputado_ausente.ID_CENTRO, imputado_ausente.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ? new cIngresoBiometrico().Obtener((short)imputado_ausente.ID_ANIO, (short)imputado_ausente.ID_CENTRO, imputado_ausente.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();

                        foreach (var llenar in ListResultado)
                        {
                            var igual = ListaResultadoRequerido.Where(w => w.Expediente == llenar.Expediente).Count();
                            if (igual == 0)
                            {
                                ColorNIPAprobacion = new SolidColorBrush(Colors.Green);
                                MarkCheck = " \u2713 \u2713";
                                MensajeNipAprobacion = "Encontrado";
                                ListaResultadoRequerido.Add(llenar);
                                BuscarNIP = string.Empty;
                                NIPCapturaVisible = false;
                            }
                            else
                            {
                                ColorNIPAprobacion = new SolidColorBrush(Colors.DarkOrange);
                                MarkCheck = "!";
                                MensajeNipAprobacion = "Capturado previamente";
                                BuscarNIP = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        ColorNIPAprobacion = new SolidColorBrush(Colors.Red);
                        MarkCheck = "X";
                        MensajeNipAprobacion = "No encontrado";
                        BuscarNIP = string.Empty;
                    }
                }
                else
                {
                    ColorNIPAprobacion = new SolidColorBrush(Colors.Red);
                    MarkCheck = "X";
                    MensajeNipAprobacion = "No encontrado";
                    BuscarNIP = string.Empty;
                }
                await TaskEx.Delay(2000);
                ColorNIPAprobacion = new SolidColorBrush(Colors.DarkBlue);
                MarkCheck = "🔍";
                MensajeNipAprobacion = "Capture NIP";
                ImagenInterno = ImagenPlaceHolder.getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener información.", ex);
            }
        }

        // NIP PARA DAR SALIDA A LOS INTERNOS (REQUERIDOS)
        private async void CompararNIPAsistenciaInterno(string nip)
        {
            try
            {
                ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                var interno = new cIngreso().ObtenerPorNIP(nip);

                if (interno != null)
                {
                    if (!ParametroEstatusInactivos.Contains(interno.ID_ESTATUS_ADMINISTRATIVO))
                    {
                        var imputado = interno.IMPUTADO;
                        var InternoAusente = new cIngresoUbicacion();
                        var Autorizado = new cIngresoUbicacion().ObtenerTodos().Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO
                            && w.ID_INGRESO == interno.ID_INGRESO && w.ESTATUS == 0).OrderByDescending(o => o.ID_CONSEC).FirstOrDefault();
                        var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());

                        if (Autorizado != null)
                        {
                            ListResultado.Add(new ResultadoBusquedaBiometrico()
                            {
                                IdCentro = imputado.ID_CENTRO,
                                IdAnio = imputado.ID_ANIO,
                                IdImputado = imputado.ID_IMPUTADO,
                                Nombre = string.IsNullOrEmpty(imputado.NOMBRE) ? string.Empty : imputado.NOMBRE.TrimEnd(),
                                APaterno = string.IsNullOrEmpty(imputado.PATERNO) ? string.Empty : imputado.PATERNO.TrimEnd(),
                                AMaterno = string.IsNullOrEmpty(imputado.MATERNO) ? string.Empty : imputado.MATERNO.TrimEnd(),
                                Expediente = imputado.ID_ANIO + "/" + imputado.ID_IMPUTADO,
                                ASISTENCIA = true
                            });
                            //ColorNIPAprobacion = new SolidColorBrush(Colors.Green);
                            //MarkCheck = " \u2713 \u2713";
                            //MensajeNipAprobacion = "Encontrado";
                            //NIPCapturaVisible = false;
                            ImagenInterno = new cIngresoBiometrico().Obtener((short)imputado.ID_ANIO, (short)imputado.ID_CENTRO, imputado.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ? new cIngresoBiometrico().Obtener((short)imputado.ID_ANIO, (short)imputado.ID_CENTRO, imputado.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();

                            foreach (var llenar in ListResultado)
                            {
                                var igual = ListaResultadoRequerido.Where(w => w.Expediente == llenar.Expediente).Count();
                                if (igual == 0)
                                {
                                    ColorNIPAprobacion = new SolidColorBrush(Colors.Green);
                                    MarkCheck = " \u2713 \u2713";
                                    MensajeNipAprobacion = "Encontrado";
                                    ListaResultadoRequerido.Add(llenar);
                                    BuscarNIP = string.Empty;
                                    NIPCapturaVisible = false;
                                }
                                else
                                {
                                    ColorNIPAprobacion = new SolidColorBrush(Colors.DarkOrange);
                                    MarkCheck = "!";
                                    MensajeNipAprobacion = "Capturado previamente";
                                    BuscarNIP = string.Empty;
                                }
                            }
                        }
                        else if (Autorizado == null)
                        {

                            var interno_primera_vez = new cIngresoUbicacion().ObtenerTodos().Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO
                           && w.ID_INGRESO == interno.ID_INGRESO).Count();
                            if (interno_primera_vez == 0)
                            {
                                ListResultado.Add(new ResultadoBusquedaBiometrico()
                                {
                                    IdCentro = imputado.ID_CENTRO,
                                    IdAnio = imputado.ID_ANIO,
                                    IdImputado = imputado.ID_IMPUTADO,
                                    Nombre = string.IsNullOrEmpty(imputado.NOMBRE) ? string.Empty : imputado.NOMBRE.TrimEnd(),
                                    APaterno = string.IsNullOrEmpty(imputado.PATERNO) ? string.Empty : imputado.PATERNO.TrimEnd(),
                                    AMaterno = string.IsNullOrEmpty(imputado.MATERNO) ? string.Empty : imputado.MATERNO.TrimEnd(),
                                    Expediente = imputado.ID_ANIO + "/" + imputado.ID_IMPUTADO,
                                    ASISTENCIA = true
                                });
                                //ColorNIPAprobacion = new SolidColorBrush(Colors.Green);
                                //MarkCheck = " \u2713 \u2713";
                                //MensajeNipAprobacion = "Encontrado";
                                //NIPCapturaVisible = false;
                                ImagenInterno = new cIngresoBiometrico().Obtener((short)imputado.ID_ANIO, (short)imputado.ID_CENTRO, imputado.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ? new cIngresoBiometrico().Obtener((short)imputado.ID_ANIO, (short)imputado.ID_CENTRO, imputado.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();

                                foreach (var llenar in ListResultado)
                                {
                                    var igual = ListaResultadoRequerido.Where(w => w.Expediente == llenar.Expediente).Count();
                                    if (igual == 0)
                                    {
                                        ColorNIPAprobacion = new SolidColorBrush(Colors.Green);
                                        MarkCheck = " \u2713 \u2713";
                                        MensajeNipAprobacion = "Encontrado";
                                        ListaResultadoRequerido.Add(llenar);
                                        BuscarNIP = string.Empty;
                                        NIPCapturaVisible = false;
                                    }
                                    else
                                    {
                                        ColorNIPAprobacion = new SolidColorBrush(Colors.DarkOrange);
                                        MarkCheck = "!";
                                        MensajeNipAprobacion = "Capturado previamente";
                                        BuscarNIP = string.Empty;
                                    }
                                }
                            }
                            else
                            {
                                ColorNIPAprobacion = new SolidColorBrush(Colors.Red);
                                MarkCheck = "X";
                                MensajeNipAprobacion = "No encontrado";
                                BuscarNIP = string.Empty;
                            }
                        }
                        else
                        {
                            ColorNIPAprobacion = new SolidColorBrush(Colors.Red);
                            MarkCheck = "X";
                            MensajeNipAprobacion = "No encontrado";
                            BuscarNIP = string.Empty;
                        }
                    }
                    else
                    {
                        ColorNIPAprobacion = new SolidColorBrush(Colors.Red);
                        MarkCheck = "X";
                        MensajeNipAprobacion = "No encontrado";
                        BuscarNIP = string.Empty;
                    }
                    await TaskEx.Delay(2000);
                    ColorNIPAprobacion = new SolidColorBrush(Colors.DarkBlue);
                    MarkCheck = "🔍";
                    MensajeNipAprobacion = "Capture NIP";
                    ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener información.", ex);
            }
        }

        public override void OnCaptured(DPUruNet.CaptureResult captureResult)
        {
            if (ScannerMessage.Contains("Procesando..."))
                return;

            ShowLoading = Visibility.Visible;
            ShowLine = Visibility.Visible;
            ShowCapturar = Visibility.Collapsed;

            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ShowLine = Visibility.Visible;
                ScannerMessage = "Procesando...";
                ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
            }));
            base.OnCaptured(captureResult);

            switch (BuscarPor)
            {
                case enumTipoPersona.IMPUTADO:
                    CompararHuellaAsistenciaInterno();
                    break;
                default:
                    break;
            }
            GuardandoHuellas = true;
            ShowLoading = Visibility.Collapsed;
            ShowCapturar = Conectado ? Visibility.Visible : Visibility.Collapsed;
            ShowLine = Visibility.Collapsed;
        }

        //HUELLA PARA LOS INTERNOS
        private Task<bool> CompararHuellaAsistenciaInterno(byte[] Huella = null, enumTipoBiometrico? Finger = null)
        {
            try
            {
                var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null ?? Huella;
                var verifyFinger = Finger ?? (DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO);
                ImagenInterno = null;
                ListResultado = null;
                TextoRegistroEdificio = null;
                if (bytesHuella == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (Finger == null)
                            ScannerMessage = "Vuelve a capturar las huellas";
                        else
                            ScannerMessage = "Siguiente Huella";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        ShowLine = Visibility.Collapsed;
                    }));
                }
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                    AceptarBusquedaHuellaFocus = false;
                }));
                var Service = new BiometricoServiceClient();
                if (Service == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Error en el servicio de comparación";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        ShowLine = Visibility.Collapsed;
                    }));
                }

                //COINCIDENCIA DEL INTERNO POR EDIFICIO Y SECTOR
                if (IdEdificio != -1 && IdSector != -1)
                {
                     var CompareResult =  Service.CompararHuellaImputadoPorUbicacion(new ComparationRequest { BIOMETRICO = bytesHuella, ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, ID_TIPO_PERSONA = null, ID_CENTRO = GlobalVar.gCentro, ID_EDIFICIO = IdEdificio, ID_SECTOR = IdSector });
                    //var CompareResult = Service.CompararHuellaImputado(new ComparationRequest { BIOMETRICO = bytesHuella, ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, ID_TIPO_PERSONA = null});

                    if (CompareResult.Identify)
                    {
                        ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();

                        if (CompareResult.Result.Count() > 1)
                        {
                            TextoRegistroEdificio = "NOTA: Se encontró más de una coincidencia, por favor intente de nuevo";
                            ScannerMessage = "";
                            return TaskEx.FromResult(false);
                        }
                        else
                        {
                            foreach (var item in CompareResult.Result)
                            {
                                var imputado = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == item.ID_ANIO && w.ID_CENTRO == item.ID_CENTRO && w.ID_IMPUTADO == item.ID_IMPUTADO && (w.ID_TIPO_BIOMETRICO == (DD_Dedo.HasValue ? (short)DD_Dedo.Value : (short)enumTipoBiometrico.INDICE_DERECHO) && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)).OrderBy(o => o.ID_CENTRO).ThenBy(t => t.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).FirstOrDefault();

                                ShowContinuar = Visibility.Collapsed;
                                if (imputado == null)
                                    continue;

                                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    var ingresobiometrico = imputado.IMPUTADO.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                    var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());

                                    if (ingresobiometrico != null)
                                        if (ingresobiometrico.INGRESO_BIOMETRICO.Any())
                                            if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                            else
                                                if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                    FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);

                                    ListResultado.Add(new ResultadoBusquedaBiometrico()
                                    {
                                        IdCentro = imputado.ID_CENTRO,
                                        IdAnio = imputado.ID_ANIO,
                                        IdImputado = imputado.ID_IMPUTADO,
                                        Nombre = string.IsNullOrEmpty(imputado.IMPUTADO.NOMBRE) ? string.Empty : imputado.IMPUTADO.NOMBRE.TrimEnd(),
                                        APaterno = string.IsNullOrEmpty(imputado.IMPUTADO.PATERNO) ? string.Empty : imputado.IMPUTADO.PATERNO.TrimEnd(),
                                        AMaterno = string.IsNullOrEmpty(imputado.IMPUTADO.MATERNO) ? string.Empty : imputado.IMPUTADO.MATERNO.TrimEnd(),
                                        Expediente = imputado.ID_ANIO + "/" + imputado.ID_IMPUTADO,
                                        ASISTENCIA = true,
                                        NIP = imputado.IMPUTADO.NIP,
                                        Imputado = imputado.IMPUTADO
                                    });
                                }));
                                ImagenInterno = new cIngresoBiometrico().Obtener((short)imputado.ID_ANIO, (short)imputado.ID_CENTRO, imputado.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ? new cIngresoBiometrico().Obtener((short)imputado.ID_ANIO, (short)imputado.ID_CENTRO, imputado.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();
                            }
                        }
                        foreach (var llenar in ListResultado)
                        {
                            var igual = ListaResultadoRequerido.Where(w => w.Expediente == llenar.Expediente).Count();

                            if (igual == 0)
                            {
                                ListaResultadoRequerido.Add(llenar);
                            }
                            ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);
                            ShowContinuar = Visibility.Collapsed;

                            //LOGICA PARA DAR SALIDA A LOS INTERNOS (REQUERIDOS)
                            if (!ModoHuella)
                            {
                                foreach (var row in ListResultado)
                                {
                                    //var test = new InternosRequeridos().ListaInternos(Fechas.GetFechaDateServer.Date, Fechas.GetFechaDateServer.Date, GlobalVar.gCentro, IdEdificio, IdSector).Where(w => w.IdImputado == row.IdImputado).Count();
                                    //foreach (var item in ListaRequeridosInternos)
                                    //{
                                    //    if (row.Expediente == item.Expediente)
                                    //    {
                                    //        row.BanderaActividad = true;
                                    //        break;
                                    //    }
                                    //}
                                    ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);
                                    if (ListResultado.Count > 1)
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            TextoRegistroEdificio = "NOTA: Se encontró más de una coincidencia, por favor intente de nuevo";
                                        }));
                                    }
                                    if (ListResultado.Any())
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            if (igual > 0)
                                            {
                                                ScannerMessage = "Capturado previamente";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Orange);
                                            }
                                            else if (!CancelKeepSearching)
                                            {
                                                //if (row.BanderaActividad)
                                                //{
                                                ScannerMessage = "Registro encontrado";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Green);
                                                //}
                                                //else
                                                //{
                                                //    ScannerMessage = "No tiene actividad";
                                                //    AceptarBusquedaHuellaFocus = true;
                                                //    ColorMessage = new SolidColorBrush(Colors.Red);
                                                //}
                                            }
                                        }));
                                        if (Finger != null)
                                            Service.Close();

                                        return TaskEx.FromResult(false);
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            if (!CancelKeepSearching)
                                            {
                                                ScannerMessage = "Registro no encontrado";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Red);
                                                ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                                            }
                                        }));
                                    }
                                }
                            }

                            //LOGICA PARA DAR ENTRADA AL EDIFICIO (INTERNOS AUSENTES)
                            else
                            {
                                //ListaAusentesInternos = new InternosAusentes().ListInternosAusentes();
                                foreach (var row in ListResultado)
                                {
                                    //foreach (var item in ListaAusentesInternos)
                                    //{
                                    //    if (row.Expediente == item.Expediente)
                                    //    {
                                    //        row.BanderaActividad = true;
                                    //        break;
                                    //    }
                                    //}
                                    ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);
                                    if (ListResultado.Any())
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            if (igual > 0)
                                            {
                                                ScannerMessage = "Capturado previamente";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Orange);
                                            }
                                            else if (!CancelKeepSearching)
                                            {
                                                //if (row.BanderaActividad)
                                                //{
                                                ScannerMessage = "Registro encontrado";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Green);
                                                //}
                                                //else
                                                //{
                                                //    ScannerMessage = "No tiene actividad";
                                                //    AceptarBusquedaHuellaFocus = true;
                                                //    ColorMessage = new SolidColorBrush(Colors.Red);
                                                //}
                                            }
                                        }));
                                        if (Finger != null)
                                            Service.Close();

                                        return TaskEx.FromResult(false);
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            if (!CancelKeepSearching)
                                            {
                                                ScannerMessage = "Registro no encontrado";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Red);
                                                ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                                            }
                                        }));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            if (!CancelKeepSearching)
                            {
                                ScannerMessage = "Huella no encontrada";
                                ColorMessage = new SolidColorBrush(Colors.Red);

                                AceptarBusquedaHuellaFocus = true;
                                ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                            }
                        }));
                        IsSucceed = false;
                        if (!CancelKeepSearching)
                        {
                            SelectRegistro = null;
                        }
                        PropertyImage = null;
                    }
                }
                //COINCIDENCIA DEL INTERNO EN TODOS LOS EDIFICIOS
                else if (IdEdificio == -1 || IdSector == -1)
                {
                    var CompareResult = Service.CompararHuellaImputadoPorUbicacion(new ComparationRequest { BIOMETRICO = bytesHuella, ID_TIPO_BIOMETRICO = verifyFinger, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, ID_TIPO_PERSONA = null, ID_CENTRO = GlobalVar.gCentro });
                    //var CompareResult = Service.CompararHuellaImputado(new ComparationRequest { BIOMETRICO = bytesHuella, ID_TIPO_BIOMETRICO = verifyFinger, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, ID_TIPO_PERSONA = null });

                    if (CompareResult.Identify)
                    {
                        ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();

                        if (CompareResult.Result.Count() > 1)
                        {
                            TextoRegistroEdificio = "NOTA: Se encontró más de una coincidencia, por favor intente de nuevo";
                            ScannerMessage = "";
                            return TaskEx.FromResult(false);
                        }
                        else
                        {
                            foreach (var item in CompareResult.Result)
                            {
                                var imputado = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == item.ID_ANIO && w.ID_CENTRO == item.ID_CENTRO && w.ID_IMPUTADO == item.ID_IMPUTADO && (w.ID_TIPO_BIOMETRICO == (DD_Dedo.HasValue ? (short)DD_Dedo.Value : (short)enumTipoBiometrico.INDICE_DERECHO) && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)).OrderBy(o => o.ID_CENTRO).ThenBy(t => t.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).FirstOrDefault();

                                ShowContinuar = Visibility.Collapsed;
                                if (imputado == null)
                                    continue;

                                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    var ingresobiometrico = imputado.IMPUTADO.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                    var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());

                                    if (ingresobiometrico != null)
                                        if (ingresobiometrico.INGRESO_BIOMETRICO.Any())
                                            if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                            else
                                                if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                    FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);

                                    ListResultado.Add(new ResultadoBusquedaBiometrico()
                                    {
                                        IdCentro = imputado.ID_CENTRO,
                                        IdAnio = imputado.ID_ANIO,
                                        IdImputado = imputado.ID_IMPUTADO,
                                        Nombre = string.IsNullOrEmpty(imputado.IMPUTADO.NOMBRE) ? string.Empty : imputado.IMPUTADO.NOMBRE.TrimEnd(),
                                        APaterno = string.IsNullOrEmpty(imputado.IMPUTADO.PATERNO) ? string.Empty : imputado.IMPUTADO.PATERNO.TrimEnd(),
                                        AMaterno = string.IsNullOrEmpty(imputado.IMPUTADO.MATERNO) ? string.Empty : imputado.IMPUTADO.MATERNO.TrimEnd(),
                                        Expediente = imputado.ID_ANIO + "/" + imputado.ID_IMPUTADO,
                                        ASISTENCIA = true,
                                        NIP = imputado.IMPUTADO.NIP,
                                        Imputado = imputado.IMPUTADO
                                    });
                                }));
                                ImagenInterno = new cIngresoBiometrico().Obtener((short)imputado.ID_ANIO, (short)imputado.ID_CENTRO, imputado.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ? new cIngresoBiometrico().Obtener((short)imputado.ID_ANIO, (short)imputado.ID_CENTRO, imputado.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();
                            }
                        }
                        foreach (var llenar in ListResultado)
                        {
                            var igual = ListaResultadoRequerido.Where(w => w.Expediente == llenar.Expediente).Count();

                            if (igual == 0)
                            {
                                ListaResultadoRequerido.Add(llenar);
                            }
                            else
                            {
                                llenar.ASISTENCIA = false;
                            }
                            ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);
                            ShowContinuar = Visibility.Collapsed;

                            //LOGICA PARA DAR SALIDA A LOS INTERNOS (REQUERIDOS)
                            if (!ModoHuella)
                            {
                                //ListaRequeridosInternos = new InternosRequeridos().ListaInternos(Fechas.GetFechaDateServer.Date, Fechas.GetFechaDateServer.Date, GlobalVar.gCentro, IdEdificio, IdSector);
                                foreach (var row in ListResultado)
                                {
                                    //foreach (var item in ListaRequeridosInternos)
                                    //{
                                    //    if (row.Expediente == item.Expediente)
                                    //    {
                                    //        row.BanderaActividad = true;
                                    //        break;
                                    //    }
                                    //}
                                    ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);
                                    if (ListResultado.Any())
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            if (igual > 0)
                                            {
                                                ScannerMessage = "Capturado previamente";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Orange);
                                            }
                                            else if (!CancelKeepSearching)
                                            {
                                                //if (row.BanderaActividad)
                                                //{
                                                ScannerMessage = "Registro encontrado";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Green);
                                                //}
                                                //else
                                                //{
                                                //    ScannerMessage = "No tiene actividad";
                                                //    AceptarBusquedaHuellaFocus = true;
                                                //    ColorMessage = new SolidColorBrush(Colors.Red);
                                                //}
                                            }
                                        }));
                                        if (Finger != null)
                                            Service.Close();

                                        return TaskEx.FromResult(false);
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            if (!CancelKeepSearching)
                                            {
                                                ScannerMessage = "Registro no encontrado";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Red);
                                                ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                                            }
                                        }));
                                    }
                                }
                            }

                            //LOGICA PARA DAR ENTRADA AL EDIFICIO (INTERNOS AUSENTES)
                            else
                            {
                                //ListaAusentesInternos = new InternosAusentes().ListInternosAusentes();
                                foreach (var row in ListResultado)
                                {
                                    //foreach (var item in ListaAusentesInternos)
                                    //{
                                    //    if (row.Expediente == item.Expediente)
                                    //    {
                                    //        row.BanderaActividad = true;
                                    //        break;
                                    //    }
                                    //}
                                    ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);
                                    if (ListResultado.Any())
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            if (igual > 0)
                                            {
                                                ScannerMessage = "Capturado previamente";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Orange);
                                            }
                                            else if (!CancelKeepSearching)
                                            {
                                                //if (row.BanderaActividad)
                                                //{
                                                ScannerMessage = "Registro encontrado";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Green);
                                                //}
                                                //else
                                                //{
                                                //    ScannerMessage = "No tiene actividad";
                                                //    AceptarBusquedaHuellaFocus = true;
                                                //    ColorMessage = new SolidColorBrush(Colors.Red);
                                                //}
                                            }
                                        }));
                                        if (Finger != null)
                                            Service.Close();

                                        return TaskEx.FromResult(false);
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            if (!CancelKeepSearching)
                                            {
                                                ScannerMessage = "Registro no encontrado";
                                                AceptarBusquedaHuellaFocus = true;
                                                ColorMessage = new SolidColorBrush(Colors.Red);
                                                ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                                            }
                                        }));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            if (!CancelKeepSearching)
                            {
                                ScannerMessage = "Huella no encontrada";
                                ColorMessage = new SolidColorBrush(Colors.Red);
                                AceptarBusquedaHuellaFocus = true;
                                ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                            }
                        }));
                        IsSucceed = false;
                        if (!CancelKeepSearching)
                        {
                            SelectRegistro = null;
                        }
                        PropertyImage = null;
                    }
                }
                DD_Dedo = enumTipoBiometrico.INDICE_DERECHO;
                Service.Close();
                FingerPrintData = null;

                return TaskEx.FromResult(true);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo conectar al servidor de huellas", ex);
                return TaskEx.FromResult(false);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                return TaskEx.FromResult(false);
            }
        }

        private void Aceptar(Window Window)
        {
            if (ScannerMessage.Contains("Procesando..."))
                return;
            Window.Close();
        }

        private void UnLoad(LeerInternosEdificio Window = null)
        {
            PrincipalViewModel.CambiarVentanaSelecccionado += (o, e) =>
            {

            };
        }

        private async Task WaitForFingerPrints()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!GuardandoHuellas) ;
            });
        }
        #endregion

        #region [CLASE PARA INTERNOS]
        public class ResultadoBusquedaBiometrico
        {
            public int IdCentro { get; set; }
            public int IdAnio { get;set;}
            public int IdImputado { get; set; }
            public string Expediente { get; set; }
            public string NIP { get; set; }
            public string APaterno { get; set; }
            public string AMaterno { get; set; }
            public string Nombre { get; set; }
            public bool ASISTENCIA { get; set; }
            public ImageSource Foto { get; set; }
            public IMPUTADO Imputado { get; set; }
            public bool BanderaActividad { get; set; } // VARIABLE PARA SABER SI TIENE ACTIVIDAD
        }
        #endregion
    }
}

