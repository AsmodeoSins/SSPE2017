using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReporteCredencialBibliotecaViewModel
    {
        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        private bool busquedaAvanzadaChecked;
        public bool BusquedaAvanzadaChecked
        {
            get { return busquedaAvanzadaChecked; }
            set
            {
                busquedaAvanzadaChecked = value;
                ListaIngresos = new List<cCredencialBiblioteca>();
                SeleccionarTodosIngresos = false;
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

        private List<EDIFICIO> edificios;
        public List<EDIFICIO> Edificios
        {
            get { return edificios; }
            set { edificios = value; OnPropertyChanged("Edificios"); }
        }

        private List<SECTOR> sectores;
        public List<SECTOR> Sectores
        {
            get { return sectores; }
            set { sectores = value; OnPropertyChanged("Sectores"); }
        }

        private EDIFICIO selectedEdificio;
        public EDIFICIO SelectedEdificio
        {
            get { return selectedEdificio; }
            set
            {
                selectedEdificio = value;
                if (value.ID_EDIFICIO != TODOS_LOS_EDIFICIOS)
                {
                    ObtenerSectores();
                }
                else
                {
                    Sectores = new List<SECTOR>();
                    Sectores.Add(new SECTOR() { ID_SECTOR = TODOS_LOS_SECTORES, DESCR = "TODOS" });
                    SelectedSector = Sectores.FirstOrDefault();
                }

                OnPropertyChanged("SelectedEdificio");
            }
        }

        private SECTOR selectedSector;
        public SECTOR SelectedSector
        {
            get { return selectedSector; }
            set { selectedSector = value; OnPropertyChanged("SelectedSector"); }
        }

        private List<cCredencialBiblioteca> listaIngresos;
        public List<cCredencialBiblioteca> ListaIngresos
        {
            get { return listaIngresos; }
            set { listaIngresos = value; OnPropertyChanged("ListaIngresos"); }
        }

        private List<cCredencialBiblioteca> listaIngresosSeleccionados;
        public List<cCredencialBiblioteca> ListaIngresosSeleccionados
        {
            get { return listaIngresosSeleccionados; }
            set { listaIngresosSeleccionados = value; OnPropertyChanged("ListaIngresosSeleccionados"); }
        }

        private cCredencialBiblioteca selectedIngreso;
        public cCredencialBiblioteca SelectedIngreso
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

        private cCredencialBiblioteca selectedIngresoSeleccionado;
        public cCredencialBiblioteca SelectedIngresoSeleccionado
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

        private ReporteCredencialBiblioteca ventana;
        public ReporteCredencialBiblioteca Ventana
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

        private Visibility reportViewerVisible;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
        }

        const string EDIFICIO_ACTIVO = "S";
        const string EDIFICIO_INACTIVO = "N";
        const string SECTOR_ACTIVO = "S";
        const string SECTOR_INACTIVO = "N";
        const short TODOS_LOS_EDIFICIOS = -1;
        const short TODOS_LOS_SECTORES = -1;

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
