using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoAmparoIndirectoTipoViewModel
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

        public ICommand AmparoIndirectoTipoLoading
        {
            get { return new DelegateCommand<AmparoIndirectoTipoView>(AmparoIndirectoTipoLoad); }
        }
    }
}