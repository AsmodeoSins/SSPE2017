using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    public partial class ReportePaseListaViewModel
    {
        #region Ventana
        private ReportePaseLista ventana;

        public ReportePaseLista Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }
        #endregion

        #region Propiedades Listas_Filtros
        private List<eMesesAnio> meses;
        public List<eMesesAnio> Meses
        {
            get { return meses; }
            set { meses = value; OnPropertyChanged("Meses"); }
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
        #endregion

        #region Propiedades Filtros
        private EDIFICIO selectedEdificio;
        public EDIFICIO SelectedEdificio
        {
            get { return selectedEdificio; }
            set
            {
                selectedEdificio = value;
                if (SelectedEdificio.ID_EDIFICIO != TODOS_LOS_EDIFICIOS)
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
        #endregion

        #region Propiedades Reporte
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

        private eMesesAnio selectedMes;
        public eMesesAnio SelectedMes
        {
            get { return selectedMes; }
            set { selectedMes = value; OnPropertyChanged("SelectedMes"); }
        }
        #endregion

        #region Constantes
        const string EDIFICIO_ACTIVO = "S";
        const string EDIFICIO_INACTIVO = "N";
        const string SECTOR_ACTIVO = "S";
        const string SECTOR_INACTIVO = "N";
        const short TODOS_LOS_EDIFICIOS = -1;
        const short TODOS_LOS_SECTORES = -1;
        const string CAMA_LIBERADA = "S";
        const string CAMA_OCUPADA = "N";
        const double CAMAS_POR_PAGINA = 35.0;
        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
