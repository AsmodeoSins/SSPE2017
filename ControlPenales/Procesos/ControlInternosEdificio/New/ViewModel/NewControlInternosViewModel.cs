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
    partial class NewControlInternosViewModel : ValidationViewModelBase
    {
        #region [CONSTRUCTOR]
        public NewControlInternosViewModel() { }
        #endregion

        #region [METODOS]
        private async void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    if (IndiceTab == 0)
                    {
                        timer.Stop();
                        tMinuto = 1;
                        tSegundo = 59;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateRequeridos);
                        timer.Start();
                    }
                    else
                    {
                        timer.Stop();
                        tMinuto = 1;
                        tSegundo = 59;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateAusentes);
                        timer.Start();
                    }
                    break;
                case "registroSalida":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    BuscarHuella();
                    break;
                case "asignarCustodio":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    OnBuscarPorHuellaCustodio();
                    break;
                case "limpiarEnrolamientos":
                    LimpiarCustodio();
                    break;
                case "autorizar":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    if (IndiceTab == 0)
                        GuardarRequeridos();
                    else
                        GuardarAusentes();
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NewControlInternosEdificioView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.NewControlInternosViewModel();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }
        
        private void UnLoad(NewControlInternosEdificioView Window = null)
        {
            //PrincipalViewModel.CambiarVentanaSelecccionado += (o, e) =>
            //{
            //    timer.Stop();
            //};
        }

        private async void OnLoad(NewControlInternosEdificioView Window = null)
        {
            try
            {
                ConfiguraPermisos();
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    LstEdificio = new ObservableCollection<EDIFICIO>(new cEdificio().ObtenerTodos(string.Empty, 0, GlobalVar.gCentro, "S"));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstEdificio.Insert(0, new EDIFICIO() { ID_EDIFICIO = -1, DESCR = "TODOS" });
                    }));

                    #region Timer
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
                    #endregion
                });
                if(pConsultar)
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateAusentes);
                IniciarTimer();

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar.", ex);
            }
            ///DESCOMENTAR EN CASO DE OCUPAR CONFIGURAR PERMISOS
            //ConfiguraPermisos();
        }

        private void PopulateRequeridos()
        {
            try
            {
                LstInternosRequeridos = new ObservableCollection<cControlInternoEdificio>(new cIngresoUbicacion().ObtenerInternosRequeridos(GlobalVar.gCentro,FEdificio != -1 ? (short?)FEdificio : null,FSector != -1 ? (short?)FSector : null).Select(w => new cControlInternoEdificio()
                {
                 SELECCIONE = false,
                 UBICACION = w.UBICACION,
                 ID_CENTRO = w.ID_CENTRO,
                 ID_ANIO = w.ID_ANIO,
                 ID_IMPUTADO = w.ID_IMPUTADO,
                 ID_INGRESO = w.ID_INGRESO,
                 EXPEDIENTE = string.Format("{0}/{1}",w.ID_ANIO,w.ID_IMPUTADO),
                 NOMBRE = w.NOMBRE,
                 PATERNO = w.PATERNO,
                 MATERNO = w.MATERNO,
                 ID_UB_CENTRO = w.ID_UB_CENTRO,
                 ID_UB_EDIFICIO = w.ID_UB_EDIFICIO,
                 ID_UB_SECTOR = w.ID_UB_SECTOR,
                 ID_UB_CELDA = w.ID_UB_CELDA,
                 ID_UB_CAMA = w.ID_UB_CAMA,
                 UBICACION_CENTRO = string.Format("{0}-{1}-{2}-{3}",w.EDIFICIO,w.SECTOR,w.ID_UB_CELDA,w.ID_UB_CAMA),
                 UBICACION_ACTUAL = !string.IsNullOrEmpty(w.UBICACION_ACTUAL) ? w.UBICACION_ACTUAL : string.Format("{0}-{1}-{2}-{3}",w.EDIFICIO,w.SECTOR,w.ID_UB_CELDA,w.ID_UB_CAMA),
                 FECHA_ACTIVIDAD = w.FECHA_ACTIVIDAD,
                 HORA_ACTIVIDAD = w.HORA_ACTIVIDAD,
                 ID_AREA = w.ID_AREA,
                 AREA = w.AREA,
                 ACTIVIDAD = w.ACTIVIDAD,
                 TIPO = w.TIPO
                }));

                #region Seleccionados
                if (LstInternosRequeridos != null)
                {
                    if (lstInternosRequeridosSeleccionados != null)
                    {
                        foreach (var l in lstInternosRequeridosSeleccionados)
                        {
                            var x = LstInternosRequeridos.FirstOrDefault(w => w.ID_CENTRO == l.ID_CENTRO && w.ID_ANIO == l.ID_ANIO && w.ID_IMPUTADO == l.ID_IMPUTADO);
                            if (x != null)
                                x.SELECCIONE = true;
                        }
                    }
                    LstInternosRequeridos = new ObservableCollection<cControlInternoEdificio>(LstInternosRequeridos);
                }
                #endregion

                if (IndiceTab == 0)
                {
                    TotalInternos = string.Format("Total de Internos: {0}", LstInternosRequeridos != null ? LstInternosRequeridos.Count : 0);
                    TotalSeleccionados = string.Format("Total de Internos Seleccionados: {0}", LstInternosRequeridos != null ? LstInternosRequeridos.Count(w => w.SELECCIONE) : 0);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar internos requeridos.", ex);
            }
        }

        private void PopulateAusentes() {
            try
            {
                LstInternosAusentes = new ObservableCollection<cControlInternoEdificio>(new cIngresoUbicacion().ObtenerInternosAusentes(GlobalVar.gCentro).Select(w => new cControlInternoEdificio()
                {
                    SELECCIONE = false,
                    UBICACION = w.UBICACION,
                    ID_CENTRO = w.ID_CENTRO,
                    ID_ANIO = w.ID_ANIO,
                    ID_IMPUTADO = w.ID_IMPUTADO,
                    ID_INGRESO = w.ID_INGRESO,
                    EXPEDIENTE = string.Format("{0}/{1}", w.ID_ANIO, w.ID_IMPUTADO),
                    NOMBRE = w.NOMBRE,
                    PATERNO = w.PATERNO,
                    MATERNO = w.MATERNO,
                    ID_UB_CENTRO = w.ID_UB_CENTRO,
                    ID_UB_EDIFICIO = w.ID_UB_EDIFICIO,
                    ID_UB_SECTOR = w.ID_UB_SECTOR,
                    ID_UB_CELDA = w.ID_UB_CELDA,
                    ID_UB_CAMA = w.ID_UB_CAMA,
                    UBICACION_CENTRO = string.Format("{0}-{1}-{2}-{3}", w.EDIFICIO, w.SECTOR, w.ID_UB_CELDA, w.ID_UB_CAMA),
                    FECHA_ACTIVIDAD = w.FECHA_ACTIVIDAD,
                    HORA_ACTIVIDAD = w.HORA_ACTIVIDAD,
                    ID_AREA = w.ID_AREA,
                    AREA = w.AREA,
                    ACTIVIDAD = w.ACTIVIDAD,
                    TIPO = w.TIPO
                }));

                #region Seleccionados
                if (LstInternosAusentes != null)
                {
                    if (lstInternosAusentesSeleccionados != null)
                    {
                        foreach (var l in lstInternosAusentesSeleccionados)
                        {
                            var x = LstInternosAusentes.FirstOrDefault(w => w.ID_CENTRO == l.ID_CENTRO && w.ID_ANIO == l.ID_ANIO && w.ID_IMPUTADO == l.ID_IMPUTADO);
                            if (x != null)
                                x.SELECCIONE = true;
                        }
                    }
                    LstInternosAusentes = new ObservableCollection<cControlInternoEdificio>(LstInternosAusentes);
                }
                #endregion

                if (IndiceTab == 2)
                {
                    TotalInternos = string.Format("Total de Internos: {0}", LstInternosAusentes != null ? LstInternosAusentes.Count : 0);
                    TotalSeleccionados = string.Format("Total de Internos Seleccionados: {0}", LstInternosAusentes != null ? LstInternosAusentes.Count(w => w.SELECCIONE) : 0);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar internos ausentes.", ex);
            }
        }

        private void BuscarHuella()
        {
            try
            {
                //var windowBusqueda = new LeerInternosEdificio();
                //windowBusqueda.DataContext = new LeerInternosEdificioViewModel(enumTipoPersona.IMPUTADO,null,false,FEdificio != -1 ? (short?)FEdificio : null,FSector != -1 ? (short?)FSector : null);
                var windowBusqueda = new LeerInternosEdificio();
                windowBusqueda.DataContext = new LeerInternosEdificioViewModel(enumTipoPersona.IMPUTADO, null, false, FEdificio, FSector);
                if (CLSFPCaptureDllWrapper.CLS_Initialize() != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.LISTA_ASISTENCIA_INTERNO_EDIFICIO);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGÚRESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
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
                windowBusqueda.Closed += (s, e) =>
                {
                    try
                    {
                        var resultado = ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).ListaResultadoRequerido;
                        if (resultado != null)
                        {
                            if (IndiceTab == 0)
                            {
                                if (LstInternosRequeridos != null)
                                {
                                    foreach (var r in resultado)
                                    {
                                        var imp = LstInternosRequeridos.Where(w => w.ID_CENTRO == r.IdCentro && w.ID_ANIO == r.IdAnio && w.ID_IMPUTADO == r.IdImputado && w.UBICACION == 0).FirstOrDefault();
                                        if (imp != null)
                                        {
                                            imp.SELECCIONE = true;
                                            if (lstInternosRequeridosSeleccionados == null)
                                                lstInternosRequeridosSeleccionados = new ObservableCollection<cControlInternoEdificio>();
                                            lstInternosRequeridosSeleccionados.Add(imp);
                                        }
                                    }
                                    LstInternosRequeridos = new ObservableCollection<cControlInternoEdificio>(LstInternosRequeridos);
                                    TotalInternos = string.Format("Total de Internos: {0}", LstInternosRequeridos != null ? LstInternosRequeridos.Count : 0);
                                    TotalSeleccionados = string.Format("Total de Internos Seleccionados: {0}", LstInternosRequeridos != null ? LstInternosRequeridos.Count(w => w.SELECCIONE) : 0);
                                }
                            }
                            else
                            {
                                if (LstInternosAusentes != null)
                                {
                                    foreach (var r in resultado)
                                    {
                                        var imp = LstInternosAusentes.Where(w => w.ID_CENTRO == r.IdCentro && w.ID_ANIO == r.IdAnio && w.ID_IMPUTADO == r.IdImputado && w.UBICACION > 0).FirstOrDefault();
                                        if (imp != null)
                                        {
                                            imp.SELECCIONE = true;
                                            if (lstInternosAusentesSeleccionados == null)
                                                lstInternosAusentesSeleccionados = new ObservableCollection<cControlInternoEdificio>();
                                            lstInternosAusentesSeleccionados.Add(imp);
                                        }
                                    }
                                    LstInternosAusentes = new ObservableCollection<cControlInternoEdificio>(LstInternosAusentes);
                                    TotalInternos = string.Format("Total de Internos: {0}", LstInternosAusentes != null ? LstInternosAusentes.Count : 0);
                                    TotalSeleccionados = string.Format("Total de Internos Seleccionados: {0}", LstInternosAusentes != null ? LstInternosAusentes.Count(w => w.SELECCIONE) : 0);                                
                                }
                            }
                            
                        }
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                    }
                };
                if(IndiceTab == 1)
                    ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).ModoHuella = true;
                else
                    ((LeerInternosEdificioViewModel)windowBusqueda.DataContext).ModoHuella = false;
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar la huella del imputado.", ex);
            }
        }

        private async void OnBuscarPorHuellaCustodio()
        {
            try
            {
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
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
                windowBusqueda.DataContext = new LeerCustodioEdificioViewModel(enumTipoPersona.PERSONA_EMPLEADO, nRet == 0, requiereGuardarHuellas, null,null);

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
                        if (e.Key == System.Windows.Input.Key.Escape)
                        { 
                            windowBusqueda.Close();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        }
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
                        
                        var c = ((LeerCustodioEdificioViewModel)windowBusqueda.DataContext).ListResultadoCustodio != null ? ((LeerCustodioEdificioViewModel)windowBusqueda.DataContext).ListResultadoCustodio.Select(se => se.Persona).FirstOrDefault() : null;
                        if (c != null)
                            SelectedCustodio = new cPersona().Obtener(c.ID_PERSONA).FirstOrDefault();
                        else
                            SelectedCustodio = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                    }
                };
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar custodio por huellas.", ex);
            }
        }

        private void LimpiarCustodio()
        {
            FolioBuscar = null;
            NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
            ImagenCustodio = new Imagenes().getImagenPerson();
            SelectedCustodio = null;
            AutorizarBtnEnabled = false;
        }

        private async void GuardarRequeridos() 
        {
            try
            {
                if (LstInternosRequeridos != null)
                {
                    if (LstInternosRequeridos.Count(w => w.SELECCIONE) > 0)
                    { 
                        var hoy = Fechas.GetFechaDateServer;
                        var iu = new List<INGRESO_UBICACION>();
                        foreach (var w in LstInternosRequeridos.Where(w => w.SELECCIONE))
                        {
                            iu.Add(new INGRESO_UBICACION()
                            {
                                ID_CENTRO = w.ID_CENTRO,
                                ID_ANIO = w.ID_ANIO,
                                ID_IMPUTADO = w.ID_IMPUTADO,
                                ID_INGRESO = w.ID_INGRESO,
                                ID_CONSEC = 0,
                                ID_AREA = w.ID_AREA,
                                MOVIMIENTO_FEC = hoy,
                                ACTIVIDAD = w.ACTIVIDAD,
                                ESTATUS = 1,
                                ID_CUSTODIO = SelectedCustodio != null ? (int?)SelectedCustodio.ID_PERSONA : null
                                //INTERNO_UBICACION
                            });
                        }

                        if(new cIngresoUbicacion().Insertar(iu))
                        {
                            lstInternosRequeridosSeleccionados = new ObservableCollection<cControlInternoEdificio>();
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateRequeridos);
                            new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "No ha seleccionado a ningun interno");
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación","No ha seleccionado a ningun interno");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los internos requeridos.", ex);
            }
        }

        private async void GuardarAusentes()
        {
            try
            {
                if (LstInternosAusentes != null)
                {
                    if (LstInternosAusentes.Count(w => w.SELECCIONE) > 0)
                    {
                        var hoy = Fechas.GetFechaDateServer;
                        var iu = new List<INGRESO_UBICACION>();
                        foreach (var w in LstInternosAusentes.Where(w => w.SELECCIONE))
                        {
                            iu.Add(new INGRESO_UBICACION()
                            {
                                ID_CENTRO = w.ID_CENTRO,
                                ID_ANIO = w.ID_ANIO,
                                ID_IMPUTADO = w.ID_IMPUTADO,
                                ID_INGRESO = w.ID_INGRESO,
                                ID_CONSEC = 0,
                                ID_AREA = w.ID_AREA,
                                MOVIMIENTO_FEC = hoy,
                                ACTIVIDAD = w.ACTIVIDAD,
                                ESTATUS = 0,
                                //ID_CUSTODIO = 
                                INTERNO_UBICACION = "S"
                            });
                        }

                        if (new cIngresoUbicacion().Insertar(iu))
                        {
                            lstInternosAusentesSeleccionados = new ObservableCollection<cControlInternoEdificio>();
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateAusentes);
                            new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "No ha seleccionado a ningun interno");
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "No ha seleccionado a ningun interno");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los internos requeridos.", ex);
            }
        }
        #endregion

        #region [PERMISOS]
        ///DADO QUE NO SE VISUALIZA EN INTERFAZ EL COMO PRESENTAR AL USUARIO LA PRESENTACION VISUAL DE LOS PRIVILEGIOS, SE REALIZA CONFIGURACION BASE A ESPERA DE DETERMINAR (POR PARTE DEL DESARROLLADOR DEL MODULO) LA CONFIGURACION.
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CONTROL_DE_INTERNOS_EN_EDIFICIOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username); //CONSULTA LA TABLA DE LOS PERMISOS, BUSCANDO POR EL NOMBRE DEL MODULO QUE SE DECLARA EN EL ENUMERADOR
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            pInsertar = true;
                        if (p.EDITAR == 1)
                            pEditar = true;
                        if (p.CONSULTAR == 1)
                            pConsultar = true;
                        if (p.IMPRIMIR == 1)
                            pImprimir = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region Timer
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
            if (tMinuto == 1)
            {
                if (tSegundo >= 1)
                    tSegundo--;
                else
                {
                    if (tSegundo == 0)
                    {
                        tMinuto--;
                        tSegundo = 59;
                    }
                }
                Actualizacion = string.Format("Actualización en: 00:0{0}:{1}", tMinuto, tSegundo > 9 ? tSegundo.ToString() : "0" + tSegundo);
            }
            else
            {
                if (tSegundo >= 1)
                    tSegundo--;
                else
                if (tSegundo == 0)
                {
                    tMinuto = 1;
                    tSegundo = 59;
                    timer.Stop();
                    StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateRequeridos);
                    StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateAusentes);
                    timer.Start();
                    return;
                }
                Actualizacion = string.Format("Actualización en: 00:0{0}:{1}", tMinuto, tSegundo > 9 ? tSegundo.ToString() : "0" + tSegundo);
            }
        }
        #endregion
    }
}
