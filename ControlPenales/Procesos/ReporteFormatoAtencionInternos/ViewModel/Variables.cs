using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReportePapeletasViewModel
    {
        private ReportePapeletas ventana;
        public ReportePapeletas Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
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

        private short fEdificio = -1;
        public short FEdificio
        {
            get { return fEdificio; }
            set { fEdificio = value; OnPropertyChanged("FEdificio"); }
        }

        private short fSector = -1;
        public short FSector
        {
            get { return fSector; }
            set { fSector = value; OnPropertyChanged("FSector"); }
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

        const short TODOS_LOS_EDIFICIOS = -1;
        const short TODOS_LOS_SECTORES = -1;
        const string EDIFICIO_ACTIVO = "S";
        const string EDIFICIO_INACTIVO = "N";
        const string SECTOR_ACTIVO = "S";
        const string SECTOR_INACTIVO = "N";
        const string CAMA_LIBERADA = "S";
        const string CAMA_OCUPADA = "N";

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
