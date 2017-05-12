using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class PoblacionExtranjeraViewModel
    {
        private Visibility _reportViewerVisible = Visibility.Visible;
        public Visibility ReportViewerVisible
        {
            get { return _reportViewerVisible; }
            set
            {
                _reportViewerVisible = value;
                OnPropertyChanged("ReportViewerVisible");
            }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set
            {
                reporte = value;
                OnPropertyChanged("Reporte");
            }
        }

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
