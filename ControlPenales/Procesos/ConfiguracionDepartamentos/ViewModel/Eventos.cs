using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ConfiguracionDepartamentosViewModel
    {
        public ICommand OnLoading
        {
            get { return new DelegateCommand<ConfiguracionDepartamentosView>(OnLoad); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
    }
}
