using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms.Integration;

namespace ControlPenales
{
    public partial class ReporteSituacionJuridicaViewModel
    {
        private ReporteSituacionJuridicaView ventana;
        public ReporteSituacionJuridicaView Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }

        private Visibility reportViewerVisible;
        public Visibility ReportViewerVisible
        {
            get { return reportViewerVisible; }
            set { reportViewerVisible = value; OnPropertyChanged("ReportViewerVisible"); }
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

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }


        const short TODOS_LOS_EDIFICIOS = -1;
        const short TODOS_LOS_SECTORES = -1;
        const string EDIFICIO_ACTIVO = "S";
        const string EDIFICIO_INACTIVO = "N";
        const string SECTOR_ACTIVO = "S";
        const string SECTOR_INACTIVO = "N";

        #region Constantes_Situaciones_Juridicas
        const string INDICIADO_NUMERO = "1";
        const string PROCESADO_NUMERO = "2";
        const string SENTENCIADO_NUMERO = "3";
        const string DISCRECIONAL = "4";

        const string INDICIADO = "I";
        const string PROCESADO = "P";
        const string SENTENCIADO = "S";
        const string COMPURGADO = "C";
        const string IDENTIFICADO = "U";
        const string LIBERADO = "L";
        const string PRELIBERADO = "R";
        const string EVADIDO = "B";

        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion

    }
}
