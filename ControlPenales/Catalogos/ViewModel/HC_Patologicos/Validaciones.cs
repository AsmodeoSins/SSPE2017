using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class Catalogo_PatologicosViewModel
    {
        private void setValidacionRule()
        {
            ClearRules();
            AddRule(()=>TextPatologico,()=>!string.IsNullOrWhiteSpace(TextPatologico),"ANTECEDENTE PERSONAL PATOLOGICO ES REQUERIDO!");
            RaisePropertyChanged("TextPatologico");
        }
    }
}
