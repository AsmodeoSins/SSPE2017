using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GESAL.ViewModels
{
    public partial class CalendarizacionViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        private void setBuscarCalendarizacion()
        {
            base.ClearRules();
            base.AddRule(() => SelectedAlmacen, () => (SelectedAlmacen!=null), "SELECCIONE UN ALMACEN");
            RaisePropertyChanged("SelectedAlmacen");
        }

        private void setAgendarProductos()
        {
            base.ClearRules();
            base.AddRule(() => IsProgramadosOK, () => (IsProgramadosOK), "TODOS LAS ENTREGAS PROGRAMADAS TIENEN QUE SER MAYOR A 0 O MENOR AL NUMERO PENDIENTE");
            RaisePropertyChanged("IsProgramadosOK");
        }

        private void setRecalendarizacionConIncidencia()
        {
            base.ClearRules();
            base.AddRule(() => SelectedIncidencia_Tipo, () => (SelectedIncidencia_Tipo != null && SelectedIncidencia_Tipo.ID_TIPO_INCIDENCIA != -1), "TIPO DE INCIDENCIA ES OBLIGATORIO!");
            base.AddRule(() => Observacion_Incidencia, () => (!string.IsNullOrWhiteSpace(Observacion_Incidencia)), "OBSERVACION ES OBLIGATORIA!");
            base.AddRule(() => IsFechaValid, () => (IsFechaValid), "LA FECHA TIENE QUE SER VALIDA!");
            RaisePropertyChanged("SelectedIncidencia_Tipo");
            RaisePropertyChanged("Observacion_Incidencia");
            RaisePropertyChanged("IsFechaValid");
        }

        private void setRecalendarizacionSinIncidencia()
        {
            base.ClearRules();
            base.AddRule(() => IsFechaValid, () => (IsFechaValid), "LA FECHA TIENE QUE SER VALIDA!");
            RaisePropertyChanged("IsFechaValid");
        }

        private void setRulesIncidencia()
        {
            base.ClearRules();
            base.AddRule(() => SelectedIncidencia_Tipo, () => (SelectedIncidencia_Tipo != null && SelectedIncidencia_Tipo.ID_TIPO_INCIDENCIA != -1), "TIPO DE INCIDENCIA ES OBLIGATORIO!");
            base.AddRule(() => Observacion_Incidencia, () => (!string.IsNullOrWhiteSpace(Observacion_Incidencia)), "OBSERVACION ES OBLIGATORIA!");
            RaisePropertyChanged("SelectedIncidencia_Tipo");
            RaisePropertyChanged("Observacion_Incidencia");
        }
    }
}
