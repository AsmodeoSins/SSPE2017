
namespace ControlPenales
{
    partial class CatalogoCiudadesViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDO!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
            //base.AddRule(() => SelectedMunicipio, () => SelectedMunicipio != null && SelectedMunicipio.ID_ENTIDAD > 0, "MUNICIPIO ES REQUERIDO!");
        }
    }
}