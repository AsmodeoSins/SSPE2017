namespace ControlPenales
{
    partial class CatalogoEspecialistasViewModel
    {
        #region Click

        private System.Windows.Input.ICommand _onClick;
        public System.Windows.Input.ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch)); }
        }
        #endregion

        private System.Windows.Input.ICommand enterClick;
        public System.Windows.Input.ICommand EnterClick
        {
            get { return enterClick ?? (enterClick = new RelayCommand(ClickEnter)); }
        }

        #region Load
        public System.Windows.Input.ICommand CatalogoEspecialistasLoading
        {
            get { return new DelegateCommand<CatalogoEspecialistasView>(PageLoad); }
        }
        #endregion

        private System.Windows.Input.ICommand especialistaClick;
        public System.Windows.Input.ICommand EspecialistaClick
        {
            get { return especialistaClick ?? (especialistaClick = new RelayCommand(EspecialistaEnter)); }
        }


        private System.Windows.Input.ICommand _CargarMasPersonas;
        public System.Windows.Input.ICommand CargarMasPersonas
        {
            get
            {
                return _CargarMasPersonas ?? (_CargarMasPersonas = new RelayCommand(async (e) =>
                {
                    if (((System.Windows.Controls.ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((System.Windows.Controls.ScrollChangedEventArgs)e).ExtentHeight - ((System.Windows.Controls.ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((System.Windows.Controls.ScrollChangedEventArgs)e).VerticalOffset == (((System.Windows.Controls.ScrollChangedEventArgs)e).ExtentHeight - ((System.Windows.Controls.ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            if (SeguirCargandoPersonas)
                            {
                                ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                                ListPersonas.InsertRange(await SegmentarPersonasBusqueda(Pagina));
                                ListPersonasAuxiliar.InsertRange(ListPersonas);
                            };
                        };
                }));
            }
        }
    }
}