using MVVMShared.ViewModels;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GESAL.Clases.Extendidas;
using SSP.Servidor.ModelosExtendidos;
using GESAL.Models;

namespace GESAL.ViewModels
{
    public partial class RequisicionExtraordinariaPrincipalViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        #region Traspaso Externo
        private bool isBusquedaProductosTraspasosVisible=false;
        public bool IsBusquedaProductosTraspasosVisible
        {
            get {return isBusquedaProductosTraspasosVisible;}
            set {isBusquedaProductosTraspasosVisible=value;RaisePropertyChanged("IsBusquedaProductosTraspasosVisible");}
        }

        private string busquedaFolioRequisicionTraspaso=string.Empty;
        public string BusquedaFolioRequisicionTraspaso
        {
            get{return busquedaFolioRequisicionTraspaso;}
            set{busquedaFolioRequisicionTraspaso=value;RaisePropertyChanged("BusquedaFolioRequisicionTraspaso");}
        }

        private bool isAlmacenOrigenVisible = false;
        public bool IsAlmacenOrigenVisible
        {
            get { return isAlmacenOrigenVisible; }
            set { isAlmacenOrigenVisible = value; RaisePropertyChanged("IsAlmacenOrigenVisible"); }
        }

        private ObservableCollection<EXT_V_INVENTARIO_PRODUCTO> inventario_productos;
        public ObservableCollection<EXT_V_INVENTARIO_PRODUCTO> Inventario_Productos
        {
            get { return inventario_productos; }
            set { Inventario_Productos = value; RaisePropertyChanged("Inventario_Productos"); }
        }

        private EXT_V_INVENTARIO_PRODUCTO selectedInventario_Producto;
        public EXT_V_INVENTARIO_PRODUCTO SelectedInventario_Producto
        {
            get { return selectedInventario_Producto; }
            set { selectedInventario_Producto = value; RaisePropertyChanged("SelectedInventario_Producto"); }
        }

        private List<EXT_V_INVENTARIO_PRODUCTO> seleccionadosInventario_Producto;

        private ObservableCollection<EXT_PRODUCTO_TRASPASO_EXTERNO> productosTraspasoExterno;
        public ObservableCollection<EXT_PRODUCTO_TRASPASO_EXTERNO> ProductosTraspasoExterno
        {
            get { return productosTraspasoExterno; }
            set
            {
                productosTraspasoExterno = value;
                if (value != null && value.Count > 0)
                    OnPropertyValidateChanged("ProductosTraspasoExterno");
                else
                {
                    RaisePropertyChanged("ProductosTraspasoExterno");
                    StaticSourcesViewModel.SourceChanged = false;
                }
            }
        }

        private EXT_PRODUCTO_TRASPASO_EXTERNO selectedProductoTraspasoExterno;
        public EXT_PRODUCTO_TRASPASO_EXTERNO SelectedProductoTraspasoExterno
        {
            get { return selectedProductoTraspasoExterno; }
            set { selectedProductoTraspasoExterno = value; RaisePropertyChanged("SelectedProductoTraspasoExterno"); }
        }

        private string buscarProductoTraspaso;

        public string BuscarProductoTraspaso
        {
            get { return buscarProductoTraspaso; }
            set { buscarProductoTraspaso = value; RaisePropertyChanged("BuscarProductoTraspaso"); }
        }

        private bool isTraspasoExternoValido = false;
        public bool IsTraspasoExternoValido
        {
            get { return isTraspasoExternoValido; }
            set { isTraspasoExternoValido = value; RaisePropertyChanged("IsTraspasoExternoValido"); }
        }

        private string mensajeErrorTraspasoExterno = string.Empty;
        public string MensajeErrorTraspasoExterno
        {
            get { return mensajeErrorTraspasoExterno; }
            set { mensajeErrorTraspasoExterno = value; RaisePropertyChanged("MensajeErrorTraspasoExterno"); }
        }

        #region Propiedades Filtro Origen

        private List<TipoBusquedaAlmacen> tiposBusqueda = new TiposBusquedaAlmacen().LISTA_TIPOBUSQUEDAALMACEN;

        public List<TipoBusquedaAlmacen> TiposBusqueda
        {
            get { return tiposBusqueda; }
        }

