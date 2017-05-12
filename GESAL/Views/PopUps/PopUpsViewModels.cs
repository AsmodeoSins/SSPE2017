using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using GESAL.Views.Principal;
namespace GESAL
{
    public class PopUpsViewModels
    {
        #region [Lista de PopUps]
        public enum TipoPopUp
        {
            CERRAR_TODOS,
            BUSQUEDA_PROVEEDORES,
            BUSQUEDA_REQUISICIONES,
            BUSQUEDA_ORDENES_COMPRA,
            AGENDA,
            BUSQUEDA_ORDENES_COMPRA_TRANSITO_DETALLE,
            ALMACEN_POPUP,
            PRODUCTO_UNIDAD_MEDIDAPOPUP,
            PRODUCTO_SUBCATEGORIAPOPUP,
            PRODUCTO_CATEGORIAPOPUP,
            ALMACEN_TIPOPOPUP,
            PRODUCTO_PRESENTACIONPOPUP,
            PRODUCTOPOPUP,
            ALMACEN_GRUPOPOPUP,
            FONDOOBSCURO,
            ENTRADAS_LOTES,
            RECHAZO_PRODUCTO,
            MENSAJEPROXY,
            DIGITALIZAR_DOCUMENTO,
            BUSQUEDA_REQUISICION_ESTATUS
        }
        #endregion

        #region [Variables]
        private static object _PopUpDataContext;
        public static object PopUpDataContext
        {
            get { return _PopUpDataContext; }
        }

