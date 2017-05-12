using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteIncidenteViewModel
    {
        private ICommand _onClick;
        public ICommand OnClick 
        {
            get { return _onClick ?? (_onClick = new RelayCommand(ClickSwitch));}
        }

        public ICommand ReporteLoading
        {
            get { return new DelegateCommand<ReporteIncidenteView>(OnLoad); }
        }
    }
}
