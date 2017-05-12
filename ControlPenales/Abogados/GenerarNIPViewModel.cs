using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    class GenerarNIPViewModel : ValidationViewModelBase, IPageViewModel
    {
        public GenerarNIPViewModel() { }
        public string Name
        {
            get
            {
                return "generacion_nip";
            }
        }
        void IPageViewModel.inicializa()
        { }
    }
}
