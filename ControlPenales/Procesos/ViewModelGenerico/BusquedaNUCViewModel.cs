using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Oracle.ManagedDataAccess.Client;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ControlPenales
{
    public class BusquedaNUCViewModel : FingerPrintScanner
    {
        #region [Constructores]
        public BusquedaNUCViewModel()
        {
            ImagenInterconexion = new Imagenes().getImagenPerson();
            BuscarHuellaVisible = false;
            TextoBoton = "Crear Nuevo Expediente";
        }
        #endregion
        #region [Comandos]
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BuscarNUCView>(OnLoad); }
        }

        public ICommand CommandAceptar
        {
            get { return new DelegateCommand<BuscarNUCView>(Aceptar); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }
        #endregion
        #region [Propiedades]
        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }

        private Brush _ColorMessage;
        public Brush ColorMessage
        {
            get { return _ColorMessage; }
            set
            {
                _ColorMessage = value;
                RaisePropertyChanged("ColorMessage");
            }
        }

        private ImageSource _ImputadoFoto;
        public ImageSource ImputadoFoto
        {
            get { return _ImputadoFoto; }
            set
            {
                _ImputadoFoto = value;
                OnPropertyChanged("ImputadoFoto");
            }
        }

        private IMPUTADO _Imputado;
        public IMPUTADO Imputado
        {
            get { return _Imputado; }
            set
            {
                _Imputado = value;
                OnPropertyChanged();
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

        public string Expediente
        {
            get { return Imputado != null ? (Imputado.ID_ANIO + "/" + Imputado.ID_IMPUTADO) : string.Empty; }
            set { OnPropertyChanged("Expediente"); }
        }

        public string NIP
        {
            get { return Imputado != null ? Imputado.NIP.ToString() : string.Empty; }
            set { OnPropertyChanged("NIP"); }
        }

        public string APaterno
        {
            get { return Imputado != null ? Imputado.PATERNO.Trim() : string.Empty; }
            set { OnPropertyChanged("APaterno"); }
        }

        public string AMaterno
        {
            get { return Imputado != null ? Imputado.MATERNO.Trim() : string.Empty; }
            set { OnPropertyChanged("AMaterno"); }
        }

        public string Nombre
        {
            get { return Imputado != null ? Imputado.NOMBRE.Trim() : string.Empty; }
            set { OnPropertyChanged("Nombre"); }
        }

        private string textoBoton;
        public string TextoBoton
        {
            get { return textoBoton; }
            set { textoBoton = value; OnPropertyChanged("TextoBoton"); }
        }

        private bool aceptarEnabled = true;
        public bool AceptarEnabled
        {
            get { return aceptarEnabled; }
            set { aceptarEnabled = value; OnPropertyChanged("AceptarEnabled"); }
        }
        //INTERCONEXION
        private string nuc;
        public string Nuc
        {
            get { return nuc; }
            set { nuc = value; OnPropertyChanged("Nuc"); }
        }

        private VM_IMPUTADOSDATOS selectedInterconexion;
        public VM_IMPUTADOSDATOS SelectedInterconexion
        {
            get { return selectedInterconexion; }
            set
            {
                selectedInterconexion = value;
                if (selectedInterconexion != null)
                {
                    if (selectedInterconexion.FOTO != null)
                    {
                        ImagenInterconexion = selectedInterconexion.FOTO;
                    }
                    else
                        ImagenInterconexion = new Imagenes().getImagenPerson();
                    BuscarHuellaVisible = true;
                }
                else
                    ImagenInterconexion = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedInterconexion");
            }
        }

        private byte[] imagenInterconexion;
        public byte[] ImagenInterconexion
        {
            get { return imagenInterconexion; }
            set
            {
                imagenInterconexion = value;
                OnPropertyChanged("ImagenInterconexion");
            }
        }

        private ObservableCollection<VM_IMPUTADOSDATOS> lstInterconexion;
        public ObservableCollection<VM_IMPUTADOSDATOS> LstInterconexion
        {
            get { return lstInterconexion; }
            set { lstInterconexion = value; OnPropertyChanged("LstInterconexion"); }
        }

        private bool buscarHuellaVisible;
        public bool BuscarHuellaVisible
        {
            get { return buscarHuellaVisible; }
            set { buscarHuellaVisible = value; OnPropertyChanged("BuscarHuellaVisible"); }
        }

        private bool GuardandoHuellas { get; set; }

        private ResultadoBusquedaBiometrico _SelectRegistro;
        public ResultadoBusquedaBiometrico SelectRegistro
        {
            get { return _SelectRegistro; }
            set
            {
                _SelectRegistro = value;
                if (value != null)
                {
                    if (value.Imputado != null)
                    {
                        if (value.Imputado.INGRESO != null)
                        {
                            var ing = value.Imputado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            if (ing != null)
                            {
                                if (ing.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                                    mensajeAlerta();    
                            }
                        }
                    }
                }
                OnPropertyChanged("SelectRegistro");
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

        private Visibility _ShowSearch = Visibility.Collapsed;
        public Visibility ShowSearch
        {
            get { return _ShowSearch; }
            set
            {
                _ShowSearch = value;
                OnPropertyChanged("ShowSearch");
            }
        }

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

        private bool Conectado;
        private bool CancelKeepSearching { get; set; }
        private bool isKeepSearching { get; set; }

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
        #endregion
        #region [Metodos]
        private void OnLoad(BuscarNUCView Window)
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

            CompareImputado();

            GuardandoHuellas = true;
            ShowLoading = Visibility.Collapsed;
            ShowCapturar = Conectado ? Visibility.Visible : Visibility.Collapsed;
            ShowLine = Visibility.Collapsed;
        }

        private async void Aceptar(BuscarNUCView Window)
        {
            AceptarEnabled = false;
            if (ScannerMessage.Contains("Procesando..."))
            {
                AceptarEnabled = true;
                return; 
            }

            if (SelectedInterconexion != null)
            {
                if (SelectRegistro != null)
                    Imputado = SelectRegistro.Imputado;
                MediaFiliacion();
                _IsSucceed = true;
                CancelKeepSearching = true;
                isKeepSearching = true;
                if (SelectRegistro != null)
                    await WaitForFingerPrints();
                Window.Close();

            }
            else
                mensajeAlerta("Validación","Favor de seleccionar un NUC.");
            AceptarEnabled = true;
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

        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar_nuc":
                    BuscarNUCInterconexion();
                    break;
                case "buscar_nuevo":
                    LimpiarBusqueda();
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            var tb = (TextBox)obj;
            if (tb != null)
            {
                Nuc = tb.Text;
                BuscarNUCInterconexion();
            }
        }

        /// <summary>
        /// Buscamos un NUC en las vistas materializadas de la Procuraduria
        /// </summary>
        private async void BuscarNUCInterconexion()
        {
            bool Error = false;
                ShowSearch = Visibility.Visible;
                await Task.Factory.StartNew(() =>
                    {
                        try{
                            if (!string.IsNullOrEmpty(Nuc))
                            {
                                LstInterconexion = new ObservableCollection<VM_IMPUTADOSDATOS>();
                                using (EntidadInterconexion contexto = new EntidadInterconexion())
                                {
                                    LstInterconexion = new ObservableCollection<VM_IMPUTADOSDATOS>(contexto.Database.SqlQuery<VM_IMPUTADOSDATOS>("SELECT * FROM SSP.VM_IMPUTADOSDATOS WHERE EXPEDIENTEID LIKE '1%' AND EXPEDIENTEID LIKE '%'||:param1 ", new OracleParameter("param1", Nuc)));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Error = true;
                        }
                    });
                ShowSearch = Visibility.Collapsed;
            if(Error)
                mensajeAlerta("Algo pasó...","Ocurrió un error al consultar interconexión, verifique su conexión.");
        }

        private void LimpiarBusqueda()
        {
            Nuc = null;
            LstInterconexion = new ObservableCollection<VM_IMPUTADOSDATOS>();
            ImagenInterconexion = new Imagenes().getImagenPerson();
            LimpiarCampos();
            BuscarHuellaVisible = false;
            SelectedInterconexion = null;
            Imputado = null;
        }

        private Task<bool> CompareImputado(byte[] Huella = null, enumTipoBiometrico? Finger = null)
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
            var CompareResult = Service.CompararHuellaImputado(new ComparationRequest { BIOMETRICO = bytesHuella, ID_TIPO_BIOMETRICO = verifyFinger, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP });

            if (CompareResult.Identify)
            {
                ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();

                foreach (var item in CompareResult.Result)
                {
                    var imputado = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == item.ID_ANIO && w.ID_CENTRO == item.ID_CENTRO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_TIPO_BIOMETRICO == (DD_Dedo.HasValue ? (short)DD_Dedo.Value : (short)enumTipoBiometrico.INDICE_DERECHO) && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP).FirstOrDefault();

                    if (imputado == null)
                    {
                        ScannerMessage = "Registro no encontrado";
                        TextoBoton = "Crear Nuevo Expediente";
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        return TaskEx.FromResult(false);
                    }

                    Imputado = imputado.IMPUTADO;

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
                            NIP = imputado.IMPUTADO.NIP.ToString(),
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
        #endregion

        private ObservableCollection<cMF> lstMediaFiliacion;
        public ObservableCollection<cMF> LstMediaFiliacion
        {
            get { return lstMediaFiliacion; }
            set { lstMediaFiliacion = value; OnPropertyChanged("LstMediaFiliacion"); }
        }

        private ObservableCollection<IMPUTADO_FILIACION> lstImputadoFiliacion;
        public ObservableCollection<IMPUTADO_FILIACION> LstImputadoFiliacion
        {
            get { return lstImputadoFiliacion; }
            set { lstImputadoFiliacion = value; OnPropertyChanged("LstImputadoFiliacion"); }
        }

        void MediaFiliacion()
        {
            if (SelectedInterconexion != null)
            {
                using (EntidadInterconexion contexto = new EntidadInterconexion())
                {
                    LstMediaFiliacion = new ObservableCollection<cMF>(contexto.Database.SqlQuery<cMF>("SELECT * FROM SSP.VM_IMPUTADOSFILIACION WHERE EXPEDIENTEid= :param1 AND PERSONAFISICAID = :param2 ", new OracleParameter("param1", SelectedInterconexion.EXPEDIENTEID), new OracleParameter("param2", SelectedInterconexion.PERSONAFISICAID)));
                    if (LstMediaFiliacion != null)
                    {
                        foreach (var mf in LstMediaFiliacion)
                        {
                            if (LstImputadoFiliacion == null)
                                LstImputadoFiliacion = new ObservableCollection<IMPUTADO_FILIACION>();

                            if (mf.CEJACOLOCACIONID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 12,
                                    ID_TIPO_FILIACION = (short)mf.CEJACOLOCACIONID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.CEJAFORMAID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 14,
                                    ID_TIPO_FILIACION = (short)mf.CEJAFORMAID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.CEJAGROSORID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 13,
                                    ID_TIPO_FILIACION = (short)mf.CEJAGROSORID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.CEJATAMANOID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 15,
                                    ID_TIPO_FILIACION = (short)mf.CEJATAMANOID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.PIELCOLORID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 30,
                                    ID_TIPO_FILIACION = (short)mf.PIELCOLORID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.CABELLOESTILOID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 11,
                                    ID_TIPO_FILIACION = (short)mf.CABELLOESTILOID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.CABELLOCOLORID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 9,
                                    ID_TIPO_FILIACION = (short)mf.CABELLOCOLORID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.FRENTEALTURAID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 27,
                                    ID_TIPO_FILIACION = (short)mf.FRENTEALTURAID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.FRENTEANCHURAID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 29,
                                    ID_TIPO_FILIACION = (short)mf.FRENTEANCHURAID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.OJOFORMAID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 17,
                                    ID_TIPO_FILIACION = (short)mf.OJOFORMAID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.OJOTAMANOID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 18,
                                    ID_TIPO_FILIACION = (short)mf.OJOTAMANOID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.NARIZTIPOID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 3,
                                    ID_TIPO_FILIACION = (short)mf.NARIZTIPOID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.NARIZBASEID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 5,
                                    ID_TIPO_FILIACION = (short)mf.NARIZBASEID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.BOCATAMANOID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 19,
                                    ID_TIPO_FILIACION = (short)mf.BOCATAMANOID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.BOCAPECULIARID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 20,
                                    ID_TIPO_FILIACION = (short)mf.BOCAPECULIARID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.LABIOLONGITUDID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 32,
                                    ID_TIPO_FILIACION = (short)mf.LABIOLONGITUDID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.LABIOGROSORID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 21,
                                    ID_TIPO_FILIACION = (short)mf.LABIOGROSORID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.LABIOPECULIARID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 33,
                                    ID_TIPO_FILIACION = (short)mf.LABIOPECULIARID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.BARBILLAINCLINACIONID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 26,
                                    ID_TIPO_FILIACION = (short)mf.BARBILLAINCLINACIONID,
                                    //FEC_CAPTURA = DateTime.Now
                                });

                            if (mf.BARBILLAFORMAID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 25,
                                    ID_TIPO_FILIACION = (short)mf.BARBILLAFORMAID,
                                    //FEC_CAPTURA = DateTime.Now
                                });


                            if (mf.OREJAFORMAID != null)
                                LstImputadoFiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_MEDIA_FILIACION = 34,
                                    ID_TIPO_FILIACION = (short)mf.OREJAFORMAID,
                                    //FEC_CAPTURA = DateTime.Now
                                });
                        }
                    }
                }
            }
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
    }
}
