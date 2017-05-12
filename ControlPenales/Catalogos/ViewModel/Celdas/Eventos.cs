using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoCeldasViewModel
    {
        private ICommand _buscarClick;
        public ICommand BuscarClick
        {
            get { return _buscarClick ?? (_buscarClick = new RelayCommand(ClickEnter)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch)); }
        }

        public ICommand CeldasLoading
        {
            get { return new DelegateCommand<CatalogoCeldasView>(CeldasLoad); }
        }
    }
}
