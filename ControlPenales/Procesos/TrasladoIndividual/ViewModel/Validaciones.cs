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
using SSP.Servidor;

namespace ControlPenales
{
    partial class TrasladoIndividualViewModel : ValidationViewModelBase, IPageViewModel
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
                base.AddRule(() => DTAutorizado, () => DTAutorizado != -1, "AUTORIZO ES REQUERIDO!");


                base.AddRule(() => DEFechaValid, () => DEFechaValid, "FECHA DE EGRESO ES REQUERIDA!");
                base.AddRule(() => DENoOficio, () => !string.IsNullOrEmpty(DENoOficio), "NO.OFICIO DE SALIDA ES REQUERIDO!");
                base.AddRule(() => DEAutoridad, () => DEAutoridad != -1, "AUTORIDAD ES REQUERIDA!");
                base.AddRule(() => DEMotivo, () => DEMotivo != -1, "MOTIVO DE SALIDA ES REQUERIDA!");

                OnPropertyChanged("DTFechaValid");
                OnPropertyChanged("DTMotivo");
                OnPropertyChanged("DTJustificacion");
                OnPropertyChanged("DTCentroDestino");
                OnPropertyChanged("DTNoOficio");
                OnPropertyChanged("DTAutorizado");
                OnPropertyChanged("DEFechaValid");
                OnPropertyChanged("DENoOficio");
                OnPropertyChanged("DEAutoridad");
                OnPropertyChanged("DEMotivo");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en traslado", ex);
            }
        }
    }
}
