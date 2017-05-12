using MVVMShared.ViewModels;
using SSP.Controlador.Catalogo.Almacenes;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Principales.Almacenes;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GESAL.Clases.Extendidas;
using GESAL.Clases.Enums;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor.ModelosExtendidos;

namespace GESAL.ViewModels
{
    public partial class CalendarizacionViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        #region Metodos

        #region Metodos de Comandos

        private async void OnLoad(object sender)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Espere un momento...");
            try
            {
                await Task.Factory.StartNew(() => CargarMunicipios());
                await Task.Factory.StartNew(() => CargarCentros(selectedMunicipio.ID_MUNICIPIO));
                await Task.Factory.StartNew(() => CargarAlmacenes(selectedCentro.ID_CENTRO));
                await Task.Factory.StartNew(() => {
                    if (SelectedAlmacen != null)
                        SeleccionarFechasAgenda(SelectedAlmacen.ID_ALMACEN, DateTime.Now.Month, DateTime.Now.Year);
                });
                setBuscarCalendarizacion();
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

        private void CargarCatalogos()
        {
            CargarMunicipios();
            CargarCentros(selectedMunicipio.ID_MUNICIPIO);
            CargarAlmacenes(selectedCentro.ID_CENTRO);
        }

        private async void AbrirAgenda(Object parametro)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                FechaAgenda = Convert.ToDateTime(parametro);
                FechaAgendaOriginal = Convert.ToDateTime(parametro);
                CondicionesInicialesAgenda();
                CargaAgenda(SelectedAlmacen.ID_ALMACEN, FechaAgenda.Value);
                StaticSourcesViewModel.CloseProgressLoading();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGENDA);
                
