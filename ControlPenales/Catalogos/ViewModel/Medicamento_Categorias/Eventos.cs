using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoMedicamento_CategoriasViewModel
    {
        public ICommand CatalogoMedicamento_CategoriasLoading
        {
            get { return new DelegateCommand<CatalogoMedicamento_CategoriasView>(CatalogoMedicamento_CategoriasLoad); }
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get { return onClick ?? (onClick = new RelayCommand(ClickSwitch)); }
        }
    }
}
