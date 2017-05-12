using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class ConfiguracionDepartamentosViewModel
    {

        void ValidacionDatosGenerales()
        {
            base.ClearRules();
            base.AddRule(() => SelectPaisNacimiento, () => SelectPaisNacimiento > -1, "PAÍS ES REQUERIDO!");
            base.AddRule(() => SelectEntidadNacimiento, () => SelectEntidadNacimiento > -1, "ESTADO ES REQUERIDO!");
            base.AddRule(() => SelectMunicipioNacimiento, () => SelectMunicipioNacimiento > -1, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => SelectCentro, () => SelectCentro > -1, "CENTRO ES REQUERIDO!");
            base.AddRule(() => SelectedCoordinador, () => SelectedCoordinador.ID_EMPLEADO > -1, "COORDINADOR ES REQUERIDO!");


            OnPropertyChanged("SelectPaisNacimiento");
            OnPropertyChanged("SelectEntidadNacimiento");
            OnPropertyChanged("SelectMunicipioNacimiento");
            OnPropertyChanged("SelectCentro");
            OnPropertyChanged("SelectedCoordinador");
        }
    }
}