        private static PrincipalView _MainWindow;
        public static PrincipalView MainWindow
        {
            get { return PopUpsViewModels._MainWindow; }
            set
            {
                PopUpsViewModels._MainWindow = value;

                #region Catalogos
                value.AlmacenPopUpView.IsVisibleChanged += (s, e) =>
                {
                    if (!((bool)e.NewValue))
                        return;

                    MainWindow.AlmacenPopUpView.Focusable = true;
                    MainWindow.AlmacenPopUpView.Focus();
                };
                value.Producto_Unidad_MedidaPopUpView.IsVisibleChanged += (s, e) =>
                {
                    if (!((bool)e.NewValue))
                        return;

                    MainWindow.Producto_Unidad_MedidaPopUpView.Focusable = true;
                    MainWindow.Producto_Unidad_MedidaPopUpView.Focus();
                };
                value.Producto_SubCategoriaPopUpView.IsVisibleChanged += (s, e) =>
                {
                    if (!((bool)e.NewValue))
                        return;

                    MainWindow.Producto_SubCategoriaPopUpView.Focusable = true;
                    MainWindow.Producto_SubCategoriaPopUpView.Focus();
                };
                value.Producto_CategoriaPopUpView.IsVisibleChanged += (s,e) =>
                {
                    if (!((bool)e.NewValue))
                        return;
                    MainWindow.Producto_CategoriaPopUpView.Focusable = true;
                    MainWindow.Producto_CategoriaPopUpView.Focus();
                };
                value.Almacen_TipoPopUpView.IsVisibleChanged += (s, e) =>
                {
                    if(!((bool)e.NewValue))
                        return;
                    MainWindow.Almacen_TipoPopUpView.Focusable=true;
                    MainWindow.Almacen_TipoPopUpView.Focus();
                };
                value.Producto_PresentacionPopUpView.IsVisibleChanged += (s, e) =>
                {
                    if (!((bool)e.NewValue))
                        return;
                    MainWindow.Producto_PresentacionPopUpView.Focusable = true;
                    MainWindow.Producto_PresentacionPopUpView.Focus();
                };
                value.ProductoPopUpView.IsVisibleChanged += (s, e) =>
                {
                    if (!((bool)e.NewValue))
                        return;
                    MainWindow.ProductoPopUpView.Focusable = true;
                    MainWindow.ProductoPopUpView.Focus();
                };

                value.Almacen_GrupoPopUpView.IsVisibleChanged += (s, e) =>
                {
                    if(!((bool)e.NewValue))
                        return;
                    MainWindow.Almacen_GrupoPopUpView.Focusable = true;
                    MainWindow.ProductoPopUpView.Focus();
                };
                #endregion

                value.BusquedaProveedoresView.IsVisibleChanged += (s, e) =>
                {
                    if (!((bool)e.NewValue))
                        return;

                    MainWindow.BusquedaProveedoresView.Focusable = true;
                    MainWindow.BusquedaProveedoresView.Focus();
                };

                value.BusquedaRequisicionesProductosView.IsVisibleChanged += (s, e) => {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.BusquedaRequisicionesProductosView.Focusable = true;
                    MainWindow.BusquedaRequisicionesProductosView.Focus();
                };
                value.BusquedaOrdenes_CompraView.IsVisibleChanged += (s, e) =>
                {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.BusquedaOrdenes_CompraView.Focusable = true;
                    MainWindow.BusquedaOrdenes_CompraView.Focus();
                };
                value.AgendaView.IsVisibleChanged += (s, e) =>
                {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.AgendaView.Focusable = true;
                    MainWindow.AgendaView.Focus();
                };
                value.BusquedaOrdenes_Compra_TransitoDetalleView.IsVisibleChanged += (s, e) => {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.BusquedaOrdenes_Compra_TransitoDetalleView.Focusable = true;
                    MainWindow.BusquedaOrdenes_Compra_TransitoDetalleView.Focus();
                };
                value.FondoObscuroView.IsVisibleChanged += (s, e) =>
                {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.FondoObscuroView.Focusable = true;
                    MainWindow.FondoObscuroView.Focus();
                };
                value.AsignaLotesProductoView.IsVisibleChanged += (s, e) =>
                {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.AsignaLotesProductoView.Focusable = true;
                    MainWindow.AsignaLotesProductoView.Focus();
                };
                value.RechazoProductoEntradaView.IsVisibleChanged += (s, e) =>
                {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.RechazoProductoEntradaView.Focusable = true;
                    MainWindow.RechazoProductoEntradaView.Focus();
                };
                value.MensajeConfirmacionProxy.IsVisibleChanged += (s, e) =>
                {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.MensajeConfirmacionProxy.Focusable = true;
                    MainWindow.MensajeConfirmacionProxy.Focus();
                };
                value.MensajeConfirmacionProxy.IsVisibleChanged += (s, e) =>
                {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.MensajeConfirmacionProxy.Focusable = true;
                    MainWindow.MensajeConfirmacionProxy.Focus();
                };
                value.DigitalizacionDocumentosView.IsVisibleChanged += (s, e) =>
                {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.DigitalizacionDocumentosView.Focusable = true;
                    MainWindow.DigitalizacionDocumentosView.Focus();
                };
                value.BusquedaRequisicionView.IsVisibleChanged += (s, e) => {
                    if (!(bool)e.NewValue)
                        return;
                    MainWindow.BusquedaRequisicionView.Focusable = true;
                    MainWindow.BusquedaRequisicionView.Focus();
                };
            }
        }

        #region Catalogos
        private static Visibility _VisibleAlmacenPopUp = Visibility.Collapsed;
        public static Visibility VisibleAlmacenPopUp { get { return PopUpsViewModels._VisibleAlmacenPopUp; } }

        private static Visibility _VisibleProducto_Unidad_MedidaPopUp = Visibility.Collapsed;
        public static Visibility VisibleProducto_Unidad_MedidaPopUp { get { return PopUpsViewModels._VisibleProducto_Unidad_MedidaPopUp; } }

        private static Visibility _VisibleProducto_SubCategoriaPopUp = Visibility.Collapsed;
        public static Visibility VisibleProducto_SubCategoriaPopUp { get { return PopUpsViewModels._VisibleProducto_SubCategoriaPopUp; } }

        private static Visibility _VisibleProducto_CategoriaPopUp = Visibility.Collapsed;
        public static Visibility VisibleProducto_CategoriaPopUp { get { return PopUpsViewModels._VisibleProducto_CategoriaPopUp; } }

        private static Visibility _VisibleAlmacen_TipoPopUp = Visibility.Collapsed;
        public static Visibility VisibleAlmacen_TipoPopUp { get { return PopUpsViewModels._VisibleAlmacen_TipoPopUp; } }

