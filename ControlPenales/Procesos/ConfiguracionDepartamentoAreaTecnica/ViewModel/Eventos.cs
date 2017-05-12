using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ConfiguracionDepartamentoAreaTecnicaViewModel
    {
        #region General
        public ICommand ConfiguracionDepartamentoAreaTecnicaOnLoading
        {
            get { return new DelegateCommand<ConfiguracionDepartamentoAreaTecnicaView>(ConfiguracionDepartamentoAreaTecnicaLoad); }
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(ClickSwitch));
            }
        }
        #endregion
    }
}
