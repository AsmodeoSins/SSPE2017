using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MVVMShared.Helpers
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
