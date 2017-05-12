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
    public class ConsultaViewModel : ValidationViewModelBase,IPageViewModel
    {
        public string Name
        {
            get
            {
                return "consulta";
            }
        }

        public ConsultaViewModel()
        { 
            
        }

        private int test() {
            return 0;
        }
        void IPageViewModel.inicializa()
        { }
        
    }
}
