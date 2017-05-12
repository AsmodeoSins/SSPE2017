namespace ControlPenales
{
    public partial class HojaEnfermeriaViewModel
    {
        void ValidacionesIncidencias() 
        {
            try
            {
                base.AddRule(() => Observacion, () => !string.IsNullOrEmpty(Observacion), "OBSERVACIONES ES REQUERIDO!");
                base.AddRule(() => SelectedIncidenteMotivoValue, () => SelectedIncidenteMotivoValue.HasValue ? SelectedIncidenteMotivoValue != -1 : false, "MOTIVO DE INCIDENCIA ES REQUERIDO!");
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
    }
}