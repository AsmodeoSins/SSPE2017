using ControlPenales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CatalogoUnidadMedidaViewModel
    {
        private ICommand _buscarClick;
        public ICommand BuscarClick
        {
            get { return _buscarClick ?? (_buscarClick = new RelayCommand(ClickEnter)); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        public ICommand CatalogoUnidadMedidaLoading
        {
            get { return new DelegateCommand<CatalogoUnidadMedidaView>(TipoUnidadMedidaLoad); }
        }
    }
}
