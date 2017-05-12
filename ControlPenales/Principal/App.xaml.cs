using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
namespace ControlPenales
{
    public class EntryPoint
    {
        //static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
        [STAThread]
        public static void Main()
        {
            try
            {
                SplashScreen splashScreen = new SplashScreen("login/loadapp.png");
                splashScreen.Show(true);
                ControlPenales.App app = new ControlPenales.App();
                app.InitializeComponent();
                app.Run(); 
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
            //if (mutex.WaitOne(TimeSpan.Zero, true))
            //{
                
            //    mutex.ReleaseMutex();
            //}
            //else
            //{
            //    MessageBox.Show("Ya existe una instancia de la aplicación ejecutandose");
            //}
        }
    }
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>    
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-MX"); ;
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-MX"); ;

                FrameworkElement.LanguageProperty.OverrideMetadata(
                  typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

                base.OnStartup(e);

                var _view = new ControlPenales.LoginView();
                var _viewModel = new LoginViewModel();
                _view.DataContext = _viewModel;
                _view.Show();

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
