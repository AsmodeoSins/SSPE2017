

namespace ControlPenales
{
    partial class CatalogoProgramasViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Nombre, () => !string.IsNullOrEmpty(Nombre), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedTipo, () => SelectedTipo != null ? SelectedTipo != -1 : false, "DEPARTAMENTO ES REQUERIDO!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
            OnPropertyChanged("Nombre");
            OnPropertyChanged("Descripcion");
            OnPropertyChanged("SelectedTipo");
            OnPropertyChanged("SelectedEstatus");
        }
    }
}