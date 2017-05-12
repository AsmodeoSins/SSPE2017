using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class ResporteBitacoraTiempoViewModel
    {
        #region Panatalla
        private bool reportViewerVisible = false;
        public bool ReportViewerVisible
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

        private DateTime? _TextFechaInicio;

        public DateTime? TextFechaInicio
        {
            get { return _TextFechaInicio; }
            set { _TextFechaInicio = value; OnPropertyChanged("TextFechaInicio"); }
        }

        private DateTime? _TextFechaFin;

        public DateTime? TextFechaFin
        {
            get { return _TextFechaFin; }
            set { _TextFechaFin = value; OnPropertyChanged("TextFechaFin"); }
        }

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