        private static Visibility _VisibleProducto_PresentacionPopUp = Visibility.Collapsed;
        public static Visibility VisibleProducto_PresentacionPopUp { get { return PopUpsViewModels._VisibleProducto_PresentacionPopUp; } }

        private static Visibility _VisibleProductoPopUp = Visibility.Collapsed;
        public static Visibility VisibleProductoPopUp { get { return PopUpsViewModels._VisibleProductoPopUp; } }

        private static Visibility _VisibleAlmacen_GrupoPopUp = Visibility.Collapsed;
        public static Visibility VisibleAlmacen_GrupoPopup { get { return PopUpsViewModels._VisibleAlmacen_GrupoPopUp; } }
        #endregion

        private static Visibility _VisibleBusquedaProveedores = Visibility.Collapsed;
        public static Visibility VisibleBusquedaProveedores { get { return PopUpsViewModels._VisibleBusquedaProveedores; } }

        private static Visibility _VisibleBusquedaRequisicionesProductos = Visibility.Collapsed;
        public static Visibility VisibleBusquedaRequisicionesProductos { get { return PopUpsViewModels._VisibleBusquedaRequisicionesProductos; } }

        private static Visibility _VisibleBusquedaOrdenes_Compra = Visibility.Collapsed;
        public static Visibility VisibleBusquedaOrdenes_Compra { get { return PopUpsViewModels._VisibleBusquedaOrdenes_Compra; } }

        private static Visibility _VisibleAgenda = Visibility.Collapsed;
        public static Visibility VisibleAgenda { get { return PopUpsViewModels._VisibleAgenda; } }

        private static Visibility _VisibleBusquedaOrdenes_Compra_TransitoDetalle=Visibility.Collapsed;
        public static Visibility VisibleBusquedaOrdenes_Compra_TransitoDetalle { get { return PopUpsViewModels._VisibleBusquedaOrdenes_Compra_TransitoDetalle; } }

        private static Visibility _VisibleFondoObscuro = Visibility.Collapsed;
        public static Visibility VisibleFondoObscuro { get { return PopUpsViewModels._VisibleFondoObscuro; } }

        private static Visibility _VisibleAsignaLotesProducto = Visibility.Collapsed;
        public static Visibility VisibleAsignaLotesProducto { get { return PopUpsViewModels._VisibleAsignaLotesProducto; } }

        private static Visibility _VisibleRechazoProducto = Visibility.Collapsed;
        public static Visibility VisibleRechazoProducto { get { return PopUpsViewModels._VisibleRechazoProducto; } }

        private static Visibility _VisibleMensajeProxy = Visibility.Collapsed;
        public static Visibility VisibleMensajeProxy { get { return PopUpsViewModels._VisibleMensajeProxy; } }
        private static Visibility _VisibleDigitalizacionDocumento = Visibility.Collapsed;
        public static Visibility VisibleDigitalizacionDocumento { get { return PopUpsViewModels._VisibleDigitalizacionDocumento; } }

        private static Visibility _VisibleBusquedaRequisicion = Visibility.Collapsed;
        public static Visibility VisibleBusquedaRequisicion { get { return PopUpsViewModels._VisibleBusquedaRequisicion; } }

        #endregion

