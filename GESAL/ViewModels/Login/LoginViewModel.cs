using MVVMShared.Behavior;
using DPUruNet;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biometrico.DigitalPersona;
using MVVMShared.Splasher;
using MVVMShared.Listeners;
using MVVMShared.Commands;
using GESAL.Views.Login;
using GESAL.Models;
using GESAL.BiometricoServiceReference;
using GESAL.Views.Principal;
using GESAL.ViewModels.Principal;
using MVVMShared.ViewModels;
using SSP.Controlador.Catalogo.Justicia;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;
namespace GESAL.ViewModels.Login
{
    public class LoginViewModel : FingerPrintScanner
    {
        #region variables
        private string error;

        public string Error
        {
            get { return error; }
            set { error = value; OnPropertyChanged("Error"); }
        }

        private bool bandError;

        public bool BandError
        {
            get { return bandError; }
            set { bandError = value; OnPropertyChanged("BandError"); }
        }

        private Usuario user;

        public  Usuario User
        {
            get { return user; }
            set { user = value; OnPropertyChanged("User"); }
        }

        private BiometricoServiceReference.enumTipoBiometrico? _DD_Dedo = (BiometricoServiceReference.enumTipoBiometrico)Convert.ToInt16(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings["DedoLoggin"].Value);
        public BiometricoServiceReference.enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set
            {
                _DD_Dedo = value;
                OnPropertyChanged("DD_Dedo");
                GuardarDedoSeleccionado();
            }
        }
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            User = new Usuario();
            BandError = false;
        }
        #endregion

        #region metodos
        private void OnLoad(LoginView Window)
        {
            try
            {
                if (CurrentReader != null)
                {
                    CurrentReader.Dispose();
                    CurrentReader = null;
                }

                CurrentReader = Readers[0];

                if (CurrentReader == null)
                    return;

                Window.Closed += (s, e) =>
                {
                    OnProgress.Abort();
                    CancelCaptureAndCloseReader(OnCaptured);
                };

                if (!OpenReader())
                    Window.Close();

                if (!StartCaptureAsync(OnCaptured))
                    Window.Close();

                OnProgress = new Thread(() => InvokeDelegate(Window));

            }
            catch { }
        }

        private void autentificacion()
        {
            Error = string.Empty;
            BandError = false;
            //SSP.Servidor.GlobalVariables.gUser = GlobalVar.gUsr = User.Username;
            SSP.Servidor.GlobalVariables.gUser = GlobalVar.gUsr = "SYSTEM";
            SSP.Servidor.GlobalVariables.gPass = GlobalVar.gPass = "QUADRO";
            User.Password = "12345";
            if (string.IsNullOrWhiteSpace(user.NombreCompleto))
                user.NombreCompleto = "Salvador Ruiz Guevara";

            if (string.IsNullOrEmpty(User.Username) ? false : string.IsNullOrEmpty(User.Password) ? false : true)
            {
                if (User.Username.Equals(User.Username) && User.Password.Equals("12345"))
                {
                    if (User.Username.Equals("ADMINALIMENTACION"))
                    {
                        User.Almacen_Grupo = "A";
                        user.CENTRO = null;
                        User.ROL = "ADMINISTRADOR CENTRAL";
                    }

                    if (User.Username.Equals("ADMINMEDICAMENTOS"))
                    {
                        User.Almacen_Grupo = "M";
                        user.CENTRO = null;
                        User.ROL = "ADMINISTRADOR CENTRAL";
                    }

                    if (User.Username.Equals("ADMINMEXICALI"))
                    {
                        User.Almacen_Grupo = "";
                        user.CENTRO = 4;
                        User.ROL = "ADMINISTRADOR DE CENTRO";
                    }
                    if (User.Username.Equals("ADMINTIJUANA"))
                    {
                        User.Almacen_Grupo = "";
                        user.CENTRO = 2;
                        User.ROL = "ADMINISTRADOR DE CENTRO";
                    }
                    if (User.Username.Equals("ADMINENSENADA"))
                    {
                        User.Almacen_Grupo = "";
                        user.CENTRO = 5;
                        User.ROL = "ADMINISTRADOR DE CENTRO";
                    }
                    if (User.Username.Equals("adminAP"))
                    {
                        User.Almacen_Grupo = "A";
                        User.ROL = "ALMACENISTA";
                    }

                    if (User.Username.Equals("adminUA"))
                    {
                        User.Almacen_Grupo = "A";
                        user.ROL = "ALMACENISTA";
                    }

                    if (User.Username.Equals("adminAlim"))
                    {
                        User.Almacen_Grupo = "A";
                        user.ROL = "ALMACENISTA";
                    }

                    if (User.Username.Equals("adminMed"))
                    {
                        User.Almacen_Grupo = "M";
                        user.ROL = "ALMACENISTA";
                    }
                        
                    Application.Current.MainWindow.Hide();

                    var _view = new PrincipalView();
                    var _viewModel = new PrincipalViewModel(User, DialogCoordinator.Instance);
                    _view.DataContext = _viewModel;


                    Application.Current.MainWindow.Close();

                    var metro = Application.Current.Windows[0] as MahApps.Metro.Controls.MetroWindow;


                    //Tenemos que cambiar esto a una dependencia

                    //((ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                    //((ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();


                    Splasher.Splash = new GESAL.Views.Login.SplashScreen();
                    Splasher.ShowSplash();

                    for (int i = 0; i < 1500; i++)
                    {
                        if (i < 500)
                        {
                            MessageListener.Instance.ReceiveMessage("Revisando Disco Duro");
                            Thread.Sleep(1);
                            continue;
                        }
                        if (i < 1000)
                        {
                            MessageListener.Instance.ReceiveMessage("Verificando Credenciales");
                            Thread.Sleep(1);
                            continue;
                        }
                        if (i < 1500)
                        {
                            MessageListener.Instance.ReceiveMessage("Esperando Validación");
                            Thread.Sleep(1);
                            continue;
                        }
                    }


                    Splasher.CloseSplash();
                    _view.Show();
                }
                else
                {
                    Error = "El usuario o la contraseña son incorrectos";
                    BandError = true;
                }
            }
            else
            {
                Error = "Favor de capturar usuario y contraseña";
                BandError = true;
            }
        }

        private void ButtonClick(object obj)
        {
            if (obj != null)
            {
                User.Password = ((PasswordBox)(obj)).Password;
                this.autentificacion();
            }
        }

        private void limpiarClick(object button)
        {
            User.Username = string.Empty;
            User.Password = string.Empty;
            Error = string.Empty;
            BandError = false;
        }

        public override void OnCaptured(DPUruNet.CaptureResult captureResult)
        {
            Error = string.Empty;
            base.OnCaptured(captureResult);
            Identify();
        }

        private async void Identify(object Huella = null)
        {
            try
            {
                if (FingerPrintData == null)
                    return;

                Error = "Iniciando sesión...";
                var Service = new BiometricoServiceClient();

                var CompareResult = Service.CompararHuellaImputado(new ComparationRequest { BIOMETRICO = FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes, ID_TIPO_BIOMETRICO = DD_Dedo.HasValue ? DD_Dedo.Value : BiometricoServiceReference.enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP });

                if (CompareResult.Identify)
                {
                    var result = CompareResult.Result[0];
                    var NombreLogin = await StaticSourcesViewModel.CargarDatosAsync(() => new cImputado().GetData().Where(w => w.ID_ANIO == result.ID_ANIO && w.ID_CENTRO == result.ID_CENTRO && w.ID_IMPUTADO == result.ID_IMPUTADO).FirstOrDefault());
                    User.NombreCompleto = NombreLogin.NOMBRE.Trim() + " " + NombreLogin.PATERNO.Trim() + " " + NombreLogin.MATERNO.Trim();
                    //cambiar el proceso para que traiga el username y de ahi sacar el resto de los datos.
                    if (User.NombreCompleto=="ANGEL EMILIO AREVALO MARQUEZ")
                        User.Username = "adminAlim";
                    else
                        User.Username = "adminMed";
                    OnProgress.Start();
                }
                else
                    Error = "El usuario o la contraseña son incorrectos";

                FingerPrintData = null;
            }
            catch
            {
                Error = "Ocurrió un error al iniciar sesión";
            }
        }

        public override void OnSucceed(Window Window)
        {
            autentificacion();
        }

        private bool selectfingeropen = false;

        private void SelectFinger(LoginView Window = null)
        {
            Window.Height = selectfingeropen ? (Window.Height - 50) : (Window.Height + 50);
            selectfingeropen = !selectfingeropen;
        }

        private void GuardarDedoSeleccionado()
        {
#if DEBUG
            var oConfigDebug = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);
            oConfigDebug.AppSettings.Settings["DedoLoggin"].Value = DD_Dedo.HasValue ? ((short)DD_Dedo.Value).ToString() : ((short)BiometricoServiceReference.enumTipoBiometrico.INDICE_DERECHO).ToString();
            oConfigDebug.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
