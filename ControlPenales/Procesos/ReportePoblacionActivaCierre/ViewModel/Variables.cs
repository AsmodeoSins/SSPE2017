namespace ControlPenales
{
    public partial class ReportePoblacionActivaCierreViewModel
    {
        #region Panatalla
        private System.Windows.Visibility _reportViewerVisible = System.Windows.Visibility.Visible;
        public System.Windows.Visibility ReportViewerVisible
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
                {
                    //FechaMaxima = value.Value; 
                    if (SelectedFechaFinal.HasValue)
                    {
                        base.ClearRules();
                        base.AddRule(() => SelectedFechaInicial, () => SelectedFechaInicial.Value.Date <= SelectedFechaFinal.Value.Date, "LA FECHA INICIO DEBE SER MAYOR A LA FECHA FINAL!");
                        OnPropertyChanged("SelectedFechaInicial");
                    }
                }
                else
                {
                    base.ClearRules();
                    base.AddRule(() => SelectedFechaInicial, () => SelectedFechaInicial.HasValue, "FECHA DE INICIO ES REQUERIDA!");
                    OnPropertyChanged("SelectedFechaInicial");
                    base.RemoveRule("SelectedFechaFinal");
                    base.AddRule(() => SelectedFechaFinal, () => SelectedFechaFinal.HasValue, "FECHA FINAL ES REQUERIDA!");
                    OnPropertyChanged("SelectedFechaFinal");
                }
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
            set { selectedFechaFinal = value;
            if (value.HasValue)
            {
                if (SelectedFechaInicial.HasValue)
                {
                    base.ClearRules();
                    base.AddRule(() => SelectedFechaInicial, () => SelectedFechaInicial.Value.Date <= SelectedFechaFinal.Value.Date, "LA FECHA INICIO DEBE SER MENOR A LA FECHA FINAL!");
                    OnPropertyChanged("SelectedFechaInicial");
                }
            }
            else
            {
                base.ClearRules();
                base.RemoveRule("SelectedFechaInicial");
                base.AddRule(() => SelectedFechaInicial, () => SelectedFechaInicial.HasValue, "FECHA DE INICIO ES REQUERIDA!");
                OnPropertyChanged("SelectedFechaInicial");
                base.RemoveRule("SelectedFechaFinal");
                base.AddRule(() => SelectedFechaFinal, () => SelectedFechaFinal.HasValue, "FECHA FINAL ES REQUERIDA!");
                OnPropertyChanged("SelectedFechaFinal");
            }
                OnPropertyChanged("SelectedFechaFinal"); }
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