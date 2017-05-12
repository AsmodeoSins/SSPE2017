using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ControlPenales
{
    partial class TrasladoMasivoViewModel
    {
        private void SetValidacionesTraslados()
        {
            try
            {
                base.ClearRules();

                base.AddRule(() => DTFechaValid, () => DTFechaValid, "FECHA DE TRASLADO ES REQUERIDO!");
                base.AddRule(() => DTMotivo, () => DTMotivo != -1, "MOTIVO ES REQUERIDO!");
                base.AddRule(() => DTJustificacion, () => !string.IsNullOrEmpty(DTJustificacion), "JUSTIFICACION ES REQUERIDO!");
                base.AddRule(() => DTCentroDestino, () => DTCentroDestino != -1, "CENTRO DESTINO ES REQUERIDO!");
                base.AddRule(() => DTNoOficio, () => !string.IsNullOrEmpty(DTNoOficio), "NO.OFICIO ES REQUERIDO!");
                base.AddRule(() => DENoOficio, () => !string.IsNullOrEmpty(DENoOficio), "NO.OFICIO DE SALIDA ES REQUERIDO!");
                base.AddRule(() => LstIngresosSeleccionados, () => (LstIngresosSeleccionados!=null && LstIngresosSeleccionados.Count > 0), "DEBE DE AGREGAR POR LO MENOS UN IMPUTADO AL TRASLADO");
                base.AddRule(() => AutorizaSalida, () => !string.IsNullOrWhiteSpace(AutorizaSalida) && AutorizaSalida != "SELECCIONE", "AUTORIZA SALIDA ES REQUERIDO!");
                OnPropertyChanged("DTFechaValid");
                OnPropertyChanged("DTMotivo");
                OnPropertyChanged("DTJustificacion");
                OnPropertyChanged("DTCentroDestino");
                OnPropertyChanged("DTNoOficio");
                OnPropertyChanged("DENoOficio");
                OnPropertyChanged("LstIngresosSeleccionados");
                RaisePropertyChanged("AutorizaSalida");
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
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en traslado", ex);
            }
        }
    }
}