        #region [Metodos]
        public static void ShowPopUp(object ViewModel, TipoPopUp PopUpToShow)
        {
            if (ViewModel != _PopUpDataContext)
                _PopUpDataContext = null;

            _PopUpDataContext = ViewModel;
            RaiseStaticPropertyChanged("PopUpDataContext");

            switch (PopUpToShow)
            {
                case TipoPopUp.BUSQUEDA_PROVEEDORES:
                    _VisibleBusquedaProveedores = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleBusquedaProveedores");
                    break;
                case TipoPopUp.BUSQUEDA_REQUISICIONES:
                    _VisibleBusquedaRequisicionesProductos = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleBusquedaRequisicionesProductos");
                    break;
                case TipoPopUp.BUSQUEDA_ORDENES_COMPRA:
                    _VisibleBusquedaOrdenes_Compra = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleBusquedaOrdenes_Compra");
                    break;
                case TipoPopUp.AGENDA:
                    _VisibleAgenda = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleAgenda");
                    break;
                case TipoPopUp.BUSQUEDA_ORDENES_COMPRA_TRANSITO_DETALLE:
                    _VisibleBusquedaOrdenes_Compra_TransitoDetalle = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleBusquedaOrdenes_Compra_TransitoDetalle");
                    break;
                case TipoPopUp.ALMACEN_POPUP:
                    _VisibleAlmacenPopUp = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleAlmacenPopUp");
                    break;
                case TipoPopUp.PRODUCTO_UNIDAD_MEDIDAPOPUP:
                    _VisibleProducto_Unidad_MedidaPopUp = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleProducto_Unidad_MedidaPopUp");
                    break;
                case TipoPopUp.PRODUCTO_SUBCATEGORIAPOPUP:
                    _VisibleProducto_SubCategoriaPopUp = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleProducto_SubCategoriaPopUp");
                    break;
                case TipoPopUp.PRODUCTO_CATEGORIAPOPUP:
                    _VisibleProducto_CategoriaPopUp = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleProducto_CategoriaPopUp");
                    break;
                case TipoPopUp.ALMACEN_TIPOPOPUP:
                    _VisibleAlmacen_TipoPopUp = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleAlmacen_TipoPopUp");
                    break;
                case TipoPopUp.PRODUCTO_PRESENTACIONPOPUP:
                    _VisibleProducto_PresentacionPopUp = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleProducto_PresentacionPopUp");
                    break;
                case TipoPopUp.PRODUCTOPOPUP:
                    _VisibleProductoPopUp = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleProductoPopUp");
                    break;
                case TipoPopUp.ALMACEN_GRUPOPOPUP:
                    _VisibleAlmacen_GrupoPopUp = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleAlmacen_GrupoPopUp");
                    break;
                case TipoPopUp.FONDOOBSCURO:
                    _VisibleFondoObscuro = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleFondoObscuro");
                    break;
                case TipoPopUp.ENTRADAS_LOTES:
                    _VisibleAsignaLotesProducto = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleAsignaLotesProducto");
                    break;
                case TipoPopUp.RECHAZO_PRODUCTO:
                    _VisibleRechazoProducto = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleRechazoProducto");
                    break;
                case TipoPopUp.MENSAJEPROXY:
                    _VisibleMensajeProxy = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleMensajeProxy");
                    break;
                case TipoPopUp.DIGITALIZAR_DOCUMENTO:
                    _VisibleDigitalizacionDocumento = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleDigitalizacionDocumento");
                    break;
                case TipoPopUp.BUSQUEDA_REQUISICION_ESTATUS:
                    _VisibleBusquedaRequisicion = Visibility.Visible;
                    RaiseStaticPropertyChanged("VisibleBusquedaRequisicion");
                    break;
                default:
                    break;
            }
        }

