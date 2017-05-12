using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlPenales
{
    public class StaticSourcesViewModel
    {
        public static Usuario UsuarioLogin;

        #region [Loading]
        private static Visibility _ShowLoading = Visibility.Collapsed;
        public static Visibility ShowLoading
        {
            get { return _ShowLoading; }
            set
            {
                if (_ShowLoading == value) return;

                if (value == Visibility.Collapsed || value == Visibility.Hidden)
                    PopUpsViewModels.EnabledMenu = true;
                else
                    PopUpsViewModels.EnabledMenu = false;

                _ShowLoading = value;
                RaiseStaticPropertyChanged("ShowLoading");
            }
        }
        #endregion

        #region [LockMenu]
        public static int TIMETOLOCK { get { return (int)new TimeSpan(0/*Dias*/, 5/*Horas*/, 0/*Minutos*/, 0/*Segundos*/).TotalSeconds; } }
        private static int? _TimerLock = TIMETOLOCK;
        public static int? TimerLock
        {
            get { return StaticSourcesViewModel._TimerLock; }
            set
            {
                StaticSourcesViewModel._TimerLock = value;
                RaiseStaticPropertyChanged("TimerLock");
            }
        }

        public static async void LockMenu()
        {
            await Task.Factory.StartNew(async () =>
            {
                var FECHASERVER = Fechas.GetFechaDateServer.Date;
                do
                {
                    if (DateTime.Now.Date != FECHASERVER)
                        TimerLock = 0;
                    else
                    {
                        TimerLock--;
                        await TaskEx.Delay(1000);
                    }
                } while (TimerLock != 0);
                PopUpsViewModels.LockMenu = Visibility.Collapsed;
                PopUpsViewModels.UnLockMenu = Visibility.Visible;
                PopUpsViewModels.FocusBlock = true;
            });
        }
        #endregion

        #region [Mensaje Progreso]
        private static Visibility _ShowProgressDialog = Visibility.Collapsed;
        public static Visibility ShowProgressDialog { get { return _ShowProgressDialog; } }

        private static Visibility _ShowProgressRing = Visibility.Collapsed;
        public static Visibility ShowProgressRing { get { return _ShowProgressRing; } }
        private static Visibility _ShowProgressBar = Visibility.Collapsed;
        public static Visibility ShowProgressBar { get { return _ShowProgressBar; } }
        private static Visibility _ShowMetroProgressBar = Visibility.Collapsed;
        public static Visibility ShowMetroProgressBar { get { return _ShowMetroProgressBar; } }

        private static string _ProgressMessage;
        public static string ProgressMessage { get { return _ProgressMessage; } }

        public enum TipoLoader
        {
            PROGRESSRING,
            PROGRESSBAR,
            METROPROGRESSBAR
        }

        public static void ShowMensajeProgreso(string Mensaje, TipoLoader Loader = TipoLoader.PROGRESSRING)
        {
            try
            {
                switch (Loader)
                {
                    case TipoLoader.PROGRESSRING:
                        _ShowProgressRing = Visibility.Visible;
                        RaiseStaticPropertyChanged("ShowProgressRing");
                        break;
                    case TipoLoader.PROGRESSBAR:
                        _ShowProgressBar = Visibility.Visible;
                        RaiseStaticPropertyChanged("ShowProgressBar");
                        break;
                    case TipoLoader.METROPROGRESSBAR:
                        _ShowMetroProgressBar = Visibility.Visible;
                        RaiseStaticPropertyChanged("ShowMetroProgressBar");
                        break;
                    default:
                        break;
                }

                _ProgressMessage = Mensaje;
                RaiseStaticPropertyChanged("ProgressMessage");

                _ShowProgressDialog = Visibility.Visible;
                RaiseStaticPropertyChanged("ShowProgressDialog");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el mensaje de progreso", ex);
            }
        }

        public static void CloseMensajeProgreso()
        {
            try
            {
                _ShowProgressRing = Visibility.Collapsed;
                RaiseStaticPropertyChanged("ShowProgressRing");
                _ShowMetroProgressBar = Visibility.Collapsed;
                RaiseStaticPropertyChanged("ShowMetroProgressBar");
                _ShowMetroProgressBar = Visibility.Collapsed;
                RaiseStaticPropertyChanged("ShowMetroProgressBar");

                _ProgressMessage = string.Empty;
                RaiseStaticPropertyChanged("ProgressMessage");

                _ShowProgressDialog = Visibility.Collapsed;
                RaiseStaticPropertyChanged("ShowProgressDialog");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar el mensaje de progreso", ex);
            }
        }
        #endregion

        #region [Mensaje Error]

        private ICommand _ErrorCommand;
        public ICommand ErrorCommand
        {
            get { return _ErrorCommand ?? (_ErrorCommand = new RelayCommand(CommandMensajeError)); }
        }

        private ICommand _CopyCommand;
        public ICommand CopyCommand
        {
            get { return _CopyCommand ?? (_CopyCommand = new RelayCommand(CommandCopiarError)); }
        }

        private static Visibility _ShowErrorDialog = Visibility.Collapsed;
        public static Visibility ShowErrorDialog
        {
            get { return StaticSourcesViewModel._ShowErrorDialog; }
            set
            {
                StaticSourcesViewModel._ShowErrorDialog = value;
                RaiseStaticPropertyChanged("ShowErrorDialog");
            }
        }

        private static string _ErrorMessage = string.Empty;
        public static string ErrorMessage { get { return _ErrorMessage.ToUpper(); } }
        private static string _ErrorMessageDetalle = string.Empty;
        public static string ErrorMessageDetalle { get { return _ErrorMessageDetalle.ToUpper(); } }
        private static string _ClipMessageDetalle = string.Empty;
        public static string ClipMessageDetalle { get { return _ClipMessageDetalle.ToUpper(); } }
        private static string _TituloMessage = "NO HAY MENSAJE DE ERROR";
        public static string TituloMessage { get { return _TituloMessage.ToUpper(); } }

        private static bool _TabSetSelect = true;
        public static bool TabSetSelect
        {
            get { return StaticSourcesViewModel._TabSetSelect; }
            set { StaticSourcesViewModel._TabSetSelect = value; }
        }


        public static void ShowMessageError(string Titulo, string Mensaje, Exception ex)
        {
            _TituloMessage = Titulo;
            RaiseStaticPropertyChanged("TituloMessage");
            _ErrorMessage = Mensaje + "\n\nContacte a un administrador";
            RaiseStaticPropertyChanged("ErrorMessage");
            _TabSetSelect = true;
            RaiseStaticPropertyChanged("TabSetSelect");

            var trace = new System.Diagnostics.StackTrace(ex, true);

            var DetalleMensaje = new StringBuilder();
            DetalleMensaje.AppendLine(">>>[Excepción Principal]");
            DetalleMensaje.AppendLine(ex.Message + (ex.InnerException != null ? "\n" : string.Empty));
            if (ex.InnerException != null)
            {
                var innermesaje = true;
                var innerobject = ex.InnerException;
                DetalleMensaje.AppendLine(">>>[Excepción Interna]");

                do
                {
                    DetalleMensaje.AppendLine((innerobject != null ? innerobject.Message : string.Empty));
                    innerobject = innerobject.InnerException;
                    if (innerobject == null)
                        innermesaje = false;
                }
                while (innermesaje);
            }
            DetalleMensaje.AppendLine("\n>>>[Ubicación del Problema]");
            DetalleMensaje.AppendLine("Clase: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().ReflectedType.FullName);
            DetalleMensaje.AppendLine("Metodo: " + trace.GetFrame((trace.FrameCount - 1)).GetMethod().Name);
            DetalleMensaje.AppendLine("Linea: " + trace.GetFrame((trace.FrameCount - 1)).GetFileLineNumber());
            DetalleMensaje.AppendLine("Columna: " + trace.GetFrame((trace.FrameCount - 1)).GetFileColumnNumber());

            _ErrorMessageDetalle = DetalleMensaje.ToString();
            RaiseStaticPropertyChanged("ErrorMessageDetalle");

            ShowErrorDialog = Visibility.Visible;
        }

        public static void CommandMensajeError(object obj)
        {
            if (obj is MahApps.Metro.Controls.MetroAnimatedSingleRowTabControl)
            {
                var metrotab = (MahApps.Metro.Controls.MetroAnimatedSingleRowTabControl)obj;
                metrotab.SelectedIndex = ((TabItem)metrotab.SelectedItem).Name == "TabMensaje" ? 1 : 0;
            }
            else
            {
                ShowErrorDialog = Visibility.Collapsed;
                PopUpsViewModels.EnabledMenu = true;
            }

        }

        private void CommandCopiarError(object obj)
        {
            Clipboard.SetText(ErrorMessageDetalle);

            if (!ClipMessageDetalle.Equals(string.Empty))
                return;
            _ClipMessageDetalle = "Mensaje Copiado al portapapeles".ToUpper();
            RaiseStaticPropertyChanged("ClipMessageDetalle");
            Task.Factory.StartNew(async () =>
            {
                await TaskEx.Delay(2500);
                _ClipMessageDetalle = string.Empty;
                RaiseStaticPropertyChanged("ClipMessageDetalle");
            });
        }
        #endregion

        #region [Toast Notification]
        public enum enumTipoMensaje
        {
            MENSAJE_CORRECTO,
            MENSAJE_ERROR,
            MENSAJE_INFORMACION,
            MESNAJE_ADVERTENCIA
        }

        private static string _Title;
        public static string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;
                _Title = value;
                RaiseStaticPropertyChanged("Title");
            }
        }

        private static string _Message;
        public static string Message
        {
            get { return _Message; }
            set
            {
                if (_Message == value) return;
                _Message = value;
                RaiseStaticPropertyChanged("Message");
            }
        }

        private static int _Duration = 3;
        public static int Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration == value) return;
                _Duration = value;
                RaiseStaticPropertyChanged("Duration");
            }
        }

        private static Brush _BackgroudMessage;
        public static Brush BackgroudMessage
        {
            get { return _BackgroudMessage; }
            set
            {
                _BackgroudMessage = value;
                RaiseStaticPropertyChanged("BackgroudMessage");
            }
        }

        private static string _Image;
        public static string Image
        {
            get { return _Image; }
            set
            {
                if (_Image == value) return;
                _Image = value;
                RaiseStaticPropertyChanged("Image");
            }
        }

        private static string _TextWrapping = "WrapWithOverflow";
        public static string TextWrapping
        {
            get { return _TextWrapping; }
            set
            {
                if (_TextWrapping == value) return;
                _TextWrapping = value;
                RaiseStaticPropertyChanged("TextWrapping");
            }
        }

        public static void Mensaje(string Titulo, string Mensaje, enumTipoMensaje TipoMensaje, int Duracion = 3)
        {
            try
            {
                Message = null;
                Duration = Duracion;
                switch (TipoMensaje)
                {
                    case enumTipoMensaje.MENSAJE_CORRECTO:
                        BackgroudMessage = new SolidColorBrush(Color.FromRgb(59, 145, 63));
                        Image = @"..\Imagen\correcto-verde-icono-esta-bien.png";
                        break;
                    case enumTipoMensaje.MENSAJE_ERROR:
                        BackgroudMessage = new SolidColorBrush(Color.FromRgb(230, 65, 58));
                        Image = @"..\Imagen\aspa-roja-con-borde-incorrecto.png";
                        break;
                    case enumTipoMensaje.MENSAJE_INFORMACION:
                        BackgroudMessage = new SolidColorBrush(Color.FromRgb(71, 136, 200));
                        Image = @"..\Imagen\information-icon.png";
                        break;
                    case enumTipoMensaje.MESNAJE_ADVERTENCIA:
                        BackgroudMessage = new SolidColorBrush(Color.FromRgb(252, 210, 9));
                        Image = @"..\Imagen\notification_warning.png";
                        break;
                    default:
                        break;
                }
                Title = Titulo;
                Message = Mensaje;
                TextWrapping = "WrapWithOverflow";
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al lanzar mensaje", ex);
            }

        }
        #endregion

        #region [Propiedades Estaticas]
        private static bool _SalidaEspecialista = false;
        public static bool SalidaEspecialista
        {
            get { return StaticSourcesViewModel._SalidaEspecialista; }
            set { StaticSourcesViewModel._SalidaEspecialista = value; }
        }
        private static bool _EspecialistaCambiosCancelados = false;
        public static bool EspecialistaCambiosCancelados
        {
            get { return StaticSourcesViewModel._EspecialistaCambiosCancelados; }
            set { StaticSourcesViewModel._EspecialistaCambiosCancelados = value; }
        }
        private static bool _SourceChanged;
        public static bool SourceChanged
        {
            get { return StaticSourcesViewModel._SourceChanged; }
            set { StaticSourcesViewModel._SourceChanged = value; }
        }
        #endregion

        #region [Footer]
        private static Visibility _ShowFooter = Visibility.Collapsed;
        public static Visibility ShowFooter
        {
            get { return _ShowFooter; }
            set
            {
                _ShowFooter = value;
                RaiseStaticPropertyChanged("ShowFooter");
            }
        }

        private static ICommand _LabelClicked;
        public static ICommand LabelClicked
        {
            get { return _LabelClicked ?? (_LabelClicked = new RelayCommand(TopWindow)); }
        }

        public static void TopWindow(object obj)
        {
            PopUpsViewModels.MainWindow.ScrollContent.ScrollToTop();
        }
        #endregion

        #region [Consultores Asincronos]
        /// <summary>
        /// Metodo auxiliar para ejecutar metodos con varios querys
        /// </summary>
        /// <param name="method">Metodo que contiene todos los q  uerys</param>
        public static async Task CargarDatosMetodoAsync(Action method)
        {
            try
            {
                var currentCulture = CultureInfo.CurrentUICulture;
                StaticSourcesViewModel.ShowLoading = Visibility.Visible;
                await Task.Factory.StartNew(() => 
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture;
                        Thread.CurrentThread.CurrentUICulture = currentCulture;
                        method();
                    });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el proceso", ex);
            }
            finally
            {
                StaticSourcesViewModel.ShowLoading = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Metodo auxiliar para ejecutar metodos con varios querys
        /// </summary>
        /// <param name="method">Metodo que contiene todos los querys</param>
        public static async void CargarDatosMetodo(Action method)
        {
            try
            {
                StaticSourcesViewModel.ShowLoading = Visibility.Visible;
                var tcs = new TaskCompletionSource<Action>();
                tcs.SetResult(method);
                await tcs.Task;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el proceso", ex);
            }
            finally
            {
                StaticSourcesViewModel.ShowLoading = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Metodo para ejecutar tareas asincronamente
        /// </summary>
        /// <typeparam name="T"> tipo de dato que regresara la funcion</typeparam>
        /// <param name="predicade">funcion con la tarea programada</param>
        /// <returns></returns>
        public static async Task<T> CargarDatosAsync<T>(Func<T> predicade)
        {
            T result = default(T);
            try
            {
                StaticSourcesViewModel.ShowLoading = Visibility.Visible;
                result = await Task.Factory.StartNew<T>(predicade);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el proceso", ex);
            }
            finally
            {
                StaticSourcesViewModel.ShowLoading = Visibility.Hidden;
            }
            return result;
        }

        /// <summary>
        /// Metodo auxiliar para ejecutar query
        /// </summary>
        /// <typeparam name="T">Tipo del Objeto que regresara el query</typeparam>
        /// <param name="predicade">Query en forma de expresion lambda </param>
        /// <returns></returns>
        public static async Task<T> CargarDatos<T>(Func<T> predicade)
        {
            T result = default(T);
            try
            {
                StaticSourcesViewModel.ShowLoading = Visibility.Visible;
                var tcs = new TaskCompletionSource<T>();
                tcs.SetResult(predicade());
                result = await tcs.Task;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error el proceso", ex);
            }
            finally
            {
                StaticSourcesViewModel.ShowLoading = Visibility.Hidden;
            }
            return (T)result;
        }
        #endregion

        /// <summary>
        /// Metodo auxiliar para ejecutar procesos asincronos
        /// </summary>
        /// <typeparam name="T">Objeto que regresa el proceso</typeparam>
        /// <param name="Mensaje">Mensaje para el usuario</param>
        /// <param name="predicade">Proceso</param>
        /// <param name="Tipo_Loader">Tipo de loading</param>
        /// <returns>Respuesta del Proceso</returns>
        public static async Task<T> OperacionesAsync<T>(string Mensaje, Func<T> predicade, TipoLoader Tipo_Loader = TipoLoader.PROGRESSRING)
        {
            T result = default(T);
            try
            {
                StaticSourcesViewModel.ShowMensajeProgreso(Mensaje, Tipo_Loader);
                result = await Task.Factory.StartNew<T>(predicade);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el proceso", ex);
            }
            finally
            {
                StaticSourcesViewModel.CloseMensajeProgreso();
            }
            return result;
        }

        #region [Aux]
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged(string propName)
        {
            EventHandler<PropertyChangedEventArgs> handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
