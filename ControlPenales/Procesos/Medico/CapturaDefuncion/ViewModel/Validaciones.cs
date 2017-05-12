using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CapturaDefuncionViewModel
    {
        public void setValidacionesCapturaDefuncion()
        {
            ClearRules();
            AddRule(() => TextLugarDeceso, () => !string.IsNullOrWhiteSpace(TextLugarDeceso), "LUGAR DEL DECESO ES REQUERIDA");
            AddRule(() => TextHechosDeceso, () => !string.IsNullOrWhiteSpace(TextHechosDeceso), "HECHOS ES REQUERIDA");
            AddRule(() => IsFechaDecesoValida, () => IsFechaDecesoValida, "FECHA DE DECESO ES REQUERIDA");
            AddRule(() => IsEnfermedadValida, () => IsEnfermedadValida, "CAUSA DECESO ES REQUERIDA");
            RaisePropertyChanged("TextLugarDeceso");
            RaisePropertyChanged("TextHechosDeceso");
            RaisePropertyChanged("IsFechaDecesoValida");
        }
    }
}
