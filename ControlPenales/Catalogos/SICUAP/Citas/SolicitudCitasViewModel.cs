using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ControlPenales
{
    class SolicitudCitasViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public SolicitudCitasViewModel() 
        {
        }
        #endregion

        #region variables
        public string Name
        {
            get
            {
                return "solicitud_cita";
            }
        }
        #endregion
        void IPageViewModel.inicializa()
        { }
    }
}
