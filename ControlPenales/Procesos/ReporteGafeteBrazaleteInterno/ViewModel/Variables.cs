using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteBrazaleteGafeteViewModel
    {
        #region Propiedades Reporte
        private Visibility brazaleteReportViewerVisible = Visibility.Visible;
        public Visibility BrazaleteReportViewerVisible
        {
            get { return brazaleteReportViewerVisible; }
            set { brazaleteReportViewerVisible = value; OnPropertyChanged("BrazaleteReportViewerVisible"); }
        }

        private Visibility gafeteReportViewerVisible = Visibility.Visible;
        public Visibility GafeteReportViewerVisible
        {
            get { return gafeteReportViewerVisible; }
            set { gafeteReportViewerVisible = value; OnPropertyChanged("GafeteReportViewerVisible"); }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }
        #endregion

        #region Enumerables
        public enum enumTipoReporte
        {
            BRAZALETE = 1,
            GAFETE = 2
        }
        #endregion

        #region Constantes
        const string EDIFICIO_ACTIVO = "S";
        const string EDIFICIO_INACTIVO = "N";
        const string SECTOR_ACTIVO = "S";
        const string SECTOR_INACTIVO = "N";
        const short TODOS_LOS_EDIFICIOS = -1;
        const short TODOS_LOS_SECTORES = -1;
        #endregion

        #region Propiedades SeleccionTipoReporte
        private bool brazaleteSelected;
        public bool BrazaleteSelected
        {
            get { return brazaleteSelected; }
            set
            {
                brazaleteSelected = value;
                if (value)
                {
                    TextoGenerarReporte = "Generar Brazalete(s)";
                    BrazaleteReportViewerVisible = Visibility.Visible;
                    GafeteReportViewerVisible = Visibility.Collapsed;
                }

                OnPropertyChanged("BrazaleteSelected");
            }
        }

        private bool gafeteSelected;
        public bool GafeteSelected
        {
            get { return gafeteSelected; }
            set
            {
                gafeteSelected = value;
                if (value)
                {
                    TextoGenerarReporte = "Generar Gafete(s)";
                    BrazaleteReportViewerVisible = Visibility.Collapsed;
                    GafeteReportViewerVisible = Visibility.Visible;
                }

                OnPropertyChanged("GafeteSelected");
            }
        }
        #endregion

        #region Propiedades Textos
        private string textoGenerarReporte;
        public string TextoGenerarReporte
        {
            get { return textoGenerarReporte; }
            set { textoGenerarReporte = value; OnPropertyChanged("TextoGenerarReporte"); }
        }

        private bool busquedaAvanzadaChecked;
        public bool BusquedaAvanzadaChecked
        {
            get { return busquedaAvanzadaChecked; }
            set
            {
                busquedaAvanzadaChecked = value;
                ListaIngresos = new List<cInternoGafeteBrazalete>();
                OnPropertyChanged("BusquedaAvanzadaChecked");
            }
        }

        private int? anioBuscar;
        public int? AnioBuscar
        {
            get { return anioBuscar; }
            set { anioBuscar = value; OnPropertyChanged("AnioBuscar"); }
        }

        private int? folioBuscar;
        public int? FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value; OnPropertyChanged("FolioBuscar"); }
        }

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set { nombreBuscar = value; OnPropertyChanged("NombreBuscar"); }
        }

        private string apellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return apellidoPaternoBuscar; }
            set { apellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
        }

        private string apellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return apellidoMaternoBuscar; }
            set { apellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscar"); }
        }



        #endregion

        #region Propiedades Listas_Filtros
        private List<EDIFICIO> edificios;
        public List<EDIFICIO> Edificios
        {
            get { return edificios; }
            set
            {
                edificios = value
                    ; OnPropertyChanged("Edificios");
            }
        }


        private List<SECTOR> sectores;
        public List<SECTOR> Sectores
        {
            get { return sectores; }
            set { sectores = value; OnPropertyChanged("Sectores"); }
        }
        #endregion

        #region Propiedades Filtros
        private EDIFICIO selectedEdificio;
        public EDIFICIO SelectedEdificio
        {
            get { return selectedEdificio; }
            set
            {
                selectedEdificio = value;
                ObtenerSectores();
                OnPropertyChanged("SelectedEdificio");
            }
        }

        private SECTOR selectedSector;
        public SECTOR SelectedSector
        {
            get { return selectedSector; }
            set { selectedSector = value; OnPropertyChanged("SelectedSector"); }
        }
        #endregion

        #region Propiedades Interno
        private List<cInternoGafeteBrazalete> listaIngresos;
        public List<cInternoGafeteBrazalete> ListaIngresos
        {
            get { return listaIngresos; }
            set { listaIngresos = value; OnPropertyChanged("ListaIngresos"); }
        }

        private List<cInternoGafeteBrazalete> listaIngresosSeleccionados;
        public List<cInternoGafeteBrazalete> ListaIngresosSeleccionados
        {
            get { return listaIngresosSeleccionados; }
            set { listaIngresosSeleccionados = value; OnPropertyChanged("ListaIngresosSeleccionados"); }
        }

        private cInternoGafeteBrazalete selectedIngreso;
        public cInternoGafeteBrazalete SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value;
            if (value != null)
            {
                FotoIngreso = value.FOTOINGRESO;
                FotoCentro = value.FOTOCENTRO;
            }
            else
            {
                FotoIngreso = FotoCentro = new Imagenes().getImagenPerson();
            }
                OnPropertyChanged("SelectedIngreso"); }
        }

        private cInternoGafeteBrazalete selectedIngresoSeleccionado;
        public cInternoGafeteBrazalete SelectedIngresoSeleccionado
        {
            get { return selectedIngresoSeleccionado; }
            set { selectedIngresoSeleccionado = value;
            if (value != null)
            {
                FotoIngreso = value.FOTOINGRESO;
                FotoCentro = value.FOTOCENTRO;
            }
            else
            {
                FotoIngreso = FotoCentro = new Imagenes().getImagenPerson();
            }                
                OnPropertyChanged("SelectedIngresoSeleccionado"); }
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
        #endregion

        #region Propiedades Ventana
        private ReporteBrazaleteGafete ventana;
        public ReporteBrazaleteGafete Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
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
        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
