
namespace ControlPenales
{
    partial class CatalogoTipoRelacionViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus!=null, "ESTATUS ES REQUERIDO!");

        }
    }
}