using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class ReAgendaTrasladoVirtualViewModel
    {
        public void setAgregarAgendaValidacionRules()
        {
            base.ClearRules();
            base.AddRule(() => AgregarCitaMedicaAgendaFechaValid, () => AgregarCitaMedicaAgendaFechaValid, "LA FECHA DE LA CITA ES REQUERIDA!");
            base.AddRule(() => AgregarCitaMedicaAgendaHoraI, () => AgregarCitaMedicaAgendaHoraI.HasValue, "LA HORA INICIAL DE LA CITA ES REQUERIDA!");
            base.AddRule(() => AgregarCitaMedicaAgendaHoraF, () => AgregarCitaMedicaAgendaHoraF.HasValue && AgregarCitaMedicaAgendaHorasValid, "LA HORA FINAL DE LA CITA ES REQUERIDA Y TIENE QUE SER MAYOR A LA HORA INICIAL!");
            base.AddRule(() => SelectedCitaMedicaArea, () =>  SelectedCitaMedicaArea != -1, "EL AREA ES REQUERIDA!");
            RaisePropertyChanged("AgregarCitaMedicaAgendaFechaValid");
            RaisePropertyChanged("AgregarCitaMedicaAgendaHoraI");
            RaisePropertyChanged("AgregarCitaMedicaAgendaHoraF");
            RaisePropertyChanged("SelectedCitaMedicaArea");
        }

        private void setValidacionesInterconsultaExterna()
        {
            ClearRules();
            AddRule(() => SelectedTV_Inter_HospitalValue, () => SelectedTV_Inter_HospitalValue != -1, "SELECCIONE UNA INSTITUCIÓN MÉDICA");
            AddRule(() => SelectedTV_Inter_Cita_TipoValue, () => SelectedTV_Inter_Cita_TipoValue != -1, "SELECCIONE UN TIPO DE ATENCIÓN DEL PACIENTE");
            AddRule(() => TV_Inter_HR_Motivo, () => !string.IsNullOrWhiteSpace(TV_Inter_HR_Motivo), "CAPTURE EL MOTIVO");
            AddRule(() => IsFechaCitaValid, () => ((!isfechacitarequerida && !TV_Inter_FechaCita.HasValue) || (TV_Inter_FechaCita.HasValue && IsFechaCitaValid)), "FECHA DE LA CITA ES REQUERIDA!");
            RaisePropertyChanged("IsFechaCitaValid");
            RaisePropertyChanged("SelectedTV_Inter_HospitalValue");
            RaisePropertyChanged("SelectedTV_Inter_Cita_TipoValue");
            RaisePropertyChanged("TV_Inter_HR_Motivo");
        }

        private void setValidacionesRefMedica()
        {
            ClearRules();
            AddRule(() => SelectedTV_Inter_CentroValue, () => SelectedTV_Inter_CentroValue != -1, "SELECCIONE UN CENTRO");
            AddRule(() => TV_Inter_Motivo, () => !string.IsNullOrWhiteSpace(TV_Inter_Motivo), "CAPTURE EL MOTIVO");
            RaisePropertyChanged("SelectedTV_Inter_CentroValue");
            RaisePropertyChanged("TV_Inter_Motivo");
        }

        private void setValidacionHospitalOtros()
        {
            AddRule(()=>TV_Inter_Otro_Hospital,()=>!string.IsNullOrWhiteSpace(TV_Inter_Otro_Hospital),"CAPTURE LA INSTITUCIÓN MÉDICA");
            RaisePropertyChanged("TV_Inter_Otro_Hospital");
        }

        private void clearValidacionHospitalOtros()
        {
            RemoveRule("TV_Inter_Otro_Hospital");
        }

    }
}
