using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoMedicamento_SubcategoriasViewModel
    {
        private void setValidacionRule()
        {
            ClearRules();
            AddRule(() => TextDescripcion, () => !string.IsNullOrWhiteSpace(TextDescripcion), "DESCRIPCION ES REQUERIDO!");
            AddRule(() => SelectCategoria, () => SelectCategoria != null ? SelectCategoria > 0 : false, "CATEGORIA ES REQUERIDA!");
            RaisePropertyChanged("TextDescripcion");
            RaisePropertyChanged("SelectCategoria");
        }
    }
}
