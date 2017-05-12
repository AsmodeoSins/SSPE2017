using ControlPenales;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class SancionesDisciplinariasViewModel : ValidationViewModelBase
    {

        private void SetValidacionesIncidente()
        {

            base.ClearRules();
            base.AddRule(() => IdIncidenteTipo, () => IdIncidenteTipo != -1, "TIPO ES REQUERIDO!");
            //if (FecIncidencia.HasValue)
            //{
            //    base.AddRule(() => FecIncidencia, () => FecIncidencia.HasValue ? FecIncidencia.Value <= Fechas.GetFechaDateServer : false, "LA FECHA SELECCIONADA NO PUEDE SER MAYOR AL DIA ACTUAL");
            //    OnPropertyChanged("FecIncidencia");
            //}
            base.AddRule(() => FecIncidencia, () => FecIncidencia.HasValue ? FecIncidencia.Value <= Fechas.GetFechaDateServer : false, FecIncidencia == null ? "LA FECHA ES REQUERIDA" : "LA FECHA SELECCIONADA NO PUEDE SER MAYOR AL DIA ACTUAL");
            base.AddRule(() => Motivo, () => !string.IsNullOrEmpty(Motivo), "MOTIVO ES REQUERIDO!");
            OnPropertyChanged("IdIncidenteTipo");
            OnPropertyChanged("FecIncidencia");
            OnPropertyChanged("Motivo");
        }

        private void SetValidacionesSancion()
        {
            base.ClearRules();
            base.AddRule(() => IdSancionTipo, () => IdSancionTipo != -1 , "TIPO DE SANCION ES REQUERIDO!");
            //base.AddRule(() => SelectedSancion, () => SelectedSancion != -1, "TIPO DE SANCION ES REQUERIDO!");
            //OnPropertyChanged("SelectedSancion");
            base.AddRule(() => FechaInicio, () => FechaInicio.HasValue, "Fecha Inicio es Requerida");
            base.AddRule(() => FechaFin, () => FechaFin.HasValue, "Fecha Fin es Requerida");
            OnPropertyChanged("IdSancionTipo");
            OnPropertyChanged("FechaInicio");
            OnPropertyChanged("FechaFin");
        }
    }
}
