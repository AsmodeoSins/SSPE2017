using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Controlador.Catalogo.Justicia;
using System.Collections.ObjectModel;
using SSP.Servidor;
using ControlPenales.BiometricoServiceReference;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
namespace ControlPenales
{
    partial class AgendaMedicaViewModel : ValidationViewModelBase
    {
        public async void AgendaMedicaOnLoad(object sender)
        {
            try
            {
                AgendaMedicaView window =(AgendaMedicaView) sender;
                estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                window.Agenda.AddAppointment += Agenda_AddAppointment;
                BusquedaFecha = FechaAgenda = _FechaServer;
                ConfiguraPermisos();
               
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                        roles = new cUsuarioRol().ObtenerTodos(StaticSourcesViewModel.UsuarioLogin.Username).Where(w => w.ID_CENTRO == GlobalVar.gCentro && (w.ID_ROL == (short)enumRolesAreasTecnicas.MEDICO || w.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA)).Select(s => s.ID_ROL).ToList();
                        CargarCatalogos(true);
                        if (roles.Count == 1)
                        {

                            selectedAtencionTipoAgenda = roles.First() == (short)enumRolesAreasTecnicas.MEDICO ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL;
                            selectedAtencionTipo = selectedAtencionTipoAgenda;
                            RaisePropertyChanged("SelectedAtencionTipo");
                            headerAgenda = "AGENDA de " + _empleado.NOMBRE_COMPLETO + " - " + new cAtencionTipo().Obtener(selectedAtencionTipoAgenda.Value).DESCR;
                            RaisePropertyChanged("HeaderAgenda");
                            var agenda = new cAtencionCita().ObtenerPorUsuarioyTipoAtencionMedica(GlobalVar.gCentro, _empleado.Usuario.ID_USUARIO, selectedAtencionTipoAgenda.Value, estatus_inactivos, new List<string> { "N", "A" });
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
                            if (!new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica((short)eAreas.AREA_MEDICA, _empleado.Usuario.ID_USUARIO))
                                IsAtencionTiposEnabled = false;
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
            try
            {
                if (!AgregarAgendaHoraI.HasValue || AgregarAgendaHoraI.Value < Fechas.GetFechaDateServer)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "La fecha y hora tienen que ser mayor a la fecha actual");
                    return;
                }
                    
                if (!SelectedAtencionTipo.HasValue || SelectedAtencionTipo == -1)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un tipo de atención");
                    return;
                }
                lstAgendaEmpleados = new List<cUsuarioExtendida>();
                lstAgendaEmpleados.Add(lstEmpleados.FirstOrDefault(w => w.ID_EMPLEADO == _empleado.ID_EMPLEADO));
                RaisePropertyChanged("LstAgendaEmpleados");
                selectedAgendaEmpleadoValue = _empleado.ID_EMPLEADO;
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
                if (selectedAtencionTipo.Value==(short)enumAtencionTipo.CONSULTA_MEDICA)
                    tipoServicioDescripcion = new cAtencionServicio().ObtenerXID(selectedAtencionTipo.Value, (short)enumAtencionServicio.HISTORIA_CLINICA_MEDICA).DESCR;
                else if (selectedAtencionTipo.Value==(short)enumAtencionTipo.CONSULTA_DENTAL)
                    tipoServicioDescripcion = new cAtencionServicio().ObtenerXID(selectedAtencionTipo.Value, (short)enumAtencionServicio.HISTORIA_CLINICA_DENTAL).DESCR;
                RaisePropertyChanged("TipoServicioDescripcion");
                fechaOriginal = null;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar a la agenda.", ex);
            }
        }

        private void AgendaClick(Object obj)
        {
            try
            {
                
                AgregarAgendaHoraI = new DateTime(FechaAgenda.Year, FechaAgenda.Month, FechaAgenda.Day, int.Parse(obj.ToString().Split(':')[0]), int.Parse(obj.ToString().Split(':')[1]), 0);
                AgregarAgendaHoraF = new DateTime(FechaAgenda.Year, FechaAgenda.Month, FechaAgenda.Day, int.Parse(obj.ToString().Split(':')[1]) == 45 ? int.Parse(obj.ToString().Split(':')[0]) + 1 : int.Parse(obj.ToString().Split(':')[0]),
                    int.Parse(obj.ToString().Split(':')[1]) == 45 ? int.Parse(obj.ToString().Split(':')[1]) - 45 : int.Parse(obj.ToString().Split(':')[1]) + 15, 0);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar nuevos datos a la agenda.", ex);
            }
        }

