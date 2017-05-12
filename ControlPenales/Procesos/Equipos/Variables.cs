using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace ControlPenales
{
    partial class EquiposViewModel
    {
        #region Variables para visualizar Controles
        private Visibility isEquiposVisible = Visibility.Visible;
        public Visibility IsEquiposVisible
        {
            get { return isEquiposVisible; }
            set { isEquiposVisible = value; OnPropertyChanged("IsEquiposVisible"); }
        }
        #endregion
        
        #region Variables Privadas
        DateTime _FechaServer = Fechas.GetFechaDateServer;
        #endregion
        
        #region Busqueda
        private string busqueda;
        public string Busqueda
        {
            get { return busqueda; }
            set { busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private ObservableCollection<CATALOGO_EQUIPOS> listItems;
        public ObservableCollection<CATALOGO_EQUIPOS> ListItems
        {
            get { return listItems; }
            set { listItems = value; OnPropertyChanged("ListItems"); }
        }

        private CATALOGO_EQUIPOS selectedItem;
        public CATALOGO_EQUIPOS SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value;
            if (value != null)
            {
                EditarMenuEnabled = true;
                Editar = false;
            }
            else
            {
                EditarMenuEnabled = false;
                Editar = true;
            }
                OnPropertyChanged("SelectedItem"); }
        }
        #endregion

        #region Equipos

        private ObservableCollection<AREA> lstAreasAsignadas;
        public ObservableCollection<AREA> LstAreasAsignadas
        {
            get { return lstAreasAsignadas; }
            set { lstAreasAsignadas = value; OnPropertyChanged("LstAreasAsignadas"); }
        }

        private AREA selectedAreaAsignada;
        public AREA SelectedAreaAsignada
        {
            get { return selectedAreaAsignada; }
            set { selectedAreaAsignada = value; OnPropertyChanged("SelectedAreaAsignada"); }
        }

        private ObservableCollection<AREA> lstAreas;
        public ObservableCollection<AREA> LstAreas
        {
            get { return lstAreas; }
            set { lstAreas = value; OnPropertyChanged("LstAreas"); }
        }
        private short selectedAreaValue;
        public short SelectedAreaValue
        {
            get { return selectedAreaValue; }
            set { selectedAreaValue = value; OnPropertyValidateChanged("SelectedAreaValue"); }
        }
        private ObservableCollection<CENTRO> lstCentros;
        public ObservableCollection<CENTRO> LstCentros
        {
            get { return lstCentros; }
            set { lstCentros = value; OnPropertyChanged("LstCentros"); }
        }

        private CENTRO selectedCentro;
        public CENTRO SelectedCentro
        {
            get { return selectedCentro; }
            set { selectedCentro = value; OnPropertyValidateChanged("SelectedCentro"); }
        }

        private ObservableCollection<TipoEquipo> lstTipoEquipo;
        public ObservableCollection<TipoEquipo> LstTipoEquipo
        {
            get { return lstTipoEquipo; }
            set { lstTipoEquipo = value; OnPropertyChanged("LstTipoEquipo"); }
        }

        private short eCentro = GlobalVar.gCentro;
        public short ECentro
        {
            get { return eCentro; }
            set { eCentro = value; OnPropertyValidateChanged("ECentro"); }
        }

        private string eIP;
        public string EIP
        {
            get { return eIP; }
            set { eIP = value; OnPropertyValidateChanged("EIP"); }
        }

        private string eMacAddress;
        public string EMacAddress
        {
            get { return eMacAddress; }
            set { eMacAddress = value; OnPropertyValidateChanged("EMacAddress"); }
        }

        private string eSerieVolum;
        public string ESerieVolum
        {
            get { return eSerieVolum; }
            set { eSerieVolum = value; OnPropertyValidateChanged("ESerieVolum"); }
        }

        private string eDescripcion;
        public string EDescripcion
        {
            get { return eDescripcion; }
            set { eDescripcion = value; OnPropertyValidateChanged("EDescripcion"); }
        }

        private string eEstatus = "S";
        public string EEstatus
        {
            get { return eEstatus; }
            set { eEstatus = value; OnPropertyValidateChanged("EEstatus"); }
        }

        private DateTime eFechaAlta;
        public DateTime EFechaAlta
        {
            get { return eFechaAlta; }
            set { eFechaAlta = value; OnPropertyValidateChanged("EFechaAlta"); }
        }

        private DateTime? eFechaBaja;
        public DateTime? EFechaBaja
        {
            get { return eFechaBaja; }
            set { eFechaBaja = value; OnPropertyValidateChanged("EFechaBaja"); }
        }

        private DateTime? eFechaModificacion;
        public DateTime? EFechaModificacion
        {
            get { return eFechaModificacion; }
            set { eFechaModificacion = value; OnPropertyValidateChanged("EFechaModificacion"); }
        }

        private short eTipoEquipo = -1;
        public short ETipoEquipo
        {
            get { return eTipoEquipo; }
            set { eTipoEquipo = value; OnPropertyValidateChanged("ETipoEquipo"); }
        }

        private bool eBiometria;
        public bool EBiometria
        {
            get { return eBiometria; }
            set { eBiometria = value; OnPropertyValidateChanged("EBiometria"); }
        }
        #endregion

        #region Pantalla
        private bool editar = false;
        public bool Editar
        {
            get { return editar; }
            set { editar = value; OnPropertyChanged("Editar"); }
        }

        private EstatusControl lista_estatus = new EstatusControl();
        public EstatusControl Lista_Estatus
        {
            get { return lista_estatus; }
            set { lista_estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private Estatus selectedEstatus = null;
        public Estatus SelectedEstatus
        {
            get { return selectedEstatus; }
            set { selectedEstatus = value; RaisePropertyChanged("SelectedEstatus"); }
        }

        private bool emptyVisible;
        public bool EmptyVisible
        {
            get { return emptyVisible; }
            set { emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }

        private int maxLength;
        public int MaxLength
        {
            get { return maxLength; }
            set { maxLength = value; OnPropertyChanged("MaxLength"); }
        }
       
        private bool focusText;
        public bool FocusText
        {
            get { return focusText; }
            set { focusText = value; OnPropertyChanged("FocusText"); }
        }
       
        public bool bandera_editar = false;
        
        private string cambio;
        public string Cambio
        {
            get { return cambio; }
            set { cambio = value; OnPropertyChanged("Cambio"); }
        }
       
        private string catalogHeader;
        public string CatalogoHeader
        {
            get { return catalogHeader; }
            set { catalogHeader = value; }
        }
       
        private string headerAgregar;
        public string HeaderAgregar
        {
            get { return headerAgregar; }
            set { headerAgregar = value; OnPropertyChanged("HeaderAgregar"); }
        }
     
        private int seleccionIndice;
        public int SeleccionIndice
        {
            get { return seleccionIndice; }
            set { seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }
      
        private bool guardarMenuEnabled = false;
        public bool GuardarMenuEnabled
        {
            get { return guardarMenuEnabled; }
            set { guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }
    
        private bool agregarMenuEnabled = true;
        public bool AgregarMenuEnabled
        {
            get { return agregarMenuEnabled; }
            set { agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }
     
        private bool editarMenuEnabled = false;
        public bool EditarMenuEnabled
        {
            get { return editarMenuEnabled; }
            set { editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }
   
        private bool eliminarMenuEnabled = false;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }
 
        private bool cancelarMenuEnabled = false;
        public bool CancelarMenuEnabled
        {
            get { return cancelarMenuEnabled; }
            set { cancelarMenuEnabled = value; OnPropertyChanged("CancelarMenuEnabled"); }
        }
   
        private bool exportarMenuEnabled = true;
        public bool ExportarMenuEnabled
        {
            get { return exportarMenuEnabled; }
            set { exportarMenuEnabled = value; OnPropertyChanged("ExportarMenuEnabled"); }
        }

        private bool salirMenuEnabled = true;
        public bool SalirMenuEnabled
        {
            get { return salirMenuEnabled; }
            set { salirMenuEnabled = value; OnPropertyChanged("SalirMenuEnabled"); }
        }
   
        private bool ayudaMenuEnabled = true;
        public bool AyudaMenuEnabled
        {
            get { return ayudaMenuEnabled; }
            set { ayudaMenuEnabled = value; OnPropertyChanged("AyudaMenuEnabled"); }
        }
   
        private bool agregarVisible = false;
        public bool AgregarVisible
        {
            get { return agregarVisible; }
            set { agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }

        private bool editarVisible = false;
        public bool EditarVisible
        {
            get { return editarVisible; }
            set { editarVisible = value; OnPropertyChanged("EditarVisible"); }
        }
 
        private bool nuevoVisible =  false;
        public bool NuevoVisible
        {
            get { return nuevoVisible; }
            set { nuevoVisible = value; OnPropertyChanged("NuevoVisible"); }
        }
        #endregion

        #region Permisos
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}