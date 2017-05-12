using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace ControlPenales
{
    public class EscapePopUpHelpper : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            if (((KeyEventArgs)value).Key != Key.Escape)
                return null;
            return parameter;
        }
    }
}
