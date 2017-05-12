namespace ControlPenales
{
    partial class CatalogoEspecialistasViewModel
    {
        void setValidaciones() 
        {
            try
            {
                base.ClearRules();
                AddRule(() => SelectedEspecialidadEdicion, () => SelectedEspecialidadEdicion != null && SelectedEspecialidadEdicion != -1, "SELECCIONE UNA ESPECIALIDAD!");
                OnPropertyChanged("SelectedEspecialidadEdicion");
                AddRule(() => SelectedEstatus, () => SelectedEstatus != null , "SELECCIONE UN ESTATUS!");
                OnPropertyChanged("SelectedEstatus");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        void ValidacionesEspecialistas() 
        {
            try
            {
                base.ClearRules();
                base.RemoveRule("TextNombre");
                base.RemoveRule("TextPaterno");
                base.RemoveRule("TextMaterno");
                AddRule(() => TextNombre, () => !string.IsNullOrEmpty(TextNombre), "NOMBRE DEL ESPECIALISTA ES REQUERIDO!");
                OnPropertyChanged("TextNombre");
                AddRule(() => TextPaterno, () => !string.IsNullOrEmpty(TextPaterno), "APELLIDO PATERNO DEL ESPECIALISTA ES REQUERIDO!");
                OnPropertyChanged("TextPaterno");
                AddRule(() => TextMaterno, () => !string.IsNullOrEmpty(TextMaterno), "APELLIDO MATERNO ESPECIALISTA ES REQUERIDO!");
                OnPropertyChanged("TextMaterno");
            }
            catch (System.Exception exc)
            {
                
                throw;
            }
        }
    }
}