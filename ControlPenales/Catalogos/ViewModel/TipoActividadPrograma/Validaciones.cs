using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
   partial class CatalogoTipoActividadProgramaViewModel
    {

       private void setValidationRules()
       {
           base.ClearRules();
           base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
           base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
           base.AddRule(() => Nombre, () => !string.IsNullOrEmpty(Nombre), "NOMBRE ES REQUERIDO!");
           base.AddRule(() => SelectedDepartamento, () => SelectedDepartamento != null ? SelectedDepartamento.ID_DEPARTAMENTO != -1 : false, "DEPARTAMENTO ES REQUERIDO!");
           base.AddRule(() => Orden, () => Orden!= null, "ORDEN ES REQUERIDO!");
           OnPropertyChanged("Descripcion");
           OnPropertyChanged("SelectedEstatus");
           OnPropertyChanged("Nombre");
           OnPropertyChanged("SelectedDepartamento");
           OnPropertyChanged("Orden");
           // base.AddRule(() => Descripcion, () => SELECTED, "DESCRIPCIÓN ES REQUERIDA!");
       }
    }
}
