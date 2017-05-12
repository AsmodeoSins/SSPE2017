using System.Windows.Controls;
using System.Windows.Input;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class BitacoraPasesPorVerificarViewModel
    {
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<BitacoraPasesPorVerificar>(Load_Window); }
        }
        //private ICommand _MouseDoubleClickCommand;
        //public ICommand MouseDoubleClickCommand
        //{
        //    get { return _MouseDoubleClickCommand ?? (_MouseDoubleClickCommand = new RelayCommand(VerificarPase)); }
        //}
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
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
                                ListPasesTotales.InsertRange(await SegmentarResultadoBusqueda(Pagina));
                                ListPases = ListPasesTotales;
                            }
                        }
                }));
            }
        }

        private ICommand filtrarEnter;
        public ICommand FiltrarEnter
        {
            get { return filtrarEnter ?? (filtrarEnter = new RelayCommand(Filtrar)); }
        }
    }
}
