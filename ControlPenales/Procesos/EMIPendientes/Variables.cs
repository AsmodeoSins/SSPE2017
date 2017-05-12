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
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class EMIPendientesViewModel : ValidationViewModelBase
    {
        #region Buscar
        private short opcion = 0;
        public short Opcion
        {
            get { return opcion; }
            set { opcion = value;
                OnPropertyChanged("Opcion"); }
        }

        private short filtro = 0;

        private RangeEnabledObservableCollection<INGRESO> lstResultado;
        public RangeEnabledObservableCollection<INGRESO> LstResultado
        {
            get { return lstResultado; }
            set { lstResultado = value;
                OnPropertyChanged("LstResultado"); }
        }

        private Visibility resultadosVisible = Visibility.Collapsed;
        public Visibility ResultadosVisible
        {
            get { return resultadosVisible; }
            set { resultadosVisible = value; OnPropertyChanged("ResultadosVisible"); }
        }

        private int Pagina { get; set; }
        
        private bool SeguirCargando { get; set; }

        private string total;
        public string Total
        {
            get { return total; }
            set { total = value; OnPropertyChanged("Total"); }
        }
        #endregion

        #region Pantalla
        private bool menuGuardarEnabled = false;
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

        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        #endregion

        #region Permisos
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
