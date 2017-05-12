using ControlPenales.Clases;
using System.Windows.Controls;
using System.Windows.Input;


namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        private ICommand _OnUserControlClose;
        public ICommand OnUserControlClose
        {
            get
            {
                return _OnUserControlClose ?? (_OnUserControlClose = new RelayCommand(CloseView));
            }
        }
        
        #region Click
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        
        private ICommand _validarClick;
        public ICommand ValidarClick
        {
            get
            {
                return _validarClick ?? (_validarClick = new RelayCommand(clickSwitch));
            }
        }
        
        private ICommand _treeViewClick;
        public ICommand TreeViewClick
        {
            get
            {
                return _treeViewClick ?? (_treeViewClick = new RelayCommand(TreeClick));
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

        private ICommand modelClick;
        public ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(ClickEnter));
            }
        }
        
        private ICommand buscarRelacionInternoClick;
        public ICommand BuscarRelacionInternoClick
        {
            get
            {
                return buscarRelacionInternoClick ?? (buscarRelacionInternoClick = new RelayCommand(BuscarRelacionInterno));
            }
        }
        
        private ICommand _onClickAlias;
        public ICommand OnClickAlias
        {
            get
            {
                return _onClickAlias ?? (_onClickAlias = new RelayCommand(GuardarAlias));
            }
        }
        
        private ICommand _onClickApodo;
        public ICommand OnClickApodo
        {
            get
            {
                return _onClickApodo ?? (_onClickApodo = new RelayCommand(GuardarApodo));
            }
        }
        
        private ICommand _onClickRelacionInterno;
        public ICommand OnClickRelacionInterno
        {
            get
            {
                return _onClickRelacionInterno ?? (_onClickRelacionInterno = new RelayCommand(GuardarRelacionInterno));
            }
        }
        #endregion

        #region [TREEVIEW]
        private ICommand _Checked;
        public ICommand Checked
        {
            get
            {
                return _Checked ?? (_Checked = new DelegateCommand<TreeViewList>((SelectedItem) => { SetIsSelectedProperty(SelectedItem, true); }));
            }
        }

        private ICommand _Unchecked;
        public ICommand Unchecked
        {
            get
            {
                return _Unchecked ?? (_Unchecked = new DelegateCommand<TreeViewList>((SelectedItem) => { SetIsSelectedProperty(SelectedItem, false); }));
            }
        }
        #endregion

        #region [WebCam]
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<FotosHuellasDigitalesView>(OnLoad); }
        }
        public ICommand WindowClosing
        {
            get { return new DelegateCommand<RegistroIngresoView>(OnUnLoad); }
        }
        public ICommand CaptureImage
        {
            get { return new DelegateCommand<Image>(OnTakePicture); }
        }
        //public ICommand SaveImages
        //{
        //    get { return new DelegateCommand<string>(SaveImagesTo); }
        //}
        public ICommand CamSettings
        {
            get { return new DelegateCommand<string>(OpenSetting); }
        }
        private ICommand onClickNotaPandilla;
        public ICommand OnClickNotaPandilla
        {
            get
            {
                return onClickNotaPandilla ?? (onClickNotaPandilla = new RelayCommand(GuardarPandilla));
            }
        }

        #endregion

        #region [Huellas Digitales]
        private ICommand _Open442;
        public ICommand Open442
        {
            get { return _Open442 ?? (_Open442 = new RelayCommand(ShowIdentification)); }
        }

        public ICommand OnClickOk
        {
            get { return new DelegateCommand<Image>(OkClick); }
        }

        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }
        #endregion

       #region [LOADS]
        public ICommand IngresoLoading
        {
            get { return new DelegateCommand<RegistroIngresoView>(IngresoLoad); }
        }
        public ICommand DatosIngresoInternoLoading
        {
            get { return new DelegateCommand<DatosIngresoInternoView>(DatosIngresoInternoLoad); }
        }
        public ICommand TrasladoLoading
        {
            get { return new DelegateCommand<IngresoTrasladoView>(TrasladoLoad); }
        }
        public ICommand DatosGeneralesIdentificacionLoading
        {
            get { return new DelegateCommand<DatosGeneralesIdentificacionView>(DatosIdentificacionLoad); }
        }
        public ICommand ApodosAliasReferenciasLoading
        {
            get { return new DelegateCommand<ApodosAliasReferenciasView>(ApodosAliasReferenciasLoad); }
        }
        #endregion

        #region INTERCONEXION
        public ICommand BuscarNUCInterconexion
        {
            get { return new DelegateCommand<string>(OnBuscarNUCInterconexion); }
        }
        #endregion

        #region TRASLADOS
        private ICommand cmdComboBoxCentroOrigenChanged;
        public ICommand CmdComboBoxCentroOrigenChanged
        {
            get { return cmdComboBoxCentroOrigenChanged ?? (cmdComboBoxCentroOrigenChanged = new RelayCommand(ModeloUpdatedChanged)); }
        }
        #endregion

        #region Grid
        private ICommand _CargarMasResultados;
        public ICommand CargarMasResultados
        {
            get
            {
                return _CargarMasResultados ?? (_CargarMasResultados = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            if (SeguirCargando)
                                ListExpediente.InsertRange(await SegmentarResultadoBusqueda(Pagina));
                        }
                }));
            }
        }

        private ICommand _CargarMasRelacionImputado;
        public ICommand CargarMasRelacionImputado
        {
            get
            {
                return _CargarMasRelacionImputado ?? (_CargarMasRelacionImputado = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            if (RISeguirCargando)
                            {
                                ListBuscarRelacionInterno.InsertRange(await SegmentarResultadoBusquedaRelacionInterno(RIPagina));
                                EmptyBuscarRelacionInternoVisible = ListBuscarRelacionInterno.Count > 0 ? false : true;
                            }
                        }
                }));
            }
        }
        #endregion
    }
}
