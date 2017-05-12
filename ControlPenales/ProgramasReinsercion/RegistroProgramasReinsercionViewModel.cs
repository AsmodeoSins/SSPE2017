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
    class RegistroProgramasReinsercionViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public RegistroProgramasReinsercionViewModel()
        {
            SuspendidosTabName = "SUSPENDIDOS Y CANCELADOS";
            CanceladosVisible = false;
        }
        #endregion

        #region variables
        private bool canceladosVisible;
        public bool CanceladosVisible
        {
            get { return canceladosVisible; }
            set { canceladosVisible = value; OnPropertyChanged("CanceladosVisible"); }
        }
        private string suspendidosTabName;
        public string SuspendidosTabName
        {
            get { return suspendidosTabName; }
            set { suspendidosTabName = value; OnPropertyChanged("SuspendidosTabName"); }
        }
        public string Name
        {
            get
            {
                return "registro_programas_reinsercion";
            }
        }
        #endregion

        void IPageViewModel.inicializa()
        { }
    }
}
