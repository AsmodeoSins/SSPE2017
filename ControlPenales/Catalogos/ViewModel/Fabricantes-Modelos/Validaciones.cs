using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoFabricantes_Modelos_ViewModel
    {
       private void setValidationRules()
       {
           base.ClearRules();
           base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
           base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
           base.AddRule(() => Tipo, () => Tipo != -1, "OBJETO ES REQUERIDO!");
           OnPropertyChanged("Descripcion");
           OnPropertyChanged("SelectedEstatus");
           OnPropertyChanged("Tipo");
       }
    }
}
