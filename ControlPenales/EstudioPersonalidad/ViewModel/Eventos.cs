using System.Windows.Input;

namespace ControlPenales
{
    partial class EstudioPersonalidadViewModel
    {
        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        #region Load
        public ICommand EstudioPersonalidadLoad
        {
            get { return new DelegateCommand<EstudioPersonalidadView>(LoadEstudioPersonalidad); }
        }
        #endregion
        #region Busqueda
        
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
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
                return modelClick ?? (modelClick = new RelayCommand(ModelEnter));
            }
        }

        public ICommand CierreEstudiosPersonalidadLoading
        {
            get { return new DelegateCommand<CierreEstudiosPersonalidadView>(InicializaCierreEstudiosPersonalidad); }
        }

        public ICommand EstudiosPersonalidadLoading
        {
            get { return new DelegateCommand<EstudioPersonalidadView>(InicializaListaEstudiosPersonalidad); }
        }

        #endregion
    }
}