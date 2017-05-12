using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteReosPeligrososViewModel
    {
        private ReporteReosPeligrosos ventana;
        public ReporteReosPeligrosos Ventana
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

        private Visibility reportViewerVisible;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }

        private bool fechasHabilitadas;
        public bool FechasHabilitadas
        {
            get { return fechasHabilitadas; }
            set { fechasHabilitadas = value; OnPropertyChanged("FechasHabilitadas"); }
        }

        private bool todosLosReos;
        public bool TodosLosReos
        {
            get { return todosLosReos; }
            set
            {

                todosLosReos = value;
                if (value)
                    FechasHabilitadas = false;
                else
                    FechasHabilitadas = true;

                OnPropertyChanged("TodosLosReos");
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
