using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    class RevisarCambiosUbicacionViewModel : ValidationViewModelBase, IPageViewModel
    {
        public RevisarCambiosUbicacionViewModel() { }
        public string Name
        {
            get
            {
                return "revisar_cambios_ubicacion";
            }
        }
        void IPageViewModel.inicializa()
        { }
    }
}
