using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoCampañiasViewModel
    {
        void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => Estatus, () => !string.IsNullOrEmpty(Estatus), "ESTATUS ES REQUERIDA!");
        }
    }
}
