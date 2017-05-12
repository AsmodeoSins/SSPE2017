using System;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        void setValidacionesDatosGrupoFamiliar()
        {
            base.ClearRules();        
    
        }

        void setValidacionesDatosGrupoFamiliarPop()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => GrupoFamiliar, () => GrupoFamiliar.HasValue ? GrupoFamiliar.Value != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => PaternoGrupoFamiliar, () => !string.IsNullOrEmpty(PaternoGrupoFamiliar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("PaternoGrupoFamiliar");
                base.AddRule(() => MaternoGrupoFamiliar, () => !string.IsNullOrEmpty(MaternoGrupoFamiliar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("MaternoGrupoFamiliar");
                base.AddRule(() => NombreGrupoFamiliar, () => !string.IsNullOrEmpty(NombreGrupoFamiliar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("NombreGrupoFamiliar");
                base.AddRule(() => EdadGrupoFamiliar, () => EdadGrupoFamiliar.HasValue ? EdadGrupoFamiliar.Value > 0 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => SelectedRelacionGrupoFamiliar, () => SelectedRelacionGrupoFamiliar != null ? SelectedRelacionGrupoFamiliar.ID_TIPO_REFERENCIA != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("SelectedRelacionGrupoFamiliar");
                base.AddRule(() => DomicilioGrupoFamiliar, () => !string.IsNullOrEmpty(DomicilioGrupoFamiliar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => SelectedOcupacionGrupoFamiliar, () => SelectedOcupacionGrupoFamiliar != null ? SelectedOcupacionGrupoFamiliar.ID_OCUPACION != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => SelectedEdoCivilGrupoFamiliar, () => SelectedEdoCivilGrupoFamiliar != null ? SelectedEdoCivilGrupoFamiliar.ID_ESTADO_CIVIL != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                OnPropertyChanged("GrupoFamiliar");
                OnPropertyChanged("PaternoGrupoFamiliar");
                OnPropertyChanged("MaternoGrupoFamiliar");
                OnPropertyChanged("NombreGrupoFamiliar");
                OnPropertyChanged("EdadGrupoFamiliar");
                OnPropertyChanged("SelectedRelacionGrupoFamiliar");
                OnPropertyChanged("DomicilioGrupoFamiliar");
                OnPropertyChanged("SelectedOcupacionGrupoFamiliar");
                OnPropertyChanged("SelectedEdoCivilGrupoFamiliar");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en datos grupo familiar", ex);
            }
        }
    }
}
