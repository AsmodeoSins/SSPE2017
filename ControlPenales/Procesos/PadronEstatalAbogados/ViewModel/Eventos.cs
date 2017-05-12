using ControlPenales;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class PadronEstatalAbogadosViewModel
    {
        #region Load
        public ICommand OnLoading
        {
            get { return new DelegateCommand<PadronEstatalAbogadosView>(OnLoad); }
        }
        #endregion
    }
}
