using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Almacenes;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using MoreLinq;
using SSP.Controlador.Catalogo.Justicia.Medico;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace ControlPenales
{
    partial class NotaEvolucionViewModel : FingerPrintScanner
    {
        public NotaEvolucionViewModel() { }

        private void TimerStart(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void TimerStop(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private async void NotaMedicaLoad(NotaEvolucionView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    try
                    {
                        if (Window == null) return;
                        #region AutoCompletes
                        AutoCompleteTB = Window.AutoCompleteTB;
                        AutoCompleteLB = AutoCompleteTB.Template.FindName("PART_ListBox", Window.AutoCompleteTB) as ListBox;
                        AutoCompleteTB.PreviewMouseDown += new MouseButtonEventHandler(listBox_MouseUp);
                        AutoCompleteTB.KeyDown += listBox_KeyDown;
                        //AutoCompleteProcsMeds = Window.AutoCompleteTB_ProcsMeds;
                        //AutoCompleteProcsMedsLB = AutoCompleteProcsMeds.Template.FindName("PART_ListBox", Window.AutoCompleteTB_ProcsMeds) as ListBox;
                        //AutoCompleteProcsMeds.PreviewMouseDown += new MouseButtonEventHandler(Producto_ProcedimientoMedico_MouseUp);
                        //AutoCompleteProcsMeds.KeyDown += Producto_ProcedimientoMedico_KeyDown;
                        AutoCompleteReceta = Window.AutoCompleteReceta;
                        AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", Window.AutoCompleteReceta) as ListBox;
                        AutoCompleteReceta.PreviewMouseDown += new MouseButtonEventHandler(MouseUpReceta);
                        AutoCompleteReceta.KeyDown += KeyDownReceta;
                        #endregion
                        Window.Unloaded += TimerStop;
                        ValidaRol();
                        if (!IsMedico)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(async delegate
                            {
                                if (await new Dialogos().ConfirmacionDialogoReturn("Validación", "No cuenta con el rol adecuado para entrar a este módulo.") == 1)
                                {
                                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new BandejaEntradaView();
                                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
                                }
                            }));
                            return;
                        }
                        ConfiguraPermisos();
                        #region Parametros
                        ParametroNotaMedicaTipoServicio = Parametro.NOTA_MEDICA_TIPO_SERVICIO;
                        ParametroEstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        ParametroLogoEstado = Parametro.LOGO_ESTADO;
                        ParametroReporteLogo2 = Parametro.REPORTE_LOGO2;
                        ParametroCuerpoDorso = Parametro.CUERPO_DORSO;
                        ParametroCuerpoFrente = Parametro.CUERPO_FRENTE;
                        ParametroImagenZonaCorporal = Parametro.IMAGEN_ZONA_CORPORAL;
                        ParametroEstatusCitaSinAtender = Parametro.AT_CITA_ESTATUS_SIN_ATENDER;
                        ParametroEstatusCitaConcluido = Parametro.AT_CITA_ESTATUS_CONCLUIDO;
                        ParametroEstatusCitaAtendiendo = Parametro.AT_CITA_ESTATUS_ATENDIENDO;
                        ParametroSolicitudAtencionPorMes = Parametro.SOLICITUD_ATENCION_POR_MES;
                        ParametroGuardarHuellaEnBusquedaPadronVisita = Parametro.GuardarHuellaEnBusquedaPadronVisita;
                        #endregion
                        #region Consultas
                        ListMotivosCancelarProcMed = new ObservableCollection<ATENCION_CITA_IN_MOTIVO>(new cAtencion_Cita_In_Motivo().ObtenerTodos());
                        ListAtencionTipo = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                        idPersona = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault().ID_PERSONA;
                        var rol = new cUsuarioRol().GetData().First(x => x.ID_USUARIO.Contains(StaticSourcesViewModel.UsuarioLogin.Username));
                        if (!(IsDentista && IsMedico))
                        {
                            var lista = new cAtencionServicio().ObtenerTodo().Where(w => w.ESTATUS == "S" &&
                                rol.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA ? w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL : w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA);
                            foreach (var item in ParametroNotaMedicaTipoServicio)
                            {
                                var div = item.Split('-');
                                var atencion = short.Parse(div[0]);
                                var servicio = short.Parse(div[1]);
                                lista = lista.Where(w => w.ID_TIPO_ATENCION == atencion ? w.ID_TIPO_SERVICIO != servicio : true);
                            }
                            ListTipoServicio = new ObservableCollection<ATENCION_SERVICIO>(lista.OrderBy(o => o.DESCR));
                        }
                        CitasVisible = (IsEnfermero && !IsMedico) ? Visibility.Collapsed : Visibility.Visible;
                        CargarImputados();
                        ListMedicos = new ObservableCollection<EMPLEADO>(new cEmpleado().GetData(g => g.ID_CENTRO == GlobalVar.gCentro ?
                            g.ESTATUS == "S" ?
                                g.USUARIO.Any(f => f.ESTATUS == "S" ?
                                    f.USUARIO_ROL.Any(a => a.ID_ROL == (short)enumRolesAreasTecnicas.MEDICO)
                                : false)
                            : false
                        : false));
                        ListEnfermeros = new ObservableCollection<USUARIO>(new cEmpleado().GetData(g => g.ID_CENTRO == GlobalVar.gCentro ?
                            g.ESTATUS == "S" ?
                                g.USUARIO.Any(f => f.ESTATUS == "S" ?
                                    f.USUARIO_ROL.Any(a => a.ID_ROL == (short)enumRolesAreasTecnicas.ENFERMERO)
                                : false)
                            : false
                        : false).Select(s => s.USUARIO.FirstOrDefault(f => f.USUARIO_ROL.Any(a => a.ID_ROL == (short)enumRolesAreasTecnicas.ENFERMERO))));
                        LstEmpleados = new List<cUsuarioExtendida>();
                        LstEmpleados = new cEmpleado().GetData(g => g.ID_CENTRO == GlobalVar.gCentro ?
                            g.ESTATUS == "S" ?
                                g.USUARIO.Any(f => f.ESTATUS == "S" ?
                                    f.USUARIO_ROL.Any(a => a.ID_ROL == (short)enumRolesAreasTecnicas.ENFERMERO)
                                : false)
                            : false
                        : false).ToList().Select(s => new cUsuarioExtendida
                        {
                            ID_EMPLEADO = s.ID_EMPLEADO,
                            NOMBRE_COMPLETO = s.PERSONA.NOMBRE.Trim() + " " + s.PERSONA.PATERNO.Trim() + " " + s.PERSONA.MATERNO.Trim(),
                            Usuario = s.USUARIO.First(f => f.ID_PERSONA == s.ID_EMPLEADO)
                        }).ToList();
                        LstTipoServAux = new ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT>(new cTipo_Serv_Aux_Diag_Trat().ObtenerTodos("", "S"));
                        ListProcedimientoMedicoHijoAux = new ObservableCollection<PROC_MED>(new cProcedimientosMedicos().ObtenerTodos().Where(w => w.ESTATUS == "S"));
                        ListProcedimientoSubtipo = new ObservableCollection<PROC_MED_SUBTIPO>(new cProcedimientosSubtipo().ObtenerTodos().Where(w => w.ESTATUS == "S"));
                        ListProcedimientosMedicosEnCitaParaAgregar = new ObservableCollection<PROC_MED>(new cProcedimientosMedicos().ObtenerTodosActivos());
                        ListTipoTratamiento = new ObservableCollection<DENTAL_TIPO_TRATAMIENTO>(new cTratamientoTipoDental().ObtenerActivos());
                        ListPronostico = new ObservableCollection<PRONOSTICO>(new cPronostico().ObtenerTodos("S").OrderBy(o => o.DESCR));
                        ListDietas = new ObservableCollection<DietaMedica>(new cDietas().ObtenerTodosActivos().Where(w => w.ESTATUS == "S").OrderBy(o => o.DESCR).Select(s => new DietaMedica { DIETA = s, ELEGIDO = false }));
                        ListUnidadMedida = new ObservableCollection<PRODUCTO_UNIDAD_MEDIDA>(new cProducto_Unidad_Medida().Seleccionar("S"));
                        LstPatologicos = new ObservableCollection<PatologicosMedicos>(new cPatologicoCat().ObtenerTodo().Where(w => w.ESTATUS == "S").Select(s => new PatologicosMedicos { PATOLOGICO_CAT = s, SELECCIONADO = false }));
                        ListProcedimientosMedicos = new ObservableCollection<PROC_MED>(new cProcedimientosMedicos().ObtenerTodosActivos().Where(w =>
                            w.PROC_MED_SUBTIPO.ID_TIPO_ATENCION == (IsMedico ? (short)enumAtencionTipo.CONSULTA_MEDICA : IsDentista ? (short)enumAtencionTipo.CONSULTA_DENTAL : 0)).OrderBy(o => o.PROC_MED_SUBTIPO.DESCR).ThenBy(t => t.DESCR));
                        ListControlPrenatal = new ObservableCollection<CONTROL_PRENATAL>(new cControlPrenatal().GetData().Where(w => w.ESTATUS == "S"));
                        #endregion
                        ListRadioButonsFrente = new List<RadioButton>();
                        ListRadioButonsDorso = new List<RadioButton>();
                        ListCheckBoxOdontograma = new List<CheckBox>();
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            try
                            {
                                if (SelectLesion != null)
                                    foreach (var item in ListRadioButonsFrente)
                                        item.IsChecked = item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false;
                                if (SelectLesion != null)
                                    foreach (var item in ListRadioButonsDorso)
                                        item.IsChecked = item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false;
                                foreach (var item in ListCheckBoxOdontograma)
                                    item.IsChecked = false;
                                ListControlPrenatal.Insert(0, new CONTROL_PRENATAL { ID_CONTROL_PRENATAL = -1, DESCR = "SELECCIONE" });
                                SelectControlPreNatal = -1;
                                LstTipoServAux.Insert(0, new TIPO_SERVICIO_AUX_DIAG_TRAT { ID_TIPO_SADT = -1, DESCR = "TODOS" });
                                SelectedTipoServAux = -1;
                                LstSubTipoServAux = LstSubTipoServAux ?? new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>();
                                LstSubTipoServAux.Insert(0, new SUBTIPO_SERVICIO_AUX_DIAG_TRAT { ID_SUBTIPO_SADT = -1, DESCR = "TODOS" });
                                SelectedSubTipoServAux = -1;
                                LstDiagnosticosPrincipal = LstDiagnosticosPrincipal ?? new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>();
                                LstDiagnosticosPrincipal.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { ID_SERV_AUX = -1, DESCR = "TODOS" });
                                SelectedDiagnPrincipal = -1;
                                ListMotivosCancelarProcMed.Insert(0, new ATENCION_CITA_IN_MOTIVO { ID_ACMOTIVO = -1, DESCR = "SELECCIONAR" });
                                SelectMotivoCancelarProcMed = ListMotivosCancelarProcMed.First(f => f.ID_ACMOTIVO == -1);
                                ListProcedimientosMedicosEnCitaParaAgregar.Insert(0, new PROC_MED { DESCR = "SELECCIONE", ID_PROCMED = -1, });
                                ListProcedimientoSubtipo.Insert(0, new PROC_MED_SUBTIPO { DESCR = "SELECCIONE", ID_PROCMED_SUBTIPO = -1, });
                                SelectProcedimientoSubtipo = ListProcedimientoSubtipo.First(f => f.ID_PROCMED_SUBTIPO == -1);
                                SelectProcMedEnCitaParaAgendar = ListProcedimientosMedicosEnCitaParaAgregar.First(f => f.ID_PROCMED == -1);
                                if (!LstEmpleados.Any(a => a.ID_EMPLEADO == rol.USUARIO.ID_PERSONA))
                                    LstEmpleados.Add(new cUsuarioExtendida
                                    {
                                        ID_EMPLEADO = rol.USUARIO.ID_PERSONA.Value,
                                        NOMBRE_COMPLETO = rol.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() + " " + rol.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() + " " +
                                        (string.IsNullOrEmpty(rol.USUARIO.EMPLEADO.PERSONA.MATERNO) ? string.Empty : rol.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim()),
                                        Usuario = rol.USUARIO
                                    });
                                ListEnfermeros.Insert(0, new USUARIO { ID_USUARIO = "-1", EMPLEADO = new EMPLEADO { PERSONA = new SSP.Servidor.PERSONA { NOMBRE = "SELECCIONE" } } });
                                if (!ListEnfermeros.Any(a => a.ID_PERSONA == rol.USUARIO.ID_PERSONA))
                                    ListEnfermeros.Insert(1, rol.USUARIO);
                                LstEmpleados.Insert(0, new cUsuarioExtendida { ID_EMPLEADO = -1, NOMBRE_COMPLETO = "SELECCIONE" });
                                SelectMedico = -1;
                                CheckMedicoEnable = IsEnfermero;
                                SelectedEmpleadoValue = new cUsuarioExtendida
                                {
                                    ID_EMPLEADO = rol.USUARIO.ID_PERSONA.Value,
                                    NOMBRE_COMPLETO = rol.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() + " " + rol.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() + " " + (string.IsNullOrEmpty(rol.USUARIO.EMPLEADO.PERSONA.MATERNO) ? "" : rol.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim()),
                                    Usuario = rol.USUARIO,
                                }; ;
                                ListTipoTratamiento.Insert(0, new DENTAL_TIPO_TRATAMIENTO { ID_TIPO_TRATA = -1, DESCR = "SELECCIONE" });
                                SelectTipoTratamiento = ListTipoTratamiento.First(f => f.ID_TIPO_TRATA == -1);
                                ListTratamientoOdonto = new ObservableCollection<DENTAL_TRATAMIENTO>();
                                ListTratamientoOdonto.Insert(0, new DENTAL_TRATAMIENTO { ID_TRATA = -1, DESCR = "SELECCIONE" });
                                SelectTratamientoOdonto = ListTratamientoOdonto.First(f => f.ID_TRATA == -1);
                                ListPronostico.Insert(0, new PRONOSTICO { ID_PRONOSTICO = -1, DESCR = "SELECCIONE", ESTATUS = "S" });
                                ListUnidadMedida.Insert(0, new PRODUCTO_UNIDAD_MEDIDA { ID_UNIDAD_MEDIDA = -1, DESCR = "SELECCIONE", NOMBRE = "SELECCIONE" });
                                ListAtencionTipo.Insert(0, new ATENCION_TIPO { ID_TIPO_ATENCION = -1, DESCR = "SELECCIONE" });
                                ListTipoServicio = ListTipoServicio ?? new ObservableCollection<ATENCION_SERVICIO>();
                                ListTipoServicio.Insert(0, new ATENCION_SERVICIO { ID_TIPO_ATENCION = -1, ID_TIPO_SERVICIO = -1, DESCR = "SELECCIONE" });
                                ListTipoServicioAux = new ObservableCollection<ATENCION_SERVICIO>(ListTipoServicio);
                                SelectTipoAtencion = (!(IsDentista && IsMedico)) ? ListAtencionTipo.First(f => rol.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA ? f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL :
                                    f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA) : ListAtencionTipo.First(f => f.ID_TIPO_ATENCION == -1);
                                SelectTipoServicio = ListTipoServicio.First(f => f.ID_TIPO_ATENCION == -1 && f.ID_TIPO_SERVICIO == -1);
                                SelectPronostico = -1;
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la configuración inicial.", ex);
                            }
                        }));
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la configuración inicial.", ex);
                    }
                });
                IniciarTimer();
                timer.Start();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la configuración inicial", ex);
            }
        }

        private void IniciarTimer()
        {
            FechaActualizacion = Fechas.GetFechaDateServer.AddMinutes(1);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Stop();
        }

        async void timer_Tick(object sender, EventArgs e)
        {
            if (tMinuto == 1)
            {
                if (tSegundo >= 1)
                    tSegundo--;
                else
                {
                    if (tSegundo == 0)
                    {
                        tMinuto--;
                        tSegundo = 59;
                    }
                }
                TextContador = string.Format("Actualización en: 00:0{0}:{1}", tMinuto, tSegundo > 9 ? tSegundo.ToString() : "0" + tSegundo);
            }
            else
            {
                if (tSegundo >= 1)
                    tSegundo--;
                else
                    if (tSegundo == 0)
                    {
                        tMinuto = 0;
                        tSegundo = 59;
                        timer.Stop();
                        //StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateRequeridos);
                        //StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateAusentes);
                        if (ListaImputadosVisible == Visibility.Visible ?
                            (PopUpsViewModels.VisibleFondoOscuro != Visibility.Visible && PopUpsViewModels.VisibleBusquedaImputado != Visibility.Visible && StaticSourcesViewModel.ShowErrorDialog != Visibility.Visible && Preguntando)
                        : false)
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarImputados(); });
                        }
                        timer.Start();
                        return;
                    }
                TextContador = string.Format("Actualización en: 00:0{0}:{1}", tMinuto, tSegundo > 9 ? tSegundo.ToString() : "0" + tSegundo);
            }
            /*
            var result = ToolsTimer.GetTimeUntilNextHour(FechaActualizacion.Hour, FechaActualizacion.Minute, FechaActualizacion.Second);
            hora = result.Hours.ToString().Length < 2 ? string.Format("0{0}", result.Hours.ToString()) : result.Hours.ToString();
            minuto = result.Minutes.ToString().Length < 2 ? string.Format("0{0}", result.Minutes.ToString()) : result.Minutes.ToString();
            segundo = result.Seconds.ToString().Length < 2 ? string.Format("0{0}", result.Seconds.ToString()) : result.Seconds.ToString();
            TextContador = string.Format("Actualización en: {0}:{1}:{2}", hora, minuto, segundo);
            if (((result.Hours == 0 && result.Minutes == 0 && result.Seconds == 0) || result.Hours == 23))
            {
                TextContador = "Actualización en: 00:00:00";
                timer.Stop();
                if (ListaImputadosVisible == Visibility.Visible ?
                    (PopUpsViewModels.VisibleFondoOscuro != Visibility.Visible && PopUpsViewModels.VisibleBusquedaImputado != Visibility.Visible && StaticSourcesViewModel.ShowErrorDialog != Visibility.Visible)
                : false)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarImputados(); });
                }
                FechaActualizacion = Fechas.GetFechaDateServer.AddMinutes(1);
                timer.Start();
            }
            */
        }

        private void CargarImputados()
        {
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                #region LISTA DE IMPUTADOS
                var query = new cHospitalizacion().GetData(i => i.ID_HOSEST == 1 ?
                    i.NOTA_MEDICA != null ?
                        i.NOTA_MEDICA.ATENCION_MEDICA != null ?
                            i.NOTA_MEDICA.ATENCION_MEDICA.ID_CENTRO_UBI == GlobalVar.gCentro
                        : false
                    : false
                : false);
                if (query != null ? query.Any() : false)
                    ListHospitalizados = new ObservableCollection<HOSPITALIZACION>(query.OrderBy(o => o.ID_ATENCION_MEDICA));

                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los imputados.", ex);
            }
        }

        private void CargarDatosImputadoSeleccionado()
        {
            try
            {
                TextAnioImputado = SelectIngreso.ID_ANIO;
                TextFolioImputado = SelectIngreso.ID_IMPUTADO;
                TextNombreImputado = SelectIngreso.IMPUTADO.NOMBRE.Trim();
                TextPaternoImputado = SelectIngreso.IMPUTADO.PATERNO.Trim();
                TextMaternoImputado = string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim();
                TextEdad = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : "0";
                TextSexo = SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : SelectIngreso.IMPUTADO.SEXO == "F" ? "FEMENINO" : "";
                var hoy = Fechas.GetFechaDateServer;
                TextHoraRegistro = hoy.ToString("HH:mm");
                TextFechaRegistro = hoy.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los imputados.", ex);
            }
        }

        private void CargarDatosTipoServicio(short atencion)
        {
            try
            {

                ListTipoServicio = new ObservableCollection<ATENCION_SERVICIO>(new cAtencionServicio().ObtenerXAtencion(atencion));
                if (ListTipoServicio.Any())
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListTipoServicio.Insert(0, new ATENCION_SERVICIO { ID_TIPO_ATENCION = -1, ID_TIPO_SERVICIO = -1, DESCR = "SELECCIONE" });
                        SelectTipoServicio = ListTipoServicio.First(f => f.ID_TIPO_ATENCION == -1 && f.ID_TIPO_SERVICIO == -1);
                    }));

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos correctos.", ex);
            }

        }

        private void NotaMedicaUnLoad(NotaMedicaView Window = null)
        {
            try
            {
                AutoCompleteTB.PreviewMouseDown -= new MouseButtonEventHandler(listBox_MouseUp);
                AutoCompleteTB.KeyDown -= listBox_KeyDown;
                PrincipalViewModel.CambiarVentanaSelecccionado += TimerStop;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la configuración principal", ex);
            }
        }

        private void ValidaRol()
        {
            try
            {/*
                if (new cUsuarioRol().GetData().Any(x => x.ID_ROL == (short)enumRolesAreasTecnicas.ENFERMERO && (string.IsNullOrEmpty(x.ID_USUARIO) ? x.ID_USUARIO : x.ID_USUARIO.Trim()) == StaticSourcesViewModel.UsuarioLogin.Username))
                    IsSignosVitalesVisible = IsEnfermero = true;
                */
                if (new cUsuarioRol().GetData().Any(x => (x.ID_ROL == (short)enumRolesAreasTecnicas.MEDICO || x.ID_ROL == (short)enumRolesCoordinadoresAreasTecnicas.COORDINADOR_MEDICO) &&
                    (string.IsNullOrEmpty(x.ID_USUARIO) ? x.ID_USUARIO : x.ID_USUARIO.Trim()) == StaticSourcesViewModel.UsuarioLogin.Username))
                    IsMedico = true;
                /*IsSignosVitalesVisible = IsDiagnosticoEnabled = */

                /*if (new cUsuarioRol().GetData().Any(x => x.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA && (string.IsNullOrEmpty(x.ID_USUARIO) ? x.ID_USUARIO : x.ID_USUARIO.Trim()) == StaticSourcesViewModel.UsuarioLogin.Username))
                    IsDentista = true;

                if (IsEnfermero && IsMedico)
                    IsAmbos = true;*/
            }

            catch (Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar el rol del usuario.", exc);
            }
        }

        private async void clickSwitch(Object obj)
        {
            if (obj is Appointment)
            {
                try
                {
                    #region APPOINTMENT
                    var appoint = (Appointment)obj;
                    if (ProcedimientoMedicoParaAgenda == null)
                    {
                        AgregarHora = !AgregarHora;
                        if (AgregarHora)
                        {
                            AHoraI = appoint.StartTime;
                            AHoraF = appoint.EndTime;
                            ValidacionSolicitud();
                            var horaini = AHoraI;
                            var horafin = AHoraF;
                            //LimpiarAgenda();
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
                                    ValidacionSolicitud();
                                    var horaini = AHoraI;
                                    var horafin = AHoraF;
                                    //LimpiarAgenda();
                                    AHoraI = horaini;
                                    AHoraF = horafin;
                                    AFecha = new DateTime(appoint.StartTime.Year, appoint.StartTime.Month, appoint.StartTime.Day);
                                }
                                else
                                    base.ClearRules();
                                GuardarAgendaEnabled = true;
                                if (SelectIngreso != null ?
                                    SelectIngreso.ATENCION_CITA.Any(a => a.CITA_FECHA_HORA.Value.Year == AHoraI.Value.Year && a.CITA_FECHA_HORA.Value.Month == AHoraI.Value.Month && a.CITA_FECHA_HORA.Value.Day == AHoraI.Value.Day &&
                                        a.CITA_FECHA_HORA.Value.Hour == AHoraI.Value.Hour && a.CITA_FECHA_HORA.Value.Minute == AHoraI.Value.Minute)
                                : false)
                                {
                                    var cita = SelectIngreso.ATENCION_CITA.First(a => a.CITA_FECHA_HORA.Value.Year == AHoraI.Value.Year && a.CITA_FECHA_HORA.Value.Month == AHoraI.Value.Month && a.CITA_FECHA_HORA.Value.Day == AHoraI.Value.Day &&
                                        a.CITA_FECHA_HORA.Value.Hour == AHoraI.Value.Hour && a.CITA_FECHA_HORA.Value.Minute == AHoraI.Value.Minute);
                                    GuardarAgendaEnabled = cita != null ? !(cita.PROC_ATENCION_MEDICA_PROG.Any(w => w.ID_PROCMED == SelectProcMedSeleccionado.ID_PROC_MED)) : true;
                                }
                                else
                                    GuardarAgendaEnabled = ListProcMedsSeleccionados != null ? !ListProcMedsSeleccionados.Where(an => an.ID_PROC_MED == SelectProcMedSeleccionado.ID_PROC_MED).Any(an => an.CITAS != null ?
                                        an.CITAS.Any(a => a.FECHA_INICIAL.Year == AHoraI.Value.Year && a.FECHA_INICIAL.Month == AHoraI.Value.Month && a.FECHA_INICIAL.Day == AHoraI.Value.Day && a.FECHA_INICIAL.Hour == AHoraI.Value.Hour &&
                                        a.FECHA_INICIAL.Minute == AHoraI.Value.Minute) : false) : true;
                                if (ListProcMedsSeleccionados != null)
                                    ProcedimientosMedicosEnCitaEnMemoria = new ObservableCollection<CustomCitasProcedimientosMedicos>(ListProcMedsSeleccionados.Where(w => w.CITAS != null).SelectMany(s => s.CITAS)
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
                    #region PROCEDIMIENTOS MEDICOS
                    ProcedimientoMedicoParaAgenda = (CustomProcedimientosMedicosSeleccionados)obj;
                    if (LstAgenda == null)
                        LstAgenda = new ObservableCollection<Appointment>();
                    EmpleadosEnAgendaEnabled = true;
                    ProcedimientoMedicoPorAgendar = ProcedimientoMedicoParaAgenda.PROC_MED_DESCR;
                    AgregarProcedimientoMedicoLayoutVisible = Visibility.Visible;
                    SelectedEmpleadoValue = LstEmpleados.First(f => f.ID_EMPLEADO == -1);
                    AgendaView = new AgendarCitaConCalendarioView();
                    SelectedDateCalendar = fHoy;
                    AgendaView.Loaded += AgendaLoaded;
                    AgendaView.Agenda.AddAppointment += AgendaAddAppointment;
                    AgendaView.btn_guardar.Click += AgendaClick;
                    AgendaView.Closing += AgendaClosing;
                    AgendaView.Owner = PopUpsViewModels.MainWindow;
                    OnPropertyChanged("LstEmpleados");
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
            if (obj is DataGrid)
            {
                var cacha = string.Empty;
                try
                {
                    if (((DataGrid)obj).Name.Contains("Historial"))
                    {
                        cacha = "Ocurrió un error al cargar los datos de la enfermedad de la historia clínica.";
                        #region HISTORIA CLINICA
                        var selected = ((HistoriaclinicaPatologica)((DataGrid)obj).SelectedItem);
                        if (!selected.DESHABILITADO) return;
                        RecetaHistoria = false;
                        ProcedimientoMedico = false;
                        TituloHeaderExpandirDescripcion = "DESCRIPCION DE " + selected.PATOLOGICO_CAT.DESCR;
                        TextAmpliarDescripcion = selected.OBSERVACIONES;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                        #endregion
                    }
                    else if (((DataGrid)obj).Name.Contains("dgProcedimientos"))
                    {
                        cacha = "Ocurrió un error al cargar los datos del procedimiento médico seleccionado.";
                        #region PROCEDIMIENTOS MEDICOS
                        var selected = ((CustomProcedimientosMedicosSeleccionados)((DataGrid)obj).SelectedItem);
                        TituloHeaderExpandirDescripcion = "DESCRIPCION DE " + selected.PROC_MED_DESCR;
                        ProcedimientoMedico = true;
                        RecetaHistoria = false;
                        TextAmpliarDescripcion = selected.OBSERV;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                        #endregion
                    }
                    else if (((DataGrid)obj).Name.Contains("Receta"))
                    {
                        cacha = "Ocurrió un error al cargar los datos de la receta médica.";
                        #region RECETA MEDICA
                        if (!(((DataGrid)obj).SelectedItem is RecetaMedica)) return;
                        var selected = ((RecetaMedica)((DataGrid)obj).SelectedItem);
                        RecetaHistoria = true;
                        ProcedimientoMedico = false;
                        TituloHeaderExpandirDescripcion = "OBSERVACIONES DE " + selected.PRODUCTO.NOMBRE;
                        TextAmpliarDescripcion = selected.OBSERVACIONES;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                        #endregion
                    }
                    //else if (((DataGrid)obj).Name.Contains("Hospitalizados"))
                    else
                    {
                        cacha = "Ocurrió un error al cargar los datos del imputado seleccionado.";
                        #region SELECT IMPUTADO LISTA
                        if (!(((DataGrid)obj).SelectedItem is INGRESO) && !(((DataGrid)obj).SelectedItem is ATENCION_CITA) && !(((DataGrid)obj).SelectedItem is ATENCION_MEDICA) && !(((DataGrid)obj).SelectedItem is EXCARCELACION) && !(((DataGrid)obj).SelectedItem is TRASLADO_DETALLE))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Ocurrió un error con el imputado seleccionado.");
                            return;
                        }
                        var selected = ((DataGrid)obj).SelectedItem;
                        var EsUrgencia = ((DataGrid)obj).Name.Contains("Urgencia");
                        var EsSancion = ((DataGrid)obj).Name.Contains("Sancion");
                        var regresa = false;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            var ingreso = new INGRESO();
                            var atencionCita = new ATENCION_CITA();
                            var urgencia = new ATENCION_MEDICA();
                            var sancion = new ATENCION_MEDICA();
                            var excarcelacion = new EXCARCELACION();
                            var trasladoDetalle = new TRASLADO_DETALLE();
                            SelectAtencionMedica = new ATENCION_MEDICA();
                            SelectCitaMedicaImputado = null;
                            SelectExcarcelacionImputado = null;
                            SelectTrasladoImputado = null;
                            SelectIngresoConsulta = null;
                            SelectAtencionMedica = null;
                            ingreso = null;
                            atencionCita = null;
                            excarcelacion = null;
                            trasladoDetalle = null;
                            if (selected is INGRESO)
                            {
                                ingreso = (INGRESO)selected;
                                SelectIngresoConsulta = (INGRESO)selected;
                                SelectAtencionMedica = ingreso.ATENCION_MEDICA.Any(w => IsMedico ?
                                    (w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO)
                                : IsDentista ?
                                    (w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO)
                                : IsEnfermero ?
                                    (w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO)
                                : false) ?
                                    ingreso.ATENCION_MEDICA.First(w => IsMedico ?
                                        (w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO)
                                    : IsDentista ?
                                        (w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO)
                                    : IsEnfermero ?
                                        (w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO)
                                    : false)
                                : null;
                                if (IsEnfermero ? SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null : false : false)
                                {
                                    SelectAtencionMedica = null;
                                    SelectAtencionMedicaAux = null;
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        new Dialogos().ConfirmacionDialogo("Notificación!", "El imputado ya tiene los signos vitales capturados.");
                                    }));
                                    regresa = true;
                                    return;
                                }
                            }
                            else if (selected is ATENCION_CITA)
                            {
                                atencionCita = new cAtencionCita().Obtener(((ATENCION_CITA)selected).ID_CITA, GlobalVar.gCentro);
                                SelectCitaMedicaImputado = atencionCita;
                                ingreso = atencionCita.INGRESO;
                                SelectAtencionMedica = atencionCita.ATENCION_MEDICA;
                                if (IsMedico)
                                    atencionCita.ID_USUARIO = GlobalVar.gUsr;
                                if (IsEnfermero)
                                {
                                    CambiarMedicoVisible = Visibility.Collapsed;
                                    ObsSignosVitalesVisible = Visibility.Collapsed;
                                    PesoEnabled = false;
                                    TallaEnabled = atencionCita.ID_TIPO_ATENCION != (short)enumAtencionTipo.CONSULTA_MEDICA;
                                }
                                if (atencionCita.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS)
                                {
                                    ListProcedimientosMedicosEnCita = new ObservableCollection<CustomProcedimientosMedicosCitados>(
                                        atencionCita.PROC_ATENCION_MEDICA_PROG.Where(f => f.ID_CENTRO_UBI == GlobalVar.gCentro && f.ID_CITA == atencionCita.ID_CITA && f.PROC_ATENCION_MEDICA != null).Select(s => s.PROC_ATENCION_MEDICA.PROC_MED)
                                        .Select(s => new CustomProcedimientosMedicosCitados
                                        {
                                            PROC_MED = new PROC_MED
                                            {
                                                DESCR = s.DESCR,
                                                ESTATUS = s.ESTATUS,
                                                ID_PROCMED = s.ID_PROCMED,
                                                ID_PROCMED_SUBTIPO = s.ID_PROCMED_SUBTIPO,
                                                PROC_ATENCION_MEDICA = s.PROC_ATENCION_MEDICA,
                                                PROC_MATERIAL = s.PROC_MATERIAL,
                                                PROC_MED_SUBTIPO = s.PROC_MED_SUBTIPO,
                                            },
                                            PROCEDIMIENTOS_MATERIALES = new ObservableCollection<CustomProcedimientosMaterialesCitados>(s.PROC_MATERIAL.Select(sp => sp == null ? null :
                                                new CustomProcedimientosMaterialesCitados
                                                {
                                                    CANTIDAD = new Nullable<int>(),
                                                    PROC_MATERIAL = sp,
                                                    PRODUCTO = sp.PRODUCTO
                                                })),
                                            IsVisible = s.PROC_MATERIAL.Any() ? Visibility.Visible : Visibility.Collapsed,
                                        }).OrderBy(o => o.PROC_MED.DESCR));
                                    ListTipoServicio = new ObservableCollection<ATENCION_SERVICIO>();
                                    ListTipoServicio.Add(new ATENCION_SERVICIO { ID_TIPO_SERVICIO = (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS, DESCR = "PROCEDIMIENTO MEDICO" });
                                    SelectTipoServicio = ListTipoServicio.FirstOrDefault();
                                    Application.Current.Dispatcher.Invoke((Action)(delegate { AutoCompleteReceta.IsEnabled = false; }));
                                    if (atencionCita.PROC_ATENCION_MEDICA_PROG != null ?
                                        atencionCita.PROC_ATENCION_MEDICA_PROG.Any() ?
                                            atencionCita.PROC_ATENCION_MEDICA_PROG.Select(s => s.PROC_ATENCION_MEDICA).Any() ?
                                                atencionCita.PROC_ATENCION_MEDICA_PROG.Select(s => s.PROC_ATENCION_MEDICA).Select(s => s.ATENCION_MEDICA).Any() ?
                                                    atencionCita.PROC_ATENCION_MEDICA_PROG.Select(s => s.PROC_ATENCION_MEDICA).Select(s => s.ATENCION_MEDICA).Where(w => w != null ? w.RECETA_MEDICA != null ? w.RECETA_MEDICA.Any() : false : false)
                                                    .SelectMany(s => s.RECETA_MEDICA).Any() ?
                                                        atencionCita.PROC_ATENCION_MEDICA_PROG.Select(s => s.PROC_ATENCION_MEDICA).Select(s => s.ATENCION_MEDICA).Where(w => w != null ? w.RECETA_MEDICA != null ? w.RECETA_MEDICA.Any() : false : false)
                                                        .SelectMany(s => s.RECETA_MEDICA).SelectMany(s => s.RECETA_MEDICA_DETALLE).Any()
                                                    : false
                                                : false
                                            : false
                                        : false
                                    : false)
                                    {
                                        ListRecetas = new ObservableCollection<RecetaMedica>(
                                            atencionCita.PROC_ATENCION_MEDICA_PROG.Where(w => w.ID_CENTRO_UBI == GlobalVar.gCentro).Select(s => s.PROC_ATENCION_MEDICA).Select(s => s.ATENCION_MEDICA)
                                               .SelectMany(s => s.RECETA_MEDICA).SelectMany(s => s.RECETA_MEDICA_DETALLE).Select(s => new RecetaMedica
                                               {
                                                   CANTIDAD = s.DOSIS,
                                                   DURACION = s.DURACION,
                                                   HORA_MANANA = s.DESAYUNO == 1,
                                                   HORA_NOCHE = s.CENA == 1,
                                                   HORA_TARDE = s.COMIDA == 1,
                                                   OBSERVACIONES = s.OBSERV,
                                                   PRODUCTO = s.PRODUCTO,
                                                   MEDIDA = s.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? s.PRODUCTO.ID_UNIDAD_MEDIDA.Value : 0,
                                                   PRESENTACION = s.ID_PRESENTACION_MEDICAMENTO,
                                               }).DistinctBy(d => d.PRODUCTO.ID_PRODUCTO));
                                    }
                                    if (atencionCita.PROC_ATENCION_MEDICA_PROG != null ?
                                        atencionCita.PROC_ATENCION_MEDICA_PROG.Any() ?
                                            atencionCita.PROC_ATENCION_MEDICA_PROG.Select(s => s.PROC_ATENCION_MEDICA).Any() ?
                                                atencionCita.PROC_ATENCION_MEDICA_PROG.Select(s => s.PROC_ATENCION_MEDICA).Select(s => s.ATENCION_MEDICA).Any() ?
                                                    atencionCita.PROC_ATENCION_MEDICA_PROG.Select(s => s.PROC_ATENCION_MEDICA).Select(s => s.ATENCION_MEDICA != null ? s.ATENCION_MEDICA.NOTA_MEDICA != null ? s.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any() : false : false).Any()
                                                : false
                                            : false
                                        : false
                                    : false)
                                    {
                                        var dietas = atencionCita.PROC_ATENCION_MEDICA_PROG.Select(s => s.PROC_ATENCION_MEDICA)
                                            .Where(s => s.ATENCION_MEDICA != null ? s.ATENCION_MEDICA.NOTA_MEDICA != null ? s.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any() : false : false)
                                            .SelectMany(s => s.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_DIETA);
                                        foreach (var item in ListDietas)
                                        {
                                            if (dietas.Any(a => a.ID_DIETA == item.DIETA.ID_DIETA))
                                            {
                                                item.ELEGIDO = true;
                                            }
                                        }
                                    }
                                    ListDietas = new ObservableCollection<DietaMedica>(ListDietas.OrderBy(o => o.DIETA.DESCR));
                                    //ListDietasAux = new ObservableCollection<DietaMedica>(ListDietas.OrderBy(o => o.DIETA.DESCR));
                                    base.ClearRules();
                                    LimpiarValidaciones();
                                }
                            }
                            else if (selected is ATENCION_MEDICA ? EsUrgencia : false)
                            {
                                urgencia = (ATENCION_MEDICA)selected;
                                SelectUrgenciaImputado = urgencia;
                                ingreso = urgencia.INGRESO;
                                SelectAtencionMedica = urgencia;
                            }
                            else if (selected is ATENCION_MEDICA ? EsSancion : false)
                            {
                                sancion = (ATENCION_MEDICA)selected;
                                SelectSancionImputado = sancion;
                                ingreso = sancion.INGRESO;
                                SelectAtencionMedica = sancion;
                            }
                            else if (selected is EXCARCELACION)
                            {
                                AtencionTipoEnabled = false;
                                TipoNotaEnabled = false;
                                excarcelacion = (EXCARCELACION)selected;
                                SelectExcarcelacionImputado = excarcelacion;
                                ingreso = excarcelacion.INGRESO;
                                SelectAtencionMedica = excarcelacion.CERT_MEDICO_SALIDA.HasValue ?
                                    (excarcelacion.CERT_MEDICO_RETORNO.HasValue ?
                                        (excarcelacion.ATENCION_MEDICA1 != null ?
                                            (excarcelacion.ATENCION_MEDICA1.NOTA_SIGNOS_VITALES != null ?
                                                (excarcelacion.ATENCION_MEDICA1.CERTIFICADO_MEDICO != null ?
                                                    excarcelacion.ATENCION_MEDICA1
                                                : excarcelacion.ATENCION_MEDICA1)
                                            : excarcelacion.ATENCION_MEDICA1)
                                        : excarcelacion.ATENCION_MEDICA != null ?
                                            excarcelacion.ATENCION_MEDICA
                                        : null)
                                    : excarcelacion.ATENCION_MEDICA != null ?
                                        excarcelacion.ATENCION_MEDICA
                                    : null)
                                : null;
                            }
                            else if (selected is TRASLADO_DETALLE)
                            {
                                AtencionTipoEnabled = false;
                                TipoNotaEnabled = false;
                                trasladoDetalle = (TRASLADO_DETALLE)selected;
                                SelectTrasladoImputado = trasladoDetalle;
                                ingreso = trasladoDetalle.INGRESO;
                                SelectAtencionMedica = trasladoDetalle.ATENCION_MEDICA;
                            }
                            if (ingreso.ID_UB_CENTRO.HasValue ? ingreso.ID_UB_CENTRO != GlobalVar.gCentro : false)
                            {
                                SelectExpediente = null;
                                SelectIngreso = null;
                                SelectAtencionMedica = null;
                                SelectAtencionMedicaAux = null;
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                                }));
                                regresa = true;
                                return;
                            }
                            if (ParametroEstatusInactivos.Any(a => a.HasValue ? a.Value == ingreso.ID_ESTATUS_ADMINISTRATIVO : false))
                            {
                                SelectAtencionMedica = null;
                                SelectAtencionMedicaAux = null;
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                                }));
                                regresa = true;
                                return;
                            }
                            if (IsEnfermero)
                            {
                                if (SelectAtencionMedica != null ?
                                        SelectAtencionMedica.NOTA_SIGNOS_VITALES != null
                                    : false)
                                {
                                    if (SelectExcarcelacionImputado != null ?
                                            (SelectExcarcelacionImputado.CERT_MEDICO_SALIDA.HasValue ?
                                                (SelectExcarcelacionImputado.ATENCION_MEDICA != null ?
                                                    SelectExcarcelacionImputado.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                                        SelectExcarcelacionImputado.CERT_MEDICO_RETORNO.HasValue
                                                    : true
                                                : true)
                                            : SelectExcarcelacionImputado.CERT_MEDICO_RETORNO.HasValue)
                                        : true)
                                    {
                                        SelectAtencionMedica = null;
                                        SelectAtencionMedicaAux = null;
                                        Application.Current.Dispatcher.Invoke((Action)(delegate
                                        {
                                            new Dialogos().ConfirmacionDialogo("Notificación!", "El imputado ya tiene los signos vitales capturados.");
                                        }));
                                        regresa = true;
                                        return;
                                    }
                                    else
                                        RetornoExcarcelacion = true;
                                }
                            }
                            if (IsMedico ?
                                SelectAtencionMedica != null ?
                                    SelectAtencionMedica.NOTA_SIGNOS_VITALES != null ?
                                        SelectAtencionMedica.NOTA_MEDICA != null
                                    : false
                                : false
                            : false)
                            {
                                if (IsMedico ?
                                        ((SelectExcarcelacionImputado != null ?
                                            (SelectExcarcelacionImputado.CERT_MEDICO_SALIDA.HasValue ?
                                                (SelectExcarcelacionImputado.ATENCION_MEDICA != null ?
                                                    (SelectExcarcelacionImputado.ATENCION_MEDICA.CERTIFICADO_MEDICO != null ?
                                                        (SelectExcarcelacionImputado.CERT_MEDICO_RETORNO.HasValue ?
                                                            (SelectExcarcelacionImputado.ATENCION_MEDICA1 != null ?
                                                                SelectExcarcelacionImputado.ATENCION_MEDICA1.CERTIFICADO_MEDICO != null
                                                            : false)
                                                        : false)
                                                    : false)
                                                : false)
                                            : false)
                                        : false)
                                        || (SelectTrasladoImputado != null ?
                                            SelectTrasladoImputado.ID_ATENCION_MEDICA.HasValue ?
                                                SelectTrasladoImputado.ATENCION_MEDICA.CERTIFICADO_MEDICO != null
                                            : false
                                        : false))
                                    : false)
                                {
                                    SelectAtencionMedica = null;
                                    SelectAtencionMedicaAux = null;
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        new Dialogos().ConfirmacionDialogo("Notificación!", "El imputado ya tiene la consulta capturada.");
                                    }));
                                    regresa = true;
                                    return;
                                }
                                else
                                {
                                    if (SelectIngresoConsulta != null)
                                    {
                                        var x = SelectIngresoConsulta.ATENCION_MEDICA.Any(w => w.NOTA_SIGNOS_VITALES != null && w.ID_TIPO_ATENCION == ((IsMedico || IsEnfermero) ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL) &&
                                            w.ID_TIPO_SERVICIO == ((IsMedico || IsEnfermero) ? (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO : (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO)) ?
                                                SelectIngresoConsulta.ATENCION_MEDICA.First(w => w.NOTA_SIGNOS_VITALES != null && w.ID_TIPO_ATENCION == ((IsMedico || IsEnfermero) ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL) &&
                                                    w.ID_TIPO_SERVICIO == ((IsMedico || IsEnfermero) ? (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO : (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO)) : null;
                                        if (x != null ?
                                            x.ID_TIPO_ATENCION == ((IsMedico || IsEnfermero) ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL) ?
                                                x.ID_TIPO_SERVICIO == ((IsMedico || IsEnfermero) ? (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO : (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO) ?
                                                    x.NOTA_SIGNOS_VITALES != null ?
                                                        x.NOTA_MEDICA != null
                                                    : false
                                                : false
                                            : false
                                        : false)
                                        {
                                            SelectAtencionMedica = null;
                                            SelectAtencionMedicaAux = null;
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                new Dialogos().ConfirmacionDialogo("Notificación!", "El imputado ya tiene la consulta capturada.");
                                            }));
                                            regresa = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (EsUrgencia || EsSancion)
                                        {
                                            if (SelectAtencionMedica != null ?
                                                SelectAtencionMedica.NOTA_SIGNOS_VITALES != null ?
                                                    SelectAtencionMedica.NOTA_MEDICA != null
                                                : false
                                            : false)
                                            {
                                                SelectAtencionMedica = null;
                                                SelectAtencionMedicaAux = null;
                                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                                {
                                                    new Dialogos().ConfirmacionDialogo("Notificación!", "El imputado ya tiene la consulta capturada.");
                                                }));
                                                regresa = true;
                                                return;
                                            }
                                        }
                                        else
                                            RetornoExcarcelacion = IsMedico ?
                                                (SelectExcarcelacionImputado != null ?
                                                    (SelectExcarcelacionImputado.CERT_MEDICO_SALIDA.HasValue ?
                                                        (SelectExcarcelacionImputado.ATENCION_MEDICA != null ?
                                                            (SelectExcarcelacionImputado.ATENCION_MEDICA.CERTIFICADO_MEDICO != null ?
                                                                (SelectExcarcelacionImputado.CERT_MEDICO_RETORNO.HasValue ||
                                                                (SelectExcarcelacionImputado.ATENCION_MEDICA1 != null ?
                                                                    SelectExcarcelacionImputado.ATENCION_MEDICA1.CERTIFICADO_MEDICO == null
                                                                : true))
                                                            : false)
                                                        : false)
                                                    : false)
                                                : false)
                                            : false;
                                    }
                                }
                            }
                            SelectExpediente = ingreso.IMPUTADO;
                            SelectIngreso = ingreso;
                        });
                        if (regresa) return;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        HuellaWindow = new BuscarPorHuellaYNipView();
                        HuellaWindow.DataContext = this;
                        ConstructorHuella(0);
                        HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                        //HuellaWindow.Closed += HuellaClosed;
                        HuellaWindow.ShowDialog();
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", cacha, ex);
                }
                return;
            }
            switch (obj.ToString())
            {

                case "seleccionar_hospitalizacion":
                    try
                    {
                        //cacha = "Ocurrió un error al cargar los datos del imputado hospitalizado.";

                        #region HOSPITALIZACION
                        //if (!(((DataGrid)obj).SelectedItem is HOSPITALIZACION)) return;
                        //SelectHospitalizacion = ((HOSPITALIZACION)((DataGrid)obj).SelectedItem);
                        if (SelectHospitalizado == null) return;
                        SelectIngreso = SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.INGRESO;
                        SelectedIngreso = SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.INGRESO;
                        var UltimaNota = new NOTA_EVOLUCION();
                        var UltimaNotaString = string.Empty;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            var query = new cNotaEvolucion().ObtenerTodas().Where(a => a.NOTA_MEDICA != null ?
                                a.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                    a.NOTA_MEDICA.ATENCION_MEDICA.ID_CENTRO == SelectIngreso.ID_CENTRO ?
                                        a.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO == SelectIngreso.ID_ANIO ?
                                            a.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO ?
                                                a.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO == SelectIngreso.ID_INGRESO
                                            : false
                                        : false
                                    : false
                                : false
                            : false);
                            UltimaNota = query.Any() ? query.OrderByDescending(o => o.REGISTRO_FEC).FirstOrDefault() : null;
                            var personaNombre = UltimaNota != null ?
                                UltimaNota.NOTA_MEDICA.PERSONA.NOMBRE.Trim() + " " + UltimaNota.NOTA_MEDICA.PERSONA.PATERNO.Trim() + " " +
                                (string.IsNullOrEmpty(UltimaNota.NOTA_MEDICA.PERSONA.MATERNO) ? string.Empty : UltimaNota.NOTA_MEDICA.PERSONA.MATERNO.Trim())
                            : string.Empty;
                            UltimaNotaString = UltimaNota != null ?
                                ("ÚLTIMA NOTA\nFecha: " + UltimaNota.REGISTRO_FEC.Value.ToString("dd/MM/yyyy") + " Hora: " + UltimaNota.REGISTRO_FEC.Value.ToString("HH:mm tt") + "\nUsuario: " + personaNombre)  ///falta usuario en nota evolucion 
                            : "NOTA DE EVOLUCIÓN";
                            //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        });
                        await TaskEx.Delay(100);
                        NotaEvolucion = SelectHospitalizado.ATENCION_MEDICA.Any() ?
                            SelectHospitalizado.ATENCION_MEDICA.Any(a => a.NOTA_MEDICA != null ? a.NOTA_MEDICA.NOTA_EVOLUCION != null : false) ?
                                SelectHospitalizado.ATENCION_MEDICA.OrderByDescending(o => o.ATENCION_FEC).First(a => a.NOTA_MEDICA != null ? a.NOTA_MEDICA.NOTA_EVOLUCION != null : false)
                            : null
                        : null;
                        var pregunta = 0;
                        Preguntando = false;
                        //if (NotaEvolucion == null)
                        pregunta = NotaEvolucion != null ?
                            await new Dialogos().ConfirmacionTresBotonesDinamico(UltimaNotaString,
                                "Se va a realizar una nueva nota de evolución o se va a agregar a la última nota de evolución realizada?", "Nueva nota", 0, "Agregar a última", 1, "Cancelar", 2)
                        : await new Dialogos().ConfirmacionDosBotonesCustom(UltimaNotaString,
                             "Está usted seguro que desea realizar una nueva nota de evolución?", "Nueva nota", 0, "Cancelar", 2);
                        Preguntando = true;
                        //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        if (pregunta == 0)
                        {
                            NuevaNota = true;
                            AgregarNota = false;
                        }
                        else if (pregunta == 1)
                        {
                            NuevaNota = false;
                            AgregarNota = true;
                        }
                        else if (pregunta == 2)
                        {
                            NuevaNota = false;
                            AgregarNota = false;
                            return;
                        }
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            ValidacionesMedico();
                            var UltimaHojaEnfermeria = SelectHospitalizado.NOTA_MEDICA != null ?
                                SelectHospitalizado.HOJA_ENFERMERIA != null ?
                                    SelectHospitalizado.HOJA_ENFERMERIA.Any(a => a.HOJA_ENFERMERIA_LECTURA.Any()) ?
                                        SelectHospitalizado.HOJA_ENFERMERIA.Where(w => w.HOJA_ENFERMERIA_LECTURA.Any()).OrderByDescending(o => o.FECHA_REGISTRO).FirstOrDefault().HOJA_ENFERMERIA_LECTURA.OrderByDescending(o => o.FECHA_LECTURA).FirstOrDefault()
                                    : null
                                : null
                            : null;

                            #region SIGNOS VITALES DE ATENCION MEDICA
                            TextUltimaHoraSignos = AgregarNota ?
                                SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.ATENCION_FEC.HasValue ?
                                            SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.ATENCION_FEC.Value.ToString("HH:mm")
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : UltimaHojaEnfermeria != null ?
                                UltimaHojaEnfermeria.FECHA_LECTURA.HasValue ?
                                    UltimaHojaEnfermeria.FECHA_LECTURA.Value.ToString("HH:mm")
                                : string.Empty
                            : string.Empty;

                            TextUltimaFechaSignos = AgregarNota ?
                                SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.ATENCION_FEC.HasValue ?
                                            SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.ATENCION_FEC.Value.ToString("dd/MM/yyyy")
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : UltimaHojaEnfermeria != null ?
                                UltimaHojaEnfermeria.FECHA_LECTURA.HasValue ?
                                    UltimaHojaEnfermeria.FECHA_LECTURA.Value.ToString("dd/MM/yyyy")
                                : string.Empty
                            : string.Empty;

                            TextPeso = AgregarNota ?
                                SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ?
                                            SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : string.Empty;

                            TextTalla = AgregarNota ?
                                SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ?
                                            SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TALLA
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : string.Empty;

                            TextGlucemia = AgregarNota ?
                                SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ?
                                            SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.GLUCEMIA
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : string.Empty;

                            TextFrecuenciaCardiaca = AgregarNota ?
                                SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ?
                                            SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : UltimaHojaEnfermeria != null ?
                                UltimaHojaEnfermeria.PC
                            : string.Empty;

                            TextFrecuenciaRespira = AgregarNota ?
                                SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ?
                                            SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : UltimaHojaEnfermeria != null ?
                                UltimaHojaEnfermeria.PR
                            : string.Empty;

                            TextTemperatura = AgregarNota ?
                                SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ?
                                            SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TEMPERATURA
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : UltimaHojaEnfermeria != null ?
                                UltimaHojaEnfermeria.TEMP
                            : string.Empty;

                            TextTensionArterial1 = AgregarNota ?
                                SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ?
                                            !string.IsNullOrEmpty(SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL) ?
                                                SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL.Split('/')[0]
                                            : string.Empty
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : UltimaHojaEnfermeria != null ?
                                !string.IsNullOrEmpty(UltimaHojaEnfermeria.TA) ?
                                    UltimaHojaEnfermeria.TA.Split('/')[0]
                                : string.Empty
                            : string.Empty;

                            TextTensionArterial2 = AgregarNota ?
                                SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ?
                                            !string.IsNullOrEmpty(SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL) ?
                                                SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL.Split('/').Count() > 1 ?
                                                    SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL.Split('/')[1]
                                                : string.Empty
                                            : string.Empty
                                        : string.Empty
                                    : string.Empty
                                : string.Empty
                            : UltimaHojaEnfermeria != null ?
                                !string.IsNullOrEmpty(UltimaHojaEnfermeria.TA) ?
                                    UltimaHojaEnfermeria.TA.Split('/').Count() > 1 ?
                                        UltimaHojaEnfermeria.TA.Split('/')[1]
                                    : string.Empty
                                : string.Empty
                            : string.Empty;

                            #endregion

                            SolicitaInterconsultaCheck = AgregarNota ?
                                NotaEvolucion != null ?
                                    NotaEvolucion.NOTA_MEDICA != null ?
                                        NotaEvolucion.NOTA_MEDICA.OCUPA_INTERCONSULTA == "S"
                                    : false
                                : false
                            : false;
                            InterconsultaEnabled = AgregarNota ?
                                NotaEvolucion != null ?
                                    NotaEvolucion.NOTA_MEDICA != null ?
                                        NotaEvolucion.NOTA_MEDICA.OCUPA_INTERCONSULTA == "S" ?
                                            NotaEvolucion.NOTA_MEDICA.CANALIZACION == null
                                        : true
                                    : true
                                : true
                            : true;

                            if (AgregarNota ? SelectHospitalizado.NOTA_MEDICA != null ? SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA != null : false : false)
                            {
                                if (NotaEvolucion == null) return;

                                #region CAMPOS ABIERTOS
                                LabelExploracionFisica = NotaEvolucion.NOTA_MEDICA != null ?
                                    NotaEvolucion.NOTA_MEDICA.EXPLORACION_FISICA
                                : string.Empty;
                                if (NotaEvolucion.NOTA_MEDICA != null ?
                                    !string.IsNullOrEmpty(NotaEvolucion.NOTA_MEDICA.EXPLORACION_FISICA) ?
                                        NotaEvolucion.NOTA_MEDICA.EXPLORACION_FISICA.Length > 0
                                    : false
                                : false)
                                    LengthExploracionFisica = 498 - NotaEvolucion.NOTA_MEDICA.EXPLORACION_FISICA.Length;
                                else
                                    LengthExploracionFisica = 500;
                                TextExploracionFisica = string.Empty;

                                LabelResumenInterrogatorio = NotaEvolucion.NOTA_MEDICA != null ?
                                    NotaEvolucion.NOTA_MEDICA.RESUMEN_INTERROGAT
                                : string.Empty;
                                if (NotaEvolucion.NOTA_MEDICA != null ?
                                    !string.IsNullOrEmpty(NotaEvolucion.NOTA_MEDICA.RESUMEN_INTERROGAT) ?
                                        NotaEvolucion.NOTA_MEDICA.RESUMEN_INTERROGAT.Length > 0
                                    : false
                                : false)
                                    LengthResumenInterrogatorio = 498 - NotaEvolucion.NOTA_MEDICA.RESUMEN_INTERROGAT.Length;
                                else
                                    LengthResumenInterrogatorio = 500;
                                TextResumenInterrogatorio = string.Empty;

                                LabelEstudios = NotaEvolucion.NOTA_MEDICA != null ?
                                    NotaEvolucion.NOTA_MEDICA.NOTA_EVOLUCION.ESTUDIO_RESULTADO
                                : string.Empty;
                                if (NotaEvolucion.NOTA_MEDICA != null ?
                                    NotaEvolucion.NOTA_MEDICA.NOTA_EVOLUCION != null ?
                                        !string.IsNullOrEmpty(NotaEvolucion.NOTA_MEDICA.NOTA_EVOLUCION.ESTUDIO_RESULTADO) ?
                                            NotaEvolucion.NOTA_MEDICA.NOTA_EVOLUCION.ESTUDIO_RESULTADO.Length > 0
                                        : false
                                    : false
                                : false)
                                    LengthEstudios = 498 - NotaEvolucion.NOTA_MEDICA.NOTA_EVOLUCION.ESTUDIO_RESULTADO.Length;
                                else
                                    LengthEstudios = 500;
                                TextEstudios = string.Empty;

                                SelectPronostico = NotaEvolucion != null ?
                                    NotaEvolucion.NOTA_MEDICA.ID_PRONOSTICO
                                : new Nullable<short>();
                                #endregion
                            }
                            else if (NuevaNota || (SelectHospitalizado.NOTA_MEDICA != null ? (SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA == null && (NuevaNota || AgregarNota)) : true))
                            {
                                LabelExploracionFisica = string.Empty;
                                LengthExploracionFisica = 500;
                                TextExploracionFisica = string.Empty;
                                LabelResumenInterrogatorio = string.Empty;
                                LengthResumenInterrogatorio = 500;
                                TextResumenInterrogatorio = string.Empty;
                                LabelEstudios = string.Empty;
                                LengthEstudios = 500;
                                TextEstudios = string.Empty;
                            }
                            else
                            {
                                SelectHospitalizado = null;
                                ListHospitalizados = new ObservableCollection<HOSPITALIZACION>(ListHospitalizados.ToList());
                                SelectHospitalizado = null;
                                SelectIngreso = null;
                                LimpiarValidaciones();
                                return;
                            }
                            ListaImputadosVisible = Visibility.Collapsed;
                            EstudiosVisible = Visibility.Visible;
                            NotaMedicaVisible = Visibility.Visible;
                            DiagnosticoVisible = Visibility.Visible;
                            SignosVitalesVisible = Visibility.Visible;
                            PlanSeguimientoVisible = Visibility.Visible;
                            ExploracionFisicaVisible = Visibility.Visible;
                            ResumenInterrogatorioVisible = Visibility.Visible;
                            ProcedimientosMedicosVisible = Visibility.Visible;
                            MenuGuardarEnabled = true;
                            CargarDatosImputadoSeleccionado();

                            #region ENFERMEDADES
                            var enfermedades = new List<NOTA_MEDICA_ENFERMEDAD>();
                            if (SelectHospitalizado.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any())
                                foreach (var item in SelectHospitalizado.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD)
                                    enfermedades.Add(item);
                            if (SelectHospitalizado.ATENCION_MEDICA.Any() ? SelectHospitalizado.ATENCION_MEDICA.Any(a => a.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any()) : false)
                                foreach (var item in SelectHospitalizado.ATENCION_MEDICA.SelectMany(s => s.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD))
                                    enfermedades.Add(item);

                            if (SelectHospitalizado.NOTA_MEDICA != null ?
                                SelectHospitalizado.NOTA_MEDICA.CANALIZACION != null ?
                                    SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null ?
                                        SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any() ?
                                            SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SOL_INTERCONSULTA_INTERNA.Any() ?
                                                a.SOL_INTERCONSULTA_INTERNA.Any(an => an.ESPECIALISTA_CITA.Any(any => any.ATENCION_CITA != null ?
                                                    any.ATENCION_CITA.ATENCION_MEDICA != null ?
                                                        any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                                            any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any()
                                                        : false
                                                    : false
                                                : false))
                                            : false)
                                        : false
                                    : false
                                : false
                            : false)
                                foreach (var item in SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.SelectMany(s => s.SOL_INTERCONSULTA_INTERNA).SelectMany(s => s.ESPECIALISTA_CITA)
                                    .Where(w => w.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any()).SelectMany(s => s.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD))
                                    enfermedades.Add(item);


                            ListEnfermedades = new ObservableCollection<NOTA_MEDICA_ENFERMEDAD>(enfermedades);
                            #endregion

                            #region RECETAS

                            #region HISTORIAL RECETAS
                            var recets = new List<RECETA_MEDICA_DETALLE>();
                            ListHistorialRecetas = new ObservableCollection<RecetaMedica>();
                            if (SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.Any() ? SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.Any(s => s.RECETA_MEDICA_DETALLE.Any()) : false)
                                foreach (var item in SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.SelectMany(s => s.RECETA_MEDICA_DETALLE))
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        recets.Add(item);
                                    }));
                            if (SelectHospitalizado.ATENCION_MEDICA.Any() ?
                                SelectHospitalizado.ATENCION_MEDICA.Any(a => a.NOTA_MEDICA != null ?
                                    a.NOTA_MEDICA.RECETA_MEDICA.Any(rm => rm.RECETA_MEDICA_DETALLE.Any())
                                : false)
                            : false)
                                foreach (var item in SelectHospitalizado.ATENCION_MEDICA.Where(w => w.NOTA_MEDICA != null).SelectMany(s => s.NOTA_MEDICA.RECETA_MEDICA).SelectMany(s => s.RECETA_MEDICA_DETALLE))
                                    if (!recets.Any(a => a.ID_ATENCION_MEDICA == item.ID_ATENCION_MEDICA ?
                                        a.ID_CENTRO_UBI == item.ID_CENTRO_UBI ?
                                            a.ID_PRODUCTO == item.ID_PRODUCTO ?
                                                a.ID_PRESENTACION_MEDICAMENTO == item.ID_PRESENTACION_MEDICAMENTO ?
                                                    a.ID_FOLIO == item.ID_FOLIO
                                                : false
                                            : false
                                        : false
                                    : false))
                                        Application.Current.Dispatcher.Invoke((Action)(delegate
                                           {
                                               recets.Add(item);
                                           }));

                            if (SelectHospitalizado.NOTA_MEDICA != null ?
                                SelectHospitalizado.NOTA_MEDICA.CANALIZACION != null ?
                                    SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null ?
                                        SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any() ?
                                            SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SOL_INTERCONSULTA_INTERNA.Any() ?
                                                a.SOL_INTERCONSULTA_INTERNA.Any(an => an.ESPECIALISTA_CITA.Any(any => any.ATENCION_CITA != null ?
                                                    any.ATENCION_CITA.ATENCION_MEDICA != null ?
                                                        any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                                            any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.RECETA_MEDICA.Any(rm => rm.RECETA_MEDICA_DETALLE.Any())
                                                        : false
                                                    : false
                                                : false))
                                            : false)
                                        : false
                                    : false
                                : false
                            : false)
                                foreach (var item in SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.SelectMany(s => s.SOL_INTERCONSULTA_INTERNA).SelectMany(s => s.ESPECIALISTA_CITA)
                                    .Where(w => w.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.RECETA_MEDICA.Any(a => a.RECETA_MEDICA_DETALLE.Any())).SelectMany(s => s.ATENCION_CITA.ATENCION_MEDICA.RECETA_MEDICA).
                                        SelectMany(s => s.RECETA_MEDICA_DETALLE))
                                    if (!recets.Any(a => a.ID_ATENCION_MEDICA == item.ID_ATENCION_MEDICA ?
                                        a.ID_CENTRO_UBI == item.ID_CENTRO_UBI ?
                                            a.ID_PRODUCTO == item.ID_PRODUCTO ?
                                                a.ID_PRESENTACION_MEDICAMENTO == item.ID_PRESENTACION_MEDICAMENTO ?
                                                    a.ID_FOLIO == item.ID_FOLIO
                                                : false
                                            : false
                                        : false
                                    : false))
                                        Application.Current.Dispatcher.Invoke((Action)(delegate
                                        {
                                            recets.Add(item);
                                        }));
                            if (recets != null ? recets.Any() : false)
                                ListHistorialRecetas = new ObservableCollection<RecetaMedica>(recets.OrderByDescending(o => o.RECETA_MEDICA.RECETA_FEC).Select(item => new RecetaMedica
                                {
                                    CANTIDAD = item.DOSIS,
                                    DURACION = item.DURACION,
                                    ELIMINADO = item.CANCELA_ID_ATENCION_MEDICA.HasValue,
                                    HABILITAR = false,
                                    HORA_MANANA = item.DESAYUNO == 1,
                                    HORA_NOCHE = item.CENA == 1,
                                    HORA_TARDE = item.COMIDA == 1,
                                    MEDIDA = item.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? item.PRODUCTO.ID_UNIDAD_MEDIDA.Value : 0,
                                    OBSERVACIONES = item.OBSERV,
                                    PRESENTACION = item.ID_PRESENTACION_MEDICAMENTO,
                                    PRODUCTO = item.PRODUCTO,
                                    RECETA_MEDICA_DETALLE = item,
                                }));
                            #endregion

                            #region RECETAS ACTUALES
                            /*
                            var recetas = new List<RecetaMedica>();
                            if (SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.Any() ? SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.Any(s => s.RECETA_MEDICA_DETALLE.Any(a => !a.CANCELA_ID_ATENCION_MEDICA.HasValue)) : false)
                                foreach (var s in SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.SelectMany(s => s.RECETA_MEDICA_DETALLE).Where(w => !w.CANCELA_ID_ATENCION_MEDICA.HasValue))
                                    recetas.Add(new RecetaMedica
                                    {
                                        CANTIDAD = s.DOSIS,
                                        DURACION = s.DURACION,
                                        HORA_MANANA = s.DESAYUNO == 1,
                                        HORA_NOCHE = s.CENA == 1,
                                        HORA_TARDE = s.COMIDA == 1,
                                        OBSERVACIONES = s.OBSERV,
                                        PRODUCTO = s.PRODUCTO,
                                        MEDIDA = s.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? s.PRODUCTO.ID_UNIDAD_MEDIDA.Value : 0,
                                        PRESENTACION = s.ID_PRESENTACION_MEDICAMENTO,
                                        RECETA_MEDICA_DETALLE = s,
                                        HABILITAR = false,
                                        ELIMINADO = s.CANCELA_ID_ATENCION_MEDICA.HasValue,
                                    });
                            if (SelectHospitalizado.ATENCION_MEDICA.Any() ?
                                SelectHospitalizado.ATENCION_MEDICA.Any(a => a.NOTA_MEDICA != null ?
                                    a.NOTA_MEDICA.RECETA_MEDICA.Any(rm => rm.RECETA_MEDICA_DETALLE.Any(rmd => !rmd.CANCELA_ID_ATENCION_MEDICA.HasValue))
                                : false)
                            : false)
                                foreach (var s in SelectHospitalizado.ATENCION_MEDICA.Where(w => w.NOTA_MEDICA != null).SelectMany(s => s.NOTA_MEDICA.RECETA_MEDICA).SelectMany(s => s.RECETA_MEDICA_DETALLE)
                                    .Where(w => !w.CANCELA_ID_ATENCION_MEDICA.HasValue))
                                    recetas.Add(new RecetaMedica
                                    {
                                        CANTIDAD = s.DOSIS,
                                        DURACION = s.DURACION,
                                        HORA_MANANA = s.DESAYUNO == 1,
                                        HORA_NOCHE = s.CENA == 1,
                                        HORA_TARDE = s.COMIDA == 1,
                                        OBSERVACIONES = s.OBSERV,
                                        PRODUCTO = s.PRODUCTO,
                                        MEDIDA = s.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? s.PRODUCTO.ID_UNIDAD_MEDIDA.Value : 0,
                                        PRESENTACION = s.ID_PRESENTACION_MEDICAMENTO,
                                        RECETA_MEDICA_DETALLE = s,
                                        HABILITAR = false,
                                        ELIMINADO = s.CANCELA_ID_ATENCION_MEDICA.HasValue,
                                    });
                            if (SelectHospitalizado.NOTA_MEDICA != null ?
                                SelectHospitalizado.NOTA_MEDICA.CANALIZACION != null ?
                                    SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null ?
                                        SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any() ?
                                            SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SOL_INTERCONSULTA_INTERNA.Any() ?
                                                a.SOL_INTERCONSULTA_INTERNA.Any(an => an.ESPECIALISTA_CITA.Any(any => any.ATENCION_CITA != null ?
                                                    any.ATENCION_CITA.ATENCION_MEDICA != null ?
                                                        any.ATENCION_CITA.ATENCION_MEDICA.RECETA_MEDICA.Any(rm => rm.RECETA_MEDICA_DETALLE.Any(rmd => !rmd.CANCELA_ID_ATENCION_MEDICA.HasValue))
                                                    : false
                                                : false))
                                            : false)
                                        : false
                                    : false
                                : false
                            : false)
                            {
                                foreach (var s in SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.SelectMany(s => s.SOL_INTERCONSULTA_INTERNA).SelectMany(s => s.ESPECIALISTA_CITA)
                                    .Where(w => w.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.RECETA_MEDICA.Any(a => a.RECETA_MEDICA_DETALLE.Any())).SelectMany(s => s.ATENCION_CITA.ATENCION_MEDICA.RECETA_MEDICA).
                                    SelectMany(s => s.RECETA_MEDICA_DETALLE).Where(w => !w.CANCELA_ID_ATENCION_MEDICA.HasValue))
                                    recetas.Add(new RecetaMedica
                                    {
                                        CANTIDAD = s.DOSIS,
                                        DURACION = s.DURACION,
                                        HORA_MANANA = s.DESAYUNO == 1,
                                        HORA_NOCHE = s.CENA == 1,
                                        HORA_TARDE = s.COMIDA == 1,
                                        OBSERVACIONES = s.OBSERV,
                                        PRODUCTO = s.PRODUCTO,
                                        MEDIDA = s.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? s.PRODUCTO.ID_UNIDAD_MEDIDA.Value : 0,
                                        PRESENTACION = s.ID_PRESENTACION_MEDICAMENTO,
                                        RECETA_MEDICA_DETALLE = s,
                                        HABILITAR = false,
                                        ELIMINADO = s.CANCELA_ID_ATENCION_MEDICA.HasValue,
                                    });
                            }
                            */
                            //ListRecetas = new ObservableCollection<RecetaMedica>(recetas);
                            ListRecetas = new ObservableCollection<RecetaMedica>();
                            #endregion

                            #endregion

                            #region DIETAS

                            #region HISTORIAL DIETAS
                            ListHistorialDietas = ListHistorialDietas ?? new ObservableCollection<NOTA_MEDICA_DIETA>();
                            var historialdietas = new ObservableCollection<NOTA_MEDICA_DIETA>();
                            if (SelectHospitalizado.NOTA_MEDICA != null ?
                                SelectHospitalizado.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(a => a.ESTATUS == "N")
                            : false)
                                foreach (var item in SelectHospitalizado.NOTA_MEDICA.NOTA_MEDICA_DIETA.Where(a => a.ESTATUS == "N"))
                                    historialdietas.Add(item);
                            if (SelectHospitalizado.NOTA_MEDICA != null ?
                                SelectHospitalizado.NOTA_MEDICA.CANALIZACION != null ?
                                    SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null ?
                                        SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any() ?
                                            SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SOL_INTERCONSULTA_INTERNA.Any() ?
                                                a.SOL_INTERCONSULTA_INTERNA.Any(an => an.ESPECIALISTA_CITA.Any(any => any.ATENCION_CITA != null ?
                                                    any.ATENCION_CITA.ATENCION_MEDICA != null ?
                                                        any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                                            any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(d => d.ESTATUS == "N")
                                                        : false
                                                    : false
                                                : false))
                                            : false)
                                        : false
                                    : false
                                : false
                            : false)
                                foreach (var item in SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.SelectMany(s => s.SOL_INTERCONSULTA_INTERNA).SelectMany(s => s.ESPECIALISTA_CITA)
                                    .SelectMany(s => s.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_DIETA).Where(w => w.ESTATUS == "N"))
                                    historialdietas.Add(item);
                            if (SelectHospitalizado.ATENCION_MEDICA.Any() ?
                                SelectHospitalizado.ATENCION_MEDICA.Any(a => a.NOTA_MEDICA != null ? a.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(an => an.ESTATUS == "N") : false)
                            : false)
                                foreach (var item in SelectHospitalizado.ATENCION_MEDICA.SelectMany(s => s.NOTA_MEDICA.NOTA_MEDICA_DIETA).Where(a => a.ESTATUS == "N"))
                                    historialdietas.Add(item);
                            if (historialdietas != null ? historialdietas.Any() : false)
                                ListHistorialDietas = new ObservableCollection<NOTA_MEDICA_DIETA>(historialdietas.OrderByDescending(o => o.DIETA_FEC));
                            #endregion

                            #region DIETAS ACTUALES
                            foreach (var item in ListDietas)
                            {

                                var nota = new ATENCION_MEDICA();

                                if (SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(a => a.ID_DIETA == item.DIETA.ID_DIETA && a.ESTATUS == "S")
                                : false)
                                {
                                    item.ELEGIDO = true;
                                    item.ID_ATENCION_MEDICA = SelectHospitalizado.NOTA_MEDICA.NOTA_MEDICA_DIETA.First(a => a.ID_DIETA == item.DIETA.ID_DIETA && a.ESTATUS == "S").ID_ATENCION_MEDICA;
                                    item.ID_CENTRO_UBI = SelectHospitalizado.NOTA_MEDICA.NOTA_MEDICA_DIETA.First(a => a.ID_DIETA == item.DIETA.ID_DIETA && a.ESTATUS == "S").ID_CENTRO_UBI;
                                    item.FECHA = SelectHospitalizado.NOTA_MEDICA.NOTA_MEDICA_DIETA.First(a => a.ID_DIETA == item.DIETA.ID_DIETA && a.ESTATUS == "S").DIETA_FEC;
                                    continue;
                                }
                                if (SelectHospitalizado.NOTA_MEDICA != null ?
                                    SelectHospitalizado.NOTA_MEDICA.CANALIZACION != null ?
                                        SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null ?
                                            SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any() ?
                                                SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SOL_INTERCONSULTA_INTERNA.Any() ?
                                                    a.SOL_INTERCONSULTA_INTERNA.Any(an => an.ESPECIALISTA_CITA.Any(any => any.ATENCION_CITA != null ?
                                                        any.ATENCION_CITA.ATENCION_MEDICA != null ?
                                                            any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                                                any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(d => d.ID_DIETA == item.DIETA.ID_DIETA && d.ESTATUS == "S")
                                                            : false
                                                        : false
                                                    : false))
                                                : false)
                                            : false
                                        : false
                                    : false
                                : false)
                                {
                                    nota = SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.SelectMany(s => s.SOL_INTERCONSULTA_INTERNA).SelectMany(s => s.ESPECIALISTA_CITA)
                                        .Where(w => w.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(d => d.ID_DIETA == item.DIETA.ID_DIETA && d.ESTATUS == "S"))
                                            .Select(s => s.ATENCION_CITA.ATENCION_MEDICA).FirstOrDefault();
                                    item.ELEGIDO = true;
                                    item.ID_ATENCION_MEDICA = nota.ID_ATENCION_MEDICA;
                                    item.ID_CENTRO_UBI = nota.ID_CENTRO_UBI;
                                    item.FECHA = nota.NOTA_MEDICA.NOTA_MEDICA_DIETA.First(f => f.ID_DIETA == item.DIETA.ID_DIETA && f.ESTATUS == "S").DIETA_FEC;
                                    continue;
                                }
                                if (SelectHospitalizado.ATENCION_MEDICA.Any() ?
                                    SelectHospitalizado.ATENCION_MEDICA.Any(a => a.NOTA_MEDICA != null ? a.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(an => an.ID_DIETA == item.DIETA.ID_DIETA && an.ESTATUS == "S") : false)
                                : false)
                                {
                                    nota = SelectHospitalizado.ATENCION_MEDICA.First(a => a.NOTA_MEDICA != null ?
                                        a.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(an => an.ID_DIETA == item.DIETA.ID_DIETA && an.ESTATUS == "S")
                                    : false);
                                    item.ELEGIDO = true;
                                    item.ID_ATENCION_MEDICA = nota.ID_ATENCION_MEDICA;
                                    item.ID_CENTRO_UBI = nota.ID_CENTRO_UBI;
                                    item.FECHA = nota.NOTA_MEDICA.NOTA_MEDICA_DIETA.First(f => f.ID_DIETA == item.DIETA.ID_DIETA && f.ESTATUS == "S").DIETA_FEC;
                                    continue;
                                }
                            }
                            ListDietasAux = new ObservableCollection<DietaMedica>();
                            foreach (var item in ListDietas)
                                ListDietasAux.Add(new DietaMedica
                                {
                                    ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ELEGIDO = item.ELEGIDO,
                                    DIETA = item.DIETA,
                                    FECHA = item.FECHA,
                                });
                            #endregion

                            #endregion

                            #region PROCEDIMIENTOS MEDICOS
                            var procedimientos = new List<CustomProcedimientosMedicosSeleccionados>();
                            var citas = new List<CustomCitasProcedimientosMedicos>();
                            //var agenda = string.Empty;
                            var hoy = Fechas.GetFechaDateServer;
                            if (SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.PROC_ATENCION_MEDICA.Any())
                                foreach (var s in SelectHospitalizado.NOTA_MEDICA.ATENCION_MEDICA.PROC_ATENCION_MEDICA)
                                    procedimientos.Add(new CustomProcedimientosMedicosSeleccionados
                                    {
                                        ID_PROC_MED = s.ID_PROCMED,
                                        PROC_MED_DESCR = s.PROC_MED.DESCR,
                                        OBSERV = s.OBSERV,
                                        AGENDA = s.PROC_ATENCION_MEDICA_PROG.Any(a => a.ATENCION_CITA != null ?
                                            a.ATENCION_CITA.CITA_FECHA_HORA.HasValue
                                        : false) ?
                                            s.PROC_ATENCION_MEDICA_PROG.First(f => f.ATENCION_CITA != null ?
                                                f.ATENCION_CITA.CITA_FECHA_HORA.HasValue
                                            : false).ATENCION_CITA.CITA_FECHA_HORA.Value.ToString("dd/MM/yyyy HH:mm tt")
                                        : s.REGISTRO_FEC.HasValue ?
                                            s.REGISTRO_FEC.Value.ToString("dd/MM/yyyy HH:mm tt")
                                        : string.Empty,
                                        CITAS = citas,
                                        ENABLE = false,
                                        ID_ATENCION_MEDICA = s.ID_ATENCION_MEDICA,
                                        ID_CENTRO_UBI = s.ID_CENTRO_UBI,
                                    });
                            if (SelectHospitalizado.ATENCION_MEDICA.Any(a => a.PROC_ATENCION_MEDICA.Any()))
                                foreach (var s in SelectHospitalizado.ATENCION_MEDICA.Where(w => w.NOTA_MEDICA != null).SelectMany(s => s.PROC_ATENCION_MEDICA))
                                    procedimientos.Add(new CustomProcedimientosMedicosSeleccionados
                                    {
                                        ID_PROC_MED = s.ID_PROCMED,
                                        PROC_MED_DESCR = s.PROC_MED.DESCR,
                                        OBSERV = s.OBSERV,
                                        AGENDA = s.PROC_ATENCION_MEDICA_PROG.Any(a => a.ATENCION_CITA != null ?
                                            a.ATENCION_CITA.CITA_FECHA_HORA.HasValue
                                        : false) ?
                                            s.PROC_ATENCION_MEDICA_PROG.First(f => f.ATENCION_CITA != null ?
                                                f.ATENCION_CITA.CITA_FECHA_HORA.HasValue
                                            : false).ATENCION_CITA.CITA_FECHA_HORA.Value.ToString("dd/MM/yyyy HH:mm tt")
                                        : s.REGISTRO_FEC.HasValue ?
                                            s.REGISTRO_FEC.Value.ToString("dd/MM/yyyy HH:mm tt")
                                        : string.Empty,
                                        ENABLE = false,
                                        ID_ATENCION_MEDICA = s.ID_ATENCION_MEDICA,
                                        ID_CENTRO_UBI = s.ID_CENTRO_UBI,
                                    });
                            if (SelectHospitalizado.NOTA_MEDICA != null ?
                                SelectHospitalizado.NOTA_MEDICA.CANALIZACION != null ?
                                    SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null ?
                                        SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any() ?
                                            SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SOL_INTERCONSULTA_INTERNA.Any() ?
                                                a.SOL_INTERCONSULTA_INTERNA.Any(an => an.ESPECIALISTA_CITA.Any(any => any.ATENCION_CITA != null ?
                                                    any.ATENCION_CITA.ATENCION_MEDICA != null ?
                                                        any.ATENCION_CITA.ATENCION_MEDICA.PROC_ATENCION_MEDICA.Any()
                                                    : false
                                                : false))
                                            : false)
                                        : false
                                    : false
                                : false
                            : false)
                            {
                                foreach (var s in SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.SelectMany(s => s.SOL_INTERCONSULTA_INTERNA).SelectMany(s => s.ESPECIALISTA_CITA)
                                    .SelectMany(s => s.ATENCION_CITA.ATENCION_MEDICA.PROC_ATENCION_MEDICA))
                                    procedimientos.Add(new CustomProcedimientosMedicosSeleccionados
                                    {
                                        ID_PROC_MED = s.ID_PROCMED,
                                        PROC_MED_DESCR = s.PROC_MED.DESCR,
                                        OBSERV = s.OBSERV,
                                        AGENDA = s.PROC_ATENCION_MEDICA_PROG.Any(a => a.ATENCION_CITA != null ?
                                            a.ATENCION_CITA.CITA_FECHA_HORA.HasValue
                                        : false) ?
                                            s.PROC_ATENCION_MEDICA_PROG.First(f => f.ATENCION_CITA != null ?
                                                f.ATENCION_CITA.CITA_FECHA_HORA.HasValue
                                            : false).ATENCION_CITA.CITA_FECHA_HORA.Value.ToString("dd/MM/yyyy HH:mm tt")
                                        : s.REGISTRO_FEC.HasValue ?
                                            s.REGISTRO_FEC.Value.ToString("dd/MM/yyyy HH:mm tt")
                                        : string.Empty,
                                        ENABLE = false,
                                        ID_ATENCION_MEDICA = s.ID_ATENCION_MEDICA,
                                        ID_CENTRO_UBI = s.ID_CENTRO_UBI,
                                    });
                            }

                            ListHistorialProcMeds = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>(procedimientos.Any() ? procedimientos.OrderByDescending(o => o.AGENDA).ToList() : procedimientos);
                            ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>();
                            #endregion

                            CargarPatologicos();

                            ValidacionesMedico();
                        });
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado hospitalizado.", ex);
                    }
                    break;

                #region LESION
                case "eliminar_lesion":
                    try
                    {
                        if (SelectLesionEliminar == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Debes seleccionar una lesión.");
                            return;
                        }
                        if (!ListLesiones.Any())
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Ocurrió un error con la lesión seleccionada.");
                            return;
                        }
                        ListLesiones.Remove(SelectLesionEliminar);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar la lesión.", ex);
                    }
                    break;
                case "limpiar_lesion":
                    try
                    {
                        BotonLesionEnabled = false;
                        TextDescripcionLesion = string.Empty;
                        var radioButons = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<RadioButton>(((NotaMedicaView)((ContentControl)Application.Current.Windows[0].FindName("contentControl")).Content)).ToList();
                        foreach (var item in radioButons)
                            item.IsChecked = false;
                        SelectLesion = null;
                        SelectRegion = new Nullable<short>();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar los campos de la lesión.", ex);
                    }
                    break;
                case "agregar_lesion":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        try
                        {
                            if (SelectIngreso == null)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso.");
                                }));
                                return;
                            };
                            if (string.IsNullOrEmpty(TextDescripcionLesion))
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Debes ingresar una descripción de la lesión.");
                                }));
                                return;
                            }
                            if (SelectRegion == null)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar la región donde se encuentra la lesión.");
                                }));
                                return;
                            };
                            ListLesiones = ListLesiones ?? new ObservableCollection<LesionesCustom>();
                            BotonLesionEnabled = false;
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                ListLesiones.Add(new LesionesCustom { DESCR = TextDescripcionLesion, REGION = new cAnatomiaTopografica().Obtener((int)SelectRegion) });
                                var radios = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<RadioButton>(((NotaMedicaView)((ContentControl)Application.Current.Windows[0].FindName("contentControl")).Content)).ToList();
                                foreach (var item in radios)
                                    item.IsChecked = false;
                                SelectLesion = null;
                                SelectRegion = new Nullable<short>();
                            }));
                            TextDescripcionLesion = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar los datos de la lesion.", ex);
                        }
                    });

                    break;
                #endregion

                #region HUELLA Y NIP
                case "no_acepto_compromiso":
                    HuellaCompromiso = false;
                    NotaMedica.FIRMO_CONFORMIDAD = "N";
                    clickSwitch("acepto_compromiso");
                    break;
                case "acepto_compromiso":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        try
                        {
                            if (HuellaCompromiso)
                            {
                                if (string.IsNullOrEmpty(TextNIPCartaCompromiso))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(async delegate
                                    {
                                        CartaView.Hide();
                                        var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Exito!", "Hace falta el NIP para empatar al imputado.");
                                        CartaView.Show();
                                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                    }));
                                    return;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(SelectExpediente.NIP) ? true : SelectExpediente.NIP.Trim() != TextNIPCartaCompromiso)
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(async delegate
                                        {
                                            CartaView.Hide();
                                            var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Exito!", "El NIP no concuerda con el del imputado seleccionado.");
                                            CartaView.Show();
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                        }));
                                        return;
                                    }
                                }
                            }
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                CartaView.Hide();
                            }));
                            var usr = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault();
                            var hoy = Fechas.GetFechaDateServer;

                            #region PROCEDIMIENTOS MEDICOS
                            var ProcsMeds = new List<PROC_ATENCION_MEDICA>();
                            var citas = new List<ATENCION_CITA>();
                            var ProcsProgs = new List<PROC_ATENCION_MEDICA_PROG>();
                            var idCita = 0;
                            var cit = new ATENCION_CITA();
                            var ProcAtMedProg = new PROC_ATENCION_MEDICA_PROG();
                            if (ListProcMedsSeleccionados != null)
                            {
                                foreach (var item in ListProcMedsSeleccionados.SelectMany(s => s.CITAS).OrderBy(o => o.FECHA_INICIAL))
                                {
                                    if (new cAtencionCita().ObtieneCitaOtraArea(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO,
                                        SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, item.FECHA_INICIAL, item.FECHA_FINAL, AtencionCita != null ? AtencionCita.ID_CITA : 0, GlobalVar.gCentro) != null)
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(delegate
                                        {
                                            new Dialogos().ConfirmacionDialogo("Algo pasó...", "Existe un traslape en la fecha [ " + item.FECHA_INICIAL.ToString("dd/MM/yyyy HH:mm tt") + " ].");
                                        }));
                                        return;
                                    }
                                    idCita = new cNotaMedica().GetSequence<short>("ATENCION_CITA_SEQ");
                                    cit = new ATENCION_CITA
                                    {
                                        CITA_FECHA_HORA = item.FECHA_INICIAL,
                                        CITA_HORA_TERMINA = item.FECHA_FINAL,
                                        ID_ANIO = AtencionMedica.ID_ANIO,
                                        ID_AREA = (short)enumAreas.MEDICA_PA,
                                        ID_CENTRO = AtencionMedica.ID_CENTRO,
                                        ID_IMPUTADO = AtencionMedica.ID_IMPUTADO,
                                        ID_INGRESO = AtencionMedica.ID_INGRESO,
                                        ID_RESPONSABLE = item.ENFERMERO,
                                        ID_TIPO_ATENCION = SelectTipoAtencion.ID_TIPO_ATENCION,
                                        ID_TIPO_SERVICIO = (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS,
                                        ID_USUARIO = GlobalVar.gUsr,
                                        ESTATUS = "N",
                                        ID_CITA = idCita,
                                        ID_CENTRO_UBI = GlobalVar.gCentro,
                                    };
                                    if (ProcAtMedProg != null ? !(ProcAtMedProg.ID_PROCMED == item.ID_PROC_MED && ProcAtMedProg.ID_USUARIO_ASIGNADO == GlobalVar.gUsr && ProcAtMedProg.ID_CITA == idCita) : false)
                                    {
                                        ProcAtMedProg = new PROC_ATENCION_MEDICA_PROG
                                        {
                                            ESTATUS = "N",
                                            ID_PROCMED = item.ID_PROC_MED,
                                            ID_USUARIO_ASIGNADO = GlobalVar.gUsr,
                                            ID_CITA = idCita,
                                            ID_CENTRO_UBI = GlobalVar.gCentro,
                                            ID_CENTRO_CITA = GlobalVar.gCentro,
                                            PROC_ATENCION_MEDICA = new PROC_ATENCION_MEDICA
                                            {
                                                ID_PROCMED = item.ID_PROC_MED,
                                                ID_USUARIO = GlobalVar.gUsr,
                                                OBSERV = ListProcMedsSeleccionados.Any(f => f.ID_PROC_MED == item.ID_PROC_MED) ? ListProcMedsSeleccionados.First(f => f.ID_PROC_MED == item.ID_PROC_MED).OBSERV : string.Empty,
                                                REGISTRO_FEC = hoy,
                                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                            }
                                        };
                                        cit.PROC_ATENCION_MEDICA_PROG.Add(ProcAtMedProg);
                                    }
                                    citas.Add(cit);
                                }
                            }
                            #endregion

                            #region MUJERES
                            Mujeres = null;
                            if (NotaMujeres)
                            {
                                Mujeres = new HISTORIA_CLINICA_GINECO_OBSTRE
                                {
                                    ABORTO = MujeresAuxiliar != null ? MujeresAuxiliar.ABORTO == TextAbortos ? null : TextAbortos : TextAbortos,
                                    ABORTO_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.ABORTO == TextAbortos ? null : "S" : "S",
                                    ANIOS_RITMO = MujeresAuxiliar != null ? MujeresAuxiliar.ANIOS_RITMO == TextAniosRitmo ? null : TextAniosRitmo : TextAniosRitmo,
                                    ANIOS_RITMO_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.ANIOS_RITMO == TextAniosRitmo ? null : "S" : "S",
                                    CESAREA = MujeresAuxiliar != null ? MujeresAuxiliar.CESAREA == TextCesareas ? null : TextCesareas : TextCesareas,
                                    CESAREA_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.CESAREA == TextCesareas ? null : "S" : "S",
                                    CONTROL_PRENATAL = MujeresAuxiliar != null ?
                                        MujeresAuxiliar.CONTROL_PRENATAL == (ControlPreNatal == 0 ?
                                            "S"
                                        : ControlPreNatal == 1 ?
                                            "N"
                                        : null) ?
                                            null
                                        : ControlPreNatal == 0 ?
                                            "S"
                                        : ControlPreNatal == 1 ?
                                            "N"
                                        : null
                                    : null,
                                    CONTROL_PRENATAL_MODIFICADO = MujeresAuxiliar != null ?
                                        MujeresAuxiliar.CONTROL_PRENATAL == (ControlPreNatal == 0 ?
                                            "S"
                                        : ControlPreNatal == 1 ?
                                            "N"
                                        : null) ?
                                            null
                                        : "S"
                                    : null,
                                    ID_CONTROL_PRENATAL = MujeresAuxiliar != null ?
                                        MujeresAuxiliar.ID_CONTROL_PRENATAL == SelectControlPreNatal ?
                                            null
                                        : SelectControlPreNatal == -1 ? null : SelectControlPreNatal
                                    : SelectControlPreNatal == -1 ? null : SelectControlPreNatal,
                                    DEFORMACION = MujeresAuxiliar != null ? MujeresAuxiliar.DEFORMACION == TextDeformacionesOrganicas ? null : TextDeformacionesOrganicas : TextDeformacionesOrganicas,
                                    DEFORMACION_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.DEFORMACION == TextDeformacionesOrganicas ? null : "S" : "S",
                                    EMBARAZO = MujeresAuxiliar != null ? MujeresAuxiliar.EMBARAZO == TextEmbarazos ? new Nullable<short>() : TextEmbarazos : TextEmbarazos,
                                    EMBARAZO_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.EMBARAZO == TextEmbarazos ? null : "S" : "S",
                                    FECHA_PROBABLE_PARTO = MujeresAuxiliar != null ? MujeresAuxiliar.FECHA_PROBABLE_PARTO == FechaProbParto ? new Nullable<DateTime>() : FechaProbParto : FechaProbParto,
                                    FECHA_PROBABLE_PARTO_MOD = MujeresAuxiliar != null ? MujeresAuxiliar.FECHA_PROBABLE_PARTO == FechaProbParto ? null : "S" : "S",
                                    PARTO = MujeresAuxiliar != null ? MujeresAuxiliar.PARTO == TextPartos ? new Nullable<short>() : TextPartos : TextPartos,
                                    PARTO_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.PARTO == TextPartos ? null : "S" : "S",
                                    ULTIMA_MENSTRUACION_FEC = MujeresAuxiliar != null ? MujeresAuxiliar.ULTIMA_MENSTRUACION_FEC == FechaUltimaMenstruacion ? new Nullable<DateTime>() : FechaUltimaMenstruacion : FechaUltimaMenstruacion,
                                    ULTIMA_MENS_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.ULTIMA_MENSTRUACION_FEC == FechaUltimaMenstruacion ? null : "S" : "S",
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    FUENTE = "NM",
                                    MOMENTO_REGISTRO = "DI",
                                    REGISTRO_FEC = hoy,
                                };
                            }
                            #endregion

                            if (SignosVitales != null)// ?
                            //    new cNotaMedica().InsertarNotaMedicaTransaccion(Fechas.GetFechaDateServer, SignosVitales, NotaMedica, AtencionMedica, AtencionCita, NotaUrgencia, NotaInterconsulta, NotaPostOperatoria,
                            //        NotaPreAnestecica, NotaPreOperatoria, NotaTraslado, NotaEvolucion, Enfermedades, certificado, lesiones, dietas, Excarcelacion, TrasladoDetalle, patologicos, AtencionCitaSeguimiento,
                            //        RecetaMedica, RecetaMedicaDetalle, citas) :
                            //    new cNotaMedica().ActualizarNotaMedicaTransaccion(Fechas.GetFechaDateServer, NotaMedica, AtencionMedica, AtencionCita, NotaUrgencia, NotaInterconsulta, NotaPostOperatoria,
                            //       NotaPreAnestecica, NotaPreOperatoria, NotaTraslado, NotaEvolucion, Enfermedades, certificado, lesiones, dietas, Excarcelacion, TrasladoDetalle, patologicos, AtencionCitaSeguimiento,
                            //       RecetaMedica, RecetaMedicaDetalle, citas))
                            {
                                MenuGuardarEnabled = false;
                                Application.Current.Dispatcher.Invoke((Action)(async delegate
                                {
                                    CartaView.Close();
                                    CartaView = null;
                                    HuellaCompromiso = false;
                                    var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Exito!", "La nota ha sido guardada correctamente.");
                                    if (certificado == null)
                                        clickSwitch("limpiar_menu");
                                    else
                                    {
                                        if (AtencionMedica.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && AtencionMedica.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO)
                                        {
                                        }
                                        MenuHClinicaEnabled = true;
                                        BotonRegresarVisible = Visibility.Visible;
                                        SignosVitalesReadOnly = true;
                                    }
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                }));
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            }
                        }
                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                CartaView.Close();
                                CartaView = null;
                            }));
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
                        }
                    });
                    break;
                case "buscar_huella":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    HuellaWindow = new BuscarPorHuellaYNipView();
                    HuellaWindow.DataContext = this;
                    ConstructorHuella(0);
                    HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                    //HuellaWindow.Closed += HuellaClosed;
                    HuellaWindow.ShowDialog();
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
                        _IsSucceed = true;
                        var _error_tipo = 0;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                if (SelectRegistro == null)
                                {
                                    _error_tipo = 1;
                                    return;
                                }
                                SelectExpediente = SelectRegistro.Imputado;
                                if (SelectExpediente.INGRESO.Count == 0)
                                {
                                    SelectExpediente = null;
                                    SelectIngreso = null;
                                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                }
                                if (ParametroEstatusInactivos.Any(a => a.HasValue ? a.Value == SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO : false))
                                {
                                    _error_tipo = 2;
                                    return;
                                }
                                if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO.HasValue ?
                                    SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro : false)
                                {
                                    SelectExpediente = null;
                                    SelectIngreso = null;
                                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                    _error_tipo = 3;
                                    return;
                                }
                                AtencionSeleccionada = true;
                                CitaNoExistente = false;
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                //SeleccionarImputado();
                                //HideGroupBox();
                                //ObtenerSignosVitales();
                                //CargarPatologicos();
                                if (IsEnfermero)
                                {
                                    //HideGroupBox();
                                    NotaMedicaVisible = Visibility.Visible;
                                    SignosVitalesVisible = Visibility.Visible;
                                    TipoNotaEnabled = true;
                                    AtencionTipoEnabled = false;
                                    if (CitaNoExistente)
                                    {
                                        SelectTipoAtencion = ListAtencionTipo.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA);
                                        CargarDatosTipoServicio((short)enumAtencionTipo.CONSULTA_MEDICA);
                                        SelectTipoServicio = ListTipoServicio.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && f.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CITA_MEDICA);
                                    }
                                    else { }
                                }
                                else if (IsMedico)
                                {
                                    NotaMedicaVisible = Visibility.Visible;
                                    SignosVitalesVisible = Visibility.Visible;
                                    DiagnosticoVisible = Visibility.Visible;
                                    TratamientoVisible = Visibility.Visible;
                                    TipoNotaEnabled = true;
                                    AtencionTipoEnabled = true;
                                }
                                else if (IsDentista)
                                {
                                    NotaMedicaVisible = Visibility.Visible;
                                    SignosVitalesVisible = Visibility.Collapsed;
                                    DiagnosticoVisible = Visibility.Collapsed;
                                    TratamientoVisible = Visibility.Collapsed;
                                    TipoNotaEnabled = true;
                                    AtencionTipoEnabled = true;
                                    TratamientoDentalVisible = Visibility.Visible;

                                    SelectTipoAtencion = ListAtencionTipo.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL);
                                    CargarDatosTipoServicio((short)enumAtencionTipo.CONSULTA_DENTAL);
                                    SelectTipoServicio = ListTipoServicio.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL && f.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CITA_MEDICA_DENTAL);
                                }
                                else
                                {
                                    TipoNotaEnabled = false;
                                    TratamientoVisible = !IsEnfermero ? Visibility.Visible : Visibility.Collapsed;
                                    DiagnosticoVisible = !IsEnfermero ? Visibility.Visible : Visibility.Collapsed;
                                }
                                if (SelectCitaMedicaImputado != null)
                                {
                                    SelectTipoAtencion = ListAtencionTipo.First(f => f.ID_TIPO_ATENCION == SelectCitaMedicaImputado.ID_TIPO_ATENCION);
                                    CargarDatosTipoServicio(SelectCitaMedicaImputado.ID_TIPO_ATENCION.Value);
                                    SelectTipoServicio = ListTipoServicio.First(f => f.ID_TIPO_ATENCION == SelectCitaMedicaImputado.ID_TIPO_ATENCION && f.ID_TIPO_SERVICIO == SelectCitaMedicaImputado.ID_TIPO_SERVICIO);
                                    TipoNotaEnabled = false;
                                    AtencionTipoEnabled = false;
                                    CambiarMedicoVisible = Visibility.Visible;

                                    if (IsMedico || IsDentista)
                                    {
                                        if (SelectCitaMedicaImputado.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                                               SelectCitaMedicaImputado.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_MEDICA
                                           : SelectCitaMedicaImputado.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ?
                                               SelectCitaMedicaImputado.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_DENTAL
                                           : false)
                                        {
                                            StaticSourcesViewModel.SourceChanged = false;
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                HuellaWindow.Close();
                                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new HistoriaClinicaView();
                                                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext =
                                                    new HistoriaClinicaViewModel(SelectCitaMedicaImputado.ID_IMPUTADO, SelectCitaMedicaImputado.ID_ANIO, SelectCitaMedicaImputado.ID_INGRESO, SelectCitaMedicaImputado.ID_CENTRO);
                                            }));
                                            return;
                                        }
                                    }
                                }
                                else if (SelectExcarcelacionImputado != null || SelectTrasladoImputado != null)
                                {
                                    AtencionTipoEnabled = false;
                                    TipoNotaEnabled = false;
                                    SelectTipoAtencion = ListAtencionTipo.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA);
                                    CargarDatosTipoServicio((short)enumAtencionTipo.CONSULTA_MEDICA);
                                    SelectTipoServicio = ListTipoServicio.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && f.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_INTEGRIDAD_FISICA);
                                }
                                else if (SelectUrgenciaImputado != null || SelectSancionImputado != null)
                                {
                                    SelectTipoAtencion = ListAtencionTipo.First(f => f.ID_TIPO_ATENCION == ((IsMedico || IsEnfermero) ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL));
                                    CargarDatosTipoServicio(((IsMedico || IsEnfermero) ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL));
                                    SelectTipoServicio = ListTipoServicio.First(f => f.ID_TIPO_ATENCION == ((IsMedico || IsEnfermero) ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL) &&
                                        f.ID_TIPO_SERVICIO == ((IsMedico || IsEnfermero) ? (short)enumAtencionServicio.CITA_MEDICA : (short)enumAtencionServicio.CITA_MEDICA_DENTAL));
                                }
                                else if (SelectIngresoConsulta != null)
                                {
                                    SelectTipoAtencion = (IsMedico || IsEnfermero) ?
                                        ListAtencionTipo.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA)
                                    : ListAtencionTipo.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL);
                                    CargarDatosTipoServicio((IsMedico || IsEnfermero) ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL);
                                    SelectTipoServicio = (IsMedico || IsEnfermero) ?
                                        (ListTipoServicio.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA && f.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO))
                                    : ListTipoServicio.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL && f.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO);
                                }
                                //INSERTAR EN INGRESO UBICACION
                                var ingresoUbicacion = new cIngresoUbicacion().Insertar(new INGRESO_UBICACION
                                {
                                    ID_AREA = (short)enumAreas.MEDICA_PA,
                                    MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                    ESTATUS = 2,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                }, false, false, new Nullable<int>(), false);
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    HuellaWindow.Close();
                                    MenuBuscarEnabled = false;
                                    MenuGuardarEnabled = true;
                                    MenuArchivosEnabled = true;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                }));

                            }
                            catch (Exception ex)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
                                }));
                            }
                        });
                        switch (_error_tipo)
                        {
                            case 1:
                                await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "Debes seleccionar un imputado.");
                                break;
                            case 2:
                                await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                                break;
                            case 3:
                                await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "El ingreso seleccionado no pertenece a su centro.");
                                break;
                        }
                        if (_error_tipo != 0)
                        {
                            HuellaWindow.Show();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            return;
                        }
                        if (SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO == -1 : true)
                            ElementosDisponibles = false;
                        else
                        {
                            CambiarMedicoVisible = IsEnfermero ?
                                Visibility.Collapsed
                            : SelectTipoAtencion.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                                !(SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_MEDICA) ?
                                    Visibility.Visible
                                : Visibility.Collapsed
                            : SelectTipoAtencion.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ?
                                !(SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_MEDICA) ?
                                    Visibility.Visible
                                : Visibility.Collapsed
                            : Visibility.Collapsed;
                            ElementosDisponibles = true;
                        }
                        CheckCambiarMedico = false;
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    catch (Exception ex)
                    {
                        HuellaWindow.Show();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
                    }
                    break;
                case "cerrar_buscar_huella_2":
                case "cerrar_buscar_huella":
                    if (HuellaWindow != null ? HuellaWindow.IsLoaded : false)
                        HuellaWindow.Close();
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
                            var auxiliar = new List<ResultadoBusquedaBiometrico>();
                            ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                            if (SelectExpediente == null)
                                SelectExpediente = SelectIngresoConsulta != null ? SelectIngresoConsulta.IMPUTADO : null;
                            if (string.IsNullOrEmpty(SelectExpediente.NIP) ? false : SelectExpediente.NIP.Trim() == TextNipBusqueda)
                            {
                                var ingresobiometrico = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                                if (ingresobiometrico != null ? ingresobiometrico.INGRESO_BIOMETRICO.Any() : false)
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
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar realizar la búsqueda por nip.", ex);
                        }
                    });
                    HuellaWindow.Show();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    break;
                #endregion

                #region BUSCAR IMPUTADOS
                case "nueva_busqueda":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = new int?();
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    break;
                case "buscar_visible":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    var aux = SelectAtencionMedicaAux;
                    SelectAtencionMedica = aux;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_menu":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                        SelectExpediente = new IMPUTADO();
                        EmptyExpedienteVisible = true;
                        ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                        FolioBuscar = AnioBuscar = null;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        var am = SelectAtencionMedica;
                        SelectAtencionMedicaAux = am;
                    });
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    try
                    {
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso.");
                            return;
                        };
                        if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        };
                        if (ParametroEstatusInactivos.Any(a => a.HasValue ? a.Value == SelectIngreso.ID_ESTATUS_ADMINISTRATIVO : false))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un ingreso activo.");
                            return;
                        }
                        SelectCitaMedicaImputado = null;
                        SelectAtencionMedica = null;
                        SelectAtencionMedicaAux = null;
                        AtencionSeleccionada = false;
                        await TaskEx.Delay(100);
                        CitaNoExistente = true;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        HuellaWindow = new BuscarPorHuellaYNipView();
                        HuellaWindow.DataContext = this;
                        ConstructorHuella(0);
                        HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                        //HuellaWindow.Closed += HuellaClosed;
                        HuellaWindow.ShowDialog();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
                    }
                    break;
                #endregion

                #region DENTAL
                case "agregar_tratamiento_dental":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        try
                        {
                            if (ListOdontograma != null ? !ListOdontograma.Any() : true)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar una parte del odontograma para poder agregarla a la lista de tratamientos.");
                                }));
                                return;
                            }
                            if (SelectTipoTratamiento != null ? SelectTipoTratamiento.ID_TIPO_TRATA <= 0 : true)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Debes escribir el tratamiento realizado para poder agregarla a la lista.");
                                }));
                                return;
                            }
                            if (SelectTratamientoOdonto != null ? SelectTratamientoOdonto.ID_TRATA <= 0 : true)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Debes escribir el tratamiento realizado para poder agregarla a la lista.");
                                }));
                                return;
                            }
                            ListTratamientoDental = ListTratamientoDental ?? new List<CustomTratamientoDental>();
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                ListTratamientoDental.Add(new CustomTratamientoDental
                                {
                                    FECHA = Fechas.GetFechaDateServer,
                                    OBSERV = TextTratamientoDental,
                                    SELECCIONADOS = ListOdontograma,
                                    ID_TIPO_TRATAMIENTO = SelectTipoTratamiento.ID_TIPO_TRATA,
                                    ID_TRATAMIENTO = SelectTratamientoOdonto.ID_TRATA,
                                    TIPO = SelectTipoTratamiento.DESCR,
                                    TRATAMIENTO = SelectTratamientoOdonto.DESCR,
                                });
                                ListTratamientoDental = ListTratamientoDental.Select(s => s).ToList();
                                foreach (var item in ListCheckBoxOdontograma)
                                    item.IsChecked = false;
                            }));
                            SelectTipoTratamiento = ListTipoTratamiento.First(f => f.ID_TIPO_TRATA == -1);
                            TextTratamientoDental = string.Empty;
                            ListOdontograma = new List<CustomOdontograma>();
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al intentar agregar un nuevo tratamiento dental.", ex);
                        }
                    });
                    break;
                case "limpiar_tratamiento_dental":
                    try
                    {
                        SelectTipoTratamiento = null;
                        SelectTratamientoOdonto = null;
                        TextTratamientoDental = string.Empty;
                        ListOdontograma = new List<CustomOdontograma>();
                        foreach (var item in ListCheckBoxOdontograma)
                            item.IsChecked = false;
                        TextDentalEnable = true;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar la seccion de tratamiento dental.", ex);
                    }
                    break;
                case "eliminar_tratamiento_dental":
                    try
                    {
                        if (SelectTratamientoDental == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un tratamiento.");
                            return;
                        }
                        if (!ListTratamientoDental.Any())
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "No hay tratamientos en la lista.");
                            return;
                        }
                        if (!ListTratamientoDental.Any(a => a.OBSERV == SelectTratamientoDental.OBSERV && a.FECHA == SelectTratamientoDental.FECHA &&
                            a.ID_TIPO_TRATAMIENTO == SelectTratamientoDental.ID_TIPO_TRATAMIENTO && a.ID_TRATAMIENTO == SelectTratamientoDental.ID_TRATAMIENTO))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un tratamiento correcto.");
                            return;
                        }
                        if (await new Dialogos().ConfirmarEliminar("Validacion", "Esta seguro que desea eliminar este registro antes de guardarlo?") == 1)
                        {
                            ListTratamientoDental.Remove(ListTratamientoDental.First(a => a.OBSERV == SelectTratamientoDental.OBSERV && a.FECHA == SelectTratamientoDental.FECHA &&
                            a.ID_TIPO_TRATAMIENTO == SelectTratamientoDental.ID_TIPO_TRATAMIENTO && a.ID_TRATAMIENTO == SelectTratamientoDental.ID_TRATAMIENTO));
                            ListTratamientoDental = ListTratamientoDental.Select(s => s).ToList();
                            SelectTipoTratamiento = null;
                            SelectTratamientoOdonto = null;
                            TextTratamientoDental = string.Empty;
                            ListOdontograma = new List<CustomOdontograma>();
                            foreach (var item in ListCheckBoxOdontograma)
                                item.IsChecked = false;
                            TextDentalEnable = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar la sección de tratamiento dental.", ex);
                    }
                    break;
                #endregion

                #region PATOLOGIA
                case "guardar_ampliar_descripcion":
                    try
                    {
                        if (RecetaHistoria)
                        {
                            ListRecetas.First(a => a.PRODUCTO.ID_PRODUCTO == SelectReceta.PRODUCTO.ID_PRODUCTO).OBSERVACIONES = TextAmpliarDescripcion;
                            SelectReceta.OBSERVACIONES = TextAmpliarDescripcion;
                            ListRecetas = new ObservableCollection<RecetaMedica>(ListRecetas.OrderBy(o => o.PRODUCTO.DESCRIPCION));
                        }
                        else if (ProcedimientoMedico)
                        {
                            ListProcMedsSeleccionados.First(f => f.PROC_MED_DESCR == SelectProcMedSeleccionado.PROC_MED_DESCR).OBSERV = TextAmpliarDescripcion;
                            SelectProcMedSeleccionado.OBSERV = TextAmpliarDescripcion;
                            ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>(ListProcMedsSeleccionados.OrderBy(o => o.PROC_MED_DESCR));
                        }
                        else
                        {
                            ListCondensadoPatologicosAux.First(a => a.HISTORIA_CLINICA_PATOLOGICOS.ID_CONSEC == SelectedCondensadoPato.HISTORIA_CLINICA_PATOLOGICOS.ID_CONSEC ?
                                a.PATOLOGICO_CAT.ID_PATOLOGICO == SelectedCondensadoPato.HISTORIA_CLINICA_PATOLOGICOS.ID_PATOLOGICO
                            : false).OBSERVACIONES = TextAmpliarDescripcion;
                            SelectedCondensadoPato.OBSERVACIONES = TextAmpliarDescripcion;
                            LstCondensadoPatologicos = new ObservableCollection<HistoriaclinicaPatologica>(ListCondensadoPatologicosAux.OrderBy(o => !o.RECUPERADO).ThenBy(t => t.PATOLOGICO_CAT.DESCR));
                        }
                        TextAmpliarDescripcion = string.Empty;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la patología.", ex);
                    }
                    break;

                case "cancelar_ampliar_descripcion":
                    TextAmpliarDescripcion = string.Empty;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;
                case "agregar_patol":
                    try
                    {
                        if (SelectedPatologico == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar una patología para poder agregarla a la lista.");
                            return;
                        }
                        if (LstCondensadoPatologicos != null ? LstCondensadoPatologicos.Any(a => a.PATOLOGICO_CAT.ID_PATOLOGICO == SelectedPatologico.PATOLOGICO_CAT.ID_PATOLOGICO && !a.RECUPERADO) : false)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La patología seleccionada ya se encuentra en la lista.");
                            return;
                        }
                        ListCondensadoPatologicosAux = ListCondensadoPatologicosAux ?? new ObservableCollection<HistoriaclinicaPatologica>();
                        var ahora = Fechas.GetFechaDateServer;
                        ListCondensadoPatologicosAux.Add(new HistoriaclinicaPatologica
                        {
                            HISTORIA_CLINICA_PATOLOGICOS = new HISTORIA_CLINICA_PATOLOGICOS
                            {
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_CONSEC = 0,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_PATOLOGICO = SelectedPatologico.PATOLOGICO_CAT.ID_PATOLOGICO,
                                MOMENTO_DETECCION = SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                                    SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO ?
                                        "EI"
                                    : "DI"
                                : "DI",
                                OTROS_DESCRIPCION = string.Empty,
                                OBSERVACIONES = string.Empty,
                                RECUPERADO = "N",
                                REGISTRO_FEC = ahora,
                                PATOLOGICO_CAT = SelectedPatologico.PATOLOGICO_CAT,
                                ID_NOPATOLOGICO = (short)(ListCondensadoPatologicosAux.Count(c => c.HISTORIA_CLINICA_PATOLOGICOS.ID_PATOLOGICO == SelectedPatologico.PATOLOGICO_CAT.ID_PATOLOGICO) + 1)
                            },
                            OBSERVACIONES = string.Empty,
                            PATOLOGICO_CAT = SelectedPatologico.PATOLOGICO_CAT,
                            RECUPERADO = false,
                            DESHABILITADO = true,
                            REGISTRO_FEC = ahora,
                            ELIMINABLE = true
                        });
                        LstCondensadoPatologicos = new ObservableCollection<HistoriaclinicaPatologica>(ListCondensadoPatologicosAux.OrderBy(o => !o.RECUPERADO).ThenBy(t => t.PATOLOGICO_CAT.DESCR));
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la patología.", ex);
                    }
                    break;
                case "remove_patol":
                    try
                    {
                        if (SelectedCondensadoPato == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar una patología para poder quitarla de la lista.");
                            return;
                        }
                        if (!SelectedCondensadoPato.ELIMINABLE)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "No puedes quitar una patología que ya ha sido guardada.");
                            return;
                        }
                        if (LstCondensadoPatologicos.Any(a => !SelectedCondensadoPato.DESHABILITADO ?
                            a.HISTORIA_CLINICA_PATOLOGICOS.ID_CONSEC == SelectedCondensadoPato.HISTORIA_CLINICA_PATOLOGICOS.ID_CONSEC ?
                                a.PATOLOGICO_CAT.ID_PATOLOGICO == SelectedCondensadoPato.HISTORIA_CLINICA_PATOLOGICOS.ID_PATOLOGICO
                            : false
                        : false))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La patología seleccionada no puede ser eliminada de la lista.");
                            return;
                        }
                        ListCondensadoPatologicosAux = ListCondensadoPatologicosAux ?? new ObservableCollection<HistoriaclinicaPatologica>();
                        ListCondensadoPatologicosAux.Remove(ListCondensadoPatologicosAux.First(f => f.PATOLOGICO_CAT.ID_PATOLOGICO == SelectedCondensadoPato.HISTORIA_CLINICA_PATOLOGICOS.ID_PATOLOGICO &&
                            f.REGISTRO_FEC == SelectedCondensadoPato.REGISTRO_FEC));
                        LstCondensadoPatologicos = new ObservableCollection<HistoriaclinicaPatologica>(ListCondensadoPatologicosAux.OrderBy(o => !o.RECUPERADO).ThenBy(t => t.PATOLOGICO_CAT.DESCR));
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la patología.", ex);
                    }
                    break;
                #endregion

                #region ARCHIVOS MEDICOS
                case "ver_documento_resultado_servicios":
                    if (SelectedResultadoSinArchivo == null)
                        return;

                    if (string.IsNullOrEmpty(SelectedResultadoSinArchivo.ExtensionArchivo))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Verifique la extensión del archivo seleccionado");
                        return;
                    }

                    //DescargaArchivo(SelectedResultadoSinArchivo);
                    break;
                case "cambio_tipo_serv_aux":
                    try
                    {
                        //CargarSubTipo_Servicios_Auxiliares(SelectedTipoServAux);
                        SelectedSubtipoServAux = -1;
                        SelectedDiagnPrincipal = -1;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de los archivos medicos.", ex);
                    }
                    break;
                case "cambio_subtipo_serv_aux":
                    try
                    {
                        //CargarDiagnosticosPrincipal(SelectedSubtipoServAux);
                        SelectedDiagnPrincipal = -1;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de los archivos medicos.", ex);
                    }
                    break;
                case "menu_archivos_medicos":
                    try
                    {
                        if (!PConsultar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        ArchivosMedicos = new ArchivosMedicosView();
                        ArchivosMedicos.DataContext = this;
                        ArchivosMedicos.Owner = PopUpsViewModels.MainWindow;
                        //ArchivosMedicos.Closed += ArchivosMedicosClosed;
                        LstCustomizadaSinArchivos = SelectIngreso.SERVICIO_AUXILIAR_RESULTADO.Any() ? new ObservableCollection<CustomGridSinBytes>(SelectIngreso.SERVICIO_AUXILIAR_RESULTADO.Select(item => new CustomGridSinBytes
                        {
                            FechaRegistro = item.REGISTRO_FEC.HasValue ? item.REGISTRO_FEC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                            IdResult = item.ID_SA_RESULTADO,
                            ServicioAuxiliar = item.ID_SERV_AUX.HasValue ? !string.IsNullOrEmpty(item.SERVICIO_AUX_DIAG_TRAT.DESCR) ? item.SERVICIO_AUX_DIAG_TRAT.DESCR.Trim() : string.Empty : string.Empty,
                            NombreUsuario = item.USUARIO != null ? item.USUARIO.EMPLEADO != null ? item.USUARIO.EMPLEADO.PERSONA != null ?
                                string.Format("{0} {1} {2}",
                                    !string.IsNullOrEmpty(item.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? item.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(item.USUARIO.EMPLEADO.PERSONA.PATERNO) ? item.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(item.USUARIO.EMPLEADO.PERSONA.MATERNO) ? item.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty)
                                     : string.Empty : string.Empty : string.Empty,
                            VisibleDocumentoResult = item.CAMPO_BLOB != null ? item.CAMPO_BLOB.Any() ? true : false : false,
                            ExtensionArchivo = item.ID_FORMATO.HasValue ? !string.IsNullOrEmpty(item.FORMATO_DOCUMENTO.DESCR) ? item.FORMATO_DOCUMENTO.DESCR.Trim() : string.Empty : string.Empty
                        })) : new ObservableCollection<CustomGridSinBytes>();
                        EmptyResultados = LstCustomizadaSinArchivos.Count <= 0;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        ArchivosMedicos.ShowDialog();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de los archivos medicos.", ex);
                    }
                    break;
                case "buscar_result_existentes":
                    try
                    {
                        if (ArchivosMedicos == null) return;
                        ArchivosMedicos.Hide();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        if (SelectIngreso == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Seleccione un ingreso válido.");
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            ArchivosMedicos.Show();
                            return;
                        }
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            //BuscarResultadosExistentes();
                        });
                        ArchivosMedicos.Show();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar los archivos medicos existentes.", ex);
                    }
                    break;
                #endregion

                #region PROCEDIMIENTOS MEDICOS
                case "cancelar_procedimiento_medico":
                    try
                    {
                        CancelarProcMedWindow.Hide();
                        if (SelectProcMedEnCitaParaAgendar != null ? SelectProcMedEnCitaParaAgendar.ID_PROCMED <= 0 : false)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes seleccionar un procedimiento médico válido para poder cancelarlo.");
                            CancelarProcMedWindow.Show();
                            return;
                        }
                        if (SelectMotivoCancelarProcMed != null ? SelectMotivoCancelarProcMed.ID_ACMOTIVO <= 0 : false)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes seleccionar un motivo para poder cancelarlo.");
                            CancelarProcMedWindow.Show();
                            return;
                        }
                        if (ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED != null ?
                            (a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED && a.PROC_MED.ID_PROCMED_SUBTIPO == SelectProcMedEnCitaParaAgendar.ID_PROCMED_SUBTIPO)
                        : false))
                        {
                            ListProcedimientosMedicosEnCita.Remove(ListProcedimientosMedicosEnCita.First(a => a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED && a.PROC_MED.ID_PROCMED_SUBTIPO == SelectProcMedEnCitaParaAgendar.ID_PROCMED_SUBTIPO));
                            SelectProcMedEnCitaParaAgendarAux = new PROC_MED
                            {
                                DESCR = SelectProcMedEnCitaParaAgendar.DESCR,
                                ESTATUS = SelectProcMedEnCitaParaAgendar.ESTATUS,
                                ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                ID_PROCMED_SUBTIPO = SelectProcMedEnCitaParaAgendar.ID_PROCMED_SUBTIPO,
                                PROC_MED_SUBTIPO = SelectProcMedEnCitaParaAgendar.PROC_MED_SUBTIPO,
                                PROC_ATENCION_MEDICA = SelectProcMedEnCitaParaAgendar.PROC_ATENCION_MEDICA,
                                PROC_MATERIAL = SelectProcMedEnCitaParaAgendar.PROC_MATERIAL
                            };
                            SelectProcMedEnCitaParaAgendar = ListProcedimientosMedicosEnCitaParaAgregar.First(f => f.ID_PROCMED == -1);
                            ProcedimientosIncidencias = ProcedimientosIncidencias ?? new List<PROC_ATENCION_MEDICA_PROG_INCI>();
                            ProcedimientosIncidencias.Add(new PROC_ATENCION_MEDICA_PROG_INCI
                            {
                                ID_ACMOTIVO = SelectMotivoCancelarProcMed.ID_ACMOTIVO,
                                ID_USUARIO = GlobalVar.gUsr,
                                OBSERVACIONES = TextObservacionesCancelarProcMed,
                                REGISTRO_FEC = Fechas.GetFechaDateServer,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                            });
                            await new Dialogos().ConfirmacionDialogoReturn("Exito!", "Se elimino el procedimiento médico correctamente.");
                            #region PROCEDIMIENTOS MEDICOS
                            ProcedimientoMedicoParaAgenda = new CustomProcedimientosMedicosSeleccionados();
                            if (LstAgenda == null)
                                LstAgenda = new ObservableCollection<Appointment>();
                            EmpleadosEnAgendaEnabled = true;
                            AgregarProcedimientoMedicoLayoutVisible = Visibility.Visible;
                            SelectedEmpleadoValue = LstEmpleados.First(f => f.ID_EMPLEADO == new cUsuario().ObtenerUsuario(GlobalVar.gUsr).ID_PERSONA);
                            AgendaView = new AgendarCitaConCalendarioView();
                            SelectedDateCalendar = fHoy;
                            //AgendaView.Loaded += AgendaLoaded;
                            //AgendaView.Agenda.AddAppointment += AgendaAddAppointment;
                            //AgendaView.btn_guardar.Click += AgendaClick;
                            //AgendaView.Closing += AgendaClosing;
                            AgendaView.Owner = PopUpsViewModels.MainWindow;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            AgendaView.ShowDialog();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            #endregion
                            CancelarProcMedWindow.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un problema al cancelar el procedimiento médico seleccionado.", ex);
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
                                                    SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? "" : SelectIngreso.IMPUTADO.MATERNO.Trim());
                                                //ObtenerAgenda(SelectedEmpleadoValue.Usuario.ID_USUARIO, true);
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
                case "agregar_procedimiento_en_cita":
                    try
                    {
                        if (SelectProcMedEnCitaParaAgendarAux != null ? SelectProcMedEnCitaParaAgendarAux.ID_PROCMED <= 0 : true)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento médico valido para poder agregarlo.");
                            return;
                        }
                        if (!ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED != null ? a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendarAux.ID_PROCMED && a.PROC_MED.ID_PROCMED_SUBTIPO == SelectProcMedEnCitaParaAgendarAux.ID_PROCMED_SUBTIPO : true))
                        {
                            ListProcedimientosMedicosEnCita.Add(new CustomProcedimientosMedicosCitados
                            {
                                PROC_MED = new PROC_MED
                                {
                                    DESCR = SelectProcMedEnCitaParaAgendarAux.DESCR,
                                    ESTATUS = SelectProcMedEnCitaParaAgendarAux.ESTATUS,
                                    ID_PROCMED = SelectProcMedEnCitaParaAgendarAux.ID_PROCMED,
                                    ID_PROCMED_SUBTIPO = SelectProcMedEnCitaParaAgendarAux.ID_PROCMED_SUBTIPO,
                                    PROC_MATERIAL = SelectProcMedEnCitaParaAgendarAux.PROC_MATERIAL,
                                    PROC_MED_SUBTIPO = SelectProcMedEnCitaParaAgendarAux.PROC_MED_SUBTIPO,
                                    PROC_ATENCION_MEDICA = SelectProcMedEnCitaParaAgendarAux.PROC_ATENCION_MEDICA,
                                },
                                PROCEDIMIENTOS_MATERIALES = new ObservableCollection<CustomProcedimientosMaterialesCitados>(SelectProcMedEnCitaParaAgendarAux.PROC_MATERIAL.Select(s => new CustomProcedimientosMaterialesCitados
                                {
                                    PROC_MATERIAL = s,
                                    PRODUCTO = s.PRODUCTO,
                                    CANTIDAD = new Nullable<int>(),
                                })),
                                IsVisible = SelectProcMedEnCitaParaAgendarAux.PROC_MATERIAL != null ? SelectProcMedEnCitaParaAgendarAux.PROC_MATERIAL.Any() ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed,
                            });
                            SelectProcMedEnCitaParaAgendar = ListProcedimientosMedicosEnCitaParaAgregar.First(f => f.ID_PROCMED == -1);
                        }
                        if (ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED != null))
                            ListProcedimientosMedicosEnCita = new ObservableCollection<CustomProcedimientosMedicosCitados>(ListProcedimientosMedicosEnCita.OrderBy(o => o.PROC_MED.DESCR));
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar un nuevo procedimiento médico a la consulta médica.", ex);
                    }
                    break;
                case "eliminar_procedimiento_en_cita":
                    try
                    {
                        if (SelectProcMedEnCitaParaAgendar != null ? SelectProcMedEnCitaParaAgendar.ID_PROCMED <= 0 : false)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento médico válido para poder cancelarlo.");
                            return;
                        }
                        if (SelectCitaMedicaImputado.PROC_ATENCION_MEDICA_PROG.Any(s => s.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED))
                        {
                            CancelarProcMedWindow = new CancelarProcedimientoMedicoView();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            CancelarProcMedWindow.DataContext = this;
                            CancelarProcMedWindow.Owner = PopUpsViewModels.MainWindow;
                            //CancelarProcMedWindow.Closed += CancelarProcMedClosed;
                            ProcedimientoMedicoPorAgendar = SelectProcMedEnCitaParaAgendar.DESCR;
                            SelectMotivoCancelarProcMed = ListMotivosCancelarProcMed.First(f => f.ID_ACMOTIVO == -1);
                            TextObservacionesCancelarProcMed = string.Empty;
                            CancelarProcMedWindow.ShowDialog();
                            return;
                        }
                        if (ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED != null ?
                            (a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED && a.PROC_MED.ID_PROCMED_SUBTIPO == SelectProcMedEnCitaParaAgendar.ID_PROCMED_SUBTIPO)
                        : false))
                        {
                            ListProcedimientosMedicosEnCita.Remove(ListProcedimientosMedicosEnCita.First(a => a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED && a.PROC_MED.ID_PROCMED_SUBTIPO == SelectProcMedEnCitaParaAgendar.ID_PROCMED_SUBTIPO));
                            SelectProcMedEnCitaParaAgendar = ListProcedimientosMedicosEnCitaParaAgregar.First(f => f.ID_PROCMED == -1);
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar un procedimiento médico de la consulta médica.", ex);
                    }
                    break;
                case "eliminar_procedimiento_medico_agendado":
                    if (SelectProcedimientoMedicoAgendado == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un insumo.");
                        return;
                    }
                    if (ListProcedimientosMedicosEnCita != null ? !ListProcedimientosMedicosEnCita.Any() : false)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No hay ningun insumo registrado en el procedimiento médico actual.");
                        return;
                    }
                    ListProcedimientosMedicosEnCita.Remove(SelectProcedimientoMedicoAgendado);
                    break;
                case "agregar_procedimiento_medico":
                    if (SelectProcedimientoMedicoHijo == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento médico.");
                        return;
                    }
                    if (SelectProcedimientoMedicoHijo.ID_PROCMED == -1)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento médico.");
                        return;
                    }
                    ListProcMedsSeleccionados = ListProcMedsSeleccionados ?? new ObservableCollection<CustomProcedimientosMedicosSeleccionados>();
                    if (ListProcMedsSeleccionados.Any(a => a.ID_PROC_MED == SelectProcedimientoMedicoHijo.ID_PROCMED))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El procedimiento médico seleccionado ya se encuentra en la lista.");
                        return;
                    }
                    ListProcMedsSeleccionados.Add(new CustomProcedimientosMedicosSeleccionados()
                    {
                        AGENDA = string.Empty,
                        PROC_MED_DESCR = SelectProcedimientoMedicoHijo.DESCR,
                        OBSERV = string.Empty,
                        ID_PROC_MED = SelectProcedimientoMedicoHijo.ID_PROCMED,
                        CITAS = new List<CustomCitasProcedimientosMedicos>(),
                    });
                    SelectProcedimientoSubtipo = ListProcedimientoSubtipo.First(f => f.ID_PROCMED_SUBTIPO == -1);
                    ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>(ListProcMedsSeleccionados.OrderBy(o => o.PROC_MED_DESCR));
                    break;
                case "remove_procedimiento_medico":
                    if (SelectProcMedSeleccionado == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento médico para poder eliminarlo.");
                        return;
                    }
                    if (ListProcMedsSeleccionados != null ? !ListProcMedsSeleccionados.Any() : true)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No hay ningun procedimiento que quitar.");
                        return;
                    }
                    if (ListProcMedsSeleccionados.Any(a => a.ID_PROC_MED == SelectProcMedSeleccionado.ID_PROC_MED))
                    {
                        ListProcMedsSeleccionados.Remove(ListProcMedsSeleccionados.First(f => f.ID_PROC_MED == SelectProcMedSeleccionado.ID_PROC_MED));
                        //SelectProcedimientoSubtipo = ListProcedimientoSubtipo.First(f => f.ID_PROCMED_SUBTIPO == -1);
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El procedimiento seleccionada no forma parte de la lista.");
                        return;
                    }
                    break;
                #endregion

                #region OTROS
                case "regresar_listado":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotaMedicaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new NotaMedicaViewModel();
                    break;
                case "historia_clinica_menu":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                        return;
                    }
                    #region HISTORIA CLINICA MEDICA
                    var FechaHoy = Fechas.GetFechaDateServer;
                    var HistoriaClinica = new HISTORIA_CLINICA
                    {
                        EF_PESO = TextPeso,
                        EF_ESTATURA = TextTalla,
                        EF_RESPIRACION = TextFrecuenciaRespira,
                        EF_PULSO = TextFrecuenciaCardiaca,
                        EF_TEMPERATURA = TextTemperatura,
                        EF_PRESION_ARTERIAL = TextTensionArterial1 + "/" + TextTensionArterial2,
                        ESTATUS = "I",
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        ESTUDIO_FEC = FechaHoy,
                    };
                    if (new cNotaMedica().InsertarHistoriaClinicaXEnfermero(HistoriaClinica))
                    {
                        if (timer != null)
                            timer.Stop();
                        StaticSourcesViewModel.SourceChanged = false;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new HistoriaClinicaView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext =
                            new HistoriaClinicaViewModel(SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_ANIO, SelectIngreso.ID_INGRESO, SelectIngreso.ID_CENTRO);
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ocurrió un error al guardar los signos vitales en la historia clinica.");
                        return;
                    }
                    #endregion
                    break;
                case "reporte_menu":
                    try
                    {
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                            return;
                        }
                        if (!SelectIngreso.ATENCION_MEDICA.Any(a => a.CERTIFICADO_MEDICO != null))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un imputado con un certificado de nuevo ingreso registrado.");
                            return;
                        }
                        SelectAtencionMedica = SelectIngreso.ATENCION_MEDICA.First(f => f.CERTIFICADO_MEDICO != null);
                        if (SelectAtencionMedica == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El imputado seleccionado no cuenta con un certificado médico de ingreso.");
                            return;
                        }
                        var CertView = new ReporteView();
                        var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                ListLesiones = new ObservableCollection<LesionesCustom>(SelectAtencionMedica.CERTIFICADO_MEDICO.LESION.Where(w => w.ID_CENTRO_UBI == GlobalVar.gCentro)
                                    .Select(s => new LesionesCustom
                                    {
                                        DESCR = s.DESCR,
                                        REGION = s.ANATOMIA_TOPOGRAFICA
                                    }));
                                CertView.ReporteViewer.LocalReport.ReportPath = "Reportes/rCertificadoNuevoIngreso.rdlc";
                                CertView.ReporteViewer.LocalReport.DataSources.Clear();
                                var medico = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault().EMPLEADO;
                                var hoy = Fechas.GetFechaDateServer;
                                var hora = hoy.ToShortTimeString() + " hrs. Del día ";
                                var dia = hoy.Day + " de " + hoy.ToString("MMMM").ToUpper() + " del " + hoy.Year + ".";
                                rds1.Name = "DataSet1";
                                #region Objeto
                                var lesiones = string.Empty;
                                var i = 0;
                                foreach (var item in SelectAtencionMedica.CERTIFICADO_MEDICO.LESION)
                                {
                                    lesiones = lesiones + item.DESCR.Trim() + " EN " + item.ANATOMIA_TOPOGRAFICA.DESCR + (i == SelectAtencionMedica.CERTIFICADO_MEDICO.LESION.Count() ? string.Empty : ", ");
                                    i++;
                                }
                                var delitos = "";
                                if (SelectIngreso.CAUSA_PENAL.Any(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO))
                                {
                                    var count = 1;
                                    foreach (var item in SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO)
                                    {
                                        delitos = delitos + item.DESCR_DELITO + (count == SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO.Count ? "" : ",");
                                        count++;
                                    }
                                }
                                var municipio = new cMunicipio().Obtener(SelectIngreso.IMPUTADO.NACIMIENTO_ESTADO.Value, SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                                rds1.Value = new List<cReporteCertificadoNuevoIngreso>()
                                {
                                    new cReporteCertificadoNuevoIngreso
                                    {
                                        Centro = SelectIngreso.CENTRO1 != null ? SelectIngreso.CENTRO1.DESCR.Trim() : "",
                                        Ciudad = SelectIngreso.CENTRO1 != null ? SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1.Trim() : "",
                                        Director = SelectIngreso.CENTRO1 != null ? SelectIngreso.CENTRO1.DIRECTOR.Trim() : "",
                                        Cedula = medico.CEDULA,
                                        Edad = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value : new DateTime()).ToString(),
                                        FechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                                        FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                                        Fecha = hora + dia,
                                        Escolaridad = SelectIngreso.ESCOLARIDAD != null ? SelectIngreso.ESCOLARIDAD.DESCR : "",
                                        Originario = SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? municipio.MUNICIPIO1 + ", " + municipio.ENTIDAD.DESCR : SelectIngreso.IMPUTADO.NACIMIENTO_LUGAR,
                                        NombreImputado = SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + SelectIngreso.IMPUTADO.MATERNO.Trim(),
                                        TipoDelito = delitos,
                                        NombreMedico = medico.PERSONA.NOMBRE.Trim() + " " + medico.PERSONA.PATERNO.Trim() + " " + medico.PERSONA.MATERNO.Trim(),
                                        Sexo = medico.PERSONA.SEXO == "M" ? "MASCULINO" : medico.PERSONA.SEXO == "F" ? "FEMENINO" : "", 
                                        SignosVitales_FC = SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC : string.Empty : string.Empty,
                                        SignosVitales_FR = SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA : string.Empty : string.Empty,
                                        SignosVitales_T = SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES.TEMPERATURA : string.Empty : string.Empty,
                                        SignosVitales_TA = SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL : string.Empty : string.Empty,
                                        Logo1 = ParametroLogoEstado,
                                        Logo2 = ParametroReporteLogo2,
                                        Dorso = ParametroCuerpoDorso,
                                        Frente = ParametroCuerpoFrente,
                                        Check = ParametroImagenZonaCorporal,
                                        AntecedentesPatologicos = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.ANTECEDENTES_PATOLOGICOS : "" : "",
                                        Interrogatorio = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.EXPLORACION_FISICA : "" : "",
                                        PadecimientoTratamiento = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.PADECIMIENTO : "" : "",
                                        Toxicomanias = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.TOXICOMANIAS : "" : "",
                                        AmeritaHospitalizacion = SelectAtencionMedica != null?SelectAtencionMedica.CERTIFICADO_MEDICO != null ?SelectAtencionMedica.CERTIFICADO_MEDICO.AMERITA_HOSPITALIZACION == "S" ? "SI" : "NO" : "NO" : "NO",
                                        Diagnostico = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.DIAGNOSTICO : string.Empty : string.Empty,
                                        DiasEnSanar = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.TARDA_SANAR_15 == "S" ? "SI" : "NO" : "NO" : "NO",
                                        Observaciones = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.OBSERVACIONES : string.Empty : string.Empty,
                                        PeligraVida = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.PELIGRA_VIDA == "S" ? "SI" : "NO" : "NO" : "NO",
                                        PlanTerapeutico = SelectAtencionMedica != null? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.PLAN_TERAPEUTICO : string.Empty : string.Empty,
                                        Seguimiento = SelectAtencionMedica != null ? SelectAtencionMedica.ID_CITA_SEGUIMIENTO.HasValue ? "SI" : "NO" : "NO",
                                        F_FRONTAL=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FRONTAL) : true,
                                        D_ANTEBRAZO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_DERECHO) : true,
                                        D_ANTEBRAZO_IZQUIERDO =ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_IZQUIERDO) : true,
                                        D_BRAZO_POSTERIOR_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_DERECHO) : true,
                                        D_BRAZO_POSTERIOR_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_IZQUIERDO) : true,
                                        D_CALCANEO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_DERECHO) : true,
                                        D_CALCANEO_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_IZQUIERDA) : true,
                                        D_CODO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_DERECHO) : true,
                                        D_CODO_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_IZQUIERDO) : true,
                                        D_COSTILLAS_COSTADO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_DERECHO) : true,
                                        D_COSTILLAS_COSTADO_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_IZQUIERDO) : true,
                                        D_DORSAL_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_DERECHA) : true,
                                        D_DORSAL_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_IZQUIERDA) : true,
                                        D_ESCAPULAR_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_DERECHA) : true,
                                        D_ESCAPULAR_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_IZQUIERDA) : true,
                                        D_FALANGES_POSTERIORES_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_DERECHA) : true,
                                        D_FALANGES_POSTERIORES_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_IZQUIERDA) : true,
                                        D_GLUTEA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_DERECHA) : true,
                                        D_GLUTEA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_IZQUIERDA) : true,
                                        D_LUMBAR_RENAL_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_LUMBAR_RENAL_DERECHA) : true,
                                        D_LUMBAR_RENAL_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                                        D_METATARZIANA_PLANTA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                                        D_METATARZIANA_PLANTA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_IZQUIERDA) : true,
                                        D_MUÑECA_POSTERIOR_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_DERECHA) : true,
                                        D_MUÑECA_POSTERIOR_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_IZQUIERDA) : true,
                                        D_MUSLO_POSTERIOR_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_DERECHO) : true,
                                        D_MUSLO_POSTERIOR_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_IZQUIERDO) : true,
                                        D_OCCIPITAL_NUCA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OCCIPITAL_NUCA) : true,
                                        D_OREJA_POSTERIOR_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_DERECHA) : true,
                                        D_OREJA_POSTERIOR_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_IZQUIERDA) : true,
                                        D_PANTORRILLA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_DERECHA) : true,
                                        D_PANTORRILLA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_IZQUIERDA) : true,
                                        D_POPLITEA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_DERECHA) : true,
                                        D_POPLITEA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_IZQUIERDA) : true,
                                        D_POSTERIOR_CABEZA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CABEZA) : true,
                                        D_POSTERIOR_CUELLO =ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CUELLO) : true,
                                        D_POSTERIOR_ENTREPIERNA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_DERECHA) : true,
                                        D_POSTERIOR_ENTREPIERNA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_IZQUIERDA) : true,
                                        D_POSTERIOR_HOMBRO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_DERECHO) : true,
                                        D_POSTERIOR_HOMBRO_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_IZQUIERDO) : true,
                                        D_SACRA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_SACRA) : true,
                                        D_TOBILLO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_DERECHO) : true,
                                        D_TOBILLO_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_IZQUIERDO) : true,
                                        D_TORACICA_DORSAL=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TORACICA_DORSAL) : true,
                                        D_VERTEBRAL_CERVICAL=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_CERVICAL) : true,
                                        D_VERTEBRAL_LUMBAR=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_LUMBAR) : true,
                                        D_VERTEBRAL_TORACICA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_TORACICA) : true,
                                        F_ANTEBRAZO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_DERECHO) : true,
                                        F_ANTEBRAZO_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_IZQUIERDO) : true,
                                        F_ANTERIOR_CUELLO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTERIOR_CUELLO) : true,
                                        F_MUÑECA_ANTERIOR_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_DERECHA) : true,
                                        F_MUÑECA_ANTERIOR_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_IZQUIERDA) : true,
                                        F_AXILA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_DERECHA) : true,
                                        F_AXILA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_IZQUIERDA) : true,
                                        F_BAJO_VIENTRE_HIPOGASTRIO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BAJO_VIENTRE_HIPOGASTRIO) : true,
                                        F_BRAZO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_DERECHO) : true,
                                        F_BRAZO_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_IZQUIERDO) : true,
                                        F_CARA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                                        F_CARA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CARA_IZQUIERDA) : true,
                                        F_CLAVICULAR_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                                        F_CLAVICULAR_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_IZQUIERDA) : true,
                                        F_CODO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_DERECHO) : true,
                                        F_CODO_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_IZQUIERDO) : true,
                                        F_ENTREPIERNA_ANTERIOR_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_DERECHA) : true,
                                        F_ENTREPIERNA_ANTERIOR_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_IZQUIERDA) : true,
                                        F_EPIGASTRIO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_EPIGASTRIO) : true,
                                        F_ESCROTO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESCROTO) : true,
                                        F_ESPINILLA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_DERECHA) : true,
                                        F_ESPINILLA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_IZQUIERDA) : true,
                                        F_FALANGES_MANO_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_DERECHA) : true,
                                        F_FALANGES_MANO_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_IZQUIERDA) : true,
                                        F_FALANGES_PIE_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_DERECHO) : true,
                                        F_FALANGES_PIE_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_IZQUIERDO) : true,
                                        F_HIPOCONDRIA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_DERECHA) : true,
                                        F_HIPOCONDRIA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_IZQUIERDA) : true,
                                        F_HOMBRO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_DERECHO) : true,
                                        F_HOMBRO_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_IZQUIERDO) : true,
                                        F_INGUINAL_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_DERECHA) : true,
                                        F_INGUINAL_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_IZQUIERDA) : true,
                                        F_LATERAL_CABEZA_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_DERECHO) : true,
                                        F_LATERAL_CABEZA_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_IZQUIERDO) : true,
                                        F_MANDIBULA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MANDIBULA) : true,
                                        F_METACARPIANOS_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_DERECHA) : true,
                                        F_METACARPIANOS_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_IZQUIERDA) : true,
                                        F_METATARZIANA_DORSAL_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_DERECHO) : true,
                                        F_METATARZIANA_DORSAL_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_IZQUIERDO) : true,
                                        F_MUSLO_ANTERIOR_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_DERECHO) : true,
                                        F_MUSLO_ANTERIOR_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_IZQUIERDO) : true,
                                        F_NARIZ=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_NARIZ) : true,
                                        F_ORBITAL_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_DERECHO) : true,
                                        F_ORBITAL_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_IZQUIERDO) : true,
                                        F_OREJA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_DERECHA) : true,
                                        F_OREJA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_IZQUIERDA) : true,
                                        F_PENE_VAGINA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PENE_VAGINA) : true,
                                        F_PEZON_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_DERECHO) : true,
                                        F_PEZON_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_IZQUIERDO) : true,
                                        F_RODILLA_DERECHA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_DERECHA) : true,
                                        F_RODILLA_IZQUIERDA=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_IZQUIERDA) : true,
                                        F_TOBILLO_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_DERECHO) : true,
                                        F_TOBILLO_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_IZQUIERDO) : true,
                                        F_TORAX_CENTRAL=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_CENTRAL) : true,
                                        F_TORAX_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_DERECHO) : true,
                                        F_TORAX_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_IZQUIERDO) : true,
                                        F_UMBILICAL_MESOGASTIO = ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_UMBILICAL_MESOGASTIO) : true,
                                        F_VACIO_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_DERECHA) : true,
                                        F_VACIO_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_IZQUIERDA) : true,
                                        F_VERTICE_CABEZA = ListLesiones!=null ? !ListLesiones.Any(a=>a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VERTICE_CABEZA) : true
                                    }
                                };
                                #endregion
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    try
                                    {
                                        CertView.DataContext = this;
                                        CertView.ReporteViewer.LocalReport.DataSources.Add(rds1);
                                        CertView.ReporteViewer.ShowExportButton = false;
                                        CertView.ReporteViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                                        CertView.ReporteViewer.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                                        CertView.ReporteViewer.ZoomPercent = 100;
                                        CertView.ReporteViewer.RefreshReport();
                                        CertView.Owner = PopUpsViewModels.MainWindow;
                                    }
                                    catch (Exception ex)
                                    {
                                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del certificado seleccionado.", ex);
                                    }
                                }));
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del certificado seleccionado.", ex);
                            }
                        });
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        CertView.ShowDialog();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del certificado seleccionado.", ex);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    break;
                case "ficha_menu":
                    break;
                case "eliminar_receta":
                    if (SelectReceta != null ? SelectReceta.PRODUCTO == null : true) return;
                    if (ListRecetas.Any(a => a.PRODUCTO.ID_PRODUCTO == SelectReceta.PRODUCTO.ID_PRODUCTO))
                        if (SelectReceta.RECETA_MEDICA_DETALLE == null)
                        {
                            ListRecetas.Remove(ListRecetas.First(a => a.PRODUCTO.ID_PRODUCTO == SelectReceta.PRODUCTO.ID_PRODUCTO));
                            return;
                        }
                    break;
                case "eliminar_receta_historial":
                    if (SelectRecetaHistorial != null ? SelectRecetaHistorial.PRODUCTO == null : true) return;
                    if (ListHistorialRecetas.Any(a => a.PRODUCTO.ID_PRODUCTO == SelectRecetaHistorial.PRODUCTO.ID_PRODUCTO))
                    {
                        ListRecetasCanceladas = ListRecetasCanceladas ?? new List<RecetaMedica>();
                        if (await new Dialogos().ConfirmarEliminar("Validación...", "Esta seguro que desea CANCELAR/SUSPENDER este medicamento?") == 1)
                            if (!ListRecetasCanceladas.Any(a => a.PRODUCTO.ID_PRODUCTO == SelectRecetaHistorial.PRODUCTO.ID_PRODUCTO && !a.ELIMINADO &&
                                (a.RECETA_MEDICA_DETALLE != null ? !a.RECETA_MEDICA_DETALLE.CANCELA_ID_ATENCION_MEDICA.HasValue : true)))
                            {
                                ListRecetasCanceladas.Add(ListHistorialRecetas.First(a => a.PRODUCTO.ID_PRODUCTO == SelectRecetaHistorial.PRODUCTO.ID_PRODUCTO));
                                ListHistorialRecetas.First(a => a.PRODUCTO.ID_PRODUCTO == SelectRecetaHistorial.PRODUCTO.ID_PRODUCTO).ELIMINADO = true;
                                ListHistorialRecetas = new ObservableCollection<RecetaMedica>(ListHistorialRecetas.ToList());
                            }
                        //ListRecetas.Remove(ListRecetas.First(a => a.PRODUCTO.ID_PRODUCTO == SelectReceta.PRODUCTO.ID_PRODUCTO));
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
                            SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? "" : SelectIngreso.IMPUTADO.MATERNO.Trim());
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
                case "agendar_cita":
                    try
                    {
                        LstAgenda = LstAgenda ?? new ObservableCollection<Appointment>();
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            var useer = new cUsuario().ObtenerTodos(GlobalVar.gUsr);
                            if (useer.Any() ? !useer.FirstOrDefault().ID_PERSONA.HasValue : false) return;
                            var usr = useer.FirstOrDefault();
                        });
                        SelectedEmpleadoValue = LstEmpleados.First(f => f.Usuario != null ? string.IsNullOrEmpty(f.Usuario.ID_USUARIO) ? false : f.Usuario.ID_USUARIO.Trim() == GlobalVar.gUsr : false);
                        EmpleadosEnAgendaEnabled = false;
                        ProcedimientoMedicoParaAgenda = null;
                        AgregarProcedimientoMedicoLayoutVisible = Visibility.Collapsed;
                        AgendaView = new AgendarCitaConCalendarioView();
                        SelectedDateCalendar = fHoy;
                        //AgendaView.Loaded += AgendaLoaded;
                        //AgendaView.Agenda.AddAppointment += AgendaAddAppointment;
                        //AgendaView.btn_guardar.Click += AgendaClick;
                        //AgendaView.Closing += AgendaClosing;
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
                case "borrar_enfermedad":
                    if (ListEnfermedades == null) return;
                    if (SelectEnfermedad == null) return;
                    ListEnfermedades.Remove(ListEnfermedades.First(f => f.ID_ENFERMEDAD == SelectEnfermedad.ID_ENFERMEDAD));
                    break;
                case "salir_menu":
                    Imputado = null;
                    PrincipalViewModel.SalirMenu();
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotaEvolucionView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new NotaEvolucionViewModel();
                    break;
                #endregion

                case "guardar_menu":
                    try
                    {
                        ValidacionesMedico();
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios. \n\n" + base.Error);
                            /* Application.Current.Dispatcher.Invoke((Action)(delegate { })); */
                            return;
                        }
                        if (ListRecetas != null ?
                            ListRecetas.Any() ?
                                ListRecetas.Any(a => (a.CANTIDAD.HasValue ? a.CANTIDAD.Value <= 0 : true) ||
                                                     (a.DURACION.HasValue ? a.DURACION.Value <= 0 : true) ||
                                                     (!a.HORA_MANANA && !a.HORA_TARDE && !a.HORA_NOCHE) ||
                                                     (a.PRESENTACION.HasValue ?
                                                        a.PRESENTACION.Value <= 0 ?
                                                            a.PRODUCTO != null ?
                                                                a.PRODUCTO.PRODUCTO_PRESENTACION_MED.Count > 0
                                                            : false
                                                        : false
                                                     : a.PRODUCTO != null ?
                                                        a.PRODUCTO.PRODUCTO_PRESENTACION_MED.Count > 0
                                                     : false))
                            : false
                        : false)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos de la receta médica.");
                            return;
                        }
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            var hoy = Fechas.GetFechaDateServer;

                            #region NOTAS
                            var AtencionMedica = new ATENCION_MEDICA
                            {
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_UB_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_HOSPITA = SelectHospitalizado.ID_HOSPITA,
                                ID_TIPO_ATENCION = IsMedico ? (short)enumAtencionTipo.CONSULTA_MEDICA : (short)enumAtencionTipo.CONSULTA_DENTAL,
                                ID_TIPO_SERVICIO = IsMedico ? (short)enumAtencionServicio.NOTA_EVOLUCION : (short)enumAtencionServicio.NOTA_EVOLUCIÓN_DENTAL,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ATENCION_FEC = hoy,
                            };
                            var userPersona = new cUsuario().ObtenerUsuario(GlobalVar.gUsr);
                            var NotaMed = new NOTA_MEDICA
                            {
                                EXPLORACION_FISICA = TextExploracionFisica,
                                ID_PRONOSTICO = SelectPronostico,
                                ID_RESPONSABLE = userPersona.ID_PERSONA,
                                OCUPA_HOSPITALIZACION = "N",
                                OCUPA_INTERCONSULTA = SolicitaInterconsultaCheck ? "S" : "N",
                                //PLAN_ESTUDIO_TRATA = TextEstudios,
                                RESUMEN_INTERROGAT = TextResumenInterrogatorio,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                            };
                            var NotaSignosVitales = new NOTA_SIGNOS_VITALES
                            {
                                FRECUENCIA_CARDIAC = TextFrecuenciaCardiaca,
                                FRECUENCIA_RESPIRA = TextFrecuenciaRespira,
                                ID_RESPONSABLE = userPersona.ID_PERSONA,
                                PESO = TextPeso,
                                TALLA = TextTalla,
                                TEMPERATURA = TextTemperatura,
                                GLUCEMIA = TextGlucemia,
                                TENSION_ARTERIAL = TensionArterial,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                            };
                            var NotaEvol = new NOTA_EVOLUCION
                            {
                                REGISTRO_FEC = hoy,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                ESTUDIO_RESULTADO = TextEstudios,
                            };
                            #endregion

                            #region ENFERMEDADES
                            var EnfermedadesPorAgregar = new List<NOTA_MEDICA_ENFERMEDAD>();
                            EnfermedadesPorAgregar = null;
                            if (ListEnfermedades != null ? ListEnfermedades.Any() : false)
                            {
                                foreach (var item in ListEnfermedades)
                                {
                                    if (!(SelectHospitalizado.NOTA_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any() ?
                                            SelectHospitalizado.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any(a => a.ID_ENFERMEDAD == item.ID_ENFERMEDAD)
                                        : false
                                    : false)
                                        &&
                                    !(SelectHospitalizado.ATENCION_MEDICA.Any() ?
                                        SelectHospitalizado.ATENCION_MEDICA.Any(a => a.NOTA_MEDICA != null ?
                                            a.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any() ?
                                                a.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any(e => e.ID_ENFERMEDAD == item.ID_ENFERMEDAD)
                                            : false
                                        : false)
                                    : false)
                                        &&
                                    !(SelectHospitalizado.NOTA_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.CANALIZACION != null ?
                                            SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null ?
                                                SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any() ?
                                                    SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SOL_INTERCONSULTA_INTERNA.Any() ?
                                                        a.SOL_INTERCONSULTA_INTERNA.Any(an => an.ESPECIALISTA_CITA.Any(any => any.ATENCION_CITA != null ?
                                                            any.ATENCION_CITA.ATENCION_MEDICA != null ?
                                                                any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                                                    any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any(e => e.ID_ENFERMEDAD == item.ID_ENFERMEDAD)
                                                                : false
                                                            : false
                                                        : false))
                                                    : false)
                                                : false
                                            : false
                                        : false
                                    : false))
                                    {
                                        EnfermedadesPorAgregar = EnfermedadesPorAgregar ?? new List<NOTA_MEDICA_ENFERMEDAD>();
                                        EnfermedadesPorAgregar.Add(new NOTA_MEDICA_ENFERMEDAD
                                        {
                                            ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                            ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                            REGISTRO_FEC = hoy,
                                            ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                            ENFERMEDAD = item.ENFERMEDAD
                                        });
                                    }
                                }
                            }
                            #endregion

                            #region RECETAS
                            var receta = new RECETA_MEDICA();
                            var recetas = new List<RECETA_MEDICA_DETALLE>();
                            var RecetasCanceladas = new List<RECETA_MEDICA_DETALLE>();
                            var RecetasAgregadas = new List<RECETA_MEDICA_DETALLE>();
                            RecetasCanceladas = null;
                            #region CANCELADAS
                            if (ListRecetasCanceladas != null ? ListRecetasCanceladas.Any() : false)
                            {
                                foreach (var item in ListRecetasCanceladas)
                                {
                                    if (!(SelectHospitalizado.NOTA_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.Any() ?
                                            SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.Any(a => a.RECETA_MEDICA_DETALLE.Any()) ?
                                                SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.SelectMany(s => s.RECETA_MEDICA_DETALLE).Any(a => a.ID_PRODUCTO == item.PRODUCTO.ID_PRODUCTO ?
                                                    !a.CANCELA_ID_ATENCION_MEDICA.HasValue ?
                                                        !a.CANCELA_ID_CENTRO_UBI.HasValue ?
                                                            item.RECETA_MEDICA_DETALLE != null ?
                                                                a.ID_CENTRO_UBI == item.RECETA_MEDICA_DETALLE.ID_CENTRO_UBI ?
                                                                    a.ID_ATENCION_MEDICA == item.RECETA_MEDICA_DETALLE.ID_ATENCION_MEDICA
                                                                : false
                                                            : false
                                                        : false
                                                    : false
                                                : false)
                                            : false
                                        : false
                                    : false)
                                    ||
                                    !(SelectHospitalizado.ATENCION_MEDICA.Any() ?
                                        SelectHospitalizado.ATENCION_MEDICA.Any(a => a.RECETA_MEDICA.Any()) ?
                                            SelectHospitalizado.ATENCION_MEDICA.SelectMany(s => s.RECETA_MEDICA).Any(s => s.RECETA_MEDICA_DETALLE.Any()) ?
                                                SelectHospitalizado.ATENCION_MEDICA.SelectMany(s => s.RECETA_MEDICA).SelectMany(s => s.RECETA_MEDICA_DETALLE).Any(an => an.ID_PRODUCTO == item.PRODUCTO.ID_PRODUCTO ?
                                                    !an.CANCELA_ID_ATENCION_MEDICA.HasValue ?
                                                        !an.CANCELA_ID_CENTRO_UBI.HasValue ?
                                                            item.RECETA_MEDICA_DETALLE != null ?
                                                                an.ID_CENTRO_UBI == item.RECETA_MEDICA_DETALLE.ID_CENTRO_UBI ?
                                                                    an.ID_ATENCION_MEDICA == item.RECETA_MEDICA_DETALLE.ID_ATENCION_MEDICA
                                                                : false
                                                            : false
                                                        : false
                                                    : false
                                                : false)
                                            : false
                                        : false
                                    : false)
                                    ||
                                    !(SelectHospitalizado.NOTA_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.CANALIZACION != null ?
                                            SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null ?
                                                SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any() ?
                                                    SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SOL_INTERCONSULTA_INTERNA.Any() ?
                                                        a.SOL_INTERCONSULTA_INTERNA.Any(an => an.ESPECIALISTA_CITA.Any(any => any.ATENCION_CITA != null ?
                                                            any.ATENCION_CITA.ATENCION_MEDICA != null ?
                                                                any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                                                    any.ATENCION_CITA.ATENCION_MEDICA.RECETA_MEDICA.Any(rm => rm.RECETA_MEDICA_DETALLE.Any()) ?
                                                                        any.ATENCION_CITA.ATENCION_MEDICA.RECETA_MEDICA.SelectMany(s => s.RECETA_MEDICA_DETALLE).Any(rm => rm.ID_PRODUCTO == item.PRODUCTO.ID_PRODUCTO ?
                                                                            !rm.CANCELA_ID_ATENCION_MEDICA.HasValue ?
                                                                                !rm.CANCELA_ID_CENTRO_UBI.HasValue ?
                                                                                    item.RECETA_MEDICA_DETALLE != null ?
                                                                                        rm.ID_CENTRO_UBI == item.RECETA_MEDICA_DETALLE.ID_CENTRO_UBI ?
                                                                                            rm.ID_ATENCION_MEDICA == item.RECETA_MEDICA_DETALLE.ID_ATENCION_MEDICA
                                                                                        : false
                                                                                    : false
                                                                                : false
                                                                            : false
                                                                        : false)
                                                                    : false
                                                                : false
                                                            : false
                                                        : false))
                                                    : false)
                                                : false
                                            : false
                                        : false
                                    : false))
                                    {
                                        RecetasCanceladas = RecetasCanceladas ?? new List<RECETA_MEDICA_DETALLE>();
                                        RecetasCanceladas.Add(new RECETA_MEDICA_DETALLE
                                        {
                                            CENA = (short)(item.HORA_NOCHE ? 1 : 0),
                                            COMIDA = (short)(item.HORA_TARDE ? 1 : 0),
                                            DESAYUNO = (short)(item.HORA_MANANA ? 1 : 0),
                                            DOSIS = item.CANTIDAD,
                                            DURACION = item.DURACION,
                                            ID_CENTRO_UBI = item.RECETA_MEDICA_DETALLE.ID_CENTRO_UBI,
                                            ID_ATENCION_MEDICA = item.RECETA_MEDICA_DETALLE.ID_ATENCION_MEDICA,
                                            ID_FOLIO = item.RECETA_MEDICA_DETALLE.ID_FOLIO,
                                            ID_PRESENTACION_MEDICAMENTO = item.PRESENTACION,
                                            ID_PRODUCTO = item.PRODUCTO.ID_PRODUCTO,
                                            OBSERV = item.OBSERVACIONES,
                                        });
                                    }
                                }
                            }
                            #endregion
                            RecetasAgregadas = null;
                            #region AGREGADAS
                            if (ListRecetas != null ? ListRecetas.Any() : false)
                            {
                                receta = new RECETA_MEDICA
                                {
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    ID_USUARIO = GlobalVar.gUsr,
                                    ESTATUS = "S",
                                    RECETA_FEC = hoy,
                                };

                                foreach (var item in ListRecetas)
                                {
                                    if (!(SelectHospitalizado.NOTA_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.Any() ?
                                            SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.Any(a => a.RECETA_MEDICA_DETALLE.Any()) ?
                                                SelectHospitalizado.NOTA_MEDICA.RECETA_MEDICA.SelectMany(s => s.RECETA_MEDICA_DETALLE).Any(a => a.ID_PRODUCTO == item.PRODUCTO.ID_PRODUCTO ?
                                                    a.DOSIS == item.CANTIDAD ?
                                                        a.DURACION == item.DURACION /*?
                                                            a.ID_ATENCION_MEDICA == new Nullable<int>() ?
                                                                a.ID_CENTRO_UBI == new Nullable<short>()
                                                            : false
                                                        : false*/
                                                    : false
                                                : false)
                                            : false
                                        : false
                                    : false)
                                    &&
                                    !(SelectHospitalizado.ATENCION_MEDICA.Any() ?
                                        SelectHospitalizado.ATENCION_MEDICA.Any(a => a.RECETA_MEDICA.Any()) ?
                                            SelectHospitalizado.ATENCION_MEDICA.SelectMany(s => s.RECETA_MEDICA).Any(s => s.RECETA_MEDICA_DETALLE.Any()) ?
                                                SelectHospitalizado.ATENCION_MEDICA.SelectMany(s => s.RECETA_MEDICA).SelectMany(s => s.RECETA_MEDICA_DETALLE).Any(an => an.ID_PRODUCTO == item.PRODUCTO.ID_PRODUCTO ?
                                                    an.DOSIS == item.CANTIDAD ?
                                                        an.DURACION == item.DURACION
                                                    : false
                                                : false)
                                            : false
                                        : false
                                    : false)
                                        &&
                                    !(SelectHospitalizado.NOTA_MEDICA != null ?
                                        SelectHospitalizado.NOTA_MEDICA.CANALIZACION != null ?
                                            SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null ?
                                                SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any() ?
                                                    SelectHospitalizado.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.SOL_INTERCONSULTA_INTERNA.Any() ?
                                                        a.SOL_INTERCONSULTA_INTERNA.Any(an => an.ESPECIALISTA_CITA.Any(any => any.ATENCION_CITA != null ?
                                                            any.ATENCION_CITA.ATENCION_MEDICA != null ?
                                                                any.ATENCION_CITA.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                                                    any.ATENCION_CITA.ATENCION_MEDICA.RECETA_MEDICA.Any(s => s.RECETA_MEDICA_DETALLE.Any(r => r.ID_PRODUCTO == item.PRODUCTO.ID_PRODUCTO))
                                                                : false
                                                            : false
                                                        : false))
                                                    : false)
                                                : false
                                            : false
                                        : false
                                    : false))
                                    {
                                        RecetasAgregadas = RecetasAgregadas ?? new List<RECETA_MEDICA_DETALLE>();
                                        RecetasAgregadas.Add(new RECETA_MEDICA_DETALLE
                                        {
                                            //ID_FOLIO = cancela.ID_FOLIO,
                                            //ID_ATENCION_MEDICA = item.RECETA_MEDICA_DETALLE.ID_ATENCION_MEDICA,
                                            ID_CENTRO_UBI = GlobalVar.gCentro,
                                            DOSIS = item.CANTIDAD,
                                            DURACION = item.DURACION,
                                            OBSERV = item.OBSERVACIONES,
                                            ID_PRODUCTO = item.PRODUCTO.ID_PRODUCTO,
                                            CENA = (short)(item.HORA_NOCHE ? 1 : 0),
                                            COMIDA = (short)(item.HORA_TARDE ? 1 : 0),
                                            DESAYUNO = (short)(item.HORA_MANANA ? 1 : 0),
                                            ID_PRESENTACION_MEDICAMENTO = item.PRESENTACION,
                                        });
                                    }
                                }
                            }
                            #endregion
                            #endregion

                            #region DIETAS
                            var DietasPorAgregar = new List<NOTA_MEDICA_DIETA>();
                            var DietasCanceladas = new List<NOTA_MEDICA_DIETA>();
                            DietasPorAgregar = null;
                            DietasCanceladas = null;
                            if (ListDietas.Any() ? ListDietas.Any(a => a.ELEGIDO) : false)
                            {
                                foreach (var item in ListDietas.Where(a => a.ELEGIDO))
                                {
                                    if (!ListDietasAux.Where(w => w.ELEGIDO).Any(a => a.DIETA.ID_DIETA == item.DIETA.ID_DIETA))
                                    {
                                        DietasPorAgregar = DietasPorAgregar ?? new List<NOTA_MEDICA_DIETA>();
                                        DietasPorAgregar.Add(new NOTA_MEDICA_DIETA
                                        {
                                            DIETA_FEC = hoy,
                                            ID_CENTRO_UBI = GlobalVar.gCentro,
                                            ID_DIETA = item.DIETA.ID_DIETA,
                                            ESTATUS = "S",
                                        });
                                    }
                                }
                                foreach (var item in ListDietas.Where(a => !a.ELEGIDO))
                                {
                                    if (ListDietasAux.Where(w => w.ELEGIDO).Any(a => a.DIETA.ID_DIETA == item.DIETA.ID_DIETA))
                                    {
                                        DietasCanceladas = DietasCanceladas ?? new List<NOTA_MEDICA_DIETA>();
                                        DietasCanceladas.Add(new NOTA_MEDICA_DIETA
                                        {
                                            DIETA_FEC = AgregarNota ? item.FECHA : hoy,
                                            ID_CENTRO_UBI = ListDietasAux.Where(w => w.ELEGIDO).First(a => a.DIETA.ID_DIETA == item.DIETA.ID_DIETA).ID_CENTRO_UBI.Value,
                                            ID_ATENCION_MEDICA = ListDietasAux.Where(w => w.ELEGIDO).First(a => a.DIETA.ID_DIETA == item.DIETA.ID_DIETA).ID_ATENCION_MEDICA.Value,
                                            //item.ID_CENTRO_UBI.HasValue ? item.ID_CENTRO_UBI.Value : GlobalVar.gCentro,
                                            //ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA.HasValue ? item.ID_ATENCION_MEDICA.Value : 0,
                                            ID_DIETA = item.DIETA.ID_DIETA,
                                            ESTATUS = "N",
                                        });
                                    }
                                }
                            }
                            #endregion

                            #region PROCEDIMIENTOS MEDICOS
                            var ProcedimientosCitas = new List<ATENCION_CITA>();
                            var idCita = 0;
                            var cit = new ATENCION_CITA();
                            var ProcAtMedProg = new PROC_ATENCION_MEDICA_PROG();
                            if (ListProcMedsSeleccionados != null)
                            {
                                foreach (var item in ListProcMedsSeleccionados.SelectMany(s => s.CITAS).OrderBy(o => o.FECHA_INICIAL))
                                {
                                    if (new cAtencionCita().ObtieneCitaOtraArea(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO,
                                        SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, item.FECHA_INICIAL, item.FECHA_FINAL, AtencionCita != null ? AtencionCita.ID_CITA : 0, GlobalVar.gCentro) != null)
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(delegate
                                        {
                                            new Dialogos().ConfirmacionDialogo("Algo pasó...", "Existe un traslape en la fecha [ " + item.FECHA_INICIAL.ToString("dd/MM/yyyy HH:mm") + " ].");
                                        }));
                                        return;
                                    }
                                    idCita = new cNotaMedica().GetSequence<short>("ATENCION_CITA_SEQ");
                                    cit = new ATENCION_CITA
                                    {
                                        ESTATUS = "N",
                                        ID_CITA = idCita,
                                        ID_USUARIO = GlobalVar.gUsr,
                                        ID_RESPONSABLE = item.ENFERMERO,
                                        ID_ANIO = AtencionMedica.ID_ANIO,
                                        ID_CENTRO_UBI = GlobalVar.gCentro,
                                        CITA_FECHA_HORA = item.FECHA_INICIAL,
                                        CITA_HORA_TERMINA = item.FECHA_FINAL,
                                        ID_AREA = (short)enumAreas.MEDICA_PA,
                                        ID_CENTRO = AtencionMedica.ID_CENTRO,
                                        ID_INGRESO = AtencionMedica.ID_INGRESO,
                                        ID_IMPUTADO = AtencionMedica.ID_IMPUTADO,
                                        ID_TIPO_ATENCION = SelectTipoAtencion.ID_TIPO_ATENCION,
                                        ID_TIPO_SERVICIO = (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS,
                                    };
                                    if (ProcAtMedProg != null ? !(ProcAtMedProg.ID_PROCMED == item.ID_PROC_MED && ProcAtMedProg.ID_USUARIO_ASIGNADO == GlobalVar.gUsr && ProcAtMedProg.ID_CITA == idCita) : false)
                                    {
                                        ProcAtMedProg = new PROC_ATENCION_MEDICA_PROG
                                        {
                                            ESTATUS = "N",
                                            ID_CITA = idCita,
                                            ID_PROCMED = item.ID_PROC_MED,
                                            ID_CENTRO_UBI = GlobalVar.gCentro,
                                            ID_CENTRO_CITA = GlobalVar.gCentro,
                                            ID_USUARIO_ASIGNADO = GlobalVar.gUsr,
                                            PROC_ATENCION_MEDICA = new PROC_ATENCION_MEDICA
                                            {
                                                ID_PROCMED = item.ID_PROC_MED,
                                                ID_USUARIO = GlobalVar.gUsr,
                                                OBSERV = ListProcMedsSeleccionados.Any(f => f.ID_PROC_MED == item.ID_PROC_MED) ? ListProcMedsSeleccionados.First(f => f.ID_PROC_MED == item.ID_PROC_MED).OBSERV : string.Empty,
                                                REGISTRO_FEC = hoy,
                                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                            }
                                        };
                                        cit.PROC_ATENCION_MEDICA_PROG.Add(ProcAtMedProg);
                                    }
                                    ProcedimientosCitas.Add(cit);
                                }
                            }
                            #endregion

                            #region HISTORIA CLINICA
                            if (LstCondensadoPatologicos != null ? LstCondensadoPatologicos.Any() : false)
                                patologicos = LstCondensadoPatologicos.Select(sel =>
                                    new HISTORIA_CLINICA_PATOLOGICOS
                                    {
                                        ID_ANIO = sel.HISTORIA_CLINICA_PATOLOGICOS.ID_ANIO,
                                        ID_CENTRO = sel.HISTORIA_CLINICA_PATOLOGICOS.ID_CENTRO,
                                        ID_IMPUTADO = sel.HISTORIA_CLINICA_PATOLOGICOS.ID_IMPUTADO,
                                        ID_INGRESO = sel.HISTORIA_CLINICA_PATOLOGICOS.ID_INGRESO,
                                        ID_CONSEC = sel.HISTORIA_CLINICA_PATOLOGICOS.ID_CONSEC,
                                        ID_PATOLOGICO = sel.HISTORIA_CLINICA_PATOLOGICOS.ID_PATOLOGICO,
                                        MOMENTO_DETECCION = sel.HISTORIA_CLINICA_PATOLOGICOS.MOMENTO_DETECCION,
                                        OTROS_DESCRIPCION = sel.HISTORIA_CLINICA_PATOLOGICOS.OTROS_DESCRIPCION,
                                        RECUPERADO = sel.RECUPERADO ? "S" : "N",
                                        REGISTRO_FEC = sel.HISTORIA_CLINICA_PATOLOGICOS.REGISTRO_FEC,
                                        OBSERVACIONES = sel.OBSERVACIONES,
                                        ID_NOPATOLOGICO = sel.HISTORIA_CLINICA_PATOLOGICOS.ID_NOPATOLOGICO
                                    }).ToList();
                            #endregion

                            #region MUJERES
                            if (SelectIngreso.IMPUTADO.SEXO == "F")
                            {
                                NotaMujeres = MujeresAuxiliar != null ?
                                    ((TextAniosRitmo != (string.IsNullOrEmpty(MujeresAuxiliar.ANIOS_RITMO) ? MujeresAuxiliar.ANIOS_RITMO : MujeresAuxiliar.ANIOS_RITMO.Trim())) ||
                                    (TextEmbarazos != MujeresAuxiliar.EMBARAZO) ||
                                    (TextPartos != MujeresAuxiliar.PARTO) ||
                                    (TextAbortos != MujeresAuxiliar.ABORTO) ||
                                    (TextCesareas != MujeresAuxiliar.CESAREA) ||
                                    (FechaUltimaMenstruacion != MujeresAuxiliar.ULTIMA_MENSTRUACION_FEC) ||
                                    (TextDeformacionesOrganicas != (string.IsNullOrEmpty(MujeresAuxiliar.DEFORMACION) ? MujeresAuxiliar.DEFORMACION : MujeresAuxiliar.DEFORMACION.Trim())) ||
                                    (FechaProbParto != MujeresAuxiliar.FECHA_PROBABLE_PARTO) ||
                                    (MujeresAuxiliar.ID_CONTROL_PRENATAL.HasValue ? SelectControlPreNatal != MujeresAuxiliar.ID_CONTROL_PRENATAL.Value : SelectControlPreNatal != -1))
                                : false;
                                if (ControlPreNatal == 0 ? SelectControlPreNatal == -1 : false)
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        new Dialogos().ConfirmacionDialogo("Notificación!", "Favor de seleccionar el control prenatal que utiliza.");
                                    }));
                                    return;
                                }
                            }

                            Mujeres = null;
                            if (NotaMujeres)
                            {
                                Mujeres = new HISTORIA_CLINICA_GINECO_OBSTRE
                                {
                                    ABORTO = MujeresAuxiliar != null ? MujeresAuxiliar.ABORTO == TextAbortos ? null : TextAbortos : TextAbortos,
                                    ABORTO_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.ABORTO == TextAbortos ? null : "S" : "S",
                                    ANIOS_RITMO = MujeresAuxiliar != null ? MujeresAuxiliar.ANIOS_RITMO == TextAniosRitmo ? null : TextAniosRitmo : TextAniosRitmo,
                                    ANIOS_RITMO_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.ANIOS_RITMO == TextAniosRitmo ? null : "S" : "S",
                                    CESAREA = MujeresAuxiliar != null ? MujeresAuxiliar.CESAREA == TextCesareas ? null : TextCesareas : TextCesareas,
                                    CESAREA_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.CESAREA == TextCesareas ? null : "S" : "S",
                                    CONTROL_PRENATAL = MujeresAuxiliar != null ?
                                        MujeresAuxiliar.CONTROL_PRENATAL == (ControlPreNatal == 0 ?
                                            "S"
                                        : ControlPreNatal == 1 ?
                                            "N"
                                        : null) ?
                                            null
                                        : ControlPreNatal == 0 ?
                                            "S"
                                        : ControlPreNatal == 1 ?
                                            "N"
                                        : null
                                    : null,
                                    CONTROL_PRENATAL_MODIFICADO = MujeresAuxiliar != null ?
                                        MujeresAuxiliar.CONTROL_PRENATAL == (ControlPreNatal == 0 ?
                                            "S"
                                        : ControlPreNatal == 1 ?
                                            "N"
                                        : null) ?
                                            null
                                        : "S"
                                    : null,
                                    ID_CONTROL_PRENATAL = MujeresAuxiliar != null ?
                                        MujeresAuxiliar.ID_CONTROL_PRENATAL == SelectControlPreNatal ?
                                            null
                                        : SelectControlPreNatal == -1 ? null : SelectControlPreNatal
                                    : SelectControlPreNatal == -1 ? null : SelectControlPreNatal,
                                    ID_CONTROL_PRENATAL_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.ID_CONTROL_PRENATAL == SelectControlPreNatal ? null : "S" : "S",
                                    DEFORMACION = MujeresAuxiliar != null ? MujeresAuxiliar.DEFORMACION == TextDeformacionesOrganicas ? null : TextDeformacionesOrganicas : TextDeformacionesOrganicas,
                                    DEFORMACION_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.DEFORMACION == TextDeformacionesOrganicas ? null : "S" : "S",
                                    EMBARAZO = MujeresAuxiliar != null ? MujeresAuxiliar.EMBARAZO == TextEmbarazos ? new Nullable<short>() : TextEmbarazos : TextEmbarazos,
                                    EMBARAZO_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.EMBARAZO == TextEmbarazos ? null : "S" : "S",
                                    FECHA_PROBABLE_PARTO = MujeresAuxiliar != null ? MujeresAuxiliar.FECHA_PROBABLE_PARTO == FechaProbParto ? new Nullable<DateTime>() : FechaProbParto : FechaProbParto,
                                    FECHA_PROBABLE_PARTO_MOD = MujeresAuxiliar != null ? MujeresAuxiliar.FECHA_PROBABLE_PARTO == FechaProbParto ? null : "S" : "S",
                                    PARTO = MujeresAuxiliar != null ? MujeresAuxiliar.PARTO == TextPartos ? new Nullable<short>() : TextPartos : TextPartos,
                                    PARTO_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.PARTO == TextPartos ? null : "S" : "S",
                                    ULTIMA_MENSTRUACION_FEC = MujeresAuxiliar != null ? MujeresAuxiliar.ULTIMA_MENSTRUACION_FEC == FechaUltimaMenstruacion ? new Nullable<DateTime>() : FechaUltimaMenstruacion : FechaUltimaMenstruacion,
                                    ULTIMA_MENS_MODIFICADO = MujeresAuxiliar != null ? MujeresAuxiliar.ULTIMA_MENSTRUACION_FEC == FechaUltimaMenstruacion ? null : "S" : "S",
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    FUENTE = "NM",
                                    MOMENTO_REGISTRO = "DI",
                                    REGISTRO_FEC = hoy,
                                };
                            }
                            #endregion

                            if (NuevaNota)
                            {
                                if (!new cNotaEvolucion().InsertarNotaEvolucion(AtencionMedica, NotaEvol, NotaSignosVitales, NotaMed, EnfermedadesPorAgregar, receta, RecetasCanceladas, RecetasAgregadas, DietasCanceladas,
                                    DietasPorAgregar, ProcedimientosCitas, patologicos, Mujeres, (short)enumMensajeTipo.DETECCION_AREA_MEDICA_GRUPO_VULNERABLE))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        new Dialogos().ConfirmacionDialogo("Notificación!", "Ocurrió un error al intentar guardar la información de la nota de evolución.");
                                    }));
                                    return;
                                }
                            }
                            else if (AgregarNota)
                            {
                                AtencionMedica = new ATENCION_MEDICA
                                {
                                    ID_ANIO = NotaEvolucion.ID_ANIO,
                                    ID_CENTRO = NotaEvolucion.ID_CENTRO,
                                    ID_INGRESO = NotaEvolucion.ID_INGRESO,
                                    ID_HOSPITA = NotaEvolucion.ID_HOSPITA,
                                    ID_IMPUTADO = NotaEvolucion.ID_IMPUTADO,
                                    ATENCION_FEC = NotaEvolucion.ATENCION_FEC,
                                    ID_CENTRO_UBI = NotaEvolucion.ID_CENTRO_UBI,
                                    ID_TIPO_ATENCION = NotaEvolucion.ID_TIPO_ATENCION,
                                    ID_TIPO_SERVICIO = NotaEvolucion.ID_TIPO_SERVICIO,
                                    ID_ATENCION_MEDICA = NotaEvolucion.ID_ATENCION_MEDICA,
                                    ID_CITA_SEGUIMIENTO = NotaEvolucion.ID_CITA_SEGUIMIENTO,
                                };
                                NotaEvol.REGISTRO_FEC = NotaEvolucion.ATENCION_FEC;
                                NotaEvol.ESTUDIO_RESULTADO = LabelEstudios + (string.IsNullOrEmpty(TextEstudios) ? string.Empty : ("\n" + TextEstudios));
                                NotaSignosVitales.ID_ATENCION_MEDICA = NotaEvolucion.ID_ATENCION_MEDICA;
                                NotaMed = new NOTA_MEDICA
                                {
                                    DIAGNOSTICO = NotaEvolucion.NOTA_MEDICA.DIAGNOSTICO,
                                    EXPLORACION_FISICA = LabelExploracionFisica + (string.IsNullOrEmpty(TextExploracionFisica) ? string.Empty : ("\n" + TextExploracionFisica)),
                                    FIRMO_CONFORMIDAD = NotaEvolucion.NOTA_MEDICA.FIRMO_CONFORMIDAD,
                                    ID_ATENCION_MEDICA = NotaEvolucion.NOTA_MEDICA.ID_ATENCION_MEDICA,
                                    ID_CENTRO_UBI = NotaEvolucion.NOTA_MEDICA.ID_CENTRO_UBI,
                                    ID_PRONOSTICO = SelectPronostico,
                                    ID_RESPONSABLE = NotaEvolucion.NOTA_MEDICA.ID_RESPONSABLE,
                                    OCUPA_HOSPITALIZACION = NotaEvolucion.NOTA_MEDICA.OCUPA_HOSPITALIZACION,// == "N" ? CheckedHospitalizacion ? "S" : NotaEvolucion.NOTA_MEDICA.OCUPA_HOSPITALIZACION : NotaEvolucion.NOTA_MEDICA.OCUPA_HOSPITALIZACION,
                                    OCUPA_INTERCONSULTA = SolicitaInterconsultaCheck ? "S" : "N",
                                    PLAN_ESTUDIO_TRATA = NotaEvolucion.NOTA_MEDICA.PLAN_ESTUDIO_TRATA,
                                    RESULTADO_SERV_AUX = NotaEvolucion.NOTA_MEDICA.RESULTADO_SERV_AUX,
                                    RESULTADO_SERV_TRA = NotaEvolucion.NOTA_MEDICA.RESULTADO_SERV_TRA,
                                    RESUMEN_INTERROGAT = LabelResumenInterrogatorio + (string.IsNullOrEmpty(TextResumenInterrogatorio) ? string.Empty : ("\n" + TextResumenInterrogatorio)),
                                };
                                if (!new cNotaEvolucion().ActualizarNotaEvolucion(AtencionMedica, NotaEvol, NotaSignosVitales, NotaMed, EnfermedadesPorAgregar, receta, RecetasCanceladas, RecetasAgregadas, DietasCanceladas,
                                    DietasPorAgregar, ProcedimientosCitas, (short)enumMensajeTipo.DETECCION_AREA_MEDICA_GRUPO_VULNERABLE, patologicos))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        new Dialogos().ConfirmacionDialogo("Notificación!", "Ocurrió un error al intentar actualizar la información de la nota de evolución.");
                                    }));
                                    return;
                                }
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "Ocurrió un error al guardar la información de la nota de evolución.");
                                }));
                                return;
                            }
                            Application.Current.Dispatcher.Invoke((Action)(async delegate
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Exito!", "La nota ha sido guardada correctamente.");
                                clickSwitch("limpiar_menu");
                            }));
                            return;
                        });
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la información de la nota de evolución.", ex);
                    }
                    break;
                default:
                    break;
            }

            return;
        }

        private void CargarPatologicos()
        {
            try
            {
                HistoriaClinicaEnabled = SelectIngreso.HISTORIA_CLINICA.Any();
                ListCondensadoPatologicosAux = SelectIngreso.HISTORIA_CLINICA.Any() ?
                    new ObservableCollection<HistoriaclinicaPatologica>(SelectIngreso.HISTORIA_CLINICA.OrderByDescending(o => o.ID_CONSEC).FirstOrDefault().HISTORIA_CLINICA_PATOLOGICOS
                    .Select(s => new HistoriaclinicaPatologica
                    {
                        HISTORIA_CLINICA_PATOLOGICOS = new HISTORIA_CLINICA_PATOLOGICOS
                        {
                            ID_ANIO = s.ID_ANIO,
                            ID_CENTRO = s.ID_CENTRO,
                            ID_CONSEC = s.ID_CONSEC,
                            ID_IMPUTADO = s.ID_IMPUTADO,
                            ID_INGRESO = s.ID_INGRESO,
                            ID_NOPATOLOGICO = s.ID_NOPATOLOGICO,
                            ID_PATOLOGICO = s.ID_PATOLOGICO,
                            MOMENTO_DETECCION = s.MOMENTO_DETECCION,
                            OBSERVACIONES = s.OBSERVACIONES,
                            OTROS_DESCRIPCION = s.OTROS_DESCRIPCION,
                            RECUPERADO = s.RECUPERADO,
                            REGISTRO_FEC = s.REGISTRO_FEC,
                        },
                        PATOLOGICO_CAT = s.PATOLOGICO_CAT,
                        REGISTRO_FEC = s.REGISTRO_FEC.HasValue ? s.REGISTRO_FEC.Value : new DateTime(),
                        OBSERVACIONES = s.OBSERVACIONES,
                        RECUPERADO = s.RECUPERADO == "S",
                        DESHABILITADO = !(s.RECUPERADO == "S"),
                        ELIMINABLE = false,
                    }))
                : new ObservableCollection<HistoriaclinicaPatologica>();
                LstCondensadoPatologicos = new ObservableCollection<HistoriaclinicaPatologica>(ListCondensadoPatologicosAux.OrderBy(o => !o.RECUPERADO).ThenBy(t => t.PATOLOGICO_CAT.DESCR));

                MujeresVisible = SelectIngreso.IMPUTADO.SEXO == "F" ? Visibility.Visible : Visibility.Collapsed;
                if (SelectIngreso.HISTORIA_CLINICA.Any() && SelectIngreso.IMPUTADO.SEXO == "F")
                {
                    MujeresEnabled = !AgregarNota;
                    var hc = selectIngreso.HISTORIA_CLINICA.OrderByDescending(o => o.ID_CONSEC).FirstOrDefault();
                    CheckMenarquia = hc.MU_MENARQUIA;
                    MenarquiaEnabled = !hc.MU_MENARQUIA.HasValue;
                    MujeresAuxiliar = new HISTORIA_CLINICA_GINECO_OBSTRE();
                    if (hc.HISTORIA_CLINICA_GINECO_OBSTRE.Any())
                    {
                        var gineco = hc.HISTORIA_CLINICA_GINECO_OBSTRE.OrderBy(o => o.ID_GINECO).FirstOrDefault();
                        MujeresAuxiliar.ANIOS_RITMO = TextAniosRitmo = gineco.ANIOS_RITMO;
                        MujeresAuxiliar.EMBARAZO = TextEmbarazos = gineco.EMBARAZO;
                        MujeresAuxiliar.PARTO = TextPartos = gineco.PARTO;
                        MujeresAuxiliar.ABORTO = TextAbortos = gineco.ABORTO;
                        MujeresAuxiliar.CESAREA = TextCesareas = gineco.CESAREA;
                        MujeresAuxiliar.ULTIMA_MENSTRUACION_FEC = FechaUltimaMenstruacion = gineco.ULTIMA_MENSTRUACION_FEC;
                        MujeresAuxiliar.DEFORMACION = TextDeformacionesOrganicas = gineco.DEFORMACION;
                        MujeresAuxiliar.CONTROL_PRENATAL = gineco.CONTROL_PRENATAL;
                        ControlPreNatal = (short)(gineco.CONTROL_PRENATAL == "N" ? 1 : gineco.CONTROL_PRENATAL == "S" ? 0 : -1);
                        if (ControlPreNatal == 1)
                        {
                            MujeresAuxiliar.ID_CONTROL_PRENATAL = gineco.ID_CONTROL_PRENATAL;
                            SelectControlPreNatal = (short)(gineco.ID_CONTROL_PRENATAL.Value > 0 ? gineco.ID_CONTROL_PRENATAL.Value : -1);
                        }
                        MujeresAuxiliar.FECHA_PROBABLE_PARTO = FechaProbParto = gineco.FECHA_PROBABLE_PARTO;
                        foreach (var item in hc.HISTORIA_CLINICA_GINECO_OBSTRE.OrderBy(o => o.ID_GINECO))
                        {
                            if (item.ANIOS_RITMO_MODIFICADO == "S")
                            {
                                MujeresAuxiliar.ANIOS_RITMO = item.ANIOS_RITMO;
                                TextAniosRitmo = item.ANIOS_RITMO;
                            }
                            if (item.CESAREA_MODIFICADO == "S")
                            {
                                MujeresAuxiliar.CESAREA = item.CESAREA;
                                TextCesareas = item.CESAREA;
                            }
                            if (item.DEFORMACION_MODIFICADO == "S")
                            {
                                MujeresAuxiliar.DEFORMACION = item.DEFORMACION;
                                TextDeformacionesOrganicas = item.DEFORMACION;
                            }
                            if (item.EMBARAZO_MODIFICADO == "S")
                            {
                                MujeresAuxiliar.EMBARAZO = item.EMBARAZO;
                                TextEmbarazos = item.EMBARAZO;
                            }
                            if (item.PARTO_MODIFICADO == "S")
                            {
                                MujeresAuxiliar.PARTO = item.PARTO;
                                TextPartos = item.PARTO;
                            }
                            if (item.ABORTO_MODIFICADO == "S")
                            {
                                MujeresAuxiliar.ABORTO = item.ABORTO;
                                TextAbortos = item.ABORTO;
                            }
                            if (item.ULTIMA_MENS_MODIFICADO == "S")
                            {
                                MujeresAuxiliar.ULTIMA_MENSTRUACION_FEC = item.ULTIMA_MENSTRUACION_FEC;
                                FechaUltimaMenstruacion = item.ULTIMA_MENSTRUACION_FEC;
                            }
                            if (item.CONTROL_PRENATAL_MODIFICADO == "S")
                            {
                                MujeresAuxiliar.CONTROL_PRENATAL = item.CONTROL_PRENATAL;
                                ControlPreNatal = (short)(item.CONTROL_PRENATAL == "N" ? 1 : item.CONTROL_PRENATAL == "S" ? 0 : -1);
                                MujeresAuxiliar.ID_CONTROL_PRENATAL = SelectControlPreNatal = (short)(gineco.ID_CONTROL_PRENATAL.HasValue ? gineco.ID_CONTROL_PRENATAL.Value > 0 ? gineco.ID_CONTROL_PRENATAL.Value : -1 : -1);
                            }
                            if (item.ID_CONTROL_PRENATAL_MODIFICADO == "S" ? item.ID_CONTROL_PRENATAL != null : false)
                            {
                                MujeresAuxiliar.ID_CONTROL_PRENATAL = item.ID_CONTROL_PRENATAL;
                                SelectControlPreNatal = (short)(item.ID_CONTROL_PRENATAL.Value > 0 ? item.ID_CONTROL_PRENATAL.Value : -1);
                            }
                            if (item.FECHA_PROBABLE_PARTO_MOD == "S")
                            {
                                MujeresAuxiliar.FECHA_PROBABLE_PARTO = item.FECHA_PROBABLE_PARTO;
                                FechaProbParto = item.FECHA_PROBABLE_PARTO;
                            }
                            if (item.ULTIMA_MENS_MODIFICADO == "S")
                            {
                                MujeresAuxiliar.ULTIMA_MENSTRUACION_FEC = item.ULTIMA_MENSTRUACION_FEC;
                                FechaUltimaMenstruacion = item.ULTIMA_MENSTRUACION_FEC;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso.", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.NOTA_EVOLUCION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = true;
                        if (p.EDITAR == 1)
                            PEditar = true;
                        if (p.IMPRIMIR == 1)
                            PImprimir = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region ENFERMEDAD

        private void listBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
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
                    ListEnfermedades = ListEnfermedades ?? new ObservableCollection<NOTA_MEDICA_ENFERMEDAD>();
                    if (!ListEnfermedades.Any(a => a.ID_ENFERMEDAD == ((ENFERMEDAD)item).ID_ENFERMEDAD))
                    {
                        var notaenfermedad = (ENFERMEDAD)item;
                        var hoy = Fechas.GetFechaDateServer;
                        ListEnfermedades.Insert(0, new NOTA_MEDICA_ENFERMEDAD
                        {
                            ID_ATENCION_MEDICA = 0,
                            ID_CENTRO_UBI = GlobalVar.gCentro,
                            REGISTRO_FEC = hoy,
                            ID_ENFERMEDAD = notaenfermedad.ID_ENFERMEDAD,
                            ENFERMEDAD = notaenfermedad,
                        });
                        AutoCompleteTB.Text = string.Empty;
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
                        ListEnfermedades = ListEnfermedades ?? new ObservableCollection<NOTA_MEDICA_ENFERMEDAD>();
                        if (!ListEnfermedades.Any(a => a.ID_ENFERMEDAD == ((ENFERMEDAD)item).ID_ENFERMEDAD))
                        {
                            var notaenfermedad = (ENFERMEDAD)item;
                            var hoy = Fechas.GetFechaDateServer;
                            ListEnfermedades.Insert(0, new NOTA_MEDICA_ENFERMEDAD
                            {
                                ID_ATENCION_MEDICA = 0,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                REGISTRO_FEC = hoy,
                                ID_ENFERMEDAD = notaenfermedad.ID_ENFERMEDAD,
                                ENFERMEDAD = notaenfermedad,
                            });
                            AutoCompleteTB.Text = string.Empty;
                            AutoCompleteTB.Focus();
                        }
                        else
                            popup.IsOpen = false;
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

        #region RECETA

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
                            PRESENTACION = new Nullable<short>(),
                            PRODUCTO = ((RecetaMedica)item).PRODUCTO,
                            HABILITAR = true,
                        });
                        AutoCompleteReceta.Text = string.Empty;
                        AutoCompleteReceta.Focus();
                    }
                    else
                        popup.IsOpen = false;
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
                                HABILITAR = true,
                            });
                            AutoCompleteReceta.Text = string.Empty;
                            AutoCompleteReceta.Focus();
                        }
                        else
                            popup.IsOpen = false;
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
                Window.Closed += (s, e) =>
                {
                    try
                    {
                        if (OnProgress == null) return;
                        if (!_IsSucceed) SelectRegistro = null;
                        OnProgress.Abort();
                        CancelCaptureAndCloseReader(OnCaptured);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar búsqueda.", ex);
                    }
                };
                if (CurrentReader != null)
                {
                    CurrentReader.Dispose();
                    CurrentReader = null;
                }
                CurrentReader = Readers[0];
                if (CurrentReader == null) return;
                if (!OpenReader()) Window.Close();
                if (!StartCaptureAsync(OnCaptured)) Window.Close();
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
                if (ScannerMessage.Contains("Procesando...")) return;
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
                        if (ParametroEstatusInactivos.Any(a => a.HasValue ? a.Value == SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO : false))
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
                        NotaMedicaVisible = Visibility.Visible;
                        ListaImputadosVisible = Visibility.Collapsed;
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
                if (ScannerMessage.Contains("Procesando...")) return;
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    TextNipBusqueda = string.Empty;
                    PropertyImage = new BitmapImage();
                    ShowLoading = Visibility.Visible;
                    ShowCapturar = Visibility.Collapsed;
                    ShowLine = Visibility.Visible;
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                }));
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
                            if (pBuffer != IntPtr.Zero) Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);
                            CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                            var bufferWSQ = new byte[nBufferLength];
                            if (pBuffer != IntPtr.Zero) Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);
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
                            if (!CancelKeepSearching) await KeepSearch();
                            else
                                if (!_GuardarHuellas) break;
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda por huella.", ex);
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
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = Finger == null ? "Vuelve a capturar las huellas" : ScannerMessage = "Siguiente Huella";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        ShowLine = Visibility.Collapsed;
                    }));
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                    AceptarBusquedaHuellaFocus = false;
                }));
                var Service = new BiometricoServiceClient();
                if (Service == null)
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Error en el servicio de comparación";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        ShowLine = Visibility.Collapsed;
                    }));
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
                    if (HuellaCompromiso)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            HuellaWindow.Close();
                            CartaView.Close();
                            HuellaCompromiso = false;
                            clickSwitch("acepto_compromiso");
                        }));
                    }
                    else
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
                            AMaterno = string.IsNullOrEmpty(SelectExpediente.MATERNO) ? string.Empty : SelectExpediente.MATERNO.Trim(),
                            APaterno = SelectExpediente.PATERNO.Trim(),
                            Nombre = SelectExpediente.NOMBRE.Trim(),
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
                        if (Finger != null) Service.Close();
                        return TaskEx.FromResult(false);
                    }
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
                    if (!CancelKeepSearching) _SelectRegistro = null;
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
                        ScannerMessage = Finger == null ? "Vuelve a capturar las huellas" : "Siguiente Huella";
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
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Error en el servicio de comparación";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        ShowLine = Visibility.Collapsed;
                    }));
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
                        if (persona == null) continue;
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            var perosonabiometrico = new cPersonaBiometrico().GetData().Where(w => w.ID_PERSONA == persona.ID_PERSONA).ToList();
                            var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                            if (perosonabiometrico != null ? perosonabiometrico.Any() : false)
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

                        if (Finger != null) Service.Close();
                        return TaskEx.FromResult(false);
                    }
                    else
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
                    if (!CancelKeepSearching) _SelectRegistro = null;
                    PropertyImage = null;
                }
                Service.Close();
                FingerPrintData = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda por huella.", ex);
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la búsqueda por huella.", ex);
            }
        }

        private async void OnBuscarPorHuella(string obj = "")
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
                await TaskEx.Delay(400);
                var nRet = -1;
                var bandera = true;
                if (ParametroGuardarHuellaEnBusquedaPadronVisita)
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
                ConstructorHuella(enumTipoPersona.IMPUTADO, nRet == 0, ParametroGuardarHuellaEnBusquedaPadronVisita);
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
                    if (bandera == true) CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    if (!IsSucceed) return;
                    if (SelectRegistro != null ? SelectRegistro.Imputado == null : null == null) return;
                };
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }
        #endregion

        #region AGENDA
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
                if (ProcedimientoMedicoParaAgenda == null)
                {
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
                else
                {
                    if (SelectedEmpleadoValue.ID_EMPLEADO > 0)
                    {
                        if (BuscarAgenda)
                        {
                            if (SelectProcMedSeleccionado != null ?
                                SelectProcMedSeleccionado.CITAS != null ?
                                    SelectProcMedSeleccionado.CITAS.Any(a => a.FECHA_INICIAL.Year == AHoraI.Value.Year && a.FECHA_INICIAL.Month == AHoraI.Value.Month && a.FECHA_INICIAL.Day == AHoraI.Value.Day &&
                                        a.FECHA_INICIAL.Hour == AHoraI.Value.Hour && a.FECHA_INICIAL.Minute == AHoraI.Value.Minute)
                                : false
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
                            if (ListProcMedsSeleccionados != null)
                            {
                                ProcedimientosMedicosEnCitaEnMemoria = new ObservableCollection<CustomCitasProcedimientosMedicos>(ListProcMedsSeleccionados.Where(w => w.CITAS != null).SelectMany(s => s.CITAS)
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
                ProcedimientosMedicosSeleccionadosAgenda = "";
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la agenda.", ex);
            }
        }

        private void AgendaLoaded(object sender, EventArgs e)
        {
            try
            {
                AgendaView.DataContext = this;
                selectedDateBusqueda = DateTime.Now;
                OnPropertyChanged("SelectedDateBusqueda");
                var _usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().Obtener(StaticSourcesViewModel.UsuarioLogin.Username);
                TituloAgenda = "AGENDA DE: " + SelectedEmpleadoValue.NOMBRE_COMPLETO + "\tPACIENTE: " +
                    SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? "" : SelectIngreso.IMPUTADO.MATERNO.Trim());
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

        private async void AgendaClick(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!AFecha.HasValue)
                {
                    MensajeError = "Favor de validar el campo de la fecha.";
                    return;
                }
                if (!AHoraI.HasValue)
                {
                    MensajeError = "Favor de validar el campo de la hora inicial.";
                    return;
                }
                if (!AHoraF.HasValue)
                {
                    MensajeError = "Favor de validar el campo de la hora final.";
                    return;
                }
                var hoy = Fechas.GetFechaDateServer;
                var _nueva_fecha_inicio = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraI.Value.Hour, AHoraI.Value.Minute, AHoraI.Value.Second);
                if (hoy > _nueva_fecha_inicio)
                {
                    MensajeError = "La fecha y hora deben de ser mayor a la actual";
                    return;
                }
                var _nueva_fecha_final = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraF.Value.Hour, AHoraF.Value.Minute, AHoraF.Value.Second);
                if (_nueva_fecha_inicio > _nueva_fecha_final)
                {
                    MensajeError = "La fecha inicial no puede ser mayor a la fecha final";
                    return;
                }

                if (AHoraI.Value.Minute % 15 != 0 || AHoraF.Value.Minute % 15 != 0)
                {
                    MensajeError = "Los intervalos de atención tienen que ser en bloques de 15 minutos";
                    return;
                }
                if (AFecha != null)
                {
                    if (ProcedimientoMedicoParaAgenda == null)
                    {
                        if (AtencionCitaSeguimiento != null)
                        {
                            LstAgenda.Remove(LstAgenda.FirstOrDefault(w => w.StartTime == AtencionCitaSeguimiento.CITA_FECHA_HORA && w.EndTime == AtencionCitaSeguimiento.CITA_HORA_TERMINA));
                        }
                        LstAgenda = LstAgenda ?? new ObservableCollection<Appointment>();
                        LstAgenda.Add(new Appointment()
                        {
                            Subject = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty),
                            StartTime = _nueva_fecha_inicio,
                            EndTime = _nueva_fecha_final,
                        });
                        var user = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault();
                        AtencionCitaSeguimiento = new ATENCION_CITA
                        {
                            CITA_FECHA_HORA = _nueva_fecha_inicio,
                            CITA_HORA_TERMINA = _nueva_fecha_final,
                            ESTATUS = ParametroEstatusCitaSinAtender,
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_TIPO_ATENCION = IsMedico ? (short)enumAtencionTipo.CONSULTA_MEDICA : IsDentista ? (short)enumAtencionTipo.CONSULTA_DENTAL : new Nullable<short>(),
                            ID_TIPO_SERVICIO = IsMedico ? (short)enumAtencionServicio.CITA_MEDICA : IsDentista ? (short)enumAtencionServicio.CITA_MEDICA_DENTAL : new Nullable<short>(),
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
                        if (string.IsNullOrEmpty(ProcedimientoMedicoParaAgenda.PROC_MED_DESCR))
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
                            if (AgregarNota)
                                if (new cAtencionCita().ObtieneCitaOtraArea(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO,
                                    SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, AHoraI.Value, AHoraF.Value, AtencionCita != null ? AtencionCita.ID_CITA : 0, GlobalVar.gCentro) != null)
                                {
                                    AgendaView.Hide();
                                    await new Dialogos().ConfirmacionDialogoReturn("Algo pasó...", "Existe un traslape en la fecha y hora seleccionada [ " + AHoraI.Value.ToString("dd/MM/yyyy HH:mm tt") + " ].");
                                    AgendaView.Show();
                                    return;
                                }
                            foreach (var item in ListProcMedsSeleccionados.Where(f => f.ID_PROC_MED == ProcedimientoMedicoParaAgenda.ID_PROC_MED))
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
                            Subject = string.Format("{0}", SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim())),
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

        private async Task<List<cSolicitudCita>> SegmentarBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<cSolicitudCita>>(() =>
                    new ObservableCollection<cSolicitudCita>(new cAtencionIngreso().ObtenerTodoEnAreas(GlobalVar.gCentro, true, null, CAnio, CFolio, CNombre, CPaterno, CMaterno,
                        CArea != -1 ? new List<short> { CArea.Value } : areas_tecnicas_rol, CFecha, CEstatus != -1 ? CEstatus : null, fHoy, _Pag)
                        .Select(s => new cSolicitudCita
                        {
                            ATENCION_INGRESO = s,
                            CANT_SOLICITUDES_ATENDIDAS = s.INGRESO.ATENCION_CITA.Where(w => w.ID_CENTRO_UBI == GlobalVar.gCentro ?
                                w.ATENCION_SOLICITUD != null ?
                                    w.CITA_FECHA_HORA.Value.Month == s.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.Month ?
                                        w.CITA_FECHA_HORA.Value.Year == s.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.Year
                                    : false
                                : false
                            : false).Count(),
                            MAX_ATENCIONES = ParametroSolicitudAtencionPorMes
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

        private void ObtenerAgenda(string usuario, bool isExceptionManaged = false)
        {
            try
            {
                //if (selectedSolicitud != null)
                //{
                lstAgenda = new ObservableCollection<Appointment>();
                var hoy = Fechas.GetFechaDateServer;
                var agenda = new List<ATENCION_CITA>();
                if (ProcedimientoMedicoParaAgenda == null)
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
                    foreach (var item in ListProcMedsSeleccionados.Where(e => e.CITAS != null).SelectMany(s => s.CITAS).Where(w => w.ENFERMERO == SelectedEmpleadoValue.ID_EMPLEADO))
                    {
                        LstAgenda.Add(new Appointment()
                        {
                            Subject = string.Format("{0} {1} {2}", SelectIngreso.IMPUTADO.NOMBRE.Trim(), SelectIngreso.IMPUTADO.PATERNO.Trim(), string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? "" : SelectIngreso.IMPUTADO.MATERNO.Trim()),
                            StartTime = item.FECHA_INICIAL,
                            EndTime = item.FECHA_FINAL,
                        });
                    }
                }
                RaisePropertyChanged("LstAgenda");
                //GuardarAgendaEnabled = true;
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

        private bool GuardarAgenda()
        {
            try
            {
                if (!base.HasErrors)
                {
                    var obj = new ATENCION_CITA();
                    obj.ID_CITA = 0;
                    obj.ESTATUS = ParametroEstatusCitaSinAtender;
                    obj.CITA_FECHA_HORA = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraI.Value.Hour, AHoraI.Value.Minute, 0);
                    obj.CITA_HORA_TERMINA = new DateTime(AFecha.Value.Year, AFecha.Value.Month, AFecha.Value.Day, AHoraF.Value.Hour, AHoraF.Value.Minute, 0);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar agenda.", ex);
            }
            return false;
        }
        #endregion

    }
}