using ControlPenales;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteRelacionInternoUbicacionViewModel
    {
        #region Filtros
        private ObservableCollection<EDIFICIO> lstEdificios;
        public ObservableCollection<EDIFICIO> LstEdificios
        {
            get { return lstEdificios; }
            set { lstEdificios = value; OnPropertyChanged("LstEdificios"); }
        }
        private EDIFICIO selectedEdificio;
        public EDIFICIO SelectedEdificio
        {
            get { return selectedEdificio; }
            set { selectedEdificio = value;
            if (value != null)
            {
                if (value.SECTOR != null)
                    LstSectores = new ObservableCollection<SECTOR>(value.SECTOR);
                else
                    LstSectores = new ObservableCollection<SECTOR>();
                LstSectores.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "TODOS" });
                FSector = -1;
            }
                OnPropertyChanged("SelectedEdificio"); }
        }
        private short? fEdificio;
        public short? FEdificio
        {
            get { return fEdificio; }
            set { fEdificio = value; OnPropertyChanged("FEdificio"); }
        }
        private ObservableCollection<SECTOR> lstSectores;
        public ObservableCollection<SECTOR> LstSectores
        {
            get { return lstSectores; }
            set { lstSectores = value; OnPropertyChanged("LstSectores"); }
        }
        private SECTOR selectedSector;
        public SECTOR SelectedSector
        {
            get { return selectedSector; }
            set { selectedSector = value; OnPropertyChanged("SelectedSector"); }
        }
        private short? fSector;
        public short? FSector
        {
            get { return fSector; }
            set { fSector = value; OnPropertyChanged("FSector"); }
        }

        private bool incluirFoto = false;
        public bool IncluirFoto
        {
            get { return incluirFoto; }
            set { incluirFoto = value; OnPropertyChanged("IncluirFoto"); }
        }

        private bool incluirEdad = false;
        public bool IncluirEdad
        {
            get { return incluirEdad; }
            set { incluirEdad = value; OnPropertyChanged("IncluirEdad"); }
        }

        private bool incluirNCP = false;
        public bool IncluirNCP
        {
            get { return incluirNCP; }
            set { incluirNCP = value; OnPropertyChanged("IncluirNCP"); }
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

        private WindowsFormsHost repositorio;
        public WindowsFormsHost Repositorio
        {
            get { return repositorio; }
            set { repositorio = value; OnPropertyChanged("Repositorio"); }
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
