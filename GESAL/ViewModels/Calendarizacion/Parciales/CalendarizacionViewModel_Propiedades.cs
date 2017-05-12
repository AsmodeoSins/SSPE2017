using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GESAL.Clases.Misc;
using System.Collections.ObjectModel;
using SSP.Servidor;
using SSP.Servidor.ModelosExtendidos;
using GESAL.Clases.Extendidas;
using GESAL.Clases.Enums;
namespace GESAL.ViewModels
{
    public partial class CalendarizacionViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        #region Propiedades

        #region Propiedades para bindear con Control calendario
        private string bindCmdDayClick = string.Empty;
        public string BindCmdDayClick
        {
            get { return bindCmdDayClick; }
            set { bindCmdDayClick = value; RaisePropertyChanged("BindCmdDayClick"); }
        }

        private string bindSelectedMesProperty = string.Empty;
        public string BindSelectedMesProperty
        {
            get { return bindSelectedMesProperty; }
            set { bindSelectedMesProperty = value; RaisePropertyChanged("BindSelectedMesProperty"); }
        }

        private string bindAnioMinimoProperty = string.Empty;
        public string BindAnioMinimoProperty
        {
            get { return bindAnioMinimoProperty; }
            set { bindAnioMinimoProperty = value; RaisePropertyChanged("BindAnioMinimoProperty"); }
        }

        private string bindAnioMaximoProperty = string.Empty;
        public string BindAnioMaximoProperty
        {
            get { return bindAnioMaximoProperty; }
            set { bindAnioMaximoProperty = value; RaisePropertyChanged("BindAnioMaximoProperty"); }
        }

        private string bindSelectedAnioProperty = string.Empty;
        public string BindSelectedAnioProperty
        {
            get { return bindSelectedAnioProperty; }
            set { bindSelectedAnioProperty = value; RaisePropertyChanged("BindSelectedAnioProperty"); }
        }

        private string bindDiasAgendadosProperty = string.Empty;
        public string BindDiasAgendadosProperty
        {
            get { return bindDiasAgendadosProperty; }
            set { bindDiasAgendadosProperty = value; RaisePropertyChanged("BindDiasAgendadosProperty"); }
        }


        #endregion

        #region Propiedades Calendario
        private int selectedMes = DateTime.Now.Month;
        public int SelectedMes
        {
            get { return selectedMes; }
            set { selectedMes = value;
            if (value > 0)
                if (SelectedAlmacen != null)
                    SeleccionarFechasAgenda(SelectedAlmacen.ID_ALMACEN, value, SelectedAnio);
                else
                    DiasAgendados = new ObservableCollection<DateTime>();
            RaisePropertyChanged("SelectedMes");
            }
        }

        private int anioMinimo = DateTime.Now.Year;
        public int AnioMinimo
        {
            get { return anioMinimo; }
            set { anioMinimo = value; RaisePropertyChanged("AnioMinimo"); }
        }


        private int anioMaximo = DateTime.Now.Year;
        public int AnioMaximo
        {
            get { return anioMaximo; }
            set { anioMaximo = value; RaisePropertyChanged("AnioMaximo"); }
        }

        private int selectedAnio = DateTime.Now.Year;
        public int SelectedAnio
        {
            get { return selectedAnio; }
            set { selectedAnio = value;
            if (value > 0)
                if(SelectedAlmacen!=null)
                    SeleccionarFechasAgenda(SelectedAlmacen.ID_ALMACEN, SelectedMes, value);
                else
                    DiasAgendados = new ObservableCollection<DateTime>();
            RaisePropertyChanged("SelectedAnio"); }
        }

        private ObservableCollection<DateTime> diasAgendados = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> DiasAgendados
        {
            get { return diasAgendados; }
            set { diasAgendados = value; RaisePropertyChanged("DiasAgendados"); }
        }
        #endregion

        #region Propiedades Agenda

        #region Habilitar Acciones de Agenda
        private bool eliminarHabilitado = false;
        public bool EliminarHabilitado
        {
            get { return eliminarHabilitado; }
            set { eliminarHabilitado = value; RaisePropertyChanged("EliminarHabilitado"); }
        }

