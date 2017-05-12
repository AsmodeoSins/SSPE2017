using System.Windows.Controls;
using System.Windows.Input;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class RecepcionAduanaViewModel
    {
        #region Click
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }
        private ICommand enterClick;
        public ICommand EnterClick
        {
            get { return enterClick ?? (enterClick = new RelayCommand(ClickEnter)); }
        }
        private ICommand aduanaClick;
        public ICommand AduanaClick
        {
            get { return aduanaClick ?? (aduanaClick = new RelayCommand(AduanaEnter)); }
        }
        private ICommand _HeaderClick;
        public ICommand HeaderClick
        {
            get
            {
                return _HeaderClick ?? (_HeaderClick = new RelayCommand(HeaderSort));
            }
        }
        private ICommand _dgImputadoCommand;
        public ICommand dgImputadoCommand
        {
            get { return _dgImputadoCommand ?? (_dgImputadoCommand = new RelayCommand(SeleccionarVisitante)); }
        }
        #endregion

        #region Load
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<RecepcionAduanaView>(Load_Window); }
        }
        
        #endregion

        #region Checked
        private ICommand _Checked;
        public ICommand Checked
        {
            get
            {
                return _Checked ?? (_Checked = new DelegateCommand<object>((SelectedItem) => { CheckBoxSelectedOnGrid(SelectedItem); }));
            }
        }
        #endregion

        #region Grid
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
                                ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                                ListPersonas.InsertRange(await SegmentarPersonasBusqueda(Pagina));
                                ListPersonasAuxiliar.InsertRange(ListPersonas);
                            }
                        }
                }));
            }
        }
        #endregion

        #region Huellas
        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }
        #endregion

        //public ICommand CaptureImage
        //{
        //    get { return new DelegateCommand<Image>(OnTakePicture); }
        //}
    }
}
