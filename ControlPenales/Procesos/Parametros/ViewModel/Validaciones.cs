using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class AdministracionParametrosViewModel
    {
       public void SetValidaciones()
       {
           base.ClearRules();
          
           base.AddRule(() => Clave, () => !string.IsNullOrEmpty(Clave), "CLAVE ES REQUERIDO!");
          
           base.AddRule(() => DESCR, () => !string.IsNullOrEmpty(DESCR), "DESCRIPCIÓN ES REQUERIDO!");
           base.AddRule(() => SelectedCentro, () => SelectedCentro.ID_CENTRO >-1, "CENTRO ES REQUERIDO!");
           OnPropertyChanged("SelectedCentro");

           
           
       }
    }
}
