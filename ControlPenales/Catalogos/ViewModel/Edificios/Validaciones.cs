
namespace ControlPenales
{
    partial class CatalogoEdificiosViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedCentro, () => SelectedCentro.ID_CENTRO >= 1, "CENTRO ES REQUERIDO!");
            base.AddRule(() => SelectedMunicipio, () => SelectedMunicipio.ID_MUNICIPIO >= 1, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
        }
    }
}