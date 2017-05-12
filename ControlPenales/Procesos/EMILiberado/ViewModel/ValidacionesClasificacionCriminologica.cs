using System;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        void setValidacionesClasificacionCriminologica()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => ClasificacionCriminologica, () => ClasificacionCriminologica.HasValue ? ClasificacionCriminologica.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => PertenenciaCrimenOrganizado, () => PertenenciaCrimenOrganizado.HasValue ? PertenenciaCrimenOrganizado.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("ClasificacionCriminologica");
                OnPropertyChanged("PertenenciaCrimenOrganizado");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error", ex);
            }
        }

        void setValidacionesCriminodiagnostico()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => EgocentrismoSelected, () => EgocentrismoSelected.HasValue ? EgocentrismoSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => AgresividadSelected, () => AgresividadSelected.HasValue ? AgresividadSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => IndiferenciaAfectivaSelected, () => IndiferenciaAfectivaSelected.HasValue ? IndiferenciaAfectivaSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => LabilidadAfectivaSelected, () => LabilidadAfectivaSelected.HasValue ? LabilidadAfectivaSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");

                base.AddRule(() => AdaptacionSocialSelected, () => AdaptacionSocialSelected.HasValue ? AdaptacionSocialSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => LiderazgoSelected, () => LiderazgoSelected.HasValue ? LiderazgoSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => ToleranciaFrustracionSelected, () => ToleranciaFrustracionSelected.HasValue ? ToleranciaFrustracionSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => ControlImpulsosSelected, () => ControlImpulsosSelected.HasValue ? ControlImpulsosSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");

                base.AddRule(() => CapacidadCriminalSelected, () => CapacidadCriminalSelected.HasValue ? CapacidadCriminalSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => PronosticoIntrainstitucionalSelected, () => PronosticoIntrainstitucionalSelected.HasValue ?
                    PronosticoIntrainstitucionalSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => IndiceEstadoPeligrosoSelected, () => IndiceEstadoPeligrosoSelected.HasValue ? IndiceEstadoPeligrosoSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => UbicacionClasificacionCriminologicaSelected, () => UbicacionClasificacionCriminologicaSelected.HasValue ?
                    UbicacionClasificacionCriminologicaSelected.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");

                OnPropertyChanged("EgocentrismoSelected");
                OnPropertyChanged("AgresividadSelected");
                OnPropertyChanged("IndiferenciaAfectivaSelected");
                OnPropertyChanged("LabilidadAfectivaSelected");

                OnPropertyChanged("AdaptacionSocialSelected");
                OnPropertyChanged("LiderazgoSelected");
                OnPropertyChanged("ToleranciaFrustracionSelected");
                OnPropertyChanged("ControlImpulsosSelected");

                OnPropertyChanged("CapacidadCriminalSelected");
                OnPropertyChanged("PronosticoIntrainstitucionalSelected");
                OnPropertyChanged("IndiceEstadoPeligrosoSelected");
                OnPropertyChanged("UbicacionClasificacionCriminologicaSelected");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer las validaciones en criminológica diagnóstico", ex);
            }
        }

        void setValidacionesClasificacionCriminologicaPop()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => MotivoProceso, () => string.IsNullOrEmpty(MotivoProceso) != true, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => Cantidad, () => Cantidad.HasValue ? Cantidad != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => TiempoSancionProceso, () => string.IsNullOrEmpty(TiempoSancionProceso) != true, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged();
                //base.AddRule(() => ClasificacionCriminologica, () => ClasificacionCriminologica != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => PertenenciaCrimenOrganizado, () => PertenenciaCrimenOrganizado != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => MotivoProceso, () => !string.IsNullOrEmpty(MotivoProceso), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Cantidad, () => Cantidad != 0, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => TiempoSancionProceso, () => !string.IsNullOrEmpty(TiempoSancionProceso), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => NuevoProceso, () => !string.IsNullOrEmpty(NuevoProceso), "ESTE CAMPO ES REQUERIDO PARA GRABAR");            
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer la validación en clasificación criminológica", ex);
            }
        }
    }
}
