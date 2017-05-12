using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoMedicamento_CategoriasViewModel
    {
        private void setValidacionRule()
        {
            ClearRules();
            AddRule(()=>TextMedicamento_Categoria,()=>!string.IsNullOrWhiteSpace(TextMedicamento_Categoria),"CATEGORIA ES REQUERIDO!");
            AddRule(()=>TextDescripcion,()=>!string.IsNullOrWhiteSpace(TextDescripcion),"DESCRIPCION ES REQUERIDO!");
            RaisePropertyChanged("TextMedicamento_Categoria");
            RaisePropertyChanged("TextDescripcion");
        }
    }
}
