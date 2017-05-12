using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteHorarioResponsableGruposViewModel
    {
        #region Filtros
        private ObservableCollection<NombreEmpleado> _LstResponsable;
        public ObservableCollection<NombreEmpleado> LstResponsable
        {
            get { return _LstResponsable; }
            set
            {
                _LstResponsable = value;
                OnPropertyChanged("LstResponsable");
            }
        }

        int _SelectedResponsable;
        public int SelectedResponsable
        {
            get { return _SelectedResponsable; }
            set
            {
                _SelectedResponsable = value;
                OnPropertyChanged("SelectedResponsable");
                LimpiarReporte();
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
