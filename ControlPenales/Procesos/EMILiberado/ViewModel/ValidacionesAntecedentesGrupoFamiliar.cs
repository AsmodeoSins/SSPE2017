using System;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        void setValidacionesAntecedentesGrupoFamiliar()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => SelectedParentescoFDroga, () => SelectedParentescoFDroga != null ? SelectedParentescoFDroga.ID_TIPO_REFERENCIA > 0 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => AnioFDroga, () => AnioFDroga.HasValue, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => SelectedDrogaFDroga, () => SelectedDrogaFDroga != null ? SelectedDrogaFDroga.ID_DROGA > 0 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => SelectedRelacionFDroga, () => SelectedRelacionFDroga != null ? SelectedRelacionFDroga.ID_RELACION > 0 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en antecedentes grupo familiar", ex);
            }
        }
    }
}
