using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteHorarioResponsableGruposViewModel
    {
        #region Click
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(SwitchClick));
            }
        }
        #endregion

        #region Load
        public ICommand OnLoading
        {
            get { return new DelegateCommand<ReporteHorarioResponsableGruposView>(OnLoad); }
        }
        #endregion
    }
}
