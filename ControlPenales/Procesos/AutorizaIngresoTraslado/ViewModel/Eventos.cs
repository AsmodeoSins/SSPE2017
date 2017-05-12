using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    partial class AutorizaIngresoTrasladoViewModel
    {
        public ICommand AutorizaIngresoTrasladoOnLoading
        {
            get { return new DelegateCommand<TrasladoMasivoView>(AutorizaIngresoTrasladoLoad); }
        }

        private ICommand cmdCambioPropiedad;
        public ICommand CmdCambioPropiedad
        {
            get { return cmdCambioPropiedad??(cmdCambioPropiedad=new RelayCommand(CambioPropiedad));}
        }

        private ICommand onClick;
        public ICommand OnClick
        {
            get { return onClick??(onClick=new RelayCommand(ClickSwitch));}
        }

    }
}
