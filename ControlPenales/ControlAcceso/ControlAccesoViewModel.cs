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
    public class ControlAccesoViewModel : ValidationViewModelBase,IPageViewModel
    {
        public string Name
        {
            get
            {
                return "control_acceso";
            }
        }

        public ControlAccesoViewModel()
        { 
            
        }
        void IPageViewModel.inicializa()
        { }
        
    }
}
