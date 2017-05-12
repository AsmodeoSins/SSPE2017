using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteInternoGrupoViewModel
    {
        #region Filtros
        private ObservableCollection<EJE> lstEje;
        public ObservableCollection<EJE> LstEje
        {
            get { return lstEje; }
            set { lstEje = value; OnPropertyChanged("LstEje"); }
        }

        private short? selectedEje;
        public short? SelectedEje
        {
            get { return selectedEje; }
            set
            {
                selectedEje = value;
                OnPropertyChanged("SelectedEje");

                if (value.HasValue)
                    ProgramasLoad(value.Value);
                else
                    LstPrograma = null;
            }
        }

        private ObservableCollection<TIPO_PROGRAMA> lstPrograma;
        public ObservableCollection<TIPO_PROGRAMA> LstPrograma
        {
            get { return lstPrograma; }
            set
            {
                lstPrograma = value;
                OnPropertyChanged("LstPrograma");
            }
        }

        private short? selectedPrograma;
        public short? SelectedPrograma
        {
            get { return selectedPrograma; }
            set
            {
                selectedPrograma = value;
                OnPropertyChanged("SelectedPrograma");

                if (value.HasValue)
                    ActividadesLoad(value.Value);
                else
                    LstActividad = null;
            }
        }

        private ObservableCollection<ACTIVIDAD> lstActividad;
        public ObservableCollection<ACTIVIDAD> LstActividad
        {
            get { return lstActividad; }
            set
            {
                lstActividad = value;
                OnPropertyChanged("LstActividad");
            }
        }

        private short? selectedActividad;
        public short? SelectedActividad
        {
            get { return selectedActividad; }
            set
            {
                selectedActividad = value;
                OnPropertyChanged("SelectedActividad");

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    ReportViewerVisible = Visibility.Collapsed;
                    if (value.HasValue)
                        LstGrupo = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<GRUPO>>(() => new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == value.Value && w.ID_EJE == SelectedEje && w.ID_ESTATUS_GRUPO == 1).Select(s => s).Distinct().OrderBy(o => o.DESCR).ToList()));
                    else
                        LstGrupo = null;

                    Reporte.Reset();
                    Reporte.RefreshReport();
                    ReportViewerVisible = Visibility.Visible;
                }));
            }
        }

        private ObservableCollection<GRUPO> lstGrupo;
        public ObservableCollection<GRUPO> LstGrupo
        {
            get { return lstGrupo; }
            set { lstGrupo = value; OnPropertyChanged("LstGrupo"); }
        }

        short? selectedGrupo;
        public short? SelectedGrupo
        {
            get { return selectedGrupo; }
            set
            {
                selectedGrupo = value;
                OnPropertyChanged("SelectedGrupo");
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
