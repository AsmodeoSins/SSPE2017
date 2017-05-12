using ControlPenales.BiometricoServiceReference;
using System.Linq;
namespace ControlPenales
{
    partial class AgendaEspecialistasViewModel : ValidationViewModelBase
    {
        public AgendaEspecialistasViewModel() { }

        public async void AgendaEspecialistasOnLoad(object sender)
        {
            try
            {
                AgendaEspecialistasView window = (AgendaEspecialistasView)sender;
                estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                window.Agenda.AddAppointment += Agenda_AddAppointment;
                BusquedaFecha = FechaAgenda = _FechaServer;
                ConfiguraPermisos();

                //await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                //{
                roles = new SSP.Controlador.Catalogo.Justicia.cUsuarioRol().ObtenerTodos(GlobalVar.gUsr).Where(w => w.ID_CENTRO == GlobalVar.gCentro && (w.ID_ROL == (short)enumRolesAreasTecnicas.MEDICO || w.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA)).Select(s => s.ID_ROL).ToList();
                CargarCatalogos(true);
                //});
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la agenda es especialistas", ex);
            }
        }

        void Agenda_AddAppointment(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (!MenuBuscarEnabled)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                };

                if (!AgregarAgendaHoraI.HasValue || AgregarAgendaHoraI.Value < Fechas.GetFechaDateServer)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "La fecha y hora tienen que ser mayor a la fecha actual");
                    return;
                }

                if (!SelectedEspecialista.HasValue || SelectedEspecialista == -1)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un especialista");
                    return;
                }

                selectedAgendaEmpleadoValue = SelectedEspecialista.Value;
                RaisePropertyChanged("SelectedAgendaEmpleadoValue");
                isAgendaEmpleadoEnabled = false;
                RaisePropertyChanged("IsAgendaEmpleadoEnabled");
                tipoAgregarAgenda = ModoAgregarAgenda.INSERTAR;

                //REGRESA LOS CAMPOS A COMO ESTABAN
                CamposCitasEspecialistas = true;
                OnPropertyChanged("CamposCitasEspecialistas");
                VisibleCitasPendientes = System.Windows.Visibility.Visible;
                OnPropertyChanged("VisibleCitasPendientes");
                //FIN

                selectIngreso = null;
                selectedAtencion_Cita = null;
                setAgregarAgendaValidacionRules();
                setReadOnlyDatosImputadosAgregarAgenda(false);
                Limpiar();
                AgregarAgendaFechaValid = true;
                IsEnabledBuscarImputadoAgregarAgenda = true;
                fechaOriginal = null;
                InicializaListaInterconsultas();
                LimpiarCamposBusqueda();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar a la agenda de especialista.", ex);
            }
        }

        private void InicializaListaInterconsultas()
        {
            try
            {
                LstPrioridadesBuscar = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INTERCONSULTA_NIVEL_PRIORIDAD>(new SSP.Controlador.Catalogo.Justicia.cNivelPrioridad().GetData(x => x.ESTATUS == "S", y => y.DESCR));
                LstPrioridadesBuscar.Insert(0, new SSP.Servidor.INTERCONSULTA_NIVEL_PRIORIDAD { DESCR = "SELECCIONE", ID_INIVEL = -1 });

                IdBuscarPrioridades = -1;
                var _ListaTemporal = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SOL_INTERCONSULTA_INTERNA>(new SSP.Controlador.Catalogo.Justicia.cSolicitudInterconsultaInterna().GetData(x => x.INTERCONSULTA_SOLICITUD != null && x.INTERCONSULTA_SOLICITUD.ESTATUS == "S" && x.INTERCONSULTA_SOLICITUD.ID_INTER == 1 && x.INTERCONSULTA_SOLICITUD.ID_INTERAT != 4));
                ListaInterconsultasBusqueda = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SOL_INTERCONSULTA_INTERNA>();
                if (_ListaTemporal.Any())
                    foreach (var item in _ListaTemporal)
                    {
                        if (item.INTERCONSULTA_SOLICITUD != null)
                            if (item.INTERCONSULTA_SOLICITUD.ID_ESPECIALIDAD != SelectedEspecialidad)
                                continue;

                        if (item.ESPECIALISTA_CITA.Any())
                            continue;

                        //ListaInterconsultasBusqueda.Add(item);
                        System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            ListaInterconsultasBusqueda.Add(item);
                        }));
                    };

                OnPropertyChanged("ListaInterconsultasBusqueda");
                if (SelectedEspecialista != null)
                    if (LstNombresEspecialistas != null)
                        if (LstNombresEspecialistas.Any())
                        {
                            var _Especialista = LstNombresEspecialistas.FirstOrDefault(x => x.IdEspecialista == SelectedEspecialista);
                            if (_Especialista != null)
                            {
                                LstEspecialistasMostrar = new System.Collections.ObjectModel.ObservableCollection<NombresEspecialistas>();
                                LstEspecialistasMostrar.Add(new NombresEspecialistas
                                {
                                    IdEspecialista = _Especialista.IdEspecialista,
                                    IdPersona = _Especialista.IdPersona,
                                    NombreEspecialista = _Especialista.NombreEspecialista
                                });

                                IdEspecialistaMostrado = SelectedEspecialista.Value;
                            };
                        };
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar a la agenda de especialista.", exc);
            }
        }

        public static void SalirMenu()
        {
            try
            {
                var metro = System.Windows.Application.Current.Windows[0] as MahApps.Metro.Controls.MetroWindow;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                System.GC.Collect();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de la agenda de especialista", ex);
            }
        }

        private void LimpiarCamposBusqueda()
        {
            try
            {
                AnioBuscarInter = null;
                FolioBuscarInter = null;
                NombreBuscarInter = ApellidoPaternoBuscarInter = ApellidoMaternoBuscarInter = string.Empty;
                IdBuscarPrioridades = -1;
                FechaInicialBuscarInter = null;
                FechaFinalBuscarInter = null;
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar los campos de la búsqueda", exc);
            }
        }

        public async void OnClickSwitch(object parametro)
        {
            if (parametro != null)
                switch (parametro.ToString())
                {
                    case "salir_menu":
                        SalirMenu();
                        break;
                    case "limpiar_menu":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new AgendaEspecialistasView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.AgendaEspecialistasViewModel();
                        break;
                    case "buscar_agenda":
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                            return;
                        }
                        if (permisos_crear)
                            IsAgendaEnabled = true;

                        Limpiar();
                        setReadOnlyDatosImputadosAgregarAgenda(true);
                        await CargarAgenda();
                        break;
                    case "cancelar_agregar_agenda_medica":
                        selectIngresoAuxiliar = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
                        setBuscarAgendaValidacionRules();
                        break;
                    case "cancelar_buscar_interconsulta_especialista":
                        selectIngresoAuxiliar = null;
                        //REGRESA LOS CAMPOS A COMO ESTABAN
                        CamposCitasEspecialistas = true;
                        OnPropertyChanged("CamposCitasEspecialistas");
                        VisibleCitasPendientes = System.Windows.Visibility.Visible;
                        OnPropertyChanged("VisibleCitasPendientes");
                        //FIN

                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
                        break;
                    case "seleccionar_interconsulta_especialista":
                        if (SelectedInterconsultaBusqueda != null)
                        {
                            setAgregarAgendaValidacionRules();
                            if (base.HasErrors)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                                return;
                            };

                            selectIngreso = SelectedInterconsultaBusqueda != null ? SelectedInterconsultaBusqueda.INTERCONSULTA_SOLICITUD != null ? SelectedInterconsultaBusqueda.INTERCONSULTA_SOLICITUD.CANALIZACION != null ? SelectedInterconsultaBusqueda.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA != null ? SelectedInterconsultaBusqueda.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ? SelectedInterconsultaBusqueda.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO : null : null : null : null : null;
                            AgregarAgenda();
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione una interconsulta para continuar!");

                        break;
                    case "agregar_agenda_medica":
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                            return;
                        }
                        if (selectedAtencion_Cita == null && selectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "SELECCIONAR UN IMPUTADO ES REQUERIDO!");
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

                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
                            setIncidenteMotivo();
                            Observacion = string.Empty;
                        }
                        else
                            AgregarAgenda();

                        break;
                    case "buscar_imputadoagregaragenda":
                        break;
                    case "nueva_busqueda":
                        ListExpediente.Clear();
                        ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                        FolioBuscar = AnioBuscar = null;
                        SelectExpediente = new SSP.Servidor.IMPUTADO();
                        EmptyExpedienteVisible = true;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        SelectIngresoEnabled = false;
                        break;
                    case "buscar_salir":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        resetIngreso();
                        if (tipoBusquedaImputado == ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO)
                        {
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
                        }
                        else
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_AGENDA);
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
                            if (tipoBusquedaImputado == ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO)
                            {
                                if (!selectIngreso.ATENCION_MEDICA.Any(a => a.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && a.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO))
                                {
                                    _tipo_error = 4;
                                    return false;
                                }
                                if ((selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_MEDICA && SelectIngreso.HISTORIA_CLINICA.Any(w => w.ESTATUS == "T")) ||
                                    (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_DENTAL && SelectIngreso.HISTORIA_CLINICA_DENTAL.Any(w => w.ESTATUS == "T")))
                                {
                                    _tipo_error = 3;
                                    return false;
                                }
                                if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_DENTAL && !SelectIngreso.HISTORIA_CLINICA.Any(w => w.ESTATUS == "T"))
                                {
                                    _tipo_error = 5;
                                    return false;
                                }
                                if ((selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_MEDICA && selectIngreso.ATENCION_CITA.Any(w => w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_MEDICA)) ||
                                (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_DENTAL && selectIngreso.ATENCION_CITA.Any(w => w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_DENTAL)))
                                {
                                    _tipo_error = 6;
                                    return false;
                                }
                                folioImputadoAgregarAgenda = SelectIngreso.ID_IMPUTADO;
                                RaisePropertyChanged("FolioImputadoAgregarAgenda");
                                anioImputadoAgregarAgenda = SelectIngreso.ID_ANIO;
                                RaisePropertyChanged("AnioImputadoAgregarAgenda");
                                apPaternoAgregarAgenda = SelectIngreso.IMPUTADO.PATERNO;
                                RaisePropertyChanged("ApPaternoAgregarAgenda");
                                apMaternoAgregarAgenda = SelectIngreso.IMPUTADO.MATERNO;
                                RaisePropertyChanged("ApMaternoAgregarAgenda");
                                nombreAgregarAgenda = SelectIngreso.IMPUTADO.NOMBRE;
                                RaisePropertyChanged("NombreAgregarAgenda");
                                sexoImputadoAgregarAgenda = string.IsNullOrWhiteSpace(SelectIngreso.IMPUTADO.SEXO) ? string.Empty : SelectIngreso.IMPUTADO.SEXO == "M" ? "Masculino" : "Femenino";
                                RaisePropertyChanged("SexoImputadoAgregarAgenda");
                                edadImputadoAgregarAgenda = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? (short?)new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value) : null;
                                RaisePropertyChanged("EdadImputadoAgregarAgenda");
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).Any())
                                    imagenAgregarAgenda = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    imagenAgregarAgenda = new Imagenes().getImagenPerson();
                                RaisePropertyChanged("ImagenAgregarAgenda");
                            }
                            else
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
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).Any())
                                    buscarImagenImputadoAgenda = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    buscarImagenImputadoAgenda = new Imagenes().getImagenPerson();
                                RaisePropertyChanged("BuscarImagenImputadoAgenda");
                                headerBuscarAgenda = "Citas del imputado - " + selectIngreso.ID_ANIO.ToString() + "/" + selectIngreso.ID_IMPUTADO.ToString();
                                RaisePropertyChanged("HeaderBuscarAgenda");
                                if (new SSP.Controlador.Catalogo.Justicia.cTecnicaArea().IsUsuarioCoordinadorAreaTecnica((short)eAreas.AREA_MEDICA, GlobalVar.gUsr))
                                    busquedaAgenda = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA>(new SSP.Controlador.Catalogo.Justicia.cAtencionCita().ObtenerTodoPorImputado(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                                    selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, fechaInicial, null, selectedBusquedaAgendaAtencionTipo.Value));
                                else
                                    busquedaAgenda = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA>(new SSP.Controlador.Catalogo.Justicia.cAtencionCita().ObtenerTodoPorImputado(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                                    selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, fechaInicial, null, selectedBusquedaAgendaAtencionTipo.Value, GlobalVar.gUsr));
                                RaisePropertyChanged("BusquedaAgenda");
                            }
                            selectIngresoAuxiliar = SelectIngreso;
                            return true;
                        }))
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            if (tipoBusquedaImputado == ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO)
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
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
                                case 3:
                                    var _hc_tipo = string.Empty;
                                    if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_MEDICA)
                                        _hc_tipo = "historia clinica médica";
                                    else if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_DENTAL)
                                        _hc_tipo = "historia clinica dental";
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                        SelectIngreso.ID_IMPUTADO.ToString() + "] ya cuenta con su " + _hc_tipo + " completa.");
                                    break;
                                case 4:
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                        SelectIngreso.ID_IMPUTADO.ToString() + "] no cuenta con su certificado medico de ingreso.");

                                    break;
                                case 5:
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                        SelectIngreso.ID_IMPUTADO.ToString() + "] no cuenta con su historia clinica médica.");

                                    break;
                                case 6:
                                    var _hc_tipo2 = string.Empty;
                                    if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_MEDICA)
                                        _hc_tipo2 = "historia clinica médica";
                                    else if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_DENTAL)
                                        _hc_tipo2 = "historia clinica dental";
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                        SelectIngreso.ID_IMPUTADO.ToString() + "] ya cuenta con una cita para " + _hc_tipo2 + ".");
                                    break;
                            }
                        }

                        break;
                    case "buscar_menu":
                        LimpiarBusqueda();
                        if (!SelectedEspecialista.HasValue || SelectedEspecialista == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un especialista");
                            return;
                        }

                        selectedAgendaEmpleadoValue = SelectedEspecialista.Value;
                        RaisePropertyChanged("SelectedAgendaEmpleadoValue");
                        isAgendaEmpleadoEnabled = false;
                        RaisePropertyChanged("IsAgendaEmpleadoEnabled");
                        tipoAgregarAgenda = ModoAgregarAgenda.INSERTAR;
                        selectIngreso = null;
                        selectedAtencion_Cita = null;
                        setAgregarAgendaValidacionRules();
                        setReadOnlyDatosImputadosAgregarAgenda(false);
                        Limpiar();
                        AgregarAgendaFechaValid = true;
                        IsEnabledBuscarImputadoAgregarAgenda = true;
                        fechaOriginal = null;
                        InicializaListaInterconsultas();
                        LimpiarCamposBusqueda();

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
                        break;
                    case "filtro_busqueda_imputado_agenda":
                        break;
                    case "cancelar_buscar_agenda":
                        selectIngresoAuxiliar = null;
                        setBuscarAgendaValidacionRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_AGENDA);
                        break;
                    case "seleccionar_buscar_agenda":
                        if (SelectedBuscarAgenda == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "SELECCIONAR UNA CITA ES REQUERIDO!");
                            return;
                        }
                        SelectedEmpleadoValue = LstEmpleados.FirstOrDefault(w => w.Usuario.ID_USUARIO == SelectedBuscarAgenda.ID_USUARIO).ID_EMPLEADO;
                        SelectedAtencionTipo = SelectedBuscarAgenda.ID_TIPO_ATENCION;
                        BusquedaFecha = SelectedBuscarAgenda.CITA_FECHA_HORA.Value;

                        if (permisos_crear)
                            IsAgendaEnabled = true;
                        await CargarAgenda();
                        selectIngresoAuxiliar = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_AGENDA);
                        setBuscarAgendaValidacionRules();
                        break;
                    case "agregar_incidente":
                        var _incidencia = new SSP.Servidor.ATENCION_CITA_INCIDENCIA
                        {
                            ATENCION_ORIGINAL_FEC = fechaOriginal,
                            ID_ACMOTIVO = selectedIncidenteMotivoValue,
                            ID_USUARIO = GlobalVar.gUsr,
                            OBSERV = Observacion,
                            REGISTRO_FEC = _FechaServer
                        };
                        AgregarAgenda(_incidencia);
                        break;
                    case "cancelar_incidente":
                        setAgregarAgendaValidacionRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
                        break;


                    case "cancelar_buscar_interconsulta":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
                        break;
                    case "filtro_interconsultas":
                        if (!IsFechaIniBusquedaSolValida)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La fecha de inicio tiene que ser menor a la fecha fin!");
                            return;
                        }
                        //await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        //{
                        BuscarSolicitudesInterconsulta(selectedAtencion_TipoBuscarValue == -1 ? (short?)null : selectedAtencion_TipoBuscarValue, anioBuscarInter, folioBuscarInter,
                            nombreBuscarInter, apellidoPaternoBuscarInter, apellidoMaternoBuscarInter, IdBuscarPrioridades, fechaInicialBuscarInter, fechaFinalBuscarInter, true);
                        //});

                        break;
                    case "seleccionar_interconsulta":
                        break;
                }
        }

        private void BuscarSolicitudesInterconsulta(short? tipo_atencion = null, short? anio_imputado = null, int? folio_imputado = null, string nombre = "", string paterno = "", string materno = "", short? tipo_interconsulta = null,
    System.DateTime? fecha_inicio = null, System.DateTime? fecha_final = null, bool isExceptionManaged = false)
        {
            try
            {
                ListaInterconsultasBusqueda = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SOL_INTERCONSULTA_INTERNA>();
                var _CitasGeneral = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SOL_INTERCONSULTA_INTERNA>(new SSP.Controlador.Catalogo.Justicia.cSolicitudInterconsultaInterna().BuscarSolicitudes(new System.Collections.Generic.List<string> { "E", "S" }, (short)enumAtencionTipo.CONSULTA_MEDICA, anio_imputado, folio_imputado, nombre, paterno, materno, tipo_interconsulta, fecha_inicio, fecha_final));//.Where(x => !x.ESPECIALISTA_CITA.Any()));
                if (_CitasGeneral.Any())
                    foreach (var item in _CitasGeneral)
                    {
                        if (item.INTERCONSULTA_SOLICITUD != null)
                            if (item.INTERCONSULTA_SOLICITUD.ID_ESPECIALIDAD != SelectedEspecialidad)
                                continue;

                        if (item.ESPECIALISTA_CITA != null)
                            if (item.ESPECIALISTA_CITA.Any())
                                continue;

                        System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                          {
                              ListaInterconsultasBusqueda.Add(item);
                          }));
                    };

                RaisePropertyChanged("ListaInterconsultasBusqueda");
            }
            catch (System.Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar las solicitudes de interconsulta", ex);
            }
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


        private async void AgregarAgenda(SSP.Servidor.ATENCION_CITA_INCIDENCIA _incidencia = null)
        {
            System.DateTime _nueva_fecha_inicio = new System.DateTime(agregarAgendaFecha.Value.Year, agregarAgendaFecha.Value.Month, agregarAgendaFecha.Value.Day,
                                AgregarAgendaHoraI.Value.Hour, AgregarAgendaHoraI.Value.Minute, 0);
            System.DateTime _nueva_fecha_final = new System.DateTime(agregarAgendaFecha.Value.Year, agregarAgendaFecha.Value.Month, agregarAgendaFecha.Value.Day,
                            AgregarAgendaHoraF.Value.Hour, AgregarAgendaHoraF.Value.Minute, 0);
            if (_nueva_fecha_inicio < Fechas.GetFechaDateServer)
            {
                new Dialogos().ConfirmacionDialogo("Validación!", "LA FECHA DE LA CITA NO PUEDE SER MENOR A LA FECHA Y HORA ACTUAL!");
                return;
            }

            if (AgregarAgendaHoraI.Value.Minute % 15 != 0 || AgregarAgendaHoraF.Value.Minute % 15 != 0)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "LOS INTERVALOS DE ATENCIÓN TIENEN QUE SER EN BLOQUES DE 15 MINUTOS!");
                return;
            }

            if (tipoAgregarAgenda == ModoAgregarAgenda.EDICION)
            {
                if (SelectedEspecialista == null)
                    return;

                if (SelectedEspecialidad == null)
                    return;

                var otra_cita = new SSP.Controlador.Catalogo.Justicia.cAtencionCita().ObtieneCitaOtraArea(selectedAtencion_Cita.INGRESO.ID_CENTRO, selectedAtencion_Cita.INGRESO.ID_ANIO,
                    selectedAtencion_Cita.INGRESO.ID_IMPUTADO, selectedAtencion_Cita.INGRESO.ID_INGRESO, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1), selectedAtencion_Cita.ID_CITA, selectedAtencion_Cita.ID_CENTRO_UBI);
                if (otra_cita == null)
                {
                    if (!new SSP.Controlador.Catalogo.Justicia.cAtencionCita().IsOverlapCita(selectedAtencion_Cita.ID_TIPO_ATENCION.Value, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1), estatus_inactivos, selectedAtencion_Cita.ID_CITA)) // a la hora final hay que restarle un minuto para no traslapar horarios que comienzan terminan a la misma hora
                    {
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando cita de especialista", () =>
                        {
                            var _atencion_cita = new SSP.Servidor.ATENCION_CITA
                            {
                                CITA_FECHA_HORA = _nueva_fecha_inicio,
                                CITA_HORA_TERMINA = _nueva_fecha_final,
                                ESTATUS = "N",
                                ID_AREA = selectedArea.Value,
                                ID_ANIO = selectIngreso.ID_ANIO,
                                ID_ATENCION_MEDICA = null,
                                ID_CENTRO = selectIngreso.ID_CENTRO,
                                ID_IMPUTADO = selectIngreso.ID_IMPUTADO,
                                ID_CITA = selectedAtencion_Cita.ID_CITA,
                                ID_INGRESO = selectIngreso.ID_INGRESO,
                                ID_TIPO_ATENCION = (short)enumAtencionTipo.CONSULTA_MEDICA,
                                ID_TIPO_SERVICIO = (short)enumAtencionServicio.ESPECIALIDAD_INTERNA,
                                ID_RESPONSABLE = null,
                                ID_USUARIO = GlobalVar.gUsr,
                                ID_CENTRO_UBI = SelectedInterconsultaBusqueda.ID_CENTRO_UBI
                            };

                            var _AgendaEspecialistaCita = new SSP.Servidor.ESPECIALISTA_CITA()
                            {
                                ID_ESPECIALISTA = SelectedEspecialista.Value,
                                ID_USUARIO = GlobalVar.gUsr,
                                ID_ESPECIALIDAD = SelectedEspecialidad.Value,
                                REGISTRO_FEC = Fechas.GetFechaDateServer,
                                ID_SOLICITUD = SelectedInterconsultaBusqueda.ID_SOLICITUD,
                                ID_CITA = selectedAtencion_Cita.ID_CITA,
                                ID_CENTRO_UBI = SelectedInterconsultaBusqueda.ID_CENTRO_UBI,
                                SOL_INTERCONSULTA_INTERNA = SelectedInterconsultaBusqueda
                            };

                            _AgendaEspecialistaCita.ATENCION_CITA = _atencion_cita;
                            _AgendaEspecialistaCita.SOL_INTERCONSULTA_INTERNA = SelectedInterconsultaBusqueda;
                            return new SSP.Controlador.Catalogo.Justicia.cCitasEspecialistas().ActualizarEspecialistaCita(_AgendaEspecialistaCita);
                        }))
                        {
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La cita de especialista fue actualizada con exito");
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
                            setBuscarAgendaValidacionRules();
                            await CargarAgenda();
                        }
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "LA CITA SE TRASLAPA CON OTRA CITA!");
                        return;
                    }
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "LA CITA SE TRASLAPA CON OTRA CITA DEL IMPUTADO!");
                    return;
                }
            }
            else
            {
                if (SelectedEspecialista == null)
                    return;

                if (SelectedEspecialidad == null)
                    return;

                var otra_cita = new SSP.Controlador.Catalogo.Justicia.cAtencionCita().ObtieneCitaOtraArea(selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                    selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1));
                if (otra_cita == null)
                {
                    if (!new SSP.Controlador.Catalogo.Justicia.cAtencionCita().IsOverlapCita(selectedAtencionTipo.Value, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1), estatus_inactivos)) // a la hora final hay que restarle un minuto para no traslapar horarios que comienzan terminan a la misma hora
                    {
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando cita de especialista", () =>
                        {
                            var _detallesEspecialista = new SSP.Controlador.Catalogo.Justicia.cCitasEspecialistas().GetData(x => x.ID_ESPECIALISTA == SelectedEspecialista).FirstOrDefault();
                            var _atencion_cita = new SSP.Servidor.ATENCION_CITA
                            {
                                CITA_FECHA_HORA = _nueva_fecha_inicio,
                                CITA_HORA_TERMINA = _nueva_fecha_final,
                                ESTATUS = "N",
                                ID_AREA = selectedArea.Value,
                                ID_ANIO = selectIngreso.ID_ANIO,
                                ID_ATENCION_MEDICA = null,
                                ID_CENTRO = selectIngreso.ID_CENTRO,
                                ID_IMPUTADO = selectIngreso.ID_IMPUTADO,
                                ID_INGRESO = selectIngreso.ID_INGRESO,
                                ID_TIPO_ATENCION = (short)enumAtencionTipo.CONSULTA_MEDICA,
                                ID_TIPO_SERVICIO = (short)enumAtencionServicio.ESPECIALIDAD_INTERNA,
                                ID_RESPONSABLE = null,
                                ID_USUARIO = GlobalVar.gUsr,
                                ID_CENTRO_UBI = SelectedInterconsultaBusqueda.ID_CENTRO_UBI
                            };

                            var _AgendaEspecialistaCita = new SSP.Servidor.ESPECIALISTA_CITA()
                            {
                                ID_ESPECIALISTA = SelectedEspecialista.Value,
                                ID_USUARIO = GlobalVar.gUsr,
                                ID_ESPECIALIDAD = SelectedEspecialidad.Value,
                                REGISTRO_FEC = Fechas.GetFechaDateServer,
                                ID_SOLICITUD = SelectedInterconsultaBusqueda.ID_SOLICITUD,
                                ID_CENTRO_UBI = SelectedInterconsultaBusqueda.ID_CENTRO_UBI,
                                SOL_INTERCONSULTA_INTERNA = SelectedInterconsultaBusqueda
                            };

                            _AgendaEspecialistaCita.ATENCION_CITA = _atencion_cita;
                            if (new SSP.Controlador.Catalogo.Justicia.cCitasEspecialistas().InsertarCita(_AgendaEspecialistaCita))
                                return true;
                            else
                                return false;
                        }))
                        {
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La cita fue actualizada con exito");
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
                            setBuscarAgendaValidacionRules();
                            selectIngresoAuxiliar = null;
                            await CargarAgenda();
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Error!", "Surgió un error al ingresar la cita");
                        }
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "LA CITA SE TRASLAPA CON OTRA CITA!");
                        return;
                    }
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "LA CITA SE TRASLAPA CON OTRA CITA DEL IMPUTADO!");
                    return;
                }
            }
        }

        private void CargarAtencionCitaInMotivo(bool isExceptionManaged = false)
        {
            try
            {
                lstIncidenteMotivo = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA_IN_MOTIVO>(new SSP.Controlador.Catalogo.Justicia.cAtencion_Cita_In_Motivo().ObtenerTodos());
                lstIncidenteMotivo.Insert(0, new SSP.Servidor.ATENCION_CITA_IN_MOTIVO { ID_ACMOTIVO = -1, DESCR = "SELECCIONE" });
                OnPropertyChanged("LstIncidenteMotivo");
                selectedIncidenteMotivoValue = -1;
                RaisePropertyChanged("SelectedIncidenteMotivoValue");
            }
            catch (System.Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de motivos de incidencia", ex);
                else
                    throw ex;
            }
        }

        private void AgendaClick(System.Object obj)
        {
            try
            {
                AgregarAgendaHoraI = new System.DateTime(FechaAgenda.Year, FechaAgenda.Month, FechaAgenda.Day, int.Parse(obj.ToString().Split(':')[0]), int.Parse(obj.ToString().Split(':')[1]), 0);
                AgregarAgendaHoraF = new System.DateTime(FechaAgenda.Year, FechaAgenda.Month, FechaAgenda.Day, int.Parse(obj.ToString().Split(':')[1]) == 45 ? int.Parse(obj.ToString().Split(':')[0]) + 1 : int.Parse(obj.ToString().Split(':')[0]),
                    int.Parse(obj.ToString().Split(':')[1]) == 45 ? int.Parse(obj.ToString().Split(':')[1]) - 45 : int.Parse(obj.ToString().Split(':')[1]) + 15, 0);
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar nuevos datos a la agenda.", ex);
            }
        }

        private async System.Threading.Tasks.Task CargarAgenda()
        {
            if (SelectedEspecialista == null)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un especialista para visualizar su agenda");
                return;
            };

            if (SelectedEspecialista == -1)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un especialista para visualizar su agenda");
                return;
            };

            var _EspecialistaElegidoComboBox = LstNombresEspecialistas.FirstOrDefault(x => x.IdEspecialista == SelectedEspecialista);
            if (_EspecialistaElegidoComboBox == null)
                return;

            var _detallesEspecialista = new SSP.Controlador.Catalogo.Justicia.cEspecialistas().GetData().FirstOrDefault(x => x.ID_ESPECIALISTA == SelectedEspecialista);
            if (_detallesEspecialista == null)
                return;

            _empleado = new cUsuarioExtendida()
            {
                ID_EMPLEADO = _detallesEspecialista.ID_ESPECIALISTA,
                NOMBRE_COMPLETO = _detallesEspecialista != null ? _detallesEspecialista.PERSONA != null ?
                    string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(_detallesEspecialista.PERSONA.NOMBRE) ? _detallesEspecialista.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_detallesEspecialista.PERSONA.PATERNO) ? _detallesEspecialista.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_detallesEspecialista.PERSONA.MATERNO) ? _detallesEspecialista.PERSONA.MATERNO.Trim() : string.Empty)
                            : string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_detallesEspecialista.ESPECIALISTA_NOMBRE) ? _detallesEspecialista.ESPECIALISTA_NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_detallesEspecialista.ESPECIALISTA_PATERNO) ? _detallesEspecialista.ESPECIALISTA_PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_detallesEspecialista.ESPECIALISTA_MATERNO) ? _detallesEspecialista.ESPECIALISTA_MATERNO.Trim() : string.Empty)
                                    : string.Format("{0} {1} {2}",
                                        !string.IsNullOrEmpty(_detallesEspecialista.ESPECIALISTA_NOMBRE) ? _detallesEspecialista.ESPECIALISTA_NOMBRE.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(_detallesEspecialista.ESPECIALISTA_PATERNO) ? _detallesEspecialista.ESPECIALISTA_PATERNO.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(_detallesEspecialista.ESPECIALISTA_MATERNO) ? _detallesEspecialista.ESPECIALISTA_MATERNO.Trim() : string.Empty),
                Usuario = _detallesEspecialista.USUARIO
            };

            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                selectedAtencionTipoAgenda = selectedAtencionTipo.Value;
                headerAgenda = "AGENDA DE " + _EspecialistaElegidoComboBox.NombreEspecialista;
                RaisePropertyChanged("HeaderAgenda");
                var agenda = new SSP.Controlador.Catalogo.Justicia.cCitasEspecialistas().GetData(x => x.ID_ESPECIALISTA == _detallesEspecialista.ID_ESPECIALISTA); //new SSP.Controlador.Catalogo.Justicia.cAtencionCita().ObtenerPorUsuarioyTipoAtencionMedica(GlobalVar.gCentro, _empleado.Usuario.ID_USUARIO, selectedAtencionTipoAgenda.Value, estatus_inactivos, new List<string> { "N", "A" });
                if (agenda != null && agenda.Count() > 0)
                {
                    lstAgenda = new System.Collections.ObjectModel.ObservableCollection<Appointment>();
                    fechasAgendadas = new System.Collections.Generic.List<System.DateTime>();
                    foreach (var a in agenda)
                        if (a.ATENCION_CITA != null)
                        {
                            if (!fechasAgendadas.Contains(System.DateTime.Parse(a.ATENCION_CITA.CITA_FECHA_HORA.Value.ToShortDateString())))
                                fechasAgendadas.Add(System.DateTime.Parse(a.ATENCION_CITA.CITA_FECHA_HORA.Value.ToShortDateString()));
                            lstAgenda.Add(new Appointment()
                            {
                                Subject = string.Format("{0} {1} {2}",
                                a.ATENCION_CITA != null ? a.ATENCION_CITA.INGRESO != null ? a.ATENCION_CITA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(a.ATENCION_CITA.INGRESO.IMPUTADO.NOMBRE) ? a.ATENCION_CITA.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                a.ATENCION_CITA != null ? a.ATENCION_CITA.INGRESO != null ? a.ATENCION_CITA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(a.ATENCION_CITA.INGRESO.IMPUTADO.PATERNO) ? a.ATENCION_CITA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                a.ATENCION_CITA != null ? a.ATENCION_CITA.INGRESO != null ? a.ATENCION_CITA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(a.ATENCION_CITA.INGRESO.IMPUTADO.MATERNO) ? a.ATENCION_CITA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty),
                                StartTime = new System.DateTime(a.ATENCION_CITA.CITA_FECHA_HORA.Value.Year, a.ATENCION_CITA.CITA_FECHA_HORA.Value.Month, a.ATENCION_CITA.CITA_FECHA_HORA.Value.Day, a.ATENCION_CITA.CITA_FECHA_HORA.Value.Hour, a.ATENCION_CITA.CITA_FECHA_HORA.Value.Minute, a.ATENCION_CITA.CITA_FECHA_HORA.Value.Second),
                                EndTime = new System.DateTime(a.ATENCION_CITA.CITA_HORA_TERMINA.Value.Year, a.ATENCION_CITA.CITA_HORA_TERMINA.Value.Month, a.ATENCION_CITA.CITA_HORA_TERMINA.Value.Day, a.ATENCION_CITA.CITA_HORA_TERMINA.Value.Hour, a.ATENCION_CITA.CITA_HORA_TERMINA.Value.Minute, a.ATENCION_CITA.CITA_HORA_TERMINA.Value.Second),
                                ID_CITA = a.ID_CITA
                            });
                        };

                    lstAgenda = new System.Collections.ObjectModel.ObservableCollection<Appointment>(lstAgenda);
                    RaisePropertyChanged("LstAgenda");
                    fechasAgendadas = new System.Collections.Generic.List<System.DateTime>(fechasAgendadas);
                    RaisePropertyChanged("FechasAgendadas");
                }
                else
                {
                    lstAgenda = new System.Collections.ObjectModel.ObservableCollection<Appointment>();
                    RaisePropertyChanged("LstAgenda");
                    fechasAgendadas = new System.Collections.Generic.List<System.DateTime>();
                    RaisePropertyChanged("FechasAgendadas");
                }
            });
            var fecha_minima = System.DateTime.Now;
            if (fechasAgendadas != null && fechasAgendadas.Count() > 0)
            {
                fecha_minima = fechasAgendadas.Min();
                FechaInicial = fechasAgendadas.Min();
            }
            if (fecha_minima >= BusquedaFecha.Value)
                BusquedaFecha = fecha_minima;
            FechaAgenda = BusquedaFecha.Value;
        }

        private void Limpiar()
        {
            ImagenAgregarAgenda = new Imagenes().getImagenPerson(); ;
            FolioImputadoAgregarAgenda = null;
            SexoImputadoAgregarAgenda = ApPaternoAgregarAgenda = NombreAgregarAgenda = ApMaternoAgregarAgenda = string.Empty;
            AnioImputadoAgregarAgenda = EdadImputadoAgregarAgenda = null;
            AgregarAgendaFecha = FechaAgenda;
            SelectedArea = -1;
        }

        private async System.Threading.Tasks.Task<System.Collections.Generic.List<SSP.Servidor.IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                return new System.Collections.Generic.List<SSP.Servidor.IMPUTADO>();

            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.IMPUTADO>>(() => new SSP.Controlador.Catalogo.Justicia.cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
            if (result.Any())
            {
                Pagina++;
                SeguirCargando = true;
            }
            else
                SeguirCargando = false;

            return result.ToList();
        }

        private void ClickBuscarInterno(object parametro)
        {
            buscarImputadoInterno(parametro);
        }

        private async void buscarImputadoInterno(System.Object obj = null)
        {
            ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
            ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
            ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
            SelectIngresoEnabled = false;
            if (ListExpediente != null)
                EmptyExpedienteVisible = ListExpediente.Count < 0;
            else
                EmptyExpedienteVisible = true;
        }

        private void LimpiarBusqueda()
        {
            BuscarAnioImputadoAgenda = BuscarFolioImputadoAgenda = null;
            BuscarNombreImputadoAgenda = BuscarApPaternoImputadoAgenda = BuscarApMaternoImputadoAgenda = string.Empty;
        }

        private void setReadOnlyDatosImputadosAgregarAgenda(bool isReadOnly)
        {
            IsReadOnlyAnioImputadoAgregarAgenda = isReadOnly;
            IsReadOnlyApMaternoAgregarAgenda = isReadOnly;
            IsReadOnlyApPaternoAgregarAgenda = isReadOnly;
            IsReadOnlyFolioImputadoAgregarAgenda = isReadOnly;
            IsReadOnlyNombreAgregarAgenda = isReadOnly;
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                MenuBuscarEnabled = false;
                permisos_crear = false;
                permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.AGENDA_ESPECIALISTA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Count() > 0)
                    MenuLimpiarEnabled = true;
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        permisos_crear = true;
                    if (p.CONSULTAR == 1)
                        MenuBuscarEnabled = true;
                };
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion



        private void CargarCatalogos(bool isExceptionManaged = false)
        {
            CargarEspecialidades();
            CargarAtencionTipo(isExceptionManaged);
            CargarAreas(isExceptionManaged);
            CargarBusquedaAgendaAtencionTipo(isExceptionManaged);
        }

        public void CargarEspecialidades()
        {
            try
            {
                LstEspecialidades = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD>(new SSP.Controlador.Catalogo.Justicia.cEspecialidades().GetData(x => x.ESTATUS == "S", y => y.DESCR));
                LstEspecialidades.Insert(0, new SSP.Servidor.ESPECIALIDAD { DESCR = "SELECCIONE", ID_ESPECIALIDAD = -1 });
                SelectedEspecialidad = -1;
                RaisePropertyChanged("LstEspecialidades");

                LstNombresEspecialistas = new System.Collections.ObjectModel.ObservableCollection<NombresEspecialistas>();
                LstNombresEspecialistas.Insert(0, new NombresEspecialistas { IdEspecialista = -1, NombreEspecialista = "SELECCIONE" });
                SelectedEspecialista = -1;
                RaisePropertyChanged("LstNombresEspecialistas");
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de atencion", ex);
            }
        }

        public async void OnModelChangedSwitch(object parametro)
        {
            try
            {
                if (parametro != null)
                {
                    switch (parametro.ToString())
                    {
                        case "cambio_fecha_seleccionada":
                            if (!BusquedaFecha.HasValue)
                                FechaAgendaValid = false;
                            else
                                FechaAgendaValid = true;
                            break;
                        case "cambio_fecha_agregar_agenda":
                            if (AgregarAgendaFecha.HasValue)
                                AgregarAgendaFechaValid = true;
                            else
                                AgregarAgendaFechaValid = false;
                            break;
                        case "cambio_especialidad":
                            if (SelectedEspecialidad != null)
                            {
                                LstNombresEspecialistas = new System.Collections.ObjectModel.ObservableCollection<NombresEspecialistas>();
                                var _Especialistas = new SSP.Controlador.Catalogo.Justicia.cEspecialistas().GetData(x => x.ID_ESPECIALIDAD == SelectedEspecialidad);
                                if (_Especialistas.Any())
                                    foreach (var item in _Especialistas)
                                    {
                                        if (item.ID_PERSONA == null)
                                        {
                                            LstNombresEspecialistas.Add(new NombresEspecialistas
                                            {
                                                IdPersona = item.ID_PERSONA,//NO TIENE PERSONA, PERO TIENE EL ID DE LA TABLA
                                                IdEspecialista = item.ID_ESPECIALISTA,
                                                NombreEspecialista = string.Format("{0} {1} {2}",
                                                !string.IsNullOrEmpty(item.ESPECIALISTA_NOMBRE) ? item.ESPECIALISTA_NOMBRE.Trim() : string.Empty,
                                                !string.IsNullOrEmpty(item.ESPECIALISTA_PATERNO) ? item.ESPECIALISTA_PATERNO.Trim() : string.Empty,
                                                !string.IsNullOrEmpty(item.ESPECIALISTA_MATERNO) ? item.ESPECIALISTA_MATERNO.Trim() : string.Empty
                                                )
                                            });
                                        }
                                        else
                                        {
                                            LstNombresEspecialistas.Add(new NombresEspecialistas
                                            {
                                                IdPersona = item.ID_PERSONA,
                                                IdEspecialista = item.ID_ESPECIALISTA,
                                                NombreEspecialista = string.Format("{0} {1} {2}",
                                                item.PERSONA != null ? !string.IsNullOrEmpty(item.PERSONA.NOMBRE) ? item.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty,
                                                item.PERSONA != null ? !string.IsNullOrEmpty(item.PERSONA.PATERNO) ? item.PERSONA.PATERNO.Trim() : string.Empty : string.Empty,
                                                item.PERSONA != null ? !string.IsNullOrEmpty(item.PERSONA.MATERNO) ? item.PERSONA.MATERNO.Trim() : string.Empty : string.Empty
                                                )
                                            });
                                        }
                                    }

                                LstNombresEspecialistas.Insert(0, new NombresEspecialistas { NombreEspecialista = "SELECCIONE", IdEspecialista = -1 });
                                RaisePropertyChanged("LstNombresEspecialistas");
                                SelectedEspecialista = -1;
                                RaisePropertyChanged("SelectedEspecialista");
                            };
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de informacion", ex);
            }
        }

        private void CargarAtencionTipo(bool isExceptionManaged = false)
        {
            try
            {
                lstAtencionTipos = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_TIPO>(new SSP.Controlador.Catalogo.Justicia.cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                lstAtencionTipos.Insert(0, new SSP.Servidor.ATENCION_TIPO { ID_TIPO_ATENCION = -1, DESCR = "SELECCIONE" });
                OnPropertyChanged("LstAtencionTipos");
            }
            catch (System.Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de atencion", ex);
                else
                    throw ex;
            }
        }

        private void CargarAreas(bool isExceptionManaged = false)
        {
            try
            {
                lstAreas = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.AREA>(new SSP.Controlador.Catalogo.Justicia.cArea().ObtenerTodos());
                lstAreas.Insert(0, new SSP.Servidor.AREA { ID_AREA = -1, DESCR = "SELECCIONE" });
                OnPropertyChanged("LstAtencionTipos");
            }
            catch (System.Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catálogo de áreas", ex);
                else
                    throw ex;
            }
        }

        private void CargarBusquedaAgendaAtencionTipo(bool isExceptionManaged = false)
        {
            try
            {
                lstBusquedaAgendaAtencionTipos = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_TIPO>(new SSP.Controlador.Catalogo.Justicia.cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                lstBusquedaAgendaAtencionTipos.Insert(0, new SSP.Servidor.ATENCION_TIPO { ID_TIPO_ATENCION = -1, DESCR = "SELECCIONE" });
                OnPropertyChanged("LstBusquedaAgendaAtencionTipos");
            }
            catch (System.Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de atencion", ex);
                else
                    throw ex;
            }
        }

        public async void AppointmentClick(object parametro)
        {
            selectIngreso = null;
            if (parametro != null)
            {
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                    return;
                };

                InicializaListaInterconsultas();
                tipoAgregarAgenda = ModoAgregarAgenda.EDICION;
                var _fecha_servidor = Fechas.GetFechaDateServer;
                VisibleCitasPendientes = System.Windows.Visibility.Collapsed;
                OnPropertyChanged("VisibleCitasPendientes");
                CamposCitasEspecialistas = false;
                OnPropertyChanged("CamposCitasEspecialistas");

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    selectedAtencion_Cita = new SSP.Controlador.Catalogo.Justicia.cAtencionCita().Obtener(((Appointment)parametro).ID_CITA.Value, GlobalVar.gCentro);
                    if (selectedAtencion_Cita != null)
                    {
                        var _DetallesCitaMedica = new SSP.Controlador.Catalogo.Justicia.cCitasEspecialistas().GetData(x => x.ID_CITA == selectedAtencion_Cita.ID_CITA).FirstOrDefault();
                        if (_DetallesCitaMedica != null)
                            SelectedInterconsultaBusqueda = _DetallesCitaMedica.SOL_INTERCONSULTA_INTERNA;
                    };

                    fechaOriginal = selectedAtencion_Cita.CITA_FECHA_HORA.HasValue ? selectedAtencion_Cita.CITA_FECHA_HORA.Value : new System.DateTime?();
                    folioImputadoAgregarAgenda = selectedAtencion_Cita.INGRESO != null ? selectedAtencion_Cita.INGRESO.ID_IMPUTADO : new int?();
                    RaisePropertyChanged("FolioImputadoAgregarAgenda");
                    anioImputadoAgregarAgenda = selectedAtencion_Cita.INGRESO != null ? selectedAtencion_Cita.INGRESO.ID_ANIO : new short?();
                    RaisePropertyChanged("AnioImputadoAgregarAgenda");
                    apPaternoAgregarAgenda = selectedAtencion_Cita.INGRESO != null ? selectedAtencion_Cita.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(selectedAtencion_Cita.INGRESO.IMPUTADO.PATERNO) ? selectedAtencion_Cita.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty;
                    RaisePropertyChanged("ApPaternoAgregarAgenda");
                    apMaternoAgregarAgenda = selectedAtencion_Cita.INGRESO != null ? selectedAtencion_Cita.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(selectedAtencion_Cita.INGRESO.IMPUTADO.MATERNO) ? selectedAtencion_Cita.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty;
                    RaisePropertyChanged("ApMaternoAgregarAgenda");
                    nombreAgregarAgenda = selectedAtencion_Cita.INGRESO != null ? selectedAtencion_Cita.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(selectedAtencion_Cita.INGRESO.IMPUTADO.NOMBRE) ? selectedAtencion_Cita.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty;
                    RaisePropertyChanged("NombreAgregarAgenda");
                    sexoImputadoAgregarAgenda = selectedAtencion_Cita.INGRESO != null ? selectedAtencion_Cita.INGRESO.IMPUTADO != null ? string.IsNullOrWhiteSpace(selectedAtencion_Cita.INGRESO.IMPUTADO.SEXO) ? string.Empty : selectedAtencion_Cita.INGRESO.IMPUTADO.SEXO == "M" ? "Masculino" : "Femenino" : string.Empty : string.Empty;
                    RaisePropertyChanged("SexoImputadoAgregarAgenda");
                    edadImputadoAgregarAgenda = selectedAtencion_Cita.INGRESO != null ? selectedAtencion_Cita.INGRESO.IMPUTADO != null ? selectedAtencion_Cita.INGRESO.IMPUTADO.NACIMIENTO_FECHA.HasValue ? (short?)new Fechas().CalculaEdad(selectedAtencion_Cita.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value) : null : null : null;
                    RaisePropertyChanged("EdadImputadoAgregarAgenda");
                    selectedArea = selectedAtencion_Cita.ID_AREA;
                    RaisePropertyChanged("SelectedArea");
                    if (selectedAtencion_Cita.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).Any())
                        imagenAgregarAgenda = selectedAtencion_Cita.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
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
                    tipoServicioDescripcion = selectedAtencion_Cita.ATENCION_SERVICIO != null ? !string.IsNullOrEmpty(selectedAtencion_Cita.ATENCION_SERVICIO.DESCR) ? selectedAtencion_Cita.ATENCION_SERVICIO.DESCR.Trim() : string.Empty : string.Empty;
                    RaisePropertyChanged("TipoServicioDescripcion");
                });
                setAgregarAgendaValidacionRules();
                setReadOnlyDatosImputadosAgregarAgenda(true);
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CITA_ESPECIALISTA);
            }
        }
    }
}