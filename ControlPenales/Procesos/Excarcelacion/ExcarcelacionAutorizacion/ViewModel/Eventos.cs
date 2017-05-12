using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ExcarcelacionAutorizacionViewModel
    {
        #region General
        public ICommand CmdExcarcelacionAutorizacionOnLoad
        {
            get { return new DelegateCommand<ExcarcelacionAutorizacionView>(ExcarcelacionAutorizacionOnLoad); }
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get { return onClick ?? (onClick = new RelayCommand(ClickSwitch)); }
        }

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged??(cmdModelChanged=new RelayCommand(ModelChangedSwitch));}
        }

        //private ICommand cmdModelChanged;
        //public ICommand CmdModelChanged
        //{
        //    get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(ModelChangedSwitch)); }
        //}
        #endregion
    }
}
