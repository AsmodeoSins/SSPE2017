using System;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {
        void setValidacionesUsoDrogas()
        {
            try
            {
                base.ClearRules();
                #region [Uso de Drogas]
                #region [Uso de Drogas]
                //base.AddRule(() => Droga, () => Droga != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => EdadIncio, () => EdadIncio >=0 && <=Edad, "EDAD INVÁLIDA");
                //base.AddRule(() => FechaUltimaDosis, () => FechaUltimaDosis <= System.DateTime.Now, "FECHA INVÁLIDA");
                //base.AddRule(() => FrecuenciaUso, () => FrecuenciaUso != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => ConsumoActual, () => !string.IsNullOrEmpty(ConsumoActual), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => TiempoConsumo, () => TiempoConsumo != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en uso droga", ex);
            }
        }
    }
}
