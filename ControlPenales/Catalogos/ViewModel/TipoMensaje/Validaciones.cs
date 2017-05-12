
namespace ControlPenales
{
    partial class CatalogoTipoMensajeViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => Prioridad, () => Prioridad.HasValue ? Prioridad != -1 ? true : false : false, "PRIORIDAD ES REQUERIDA!");
            base.AddRule(() => Encabezado, () => !string.IsNullOrEmpty(Encabezado), "ENCABEZADO ES REQUERIDO!");
            base.AddRule(() => Contenido, () => !string.IsNullOrEmpty(Contenido), "CONTENIDO ES REQUERIDO!");
            base.AddRule(() => SelectedEstatus, () => SelectedEstatus!= null , "ESTATUS ES REQUERIDA!");
          
            OnPropertyChanged("Descripcion");
            OnPropertyChanged("Prioridad");
            OnPropertyChanged("Encabezado");
            OnPropertyChanged("Contenido");
            OnPropertyChanged("SelectedEstatus");
        }
    }
}