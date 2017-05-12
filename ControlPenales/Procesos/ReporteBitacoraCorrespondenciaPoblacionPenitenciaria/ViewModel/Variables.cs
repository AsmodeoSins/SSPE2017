using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteBitacoraCorrespondenciaPoblacionPenitenciariaViewModel
    {
        private ReporteBitacoraCorrespondenciaPoblacionPenitenciaria ventana;
        public ReporteBitacoraCorrespondenciaPoblacionPenitenciaria Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }

        private DateTime selectedFecha;
        public DateTime SelectedFecha
        {
            get { return selectedFecha; }
            set { selectedFecha = value; OnPropertyChanged("SelectedFecha"); }
        }

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

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