        private TipoBusquedaAlmacen selectedTipoBusqueda;
        public TipoBusquedaAlmacen SelectedTipoBusqueda
        {
            get { return selectedTipoBusqueda; }
            set { selectedTipoBusqueda = value; RaisePropertyChanged("SelectedTipoBusqueda"); }
        }

        private ObservableCollection<MUNICIPIO> municipiosOrigen;
        public ObservableCollection<MUNICIPIO> MunicipiosOrigen
        {
            get { return municipiosOrigen; }
            set { municipiosOrigen = value; }
        }

        private short selectedMunicipioOrigenValue=-1;
        public short SelectedMunicipioOrigenValue
        {
            get { return selectedMunicipioOrigenValue; }
            set
            {
                selectedMunicipioOrigenValue = value;
                RaisePropertyChanged("SelectedMunicipioOrigenValue");
            }
        }

        private ObservableCollection<CENTRO> centrosOrigen;
        public ObservableCollection<CENTRO> CentrosOrigen
        {
            get { return centrosOrigen; }
            set { centrosOrigen = value; RaisePropertyChanged("CentrosOrigen"); }
        }

        private short selectedCentroOrigenValue = -1;
        public short SelectedCentroOrigenValue
        {
            get { return selectedCentroOrigenValue; }
            set { selectedCentroOrigenValue = value; RaisePropertyChanged("SelectedCentroOrigenValue"); }
        }

        private ObservableCollection<ALMACEN_TIPO_CAT> almacen_Tipos_CatOrigen;
        public ObservableCollection<ALMACEN_TIPO_CAT> Almacen_Tipos_CatOrigen
        {
            get { return almacen_Tipos_CatOrigen; }
            set { almacen_Tipos_CatOrigen = value; RaisePropertyChanged("Almacen_Tipos_CatOrigen"); }
        }

        private short selectedAlmacen_Tipo_CatOrigenValue = -1;
        public short SelectedAlmacen_Tipo_CatOrigenValue
        {
            get { return selectedAlmacen_Tipo_CatOrigenValue; }
            set { selectedAlmacen_Tipo_CatOrigenValue = value; RaisePropertyChanged("SelectedAlmacen_Tipo_CatOrigenValue"); }
        }

        private ObservableCollection<ALMACEN> almacenesPrincipalesOrigen;
        public ObservableCollection<ALMACEN> AlmacenesPrincipalesOrigen
        {
            get { return almacenesPrincipalesOrigen; }
            set { almacenesPrincipalesOrigen = value; RaisePropertyChanged("AlmacenesPrincipalesOrigen"); }
        }

        private short selectedAlmacenPrincipalOrigenValue = -1;
        public short SelectedAlmacenPrincipalOrigenValue
        {
            get { return selectedAlmacenPrincipalOrigenValue; }
            set { selectedAlmacenPrincipalOrigenValue = value; RaisePropertyChanged("SelectedAlmacenPrincipalOrigenValue");}
        }
        #endregion

        #endregion

        #region Requisicion Extraordinaria
        #region Propiedades para Habilitar Controles
        private bool isRequisicionHabilitado = false;
        public bool IsRequisicionHabilitado
        {
            get { return isRequisicionHabilitado; }
            set { isRequisicionHabilitado = value; RaisePropertyChanged("IsRequisicionHabilitado"); }
        }

        private bool isDatosCentroRequisicionHabilitado = false;
        public bool IsDatosCentroRequisicionHabilitado
        {
            get { return isDatosCentroRequisicionHabilitado; }
            set { isDatosCentroRequisicionHabilitado = value; RaisePropertyChanged("IsDatosCentroRequisicionHabilitado"); }
        }
        #endregion
        #region Filtros Requisicion

        private ObservableCollection<MUNICIPIO> municipios;
        public ObservableCollection<MUNICIPIO> Municipios
        {
            get { return municipios; }
            set { municipios = value; }
        }

        private short selectedMunicipioOldValue = -1;
        private short selectedMunicipioValue=-1;
        public short SelectedMunicipioValue
        {
            get { return selectedMunicipioValue; }
            set
            {
                selectedMunicipioOldValue = selectedMunicipioValue;
                selectedMunicipioValue = value;
                RaisePropertyChanged("SelectedMunicipioValue");
                //MunicipioCambio(value);
            }
        }
        private ObservableCollection<CENTRO> centros;
        public ObservableCollection<CENTRO> Centros
        {
            get { return centros; }
            set { centros = value; RaisePropertyChanged("Centros"); }
        }

