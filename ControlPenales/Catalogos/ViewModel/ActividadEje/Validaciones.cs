using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoActividadEjeViewModel
    {
        private void setValidaciones()
        {
            base.ClearRules();
            AddRule(() => SelectedEje, () => SelectedEje!=-1,"SELECCIONE UN EJE!");
            AddRule(() => SelectedActividad, () => SelectedActividad != null && SelectedActividad.ID_ACTIVIDAD != -1, "SELECCIONE UNA ACTIVIDAD!");
            AddRule(() => SelectedEstatus, () => SelectedEstatus != null && SelectedEstatus.CLAVE!="-1" , "SELECCIONE UN ESTATUS!");
            RaisePropertyChanged("SelectedEje");
            RaisePropertyChanged("SelectedActividad");
            RaisePropertyChanged("SelectedEstatus");
        }
    }
}
