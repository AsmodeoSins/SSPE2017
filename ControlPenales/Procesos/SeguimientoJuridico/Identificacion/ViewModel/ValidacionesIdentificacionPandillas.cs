
namespace ControlPenales
{
    partial class JuridicoIdentificacionViewModel
    {
        void setValidacionesIdentificacionPandillas()
        {
            base.ClearRules();
            base.AddRule(() => SelectedPandillaValue, () => SelectedPandillaValue > -1, "PANDILLA ES REQUERIDA!");

            OnPropertyChanged("SelectedPandillaValue");
        }
    }
}
