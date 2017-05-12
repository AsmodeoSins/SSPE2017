using System;

namespace ControlPenales
{
    partial class PlanimetriaViewModel
    {
        void setValidacionesPlanimetria()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => IdSectorClasificacion, () => IdSectorClasificacion.HasValue ? (IdSectorClasificacion != -1 ? true : false) : false, "CLASIFICACION ES REQUERIDO!");
                OnPropertyChanged("IdSectorClasificacion");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar reglas de validacion", ex);
            }
        }
    }
}
