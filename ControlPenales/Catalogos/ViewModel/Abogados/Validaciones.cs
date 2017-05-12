
namespace ControlPenales
{
    partial class CatalogoAbogadosViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => Clave, () => !string.IsNullOrEmpty(Clave), "CLAVE REQUERIDA!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
        }
    }
}