        private bool salvarHabilitado = false;
        public bool SalvarHabilitado
        {
            get { return salvarHabilitado; }
            set { salvarHabilitado = value; RaisePropertyChanged("SalvarHabilitado"); }
        }

        private bool recalendarizarHabilitado = false;
        public bool RecalendarizarHabilitado
        {
            get { return recalendarizarHabilitado; }
            set { recalendarizarHabilitado = value; RaisePropertyChanged("RecalendarizarHabilitado"); }
        }

        #endregion

        #region Accion del Boton Salvar
        private AccionSalvar realizarAccion;
        public AccionSalvar RealizarAccion
        {
            get { return realizarAccion; }
            set { realizarAccion = value; }
        }
        #endregion
        
        private DateTime? fechaAgenda;
        public DateTime? FechaAgenda
        {
            get { return fechaAgenda; }
            set {
                if (value == null)
                    fechaAgenda = fechaAgendaOriginal;
                else
                    fechaAgenda = value;
                if (fechaAgenda == FechaAgendaOriginal && RealizarAccion == AccionSalvar.Actualizar)
                    EliminarHabilitado = true;
                else
                    EliminarHabilitado = false;
                RaisePropertyChanged("FechaAgenda"); }
        }

        private DateTime? fechaAgendaOriginal;
        public DateTime? FechaAgendaOriginal
        {
            get { return fechaAgendaOriginal; }
            set { fechaAgendaOriginal = value; RaisePropertyChanged("FechaAgendaOriginal"); }
        }

        private bool isProductosAgendaVisible = false;
        public bool IsProductosAgendaVisible
        { 
            get { return isProductosAgendaVisible; }
            set { isProductosAgendaVisible = value; RaisePropertyChanged("IsProductosAgendaVisible"); }
        }

        private bool isParametrosBusquedaEnabled = true;
        public bool IsParametrosBusquedaEnabled
        {
            get { return isParametrosBusquedaEnabled; }
            set { isParametrosBusquedaEnabled = value; RaisePropertyChanged("IsParametrosBusquedaEnabled"); }
        }

        private bool isProgramadosOK = false;
        public bool IsProgramadosOK
        {
            get { return isProgramadosOK; }
            set { isProgramadosOK = value; RaisePropertyChanged("IsProgramadosOK"); }
        }

        private DateTime fechaActual = DateTime.Now.Date;
        public DateTime FechaActual
        {
            get { return fechaActual; }
            set { fechaActual = value; RaisePropertyChanged("FechaActual"); }
        }

        private bool isFechaAgendaEnabled = false;
        public bool IsFechaAgendaEnabled
        {
            get { return isFechaAgendaEnabled; }
            set { isFechaAgendaEnabled = value; RaisePropertyChanged("IsFechaAgendaEnabled"); }
        }
        #endregion

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
            set {
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
            set { 
                selectedCentro = value;
                RaisePropertyChanged("SelectedCentro");
                if (value != null)
                    CentroCambio(value.ID_CENTRO);
            }
        }
        #endregion

        #region Propiedades Almacenes
        private ObservableCollection<ALMACEN> almacenes = null;
        public ObservableCollection<ALMACEN> Almacenes
        {
            get { return almacenes; }
            set { almacenes = value; RaisePropertyChanged("Almacenes"); }
        }
        private ALMACEN selectedAlmacen = null;
        public ALMACEN SelectedAlmacen
        {
            get { return selectedAlmacen; }
            set {
                selectedAlmacen = value;
                if (value != null)
                    SeleccionarFechasAgenda(value.ID_ALMACEN, SelectedMes, SelectedAnio);
                RaisePropertyChanged("SelectedAlmacen"); }
        }
        #endregion

        #region Propiedades de Orden de Compra
        private string buscarOrdenParametro = null;
        public string BuscarOrdenParametro
        {
            get { return buscarOrdenParametro; }
            set { buscarOrdenParametro = value; RaisePropertyChanged("BuscarOrdenParametro"); }
        }

        private ObservableCollection<ORDEN_COMPRA> ordenes_Compra = null;
        public ObservableCollection<ORDEN_COMPRA> Ordenes_Compra
        { get { return ordenes_Compra; }
            set { ordenes_Compra = value; RaisePropertyChanged("Ordenes_Compra"); }
        }

