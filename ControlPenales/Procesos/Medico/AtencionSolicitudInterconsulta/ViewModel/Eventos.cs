using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class AtencionSolicitudInterconsultaViewModel
    {
        public ICommand CmdSolicitudInterconsultaOnLoading
        {
            get { return new DelegateCommand<SolicitudInterconsultaView>(SolicitudInterconsultaOnLoading); }
        }

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }


        private ICommand onClick;
        public ICommand OnClick
        {
            get { return onClick ?? (onClick = new RelayCommand(ClickSwitch)); }
        }

        public ICommand SI_SeniasFrenteLoading
        {
            get { return new DelegateCommand<SI_SeniasFrenteView>(SeniasFrenteLoad); }
        }

        public ICommand SI_SeniasDorsoLoading
        {
            get { return new DelegateCommand<SI_SeniasDorsoView>(SeniasDorsoLoad); }
        }
    }
}
