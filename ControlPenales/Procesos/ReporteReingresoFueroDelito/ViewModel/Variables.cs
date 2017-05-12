namespace ControlPenales
{
    public partial class ReporteReingresoViewModel
    {
        #region Panatalla
        private System.Windows.Visibility _reportViewerVisible = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility ReportViewerVisible
        {
            get { return _reportViewerVisible; }
            set { _reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        #endregion

        private System.DateTime? selectedFechaInicial = Fechas.GetFechaDateServer;
        public System.DateTime? SelectedFechaInicial
        {
            get { return selectedFechaInicial; }
            set
            {
                selectedFechaInicial = value;
                if (value.HasValue)
                    FechaMaxima = value.Value;

                OnPropertyChanged("SelectedFechaInicial");
            }
        }

        private System.DateTime _fechaMaxima = Fechas.GetFechaDateServer;
        public System.DateTime FechaMaxima
        {
            get { return _fechaMaxima; }
            set { _fechaMaxima = value; OnPropertyChanged("FechaMaxima"); }
        }

        private System.DateTime _fechaMinima;
        public System.DateTime FechaMinima
        {
            get { return _fechaMinima; }
            set { _fechaMinima = value; OnPropertyChanged("FechaMinima"); }
        }

        private System.DateTime? selectedFechaFinal = Fechas.GetFechaDateServer;
        public System.DateTime? SelectedFechaFinal
        {
            get { return selectedFechaFinal; }
            set { selectedFechaFinal = value; OnPropertyChanged("SelectedFechaFinal"); }
        }

        private bool _FechasHabilitadas = true;
        public bool FechasHabilitadas
        {
            get { return _FechasHabilitadas; }
            set { _FechasHabilitadas = value; OnPropertyChanged("FechasHabilitadas"); }
        }

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}