                setAgendarProductos();
                
            }
            catch (Exception ex)
            {
                _error = true;
            }

            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar la agenda. Favor de contactar al administrador");
            }
        }

        private async void AccionAgenda(Object parametro)
        {
            switch (parametro.ToString())
            {
                case "salvar":
                    SalvarAgenda();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGENDA);
                    if(RealizarAccion==AccionSalvar.Actualizar)
                        SeleccionarFechasAgenda(SelectedAlmacen.ID_ALMACEN,SelectedMes,SelectedAnio);
                    setBuscarCalendarizacion();
                    break;
                case "buscar_orden":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGENDA);
                    BuscarOrdenes();
                    break;
                case "cancelar":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGENDA);
                    setBuscarCalendarizacion();
                    break;
                case "eliminar":
                    if (SelectedOrden_Compra_Detalle.IsEditable)
                        EliminarSelectedOrden_Compra_Detalle(SelectedOrden_Compra_Detalle);
                    break;
                case "recalendarizar_individual":
                    if (SelectedOrden_Compra_Detalle.IsEditable)
                        Recalendarizacion(selectedOrden_Compra_Detalle);
                    break;
                case "borrar":
                    await BorrarAgenda();                    
                    SeleccionarFechasAgenda(SelectedAlmacen.ID_ALMACEN,SelectedMes,SelectedAnio);
                    setBuscarCalendarizacion();
                    break;
                case "recalendarizar":
                    IsFechaAgendaEnabled = !IsFechaAgendaEnabled;
                    break;
            }
        }
        #endregion

        #region Metodos de Agenda
        private void CondicionesInicialesAgenda()
        {
            EliminarHabilitado = false;
            IsProductosAgendaVisible = false;
            SalvarHabilitado = false;
            RecalendarizarHabilitado = false;
            IsFechaAgendaEnabled = false;
            IsProgramadosOK = false;
            RealizarAccion = AccionSalvar.Salvar;
            LimpiarCamposAgenda();
        }

        private void LimpiarCamposAgenda()
        {
            BuscarOrdenParametro = string.Empty;
            SelectedOrden_Compra_Detalles = new ObservableCollection<EXT_Orden_Compra_Detalle_Transito>();
        }

        private async void BuscarOrdenes()
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Ordenes_Compra = new ObservableCollection<ORDEN_COMPRA>(new cOrden_Compra().SeleccionarPorAlmacenparaCalendarizar(SelectedAlmacen.ID_ALMACEN));
                    if (!string.IsNullOrWhiteSpace(BuscarOrdenParametro))
                        Ordenes_Compra = new ObservableCollection<ORDEN_COMPRA>(Ordenes_Compra.Where(w => w.NUM_ORDEN.Value.ToString().Contains(BuscarOrdenParametro)));
                });
                StaticSourcesViewModel.CloseProgressLoading();
                SelectedOrden_CompraPopUp = null;
                Ordenes_Compra_Detalle = null; 
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA_ORDENES_COMPRA_TRANSITO_DETALLE);
            }
            catch (Exception ex)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar la agenda. Favor de contactar al administrador");
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

        private void CargarAlmacenes(short id_centro)
        {
            try
            {
                Almacenes = new ObservableCollection<ALMACEN>(new cAlmacen().Seleccionar(string.Empty, id_centro,_usuario.Almacen_Grupo,"S"));
                selectedAlmacen = Almacenes.FirstOrDefault();
                RaisePropertyChanged("SelectedAlmacen");
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al cargar los almacenes. Favor de contactar al administrador");
            }
        }

        private async void MunicipioCambio(int id_municipio)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                
                CargarCentros(id_municipio);
                CargarAlmacenes(selectedCentro.ID_CENTRO);
                if (SelectedAlmacen != null)
                    SeleccionarFechasAgenda(SelectedAlmacen.ID_ALMACEN, SelectedMes, SelectedAnio);
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

                CargarAlmacenes(id_centro);
                if (SelectedAlmacen != null)
                    SeleccionarFechasAgenda(SelectedAlmacen.ID_ALMACEN, SelectedMes, SelectedAnio);
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

        private void VerificarProgramados(object parametro)
        {
            IsProgramadosOK=!SelectedOrden_Compra_Detalles.Any(a=>a.IS_PROGRAMADOVALID==false);
            if (SelectedOrden_Compra_Detalles.Any(a => a.IsEditable))
                SalvarHabilitado = IsProgramadosOK;
            else
                SalvarHabilitado = false;
        }

        private void CargaAgenda(int id_almacen, DateTime dia)
        {
            try
            {
                SelectedOrden_Compra_Detalles = new ObservableCollection<EXT_Orden_Compra_Detalle_Transito>(new cCalendarizar_Entrega_Producto()
                    .Seleccionar(id_almacen, dia)
                    .Where(w => w.ESTATUS != "CA" && w.ESTATUS != "RL")
                    .Select(s => new EXT_Orden_Compra_Detalle_Transito
                    {
                        ID_CALENDARIZACION_ENTREGA = s.ID_CALENDARIZACION_ENTREGA,
                        PRODUCTO_NOMBRE = s.PRODUCTO.NOMBRE,
                        PROGRAMADO = s.CANTIDAD,
                        PROGRAMADO_ORIGINAL = s.CANTIDAD,
                        ID_PROVEEDOR = s.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.PROVEEDOR.ID_PROV,
                        PROVEEDOR_NOMBRE = s.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.PROVEEDOR.NOMBRE,
                        ID_PRODUCTO = s.ID_PRODUCTO,
                        IS_PROGRAMADOVALID = true,
                        IS_SELECTED = true,
                        NUM_ORDEN = s.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.NUM_ORDEN.Value,
                        ID_ALMACEN = s.CALENDARIZAR_ENTREGA.ID_ALMACEN,
                        ID_ORDEN_COMPRA = s.CALENDARIZAR_ENTREGA.ID_ORDEN_COMPRA.Value,
                        IsEditable=s.ESTATUS=="PR"?true:false,
                        CANTIDAD_ENTREGADA_ENTRADA = (!s.CALENDARIZAR_ENTREGA.MOVIMIENTO.MOVIMIENTO_PRODUCTO.Any(f => f.ID_PRODUCTO == s.ID_PRODUCTO)) ? 0 : (s.CALENDARIZAR_ENTREGA.MOVIMIENTO.MOVIMIENTO_PRODUCTO.Where(w => w.ID_PRODUCTO == s.ID_PRODUCTO && w.ID_MOVIMIENTO == s.CALENDARIZAR_ENTREGA.MOVIMIENTO.ID_MOVIMIENTO).Sum(su => su.CANTIDAD.Value)),
                        PRECIO_COMPRA = s.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == s.ID_PRODUCTO && f.ID_ALMACEN == s.CALENDARIZAR_ENTREGA.ID_ALMACEN).PRECIO_COMPRA,
                        CANTIDAD_ENTREGADA = s.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == s.ID_PRODUCTO && f.ID_ALMACEN == s.CALENDARIZAR_ENTREGA.ID_ALMACEN).CANTIDAD_ENTREGADA,
                        CANTIDAD_ORDEN = s.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == s.ID_PRODUCTO && f.ID_ALMACEN == s.CALENDARIZAR_ENTREGA.ID_ALMACEN).CANTIDAD_ORDEN,
                        CANTIDAD_TRANSITO = s.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == s.ID_PRODUCTO && f.ID_ALMACEN == s.CALENDARIZAR_ENTREGA.ID_ALMACEN).CANTIDAD_TRANSITO,
                        DIFERENCIA = s.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == s.ID_PRODUCTO && f.ID_ALMACEN == s.CALENDARIZAR_ENTREGA.ID_ALMACEN).DIFERENCIA,
                        ID_ORDEN_COMPRA_DET = s.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == s.ID_PRODUCTO && f.ID_ALMACEN == s.CALENDARIZAR_ENTREGA.ID_ALMACEN).ID_ORDEN_COMPRA_DET
                    }).ToList());
            }
            catch(Exception ex)
            {
                throw ex;
            }
            if (SelectedOrden_Compra_Detalles != null && SelectedOrden_Compra_Detalles.Count > 0)
            {
                if (dia < DateTime.Now.Date)
                {
                    SalvarHabilitado = false;
                    EliminarHabilitado = false;
                }
                else
                    if (SelectedOrden_Compra_Detalles.Any(a => a.IsEditable))
                        EliminarHabilitado = true;
                    else
                        EliminarHabilitado = false;
                if (FechaAgenda >= DateTime.Now.Date && SelectedOrden_Compra_Detalles.Any(a => a.IsEditable))
                    RecalendarizarHabilitado = true;
                IsProductosAgendaVisible = true;
                VerificarProgramados(null);
                RealizarAccion = AccionSalvar.Actualizar;
            }
        }

        private async void EliminarSelectedOrden_Compra_Detalle(EXT_Orden_Compra_Detalle_Transito item)
        {
            if (item.ID_CALENDARIZACION_ENTREGA != 0)
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGENDA);
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Si",
                    NegativeButtonText = "No"
                };
                if (await (_dialogCoordinator.ShowMessageAsync(this, "Mensaje de confirmación", "¿La cancelación es atribuible al proveedor? Si es asi se levantara una incidencia.", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative, mySettings)) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                {
                    IsCapturarIncidenciaVisible = true;
                    setRulesIncidencia();
                    IsRecalendarizarVisible = false;
                    item.FechaRecalendarizacion = null;
                    EncabezadoIncidenciaProducto = "Cancelación de " + item.PRODUCTO_NOMBRE + " del proveedor " + item.PROVEEDOR_NOMBRE;
                    await Cargar_Incidente_Tipos();
                    
                    Observacion_Incidencia = string.Empty;
                    FechaRecalidarizarProductoRechazado = null;

                    tipoAccionPopUp = TipoAccionesPopUp.IncidenciaCancelar;
                    if (item.INCIDENCIA_TIPO != null)
                        SelectedIncidencia_Tipo = Incidencia_Tipos.First(w => w.ID_TIPO_INCIDENCIA == item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA && w.ID_ALMACEN_GRUPO == item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO);
                    else
                        SelectedIncidencia_Tipo = Incidencia_Tipos.First(w => w.ID_ALMACEN_GRUPO == SelectedAlmacen.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO && w.ID_TIPO_INCIDENCIA == -1);
                    if (!string.IsNullOrWhiteSpace(item.INCIDENCIA_OBSERVACIONES))
                        Observacion_Incidencia = item.INCIDENCIA_OBSERVACIONES;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RECHAZO_PRODUCTO);
                }
                else
                {
                    SelectedOrden_Compra_Detalle.Estatus = "CA";
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGENDA);
                }
            }
        }
        #endregion

        #region Metodos de Orden de Compra
        private void AccionOrdenCompra(object parametro)
        {
            switch(parametro.ToString())
            {
                case "seleccionado":
                    SeleccionarOrden_Compra_Detalles();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_ORDENES_COMPRA_TRANSITO_DETALLE);
                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.AGENDA);
                    break;
                    
                case "cancelado":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_ORDENES_COMPRA_TRANSITO_DETALLE);
                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.AGENDA);
                    break;
            }
        }

        private void CargarDetalleOrdenCompra(ORDEN_COMPRA orden_compra)
        {
            Ordenes_Compra_Detalle = new ObservableCollection<EXT_Orden_Compra_Detalle_Transito>(orden_compra.ORDEN_COMPRA_DETALLE.Where(w => w.ID_ALMACEN == SelectedAlmacen.ID_ALMACEN)
                .Where(w=>!SelectedOrden_Compra_Detalles.Any(a=>a.ID_PRODUCTO==w.ID_PRODUCTO && a.ID_ALMACEN==w.ID_ALMACEN))
                .Select(s => new EXT_Orden_Compra_Detalle_Transito {
                    CANTIDAD_ENTREGADA=s.CANTIDAD_ENTREGADA,
                    CANTIDAD_ORDEN=s.CANTIDAD_ORDEN,
                    CANTIDAD_TRANSITO=s.CANTIDAD_TRANSITO,
                    DIFERENCIA=s.DIFERENCIA,
                    ID_ALMACEN=s.ID_ALMACEN,
                    ID_ORDEN_COMPRA=s.ID_ORDEN_COMPRA,
                    ID_ORDEN_COMPRA_DET=s.ID_ORDEN_COMPRA_DET,
                    ID_PRODUCTO=s.ID_PRODUCTO,
                    IS_SELECTED=false,
                    PRECIO_COMPRA=s.PRECIO_COMPRA,
                    PRODUCTO_NOMBRE=s.PRODUCTO.NOMBRE,
                    PROVEEDOR_NOMBRE=s.ORDEN_COMPRA.PROVEEDOR.NOMBRE,
                    NUM_ORDEN=s.ORDEN_COMPRA.NUM_ORDEN.Value

                }).ToList());
        }

        private void RaiseChangeCheckedOrdenes_Compra_Detalle(object obj)
        {
            RaisePropertyChanged("Ordenes_Compra_Detalle");
        }

        private void SeleccionarOrden_Compra_Detalles()
        {
             foreach (var item in Ordenes_Compra_Detalle)
             {
                 if (item.IS_SELECTED)
                     SelectedOrden_Compra_Detalles.Add(item);
             }
            if (SelectedOrden_Compra_Detalles.Count>0)
            {
                IsProductosAgendaVisible = true;
            }
        }

        private void SalvarAgenda()
        {
            try
            {
                var agenda = new List<EXT_CALENDARIZAR_ENTREGA>();
                var datos_globales = SelectedOrden_Compra_Detalles.Select(s => new
                {
                    ID_ALMACEN = s.ID_ALMACEN,
                    ID_ORDEN_COMPRA = s.ID_ORDEN_COMPRA
                }).Distinct();
                foreach (var item in datos_globales)
                {
                    var detalle_agenda = new List<EXT_CALENDARIZAR_ENTREGA_PRODUCTO>();
                    foreach (var item_detalle in SelectedOrden_Compra_Detalles.Where(w => w.ID_ORDEN_COMPRA == item.ID_ORDEN_COMPRA &&
                        w.ID_ALMACEN == item.ID_ALMACEN))
                        detalle_agenda.Add(new EXT_CALENDARIZAR_ENTREGA_PRODUCTO
                        {
                            CANTIDAD = item_detalle.PROGRAMADO.Value,
                            ID_PRODUCTO = item_detalle.ID_PRODUCTO,
                            IsEditable=item_detalle.IsEditable,
                            ESTATUS=item_detalle.Estatus,
                            FechaRecalendarizacion=item_detalle.FechaRecalendarizacion,
                            INCIDENCIA_TIPO=(item_detalle.INCIDENCIA_TIPO!=null&&item_detalle.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA!=-1)?item_detalle.INCIDENCIA_TIPO:null,
                            INCIDENCIA_OBSERVACIONES=item_detalle.INCIDENCIA_OBSERVACIONES
                        });
                    agenda.Add(new EXT_CALENDARIZAR_ENTREGA
                    {
                        FEC_PACTADA = FechaAgenda.Value,
                        FECHA = DateTime.Now,
                        ID_ALMACEN = item.ID_ALMACEN,
                        ID_ENTRADA = null,
                        ID_ORDEN_COMPRA = item.ID_ORDEN_COMPRA,
                        ID_USUARIO = _usuario.Username,
                        CALENDARIZAR_ENTREGA_PRODUCTO = detalle_agenda,
                        ESTATUS="PR"
                    });

                }
                if (RealizarAccion == AccionSalvar.Salvar)
                {
                    new cCalendarizar_Entrega().Insertar(agenda);
                    AgregarFecha(FechaAgenda.Value);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificación", "Se inserto la calendarización con exito");
                }
                else if (RealizarAccion == AccionSalvar.Actualizar)
                {
                    if (agenda.Count > 0)
                        new cCalendarizar_Entrega().Actualizar(agenda,FechaAgendaOriginal.Value);
                    else
                        new cCalendarizar_Entrega().EliminarporFecha(agenda);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificación", "Se actualizo la calendarización con exito");
                }
            }catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al salvar la agenda. Favor de contactar al administrador");
            }
            
        }

        private void SeleccionarFechasAgenda(int id_almacen,int mes, int anio)
        {
            try
            {
                DiasAgendados = new ObservableCollection<DateTime>(new cCalendarizar_Entrega().Seleccionar(id_almacen, mes, anio)
                    .Where(w => w.ESTATUS != "CA")
                    .Select(s => s.FEC_PACTADA.Value).Distinct().ToList());
            }
            catch(Exception ex)
            {
                throw new Exception("Hubo un error al cargar los almacenes. Favor de contactar al administrador");
            }
        }


        private void AgregarFecha(DateTime agendar_fecha)
        {

            if (!DiasAgendados.Contains(agendar_fecha))
            {
                DiasAgendados.Add(agendar_fecha);
                var dias_array = new DateTime[DiasAgendados.Count];
                DiasAgendados.CopyTo(dias_array,0);
                DiasAgendados = new ObservableCollection<DateTime>(dias_array.ToList());
            }
                

        }

        private async Task BorrarAgenda()
        {
            var _error = false;
            try
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGENDA);
                if (await _dialogCoordinator.ShowMessageAsync(this, "Confirmación", String.Format("¿Esta seguro de eliminar la agenda del dia {0}?", String.Format("{0:M/d/yyyy}", FechaAgenda)), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    var agenda = new List<EXT_CALENDARIZAR_ENTREGA>();
                    var datos_globales = SelectedOrden_Compra_Detalles.Select(s => new
                    {
                        ID_ALMACEN = s.ID_ALMACEN,
                        ID_ORDEN_COMPRA = s.ID_ORDEN_COMPRA
                    }).Distinct();
                    foreach (var item in datos_globales)
                    {
                        var detalle_agenda = new List<EXT_CALENDARIZAR_ENTREGA_PRODUCTO>();
                        foreach (var item_detalle in SelectedOrden_Compra_Detalles.Where(w => w.ID_ORDEN_COMPRA == item.ID_ORDEN_COMPRA &&
                            w.ID_ALMACEN == item.ID_ALMACEN))
                            detalle_agenda.Add(new EXT_CALENDARIZAR_ENTREGA_PRODUCTO
                            {
                                CANTIDAD = item_detalle.PROGRAMADO.Value,
                                ID_PRODUCTO = item_detalle.ID_PRODUCTO,
                                IsEditable = item_detalle.IsEditable
                            });
                        agenda.Add(new EXT_CALENDARIZAR_ENTREGA
                        {
                            FEC_PACTADA = FechaAgenda.Value,
                            FECHA = DateTime.Now,
                            ID_ALMACEN = item.ID_ALMACEN,
                            ID_ENTRADA = null,
                            ID_ORDEN_COMPRA = item.ID_ORDEN_COMPRA,
                            ID_USUARIO = _usuario.Username,
                            CALENDARIZAR_ENTREGA_PRODUCTO = detalle_agenda,
                            ESTATUS = "PR"
                        });

                    }
                    new cCalendarizar_Entrega().EliminarporFecha(agenda);
                    await _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se elimino la unidad de medida con exito");
                }
                else
                {
                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.AGENDA);
                }

            }
            catch (Exception ex)
            { _error = true; };
            if (_error)
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");

        }

        
        #endregion

        #region Metodos de PopUpRechazo

        private async void Recalendarizacion(EXT_Orden_Compra_Detalle_Transito _orden_compra_detalle_transito)
        {
            try
            {
                if (_orden_compra_detalle_transito.ID_CALENDARIZACION_ENTREGA!=0)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGENDA);
                    var mySettings = new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Si",
                        NegativeButtonText = "No"
                    };
                    if (await (_dialogCoordinator.ShowMessageAsync(this, "Mensaje de confirmación", "¿La recalendarizacion es atribuible al proveedor? Si es asi se levantara una incidencia.", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative,mySettings)) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                    {
                        IsCapturarIncidenciaVisible = true;
                        setRecalendarizacionConIncidencia();
                    }
                    else
                    {
                        IsCapturarIncidenciaVisible = false;
                        setRecalendarizacionSinIncidencia();
                    }
                    tipoAccionPopUp = TipoAccionesPopUp.IncidenciaRecalendarizar;
                    IsRecalendarizarVisible = true;
                    SelectedFechasEntregaProveedor = new cCalendarizar_Entrega_Producto().SeleccionarFechasEntregaProductoProveedorRestantesMes(SelectedAlmacen.ID_ALMACEN,
                            _orden_compra_detalle_transito.ID_PROVEEDOR, _orden_compra_detalle_transito.ID_PRODUCTO, FechaAgenda.Value.Month).Select(s => s.CALENDARIZAR_ENTREGA.FEC_PACTADA.Value).ToList();
                    selectedFechasEntregaProveedorCopia = new DateTime[SelectedFechasEntregaProveedor.Count];
                    SelectedFechasEntregaProveedor.CopyTo(selectedFechasEntregaProveedorCopia);
                    EncabezadoIncidenciaProducto = "Recalendarizacion de " + _orden_compra_detalle_transito.PRODUCTO_NOMBRE + " del proveedor " + _orden_compra_detalle_transito.PROVEEDOR_NOMBRE;
                    await Cargar_Incidente_Tipos()
                    .ContinueWith((prevTask) => {
                        if (_orden_compra_detalle_transito.INCIDENCIA_TIPO != null)
                            SelectedIncidencia_Tipo = Incidencia_Tipos.First(w => w.ID_TIPO_INCIDENCIA == _orden_compra_detalle_transito.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA && w.ID_ALMACEN_GRUPO == _orden_compra_detalle_transito.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO);
                        else
                            SelectedIncidencia_Tipo = Incidencia_Tipos.First(w =>w.ID_TIPO_INCIDENCIA == -1);
                    });
                    if (!String.IsNullOrWhiteSpace(_orden_compra_detalle_transito.INCIDENCIA_OBSERVACIONES))
                        Observacion_Incidencia = _orden_compra_detalle_transito.INCIDENCIA_OBSERVACIONES;
                    else
                        Observacion_Incidencia = string.Empty;
                    if (_orden_compra_detalle_transito.FechaRecalendarizacion.HasValue)
                        FechaRecalidarizarProductoRechazado = _orden_compra_detalle_transito.FechaRecalendarizacion.Value;
                    else
                        FechaRecalidarizarProductoRechazado = null;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RECHAZO_PRODUCTO);
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void RecargarFechas()
        {
            if (RecalendarizarDisplayDate.Month == FechaAgenda.Value.Month)
            {
                SelectedFechasEntregaProveedor = selectedFechasEntregaProveedorCopia.ToList();
            }
            else
            {
                SelectedFechasEntregaProveedor = new List<DateTime>();
            }
        }

        private async Task Cargar_Incidente_Tipos()
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento...");
            try
            {
                Incidencia_Tipos = new ObservableCollection<INCIDENCIA_TIPO>(new cIncidencia_Tipo().Seleccionar(SelectedAlmacen.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO, "S").ToList());
                var dummy = new INCIDENCIA_TIPO
                {
                    ACTIVO = "S",
                    ID_ALMACEN_GRUPO = _usuario.Almacen_Grupo,
                    DESCR = "SELECCIONA UNA",
                    ID_TIPO_INCIDENCIA = -1
                };
                Incidencia_Tipos.Add(dummy);
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

        private void RecalendarizarProductoSwitch(object parametro)
        {
            var detalle = SelectedOrden_Compra_Detalles.First(f => f.ID_ALMACEN == SelectedOrden_Compra_Detalle.ID_ALMACEN && f.ID_PRODUCTO == SelectedOrden_Compra_Detalle.ID_PRODUCTO
                && f.ID_ORDEN_COMPRA == SelectedOrden_Compra_Detalle.ID_ORDEN_COMPRA);
            switch (parametro.ToString())
            {
                case "ok":

                    detalle.INCIDENCIA_TIPO = SelectedIncidencia_Tipo;
                    detalle.INCIDENCIA_OBSERVACIONES = Observacion_Incidencia;
                    if (tipoAccionPopUp==TipoAccionesPopUp.IncidenciaRecalendarizar)
                    {
                        detalle.FechaRecalendarizacion = FechaRecalidarizarProductoRechazado;
                        detalle.Estatus = "RL";
                    }
                    else
                    {
                        SelectedOrden_Compra_Detalle.Estatus = "CA";
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RECHAZO_PRODUCTO);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGENDA);
                    setAgendarProductos();
                    break;
                case "cancelado":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RECHAZO_PRODUCTO);
                    SelectedIncidencia_Tipo = null;
                    Observacion_Incidencia = string.Empty;
                    FechaRecalidarizarProductoRechazado = null;
                    setAgendarProductos();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGENDA);
                    break;
            }
        }
        #endregion
        #endregion
    }
}
