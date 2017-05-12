using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class ControlSancionesViewModel
    {
        private void SetValidacionesSancion()
        {
            base.ClearRules();
            base.AddRule(() => IdSancionTipo, () => IdSancionTipo != -1, "TIPO DE SANCION ES REQUERIDO!");
            OnPropertyChanged("IdSancionTipo");

            base.AddRule(() => FechaInicio, () => FechaInicio.HasValue, "FECHA DE INICIO ES REQUERIDA!");
            base.AddRule(() => FechaFin, () => FechaFin.HasValue, "FECHA DE FIN ES REQUERIDA!");
            OnPropertyChanged("FechaInicio");
            OnPropertyChanged("FechaFin");
        }
    }
}
