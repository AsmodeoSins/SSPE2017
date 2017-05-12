using ControlPenales;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class CitaViewModel : ValidationViewModelBase
    {
        #region constructor
        public CitaViewModel()
        {
            //Prueba Agenda
        }
        #endregion

        #region metodos
        private void AgendaSwitch(Object obj)
        {
            try
            {
                AHoraI = new DateTime(SelectedDateCalendar.Value.Year, SelectedDateCalendar.Value.Month, SelectedDateCalendar.Value.Day,
                    int.Parse(obj.ToString().Split(':')[0]), int.Parse(obj.ToString().Split(':')[1]), 0);
                AHoraF = new DateTime(SelectedDateCalendar.Value.Year, SelectedDateCalendar.Value.Month, SelectedDateCalendar.Value.Day,
                    int.Parse(obj.ToString().Split(':')[1]) == 45 ? int.Parse(obj.ToString().Split(':')[0]) + 1 : int.Parse(obj.ToString().Split(':')[0]),
                    int.Parse(obj.ToString().Split(':')[1]) == 45 ? int.Parse(obj.ToString().Split(':')[1]) - 45 : int.Parse(obj.ToString().Split(':')[1]) + 15, 0);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar nuevos datos a la agenda.", ex);
            }
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_menu":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO);
                    break;
                case "reporte_menu":
                    break;
                case "buscar":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    ObtenerSolicitudes();
                    break;
                case "guardar_menu":
                    var x = string.Empty;
                    break;
                case "agendar_cita":
                    try
                    {
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }
                        if (LstAgenda == null)
                            LstAgenda = new ObservableCollection<Appointment>();
                        AgendaView = new AgendarCitaView();                        
                        SelectedDateCalendar = fHoy;
                        AgendaView.Loaded += AgendaLoaded;
                        AgendaView.Agenda.AddAppointment += AgendaAddAppointment;
                        //AgendaView.Agenda.CurrentDate
                        AgendaView.btn_guardar.Click += AgendaClick;
                        AgendaView.Closing += AgendaClosing;
                        AgendaView.Owner = PopUpsViewModels.MainWindow;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        AgendaView.ShowDialog();
                        AgendaView.Loaded -= AgendaLoaded;
                        AgendaView.Agenda.AddAppointment -= AgendaAddAppointment;
                        AgendaView.btn_guardar.Click -= AgendaClick;
                        AgendaView.Closing -= AgendaClosing;
                        AgendaView = null;
                        
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar nuevos datos a la agenda.", ex);
                    }
                    break;
                case "descartar_solicitud":
                    try
                    {
                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }
                        if (SelectedSolicitud != null)
                        {
                            if (await new Dialogos().ConfirmarEliminar("Vaidación", "¿Esta seguro que desea descartar una solicitud?") == 1)
                            {
                                if (ActualizaEstatusSolicitud((short)enumSolicitudCita.DESCARTADA))
                                {
                                    ObtenerSolicitudes();
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Error", "Ocurrió un problema al cambiar el estatus de la solicitud.");
                            }
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una solicitud");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar nuevos datos a la agenda.", ex);
                    }
                    break;
                //case "cancelar_agenda":
                //    CancelarAgenda();
                //    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CitaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new CitaViewModel();
                    break;
                case "ver_agenda":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                        _empleado =lstEmpleados.Any(w=>w.ID_EMPLEADO==selectedEmpleadoValue)?lstEmpleados.First(w=>w.ID_EMPLEADO==selectedEmpleadoValue):null;
                        if (_empleado!=null)
                        {
                            tituloAgenda = "AGENDA - " + _empleado.NOMBRE_COMPLETO;
                            RaisePropertyChanged("TituloAgenda");
                            ObtenerAgenda(_empleado.Usuario.ID_USUARIO,true);
                            isAgendarCitaEnabled = true;
                            RaisePropertyChanged("IsAgendarCitaEnabled");
                        }           
                    });
                    break;
            }
        }

        public async Task CargarEmpleados()
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                if (new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica(selectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TECNICA.Value, GlobalVar.gUsr))
                {
                    lstEmpleados = new SSP.Controlador.Catalogo.Justicia.cUsuario().ObtenerTodosporAreaTecnica(GlobalVar.gCentro, selectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TECNICA.Value,
                        selectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TECNICA.Value==(short)eAreas.AREA_MEDICA?
                        selectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TIPO_ATENCION.Value==(short)enumAtencionTipo.CONSULTA_MEDICA?
                        new List<short>{(short)enumRolesAreasTecnicas.MEDICO}:selectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TIPO_ATENCION.Value==(short)enumAtencionTipo.CONSULTA_DENTAL?
                        new List<short>{(short)enumRolesAreasTecnicas.DENTISTA}:null:null).Select(s => new cUsuarioExtendida
                    {
                        Usuario = s,
                        NOMBRE_COMPLETO = s.EMPLEADO.PERSONA.NOMBRE.Trim() + (s.EMPLEADO.PERSONA.PATERNO == null || s.EMPLEADO.PERSONA.PATERNO.Trim() == "" ? "" : " " + s.EMPLEADO.PERSONA.PATERNO.Trim()) +
                           (s.EMPLEADO.PERSONA.MATERNO == null || s.EMPLEADO.PERSONA.MATERNO.Trim() == "" ? "" : " " + s.EMPLEADO.PERSONA.MATERNO.Trim()),
                        ID_EMPLEADO = s.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO
                    }).ToList();
                    var _usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().Obtener(StaticSourcesViewModel.UsuarioLogin.Username);
                    if (!lstEmpleados.Any(w=>w.Usuario.ID_USUARIO==_usuario.ID_USUARIO))
                        lstEmpleados.Insert(0, new cUsuarioExtendida{ Usuario=_usuario,
                                                                      NOMBRE_COMPLETO = _usuario.EMPLEADO.PERSONA.NOMBRE.Trim() + (string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.PATERNO) ? string.Empty : " " + _usuario.EMPLEADO.PERSONA.PATERNO.Trim()) +
                            (string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.MATERNO) ? string.Empty : " " + _usuario.EMPLEADO.PERSONA.MATERNO.Trim()),
                               ID_EMPLEADO = _usuario.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO
                            });
                    RaisePropertyChanged("LstEmpleados");
                    selectedEmpleadoValue = _usuario.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO; 
                    RaisePropertyChanged("SelectedEmpleadoValue");
                    isEmpleadoEnabled = true;
                    RaisePropertyChanged("IsEmpleadoEnabled");
                    _empleado = lstEmpleados[0];
                    tituloAgenda = "AGENDA - " + _empleado.NOMBRE_COMPLETO;
                    RaisePropertyChanged("TituloAgenda");
                    isAgendarCitaEnabled = true;
                    RaisePropertyChanged("IsAgendarCitaEnabled");
                    ObtenerAgenda(_usuario.ID_USUARIO, true);
                }
                else
                {
                    var _usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().Obtener(StaticSourcesViewModel.UsuarioLogin.Username);
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
                    tituloAgenda = "AGENDA - " + _empleado.NOMBRE_COMPLETO;
                    RaisePropertyChanged("TituloAgenda");
                    isAgendarCitaEnabled = true;
                    RaisePropertyChanged("IsAgendarCitaEnabled");
                    ObtenerAgenda(_usuario.ID_USUARIO,true);
                }
            });
        }

        private void AgendaAddAppointment(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is Calendar))
                    return;
                AgregarHora = !AgregarHora;
                if (AgregarHora)
                {
                    ValidacionSolicitud();
                    var horaini = AHoraI;
                    var horafin = AHoraF;
                    LimpiarAgenda();
                    AHoraI = horaini;
                    AHoraF = horafin;
                    AFecha = new DateTime(((Calendar)sender).CurrentDate.Year, ((Calendar)sender).CurrentDate.Month, ((Calendar)sender).CurrentDate.Day);
                }
                else
                    base.ClearRules();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar nuevos datos a la agenda.", ex);
            }
        }

        private void AgendaClosing(object sender, EventArgs e)
        {
            try
            {
                ObtenerSolicitudes();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la agenda.", ex);
            }
        }

        private async void AgendaLoaded(object sender, EventArgs e)
        {
            try
            {
                AgendaView.DataContext = this;
                await CargarEmpleados();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la agenda.", ex);
            }
        }

        private void AgendaClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!base.HasErrors)
                {
                    var hoy = Fechas.GetFechaDateServer;
                    var _nueva_fecha_inicio = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraI.Value.Hour, AHoraI.Value.Minute, AHoraI.Value.Second);
                    var _nueva_fecha_final=new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraF.Value.Hour, AHoraF.Value.Minute, AHoraF.Value.Second);
                    if (hoy>_nueva_fecha_inicio)
                    {
                        MensajeError = "La fecha y hora deben de ser mayor a la actual";
                        return;
                    }           
                    if (AHoraI.Value.Minute % 15 != 0 || AHoraF.Value.Minute % 15 != 0)
                    {
                        MensajeError = "Los intervalos de atención tienen que ser en bloques de 15 minutos";
                        return;
                    }
                    if (GuardarAgenda())
                    {
                        if (AFecha != null)
                        {
                            LstAgenda.Add(new Appointment()
                            {
                                Subject = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectedSolicitud.ATENCION_INGRESO.INGRESO.IMPUTADO.NOMBRE) ? SelectedSolicitud.ATENCION_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedSolicitud.ATENCION_INGRESO.INGRESO.IMPUTADO.PATERNO) ? SelectedSolicitud.ATENCION_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedSolicitud.ATENCION_INGRESO.INGRESO.IMPUTADO.MATERNO) ? SelectedSolicitud.ATENCION_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty),
                                StartTime = _nueva_fecha_inicio,
                                EndTime = _nueva_fecha_final,
                            });
                            AgendaView.Agenda.FilterAppointments();
                            AgregarHora = !AgregarHora;
                            AgendaView.Close();
                            base.ClearRules();
                        }
                    }
                    else
                        //    new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un problema al guardar la fecha");
                        MensajeError = "Ocurrió un problema al guardar la fecha";
                }
                else
                {
                    if (AHoraI.HasValue && AHoraF.HasValue && !AHorasValid)
                        MensajeError = "La hora fin debe ser mayor a la hora inicio de la cita";
                    else
                        MensajeError = string.Format("Faltan datos por capturar: {0}.", base.Error);
                };
                //    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la agenda.", ex);
            }
        }

        private async void OnLoad(CitaView obj = null)
        {
            try
            {
                estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                ConfiguraPermisos();
                var _usuario = new cUsuario().ObtenerUsuario(StaticSourcesViewModel.UsuarioLogin.Username);

                //areas tecnicas de roles no coordinadores
                int[] _roles_AT = Array.ConvertAll(Enum.GetValues(typeof(enumRolesAreasTecnicas)).Cast<enumRolesAreasTecnicas>().ToArray(), value => (int)value);
                //if (_usuario.USUARIO_ROL.Any(w => _roles_AT.Contains(w.ID_ROL)) && _usuario.EMPLEADO.DEPARTAMENTO!=null /*&& _usuario.EMPLEADO.DEPARTAMENTO.ID_TECNICA.HasValue*/)
                if (_usuario.USUARIO_ROL.Any(w => _roles_AT.Contains(w.ID_ROL) && w.ID_CENTRO==GlobalVar.gCentro) && _usuario.EMPLEADO.DEPARTAMENTO != null && _usuario.EMPLEADO.DEPARTAMENTO.DEPARTAMENTO_AREA_TECNICA!=null)
                {
                    //areas_tecnicas_rol.Add(_usuario.EMPLEADO.DEPARTAMENTO.ID_TECNICA.Value);
                    areas_tecnicas_rol.AddRange(_usuario.EMPLEADO.DEPARTAMENTO.DEPARTAMENTO_AREA_TECNICA.Select(s => s.ID_TECNICA));
                }
                //areas tecnicas de roles de coordinadores.
                foreach (var _at_item in new cTecnicaArea().ObtenerporUsuario(StaticSourcesViewModel.UsuarioLogin.Username))
                    if (!areas_tecnicas_rol.Contains(_at_item.ID_TECNICA))
                        areas_tecnicas_rol.Add(_at_item.ID_TECNICA);

                //roles de coordinador de area tecnica
                if (_usuario.USUARIO_ROL.Any(w=>w.ID_ROL==(short)enumRolesCoordinadoresAreasTecnicas.COORDINADOR_AREA_TECNICA && w.ID_CENTRO==GlobalVar.gCentro))
                {
                    foreach (var _at_item in new cTecnicaArea().ObtenerporDepartamento().Select(s => s.ID_TECNICA))
                        if (!areas_tecnicas_rol.Contains(_at_item))
                            areas_tecnicas_rol.Add(_at_item);
                }
                if (PConsultar)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
                }                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las citas.", ex);
            }
        }

        private void ClickEnter(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                if (obj != null)
                {
                    var textbox = (TextBox)obj;
                    switch (textbox.Name)
                    {
                        case "AnioBuscar":
                            if (!string.IsNullOrEmpty(textbox.Text))
                                CAnio = short.Parse(textbox.Text);
                            else
                                CAnio = null;
                            break;
                        case "FolioBuscar":
                            if (!string.IsNullOrEmpty(textbox.Text))
                                CFolio = int.Parse(textbox.Text);
                            else
                                CFolio = null;
                            break;
                        case "PaternoBuscar":
                            CPaterno = textbox.Text;
                            break;
                        case "MaternoBuscar":
                            CMaterno = textbox.Text;
                            break;
                        case "NombreBuscar":
                            CNombre = textbox.Text;
                            break;
                    }
                    ObtenerSolicitudes();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private void CargarListas()
        {
            try
            {
                LstArea = new ObservableCollection<AREA_TECNICA>(new cAreaTecnica().ObtenerTodo().Where(w=>areas_tecnicas_rol.Contains(w.ID_TECNICA)));

                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {

                    LstArea.Insert(0, new AREA_TECNICA() { ID_TECNICA = -1, DESCR = "SELECCIONE" });
                    ObtenerSolicitudes();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listados", ex);
            }
        }

        private async Task<List<cSolicitudCita>> SegmentarBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var _max_atenciones = Parametro.SOLICITUD_ATENCION_POR_MES;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<cSolicitudCita>>(() =>
                             new ObservableCollection<cSolicitudCita>(new cAtencionIngreso().ObtenerTodoEnAreas(GlobalVar.gCentro, true, null, CAnio, CFolio, CNombre, CPaterno, CMaterno,
                                 CArea != -1 ? new List<short> { CArea.Value } : areas_tecnicas_rol, CFecha, CEstatus != -1 ? CEstatus : null, fHoy, _Pag)
                                 .Select(s => new cSolicitudCita
                                 {
                                     ATENCION_INGRESO = s,
                                     CANT_SOLICITUDES_ATENDIDAS = s.INGRESO.ATENCION_CITA.Where(w =>w.ATENCION_SOLICITUD!=null  
                                           && w.CITA_FECHA_HORA.Value.Month == s.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.Month && w.CITA_FECHA_HORA.Value.Year == s.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.Year).Count(),
                                     MAX_ATENCIONES=_max_atenciones
                                 })));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargandoIngresos = true;
                }
                else
                    SeguirCargandoIngresos = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar internos.", ex);
                return new List<cSolicitudCita>();
            }
        }
        #endregion

        #region Solicitud
        private async void ObtenerSolicitudes()
        {
            try
            {
                LstSolicitudes = new RangeEnabledObservableCollection<cSolicitudCita>();
                LstSolicitudes.InsertRange(await SegmentarBusqueda());
                EmptySolicitudes = LstSolicitudes.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las solicitudes", ex);
            }
        }
        #endregion

        #region Agenda
        private bool GuardarAgenda()
        {
            try
            {
                if (SelectedSolicitud != null)
                {
                    if (!base.HasErrors)
                    {
                        var obj = new ATENCION_CITA();
                        obj.ID_CENTRO_UBI = SelectedSolicitud.ATENCION_INGRESO.ID_CENTRO_UBI;
                        obj.ID_CITA = 0;
                        obj.ESTATUS = "N";
                        obj.ID_CENTRO = SelectedSolicitud.ATENCION_INGRESO.ID_CENTRO;
                        obj.ID_ANIO = SelectedSolicitud.ATENCION_INGRESO.ID_ANIO;
                        obj.ID_IMPUTADO = SelectedSolicitud.ATENCION_INGRESO.ID_IMPUTADO;
                        obj.ID_INGRESO = SelectedSolicitud.ATENCION_INGRESO.ID_INGRESO;
                        obj.ID_ATENCION = SelectedSolicitud.ATENCION_INGRESO.ID_ATENCION;
                        obj.CITA_FECHA_HORA = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraI.Value.Hour, AHoraI.Value.Minute, 0);
                        obj.CITA_HORA_TERMINA = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraF.Value.Hour, AHoraF.Value.Minute, 0);
                        obj.ID_TIPO_ATENCION = SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TIPO_ATENCION;
                        obj.ID_AREA = SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_AREA;
                        obj.ID_TIPO_SERVICIO = SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TECNICA == (short)eAreas.AREA_MEDICA ? SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                            (short?)enumAtencionServicio.CITA_MEDICA : SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? (short?)enumAtencionServicio.CITA_MEDICA_DENTAL : null : null;
                        obj.ID_RESPONSABLE = _empleado.ID_EMPLEADO;
                        obj.ID_CENTRO_AT_SOL = SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_CENTRO;
                        obj.ID_USUARIO = GlobalVar.gUsr;
                        obj.ID_CITA = new cAtencionCita().Agregar(obj);
                        if (obj.ID_CITA > 0)
                        {
                            ActualizaEstatusSolicitud((short)enumSolicitudCita.AGENDADA);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar agenda.", ex);
            }
            return false;
        }

        private void ObtenerAgenda(string usuario, bool isExceptionManaged=false)
        {
            try
            {
                if (selectedSolicitud != null)
                {
                    lstAgenda = new ObservableCollection<Appointment>();
                    var hoy = Fechas.GetFechaDateServer;
                    var agenda = new cAtencionCita().ObtenerPorUsuarioDesdeFecha(GlobalVar.gCentro, usuario, Fechas.GetFechaDateServer,estatus_inactivos); 
                    if (agenda != null)
                    {
                        foreach (var a in agenda)
                        {
                            lstAgenda.Add(new Appointment()
                            {
                                Subject = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(a.INGRESO.IMPUTADO.NOMBRE) ? a.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(a.INGRESO.IMPUTADO.PATERNO) ? a.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.INGRESO.IMPUTADO.MATERNO) ? a.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty),
                                StartTime = new DateTime(a.CITA_FECHA_HORA.Value.Year, a.CITA_FECHA_HORA.Value.Month, a.CITA_FECHA_HORA.Value.Day, a.CITA_FECHA_HORA.Value.Hour, a.CITA_FECHA_HORA.Value.Minute, a.CITA_FECHA_HORA.Value.Second),
                                EndTime = new DateTime(a.CITA_HORA_TERMINA.Value.Year, a.CITA_HORA_TERMINA.Value.Month, a.CITA_HORA_TERMINA.Value.Day, a.CITA_HORA_TERMINA.Value.Hour, a.CITA_HORA_TERMINA.Value.Minute, a.CITA_HORA_TERMINA.Value.Second),
                            });
                        }
                    }
                    RaisePropertyChanged("LstAgenda");
                }
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la agenda", ex);
                else
                    throw ex;
            }
        }

        private void LimpiarAgenda()
        {
            AFechaValid = AHorasValid = false;
            AFecha = AHoraI = AHoraF = null;
        }

        //no se puede cancelar una agenda de manera manual
        //private async void CancelarAgenda()
        //{
        //    if (SelectedSolicitud != null)
        //    {
        //        var ac = SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ATENCION_CITA.Where(w => w.ID_CENTRO == SelectedSolicitud.ATENCION_INGRESO.ID_CENTRO && w.ID_ANIO == SelectedSolicitud.ATENCION_INGRESO.ID_ANIO && w.ID_IMPUTADO == SelectedSolicitud.ATENCION_INGRESO.ID_IMPUTADO && w.ID_INGRESO == SelectedSolicitud.ATENCION_INGRESO.ID_INGRESO).FirstOrDefault();
        //        if (ac != null)
        //        {
        //            if (ac.CITA_FECHA_HORA < fHoy)
        //            {
        //                new Dialogos().ConfirmacionDialogo("Éxito", "No se puede cancelar citas de una fecha pasada");
        //                return;
        //            }
        //        }
        //        if (await new Dialogos().ConfirmarEliminar("Validación", "¿Esta seguro que desea cancelar este horario agendado?") == 1)
        //        {
        //            if (ActualizaEstatusSolicitud((short)enumSolicitudCita.CANCELADA))
        //            {
        //                SelectedSolicitud.ATENCION_INGRESO.ESTATUS = (short)enumSolicitudCita.CANCELADA;
        //                var x = LstSolicitudes;
        //                LstSolicitudes = new RangeEnabledObservableCollection<cSolicitudCita>();
        //                LstSolicitudes.InsertRange(x);
        //                new Dialogos().ConfirmacionDialogo("Éxito", "Se ha cancelado correctamente");
        //            }
        //            else
        //                new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al cancelar");
        //        }
        //    }
        //    else
        //        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una solicitud");

        //}

        private bool ActualizaEstatusSolicitud(short estatus)
        {
            try
            {
                if (SelectedSolicitud != null)
                {
                    var obj = new ATENCION_INGRESO();
                    #region copia
                    obj.ID_ATENCION = SelectedSolicitud.ATENCION_INGRESO.ID_ATENCION;
                    obj.ID_CENTRO = SelectedSolicitud.ATENCION_INGRESO.ID_CENTRO;
                    obj.ID_ANIO = SelectedSolicitud.ATENCION_INGRESO.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedSolicitud.ATENCION_INGRESO.ID_IMPUTADO;
                    obj.ID_INGRESO = SelectedSolicitud.ATENCION_INGRESO.ID_INGRESO;
                    obj.REGISTRO_FEC = SelectedSolicitud.ATENCION_INGRESO.REGISTRO_FEC;
                    obj.ID_CENTRO_UBI = SelectedSolicitud.ATENCION_INGRESO.ID_CENTRO_UBI;
                    #endregion
                    switch (estatus)
                    {
                        case (short)enumSolicitudCita.AGENDADA:
                            obj.ESTATUS = (short)enumSolicitudCita.AGENDADA;
                            break;
                        case (short)enumSolicitudCita.DESCARTADA:
                            obj.ESTATUS = (short)enumSolicitudCita.DESCARTADA;
                            break;
                        case (short)enumSolicitudCita.CANCELADA:
                            obj.ESTATUS = (short)enumSolicitudCita.CANCELADA;
                            break;
                    }
                    if (new cAtencionIngreso().Actualizar(obj))
                        return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al actualizar estatus de la solicitud", ex);
            }
            return false;
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                permisos = new cProcesoUsuario().Obtener(enumProcesos.CITA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Count() > 0)
                    MenuLimpiarEnabled = true;
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = true;
                        if (p.EDITAR == 1)
                            PEditar = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = true;
                        if (p.IMPRIMIR == 1)
                            PImprimir = true;
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
