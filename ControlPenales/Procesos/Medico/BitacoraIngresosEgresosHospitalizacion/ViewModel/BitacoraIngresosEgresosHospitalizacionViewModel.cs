using ControlPenales.BiometricoServiceReference;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Medico;
using SSP.Controlador.Catalogo.Justicia.Medico.CertificadoMedico;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;

namespace ControlPenales
{
    public partial class BitacoraIngresosEgresosHospitalizacionViewModel : ValidationViewModelBase
    {
        public BitacoraIngresosEgresosHospitalizacionViewModel()
        {

        }

        public BitacoraIngresosEgresosHospitalizacionViewModel(enumProcesos _proceso_origen, DateTime _fecha_calendario, enumResultadoOperacion _resultado_operacion)
        {
            proceso_origen = _proceso_origen;
            fecha_proceso_origen = _fecha_calendario;
            resultado_operacion = _resultado_operacion;
        }

        public async void OnLoad(BitacoraIngresosEgresosHospitalizacionView Window)
        {
            #region AutoCompletes
            AutoCompleteTB = Window.AutoCompleteTB;
            AutoCompleteLB = AutoCompleteTB.Template.FindName("PART_ListBox", Window.AutoCompleteTB) as ListBox;
            AutoCompleteTB.PreviewMouseDown += new MouseButtonEventHandler(listBox_MouseUp);
            AutoCompleteTB.KeyDown += listBox_KeyDown;
            AutoCompleteReceta = Window.AutoCompleteReceta;
            AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", Window.AutoCompleteReceta) as ListBox;
            AutoCompleteReceta.PreviewMouseDown += new MouseButtonEventHandler(MouseUpReceta);
            AutoCompleteReceta.KeyDown += KeyDownReceta;
            #endregion
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                ObtenerTiposHospitalizaciones();
                SelectedTipoHospitalizacionValue = SELECCIONE;
                RaisePropertyChanged("SelectedTipoHospitalizacionValue");
                ObtenerCamasDisponibles();
                selectedCamaHospitalValue = SELECCIONE;
                RaisePropertyChanged("SelectedCamaHospitalValue");
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ParametroEstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    IngresarEnabled = true;
                    GuardarMenuEnabled = false;
                    IngresarMenuEnabled = true;
                    EgresarMenuEnabled = false;
                    BuscarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    SalirMenuEnabled = true;
                    Ventana = Window;
                    
                    
                    var usuario_persona = new cUsuario().ObtenerUsuario(GlobalVar.gUsr).EMPLEADO.PERSONA;
                    UsuarioAreaMedica = string.Format("{1} {2} {0}",
                        !string.IsNullOrEmpty(usuario_persona.NOMBRE) ? usuario_persona.NOMBRE.TrimEnd() : string.Empty,
                        !string.IsNullOrEmpty(usuario_persona.PATERNO) ? usuario_persona.PATERNO.TrimEnd() : string.Empty,
                        !string.IsNullOrEmpty(usuario_persona.MATERNO) ? usuario_persona.MATERNO.TrimEnd() : string.Empty);
                    ObtenerCantidadesCamas();
                    var _fecha_server = Fechas.GetFechaDateServer;
                    FechaMinimaHospitalizacion = _fecha_server.AddDays(-1);
                    FechaMaximaHospitalizacion = _fecha_server;
                    if (fecha_proceso_origen.HasValue)
                        SelectedFechaHospitalizacion=fecha_proceso_origen.Value;
                    else
                        SelectedFechaHospitalizacion = FechaServer = Fechas.GetFechaDateServer;
                    TituloGroupBoxHospitalizaciones = string.Format("HOSPITALIZACIONES DEL DIA: {0}/{1}/{2}",
                        FechaServer.Day,
                        FechaServer.Month,
                        FechaServer.Year);

                    FotoIngresoNotaMedica = new Imagenes().getImagenPerson();

                }));
            });

            if (proceso_origen.HasValue)
            {
                switch (proceso_origen)
                {
                    case enumProcesos.CAPTURADEFUNCION:
                        if (resultado_operacion.HasValue && resultado_operacion.Value==enumResultadoOperacion.EXITO)
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La tarjerta informativa de deceso fue capturada con exito");
                        break;
                }
            }
            else
            {
                StaticSourcesViewModel.SourceChanged = false;
            }
            ConfiguraPermisos();
        }

        public async void ClickSwitch(object obj)
        {
            #region Agenda
            if (obj is Appointment)
            {
                try
                {
                    #region APPOINTMENT
                    var appoint = (Appointment)obj;
                    if (procedimientoMedicoParaAgenda == null)
                    {
                        AgregarHora = !AgregarHora;
                        if (AgregarHora)
                        {
                            AHoraI = appoint.StartTime;
                            AHoraF = appoint.EndTime;
                            ValidacionHoras();
                            var horaini = AHoraI;
                            var horafin = AHoraF;
                            LimpiarAgenda();
                            AHoraI = horaini;
                            AHoraF = horafin;
                            AFecha = new DateTime(appoint.StartTime.Year, appoint.StartTime.Month, appoint.StartTime.Day);
                        }
                        else
                            base.ClearRules();
                    }
                    else
                    {
                        if (SelectedEmpleadoValue.ID_EMPLEADO > 0)
                        {
                            if (BuscarAgenda)
                            {
                                AgregarHora = !AgregarHora;
                                if (AgregarHora)
                                {
                                    AHoraI = appoint.StartTime;
                                    AHoraF = appoint.EndTime;
                                    ValidacionHoras();
                                    var horaini = AHoraI;
                                    var horafin = AHoraF;
                                    LimpiarAgenda();
                                    AHoraI = horaini;
                                    AHoraF = horafin;
                                    AFecha = new DateTime(appoint.StartTime.Year, appoint.StartTime.Month, appoint.StartTime.Day);
                                }
                                else
                                    base.ClearRules();
                                GuardarAgendaEnabled = true;
                                if (SelectedIngreso != null ?
                                    SelectedIngreso.ATENCION_CITA.Any(a => a.CITA_FECHA_HORA.Value.Year == AHoraI.Value.Year && a.CITA_FECHA_HORA.Value.Month == AHoraI.Value.Month && a.CITA_FECHA_HORA.Value.Day == AHoraI.Value.Day &&
                                        a.CITA_FECHA_HORA.Value.Hour == AHoraI.Value.Hour && a.CITA_FECHA_HORA.Value.Minute == AHoraI.Value.Minute)
                                : false)
                                {
                                    var cita = SelectedIngreso.ATENCION_CITA.First(a => a.CITA_FECHA_HORA.Value.Year == AHoraI.Value.Year && a.CITA_FECHA_HORA.Value.Month == AHoraI.Value.Month && a.CITA_FECHA_HORA.Value.Day == AHoraI.Value.Day &&
                                        a.CITA_FECHA_HORA.Value.Hour == AHoraI.Value.Hour && a.CITA_FECHA_HORA.Value.Minute == AHoraI.Value.Minute);
                                    GuardarAgendaEnabled = cita != null ? !(cita.PROC_ATENCION_MEDICA_PROG.Any(w => w.ID_PROCMED == SelectProcMedEnCitaParaAgendarAux.ID_PROCMED)) : true;
                                }
                                else
                                    GuardarAgendaEnabled = ListProcMedsSeleccionados != null ? !ListProcMedsSeleccionados.Where(an => an.ID_PROC_MED == SelectProcMedSeleccionado.ID_PROC_MED).Any(an =>
                                        an.CITAS.Any(a => a.FECHA_INICIAL.Year == AHoraI.Value.Year && a.FECHA_INICIAL.Month == AHoraI.Value.Month && a.FECHA_INICIAL.Day == AHoraI.Value.Day && a.FECHA_INICIAL.Hour == AHoraI.Value.Hour &&
                                        a.FECHA_INICIAL.Minute == AHoraI.Value.Minute)) : true;
                                if (ListProcMedsSeleccionados != null)
                                    ProcedimientosMedicosEnCitaEnMemoria = new ObservableCollection<CustomCitasProcedimientosMedicos>(ListProcMedsSeleccionados.SelectMany(s => s.CITAS)
                                        .Where(w => w.FECHA_INICIAL.Year == AHoraI.Value.Year && w.FECHA_INICIAL.Month == AHoraI.Value.Month && w.FECHA_INICIAL.Day == AHoraI.Value.Day && w.FECHA_INICIAL.Hour == AHoraI.Value.Hour &&
                                            w.FECHA_INICIAL.Minute == AHoraI.Value.Minute));
                            }
                            else
                            {
                                AgendaView.Hide();
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes cargar la agenda del responsable seleccionado.");
                                AgendaView.Show();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                return;
                            }
                        }
                        else
                        {
                            AgendaView.Hide();
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes seleccionar al personal responsable del procedimiento medico.");
                            AgendaView.Show();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            return;
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar los datos de la agenda.", ex);
                }
            }
            if (obj is CustomProcedimientosMedicosSeleccionados)
            {
                try
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                        CargarEmpleadosProcedimientosMedicos(true);
                    });
                    #region PROCEDIMIENTOS MEDICOS
                    procedimientoMedicoParaAgenda = (CustomProcedimientosMedicosSeleccionados)obj;
                    if (LstAgenda == null)
                        LstAgenda = new ObservableCollection<Appointment>();
                    EmpleadosEnAgendaEnabled = true;
                    ProcedimientoMedicoPorAgendar = procedimientoMedicoParaAgenda.PROC_MED_DESCR;
                    AgregarProcedimientoMedicoLayoutVisible = Visibility.Visible;
                    SelectedEmpleadoValue = LstEmpleados.First(f => f.ID_EMPLEADO == -1);
                    AgendaView = new AgendarCitaConCalendarioView();
                    SelectedDateCalendar = fHoy;
                    AgendaView.Loaded += AgendaLoaded;
                    AgendaView.Agenda.AddAppointment += AgendaAddAppointment;
                    AgendaView.btn_guardar.Click += AgendaClick;
                    AgendaView.Closing += AgendaClosing;
                    AgendaView.Owner = PopUpsViewModels.MainWindow;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    AgendaView.ShowDialog();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    #endregion
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de la agenda.", ex);
                }
                return;
            }
            #endregion


            switch (obj.ToString())
            {
                case "menu_ingresar":
                    try
                    {
                        var permisos = new cProcesoUsuario().Obtener(enumProcesos.BITACORA_HOSPITALIZACION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);

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
                        IndexTab = 0;
                        IngresoHospitalizacionEnabled = CancelarMenuEnabled = true;
                        IngresarMenuEnabled = false;
                        tipo_seleccionado = TIPO_GUARDAR_HOSPITALIZACION.INGRESO;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            ObtenerCamasDisponibles();
                            selectedCamaHospitalValue = SELECCIONE;
                            RaisePropertyChanged("SelectedCamaHospitalValue");
                        });
                        setValidacionIngreso();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    catch (Exception ex)
                    {

                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener los permisos.", ex);
                    }
                    break;
                //case "ConfirmarHospitalizacion":

                //    break;
                case "menu_cancelar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea cancelar la operación?") != 1)
                            return;
                    }
                    IngresoHospitalizacionEnabled = CancelarMenuEnabled = false;
                    IngresarMenuEnabled = true;
                    SelectedNotaMedica = null;
                    GuardarMenuEnabled = false;
                    //NotaMedicaSelected = false;
                    AnioHospitalizacion = FolioHospitalizacion = null;
                    NombreHospitalizacion = ApellidoPaternoHospitalizacion = ApellidoMaternoHospitalizacion = string.Empty;
                    SelectedCamaHospitalValue = SELECCIONE;
                    SelectedTipoHospitalizacionValue = SELECCIONE;
                    HospitalizacionFecha = null;
                    IndexTab = 0;
                    IsNotaEgresoEnabled = false;
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "NotasMedicasHospitalizacion":
                    if (SelectedTipoHospitalizacionValue != SELECCIONE)
                    {

                        ListaNotasMedicas = new List<NOTA_MEDICA>();
                        var lista_notas_medicas = new List<NOTA_MEDICA>();

                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                var atenciones_medicas = new List<ATENCION_MEDICA>();
                                listaNotasMedicas = new cAtencionMedica().
                                    ObtenerAtencionesMedicaNotasHospitalizacion(
                                    FechaMinimaHospitalizacion.Value,
                                    FechaMaximaHospitalizacion.Value,
                                    (short)selectedTipoHospitalizacionValue,
                                    AnioHospitalizacion,
                                    FolioHospitalizacion,
                                    !string.IsNullOrEmpty(NombreHospitalizacion) ? NombreHospitalizacion.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(ApellidoPaternoHospitalizacion) ? ApellidoPaternoHospitalizacion.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(ApellidoMaternoHospitalizacion) ? ApellidoMaternoHospitalizacion.Trim() : string.Empty)
                                    .Select(s=>s.NOTA_MEDICA)
                                    .ToList();
                                RaisePropertyChanged("ListaNotasMedicas");
                                //if (selectedTipoHospitalizacionValue == (int)enumHospitalizacionIngresoTipo.ESPECIALIDAD)
                                //{
                                //    atenciones_medicas = new cAtencionMedica().
                                //    ObtenerAtencionesMedicaNotasHospitalizacion(
                                //    FechaMinimaHospitalizacion.Value,
                                //    FechaMaximaHospitalizacion.Value,
                                //    AnioHospitalizacion.HasValue ? AnioHospitalizacion.Value : 0,
                                //    FolioHospitalizacion.HasValue ? FolioHospitalizacion.Value : 0,
                                //    !string.IsNullOrEmpty(NombreHospitalizacion) ? NombreHospitalizacion : string.Empty,
                                //    !string.IsNullOrEmpty(ApellidoPaternoHospitalizacion) ? ApellidoPaternoHospitalizacion : string.Empty,
                                //    !string.IsNullOrEmpty(ApellidoMaternoHospitalizacion) ? ApellidoMaternoHospitalizacion : string.Empty)
                                //    .ToList();
                                //}
                                //else if (selectedTipoHospitalizacionValue == (int)enumHospitalizacionIngresoTipo.EXTERNA)
                                //{
                                //    atenciones_medicas = new cAtencionMedica().
                                //        ObtenerAtencionesMedicaNotasHospitalizacion(
                                //        FechaMinimaHospitalizacion.Value,
                                //        FechaMaximaHospitalizacion.Value,
                                //        AnioHospitalizacion.HasValue ? AnioHospitalizacion.Value : 0,
                                //        FolioHospitalizacion.HasValue ? FolioHospitalizacion.Value : 0,
                                //        !string.IsNullOrEmpty(NombreHospitalizacion) ? NombreHospitalizacion : string.Empty,
                                //        !string.IsNullOrEmpty(ApellidoPaternoHospitalizacion) ? ApellidoPaternoHospitalizacion : string.Empty,
                                //        !string.IsNullOrEmpty(ApellidoMaternoHospitalizacion) ? ApellidoMaternoHospitalizacion : string.Empty).
                                //        ToList();
                                //}
                                //else
                                //{
                                //    atenciones_medicas = new cAtencionMedica().
                                //    ObtenerAtencionesMedicaNotasHospitalizacion(
                                //    FechaMinimaHospitalizacion.Value,
                                //    FechaMaximaHospitalizacion.Value,
                                //    AnioHospitalizacion.HasValue ? AnioHospitalizacion.Value : 0,
                                //    FolioHospitalizacion.HasValue ? FolioHospitalizacion.Value : 0,
                                //    !string.IsNullOrEmpty(NombreHospitalizacion) ? NombreHospitalizacion : string.Empty,
                                //    !string.IsNullOrEmpty(ApellidoPaternoHospitalizacion) ? ApellidoPaternoHospitalizacion : string.Empty,
                                //    !string.IsNullOrEmpty(ApellidoMaternoHospitalizacion) ? ApellidoMaternoHospitalizacion : string.Empty).
                                //    ToList();
                                //}


                                //foreach (var atencion_medica in atenciones_medicas)
                                //{
                                //    if (!ListaHospitalizaciones.Any(a =>
                                //        a.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_ANIO == atencion_medica.ID_ANIO &&
                                //        a.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_CENTRO == atencion_medica.ID_CENTRO &&
                                //        a.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_IMPUTADO == atencion_medica.ID_IMPUTADO &&
                                //        a.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_INGRESO == atencion_medica.ID_INGRESO))
                                //    {
                                //        atencion_medica.INGRESO.IMPUTADO.NOMBRE = !string.IsNullOrEmpty(atencion_medica.INGRESO.IMPUTADO.NOMBRE) ? atencion_medica.INGRESO.IMPUTADO.NOMBRE.TrimEnd() : string.Empty;
                                //        atencion_medica.INGRESO.IMPUTADO.PATERNO = !string.IsNullOrEmpty(atencion_medica.INGRESO.IMPUTADO.PATERNO) ? atencion_medica.INGRESO.IMPUTADO.PATERNO.TrimEnd() : string.Empty;
                                //        atencion_medica.INGRESO.IMPUTADO.MATERNO = !string.IsNullOrEmpty(atencion_medica.INGRESO.IMPUTADO.MATERNO) ? atencion_medica.INGRESO.IMPUTADO.MATERNO.TrimEnd() : string.Empty;
                                //        atencion_medica.INGRESO.IMPUTADO.SEXO = !string.IsNullOrEmpty(atencion_medica.INGRESO.IMPUTADO.SEXO) ? atencion_medica.INGRESO.IMPUTADO.SEXO.TrimEnd() : string.Empty;
                                //        lista_notas_medicas.Add(atencion_medica.NOTA_MEDICA);

                                //    }
                                //}
                                //Application.Current.Dispatcher.Invoke((Action)(delegate
                                //{
                                //    ListaNotasMedicas = lista_notas_medicas;
                                //}));
                            }
                            catch (Exception ex)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo cargar la información de las notas médicas.", ex);
                                }));
                            }
                        });
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.NOTAS_MEDICAS_HOSPITALIZACION);
                    }
                    break;
                case "SeleccionarNotaMedicaHospitalizacion":
                    if (SelectedNotaMedica != null)
                    {
                        AnioHospitalizacion = SelectedNotaMedica.ATENCION_MEDICA.ID_ANIO;
                        FolioHospitalizacion = SelectedNotaMedica.ATENCION_MEDICA.ID_IMPUTADO;
                        NombreHospitalizacion = SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE;
                        ApellidoPaternoHospitalizacion = SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO;
                        ApellidoMaternoHospitalizacion = SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.NOTAS_MEDICAS_HOSPITALIZACION);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                        GuardarMenuEnabled = true;
                        var foto_seguimiento = SelectedNotaMedica.ATENCION_MEDICA.INGRESO.INGRESO_BIOMETRICO.FirstOrDefault(f => f.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO);
                        FotoIngresoNotaMedica = foto_seguimiento != null ? (foto_seguimiento.BIOMETRICO) : new Imagenes().getImagenPerson();

                    }
                    else
                    {
                        var metro = Application.Current.Windows[0] as MetroWindow;
                        var metroSettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Cerrar",
                            AnimateShow = true,
                            AnimateHide = false

                        };
                        await metro.ShowMessageAsync(
                            "Validación",
                            "Debe seleccionar una nota médica.",
                            MessageDialogStyle.Affirmative,
                            metroSettings);
                    }

                    break;
                case "CancelarNotasMedicasHospitalizacion":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.NOTAS_MEDICAS_HOSPITALIZACION);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    SelectedNotaMedica = null;
                    ListaNotasMedicas = new List<NOTA_MEDICA>();
                    break;
                case "menu_guardar":
                    if (HasErrors)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", String.Format("Falta capturar los campos obligatorios. \n\n {0}", Error));
                        return;
                    }
                    if (tipo_seleccionado==TIPO_GUARDAR_HOSPITALIZACION.INGRESO)
                    {
                        await GuardarIngresoHospitalizacion();
                        CargarHospitalizaciones();
                    }
                    else if (tipo_seleccionado==TIPO_GUARDAR_HOSPITALIZACION.ALTA)
                    {
                       
                        if (ListRecetas != null ?
                            ListRecetas.Any() ?
                                ListRecetas.Any(a => (a.CANTIDAD.HasValue ? a.CANTIDAD.Value <= 0 : true) ||
                                                     (a.DURACION.HasValue ? a.DURACION.Value <= 0 : true) ||
                                                     (!a.HORA_MANANA && !a.HORA_TARDE && !a.HORA_NOCHE) ||
                                                     (a.PRESENTACION.HasValue ?
                                                        a.PRESENTACION.Value <= 0 ?
                                                            a.PRODUCTO != null ?
                                                                a.PRODUCTO.PRODUCTO_PRESENTACION.Count > 0
                                                            : false
                                                        : false
                                                     : a.PRODUCTO != null ?
                                                        a.PRODUCTO.PRODUCTO_PRESENTACION.Count > 0
                                                     : false))
                            : false
                        : false)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos de la receta médica.");
                            return;
                        }
                          if (ListProcMedsSeleccionados!=null)
                              foreach (var item in ListProcMedsSeleccionados)
                              { 
                                  if (item.CITAS==null || item.CITAS.Count()==0)
                                  {
                                      new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar de agendar los procedimientos medicos programados.");
                                      return;
                                  }
                              }



                        GuardarAlta();
                    }
                    break;
                case "menu_salir":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "alta_hospitalizacion":
                    if (SelectedHospitalizacion==null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No hay un registro de hospitalizacion seleccionado");
                        return;
                    }
                    if (!_permisos_agregar)
                    {
                        new Dialogos().ConfirmacionDialogo("Permisos", "No tiene permisos para dar de alta un imputado");
                        return;
                    }
                    if (SelectedFechaHospitalizacion.Date != FechaServer.Date)
                    {
                        new Dialogos().ConfirmacionDialogo("Permisos", "Favor de dar de alta el imputado en la fecha actual");
                        return;
                    }
                    if (SelectedHospitalizacion.ALTA_FEC.HasValue)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ya fue dado de alta el imputado");
                        return;
                    }
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea seleccionar otra hospitalizacion para generar una alta?") != 1)
                            return;
                    }
                    IndexTab = 1;
                    LimpiarAlta();
                    tipo_seleccionado = TIPO_GUARDAR_HOSPITALIZACION.ALTA;
                    
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        
                        CargarDatosEgreso(true);
                        CargarDietasTratamiento(true);
                        ObtenerMetodosEgresoMedico(true);
                        CargarProcedimientosSubTipos();
                        CargarProcedimientos(-1);
                        selectedProcMedSubTipoValue = -1;
                        RaisePropertyChanged("SelectedProcMedSubTipoValue");
                        selectedProcedimientoMedicoValue = -1;
                        RaisePropertyChanged("SelectedProcedimientoMedicoValue");
                        selectedMotivoEgresoMedicoValue = -1;
                        RaisePropertyChanged("SelectedMotivoEgresoMedicoValue");
                        IsNotaEgresoVisible = Visibility.Collapsed;
                        IsExcarcelacionVisible = Visibility.Collapsed;
                        isLiberacionVisible = Visibility.Collapsed;
                    });
                    GuardarMenuEnabled = true;
                    CancelarMenuEnabled = true;
                    IngresarMenuEnabled = false;
                    IsNotaEgresoEnabled = true;
                    setValidacionAlta();
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "agregar_procedimiento_medico":
                    
                    if (SelectedProcedimientoMedicoValue==-1)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccionar un procedimiento medico para agregar");
                        return;
                    }
                    if (_ListProcMedsSeleccionados == null)
                        _ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>();
                    if (!_ListProcMedsSeleccionados.Contains(_ListProcMedsSeleccionados.FirstOrDefault(w => w.ID_PROC_MED == SelectedProcedimientoMedicoValue)))
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                            var _proc_med = lstProcedimientoMedico.FirstOrDefault(w => w.ID_PROCMED == SelectedProcedimientoMedicoValue);
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                _ListProcMedsSeleccionados.Add(new CustomProcedimientosMedicosSeleccionados
                                {
                                    ID_PROC_MED = _proc_med.ID_PROCMED,
                                    PROC_MED_DESCR = _proc_med.DESCR,
                                    OBSERV = string.Empty,
                                    CITAS = new List<CustomCitasProcedimientosMedicos>()
                                });
                                _ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>(_ListProcMedsSeleccionados);
                                RaisePropertyChanged("ListProcMedsSeleccionados");
                            }));
                        });
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No se pueden repetir procedimientos medicos en la lista");
                        return;
                    }
                    break;
                case "remove_procedimiento_medico":
                    if (SelectProcMedSeleccionado == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccionar un procedimiento medico para eliminar");
                        return;
                    }
                    if (ListProcMedsSeleccionados == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No se han agregado procedimientos medicos");
                        return;
                    }
                    if (ListProcMedsSeleccionados.Contains(SelectProcMedSeleccionado))
                    {
                        ListProcMedsSeleccionados.Remove(SelectProcMedSeleccionado);
                        ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>(ListProcMedsSeleccionados);
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No se encuentra el procedimientos medico en la lista");
                        return;
                    }
                    break;
                case "ver_agenda":
                    if (SelectedEmpleadoValue != null ? SelectedEmpleadoValue.ID_EMPLEADO <= 0 : true)
                    {
                        if (AgendaView != null)
                            AgendaView.Hide();
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes seleccionar un responsable.");
                        if (AgendaView != null)
                            AgendaView.Show();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        return;
                    }
                    if (AgendaView != null)
                    {
                        AgendaView.Hide();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        var _usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().Obtener(StaticSourcesViewModel.UsuarioLogin.Username);
                        TituloAgenda = "AGENDA DE: " + SelectedEmpleadoValue.NOMBRE_COMPLETO + "\tAGENDA - " +
                            SelectedIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectedIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? "" : SelectedIngreso.IMPUTADO.MATERNO.Trim());
                        ObtenerAgenda(SelectedEmpleadoValue.Usuario.ID_USUARIO, true);
                        isAgendarCitaEnabled = true;
                        RaisePropertyChanged("IsAgendarCitaEnabled");
                        selectedDateCalendar = selectedDateBusqueda;
                        RaisePropertyChanged("SelectedDateCalendar");
                    });
                    if (AgendaView != null)
                    {
                        AgendaView.Show();
                        BuscarAgenda = true;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    break;
                case "borrar_procedimiento_medico_agenda":
                    try
                    {
                        if (SelectProcedimientoMedicoEnCitaEnMemoria == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento médico para poder eliminarlo de la hora seleccionada.");
                            return;
                        }
                        if (SelectProcedimientoMedicoEnCitaEnMemoria.ID_PROC_MED <= 0)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento médico válido para poder eliminarlo de la hora seleccionada.");
                            return;
                        }
                        if (ProcedimientosMedicosEnCitaEnMemoria.Any(a => a.ID_PROC_MED == SelectProcedimientoMedicoEnCitaEnMemoria.ID_PROC_MED))
                        {
                            if (ListProcMedsSeleccionados.Any(w => w.ID_PROC_MED == SelectProcedimientoMedicoEnCitaEnMemoria.ID_PROC_MED ?
                                    w.CITAS.Any(a => a.FECHA_INICIAL.Year == AHoraI.Value.Year && a.FECHA_INICIAL.Month == AHoraI.Value.Month && a.FECHA_INICIAL.Day == AHoraI.Value.Day && a.FECHA_INICIAL.Hour == AHoraI.Value.Hour &&
                                        a.FECHA_INICIAL.Minute == AHoraI.Value.Minute)
                                : false))
                            {
                                foreach (var item in ListProcMedsSeleccionados)
                                {
                                    if (item.CITAS.Any(a => a.ID_PROC_MED == SelectProcedimientoMedicoEnCitaEnMemoria.ID_PROC_MED ?
                                        a.FECHA_INICIAL.Year == AHoraI.Value.Year && a.FECHA_INICIAL.Month == AHoraI.Value.Month && a.FECHA_INICIAL.Day == AHoraI.Value.Day && a.FECHA_INICIAL.Hour == AHoraI.Value.Hour &&
                                        a.FECHA_INICIAL.Minute == AHoraI.Value.Minute
                                    : false))
                                    {
                                        item.CITAS.Remove(item.CITAS.First(a => a.FECHA_INICIAL.Year == AHoraI.Value.Year && a.FECHA_INICIAL.Month == AHoraI.Value.Month && a.FECHA_INICIAL.Day == AHoraI.Value.Day &&
                                            a.FECHA_INICIAL.Hour == AHoraI.Value.Hour && a.FECHA_INICIAL.Minute == AHoraI.Value.Minute));
                                        item.AGENDA = string.Empty;
                                        foreach (var itm in item.CITAS)
                                        {
                                            item.AGENDA = item.AGENDA + itm.FECHA_INICIAL.ToString("dd \\de MMMM, yyyy a la\\s HH:mm") + "\n";
                                        }
                                        if (AgendaView != null)
                                        {
                                            AgendaView.Hide();
                                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                        }
                                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                                        {
                                            try
                                            {
                                                var _usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().Obtener(StaticSourcesViewModel.UsuarioLogin.Username);
                                                TituloAgenda = "AGENDA DE: " + SelectedEmpleadoValue.NOMBRE_COMPLETO + "\tAGENDA - " +
                                                    SelectedIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectedIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? "" : SelectedIngreso.IMPUTADO.MATERNO.Trim());
                                                ObtenerAgenda(SelectedEmpleadoValue.Usuario.ID_USUARIO, true);
                                                isAgendarCitaEnabled = true;
                                                RaisePropertyChanged("IsAgendarCitaEnabled");
                                                selectedDateCalendar = selectedDateBusqueda;
                                                RaisePropertyChanged("SelectedDateCalendar");
                                            }
                                            catch (Exception ex)
                                            {
                                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al de nuevo la agenda.", ex);
                                            }
                                        });
                                        if (AgendaView != null)
                                        {
                                            AgendaView.Show();
                                            BuscarAgenda = true;
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                        }
                                        break;
                                    }
                                }
                            }
                            var procmed = SelectProcMedSeleccionado.ID_PROC_MED;
                            ProcedimientosMedicosEnCitaEnMemoria.Remove(ProcedimientosMedicosEnCitaEnMemoria.First(a => a.ID_PROC_MED == SelectProcedimientoMedicoEnCitaEnMemoria.ID_PROC_MED));
                            ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>(ListProcMedsSeleccionados.OrderBy(o => o.PROC_MED_DESCR));
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar el procedimiento médico de la hora seleccionada.", ex);
                    }
                    break;
                case "agendar_cita":
                    try
                    {
                        LstAgenda = LstAgenda ?? new ObservableCollection<Appointment>();
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            var useer = new cUsuario().ObtenerTodos(GlobalVar.gUsr);
                            if (useer.Any() ? !useer.FirstOrDefault().ID_PERSONA.HasValue : false) return;
                            var usr = useer.FirstOrDefault();
                            CargarEmpleadosProcedimientosMedicos(true);
                        });
                        SelectedEmpleadoValue = LstEmpleados.First(f => f.Usuario != null ? string.IsNullOrEmpty(f.Usuario.ID_USUARIO) ? false : f.Usuario.ID_USUARIO.Trim() == GlobalVar.gUsr : false);
                        EmpleadosEnAgendaEnabled = false;
                        procedimientoMedicoParaAgenda = null;
                        AgregarProcedimientoMedicoLayoutVisible = Visibility.Collapsed;
                        AgendaView = new AgendarCitaConCalendarioView();
                        SelectedDateCalendar = fHoy;
                        AgendaView.Loaded += AgendaLoaded;
                        AgendaView.Agenda.AddAppointment += AgendaAddAppointment;
                        AgendaView.btn_guardar.Click += AgendaClick;
                        AgendaView.Closing += AgendaClosing;
                        AgendaView.Owner = PopUpsViewModels.MainWindow;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        AgendaView.ShowDialog();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar nuevos datos a la agenda.", ex);
                    }
                    break;
                case "menu_reporte":
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Generando datos del reporte", GeneraReporteDatos))
                        ReportViewer_Requisicion();
                    break;
                case "eliminar_receta":
                    if (SelectReceta != null ? SelectReceta.PRODUCTO == null : true) return;
                    if (ListRecetas.Any(a => a.PRODUCTO.ID_PRODUCTO == SelectReceta.PRODUCTO.ID_PRODUCTO))
                        ListRecetas.Remove(ListRecetas.First(a => a.PRODUCTO.ID_PRODUCTO == SelectReceta.PRODUCTO.ID_PRODUCTO));
                    break;
                case "borrar_enfermedad":
                    if (ListEnfermedades == null) return;
                    if (SelectEnfermedad == null) return;
                    ListEnfermedades.Remove(ListEnfermedades.First(f => f.ID_ENFERMEDAD == SelectEnfermedad.ID_ENFERMEDAD));
                    break;
            }
        }

        private bool GeneraReporteDatos()
        {
            try
            {
                ds_detalle = new cHospitalizacion().ObtenerHospitalizacionesPorFecha(SelectedFechaHospitalizacion,GlobalVar.gCentro).Select(s=>new EXT_REPORTE_BITACORA_HOSPITALIZACION_DETALLE{
                    FECHA_ALTA=s.ALTA_FEC,
                    FECHA_INGRESO = s.INGRESO_FEC,
                    FechaNac = s.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA.HasValue?s.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value:(DateTime?) null,
                    ID_ANIO=s.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_ANIO,
                    ID_IMPUTADO= s.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_IMPUTADO,
                    HOSPITALIZACION_TIPO=s.ID_INGHOSTIP,
                    MATERNO=s.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO,
                    MATERNO_MEDICO_ALTA=s.EMPLEADO1.PERSONA.MATERNO,
                    MATERNO_MEDICO_INGRESO=s.EMPLEADO.PERSONA.MATERNO,
                    NOCAMA=s.CAMA_HOSPITAL.DESCR,
                    NOMBRE=s.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE,
                    NOMBRE_MEDICO_ALTA=s.EMPLEADO1.PERSONA.NOMBRE,
                    NOMBRE_MEDICO_INGRESO=s.EMPLEADO.PERSONA.NOMBRE,
                    PATERNO=s.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO,
                    PATERNO_MEDICO_ALTA=s.EMPLEADO1.PERSONA.PATERNO,
                    PATERNO_MEDICO_INGRESO=s.EMPLEADO.PERSONA.PATERNO
                }).ToList();

                var logo1 = Parametro.REPORTE_LOGO2;
                var logo2 = Parametro.LOGO_ESTADO;
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault().DESCR;
                ds_encabezado = new List<EXT_REPORTE_BITACORA_HOSPITALIZACION_ENCABEZADO>() {
                    new EXT_REPORTE_BITACORA_HOSPITALIZACION_ENCABEZADO{
                        LOGO1=logo1,
                        LOGO2=logo2,
                        CERESO=centro,
                        CAMAS_DESOCUPADAS=camasDesocupadas,
                        CAMAS_OCUPADAS=camasOcupadas
                    }
                };
                return true;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void ReportViewer_Requisicion()
        {
            try
            {
                var _reporte = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                _reporte.Closed += (s, e) =>
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                };
                _reporte.Owner = PopUpsViewModels.MainWindow;
                _reporte.Show();
                _reporte.Report.LocalReport.ReportPath = "Reportes/rBitacoraIngresosEgresosHospitalizacion.rdlc";
                _reporte.Report.LocalReport.DataSources.Clear();
                Microsoft.Reporting.WinForms.ReportDataSource rsd1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rsd1.Name = "DS_DETALLES";
                rsd1.Value = ds_detalle;
                Microsoft.Reporting.WinForms.ReportDataSource rsd2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rsd2.Name = "DS_ENCABEZADO";
                rsd2.Value = ds_encabezado;
                _reporte.Report.LocalReport.DataSources.Add(rsd1);
                _reporte.Report.LocalReport.DataSources.Add(rsd2);
                _reporte.Report.RefreshReport();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el reporte", ex);
            }

        }

        private async Task  GuardarIngresoHospitalizacion()
        {
            try
            {

                if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando alta de la hospitalizacion", () =>
                {
                    var usuario = new cUsuario().ObtenerUsuario(GlobalVar.gUsr).EMPLEADO.PERSONA;
                    new cHospitalizacion().InsertarNuevaHospitalizacion(new HOSPITALIZACION()
                    {
                        ID_HOSPITA = new cProceso().GetIDProceso<int>("HOSPITALIZACION", "ID_HOSPITA", string.Format("{0}={1}", "ID_CENTRO", GlobalVar.gCentro)),
                        ID_INGHOSTIP = selectedTipoHospitalizacionValue,
                        ID_CAMA_HOSPITAL = selectedCamaHospitalValue,
                        ID_CENTRO = SelectedNotaMedica.ID_CENTRO_UBI,
                        ID_ATENCION_MEDICA = SelectedNotaMedica.ID_ATENCION_MEDICA,
                        ID_CENTRO_UBI = SelectedNotaMedica.ID_CENTRO_UBI,
                        INGRESO_MEDICO = usuario.ID_PERSONA,
                        ID_HOSEST = (short)enumHospitalizacionEstatus.HOSPITALIZADO,
                        INGRESO_FEC = HospitalizacionFecha.Value,
                        REGISTRO_FEC = Fechas.GetFechaDateServer,
                        ID_USUARIO = GlobalVar.gUsr,

                    },
                    new CAMA_HOSPITAL()
                    {
                        ID_CAMA_HOSPITAL = selectedCamaHospitalValue,
                        ID_CENTRO = GlobalVar.gCentro,
                        ESTATUS = OCUPADA
                    });
                    return true;
                }))
                {
                    new Dialogos().ConfirmacionDialogo("EXITO!", "La alta de hospitalizacion fue guardada con exito");
                    IngresarMenuEnabled = true;
                    /*NotaMedicaSelected*/
                    GuardarMenuEnabled = CancelarMenuEnabled = IngresoHospitalizacionEnabled = false;
                    selectedTipoHospitalizacionValue = SELECCIONE;
                    RaisePropertyChanged("SelectedTipoHospitalizacionValue");
                    HospitalizacionFecha = null;
                    AnioHospitalizacion = FolioHospitalizacion = null;
                    UsuarioAreaMedica = NombreHospitalizacion = ApellidoPaternoHospitalizacion = ApellidoMaternoHospitalizacion = string.Empty;
                    SelectedNotaMedica = null;
                    ListaNotasMedicas = new List<NOTA_MEDICA>();
                    SelectedCamaHospitalValue = SELECCIONE;
                    StaticSourcesViewModel.SourceChanged = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al dar de alta la hospitalización.", ex);
            }
        }

        private async void  GuardarAlta()
        {
            try
            {

                if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando alta de la hospitalizacion",() => {
                    var _fecha_servidor = Fechas.GetFechaDateServer;
                    var _usuario = new cUsuario().ObtenerUsuario(GlobalVar.gUsr);
                    var _diagnostico = new List<NOTA_MEDICA_ENFERMEDAD>();
                    if (ListEnfermedades!=null)
                        foreach(var item in ListEnfermedades)
                        {
                            _diagnostico.Add(new NOTA_MEDICA_ENFERMEDAD {
                                ID_CENTRO_UBI=selectedHospitalizacion.ID_CENTRO_UBI,
                                ID_ENFERMEDAD=item.ID_ENFERMEDAD
                            });
                        }
                    var _receta_detalle = new List<RECETA_MEDICA_DETALLE>();
                    var _receta = new List <RECETA_MEDICA>();
                    if (ListRecetas!=null && ListRecetas.Count()>0)
                    {
                        foreach (var item in ListRecetas)
                        {
                            _receta_detalle.Add(new RECETA_MEDICA_DETALLE
                            {
                                CENA = item.HORA_NOCHE ? (short)1 : (short)0,
                                COMIDA = item.HORA_TARDE ? (short)1 : (short)0,
                                DESAYUNO = item.HORA_MANANA ? (short)1 : (short)0,
                                DOSIS = item.CANTIDAD,
                                DURACION = item.DURACION,
                                ID_CENTRO_UBI = selectedHospitalizacion.ID_CENTRO_UBI,
                                ID_PRESENTACION_MEDICAMENTO = item.PRESENTACION,
                                ID_PRODUCTO = item.PRODUCTO.ID_PRODUCTO,
                                OBSERV = item.OBSERVACIONES
                            });
                        }
                        _receta.Add(new RECETA_MEDICA() {
                            ESTATUS="S",
                            ID_CENTRO_UBI=selectedHospitalizacion.ID_CENTRO_UBI,
                            ID_USUARIO=GlobalVar.gUsr,
                            RECETA_FEC=_fecha_servidor,
                            RECETA_MEDICA_DETALLE=_receta_detalle
                        });
                    }
                    var _dietas = new List<NOTA_MEDICA_DIETA>();
                    if (LstDietasTratamiento!=null)
                        foreach(var item in LstDietasTratamiento.Where(w=>w.ELEGIDO))
                        {
                            _dietas.Add(new NOTA_MEDICA_DIETA {
                                DIETA_FEC=_fecha_servidor,
                                ESTATUS="S",
                                ID_CENTRO_UBI=selectedHospitalizacion.ID_CENTRO_UBI,
                                ID_DIETA=item.DIETA.ID_DIETA
                            });
                    }
                    #region procedimiento
                    var atenciones_cita = new ATENCION_CITA();
                    var proc_atencion_medica = new List<PROC_ATENCION_MEDICA>();
                    var proc_atencion_medica_prog = new List<PROC_ATENCION_MEDICA_PROG>();
                    
                    if (ListProcMedsSeleccionados!=null)
                    foreach (var item in ListProcMedsSeleccionados)
                    {
                        proc_atencion_medica_prog = new List<PROC_ATENCION_MEDICA_PROG>();
                            foreach(var item_cita in item.CITAS)
                            {
                                atenciones_cita = new ATENCION_CITA
                                {
                                    CITA_FECHA_HORA = item_cita.FECHA_INICIAL,
                                    CITA_HORA_TERMINA = item_cita.FECHA_FINAL,
                                    ESTATUS = "N",
                                    ID_ANIO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO,
                                    ID_AREA = (short)enumAreas.MEDICA_PB,
                                    ID_CENTRO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_CENTRO,
                                    ID_CENTRO_UBI = selectedHospitalizacion.ID_CENTRO_UBI,
                                    ID_IMPUTADO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO,
                                    ID_INGRESO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO,
                                    ID_RESPONSABLE = item_cita.ENFERMERO,
                                    ID_TIPO_ATENCION = (short)enumAtencionTipo.CONSULTA_MEDICA,
                                    ID_TIPO_SERVICIO = (short)enumAtencionServicio.NOTA_EGRESO,
                                    ID_USUARIO = GlobalVar.gUsr,
                                };
                                proc_atencion_medica_prog.Add(new PROC_ATENCION_MEDICA_PROG {
                                    ATENCION_CITA=atenciones_cita,
                                    ESTATUS="S",
                                    ID_CENTRO_UBI=selectedHospitalizacion.ID_CENTRO_UBI,
                                    ID_PROCMED=item_cita.ID_PROC_MED,
                                    ID_USUARIO_ASIGNADO=new cUsuario().Obtener(item_cita.ENFERMERO).First().ID_USUARIO
                                });
                            }
                            proc_atencion_medica.Add(new PROC_ATENCION_MEDICA {
                                ID_CENTRO_UBI=selectedHospitalizacion.ID_CENTRO_UBI,
                                ID_PROCMED=item.ID_PROC_MED,
                                ID_USUARIO=GlobalVar.gUsr,
                                OBSERV=item.OBSERV,
                                PROC_ATENCION_MEDICA_PROG=proc_atencion_medica_prog,
                                REGISTRO_FEC=_fecha_servidor
                            });
                        }
                    #endregion
                    var _cita_seguimiento = new ATENCION_CITA();
                    if (CheckedSeguimiento)
                    {
                        _cita_seguimiento = new ATENCION_CITA
                        {
                            CITA_FECHA_HORA = AtencionCitaSeguimiento.CITA_FECHA_HORA,
                            CITA_HORA_TERMINA = AtencionCitaSeguimiento.CITA_HORA_TERMINA,
                            ESTATUS = "N",
                            ID_ANIO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO,
                            ID_AREA = (short)enumAreas.MEDICA_PB,
                            ID_CENTRO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_CENTRO,
                            ID_CENTRO_UBI = selectedHospitalizacion.ID_CENTRO_UBI,
                            ID_IMPUTADO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO,
                            ID_INGRESO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO,
                            ID_RESPONSABLE = AtencionCitaSeguimiento.ID_RESPONSABLE,
                            ID_TIPO_ATENCION = (short)enumAtencionTipo.CONSULTA_MEDICA,
                            ID_TIPO_SERVICIO = (short)enumAtencionServicio.CITA_MEDICA,
                            ID_USUARIO = GlobalVar.gUsr
                        };
                    }
                    var _atencion_medica = new ATENCION_MEDICA
                    {
                        ATENCION_FEC = FechaEgresoHospitalizacion,
                        ID_ANIO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO,
                        ID_CENTRO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_CENTRO,
                        ID_CENTRO_UBI = selectedHospitalizacion.ID_CENTRO_UBI,
                        ID_HOSPITA = selectedHospitalizacion.ID_HOSPITA,
                        ID_IMPUTADO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO,
                        ID_INGRESO = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO,
                        ID_TIPO_ATENCION = (short)enumAtencionTipo.CONSULTA_MEDICA,
                        ID_TIPO_SERVICIO = (short)enumAtencionServicio.NOTA_EGRESO,
                        ATENCION_CITA1 = CheckedSeguimiento? _cita_seguimiento:null,
                        PROC_ATENCION_MEDICA = proc_atencion_medica
                    };
                    var _nota_medica = new NOTA_MEDICA {
                        ID_CENTRO_UBI=selectedHospitalizacion.ID_CENTRO_UBI,
                        ID_PRONOSTICO=null,
                        ID_RESPONSABLE=_usuario.ID_PERSONA,
                        OCUPA_HOSPITALIZACION = "N",
                        OCUPA_INTERCONSULTA=solicitaInterconsultaCheck?"S":"N",
                        NOTA_MEDICA_ENFERMEDAD = _diagnostico,
                        RECETA_MEDICA=_receta,
                        NOTA_MEDICA_DIETA=_dietas,
                        ATENCION_MEDICA=_atencion_medica
                    };
                    
                    var _nota_egreso=new NOTA_EGRESO{
                        DIAS_ESTANCIA=DiasHospitalizado,
                        FECHA_REGISTRO=_fecha_servidor,
                        ID_ANIO=selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO,
                        ID_CENTRO=selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_CENTRO,
                        ID_CENTRO_UBI=selectedHospitalizacion.ID_CENTRO_UBI,
                        ID_HOSPITA=selectedHospitalizacion.ID_HOSPITA,
                        ID_IMPUTADO=selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO,
                        ID_INGRESO=selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO,
                        USUARIO_REGISTRO=GlobalVar.gUsr,
                        ID_MOEGMED=selectedMotivoEgresoMedicoValue,
                        ID_LIBERACION=selectedMotivoEgresoMedicoValue==(short)enumMotivoEgreso.LIBERACION?selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.LIBERACION.OrderByDescending(o=>o.LIBERACION_FEC).First().ID_LIBERACION:(short?)null,
                        RESUMEN_EVOLUCION=textEvolucionEstadoActualEgresoMedico,
                        ID_CONSEC=selectedMotivoEgresoMedicoValue==(short)enumMotivoEgreso.EXCARCELACION?SelectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AU" || w.ID_ESTATUS == "PR" || w.ID_ESTATUS == "AC").ID_CONSEC:(int?)null,
                        NOTA_MEDICA=_nota_medica                        
                    };
                    new cNotaEgreso().Insertar(_nota_egreso, fechaEgresoHospitalizacion.Value, _usuario.ID_PERSONA.Value);
                    return true;
                }))
                {
                    new Dialogos().ConfirmacionDialogo("EXITO!", "La alta de hospitalizacion fue guardada con exito");
                    GuardarMenuEnabled = false;
                    CargarHospitalizaciones();
                    IsFechaEgresoValida = false;
                    IsNotaEgresoVisible = Visibility.Collapsed;
                    SelectedMotivoEgresoMedicoValue = -1;
                    LimpiarAlta();
                    IndexTab = 0;
                    IsNotaEgresoEnabled = false;
                    StaticSourcesViewModel.SourceChanged = false;
                }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al dar de alta la hospitalización.", ex);
            }
        }

        private void CargarDatosEgreso(bool isExceptionManaged=false)
        {
            try
            {
                textAnioImputadoEgreso = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.ID_ANIO.ToString();
                RaisePropertyChanged("TextAnioImputadoEgreso");
                textFolioImputadoEgreso = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.ID_IMPUTADO.ToString();
                RaisePropertyChanged("TextFolioImputadoEgreso");
                textMaternoImputadoEgreso = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO;
                RaisePropertyChanged("TextMaternoImputadoEgreso");
                textPaternoImputadoEgreso = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO;
                RaisePropertyChanged("TextPaternoImputadoEgreso");
                textNombreImputadoEgreso = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE;
                RaisePropertyChanged("TextNombreImputadoEgreso");
                textSexoImputadoEgreso = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO";
                RaisePropertyChanged("TextSexoImputadoEgreso");
                textEdadImputadoEgreso = new Fechas().CalculaEdad(selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString();
                RaisePropertyChanged("TextEdadImputadoEgreso");
                imagenIngresoEgreso = selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                    selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.INGRESO_BIOMETRICO.First(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO :
                    selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                    selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                    new Imagenes().getImagenPerson();
                RaisePropertyChanged("ImagenIngresoEgreso");
                textCamaHospitalizacionEgreso = selectedHospitalizacion.CAMA_HOSPITAL.DESCR;
                RaisePropertyChanged("TextCamaHospitalizacionEgreso");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la informacion para el egreso.", ex);
            }
        }

        public async void CargarHospitalizacionesPorFecha(BitacoraIngresosEgresosHospitalizacionView obj)
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarHospitalizaciones(true); });
        }

        public void CargarHospitalizaciones(bool isExceptionManaged=false)
        {
            try
            {
                listaHospitalizaciones = new cHospitalizacion().ObtenerHospitalizacionesPorFecha(SelectedFechaHospitalizacion,GlobalVar.gCentro).ToList();
                RaisePropertyChanged("ListaHospitalizaciones");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener las hospitalizaciones por fecha.", ex);
            }
        }

        public void ObtenerTiposHospitalizaciones()
        {
            try
            {
                listaTipoHospitalizaciones = new List<HOSPITALIZACION_INGRESO_TIPO>(new cHospitalizacionIngresoTipo().ObtenerTipos().ToList());
                listaTipoHospitalizaciones.Insert(0,new HOSPITALIZACION_INGRESO_TIPO()
                 {
                     DESCR = "SELECCIONE",
                     ID_INGHOSTIP = SELECCIONE
                 });
                RaisePropertyChanged("ListaTipoHospitalizaciones");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo obtener los tipos de hospitalizaciones para el ingreso.", ex);
            }
        }

        public void ObtenerCantidadesCamas()
        {
            try
            {
                CamasDesocupadas = string.Format("DISPONIBLES: {0}", new cCamaHospital().ObtenerCamasHospitalEstatus(ACTIVA, GlobalVar.gCentro).Count());
                CamasOcupadas = string.Format("OCUPADAS: {0}", new cCamaHospital().ObtenerCamasHospitalEstatus(OCUPADA, GlobalVar.gCentro).Count());
            }
            catch (Exception ex)
            {

                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo obtener la disponibilidad de camas.", ex);
            }
        }

        public void ObtenerCamasDisponibles()
        {
            try
            {
                listaCamasHospitalizacion = new List<CAMA_HOSPITAL>();
                listaCamasHospitalizacion.Add(new CAMA_HOSPITAL()
                {
                    DESCR = "SELECCIONE",
                    ID_CAMA_HOSPITAL = SELECCIONE
                });
                listaCamasHospitalizacion.AddRange(new cCamaHospital().ObtenerCamasHospitalEstatus(ACTIVA, GlobalVar.gCentro).ToList());
                RaisePropertyChanged("ListaCamasHospitalizacion");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo obtener la lista de camas disponibles.", ex);
            }
        }

        #region Catalogos
        public void ObtenerMetodosEgresoMedico(bool isExceptionManaged=false)
        {
            try
            {
                lstMotivoEgresoMedico = new ObservableCollection<MOTIVO_EGRESO_MEDICO>(new cMotivoEgresoMedico().ObtenerMotivosEgresosMedicos("S"));
                lstMotivoEgresoMedico.Add(new MOTIVO_EGRESO_MEDICO {
                    DESCR="SELECCIONAR",
                    ID_MOEGMED=-1
                });
                RaisePropertyChanged("LstMotivoEgresoMedico");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo obtener el catalogo de metodos de egreso.", ex);
            }
        }

        public void CargarDietasTratamiento(bool isExceptionManaged=false)
        {
            try
            {
                lstDietasTratamiento = new ObservableCollection<DietaMedica>(new cDietas().ObtenerTodosActivos().OrderBy(o => o.DESCR).Select(s => new DietaMedica { DIETA = s, ELEGIDO = false }));
                RaisePropertyChanged("LstDietasTratamiento");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo obtener el catalogo de metodos de egreso.", ex);
            }
        }

        public void CargarProcedimientosSubTipos(bool isExceptionManaged=false)
        {
            try
            {
                lstProcMedSubTipo= new ObservableCollection<PROC_MED_SUBTIPO>(new cProcedimientosSubtipo().ObtenerTodosActivos());
                lstProcMedSubTipo.Add(new PROC_MED_SUBTIPO {
                    ID_PROCMED_SUBTIPO=-1,
                    DESCR="SELECCIONE"
                });
                RaisePropertyChanged("LstProcMedSubTipo");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo obtener el catalogo de metodos de egreso.", ex);
            }
        }

        public void CargarProcedimientos(short id_proc_sub_tipo,bool isExceptionManaged=false)
        {
            try
            {
                lstProcedimientoMedico = new ObservableCollection<PROC_MED>(new cProcedimientosMedicos().ObtenerXSubtipo(id_proc_sub_tipo));
                lstProcedimientoMedico.Add(new PROC_MED {
                    DESCR="SELECCIONE",
                    ID_PROCMED=-1
                });
                RaisePropertyChanged("LstProcedimientoMedico");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo obtener el catalogo de metodos de egreso.", ex);
            }
        }

        public void CargarEmpleadosProcedimientosMedicos(bool isExceptionManaged=false)
        {
            try
            {
                lstEmpleados = new List<cUsuarioExtendida>(new cEmpleado().ObtenerTodosEmpleados(GlobalVar.gCentro, "S", (short)enumRolesAreasTecnicas.ENFERMERO)
                    .Select(s => new cUsuarioExtendida {
                        ID_EMPLEADO = s.ID_EMPLEADO,
                        NOMBRE_COMPLETO = s.PERSONA.NOMBRE.Trim() + (s.PERSONA.PATERNO == null || s.PERSONA.PATERNO.Trim() == "" ? "" : " " + s.PERSONA.PATERNO.Trim()) +
                           (s.PERSONA.MATERNO == null || s.PERSONA.MATERNO.Trim() == "" ? "" : " " + s.PERSONA.MATERNO.Trim()),
                        Usuario = s.USUARIO.FirstOrDefault(f => f.ID_PERSONA == s.ID_EMPLEADO)
                    }));
                var _usuario = new cUsuario().ObtenerUsuario(GlobalVar.gUsr);
                if (_usuario!=null)
                {
                    if (!lstEmpleados.Any(w=>w.ID_EMPLEADO==_usuario.EMPLEADO.ID_EMPLEADO))
                        lstEmpleados.Add(new cUsuarioExtendida
                        {
                            ID_EMPLEADO = _usuario.EMPLEADO.ID_EMPLEADO,
                            NOMBRE_COMPLETO = _usuario.EMPLEADO.PERSONA.NOMBRE.Trim() + (_usuario.EMPLEADO.PERSONA.PATERNO == null || _usuario.EMPLEADO.PERSONA.PATERNO.Trim() == "" ? "" : " " + _usuario.EMPLEADO.PERSONA.PATERNO.Trim()) +
                               (_usuario.EMPLEADO.PERSONA.MATERNO == null || _usuario.EMPLEADO.PERSONA.MATERNO.Trim() == "" ? "" : " " + _usuario.EMPLEADO.PERSONA.MATERNO.Trim()),
                            Usuario=_usuario
                        });
                }
                lstEmpleados.Insert(0, new cUsuarioExtendida { ID_EMPLEADO = -1, NOMBRE_COMPLETO = "SELECCIONE" });
                RaisePropertyChanged("LstEmpleados");
            }
            catch(Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "No se pudo obtener el catalogo de metodos de egreso.", ex);
            }
        }
        #endregion

        #region General
        private void LimpiarAlta()
        {
            FechaEgresoHospitalizacion = null;
            TextEvolucionEstadoActualEgresoMedico = string.Empty;
            TextAnioImputadoEgreso = string.Empty;
            TextFolioImputadoEgreso = string.Empty;
            TextPaternoImputadoEgreso = string.Empty;
            TextMaternoImputadoEgreso = string.Empty;
            TextNombreImputadoEgreso = string.Empty;
            TextSexoImputadoEgreso = string.Empty;
            TextEdadImputadoEgreso = string.Empty;
            TextCamaHospitalizacionEgreso = string.Empty;
            SelectedMotivoEgresoMedicoValue = SELECCIONE;
            LstTratamientoMedicoHistorial = new List<TratamientoHistorial>();
            LstDietas = new List<NOTA_MEDICA_DIETA>();
            LstProcedimientos = new List<PROC_ATENCION_MEDICA_PROG>();
            ListEnfermedades = new ObservableCollection<ENFERMEDAD>();
            SolicitaInterconsultaCheck = false;
            CheckedSeguimiento = false;
            StrFechaSeguimiento = string.Empty;
            AtencionCitaSeguimiento = null;
            ListRecetas = new ObservableCollection<RecetaMedica>();
            LstDietasTratamiento = new ObservableCollection<DietaMedica>();
            SelectedProcMedSubTipoValue = SELECCIONE;
            SelectedProcedimientoMedicoValue = SELECCIONE;
            ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>();
        }

        private async void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_proc_subtipo":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                        CargarProcedimientos(selectedProcMedSubTipoValue);
                        selectedProcedimientoMedicoValue = -1;
                        RaisePropertyChanged("SelectedProcedimientoMedicoValue");
                    });
                    break;
                case "cambio_motivo":
                    switch ((short)SelectedMotivoEgresoMedicoValue)
                    {
                        case (short)enumMotivoEgreso.DEFUNCION:
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "¿Esta seguro de registrar un deceso?") != 1)
                            {
                                IsNotaEgresoVisible = Visibility.Collapsed;
                                return;
                            }

                            if (SelectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.NOTA_DEFUNCION != null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El imputado ya cuenta con una tarjeta informativa de deceso");
                                return;
                            }
                            var metro = Application.Current.Windows[0] as MetroWindow;
                            GC.Collect();
                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new CapturaDefuncionView();
                            ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = 
                                new CapturaDefuncionViewModel(selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO,enumProcesos.BITACORA_HOSPITALIZACION);
                            break;
                        case (short)enumMotivoEgreso.EXCARCELACION:
                            if (!SelectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.EXCARCELACION.Any(w => w.ID_ESTATUS == "AU" || w.ID_ESTATUS == "PR" || w.ID_ESTATUS == "AC"))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El imputado no cuenta con ninguna excarcelacion valida para realizar la alta de hospitalizacion");
                                IsNotaEgresoVisible = Visibility.Collapsed;
                                return;
                            }
                            var id_hospital_otros = (short)Parametro.ID_HOSPITAL_OTROS;
                            var _excarcelacion=SelectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AU" || w.ID_ESTATUS == "PR" || w.ID_ESTATUS == "AC");
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                                fechaExcarcelacionEgresoHospitalizacion = _excarcelacion.PROGRAMADO_FEC.Value.ToString(@"dd/MM/yyyy HH:mm");
                                RaisePropertyChanged("FechaExcarcelacionEgresoHospitalizacion");
                                StringBuilder _strbuilder = new StringBuilder();
                                foreach(var item in _excarcelacion.EXCARCELACION_DESTINO)
                                {
                                    if (_strbuilder.Length > 0)
                                        _strbuilder.Append(", ");
                                    _strbuilder.Append(item.EXCARCELACION.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? item.JUZGADO.DESCR : item.INTERCONSULTA_SOLICITUD != null ? item.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any() ? item.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ? 
                                            item.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL==id_hospital_otros?item.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL_OTRO: !string.IsNullOrEmpty(item.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR) ? item.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty);
                                }
                                destinoExcarcelacionEgresoHospitalizacion = _strbuilder.ToString();
                                RaisePropertyChanged("DestinoExcarcelacionEgresoHospitalizacion");
                            });
                            
                            IsExcarcelacionVisible = Visibility.Visible;
                            IsLiberacionVisible = Visibility.Collapsed;
                            break;
                        case (short) enumMotivoEgreso.LIBERACION:
                            if (selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAUSA_PENAL.Count==0 || selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAUSA_PENAL.Any(w=>w.ID_ESTATUS_CP==0 || w.ID_ESTATUS_CP==1))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El imputado no cuenta con ninguna liberación valida para realizar la alta de hospitalizacion");
                                IsNotaEgresoVisible = Visibility.Collapsed;
                                return;
                            }
                            IsLiberacionVisible = Visibility.Visible;
                            IsExcarcelacionVisible = Visibility.Collapsed;
                            FolioLiberacion=selectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.LIBERACION.OrderByDescending(o=>o.LIBERACION_FEC).First().LIBERACION_OFICIO;
                            break;
                    }
                    if (SelectedMotivoEgresoMedicoValue!=(short)enumMotivoEgreso.DEFUNCION)
                    {
                        IsNotaEgresoVisible = Visibility.Visible;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                            fechaIngresoHospitalizacion = selectedHospitalizacion.INGRESO_FEC.Value;
                            RaisePropertyChanged("FechaIngresoHospitalizacion");
                            StringBuilder _strbuilder = new StringBuilder();
                            foreach (var item in selectedHospitalizacion.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD)
                            {
                                if (_strbuilder.Length > 0)
                                    _strbuilder.Append(", ");
                                _strbuilder.Append(item.ENFERMEDAD.NOMBRE.Trim());
                            }
                            diagnosticoIngresoHospitalizacion = _strbuilder.ToString();
                            RaisePropertyChanged("DiagnosticoIngresoHospitalizacion");
                            var _tratamiento_historial = new cRecetaMedicaDetalle().Obtener_Historial_Hospitalizacion(selectedHospitalizacion.ID_HOSPITA, selectedHospitalizacion.ID_CENTRO_UBI);
                            if (_tratamiento_historial != null)
                                lstTratamientoMedicoHistorial = _tratamiento_historial
                                .Select(s => new TratamientoHistorial
                                {
                                    CANTIDAD = s.DOSIS,
                                    DURACION = s.DURACION,
                                    FORMULA_MEDICA = s.PRODUCTO.FORMA_FARMACEUTICA.DESCR.Trim(),
                                    MANANA = s.DESAYUNO.HasValue && s.DESAYUNO.Value == 1 ? true : false,
                                    TARDE = s.COMIDA.HasValue && s.COMIDA.Value == 1 ? true : false,
                                    NOCHE = s.CENA.HasValue && s.CENA.Value == 1 ? true : false,
                                    OBSERVACIONES = s.OBSERV,
                                    PRESENTACION = s.PRESENTACION_MEDICAMENTO.DESCRIPCION.Trim(),
                                    UNIDAD_MEDIDA = s.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.DESCR.Trim(),
                                    MEDICAMENTO = s.PRODUCTO.NOMBRE
                                }).OrderBy(o => o.MEDICAMENTO).ToList();
                            else
                                lstTratamientoMedicoHistorial = new List<TratamientoHistorial>();
                            RaisePropertyChanged("LstTratamientoMedicoHistorial");
                            var _dietas = new cNotaMedicaDieta().Obtener_Historial_Hospitalizacion(selectedHospitalizacion.ID_HOSPITA, selectedHospitalizacion.ID_CENTRO_UBI);
                            if (_dietas != null)
                                lstDietas = _dietas.OrderBy(o=>o.DIETA.DESCR).ToList();
                            else
                                lstDietas = new List<NOTA_MEDICA_DIETA>();
                            RaisePropertyChanged("LstDietas");
                            var _proc_historico=new cProc_Atencion_Medica_Prog().Obtener_Historial_Hospitalizacion(selectedHospitalizacion.ID_HOSPITA, selectedHospitalizacion.ID_CENTRO_UBI);
                            if (_proc_historico != null)
                                lstProcedimientos = _proc_historico.OrderBy(o=>o.PROC_ATENCION_MEDICA.PROC_MED.DESCR).ToList();
                            else
                                lstProcedimientos = new List<PROC_ATENCION_MEDICA_PROG>();
                            RaisePropertyChanged("LstProcedimientos");
                        });
                    }
                    break;
                case "cambio_fecha_egreso":
                    if (fechaEgresoHospitalizacion.HasValue)
                    {
                        IsFechaEgresoValida = true;
                        DiasHospitalizado = (short)(fechaEgresoHospitalizacion.Value - fechaIngresoHospitalizacion).TotalDays;
                        if (DiasHospitalizado == 0)
                            DiasHospitalizado = 1;
                    }
                    else
                    {
                        DiasHospitalizado = null;
                        IsFechaEgresoValida = false;
                    }
                        
                    break;
            }
        }

        #region Autocomplete
        #region ENFERMEDAD

        private void listBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var popup = AutoCompleteTB.Template.FindName("PART_Popup", AutoCompleteTB) as System.Windows.Controls.Primitives.Popup;
                AutoCompleteLB = AutoCompleteTB.Template.FindName("PART_ListBox", AutoCompleteTB) as ListBox;
                var dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is ListBoxItem))
                    dep = VisualTreeHelper.GetParent(dep);
                if (dep == null) return;
                var item = AutoCompleteLB.ItemContainerGenerator.ItemFromContainer(dep);
                if (item == null) return;
                //new ControlPenales.Controls.AutoCompleteTextBox().SetTextValueBySelection(item, false);
                if (item is ENFERMEDAD)
                {
                    ListEnfermedades = ListEnfermedades ?? new ObservableCollection<ENFERMEDAD>();
                    if (!ListEnfermedades.Any(a => a.ID_ENFERMEDAD == ((ENFERMEDAD)item).ID_ENFERMEDAD))
                    {
                        ListEnfermedades.Insert(0, (ENFERMEDAD)item);
                        AutoCompleteTB.Text = string.Empty;
                        
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No se puede repetir una enfermedad");
                        popup.IsOpen = false;
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter || e.Key == Key.Return)
                {
                    var popup = AutoCompleteTB.Template.FindName("PART_Popup", AutoCompleteTB) as System.Windows.Controls.Primitives.Popup;
                    AutoCompleteLB = AutoCompleteTB.Template.FindName("PART_ListBox", AutoCompleteTB) as ListBox;
                    var dep = (DependencyObject)e.OriginalSource;
                    while ((dep != null) && !(dep is ListBoxItem))
                        dep = VisualTreeHelper.GetParent(dep);
                    if (dep == null) return;
                    var item = AutoCompleteLB.ItemContainerGenerator.ItemFromContainer(dep);
                    if (item == null) return;
                    if (item is ENFERMEDAD)
                    {
                        ListEnfermedades = ListEnfermedades ?? new ObservableCollection<ENFERMEDAD>();
                        if (!ListEnfermedades.Any(a => a.ID_ENFERMEDAD == ((ENFERMEDAD)item).ID_ENFERMEDAD))
                        {
                            ListEnfermedades.Insert(0, (ENFERMEDAD)item);
                            AutoCompleteTB.Text = string.Empty;
                            AutoCompleteTB.Focus();
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "No se puede repetir una enfermedad");
                            popup.IsOpen = false;
                            return;
                        }

                    }
                }
                else if (e.Key == Key.Tab) { }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        private void MouseUpReceta(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var popup = AutoCompleteReceta.Template.FindName("PART_Popup", AutoCompleteReceta) as System.Windows.Controls.Primitives.Popup;
                AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", AutoCompleteReceta) as ListBox;
                var dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is ListBoxItem))
                    dep = VisualTreeHelper.GetParent(dep);
                if (dep == null) return;
                var item = AutoCompleteRecetaLB.ItemContainerGenerator.ItemFromContainer(dep);
                if (item == null) return;
                if (item is RecetaMedica)
                {
                    ListRecetas = ListRecetas ?? new ObservableCollection<RecetaMedica>();
                    if (!ListRecetas.Any(a => a.PRODUCTO.ID_PRODUCTO == ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO))
                    {
                        ListRecetas.Insert(0, new RecetaMedica
                        {
                            CANTIDAD = new Nullable<decimal>(),
                            DURACION = new Nullable<short>(),
                            HORA_MANANA = false,
                            HORA_NOCHE = false,
                            HORA_TARDE = false,
                            MEDIDA = ((RecetaMedica)item).MEDIDA,
                            OBSERVACIONES = string.Empty,
                            PRESENTACION = 0,
                            PRODUCTO = ((RecetaMedica)item).PRODUCTO,
                        });
                        AutoCompleteReceta.Text = string.Empty;
                        AutoCompleteReceta.Focus();
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No se puede repetir un medicamento");
                        popup.IsOpen = false;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        private void KeyDownReceta(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter || e.Key == Key.Return)
                {
                    var popup = AutoCompleteReceta.Template.FindName("PART_Popup", AutoCompleteReceta) as System.Windows.Controls.Primitives.Popup;
                    AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", AutoCompleteReceta) as ListBox;
                    var dep = (DependencyObject)e.OriginalSource;
                    while ((dep != null) && !(dep is ListBoxItem))
                        dep = VisualTreeHelper.GetParent(dep);
                    if (dep == null) return;
                    var item = AutoCompleteRecetaLB.ItemContainerGenerator.ItemFromContainer(dep);
                    if (item == null) return;
                    if (item is RecetaMedica)
                    {
                        ListRecetas = ListRecetas ?? new ObservableCollection<RecetaMedica>();
                        if (!ListRecetas.Any(a => a.PRODUCTO.ID_PRODUCTO == ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO))
                        {
                            ListRecetas.Insert(0, (RecetaMedica)item);
                            AutoCompleteReceta.Text = string.Empty;
                            AutoCompleteReceta.Focus();
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "No se puede repetir un medicamento");
                            popup.IsOpen = false;
                            return;
                        }
                    }
                }
                else if (e.Key == Key.Tab) { }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        #endregion
        #endregion
        #endregion

        #region Agenda
        private void AgendaLoaded(object sender, EventArgs e)
        {
            try
            {
                AgendaView.DataContext = this;
                SelectedIngreso = SelectedHospitalizacion.NOTA_MEDICA.ATENCION_MEDICA.INGRESO;
                selectedDateBusqueda = DateTime.Now;
                OnPropertyChanged("SelectedDateBusqueda");
                var _usuario = new cUsuario().Obtener(StaticSourcesViewModel.UsuarioLogin.Username);
                TituloAgenda = "AGENDA DE: " + SelectedEmpleadoValue.NOMBRE_COMPLETO + "\tPACIENTE: " +
                    SelectedIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectedIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? "" : SelectedIngreso.IMPUTADO.MATERNO.Trim());
                if (SelectedEmpleadoValue != null ? SelectedEmpleadoValue.Usuario != null ? !string.IsNullOrEmpty(SelectedEmpleadoValue.Usuario.ID_USUARIO) : false : false)
                    ObtenerAgenda(SelectedEmpleadoValue.Usuario.ID_USUARIO, true);
                else
                    LstAgenda = new ObservableCollection<Appointment>();
                isAgendarCitaEnabled = true;
                RaisePropertyChanged("IsAgendarCitaEnabled");
                selectedDateCalendar = selectedDateBusqueda;
                RaisePropertyChanged("SelectedDateCalendar");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la agenda.", ex);
            }
        }

        private void ObtenerAgenda(string usuario, bool isExceptionManaged = false)
        {
            try
            {
                //if (selectedSolicitud != null)
                //{
                lstAgenda = new ObservableCollection<Appointment>();
                var hoy = Fechas.GetFechaDateServer;
                var agenda = new List<ATENCION_CITA>();
                if (procedimientoMedicoParaAgenda == null)
                {
                    var listaa = new cAtencionCita().ObtenerPorUsuarioDesdeFecha(GlobalVar.gCentro, usuario, hoy, ParametroEstatusInactivos);
                    agenda = listaa != null ? listaa.Any() ? listaa.ToList() : new List<ATENCION_CITA>() : new List<ATENCION_CITA>();
                }
                else if (SelectedEmpleadoValue != null ? SelectedEmpleadoValue.ID_EMPLEADO > 0 : false)
                {
                    var listaa = new cAtencionCita().ObtenerPorUsuarioDesdeFecha(GlobalVar.gCentro, new cUsuario().Obtener(SelectedEmpleadoValue.ID_EMPLEADO).FirstOrDefault().ID_USUARIO, hoy, ParametroEstatusInactivos);
                    agenda = listaa != null ? listaa.Any() ? listaa.ToList() : new List<ATENCION_CITA>() : new List<ATENCION_CITA>();
                }
                else
                    return;
                if (agenda != null)
                {
                    foreach (var a in agenda.Where(w => w.ID_CENTRO_UBI == GlobalVar.gCentro))
                    {
                        lstAgenda.Add(new Appointment()
                        {
                            Subject = string.Format("{0} {1} {2}", a.INGRESO.IMPUTADO.NOMBRE.Trim(), a.INGRESO.IMPUTADO.PATERNO.Trim(), string.IsNullOrEmpty(a.INGRESO.IMPUTADO.MATERNO) ? "" : a.INGRESO.IMPUTADO.MATERNO.Trim()),
                            StartTime = new DateTime(a.CITA_FECHA_HORA.Value.Year, a.CITA_FECHA_HORA.Value.Month, a.CITA_FECHA_HORA.Value.Day, a.CITA_FECHA_HORA.Value.Hour, a.CITA_FECHA_HORA.Value.Minute, a.CITA_FECHA_HORA.Value.Second),
                            EndTime = new DateTime(a.CITA_HORA_TERMINA.Value.Year, a.CITA_HORA_TERMINA.Value.Month, a.CITA_HORA_TERMINA.Value.Day, a.CITA_HORA_TERMINA.Value.Hour, a.CITA_HORA_TERMINA.Value.Minute, a.CITA_HORA_TERMINA.Value.Second),
                            ID_CITA = a.ID_CITA,
                        });
                    }
                }
                if (AtencionCitaSeguimiento != null)
                {
                    AtencionCitaSeguimiento.INGRESO = new cIngreso().Obtener(AtencionCitaSeguimiento.ID_CENTRO.Value, AtencionCitaSeguimiento.ID_ANIO.Value, AtencionCitaSeguimiento.ID_IMPUTADO.Value, AtencionCitaSeguimiento.ID_INGRESO.Value);
                    lstAgenda.Add(new Appointment
                    {
                        Subject = string.Format("{0} {1} {2}", AtencionCitaSeguimiento.INGRESO.IMPUTADO.NOMBRE.Trim(), AtencionCitaSeguimiento.INGRESO.IMPUTADO.PATERNO.Trim(),
                            string.IsNullOrEmpty(AtencionCitaSeguimiento.INGRESO.IMPUTADO.MATERNO) ? "" : AtencionCitaSeguimiento.INGRESO.IMPUTADO.MATERNO.Trim()),
                        StartTime = AtencionCitaSeguimiento.CITA_FECHA_HORA.Value,
                        EndTime = AtencionCitaSeguimiento.CITA_HORA_TERMINA.Value,
                        ID_CITA = AtencionCitaSeguimiento.ID_CITA,
                    });
                }
                if (ListProcMedsSeleccionados != null)
                {
                    foreach (var item in ListProcMedsSeleccionados.SelectMany(s => s.CITAS).Where(w => w.ENFERMERO == SelectedEmpleadoValue.ID_EMPLEADO))
                    {
                        LstAgenda.Add(new Appointment()
                        {
                            Subject = string.Format("{0} {1} {2}", SelectedIngreso.IMPUTADO.NOMBRE.Trim(), SelectedIngreso.IMPUTADO.PATERNO.Trim(), string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? "" : SelectedIngreso.IMPUTADO.MATERNO.Trim()),
                            StartTime = item.FECHA_INICIAL,
                            EndTime = item.FECHA_FINAL,
                        });
                    }
                }
                RaisePropertyChanged("LstAgenda");
                GuardarAgendaEnabled = true;
                //}
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la agenda.", ex);
                else
                    throw ex;
            }
        }

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

        private void LimpiarAgenda()
        {
            AFechaValid = AHorasValid = false;
            AFecha = AHoraI = AHoraF = null;
        }

        private async void AgendaAddAppointment(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is Calendar))
                    return;
                if (procedimientoMedicoParaAgenda == null)
                {
                    AgregarHora = !AgregarHora;
                    if (AgregarHora)
                    {
                        ValidacionHoras();  //antes ValidacionSolicitud
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
                else
                {
                    if (SelectedEmpleadoValue.ID_EMPLEADO > 0)
                    {
                        if (BuscarAgenda)
                        {
                            if (SelectProcMedSeleccionado != null ?
                                SelectProcMedSeleccionado.CITAS.Any(a => a.FECHA_INICIAL.Year == AHoraI.Value.Year && a.FECHA_INICIAL.Month == AHoraI.Value.Month && a.FECHA_INICIAL.Day == AHoraI.Value.Day &&
                                    a.FECHA_INICIAL.Hour == AHoraI.Value.Hour && a.FECHA_INICIAL.Minute == AHoraI.Value.Minute)
                            : false)
                            {
                                AgendaView.Hide();
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", "Ya tienes agendado este procedimiento a la misma hora.");
                                AgendaView.Show();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                return;
                            }
                            GuardarAgendaEnabled = true;
                            AgregarHora = !AgregarHora;
                            if (AgregarHora)
                            {
                                ValidacionHoras();
                                var horaini = AHoraI;
                                var horafin = AHoraF;
                                LimpiarAgenda();
                                AHoraI = horaini;
                                AHoraF = horafin;
                                AFecha = new DateTime(((Calendar)sender).CurrentDate.Year, ((Calendar)sender).CurrentDate.Month, ((Calendar)sender).CurrentDate.Day);
                            }
                            else
                                base.ClearRules();
                            if (ListProcMedsSeleccionados != null)
                            {
                                ProcedimientosMedicosEnCitaEnMemoria = new ObservableCollection<CustomCitasProcedimientosMedicos>(ListProcMedsSeleccionados.SelectMany(s => s.CITAS)
                                    .Where(w => w.FECHA_INICIAL.Year == AHoraI.Value.Year && w.FECHA_INICIAL.Month == AHoraI.Value.Month && w.FECHA_INICIAL.Day == AHoraI.Value.Day && w.FECHA_INICIAL.Hour == AHoraI.Value.Hour &&
                                        w.FECHA_INICIAL.Minute == AHoraI.Value.Minute));
                            }
                        }
                        else
                        {
                            AgendaView.Hide();
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes cargar la agenda del responsable seleccionado.");
                            AgendaView.Show();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            return;
                        }
                    }
                    else
                    {
                        AgendaView.Hide();
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes seleccionar al personal responsable del procedimiento médico.");
                        AgendaView.Show();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        return;
                    }
                }
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
                AgendaView.Loaded -= AgendaLoaded;
                AgendaView.Agenda.AddAppointment -= AgendaAddAppointment;
                AgendaView.btn_guardar.Click -= AgendaClick;
                AgendaView.Closing -= AgendaClosing;
                AgendaView = null;
                AgregarHora = false;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la agenda.", ex);
            }
        }

        private async void AgendaClick(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!AFecha.HasValue)
                {
                    MensajeError = "Favor de validar el campo de la fecha.";
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        Task.Factory.StartNew(async () => { await TaskEx.Delay(4000); MensajeError = string.Empty; });
                    }));
                    return;
                }
                if (!AHoraI.HasValue)
                {
                    MensajeError = "Favor de validar el campo de la hora inicial.";
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        Task.Factory.StartNew(async () => { await TaskEx.Delay(4000); MensajeError = string.Empty; });
                    }));
                    return;
                }
                if (!AHoraF.HasValue)
                {
                    MensajeError = "Favor de validar el campo de la hora final.";
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        Task.Factory.StartNew(async () => { await TaskEx.Delay(4000); MensajeError = string.Empty; });
                    }));
                    return;
                }
                var hoy = Fechas.GetFechaDateServer;
                var _nueva_fecha_inicio = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraI.Value.Hour, AHoraI.Value.Minute, AHoraI.Value.Second);
                if (hoy > _nueva_fecha_inicio)
                {
                    MensajeError = "La fecha y hora deben de ser mayor a la actual";
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        Task.Factory.StartNew(async () => { await TaskEx.Delay(4000); MensajeError = string.Empty; });
                    }));
                    return;
                }
                var _nueva_fecha_final = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraF.Value.Hour, AHoraF.Value.Minute, AHoraF.Value.Second);
                if (_nueva_fecha_inicio > _nueva_fecha_final)
                {
                    MensajeError = "La fecha inicial no puede ser mayor a la fecha final";
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        Task.Factory.StartNew(async () => { await TaskEx.Delay(4000); MensajeError = string.Empty; });
                    }));
                    return;
                }

                if (AHoraI.Value.Minute % 15 != 0 || AHoraF.Value.Minute % 15 != 0)
                {
                    MensajeError = "Los intervalos de atención tienen que ser en bloques de 15 minutos";
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        Task.Factory.StartNew(async () => { await TaskEx.Delay(4000); MensajeError = string.Empty; });
                    }));
                    return;
                }
                if (AFecha != null)
                {
                    if (procedimientoMedicoParaAgenda == null)
                    {
                        if (AtencionCitaSeguimiento != null)
                        {
                            LstAgenda.Remove(LstAgenda.FirstOrDefault(w => w.StartTime == AtencionCitaSeguimiento.CITA_FECHA_HORA && w.EndTime == AtencionCitaSeguimiento.CITA_HORA_TERMINA));
                        }
                        LstAgenda = LstAgenda ?? new ObservableCollection<Appointment>();
                        LstAgenda.Add(new Appointment()
                        {
                            Subject = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.NOMBRE) ? SelectedIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.PATERNO) ? SelectedIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? SelectedIngreso.IMPUTADO.MATERNO.Trim() : string.Empty),
                            StartTime = _nueva_fecha_inicio,
                            EndTime = _nueva_fecha_final,
                        });
                        var user = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault();
                        AtencionCitaSeguimiento = new ATENCION_CITA
                        {
                            CITA_FECHA_HORA = _nueva_fecha_inicio,
                            CITA_HORA_TERMINA = _nueva_fecha_final,
                            ESTATUS = ParametroEstatusCitaSinAtender,
                            ID_ANIO = SelectedIngreso.ID_ANIO,
                            ID_CENTRO = SelectedIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectedIngreso.ID_INGRESO,
                            ID_TIPO_ATENCION = (short)enumAtencionTipo.CONSULTA_MEDICA,
                            ID_TIPO_SERVICIO = (short)enumAtencionServicio.CITA_MEDICA,
                            ID_RESPONSABLE = user.ID_PERSONA,
                            ID_USUARIO = GlobalVar.gUsr,
                            ID_AREA = (short)enumAreas.MEDICA_PA
                        };
                        AgendaView.Agenda.FilterAppointments();
                        AgregarHora = !AgregarHora;
                        StrFechaSeguimiento = "Fecha de la próxima cita: " + AtencionCitaSeguimiento.CITA_FECHA_HORA.Value.ToString("dd \\de MMMM, yyyy a la\\s HH:mm");
                        AgendaView.Close();
                    }
                    else
                    {
                        if (SelectedEmpleadoValue != null ? SelectedEmpleadoValue.ID_EMPLEADO <= 0 : true)
                        {
                            AgendaView.Hide();
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Favor de seleccionar empleado.");
                            AgendaView.Show();
                            AgregarHora = false;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            return;
                        }
                        if (string.IsNullOrEmpty(procedimientoMedicoParaAgenda.PROC_MED_DESCR))
                        {
                            if (ListProcMedsSeleccionados == null)
                                ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>();
                            if (ListProcMedsSeleccionados.Any(a => a.ID_PROC_MED == SelectProcMedEnCitaParaAgendar.ID_PROCMED))
                            {
                                foreach (var item in ListProcMedsSeleccionados.Where(a => a.ID_PROC_MED == SelectProcMedEnCitaParaAgendar.ID_PROCMED))
                                {
                                    item.CITAS.Add(new CustomCitasProcedimientosMedicos
                                    {
                                        FECHA_INICIAL = AHoraI.Value,
                                        FECHA_FINAL = AHoraF.Value,
                                        ENFERMERO = SelectedEmpleadoValue.ID_EMPLEADO,
                                        ID_PROC_MED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                        PROC_MED_DESCR = SelectProcMedEnCitaParaAgendar.DESCR,
                                    });
                                }
                            }
                            else
                            {
                                ListProcMedsSeleccionados.Add(new CustomProcedimientosMedicosSeleccionados()
                                {
                                    AGENDA = string.Empty,
                                    PROC_MED_DESCR = SelectProcMedEnCitaParaAgendarAux.DESCR,
                                    OBSERV = string.Empty,
                                    ID_PROC_MED = SelectProcMedEnCitaParaAgendarAux.ID_PROCMED,
                                    CITAS = new List<CustomCitasProcedimientosMedicos>() 
                                    {
                                        new CustomCitasProcedimientosMedicos
                                        {
                                    FECHA_INICIAL = AHoraI.Value,
                                    FECHA_FINAL = AHoraF.Value,
                                            ENFERMERO = SelectedEmpleadoValue.ID_EMPLEADO,
                                            ID_PROC_MED = SelectProcMedEnCitaParaAgendarAux.ID_PROCMED,
                                            PROC_MED_DESCR = SelectProcMedEnCitaParaAgendarAux.DESCR,
                                        } 
                                    },
                                });
                            }
                        }
                        else
                        {
                            foreach (var item in ListProcMedsSeleccionados.Where(f => f.ID_PROC_MED == procedimientoMedicoParaAgenda.ID_PROC_MED))
                            {
                                item.AGENDA = item.AGENDA + AHoraI.Value.ToString("dd \\de MMMM, yyyy a la\\s HH:mm") + "\n";
                                item.CITAS = item.CITAS ?? new List<CustomCitasProcedimientosMedicos>();
                                item.CITAS.Add(new CustomCitasProcedimientosMedicos
                                {
                                    FECHA_INICIAL = AHoraI.Value,
                                    FECHA_FINAL = AHoraF.Value,
                                    ENFERMERO = SelectedEmpleadoValue.ID_EMPLEADO,
                                    ID_PROC_MED = SelectProcMedSeleccionado.ID_PROC_MED,
                                    PROC_MED_DESCR = SelectProcMedSeleccionado.PROC_MED_DESCR,
                                });
                            }
                        }
                        AgregarHora = false;
                        var responsable = new cPersona().Obtener(SelectedEmpleadoValue.ID_EMPLEADO).FirstOrDefault();
                        LstAgenda = LstAgenda ?? new ObservableCollection<Appointment>();
                        LstAgenda.Add(new Appointment()
                        {
                            Subject = string.Format("{0}", SelectedIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectedIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectedIngreso.IMPUTADO.MATERNO.Trim())),
                            StartTime = _nueva_fecha_inicio,
                            EndTime = _nueva_fecha_final,
                        });
                        LstAgenda = new ObservableCollection<Appointment>(LstAgenda.OrderBy(o => o.ID_CITA));
                        ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>(ListProcMedsSeleccionados.OrderBy(o => o.PROC_MED_DESCR));
                    }
                }
            }
            catch (Exception ex)
            {
                AgendaView.Close();
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la agenda.", ex);
            }
        }
        #endregion

        private enum TIPO_GUARDAR_HOSPITALIZACION
        {
            INGRESO=1,
            ALTA=2
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                MenuAgregarEnabled = false;
                ReporteMenuEnabled = false;
                IsBuscarporFechaEnabled = false;
                _permisos_agregar = false;
                permisos = new cProcesoUsuario().Obtener(enumProcesos.BITACORA_HOSPITALIZACION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                    {
                        MenuAgregarEnabled = true;
                        _permisos_agregar = true;
                    }
                    if (p.CONSULTAR == 1)
                    {
                        IsBuscarporFechaEnabled = true;
                        ReporteMenuEnabled = true;
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
