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

namespace ControlPenales.ControlInternosEdificio.ViewModel
{
    public class LeerCustodioEdificioViewModel : FingerPrintScanner
    {
        #region [VARIABLES]
        private short? IdEdificio;
        private short? IdSector;
        #endregion

        #region [CONSTRUCTOR]
        public LeerCustodioEdificioViewModel(enumTipoPersona tipobusqueda, bool? set442 = null, bool GuardarHuellas = false, short? IdEdificio = 0, short? IdSector = 0)
        {
            ListaCustodio = new List<ResultadoBusquedaBiometricoCustodio>();
            Buscar = tipobusqueda;
            Conect = set442.HasValue ? set442.Value : false;
            CapturarShow = set442.HasValue ? set442.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
            this.GuardarHuellas = GuardarHuellas;
            this.IdEdificio = IdEdificio;
            this.IdSector = IdSector;
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            switch (tipobusqueda)
            {
                case enumTipoPersona.PERSONA_EMPLEADO:
                    BusquedaCabecera = "Datos del Custodio";
                    FotoCabecera = "Foto del Custodio";
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region [PROPIEDADES]
        private bool Conect;
        private bool GuardarHuellas { get; set; }
        public IList<PlantillaBiometrico> HuellasCapturadasCustodio { get; set; }

        private string _busquedaCabecera;
        public string BusquedaCabecera
        {
            get { return _busquedaCabecera; }
            set { _busquedaCabecera = value; OnPropertyChanged("BusquedaCabecera"); }
        }

        private string _fotoCabecera;
        public string FotoCabecera
        {
            get { return _fotoCabecera; }
            set { _fotoCabecera = value; OnPropertyChanged("FotoCabecera"); }
        }

        private Visibility _capturarShow = Visibility.Collapsed;
        public Visibility CapturarShow
        {
            get { return _capturarShow; }
            set { _capturarShow = value; OnPropertyChanged("CapturarShow"); }
        }

        private Visibility _showLinea;
        public Visibility ShowLinea
        {
            get { return _showLinea; }
            set { _showLinea = value; OnPropertyChanged("ShowLinea"); }
        }

        private Visibility _showImagenCustodio;
        public Visibility ShowImagenCustodio
        {
            get { return _showImagenCustodio; }
            set { _showImagenCustodio = value; OnPropertyChanged("ShowImagenCustodio"); }
        }

        private byte[] _imagenCustodio;
        public byte[] ImagenCustodio
        {
            get { return _imagenCustodio; }
            set { _imagenCustodio = value; OnPropertyChanged("ImagenCustodio"); }
        }

        private string _markCheck;
        public string MarkCheck
        {
            get { return _markCheck; }
            set { _markCheck = value; OnPropertyChanged("MarkCheck"); }
        }

        private bool _imageHuellaVisible;
        public bool ImageHuellaVisible
        {
            get { return _imageHuellaVisible; }
            set { _imageHuellaVisible = value; OnPropertyChanged("ImageHuellaVisible"); }
        }

        private string _image;
        public string Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged("Image"); }
        }

        private Window WindowEnrolamientoCustodio { get; set; }

        private ResultadoBusquedaBiometricoCustodio _selectedRegistroCustodio;
        public ResultadoBusquedaBiometricoCustodio SelectedRegistroCustodio
        {
            get { return _selectedRegistroCustodio; }
            set { _selectedRegistroCustodio = value; OnPropertyChanged("SelectedRegistroCustodio"); }
        }

        private bool _isSucceded = false;
        public bool IsSucceded
        {
            get { return _isSucceded; }
            set { _isSucceded = value; OnPropertyChanged("IsSucceded"); }
        }

        private List<ResultadoBusquedaBiometricoCustodio> _listResultadoCustodio;
        public List<ResultadoBusquedaBiometricoCustodio> ListResultadoCustodio
        {
            get { return _listResultadoCustodio; }
            set { _listResultadoCustodio = value; OnPropertyChanged("ListResultadoCustodio"); }
        }

