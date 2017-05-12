using MVVMShared.ViewModels;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GESAL.Clases.Extendidas;
using GESAL.Clases.Misc;
using MVVMShared.Converters;
using GESAL.Clases.ExtensionesClases;
using System.Windows.Controls;
using System.Windows.Data;
namespace GESAL.ViewModels
{
    public partial class EntradasAlmacenesViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        #region Propiedades para habilitar controles
        private bool habilitarAsignarLotesOk = false;
        public bool HabilitarAsignarLotesOk
        {
            get { return habilitarAsignarLotesOk; }
            set { habilitarAsignarLotesOk = value; RaisePropertyChanged("HabilitarAsignarLotesOk"); }
        }

        private bool habilitarMunicipio = true;
        public bool HabilitarMunicipio
        {
            get { return habilitarMunicipio; }
            set { habilitarMunicipio = value; RaisePropertyChanged("HabilitarMunicipio"); }
        }

        private bool habilitarCentro = true;
        public bool HabilitarCentro
        {
            get { return habilitarCentro; }
            set { habilitarCentro = value; RaisePropertyChanged("HabilitarCentro"); }
        }
        private bool habilitarAlmacen_Tipo_Cat = true;
        public bool HabilitarAlmacen_Tipo_Cat
        {
            get { return habilitarAlmacen_Tipo_Cat; }
            set { habilitarAlmacen_Tipo_Cat = value; RaisePropertyChanged("HabilitarAlmacen_Tipo_Cat"); }
        }
        private bool habilitarAlmacenPrincipal = true;
        public bool HabilitarAlmacenPrincipal
        {
            get { return habilitarAlmacenPrincipal; }
            set { habilitarAlmacenPrincipal = value; RaisePropertyChanged("HabilitarAlmacenPrincipal"); }
        }
        #endregion
        #region Filtros
        #region Propiedades Municipios
        private ObservableCollection<MUNICIPIO> municipios = null;
        public ObservableCollection<MUNICIPIO> Municipios
        {
            get { return municipios; }
            set { municipios = value; RaisePropertyChanged("Municipios"); }
        }
        private MUNICIPIO selectedMunicipio = null;
        public MUNICIPIO SelectedMunicipio
        {
            get { return selectedMunicipio; }
            set
            {
                selectedMunicipio = value;
                RaisePropertyChanged("SelectedMunicipio");
                if (value != null)
                    MunicipioCambio(value.ID_MUNICIPIO);
            }
        }
        #endregion

        #region Propiedades Centros
        private ObservableCollection<CENTRO> centros = null;
        public ObservableCollection<CENTRO> Centros
        {
            get { return centros; }
            set { centros = value; RaisePropertyChanged("Centros"); }
        }
        private CENTRO selectedCentro = null;
        public CENTRO SelectedCentro
        {
            get { return selectedCentro; }
            set
            {
                selectedCentro = value;
                RaisePropertyChanged("SelectedCentro");
                if (value != null)
                    CentroCambio(value.ID_CENTRO);
            }
        }
        #endregion

        #region Propiedades Almacenes
        private ObservableCollection<ALMACEN> almacenesPrincipales = null;
        public ObservableCollection<ALMACEN> AlmacenesPrincipales
        {
            get { return almacenesPrincipales; }
            set { almacenesPrincipales = value; RaisePropertyChanged("AlmacenesPrincipales"); }
        }
        private ALMACEN selectedAlmacenPrincipal = null;
        public ALMACEN SelectedAlmacenPrincipal
        {
            get { return selectedAlmacenPrincipal; }
            set
            {
                selectedAlmacenPrincipal = value;
                RaisePropertyChanged("SelectedAlmacenPrincipal");
                if (value != null)
                    AlmacenPrincipalCambio(value);
            }
        }
        #endregion

        #region Propiedades Tipo de Almacen
        private ObservableCollection<ALMACEN_TIPO_CAT> almacen_Tipos_Cat = null;
        public ObservableCollection<ALMACEN_TIPO_CAT> Almacen_Tipos_Cat
        {
            get { return almacen_Tipos_Cat; }
            set { almacen_Tipos_Cat = value; RaisePropertyChanged("Almacen_Tipos_Cat"); }
        }

        private ALMACEN_TIPO_CAT selectedAlmacen_Tipo_Cat = null;
        public ALMACEN_TIPO_CAT SelectedAlmacen_Tipo_Cat
        {
            get { return selectedAlmacen_Tipo_Cat; }
            set { 
                selectedAlmacen_Tipo_Cat = value;
                RaisePropertyChanged("SelectedAlmacen_Tipo_Cat");
                if (value != null)
                    Almacen_TipoCambio(value.ID_ALMACEN_TIPO);
            }
        }

        #endregion
        #endregion

