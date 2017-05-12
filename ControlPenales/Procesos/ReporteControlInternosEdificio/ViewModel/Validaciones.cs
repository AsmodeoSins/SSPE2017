using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class ReporteControlInternoEdificioViewModel
    {
        private void Validaciones()
        {
            try
            {
                base.ClearRules();
                if (FechaInicio == null)
                    base.AddRule(() => FechaInicio, () => FechaInicio != null, "FECHA DE INICIO ES REQUERIDA!");
                if (FechaFin == null)
                    base.AddRule(() => FechaFin, () => FechaFin != null, "FECHA FIN ES REQUERIDO!");
                if (FechaInicio != null && FechaFin != null)
                    base.AddRule(() => FechaFin, () => FechaFin.Value.Date >= FechaInicio.Value.Date, "LA FECHA FIN DEBE SER MAYOR O IGUAL A LA FECHA INICIO!");
                OnPropertyChanged("FechaInicio");
                OnPropertyChanged("FechaFin");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las reglas de validación", ex);
            }
        }
    }
}
