using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class RegistroVisitaExternaViewModel
    {
        #region Load
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<PadronVisitaExternaView>(Load_Window); }
        }
        #endregion

        #region Click
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand enterClickNuevo;
        public ICommand EnterClickNuevo
        {
            get { return enterClickNuevo ?? (enterClickNuevo = new RelayCommand(ClickEnterNuevo)); }
        }

        private ICommand enterClick;
        public ICommand EnterClick
        {
            get { return enterClick ?? (enterClick = new RelayCommand(ClickEnter)); }
        }
        #endregion

        #region Buscar
        public ICommand CaptureImage
        {
            get { return new DelegateCommand<Image>(CapturarFoto); }
        }

        public ICommand FrenteDetrasCommand
        {
            get { return new DelegateCommand<TomarFotoSenaParticularView>(FrenteDetrasImages); }
        }
        #endregion

        #region Buscar
        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }
        #endregion

        #region Scroll
        private ICommand _CargarMasPersonas;
        public ICommand CargarMasPersonas
        {
            get
            {
                return _CargarMasPersonas ?? (_CargarMasPersonas = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            if (SeguirCargandoPersonas)
                            {
                                ListPersonas.InsertRange(await SegmentarPersonasBusqueda(Pagina));
                            }
                        }
                }));
            }
        }
        #endregion
    }
}
