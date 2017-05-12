using ControlPenales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CertificadoMedicoCancelacion_TrasladoViewModel
    {
        public ICommand CrtificadomedicoCancelacion_TrasladoLoading
        {
            get { return new DelegateCommand<CrtificadomedicoCancelacion_TrasladoView>(CertificadoMedicoCancelacion_TrasladoLoad); }
        }
        
             private ICommand _onClick;
             public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

             private ICommand buscarClick;
             public ICommand BuscarClick
             {
                 get
                 {
                     return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
                 }
             }


            

    }
}
