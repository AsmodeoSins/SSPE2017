using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ReporteTrasladosEstatalesViewModel
    {

        public ICommand OnLoading
        {
            get { return new DelegateCommand<ReporteTrasladosEstatalesView>(OnLoaded); }
        }
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(GeneraReporte));
            }
        }
    }
}
