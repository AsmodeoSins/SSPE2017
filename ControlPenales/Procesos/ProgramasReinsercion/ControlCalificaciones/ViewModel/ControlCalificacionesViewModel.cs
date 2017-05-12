using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class ControlCalificacionesViewModel
    {
        /* [descripcion de clase]
         * modulo de manejo de sanciones
         * 
         * este modulo se usa para el control de los internos en sus correspondientes grupos, se puede cancelar o suspender de este.
         * 
         * poner atencion en la opcion de cancelar, revisar si ya no pertenece al grupo
         * 
         * si se suspende pasa a lista de supendidos y se esperara a ser reactivado
         * 
         */

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CONTROL_CALIFICACIONES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
                        EnabledEditGrupo = true;
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

        ///TODO: ingresar el usuario para el filtro de responsable... TODO!!

        /// <summary>
        /// metodo que tiene todas las funcionalidades del evento click
        /// </summary>
        /// <param name="obj">nombre del parametro del control</param>
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
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ControlCalificacionesView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlCalificacionesViewModel();
                        break;
                    case "menu_guardar":
                        //se guardan las evaluaciones
                        if (ListParticipantes == null)
                            break;

                        if (await new Dialogos().ConfirmarEliminar("Control de Calificaciones", "Esta Seguro De Guardar?") == 0)
                            break;

                        var dalNT = new cNotaTecnica();
                        var dalGP = new cGrupoParticipante();
                        var fechaserver = Fechas.GetFechaDateServer;
                        foreach (var item in ListParticipantes.Where(w => !string.IsNullOrEmpty(w.NOTA_TECNICA)))
                        {
                            dalNT.Insert(new NOTA_TECNICA()
                            {
                                ID_CENTRO = item.Entity.ID_CENTRO,
                                ID_TIPO_PROGRAMA = item.Entity.ID_TIPO_PROGRAMA,
                                ID_ACTIVIDAD = item.Entity.ID_ACTIVIDAD,
                                ID_GRUPO = item.Entity.ID_GRUPO.Value,
                                ID_CONSEC = item.Entity.ID_CONSEC,
                                NOTA = item.NOTA_TECNICA,
                                FEC_REGISTRO = fechaserver,
                                //1 acreditado, 2 no acreditado
                                ESTATUS = item.ACREDITO ? (short)1 : (short)2
                            });

                            dalGP.Update(new GRUPO_PARTICIPANTE()
                            {
                                EJE = item.Entity.EJE,
                                //5 concluido
                                ESTATUS = 5,
                                FEC_REGISTRO = item.Entity.FEC_REGISTRO,
                                ID_ACTIVIDAD = item.Entity.ID_ACTIVIDAD,
                                ID_CENTRO = item.Entity.ID_CENTRO,
                                ID_CONSEC = item.Entity.ID_CONSEC,
                                ID_GRUPO = item.Entity.ID_GRUPO,
                                ID_TIPO_PROGRAMA = item.Entity.ID_TIPO_PROGRAMA,
                                ING_ID_ANIO = item.Entity.ING_ID_ANIO,
                                ING_ID_CENTRO = item.Entity.ING_ID_CENTRO,
                                ING_ID_IMPUTADO = item.Entity.ING_ID_IMPUTADO,
                                ING_ID_INGRESO = item.Entity.ING_ID_INGRESO

                            });
                        }

                        if (ListParticipantes.Where(w => !string.IsNullOrEmpty(w.NOTA_TECNICA)).Any())
                        {
                            var grupo = new cGrupo().GetData().Where(w => w.ID_GRUPO == SelectedGrupo).FirstOrDefault();
                            new cGrupo().Update(new GRUPO()
                            {
                                DESCR = grupo.DESCR,
                                FEC_FIN = grupo.FEC_FIN,
                                FEC_INICIO = grupo.FEC_INICIO,
                                ID_ACTIVIDAD = grupo.ID_ACTIVIDAD,
                                ID_CENTRO = grupo.ID_CENTRO,
                                ID_DEPARTAMENTO = grupo.ID_DEPARTAMENTO,
                                //4 concluido
                                ID_ESTATUS_GRUPO = 4,
                                ID_GRUPO = grupo.ID_GRUPO,
                                ID_GRUPO_RESPONSABLE = grupo.ID_GRUPO_RESPONSABLE,
                                ID_TIPO_PROGRAMA = grupo.ID_TIPO_PROGRAMA,
                                RECURRENCIA = grupo.RECURRENCIA,
                                ID_EJE = grupo.ID_EJE
                            });
                        }

                        StaticSourcesViewModel.SourceChanged = false;
                        ListaInternosCalificacion(SelectedGrupo);
                        await new Dialogos().ConfirmacionDialogoReturn("Control de Calificaciones", "Participantes Evaluados Exitosamente");
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
        /// metodo que se carga cuando se selecciona el modulo
        /// </summary>
        /// <param name="obj"></param>
        private async void ControlCalificacionesLoad(ControlCalificacionesView obj)
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
                //carga lista de ejes
                var index = 0;
                ListEjes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EJE>>(() => new ObservableCollection<EJE>(new cGrupoParticipante().GetData().Where(w => w.ESTATUS == 1 && !w.NOTA_TECNICA.Any()).Select(s => s.EJE1).Distinct().OrderBy(o => o.ORDEN)));
                if (ListEjes.Count > 0)
                {
                    //ejes tipo modelo
                    ListEjes.Insert(0, new EJE() { COMPLEMENTARIO = "MODELO" });
                    index = ListEjes.IndexOf(ListEjes.Where(w => w.COMPLEMENTARIO != "MODELO").OrderBy(o => o.COMPLEMENTARIO == "S").ThenBy(t => t.ORDEN).Where(w => w.COMPLEMENTARIO == "S").FirstOrDefault());
                    //ejes tipo complementario
                    if (index > 0)
                        ListEjes.Insert(index, new EJE() { COMPLEMENTARIO = "COMPLEMENTARIO" });
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar modulo de creacion de programas", ex);
            }
        }
        /// <summary>
        /// metodo que carga los programas dependientes del eje
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
                ListProgramas = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TIPO_PROGRAMA>>(() => new ObservableCollection<TIPO_PROGRAMA>(new cGrupoParticipante().GetData().Where(w => w.ESTATUS == 1 && w.EJE == Id_eje && departamentosUsuarios.Contains(w.ACTIVIDAD.TIPO_PROGRAMA.ID_DEPARTAMENTO.Value) && !w.NOTA_TECNICA.Any()).Select(s => s.ACTIVIDAD.TIPO_PROGRAMA).Distinct().OrderBy(o => o.NOMBRE).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar programas", ex);
            }
        }
        /// <summary>
        /// metodo que carga las actividades del programa seleccionado
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
                ListActividades = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ACTIVIDAD>>(() => new ObservableCollection<ACTIVIDAD>(new cGrupoParticipante().GetData().Where(w => (w.ESTATUS == 1 || w.ESTATUS == 2) && w.EJE == SelectedEje && w.ID_TIPO_PROGRAMA == Id_tipo_programa && !w.NOTA_TECNICA.Any()).Select(s => s.ACTIVIDAD).Distinct().OrderBy(o => o.DESCR).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar actividades", ex);
            }
        }
        /// <summary>
        /// obtiene lista de internos a ser evaluados
        /// </summary>
        /// <param name="Id_grupo">id del grupo seleccionado</param>
        async void ListaInternosCalificacion(short Id_grupo)
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

                ListParticipantes = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ListaInternosCalificacion>>(() =>
                {
                    var listainternos = new ObservableCollection<ListaInternosCalificacion>();
                    try
                    {
                        listainternos = new ObservableCollection<ListaInternosCalificacion>(new cGrupoParticipante().GetData()
                            .Where(w => (w.ID_GRUPO == Id_grupo) && (w.ESTATUS == 1 || w.ESTATUS == 2) && w.EJE == SelectedEje && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == SelectedActividad && !w.NOTA_TECNICA.Any())
                            .OrderBy(o => o.ACTIVIDAD.TIPO_PROGRAMA.ORDEN)
                            .ThenBy(o => o.ACTIVIDAD.ORDEN)
                            .ToList().Select(s => new ListaInternosCalificacion
                            {
                                Entity = s,
                                NOMBRE = s.INGRESO.IMPUTADO.NOMBRE.Trim(),
                                PATERNO = s.INGRESO.IMPUTADO.PATERNO.Trim(),
                                MATERNO = s.INGRESO.IMPUTADO.MATERNO.Trim(),
                                ImagenParticipante = s.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault() != null ? s.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson(),
                                ID_ANIO = s.ING_ID_ANIO,
                                ID_IMPUTADO = s.ING_ID_IMPUTADO,
                                UBICACION = s.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + s.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() + s.INGRESO.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(s.INGRESO.CAMA.DESCR) ? s.INGRESO.CAMA.ID_CAMA.ToString().Trim() : s.INGRESO.CAMA.ID_CAMA + " " + s.INGRESO.CAMA.DESCR.Trim()),
                                PLANIMETRIA = s.INGRESO.CAMA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty,
                                PLANIMETRIACOLOR = s.INGRESO.CAMA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? s.INGRESO.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.COLOR : string.Empty : string.Empty : string.Empty : string.Empty,
                                RESTANTE = string.IsNullOrEmpty(CalcularSentencia(s.INGRESO)) ? string.Empty : CalcularSentencia(s.INGRESO).Replace('_', ' '),
                                SENTENCIA = varauxSentencia,
                                //DELITO = s.INGRESO.INGRESO_DELITO.DESCR,
                                DELITO = s.INGRESO.DELITO != null ? s.INGRESO.DELITO.DESCR : string.Empty,
                                ASISTENCIA = ObtenerPorcentajeAsistencia(s, s.INGRESO.GRUPO_PARTICIPANTE)
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
        /// <param name="ingres">objeto ingreso del interno seleccionado</param>
        /// <returns>cadena de texto con el resultado de la operacion, en una variable se presenta la sentencia</returns>
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

                        //sentencia
                        varauxSentencia = anios + (anios == 1 ? " Año " : " Años ") + meses + (meses == 1 ? " Mes " : " Meses ") + dias + (dias == 1 ? " Dia" : " Dias");
                        //sentencia restante
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
        /// metodo que obtiene el porcentaje de asistencia del interno
        /// </summary>
        /// <param name="item">objeto de tipo grupo participante</param>
        /// <param name="collection"> colleccion de grupo participante</param>
        /// <returns>cadena de texto con el resultado de la operacion %</returns>
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular porcentaje de asistencia", ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// metodo que calcula y muestra en propiedades el los cursos y total de horas del interno seleccionado
        /// </summary>
        /// <param name="collection">colleccion de grupo participante</param>
        /// <param name="TotalActividades"> calculo de total de actividades del interno</param>
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
    }
}
