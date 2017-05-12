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
    public class CapturaVisitaViewModel : ValidationViewModelBase,IPageViewModel
    {

      
        public string Name
        {
            get
            {
                return "programacion_visita_edificio";
            }
        }

        public CapturaVisitaViewModel() { }

        void IPageViewModel.inicializa()
        { }
 
    }
}
