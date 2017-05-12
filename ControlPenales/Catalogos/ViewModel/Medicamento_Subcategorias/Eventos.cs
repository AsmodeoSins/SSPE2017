using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoMedicamento_SubcategoriasViewModel
    {
        public ICommand CatalogoMedicamento_SubcategoriasLoading
        {
            get { return new DelegateCommand<CatalogoMedicamento_SubcategoriasView>(CatalogoMedicamento_CategoriasLoad); }
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get { return onClick ?? (onClick = new RelayCommand(ClickSwitch)); }
        }
    }
}
