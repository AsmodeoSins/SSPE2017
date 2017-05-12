using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteControlVisitantesXDiaIntimaViewModel
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

        private ReporteControlVisitantesXDiaIntima ventana;
        public ReporteControlVisitantesXDiaIntima Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }

        private DateTime selectedFechaInicial;
        public DateTime SelectedFechaInicial
        {
            get { return selectedFechaInicial; }
            set { selectedFechaInicial = value; OnPropertyChanged("SelectedFechaInicial"); }
        }

        private DateTime selectedFechaFinal;
        public DateTime SelectedFechaFinal
        {
            get { return selectedFechaFinal; }
            set { selectedFechaFinal = value; OnPropertyChanged("SelectedFechaFinal"); }
        }

        const int MAYORIA_DE_EDAD = 18;
        const string HOMBRES = "HOMBRES";
        const string MUJERES = "MUJERES";
        const string MENORES = "MENORES";
        const string FEMENINO = "F";
        const string MASCULINO = "M";

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
