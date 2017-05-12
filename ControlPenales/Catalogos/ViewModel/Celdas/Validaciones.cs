
namespace ControlPenales
{
    partial class CatalogoCeldasViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedSector, () => SelectedSector.ID_SECTOR >= 1, "SECTOR ES REQUERIDO!");
            base.AddRule(() => SelectedEdificio, () => SelectedEdificio.ID_EDIFICIO >= 1, "EDIFICIO ES REQUERIDO!");
            base.AddRule(() => SelectedMunicipio, () => SelectedMunicipio.ID_MUNICIPIO >= 1, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => SelectedCentro, () => SelectedCentro.ID_CENTRO >= 1, "CENTRO ES REQUERIDO!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
        }
    }
}