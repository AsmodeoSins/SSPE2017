using System.Windows.Controls;
using System.Windows.Input;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class PadronVisitasViewModel
    {
        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        private ICommand tipoPaseOpened;
        public ICommand TipoPaseOpened
        {
            get 
            { 
                return tipoPaseOpened ?? (tipoPaseOpened = new RelayCommand(TipoPaseOpen)); 
            }
        }
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }
        private ICommand modelClick;
        public ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(EnterExpediente));
            }
        }
        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(EnterBuscar));
            }
        }
        private ICommand enterClick;
        public ICommand EnterClick
        {
            get { return enterClick ?? (enterClick = new RelayCommand(ClickEnter)); }
        }
        private ICommand acompananteClick;
        public ICommand AcompananteClick
        {
            get { return acompananteClick ?? (acompananteClick = new RelayCommand(AcompananteSwitch)); }
        }
        private ICommand _dgImputadoCommand;
        public ICommand dgImputadoCommand
        {
            get { return _dgImputadoCommand ?? (_dgImputadoCommand = new RelayCommand(SeleccionarImputado)); }
        }
        private ICommand _dgAcompananteCommand;
        public ICommand dgAcompananteCommand
        {
            get { return _dgAcompananteCommand ?? (_dgAcompananteCommand = new RelayCommand(SeleccionarAcompaniante)); }
        }

        public ICommand TomarFoto
        {
            get { return new DelegateCommand<object>(AbrirCamara); }
        }

        public ICommand DiaVisita
        {
            get { return new DelegateCommand<string>(SetDiaVisita); }
        }

        public ICommand CamSettings
        {
            get { return new DelegateCommand<string>(OpenSetting); }
        }

        public ICommand CaptureImage
        {
            get { return new DelegateCommand<Image>(CapturarFoto); }
        }

        public ICommand OnLoaded
        {
            get { return new DelegateCommand<PadronVisitasView>(Load_Window); }
        }
        /*
        public ICommand OnLoadedCapturaVisita
        {
            get { return new DelegateCommand<CapturaVisitaView>(LoadCapturaVisita); }
        }
        */
        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }
        public ICommand startScanning
        {
            get { return new DelegateCommand<PdfViewer>(Scan); }
        }
        public ICommand OpenDocument
        {
            get { return new DelegateCommand<PdfViewer>(AbrirDocumento); }
        }
        private ICommand _GafeteClick;

        //EVENTO SEGMENTACION
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
        public ICommand ProgramarVisitaLoaded
        {
            get { return new DelegateCommand<ProgramarVisitaView>(ProgramarVisitaLoad); }
        }

        //public ICommand BorrarVisita
        //{
        //    get { return new DelegateCommand<object>(BorrarProgramacionVisita); }
        //}
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BuscarPorHuellaYNipView>(OnLoad); }
        }
    }
}
