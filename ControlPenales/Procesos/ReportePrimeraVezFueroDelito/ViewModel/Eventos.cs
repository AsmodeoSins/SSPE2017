namespace ControlPenales
{
    public partial class ReportePrimeraVezFueroDelitoViewModel 
    {
        #region Click
        private System.Windows.Input.ICommand _onClick;
        public System.Windows.Input.ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(SwitchClick));
            }
        }
        #endregion

        #region Load
        public System.Windows.Input.ICommand WindowLoading
        {
            get { return new DelegateCommand<ReportePrimeraVezFueroDelitoView>(OnLoad); }
        }
        #endregion
    }
}