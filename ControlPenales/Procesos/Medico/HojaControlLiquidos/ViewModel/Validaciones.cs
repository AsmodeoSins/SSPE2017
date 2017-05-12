namespace ControlPenales
{
    public partial class HojaControlLiquidosViewModel
    {
        void ValidacionesSignosVitales()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => Arterial1, () => !string.IsNullOrEmpty(Arterial1), "TENSIÓN ARTERIAL ES REQUERIDA");
                base.AddRule(() => Arterial2, () => !string.IsNullOrEmpty(Arterial2), "TENSIÓN ARTERIAL ES REQUERIDA");
                base.AddRule(() => FrecuenciaCardiaca, () => !string.IsNullOrEmpty(FrecuenciaCardiaca), "FRECUENCIA CARDIACA ES REQUERIDA");
                base.AddRule(() => FrecuenciaRespiratoria, () => !string.IsNullOrEmpty(FrecuenciaRespiratoria), "FRECUENCIA RESPIRATORIA ES REQUERIDA");
                base.AddRule(() => Temperatura, () => !string.IsNullOrEmpty(Temperatura), "TEMPERATURA ES REQUERIDA");
                base.AddRule(() => Glucemia, () => !string.IsNullOrEmpty(Glucemia), "GLUCEMIA ES REQUERIDA");

                OnPropertyChanged("Arterial1");
                OnPropertyChanged("Glucemia");
                OnPropertyChanged("Arterial2");
                OnPropertyChanged("FrecuenciaCardiaca");
                OnPropertyChanged("FrecuenciaRespiratoria");
                OnPropertyChanged("Temperatura");
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        void ValidacionesLiquidosIngreso() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => TxtCantidad, () => TxtCantidad.HasValue, "CANTIDAD ES REQUERIDA");
                base.AddRule(() => SelectedLiqIngreso, () => SelectedLiqIngreso.HasValue ? SelectedLiqIngreso.Value != -1 : false, "LÍQUIDO ES REQUERIDO");

                OnPropertyChanged("TxtCantidad");
                OnPropertyChanged("SelectedLiqIngreso");
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }

        void LimpiaValidacionesLiquidosIngreso() 
        {
            try
            {
                base.RemoveRule("TxtCantidad");
                base.RemoveRule("SelectedLiqIngreso");
                OnPropertyChanged("TxtCantidad");
                OnPropertyChanged("SelectedLiqIngreso");
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }
    }
}