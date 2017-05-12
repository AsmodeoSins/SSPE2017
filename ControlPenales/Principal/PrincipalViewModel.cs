using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class PrincipalViewModel : ValidationViewModelBase
    {
        #region eventos
        //DECLARACION EVENTO
        public static event EventHandler CambiarVentanaSelecccionado;
        public static event EventHandler SalirSistema;
        public static event EventHandler Cancelar;
        public static event EventHandler Cerrar;
        #endregion

        #region variables
        private BandejaEntradaViewModel _MainMenu;
        public BandejaEntradaViewModel MainMenu
        {
            get { return _MainMenu != null ? _MainMenu : new BandejaEntradaViewModel(); }
            set { _MainMenu = value; }
        }
        private IPageViewModel _currentPageViewModel;
        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }
        private Usuario usuario;
        public Usuario Usuario
        {
            get { return usuario; }
            set { usuario = value; OnPropertyChanged("Usuario"); }
        }
        private string logeado;
        public string Logeado
        {
            get { return logeado; }
            set { logeado = value; OnPropertyChanged("Logeado"); }
        }
        private int nNoNotificaciones = 0;
        private string sNoNotificaciones;
        public string SNoNotificaciones
        {
            get { return sNoNotificaciones; }
            set { sNoNotificaciones = value; OnPropertyChanged("SNoNotificaciones"); }
        }
        private bool notificacionesVisible;
        public bool NotificacionesVisible
        {
            get { return notificacionesVisible; }
            set { notificacionesVisible = value; OnPropertyChanged("NotificacionesVisible"); }
        }
        private bool menuPrincipalVisible;
        public bool MenuPrincipalVisible
        {
            get { return menuPrincipalVisible; }
            set { menuPrincipalVisible = value; OnPropertyChanged("MenuPrincipalVisible"); }
        }
        private static string dialogTitulo;
        public static string DialogTitulo
        {
            get { return PrincipalViewModel.dialogTitulo; }
            set { PrincipalViewModel.dialogTitulo = value; }
        }
        private static string dialogMensaje;
        public static string DialogMensaje
        {
            get { return PrincipalViewModel.dialogMensaje; }
            set { PrincipalViewModel.dialogMensaje = value; }
        }

        string _UnLockPassword;
        public string UnLockPassword
        {
            get { return _UnLockPassword; }
            set
            {
                _UnLockPassword = value;
                OnPropertyChanged("UnLockPassword");
            }
        }
        string _ErrorLogin;
        public string ErrorLogin
        {
            get { return _ErrorLogin; }
            set
            {
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

        private string titulo = "Sistema de Información y Control Estatal Penitenciario";
        public string Titulo
        {
            get { return titulo; }
            set { titulo = value; OnPropertyChanged("Titulo"); }
        }
        #endregion

        //notificaciones
        private System.Timers.Timer aTimer;
        #region contructor
        public PrincipalViewModel(Usuario usuario)
        {
            try
            {
                this.usuario = usuario;
                Logeado = string.Format("USUARIO: {0}", this.usuario.Nombre).ToUpper();
                Titulo = string.Format("Sistema de Información y Control Estatal Penitenciario, Centro:{0}", usuario.CentroNombre);

                NotificacionesVisible = false;
                SNoNotificaciones = "0";

                //aTimer = new System.Timers.Timer();
                //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                //aTimer.Interval = 6000000;//se refrescara cada 5 min
                //aTimer.Enabled = true;

                MenuPrincipalVisible = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar al usuario", ex);
            }
        }
        #endregion

        #region Command
        private ICommand _changePageCommand;
        public ICommand ChangePageCommand
        {
            get
            {
                return _changePageCommand ?? (_changePageCommand = new RelayCommand(ChangeViewModel));
            }
        }

        private ICommand _onClickNotificaciones;
        public ICommand OnClickNotificaciones
        {
            get
            {
                return _onClickNotificaciones ?? (_onClickNotificaciones = new RelayCommand(setNotificaciones));
            }
        }

        private ICommand _WindowLoad;
        public ICommand WindowLoad
        {
            get
            {
                return _WindowLoad ?? (_WindowLoad = new RelayCommand(OnWindowLoaded));
            }
        }
        private ICommand _MouseMove;
        public ICommand MouseMove
        {
            get
            {
                return _MouseMove ?? (_MouseMove = new RelayCommand(OnMouseMove));
            }
        }
        private ICommand _OnUnLockMenuClick;
        public ICommand OnUnLockMenuClick
        {
            get
            {
                return _OnUnLockMenuClick ?? (_OnUnLockMenuClick = new RelayCommand(UnLockMenuClick));
            }
        }

        private ICommand _OnAppClose;
        public ICommand OnAppClose
        {
            get
            {
                return _OnAppClose ?? (_OnAppClose = new RelayCommand(CloseApp));
            }
        }

        private ICommand _onClickMenuPrincipal;
        public ICommand OnClickCloseMenuPrincipal
        {
            get
            {
                return _onClickMenuPrincipal ?? (_onClickMenuPrincipal = new RelayCommand(menuPrincipal));
            }
        }

        //CONTROLAR CLICK OPCIONES DEL MENU
        private ICommand _onClickMenuPrincipalOpcion;
        public ICommand OnClickMenuPrincipalOpcion
        {
            get
            {
                return _onClickMenuPrincipalOpcion ?? (_onClickMenuPrincipalOpcion = new RelayCommand(menuOpcionesSwitch));
            }
        }
        #endregion

        #region Methods
        private void menuPrincipal(object viewModel)
        {
            MenuPrincipalVisible = !MenuPrincipalVisible;
        }

        //DECLARACION METODO EVENTO
        public static void OnCambiarVentana(object sender, EventArgs e)
        {
            if (CambiarVentanaSelecccionado != null)
            {
                CambiarVentanaSelecccionado(sender, e);
            }
        }

        public static void OnSalirSistema(object sender, EventArgs e)
        {
            if (SalirSistema != null)
            {
                SalirSistema(sender, e);
            }
        }

        public static void OnCancelarSistema(object sender, EventArgs e)
        {
            if (Cancelar != null)
            {
                Cancelar(sender, e);
            }
        }

        public static void OnCerrarSistema(object sender, EventArgs e)
        {
            if (Cerrar != null)
            {
                Cerrar(sender, e);
            }
        }

        public async void ChangeViewModel(object param)
        {
            try
            {
                OnCambiarVentana(this, EventArgs.Empty);

                var metro = Application.Current.Windows[0] as MetroWindow;

                if (StaticSourcesViewModel.SourceChanged)
                {
                    var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?");

                    if (dialogresult != 0)
                    {
                        StaticSourcesViewModel.EspecialistaCambiosCancelados = true;
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                        return;
                }
                if (Type.GetType(param.ToString()) != null)
                {
                    if (Type.GetType(param.ToString()).Name.Equals("BandejaEntradaViewModel"))
                    {
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                        GC.Collect();
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
                        return;
                    }
                    else
                    {
                        var params1 = (object[])param;
                        var model = Type.GetType((params1[0]).ToString());
                        var view = Type.GetType((params1[1]).ToString());
                        if (model == null || view == null)
                            return;

                        if (((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext is NotaMedicaEspecialistaViewModel ?
                               model.Name != "NotaMedicaEspecialistaViewModel"
                        : false)
                        {
                            var especialistaModel = (NotaMedicaEspecialistaViewModel)((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext;
                            SalirMenu(especialistaModel);

                            await Task.Factory.StartNew(() =>
                            {
                                while (true)
                                {
                                    if (StaticSourcesViewModel.SalidaEspecialista)
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(delegate
                                        {
                                            foreach (var item in PopUpsViewModels.MainWindow.OwnedWindows)
                                            {
                                                if (item.ToString().Equals("ControlPenales.BuscarPorHuellaYNipView"))
                                                {
                                                    if (especialistaModel.HuellaWindowSalida != null)
                                                        especialistaModel.HuellaWindowSalida.Close();
                                                    if (especialistaModel.HuellaWindow != null)
                                                        especialistaModel.HuellaWindow.Close();
                                                }
                                            }
                                            especialistaModel.HuellaWindowSalida = null;
                                            especialistaModel.HuellaWindow = null;
                                            especialistaModel = null;
                                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                                            GC.Collect();
                                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = System.Activator.CreateInstance(view);
                                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = System.Activator.CreateInstance(model);
                                        }));
                                        //StaticSourcesViewModel.SalidaEspecialista = false;
                                        return;
                                    }
                                    if (StaticSourcesViewModel.EspecialistaCambiosCancelados)
                                    {
                                        return;
                                    }
                                    TaskEx.Delay(1500);
                                }
                            });
                            return;
                        }

                        if (view.BaseType.Name.Equals("MetroWindow"))
                        {
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            var WindowsDisplay = System.Activator.CreateInstance(view);
                            ((Window)WindowsDisplay).DataContext = System.Activator.CreateInstance(model);
                            ((Window)WindowsDisplay).Owner = PopUpsViewModels.MainWindow;
                            ((Window)WindowsDisplay).Icon = PopUpsViewModels.MainWindow.Icon;
                            ((Window)WindowsDisplay).ShowDialog();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            return;
                        }

                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                        GC.Collect();
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = System.Activator.CreateInstance(view);
                        if (model.Name == "ExcarcelacionViewModel")
                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = System.Activator.CreateInstance(model, new object[] { enumVentanaOrigen_Excarcelacion.MAIN_WINDOW, null, null, null });
                        else if (model.Name == "EstudioPersonalidadViewModel")
                        {
                            var _modo = params1[2];
                            if (short.Parse(_modo.ToString()) == (short)eProcesoVentanasEstudioPersonalidad.CIERRE_ESTUDIOS)
                                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new EstudioPersonalidadViewModel(eProcesoVentanasEstudioPersonalidad.CIERRE_ESTUDIOS);

                            else if (short.Parse(_modo.ToString()) == (short)eProcesoVentanasEstudioPersonalidad.PROGRAMACION_ESTUDIOS)
                                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new EstudioPersonalidadViewModel(eProcesoVentanasEstudioPersonalidad.PROGRAMACION_ESTUDIOS);
                        }

                        else if (model.Name == "HistoriaClinicaViewModel")
                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new HistoriaClinicaViewModel(null, null, null, null);
                        else
                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = System.Activator.CreateInstance(model);
                        return;
                    }
                }
                else if (param.ToString() == "interno")
                {
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                    GC.Collect();
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new ConsultaExpedienteInternoView();
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new ConsultaExpedienteInternoViewModel();

                    return;
                }
                else
                {
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                    GC.Collect();
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
                    return;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambiar de módulo", ex);
            }
        }

        public void setNotificaciones(object viewModel)
        {
            try
            {
                var metro = Application.Current.Windows[0] as MetroWindow;

                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                GC.Collect();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener notificaciones", ex);
            }
        }

        public async static void SalirMenu(NotaMedicaEspecialistaViewModel model = null)
        {
            try
            {
                var metro = Application.Current.Windows[0] as MetroWindow;
                if (StaticSourcesViewModel.SourceChanged)
                {
                    var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?");
                    if (dialogresult != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                    {
                        if (((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext is NotaMedicaEspecialistaViewModel)
                        {
                            model = (NotaMedicaEspecialistaViewModel)((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext;
                            model.SalirClickMenu = true;
                            StaticSourcesViewModel.EspecialistaCambiosCancelados = true;
                        }
                        return;
                    }
                }
                if (((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext is NotaMedicaEspecialistaViewModel)
                {
                    PopUpsViewModels.ShowPopUp(((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    model = (NotaMedicaEspecialistaViewModel)((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext;
                    model.HuellaWindowSalida = new BuscarPorHuellaYNipView();
                    model.HuellaWindowSalida.DataContext = ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext;
                    model.HuellaWindowSalida.Owner = PopUpsViewModels.MainWindow;
                    model.HuellaWindowSalida.IsCloseButtonEnabled = false;
                    model.BuscarPor = enumTipoPersona.PERSONA_EMPLEADO;
                    model.AceptaSalida = true;
                    model.IsEspecialista = false;
                    StaticSourcesViewModel.SalidaEspecialista = false;
                    StaticSourcesViewModel.EspecialistaCambiosCancelados = false;
                    model.HuellaWindowSalida.ShowDialog();
                }
                else
                {
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                    GC.Collect();
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir del módulo", ex);
            }
        }

        private async void OnWindowLoaded(object parameter)
        {
            try
            {
                PopUpsViewModels.MainWindow = ((PrincipalView)parameter);
                StaticSourcesViewModel.LockMenu();
                await StaticSourcesViewModel.CargarDatosMetodoAsync(ObtenerMenuEnabled);
                if (usuario.Username != "SYSTEM")
                    NotificarVigenciaPass();

                ((Window)parameter).Closing += (ss, ee) =>
                {
                    try
                    {
                        cerrarAplicacion();
                        ee.Cancel = true;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la aplicación", ex);
                    }
                };

                ((PrincipalView)parameter).ScrollContent.ScrollChanged += (s, e) =>
                {
                    try
                    {
                        if (((ScrollViewer)s).VerticalOffset == 0 && ((ScrollViewer)s).ScrollableHeight == 0)
                        {
                            StaticSourcesViewModel.ShowFooter = Visibility.Collapsed;
                            return;
                        }

                        if (((ScrollViewer)s).VerticalOffset == ((ScrollViewer)s).ScrollableHeight)
                            StaticSourcesViewModel.ShowFooter = Visibility.Visible;
                        else
                            StaticSourcesViewModel.ShowFooter = Visibility.Collapsed;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la aplicación", ex);
                    }
                };

                #region Control de Sesion
                //aSesionTimer = new System.Timers.Timer();
                //aSesionTimer.Elapsed += (s, e) => { ActualizarSesion("S"); };
                //aSesionTimer.Interval = sesionT;
                //aSesionTimer.Enabled = true;
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la aplicación", ex);
            }
        }

        private void CloseApp(object parameter)
        {
            try
            {
                ((Window)parameter).Close();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la aplicación", ex);
            }
        }

        private async void cerrarAplicacion()
        {
            try
            {
                OnSalirSistema(this, EventArgs.Empty);
                var metro = Application.Current.Windows[0] as MetroWindow;
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Cerrar",
                    NegativeButtonText = "Cancelar",
                    AnimateShow = true,
                    AnimateHide = false
                };
                ((DigitalizacionExpedienteView)PopUpsViewModels.MainWindow.DigitalizarDocumentos).pdfViewer.Visibility = Visibility.Collapsed;

                var windowsHost = ((UserControl)((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content).FindChildren<System.Windows.Forms.Integration.WindowsFormsHost>();
                if (windowsHost != null)
                    foreach (var item in windowsHost)
                        item.Visibility = Visibility.Collapsed;

                var result = await metro.ShowMessageAsync("¿Cerrar Aplicación?", "¿Está seguro que quiere cerrar la aplicación?", MessageDialogStyle.AffirmativeAndNegative, mySettings);

                if (result == MessageDialogResult.Affirmative)
                {
                    if (ActualizarSesion("N"))
                        Application.Current.Shutdown();
                }
                else
                {
                    foreach (var item in windowsHost)
                        item.Visibility = Visibility.Visible;
                    OnCancelarSistema(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la aplicación", ex);
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                DateTime f1, f2;
                f1 = Fechas.GetFechaDateServer;
                f2 = Fechas.GetFechaDateServer;

                f1 = new DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new DateTime(f2.Year, f2.Month, f2.Day);
                TimeSpan ts = f2 - f1;
                // Difference in days.
                if (ts.Days == 0)
                {
                    f2 = f2.AddDays(1);
                }

                nNoNotificaciones = new cUsuarioMensaje().ObtenerCount(GlobalVar.gUsr, f1, f2);
                if (nNoNotificaciones < 100)
                    SNoNotificaciones = nNoNotificaciones.ToString();
                else
                    SNoNotificaciones = "99+";
                if (nNoNotificaciones > 0)
                    NotificacionesVisible = true;
                else
                    NotificacionesVisible = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en la recepción de nuevas notificaciones", ex);
            }
        }

        #region [UnlockMenu]
        private void UnLockMenuClick(object obj)
        {
            if (DateTime.Now.Date != Fechas.GetFechaDateServer.Date)
            {
                ErrorLogin = "La Fecha De Su Equipo Es Diferente A La Del Servidor, Contacte A Un Administrador";
                return;
            }
            if (!(obj is PasswordBox))
                return;
            if (!cEncriptacion.IsEquals(((PasswordBox)(obj)).Password.ToUpper(), new cUsuario().ObtenerUsuario(Usuario.Username).PASSWORD))
            {
                ErrorLogin = Resources.ControlPenales.Login.LoginViewModel.usuario_contrasena_incorrecto;
                return;
            }

            ((PasswordBox)(obj)).Password = string.Empty;
            PopUpsViewModels.LockMenu = Visibility.Visible;
            PopUpsViewModels.UnLockMenu = Visibility.Collapsed;
            PopUpsViewModels.FocusBlock = false;
            StaticSourcesViewModel.LockMenu();
        }

        private void OnMouseMove(object obj)
        {
            StaticSourcesViewModel.TimerLock = StaticSourcesViewModel.TIMETOLOCK;
        }
        #endregion

        private void menuOpcionesSwitch(object opcion)
        {
            try
            {
                switch (opcion.ToString())
                {
                    case "ControlPenales.BandejaEntradaViewModel":
                        ChangeViewModel(opcion);
                        MenuPrincipalVisible = false;
                        break;
                    case "menu_principal_buscar":
                        ChangeViewModel("interno");
                        MenuPrincipalVisible = false;
                        break;
                    case "menu_principal_agenda":
                        ChangeViewModel("interno_agenda");
                        MenuPrincipalVisible = false;
                        break;
                    case "menu_principal_expediente":
                        ChangeViewModel("agenda");
                        MenuPrincipalVisible = false;
                        break;
                    case "menu_principal_ayuda":
                        MenuPrincipalVisible = false;
                        break;
                    case "menu_principal_ultimo_error":
                        MenuPrincipalVisible = false;
                        StaticSourcesViewModel.ShowErrorDialog = Visibility.Visible;
                        break;
                    case "menu_principal_bloquear_sistema":
                        StaticSourcesViewModel.TimerLock = 0;
                        while (PopUpsViewModels.LockMenu != Visibility.Collapsed)
                            MenuPrincipalVisible = false;
                        break;
                    case "menu_principal_salir":
                        cerrarAplicacion();
                        MenuPrincipalVisible = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                MenuPrincipalVisible = false;
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una opción", ex);
            }
        }
        #endregion

        #region Validar Vigencia Password
        private void NotificarVigenciaPass()
        {
            var hoy = Fechas.GetFechaDateServer;
            TimeSpan ts = usuario.VigenciaPassword.Value - hoy;
            if (/*ts.TotalDays > 0 &&*/ ts.TotalDays <= 5)
            {
                new Dialogos().ConfirmacionDialogo("Notificación", string.Format("Favor de cambiar tu password, te quedan {0} dias de vigencia", Math.Floor(ts.TotalDays)));
            }
        }
        #endregion

        #region Menu
        private void ObtenerMenu()
        {
            try
            {
                var procesos = new cProceso().ObtenerProcesoMenu(Usuario.Username);
                if (procesos != null)
                {
                    foreach (var p in procesos)
                    {
                        switch (p.ID_PROCESO)
                        {
                            //Registro    
                            case (short)enumProcesos.INGRESO:
                                MRegistroV = MRegistroMV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.DECOMISO:
                                MDecomisoV = MRegistroMV = Visibility.Visible;
                                break;
                            //Seguimiento
                            //Juridico
                            case (short)enumProcesos.CAUSA_PENAL:
                                MSeguimientoMV = MJuridicoMV = MCausaPenalV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.FICHA:
                                MSeguimientoMV = MJuridicoMV = MIdentificacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.ESTATUS_ADMINISTRATIVO:
                                MSeguimientoMV = MJuridicoMV = MEstatusAdministrativoV = Visibility.Visible;
                                break;
                            //Administrativo
                            case (short)enumProcesos.PERTENENCIAS:
                                MSeguimientoMV = MPertenenciasV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.UBICACION_ESTANCIAS:
                                MSeguimientoMV = MUbicacionEstanciaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.SANCIONES_DISCIPLINARIAS:
                                MSeguimientoMV = MIncidentesV = Visibility.Visible;
                                break;
                            //Tecnico
                            //Programas de Reinsercion
                            case (short)enumProcesos.CREACION_GRUPOS:
                                MSeguimientoMV = MTecnicoMV = MProgramasReinsercionMV = MCreacionGruposV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.MANEJO_GRUPOS:
                                MSeguimientoMV = MTecnicoMV = MProgramasReinsercionMV = MManejoGruposV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.MANEJO_EMPALMES:
                                MSeguimientoMV = MTecnicoMV = MProgramasReinsercionMV = MManejoEmpalmesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.MANEJO_CANCELADO_SUSPENDIDO:
                                MSeguimientoMV = MTecnicoMV = MProgramasReinsercionMV = MManejoCanceladoSuspendidoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CONTROL_CALIFICACIONES:
                                MSeguimientoMV = MTecnicoMV = MProgramasReinsercionMV = MControlCalificacionesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CONTROL_PARTICIPACION:
                                MSeguimientoMV = MTecnicoMV = MProgramasReinsercionMV = MControlParticipacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CONTROL_SANCIONES:
                                MSeguimientoMV = MTecnicoMV = MControlSancionesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.EMI:
                                MSeguimientoMV = MTecnicoMV = MEMIV = Visibility.Visible;
                                break;
                            //Control y Seguridad
                            //Visitas
                            case (short)enumProcesos.PADRON_VISITAS:
                                MControlSeguridadMV = MVisitasMV = MPadronVisitaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.PROGRAMACION_VISITA_POR_EDIFICIO:
                                MControlSeguridadMV = MVisitasMV = MProgramaVisitaEdificioV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.PROGRAMACION_VISITA_POR_APELLIDO:
                                MControlSeguridadMV = MVisitasMV = mProgramaVisitaApellidoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.VERIFICAR_PASE:
                                MControlSeguridadMV = MVisitasMV = MVerificarPaseV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CANCELAR_SUSPENDER_CREDENCIALES:
                                MControlSeguridadMV = MVisitasMV = MCancelarSuspenderCredencialesV = Visibility.Visible;
                                break;
                            //case (short)enumProcesos.SOLICITUD_ATENCION:
                            //MControlSeguridadMV = MSolicitudCitasV = Visibility.Visible;
                            //break;
                            case (short)enumProcesos.REGISTRO_PERSONAL:
                                MControlSeguridadMV = MPersonalV = Visibility.Visible;
                                break;
                            //Abogados
                            case (short)enumProcesos.PADRON_ABOGADOS:
                                MControlSeguridadMV = MAbogadosMV = MPadronAbogadosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.PADRON_COLABORADORES:
                                MControlSeguridadMV = MAbogadosMV = MPadronColaboradoresV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.PADRON_ACTUARIOS:
                                MControlSeguridadMV = MAbogadosMV = MActuariosMV = MPadronActuariosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.REQUERIMIENTO_INTERNOS:
                                MControlSeguridadMV = MAbogadosMV = MActuariosMV = MRequerimientoInternosV = Visibility.Visible;
                                break;
                            //Visitante externo
                            case (short)enumProcesos.VISITA_EXTERNA:
                                MControlSeguridadMV = MVisitaExternaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.RECEPCION_ADUANA:
                                MControlSeguridadMV = MAduanaV = Visibility.Visible;
                                break;
                            //Control Internos
                            case (short)enumProcesos.CONTROL_DE_INTERNOS_EN_EDIFICIOS:
                                MControlSeguridadMV = MControlInternosMV = MControlInternosEdificioV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CONTROL_ACTIVIDADES_PROGRAMADAS:
                                MControlSeguridadMV = MControlInternosMV = MControlActividadesProgramadasV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CONTROL_ACTIVIDADES_NO_PROGRAMADAS:
                                MControlSeguridadMV = MControlInternosMV = MControlActividadesNoProgramadasV = Visibility.Visible;
                                break;
                            //Salida
                            //Excarcelaciones
                            case (short)enumProcesos.EXCARCELACIONES:
                                MSalidaMV = MExcarcelacionesMV = MExcarcelacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.EXCARCELACIONAUTORIZACION:
                                MSalidaMV = MExcarcelacionesMV = MAutorizacionExcarcelacionV = Visibility.Visible;
                                break;
                            //Traslados
                            case (short)enumProcesos.TRASLADOEXTERNO:
                                MSalidaMV = MTrasladoMV = MTrasladoForaneoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.TRASLADOMASIVO:
                                MSalidaMV = MTrasladoMV = MTrasladoMasivoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.AUTORIZAINGRESOTRASLADO:
                                MSalidaMV = MTrasladoMV = MAutorizaIngresoTrasladoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.LIBERACION:
                                MSalidaMV = MLiberacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.LIBERACION_BIOMETRICA:
                                MSalidaMV = MLiberacionBiometriaV = Visibility.Visible;
                                break;
                            //Eventos
                            case (short)enumProcesos.EVENTO:
                                MEventosMV = MProgramacionEventoV = Visibility.Visible;
                                break;
                            //Citas
                            case (short)enumProcesos.SOLICITUD_ATENCION:
                                MCitasMV = MSolicitudCitaV/*MSolicitudCitasV*/ = Visibility.Visible;

                                break;
                            case (short)enumProcesos.CITA:
                                MCitasMV = MAgendarCitaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.ATENCION_CITA_LISTA:
                                MCitasMV = MAtenderCitaV = Visibility.Visible;
                                break;
                            //Liberados
                            case (short)enumProcesos.REGISTRO_LIBERADOS:
                                MLiberadosMV = MRegistroLiberadosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.TRABAJO_SOCIAL:
                                MLiberadosMV = MTrabajoSocialV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.REPORTE_PSICOLOGICO:
                                MLiberadosMV = MReportePsicologicoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.VISITA_DOMICILIARIA:
                                MLiberadosMV = MVisitaDomiciliariaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.EMI_LIBERADOS:
                                MLiberadosMV = MEMILiberadoV = Visibility.Visible;
                                break;
                            //Estudios
                            case (short)enumProcesos.ESTUDIO_SOCIOECONOMICO:
                                MEstudiosMV = MEstudioSocioeconomicoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.REALIZACION_LISTAS_ESTUDIOS:
                                MEstudiosMV = MRealizacionListasV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.ESTUDIO_PERSONALIDAD:
                                MEstudiosMV = MEstudioPersonalidadV = Visibility.Visible;
                                break;
                            //Area Medica
                            case (short)enumProcesos.HISTORIA_CLINICA_DENTAL:
                                MSeguimientoMV = MTecnicoMV = MAreaMedicaMV = MHistoriaClinicaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.HISTORIA_CLINICA:
                                MSeguimientoMV = MTecnicoMV = MAreaMedicaMV = MHistoriaClinicaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.AGENDAMEDICA:
                                MSeguimientoMV = MTecnicoMV = MAreaMedicaMV = MAgendaMedicaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.REAGENDATRASLADOVIRTUAL:
                                MSeguimientoMV = MTecnicoMV = MAreaMedicaMV = MReagendaTVV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.NOTA_MEDICA:
                                MSeguimientoMV = MTecnicoMV = MAreaMedicaMV = MNotaMedicaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.NOTA_MEDICA_ESPECIALISTA:
                                MSeguimientoMV = MTecnicoMV = MAreaMedicaMV = MNotaMedicaEspecialistaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.NOTA_EVOLUCION:
                                MSeguimientoMV = MTecnicoMV = MAreaMedicaMV = MNotaEvolucionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.HOJA_ENFERMERIA:
                                MSeguimientoMV = MTecnicoMV = MAreaMedicaMV = MHojaEnfermeriaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.HOJA_CONTROL_LIQUIDOS:
                                MSeguimientoMV = MTecnicoMV = MAreaMedicaMV = MhojacontrolLiquidosV = Visibility.Visible;
                                break;
                            //hasta aqui se tapa el area medica
                            case (short)enumProcesos.CERTIFICADO_MEDICO_TRASPASOS_CANCELACIONES:
                                MSeguimientoMV = MTecnicoMV = MAreaMedicaMV = MCertificadoTraspasoCancelacionesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.SOLICITUD_ATENCION_ESTATUS:
                                MAreaMedicaMV = MSolicitudAtencionEstatusV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CONSULTA_UNIFICADA:
                                MConsultaUnificadaV = Visibility.Visible;
                                break;
                            //Consulta
                            //Internos
                            case (short)enumProcesos.ENTREVISTA_INICIAL_TRABAJO_SOCIAL:
                                MInternosMV = MFormatosMV = MEntrevistaInicialTrabajoSocialV = Visibility.Visible;
                                break;
                            //Planimetria
                            case (short)enumProcesos.PLANIMETRIA:
                                MPlanimetriaV = Visibility.Visible;
                                break;
                            //Reportes
                            //Visita Legal
                            case (short)enumProcesos.BITACORA_REGISTRO_ABOGADO:
                                MVisitaLegalMV = MBitacoraRegistroAbogadoReporteV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.REPORTE_ABOGADO:
                                MVisitaLegalMV = MPadronAbogadoReporteV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.REPORTE_ABOGADOS_FECHA:
                                MVisitaLegalMV = MRecordVisitaAbogadoMV = MRecordAbogadoReporteFechaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.REPORTE_ABOGADO_INGRESO:
                                MVisitaLegalMV = MRecordVisitaAbogadoMV = MRecordAbogadoReporteIngresoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.REPORTE_ABOGADO_POBLACION_ASIGNADA:
                                MVisitaLegalMV = MRecordVisitaAbogadoMV = MRecordAbogadoReporteIngresoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.REPORTE_EMI_PENDIENTE:
                                MEMIPendientesV = Visibility.Visible;
                                break;
                            //Catalogos
                            case (short)enumProcesos.CAT_MEDIA_FILIACION:
                                MPersonasMV = MFiliacionMV = MMediaFiliacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_FILIACION:
                                MPersonasMV = MFiliacionMV = MTipoFiliacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_ESCOLARIDAD:
                                MPersonasMV = MEscolaridadV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_RELIGION:
                                MPersonasMV = MReligionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_SANGRE:
                                MPersonasMV = MTipoSangreV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_OCUPACION:
                                MPersonasMV = MOcupacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_ETNIAS:
                                MPersonasMV = MEtniaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_PANDILLAS:
                                MPersonasMV = MPandillaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_COMPORTAMIENTO_HOMOSEXUAL:
                                MPersonasMV = MComportamientoHomosexualV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TATUAJES:
                                MPersonasMV = MTatuajesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_CICATRICES:
                                MPersonasMV = MCicatricesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_DEFECTOS:
                                MPersonasMV = MDefectosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_LUNARES:
                                MPersonasMV = MLunaresV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_DISCAPACIDAD:
                                MPersonasMV = MTipoDiscapacidadV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_ABOGADO:
                                MTipoAbogadoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_PAISES:
                                MInstitucionMV = MPaisesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_ESTADOS:
                                MInstitucionMV = MEstadosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_MUNICIPIO:
                                MInstitucionMV = MMunicipiosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_COLONIAS:
                                MInstitucionMV = MColoniasV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_CENTROS:
                                MInstitucionMV = MCeresosMV = MCentrosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_EDIFICIOS:
                                MInstitucionMV = MCeresosMV = MEdificiosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_SECTORES:
                                MInstitucionMV = MCeresosMV = MSectoresV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_CELDAS:
                                MInstitucionMV = MCeresosMV = MCeldasV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_CAMAS:
                                MInstitucionMV = MCeresosMV = MCamasV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_AGENCIAS:
                                MInstitucionMV = MAgenciasV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_JUZGADOS:
                                MInstitucionMV = MJuzgadosV = Visibility.Visible;
                                break;
                          
                            case (short)enumProcesos.CAT_DEPARTAMENTOS:
                                MOrganizacionInternaMV = MDepartamentosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_PROGRAMA:
                                MOrganizacionInternaMV = MProgramaRehabilitacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_AREAS:
                                MOrganizacionInternaMV = MAreasV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_ACTIVIDAD_PROGRAMAS:
                                MOrganizacionInternaMV = MTipoActividadesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_ACTIVIDADES:
                                MOrganizacionInternaMV = MActividadesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_EJES:
                                MOrganizacionInternaMV = MEjesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_DELITOS:
                                MLegalMV = MDelitoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_AUTOS_TERMINOS:
                                MLegalMV = MAutoTerminoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_RECURSO:
                                MLegalMV = MTiposRecursosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_INGRESO:
                                MLegalMV = MTipoIngresoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_ESTUDIO:
                                MEstudioMV = MTipoEstudioV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_MOTIVO_SOLICITUD_ESTUDIO:
                                MEstudioMV = MMotivoSolicitudEstudioV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_VISITANTE:
                                MVisitasCMV = MTipoVisitaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_RELACION:
                                MVisitasCMV = MRelacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_MARCA_MODELO:
                                MDecomisosMV = MMarcasModelosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_FABRICANTE_MODELO:
                                MDecomisosMV = MFabricanteModeloV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_COMPANIAS:
                                MDecomisosMV = MCompaniasV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_UNIDAD_MEDIDA:
                                MDecomisosMV = MUnidadMedidaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_OBJETOS:
                                MDecomisosMV = MObjetoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_GRUPOS_POLICIALES:
                                MDecomisosMV = MGruposPolicialesV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_SECTOR_CLASIFICACION:
                                MPlanimetriaMV = MSectorClasificacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_MENSAJE:
                                MNotificacionesMV = MTipoMensajeV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_AMPARO_INDIRECTO:
                                MCausaPenalCMV = MTipoAmparoIndirectoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_INCIDENTE:
                                MCausaPenalCMV = MTipoIncidenteV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_CONSULTA_UNIFICADA:
                                MConsultaUnificadaCV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_HORARIO_YARDAS:
                                MYardasV = Visibility.Visible;
                                break;
                            //Configurar
                            case (short)enumProcesos.PRIVILEGIOS:
                                MSeguridadMV = MPrivilegiosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.ACTIVACION_CUENTA:
                                MSeguridadMV = MActivacionCuentaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAMBIO_CLAVE_ACCESO:
                                MSeguridadMV = MCambioClaveAccesoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_EQUIPOS_AUTORIZADOS:
                                MSeguridadMV = MEquiposAutorizadosV = Visibility.Visible;
                                break;
                            //case (short)enumProcesos.OPCIONES_GENERALES:
                            //    MOpcionesGeneralesV = Visibility.Visible;
                            //    break;
                            //case (short)enumProcesos.EQUIPO_AREA:
                            //    MEquipoAreaV = Visibility.Visible;
                            //    break;
                            case (short)enumProcesos.CONF_DEPARTAMENTOS:
                                MConfigurarDepartamentosV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CONFIGURACIONDEPARTAMENTOAREATECNICA:
                                MConfigurarDepartamentosAreaTecnicaV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_PARAMETROS:
                                MCatParametroV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.REPORTE_CERTIFICADO_MEDICO_NUEVO_INGRESO:
                                MReporteNotasMedicas = Visibility.Visible;
                                break;
                       }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error cargar opciones del menu", ex);
            }
        }
        #endregion

        #region Menu Enabled
        private void ObtenerMenuEnabled()
        {
            try
            {
                var procesos = new cProceso().ObtenerProcesoMenu(Usuario.Username);
                if (procesos != null)
                {
                    foreach (var p in procesos)
                    {
                        switch (p.ID_PROCESO)
                        {
                            //Registro    
                            case (short)enumProcesos.INGRESO:
                                MRegistro = true;
                                break;
                            case (short)enumProcesos.DECOMISO:
                                MDecomiso = true;
                                break;
                            case (short)enumProcesos.CORRESPONDENCIA:
                                MCorrespondencia = true;
                                break;
                            //Seguimiento
                            //Juridico
                            case (short)enumProcesos.CAUSA_PENAL:
                                MCausaPenal = true;
                                break;
                            case (short)enumProcesos.FICHA:
                                MIdentificacion = true;
                                break;
                            case (short)enumProcesos.ESTATUS_ADMINISTRATIVO:
                                MEstatusAdministrativo = true;
                                break;
                            //Administrativo
                            case (short)enumProcesos.PERTENENCIAS:
                                MPertenencias = true;
                                break;
                            case (short)enumProcesos.UBICACION_ESTANCIAS:
                                MUbicacionEstancia = true;
                                break;
                            case (short)enumProcesos.SANCIONES_DISCIPLINARIAS:
                                MIncidentes = true;
                                break;
                            //Tecnico
                            case (short)enumProcesos.PROGRAMACION_EVENTOS:
                                MProgramacionEventos = true;
                                break;
                            case (short)enumProcesos.NOTIFICACION_TS:
                                MNotificacionTrabajoSocial = true;
                                break;
                            //Programas de Reinsercion
                            case (short)enumProcesos.CREACION_GRUPOS:
                                MCreacionGrupos = true;
                                break;
                            case (short)enumProcesos.MANEJO_GRUPOS:
                                MManejoGrupos = true;
                                break;
                            case (short)enumProcesos.MANEJO_EMPALMES:
                                MManejoEmpalmes = true;
                                break;
                            case (short)enumProcesos.MANEJO_CANCELADO_SUSPENDIDO:
                                MManejoCanceladosSuspendidos = true;
                                break;
                            case (short)enumProcesos.CONTROL_CALIFICACIONES:
                                MControlCalificaciones = true;
                                break;
                            case (short)enumProcesos.CONTROL_PARTICIPACION:
                                MControlParticipantes = true;
                                break;
                            case (short)enumProcesos.CONTROL_SANCIONES:
                                MControlSanciones = true;
                                break;
                            case (short)enumProcesos.EMI:
                                MEMI = true;
                                break;
                            //Control y Seguridad
                            //Visitas
                            case (short)enumProcesos.PADRON_VISITAS:
                                MPadronVisitaFamiliar = true;
                                break;
                            case (short)enumProcesos.PROGRAMACION_VISITA_POR_EDIFICIO:
                                MVisitasPorEdificio = true;
                                break;
                            case (short)enumProcesos.PROGRAMACION_VISITA_POR_APELLIDO:
                                MVisitasPorApellido = true;
                                break;
                            case (short)enumProcesos.VERIFICAR_PASE:
                                MVerificarPases = true;
                                break;
                            case (short)enumProcesos.CANCELAR_SUSPENDER_CREDENCIALES:
                                MCancelarCredenciales = true;
                                break;
                            //case (short)enumProcesos.SOLICITUD_ATENCION:
                            //    MSolicitudCita = true;
                            //    break;
                            case (short)enumProcesos.REGISTRO_PERSONAL:
                                MPadronPersonal = true;
                                break;
                            //Abogados
                            case (short)enumProcesos.PADRON_ABOGADOS:
                                MPadronAbogados = true;
                                break;
                            case (short)enumProcesos.PADRON_COLABORADORES:
                                MPadronColaboradores = true;
                                break;
                            case (short)enumProcesos.PADRON_ACTUARIOS:
                                MPadronActuarios = true;
                                break;
                            case (short)enumProcesos.REQUERIMIENTO_INTERNOS:
                                MRequerimientoInternos = true;
                                break;
                            //Visitante externo
                            case (short)enumProcesos.VISITA_EXTERNA:
                                MPadronVisitaExterna = true;
                                break;
                            case (short)enumProcesos.RECEPCION_ADUANA:
                                MAccesoAduana = true;
                                break;
                            //Control Internos
                            case (short)enumProcesos.CONTROL_DE_INTERNOS_EN_EDIFICIOS:
                                MInternosPorEdificio = true;
                                break;
                            case (short)enumProcesos.CONTROL_ACTIVIDADES_PROGRAMADAS:
                                MProgramas = true;
                                break;
                            case (short)enumProcesos.CONTROL_ACTIVIDADES_NO_PROGRAMADAS:
                                MActividadesNoProgramadas = true;
                                break;
                            //Salida
                            //Excarcelaciones
                            case (short)enumProcesos.EXCARCELACIONES:
                                MExcarcelacion = true;
                                break;
                            case (short)enumProcesos.EXCARCELACIONAUTORIZACION:
                                MAutorizarExcarcelacion = true;
                                break;
                            //Traslados
                            case (short)enumProcesos.TRASLADOEXTERNO:
                                MTrasladoForaneo = true;
                                break;
                            case (short)enumProcesos.TRASLADOMASIVO:
                                MTrasladoMasivo = true;
                                break;
                            case (short)enumProcesos.AUTORIZAINGRESOTRASLADO:
                                MAutorizaIngresoTraslado = true;
                                break;
                            case (short)enumProcesos.LIBERACION:
                                MPreBaja = true;
                                break;
                            case (short)enumProcesos.LIBERACION_BIOMETRICA:
                                MLiberacionBiometria = true;
                                break;
                            //Eventos
                            case (short)enumProcesos.EVENTO:
                                MProgramacionEventos = true;
                                break;
                            //Citas
                            case (short)enumProcesos.SOLICITUD_ATENCION:
                                MSolicitudCita = true;
                                break;
                            case (short)enumProcesos.CITA:
                                MAgendarCita = true;
                                break;
                            case (short)enumProcesos.ATENCION_CITA_LISTA:
                                MAtenderCita = true;
                                break;
                            //Liberados
                            case (short)enumProcesos.REGISTRO_LIBERADOS:
                                MRegistroLiberados = true;
                                break;
                            case (short)enumProcesos.TRABAJO_SOCIAL:

                                break;
                            case (short)enumProcesos.REPORTE_PSICOLOGICO:
                                MReportePsicologicoLiberado = true;
                                break;
                            case (short)enumProcesos.ENTREVISTA_INICIAL_LIBERADOS:
                                MEntrevistaInicialLiberado = true;
                                break;
                            case (short)enumProcesos.VISITA_DOMICILIARIA:
                                MVisitaDomiciliariaLiberado = true;
                                break;
                            case (short)enumProcesos.EMI_LIBERADOS:
                                MEMILiberados = true;
                                break;
                            case (short)enumProcesos.ESCALA_RIESGO:
                                MEscalaRiesgo = true;
                                break;
                            case (short)enumProcesos.PROGRAMA_LIBERTAD:
                                MSeguimientoLibertad = true;
                                break;
                            //Estudios
                            case (short)enumProcesos.ESTUDIO_SOCIOECONOMICO:
                                MEstudioSocioEconomico = true;
                                break;
                            case (short)enumProcesos.REALIZACION_LISTAS_ESTUDIOS:
                                MCreacionListas = true;
                                break;
                            case (short)enumProcesos.ESTUDIO_PERSONALIDAD:
                                MProgramacionEstudios = true;//PROGRAMACION DE ESTUDIOS DE PERSONALIDAD
                                break;
                            case (short)enumProcesos.REALIZACION_ESTUDIOS_PERSONALIDAD:
                                MRealizacionEstudio = true;
                                break;
                            case (short)enumProcesos.CIERRE_ESTUDIOS_PERSONALIDAD:
                                MCierreEstudio = true;//CIERRE DE ESTUDIOS DE PERSONALIDAD
                                break;
                            //Area Medica
                            case (short)enumProcesos.HISTORIA_CLINICA_DENTAL:
                                MHistoriaClinica = true;
                                break;
                            case (short)enumProcesos.HISTORIA_CLINICA:
                                MHistoriaClinica = true;
                                break;
                            case (short)enumProcesos.HOJA_ENFERMERIA:
                                MHojaEnfermeria = true;
                                break;
                            case (short)enumProcesos.HOJA_CONTROL_LIQUIDOS:
                                MhojacontrolLiquidos = true;
                                break;
                            case (short)enumProcesos.AGENDAMEDICA:
                                MAgendaMedica = true;
                                break;
                            case (short)enumProcesos.NOTA_MEDICA:
                                MNotaMedica = true;
                                break;
                            case (short)enumProcesos.NOTA_MEDICA_ESPECIALISTA:
                                MNotaMedicaEspecialista = true;
                                break;
                            case (short)enumProcesos.NOTA_EVOLUCION:
                                MNotaEvolucion = true;
                                break;
                            //hasta aqui se tapa el area medica
                            case (short)enumProcesos.CERTIFICADO_MEDICO_TRASPASOS_CANCELACIONES:
                                MCertificadoMedico = true;
                                break;
                            case (short)enumProcesos.SOLICITUD_ATENCION_ESTATUS:

                                break;
                            case (short)enumProcesos.CONSULTA_UNIFICADA:
                                MConsultaUnificada = true;
                                break;
                            //Consulta
                            //Internos
                            case (short)enumProcesos.ENTREVISTA_INICIAL_TRABAJO_SOCIAL:
                                MEntrevistaInicialTS = true;
                                break;
                            //Planimetria
                            case (short)enumProcesos.PLANIMETRIA:
                                MPlanimetria = true;
                                break;
                            //Reportes
                            //Visita Legal
                            case (short)enumProcesos.BITACORA_REGISTRO_ABOGADO:
                                MReporteBitacoraRegistroAbogado = true;
                                break;
                            case (short)enumProcesos.REPORTE_ABOGADO:
                                MReportePadronAbogados = true;
                                break;
                            case (short)enumProcesos.REPORTE_ABOGADOS_FECHA:
                                MReporteRecordVisitaFecha = true;
                                break;
                            case (short)enumProcesos.REPORTE_ABOGADO_INGRESO:
                                MReporteRecordVisitaIngreso = true;
                                break;
                            case (short)enumProcesos.REPORTE_ABOGADO_POBLACION_ASIGNADA:
                                MReporteAbogadosPoblacionAsignada = true;
                                break;
                            case (short)enumProcesos.REPORTE_EMI_PENDIENTE:
                                MReporteEMIPendiente = true;
                                break;
                            //Catalogos
                            case (short)enumProcesos.CAT_MEDIA_FILIACION:
                                MCatalogoMediaFiliacion = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_FILIACION:
                                MCatalogoTipoFiliacion = true;
                                break;
                            case (short)enumProcesos.CAT_ESCOLARIDAD:
                                MCatalogoEscolaridad = true;
                                break;
                            case (short)enumProcesos.CAT_RELIGION:
                                MCatalogoReligion = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_SANGRE:
                                MCatalogoTipoSangre = true;
                                break;
                            case (short)enumProcesos.CAT_OCUPACION:
                                MCatalogoOcupacion = true;
                                break;
                            case (short)enumProcesos.CAT_ETNIAS:
                                MCatalogoEtnia = true;
                                break;
                            case (short)enumProcesos.CAT_PANDILLAS:
                                MCatalogoPandillas = true;
                                break;
                            case (short)enumProcesos.CAT_COMPORTAMIENTO_HOMOSEXUAL:
                                MCatalogoComportamientoHomosexual = true;
                                break;
                            case (short)enumProcesos.CAT_TATUAJES:
                                MCatalogoTatuajes = true;
                                break;
                            case (short)enumProcesos.CAT_CICATRICES:
                                MCatalogoCicatricez = true;
                                break;
                            case (short)enumProcesos.CAT_DEFECTOS:
                                MCatalogoDefectos = true;
                                break;
                            case (short)enumProcesos.CAT_LUNARES:
                                MCatalogoLunares = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_DISCAPACIDAD:
                                MCatalogoTipoDiscapacidad = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_ABOGADO:
                                MCatalogoTipoAbogado = true;
                                break;
                            case (short)enumProcesos.CAT_PAISES:
                                MCatalogoPaises = true;
                                break;
                            case (short)enumProcesos.CAT_ESTADOS:
                                MCatalogoEstados = true;
                                break;
                            case (short)enumProcesos.CAT_MUNICIPIO:
                                MCatalogoMunicipios = true;
                                break;
                            case (short)enumProcesos.CAT_COLONIAS:
                                MCatalogoColonias = true;
                                break;
                            case (short)enumProcesos.CAT_CENTROS:
                                MCatalogoCentros = true;
                                break;
                            case (short)enumProcesos.CAT_EDIFICIOS:
                                MCatalogoEdificios = true;
                                break;
                            case (short)enumProcesos.CAT_SECTORES:
                                MCatalogoSectores = true;
                                break;
                            case (short)enumProcesos.CAT_CELDAS:
                                MCatalogoCeldas = true;
                                break;
                            case (short)enumProcesos.CAT_CAMAS:
                                MCatalogoCamas = true;
                                break;
                            case (short)enumProcesos.CAT_AGENCIAS:
                                MCatalogoAgencias = true;
                                break;
                            case (short)enumProcesos.CAT_JUZGADOS:
                                MCatalogoJuzgados = true;
                                break;
                            case (short)enumProcesos.CAT_DEPARTAMENTOS:
                                MCatalogoDepartamentos = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_PROGRAMA:
                                //MOrganizacionInternaMV = MProgramaRehabilitacionV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_AREAS:
                                MCatalogoAreas = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_ACTIVIDAD_PROGRAMAS:
                                MCatalogoTiposActividades = true;
                                break;
                            case (short)enumProcesos.CAT_ACTIVIDADES:
                                //MOrganizacionInternaMV = MActividadesV = Visibility.Visible;
                                MCatalogoActividades = true;
                                break;
                            case (short)enumProcesos.CAT_EJES:
                                MCatalogoEjes = true;
                                break;
                            case (short)enumProcesos.CAT_DELITOS:
                                MCatalogoDelitos = true;
                                break;
                            case (short)enumProcesos.CAT_AUTOS_TERMINOS:
                                //MLegalMV = MAutoTerminoV = Visibility.Visible;
                                break;
                            case (short)enumProcesos.CAT_TIPO_RECURSO:
                                MCatalogoTiposRecursos = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_INGRESO:
                                MCatalogoTiposIngreso = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_ESTUDIO:
                                MCatalogoTiposEstudio = true;
                                break;
                            case (short)enumProcesos.CAT_MOTIVO_SOLICITUD_ESTUDIO:
                                MCatalogoMotivosSolicitudEstudios = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_VISITANTE:
                                MCatalogoTiposVisita = true;
                                break;
                            case (short)enumProcesos.CAT_RELACION:
                                MCatalogoRelacion = true;
                                break;
                            case (short)enumProcesos.CAT_MARCA_MODELO:
                                MCatalogoDecomisoMarcaModelo = true;
                                break;
                            case (short)enumProcesos.CAT_FABRICANTE_MODELO:
                                MCatalogoDecomisoFabricanteModelo = true;
                                break;
                            case (short)enumProcesos.CAT_COMPANIAS:
                                MCatalogoDecomisoCompanias = true;
                                break;
                            case (short)enumProcesos.CAT_UNIDAD_MEDIDA:
                                MCatalogoDecomisoUnidadMedida = true;
                                break;
                            case (short)enumProcesos.CAT_OBJETOS:
                                MCatalogoObjetos = true;
                                break;
                            case (short)enumProcesos.CAT_GRUPOS_POLICIALES:
                                MCatalogoGruposPoliciales = true;
                                break;
                            case (short)enumProcesos.CAT_SECTOR_CLASIFICACION:
                                MCatalogoSectorClasificacion = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_MENSAJE:
                                MCatalogoTipoMensaje = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_AMPARO_INDIRECTO:
                                MCatalogoTipoAmparoIndirecto = true;
                                break;
                            case (short)enumProcesos.CAT_TIPO_INCIDENTE:
                                MCatalogoTipoIncidente = true;
                                break;
                            case (short)enumProcesos.CAT_CONSULTA_UNIFICADA:
                                MCatalogoConsultaUnificada = true;
                                break;
                            case (short)enumProcesos.CAT_HORARIO_YARDAS:
                                MCatalogoHorarioYardas = true;
                                break;
                            case (short)enumProcesos.CATALOGOCAMASHOSPITAL:
                                MCatalogoCamasHospital=true;
                                break;
                            //Configurar
                            case (short)enumProcesos.PRIVILEGIOS:
                                MPrivilegios = true;
                                break;
                            case (short)enumProcesos.ACTIVACION_CUENTA:
                                MActivacionCuenta = true;
                                break;
                            case (short)enumProcesos.CAMBIO_CLAVE_ACCESO:
                                MCambioClaveAcceso = true;
                                break;
                            case (short)enumProcesos.CAT_EQUIPOS_AUTORIZADOS:
                                MEquiposAutorizados = true;
                                break;
                            //case (short)enumProcesos.OPCIONES_GENERALES:
                            //    MOpcionesGeneralesV = Visibility.Visible;
                            //    break;
                            //case (short)enumProcesos.EQUIPO_AREA:
                            //    MEquipoAreaV = Visibility.Visible;
                            //    break;
                            case (short)enumProcesos.CONF_DEPARTAMENTOS:
                                MConfiguracionDepartamentos = true;
                                break;
                            case (short)enumProcesos.CONFIGURACIONDEPARTAMENTOAREATECNICA:
                                MConfiguracionDepartamentos = true;
                                break;
                            case (short)enumProcesos.CAT_PARAMETROS:
                                MAdministracionParametros = true;
                                break;
                            case (short)enumProcesos.REPORTE_CERTIFICADO_MEDICO_NUEVO_INGRESO:
                                MReporteNotasMedicasEnable = true;
                                break;


                            //PROCESO >> TODO: REACOMODAR 'CASES'
                            case (short)enumProcesos.NOTA_TECNICA:
                                MNotaTecnica = true;
                                break;
                            case (short)enumProcesos.HISTORIAL_CITAS:
                                MHistorialCitas = true;
                                break;
                            case (short)enumProcesos.VISITAS_LEGALES:
                                MVisitasLegales = true;
                                break;


                            //REPORTES >> TODO: REACOMODAR 'CASES'
                            case (short)enumProcesos.REPORTE_LISTADO_GENERAL:
                                MReporteListadoGeneral = true;
                                break;

                            case (short)enumProcesos.REPORTE_IMPRESION_LISTAS:
                                MReporteImpresionLista = true;
                                break;
                            case (short)enumProcesos.REPORTE_SITUACION_JURIDICA:
                                MReporteSituacionJuridica = true;
                                break;
                            case (short)enumProcesos.REPORTE_CONTROL_INTERNOS_EDIFICIO:
                                MReporteControlInternosEdificio = true;
                                break;
                            case (short)enumProcesos.REPORTE_INTERNOS_UBICACION:
                                MReporteInternosPorUbicacion = true;
                                break;
                            case (short)enumProcesos.REPORTE_INTERNOS_REUBICACIONES:
                                MReporteInternosReubicacion = true;
                                break;
                            case (short)enumProcesos.REPORTE_PAPELETAS:
                                MReportePapeletas = true;
                                break;
                            case (short)enumProcesos.REPORTE_ALTAS_BAJAS:
                                MReporteAltasBajas = true;
                                break;
                            case (short)enumProcesos.REPORTE_TRASLADOS_ESTATALES:
                                MReporteTrasladosEstatales = true;
                                break;
                            case (short)enumProcesos.REPORTE_LISTADO_GENERAL_DELITO:
                                MReporteListadoGeneralDelito = true;
                                break;
                            case (short)enumProcesos.REPORTE_GAFETES_BRAZALETES:
                                MReporteGafetes = true;
                                break;
                            case (short)enumProcesos.REPORTE_CREDENCIAL_BIBLIOTECA:
                                MReporteCredencialBiblioteca = true;
                                break;
                            case (short)enumProcesos.REPORTE_CONTROL_VISITANTES:
                                MReporteControlVisitantes = true;
                                break;
                            case (short)enumProcesos.REPORTE_POBLACION:
                                MReportePoblacion = true;
                                break;
                            case (short)enumProcesos.REPORTE_ESTADISTICA_ALTAS_BAJAS:
                                MReportePoblacionAltasBajas = true;
                                break;
                            case (short)enumProcesos.REPORTE_POBLACION_DELITO:
                                MReportePoblacionPorDelitos = true;
                                break;
                            case (short)enumProcesos.REPORTE_POBLACION_ENTIDAD_PROCEDENCIA:
                                MReportePoblacionEntidadProcedencia = true;
                                break;
                            case (short)enumProcesos.REPORTE_POBLACION_INDIGENA:
                                MReportePoblacionIndigena = true;
                                break;
                            case (short)enumProcesos.REPORTE_POBLACION_EXTRANJERA:
                                MReportePoblacionExtranjera = true;
                                break;
                            case (short)enumProcesos.REPORTE_ACTIVIDADES:
                                MReporteActividades = true;
                                break;
                            case (short)enumProcesos.REPORTE_POBLACION_TERCERA_EDAD:
                                MReportePoblacionTerceraEdad = true;
                                break;
                            case (short)enumProcesos.REPORTE_TOTAL_INGRESOS:
                                MReportePoblacionTotalIngresos = true;
                                break;
                            case (short)enumProcesos.REPORTE_MOTIVOS_SALIDA:
                                MReportePoblacionMotivosSalida = true;
                                break;
                            case (short)enumProcesos.REPORTE_POBLACION_ACTIVA_CIERRE:
                                MReportePoblacionActivaCierre = true;
                                break;
                            case (short)enumProcesos.REPORTE_PRIMERA_VEZ_FUERO_DELITO:
                                MReportePrimeraVezFueroDelito = true;
                                break;
                            case (short)enumProcesos.REPORTE_SENTENCIADO_SEXO_FUERO_DELITO:
                                MReporteSentenciadoSexoFueroDelito = true;
                                break;
                            case (short)enumProcesos.REPORTE_PROCESADO_SEXO_FUERO_DELITO:
                                MReporteProcesadoSexoFueroDelito = true;
                                break;
                            case (short)enumProcesos.REPORTE_CDNH:
                                MReporteCNDH = true;
                                break;
                            case (short)enumProcesos.REPORTE_TIEMPO_COMPURGACION:
                                MReporteTiempoCompurgar = true;
                                break;
                            case (short)enumProcesos.REPORTE_CAUSA_PENAL:
                                MReporteCausaPenal = true;
                                break;
                            case (short)enumProcesos.REPORTE_RELACION_INTERNO_ABOGADO:
                                MReporteRelacionAbogadoInterno = true;
                                break;

                            case (short)enumProcesos.REPORTE_KARDEX_INTERNO:
                                MReporteKardexInterno = true;
                                break;
                            case (short)enumProcesos.REPORTE_LISTADO_INTERNOS_GRUPO:
                                MReporteInternosEnGrupos = true;
                                break;
                            case (short)enumProcesos.REPORTE_LISTADO_GRUPOS_ACTIVOS:
                                MReporteGruposActivos = true;
                                break;
                            case (short)enumProcesos.REPORTE_HORARIOS_GRUPOS:
                                MReporteHorarioGrupo = true;
                                break;
                            case (short)enumProcesos.REPORTE_LISTADO_RESPONSABLES_GRUPO:
                                MReporteResponsableGrupo = true;
                                break;
                            case (short)enumProcesos.REPORTE_HORARIO_RESPONSABLE_GRUPO:
                                MReporteHorarioResponsableGrupo = true;
                                break;
                            case (short)enumProcesos.REPORTE_HORARIO_AREAS:
                                MReporteHorarioAreas = true;
                                break;
                            case (short)enumProcesos.REPORTE_PADRON_EMPLEADO:
                                MReportePadronEmpleado = MReporteAsistenciaPadronEmpleado = true;//Revisar
                                break;
                            case (short)enumProcesos.REPORTE_FORMATO_IDENTIFICACION_FICHA:
                                MReporteFormatoIdentificacion = true;
                                break;
                            case (short)enumProcesos.REPORTE_DECOMISOS_FECHA:
                                MReporteDecomisos = true;
                                break;
                            case (short)enumProcesos.REPORTE_DECOMISO_CUSTODIO:
                                MReporteDecomisoCustodio = true;
                                break;
                            case (short)enumProcesos.REPORTE_DECOMISO_OBJETO:
                                MReporteDecomisoObjeto = true;
                                break;
                            case (short)enumProcesos.REPORTE_ALTO_IMPACTO:
                                MReporteAltoImpacto = true;
                                break;
                            case (short)enumProcesos.REPORTE_REOS_PELIGROSOS:
                                MReporteReosPeligrosos = true;
                                break;
                            case (short)enumProcesos.REPORTE_BIT_ACCESO_ADUANA:
                                MReporteBitacoraAccesoAduana = true;
                                break;
                            case (short)enumProcesos.REPORTE_BIT_CORRESP_POB_PENIT:
                                MReporteBitacoraCorrespondencia = true;
                                break;
                            case (short)enumProcesos.REPORTE_PADRON_VISITA_EXTERNA:
                                MReportePadronVisitaExterna = true;
                                break;
                            case (short)enumProcesos.REPORTE_TOTALES_VISITA:
                                MReporteTotalVisitas = true;
                                break;
                            case (short)enumProcesos.REPORTE_POBLACION_INTERNOS:
                                MReportePoblacionInternos = true;
                                break;
                            case (short)enumProcesos.REPORTE_INGRESOS_EGRESOS:
                                MReporteIngresosEgresos = true;
                                break;
                            case (short)enumProcesos.REPORTE_VISITA_FAMILIAR:
                                MReporteVisitaFamiliar = true;
                                break;
                            case (short)enumProcesos.REPORTE_CONTROL_VISITANTES_DIA:
                                MReporteVisitantesPorDia = true;
                                break;
                            case (short)enumProcesos.REPORTE_CONTROL_VISITANTES_INTIMOS_DIA:
                                MReporteVisitantesPorDiaIntima = true;
                                break;
                            case (short)enumProcesos.REPORTE_BITACORA_TIEMPOS:
                                MReporteBitacoraTiempos = true;
                                break;
                            case (short)enumProcesos.REPORTE_VISITANTES_TRAMITE:
                                MReporteVisitantesTramite = true;
                                break;
                            case (short)enumProcesos.REPORTE_VISITANTES_REGISTRADOS_INTERNO:
                                MReporteVisitantesRegistradosPorInterno = true;
                                break;
                            case (short)enumProcesos.REPORTE_PROGRAMAS_REHABILITACION:
                                MReporteProgramasRehabilitacion = true;
                                break;
                            case (short)enumProcesos.CATALOGOACTIVIDADEJE:
                                MCatalogoActividadEje = true;
                                break;
                            case (short)enumProcesos.CATALOGOESPECIALIDADES:
                                MCatalogoEspecialidades = true;
                                break;
                            case (short)enumProcesos.CATALOGO_DE_ESPECIALISTAS:
                                MCatalogoEspecialistas = true;
                                break;
                            case (short)enumProcesos.CATALOGOSERVICIOSAUXILIARES:
                                MCatalogoServiciosAuxiliaresDiagnostico = true;
                                break;
                            case (short)enumProcesos.CATALOGOTIPOSERVICIOSAUX:
                                MCatalogoTipoServiciosAuxiliaresDiagnostico = true;
                                break;
                            case (short)enumProcesos.CATALOGOSUBTIPOSERVICIOSAUX:
                                MCatalogoSubTipoServiciosAuxiliaresDiagnostico = true;
                                break;
                            case (short)enumProcesos.CATALOGOTIPOATENCIONINTERCONSULTA:
                                MCatalogoTipoAtencionInterconsulta = true;
                                break;
                            case (short)enumProcesos.CAT_PROCEDIMIENTO_MEDICO:
                                MCatalogoProcedimientosMedicos = true;
                                break;
                            case (short)enumProcesos.CAT_PROCEDIMIENTO_SUBTIPO:
                                MCatalogoProcedimientosMedicosSubTipo = true;
                                break;
                            case (short)enumProcesos.CAT_PROCEDIMIENTO_MATERIAL:
                                MCatalogoProcedimientosMateriales = true;
                                break;
                            case (short)enumProcesos.SOLICITUD_CANALIZACION:
                                MSolicitudCanalizacion = true;
                                break;
                            case (short)enumProcesos.ATENCION_CANALIZACION:
                                MAtencionCanalizacion = true;
                                break;
                            case (short)enumProcesos.RESULTADOS_SERVICIOS_AUXILIARES:
                                MResultadoServiciosAuxiliares = true;
                                break;
                            case (short)enumProcesos.AGENDA_ESPECIALISTA:
                                MAgendaEspecialista = true;
                                break;
                            case (short)enumProcesos.CAPTURADEFUNCION:
                                MTarjetaInformativaDeceso = true;
                                break;
                            case (short)enumProcesos.BITACORA_HOSPITALIZACION:
                                MBitacoraHospitalizacion = true;
                                break;
                           case (short)enumProcesos.CATALOGOENFERMEDADES:
                                MCatalogoEnfermedades = true;
                                break;
                            case (short)enumProcesos.CATALOGOMEDICAMENTOS:
                                MCatalogoMedicamentos = true;
                                break;
                            case (short)enumProcesos.CATALOGOMEDICAMENTO_CATEGORIAS:
                                MCatalogoMedicamento_Categorias = true;
                                break;
                            case (short)enumProcesos.CATALOGOMEDICAMENTO_SUBCATEGORIAS:
                                MCatalogoMedicamento_Subcategorias = true;
                                break;
                            case (short)enumProcesos.CATALOGO_PATOLOGICOS:
                                MCatalogo_Patologicos = true;
                                break;
                            case (short)enumProcesos.AGENDAENFERMERO:
                                MAgendaEnfermero=true;
                                break;
                            case (short)enumProcesos.PROGRAMAS_LIBERTAD:
                                MProgramasLibertad = true;
                                break;
                            case (short)enumProcesos.UNIDAD_RECEPTORA:
                                MUnidadReceptora = true;
                                break;
                            case (short)enumProcesos.BITACORA_VISITA_FAMILIAR:
                                MReporteBitacoraVisitaFamiliar = true;
                                break;
                            case (short)enumProcesos.REAGENDATRASLADOVIRTUAL:
                                MReagendaTV = true;
                                break;
                            case (short)enumProcesos.CAT_INSTITUCIONES_MEDICAS:
                                MCatalogoInstitucionesMedicas = true;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error cargar opciones del menu", ex);
            }
        }

        #endregion

        #region Control de Session
        private bool ActualizarSesion(string activo = "N")
        {
            try
            {
                var s = new SESION();
                s.ID_SESION = GlobalVar.gSesion;
                s.FECHA_CONTROL = Fechas.GetFechaDateServer;
                s.ACTIVO = activo;
                return new cSesion().Actualizar(s);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la sesion", ex);
                return false;
            }
        }
        #endregion
    }
}
