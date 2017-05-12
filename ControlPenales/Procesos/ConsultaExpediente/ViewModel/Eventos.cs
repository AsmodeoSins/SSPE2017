using System.Windows.Input;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        private ICommand _dgImputadoCommand;
        public ICommand dgImputadoCommand
        {
            get { return _dgImputadoCommand ?? (_dgImputadoCommand = new RelayCommand(SeleccionarExpediente)); }
        }
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<ConsultaExpedienteInternoView>(Load_Window); }
        }
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        //EVENTO PARA DOBLE CLIC PARA abrir emi
        private ICommand _doubleClickCommand;
        public ICommand DoubleClickCommand
        {
            get { return _doubleClickCommand ?? (_doubleClickCommand = new RelayCommand(clickSwitch)); }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(BuscarInternoPopup));
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
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<FotosHuellasDigitalesView>(OnLoad); }
        }
        //public ICommand LoadBuscarExpediente
        //{
        //    get { return new DelegateCommand<BuscarExpedienteView>(LoadConsultaExpediente); }
        //}
        public ICommand SeniasFrenteLoading
        {
            get { return new DelegateCommand<SeniasFrenteView>(SeniasFrenteLoad); }
        }
        public ICommand SeniasDorsoLoading
        {
            get { return new DelegateCommand<SeniasDorsoView>(SeniasDorsoLoad); }
        }

        private System.Windows.Input.ICommand cmdModelChanged;
        public System.Windows.Input.ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

    }
}
