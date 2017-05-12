
namespace ControlPenales
{
    partial class ControlInternosViewModel
    {
        #region [VALIDACIONES]
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => SelectedSector, () => (SelectedEdificio.ID_EDIFICIO > 0 && SelectedSector.ID_SECTOR > 0) || (SelectedSector.ID_SECTOR == -1 && SelectedEdificio.ID_EDIFICIO == -1), "SECTOR ES REQUERIDO!");
        }
        #endregion
    }
}
