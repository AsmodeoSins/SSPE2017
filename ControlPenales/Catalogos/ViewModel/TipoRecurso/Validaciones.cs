
namespace ControlPenales
{
    partial class TipoRecursoViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => Tipo, () => !string.IsNullOrEmpty(Tipo), "TIPO ES REQUERIDO!");
            OnPropertyChanged("Descripcion");
            OnPropertyChanged("Tipo");
        }
    }
}