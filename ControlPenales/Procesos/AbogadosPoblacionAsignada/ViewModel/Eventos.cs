using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class AbogadosPoblacionAsignadaViewModel
    {
        #region Load
        public ICommand OnLoading
        {
            get { return new DelegateCommand<AbogadosPoblacionAsignadaView>(OnLoad); }
        }
        #endregion
    }
}
