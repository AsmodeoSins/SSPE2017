using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GESAL.Views;
using System.Windows;
using MVVMShared.ViewModels;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Almacenes;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Principales.Almacenes;
using SSP.Controlador.Principales.Compartidos;
using GESAL.Clases.Extendidas;
using GESAL.Clases.ExtensionesClases;
using System.Threading.Tasks;
using SSP.Servidor.ModelosExtendidos;
using GESAL.Clases.Misc;
using WPFPdfViewer;
using MVVMShared.Views;
using System.IO;
using System.Threading;
using SSP.Controlador.Enums;
using System.Windows.Controls;
using System.Windows.Data;
using System.Reflection;
namespace GESAL.ViewModels
{
    public partial class EntradasAlmacenesViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        private EntradasAlmacenesView _window;
        private async void OnLoad(object sender)
        {
            var _modeloAutenticacion = new AutenticacionViewModel(_dialogCoordinator,_usuario);
            var _vistaAutenticacion = new AutenticacionView();
            _vistaAutenticacion.DataContext = _modeloAutenticacion;
            _vistaAutenticacion.Owner = PopUpsViewModels.MainWindow;
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FONDOOBSCURO);

            if (_vistaAutenticacion.ShowDialog() == true)
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FONDOOBSCURO);
                StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
                await Task.Factory.StartNew(() => {
                    CargarAlmacen_TipoCat();
                    CargarMunicipios();
                    CargarCentros(SelectedMunicipio.ID_MUNICIPIO);
                    CargarAlmacenesPrincipales(SelectedAlmacen_Tipo_Cat.ID_ALMACEN_TIPO, SelectedCentro.ID_CENTRO);
                    if (SelectedAlmacenPrincipal != null)
                        CargarOrdenesCalendarizadas(FechaActual, SelectedAlmacenPrincipal.ID_ALMACEN);

                    if (SelectedAlmacenPrincipal != null && SelectedAlmacenPrincipal.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == "M")
                    {
                        CargarFechaMinimaCaducidadMedicamento(SelectedCentro.ID_CENTRO);
                        if (fechaCaducidadMinimaMedicamento.HasValue)
                            MensajeFechaCaducidad = "LA FECHA DE CADUCIDAD TIENE QUE SER MAYOR A " + fechaCaducidadMinimaMedicamento.Value.ToString() + "MESES!";
                    }
                });
                StaticSourcesViewModel.CloseProgressLoading();
                StaticSourcesViewModel.SourceChanged = false;
                _window = (EntradasAlmacenesView)sender;
            }
            else
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FONDOOBSCURO);
                PopUpsViewModels.MainWindow.contenedorVistas.Content = null;
                PopUpsViewModels.MainWindow.contenedorVistas.DataContext = null;
            }
        }

        #region Metodos Filtros
        private async void MunicipioCambio(int id_municipio)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {

                CargarCentros(id_municipio);
                CargarAlmacenesPrincipales(SelectedAlmacen_Tipo_Cat.ID_ALMACEN_TIPO,selectedCentro.ID_CENTRO);
                if (SelectedAlmacenPrincipal != null)
                    CargarOrdenes(SelectedAlmacenPrincipal.ID_ALMACEN);
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

        private async void CentroCambio(short id_centro)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {

                CargarAlmacenesPrincipales(SelectedAlmacen_Tipo_Cat.ID_ALMACEN_TIPO,id_centro);
                if (SelectedAlmacenPrincipal != null)
                    CargarOrdenes(SelectedAlmacenPrincipal.ID_ALMACEN);
                if (SelectedAlmacenPrincipal != null && SelectedAlmacenPrincipal.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == "M")
                {
                    CargarFechaMinimaCaducidadMedicamento(id_centro);
                    if (fechaCaducidadMinimaMedicamento.HasValue)
                        MensajeFechaCaducidad = "LA FECHA DE CADUCIDAD TIENE QUE SER MAYOR A " + fechaCaducidadMinimaMedicamento.Value.ToString() + "MESES!";
                }
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

        private async void Almacen_TipoCambio(short id_almacen_tipo_cat)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                CargarAlmacenesPrincipales(id_almacen_tipo_cat, SelectedCentro.ID_CENTRO);
                if (SelectedAlmacenPrincipal != null)
                    CargarOrdenes(SelectedAlmacenPrincipal.ID_ALMACEN);
                if (SelectedAlmacenPrincipal != null && SelectedAlmacenPrincipal.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == "M")
                {
                    CargarFechaMinimaCaducidadMedicamento(SelectedCentro.ID_CENTRO);
                    if (fechaCaducidadMinimaMedicamento.HasValue)
                        MensajeFechaCaducidad = "LA FECHA DE CADUCIDAD TIENE QUE SER MAYOR A " + fechaCaducidadMinimaMedicamento.Value.ToString() + " MESES!";
                }
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch(Exception ex)
            {
                _error = true;
            }
            if(_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }

        private async void AlmacenPrincipalCambio(ALMACEN almacen)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                if (SelectedAlmacenPrincipal != null)
                    CargarOrdenes(SelectedAlmacenPrincipal.ID_ALMACEN);
                if (almacen.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == "M")
                {
                    CargarFechaMinimaCaducidadMedicamento(SelectedCentro.ID_CENTRO);
                    if (fechaCaducidadMinimaMedicamento.HasValue)
                        MensajeFechaCaducidad = "LA FECHA DE CADUCIDAD TIENE QUE SER MAYOR A " + fechaCaducidadMinimaMedicamento.Value.ToString() + "MESES!";
                }
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

        private void CargarMunicipios()
        {
            try
            {
                Municipios = new ObservableCollection<MUNICIPIO>(new cMunicipio().Obtener(2).ToList());
                selectedMunicipio = Municipios.Where(w => w.ID_MUNICIPIO != 0).First();
                RaisePropertyChanged("SelectedMunicipio");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los municipios. Favor de contactar al administrador");
            }
        }

        private void CargarCentros(int id_municipio)
        {
            try
            {
                Centros = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos(string.Empty, 0, id_municipio).ToList());
                selectedCentro = Centros.FirstOrDefault();
                RaisePropertyChanged("SelectedCentro");

            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los centros. Favor de contactar al administrador");
            }
        }

        private void CargarAlmacenesPrincipales(short id_almacen_tipo_cat,short id_centro)
        {
            try
            {
                AlmacenesPrincipales  = new ObservableCollection<ALMACEN>(new cAlmacen().SeleccionarAlmacenesPrincipales(id_almacen_tipo_cat, id_centro, _usuario.Almacen_Grupo, "S"));
                selectedAlmacenPrincipal = AlmacenesPrincipales.FirstOrDefault();
                RaisePropertyChanged("SelectedAlmacenPrincipal");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los almacenes. Favor de contactar al administrador");
            }
        }

        private void CargarAlmacen_TipoCat()
        {
            try
            {
                Almacen_Tipos_Cat = new ObservableCollection<ALMACEN_TIPO_CAT>(new cAlmacen_Tipo_Cat().Seleccionar(_usuario.Almacen_Grupo, "S"));
                selectedAlmacen_Tipo_Cat = Almacen_Tipos_Cat.FirstOrDefault();
                RaisePropertyChanged("SelectedAlmacen_Tipo_Cat");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los tipos de almacen. Favor de contactar al administrador");
            }
        }

        private async void Cargar_Incidente_Tipos()
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                Incidencia_Tipos = new ObservableCollection<INCIDENCIA_TIPO>(new cIncidencia_Tipo().Seleccionar(SelectedAlmacen_Tipo_Cat.ID_ALMACEN_GRUPO,"S").ToList());
                var dummy = new INCIDENCIA_TIPO {
                    ACTIVO="S",
                    ID_ALMACEN_GRUPO=_usuario.Almacen_Grupo,
                    DESCR="SELECCIONA UNA", 
                    ID_TIPO_INCIDENCIA=-1
                };
                Incidencia_Tipos.Add(dummy);
                SelectedIncidencia_Tipo = dummy;
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
            }
        }
        #endregion

        private void CargarFechaMinimaCaducidadMedicamento(int id_centro)
        {
            try
            {
                var temp=new cParametro().Seleccionar("MED_CADUCIDAD_MAX_PROVEEDOR", id_centro);
                if (temp == null)
                    fechaCaducidadMinimaMedicamento = null;
                else
                    fechaCaducidadMinimaMedicamento = temp.First().VALOR_NUM;
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar la fecha de caducidad de medicamentos. Favor de contactar al administrador");
            }
        }

        private void CargarOrdenesCalendarizadas(DateTime fecha,int id_almacen)
        {
            try
            {
                Ordenes_Calendarizadas=new ObservableCollection<ORDEN_COMPRA> (new cOrden_Compra().SeleccionarOrdenesCalendarizadas(fecha,id_almacen));
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar la calendarizacion de entregas. Favor de contactar al administrador");
            }
        }

        private void CargarOrdenesPorEstatus(List<string> estatus, short id_almacen) //regresa las ordenes capturadas por almacen para el caso donde se recibe la OC sin calendarizacion y de manera total
        {
            try
            {
                Ordenes_Manual = new ObservableCollection<ORDEN_COMPRA>(new cOrden_Compra().SeleccionarPorEstatus(estatus,id_almacen)
                    .Where(w=>w.ORDEN_COMPRA_DETALLE.Any(a=>a.DIFERENCIA>0 && a.ID_ALMACEN==id_almacen)
                     && (w.MOVIMIENTO.Any(a=>a.ID_ORDEN_COMPRA==w.ID_ORDEN_COMPRA && a.ID_ALMACEN==id_almacen && !a.CALENDARIZAR_ENTREGA.Any(a2=>a2.ID_ENTRADA==a.ID_MOVIMIENTO))||!w.MOVIMIENTO.Any(a=>a.ID_ORDEN_COMPRA==w.ID_ORDEN_COMPRA))
                     && (w.INCIDENCIA.Any(a=>a.ID_ORDEN_COMPRA==w.ID_ORDEN_COMPRA && a.ID_ALMACEN==id_almacen/* && !a.CALENDARIZAR_ENTREGA.Any(a2=>a2.ID_INCIDENCIA==a.ID_INCIDENCIA)*/)||!w.INCIDENCIA.Any(a=>a.ID_ORDEN_COMPRA==w.ID_ORDEN_COMPRA)))
                    .ToList());
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar la calendarizacion de entregas. Favor de contactar al administrador");
            }
        }

        private async void ValidarCambioOnOrdenChanged()
        {
            if (StaticSourcesViewModel.SourceChanged)
                if (await (_dialogCoordinator.ShowMessageAsync(this, "Advertencia", "Hay cambios sin guardar, ¿Seguro que desea cambiar de orden sin guardar?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative)) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                {
                    HeaderProductoGrupo = "Forma de Recepción de Productos de la Orden " + SelectedOrden_Calendarizada.NUM_ORDEN;
                    IsGridValido = false;
                    IsRecepcionProductosVisible = true;
                    HabilitarAlmacen_Tipo_Cat = false;
                    HabilitarAlmacenPrincipal = false;
                    HabilitarCentro = false;
                    HabilitarMunicipio = false;
                    setValidationRulesEntradasAlmacen();                    
                    CargarProductos(SelectedAlmacenPrincipal.ID_ALMACEN, SelectedOrden_Calendarizada.ID_ORDEN_COMPRA, FechaActual);
                }
                else
                {

                    selectedOrden_Calendarizada = selectedOrden_CalendarizadaOld;
                    RaisePropertyChanged("SelectedOrden_Calendarizada");
                }
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
                CargarProductos(SelectedAlmacenPrincipal.ID_ALMACEN, SelectedOrden_Calendarizada.ID_ORDEN_COMPRA, FechaActual);
            }
                
        }

        private async void CargarProductos(int id_almacen, int id_orden_compra, DateTime fecha)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                if (SelectedTabindexOrdenes == 0)
                {
                    Productos = new ObservableCollection<EXT_RECEPCION_PRODUCTOS>(new cCalendarizar_Entrega_Producto().Seleccionar(id_almacen, id_orden_compra, fecha.Date)
                        .Where(w=>w.ESTATUS=="PR")
                        .AsEnumerable()
                        .Select(s => new EXT_RECEPCION_PRODUCTOS
                        {
                            DESCRIPCION = s.PRODUCTO.DESCRIPCION,
                            ID_PRODUCTO = s.PRODUCTO.ID_PRODUCTO,
                            NOMBRE = s.PRODUCTO.NOMBRE,
                            ORDENADO = s.CANTIDAD.Value,
                            RESTANTE = s.CANTIDAD.Value,
                            RECEPCION_PRODUCTO_DETALLE = new List<EXT_RECEPCION_PRODUCTO_DETALLE>(),
                            RECIBIDO = 0,
                            RECIBIDOOLD = 0,
                            UNIDAD_MEDIDA = s.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE
                        }).ToList());
                    //
                    
                }
                else
                {
                    Productos = new ObservableCollection<EXT_RECEPCION_PRODUCTOS>(new cOrden_Compra_Detalle().Seleccionar(SelectedOrden_Calendarizada.ID_ORDEN_COMPRA,
                        SelectedAlmacenPrincipal.ID_ALMACEN)
                        .Where(w => w.DIFERENCIA.Value > 0)
                        .AsEnumerable()
                        .Select(s => new EXT_RECEPCION_PRODUCTOS
                        {
                            DESCRIPCION = s.PRODUCTO.DESCRIPCION,
                            ID_PRODUCTO = s.ID_PRODUCTO,
                            NOMBRE = s.PRODUCTO.NOMBRE,
                            ORDENADO = s.DIFERENCIA.Value,
                            RESTANTE = s.DIFERENCIA.Value,
                            RECEPCION_PRODUCTO_DETALLE = new List<EXT_RECEPCION_PRODUCTO_DETALLE>(),
                            RECIBIDO = 0,
                            RECIBIDOOLD = 0,
                            UNIDAD_MEDIDA = s.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE
                        }).ToList());
                }
                StaticSourcesViewModel.CloseProgressLoading();
                if (Productos.Count > 0)
                {
                    listaeventos = new List<KeyValuePair<DataGridRow, EventHandler<DataTransferEventArgs>>>();
                    var dg = _window.dgProductos;
                    dg.UpdateLayout(); //como el datagrid esta virtualizado no existe todavia el contenedor, hay que actualizarlo para que genere los contenedores (datagridrow)                    
                    var drows = DataGridHelper.GetDataGridRows(dg);
                    foreach (var item in drows)
                    {
                        TextBox tb = DataGridHelper.FindChild<TextBox>(item, "dgItemRecibido");
                        EventHandler<DataTransferEventArgs> evento = new EventHandler<DataTransferEventArgs>((s, e) => ConfirmacionIncidenciaProducto(item, null));
                        tb.SourceUpdated += evento;
                        listaeventos.Add(new KeyValuePair<DataGridRow, EventHandler<DataTransferEventArgs>>(item, evento));
                    }
                }


            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los productos. Favor de contactar al administrador");
            }
        }

        private bool ValidarGrid()
        {
            foreach (var item in Productos)
            {
                if (item.HasErrors)
                    return false;
            }
            return true;
        }

        private void DblClickSwitch(object sender)
        {
            
            if (SelectedProducto!=null && SelectedProducto.ISCMDBLCLICK_HABILITADO)
            {
                CondicionesInicialesAsignaLote();
                if (SelectedProducto.RECEPCION_PRODUCTO_DETALLE == null || SelectedProducto.RECEPCION_PRODUCTO_DETALLE.Count == 0)
                {
                    Recepcion_Producto_Detalle = new List<EXT_RECEPCION_PRODUCTO_DETALLE>();
                    Restante = SelectedProducto.ORDENADO;
                }
                else
                {
                    Recepcion_Producto_Detalle = (List<EXT_RECEPCION_PRODUCTO_DETALLE>)SelectedProducto.RECEPCION_PRODUCTO_DETALLE.Clone();
                    Restante = SelectedProducto.ORDENADO - Recepcion_Producto_Detalle.Select(s => s.RECIBIDO).Sum();
                    HabilitarAsignarLotesOk = true;
                }
                if (SelectedAlmacenPrincipal.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == "M")
                    IsFechaCaducidadValid = false;
                else
                    IsFechaCaducidadValid = true;
                setValidationRulesAsignaLotes();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ENTRADAS_LOTES);
            }
        }

        private async void CapturaEntradaLote(object parameter)
        {
            switch (parameter.ToString())
            {
                case "ok":
                    base.ClearRules();

                    if (Recepcion_Producto_Detalle.Count > 0)
                    {
                        if (Restante==0 || Restante==SelectedProducto.ORDENADO)
                        {

                            SelectedProducto.RECEPCION_PRODUCTO_DETALLE = Recepcion_Producto_Detalle;
                            SelectedProducto.RECIBIDO = SelectedProducto.ORDENADO - Restante;
                            SelectedProducto.RECIBIDOOLD = SelectedProducto.RECIBIDO;
                            SelectedProducto.ISENABLED_RECIBIDO = false;
                            //si hubo captura de incidencia, quitarla.
                            SelectedProducto.INCIDENCIA_OBSERVACIONES = string.Empty;
                            SelectedProducto.INCIDENCIA_TIPO = null;
                            SelectedProducto.FechaRecalendarizacion = null;
                            setValidationRulesEntradasAlmacen();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ENTRADAS_LOTES);
                            IsGridValido = ValidarGrid();

                        }
                        else
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ENTRADAS_LOTES);
                            if (await _dialogCoordinator.ShowMessageAsync(this, "Mensaje de Confirmación", "¿Esta seguro de recibir el producto? La cantidad recibida es menor a la ordenada y se generara una incidencia sobre este producto.", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                            {
                                EncabezadoIncidenciaProducto = "Capturar incidencia de rechazo de producto " + SelectedProducto.NOMBRE;
                                Cargar_Incidente_Tipos();
                                Observacion_Incidencia = string.Empty;
                                FechaRecalidarizarProductoRechazado = null;
                                setValidacionRulesRechazoProducto();
                                producto_Rechazar = SelectedProducto;
                                producto_Rechazar.RESTANTE = Restante;
                                RaisePropertyChanged("Producto_Rechazar");
                                SelectedFechasEntregaProveedor = new cCalendarizar_Entrega_Producto().SeleccionarFechasEntregaProductoProveedorRestantesMes(SelectedAlmacenPrincipal.ID_ALMACEN,
                                        SelectedOrden_Calendarizada.ID_PROV.Value, Producto_Rechazar.ID_PRODUCTO, FechaActual.Month).Select(s => s.CALENDARIZAR_ENTREGA.FEC_PACTADA.Value).ToList();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RECHAZO_PRODUCTO);
                                provieneDeEntradaLotes = true;
                            }
                        }
                    }
                    else
                    {
                        //si hubo captura de incidencia, quitarla.
                        SelectedProducto.INCIDENCIA_OBSERVACIONES = string.Empty;
                        SelectedProducto.INCIDENCIA_TIPO = null;
                        SelectedProducto.FechaRecalendarizacion = null;

                        SelectedProducto.RECEPCION_PRODUCTO_DETALLE = Recepcion_Producto_Detalle;
                        SelectedProducto.RECIBIDO = 0;
                        SelectedProducto.ISENABLED_RECIBIDO = true;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ENTRADAS_LOTES);
                        setValidationRulesEntradasAlmacen();
                        IsGridValido = ValidarGrid();
                    }
                   

                    break;
                case "cancelado":
                    base.ClearRules();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ENTRADAS_LOTES);
                    setValidationRulesEntradasAlmacen();
                    break;
            }
        }

        private void CondicionesInicialesAsignaLote()
        {
            HabilitarAsignarLotesOk = false;
            LimpiarAsignaLote();
        }

        private void LimpiarAsignaLote()
        {
            Lote = null;
            Recibido = null;
            Fecha_Caducidad = null;
        }

        private void AsignaLotesSwitch(object parametro)
        {
            var temp = (List<EXT_RECEPCION_PRODUCTO_DETALLE>)Recepcion_Producto_Detalle.Clone();
            switch (parametro.ToString())
            {

                case "agregar_lote":
                    temp.Add(new EXT_RECEPCION_PRODUCTO_DETALLE {
                        FECHA_CADUCIDAD=Fecha_Caducidad,
                        ID_PRODUCTO=SelectedProducto.ID_PRODUCTO,
                        LOTE=Lote.Value,
                        RECIBIDO=Recibido.Value
                    });
                    Restante -= Recibido.Value;
                    Recepcion_Producto_Detalle = temp;
                    HabilitarAsignarLotesOk = true;
                    LimpiarAsignaLote();
                    break;
                case "eliminar":
                    Restante += SelectedRecepcion_Producto.RECIBIDO;
                    temp.Remove(temp.FirstOrDefault(w=>w.ID_PRODUCTO==SelectedRecepcion_Producto.ID_PRODUCTO && w.LOTE==SelectedRecepcion_Producto.LOTE ));
                    Recepcion_Producto_Detalle = temp;
                    if (Recepcion_Producto_Detalle.Count == 0 && SelectedProducto.RECEPCION_PRODUCTO_DETALLE.Count==0)
                        HabilitarAsignarLotesOk = false;
                    else
                        HabilitarAsignarLotesOk = true;
                    break;
            }
        }

        private async void RaiseChangeCheckedProductos(object obj)
        {
            RaisePropertyChanged("Productos");
            IsGridValido=ValidarGrid();
            if (SelectedProducto.IS_CHECKED)
            {
                if (await _dialogCoordinator.ShowMessageAsync(this, "Mensaje de Confirmación", "¿Esta seguro de rechazar el producto? Se generara una incidencia sobre este producto.", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                {
                    EncabezadoIncidenciaProducto = "Capturar incidencia de rechazo de producto " + SelectedProducto.NOMBRE;
                    Cargar_Incidente_Tipos();
                    Observacion_Incidencia = string.Empty;
                    FechaRecalidarizarProductoRechazado = null;
                    setValidacionRulesRechazoProducto();
                    Producto_Rechazar = SelectedProducto;
                    SelectedFechasEntregaProveedor = new cCalendarizar_Entrega_Producto().SeleccionarFechasEntregaProductoProveedorRestantesMes(SelectedAlmacenPrincipal.ID_ALMACEN,
                            SelectedOrden_Calendarizada.ID_PROV.Value, Producto_Rechazar.ID_PRODUCTO, FechaActual.Month).Select(s => s.CALENDARIZAR_ENTREGA.FEC_PACTADA.Value).ToList();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RECHAZO_PRODUCTO);
                    provieneDeEntradaLotes = false;
                    
                }
                else
                {
                    SelectedProducto.IS_CHECKED = false;
                    SelectedProducto.FechaRecalendarizacion = null;
                    SelectedProducto.INCIDENCIA_OBSERVACIONES = null;
                    SelectedProducto.INCIDENCIA_TIPO = null;
                    if (SelectedTabIndex == 1)
                        ActivarValidacionesProductoGrid();
                }
            }
        }

        

        private async void ConfirmacionIncidenciaProducto(Object sender, DataTransferEventArgs args)
        {
            try
            {
                if (sender != null)
                {
                    IsGridValido = ValidarGrid();
                    var param = (EXT_RECEPCION_PRODUCTOS)((DataGridRow)sender).Item;
                    var _recibido = param.RECIBIDO;
                    var _ordenado = param.ORDENADO;
                    var _id_producto = param.ID_PRODUCTO;
                    Producto_Rechazar = Productos.First(w => w.ID_PRODUCTO == _id_producto);
                    if (_recibido != 0 && _recibido < _ordenado)
                    {
                        
                        if (await (_dialogCoordinator.ShowMessageAsync(this,"Mensaje de Confirmación", "¿Esta seguro de recibir el producto? La cantidad recibida es menor a la ordenada y se generara una incidencia sobre este producto.",MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative)) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                        {
                            SelectedFechasEntregaProveedor = new cCalendarizar_Entrega_Producto().SeleccionarFechasEntregaProductoProveedorRestantesMes(SelectedAlmacenPrincipal.ID_ALMACEN,
                                SelectedOrden_Calendarizada.ID_PROV.Value, Producto_Rechazar.ID_PRODUCTO, FechaActual.Month).Select(s => s.CALENDARIZAR_ENTREGA.FEC_PACTADA.Value).ToList();
                            selectedFechasEntregaProveedorCopia = new DateTime[SelectedFechasEntregaProveedor.Count];
                            SelectedFechasEntregaProveedor.CopyTo(selectedFechasEntregaProveedorCopia);
                            EncabezadoIncidenciaProducto = "Capturar incidencia de rechazo de producto " + Producto_Rechazar.NOMBRE;
                            Cargar_Incidente_Tipos();
                            Observacion_Incidencia = string.Empty;
                            FechaRecalidarizarProductoRechazado = null;
                            setValidacionRulesRechazoProducto();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RECHAZO_PRODUCTO);
                            provieneDeEntradaLotes = false;
                        }
                        else
                        {
                            //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.MENSAJEPROXY);
                            Producto_Rechazar.RECIBIDO = Producto_Rechazar.RECIBIDOOLD;
                            IsGridValido = ValidarGrid();
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }

        }
        private async void BuscarOC(object parametro)
        {
            if (SelectedTabindexOrdenes==0)
            {
                var _error = false;
                StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
                try
                {
                    await Task.Factory.StartNew(() =>
                        {
                    Ordenes_Calendarizadas = new ObservableCollection<ORDEN_COMPRA>(new cOrden_Compra().SeleccionarOrdenesCalendarizadas(FechaActual, SelectedAlmacenPrincipal.ID_ALMACEN).ToList());
                    int OC = 0;
                    if (!string.IsNullOrWhiteSpace(parametro.ToString()) && int.TryParse(parametro.ToString(), out OC))
                    {
                        Ordenes_Calendarizadas = new ObservableCollection<ORDEN_COMPRA>(Ordenes_Calendarizadas.Where(w => w.NUM_ORDEN == OC));
                    }
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
                    await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al filtrar las ordenes. Favor de contactar al administrador");
                }
               
            }
            else
            {
                if (Ordenes_Manual != null)
                {
                    var _error = false;
                    StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
                    try
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            var temp = Ordenes_Manual;
                            int OC = 0;
                            if (!string.IsNullOrWhiteSpace(parametro.ToString()) && int.TryParse(parametro.ToString(), out OC))
                                Ordenes_ManualCopy = new ObservableCollection<ORDEN_COMPRA>(temp.Where(w => w.NUM_ORDEN.ToString().Contains(OC.ToString())));
                            else
                                Ordenes_ManualCopy = temp;
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
                        await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al filtrar las ordenes. Favor de contactar al administrador");
                    }
                }
            }
        }

        private void ActivarValidacionesProductoGrid()
        {
            if (!Productos.Any(a => a.IS_CHECKED))
                foreach (var item in Productos)
                    item.HabilitarValidacion();
        }

        private void ValidarFechaCaducidad()
        {
            if (SelectedAlmacenPrincipal.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO != "M")
                IsFechaCaducidadValid = true;
            else
            {
                var fecha_minima_caducidad=DateTime.Now.Date;
                fecha_minima_caducidad= fecha_minima_caducidad.AddMonths(fechaCaducidadMinimaMedicamento.Value);
                if (Fecha_Caducidad.HasValue && Fecha_Caducidad.Value >= fecha_minima_caducidad)
                    IsFechaCaducidadValid = true;
                else
                    IsFechaCaducidadValid = false;
            }
        }

        private void CargarOrdenes(short id_almacen)
        {
            Productos = null;
            IsRecepcionProductosVisible = false;
            try
            {
                if (SelectedTabindexOrdenes == 0)
                {
                    BusquedaParametro = string.Empty;
                    CargarOrdenesCalendarizadas(FechaActual, id_almacen);
                }
                else
                {
                    BusquedaParametroManual = string.Empty;
                    CargarOrdenesPorEstatus(new List<string>{"CP","PR"}, id_almacen);
                }
            }catch(Exception ex)
            {
                throw new Exception("Hubo un error al cargar las ordenes. Favor de contactar al administrador");
            }
            
        }

        private async void OnSelectedTabIndexOrdenesChanged(object propiedad)
        {
            DataGridRow selectedrow = null;
            var dg = _window.dgProductos;
            var drows = DataGridHelper.GetDataGridRows(dg);
            if (dg.Items.Count>0)
            {
                foreach (var item in drows)
                {
                    if (item.IsSelected)
                    {
                        selectedrow = item;
                        TextBox tb = DataGridHelper.FindChild<TextBox>(item, "dgItemRecibido");
                        foreach (var itemevento in listaeventos)
                        {
                            if (itemevento.Key == item)
                                tb.SourceUpdated -= itemevento.Value;
                            break;
                        }
                        break;
                    }
                }
            }
            decimal recibidoOld = (selectedProducto!=null && selectedProducto.RECIBIDO>0)?selectedProducto.RECIBIDO:0;
            await ValidarOnSelectedTabIndexOrdenesChanged(selectedrow,recibidoOld);
            if (dg.Items.Count > 0)
            {
                foreach (var item in drows)
                {
                    if (item.IsSelected)
                    {
                        TextBox tb = DataGridHelper.FindChild<TextBox>(item, "dgItemRecibido");
                        foreach (var itemevento in listaeventos)
                        {
                            if (itemevento.Key == item)
                                tb.SourceUpdated += itemevento.Value;
                            break;
                        }
                        break;
                    }
                }
            }
        }

        private async Task ValidarOnSelectedTabIndexOrdenesChanged(DataGridRow selectedrow, decimal recibidoOld)
        {
            
            if (StaticSourcesViewModel.SourceChanged)
                if (await (_dialogCoordinator.ShowMessageAsync(this,"Advertencia", "Hay cambios sin guardar, ¿Seguro que desea cambiar de pestaña sin guardar?",MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative)) ==MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                {            
                    selectedOrden_Calendarizada = null;
                    selectedOrden_CalendarizadaOld = null;
                    RaisePropertyChanged("SelectedOrden_Calendarizada");
                    HabilitarAlmacen_Tipo_Cat = true;
                    HabilitarAlmacenPrincipal = true;
                    HabilitarCentro = true;
                    HabilitarMunicipio = true;
                    SelectedTabIndexOrdenesChanged();
                    StaticSourcesViewModel.SourceChanged = false;
                }
                else
                {
                    selectedTabindexOrdenes = selectedTabindexOrdenesOld;
                    RaisePropertyChanged("SelectedTabindexOrdenes");
                    if (selectedrow!=null && recibidoOld!=SelectedProducto.RECIBIDO)
                    {
                        ConfirmacionIncidenciaProducto(selectedrow, null);
                    }
                }
            else
            {
                SelectedTabIndexOrdenesChanged();
            }
        }

        private void SelectedTabIndexOrdenesChanged()
        {
            IsRecepcionProductosVisible = false;
            if (SelectedTabindexOrdenes == 0)
            {
                IsRecalendarizarVisible = true;
                BusquedaParametro = string.Empty;

                try
                {
                    if (SelectedAlmacenPrincipal != null)
                        CargarOrdenesCalendarizadas(FechaActual, SelectedAlmacenPrincipal.ID_ALMACEN);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                IsRecalendarizarVisible = false;
                BusquedaParametroManual = string.Empty;
                try
                {
                    if (SelectedAlmacenPrincipal != null)
                        CargarOrdenesPorEstatus(new List<string>{"CP","PR"},SelectedAlmacenPrincipal.ID_ALMACEN);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #region Metodos Rechazar Producto
        private void SelectedTabIndexChanged ()
        {
            //quitar funcionalidad
            DataGridRow selectedrow = null;
            var dg = _window.dgProductos;
            var drows = DataGridHelper.GetDataGridRows(dg);
            if (dg.Items.Count > 0)
            {
                foreach (var item in drows)
                {
                    if (item.IsSelected)
                    {
                        selectedrow = item;
                        TextBox tb = DataGridHelper.FindChild<TextBox>(item, "dgItemRecibido");
                        foreach (var itemevento in listaeventos)
                        {
                            if (itemevento.Key == item)
                                tb.SourceUpdated -= itemevento.Value;
                            break;
                        }
                        break;
                    }
                }
            }
            decimal recibidoOld = (selectedProducto != null && selectedProducto.RECIBIDO > 0) ? selectedProducto.RECIBIDO : 0;

            CargarProductos(SelectedAlmacenPrincipal.ID_ALMACEN, SelectedOrden_Calendarizada.ID_ORDEN_COMPRA, FechaActual.Date);
            if (SelectedTabIndex==1)
            {
                IsRecalendarizarVisible = false;
                IsRechazo_Entrada_Valido = false;
                setValidacionRulesRechazoEntrada();
            }
            else
            {
                if (SelectedTabindexOrdenes == 0)
                    IsRecalendarizarVisible = true;
                else
                    IsRecalendarizarVisible = false;
                IsGridValido = false;
                setValidationRulesEntradasAlmacen();
            }
        }

        private void RechazarProductoSwitch(object parametro)
        {
            var _producto = Productos.First(w => w.ID_PRODUCTO == Producto_Rechazar.ID_PRODUCTO);
            switch (parametro.ToString())
            {
                case "ok":
                    if(provieneDeEntradaLotes)
                    {
                        SelectedProducto.RECEPCION_PRODUCTO_DETALLE = Recepcion_Producto_Detalle;
                        SelectedProducto.RECIBIDO = SelectedProducto.ORDENADO - Restante;
                        SelectedProducto.RECIBIDOOLD = SelectedProducto.RECIBIDO;
                        SelectedProducto.ISENABLED_RECIBIDO = false;
                        IsGridValido = ValidarGrid();
                    }
                    _producto.INCIDENCIA_TIPO = SelectedIncidencia_Tipo;
                    _producto.INCIDENCIA_OBSERVACIONES = Observacion_Incidencia;
                    _producto.FechaRecalendarizacion = FechaRecalidarizarProductoRechazado;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RECHAZO_PRODUCTO);
                    IsRechazo_Entrada_Valido = true;
                    
                    if (SelectedTabIndex == 1)
                    {
                        DesactivarValidacionesProductoGrid();
                        setValidacionRulesRechazoEntrada();
                    }
                    else
                        setValidationRulesEntradasAlmacen();
                    break;
                case "cancelado":
                    if(provieneDeEntradaLotes)
                    {
                        SelectedProducto.RECEPCION_PRODUCTO_DETALLE = null;
                        SelectedProducto.RECIBIDO = 0;
                        SelectedProducto.ISENABLED_RECIBIDO = true;
                        
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RECHAZO_PRODUCTO);
                    _producto.RECIBIDO = _producto.RECIBIDOOLD;
                    _producto.IS_CHECKED = false;
                    _producto.INCIDENCIA_OBSERVACIONES = string.Empty;
                    _producto.INCIDENCIA_TIPO = null;
                    _producto.FechaRecalendarizacion = null;
                     setValidationRulesEntradasAlmacen();
                     IsGridValido = ValidarGrid();
                    break;
            }
        }

        private bool ChecarRechazo_Entrada_Valido()
        {
            return Productos.Any(w => w.IS_CHECKED);
        }

        private void DesactivarValidacionesProductoGrid()
        {
            var temp = new ObservableCollection<EXT_RECEPCION_PRODUCTOS>(Productos.Clone());
            foreach (var item in temp)
                if (!item.IS_CHECKED)
                    item.InhabilitarValidacion();
            Productos = temp;
        }

        //private void MessageDialogoSwitch(object parametro)
        //{
        //    if (parametro!=null)
        //    {
        //        switch(parametro.ToString())
        //        {
        //            case "ok":
        //                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.MENSAJEPROXY);
        //                SelectedFechasEntregaProveedor = new cCalendarizar_Entrega_Producto().SeleccionarFechasEntregaProductoProveedorRestantesMes(SelectedAlmacenPrincipal.ID_ALMACEN,
        //                    SelectedOrden_Calendarizada.ID_PROV.Value, Producto_Rechazar.ID_PRODUCTO, FechaActual.Month).Select(s => s.CALENDARIZAR_ENTREGA.FEC_PACTADA.Value).ToList();
        //                selectedFechasEntregaProveedorCopia=new DateTime[SelectedFechasEntregaProveedor.Count];
        //                SelectedFechasEntregaProveedor.CopyTo(selectedFechasEntregaProveedorCopia);
        //                EncabezadoIncidenciaProducto = "Capturar incidencia de rechazo de producto " + Producto_Rechazar.NOMBRE;
        //                Cargar_Incidente_Tipos();
        //                Observacion_Incidencia = string.Empty;
        //                FechaRecalidarizarProductoRechazado = null;
        //                setValidacionRulesRechazoProducto();
        //                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RECHAZO_PRODUCTO);
        //                provieneDeEntradaLotes = false;
        //                break;
        //            case "cancelar":
        //                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.MENSAJEPROXY);
        //                Producto_Rechazar.RECIBIDO=Producto_Rechazar.RECIBIDOOLD;
        //                IsGridValido = ValidarGrid();
        //                break;
        //        }
        //    }
        //}

        private void RecargarFechas()
        {
            if (RecalendarizarDisplayDate.Month==FechaActual.Month)
            {
                SelectedFechasEntregaProveedor = selectedFechasEntregaProveedorCopia.ToList();
            }
            else
            {
                SelectedFechasEntregaProveedor = new List<DateTime>();
            }
        }


        #endregion

        #region Accion Menu
        private async void AccionMenuSwitch(object parametro)
        {
            if(parametro!=null)
            {
                switch(parametro.ToString())
                {
                    case "salvar":
                        StaticSourcesViewModel.ShowProgressLoading("Registrando la entrada al almacén", "Espere por favor");
                        var _error = false;
                        try
                        {
                            var recepcion_productos_controlador = new List<EXT_RECEPCION_PRODUCTOS_B>();
                            foreach (var item in Productos)
                            {
                                var recepcion_producto_detalle_controlador = new List<EXT_RECEPCION_PRODUCTO_DETALLE_B>();
                                if (item.RECEPCION_PRODUCTO_DETALLE != null && item.RECEPCION_PRODUCTO_DETALLE.Count > 0)
                                {
                                    foreach (var item_recepcion in item.RECEPCION_PRODUCTO_DETALLE)
                                        recepcion_producto_detalle_controlador.Add(new EXT_RECEPCION_PRODUCTO_DETALLE_B
                                        {
                                            FECHA_CADUCIDAD = item_recepcion.FECHA_CADUCIDAD,
                                            ID_PRODUCTO = item_recepcion.ID_PRODUCTO,
                                            LOTE = item_recepcion.LOTE,
                                            RECIBIDO = item_recepcion.RECIBIDO
                                        });
                                }
                                recepcion_productos_controlador.Add(new EXT_RECEPCION_PRODUCTOS_B
                                {
                                    FechaRecalendarizacion = item.FechaRecalendarizacion,
                                    ID_PRODUCTO = item.ID_PRODUCTO,
                                    INCIDENCIA_OBSERVACIONES = item.INCIDENCIA_OBSERVACIONES,
                                    INCIDENCIA_TIPO = item.INCIDENCIA_TIPO,
                                    ORDENADO = item.ORDENADO,
                                    RECEPCION_PRODUCTO_DETALLE = recepcion_producto_detalle_controlador,
                                    RECIBIDO = item.RECIBIDO,
                                    RESTANTE = item.RESTANTE,
                                });
                            }
                            if (SelectedTabIndex == 0)
                            {
                                var entrada = new MOVIMIENTO
                                {
                                    FECHA = DateTime.Now.Date,
                                    ID_ALMACEN = SelectedAlmacenPrincipal.ID_ALMACEN,
                                    ID_ALMACEN_GRUPO = SelectedAlmacenPrincipal.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO,
                                    ID_ENTRADA_TIPO = 1,
                                    ID_ORDEN_COMPRA = SelectedOrden_Calendarizada.ID_ORDEN_COMPRA,
                                    ID_USUARIO = _usuario.Username,
                                    TIPO="S"
                                };
                                if (facturaDigitalizado != null && facturaDigitalizado.Count() > 0)
                                    entrada.ENTRADA_FACTURA = new ENTRADA_FACTURA
                                    {
                                        FACTURA = facturaDigitalizado,
                                        OBSERV = ObservacionDocumento
                                    };

                                new cMovimiento().Insertar(entrada, recepcion_productos_controlador, _usuario.Username,
                                    SelectedTabindexOrdenes==0?FormaEntradaOrden.EntradaCalendarizada:FormaEntradaOrden.EntradaManual);
                            }
                            else
                            {
                                var incidencia = new INCIDENCIA
                                {
                                    FECHA = DateTime.Now.Date,
                                    ID_ALMACEN = SelectedAlmacenPrincipal.ID_ALMACEN,
                                    ID_ORDEN_COMPRA = SelectedOrden_Calendarizada.ID_ORDEN_COMPRA,
                                    ID_USUARIO = _usuario.Username,
                                };
                                new cIncidencia().Insertar(incidencia, recepcion_productos_controlador, _usuario.Username,
                                    SelectedTabindexOrdenes == 0 ? FormaEntradaOrden.EntradaCalendarizada : FormaEntradaOrden.EntradaManual);
                            }
                            Limpiar();
                            IsRecepcionProductosVisible = false;
                            CargarOrdenesCalendarizadas(DateTime.Now.Date, SelectedAlmacenPrincipal.ID_ALMACEN);
                        }
                        catch(Exception ex)
                        {
                            _error = true;
                        }
                        StaticSourcesViewModel.CloseProgressLoading();
                        if (_error)
                            await _dialogCoordinator.ShowMessageAsync(this,"Mensaje de error", "Hubo un error al registrar la entrada a almacén. Favor de contactar al administador");
                        else
                        {
                            if (SelectedTabIndex == 0)
                                await _dialogCoordinator.ShowMessageAsync(this, "Mensaje de exito", "Se registro la entrada al almacén con exito");
                            else
                                await _dialogCoordinator.ShowMessageAsync(this, "Mensaje de exito", "Se registro el rechazo con exito");
                            SelectedTabIndexOrdenesChanged(); 
                        }
                            
                        break;
                        
                       
                    case "escanear":
                        ObtenerTipoDocumento(1);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        break;
                    case "cancelar":
                        Limpiar();
                        IsRecepcionProductosVisible = false;
                        break;
                }
            }
        }

        private void Limpiar()
        {
            
            SelectedTabIndex = 0;
            DocumentoDigitalizado = null;
            GuardarEscanerHabilitado = false;
            AbrirImagenEscanerHabilitado = false;
            ObservacionDocumento = string.Empty;
            AutoGuardado = true;
            HabilitarAlmacen_Tipo_Cat = true;
            HabilitarAlmacenPrincipal = true;
            HabilitarCentro = true;
            HabilitarMunicipio = true;
        }
        #endregion

        #region Digitalizacion
        private void ObtenerTipoDocumento(short tipo)
        {
            try
            {
                DocumentoDigitalizado = null;
                ObservacionDocumento = string.Empty;
                DatePickCapturaDocumento = DateTime.Now;
                ListTipoDocumento = new ObservableCollection<TipoDocumento>();
                listTipoDocumento.Add(new TipoDocumento {
                    ID_TIPO_DOCUMENTO=1,
                    DESCR="FACTURA",
                    DIGITALIZADO=false
                });
                SelectedTipoDocumento = ListTipoDocumento[0];
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los tipos de documento.", ex);
            }

        }
        private async void Scan(PdfViewer obj)
        {
            
            var _error = false;
           StaticSourcesViewModel.ShowProgressLoading("Escaneando documento", "Espere por favor");
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    await escaner.Scann(obj);
                    DatePickCapturaDocumento = DateTime.Now;
                    DocumentoDigitalizado = escaner.ScannedDocument;
                    escaner.Dispose();
                    GuardarEscanerHabilitado = true;
                    if (AutoGuardado)
                        if (DocumentoDigitalizado != null)
                            GuardarDocumento();
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
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error al escanear el documento. Favor de contactar al administrador");
            }
        }
        private async void AbrirDocumento(PdfViewer obj)
        {
            try
            {
                if (SelectedTipoDocumento == null)
                {
                    escaner.Hide();
                    await _dialogCoordinator.ShowMessageAsync(this,"Digitalizacion", "Elija El Tipo De Documento A Digitalizar");
                    return;
                }

                if (facturaDigitalizado == null)
                    return;
                await Task.Factory.StartNew(() =>
                {
                    var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                    File.WriteAllBytes(fileNamepdf, facturaDigitalizado);
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        obj.LoadFile(fileNamepdf);
                        obj.Visibility = Visibility.Visible;
                    }));
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir documento.", ex);
            }
        }
        private  void GuardarDocumento()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    facturaDigitalizado = new byte[DocumentoDigitalizado.Count()];
                    DocumentoDigitalizado.CopyTo(facturaDigitalizado,0);
                    escaner.Hide();
                    AbrirImagenEscanerHabilitado = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                    LimpiarEscaner();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al digitalizar documento.", ex);
            }
        }

        private void AccionEscanerSwitch(object parametro)
        {
            if (parametro!=null)
            {
                switch(parametro.ToString())
                {
                    case "guardar_documento":
                        GuardarDocumento();
                        break;
                    case "cerrar_escaner":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        LimpiarEscaner();
                        break;
                }
            }
        }

        private void LimpiarEscaner()
        {
            GuardarEscanerHabilitado = false;            
            ListTipoDocumento[0].DIGITALIZADO = true;
            ListTipoDocumento = new ObservableCollection<TipoDocumento>(ListTipoDocumento);

        }
        #endregion
    }
}
