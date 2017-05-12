using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    class ReporteViewModel
    {

        //VARIABLES

        //EVENTOS
        public ICommand PageLoading
        {
            get { return new DelegateCommand<ReporteView>(PaginaLoad); }
        }

        //FUNCIONES
        private void PaginaLoad(ReporteView Window = null)
        {
            var metro = Application.Current.Windows[5] as MetroWindow;
            var x = ((System.Windows.Controls.Button)metro.FindName("myButton"));
        }
    }
}
