namespace ControlPenales
{
    partial class ResultadoTratamientoServAuxViewModel
    {
        void ValidacionesAgregarArchivos() 
        {
            base.ClearRules();
            base.AddRule(() => SelectedTipoServAuxEdicion, () => SelectedTipoServAuxEdicion != decimal.Zero ? SelectedTipoServAuxEdicion != -1 : false, "TIPO DE SERVICIO ES REQUERIDO!");
            OnPropertyChanged("SelectedTipoServAuxEdicion");
            base.AddRule(() => SelectedSubTipoServAuxEdicion, () => SelectedSubTipoServAuxEdicion != decimal.Zero ? SelectedSubTipoServAuxEdicion != -1 : false, "SUB TIPO DE SERVICIO ES REQUERIDO!");
            OnPropertyChanged("SelectedSubTipoServAuxEdicion");
            base.AddRule(() => SelectedDiagnosticoEdicion, () => SelectedDiagnosticoEdicion != decimal.Zero ? SelectedDiagnosticoEdicion != -1 : false, "DIAGNÓSTICO ES REQUERIDO!");
            OnPropertyChanged("SelectedDiagnosticoEdicion");
        }
    }
}