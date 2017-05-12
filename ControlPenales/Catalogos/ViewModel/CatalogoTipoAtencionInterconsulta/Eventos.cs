using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoTipoAtencionInterconsultaViewModel
    {
        #region Click

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch)); }
        }
        #endregion

        #region Load
        public ICommand CatalogoTipoAtencionInterconsultaLoading
        {
            get { return new DelegateCommand<CatalogoTipoAtencionInterconsultaView>(PageLoad); }
        }
        #endregion

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(CambioModelo)); }
        }
    }
}
