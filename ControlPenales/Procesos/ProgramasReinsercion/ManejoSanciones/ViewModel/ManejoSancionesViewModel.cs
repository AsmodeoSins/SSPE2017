using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ManejoSancionesViewModel
    {
        /* [descripcion de clase]
         * modulo de manejo cancelados/suspendidos
         * 
         * en este modulo se tomara la decicion de aprovar la accion requerida, ya sea cancelar o suspender al interno del grupo
         * 
         * cuidar cuando se cancele el interno se borre sus horas en el grupo 
         * 
         */

        ///TODO: ingresar el usuario para el filtro de responsable... TODO!!

        /// <summary>
        /// metodo que tiene las funciones de los eventos de click
        /// </summary>
        /// <param name="obj">nombre del parametro del evento</param>
        private async void clickSwitch(Object obj)
        {
            try
            {
                if (obj.ToString() != "menu_guardar")
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                switch (obj.ToString())
                {
                    case "menu_cancelar":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = null;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = null;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ManejoSancionesView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ManejoSancionesViewModel();
                        break;
                    case "menu_guardar":
                        //guarda evaluacion de requerimiento
                        if (!SelectedTabSuspendidos)
                            GuardarDesicionesBandeja();
                        else
                            GuardarReactivados();
                        break;
                    case "menu_salir":
                        PrincipalViewModel.SalirMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo.", ex);
            }
        }
        /// <summary>
        /// metodo que hace las operacion segun el estatus a aprovar del interno, cuando se cancela se borra el horario del interno
        /// </summary>
        private async void GuardarDesicionesBandeja()
        {
            try
            {
                if (ListParticipantes == null)
                    return;
                if (ListParticipantes.Where(w => string.IsNullOrEmpty(w.RESPUESTA_TECNICA)).Any())
                {
                    await new Dialogos().ConfirmacionDialogoReturn("Manejo Sanciones", "Faltan Participantes Por Ser Evaluados");
                    return;
                }

                if (await new Dialogos().ConfirmarEliminar("Manejo Sanciones", "Esta Seguro De Guardar?") == 0)
                    return;

                var dal = new cGrupoParticipanteCancelado();
                var fechaserver = Fechas.GetFechaDateServer;
                foreach (var item in ListParticipantes)
                {
                    dal.Update(new GRUPO_PARTICIPANTE_CANCELADO()
                    {
                        ID_CENTRO = item.Entity.ID_CENTRO,
                        ID_TIPO_PROGRAMA = item.Entity.ID_TIPO_PROGRAMA,
                        ID_ACTIVIDAD = item.Entity.ID_ACTIVIDAD,
                        ID_CONSEC = item.Entity.ID_CONSEC,
                        ID_CONS_CANCELADO = item.Entity.ID_CONS_CANCELADO,
                        ID_GRUPO = item.Entity.ID_GRUPO,
                        ID_USUARIO = item.Entity.ID_USUARIO,
                        SOLICITUD_FEC = item.Entity.SOLICITUD_FEC,
                        RESPUESTA_FEC = fechaserver,
                        MOTIVO = item.Entity.MOTIVO,
                        ID_ESTATUS = item.Entity.ID_ESTATUS,
                        //1 aceptado, 0 rechazado
                        ESTATUS = item.APROBADO ? (short)1 : (short)0,
                        RESPUESTA = item.RESPUESTA_TECNICA
                    });
                }

                var dalGP = new cGrupoParticipante();
                var dalGA = new cGrupoAsistencia();
                foreach (var item in ListParticipantes.Where(w => w.APROBADO))
                {
                    dalGP.Update(new GRUPO_PARTICIPANTE()
                    {
                        EJE = item.Entity.GRUPO_PARTICIPANTE.EJE,
                        ESTATUS = item.Entity.ID_ESTATUS == 3 ? (short)3 : item.Entity.ID_ESTATUS == 4 ? (short)4 : (short)2,
                        FEC_REGISTRO = item.Entity.GRUPO_PARTICIPANTE.FEC_REGISTRO,
                        ID_ACTIVIDAD = item.Entity.GRUPO_PARTICIPANTE.ID_ACTIVIDAD,
                        ID_CENTRO = item.Entity.GRUPO_PARTICIPANTE.ID_CENTRO,
                        ID_CONSEC = item.Entity.GRUPO_PARTICIPANTE.ID_CONSEC,
                        ID_GRUPO = item.Entity.ID_ESTATUS == 3 ? new Nullable<short>() : item.Entity.ID_ESTATUS == 4 ? item.Entity.ID_GRUPO : item.Entity.ID_GRUPO,
                        ID_TIPO_PROGRAMA = item.Entity.GRUPO_PARTICIPANTE.ID_TIPO_PROGRAMA,
                        ING_ID_ANIO = item.Entity.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                        ING_ID_CENTRO = item.Entity.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                        ING_ID_IMPUTADO = item.Entity.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                        ING_ID_INGRESO = item.Entity.GRUPO_PARTICIPANTE.ING_ID_INGRESO
                    });

                    //se borra horario y posibles empalmes
                    if (item.Entity.ID_ESTATUS == 3)
                    {
                        dalGA.EliminarEmpalme(new GRUPO_ASISTENCIA()
                        {
                            ID_CENTRO = item.Entity.ID_CENTRO,
                            ID_TIPO_PROGRAMA = item.Entity.ID_TIPO_PROGRAMA,
                            ID_ACTIVIDAD = item.Entity.ID_ACTIVIDAD,
                            ID_GRUPO = item.Entity.ID_GRUPO,
                            ID_CONSEC = item.Entity.ID_CONSEC
                        });

                    }
                }

                StaticSourcesViewModel.SourceChanged = false;
                ListaInternosAEvaluar(SelectedGrupo);
                await new Dialogos().ConfirmacionDialogoReturn("Manejo Sanciones", "Participantes Evaluados Exitosamente");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al evaluar a participantes", ex);
            }
        }
        /// <summary>
        /// metodo que reactiva a internos suspendidos previamente marcados como reactivados
        /// </summary>
        private async void GuardarReactivados()
        {
            try
            {
                if (SuspenListParticipantes == null)
                    return;
                if (SuspenListParticipantes.Where(w => w.REACTIVAR).Count() <= 0)
                    return;

                if (await new Dialogos().ConfirmarEliminar("Manejo Sanciones", "Esta Seguro De Guardar?") == 0)
                    return;

                var dal = new cGrupoParticipanteCancelado();
                foreach (var item in SuspenListParticipantes.Where(w => w.REACTIVAR))
                {
                    dal.Update(new GRUPO_PARTICIPANTE_CANCELADO()
                    {
                        ID_CENTRO = item.Entity.ID_CENTRO,
                        ID_TIPO_PROGRAMA = item.Entity.ID_TIPO_PROGRAMA,
                        ID_ACTIVIDAD = item.Entity.ID_ACTIVIDAD,
                        ID_CONSEC = item.Entity.ID_CONSEC,
                        ID_CONS_CANCELADO = item.Entity.ID_CONS_CANCELADO,
                        ID_GRUPO = item.Entity.ID_GRUPO,
                        ID_USUARIO = item.Entity.ID_USUARIO,
                        SOLICITUD_FEC = item.Entity.SOLICITUD_FEC,
                        RESPUESTA_FEC = item.RESPUESTA_FECHA,
                        MOTIVO = item.Entity.MOTIVO,
                        ID_ESTATUS = item.Entity.ID_ESTATUS,
                        //1 aceptado, 0 rechazado, 2 reactivado
                        ESTATUS = item.REACTIVAR ? (short)2 : item.Entity.ESTATUS,
                        RESPUESTA = item.RESPUESTA_TECNICA
                    });
                }

                var dalGP = new cGrupoParticipante();
                foreach (var item in SuspenListParticipantes.Where(w => w.REACTIVAR))
                {
                    dalGP.Update(new GRUPO_PARTICIPANTE()
                    {
                        EJE = item.Entity.GRUPO_PARTICIPANTE.EJE,
                        ESTATUS = 2,
                        FEC_REGISTRO = item.Entity.GRUPO_PARTICIPANTE.FEC_REGISTRO,
                        ID_ACTIVIDAD = item.Entity.GRUPO_PARTICIPANTE.ID_ACTIVIDAD,
                        ID_CENTRO = item.Entity.GRUPO_PARTICIPANTE.ID_CENTRO,
                        ID_CONSEC = item.Entity.GRUPO_PARTICIPANTE.ID_CONSEC,
                        ID_GRUPO = item.Entity.ID_GRUPO,
                        ID_TIPO_PROGRAMA = item.Entity.GRUPO_PARTICIPANTE.ID_TIPO_PROGRAMA,
                        ING_ID_ANIO = item.Entity.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                        ING_ID_CENTRO = item.Entity.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                        ING_ID_IMPUTADO = item.Entity.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                        ING_ID_INGRESO = item.Entity.GRUPO_PARTICIPANTE.ING_ID_INGRESO
                    });
                }

                StaticSourcesViewModel.SourceChanged = false;
                ListaInternosSuspendidos(SelectedGrupo);
                await new Dialogos().ConfirmacionDialogoReturn("Manejo Sanciones", "Participantes Evaluados Exitosamente");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar participantes evaluados", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.MANEJO_CANCELADO_SUSPENDIDO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                    {
                        AgregarMenuEnabled = true;
                    }
                    if (p.CONSULTAR == 1)
                    {
                        EjeEnabled = true;
                        ProgramaEnabled = true;
                        ActividadEnabled = true;
                        GrupoEnabled = true;
                        TabSuspendidoEnabled = true;
                    }
                    if (p.EDITAR == 1)
                        EditarMenuEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        /// <summary>
        /// metodo que carga informacion al seleccionar modulo
        /// </summary>
        /// <param name="obj">usercontrol del modulo</param>
        private async void ManejoSancionesLoad(ManejoSancionesView obj)
        {
            try
            {
                ConfiguraPermisos();
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
                Tab = obj.TabManejoSanciones;
                //cargar combobox de flitros
                var index = 0;
                ListEjes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EJE>>(() => new ObservableCollection<EJE>(new cGrupoParticipanteCancelado().GetData().Where(w => w.ESTATUS == null && w.GRUPO_PARTICIPANTE.ESTATUS == 2).Select(s => s.GRUPO_PARTICIPANTE.EJE1).Distinct().OrderBy(o => o.ORDEN)));
                if (ListEjes.Count > 0)
                {
                    //ejes modelo
                    ListEjes.Insert(0, new EJE() { COMPLEMENTARIO = "MODELO" });
                    index = ListEjes.IndexOf(ListEjes.Where(w => w.COMPLEMENTARIO != "MODELO").OrderBy(o => o.COMPLEMENTARIO == "S").ThenBy(t => t.ORDEN).Where(w => w.COMPLEMENTARIO == "S").FirstOrDefault());
                    //ejes complementarios
                    if (index > 0)
                        ListEjes.Insert(index, new EJE() { COMPLEMENTARIO = "COMPLEMENTARIO" });
                }

                SuspenListEjes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EJE>>(() => new ObservableCollection<EJE>(new cGrupoParticipanteCancelado().GetData().Where(w => w.ID_ESTATUS == 4 && w.ESTATUS == 1 && w.RESPUESTA_FEC != null && w.GRUPO_PARTICIPANTE.ESTATUS == 4).Select(s => s.GRUPO_PARTICIPANTE.EJE1).Distinct().OrderBy(o => o.ORDEN)));
                if (SuspenListEjes.Count > 0)
                {
                    //ejes modelo
                    SuspenListEjes.Insert(0, new EJE() { COMPLEMENTARIO = "MODELO" });
                    index = SuspenListEjes.IndexOf(SuspenListEjes.Where(w => w.COMPLEMENTARIO != "MODELO").OrderBy(o => o.COMPLEMENTARIO == "S").ThenBy(t => t.ORDEN).Where(w => w.COMPLEMENTARIO == "S").FirstOrDefault());
                    //eje complementarios
                    if (index > 0)
                        SuspenListEjes.Insert(index, new EJE() { COMPLEMENTARIO = "COMPLEMENTARIO" });
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar modulo de cancelar/suspender", ex);
            }
        }
        /// <summary>
        /// metodo que carga el listado de programas correspondiente al eje seleccionado
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
                ListProgramas = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TIPO_PROGRAMA>>(() => new ObservableCollection<TIPO_PROGRAMA>(new cGrupoParticipanteCancelado().GetData().Where(w => w.ESTATUS == null && w.GRUPO_PARTICIPANTE.ESTATUS == 2 && w.GRUPO_PARTICIPANTE.EJE == Id_eje && departamentosUsuarios.Contains(w.GRUPO_PARTICIPANTE.ACTIVIDAD.TIPO_PROGRAMA.ID_DEPARTAMENTO.Value) && !w.GRUPO_PARTICIPANTE.NOTA_TECNICA.Any()).Select(s => s.GRUPO_PARTICIPANTE.ACTIVIDAD.TIPO_PROGRAMA).Distinct().OrderBy(o => o.NOMBRE).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar programas", ex);
            }
        }
        /// <summary>
        /// metodo que carga la lista de actividades correspondientes al programa seleccionado
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
                ListActividades = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ACTIVIDAD>>(() => new ObservableCollection<ACTIVIDAD>(new cGrupoParticipanteCancelado().GetData().Where(w => w.ESTATUS == null && w.GRUPO_PARTICIPANTE.ESTATUS == 2 && w.GRUPO_PARTICIPANTE.EJE == SelectedEje && w.ID_TIPO_PROGRAMA == Id_tipo_programa && !w.GRUPO_PARTICIPANTE.NOTA_TECNICA.Any()).Select(s => s.GRUPO_PARTICIPANTE.ACTIVIDAD).Distinct().OrderBy(o => o.DESCR).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar actividades", ex);
            }
        }
        /// <summary>
        /// metodo que lista los internos a ser evaluados segun grupo seleccionado
        /// </summary>
        /// <param name="Id_grupo">id del grupo seleccionado</param>
        async void ListaInternosAEvaluar(short Id_grupo)
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

                ListParticipantes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListaInternosSancionados>>(() =>
                {
                    var listainternos = new ObservableCollection<ListaInternosSancionados>();
                    try
                    {
                        listainternos = new ObservableCollection<ListaInternosSancionados>(new cGrupoParticipanteCancelado().GetData()
                            .Where(w => (w.ID_GRUPO == Id_grupo) && w.ESTATUS == null && w.GRUPO_PARTICIPANTE.ESTATUS == 2 && w.GRUPO_PARTICIPANTE.EJE == SelectedEje && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == SelectedActividad && !w.GRUPO_PARTICIPANTE.NOTA_TECNICA.Any())
                            .OrderBy(o => o.GRUPO_PARTICIPANTE.ACTIVIDAD.TIPO_PROGRAMA.ORDEN)
                            .ThenBy(o => o.GRUPO_PARTICIPANTE.ACTIVIDAD.ORDEN)
                            .ToList().Select(s => new ListaInternosSancionados
                            {
                                Entity = s,
                                NOMBRE = s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE.Trim(),
                                PATERNO = s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO.Trim(),
                                MATERNO = s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO.Trim(),
                                ImagenParticipante = s.GRUPO_PARTICIPANTE.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault() != null ? s.GRUPO_PARTICIPANTE.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson(),
                                ID_ANIO = s.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                                ID_IMPUTADO = s.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                                UBICACION = s.GRUPO_PARTICIPANTE.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + s.GRUPO_PARTICIPANTE.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() + s.GRUPO_PARTICIPANTE.INGRESO.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(s.GRUPO_PARTICIPANTE.INGRESO.CAMA.DESCR) ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.ID_CAMA.ToString().Trim() : s.GRUPO_PARTICIPANTE.INGRESO.CAMA.ID_CAMA + " " + s.GRUPO_PARTICIPANTE.INGRESO.CAMA.DESCR.Trim()),
                                PLANIMETRIA = s.GRUPO_PARTICIPANTE.INGRESO.CAMA != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty,
                                PLANIMETRIACOLOR = s.GRUPO_PARTICIPANTE.INGRESO.CAMA != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.COLOR : string.Empty : string.Empty : string.Empty : string.Empty,
                                RESTANTE = string.IsNullOrEmpty(CalcularSentencia(s.GRUPO_PARTICIPANTE.INGRESO)) ? string.Empty : CalcularSentencia(s.GRUPO_PARTICIPANTE.INGRESO).Replace('_', ' '),
                                SENTENCIA = varauxSentencia,
                                MOTIVO = s.MOTIVO,
                                MOVIMIENTO = s.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                                ASISTENCIA = ObtenerPorcentajeAsistencia(s.GRUPO_PARTICIPANTE, s.GRUPO_PARTICIPANTE.INGRESO.GRUPO_PARTICIPANTE)
                            })
                            .OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO)
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
        /// metodo que calcula sentencia y sentencia restante
        /// </summary>
        /// <param name="ingres">objeto ingreso</param>
        /// <returns>cadena de texto de sentencia restante y en variable sentencia</returns>
        private string CalcularSentencia(INGRESO ingres)
        {
            try
            {
                if (ingres != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (ingres.CAUSA_PENAL != null)
                    {
                        foreach (var cp in ingres.CAUSA_PENAL)
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
                        return a + (a == 1 ? " Año " : " Años ") + m + (m == 1 ? " Mes " : " Meses ") + d + (d == 1 ? " Dia" : " Dias");
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
        /// metodo que hace el calculo para obtener el porcentaje de asistencia del interno en el grupo seleccioinado
        /// </summary>
        /// <param name="item">objeto grupo participante</param>
        /// <param name="collection">lista de grupo participante</param>
        /// <returns>cadena de texto en formato porcentaje del calculo</returns>
        private string ObtenerPorcentajeAsistencia(GRUPO_PARTICIPANTE item, ICollection<GRUPO_PARTICIPANTE> collection)
        {
            try
            {
                var TotalHoras = 0.0;
                var AsistenciaHoras = 0.0;
                TotalHoras = item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0;
                AsistenciaHoras = item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && collection.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();

                if (double.IsNaN((AsistenciaHoras / TotalHoras)))
                    return string.Empty;

                return string.Format("{0:P2}", (AsistenciaHoras / TotalHoras));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del participante.", ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// metodo que calcula y muestra los cursos y horas del interno
        /// </summary>
        /// <param name="collection">listado de grupo participante</param>
        /// <param name="TotalActividades">total de actividades del interno</param>
        private void ObtenerCursosAprovadosTotalHoras(ICollection<GRUPO_PARTICIPANTE> collection, int TotalActividades)
        {
            var acreditados = 0;
            try
            {
                var HorasAsistencia = 0;
                var TotalAsistencia = 0;
                foreach (var item in collection)
                {
                    acreditados = acreditados + item.NOTA_TECNICA.Where(w => w.ESTATUS == 1).Count();
                    TotalAsistencia = TotalAsistencia + (item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0);
                    HorasAsistencia = HorasAsistencia + item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && collection.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();
                }
                MaxValueProBar = TotalActividades == 0 ? 1 : TotalActividades;
                CantidadActividadesAprovadas = acreditados;

                HorasTratamiento = HorasAsistencia + "/" + TotalAsistencia;
                AvanceTratamiento = acreditados + "/" + TotalActividades;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener avances del participante.", ex);
            }
        }
        /// <summary>
        /// metodo que lista los programas segun el eje seleccionado
        /// </summary>
        /// <param name="Id_eje">id del eje seleccionado</param>
        async void ProgramasLoadSuspen(short Id_eje)
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
                SuspenListProgramas = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TIPO_PROGRAMA>>(() => new ObservableCollection<TIPO_PROGRAMA>(new cGrupoParticipanteCancelado().GetData().Where(w => w.ID_ESTATUS == 4 && w.ESTATUS == 1 && w.RESPUESTA_FEC != null && w.GRUPO_PARTICIPANTE.ESTATUS == 4 && w.GRUPO_PARTICIPANTE.EJE == Id_eje && departamentosUsuarios.Contains(w.GRUPO_PARTICIPANTE.ACTIVIDAD.TIPO_PROGRAMA.ID_DEPARTAMENTO.Value) && !w.GRUPO_PARTICIPANTE.NOTA_TECNICA.Any()).Select(s => s.GRUPO_PARTICIPANTE.ACTIVIDAD.TIPO_PROGRAMA).Distinct().OrderBy(o => o.NOMBRE).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar programas", ex);
            }
        }
        /// <summary>
        /// metodo que lista las actividades del programa seleccionado 
        /// </summary>
        /// <param name="Id_tipo_programa">id del programa seleccionado</param>
        async void ActividadesLoadSuspen(short Id_tipo_programa)
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
                SuspenListActividades = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ACTIVIDAD>>(() => new ObservableCollection<ACTIVIDAD>(new cGrupoParticipanteCancelado().GetData().Where(w => w.ID_ESTATUS == 4 && w.ESTATUS == 1 && w.RESPUESTA_FEC != null && w.GRUPO_PARTICIPANTE.ESTATUS == 4 && w.GRUPO_PARTICIPANTE.EJE == SuspenSelectedEje && w.ID_TIPO_PROGRAMA == Id_tipo_programa && !w.GRUPO_PARTICIPANTE.NOTA_TECNICA.Any()).Select(s => s.GRUPO_PARTICIPANTE.ACTIVIDAD).Distinct().OrderBy(o => o.DESCR).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar actividades", ex);
            }
        }
        /// <summary>
        /// metodo que listalos internos suspendidos del grupo seleccionado
        /// </summary>
        /// <param name="Id_grupo">id grupo seleccionado</param>
        async void ListaInternosSuspendidos(short Id_grupo)
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

                SuspenListParticipantes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListaInternosSancionados>>(() =>
                {
                    var listainternos = new ObservableCollection<ListaInternosSancionados>();
                    try
                    {
                        listainternos = new ObservableCollection<ListaInternosSancionados>(new cGrupoParticipanteCancelado().GetData()
                            .Where(w => (w.ID_GRUPO == Id_grupo) && w.ID_ESTATUS == 4 && w.ESTATUS == 1 && w.RESPUESTA_FEC != null && w.GRUPO_PARTICIPANTE.ESTATUS == 4 && w.GRUPO_PARTICIPANTE.EJE == SuspenSelectedEje && w.ID_TIPO_PROGRAMA == SuspenSelectedPrograma && w.ID_ACTIVIDAD == SuspenSelectedActividad && !w.GRUPO_PARTICIPANTE.NOTA_TECNICA.Any())
                            .OrderBy(o => o.GRUPO_PARTICIPANTE.ACTIVIDAD.TIPO_PROGRAMA.ORDEN)
                            .ThenBy(o => o.GRUPO_PARTICIPANTE.ACTIVIDAD.ORDEN)
                            .ToList().Select(s => new ListaInternosSancionados
                            {
                                Entity = s,
                                NOMBRE = s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE.Trim(),
                                PATERNO = s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO.Trim(),
                                MATERNO = s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO.Trim(),
                                ImagenParticipante = s.GRUPO_PARTICIPANTE.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault() != null ? s.GRUPO_PARTICIPANTE.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson(),
                                ID_ANIO = s.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                                ID_IMPUTADO = s.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                                UBICACION = s.GRUPO_PARTICIPANTE.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + s.GRUPO_PARTICIPANTE.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() + s.GRUPO_PARTICIPANTE.INGRESO.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(s.GRUPO_PARTICIPANTE.INGRESO.CAMA.DESCR) ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.ID_CAMA.ToString().Trim() : s.GRUPO_PARTICIPANTE.INGRESO.CAMA.ID_CAMA + " " + s.GRUPO_PARTICIPANTE.INGRESO.CAMA.DESCR.Trim()),
                                PLANIMETRIA = s.GRUPO_PARTICIPANTE.INGRESO.CAMA != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty,
                                PLANIMETRIACOLOR = s.GRUPO_PARTICIPANTE.INGRESO.CAMA != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.GRUPO_PARTICIPANTE.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.COLOR : string.Empty : string.Empty : string.Empty : string.Empty,
                                RESTANTE = string.IsNullOrEmpty(CalcularSentencia(s.GRUPO_PARTICIPANTE.INGRESO)) ? string.Empty : CalcularSentencia(s.GRUPO_PARTICIPANTE.INGRESO).Replace('_', ' '),
                                MOTIVO = s.MOTIVO,
                                MOVIMIENTO = s.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                                ASISTENCIA = ObtenerPorcentajeAsistenciaSuspen(s.GRUPO_PARTICIPANTE, s.GRUPO_PARTICIPANTE.INGRESO.GRUPO_PARTICIPANTE),
                                RESPUESTA_FECHA = s.RESPUESTA_FEC,
                                RESPUESTA_TECNICA = s.RESPUESTA
                            })
                            .OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO)
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
        /// metodo que hace el calculo para obtener el porcentaje de asistencia del interno en el grupo seleccioinado
        /// </summary>
        /// <param name="item">objeto grupo participante</param>
        /// <param name="collection">lista de grupo participante</param>
        /// <returns>cadena de texto en formato porcentaje del calculo</returns>
        private string ObtenerPorcentajeAsistenciaSuspen(GRUPO_PARTICIPANTE item, ICollection<GRUPO_PARTICIPANTE> collection)
        {
            try
            {
                var TotalHoras = 0.0;
                var AsistenciaHoras = 0.0;
                TotalHoras = item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0;
                AsistenciaHoras = item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && collection.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();

                if (double.IsNaN((AsistenciaHoras / TotalHoras)))
                    return string.Empty;

                return string.Format("{0:P2}", (AsistenciaHoras / TotalHoras));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del participante.", ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// metodo que calcula y muestra los cursos y horas del interno
        /// </summary>
        /// <param name="collection">listado de grupo participante</param>
        /// <param name="TotalActividades">total de actividades del interno</param>
        private void ObtenerCursosAprovadosTotalHorasSuspen(ICollection<GRUPO_PARTICIPANTE> collection, int TotalActividades)
        {
            var acreditados = 0;
            try
            {
                var HorasAsistencia = 0;
                var TotalAsistencia = 0;
                foreach (var item in collection)
                {
                    acreditados = acreditados + item.NOTA_TECNICA.Where(w => w.ESTATUS == 1).Count();
                    TotalAsistencia = TotalAsistencia + (item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0);
                    HorasAsistencia = HorasAsistencia + item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && collection.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();
                }
                SuspenMaxValueProBar = TotalActividades == 0 ? 1 : TotalActividades;
                SuspenCantidadActividadesAprovadas = acreditados;

                SuspenHorasTratamiento = HorasAsistencia + "/" + TotalAsistencia;
                SuspenAvanceTratamiento = acreditados + "/" + TotalActividades;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener avances del participante.", ex);
            }
        }
        /// <summary>
        /// metodo que ejecuta en el cambio de pestaña
        /// </summary>
        /// <param name="e">SelectionChangedEventArgs</param>
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

                    if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                    {
                        StaticSourcesViewModel.SourceChanged = false;
                        SelectedEje = null;
                        SelectedPrograma = null;
                        SelectedActividad = null;
                        SelectedGrupo = -1;

                        SuspenSelectedEje = null;
                        SuspenSelectedPrograma = null;
                        SuspenSelectedActividad = null;
                        SuspenSelectedGrupo = -1;
                        //LimpiarCampos();
                        //LimpiarCamposCompl();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                    {
                        cancellingTabSelectionChange = true;
                        Tab.SelectedItem = ((SelectionChangedEventArgs)e).RemovedItems[0];
                        cancellingTabSelectionChange = false;
                        return;
                    }
                }

                //cargar combobox de filtros
                var index = 0;
                ListEjes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EJE>>(() => new ObservableCollection<EJE>(new cGrupoParticipanteCancelado().GetData().Where(w => w.ESTATUS == null && w.GRUPO_PARTICIPANTE.ESTATUS == 2).Select(s => s.GRUPO_PARTICIPANTE.EJE1).Distinct().OrderBy(o => o.ORDEN)));
                if (ListEjes.Count > 0)
                {
                    //eje modelo
                    ListEjes.Insert(0, new EJE() { COMPLEMENTARIO = "MODELO" });
                    index = ListEjes.IndexOf(ListEjes.Where(w => w.COMPLEMENTARIO != "MODELO").OrderBy(o => o.COMPLEMENTARIO == "S").ThenBy(t => t.ORDEN).Where(w => w.COMPLEMENTARIO == "S").FirstOrDefault());
                    //eje complementario
                    if (index > 0)
                        ListEjes.Insert(index, new EJE() { COMPLEMENTARIO = "COMPLEMENTARIO" });
                }

                SuspenListEjes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EJE>>(() => new ObservableCollection<EJE>(new cGrupoParticipanteCancelado().GetData().Where(w => w.ID_ESTATUS == 4 && w.ESTATUS == 1 && w.RESPUESTA_FEC != null && w.GRUPO_PARTICIPANTE.ESTATUS == 4).Select(s => s.GRUPO_PARTICIPANTE.EJE1).Distinct().OrderBy(o => o.ORDEN)));
                if (SuspenListEjes.Count > 0)
                {
                    //eje modelo
                    SuspenListEjes.Insert(0, new EJE() { COMPLEMENTARIO = "MODELO" });
                    index = SuspenListEjes.IndexOf(SuspenListEjes.Where(w => w.COMPLEMENTARIO != "MODELO").OrderBy(o => o.COMPLEMENTARIO == "S").ThenBy(t => t.ORDEN).Where(w => w.COMPLEMENTARIO == "S").FirstOrDefault());
                    //eje complementario
                    if (index > 0)
                        SuspenListEjes.Insert(index, new EJE() { COMPLEMENTARIO = "COMPLEMENTARIO" });
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambiar tab", ex);
            }
        }
    }
}