        private EMPLEADO _custodioSelect;
        public EMPLEADO CustodioSelect
        {
            get { return _custodioSelect; }
            set
            {
                _custodioSelect = value;
                OnPropertyChanged("CustodioSelect");
            }
        }

        private List<ResultadoBusquedaBiometricoCustodio> _listaCustodio;
        public List<ResultadoBusquedaBiometricoCustodio> ListaCustodio
        {
            get { return _listaCustodio; }
            set { _listaCustodio = value; OnPropertyChanged("ListaCustodio"); }
        }

        public enumTipoPersona Buscar { get; set; }

        private enumTipoBiometrico? _dD_DedoHuella = enumTipoBiometrico.INDICE_DERECHO;
        public enumTipoBiometrico? DD_DedoHuella
        {
            get { return _dD_DedoHuella; }
            set
            {
                LimpiarCampos();
                _dD_DedoHuella = value;
                OnPropertyChanged("DD_DedoHuella");
            }
        }

        private bool _focusAceptarBusquedaHuellaFocus;
        public bool FocusAceptarBusquedaHuella
        {
            get { return _focusAceptarBusquedaHuellaFocus; }
            set { _focusAceptarBusquedaHuellaFocus = value; OnPropertyChanged("FocusAceptarBusquedaHuella"); }
        }

        private System.Windows.Media.Brush _colorMensaje;
        public System.Windows.Media.Brush ColorMensaje
        {
            get { return _colorMensaje; }
            set { _colorMensaje = value; RaisePropertyChanged("ColorMensaje"); }
        }

        private Visibility _showContinue = Visibility.Collapsed;
        public Visibility ShowContinue
        {
            get { return _showContinue; }
            set { _showContinue = value; OnPropertyChanged("ShowContinue"); }
        }

        private Visibility _showLoadingCustodio = Visibility.Collapsed;
        public Visibility ShowLoadingCustodio
        {
            get { return _showLoadingCustodio; }
            set { _showLoadingCustodio = value; OnPropertyChanged("ShowLoadingCustodio"); }
        }

        private bool _modoHuellaCustodio;
        public bool ModoHuellaCustodio
        {
            get { return _modoHuellaCustodio; }
            set
            {
                ModoHuellaCustodioHabilitado = false;
                _modoHuellaCustodio = value;
                OnPropertyChanged("ModoHuellaCustodio");
            }
        }

        private bool _modoHuellaCustodioHabilitado;
        public bool ModoHuellaCustodioHabilitado
        {
            get { return _modoHuellaCustodioHabilitado; }
            set { _modoHuellaCustodioHabilitado = value; OnPropertyChanged("ModoHuellaCustodioHabilitado"); }
        }

        private string _textoRegistroEdificioCustodio;
        public string TextoRegistroEdificioCustodio
        {
            get { return _textoRegistroEdificioCustodio; }
            set { _textoRegistroEdificioCustodio = value; OnPropertyChanged("TextoRegistroEdificioCustodio"); }
        }

        private bool _nipCapturaVisible;
        public bool NIPCapturaVisible
        {
            get { return _nipCapturaVisible; }
            set { _nipCapturaVisible = value; OnPropertyChanged("NIPCapturaVisible"); }
        }

        private string _buscarNIP;
        public string BuscarNIP
        {
            get { return _buscarNIP; }
            set { _buscarNIP = value; OnPropertyChanged("BuscarNIP"); }
        }

        private SolidColorBrush _fondoLimpiarNIP;
        public SolidColorBrush FondoLimpiarNIP
        {
            get { return _fondoLimpiarNIP; }
            set { _fondoLimpiarNIP = value; OnPropertyChanged("FondoLimpiarNIP"); }
        }