        private short selectedCentroOldValue = -1;
        private short selectedCentroValue=-1;
        public short SelectedCentroValue
        {
            get { return selectedCentroValue; }
            set
            {
                selectedCentroOldValue = selectedCentroValue;
                selectedCentroValue = value;
                RaisePropertyChanged("SelectedCentroValue");
            }
        }

        private ObservableCollection<ALMACEN> almacenesPrincipales;
        public ObservableCollection<ALMACEN> AlmacenesPrincipales
        {
            get { return almacenesPrincipales; }
            set { almacenesPrincipales = value; RaisePropertyChanged("AlmacenesPrincipales"); }
        }

        private short selectedAlmacenPrincipalOldValue = -1;
        private short selectedAlmacenPrincipalValue=-1;
        public short SelectedAlmacenPrincipalValue
        {
            get { return selectedAlmacenPrincipalValue; }
            set 
            {
                selectedAlmacenPrincipalOldValue = selectedAlmacenPrincipalValue;
                selectedAlmacenPrincipalValue = value;
                RaisePropertyChanged("SelectedAlmacenPrincipalValue");
                //AlmacenPrincipal_Cambio(value);
            }
        }

        private ObservableCollection<ALMACEN_TIPO_CAT> almacen_Tipos_Cat;
        public ObservableCollection<ALMACEN_TIPO_CAT> Almacen_Tipos_Cat
        {
            get { return almacen_Tipos_Cat; }
            set { almacen_Tipos_Cat = value; RaisePropertyChanged("Almacen_Tipos_Cat"); }
        }

        private short selectedAlmacen_Tipo_CatOldValue = -1;
        private short selectedAlmacen_Tipo_CatValue=-1;
        public short SelectedAlmacen_Tipo_CatValue
        {
            get { return selectedAlmacen_Tipo_CatValue; }
            set
            {
                selectedAlmacen_Tipo_CatOldValue = selectedAlmacen_Tipo_CatValue;
                selectedAlmacen_Tipo_CatValue = value;
                RaisePropertyChanged("SelectedAlmacen_Tipo_CatValue");
                //Almacen_TipoCambio(value);
            }
        }

        #endregion

        private List<EXT_V_INVENTARIO_PRODUCTO> productos;
        public List<EXT_V_INVENTARIO_PRODUCTO> Productos
        {
            get { return productos; }
            set { productos = value; RaisePropertyChanged("Productos"); }
        }

        private List<EXT_V_INVENTARIO_PRODUCTO> seleccionadosProductos;


        private EXT_V_INVENTARIO_PRODUCTO selectedProducto;
        public EXT_V_INVENTARIO_PRODUCTO SelectedProducto
        {
            get { return selectedProducto; }
            set { selectedProducto = value; RaisePropertyChanged("SelectedProducto"); }
        }

        private string buscarProducto = string.Empty;
        public string BuscarProducto
        {
            get { return buscarProducto; }
            set { buscarProducto = value; RaisePropertyChanged("BuscarProducto"); }
        }

        private ObservableCollection<EXT_PRODUCTO_REQUISICION> productosRequisicion;
        public ObservableCollection<EXT_PRODUCTO_REQUISICION> ProductosRequisicion
        {
            get { return productosRequisicion; }
            set 
            { 
                productosRequisicion = value;
                if (value != null && value.Count > 0)
                    OnPropertyValidateChanged("ProductosRequisicion");
                else
                {
                    RaisePropertyChanged("ProductosRequisicion");
                    StaticSourcesViewModel.SourceChanged = false;
                }
            }
        }

        private EXT_PRODUCTO_REQUISICION selectedProductoRequisicion;
        public EXT_PRODUCTO_REQUISICION SelectedProductoRequisicion
        {
            get { return selectedProductoRequisicion; }
            set { selectedProductoRequisicion = value; RaisePropertyChanged("SelectedProductoRequisicion"); }
        }

        private bool isProductosRequisicionValido = false;
        public bool IsProductosRequisicionValido
        {
            get { return isProductosRequisicionValido; }
            set { isProductosRequisicionValido = value; RaisePropertyChanged("IsProductosRequisicionValido"); }
        }

