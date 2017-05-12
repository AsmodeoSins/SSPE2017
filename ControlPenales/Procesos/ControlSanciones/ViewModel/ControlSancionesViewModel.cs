
using Novacode;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ControlPenales
{
    partial class ControlSancionesViewModel : ValidationViewModelBase
    {
        public ControlSancionesViewModel() { }

        async void OnLoad(ControlSancionesView obj)
        {
            try
            {
                ListIncidente = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INCIDENTE_TIPO>>(() => new ObservableCollection<INCIDENTE_TIPO>(new cIncidenteTipo().GetData().ToList()));
                ListIncidente.Insert(0, new INCIDENTE_TIPO() { ID_INCIDENTE_TIPO = -1, DESCR = "SELECIONE" });

                ListTipoSanciones = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SANCION_TIPO>>(() => new ObservableCollection<SANCION_TIPO>(new cSancionTipo().GetData().ToList()));
                ListTipoSanciones.Insert(0, new SANCION_TIPO() { ID_SANCION = -1, DESCR = "SELECCIONE" });

                CargarDatos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar control sanciones", ex);
            }
        }

        private async void clickSwitch(object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "guardar_menu":
                        break;
                    case "buscar_menu":
                        break;
                    case "limpiar_menu":
                        //CargarDatos();
                        //ListSanciones = null;
                        //ImagenImputado = null;
                        //FechaRegistro = null;
                        //TextMotivo = null;
                        //ListIncidente = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INCIDENTE_TIPO>>(() => new ObservableCollection<INCIDENTE_TIPO>(new cIncidenteTipo().GetData().ToList()));
                        StaticSourcesViewModel.SourceChanged = false;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ControlSancionesView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.ControlSancionesViewModel();
                        break;
                    case "reporte_menu":
                        if (!PImprimir)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }
                        if (SelectIncidentesCumplimentar == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una sanción a cumplimentar");
                            return;
                        }
                        EsActa = EsCitatorioInterno = EsParteInformativo = false;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONAR_REPORTE_SANCION);

                        //ImprimirReporte();
                        break;
                    case "ficha_menu":
                        break;
                    case "ayuda_menu":
                        break;
                    case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                        break;
                    case "sancion_autorizar":

                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }
                        if (TabIndex == 0)
                        {
                            if (SelectIncidentes == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un interno.");
                                break;
                            }
                        }
                        else if(TabIndex == 1)
                        {
                            if (SelectIncidentesCumplimentar == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un interno.");
                                break;
                            }
                        }
                        
                        #region Comentado
                        //new cIncidente().Actualizar(new INCIDENTE
                        //{
                        //    AUTORIZACION_FEC = Fechas.GetFechaDateServer,
                        //    ESTATUS = "A",
                        //    ID_ANIO = SelectIncidentes.Id_Anio,
                        //    ID_CENTRO = SelectIncidentes.Id_Centro,
                        //    ID_IMPUTADO = SelectIncidentes.Id_Imputado,
                        //    ID_INCIDENTE = SelectIncidentes.Id_Incidente,
                        //    ID_INCIDENTE_TIPO = SelectIncidentes.Id_Incidente_Tipo,
                        //    ID_INGRESO = SelectIncidentes.Id_Ingreso,
                        //    MOTIVO = SelectIncidentes.Motivo,
                        //    REGISTRO_FEC = SelectIncidentes.Registro_Fecha
                        //});
#endregion
                        CambiarEstatus("A",Fechas.GetFechaDateServer);
                        CargarDatos();

                        break;
                    case "sancion_cancelar":
                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        if (TabIndex == 0)
                        {
                            if (SelectIncidentes == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un interno.");
                                break;
                            }
                        }
                        else if (TabIndex == 1)
                        {
                            if (SelectIncidentesCumplimentar == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un interno.");
                                break;
                            }
                        }
                        CambiarEstatus("C");
                        CargarDatos();
                        break;
                    case "sancion_pendiente":

                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        if (TabIndex == 0)
                        {
                            if (SelectIncidentes == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un interno.");
                                break;
                            }
                        }
                        else if (TabIndex == 1)
                        {
                            if (SelectIncidentesCumplimentar == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un interno.");
                                break;
                            }
                        }
                        CambiarEstatus("P");
                        #region comentado

