using ControlPenales.BiometricoServiceReference;
using LinqKit;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.Objects;

namespace ControlPenales
{
    partial class CreacionGruposViewModel : ValidationViewModelBase
    {
        /* [descripcion de clase]
         * modulo de creacion de grupos
         * clase importante****
         * 
         * se debe de notificar cuando haya empalmes
         * 
         * [modelo]
         * aqui se crean los grupos en los cuales seran integrados los imputados segun las especificaciones del tratamiento en el EMI, estar muy al pendiente de los estatus y de las fechas
         * pudiera ocacionar problema el revisar la disponibilidad de areas y responsables
         * 
         * [complementario]
         * aqui se crean los grupos en los cuales se iran abriendo conforme la marcha y se enlisata todoa la poblacion, esta al pendiente de los estatus y de las fechas
         * pudiera ocacionar problema el listar a toda la poblacion
         * 
         */

        /// <summary>
        /// constructor de la clase
        /// </summary>
        public CreacionGruposViewModel() { }
        /// <summary>
        /// constructor de la clase que selecciona el tab donde se quedo
        /// </summary>
        /// <param name="TabSelected">valor del tab seleccionado</param>
        public CreacionGruposViewModel(bool TabSelected) { SelectedTabComplementario = TabSelected; }

        ///TODO: ingresar el usuario para el filtro de responsable... TODO!!

        #region [PERMISOS]
        ///SE CREA ESTRUCTURA DE PERMISOS EN BASE A CREACION DE ROLES
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CREACION_GRUPOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                    {
                        AgregarMenuEnabled = true;
                        EjeEnabled = true;
                        ProgramaEnabled = true;
                        ActividadEnabled = true;
                        TabComplementarioEnabled = true;
                    }
                    //if (p.CONSULTAR == 1)
                    //{
                    //    BuscarHabilitado = true;
                    //    TextoHabilitado = true;
                    //}
                    if (p.EDITAR == 1)
                        EditarMenuEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }

        //private void ConfiguraPermisosGruposComplementarios()
        //{
        //    try
        //    {
        //        var permisos = new cProcesoUsuario().Obtener(enumProcesos.CREACION_GRUPOS_COMPLEMENTARIOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
        //        if (permisos.Any())
        //        {
        //            foreach (var p in permisos)
        //            {
        //                if (p.INSERTAR == 1)
        //                    PInsertar = true;
        //                if (p.EDITAR == 1)
        //                    PEditar = true;
        //                if (p.CONSULTAR == 1)
        //                    PConsultar = true;
        //                if (p.IMPRIMIR == 1)
        //                    PImprimir = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
        //    }
        //}
        #endregion

