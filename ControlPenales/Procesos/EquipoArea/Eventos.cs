using System.Windows.Input;

namespace ControlPenales
{
    partial class EquipoAreaViewModel
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
        #endregion

        #region Load
        public ICommand EquipoAreaLoading
        {
            get { return new DelegateCommand<EquipoAreaView>(WindowLoad); }
        }
        #endregion
    }
}