        private ORDEN_COMPRA selectedOrden_CompraPopUp = null;
        public ORDEN_COMPRA SelectedOrden_CompraPopUp
        {
            get { return selectedOrden_CompraPopUp; }
            set { selectedOrden_CompraPopUp = value;
            if (value != null)
                CargarDetalleOrdenCompra(value);
                RaisePropertyChanged("SelectedOrden_CompraPopUp"); }
        }

        private ObservableCollection<EXT_Orden_Compra_Detalle_Transito> ordenes_Compra_Detalle = null;
        public ObservableCollection<EXT_Orden_Compra_Detalle_Transito> Ordenes_Compra_Detalle
        {
            get { return ordenes_Compra_Detalle; }
            set { ordenes_Compra_Detalle = value; RaisePropertyChanged("Ordenes_Compra_Detalle"); }
        }

        private EXT_Orden_Compra_Detalle_Transito selectedOrden_Compra_DetallePopUp;
        public EXT_Orden_Compra_Detalle_Transito SelectedOrden_Compra_DetallePopUp
        {
            get { return selectedOrden_Compra_DetallePopUp; }
            set { selectedOrden_Compra_DetallePopUp = value; RaisePropertyChanged("SelectedOrden_Compra_DetallePopUp"); }
        }

        private ObservableCollection<EXT_Orden_Compra_Detalle_Transito> selectedOrden_Compra_Detalles=new ObservableCollection<EXT_Orden_Compra_Detalle_Transito>();
        public ObservableCollection<EXT_Orden_Compra_Detalle_Transito> SelectedOrden_Compra_Detalles
        {
            get { return selectedOrden_Compra_Detalles; }
            set { selectedOrden_Compra_Detalles = value; RaisePropertyChanged("SelectedOrden_Compra_Detalles"); }
        }

        private EXT_Orden_Compra_Detalle_Transito selectedOrden_Compra_Detalle=null;
        public EXT_Orden_Compra_Detalle_Transito SelectedOrden_Compra_Detalle
        {
            get { return selectedOrden_Compra_Detalle; }
            set { selectedOrden_Compra_Detalle = value; RaisePropertyChanged("SelectedOrden_Compra_Detalle"); }
        }

        #endregion

        #region Propiedades de Calendarizar Entrega
        private CALENDARIZAR_ENTREGA calendarizar_Entrega = null;
        public CALENDARIZAR_ENTREGA Calendarizar_Entrega
        {
            get { return calendarizar_Entrega; }
            set { calendarizar_Entrega = value; RaisePropertyChanged("Calendarizar_Entrega"); }
        }
        #endregion

        #region Rechazar Producto PopUp

        #region Acciones Rechazar Producto PopUp
        enum TipoAccionesPopUp
        {
            IncidenciaCancelar,
            IncidenciaRecalendarizar
        }

        private TipoAccionesPopUp tipoAccionPopUp;
            
        #endregion

        private bool isFechaValid = false;
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

        private string observacion_Incidencia = string.Empty;
        public string Observacion_Incidencia
        {
            get { return observacion_Incidencia; }
            set { observacion_Incidencia = value; RaisePropertyChanged("Observacion_Incidencia"); }
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

        private DateTime recalendarizarDisplayDate = DateTime.Now;
        public DateTime RecalendarizarDisplayDate
        {
            get { return recalendarizarDisplayDate; }
            set
            {
                recalendarizarDisplayDate = value;
                RaisePropertyChanged("RecalendarizarDisplayDate");
                RecargarFechas();
            }
        }

        private DateTime? fechaRecalidarizarProductoRechazado = null;
        public DateTime? FechaRecalidarizarProductoRechazado
        {
            get { return fechaRecalidarizarProductoRechazado; }
            set { 
                fechaRecalidarizarProductoRechazado = value;
                if (value.HasValue && value.Value.Month == FechaAgendaOriginal.Value.Month)
                    IsFechaValid = true;
                else
                    IsFechaValid = false;
                RaisePropertyChanged("FechaRecalidarizarProductoRechazado");

            }
        }

        #endregion
        #endregion
    }
}
