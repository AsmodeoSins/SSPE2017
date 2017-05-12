using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoTipoMensajeViewModel
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

        //loading 
        public ICommand TipoMensajeLoading
        {
            get { return new DelegateCommand<CatalogoTipoMensajeView>(TipoMensajeLoad); }
        }
    }
}