        private bool isCantidadesRequeridasValidas = false;
        public bool IsCantidadesRequeridasValidas
        {
            get { return isCantidadesRequeridasValidas; }
            set { isCantidadesRequeridasValidas = value; RaisePropertyChanged("IsCantidadesRequeridasValidas"); }
        }

        private ObservableCollection<REQUISICION_CENTRO> requisicionesExtraordinarias;
        public ObservableCollection<REQUISICION_CENTRO> RequisicionesExtraordinarias
        {
            get { return requisicionesExtraordinarias; }
            set { requisicionesExtraordinarias = value; RaisePropertyChanged("RequisicionesExtraordinarias"); }
        }

        private REQUISICION_CENTRO selectedRequisicionExtraordinariaPop_Up;
        public REQUISICION_CENTRO SelectedRequisicionExtraordinariaPop_Up
        {
            get { return selectedRequisicionExtraordinariaPop_Up; }
            set { selectedRequisicionExtraordinariaPop_Up = value; RaisePropertyChanged("SelectedRequisicionExtraordinariaPop_Up"); }
        }

        private string tituloDatosRequisicion = string.Empty;
        public string TituloDatosRequisicion
        {
            get { return tituloDatosRequisicion; }
            set { tituloDatosRequisicion = value; RaisePropertyChanged("TituloDatosRequisicion"); }
        }

        #endregion

        #region Propiedades para Habilitar Comandos de Menu
        private bool agregarHabilitado = false;
        public bool AgregarHabilitado
        {
            get { return agregarHabilitado; }
            set { agregarHabilitado = value; RaisePropertyChanged("AgregarHabilitado"); }
        }

        private bool cancelarHabilitado = false;
        public bool CancelarHabilitado
        {
            get { return cancelarHabilitado; }
            set { cancelarHabilitado = value; RaisePropertyChanged("CancelarHabilitado"); }
        }

        private bool salvarHabilitado = false;
        public bool SalvarHabilitado
        {
            get { return salvarHabilitado; }
            set { salvarHabilitado = value; RaisePropertyChanged("SalvarHabilitado"); }
        }

        private bool editarHabilitado = false;
        public bool EditarHabilitado
        {
            get { return editarHabilitado; }
            set { editarHabilitado = value; RaisePropertyChanged("EditarHabilitado"); }
        }

        private bool eliminarHabilitado = false;
        public bool EliminarHabilitado
        { 
            get { return eliminarHabilitado; }
            set { eliminarHabilitado = value; RaisePropertyChanged("EliminarHabilitado"); }
        }
        private bool imprimirHabilitado = false;
        public bool ImprimirHabilitado
        {
            get { return imprimirHabilitado; }
            set { imprimirHabilitado = value; RaisePropertyChanged("ImprimirHabilitado"); }
        }
        #endregion

        #region Propiedades para Habilitar Tab Items
        private bool isTraspasoExternoHabilitado = false;
        public bool IsTraspasoExternoHabilitado
        {
            get { return isTraspasoExternoHabilitado; }
            set { isTraspasoExternoHabilitado = value; RaisePropertyChanged("IsTraspasoExternoHabilitado"); }
        }

        private bool isOCExtraordinariaHabilitado = false;
        public bool IsOCExtraordinariaHabilitado
        {
            get { return isOCExtraordinariaHabilitado; }
            set { isOCExtraordinariaHabilitado = value; RaisePropertyChanged("IsOCExtraordinariaHabilitado"); }
        }
        #endregion

        private int selectedTabIndexTipoOperacion;
        public int SelectedTabIndexTipoOperacion
        {
            get { return selectedTabIndexTipoOperacion; }
            set { selectedTabIndexTipoOperacion = value; RaisePropertyChanged("SelectedTabIndexTipoOperacion"); }
        }

        private REQUISICION_CENTRO selectedRequisicionExtraordinaria = null;
        public REQUISICION_CENTRO SelectedRequisicionExtraordinaria
        {
            get { return selectedRequisicionExtraordinaria; }
            set { selectedRequisicionExtraordinaria = value; RaisePropertyChanged("SelectedRequisicionExtraordinaria"); }
        }
    }
}
