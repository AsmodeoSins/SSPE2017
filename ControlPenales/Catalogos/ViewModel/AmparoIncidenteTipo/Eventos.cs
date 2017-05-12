using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoAmparoIncidenteTipoViewModel
    {
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand _buscarClick;
        public ICommand BuscarClick
        {
            get { return _buscarClick ?? (_buscarClick = new RelayCommand(ClickEnter)); }
        }

        // cargando...
        public ICommand AmparoIncidenteTipoLoading
        {
            get { return new DelegateCommand<AmparoIncidenteTipoView>(AmparoIncidenteTipoLoad); }
        }
    }
}