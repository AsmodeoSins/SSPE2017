
namespace ControlPenales
{
    partial class CatalogoCentrosViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Nombre, () => !string.IsNullOrEmpty(Nombre), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => SelectEntidad, () => SelectEntidad != null && SelectEntidad.ID_ENTIDAD > 0, "ESTADO ES REQUERIDO!");
            base.AddRule(() => SelectMunicipio, () => SelectMunicipio != null && SelectMunicipio.ID_ENTIDAD > 0, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS REQUERIDO!");
        }
    }
}