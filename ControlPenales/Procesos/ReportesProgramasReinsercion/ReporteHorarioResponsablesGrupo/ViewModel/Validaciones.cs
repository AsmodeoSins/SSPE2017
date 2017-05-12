using System;

namespace ControlPenales
{
    public partial class ReporteHorarioResponsableGruposViewModel
    {  
        #region Validaciones
        private void ValidarFiltros()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => SelectedResponsable, () => SelectedResponsable != 0, "RESPONSABLE ES REQUERIDO!");
                OnPropertyChanged("SelectedResponsable");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las reglas de validación", ex);
            }
        }
        #endregion
    }
}
