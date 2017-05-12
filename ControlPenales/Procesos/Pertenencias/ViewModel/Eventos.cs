using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class RegistroPertenenciasViewModel
    {
        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        #region Click
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand _ModelClick;
        public ICommand ModelClick
        {
            get { return _ModelClick ?? (_ModelClick = new RelayCommand(EnterExpediente)); }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(EnterBuscar));
            }
        }

        private ICommand _MouseDoubleClickCommand;
        public ICommand MouseDoubleClickCommand
        {
            get
            {
                return _MouseDoubleClickCommand ?? (_MouseDoubleClickCommand = new RelayCommand(FotoObjeto));
            }
        }
        #endregion

        #region Load
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<RegistroPertenenciasView>(Load_Window); }
        }
        #endregion

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

        #region Imagen
        public ICommand CaptureImage
        {
            get { return new DelegateCommand<Image>(CapturarFoto); }
        }
        #endregion

    }
}
