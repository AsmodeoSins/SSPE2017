using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class BitacoraIngresosEgresosHospitalizacionViewModel
    {

        private void setValidacionIngreso()
        {
            base.ClearRules();
            base.AddRule(() => SelectedTipoHospitalizacionValue, () => SelectedTipoHospitalizacionValue!=SELECCIONE, "Debe seleccionar un tipo de hospitalización.");
            OnPropertyChanged("SelectedTipoHospitalizacionValue");
            base.AddRule(() => SelectedCamaHospitalValue, () => SelectedCamaHospitalValue != SELECCIONE, "Debe seleccionar una cama antes de confirmar una hospitalización.");
            OnPropertyChanged("SelectedCamaHospitalValue");
            base.AddRule(() => HospitalizacionFecha, () => HospitalizacionFecha.HasValue, "Debe seleccionar una fecha de hospitalización.");
            OnPropertyChanged("HospitalizacionFecha");
        }


        private void ValidacionHoras()
        {
            base.ClearRules();
            base.AddRule(() => AFechaValid, () => AFechaValid, "FECHA ES REQUERIDA");
            base.AddRule(() => AHoraI, () => AHoraI.HasValue, "HORA INICIO ES REQUERIDA");
            base.AddRule(() => AHoraF, () => AHoraF.HasValue, "HORA FIN REQUERIDA");
            base.AddRule(() => AHorasValid, () => AHorasValid, "LA HORA DE INICIO DEBE SER MAYOR A LA HORA FIN");

            OnPropertyChanged("AFechaValid");
            OnPropertyChanged("AHoraI");
            OnPropertyChanged("AHoraF");
            OnPropertyChanged("AHorasValid");
        }
        
        private void setValidacionAlta()
        {
            base.ClearRules();
            base.AddRule(() => SelectedMotivoEgresoMedicoValue, () => SelectedMotivoEgresoMedicoValue!=-1, "SELECCIONAR UN MOTIVO DE ALTA ES REQUERIDO");
            OnPropertyChanged("SelectedMotivoEgresoMedicoValue");
            base.AddRule(() => IsFechaEgresoValida, () => IsFechaEgresoValida, "CAPTURAR FECHA DE ALTA ES REQUERIDO");
            OnPropertyChanged("IsFechaEgresoValida");
        }
    }
}
