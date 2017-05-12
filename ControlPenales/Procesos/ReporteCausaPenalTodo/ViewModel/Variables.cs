using ControlPenales;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteCausaPenalTodoViewModel
    {
        #region Filtros
        private bool comun;
        public bool Comun
        {
            get { return comun; }
            set { comun = value;
                OnPropertyChanged("Comun"); }
        }

        private bool federal;
        public bool Federal
        {
            get { return federal; }
            set { federal = value;
                OnPropertyChanged("Federal"); }
        }

        private bool ambos = true;
        public bool Ambos
        {
            get { return ambos; }
            set { ambos = value;
                OnPropertyChanged("Ambos"); }
        }
        #endregion
        
        #region Panatalla
        private Visibility reportViewerVisible = Visibility.Collapsed;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        private WindowsFormsHost repositorio;
        public WindowsFormsHost Repositorio
        {
            get { return repositorio; }
            set { repositorio = value; OnPropertyChanged("Repositorio"); }
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
