using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using SSP.Controlador;
namespace ControlPenales
{
    partial class PlanimetriaViewModel
    {
        #region Listas
        private ObservableCollection<CENTRO> lstCentros;
        public ObservableCollection<CENTRO> LstCentros
        {
            get { return lstCentros; }
            set { lstCentros = value; OnPropertyChanged("LstCentros"); }
        }

        private ObservableCollection<CELDA> lstCeldas;
        public ObservableCollection<CELDA> LstCeldas
        {
            get { return lstCeldas; }
            set
            {
                lstCeldas = value;
                OnPropertyChanged("LstCeldas");
            }
        }

        private ObservableCollection<SECTOR_OBSERVACION> lstObservacionesXSector;
        public ObservableCollection<SECTOR_OBSERVACION> LstObservacionesXSector
        {
            get { return lstObservacionesXSector; }
            set
            {
                lstObservacionesXSector = value;
                OnPropertyChanged("LstObservacionesXSector");
            }
        }

        private SECTOR_OBSERVACION_CELDA selectedSectorObservacionCelda;
        public SECTOR_OBSERVACION_CELDA SelectedSectorObservacionCelda
        {
            get { return selectedSectorObservacionCelda; }
            set { 
            selectedSectorObservacionCelda = value;
            if (value != null)
                PopulateImputadoCelda();
            OnPropertyChanged("SelectedSectorObservacionCelda");
            }
        }
        
        private ObservableCollection<CeldaX> lstCeldaX;
        public ObservableCollection<CeldaX> LstCeldaX
        {
            get { return lstCeldaX; }
            set { lstCeldaX = value; OnPropertyChanged("LstCeldaX"); }
        }

        //private ObservableCollection<AuxiliarPlanimetria> lstAuxPlanimetria;
        //public ObservableCollection<AuxiliarPlanimetria> LstAuxPlanimetria
        //{
        //    get { return lstAuxPlanimetria; }
        //    set
        //    {
        //        lstAuxPlanimetria = value;
        //        OnPropertyChanged("LstAuxPlanimetria");
        //    }
        //}
        #endregion
       
        #region Selected
        private SECTOR _SelectedSector;
        public SECTOR SelectedSector
        {
            get { return _SelectedSector; }
            set
            {
                _SelectedSector = value;
                if (value != null)
                {
                    PopulateImputadoSector();
                }
                OnPropertyChanged("SelectedSector");
            }
        }
        private SECTOR_OBSERVACION _SelectedObservacion;
        public SECTOR_OBSERVACION SelectedObservacion
        {
            get { return _SelectedObservacion; }
            set
            {
                _SelectedObservacion = value;
                PopulateImputadoSector();
                OnPropertyChanged("SelectedObservacion");
            }
        }

        #endregion
        
        #region PopUp
        private SECTOR_OBSERVACION _popUpCeldaObs = new SECTOR_OBSERVACION();
        public SECTOR_OBSERVACION popUpCeldaObs
        {
            get { return _popUpCeldaObs; }
            set { _popUpCeldaObs = value; OnPropertyChanged("popUpCeldaObs"); }
        }

        private string _popUpObservacion;
        public string popUpObservacion
        {
            get { return _popUpObservacion; }
            set { _popUpObservacion = value; OnPropertyChanged("popUpObservacion"); }
        }
        #endregion
        
        #region variables
        private string _EdificioSectorHeader;
        public string EdificioSectorHeader
        {
            get { return _EdificioSectorHeader; }
            set 
            {
                _EdificioSectorHeader = value;
                OnPropertyChanged("EdificioSectorHeader");
            }
        }
        #endregion

        //CLASIFICACION
        private ObservableCollection<SECTOR_CLASIFICACION> lstSectorClasificacion;
        public ObservableCollection<SECTOR_CLASIFICACION> LstSectorClasificacion
        {
            get { return lstSectorClasificacion; }
            set { lstSectorClasificacion = value; OnPropertyChanged("LstSectorClasificacion"); }
        }

        private SECTOR_CLASIFICACION selectedSectorClasificacion;
        public SECTOR_CLASIFICACION SelectedSectorClasificacion
        {
            get { return selectedSectorClasificacion; }
            set { selectedSectorClasificacion = value; OnPropertyChanged("SelectedSectorClasificacion"); }
        }

        private short? idSectorClasificacion;
        public short? IdSectorClasificacion
        {
            get { return idSectorClasificacion; }
            set { idSectorClasificacion = value; OnPropertyChanged("IdSectorClasificacion"); }
        }

        //MOSTRAR LOS INTERNOS EN EL SECTOR
        private ObservableCollection<INGRESO> lstImputadoSector;
        public ObservableCollection<INGRESO> LstImputadoSector
        {
            get { return lstImputadoSector; }
            set { lstImputadoSector = value;
                OnPropertyChanged("LstImputadoSector"); }
        }

        private ObservableCollection<INGRESO> lstImputadoCelda;
        public ObservableCollection<INGRESO> LstImputadoCelda
        {
            get { return lstImputadoCelda; }
            set
            {
                lstImputadoCelda = value;
                OnPropertyChanged("LstImputadoCelda");
            }
        }

        private bool emptyImputados = true;
        public bool EmptyImputados
        {
            get { return emptyImputados; }
            set { emptyImputados = value; OnPropertyChanged("EmptyImputados"); }
        }

        //selecciona todos
        private bool isTodosSelected;
        public bool IsTodosSelected
        {
            get { return isTodosSelected; }
            set
            {
                isTodosSelected = value;
                OnPropertyChanged("IsTodosSelected");

                if (LstCeldaX != null)
                {
                    foreach (var c in LstCeldaX)
                    {
                        c.Seleccionado = value;
                    }
                    LstCeldaX = new ObservableCollection<CeldaX>(LstCeldaX);
                }

                //foreach (var ingreso in LstIAS)
                //{
                //    ingreso.Seleccione = value;
                //}
                //LstIAS = new ObservableCollection<Clases.IngresoAinterior>(LstIAS);
            }
        }

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set { pInsertar = value; }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set { pEditar = value; }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set { pConsultar = value; }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set { pImprimir = value; }
        }
        #endregion

        #region Menu
        private bool reporteEnabled = false;
        public bool ReporteEnabled
        {
            get { return reporteEnabled; }
            set { reporteEnabled = value; OnPropertyChanged("ReporteEnabled"); }
        }
        private bool agregarBotonEnabled = false;

        public bool AgregarBotonEnabled
        {
            get { return agregarBotonEnabled; }
            set { agregarBotonEnabled = value; OnPropertyChanged("AgregarBotonEnabled"); }
        }

        #endregion

        #region Menu
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        #region Pantalla
        private double imagenHeight;
        public double ImagenHeight
        {
            get { return imagenHeight; }
            set { imagenHeight = value; OnPropertyChanged("ImagenHeight"); }
        }
        #endregion

    }
}
