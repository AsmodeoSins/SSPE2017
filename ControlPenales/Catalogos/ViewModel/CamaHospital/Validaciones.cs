
namespace ControlPenales
{
    public partial class CatalogoCamasHospitalViewModel
    {
        private void ValidacionEstatusBusqueda()
        {
            base.ClearRules();
            base.AddRule(() => SelectedEstatusBusqueda, () => SelectedEstatusBusqueda.CLAVE != SELECCIONAR, "SELECCIONE UN ESTATUS.");
            OnPropertyChanged("SelectedEstatusBusqueda");
        }

        private void ValidacionGuardar()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DEBE AGREGAR UNA DESCRIPCIÓN");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus.CLAVE != SELECCIONAR, "DEBE SELECCIONAR UN ESTATUS.");
            OnPropertyChanged("Descripcion");
            OnPropertyChanged("SelectedEstatus");
        }

    }
}
