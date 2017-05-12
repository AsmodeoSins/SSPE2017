

namespace ControlPenales
{
    partial class CatalogoAreasViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCION ES REQUERIDA!");
            base.AddRule(() => SelectedTipo, () => SelectedTipo == null && SelectedTipo.ID_CENTRO >= 1, "CENTRO ES REQUERIDO!");
        }
    }
}