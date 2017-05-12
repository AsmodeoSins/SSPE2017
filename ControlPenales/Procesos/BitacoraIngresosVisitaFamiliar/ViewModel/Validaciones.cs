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
    public partial class BitacoraIngresosVisitaFamiliarViewModel
    {
        #region Validaciones
        private void ValidarFiltros()
        {
            try
            {
                base.ClearRules();
                if (FFechaInicio == null)
                    base.AddRule(() => FFechaInicio, () => FFechaInicio != null, "FECHA DE INICIO ES REQUERIDA!");
                //if (FFechaFin == null)
                //    base.AddRule(() => FFechaFin, () => FFechaFin != null, "FECHA FIN ES REQUERIDO!");
                //if(FFechaInicio != null && FFechaFin != null)
                //    base.AddRule(() => FFechaFin, () => FFechaFin.Value.Date >= FFechaInicio.Value.Date, "LA FECHA FIN DEBE SER MAYOR O IGUAL A LA FECHA INICIO!");
                OnPropertyChanged("FFechaInicio");
                //OnPropertyChanged("FFechaFin");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las reglas de validación", ex);
            }
        }
        #endregion

    }
}
