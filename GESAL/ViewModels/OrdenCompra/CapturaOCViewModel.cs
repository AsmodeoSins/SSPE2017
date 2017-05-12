using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.ViewModels;
using System.ComponentModel;
using MVVMShared.Commands;
using System.Windows.Input;
using GESAL.Models;
using GESAL;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Almacenes;
using SSP.Controlador.Principales.Almacenes;
using SSP.Servidor;
using SSP.Servidor.ModelosExtendidos;
using System.Threading.Tasks;
using GESAL.Clases.Enums;
using MahApps.Metro.Controls.Dialogs;
namespace GESAL.ViewModels
{
    public class CapturaOCViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;
       
        
        public CapturaOCViewModel(IDialogCoordinator dialogCoordinator, Usuario usuario)
        {
            _dialogCoordinator = dialogCoordinator;
            _usuario = usuario;
        }

        #region Enums

        public enum Tipo_Reglas
        {
            Salvar,
            BuscarOC,
            BuscarProveedor,
            BuscarRequisicion
        }

        private enum TipoBusqueda
        {
            Exacta,
            Aproximada
        }

        #endregion

        #region Propiedades

        #region Accion del Boton Salvar
        private AccionSalvar realizarAccion;
        public AccionSalvar RealizarAccion
        {
            get { return realizarAccion; }
            set { realizarAccion = value; }
        }
        #endregion

        #region Variables para habilitar/deshabilitar controles

        #region Propiedades Habilitar Acciones

        private bool editarHabilitado = false;
        public bool EditarHabilitado
        {
            get { return editarHabilitado; }
            set { editarHabilitado = value; RaisePropertyChanged("EditarHabilitado"); }
        }

        private bool cancelarHabilitado = false;
        public bool CancelarHabilitado
        {
            get { return cancelarHabilitado; }
            set { cancelarHabilitado = value; RaisePropertyChanged("CancelarHabilitado"); }
        }

        private bool eliminarHabilitado = false;
        public bool EliminarHabilitado
        {
            get { return eliminarHabilitado; }
            set { eliminarHabilitado = value; RaisePropertyChanged("EliminarHabilitado"); }
        }

        private bool salvarHabilitado = true;
        public bool SalvarHabilitado
        {
            get { return salvarHabilitado; }
            set { salvarHabilitado = value; RaisePropertyChanged("SalvarHabilitado"); }
        }
        #endregion

        private bool noOrdenIsReadOnly = false;
        public bool NoOrdenIsReadOnly
        {
            get { return noOrdenIsReadOnly; }
            set { noOrdenIsReadOnly = value; RaisePropertyChanged("NoOrdenIsReadOnly"); }
        }

        private bool isBloqueProveedorEnabled = true;

        public bool IsBloqueProveedorEnabled
        {
            get { return isBloqueProveedorEnabled; }
            set { isBloqueProveedorEnabled = value; RaisePropertyChanged("IsBloqueProveedorEnabled"); }
        }

        private bool isBloqueRequisicionEnabled = true;
        public bool IsBloqueRequisicionEnabled
        {
            get { return isBloqueRequisicionEnabled; }
            set { isBloqueRequisicionEnabled = value; RaisePropertyChanged("IsBloqueRequisicionEnabled"); }
        }

        private bool isFechaOCEnabled = true;
        public bool IsFechaOCEnabled
        {
            get { return isFechaOCEnabled; }
            set { isFechaOCEnabled = value; RaisePropertyChanged("IsFechaOCEnabled"); }
        }

        #endregion

        #region Propiedades de Proveedores
        private int? id_Proveedor;
        public int? ID_Proveedor
        {
            get { return id_Proveedor; }
            set { id_Proveedor = value;
            if (id_Proveedor.HasValue && id_Proveedor.Value>0)
                IsProveedorValid = true;
            else
                IsProveedorValid = false;
                RaisePropertyChanged("ID_Proveedor"); }
        }

        private string nombre_Proveedor;
        public string Nombre_Proveedor
        {
            get { return nombre_Proveedor; }
            set { nombre_Proveedor = value; RaisePropertyChanged("Nombre_Proveedor"); }
        }

        private string razon_social;
        public string Razon_Social
        {
            get { return razon_social; }
            set { razon_social = value; RaisePropertyChanged("Razon_Social"); }
        }

        private string rfc;
        public string RFC
        { get { return rfc; }
            set { rfc = value; RaisePropertyChanged("RFC"); }
        }

        private string buscarProveedor = string.Empty;
        public string BuscarProveedor
        {
            get { return buscarProveedor; }
            set { buscarProveedor = value; RaisePropertyChanged("BuscarProveedor"); }
        }

        private ObservableCollection<PROVEEDOR> proveedores;
        public ObservableCollection<PROVEEDOR> Proveedores
        {
            get { return proveedores; }
            set { proveedores = value; RaisePropertyChanged("Proveedores"); }
        }

        private PROVEEDOR selectedProveedor=null;
        public PROVEEDOR SelectedProveedor
        { get { return selectedProveedor; }
            set { selectedProveedor = value;
            if (value != null)
            {
                OnPropertyValidateChanged("SelectedProveedor");
                ID_Proveedor = value.ID_PROV;
                Nombre_Proveedor = value.NOMBRE;
                Razon_Social = value.RAZON_SOCIAL;
                RFC = value.RFC;
            }
            else
                RaisePropertyChanged("SelectedProveedor");
            }
        }

