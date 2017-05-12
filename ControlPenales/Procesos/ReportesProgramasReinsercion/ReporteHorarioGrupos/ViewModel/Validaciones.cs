using System;

namespace ControlPenales
{
    public partial class ReporteHorarioGruposViewModel
    {  
        #region Validaciones
        private void ValidarFiltros()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => SelectedEje, () => !SelectedEje.HasValue, "EJE ES REQUERIDO!");
                base.AddRule(() => SelectedPrograma, () => !SelectedPrograma.HasValue, "PROGRAMA ES REQUERIDO!");
                base.AddRule(() => SelectedActividad, () => !SelectedActividad.HasValue, "ACTIVIDAD ES REQUERIDA!");
                base.AddRule(() => SelectedGrupo, () => !SelectedGrupo.HasValue, "GRUPO ES REQUERIDO!");
                OnPropertyChanged("SelectedEje");
                OnPropertyChanged("SelectedPrograma");
                OnPropertyChanged("SelectedActividad");
                OnPropertyChanged("SelectedGrupo");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las reglas de validación", ex);
            }
        }
        #endregion
    }
}
