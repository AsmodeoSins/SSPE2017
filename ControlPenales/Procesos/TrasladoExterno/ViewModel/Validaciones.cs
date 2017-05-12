using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class TrasladoExternoViewModel
    {
        private void SetValidacionesTraslados()
        {
            try
            {
                base.ClearRules();

                base.AddRule(() => DTFechaValid, () => DTFechaValid, "FECHA DE TRASLADO ES REQUERIDO!");
                base.AddRule(() => DTMotivo, () => DTMotivo != -1, "MOTIVO ES REQUERIDO!");
                base.AddRule(() => DTJustificacion, () => !string.IsNullOrEmpty(DTJustificacion), "JUSTIFICACION ES REQUERIDO!");
                base.AddRule(() => DTNoOficio, () => !string.IsNullOrEmpty(DTNoOficio), "NO.OFICIO ES REQUERIDO!");
                base.AddRule(() => DENoOficio, () => !string.IsNullOrEmpty(DENoOficio), "NO.OFICIO DE SALIDA ES REQUERIDO!");
                base.AddRule(()=>SelectIngreso,()=>SelectIngreso!=null,"INGRESO ES REQUERIDO!");
                base.AddRule(() => AutorizaSalida, () => !string.IsNullOrWhiteSpace(AutorizaSalida) && AutorizaSalida != "SELECCIONE", "AUTORIZA SALIDA ES REQUERIDO!");
                base.AddRule(()=>SelectedEmisor,()=>SelectedEmisor!=-1,"CENTRO DESTINO ES OBLIGATORIO");
                OnPropertyChanged("DTFechaValid");
                OnPropertyChanged("DTMotivo");
                OnPropertyChanged("DTJustificacion");
                OnPropertyChanged("DTNoOficio");
                OnPropertyChanged("DENoOficio");
                OnPropertyChanged("SelectIngreso");
                RaisePropertyChanged("AutorizaSalida");
                OnPropertyChanged("SelectedEmisor");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en traslado", ex);
            }
        }

        private void SetValidacionesBuscaTraslados()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => SelectedTraslado, () => SelectedTraslado != null, "DEBE DE SELECCIONAR UN TRASLADO");
                RaisePropertyChanged("SelectedTraslado");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en traslado", ex);
            }
        }
    }
}
