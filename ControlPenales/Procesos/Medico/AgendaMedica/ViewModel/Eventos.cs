using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class AgendaMedicaViewModel
    {
        public ICommand CmdAgendaMedicaOnLoad
        {
            get { return new DelegateCommand<AgendaMedicaView>(AgendaMedicaOnLoad); }
        }
        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get { return onClick ?? (onClick = new RelayCommand(OnClickSwitch)); }
        }

        private ICommand onAppointmentClick;
        public ICommand OnAppointmentClick
        {
            get { return onAppointmentClick ?? (onAppointmentClick = new RelayCommand(AppointmentClick)); }
        }

        private ICommand _ClickAgenda;
        public ICommand ClickAgenda
        {
            get
            {
                return _ClickAgenda ?? (_ClickAgenda = new RelayCommand(AgendaClick));
            }
        }


        #region Buscar Imputado
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

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get { return buscarClick ?? (buscarClick = new RelayCommand(ClickBuscarInterno)); }
        }
        #endregion
    }
}
