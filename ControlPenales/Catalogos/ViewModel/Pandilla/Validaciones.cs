
namespace ControlPenales
{
    partial class CatalogoPandillaViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Nombre, () => !string.IsNullOrEmpty(Nombre), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => Ubicacion, () => !string.IsNullOrEmpty(Ubicacion), "UBICACIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
        }
     
    }
}