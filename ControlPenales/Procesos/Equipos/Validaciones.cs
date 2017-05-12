
using System.Text.RegularExpressions;
namespace ControlPenales
{
    partial class EquiposViewModel
    {
        private void setValidationRules()
        {
            base.ClearRules();
            Regex rgxIP = new Regex(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$");
            base.AddRule(() => EIP, () => !string.IsNullOrEmpty(EIP) ? rgxIP.IsMatch(EIP) ? true : false : false, "ESTATUS ES REQUERIDO O EL FORMATO ES INCORRECTO!");
            Regex rgxMAC = new Regex(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
            base.AddRule(() => EMacAddress, () => !string.IsNullOrEmpty(EMacAddress) ? rgxMAC.IsMatch(EMacAddress) ? true : false : false, "MAC ADDRESS ES REQUERIDO O EL FORMATO ES INCORRECTO!");
            base.AddRule(() => EDescripcion, () => !string.IsNullOrEmpty(EDescripcion), "DESCRIPCIÓN ES REQUERIDO!");
            base.AddRule(() => ECentro, () => ECentro != -1, "CENTRO ES REQUERIDO!");
            base.AddRule(() => ETipoEquipo, () => ETipoEquipo != -1, "TIPO DE EQUIPO ES REQUERIDO!");
            OnPropertyChanged("ECentro");
            OnPropertyChanged("EIP");
            OnPropertyChanged("EMacAddress");
            OnPropertyChanged("EDescripcion");
            OnPropertyChanged("ETipoEquipo");
        }
    }
}