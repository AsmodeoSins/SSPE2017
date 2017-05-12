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
using ControlPenales.BiometricoServiceReference;
using Cogent.Biometrics;
using System.Windows.Media;
using DPUruNet;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Media.Animation;
using SSP.Controlador.Catalogo.Justicia.Ingreso;

namespace ControlPenales
{
    partial class AtencionCitaListaViewModel : FingerPrintScanner
    {
        #region Constructor
        public AtencionCitaListaViewModel() { }
        #endregion

        #region Funciones Eventos
        private async void WindowLoad(AtencionCitaListaView obj = null)
        {
            try
            {
                ConfiguraPermisos();
                estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                if (PConsultar)
                { 
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(ObtenerListado); 
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listado.", ex);
            }
        }

        private async void clickSwitch(Object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "atender_cita":
                        if (!PConsultar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }
                        if (SelectedAtencionCita != null)
                        {
                            #region Validar Ubicacion Interno
                            if (SelectedAtencionCita != null)
                            {
                                var ubicacion = SelectedAtencionCita.INGRESO.INGRESO_UBICACION.OrderByDescending(w => w.ID_CONSEC).FirstOrDefault();
                                if (ubicacion != null)
                                {
                                    if (ubicacion.ESTATUS == 0)
                                    {
                                        new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado se encuentra aun en su celda");
                                        break;
                                    }
                                }
                                else
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado se encuentra aun en su celda");
                                    break;
                                }
                            }
                            #endregion
                            SelectExpediente = SelectedAtencionCita.INGRESO.IMPUTADO;
                            SelectIngreso = SelectedAtencionCita.INGRESO;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            HuellaWindow = new BuscarPorHuellaYNipView();
                            HuellaWindow.DataContext = this;
                            ConstructorHuella(0);
                            HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                            HuellaWindow.Closed += HuellaClosed;
                            HuellaWindow.ShowDialog();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new AtencionCitaView();
                            //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new AtencionCitaViewModel(SelectedAtencionCita);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar la cita a atender.");
                        break;
                    case "reagendar_cita":
                        if (SelectedAtencionCita != null)
                        {

                            SelectedSolicitud = new cSolicitudCita() { ATENCION_INGRESO = SelectedAtencionCita.ATENCION_SOLICITUD.ATENCION_INGRESO.FirstOrDefault() };
                            Reagendar();
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar la cita a reagendar.");
                        break;
                    case "buscar_nip":
                        ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                        if (string.IsNullOrEmpty(TextNipBusqueda)) return;
                        HuellaWindow.Hide();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                //var tipo = (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO;
                                var auxiliar = new List<ResultadoBusquedaBiometrico>();
                                ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();

                                if (SelectExpediente.NIP.Trim() == TextNipBusqueda.Trim())
                                {
                                    var ingresobiometrico = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                    var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                                    if (ingresobiometrico != null)
                                        if (ingresobiometrico.INGRESO_BIOMETRICO.Any())
                                            if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                            else
                                                if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                    FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                    auxiliar.Add(new ResultadoBusquedaBiometrico
                                    {
                                        AMaterno = SelectExpediente.MATERNO.Trim(),
                                        APaterno = SelectExpediente.PATERNO.Trim(),
                                        Expediente = SelectExpediente.ID_ANIO + "/" + SelectExpediente.ID_IMPUTADO,
                                        Foto = FotoBusquedaHuella,
                                        Imputado = SelectExpediente,
                                        NIP = !string.IsNullOrEmpty(SelectExpediente.NIP) ? SelectExpediente.NIP : string.Empty,
                                        Nombre = SelectExpediente.NOMBRE.Trim(),
                                        Persona = null
                                    });
                                }
                                ListResultado = auxiliar.Any() ? auxiliar.ToList() : new List<ResultadoBusquedaBiometrico>();
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar realizar la busqueda por nip.", ex);
                            }
                        });
                        HuellaWindow.Show();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        break;
                    case "aceptar_huella":
                        try
                        {
                            HuellaWindow.Hide();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            if (ScannerMessage.Contains("Procesando..."))
                                return;
                            CancelKeepSearching = true;
                            isKeepSearching = true;
                            //await WaitForFingerPrints();
                            _IsSucceed = true;
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(async () =>
                            {
                                try
                                {
                                    if (SelectRegistro == null)
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(async delegate
                                        {
                                            var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "Debes seleccionar un imputado.");
                                            //new Dialogos().ConfirmacionDialogo("Notificación!", "Debes seleccionar un imputado.");
                                            HuellaWindow.Show();
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                        }));
                                        return;
                                    }
                                    SelectExpediente = SelectRegistro.Imputado;
                                    if (SelectExpediente.INGRESO.Count == 0)
                                    {
                                        SelectExpediente = null;
                                        SelectIngreso = null;
                                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                        //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                    }
                                    if (Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(a => a.HasValue ? a.Value == SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO : false))
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(async delegate
                                        {
                                            var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                                            HuellaWindow.Show();
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                            //new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                                        }));
                                        return;
                                    }
                                    if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO.HasValue ?
                                        SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro : false)
                                    {
                                        SelectExpediente = null;
                                        SelectIngreso = null;
                                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                        //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                        Application.Current.Dispatcher.Invoke((Action)(async delegate
                                        {
                                            var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "El ingreso seleccionado no pertenece a su centro.");
                                            HuellaWindow.Show();
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                            //new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                                        }));
                                        return;
                                    }

                                        Application.Current.Dispatcher.Invoke((Action)(async delegate
                                        {
                                            if ((new cIngresoUbicacion().CambiarEstatus(SelectedAtencionCita.ID_CENTRO.Value, SelectedAtencionCita.ID_ANIO.Value, SelectedAtencionCita.ID_IMPUTADO.Value, SelectedAtencionCita.ID_INGRESO.Value)))
                                            {
                                                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new AtencionCitaView();
                                                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new AtencionCitaViewModel(SelectedAtencionCita);
                                            }
                                            else
                                                new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al actualizar la ubicación del interno");
                                         
                                        }));

                                    ElementosDisponibles = true;
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        HuellaWindow.Close();
                                        MenuBuscarEnabled = false;
                                        MenuGuardarEnabled = true;
                                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                    }));
                                    StaticSourcesViewModel.SourceChanged = false;
                                }
                                catch (Exception ex)
                                {
                                    HuellaWindow.Show();
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            HuellaWindow.Show();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
                        }
                        break;
                    case "limpiar_menu":
                        StaticSourcesViewModel.SourceChanged = false;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new AtencionCitaListaView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new AtencionCitaListaViewModel();
                        break;
                    case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                        break;

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar opción", ex);
            }
        }

        private async void SeleccionaInterno(Object obj = null)
        {
            try
            {
                if (obj != null)
                {
                    if (obj is ATENCION_CITA)
                    {
                        SelectedAtencionCita = (ATENCION_CITA)obj;
                        if (SelectedAtencionCita != null)
                        {
                            #region Validar Ubicacion Interno
                            if (SelectedAtencionCita != null)
                            {
                                var ubicacion = SelectedAtencionCita.INGRESO.INGRESO_UBICACION.OrderByDescending(w => w.ID_CONSEC).FirstOrDefault();
                                if (ubicacion != null)
                                {
                                    if (ubicacion.ESTATUS == 0)
                                    {

                                        new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado se encuentra aun en su celda");
                                        return;
                                    }
                                }
                            }
                            #endregion

                            SelectExpediente = SelectedAtencionCita.INGRESO.IMPUTADO;
                            SelectIngreso = SelectedAtencionCita.INGRESO;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            HuellaWindow = new BuscarPorHuellaYNipView();
                            HuellaWindow.DataContext = this;
                            ConstructorHuella(0);
                            HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                            HuellaWindow.Closed += HuellaClosed;
                            HuellaWindow.ShowDialog();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar la cita a atender.");
                    }
                    #region comentado
                    //if (obj is ATENCION_CITA)
                    //{
                    //    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new AtencionCitaView();
                    //    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new AtencionCitaViewModel((ATENCION_CITA)obj);
                    //}
                    #endregion
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listado.", ex);
            }
        }
        #endregion

        #region Reagendar
        private void Reagendar() 
        {
            SelectedDateCalendar = Fechas.GetFechaDateServer.Date;
            if (LstAgenda == null)
                LstAgenda = new ObservableCollection<Appointment>();
            AgendaView = new AgendarCitaView();
            AgendaView.Loaded += AgendaLoaded;
            AgendaView.Agenda.AddAppointment += AgendaAddAppointment;
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

        private void AgendaAddAppointment(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is Calendar))
                    return;
                AgregarHora = !AgregarHora;
                if (AgregarHora)
                {
                    //ValidacionSolicitud();
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

        private void AgendaClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!base.HasErrors)
                {
                    var hoy = Fechas.GetFechaDateServer;
                    var _nueva_fecha_inicio = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraI.Value.Hour, AHoraI.Value.Minute, AHoraI.Value.Second);
                    var _nueva_fecha_final = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraF.Value.Hour, AHoraF.Value.Minute, AHoraF.Value.Second);
                    if (hoy > _nueva_fecha_inicio)
                    {
                        MensajeError = "La fecha y hora deben de ser mayor a la actual";
                        return;
                    }

                    if (AHoraI.Value.Minute % 15 != 0 || AHoraF.Value.Minute % 15 != 0)
                    {
                        MensajeError = "Los intervalos de atención tienen que ser en bloques de 15 minutos";
                        return;
                    }

                    Guardar();
                    //if (GuardarAgenda())
                    //{
                    //    if (AFecha != null)
                    //    {
                    //        LstAgenda.Add(new Appointment()
                    //        {
                    //            Subject = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectedAtencionCita.INGRESO.IMPUTADO.NOMBRE) ? SelectedAtencionCita.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedAtencionCita.INGRESO.IMPUTADO.PATERNO) ? SelectedAtencionCita.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedAtencionCita.INGRESO.IMPUTADO.MATERNO) ? SelectedAtencionCita.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty),
                    //            StartTime = _nueva_fecha_inicio,
                    //            EndTime = _nueva_fecha_final,
                    //        });
                    //        AgendaView.Agenda.FilterAppointments();
                    //        AgregarHora = !AgregarHora;
                    //        AgendaView.Close();
                    //        base.ClearRules();
                    //    }
                    //}
                    //else
                    //    MensajeError = "Ocurrio un problema al guardar la fecha";
                }
                else
                {
                    if (AHoraI.HasValue && AHoraF.HasValue && !AHorasValid)
                        MensajeError = "La hora fin debe ser mayor a la hora inicio de la cita";
                    else
                        MensajeError = string.Format("Faltan datos por capturar: {0}.", base.Error);
                };

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la agenda.", ex);
            }
        }

        private void AgendaClosing(object sender, EventArgs e)
        {
            try
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                //ObtenerSolicitudes();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la agenda.", ex);
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

        private async void Guardar() {
            try
            {
                var ac = new ATENCION_CITA();
                ac.ID_CITA = SelectedAtencionCita.ID_CITA;
                ac.CITA_FECHA_HORA = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraI.Value.Hour, AHoraI.Value.Minute, 0);
                ac.CITA_HORA_TERMINA = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraF.Value.Hour, AHoraF.Value.Minute, 0);
                if (new cAtencionCita().ReagendarCita(ac))
                {
                    AgregarHora = !AgregarHora;
                    base.ClearRules();
                    AgendaView.Close();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(ObtenerListado);
                }
                else
                {
                    MensajeError = "Ocurrio un problema al guardar la fecha";
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al reagendar cita.", ex);
            }
        }
        #endregion

        #region Empleado
        public async Task CargarEmpleados()
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                if (new cTecnicaArea().IsUsuarioCoordinadorAreaTecnica(selectedAtencionCita.ATENCION_SOLICITUD.ID_TECNICA.Value, GlobalVar.gUsr))
                {
                    lstEmpleados = new SSP.Controlador.Catalogo.Justicia.cUsuario().ObtenerTodosporAreaTecnica(GlobalVar.gCentro, selectedAtencionCita.ATENCION_SOLICITUD.ID_TECNICA.Value).Select(s => new cUsuarioExtendida
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
                    ObtenerAgenda(_usuario.ID_USUARIO, true);
                }
            });
        }

        private void ObtenerAgenda(string usuario, bool isExceptionManaged = false)
        {
            try
            {
                if (SelectedAtencionCita != null)
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

        private bool GuardarAgenda()
        {
            try
            {
                //if (se != null)
                //{
                //    if (!base.HasErrors)
                //    {
                //        var obj = new ATENCION_CITA();
                //        obj.ID_CITA = 0;
                //        obj.ESTATUS = "N";
                //        obj.ID_CENTRO = SelectedSolicitud.ATENCION_INGRESO.ID_CENTRO;
                //        obj.ID_ANIO = SelectedSolicitud.ATENCION_INGRESO.ID_ANIO;
                //        obj.ID_IMPUTADO = SelectedSolicitud.ATENCION_INGRESO.ID_IMPUTADO;
                //        obj.ID_INGRESO = SelectedSolicitud.ATENCION_INGRESO.ID_INGRESO;
                //        obj.ID_ATENCION = SelectedSolicitud.ATENCION_INGRESO.ID_ATENCION;
                //        obj.CITA_FECHA_HORA = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraI.Value.Hour, AHoraI.Value.Minute, 0);
                //        obj.CITA_HORA_TERMINA = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraF.Value.Hour, AHoraF.Value.Minute, 0);
                //        obj.ID_TIPO_ATENCION = SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TIPO_ATENCION;
                //        obj.ID_AREA = SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_AREA;
                //        obj.ID_TIPO_SERVICIO = SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TECNICA == (short)eAreas.AREA_MEDICA ? SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                //            (short?)enumAtencionServicio.CITA_MEDICA : SelectedSolicitud.ATENCION_INGRESO.ATENCION_SOLICITUD.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? (short?)enumAtencionServicio.CITA_MEDICA_DENTAL : null : null;
                //        obj.ID_USUARIO = _empleado.Usuario.ID_USUARIO.Trim();
                //        obj.ID_CITA = new cAtencionCita().Agregar(obj);
                //        if (obj.ID_CITA > 0)
                //        {
                //            ActualizaEstatusSolicitud((short)enumSolicitudCita.AGENDADA);
                //            return true;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar agenda.", ex);
            }
            return false;
        }
        #endregion

        #region Listado
        private void ObtenerListado() {
            try 
            {
                LstAtencionCita = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().ObtenerCitasPorUsuario(estatus_inactivos,GlobalVar.gUsr.Trim(),Fechas.GetFechaDateServer.Date));
                ListaVaciaVisible = LstAtencionCita.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener lista de internos.", ex);
            }
        }
        #endregion

        #region BUSCAR POR HUELLA
        private void LimpiarCampos()
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ScannerMessage = "Capture Huella";
                ColorMessage = new SolidColorBrush(Colors.Green);
                AceptarBusquedaHuellaFocus = true;
            }));
            _SelectRegistro = null;
            PropertyImage = null;
        }

        private void OnLoad(BuscarPorHuellaYNipView Window)
        {
            try
            {
                BuscarPor = enumTipoPersona.IMPUTADO;
                ListResultado = null;
                PropertyImage = null;
                FotoRegistro = new Imagenes().getImagenPerson();
                TextNipBusqueda = string.Empty;

                #region [Huellas Digitales]
                var myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 0;
                myDoubleAnimation.To = 185;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.3));
                myDoubleAnimation.AutoReverse = true;
                myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

                Storyboard.SetTargetName(myDoubleAnimation, "Ln");
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
                var myStoryboard = new Storyboard();
                myStoryboard.Children.Add(myDoubleAnimation);
                myStoryboard.Begin(Window.Ln);
                #endregion

                Window.Closed += (s, e) =>
                {
                    try
                    {
                        if (OnProgress == null)
                            return;

                        if (!_IsSucceed)
                            SelectRegistro = null;

                        OnProgress.Abort();
                        CancelCaptureAndCloseReader(OnCaptured);
                        //if (AtencionSeleccionada)
                        //{
                        //    var aux = SelectAtencionMedicaAux;
                        //    SelectAtencionMedica = aux;
                        //}
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar busqueda", ex);
                    }
                };

                if (CurrentReader != null)
                {
                    CurrentReader.Dispose();
                    CurrentReader = null;
                }

                CurrentReader = Readers[0];

                if (CurrentReader == null)
                    return;

                if (!OpenReader())
                    Window.Close();

                if (!StartCaptureAsync(OnCaptured))
                    Window.Close();

                OnProgress = new Thread(() => InvokeDelegate(Window));

                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Capture Huella";
                    ColorMessage = new SolidColorBrush(Colors.Green);
                }));
                GuardandoHuellas = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de la busqueda por huella.", ex);
            }
        }

        private async void Aceptar(Window Window)
        {
            try
            {
                if (ScannerMessage.Contains("Procesando..."))
                    return;
                CancelKeepSearching = true;
                isKeepSearching = true;
                await WaitForFingerPrints();
                _IsSucceed = true;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    try
                    {
                        if (SelectRegistro == null) return;
                        SelectExpediente = SelectRegistro.Imputado;
                        if (SelectExpediente.INGRESO.Count == 0)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        }
                        if (Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(a => a.HasValue ? a.Value == SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO : false))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                            }));
                            return;
                        }
                        if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO.HasValue ?
                            SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro : false)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            }));
                            return;
                        }
                        SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        //NotaMedicaVisible = Visibility.Visible;
                        //ListaImputadosVisible = Visibility.Collapsed;
                        //SeleccionarImputado();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
                    }
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }

        public async override void OnCaptured(DPUruNet.CaptureResult captureResult)
        {
            try
            {
                if (ScannerMessage.Contains("Procesando..."))
                    return;

                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    TextNipBusqueda = string.Empty;
                    PropertyImage = new BitmapImage();
                    ShowLoading = Visibility.Visible;
                    //ShowLine = Visibility.Visible;
                    ShowCapturar = Visibility.Collapsed;
                    ShowLine = Visibility.Visible;
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                }));

                //await TaskEx.Delay(500);


                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    base.OnCaptured(captureResult);
                }));
                ListResultado = null;

                switch (BuscarPor)
                {
                    case enumTipoPersona.IMPUTADO:
                        await CompareImputado();
                        break;
                    case enumTipoPersona.PERSONA_TODOS:
                    case enumTipoPersona.PERSONA_VISITA:
                    case enumTipoPersona.PERSONA_ABOGADO:
                    case enumTipoPersona.PERSONA_EMPLEADO:
                    case enumTipoPersona.PERSONA_EXTERNA:
                        //ComparePersona();
                        break;
                    default:
                        break;
                }

                GuardandoHuellas = true;
                ShowLoading = Visibility.Collapsed;
                ShowCapturar = Conectado ? Visibility.Visible : Visibility.Collapsed;
                ShowLine = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }
        }

        private async void Capture(string obj)
        {
            try
            {
                ShowLoading = Visibility.Visible;
                ShowLine = Visibility.Visible;
                var nRet = -1;
                try
                {
                    CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                    nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);
                    ShowCapturar = Visibility.Collapsed;

                    #region [Huellas]
                    if (nRet == 0)
                    {
                        ScannerMessage = "Procesando...";
                        ShowLine = Visibility.Visible;
                        ListResultado = null;
                        HuellasCapturadas = new List<PlantillaBiometrico>();

                        for (short i = 1; i <= 10; i++)
                        {
                            var pBuffer = IntPtr.Zero;
                            var nBufferLength = 0;
                            var nNFIQ = 0;
                            ListResultado = null;
                            GuardandoHuellas = false;

                            CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                            var bufferBMP = new byte[nBufferLength];
                            if (pBuffer != IntPtr.Zero)
                                Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);

                            CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                            var bufferWSQ = new byte[nBufferLength];
                            if (pBuffer != IntPtr.Zero)
                                Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);

                            CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);

                            Fmd FMD = null;
                            if (bufferBMP.Length != 0)
                            {
                                PropertyImage = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                            }

                            ShowContinuar = Visibility.Collapsed;
                            await TaskEx.Delay(1);

                            switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                            {
                                #region [Pulgar Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO);
                                    break;
                                #endregion
                                #region [Indice Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_DERECHO);
                                    break;
                                #endregion
                                #region [Medio Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_DERECHO);
                                    break;
                                #endregion
                                #region [Anular Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_DERECHO);
                                    break;
                                #endregion
                                #region [Meñique Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_DERECHO);
                                    break;
                                #endregion
                                #region [Pulgar Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.PULGAR_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO);
                                    break;
                                #endregion
                                #region [Indice Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_IZQUIERDO);
                                    break;
                                #endregion
                                #region [Medio Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_IZQUIERDO);
                                    break;
                                #endregion
                                #region [Anular Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_IZQUIERDO);
                                    break;
                                #endregion
                                #region [Meñique Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_IZQUIERDO);
                                    isKeepSearching = true;
                                    break;
                                #endregion
                                default:
                                    break;
                            }

                            ShowContinuar = Visibility.Visible;
                            ShowCapturar = Visibility.Collapsed;

                            if (!CancelKeepSearching)
                                await KeepSearch();
                            else
                                if (!_GuardarHuellas)
                                    break;
                        }

                        GuardandoHuellas = true;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            ScannerMessage = "Vuelve a capturar las huellas";
                            ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        }));
                    }
                    #endregion
                }
                catch
                {
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                }

                if (nRet == 0)
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Busqueda Terminada";
                        ColorMessage = new SolidColorBrush(Colors.Green);
                        AceptarBusquedaHuellaFocus = true;
                    }));

                ShowLine = Visibility.Collapsed;
                ShowLoading = Visibility.Collapsed;
                ShowContinuar = Visibility.Collapsed;
                await TaskEx.Delay(1500);
                ShowCapturar = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }

        }

        private async Task WaitForFingerPrints()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!GuardandoHuellas) ;
            });
        }

        private Task<bool> CompareImputado(byte[] Huella = null, enumTipoBiometrico? Finger = null)
        {
            try
            {
                var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null ?? Huella;
                var verifyFinger = Finger ?? (DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO);
                if (bytesHuella == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = Finger == null ? "Vuelve a capturar las huellas" : ScannerMessage = "Siguiente Huella";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        ShowLine = Visibility.Collapsed;
                    }));
                }
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                    AceptarBusquedaHuellaFocus = false;
                }));
                var Service = new BiometricoServiceClient();
                if (Service == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Error en el servicio de comparacion";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        ShowLine = Visibility.Collapsed;
                    }));
                }
                var list = SelectExpediente.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)verifyFinger && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 && w.BIOMETRICO != null).AsEnumerable()
                    .Select(s => new
                    {
                        IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data
                    }).ToList();
                var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, list.Where(w => w.FMD != null).Select(s => s.FMD), (0x7fffffff / 100000), 10);
                var identify = true;
                identify = doIdentify.ResultCode == Constants.ResultCode.DP_SUCCESS ? doIdentify.Indexes.Count() > 0 : false;
                if (identify)
                {
                    ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>(); var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                    if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Any())
                        if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO &&
                            w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Where(w =>
                                w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                        else
                            if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO &&
                                w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Where(w =>
                                    w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                    ListResultado.Add(new ResultadoBusquedaBiometrico
                    {
                        AMaterno = SelectExpediente.MATERNO,
                        APaterno = SelectExpediente.PATERNO,
                        Nombre = SelectExpediente.PATERNO,
                        Expediente = SelectExpediente.ID_ANIO + " / " + SelectExpediente.ID_IMPUTADO,
                        Foto = FotoBusquedaHuella,
                        Imputado = SelectExpediente,
                        NIP = SelectExpediente.NIP
                    });
                    ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);
                    SelectRegistro = ListResultado.FirstOrDefault();
                    ShowContinuar = Visibility.Collapsed;
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (!CancelKeepSearching)
                        {
                            ScannerMessage = "Huella empatada";
                            AceptarBusquedaHuellaFocus = true;
                            ColorMessage = new SolidColorBrush(Colors.Green);
                        }
                    }));
                    if (Finger != null)
                        Service.Close();
                    return TaskEx.FromResult(false);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (!CancelKeepSearching)
                        {
                            ScannerMessage = "Huella no concuerda";
                            ColorMessage = new SolidColorBrush(Colors.Red);
                            AceptarBusquedaHuellaFocus = true;
                        }
                    }));
                    _IsSucceed = false;
                    if (!CancelKeepSearching)
                        _SelectRegistro = null;
                    PropertyImage = null;
                }
                Service.Close();
                FingerPrintData = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }
            return TaskEx.FromResult(true);
        }

        private Task<bool> ComparePersona(byte[] Huella = null, enumTipoBiometrico? Finger = null)
        {
            try
            {
                var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null ?? Huella;
                var verifyFinger = Finger ?? (DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO);

                if (bytesHuella == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (Finger == null)
                            ScannerMessage = "Vuelve a capturar las huellas";
                        else
                            ScannerMessage = "Siguiente Huella";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        ShowLine = Visibility.Collapsed;
                    }));
                }
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                    AceptarBusquedaHuellaFocus = false;
                }));

                var Service = new BiometricoServiceClient();
                if (Service == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Error en el servicio de comparacion";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        ShowLine = Visibility.Collapsed;
                    }));
                }

                var CompareResult = Service.CompararHuellaPersona(new ComparationRequest
                {
                    BIOMETRICO = bytesHuella,
                    ID_TIPO_BIOMETRICO = verifyFinger,
                    ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP,
                    ID_TIPO_PERSONA = BuscarPor == enumTipoPersona.PERSONA_TODOS ? new Nullable<enumTipoPersona>() : BuscarPor
                });

                if (CompareResult.Identify)
                {
                    ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();

                    foreach (var item in CompareResult.Result)
                    {
                        var persona = new cPersonaBiometrico().GetData().Where(w => w.ID_PERSONA == item.ID_PERSONA && (w.ID_TIPO_BIOMETRICO == (DD_Dedo.HasValue ? (short)DD_Dedo.Value : (short)enumTipoBiometrico.INDICE_DERECHO)) && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP).FirstOrDefault();

                        ShowContinuar = Visibility.Collapsed;
                        if (persona == null)
                            continue;

                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            var perosonabiometrico = new cPersonaBiometrico().GetData().Where(w => w.ID_PERSONA == persona.ID_PERSONA).ToList();
                            var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());

                            if (perosonabiometrico != null)
                                if (perosonabiometrico.Any())
                                    if (perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                        FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                    else
                                        if (perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                            FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);

                            ListResultado.Add(new ResultadoBusquedaBiometrico()
                            {
                                Nombre = persona.PERSONA.NOMBRE,
                                APaterno = persona.PERSONA.PATERNO,
                                AMaterno = persona.PERSONA.MATERNO,
                                Expediente = persona.PERSONA.ID_PERSONA.ToString(),
                                NIP = persona.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).Any() ?
                                    persona.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().NIP.HasValue
                                        ? persona.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().NIP.Value.ToString()
                                            : string.Empty : string.Empty,
                                Foto = FotoBusquedaHuella,
                                Persona = persona.PERSONA
                            });

                        }));
                    }

                    ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);

                    ShowContinuar = Visibility.Collapsed;

                    if (ListResultado.Any())
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            if (!CancelKeepSearching)
                            {
                                ScannerMessage = "Registro encontrado";
                                AceptarBusquedaHuellaFocus = true;
                                ColorMessage = new SolidColorBrush(Colors.Green);
                            }
                        }));

                        if (Finger != null)
                            Service.Close();

                        return TaskEx.FromResult(false);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            if (!CancelKeepSearching)
                            {
                                ScannerMessage = "Registro no encontrado";
                                AceptarBusquedaHuellaFocus = true;
                                ColorMessage = new SolidColorBrush(Colors.Red);
                            }
                        }));
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (!CancelKeepSearching)
                        {
                            ScannerMessage = "Huella no encontrada";
                            ColorMessage = new SolidColorBrush(Colors.Red);
                            AceptarBusquedaHuellaFocus = true;
                        }
                    }));
                    _IsSucceed = false;
                    if (!CancelKeepSearching)
                    {
                        _SelectRegistro = null;
                    }
                    PropertyImage = null;
                }

                Service.Close();
                FingerPrintData = null;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }
            return TaskEx.FromResult(true);
        }

        private async Task KeepSearch()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!isKeepSearching) ;
            });
            isKeepSearching = false;
        }

        private void ConstructorHuella(enumTipoPersona tipobusqueda, bool? set442 = null, bool GuardarHuellas = false)
        {
            try
            {
                BuscarPor = tipobusqueda;
                Conectado = set442.HasValue ? set442.Value : false;
                ShowCapturar = set442.HasValue ? set442.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                _GuardarHuellas = GuardarHuellas;
                switch (tipobusqueda)
                {
                    case enumTipoPersona.IMPUTADO:
                        CabeceraBusqueda = "Datos del Imputado";
                        CabeceraFoto = "Foto Imputado";
                        break;
                    case enumTipoPersona.PERSONA_VISITA:
                    case enumTipoPersona.PERSONA_EXTERNA:
                        CabeceraBusqueda = "Datos de la Persona";
                        CabeceraFoto = "Foto Persona";
                        break;
                    case enumTipoPersona.PERSONA_ABOGADO:
                        CabeceraBusqueda = "Datos del Abogado";
                        CabeceraFoto = "Foto Abogado";
                        break;
                    case enumTipoPersona.PERSONA_EMPLEADO:
                        CabeceraBusqueda = "Datos del Empleado";
                        CabeceraFoto = "Foto Empleado";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la busqueda por huella.", ex);
            }
        }

        private async void OnBuscarPorHuella(string obj = "")
        {
            try
            {
                //if (!PConsultar)
                //{
                //    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                //    return;
                //}

                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
                await TaskEx.Delay(400);
                var nRet = -1;
                var bandera = true;
                var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaPadronVisita;
                if (requiereGuardarHuellas)
                    try
                    {
                        nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                    }
                    catch
                    {
                        bandera = false;
                    }
                else
                    bandera = false;

                var windowBusqueda = new BusquedaHuella();
                windowBusqueda.DataContext = this;
                ConstructorHuella(enumTipoPersona.IMPUTADO, nRet == 0, requiereGuardarHuellas);
                windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Imputado")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "IMPUTADO"
                });
                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }
                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Escape)windowBusqueda.Close(); };
                windowBusqueda.Closed += (s, e) =>
                {
                    HuellasCapturadas = ((NotaMedicaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    //var huella = ((NotaMedicaViewModel)windowBusqueda.DataContext);
                    if (!IsSucceed)
                        return;
                    if (SelectRegistro != null ? SelectRegistro.Imputado == null : null == null)
                        return;
                    //SelectPersona = huella.SelectRegistro.Persona;
                };
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }

        private void HuellaClosed(object sender, EventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    //BuscarReadOnly = false;
                    HuellaWindow.Closed -= HuellaClosed;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la ventana de busqueda.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.ATENCION_CITA_LISTA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
}
