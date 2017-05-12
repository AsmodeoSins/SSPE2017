
namespace ControlPenales
{
    partial class CatalogoColoniasViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "NOMBRE DE COLONIA ES REQUERIDO!");
            base.AddRule(() => SelectedMunicipio, () => SelectedMunicipio != null && SelectedMunicipio.ID_ENTIDAD > 0, "MUNICIPIO ES REQUERIDO!");
        }
    }
}