        private PROVEEDOR selectedProveedorPopUp=null;
        public PROVEEDOR SelectedProveedorPopUp
        {
            get { return selectedProveedorPopUp; }
            set
            {   selectedProveedorPopUp = value;
                RaisePropertyChanged("SelectedProveedorPopUp"); }
        }
        #endregion

        #region Propiedades de Producto Tipo
        private ObservableCollection<ALMACEN_TIPO_CAT> almacen_Tipos_Cat;
        public ObservableCollection<ALMACEN_TIPO_CAT> Almacen_Tipos_Cat
        {
            get { return almacen_Tipos_Cat; }
            set { almacen_Tipos_Cat = value; RaisePropertyChanged("Almacen_Tipos_Cat"); }
        }

        private ALMACEN_TIPO_CAT selectedAlmacen_Tipo_Cat;
        public ALMACEN_TIPO_CAT SelectedAlmacen_Tipo_Cat
        {
            get { return selectedAlmacen_Tipo_Cat; }
            set { selectedAlmacen_Tipo_Cat = value; RaisePropertyChanged("SelectedAlmacen_Tipo_Cat"); }
        }
        #endregion

        #region Requisiciones
        private ObservableCollection<REQUISICION> requisiciones;
        public ObservableCollection<REQUISICION> Requisiciones
        { get { return requisiciones; }
            set { requisiciones = value; RaisePropertyChanged("Requisiciones"); }
        }

        private REQUISICION selectedRequisicionPopUp;
        public REQUISICION SelectedRequisicionPopUp
        {
            get { return selectedRequisicionPopUp; }
            set {
                selectedRequisicionPopUp = value;
                if (value != null)
                {
                    CargarRequisicion_Productos(value);
                    EncabezadoRequisicionDetalle = "Detalle de Requisicion " + value.ID_REQUISICION.ToString();
                }
                else
                {
                    Requisicion_Productos = new ObservableCollection<EXT_Requisicion_Producto>();
                    Requisicion_Centro_Productos = new ObservableCollection<REQUISICION_CENTRO_PRODUCTO>();
                }

                RaisePropertyChanged("SelectedRequisicionPopUp"); }
        }

        private string buscarRequisicion;
        public string BuscarRequisicion
        {
            get { return buscarRequisicion; }
            set { buscarRequisicion = value; RaisePropertyChanged("BuscarRequisicion"); }
        }

        private ObservableCollection<EXT_Requisicion_Producto> requisicion_productos;
        public ObservableCollection<EXT_Requisicion_Producto> Requisicion_Productos
        {
            get { return requisicion_productos; }
            set { requisicion_productos = value; OnPropertyValidateChanged("Requisicion_Productos"); }
        }

        private EXT_Requisicion_Producto selectedRequisicion_ProductoPopUp;
        public EXT_Requisicion_Producto SelectedRequisicion_ProductoPopUp
        {
            get { return selectedRequisicion_ProductoPopUp; }
            set { selectedRequisicion_ProductoPopUp = value;
                if(value!=null)
                {
                    CargarRequisiciones_Centros_Productos(SelectedRequisicionPopUp, value);
                    EncabezadoRequisicionCentroDetalle = "Detalle de Requisición por Centro - " + value.NOMBRE_PRODUCTO.ToString();
                }
                RaisePropertyChanged("SelectedRequisicion_ProductoPopUp"); }
        }

        private ObservableCollection<REQUISICION_CENTRO_PRODUCTO> requisicion_Centro_Productos;
        public ObservableCollection<REQUISICION_CENTRO_PRODUCTO> Requisicion_Centro_Productos
        {
            get { return requisicion_Centro_Productos; }
            set { requisicion_Centro_Productos = value; RaisePropertyChanged("Requisicion_Centro_Productos"); }
        }

        private REQUISICION selectedRequisicion;
        public REQUISICION SelectedRequisicion
        {
            get { return selectedRequisicion; }
            set 
            { 
                selectedRequisicion = value;
                if (value != null)
                    OnPropertyValidateChanged("SelectedRequisicion");
                else
                    RaisePropertyChanged("SelectedRequisicion");
            }
        }
         
        #endregion

        #region Ordenes de Compra
        private ObservableCollection<ORDEN_COMPRA> ordenes_Compra;
        public ObservableCollection<ORDEN_COMPRA> Ordenes_Compra
        {
            get { return ordenes_Compra; }
            set { ordenes_Compra = value; RaisePropertyChanged("Ordenes_Compra"); }
        }

        private ORDEN_COMPRA selectedOrden_CompraPopUp;
        public ORDEN_COMPRA SelectedOrden_CompraPopUp
        {
            get { return selectedOrden_CompraPopUp; }
            set { selectedOrden_CompraPopUp = value; RaisePropertyChanged("SelectedOrden_CompraPopUp"); }
        }
        #endregion

        #region Encabezados de Grupos
        private string encabezadoRequisicionDetalle = "Detalle de Requisicion";
        public string EncabezadoRequisicionDetalle
        { get { return encabezadoRequisicionDetalle; }
            set { encabezadoRequisicionDetalle = value; RaisePropertyChanged("EncabezadoRequisicionDetalle"); }
        }

        private string encabezadoRequisicionCentroDetalle = "Detalle de Requisicion Por Centro";
        public string EncabezadoRequisicionCentroDetalle
        {
            get { return encabezadoRequisicionCentroDetalle; }
            set { encabezadoRequisicionCentroDetalle = value; RaisePropertyChanged("EncabezadoRequisicionCentroDetalle"); }
        }
        #endregion

        