//                        if (SelectIncidentes != null)
//                            new cIncidente().ActualizarEstatus(new INCIDENTE() { 
//                            ID_CENTRO = SelectIncidentes.Id_Centro,
//                            ID_ANIO = SelectIncidentes.Id_Anio,
//                            ID_IMPUTADO = SelectIncidentes.Id_Imputado,
//                            ID_INGRESO = SelectIncidentes.Id_Ingreso,
//                            ID_INCIDENTE = SelectIncidentes.Id_Incidente,
//                            ESTATUS = "P"
//                            });
//                            #region comentado
//                           // new cIncidente().Actualizar(new INCIDENTE
//                           //{
//                           //    AUTORIZACION_FEC = null,
//                           //    ESTATUS = "P",
//                           //    ID_ANIO = SelectIncidentes.Id_Anio,
//                           //    ID_CENTRO = SelectIncidentes.Id_Centro,
//                           //    ID_IMPUTADO = SelectIncidentes.Id_Imputado,
//                           //    ID_INCIDENTE = SelectIncidentes.Id_Incidente,
//                           //    ID_INCIDENTE_TIPO = SelectIncidentes.Id_Incidente_Tipo,
//                           //    ID_INGRESO = SelectIncidentes.Id_Ingreso,
//                           //    MOTIVO = SelectIncidentes.Motivo,
//                           //    REGISTRO_FEC = SelectIncidentes.Registro_Fecha
//                           //});
//#endregion

//                        if (SelectIncidentesCumplimentar != null)
//                            new cIncidente().ActualizarEstatus(new INCIDENTE() { 
//                            ID_CENTRO = SelectIncidentesCumplimentar.Id_Centro,
//                            ID_ANIO = SelectIncidentesCumplimentar.Id_Anio,
//                            ID_IMPUTADO = SelectIncidentesCumplimentar.Id_Imputado,
//                            ID_INGRESO = SelectIncidentesCumplimentar.Id_Ingreso,
//                            ID_INCIDENTE = SelectIncidentesCumplimentar.Id_Incidente,
//                            ESTATUS = "P"
//                            });
//                            #region comentado
//                            //new cIncidente().Actualizar(new INCIDENTE
//                            //{
//                            //    AUTORIZACION_FEC = null,
//                            //    ESTATUS = "P",
//                            //    ID_ANIO = SelectIncidentesCumplimentar.Id_Anio,
//                            //    ID_CENTRO = SelectIncidentesCumplimentar.Id_Centro,
//                            //    ID_IMPUTADO = SelectIncidentesCumplimentar.Id_Imputado,
//                            //    ID_INCIDENTE = SelectIncidentesCumplimentar.Id_Incidente,
//                            //    ID_INCIDENTE_TIPO = SelectIncidentesCumplimentar.Id_Incidente_Tipo,
//                            //    ID_INGRESO = SelectIncidentesCumplimentar.Id_Ingreso,
//                            //    MOTIVO = SelectIncidentesCumplimentar.Motivo,
//                            //    REGISTRO_FEC = SelectIncidentesCumplimentar.Registro_Fecha
//                        //});
                        //#endregion
