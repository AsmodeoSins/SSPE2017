using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using Oracle.ManagedDataAccess.Client;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlPenales
{
    public class BusquedaNUCVM : FingerPrintScanner
    {
        #region [Constructores]
        public BusquedaNUCVM()
        {
            ImagenInterconexion = new Imagenes().getImagenPerson();
            BuscarHuellaVisible = false;
            TextoBoton = "Crear Nuevo Expediente";
        }
        #endregion
        #region [Comandos]
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BuscarNUC>(OnLoad); }
        }

        public ICommand CommandAceptar
        {
            get { return new DelegateCommand<BuscarNUC>(Aceptar); }
        }

        public ICommand CommandCerrar
        {
            get { return new DelegateCommand<BuscarNUC>(Cerrar); }
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
                    SelectedNUC = selectedInterconexion.EXPEDIENTEID.ToString();
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

        private string selectedNUC;
        public string SelectedNUC
        {
            get { return selectedNUC; }
            set { selectedNUC = value; OnPropertyChanged("SelectedNUC"); }
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
        #endregion
        #region [Metodos]
        private void OnLoad(BuscarNUC Window)
        {
            Window.Closed += (s, e) =>
            {
                try
                {
                    if (OnProgress == null)
                        return;

                    if (!_IsSucceed)
                        Imputado = null;

                    OnProgress.Abort();
                    CancelCaptureAndCloseReader(OnCaptured);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar buscar por nuc", ex);
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
        }

        public override void OnCaptured(DPUruNet.CaptureResult captureResult)
        {
            if (ScannerMessage == "Procesando... ")
                return;
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
               {
                   ScannerMessage = "Procesando...";
                   ColorMessage = new SolidColorBrush(Color.FromRgb(51, 115, 242));
               }));
            base.OnCaptured(captureResult);
            Compare();
        }

        private void Compare(object Huella = null)
        {
            if (FingerPrintData == null)
            {
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
               {
                   ScannerMessage = "Vuelve a capturar la huella";
                   TextoBoton = "Crear Nuevo Expediente";
                   ColorMessage = new SolidColorBrush(Colors.DarkOrange);
               }));
                return;
            }
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
               {
                   ScannerMessage = "Procesando... ";
                   ColorMessage = new SolidColorBrush(Color.FromRgb(51, 115, 242));
                   AceptarBusquedaHuellaFocus = false;
               }));

            var Service = new BiometricoServiceClient();
            var CompareResult = Service.CompararHuellaImputado(new ComparationRequest { BIOMETRICO = FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes, ID_TIPO_BIOMETRICO = (DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO), ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP });

            if (CompareResult.Identify)
            {
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    var item = CompareResult.Result.FirstOrDefault();
                    var imputado = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == item.ID_ANIO && w.ID_CENTRO == item.ID_CENTRO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_TIPO_BIOMETRICO == (DD_Dedo.HasValue ? (short)DD_Dedo.Value : (short)enumTipoBiometrico.INDICE_DERECHO) && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP).FirstOrDefault();

                    if (imputado == null)
                    {
                        ScannerMessage = "Registro no encontrado";
                        TextoBoton = "Crear Nuevo Expediente";
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        return;
                    }

                    Imputado = imputado.IMPUTADO;

                    var ingresobiometrico = imputado.IMPUTADO.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();

                    if (ingresobiometrico != null)
                        if (ingresobiometrico.INGRESO_BIOMETRICO.Any())
                            if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                ImputadoFoto = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                            else
                                if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    ImputadoFoto = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                else
                                    ImputadoFoto = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                    TextoBoton = "Nuevo Ingreso";
                    ScannerMessage = "Registro encontrado";
                    AceptarBusquedaHuellaFocus = true;
                    ColorMessage = new SolidColorBrush(Colors.Green);
                }));
            }
            else
            {
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
               {
                   TextoBoton = "Crear Nuevo Expediente";
                   ScannerMessage = "Huella no encontrada";
                   ColorMessage = new SolidColorBrush(Colors.Red);
                   AceptarBusquedaHuellaFocus = true;
               }));
                _IsSucceed = false;
                _Imputado = null;
                Nombre = string.Empty;
                APaterno = string.Empty;
                AMaterno = string.Empty;
                Expediente = string.Empty;
                NIP = string.Empty;
                PropertyImage = null;
                ImputadoFoto = null;
            }

            Service.Close();
            FingerPrintData = null;
        }

        private void Aceptar(BuscarNUC Window)
        {
            if (ScannerMessage == "Procesando... ")
                return;
            
            //if (SelectedInterconexion != null)
            //{
                    _IsSucceed = true;
                    Window.Close();
            //}
        }

        private void Cerrar(BuscarNUC Window)
        {
            _IsSucceed = false;
            Window.Close();
        }

        private void LimpiarCampos()
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
               {
                   ScannerMessage = "Capture Huella";
                   ColorMessage = new SolidColorBrush(Colors.Green);
                   AceptarBusquedaHuellaFocus = true;
               }));
            _Imputado = null;
            Nombre = string.Empty;
            APaterno = string.Empty;
            AMaterno = string.Empty;
            Expediente = string.Empty;
            NIP = string.Empty;
            PropertyImage = null;
            ImputadoFoto = null;
        }

        private void clickSwitch(Object obj)
        {
            SelectedNUC = string.Empty;
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
        private void BuscarNUCInterconexion()
        {
            if (!string.IsNullOrEmpty(Nuc))
            {
                LstInterconexion = new ObservableCollection<VM_IMPUTADOSDATOS>();
                using (EntidadInterconexion contexto = new EntidadInterconexion())
                {
                    LstInterconexion = new ObservableCollection<VM_IMPUTADOSDATOS>(contexto.Database.SqlQuery<VM_IMPUTADOSDATOS>("SELECT * FROM SSP.VM_IMPUTADOSDATOS WHERE EXPEDIENTEID LIKE '1%' AND EXPEDIENTEID LIKE '%'||:param1 ", new OracleParameter("param1", Nuc)));
                }
             }
        }

        private void LimpiarBusqueda() {
            Nuc = null;
            LstInterconexion = new ObservableCollection<VM_IMPUTADOSDATOS>();
            ImagenInterconexion = new Imagenes().getImagenPerson();
            LimpiarCampos();
            BuscarHuellaVisible = false;
            SelectedInterconexion = null;
            Imputado = null;
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
        
        
    
    }
}
