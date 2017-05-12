using System;
using System.ComponentModel;

namespace ControlPenales
{
    partial class EstatusAdministrativoViewModel
    {
        void setValidacionesDatosIngreso()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => FechaCeresoIngreso, () => FechaCeresoIngreso > System.DateTime.Parse("01/01/1950"), "TIPO DE INGRESO ES REQUERIDO!");
                base.AddRule(() => SelectTipoIngreso, () => SelectTipoIngreso >= 0, "TIPO DE INGRESO ES REQUERIDO!");
                base.AddRule(() => SelectClasificacionJuridica, () => !string.IsNullOrEmpty(SelectClasificacionJuridica), "CLASIFICACIÓN JURIDICA ES REQUERIDA!");
                base.AddRule(() => SelectEstatusAdministrativo, () => SelectEstatusAdministrativo.HasValue ? SelectEstatusAdministrativo > 0 : false, "ESTATUS ADMINISTRATIVO ES REQUERIDO!");
                //base.AddRule(() => IngresoDelito, () => IngresoDelito.HasValue ? IngresoDelito.Value > 0 : false, "TIPO DE DELITO ES REQUERIDO!");
                base.AddRule(() => DescrDelito, () => !string.IsNullOrEmpty(DescrDelito), "DELITO ES REQUERIDO ES REQUERIDO!");
                base.AddRule(() => SelectedCama, () => SelectedCama != null, "UBICACIÓN ES REQUERIDO!");
                base.AddRule(() => SelectTipoAutoridadInterna, () => SelectTipoAutoridadInterna.HasValue ? SelectTipoAutoridadInterna.Value >= 0 : false, "TIPO DE AUTORIDAD QUE INTERNA ES REQUERIDA!");
                base.AddRule(() => SelectTipoSeguridad, () => SelectTipoSeguridad != null && SelectTipoSeguridad != "", "TIPO DE SEGURIDAD ES REQUERIDO!");
                base.AddRule(() => SelectTipoDisposicion, () => SelectTipoDisposicion.HasValue ? SelectTipoDisposicion.Value >= 0 : false, "TIPO DE DISPOSICIÓN ES REQUERIDO!");
                base.AddRule(() => TextNumeroOficio, () => TextNumeroOficio != null && TextNumeroOficio != "", "NUMERO DE OFICIO ES REQUERIDO!");
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear validaciones.", ex);
            }
        }
    }
}
