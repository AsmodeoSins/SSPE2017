namespace ControlPenales
{
    partial class NotificacionTrabajoSocialViewModel
    {
        void ValidacionesNotificacionMedico()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => FechaSolicitud, () => FechaSolicitud.HasValue, "FECHA DE SOLICITUD ES REQUERIDA!");
                OnPropertyChanged("FechaSolicitud");
                //base.AddRule(() => Expediente, () => Expediente.HasValue ? Expediente.Value != decimal.Zero ? Expediente.Value != -1 : false : false, "EXPEDIENTE ES REQUERIDO!");
                //OnPropertyChanged("Expediente");
                base.AddRule(() => MotivoNotificacion, () => !string.IsNullOrEmpty(MotivoNotificacion), "MOTIVO ES REQUERIDO!");
                OnPropertyChanged("MotivoNotificacion");
                base.AddRule(() => CaracterNotificacion, () => CaracterNotificacion.HasValue ? CaracterNotificacion != decimal.Zero ? CaracterNotificacion != -1 : false : false, "CARÁCTER DE NOTIFICACIÓN ES REQUERIDO!");
                OnPropertyChanged("CaracterNotificacion");
                base.AddRule(() => RiesgosNotificacionTS, () => RiesgosNotificacionTS.HasValue ? RiesgosNotificacionTS != decimal.Zero ? RiesgosNotificacionTS != -1 : false : false, "RIESGOS ES REQUERIDO!");
                OnPropertyChanged("RiesgosNotificacionTS");
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }

        void ValidacionesTrabajoSocial() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => MensajeRespuestaTS, () => !string.IsNullOrEmpty(MensajeRespuestaTS), "MENSAJE ES REQUERIDO!");
                OnPropertyChanged("MensajeRespuestaTS");
            }
            catch (System.Exception exc)
            {
                
                throw;
            }
        }
    }
}