        private SolidColorBrush _fondoBackSpaceNIP;
        public SolidColorBrush FondoBackSpaceNIP
        {
            get { return _fondoBackSpaceNIP; }
            set { _fondoBackSpaceNIP = value; OnPropertyChanged("FondoBackSpaceNIP"); }
        }

        private bool _modoHuellaHabilitado;
        public bool ModoHuellaHabilitado
        {
            get { return _modoHuellaHabilitado; }
            set { _modoHuellaHabilitado = value; OnPropertyChanged("ModoHuellaHabilitado"); }
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

        public Imagenes ImagenPlaceHolder { get; set; }

        private enum enumAreaTrabajo
        {
            MEDICO_DE_GUARDIA = 12,
            COMANDANCIA = 4
        }

        public enum enumMensajeNIP
        {
            ENCONTRADO = 1,
            NO_ENCONTRADO = 2
        }

        private System.Windows.Media.Brush _colorAprobacionNIP;
        public System.Windows.Media.Brush ColorNIPAprobacion
        {
            get { return _colorAprobacionNIP; }
            set { _colorAprobacionNIP = value; RaisePropertyChanged("ColorNIPAprobacion"); }
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

        private bool CancelKeepSearching { get; set; }
        private bool GuardandoHuellas { get; set; }
        private bool isKeepSearching { get; set; }
        #endregion

        #region [COMMANDOS]
        private ICommand _onClickBuscarNip;
        public ICommand OnClickBuscarNip
        {
            get { return _onClickBuscarNip ?? (_onClickBuscarNip = new RelayCommand(ClickBuscarNip)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand _nipEnter;
        public ICommand NIPEnter
        {
            get { return _nipEnter ?? (_nipEnter = new RelayCommand(ClickNIPEnter)); }
        }

        public ICommand WindowLoading
        {
            get { return new DelegateCommand<LeerCustodioEdificio>(OnLoad); }
        }

        public ICommand CommandAceptar
        {
            get { return new DelegateCommand<Window>(Aceptar); }
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
                    LeerInternos.GetWindow(WindowEnrolamientoCustodio).Close();
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
        #endregion

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

        #region [METODOS]
        private void ClickNIPEnter(Object obj)
        {
            if (obj != null)
            {
                if (!ModoHuella)
                {
                    BuscarNIP = ((System.Windows.Controls.TextBox)(obj)).Text;
                    ListResultadoCustodio = null;
                    CompararNIPAsistenciaCustodio(BuscarNIP);
                }
                else
                {
                    BuscarNIP = ((System.Windows.Controls.TextBox)(obj)).Text;
                    ListResultadoCustodio = null;
                    CompararNIPAsistenciaCustodio(BuscarNIP);
                }
            }
        }

        private void ClickBuscarNip(Object obj)
        {
            if (obj != null)
            {
                if (!ModoHuella)
                {
                    ListResultadoCustodio = null;
                    CompararNIPAsistenciaCustodio(BuscarNIP);
                }
                else
                {
                    ListResultadoCustodio = null;
                    CompararNIPAsistenciaCustodio(BuscarNIP);
                }
            }
        }

        private void ClickEnterNIP(Object obj)
        {
            if (obj != null)
            {
                if (ModoHuella == false)
                {
                    BuscarNIP = ((System.Windows.Controls.TextBox)(obj)).Text;
                    ListResultadoCustodio = null;
                    CompararNIPAsistenciaCustodio(BuscarNIP);
                }
                else
                {
                    BuscarNIP = ((System.Windows.Controls.TextBox)(obj)).Text;
                    ListResultadoCustodio = null;
                    CompararNIPAsistenciaCustodio(BuscarNIP);
                }
            }
        }

        private void LimpiarCampos()
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ScannerMessage = "Capture Huella";
                ColorMensaje = new SolidColorBrush(Colors.Green);
                FocusAceptarBusquedaHuella = true;
            }));
            SelectedRegistroCustodio = null;
            PropertyImage = null;
        }

        private void OnLoad(LeerCustodioEdificio Window)
        {
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
            MensajeNipAprobacion = "Capture NIP";
            MarkCheck = "🔍";
            ImagenPlaceHolder = new Imagenes();
            ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
            #endregion

            Window.Closed += (s, e) =>
            {
                try
                {
                    if (OnProgress == null)
                        return;

                    if (!IsSucceded)
                        SelectedRegistroCustodio = null;

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
                ColorMensaje = new SolidColorBrush(Colors.Green);
                ColorNIPAprobacion = new SolidColorBrush(Colors.DarkBlue);
            }));
            GuardandoHuellas = true;
        }

        //NIP PARA DAR ENCONTRAR A LOS CUSTODIOS (AUSENTES)
        private async void CompararNIPAsistenciaCustodio(string nip)
        {
            try
            {
                ListResultadoCustodio = ListResultadoCustodio ?? new List<ResultadoBusquedaBiometricoCustodio>();
                if (BuscarNIP != string.Empty)
                {
                    long NIP = Convert.ToInt64(nip);
                    //Se busca el custodio por medio del ID obtenido
                    CustodioSelect = new cEmpleado().ObtenerEmpleadoPorDepartamento(NIP, (short)enumAreaTrabajo.COMANDANCIA);

                    if (CustodioSelect != null)
                    {
                        ListResultadoCustodio.Add(new ResultadoBusquedaBiometricoCustodio()
                            {
                                //Se muestra la información del custodio encontrado
                                Nombre = string.IsNullOrEmpty(CustodioSelect.PERSONA.NOMBRE) ? string.Empty : CustodioSelect.PERSONA.NOMBRE.TrimEnd(),
                                CPaterno = string.IsNullOrEmpty(CustodioSelect.PERSONA.PATERNO) ? string.Empty : CustodioSelect.PERSONA.PATERNO.TrimEnd(),
                                CMaterno = string.IsNullOrEmpty(CustodioSelect.PERSONA.MATERNO) ? string.Empty : CustodioSelect.PERSONA.MATERNO.TrimEnd(),
                                IdPersona = CustodioSelect.ID_EMPLEADO,
                                ENCONTRADO = true,
                                Persona = CustodioSelect.PERSONA
                            });
                        ImagenCustodio = new cPersonaBiometrico().ObtenerTodos(CustodioSelect.ID_EMPLEADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ? new cPersonaBiometrico().ObtenerTodos(CustodioSelect.ID_EMPLEADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();
                        //SE ENCONTRO CUSTODIO
                        CambiarMensajeNIP(enumMensajeNIP.ENCONTRADO);
                        NIPCapturaVisible = false;
                    }
                    else
                    {
                        //NO SE ENCONTRO CUSTODIO
                        CambiarMensajeNIP(enumMensajeNIP.NO_ENCONTRADO);
                    }
                }
                else
                {
                    //NIP VIENE VACIO
                    CambiarMensajeNIP(enumMensajeNIP.NO_ENCONTRADO);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener información.", ex);
            }
        }

        /// <summary>
        /// Muestra el mensaje de resultado de la comparación del ID del custodio.
        /// </summary>
        /// <param name="TipoMensaje">Resultado que indica el tipo de mensaje a mostrar.</param>
        public async void CambiarMensajeNIP(enumMensajeNIP TipoMensaje)
        {
            switch (TipoMensaje)
            {
                case enumMensajeNIP.ENCONTRADO:
                    MarkCheck = "\u2713 \u2713";
                    MensajeNipAprobacion = "Custodio encontrado";
                    //ImagenEvaluacion = new Imagenes().getImagenPermitido();
                    ColorNIPAprobacion = new SolidColorBrush(Colors.Green);
                    break;
                case enumMensajeNIP.NO_ENCONTRADO:
                    MarkCheck = "X";
                    ColorNIPAprobacion = new SolidColorBrush(Colors.Red);
                    MensajeNipAprobacion = "Custodio no encontrado";
                    break;
            }
            await TaskEx.Delay(2000);
            MarkCheck = "🔍";
            ColorNIPAprobacion = new SolidColorBrush(Colors.DarkBlue);
            BuscarNIP = string.Empty;
            MensajeNipAprobacion = "Capture NIP";
            ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
        }

        public override void OnCaptured(DPUruNet.CaptureResult captureResult)
        {
            if (ScannerMessage.Contains("Procesando..."))
                return;

            ShowLoadingCustodio = Visibility.Visible;
            ShowLinea = Visibility.Visible;
            CapturarShow = Visibility.Collapsed;

            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ShowLinea = Visibility.Visible;
                ScannerMessage = "Procesando...";
                ColorMensaje = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
            }));
            base.OnCaptured(captureResult);

            switch (Buscar)
            {
                case enumTipoPersona.PERSONA_EMPLEADO:
                    CompararHuellaCustodio();
                    break;
                default:
                    break;
            }
            GuardandoHuellas = true;
            ShowLoadingCustodio = Visibility.Collapsed;
            CapturarShow = Conect ? Visibility.Visible : Visibility.Collapsed;
            ShowLinea = Visibility.Collapsed;
        }

        //HUELLA PARA LOS CUSTODIOS
        private Task<bool> CompararHuellaCustodio(byte[] Huella = null, enumTipoBiometrico? Finger = null)
        {
            try
            {
                var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null ?? Huella;
                var verifyFinger = Finger ?? (DD_DedoHuella.HasValue ? DD_DedoHuella.Value : enumTipoBiometrico.INDICE_DERECHO);
                ImagenCustodio = null;
                ListResultadoCustodio = null;
                TextoRegistroEdificioCustodio = null;

                if (bytesHuella == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (Finger == null)
                            ScannerMessage = "Vuelve a capturar las huellas";
                        else
                            ScannerMessage = "Siguiente Huella";
                        FocusAceptarBusquedaHuella = true;
                        ColorMensaje = new SolidColorBrush(Colors.DarkOrange);
                        ShowLinea = Visibility.Collapsed;
                    }));
                }
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Procesando...";
                    ColorMensaje = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                    FocusAceptarBusquedaHuella = false;
                }));
                var Service = new BiometricoServiceClient();
                if (Service == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Error en el servicio de comparación";
                        FocusAceptarBusquedaHuella = true;
                        ColorMensaje = new SolidColorBrush(Colors.Red);
                        ShowLinea = Visibility.Collapsed;
                    }));
                }
                var CompareResult = Service.CompararHuellaPersona(new ComparationRequest { BIOMETRICO = bytesHuella, ID_TIPO_BIOMETRICO = verifyFinger, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, ID_TIPO_PERSONA = enumTipoPersona.PERSONA_EMPLEADO, ID_CENTRO = GlobalVar.gCentro });
                //var CompareResult = Service.CompararHuellaPersona(new ComparationRequest { BIOMETRICO = bytesHuella, ID_TIPO_BIOMETRICO = verifyFinger, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, ID_TIPO_PERSONA = enumTipoPersona.PERSONA_EMPLEADO });

                if (CompareResult.Identify)
                {
                    ListResultadoCustodio = ListResultadoCustodio ?? new List<ResultadoBusquedaBiometricoCustodio>();

                    if (CompareResult.Result.Count() > 1)
                    {
                        TextoRegistroEdificioCustodio = "NOTA: Se encontró más de una coincidencia, por favor intente de nuevo";
                        ScannerMessage = "";
                        return TaskEx.FromResult(false);
                    }
                    else
                    {
                        foreach (var item in CompareResult.Result)
                        {
                            var custodio = new cPersonaBiometrico().GetData().Where(w => w.ID_PERSONA == item.ID_PERSONA && (w.ID_TIPO_BIOMETRICO == (DD_DedoHuella.HasValue ? (short)DD_DedoHuella.Value : (short)enumTipoBiometrico.INDICE_DERECHO) && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)).OrderBy(o => o.ID_PERSONA).FirstOrDefault();

                            ShowContinue = Visibility.Collapsed;
                            if (custodio == null)
                                continue;

                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                var custodio_biometrico = custodio.PERSONA;
                                var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());

                                if (custodio_biometrico != null)
                                    if (custodio_biometrico.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                        FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(custodio_biometrico.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                    else
                                        if (custodio_biometrico.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                            FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(custodio_biometrico.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);

                                ListResultadoCustodio.Add(new ResultadoBusquedaBiometricoCustodio()
                                {
                                    IdPersona = custodio.PERSONA.ID_PERSONA,
                                    Nombre = string.IsNullOrEmpty(custodio.PERSONA.NOMBRE) ? string.Empty : custodio.PERSONA.NOMBRE.TrimEnd(),
                                    CPaterno = string.IsNullOrEmpty(custodio.PERSONA.PATERNO) ? string.Empty : custodio.PERSONA.PATERNO.TrimEnd(),
                                    CMaterno = string.IsNullOrEmpty(custodio.PERSONA.MATERNO) ? string.Empty : custodio.PERSONA.MATERNO.TrimEnd(),
                                    ENCONTRADO = true,
                                    Persona = custodio.PERSONA
                                });
                            }));
                            ImagenCustodio = new cPersonaBiometrico().ObtenerTodos(custodio.ID_PERSONA, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ? new cPersonaBiometrico().ObtenerTodos(custodio.ID_PERSONA, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();
                        }
                    }

                    foreach (var llenar in ListResultadoCustodio)
                    {
                        var igual = ListaCustodio.Where(w => w.IdPersona == llenar.IdPersona).Count();

                        if (igual == 0)
                        {
                            ListaCustodio.Add(llenar);
                        }
                        else
                        {
                            llenar.ENCONTRADO = false;
                        }
                        ListResultadoCustodio = new List<ResultadoBusquedaBiometricoCustodio>(ListResultadoCustodio);
                        ShowContinue = Visibility.Collapsed;

                        if (ListResultadoCustodio.Any())
                        {
                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                if (igual > 0)
                                {
                                    ScannerMessage = "Registro repetido";
                                    FocusAceptarBusquedaHuella = true;
                                    ColorMensaje = new SolidColorBrush(Colors.Orange);
                                }
                                else if (!CancelKeepSearching)
                                {
                                    ScannerMessage = "Registro encontrado";
                                    FocusAceptarBusquedaHuella = true;
                                    ColorMensaje = new SolidColorBrush(Colors.Green);
                                }
                            }));
                            if (Finger != null)
                                Service.Close();

                            return TaskEx.FromResult(false);
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
                            ColorMensaje = new SolidColorBrush(Colors.Red);
                            FocusAceptarBusquedaHuella = true;
                            ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
                        }
                    }));
                    IsSucceded = false;
                    if (!CancelKeepSearching)
                    {
                        SelectedRegistroCustodio = null;
                    }
                    PropertyImage = null;
                }
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar búsqueda", ex);
                return TaskEx.FromResult(false);
            }
        }

        private void Aceptar(Window Window)
        {
            if (ScannerMessage.Contains("Procesando..."))
                return;

            IsSucceded = true;
            Window.Close();
        }

        private async Task WaitForFingerPrints()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!GuardandoHuellas) ;
            });
        }
        #endregion

        #region [CLASE PARA CUSTODIOS]
        public class ResultadoBusquedaBiometricoCustodio
        {
            public int IdPersona { get; set; }
            public string CPaterno { get; set; }
            public string CMaterno { get; set; }
            public string Nombre { get; set; }
            public bool ENCONTRADO { get; set; }
            public ImageSource Foto { get; set; }
            public SSP.Servidor.PERSONA Persona { get; set; }
        }
        #endregion
    }
}