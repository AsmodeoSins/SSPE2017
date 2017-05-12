namespace ControlPenales
{
    public partial class HojaEnfermeriaViewModel
    {
        #region Pagina Cargada
        public System.Windows.Input.ICommand HojaEnfermeriaLoading
        {
            get { return new DelegateCommand<HojaEnfermeriaView>(LoadHojaEnfermeria); }
        }

        public System.Windows.Input.ICommand CapturaSolucionesHELoading
        {
            get { return new DelegateCommand<SeccionSolucionesHEView>(LoadSolucionesWindow); }
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

        #region DoubleClick
        //private System.Windows.Input.ICommand clickGridCommand;
        //public System.Windows.Input.ICommand ClickGridCommand
        //{
        //    get
        //    {
        //        return clickGridCommand ?? (clickGridCommand = new RelayCommand(SeleccionaCateter));
        //    }
        //}

        #endregion

        private System.Windows.Input.ICommand buscarClick;
        public System.Windows.Input.ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }

        private System.Windows.Input.ICommand cmdModelChanged;
        public System.Windows.Input.ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        public System.Windows.Input.ICommand SeniasFrenteLoading
        {
            get { return new DelegateCommand<SeniasFrenteView>(SeniasFrenteLoad); }
        }
        public System.Windows.Input.ICommand SeniasDorsoLoading
        {
            get { return new DelegateCommand<SeniasDorsoView>(SeniasDorsoLoad); }
        }

        private System.Windows.Input.ICommand regionClick;
        public System.Windows.Input.ICommand RegionClick
        {
            get
            {
                return regionClick ?? (regionClick = new RelayCommand(RegionSwitch));
            }
        }

        private System.Windows.Input.ICommand _LesionClick;
        public System.Windows.Input.ICommand LesionClick
        {
            get
            {
                return _LesionClick ?? (_LesionClick = new RelayCommand(LesionSelected));
            }
        }

    }
}