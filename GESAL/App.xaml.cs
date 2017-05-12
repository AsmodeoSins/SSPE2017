

using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
namespace GESAL
{  /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>    
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-MX"); ;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-MX"); ;

            FrameworkElement.LanguageProperty.OverrideMetadata(
              typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture
.IetfLanguageTag)));


            base.OnStartup(e);
            var _view = new GESAL.Views.Login.LoginView();
            var _viewModel = new GESAL.ViewModels.Login.LoginViewModel();
            _view.DataContext = _viewModel;
            _view.Show();
        }
    }
}