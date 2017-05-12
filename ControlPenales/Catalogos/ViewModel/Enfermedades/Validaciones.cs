using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoEnfermedadesViewModel
    {
        private void setValidacionRules()
        {
            ClearRules();
            AddRule(()=>SelectedTipoEnfermedadValue,()=>SelectedTipoEnfermedadValue!="-1","TIPO DE ENFERMEDAD ES REQUERIDO!");
            AddRule(()=>TextLetraEnfermedad,()=>!string.IsNullOrWhiteSpace(TextLetraEnfermedad),"LETRA ES REQUERIDA!");
            AddRule(()=>TextClaveEnfermedad,()=>!string.IsNullOrWhiteSpace(TextClaveEnfermedad),"CLAVE ES REQUERIDA!");
            AddRule(()=>TextEnfermedad,()=>!string.IsNullOrWhiteSpace("TextEnfermedad"),"ENFERMEDAD ES REQUERIDA!");
            RaisePropertyChanged("SelectedTipoEnfermedadValue");
            RaisePropertyChanged("TextLetraEnfermedad");
            RaisePropertyChanged("TextClaveEnfermedad");
            RaisePropertyChanged("TextEnfermedad");
        }
    }
}
