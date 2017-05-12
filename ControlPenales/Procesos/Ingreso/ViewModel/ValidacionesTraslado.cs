using System.ComponentModel;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        void AgregarValidacionesTraslado()
        {
           
            base.AddRule(() => DTMotivo, () => DTMotivo != -1, "MOTIVO ES REQUERIDO!");
            base.AddRule(() => DTJustificacion, () => !string.IsNullOrWhiteSpace(DTJustificacion), "JUSTIFICACION ES REQUERIDO!");
            base.AddRule(() => DTCentroOrigen, () => DTCentroOrigen != -1, "CENTRO ORIGEN ES REQUERIDO!");
            base.AddRule(() => DTNoOficio, () => !string.IsNullOrWhiteSpace(DTNoOficio), "OFICIO AUTORIZACION ES REQUERIDO!");

            RaisePropertyChanged("DTMotivo");
            RaisePropertyChanged("DTJustificacion");
            RaisePropertyChanged("DTCentroOrigen");
            RaisePropertyChanged("DTNoOficio");
        }

        void RemoveValidacionesTraslado()
        {
            base.RemoveRule("DTMotivo");
            base.RemoveRule("DTJustificacion");
            base.RemoveRule("DTCentroOrigen");
            base.RemoveRule("DTNoOficio");
            
            RaisePropertyChanged("DTMotivo");
            RaisePropertyChanged("DTJustificacion");
            RaisePropertyChanged("DTCentroOrigen");
            RaisePropertyChanged("DTNoOficio");
        }
    }
}