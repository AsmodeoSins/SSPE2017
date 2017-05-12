
namespace ControlPenales
{
    partial class UnidadReceptoraViewModel
    {
        private void ValidacionesUnidadReceptora()
        {
            base.ClearRules();
            base.AddRule(() => NombreUR, () => !string.IsNullOrEmpty(NombreUR), "NOMBRE ES REQUERIDA!");
            base.AddRule(() => DescripcionUR, () => !string.IsNullOrEmpty(DescripcionUR), "DESCRIPCIÓN ES REQUERIDA!");
            base.AddRule(() => EntidadUR, () => EntidadUR != -1, "ESTADO ES REQUERIDO!");
            base.AddRule(() => MunicipioUR, () => MunicipioUR != -1, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => ColoniaUR, () => MunicipioUR != -1, "MUNICIPIO ES REQUERIDO!");
            base.AddRule(() => CalleUR, () => !string.IsNullOrEmpty(CalleUR), "CALLE ES REQUERIDO!");
            base.AddRule(() => NoExteriorUR, () => !string.IsNullOrEmpty(NoExteriorUR), "NÚMERO EXTERIOR ES REQUERIDO!");
            base.AddRule(() => CPUR, () => !string.IsNullOrEmpty(CPUR), "CODIGO POSTAL ES REQUERIDO!");
            base.AddRule(() => TelefonoUR, () => !string.IsNullOrEmpty(TelefonoUR), "TELÉFONO ES REQUERIDO!");
            base.AddRule(() => EstatusUR, () => !string.IsNullOrEmpty(EstatusUR), "ESTATUS ES REQUERIDO!");
            OnPropertyChanged("NombreUR");
            OnPropertyChanged("DescripcionUR");
            OnPropertyChanged("EntidadUR");
            OnPropertyChanged("MunicipioUR");
            OnPropertyChanged("ColoniaUR");
            OnPropertyChanged("CalleUR");
            OnPropertyChanged("NoExteriorUR");
            OnPropertyChanged("CPUR");
            OnPropertyChanged("TelefonoUR");
            OnPropertyChanged("EstatusUR");
        }

        private void ValidacionResponsable() 
        {
            base.ClearRules();
            base.AddRule(() => NombreAlias, () => !string.IsNullOrEmpty(NombreAlias), "NOMBRE ES REQUERIDA!");
            base.AddRule(() => PaternoAlias, () => !string.IsNullOrEmpty(PaternoAlias), "APELLIDO PATERNO ES REQUERIDA!");
            base.AddRule(() => MaternoAlias, () => !string.IsNullOrEmpty(MaternoAlias), "APELLIDO MATERNO ES REQUERIDA!");
            OnPropertyChanged("NombreAlias");
            OnPropertyChanged("PaternoAlias");
            OnPropertyChanged("MaternoAlias");
        }
    }
}