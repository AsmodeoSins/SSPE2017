using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteHorarioAreaViewModel
    {
        #region Filtros
        private ObservableCollection<AREA> _ListArea;
        public ObservableCollection<AREA> ListArea
        {
            get { return _ListArea; }
            set
            {
                _ListArea = value;
                OnPropertyChanged("ListArea");
            }
        }

        short? _SelectedArea;
        public short? SelectedArea
        {
            get { return _SelectedArea; }
            set
            {
                _SelectedArea = value;
                OnPropertyChanged("SelectedArea");
            }
        }
        #endregion

        #region Panatalla
        private Visibility reportViewerVisible = Visibility.Visible;
        public Visibility ReportViewerVisible
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

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
