using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class ExcarcelacionAutorizacionViewModel
    {
        void setValidacionesExcarcelacion_Actualizacion()
        {
            base.ClearRules();
            base.AddRule(() => SelectedExcarcelacion, () => SelectedExcarcelacion != null, "SELECCIONAR UNA EXCARCELACIÓN ES REQUERIDO!");
            base.AddRule(() => IsFechaIniValida, () => IsFechaIniValida, "LA FECHA DE INICIO TIENE QUE SER MENOR A LA FECHA FIN!");
            OnPropertyChanged("SelectedExcarcelacion");
            OnPropertyChanged("IsFechaIniValida");
        }

        void setValidacionesExcarcelacion_Cancela_Motivo()
        {
            base.ClearRules();
            base.AddRule(() => SelectedCancelacion_MotivoValue, () => selectedCancelacion_MotivoValue != 0, "MOTIVO DE LA CANCELACIÓN ES REQUERIDO!");
            base.AddRule(() => Cancelacion_Observacion, () => !string.IsNullOrWhiteSpace(Cancelacion_Observacion), "OBSERVACIÓN DE LA CANCELACIÓN ES REQUERIDO!");
            OnPropertyChanged("SelectedCancelacion_MotivoValue");
            OnPropertyChanged("Cancelacion_Observacion");
        }
    }
}
