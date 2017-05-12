
namespace ControlPenales
{
    partial class ProcedimientosMedicosViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => SelectSubtipoAgregar, () => SelectSubtipoAgregar != null ? SelectSubtipoAgregar.ID_PROCMED_SUBTIPO > 0 : false, "TIPO DE ATENCIÓN ES REQUERIDO!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus != null, "ESTATUS ES REQUERIDO!");
        }

    }
}