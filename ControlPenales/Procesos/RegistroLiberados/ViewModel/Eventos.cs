using ControlPenales.Clases;
using System.Windows.Controls;
using System.Windows.Input;


namespace ControlPenales
{
    partial class RegistroLiberadosViewModel
    {
       // private ICommand _OnUserControlClose;
       // public ICommand OnUserControlClose
       // {
       //     get
       //     {
       //         return _OnUserControlClose ?? (_OnUserControlClose = new RelayCommand(CloseView));
       //     }
       // }
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
      
       // private ICommand _validarClick;
       // public ICommand ValidarClick
       // {
       //     get
       //     {
       //         return _validarClick ?? (_validarClick = new RelayCommand(clickSwitch));
       //     }
       // }
       // private ICommand _treeViewClick;
       // public ICommand TreeViewClick
       // {
       //     get
       //     {
       //         return _treeViewClick ?? (_treeViewClick = new RelayCommand(TreeClick));
       //     }
       // }
        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }

        private ICommand buscarCPClick;
        public ICommand BuscarCPClick
        {
            get
            {
                return buscarCPClick ?? (buscarCPClick = new RelayCommand(ClickEnterCP));
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
       // private ICommand buscarRelacionInternoClick;
       // public ICommand BuscarRelacionInternoClick
       // {
       //     get
       //     {
       //         return buscarRelacionInternoClick ?? (buscarRelacionInternoClick = new RelayCommand(BuscarRelacionInterno));
       //     }
       // }
       // private ICommand _onClickAlias;
       // public ICommand OnClickAlias
       // {
       //     get
       //     {
       //         return _onClickAlias ?? (_onClickAlias = new RelayCommand(GuardarAlias));
       //     }
       // }
       // private ICommand _onClickApodo;
       // public ICommand OnClickApodo
       // {
       //     get
       //     {
       //         return _onClickApodo ?? (_onClickApodo = new RelayCommand(GuardarApodo));
       //     }
       // }
       // private ICommand _onClickRelacionInterno;
       // public ICommand OnClickRelacionInterno
       // {
       //     get
       //     {
       //         return _onClickRelacionInterno ?? (_onClickRelacionInterno = new RelayCommand(GuardarRelacionInterno));
       //     }
       // }

       // #region [TREEVIEW]
       // private ICommand _Checked;
       // public ICommand Checked
       // {
       //     get
       //     {
       //         return _Checked ?? (_Checked = new DelegateCommand<TreeViewList>((SelectedItem) => { SetIsSelectedProperty(SelectedItem, true); }));
       //     }
       // }

       // private ICommand _Unchecked;
       // public ICommand Unchecked
       // {
       //     get
       //     {
       //         return _Unchecked ?? (_Unchecked = new DelegateCommand<TreeViewList>((SelectedItem) => { SetIsSelectedProperty(SelectedItem, false); }));
       //     }
       // }
       // #endregion

        #region [WebCam]
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<FotosHuellasDigitalesEstatusAdminView>(OnLoad); }
        }

        //public ICommand WindowClosing
        //{
        //    get { return new DelegateCommand<RegistroIngresoView>(OnUnLoad); }
        //}
        public ICommand CaptureImage
        {
            get { return new DelegateCommand<Image>(OnTakePicture); }
        }
        public ICommand CamSettings
        {
            get { return new DelegateCommand<string>(OpenSetting); }
        }
        //private ICommand onClickNotaPandilla;
        //public ICommand OnClickNotaPandilla
        //{
        //    get
        //    {
        //        return onClickNotaPandilla ?? (onClickNotaPandilla = new RelayCommand(GuardarPandilla));
        //    }
        //}

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

        public ICommand BuscarHuellaEmpleado
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuellaEmpleado); }
        }
        #endregion

       //#region [LOADS]
        public ICommand LiberadoLoading
        {
            get { return new DelegateCommand<RegistroLiberadosView>(LiberadoLoad); }
        }

        public ICommand WindowMedidaLoading
        {
            get { return new DelegateCommand<BuscarPorHuellaYNipMedidaView>(OnLoadMedida); }
        }
       // public ICommand DatosIngresoInternoLoading
       // {
       //     get { return new DelegateCommand<DatosIngresoInternoView>(DatosIngresoInternoLoad); }
       // }
       // public ICommand TrasladoLoading
       // {
       //     get { return new DelegateCommand<IngresoTrasladoView>(TrasladoLoad); }
       // }
        public ICommand DatosGeneralesIdentificacionLoading
        {
            get { return new DelegateCommand<DatosGeneralesIdentificacionEstatusAdminView>(DatosIdentificacionLoad); }
        }
       // public ICommand ApodosAliasReferenciasLoading
       // {
       //     get { return new DelegateCommand<ApodosAliasReferenciasView>(ApodosAliasReferenciasLoad); }
       // }
       // #endregion

       // #region INTERCONEXION
       // public ICommand BuscarNUCInterconexion
       // {
       //     get { return new DelegateCommand<string>(OnBuscarNUCInterconexion); }
       // }
       // #endregion

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
                                LstLiberados.InsertRange(await SegmentarResultadoBusquedaLiberados(Pagina));
                                //ListExpediente.InsertRange(await SegmentarResultadoBusqueda(Pagina));
                        }
                }));
            }
        }

        #region INTERCONEXION
        public ICommand BuscarNUCInterconexion
        {
            get { return new DelegateCommand<string>(OnBuscarNUCInterconexion); }
        }
        #endregion
    }
}
