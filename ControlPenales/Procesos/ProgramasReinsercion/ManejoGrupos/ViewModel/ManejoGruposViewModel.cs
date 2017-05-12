using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ControlPenales
{
    partial class ManejoGruposViewModel
    {
        /* [descripcion de clase]
         * modulo de manejo de grupos
         * 
         * modulo para modificar horarios del grupo, asi como a sus integrantes
         * 
         * en este modulo se debe notificar cuando ocurran empalmes y cuando se quiera cambiar el estatus del participante ya sea a cancelado o suspendido
         * aqui se modifica tanto grupos de modelo y complementarios
         * 
         * cuidar el estatus del grupo y de los participantes
         * 
         * 
         */

        ///TODO: Revisar los querys, falta poner el centro del usuario y departamento

        /// <summary>
        /// metodo que carga al entrar al modulo
        /// </summary>
        /// <param name="obj">usercontrol del modulo</param>
        private void ManejoGruposLoad(ManejoGruposView obj)
        {
            try
            {
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
                
                VerGrupos = true;
                //con esto abre la ventana de edicion de fecha
                BindCmdDayClick = "CmdAbrirAgenda";
                Calendario = obj.Calendario;
                DatagridHorario = PopUpsViewModels.MainWindow.EditarFechaView.DG_HorarioDia;
                ConfiguraPermisos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar manejo de grupos", ex);
            }

        }
        /// <summary>
        /// metodo que carga las actividades segun el programa seleccionado
        /// </summary>
        /// <param name="Id_tipo_programa">id del programa seleccionado</param>
        async void ActividadesLoad(short Id_tipo_programa)
        {
            try
            {
                ListActividades = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ACTIVIDAD>>(() => new ObservableCollection<ACTIVIDAD>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == Id_tipo_programa).Select(s => s.ACTIVIDAD).Distinct().OrderBy(o => o.DESCR).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar actividades", ex);
            }
        }
        /// <summary>
        /// metodo que carga los grupos que tienen como actividad la seleccionada previamente
        /// </summary>
        /// <param name="Id_actividad">id de la actividad seleccionada</param>
        async void GrupoLoad(short Id_actividad)
        {
            try
            {
                ListGrupo = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<GRUPO>>(() => new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == Id_actividad && w.ID_ESTATUS_GRUPO == 1).Select(s => s).Distinct().OrderBy(o => o.DESCR).ToList()));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar grupos", ex);
            }
        }
        /// <summary>
        /// metodo para abrir el popup de edicion de fecha
        /// </summary>
        /// <param name="obj">un objeto tipo grupo_horario o un objeto tipo datetime</param>
        private async void AbrirAgenda(object obj)
        {
            try
            {
                StaticSourcesViewModel.ShowMensajeProgreso("Abriendo Modal");
                ListArea = ListArea ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<AREA>>(() => new ObservableCollection<AREA>(new cArea().GetData().Where(w => w.ID_TIPO_AREA != 5)));
                ListEstatusGrupo = ListEstatusGrupo ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<GRUPO_HORARIO_ESTATUS>>(() => new ObservableCollection<GRUPO_HORARIO_ESTATUS>(new cGrupoHorarioEstatus().GetData()));

                SelectedArea = null;
                SelectedEstatusGrupo = 1;
                EditFechaInicio = null;
                EditFechaFin = null;
                SelectedFecha = null;
                await TaskEx.Delay(1);
                EntityUpdate = null;
                AgregarFecha = Visibility.Collapsed;

                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.EDITAR_FECHA);

                if (obj == null)
                    return;

                // se valida de que tipo es el objeto
                if (obj is GRUPO_HORARIO)
                {
                    EntityUpdate = (GRUPO_HORARIO)obj;

                    SelectedArea = EntityUpdate.ID_AREA;
                    SelectedEstatusGrupo = EntityUpdate.ESTATUS;
                    EditFechaInicio = EntityUpdate.HORA_INICIO;
                    EditFechaFin = EntityUpdate.HORA_TERMINO;

                    SelectedFecha = EntityUpdate.HORA_INICIO.Value.Date;
                }
                else
                    SelectedFecha = (DateTime)obj;

                await TaskEx.Delay(1);
                if (ListInternosDia == null)
                    await TaskEx.Delay(1);

                DatagridHorario.ItemsSource = ListInternosDia.Where(w => w.ListHorario.Any());
                StaticSourcesViewModel.CloseMensajeProgreso();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir agenda", ex);
            }
        }
        /// <summary>
        /// metodo que obtiene el horario del participante seleccionado
        /// </summary>
        /// <param name="s">objeto tipo grupo participante</param>
        /// <param name="horaincio">objeto tipo datetime</param>
        /// <param name="horatermino">objeto tipo datetime</param>
        /// <returns>lista de horario del participante</returns>
        private ObservableCollection<ListHorario> ListaHorario(GRUPO_PARTICIPANTE s, DateTime horaincio, DateTime horatermino)
        {
            try
            {
                var horarios = new List<ListHorario>();
                foreach (var item in new cGrupoParticipante().GetData().Where(w => w.ID_CENTRO == s.ID_CENTRO && w.ING_ID_ANIO == s.ING_ID_ANIO && w.ING_ID_IMPUTADO == s.ING_ID_IMPUTADO && w.ING_ID_INGRESO == s.ING_ID_INGRESO && w.ID_GRUPO != null))
                {
                    foreach (var subitem in new cGrupoAsistencia().GetData().Where(w => w.GRUPO_HORARIO.GRUPO.ID_ESTATUS_GRUPO == 1 && w.GRUPO_HORARIO.ESTATUS == 1 && w.ESTATUS == 1 && w.ID_GRUPO == item.ID_GRUPO && w.GRUPO_HORARIO.HORA_INICIO >= horaincio && w.GRUPO_HORARIO.HORA_TERMINO < horatermino).AsEnumerable().Select(se => new ListHorario()
                        {
                            GrupoHorarioEntity = se.GRUPO_HORARIO,
                            NombreActividad = se.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR,
                            NombreGrupo = se.GRUPO_HORARIO.GRUPO.DESCR,
                            AREADESCR = se.GRUPO_HORARIO.AREA.DESCR,
                            GRUPO_HORARIO_ESTATUSDESCR = se.GRUPO_HORARIO.GRUPO_HORARIO_ESTATUS.DESCR,
                            DESCRDIA = se.GRUPO_HORARIO.HORA_INICIO.Value.ToLongDateString(),
                            HORA_INICIO = se.GRUPO_HORARIO.HORA_INICIO.Value,
                            HORA_TERMIINO = se.GRUPO_HORARIO.HORA_TERMINO.Value,
                            strHORA_INICIO = se.GRUPO_HORARIO.HORA_INICIO.Value.ToShortTimeString(),
                            strHORA_TERMIINO = se.GRUPO_HORARIO.HORA_TERMINO.Value.ToShortTimeString(),
                            State = string.Empty,
                            Id_Actividad = se.GRUPO_HORARIO.GRUPO.ACTIVIDAD.ID_ACTIVIDAD,
                            Id_Programa = se.GRUPO_HORARIO.GRUPO.ACTIVIDAD.ID_TIPO_PROGRAMA,
                        }))
                        horarios.Add(subitem);
                }
                return new ObservableCollection<ListHorario>(horarios.OrderBy(o => o.HORA_INICIO).ToList());
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error ", ex);
            }
            return new ObservableCollection<ListHorario>();
        }
        /// <summary>
        /// metodo que crea el control de calendario
        /// </summary>
        /// <param name="value">grupo seleccionado</param>
        void CargarCalendarioGrupo(GRUPO value)
        {
            try
            {
                if (value == null)
                    Calendario.DisposeCalendario();
                else
                {
                    if (value.GRUPO_HORARIO.Count < 1)
                    {
                        Calendario.DisposeCalendario();
                        return;
                    }
                    Calendario.SelectedMes = value.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value > Fechas.GetFechaDateServer ? value.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.Month : value.GRUPO_HORARIO.OrderByDescending(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value < Fechas.GetFechaDateServer ? value.GRUPO_HORARIO.OrderByDescending(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.Month : Fechas.GetFechaDateServer.Month;

                    Calendario.SelectedAnio = value.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value > Fechas.GetFechaDateServer ? value.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.Year : value.GRUPO_HORARIO.OrderByDescending(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value < Fechas.GetFechaDateServer ? value.GRUPO_HORARIO.OrderByDescending(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.Year : Fechas
                        .GetFechaDateServer.Year;
                    Calendario.CrearCalendario(value.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value, value.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value, value.GRUPO_HORARIO.ToList());
                    Calendario.DiasAgendados = new ObservableCollection<GRUPO_HORARIO>(value.GRUPO_HORARIO.ToList());
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Cargar Calendario", ex);
            }
        }
        /// <summary>
        /// metodo que ejecuta el evento valuechange
        /// </summary>
        /// <param name="SelectedItem">datagrid del popup de edicion de fecha</param>
        private void SetIsSelectedProperty(DataGrid SelectedItem)
        {
            try
            {
                SelectedItem.Visibility = Visibility.Hidden;
                if (EditFechaInicio.HasValue && EditFechaFin.HasValue)
                {
                    var horainicio = new DateTime(SelectedFecha.HasValue ? SelectedFecha.Value.Year : EntityUpdate.HORA_INICIO.Value.Year, SelectedFecha.HasValue ? SelectedFecha.Value.Month : EntityUpdate.HORA_INICIO.Value.Month, SelectedFecha.HasValue ? SelectedFecha.Value.Day : EntityUpdate.HORA_INICIO.Value.Day, EditFechaInicio.Value.Hour, EditFechaInicio.Value.Minute, 0);
                    var horatermino = new DateTime(SelectedFecha.HasValue ? SelectedFecha.Value.Year : EntityUpdate.HORA_TERMINO.Value.Year, SelectedFecha.HasValue ? SelectedFecha.Value.Month : EntityUpdate.HORA_TERMINO.Value.Month, SelectedFecha.HasValue ? SelectedFecha.Value.Day : EntityUpdate.HORA_TERMINO.Value.Day, EditFechaFin.Value.Hour, EditFechaFin.Value.Minute, 0).AddSeconds(1);

                    if (((CollectionView)(((ItemsControl)(SelectedItem)).Items)).SourceCollection.Cast<ListaDiaInternos>().Where(w => w.ListHorario.Where(wh => wh.HORA_INICIO >= horainicio && wh.HORA_TERMIINO < horatermino).Any()).Any())
                    {
                        foreach (var item in ((CollectionView)(((ItemsControl)(SelectedItem)).Items)).SourceCollection.Cast<ListaDiaInternos>())
                        {
                            item.State = string.Empty;
                            foreach (var subitem in item.ListHorario)
                                subitem.State = string.Empty;
                        }

                        foreach (var item in ((CollectionView)(((ItemsControl)(SelectedItem)).Items)).SourceCollection.Cast<ListaDiaInternos>().Where(w => w.ListHorario.Where(wh => wh.HORA_INICIO >= horainicio && wh.HORA_TERMIINO < horatermino).Any()))
                        {
                            foreach (var subitem in item.ListHorario.Where(w => (DateTime)w.HORA_INICIO >= horainicio && (DateTime)w.HORA_TERMIINO < horatermino))
                                if (!(subitem.Id_Actividad == SelectedActividad && subitem.Id_Programa == SelectedPrograma))
                                {
                                    item.State = "Empalme";
                                    subitem.State = "Empalme";
                                    DatagridHorario.Visibility = Visibility.Visible;
                                }
                        }
                    }

                    var temp = SelectedItem.ItemsSource;
                    SelectedItem.ItemsSource = null;
                    SelectedItem.ItemsSource = temp;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al marcar/desmarcar registro", ex);
            }
        }
        /// <summary>
        /// metodo que valida la disponibilidad del area seleccionada
        /// </summary>
        void ValidateAreaDisponible()
        {
            try
            {
                TextError = null;
                if (EditFechaFin.HasValue && EditFechaInicio.HasValue && SelectedArea.HasValue)
                {
                    var horainicio = new DateTime(SelectedFecha.HasValue ? SelectedFecha.Value.Year : EditFechaInicio.Value.Year, SelectedFecha.HasValue ? SelectedFecha.Value.Month : EditFechaInicio.Value.Month, SelectedFecha.HasValue ? SelectedFecha.Value.Day : EditFechaInicio.Value.Day, EditFechaInicio.Value.Hour, EditFechaInicio.Value.Minute, 0);
                    var horatermino = new DateTime(SelectedFecha.HasValue ? SelectedFecha.Value.Year : EditFechaFin.Value.Year, SelectedFecha.HasValue ? SelectedFecha.Value.Month : EditFechaFin.Value.Month, SelectedFecha.HasValue ? SelectedFecha.Value.Day : EditFechaFin.Value.Day, EditFechaFin.Value.Hour, EditFechaFin.Value.Minute, 0).AddSeconds(1);

                    if (new cGrupoHorario().GetData().Where(w => w.ID_GRUPO != SelectedGrupo.ID_GRUPO && w.GRUPO.ID_ESTATUS_GRUPO == 1 && w.ESTATUS == 1 && w.ID_AREA == SelectedArea.Value && w.HORA_INICIO >= horainicio && w.HORA_TERMINO < horatermino).Any() || new cAtencionCita().GetData().Where(w => (w.CITA_FECHA_HORA >= horainicio && w.CITA_HORA_TERMINA < horatermino)
                        && w.ATENCION_SOLICITUD.ATENCION_INGRESO.Any(a => a.ID_ANIO == w.INGRESO.ID_ANIO && a.ID_CENTRO == w.INGRESO.ID_CENTRO && a.ID_IMPUTADO == w.INGRESO.ID_IMPUTADO && a.ID_INGRESO == w.INGRESO.ID_INGRESO && a.ID_ATENCION == w.ATENCION_SOLICITUD.ID_ATENCION && a.ESTATUS == 1)).Any() || new cEvento().GetData().Where(w => (w.HORA_INVITACION >= horainicio && w.HORA_FIN < horatermino) && w.ID_ESTATUS == 1).Any())
                        TextError = "ESTE LUGAR ESTA OCUPADO A ESTA HORA";
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar area disponible", ex);
            }
        }
        /// <summary>
        /// metodo que valida si la fecha es correcta
        /// </summary>
        /// <param name="variableName">nombre de la variable</param>
        /// <param name="value">valor a igualar</param>
        /// <param name="Property">nombre de la propiedad</param>
        /// <returns>reanuda el hilo principal</returns>
        private async Task ValidarFecha(string variableName, object value, string Property)
        {
            try
            {
                this.GetType().GetField(variableName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, value);
                OnPropertyChanged(Property);
                await TaskEx.Delay(1);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar advertencia de cambio en horario", ex);
            }
        }
        /// <summary>
        /// metodo que tiene todas la funcionalidad de los clicks
        /// </summary>
        /// <param name="obj">parametro del evento click</param>
        private async void clickSwitch(Object obj)
        {
            try
            {
                var respuesta = false;
                switch (obj.ToString())
                {
                    case "menu_guardar":

                        break;
                    case "menu_agregar":
                        if (!SelectedPrograma.HasValue || !SelectedActividad.HasValue || SelectedGrupo == null)
                            return;

                        ListArea = ListArea ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<AREA>>(() => new ObservableCollection<AREA>(new cArea().GetData().Where(w => w.ID_TIPO_AREA != 5)));
                        ListEstatusGrupo = ListEstatusGrupo ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<GRUPO_HORARIO_ESTATUS>>(() => new ObservableCollection<GRUPO_HORARIO_ESTATUS>(new cGrupoHorarioEstatus().GetData()));
                        SelectedEstatusGrupo = 1;

                        SelectedArea = null;
                        EditFechaInicio = null;
                        EditFechaFin = null;
                        SelectedFecha = FechaServer.Date;
                        EntityUpdate = null;
                        AgregarFecha = Visibility.Visible;

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.EDITAR_FECHA);
                        break;
                    case "menu_eliminar":
                        break;
                    case "menu_editar":
                        if (SelectedGrupo == null)
                            return;

                        ListEstatus = ListEstatus ?? new ObservableCollection<GRUPO_PARTICIPANTE_ESTATUS>(new cGrupoParticipanteEstatus().GetData().Where(w => w.ID_ESTATUS == 2 || w.ID_ESTATUS == 3 || w.ID_ESTATUS == 4));
                        ListInternosGrupo = new ObservableCollection<ListaManejoInternos>(new cGrupoParticipante().GetData().Where(w => w.ID_GRUPO == SelectedGrupo.ID_GRUPO && (w.GRUPO_PARTICIPANTE_CANCELADO.Where(wh => (wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_CONSEC == w.ID_CONSEC && wh.ID_GRUPO == w.ID_GRUPO)).Any() ?

                            w.GRUPO_PARTICIPANTE_CANCELADO.Where(wh => (wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_CONSEC == w.ID_CONSEC) &&
                                (wh.RESPUESTA_FEC == null ?

                                (wh.RESPUESTA_FEC == null && wh.ESTATUS == 0) :

                                (w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC) && whe.RESPUESTA_FEC != null && (whe.ESTATUS == 0 || whe.ESTATUS == 2)).Count() == w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC)).Count()))).Any()


                                : true)).OrderBy(o => o.ING_ID_ANIO).ThenBy(t => t.ING_ID_IMPUTADO).AsEnumerable().Select(s => new ListaManejoInternos()
                        {
                            Entity = s,
                            FOLIO = s.INGRESO.IMPUTADO.ID_ANIO + "\\" + s.INGRESO.IMPUTADO.ID_IMPUTADO,
                            PATERNO = s.INGRESO.IMPUTADO.PATERNO.Trim(),
                            MATERNO = s.INGRESO.IMPUTADO.MATERNO.Trim(),
                            NOMBRE = s.INGRESO.IMPUTADO.NOMBRE.Trim(),
                            ListEstatusGrupoParticipante = ListEstatus,
                            SelectEstatus = s.ESTATUS
                        }));

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.EDITAR_INTEGRANTES_GRUPO);
                        break;
                    case "menu_cancelar":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ManejoGruposView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ManejoGruposViewModel();
                        break;
                    case "menu_ayuda":
                        break;
                    case "menu_salir":
                        PrincipalViewModel.SalirMenu();
                        break;
                    case "guardar_EdicionFecha":
                        if (SelectedArea == null || SelectedEstatusGrupo == null || EditFechaInicio == null || EditFechaFin == null || FechaValidateEditHasError || !SelectedFecha.HasValue)
                            if (await new Dialogos().ConfirmacionDialogoReturn("Manejo de Grupo", "Faltan Campos Por Capturar") == 1)
                                return;

                        if (AgregarFecha == Visibility.Visible)
                            EntityUpdate = new cGrupoHorario().GetData().Where(w => w.ID_GRUPO == SelectedGrupo.ID_GRUPO && (w.HORA_INICIO.Value.Year == SelectedFecha.Value.Year && w.HORA_INICIO.Value.Month == SelectedFecha.Value.Month && w.HORA_INICIO.Value.Day == SelectedFecha.Value.Day)).FirstOrDefault();

                        if (EntityUpdate != null)
                        {
                            respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando Fecha Del Grupo", () =>
                            {
                                try
                                {
                                    new cGrupoHorario().Update(new GRUPO_HORARIO()
                                    {
                                        ESTATUS = SelectedEstatusGrupo,
                                        HORA_INICIO = new DateTime(EntityUpdate.HORA_INICIO.Value.Year, EntityUpdate.HORA_INICIO.Value.Month, EntityUpdate.HORA_INICIO.Value.Day, EditFechaInicio.Value.Hour, EditFechaInicio.Value.Minute, 0),
                                        HORA_TERMINO = new DateTime(EntityUpdate.HORA_TERMINO.Value.Year, EntityUpdate.HORA_TERMINO.Value.Month, EntityUpdate.HORA_TERMINO.Value.Day, EditFechaFin.Value.Hour, EditFechaFin.Value.Minute, 0),
                                        ID_ACTIVIDAD = EntityUpdate.ID_ACTIVIDAD,
                                        ID_AREA = SelectedArea,
                                        ID_CENTRO = EntityUpdate.ID_CENTRO,
                                        ID_GRUPO = EntityUpdate.ID_GRUPO,
                                        ID_GRUPO_HORARIO = EntityUpdate.ID_GRUPO_HORARIO,
                                        ID_TIPO_PROGRAMA = EntityUpdate.ID_TIPO_PROGRAMA
                                    });

                                    EditFechaInicio = EditFechaInicio.Value.AddSeconds(1);

                                    foreach (var item in ListInternosDia)
                                    {
                                        var update = new cGrupoAsistencia().GetData().Where(w => w.ID_CENTRO == item.Entity.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.Entity.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.Entity.ID_ACTIVIDAD && w.ID_GRUPO == item.Entity.ID_GRUPO.Value && w.ID_GRUPO_HORARIO == EntityUpdate.ID_GRUPO_HORARIO && w.ID_CONSEC == item.Entity.ID_CONSEC).FirstOrDefault();
                                        if (update == null)
                                            continue;

                                        var updateempalme = ListInternosDia.Where(w => w.Entity.ID_CENTRO == item.Entity.ID_CENTRO && w.Entity.ID_TIPO_PROGRAMA == item.Entity.ID_TIPO_PROGRAMA && w.Entity.ID_ACTIVIDAD == item.Entity.ID_ACTIVIDAD && w.Entity.ID_GRUPO == item.Entity.ID_GRUPO.Value && w.Entity.GRUPO.GRUPO_HORARIO.Where(wh => wh.ID_CENTRO == item.Entity.ID_CENTRO && wh.ID_TIPO_PROGRAMA == item.Entity.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == item.Entity.ID_ACTIVIDAD && wh.ID_GRUPO == item.Entity.ID_GRUPO.Value && wh.ID_GRUPO_HORARIO == EntityUpdate.ID_GRUPO_HORARIO).Any() && w.Entity.ID_CONSEC == item.Entity.ID_CONSEC).FirstOrDefault().State == "Empalme";

                                        new cGrupoAsistencia().Update(new GRUPO_ASISTENCIA()
                                        {
                                            ID_CENTRO = update.ID_CENTRO,
                                            ID_TIPO_PROGRAMA = update.ID_TIPO_PROGRAMA,
                                            ID_ACTIVIDAD = update.ID_ACTIVIDAD,
                                            ID_GRUPO = update.ID_GRUPO,
                                            ID_GRUPO_HORARIO = update.ID_GRUPO_HORARIO,
                                            ID_CONSEC = update.ID_CONSEC,
                                            FEC_REGISTRO = update.FEC_REGISTRO,
                                            ASISTENCIA = update.ASISTENCIA,
                                            EMPALME = updateempalme ? update.EMPALME : 0,
                                            EMP_COORDINACION = updateempalme ? update.EMP_COORDINACION : 0,
                                            EMP_APROBADO = updateempalme ? update.EMP_APROBADO : null,
                                            EMP_FECHA = updateempalme ? update.EMP_FECHA : null,
                                            ESTATUS = update.ESTATUS
                                        });
                                    }

                                    if (ListInternosDia.Where(w => w.Revision && w.ListHorario != null && w.ListHorario.Where(wh => wh.State.Equals("Empalme")).Any()).Any())
                                    {
                                        var listGA = new List<ListaEmpalmesInterno>();
                                        var entitylistGH = new List<GRUPO_HORARIO>();
                                        foreach (var item in ListInternosDia.Where(w => w.Revision && w.ListHorario != null && w.ListHorario.Where(wh => wh.State.Equals("Empalme")).Any()))
                                        {
                                            foreach (var itemLisAct in item.ListHorario.Where(w => w.State.Equals("Empalme")))
                                                entitylistGH.Add(itemLisAct.GrupoHorarioEntity);
                                            listGA.Add(new ListaEmpalmesInterno() { EntityGrupoParticipante = item.Entity, ListGrupoHorario = entitylistGH });
                                        }
                                        new cGrupoAsistencia().GenerarEmpalmes(listGA);
                                    }
                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al modificar el estatus de los integrantes del grupo", ex);
                                    return false;
                                }
                            });

                            if (respuesta)
                                await new Dialogos().ConfirmacionDialogoReturn("Manejo de Grupos", "El horario se modifico exitosamente");
                        }
                        else
                        {
                            respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Insertando Nueva Fecha Para El Grupo", () =>
                            {
                                try
                                {
                                    var idGH = new cGrupoHorario().Insertar(new GRUPO_HORARIO()
                                    {
                                        ESTATUS = SelectedEstatusGrupo,
                                        HORA_INICIO = new DateTime(SelectedFecha.Value.Year, SelectedFecha.Value.Month, SelectedFecha.Value.Day, EditFechaInicio.Value.Hour, EditFechaInicio.Value.Minute, 0),
                                        HORA_TERMINO = new DateTime(SelectedFecha.Value.Year, SelectedFecha.Value.Month, SelectedFecha.Value.Day, EditFechaFin.Value.Hour, EditFechaFin.Value.Minute, 0),
                                        ID_ACTIVIDAD = SelectedActividad.Value,
                                        ID_AREA = SelectedArea,
                                        ID_CENTRO = GlobalVar.gCentro,
                                        ID_GRUPO = SelectedGrupo.ID_GRUPO,
                                        ID_TIPO_PROGRAMA = SelectedPrograma.Value
                                    });

                                    foreach (var item in ListInternosDia)
                                        if (item.State != "Empalme" || item.Revision)
                                            new cGrupoAsistencia().Insert(new GRUPO_ASISTENCIA()
                                            {
                                                ID_CENTRO = GlobalVar.gCentro,
                                                ID_TIPO_PROGRAMA = item.Entity.ID_TIPO_PROGRAMA,
                                                ID_ACTIVIDAD = item.Entity.ID_ACTIVIDAD,
                                                ID_GRUPO = item.Entity.ID_GRUPO.Value,
                                                ID_GRUPO_HORARIO = idGH.ID_GRUPO_HORARIO,
                                                ID_CONSEC = item.Entity.ID_CONSEC,
                                                FEC_REGISTRO = Fechas.GetFechaDateServer,
                                                ASISTENCIA = null,
                                                EMPALME = 0,
                                                EMP_COORDINACION = 0,
                                                EMP_APROBADO = null,
                                                EMP_FECHA = null,
                                                ESTATUS = 1
                                            });

                                    if (ListInternosDia.Where(w => w.Revision && w.ListHorario != null && w.ListHorario.Where(wh => wh.State.Equals("Empalme")).Any()).Any())
                                    {
                                        var listGA = new List<ListaEmpalmesInterno>();
                                        var entitylistGH = new List<GRUPO_HORARIO>();
                                        foreach (var item in ListInternosDia.Where(w => w.Revision && w.ListHorario != null && w.ListHorario.Where(wh => wh.State.Equals("Empalme")).Any()))
                                        {
                                            foreach (var itemLisAct in item.ListHorario.Where(w => w.State.Equals("Empalme")))
                                                entitylistGH.Add(itemLisAct.GrupoHorarioEntity);
                                            listGA.Add(new ListaEmpalmesInterno() { EntityGrupoParticipante = item.Entity, ListGrupoHorario = entitylistGH });
                                        }
                                        new cGrupoAsistencia().GenerarEmpalmes(listGA);
                                    }

                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al modificar el estatus de los integrantes del grupo", ex);
                                    return false;
                                }
                            });

                            if (respuesta)
                                await new Dialogos().ConfirmacionDialogoReturn("Manejo de Grupos", "El nuevo horario se agrego exitosamente");
                        }

                        CargarCalendarioGrupo(new cGrupo().GetData().Where(w => w.ID_GRUPO == SelectedGrupo.ID_GRUPO).FirstOrDefault());
                        TextError = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EDITAR_FECHA);
                        break;
                    case "cancelar_EdicionFecha":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EDITAR_FECHA);
                        break;
                    case "guardar_EdicionIntegrantesGrupo":
                        respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando estatus a los internos", () =>
                        {
                            try
                            {
                                var dal = new cGrupoParticipanteCancelado();
                                foreach (var item in ListInternosGrupo.Where(w => w.SelectEstatus != 2))
                                {
                                    dal.InsertarParticipanteCancelado(new GRUPO_PARTICIPANTE_CANCELADO()
                                    {
                                        ID_CENTRO = item.Entity.ID_CENTRO,
                                        ID_ACTIVIDAD = item.Entity.ID_ACTIVIDAD,
                                        ID_TIPO_PROGRAMA = item.Entity.ID_TIPO_PROGRAMA,
                                        ID_CONSEC = item.Entity.ID_CONSEC,
                                        ID_GRUPO = item.Entity.ID_GRUPO.Value,
                                        ID_USUARIO = GlobalVar.gUsr,
                                        SOLICITUD_FEC = Fechas.GetFechaDateServer,
                                        RESPUESTA_FEC = null,
                                        MOTIVO = item.MOTIVO,
                                        ID_ESTATUS = item.SelectEstatus
                                    });
                                }
                                return true;
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al modificar el estatus de los integrantes del grupo", ex);
                                return false;
                            }
                        });

                        if (respuesta)
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EDITAR_INTEGRANTES_GRUPO);
                            await new Dialogos().ConfirmacionDialogoReturn("Manejo de Grupo", "El cambio de estatus se actualizo exitosamente");
                        }
                        break;
                    case "cancelar_EdicionIntegranteGrupo":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EDITAR_INTEGRANTES_GRUPO);
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en la modificacion de la agenda", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.MANEJO_GRUPOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    //if (p.INSERTAR == 1)
                    //{
                    //    AgregarMenuEnabled = true;
                    //}
                    //if (p.CONSULTAR == 1)
                    //{
                    //    EjeEnabled = true;
                    //    ProgramaEnabled = true;
                    //    ActividadEnabled = true;
                    //    GrupoEnabled = true;
                    //}
                    //if (p.EDITAR == 1)
                    //    EditarMenuEnabled = true;
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