        #region Digitalizacion
        private bool guardarEscanerHabilitado = false;
        public bool GuardarEscanerHabilitado
        {
            get { return guardarEscanerHabilitado; }
            set { guardarEscanerHabilitado = value; RaisePropertyChanged("GuardarEscanerHabilitado"); }
        }

        private bool abrirImagenEscanerHabilitado = false;
        public bool AbrirImagenEscanerHabilitado
        {
            get { return abrirImagenEscanerHabilitado; }
            set { abrirImagenEscanerHabilitado = value; RaisePropertyChanged("AbrirImagenEscanerHabilitado"); }
        }

        private ObservableCollection<TipoDocumento> listTipoDocumento;
        public ObservableCollection<TipoDocumento> ListTipoDocumento
        {
            get { return listTipoDocumento; }
            set { listTipoDocumento = value; OnPropertyChanged("ListTipoDocumento"); }
        }

        TipoDocumento _SelectedTipoDocumento;
        public TipoDocumento SelectedTipoDocumento
        {
            get { return _SelectedTipoDocumento; }
            set
            {
                DocumentoDigitalizado = null;
                ObservacionDocumento = string.Empty;
                _SelectedTipoDocumento = value;
                OnPropertyChanged("SelectedTipoDocumento");
            }
        }

        private DateTime? datePickCapturaDocumento = DateTime.Now;
        public DateTime? DatePickCapturaDocumento
        {
            get { return datePickCapturaDocumento; }
            set { datePickCapturaDocumento = value; OnPropertyChanged("DatePickCapturaDocumento"); }
        }

        private string observacionDocumento;
        public string ObservacionDocumento
        {
            get { return observacionDocumento; }
            set { observacionDocumento = value; OnPropertyChanged("ObservacionDocumento"); }
        }

        private bool _AutoGuardado = true;
        public bool AutoGuardado
        {
            get { return _AutoGuardado; }
            set
            {
                _AutoGuardado = value;
                OnPropertyChanged("AutoGuardado");
            }
        }
        DigitalizarDocumento escaner = new DigitalizarDocumento();
        public byte[] DocumentoDigitalizado { get; set; }
            
        #endregion

        private byte[] facturaDigitalizado;
        public byte[] FacturaDigitalizado
        {
            get { return facturaDigitalizado; }
            set {
                facturaDigitalizado = value;
                OnPropertyValidateChanged("FacturaDigitalizado"); }
        }

        private DateTime fechaActual = DateTime.Now;
        public DateTime FechaActual
        {
            get { return fechaActual; }
            set { fechaActual = value; RaisePropertyChanged("FechaActual"); }
        }

        private ObservableCollection<ORDEN_COMPRA> ordenes_Calendarizadas = null;
        public ObservableCollection<ORDEN_COMPRA> Ordenes_Calendarizadas
        {
            get { return ordenes_Calendarizadas; }
            set { ordenes_Calendarizadas = value; RaisePropertyChanged("Ordenes_Calendarizadas"); }
        }

        private ObservableCollection<ORDEN_COMPRA> ordenes_Manual = null;
        public ObservableCollection<ORDEN_COMPRA> Ordenes_Manual
        {
            get { return ordenes_Manual; }
            set { ordenes_Manual = value;
                Ordenes_ManualCopy = value;
                RaisePropertyChanged("Ordenes_Manual"); }
        }

        private ObservableCollection<ORDEN_COMPRA> ordenes_ManualCopy = null;
        public ObservableCollection<ORDEN_COMPRA> Ordenes_ManualCopy
        {
            get { return ordenes_ManualCopy; }
            set { ordenes_ManualCopy = value; RaisePropertyChanged("Ordenes_ManualCopy"); }
        }

        private ORDEN_COMPRA selectedOrden_CalendarizadaOld = null;

        private ORDEN_COMPRA selectedOrden_Calendarizada = null;
        public ORDEN_COMPRA SelectedOrden_Calendarizada
        {
            get { return selectedOrden_Calendarizada; }
            set {
                if (selectedOrden_Calendarizada == null)
                    selectedOrden_CalendarizadaOld = value;
                else
                    selectedOrden_CalendarizadaOld = selectedOrden_Calendarizada;
                selectedOrden_Calendarizada = value;
                OnPropertyValidateChanged("SelectedOrden_Calendarizada");
                
                if (value != null)
                {
                    if (selectedOrden_CalendarizadaOld!=selectedOrden_Calendarizada)
                        ValidarCambioOnOrdenChanged();
                    else
                    {
                        HeaderProductoGrupo = "Forma de Recepción de Productos de la Orden " + SelectedOrden_Calendarizada.NUM_ORDEN;
                        IsGridValido = false;
                        IsRecepcionProductosVisible = true;
                        HabilitarAlmacen_Tipo_Cat = false;
                        HabilitarAlmacenPrincipal = false;
                        HabilitarCentro = false;
                        HabilitarMunicipio = false;
                        setValidationRulesEntradasAlmacen();
                        CargarProductos(SelectedAlmacenPrincipal.ID_ALMACEN, value.ID_ORDEN_COMPRA, FechaActual);
                    }
                }
                else
                {
                    Productos = null;
                    IsRecepcionProductosVisible = false;
                    HabilitarAlmacen_Tipo_Cat = true;
                    HabilitarAlmacenPrincipal = true;
                    HabilitarCentro = true;
                    HabilitarMunicipio = true;
                }
            }
        }

