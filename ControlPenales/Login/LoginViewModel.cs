using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using ControlPenales.Login;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Principales.Compartidos;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using res = Resources;

namespace ControlPenales
{
    public class LoginViewModel : FingerPrintScanner
    {
        #region variables
        private PasswordBox TBPwd;
        private string UBD = string.Empty;
        private double OriginalHeight { get; set; }
        private double Margen = 50;

        public string VersionInfo
        {
            get
            {
                string publishVersion = "LOCAL";
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                    publishVersion = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

                return publishVersion;
            }
        }
        private string _ErrorLogin;
        public string ErrorLogin
        {
            get { return _ErrorLogin; }
            set
            {
                if (value != string.Empty)
                {
                    LoginLoading = Visibility.Collapsed;
                    LoginMargin = new Thickness(0, 180, 0, 0);
                    EnabledContent = true;
                }
                if (value == _ErrorLogin)
                    return;
                _ErrorLogin = value;
                OnPropertyChanged("ErrorLogin");
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Task.Factory.StartNew(async () => { await TaskEx.Delay(4000); ErrorLogin = string.Empty; });
                }));
            }
        }

        private bool bandError;

        public bool BandError
        {
            get { return bandError; }
            set { bandError = value; OnPropertyChanged("BandError"); }
        }

        private Usuario user;
        public Usuario User
        {
            get { return user; }
            set
            {
                user = value;
                StaticSourcesViewModel.UsuarioLogin = value;
                OnPropertyChanged("User");
            }
        }

        private bool huella = false;

        private enumTipoBiometrico? _DD_Dedo = (enumTipoBiometrico)Convert.ToInt16(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings["DedoLogin"].Value);
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set
            {
                _DD_Dedo = value;
                OnPropertyChanged("DD_Dedo");
                GuardarLoginSeleccionado();
            }
        }

        private ObservableCollection<CENTRO> lstCentro;
        public ObservableCollection<CENTRO> LstCentro
        {
            get { return lstCentro; }
            set { lstCentro = value; OnPropertyChanged("LstCentro"); }
        }

        private CENTRO selectCentro;
        public CENTRO SelectCentro
        {
            get { return selectCentro; }
            set { selectCentro = value; OnPropertyChanged("SelectCentro"); }
        }

        private short? selectedCentro = Convert.ToInt16(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings["CentroLogin"].Value);
        public short? SelectedCentro
        {
            get { return selectedCentro; }
            set
            {
                selectedCentro = value;
                OnPropertyChanged("SelectedCentro");
                GuardarLoginSeleccionado();

            }
        }

        Visibility _LoginLoading = Visibility.Collapsed;
        public Visibility LoginLoading
        {
            get { return _LoginLoading; }
            set
            {
                _LoginLoading = value;
                OnPropertyChanged("LoginLoading");
                if (value == Visibility.Visible)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                    {
                        EnabledContent = false;
                        if (Window != null)
                            if (Window.Height != OriginalHeight)
                                Window.Height = selectfingeropen ? (Window.Height - Margen) : (Window.Height + Margen);

                        await Task.Factory.StartNew(async () =>
                        {
                            for (int i = 180; i > 50; i -= 12)
                            {
                                LoginMargin = new Thickness(0, i, 0, 0);
                                await TaskEx.Delay(30);
                            }
                        });
                    }));
                }
            }
        }

        bool _EnabledContent = true;
        public bool EnabledContent
        {
            get { return _EnabledContent; }
            set
            {
                _EnabledContent = value;
                OnPropertyChanged("EnabledContent");
            }
        }
        LoginView Window;

        Thickness _LoginMargin = new Thickness(0, 180, 0, 0);
        public Thickness LoginMargin
        {
            get { return _LoginMargin; }
            set
            {
                _LoginMargin = value;
                OnPropertyChanged("LoginMargin");
            }
        }

        private Visibility visibleBotonBiometrico = Visibility.Collapsed;
        public Visibility VisibleBotonBiometrico
        {
            get { return visibleBotonBiometrico; }
            set { visibleBotonBiometrico = value; OnPropertyChanged("VisibleBotonBiometrico"); }
        }
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            try
            {
                var ayer = new DateTime(2016,09,30).Date;
                ayer = ayer.AddYears(2);
                ayer = ayer.AddMonths(6);
                ayer = ayer.AddDays(12);
                
               

                User = new Usuario();
                BandError = false;
            }
            catch (Exception ex)
            {
                var trace = new System.Diagnostics.StackTrace(ex, true);
                Trace.WriteLine("\n>>>[Ubicación del Problema]");
                Trace.WriteLine("Clase: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().ReflectedType.FullName);
                Trace.WriteLine("Metodo: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().Name);
                Trace.WriteLine("Linea: " + trace.GetFrame((trace.FrameCount - 1)).GetFileLineNumber());
                Trace.WriteLine("Columna: " + trace.GetFrame((trace.FrameCount - 1)).GetFileColumnNumber());
                Trace.WriteLine(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
                StaticSourcesViewModel.ShowMessageError(res.General.algo_paso, res.ControlPenales.Login.LoginViewModel.error_login, ex);
            }
        }
        #endregion

        #region metodos
        private void OnLoad(LoginView Window)
        {
            try
            {
                XMLCentros();
                //Asignamos usuarios temporales
                UsuarioBaseDatos();
                //Validamos si esta habilitado el biometrico de la pantalla
                ValidacionesBiometrico();
                ObtenerPassWord();
                #region Comentado
                //GlobalVar.gArea = Convert.ToInt16(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings["EquipoArea"].Value);
                //LstCentro = LstCentro ?? new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos());
                //LstCentro.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = Resources.General.seleccione });
                #endregion
                TBPwd = Window.tbPassword;
                OriginalHeight = Window.Height;

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
                    try
                    {
                        OnProgress.Abort();
                        CancelCaptureAndCloseReader(OnCaptured);
                    }
                    catch (Exception ex)
                    {
                        var trace = new System.Diagnostics.StackTrace(ex, true);
                        Trace.WriteLine("\n>>>[Ubicación del Problema]");
                        Trace.WriteLine("Clase: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().ReflectedType.FullName);
                        Trace.WriteLine("Metodo: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().Name);
                        Trace.WriteLine("Linea: " + trace.GetFrame((trace.FrameCount - 1)).GetFileLineNumber());
                        Trace.WriteLine("Columna: " + trace.GetFrame((trace.FrameCount - 1)).GetFileColumnNumber());
                        Trace.WriteLine(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
                    }
                };

                if (!OpenReader())
                    Window.Close();

                if (!StartCaptureAsync(OnCaptured))
                    Window.Close();

                OnProgress = new Thread(() => InvokeDelegate(Window));

            }
            catch (Exception ex)
            {
                var trace = new System.Diagnostics.StackTrace(ex, true);
                Trace.WriteLine("\n>>>[Ubicación del Problema]");
                Trace.WriteLine("Clase: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().ReflectedType.FullName);
                Trace.WriteLine("Metodo: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().Name);
                Trace.WriteLine("Linea: " + trace.GetFrame((trace.FrameCount - 1)).GetFileLineNumber());
                Trace.WriteLine("Columna: " + trace.GetFrame((trace.FrameCount - 1)).GetFileColumnNumber());
                Trace.WriteLine(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
                StaticSourcesViewModel.ShowMessageError(res.General.algo_paso, res.ControlPenales.Login.LoginViewModel.error_login, ex);
            }
        }

        private void UsuarioBaseDatos()
        {
            GlobalVariables.gUser = "CONEXION";
            GlobalVariables.gPass = UBD = "C0N3X10N";
            //GlobalVariables.gUser = "SYSTEM";//"QPERSONA";
            //GlobalVariables.gPass = "QUADRO";
            //UBD = "QUADRO";// Parametro.PASSWORD_USUARIO_BD;
        }

        private void ObtenerPassWord() {
            try
            {
                var parametro = new cParametros().Obtener("PASSWORD_USUARIO_BD", SelectedCentro.Value);
                if (parametro != null)
                {
                    GlobalVariables.gPass = UBD = !string.IsNullOrEmpty(parametro.VALOR) ? parametro.VALOR.Trim() : string.Empty;
                }
            }
            catch (Exception ex)
            { 
            }
        }

        private async void autentificacion()
        {
            try
            {
                string MsjError = string.Empty;
                ErrorLogin = string.Empty;
                BandError = false;
                ///TODO:Quitar despues de pruebas
                if (SelectedCentro == -1)
                {
                    ErrorLogin = res.ControlPenales.Login.LoginViewModel.seleccione_centro;
                    BandError = true;
                    return;
                }

                //GlobalVariables.gPass = UBD; //User.Password =   //User.Password.ToUpper();
                //User.Username = "SYSTEM";
                //User.Password = "quadro";

                if (string.IsNullOrEmpty(User.Username) ? false : string.IsNullOrEmpty(User.Password) && huella == false ? false : true)
                //if (!string.IsNullOrEmpty(User.Username) || User.Username == "SYS" || User.Username == "TONGOLELE")
                {
                   // if (User.Username == "SYSTEM" || User.Username == "SYS" || User.Username == "TONGOLELE")
                     //   User.Password = "QUADRO";
                    GlobalVariables.gUser = User.Username;
                    //GlobalVariables.gPass = User.Password;
                    //validar usuario
                    LoginLoading = Visibility.Visible;
                    USUARIO usr = null;

                    if (await Task.Factory.StartNew<bool>(() => { usr = new cUsuario().ObtenerUsuario(User.Username); return usr == null; }))
                    {
                        ErrorLogin = res.ControlPenales.Login.LoginViewModel.usuario_contrasena_incorrecto;
                        BandError = true;
                        return;
                    }


                    #region Valida Sesion
                    if (!await Task.Factory.StartNew<bool>(() => { return ValidarSesion(); }))
                    {
                        ErrorLogin = "Ya existe una sesion activa con este usuario";
                        BandError = true;
                        TBPwd.Focus();
                        return;
                    }
                    #endregion


                    //OBTENEMOS usuario
                    if (User.Username.Equals(usr.ID_USUARIO.Trim()) && (huella == true ? true : !string.IsNullOrEmpty(usr.PASSWORD) ? cEncriptacion.IsEquals(User.Password.ToUpper(), usr.PASSWORD) : false))
                    {
                        huella = false;
                        if ((!string.IsNullOrEmpty(usr.ESTATUS) ? usr.ESTATUS : string.Empty) == "S")
                        {
                            var hoy = Fechas.GetFechaDateServer;
                            TimeSpan ts;
                            if (User.Username == "SYSTEM")
                                ts = hoy.AddDays(100) - hoy;
                            else
                                ts = usr.VENCE_PASS.Value - hoy;
                            if (Math.Floor(ts.TotalDays) > 0)
                            {
                                Application.Current.MainWindow.Hide();
                                var _view = new ControlPenales.PrincipalView();

                                User.Nombre = User.Username == "SYSTEM" ? "SYSTEM" : string.Format("{0} {1} {2}", !string.IsNullOrEmpty(usr.EMPLEADO.PERSONA.NOMBRE) ? usr.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(usr.EMPLEADO.PERSONA.PATERNO) ? usr.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(usr.EMPLEADO.PERSONA.MATERNO) ? usr.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty);
                                User.Password = usr.PASSWORD;
                                User.VigenciaPassword = usr.VENCE_PASS;
                                User.CentroNombre = SelectCentro.DESCR;
                                var _viewModel = new PrincipalViewModel(User);
                                _view.DataContext = _viewModel;
                                //centro
                                GlobalVar.gCentro = GlobalVariables.gCentro = SelectedCentro.Value;

                                Application.Current.MainWindow.Close();

                                var metro = Application.Current.Windows[0] as MahApps.Metro.Controls.MetroWindow;

                                ((ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                                ((ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();

                                //Validar permisos del equipo
                                Splasher.Splash = new ControlPenales.Login.SplashScreen();
                                Splasher.ShowSplash();

                                var _error_validacion = true;
                                CATALOGO_EQUIPOS _mac_valida = null;


                                MessageListener.Instance.ReceiveMessage(res.General.verificando_crendenciales);

                                ///Eliminar cuando se implemente la verificacion de equipo
                                //await TaskEx.Delay(1500);
                                ///Eliminar cuando se implemente la verificacion de equipo

                                //esteban
                                //if(!string.IsNullOrEmpty(GlobalVar.gIP) && !string.IsNullOrEmpty(GlobalVar.gMAC_ADDRESS))
                                //{
                                //    _error_validacion = false;
                                //}
                                #region comentado
                                await Task.Factory.StartNew(() =>
                                {
                                    var _nics = VerificacionDispositivo.GetMacAddress();
                                    foreach (var item in _nics)
                                    {
                                        //borrar
                                        foreach(var item2 in item.ips)
                                        Trace.WriteLine("\n>IP:"+ item2 + " ,MAC:" + item.mac );
                                        //hasta aqui
                                        _mac_valida = new cCatalogoEquipos().ValidarEquipoMAC(item.ips, item.mac);
                                        if (_mac_valida != null)
                                        {
                                            GlobalVar.gIP = _mac_valida.IP;
                                            GlobalVar.gMAC_ADDRESS = _mac_valida.MAC_ADDRESS;
                                            _error_validacion = false;
                                            break;
                                        }
                                    }
                                });
                                #endregion
                                //hasta aqui
                                if (Parametro.VERIFICA_HD_SERIAL && _mac_valida != null)
                                {
                                    _error_validacion = true;
                                    MessageListener.Instance.ReceiveMessage(res.General.revisando_disco_duro);
                                    await Task.Factory.StartNew(() =>
                                    {
                                        var hds = VerificacionDispositivo.GETHDSerial();
                                        foreach (var item in hds)
                                        {
                                            if (new cCatalogoEquipos().ValidarHD(item.SerialNo, _mac_valida.IP, _mac_valida.MAC_ADDRESS))
                                            {
                                                _error_validacion = false;
                                                break;
                                            }
                                            else
                                            {
                                                MsjError = "Error al validar HD";
                                            }
                                        }
                                    });
                                }

                                //Validar si tiene permisos para entrar al centro
                                await Task.Factory.StartNew(() =>
                                {
                                    var procesos = new cProcesoUsuario().ObtenerTodos(GlobalVariables.gUser, null, SelectCentro.ID_CENTRO).FirstOrDefault();
                                    if (procesos == null)
                                    {
                                        Trace.WriteLine("no pudo validar procesos");
                                        _error_validacion = true;
                                    }
                                });

                                MessageListener.Instance.ReceiveMessage(res.General.esperando_validacion);
                                await TaskEx.Delay(1500);
                                //if (!_error_validacion)
                                //_error_validacion = false;
                                if (_error_validacion == false)
                                {
                                    Splasher.CloseSplash();
                                    CrearSesion();
                                    _view.Show();
                                }
                                else
                                {
                                    MessageListener.Instance.ReceiveMessage("ACCESO DENEGADO");
                                    await TaskEx.Delay(7500);
                                    Splasher.CloseSplash();
                                    Application.Current.Shutdown();
                                }
                                ///Eliminar cuando se implemente la verificacion de equipo
                                //Splasher.CloseSplash();
                                //_view.Show();
                                ///Eliminar cuando se implemente la verificacion de equipo
                            }
                            else
                            {
                                ErrorLogin = res.ControlPenales.Login.LoginViewModel.password_caducado;
                                BandError = true;
                                await TaskEx.Delay(500);
                                TBPwd.Focus();
                            }
                        }
                        else
                        {
                            ErrorLogin = res.ControlPenales.Login.LoginViewModel.usuario_inactivo;
                            BandError = true;
                            await TaskEx.Delay(500);
                            TBPwd.Focus();
                        }
                    }
                    else
                    {
                        ErrorLogin = res.ControlPenales.Login.LoginViewModel.usuario_contrasena_incorrecto;
                        BandError = true;
                        await TaskEx.Delay(500);
                        TBPwd.Focus();
                    }
                }
                else
                {
                    ErrorLogin = res.ControlPenales.Login.LoginViewModel.usuario_contrasena_incorrecto;
                    BandError = true;
                    await TaskEx.Delay(500);
                    TBPwd.Focus();
                }
            }
            catch (Exception ex)
            {
                var trace = new System.Diagnostics.StackTrace(ex, true);
                Trace.WriteLine("\n>>>[Ubicación del Problema]");
                Trace.WriteLine("Clase: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().ReflectedType.FullName);
                Trace.WriteLine("Metodo: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().Name);
                Trace.WriteLine("Linea: " + trace.GetFrame((trace.FrameCount - 1)).GetFileLineNumber());
                Trace.WriteLine("Columna: " + trace.GetFrame((trace.FrameCount - 1)).GetFileColumnNumber());
                Trace.WriteLine(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
                StaticSourcesViewModel.ShowMessageError(res.General.algo_paso, res.ControlPenales.Login.LoginViewModel.error_login, ex);
            }
        }

        private void ButtonClick(object obj)
        {
            if (obj != null)
            {
                User.Password = ((PasswordBox)(obj)).Password;
                if (GuardarConfiguracionEntidad(string.Empty, User.Username, User.Password, "SSPE"))
                {
                   // SSP.Servidor.GlobalVariables.gUser = GlobalVar.gUsr = User.Username;
                    //SSP.Servidor.GlobalVariables.gPass = GlobalVar.gPass = User.Password;
                    GlobalVar.gCentro = SelectedCentro.Value;
                    //SSPEntidades test = new SSPEntidades();
                    var STR = ConfiguracionEntity();
                    this.autentificacion();
                }
            }
        }

        private void limpiarClick(object button)
        {
            User.Username = string.Empty;
            User.Password = string.Empty;
            ErrorLogin = string.Empty;
            BandError = false;
        }

        public override void OnCaptured(DPUruNet.CaptureResult captureResult)
        {
            ErrorLogin = string.Empty;
            base.OnCaptured(captureResult);
            Identify();
        }

        private async void Identify(object Huella = null)
        {
            try
            {
                if (FingerPrintData == null)
                    return;

                LoginLoading = Visibility.Visible;
                var Service = new BiometricoServiceClient();

                var CompareResult = Service.CompararHuellaPersona(new ComparationRequest
                {
                    BIOMETRICO = FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes,
                    ID_TIPO_PERSONA = enumTipoPersona.PERSONA_EMPLEADO,
                    ID_TIPO_BIOMETRICO = DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO,
                    ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP
                });//.CompararHuellaImputado(new ComparationRequest { BIOMETRICO = FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes, ID_TIPO_BIOMETRICO = DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP });

                if (CompareResult.Identify)
                {
                    ///TODO: cambiar usuario login ERNESTO
                    var result = CompareResult.Result[0];
                    GlobalVariables.gUser = "CONEXION";//"QPERSONA";
                    GlobalVariables.gPass = "C0N3X10N";//"QUADRO";
                    var persona = await StaticSourcesViewModel.CargarDatosAsync(() => new cPersona().ObtenerPersona(result.ID_PERSONA));
                    GlobalVariables.gUser = string.Empty;
                    GlobalVariables.gPass = string.Empty;
                    if (persona != null)
                    {
                        if (persona.EMPLEADO.USUARIO.Count > 0)
                        {
                            var u = persona.EMPLEADO.USUARIO.FirstOrDefault();
                            //persona.USUARIO.FirstOrDefault().ID_USUARIO.Trim();
                            User.Username = u.ID_USUARIO.Trim();
                            User.Password = string.Empty;
                            huella = true;
                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                autentificacion();
                            }));


                        }
                    }
                    //var NombreLogin = await StaticSourcesViewModel.CargarDatosAsync(() => new SSP.Controlador.Catalogo.Justicia.cImputado().GetData().Where(w => w.ID_ANIO == result.ID_ANIO && w.ID_CENTRO == result.ID_CENTRO && w.ID_IMPUTADO == result.ID_IMPUTADO).FirstOrDefault());
                    //User.Username = "SYSTEM";
                    //User.Username = NombreLogin.NOMBRE.Trim() + " " + NombreLogin.PATERNO.Trim() + " " + NombreLogin.MATERNO.Trim();
                    //OnProgress.Start();
                }
                else
                {
                    ErrorLogin = res.ControlPenales.Login.LoginViewModel.usuario_contrasena_incorrecto;
                }

                FingerPrintData = null;
            }
            catch (Exception ex)
            {
                var trace = new System.Diagnostics.StackTrace(ex, true);
                Trace.WriteLine("\n>>>[Ubicación del Problema]");
                Trace.WriteLine("Clase: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().ReflectedType.FullName);
                Trace.WriteLine("Metodo: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().Name);
                Trace.WriteLine("Linea: " + trace.GetFrame((trace.FrameCount - 1)).GetFileLineNumber());
                Trace.WriteLine("Columna: " + trace.GetFrame((trace.FrameCount - 1)).GetFileColumnNumber());
                Trace.WriteLine(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
                ErrorLogin = res.ControlPenales.Login.LoginViewModel.error_sesion;
            }
        }

        public override void OnSucceed(Window Window)
        {
            autentificacion();
        }

        private bool selectfingeropen = false;

        private void SelectFinger(LoginView _Window = null)
        {
            Window = _Window;
            Window.Height = selectfingeropen ? (Window.Height - Margen) : (Window.Height + Margen);
            selectfingeropen = !selectfingeropen;
        }

        private void GuardarLoginSeleccionado()
        {
            try
            {
#if DEBUG
                var oConfigDebug = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);
                oConfigDebug.AppSettings.Settings["DedoLogin"].Value = DD_Dedo.HasValue ? ((short)DD_Dedo.Value).ToString() : ((short)enumTipoBiometrico.INDICE_DERECHO).ToString();
                oConfigDebug.AppSettings.Settings["CentroLogin"].Value = SelectedCentro.HasValue ? SelectedCentro.ToString() : "-1";
                oConfigDebug.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
#else
            //C:\Users\ernesto-quadro\AppData\Local\Apps\2.0\K28OGXZC.KG3\2TRL1481.VG9\cont..tion_7df9047fb6387397_0001.0000_2cfa02e12fedfc2a\ControlPenales.exe
            var filePath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var path = filePath.FilePath.Substring(0, (filePath.FilePath.Length - (".Config").Length));
            var oConfig = ConfigurationManager.OpenExeConfiguration(path);
            oConfig.AppSettings.Settings["DedoLogin"].Value = DD_Dedo.HasValue ? ((short)DD_Dedo.Value).ToString() : ((short)enumTipoBiometrico.INDICE_DERECHO).ToString();
            oConfig.AppSettings.Settings["CentroLogin"].Value = SelectedCentro.HasValue ? SelectedCentro.ToString() : "-1";
            oConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
#endif
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }

        }

        public bool GuardarConfiguracionEntidad(string server, string user, string password, string database)
        {
            try
            {
                ////<add name="SSPEntidades" connectionString="metadata=res://*/Modelo.csdl|res://*/Modelo.ssdl|res://*/Modelo.msl;provider=Oracle.ManagedDataAccess.Client;provider connection string=&quot;DATA SOURCE=SSPE;PASSWORD=QUADRO;USER ID=SYSTEM&quot;" providerName="System.Data.EntityClient" />
                //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                //config.ConnectionStrings.ConnectionStrings["SSPEntidades"].ConnectionString = string.Format("metadata=res://*/Modelo.csdl|res://*/Modelo.ssdl|res://*/Modelo.msl;provider=Oracle.ManagedDataAccess.Client;provider connection string=&quot;DATA SOURCE={0};PASSWORD={1};USER ID={2}&quot;",database,password,user);
                //config.ConnectionStrings.ConnectionStrings["SSPEntidades"].ProviderName = "System.Data.EntityClient";
                //config.Save(ConfigurationSaveMode.Full, true);
                //ConfigurationManager.RefreshSection("connectionStrings");
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        public string ConfiguracionEntity()
        {

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return config.ConnectionStrings.ConnectionStrings["SSPEntidades"].ConnectionString;

        }

        private void XMLCentros()
        {
            try
            {
                LstCentro = new ObservableCollection<CENTRO>();
                XmlDocument doc = new XmlDocument();
                doc.Load(@"XML\centros.xml");
                XmlNodeList centros = doc.GetElementsByTagName("centro");
                foreach (XmlElement nodo in centros)
                {
                    LstCentro.Add(new CENTRO() { ID_CENTRO = short.Parse(nodo.Attributes["ID"].Value), DESCR = nodo.Attributes["DESCR"].Value });
                }
                LstCentro.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = Resources.General.seleccione });
                ///TODO:Quitar despues de pruebas
                //User.Username = "SYSTEM";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.InnerException);
            }
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

        #region Validaciones
        private async void ValidacionesBiometrico()
        {
            try
            {
              
                await Task.Factory.StartNew(() =>
                {
                    var _mac_valida = new CATALOGO_EQUIPOS();
                    var _nics = VerificacionDispositivo.GetMacAddress();
                    foreach (var item in _nics)
                    {
                        _mac_valida = new cCatalogoEquipos().ValidarEquipoMAC(item.ips, item.mac);
                        if (_mac_valida != null)
                        {
                            GlobalVar.gIP = _mac_valida.IP;
                            GlobalVar.gMAC_ADDRESS = _mac_valida.MAC_ADDRESS;
                            if (_mac_valida.BIOMETRIA == "S")
                            {
                                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    VisibleBotonBiometrico = Visibility.Visible;
                                }));
                            }
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
        }
        #endregion

        #region Sesion
        private bool ValidarSesion()
        {
            try
            {
                //var v = new cSesion().ObtenerTodos(User.Username,"S").FirstOrDefault();
                //if (v == null)
                //{
                //    return true;
                //}
                //else
                //{
                //    var hoy = Fechas.GetFechaDateServer;
                //    if ((hoy - v.FECHA_CONTROL.Value).TotalMinutes > 5)
                //        return true;
                //}
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.InnerException);
            }
            return false;
        }

        private void CrearSesion()
        {
            try
            {
                var s = new SESION();

                s.USUARIO = User.Username;
                s.FECHA = Fechas.GetFechaDateServer;
                s.FECHA_CONTROL = s.FECHA;
                s.ACTIVO = "S";
                s.IP = GlobalVar.gIP;
                GlobalVar.gSesion = new cSesion().Insertar(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.InnerException);
            }
        }
        #endregion
    }
}