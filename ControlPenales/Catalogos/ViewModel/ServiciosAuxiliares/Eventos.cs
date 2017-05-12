using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoServiciosAuxiliaresViewModel
    {
        #region Click

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch)); }
        }
        #endregion

        #region Load
        public ICommand CatalogoServiciosAuxiliaresLoading
        {
            get { return new DelegateCommand<CatalogoServiciosAuxiliaresView>(PageLoad); }
        }
        #endregion

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged??(cmdModelChanged=new RelayCommand(CambioModelo));}
        }
    }
}
