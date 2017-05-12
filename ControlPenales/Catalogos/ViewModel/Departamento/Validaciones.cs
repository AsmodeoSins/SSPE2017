

namespace ControlPenales
{
    partial class CatalogoDepartamentosViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedTipoValue, () => SelectedTipoValue != -1, "ROL ES REQUERIDO!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
            RaisePropertyChanged("Descripcion");
            RaisePropertyChanged("SelectedTipoValue");
            RaisePropertyChanged("SelectedEstatus");
        }
    }
}