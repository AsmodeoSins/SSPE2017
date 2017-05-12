using System;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        void setValidacionesTatuajes()
        {
            try
            {
                base.ClearRules();
                //#region [Tatuajes]
                //#region [Tatuajes]
                //base.AddRule(() => CantidadAntesIngresoAntisocial, () => CantidadAntesIngresoAntisocial != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadIntramurosAntisocial, () => CantidadIntramurosAntisocial != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadAntesIngresoErotico, () => CantidadAntesIngresoErotico != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadIntramurosErotico, () => CantidadIntramurosErotico != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadAntesIngresoReligioso, () => CantidadAntesIngresoReligioso != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadIntramurosReligioso, () => CantidadIntramurosReligioso != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadAntesIngresoIdentificacion, () => CantidadAntesIngresoIdentificacion != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadIntramurosIdentificacion, () => CantidadIntramurosIdentificacion != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadAntesIngresoDecorativo, () => CantidadAntesIngresoDecorativo != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadIntramurosDecorativo, () => CantidadIntramurosDecorativo != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadAntesIngresoSentimental, () => CantidadAntesIngresoSentimental != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CantidadIntramurosSentimental, () => CantidadIntramurosSentimental != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => TatuajesTotal, () => TatuajesTotal != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => TatuajesDescripcion, () => TatuajesDescripcion != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //#endregion
                //#endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en tatuajes", ex);
            }
        }
    }
}