        private ObservableCollection<EXT_Requisicion_Producto_Seleccionado> requisicion_Productos_Seleccionados;
        public ObservableCollection<EXT_Requisicion_Producto_Seleccionado> Requisicion_Productos_Seleccionados
        {
            get { return requisicion_Productos_Seleccionados; }
            set { requisicion_Productos_Seleccionados = value;
            if (value != null && value.Count > 0)
                IsVisibleProductosSeleccionados = true;
            else
                IsVisibleProductosSeleccionados = false;
                RaisePropertyChanged("Requisicion_Productos_Seleccionados"); }
        }

        private bool isVisibleProductosSeleccionados=false;
        public bool IsVisibleProductosSeleccionados
        { get { return isVisibleProductosSeleccionados; }
            set { isVisibleProductosSeleccionados = value; RaisePropertyChanged("IsVisibleProductosSeleccionados"); }
        }

        private int? noOrden;
        public int? NoOrden
        {
            get { return noOrden; }
            set 
            { 
                noOrden = value;
                if (value.HasValue)
                    OnPropertyValidateChanged("NoOrden");
                else
                    RaisePropertyChanged("NoOrden");
            }
        }

        private DateTime? fechaOC = null;
        public DateTime? FechaOC
        { get { return fechaOC; }
            set { 
                fechaOC = value;
                if (value != null && value.Value.Date<=DateTime.Now.Date)
                    IsFechaOCValid = true;
                else
                    IsFechaOCValid = false;
                if (value.HasValue)
                    OnPropertyValidateChanged("FechaOC");
                else
                    RaisePropertyChanged("FechaOC");
            }
        }

        private DateTime fechaRegistro = DateTime.Now.Date;
        public DateTime FechaRegistro
        {
            get { return fechaRegistro; }
            set { fechaRegistro = value; RaisePropertyChanged("FechaRegistro"); }
        }

        private bool isFechaOCValid = false;
        public bool IsFechaOCValid
        { get { return isFechaOCValid; }
            set { isFechaOCValid = value; RaisePropertyChanged("IsFechaOCValid"); }
        }

       

        private bool isProveedorValid = false;
        public bool IsProveedorValid
        {
            get { return isProveedorValid; }
            set { isProveedorValid = value; RaisePropertyChanged("IsProveedorValid"); }
        }

        private bool isRequisicionValid = false;
        public bool IsRequisicionValid
        {
            get { return isRequisicionValid; }
            set { isRequisicionValid = value; RaisePropertyChanged("IsRequisicionValid"); }
        }

        private bool isPreciosValid = false;
        public bool IsPreciosValid
        {
            get { return isPreciosValid; }
            set { isPreciosValid = value; RaisePropertyChanged("IsPreciosValid"); }
        }
        #endregion

        #region Validacion
        private void setValidationRules(Tipo_Reglas tipo_reglas)
        {
            base.ClearRules();
            switch(tipo_reglas)
            {
                case Tipo_Reglas.Salvar:
                    base.AddRule(() => NoOrden, () => (NoOrden.HasValue && NoOrden.Value>0), "NUMERO DE ORDEN ES REQUERIDO!");
                    base.AddRule(() => IsFechaOCValid, () => IsFechaOCValid, "LA FECHA DE LA OC TIENE QUE SER VALIDA!");
                    base.AddRule(() => IsProveedorValid , () => (IsProveedorValid), "SELECCIONE UN PROVEEDOR!");
                    base.AddRule(() => IsRequisicionValid, () => (IsRequisicionValid), "SELECCIONE UNA REQUISICION Y LOS PRODUCTOS PARA LA OC!");
                    base.AddRule(() => IsPreciosValid, () => (IsPreciosValid), "SELECCIONE UNA REQUISICION Y LOS PRODUCTOS PARA LA OC!");
                    RaisePropertyChanged("NoOrden");
                    RaisePropertyChanged("IsProveedorValid");
                    RaisePropertyChanged("IsRequisicionValid");
                    RaisePropertyChanged("IsPreciosValid");
                    RaisePropertyChanged("IsFechaOCValid");
                    break;
                case Tipo_Reglas.BuscarOC:
                    base.AddRule(() => SelectedOrden_CompraPopUp, () => (SelectedOrden_CompraPopUp != null), "SE DEBE SELECCIONAR UNA ORDEN DE COMPRA!");
                    RaisePropertyChanged("SelectedOrden_CompraPopUp");
                    break;
                case Tipo_Reglas.BuscarProveedor:
                    base.AddRule(()=>SelectedProveedorPopUp, ()=> (SelectedProveedorPopUp != null), "SE DEBE SELECCIONAR UN PROVEEDOR!");
                    RaisePropertyChanged("SelectedProveedorPopUp");
                    break;
                case Tipo_Reglas.BuscarRequisicion:
                    base.AddRule(() => SelectedRequisicionPopUp, () => (SelectedRequisicionPopUp != null), "SE DEBE DE SELECCIONAR UNA REQUISICION!");
                    RaisePropertyChanged("SelectedRequisicionPopUp");
                    break;
            }
        }
        #endregion

        #region Metodos
        #region Buscar

