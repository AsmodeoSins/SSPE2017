using System.ComponentModel;

namespace ControlPenales
{
    partial class EscalaRiesgoViewModel
    {
        void Validaciones()
        {
            base.ClearRules();
            base.AddRule(() => Lugar, () => !string.IsNullOrEmpty(Lugar), "LUGAR ES REQUERIDO!");
            base.AddRule(() => Fecha, () => Fecha.HasValue, "FECHA ES REQUERIDA!");
            base.AddRule(() => Paterno, () => !string.IsNullOrEmpty(Paterno), "APELLIDO PATERNO ES REQUERIDO!");
            base.AddRule(() => Materno, () => !string.IsNullOrEmpty(Materno), "APELLIDO MATERNO ES REQUERIDO!");
            base.AddRule(() => Nombre, () => !string.IsNullOrEmpty(Nombre), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => NUC, () => !string.IsNullOrEmpty(NUC), "NUC ES REQUERIDO!");
            OnPropertyChanged("Lugar");
            OnPropertyChanged("Fecha");
            OnPropertyChanged("Paterno");
            OnPropertyChanged("Materno");
            OnPropertyChanged("Nombre");
            OnPropertyChanged("NUC");
        }
    }
}