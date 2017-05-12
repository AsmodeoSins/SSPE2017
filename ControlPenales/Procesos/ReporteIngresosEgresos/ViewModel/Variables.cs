using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteIngresosEgresosViewModel
    {

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        private DateTime selectedfechaInicial;
        public DateTime SelectedFechaInicial
        {
            get { return selectedfechaInicial; }
            set { selectedfechaInicial = value; OnPropertyChanged("SelectedFechaInicial"); }
        }

        private DateTime selectedfechaFinal;
        public DateTime SelectedFechaFinal
        {
            get { return selectedfechaFinal; }
            set { selectedfechaFinal = value; OnPropertyChanged("SelectedfechaFinal"); }
        }

        private Visibility reportViewerVisible;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }

        private Visibility seleccionMesVisible;
        public Visibility SeleccionMesVisible
        {
            get { return seleccionMesVisible; }
            set { seleccionMesVisible = value; OnPropertyChanged("SeleccionMesVisible"); }
        }

        private Visibility rangoFechasVisible;
        public Visibility RangoFechasVisible
        {
            get { return rangoFechasVisible; }
            set { rangoFechasVisible = value; OnPropertyChanged("RangoFechasVisible"); }
        }

        private ReporteIngresosEgresos ventana;
        public ReporteIngresosEgresos Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }

        private bool seleccionMesSelected;
        public bool SeleccionMesSelected
        {
            get { return seleccionMesSelected; }
            set
            {
                seleccionMesSelected = value;
                if (value)
                {
                    SeleccionMesVisible = Visibility.Visible;
                    RangoFechasVisible = Visibility.Collapsed;
                }
                OnPropertyChanged("SeleccionMesSelected");
            }
        }

        private bool rangoFechasSelected;
        public bool RangoFechasSelected
        {
            get { return rangoFechasSelected; }
            set
            {
                rangoFechasSelected = value;
                if (value)
                {
                    SeleccionMesVisible = Visibility.Collapsed;
                    RangoFechasVisible = Visibility.Visible;
                }
                OnPropertyChanged("RangoFechasSelected");
            }
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
