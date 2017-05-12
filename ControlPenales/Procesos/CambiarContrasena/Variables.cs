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
using SSP.Servidor;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;

namespace ControlPenales
{
    partial class CambiarContrasenaViewModel
    {
        #region Passwords
        private string pPassword;
        public string PPassword
        {
            get { return pPassword; }
            set { pPassword = value; OnPropertyValidateChanged("PPassword"); }
        }

        private string pPasswordNuevo;
        public string PPasswordNuevo
        {
            get { return pPasswordNuevo; }
            set { pPasswordNuevo = value; OnPropertyValidateChanged("PPasswordNuevo"); }
        }

        private string pPasswordNuevoRepetir;
        public string PPasswordNuevoRepetir
        {
            get { return pPasswordNuevoRepetir; }
            set { pPasswordNuevoRepetir = value; OnPropertyValidateChanged("PPasswordNuevoRepetir"); }
        }
        #endregion

        #region Menu
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }

        private bool menuGuardarEnabled = true;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }

        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        #region Permisos
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion 
    }
}
