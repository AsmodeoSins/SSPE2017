
namespace ControlPenales
{
    partial class ProcedimientosSubtipoViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectTipoAtencionAgregar, () => SelectTipoAtencionAgregar != null ? SelectTipoAtencionAgregar.ID_TIPO_ATENCION > 0 : false, "TIPO DE ATENCIÓN ES REQUERIDO!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
        }

    }
}