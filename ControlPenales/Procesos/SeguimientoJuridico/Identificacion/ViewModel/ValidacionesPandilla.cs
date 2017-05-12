
namespace ControlPenales
{
    partial class JuridicoIdentificacionViewModel
    {
        void setValidacionesPandilla()
        {
            base.ClearRules();
            base.AddRule(() => SelectedPandillaValue, () => SelectedPandillaValue > -1, "PANDILLA ES REQUERIDA!");

            OnPropertyChanged("SelectedPandillaValue");
        }
    }
}
