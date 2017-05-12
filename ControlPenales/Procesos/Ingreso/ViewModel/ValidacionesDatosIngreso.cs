using System.ComponentModel;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        void setValidacionesDatosIngreso()
        {
            base.ClearRules();
            OnPropertyChanged("");
            base.AddRule(() => FechaCeresoIngreso, () => FechaCeresoIngreso != System.DateTime.Parse("01/01/0001"), "TIPO DE INGRESO ES REQUERIDO!");
            base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso.HasValue ? SelectTipoIngreso.Value >= 0 : false, "TIPO DE INGRESO ES REQUERIDO!");
            base.AddRule(() => SelectClasificacionJuridica, () => !string.IsNullOrEmpty(SelectClasificacionJuridica), "CLASIFICACION JURIDICA ES REQUERIDA!");
            base.AddRule(() => SelectEstatusAdministrativo, () => SelectEstatusAdministrativo.HasValue ? SelectEstatusAdministrativo.Value > 0 : false, "ESTATUS ADMINISTRATIVO ES REQUERIDO!");
            //base.AddRule(() => IngresoDelito, () => IngresoDelito > 0, "TIPO DE DELITO ES REQUERIDO!");
            base.AddRule(() => DescrDelito, () => !string.IsNullOrEmpty(DescrDelito), "DELITO ES REQUERIDO!");
            base.AddRule(() => SelectedCama, () => SelectedCama != null, "UBICACION ES REQUERIDO!");
            base.AddRule(() => SelectTipoAutoridadInterna, () => SelectTipoAutoridadInterna.HasValue ? SelectTipoAutoridadInterna.Value >= 0 : false, "TIPO DE AUTORIDAD QUE INTERNA ES REQUERIDA!");
            base.AddRule(() => SelectTipoSeguridad, () => !string.IsNullOrEmpty(SelectTipoSeguridad), "TIPO DE SEGURIDAD ES REQUERIDO!");
            base.AddRule(() => SelectTipoDisposicion, () => SelectTipoDisposicion.HasValue ? SelectTipoDisposicion.Value >= 0 : false, "TIPO DE DISPOSICION ES REQUERIDO!");
            base.AddRule(() => TextNumeroOficio, () => !string.IsNullOrEmpty(TextNumeroOficio), "NUMERO DE OFICIO ES REQUERIDO!");
            OnPropertyChanged("FechaCeresoIngreso");
            OnPropertyChanged("SelectTipoIngreso");
            OnPropertyChanged("SelectClasificacionJuridica");
            OnPropertyChanged("SelectEstatusAdministrativo");
            OnPropertyChanged("DescrDelito");
            OnPropertyChanged("SelectedCama");
            OnPropertyChanged("SelectTipoAutoridadInterna");
            OnPropertyChanged("SelectTipoSeguridad");
            OnPropertyChanged("SelectTipoDisposicion");
            OnPropertyChanged("TextNumeroOficio");
        }
    }
}