#endregion
                        CargarDatos();
                        break;
                    case "sancion_calendarizar":
                        break;
                    case "insertar_sancion":
                        if (SelectIncidentes == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un incidente.");
                            break; 
                        }

                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        #region Limpiar
                        IdSancionTipo = -1;
                        FechaInicio = FechaFin = null;
                        HoraInicio = "00";
                        MinutoInicio = "00";

                        HoraFin = "23";
                        MinutoFin = "00";
                        #endregion

                        SetValidacionesSancion();
                        SelectSancion = ListTipoSanciones.Where(w => w.ID_SANCION == -1).FirstOrDefault();
                        //SelectedIncidente = -1;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                        //ListTipoSanciones = ListTipoSanciones ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SANCION_TIPO>>(() => new ObservableCollection<SANCION_TIPO>(new cSancionTipo().GetData().ToList()));
                        Agregar = true;
                        FechaLowerVal = Fechas.GetFechaDateServer;
                        FechaUpperVal = Fechas.GetFechaDateServer.AddDays(2);
                        HoraLowerVal = new DateTime(0001, 01, 01, 10, 0, 0);
                        HoraUpperVal = new DateTime(0001, 01, 01, 15, 0, 0);
                        break;
                    case "editar_sancion":
                        if (SelectSanciones == null)
                            break;

                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        SetValidacionesSancion();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                        //ListTipoSanciones = ListTipoSanciones ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SANCION_TIPO>>(() => new ObservableCollection<SANCION_TIPO>(new cSancionTipo().GetData().ToList()));
                        Agregar = false;
                        #region Limpiar
                        IdSancionTipo = -1;
                        FechaInicio = FechaFin = null;
                        HoraInicio = "00";
                        MinutoInicio = "00";
                        HoraFin = "23";
                        MinutoFin = "59";
                        #endregion

                        IdSancionTipo = SelectSanciones.Id_Sancion;
                        FechaInicio = SelectSanciones.IniciaFecha;
                        FechaFin = SelectSanciones.TerminaFecha;
                        if (FechaInicio.Value.Hour > 9)
                            HoraInicio = FechaInicio.Value.Hour.ToString();
                        else
                            HoraInicio = "0" + FechaInicio.Value.Hour;

                        if (FechaInicio.Value.Minute > 9)
                            MinutoInicio = FechaInicio.Value.Minute.ToString();
                        else
                            MinutoInicio = "0" + FechaInicio.Value.Minute;


                        if (FechaFin.Value.Hour > 9)
                            HoraFin = FechaFin.Value.Hour.ToString();
                        else
                            HoraFin = "0" + FechaFin.Value.Hour;

                        if (FechaFin.Value.Minute > 9)
                            MinutoFin = FechaFin.Value.Minute.ToString();
                        else
                            MinutoFin = "0" + FechaFin.Value.Minute;

                        //FechaLowerVal = SelectSanciones.IniciaFecha.Value;
                        //FechaUpperVal = SelectSanciones.TerminaFecha.Value;
                        //HoraLowerVal = SelectSanciones.IniciaFecha.Value;
                        //HoraUpperVal = SelectSanciones.TerminaFecha.Value;
                        break;
                    case "borrar_sancion":
                        break;
                    case "guardar_sancionpopup":

                        if (this.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación","Favor de capturar los datos requeridos");
                            break;
                        }
                        //Validacion fechas
                        if (FechaInicio.Value.Date > FechaFin.Value.Date)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La fecha de inicio debe ser menor o igual a la fecha final");
                            break;
                        }
                        //Validacion horas
                        int inicio = int.Parse(HoraInicio + MinutoInicio);
                        int fin = int.Parse(HoraFin + MinutoFin);
                        if (inicio > fin)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La hora de inicio debe ser menor o igual a la hora final");
                            break;
                        }

                        var fi = FechaInicio.Value.Date;
                        fi = fi.AddHours(int.Parse(HoraInicio));
                        fi = fi.AddMinutes(int.Parse(MinutoInicio));

                        var ff = FechaFin.Value.Date;
                        ff = ff.AddHours(int.Parse(HoraFin));
                        ff = ff.AddMinutes(int.Parse(MinutoFin));
                        if (Agregar)
                        {

                            new cSancion().Insert(new SANCION
                            {
                                ID_ANIO = SelectIncidentes.Id_Anio,
                                ID_CENTRO = SelectIncidentes.Id_Centro,
                                ID_IMPUTADO = SelectIncidentes.Id_Imputado,
                                ID_INCIDENTE = SelectIncidentes.Id_Incidente,
                                ID_INGRESO = SelectIncidentes.Id_Ingreso,
                                ID_SANCION = SelectSancion.ID_SANCION,
                                INICIA_FEC = fi,//new DateTime(FechaLowerVal.Year, FechaLowerVal.Month, FechaLowerVal.Day, HoraLowerVal.Hour, HoraLowerVal.Minute, HoraLowerVal.Second),
                                TERMINA_FEC = ff,//new DateTime(FechaUpperVal.Year, FechaUpperVal.Month, FechaUpperVal.Day, HoraUpperVal.Hour, HoraUpperVal.Minute, HoraUpperVal.Second)
                            });

                            CargarSancionesImputado(SelectIncidentes.Id_Centro, SelectIncidentes.Id_Anio, SelectIncidentes.Id_Imputado, SelectIncidentes.Id_Ingreso, SelectIncidentes.Id_Incidente);
                        }
                        else
                        {
                            new cSancion().Actualizar(new SANCION
                            {
                                ID_ANIO = SelectSanciones.Id_Anio,
                                ID_CENTRO = SelectSanciones.Id_Centro,
                                ID_IMPUTADO = SelectSanciones.Id_Imputado,
                                ID_INCIDENTE = SelectSanciones.Id_Incidente,
                                ID_INGRESO = SelectSanciones.Id_Ingreso,
                                ID_SANCION = SelectSanciones.Id_Sancion,
                                INICIA_FEC = fi,//new DateTime(FechaLowerVal.Year, FechaLowerVal.Month, FechaLowerVal.Day, HoraLowerVal.Hour, HoraLowerVal.Minute, HoraLowerVal.Second),
                                TERMINA_FEC = ff,//new DateTime(FechaUpperVal.Year, FechaUpperVal.Month, FechaUpperVal.Day, HoraUpperVal.Hour, HoraUpperVal.Minute, HoraUpperVal.Second)

                            });

                            CargarSancionesImputado(SelectSanciones.Id_Centro, SelectSanciones.Id_Anio, SelectSanciones.Id_Imputado, SelectSanciones.Id_Ingreso, SelectSanciones.Id_Incidente);
                        }

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                        this.ClearRules();
                        CargarDatos();
                        break;
                    case "cancelar_sancionpopup":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                        this.ClearRules();
                        break;
                    case "VerTodosCumplimentar":
                        FechaSancionesCumplidas = null;
                        break;

                    case "seleccionar_reporte":
                        if (EsActa || EsCitatorioInterno || EsParteInformativo)
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONAR_REPORTE_SANCION);
                            if (EsActa)
                            {
                                ImprimirActa();
                            }
                            else
                                if (EsCitatorioInterno)
                                {
                                    ImprimirCitatorioInterno();
                                }
                                else
                                    if (EsParteInformativo)
                                    { 
                                    
                                    }
                             
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar el formato a imprimir");
                        
                        break;
                    
                    case "cancelar_reporte":
                        EsActa = EsCitatorioInterno = EsParteInformativo = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONAR_REPORTE_SANCION);
                        break;
                   
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo", ex);
            }
        }

        private async void CargarSancionesImputado(int IdCentro, int IdAnio, int IdImputado, int IdIngreso, int IdIncidente)
        {
            try
            {
                ListSanciones = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListadoSanciones>>(() => new ObservableCollection<ListadoSanciones>(new cSancion().GetData().Where(w => w.ID_CENTRO == IdCentro && w.ID_ANIO == IdAnio && w.ID_IMPUTADO == IdImputado && w.ID_INGRESO == IdIngreso && w.ID_INCIDENTE == IdIncidente).Select(s => new ListadoSanciones
                    {
                        STR_Sancion = s.SANCION_TIPO.DESCR,
                        IniciaFecha = s.INICIA_FEC,
                        TerminaFecha = s.TERMINA_FEC,
                        Id_Sancion = s.ID_SANCION,
                        Id_Anio = s.ID_ANIO,
                        Id_Centro = s.ID_CENTRO,
                        Id_Imputado = s.ID_IMPUTADO,
                        Id_Incidente = s.ID_INCIDENTE,
                        Id_Ingreso = s.ID_INGRESO
                    }).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar sanciones imputado", ex);
            }
        }

        private async void CargarDatos()
        {
            try
            {
                ListIncidentes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListadoIncidentes>>(() => new ObservableCollection<ListadoIncidentes>(new cIncidente().GetData().Where(w => w.ESTATUS == "P" && w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).Select(s => new ListadoIncidentes
                {
                    Autorizacion_Fecha = s.AUTORIZACION_FEC,
                    Estatus = "PENDIENTE",
                    Id_Anio = s.ID_ANIO,
                    Id_Centro = s.ID_CENTRO,
                    Id_Imputado = s.ID_IMPUTADO,
                    Id_Incidente = s.ID_INCIDENTE,
                    Id_Incidente_Tipo = s.ID_INCIDENTE_TIPO,
                    Id_Ingreso = s.ID_INGRESO,
                    Materno = s.INGRESO.IMPUTADO.MATERNO,
                    Motivo = s.MOTIVO,
                    Nombre = s.INGRESO.IMPUTADO.NOMBRE,
                    Paterno = s.INGRESO.IMPUTADO.PATERNO,
                    Registro_Fecha = s.REGISTRO_FEC,

                    ImagenImputado = s.INGRESO.INGRESO_BIOMETRICO
                }).ToList()));

                ListIncidentesCumplimentar = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListadoIncidentes>>(() => new ObservableCollection<ListadoIncidentes>(new cIncidente().GetData().Where(w => w.ESTATUS == "A" && w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).Select(s => new ListadoIncidentes
                {
                    Autorizacion_Fecha = s.AUTORIZACION_FEC,
                    Estatus = "AUTORIZADA",
                    Id_Anio = s.ID_ANIO,
                    Id_Centro = s.ID_CENTRO,
                    Id_Imputado = s.ID_IMPUTADO,
                    Id_Incidente = s.ID_INCIDENTE,
                    Id_Incidente_Tipo = s.ID_INCIDENTE_TIPO,
                    Id_Ingreso = s.ID_INGRESO,
                    Materno = s.INGRESO.IMPUTADO.MATERNO,
                    Motivo = s.MOTIVO,
                    Nombre = s.INGRESO.IMPUTADO.NOMBRE,
                    Paterno = s.INGRESO.IMPUTADO.PATERNO,
                    Registro_Fecha = s.REGISTRO_FEC,

                    ImagenImputado = s.INGRESO.INGRESO_BIOMETRICO
                }).ToList()));

                ConfiguraPermisos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CONTROL_SANCIONES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = MenuInsertarEnabled = true;
                        if (p.EDITAR == 1)
                            PEditar = /*MenuGuardarEnabled =*/ true;
                        if (p.CONSULTAR == 1)
                            PConsultar = true;
                        if (p.IMPRIMIR == 1)
                            PImprimir = MenuReporteEnabled =  true;

                        //if (!PInsertar && !PEditar)
                        //    MenuGuardarEnabled = false;
                        //else
                        //    MenuGuardarEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        private async void FiltrarCumplimentados(DateTime? Fecha)
        {
            try
            {
                if (Fecha.HasValue)
                {
                    var aux = Fecha.Value.AddDays(1);
                    ListIncidentesCumplimentar = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListadoIncidentes>>(() => new ObservableCollection<ListadoIncidentes>(new cIncidente().GetData().Where(w => w.ESTATUS == "A" && (w.AUTORIZACION_FEC >= Fecha.Value && w.AUTORIZACION_FEC <= aux)).Select(s => new ListadoIncidentes
                    {
                        Autorizacion_Fecha = s.AUTORIZACION_FEC,
                        Estatus = "AUTORIZADA",
                        Id_Anio = s.ID_ANIO,
                        Id_Centro = s.ID_CENTRO,
                        Id_Imputado = s.ID_IMPUTADO,
                        Id_Incidente = s.ID_INCIDENTE,
                        Id_Incidente_Tipo = s.ID_INCIDENTE_TIPO,
                        Id_Ingreso = s.ID_INGRESO,
                        Materno = s.INGRESO.IMPUTADO.MATERNO,
                        Motivo = s.MOTIVO,
                        Nombre = s.INGRESO.IMPUTADO.NOMBRE,
                        Paterno = s.INGRESO.IMPUTADO.PATERNO,
                        Registro_Fecha = s.REGISTRO_FEC,

                        ImagenImputado = s.INGRESO.INGRESO_BIOMETRICO
                    }).ToList()));
                }
                else
                    ListIncidentesCumplimentar = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListadoIncidentes>>(() => new ObservableCollection<ListadoIncidentes>(new cIncidente().GetData().Where(w => w.ESTATUS == "A").Select(s => new ListadoIncidentes
                    {
                        Autorizacion_Fecha = s.AUTORIZACION_FEC,
                        Estatus = "AUTORIZADA",
                        Id_Anio = s.ID_ANIO,
                        Id_Centro = s.ID_CENTRO,
                        Id_Imputado = s.ID_IMPUTADO,
                        Id_Incidente = s.ID_INCIDENTE,
                        Id_Incidente_Tipo = s.ID_INCIDENTE_TIPO,
                        Id_Ingreso = s.ID_INGRESO,
                        Materno = s.INGRESO.IMPUTADO.MATERNO,
                        Motivo = s.MOTIVO,
                        Nombre = s.INGRESO.IMPUTADO.NOMBRE,
                        Paterno = s.INGRESO.IMPUTADO.PATERNO,
                        Registro_Fecha = s.REGISTRO_FEC,

                        ImagenImputado = s.INGRESO.INGRESO_BIOMETRICO
                    }).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al filtrar cumplimientos", ex);
            }
        }

        private void CambiarEstatus(string Esatus,DateTime? Fecha = null)
        {
            try
            {
                if (SelectIncidentes != null)
                {
                    if (new cIncidente().ActualizarEstatus(new INCIDENTE()
                       {
                           ID_CENTRO = SelectIncidentes.Id_Centro,
                           ID_ANIO = SelectIncidentes.Id_Anio,
                           ID_IMPUTADO = SelectIncidentes.Id_Imputado,
                           ID_INGRESO = SelectIncidentes.Id_Ingreso,
                           ID_INCIDENTE = SelectIncidentes.Id_Incidente,
                           ESTATUS = Esatus,
                           AUTORIZACION_FEC = Fecha
                       }))
                    {
                        GenerarNotificacion(Esatus);
                        if (Esatus == "A")
                        {
                            new Dialogos().ConfirmacionDialogo("Éxito", "La sansión se a autorizado correctamente "); 
                        }
                        else if (Esatus == "C")
                        {
                            new Dialogos().ConfirmacionDialogo("Éxito", "La sansión se a cancelado correctamente "); 
                        }
                        else
                            if (Esatus == "P")
                            {
                                new Dialogos().ConfirmacionDialogo("Éxito", "La sansión se a puesto en pendiente"); 
                            }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al cambiar el estatus de la sanción"); 
                }
                #region comentado
                // new cIncidente().Actualizar(new INCIDENTE
                //{
                //    AUTORIZACION_FEC = null,
                //    ESTATUS = "P",
                //    ID_ANIO = SelectIncidentes.Id_Anio,
                //    ID_CENTRO = SelectIncidentes.Id_Centro,
                //    ID_IMPUTADO = SelectIncidentes.Id_Imputado,
                //    ID_INCIDENTE = SelectIncidentes.Id_Incidente,
                //    ID_INCIDENTE_TIPO = SelectIncidentes.Id_Incidente_Tipo,
                //    ID_INGRESO = SelectIncidentes.Id_Ingreso,
                //    MOTIVO = SelectIncidentes.Motivo,
                //    REGISTRO_FEC = SelectIncidentes.Registro_Fecha
                //});
                #endregion

                if (SelectIncidentesCumplimentar != null)
                {
                    if (new cIncidente().ActualizarEstatus(new INCIDENTE()
                      {
                          ID_CENTRO = SelectIncidentesCumplimentar.Id_Centro,
                          ID_ANIO = SelectIncidentesCumplimentar.Id_Anio,
                          ID_IMPUTADO = SelectIncidentesCumplimentar.Id_Imputado,
                          ID_INGRESO = SelectIncidentesCumplimentar.Id_Ingreso,
                          ID_INCIDENTE = SelectIncidentesCumplimentar.Id_Incidente,
                          ESTATUS = Esatus,
                          AUTORIZACION_FEC = Fecha
                      })) 
                    {
                          GenerarNotificacion(Esatus);
                          if (Esatus == "A")
                          { 
                              new Dialogos().ConfirmacionDialogo("Éxito", "La sansión se a autorizado correctamente "); 
                          }
                          else if (Esatus == "C")
                              new Dialogos().ConfirmacionDialogo("Éxito", "La sansión se a cancelado correctamente ");
                          else
                              if (Esatus == "P")
                                  new Dialogos().ConfirmacionDialogo("Éxito", "La sansión se a puesto en pendiente"); 
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al cambiar el estatus de la sanción"); 
                }
                #region comentado
                //new cIncidente().Actualizar(new INCIDENTE
                //{
                //    AUTORIZACION_FEC = null,
                //    ESTATUS = "P",
                //    ID_ANIO = SelectIncidentesCumplimentar.Id_Anio,
                //    ID_CENTRO = SelectIncidentesCumplimentar.Id_Centro,
                //    ID_IMPUTADO = SelectIncidentesCumplimentar.Id_Imputado,
                //    ID_INCIDENTE = SelectIncidentesCumplimentar.Id_Incidente,
                //    ID_INCIDENTE_TIPO = SelectIncidentesCumplimentar.Id_Incidente_Tipo,
                //    ID_INGRESO = SelectIncidentesCumplimentar.Id_Ingreso,
                //    MOTIVO = SelectIncidentesCumplimentar.Motivo,
                //    REGISTRO_FEC = SelectIncidentesCumplimentar.Registro_Fecha
                //});
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al actualizar estatus", ex);
            }
        }

        #region Reporte
        private void ImprimirActa()
        {
            try
            {
                if (SelectIncidentesCumplimentar == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una sanción");
                    return;
                }
          
                var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.ACTA_CONSEJO_TECNICO_INTERDICIPLINARIO);
                if (documento == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la plantilla del documento");
                    return;
                }

                using (var ms = new MemoryStream())
                {
                    ms.Write(documento.DOCUMENTO, 0, documento.DOCUMENTO.Length);
                    using (DocX doc = DocX.Load(ms))
                    {
                        var fecha = SelectIncidentesCumplimentar.Autorizacion_Fecha;
                        doc.ReplaceText("{hora}", fecha.Value.Hour + ":" + fecha.Value.Minute);
                        doc.ReplaceText("{dia}", fecha.Value.Day.ToString());
                        CultureInfo cultura = new CultureInfo("es-MX");
                        doc.ReplaceText("{mes}", cultura.DateTimeFormat.GetMonthName(fecha.Value.Month));
                        doc.ReplaceText("{anio}", fecha.Value.Year.ToString());

                        var incidente = new cIncidente().ObtenerTodas(
                            SelectIncidentesCumplimentar.Id_Centro,
                            SelectIncidentesCumplimentar.Id_Anio,
                            SelectIncidentesCumplimentar.Id_Imputado,
                            SelectIncidentesCumplimentar.Id_Ingreso,
                            SelectIncidentesCumplimentar.Id_Incidente
                            ).FirstOrDefault();
                        if (incidente != null)
                        {
                            var interno = incidente.INGRESO.IMPUTADO;
                            doc.ReplaceText("{interno}", string.Format("{0} {1} {2}", interno.NOMBRE.Trim(),
                                !string.IsNullOrEmpty(interno.PATERNO) ? interno.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(interno.MATERNO) ? interno.MATERNO.Trim() : string.Empty));
                            if (incidente.SANCION != null)
                            {
                                var fmenor = incidente.SANCION.OrderBy(w => w.INICIA_FEC).FirstOrDefault();
                                var fmayor = incidente.SANCION.OrderByDescending(w => w.TERMINA_FEC).FirstOrDefault();
                                if (fmenor != null && fmayor != null)
                                {
                                    doc.ReplaceText("{fecha_inicio}", string.Format("{0:dd/MM/yyyy}", fmenor.INICIA_FEC));
                                    doc.ReplaceText("{fecha_fin}", string.Format("{0:dd/MM/yyyy}", fmayor.TERMINA_FEC));
                                    var t = (fmayor.TERMINA_FEC.Value.Date - fmenor.INICIA_FEC.Value.Date).TotalDays;
                                    doc.ReplaceText("{días_sancion}", t.ToString());
                                }
                                foreach (var s in incidente.SANCION)
                                {
                                    switch (s.ID_SANCION)
                                    {
                                        case 2:
                                            doc.ReplaceText("{1}", "X");
                                            doc.ReplaceText("{2}", "X");
                                            break;
                                        case 3:
                                            doc.ReplaceText("{3}", "X");
                                            break;
                                        case 4:
                                            doc.ReplaceText("{4}", "X");
                                            break;
                                        case 5:
                                            doc.ReplaceText("{5}", "X");
                                            break;
                                        case 9:
                                            doc.ReplaceText("{7}", "X");
                                            break;
                                        case 10:
                                            doc.ReplaceText("{8}", "X");
                                            break;
                                        case 11:
                                            doc.ReplaceText("{9}", "X");
                                            break;
                                        case 12:
                                            doc.ReplaceText("{10}", "X");
                                            doc.ReplaceText("{11}", "X");
                                            break;
                                    }
                                }
                            }
                        }
                        doc.ReplaceText("{1}", " ");
                        doc.ReplaceText("{2}", " ");
                        doc.ReplaceText("{3}", " ");
                        doc.ReplaceText("{4}", " ");
                        doc.ReplaceText("{5}", " ");
                        doc.ReplaceText("{6}", " ");
                        doc.ReplaceText("{7}", " ");
                        doc.ReplaceText("{8}", " ");
                        doc.ReplaceText("{9}", " ");
                        doc.ReplaceText("{10}", " ");
                        doc.ReplaceText("{11}", " ");
                        doc.ReplaceText("{12}", " ");



                        doc.ReplaceText("{días_sancion}", " ");
                        doc.ReplaceText("{fecha_inicio}", " ");
                        doc.ReplaceText("{fecha_fin}", " ");

                        doc.Save();

                    }

                    var tc = new TextControlView();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.editor.Loaded += (s, e) =>
                    {
                        try
                        {
                            tc.editor.Load(ms.ToArray(), TXTextControl.BinaryStreamType.WordprocessingML);
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                        }
                    };
                    tc.Owner = PopUpsViewModels.MainWindow;
                    tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Show();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos", ex);
            }
        }

        private void ImprimirCitatorioInterno()
        {
            try
            {
                if (SelectIncidentesCumplimentar == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una sanción");
                    return;
                }

                var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.CITATORIO_INTERNO);
                if (documento == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la plantilla del documento");
                    return;
                }

                if(documento.DOCUMENTO == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la plantilla del documento");
                    return;
                }

                var diccionario = new Dictionary<string, string>();
                diccionario.Add("<<encabezado1>>", Parametro.ENCABEZADO1);
                diccionario.Add("<<encabezado2>>", Parametro.ENCABEZADO2);
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                diccionario.Add("<<centro>>", centro.DESCR.Trim());
                var hoy = Fechas.GetFechaDateServer;
                diccionario.Add("<<horas>>", hoy.ToString("hh:mm"));
                diccionario.Add("<<dia>>", hoy.Day.ToString());
                CultureInfo cultura = new CultureInfo("es-MX");
                diccionario.Add("<<mes>>", cultura.DateTimeFormat.GetMonthName(hoy.Month));
                diccionario.Add("<<anio>>", hoy.Year.ToString());
                diccionario.Add("<<interno>>", string.Format("{0} {1} {2}",
                    SelectIncidentesCumplimentar.Nombre.Trim(),
                    !string.IsNullOrEmpty(SelectIncidentesCumplimentar.Paterno) ? SelectIncidentesCumplimentar.Paterno.Trim() : string.Empty,
                    !string.IsNullOrEmpty(SelectIncidentesCumplimentar.Materno) ? SelectIncidentesCumplimentar.Materno.Trim() : string.Empty));
                diccionario.Add("<<motivo>>", SelectIncidentesCumplimentar.Motivo.Trim());
                diccionario.Add("<<fracciones>>", "   ");
                diccionario.Add("<<firma_policia>>", "   ");
                diccionario.Add("<<signado>>", "   ");
                var docto = new cWord().FillFieldsDocx(documento.DOCUMENTO, diccionario);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        switch(documento.ID_FORMATO)
                        {
                            case 1://DOCX
                                tc.editor.Load(docto, TXTextControl.BinaryStreamType.WordprocessingML);
                                break;
                            case 3://PDF
                                tc.editor.Load(docto, TXTextControl.BinaryStreamType.AdobePDF);
                                break;
                            case 4://DOCX
                                tc.editor.Load(docto, TXTextControl.BinaryStreamType.WordprocessingML);
                                break;
                            default:
                                new Dialogos().ConfirmacionDialogo("Notificación!", "Formato de archivo no válido");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos", ex);
            }
        }

        #endregion

        #region Mensajes
        private void GenerarNotificacion(string Tipo) {
            try
            {
                var mt = new cTipoMensaje().Obtener((short)enumMensajeTipo.SANCION).FirstOrDefault();
                if (mt != null)
                {
                    string contenido = string.Empty;
                    var obj = new MENSAJE();
                    obj.ID_MEN_TIPO = mt.ID_MEN_TIPO;
                    if (Tipo == "A")
                    {
                        obj.ENCABEZADO = mt.ENCABEZADO;
                        contenido = "SE HA AUTORIZADO LA SANCIÓN";
                    }
                    else
                        if (Tipo == "C")
                        {
                            obj.ENCABEZADO = "SE HA CANCELADO LA SANCIÓN";
                            contenido = "SE HA CANCELADO LA SANCIÓN";
                        }
                    else
                            if (Tipo == "P")
                            {
                                obj.ENCABEZADO = "SE HA PUESTO EN PENDIENTE LA SANCIÓN";
                                contenido = "SE HA PUESTO EN PENDIENTE LA SANCIÓN";
                            }
                    
                    if (SelectIncidentes != null)
                    {
                        var i = new cImputado().Obtener(SelectIncidentes.Id_Imputado, SelectIncidentes.Id_Anio, SelectIncidentes.Id_Centro).FirstOrDefault();
                        if (i != null)
                        {
                            contenido += string.Format("DEL INTERNO {0}/{1} {2} {3} {4}",
                                SelectIncidentes.Id_Anio,
                                SelectIncidentes.Id_Imputado,
                                i.NOMBRE.Trim(),
                                !string.IsNullOrEmpty(i.PATERNO) ? i.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(i.MATERNO) ? i.MATERNO.Trim() : string.Empty);
                        }
                    }
                    obj.CONTENIDO = contenido;
                    obj.REGISTRO_FEC = Fechas.GetFechaDateServer;
                    obj.ID_CENTRO = GlobalVar.gCentro;
                    new cMensaje().Insertar(obj);
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación","No se envio notificación");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar notificación.", ex);
            }
        }
        #endregion
    }
}
