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
    public class PadronVisitaExternaViewModel : ValidationViewModelBase,IPageViewModel
    {
        public string Name
        {
            get
            {
                return "padron_visita_externa";
            }
        }

        public PadronVisitaExternaViewModel()
        { 
            
        }
        void IPageViewModel.inicializa()
        { }
    }
}
