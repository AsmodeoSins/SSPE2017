using System.Windows.Controls;
namespace ControlPenales
{
    public partial class HojaControlLiquidosViewModel
    {
        #region Busqueda
        private System.Windows.Input.ICommand modelClick;
        public System.Windows.Input.ICommand ModelClick
        {
            get
            {
                return modelClick ?? (modelClick = new RelayCommand(ModelEnter));
            }
        }

        private System.Windows.Input.ICommand _onClick;
        public System.Windows.Input.ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

        private System.Windows.Input.ICommand buscarClick;
        public System.Windows.Input.ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }

        //HojaControlLiquidosWindow
        #region Load
        public System.Windows.Input.ICommand HojaLiquidosLoading
        {
            get { return new DelegateCommand<HojaControlLiquidosView>(PageLoad); }
        }
        #endregion


        private System.Windows.Input.ICommand cmdModelChanged;
        public System.Windows.Input.ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        private System.Windows.Input.ICommand _CargarMasResultados;
        public System.Windows.Input.ICommand CargarMasResultados
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
    }
}