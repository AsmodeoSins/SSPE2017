
namespace ControlPenales
{
    partial class YardasViewModel
    {
        private void setValidationRules()
        {

            base.ClearRules();
            base.AddRule(() => YEdificio, () => YEdificio != -1, "EDIFICIO ES REQUERIDO!");
            base.AddRule(() => YSector, () => YSector != -1, "SECTOR ES REQUERIDO!");
            base.AddRule(() => YArea, () => YArea != -1, "ÁREA ES REQUERIDO!");
            //celdas
            if (YCeldaInicio != null && YCeldaFin != null)
            {
                base.AddRule(() => YCeldaFin, () => YCeldaFin > YCeldaInicio, "CELDA FINAL DEBE SER MAYOR A LA CELDA INICIO");
            }
            else
            {
                base.AddRule(() => YCeldaInicio, () => YCeldaInicio != null, "CELDA INICIO ES REQUERIDO!");
                base.AddRule(() => YCeldaFin, () => YCeldaFin != null, "CELDA FIN ES REQUERIDO!");
            }
            base.AddRule(() => YDiaSemana, () => YDiaSemana != -1, "DIA DE LA SEMANA ES REQUERIDO!");

                if (YHoraInicio == null)
                    base.AddRule(() => YHoraInicio, () => YHoraInicio != null, "HORA INICIO ES REQUERIDO!");
                else
                    base.AddRule(() => YHoraInicio, () => YHoraInicio < 24, "HORA INICIO DEBE SER MENOR A 24!");

                if (YMinInicio == null)
                    base.AddRule(() => YMinInicio, () => YMinInicio != null, "MINUTO INICIO ES REQUERIDO!");
                else
                    base.AddRule(() => YMinInicio, () => YMinInicio < 60, "MINUTO INICIO DEBE SER MENOR A 60!");

                if (YHoraFin == null)
                    base.AddRule(() => YHoraFin, () => YHoraFin != null, "HORA FIN ES REQUERIDO!");
                else
                    base.AddRule(() => YHoraFin, () => YHoraFin < 24, "HORA FIN DEBE SER MENOR A 24!");

                if (YMinFin == null)
                    base.AddRule(() => YMinFin, () => YMinFin != null, "MINUTO FIN ES REQUERIDO!");
                else
                    base.AddRule(() => YMinFin, () => YMinFin < 60, "MINUTO FIN DEBE SER MENOR A 60!"); 
            
            OnPropertyChanged("YEdificio");
            OnPropertyChanged("YSector");
            OnPropertyChanged("YArea");
            OnPropertyChanged("YCeldaInicio");
            OnPropertyChanged("YCeldaFin");
            OnPropertyChanged("YDiaSemana");
            OnPropertyChanged("YHoraInicio");
            OnPropertyChanged("YMinInicio");
            OnPropertyChanged("YHoraFin");
            OnPropertyChanged("YMinFin");
        }

        private bool ValidaHora()
        {
            if ((((YHoraInicio * 60) + YMinInicio) > ((YHoraFin * 60) + YMinFin)))
            {
                new Dialogos().ConfirmacionDialogo("Validación","La hora final debe ser mayor a la hora inicio");
                return false;
            }
            return true;
        }
    }
}