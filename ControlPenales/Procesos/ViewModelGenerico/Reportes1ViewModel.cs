using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class Reportes1ViewModel
    {
        public Reportes1ViewModel()
        {
        }

        public System.Windows.Input.ICommand Reportes1Loading
        {
            get
            {
                return new DelegateCommand<Reportes1>(DatosIdentificacionLoad);
            }
        }
        private void DatosIdentificacionLoad(Reportes1 Window = null)
        {
            var metro = System.Windows.Application.Current.Windows[5] as MahApps.Metro.Controls.MetroWindow;

            var boton = ((System.Windows.Controls.Button)metro.FindName("myBoton"));
        }
    }
}
