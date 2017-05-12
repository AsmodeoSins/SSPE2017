using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoEjeViewModel
    {
        void setValidationRules() 
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => Orden, () => Orden != null, "ORDEN ES REQUERIDO!");
            base.AddRule(() => Complementario, () => !string.IsNullOrEmpty(Complementario), "COMPLEMENTARIO ES REQUERIDA!");
        }
    }
}
