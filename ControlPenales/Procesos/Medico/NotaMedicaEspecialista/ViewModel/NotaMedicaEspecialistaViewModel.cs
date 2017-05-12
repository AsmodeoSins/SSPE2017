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
using SSP.Controlador.Catalogo.Justicia.Medico.CertificadoMedico;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia.Medico;

namespace ControlPenales
{
    partial class NotaMedicaEspecialistaViewModel : FingerPrintScanner
    {
        public NotaMedicaEspecialistaViewModel() { }

        private void TimerStart(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void TimerStop(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private async void NotaMedicaLoad(NotaMedicaView Window = null)
        {
            try
            {
                if (Window.TabListas == null) return;
                if (Window.TabListas.Items == null) return;
                foreach (var item in Window.TabListas.Items)
                {
                    if (!(item is TabItem)) continue;
                    if (((TabItem)item).Header.ToString().Contains("CITAS MEDICAS")) continue;
                    ((TabItem)item).Visibility = Visibility.Collapsed;
                }
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
                        AutoCompleteProcsMeds = Window.AutoCompleteTB_ProcsMeds;
                        AutoCompleteProcsMedsLB = AutoCompleteProcsMeds.Template.FindName("PART_ListBox", Window.AutoCompleteTB_ProcsMeds) as ListBox;
                        AutoCompleteProcsMeds.PreviewMouseDown += new MouseButtonEventHandler(Producto_ProcedimientoMedico_MouseUp);
                        AutoCompleteProcsMeds.KeyDown += Producto_ProcedimientoMedico_KeyDown;
                        AutoCompleteReceta = Window.AutoCompleteReceta;
                        AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", Window.AutoCompleteReceta) as ListBox;
                        AutoCompleteReceta.PreviewMouseDown += new MouseButtonEventHandler(MouseUpReceta);
                        AutoCompleteReceta.KeyDown += KeyDownReceta;
                        #endregion
                        if (Window.CertificadoIngresoWindow == null) return;
                        if (Window.CertificadoIngresoWindow.SeniasFrenteWindow == null) return;
                        if (Window.CertificadoIngresoWindow.SeniasDorsoWindow == null) return;
                        Window.Unloaded += TimerStop;
                        _WindowOdontogramaInicial = Window.OdontogramaView;
                        ValidaRol();
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
                        LstEnfermedadesDentales = new ObservableCollection<ENFERMEDAD>(new cEnfermedades().GetData(x => x.TIPO == "D"));
                        ListTratamientosDentales = new ObservableCollection<DENTAL_TRATAMIENTO>(new cTratamientoDental().GetData(g => g.ESTATUS == "S"));
                        ListMotivosCancelarProcMed = new ObservableCollection<ATENCION_CITA_IN_MOTIVO>(new cAtencion_Cita_In_Motivo().ObtenerTodos());
                        ListAtencionTipo = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                        var rol = new cUsuarioRol().GetData().First(x => x.ID_USUARIO.Contains(StaticSourcesViewModel.UsuarioLogin.Username));
                        if (!(IsDentista && IsMedico))
                        {
                            var lista = new cAtencionServicio().ObtenerTodo().Where(w => w.ESTATUS == "S" && 
                                w.ID_TIPO_ATENCION == (rol.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA ? (short)enumAtencionTipo.CONSULTA_DENTAL : (short)enumAtencionTipo.CONSULTA_MEDICA));
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
                            NOMBRE_COMPLETO = s.PERSONA.NOMBRE.Trim() + " " + s.PERSONA.PATERNO.Trim() + " " + (string.IsNullOrEmpty(s.PERSONA.MATERNO) ? string.Empty : s.PERSONA.MATERNO.Trim()),
                            Usuario = s.USUARIO.First(f => f.ID_PERSONA == s.ID_EMPLEADO)
                        }).ToList();
                        LstTipoServAux = new ObservableCollection<TIPO_SERVICIO_AUX_DIAG_TRAT>(new cTipo_Serv_Aux_Diag_Trat().ObtenerTodos("", "S"));
                        ListProcedimientoMedicoHijoAux = new ObservableCollection<PROC_MED>(new cProcedimientosMedicos().ObtenerTodos().Where(w => w.ESTATUS == "S"));
                        ListProcedimientoSubtipo = new ObservableCollection<PROC_MED_SUBTIPO>(new cProcedimientosSubtipo().ObtenerTodos().Where(w => w.ESTATUS == "S"));
                        ListProcedimientosMedicosEnCitaParaAgregar = new ObservableCollection<PROC_MED>(new cProcedimientosMedicos().ObtenerTodosActivos());
                        ListTipoTratamiento = new ObservableCollection<DENTAL_TIPO_TRATAMIENTO>(new cTratamientoTipoDental().ObtenerActivos());
                        LstPronostico = new ObservableCollection<PRONOSTICO>(new cPronostico().ObtenerTodos("S").OrderBy(o => o.DESCR));
                        ListDietas = new ObservableCollection<DietaMedica>(new cDietas().ObtenerTodosActivos().Where(w => w.ESTATUS == "S").OrderBy(o => o.DESCR).Select(s => new DietaMedica { DIETA = s, ELEGIDO = false }));
                        ListUnidadMedida = new ObservableCollection<PRODUCTO_UNIDAD_MEDIDA>(new cProducto_Unidad_Medida().Seleccionar("S"));
                        LstPatologicos = new ObservableCollection<PatologicosMedicos>(new cPatologicoCat().ObtenerTodo().Where(w => w.ESTATUS == "S").Select(s => new PatologicosMedicos { PATOLOGICO_CAT = s, SELECCIONADO = false }));
                        //ListProcedimientosMedicos = new ObservableCollection<PROC_MED>(new cProcedimientosMedicos().ObtenerTodosActivos().Where(w =>
                        //    w.PROC_MED_SUBTIPO.ID_TIPO_ATENCION == (IsMedico ? (short)enumAtencionTipo.CONSULTA_MEDICA : IsDentista ? (short)enumAtencionTipo.CONSULTA_DENTAL : 0)).OrderBy(o => o.PROC_MED_SUBTIPO.DESCR).ThenBy(t => t.DESCR));
                        ListControlPrenatal = new ObservableCollection<CONTROL_PRENATAL>(new cControlPrenatal().GetData().Where(w => w.ESTATUS == "S"));
                        #endregion
                        ListRadioButonsFrente = new List<RadioButton>();
                        ListRadioButonsDorso = new List<RadioButton>();
                        ListCheckBoxOdontograma = new List<CheckBox>();
                        var hoy = Fechas.GetFechaDateServer;
                        //ListaEspecialistas = new ObservableCollection<ESPECIALISTA>(new cEspecialistas().GetData(g => g.ESPECIALISTA_CITA.Any(a => a.REGISTRO_FEC.HasValue ?
                        //    (a.ATENCION_CITA.CITA_FECHA_HORA.Value.Day == hoy.Day && a.ATENCION_CITA.CITA_FECHA_HORA.Value.Month == hoy.Month && a.ATENCION_CITA.CITA_FECHA_HORA.Value.Year == hoy.Year)
                        //: false)));
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            try
                            {
                                if (IsDentista || IsMedico)
                                {
                                    Window.dgListaCitasMedicasPropias.Columns.First(f => f.Header.ToString().Contains("Medico")).Visibility = Visibility.Collapsed;
                                    Window.dgListaHistoriaClinicaCitas.Columns.First(f => f.Header.ToString().Contains("atencion")).Visibility = Visibility.Collapsed;
                                }
                                ListRadioButonsFrente = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.CertificadoIngresoWindow.SeniasFrenteWindow.FindName("GridFrente"))).ToList();
                                if (SelectLesion != null)
                                    foreach (var item in ListRadioButonsFrente)
                                        item.IsChecked = item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false;
                                ListRadioButonsDorso = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.CertificadoIngresoWindow.SeniasDorsoWindow.FindName("GridDorso"))).ToList();
                                if (SelectLesion != null)
                                    foreach (var item in ListRadioButonsDorso)
                                        item.IsChecked = item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false;
                                ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)Window.OdontogramaView.FindName("GridOdontograma"))).ToList();
                                foreach (var item in ListCheckBoxOdontograma)
                                    item.IsChecked = false;
                                LstEnfermedadesDentales.Insert(0, (new ENFERMEDAD() { ID_ENFERMEDAD = -1, NOMBRE = "SELECCIONE" }));
                                IdSelectedEnfermedadDental = -1;
                                ListTratamientosDentales.Insert(0, (new DENTAL_TRATAMIENTO() { ID_TRATA = -1, DESCR = "SELECCIONE" }));
                                IdSelectedTratamientoDental = -1;
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
                                        NOMBRE_COMPLETO = rol.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() + " " + rol.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() + " " + rol.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim(),
                                        Usuario = rol.USUARIO,
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
                                /*ListTipoTratamiento.Insert(0, new DENTAL_TIPO_TRATAMIENTO { ID_TIPO_TRATA = -1, DESCR = "SELECCIONE" });
                                SelectTipoTratamiento = ListTipoTratamiento.First(f => f.ID_TIPO_TRATA == -1);
                                ListTratamientoOdonto = new ObservableCollection<DENTAL_TRATAMIENTO>();
                                ListTratamientoOdonto.Insert(0, new DENTAL_TRATAMIENTO { ID_TRATA = -1, DESCR = "SELECCIONE" });
                                SelectTratamientoOdonto = ListTratamientoOdonto.First(f => f.ID_TRATA == -1);
                                */
                                LstPronostico.Insert(0, new PRONOSTICO { ID_PRONOSTICO = -1, DESCR = "SELECCIONE", ESTATUS = "S" });
                                ListUnidadMedida.Insert(0, new PRODUCTO_UNIDAD_MEDIDA { ID_UNIDAD_MEDIDA = -1, DESCR = "SELECCIONE", NOMBRE = "SELECCIONE" });
                                ListAtencionTipo.Insert(0, new ATENCION_TIPO { ID_TIPO_ATENCION = -1, DESCR = "SELECCIONE" });
                                ListTipoServicio = ListTipoServicio ?? new ObservableCollection<ATENCION_SERVICIO>();
                                ListTipoServicio.Insert(0, new ATENCION_SERVICIO { ID_TIPO_ATENCION = -1, ID_TIPO_SERVICIO = -1, DESCR = "SELECCIONE" });
                                ListTipoServicioAux = new ObservableCollection<ATENCION_SERVICIO>(ListTipoServicio);
                                SelectTipoAtencion = (!(IsDentista && IsMedico)) ? ListAtencionTipo.First(f => rol.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA ? f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL :
                                    f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA) : ListAtencionTipo.First(f => f.ID_TIPO_ATENCION == -1);
                                SelectTipoServicio = ListTipoServicio.First(f => f.ID_TIPO_ATENCION == -1 && f.ID_TIPO_SERVICIO == -1);
                                SelectedPronostico = -1;
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
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                //HuellaWindow = null;
                //var metro = Application.Current.Windows[0] as MetroWindow;
                //HuellaWindow.Owner = metro;
                var existePropietario = true;
                foreach (var item in PopUpsViewModels.MainWindow.OwnedWindows)
                {
                    if (item.ToString().Equals("ControlPenales.BuscarPorHuellaYNipView"))
                    {
                        existePropietario = false;
                        if (HuellaWindow == null)
                        {
                            HuellaWindow = new BuscarPorHuellaYNipView();
                            HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                        }
                        if (HuellaWindowSalida == null)
                        {
                            HuellaWindowSalida = new BuscarPorHuellaYNipView();
                            HuellaWindowSalida.Owner = PopUpsViewModels.MainWindow;
                        }
                        HuellaWindowSalida.Close();
                        HuellaWindow.Close();
                        //break;
                    }
                }
                HuellaWindow = new BuscarPorHuellaYNipView();
                HuellaWindow.DataContext = this;
                ConstructorHuella(0);
                if (existePropietario)
                    HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                HuellaWindow.Closed += HuellaClosed;
                HuellaWindow.IsCloseButtonEnabled = true;
                BuscarPor = enumTipoPersona.PERSONA_EMPLEADO;
                HuellaWindow.ShowDialog();
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
            if (EspecialistasWindow != null ? !EspecialistasWindow.IsVisible : false)
            {
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
                        (PopUpsViewModels.VisibleFondoOscuro != Visibility.Visible && PopUpsViewModels.VisibleBusquedaImputado != Visibility.Visible && StaticSourcesViewModel.ShowErrorDialog != Visibility.Visible &&
                        (EspecialistasWindow != null ? !EspecialistasWindow.IsVisible : true))
                    : false)
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarImputados(); });
                    }
                    FechaActualizacion = Fechas.GetFechaDateServer.AddMinutes(1);
                    timer.Start();
                }
            }
        }

        private void CargarImputados()
        {
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                ListCitadosPropios = new ObservableCollection<ATENCION_CITA>(new cAtencionCita().GetData().Where(w => w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro ?
                    !ParametroEstatusInactivos.Contains(w.INGRESO.ID_ESTATUS_ADMINISTRATIVO) ?
                        w.CITA_FECHA_HORA.HasValue ?
                            (w.CITA_FECHA_HORA.Value.Year == hoy.Year && w.CITA_FECHA_HORA.Value.Month == hoy.Month && w.CITA_FECHA_HORA.Value.Day == hoy.Day) ?
                                (w.ESTATUS != "C") ?
                                    IsMedico || IsDentista ?
                    //w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                                            w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.ESPECIALIDAD_INTERNA || w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.ESPECIALIDAD_INTERNA_DENTAL ?
                                                idPersona > 0 ?
                                                    w.ESPECIALISTA_CITA.Any() ?
                                                        w.ESPECIALISTA_CITA.Any(a => a.ID_ESPECIALISTA == idPersona ? true : a.ESPECIALISTA.ID_PERSONA == idPersona)
                                                    : false
                                                : false
                                            : false
                    //: false
                                    : false
                                : false
                            : false
                        : false
                    : false
                : false).OrderBy(o => o.ID_RESPONSABLE).ThenBy(o => o.CITA_FECHA_HORA));
                CitasMedicasPropiasHeader = "CITAS (" + ListCitadosPropios.Count + ")";/* (IsDentista ? ("CITAS DENTALES (") : ("CITAS MÉDICAS ("))*/
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
            {
                if (new cUsuarioRol().GetData().Any(x => x.ID_ROL == (short)enumRolesAreasTecnicas.ENFERMERO && (string.IsNullOrEmpty(x.ID_USUARIO) ? x.ID_USUARIO : x.ID_USUARIO.Trim()) == StaticSourcesViewModel.UsuarioLogin.Username))
                    IsSignosVitalesVisible = IsEnfermero = true;

                if (new cUsuarioRol().GetData().Any(x => x.ID_ROL == (short)enumRolesAreasTecnicas.MEDICO && (string.IsNullOrEmpty(x.ID_USUARIO) ? x.ID_USUARIO : x.ID_USUARIO.Trim()) == StaticSourcesViewModel.UsuarioLogin.Username))
                    IsSignosVitalesVisible = IsDiagnosticoEnabled = IsMedico = true;

                if (new cUsuarioRol().GetData().Any(x => x.ID_ROL == (short)enumRolesAreasTecnicas.DENTISTA && (string.IsNullOrEmpty(x.ID_USUARIO) ? x.ID_USUARIO : x.ID_USUARIO.Trim()) == StaticSourcesViewModel.UsuarioLogin.Username))
                    IsDentista = true;

                if (IsEnfermero && IsMedico)
                    IsAmbos = true;
            }

            catch (Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar el rol del usuario.", exc);
            }
        }

        private void Validaciones()
        {
            if (IsAmbos)
            {
                ValidacionesAmbosPerfiles();
                return;
            }
            if (IsEnfermero)
            {
                ValidacionesEnfermero();
                return;
            }
            if (IsMedico)
            {
                ValidacionesMedico();
                return;
            }
            if (IsDentista)
            {
                ValidacionesMedico();
                //ValidacionesDentista();
                return;
            }
        }

        private string ProcesaProgreso()
        {
            int Actual = new int();
            if (IsBusquedaSimpleValidada)
                Actual++;
            if (IsPlanimetriaValidada)
                Actual++;
            return string.Format("PROGRESO {0} de {1}", Actual, MaxValidaciones);
        }

        private async void ModelEnter(Object obj)
        {
            try
            {
                NombreBuscar = TextNombreImputado;
                ApellidoPaternoBuscar = TextPaternoImputado;
                ApellidoMaternoBuscar = TextMaternoImputado;
                FolioBuscar = TextFolioImputado;
                AnioBuscar = TextAnioImputado;
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                NombreBuscar = string.IsNullOrEmpty(TextNombreImputado) ? string.Empty : TextNombreImputado;
                ApellidoPaternoBuscar = string.IsNullOrEmpty(TextPaternoImputado) ? string.Empty : TextPaternoImputado;
                ApellidoMaternoBuscar = string.IsNullOrEmpty(TextMaternoImputado) ? string.Empty : TextMaternoImputado;
                if (AnioBuscar != null && FolioBuscar != null)
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                if (ListExpediente[0].INGRESO.Count == 0)
                                {
                                    SelectExpediente = null;
                                    SelectIngreso = null;
                                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                }
                                if (ParametroEstatusInactivos.Any(a => a.HasValue ? a.Value == ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO : false))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                                    }));
                                    return;
                                }
                                if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO.HasValue ?
                                    ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro : false)
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
                                SelectExpediente = ListExpediente[0];
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                SeleccionarImputado();
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado.", ex);
                            }
                        });
                    }
                    else
                    {
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                }
                else
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    EmptyExpedienteVisible = ListExpediente.Count <= 0;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }

            return;
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "AnioBuscar":
                                AnioBuscar = string.IsNullOrEmpty(textbox.Text) ? new Nullable<int>() : int.Parse(textbox.Text);
                                break;
                            case "FolioBuscar":
                                FolioBuscar = string.IsNullOrEmpty(textbox.Text) ? new Nullable<int>() : int.Parse(textbox.Text);
                                break;
                        }
                    }
                }

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                EmptyExpedienteVisible = ListExpediente.Count <= 0;
                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }

            return;
        }

        private async Task<bool> ProcesaValidezCita()
        {
            var respuesta = false;
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    try
                    {
                        var FechaInicioString = string.Empty;
                        var FechaFinString = string.Empty;
                        var FechaHoy = Fechas.GetFechaDateServer;
                        var Atenciones = new cAtencionCita().ObtenerTodo(GlobalVar.gCentro, ParametroEstatusInactivos, (short)eAreas.AREA_MEDICA)
                            .Where(w => w.ID_CENTRO == SelectedIngreso.ID_CENTRO && w.ID_ANIO == SelectedIngreso.ID_ANIO && w.ID_IMPUTADO == SelectedIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectedIngreso.ID_INGRESO);
                        if (Atenciones.Any(w => !w.CITA_FECHA_HORA.HasValue ? false :
                                (w.CITA_FECHA_HORA.Value.Day == FechaHoy.Day && w.CITA_FECHA_HORA.Value.Month == FechaHoy.Month && w.CITA_FECHA_HORA.Value.Year == FechaHoy.Year)))
                        {
                            respuesta = true;
                            var atencionesHoy = Atenciones.Where(w => !w.CITA_FECHA_HORA.HasValue ? false :
                                (w.CITA_FECHA_HORA.Value.Day == FechaHoy.Day && w.CITA_FECHA_HORA.Value.Month == FechaHoy.Month && w.CITA_FECHA_HORA.Value.Year == FechaHoy.Year)).ToList();
                            if (atencionesHoy.Count > 1)
                            {
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_CONSULTAS_MEDICAS);
                                ListConsultasMedicasAux = new ObservableCollection<ATENCION_CITA>(Atenciones);
                                ListConsultasMedicas = new ObservableCollection<ATENCION_CITA>(ListConsultasMedicasAux.OrderByDescending(o => o.CITA_FECHA_HORA));
                                SelectConsultaMedica = null;
                                EmptyBuscarConsultasMedicasVisible = false;
                                TextBuscarConsultaMedica = string.Empty;
                            }
                            else
                            {
                                SelectConsultaMedica = atencionesHoy.FirstOrDefault();
                                ObtenerSignosVitales();
                            }
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El imputado ingresado no tiene citas programadas el día de hoy hacia el área médica");
                            }));
                            SelectedCita = null;
                            respuesta = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar la cita.", ex);
                    }
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar la cita.", ex);
            }
            return respuesta;
        }

        #region GET_DATOS
        /*private void GetDatosUrgencia()
        {
            if (SelectConsultaMedica.ATENCION_MEDICA == null) return;
            if (SelectConsultaMedica.ATENCION_MEDICA.NOTA_URGENCIA == null) return;

            TextUrgenciaDestino = SelectConsultaMedica.ATENCION_MEDICA.NOTA_URGENCIA.DESTINO_DESPUES_UR;
            TextUrgenciaEstadoMental = SelectConsultaMedica.ATENCION_MEDICA.NOTA_URGENCIA.ESTADO_MENTAL;
            TextUrgenciaMotivo = SelectConsultaMedica.ATENCION_MEDICA.NOTA_URGENCIA.MOTIVO_CONSULTA;
            TextUrgenciaProcedimiento = SelectConsultaMedica.ATENCION_MEDICA.NOTA_URGENCIA.PROCEDIMIENTO_AREA;
        }
        private void GetDatosEvolucion()
        {
            if (SelectConsultaMedica.ATENCION_MEDICA == null) return;
            
            TextEvolucion = SelectConsultaMedica.ATENCION_MEDICA;
            TextExistencia = SelectConsultaMedica.ATENCION_MEDICA;
            TextActualizacion = SelectConsultaMedica.ATENCION_MEDICA;
            
        }
        private void GetDatosInterconsulta()
        {
            if (SelectConsultaMedica.ATENCION_MEDICA == null) return;
            if (SelectConsultaMedica.ATENCION_MEDICA.NOTA_INTERCONSULTA == null) return;

            TextInterconsultaCriterio = SelectConsultaMedica.ATENCION_MEDICA.NOTA_INTERCONSULTA.CRITERIO_DIAGNOSTI;
            TextInterconsultaMotivo = SelectConsultaMedica.ATENCION_MEDICA.NOTA_INTERCONSULTA.MOTIVO_CONSULTA;
            TextInterconsultaSugerencias = SelectConsultaMedica.ATENCION_MEDICA.NOTA_INTERCONSULTA.SUGERENCIAS_DIAGNO;
            TextInterconsultaTratamiento = SelectConsultaMedica.ATENCION_MEDICA.NOTA_INTERCONSULTA.TRATAMIENTO;
        }
        private void GetDatosTraslado()
        {
            if (SelectConsultaMedica.ATENCION_MEDICA == null) return;
            if (SelectConsultaMedica.ATENCION_MEDICA.NOTA_REFERENCIA_TR == null) return;

            TextTrasladoEnvia = SelectConsultaMedica.ATENCION_MEDICA.NOTA_REFERENCIA_TR.ESTABLECIMIENTO_EN;
            TextTrasladoMotivo = SelectConsultaMedica.ATENCION_MEDICA.NOTA_REFERENCIA_TR.MOTIVO_ENVIO;
            TextTrasladoReceptor = SelectConsultaMedica.ATENCION_MEDICA.NOTA_REFERENCIA_TR.ESTABLECIMIENTO_RE;
        }
        private void GetDatosPreOp()
        {
            if (SelectConsultaMedica.ATENCION_MEDICA == null) return;
            if (SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_OPERATORI == null) return;

            TextPreOpCuidados = SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_OPERATORI.CUIDADOS;
            TextPreOpDiagnostico = SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_OPERATORI.DIAGNOSTICO_PREOPE;
            TextPreOpFecha = SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_OPERATORI.FEC_CIRUGIA_REALIZ.HasValue ? SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_OPERATORI.FEC_CIRUGIA_REALIZ.Value : new Nullable<DateTime>();
            TextPreOpPlan = SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_OPERATORI.PLAN_QUIRURGICO;
            TextPreOpPlanTerapeutico = SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_OPERATORI.PLAN_TERAPEUTICO_P;
            TextPreOpRiesgo = SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_OPERATORI.RIESGO_QUIRURGICO;
        }
        private void GetDatosPreAn()
        {
            if (SelectConsultaMedica.ATENCION_MEDICA == null) return;
            if (SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_ANESTECIC == null) return;

            TextPreAnestEvolucion = SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_ANESTECIC.EVALUACION_CLINICA;
            TextPreAnestRiesgo = SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_ANESTECIC.RIESGO_ANESTESICO;
            TextPreAnestTipo = SelectConsultaMedica.ATENCION_MEDICA.NOTA_PRE_ANESTECIC.TIPO_ANESTESIA;
        }
        private void GetDatosPostOp()
        {
            if (SelectConsultaMedica.ATENCION_MEDICA == null) return;
            if (SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR == null) return;

            TextPostOpAccidentes = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.ACCIDENTES;
            TextPostOpDiagnosticoPostOp = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.DIAGNOSTICO_POST_O;
            TextPostOpEstadoInmediato = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.ESTADO_POST_QUIRUR;
            TextPostOpGasasCompresas = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.REPORTE_GASAS_COMP;
            TextPostOpHallazgosTransoperatorios = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.HALLASZGOS_TRANSOP;
            TextPostOpIncidentes = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.INCIDENTES;
            TextPostOpInterpretacion = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.RESULTADOS;
            TextPostOpPiezasBiopsias = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.ENVIO_EXAMEN_MACRO;
            TextPostOpPlaneada = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.OPERACION_PLANEADA;
            TextPostOpRealizada = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.OPERACION_REALIZAD;
            TextPostOpSangrado = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.CUANTIFICACION_SAN;
            TextPostOpTecnicaQuirurgica = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.DESCR_TECNICA_QUIR;
            TextPostOpTratamientoInmediato = SelectConsultaMedica.ATENCION_MEDICA.NOTA_POST_OPERATOR.PLAN_TRATAMIENTO_P;
        }*/
        #endregion

        #region LIMPIAR
        private void LimpiarGeneral()
        {
            LimpiarImputado();
            LimpiarSignosVitales();
            LimpiarDiagnosticoTratamiento();
            LimpiarUrgencia();
            LimpiarEvolucion();
            LimpiarInterconsulta();
            LimpiarTraslado();
            LimpiarPreOP();
            LimpiarPreAn();
            LimpiarPostop();
        }
        private void LimpiarImputado()
        {
            TextFolioImputado = new Nullable<int>();
            TextAnioImputado = new Nullable<int>();
            TextNombre = string.Empty;
            TextNombreImputado = string.Empty;
            TextPaterno = string.Empty;
            TextPaternoImputado = string.Empty;
            TextMaterno = string.Empty;
            TextMaternoImputado = string.Empty;
            Sexo = string.Empty;
            Edad = string.Empty;
            //SelectTipoAtencion = ListAtencionTipo.First(f => f.ID_TIPO_ATENCION == -1);
            FotoIngreso = new Imagenes().getImagenPerson();
        }
        private void LimpiarSignosVitales()
        {
            Peso = string.Empty;
            Talla = string.Empty;
            Glucemia = string.Empty;
            TensionArterial = string.Empty;
            TextFrecuenciaCardiaca = string.Empty;
            FrecuenciaRespira = string.Empty;
            Temperatura = string.Empty;
            ObservacionesSignosVitales = string.Empty;
        }
        private void LimpiarDiagnosticoTratamiento()
        {
            ResumenInterr = string.Empty;
            ExploracionFisica = string.Empty;
            ResultadoServAux = string.Empty;
            ResultadoServTrat = string.Empty;
            Diagnostico = string.Empty;
            PlanEstudioTrat = string.Empty;
            SelectedPronostico = -1;
            ResponsableNotaMedica = string.Empty;
            ListEnfermedades = new ObservableCollection<ENFERMEDAD>();
        }
        private void LimpiarUrgencia()
        {
            TextUrgenciaProcedimiento = string.Empty;
            TextUrgenciaMotivo = string.Empty;
            TextUrgenciaEstadoMental = string.Empty;
            TextUrgenciaDestino = string.Empty;
        }
        private void LimpiarEvolucion()
        {
            TextEvolucion = string.Empty;
            TextExistencia = string.Empty;
            TextActualizacion = string.Empty;
        }
        private void LimpiarInterconsulta()
        {
            TextInterconsultaTratamiento = string.Empty;
            TextInterconsultaSugerencias = string.Empty;
            TextInterconsultaMotivo = string.Empty;
            TextInterconsultaCriterio = string.Empty;
        }
        private void LimpiarTraslado()
        {
            TextTrasladoEnvia = string.Empty;
            TextTrasladoMotivo = string.Empty;
            TextTrasladoReceptor = string.Empty;
        }
        private void LimpiarPreOP()
        {
            TextPreOpCuidados = string.Empty;
            TextPreOpDiagnostico = string.Empty;
            TextPreOpFecha = new Nullable<DateTime>();
            TextPreOpPlan = string.Empty;
            TextPreOpPlanTerapeutico = string.Empty;
            TextPreOpRiesgo = string.Empty;
        }
        private void LimpiarPreAn()
        {
            TextPreAnestEvolucion = string.Empty;
            TextPreAnestRiesgo = string.Empty;
            TextPreAnestTipo = string.Empty;
        }
        private void LimpiarPostop()
        {
            TextPostOpAccidentes = string.Empty;
            TextPostOpDiagnosticoPostOp = string.Empty;
            TextPostOpEstadoInmediato = string.Empty;
            TextPostOpGasasCompresas = string.Empty;
            TextPostOpHallazgosTransoperatorios = string.Empty;
            TextPostOpIncidentes = string.Empty;
            TextPostOpInterpretacion = string.Empty;
            TextPostOpPiezasBiopsias = string.Empty;
            TextPostOpPlaneada = string.Empty;
            TextPostOpRealizada = string.Empty;
            TextPostOpSangrado = string.Empty;
            TextPostOpTecnicaQuirurgica = string.Empty;
            TextPostOpTratamientoInmediato = string.Empty;
        }
        #endregion

        private void HideGroupBox()
        {
            NotaMedicaVisible = Visibility.Collapsed;
            ListaImputadosVisible = Visibility.Collapsed;
            SignosVitalesVisible = Visibility.Collapsed;
            DiagnosticoVisible = Visibility.Collapsed;
            TabMedicoVisibles = Visibility.Collapsed;
            CertificadoMedicoIngresoVisible = Visibility.Collapsed;
        }

        private void HuellaClosed(object sender, EventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    HuellaWindow.Closed -= HuellaClosed;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    if (!IsEspecialista ? MedicoSupervisor == null ? SelectEspecialista == null : false : false)
                    {
                        var metro = Application.Current.Windows[0] as MetroWindow;
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                        GC.Collect();
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
                    }
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la ventana de búsqueda.", ex);
            }
        }

        private void EspecialistasClosed(object sender, EventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    EspecialistasWindow.Closed -= EspecialistasClosed;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la ventana de búsqueda.", ex);
            }
        }

        private void DietaSelecccionada(object SelectedItem)
        {
            try
            {
                if (((object[])(SelectedItem))[0] is DietaMedica)
                {
                    /*var causaPenal = (DietaMedica)(((object[])(SelectedItem))[0]);
                    var checkBox = (CheckBox)(((object[])(SelectedItem))[1]);
                    if ((((object[])(SelectedItem))[2]) is CheckBox)
                    {
                        if (SelectTipoServicio.ID_TIPO_SERVICIO != (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS)
                            causaPenal.ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
                    }
                    else
                    {
                        if (SelectTipoServicio.ID_TIPO_SERVICIO != (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS)
                            causaPenal.ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
                    }*/
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la dieta.", ex);
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
                                    ValidacionSolicitud();
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
                                if (SelectIngreso != null ?
                                    SelectIngreso.ATENCION_CITA.Any(a => a.CITA_FECHA_HORA.Value.Year == AHoraI.Value.Year && a.CITA_FECHA_HORA.Value.Month == AHoraI.Value.Month && a.CITA_FECHA_HORA.Value.Day == AHoraI.Value.Day &&
                                        a.CITA_FECHA_HORA.Value.Hour == AHoraI.Value.Hour && a.CITA_FECHA_HORA.Value.Minute == AHoraI.Value.Minute)
                                : false)
                                {
                                    //var cita = SelectIngreso.ATENCION_CITA.First(a => a.CITA_FECHA_HORA.Value.Year == AHoraI.Value.Year && a.CITA_FECHA_HORA.Value.Month == AHoraI.Value.Month && a.CITA_FECHA_HORA.Value.Day == AHoraI.Value.Day &&
                                    //    a.CITA_FECHA_HORA.Value.Hour == AHoraI.Value.Hour && a.CITA_FECHA_HORA.Value.Minute == AHoraI.Value.Minute);

                                    GuardarAgendaEnabled = ListProcMedsSeleccionados != null ?
                                        ListProcMedsSeleccionados.Any() ?
                                            !(ListProcMedsSeleccionados.Any(a => a.ID_PROC_MED == SelectProcMedSeleccionado.ID_PROC_MED ?
                                                a.CITAS.Any(an => an.FECHA_INICIAL.Year == AHoraI.Value.Year && an.FECHA_INICIAL.Month == AHoraI.Value.Month && an.FECHA_INICIAL.Day == AHoraI.Value.Day &&
                                                    an.FECHA_INICIAL.Hour == AHoraI.Value.Hour && an.FECHA_INICIAL.Minute == AHoraI.Value.Minute)
                                            : false))
                                        : true
                                    : true;
                                    //GuardarAgendaEnabled = cita != null ? !(cita.PROC_ATENCION_MEDICA_PROG.Any(w => SelectProcMedEnCitaParaAgendarAux != null ? w.ID_PROCMED == SelectProcMedEnCitaParaAgendarAux.ID_PROCMED : false)) : true;
                                }
                                else
                                    GuardarAgendaEnabled = ListProcMedsSeleccionados != null ? !ListProcMedsSeleccionados.Where(an => an.ID_PROC_MED == SelectProcMedSeleccionado.ID_PROC_MED).Any(an =>
                                        an.CITAS.Any(a => a.FECHA_INICIAL.Year == AHoraI.Value.Year && a.FECHA_INICIAL.Month == AHoraI.Value.Month && a.FECHA_INICIAL.Day == AHoraI.Value.Day && a.FECHA_INICIAL.Hour == AHoraI.Value.Hour &&
                                        a.FECHA_INICIAL.Minute == AHoraI.Value.Minute)) : true;
                                if (ListProcMedsSeleccionados != null)
                                {
                                    ProcedimientosMedicosEnCitaEnMemoria = new ObservableCollection<CustomCitasProcedimientosMedicos>(ListProcMedsSeleccionados.SelectMany(s => s.CITAS)
                                        .Where(w => w.FECHA_INICIAL.Year == AHoraI.Value.Year && w.FECHA_INICIAL.Month == AHoraI.Value.Month && w.FECHA_INICIAL.Day == AHoraI.Value.Day && w.FECHA_INICIAL.Hour == AHoraI.Value.Hour &&
                                               w.FECHA_INICIAL.Minute == AHoraI.Value.Minute).DistinctBy(d => d.ID_PROC_MED));
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
                        NOMBRE_COMPLETO = s.PERSONA.NOMBRE.Trim() + " " + s.PERSONA.PATERNO.Trim() + " " + (string.IsNullOrEmpty(s.PERSONA.MATERNO) ? string.Empty : s.PERSONA.MATERNO.Trim()),
                        Usuario = s.USUARIO.First(f => f.ID_PERSONA == s.ID_EMPLEADO)
                    }).ToList();
                    var usr = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault();
                    if (!LstEmpleados.Any(a => a.ID_EMPLEADO == usr.ID_PERSONA))
                        LstEmpleados.Add(new cUsuarioExtendida
                        {
                            ID_EMPLEADO = usr.ID_PERSONA.Value,
                            NOMBRE_COMPLETO = usr.EMPLEADO.PERSONA.NOMBRE.Trim() + " " + usr.EMPLEADO.PERSONA.PATERNO.Trim() + " " + (string.IsNullOrEmpty(usr.EMPLEADO.PERSONA.MATERNO) ? string.Empty : usr.EMPLEADO.PERSONA.MATERNO.Trim()),
                            Usuario = usr
                        });
                    LstEmpleados.Insert(0, new cUsuarioExtendida
                    {
                        ID_EMPLEADO = -1,
                        NOMBRE_COMPLETO = "SELECCIONE",
                        Usuario = new USUARIO(),
                    });
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
                    if (((DataGrid)obj).Name.Contains("Tratamiento"))
                    {
                        cacha = "Ocurrió un error al cargar los datos del tratamiento.";
                        #region TRATAMIENTO DENTAL
                        var selected = ((CustomTratamientoDental)((DataGrid)obj).SelectedItem);
                        var parametro = new string[] { };
                        foreach (var item in ListCheckBoxOdontograma)
                            item.IsChecked = false;
                        foreach (var item in selected.SELECCIONADOS)
                        {
                            foreach (var ckb in ListCheckBoxOdontograma)
                            {
                                parametro = ((object[])ckb.CommandParameter)[1].ToString().Split('_');
                                if (item.ID_DIENTE == short.Parse(parametro[0]) && item.ID_POSICION == short.Parse(parametro[1]))
                                    ckb.IsChecked = true;
                            }
                        }
                        TextTratamientoDental = selected.OBSERV;
                        SelectTipoTratamiento = ListTipoTratamiento.First(f => f.ID_TIPO_TRATA == selected.ID_TIPO_TRATAMIENTO);
                        SelectTratamientoOdonto = ListTratamientoOdonto.First(f => f.ID_TRATA == selected.ID_TRATAMIENTO && f.ID_TIPO_TRATA == selected.ID_TIPO_TRATAMIENTO);
                        TextDentalEnable = false;
                        #endregion
                    }
                    else if (((DataGrid)obj).Name.Contains("Historial"))
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
                                SeguimientoCheckedEnabled = SelectEspecialista != null ?
                                    SelectEspecialista.ID_PERSONA.HasValue
                                : false;
                                ListProcedimientosMedicos = new ObservableCollection<PROC_MED>(new cProcedimientosMedicos().ObtenerTodosActivos().Where(w => w.PROC_MED_SUBTIPO.ID_TIPO_ATENCION == SelectCitaMedicaImputado.ID_TIPO_ATENCION).OrderBy(o => o.PROC_MED_SUBTIPO.DESCR).ThenBy(t => t.DESCR));
                                /*
                                HospitalizacionEnable = ingreso != null ?
                                    !ingreso.ATENCION_MEDICA.Any(a => a.HOSPITALIZACION != null ?
                                        a.HOSPITALIZACION.ID_HOSEST == 1
                                    : false)
                                    ||
                                    ingreso.ATENCION_MEDICA.Any(a => a.NOTA_MEDICA != null ?
                                        a.NOTA_MEDICA.HOSPITALIZACION.Any(h => h.ID_HOSEST == 1)
                                    : false)
                                : false;
                                */
                                LstOdontogramaInicial = new ObservableCollection<ODONTOGRAMA_SEGUIMIENTO2>();
                                if (SelectCitaMedicaImputado != null ?
                                    SelectCitaMedicaImputado.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ?
                                        SelectCitaMedicaImputado.ATENCION_MEDICA != null ?
                                            SelectCitaMedicaImputado.ATENCION_MEDICA.ODONTOGRAMA_SEGUIMIENTO2.Any()
                                        : false
                                    : false
                                : false)
                                    LstOdontogramaInicial = new ObservableCollection<ODONTOGRAMA_SEGUIMIENTO2>(SelectCitaMedicaImputado.ATENCION_MEDICA.ODONTOGRAMA_SEGUIMIENTO2);
                                if (IsMedico || IsDentista)
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
                                        ListRecetas = new ObservableCollection<RecetaMedica>(atencionCita.PROC_ATENCION_MEDICA_PROG.Where(w => w.ID_CENTRO_UBI == GlobalVar.gCentro).Select(s => s.PROC_ATENCION_MEDICA).Select(s => s.ATENCION_MEDICA)
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
                                    base.ClearRules();
                                    LimpiarValidaciones();

                                }
                                if (atencionCita.ID_TIPO_SERVICIO == (short)enumAtencionServicio.ESPECIALIDAD_INTERNA ?
                                    atencionCita.ATENCION_MEDICA != null ?
                                        atencionCita.ATENCION_MEDICA.NOTA_MEDICA != null
                                    : false
                                : false)
                                {
                                    EditarCitaEspecialista = SelectCitaMedicaImputado != null ? SelectCitaMedicaImputado.ATENCION_MEDICA != null ? SelectCitaMedicaImputado.ATENCION_MEDICA.NOTA_MEDICA != null : false : false;
                                    MujeresEnabled = !EditarCitaEspecialista;
                                    if (EditarCitaEspecialista)
                                    {
                                        SelectIngreso = atencionCita.INGRESO;
                                        ExploracionFisica = atencionCita.ATENCION_MEDICA.NOTA_MEDICA.EXPLORACION_FISICA;
                                        ListEnfermedades = new ObservableCollection<ENFERMEDAD>(atencionCita.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Select(s => s.ENFERMEDAD));
                                        SelectedPronostico = atencionCita.ATENCION_MEDICA.NOTA_MEDICA.ID_PRONOSTICO;
                                        SolicitaInterconsultaCheck = atencionCita.ATENCION_MEDICA.NOTA_MEDICA.OCUPA_INTERCONSULTA == "S";
                                        CheckedHospitalizacion = atencionCita.ATENCION_MEDICA.NOTA_MEDICA.OCUPA_HOSPITALIZACION == "S";
                                        CheckedSeguimiento = atencionCita.ATENCION_MEDICA.ID_CITA_SEGUIMIENTO.HasValue ? atencionCita.ATENCION_MEDICA.ID_CITA_SEGUIMIENTO.Value > 0 : false;
                                        //StrFechaSeguimiento = atencionCita.ATENCION_MEDICA.ATENCION_CITA1 != null ? atencionCita.ATENCION_MEDICA.ATENCION_CITA1.CITA_FECHA_HORA.ToString() : string.Empty;
                                        StrFechaSeguimiento = atencionCita.ATENCION_MEDICA.ATENCION_CITA1 != null ?
                                            atencionCita.ATENCION_MEDICA.ATENCION_CITA1.CITA_FECHA_HORA.HasValue ?
                                                "Fecha de la próxima cita: " + atencionCita.ATENCION_MEDICA.ATENCION_CITA1.CITA_FECHA_HORA.Value.ToString("dd \\de MMMM, yyyy a la\\s HH:mm")
                                            : string.Empty
                                        : string.Empty;
                                        AtencionCitaSeguimiento = atencionCita.ATENCION_MEDICA.ATENCION_CITA1;
                                        ResumenInterr = atencionCita.ATENCION_MEDICA.NOTA_MEDICA.RESUMEN_INTERROGAT;
                                        CargarPatologicos();
                                        ListRecetas = new ObservableCollection<RecetaMedica>(atencionCita.ATENCION_MEDICA.RECETA_MEDICA.SelectMany(s => s.RECETA_MEDICA_DETALLE).Select(s => new RecetaMedica
                                        {
                                            CANTIDAD = s.DOSIS,
                                            DURACION = s.DURACION,
                                            HORA_MANANA = s.DESAYUNO.HasValue ? s.DESAYUNO.Value == 1 : false,
                                            HORA_NOCHE = s.CENA.HasValue ? s.CENA.Value == 1 : false,
                                            HORA_TARDE = s.COMIDA.HasValue ? s.COMIDA.Value == 1 : false,
                                            MEDIDA = s.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? s.PRODUCTO.ID_UNIDAD_MEDIDA.Value : 0,
                                            OBSERVACIONES = s.OBSERV,
                                            PRESENTACION = s.ID_PRESENTACION_MEDICAMENTO.HasValue ? s.ID_PRESENTACION_MEDICAMENTO.Value : new Nullable<short>(),
                                            PRODUCTO = s.PRODUCTO,
                                        }));
                                        foreach (var item in ListDietas)
                                        {
                                            item.ELEGIDO = atencionCita.ATENCION_MEDICA.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(a => a.ID_DIETA == item.DIETA.ID_DIETA);
                                        }
                                        ListDietas = new ObservableCollection<DietaMedica>(ListDietas);
                                        ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>();
                                        var citas = new List<CustomCitasProcedimientosMedicos>();
                                        var agenda = string.Empty;
                                        var cit = new ATENCION_CITA();
                                        Application.Current.Dispatcher.Invoke((Action)(delegate
                                        {
                                            foreach (var item in atencionCita.ATENCION_MEDICA.PROC_ATENCION_MEDICA)
                                            {
                                                agenda = string.Empty;
                                                citas = new List<CustomCitasProcedimientosMedicos>();
                                                foreach (var itm in item.PROC_ATENCION_MEDICA_PROG)
                                                {
                                                    agenda = agenda + (itm.ATENCION_CITA.CITA_FECHA_HORA.HasValue ? itm.ATENCION_CITA.CITA_FECHA_HORA.Value.ToString("dd \\de MMMM, yyyy a la\\s HH:mm") : string.Empty) + "\n";
                                                    citas.Add(new CustomCitasProcedimientosMedicos
                                                    {
                                                        ID_PROC_MED = itm.ID_PROCMED,
                                                        PROC_MED_DESCR = item.PROC_MED.DESCR,
                                                        FECHA_INICIAL = itm.ATENCION_CITA.CITA_FECHA_HORA.HasValue ? itm.ATENCION_CITA.CITA_FECHA_HORA.Value : new DateTime(),
                                                        FECHA_FINAL = itm.ATENCION_CITA.CITA_HORA_TERMINA.HasValue ? itm.ATENCION_CITA.CITA_HORA_TERMINA.Value : new DateTime(),
                                                        ENFERMERO = itm.ATENCION_CITA.ID_RESPONSABLE.HasValue ? itm.ATENCION_CITA.ID_RESPONSABLE.Value : 0,
                                                    });
                                                }
                                                ListProcMedsSeleccionados.Add(new CustomProcedimientosMedicosSeleccionados
                                                {
                                                    AGENDA = agenda,
                                                    PROC_MED_DESCR = item.PROC_MED.DESCR,
                                                    OBSERV = item.OBSERV,
                                                    ID_PROC_MED = item.ID_PROCMED,
                                                    CITAS = citas,
                                                });
                                            }
                                        }));
                                        if (atencionCita.INGRESO.IMPUTADO.SEXO == "F") { /* NO EXISTEN VARIABLES PARA LA SECCION FEMENINA */ }
                                        if (atencionCita.ATENCION_MEDICA.CERTIFICADO_MEDICO != null)
                                        {
                                            CheckedToxicomanias = !string.IsNullOrEmpty(atencionCita.ATENCION_MEDICA.CERTIFICADO_MEDICO.TOXICOMANIAS);
                                            TextToxicomanias = atencionCita.ATENCION_MEDICA.CERTIFICADO_MEDICO.TOXICOMANIAS;
                                            CheckedPeligroVida = atencionCita.ATENCION_MEDICA.CERTIFICADO_MEDICO.PELIGRA_VIDA == "S";
                                            Checked15DiasSanar = atencionCita.ATENCION_MEDICA.CERTIFICADO_MEDICO.TARDA_SANAR_15 == "S";
                                        }
                                    }
                                    SignosVitalesReadOnly = false;
                                }

                                DiagnosticoVisible = Visibility.Collapsed;
                                base.ClearRules();
                                LimpiarValidaciones();
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
                        HuellaWindow.Closed += HuellaClosed;
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
                                        if (CartaView != null)
                                            CartaView.Hide();
                                        var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Exito!", "Hace falta el NIP para empatar al imputado.");
                                        if (CartaView != null)
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
                                            if (CartaView != null)
                                                CartaView.Hide();
                                            var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Exito!", "El NIP no concuerda con el del imputado seleccionado.");
                                            if (CartaView != null)
                                                CartaView.Show();
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                        }));
                                        return;
                                    }
                                }
                            }
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                if (CartaView != null)
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
                            var ListProcAtMedProg = new List<PROC_ATENCION_MEDICA_PROG>();
                            if (ListProcMedsSeleccionados != null)
                            {
                                citas = null;
                                foreach (var item in ListProcMedsSeleccionados.SelectMany(s => s.CITAS).OrderBy(o => o.FECHA_INICIAL))
                                {
                                    var citaTraslape = new cAtencionCita().ObtieneCitaOtraArea(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, item.FECHA_INICIAL, item.FECHA_FINAL, 
                                        AtencionCita != null ? AtencionCita.ID_CITA : 0);
                                    if (citaTraslape != null ?
                                        EditarCitaEspecialista ?
                                            false
                                        : citaTraslape.ID_CITA != SelectCitaMedicaImputado.ID_CITA ?
                                            citaTraslape.ID_CENTRO_UBI == SelectCitaMedicaImputado.ID_CENTRO_UBI
                                        : false
                                    : false)
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(delegate
                                        {
                                            new Dialogos().ConfirmacionDialogo("Algo pasó...", "Existe un traslape en la fecha [ " + item.FECHA_INICIAL.ToString("dd/MM/yyyy HH:mm") + " ].");
                                        }));
                                        return;
                                    }
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
                                        ID_CENTRO_UBI = GlobalVar.gCentro,
                                    };
                                    var resp = new cUsuario().Obtener(item.ENFERMERO);
                                    if (citas != null ?
                                        citas.Any(a => a.CITA_FECHA_HORA == cit.CITA_FECHA_HORA ?
                                            !a.PROC_ATENCION_MEDICA_PROG.Any(an => an.ID_PROCMED == item.ID_PROC_MED && an.ID_USUARIO_ASIGNADO.Trim() == (resp.Any() ? resp.FirstOrDefault().ID_USUARIO.Trim() : string.Empty))
                                        : true)
                                    : true)
                                    {
                                        ProcAtMedProg = new PROC_ATENCION_MEDICA_PROG
                                        {
                                            ESTATUS = "N",
                                            ID_PROCMED = item.ID_PROC_MED,
                                            ID_USUARIO_ASIGNADO = resp.Any() ? 
                                                resp.FirstOrDefault().ID_USUARIO 
                                            : string.Empty,
                                            ID_CITA = idCita,
                                            ID_CENTRO_UBI = GlobalVar.gCentro,
                                            ID_CENTRO_CITA = GlobalVar.gCentro,
                                            PROC_ATENCION_MEDICA = new PROC_ATENCION_MEDICA
                                            {
                                                ID_PROCMED = item.ID_PROC_MED,
                                                ID_USUARIO = GlobalVar.gUsr,
                                                OBSERV = ListProcMedsSeleccionados.Any(f => f.ID_PROC_MED == item.ID_PROC_MED) ? 
                                                    ListProcMedsSeleccionados.First(f => f.ID_PROC_MED == item.ID_PROC_MED).OBSERV 
                                                : string.Empty,
                                                REGISTRO_FEC = hoy,
                                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                                ID_ATENCION_MEDICA = EditarCitaEspecialista ? 
                                                    SelectCitaMedicaImputado.ID_ATENCION_MEDICA.HasValue ? 
                                                        SelectCitaMedicaImputado.ID_ATENCION_MEDICA.Value 
                                                    : 0 
                                                : 0,
                                            },
                                        };
                                        cit.PROC_ATENCION_MEDICA_PROG.Add(ProcAtMedProg);
                                        ListProcAtMedProg.Add(ProcAtMedProg);
                                    }
                                    if (citas == null) citas = new List<ATENCION_CITA>();
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
                                    ABORTO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.ABORTO == TextAbortos ? 
                                            null 
                                        : TextAbortos 
                                    : TextAbortos,
                                    ABORTO_MODIFICADO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.ABORTO == TextAbortos ? 
                                            null 
                                        : "S" 
                                    : "S",
                                    ANIOS_RITMO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.ANIOS_RITMO == TextAniosRitmo ? 
                                            null 
                                        : TextAniosRitmo 
                                    : TextAniosRitmo,
                                    ANIOS_RITMO_MODIFICADO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.ANIOS_RITMO == TextAniosRitmo ? 
                                            null 
                                        : "S" 
                                    : "S",
                                    CESAREA = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.CESAREA == TextCesareas ? 
                                            null 
                                        : TextCesareas 
                                    : TextCesareas,
                                    CESAREA_MODIFICADO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.CESAREA == TextCesareas ? 
                                            null 
                                        : "S" 
                                    : "S",
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
                                        : SelectControlPreNatal == -1 ? 
                                            null 
                                        : SelectControlPreNatal
                                    : SelectControlPreNatal == -1 ? 
                                        null 
                                    : SelectControlPreNatal,
                                    ID_CONTROL_PRENATAL_MODIFICADO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.ID_CONTROL_PRENATAL == SelectControlPreNatal ? 
                                            null 
                                        : "S" 
                                    : "S",
                                    DEFORMACION = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.DEFORMACION == TextDeformacionesOrganicas ? 
                                            null 
                                        : TextDeformacionesOrganicas 
                                    : TextDeformacionesOrganicas,
                                    DEFORMACION_MODIFICADO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.DEFORMACION == TextDeformacionesOrganicas ? 
                                            null 
                                        : "S" 
                                    : "S",
                                    EMBARAZO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.EMBARAZO == TextEmbarazos ? 
                                            new Nullable<short>() 
                                        : TextEmbarazos 
                                    : TextEmbarazos,
                                    EMBARAZO_MODIFICADO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.EMBARAZO == TextEmbarazos ? 
                                            null 
                                        : "S" 
                                    : "S",
                                    FECHA_PROBABLE_PARTO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.FECHA_PROBABLE_PARTO == FechaProbParto ? 
                                            new Nullable<DateTime>() 
                                        : FechaProbParto 
                                    : FechaProbParto,
                                    FECHA_PROBABLE_PARTO_MOD = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.FECHA_PROBABLE_PARTO == FechaProbParto ? 
                                            null 
                                        : "S" 
                                    : "S",
                                    PARTO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.PARTO == TextPartos ? 
                                            new Nullable<short>() 
                                        : TextPartos 
                                    : TextPartos,
                                    PARTO_MODIFICADO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.PARTO == TextPartos ? 
                                            null 
                                        : "S" 
                                    : "S",
                                    ULTIMA_MENSTRUACION_FEC = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.ULTIMA_MENSTRUACION_FEC == FechaUltimaMenstruacion ? 
                                            new Nullable<DateTime>() 
                                        : FechaUltimaMenstruacion 
                                    : FechaUltimaMenstruacion,
                                    ULTIMA_MENS_MODIFICADO = MujeresAuxiliar != null ? 
                                        MujeresAuxiliar.ULTIMA_MENSTRUACION_FEC == FechaUltimaMenstruacion ? 
                                            null 
                                        : "S" 
                                    : "S",
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

                            if (EditarCitaEspecialista ? RecetaMedica != null ? RecetaMedica.ID_FOLIO <= 0 : false : false)
                            {
                                var query = new cRecetaMedica().ObtenerXAtencionMedica(AtencionMedica.ID_ATENCION_MEDICA, AtencionMedica.ID_CENTRO_UBI);
                                RecetaMedica.ID_FOLIO = query.Any() ? query.FirstOrDefault().ID_FOLIO : 0;
                            }
                            if (EditarCitaEspecialista ?
                                    new cNotaMedica().ActualizarNotaMedicaEspecialista(Fechas.GetFechaDateServer, SignosVitales, NotaMedica, AtencionMedica, AtencionCita, Enfermedades, lesiones, dietas, patologicos, AtencionCitaSeguimiento,
                                    RecetaMedica, RecetaMedicaDetalle, citas, SelectEspecialista, GlobalVar.gUsr, (short)enumMensajeTipo.DETECCION_AREA_MEDICA_GRUPO_VULNERABLE, Mujeres, LstOdontogramaInicial.Any() ? LstOdontogramaInicial.ToList() : null)
                                : false)
                            {
                                MenuGuardarEnabled = false;
                                Application.Current.Dispatcher.Invoke((Action)(async delegate
                                {
                                    if (CartaView != null)
                                    {
                                        CartaView.Close();
                                        CartaView = null;
                                    }
                                    HuellaCompromiso = false;
                                    GuardadoListo = true;
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
                                    StaticSourcesViewModel.SourceChanged = false;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                }));
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            }
                            else
                            {
                                if (SignosVitales != null ?
                                    new cNotaMedica().InsertarNotaMedicaTransaccion(Fechas.GetFechaDateServer, SignosVitales, NotaMedica, AtencionMedica, AtencionCita, NotaUrgencia, NotaInterconsulta, NotaPostOperatoria,
                                        NotaPreAnestecica, NotaPreOperatoria, NotaTraslado, NotaEvolucion, Enfermedades, certificado, lesiones, dietas, Excarcelacion, TrasladoDetalle, patologicos, Mujeres, AtencionCitaSeguimiento,
                                        RecetaMedica, RecetaMedicaDetalle, citas, SelectEspecialista, GlobalVar.gUsr, null, (short)enumMensajeTipo.DETECCION_AREA_MEDICA_GRUPO_VULNERABLE, LstOdontogramaInicial.Any() ? LstOdontogramaInicial.ToList() : null) :
                                    new cNotaMedica().ActualizarNotaMedicaTransaccion(Fechas.GetFechaDateServer, NotaMedica, AtencionMedica, AtencionCita, NotaUrgencia, NotaInterconsulta, NotaPostOperatoria,
                                       NotaPreAnestecica, NotaPreOperatoria, NotaTraslado, NotaEvolucion, Enfermedades, certificado, lesiones, dietas, Excarcelacion, TrasladoDetalle, patologicos, Mujeres, AtencionCitaSeguimiento,
                                       RecetaMedica, RecetaMedicaDetalle, citas, SelectEspecialista, GlobalVar.gUsr, null, (short)enumMensajeTipo.DETECCION_AREA_MEDICA_GRUPO_VULNERABLE, LstOdontogramaInicial.Any() ? LstOdontogramaInicial.ToList() : null))
                                {
                                    MenuGuardarEnabled = false;
                                    Application.Current.Dispatcher.Invoke((Action)(async delegate
                                    {
                                        if (CartaView != null)
                                        {
                                            CartaView.Close();
                                            CartaView = null;
                                        }
                                        HuellaCompromiso = false;
                                        GuardadoListo = true;
                                        var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Exito!", "La nota ha sido guardada correctamente.");
                                        if (certificado == null)
                                            clickSwitch("limpiar_menu");
                                        else
                                        {
                                            MenuHClinicaEnabled = true;
                                            BotonRegresarVisible = Visibility.Visible;
                                            SignosVitalesReadOnly = true;
                                        }
                                        StaticSourcesViewModel.SourceChanged = false;
                                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                    }));
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                if (CartaView != null)
                                {
                                    CartaView.Close();
                                    CartaView = null;
                                }
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
                    HuellaWindow.Closed += HuellaClosed;
                    HuellaWindow.ShowDialog();
                    break;
                case "aceptar_huella":
                    try
                    {
                        if (HuellaWindow != null ? HuellaWindow.IsVisible : false)
                            HuellaWindow.Hide();
                        if (HuellaWindowSalida != null ? HuellaWindowSalida.IsVisible : false)
                            HuellaWindowSalida.Hide();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        if (ScannerMessage.Contains("Procesando..."))
                            return;
                        CancelKeepSearching = true;
                        isKeepSearching = true;
                        _IsSucceed = true;
                        var _error_tipo = 0;
                        if (SelectRegistro == null)
                        {
                            _error_tipo = 1;
                            return;
                        }
                        if (!IsEspecialista)
                        {
                            if (!AceptaEntrada && !AceptaSalida)
                            {
                                MedicoSupervisor = SelectRegistro.Persona;
                                IsEspecialista = true;
                                AceptaEntrada = true;
                                BuscarPor = enumTipoPersona.IMPUTADO;
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                                {
                                    ListaEspecialistas = new ObservableCollection<ESPECIALISTA>();
                                    var hoy = Fechas.GetFechaDateServer;
                                    var personas = new cEspecialistas().GetData(g => g.ESPECIALISTA_CITA.Any(a => a.ATENCION_CITA != null ?
                                        a.ATENCION_CITA.CITA_FECHA_HORA.HasValue ?
                                            (a.ATENCION_CITA.CITA_FECHA_HORA.Value.Day == hoy.Day && a.ATENCION_CITA.CITA_FECHA_HORA.Value.Month == hoy.Month && a.ATENCION_CITA.CITA_FECHA_HORA.Value.Year == hoy.Year)
                                        : false
                                    : false));
                                    if (personas.Any())
                                        ListaEspecialistas = new ObservableCollection<ESPECIALISTA>(personas.OrderBy(o => o.ESPECIALISTA_PATERNO).ThenBy(t => t.ESPECIALISTA_MATERNO).ThenBy(t => t.ESPECIALISTA_NOMBRE).ToList().Select(s => new ESPECIALISTA
                                        {
                                            CENTRO = s.CENTRO,
                                            ESPECIALIDAD = s.ESPECIALIDAD,
                                            ESPECIALISTA_CITA = s.ESPECIALISTA_CITA,
                                            ESPECIALISTA_MATERNO = s.PERSONA != null ? s.PERSONA.MATERNO : s.ESPECIALISTA_MATERNO,
                                            ESPECIALISTA_PATERNO = s.PERSONA != null ? s.PERSONA.PATERNO : s.ESPECIALISTA_PATERNO,
                                            ESPECIALISTA_NOMBRE = s.PERSONA != null ? s.PERSONA.NOMBRE : s.ESPECIALISTA_NOMBRE,
                                            ESTATUS = s.ESTATUS,
                                            ID_CENTRO_UBI = s.ID_CENTRO_UBI,
                                            ID_ESPECIALIDAD = s.ID_ESPECIALIDAD,
                                            ID_ESPECIALISTA = s.ID_ESPECIALISTA,
                                            ID_PERSONA = s.ID_PERSONA,
                                            ID_USUARIO = s.ID_USUARIO,
                                            PERSONA = s.PERSONA,
                                            REGISTRO_FEC = s.REGISTRO_FEC,
                                            USUARIO = s.USUARIO,
                                        }));
                                    EmptyEspecialistas = personas.Any() ? Visibility.Collapsed : Visibility.Visible;
                                    FotoRegistro = new Imagenes().getImagenPerson();
                                });
                                Application.Current.Dispatcher.Invoke((Action)(delegate { }));
                                HuellaWindow.Close();
                                EspecialistasWindow = new ListaEspecialistasView();
                                EspecialistasWindow.DataContext = this;
                                EspecialistasWindow.Owner = PopUpsViewModels.MainWindow;
                                EspecialistasWindow.Closed += EspecialistasClosed;
                                EspecialistasWindow.IsCloseButtonEnabled = false;
                                EspecialistasWindow.IsMinButtonEnabled = false;
                                EspecialistasWindow.IsMaxRestoreButtonEnabled = false;
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                EspecialistasWindow.ShowDialog();
                                return;
                            }
                            else if (AceptaEntrada && !AceptaSalida)
                            {
                                AceptaSalida = true;
                                BuscarPor = enumTipoPersona.PERSONA_EMPLEADO;
                                StaticSourcesViewModel.SalidaEspecialista = true;
                                return;
                            }
                            else if (AceptaSalida)
                            {
                                if (MedicoSupervisor.ID_PERSONA != SelectRegistro.Persona.ID_PERSONA)
                                {
                                    await new Dialogos().ConfirmacionDialogoReturn("Algo pasó...", "El médico no concuerda.");
                                    StaticSourcesViewModel.ShowLoading = Visibility.Collapsed;
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                    HuellaWindowSalida.Show();
                                    return;
                                }
                                StaticSourcesViewModel.SalidaEspecialista = true;
                                return;
                            }
                        }
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
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
                                var existeHosp = new cHospitalizacion().ObtenerHospitalizacionesPorIngreso(SelectIngreso);
                                HospitalizacionEnable = !existeHosp.Any();
                                SeleccionarImputado();
                                HideGroupBox();
                                ObtenerSignosVitales();
                                CargarPatologicos();
                                if (IsEnfermero)
                                {
                                    HideGroupBox();
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
                                if (SelectCitaMedicaImputado != null ? SelectCitaMedicaImputado.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA : false)
                                {
                                    NotaMedicaVisible = Visibility.Visible;
                                    SignosVitalesVisible = Visibility.Visible;
                                    DiagnosticoVisible = Visibility.Visible;
                                    TratamientoVisible = Visibility.Visible;
                                    TipoNotaEnabled = true;
                                    AtencionTipoEnabled = true;
                                }
                                else if (SelectCitaMedicaImputado != null ? SelectCitaMedicaImputado.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL : false)
                                {
                                    NotaMedicaVisible = Visibility.Visible;
                                    SignosVitalesVisible = Visibility.Visible;
                                    TratamientoVisible = Visibility.Visible;
                                    TipoNotaEnabled = true;
                                    AtencionTipoEnabled = true;
                                    TratamientoDentalVisible = Visibility.Visible;
                                    ProcedimientosMedicosVisible = Visibility.Visible;
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
                                    CambiarMedicoVisible = Visibility.Collapsed;
                                    ProcedimientoTratamientoDentalVisible = ProcedimientoMedicoSeleccionadoVisible = SelectCitaMedicaImputado.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ?
                                        SelectCitaMedicaImputado.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_DENTALES ?
                                            Visibility.Visible
                                        : Visibility.Collapsed
                                    : Visibility.Collapsed;
                                    MujeresVisible = SelectCitaMedicaImputado.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ? Visibility.Visible : Visibility.Collapsed;
                                    HistoriaClinicaEnabled = SelectCitaMedicaImputado.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA;
                                    DiagnosticoVisible = SelectCitaMedicaImputado.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible : Visibility.Visible;

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
                                if (!EditarCitaEspecialista)
                                {//INSERTAR EN INGRESO UBICACION
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
                                }
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    HuellaWindow.Close();
                                    //HuellaWindow = null;
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
                        {/*
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
                            : Visibility.Collapsed;*/
                            ElementosDisponibles = true;
                        }
                        CheckCambiarMedico = false;
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
                        //HuellaWindow.Show();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    break;
                case "cerrar_buscar_huella_2":
                case "cerrar_buscar_huella":
                    if ((HuellaWindow != null ? HuellaWindow.IsLoaded : false) && IsEspecialista)
                    {
                        HuellaWindow.Close();
                        //HuellaWindow = null;
                    }
                    break;
                case "buscar_nip":
                    try
                    {
                        ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                        if (string.IsNullOrEmpty(TextNipBusqueda)) return;
                        if (HuellaWindow != null ? HuellaWindow.IsVisible : false)
                            HuellaWindow.Hide();
                        if (HuellaWindowSalida != null ? HuellaWindowSalida.IsVisible : false)
                            HuellaWindowSalida.Hide();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                var auxiliar = new List<ResultadoBusquedaBiometrico>();
                                ListResultado = new List<ResultadoBusquedaBiometrico>();
                                var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                                if (!IsEspecialista)
                                {
                                    var personaNip = new cPersona().ObtenerPersona(long.Parse(TextNipBusqueda));
                                    if (personaNip != null)
                                    {
                                        if (personaNip.EMPLEADO != null ? !personaNip.EMPLEADO.USUARIO.Any(a => a.ID_USUARIO.Trim() == GlobalVar.gUsr) : true)
                                        {
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                try
                                                {
                                                    ScannerMessage = "CLAVE NO CONCUERDA";
                                                    ColorMessage = new SolidColorBrush(Colors.Red);
                                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                                    if (AceptaEntrada && AceptaSalida)
                                                        HuellaWindowSalida.Show();
                                                    else
                                                        HuellaWindow.Show();
                                                }
                                                catch (Exception ex)
                                                {
                                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar realizar la búsqueda por nip.", ex);
                                                }
                                            }));
                                            return;
                                        }
                                        if (personaNip != null ? personaNip.PERSONA_BIOMETRICO.Any() : false)
                                            if (personaNip.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(personaNip.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                            else
                                                if (personaNip.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                    FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(personaNip.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);

                                        if (personaNip.EMPLEADO != null ?
                                            personaNip.EMPLEADO.USUARIO != null ?
                                                personaNip.EMPLEADO.USUARIO.Any(a => a.ID_USUARIO.Trim() == GlobalVar.gUsr ?
                                                    a.USUARIO_ROL.Any(an => an.ID_ROL == (short)enumRolesAreasTecnicas.MEDICO || an.ID_ROL == (short)enumRolesCoordinadoresAreasTecnicas.COORDINADOR_MEDICO)
                                                : false)
                                            : false
                                        : false)
                                        {
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                ScannerMessage = "CLAVE ENCONTRADA";
                                                ColorMessage = new SolidColorBrush(Colors.Green);
                                            }));
                                            auxiliar.Add(new ResultadoBusquedaBiometrico
                                               {
                                                   AMaterno = string.IsNullOrEmpty(personaNip.MATERNO) ? string.Empty : personaNip.MATERNO.Trim(),
                                                   APaterno = personaNip.PATERNO.Trim(),
                                                   Expediente = personaNip.ID_PERSONA.ToString(),
                                                   Foto = FotoBusquedaHuella,
                                                   Imputado = null,
                                                   NIP = personaNip.PERSONA_NIP.Any(a => a.ID_CENTRO == GlobalVar.gCentro && a.NIP.HasValue) ?
                                                       personaNip.PERSONA_NIP.First(f => f.ID_CENTRO == GlobalVar.gCentro && f.NIP.HasValue).NIP.Value.ToString()
                                                   : string.Empty,
                                                   Nombre = personaNip.NOMBRE.Trim(),
                                                   Persona = personaNip
                                               });
                                        }
                                        else
                                        {
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                ScannerMessage = "CLAVE NO CONCUERDA";
                                                ColorMessage = new SolidColorBrush(Colors.Red);
                                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                                if (AceptaEntrada && AceptaSalida)
                                                    HuellaWindowSalida.Show();
                                                else
                                                    HuellaWindow.Show();
                                            }));
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(delegate
                                        {
                                            ScannerMessage = "CLAVE NO CONCUERDA";
                                            ColorMessage = new SolidColorBrush(Colors.Red);
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                            if (AceptaEntrada && AceptaSalida)
                                                HuellaWindowSalida.Show();
                                            else
                                                HuellaWindow.Show();
                                        }));
                                        return;
                                    }
                                }
                                else
                                {
                                    if (SelectExpediente == null)
                                        SelectExpediente = SelectIngresoConsulta != null ? SelectIngresoConsulta.IMPUTADO : null;
                                    if (string.IsNullOrEmpty(SelectExpediente.NIP) ? false : SelectExpediente.NIP.Trim() == TextNipBusqueda)
                                    {
                                        var ingresobiometrico = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                        if (ingresobiometrico != null ? ingresobiometrico.INGRESO_BIOMETRICO.Any() : false)
                                            if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                            else
                                                if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                    FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                        auxiliar.Add(new ResultadoBusquedaBiometrico
                                        {
                                            AMaterno = string.IsNullOrEmpty(SelectExpediente.MATERNO) ? string.Empty : SelectExpediente.MATERNO.Trim(),
                                            APaterno = SelectExpediente.PATERNO.Trim(),
                                            Expediente = SelectExpediente.ID_ANIO + "/" + SelectExpediente.ID_IMPUTADO,
                                            Foto = FotoBusquedaHuella,
                                            Imputado = SelectExpediente,
                                            NIP = !string.IsNullOrEmpty(SelectExpediente.NIP) ? SelectExpediente.NIP : string.Empty,
                                            Nombre = SelectExpediente.NOMBRE.Trim(),
                                            Persona = null
                                        });
                                    }
                                }
                                ListResultado = auxiliar.Any() ? auxiliar.ToList() : new List<ResultadoBusquedaBiometrico>();
                                if (!AceptaSalida)
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        if (HuellaWindow != null ? !HuellaWindow.IsVisible : false)
                                        {
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                            HuellaWindow.Show();
                                        }
                                    }));
                                else
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        if (HuellaWindowSalida != null ? !HuellaWindowSalida.IsVisible : false)
                                        {
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                            HuellaWindowSalida.Show();
                                        }
                                    }));
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar realizar la búsqueda por nip.", ex);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar realizar la búsqueda por nip.", ex);
                    }
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
                        HuellaWindow.Closed += HuellaClosed;
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
                case "agregar_odonto_inicial":
                    if (SelectIngreso != null)
                        GuardaDatosDientes();
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }
                    break;
                case "limpiar_odonto_inicial":
                    LimpiarDientesIniciales();
                    break;
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

                    DescargaArchivo(SelectedResultadoSinArchivo);
                    break;
                case "cambio_tipo_serv_aux":
                    try
                    {
                        CargarSubTipo_Servicios_Auxiliares(SelectedTipoServAux);
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
                        CargarDiagnosticosPrincipal(SelectedSubtipoServAux);
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
                        ArchivosMedicos.Closed += ArchivosMedicosClosed;
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
                            BuscarResultadosExistentes();
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
                            AgendaView.Loaded += AgendaLoaded;
                            AgendaView.Agenda.AddAppointment += AgendaAddAppointment;
                            AgendaView.btn_guardar.Click += AgendaClick;
                            AgendaView.Closing += AgendaClosing;
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
                            CancelarProcMedWindow.Closed += CancelarProcMedClosed;
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
                    if (SelectProcedimientoMedicoHijo == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento médico para poder eliminarlo.");
                        return;
                    }
                    if (SelectProcedimientoMedicoHijo.ID_PROCMED == -1)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento médico para poder eliminarlo.");
                        return;
                    }
                    if (ListProcMedsSeleccionados != null ? !ListProcMedsSeleccionados.Any() : true)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No hay ningun procedimiento que quitar.");
                        return;
                    }
                    if (ListProcMedsSeleccionados.Any(a => a.ID_PROC_MED == SelectProcedimientoMedicoHijo.ID_PROCMED))
                    {
                        ListProcMedsSeleccionados.Remove(ListProcMedsSeleccionados.First(f => f.ID_PROC_MED == SelectProcedimientoMedicoHijo.ID_PROCMED));
                        SelectProcedimientoSubtipo = ListProcedimientoSubtipo.First(f => f.ID_PROCMED_SUBTIPO == -1);
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El procedimiento seleccionada no forma parte de la lista.");
                        return;
                    }
                    break;
                #endregion

                #region OTROS
                case "cancelar_especialista":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    EspecialistasWindow.Close();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new BandejaEntradaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
                    break;
                case "seleccionar_especialista":
                    if (SelectEspecialista == null)
                    {
                        EspecialistasWindow.Hide();
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes seleccionar un especialista.");
                        EspecialistasWindow.Show();
                        return;
                    }
                    idPersona = SelectEspecialista.ID_PERSONA.HasValue ? SelectEspecialista.ID_PERSONA.Value : SelectEspecialista.ID_ESPECIALISTA;
                    CargarImputados();
                    EspecialistasWindow.Close();
                    BuscarPor = enumTipoPersona.IMPUTADO;
                    SelectRegistro = null;
                    ListResultado = null;
                    break;
                case "regresar_listado":
                    /*((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotaMedicaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.NotaMedicaEspecialistaViewModel();
                    if (ListaImputadosVisible == Visibility.Visible)
                    {*/
                    ListaImputadosVisible = Visibility.Visible;
                    NotaMedicaVisible = Visibility.Collapsed;
                    CargarImputados();
                    /*}
                    else
                    {
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotaMedicaView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.NotaMedicaEspecialistaViewModel();
                    }*/
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
                        EF_PESO = Peso,
                        EF_ESTATURA = Talla,
                        EF_RESPIRACION = FrecuenciaRespira,
                        EF_PULSO = TextFrecuenciaCardiaca,
                        EF_TEMPERATURA = Temperatura,
                        EF_PRESION_ARTERIAL = TensionArterial1 + "/" + TensionArterial2,
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
                                        Titulo = SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_INTEGRIDAD_FISICA ? 
                                            "CERTIFICADO MÉDICO DE INTEGRIDAD FÍSICA"
                                        : SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO ? 
                                            "CERTIFICADO MÉDICO DE NUEVO INGRESO" 
                                        : "CERTIFICADO MÉDICO",
                                        Centro = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DESCR) ? string.Empty:SelectIngreso.CENTRO1.DESCR.Trim() : "",
                                        Ciudad = SelectIngreso.CENTRO1 != null ? 
                                            SelectIngreso.CENTRO1.MUNICIPIO != null ? 
                                                (string.IsNullOrEmpty(SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1) ? 
                                                    string.Empty 
                                                : SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1.Trim()) 
                                            : string.Empty 
                                        : string.Empty,
                                        Director = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DIRECTOR) ? string.Empty : SelectIngreso.CENTRO1.DIRECTOR.Trim() : "",
                                        Cedula = medico.CEDULA,
                                        Edad = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value : new DateTime()).ToString(),
                                        FechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                                        FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                                        Fecha = hora + dia,
                                        Escolaridad = SelectIngreso.ESCOLARIDAD != null ? SelectIngreso.ESCOLARIDAD.DESCR : "",
                                        Originario = SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? 
                                            (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1) ? 
                                                string.Empty 
                                            : SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1.Trim()) + ", " +
                                            (SelectIngreso.IMPUTADO.MUNICIPIO != null ? 
                                                SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD != null ? 
                                                    (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR) ? string.Empty : SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR.Trim())
                                                : string.Empty
                                            : string.Empty)
                                        : SelectIngreso.IMPUTADO.NACIMIENTO_LUGAR,
                                        NombreImputado = SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + 
                                            (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim()),
                                        TipoDelito = string.IsNullOrEmpty(delitos) ? "N/A" : delitos,
                                        NombreMedico = medico.PERSONA.NOMBRE.Trim() + " " + medico.PERSONA.PATERNO.Trim() + " " + (string.IsNullOrEmpty(medico.PERSONA.MATERNO) ? string.Empty : medico.PERSONA.MATERNO.Trim()),
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
                        ListRecetas.Remove(ListRecetas.First(a => a.PRODUCTO.ID_PRODUCTO == SelectReceta.PRODUCTO.ID_PRODUCTO));
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
                        //if (SelectedEmpleadoValue.Usuario.ID_USUARIO == null)
                        //{
                        ObtenerAgenda(SelectedEmpleadoValue.Usuario.ID_USUARIO, true);
                        //}
                        //else
                        //{
                        //ObtenerAgenda(SelectedEmpleadoValue.Usuario.ID_USUARIO, true);
                        //}
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
                        var usr = new USUARIO();
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            var useer = new cUsuario().ObtenerTodos(GlobalVar.gUsr);
                            if (useer.Any() ? !useer.FirstOrDefault().ID_PERSONA.HasValue : false) return;
                            usr = useer.FirstOrDefault();
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
                                NOMBRE_COMPLETO = s.PERSONA.NOMBRE.Trim() + " " + s.PERSONA.PATERNO.Trim() + " " + (string.IsNullOrEmpty(s.PERSONA.MATERNO) ? string.Empty : s.PERSONA.MATERNO.Trim()),
                                Usuario = s.USUARIO.First(f => f.ID_PERSONA == s.ID_EMPLEADO),
                            }).ToList();
                        });
                        if (!LstEmpleados.Any(a => a.ID_EMPLEADO == usr.ID_PERSONA))
                            LstEmpleados.Add(new cUsuarioExtendida
                            {
                                ID_EMPLEADO = usr.ID_PERSONA.Value,
                                NOMBRE_COMPLETO = usr.EMPLEADO.PERSONA.NOMBRE.Trim() + " " + usr.EMPLEADO.PERSONA.PATERNO.Trim() + " " + (string.IsNullOrEmpty(usr.EMPLEADO.PERSONA.MATERNO) ? string.Empty : usr.EMPLEADO.PERSONA.MATERNO.Trim()),
                                Usuario = usr,
                            });
                        LstEmpleados.Add(new cUsuarioExtendida
                        {
                            ID_EMPLEADO = SelectEspecialista.ID_PERSONA.HasValue ? SelectEspecialista.ID_PERSONA.Value : MedicoSupervisor.ID_PERSONA,
                            NOMBRE_COMPLETO = SelectEspecialista.ESPECIALISTA_NOMBRE.Trim() + " " + SelectEspecialista.ESPECIALISTA_PATERNO.Trim() + " " +
                            (string.IsNullOrEmpty(SelectEspecialista.ESPECIALISTA_MATERNO) ? string.Empty : SelectEspecialista.ESPECIALISTA_MATERNO.Trim()),
                            Usuario = new USUARIO(),
                        });
                        SelectedEmpleadoValue = LstEmpleados.First(f => f.ID_EMPLEADO == (SelectEspecialista.ID_PERSONA.HasValue ? SelectEspecialista.ID_PERSONA.Value : MedicoSupervisor.ID_PERSONA));
                        EmpleadosEnAgendaEnabled = false;
                        ProcedimientoMedicoParaAgenda = null;
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
                case "borrar_enfermedad":
                    if (ListEnfermedades == null) return;
                    if (SelectEnfermedad == null) return;
                    ListEnfermedades.Remove(ListEnfermedades.First(f => f.ID_ENFERMEDAD == SelectEnfermedad.ID_ENFERMEDAD));
                    break;
                case "salir_menu":
                    StaticSourcesViewModel.SalidaEspecialista = false;
                    StaticSourcesViewModel.EspecialistaCambiosCancelados = false;
                    SalirClickMenu = false;
                    Imputado = null;
                    FotoRegistro = new Imagenes().getImagenPerson();
                    PrincipalViewModel.SalirMenu();
                    await Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            if (SalirClickMenu)
                            {
                                return;
                            }
                            else
                            {
                                if (StaticSourcesViewModel.SalidaEspecialista)
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        foreach (var item in PopUpsViewModels.MainWindow.OwnedWindows)
                                        {
                                            if (item.ToString().Equals("ControlPenales.BuscarPorHuellaYNipView"))
                                            {
                                                if (HuellaWindowSalida != null)
                                                    HuellaWindowSalida.Close();
                                                if (HuellaWindow != null)
                                                    HuellaWindow.Close();
                                            }
                                        }
                                        HuellaWindowSalida = null;
                                        HuellaWindow = null;
                                        var metro = Application.Current.Windows[0] as MetroWindow;
                                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                                        GC.Collect();
                                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                                        ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
                                    }));
                                    return;
                                }
                                if (StaticSourcesViewModel.EspecialistaCambiosCancelados)
                                {
                                    return;
                                }
                                TaskEx.Delay(1500);
                            }
                        }
                    });
                    break;
                case "limpiar_menu":
                    /*((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotaMedicaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.NotaMedicaEspecialistaViewModel();*/
                    if (SelectEspecialista != null || GuardadoListo)
                    {
                        EditarCitaEspecialista = false;
                        SolicitaInterconsultaCheck = CheckedHospitalizacion = CheckedPeligroVida = Checked15DiasSanar = false;
                        CheckedSeguimiento = false;
                        ListProcMedsSeleccionados = new ObservableCollection<CustomProcedimientosMedicosSeleccionados>();
                        TensionArterial1 = TensionArterial2 = TensionArterial = string.Empty;
                        SelectedPronostico = -1;
                        ListDietas = new ObservableCollection<DietaMedica>(ListDietas.Select(s => new DietaMedica
                        {
                            DIETA = s.DIETA,
                            ELEGIDO = false,
                        }));
                        LstOdontogramaInicial = new ObservableCollection<ODONTOGRAMA_SEGUIMIENTO2>();
                        ListRecetas = new ObservableCollection<RecetaMedica>();
                        ListEnfermedades = new ObservableCollection<ENFERMEDAD>();
                        SelectProcedimientoSubtipo = ListProcedimientoSubtipo.First(f => f.ID_PROCMED_SUBTIPO == -1);
                        ListaImputadosVisible = Visibility.Visible;
                        NotaMedicaVisible = Visibility.Collapsed;
                        CargarImputados();
                    }
                    else
                    {
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotaMedicaView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.NotaMedicaEspecialistaViewModel();
                    }
                    break;
                case "guardar_menu":
                    ValidarGuardar();
                    break;
                #endregion

                default:
                    break;
            }

            return;
        }

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
                            ID_TIPO_ATENCION = (short)enumAtencionTipo.CONSULTA_MEDICA,
                            ID_TIPO_SERVICIO = (short)enumAtencionServicio.ESPECIALIDAD_INTERNA,
                            ID_CITA = EditarCitaEspecialista ?
                                SelectCitaMedicaImputado != null ?
                                    SelectCitaMedicaImputado.ATENCION_MEDICA != null ?
                                        SelectCitaMedicaImputado.ATENCION_MEDICA.ID_CITA_SEGUIMIENTO.HasValue ?
                                            SelectCitaMedicaImputado.ATENCION_MEDICA.ID_CITA_SEGUIMIENTO.Value
                                        : 0
                                    : 0
                                : 0
                            : 0,
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
                            if (ListProcMedsSeleccionados.Any(a => a.CITAS.Any(an => an.FECHA_INICIAL.Year == AHoraI.Value.Year && an.FECHA_INICIAL.Month == AHoraI.Value.Month && an.FECHA_INICIAL.Day == AHoraI.Value.Day &&
                                an.FECHA_INICIAL.Hour == AHoraI.Value.Hour && an.FECHA_INICIAL.Minute == AHoraI.Value.Minute && an.ENFERMERO == SelectedEmpleadoValue.ID_EMPLEADO)))
                            {
                                AgendaView.Hide();
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", "Ya existe un procedimiento a esta hora.");
                                AgendaView.Show();
                                AgregarHora = false;
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                return;
                            }
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
                    if (string.IsNullOrEmpty(usuario))
                    {
                        var listaa = new cAtencionCita().GetData().Where(w => !w.ID_RESPONSABLE.HasValue ?
                            w.ID_CENTRO_UBI == GlobalVar.gCentro ?
                                w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro ?
                                    !ParametroEstatusInactivos.Contains(w.INGRESO.ID_ESTATUS_ADMINISTRATIVO) ?
                                        w.CITA_FECHA_HORA.HasValue ?
                                            (w.CITA_FECHA_HORA.Value.Year == hoy.Year && w.CITA_FECHA_HORA.Value.Month == hoy.Month && w.CITA_FECHA_HORA.Value.Day == hoy.Day) ||
                                            (w.CITA_FECHA_HORA.Value <= hoy) ?
                                                (w.ESTATUS != "S" && w.ESTATUS != "C") ?
                                                    IsMedico ?
                                                        w.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                                                            w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.ESPECIALIDAD_INTERNA ?
                                                               idPersona > 0 ?
                                                                    w.ESPECIALISTA_CITA.Any() ?
                                                                        w.ESPECIALISTA_CITA.Any(a => a.ID_ESPECIALISTA == idPersona ? true : a.ESPECIALISTA.ID_PERSONA == idPersona)
                                                                    : false
                                                                : false
                                                            : false
                                                        : false
                                                    : false
                                                : false
                                            : false
                                        : false
                                    : false
                                : false
                            : false
                        : false).OrderBy(o => o.ID_RESPONSABLE).ThenBy(o => o.CITA_FECHA_HORA);
                        agenda = listaa != null ? listaa.Any() ? listaa.ToList() : new List<ATENCION_CITA>() : new List<ATENCION_CITA>();
                    }
                    else
                    {
                        var listaa = new cAtencionCita().ObtenerPorUsuarioDesdeFecha(GlobalVar.gCentro, usuario, hoy, ParametroEstatusInactivos);
                        agenda = listaa != null ? listaa.Any() ? listaa.ToList() : new List<ATENCION_CITA>() : new List<ATENCION_CITA>();
                    }
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
                            Subject = string.Format("{0} {1} {2}", SelectIngreso.IMPUTADO.NOMBRE.Trim(), SelectIngreso.IMPUTADO.PATERNO.Trim(), string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? "" : SelectIngreso.IMPUTADO.MATERNO.Trim()),
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

        private void GuardaDatosDientes()
        {
            try
            {
                if (ListOdontograma == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione las áreas a considerar");
                    return;
                }

                if (ListOdontograma.Count == 0)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione al menos un área a considerar");
                    return;
                }

                if (!SelectedAplicaEnfermedadDental && !SelectedAplicaTratamientoDental)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione la enfermedad o el tratamiento a considerar");
                    return;
                }


                if (SelectedAplicaEnfermedadDental)
                    if (IdSelectedEnfermedadDental.HasValue ? IdSelectedEnfermedadDental == -1 : true)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione la enfermedad a considerar");
                        return;
                    }

                if (SelectedAplicaTratamientoDental)
                    if (IdSelectedTratamientoDental.HasValue ? IdSelectedTratamientoDental == -1 : true)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación!", "Seleccione el tratamiento a considerar");
                        return;
                    }

                if (LstOdontogramaInicial == null)
                    LstOdontogramaInicial = new ObservableCollection<ODONTOGRAMA_SEGUIMIENTO2>();

                foreach (var item in ListOdontograma)
                {
                    string _detalleTrat = string.Empty;
                    var _detallePosicion = new cOdontograma().GetData(x => x.ID_POSICION == item.ID_DIENTE).FirstOrDefault();
                    var _detalleDiente = new cOdontogramaTipo().GetData(x => x.ID_TIPO_ODO == item.ID_POSICION).FirstOrDefault();
                    if (IdTratamientoSeguimiento.HasValue)
                    {
                        var _tratam = new cTratamientoDental().GetData(x => x.ID_TRATA == IdTratamientoSeguimiento).FirstOrDefault();
                        if (_tratam != null)
                            _detalleTrat = _tratam != null ? !string.IsNullOrEmpty(_tratam.DESCR) ? _tratam.DESCR.Trim() : string.Empty : string.Empty;
                    };

                    DateTime _FechaActual = Fechas.GetFechaDateServer;
                    LstOdontogramaInicial.Add(new ODONTOGRAMA_SEGUIMIENTO2
                    {
                        ID_ENFERMEDAD = IdSelectedEnfermedadDental,
                        REGISTRO_FEC = _FechaActual,
                        ID_TIPO_ODO = item.ID_POSICION,
                        ID_POSICION = item.ID_DIENTE,
                        ID_TRATA = IdSelectedTratamientoDental,
                        ESTATUS = "S",
                        ENFERMEDAD = IdSelectedEnfermedadDental != -1 ? LstEnfermedadesDentales.First(f => f.ID_ENFERMEDAD == IdSelectedEnfermedadDental) : null,
                        DENTAL_TRATAMIENTO = IdSelectedTratamientoDental != -1 ? ListTratamientosDentales.First(f => f.ID_TRATA == IdSelectedTratamientoDental) : null,
                    });
                };

                ListOdontograma = new List<CustomOdontograma>();
                SelectedAplicaEnfermedadDental = SelectedAplicaTratamientoDental = false;
                IdSelectedTratamientoDental = -1;
                IdSelectedEnfermedadDental = -1;
                ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)_WindowOdontogramaInicial.FindName("GridOdontograma"))).ToList();
                if (ListCheckBoxOdontograma.Any())
                    foreach (var item in ListCheckBoxOdontograma)
                        item.IsChecked = false;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos a la lista.", ex);
            }
        }

        private void LimpiarDientesIniciales()
        {
            try
            {
                ListOdontograma = new List<CustomOdontograma>();
                SelectedAplicaEnfermedadDental = SelectedAplicaTratamientoDental = false;
                IdSelectedTratamientoDental = -1;
                IdSelectedEnfermedadDental = -1;
                ListCheckBoxOdontograma = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<CheckBox>(((Grid)_WindowOdontogramaInicial.FindName("GridOdontograma"))).ToList();
                if (ListCheckBoxOdontograma.Any())
                    foreach (var item in ListCheckBoxOdontograma)
                        item.IsChecked = false;
            }

            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar los campos.", ex);
            }
        }

        private async void ValidarGuardar()
        {
            try
            {
                var cacha = string.Empty;
                var ret = false;
                var hoy = new DateTime();
                var user = new USUARIO();
                Validaciones();
                if (base.HasErrors)
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios. \n\n" + base.Error);
                    }));
                    return;
                }
                if (SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS)
                {
                    #region PROCEDIMIENTO MEDICO AGENDADO
                    if (ListProcedimientosMedicosEnCita != null ? !ListProcedimientosMedicosEnCita.Any() : false)
                    {
                        if (await new Dialogos().ConfirmarEliminar("Validacion", "Está seguro que desea guardar la consulta sin algun procedimiento médico?") != 1)
                        {
                            return;
                        }
                        else { }
                    }
                    if (ListProcedimientosMedicosEnCita.Any(a => a.PROCEDIMIENTOS_MATERIALES != null ? a.PROCEDIMIENTOS_MATERIALES.Count() > 0 ? a.PROCEDIMIENTOS_MATERIALES.Any(an => !an.CANTIDAD.HasValue) : false : true))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Hace falta ingresar el insumo utilizado de un procedimiento.");
                        return;
                    }
                    var proc_at_med_prog = SelectCitaMedicaImputado.PROC_ATENCION_MEDICA_PROG.Any(w => w.ID_CENTRO_CITA == GlobalVar.gCentro && !string.IsNullOrEmpty(w.ID_USUARIO_ASIGNADO) && w.ID_CITA == SelectCitaMedicaImputado.ID_CITA) ?
                        SelectCitaMedicaImputado.PROC_ATENCION_MEDICA_PROG.Where(w => w.ID_CENTRO_CITA == GlobalVar.gCentro && !string.IsNullOrEmpty(w.ID_USUARIO_ASIGNADO) && w.ID_CITA == SelectCitaMedicaImputado.ID_CITA).First()
                    : null;
                    if (proc_at_med_prog == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ocurrió un error con la consulta seleccionada.");
                        return;
                    }
                    #endregion
                }
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    try
                    {
                        #region VARIABLES
                        hoy = Fechas.GetFechaDateServer;
                        user = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault();
                        RecetaMedica = new RECETA_MEDICA();
                        RecetaMedicaDetalle = new List<RECETA_MEDICA_DETALLE>();
                        SignosVitales = new NOTA_SIGNOS_VITALES();
                        NotaMedica = new NOTA_MEDICA();
                        AtencionCita = new ATENCION_CITA();
                        Excarcelacion = new EXCARCELACION();
                        TrasladoDetalle = new TRASLADO_DETALLE();
                        NuevoIngreso = new INGRESO();
                        NotaUrgencia = new NOTA_URGENCIA();
                        NotaInterconsulta = new NOTA_INTERCONSULTA();
                        NotaTraslado = new NOTA_REFERENCIA_TR();
                        NotaPreOperatoria = new NOTA_PRE_OPERATORI();
                        NotaPreAnestecica = new NOTA_PRE_ANESTECIC();
                        NotaPostOperatoria = new NOTA_POST_OPERATOR();
                        NotaEvolucion = new NOTA_EVOLUCION();
                        Enfermedades = new List<NOTA_MEDICA_ENFERMEDAD>();
                        certificado = new CERTIFICADO_MEDICO();
                        lesiones = new List<LESION>();
                        dietas = new List<DIETA>();
                        patologicos = new List<HISTORIA_CLINICA_PATOLOGICOS>();
                        AtencionMedica = new ATENCION_MEDICA();
                        RecetaMedica = null;
                        RecetaMedicaDetalle = null;
                        NotaUrgencia = null;
                        NotaInterconsulta = null;
                        NotaTraslado = null;
                        NotaPreOperatoria = null;
                        NotaPreAnestecica = null;
                        NotaPostOperatoria = null;
                        NotaEvolucion = null;
                        certificado = null;
                        lesiones = null;
                        dietas = null;
                        patologicos = null;
                        AtencionCita = null;
                        Excarcelacion = null;
                        TrasladoDetalle = null;
                        NuevoIngreso = null;
                        #endregion

                        #region VALIDACIONES
                        if (SelectIngreso == null)
                        {
                            cacha = "Es necesario realizar las validaciones al imputado.";
                            ret = true;
                            return;
                        }
                        if (SelectTipoAtencion != null ? SelectTipoAtencion.ID_TIPO_ATENCION <= 0 : false)
                        {
                            cacha = "Es necesario seleccionar el tipo de nota que se realizará.";
                            ret = true;
                            return;
                        }
                        if (SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO <= 0 : false)
                        {
                            cacha = "Es necesario seleccionar el tipo de servicio que se realizará.";
                            ret = true;
                            return;
                        }
                        if (!ElementosDisponibles)
                        {
                            cacha = "No tiene permitido el modificar o agregar información a esta consulta.";
                            ret = true;
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
                            cacha = "Favor de capturar los campos obligatorios de la receta médica.";
                            ret = true;
                            return;
                        }
                        if (SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS)
                        {
                            if (ListProcedimientosMedicosEnCita != null ?
                                ListProcedimientosMedicosEnCita.Any() ?
                                    ListProcedimientosMedicosEnCita.Any(a => a.PROCEDIMIENTOS_MATERIALES.Any(an => an.CANTIDAD.HasValue ? an.CANTIDAD.Value <= 0 : false))
                                : false
                            : false)
                            {
                                cacha = "Favor de capturar el campo CANTIDAD en la lista de procedimientos materiales utilizados.";
                                ret = true;
                                return;
                            }
                        }
                        #endregion

                        AtencionMedica = new ATENCION_MEDICA()
                        {
                            ATENCION_FEC = hoy,
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_CENTRO_UBI = GlobalVar.gCentro,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_TIPO_ATENCION = SelectTipoAtencion.ID_TIPO_ATENCION,
                            ID_TIPO_SERVICIO = SelectTipoServicio.ID_TIPO_SERVICIO,
                            ID_ATENCION_MEDICA = SelectAtencionMedica != null ? SelectAtencionMedica.ID_ATENCION_MEDICA : 0,
                        };

                        #region VALIDAR HOSPITALIZACION
                        if (SelectCitaMedicaImputado.ESPECIALISTA_CITA.Any() ?
                            SelectCitaMedicaImputado.ESPECIALISTA_CITA.Any(a => a.SOL_INTERCONSULTA_INTERNA != null ?
                                a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD != null ?
                                    a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION != null ?
                                        a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA != null ?
                                            a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                                a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION != null ?
                                                    a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION.ID_HOSEST == 1
                                                : false
                                            : false
                                        : false
                                    : false
                                : false
                            : false)
                        : false)
                            AtencionMedica.ID_HOSPITA = (int)SelectCitaMedicaImputado.ESPECIALISTA_CITA.First(a => a.SOL_INTERCONSULTA_INTERNA != null ?
                                a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD != null ?
                                    a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION != null ?
                                        a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA != null ?
                                            a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                                a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION != null ?
                                                    a.SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION.ID_HOSEST == 1
                                                : false
                                            : false
                                        : false
                                    : false
                                : false
                            : false).SOL_INTERCONSULTA_INTERNA.INTERCONSULTA_SOLICITUD.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION.ID_HOSPITA;

                        if (SelectCitaMedicaImputado.ESPECIALISTA_CITA.Any(a => a.ATENCION_CITA != null ?
                            a.ATENCION_CITA.ATENCION_MEDICA1 != null ?
                                a.ATENCION_CITA.ATENCION_MEDICA1.Any(an => an.HOSPITALIZACION != null)
                            : false
                        : false))
                            AtencionMedica.ID_HOSPITA = SelectCitaMedicaImputado.ESPECIALISTA_CITA.First(a => a.ATENCION_CITA != null ?
                                a.ATENCION_CITA.ATENCION_MEDICA != null ?
                                    a.ATENCION_CITA.ATENCION_MEDICA1.Any(an => an.HOSPITALIZACION != null)
                                : false
                            : false).ATENCION_CITA.ATENCION_MEDICA1.First(f => f.HOSPITALIZACION != null).HOSPITALIZACION.ID_HOSPITA;

                        if (SelectCitaMedicaImputado.ATENCION_MEDICA1.Any() ?
                            SelectCitaMedicaImputado.ATENCION_MEDICA1.Any(a => a.ID_TIPO_SERVICIO == (short)enumAtencionServicio.ESPECIALIDAD_INTERNA ?
                                a.NOTA_MEDICA != null ?
                                    a.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        a.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION != null ?
                                            a.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION.ID_HOSEST == 1
                                        : false
                                    : false
                                : false
                            : false)
                        : false)
                            AtencionMedica.ID_HOSPITA = SelectCitaMedicaImputado.ATENCION_MEDICA1.First(a => a.ID_TIPO_SERVICIO == (short)enumAtencionServicio.ESPECIALIDAD_INTERNA ?
                                a.NOTA_MEDICA != null ?
                                    a.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        a.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION != null ?
                                            a.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION.ID_HOSEST == 1
                                        : false
                                    : false
                                : false
                            : false).NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION.ID_HOSPITA;

                        if (SelectCitaMedicaImputado.ATENCION_MEDICA != null ?
                            SelectCitaMedicaImputado.ATENCION_MEDICA.ID_TIPO_SERVICIO == (short)enumAtencionServicio.NOTA_EVOLUCION ?
                                SelectCitaMedicaImputado.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                    SelectCitaMedicaImputado.ATENCION_MEDICA.NOTA_MEDICA.ATENCION_MEDICA != null ?
                                        SelectCitaMedicaImputado.ATENCION_MEDICA.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION != null ?
                                            SelectCitaMedicaImputado.ATENCION_MEDICA.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION.ID_HOSEST == 1
                                        : false
                                    : false
                                : false
                            : false
                        : false)
                            AtencionMedica.ID_HOSPITA = SelectCitaMedicaImputado.ATENCION_MEDICA.NOTA_MEDICA.ATENCION_MEDICA.HOSPITALIZACION.ID_HOSPITA;
                        #endregion

                        if (IsMedico)
                        {
                            #region medico
                            NotaMedica.DIAGNOSTICO = Diagnostico;
                            NotaMedica.EXPLORACION_FISICA = ExploracionFisica;
                            NotaMedica.ID_ATENCION_MEDICA = SelectAtencionMedica != null ?
                                SelectExcarcelacionImputado != null ?
                                    SelectExcarcelacionImputado.CERT_MEDICO_RETORNO.HasValue ?
                                        SelectAtencionMedica.ID_ATENCION_MEDICA
                                    : SelectExcarcelacionImputado.CERT_MEDICO_SALIDA.HasValue ?
                                        SelectExcarcelacionImputado.ATENCION_MEDICA.NOTA_MEDICA == null ?
                                            SelectAtencionMedica.ID_ATENCION_MEDICA
                                        : 0
                                    : 0
                                : 0
                            : 0;

                            NotaMedica.ID_RESPONSABLE = SelectEspecialista.ID_PERSONA == null ?
                                MedicoSupervisor.ID_PERSONA
                            : new cEspecialistas().GetData(g => g.ID_ESPECIALISTA == SelectEspecialista.ID_PERSONA).Any(a => a.ID_PERSONA.HasValue) ?
                                new cEspecialistas().GetData(g => g.ID_ESPECIALISTA == SelectEspecialista.ID_PERSONA).First(f => f.ID_PERSONA.HasValue).ID_PERSONA
                            : MedicoSupervisor.ID_PERSONA;
                            NotaMedica.PLAN_ESTUDIO_TRATA = PlanEstudioTrat;
                            NotaMedica.ID_PRONOSTICO = SelectedPronostico;
                            NotaMedica.RESULTADO_SERV_AUX = ResultadoServAux;
                            NotaMedica.RESULTADO_SERV_TRA = ResultadoServTrat;
                            NotaMedica.RESUMEN_INTERROGAT = ResumenInterr;
                            NotaMedica.FIRMO_CONFORMIDAD = "S";
                            NotaMedica.OCUPA_INTERCONSULTA = SolicitaInterconsultaCheck ? "S" : "N";
                            NotaMedica.OCUPA_HOSPITALIZACION = CheckedHospitalizacion ? "S" : "N";
                            NotaMedica.ID_CENTRO_UBI = GlobalVar.gCentro;

                            #region CITA
                            if (SelectCitaMedicaImputado != null)
                            {
                                AtencionCita = new ATENCION_CITA()
                                {
                                    CITA_FECHA_HORA = SelectCitaMedicaImputado.CITA_FECHA_HORA,
                                    CITA_HORA_TERMINA = SelectCitaMedicaImputado.CITA_HORA_TERMINA,
                                    ESTATUS = ParametroEstatusCitaConcluido,
                                    ID_ANIO = SelectCitaMedicaImputado.ID_ANIO,
                                    ID_AREA = SelectCitaMedicaImputado.ID_AREA,
                                    ID_ATENCION = SelectCitaMedicaImputado.ID_ATENCION,
                                    ID_CENTRO = SelectCitaMedicaImputado.ID_CENTRO,
                                    ID_CITA = SelectCitaMedicaImputado.ID_CITA,
                                    ID_CENTRO_UBI = SelectCitaMedicaImputado.ID_CENTRO_UBI,
                                    ID_IMPUTADO = SelectCitaMedicaImputado.ID_IMPUTADO,
                                    ID_INGRESO = SelectCitaMedicaImputado.ID_INGRESO,
                                    ID_RESPONSABLE = SelectEspecialista.ID_PERSONA == null ? MedicoSupervisor.ID_PERSONA : SelectEspecialista.ID_PERSONA,
                                    ID_TIPO_ATENCION = SelectCitaMedicaImputado.ID_TIPO_ATENCION,
                                    ID_TIPO_SERVICIO = SelectCitaMedicaImputado.ID_TIPO_SERVICIO,
                                    ID_USUARIO = GlobalVar.gUsr,
                                };
                                AtencionMedica.ID_ATENCION_MEDICA = SelectCitaMedicaImputado.ID_ATENCION_MEDICA.HasValue ? SelectCitaMedicaImputado.ID_ATENCION_MEDICA.Value : 0;
                                if (SelectCitaMedicaImputado.ATENCION_MEDICA != null ? SelectCitaMedicaImputado.ATENCION_MEDICA.NOTA_SIGNOS_VITALES == null : false)
                                {
                                    SignosVitales.FRECUENCIA_CARDIAC = TextFrecuenciaCardiaca;
                                    SignosVitales.FRECUENCIA_RESPIRA = FrecuenciaRespira;
                                    SignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                    SignosVitales.ID_RESPONSABLE = SelectEspecialista.ID_PERSONA == null ? MedicoSupervisor.ID_PERSONA : SelectEspecialista.ID_PERSONA;
                                    SignosVitales.OBSERVACIONES = ObservacionesSignosVitales;
                                    SignosVitales.PESO = Peso;
                                    SignosVitales.TALLA = Talla;
                                    SignosVitales.GLUCEMIA = Glucemia;
                                    SignosVitales.TEMPERATURA = Temperatura;
                                    SignosVitales.TENSION_ARTERIAL = TensionArterial = TensionArterial;
                                }
                                else
                                    SignosVitales = null;
                            }
                            #endregion

                            #region EXCARCELACION
                            if (SelectExcarcelacionImputado != null)
                            {
                                Excarcelacion = new EXCARCELACION
                                {
                                    CANCELADO_TIPO = SelectExcarcelacionImputado.CANCELADO_TIPO,
                                    CERT_MEDICO_RETORNO = SelectExcarcelacionImputado.CERT_MEDICO_SALIDA.HasValue && !SelectExcarcelacionImputado.CERT_MEDICO_RETORNO.HasValue ? AtencionMedica.ID_ATENCION_MEDICA : SelectExcarcelacionImputado.CERT_MEDICO_RETORNO,
                                    CERT_MEDICO_SALIDA = SelectExcarcelacionImputado.CERT_MEDICO_SALIDA.HasValue ? SelectExcarcelacionImputado.CERT_MEDICO_SALIDA.Value : AtencionMedica.ID_ATENCION_MEDICA,
                                    CERTIFICADO_MEDICO = SelectExcarcelacionImputado.CERTIFICADO_MEDICO,
                                    ID_ANIO = SelectExcarcelacionImputado.ID_ANIO,
                                    ID_CENTRO = SelectExcarcelacionImputado.ID_CENTRO,
                                    ID_CONSEC = SelectExcarcelacionImputado.ID_CONSEC,
                                    ID_ESTATUS = SelectExcarcelacionImputado.ID_ESTATUS,
                                    ID_IMPUTADO = SelectExcarcelacionImputado.ID_IMPUTADO,
                                    ID_INCIDENCIA_TRASLADO = SelectExcarcelacionImputado.ID_INCIDENCIA_TRASLADO,
                                    ID_INGRESO = SelectExcarcelacionImputado.ID_INGRESO,
                                    ID_TIPO_EX = SelectExcarcelacionImputado.ID_TIPO_EX,
                                    ID_USUARIO = SelectExcarcelacionImputado.ID_USUARIO,
                                    INCIDENCIA_OBSERVACION = SelectExcarcelacionImputado.INCIDENCIA_OBSERVACION,
                                    OBSERVACION = SelectExcarcelacionImputado.OBSERVACION,
                                    PROGRAMADO_FEC = SelectExcarcelacionImputado.PROGRAMADO_FEC,
                                    REGISTRO_FEC = SelectExcarcelacionImputado.REGISTRO_FEC,
                                    RETORNO_FEC = SelectExcarcelacionImputado.RETORNO_FEC,
                                    SALIDA_FEC = SelectExcarcelacionImputado.SALIDA_FEC,
                                    TRASLADO_INCIDENCIA_TIPO = SelectExcarcelacionImputado.TRASLADO_INCIDENCIA_TIPO,
                                    ID_CENTRO_UBI = SelectExcarcelacionImputado.ID_CENTRO_UBI,
                                    ID_INCIDENCIA_TRASLADO_RETORNO = SelectExcarcelacionImputado.ID_INCIDENCIA_TRASLADO_RETORNO,
                                };
                                if (SelectExcarcelacionImputado.ATENCION_MEDICA != null ? SelectExcarcelacionImputado.ATENCION_MEDICA.NOTA_SIGNOS_VITALES == null : false)
                                {
                                    SignosVitales.FRECUENCIA_CARDIAC = TextFrecuenciaCardiaca;
                                    SignosVitales.FRECUENCIA_RESPIRA = FrecuenciaRespira;
                                    SignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                    SignosVitales.ID_RESPONSABLE = SelectEspecialista.ID_PERSONA == null ? MedicoSupervisor.ID_PERSONA : SelectEspecialista.ID_PERSONA;
                                    SignosVitales.OBSERVACIONES = ObservacionesSignosVitales;
                                    SignosVitales.PESO = Peso;
                                    SignosVitales.TALLA = Talla;
                                    SignosVitales.TEMPERATURA = Temperatura;
                                    SignosVitales.TENSION_ARTERIAL = TensionArterial;
                                    SignosVitales.ID_CENTRO_UBI = GlobalVar.gCentro;
                                }
                                if (SelectExcarcelacionImputado.ATENCION_MEDICA1 != null ? SelectExcarcelacionImputado.ATENCION_MEDICA1.NOTA_SIGNOS_VITALES == null : false)
                                {
                                    SignosVitales.FRECUENCIA_CARDIAC = TextFrecuenciaCardiaca;
                                    SignosVitales.FRECUENCIA_RESPIRA = FrecuenciaRespira;
                                    SignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                    SignosVitales.ID_RESPONSABLE = SelectEspecialista.ID_PERSONA == null ? MedicoSupervisor.ID_PERSONA : SelectEspecialista.ID_PERSONA;
                                    SignosVitales.OBSERVACIONES = ObservacionesSignosVitales;
                                    SignosVitales.PESO = Peso;
                                    SignosVitales.TALLA = Talla;
                                    SignosVitales.GLUCEMIA = Glucemia;
                                    SignosVitales.TEMPERATURA = Temperatura;
                                    SignosVitales.TENSION_ARTERIAL = TensionArterial;
                                    SignosVitales.ID_CENTRO_UBI = GlobalVar.gCentro;
                                }
                                else
                                    SignosVitales = null;
                            }
                            #endregion

                            #region TRASLADO
                            if (SelectTrasladoImputado != null)
                                TrasladoDetalle = new TRASLADO_DETALLE
                                {
                                    AMP_ID_AMPARO_INDIRECTO = SelectTrasladoImputado.AMP_ID_AMPARO_INDIRECTO,
                                    AMP_ID_ANIO = SelectTrasladoImputado.AMP_ID_ANIO,
                                    AMP_ID_CENTRO = SelectTrasladoImputado.AMP_ID_CENTRO,
                                    AMP_ID_IMPUTADO = SelectTrasladoImputado.AMP_ID_IMPUTADO,
                                    AMP_ID_INGRESO = SelectTrasladoImputado.AMP_ID_INGRESO,
                                    CANCELADO_OBSERVA = SelectTrasladoImputado.CANCELADO_OBSERVA,
                                    EGRESO_FEC = SelectTrasladoImputado.EGRESO_FEC,
                                    ID_ANIO = SelectTrasladoImputado.ID_ANIO,
                                    ID_CENTRO = SelectTrasladoImputado.ID_CENTRO,
                                    ID_ATENCION_MEDICA = SelectTrasladoImputado.ID_ATENCION_MEDICA,
                                    ID_CENTRO_TRASLADO = SelectTrasladoImputado.ID_CENTRO_TRASLADO,
                                    ID_ESTATUS = SelectTrasladoImputado.ID_ESTATUS,
                                    ID_ESTATUS_ADMINISTRATIVO = SelectTrasladoImputado.ID_ESTATUS_ADMINISTRATIVO,
                                    ID_IMPUTADO = SelectTrasladoImputado.ID_IMPUTADO,
                                    ID_INCIDENCIA_TRASLADO = SelectTrasladoImputado.ID_INCIDENCIA_TRASLADO,
                                    ID_INGRESO = SelectTrasladoImputado.ID_INGRESO,
                                    ID_MOTIVO = SelectTrasladoImputado.ID_MOTIVO,
                                    ID_TRASLADO = SelectTrasladoImputado.ID_TRASLADO,
                                    INCIDENCIA_OBSERVACION = SelectTrasladoImputado.INCIDENCIA_OBSERVACION,
                                    ID_CENTRO_UBI = SelectTrasladoImputado.ID_CENTRO_UBI,
                                };
                            #endregion

                            #region RECETA
                            if (ListRecetas != null ? ListRecetas.Any() : false)
                            {
                                RecetaMedica = new RECETA_MEDICA
                                {
                                    ID_FOLIO = 0,
                                    ID_USUARIO = GlobalVar.gUsr,
                                    RECETA_FEC = Fechas.GetFechaDateServer,
                                };
                                RecetaMedicaDetalle = new List<RECETA_MEDICA_DETALLE>(ListRecetas.Select(s => new RECETA_MEDICA_DETALLE
                                {
                                    CENA = s.HORA_NOCHE ? (short)1 : (short)0,
                                    COMIDA = s.HORA_TARDE ? (short)1 : (short)0,
                                    DESAYUNO = s.HORA_MANANA ? (short)1 : (short)0,
                                    DOSIS = s.CANTIDAD,
                                    DURACION = s.DURACION,
                                    ID_FOLIO = 0,
                                    ID_PRODUCTO = s.PRODUCTO.ID_PRODUCTO,
                                    OBSERV = s.OBSERVACIONES,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    ID_PRESENTACION_MEDICAMENTO = s.PRESENTACION,
                                }));
                            }
                            #endregion

                            if (SignosVitales != null ? SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null : false : false)
                                SignosVitales = null;
                            else
                            {
                                if (SignosVitales == null)
                                    SignosVitales = new NOTA_SIGNOS_VITALES();
                                SignosVitales.FRECUENCIA_CARDIAC = TextFrecuenciaCardiaca;
                                SignosVitales.FRECUENCIA_RESPIRA = FrecuenciaRespira;
                                SignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                SignosVitales.ID_RESPONSABLE = SelectEspecialista.PERSONA == null ? MedicoSupervisor.ID_PERSONA : SelectEspecialista.ID_PERSONA;
                                SignosVitales.OBSERVACIONES = ObservacionesSignosVitales;
                                SignosVitales.PESO = Peso;
                                SignosVitales.TALLA = Talla;
                                SignosVitales.GLUCEMIA = Glucemia;
                                SignosVitales.TEMPERATURA = Temperatura;
                                SignosVitales.TENSION_ARTERIAL = TensionArterial = TensionArterial;
                                SignosVitales.ID_CENTRO_UBI = GlobalVar.gCentro;
                            }

                            #endregion
                        }
                        else if (IsEnfermero)
                        {
                            #region enfermero
                            if (SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                                SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_MEDICA
                            : SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ?
                                SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_DENTAL
                            : false)
                            {
                                if (string.IsNullOrEmpty(TextEstatura))
                                {
                                    cacha = "EL CAMPO ESTATURA ES REQUERIDO.";
                                    ret = true;
                                    return;
                                }
                                #region HISTORIA CLINICA
                                var HistoriaClinicaDental = new HISTORIA_CLINICA_DENTAL();
                                var HistoriaClinica = new HISTORIA_CLINICA();
                                if (SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                                    SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_DENTAL
                                : false)
                                {
                                    HistoriaClinica = new HISTORIA_CLINICA
                                       {
                                           EF_PESO = Peso,
                                           EF_ESTATURA = Talla,
                                           EF_RESPIRACION = FrecuenciaRespira,
                                           EF_PULSO = TextFrecuenciaCardiaca,
                                           EF_TEMPERATURA = Temperatura,
                                           EF_PRESION_ARTERIAL = TensionArterial1 + "/" + TensionArterial2,
                                           ESTATUS = "I",
                                           ID_CENTRO = SelectIngreso.ID_CENTRO,
                                           ID_ANIO = SelectIngreso.ID_ANIO,
                                           ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                           ID_INGRESO = SelectIngreso.ID_INGRESO,
                                           ESTUDIO_FEC = hoy,
                                       };
                                    HistoriaClinicaDental = null;
                                }
                                if (SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ?
                                    SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_DENTAL
                                : false)
                                {
                                    HistoriaClinicaDental = new HISTORIA_CLINICA_DENTAL
                                       {
                                           PESO = Peso,
                                           GLICEMIA = Glucemia,
                                           FRECUENCIA_RESPIRA = FrecuenciaRespira,
                                           FRECUENCIA_CARDIAC = TextFrecuenciaCardiaca,
                                           TEMPERATURA = Temperatura,
                                           TENSION_ARTERIAL = TensionArterial1 + "/" + TensionArterial2,
                                           ESTATUS = "I",
                                           ESTATURA = TextEstatura,
                                           ID_CENTRO = SelectIngreso.ID_CENTRO,
                                           ID_ANIO = SelectIngreso.ID_ANIO,
                                           ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                           ID_INGRESO = SelectIngreso.ID_INGRESO,
                                           REGISTRO_FEC = hoy,
                                       };
                                    HistoriaClinica = null;
                                }
                                AtencionCita = new ATENCION_CITA()
                                {
                                    CITA_FECHA_HORA = SelectCitaMedicaImputado.CITA_FECHA_HORA,
                                    CITA_HORA_TERMINA = SelectCitaMedicaImputado.CITA_HORA_TERMINA,
                                    ESTATUS = ParametroEstatusCitaAtendiendo,
                                    ID_ANIO = SelectCitaMedicaImputado.ID_ANIO,
                                    ID_AREA = SelectCitaMedicaImputado.ID_AREA,
                                    ID_ATENCION = SelectCitaMedicaImputado.ID_ATENCION,
                                    ID_CENTRO = SelectCitaMedicaImputado.ID_CENTRO,
                                    ID_CITA = SelectCitaMedicaImputado.ID_CITA,
                                    ID_CENTRO_UBI = SelectCitaMedicaImputado.ID_CENTRO_UBI,
                                    ID_IMPUTADO = SelectCitaMedicaImputado.ID_IMPUTADO,
                                    ID_INGRESO = SelectCitaMedicaImputado.ID_INGRESO,
                                    ID_RESPONSABLE = SelectCitaMedicaImputado.ID_RESPONSABLE,
                                    ID_TIPO_ATENCION = SelectCitaMedicaImputado.ID_TIPO_ATENCION,
                                    ID_TIPO_SERVICIO = SelectCitaMedicaImputado.ID_TIPO_SERVICIO,
                                    ID_USUARIO = SelectCitaMedicaImputado.ID_USUARIO,
                                    ID_ATENCION_MEDICA = SelectCitaMedicaImputado.ID_ATENCION_MEDICA,
                                };
                                if (new cNotaMedica().InsertarHistoriaClinicaXEnfermero(HistoriaClinica, AtencionCita, HistoriaClinicaDental))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(async delegate
                                    {
                                        var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Exito!", "Los signos vitales han sido guardados correctamente.");
                                        clickSwitch("limpiar_menu");
                                    }));
                                    MenuGuardarEnabled = false;
                                    ret = false;
                                    return;
                                }
                                #endregion
                            }
                            else
                            {
                                #region CONSULTA MEDICA
                                SignosVitales.FRECUENCIA_CARDIAC = TextFrecuenciaCardiaca;
                                SignosVitales.FRECUENCIA_RESPIRA = FrecuenciaRespira;
                                SignosVitales.ID_RESPONSABLE = SelectEspecialista.ID_PERSONA == null ? MedicoSupervisor.ID_PERSONA : SelectEspecialista.ID_PERSONA;
                                SignosVitales.OBSERVACIONES = ObservacionesSignosVitales;
                                SignosVitales.PESO = Peso;
                                SignosVitales.TALLA = Talla;
                                SignosVitales.GLUCEMIA = Glucemia;
                                SignosVitales.TEMPERATURA = Temperatura;
                                SignosVitales.TENSION_ARTERIAL = TensionArterial;
                                SignosVitales.ID_CENTRO_UBI = GlobalVar.gCentro;
                                SignosVitales.ID_ATENCION_MEDICA = SelectedSignosVitales == null ? 0 : SelectedSignosVitales.ID_ATENCION_MEDICA;
                                if (SelectCitaMedicaImputado != null)
                                    AtencionCita = new ATENCION_CITA()
                                    {
                                        CITA_FECHA_HORA = SelectCitaMedicaImputado.CITA_FECHA_HORA,
                                        CITA_HORA_TERMINA = SelectCitaMedicaImputado.CITA_HORA_TERMINA,
                                        ESTATUS = ParametroEstatusCitaAtendiendo,
                                        ID_ANIO = SelectCitaMedicaImputado.ID_ANIO,
                                        ID_AREA = SelectCitaMedicaImputado.ID_AREA,
                                        ID_ATENCION = SelectCitaMedicaImputado.ID_ATENCION,
                                        ID_CENTRO = SelectCitaMedicaImputado.ID_CENTRO,
                                        ID_CITA = SelectCitaMedicaImputado.ID_CITA,
                                        ID_CENTRO_UBI = SelectCitaMedicaImputado.ID_CENTRO_UBI,
                                        ID_IMPUTADO = SelectCitaMedicaImputado.ID_IMPUTADO,
                                        ID_INGRESO = SelectCitaMedicaImputado.ID_INGRESO,
                                        ID_RESPONSABLE = SelectCitaMedicaImputado.ID_RESPONSABLE,
                                        ID_TIPO_ATENCION = SelectCitaMedicaImputado.ID_TIPO_ATENCION,
                                        ID_TIPO_SERVICIO = SelectCitaMedicaImputado.ID_TIPO_SERVICIO,
                                        ID_USUARIO = GlobalVar.gUsr
                                    };
                                if (SelectExcarcelacionImputado != null)
                                    Excarcelacion = new EXCARCELACION
                                    {
                                        CANCELADO_TIPO = SelectExcarcelacionImputado.CANCELADO_TIPO,
                                        CERT_MEDICO_SALIDA = SelectExcarcelacionImputado.CERT_MEDICO_SALIDA,
                                        CERT_MEDICO_RETORNO = SelectExcarcelacionImputado.CERT_MEDICO_RETORNO,
                                        CERTIFICADO_MEDICO = SelectExcarcelacionImputado.CERTIFICADO_MEDICO,
                                        ID_ANIO = SelectExcarcelacionImputado.ID_ANIO,
                                        ID_CENTRO = SelectExcarcelacionImputado.ID_CENTRO,
                                        ID_CONSEC = SelectExcarcelacionImputado.ID_CONSEC,
                                        ID_ESTATUS = SelectExcarcelacionImputado.ID_ESTATUS,
                                        ID_IMPUTADO = SelectExcarcelacionImputado.ID_IMPUTADO,
                                        ID_INCIDENCIA_TRASLADO = SelectExcarcelacionImputado.ID_INCIDENCIA_TRASLADO,
                                        ID_CENTRO_UBI = SelectExcarcelacionImputado.ID_CENTRO_UBI,
                                        ID_INGRESO = SelectExcarcelacionImputado.ID_INGRESO,
                                        ID_TIPO_EX = SelectExcarcelacionImputado.ID_TIPO_EX,
                                        ID_USUARIO = SelectExcarcelacionImputado.ID_USUARIO,
                                        INCIDENCIA_OBSERVACION = SelectExcarcelacionImputado.INCIDENCIA_OBSERVACION,
                                        OBSERVACION = SelectExcarcelacionImputado.OBSERVACION,
                                        PROGRAMADO_FEC = SelectExcarcelacionImputado.PROGRAMADO_FEC,
                                        REGISTRO_FEC = SelectExcarcelacionImputado.REGISTRO_FEC,
                                        RETORNO_FEC = SelectExcarcelacionImputado.RETORNO_FEC,
                                        SALIDA_FEC = SelectExcarcelacionImputado.SALIDA_FEC,
                                        TRASLADO_INCIDENCIA_TIPO = SelectExcarcelacionImputado.TRASLADO_INCIDENCIA_TIPO,
                                        ID_INCIDENCIA_TRASLADO_RETORNO = SelectExcarcelacionImputado.ID_INCIDENCIA_TRASLADO_RETORNO,
                                        RESPONSABLE = SelectExcarcelacionImputado.RESPONSABLE,
                                        INCIDENCIA_OBSERVACION_RETORNO = SelectExcarcelacionImputado.INCIDENCIA_OBSERVACION_RETORNO,
                                    };
                                if (SelectTrasladoImputado != null)
                                    TrasladoDetalle = new TRASLADO_DETALLE
                                    {
                                        AMP_ID_AMPARO_INDIRECTO = SelectTrasladoImputado.AMP_ID_AMPARO_INDIRECTO,
                                        AMP_ID_ANIO = SelectTrasladoImputado.AMP_ID_ANIO,
                                        AMP_ID_CENTRO = SelectTrasladoImputado.AMP_ID_CENTRO,
                                        AMP_ID_IMPUTADO = SelectTrasladoImputado.AMP_ID_IMPUTADO,
                                        AMP_ID_INGRESO = SelectTrasladoImputado.AMP_ID_INGRESO,
                                        CANCELADO_OBSERVA = SelectTrasladoImputado.CANCELADO_OBSERVA,
                                        EGRESO_FEC = SelectTrasladoImputado.EGRESO_FEC,
                                        ID_ANIO = SelectTrasladoImputado.ID_ANIO,
                                        ID_CENTRO = SelectTrasladoImputado.ID_CENTRO,
                                        ID_ATENCION_MEDICA = SelectTrasladoImputado.ID_ATENCION_MEDICA,
                                        ID_CENTRO_TRASLADO = SelectTrasladoImputado.ID_CENTRO_TRASLADO,
                                        ID_ESTATUS = SelectTrasladoImputado.ID_ESTATUS,
                                        ID_ESTATUS_ADMINISTRATIVO = SelectTrasladoImputado.ID_ESTATUS_ADMINISTRATIVO,
                                        ID_IMPUTADO = SelectTrasladoImputado.ID_IMPUTADO,
                                        ID_INCIDENCIA_TRASLADO = SelectTrasladoImputado.ID_INCIDENCIA_TRASLADO,
                                        ID_INGRESO = SelectTrasladoImputado.ID_INGRESO,
                                        ID_MOTIVO = SelectTrasladoImputado.ID_MOTIVO,
                                        ID_TRASLADO = SelectTrasladoImputado.ID_TRASLADO,
                                        INCIDENCIA_OBSERVACION = SelectTrasladoImputado.INCIDENCIA_OBSERVACION,
                                        ID_CENTRO_UBI = SelectTrasladoImputado.ID_CENTRO_UBI,
                                    };
                                if (new cNotaMedica().InsertarNotaMedicaEnfermeroTransaccion(SignosVitales, AtencionMedica, AtencionCita, Excarcelacion, TrasladoDetalle))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(async delegate
                                    {
                                        var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Exito!", "Los signos vitales han sido guardados correctamente.");
                                        clickSwitch("limpiar_menu");
                                    }));
                                    MenuGuardarEnabled = false;
                                    ret = false;
                                    return;
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else if (SelectAtencionMedica != null ? SelectAtencionMedica.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL : false)
                        {
                            #region NOTA MEDICA
                            NotaMedica.DIAGNOSTICO = Diagnostico;
                            NotaMedica.EXPLORACION_FISICA = ExploracionFisica;
                            NotaMedica.ID_ATENCION_MEDICA = SelectAtencionMedica != null ?
                                SelectExcarcelacionImputado != null ?
                                    SelectExcarcelacionImputado.CERT_MEDICO_RETORNO.HasValue ?
                                        SelectAtencionMedica.ID_ATENCION_MEDICA
                                    : SelectExcarcelacionImputado.CERT_MEDICO_SALIDA.HasValue ?
                                        SelectExcarcelacionImputado.ATENCION_MEDICA.NOTA_MEDICA == null ?
                                            SelectAtencionMedica.ID_ATENCION_MEDICA
                                        : 0
                                    : 0
                                : 0
                            : 0;

                            NotaMedica.ID_RESPONSABLE = SelectEspecialista.ID_PERSONA == null ?
                                MedicoSupervisor.ID_PERSONA
                            : new cEspecialistas().GetData(g => g.ID_ESPECIALISTA == SelectEspecialista.ID_PERSONA).Any(a => a.ID_PERSONA.HasValue) ?
                                new cEspecialistas().GetData(g => g.ID_ESPECIALISTA == SelectEspecialista.ID_PERSONA).First(f => f.ID_PERSONA.HasValue).ID_PERSONA
                            : MedicoSupervisor.ID_PERSONA;
                            NotaMedica.PLAN_ESTUDIO_TRATA = PlanEstudioTrat;
                            NotaMedica.ID_PRONOSTICO = SelectedPronostico;
                            NotaMedica.RESULTADO_SERV_AUX = ResultadoServAux;
                            NotaMedica.RESULTADO_SERV_TRA = ResultadoServTrat;
                            NotaMedica.RESUMEN_INTERROGAT = ResumenInterr;
                            NotaMedica.FIRMO_CONFORMIDAD = "S";
                            NotaMedica.OCUPA_INTERCONSULTA = SolicitaInterconsultaCheck ? "S" : "N";
                            NotaMedica.OCUPA_HOSPITALIZACION = CheckedHospitalizacion ? "S" : "N";
                            NotaMedica.ID_CENTRO_UBI = GlobalVar.gCentro;
                            #endregion

                            #region CITA
                            if (SelectCitaMedicaImputado != null)
                            {
                                AtencionCita = new ATENCION_CITA()
                                {
                                    CITA_FECHA_HORA = SelectCitaMedicaImputado.CITA_FECHA_HORA,
                                    CITA_HORA_TERMINA = SelectCitaMedicaImputado.CITA_HORA_TERMINA,
                                    ESTATUS = ParametroEstatusCitaConcluido,
                                    ID_ANIO = SelectCitaMedicaImputado.ID_ANIO,
                                    ID_AREA = SelectCitaMedicaImputado.ID_AREA,
                                    ID_ATENCION = SelectCitaMedicaImputado.ID_ATENCION,
                                    ID_CENTRO = SelectCitaMedicaImputado.ID_CENTRO,
                                    ID_CITA = SelectCitaMedicaImputado.ID_CITA,
                                    ID_CENTRO_UBI = SelectCitaMedicaImputado.ID_CENTRO_UBI,
                                    ID_IMPUTADO = SelectCitaMedicaImputado.ID_IMPUTADO,
                                    ID_INGRESO = SelectCitaMedicaImputado.ID_INGRESO,
                                    ID_RESPONSABLE = SelectMedico,
                                    ID_TIPO_ATENCION = SelectCitaMedicaImputado.ID_TIPO_ATENCION,
                                    ID_TIPO_SERVICIO = SelectCitaMedicaImputado.ID_TIPO_SERVICIO,
                                    ID_USUARIO = GlobalVar.gUsr,
                                };
                                AtencionMedica.ID_ATENCION_MEDICA = SelectCitaMedicaImputado.ID_ATENCION_MEDICA.HasValue ? SelectCitaMedicaImputado.ID_ATENCION_MEDICA.Value : 0;
                                if (SelectCitaMedicaImputado.ATENCION_MEDICA != null ? SelectCitaMedicaImputado.ATENCION_MEDICA.NOTA_SIGNOS_VITALES == null : false)
                                {
                                    SignosVitales.FRECUENCIA_CARDIAC = TextFrecuenciaCardiaca;
                                    SignosVitales.FRECUENCIA_RESPIRA = FrecuenciaRespira;
                                    SignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                    SignosVitales.ID_RESPONSABLE = SelectMedico;
                                    SignosVitales.OBSERVACIONES = ObservacionesSignosVitales;
                                    SignosVitales.PESO = Peso;
                                    SignosVitales.TALLA = Talla;
                                    SignosVitales.GLUCEMIA = Glucemia;
                                    SignosVitales.TEMPERATURA = Temperatura;
                                    SignosVitales.TENSION_ARTERIAL = TensionArterial = TensionArterial;
                                }
                                else
                                    SignosVitales = null;
                            }
                            #endregion

                            #region SIGNOS VITALES
                            if (SignosVitales != null)
                                SignosVitales = null;
                            else
                            {
                                if (SignosVitales == null)
                                    SignosVitales = new NOTA_SIGNOS_VITALES();
                                SignosVitales.FRECUENCIA_CARDIAC = TextFrecuenciaCardiaca;
                                SignosVitales.FRECUENCIA_RESPIRA = FrecuenciaRespira;
                                SignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                SignosVitales.ID_RESPONSABLE = SelectEspecialista.PERSONA == null ? MedicoSupervisor.ID_PERSONA : SelectEspecialista.ID_PERSONA;
                                SignosVitales.OBSERVACIONES = ObservacionesSignosVitales;
                                SignosVitales.PESO = Peso;
                                SignosVitales.TALLA = Talla;
                                SignosVitales.GLUCEMIA = Glucemia;
                                SignosVitales.TEMPERATURA = Temperatura;
                                SignosVitales.TENSION_ARTERIAL = TensionArterial = TensionArterial;
                                SignosVitales.ID_CENTRO_UBI = GlobalVar.gCentro;
                            }
                            #endregion

                            #region dentista
                            /*
                            if (SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ?
                                SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_DENTAL
                            : false)
                            {
                                #region HISTORIA CLINICA
                                var HistoriaClinicaDental = new HISTORIA_CLINICA_DENTAL();
                                HistoriaClinicaDental = new HISTORIA_CLINICA_DENTAL
                                   {
                                       PESO = Peso,
                                       GLICEMIA = Talla,
                                       FRECUENCIA_RESPIRA = FrecuenciaRespira,
                                       FRECUENCIA_CARDIAC = TextFrecuenciaCardiaca,
                                       TEMPERATURA = Temperatura,
                                       TENSION_ARTERIAL = TensionArterial1 + "/" + TensionArterial2,
                                       ESTATUS = "I",
                                       ID_CENTRO = SelectIngreso.ID_CENTRO,
                                       ID_ANIO = SelectIngreso.ID_ANIO,
                                       ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                       ID_INGRESO = SelectIngreso.ID_INGRESO,
                                       REGISTRO_FEC = hoy,
                                   };
                                AtencionCita = new ATENCION_CITA()
                                {
                                    CITA_FECHA_HORA = SelectCitaMedicaImputado.CITA_FECHA_HORA,
                                    CITA_HORA_TERMINA = SelectCitaMedicaImputado.CITA_HORA_TERMINA,
                                    ESTATUS = Parametro.AT_CITA_ESTATUS_ATENDIENDO,
                                    ID_ANIO = SelectCitaMedicaImputado.ID_ANIO,
                                    ID_AREA = SelectCitaMedicaImputado.ID_AREA,
                                    ID_ATENCION = SelectCitaMedicaImputado.ID_ATENCION,
                                    ID_CENTRO = SelectCitaMedicaImputado.ID_CENTRO,
                                    ID_CITA = SelectCitaMedicaImputado.ID_CITA,
                                    ID_IMPUTADO = SelectCitaMedicaImputado.ID_IMPUTADO,
                                    ID_INGRESO = SelectCitaMedicaImputado.ID_INGRESO,
                                    ID_RESPONSABLE = SelectCitaMedicaImputado.ID_RESPONSABLE,
                                    ID_TIPO_ATENCION = SelectCitaMedicaImputado.ID_TIPO_ATENCION,
                                    ID_TIPO_SERVICIO = SelectCitaMedicaImputado.ID_TIPO_SERVICIO,
                                    ID_USUARIO = SelectCitaMedicaImputado.ID_USUARIO,
                                    //checar si el doctor puede atender aunque este un enfermero asignado
                                };
                                if (new cNotaMedica().InsertarHistoriaClinicaXDentista(AtencionCita, HistoriaClinicaDental))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(async delegate
                                    {
                                        var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Exito!", "Los signos vitales han sido guardada correctamente.");
                                        clickSwitch("limpiar_menu");
                                    }));
                                    MenuGuardarEnabled = false;
                                    ret = false;
                                    return;
                                }
                                #endregion
                            }
                            else
                            {
                            }
                            */
                            #endregion
                        }
                        if (SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS)
                        {
                            #region PROCEDIMIENTO MEDICO AGENDADO
                            if (ListProcedimientosMedicosEnCita != null ? !ListProcedimientosMedicosEnCita.Any() : false)
                            {
                                //if (await new Dialogos().ConfirmacionDialogoReturn("Validacion", "Está seguro que desea guardar la consulta sin algun procedimiento médico?") != 1)
                                //{
                                //    cacha = "No hay ningun procedimiento material registrado.";
                                //    ret = true;
                                //    return;
                                //}
                                //else
                                //{
                                //    ret = false;
                                //}
                            }
                            if (ListProcedimientosMedicosEnCita.Any(a => a.PROCEDIMIENTOS_MATERIALES != null ? a.PROCEDIMIENTOS_MATERIALES.Count() > 0 : true))
                            {
                                if (ListProcedimientosMedicosEnCita.Where(a => a.PROCEDIMIENTOS_MATERIALES != null ? a.PROCEDIMIENTOS_MATERIALES.Count() > 0 : true).Any(a => a.PROCEDIMIENTOS_MATERIALES.Any(an => !an.CANTIDAD.HasValue)))
                                {
                                    cacha = "Hace falta ingresar el insumo utilizado de un procedimiento.";
                                    ret = true;
                                    return;
                                }
                            }
                            var proc_at_med_prog = SelectCitaMedicaImputado.PROC_ATENCION_MEDICA_PROG.Any(w => w.ID_CENTRO_CITA == GlobalVar.gCentro && !string.IsNullOrEmpty(w.ID_USUARIO_ASIGNADO) && w.ID_CITA == SelectCitaMedicaImputado.ID_CITA) ?
                                SelectCitaMedicaImputado.PROC_ATENCION_MEDICA_PROG.First(w => w.ID_CENTRO_CITA == GlobalVar.gCentro && !string.IsNullOrEmpty(w.ID_USUARIO_ASIGNADO) && w.ID_CITA == SelectCitaMedicaImputado.ID_CITA)
                            : null;
                            if (proc_at_med_prog == null)
                            {
                                cacha = "Ocurrió un error con la consulta seleccionada.";
                                ret = true;
                                return;
                            }
                            var ProcMedAgen = new List<PROC_MEDICO_PROG_DET>();
                            foreach (var item in ListProcedimientosMedicosEnCita.SelectMany(s => s.PROCEDIMIENTOS_MATERIALES))
                            {
                                ProcMedAgen.Add(new PROC_MEDICO_PROG_DET
                                {
                                    CANTIDAD_UTILIZADA = item.CANTIDAD,
                                    ID_PRODUCTO = item.PRODUCTO.ID_PRODUCTO,
                                    ID_AM_PROG = proc_at_med_prog.ID_AM_PROG,
                                    ID_PROCMED = item.PROC_MATERIAL.ID_PROCMED,
                                    REGISTRO_FEC = hoy,
                                    ID_ATENCION_MEDICA = proc_at_med_prog.ID_ATENCION_MEDICA,
                                    ID_CENTRO_UBI = proc_at_med_prog.ID_CENTRO_UBI,
                                });
                            }
                            SelectCitaMedicaImputado.ESTATUS = ParametroEstatusCitaConcluido;
                            SelectCitaMedicaImputado.ID_CENTRO_AT_SOL = GlobalVar.gCentro;
                            var NotaSignos = false;
                            NotaSignos = !(string.IsNullOrEmpty(Peso) && string.IsNullOrEmpty(Talla) && string.IsNullOrEmpty(Glucemia) && string.IsNullOrEmpty(TensionArterial1) && string.IsNullOrEmpty(TensionArterial2) &&
                                string.IsNullOrEmpty(TextFrecuenciaCardiaca) && string.IsNullOrEmpty(FrecuenciaRespira) && string.IsNullOrEmpty(Temperatura) && string.IsNullOrEmpty(ObservacionesSignosVitales));
                            var usr = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault();
                            var citas = new List<ATENCION_CITA>();
                            var idCita = 0;
                            var cit = new ATENCION_CITA();
                            var ProcAtMedProg = new PROC_ATENCION_MEDICA_PROG();
                            var enfermero = string.Empty;
                            if (ListProcMedsSeleccionados != null)
                            {
                                foreach (var item in ListProcMedsSeleccionados.SelectMany(s => s.CITAS).OrderBy(o => o.FECHA_INICIAL))
                                {
                                    var citaTraslape = new cAtencionCita().ObtieneCitaOtraArea(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO,
                                        SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, item.FECHA_INICIAL, item.FECHA_FINAL, AtencionCita != null ? AtencionCita.ID_CITA : 0);
                                    if (citaTraslape != null ? citaTraslape.ID_CITA != SelectCitaMedicaImputado.ID_CITA && citaTraslape.ID_CENTRO_UBI == SelectCitaMedicaImputado.ID_CENTRO_UBI : false)
                                    {
                                        // a la hora final hay que restarle un minuto para no traslapar horarios que comienzan terminan a la misma hora
                                        Application.Current.Dispatcher.Invoke((Action)(delegate
                                        {
                                            new Dialogos().ConfirmacionDialogo("Algo pasó...", "Existe un traslape en la fecha [ " + item.FECHA_INICIAL.ToString("dd/MM/yyyy HH:mm") + " ].");
                                        }));
                                        return;
                                    }
                                    idCita = new cNotaMedica().GetIDProceso<short>("ATENCION_CITA", "ID_CITA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
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
                                        enfermero = new cUsuario().Obtener(item.ENFERMERO).FirstOrDefault().ID_USUARIO;
                                        ProcAtMedProg = new PROC_ATENCION_MEDICA_PROG
                                        {
                                            ESTATUS = "N",
                                            ID_PROCMED = item.ID_PROC_MED,
                                            ID_USUARIO_ASIGNADO = enfermero,
                                            ID_CITA = idCita,
                                            ID_CENTRO_UBI = GlobalVar.gCentro,
                                            ID_CENTRO_CITA = GlobalVar.gCentro,
                                            REALIZACION_FEC = hoy,
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
                            if (new cNotaMedica().InsertarProcedimientoMedico(SelectCitaMedicaImputado, ProcMedAgen, AtencionMedica, NotaSignos ? SignosVitales : null, proc_at_med_prog, hoy, ProcedimientosIncidencias, citas))
                            {
                                Application.Current.Dispatcher.Invoke((Action)(async delegate
                                {
                                    var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Exito!", "Los procedimientos médicos han sido guardados correctamente.");
                                    clickSwitch("limpiar_menu");
                                }));
                                MenuGuardarEnabled = false;
                                ret = false;
                                return;
                            }
                            else
                                return;
                            #endregion
                        }
                        if (SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CITA_MEDICA && SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA)
                        {
                            #region NOTAS
                            if (!string.IsNullOrEmpty(TextUrgenciaDestino) || !string.IsNullOrEmpty(TextUrgenciaEstadoMental) || !string.IsNullOrEmpty(TextUrgenciaMotivo) || !string.IsNullOrEmpty(TextUrgenciaProcedimiento))
                                NotaUrgencia = new NOTA_URGENCIA
                                {
                                    DESTINO_DESPUES_UR = TextUrgenciaDestino,
                                    ESTADO_MENTAL = TextUrgenciaEstadoMental,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    MOTIVO_CONSULTA = TextUrgenciaMotivo,
                                    PROCEDIMIENTO_AREA = TextUrgenciaProcedimiento
                                };
                            if (!string.IsNullOrEmpty(TextActualizacion) || !string.IsNullOrEmpty(TextEvolucion) || !string.IsNullOrEmpty(TextExistencia))
                                NotaEvolucion = new NOTA_EVOLUCION
                                {
                                    //ACTUALIZACION_CUAD = TextActualizacion,
                                    //ID_CENTRO_UBI = GlobalVar.gCentro,
                                    //EVOLUCION = TextEvolucion,
                                    //EXT_NOTA_MEDICA = TextExistencia,
                                    //ID_RESPONSABLE = user.ID_PERSONA,
                                    //REGISTRO_FEC = hoy
                                };
                            if (!string.IsNullOrEmpty(TextInterconsultaCriterio) || !string.IsNullOrEmpty(TextInterconsultaMotivo) || !string.IsNullOrEmpty(TextInterconsultaSugerencias) || !string.IsNullOrEmpty(TextInterconsultaTratamiento))
                                NotaInterconsulta = new NOTA_INTERCONSULTA
                                {
                                    CRITERIO_DIAGNOSTI = TextInterconsultaCriterio,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    MOTIVO_CONSULTA = TextInterconsultaMotivo,
                                    REGISTRO_FEC = hoy,
                                    SUGERENCIAS_DIAGNO = TextInterconsultaSugerencias,
                                    TRATAMIENTO = TextInterconsultaTratamiento,
                                    ID_PERSONA = user.ID_PERSONA
                                };
                            if (!string.IsNullOrEmpty(TextTrasladoEnvia) || !string.IsNullOrEmpty(TextTrasladoReceptor) || !string.IsNullOrEmpty(TextTrasladoMotivo))
                                NotaTraslado = new NOTA_REFERENCIA_TR
                                {
                                    ESTABLECIMIENTO_EN = TextTrasladoEnvia,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    ESTABLECIMIENTO_RE = TextTrasladoReceptor,
                                    MOTIVO_ENVIO = TextTrasladoMotivo
                                };
                            if (!string.IsNullOrEmpty(TextPreOpCuidados) || !string.IsNullOrEmpty(TextPreOpDiagnostico) || (TextPreOpFecha.HasValue ? TextPreOpFecha.Value > hoy : false)
                                 || !string.IsNullOrEmpty(TextPreOpPlan) || !string.IsNullOrEmpty(TextPreOpPlanTerapeutico) || !string.IsNullOrEmpty(TextPreOpRiesgo))
                                NotaPreOperatoria = new NOTA_PRE_OPERATORI
                                {
                                    CUIDADOS = TextPreOpCuidados,
                                    DIAGNOSTICO_PREOPE = TextPreOpDiagnostico,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    FEC_CIRUGIA_REALIZ = TextPreOpFecha,
                                    ID_RESPONSABLE = user.ID_PERSONA,
                                    PLAN_QUIRURGICO = TextPreOpPlan,
                                    PLAN_TERAPEUTICO_P = TextPreOpPlanTerapeutico,
                                    RIESGO_QUIRURGICO = TextPreOpRiesgo,
                                    REGISTRO_FEC = hoy
                                };
                            if (!string.IsNullOrEmpty(TextPreAnestEvolucion) || !string.IsNullOrEmpty(TextPreAnestRiesgo) || !string.IsNullOrEmpty(TextPreAnestTipo))
                                NotaPreAnestecica = new NOTA_PRE_ANESTECIC
                                {
                                    EVALUACION_CLINICA = TextPreAnestEvolucion,
                                    ID_RESPONSABLE = user.ID_PERSONA,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    REGISTRO_FEC = hoy,
                                    RIESGO_ANESTESICO = TextPreAnestRiesgo,
                                    TIPO_ANESTESIA = TextPreAnestTipo
                                };
                            if (!string.IsNullOrEmpty(TextPostOpAccidentes) || !string.IsNullOrEmpty(TextPostOpSangrado) || !string.IsNullOrEmpty(TextPostOpTecnicaQuirurgica) || !string.IsNullOrEmpty(TextPostOpDiagnosticoPostOp)
                                || !string.IsNullOrEmpty(TextPostOpPiezasBiopsias) || !string.IsNullOrEmpty(TextPostOpEstadoInmediato) || !string.IsNullOrEmpty(TextPostOpHallazgosTransoperatorios) || !string.IsNullOrEmpty(TextPostOpIncidentes)
                                || !string.IsNullOrEmpty(TextPostOpPlaneada) || !string.IsNullOrEmpty(TextPostOpRealizada) || !string.IsNullOrEmpty(TextPostOpTratamientoInmediato) || !string.IsNullOrEmpty(TextPostOpGasasCompresas)
                                || !string.IsNullOrEmpty(TextPostOpInterpretacion))
                                NotaPostOperatoria = new NOTA_POST_OPERATOR
                                {
                                    ACCIDENTES = TextPostOpAccidentes,
                                    CUANTIFICACION_SAN = TextPostOpSangrado,
                                    DESCR_TECNICA_QUIR = TextPostOpTecnicaQuirurgica,
                                    DIAGNOSTICO_POST_O = TextPostOpDiagnosticoPostOp,
                                    ENVIO_EXAMEN_MACRO = TextPostOpPiezasBiopsias,
                                    ESTADO_POST_QUIRUR = TextPostOpEstadoInmediato,
                                    HALLASZGOS_TRANSOP = TextPostOpHallazgosTransoperatorios,
                                    ID_RESPONSABLE = user.ID_PERSONA,
                                    INCIDENTES = TextPostOpIncidentes,
                                    OPERACION_PLANEADA = TextPostOpPlaneada,
                                    OPERACION_REALIZAD = TextPostOpRealizada,
                                    PLAN_TRATAMIENTO_P = TextPostOpTratamientoInmediato,
                                    REGISTRO_FEC = hoy,
                                    REPORTE_GASAS_COMP = TextPostOpGasasCompresas,
                                    RESULTADOS = TextPostOpInterpretacion,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                };

                            #endregion
                        }
                        if ((SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO || SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_INTEGRIDAD_FISICA)
                            && SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA)
                        {
                            #region CERTIFICADO NUEVO INGRESO
                            certificado = new CERTIFICADO_MEDICO
                            {
                                ANTECEDENTES_PATOLOGICOS = TextAntecedentesPatologicos,
                                EXPLORACION_FISICA = TextSeDetecto,
                                NUEVO_INGRESO = SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO || SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO ? "S" : "N",
                                PADECIMIENTO = TextPadecimientoYTratamientoActual,
                                TOXICOMANIAS = CheckedToxicomanias ? TextToxicomanias : null,
                                AMERITA_HOSPITALIZACION = CheckedHospitalizacion ? "S" : "N",
                                PELIGRA_VIDA = CheckedPeligroVida ? "S" : "N",
                                TARDA_SANAR_15 = Checked15DiasSanar ? "S" : "N",
                                DIAGNOSTICO = string.Empty, // TextDiagnosticoCertificado,
                                PLAN_TERAPEUTICO = TextPlanTerapeuticoCertificado,
                                OBSERVACIONES = TextObservacionesConclusionesCertificado,
                                CANCELADO_OBSERV = "N",
                                ES_SANCION = SelectSancionImputado != null ? "S" : "N",
                                ESTATUS = "1",
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                            };
                            if (ListEnfermedades != null ? ListEnfermedades.Any() : false)
                            {
                                var x = "";
                                var i = 1;
                                foreach (var item in ListEnfermedades)
                                {
                                    x = x + item.NOMBRE + (i <= ListEnfermedades.Count ? ", " : " ");
                                    i++;
                                }
                                certificado.DIAGNOSTICO = x;
                            }
                            if (ListRecetas != null ? ListRecetas.Any() : false)
                            {
                                var x = "";
                                var i = 1;
                                foreach (var item in ListRecetas)
                                {
                                    x = x + item.CANTIDAD + " " + item.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE.Trim() + " " +
                                        (item.HORA_MANANA ? (item.HORA_TARDE ? (item.HORA_NOCHE ? "EN LA MAÑANA, TARDE Y NOCHE" : "EN LA MAÑANA Y TARDE") :
                                        (item.HORA_NOCHE ? "EN LA MAÑANA Y NOCHE" : "MAÑANA")) :
                                        (item.HORA_TARDE ? (item.HORA_NOCHE ? "EN LA TARDE Y NOCHE" : "EN LA TARDE") :
                                        (item.HORA_NOCHE ? "EN LA NOCHE" : ""))) +
                                        " DE " + item.PRODUCTO.NOMBRE.Trim() + " POR " + (item.DURACION) + " " + "DIA" + (item.DURACION > 1 ? "S" : "") + (i <= ListRecetas.Count ? ", " : " ");
                                    i++;
                                }
                                certificado.PLAN_TERAPEUTICO = x;
                            }
                            #endregion
                        }
                        if (ListDietas.Any(a => a.ELEGIDO))
                            dietas = ListDietas.Where(w => w.ELEGIDO).Select(s => s.DIETA).ToList();
                        if (ListEnfermedades != null ? ListEnfermedades.Any() : false)
                            Enfermedades = ListEnfermedades.Select(s => new NOTA_MEDICA_ENFERMEDAD { ID_ENFERMEDAD = s.ID_ENFERMEDAD, REGISTRO_FEC = hoy, ENFERMEDAD = s, }).ToList();
                        if (ListLesiones != null ? ListLesiones.Any() : false)
                            lesiones = ListLesiones.Select(s => new LESION
                            {
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_ATENCION_MEDICA = SelectAtencionMedica != null ? SelectAtencionMedica.ID_ATENCION_MEDICA : 0,
                                ID_LESION = 0,
                                ID_REGION = s.REGION.ID_REGION,
                                DESCR = s.DESCR,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                            }).ToList();
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
                                    ID_NOPATOLOGICO = sel.HISTORIA_CLINICA_PATOLOGICOS.ID_NOPATOLOGICO,
                                }).ToList();
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
                                cacha = "Favor de seleccionar el control prenatal que utiliza.";
                                ret = true;
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        cacha = "Ocurrió un error al intentar guardar. \n" + ex;
                        ret = true;
                        return;
                    }
                });

                if (ret)
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                       {
                           new Dialogos().ConfirmacionDialogo("Validación", cacha);
                       }));
                    return;
                }
                if (IsMedico || IsDentista)
                {
                    if (SelectIngreso.ATENCION_MEDICA.Any(a => a.HOSPITALIZACION != null ?
                        a.HOSPITALIZACION.ID_HOSEST == 1
                    : false))
                    {
                        HuellaCompromiso = true;
                        TextNIPCartaCompromiso = string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NIP) ? string.Empty : SelectIngreso.IMPUTADO.NIP.Trim();
                        clickSwitch("acepto_compromiso");
                        return;
                    }

                    if (SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS) return;

                    #region CARTA COMPROMISO
                    var sDietas = "";
                    if (dietas != null)
                        foreach (var item in dietas)
                            sDietas = sDietas + item.DESCR + ", ";
                    var dataset = new List<cReporteCompromiso>();
                    CartaView = new CartaCompromisoAceptaView();
                    CartaView.Closed += CartaCompromisoClose;
                    var idx = string.Empty;
                    if (ListEnfermedades != null ? ListEnfermedades.Any() : false)
                        foreach (var item in ListEnfermedades)
                            idx = idx + item.NOMBRE + ", ";
                    else
                        idx = "N/A";
                    dataset.Add(new cReporteCompromiso()
                    {
                        Centro = SelectIngreso.CENTRO1.DESCR.Trim(),
                        Expediente = SelectExpediente.ID_ANIO.ToString() + "/" + SelectExpediente.ID_IMPUTADO.ToString(),
                        Fecha = hoy.ToString(),
                        Medico = user.EMPLEADO.PERSONA.NOMBRE.Trim() + " " + user.EMPLEADO.PERSONA.PATERNO.Trim() + " " + user.EMPLEADO.PERSONA.MATERNO.Trim(),
                        NombrePaciente = SelectExpediente.NOMBRE.Trim() + " " + SelectExpediente.PATERNO.Trim() + " " + SelectExpediente.MATERNO.Trim(),
                        Sexo = SelectExpediente.SEXO == "M" ? "MASCULINO" : "FEMENINO",
                        Ubicacion = SelectIngreso.CAMA != null ?
                            SelectIngreso.CAMA.CELDA != null ?
                                SelectIngreso.CAMA.CELDA.SECTOR != null ?
                                    SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ?
                                        SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() + "" + SelectIngreso.CAMA.CELDA.ID_CELDA.ToString().Trim() + "-" + SelectIngreso.CAMA.ID_CAMA
                                    : string.Empty
                                : string.Empty
                            : string.Empty
                        : string.Empty,
                        Dieta = sDietas,
                        IDX = idx,
                        YoNombre = SelectExpediente.NOMBRE.Trim() + " " + SelectExpediente.PATERNO.Trim() + " " + SelectExpediente.MATERNO.Trim(),
                        Logo1 = ParametroLogoEstado,
                        Logo2 = ParametroReporteLogo2,
                    });
                    CartaView.ReporteCompromiso.LocalReport.ReportPath = "Reportes/rCartaCompromisoMedico.rdlc";
                    CartaView.ReporteCompromiso.LocalReport.DataSources.Clear();
                    var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = dataset;
                    CartaView.DataContext = this;
                    CartaView.ReporteCompromiso.LocalReport.DataSources.Add(rds1);
                    CartaView.ReporteCompromiso.ShowExportButton = false;
                    CartaView.ReporteCompromiso.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    CartaView.ReporteCompromiso.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                    CartaView.ReporteCompromiso.ZoomPercent = 100;
                    CartaView.ReporteCompromiso.RefreshReport();
                    CartaView.Owner = PopUpsViewModels.MainWindow;
                    HuellaWindow = new BuscarPorHuellaYNipView();
                    HuellaWindow.DataContext = this;
                    HuellaWindow.ShowInTaskbar = false;
                    HuellaWindow.WindowState = WindowState.Minimized;
                    ConstructorHuella(0);
                    HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                    HuellaWindow.Show();
                    HuellaCompromiso = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    CartaView.ShowDialog();
                    HuellaWindow.Close();
                    //HuellaWindow = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
            }
        }

        private void CartaCompromisoClose(object sender, EventArgs e)
        {
            try
            {
                TextNIPCartaCompromiso = string.Empty;
                CartaView.Closed -= CartaCompromisoClose;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la carta de compromiso.", ex);
            }
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
                            REGISTRO_FEC = s.REGISTRO_FEC
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

                if (SelectIngreso.HISTORIA_CLINICA.Any() && SelectIngreso.IMPUTADO.SEXO == "F")
                {
                    MujeresEnabled = !EditarCitaEspecialista;
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
                        MujeresAuxiliar.FECHA_PROBABLE_PARTO = FechaProbParto = gineco.FECHA_PROBABLE_PARTO;
                        foreach (var item in hc.HISTORIA_CLINICA_GINECO_OBSTRE)
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

        private void SeleccionaIngreso()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    AnioBuscar = SelectIngreso.ID_ANIO;
                    FolioBuscar = SelectIngreso.ID_IMPUTADO;
                    ApellidoPaternoBuscar = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                    ApellidoMaternoBuscar = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                    NombreBuscar = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                        SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO
                        : new Imagenes().getImagenPerson();
                    TextAnioImputado = SelectIngreso.ID_ANIO;
                    TextFolioImputado = SelectIngreso.ID_IMPUTADO;
                    TextPaternoImputado = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                    TextMaternoImputado = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                    TextNombreImputado = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                    FotoIngreso = SelectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                        SelectIngreso.INGRESO_BIOMETRICO.First(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO :
                            SelectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                                SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                        ImagenImputado = new Imagenes().getImagenPerson();
                    //MenuHClinicaEnabled = true;
                    ProcesaDatosPaciente();
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }

        private void ProcesaDatosPaciente()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    NombrePaciente = string.Format("{0} {1} {2}", SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                                                                  SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                                                                  SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);
                    Sexo = SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : SelectIngreso.IMPUTADO.SEXO == "F" ? "FEMENINO" : string.Empty;
                    Edad = SelectIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty;
                    MujeresVisible = SelectIngreso.IMPUTADO.SEXO == "M" ? Visibility.Collapsed : Visibility.Visible;
                    ListTipoServicio = new ObservableCollection<ATENCION_SERVICIO>((IsMedico ? SelectIngreso.ATENCION_MEDICA.Any(a => a.CERTIFICADO_MEDICO != null ? a.CERTIFICADO_MEDICO.NUEVO_INGRESO == "S" : false) : false) ?
                        ListTipoServicioAux.Where(w => w.ID_TIPO_SERVICIO != (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO)
                    : ListTipoServicioAux);
                }
                else
                    return;
            }
            catch (Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", exc);
            }
        }

        private void ObtenerSignosVitales()
        {
            try
            {
                if (IsMedico || IsDentista)
                {
                    if (AtencionSeleccionada && SelectAtencionMedicaAux != null)
                    {
                        var aux = SelectAtencionMedicaAux;
                        SelectAtencionMedica = aux;
                    }
                    if (SelectExcarcelacionImputado != null)
                        SelectAtencionMedica = SelectExcarcelacionImputado.CERT_MEDICO_SALIDA.HasValue ?
                            SelectExcarcelacionImputado.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ?
                                SelectExcarcelacionImputado.CERT_MEDICO_RETORNO.HasValue ?
                                    SelectExcarcelacionImputado.ATENCION_MEDICA1.NOTA_SIGNOS_VITALES != null ?
                                        SelectExcarcelacionImputado.ATENCION_MEDICA1
                                    : SelectExcarcelacionImputado.ATENCION_MEDICA
                                : SelectExcarcelacionImputado.ATENCION_MEDICA
                            : SelectExcarcelacionImputado.ATENCION_MEDICA
                        : null;
                    if (SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null : false)
                    {
                        SignosVitalesExpandidos = false;
                        SignosVitalesReadOnly = false;
                        Peso = SelectAtencionMedica.NOTA_SIGNOS_VITALES.PESO ?? string.Empty;
                        Talla = SelectAtencionMedica.NOTA_SIGNOS_VITALES.TALLA ?? string.Empty;
                        Glucemia = SelectAtencionMedica.NOTA_SIGNOS_VITALES.GLUCEMIA ?? string.Empty;
                        if (!string.IsNullOrEmpty(SelectAtencionMedica.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL) ? SelectAtencionMedica.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL.Contains("/") : false)
                        {
                            TensionArterial1 = SelectAtencionMedica.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL.Split('/')[0];
                            TensionArterial2 = SelectAtencionMedica.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL.Split('/')[1];
                        }
                        TextFrecuenciaCardiaca = SelectAtencionMedica.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC ?? string.Empty;
                        FrecuenciaRespira = SelectAtencionMedica.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA ?? string.Empty;
                        Temperatura = SelectAtencionMedica.NOTA_SIGNOS_VITALES.TEMPERATURA ?? string.Empty;
                        IdResponsableSignosVitales = SelectAtencionMedica.NOTA_SIGNOS_VITALES.ID_RESPONSABLE ?? new int?();
                        ObservacionesSignosVitales = SelectAtencionMedica.NOTA_SIGNOS_VITALES.OBSERVACIONES ?? string.Empty;
                        ResponsableSignosVitales = SelectAtencionMedica.NOTA_SIGNOS_VITALES.ID_RESPONSABLE.HasValue ?
                            string.Format("{0} {1} {2}", SelectAtencionMedica.NOTA_SIGNOS_VITALES.PERSONA != null ? !string.IsNullOrEmpty(SelectAtencionMedica.NOTA_SIGNOS_VITALES.PERSONA.NOMBRE) ?
                                SelectAtencionMedica.NOTA_SIGNOS_VITALES.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    SelectAtencionMedica.NOTA_SIGNOS_VITALES.PERSONA != null ? !string.IsNullOrEmpty(SelectAtencionMedica.NOTA_SIGNOS_VITALES.PERSONA.PATERNO) ?
                                        SelectAtencionMedica.NOTA_SIGNOS_VITALES.PERSONA.PATERNO.Trim() : string.Empty : string.Empty,
                                            SelectAtencionMedica.NOTA_SIGNOS_VITALES.PERSONA != null ? !string.IsNullOrEmpty(SelectAtencionMedica.NOTA_SIGNOS_VITALES.PERSONA.MATERNO) ?
                                                SelectAtencionMedica.NOTA_SIGNOS_VITALES.PERSONA.MATERNO.Trim() : string.Empty : string.Empty) : string.Empty;
                        SelectedSignosVitales = SelectAtencionMedica.NOTA_SIGNOS_VITALES;

                    }
                    else
                    {
                        Peso = Talla = Glucemia = TensionArterial = TextFrecuenciaCardiaca = FrecuenciaRespira = Temperatura = ObservacionesSignosVitales = string.Empty;
                        SelectedSignosVitales = null;
                        SignosVitalesExpandidos = true;
                        SignosVitalesReadOnly = false;
                    }
                }
                else
                {
                    Peso = Talla = Glucemia = TensionArterial = TextFrecuenciaCardiaca = FrecuenciaRespira = Temperatura = ObservacionesSignosVitales = string.Empty;
                    SelectedSignosVitales = null;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() =>
                    new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag/*, Parametro.ESTATUS_ADMINISTRATIVO_INACT, GlobalVar.gCentro*/));

                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }

            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new List<IMPUTADO>();
            }
        }

        private void HideNotas()
        {
            UrgenciaVisible = Visibility.Collapsed;
            EvolucionVisible = Visibility.Collapsed;
            TrasladoVisible = Visibility.Collapsed;
            InterconsultaVisible = Visibility.Collapsed;
            PreOpVisible = Visibility.Collapsed;
            PreAnestVisible = Visibility.Collapsed;
            PostOpVisible = Visibility.Collapsed;
        }

        private void SeleccionarImputado()
        {
            try
            {
                SelectedIngreso = SelectIngreso;
                IsPlanimetriaValidada = true;
                IsBusquedaSimpleValidada = true;
                ProgesoActual++;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ProgresoValidaciones = string.Concat("Validada la búsqueda por gafete ", ProcesaProgreso());
                }));
                BHuellasEnabled = true;
                CamposBusquedaEnabled = false;
                SeleccionaIngreso();
                SelectConsultaMedica = null;
                EmptyBuscarConsultasMedicasVisible = false;
                TextBuscarConsultaMedica = string.Empty;
                StaticSourcesViewModel.SourceChanged = false;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }

        private void OdontogramaClick(Object obj)
        {
            try
            {
                if (obj == null) return;
                if (!(obj is object[])) return;
                if (!(((object[])obj)[0] is CheckBox)) return;
                if (!(((object[])obj)[1] is string)) return;
                var checkbox = (CheckBox)((object[])obj)[0];
                var PosicionDiente = ((object[])obj)[1].ToString();
                ListOdontograma = ListOdontograma ?? new List<CustomOdontograma>();
                if (checkbox.IsChecked.HasValue ? checkbox.IsChecked.Value : false)
                    ListOdontograma.Add(new CustomOdontograma
                    {
                        ID_DIENTE = short.Parse(PosicionDiente.Split('_')[0]),
                        ID_POSICION = short.Parse(PosicionDiente.Split('_')[1]),
                    });
                else
                    if (ListOdontograma.Any(f => f.ID_POSICION == short.Parse(PosicionDiente.Split('_')[1]) && f.ID_DIENTE == short.Parse(PosicionDiente.Split('_')[0])))
                        ListOdontograma.Remove(ListOdontograma.First(f => f.ID_POSICION == short.Parse(PosicionDiente.Split('_')[1]) && f.ID_DIENTE == short.Parse(PosicionDiente.Split('_')[0])));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la región del cuerpo.", ex);
            }
        }

        #region ARCHIVOS MEDICOS

        private void DescargaArchivo(CustomGridSinBytes Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                var _detallesArchivo = new SSP.Controlador.Catalogo.Justicia.cServicioAuxiliarResultado().GetData(z => z.ID_SA_RESULTADO == SelectedResultadoSinArchivo.IdResult).FirstOrDefault();
                if (_detallesArchivo == null)
                    return;

                if (_detallesArchivo.ID_FORMATO == null)
                    return;

                var tc = new TextControlView();

                switch (_detallesArchivo.ID_FORMATO)
                {
                    case 1: // docx
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                tc.editor.Load(_detallesArchivo.CAMPO_BLOB, TXTextControl.BinaryStreamType.WordprocessingML);
                            }
                            catch (System.Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                            }
                        };
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
                        ArchivosMedicos.Hide();
                        break;
                    case 2://xls
                        break;
                    case 3://pdf
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                tc.editor.Load(_detallesArchivo.CAMPO_BLOB, TXTextControl.BinaryStreamType.AdobePDF);
                            }
                            catch (System.Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                            }
                        };
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
                        ArchivosMedicos.Hide();
                        break;
                    case 4://doc
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                tc.editor.Load(_detallesArchivo.CAMPO_BLOB, TXTextControl.BinaryStreamType.MSWord);
                            }
                            catch (System.Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                            }
                        };

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
                        ArchivosMedicos.Hide();
                        break;
                    case 5://jpeg
                        break;
                    case 6://xlsx
                        break;
                    case 15:
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                System.IO.MemoryStream ms = new System.IO.MemoryStream(_detallesArchivo.CAMPO_BLOB);
                                System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
                                TXTextControl.Image _imagen = new TXTextControl.Image(returnImage);
                                tc.editor.Images.Add(_imagen, TXTextControl.HorizontalAlignment.Center, 0, TXTextControl.ImageInsertionMode.FixedOnPage);
                            }
                            catch (System.Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                            }
                        };
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        break;

                    default:
                        break;
                }
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", exc);
            }
        }

        private void CargarSubTipo_Servicios_Auxiliares(short? tipo_serv_aux)
        {
            try
            {
                LstSubTipoServAux = new ObservableCollection<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(tipo_serv_aux, "S"));
                LstSubTipoServAux.Insert(0, new SUBTIPO_SERVICIO_AUX_DIAG_TRAT { ID_SUBTIPO_SADT = -1, DESCR = "TODOS" });
                LstDiagnosticosPrincipal = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>();
                LstDiagnosticosPrincipal.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "TODOS", ID_SERV_AUX = -1 });
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", ex);
            }
        }

        private void CargarDiagnosticosPrincipal(short? subTipo)
        {
            try
            {
                LstDiagnosticosPrincipal = new ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(new cServ_Aux_Diag_Trat().ObtenerTodos(SelectedTipoServAux, subTipo, "S").Select(s => new EXT_SERV_AUX_DIAGNOSTICO
                {
                    DESCR = s.DESCR,
                    ESTATUS = s.ESTATUS,
                    ID_SERV_AUX = s.ID_SERV_AUX,
                    ID_SUBTIPO_SADT = s.ID_SUBTIPO_SADT,
                    SUBTIPO_DESCR = s.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR
                }));
                LstDiagnosticosPrincipal.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "TODOS", ID_SERV_AUX = -1 });
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", ex);
            }
        }

        private void BuscarResultadosExistentes()
        {
            try
            {
                if (SelectIngreso == null) return;
                if (!FechaInicioBusquedaResultServ.HasValue)
                    FechaInicioBusquedaResultServ = Fechas.GetFechaDateServer;
                if (!FechaFinBusquedaResultServ.HasValue)
                    FechaFinBusquedaResultServ = Fechas.GetFechaDateServer;
                LstCustomizadaSinArchivos = new ObservableCollection<CustomGridSinBytes>();
                var _datos = new cServicioAuxiliarResultado().BuscarResultadosNotaMedica(new DateTime(FechaInicioBusquedaResultServ.Value.Year, FechaInicioBusquedaResultServ.Value.Month, FechaInicioBusquedaResultServ.Value.Day),
                    new DateTime(FechaFinBusquedaResultServ.Value.Year, FechaFinBusquedaResultServ.Value.Month, FechaFinBusquedaResultServ.Value.Day),
                    SelectedTipoServAux, SelectedSubtipoServAux, SelectedDiagnPrincipal, SelectIngreso, SelectedIngresoBusquedas).OrderByDescending(x => x.REGISTRO_FEC);
                if (_datos.Any())
                {
                    foreach (var item in _datos)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            LstCustomizadaSinArchivos.Add(new CustomGridSinBytes
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
                            });
                        }));
                    };
                }
                EmptyResultados = LstCustomizadaSinArchivos.Count <= 0;
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", exc);
            }
        }

        private void ArchivosMedicosClosed(object sender, EventArgs e)
        {
            try
            {
                ArchivosMedicos.Closed -= ArchivosMedicosClosed;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la ventana de búsqueda.", ex);
            }
        }

        private void TextControlClosed(object sender, EventArgs e)
        {
            try
            {
                if (ArchivosMedicos != null)
                    ArchivosMedicos.Show();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la ventana de búsqueda.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.NOTA_MEDICA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
                    ListEnfermedades = ListEnfermedades ?? new ObservableCollection<ENFERMEDAD>();
                    if (!ListEnfermedades.Any(a => a.ID_ENFERMEDAD == ((ENFERMEDAD)item).ID_ENFERMEDAD))
                    {
                        ListEnfermedades.Insert(0, (ENFERMEDAD)item);
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
                        ListEnfermedades = ListEnfermedades ?? new ObservableCollection<ENFERMEDAD>();
                        if (!ListEnfermedades.Any(a => a.ID_ENFERMEDAD == ((ENFERMEDAD)item).ID_ENFERMEDAD))
                        {
                            ListEnfermedades.Insert(0, (ENFERMEDAD)item);
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
                        ListRecetas.Add(new RecetaMedica
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
                            ListRecetas.Insert(0, (RecetaMedica)item);
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

        #region CERTIFICADO
        private void SeniasFrenteLoad(SeniasFrenteView Window = null)
        {
            try
            {
                if (Window == null) return;
                if (ListRadioButonsFrente != null ? ListRadioButonsFrente.Any() : false) return;
                ListRadioButonsFrente = new List<RadioButton>();
                ListRadioButonsFrente = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridFrente"))).ToList();
                if (SelectLesion != null)
                    foreach (var item in ListRadioButonsFrente)
                        item.IsChecked = item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar datos iniciales de las lesiones.", ex);
            }
        }

        private void SeniasDorsoLoad(SeniasDorsoView Window = null)
        {
            try
            {
                if (Window == null) return;
                if (ListRadioButonsDorso != null ? ListRadioButonsDorso.Any() : false) return;
                ListRadioButonsDorso = new List<RadioButton>();
                ListRadioButonsDorso = new ControlPenales.Clases.FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridDorso"))).ToList();
                if (SelectLesion != null)
                    foreach (var item in ListRadioButonsDorso)
                        item.IsChecked = item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar datos iniciales de las lesiones.", ex);
            }
        }

        private void RegionSwitch(Object obj)
        {
            try
            {
                SelectRegion = obj.ToString().Split('-').Any() ? short.Parse(obj.ToString().Split('-')[0]) : new Nullable<short>();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la región del cuerpo.", ex);
            }
        }

        private void LesionSelected(Object obj)
        {
            try
            {
                //if (!(obj is DataGrid)) return;
                //if (!(((DataGrid)obj).SelectedItem is LesionesCustom)) return;
                //SelectLesion = (LesionesCustom)((DataGrid)obj).SelectedItem;
                //TextDescripcionLesion = SelectLesion.DESCR;
                //SelectRegion = SelectLesion.REGION.ID_REGION;
                //if (SelectLesion.REGION.LADO == "D")
                //{
                //    TabDorso = true; 
                //    foreach (var item in ListRadioButonsDorso)
                //        if (item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false)
                //        {
                //            item.IsChecked = true;
                //            return;
                //        } 
                //}
                //if (SelectLesion.REGION.LADO == "F")
                //{
                //    TabFrente = true;
                //    foreach (var item in ListRadioButonsFrente)
                //        if (item.CommandParameter != null ? item.CommandParameter.ToString().Split('-')[0] == SelectLesion.REGION.ID_REGION.ToString() : false)
                //        {
                //            item.IsChecked = true;
                //            return; 
                //        }
                //}
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la lesion.", ex);
            }
        }

        private void GetDatosImputadoSeleccionado()
        {
            TextAnioImputado = SelectExpediente.ID_ANIO;
            TextFolioImputado = SelectExpediente.ID_IMPUTADO;
            TextPaternoImputado = SelectExpediente.PATERNO.Trim();
            TextMaternoImputado = SelectExpediente.MATERNO.Trim();
            TextNombreImputado = SelectExpediente.NOMBRE.Trim();
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
                //BuscarPor = enumTipoPersona.IMPUTADO;
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
                        SeleccionarImputado();
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
                        await ComparePersona();
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
                            //HuellaWindow = null;
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
                        ScannerMessage = "Huella no concuerda";
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        AceptarBusquedaHuellaFocus = true;
                        if (!CancelKeepSearching)
                        {
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
                    ID_TIPO_PERSONA = BuscarPor != enumTipoPersona.PERSONA_EMPLEADO ? new Nullable<enumTipoPersona>() : BuscarPor
                });
                if (CompareResult.Identify)
                {
                    ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                    var aux = ListResultado = new List<ResultadoBusquedaBiometrico>();
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

                            if (persona.PERSONA.EMPLEADO != null ?
                                persona.PERSONA.EMPLEADO.USUARIO != null ?
                                    persona.PERSONA.EMPLEADO.USUARIO.Any(a => a.ID_USUARIO.Trim() == GlobalVar.gUsr ?
                                        a.USUARIO_ROL.Any(an => an.ID_ROL == (short)enumRolesAreasTecnicas.MEDICO || an.ID_ROL == (short)enumRolesCoordinadoresAreasTecnicas.COORDINADOR_MEDICO)
                                    : false)
                                : false
                            : false)
                                aux.Add(new ResultadoBusquedaBiometrico()
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
                    ListResultado = new List<ResultadoBusquedaBiometrico>(aux);
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
                //BuscarPor = tipobusqueda;
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

        #region PROCEDIMIENTOS MEDICOS

        private void CancelarProcMedClosed(object sender, EventArgs e)
        {
            try
            {
                CancelarProcMedWindow.Closed -= ArchivosMedicosClosed;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la ventana de búsqueda.", ex);
            }
        }

        private async void Producto_ProcedimientoMedico_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AutoCompleteProcsMedsLB = AutoCompleteProcsMeds.Template.FindName("PART_ListBox", AutoCompleteProcsMeds) as ListBox;
                var dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is ListBoxItem))
                    dep = VisualTreeHelper.GetParent(dep);
                if (dep == null) return;
                var item = AutoCompleteProcsMedsLB.ItemContainerGenerator.ItemFromContainer(dep);
                if (item == null) return;
                if (item is RecetaMedica)
                {
                    if (SelectProcMedEnCitaParaAgendar == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "Debes seleccionar un procedimiento médico para agregar un insumo a la lista.");
                        return;
                    }
                    if (SelectProcMedEnCitaParaAgendar.ID_PROCMED <= 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "Debes seleccionar un procedimiento médico válido para agregar un insumo a la lista.");
                        return;
                    }
                    ListProcedimientosMedicosEnCita = ListProcedimientosMedicosEnCita ?? new ObservableCollection<CustomProcedimientosMedicosCitados>();
                    var prod = ((RecetaMedica)item).PRODUCTO;
                    if (ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED != null ? a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED : false))
                    {
                        if (ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED ?
                            a.PROCEDIMIENTOS_MATERIALES.Any(an => an.PRODUCTO.ID_PRODUCTO == ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO)
                        : false))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Ya existe ese insumo con el procedimiento médico seleccionado.");
                            return;
                        }

                        #region Agregar Producto a Procedimiento ya agregado
                        if (await new Dialogos().ConfirmarEliminar("Confirmación!", "Esta seguro que desea agregar " + prod.NOMBRE + " con el procedimiento " + SelectProcMedEnCitaParaAgendar.DESCR + "?") == 1)
                        {
                            var ListaAuxiliar = ListProcedimientosMedicosEnCita.ToList();
                            ListProcedimientosMedicosEnCita = new ObservableCollection<CustomProcedimientosMedicosCitados>();
                            foreach (var a in ListaAuxiliar)
                                if (a.PROC_MED != null ? a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED : false)
                                {
                                    if (a.PROC_MED.PROC_MATERIAL.Any())
                                        a.PROC_MED.PROC_MATERIAL.Add(new PROC_MATERIAL
                                        {
                                            ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                            ESTATUS = "S",
                                            ID_PRODUCTO = ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO,
                                            PROC_MED = SelectProcMedEnCitaParaAgendar,
                                        });
                                    else
                                        a.PROC_MED.PROC_MATERIAL = new List<PROC_MATERIAL> 
                                        { 
                                            new PROC_MATERIAL
                                            {
                                                ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                                ESTATUS = "S",
                                                ID_PRODUCTO = ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO,
                                                PROC_MED = SelectProcMedEnCitaParaAgendar,
                                            }
                                        };
                                    a.PROCEDIMIENTOS_MATERIALES.Add(new CustomProcedimientosMaterialesCitados
                                    {
                                        CANTIDAD = new Nullable<int>(),
                                        PRODUCTO = prod,
                                        PROC_MATERIAL = new PROC_MATERIAL
                                        {
                                            ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                            ESTATUS = "S",
                                            ID_PRODUCTO = ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO,
                                            PROC_MED = SelectProcMedEnCitaParaAgendar,
                                        },
                                    });
                                }
                            if (ListaAuxiliar.Any(a => a.PROC_MED != null))
                                ListProcedimientosMedicosEnCita = new ObservableCollection<CustomProcedimientosMedicosCitados>(ListaAuxiliar.OrderBy(o => o.PROC_MED.DESCR));
                        }
                        #endregion

                    }
                    else
                    {
                        #region Agregar Producto a Procedimiento ya agregado
                        if (await new Dialogos().ConfirmarEliminar("Confirmación!", "Esta seguro que desea agregar el procedimiento " + SelectProcMedEnCitaParaAgendar.DESCR + " con sus materiales?") == 1)
                        {
                            ListProcedimientosMedicosEnCita.Add(new CustomProcedimientosMedicosCitados
                            {
                                PROC_MED = new PROC_MED
                                {
                                    DESCR = SelectProcMedEnCitaParaAgendar.DESCR,
                                    ESTATUS = "S",
                                    ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                    ID_PROCMED_SUBTIPO = SelectProcMedEnCitaParaAgendar.ID_PROCMED_SUBTIPO,
                                    PROC_MED_SUBTIPO = SelectProcMedEnCitaParaAgendar.PROC_MED_SUBTIPO,
                                    PROC_MATERIAL = new List<PROC_MATERIAL> 
                                        { 
                                            new PROC_MATERIAL
                                            {
                                                ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                                ESTATUS = "S",
                                                ID_PRODUCTO = ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO,
                                                PROC_MED = SelectProcMedEnCitaParaAgendar,
                                            }
                                        },
                                },
                                PROCEDIMIENTOS_MATERIALES = new ObservableCollection<CustomProcedimientosMaterialesCitados> 
                                    { 
                                        new CustomProcedimientosMaterialesCitados
                                        { 
                                            CANTIDAD = new Nullable<int>(), 
                                            PRODUCTO = prod, 
                                            PROC_MATERIAL=new PROC_MATERIAL
                                            {
                                                ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                                ESTATUS = "S",
                                                ID_PRODUCTO = ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO,
                                                PROC_MED = SelectProcMedEnCitaParaAgendar,
                                            }
                                        }
                                    },
                                IsVisible = Visibility.Visible,
                            });
                            if (ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED != null))
                                ListProcedimientosMedicosEnCita = new ObservableCollection<CustomProcedimientosMedicosCitados>(ListProcedimientosMedicosEnCita.OrderBy(o => o.PROC_MED.DESCR));
                        }
                        #endregion

                    }
                    AutoCompleteProcsMeds.Text = string.Empty;
                    SelectProcMedEnCitaParaAgendar = ListProcedimientosMedicosEnCitaParaAgregar.First(f => f.ID_PROCMED == -1);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        private async void Producto_ProcedimientoMedico_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter || e.Key == Key.Return)
                {
                    var popup = AutoCompleteProcsMeds.Template.FindName("PART_Popup", AutoCompleteProcsMeds) as System.Windows.Controls.Primitives.Popup;
                    AutoCompleteProcsMedsLB = AutoCompleteProcsMeds.Template.FindName("PART_ListBox", AutoCompleteProcsMeds) as ListBox;
                    var dep = (DependencyObject)e.OriginalSource;
                    while ((dep != null) && !(dep is ListBoxItem))
                        dep = VisualTreeHelper.GetParent(dep);
                    if (dep == null) return;
                    var item = AutoCompleteProcsMedsLB.ItemContainerGenerator.ItemFromContainer(dep);
                    if (item == null) return;
                    if (item is RecetaMedica)
                    {
                        if (SelectProcMedEnCitaParaAgendar == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Debes seleccionar un procedimiento médico para agregar un insumo a la lista.");
                            return;
                        }
                        if (SelectProcMedEnCitaParaAgendar.ID_PROCMED <= 0)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Debes seleccionar un procedimiento médico válido para agregar un insumo a la lista.");
                            return;
                        }
                        ListProcedimientosMedicosEnCita = ListProcedimientosMedicosEnCita ?? new ObservableCollection<CustomProcedimientosMedicosCitados>();
                        var prod = ((RecetaMedica)item).PRODUCTO;
                        if (ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED != null ? a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED : false))
                        {
                            if (ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED ?
                                a.PROCEDIMIENTOS_MATERIALES.Any(an => an.PRODUCTO.ID_PRODUCTO == ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO)
                            : false))
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "Ya existe ese insumo con el procedimiento médico seleccionado.");
                                return;
                            }

                            #region Agregar Producto a Procedimiento ya agregado
                            if (await new Dialogos().ConfirmarEliminar("Confirmación!", "Esta seguro que desea agregar " + prod.NOMBRE + " con el procedimiento " + SelectProcMedEnCitaParaAgendar.DESCR + "?") == 1)
                            {
                                var ListaAuxiliar = ListProcedimientosMedicosEnCita.ToList();
                                ListProcedimientosMedicosEnCita = new ObservableCollection<CustomProcedimientosMedicosCitados>();
                                foreach (var a in ListaAuxiliar)
                                    if (a.PROC_MED != null ? a.PROC_MED.ID_PROCMED == SelectProcMedEnCitaParaAgendar.ID_PROCMED : false)
                                    {
                                        if (a.PROC_MED.PROC_MATERIAL.Any())
                                            a.PROC_MED.PROC_MATERIAL.Add(new PROC_MATERIAL
                                            {
                                                ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                                ESTATUS = "S",
                                                ID_PRODUCTO = ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO,
                                                PROC_MED = SelectProcMedEnCitaParaAgendar,
                                            });
                                        else
                                            a.PROC_MED.PROC_MATERIAL = new List<PROC_MATERIAL> 
                                        { 
                                            new PROC_MATERIAL
                                            {
                                                ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                                ESTATUS = "S",
                                                ID_PRODUCTO = ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO,
                                                PROC_MED = SelectProcMedEnCitaParaAgendar,
                                            }
                                        };
                                        a.PROCEDIMIENTOS_MATERIALES.Add(new CustomProcedimientosMaterialesCitados
                                        {
                                            CANTIDAD = new Nullable<int>(),
                                            PRODUCTO = prod,
                                            PROC_MATERIAL = new PROC_MATERIAL
                                            {
                                                ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                                ESTATUS = "S",
                                                ID_PRODUCTO = ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO,
                                                PROC_MED = SelectProcMedEnCitaParaAgendar,
                                            },
                                        });
                                    }
                                if (ListaAuxiliar.Any(a => a.PROC_MED != null))
                                    ListProcedimientosMedicosEnCita = new ObservableCollection<CustomProcedimientosMedicosCitados>(ListaAuxiliar.OrderBy(o => o.PROC_MED.DESCR));
                            }
                            #endregion

                        }
                        else
                        {
                            #region Agregar Producto a Procedimiento ya agregado
                            if (await new Dialogos().ConfirmarEliminar("Confirmación!", "Esta seguro que desea agregar el procedimiento " + SelectProcMedEnCitaParaAgendar.DESCR + " con sus materiales?") == 1)
                            {
                                ListProcedimientosMedicosEnCita.Add(new CustomProcedimientosMedicosCitados
                                {
                                    PROC_MED = new PROC_MED
                                    {
                                        DESCR = SelectProcMedEnCitaParaAgendar.DESCR,
                                        ESTATUS = "S",
                                        ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                        ID_PROCMED_SUBTIPO = SelectProcMedEnCitaParaAgendar.ID_PROCMED_SUBTIPO,
                                        PROC_MED_SUBTIPO = SelectProcMedEnCitaParaAgendar.PROC_MED_SUBTIPO,
                                        PROC_MATERIAL = new List<PROC_MATERIAL> 
                                        { 
                                            new PROC_MATERIAL
                                            {
                                                ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                                ESTATUS = "S",
                                                ID_PRODUCTO = ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO,
                                                PROC_MED = SelectProcMedEnCitaParaAgendar,
                                            }
                                        },
                                    },
                                    PROCEDIMIENTOS_MATERIALES = new ObservableCollection<CustomProcedimientosMaterialesCitados> 
                                    { 
                                        new CustomProcedimientosMaterialesCitados
                                        { 
                                            CANTIDAD = new Nullable<int>(), 
                                            PRODUCTO = prod, 
                                            PROC_MATERIAL=new PROC_MATERIAL
                                            {
                                                ID_PROCMED = SelectProcMedEnCitaParaAgendar.ID_PROCMED,
                                                ESTATUS = "S",
                                                ID_PRODUCTO = ((RecetaMedica)item).PRODUCTO.ID_PRODUCTO,
                                                PROC_MED = SelectProcMedEnCitaParaAgendar,
                                            }
                                        }
                                    },
                                    IsVisible = Visibility.Visible,
                                });
                                if (ListProcedimientosMedicosEnCita.Any(a => a.PROC_MED != null))
                                    ListProcedimientosMedicosEnCita = new ObservableCollection<CustomProcedimientosMedicosCitados>(ListProcedimientosMedicosEnCita.OrderBy(o => o.PROC_MED.DESCR));
                            }
                            #endregion
                        }
                        AutoCompleteProcsMeds.Text = string.Empty;
                        SelectProcMedEnCitaParaAgendar = ListProcedimientosMedicosEnCitaParaAgregar.First(f => f.ID_PROCMED == -1);
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

        #region Cambio SelectedItem de Busqueda de Expediente
        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_expediente":
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            //MUESTRA LOS INGRESOS
                            if (SelectExpediente.INGRESO != null && SelectExpediente.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            }
                            else
                                EmptyIngresoVisible = true;

                            //OBTENEMOS FOTO DE FRENTE
                            if (SelectIngreso != null)
                            {
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    ImagenImputado = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenImputado = new Imagenes().getImagenPerson();
                        }
                        break;
                }
            }
        }
        #endregion

    }
}