#else
            //C:\Users\ernesto-quadro\AppData\Local\Apps\2.0\K28OGXZC.KG3\2TRL1481.VG9\cont..tion_7df9047fb6387397_0001.0000_2cfa02e12fedfc2a\ControlPenales.exe
            var filePath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var path = filePath.FilePath.Substring(0, (filePath.FilePath.Length - (".Config").Length));
            var oConfig = ConfigurationManager.OpenExeConfiguration(path);
            oConfig.AppSettings.Settings["DedoLoggin"].Value = DD_Dedo.HasValue ? ((short)DD_Dedo.Value).ToString() : ((short)BiometricoServiceReference.enumTipoBiometrico.INDICE_DERECHO).ToString();
            oConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
#endif
        }
        #endregion

        #region eventos
        ICommand loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return loginCommand ?? (loginCommand = new RelayCommand(ButtonClick));
            }
        }

        ICommand limpiarCommand;
        public ICommand LimpiarCommand
        {
            get
            {
                return limpiarCommand ?? (limpiarCommand = new RelayCommand(limpiarClick));
            }
        }

        public ICommand WindowLoading
        {
            get { return new DelegateCommand<LoginView>(OnLoad); }
        }

        public ICommand FingerProperties
        {
            get { return new DelegateCommand<LoginView>(SelectFinger); }
        }
        #endregion

    }
}
