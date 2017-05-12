namespace ControlPenales
{
    public partial class ReportePoblacionActivaCierreViewModel
    {
        public void SetValidaciones()
        {
            base.ClearRules();
            base.AddRule(() => SelectedFechaInicial, () => SelectedFechaInicial.HasValue, "FECHA DE INICIO ES REQUERIDA!");
            base.AddRule(() => SelectedFechaFinal, () => SelectedFechaFinal.HasValue, "FECHA FINAL ES REQUERIDA!");
            OnPropertyChanged("SelectedFechaInicial");
            OnPropertyChanged("SelectedFechaFinal");
        }

    }
}