        private void Limpiar()
        {
            ImagenAgregarAgenda = null;
            AnioImputadoAgregarAgenda = null;
            FolioImputadoAgregarAgenda = null;
            NombreAgregarAgenda = string.Empty;
            ApMaternoAgregarAgenda = string.Empty;
            ApPaternoAgregarAgenda = string.Empty;
            SexoImputadoAgregarAgenda = string.Empty;
            EdadImputadoAgregarAgenda = null;
            AgregarAgendaFecha = FechaAgenda;
            SelectedArea = -1;
        }

        private void LimpiarBusqueda()
        {
            BuscarAnioImputadoAgenda = null;
            BuscarFolioImputadoAgenda = null;
            BuscarNombreImputadoAgenda = string.Empty;
            BuscarApPaternoImputadoAgenda = string.Empty;
            BuscarApMaternoImputadoAgenda = string.Empty;
        }

        public static void SalirMenu()
        {
            try
            {
                
                var metro = Application.Current.Windows[0] as MetroWindow;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                GC.Collect();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir del módulo", ex);
            }
        }

        public async void OnClickSwitch(object parametro)
        {
            if (parametro!=null)
                switch (parametro.ToString())
                {
                    case "salir_menu":
                        SalirMenu();
                        break;
                    case "limpiar_menu":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new AgendaMedicaView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.AgendaMedicaViewModel();
                        break;
                    case "buscar_agenda":
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
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
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
                        setBuscarAgendaValidacionRules();
                        break;
                    case "agregar_agenda_medica":
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                            return;
                        }
                        if (selectedAtencion_Cita==null && selectIngreso==null)
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

                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
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
                    case "buscar_imputadoagregaragenda":
                        BuscarImputado();
                        break;
                    case "nueva_busqueda":
                            ListExpediente.Clear();
                            ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                            FolioBuscar = AnioBuscar = null;
                            SelectExpediente = new IMPUTADO();
                            EmptyExpedienteVisible = true;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            SelectIngresoEnabled = false;
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
                    case "buscar_seleccionar":
                        var _tipo_error = 0;
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cargando información",() => {
                            if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                            {
                                _tipo_error = 1;
                                return false;
                            }
                            
                            if (SelectIngreso.TRASLADO_DETALLE.Any(a =>( a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                            {
                                _tipo_error = 2;
                                return false;
                            }
                            if (tipoBusquedaImputado==ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO)
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
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    imagenAgregarAgenda = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
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
                                    busquedaAgenda = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().ObtenerTodoPorImputado(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                                    selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, fechaInicial,  null, selectedBusquedaAgendaAtencionTipo.Value));
                                else
                                    busquedaAgenda = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().ObtenerTodoPorImputado(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                                    selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, fechaInicial,  null, selectedBusquedaAgendaAtencionTipo.Value, GlobalVar.gUsr.Trim()));
                                RaisePropertyChanged("BusquedaAgenda");
                            }
                            selectIngresoAuxiliar = SelectIngreso;
                            return true;
                        }))
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            if (tipoBusquedaImputado==ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO)
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
                                case 3:
                                    var _hc_tipo = string.Empty;
                                    if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_MEDICA)
                                        _hc_tipo = "historia clinica médica";
                                    else if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_DENTAL)
                                        _hc_tipo = "historia clinica dental";
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                        SelectIngreso.ID_IMPUTADO.ToString() + "] ya cuenta con su "+ _hc_tipo +" completa.");
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
                                        SelectIngreso.ID_IMPUTADO.ToString() + "] ya cuenta con una cita para "+ _hc_tipo2 +".");
                                    break;

                            }
                            
                        }
                        break;
                    case "buscar_menu":
                        selectIngreso = null;
                        LimpiarBusqueda();
                        SelectedBusquedaAgendaAtencionTipo = -1;
                        if (roles.Count == 1)
                        {
                            SelectedBusquedaAgendaAtencionTipo = roles.First() == (short)enumRolesAreasTecnicas.MEDICO ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL;
                        }
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
                    case "seleccionar_buscar_agenda":
                        if (SelectedBuscarAgenda==null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "SELECCIONAR UNA CITA ES REQUERIDO!");
                            return;
                        }
                        SelectedEmpleadoValue=LstEmpleados.FirstOrDefault(w => w.Usuario.ID_USUARIO == SelectedBuscarAgenda.ID_USUARIO).ID_EMPLEADO;
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
                        var _incidencia = new ATENCION_CITA_INCIDENCIA {
                            ATENCION_ORIGINAL_FEC=fechaOriginal,
                            ID_ACMOTIVO=selectedIncidenteMotivoValue,
                            ID_USUARIO=GlobalVar.gUsr,
                            OBSERV=Observacion,
                            REGISTRO_FEC=_FechaServer,
                            ID_CENTRO_UBI=GlobalVar.gCentro
                        };
                        AgregarAgenda(_incidencia);
                        break;
                    case "cancelar_incidente":
                        setAgregarAgendaValidacionRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
                        break;
                }
        }

        private void resetIngreso()
        {
            SelectIngreso = selectIngresoAuxiliar;
            if (selectIngresoAuxiliar!=null)
            {
                if (tipoBusquedaImputado==ModoBusqueda.BUSQUEDA_AGENDA_IMPUTADO)
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
                else if (tipoBusquedaImputado==ModoBusqueda.BUSQUEDA_NORMAL_IMPUTADO)
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
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                        if (new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica((short)eAreas.AREA_MEDICA, GlobalVar.gUsr))
                            busquedaAgenda = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().ObtenerTodoPorImputado(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                                selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, fechaInicial, null, selectedBusquedaAgendaAtencionTipo.Value));
                        else
                            busquedaAgenda = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().ObtenerTodoPorImputado(GlobalVar.gCentro, selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                                selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, fechaInicial, null, selectedBusquedaAgendaAtencionTipo.Value, GlobalVar.gUsr.Trim()));
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
            catch(Exception ex)
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


                    if ((selectedAtencionTipoAgenda==(short)enumAtencionTipo.CONSULTA_MEDICA && selectIngreso.HISTORIA_CLINICA.Any(w=>w.ESTATUS=="T"))||
                        (selectedAtencionTipoAgenda==(short)enumAtencionTipo.CONSULTA_DENTAL && selectIngreso.HISTORIA_CLINICA_DENTAL.Any(w=>w.ESTATUS=="T")))
                    {
                        var _hc_tipo = string.Empty;
                        if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_MEDICA)
                            _hc_tipo = "historia clinica médica";
                        else if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_DENTAL)
                            _hc_tipo = "historia clinica dental";
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                            ListExpediente[0].ID_IMPUTADO.ToString() + "] ya cuenta con su " + _hc_tipo + " completa.");
                        resetIngreso();
                        return;
                    }

                    if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_DENTAL && !SelectIngreso.HISTORIA_CLINICA.Any(w => w.ESTATUS == "T"))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                        SelectIngreso.ID_IMPUTADO.ToString() + "] no cuenta con su historia clinica médica.");
                        resetIngreso();
                        return;
                    }

                    if ((selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_MEDICA && selectIngreso.ATENCION_CITA.Any(w =>w.ID_TIPO_ATENCION==(short)enumAtencionTipo.CONSULTA_MEDICA &&  w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_MEDICA)) ||
                    (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_DENTAL && selectIngreso.ATENCION_CITA.Any(w => w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_DENTAL)))
                    {
                        var _hc_tipo = string.Empty;
                        if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_MEDICA)
                            _hc_tipo = "historia clinica médica";
                        else if (selectedAtencionTipoAgenda == (short)enumAtencionTipo.CONSULTA_DENTAL)
                            _hc_tipo = "historia clinica dental";
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                            ListExpediente[0].ID_IMPUTADO.ToString() + "] ya cuenta con la cita para " + _hc_tipo + ".");
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
            catch(Exception ex)
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
        #endregion


        private async Task CargarAgenda()
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                _empleado = lstEmpleados.Any(w => w.ID_EMPLEADO == selectedEmpleadoValue) ? lstEmpleados.First(w => w.ID_EMPLEADO == selectedEmpleadoValue) : null;
                selectedAtencionTipoAgenda = selectedAtencionTipo.Value;
                headerAgenda = "AGENDA de " + _empleado.NOMBRE_COMPLETO + " - " + new cAtencionTipo().Obtener(selectedAtencionTipoAgenda.Value).DESCR ;
                RaisePropertyChanged("HeaderAgenda");
                var agenda = new cAtencionCita().ObtenerPorUsuarioyTipoAtencionMedica(GlobalVar.gCentro, _empleado.Usuario.ID_USUARIO, selectedAtencionTipoAgenda.Value,estatus_inactivos, new List<string> {"N","A"});
                if (agenda != null && agenda.Count()>0)
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
            if (fechasAgendadas!=null && fechasAgendadas.Count()>0)
            {
                fecha_minima = fechasAgendadas.Min();
                FechaInicial = fechasAgendadas.Min(); 
            }
            if (fecha_minima >= BusquedaFecha.Value)
                BusquedaFecha = fecha_minima;            
            FechaAgenda = BusquedaFecha.Value;
        }

        public async void OnModelChangedSwitch(object parametro)
        {
            try
            {
                if (parametro!=null)
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
                        case "cambio_empleado":
                            var _empleado_seleccionado = new cEmpleado().Obtener(selectedEmpleadoValue);
                            if (_empleado_seleccionado.USUARIO.First().USUARIO_ROL.Any(w => w.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_AREA_MEDICA))
                            {
                                IsAtencionTiposEnabled = true;
                                SelectedAtencionTipo = -1;
                            }
                            else if (_empleado_seleccionado.USUARIO.First().USUARIO_ROL.Any(w => w.ID_ROL == (short)enumRolesAreasTecnicas.MEDICO))
                            {
                                IsAtencionTiposEnabled = false;
                                if (LstAtencionTipos.Any(w => w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA))
                                    SelectedAtencionTipo = LstAtencionTipos.FirstOrDefault(w => w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA).ID_TIPO_ATENCION;
                                else
                                    SelectedAtencionTipo = -1;
                            }
                            else if (_empleado_seleccionado.USUARIO.First().USUARIO_ROL.Any(w => w.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA))
                            {
                                IsAtencionTiposEnabled = false;
                                if (LstAtencionTipos.Any(w => w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL))
                                    SelectedAtencionTipo = LstAtencionTipos.FirstOrDefault(w => w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL).ID_TIPO_ATENCION;
                                else
                                    SelectedAtencionTipo = -1;
                            }
                            break;
                        #region Cambio SelectedItem de Busqueda de Expediente
                        case "cambio_expediente":
                            if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                            {
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                                {
                                    selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                                    RaisePropertyChanged("SelectExpediente");
                                });
                                //MUESTRA LOS INGRESOS
                                if (selectExpediente.INGRESO != null && selectExpediente.INGRESO.Count > 0)
                                {
                                    EmptyIngresoVisible = false;
                                    SelectIngreso = selectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                }
                                else
                                {
                                    SelectIngreso = null;
                                    EmptyIngresoVisible = true;
                                }


                                //OBTENEMOS FOTO DE FRENTE
                                if (SelectIngreso != null)
                                {
                                    if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                        ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                    else
                                        ImagenImputado = new Imagenes().getImagenPerson();
                                    TextBotonSeleccionarIngreso = "aceptar";
                                    SelectIngresoEnabled = true;
                                    if (estatus_inactivos != null && estatus_inactivos.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO))
                                    {
                                        TextBotonSeleccionarIngreso = "seleccionar ingreso";
                                        SelectIngresoEnabled = false;
                                    }
                                }
                                else
                                    ImagenImputado = new Imagenes().getImagenPerson();
                            }
                            break;
                        #endregion
                    }
                }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error en el cambio de informacion", ex);
            }
        }

        public async void AppointmentClick (object parametro)
        {
            
            selectIngreso = null;
            if (parametro!=null)
            {
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                    return;
                }
                tipoAgregarAgenda = ModoAgregarAgenda.EDICION;
                var _fecha_servidor = Fechas.GetFechaDateServer;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                    selectedAtencion_Cita = new cAtencionCita().Obtener(((Appointment)parametro).ID_CITA.Value, GlobalVar.gCentro);
                    if (new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica((short)eAreas.AREA_MEDICA, GlobalVar.gUsr))
                    {
                        var _rol = selectedAtencion_Cita.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ? (short)enumRolesAreasTecnicas.MEDICO : (short)enumRolesAreasTecnicas.DENTISTA;
                        lstAgendaEmpleados = lstEmpleados.Where(w => w.Usuario.USUARIO_ROL.Any(a => a.ID_ROL == _rol)).ToList();
                        RaisePropertyChanged("LstAgendaEmpleados");
                        isAgendaEmpleadoEnabled = true;
                        RaisePropertyChanged("IsAgendaEmpleadoEnabled");
                    }
                    else
                    {
                        lstAgendaEmpleados = new List<cUsuarioExtendida>();
                        lstAgendaEmpleados.Add(lstEmpleados.FirstOrDefault(s => s.ID_EMPLEADO == selectedAtencion_Cita.ID_RESPONSABLE));
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
                    sexoImputadoAgregarAgenda = string.IsNullOrWhiteSpace(selectedAtencion_Cita.INGRESO.IMPUTADO.SEXO)?string.Empty: selectedAtencion_Cita.INGRESO.IMPUTADO.SEXO == "M" ? "Masculino" : "Femenino";
                    RaisePropertyChanged("SexoImputadoAgregarAgenda");
                    edadImputadoAgregarAgenda = selectedAtencion_Cita.INGRESO.IMPUTADO.NACIMIENTO_FECHA.HasValue?(short?)new Fechas().CalculaEdad(selectedAtencion_Cita.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value):null;
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
                });
                setAgregarAgendaValidacionRules();
                setReadOnlyDatosImputadosAgregarAgenda(true);
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
            }
        }

        private async void AgregarAgenda(ATENCION_CITA_INCIDENCIA _incidencia=null)
        {
            var _nueva_fecha_inicio = new DateTime(agregarAgendaFecha.Value.Year, agregarAgendaFecha.Value.Month, agregarAgendaFecha.Value.Day,
                                AgregarAgendaHoraI.Value.Hour, AgregarAgendaHoraI.Value.Minute, 0);
            var _nueva_fecha_final = new DateTime(agregarAgendaFecha.Value.Year, agregarAgendaFecha.Value.Month, agregarAgendaFecha.Value.Day,
                            AgregarAgendaHoraF.Value.Hour, AgregarAgendaHoraF.Value.Minute, 0);
            if (_nueva_fecha_inicio<Fechas.GetFechaDateServer)
            {
                new Dialogos().ConfirmacionDialogo("Validación!", "LA FECHA DE LA CITA NO PUEDE SER MENOR A LA FECHA Y HORA ACTUAL!");
                return;
            }
            if (tipoAgregarAgenda==ModoAgregarAgenda.EDICION)
            {
                var otra_cita = new cAtencionCita().ObtieneCitaOtraArea(selectedAtencion_Cita.INGRESO.ID_CENTRO, selectedAtencion_Cita.INGRESO.ID_ANIO,
                    selectedAtencion_Cita.INGRESO.ID_IMPUTADO, selectedAtencion_Cita.INGRESO.ID_INGRESO, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1), selectedAtencion_Cita.ID_CITA, selectedAtencion_Cita.ID_CENTRO_UBI);
                var _usuario_actualizar=lstAgendaEmpleados.First(w=>w.ID_EMPLEADO==selectedAgendaEmpleadoValue);
                if (otra_cita==null)
                {
                    if (!new cAtencionCita().IsOverlapCita(selectedAtencion_Cita.ID_TIPO_ATENCION.Value, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1), estatus_inactivos, selectedAtencion_Cita.ID_CITA)) // a la hora final hay que restarle un minuto para no traslapar horarios que comienzan terminan a la misma hora
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
                                ID_USUARIO=GlobalVar.gUsr,
                                ID_CENTRO_UBI=selectedAtencion_Cita.ID_CENTRO_UBI,
                                ID_CENTRO_AT_SOL=selectedAtencion_Cita.ID_CENTRO_AT_SOL
                            };
                            new cAtencionCita().Actualizar(_atencion_cita, _FechaServer ,_incidencia,  (short)enumMensajeTipo.INCIDENCIA_REPROGRAMACIÓN_ATENCIÓN_MÉDICA);
                            return true;
                        }))
                        {
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La cita fue actualizada con exito");
                            if (PopUpsViewModels.VisibleAgregarIncidenciaAtencionCita==Visibility.Visible)
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.INCIDENCIA_ATENCION_CITA);
                            else
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
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
                var otra_cita = new cAtencionCita().ObtieneCitaOtraArea(selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO,
                    selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1));
                if (otra_cita == null)
                {
                    if (!new cAtencionCita().IsOverlapCita(selectedAtencionTipo.Value, _nueva_fecha_inicio, _nueva_fecha_final.AddMinutes(-1), estatus_inactivos)) // a la hora final hay que restarle un minuto para no traslapar horarios que comienzan terminan a la misma hora
                    {
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando cita", () =>
                        {
                            var _atencion_cita = new ATENCION_CITA  
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
                                ID_TIPO_ATENCION = selectedAtencionTipoAgenda.Value,
                                ID_TIPO_SERVICIO = selectedAtencionTipoAgenda.Value == (short)enumAtencionTipo.CONSULTA_MEDICA ? (short)enumAtencionServicio.HISTORIA_CLINICA_MEDICA : (short)enumAtencionServicio.HISTORIA_CLINICA_DENTAL,
                                ID_RESPONSABLE=_empleado.ID_EMPLEADO,
                                ID_USUARIO=GlobalVar.gUsr,
                                ID_CENTRO_UBI=GlobalVar.gCentro,
                                ID_CENTRO_AT_SOL=null
                            };
                            new cAtencionCita().Agregar(_atencion_cita);
                            return true;
                        }))
                        {
                            new Dialogos().ConfirmacionDialogo("EXITO!", "La cita fue actualizada con exito");
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_AGENDA_MEDICA);
                            setBuscarAgendaValidacionRules();
                            selectIngresoAuxiliar = null;
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
        }

        private void setReadOnlyDatosImputadosAgregarAgenda(bool isReadOnly)
        {
            IsReadOnlyAnioImputadoAgregarAgenda = isReadOnly;
            IsReadOnlyApMaternoAgregarAgenda = isReadOnly;
            IsReadOnlyApPaternoAgregarAgenda = isReadOnly;
            IsReadOnlyFolioImputadoAgregarAgenda = isReadOnly;
            IsReadOnlyNombreAgregarAgenda = isReadOnly;
        }

        #region Cargar Catalogos
        private void CargarCatalogos(bool isExceptionManaged = false)
        {
            CargarEmpleados((short)eAreas.AREA_MEDICA, isExceptionManaged);
            CargarAtencionTipo(isExceptionManaged);
            CargarAreas(isExceptionManaged);
            CargarBusquedaAgendaAtencionTipo(isExceptionManaged);
        }

        private void CargarAtencionTipo(bool isExceptionManaged = false)
        {
            try
            {
                lstAtencionTipos = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w=>w.ESTATUS=="S"));
                lstAtencionTipos.Insert(0, new ATENCION_TIPO
                {
                    ID_TIPO_ATENCION = -1,
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

        private void CargarBusquedaAgendaAtencionTipo(bool isExceptionManaged = false)
        {
            try
            {
                lstBusquedaAgendaAtencionTipos = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                lstBusquedaAgendaAtencionTipos.Insert(0, new ATENCION_TIPO
                {
                    ID_TIPO_ATENCION = -1,
                    DESCR = "SELECCIONE"
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

        public void CargarEmpleados(short area_tecnica,bool isExceptionManaged=false)
        {
            try
            {
                if (new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica(area_tecnica, GlobalVar.gUsr))
                {
                    lstEmpleados = new SSP.Controlador.Catalogo.Justicia.cUsuario().ObtenerTodosporAreaTecnica(GlobalVar.gCentro, area_tecnica, new List<short> {(short)enumRolesAreasTecnicas.MEDICO, (short)enumRolesAreasTecnicas.DENTISTA }).Select(s => new cUsuarioExtendida
                    {
                        Usuario = s,
                        NOMBRE_COMPLETO = s.EMPLEADO.PERSONA.NOMBRE.Trim() + (s.EMPLEADO.PERSONA.PATERNO == null || s.EMPLEADO.PERSONA.PATERNO.Trim() == "" ? "" : " " + s.EMPLEADO.PERSONA.PATERNO.Trim()) +
                           (s.EMPLEADO.PERSONA.MATERNO == null || s.EMPLEADO.PERSONA.MATERNO.Trim() == "" ? "" : " " + s.EMPLEADO.PERSONA.MATERNO.Trim()),
                        ID_EMPLEADO = s.EMPLEADO.PERSONA.EMPLEADO.ID_EMPLEADO
                    }).ToList();
                    var _usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().Obtener(StaticSourcesViewModel.UsuarioLogin.Username);
                    if (!lstEmpleados.Any(w => w.Usuario.ID_USUARIO == _usuario.ID_USUARIO))
                        lstEmpleados.Insert(0, new cUsuarioExtendida
                        {
                            Usuario = _usuario,
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
            catch(Exception ex)
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
                permisos = new cProcesoUsuario().Obtener(enumProcesos.AGENDAMEDICA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
    }
}
