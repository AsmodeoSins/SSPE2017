
namespace ControlPenales
{
    partial class CatalogoCamasViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => SelectedMunicipio, () => SelectedMunicipio.ID_MUNICIPIO != -1, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => SelectedCentro, () => SelectedCentro.ID_CENTRO != -1, "CENTRO ES REQUERIDO!");
            base.AddRule(() => SelectedEdificio, () => SelectedEdificio.ID_EDIFICIO != -1, "EDIFICIO ES REQUERIDO!");
            base.AddRule(() => SelectedSector, () => SelectedSector.ID_SECTOR != -1, "SECTOR ES REQUERIDO!");
            base.AddRule(() => SelectedCelda, () => !SelectedCelda.ID_CELDA.Equals("SELECCIONE"), "CELDA ES REQUERIDA!");
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
            OnPropertyChanged("SelectedMunicipio");
            OnPropertyChanged("SelectedCentro");
            OnPropertyChanged("SelectedEdificio");
            OnPropertyChanged("SelectedSector");
            OnPropertyChanged("SelectedCelda");
            OnPropertyChanged("Descripcion");
        }
    }
}