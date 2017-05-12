using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class AtencionSolicitudInterconsultaViewModel
    {


        private void setValidacionesInterconsultaExterna()
        {
            AddRule(() => SelectedRefMedHospital, () => SelectedRefMedHospital != -1, "SELECCIONE UNA INSTITUCIÓN MÉDICA");
            AddRule(() => SelectedRefMedTipoCitaValue, () => SelectedRefMedTipoCitaValue != -1, "SELECCIONE UN TIPO DE ATENCIÓN DEL PACIENTE");
            AddRule(() => TextRefMedMotivo, () => !string.IsNullOrWhiteSpace(TextRefMedMotivo), "CAPTURE EL MOTIVO");
            AddRule(() => IsFechaCitaValid, () => ((!isfechacitarequerida && !FechaCita.HasValue) || (FechaCita.HasValue && IsFechaCitaValid)), "FECHA DE LA CITA ES REQUERIDA!");
            RaisePropertyChanged("IsFechaCitaValid");
            RaisePropertyChanged("SelectedRefMedHospital");
            RaisePropertyChanged("SelectedRefMedTipoCitaValue");
            RaisePropertyChanged("TextRefMedMotivo");
        }

        private void clearValidacionesInterconsultaExterna()
        {
            RemoveRule("SelectedRefMedHospital");
            RemoveRule("SelectedRefMedTipoCitaValue");
            RemoveRule("TextRefMedMotivo");
            RemoveRule("IsFechaCitaValid");
            RaisePropertyChanged("IsFechaCitaValid");
            RaisePropertyChanged("SelectedRefMedHospital");
            RaisePropertyChanged("SelectedRefMedTipoCitaValue");
            RaisePropertyChanged("TextRefMedMotivo");
            //reglas de otro hospital
            RemoveRule("TextRefMedOtroHospital");
            RaisePropertyChanged("TextRefMedOtroHospital");
        }

        private void setValidacionesFechaCita()
        {
        }

        private void setValidacionesRefMedica()
        {
            AddRule(() => SelectedSolIntCentro, () => SelectedSolIntCentro != -1, "SELECCIONE UN CENTRO");
            AddRule(() => TextSolIntMotivo, () => !string.IsNullOrWhiteSpace(TextSolIntMotivo), "CAPTURE EL MOTIVO");
            RaisePropertyChanged("SelectedSolIntCentro");
            RaisePropertyChanged("TextSolIntMotivo");
        }

        private void clearValidacionesRefMedica()
        {
            RemoveRule("SelectedSolIntCentro");
            RemoveRule("TextSolIntMotivo");
            RaisePropertyChanged("SelectedSolIntCentro");
            RaisePropertyChanged("TextSolIntMotivo");
        }

        private void setValidacionesEspecialidad()
        {
            if (!FindRule("SelectedEspecialidad"))
            {
                AddRule(() => SelectedEspecialidad, () => SelectedEspecialidad != -1, "SELECCIONE UNA ESPECIALIDAD");
                RaisePropertyChanged("SelectedEspecialidad");
            }
        }

        private void clearValidacionesEspecialidad()
        {
            RemoveRule("SelectedEspecialidad");
            RaisePropertyChanged("SelectedEspecialidad");
        }

        private void setValidacionesServAux()
        {
            AddRule(() => IsServAuxSeleccionadosValid, () => IsServAuxSeleccionadosValid, "SELECCIONA UN SERVICIO AUXILIAR");
            RaisePropertyChanged("IsServAuxSeleccionadosValid");
        }

        private void clearValidacionesServAux()
        {
            RemoveRule("IsServAuxSeleccionadosValid");
            RaisePropertyChanged("IsServAuxSeleccionadosValid");
        }

        private void setValidacionesOtroHospital()
        {
            if (!FindRule("TextRefMedOtroHospital"))
            {
                AddRule(() => TextRefMedOtroHospital, () => TextRefMedOtroHospital != string.Empty, "CAPTURE EL HOSPITAL");
                RaisePropertyChanged("TextRefMedOtroHospital");
            }
        }

        private void clearValidacionesOtroHospital()
        {
            RemoveRule("TextRefMedOtroHospital");
            RaisePropertyChanged("TextRefMedOtroHospital");
        }
    }
}
