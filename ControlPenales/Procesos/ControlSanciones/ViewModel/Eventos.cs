using System.Windows.Input;

namespace ControlPenales
{
    partial class ControlSancionesViewModel
    {
        #region Load
        public ICommand ControlSancionesLoading
        {
            get
            {
                return new DelegateCommand<ControlSancionesView>(OnLoad);
            }
        }
        #endregion

        #region Click
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        #endregion
    }
}
