using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ReAgendaTrasladoVirtualViewModel
    {
        public ICommand CmdReAgendaTrasladoVirtualOnLoading
        {
            get { return new DelegateCommand<ReAgendaTrasladoVirtualView>(ReAgendaTrasladoVirtualOnLoading); }
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get { return onClick ?? (onClick = new RelayCommand(ClickSwitch)); }
        }

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }
    }
}
