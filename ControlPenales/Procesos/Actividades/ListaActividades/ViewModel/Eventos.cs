using System.Windows.Input;
namespace ControlPenales
{
    public partial class ActividadesViewModel
    {
        private ICommand mouseDoubleClickCommand;
        public ICommand MouseDoubleClickCommand
        {
            get
            {
                return mouseDoubleClickCommand ?? (mouseDoubleClickCommand = new RelayCommand(DoubleClick));
            }
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(ClickSwitch));
            }
        }


        public ICommand ActividadesLoading
        {
            get { return new DelegateCommand<ActividadesView>(ActividadesLoad); }
        }

        private ICommand actividadesLoadingPorFecha;
        public ICommand ActividadesLoadingPorFecha
        {
            get { return actividadesLoadingPorFecha ?? (actividadesLoadingPorFecha = new RelayCommand(ObtenerActividadesPorFecha)); }
        }
    }
}
