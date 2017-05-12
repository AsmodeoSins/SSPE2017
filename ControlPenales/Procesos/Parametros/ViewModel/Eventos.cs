using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
  partial  class AdministracionParametrosViewModel
    {
      public ICommand AdminParametrosLoading
        {
            get { return new DelegateCommand<AdministracionParametrosView>(OnLoad); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }


        private ICommand _BuscarClick;
      public ICommand BuscarClick
        {
            get
            {
                return _BuscarClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
      

    }
}
