using System;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {

        void setValidacionesActividades()
        {
            base.ClearRules();
        }

        void setValidacionesActividadesPop()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => popupAnioActividad, () => !string.IsNullOrEmpty(popupAnioActividad.ToString()), "INGRESE UN AÑO VÁLIDO");
                base.AddRule(() => popupDescrActividades, () => !string.IsNullOrEmpty(popupDescrActividades), "ESTE CAMPO NO PUEDE ESTAR VACÍO");
                base.AddRule(() => popupDuracionActividad, () => !string.IsNullOrEmpty(popupDuracionActividad), "ESTE CAMPO NO PUEDE ESTAR VACÍO");
                //base.AddRule(() => popupEstatusPrograma, () => popupEstatusPrograma.HasValue ? !string.IsNullOrEmpty(popupEstatusPrograma.Value.ToString()) : false, "ELIJA UNA OPCIÓN");
                base.AddRule(() => popupEstatusPrograma, () => popupEstatusPrograma != -1, "ELIJA UNA OPCIÓN");
                base.AddRule(() => popupNoProg, () => popupNoProg.HasValue ? !string.IsNullOrEmpty(popupNoProg.Value.ToString()) && popupNoProg.Value <= 20 : false, "EL VALOR DEBE SER ENTRE 1 Y 20");
                base.AddRule(() => popupTipoActividad, () => popupTipoActividad != -1, "ELIJA UNA ACTIVIDAD");

                OnPropertyChanged("popupNoProg");
                OnPropertyChanged("popupEstatusPrograma");
                OnPropertyChanged("popupDuracionActividad");
                OnPropertyChanged("popupDescrActividades");
                OnPropertyChanged("popupAnioActividad");
                OnPropertyChanged("popupTipoActividad");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en actividades", ex);
            }
        }
    }
}
