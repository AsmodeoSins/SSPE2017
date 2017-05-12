
namespace ControlPenales
{
    partial class CatalogoTipoDelitoViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "DESCRIPCIÓN ES REQUERIDA!");
        }
    }
}