        private ObservableCollection<EXT_RECEPCION_PRODUCTOS> productos = null;
        public ObservableCollection<EXT_RECEPCION_PRODUCTOS> Productos
        {
            get { return productos; }
            set { productos = value; RaisePropertyChanged("Productos"); }
        }

        private EXT_RECEPCION_PRODUCTOS selectedProducto = null;
        public EXT_RECEPCION_PRODUCTOS SelectedProducto
        {
            get { return selectedProducto; }
            set { selectedProducto = value; RaisePropertyChanged("SelectedProducto"); }
        }

        private decimal? recibido ;
        public decimal? Recibido
        {
            get { return recibido; }
            set { recibido = value; RaisePropertyChanged("Recibido");
                if (value!=null)
                {
                    if (value.Value > 0 && value.Value <= Restante)
                        IsRecibidoValid = true;
                    else
                        IsRecibidoValid = false;
                }
                else
                {
                    IsRecibidoValid = false;
                }
            }
        }

        private int? lote = null;
        public int? Lote
        {
            get { return lote; }
            set { lote = value; RaisePropertyChanged("Lote"); }
        }

        private DateTime? fecha_Caducidad = null;
        public DateTime? Fecha_Caducidad
        {
            get { return fecha_Caducidad; }
            set {
                fecha_Caducidad = value;
                RaisePropertyChanged("Fecha_Caducidad");
                ValidarFechaCaducidad();
            }
        }

        private List<EXT_RECEPCION_PRODUCTO_DETALLE> recepcion_Producto_Detalle=null;
        public List<EXT_RECEPCION_PRODUCTO_DETALLE> Recepcion_Producto_Detalle
        {
            get { return recepcion_Producto_Detalle; }
            set { recepcion_Producto_Detalle = value; RaisePropertyChanged("Recepcion_Producto_Detalle"); }
        }

        private EXT_RECEPCION_PRODUCTO_DETALLE selectedRecepcion_Producto;
        public EXT_RECEPCION_PRODUCTO_DETALLE SelectedRecepcion_Producto
        {
            get { return selectedRecepcion_Producto; }
            set { selectedRecepcion_Producto = value; RaisePropertyChanged("SelectedRecepcion_Producto"); }
        }

        private decimal restante;
        public decimal Restante
        {
            get { return restante; }
            set { restante = value; RaisePropertyChanged("Restante"); }
        }
        private  bool isRecibidoValid = false;
        public bool IsRecibidoValid
        {
            get { return isRecibidoValid; }
            set { isRecibidoValid = value; RaisePropertyChanged("IsRecibidoValid"); }
        }

        private bool isGridValido = true;
        public bool IsGridValido
        {
            get { return isGridValido; }
            set { isGridValido = value; RaisePropertyChanged("IsGridValido"); }
        }

        private bool isRecepcionProductosVisible = false;
        public bool IsRecepcionProductosVisible
        {
            get { return isRecepcionProductosVisible; }
            set { isRecepcionProductosVisible = value; RaisePropertyChanged("IsRecepcionProductosVisible"); }
        }

        private string headerProductoGrupo = string.Empty;
        public string HeaderProductoGrupo
        {
            get { return headerProductoGrupo; }
            set { headerProductoGrupo = value; RaisePropertyChanged("HeaderProductoGrupo"); }
        }

        private string busquedaParametro = string.Empty;
        public string BusquedaParametro
        {
            get { return busquedaParametro; }
            set { busquedaParametro = value; RaisePropertyChanged("BusquedaParametro"); }
        }

        private string busquedaParametroManual = string.Empty;
        public string BusquedaParametroManual
        {
            get { return busquedaParametroManual; }
            set { 
                busquedaParametroManual = value;
                RaisePropertyChanged("BusquedaParametroManual");
            }
        }


