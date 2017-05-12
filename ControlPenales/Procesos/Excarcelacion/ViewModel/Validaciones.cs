using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class ExcarcelacionViewModel
    {

         void setValidacionesExcarcelacion()
        {
            base.ClearRules();
            base.AddRule(() => SelectedExc_TipoValue, () => SelectedExc_TipoValue != 0, "TIPO DE EXCARCELACION ES REQUERIDO!");
            base.AddRule(() => Excarcelacion_Fecha, () => Excarcelacion_Fecha.HasValue, "FECHA Y HORA DE EXCARCELACION ES REQUERIDA!");
            base.AddRule(() => Observaciones, () => !string.IsNullOrWhiteSpace(Observaciones), "OBSERVACIONES ES REQUERIDO!");
            base.AddRule(() => SelectIngreso, () => SelectIngreso != null, "INGRESO DEL IMPUTADO ES REQUERIDO!");
            base.AddRule(() => ListaExcarcelacionDestinos, () => ListaExcarcelacionDestinos != null && ListaExcarcelacionDestinos.Count>0, "ES REQUERIDO POR LO MENOS UN DESTINO!");
            OnPropertyChanged("SelectedExc_TipoValue");
            OnPropertyChanged("Excarcelacion_Fecha");
            OnPropertyChanged("Observaciones");
            OnPropertyChanged("SelectIngreso");
            OnPropertyChanged("ListaExcarcelacionDestinos");
        }


        void setValidacionesJuridicas()
        {
            base.ClearRules();
            base.AddRule(() => SelectedPaisValue, () => SelectedPaisValue != 0, "PAIS ES REQUERIDO!");
            OnPropertyChanged("SelectedPaisValue");
            base.AddRule(() => SelectedEstadoValue, () => SelectedEstadoValue != 0, "ESTADO ES REQUERIDO!");
            OnPropertyChanged("SelectedEstadoValue");
            base.AddRule(() => SelectedMunicipioValue, () => SelectedMunicipioValue != 0, "MUNICIPIO ES REQUERIDO!");
            OnPropertyChanged("SelectedMunicipioValue");
            base.AddRule(() => SelectedFueroValue, () => SelectedFueroValue != "0", "FUERO ES REQUERIDO!");
            OnPropertyChanged("SelectedFueroValue");
            base.AddRule(() => SelectedJuzgadoValue, () => SelectedJuzgadoValue != 0, "JUZGADO ES REQUERIDO!");
            OnPropertyChanged("SelectedJuzgadoValue");
            base.AddRule(() => IsDocumentoAgregado, () => IsDocumentoAgregado, "DOCUMENTO ES REQUERIDO!");
            OnPropertyChanged("IsDocumentoAgregado");
            base.AddRule(() => Folio_Doc, () => !string.IsNullOrWhiteSpace(Folio_Doc), "FOLIO DE DOCUMENTO ES REQUERIDO!");
            OnPropertyChanged("Folio_Doc");
            //base.AddRule(() => CP_Excarcelacion_Destino, () => CP_Excarcelacion_Destino!=null, "CAUSA PENAL ES REQUERIDA!");
            //OnPropertyChanged("CP_Excarcelacion_Destino");    
        }

        void setValidacionesMedicas()
        {
            base.ClearRules();
            base.AddRule(() => SelectedHospitalValue, () => SelectedHospitalValue != 0, "HOSPITAL ES REQUERIDO!");
            base.AddRule(() => IsDocumentoAgregado, () => IsDocumentoAgregado, "DOCUMENTO ES REQUERIDO!");
            base.AddRule(() => Folio_Doc, () => !string.IsNullOrWhiteSpace(Folio_Doc), "FOLIO DE DOCUMENTO ES REQUERIDO!");
            OnPropertyChanged("SelectedHospitalValue");
            OnPropertyChanged("IsDocumentoAgregado");
            OnPropertyChanged("Folio_Doc");
        }

        void setValidacionesExcarcelacion_Cancela_Motivo()
        {
            base.ClearRules();
            base.AddRule(() => SelectedCancelacion_MotivoValue, () => selectedCancelacion_MotivoValue != 0, "MOTIVO DE LA CANCELACIÓN ES REQUERIDO!");
            base.AddRule(() => Cancelacion_Observacion, () =>  !string.IsNullOrWhiteSpace(Cancelacion_Observacion), "OBSERVACIÓN DE LA CANCELACIÓN ES REQUERIDO!");
            OnPropertyChanged("SelectedCancelacion_MotivoValue");
            OnPropertyChanged("Cancelacion_Observacion");
        }

    }
}
