using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFPdfViewer;
namespace ControlPenales
{
    partial class HistoriaClinicaViewModel
    {
        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        public ICommand OnLoaded
        {
            get { return new DelegateCommand<HistoriaClinicaView>(Load_Window); }
        }
        private ICommand modelClick;
        public ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(BuscarExpediente));
            }
        }
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }
        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(EnterBuscar));
            }
        }
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


        private ICommand mouseDoubleClickArbolIngresoCommand;
        public ICommand MouseDoubleClickArbolIngresoCommand
        {
            get
            {
                return mouseDoubleClickArbolIngresoCommand ?? (mouseDoubleClickArbolIngresoCommand = new RelayCommand(SeleccionaIngresoArbol));
            }
        }


        #region HUELLA
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BuscarPorHuellaYNipView>(OnLoad); }
        }
        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }

        public ICommand CommandAceptar
        {
            get { return new DelegateCommand<Window>(Aceptar); }
        }

        public ICommand CommandOpem442
        {
            get { return new DelegateCommand<string>(Capture); }
        }

        public ICommand CommandContinue
        {
            get { return new DelegateCommand<string>((s) => { isKeepSearching = s == "True" ? true : false; }); }
        }
        #endregion

        #region Digitalizacion
        //public ICommand startScanning
        //{
        //    get { return new DelegateCommand<PdfViewer>(Scan); }
        //}
        public ICommand OpenDocument
        {
            get { return new DelegateCommand<PdfViewer>(AbrirDocumento); }
        }
        #endregion
        #region Digitalizacion Aislada
        public ICommand startScanAislado
        {
            get { return new DelegateCommand<PdfViewer>(ScanAislado); }
        }
        #endregion

        #region DoubleClick
        private ICommand doubleClickGridCommand;
        public ICommand DoubleClickGridCommand
        {
            get
            {
                return doubleClickGridCommand ?? (doubleClickGridCommand = new RelayCommand(MarcaOdontogramaPosicion));
            }
        }
        #endregion


        #region DIENTES
        private ICommand checkClick;
        public ICommand CheckClick
        {
            get
            {
                return checkClick ?? (checkClick = new RelayCommand(OdontogramaClick));
            }
        }


        private ICommand checkClickSeguimiento;
        public ICommand CheckClickSeguimiento
        {
            get
            {
                return checkClickSeguimiento ?? (checkClickSeguimiento = new RelayCommand(OdontogramaClickSeg));
            }
        }

        public ICommand OdontogramaInicialDentalLoaded
        {
            get { return new DelegateCommand<OdontogramaInicialHistoriaClinicaDentalView>(InicializaOdontogramaInicialDental); }
        }

        public ICommand OdontogramaSeguimientoDentalLoaded
        {
            get { return new DelegateCommand<OdontogramaSeguimientoDental>(InicilizaOdontogramaSeguimientoDental); }
        }
        #endregion
    }
}
