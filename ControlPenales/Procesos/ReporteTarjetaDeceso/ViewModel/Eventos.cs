using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ReporteTarjetaInformativaDecesoViewModel
    {
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
    }
}
