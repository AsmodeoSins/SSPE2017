using ControlPenales.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using SSP.Servidor;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Justicia;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using ControlPenales.Clases.ControlInternos;
using System.Data.Objects;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
//using LinqKit;
using ControlPenales.ControlInternosEdificio.ViewModel;
using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using System.Transactions;
using System.Data;

namespace ControlPenales
{
    partial class ControlInternosViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        void IPageViewModel.inicializa() { }

        #region [CONSTRUCTOR]
        public ControlInternosViewModel() { }
        #endregion

        #region [METODOS]
        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                BusquedaInterno = ((System.Windows.Controls.TextBox)(obj)).Text;
            }
        }

        private void IniciarTimer()
        {
            FechaActualizacion = Fechas.GetFechaDateServer.AddMinutes(2);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Stop();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            var result = ToolsTimer.GetTimeUntilNextHour(FechaActualizacion.Hour, FechaActualizacion.Minute, FechaActualizacion.Second);
            hora = result.Hours.ToString().Length < 2 ? string.Format("0{0}", result.Hours.ToString()) : result.Hours.ToString();
            minuto = result.Minutes.ToString().Length < 2 ? string.Format("0{0}", result.Minutes.ToString()) : result.Minutes.ToString();
            segundo = result.Seconds.ToString().Length < 2 ? string.Format("0{0}", result.Seconds.ToString()) : result.Seconds.ToString();
            Actualizacion = string.Format("Actualización en: {0}:{1}:{2}", hora, minuto, segundo);
            SetUpTimer(result);
        }

        private async void SetUpTimer(TimeSpan alertTime)
        {
            DateTime current = Fechas.GetFechaDateServer;

            if ((alertTime.Hours == 0 && alertTime.Minutes == 0 && alertTime.Seconds == 0) || alertTime.Hours == 23)
            {
                timer.Stop();
                Actualizacion = string.Empty;
                ListaInternosRequeridos = null;
                ListaInternosRequeridos = await TaskInterno();
                ActualizarInternos(ListaInternosRequeridos);
                GetAusentes();

                if (LstInternosSeleccionados != null)
                    if (LstInternosSeleccionados.Count > 0)
                    {
                        foreach (var obj in ListaInternosRequeridos)
                        {
                            if (LstInternosSeleccionados.Count(w => w.Id_Grupo == obj.Id_Grupo && w.IdArea == obj.IdArea && w.IdImputado == obj.IdImputado && w.IdIngreso == obj.IdIngreso && w.Anio == obj.Anio) > 0)
                                obj.SELECCIONAR = true;
                        }
                        ListaInternosRequeridos = new List<InternosRequeridos>(ListaInternosRequeridos);
                    }
                if (LstInternosSeleccionadosAusentes != null)
                    if (LstInternosSeleccionadosAusentes.Count > 0)
                    {
                        foreach (var obj in ListaInternosSeleccionados)
                        {
                            if (LstInternosSeleccionadosAusentes.Count(w => w.Actividad == obj.Actividad && w.Area == obj.Area && w.IdImputado == obj.IdImputado && w.IdIngreso == obj.IdIngreso && w.IdAnio == obj.IdAnio) > 0)
                                obj.SELECTELIMINAR = true;
                        }
                        ListaInternosSeleccionados = new ObservableCollection<InternosAusentes>(ListaInternosSeleccionados);
                    }
                timer.Start();
            }
        }

        private async Task<List<InternosRequeridos>> TaskInterno()
        {
            return await StaticSourcesViewModel.OperacionesAsync<List<InternosRequeridos>>("Actualizando datos", () =>
            {
                try
                {
                    return new InternosRequeridos().ListaInternos(FechaInicio, FechaFin, GlobalVar.gCentro, SelectedEdificio.ID_EDIFICIO, SelectedSector.ID_SECTOR);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                    ListaInternosRequeridos = null;

                    return ListaInternosRequeridos;
                }
                finally
                {
                    FechaActualizacion = Fechas.GetFechaDateServer.AddMinutes(2);
                }
            });
        }

        //CHECKBOX INTERNOS REQUERIDOS
        private void InternoSeleccionado(object SelectedItem)
        {
            try
            {
                if (SelectedItem is InternosRequeridos)
                {
                    var obj = (InternosRequeridos)SelectedItem;
                    if (obj.SELECCIONAR)
                    {
                        if (LstInternosSeleccionados == null)
                            LstInternosSeleccionados = new List<InternosRequeridos>();
                        LstInternosSeleccionados.Add(obj);
                    }
                    else
                    {
                        if (LstInternosSeleccionados == null)
                            LstInternosSeleccionados = new List<InternosRequeridos>();
                        var o = LstInternosSeleccionados.Where(w => w.Id_Grupo == obj.Id_Grupo && w.IdArea == obj.IdArea && w.IdImputado == obj.IdImputado && w.IdIngreso == obj.IdIngreso && w.Anio == obj.Anio).FirstOrDefault() ?? null;
                        if (o != null)
                            LstInternosSeleccionados.Remove(o);
                    }
                    InternosSeleccionados = string.Format("Internos Seleccionados: {0}", LstInternosSeleccionados.Count);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
            }
        }

        //CHECKBOX INTERNOS AUSENTES
        private void EliminarInternoAusente(object SelectedItem)
        {
            try
            {
                if (SelectedItem is InternosAusentes)
                {
                    var obj = (InternosAusentes)SelectedItem;
                    if (obj.SELECTELIMINAR)
                    {
                        if (LstInternosSeleccionadosAusentes == null)
                            LstInternosSeleccionadosAusentes = new List<InternosAusentes>();
                        LstInternosSeleccionadosAusentes.Add(obj);
                    }
                    else
                    {
                        var o = LstInternosSeleccionadosAusentes.Where(w => w.Actividad == obj.Actividad && w.Area == obj.Area && w.IdImputado == obj.IdImputado && w.IdIngreso == obj.IdIngreso && w.IdAnio == obj.IdAnio).FirstOrDefault();
                        if (o != null)
                            LstInternosSeleccionadosAusentes.Remove(o);
                    }
                    InternosSeleccionados = string.Format("Internos Seleccionados: {0}", LstInternosSeleccionadosAusentes.Count);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de internos.", ex);
                return new List<IMPUTADO>();
            }
        }

        private async void Buscar()
        {
            try
            {
                if (SelectedEdificio != null && SelectedSector != null)
                {
                    if (FechaInicio != null && FechaFin != null)
                    {
                        if (FechaInicio.Value.Date > FechaFin.Value.Date)
                        {
                            //MENSAJE DE QUE LA FECHA INICIO NO PUEDE SER MAYOR A LA FECHA FIN
                            StaticSourcesViewModel.Mensaje("", "La fecha inicio no puede ser mayor a la fecha fin", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                        }
                        else if (FechaInicio.Value.Day < Fechas.GetFechaDateServer.Day || FechaInicio.Value.Month < Fechas.GetFechaDateServer.Month || FechaInicio.Value.Year < Fechas.GetFechaDateServer.Year)
                        {
                            //MENSAJE QUE EL DIA NO PUEDE SER MENOR AL DIA ACTUAL, FALTA EL MES Y ANIO
                            StaticSourcesViewModel.Mensaje("", "La fecha inicio no puede ser menor a la fecha actual", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                        }
                        else
                        {
                            if ((SelectedEdificio.ID_EDIFICIO > 0 && SelectedSector.ID_SECTOR > 0) || (SelectedSector.ID_SECTOR == -1 && SelectedEdificio.ID_EDIFICIO == -1))
                            {
                                timer.Stop();
                                ListaInternosRequeridos = null;
                                LstInternosSeleccionados = new List<InternosRequeridos>();
                                ListaInternosRequeridos = await TaskInterno();
                                ActualizarInternos(ListaInternosRequeridos);
                                TotalInternos = string.Format("Total de Internos: {0}", ListaInternosRequeridos.Count);
                                InternosSeleccionados = string.Format("Internos Seleccionados: {0}", LstInternosSeleccionados.Count);
                                GetAusentes();
                                timer.Start();
                            }
                            else
                            {
                                setValidationRules();
                            }
                        }
                    }
                }
                else
                {
                    setValidationRules();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó", "Ocurrió un problema al realizar búsqueda", ex);
            }
        }

        private async void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "limpiar_menu":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea limpiar la pantalla?") != 1)
                            return;
                    }
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ControlInternosView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlInternosViewModel();
                    break;
                case "salir_menu":
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    PrincipalViewModel.SalirMenu();
                    break;
                case "registroSalida":
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        OnBuscarPorHuella("");
                        TaskEx.Delay(5000);
                    }));
                    break;
                case "cerrarActividad":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.LISTA_ACTIVIDADES_INTERNO);
                    break;
                case "buscar":
                    ModoAlternativoHabilitado = true;
                    AsistenciaHabilitado = true;
                    AutorizarBtnEnabled = true;
                    this.Buscar();
                    break;
                case "autorizar":
                    timer.Stop();
                    var x = Custodio;
                    var traslado_detalle = new cTrasladoDetalle().ObtenerTodosTraslado();
                    var excarcelacion_interno = new cExcarcelacion().ObtenerTodos();

                    if (TipoTexto == "Entrada")
                    {
                        if (TipoRegistroChecked)
                        {
                            var listaAusentes = await SeleccionarInternosAusentes(ListaInternosSeleccionados);
                            InternosSeleccionados = string.Format("Internos Seleccionados: {0}", listaAusentes.Count);

                            if (listaAusentes.Count == 0)
                            {
                                StaticSourcesViewModel.Mensaje("NOTA", "No se agregaron internos, para agregar asegúrese que está marcada la casilla de verificación. ", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 7);
                            }
                            else if (listaAusentes.Count != 0)
                            {
                                EnrolarInternosEnabled = false;
                                NombreBuscar = string.Empty;
                                ApellidoMaternoBuscar = string.Empty;
                                ApellidoPaternoBuscar = string.Empty;
                                FolioBuscar = null;
                                ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
                            }
                            foreach (var item in listaAusentes)
                            {
                                if (item.SELECTELIMINAR)
                                {
                                    ListaAusenteEntrada.Add(item);
                                }
                            }
                        }
                        else
                        {
                            StaticSourcesViewModel.Mensaje("ERROR", "No se completó la operación, seleccione el tipo de registro (Entrada) para completar la operación. ", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 7);
                            break;
                        }
                        if (x != null)
                        {
                            GuardarAusentes(ListaAusenteEntrada, x.ID_PERSONA);
                        }
                        else
                        {
                            GuardarAusentes(ListaAusenteEntrada);
                        }
                    }
                    if (TipoTexto == "Salida")
                    {
                        if (TipoRegistroChecked)
                        {
                            var listaInternos = await SeleccionarInternos(ListaInternosRequeridos);

                            if (listaInternos.Count == 0)
                            {
                                StaticSourcesViewModel.Mensaje("NOTA", "No se agregaron internos, para agregar asegúrese que está marcada la casilla de verificación. ", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 7);
                            }
                            else if (listaInternos.Count != 0)
                            {
                                EnrolarInternosEnabled = false;
                                NombreBuscar = string.Empty;
                                ApellidoMaternoBuscar = string.Empty;
                                ApellidoPaternoBuscar = string.Empty;
                                FolioBuscar = null;
                                ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
                            }
                            foreach (var item in listaInternos)
                            {
                                if (item.TrasladoInterno)
                                {
                                    if (item.Fecha.Value.Day >= Fechas.GetFechaDateServer.Day && item.Fecha.Value.Month >= Fechas.GetFechaDateServer.Month && item.Fecha.Value.Year >= Fechas.GetFechaDateServer.Year)
                                    {
                                        internosSelect.Add(item);
                                    }
                                }
                                else if (item.Fecha.Value.Day == Fechas.GetFechaDateServer.Day && item.Fecha.Value.Month == Fechas.GetFechaDateServer.Month && item.Fecha.Value.Year == Fechas.GetFechaDateServer.Year)
                                {
                                    internosSelect.Add(item);
                                }
                            }
                        }
                        else
                        {
                            StaticSourcesViewModel.Mensaje("ERROR", "No se completó la operación, seleccione el tipo de registro (Salida) para completar la operación. ", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 7);
                            break;
                        }
                    }
                    ListaInternosRequeridos = new List<InternosRequeridos>(EliminarInternoSeleccionado(internosSelect));
                    TotalInternos = string.Format("Total de Internos: {0}", ListaInternosRequeridos.Count);

                    if (x != null)
                    {
                        this.GuardarInternoSeleccionado(internosSelect, x.ID_PERSONA);
                    }
                    else
                    {
                        this.GuardarInternoSeleccionado(internosSelect);
                    }
                    GetAusentes();

                    ListaAusenteEntrada.Clear();
                    internosSelect.Clear();

                    if (LstInternosSeleccionados == null)
                        LstInternosSeleccionados = new List<InternosRequeridos>();

                    LstInternosSeleccionados.Clear();
                    InternosSeleccionados = string.Format("Internos Seleccionados: {0}", 0);
                    if (LstInternosSeleccionadosAusentes == null)
                        LstInternosSeleccionadosAusentes = new List<InternosAusentes>();

                    LstInternosSeleccionadosAusentes.Clear();
                    InternosSeleccionados = string.Format("Internos Seleccionados: {0}", 0);
                    timer.Start();
                    break;
                case "asignarCustodio":
                    this.OnBuscarPorHuellaCustodio();
                    break;
                case "limpiarEnrolamientos":
                    EnrolarInternosEnabled = false;
                    NombreBuscar = string.Empty;
                    ApellidoMaternoBuscar = string.Empty;
                    ApellidoPaternoBuscar = string.Empty;
                    FolioBuscar = null;
                    AsistenciaHabilitado = false;
                    AutorizarBtnEnabled = false;
                    ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
                    break;
                case "informacionInterno":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                //LOGICA PARA MOSTRAR LAS SIGUIENTES ACTIVIDADES DEL INTERNO EN EL DIA
                                ListaActividadesInternos = new InternosAusentes().ListaActividades(SelectInternosSeleccionados.IdImputado, FechaInicio, FechaFin, GlobalVar.gCentro);
                                ListaActividadesInternos = new ObservableCollection<InternosAusentes>(ListaActividadesInternos.OrderBy(o => o.HoraInicio));
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                            }
                        });
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.LISTA_ACTIVIDADES_INTERNO);
                        TaskEx.Delay(5000);
                    }));
                    break;
            }
        }

        private void GuardarAusentes(List<InternosAusentes> ausentes, int? id_custodio = null)
        {
            try
            {
                foreach (var item in ausentes)
                {
                    item.Entrada = true;
                    if (!new cIngresoUbicacion().Insertar(new INGRESO_UBICACION
                    {
                        ID_CENTRO = item.IdCentro,
                        ID_ANIO = item.IdAnio,
                        ID_IMPUTADO = item.IdImputado,
                        ID_INGRESO = item.IdIngreso,
                        ID_AREA = 0,
                        MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                        ACTIVIDAD = "ESTANCIA",
                        ESTATUS = 0,
                        ID_CUSTODIO = id_custodio,
                        INTERNO_UBICACION = "S"
                    }, false, false, null, true))
                        GetAusentes();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void GetAusentes()
        {
            try
            {
                ListaInternosSeleccionados = new InternosAusentes().ListInternosAusentes(GlobalVar.gCentro);
                if (!SelectRequerido)
                    TotalInternos = string.Format("Total de Internos: {0}", ListaInternosSeleccionados.Count);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener internos ausentes", ex);
            }
        }

        private void GuardarInternoSeleccionado(List<InternosRequeridos> lista, int? id_custodio = null)
        {
            try
            {
                foreach (var item in lista)
                {
                    if (internosSelect != null)
                    {
                        if (ExisteObject(item.IdImputado) && item.Ubicacion != "ESTANCIA")
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación", "El interno seleccionado tiene salida registrada");
                        }
                        else
                        {
                            if (item.YardaInterno)
                            {
                                ingreso_ubicacion.Insertar(new INGRESO_UBICACION
                                {
                                    ID_CENTRO = item.Centro,
                                    ID_ANIO = item.Anio,
                                    ID_IMPUTADO = item.IdImputado,
                                    ID_INGRESO = item.IdIngreso,
                                    ID_AREA = item.IdArea,
                                    MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                    ACTIVIDAD = item.Actividad,
                                    ESTATUS = 2,
                                    ID_CUSTODIO = id_custodio
                                }, item.ExcarcelacionInterno, item.TrasladoInterno, item.IdAduana);
                            }
                            else
                            {
                                ingreso_ubicacion.Insertar(new INGRESO_UBICACION
                                {
                                    ID_CENTRO = item.Centro,
                                    ID_ANIO = item.Anio,
                                    ID_IMPUTADO = item.IdImputado,
                                    ID_INGRESO = item.IdIngreso,
                                    ID_AREA = item.IdArea,
                                    MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                    ACTIVIDAD = item.Actividad,
                                    ESTATUS = 1,
                                    ID_CUSTODIO = id_custodio
                                }, item.ExcarcelacionInterno, item.TrasladoInterno, item.IdAduana);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar datos", ex);
            }
        }

        private bool ExisteInternoAusente(int ID_IMPUTADO)
        {
            try
            {
                return new cIngresoUbicacion().ObtenerTodos().Where(w => w.ID_IMPUTADO == ID_IMPUTADO && (w.ESTATUS == 0)).Count() > 0;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos", ex);
                return false;
            }
        }

        //METODO QUE COMPARA ID_IMPUTADO
        private bool ExisteObject(int ID_IMPUTADO)
        {
            try
            {
                return new cIngresoUbicacion().GetData(g => g.ID_IMPUTADO == ID_IMPUTADO).Count() > 0;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos", ex);
                return false;
            }
        }

        //METODO QUE ACTUALIZA LA LISTA DE INTERNOS REQUERIDOS (AL ELIMINAR RENGLONES)
        private List<InternosRequeridos> ActualizarInternos(List<InternosRequeridos> listaInternos)
        {
            var listaeliminarBuscar = new List<InternosRequeridos>();
            if (ListaAusenteEntrada != null)
            if (ListaAusenteEntrada.Count > 0)
            {
                foreach (var list in ListaAusenteEntrada)
                {
                    foreach (var row in listaInternos)
                    {
                        if (list.Expediente == row.Expediente && list.Fecha == row.Fecha)
                        {
                            listaeliminarBuscar.Add(row);
                        }
                    }
                }
                if (listaeliminarBuscar.Count == 0)
                {
                    ListaAusenteEntrada.Clear();
                }
            }
            if (listaInternos != null)
            foreach (var item in listaInternos)
            {
                if (item.TrasladoInterno)
                {
                    item.BooleanToRowTraslado = true;
                }
                if (item.ExcarcelacionInterno)
                {
                    item.BooleanToRowExcarcelacion = true;
                }
                if (item.CitaMedica)
                {
                    item.BooleanToRowCitaMedica = true;
                }
            }
            if (ListaInternosSeleccionados != null)
            foreach (var item in ListaInternosSeleccionados)
            {
                foreach (var row in listaInternos)
                {
                    if (item.Llave == row.LlaveInterno && item.Actividad == row.Actividad)
                    {
                        listaeliminarBuscar.Add(row);
                    }
                    if (item.Expediente == row.Expediente)
                    {
                        row.Ubicacion = item.Ubicacion;
                    }
                }
            }
            if (listaeliminarBuscar != null)
            foreach (var item in listaeliminarBuscar)
            {
                listaInternos.Remove(item);
            }
            return listaInternos;
        }

        //SELECCIONA INTERNOS AUSENTES
        private async Task<List<InternosAusentes>> SeleccionarInternosAusentes(ObservableCollection<InternosAusentes> lista)
        {
            return await StaticSourcesViewModel.OperacionesAsync<List<InternosAusentes>>("Procesando entrada...", () =>
            {
                try
                {
                    Dispatcher.CurrentDispatcher.Invoke(new Action(() => { Thread.Sleep(4000); }));
                    var listaSelect = new List<InternosAusentes>();
                    foreach (var row in lista)
                    {
                        if (row.SELECTELIMINAR)
                        {
                            listaSelect.Add(row);// lista de internos para custodio 
                        }
                    }
                    return listaSelect;
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                    ListaInternosAusentes = null;

                    return ListaInternosAusentes;
                }
            });
        }

        //SELECCIONA INTERNOS REQUERIDOS
        private async Task<List<InternosRequeridos>> SeleccionarInternos(List<InternosRequeridos> lista)
        {
            return await StaticSourcesViewModel.OperacionesAsync<List<InternosRequeridos>>("Procesando salida...", () =>
            {
                try
                {
                    Dispatcher.CurrentDispatcher.Invoke(new Action(() => { Thread.Sleep(4000); }));
                    var listaSelect = new List<InternosRequeridos>();
                    foreach (var row in lista)
                    {
                        if (row.SELECCIONAR)
                        {
                            listaSelect.Add(row);// lista de internos para custodio 
                        }
                    }
                    return listaSelect;
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                    ListaInternosRequeridos = null;

                    return ListaInternosRequeridos;
                }
            });
        }

        private List<InternosRequeridos> EliminarInternoSeleccionado(List<InternosRequeridos> list)
        {
            try
            {
                var listaEliminar = new List<InternosRequeridos>();
                listaEliminar = ListaInternosRequeridos;

                foreach (var row in list)
                {
                    if (row.SELECCIONAR && !ExisteObject(row.IdImputado))
                    {
                        list.ForEach((e) =>
                        {
                            listaEliminar.Remove(e);
                        });
                    }
                    else
                        if (row.SELECCIONAR && ExisteInternoAusente(row.IdImputado))
                        {
                            list.ForEach((e) =>
                            {
                                listaEliminar.Remove(e);
                            });
                        }
                }
                return listaEliminar;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                return null;
            }
        }

        private async void OnBuscarPorHuellaCustodio(string obj = "")
        {
            try
            {
                AsistenciaBiometricaDeshabilitado = true;
                AsistenciaBiometricaSelect = true;
                AsistenciaNIPSelect = false;
                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
                await TaskEx.Delay(400);
                var nRet = -1;
                var bandera = true;
                var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaJuridico;
                if (requiereGuardarHuellas)
                    try
                    {
                        nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                    }
                    catch
                    {
                        bandera = false;
                    }
                else
                    bandera = false;

                var windowBusqueda = new LeerCustodioEdificio();
                windowBusqueda.DataContext = new LeerCustodioEdificioViewModel(enumTipoPersona.PERSONA_EMPLEADO, nRet == 0, requiereGuardarHuellas, SelectedEdificio.ID_EDIFICIO, SelectedSector.ID_SECTOR);

                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.LISTA_ASISTENCIA_INTERNO_EDIFICIO);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGÚRESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }
                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) =>
                {
                    try
                    {
                        if (e.Key == System.Windows.Input.Key.Escape) windowBusqueda.Close();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar", ex);
                    }
                };
                ((LeerCustodioEdificioViewModel)windowBusqueda.DataContext).ModoHuellaCustodio = true;
                windowBusqueda.Closed += (s, e) =>
                {
                    try
                    {
                        HuellasCapturadas = ((LeerCustodioEdificioViewModel)windowBusqueda.DataContext).HuellasCapturadasCustodio;
                        if (bandera)
                            CLSFPCaptureDllWrapper.CLS_Terminate();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                        if (!((LeerCustodioEdificioViewModel)windowBusqueda.DataContext).IsSucceded)

                            return;

                        Custodio = ((LeerCustodioEdificioViewModel)windowBusqueda.DataContext).ListResultadoCustodio != null ? ((LeerCustodioEdificioViewModel)windowBusqueda.DataContext).ListResultadoCustodio.Select(se => se.Persona).FirstOrDefault() : null;
                        if (Custodio == null)
                        {
                            EnrolarInternosEnabled = false;
                            NombreBuscar = string.Empty;
                            ApellidoMaternoBuscar = string.Empty;
                            ApellidoPaternoBuscar = string.Empty;
                            FolioBuscar = null;
                            AsistenciaHabilitado = false;
                            AutorizarBtnEnabled = false;
                            ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
                            return;
                        }
                        FolioBuscar = Custodio.ID_PERSONA;
                        ApellidoPaternoBuscar = string.IsNullOrEmpty(Custodio.PATERNO) ? string.Empty : Custodio.PATERNO.TrimEnd();
                        ApellidoMaternoBuscar = string.IsNullOrEmpty(Custodio.MATERNO) ? string.Empty : Custodio.MATERNO.TrimEnd();
                        NombreBuscar = string.IsNullOrEmpty(Custodio.NOMBRE) ? string.Empty : Custodio.NOMBRE.TrimEnd();
                        ImagenCustodio = new cPersonaBiometrico().ObtenerTodos(Custodio.ID_PERSONA, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ? new cPersonaBiometrico().ObtenerTodos(Custodio.ID_PERSONA, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();
                        ClickSwitch("buscar_visible");
                        AsistenciaHabilitado = true;
                        AutorizarBtnEnabled = true;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                    }
                };
                windowBusqueda.ShowDialog();
                AceptarBusquedaHuellaFocus = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar custodio por huellas.", ex);
            }
        }

        private async void OnBuscarPorHuella(string obj = "")
        {
            try
            {
                AsistenciaBiometricaDeshabilitado = true;
                AsistenciaBiometricaSelect = true;
                AsistenciaNIPSelect = false;
                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
                await TaskEx.Delay(400);
                var nRet = -1;
                var bandera = true;
                var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaJuridico;
                if (requiereGuardarHuellas)
                    try
                    {
                        nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                    }
                    catch
                    {
                        bandera = false;
                    }
                else
                    bandera = false;

                var windowBusqueda = new LeerInternosEdificio();
                windowBusqueda.DataContext = new LeerInternosEdificioViewModel(enumTipoPersona.IMPUTADO, nRet == 0, requiereGuardarHuellas, SelectedEdificio.ID_EDIFICIO, SelectedSector.ID_SECTOR);

                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.LISTA_ASISTENCIA_INTERNO_EDIFICIO);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGÚRESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }
                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) =>
                {
                    try
                    {
                        if (e.Key == System.Windows.Input.Key.Escape) windowBusqueda.Close();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar", ex);
                    }
                };

                //LOGICA PARA DAR SALIDA A LOS INTERNOS (REQUERIDOS)
                if (SelectRequerido)
                {
                    ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).ModoHuella = false;
                    windowBusqueda.Closed += (s, e) =>
                    {
                        try
                        {
                            HuellasCapturadas = ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                            if (bandera)
                                CLSFPCaptureDllWrapper.CLS_Terminate();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                            if (!((LeerInternosEdificioViewModel)windowBusqueda.DataContext).IsSucceed)

                                return;

                            Imputado = ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).SelectRegistro != null ? ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).SelectRegistro.Imputado : null;
                            if (Imputado == null)
                                return;

                            AnioBuscar = Imputado.ID_ANIO;
                            FolioBuscar = Imputado.ID_IMPUTADO;
                            ApellidoPaternoBuscar = Imputado.PATERNO;
                            ApellidoMaternoBuscar = Imputado.MATERNO;
                            NombreBuscar = Imputado.NOMBRE;
                            ClickSwitch("buscar_visible");
                            SelectExpediente = Imputado;
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                        }
                    };
                    windowBusqueda.ShowDialog();

                    var x = ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).ListaResultadoRequerido;
                    if(x != null)
                    foreach (var item in x)
                    {
                        var interno = ListaInternosRequeridos.Where(w => w.IdImputado == item.IdImputado).FirstOrDefault();
                        if (interno != null)
                        {
                            interno.SELECCIONAR = true;
                            if (LstInternosSeleccionados == null)
                                  LstInternosSeleccionados = new List<InternosRequeridos>();
                            LstInternosSeleccionados.Add(interno);
                            ListaInternosRequeridos = new List<InternosRequeridos>(ListaInternosRequeridos);
                        }
                        #region comentado
                        //foreach (var row in ListaInternosRequeridos)
                        //{
                        //    if (item.IdImputado == row.IdImputado)
                        //    {
                        //        row.SELECCIONAR = true;
                        //        if (row.SELECCIONAR)
                        //        {
                        //            if (LstInternosSeleccionados == null)
                        //                LstInternosSeleccionados = new List<InternosRequeridos>();
                        //            LstInternosSeleccionados.Add(row);
                        //        }
                        //        //var distinctImp = from imp in LstInternosSeleccionados
                        //        //                     group imp by imp.IdImputado
                        //        //                         into gimp
                        //        //                         select gimp.First();

                        //        //InternosSeleccionados = string.Format("Internos Seleccionados: {0}", distinctImp.Count());
                        //    }
                        //}
                        #endregion
                    }
                    if (ListaInternosRequeridos != null)
                    {
                        //ListaInternosRequeridos = new List<InternosRequeridos>(ListaInternosRequeridos);
                        InternosSeleccionados = string.Format("Internos Seleccionados: {0}", ListaInternosRequeridos.Count(w => w.SELECCIONAR));

                    }
                    else
                    {
                        ListaInternosRequeridos = new List<InternosRequeridos>();
                        InternosSeleccionados = string.Format("Internos Seleccionados: {0}", 0); 
                    }

                    //ListaInternosRequeridos = new List<InternosRequeridos>(LisaInternosRequeridos);
                    AceptarBusquedaHuellaFocus = true;
                }
                //LOGICA PARA DAR ENTRADA A LOS INTERNOS (AUSENTES)
                else if (SelectAusente)
                {
                    ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).ModoHuella = true;
                    windowBusqueda.Closed += (s, e) =>
                    {
                        try
                        {
                            HuellasCapturadas = ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                            if (bandera)
                                CLSFPCaptureDllWrapper.CLS_Terminate();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                            if (!((LeerInternosEdificioViewModel)windowBusqueda.DataContext).IsSucceed)

                                return;

                            Imputado = ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).SelectRegistro != null ? ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).SelectRegistro.Imputado : null;
                            if (Imputado == null)
                                return;

                            AnioBuscar = Imputado.ID_ANIO;
                            FolioBuscar = Imputado.ID_IMPUTADO;
                            ApellidoPaternoBuscar = Imputado.PATERNO;
                            ApellidoMaternoBuscar = Imputado.MATERNO;
                            NombreBuscar = Imputado.NOMBRE;
                            ClickSwitch("buscar_visible");
                            SelectExpediente = Imputado;
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                        }
                    };
                    windowBusqueda.ShowDialog();

                    var x = ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).ListaResultadoRequerido;
                    if(x != null)
                    foreach (var item in ListaInternosSeleccionados)
                    {
                        var interno = x.Where(w => w.Expediente == item.Expediente).FirstOrDefault();
                        if (interno != null)
                        {
                            item.SELECTELIMINAR = true;
                            if (LstInternosSeleccionadosAusentes == null)
                                LstInternosSeleccionadosAusentes = new List<InternosAusentes>();
                            LstInternosSeleccionadosAusentes.Add(item);
                            ListaInternosSeleccionados = new ObservableCollection<InternosAusentes>(ListaInternosSeleccionados);
                        }
                        #region comentado
                        //foreach (var row in x)
                        //{
                        //    if (row.Expediente == item.Expediente)
                        //    {
                        //        item.SELECTELIMINAR = true;
                        //        if (item.SELECTELIMINAR)
                        //        {
                        //            if (LstInternosSeleccionadosAusentes == null)
                        //                LstInternosSeleccionadosAusentes = new List<InternosAusentes>();
                        //            LstInternosSeleccionadosAusentes.Add(item);
                        //        }
                        //        var distinctImp = from imp in LstInternosSeleccionadosAusentes
                        //                             group imp by imp.IdImputado
                        //                                 into gimp
                        //                                 select gimp.First();

                        //        InternosSeleccionados = string.Format("Internos Seleccionados: {0}", distinctImp.Count());
                        //    }
                        //}
                        #endregion
                    }
                    if (ListaInternosSeleccionados != null)
                    {
                        InternosSeleccionados = string.Format("Internos Seleccionados: {0}", ListaInternosSeleccionados.Count(w => w.SELECTELIMINAR));
                        ListaInternosSeleccionados = new ObservableCollection<InternosAusentes>(ListaInternosSeleccionados);
                    }
                    else
                    {
                        InternosSeleccionados = string.Format("Internos Seleccionados: {0}", 0);
                        ListaInternosSeleccionados = new ObservableCollection<InternosAusentes>();
                    }
                    
                    AceptarBusquedaHuellaFocus = true;
                }
            }
            catch (NullReferenceException ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la ventana de registro por huellas.", ex);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar interno por huellas.", ex);
            }
        }

        private void UnLoad(ControlInternosView Window = null)
        {
            PrincipalViewModel.CambiarVentanaSelecccionado += (o, e) =>
            {
                timer.Stop();
            };
        }

        private async void OnLoad(ControlInternosView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Window.Unloaded += (o, e) =>
                    {
                        timer.Stop();
                    };
                    PrincipalViewModel.SalirSistema += (o, e) =>
                    {
                        timer.Stop();
                    };
                    PrincipalViewModel.Cerrar += (o, e) =>
                    {
                        timer.Stop();
                    };
                    PrincipalViewModel.Cancelar += (o, e) =>
                    {
                        timer.Start();
                    };

                    AutorizarBtnEnabled = false;
                    AsistenciaHabilitado = false;
                    ImagenPlaceHolder = new Imagenes();
                    ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
                    ListaInternosSeleccionados = new ObservableCollection<InternosAusentes>();
                    setValidationRules();
                    GetAusentes();
                    ListaEdificio = new List<EDIFICIO>(new cEdificio().ObtenerTodos(string.Empty, 0, GlobalVar.gCentro).OrderBy(o => o.DESCR));
                    ListaEdificio.Insert(0, new EDIFICIO() { ID_EDIFICIO = -1, DESCR = "TODOS" });
                    SelectedEdificio = ListaEdificio.Where(w => w.ID_EDIFICIO == -1).FirstOrDefault();
                    var actual = Fechas.GetFechaDateServer.Date;
                    FechaInicio = actual;
                    FechaFin = actual;
                });
                IniciarTimer();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar.", ex);
            }
            ///DESCOMENTAR EN CASO DE OCUPAR CONFIGURAR PERMISOS
            ConfiguraPermisos();
        }

        private async void LlenarSector()
        {
            ListaSector = null;
            SelectedSector = null;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                if (SelectedEdificio.ID_EDIFICIO >= 0)
                {
                    ListaSector = new List<SECTOR>(SelectedEdificio.SECTOR.OrderBy(o => o.DESCR));
                    ListaSector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
                    SelectedSector = ListaSector.Where(w => w.ID_SECTOR == -1).FirstOrDefault();
                }
                else
                {
                    ListaSector = new List<SECTOR>(SelectedEdificio.SECTOR.OrderBy(o => o.DESCR));
                    ListaSector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
                    SelectedSector = ListaSector.Where(w => w.ID_SECTOR == -1).FirstOrDefault();
                }
            });
        }

        private void RemoverEventos()
        {
            PrincipalViewModel.CambiarVentanaSelecccionado -= (o, e) => { };
            PrincipalViewModel.SalirSistema -= (o, e) => { };
            PrincipalViewModel.Cerrar -= (o, e) => { };
            PrincipalViewModel.Cancelar -= (o, e) => { };
        }
        #endregion

        #region [PERMISOS]
        ///DADO QUE NO SE VISUALIZA EN INTERFAZ EL COMO PRESENTAR AL USUARIO LA PRESENTACION VISUAL DE LOS PRIVILEGIOS, SE REALIZA CONFIGURACION BASE A ESPERA DE DETERMINAR (POR PARTE DEL DESARROLLADOR DEL MODULO) LA CONFIGURACION.
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CONTROL_DE_INTERNOS_EN_EDIFICIOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username); //CONSULTA LA TABLA DE LOS PERMISOS, BUSCANDO POR EL NOMBRE DEL MODULO QUE SE DECLARA EN EL ENUMERADOR
                if (permisos.Any())//SI EXISTE ALGUNA CONFIGURACION DE PERMISOS PARA ESTE MODULO
                {
                    foreach (var p in permisos)//POR LO REGULAR ES SOLO UN CAMPO, SE RECORRE Y SE AJUSTAN LAS VARIABLES PARA QUE LOS ENABLED DE LA BARRA DE MENU QUE SE USA(VARIA EN ALGUNAS VISTAS, CAMBIAR POR LAS VARIABLES QUE USA LA VISTA Y ADECUAR SEGUN LA NECESIDAD).
                    {
                        //if (p.INSERTAR == 1)//VALOR QUE VIENE DESDE LA TABLA
                        //    PInsertar = true;//VARIABLE QUE DEBEN DECLARAR SI ES QUE USAN UNA VISTA DIFERENTE A LAS CONVENCIONALES.
                        //if (p.EDITAR == 1)
                        //    PEditar = true;
                        if (p.CONSULTAR == 1)
                        {
                            EdificioHablititado = true;
                            SectorHabilitado = true;
                            FechaInicioHabilitado = true;
                            FechaFinalHabilitado = true;
                            BuscarHabilitado = true;
                            TabRequeridoHabilitado = true;
                            TabAusenteHabilitado = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
}
