
namespace ControlPenales
{
    partial class CatalogoSectorClasificacionViewModel
    {
        private void setValidationRules()
        {
           
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "POBLACIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus!=null, "ESTATUS ES REQUERIDO!");
            base.AddRule(() => Observacion, () => !string.IsNullOrEmpty(Observacion), "DESCRIPCIÓN ES REQUERIDO!");
            OnPropertyChanged("Descripcion");
            OnPropertyChanged("SelectedEstatus");
            OnPropertyChanged("Observacion");
        }
    }
}