        private async void OnClickSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "buscar_proveedor":
                    await BuscarCatalogoProveedor();
                    setValidationRules(Tipo_Reglas.BuscarProveedor);
                    break;
                case "buscar_requisicion":
                    if (SelectedOrden_CompraPopUp != null)
                        await BuscarCatalogoRequisiciones(SelectedAlmacen_Tipo_Cat.ID_ALMACEN_TIPO, BuscarRequisicion, _usuario.Almacen_Grupo, SelectedOrden_CompraPopUp.ID_ORDEN_COMPRA);
                    else
                        await BuscarCatalogoRequisiciones(SelectedAlmacen_Tipo_Cat.ID_ALMACEN_TIPO, BuscarRequisicion, _usuario.Almacen_Grupo);
                    setValidationRules(Tipo_Reglas.BuscarRequisicion);
                    break;
                case "editar_seleccion":
                    setValidationRules(Tipo_Reglas.BuscarRequisicion);
                    await BuscarCatalogoRequisiciones(SelectedRequisicion.ID_REQUISICION);

                    selectedRequisicionPopUp = Requisiciones.First(f => f.ID_REQUISICION == SelectedRequisicion.ID_REQUISICION);
                    RaisePropertyChanged("SelectedRequisicionPopUp");
                    await CargarRequisicion_ProductosAwaitable(selectedRequisicionPopUp);
                   
