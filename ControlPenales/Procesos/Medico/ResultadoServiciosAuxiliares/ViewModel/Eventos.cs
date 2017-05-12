namespace ControlPenales
{
    partial class ResultadoTratamientoServAuxViewModel
    {
        #region Busqueda
        private System.Windows.Input.ICommand modelClick;
        public System.Windows.Input.ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(ModelEnter));
            }
        }

        private System.Windows.Input.ICommand _onClick;
        public System.Windows.Input.ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

        private System.Windows.Input.ICommand buscarClick;
        public System.Windows.Input.ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }

        private System.Windows.Input.ICommand _CargarMasResultados;
        public System.Windows.Input.ICommand CargarMasResultados
        {
            get
            {
                return _CargarMasResultados ?? (_CargarMasResultados = new RelayCommand(async (e) =>
                {
                    if (((System.Windows.Controls.ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((System.Windows.Controls.ScrollChangedEventArgs)e).ExtentHeight - ((System.Windows.Controls.ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((System.Windows.Controls.ScrollChangedEventArgs)e).VerticalOffset == (((System.Windows.Controls.ScrollChangedEventArgs)e).ExtentHeight - ((System.Windows.Controls.ScrollChangedEventArgs)e).ViewportHeight))
                            if (SeguirCargando)
                                ListExpediente.InsertRange(await SegmentarResultadoBusqueda(Pagina));
                }));
            }
        }
        #endregion

        public System.Windows.Input.ICommand LoadingresultadosServAux
        {
            get { return new DelegateCommand<ResultadosServiciosAuxiliaresView>(ResultadosServiciosLoading); }
        }

        private System.Windows.Input.ICommand cmdModelChanged;
        public System.Windows.Input.ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }
    }
}