using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    class TurnosVisitaViewModel : ValidationViewModelBase, IPageViewModel
    {
        public TurnosVisitaViewModel() { }
        public string Name
        {
            get
            {
                return "turnos_visita";
            }
        }
        void IPageViewModel.inicializa()
        { }
    }
}
