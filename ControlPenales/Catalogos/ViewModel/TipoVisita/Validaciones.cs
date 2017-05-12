
namespace ControlPenales
{
    partial class CatalogoTipoVisitaViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus!=null, "ESTATUS ES REQUERIDO!");
            OnPropertyChanged("Descripcion");
            OnPropertyChanged("SelectedEstatus");
           // base.AddRule(() => Descripcion, () => SELECTED, "DESCRIPCIÓN ES REQUERIDA!");
        }
    }
}