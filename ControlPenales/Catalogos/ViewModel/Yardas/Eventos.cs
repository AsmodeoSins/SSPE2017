using System.Windows.Input;

namespace ControlPenales
{
    partial class YardasViewModel
    {
        #region Click
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
        #endregion

        #region Load
        public ICommand Loading
        {
            get { return new DelegateCommand<YardasView>(PageLoad); }
        }
        #endregion
    }
}
