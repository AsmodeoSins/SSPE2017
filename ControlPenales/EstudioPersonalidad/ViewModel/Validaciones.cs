namespace ControlPenales
{
    partial class EstudioPersonalidadViewModel
    {
        void ValidacionesPopUpEstudioPersonalidad() 
        {
            base.ClearRules();
            base.AddRule(() => FechaSolicitud, () => FechaSolicitud.HasValue , "LA FECHA DE SOLICITUD ES REQUERIDA");
            base.AddRule(() => MotivoEstudioPadreSelected, () => MotivoEstudioPadreSelected.HasValue ? MotivoEstudioPadreSelected >= 1 : false, "EL MOTIVO ES REQUERIDO");
            base.AddRule(() => SolicitadoPor, () => !string.IsNullOrEmpty(SolicitadoPor), "EL NOMBRE DE QUIEN LO SOLICITO ES REQUERIDO");
            base.AddRule(() => SituacionEstudioPadreSelected, () => SituacionEstudioPadreSelected.HasValue ? SituacionEstudioPadreSelected >= 1 : false, "LA SITUACIÓN ES REQUERIDA");
            base.AddRule(() => FechaInicio, () => FechaInicio.HasValue, "LA FECHA DE INICIO ES REQUERIDA");
            base.AddRule(() => FechaFin, () => FechaFin.HasValue, "LA FECHA DE FIN ES REQUERIDA");

            OnPropertyChanged("FechaSolicitud");
            OnPropertyChanged("MotivoEstudioPadreSelected");
            OnPropertyChanged("SolicitadoPor");
            OnPropertyChanged("SituacionEstudioPadreSelected");
            OnPropertyChanged("FechaInicio");
            OnPropertyChanged("FechaFin");
        }

        void ValidacionesNoumOficio2() 
        {
            base.ClearRules();
            base.AddRule(() => TextAmpliarDescripcion, () => !string.IsNullOrEmpty(TextAmpliarDescripcion), "EL NÚMERO DE OFICIO ES REQUERIDO");
            OnPropertyChanged("TextAmpliarDescripcion");
        }

        private void SetValidacionesIncidente()
        {

            base.ClearRules();
            base.AddRule(() => IdIncidenteTipo, () => IdIncidenteTipo != -1, "TIPO ES REQUERIDO!");
            base.AddRule(() => FecIncidencia, () => FecIncidencia.HasValue ? FecIncidencia.Value <= Fechas.GetFechaDateServer : false, FecIncidencia == null ? "LA FECHA ES REQUERIDA" : "LA FECHA SELECCIONADA NO PUEDE SER MAYOR AL DIA ACTUAL");
            base.AddRule(() => Motivo, () => !string.IsNullOrEmpty(Motivo), "MOTIVO ES REQUERIDO!");
            OnPropertyChanged("IdIncidenteTipo");
            OnPropertyChanged("FecIncidencia");
            OnPropertyChanged("Motivo");
        }

        void ValidacionesPopUpEstudioPersonalidadDetalle()
        {
            base.ClearRules();
            base.AddRule(() => TipoEstudioSelectedDetalle, () => TipoEstudioSelectedDetalle.HasValue ? TipoEstudioSelectedDetalle >= 1 : false, "EL TIPO DE ESTUDIO ES REQUERIDO");
            base.AddRule(() => EstatusEstudioSelectedDetalle, () => EstatusEstudioSelectedDetalle.HasValue ? EstatusEstudioSelectedDetalle >= 1 : false, "EL ESTATUS DE ESTUDIO ES REQUERIDO");
            base.AddRule(() => FechaSolicitudEstudioSelectedDetalle, () => FechaSolicitudEstudioSelectedDetalle.HasValue, "LA FECHA DE SOLICITUD ES REQUERIDA");
            base.AddRule(() => FechaInicioSolicitudEstudioSelectedDetalle, () => FechaInicioSolicitudEstudioSelectedDetalle.HasValue, "LA FECHA DE INICIO ES REQUERIDO");
            base.AddRule(() => FechaFinSolicitudEstudioSelectedDetalle, () => FechaFinSolicitudEstudioSelectedDetalle.HasValue, "LA FECHA DE FIN ES REQUERIDO");
            base.AddRule(() => ResultadoSolicitudEstudioSelectedDetalle, () => !string.IsNullOrEmpty(ResultadoSolicitudEstudioSelectedDetalle), "EL RESULTADO DEL ESTUDIO ES REQUERIDO");
            base.AddRule(() => DiasBonificadosSolicitudEstudioSelectedDetalle, () => DiasBonificadosSolicitudEstudioSelectedDetalle.HasValue, "LOS DÍAS BONIFICADOS SON REQUERIDOS");

            OnPropertyChanged("TipoEstudioSelectedDetalle");
            OnPropertyChanged("EstatusEstudioSelectedDetalle");
            OnPropertyChanged("FechaSolicitudEstudioSelectedDetalle");
            OnPropertyChanged("FechaInicioSolicitudEstudioSelectedDetalle");
            OnPropertyChanged("FechaFinSolicitudEstudioSelectedDetalle");
            OnPropertyChanged("ResultadoSolicitudEstudioSelectedDetalle");
            OnPropertyChanged("DiasBonificadosSolicitudEstudioSelectedDetalle");
        }
    }
}