        private int selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set { selectedTabIndex = value;
                RaisePropertyChanged("SelectedTabIndex");
                SelectedTabIndexChanged();
            }
        }

        private bool isFechaCaducidadValid = true;
        public bool IsFechaCaducidadValid
        {
            get { return isFechaCaducidadValid; }
            set { isFechaCaducidadValid = value; RaisePropertyChanged("IsFechaCaducidadValid"); }
        }
        private string mensajeFechaCaducidad = string.Empty;
        public string MensajeFechaCaducidad
        {
            get { return mensajeFechaCaducidad; }
            set { mensajeFechaCaducidad = value; RaisePropertyChanged("MensajeFechaCaducidad"); }
        }

        private int selectedTabindexOrdenesOld=0;
        private int selectedTabindexOrdenes = 0;
        public int SelectedTabindexOrdenes
        {
            get { return selectedTabindexOrdenes; }
            set {
                selectedTabindexOrdenesOld = selectedTabindexOrdenes;
                selectedTabindexOrdenes = value;
                RaisePropertyChanged("SelectedTabindexOrdenes");                
            }
        }

        private int? fechaCaducidadMinimaMedicamento = 0;

        #region Rechazar Producto PopUp
        private bool isFechaValid = true;
        public bool IsFechaValid
        {
            get { return isFechaValid; }
            set { isFechaValid = value; RaisePropertyChanged("IsFechaValid"); }
        }

        private string encabezadoIncidenciaProducto = string.Empty;
        public string EncabezadoIncidenciaProducto
        {
            get { return encabezadoIncidenciaProducto; }
            set { encabezadoIncidenciaProducto = value; RaisePropertyChanged("EncabezadoIncidenciaProducto"); }
        }

        private ObservableCollection<INCIDENCIA_TIPO> incidencia_Tipos;
        public ObservableCollection<INCIDENCIA_TIPO> Incidencia_Tipos
        {
            get { return incidencia_Tipos; }
            set { incidencia_Tipos = value; RaisePropertyChanged("Incidencia_Tipos"); }
        }

        private INCIDENCIA_TIPO selectedIncidencia_Tipo;
        public INCIDENCIA_TIPO SelectedIncidencia_Tipo
        {
            get { return selectedIncidencia_Tipo; }
            set { selectedIncidencia_Tipo = value; RaisePropertyChanged("SelectedIncidencia_Tipo"); }
        }

        private string observacion_Incidencia=string.Empty;
        public string Observacion_Incidencia
        { get { return observacion_Incidencia; }
            set { observacion_Incidencia = value; RaisePropertyChanged("Observacion_Incidencia"); }
        }

        private bool isRechazo_Entrada_Valido = false;
        public bool IsRechazo_Entrada_Valido
        {
            get { return isRechazo_Entrada_Valido; }
            set { isRechazo_Entrada_Valido = value; RaisePropertyChanged("IsRechazo_Entrada_Valido"); }
        }

        private EXT_RECEPCION_PRODUCTOS producto_Rechazar = null;
        public EXT_RECEPCION_PRODUCTOS Producto_Rechazar
        { get { return producto_Rechazar; }
            set { producto_Rechazar = value; RaisePropertyChanged("Producto_Rechazar"); }
        }

        private bool isRecalendarizarVisible = true;
        public bool IsRecalendarizarVisible
        {
            get { return isRecalendarizarVisible; }
            set { isRecalendarizarVisible = value; RaisePropertyChanged("IsRecalendarizarVisible"); }
        }

        private bool isCapturarIncidenciaVisible = true;
        public bool IsCapturarIncidenciaVisible
        {
            get { return isCapturarIncidenciaVisible; }
            set { isCapturarIncidenciaVisible = value; RaisePropertyChanged("IsCapturarIncidenciaVisible"); }
        }

        private List<DateTime> selectedFechasEntregaProveedor = new List<DateTime>();
        public List<DateTime> SelectedFechasEntregaProveedor
        {
            get { return selectedFechasEntregaProveedor; }
            set { selectedFechasEntregaProveedor = value; RaisePropertyChanged("SelectedFechasEntregaProveedor"); }
        }

        private DateTime[] selectedFechasEntregaProveedorCopia;

        private DateTime recalendarizarDisplayDate=DateTime.Now;
        public DateTime RecalendarizarDisplayDate
        {
            get { return recalendarizarDisplayDate; }
            set { recalendarizarDisplayDate = value; 
                RaisePropertyChanged("RecalendarizarDisplayDate");
                RecargarFechas();
            }
        }

        private DateTime? fechaRecalidarizarProductoRechazado = null;
        public DateTime? FechaRecalidarizarProductoRechazado
        {
            get { return fechaRecalidarizarProductoRechazado; }
            set { fechaRecalidarizarProductoRechazado = value; RaisePropertyChanged("FechaRecalidarizarProductoRechazado"); }
        }

        private bool provieneDeEntradaLotes = false;

        #endregion

        private List<KeyValuePair<DataGridRow, EventHandler<DataTransferEventArgs>>> listaeventos = null;
    }
}
