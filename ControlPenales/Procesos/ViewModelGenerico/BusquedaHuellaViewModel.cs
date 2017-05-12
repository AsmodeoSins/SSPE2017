using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ControlPenales
{
    public class BusquedaHuellaViewModel : FingerPrintScanner
    {
        #region [Constructores]
        public BusquedaHuellaViewModel(enumTipoPersona tipobusqueda, bool? set442 = null, bool GuardarHuellas = false)
        {
            BuscarPor = tipobusqueda;
            Conectado = set442.HasValue ? set442.Value : false;
            ShowCapturar = set442.HasValue ? set442.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
            _GuardarHuellas = GuardarHuellas;
            switch (tipobusqueda)
            {
                case enumTipoPersona.IMPUTADO:
                    CabeceraBusqueda = "Datos del Imputado";
                    CabeceraFoto = "Foto Imputado";
                    break;
                case enumTipoPersona.PERSONA_VISITA:
                case enumTipoPersona.PERSONA_EXTERNA:
                    CabeceraBusqueda = "Datos de la Persona";
                    CabeceraFoto = "Foto Persona";
                    break;
                case enumTipoPersona.PERSONA_ABOGADO:
                    CabeceraBusqueda = "Datos del Abogado";
                    CabeceraFoto = "Foto Abogado";
                    break;
                case enumTipoPersona.PERSONA_EMPLEADO:
                    CabeceraBusqueda = "Datos del Empleado";
                    CabeceraFoto = "Foto Empleado";
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region [Comandos]
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BusquedaHuella>(OnLoad); }
        }

        public ICommand CommandAceptar
        {
            get { return new DelegateCommand<Window>(Aceptar); }
        }

        public ICommand CommandOpem442
        {
            get { return new DelegateCommand<string>(Capture); }
        }

        public ICommand CommandContinue
        {
            get { return new DelegateCommand<string>((s) => { isKeepSearching = s == "True" ? true : false; }); }
        }
        #endregion

        #region [Propiedades]
        private bool Conectado;
        public enumTipoPersona BuscarPor { get; set; }

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

        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }

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
                OnPropertyChanged("SelectRegistro");
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

        #endregion

        #region [Metodos]
        private void OnLoad(BusquedaHuella Window)
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
            #endregion

            Window.Closed += (s, e) =>
            {
                try
                {
                    if (OnProgress == null)
                        return;

                    if (!_IsSucceed)
                        SelectRegistro = null;

                    OnProgress.Abort();
                    CancelCaptureAndCloseReader(OnCaptured);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar busqueda", ex);
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
                ColorMessage = new SolidColorBrush(Colors.Green);
            }));
            GuardandoHuellas = true;
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

            ListResultado = null;

            switch (BuscarPor)
            {
                case enumTipoPersona.IMPUTADO:
                    CompareImputado();
                    break;
                case enumTipoPersona.PERSONA_TODOS:
                case enumTipoPersona.PERSONA_VISITA:
                case enumTipoPersona.PERSONA_ABOGADO:
                case enumTipoPersona.PERSONA_EMPLEADO:
                case enumTipoPersona.PERSONA_EXTERNA:
                    ComparePersona();
                    break;
                default:
                    break;
            }

            GuardandoHuellas = true;
            ShowLoading = Visibility.Collapsed;
            ShowCapturar = Conectado ? Visibility.Visible : Visibility.Collapsed;
            ShowLine = Visibility.Collapsed;
        }

        private async void Aceptar(Window Window)
        {
            if (ScannerMessage.Contains("Procesando..."))
                return;
            CancelKeepSearching = true;
            isKeepSearching = true;
            await WaitForFingerPrints();
            _IsSucceed = true;
            Window.Close();
        }

        private async void Capture(string obj)
        {
            ShowLoading = Visibility.Visible;
            ShowLine = Visibility.Visible;
            var nRet = -1;
            try
            {
                CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);
                ShowCapturar = Visibility.Collapsed;

                #region [Huellas]
                if (nRet == 0)
                {
                    ScannerMessage = "Procesando...";
                    ShowLine = Visibility.Visible;
                    ListResultado = null;
                    HuellasCapturadas = new List<PlantillaBiometrico>();

                    for (short i = 1; i <= 10; i++)
                    {
                        var pBuffer = IntPtr.Zero;
                        var nBufferLength = 0;
                        var nNFIQ = 0;
                        ListResultado = null;
                        GuardandoHuellas = false;

                        CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                        var bufferBMP = new byte[nBufferLength];
                        if (pBuffer != IntPtr.Zero)
                            Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);

                        CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                        var bufferWSQ = new byte[nBufferLength];
                        if (pBuffer != IntPtr.Zero)
                            Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);

                        CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);

                        Fmd FMD = null;
                        if (bufferBMP.Length != 0)
                        {
                            PropertyImage = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                            FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                        }

                        ShowContinuar = Visibility.Collapsed;
                        await TaskEx.Delay(1);

                        switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                        {
                            #region [Pulgar Derecho]
                            case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO);
                                break;
                            #endregion
                            #region [Indice Derecho]
                            case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_DERECHO);
                                break;
                            #endregion
                            #region [Medio Derecho]
                            case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_DERECHO);
                                break;
                            #endregion
                            #region [Anular Derecho]
                            case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_DERECHO);
                                break;
                            #endregion
                            #region [Meñique Derecho]
                            case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_DERECHO);
                                break;
                            #endregion
                            #region [Pulgar Izquierdo]
                            case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.PULGAR_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO);
                                break;
                            #endregion
                            #region [Indice Izquierdo]
                            case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_IZQUIERDO);
                                break;
                            #endregion
                            #region [Medio Izquierdo]
                            case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_IZQUIERDO);
                                break;
                            #endregion
                            #region [Anular Izquierdo]
                            case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_IZQUIERDO);
                                break;
                            #endregion
                            #region [Meñique Izquierdo]
                            case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_IZQUIERDO);
                                isKeepSearching = true;
                                break;
                            #endregion
                            default:
                                break;
                        }

                        ShowContinuar = Visibility.Visible;
                        ShowCapturar = Visibility.Collapsed;

                        if (!CancelKeepSearching)
                            await KeepSearch();
                        else
                            if (!_GuardarHuellas)
                                break;
                    }

                    GuardandoHuellas = true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Vuelve a capturar las huellas";
                        ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                    }));
                }
                #endregion
            }
            catch
            {
                CLSFPCaptureDllWrapper.CLS_Terminate();
            }

            if (nRet == 0)
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Busqueda Terminada";
                    ColorMessage = new SolidColorBrush(Colors.Green);
                    AceptarBusquedaHuellaFocus = true;
                }));

            ShowLine = Visibility.Collapsed;
            ShowLoading = Visibility.Collapsed;
            ShowContinuar = Visibility.Collapsed;
            await TaskEx.Delay(1500);
            ShowCapturar = Visibility.Visible;

        }

        private void LimpiarCampos()
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ScannerMessage = "Capture Huella";
                ColorMessage = new SolidColorBrush(Colors.Green);
                AceptarBusquedaHuellaFocus = true;
            }));
            _SelectRegistro = null;
            PropertyImage = null;
        }

        private Task<bool> CompareImputado(byte[] Huella = null, enumTipoBiometrico? Finger = null)
        {
            try
            {
                var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null ?? Huella;
                var verifyFinger = Finger ?? (DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO);

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
                        ScannerMessage = "Error en el servicio de comparacion";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        ShowLine = Visibility.Collapsed;
                    }));
                }
                CompareResponseImputado CompareResult = null;

                //var res = Service.CompararHuellaImputadoPrueba(new ComparationRequest { BIOMETRICO = bytesHuella, ID_TIPO_BIOMETRICO = verifyFinger, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP });

                CompareResult = Service.CompararHuellaImputado(new ComparationRequest { BIOMETRICO = bytesHuella, ID_TIPO_BIOMETRICO = verifyFinger, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP });

                if (CompareResult.Identify)
                {
                    ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();

                    foreach (var item in CompareResult.Result)
                    {
                        var imputado = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == item.ID_ANIO && w.ID_CENTRO == item.ID_CENTRO && w.ID_IMPUTADO == item.ID_IMPUTADO && (w.ID_TIPO_BIOMETRICO == (DD_Dedo.HasValue ? (short)DD_Dedo.Value : (short)enumTipoBiometrico.INDICE_DERECHO) && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)).FirstOrDefault();


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
                                Nombre = imputado.IMPUTADO.NOMBRE,
                                APaterno = imputado.IMPUTADO.PATERNO,
                                AMaterno = imputado.IMPUTADO.MATERNO,
                                Expediente = imputado.ID_ANIO + "/" + imputado.ID_IMPUTADO,
                                NIP = imputado.IMPUTADO.NIP,
                                Foto = FotoBusquedaHuella,
                                Imputado = imputado.IMPUTADO
                            });

                        }));
                    }

                    ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);

                    ShowContinuar = Visibility.Collapsed;

                    if (ListResultado.Any())
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            if (!CancelKeepSearching)
                            {
                                ScannerMessage = "Registro encontrado";
                                AceptarBusquedaHuellaFocus = true;
                                ColorMessage = new SolidColorBrush(Colors.Green);
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
                            }
                        }));
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
                        }
                    }));
                    _IsSucceed = false;
                    if (!CancelKeepSearching)
                    {
                        _SelectRegistro = null;
                    }
                    PropertyImage = null;
                }

                Service.Close();
                FingerPrintData = null;

                return TaskEx.FromResult(true);
            }
            catch (Exception ex) 
            {
                ScannerMessage = "";
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error...", ex);
            }
            return TaskEx.FromResult(false);
        }

        private Task<bool> ComparePersona(byte[] Huella = null, enumTipoBiometrico? Finger = null)
        {
            var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null ?? Huella;
            var verifyFinger = Finger ?? (DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO);

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
                    ScannerMessage = "Error en el servicio de comparacion";
                    AceptarBusquedaHuellaFocus = true;
                    ColorMessage = new SolidColorBrush(Colors.Red);
                    ShowLine = Visibility.Collapsed;
                }));
            }

            var CompareResult = Service.CompararHuellaPersona(new ComparationRequest
            {
                BIOMETRICO = bytesHuella,
                ID_TIPO_BIOMETRICO = verifyFinger,
                ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP,
                ID_TIPO_PERSONA = BuscarPor == enumTipoPersona.PERSONA_TODOS ? new Nullable<enumTipoPersona>() : BuscarPor
            });

            if (CompareResult.Identify)
            {
                ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();

                foreach (var item in CompareResult.Result)
                {
                    var persona = new cPersonaBiometrico().GetData().Where(w => w.ID_PERSONA == item.ID_PERSONA && (w.ID_TIPO_BIOMETRICO == (DD_Dedo.HasValue ? (short)DD_Dedo.Value : (short)enumTipoBiometrico.INDICE_DERECHO)) && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP).FirstOrDefault();

                    ShowContinuar = Visibility.Collapsed;
                    if (persona == null)
                        continue;

                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        var perosonabiometrico = new cPersonaBiometrico().GetData().Where(w => w.ID_PERSONA == persona.ID_PERSONA).ToList();
                        var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());

                        if (perosonabiometrico != null)
                            if (perosonabiometrico.Any())
                                if (perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                else
                                    if (perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                        FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);

                        ListResultado.Add(new ResultadoBusquedaBiometrico()
                        {
                            Nombre = persona.PERSONA.NOMBRE,
                            APaterno = persona.PERSONA.PATERNO,
                            AMaterno = persona.PERSONA.MATERNO,
                            Expediente = persona.PERSONA.ID_PERSONA.ToString(),
                            NIP = persona.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).Any() ?
                                persona.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().NIP.HasValue
                                    ? persona.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().NIP.Value.ToString()
                                        : string.Empty : string.Empty,
                            Foto = FotoBusquedaHuella,
                            Persona = persona.PERSONA
                        });

                    }));
                }

                ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);

                ShowContinuar = Visibility.Collapsed;

                if (ListResultado.Any())
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (!CancelKeepSearching)
                        {
                            ScannerMessage = "Registro encontrado";
                            AceptarBusquedaHuellaFocus = true;
                            ColorMessage = new SolidColorBrush(Colors.Green);
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
                        }
                    }));
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
                    }
                }));
                _IsSucceed = false;
                if (!CancelKeepSearching)
                {
                    _SelectRegistro = null;
                }
                PropertyImage = null;
            }

            Service.Close();
            FingerPrintData = null;

            return TaskEx.FromResult(true);
        }

        private async Task KeepSearch()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!isKeepSearching) ;
            });
            isKeepSearching = false;
        }

        private async Task WaitForFingerPrints()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!GuardandoHuellas) ;
            });
        }

        public async void mensajeAlerta(string titulo = "Advertencia", string mensaje = "Este imputado esta activo en el centro.")
        {
            var metro = Application.Current.Windows[(Application.Current.Windows.Count - 1)] as MetroWindow;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Cerrar"
            };
            if (metro != null)
                await metro.ShowMessageAsync(titulo, mensaje, MessageDialogStyle.Affirmative, mySettings);
        }
        #endregion
    }

    public class ResultadoBusquedaBiometrico
    {
        public string Expediente { get; set; }
        public string NIP { get; set; }
        public string APaterno { get; set; }
        public string AMaterno { get; set; }
        public string Nombre { get; set; }
        public ImageSource Foto { get; set; }
        public IMPUTADO Imputado { get; set; }
        public SSP.Servidor.PERSONA Persona { get; set; }
    }
}
