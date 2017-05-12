using System.Windows.Input;

namespace ControlPenales
{
    partial class EquiposViewModel
    {
        #region Click
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }
        #endregion

        #region Load
        public ICommand EquiposLoading
        {
            get { return new DelegateCommand<EquiposView>(WindowLoad); }
        }
        #endregion
    }
}