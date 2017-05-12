
namespace ControlPenales
{
    partial class CatalogoModalidadDelitoViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedTipo, () => SelectedTipo != null && SelectedTipo.ID_TIPO_DELITO > 0, "TIPO DE DELITO ES REQUERIDO!");
        }
    }
}