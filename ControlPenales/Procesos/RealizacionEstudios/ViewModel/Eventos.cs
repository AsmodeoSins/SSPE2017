using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class RealizacionEstudiosViewModel
    {
        #region commands
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
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

        #region Scroll
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

        #endregion

        private ICommand modelClick;
        public ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(ModelEnter));
            }
        }

        //public ICommand OnLoaded
        //{
        //    get { return new DelegateCommand<PrivilegiosView>(OnLoad); }
        //}

        //RealizacionEstudiosPersonalidadWindow

        public ICommand RealizacionEstudiosPUnLoaded
        {
            get { return new DelegateCommand<RealizacionEstudiosPersonalidadView>(RealizacionEstudiosUnLoad); }
        }

        public ICommand RealizacionEstudiosLoading
        {
            get { return new DelegateCommand<RealizacionEstudiosPersonalidadView>(RealizacionEstudiosPLoad); }
        }

        public ICommand MedicoUnLoad
        {
            get { return new DelegateCommand<EstudioMedicoFCView>(DescargaMedico); }
        }

        public ICommand PsicologicoUnLoad
        {
            get { return new DelegateCommand<EstudioPsicologicoFCView>(DescargaPsicologico); }
        }

        public ICommand PsiquiatricoUnLoad
        {
            get { return new DelegateCommand<EstudioPsiquiatricoFCView>(DescargaPsiquiatrico); }
        }

        public ICommand CriminoUnLoad
        {
            get { return new DelegateCommand<EstudioCriminodiagnosticoFCView>(DescargaCriminologico); }
        }

        public ICommand SocioFamUnLoad
        {
            get { return new DelegateCommand<EstudioSocioFamiliarFCView>(DescargaSocioFamiliar); }
        }

        public ICommand EducativoUnLoad
        {
            get { return new DelegateCommand<EstudioEducativoCulturalDepFCView>(DescargaEducativo); }
        }

        public ICommand CapacitacionUnLoad
        {
            get { return new DelegateCommand<EstudioCapacitacionYTrabajoPenitFCView>(DescargaCapacitacion); }
        }

        public ICommand SeguridadUnLoad
        {
            get { return new DelegateCommand<InformeSeguridadCustodiaFCView>(DescargaSeguridad); }
        }

        //SeguridadUnLoad

        #region Huella
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


        #endregion



        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

    }
}