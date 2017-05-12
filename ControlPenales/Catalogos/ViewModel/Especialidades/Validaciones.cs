using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogoEspecialidadesViewModel
    {
        void setValidaciones()
        {
            base.ClearRules();
            AddRule(()=>TextEspecialidad,()=>!string.IsNullOrWhiteSpace(TextEspecialidad),"CAPTURE UNA ESPECIALIDAD");
            AddRule(() => SelectedEstatus, () => SelectedEstatus != null && SelectedEstatus.CLAVE != "-1", "SELECCIONE UN ESTATUS!");
            RaisePropertyChanged("TextEspecialidad");
            RaisePropertyChanged("SelectedEstatus");
        }
    }
}
