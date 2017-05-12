
namespace ControlPenales
{
    partial class EstatusAdministrativoViewModel
    {
        void setValidacionesIdentificacionPandillas()
        {
            base.ClearRules();
            base.AddRule(() => SelectedPandillaValue, () => SelectedPandillaValue > -1, "PANDILLA ES REQUERIDA!");
        }
    }
}
