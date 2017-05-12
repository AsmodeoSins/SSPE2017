using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class AgendaEnfermeroViewModel
    {
        public void setBuscarAgendaValidacionRules()
        {
            ClearRules();
            AddRule(() => FechaAgendaValid, () => FechaAgendaValid, "FECHA DE LA AGENDA ES REQUERIDA!");
            RaisePropertyChanged("FechaAgendaValid");
        }

        public void setAgregarAgendaValidacionRules()
        {
            base.ClearRules();
            base.AddRule(() => AgregarAgendaFechaValid, () => AgregarAgendaFechaValid, "LA FECHA DE LA CITA ES REQUERIDA!");
            base.AddRule(() => AgregarAgendaHoraI, () => AgregarAgendaHoraI.HasValue, "LA HORA INICIAL DE LA CITA ES REQUERIDA!");
            base.AddRule(() => AgregarAgendaHoraF, () => AgregarAgendaHoraF.HasValue && AgregarAgendaHorasValid, "LA HORA FINAL DE LA CITA ES REQUERIDA Y TIENE QUE SER MAYOR A LA HORA INICIAL!");
            base.AddRule(() => SelectedArea, () => SelectedArea.HasValue && SelectedArea.Value != -1, "EL AREA ES REQUERIDA!");
            RaisePropertyChanged("AgregarAgendaFechaValid");
            RaisePropertyChanged("AgregarAgendaHoraI");
            RaisePropertyChanged("AgregarAgendaHoraF");
            RaisePropertyChanged("SelectedArea");
        }

        public void setIncidenteMotivo()
        {
            base.ClearRules();
            base.AddRule(() => SelectedIncidenteMotivoValue, () => SelectedIncidenteMotivoValue != -1, "EL MOTIVO DEL INCIDENTE ES REQUERIDO");
            RaisePropertyChanged("SelectedIncidenteMotivoValue");
        }

    }
}
