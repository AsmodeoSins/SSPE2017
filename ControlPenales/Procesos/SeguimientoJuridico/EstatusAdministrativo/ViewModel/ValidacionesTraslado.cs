using System;
using System.ComponentModel;

namespace ControlPenales
{
    partial class EstatusAdministrativoViewModel
    {
        void setValidacionesTraslado()
        {
            try
            {
                base.RemoveRule("DTMotivo");
                base.RemoveRule("DTJustificacion");
                base.RemoveRule("DTCentroOrigen");
                base.RemoveRule("DTNoOficio");
                base.AddRule(() => DTMotivo, () => DTMotivo!=-1, "MOTIVO DE TRASLADO ES REQUERIDO!");
                base.AddRule(() => DTJustificacion, () => !string.IsNullOrWhiteSpace(DTJustificacion), "JUSTIFICACIÓN ES REQUERIDA!");
                base.AddRule(() => DTCentroOrigen, () => DTCentroOrigen!=-1, "CENTRO DE ORIGEN ES REQUERIDO!");
                base.AddRule(() => DTNoOficio, () =>!string.IsNullOrWhiteSpace(DTNoOficio), "OFICIO ES REQUERIDO!");
                RaisePropertyChanged("DTMotivo");
                RaisePropertyChanged("DTJustificacion");
                RaisePropertyChanged("DTCentroOrigen");
                RaisePropertyChanged("DTNoOficio");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear validaciones.", ex);
            }
        }

        void unsetValidacionesTraslado()
        {
            try
            {
                base.RemoveRule("DTMotivo");
                base.RemoveRule("DTJustificacion");
                base.RemoveRule("DTCentroOrigen");
                base.RemoveRule("DTNoOficio");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al quitar las validaciones.", ex);
            }
        }
    }
}