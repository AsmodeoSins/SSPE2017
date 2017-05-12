using System.Windows.Controls;
using System.Windows.Input;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class CancelacionVisitasViewModel
    {
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<PadronVisitasView>(Load_Window); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand _dgAcompananteCommand;
        public ICommand dgAcompananteCommand
        {
            get { return _dgAcompananteCommand ?? (_dgAcompananteCommand = new RelayCommand(SeleccionarAcompaniante)); }
        }

        private ICommand _dgImputadoCommand;
        public ICommand dgImputadoCommand
        {
            get { return _dgImputadoCommand ?? (_dgImputadoCommand = new RelayCommand(SeleccionarImputado)); }
        }

        private ICommand enterClick;
        public ICommand EnterClick
        {
            get { return enterClick ?? (enterClick = new RelayCommand(ClickEnter)); }
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
                            {
                                ListVisitantes.InsertRange(SegmentarBusqueda(Pagina));
                            }
                        }
                }));
            }
        }

    }
}
