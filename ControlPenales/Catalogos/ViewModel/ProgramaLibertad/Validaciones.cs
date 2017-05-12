
namespace ControlPenales
{
    partial class ProgramaLibertadViewModel
    {
        private void ValidacionesProgramaLibertad()
        {
            base.ClearRules();
            base.AddRule(() => DescrPL, () => !string.IsNullOrEmpty(DescrPL), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => ObjetivoPL, () => !string.IsNullOrEmpty(ObjetivoPL), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => EstatusPL, () => !string.IsNullOrEmpty(EstatusPL), "ESTATUS ES REQUERIDO!");
            OnPropertyChanged("DescrPL");
            OnPropertyChanged("ObjetivoPL");
            OnPropertyChanged("EstatusPL");
        }


        private void ValidacionesActividadLibertad()
        {
            base.ClearRules();
            base.AddRule(() => DescrAP, () => !string.IsNullOrEmpty(DescrAP), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => EstatusAP, () => !string.IsNullOrEmpty(EstatusAP), "ESTATUS ES REQUERIDO!");
            OnPropertyChanged("DescrAP");
            OnPropertyChanged("EstatusAP");
        }

    }
}