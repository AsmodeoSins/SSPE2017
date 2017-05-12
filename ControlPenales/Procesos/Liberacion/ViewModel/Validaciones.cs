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
    partial class LiberacionViewModel : ValidationViewModelBase, IPageViewModel
    {

        private void SetValidaciones() 
        {
            try
            {
                base.ClearRules();
                if (SelectedCausaPenal == null)
                {
                    base.RemoveRule("CPAnio");
                    base.RemoveRule("CPFolio");
                    base.AddRule(() => CPAnio, () => CPAnio.HasValue, "AÑO DE LA CAUSA PENAL ES REQUERIDA!");
                    base.AddRule(() => CPFolio, () => CPFolio.HasValue, "FOLIO DE LA CASA PENAL ES REQUERIDA!");
                    OnPropertyChanged("CPAnio");
                    OnPropertyChanged("CPFolio");
                }
                base.AddRule(() => EFecha, () => EFecha.HasValue, "FECHA DE EGRESO ES REQUERIDA!");
                base.AddRule(() => EOficio, () => !string.IsNullOrEmpty(EOficio), "NO.DE OFICIO ES REQUERIDO!");
                base.AddRule(() => EMotivo, () => EMotivo != -1, "MOTIVO ES REQUERIDO!");
                base.AddRule(() => EAutoridad, () => EAutoridad != -1, "AUTORIDAD ES REQUERIDA!");

                OnPropertyChanged("EFecha");
                OnPropertyChanged("EOficio");
                OnPropertyChanged("EMotivo");
                OnPropertyChanged("EAutoridad");
                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en liberación", ex);
            }
        }
    }
}
