using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    public partial class AgendaEnfermeroViewModel:ValidationViewModelBase
    {
        #region Generales
        public async void AgendaEnfermeroOnLoad(object sender)
        {
            try
            {
                AgendaEnfermeroView window = (AgendaEnfermeroView)sender;
                estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                window.Agenda.AddAppointment += Agenda_AddAppointment;
                BusquedaFecha = FechaAgenda = _FechaServer;
                ConfiguraPermisos();

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    roles = new cUsuarioRol().ObtenerTodos(StaticSourcesViewModel.UsuarioLogin.Username).Where(w => w.ID_CENTRO == GlobalVar.gCentro && (w.ID_ROL == (short)enumRolesAreasTecnicas.ENFERMERO)).Select(s => s.ID_ROL).ToList();
                    CargarCatalogos(true);
                    if (roles.Count == 1)
                    {
                        headerAgenda = "AGENDA de " + _empleado.NOMBRE_COMPLETO;
                        RaisePropertyChanged("HeaderAgenda");
                        var agenda = new cAtencionCita().ObtenerCitasPorUsuario(estatus_inactivos,new List<string>{"N", "A"},GlobalVar.gCentro, _empleado.Usuario.ID_USUARIO);
                        if (agenda != null)
                        {
                            fechasAgendadas = new List<DateTime>();
                            foreach (var a in agenda)
                            {
                                if (!fechasAgendadas.Contains(DateTime.Parse(a.CITA_FECHA_HORA.Value.ToShortDateString())))
                                    fechasAgendadas.Add(DateTime.Parse(a.CITA_FECHA_HORA.Value.ToShortDateString()));
                                lstAgenda.Add(new Appointment()
                                {
                                    Subject = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(a.INGRESO.IMPUTADO.NOMBRE) ? a.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(a.INGRESO.IMPUTADO.PATERNO) ? a.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.INGRESO.IMPUTADO.MATERNO) ? a.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty),
                                    StartTime = new DateTime(a.CITA_FECHA_HORA.Value.Year, a.CITA_FECHA_HORA.Value.Month, a.CITA_FECHA_HORA.Value.Day, a.CITA_FECHA_HORA.Value.Hour, a.CITA_FECHA_HORA.Value.Minute, a.CITA_FECHA_HORA.Value.Second),
                                    EndTime = new DateTime(a.CITA_HORA_TERMINA.Value.Year, a.CITA_HORA_TERMINA.Value.Month, a.CITA_HORA_TERMINA.Value.Day, a.CITA_HORA_TERMINA.Value.Hour, a.CITA_HORA_TERMINA.Value.Minute, a.CITA_HORA_TERMINA.Value.Second),
                                    ID_CITA = a.ID_CITA
                                });
                            }
                            lstAgenda = new ObservableCollection<Appointment>(lstAgenda);
                            RaisePropertyChanged("LstAgenda");
                            fechasAgendadas = new List<DateTime>(fechasAgendadas);
                            RaisePropertyChanged("FechasAgendadas");
                        }
                        if (permisos_crear)
                            IsAgendaEnabled = true;
                    }
                });
                if (fechasAgendadas != null && fechasAgendadas.Count > 0)
                    FechaInicial = fechasAgendadas.Min();
                else
                    FechaInicial = DateTime.Now;
                
                setBuscarAgendaValidacionRules();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar el modulo", ex);
            }
        }

        void Agenda_AddAppointment(object sender, System.Windows.RoutedEventArgs e)
        {
            return;
        }

        public async void OnClickSwitch(object parametro)
        {
            if (parametro != null)
                switch (parametro.ToString())
                {
                    case "buscar_agenda":
                        if (HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", Error));
                            return;
                        }
                        if (permisos_crear)
                            IsAgendaEnabled = true;
                        //Limpiar();
                        setReadOnlyDatosImputadosAgregarAgenda(true);
                        await CargarAgenda();
                        break;
                    case "cancelar_agregar_agenda_medica":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
                        setBuscarAgendaValidacionRules();
                        break;
                    case "agregar_agenda_medica":
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                            return;
                        }
                        if (AgregarAgendaHoraI.Value.Minute % 15 != 0 || AgregarAgendaHoraF.Value.Minute % 15 != 0)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "LOS INTERVALOS DE ATENCIÓN TIENEN QUE SER EN BLOQUES DE 15 MINUTOS!");
                            return;
                        }
                        var max_dias = Parametro.MAX_DIAS_ATENCION_MEDICA;
                        if (fechaOriginal.HasValue && fechaOriginal.Value.AddDays(max_dias) < AgregarAgendaFecha)
                        {

                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                CargarAtencionCitaInMotivo(true);
                            });

                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
                            setIncidenteMotivo();
                            Observacion = string.Empty;
                        }
                        else
                        {
                            AgregarAgenda();
                        }
                        break;
                    case "cancelar_incidente":
                        setAgregarAgendaValidacionRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
                        break;
                    case "agregar_incidente":
                        var _incidencia = new ATENCION_CITA_INCIDENCIA
                        {
                            ATENCION_ORIGINAL_FEC = fechaOriginal,
                            ID_ACMOTIVO = selectedIncidenteMotivoValue,
                            ID_USUARIO = GlobalVar.gUsr,
                            OBSERV = Observacion,
                            REGISTRO_FEC = _FechaServer,
                            ID_CENTRO_UBI = GlobalVar.gCentro
                        };
                        AgregarAgenda(_incidencia);
                        break;
                    case "buscar_menu":
                        selectIngreso = null;
                        SelectedBusquedaAgendaAtencionTipo = -1;
                        LimpiarBusqueda();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_AGENDA);
                        break;
                    case "filtro_busqueda_imputado_agenda":
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                            return;
                        }
                        BuscarImputadoAgenda();
                        break;
                    case "cancelar_buscar_agenda":
                        selectIngresoAuxiliar = null;
                        setBuscarAgendaValidacionRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_AGENDA);
                        break;
                    case "buscar_seleccionar":
                        var _tipo_error = 0;
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cargando información", () =>
                        {
                            if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                            {
                                _tipo_error = 1;
                                return false;
                            }

                            if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                            {
                                _tipo_error = 2;
                                return false;
                            }
                            if (tipoBusquedaImputado != ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO)
                            {
                                buscarAnioImputadoAgenda = selectIngreso.ID_ANIO;
                                RaisePropertyChanged("BuscarAnioImputadoAgenda");
                                buscarFolioImputadoAgenda = selectIngreso.ID_IMPUTADO;
                                RaisePropertyChanged("BuscarFolioImputadoAgenda");
                                buscarNombreImputadoAgenda = selectIngreso.IMPUTADO.NOMBRE;
                                RaisePropertyChanged("BuscarNombreImputadoAgenda");
                                buscarApPaternoImputadoAgenda = selectIngreso.IMPUTADO.PATERNO;
                                RaisePropertyChanged("BuscarApPaternoImputadoAgenda");
                                buscarApMaternoImputadoAgenda = selectIngreso.IMPUTADO.MATERNO;
                                RaisePropertyChanged("BuscarApMaternoImputadoAgenda");
                                //busquedaAgenda = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().ObtenerTodoPorImputado(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                                //    selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, fechaInicial, (short)eAreas.AREA_MEDICA, selectedBusquedaAgendaAtencionTipo.Value));
                                //RaisePropertyChanged("BusquedaAgenda");
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    buscarImagenImputadoAgenda = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    buscarImagenImputadoAgenda = new Imagenes().getImagenPerson();
                                RaisePropertyChanged("BuscarImagenImputadoAgenda");
                                headerBuscarAgenda = "Citas del imputado - " + selectIngreso.ID_ANIO.ToString() + "/" + selectIngreso.ID_IMPUTADO.ToString();
                                RaisePropertyChanged("HeaderBuscarAgenda");
                                if (new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica((short)eAreas.AREA_MEDICA, GlobalVar.gUsr))
                                    busquedaAgenda = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().ObtenerTodoPorImputadoCitasEnfermero(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO, selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, (short)enumRolesAreasTecnicas.ENFERMERO,null,selectedBusquedaAgendaAtencionTipo.HasValue && selectedBusquedaAgendaAtencionTipo.Value !=-1?(short?)selectedBusquedaAgendaAtencionTipo.Value:null,string.Empty ));
                                else
                                    busquedaAgenda = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().ObtenerTodoPorImputadoCitasEnfermero(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                                    selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, (short)enumRolesAreasTecnicas.ENFERMERO, null, selectedBusquedaAgendaAtencionTipo.HasValue && selectedBusquedaAgendaAtencionTipo.Value != -1 ? (short?)selectedBusquedaAgendaAtencionTipo.Value : null, GlobalVar.gUsr.Trim()));
                                RaisePropertyChanged("BusquedaAgenda");
                            }
                            selectIngresoAuxiliar = SelectIngreso;
                            return true;
                        }))
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            if (tipoBusquedaImputado == ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO)
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
                            else
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_AGENDA);
                        }
                        else
                        {
                            switch (_tipo_error)
                            {
                                case 1:
                                    new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                                    break;
                                case 2:
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                    SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                                    break;

                            }

                        }
                        break;
                    case "buscar_salir":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        resetIngreso();
                        if (tipoBusquedaImputado == ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO)
                        {
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
                        }
                        else
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_AGENDA);
                        break;
                    case "seleccionar_buscar_agenda":
                        if (SelectedBuscarAgenda == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "SELECCIONAR UNA CITA ES REQUERIDO!");
                            return;
                        }
                        SelectedEmpleadoValue = SelectedBuscarAgenda.ID_RESPONSABLE.Value;
                        //SelectedAtencionTipo = SelectedBuscarAgenda.ID_TIPO_ATENCION;
                        BusquedaFecha = SelectedBuscarAgenda.CITA_FECHA_HORA.Value;

                        if (permisos_crear)
                            IsAgendaEnabled = true;
                        await CargarAgenda();
                        selectIngresoAuxiliar = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_AGENDA);
                        setBuscarAgendaValidacionRules();
                        break;
                }
        }

        private void LimpiarBusqueda()
        {
            BuscarAnioImputadoAgenda = null;
            BuscarFolioImputadoAgenda = null;
            BuscarNombreImputadoAgenda = string.Empty;
            BuscarApPaternoImputadoAgenda = string.Empty;
            BuscarApMaternoImputadoAgenda = string.Empty;
        }

        public void OnModelChangedSwitch(object parametro)
        {
            try
            {
                if (parametro != null)
                {
                    switch (parametro.ToString())
                    {
                        case "cambio_fecha_agregar_agenda":
                            if (AgregarAgendaFecha.HasValue && AgregarAgendaFecha.Value.Date>=_FechaServer.Date)
                                AgregarAgendaFechaValid = true;
                            else
                                AgregarAgendaFechaValid = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error en el cambio de informacion", ex);
            }
        }

        private async void AgregarAgenda(ATENCION_CITA_INCIDENCIA _incidencia = null)
        {
            var _nueva_fecha_inicio = new DateTime(agregarAgendaFecha.Value.Year, agregarAgendaFecha.Value.Month, agregarAgendaFecha.Value.Day,
                                AgregarAgendaHoraI.Value.Hour, AgregarAgendaHoraI.Value.Minute, 0);
            var _nueva_fecha_final = new DateTime(agregarAgendaFecha.Value.Year, agregarAgendaFecha.Value.Month, agregarAgendaFecha.Value.Day,
                            AgregarAgendaHoraF.Value.Hour, AgregarAgendaHoraF.Value.Minute, 0);
            if (_nueva_fecha_inicio < Fechas.GetFechaDateServer)
            {
                new Dialogos().ConfirmacionDialogo("Validación!", "LA FECHA DE LA CITA NO PUEDE SER MENOR A LA FECHA Y HORA ACTUAL!");
                return;
            }
            var otra_cita = new cAtencionCita().ObtieneCitaOtraArea(selectedAtencion_Cita.INGRESO.ID_CENTRO, selectedAtencion_Cita.INGRESO.ID_ANIO,
                    selectedAtencion_Cita.INGRESO.ID_IMPUTADO, selectedAtencion_Cita.INGRESO.ID_INGRESO, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1), selectedAtencion_Cita.ID_CITA, selectedAtencion_Cita.ID_CENTRO_UBI);
            var _usuario_actualizar = lstAgendaEmpleados.First(w => w.ID_EMPLEADO == selectedAgendaEmpleadoValue);
            if (otra_cita == null)
            {
                var _overlap_cita=true;
                if (_usuario_actualizar.Usuario.USUARIO_ROL.Any(a=>a.ID_ROL!=(short)enumRolesAreasTecnicas.ENFERMERO))
                    _overlap_cita=new cAtencionCita().IsOverlapCita(selectedAtencion_Cita.ID_TIPO_ATENCION.Value, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1), estatus_inactivos, selectedAtencion_Cita.ID_CITA);
                else
                    _overlap_cita = new cAtencionCita().IsOverlapCitaporResponsable(_usuario_actualizar.ID_EMPLEADO, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1), estatus_inactivos, selectedAtencion_Cita.ID_CITA);
                if (!_overlap_cita) // a la hora final hay que restarle un minuto para no traslapar horarios que comienzan terminan a la misma hora
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando cita", () =>
                    {
                        var _atencion_cita = new ATENCION_CITA
                        {
                            CITA_FECHA_HORA = _nueva_fecha_inicio,
                            CITA_HORA_TERMINA = _nueva_fecha_final,
                            ESTATUS = selectedAtencion_Cita.ESTATUS,
                            ID_ANIO = selectedAtencion_Cita.ID_ANIO,
                            ID_AREA = selectedArea.Value,
                            ID_ATENCION = selectedAtencion_Cita.ID_ATENCION,
                            ID_ATENCION_MEDICA = selectedAtencion_Cita.ID_ATENCION_MEDICA,
                            ID_CENTRO = selectedAtencion_Cita.ID_CENTRO,
                            ID_CITA = selectedAtencion_Cita.ID_CITA,
                            ID_IMPUTADO = selectedAtencion_Cita.ID_IMPUTADO,
                            ID_INGRESO = selectedAtencion_Cita.ID_INGRESO,
                            ID_RESPONSABLE = _usuario_actualizar.ID_EMPLEADO,
                            ID_TIPO_ATENCION = selectedAtencion_Cita.ID_TIPO_ATENCION,
                            ID_TIPO_SERVICIO = selectedAtencion_Cita.ID_TIPO_SERVICIO,
                            ID_USUARIO = GlobalVar.gUsr,
                            ID_CENTRO_UBI = selectedAtencion_Cita.ID_CENTRO_UBI,
                            ID_CENTRO_AT_SOL = selectedAtencion_Cita.ID_CENTRO_AT_SOL
                        };
                        new cAtencionCita().Actualizar(_atencion_cita, _FechaServer, _incidencia, (short)enumMensajeTipo.INCIDENCIA_REPROGRAMACIÓN_ATENCIÓN_MÉDICA);
                        return true;
                    }))
                    {
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La cita fue actualizada con exito");
                        if (PopUpsViewModels.VisibleAgregarIncidenciaAtencionCita == Visibility.Visible)
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
                        else
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
                        setBuscarAgendaValidacionRules();
                        await CargarAgenda();
                    }

                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "LA CITA SE TRASLAPA CON OTRA CITA EN EL AREA MEDICA!");
                    return;
                }
            }
            else
            {
                new Dialogos().ConfirmacionDialogo("Validación!", "LA CITA SE TRASLAPA CON OTRA CITA DEL IMPUTADO!");
                return;
            }
        }

        private async Task CargarAgenda()
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                _empleado = lstEmpleados.Any(w => w.ID_EMPLEADO == selectedEmpleadoValue) ? lstEmpleados.First(w => w.ID_EMPLEADO == selectedEmpleadoValue) : null;
                headerAgenda = "AGENDA de " + _empleado.NOMBRE_COMPLETO;
                RaisePropertyChanged("HeaderAgenda");
                var agenda = new cAtencionCita().ObtenerCitasPorUsuario(estatus_inactivos, new List<string> { "N", "A" }, GlobalVar.gCentro, _empleado.Usuario.ID_USUARIO);
                if (agenda != null && agenda.Count() > 0)
                {
                    lstAgenda = new ObservableCollection<Appointment>();
                    fechasAgendadas = new List<DateTime>();
                    foreach (var a in agenda)
                    {
                        if (!fechasAgendadas.Contains(DateTime.Parse(a.CITA_FECHA_HORA.Value.ToShortDateString())))
                            fechasAgendadas.Add(DateTime.Parse(a.CITA_FECHA_HORA.Value.ToShortDateString()));
                        lstAgenda.Add(new Appointment()
                        {
                            Subject = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(a.INGRESO.IMPUTADO.NOMBRE) ? a.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(a.INGRESO.IMPUTADO.PATERNO) ? a.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.INGRESO.IMPUTADO.MATERNO) ? a.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty),
                            StartTime = new DateTime(a.CITA_FECHA_HORA.Value.Year, a.CITA_FECHA_HORA.Value.Month, a.CITA_FECHA_HORA.Value.Day, a.CITA_FECHA_HORA.Value.Hour, a.CITA_FECHA_HORA.Value.Minute, a.CITA_FECHA_HORA.Value.Second),
                            EndTime = new DateTime(a.CITA_HORA_TERMINA.Value.Year, a.CITA_HORA_TERMINA.Value.Month, a.CITA_HORA_TERMINA.Value.Day, a.CITA_HORA_TERMINA.Value.Hour, a.CITA_HORA_TERMINA.Value.Minute, a.CITA_HORA_TERMINA.Value.Second),
                            ID_CITA = a.ID_CITA
                        });
                    }
                    lstAgenda = new ObservableCollection<Appointment>(lstAgenda);
                    RaisePropertyChanged("LstAgenda");
                    fechasAgendadas = new List<DateTime>(fechasAgendadas);
                    RaisePropertyChanged("FechasAgendadas");
                }
                else
                {
                    lstAgenda = new ObservableCollection<Appointment>();
                    RaisePropertyChanged("LstAgenda");
                    fechasAgendadas = new List<DateTime>();
                    RaisePropertyChanged("FechasAgendadas");
                }
            });
            var fecha_minima = DateTime.Now;
            if (fechasAgendadas != null && fechasAgendadas.Count() > 0)
            {
                fecha_minima = fechasAgendadas.Min();
                FechaInicial = fechasAgendadas.Min();
            }
            if (fecha_minima >= BusquedaFecha.Value)
                BusquedaFecha = fecha_minima;
            FechaAgenda = BusquedaFecha.Value;
        }

        public async void AppointmentClick(object parametro)
        {

            if (parametro != null)
            {
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                    return;
                }
                var _fecha_servidor = Fechas.GetFechaDateServer;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    selectedAtencion_Cita = new cAtencionCita().Obtener(((Appointment)parametro).ID_CITA.Value, GlobalVar.gCentro);
                    if (new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica((short)eAreas.AREA_MEDICA, GlobalVar.gUsr))
                    {
                        
                        var _rol = selectedAtencion_Cita.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ? (short)enumRolesAreasTecnicas.MEDICO : (short)enumRolesAreasTecnicas.DENTISTA;
                        CargarEmpleadosAgenda((short)eAreas.AREA_MEDICA, new List<short>() {_rol,(short)enumRolesAreasTecnicas.ENFERMERO },true);
                        isAgendaEmpleadoEnabled = true;
                        RaisePropertyChanged("IsAgendaEmpleadoEnabled");
                    }
                    else
                    {
                        var _usuario_extendida = lstEmpleados.First(w=>w.ID_EMPLEADO==selectedEmpleadoValue);
                        lstAgendaEmpleados = new List<cUsuarioExtendida>() {
                                new cUsuarioExtendida{
                                    NOMBRE_COMPLETO=_usuario_extendida.NOMBRE_COMPLETO,
                                    ID_EMPLEADO=_usuario_extendida.ID_EMPLEADO,
                                    Usuario=_usuario_extendida.Usuario
                                }
                            };
                        RaisePropertyChanged("LstAgendaEmpleados");
                        isAgendaEmpleadoEnabled = false;
                        RaisePropertyChanged("IsAgendaEmpleadoEnabled");
                    }
                    selectedAgendaEmpleadoValue = selectedAtencion_Cita.ID_RESPONSABLE.Value;
                    RaisePropertyChanged("SelectedAgendaEmpleadoValue");


                    fechaOriginal = selectedAtencion_Cita.CITA_FECHA_HORA.Value;
                    folioImputadoAgregarAgenda = selectedAtencion_Cita.INGRESO.ID_IMPUTADO;
                    RaisePropertyChanged("FolioImputadoAgregarAgenda");
                    anioImputadoAgregarAgenda = selectedAtencion_Cita.INGRESO.ID_ANIO;
                    RaisePropertyChanged("AnioImputadoAgregarAgenda");
                    apPaternoAgregarAgenda = selectedAtencion_Cita.INGRESO.IMPUTADO.PATERNO;
                    RaisePropertyChanged("ApPaternoAgregarAgenda");
                    apMaternoAgregarAgenda = selectedAtencion_Cita.INGRESO.IMPUTADO.MATERNO;
                    RaisePropertyChanged("ApMaternoAgregarAgenda");
                    nombreAgregarAgenda = selectedAtencion_Cita.INGRESO.IMPUTADO.NOMBRE;
                    RaisePropertyChanged("NombreAgregarAgenda");
                    sexoImputadoAgregarAgenda = string.IsNullOrWhiteSpace(selectedAtencion_Cita.INGRESO.IMPUTADO.SEXO) ? string.Empty : selectedAtencion_Cita.INGRESO.IMPUTADO.SEXO == "M" ? "Masculino" : "Femenino";
                    RaisePropertyChanged("SexoImputadoAgregarAgenda");
                    edadImputadoAgregarAgenda = selectedAtencion_Cita.INGRESO.IMPUTADO.NACIMIENTO_FECHA.HasValue ? (short?)new Fechas().CalculaEdad(selectedAtencion_Cita.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value) : null;
                    RaisePropertyChanged("EdadImputadoAgregarAgenda");
                    selectedArea = selectedAtencion_Cita.ID_AREA;
                    RaisePropertyChanged("SelectedArea");
                    if (selectedAtencion_Cita.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        imagenAgregarAgenda = selectedAtencion_Cita.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        imagenAgregarAgenda = new Imagenes().getImagenPerson();
                    RaisePropertyChanged("ImagenAgregarAgenda");

                    agregarAgendaFecha = selectedAtencion_Cita.CITA_FECHA_HORA;
                    RaisePropertyChanged("AgregarAgendaFecha");
                    if (agregarAgendaFecha.HasValue)
                    {
                        agregarAgendaFechaValid = true;
                        RaisePropertyChanged("AgregarAgendaFechaValid");
                        agregarAgendaHoraI = agregarAgendaFecha.Value;
                        RaisePropertyChanged("AgregarAgendaHoraI");
                    }
                    if (selectedAtencion_Cita.CITA_HORA_TERMINA.HasValue)
                    {
                        agregarAgendaHoraF = selectedAtencion_Cita.CITA_HORA_TERMINA.Value;
                        RaisePropertyChanged("AgregarAgendaHoraF");
                        agregarAgendaHorasValid = true;
                        RaisePropertyChanged("AgregarAgendaHorasValid");
                    }
                    tipoServicioDescripcion = selectedAtencion_Cita.ATENCION_SERVICIO.DESCR;
                    RaisePropertyChanged("TipoServicioDescripcion");
                    if (AgregarAgendaFecha.HasValue && AgregarAgendaFecha.Value >= _FechaServer)
                        AgregarAgendaFechaValid = true;
                    else
                        AgregarAgendaFechaValid = false;
                });
                setAgregarAgendaValidacionRules();
                setReadOnlyDatosImputadosAgregarAgenda(true);
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
            }
        }

        private void setReadOnlyDatosImputadosAgregarAgenda(bool isReadOnly)
        {
            IsReadOnlyAnioImputadoAgregarAgenda = isReadOnly;
            IsReadOnlyApMaternoAgregarAgenda = isReadOnly;
            IsReadOnlyApPaternoAgregarAgenda = isReadOnly;
            IsReadOnlyFolioImputadoAgregarAgenda = isReadOnly;
            IsReadOnlyNombreAgregarAgenda = isReadOnly;
        }
        #endregion

        #region Cargar Catalogos
        private void CargarCatalogos(bool isExceptionManaged = false)
        {
            CargarEmpleados((short)eAreas.AREA_MEDICA, isExceptionManaged);
            //CargarAtencionTipo(isExceptionManaged);
            CargarAreas(isExceptionManaged);
            CargarBusquedaAgendaAtencionTipo(isExceptionManaged);
        }

        //private void CargarAtencionTipo(bool isExceptionManaged = false)
        //{
        //    try
        //    {
        //        lstAtencionTipos = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
        //        lstAtencionTipos.Insert(0, new ATENCION_TIPO
        //        {
        //            ID_TIPO_ATENCION = -1,
        //            DESCR = "SELECCIONE"
        //        });
        //        OnPropertyChanged("LstAtencionTipos");
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!isExceptionManaged)
        //            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de atencion", ex);
        //        else
        //            throw ex;
        //    }
        //}

        private void CargarAreas(bool isExceptionManaged = false)
        {
            try
            {
                lstAreas = new ObservableCollection<AREA>(new cArea().ObtenerTodos());
                lstAreas.Insert(0, new AREA
                {
                    ID_AREA = -1,
                    DESCR = "SELECCIONE"
                });
                OnPropertyChanged("LstAtencionTipos");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de atencion", ex);
                else
                    throw ex;
            }
        }

        //private void CargarBusquedaAgendaAtencionTipo(bool isExceptionManaged = false)
        //{
        //    try
        //    {
        //        lstBusquedaAgendaAtencionTipos = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
        //        lstBusquedaAgendaAtencionTipos.Insert(0, new ATENCION_TIPO
        //        {
        //            ID_TIPO_ATENCION = -1,
        //            DESCR = "SELECCIONE"
        //        });
        //        OnPropertyChanged("LstBusquedaAgendaAtencionTipos");
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!isExceptionManaged)
        //            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de atencion", ex);
        //        else
        //            throw ex;
        //    }
        //}

        public void CargarEmpleados(short area_tecnica, bool isExceptionManaged = false)
        {
            try
            {
                if (new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica(area_tecnica, GlobalVar.gUsr))
                {
                    lstEmpleados = new SSP.Controlador.Catalogo.Justicia.cUsuario().ObtenerTodosporAreaTecnica(GlobalVar.gCentro, area_tecnica, new List<short> { (short)enumRolesAreasTecnicas.ENFERMERO }).Select(s => new cUsuarioExtendida
                    {
                        Usuario = s,
                        NOMBRE_COMPLETO = s.EMPLEADO.PERSONA.NOMBRE.Trim() + (s.EMPLEADO.PERSONA.PATERNO == null || s.EMPLEADO.PERSONA.PATERNO.Trim() == "" ? "" : " " + s.EMPLEADO.PERSONA.PATERNO.Trim()) +
                           (s.EMPLEADO.PERSONA.MATERNO == null || s.EMPLEADO.PERSONA.MATERNO.Trim() == "" ? "" : " " + s.EMPLEADO.PERSONA.MATERNO.Trim()),
                        ID_EMPLEADO = s.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO
                    }).ToList();
                    RaisePropertyChanged("LstEmpleados");
                    if (lstEmpleados!=null && lstEmpleados.Count>0)
                    {
                        selectedEmpleadoValue = lstEmpleados.First().ID_EMPLEADO;
                        RaisePropertyChanged("SelectedEmpleadoValue");
                    }
                    isEmpleadoEnabled = true;
                    RaisePropertyChanged("IsEmpleadoEnabled");
                    _empleado = lstEmpleados[0];
                }
                else
                {

                    var _usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().Obtener(StaticSourcesViewModel.UsuarioLogin.Username);
                    if (_usuario.EMPLEADO.PERSONA.EMPLEADO.DEPARTAMENTO != null /*&& _usuario.EMPLEADO.PERSONA.EMPLEADO.DEPARTAMENTO.ID_TECNICA == (short)eAreas.AREA_MEDICA*/)
                    {
                        lstEmpleados = new List<cUsuarioExtendida>() { new cUsuarioExtendida{ Usuario=_usuario,
                        NOMBRE_COMPLETO=_usuario.EMPLEADO.PERSONA.NOMBRE.Trim() + (string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.PATERNO) ? string.Empty : " " + _usuario.EMPLEADO.PERSONA.PATERNO.Trim()) + 
                           (string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.MATERNO)?string.Empty: " " + _usuario.EMPLEADO.PERSONA.MATERNO.Trim()),
                           ID_EMPLEADO=_usuario.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO
                    } };
                        RaisePropertyChanged("LstEmpleados");
                        selectedEmpleadoValue = _usuario.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO;
                        RaisePropertyChanged("SelectedEmpleadoValue");
                        isEmpleadoEnabled = false;
                        RaisePropertyChanged("IsEmpleadoEnabled");
                        _empleado = lstEmpleados[0];
                    }

                }
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de atencion", ex);
                else
                    throw ex;
            }
        }

        public void CargarEmpleadosAgenda(short area_tecnica, List<short> roles, bool isExceptionManaged = false)
        {
            try
            {
                lstAgendaEmpleados = new SSP.Controlador.Catalogo.Justicia.cUsuario().ObtenerTodosporAreaTecnica(GlobalVar.gCentro, area_tecnica, roles).Select(s => new cUsuarioExtendida
                {
                    Usuario = s,
                    NOMBRE_COMPLETO = s.EMPLEADO.PERSONA.NOMBRE.Trim() + (s.EMPLEADO.PERSONA.PATERNO == null || s.EMPLEADO.PERSONA.PATERNO.Trim() == "" ? "" : " " + s.EMPLEADO.PERSONA.PATERNO.Trim()) +
                       (s.EMPLEADO.PERSONA.MATERNO == null || s.EMPLEADO.PERSONA.MATERNO.Trim() == "" ? "" : " " + s.EMPLEADO.PERSONA.MATERNO.Trim()),
                    ID_EMPLEADO = s.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO
                }).ToList();
                var _usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().Obtener(StaticSourcesViewModel.UsuarioLogin.Username);
                if (!lstAgendaEmpleados.Any(w => w.Usuario.ID_USUARIO == _usuario.ID_USUARIO))
                    lstAgendaEmpleados.Insert(0, new cUsuarioExtendida
                    {
                        Usuario = _usuario,
                        NOMBRE_COMPLETO = _usuario.EMPLEADO.PERSONA.NOMBRE.Trim() + (string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.PATERNO) ? string.Empty : " " + _usuario.EMPLEADO.PERSONA.PATERNO.Trim()) +
                        (string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.MATERNO) ? string.Empty : " " + _usuario.EMPLEADO.PERSONA.MATERNO.Trim()),
                        ID_EMPLEADO = _usuario.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO
                    });
                RaisePropertyChanged("LstAgendaEmpleados");
                if (lstAgendaEmpleados != null && lstAgendaEmpleados.Count > 0)
                {
                    selectedAgendaEmpleadoValue = selectedEmpleadoValue;
                    RaisePropertyChanged("SelectedAgendaEmpleadoValue");
                }
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de atencion", ex);
                else
                    throw ex;
            }
        }

        private void CargarAtencionCitaInMotivo(bool isExceptionManaged = false)
        {
            try
            {
                lstIncidenteMotivo = new ObservableCollection<ATENCION_CITA_IN_MOTIVO>(new cAtencion_Cita_In_Motivo().ObtenerTodos());
                lstIncidenteMotivo.Insert(0, new ATENCION_CITA_IN_MOTIVO
                {
                    ID_ACMOTIVO = -1,
                    DESCR = "SELECCIONE"
                });
                OnPropertyChanged("LstIncidenteMotivo");
                selectedIncidenteMotivoValue = -1;
                RaisePropertyChanged("SelectedIncidenteMotivoValue");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de motivos de incidencia", ex);
                else
                    throw ex;
            }
        }

        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                MenuBuscarEnabled = false;
                permisos_crear = false;
                permisos = new cProcesoUsuario().Obtener(enumProcesos.AGENDAENFERMERO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Count() > 0)
                    MenuLimpiarEnabled = true;
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        permisos_crear = true;
                    if (p.CONSULTAR == 1)
                    {
                        MenuBuscarEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region Buscar Imputado

        #region Busqueda de Agenda
        private async void BuscarImputadoAgenda()
        {
            try
            {
                NombreBuscar = BuscarNombreImputadoAgenda;
                ApellidoPaternoBuscar = BuscarApPaternoImputadoAgenda;
                ApellidoMaternoBuscar = BuscarApMaternoImputadoAgenda;
                FolioBuscar = BuscarFolioImputadoAgenda;
                AnioBuscar = BuscarAnioImputadoAgenda;
                ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (AnioBuscar.HasValue && FolioBuscar.HasValue && ListExpediente.Count() == 1)
                {
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    selectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    if (selectIngreso == null || ListExpediente[0].INGRESO.Count() == 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso para este imputado.");
                        resetIngreso();
                        return;
                    }
                    if (estatus_inactivos.Contains(selectIngreso.ID_ESTATUS_ADMINISTRATIVO))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                        resetIngreso();
                        return;
                    }
                    if (selectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        resetIngreso();
                        return;
                    }
                    if (selectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                            ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                        resetIngreso();
                        return;
                    }

                    BuscarFolioImputadoAgenda = selectIngreso.ID_IMPUTADO;
                    BuscarAnioImputadoAgenda = selectIngreso.ID_ANIO;
                    BuscarApPaternoImputadoAgenda = selectIngreso.IMPUTADO.PATERNO;
                    BuscarApMaternoImputadoAgenda = selectIngreso.IMPUTADO.MATERNO;
                    BuscarNombreImputadoAgenda = selectIngreso.IMPUTADO.NOMBRE;
                    if (selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        BuscarImagenImputadoAgenda = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        BuscarImagenImputadoAgenda = new Imagenes().getImagenPerson();
                    HeaderBuscarAgenda = "Citas del imputado - " + selectIngreso.ID_ANIO.ToString() + "/" + selectIngreso.ID_IMPUTADO.ToString();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        if (new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica((short)eAreas.AREA_MEDICA, GlobalVar.gUsr))
                            busquedaAgenda = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().ObtenerTodoPorImputadoCitasEnfermero(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                                selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, (short) enumRolesAreasTecnicas.ENFERMERO, null, selectedBusquedaAgendaAtencionTipo.HasValue && selectedBusquedaAgendaAtencionTipo!=-1?(short?)selectedBusquedaAgendaAtencionTipo.Value:null));
                        else
                            busquedaAgenda = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().ObtenerTodoPorImputadoCitasEnfermero(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                                selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO,(short)enumRolesAreasTecnicas.ENFERMERO,null, selectedBusquedaAgendaAtencionTipo.HasValue && selectedBusquedaAgendaAtencionTipo.Value!=-1?(short?)selectedBusquedaAgendaAtencionTipo.Value:null, GlobalVar.gUsr.Trim()));
                        RaisePropertyChanged("BusquedaAgenda");
                    });
                    selectIngresoAuxiliar = selectIngreso;
                }
                else
                {
                    tipoBusquedaImputado = ModoBusqueda.BUSQUEDA_AGENDA_IMPUTADO;
                    SelectIngresoEnabled = false;
                    if (ListExpediente != null)
                        EmptyExpedienteVisible = ListExpediente.Count < 0;
                    else
                        EmptyExpedienteVisible = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_AGENDA);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    CrearNuevoExpedienteEnabled = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar el imputado", ex);
            }
        }
        #endregion

        private void ClickBuscarInterno(object parametro)
        {
            buscarImputadoInterno(parametro);
        }


        private async void BuscarImputado()
        {
            try
            {
                NombreBuscar = NombreAgregarAgenda;
                ApellidoPaternoBuscar = ApPaternoAgregarAgenda;
                ApellidoMaternoBuscar = ApMaternoAgregarAgenda;
                FolioBuscar = FolioImputadoAgregarAgenda;
                AnioBuscar = AnioImputadoAgregarAgenda;
                ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (AnioBuscar.HasValue && FolioBuscar.HasValue && ListExpediente.Count() == 1)
                {
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    selectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    if (selectIngreso == null || ListExpediente[0].INGRESO.Count() == 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso para este imputado.");
                        return;
                    }
                    var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    foreach (var item in EstatusInactivos)
                    {
                        if (selectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                            resetIngreso();
                            return;
                        }
                    }
                    if (selectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        resetIngreso();
                        return;
                    }
                    if (selectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                            ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                        resetIngreso();
                        return;
                    }
                    if (!selectIngreso.ATENCION_MEDICA.Any(a => a.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && a.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                            ListExpediente[0].ID_IMPUTADO.ToString() + "] no cuenta con su certificado medico de ingreso.");
                        resetIngreso();
                        return;
                    }



                    FolioImputadoAgregarAgenda = selectIngreso.ID_IMPUTADO;
                    AnioImputadoAgregarAgenda = selectIngreso.ID_ANIO;
                    ApPaternoAgregarAgenda = selectIngreso.IMPUTADO.PATERNO;
                    ApMaternoAgregarAgenda = selectIngreso.IMPUTADO.MATERNO;
                    NombreAgregarAgenda = selectIngreso.IMPUTADO.NOMBRE;
                    SexoImputadoAgregarAgenda = string.IsNullOrWhiteSpace(selectIngreso.IMPUTADO.SEXO) ? string.Empty : selectIngreso.IMPUTADO.SEXO == "M" ? "Masculino" : "Femenino";
                    EdadImputadoAgregarAgenda = selectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? (short?)new Fechas().CalculaEdad(selectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value) : null;
                    if (selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        imagenAgregarAgenda = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        imagenAgregarAgenda = new Imagenes().getImagenPerson();
                    RaisePropertyChanged("ImagenAgregarAgenda");
                    selectIngresoAuxiliar = selectIngreso;
                }
                else
                {
                    tipoBusquedaImputado = ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO;
                    SelectIngresoEnabled = false;
                    if (ListExpediente != null)
                        EmptyExpedienteVisible = ListExpediente.Count < 0;
                    else
                        EmptyExpedienteVisible = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    CrearNuevoExpedienteEnabled = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar el imputado", ex);
            }
        }

        private async void buscarImputadoInterno(Object obj = null)
        {
            ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
            ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
            ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
            SelectIngresoEnabled = false;
            if (ListExpediente != null)
                EmptyExpedienteVisible = ListExpediente.Count < 0;
            else
                EmptyExpedienteVisible = true;
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
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

        private void resetIngreso()
        {
            SelectIngreso = selectIngresoAuxiliar;
            if (selectIngresoAuxiliar != null)
            {
                if (tipoBusquedaImputado == ModoBusqueda.BUSQUEDA_AGENDA_IMPUTADO)
                {
                    buscarAnioImputadoAgenda = selectIngresoAuxiliar.ID_ANIO;
                    RaisePropertyChanged("BuscarAnioImputadoAgenda");
                    buscarFolioImputadoAgenda = selectIngresoAuxiliar.ID_IMPUTADO;
                    RaisePropertyChanged("BuscarFolioImputadoAgenda");
                    buscarNombreImputadoAgenda = selectIngresoAuxiliar.IMPUTADO.NOMBRE;
                    RaisePropertyChanged("BuscarNombreImputadoAgenda");
                    buscarApPaternoImputadoAgenda = selectIngresoAuxiliar.IMPUTADO.PATERNO;
                    RaisePropertyChanged("BuscarApPaternoImputadoAgenda");
                    buscarApMaternoImputadoAgenda = selectIngresoAuxiliar.IMPUTADO.MATERNO;
                    RaisePropertyChanged("BuscarApMaternoImputadoAgenda");
                }
                else if (tipoBusquedaImputado == ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO)
                {
                    folioImputadoAgregarAgenda = selectIngresoAuxiliar.ID_IMPUTADO;
                    RaisePropertyChanged("FolioImputadoAgregarAgenda");
                    anioImputadoAgregarAgenda = selectIngresoAuxiliar.ID_ANIO;
                    RaisePropertyChanged("AnioImputadoAgregarAgenda");
                    apPaternoAgregarAgenda = selectIngresoAuxiliar.IMPUTADO.PATERNO;
                    RaisePropertyChanged("ApPaternoAgregarAgenda");
                    apMaternoAgregarAgenda = selectIngresoAuxiliar.IMPUTADO.MATERNO;
                    RaisePropertyChanged("ApMaternoAgregarAgenda");
                    nombreAgregarAgenda = selectIngresoAuxiliar.IMPUTADO.NOMBRE;
                    RaisePropertyChanged("NombreAgregarAgenda");
                }
            }
        }
        #endregion

        private void CargarBusquedaAgendaAtencionTipo(bool isExceptionManaged = false)
        {
            try
            {
                lstBusquedaAgendaAtencionTipos = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                lstBusquedaAgendaAtencionTipos.Insert(0, new ATENCION_TIPO
                {
                    ID_TIPO_ATENCION = -1,
                    DESCR = "TODAS"
                });
                OnPropertyChanged("LstBusquedaAgendaAtencionTipos");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de atencion", ex);
                else
                    throw ex;
            }
        }
    }
}
