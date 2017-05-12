using System;

namespace ControlPenales
{
    public partial class ReporteHorarioAreaViewModel
    {  
        #region Validaciones
        private void ValidarFiltros()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => SelectedArea, () => !SelectedArea.HasValue, "AREA ES REQUERIDO!");
                OnPropertyChanged("SelectedArea");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las reglas de validación", ex);
            }
        }
        #endregion
    }
}
