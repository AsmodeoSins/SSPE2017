using System.Windows.Input;

namespace ControlPenales
{
    partial class ProgramaLibertadViewModel
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

        public ICommand Loading
        {
            get { return new DelegateCommand<ProgramaLibertadView>(Load); }
        }
    }
}