using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteControlDeVisitantesViewModel
    {
        private bool graficaEnabled;
        public bool GraficaEnabled
        {
            get { return graficaEnabled; }
            set { graficaEnabled = value; OnPropertyChanged("GraficaEnabled"); }
        }

        private ReporteControlDeVisitantes ventana;
        public ReporteControlDeVisitantes Ventana
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

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporteGrafica;
        public Microsoft.Reporting.WinForms.ReportViewer ReporteGrafica
        {
            get { return reporteGrafica; }
            set { reporteGrafica = value; OnPropertyChanged("ReporteGrafica"); }
        }

        private Visibility reportViewerVisible;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }

        private int PAGINA = 1;
        private int ULTIMO_REGISTRO = 0;

        public enum enumTipoPersona
        {
            ABOGADO = 2,
            VISITA = 3
        }

        const string ES_VISITA_INTIMA = "S";
        const string NO_ES_VISITA_INTIMA = "N";
        const string VISITA_INTIMA = "ÍNTIMA";
        const string VISITA_FAMILIAR = "FAMILIAR";
        const string VISITA_LEGAL = "LEGAL";
        const int MAYORIA_DE_EDAD = 18;
        const string MASCULINO = "M";
        const string FEMENINO = "F";
        const string HOMBRES = "HOMBRES";
        const string MUJERES = "MUJERES";
        const string MENORES = "MENORES";
        const int VISITANTES_POR_PAGINA = 10;

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
