namespace ControlPenales
{
    partial class RecetaMedicaViewModel 
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
        #endregion

        #region Pagina Cargada
        public System.Windows.Input.ICommand RecetaMedicaLoading
        {
            get { return new DelegateCommand<RecetaMedicaView>(LoadRecetaMedica); }
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

        private System.Windows.Input.ICommand cmdModelChanged;
        public System.Windows.Input.ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

    }
}