        #region [GRUPO]
        /// <summary>
        /// metodo para cargar informacion al entrar al modulo
        /// </summary>
        /// <param name="obj">usercontrol del modulo</param>
        async void CreacionGruposLoad(CreacionGruposView obj)
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }

                #region Departamentos por usuario
                departamentosUsuarios = new List<int>();
                var roles = new cUsuarioRol().ObtenerTodos(GlobalVar.gUsr);
                if (roles != null)
                {
                    foreach (var r in roles)
                    {
                        if (r.SISTEMA_ROL.DEPARTAMENTO != null)
                        {
                            foreach (var d in r.SISTEMA_ROL.DEPARTAMENTO)
                            {
                                departamentosUsuarios.Add(d.ID_DEPARTAMENTO);
                            }
                        }
                    }
                }
                #endregion
                //obtener control tabs
                Tab = obj.TabCreacionGrupo;
                //instanciamos el evento selectionchanged
                obj.TabCreacionGrupo.SelectionChanged += TabCreacionGrupo_SelectionChanged;
                //cargamos combobox de ejes
                ListEjes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EJE>>(() => new ObservableCollection<EJE>(new cGrupoParticipante().GetData().Where(w => w.ESTATUS == 1 && w.EJE1.COMPLEMENTARIO == "N").Select(s => s.EJE1).Distinct().OrderBy(o => o.ORDEN)));
                //cargamos combobox de responsables
                ListResponsable = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<NombreEmpleado>>(() => new ObservableCollection<NombreEmpleado>(new cPersona().GetData().Where(w => w.ID_TIPO_PERSONA == 1).Select(s => new NombreEmpleado
                {
                    ID_PERSONA = s.ID_PERSONA,
                    NOMBRE_COMPLETO = s.PATERNO.Trim() + " " + s.MATERNO.Trim() + " " + s.NOMBRE.Trim()
                }).OrderBy(o => o.NOMBRE_COMPLETO)));
                //cargamos combo de areas
                ListArea = ListArea ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<AREA>>(() => new ObservableCollection<AREA>(new cArea().GetData().Where(w => w.ID_TIPO_AREA != 5)));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar modulo de creacion de programas", ex);
            }
            ConfiguraPermisos();
        }
        /// <summary>
        /// cargamos los programas segun el eje seleccionado
        /// </summary>
        /// <param name="Id_eje">id del eje seleccionado</param>
        async void ProgramasLoad(short Id_eje)
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }
                ListProgramas = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TIPO_PROGRAMA>>(() => new ObservableCollection<TIPO_PROGRAMA>(new cGrupoParticipante().GetData().Where(w => w.ESTATUS == 1 && w.EJE == Id_eje && departamentosUsuarios.Contains(w.ACTIVIDAD.TIPO_PROGRAMA.ID_DEPARTAMENTO.Value)).Select(s => s.ACTIVIDAD.TIPO_PROGRAMA).Distinct().OrderBy(o => o.NOMBRE).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar programas", ex);
            }
        }
        /// <summary>
        /// cargamos las actividades segun el programa seleccionado
        /// </summary>
        /// <param name="Id_tipo_programa">id del programa seleccionado</param>
        async void ActividadesLoad(short Id_tipo_programa)
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }
                ListActividades = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ACTIVIDAD>>(() => new ObservableCollection<ACTIVIDAD>(new cGrupoParticipante().GetData().Where(w => (w.ESTATUS == 1 || w.ESTATUS == 2) && w.EJE == SelectedEje && w.ID_TIPO_PROGRAMA == Id_tipo_programa).Select(s => s.ACTIVIDAD).Distinct().OrderBy(o => o.DESCR).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar actividades", ex);
            }
        }
        /// <summary>
        /// cargamos a los internos que se seleccionaron para integrarse a esta actividad
        /// </summary>
        /// <param name="Id_actividad">id de la actividad seleccionada</param>
        async void ListaInternosLoad(short Id_actividad)
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }
                SelectedCount = 0;
                selectedCountTrue = false;
                isOCUPANTE_MAX = false;
                IsOCUPANTE_MIN = false;
                EnabledListaHorario = false;

                ///carga asincrona de los imputados
                ListGrupoParticipante = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListaInternos>>(() =>
                {
                    var listainternos = new ObservableCollection<ListaInternos>();
                    try
                    {
                        var _fecha_server = Fechas.GetFechaDateServer;
                        ListGrupo = new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_EJE == SelectedEje && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == Id_actividad && (w.ID_ESTATUS_GRUPO == 1 || w.ID_ESTATUS_GRUPO == 4)).Select(s => s).Distinct().OrderBy(o => o.DESCR).ToList());
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;

                        listainternos = new ObservableCollection<ListaInternos>(new cGrupoParticipante().GetData().
                            Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_GRUPO == null && (w.ESTATUS == 1 || w.ESTATUS == 3) && w.EJE == SelectedEje
                                && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == Id_actividad && !EstatusInactivos.Contains(w.INGRESO.ID_ESTATUS_ADMINISTRATIVO)
                                && !w.INGRESO.TRASLADO_DETALLE.Any(wh => (wh.ID_ESTATUS != "CA" ? wh.TRASLADO.ORIGEN_TIPO != "F" : false)) &&
                                //Valida si el interno se encuentra en el centro
                        w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro)

                                .OrderBy(o => o.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(o => o.ACTIVIDAD.ORDEN).ToList().Select(s => new ListaInternos
                               {
                                   Entity = s,
                                   NOMBRE = s.INGRESO.IMPUTADO.NOMBRE != null ? s.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                   PATERNO = s.INGRESO.IMPUTADO.PATERNO != null ? s.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                                   MATERNO = s.INGRESO.IMPUTADO.MATERNO != null ? s.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty,
                                   NOMBRECOMPLETO = (s.INGRESO.IMPUTADO.NOMBRE != null ? s.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty) + " "
                                                  + (s.INGRESO.IMPUTADO.PATERNO != null ? s.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty) + " "
                                                  + (s.INGRESO.IMPUTADO.MATERNO != null ? s.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty),
                                   ImageSource = s.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault() != null ? s.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson(),
                                   FOLIO = s.ING_ID_ANIO + "\\" + s.ING_ID_IMPUTADO,
                                   UBICACION = s.INGRESO.CAMA != null ? s.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + s.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() + s.INGRESO.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(s.INGRESO.CAMA.DESCR) ? s.INGRESO.CAMA.ID_CAMA.ToString().Trim() : s.INGRESO.CAMA.ID_CAMA + " " + s.INGRESO.CAMA.DESCR.Trim()) : string.Empty,
                                   PLANIMETRIA = s.INGRESO.CAMA != null ? s.INGRESO.CAMA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                   PLANIMETRIACOLOR = s.INGRESO.CAMA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.COLOR : string.Empty : string.Empty : string.Empty : string.Empty,
                                   RESTANTE = string.IsNullOrEmpty(CalcularSentencia(s.INGRESO.CAUSA_PENAL)) ? string.Empty : CalcularSentencia(s.INGRESO.CAUSA_PENAL).Replace('_', ' '),
                                   SENTENCIA = varauxSentencia,
                                   RESTANTEsplit = string.IsNullOrEmpty(CalcularSentencia(s.INGRESO.CAUSA_PENAL)) ? "999_Años_999_Meses_999_Dias" : CalcularSentencia(s.INGRESO.CAUSA_PENAL),
                                   ShowEmpalme = new Thickness(0)
                               })
                               .OrderBy(o => Convert.ToInt32(o.RESTANTEsplit.Split('_')[0]))
                               .ThenBy(o => Convert.ToInt32(o.RESTANTEsplit.Split('_')[2]))
                               .ThenBy(o => Convert.ToInt32(o.RESTANTEsplit.Split('_')[4]))
                               .ToList());
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear listado", ex);
                    }
                    return listainternos;
                });
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listado", ex);
            }
        }
        /// <summary>
        /// cargamos a los internos que se seleccionaron para integrarse a esta actividad y a los que pertenecen al grupo seleccionado
        /// </summary>
        /// <param name="Id_grupo">id del grupo seleccionado</param>
        async void ListaInternosUpdate(short Id_grupo)
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }
                SelectedCount = 0;
                selectedCountTrue = false;
                isOCUPANTE_MAX = false;
                IsOCUPANTE_MIN = false;
                var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                ListGrupoParticipante = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListaInternos>>(() =>
                {
                    var listainternos = new ObservableCollection<ListaInternos>();
                    try
                    {
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        listainternos = new ObservableCollection<ListaInternos>(new cGrupoParticipante().GetData()
                            .Where(w => w.ID_CENTRO == GlobalVar.gCentro && (w.ID_GRUPO == null || w.ID_GRUPO == Id_grupo) && (w.ESTATUS == 1 || w.ESTATUS == 2 || w.ESTATUS == 3)
                                && w.EJE == SelectedEje && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == SelectedActividad
                                && !EstatusInactivos.Contains(w.INGRESO.ID_ESTATUS_ADMINISTRATIVO)
                                && !w.INGRESO.TRASLADO_DETALLE.Any(wh => (wh.ID_ESTATUS != "CA" ? wh.TRASLADO.ORIGEN_TIPO != "F" : false))
                                && (w.GRUPO_PARTICIPANTE_CANCELADO.Where(wh => (wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD
                                    && wh.ID_CONSEC == w.ID_CONSEC && wh.ID_GRUPO == w.ID_GRUPO)).Any() ?
                            w.GRUPO_PARTICIPANTE_CANCELADO.Where(wh => (wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_CONSEC == w.ID_CONSEC) &&
                                (wh.RESPUESTA_FEC == null ?

                                (wh.RESPUESTA_FEC == null && wh.ESTATUS == 0) :

                                (w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC) && whe.RESPUESTA_FEC != null && (whe.ESTATUS == 0 || whe.ESTATUS == 2)).Count() == (w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC)).Count() - w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC) && whe.RESPUESTA_FEC != null && whe.ESTATUS == 1 && whe.ID_ESTATUS == 3).Count())))).Any()


                                : true))
                            .OrderBy(o => o.ACTIVIDAD.TIPO_PROGRAMA.ORDEN)
                            .ThenBy(o => o.ACTIVIDAD.ORDEN)
                            .ToList().Select(s => new ListaInternos
                                {
                                    elegido = s.ID_GRUPO.HasValue,
                                    Entity = s,
                                    NOMBRE = s.INGRESO.IMPUTADO.NOMBRE.Trim(),
                                    PATERNO = s.INGRESO.IMPUTADO.PATERNO.Trim(),
                                    MATERNO = s.INGRESO.IMPUTADO.MATERNO.Trim(),
                                    NOMBRECOMPLETO = s.INGRESO.IMPUTADO.NOMBRE.Trim() + " "
                                                   + s.INGRESO.IMPUTADO.PATERNO.Trim() + " "
                                                   + s.INGRESO.IMPUTADO.MATERNO.Trim(),
                                    ImageSource = s.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault() != null ? s.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson(),
                                    FOLIO = s.ING_ID_ANIO + "\\" + s.ING_ID_IMPUTADO,
                                    UBICACION = s.INGRESO.CAMA != null ? s.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + s.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() + s.INGRESO.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(s.INGRESO.CAMA.DESCR) ? s.INGRESO.CAMA.ID_CAMA.ToString().Trim() : s.INGRESO.CAMA.ID_CAMA + " " + s.INGRESO.CAMA.DESCR.Trim()) : string.Empty,
                                    PLANIMETRIA = s.INGRESO.CAMA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty,
                                    PLANIMETRIACOLOR = s.INGRESO.CAMA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.COLOR : string.Empty : string.Empty : string.Empty : string.Empty,
                                    RESTANTE = string.IsNullOrEmpty(CalcularSentencia(s.INGRESO.CAUSA_PENAL)) ? string.Empty : CalcularSentencia(s.INGRESO.CAUSA_PENAL).Replace('_', ' '),
                                    RESTANTEsplit = string.IsNullOrEmpty(CalcularSentencia(s.INGRESO.CAUSA_PENAL)) ? "999_Años_999_Meses_999_Dias" : CalcularSentencia(s.INGRESO.CAUSA_PENAL),
                                    ShowEmpalme = new Thickness(0)
                                })
                            .OrderByDescending(o => o.elegido)
                            .ThenBy(o => Convert.ToInt32(o.RESTANTEsplit.Split('_')[0]))
                            .ThenBy(o => Convert.ToInt32(o.RESTANTEsplit.Split('_')[2]))
                            .ThenBy(o => Convert.ToInt32(o.RESTANTEsplit.Split('_')[4]))
                            .ToList());

                        SelectedCount = new cGrupoParticipante().GetData().Where(w => w.ID_GRUPO == SelectedGrupo.ID_GRUPO).Count();
                        CalcularInternosSeleccionados();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear listado", ex);
                    }
                    return listainternos;
                });
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listado", ex);
            }
        }
        /// <summary>
        /// genera lista de las actividades programadas para el imputado seleccionado
        /// </summary>
        /// <param name="iNGRESO">ingreso activo del imputado</param>
        /// <returns>listado con el horario del interno</returns>
        private List<ListaActividad> GenerarListaActividades(INGRESO iNGRESO)
        {
            var Actividades = new List<ListaActividad>();
            try
            {
                var entity = new cGrupoParticipante().GetData().Where(w => w.ID_CENTRO == iNGRESO.ID_UB_CENTRO && w.ING_ID_ANIO == iNGRESO.ID_ANIO && w.ING_ID_IMPUTADO == iNGRESO.ID_IMPUTADO && w.ING_ID_INGRESO == iNGRESO.ID_INGRESO).ToList();
                foreach (var item in entity.Where(w => w.ID_GRUPO != null).Select(s => s.GRUPO))
                {
                    IsEmpalme = new List<bool>();
                    if (!(item.ID_ACTIVIDAD == SelectedActividad && item.ID_TIPO_PROGRAMA == SelectedPrograma && item.ID_EJE == SelectedEje.Value))
                        Actividades.Add(new ListaActividad()
                        {
                            NombreEje = item.ACTIVIDAD_EJE.EJE.DESCR,
                            NombreGrupo = item.DESCR,
                            NombreActividad = item.ACTIVIDAD.DESCR,
                            RecurrenciaActividad = item.RECURRENCIA,
                            InicioActividad = item.FEC_INICIO.Value.ToShortDateString(),
                            FinActividad = item.FEC_FIN.Value.ToShortDateString(),
                            ListHorario = ValidacionHorarioImputado(item.GRUPO_HORARIO),
                            orden = item.ACTIVIDAD.PRIORIDAD,
                            State = IsEmpalme.Where(w => w).Any() ? "Empalme" : string.Empty,
                            Revision = false
                        });
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listado de actividades", ex);
            }
            return Actividades.OrderBy(o => o.orden).ToList();
        }
        /// <summary>
        /// validacion de horario del imputado para empalme
        /// </summary>
        /// <param name="collection">horario del grupo</param>
        /// <returns>lista de horas empalmadas</returns>
        private ObservableCollection<ListHorario> ValidacionHorarioImputado(ICollection<GRUPO_HORARIO> collection)
        {
            var listavalidadohorarioimputado = new ObservableCollection<ListHorario>();
            try
            {
                listavalidadohorarioimputado = new ObservableCollection<ListHorario>(collection.Where(w => (w.HORA_INICIO.Value.Date >= Fechas.GetFechaDateServer.Date) && w.ESTATUS == 1 && w.GRUPO.ID_ESTATUS_GRUPO == 1).OrderBy(o => o.HORA_INICIO).Select(s => new ListHorario()
                  {
                      GrupoHorarioEntity = s,
                      AREADESCR = s.AREA.DESCR,
                      GRUPO_HORARIO_ESTATUSDESCR = s.GRUPO_HORARIO_ESTATUS.DESCR,
                      DESCRDIA = s.HORA_INICIO.Value.ToLongDateString(),
                      strHORA_INICIO = s.HORA_INICIO.Value.ToShortTimeString(),
                      strHORA_TERMIINO = s.HORA_TERMINO.Value.ToShortTimeString(),
                      State = ValidateEmpalme(s)
                  }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar horario imputado", ex);
            }
            return listavalidadohorarioimputado;
        }
        /// <summary>
        /// metodo para marcar empalme el registro
        /// </summary>
        /// <param name="s">horario a revisar</param>
        /// <returns>estado de si esta empalmado o no</returns>
        private string ValidateEmpalme(GRUPO_HORARIO s)
        {
            try
            {
                if (PanelUpdate == Visibility.Collapsed)
                {
                    if (SelectedInterno != null)
                        if (!SelectedInterno.Entity.ID_GRUPO.HasValue)
                            if (ListGrupoHorario.Where(w => (w.HORA_INICIO >= s.HORA_INICIO && w.HORA_TERMINO <= s.HORA_TERMINO) && w.ESTATUS == 1 && w.GRUPO.ID_ESTATUS_GRUPO == 1).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                    IsEmpalme.Add(false);
                    return string.Empty;
                }
                else
                {
                    #region [NUEVO]
                    if (!FechaFin.HasValue)
                        return string.Empty;

                    var FECHA_INICIO = new DateTime();
                    var FECHA_FIN = new DateTime();
                    var DATE_END = FechaFin.Value.AddDays(1);

                    switch (s.HORA_INICIO.Value.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            if (!InicioDiaDomingo.HasValue || !FinDiaDomingo.HasValue)
                            {
                                IsEmpalme.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaDomingo.Value.Hour, InicioDiaDomingo.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaDomingo.Value.Hour, FinDiaDomingo.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Monday:
                            if (!InicioDiaLunes.HasValue || !FinDiaLunes.HasValue)
                            {
                                IsEmpalme.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaLunes.Value.Hour, InicioDiaLunes.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaLunes.Value.Hour, FinDiaLunes.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Tuesday:
                            if (!InicioDiaMartes.HasValue || !FinDiaMartes.HasValue)
                            {
                                IsEmpalme.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaMartes.Value.Hour, InicioDiaMartes.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaMartes.Value.Hour, FinDiaMartes.Value.Minute, 0);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Wednesday:
                            if (!InicioDiaMiercoles.HasValue || !FinDiaMiercoles.HasValue)
                            {
                                IsEmpalme.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaMiercoles.Value.Hour, InicioDiaMiercoles.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaMiercoles.Value.Hour, FinDiaMiercoles.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Thursday:
                            if (!InicioDiaJueves.HasValue || !FinDiaJueves.HasValue)
                            {
                                IsEmpalme.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaJueves.Value.Hour, InicioDiaJueves.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaJueves.Value.Hour, FinDiaJueves.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Friday:
                            if (!InicioDiaViernes.HasValue || !FinDiaViernes.HasValue)
                            {
                                IsEmpalme.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaViernes.Value.Hour, InicioDiaViernes.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaViernes.Value.Hour, FinDiaViernes.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Saturday:
                            if (!InicioDiaSabado.HasValue || !FinDiaSabado.HasValue)
                            {
                                IsEmpalme.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaSabado.Value.Hour, InicioDiaSabado.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaSabado.Value.Hour, FinDiaSabado.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        default:
                            break;
                    }

                    IsEmpalme.Add(false);
                    return string.Empty;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar horario empalmado", ex);
            }
            return string.Empty;
        }
        /// <summary>
        /// metodo que ejecuta el evento click del checkbox del listado de horarios empalmados
        /// </summary>
        /// <param name="SelectedItem">arreglo de dos objetos [0]objeto listado interno, [1]checkbox</param>
        /// <param name="p">marcar o desmarcar casilla</param>
        private async void SetIsSelectedProperty(object SelectedItem, bool p)
        {
            try
            {
                if (selectedCountTrue)
                    return;

                if (((CheckBox)(((object[])(SelectedItem))[1])).IsChecked.Value)
                {
                    if (isOCUPANTE_MAX)
                    {
                        selectedCountTrue = true;
                        ((CheckBox)(((object[])(SelectedItem))[1])).IsChecked = false;
                        selectedCountTrue = false;
                        return;
                    }
                    (((System.Windows.Controls.ListBoxItem)(((System.Windows.FrameworkElement)(((System.Windows.FrameworkElement)(((System.Windows.FrameworkElement)(((object[])(SelectedItem))[1])).TemplatedParent)).Parent)).TemplatedParent))).IsSelected = true;

                    if (ListActividadParticipante.Where(w => w.State.Equals("Empalme")).Any() && ListActividadParticipante.Where(w => w.Revision).Count() <= 0)
                    {
                        ((CheckBox)(((object[])(SelectedItem))[1])).IsChecked = false;

                        SelectedCount = p ? (SelectedCount + 1) : (SelectedCount - 1);
                        CalcularInternosSeleccionados();
                        ((ListaInternos)(((object[])(SelectedItem))[0])).elegido = false;
                        return;
                    }
                }

                var result = -1;
                if (((ListaInternos)(((object[])(SelectedItem))[0])).Entity.GRUPO != null)
                    result = await OpcionCancelarSuspenderInterno(((ListaInternos)(((object[])(SelectedItem))[0])).Entity, ((ListaInternos)(((object[])(SelectedItem))[0])));

                if (result != 0)
                {
                    SelectedCount = p ? (SelectedCount + 1) : (SelectedCount - 1);
                    CalcularInternosSeleccionados();
                    ((ListaInternos)(((object[])(SelectedItem))[0])).elegido = p;
                    if (((ListaInternos)(((object[])(SelectedItem))[0])).Entity.GRUPO == null)
                        StaticSourcesViewModel.SourceChanged = true;
                }
                else
                {
                    selectedCountTrue = true;
                    ((CheckBox)(((object[])(SelectedItem))[1])).IsChecked = true;
                    selectedCountTrue = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
            }
        }
        /// <summary>
        /// metodo que muestra mensaje al querer desmarcar un interno que ya pertenece al grupo seleccionado
        /// </summary>
        /// <param name="gRUPO_PARTICIPANTE">objeto grupo participante del interno seleccionado</param>
        /// <param name="item">objeto de lista internos</param>
        /// <returns>regresa [0] sin accion, [1]cancelar interno, [2]suspender interno</returns>
        private async Task<int> OpcionCancelarSuspenderInterno(GRUPO_PARTICIPANTE gRUPO_PARTICIPANTE, ListaInternos item)
        {
            var result = 0;
            try
            {
                result = await (new Dialogos()).ConfirmacionTresBotonesDinamico("Creación de Grupos", "Que Desea Hacer?, Cancelar o Suspender al Interno", "Cancelar", 0, "Cancelar Interno", 1, "Suspender Interno", 2);
                if (result == 0)
                    return 0;

                MotivoText = string.Empty;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.MOTIVO);
                isMotivoShow = true;
                var respuesta = false;

                await Task.Factory.StartNew(() => { do { } while (isMotivoShow); });
                if (isGuardar)
                {
                    respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando Estatus Del Interno", () =>
                    {
                        try
                        {
                            new cGrupoParticipanteCancelado().InsertarParticipanteCancelado(new GRUPO_PARTICIPANTE_CANCELADO()
                            {
                                ID_CENTRO = gRUPO_PARTICIPANTE.ID_CENTRO,
                                ID_ACTIVIDAD = gRUPO_PARTICIPANTE.ID_ACTIVIDAD,
                                ID_TIPO_PROGRAMA = gRUPO_PARTICIPANTE.ID_TIPO_PROGRAMA,
                                ID_CONSEC = gRUPO_PARTICIPANTE.ID_CONSEC,
                                ID_GRUPO = gRUPO_PARTICIPANTE.ID_GRUPO.Value,
                                ID_USUARIO = GlobalVariables.gUser,
                                SOLICITUD_FEC = Fechas.GetFechaDateServer,
                                RESPUESTA_FEC = null,
                                MOTIVO = MotivoText,
                                //4 Suspendido, 3 Cancelado
                                ID_ESTATUS = result == 1 ? (short)3 : (short)4
                            });
                            return true;
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al modificar el estatu del integrante", ex);
                            return false;
                        }
                    });

                    if (!respuesta)
                        result = 0;
                    if (respuesta)
                    {
                        ListGrupoParticipante.Remove(item);
                        CalcularInternosSeleccionados();
                        OnPropertyChanged("ListGrupoParticipante");
                        await new Dialogos().ConfirmacionDialogoReturn("Edición Grupo", "El Cambio de Estatus Se Actualizo Exitosamente");
                    }
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// metodo para revisar listado
        /// </summary>
        /// <param name="obj">control listbox</param>
        private void RevisarListadoRevision(object obj)
        {
            try
            {
                if (!(obj is ListBox))
                    return;

                ListGrupoParticipante.Where(w => w == SelectedInterno).FirstOrDefault().elegido = ListActividadParticipante.Where(w => w.Revision).Any();
                ((ListBox)obj).ItemsSource = new ObservableCollection<ListaInternos>(ListGrupoParticipante);
                SelectedCount = ListGrupoParticipante.Where(w => w.elegido).Count();
                CalcularInternosSeleccionados();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al revisar listado", ex);
            }
        }
        /// <summary>
        /// validar horario del grupo
        /// </summary>
        /// <returns></returns>
        bool ValidateHorarioListaInterno()
        {
            try
            {
                EnabledListaHorario = false;
                IsEnabledCrearGrupo = false;
                SelectedInterno = null;

                if (PanelUpdate == Visibility.Collapsed)
                    if (!string.IsNullOrEmpty(GrupoDescripcion))
                        if (SelectedResponsable.HasValue)
                        {
                            EnabledListaHorario = true;
                            IsEnabledCrearGrupo = IsOCUPANTE_MIN;
                            return EnabledListaHorario;
                        }

                if (!string.IsNullOrEmpty(GrupoDescripcion))
                    if (FechaInicio.HasValue)
                        if (FechaFin.HasValue)
                            if (FechaInicio <= FechaFin)
                                if (CheckDomingo || CheckLunes || CheckMartes || CheckMiercoles || CheckJueves || CheckViernes || CheckSabado)
                                    if (ValidateHorasHorarioMarcados())
                                        if (SelectedResponsable.HasValue)
                                        {
                                            TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);
                                            if (!(!string.IsNullOrEmpty(TextErrorResponsable)) && !FechaValidateHasError && !FechaValidateDomingoHasError && !FechaValidateLunesHasError && !FechaValidateMartesHasError && !FechaValidateMiercolesHasError && !FechaValidateJuevesHasError && !FechaValidateViernesHasError && !FechaValidateSabadoHasError)
                                            {
                                                EnabledListaHorario = true;
                                                IsEnabledCrearGrupo = IsOCUPANTE_MIN;
                                            }
                                        }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar la calendarizacion del grupo", ex);
            }
            return EnabledListaHorario;
        }
        /// <summary>
        /// metodo para validar horarios de dias marcados
        /// </summary>
        /// <returns>true, false</returns>
        bool ValidateHorasHorarioMarcados()
        {
            try
            {
                var ckDomingo = 0;
                var ckLunes = 0;
                var ckMartes = 0;
                var ckMiercoles = 0;
                var ckJueves = 0;
                var ckViernes = 0;
                var ckSabado = 0;
                var count = 0;

                if (CheckDomingo)
                {
                    count++;
                    ckDomingo = ((InicioDiaDomingo.HasValue && FinDiaDomingo.HasValue) && !FechaValidateDomingoHasError && SelectedAreaDomingo.HasValue) ? +1 : -1;
                }
                if (CheckLunes)
                {
                    count++;
                    ckLunes = ((InicioDiaLunes.HasValue && FinDiaLunes.HasValue) && !FechaValidateLunesHasError && SelectedAreaLunes.HasValue) ? +1 : -1;
                }
                if (CheckMartes)
                {
                    count++;
                    ckMartes = ((InicioDiaMartes.HasValue && FinDiaMartes.HasValue) && !FechaValidateMartesHasError && SelectedAreaMartes.HasValue) ? +1 : -1;
                }
                if (CheckMiercoles)
                {
                    count++;
                    ckMiercoles = ((InicioDiaMiercoles.HasValue && FinDiaMiercoles.HasValue) && !FechaValidateMiercolesHasError && SelectedAreaMiercoles.HasValue) ? +1 : -1;
                }
                if (CheckJueves)
                {
                    count++;
                    ckJueves = ((InicioDiaJueves.HasValue && FinDiaJueves.HasValue) && !FechaValidateJuevesHasError && SelectedAreaJueves.HasValue) ? +1 : -1;
                }
                if (CheckViernes)
                {
                    count++;
                    ckViernes = ((InicioDiaViernes.HasValue && FinDiaViernes.HasValue) && !FechaValidateViernesHasError && SelectedAreaViernes.HasValue) ? +1 : -1;
                }
                if (CheckSabado)
                {
                    count++;
                    ckSabado = ((InicioDiaSabado.HasValue && FinDiaSabado.HasValue) && !FechaValidateSabadoHasError && SelectedAreaSabado.HasValue) ? +1 : -1;
                }
                return count == (ckDomingo + ckLunes + ckMartes + ckMiercoles + ckJueves + ckViernes + ckSabado);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar las horas asignadas segun los dias marcados", ex);
            }
            return false;
        }
        /// <summary>
        /// metodo para revisar la disponibilidad del area seleccionada
        /// </summary>
        /// <param name="SelectedArea">id de el area seleccionada</param>
        /// <param name="Dia">dia marcado</param>
        /// <returns>true, false</returns>
        string ValidateAreaDisponible(short? SelectedArea, DayOfWeek Dia)
        {
            try
            {
                if (!FechaInicio.HasValue)
                    return string.Empty;
                if (!FechaFin.HasValue)
                    return string.Empty;
                if (!SelectedArea.HasValue)
                    return string.Empty;
                if (FechaInicio > FechaFin)
                    return string.Empty;

                var dates = Enumerable.Range(0, (FechaFin.Value - FechaInicio.Value).Days + 1).Select(d => FechaInicio.Value.AddDays(d)).Where(w => w.DayOfWeek == Dia);

                if (dates.Count() <= 0)
                    return string.Empty;

                #region [GrupoHorario]
                var PredicadoAND = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoOR = PredicateBuilder.False<GRUPO_HORARIO>();
                var PredicadoANDD = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDL = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDMa = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDMi = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDJ = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDV = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDS = PredicateBuilder.True<GRUPO_HORARIO>();
                #endregion
                #region [AtencionCita]
                var AC_PredicadoAND = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoOR = PredicateBuilder.False<ATENCION_CITA>();
                var AC_PredicadoANDD = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDL = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDMa = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDMi = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDJ = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDV = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDS = PredicateBuilder.True<ATENCION_CITA>();
                #endregion
                #region [evento]
                var EV_PredicadoAND = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoOR = PredicateBuilder.False<EVENTO>();
                var EV_PredicadoANDD = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDL = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDMa = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDMi = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDJ = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDV = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDS = PredicateBuilder.True<EVENTO>();
                #endregion

                var despuesde = Fechas.GetFechaDateServer.Date;
                //grupo horario
                PredicadoAND = PredicadoAND.And(w => w.ID_AREA == SelectedArea && despuesde >= w.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value && w.GRUPO.ID_ESTATUS_GRUPO == 1);
                //atencion cita
                AC_PredicadoAND = AC_PredicadoAND.And(w => w.ATENCION_SOLICITUD.ID_AREA == SelectedArea && w.ATENCION_SOLICITUD.ATENCION_CITA.OrderBy(o => o.CITA_FECHA_HORA).FirstOrDefault().CITA_FECHA_HORA >= despuesde
                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1));
                //evento
                EV_PredicadoAND = EV_PredicadoAND.And(w => w.ID_AREA == SelectedArea && w.HORA_INVITACION.Value >= despuesde && w.ID_ESTATUS == 1);

                foreach (var item in dates)
                {
                    var datetoValidate = new RangeDatesValidated();
                    switch (item.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            if (!InicioDiaDomingo.HasValue || !FinDiaDomingo.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaDomingo.Value.Hour, InicioDiaDomingo.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaDomingo.Value.Hour, FinDiaDomingo.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDD.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDD.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento EntityFunctions
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDD.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Monday:
                            if (!InicioDiaLunes.HasValue || !FinDiaLunes.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaLunes.Value.Hour, InicioDiaLunes.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaLunes.Value.Hour, FinDiaLunes.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDL.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDL.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDD.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Tuesday:
                            if (!InicioDiaMartes.HasValue || !FinDiaMartes.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaMartes.Value.Hour, InicioDiaMartes.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaMartes.Value.Hour, FinDiaMartes.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDMa.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDMa.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDD.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Wednesday:
                            if (!InicioDiaMiercoles.HasValue || !FinDiaMiercoles.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaMiercoles.Value.Hour, InicioDiaMiercoles.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaMiercoles.Value.Hour, FinDiaMiercoles.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDMi.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDMi.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDD.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Thursday:
                            if (!InicioDiaJueves.HasValue || !FinDiaJueves.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaJueves.Value.Hour, InicioDiaJueves.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaJueves.Value.Hour, FinDiaJueves.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDJ.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDJ.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDD.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Friday:
                            if (!InicioDiaViernes.HasValue || !FinDiaViernes.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaViernes.Value.Hour, InicioDiaViernes.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaViernes.Value.Hour, FinDiaViernes.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDV.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDV.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDD.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Saturday:
                            if (!InicioDiaSabado.HasValue || !FinDiaSabado.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaSabado.Value.Hour, InicioDiaSabado.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaSabado.Value.Hour, FinDiaSabado.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDS.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDS.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDD.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        default:
                            break;
                    }
                }

                if (new cGrupoHorario().GetData().AsExpandable().Where(PredicadoAND).Where(PredicadoOR.Compile()).Any() || new cAtencionCita().GetData().AsExpandable().Where(AC_PredicadoAND).Where(AC_PredicadoOR.Compile()).Any() || new cEvento().GetData().AsExpandable().Where(EV_PredicadoAND).Where(EV_PredicadoOR.Compile()).Any())
                    return "ESTE LUGAR ESTA OCUPADO A ESTA HORA";
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar la disponibilidad del area", ex);
            }
            return string.Empty;
        }
        /// <summary>
        /// metodo que valida disponibilidad del responsable
        /// </summary>
        /// <param name="SelectedResponsable">responsable seleccionado</param>
        /// <returns>valor de la disponobilidad</returns>
        string ValidateResponsableDisponible(int? SelectedResponsable)
        {
            try
            {
                if (!FechaInicio.HasValue)
                    return string.Empty;
                if (!FechaFin.HasValue)
                    return string.Empty;
                if (!SelectedResponsable.HasValue)
                    return string.Empty;
                if (FechaInicio > FechaFin)
                    return string.Empty;

                if (PanelUpdate == Visibility.Collapsed)
                {
                    var dalAtencionCita = new cAtencionCita();
                    foreach (var item in new cGrupo().GetData().Where(w => w.ID_GRUPO_RESPONSABLE == SelectedResponsable && w.ID_GRUPO != SelectedGrupo.ID_GRUPO).SelectMany(s => s.GRUPO_HORARIO))
                    {
                        if (ListGrupoHorario.Where(w => (w.HORA_INICIO >= item.HORA_INICIO && w.HORA_TERMINO <= item.HORA_TERMINO)).Any())
                        {
                            IsEnabledCrearGrupo = false;
                            return "ESTE RESPONSABLE ESTA OCUPADO PARA ESTAS FECHAS";
                        }
                    }
                    foreach (var item in new cAtencionCita().GetData().Where(w => w.ID_RESPONSABLE == SelectedResponsable
                        && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)))
                    {
                        if (ListGrupoHorario.Where(w => (w.HORA_INICIO >= item.CITA_FECHA_HORA && w.HORA_TERMINO <= item.CITA_HORA_TERMINA)).Any())
                        {
                            IsEnabledCrearGrupo = false;
                            return "ESTE RESPONSABLE ESTA OCUPADO PARA ESTAS FECHAS";
                        }
                    }
                    foreach (var item in new cEvento().GetData().Where(w => w.ID_PERSONA_RESPONSABLE == SelectedResponsable && w.ID_ESTATUS == 1))
                    {
                        if (ListGrupoHorario.Where(w => (w.HORA_INICIO >= item.HORA_INVITACION && w.HORA_TERMINO <= item.HORA_FIN)).Any())
                        {
                            IsEnabledCrearGrupo = false;
                            return "ESTE RESPONSABLE ESTA OCUPADO PARA ESTAS FECHAS";
                        }
                    }
                    IsEnabledCrearGrupo = IsOCUPANTE_MIN;
                    return string.Empty;
                }
                else
                {
                    var dias = new List<short>();

                    if (CheckDomingo)
                        dias.Add(0);
                    if (CheckLunes)
                        dias.Add(1);
                    if (CheckMartes)
                        dias.Add(2);
                    if (CheckMiercoles)
                        dias.Add(3);
                    if (CheckJueves)
                        dias.Add(4);
                    if (CheckViernes)
                        dias.Add(5);
                    if (CheckSabado)
                        dias.Add(6);

                    var dates = Enumerable.Range(0, (FechaFin.Value - FechaInicio.Value).Days + 1).Select(d => FechaInicio.Value.AddDays(d)).Where(w => dias.Contains((short)w.DayOfWeek));

                    if (dates.Count() <= 0)
                        return string.Empty;

                    #region [GrupoHorario]
                    var PredicadoAND = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoOR = PredicateBuilder.False<GRUPO_HORARIO>();
                    var PredicadoANDD = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDL = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDMa = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDMi = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDJ = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDV = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDS = PredicateBuilder.True<GRUPO_HORARIO>();
                    #endregion
                    #region [atencioncita]
                    var AC_PredicadoAND = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoOR = PredicateBuilder.False<ATENCION_CITA>();
                    var AC_PredicadoANDD = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDL = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDMa = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDMi = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDJ = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDV = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDS = PredicateBuilder.True<ATENCION_CITA>();
                    #endregion
                    #region [evento]
                    var EV_PredicadoAND = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoOR = PredicateBuilder.False<EVENTO>();
                    var EV_PredicadoANDD = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDL = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDMa = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDMi = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDJ = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDV = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDS = PredicateBuilder.True<EVENTO>();
                    #endregion

                    var despuesde = Fechas.GetFechaDateServer.Date;
                    //grupo horario
                    PredicadoAND = PredicadoAND.And(w => w.GRUPO.ID_GRUPO_RESPONSABLE == SelectedResponsable && despuesde >= w.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value && w.GRUPO.ID_ESTATUS_GRUPO == 1);
                    //atencion cita
                    AC_PredicadoAND = AC_PredicadoAND.And(w => w.ID_RESPONSABLE == SelectedResponsable && w.ATENCION_SOLICITUD.ATENCION_CITA.OrderBy(o => o.CITA_FECHA_HORA).FirstOrDefault().CITA_FECHA_HORA >= despuesde
                        && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1));
                    //evento
                    EV_PredicadoAND = EV_PredicadoAND.And(w => w.ID_PERSONA_RESPONSABLE == SelectedResponsable && w.HORA_INVITACION.Value >= despuesde && w.ID_ESTATUS == 1);

                    foreach (var item in dates)
                    {
                        var datetoValidate = new RangeDatesValidated();
                        switch (item.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                if (!InicioDiaDomingo.HasValue && !FinDiaDomingo.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaDomingo.Value.Hour, InicioDiaDomingo.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaDomingo.Value.Hour, FinDiaDomingo.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDD.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDD.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDD.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Monday:
                                if (!InicioDiaLunes.HasValue || !FinDiaLunes.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaLunes.Value.Hour, InicioDiaLunes.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaLunes.Value.Hour, FinDiaLunes.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDL.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDL.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDL.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Tuesday:
                                if (!InicioDiaMartes.HasValue || !FinDiaMartes.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaMartes.Value.Hour, InicioDiaMartes.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaMartes.Value.Hour, FinDiaMartes.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDMa.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDMa.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDMa.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Wednesday:
                                if (!InicioDiaMiercoles.HasValue || !FinDiaMiercoles.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaMiercoles.Value.Hour, InicioDiaMiercoles.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaMiercoles.Value.Hour, FinDiaMiercoles.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDMi.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDMi.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDMi.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Thursday:
                                if (!InicioDiaJueves.HasValue || !FinDiaJueves.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaJueves.Value.Hour, InicioDiaJueves.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaJueves.Value.Hour, FinDiaJueves.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDJ.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDJ.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDJ.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Friday:
                                if (!InicioDiaViernes.HasValue || !FinDiaViernes.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaViernes.Value.Hour, InicioDiaViernes.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaViernes.Value.Hour, FinDiaViernes.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDV.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDV.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDV.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Saturday:
                                if (!InicioDiaSabado.HasValue || !FinDiaSabado.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaSabado.Value.Hour, InicioDiaSabado.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaSabado.Value.Hour, FinDiaSabado.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDS.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDS.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDS.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            default:
                                break;
                        }
                    }

                    if (new cGrupoHorario().GetData().AsExpandable().Where(PredicadoAND).Where(PredicadoOR.Compile()).Any() || new cAtencionCita().GetData().AsExpandable().Where(AC_PredicadoAND).Where(AC_PredicadoOR.Compile()).Any() || new cEvento().GetData().AsExpandable().Where(EV_PredicadoAND).Where(EV_PredicadoOR.Compile()).Any())
                        return "ESTE RESPONSABLE ESTA OCUPADO PARA ESTAS FECHAS";
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar la disponibilidad del responsable", ex);
            }
            return string.Empty;
        }
        /// <summary>
        /// metodo auxiliar para advertir de un cambio en la calendarizacion del grupo para cancelar o proseguir
        /// </summary>
        /// <param name="variableName">nombre de la variable</param>
        /// <param name="value">valor a asignar</param>
        /// <param name="Property">nombre de la propiedad</param>
        /// <returns>resume el hilo creado</returns>
        private async Task AdvertenciaCambio(string variableName, object value, string Property)
        {
            try
            {
                WaitValidated = true;
                if (SelectedCount > 0)
                {
                    if (isInicioDia)
                        if (await new Dialogos().ConfirmarEliminar("Creación de Grupo", "Si cambia la calendarización del grupo afectará a los internos seleccionados, marcando en rojo aquellos que se empalmen.\nEsta operación puede tomar algunos segundos\nQuiere seguir con el cambio?") == 0)
                        {
                            var previous = this.GetType().GetProperty(Property).GetValue(this, null);
                            this.GetType().GetField(variableName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, null);
                            OnPropertyValidateChanged(Property);

                            this.GetType().GetField(variableName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, previous);
                            OnPropertyValidateChanged(Property);
                            isNeeded = false;
                            WaitValidated = false;
                            return;
                        }
                }

                this.GetType().GetField(variableName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, value);
                if (!InvalidatePropertyValidateChange)
                    OnPropertyValidateChanged(Property);
                else
                    OnPropertyChanged(Property);

                if (ListGrupoParticipante != null)
                {
                    var prevList = new ObservableCollection<ListaInternos>(ListGrupoParticipante);
                    EmpalmeHorarioNuevo = 0;
                    foreach (var item in prevList)
                        if (item.elegido)
                        {
                            if (GenerarListaActividades(item.Entity.INGRESO).Where(w => w.State.Equals("Empalme")).Any())
                            {
                                item.elegido = false;
                                item.ShowEmpalme = new Thickness(2);
                                EmpalmeHorarioNuevo = new Nullable<short>();
                            }
                        }
                        else
                            item.ShowEmpalme = new Thickness(0);

                    ListGrupoParticipante = new ObservableCollection<ListaInternos>(prevList.ToList());
                    SelectedCount = ListGrupoParticipante.Where(w => w.elegido).Count();
                    CalcularInternosSeleccionados();

                    if ("InicioDiaDomingo InicioDiaLunes InicioDiaMartes InicioDiaMiercoles InicioDiaJueves InicioDiaViernes InicioDiaSabado CheckDomingo CheckLunes CheckMartes CheckMiercoles CheckJueves CheckViernes CheckSabado".Contains(Property))
                        isInicioDia = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar advertencia de cambio en horario", ex);
            }
            WaitValidated = false;
            return;
        }
        /// <summary>
        /// metodo que calcula los internos seleccionados
        /// </summary>
        void CalcularInternosSeleccionados()
        {
            try
            {
                var totalseleccionado = 0;
                if (EnabledEditGrupo)
                {
                    totalseleccionado = SelectedCount + (new cGrupoParticipante().GetData().Where(w => w.ID_GRUPO == SelectedGrupo.ID_GRUPO).Count() - SelectedCount) + (ListGrupoParticipante != null ? ListGrupoParticipante.Where(w => w.elegido && w.Entity.GRUPO == null).Count() : 0);
                    isOCUPANTE_MAX = totalseleccionado >= OCUPANTE_MAX;
                    IsOCUPANTE_MIN = totalseleccionado >= OCUPANTE_MIN;
                }
                else
                {
                    isOCUPANTE_MAX = SelectedCount >= OCUPANTE_MAX;
                    IsOCUPANTE_MIN = SelectedCount >= OCUPANTE_MIN;
                    totalseleccionado = SelectedCount;
                }
                SelectedCountText = "Minimo de " + OCUPANTE_MIN + ", " + totalseleccionado + "/" + OCUPANTE_MAX + " Seleccionados";
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular Internos Seleccionados", ex);
            }
        }
        /// <summary>
        /// metodo que refresca la informacion de las actividades
        /// </summary>
        void RecargarActividadParticipante()
        {
            try
            {
                if (SelectedInterno != null)
                    ListActividadParticipante = new ObservableCollection<ListaActividad>(GenerarListaActividades(SelectedInterno.Entity.INGRESO));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar horario de participante", ex);
            }
        }
        /// <summary>
        /// metodo para gruardar el grupo
        /// </summary>
        private async void CrearGrupo()
        {
            try
            {
                if (WaitValidated)
                {
                    WaitValidated = false;
                    return;
                }

                if (string.IsNullOrEmpty(GrupoDescripcion))
                    if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "Debe Ingresar Nombre del Grupo") == 1)
                        return;

                var recurrencia = string.Empty;
                var dias = new List<short>();
                if (!(PanelUpdate == Visibility.Collapsed))
                {
                    if (!FechaInicio.HasValue || !FechaFin.HasValue)
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "Debe Seleccionar Fecha de Inicio y Final") == 1)
                            return;

                    if (FechaValidateHasError)
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "Las Fecha de Inicio y Final No Son Validas") == 1)
                            return;

                    if (!(CheckDomingo || CheckLunes || CheckMartes || CheckMiercoles || CheckJueves || CheckViernes || CheckSabado))
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "Debe Seleccionar Recurrencia") == 1)
                            return;

                    if (((CheckDomingo == (InicioDiaDomingo.HasValue && FinDiaDomingo.HasValue)) == false) || (CheckLunes == (InicioDiaLunes.HasValue && FinDiaLunes.HasValue) == false) || (CheckMartes == (InicioDiaMartes.HasValue && FinDiaMartes.HasValue) == false) || (CheckMiercoles == (InicioDiaMiercoles.HasValue && FinDiaMiercoles.HasValue) == false) || (CheckJueves == (InicioDiaJueves.HasValue && FinDiaJueves.HasValue) == false) || (CheckViernes == (InicioDiaViernes.HasValue && FinDiaViernes.HasValue) == false) || (CheckSabado == (InicioDiaSabado.HasValue && FinDiaSabado.HasValue) == false))
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "Debe Seleccionar Horario") == 1)
                            return;

                    if (FechaValidateDomingoHasError || FechaValidateLunesHasError || FechaValidateMartesHasError || FechaValidateMiercolesHasError || FechaValidateJuevesHasError || FechaValidateViernesHasError || FechaValidateSabadoHasError)
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "El Horaio de Inicio y Final No Es Valido") == 1)
                            return;

                    if (((CheckDomingo == (CheckDomingo && SelectedAreaDomingo.HasValue && string.IsNullOrEmpty(AreaValidateDomingo))) == false) || (CheckLunes == (CheckLunes && SelectedAreaLunes.HasValue && string.IsNullOrEmpty(AreaValidateLunes)) == false) || (CheckMartes == (CheckMartes && SelectedAreaMartes.HasValue && string.IsNullOrEmpty(AreaValidateMartes)) == false) || (CheckMiercoles == (CheckMiercoles && SelectedAreaMiercoles.HasValue && string.IsNullOrEmpty(AreaValidateMiercoles)) == false) || (CheckJueves == (CheckJueves && SelectedAreaJueves.HasValue && string.IsNullOrEmpty(AreaValidateJueves)) == false) || (CheckViernes == (CheckViernes && SelectedAreaViernes.HasValue && string.IsNullOrEmpty(AreaValidateViernes)) == false) || (CheckSabado == (CheckSabado && SelectedAreaSabado.HasValue && string.IsNullOrEmpty(AreaValidateSabado)) == false))
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "Seleccione Un Area Para Continuar") == 1)
                            return;

                    var checkBoxes = new List<string>();
                    if (CheckDomingo)
                        checkBoxes.Add("D");
                    if (CheckLunes)
                        checkBoxes.Add("L");
                    if (CheckMartes)
                        checkBoxes.Add("Ma");
                    if (CheckMiercoles)
                        checkBoxes.Add("Mi");
                    if (CheckJueves)
                        checkBoxes.Add("J");
                    if (CheckViernes)
                        checkBoxes.Add("V");
                    if (CheckSabado)
                        checkBoxes.Add("S");

                    foreach (var item in checkBoxes)
                    {
                        recurrencia += item + ",";
                        dias.Add(item == "D" ? (short)0 : item == "L" ? (short)1 : item == "Ma" ? (short)2 : item == "Mi" ? (short)3 : item == "J" ? (short)4 : item == "V" ? (short)5 : (short)6);
                    }
                    recurrencia = recurrencia.Substring(0, (recurrencia.Length - 1));

                    if (string.IsNullOrEmpty(recurrencia))
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "Opcion de Recurrencia No Valida") == 1)
                            return;
                }

                if (!SelectedResponsable.HasValue)
                    if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "Debe Seleccionar Responsable Del Grupo") == 1)
                        return;

                if (!string.IsNullOrEmpty(TextErrorResponsable))
                    if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "El Responsable Seleccioinado NO Esta Disponible Para Estas Fechas") == 1)
                        return;

                if (!IsEnabledCrearGrupo)
                    if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "Faltan Internos Por Seleccionar") == 1)
                        return;

                Validaciones();
                if (base.HasErrors)
                    if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo", "Revise Que Los Valores De Los Campos Esten Bien Ingresados") == 1)
                        return;
                ///TODO: cambiar centro por el del usuario
                ///cambiar id_departamento
                ///cambiar id_grupo_responsable

                var result = await StaticSourcesViewModel.OperacionesAsync<bool>((PanelUpdate == Visibility.Collapsed ? "Actualizando" : "Creando") + " Grupo", () =>
                {
                    var idgrupo = new Nullable<short>();
                    try
                    {
                        if (!(PanelUpdate == Visibility.Collapsed))
                        {
                            idgrupo = new cGrupo().Insertar(new GRUPO()
                            {
                                ID_CENTRO = GlobalVar.gCentro,//4,
                                ID_TIPO_PROGRAMA = SelectedPrograma.Value,
                                ID_ACTIVIDAD = SelectedActividad.Value,
                                ID_EJE = SelectedEje.Value,
                                ID_DEPARTAMENTO = 1,
                                ID_GRUPO_RESPONSABLE = SelectedResponsable,
                                ID_ESTATUS_GRUPO = SelectedEstatus,
                                FEC_INICIO = FechaInicio,
                                FEC_FIN = FechaFin,
                                RECURRENCIA = recurrencia,
                                DESCR = GrupoDescripcion
                            });

                            var dates = Enumerable.Range(0, (FechaFin.Value - FechaInicio.Value).Days + 1).Select(d => FechaInicio.Value.AddDays(d)).Where(w => dias.Contains((short)w.DayOfWeek));

                            foreach (var item in dates)
                            {
                                var hora_inicio = new DateTime();
                                var hora_fin = new DateTime();
                                var area = new short();

                                switch (item.DayOfWeek)
                                {
                                    case DayOfWeek.Sunday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaDomingo.Value.Hour, InicioDiaDomingo.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaDomingo.Value.Hour, FinDiaDomingo.Value.Minute, 0);
                                        area = SelectedAreaDomingo.Value;
                                        break;
                                    case DayOfWeek.Monday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaLunes.Value.Hour, InicioDiaLunes.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaLunes.Value.Hour, FinDiaLunes.Value.Minute, 0);
                                        area = SelectedAreaLunes.Value;
                                        break;
                                    case DayOfWeek.Tuesday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaMartes.Value.Hour, InicioDiaMartes.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaMartes.Value.Hour, FinDiaMartes.Value.Minute, 0);
                                        area = SelectedAreaMartes.Value;
                                        break;
                                    case DayOfWeek.Wednesday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaMiercoles.Value.Hour, InicioDiaMiercoles.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaMiercoles.Value.Hour, FinDiaMiercoles.Value.Minute, 0);
                                        area = SelectedAreaMiercoles.Value;
                                        break;
                                    case DayOfWeek.Thursday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaJueves.Value.Hour, InicioDiaJueves.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaJueves.Value.Hour, FinDiaJueves.Value.Minute, 0);
                                        area = SelectedAreaJueves.Value;
                                        break;
                                    case DayOfWeek.Friday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaViernes.Value.Hour, InicioDiaViernes.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaViernes.Value.Hour, FinDiaViernes.Value.Minute, 0);
                                        area = SelectedAreaViernes.Value;
                                        break;
                                    case DayOfWeek.Saturday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaSabado.Value.Hour, InicioDiaSabado.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaSabado.Value.Hour, FinDiaSabado.Value.Minute, 0);
                                        area = SelectedAreaSabado.Value;
                                        break;
                                    default:
                                        break;
                                }

                                var idgrupohorario = new cGrupoHorario().Insertar(new GRUPO_HORARIO()
                                  {
                                      ESTATUS = 1,
                                      HORA_INICIO = hora_inicio,
                                      HORA_TERMINO = hora_fin,
                                      ID_ACTIVIDAD = SelectedActividad.Value,
                                      ID_AREA = area,
                                      ID_CENTRO = GlobalVar.gCentro,//4,
                                      ID_GRUPO = idgrupo.Value,
                                      ID_TIPO_PROGRAMA = SelectedPrograma.Value
                                  });


                                foreach (var ListGP in ListGrupoParticipante.Where(w => w.elegido).Select(s => s.Entity))
                                {
                                    new cGrupoAsistencia().Insert(new GRUPO_ASISTENCIA()
                                    {
                                        ID_CENTRO = GlobalVar.gCentro,
                                        ID_TIPO_PROGRAMA = SelectedPrograma.Value,
                                        ID_ACTIVIDAD = SelectedActividad.Value,
                                        ID_GRUPO = idgrupo.Value,
                                        ID_GRUPO_HORARIO = idgrupohorario.ID_GRUPO_HORARIO,
                                        ID_CONSEC = ListGP.ID_CONSEC,
                                        FEC_REGISTRO = Fechas.GetFechaDateServer,
                                        ASISTENCIA = null,
                                        EMPALME = 0,
                                        EMP_COORDINACION = 0,
                                        EMP_APROBADO = null,
                                        EMP_FECHA = null,
                                        ESTATUS = 1
                                    });
                                }
                            }
                        }

                        if (PanelUpdate == Visibility.Collapsed)
                        {
                            var entityGrupo = new cGrupo().GetData().Where(w => w.ID_GRUPO == SelectedGrupo.ID_GRUPO).FirstOrDefault();
                            idgrupo = entityGrupo.ID_GRUPO;
                            new cGrupo().Update(new GRUPO()
                            {
                                ID_GRUPO = entityGrupo.ID_GRUPO,
                                ID_CENTRO = entityGrupo.ID_CENTRO,
                                ID_TIPO_PROGRAMA = entityGrupo.ID_TIPO_PROGRAMA,
                                ID_ACTIVIDAD = entityGrupo.ID_ACTIVIDAD,
                                ID_DEPARTAMENTO = entityGrupo.ID_DEPARTAMENTO,
                                ID_GRUPO_RESPONSABLE = SelectedResponsable,
                                ID_ESTATUS_GRUPO = SelectedEstatus,
                                FEC_INICIO = entityGrupo.FEC_INICIO,
                                FEC_FIN = entityGrupo.FEC_FIN,
                                RECURRENCIA = entityGrupo.RECURRENCIA,
                                DESCR = GrupoDescripcion,
                                ID_EJE = entityGrupo.ID_EJE
                            });

                            foreach (var item in new cGrupoParticipante().GetData().Where(w => w.ID_GRUPO == SelectedGrupo.ID_GRUPO && (w.GRUPO_PARTICIPANTE_CANCELADO.Where(wh => (wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_CONSEC == w.ID_CONSEC && wh.ID_GRUPO == w.ID_GRUPO)).Any() ?

                            w.GRUPO_PARTICIPANTE_CANCELADO.Where(wh => (wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_CONSEC == w.ID_CONSEC) &&
                                (wh.RESPUESTA_FEC == null ?

                                (wh.RESPUESTA_FEC == null && wh.ESTATUS == 0) :

                                (w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC) && whe.RESPUESTA_FEC != null && (whe.ESTATUS == 0 || whe.ESTATUS == 2)).Count() == w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC)).Count()))).Any()


                                : true)))
                            {
                                new cGrupoParticipante().Update(new GRUPO_PARTICIPANTE()
                                {
                                    EJE = item.EJE,
                                    ESTATUS = 1,
                                    FEC_REGISTRO = item.FEC_REGISTRO,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_CONSEC = item.ID_CONSEC,
                                    ID_GRUPO = null,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    ING_ID_ANIO = item.ING_ID_ANIO,
                                    ING_ID_CENTRO = item.ING_ID_CENTRO,
                                    ING_ID_IMPUTADO = item.ING_ID_IMPUTADO,
                                    ING_ID_INGRESO = item.ING_ID_INGRESO
                                });
                            }

                            foreach (var ListGP in ListGrupoParticipante.Where(w => w.elegido).Select(s => s.Entity))
                            {
                                if (!new cGrupoAsistencia().GetData().Where(w => w.ID_CONSEC == ListGP.ID_CONSEC && w.ID_GRUPO == SelectedGrupo.ID_GRUPO).Any())
                                    foreach (var item in new cGrupoHorario().GetData().Where(w => w.ID_GRUPO == SelectedGrupo.ID_GRUPO))
                                    {
                                        new cGrupoAsistencia().Insert(new GRUPO_ASISTENCIA()
                                        {
                                            ID_CENTRO = GlobalVar.gCentro,
                                            ID_TIPO_PROGRAMA = SelectedPrograma.Value,
                                            ID_ACTIVIDAD = SelectedActividad.Value,
                                            ID_GRUPO = idgrupo.Value,
                                            ID_GRUPO_HORARIO = item.ID_GRUPO_HORARIO,
                                            ID_CONSEC = ListGP.ID_CONSEC,
                                            FEC_REGISTRO = Fechas.GetFechaDateServer,
                                            ASISTENCIA = null,
                                            EMPALME = 0,
                                            EMP_COORDINACION = 0,
                                            EMP_APROBADO = null,
                                            EMP_FECHA = null,
                                            ESTATUS = 1
                                        });
                                    }
                            }
                        }

                        foreach (var item in ListGrupoParticipante.Where(w => w.elegido).Select(s => s.Entity))
                        {
                            new cGrupoParticipante().Update(new GRUPO_PARTICIPANTE()
                            {
                                EJE = item.EJE,
                                ESTATUS = 2,
                                FEC_REGISTRO = item.FEC_REGISTRO,
                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                ID_CENTRO = item.ID_CENTRO,
                                ID_CONSEC = item.ID_CONSEC,
                                ID_GRUPO = idgrupo,
                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                ING_ID_ANIO = item.ING_ID_ANIO,
                                ING_ID_CENTRO = item.ING_ID_CENTRO,
                                ING_ID_IMPUTADO = item.ING_ID_IMPUTADO,
                                ING_ID_INGRESO = item.ING_ID_INGRESO
                            });
                        }

                        if (ListGrupoParticipante.Where(w => w.elegido && w.HorarioInterno != null && w.HorarioInterno.Where(wh => wh.State.Equals("Empalme") && wh.Revision && wh.ListHorario.Where(whe => whe.State.Equals("Empalme")).Any()).Any()).Any())
                        {
                            var listGA = new List<ListaEmpalmesInterno>();
                            var entitylistGH = new List<GRUPO_HORARIO>();
                            foreach (var item in ListGrupoParticipante.Where(w => w.elegido && w.HorarioInterno != null && w.HorarioInterno.Where(wh => wh.State.Equals("Empalme") && wh.Revision && wh.ListHorario.Where(whe => whe.State.Equals("Empalme")).Any()).Any()))
                            {
                                foreach (var itemLisAct in item.HorarioInterno.Where(w => w.ListHorario.Where(whe => whe.State.Equals("Empalme")).Any()))
                                    entitylistGH.AddRange(itemLisAct.ListHorario.Where(w => w.State.Equals("Empalme")).Select(s => s.GrupoHorarioEntity).ToList());
                                listGA.Add(new ListaEmpalmesInterno() { EntityGrupoParticipante = item.Entity, ListGrupoHorario = entitylistGH });
                            }
                            new cGrupoAsistencia().GenerarEmpalmes(listGA);
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear grupo", ex);
                        if (idgrupo.HasValue)
                            new cGrupo().CrearGrupoRollback(idgrupo.Value);
                        return false;
                    }
                    return true;
                });

                if (result)
                {
                    LimpiarCampos();
                    if (PanelUpdate == Visibility.Collapsed)
                        await new Dialogos().ConfirmacionDialogoReturn("Crear Grupo", "Grupo Actualizado Exitosamente");
                    else
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Crear Grupo", "Grupo Creado Exitosamente");
                        ListaInternosLoad(SelectedActividad.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear grupo", ex);
            }
        }
        /// <summary>
        /// metodo para limpiar el tab de creacion de grupo modelo
        /// </summary>
        private void LimpiarCampos()
        {
            try
            {
                #region [GRUPO]
                InvalidatePropertyValidateChange = true;
                StaticSourcesViewModel.SourceChanged = false;
                SelectedGrupo = null;
                SelectedCount = 0;
                ListGrupoParticipante = null;
                InicioDiaDomingo = FinDiaDomingo = InicioDiaLunes = FinDiaLunes = InicioDiaMartes = FinDiaMartes = InicioDiaMiercoles = FinDiaMiercoles = InicioDiaJueves = FinDiaJueves = InicioDiaViernes = FinDiaViernes = InicioDiaSabado = FinDiaSabado = FechaInicio = FechaFin = null;
                SelectedEstatus = SelectedAreaDomingo = SelectedAreaLunes = SelectedAreaMartes = SelectedAreaMiercoles = SelectedAreaJueves = SelectedAreaViernes = SelectedAreaSabado = null;
                SelectedResponsable = null;
                selectedCountTrue = isOCUPANTE_MAX = IsOCUPANTE_MIN = false;
                MensajeText = SelectedCountText = GrupoDescripcion = string.Empty;
                CheckDomingo = CheckLunes = CheckMartes = CheckMiercoles = CheckJueves = CheckViernes = CheckSabado = false;
                #endregion
                InvalidatePropertyValidateChange = false;
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Limpiar Campos", ex);
            }
        }
        #endregion

        #region [COMPLEMENTARIO]
        /// <summary>
        /// metodo para cargar informacion al entrar al modulo
        /// </summary>
        /// <param name="obj">usercontrol del modulo</param>
        async void CreacionGruposComplementariosLoad()
        {
            try
            {
                ListProgramasCompl = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TIPO_PROGRAMA>>(() => new ObservableCollection<TIPO_PROGRAMA>(new cActividadEje().GetData().Where(w => w.EJE.COMPLEMENTARIO == "S").Select(s => s.ACTIVIDAD.TIPO_PROGRAMA).Distinct().OrderBy(o => o.NOMBRE).ToList()));
                ListResponsable = ListResponsable ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<NombreEmpleado>>(() => new ObservableCollection<NombreEmpleado>(new cPersona().GetData().Where(w => w.ID_TIPO_PERSONA == 1).Select(s => new NombreEmpleado
                {
                    ID_PERSONA = s.ID_PERSONA,
                    NOMBRE_COMPLETO = s.PATERNO.Trim() + " " + s.MATERNO.Trim() + " " + s.NOMBRE.Trim()
                }).OrderBy(o => o.NOMBRE_COMPLETO)));
                ListArea = ListArea ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<AREA>>(() => new ObservableCollection<AREA>(new cArea().GetData().Where(w => w.ID_TIPO_AREA != 5)));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar modulo de creacion de programas", ex);
            }
        }
        /// <summary>
        /// cargamos las actividades segun el programa seleccionado
        /// </summary>
        /// <param name="Id_tipo_programa">id del programa seleccionado</param>
        async void ActividadesComplementariasLoad(short Id_tipo_programa)
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }
                ListActividadesCompl = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ACTIVIDAD>>(() => new ObservableCollection<ACTIVIDAD>(new cActividadEje().GetData().Where(w => w.EJE.COMPLEMENTARIO == "S" && w.ID_TIPO_PROGRAMA == Id_tipo_programa).Select(s => s.ACTIVIDAD).Distinct().OrderBy(o => o.DESCR).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar actividades", ex);
            }
        }
        /// <summary>
        /// cargamos a la poblacion del centro
        /// </summary>
        /// <param name="Id_actividad">id de la actividad seleccionada</param>
        async void ListaInternosComplLoad(short Id_actividad)
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }
                SelectedCountCompl = 0;
                selectedCountComplTrue = false;
                isOCUPANTE_MAXCompl = false;
                IsOCUPANTE_MINCompl = false;
                EnabledListaHorarioCompl = false;

                //cargamos los combobox para los filtros
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    try
                    {
                        ListGrupoCompl = new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == SelectedProgramaCompl && w.ID_ACTIVIDAD == Id_actividad).Select(s => s).Distinct().OrderBy(o => o.DESCR).ToList());
                        ListDelitosCompl = ListDelitosCompl ?? new ObservableCollection<DELITO>(new cDelito().GetData().OrderBy(o => o.DESCR).ToList());
                        ListPlanimetriaCompl = ListPlanimetriaCompl ?? new ObservableCollection<SECTOR_CLASIFICACION>(new cSectorClasificacion().GetData().OrderBy(o => o.POBLACION).ToList());
                        ListPAniosCompl = ListPAniosCompl ?? new ObservableCollection<RangoANIOS>(new cIngreso().GetData().Select(s => s.ID_ANIO).Distinct().OrderBy(o => o).AsEnumerable().Select(s => new RangoANIOS { ANIO = s.ToString(), ID_ANIO = s }));
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos", ex);
                    }
                });
                if (!ListDelitosCompl.Where(w => w.ID_DELITO == -1).Any())
                    ListDelitosCompl.Insert(0, new DELITO() { ID_DELITO = -1, DESCR = string.Empty });

                if (!ListPlanimetriaCompl.Where(w => w.ID_SECTOR_CLAS == -1).Any())
                    ListPlanimetriaCompl.Insert(0, new SECTOR_CLASIFICACION() { ID_SECTOR_CLAS = -1, POBLACION = string.Empty });

                if (ListPAniosCompl.Any())
                    ListPAniosCompl.Insert(0, new RangoANIOS() { ID_ANIO = null, ANIO = string.Empty });

                ListGrupoParticipanteCompl = new RangeEnabledObservableCollection<ListaInternosCompl>();
                ListGrupoParticipanteCompl.InsertRange(await SegmentarResultadoParticipantes());

                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listado", ex);
            }
        }
        /// <summary>
        /// cargamos a la poblacion y a los que pertenecen al grupo seleccionado
        /// </summary>
        /// <param name="Id_grupo">id del grupo seleccionado</param>
        async void ListaInternosUpdateCompl(short Id_grupo)
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }
                SelectedCount = 0;
                selectedCountTrue = false;
                isOCUPANTE_MAX = false;
                IsOCUPANTE_MIN = false;

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    try
                    {
                        ListDelitosCompl = ListDelitosCompl ?? new ObservableCollection<DELITO>(new cDelito().GetData().OrderBy(o => o.DESCR).ToList());
                        ListPlanimetriaCompl = ListPlanimetriaCompl ?? new ObservableCollection<SECTOR_CLASIFICACION>(new cSectorClasificacion().GetData().OrderBy(o => o.POBLACION).ToList());
                        ListPAniosCompl = ListPAniosCompl ?? new ObservableCollection<RangoANIOS>(new cIngreso().GetData().Select(s => s.ID_ANIO).Distinct().OrderBy(o => o).AsEnumerable().Select(s => new RangoANIOS { ANIO = s.ToString(), ID_ANIO = s }));
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos", ex);
                    }
                });
                if (!ListDelitosCompl.Where(w => w.ID_DELITO == -1).Any())
                    ListDelitosCompl.Insert(0, new DELITO() { ID_DELITO = -1, DESCR = string.Empty });

                if (!ListPlanimetriaCompl.Where(w => w.ID_SECTOR_CLAS == -1).Any())
                    ListPlanimetriaCompl.Insert(0, new SECTOR_CLASIFICACION() { ID_SECTOR_CLAS = -1, POBLACION = string.Empty });

                if (ListPAniosCompl.Any())
                    ListPAniosCompl.Insert(0, new RangoANIOS() { ID_ANIO = null, ANIO = string.Empty });

                ListGrupoParticipanteCompl = new RangeEnabledObservableCollection<ListaInternosCompl>();
                ListGrupoParticipanteCompl.InsertRange(await SegmentarResultadoParticipantes());

                ListaRestaurarSeleccionadosCompl = new ObservableCollection<ListaInternosCompl>(ListGrupoParticipanteCompl.Where(w => w.elegido));

                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listado", ex);
            }
        }
        /// <summary>
        /// metodo para segmentar listado de internos
        /// </summary>
        /// <param name="_Pag">numero de pagina</param>
        /// <returns>lista segmentada en 30</returns>
        private async Task<List<ListaInternosCompl>> SegmentarResultadoParticipantes(int _Pag = 1)
        {
            var result = new ObservableCollection<ListaInternosCompl>();
            try
            {
                Pagina = _Pag;
                result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListaInternosCompl>>(() =>
                {
                    var listresult = new ObservableCollection<ListaInternosCompl>();
                    try
                    {
                        var edadinicio = string.IsNullOrEmpty(SelectedEdades) ? new Nullable<int>() : Fechas.GetFechaDateServer.AddYears(-(Convert.ToInt32(SelectedEdades.Split('-')[0]))).Year;
                        var edadfinal = string.IsNullOrEmpty(SelectedEdades) ? new Nullable<int>() : Fechas.GetFechaDateServer.AddYears(-(Convert.ToInt32(SelectedEdades.Split('-')[1]))).Year;
                        var grupo = SelectedGrupoCompl != null ? SelectedGrupoCompl.ID_GRUPO : new Nullable<short>();

                        if (SelectedActividadCompl.HasValue)
                            listresult = new ObservableCollection<ListaInternosCompl>(new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro, Parametro.ESTATUS_ADMINISTRATIVO_INACT, SelectedProgramaCompl.Value, SelectedActividadCompl.Value, SelectedDelitoCompl, SelectedPlanimetriaCompl, edadinicio, edadfinal, SelectedAniosCompl, grupo, _Pag)
                                .Where(w => !w.TRASLADO_DETALLE.Any(wh => (wh.ID_ESTATUS != "CA" ? wh.TRASLADO.ORIGEN_TIPO != "F" : false)))
                                .AsEnumerable()
                                .Select(s => new ListaInternosCompl
                                {
                                    elegido = RestaurarSeleccionadosCompl(s.ID_ANIO, s.ID_IMPUTADO) ? true : SelectedGrupoCompl != null ? s.GRUPO_PARTICIPANTE.Count > 0 ? s.GRUPO_PARTICIPANTE.Where(w => w.ID_ACTIVIDAD == SelectedActividadCompl && w.ID_TIPO_PROGRAMA == SelectedProgramaCompl && w.ID_GRUPO == grupo).Any() : false : false,
                                    Entity = s,
                                    ListGRPA = s.GRUPO_PARTICIPANTE.ToList(),
                                    NOMBRE = s != null ? s.IMPUTADO != null ? !string.IsNullOrEmpty(s.IMPUTADO.NOMBRE) ? s.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                    PATERNO = s != null ? s.IMPUTADO != null ? !string.IsNullOrEmpty(s.IMPUTADO.PATERNO) ? s.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                    MATERNO = s != null ? s.IMPUTADO != null ? !string.IsNullOrEmpty(s.IMPUTADO.MATERNO) ? s.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                    NOMBRECOMPLETO = s != null ? s.IMPUTADO != null ? (!string.IsNullOrEmpty(s.IMPUTADO.NOMBRE) ? s.IMPUTADO.NOMBRE.Trim() : string.Empty) + " " + (!string.IsNullOrEmpty(s.IMPUTADO.PATERNO) ? s.IMPUTADO.PATERNO.Trim() : string.Empty) + " " + (!string.IsNullOrEmpty(s.IMPUTADO.MATERNO) ? s.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty,
                                    ImageSource =
                                    s.INGRESO_BIOMETRICO.Count > 0 ?
                                    s.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                                    s.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                    s.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                                    s.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                    new Imagenes().getImagenPerson() :
                                    new Imagenes().getImagenPerson(),
                                    FOLIO = s != null ? s.ID_ANIO + "\\" + s.ID_IMPUTADO : string.Empty,

                                    UBICACION = s.CAMA != null ? s.CAMA.CELDA != null ? s.CAMA.CELDA.SECTOR != null ? s.CAMA.CELDA.SECTOR.EDIFICIO != null ? s.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + s.CAMA.CELDA.SECTOR.DESCR.Trim() + s.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(s.CAMA != null ? s.CAMA.DESCR : string.Empty) ? s.CAMA != null ? s.CAMA.ID_CAMA.ToString().Trim() : string.Empty : s.CAMA != null ? s.CAMA.ID_CAMA + " " + s.CAMA.DESCR.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty,
                                    PLANIMETRIA = s.CAMA != null ? s.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty,
                                    PLANIMETRIACOLOR = s.CAMA != null ? s.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.COLOR : string.Empty : string.Empty : string.Empty : string.Empty,
                                    RESTANTE = string.IsNullOrEmpty(CalcularSentencia(s.CAUSA_PENAL)) ? string.Empty : CalcularSentencia(s.CAUSA_PENAL).Replace('_', ' '),
                                    SENTENCIA = varauxSentencia,
                                    RESTANTEsplit = string.IsNullOrEmpty(CalcularSentencia(s.CAUSA_PENAL)) ? "999_Años_999_Meses_999_Dias" : CalcularSentencia(s.CAUSA_PENAL),
                                    ShowEmpalme = new Thickness(0),
                                    hasActividad = s.GRUPO_PARTICIPANTE.Where(w => w.ID_ACTIVIDAD == SelectedActividadCompl && w.ID_TIPO_PROGRAMA == SelectedProgramaCompl).Any()
                                })
                                .OrderByDescending(o => grupo.HasValue ? o.elegido : o.hasActividad)
                                .ThenByDescending(o => grupo.HasValue ? o.hasActividad : o.elegido)
                                .ThenBy(t => Convert.ToInt32(t.RESTANTEsplit.Split('_')[0]))
                                .ThenBy(t => Convert.ToInt32(t.RESTANTEsplit.Split('_')[2]))
                                .ThenBy(t => Convert.ToInt32(t.RESTANTEsplit.Split('_')[4]))
                                .ThenBy(t => t.Entity.ID_ANIO)
                                .ThenBy(t => t.Entity.ID_IMPUTADO)
                                );

                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Obtener Ingresos Activos", ex);
                    }
                    return listresult;
                });
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                CalcularInternosSeleccionadosCompl();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de segmentacion", ex);
            }
            return result.ToList();
        }
        /// <summary>
        /// metodo que restablece internos seleccionados
        /// </summary>
        /// <param name="p1">anio</param>
        /// <param name="p2">imputado</param>
        /// <returns></returns>
        private bool RestaurarSeleccionadosCompl(short p1, int p2)
        {
            return ListaRestaurarSeleccionadosCompl.Where(w => w.FOLIO.Equals(p1 + "\\" + p2)).Any();
        }
        /// <summary>
        /// validar horario del grupo
        /// </summary>
        /// <returns></returns>
        bool ValidateHorarioListaInternoCompl()
        {
            try
            {
                EnabledListaHorarioCompl = false;
                IsEnabledCrearGrupoCompl = false;
                SelectedInternoCompl = null;

                if (PanelUpdateCompl == Visibility.Collapsed)
                    if (!string.IsNullOrEmpty(GrupoDescripcionCompl))
                        if (SelectedResponsableCompl.HasValue)
                        {
                            EnabledListaHorarioCompl = true;
                            IsEnabledCrearGrupoCompl = IsOCUPANTE_MINCompl;
                            return EnabledListaHorarioCompl;
                        }

                if (!string.IsNullOrEmpty(GrupoDescripcionCompl))
                    if (FechaInicioCompl.HasValue)
                        if (FechaFinCompl.HasValue)
                            if (FechaInicioCompl <= FechaFinCompl)
                                if (CheckDomingoCompl || CheckLunesCompl || CheckMartesCompl || CheckMiercolesCompl || CheckJuevesCompl || CheckViernesCompl || CheckSabadoCompl)
                                    if (ValidateHorasHorarioMarcadosCompl())
                                        if (SelectedResponsableCompl.HasValue)
                                        {
                                            TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);
                                            if (!(!string.IsNullOrEmpty(TextErrorResponsableCompl)) && !FechaValidateHasErrorCompl && !FechaValidateDomingoHasErrorCompl && !FechaValidateLunesHasErrorCompl && !FechaValidateMartesHasErrorCompl && !FechaValidateMiercolesHasErrorCompl && !FechaValidateJuevesHasErrorCompl && !FechaValidateViernesHasErrorCompl && !FechaValidateSabadoHasErrorCompl)
                                            {
                                                EnabledListaHorarioCompl = true;
                                                IsEnabledCrearGrupoCompl = IsOCUPANTE_MINCompl;
                                            }
                                        }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar la calendarizacion del grupo", ex);
            }
            return EnabledListaHorarioCompl;
        }
        /// <summary>
        /// metodo para validar horarios de dias marcados
        /// </summary>
        /// <returns>true, false</returns>
        bool ValidateHorasHorarioMarcadosCompl()
        {
            try
            {
                var ckDomingo = 0;
                var ckLunes = 0;
                var ckMartes = 0;
                var ckMiercoles = 0;
                var ckJueves = 0;
                var ckViernes = 0;
                var ckSabado = 0;
                var count = 0;

                if (CheckDomingoCompl)
                {
                    count++;
                    ckDomingo = ((InicioDiaDomingoCompl.HasValue && FinDiaDomingoCompl.HasValue) && !FechaValidateDomingoHasErrorCompl && SelectedAreaDomingoCompl.HasValue) ? +1 : -1;
                }
                if (CheckLunesCompl)
                {
                    count++;
                    ckLunes = ((InicioDiaLunesCompl.HasValue && FinDiaLunesCompl.HasValue) && !FechaValidateLunesHasErrorCompl && SelectedAreaLunesCompl.HasValue) ? +1 : -1;
                }
                if (CheckMartesCompl)
                {
                    count++;
                    ckMartes = ((InicioDiaMartesCompl.HasValue && FinDiaMartesCompl.HasValue) && !FechaValidateMartesHasErrorCompl && SelectedAreaMartesCompl.HasValue) ? +1 : -1;
                }
                if (CheckMiercolesCompl)
                {
                    count++;
                    ckMiercoles = ((InicioDiaMiercolesCompl.HasValue && FinDiaMiercolesCompl.HasValue) && !FechaValidateMiercolesHasErrorCompl && SelectedAreaMiercolesCompl.HasValue) ? +1 : -1;
                }
                if (CheckJuevesCompl)
                {
                    count++;
                    ckJueves = ((InicioDiaJuevesCompl.HasValue && FinDiaJuevesCompl.HasValue) && !FechaValidateJuevesHasErrorCompl && SelectedAreaJuevesCompl.HasValue) ? +1 : -1;
                }
                if (CheckViernesCompl)
                {
                    count++;
                    ckViernes = ((InicioDiaViernesCompl.HasValue && FinDiaViernesCompl.HasValue) && !FechaValidateViernesHasErrorCompl && SelectedAreaViernesCompl.HasValue) ? +1 : -1;
                }
                if (CheckSabadoCompl)
                {
                    count++;
                    ckSabado = ((InicioDiaSabadoCompl.HasValue && FinDiaSabadoCompl.HasValue) && !FechaValidateSabadoHasErrorCompl && SelectedAreaSabadoCompl.HasValue) ? +1 : -1;
                }
                return count == (ckDomingo + ckLunes + ckMartes + ckMiercoles + ckJueves + ckViernes + ckSabado);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar las horas asignadas segun los dias marcados", ex);
            }
            return false;
        }
        /// <summary>
        /// metodo que refresca la informacion de las actividades
        /// </summary>
        void RecargarActividadParticipanteCompl()
        {
            try
            {
                if (SelectedInternoCompl != null)
                    ListActividadParticipanteCompl = new ObservableCollection<ListaActividad>(GenerarListaActividadesCompl(SelectedInternoCompl.Entity));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar horario de participante", ex);
            }
        }
        /// <summary>
        /// genera lista de las actividades programadas para el imputado seleccionado
        /// </summary>
        /// <param name="iNGRESO">ingreso activo del imputado</param>
        /// <returns>listado con el horario del interno</returns>
        private List<ListaActividad> GenerarListaActividadesCompl(INGRESO iNGRESO)
        {
            var Actividades = new List<ListaActividad>();
            try
            {
                var entity = new cGrupoParticipante().GetData().Where(w => w.ID_CENTRO == iNGRESO.ID_UB_CENTRO && w.ING_ID_ANIO == iNGRESO.ID_ANIO && w.ING_ID_IMPUTADO == iNGRESO.ID_IMPUTADO && w.ING_ID_INGRESO == iNGRESO.ID_INGRESO).ToList();
                foreach (var item in entity.Where(w => w.ID_GRUPO != null).Select(s => s.GRUPO))
                {
                    IsEmpalmeCompl = new List<bool>();
                    if (!(item.ID_ACTIVIDAD == SelectedActividadCompl && item.ID_TIPO_PROGRAMA == SelectedProgramaCompl))
                        Actividades.Add(new ListaActividad()
                        {
                            NombreGrupo = item.DESCR,
                            NombreActividad = item.ACTIVIDAD.DESCR,
                            RecurrenciaActividad = item.RECURRENCIA,
                            InicioActividad = item.FEC_INICIO.Value.ToShortDateString(),
                            FinActividad = item.FEC_FIN.Value.ToShortDateString(),
                            ListHorario = ValidacionHorarioImputadoCompl(item.GRUPO_HORARIO),
                            orden = item.ACTIVIDAD.PRIORIDAD,
                            State = IsEmpalmeCompl.Where(w => w).Any() ? "Empalme" : string.Empty
                        });
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listado de actividades", ex);
            }
            return Actividades.OrderBy(o => o.orden).ToList();
        }
        /// <summary>
        /// validacion de horario del imputado para empalme
        /// </summary>
        /// <param name="collection">horario del grupo</param>
        /// <returns>lista de horas empalmadas</returns>
        private ObservableCollection<ListHorario> ValidacionHorarioImputadoCompl(ICollection<GRUPO_HORARIO> collection)
        {
            var listavalidadohorarioimputado = new ObservableCollection<ListHorario>();
            try
            {
                listavalidadohorarioimputado = new ObservableCollection<ListHorario>(collection.Where(w => (w.HORA_INICIO.Value.Date >= Fechas.GetFechaDateServer.Date) && w.ESTATUS == 1 && w.GRUPO.ID_ESTATUS_GRUPO == 1).OrderBy(o => o.HORA_INICIO).Select(s => new ListHorario()
                {
                    GrupoHorarioEntity = s,
                    AREADESCR = s.AREA.DESCR,
                    GRUPO_HORARIO_ESTATUSDESCR = s.GRUPO_HORARIO_ESTATUS.DESCR,
                    DESCRDIA = s.HORA_INICIO.Value.ToLongDateString(),
                    strHORA_INICIO = s.HORA_INICIO.Value.ToShortTimeString(),
                    strHORA_TERMIINO = s.HORA_TERMINO.Value.ToShortTimeString(),
                    State = ValidateEmpalmeCompl(s)
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar horario imputado", ex);
            }
            return listavalidadohorarioimputado;
        }
        /// <summary>
        /// metodo para marcar empalme el registro
        /// </summary>
        /// <param name="s">horario a revisar</param>
        /// <returns>estado de si esta empalmado o no</returns>
        private string ValidateEmpalmeCompl(GRUPO_HORARIO s)
        {
            try
            {
                if (PanelUpdateCompl == Visibility.Collapsed)
                {
                    if (SelectedInternoCompl != null)
                        if (ListGrupoHorarioCompl.Where(w => (s.HORA_INICIO >= w.HORA_INICIO && s.HORA_TERMINO <= w.HORA_TERMINO) && w.ESTATUS == 1 && w.GRUPO.ID_ESTATUS_GRUPO == 1).Any())
                        {
                            IsEmpalmeCompl.Add(true);
                            return "Empalme";
                        }
                    IsEmpalmeCompl.Add(false);
                    return string.Empty;
                }
                else
                {
                    #region [NUEVO]
                    if (!FechaFinCompl.HasValue)
                        return string.Empty;

                    var FECHA_INICIO = new DateTime();
                    var FECHA_FIN = new DateTime();
                    var DATE_END = FechaFinCompl.Value.AddDays(1);

                    switch (s.HORA_INICIO.Value.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            if (!InicioDiaDomingoCompl.HasValue || !FinDiaDomingoCompl.HasValue)
                            {
                                IsEmpalmeCompl.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaDomingoCompl.Value.Hour, InicioDiaDomingoCompl.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaDomingoCompl.Value.Hour, FinDiaDomingoCompl.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Monday:
                            if (!InicioDiaLunesCompl.HasValue || !FinDiaLunesCompl.HasValue)
                            {
                                IsEmpalmeCompl.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaLunesCompl.Value.Hour, InicioDiaLunesCompl.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaLunesCompl.Value.Hour, FinDiaLunesCompl.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Tuesday:
                            if (!InicioDiaMartesCompl.HasValue || !FinDiaMartesCompl.HasValue)
                            {
                                IsEmpalmeCompl.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaMartesCompl.Value.Hour, InicioDiaMartesCompl.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaMartesCompl.Value.Hour, FinDiaMartesCompl.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Wednesday:
                            if (!InicioDiaMiercolesCompl.HasValue || !FinDiaMiercolesCompl.HasValue)
                            {
                                IsEmpalmeCompl.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaMiercolesCompl.Value.Hour, InicioDiaMiercolesCompl.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaMiercolesCompl.Value.Hour, FinDiaMiercolesCompl.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Thursday:
                            if (!InicioDiaJuevesCompl.HasValue || !FinDiaJuevesCompl.HasValue)
                            {
                                IsEmpalmeCompl.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaJuevesCompl.Value.Hour, InicioDiaJuevesCompl.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaJuevesCompl.Value.Hour, FinDiaJuevesCompl.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Friday:
                            if (!InicioDiaViernesCompl.HasValue || !FinDiaViernesCompl.HasValue)
                            {
                                IsEmpalmeCompl.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaViernesCompl.Value.Hour, InicioDiaViernesCompl.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaViernesCompl.Value.Hour, FinDiaViernesCompl.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        case DayOfWeek.Saturday:
                            if (!InicioDiaSabadoCompl.HasValue || !FinDiaSabadoCompl.HasValue)
                            {
                                IsEmpalmeCompl.Add(false);
                                return string.Empty;
                            }

                            FECHA_INICIO = new DateTime(s.HORA_INICIO.Value.Year, s.HORA_INICIO.Value.Month, s.HORA_INICIO.Value.Day, InicioDiaSabadoCompl.Value.Hour, InicioDiaSabadoCompl.Value.Minute, 0);
                            FECHA_FIN = new DateTime(s.HORA_TERMINO.Value.Year, s.HORA_TERMINO.Value.Month, s.HORA_TERMINO.Value.Day, FinDiaSabadoCompl.Value.Hour, FinDiaSabadoCompl.Value.Minute, 01);

                            if (new cGrupoHorario().GetData().Where(w => (w.HORA_INICIO < FECHA_FIN && FECHA_INICIO < w.HORA_TERMINO) && w.ID_CENTRO == s.ID_CENTRO && w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.ID_GRUPO == s.ID_GRUPO && w.ID_GRUPO_HORARIO == s.ID_GRUPO_HORARIO && w.ID_AREA == s.ID_AREA && (w.HORA_INICIO >= FechaInicio || w.HORA_TERMINO <= DATE_END)).Any())
                            {
                                IsEmpalme.Add(true);
                                return "Empalme";
                            }
                            break;
                        default:
                            break;
                    }

                    IsEmpalmeCompl.Add(false);
                    return string.Empty;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar horario empalmado", ex);
            }
            return string.Empty;
        }
        /// <summary>
        /// metodo para revisar la disponibilidad del area seleccionada
        /// </summary>
        /// <param name="SelectedArea">id de el area seleccionada</param>
        /// <param name="Dia">dia marcado</param>
        /// <returns>true, false</returns>
        string ValidateAreaDisponibleCompl(short? SelectedAreaCompl, DayOfWeek Dia)
        {
            try
            {
                if (!FechaInicioCompl.HasValue)
                    return string.Empty;
                if (!FechaFinCompl.HasValue)
                    return string.Empty;
                if (!SelectedAreaCompl.HasValue)
                    return string.Empty;
                if (FechaInicioCompl > FechaFinCompl)
                    return string.Empty;

                var dates = Enumerable.Range(0, (FechaFinCompl.Value - FechaInicioCompl.Value).Days + 1).Select(d => FechaInicioCompl.Value.AddDays(d)).Where(w => w.DayOfWeek == Dia);

                if (dates.Count() <= 0)
                    return string.Empty;

                #region [grupo horario]
                var PredicadoAND = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoOR = PredicateBuilder.False<GRUPO_HORARIO>();
                var PredicadoANDD = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDL = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDMa = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDMi = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDJ = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDV = PredicateBuilder.True<GRUPO_HORARIO>();
                var PredicadoANDS = PredicateBuilder.True<GRUPO_HORARIO>();
                #endregion
                #region [atencion cita]
                var AC_PredicadoAND = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoOR = PredicateBuilder.False<ATENCION_CITA>();
                var AC_PredicadoANDD = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDL = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDMa = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDMi = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDJ = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDV = PredicateBuilder.True<ATENCION_CITA>();
                var AC_PredicadoANDS = PredicateBuilder.True<ATENCION_CITA>();
                #endregion
                #region [evento]
                var EV_PredicadoAND = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoOR = PredicateBuilder.False<EVENTO>();
                var EV_PredicadoANDD = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDL = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDMa = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDMi = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDJ = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDV = PredicateBuilder.True<EVENTO>();
                var EV_PredicadoANDS = PredicateBuilder.True<EVENTO>();
                #endregion

                var despuesde = Fechas.GetFechaDateServer.Date;
                //grupo horario
                PredicadoAND = PredicadoAND.And(w => w.ID_AREA == SelectedAreaCompl && despuesde >= w.GRUPO.FEC_INICIO.Value && w.GRUPO.ID_ESTATUS_GRUPO == 1);
                //atencion cita
                AC_PredicadoAND = AC_PredicadoAND.And(w => w.ATENCION_SOLICITUD.ID_AREA == SelectedAreaCompl && w.ATENCION_SOLICITUD.ATENCION_CITA.OrderBy(o => o.CITA_FECHA_HORA).FirstOrDefault().CITA_FECHA_HORA >= despuesde
                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1));
                //evento
                EV_PredicadoAND = EV_PredicadoAND.And(w => w.ID_AREA == SelectedAreaCompl && w.HORA_INVITACION.Value >= despuesde && w.ID_ESTATUS == 1);

                foreach (var item in dates)
                {
                    var datetoValidate = new RangeDatesValidated();
                    switch (item.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            if (!InicioDiaDomingoCompl.HasValue || !FinDiaDomingoCompl.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaDomingoCompl.Value.Hour, InicioDiaDomingoCompl.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaDomingoCompl.Value.Hour, FinDiaDomingoCompl.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDD.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDD.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDD.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Monday:
                            if (!InicioDiaLunesCompl.HasValue || !FinDiaLunesCompl.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaLunesCompl.Value.Hour, InicioDiaLunesCompl.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaLunesCompl.Value.Hour, FinDiaLunesCompl.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDL.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDL.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDL.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Tuesday:
                            if (!InicioDiaMartesCompl.HasValue || !FinDiaMartesCompl.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaMartesCompl.Value.Hour, InicioDiaMartesCompl.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaMartesCompl.Value.Hour, FinDiaMartesCompl.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDMa.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDMa.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDMa.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Wednesday:
                            if (!InicioDiaMiercolesCompl.HasValue || !FinDiaMiercolesCompl.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaMiercolesCompl.Value.Hour, InicioDiaMiercolesCompl.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaMiercolesCompl.Value.Hour, FinDiaMiercolesCompl.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDMi.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDMi.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDMi.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Thursday:
                            if (!InicioDiaJuevesCompl.HasValue || !FinDiaJuevesCompl.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaJuevesCompl.Value.Hour, InicioDiaJuevesCompl.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaJuevesCompl.Value.Hour, FinDiaJuevesCompl.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDJ.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDJ.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDJ.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Friday:
                            if (!InicioDiaViernesCompl.HasValue || !FinDiaViernesCompl.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaViernesCompl.Value.Hour, InicioDiaViernesCompl.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaViernesCompl.Value.Hour, FinDiaViernesCompl.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDV.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDV.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDV.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        case DayOfWeek.Saturday:
                            if (!InicioDiaSabadoCompl.HasValue || !FinDiaSabadoCompl.HasValue)
                                return "DEBE SELECCIONAR UN HORARIO";

                            datetoValidate = new RangeDatesValidated()
                            {
                                FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaSabadoCompl.Value.Hour, InicioDiaSabadoCompl.Value.Minute, 0),
                                FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaSabadoCompl.Value.Hour, FinDiaSabadoCompl.Value.Minute, 01)
                            };
                            //grupo horario
                            PredicadoOR = PredicadoOR.Or(PredicadoANDS.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                            //atencion cita
                            AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDS.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                            //evento
                            EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDS.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                            break;
                        default:
                            break;
                    }
                }
                if (new cGrupoHorario().GetData().AsExpandable().Where(PredicadoAND).Where(PredicadoOR.Compile()).Any() || new cAtencionCita().GetData().AsExpandable().Where(AC_PredicadoAND).Where(AC_PredicadoOR.Compile()).Any() || new cEvento().GetData().AsExpandable().Where(EV_PredicadoAND).Where(EV_PredicadoOR.Compile()).Any())
                    return "ESTE LUGAR ESTA OCUPADO A ESTA HORA";
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar la disponibilidad del area", ex);
            }
            return string.Empty;
        }
        /// <summary>
        /// metodo que valida disponibilidad del responsable
        /// </summary>
        /// <param name="SelectedResponsable">responsable seleccionado</param>
        /// <returns>valor de la disponobilidad</returns>
        string ValidateResponsableDisponibleCompl(int? SelectedResponsableCompl)
        {
            try
            {
                if (!FechaInicioCompl.HasValue)
                    return string.Empty;
                if (!FechaFinCompl.HasValue)
                    return string.Empty;
                if (!SelectedResponsableCompl.HasValue)
                    return string.Empty;
                if (FechaInicioCompl > FechaFinCompl)
                    return string.Empty;

                if (PanelUpdateCompl == Visibility.Collapsed)
                {
                    foreach (var item in new cGrupo().GetData().Where(w => w.ID_GRUPO_RESPONSABLE == SelectedResponsableCompl && w.ID_GRUPO != SelectedGrupoCompl.ID_GRUPO))
                    {
                        foreach (var subitem in item.GRUPO_HORARIO)
                            if (ListGrupoHorarioCompl.Where(w => (w.HORA_INICIO >= subitem.HORA_INICIO && w.HORA_TERMINO <= subitem.HORA_TERMINO)).Any())
                            {
                                IsEnabledCrearGrupoCompl = false;
                                return "ESTE RESPONSABLE ESTA OCUPADO PARA ESTAS FECHAS";
                            }
                    }
                    foreach (var item in new cAtencionCita().GetData().Where(w => w.ID_RESPONSABLE == SelectedResponsableCompl
                        && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)))
                    {
                        if (ListGrupoHorarioCompl.Where(w => (w.HORA_INICIO >= item.CITA_FECHA_HORA && w.HORA_TERMINO <= item.CITA_HORA_TERMINA)).Any())
                        {
                            IsEnabledCrearGrupoCompl = false;
                            return "ESTE RESPONSABLE ESTA OCUPADO PARA ESTAS FECHAS";
                        }
                    }
                    foreach (var item in new cEvento().GetData().Where(w => w.ID_PERSONA_RESPONSABLE == SelectedResponsableCompl && w.ID_ESTATUS == 1))
                    {
                        if (ListGrupoHorarioCompl.Where(w => (w.HORA_INICIO >= item.HORA_INVITACION && w.HORA_TERMINO <= item.HORA_FIN)).Any())
                        {
                            IsEnabledCrearGrupo = false;
                            return "ESTE RESPONSABLE ESTA OCUPADO PARA ESTAS FECHAS";
                        }
                    }
                    IsEnabledCrearGrupoCompl = IsOCUPANTE_MINCompl;
                    return string.Empty;
                }
                else
                {
                    var dias = new List<short>();

                    if (CheckDomingoCompl)
                        dias.Add(0);
                    if (CheckLunesCompl)
                        dias.Add(1);
                    if (CheckMartesCompl)
                        dias.Add(2);
                    if (CheckMiercolesCompl)
                        dias.Add(3);
                    if (CheckJuevesCompl)
                        dias.Add(4);
                    if (CheckViernesCompl)
                        dias.Add(5);
                    if (CheckSabadoCompl)
                        dias.Add(6);

                    var dates = Enumerable.Range(0, (FechaFinCompl.Value - FechaInicioCompl.Value).Days + 1).Select(d => FechaInicioCompl.Value.AddDays(d)).Where(w => dias.Contains((short)w.DayOfWeek));

                    if (dates.Count() <= 0)
                        return string.Empty;

                    #region [grupo horario]
                    var PredicadoAND = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoOR = PredicateBuilder.False<GRUPO_HORARIO>();
                    var PredicadoANDD = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDL = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDMa = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDMi = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDJ = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDV = PredicateBuilder.True<GRUPO_HORARIO>();
                    var PredicadoANDS = PredicateBuilder.True<GRUPO_HORARIO>();
                    #endregion
                    #region [atencioncita]
                    var AC_PredicadoAND = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoOR = PredicateBuilder.False<ATENCION_CITA>();
                    var AC_PredicadoANDD = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDL = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDMa = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDMi = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDJ = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDV = PredicateBuilder.True<ATENCION_CITA>();
                    var AC_PredicadoANDS = PredicateBuilder.True<ATENCION_CITA>();
                    #endregion
                    #region [evento]
                    var EV_PredicadoAND = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoOR = PredicateBuilder.False<EVENTO>();
                    var EV_PredicadoANDD = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDL = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDMa = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDMi = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDJ = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDV = PredicateBuilder.True<EVENTO>();
                    var EV_PredicadoANDS = PredicateBuilder.True<EVENTO>();
                    #endregion

                    var despuesde = Fechas.GetFechaDateServer.Date;
                    //grupo horario
                    PredicadoAND = PredicadoAND.And(w => w.GRUPO.ID_GRUPO_RESPONSABLE == SelectedResponsableCompl && despuesde >= w.GRUPO.FEC_INICIO.Value && w.GRUPO.ID_ESTATUS_GRUPO == 1);
                    //atencion cita
                    AC_PredicadoAND = AC_PredicadoAND.And(w => w.ID_RESPONSABLE == SelectedResponsableCompl && w.ATENCION_SOLICITUD.ATENCION_CITA.OrderBy(o => o.CITA_FECHA_HORA).FirstOrDefault().CITA_FECHA_HORA >= despuesde
                        && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1));
                    //evento
                    EV_PredicadoAND = EV_PredicadoAND.And(w => w.ID_PERSONA_RESPONSABLE == SelectedResponsableCompl && w.HORA_INVITACION.Value >= despuesde && w.ID_ESTATUS == 1);

                    foreach (var item in dates)
                    {
                        var datetoValidate = new RangeDatesValidated();
                        switch (item.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                if (!InicioDiaDomingoCompl.HasValue && !FinDiaDomingoCompl.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaDomingoCompl.Value.Hour, InicioDiaDomingoCompl.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaDomingoCompl.Value.Hour, FinDiaDomingoCompl.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDD.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDD.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDD.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Monday:
                                if (!InicioDiaLunesCompl.HasValue || !FinDiaLunesCompl.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaLunesCompl.Value.Hour, InicioDiaLunesCompl.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaLunesCompl.Value.Hour, FinDiaLunesCompl.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDL.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDL.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDL.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Tuesday:
                                if (!InicioDiaMartesCompl.HasValue || !FinDiaMartesCompl.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaMartesCompl.Value.Hour, InicioDiaMartesCompl.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaMartesCompl.Value.Hour, FinDiaMartesCompl.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDMa.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDMa.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDMa.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Wednesday:
                                if (!InicioDiaMiercolesCompl.HasValue || !FinDiaMiercolesCompl.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaMiercolesCompl.Value.Hour, InicioDiaMiercolesCompl.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaMiercolesCompl.Value.Hour, FinDiaMiercolesCompl.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDMi.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDMi.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDMi.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Thursday:
                                if (!InicioDiaJuevesCompl.HasValue || !FinDiaJuevesCompl.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaJuevesCompl.Value.Hour, InicioDiaJuevesCompl.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaJuevesCompl.Value.Hour, FinDiaJuevesCompl.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDJ.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDJ.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDJ.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Friday:
                                if (!InicioDiaViernesCompl.HasValue || !FinDiaViernesCompl.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaViernesCompl.Value.Hour, InicioDiaViernesCompl.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaViernesCompl.Value.Hour, FinDiaViernesCompl.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDV.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDV.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDV.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            case DayOfWeek.Saturday:
                                if (!InicioDiaSabadoCompl.HasValue || !FinDiaSabadoCompl.HasValue)
                                    return "DEBE SELECCIONAR UN HORARIO";

                                datetoValidate = new RangeDatesValidated()
                                {
                                    FECHA_INICIO = new DateTime(item.Year, item.Month, item.Day, InicioDiaSabadoCompl.Value.Hour, InicioDiaSabadoCompl.Value.Minute, 0),
                                    FECHA_FIN = new DateTime(item.Year, item.Month, item.Day, FinDiaSabadoCompl.Value.Hour, FinDiaSabadoCompl.Value.Minute, 01)
                                };
                                //grupo horario
                                PredicadoOR = PredicadoOR.Or(PredicadoANDS.And(w => ((w.HORA_INICIO >= datetoValidate.FECHA_INICIO) && ((w.HORA_INICIO < datetoValidate.FECHA_FIN) || (w.HORA_TERMINO < datetoValidate.FECHA_FIN))) && w.ESTATUS == 1));
                                //atencion cita
                                AC_PredicadoOR = AC_PredicadoOR.Or(AC_PredicadoANDS.And(w => (w.CITA_FECHA_HORA >= datetoValidate.FECHA_INICIO && w.CITA_HORA_TERMINA < datetoValidate.FECHA_FIN)
                                    && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)));
                                //evento
                                EV_PredicadoOR = EV_PredicadoOR.Or(EV_PredicadoANDS.And(w => (w.HORA_INVITACION >= datetoValidate.FECHA_INICIO && w.HORA_FIN < datetoValidate.FECHA_FIN) && w.ID_ESTATUS == 1));
                                break;
                            default:
                                break;
                        }
                    }
                    if (new cGrupoHorario().GetData().AsExpandable().Where(PredicadoAND).Where(PredicadoOR.Compile()).Any() || new cAtencionCita().GetData().AsExpandable().Where(AC_PredicadoAND).Where(AC_PredicadoOR.Compile()).Any() || new cEvento().GetData().AsExpandable().Where(EV_PredicadoAND).Where(EV_PredicadoOR.Compile()).Any())
                        return "ESTE RESPONSABLE ESTA OCUPADO PARA ESTAS FECHAS";
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar la disponibilidad del responsable", ex);
            }
            return string.Empty;
        }
        /// <summary>
        /// metodo auxiliar para advertir de un cambio en la calendarizacion del grupo para cancelar o proseguir
        /// </summary>
        /// <param name="variableName">nombre de la variable</param>
        /// <param name="value">valor a asignar</param>
        /// <param name="Property">nombre de la propiedad</param>
        /// <returns>resume el hilo creado</returns>
        private async Task AdvertenciaCambioCompl(string variableName, object value, string Property)
        {
            try
            {
                WaitValidatedCompl = true;
                if (SelectedCountCompl > 0)
                {
                    if (isInicioDiaCompl)
                        if (await new Dialogos().ConfirmarEliminar("Creación de Grupo Complementario", "Si cambia la calendarización del grupo afectará a los internos seleccionados, marcando en rojo aquellos que se empalmen.\nEsta operación puede tomar algunos segundos\nQuiere seguir con el cambio?") == 0)
                        {
                            var previous = this.GetType().GetProperty(Property).GetValue(this, null);
                            this.GetType().GetField(variableName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, null);
                            OnPropertyValidateChanged(Property);

                            this.GetType().GetField(variableName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, previous);
                            OnPropertyValidateChanged(Property);
                            isNeeded = false;
                            WaitValidated = false;
                            return;
                        }
                }

                this.GetType().GetField(variableName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, value);
                if (!InvalidatePropertyValidateChange)
                    OnPropertyValidateChanged(Property);
                else
                    OnPropertyChanged(Property);

                if (ListGrupoParticipanteCompl != null)
                {
                    var prevList = new ObservableCollection<ListaInternosCompl>(ListGrupoParticipanteCompl);
                    EmpalmeHorarioNuevoCompl = 0;
                    foreach (var item in prevList)
                        if (item.elegido)
                        {
                            if (GenerarListaActividadesCompl(item.Entity).Where(w => w.State.Equals("Empalme")).Any())
                            {
                                item.elegido = false;
                                item.ShowEmpalme = new Thickness(2);
                                EmpalmeHorarioNuevoCompl = new Nullable<short>();
                            }
                        }
                        else
                            item.ShowEmpalme = new Thickness(0);

                    ListGrupoParticipanteCompl = new RangeEnabledObservableCollection<ListaInternosCompl>();
                    ListGrupoParticipanteCompl.InsertRange(prevList.ToList());
                    SelectedCount = ListGrupoParticipanteCompl.Where(w => w.elegido).Count();
                    CalcularInternosSeleccionadosCompl();

                    if ("InicioDiaDomingoCompl InicioDiaLunesCompl InicioDiaMartesCompl InicioDiaMiercolesCompl InicioDiaJuevesCompl InicioDiaViernesCompl InicioDiaSabadoCompl CheckDomingoCompl CheckLunesCompl CheckMartesCompl CheckMiercolesCompl CheckJuevesCompl CheckViernesCompl CheckSabadoCompl".Contains(Property))
                        isInicioDiaCompl = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar advertencia de cambio en horario", ex);
            }
            WaitValidatedCompl = false;
            return;
        }
        /// <summary>
        /// metodo que ejecuta el evento click del checkbox del listado de horarios empalmados
        /// </summary>
        /// <param name="SelectedItem">arreglo de dos objetos [0]objeto listado interno, [1]checkbox</param>
        /// <param name="p">marcar o desmarcar casilla</param>
        private async void SetIsSelectedPropertyCompl(object SelectedItem, bool p)
        {
            try
            {
                if (selectedCountComplTrue)
                    return;

                if (((CheckBox)(((object[])(SelectedItem))[1])).IsChecked.Value)
                {
                    if (isOCUPANTE_MAXCompl)
                    {
                        ((CheckBox)(((object[])(SelectedItem))[1])).IsChecked = false;
                        return;
                    }
                    (((System.Windows.Controls.ListBoxItem)(((System.Windows.FrameworkElement)(((System.Windows.FrameworkElement)(((System.Windows.FrameworkElement)(((object[])(SelectedItem))[1])).TemplatedParent)).Parent)).TemplatedParent))).IsSelected = true;

                    if (ListActividadParticipanteCompl.Where(w => w.State.Equals("Empalme")).Any() && ListActividadParticipanteCompl.Where(w => w.Revision).Count() <= 0)
                    {
                        selectedCountComplTrue = true;
                        ((CheckBox)(((object[])(SelectedItem))[1])).IsChecked = false;
                        selectedCountComplTrue = false;

                        CalcularInternosSeleccionadosCompl();
                        ((ListaInternosCompl)(((object[])(SelectedItem))[0])).elegido = false;
                        ListaRestaurarSeleccionadosCompl.Remove(((ListaInternosCompl)(((object[])(SelectedItem))[0])));
                        return;
                    }
                }

                selectedCountComplTrue = true;
                var result = -1;
                var entityselected = ((ListaInternosCompl)(((object[])(SelectedItem))[0])).Entity;
                if (entityselected.GRUPO_PARTICIPANTE.Any())
                    if (entityselected.GRUPO_PARTICIPANTE.Where(wh => wh.ING_ID_CENTRO == entityselected.ID_UB_CENTRO && wh.ING_ID_ANIO == entityselected.ID_ANIO && wh.ING_ID_IMPUTADO == entityselected.ID_IMPUTADO && wh.ING_ID_INGRESO == entityselected.ID_INGRESO && wh.ID_TIPO_PROGRAMA == SelectedProgramaCompl && wh.ID_ACTIVIDAD == SelectedActividadCompl && wh.ID_GRUPO != null).Any())
                        result = await OpcionCancelarSuspenderInternoCompl(entityselected.GRUPO_PARTICIPANTE.Where(wh => wh.ING_ID_CENTRO == entityselected.ID_UB_CENTRO && wh.ING_ID_ANIO == entityselected.ID_ANIO && wh.ING_ID_IMPUTADO == entityselected.ID_IMPUTADO && wh.ING_ID_INGRESO == entityselected.ID_INGRESO && wh.ID_TIPO_PROGRAMA == SelectedProgramaCompl && wh.ID_ACTIVIDAD == SelectedActividadCompl).FirstOrDefault(), ((ListaInternosCompl)(((object[])(SelectedItem))[0])));

                selectedCountComplTrue = false;

                if (result != 0)
                {
                    ((ListaInternosCompl)(((object[])(SelectedItem))[0])).elegido = p;
                    if (p)
                        ListaRestaurarSeleccionadosCompl.Add(((ListaInternosCompl)(((object[])(SelectedItem))[0])));
                    else
                        ListaRestaurarSeleccionadosCompl.Remove(((ListaInternosCompl)(((object[])(SelectedItem))[0])));
                    SelectedCountCompl = ListaRestaurarSeleccionadosCompl.Count;
                    CalcularInternosSeleccionadosCompl();
                    ChangeListSelected = true;
                    if (entityselected.GRUPO_PARTICIPANTE.Where(wh => wh.ING_ID_CENTRO == entityselected.ID_UB_CENTRO && wh.ING_ID_ANIO == entityselected.ID_ANIO && wh.ING_ID_IMPUTADO == entityselected.ID_IMPUTADO && wh.ING_ID_INGRESO == entityselected.ID_INGRESO && wh.ID_TIPO_PROGRAMA == SelectedProgramaCompl && wh.ID_ACTIVIDAD == SelectedActividadCompl && wh.ID_GRUPO == null).Any())
                        StaticSourcesViewModel.SourceChanged = true;
                }
                else
                {
                    selectedCountComplTrue = true;
                    ((CheckBox)(((object[])(SelectedItem))[1])).IsChecked = true;
                    selectedCountComplTrue = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
                selectedCountComplTrue = false;
            }
        }
        /// <summary>
        /// metodo que muestra mensaje al querer desmarcar un interno que ya pertenece al grupo seleccionado
        /// </summary>
        /// <param name="gRUPO_PARTICIPANTE">objeto grupo participante del interno seleccionado</param>
        /// <param name="item">objeto de lista internos</param>
        /// <returns>regresa [0] sin accion, [1]cancelar interno, [2]suspender interno</returns>
        private async Task<int> OpcionCancelarSuspenderInternoCompl(GRUPO_PARTICIPANTE gRUPO_PARTICIPANTE, ListaInternosCompl item)
        {
            var result = 0;
            try
            {
                result = await (new Dialogos()).ConfirmacionTresBotonesDinamico("Creación de Grupos Complementarios", "Que Desea Hacer?, Cancelar o Suspender al Interno", "Cancelar", 0, "Cancelar Interno", 1, "Suspender Interno", 2);
                if (result == 0)
                    return 0;

                MotivoText = string.Empty;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.MOTIVO);
                isMotivoShow = true;
                var respuesta = false;

                await Task.Factory.StartNew(() => { do { } while (isMotivoShow); });
                if (isGuardar)
                {
                    respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando Estatus Del Interno", () =>
                    {
                        try
                        {
                            new cGrupoParticipanteCancelado().InsertarParticipanteCancelado(new GRUPO_PARTICIPANTE_CANCELADO()
                            {
                                ID_CENTRO = gRUPO_PARTICIPANTE.ID_CENTRO,
                                ID_ACTIVIDAD = gRUPO_PARTICIPANTE.ID_ACTIVIDAD,
                                ID_TIPO_PROGRAMA = gRUPO_PARTICIPANTE.ID_TIPO_PROGRAMA,
                                ID_CONSEC = gRUPO_PARTICIPANTE.ID_CONSEC,
                                ID_GRUPO = gRUPO_PARTICIPANTE.ID_GRUPO.Value,
                                ID_USUARIO = GlobalVariables.gUser,
                                SOLICITUD_FEC = Fechas.GetFechaDateServer,
                                RESPUESTA_FEC = null,
                                MOTIVO = MotivoText,
                                //4 Suspendido, 3 Cancelado
                                ID_ESTATUS = result == 1 ? (short)3 : (short)4
                            });
                            return true;
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al modificar el estatu del integrante", ex);
                            return false;
                        }
                    });

                    if (!respuesta)
                        result = 0;
                    if (respuesta)
                    {
                        ListGrupoParticipanteCompl.Remove(item);
                        CalcularInternosSeleccionadosCompl();
                        OnPropertyChanged("ListGrupoParticipanteCompl");
                        await new Dialogos().ConfirmacionDialogoReturn("Edición Grupo Complementario", "El Cambio de Estatus Se Actualizo Exitosamente");
                    }
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// metodo para revisar listado
        /// </summary>
        /// <param name="obj">control listbox</param>
        private void RevisarListadoRevisionCompl(object obj)
        {
            try
            {
                if (!(obj is ListBox))
                    return;
                if (!SelectedInternoCompl.elegido)
                    return;

                ListaRestaurarSeleccionadosCompl.Remove((ListaInternosCompl)((((System.Windows.Controls.ListBox)(obj)).SelectedItem)));
                var bkLista = ListGrupoParticipanteCompl;
                var bkselected = SelectedInternoCompl;
                var bkActividad = ListActividadParticipanteCompl;
                bkLista.Where(w => w == SelectedInternoCompl).FirstOrDefault().elegido = ListActividadParticipanteCompl.Where(w => w.Revision).Any();
                //ListGrupoParticipanteCompl.Where(w => w == SelectedInternoCompl).FirstOrDefault().elegido = ListActividadParticipanteCompl.Where(w => w.Revision).Any();
                ListGrupoParticipanteCompl = new RangeEnabledObservableCollection<ListaInternosCompl>();
                ListGrupoParticipanteCompl.InsertRange(bkLista);
                ((ListBox)obj).ItemsSource = bkLista;
                SelectedInternoCompl = bkselected;
                ListActividadParticipanteCompl = bkActividad;
                SelectedCountCompl = ListGrupoParticipanteCompl.Where(w => w.elegido).Count();
                CalcularInternosSeleccionadosCompl();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al revisar listado", ex);
            }
        }
        /// <summary>
        /// metodo que calcula los internos seleccionados
        /// </summary>
        void CalcularInternosSeleccionadosCompl()
        {
            try
            {

                var internoscancelados = 0;
                var countinternos = 0;
                if (EnabledEditGrupoCompl)
                {
                    if (SelectedGrupoCompl != null)
                        internoscancelados = new cGrupoParticipante().GetData()
                               .Where(wh => (wh.ID_GRUPO == SelectedGrupoCompl.ID_GRUPO) && (wh.ESTATUS == 1 || wh.ESTATUS == 2) && wh.ID_TIPO_PROGRAMA == SelectedProgramaCompl
                                   && wh.ID_ACTIVIDAD == SelectedActividadCompl
                                   && !wh.INGRESO.TRASLADO_DETALLE.Any(whe => (whe.ID_ESTATUS != "CA" ? whe.TRASLADO.ORIGEN_TIPO != "F" : false)) && (wh.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == wh.ID_CENTRO && whe.ID_TIPO_PROGRAMA == wh.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == wh.ID_ACTIVIDAD && whe.ID_CONSEC == wh.ID_CONSEC && whe.ID_GRUPO == wh.ID_GRUPO)).Any() ?

                               !wh.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == wh.ID_CENTRO && whe.ID_TIPO_PROGRAMA == wh.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == wh.ID_ACTIVIDAD && whe.ID_CONSEC == wh.ID_CONSEC) &&
                                   (whe.RESPUESTA_FEC == null ?

                                   (whe.RESPUESTA_FEC == null && whe.ESTATUS == 0) :

                                   (wh.GRUPO_PARTICIPANTE_CANCELADO.Where(wher => (wher.ID_CENTRO == wh.ID_CENTRO && wher.ID_TIPO_PROGRAMA == wh.ID_TIPO_PROGRAMA && wher.ID_ACTIVIDAD == wh.ID_ACTIVIDAD && wher.ID_CONSEC == wh.ID_CONSEC) && wher.RESPUESTA_FEC != null && (wher.ESTATUS == 0 || wher.ESTATUS == 2)).Count() == wh.GRUPO_PARTICIPANTE_CANCELADO.Where(wher => (wher.ID_CENTRO == wh.ID_CENTRO && wher.ID_TIPO_PROGRAMA == wh.ID_TIPO_PROGRAMA && wher.ID_ACTIVIDAD == wh.ID_ACTIVIDAD && wher.ID_CONSEC == wh.ID_CONSEC)).Count()))).Any()


                                   : false)).Count();

                    if (!totalseleccionado.HasValue)
                        countinternos = new cGrupoParticipante().GetData().Where(w => w.ID_GRUPO == SelectedGrupoCompl.ID_GRUPO).Count();

                    totalseleccionado = totalseleccionado.HasValue ? (SelectedCountCompl + internoscancelados) : countinternos;
                    isOCUPANTE_MAXCompl = totalseleccionado >= OCUPANTE_MAXCompl;
                    IsOCUPANTE_MINCompl = totalseleccionado >= OCUPANTE_MINCompl;
                }
                else
                {
                    isOCUPANTE_MAXCompl = SelectedCountCompl >= OCUPANTE_MAXCompl;
                    IsOCUPANTE_MINCompl = SelectedCountCompl >= OCUPANTE_MINCompl;
                    totalseleccionado = SelectedCountCompl;
                }
                SelectedCountTextCompl = "Minimo de " + OCUPANTE_MINCompl + ", " + totalseleccionado + "/" + OCUPANTE_MAXCompl + " Seleccionados";
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular Internos Seleccionados", ex);
            }
        }
        /// <summary>
        /// metodo para guardar grupo complementario
        /// </summary>
        private async void CrearGrupoCompl()
        {
            try
            {
                if (WaitValidatedCompl)
                {
                    WaitValidatedCompl = false;
                    return;
                }

                if (string.IsNullOrEmpty(GrupoDescripcionCompl))
                    if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "Debe Ingresar Nombre del Grupo") == 1)
                        return;

                var recurrencia = string.Empty;
                var dias = new List<short>();
                if (!(PanelUpdateCompl == Visibility.Collapsed))
                {
                    if (!FechaInicioCompl.HasValue || !FechaFinCompl.HasValue)
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "Debe Seleccionar Fecha de Inicio y Final") == 1)
                            return;

                    if (FechaValidateHasErrorCompl)
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "Las Fecha de Inicio y Final No Son Validas") == 1)
                            return;

                    if (!(CheckDomingoCompl || CheckLunesCompl || CheckMartesCompl || CheckMiercolesCompl || CheckJuevesCompl || CheckViernesCompl || CheckSabadoCompl))
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "Debe Seleccionar Recurrencia") == 1)
                            return;

                    if (((CheckDomingoCompl == (InicioDiaDomingoCompl.HasValue && FinDiaDomingoCompl.HasValue)) == false) || (CheckLunesCompl == (InicioDiaLunesCompl.HasValue && FinDiaLunesCompl.HasValue) == false) || (CheckMartesCompl == (InicioDiaMartesCompl.HasValue && FinDiaMartesCompl.HasValue) == false) || (CheckMiercolesCompl == (InicioDiaMiercolesCompl.HasValue && FinDiaMiercolesCompl.HasValue) == false) || (CheckJuevesCompl == (InicioDiaJuevesCompl.HasValue && FinDiaJuevesCompl.HasValue) == false) || (CheckViernesCompl == (InicioDiaViernesCompl.HasValue && FinDiaViernesCompl.HasValue) == false) || (CheckSabadoCompl == (InicioDiaSabadoCompl.HasValue && FinDiaSabadoCompl.HasValue) == false))
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "Debe Seleccionar Horario") == 1)
                            return;

                    if (FechaValidateDomingoHasErrorCompl || FechaValidateLunesHasErrorCompl || FechaValidateMartesHasErrorCompl || FechaValidateMiercolesHasErrorCompl || FechaValidateJuevesHasErrorCompl || FechaValidateViernesHasErrorCompl || FechaValidateSabadoHasErrorCompl)
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "El Horaio de Inicio y Final No Es Valido") == 1)
                            return;

                    if (((CheckDomingoCompl == (CheckDomingoCompl && SelectedAreaDomingoCompl.HasValue && string.IsNullOrEmpty(AreaValidateDomingoCompl))) == false) || (CheckLunesCompl == (CheckLunesCompl && SelectedAreaLunesCompl.HasValue && string.IsNullOrEmpty(AreaValidateLunesCompl)) == false) || (CheckMartesCompl == (CheckMartesCompl && SelectedAreaMartesCompl.HasValue && string.IsNullOrEmpty(AreaValidateMartesCompl)) == false) || (CheckMiercolesCompl == (CheckMiercolesCompl && SelectedAreaMiercolesCompl.HasValue && string.IsNullOrEmpty(AreaValidateMiercolesCompl)) == false) || (CheckJuevesCompl == (CheckJuevesCompl && SelectedAreaJuevesCompl.HasValue && string.IsNullOrEmpty(AreaValidateJuevesCompl)) == false) || (CheckViernesCompl == (CheckViernesCompl && SelectedAreaViernesCompl.HasValue && string.IsNullOrEmpty(AreaValidateViernesCompl)) == false) || (CheckSabadoCompl == (CheckSabadoCompl && SelectedAreaSabadoCompl.HasValue && string.IsNullOrEmpty(AreaValidateSabadoCompl)) == false))
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "Seleccione Un Area Para Continuar") == 1)
                            return;

                    var checkBoxes = new List<string>();
                    if (CheckDomingoCompl)
                        checkBoxes.Add("D");
                    if (CheckLunesCompl)
                        checkBoxes.Add("L");
                    if (CheckMartesCompl)
                        checkBoxes.Add("Ma");
                    if (CheckMiercolesCompl)
                        checkBoxes.Add("Mi");
                    if (CheckJuevesCompl)
                        checkBoxes.Add("J");
                    if (CheckViernesCompl)
                        checkBoxes.Add("V");
                    if (CheckSabadoCompl)
                        checkBoxes.Add("S");

                    foreach (var item in checkBoxes)
                    {
                        recurrencia += item + ",";
                        dias.Add(item == "D" ? (short)0 : item == "L" ? (short)1 : item == "Ma" ? (short)2 : item == "Mi" ? (short)3 : item == "J" ? (short)4 : item == "V" ? (short)5 : (short)6);
                    }
                    recurrencia = recurrencia.Substring(0, (recurrencia.Length - 1));

                    if (string.IsNullOrEmpty(recurrencia))
                        if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "Opcion de Recurrencia No Valida") == 1)
                            return;
                }

                if (!SelectedResponsableCompl.HasValue)
                    if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "Debe Seleccionar Responsable Del Grupo") == 1)
                        return;

                if (!string.IsNullOrEmpty(TextErrorResponsableCompl))
                    if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "El Responsable Seleccioinado NO Esta Disponible Para Estas Fechas") == 1)
                        return;

                if (!IsEnabledCrearGrupoCompl)
                    if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "Faltan Internos Por Seleccionar") == 1)
                        return;

                Validaciones();
                if (base.HasErrors)
                    if (await new Dialogos().ConfirmacionDialogoReturn("Creación de Grupo Complementario", "Revise Que Los Valores De Los Campos Esten Bien Ingresados") == 1)
                        return;
                ///TODO: cambiar centro por el del usuario
                ///cambiar id_departamento
                ///cambiar id_grupo_responsable

                var result = await StaticSourcesViewModel.OperacionesAsync<bool>((PanelUpdateCompl == Visibility.Collapsed ? "Actualizando" : "Creando") + " Grupo Complementario", () =>
                {
                    var idgrupo = new Nullable<short>();
                    try
                    {
                        if (!(PanelUpdateCompl == Visibility.Collapsed))
                        {
                            idgrupo = new cGrupo().Insertar(new GRUPO()
                            {
                                ID_CENTRO = GlobalVar.gCentro,//4,
                                ID_TIPO_PROGRAMA = SelectedProgramaCompl.Value,
                                ID_ACTIVIDAD = SelectedActividadCompl.Value,
                                ID_DEPARTAMENTO = 1,
                                ID_GRUPO_RESPONSABLE = SelectedResponsableCompl,
                                ID_ESTATUS_GRUPO = SelectedEstatusCompl,
                                FEC_INICIO = FechaInicioCompl,
                                FEC_FIN = FechaFinCompl,
                                RECURRENCIA = recurrencia,
                                DESCR = GrupoDescripcionCompl
                            });

                            var dates = Enumerable.Range(0, (FechaFinCompl.Value - FechaInicioCompl.Value).Days + 1).Select(d => FechaInicioCompl.Value.AddDays(d)).Where(w => dias.Contains((short)w.DayOfWeek));

                            foreach (var item in dates)
                            {
                                var hora_inicio = new DateTime();
                                var hora_fin = new DateTime();
                                var area = new short();

                                switch (item.DayOfWeek)
                                {
                                    case DayOfWeek.Sunday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaDomingoCompl.Value.Hour, InicioDiaDomingoCompl.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaDomingoCompl.Value.Hour, FinDiaDomingoCompl.Value.Minute, 0);
                                        area = SelectedAreaDomingoCompl.Value;
                                        break;
                                    case DayOfWeek.Monday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaLunesCompl.Value.Hour, InicioDiaLunesCompl.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaLunesCompl.Value.Hour, FinDiaLunesCompl.Value.Minute, 0);
                                        area = SelectedAreaLunesCompl.Value;
                                        break;
                                    case DayOfWeek.Tuesday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaMartesCompl.Value.Hour, InicioDiaMartesCompl.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaMartesCompl.Value.Hour, FinDiaMartesCompl.Value.Minute, 0);
                                        area = SelectedAreaMartesCompl.Value;
                                        break;
                                    case DayOfWeek.Wednesday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaMiercolesCompl.Value.Hour, InicioDiaMiercolesCompl.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaMiercolesCompl.Value.Hour, FinDiaMiercolesCompl.Value.Minute, 0);
                                        area = SelectedAreaMiercolesCompl.Value;
                                        break;
                                    case DayOfWeek.Thursday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaJuevesCompl.Value.Hour, InicioDiaJuevesCompl.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaJuevesCompl.Value.Hour, FinDiaJuevesCompl.Value.Minute, 0);
                                        area = SelectedAreaJuevesCompl.Value;
                                        break;
                                    case DayOfWeek.Friday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaViernesCompl.Value.Hour, InicioDiaViernesCompl.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaViernesCompl.Value.Hour, FinDiaViernesCompl.Value.Minute, 0);
                                        area = SelectedAreaViernesCompl.Value;
                                        break;
                                    case DayOfWeek.Saturday:
                                        hora_inicio = new DateTime(item.Year, item.Month, item.Day, InicioDiaSabadoCompl.Value.Hour, InicioDiaSabadoCompl.Value.Minute, 0);
                                        hora_fin = new DateTime(item.Year, item.Month, item.Day, FinDiaSabadoCompl.Value.Hour, FinDiaSabadoCompl.Value.Minute, 0);
                                        area = SelectedAreaSabadoCompl.Value;
                                        break;
                                    default:
                                        break;
                                }

                                new cGrupoHorario().Insertar(new GRUPO_HORARIO()
                                {
                                    ESTATUS = 1,
                                    HORA_INICIO = hora_inicio,
                                    HORA_TERMINO = hora_fin,
                                    ID_ACTIVIDAD = SelectedActividadCompl.Value,
                                    ID_AREA = area,
                                    ID_CENTRO = GlobalVar.gCentro,//4,
                                    ID_GRUPO = idgrupo.Value,
                                    ID_TIPO_PROGRAMA = SelectedProgramaCompl.Value
                                });
                            }
                        }

                        if (PanelUpdateCompl == Visibility.Collapsed)
                        {
                            var entityGrupo = new cGrupo().GetData().Where(w => w.ID_GRUPO == SelectedGrupoCompl.ID_GRUPO).FirstOrDefault();
                            idgrupo = entityGrupo.ID_GRUPO;
                            new cGrupo().Update(new GRUPO()
                            {
                                ID_GRUPO = entityGrupo.ID_GRUPO,
                                ID_CENTRO = entityGrupo.ID_CENTRO,
                                ID_TIPO_PROGRAMA = entityGrupo.ID_TIPO_PROGRAMA,
                                ID_ACTIVIDAD = entityGrupo.ID_ACTIVIDAD,
                                ID_DEPARTAMENTO = entityGrupo.ID_DEPARTAMENTO,
                                ID_GRUPO_RESPONSABLE = SelectedResponsableCompl,
                                ID_ESTATUS_GRUPO = SelectedEstatusCompl,
                                FEC_INICIO = entityGrupo.FEC_INICIO,
                                FEC_FIN = entityGrupo.FEC_FIN,
                                RECURRENCIA = entityGrupo.RECURRENCIA,
                                DESCR = GrupoDescripcionCompl,
                                ID_EJE = entityGrupo.ID_EJE
                            });

                            foreach (var item in new cGrupoParticipante().GetData().Where(w => w.ID_GRUPO == SelectedGrupoCompl.ID_GRUPO && (w.GRUPO_PARTICIPANTE_CANCELADO.Where(wh => (wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_CONSEC == w.ID_CONSEC && wh.ID_GRUPO == w.ID_GRUPO)).Any() ?

                            w.GRUPO_PARTICIPANTE_CANCELADO.Where(wh => (wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_CONSEC == w.ID_CONSEC) &&
                                (wh.RESPUESTA_FEC == null ?

                                (wh.RESPUESTA_FEC == null && wh.ESTATUS == 0) :

                                (w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC) && whe.RESPUESTA_FEC != null && (whe.ESTATUS == 0 || whe.ESTATUS == 2)).Count() == w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC)).Count()))).Any()


                                : true)))
                            {
                                new cGrupoParticipante().Update(new GRUPO_PARTICIPANTE()
                                {
                                    EJE = item.EJE,
                                    ESTATUS = 1,
                                    FEC_REGISTRO = item.FEC_REGISTRO,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_CONSEC = item.ID_CONSEC,
                                    ID_GRUPO = null,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    ING_ID_ANIO = item.ING_ID_ANIO,
                                    ING_ID_CENTRO = item.ING_ID_CENTRO,
                                    ING_ID_IMPUTADO = item.ING_ID_IMPUTADO,
                                    ING_ID_INGRESO = item.ING_ID_INGRESO
                                });
                            }
                        }

                        foreach (var item in ListGrupoParticipanteCompl.Where(w => w.elegido).Select(s => s.Entity))
                        {
                            if (item.GRUPO_PARTICIPANTE.Where(w => w.ID_TIPO_PROGRAMA == SelectedProgramaCompl && w.ID_ACTIVIDAD == SelectedActividadCompl && w.EJE1.COMPLEMENTARIO == "S").Any())
                            {
                                foreach (var subitem in item.GRUPO_PARTICIPANTE.Where(w => w.ID_TIPO_PROGRAMA == SelectedProgramaCompl && w.ID_ACTIVIDAD == SelectedActividadCompl && w.EJE1.COMPLEMENTARIO == "S"))
                                {
                                    var entityGP = new cGrupoParticipante().ActualizarParticipanteAsistencia(new GRUPO_PARTICIPANTE()
                                    {
                                        EJE = subitem.EJE,
                                        ESTATUS = 2,
                                        FEC_REGISTRO = subitem.FEC_REGISTRO,
                                        ID_ACTIVIDAD = subitem.ID_ACTIVIDAD,
                                        ID_CENTRO = subitem.ID_CENTRO,
                                        ID_CONSEC = subitem.ID_CONSEC,
                                        ID_GRUPO = idgrupo,
                                        ID_TIPO_PROGRAMA = subitem.ID_TIPO_PROGRAMA,
                                        ING_ID_ANIO = subitem.ING_ID_ANIO,
                                        ING_ID_CENTRO = subitem.ING_ID_CENTRO,
                                        ING_ID_IMPUTADO = subitem.ING_ID_IMPUTADO,
                                        ING_ID_INGRESO = subitem.ING_ID_INGRESO
                                    });

                                    foreach (var ListGH in new cGrupo().GetData().Where(w => w.ID_GRUPO == entityGP.ID_GRUPO).FirstOrDefault().GRUPO_HORARIO)
                                    {
                                        if (!new cGrupoAsistencia().GetData().Where(w => w.ID_GRUPO == entityGP.ID_GRUPO && w.ID_CONSEC == entityGP.ID_CONSEC && w.ID_GRUPO_HORARIO == ListGH.ID_GRUPO_HORARIO).Any())
                                            new cGrupoAsistencia().Insert(new GRUPO_ASISTENCIA()
                                            {
                                                ID_CENTRO = GlobalVar.gCentro,
                                                ID_TIPO_PROGRAMA = ListGH.ID_TIPO_PROGRAMA,
                                                ID_ACTIVIDAD = ListGH.ID_ACTIVIDAD,
                                                ID_GRUPO = idgrupo.Value,
                                                ID_GRUPO_HORARIO = ListGH.ID_GRUPO_HORARIO,
                                                ID_CONSEC = entityGP.ID_CONSEC,
                                                FEC_REGISTRO = Fechas.GetFechaDateServer,
                                                ASISTENCIA = null,
                                                EMPALME = 0,
                                                EMP_COORDINACION = 0,
                                                EMP_APROBADO = null,
                                                EMP_FECHA = null,
                                                ESTATUS = 1
                                            });
                                    }
                                }
                            }
                            else
                            {
                                var entityGP = new cGrupoParticipante().InsertarParticipanteAsistencia(new GRUPO_PARTICIPANTE()
                                  {
                                      ID_CENTRO = item.ID_CENTRO,
                                      ID_TIPO_PROGRAMA = SelectedProgramaCompl.Value,
                                      ID_ACTIVIDAD = SelectedActividadCompl.Value,
                                      ID_GRUPO = idgrupo,
                                      ING_ID_CENTRO = item.ID_CENTRO,
                                      ING_ID_ANIO = item.ID_ANIO,
                                      ING_ID_IMPUTADO = item.ID_IMPUTADO,
                                      ING_ID_INGRESO = item.ID_INGRESO,
                                      FEC_REGISTRO = Fechas.GetFechaDateServer,
                                      ESTATUS = (short)2,
                                      EJE = new cActividadEje().GetData(w => w.ID_ACTIVIDAD == SelectedActividadCompl.Value && w.ID_TIPO_PROGRAMA == SelectedProgramaCompl.Value).FirstOrDefault().ID_EJE
                                  });

                                foreach (var ListGH in new cGrupo().GetData().Where(w => w.ID_GRUPO == entityGP.ID_GRUPO).FirstOrDefault().GRUPO_HORARIO)
                                    new cGrupoAsistencia().Insert(new GRUPO_ASISTENCIA()
                                    {
                                        ID_CENTRO = GlobalVar.gCentro,
                                        ID_TIPO_PROGRAMA = ListGH.ID_TIPO_PROGRAMA,
                                        ID_ACTIVIDAD = ListGH.ID_ACTIVIDAD,
                                        ID_GRUPO = idgrupo.Value,
                                        ID_GRUPO_HORARIO = ListGH.ID_GRUPO_HORARIO,
                                        ID_CONSEC = entityGP.ID_CONSEC,
                                        FEC_REGISTRO = Fechas.GetFechaDateServer,
                                        ASISTENCIA = null,
                                        EMPALME = 0,
                                        EMP_COORDINACION = 0,
                                        EMP_APROBADO = null,
                                        EMP_FECHA = null,
                                        ESTATUS = 1
                                    });
                            }
                        }

                        if (ListGrupoParticipanteCompl.Where(w => w.elegido && w.HorarioInterno != null && w.HorarioInterno.Where(wh => wh.State.Equals("Empalme") && wh.Revision && wh.ListHorario.Where(whe => whe.State.Equals("Empalme")).Any()).Any()).Any())
                        {
                            var listGA = new List<ListaEmpalmesInterno>();
                            var entitylistGH = new List<GRUPO_HORARIO>();
                            foreach (var item in ListGrupoParticipanteCompl.Where(w => w.elegido && w.HorarioInterno != null && w.HorarioInterno.Where(wh => wh.State.Equals("Empalme") && wh.Revision && wh.ListHorario.Where(whe => whe.State.Equals("Empalme")).Any()).Any()))
                            {
                                foreach (var itemLisAct in item.HorarioInterno.Where(w => w.ListHorario.Where(whe => whe.State.Equals("Empalme")).Any()))
                                    entitylistGH.AddRange(itemLisAct.ListHorario.Where(w => w.State.Equals("Empalme")).Select(s => s.GrupoHorarioEntity).ToList());
                                listGA.Add(new ListaEmpalmesInterno()
                                {
                                    EntityGrupoParticipante = new cGrupoParticipante().GetData().Where(w =>
                                        w.ING_ID_CENTRO == item.Entity.ID_CENTRO &&
                                        w.ING_ID_ANIO == item.Entity.ID_ANIO &&
                                        w.ING_ID_IMPUTADO == item.Entity.ID_IMPUTADO &&
                                        w.ING_ID_INGRESO == item.Entity.ID_INGRESO &&
                                        w.ID_TIPO_PROGRAMA == SelectedProgramaCompl.Value &&
                                        w.ID_ACTIVIDAD == SelectedActividadCompl.Value &&
                                        w.ID_GRUPO == idgrupo)
                                        .FirstOrDefault(),
                                    ListGrupoHorario = entitylistGH
                                });
                            }
                            new cGrupoAsistencia().GenerarEmpalmes(listGA);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (idgrupo.HasValue)
                            new cGrupo().CrearGrupoRollback(idgrupo.Value);
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al " + (PanelUpdateCompl == Visibility.Collapsed ? "Actualizar" : "Crear") + " Grupo Complementario", ex);
                        return false;
                    }
                    return true;
                });

                if (result)
                {
                    LimpiarCamposCompl();
                    ListGrupoCompl = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<GRUPO>>(() =>
                    {
                        return new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == SelectedProgramaCompl && w.ID_ACTIVIDAD == SelectedActividadCompl.Value).Select(s => s).Distinct().OrderBy(o => o.DESCR).ToList());
                    });
                    if (PanelUpdateCompl == Visibility.Collapsed)
                        await new Dialogos().ConfirmacionDialogoReturn("Crear Grupo", "Grupo Complementario Actualizado Exitosamente");
                    else
                    {
                        ListaInternosComplLoad(SelectedActividadCompl.Value);
                        await new Dialogos().ConfirmacionDialogoReturn("Crear Grupo", "Grupo Complementario Creado Exitosamente");
                    }
                    StaticSourcesViewModel.SourceChanged = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al " + (PanelUpdateCompl == Visibility.Collapsed ? "Actualizar" : "Crear") + " Grupo Complementario", ex);
            }
        }
        //limpiar tab grupo complementario
        private void LimpiarCamposCompl()
        {
            try
            {
                StaticSourcesViewModel.SourceChanged = false;
                InvalidatePropertyValidateChange = true;
                #region [COMPLEMENTARIAS]
                SelectedDelitoCompl = null;
                SelectedEdades = null;
                SelectedPlanimetriaCompl = null;
                SelectedAniosCompl = null;
                SelectedGrupoCompl = null;
                SelectedCountCompl = 0;
                ListGrupoParticipanteCompl = null;
                InicioDiaDomingoCompl = FinDiaDomingoCompl = InicioDiaLunesCompl = FinDiaLunesCompl = InicioDiaMartesCompl = FinDiaMartesCompl = InicioDiaMiercolesCompl = FinDiaMiercolesCompl = InicioDiaJuevesCompl = FinDiaJuevesCompl = InicioDiaViernesCompl = FinDiaViernesCompl = InicioDiaSabadoCompl = FinDiaSabadoCompl = FechaInicioCompl = FechaFinCompl = null;
                SelectedEstatusCompl = SelectedAreaDomingoCompl = SelectedAreaLunesCompl = SelectedAreaMartesCompl = SelectedAreaMiercolesCompl = SelectedAreaJuevesCompl = SelectedAreaViernesCompl = SelectedAreaSabadoCompl = null;
                SelectedResponsableCompl = null;
                selectedCountComplTrue = isOCUPANTE_MAXCompl = IsOCUPANTE_MINCompl = false;
                MensajeTextCompl = SelectedCountTextCompl = GrupoDescripcionCompl = string.Empty;
                CheckDomingoCompl = CheckLunesCompl = CheckMartesCompl = CheckMiercolesCompl = CheckJuevesCompl = CheckViernesCompl = CheckSabadoCompl = false;
                ListaRestaurarSeleccionadosCompl = new ObservableCollection<ListaInternosCompl>();
                #endregion
                InvalidatePropertyValidateChange = false;
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Limpiar Campos", ex);
            }
        }
        #endregion
        ///CALCULAR TIEMPOS DE SENTENCIA Y COMPURGACION
        private string CalcularSentencia(ICollection<CAUSA_PENAL> CausaPenal)
        {
            try
            {
                if (CausaPenal != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (CausaPenal != null)
                    {
                        foreach (var cp in CausaPenal)
                        {
                            var segundaInstancia = false;
                            if (cp.SENTENCIA != null)
                            {
                                if (cp.SENTENCIA.Count > 0)
                                {
                                    //BUSCAMOS SI TIENE 2DA INSTANCIA
                                    if (cp.RECURSO.Count > 0)
                                    {
                                        var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
                                        if (r != null)
                                        {
                                            var res = r.FirstOrDefault();
                                            if (res != null)
                                            {
                                                //SENTENCIA
                                                anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
                                                meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
                                                dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);

                                                segundaInstancia = true;
                                            }
                                        }
                                    }
                                    var s = cp.SENTENCIA.FirstOrDefault();
                                    if (s != null)
                                    {
                                        if (FechaInicioCompurgacion == null)
                                        {
                                            FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        }
                                        else
                                        {
                                            if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        }

                                        //SENTENCIA
                                        if (!segundaInstancia)
                                        {
                                            anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
                                            meses = meses + (s.MESES != null ? s.MESES.Value : 0);
                                            dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);
                                        }

                                        //ABONO
                                        anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
                                        meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
                                        dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
                                    }
                                }
                            }
                        }
                    }

                    while (dias > 29)
                    {
                        meses++;
                        dias = dias - 30;
                    }
                    while (meses > 11)
                    {
                        anios++;
                        meses = meses - 12;
                    }

                    if (FechaInicioCompurgacion != null)
                    {
                        FechaFinCompurgacion = FechaInicioCompurgacion;
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
                        //
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

                        int a = 0, m = 0, d = 0;
                        new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                        a = m = d = 0;
                        new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);

                        varauxSentencia = anios + (anios == 1 ? " Año " : " Años ") + meses + (meses == 1 ? " Mes " : " Meses ") + dias + (dias == 1 ? " Dia" : " Dias");
                        return a + (a == 1 ? "_Año_" : "_Años_") + m + (m == 1 ? "_Mes_" : "_Meses_") + d + (d == 1 ? "_Dia" : "_Dias");
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
            return string.Empty;
        }
        /// <summary>
        /// metodo que maneja las opciones del modulo
        /// </summary>
        /// <param name="obj"></param>
        private async void clickSwitch(object obj)
        {
            try
            {
                if (obj.ToString() != "menu_guardar" && (obj.ToString() != "VerSeleccionados" && obj.ToString() != "cancelar_verseleccion") && obj.ToString() != "cancelar_Motivo" && obj.ToString() != "guardar_Motivo")
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                switch (obj.ToString())
                {
                    case "menu_guardar":
                        if (!SelectedTabComplementario)
                        {
                            if (EnabledPanelCrearGrupo)
                                CrearGrupo();
                        }
                        else
                        {
                            if (EnabledPanelCrearGrupoCompl)
                                CrearGrupoCompl();
                        }
                        break;
                    case "menu_agregar":
                        if (!SelectedTabComplementario)
                        {
                            EnabledPanelCrearGrupo = SelectedEje.HasValue && SelectedPrograma.HasValue && SelectedActividad.HasValue;
                            EnabledEditGrupo = false;
                            PanelUpdate = Visibility.Visible;
                            SelectedActividad = null;
                            LimpiarCampos();
                        }
                        else
                        {
                            EnabledPanelCrearGrupoCompl = SelectedProgramaCompl.HasValue && SelectedActividadCompl.HasValue;
                            EnabledEditGrupoCompl = false;
                            PanelUpdateCompl = Visibility.Visible;
                            SelectedActividadCompl = null;
                            LimpiarCamposCompl();
                        }
                        break;
                    case "menu_eliminar":
                        break;
                    case "menu_editar":
                        if (!SelectedTabComplementario)
                        {
                            EnabledPanelCrearGrupo = false;
                            EnabledEditGrupo = true;
                            PanelUpdate = Visibility.Collapsed;
                            LimpiarCampos();
                        }
                        else
                        {
                            EnabledPanelCrearGrupoCompl = false;
                            EnabledEditGrupoCompl = true;
                            PanelUpdateCompl = Visibility.Collapsed;
                            LimpiarCamposCompl();
                        }
                        break;
                    case "menu_cancelar":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = null;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = null;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CreacionGruposView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new CreacionGruposViewModel(SelectedTabComplementario);
                        break;
                    case "menu_ayuda":
                        break;
                    case "menu_salir":
                        PrincipalViewModel.SalirMenu();
                        break;
                    case "VerSeleccionados":
                        ChangeListSelected = false;
                        ListaRestaurarSeleccionadosCompl = new ObservableCollection<ListaInternosCompl>(ListaRestaurarSeleccionadosCompl.OrderBy(o => o.FOLIO));
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VER_SELECCIONADOSCOMPL);
                        break;
                    case "cancelar_verseleccion":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VER_SELECCIONADOSCOMPL);
                        if (ChangeListSelected)
                        {
                            ListGrupoParticipanteCompl = new RangeEnabledObservableCollection<ListaInternosCompl>();
                            ListGrupoParticipanteCompl.InsertRange(await SegmentarResultadoParticipantes());
                            CalcularInternosSeleccionadosCompl();
                        }
                        break;
                    case "guardar_Motivo":
                        if (string.IsNullOrEmpty(MotivoText.Trim()))
                            break;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.MOTIVO);
                        isGuardar = true;
                        isMotivoShow = false;
                        break;
                    case "cancelar_Motivo":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.MOTIVO);
                        isGuardar = isMotivoShow = false;
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
        /// <summary>
        /// validaciones del modulo
        /// </summary>
        void Validaciones()
        {
            try
            {
                base.ClearRules();
                if (!SelectedTabComplementario)
                {
                    #region [GRUPO]
                    base.AddRule(() => GrupoDescripcion, () => !string.IsNullOrEmpty(GrupoDescripcion), "NOMBRE DEL GRUPO REQUERIDO");
                    OnPropertyChanged("GrupoDescripcion");

                    base.AddRule(() => FechaFin, () => !FechaValidateHasError, FechaValidate);
                    OnPropertyChanged("FechaFin");

                    base.AddRule(() => FinDiaDomingo, () => !FechaValidateDomingoHasError, FechaValidateDomingo);
                    OnPropertyChanged("FinDiaDomingo");
                    base.AddRule(() => FinDiaLunes, () => !FechaValidateLunesHasError, FechaValidateLunes);
                    OnPropertyChanged("FinDiaLunes");
                    base.AddRule(() => FinDiaMartes, () => !FechaValidateMartesHasError, FechaValidateMartes);
                    OnPropertyChanged("FinDiaMartes");
                    base.AddRule(() => FinDiaMiercoles, () => !FechaValidateMiercolesHasError, FechaValidateMiercoles);
                    OnPropertyChanged("FinDiaMiercoles");
                    base.AddRule(() => FinDiaJueves, () => !FechaValidateJuevesHasError, FechaValidateJueves);
                    OnPropertyChanged("FinDiaJueves");
                    base.AddRule(() => FinDiaViernes, () => !FechaValidateViernesHasError, FechaValidateViernes);
                    OnPropertyChanged("FinDiaViernes");
                    base.AddRule(() => FinDiaSabado, () => !FechaValidateSabadoHasError, FechaValidateSabado);
                    OnPropertyChanged("FinDiaSabado");

                    base.AddRule(() => SelectedAreaDomingo, () => string.IsNullOrEmpty(AreaValidateDomingo), AreaValidateDomingo);
                    OnPropertyChanged("SelectedAreaDomingo");
                    base.AddRule(() => SelectedAreaLunes, () => string.IsNullOrEmpty(AreaValidateLunes), AreaValidateLunes);
                    OnPropertyChanged("SelectedAreaLunes");
                    base.AddRule(() => SelectedAreaMartes, () => string.IsNullOrEmpty(AreaValidateMartes), AreaValidateMartes);
                    OnPropertyChanged("SelectedAreaMartes");
                    base.AddRule(() => SelectedAreaMiercoles, () => string.IsNullOrEmpty(AreaValidateMiercoles), AreaValidateMiercoles);
                    OnPropertyChanged("SelectedAreaMiercoles");
                    base.AddRule(() => SelectedAreaJueves, () => string.IsNullOrEmpty(AreaValidateJueves), AreaValidateJueves);
                    OnPropertyChanged("SelectedAreaJueves");
                    base.AddRule(() => SelectedAreaViernes, () => string.IsNullOrEmpty(AreaValidateViernes), AreaValidateViernes);
                    OnPropertyChanged("SelectedAreaViernes");
                    base.AddRule(() => SelectedAreaSabado, () => string.IsNullOrEmpty(AreaValidateSabado), AreaValidateSabado);
                    OnPropertyChanged("SelectedAreaSabado");

                    base.AddRule(() => SelectedResponsable, () => string.IsNullOrEmpty(TextErrorResponsable), TextErrorResponsable);
                    OnPropertyChanged("SelectedResponsable");
                    #endregion
                }
                else
                {
                    #region [COMPLEMENTARIAS]
                    base.AddRule(() => GrupoDescripcionCompl, () => !string.IsNullOrEmpty(GrupoDescripcionCompl), "NOMBRE DEL GRUPO REQUERIDO");
                    OnPropertyChanged("GrupoDescripcionCompl");

                    base.AddRule(() => FechaFinCompl, () => !FechaValidateHasErrorCompl, FechaValidateCompl);
                    OnPropertyChanged("FechaFinCompl");

                    base.AddRule(() => FinDiaDomingoCompl, () => !FechaValidateDomingoHasErrorCompl, FechaValidateDomingo);
                    OnPropertyChanged("FinDiaDomingoCompl");
                    base.AddRule(() => FinDiaLunesCompl, () => !FechaValidateLunesHasErrorCompl, FechaValidateLunesCompl);
                    OnPropertyChanged("FinDiaLunesCompl");
                    base.AddRule(() => FinDiaMartesCompl, () => !FechaValidateMartesHasErrorCompl, FechaValidateMartesCompl);
                    OnPropertyChanged("FinDiaMartesCompl");
                    base.AddRule(() => FinDiaMiercolesCompl, () => !FechaValidateMiercolesHasErrorCompl, FechaValidateMiercolesCompl);
                    OnPropertyChanged("FinDiaMiercolesCompl");
                    base.AddRule(() => FinDiaJuevesCompl, () => !FechaValidateJuevesHasErrorCompl, FechaValidateJuevesCompl);
                    OnPropertyChanged("FinDiaJuevesCompl");
                    base.AddRule(() => FinDiaViernesCompl, () => !FechaValidateViernesHasErrorCompl, FechaValidateViernesCompl);
                    OnPropertyChanged("FinDiaViernesCompl");
                    base.AddRule(() => FinDiaSabadoCompl, () => !FechaValidateSabadoHasErrorCompl, FechaValidateSabadoCompl);
                    OnPropertyChanged("FinDiaSabadoCompl");

                    base.AddRule(() => SelectedAreaDomingoCompl, () => string.IsNullOrEmpty(AreaValidateDomingoCompl), AreaValidateDomingoCompl);
                    OnPropertyChanged("SelectedAreaDomingoCompl");
                    base.AddRule(() => SelectedAreaLunesCompl, () => string.IsNullOrEmpty(AreaValidateLunesCompl), AreaValidateLunesCompl);
                    OnPropertyChanged("SelectedAreaLunesCompl");
                    base.AddRule(() => SelectedAreaMartesCompl, () => string.IsNullOrEmpty(AreaValidateMartesCompl), AreaValidateMartesCompl);
                    OnPropertyChanged("SelectedAreaMartesCompl");
                    base.AddRule(() => SelectedAreaMiercolesCompl, () => string.IsNullOrEmpty(AreaValidateMiercolesCompl), AreaValidateMiercolesCompl);
                    OnPropertyChanged("SelectedAreaMiercolesCompl");
                    base.AddRule(() => SelectedAreaJuevesCompl, () => string.IsNullOrEmpty(AreaValidateJuevesCompl), AreaValidateJuevesCompl);
                    OnPropertyChanged("SelectedAreaJuevesCompl");
                    base.AddRule(() => SelectedAreaViernesCompl, () => string.IsNullOrEmpty(AreaValidateViernesCompl), AreaValidateViernesCompl);
                    OnPropertyChanged("SelectedAreaViernesCompl");
                    base.AddRule(() => SelectedAreaSabadoCompl, () => string.IsNullOrEmpty(AreaValidateSabadoCompl), AreaValidateSabadoCompl);
                    OnPropertyChanged("SelectedAreaSabadoCompl");

                    base.AddRule(() => SelectedResponsableCompl, () => string.IsNullOrEmpty(TextErrorResponsableCompl), TextErrorResponsableCompl);
                    OnPropertyChanged("SelectedResponsableCompl");
                    #endregion
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en las validaciones", ex);
            }
        }
        /// <summary>
        /// evento de cambio de tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCreacionGrupo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnTabChanged(e);
        }
        /// <summary>
        /// logica en cambio de tab
        /// </summary>
        /// <param name="e"></param>
        private async void OnTabChanged(object e)
        {
            try
            {
                if (((SelectionChangedEventArgs)e).OriginalSource != Tab)
                    return;

                if (StaticSourcesViewModel.SourceChanged)
                {
                    if (cancellingTabSelectionChange)
                        return;
                    ValidaCambioTab = false;
                    if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                    {
                        InvalidatePropertyValidateChange = true;
                        StaticSourcesViewModel.SourceChanged = false;
                        SelectedEje = null;
                        SelectedPrograma = null;
                        SelectedActividad = null;
                        SelectedGrupo = null;
                        SelectedProgramaCompl = null;
                        SelectedActividadCompl = null;
                        SelectedGrupoCompl = null;
                        LimpiarCampos();
                        LimpiarCamposCompl();
                        StaticSourcesViewModel.SourceChanged = false;
                        InvalidatePropertyValidateChange = false;
                    }
                    else
                    {
                        cancellingTabSelectionChange = true;
                        ValidaCambioTab = true;
                        Tab.SelectedItem = ((SelectionChangedEventArgs)e).RemovedItems[0];
                        cancellingTabSelectionChange = false;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambiar tab", ex);
            }
        }
    }

    public class RangeDatesValidated
    {
        public DateTime FECHA_INICIO { get; set; }
        public DateTime FECHA_FIN { get; set; }
    }
}
