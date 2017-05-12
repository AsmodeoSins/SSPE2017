using System.Windows.Input;

namespace ControlPenales
{
    partial class ProcedimientosSubtipoViewModel
    {
        private ICommand _buscarClick;
        public ICommand BuscarClick
        {
            get { return _buscarClick ?? (_buscarClick = new RelayCommand(ClickEnter)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        // cargando...
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<ProcedimientosSubtipoView>(ProcedimientoSubtipoLoad); }
        }
    }
}