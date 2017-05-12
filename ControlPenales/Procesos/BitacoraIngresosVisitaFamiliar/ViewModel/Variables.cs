using ControlPenales;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class BitacoraIngresosVisitaFamiliarViewModel
    {
        #region Filtros
        private DateTime? fFechaInicio;
        public DateTime? FFechaInicio
        {
            get { return fFechaInicio; }
            set { fFechaInicio = value;
                ValidarFiltros();
                OnPropertyChanged("FFechaInicio"); }
        }

        private DateTime? fFechaFin;
        public DateTime? FFechaFin
        {
            get { return fFechaFin; }
            set { fFechaFin = value;
                ValidarFiltros();
                OnPropertyChanged("FFechaFin"); }
        }
        #endregion

        #region Panatalla
        private Visibility reportViewerVisible = Visibility.Visible;
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

        #endregion

        #region Permisos
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