                    SeleccionarProductos_Requisicion_Producto_PopUp(requisicion_productos, Requisicion_Productos_Seleccionados);
                    RaisePropertyChanged("Requisicion_Productos");
                    break;
            }
        }

        private async Task BuscarCatalogoProveedor()
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Buscando Proveedores", "Por favor espere un momento...");
            try
            {
                await CargarProveedores(BuscarProveedor);
                StaticSourcesViewModel.CloseProgressLoading();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA_PROVEEDORES);
            }catch(Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los proveedores. Favor de contactar al administrador");
            }
        }

        private async Task CargarProveedores(string parametro)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Proveedores = new ObservableCollection<PROVEEDOR>(new cProveedor().Seleccionar(parametro, _usuario.Almacen_Grupo,true).ToList());
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task BuscarCatalogoRequisiciones(int busqueda_parametro)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...", "Buscando Requisiciones");
            try
            {
                await CargarRequisiciones(busqueda_parametro);
                StaticSourcesViewModel.CloseProgressLoading();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA_REQUISICIONES);
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar las requisiciones. Favor de contactar al administrador");
            }
        }

        private async Task BuscarCatalogoRequisiciones(short id_almacen_tipo, string busqueda_parametro, string almacen_grupo_parametro, int? orden_compra=null)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...", "Buscando Requisiciones");
            try
            {
                await CargarRequisiciones(id_almacen_tipo, busqueda_parametro, almacen_grupo_parametro, orden_compra);
                StaticSourcesViewModel.CloseProgressLoading();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA_REQUISICIONES);
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar las requisiciones. Favor de contactar al administrador");
            }
        }
        private async Task CargarRequisiciones(int _id_requisicion)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Requisiciones = new ObservableCollection<REQUISICION>(new cRequisicion().Seleccionar(_id_requisicion).ToList());
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task CargarRequisiciones(short id_almacen_tipo_cat,string parametro,string almacen_grupo, int? id_orden_compra)
        {
            try
            {
                await Task.Factory.StartNew(() => {
                    if (id_orden_compra.HasValue)
                        Requisiciones = new ObservableCollection<REQUISICION>(new cRequisicion().SeleccionarNoAsignadas(id_almacen_tipo_cat, id_orden_compra.Value, almacen_grupo).ToList());
                    else
                        Requisiciones = new ObservableCollection<REQUISICION>(new cRequisicion().SeleccionarNoAsignadas(id_almacen_tipo_cat, almacen_grupo).ToList());
                    if (!string.IsNullOrWhiteSpace(parametro))
                        Requisiciones =  new ObservableCollection<REQUISICION>(Requisiciones.Where(w => w.ID_REQUISICION.ToString().Contains(parametro)));
                });
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        
        private async Task CargarRequisicion_ProductosAwaitable(REQUISICION requisicion)
        {
            try
            {
                var _error = false;
                StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...", string.Format("Buscando detalle de la requisicion {0}", requisicion.ID_REQUISICION.ToString()));

                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        if (RealizarAccion==AccionSalvar.Salvar)
                            requisicion_productos = new ObservableCollection<EXT_Requisicion_Producto>(new cRequisicionProducto().SeleccionarNoAsignadoCentro(requisicion.ID_REQUISICION).ToList());
                        else
                            requisicion_productos = new ObservableCollection<EXT_Requisicion_Producto>(new cRequisicionProducto().SeleccionarAsignadosYPosibles(requisicion.ID_REQUISICION, SelectedOrden_CompraPopUp.ID_ORDEN_COMPRA).ToList());
                    });
                    StaticSourcesViewModel.CloseProgressLoading();
                }
                catch (Exception ex)
                {
                    _error = true;
                }
                if (_error)
                {
                    StaticSourcesViewModel.CloseProgressLoading();
                    await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el detalle de la requisicion. Favor de contactar al administrador");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void CargarRequisicion_Productos(REQUISICION requisicion)
        {
            try
            {
                var _error = false;
                StaticSourcesViewModel.ShowProgressLoading(string.Format("Por favor espere un momento", "Buscando detalle de la requisicion {0}", requisicion.ID_REQUISICION.ToString()));
                
                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        if (selectedOrden_CompraPopUp==null)
                            Requisicion_Productos = new ObservableCollection<EXT_Requisicion_Producto>(new cRequisicionProducto().SeleccionarNoAsignadoCentro(requisicion.ID_REQUISICION).ToList());
                        else
                            Requisicion_Productos = new ObservableCollection<EXT_Requisicion_Producto>(new cRequisicionProducto().SeleccionarAsignadosYPosibles(requisicion.ID_REQUISICION, SelectedOrden_CompraPopUp.ID_ORDEN_COMPRA).ToList());
                    });
                    StaticSourcesViewModel.CloseProgressLoading();
                }
                catch (Exception ex)
                {
                    _error = true;
                }
                if (_error)
                {
                    StaticSourcesViewModel.CloseProgressLoading();
                    await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el detalle de la requisicion. Favor de contactar al administrador");
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void CargarRequisiciones_Centros_Productos(REQUISICION requisicion, EXT_Requisicion_Producto requisicion_producto)
        {
            try
            {
                var _error = false;
                StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento", string.Format("Buscando detalle del producto {0} por centro", requisicion_producto.NOMBRE_PRODUCTO ));
                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        Requisicion_Centro_Productos = 
                            new ObservableCollection<REQUISICION_CENTRO_PRODUCTO>(
                                new cRequisicion_Centro_Producto().Seleccionar(requisicion.ID_REQUISICION, requisicion_producto.ID_PRODUCTO).ToList());
                    });
                    StaticSourcesViewModel.CloseProgressLoading();
                }
                catch (Exception ex)
                {
                    _error = true;
                }
                if (_error)
                {
                    StaticSourcesViewModel.CloseProgressLoading();
                    await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el detalle del producto por centro. Favor de contactar al administrador");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void BuscarOrdenes_Compra()
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento", "Buscando Ordenes de Compra");
            try
            {
                await CargarOrdenes_Compra("CP");
                StaticSourcesViewModel.CloseProgressLoading();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA_ORDENES_COMPRA);
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar las ordenes de compra. Favor de contactar al administrador");
            }
        }

        private async Task CargarOrdenes_Compra(string status)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                    {
                        Ordenes_Compra = new ObservableCollection<ORDEN_COMPRA>(new cOrden_Compra().Seleccionar(status,_usuario.Almacen_Grupo).ToList());
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SeleccionarProductos_Requisicion_Producto_PopUp(ObservableCollection<EXT_Requisicion_Producto> lista_productos_popup, ObservableCollection<EXT_Requisicion_Producto_Seleccionado> lista_producto_seleccionado)
        {
            foreach (var item in lista_producto_seleccionado)
            {
                lista_productos_popup.First(f => f.ID_PRODUCTO == item.ID_PRODUCTO).IS_SELECTED = true;
            }
                
        }

        #endregion

        private void VaciarOrden_Compra(object accion)
        {
            setValidationRules(Tipo_Reglas.Salvar);
            SalvarHabilitado = true;
            switch (accion.ToString())
            {
                case "seleccionado":
                   
                    NoOrden = SelectedOrden_CompraPopUp.NUM_ORDEN;
                    FechaOC = SelectedOrden_CompraPopUp.FECHA;
                    if (SelectedOrden_CompraPopUp.MES.HasValue)
                        FechaRegistro = SelectedOrden_CompraPopUp.MES.Value.Date;
                    IsFechaOCValid = true;

                    SelectedProveedor = SelectedOrden_CompraPopUp.PROVEEDOR;
                    SelectedRequisicion = SelectedOrden_CompraPopUp.REQUISICION;
                    var lista_productos = selectedOrden_CompraPopUp.REQUISICION.REQUISICION_PRODUCTO.Where(w =>w.REQUISICION.ORDEN_COMPRA.Any(a =>a.ID_ORDEN_COMPRA==SelectedOrden_CompraPopUp.ID_ORDEN_COMPRA && a.ORDEN_COMPRA_DETALLE.Any(a2 => a2.ID_PRODUCTO == w.ID_PRODUCTO))).Select(s => new EXT_Requisicion_Producto
                    {
                        CANTIDAD = s.CANTIDAD.Value,
                        ID_PRODUCTO = s.ID_PRODUCTO,
                        ID_REQUISICION = s.ID_REQUISICION,
                        IS_SELECTED = true,
                        NOMBRE_PRODUCTO = s.PRODUCTO.NOMBRE,
                    });
                    Requisicion_Productos_Seleccionados =
                            new ObservableCollection<EXT_Requisicion_Producto_Seleccionado>(
                                new cRequisicionProducto().SeleccionadosPorOrdenDeCompra(
                                new ObservableCollection<EXT_Requisicion_Producto>(lista_productos.ToList())));
                    ValidarRequisicion();
                    ValidarPrecios(null);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_ORDENES_COMPRA);
                    RealizarAccion = AccionSalvar.Actualizar;
                    
                    NoOrdenIsReadOnly = true;
                    ModoBloqueo(true);
                    CancelarHabilitado = true;
                    EliminarHabilitado = true;
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "cancelado":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_ORDENES_COMPRA);
                   
                    break;
            }
        }
        
        private void VaciarDatosProveedor(object accion)
        {
            setValidationRules(Tipo_Reglas.Salvar);
            switch(accion.ToString())
            {
                case "seleccionado":
                    SelectedProveedor = SelectedProveedorPopUp;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_PROVEEDORES);
                    break;
                case "cancelado":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_PROVEEDORES);
                    break;
            }
        }

        private async void VaciarDatosRequisicion_Centro_Producto(object accion)
        {
            setValidationRules(Tipo_Reglas.Salvar);
            switch (accion.ToString())
            {
                case "seleccionado":
                    if (RealizarAccion==AccionSalvar.Salvar)
                    {
                        await SeleccionadosPorBusquedaRequisicionGeneral(Requisicion_Productos);
                        SelectedRequisicion = SelectedRequisicionPopUp;

                    }
                    else
                    {
                        if(SelectedRequisicion.ID_REQUISICION!=SelectedRequisicionPopUp.ID_REQUISICION)
                        {
                            await SeleccionadosPorBusquedaRequisicionGeneral(Requisicion_Productos);
                            SelectedRequisicion = SelectedRequisicionPopUp;
                        }
                        else
                        {
                            var Requisicion_Productos_Agregar = new ObservableCollection<EXT_Requisicion_Producto>();
                            foreach (var item in Requisicion_Productos)
                            {
                                var item_encontrado = requisicion_Productos_Seleccionados.FirstOrDefault(w => w.ID_PRODUCTO == item.ID_PRODUCTO && w.ID_REQUISICION == item.ID_REQUISICION);
                                if (item_encontrado == null)
                                {
                                    if (item.IS_SELECTED)
                                        Requisicion_Productos_Agregar.Add(item);
                                }
                                else
                                {
                                    if (!item.IS_SELECTED)
                                        requisicion_Productos_Seleccionados.Remove(requisicion_Productos_Seleccionados.First(w => w.ID_PRODUCTO == item.ID_PRODUCTO && w.ID_REQUISICION == item.ID_REQUISICION));
                                }
                            }
                            var lista = await SeleccionadosListaPorBusquedaRequisicionGeneral(Requisicion_Productos_Agregar);
                            foreach (var item in lista)
                                requisicion_Productos_Seleccionados.Add(item);
                            RaisePropertyChanged("Requisicion_Productos_Seleccionados");
                        }
                    }
                    
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_REQUISICIONES);
                    
                    ValidarRequisicion();
                    ValidarPrecios(null);
                    break;
                case "cancelado":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_REQUISICIONES);
                    break;
            }
        }

        private void CargarAlmacen_Tipo_Cat(bool isExceptionManaged = false)
        {
            try
            {
                almacen_Tipos_Cat =new ObservableCollection<ALMACEN_TIPO_CAT >(new cAlmacen_Tipo_Cat().Seleccionar(string.Empty,_usuario.Almacen_Grupo,"S").ToList());
                var dummy = new ALMACEN_TIPO_CAT {
                    ID_ALMACEN_TIPO=-1,
                    DESCR="Todos"
                };
                almacen_Tipos_Cat.Insert(0, dummy);
                RaisePropertyChanged("Almacen_Tipos_Cat");                
            }catch(Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los tipos. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        private void RaiseChangeCheckedRequisicionProducto(object obj)
        {
            RaisePropertyChanged("Requisicion_Productos");
        }

        private async void SeleccionarRequisiciones_Productos(REQUISICION requisicion, EXT_Requisicion_Producto requisicion_producto)
        {
            try
            {
                var _error = false;
                StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento", string.Format("Buscando detalle del producto {0} por centro", requisicion_producto.NOMBRE_PRODUCTO));
                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        Requisicion_Centro_Productos =
                            new ObservableCollection<REQUISICION_CENTRO_PRODUCTO>(
                                new cRequisicion_Centro_Producto().Seleccionar(requisicion.ID_REQUISICION, requisicion_producto.ID_PRODUCTO).ToList());
                    });
                    StaticSourcesViewModel.CloseProgressLoading();
                }
                catch (Exception ex)
                {
                    _error = true;
                }
                if (_error)
                {
                    StaticSourcesViewModel.CloseProgressLoading();
                    await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el detalle del producto por centro. Favor de contactar al administrador");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SeleccionadosPorBusquedaRequisicionGeneral(ObservableCollection<EXT_Requisicion_Producto> lista_productos)
        {
            try
            {
                var _error = false;
                StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento", string.Format("Seleccionando productos"));
                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        Requisicion_Productos_Seleccionados =
                            new ObservableCollection<EXT_Requisicion_Producto_Seleccionado>(
                                new cRequisicionProducto().SeleccionadosPorBusquedaRequisicionGeneral(lista_productos));
                    });
                    StaticSourcesViewModel.CloseProgressLoading();
                }
                catch (Exception ex)
                {
                    _error = true;
                }
                if (_error)
                {
                    StaticSourcesViewModel.CloseProgressLoading();
                    await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el detalle del producto por centro. Favor de contactar al administrador");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<EXT_Requisicion_Producto_Seleccionado>> SeleccionadosListaPorBusquedaRequisicionGeneral(ObservableCollection<EXT_Requisicion_Producto> lista_productos)
        {
            try
            {
                var resultado = new List<EXT_Requisicion_Producto_Seleccionado>();
                var _error = false;
                StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento", string.Format("Seleccionando productos"));
                try
                {
                    resultado=await Task.Factory.StartNew(() =>
                    {
                        return  new cRequisicionProducto().SeleccionadosPorBusquedaRequisicionGeneral(lista_productos);
                    });
                    StaticSourcesViewModel.CloseProgressLoading();
                    
                }
                catch (Exception ex)
                {
                    _error = true;
                }
                if (_error)
                {
                    StaticSourcesViewModel.CloseProgressLoading();
                    await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el detalle del producto por centro. Favor de contactar al administrador");
                }
                return resultado;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ModoBloqueo(bool activado)
        {
            IsBloqueProveedorEnabled = !activado;
            IsBloqueRequisicionEnabled = !activado;
            IsFechaOCEnabled = !activado;
            EditarHabilitado = activado;            
            SalvarHabilitado = !activado;
        }

        private bool CanExecute(object parameter) { return base.HasErrors == false; }

        private async void AccionMenuSwitch(object parametro)
        {
            switch(parametro.ToString())
            {
                case "agregar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await (_dialogCoordinator.ShowMessageAsync(this, "Advertencia", "Hay cambios sin guardar, ¿Seguro que desea agregar una nueva orden de compra sin guardar?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative)) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                        {
                            ModoBloqueo(false);
                            CondicionesIniciales();
                            LimpiarCampos();
                            setValidationRules(Tipo_Reglas.Salvar);
                            StaticSourcesViewModel.SourceChanged = false;
                        }
                    }
                    else
                    {
                        ModoBloqueo(false);
                        CondicionesIniciales();
                        LimpiarCampos();
                        setValidationRules(Tipo_Reglas.Salvar);
                    }
                    break;
                case "salvar":
                    GuardarOC();
                    CondicionesIniciales();
                    LimpiarCampos();
                    setValidationRules(Tipo_Reglas.Salvar);
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "buscar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await (_dialogCoordinator.ShowMessageAsync(this, "Advertencia", "Hay cambios sin guardar, ¿Seguro que desea buscar una orden de compra sin guardar?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative)) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                        {
                            SalvarHabilitado = false;
                            CondicionesIniciales();
                            LimpiarCampos();
                            BuscarOrdenes_Compra();
                            setValidationRules(Tipo_Reglas.BuscarOC);
                            StaticSourcesViewModel.SourceChanged = false;
                        }
                    }
                    else
                    {
                        SalvarHabilitado = false;
                        CondicionesIniciales();
                        LimpiarCampos();
                        BuscarOrdenes_Compra();
                        setValidationRules(Tipo_Reglas.BuscarOC);
                    }
                    break;
                case "editar":
                    ModoBloqueo(false);
                    CancelarHabilitado = true;
                    break;
                case "cancelar":
                    ModoBloqueo(false);
                    CondicionesIniciales();
                    LimpiarCampos();
                    setValidationRules(Tipo_Reglas.Salvar);
                    break;
                case "eliminar":
                    await EliminarOC();
                    CondicionesIniciales();
                    LimpiarCampos();
                    ModoBloqueo(false);
                    setValidationRules(Tipo_Reglas.Salvar);
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
            }
        }

        private void CondicionesIniciales()
        {
            RealizarAccion = AccionSalvar.Salvar;
            CancelarHabilitado = false;
            EditarHabilitado = false;
            EliminarHabilitado = false;
            IsProveedorValid = false;
            IsRequisicionValid = false;
            NoOrdenIsReadOnly = false;
            CargarAlmacen_Tipo_Cat();
            SelectedAlmacen_Tipo_Cat = Almacen_Tipos_Cat.First(w => w.ID_ALMACEN_TIPO == -1);
            ClearRules();
        }

        private void LimpiarCampos()
        {
            NoOrden = null;
            FechaOC = null;
            FechaRegistro = DateTime.Now.Date;

            BuscarProveedor = string.Empty;
            SelectedProveedor = null;
            ID_Proveedor = null;
            Nombre_Proveedor = string.Empty;
            Razon_Social = string.Empty;
            RFC = string.Empty;

            BuscarRequisicion = string.Empty;
            SelectedRequisicion = null;
            Requisicion_Productos_Seleccionados = new ObservableCollection<EXT_Requisicion_Producto_Seleccionado>();

            EncabezadoRequisicionDetalle= "Detalle de Requisicion";
            EncabezadoRequisicionCentroDetalle = "Detalle de Requisicion Por Centro";
        }

        private void ValidarRequisicion()
        {
            if (SelectedRequisicion != null && SelectedRequisicion.ID_REQUISICION > 0 && Requisicion_Productos_Seleccionados != null && Requisicion_Productos_Seleccionados.Count > 0)
                IsRequisicionValid = true;
            else
                IsRequisicionValid = false;
        }

        private void ValidarPrecios(object parametro)
        {
            isPreciosValid = true;
            foreach(var item in Requisicion_Productos_Seleccionados)
            {
                if (!(item.PRECIO_UNITARIO.HasValue && item.PRECIO_UNITARIO.Value>0))
                {
                    isPreciosValid=false;
                    break;
                }
            }
            RaisePropertyChanged("IsPreciosValid");
        }

        private async Task EliminarOC()
        {
            var _error = false;
            try
            {
                if (await _dialogCoordinator.ShowMessageAsync(this, "Confirmación", String.Format("¿Esta seguro de eliminar la Orden de Compra {0}?",NoOrden.Value.ToString()), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    new cOrden_Compra().Eliminar(SelectedOrden_CompraPopUp);
                    await _dialogCoordinator.ShowMessageAsync(this, "Notificación", "Se elimino la Orden de Compra con exito");
                }
            }
            catch (Exception ex)
            { _error = true; };
            if (_error)
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
        }

        private void GuardarOC()
        {
            try
            {
                
                var _OC = new ORDEN_COMPRA {                    
                    ESTATUS="CP",
                    FECHA=FechaOC.Value,
                    ID_PROV=ID_Proveedor.Value,
                    ID_REQUISICION=SelectedRequisicion.ID_REQUISICION,
                    ID_USUARIO=_usuario.Username,
                    MES=FechaRegistro.Date,
                    NUM_ORDEN=NoOrden
                };
                var _orden_compra_detalle = new List<EXT_Orden_Compra_Detalle>();
                foreach (var item in Requisicion_Productos_Seleccionados)
                   foreach (var producto_detalle in item.DETALLE_CENTRO_PRODUCTO)
                   {
                       _orden_compra_detalle.Add(new EXT_Orden_Compra_Detalle
                       {
                           ID_ORDEN_COMPRA =(AccionSalvar.Actualizar==RealizarAccion)? SelectedOrden_CompraPopUp.ID_ORDEN_COMPRA:0,
                           CANTIDAD_ENTREGADA=0,
                           CANTIDAD_ORDEN=producto_detalle.CANTIDAD.Value,
                           DIFERENCIA=producto_detalle.CANTIDAD.Value,
                           ID_PRODUCTO=producto_detalle.ID_PRODUCTO,
                           PRECIO_COMPRA=item.PRECIO_UNITARIO.Value,
                           ID_ALMACEN=producto_detalle.ID_ALMACEN,
                           ID_REQUISICION_CENTRO=producto_detalle.ID_REQUISICION,
                           CANTIDAD_TRANSITO=0
                       });
                   }
                if (RealizarAccion == AccionSalvar.Salvar)
                {
                    new cOrden_Compra().Insertar(_OC, _orden_compra_detalle);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se inserto la orden de compra con exito");
                }
                else if (RealizarAccion == AccionSalvar.Actualizar)
                {
                    
                    _OC.ID_ORDEN_COMPRA = SelectedOrden_CompraPopUp.ID_ORDEN_COMPRA;
                    new cOrden_Compra().Actualizar(_OC, _orden_compra_detalle);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se actualizo la orden de compra con exito");

                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
            }
        }

        private async void OnLoad(object sender)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento..");
            try
            {
                await Task.Factory.StartNew(() => CargarAlmacen_Tipo_Cat(true));
                SelectedAlmacen_Tipo_Cat = Almacen_Tipos_Cat.First(f => f.ID_ALMACEN_TIPO == -1);
                setValidationRules(Tipo_Reglas.Salvar);
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }
        #endregion

        #region Comandos
        private ICommand cmdOnClick;

        public ICommand CmdOnClick
        {
            get { return cmdOnClick ?? (cmdOnClick = new RelayCommand(OnClickSwitch)); }
        }

        private ICommand cmdSeleccionarProveedores;

        public ICommand CmdSeleccionarProveedores
        {
            get { return cmdSeleccionarProveedores ?? (cmdSeleccionarProveedores = new RelayCommand(VaciarDatosProveedor,CanExecute)); }
        }

        private ICommand cmdCancelarProveedores;

        public ICommand CmdCancelarProveedores
        {
            get { return cmdCancelarProveedores ?? (cmdCancelarProveedores = new RelayCommand(VaciarDatosProveedor)); }
        }

        private ICommand onChecked;
        public ICommand OnChecked
        {
            get { return onChecked ?? (onChecked = new RelayCommand(RaiseChangeCheckedRequisicionProducto)); }
        }

        private ICommand cmdSeleccionarRequisiciones_Productos;
        public ICommand CmdSeleccionarRequisiciones_Productos
        {
            get { return cmdSeleccionarRequisiciones_Productos ?? (cmdSeleccionarRequisiciones_Productos = new RelayCommand(VaciarDatosRequisicion_Centro_Producto,CanExecute)); }
        }

        private ICommand cmdCancelarRequisiciones_Productos;
        public ICommand CmdCancelarRequisiciones_Productos
        {
            get { return cmdCancelarRequisiciones_Productos ?? (cmdCancelarRequisiciones_Productos = new RelayCommand(VaciarDatosRequisicion_Centro_Producto)); }
        }

        private ICommand cmdaccionMenuSinValidar;
        public ICommand CmdAccionMenuSinValidar
        {
            get
            {

                return cmdaccionMenuSinValidar ?? (cmdaccionMenuSinValidar = new RelayCommand(AccionMenuSwitch));
            }
        }

        private ICommand cmdAccionMenu;
        public ICommand CmdAccionMenu
        {
            get
            {

                return cmdAccionMenu ?? (cmdAccionMenu = new RelayCommand(AccionMenuSwitch, CanExecute));
            }
        }

        private ICommand cmdValidacionPrecios;
        public ICommand CmdValidacionPrecios
        {
            get
            {

                return cmdValidacionPrecios ?? (cmdValidacionPrecios = new RelayCommand(ValidarPrecios));
            }
        }

        private ICommand cmdSeleccionarOC;
        public ICommand CmdSeleccionarOC
        {
            get { return cmdSeleccionarOC ?? (cmdSeleccionarOC = new RelayCommand(VaciarOrden_Compra, CanExecute)); }
        }

        private ICommand cmdCancelarOC;
        public ICommand CmdCancelarOC
        {
            get { return cmdCancelarOC ?? (cmdCancelarOC = new RelayCommand(VaciarOrden_Compra)); }
        }

        private ICommand cmdLoad;
        public ICommand CmdLoad
        {
            get { return cmdLoad ?? (cmdLoad = new RelayCommand(OnLoad)); }
        }

        #endregion
    }
}
