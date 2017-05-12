using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteFormatoIdentificacionViewModel
    {
        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        private ReporteFormatoIdentificacion ventana;
        public ReporteFormatoIdentificacion Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }

        private List<cFormatoIdentificacion> listaIngresos;
        public List<cFormatoIdentificacion> ListaIngresos
        {
            get { return listaIngresos; }
            set { listaIngresos = value; OnPropertyChanged("ListaIngresos"); }
        }

        private List<cFormatoIdentificacion> listaIngresosSeleccionados;
        public List<cFormatoIdentificacion> ListaIngresosSeleccionados
        {
            get { return listaIngresosSeleccionados; }
            set { listaIngresosSeleccionados = value; OnPropertyChanged("ListaIngresosSeleccionados"); }
        }

        private cFormatoIdentificacion selectedIngreso;
        public cFormatoIdentificacion SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value; OnPropertyChanged("SelectedIngreso"); }
        }

        private cFormatoIdentificacion selectedIngresoSeleccionado;
        public cFormatoIdentificacion SelectedIngresoSeleccionado
        {
            get { return selectedIngresoSeleccionado; }
            set { selectedIngresoSeleccionado = value; OnPropertyChanged("SelectedIngresoSeleccionado"); }
        }


        private byte[] fotoIngreso;
        public byte[] FotoIngreso
        {
            get { return fotoIngreso; }
            set { fotoIngreso = value; OnPropertyChanged("FotoIngreso"); }
        }

        private byte[] fotoCentro;
        public byte[] FotoCentro
        {
            get { return fotoCentro; }
            set { fotoCentro = value; OnPropertyChanged("FotoCentro"); }
        }

        private int Pagina { get; set; }
        private bool SeguirCargandoIngresos { get; set; }

        private bool seleccionarTodosIngresos;
        public bool SeleccionarTodosIngresos
        {
            get { return seleccionarTodosIngresos; }
            set { seleccionarTodosIngresos = value; OnPropertyChanged("SeleccionarTodosIngresos"); }
        }

        private bool seleccionarTodosIngresosSeleccionados;
        public bool SeleccionarTodosIngresosSeleccionados
        {
            get { return seleccionarTodosIngresosSeleccionados; }
            set { seleccionarTodosIngresosSeleccionados = value; OnPropertyChanged("SeleccionarTodosIngresosSeleccionados"); }
        }

        private bool emptyVisible;
        public bool EmptyVisible
        {
            get { return emptyVisible; }
            set { emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }

        private bool emptySeleccionadosVisible;
        public bool EmptySeleccionadosVisible
        {
            get { return emptySeleccionadosVisible; }
            set { emptySeleccionadosVisible = value; OnPropertyChanged("EmptySeleccionadosVisible"); }
        }


        private Visibility reportViewerVisible;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }

        private string anioBuscarImputado;
        public string AnioBuscarImputado
        {
            get { return anioBuscarImputado; }
            set { anioBuscarImputado = value; OnPropertyChanged("AnioBuscarImputado"); }
        }

        private string folioBuscarImputado;
        public string FolioBuscarImputado
        {
            get { return folioBuscarImputado; }
            set { folioBuscarImputado = value; OnPropertyChanged("FolioBuscarImputado"); }
        }

        private string nombreBuscarImputado;
        public string NombreBuscarImputado
        {
            get { return nombreBuscarImputado; }
            set { nombreBuscarImputado = value; OnPropertyChanged("NombreBuscarImputado"); }
        }

        private string paternoBuscarImputado;
        public string PaternoBuscarImputado
        {
            get { return paternoBuscarImputado; }
            set { paternoBuscarImputado = value; OnPropertyChanged("PaternoBuscarImputado"); }
        }

        private string maternoBuscarImputado;
        public string MaternoBuscarImputado
        {
            get { return maternoBuscarImputado; }
            set { maternoBuscarImputado = value; OnPropertyChanged("MaternoBuscarImputado"); }
        }

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
