using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteProgramaActividadesViewModel
    {
        #region Filtros
        private short fReporte = 1;
        public short FReporte
        {
            get { return fReporte; }
            set
            {
                fReporte = value;
                if (value == 1)
                {
                    FiltrosVisible = Visibility.Visible;
                    FTipo = 1;
                }
                else
                    FiltrosVisible = Visibility.Collapsed;
                OnPropertyChanged("FReporte");
            }
        }

        private DateTime? selectedFechaInicial;
        public DateTime? SelectedFechaInicial
        {
            get { return selectedFechaInicial; }
            set { selectedFechaInicial = value; OnPropertyChanged("SelectedFechaInicial"); }
        }

        private DateTime? selectedFechaFinal;
        public DateTime? SelectedFechaFinal
        {
            get { return selectedFechaFinal; }
            set { selectedFechaFinal = value; OnPropertyChanged("SelectedFechaFinal"); }
        }

        private short fTipo = 1;
        public short FTipo
        {
            get { return fTipo; }
            set
            {
                fTipo = value;
                if (value == 1)
                { 
                    RBVisible = Visibility.Visible;
                    RBVulnerable = true;
                }
                else
                    RBVisible = Visibility.Collapsed;
                OnPropertyChanged("FTipo");
            }
        }

        private bool rBVulnerable = true;
        public bool RBVulnerable
        {
            get { return rBVulnerable; }
            set { rBVulnerable = value; OnPropertyChanged("RBVulnerable"); }
        }

        private bool rBTotalPoblacion;
        public bool RBTotalPoblacion
        {
            get { return rBTotalPoblacion; }
            set { rBTotalPoblacion = value; OnPropertyChanged("RBTotalPoblacion"); }
        }
        #endregion

        #region Pantalla
        private Visibility filtrosVisible = Visibility.Visible;
        public Visibility FiltrosVisible
        {
            get { return filtrosVisible; }
            set { filtrosVisible = value; OnPropertyChanged("FiltrosVisible"); }
        }

        private Visibility rBVisible = Visibility.Visible;
        public Visibility RBVisible
        {
            get { return rBVisible; }
            set { rBVisible = value; OnPropertyChanged("RBVisible"); }
        }

        private ReportViewer reporte;
        public ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        private Visibility reportViewerVisible = Visibility.Collapsed;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
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