        public static void ClosePopUp(TipoPopUp PopUpToClose = TipoPopUp.CERRAR_TODOS)
        {
            switch (PopUpToClose)
            {
                case TipoPopUp.BUSQUEDA_PROVEEDORES:
                    _VisibleBusquedaProveedores = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleBusquedaProveedores");
                    break;     
                case TipoPopUp.BUSQUEDA_REQUISICIONES:
                    _VisibleBusquedaRequisicionesProductos = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleBusquedaRequisicionesProductos");
                    break;
                case TipoPopUp.BUSQUEDA_ORDENES_COMPRA:
                    _VisibleBusquedaOrdenes_Compra = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleBusquedaOrdenes_Compra");
                    break;
                case TipoPopUp.AGENDA:
                    _VisibleAgenda = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleAgenda");
                    break;
                case TipoPopUp.BUSQUEDA_ORDENES_COMPRA_TRANSITO_DETALLE:
                    _VisibleBusquedaOrdenes_Compra_TransitoDetalle = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleBusquedaOrdenes_Compra_TransitoDetalle");
                    break;
                case TipoPopUp.ALMACEN_POPUP:
                    _VisibleAlmacenPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleAlmacenPopUp");
                    break;
                case TipoPopUp.PRODUCTO_UNIDAD_MEDIDAPOPUP:
                    _VisibleProducto_Unidad_MedidaPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleProducto_Unidad_MedidaPopUp");
                    break;
                case TipoPopUp.PRODUCTO_SUBCATEGORIAPOPUP:
                    _VisibleProducto_SubCategoriaPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleProducto_SubCategoriaPopUp");
                    break;
                case TipoPopUp.PRODUCTO_CATEGORIAPOPUP:
                    _VisibleProducto_CategoriaPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleProducto_CategoriaPopUp");
                    break;
                case TipoPopUp.ALMACEN_TIPOPOPUP:
                    _VisibleAlmacen_TipoPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleAlmacen_TipoPopUp");
                    break;
                case TipoPopUp.PRODUCTO_PRESENTACIONPOPUP:
                    _VisibleProducto_PresentacionPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleProducto_PresentacionPopUp");
                    break;
                case TipoPopUp.PRODUCTOPOPUP:
                    _VisibleProductoPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleProductoPopUp");
                    break;
                case TipoPopUp.ALMACEN_GRUPOPOPUP:
                    _VisibleAlmacen_GrupoPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleAlmacen_GrupoPopUp");
                    break;
                case TipoPopUp.FONDOOBSCURO:
                    _VisibleFondoObscuro = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleFondoObscuro");
                    break;
                case TipoPopUp.ENTRADAS_LOTES:
                    _VisibleAsignaLotesProducto = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleAsignaLotesProducto");
                    break;
                case TipoPopUp.RECHAZO_PRODUCTO:
                    _VisibleRechazoProducto = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleRechazoProducto");
                    break;
                case TipoPopUp.MENSAJEPROXY:
                    _VisibleMensajeProxy = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleMensajeProxy");
                    break;
                case TipoPopUp.DIGITALIZAR_DOCUMENTO:
                    _VisibleDigitalizacionDocumento = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleDigitalizacionDocumento");
                    break;
                case TipoPopUp.BUSQUEDA_REQUISICION_ESTATUS:
                    _VisibleBusquedaRequisicion = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleBusquedaRequisicion");
                    break;
                case TipoPopUp.CERRAR_TODOS:
                    _PopUpDataContext = null;
                    RaiseStaticPropertyChanged("PopUpDataContext");
                    _VisibleBusquedaProveedores = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleBusquedaProveedores");
                    _VisibleBusquedaRequisicionesProductos = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleBusquedaRequisicionesProductos");
                    _VisibleBusquedaOrdenes_Compra = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleBusquedaOrdenes_Compra");
                    _VisibleAgenda = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleAgenda");
                    _VisibleBusquedaOrdenes_Compra_TransitoDetalle = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleBusquedaOrdenes_Compra_TransitoDetalle");
                    _VisibleAlmacenPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleAlmacenPopUp");
                    _VisibleProducto_Unidad_MedidaPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleProducto_Unidad_MedidaPopUp");
                    _VisibleProducto_SubCategoriaPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleProducto_SubCategoriaPopUp");
                    _VisibleProducto_CategoriaPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleProducto_CategoriaPopUp");
                    _VisibleAlmacen_TipoPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleAlmacen_TipoPopUp");
                    _VisibleProducto_PresentacionPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleProducto_PresentacionPopUp");
                    _VisibleProductoPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleProductoPopUp");
                    _VisibleAlmacen_GrupoPopUp = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleAlmacen_GrupoPopUp");
                    _VisibleFondoObscuro = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleFondoObscuro");
                    _VisibleAsignaLotesProducto = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleAsignaLotesProducto");
                    _VisibleRechazoProducto = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleRechazoProducto");
                    _VisibleMensajeProxy = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleMensajeProxy");
                     _VisibleDigitalizacionDocumento = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleDigitalizacionDocumento");
                    _VisibleBusquedaRequisicion = Visibility.Collapsed;
                    RaiseStaticPropertyChanged("VisibleBusquedaRequisicion");
                    break;
            }
        }
        #endregion

        #region [Aux]
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged(string propName)
        {
            EventHandler<PropertyChangedEventArgs> handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
