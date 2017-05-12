using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ControlParticipantesViewModel
    {
        /* [descripcion de clase]
         * clase donde se definen los eventos para el modulo control participante
         * 
         * comando para cargar ventana
         * comando para acciones click: OnClick
         * comando para busqueda segmentada: CargarMasResultados
         * comando de busqueda: BuscarClick
         * comando para la seccion agregar: AgregarCommand
         * 
         */

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        public ICommand ControlParticipantesLoading
        {
            get { return new DelegateCommand<ControlParticipantesView>(ControlParticipantesLoad); }
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
        private ICommand _InternoClick;
        public ICommand InternoClick
        {
            get { return _InternoClick ?? (_InternoClick = new RelayCommand(BuscarInterno)); }
        }
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }
        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(BuscarInternoPopup));
            }
        }
        private ICommand _AgregarCommand;
        public ICommand AgregarCommand
        {
            get
            {
                return _AgregarCommand ?? (_AgregarCommand = new RelayCommand(OnAgregarCommand));
            }
        }
    }
}
