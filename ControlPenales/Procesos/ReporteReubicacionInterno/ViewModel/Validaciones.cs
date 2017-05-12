using ControlPenales;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteReubicacionInternoViewModel
    {
        #region Validaciones
        private void ValidarFiltros()
        {
            try
            {
                base.ClearRules();
                //if (FFechaInicio == null)
                base.AddRule(() => FFechaInicio, () => FFechaInicio != null, "FECHA DE INICIO ES REQUERIDA!");
                //if (FFechaFin == null)
                base.AddRule(() => FFechaFin, () => FFechaFin != null, "FECHA FIN ES REQUERIDO!");

                base.AddRule(() => FFechaFin, () => FFechaFin.Value.Date >= FFechaInicio.Value.Date, "LA FECHA INICIO DEBE SER MENOR A LA FECHA FIN!");
                //if(FFechaInicio != null && FFechaFin != null)
                OnPropertyChanged("FFechaInicio");
                OnPropertyChanged("FFechaFin");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las reglas de validación", ex);
            }
        }
        #endregion

    }
}
