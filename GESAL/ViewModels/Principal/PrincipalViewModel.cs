using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.ViewModels;
using GESAL.Models;
using System.Timers;
using MVVMShared.Data.WPF;
using MVVMShared.Commands;
using System.Windows.Input;
//Este namespace contiene las clases de implementacion de Dialogos desde la vista modelo en github de mahapps.metro que se supone debe de salir
//en la version 1.2.0. Podemos depreciarlo cuando utilicemos esa version en nuestras aplicaciones cambiandolo a Mahapps.Metro.Controls.Dialogs
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
namespace GESAL.ViewModels.Principal
{
    public class PrincipalViewModel : ValidationViewModelBase
    {

        //notificaciones
        private System.Timers.Timer aTimer;
        private DialogCoordinator _dialogCoordinator;
        #region contructor
        public PrincipalViewModel(Usuario usuario, DialogCoordinator dialogCoordinator)
        {
            this.usuario = usuario;
            Logeado = string.Format("USUARIO: {0}", this.usuario.Username).ToUpper();

            //NotificacionesVisible = false;
            //SNoNotificaciones = "0";
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 300000;//se refrescara cada 5 min
            aTimer.Enabled = true;

            _dialogCoordinator=dialogCoordinator;
            //MenuPrincipalVisible = false;
        }
        #endregion

        #region Propiedades

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

        private ContentControlBag contentControlBag;
        public ContentControlBag ContentControlBag
        {
            get { return contentControlBag; }
            set { contentControlBag = value; OnPropertyChanged("ContentControlBag"); }
        }

        #endregion

        #region Metodos
        public async void ChangeViewModel(object param)
        {
            if (param!=null)
            {
                if (StaticSourcesViewModel.SourceChanged)
                {

                    if (await (_dialogCoordinator.ShowMessageAsync(this,"Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?",MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative))==MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }

                var params1 = (object[])param;
                var model = Type.GetType((params1[0]).ToString());
                var view = Type.GetType((params1[1]).ToString());
                if (model == null || view == null)
                    return;
                object vistamodelo = null;
                switch (model.Name)
                {
                    case "CatAlmacenViewModel": case "CatProducto_CategoriaViewModel":case "CatProducto_SubCategoriaViewModel":case "CatProducto_TipoViewModel":
                    case "CatProductoViewModel": case "CapturaOCViewModel": case "CalendarizacionViewModel":case "EntradasAlmacenesViewModel":case "RegistroTraspasosExternosViewModel":
                    case "RequisicionExtraordinariaPrincipalViewModel":
                        //var controller = await _dialogCoordinator.ShowProgressAsync(this, "Por favor espere un momento", string.Empty);
                        StaticSourcesViewModel.ShowProgressLoading();
                        await Task.Factory.StartNew(() =>{
                            vistamodelo = Activator.CreateInstance(model, new object[] { DialogCoordinator.Instance, Usuario });
                        });
                        StaticSourcesViewModel.CloseProgressLoading();
                        ContentControlBag = new ContentControlBag(Activator.CreateInstance(view), vistamodelo);
                        break;
                    default:
                        StaticSourcesViewModel.ShowProgressLoading();
                        await Task.Factory.StartNew(() =>{
                            vistamodelo = Activator.CreateInstance(model, new object[] { DialogCoordinator.Instance});
                        });
                        StaticSourcesViewModel.CloseProgressLoading();
                        ContentControlBag = new ContentControlBag(Activator.CreateInstance(view), vistamodelo);
                        break;
                }
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //nNoNotificaciones += 2;
            //if (nNoNotificaciones < 100)
            //    SNoNotificaciones = nNoNotificaciones.ToString();
            //else
            //    SNoNotificaciones = "99+";
            //if (nNoNotificaciones > 0)
            //    NotificacionesVisible = true;
            //else
            //    NotificacionesVisible = false;
        }

        private async void OnLoad(object sender)
        {
            PopUpsViewModels.MainWindow = (Views.Principal.PrincipalView)(sender);
            object vistamodelo = null;
            var model = Type.GetType("GESAL.ViewModels.BandejaEntradaViewModel");
            var view = Type.GetType("GESAL.Views.BandejaEntradaView");
            StaticSourcesViewModel.ShowProgressLoading();
            await Task.Factory.StartNew(() =>
            {
                vistamodelo = Activator.CreateInstance(model, new object[] { Usuario });
            });
            StaticSourcesViewModel.CloseProgressLoading();
            ContentControlBag = new ContentControlBag(Activator.CreateInstance(view), vistamodelo);
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

        private ICommand cmdLoad;
        public ICommand CmdLoad
        {
            get { return cmdLoad??(cmdLoad=new RelayCommand(OnLoad));}
        }
        #endregion
    }
}
