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
using ControlPenales.Clases;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ConsultaUnificadaViewModel
    {
        #region Buscar
        private string fNombre;
        public string FNombre
        {
            get { return fNombre; }
            set { fNombre = value; OnPropertyChanged("FNombre"); }
        }
        
        private string fNombreAux;

        private short fClasificacion = -1;
        public short FClasificacion
        {
            get { return fClasificacion; }
            set { fClasificacion = value; OnPropertyChanged("FClasificacion"); }
        }

        private short fClasificacionAux = -1;

        private List<CLASIFICACION_DOCUMENTO> lstClasificacion;
        public List<CLASIFICACION_DOCUMENTO> LstClasificacion
        {
            get { return lstClasificacion; }
            set { lstClasificacion = value; OnPropertyChanged("LstClasificacion"); }
        }

        private int Pagina = 1;
        private bool SeguirCargando;

        #endregion

        #region Grid
        private RangeEnabledObservableCollection<CONSULTA_UNIFICADA> lstConsultaUnificada;
        public RangeEnabledObservableCollection<CONSULTA_UNIFICADA> LstConsultaUnificada
        {
            get { return lstConsultaUnificada; }
            set { lstConsultaUnificada = value; OnPropertyChanged("LstConsultaUnificada"); }
        }

        private CONSULTA_UNIFICADA selectedConsultaUnificada;
        public CONSULTA_UNIFICADA SelectedConsultaUnificada
        {
            get { return selectedConsultaUnificada; }
            set { selectedConsultaUnificada = value; OnPropertyChanged("SelectedConsultaUnificada"); }
        }
        #endregion

        #region Pantalla
        private Visibility dataGridVacio = Visibility.Collapsed;
        public Visibility DataGridVacio
        {
            get { return dataGridVacio; }
            set { dataGridVacio = value; OnPropertyChanged("DataGridVacio"); }
        }
        #endregion

        #region Menu
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
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
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
