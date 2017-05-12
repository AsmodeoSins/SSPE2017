using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class ConfiguracionDepartamentoAreaTecnicaViewModel
    {

        private void SetValidacionesGenerales()
        {
            ClearRules();
            AddRule(() => SelectedDepartamentoValue, () => SelectedDepartamentoValue!=-1, "SELECCIONAR DEPARTAMENTO!");
            RaisePropertyChanged("SelectedDepartamentoValue");
        }

    }
}
