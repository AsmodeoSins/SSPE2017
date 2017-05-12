using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReportePoblacionInternosViewModel
    {
        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        private Visibility reportViewerVisible;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }

        private ReportePoblacionInternos ventana;
        public ReportePoblacionInternos Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }

        const string MASCULINO = "M";
        const string FEMENINO = "F";

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
