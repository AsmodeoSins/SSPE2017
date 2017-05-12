namespace ControlPenales
{
    partial class AgendaEspecialistasViewModel
    {
        public void setAgregarAgendaValidacionRules()
        {
            base.ClearRules();
            base.AddRule(() => AgregarAgendaFechaValid, () => AgregarAgendaFechaValid, "LA FECHA DE LA CITA ES REQUERIDA!");
            base.AddRule(() => AgregarAgendaHoraI, () => AgregarAgendaHoraI.HasValue, "LA HORA INICIAL DE LA CITA ES REQUERIDA!");
            base.AddRule(() => AgregarAgendaHoraF, () => AgregarAgendaHoraF.HasValue && AgregarAgendaHorasValid, "LA HORA FINAL DE LA CITA ES REQUERIDA Y TIENE QUE SER MAYOR A LA HORA INICIAL!");
            base.AddRule(() => SelectedArea, () => SelectedArea.HasValue && SelectedArea.Value != -1, "EL ÁREA ES REQUERIDA!");
            RaisePropertyChanged("AgregarAgendaFechaValid");
            RaisePropertyChanged("AgregarAgendaHoraI");
            RaisePropertyChanged("AgregarAgendaHoraF");
            RaisePropertyChanged("SelectedArea");
        }

        public void setBuscarAgendaValidacionRules()
        {
            base.ClearRules();
            base.AddRule(() => FechaAgendaValid, () => FechaAgendaValid, "FECHA DE LA AGENDA ES REQUERIDA!");
            base.AddRule(() => SelectedAtencionTipo, () => SelectedAtencionTipo != -1, "EL TIPO DE ATENCIÓN ES REQUERIDO!");
            RaisePropertyChanged("SelectedAtencionTipo");
            RaisePropertyChanged("FechaAgendaValid");
        }

        public void setIncidenteMotivo()
        {
            base.ClearRules();
            base.AddRule(() => SelectedIncidenteMotivoValue, () => SelectedIncidenteMotivoValue != -1, "EL MOTIVO DEL INCIDENTE ES REQUERIDO");
            RaisePropertyChanged("SelectedIncidenteMotivoValue");
        }

        public void setBuscarAgendaImputadoValidacionRules()
        {
            base.ClearRules();
            base.AddRule(() => SelectedBusquedaAgendaAtencionTipo, () => SelectedBusquedaAgendaAtencionTipo != -1, "EL TIPO DE ATENCIÓN ES REQUERIDO!");
            RaisePropertyChanged("SelectedBusquedaAgendaAtencionTipo");
        }

        void LimpiaValidacionesBusqueda() 
        {
            base.RemoveRule("AgregarAgendaFechaValid");
            base.RemoveRule("AgregarAgendaHoraI");
            base.RemoveRule("AgregarAgendaHoraF");
            RaisePropertyChanged("AgregarAgendaFechaValid");
            RaisePropertyChanged("AgregarAgendaHoraI");
            RaisePropertyChanged("AgregarAgendaHoraF");
        }
    }
}