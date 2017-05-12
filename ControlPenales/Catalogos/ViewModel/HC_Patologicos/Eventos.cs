using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class Catalogo_PatologicosViewModel
    {
        public ICommand CatalogoPatologicosLoading
        {
            get { return new DelegateCommand<Catalogo_PatologicosView>(CatalogoPatologicosLoad); }
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get { return onClick ?? (onClick = new RelayCommand(ClickSwitch)); }
        }
    }
}
