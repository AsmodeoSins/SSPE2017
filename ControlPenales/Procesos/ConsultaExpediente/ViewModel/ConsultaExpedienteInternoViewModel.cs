using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFPdfViewer;
using SSP.Controlador.Catalogo.Justicia.EstudioMI;
using SSP.Controlador.Catalogo.Justicia.Medico.CertificadoMedico;
using SSP.Controlador.Catalogo.Justicia.Medico;
using Microsoft.Reporting.WinForms;
using Novacode;
namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel : FingerPrintScanner
    {
        public ConsultaExpedienteInternoViewModel() { }
        private WPFPdfViewer.PdfViewer PDFViewer;
        private WPFPdfViewer.PdfViewer PDFVIewer2;
        private WPFPdfViewer.PdfViewer PDFVIewer3;
        private void Load_Window(ConsultaExpedienteInternoView Window)
        {
            try
            {
                StaticSourcesViewModel.SourceChanged = false;
                PDFViewer = PopUpsViewModels.MainWindow.MostrarPDFView.pdfViewer;
                PDFVIewer2 = PopUpsViewModels.MainWindow.MostrarPDFView.pdfViewer;
                PDFVIewer3 = PopUpsViewModels.MainWindow.MostrarPDFView.pdfViewer;
                ReporteHCM = Window.ReportHCM;
                ReporteHCD = Window.ReportHCD;
                ReporteBH = Window.ReportBH;
                ReporteKdx = Window.ReportK;

                ListTipoAtencion = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                ListTipoServicioAux = new ObservableCollection<ATENCION_SERVICIO>(new cAtencionServicio().ObtenerTodo().Where(w => w.ESTATUS == "S" && w.ID_TIPO_SERVICIO != (short)enumAtencionServicio.HISTORIA_CLINICA_MEDICA && w.ID_TIPO_SERVICIO != (short)enumAtencionServicio.TRASLADO_VIRTUAL));
                ListTipoAtencion.Insert(0, new ATENCION_TIPO()
                {
                    DESCR = "SELECCIONE",
                    ID_TIPO_ATENCION = -1,
                });

                CargarTipo_Servicios_Auxiliares();
                SelectTipoAtencion = ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == -1);

                LstDepartamentos = new ObservableCollection<DEPARTAMENTO>();
                LstDepartamentos = new ObservableCollection<DEPARTAMENTO>(new cDepartamentos().ObtenerTodos(string.Empty, "S"));
                LstDepartamentos.Insert(0, new DEPARTAMENTO() { DESCR = "SELECCIONE", ID_DEPARTAMENTO = -1 });

                ListTurnosHCL = new ObservableCollection<LIQUIDO_TURNO>();
                ListTurnosHCL = new ObservableCollection<LIQUIDO_TURNO>(new cLiquidoTurno().GetData(x => x.ESTATUS == "S").OrderBy(y => y.DESCR));
                ListTurnosHCL.Insert(0, new LIQUIDO_TURNO() { DESCR = "SELECCIONE", ID_LIQTURNO = -1 });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                #region MENU
                case "limpiar_campos":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ConsultaExpedienteInternoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ConsultaExpedienteInternoViewModel();
                    break;
                case "salida_ingreso":
                    StaticSourcesViewModel.SourceChanged = false;
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_ingreso":
                    AnioBuscar = new int?();
                    FolioBuscar = new int?();
                    NombreBuscar = string.Empty;
                    ApellidoMaternoBuscar = string.Empty;
                    ApellidoPaternoBuscar = string.Empty;
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    EmptyExpedienteVisible = true;
                    SelectExpediente = null;
                    SelectIngreso = null;
                    if (ExpedienteFisicoVisible == Visibility.Visible ? pdfViewer != null : false)
                        pdfViewer.Visibility = Visibility.Hidden;

                    if (VisibleDatosMedicos == Visibility.Visible ? ReporteBH != null : false)
                        VisibleReporteBitacoraHospitalizaciones = Visibility.Collapsed;

                    if (VisibleDatosKardex == Visibility.Visible ? ReporteKdx != null : false)
                        VisibleDatosKardexReporte = Visibility.Collapsed;

                    if (VisibleDatosMedicos == Visibility.Visible)
                    {
                        //validacion con respecto a la historia clinica seleccionada
                        if (IndiceHistoriasClinicas == 0)
                        {
                            if (TieneHistoriaClinicaMedica)
                                VisibleReporteHistoriaClinicaMedica = Visibility.Collapsed;
                        }
                        else
                        {
                            if (IndiceHistoriasClinicas == 1)
                                if (TieneHistoriaClinicaDental)
                                    VisibleReporteHistoriaClinicaDental = Visibility.Collapsed;
                        }
                    };
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA_EXPEDIENTE);
                    break;
                #endregion

                #region BUSCAR_IMPUTADOS
                case "nueva_busqueda":
                    NombreBuscar = string.Empty;
                    ApellidoPaternoBuscar = string.Empty;
                    ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = new int?();
                    AnioBuscar = new int?();
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>(); //new ObservableCollection<IMPUTADO>();
                    break;
                case "buscar_visible":
                    var exp = SelectExpediente;
                    var ing = SelectIngreso;
                    SelectExpedienteAuxiliar = exp;
                    SelectIngresoAuxiliar = ing;
                    if (ExpedienteFisicoVisible == Visibility.Visible ? pdfViewer != null : false)
                        pdfViewer.Visibility = Visibility.Hidden;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA_EXPEDIENTE);
                    break;
                case "buscar_salir":
                    var expA = SelectExpedienteAuxiliar;
                    var ingA = SelectIngresoAuxiliar;
                    SelectExpediente = expA;
                    SelectIngreso = ingA;
                    if (ExpedienteFisicoVisible == Visibility.Visible ? pdfViewer != null : false)
                        pdfViewer.Visibility = Visibility.Visible;

                    if (VisibleDatosMedicos == Visibility.Visible ? ReporteBH != null : false)
                        VisibleReporteBitacoraHospitalizaciones = Visibility.Visible;

                    if (VisibleDatosKardex == Visibility.Visible ? ReporteKdx != null : false)
                        VisibleDatosKardexReporte = Visibility.Visible;

                    if (VisibleDatosMedicos == Visibility.Visible)
                    {
                        //validacion con respecto a la historia clinica seleccionada
                        if (IndiceHistoriasClinicas == 0)
                        {
                            if (TieneHistoriaClinicaMedica)
                                VisibleReporteHistoriaClinicaMedica = Visibility.Visible;
                        }
                        else
                        {
                            if (IndiceHistoriasClinicas == 1)
                                if (TieneHistoriaClinicaDental)
                                    VisibleReporteHistoriaClinicaDental = Visibility.Visible;
                        }
                    };

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_EXPEDIENTE);
                    break;

                case "historico_interno":
                    HideAll();
                    VisibleDocumentosHistorico = Visibility.Visible;
                    break;
                case "buscar_seleccionar":
                    try
                    {
                        if (SelectExpediente == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un imputado.");
                            return;
                        }
                        if (SelectIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un ingreso.");
                            return;
                        }

                        ImputadoSeleccionado = SelectExpediente;

                        IngresoSeleccionado = null;

                        await StaticSourcesViewModel.CargarDatosMetodoAsync(GetDatosImputadoSeleccionado);
                        await TaskEx.Delay(150);

                        var ingreso = ImputadoSeleccionado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        ListIngresos = new ObservableCollection<INGRESO>(ImputadoSeleccionado.INGRESO.OrderByDescending(o => o.ID_INGRESO));
                        SelectedIngreso = ingreso;
                        HideAll();
                        IdentificacionVisible = Visibility.Visible;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_EXPEDIENTE);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado.", ex);
                    }
                    break;

                case "genera_hoja_liquidos":
                    if (SelectedIngreso == null)
                        return;

                    ProcesaBusquedaHojasControlLiquidos();
                    //if (SelectIngreso != null)
                    //    ImprimeReporteHojaControlLiquidos();
                    break;

                case "ver_documento_histori":
                    if (SelectedIngreso == null)
                        return;

                    DescargaArchivoHistorico(SelectedHistoricoDocumentos);
                    break;
                case "ver_hoja_liquidos":
                    if (SelectedIngreso == null)
                        return;

                    if (SeletedHojasLiquidos == null)
                        return;

                    ImprimeReporteHojaControlLiquidos();
                    break;
                case "informacionEmi":
                    if (SelectEmi != null)
                        ImprimirEMI();
                    break;
                #endregion

                #region historico
                case "buscar_historico_interno":
                    if (SelectIngreso == null)
                        return;

                    if (SelectedDepartamento == null)
                        return;

                    if (SelectedDepartamento == -1)
                        return;

                    ProcesaHistorico();
                    break;

                case "muestraDocumentoHistorico":
                    if (SelectedIngreso == null)
                        return;

                    if (SelectedHistoricoDocumentos == null)
                        return;

                    VisualizaDocumento();
                    break;

                case "ver_archivo_hcm":
                    if (SelectedIngreso == null)
                        return;

                    if (SelectArchivosHCM == null)
                        return;

                    DescargaArchivoHistoriasClinicas(SelectArchivosHCM, (short)eTiposHistoriaClinica.MEDICA);
                    break;

                case "ver_archivo_hcmd":
                    if (SelectedIngreso == null)
                        return;

                    if (SelectArchivosHCD == null)
                        return;

                    DescargaArchivoHistoriasClinicas(SelectArchivosHCD, (short)eTiposHistoriaClinica.DENTAL);

                    break;

                case "reporte_hoja_defuncion":
                    if (SelectedIngreso == null)
                        return;

                    ProcesaDefuncion();
                    break;
                #endregion
                #region kardex
                case "kardex_interno":
                    HideAll();
                    VisibleDatosKardex = Visibility.Visible;
                    break;
                case "KARDEX":
                    if (SelectIngreso == null)
                        return;

                    VisibleDatosKardexReporte = Visibility.Collapsed;
                    ImprimirKardex();
                    break;
                case "ACTIVIDADES":
                    if (SelectIngreso == null)
                        return;

                    VisibleDatosKardexReporte = Visibility.Collapsed;
                    GenerarReporte(obj.ToString());
                    break;
                case "HORARIO":
                    if (SelectIngreso == null)
                        return;

                    VisibleDatosKardexReporte = Visibility.Collapsed;
                    GenerarReporte(obj.ToString());
                    break;
                case "EMPALMES":

                    if (SelectIngreso == null)
                        return;

                    VisibleDatosKardexReporte = Visibility.Collapsed;
                    GenerarReporte(obj.ToString());
                    break;
                case "SANCIONES":
                    if (SelectIngreso == null)
                        return;

                    VisibleDatosKardexReporte = Visibility.Collapsed;
                    GenerarReporte(obj.ToString());
                    break;
                #endregion
                #region MENU_IZQUIERDA
                case "genera_hoja_enfermeria":
                    if (SelectedIngreso == null)
                        return;

                    ProcesaBusquedaHojasEnfermeria();
                    break;
                case "ver_hoja_enfermería":
                    if (SelectedIngreso == null)
                        return;

                    if (SeletedHojaEnfermeria == null)
                        return;

                    ImprimeHojaEnfermeria();
                    break;
                case "identificacion":
                    HideAll();
                    IdentificacionVisible = Visibility.Visible;
                    break;
                case "informacion_juridica":
                    HideAll();
                    JuridicoVisible = Visibility.Visible;
                    break;
                case "informacion_administrativa":
                    HideAll();
                    AdministrativoVisible = Visibility.Visible;
                    break;
                case "estudios":
                    HideAll();
                    EstudiosVisible = Visibility.Visible;
                    break;
                case "cancelar_documento":
                    SelectEstudioPersonalidad = SelectedEstudioTerminado = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VISUALIZAR_LISTA_DOCUMENTOS);
                    break;
                case "ver_documento_personalidad":
                    if (SelectEstudioPersonalidad != null && SelectedDocumento != null)
                        MuestraFormato(SelectedDocumento, SelectEstudioPersonalidad);
                    break;
                case "consultar_documentos_cero":
                    InicializaLista((short)eSituacionActual.STAGE4);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VISUALIZAR_LISTA_DOCUMENTOS);
                    break;
                case "visitas":
                    HideAll();
                    VisitasVisible = Visibility.Visible;
                    break;
                case "agenda":
                    if (IngresoSeleccionado == null)
                        return;

                    LstAgendaInterno = new RangeEnabledObservableCollection<Appointment>();
                    AgendaInternoView = new AgendaCitaInternoView();
                    AgendaInternoView.Loaded += AgendaLoaded;
                    AgendaInternoView.Owner = PopUpsViewModels.MainWindow;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    AgendaInternoView.ShowDialog();
                    AgendaInternoView.Loaded -= AgendaLoaded;
                    AgendaInternoView = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    break;
                case "expediente_fisico":
                    HideAll();
                    ExpedienteFisicoVisible = Visibility.Visible;
                    break;

                case "ver_documento_resultado_servicios":
                    if (SelectedIngreso == null)
                        return;

                    if (SeletedResultadoSinArchivo == null)
                        return;

                    if (string.IsNullOrEmpty(SeletedResultadoSinArchivo.ExtensionArchivo))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Verifique la extensión del archivo seleccionado");
                        return;
                    }

                    DescargaArchivo(SeletedResultadoSinArchivo);
                    break;

                case "cerrar_visualizador_documentos":
                    if (IndiceHistoriasClinicas == 0)
                    {
                        if (TieneHistoriaClinicaMedica)
                            VisibleReporteHistoriaClinicaMedica = Visibility.Visible;
                    }
                    else
                    {
                        if (IndiceHistoriasClinicas == 1)
                            if (TieneHistoriaClinicaDental)
                                VisibleReporteHistoriaClinicaDental = Visibility.Visible;
                    }

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                    break;

                case "expediente_medico":
                    HideAll();
                    VisibleDatosMedicos = Visibility.Visible;
                    break;

                case "ver_reporte_certificado_medico":
                    if (SelectedIngreso == null)
                        return;

                    if (SelectNotaMedica != null)
                    {
                        if (SelectNotaMedica.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO || SelectNotaMedica.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_INTEGRIDAD_FISICA ||
    SelectNotaMedica.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO || SelectNotaMedica.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTISTA_INTEGRIDAD_FISICA)
                            CargarReporteCertificado();
                        else if (SelectNotaMedica.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS)
                            CargarReporteProdecimientoMedico();
                        else
                            CargarReporteConsulta();
                    }
                    break;
                case "cargar_listado":
                    if (SelectedIngreso == null)
                        return;

                    if (SelectTipoAtencion != null ? SelectTipoAtencion.ID_TIPO_ATENCION <= 0 : true)
                        return;

                    var servicio = SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO > 0 ? SelectTipoServicio.ID_TIPO_SERVICIO : 0 : 0;
                    var query = new cAtencionMedica().GetData(g => g.ATENCION_FEC.HasValue ?
                        SelectTipoAtencion.ID_TIPO_ATENCION == g.ID_TIPO_ATENCION ?
                            (g.ID_ANIO == SelectIngreso.ID_ANIO && g.ID_CENTRO == SelectIngreso.ID_CENTRO && g.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && g.ID_INGRESO == SelectIngreso.ID_INGRESO) ?
                                servicio > 0 ?
                                    g.ID_TIPO_SERVICIO == servicio ?
                                        servicio == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS ?
                                            true
                                        : g.NOTA_MEDICA != null
                                    : false
                                : true
                            : false
                        : false
                    : false);
                    if (query != null && query.Any())
                        ListNotasMedicas = new ObservableCollection<ATENCION_MEDICA>(query.OrderBy(o => o.ATENCION_FEC).ToList().Select(s => new ATENCION_MEDICA
                        {
                            ATENCION_CITA = s.ATENCION_CITA,
                            ATENCION_CITA1 = s.ATENCION_CITA1,
                            ATENCION_FEC = s.ATENCION_FEC,
                            ATENCION_SERVICIO = s.ATENCION_SERVICIO,
                            CERTIFICADO_MEDICO = s.CERTIFICADO_MEDICO,
                            EXCARCELACION = s.EXCARCELACION,
                            EXCARCELACION1 = s.EXCARCELACION1,
                            HOSPITALIZACION = s.HOSPITALIZACION,
                            ID_ANIO = s.ID_ANIO,
                            ID_ATENCION_MEDICA = s.ID_ATENCION_MEDICA,
                            ID_CENTRO = s.ID_CENTRO,
                            ID_CENTRO_UBI = s.ID_CENTRO_UBI,
                            ID_CITA_SEGUIMIENTO = s.ID_CITA_SEGUIMIENTO,
                            ID_HOSPITA = s.ID_HOSPITA,
                            ID_IMPUTADO = s.ID_IMPUTADO,
                            ID_INGRESO = s.ID_INGRESO,
                            ID_TIPO_ATENCION = s.ID_TIPO_ATENCION,
                            ID_TIPO_SERVICIO = s.ID_TIPO_SERVICIO,
                            INCIDENTE = s.INCIDENTE,
                            INGRESO = s.INGRESO,
                            NOTA_INTERCONSULTA = s.NOTA_INTERCONSULTA,
                            NOTA_MEDICA = s.NOTA_MEDICA != null ?
                                s.NOTA_MEDICA
                            : (servicio == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS ?
                                s.ATENCION_CITA.Any() ?
                                    new NOTA_MEDICA { PERSONA = s.ATENCION_CITA.FirstOrDefault().PERSONA }
                                : null
                            : null),
                            NOTA_POST_OPERATOR = s.NOTA_POST_OPERATOR,
                            NOTA_PRE_ANESTECIC = s.NOTA_PRE_ANESTECIC,
                            NOTA_PRE_OPERATORI = s.NOTA_PRE_OPERATORI,
                            NOTA_REFERENCIA_TR = s.NOTA_REFERENCIA_TR,
                            NOTA_SIGNOS_VITALES = s.NOTA_SIGNOS_VITALES,
                            NOTA_URGENCIA = s.NOTA_URGENCIA,
                            PROC_ATENCION_MEDICA = s.PROC_ATENCION_MEDICA,
                            RECETA_MEDICA = s.RECETA_MEDICA,
                            TRASLADO_DETALLE = s.TRASLADO_DETALLE,
                        }));
                    else
                        ListNotasMedicas = new ObservableCollection<ATENCION_MEDICA>();

                    EmptyVisible = ListNotasMedicas != null ? ListNotasMedicas.Any() ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
                    break;

                case "buscar_result_existentes":
                    if (SelectIngreso != null)
                        BuscarResultadosExistentes();
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un ingreso valido.");

                    break;

                case "genera_bitacora_hospitalizacion":
                    if (SelectedIngreso == null)
                        return;

                    if (GeneraReporteDatos())
                        ReportViewer_Requisicion();
                    break;
                #endregion

                #region SENAS PARTICULARES
                case "seleccionar_sena_particular":
                    try
                    {
                        if (SelectedIngreso == null)
                            return;

                        if (SelectSenaParticular != null)
                        {
                            if (SelectSenaParticular.CODIGO == null)
                                return;

                            SeniasParticularesEditable = true;
                            RegionCodigo = SelectSenaParticular.CODIGO != null ? SelectSenaParticular.CODIGO.ToCharArray() : new char[0];
                            SelectAnatomiaTopografica = SelectSenaParticular.ANATOMIA_TOPOGRAFICA == null ? SelectSenaParticular.ANATOMIA_TOPOGRAFICA :
                                new cAnatomiaTopografica().Obtener((int)SelectSenaParticular.ID_REGION);
                            if (regionCodigo[4].ToString() == "F")
                                TabFrente = true;
                            else if (regionCodigo[4].ToString() == "D")
                                TabDorso = true;
                            foreach (var item in ListRadioButons)
                            {
                                if (item.CommandParameter != null)
                                    if (item.CommandParameter.ToString().Contains(SelectSenaParticular.ID_REGION.ToString() + "-" + RegionCodigo[1] + RegionCodigo[2] + RegionCodigo[3]))
                                        item.IsChecked = true;
                                    else
                                        item.IsChecked = false;
                            }
                            ImagenTatuaje = SelectSenaParticular.IMAGEN != null ? new Imagenes().ConvertByteToBitmap(SelectSenaParticular.IMAGEN) : null;
                            TextCantidad = SelectSenaParticular.CANTIDAD.HasValue ? SelectSenaParticular.CANTIDAD.Value.ToString() : string.Empty;
                            if (SelectSenaParticular.ID_TIPO_TATUAJE.HasValue)
                            {
                                ListTipoTatuaje = new ObservableCollection<TATUAJE>();
                                ListTipoTatuaje.Add(new TATUAJE
                                {
                                    DESCR = SelectSenaParticular.TATUAJE.DESCR,
                                    ID_TATUAJE = SelectSenaParticular.ID_TIPO_TATUAJE.Value,
                                });
                                SelectTatuaje = ListTipoTatuaje.First(w => w.ID_TATUAJE == SelectSenaParticular.ID_TIPO_TATUAJE);
                            }
                            if (SelectSenaParticular.CLASIFICACION != null)
                            {
                                ListClasificacionTatuaje = new ObservableCollection<TATUAJE_CLASIFICACION>();
                                ListClasificacionTatuaje.Add(new TATUAJE_CLASIFICACION
                                {
                                    DESCR = SelectSenaParticular.TATUAJE_CLASIFICACION.DESCR,
                                    ID_TATUAJE_CLA = SelectSenaParticular.CLASIFICACION,
                                });
                                SelectClasificacionTatuaje = ListClasificacionTatuaje.First(w => w.ID_TATUAJE_CLA == SelectSenaParticular.CLASIFICACION);
                            }
                            SelectTipoSenia = SelectSenaParticular.TIPO.HasValue ? (int)SelectSenaParticular.TIPO.Value : 0;
                            SelectPresentaIntramuros = SelectSenaParticular.INTRAMUROS == "S";
                            SelectPresentaIngresar = SelectSenaParticular.INTRAMUROS != "S";
                            SelectTipoCicatriz = SelectSenaParticular.TIPO == 1;
                            SelectTipoTatuaje = SelectSenaParticular.TIPO == 2;
                            SelectTipoLunar = SelectSenaParticular.TIPO == 3;
                            SelectTipoDefecto = SelectSenaParticular.TIPO == 4;
                            SelectTipoProtesis = SelectSenaParticular.TIPO == 5;
                            SelectTipoAmputacion = SelectSenaParticular.TIPO == 6;
                            TextSignificado = SelectSenaParticular.SIGNIFICADO;
                            CodigoSenia = SelectSenaParticular.CODIGO;
                            RegionValorCodigo = CodigoSenia[1].ToString() + CodigoSenia[2].ToString() + CodigoSenia[3].ToString() + "";
                            TextAmpliarDescripcion = string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la seña particular seleccionada.", ex);
                    }
                    break;
                #endregion

                #region SOCIOECONOMICO
                case "reporte_socioeconomico":
                    if (SelectIngreso != null)
                        ImprimirEstudio();
                    break;
                #endregion
            }
        }


        private async void DescargaArchivoHistoriasClinicas(CustomArchivosSimple Entity, short TipoHC)
        {
            try
            {
                if (Entity == null)
                    return;

                if (TipoHC == (short)eTiposHistoriaClinica.MEDICA)//HISTORIA CLINICA MEDICA
                {
                    var _detallesHC = new cHistoriaClinica().GetData(x => x.ID_INGRESO == SelectedIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectedIngreso.ID_IMPUTADO && x.ID_ANIO == SelectedIngreso.ID_ANIO).FirstOrDefault();

                    var _detallesArchivo = new cHistoriaClinicaDocumento().GetData(x => x.ID_INGRESO == SelectedIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectedIngreso.ID_IMPUTADO && x.ID_ANIO == SelectedIngreso.ID_ANIO && x.ID_CONSEC == _detallesHC.ID_CONSEC && x.ID_NODOCTO == Entity.Consecutivo).FirstOrDefault();
                    if (_detallesArchivo == null)
                        return;

                    if (_detallesArchivo.ID_FORMATO == null)
                    {
                        if (TieneHistoriaClinicaMedica)
                            VisibleReporteHistoriaClinicaMedica = Visibility.Visible;
                    }

                    VisibleReporteHistoriaClinicaMedica = Visibility.Collapsed;

                    var tc = new TextControlView();

                    switch (_detallesArchivo.ID_FORMATO)
                    {
                        case 1: // docx
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc.editor.Loaded += (s, e) =>
                            {
                                try
                                {
                                    tc.editor.Load(_detallesArchivo.DOCUMENTO, TXTextControl.BinaryStreamType.WordprocessingML);
                                }
                                catch (System.Exception ex)
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                                }
                            };

                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc.Owner = PopUpsViewModels.MainWindow;
                            tc.Closed += (s, e) =>
                            {
                                VisibleReporteHistoriaClinicaMedica = Visibility.Visible;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            };

                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc.Show();
                            break;

                        case 2://xls
                            if (TieneHistoriaClinicaMedica)
                                VisibleReporteHistoriaClinicaMedica = Visibility.Visible;
                            break;

                        case 3://pdf
                            var _file = _detallesArchivo.DOCUMENTO;
                            await System.Threading.Tasks.Task.Factory.StartNew(() =>
                            {
                                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".pdf";
                                System.IO.File.WriteAllBytes(fileNamepdf, _file);
                                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    PDFViewer.LoadFile(fileNamepdf);
                                    PDFViewer.Visibility = System.Windows.Visibility.Visible;
                                }));
                            });

                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                            break;

                        case 4://doc
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc.editor.Loaded += (s, e) =>
                            {
                                try
                                {
                                    tc.editor.Load(_detallesArchivo.DOCUMENTO, TXTextControl.BinaryStreamType.MSWord);
                                }
                                catch (System.Exception ex)
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                                }
                            };

                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc.Owner = PopUpsViewModels.MainWindow;
                            tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); VisibleReporteHistoriaClinicaMedica = Visibility.Visible; };
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc.Show();
                            break;

                        case 5://jpeg
                            if (TieneHistoriaClinicaMedica)
                                VisibleReporteHistoriaClinicaMedica = Visibility.Visible;
                            break;

                        case 6://xlsx
                            if (TieneHistoriaClinicaMedica)
                                VisibleReporteHistoriaClinicaMedica = Visibility.Visible;
                            break;

                        case 15:
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc.editor.Loaded += (s, e) =>
                            {
                                try
                                {
                                    System.IO.MemoryStream ms = new System.IO.MemoryStream(_detallesArchivo.DOCUMENTO);
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
                            tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); VisibleReporteHistoriaClinicaMedica = Visibility.Visible; };
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc.Show();
                            break;

                        default:
                            if (TieneHistoriaClinicaMedica)
                                VisibleReporteHistoriaClinicaMedica = Visibility.Visible;
                            break;
                    }
                }
                else
                {
                    if (TipoHC == (short)eTiposHistoriaClinica.DENTAL)
                    {
                        var detalleDental = new cHistoriaClinicaDental().GetData(x => x.ID_INGRESO == SelectedIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectedIngreso.ID_IMPUTADO && x.ID_ANIO == SelectedIngreso.ID_ANIO).FirstOrDefault();

                        var _detallesArchivo = new cHistoriaClinicaDentalDocumento().GetData(x => x.ID_INGRESO == SelectedIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectedIngreso.ID_IMPUTADO && x.ID_ANIO == SelectedIngreso.ID_ANIO && x.ID_HCDDOCTO == Entity.ConsecutivoDental).FirstOrDefault();
                        if (_detallesArchivo == null)
                            return;

                        if (_detallesArchivo == null)
                        {
                            if (TieneHistoriaClinicaDental)
                                VisibleReporteHistoriaClinicaDental = Visibility.Visible;

                            return;
                        }

                        VisibleReporteHistoriaClinicaDental = Visibility.Collapsed;
                        var tc = new TextControlView();

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                System.IO.MemoryStream ms = new System.IO.MemoryStream(_detallesArchivo.DOCUMENTO);
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
                        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); VisibleReporteHistoriaClinicaDental = Visibility.Visible; };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
                    }
                }
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los documentos", exc);
            }
        }

        private async void DescargaArchivoHistorico(CustomGridHistoricoDocumentos Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                var _detallesArchivo = new cHistoricoDocumento().GetData(x => x.ID_ANIO == SelectedIngreso.ID_ANIO && x.ID_CENTRO == SelectedIngreso.ID_UB_CENTRO && x.ID_IMPUTADO == SelectedIngreso.ID_IMPUTADO && x.ID_HISTORICO_DOC == Entity.IdHistorico).FirstOrDefault();

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
                                tc.editor.Load(_detallesArchivo.DOCUMENTO, TXTextControl.BinaryStreamType.WordprocessingML);
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
                        tc.Show();
                        break;

                    case 2://xls
                        break;

                    case 3://pdf
                        var _file = _detallesArchivo.DOCUMENTO;
                        await System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".pdf";
                            System.IO.File.WriteAllBytes(fileNamepdf, _file);
                            System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                PDFViewer.LoadFile(fileNamepdf);
                                PDFViewer.Visibility = System.Windows.Visibility.Visible;
                            }));
                        });

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                        break;

                    case 4://doc
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                tc.editor.Load(_detallesArchivo.DOCUMENTO, TXTextControl.BinaryStreamType.MSWord);
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
                        tc.Show();
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
                                System.IO.MemoryStream ms = new System.IO.MemoryStream(_detallesArchivo.DOCUMENTO);
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
                        tc.Show();
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

        private async void VisualizaDocumento()
        {
            try
            {
                var _documentoelegido = new cHistoricoDocumento().GetData(x => x.ID_HISTORICO_DOC == SelectedHistoricoDocumentos.IdHistorico && x.ID_ANIO == SelectedIngreso.ID_ANIO && x.ID_CENTRO == SelectedIngreso.ID_UB_CENTRO && x.ID_IMPUTADO == SelectedIngreso.ID_IMPUTADO).FirstOrDefault();

                if (_documentoelegido != null)
                {
                    byte[] doc;
                    await Task.Factory.StartNew(() =>
                    {
                        var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                        //File.WriteAllBytes(_documentoelegido.DOCUMENTO, doc);
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            pdfViewer3.LoadFile(fileNamepdf);
                            pdfViewer3.Visibility = Visibility.Visible;
                        }));
                    });

                };
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void OcultaTodosReportes()
        {
            VisibleDatosKardexReporte = Visibility.Collapsed;
            VisibleReporteHistoriaClinicaMedica = Visibility.Collapsed;
        }

        private void ProcesaHistorico()
        {
            try
            {
                LstHistoricoDocumentos = new ObservableCollection<CustomGridHistoricoDocumentos>(new cHistoricoDocumento().ObtenerTodosByDepartamento(SelectIngreso.ID_ANIO, SelectIngreso.ID_UB_CENTRO, SelectIngreso.ID_IMPUTADO, SelectedDepartamento).Select(x => new CustomGridHistoricoDocumentos
                {
                    IdHistorico = x.ID_HISTORICO_DOC,
                    Descripcion = x.DESCR,
                    Fecha = x.FECHA
                    //Disponible = x.DOCUMENTO != null ? x.DOCUMENTO.Length > 0 ? "SI" : "NO" : "NO"
                }));//CARGA SIN LOS ARCHIVOS
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void ImprimirEstudio()
        {
            try
            {
                Estudio = new cEstudioSocioEconomico().Obtener(SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                #region Validacion y Obtencion de Datos
                if (SelectIngreso != null)
                {
                    if (Estudio != null)
                    {
                        var DatosReporte = new cCuerpoReporte();

                        var View = new ReportesView();
                        #region Iniciliza el entorno para mostrar el reporte al usuario
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        View.Owner = PopUpsViewModels.MainWindow;
                        View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                        View.Show();
                        #endregion

                        #region Se forma el cuerpo del reporte

                        string _NombreUsuario = string.Empty;
                        var NombreUsuario = new cUsuario().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr).FirstOrDefault();
                        if (NombreUsuario != null)
                            _NombreUsuario = NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty;

                        string _NombreCoord = string.Empty;
                        var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_TRABAJO_SOCAL).FirstOrDefault();
                        if (_UsuarioCoordinador != null)
                            _NombreCoord = _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty;

                        if (SelectIngreso.IMPUTADO != null)
                        {
                            DatosReporte.Telefono = SelectIngreso.TELEFONO.HasValue ? SelectIngreso.TELEFONO.Value.ToString() : string.Empty;
                            DatosReporte.Escolaridad = SelectIngreso.ESCOLARIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? SelectIngreso.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                            DatosReporte.Ocupacion = SelectIngreso.OCUPACION != null ? !string.IsNullOrEmpty(SelectIngreso.OCUPACION.DESCR) ? SelectIngreso.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                            DatosReporte.EdadInterno = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString();
                            DatosReporte.FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty;
                            DatosReporte.NombreInternoInicial = string.Concat("NOMBRE DEL INTERNO: ", string.Format("{0}/{1} ", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO), string.Format("{0} {1} {2}",
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty), ", UBICACIÓN ACTUAL: ", string.Format("{0}-{1}{2}-{3}",
                                                    SelectIngreso != null ? SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectIngreso != null ? SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectIngreso != null ? SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectIngreso != null ? SelectIngreso.ID_UB_CAMA.HasValue ? SelectIngreso.ID_UB_CAMA.Value.ToString() : string.Empty : string.Empty));

                            DatosReporte.NombreInterno = string.Format("{0} {1} {2}",
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);

                            DatosReporte.Domicilio = string.Format("{0} {1} {2} {3}", SelectIngreso.DOMICILIO_CALLE, SelectIngreso.DOMICILIO_NUM_EXT.HasValue ? SelectIngreso.DOMICILIO_NUM_EXT != decimal.Zero ? SelectIngreso.DOMICILIO_NUM_EXT.ToString() : string.Empty : string.Empty,
                                SelectIngreso.COLONIA != null ? !string.IsNullOrEmpty(SelectIngreso.COLONIA.DESCR) ? SelectIngreso.COLONIA.DESCR.Trim() : string.Empty : string.Empty, SelectIngreso.MUNICIPIO != null ? !string.IsNullOrEmpty(SelectIngreso.MUNICIPIO.MUNICIPIO1) ? SelectIngreso.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                        };


                        DatosReporte.NombreCentro = SelectIngreso.CENTRO != null ? !string.IsNullOrEmpty(SelectIngreso.CENTRO.DESCR) ? SelectIngreso.CENTRO.DESCR.Trim() : string.Empty : string.Empty;
                        DatosReporte.SalarioPercibia = Estudio.SALARIO.HasValue ? Estudio.SALARIO.Value.ToString("c") : string.Empty;
                        DatosReporte.Elaboro = _NombreUsuario;
                        DatosReporte.Coordinador = _NombreCoord;

                        #region Datos Grupo Primario
                        if (Estudio.SOCIOE_GPOFAMPRI != null)
                        {
                            DatosReporte.GrupoFamiliarPrimario = string.Concat("Funcional ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR) ? Estudio.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR.Equals("F") ? "X" : string.Empty : string.Empty, " )  Disfuncional ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR) ? Estudio.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR.Equals("D") ? "X" : string.Empty : string.Empty, " )");
                            DatosReporte.RelacionIntroFamiliarPrimariaGrupoPrimario = string.Concat("Adecuada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR) ? Estudio.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR.Equals("A") ? "X" : string.Empty : string.Empty, " )  Inadecuada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR) ? Estudio.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR.Equals("I") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.CuantasPersonasVivenEnHogar = Estudio.SOCIOE_GPOFAMPRI.PERSONAS_VIVEN_HOGAR.HasValue ? Estudio.SOCIOE_GPOFAMPRI.PERSONAS_VIVEN_HOGAR.Value.ToString() : string.Empty;
                            DatosReporte.CuantasPersonasLaboranGrupoPrimario = Estudio.SOCIOE_GPOFAMPRI.PERSONAS_LABORAN.HasValue ? Estudio.SOCIOE_GPOFAMPRI.PERSONAS_LABORAN.Value.ToString() : string.Empty;
                            DatosReporte.IngresoMensualFamiliarGrupoPrimario = Estudio.SOCIOE_GPOFAMPRI.INGRESO_MENSUAL.HasValue ? Estudio.SOCIOE_GPOFAMPRI.INGRESO_MENSUAL.Value.ToString("c") : string.Empty;
                            DatosReporte.EgresoMensualFamiliarGrupoPrimario = Estudio.SOCIOE_GPOFAMPRI.EGRESO_MENSUAL.HasValue ? Estudio.SOCIOE_GPOFAMPRI.EGRESO_MENSUAL.Value.ToString("c") : string.Empty;
                            DatosReporte.IntegranteAntecedentesAdiccionGrupoPrimario = string.Concat("SI ( ", Estudio.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE.HasValue ? Estudio.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE.Value == (decimal)eProcedimiento.SI ? "X" : string.Empty : string.Empty, " )  NO ( ", Estudio.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE.HasValue ? Estudio.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE.Value == (decimal)eProcedimiento.NO ? "X" : string.Empty : string.Empty, " )");
                            DatosReporte.EspecifiqueAntecedentesAdiccionGrupoPrimario = Estudio.SOCIOE_GPOFAMPRI.ANTECEDENTE;
                            DatosReporte.ZonaUbicacionViviendaGrupoPrimario = string.Concat("Urbana ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA.Equals("U") ? "X" : string.Empty : string.Empty, " )  Semi-Urbana ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA.Equals("S") ? "X" : string.Empty : string.Empty, " )  Rural ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA.Equals("R") ? "X" : string.Empty : string.Empty, " )");
                            DatosReporte.CondicionesViviendaGrupoPrimario = string.Concat("Propia ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES.Equals("P") ? "X" : string.Empty : string.Empty, " )  Rentada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES.Equals("R") ? "X" : string.Empty : string.Empty, " )  Otro ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES.Equals("O") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.NivelSocioEconomicoCulturalGrupoPrimario = string.Concat(" Alto ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL.Equals("A") ? "X" : string.Empty : string.Empty, " )  Medio ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL.Equals("M") ? "X" : string.Empty : string.Empty, " )  Bajo ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL.Equals("B") ? "X" : string.Empty : string.Empty, " ) ");

                            if (Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Any())
                            {
                                var MaterialesPrimarios = Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Where(w => w.ID_TIPO == "M" && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);//Lista de los materiales
                                DatosReporte.ViviendaMaterialesPrimario = string.Concat("Cartón ( ", MaterialesPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("CARTON")) ? "X" : string.Empty,
                                    MaterialesPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ADOBE")) ? "X" : string.Empty, " ) Ladrillo ( ", MaterialesPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("LADRILLO")) ? "X" : string.Empty,
                                    " ) Madera ( ", MaterialesPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("MADERA")) ? "X" : string.Empty,
                                    " ) Block ( ", MaterialesPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("BLOCK")) ? "X" : string.Empty, " ) Otro ( ", MaterialesPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("OTRO")) ? "X" : string.Empty, " ) ");

                                var DistribucionPrimaria = Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Where(x => x.ID_TIPO == "D" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string DristriGrupoPrimario = string.Concat("Sala ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("SALA")) ? "X" : string.Empty,
                                    " )  Cocina ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("COCINA")) ? "X" : string.Empty, " ) Comedor ( ",
                                DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("COMEDOR")) ? "X" : string.Empty, " ) Recamara ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("RECAMARA")) ? "X" : string.Empty,
                                " ) Baño ( ", DistribucionPrimaria.Any(X => X.SOCIOECONOMICO_CAT.DESCR.Contains("BAÑO")) ? "X" : string.Empty, " ) Otros ( ",
                                DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("OTROS")) ? "X" : string.Empty, " ) ");
                                DatosReporte.DistVivivendaPrimario = DristriGrupoPrimario;


                                var ServiciosPrimarios = Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Where(x => x.ID_TIPO == "S" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string ServiciosP = string.Concat("Energía  Eléctrica ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ENERGIA")) ? "X" : string.Empty, " ) Agua ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AGUA")) ? "X" : string.Empty,
                                    ")  Drenaje ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("DRENAJE")) ? "X" : string.Empty, " ) Pavimento ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("PAVIMENTO")) ? "X" : string.Empty,
                                    " ) Teléfono ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("TELEFONO")) ? "X" : string.Empty, " ) TV por Cable ( ", ServiciosPrimarios.Any(X => X.SOCIOECONOMICO_CAT.DESCR.Contains("TV POR CABLE")) ? "X" : string.Empty, " )");
                                DatosReporte.ServiciosPrimario = ServiciosP;


                                var ElectrodomesticosPrimarios = Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Where(x => x.ID_TIPO == "E" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string ElectrodomesticosP = string.Concat("Estufa ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ESTUFA")) ? "X" : string.Empty, ") Refrigerador ( ",
                                    ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("REFRIGERADOR")) ? "X" : string.Empty, " ) Horno Microondas ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("HORNO MICROONDAS")) ? "X" : string.Empty,
                                    " ) Televisión ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("TELEVISION")) ? "X" : string.Empty, " ) Lavadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("LAVADORA")) ? "X" : string.Empty,
                                    " )  Secadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("SECADORA")) ? "X" : string.Empty, " ) Computadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("COMPUTADORA")) ? "X" : string.Empty,
                                    " ) Otros ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("OTROS")) ? "X" : string.Empty, " )");
                                DatosReporte.ElectViviendaPrimario = ElectrodomesticosP;


                                var MediosTransporte = Estudio.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Where(x => x.ID_TIPO == "T" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string MediosPrimarios = string.Concat(" Automóvil ( ", MediosTransporte.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AUTOMOVIL")) ? "X" : string.Empty, " ) Autobús ( ", MediosTransporte.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AUTOBUS")) ? "X" : string.Empty,
                                    " ) Otro ( ", MediosTransporte.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("OTRO")) ? "X" : string.Empty, " ) ");
                                DatosReporte.MediosTranspPrimario = MediosPrimarios;

                                var ApoyoEconomico = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_TIPO == "A" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                DatosReporte.EspecifiqueApoyoEconomico = string.Concat(" Giro Telegráfico ( ", ApoyoEconomico.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("GIRO TELEGRAFICO")) ? "X" : string.Empty, " ) Cuenta Bancaria ( ", ApoyoEconomico.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("CUENTA BANCARIA")) ? "X" : string.Empty, " ) Deposito en la Institución ( ", ApoyoEconomico.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("DESPOSITO EN LA INSTITUCION")) ? "X" : string.Empty, " )");

                            };
                        };
                        #endregion

                        #region Datos Grupo Secundario
                        if (Estudio.SOCIOE_GPOFAMSEC != null)
                        {
                            DatosReporte.GrupoFamiliarSecundario = string.Concat("Funcional ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR) ? Estudio.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR.Equals("F") ? "X" : string.Empty : string.Empty, " )  Disfuncional ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR) ? Estudio.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR.Equals("D") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.RelacionIntroFamiliarGrupoSecundario = string.Concat("Adecuada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR) ? Estudio.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR.Equals("A") ? "X" : string.Empty : string.Empty, " )  Inadecuada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR) ? Estudio.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR.Equals("I") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.CuantasPersonasLaboranGrupoSecundario = Estudio.SOCIOE_GPOFAMSEC.PERSONAS_LABORAN.HasValue ? Estudio.SOCIOE_GPOFAMSEC.PERSONAS_LABORAN.Value.ToString() : string.Empty;
                            DatosReporte.IngresoMensualGrupoSecundario = Estudio.SOCIOE_GPOFAMSEC.INGRESO_MENSUAL.HasValue ? Estudio.SOCIOE_GPOFAMSEC.INGRESO_MENSUAL.Value.ToString("c") : string.Empty;
                            DatosReporte.EgresoMensualGrupoSecundario = Estudio.SOCIOE_GPOFAMSEC.EGRESO_MENSUAL.HasValue ? Estudio.SOCIOE_GPOFAMSEC.EGRESO_MENSUAL.Value.ToString("c") : string.Empty;
                            DatosReporte.AntecedentesAdiccionGrupoSecundario = string.Concat("SI ( ", Estudio.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE.HasValue ? Estudio.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE.Value == (decimal)eProcedimiento.SI ? "X" : string.Empty : string.Empty, " )  NO ( ", Estudio.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE.HasValue ? Estudio.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE.Value == (decimal)eProcedimiento.NO ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.EspecifiqueAdiccionAntecedentesGrupoSecundario = Estudio.SOCIOE_GPOFAMSEC.ANTECEDENTE;
                            DatosReporte.ZonaUbicacionGrupoSecundario = string.Concat("Urbana ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA.Equals("U") ? "X" : string.Empty : string.Empty, " )  Semi-Urbana ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA.Equals("S") ? "X" : string.Empty : string.Empty, " )  Rural ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA.Equals("R") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.CondicionesViviendaGrupoSecundario = string.Concat("Propia ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES.Equals("P") ? "X" : string.Empty : string.Empty, " )  Rentada ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES.Equals("R") ? "X" : string.Empty : string.Empty, " )  Otro ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES) ? Estudio.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES.Equals("O") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.NivelSocioEconomicoCulturalGrupoSecundario = string.Concat(" Alto ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL.Equals("A") ? "X" : string.Empty : string.Empty, " )  Medio ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL.Equals("M") ? "X" : string.Empty : string.Empty, " )  Bajo ( ", !string.IsNullOrEmpty(Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL) ? Estudio.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL.Equals("B") ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.RecibeVisitaFamiliar = string.Concat("SI ( ", Estudio.SOCIOE_GPOFAMSEC.RECIBE_VISITA.HasValue ? Estudio.SOCIOE_GPOFAMSEC.RECIBE_VISITA.Value == (decimal)eProcedimiento.SI ? "X" : string.Empty : string.Empty, " )  NO ( ", Estudio.SOCIOE_GPOFAMSEC.RECIBE_VISITA.HasValue ? Estudio.SOCIOE_GPOFAMSEC.RECIBE_VISITA.Value == (decimal)eProcedimiento.NO ? "X" : string.Empty : string.Empty, " ) ");
                            DatosReporte.DeQuien = Estudio.SOCIOE_GPOFAMSEC.VISITA;
                            DatosReporte.Frecuencia = Estudio.SOCIOE_GPOFAMSEC.FRECUENCIA;
                            DatosReporte.EnCasoDeNoRecibirVisitaEspecifique = Estudio.SOCIOE_GPOFAMSEC.MOTIVO_NO_VISITA;
                            DatosReporte.CuentaConApoyoEconomico = string.Concat("SI ( ", Estudio.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO.HasValue ? Estudio.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO.Value == (decimal)eProcedimiento.SI ? "X" : string.Empty : string.Empty, " )  NO ( ", Estudio.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO.HasValue ? Estudio.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO.Value == (decimal)eProcedimiento.NO ? "X" : string.Empty : string.Empty, " )");
                            DatosReporte.Dictamen = Estudio.DICTAMEN;
                            DatosReporte.FechaEstudio = Estudio.DICTAMEN_FEC.HasValue ? Estudio.DICTAMEN_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;

                            if (Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Any())
                            {
                                var MaterialesSecundarios = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(w => w.ID_TIPO == "M" && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);//Lista de los materiales
                                DatosReporte.VivivendamaterialesSecundario = string.Concat("Cartón ( ", MaterialesSecundarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("CARTON")) ? "X" : string.Empty, " ) ",
                                    " Adobe ( ", MaterialesSecundarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ADOBE")) ? "X" : string.Empty, " ) Ladrillo ( ", MaterialesSecundarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("LADRILLO")) ? "X" : string.Empty,
                                    " ) Madera ( ", MaterialesSecundarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("MADERA")) ? "X" : string.Empty,
                                    ") Block ( ", MaterialesSecundarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("BLOCK")) ? "X" : string.Empty, " ) Otro ( ", MaterialesSecundarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("OTRO")) ? "X" : string.Empty, " )");

                                var DistribucionPrimaria = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_TIPO == "D" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string DristriGrupoPrimario = string.Concat("Sala ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("SALA")) ? "X" : string.Empty,
                                    " )  Cocina ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("COCINA")) ? "X" : string.Empty, " ) Comedor ( ",
                                DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("COMEDOR")) ? "X" : string.Empty, " ) Recamara ( ", DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("RECAMARA")) ? "X" : string.Empty,
                                " ) Baño ( ", DistribucionPrimaria.Any(X => X.SOCIOECONOMICO_CAT.DESCR.Contains("BAÑO")) ? "X" : string.Empty, " ) Otros ( ",
                                DistribucionPrimaria.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("OTROS")) ? "X" : string.Empty, " ) ");
                                DatosReporte.DistViviendaSecundario = DristriGrupoPrimario;


                                var ServiciosPrimarios = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_TIPO == "S" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string ServiciosP = string.Concat("Energía  Eléctrica ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ENERGIA")) ? "X" : string.Empty, " ) Agua ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AGUA")) ? "X" : string.Empty,
                                    ")  Drenaje ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("DRENAJE")) ? "X" : string.Empty, " ) Pavimento ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("PAVIMENTO")) ? "X" : string.Empty,
                                    "Teléfono ( ", ServiciosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("TELEFONO")) ? "X" : string.Empty, " ) TV por Cable ( ", ServiciosPrimarios.Any(X => X.SOCIOECONOMICO_CAT.DESCR.Contains("TV POR CABLE")) ? "X" : string.Empty, " )");
                                DatosReporte.ServicioSecundario = ServiciosP;



                                var ElectrodomesticosPrimarios = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_TIPO == "E" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string ElectrodomesticosP = string.Concat("Estufa ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("ESTUFA")) ? "X" : string.Empty, " ) Refrigerador ( ",
                                    ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("REFRIGERADOR")) ? "X" : string.Empty, " ) Horno Microondas ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("HORNO MICROONDAS")) ? "X" : string.Empty,
                                    " ) Televisión ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("TELEVISION")) ? "X" : string.Empty, " ) Lavadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("LAVADORA")) ? "X" : string.Empty,
                                    " )  Secadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("SECADORA")) ? "X" : string.Empty, " ) Computadora ( ", ElectrodomesticosPrimarios.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("COMPUTADORA")) ? "X" : string.Empty,
                                    " ) Otros ( ", ElectrodomesticosPrimarios.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("OTROS")) ? "X" : string.Empty, " ) ");
                                DatosReporte.ElectViviendaSecundario = ElectrodomesticosP;

                                var MediosTransporte = Estudio.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_TIPO == "T" && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                                string MediosPrimarios = string.Concat(" Automóvil ( ", MediosTransporte.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AUTOMOVIL")) ? "X" : string.Empty, " ) Autobús ( ", MediosTransporte.Any(x => x.SOCIOECONOMICO_CAT.DESCR.Contains("AUTOBUS")) ? "X" : string.Empty,
                                    " ) Otro ( ", MediosTransporte.Any(z => z.SOCIOECONOMICO_CAT.DESCR.Contains("OTRO")) ? "X" : string.Empty, " ) ");
                                DatosReporte.MediosTransporteSec = MediosPrimarios;
                            };
                        };

                        #endregion

                        #endregion

                        #region Se forma el encabezado del reporte
                        var Encabezado = new cEncabezado();
                        Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                        Encabezado.TituloDos = Parametro.ENCABEZADO2;
                        Encabezado.NombreReporte = "ESTUDIO SOCIOECONÓMICO";
                        Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                        Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;

                        #endregion

                        #region Inicializacion de reporte
                        View.Report.LocalReport.ReportPath = "Reportes/rEstudioSocioEconomico.rdlc";
                        View.Report.LocalReport.DataSources.Clear();
                        #endregion

                        #region Definicion de origenes de datos

                        //datasource Uno
                        var ds1 = new List<cEncabezado>();
                        ds1.Add(Encabezado);
                        Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds1.Name = "DataSet2";
                        rds1.Value = ds1;
                        View.Report.LocalReport.DataSources.Add(rds1);


                        //datasource dos
                        var ds2 = new List<cCuerpoReporte>();
                        ds2.Add(DatosReporte);
                        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds2.Name = "DataSet1";
                        rds2.Value = ds2;
                        View.Report.LocalReport.DataSources.Add(rds2);

                        View.Report.LocalReport.DisplayName = string.Format("Estudio Socioeconómico del imputado {0} {1} {2} ",//Nombre del archivo que se va a generar con el reporte independientemente del formato que se elija
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);

                        View.Report.ReportExport += (s, e) =>
                        {
                            try
                            {
                                if (e != null && !e.Extension.LocalizedName.Equals("PDF"))//Solo permite pdf
                                {
                                    e.Cancel = true;//Detiene el evento
                                    s = e = null;//Detiene el mapeo de los parametros para que no continuen en el arbol
                                    (new Dialogos()).ConfirmacionDialogo("Validación", "No tiene permitido exportar la información en este formato.");
                                }
                            }

                            catch (Exception exc)
                            {
                                throw exc;
                            }
                        };

                        View.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                        View.Report.RefreshReport();
                    }

                        #endregion
                }
                #endregion
            }

            catch (Exception exc)
            {
                throw exc;
            }

        }

        private void ImprimirEMI()
        {
            try
            {
                if (!IngresoSeleccionado.EMI_INGRESO.Any())
                    new Dialogos().ConfirmacionDialogo("Notificación", "El interno seleccionado no tiene emi");

                if (SelectEmi != null)
                {
                    CalcularSentencia();

                    if (SelectEmi == null)
                        return;

                    if (SelectEmi.EMI == null)
                        return;

                    var view = new ReportesView();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    view.Owner = PopUpsViewModels.MainWindow;
                    view.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    view.Show();

                    var ficha = new cFichaEMI();

                    if (SelectEmi.EMI.EMI_INGRESO != null)
                        if (SelectEmi.EMI.EMI_INGRESO.INGRESO != null)
                        {
                            ficha.FechaCaptura = SelectEmi.EMI.EMI_INGRESO.INGRESO.FEC_REGISTRO.HasValue
                                ? SelectEmi.EMI.EMI_INGRESO.INGRESO.FEC_REGISTRO.Value.ToString("dd/MM/yyyy")
                                : string.Empty;

                            ficha.Centro = SelectEmi.EMI.EMI_INGRESO.INGRESO.CENTRO != null
                                ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.CENTRO.DESCR)
                                    ? SelectEmi.EMI.EMI_INGRESO.INGRESO.CENTRO.DESCR.Trim()
                                    : string.Empty
                                : string.Empty;

                            ficha.EstatusJuridico = SelectEmi.EMI.EMI_INGRESO.INGRESO.ESTATUS_ADMINISTRATIVO != null
                                ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.ESTATUS_ADMINISTRATIVO.DESCR)
                                    ? SelectEmi.EMI.EMI_INGRESO.INGRESO.ESTATUS_ADMINISTRATIVO.DESCR.Trim()
                                    : string.Empty
                                : string.Empty;

                            ficha.FechaIngreso = SelectEmi.EMI.EMI_INGRESO.INGRESO.FEC_INGRESO_CERESO.HasValue ? SelectEmi.EMI.EMI_INGRESO.INGRESO.FEC_INGRESO_CERESO.Value.ToString("dd/MM/yyyy") : string.Empty;

                            ficha.Expediente = string.Format("{0}/{1}",
                                SelectEmi.EMI.EMI_INGRESO.INGRESO.ID_ANIO,
                                SelectEmi.EMI.EMI_INGRESO.INGRESO.ID_IMPUTADO);

                            ficha.EstadoCivil = SelectEmi.EMI.EMI_INGRESO.INGRESO.ID_ESTADO_CIVIL.HasValue
                                ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.ESTADO_CIVIL.DESCR)
                                    ? SelectEmi.EMI.EMI_INGRESO.INGRESO.ESTADO_CIVIL.DESCR.Trim()
                                    : string.Empty
                                : string.Empty;

                            ficha.Religion = SelectEmi.EMI.EMI_INGRESO.INGRESO.ID_RELIGION.HasValue
                                ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.RELIGION.DESCR)
                                    ? SelectEmi.EMI.EMI_INGRESO.INGRESO.RELIGION.DESCR.Trim()
                                    : string.Empty
                                : string.Empty;

                            ficha.ColoniaResidencia = SelectEmi.EMI.EMI_INGRESO.INGRESO.ID_COLONIA.HasValue
                                ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.COLONIA.DESCR)
                                    ? SelectEmi.EMI.EMI_INGRESO.INGRESO.COLONIA.DESCR.Trim()
                                    : string.Empty
                                : string.Empty;

                            ficha.CiudadEstadoResidencia = SelectEmi.EMI.EMI_INGRESO.INGRESO.MUNICIPIO != null
                                ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.MUNICIPIO.MUNICIPIO1)
                                    ? SelectEmi.EMI.EMI_INGRESO.INGRESO.MUNICIPIO.MUNICIPIO1.Trim()
                                    : string.Empty
                                : string.Empty;

                            ficha.TiempoResidenciaBC = SelectEmi.EMI.EMI_INGRESO.INGRESO.RESIDENCIA_ANIOS.HasValue
                                ? string.Format("{0} AÑOS",
                                    SelectEmi.EMI.EMI_INGRESO.INGRESO.RESIDENCIA_ANIOS)
                                : "0 AÑOS";
                        };

                    ficha.Ubicacion = string.Empty;
                    if (SelectEmi.EMI.EMI_INGRESO != null)
                        if (SelectEmi.EMI.EMI_INGRESO.INGRESO != null)
                            if (SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA != null)
                            {
                                ficha.Ubicacion = string.Format("{0}-{1}-{2}-{3}",
                                    SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA != null ? SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA.SECTOR != null ? SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                    SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA != null ? SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA.SECTOR.DESCR) ? SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                                    SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA != null ? SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.CELDA.ID_CELDA.Trim() : string.Empty,
                                        SelectEmi.EMI.EMI_INGRESO.INGRESO.CAMA.ID_CAMA);//"M15-C23-4";
                            };

                    #region Pendiente
                    ficha.CausaPenal = string.Empty;
                    if (SelectEmi.EMI != null)
                        if (SelectEmi.EMI.EMI_INGRESO.INGRESO.CAUSA_PENAL != null)
                            if (SelectEmi.EMI.EMI_INGRESO.INGRESO.CAUSA_PENAL != null && SelectEmi.EMI.EMI_INGRESO.INGRESO.CAUSA_PENAL.Any())
                                foreach (var cp in SelectEmi.EMI.EMI_INGRESO.INGRESO.CAUSA_PENAL)
                                {
                                    if (!string.IsNullOrEmpty(ficha.CausaPenal))
                                        ficha.CausaPenal = ficha.CausaPenal + ",";
                                    ficha.CausaPenal = ficha.CausaPenal + string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO);
                                }

                    //ficha.CentroProcedencia = "NINGUNO";
                    #endregion

                    if (SelectEmi.EMI.EMI_INGRESO != null)
                        if (SelectEmi.EMI.EMI_INGRESO.INGRESO != null)
                            if (SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO != null)
                            {
                                ficha.Nombre = string.Format("{0} {1} {2}",
                                    !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.NOMBRE)
                                        ? SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim()
                                        : string.Empty,
                                    !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.PATERNO)
                                        ? SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim()
                                        : string.Empty,
                                    !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.MATERNO)
                                        ? SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim()
                                        : string.Empty);

                                ficha.Edad = new Fechas().CalculaEdad(SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString();
                                ficha.Sexo = !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.SEXO) ? SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.SEXO.Equals("M") ? "MASCULINO" : "FEMENINO" : string.Empty;

                                ficha.Apodo = string.Empty;
                                if (SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.APODO != null && SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.APODO.Any())
                                    foreach (var apodo in SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.APODO)
                                    {
                                        if (!string.IsNullOrEmpty(ficha.Apodo))
                                        {
                                            ficha.Apodo = string.Format("{0},", ficha.Apodo);
                                        }

                                        ficha.Apodo = string.Format("{0}{1}", ficha.Apodo, !string.IsNullOrEmpty(apodo.APODO1) ? apodo.APODO1.Trim() : string.Empty);
                                    };

                                ficha.FechaNacimiento =
                                    SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.NACIMIENTO_FECHA.HasValue
                                        ? SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value.ToString(
                                            "dd/MM/yyyy")
                                        : string.Empty;

                                ficha.LugarNacimiento =
                                    !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.NACIMIENTO_LUGAR)
                                        ? SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.NACIMIENTO_LUGAR.Trim()
                                        : string.Empty;

                                ficha.Nacionalidad = SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.PAIS_NACIONALIDAD !=
                                                     null
                                    ? !string.IsNullOrEmpty(
                                        SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD)
                                        ? SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD.Trim
                                            ()
                                        : string.Empty
                                    : string.Empty;

                                ficha.Etnia = SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.ID_ETNIA.HasValue
                                    ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.ETNIA.DESCR)
                                        ? SelectEmi.EMI.EMI_INGRESO.INGRESO.IMPUTADO.ETNIA.DESCR.Trim()
                                        : string.Empty
                                    : string.Empty;

                            };

                    if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION != null)
                    {
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.TIEMPO_RESID_COL != null)
                            ficha.TiempoColonia = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.TIEMPO_RESID_COL;
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EDUCACION_CERTIFICADO != null)
                            ficha.UltimoGradoEstudios = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EDUCACION_GRADO != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EDUCACION_GRADO.DESCR) ? SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EDUCACION_GRADO.DESCR.Trim() : string.Empty : string.Empty;// "DOCTORADO EN CIENCIAS";
                        else
                            ficha.UltimoGradoEstudios = string.Empty;

                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.PERSONA_CONVIVENCIA_ANTERIOR != null)
                            ficha.ViviaAntesDetencion = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.PERSONA_CONVIVENCIA_ANTERIOR;//"JIMENEZ CASTRO DIANA";
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.TIPO_REFERENCIA != null)
                            ficha.Parentesco = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.TIPO_REFERENCIA != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.TIPO_REFERENCIA.DESCR) ? SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty;// "MADRE";
                        else
                            ficha.Parentesco = string.Empty;

                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EXFUNCIONARIO_SEGPUB != null)
                            ficha.ExfuncionarioSP = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EXFUNCIONARIO_SEGPUB != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EXFUNCIONARIO_SEGPUB.DESCR) ? SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EXFUNCIONARIO_SEGPUB.DESCR.Trim() : string.Empty : string.Empty;// "NO";
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.PASAPORTE != null)
                            ficha.Pasaporte = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.PASAPORTE.Equals("S") ? "SI" : "NO";//"NO";
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.LICENCIA_MANEJO != null)
                            ficha.LicenciaManejo = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.LICENCIA_MANEJO.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.CREDENCIAL_ELECTOR != null)
                            ficha.CredencialElector = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.CREDENCIAL_ELECTOR.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.CARTILLA_MILITAR != null)
                            ficha.CartillaMilitar = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.CARTILLA_MILITAR.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EDUCACION_CERTIFICADO != null)
                            ficha.CertificadoEducacion = !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EDUCACION_CERTIFICADO.DESCR) ? SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.EDUCACION_CERTIFICADO.DESCR.Trim() : string.Empty;//"DOCTORADO";
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.OFICIOS_HABILIDADES != null)
                            ficha.OficiosHabilidades = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.OFICIOS_HABILIDADES;// "ESTUDIO DE LA TEORIA DE LA CUERDA";
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.CAMBIOS_DOMICILIO_ULTIMO_ANO != null)
                            ficha.CambiosDomiciolio = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.CAMBIOS_DOMICILIO_ULTIMO_ANO.ToString();// "0";
                        if (SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.MOTIVOS_CAMBIOS_DOMICILIO != null)
                            ficha.MotivoCambio = SelectEmi.EMI.EMI_FICHA_IDENTIFICACION.MOTIVOS_CAMBIOS_DOMICILIO;// "NA";
                    }
                    var empleos = new List<cUltimosEmpleosEMI>();
                    if (SelectEmi.EMI.EMI_ULTIMOS_EMPLEOS != null)
                    {
                        foreach (var emp in SelectEmi.EMI.EMI_ULTIMOS_EMPLEOS)
                        {
                            empleos.Add(new cUltimosEmpleosEMI()
                            {
                                Ocupacion = emp.OCUPACION != null ? !string.IsNullOrEmpty(emp.OCUPACION.DESCR) ? emp.OCUPACION.DESCR.Trim() : string.Empty : string.Empty,
                                Duracion = emp.DURACION,
                                Empresa = emp.EMPRESA,
                                MotivoDesempleo = emp.MOTIVO_DESEMPLEO,
                                EmpleoFormal = emp.EMPLEO_FORMAL.Equals("S") ? "SI" : "NO",
                                Ultimo = emp.ULTIMO_EMPLEO_ANTES_DETENCION.Equals("S") ? "SI" : "NO",
                                InestabilidadLaboral = emp.INESTABILIDAD_LABORAL.Equals("S") ? "SI" : "NO"
                            });
                        }
                    }
                    //PENDIENTE
                    var delitos = new List<cDelitosEMI>();
                    var juridico = new cSituacionJuridicaEMI();
                    if (SelectEmi.EMI.EMI_SITUACION_JURIDICA != null)
                    {
                        juridico.VersionDelito = SelectEmi.EMI.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO;
                        juridico.MenorTiempoEntreIngresos = string.Empty;
                        switch (SelectEmi.EMI.EMI_SITUACION_JURIDICA.MENOR_PERIODO_LIBRE_REING)
                        {
                            case "MENORA1":
                                juridico.MenorTiempoEntreIngresos = "MENOR A UN AÑO";
                                break;
                            case "DE1A5":
                                juridico.MenorTiempoEntreIngresos = "DE 1 A 5 AÑOS";
                                break;
                            case "MAS5":
                                juridico.MenorTiempoEntreIngresos = "MAS DE 5 AÑOS";
                                break;
                        }
                        juridico.MayorTiempoEntreIngresos = string.Empty;
                        switch (SelectEmi.EMI.EMI_SITUACION_JURIDICA.MAYOR_PERIODO_LIBRE_REING)
                        {
                            case "MENORA1":
                                juridico.MayorTiempoEntreIngresos = "MENOR A UN AÑO";
                                break;
                            case "DE1A5":
                                juridico.MayorTiempoEntreIngresos = "DE 1 A 5 AÑOS";
                                break;
                            case "MAS5":
                                juridico.MayorTiempoEntreIngresos = "MAS DE 5 AÑOS";
                                break;
                        }
                        juridico.PracticadoEstudios = !string.IsNullOrEmpty(SelectEmi.EMI.EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS) ? SelectEmi.EMI.EMI_SITUACION_JURIDICA.PRACT_ESTUDIOS.Equals("S") ? "SI" : "NO" : string.Empty;
                        juridico.CuandoPracticaronEstudios = SelectEmi.EMI.EMI_SITUACION_JURIDICA.CUANDO_PRACT_ESTUDIOS;
                        juridico.DeseaTraslado = !string.IsNullOrEmpty(SelectEmi.EMI.EMI_SITUACION_JURIDICA.DESEA_TRASLADO) ? SelectEmi.EMI.EMI_SITUACION_JURIDICA.DESEA_TRASLADO.Equals("S") ? "SI" : "NO" : string.Empty;
                        juridico.DondeDeseaTraslado = SelectEmi.EMI.EMI_SITUACION_JURIDICA.DONDE_DESEA_TRASLADO;
                        juridico.MotivoDeseaTraslado = SelectEmi.EMI.EMI_SITUACION_JURIDICA.MOTIVO_DESEA_TRASLADO;
                    }

                    var ingresos = new List<cIngresosAnterioresCentroEMI>();
                    var ingresosMenores = new List<cIngresosAnterioresCentroMenoresEMI>();

                    if (SelectEmi.EMI.EMI_INGRESO_ANTERIOR != null)
                    {
                        foreach (var ing in SelectEmi.EMI.EMI_INGRESO_ANTERIOR)
                        {
                            if (ing.ID_TIPO == 1)//MENORES
                            {///TODO: cambios delito
                                ingresosMenores.Add(new cIngresosAnterioresCentroMenoresEMI()
                                {
                                    CentroM = ing.EMISOR != null ? !string.IsNullOrEmpty(ing.EMISOR.DESCR) ? ing.EMISOR.DESCR.Trim() : string.Empty : string.Empty,
                                    DelitoM = !string.IsNullOrEmpty(ing.DELITO) ? ing.DELITO.Trim() : string.Empty,
                                    PeriodoReclusionM = ing.PERIODO_RECLUSION,
                                    SancionesM = ing.SANCIONES
                                });
                            }
                            else //MAYORES
                            {///TODO: cambios delito
                                ingresos.Add(new cIngresosAnterioresCentroEMI()
                                {
                                    Centro = ing.EMISOR != null ? !string.IsNullOrEmpty(ing.EMISOR.DESCR) ? ing.EMISOR.DESCR.Trim() : string.Empty : string.Empty,
                                    Delito = !string.IsNullOrEmpty(ing.DELITO) ? ing.DELITO.Trim() : string.Empty,
                                    PeriodoReclusion = ing.PERIODO_RECLUSION,
                                    Sanciones = ing.SANCIONES
                                });
                            }
                        }
                    }

                    var sociofamiliares = new cSocioFamiliaresEMI();
                    if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES != null)
                    {
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR != null)
                            sociofamiliares.VisitaFamiliar = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_FAMILIAR.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA != null)
                            sociofamiliares.FrecuenciaVF = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA.DESCR;
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA != null)
                            sociofamiliares.VisitaIntima = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_VISITA_INTIMA.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO != null)
                            sociofamiliares.ApoyoEconomico = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.RECIBE_APOYO_ECONOMICO.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_APOYO_ECONOMICO != null)
                            sociofamiliares.CantidadAE = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_APOYO_ECONOMICO;
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA1 != null)
                            sociofamiliares.FrecuenciaAE = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.FRECUENCIA1.DESCR;
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE != null)
                            sociofamiliares.VivePadre = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_PADRE.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE != null)
                            sociofamiliares.ViveMadre = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.VIVE_MADRE.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_PADRE != null)
                            sociofamiliares.EdadImpFallecioPAdre = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_PADRE.ToString();
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_MADRE != null)
                            sociofamiliares.EdadImpFallecioMadre = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_FALLE_MADRE.ToString();
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.PADRES_JUNTOS != null)
                            sociofamiliares.PadresJuntos = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.PADRES_JUNTOS.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.MOTIVOS_SEPARACION != null)
                            sociofamiliares.MotivoSeparacionPadres = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.MOTIVOS_SEPARACION;
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_SEPARACION != null)
                            sociofamiliares.EdadSeparacionPadres = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INTERNO_SEPARACION.ToString();

                        switch (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL)
                        {
                            case 1:
                                sociofamiliares.NivelSocial = "BAJO";
                                break;
                            case 2: sociofamiliares.NivelSocial = "MEDIO";
                                break;
                            case 3:
                                sociofamiliares.NivelSocial = "ALTO";
                                break;
                        }
                        switch (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_CULTURAL)
                        {
                            case 1:
                                sociofamiliares.NivelCultural = "BAJO";
                                break;
                            case 2: sociofamiliares.NivelCultural = "MEDIO";
                                break;
                            case 3:
                                sociofamiliares.NivelCultural = "ALTO";
                                break;
                        }
                        switch (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.ID_NIVEL_SOCIAL)
                        {
                            case 1:
                                sociofamiliares.NivelEconomico = "BAJO";
                                break;
                            case 2: sociofamiliares.NivelEconomico = "MEDIO";
                                break;
                            case 3:
                                sociofamiliares.NivelEconomico = "ALTO";
                                break;
                        }

                        sociofamiliares.TotalParejas = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.TOTAL_PAREJAS.ToString();
                        sociofamiliares.Union = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_PAREJAS_UNION.ToString();
                        sociofamiliares.NoHijos = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.NUMERO_HIJOS.ToString();
                        sociofamiliares.HijosRegistrados = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.HIJOS_REGISTRADOS.ToString();
                        sociofamiliares.HijosRelacion = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_RELACION.ToString();
                        sociofamiliares.HijosVisitan = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CANTIDAD_HIJOS_VISITA.ToString();
                        sociofamiliares.ContactoNombre = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_NOMBRE;
                        sociofamiliares.ContactoPArentesco = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.TIPO_REFERENCIA.DESCR;
                        sociofamiliares.ContactoTelefono = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.HasValue ? SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.Value.ToString() : string.Empty;
                        //TextContactoTelefono = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.HasValue ? SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO.Value.ToString() : string.Empty;
                        sociofamiliares.AbandoonoFamiliar = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.ABANDONO_FAMILIAR.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EMI_FACTORES_ABANDONO != null)
                            sociofamiliares.DescrAFam = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EMI_FACTORES_ABANDONO.DESCR;
                        else
                            sociofamiliares.DescrAFam = string.Empty;
                        if (SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES != null ? SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EMI_FACTORES_HUIDA != null : false)
                            sociofamiliares.HuidasHogar = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EMI_FACTORES_HUIDA.DESCR;

                        sociofamiliares.MaltratoEmocional = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_EMOCIONAL.Equals("S") ? "SI" : "NO";
                        sociofamiliares.DescrME = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIF_MALTRATO_EMOCIONAL;
                        sociofamiliares.MaltratoFisico = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.MALTRATO_FISICO.Equals("S") ? "SI" : "NO";
                        sociofamiliares.DescrMF = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_MALTRATO_FISICO;
                        sociofamiliares.AbusoFisico = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.ABUSO_SEXUAL.Equals("S") ? "SI" : "NO";
                        sociofamiliares.DescrAF = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.ESPECIFIQUE_ABUSO_SEXUAL;
                        sociofamiliares.EdadAbuso = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_ABUSO_SEXUAL.ToString();
                        sociofamiliares.InicioContactoSexual = SelectEmi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.EDAD_INICIO_CONTACTO_SEXUAL.ToString();
                    }
                    var grupoFamiliar = new List<cGrupoFamiliarEMI>();
                    if (SelectEmi.EMI.EMI_GRUPO_FAMILIAR != null)
                    {
                        var g = string.Empty;
                        foreach (var grupo in SelectEmi.EMI.EMI_GRUPO_FAMILIAR)
                        {
                            switch (grupo.GRUPO)
                            {
                                case 1:
                                    g = "PRIMARIO";
                                    break;
                                case 2:
                                    g = "SECUNDARIO";
                                    break;
                                case 3:
                                    g = "NINGUNO";
                                    break;
                            }
                            grupoFamiliar.Add(new cGrupoFamiliarEMI()
                            {
                                Grupo = g,
                                Paterno = grupo.PATERNO,
                                Materno = grupo.MATERNO,
                                Nombre = grupo.NOMBRE,
                                Edad = grupo.EDAD.HasValue ? grupo.EDAD.ToString() : string.Empty,
                                Relacion = grupo.TIPO_REFERENCIA != null ? !string.IsNullOrEmpty(grupo.TIPO_REFERENCIA.DESCR) ? grupo.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                Domicilio = grupo.DOMICILIO,
                                Ocupacion = grupo.OCUPACION != null ? !string.IsNullOrEmpty(grupo.OCUPACION.DESCR) ? grupo.OCUPACION.DESCR.Trim() : string.Empty : string.Empty,
                                EdoCivil = grupo.ESTADO_CIVIL != null ? !string.IsNullOrEmpty(grupo.ESTADO_CIVIL.DESCR) ? grupo.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty,
                                ViveConEl = grupo.VIVE_C_EL.Equals("S") ? "SI" : "NO"
                            });
                        }
                    }
                    var familiarDelito = new List<cFamiliarAntecedentesEMI>();
                    if (SelectEmi.EMI.EMI_ANTECEDENTE_FAM_CON_DEL != null && SelectEmi.EMI.EMI_ANTECEDENTE_FAM_CON_DEL.Any())
                    {
                        foreach (var ant in SelectEmi.EMI.EMI_ANTECEDENTE_FAM_CON_DEL)
                        {
                            familiarDelito.Add(new cFamiliarAntecedentesEMI()
                            {
                                Parentesco = ant.TIPO_REFERENCIA != null ? !string.IsNullOrEmpty(ant.TIPO_REFERENCIA.DESCR) ? ant.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                //Anio = ant.ANIO.ToString(),
                                Centro = ant.EMISOR != null ? !string.IsNullOrEmpty(ant.EMISOR.DESCR) ? ant.EMISOR.DESCR.Trim() : string.Empty : string.Empty,
                                Delito = ant.DELITO,
                                Relacion = ant.TIPO_RELACION != null ? !string.IsNullOrEmpty(ant.TIPO_RELACION.DESCR) ? ant.TIPO_RELACION.DESCR.Trim() : string.Empty : string.Empty
                            });
                        }
                    }

                    var familiarDroga = new List<cFamiliarDrogaEMI>();
                    if (SelectEmi.EMI.EMI_ANTECEDENTE_FAMILIAR_DROGA != null && SelectEmi.EMI.EMI_ANTECEDENTE_FAMILIAR_DROGA.Any())
                    {
                        foreach (var droga in SelectEmi.EMI.EMI_ANTECEDENTE_FAMILIAR_DROGA)
                        {
                            familiarDroga.Add(new cFamiliarDrogaEMI()
                            {
                                Parentesco = droga.TIPO_REFERENCIA != null ? !string.IsNullOrEmpty(droga.TIPO_REFERENCIA.DESCR) ? droga.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                //Anio = droga.ANIO.ToString(),
                                Relacion = droga.TIPO_RELACION != null ? !string.IsNullOrEmpty(droga.TIPO_RELACION.DESCR) ? droga.TIPO_RELACION.DESCR.Trim() : string.Empty : string.Empty,
                                TipoDroga = droga.DROGA != null ? !string.IsNullOrEmpty(droga.DROGA.DESCR) ? droga.DROGA.DESCR.Trim() : string.Empty : string.Empty
                            });
                        }
                    }

                    var conductaParasocial = new cConductaParasocialEMI();
                    if (SelectEmi.EMI.EMI_HPS != null)
                    {
                        if (SelectEmi.EMI.EMI_HPS.VIVIO_CALLE_ORFANATO_NINO != null)
                            conductaParasocial.VivioCalleOrfanato = SelectEmi.EMI.EMI_HPS.VIVIO_CALLE_ORFANATO_NINO.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS.PERTENECE_PANDILLA_ACTUAL != null)
                            conductaParasocial.PertenecePandilla = SelectEmi.EMI.EMI_HPS.PERTENECE_PANDILLA_ACTUAL.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS != null ? SelectEmi.EMI.EMI_HPS.PANDILLA1 != null : false)
                            conductaParasocial.Pandilla = SelectEmi.EMI.EMI_HPS.PANDILLA1.NOMBRE;
                        //LstPandillas.Where(w => w.ID_PANDILLA == SelectEmi.EMI_HPS.ID_PANDILLA).FirstOrDefault().NOMBRE;
                        if (SelectEmi.EMI.EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL != null)
                            conductaParasocial.ComportamientoHomosexual = SelectEmi.EMI.EMI_HPS.COMPORTAMIENTO_HOMOSEXUAL.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS.EDAD_INICIAL_HOMOSEXUAL != null)
                            conductaParasocial.EdadInicioCH = SelectEmi.EMI.EMI_HPS.EDAD_INICIAL_HOMOSEXUAL.ToString();
                        if (SelectEmi.EMI.EMI_HPS.ROL_HOMOSEXUAL != null)
                            conductaParasocial.RolCH = SelectEmi.EMI.EMI_HPS.ROL_HOMOSEXUAL;
                        if (SelectEmi.EMI.EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR != null)
                            conductaParasocial.PandillaExterior = SelectEmi.EMI.EMI_HPS.PERTENECIO_PANDILAS_EXTERIOR.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS.EDAD_INICIAL_PANDILLAS != null)
                            conductaParasocial.EdadInicioPE = SelectEmi.EMI.EMI_HPS.EDAD_INICIAL_PANDILLAS.ToString();
                        if (SelectEmi.EMI.EMI_HPS.MOTIVOS_PERTENENCIA_PANDILLAS != null)
                            conductaParasocial.MotivoPE = SelectEmi.EMI.EMI_HPS.MOTIVOS_PERTENENCIA_PANDILLAS;
                        if (SelectEmi.EMI.EMI_HPS.VAGANCIA != null)
                            conductaParasocial.Vagancia = SelectEmi.EMI.EMI_HPS.VAGANCIA;
                        if (SelectEmi.EMI.EMI_HPS.EDAD_INICIAL_VAGANCIA != null)
                            conductaParasocial.EdadInicioVagancia = SelectEmi.EMI.EMI_HPS.EDAD_INICIAL_VAGANCIA.ToString();
                        if (SelectEmi.EMI.EMI_HPS.MOTIVOS_VAGANCIA != null)
                            conductaParasocial.MotivoVagancia = SelectEmi.EMI.EMI_HPS.MOTIVOS_VAGANCIA;
                        if (SelectEmi.EMI.EMI_HPS.CICATRICES != null)
                            conductaParasocial.Cicatrices = SelectEmi.EMI.EMI_HPS.CICATRICES.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS.EDAD_INICIO_CICATRICES != null)
                            conductaParasocial.EdadInicioC = SelectEmi.EMI.EMI_HPS.EDAD_INICIO_CICATRICES.ToString();
                        if (SelectEmi.EMI.EMI_HPS.CICATRIZ_POR_RINA != null)
                            conductaParasocial.CicatricesRinia = SelectEmi.EMI.EMI_HPS.CICATRIZ_POR_RINA.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS.MOTIVO_CICATRICES != null)
                            conductaParasocial.MotivoCicatrices = SelectEmi.EMI.EMI_HPS.MOTIVO_CICATRICES;
                        if (SelectEmi.EMI.EMI_HPS.DESERCION_ESCOLAR != null)
                            conductaParasocial.DesercionEscolar = SelectEmi.EMI.EMI_HPS.DESERCION_ESCOLAR.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS.MOTIVO_DESERCION_ESCOLAR != null)
                            conductaParasocial.MotivoDE = SelectEmi.EMI.EMI_HPS.MOTIVO_DESERCION_ESCOLAR;
                        if (SelectEmi.EMI.EMI_HPS.REPROBACION_ESCOLAR != null)
                            conductaParasocial.ReprobacionEscolar = SelectEmi.EMI.EMI_HPS.REPROBACION_ESCOLAR.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS != null ? SelectEmi.EMI.EMI_HPS.GRADO_REPROBACION_ESCOLAR.HasValue ? SelectEmi.EMI.EMI_HPS.GRADO_REPROBACION_ESCOLAR.Value != -1 : false : false)
                            conductaParasocial.GradoRE = SelectEmi.EMI.EMI_HPS.GRADO_REPROBACION_ESCOLAR.Value.ToString();
                        //LstGradoEducativo.Where(w => w.ID_GRADO == SelectEmi.EMI_HPS.GRADO_REPROBACION_ESCOLAR).FirstOrDefault().DESCR;
                        if (SelectEmi.EMI.EMI_HPS.MOTIVO_REPROBACION_ESCOLAR != null)
                            conductaParasocial.MotivoRE = SelectEmi.EMI.EMI_HPS.MOTIVO_REPROBACION_ESCOLAR;
                        if (SelectEmi.EMI.EMI_HPS.EXPULSION_ESCOLAR != null)
                            conductaParasocial.ExpulsionEscolar = SelectEmi.EMI.EMI_HPS.EXPULSION_ESCOLAR.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS != null ? SelectEmi.EMI.EMI_HPS.GRADO_EXPULSION_ESCOLAR.HasValue ? SelectEmi.EMI.EMI_HPS.GRADO_EXPULSION_ESCOLAR.Value != -1 : false : false)
                            conductaParasocial.GradoEE = SelectEmi.EMI.EMI_HPS.GRADO_EXPULSION_ESCOLAR.Value.ToString();
                        //LstGradoEducativo.Where(w => w.ID_GRADO == SelectEmi.EMI_HPS.GRADO_EXPULSION_ESCOLAR).FirstOrDefault().DESCR;
                        if (SelectEmi.EMI.EMI_HPS.MOTIVO_EXPULSION_ESCOLAR != null)
                            conductaParasocial.MotivoEE = SelectEmi.EMI.EMI_HPS.MOTIVO_EXPULSION_ESCOLAR;
                        if (SelectEmi.EMI.EMI_HPS.PROSTITUIA_HOMBRES != null)
                            conductaParasocial.ProstituiaHombres = SelectEmi.EMI.EMI_HPS.PROSTITUIA_HOMBRES.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS.PROSTITUIA_MUJERES != null)
                            conductaParasocial.ProstituiaMujeres = SelectEmi.EMI.EMI_HPS.PROSTITUIA_MUJERES.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS != null ? SelectEmi.EMI.EMI_HPS.MOTIVO_PROSTITUCION != null : false)
                            conductaParasocial.MotivoProstitucion = SelectEmi.EMI.EMI_HPS.MOTIVO_PROSTITUCION.DESCR;
                        //LstMotivosProstituye.Where(w => w.ID_MOTIVO == SelectEmi.EMI_HPS.PROSTITUYE_POR).FirstOrDefault().DESCR;
                        if (SelectEmi.EMI.EMI_HPS.PAGA_SEXUAL_HOMBRE != null)
                            conductaParasocial.PagabaSexoHombres = SelectEmi.EMI.EMI_HPS.PAGA_SEXUAL_HOMBRE.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_HPS.PAGA_SEXUAL_MUJER != null)
                            conductaParasocial.PagabaSexoMujeres = SelectEmi.EMI.EMI_HPS.PAGA_SEXUAL_MUJER.Equals("S") ? "SI" : "NO";
                    }
                    var actividades = new List<cActividadesEMI>();

                    foreach (var actividad in SelectEmi.EMI.EMI_ACTIVIDAD)
                    {
                        actividades.Add(
                            new cActividadesEMI
                            {
                                IdArea = actividad.ID_EMI_ACTIVIDAD,
                                Anio = actividad.ANO_ACTIVIDADES.HasValue ? actividad.ANO_ACTIVIDADES.ToString() : string.Empty,
                                Actividades = actividad.EMI_TIPO_ACTIVIDAD != null ? !string.IsNullOrEmpty(actividad.EMI_TIPO_ACTIVIDAD.DESCR) ? actividad.EMI_TIPO_ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty,
                                Duracion = actividad.DURACION_ACTIVIDADES,
                                ConclucionAbandono = actividad.EMI_ESTATUS_PROGRAMA != null ? !string.IsNullOrEmpty(actividad.EMI_ESTATUS_PROGRAMA.DESCR) ? actividad.EMI_ESTATUS_PROGRAMA.DESCR.Trim() : string.Empty : string.Empty,
                                NoPrograma = actividad.PROGRAMA_TERMINADO.HasValue ? actividad.PROGRAMA_TERMINADO.ToString() : string.Empty
                            });
                    }

                    var enfermedad = new cEnfermedadEMI();
                    if (SelectEmi.EMI.EMI_ENFERMEDAD != null)
                    {
                        enfermedad.DescrEnfermedad = SelectEmi.EMI.EMI_ENFERMEDAD.DESCR_ENFERMEDAD;

                        if (SelectEmi.EMI.EMI_ENFERMEDAD.APFISICA_INTEGRO != null)
                            enfermedad.Integro = SelectEmi.EMI.EMI_ENFERMEDAD.APFISICA_INTEGRO.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_ENFERMEDAD.APFISICA_LIMPIO != null)
                            enfermedad.Limpio = SelectEmi.EMI.EMI_ENFERMEDAD.APFISICA_LIMPIO.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_ENFERMEDAD.APFISICA_CONFORMADO != null)
                            enfermedad.Conformado = SelectEmi.EMI.EMI_ENFERMEDAD.APFISICA_CONFORMADO.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_ENFERMEDAD.APFISICA_ALINADO != null)
                            enfermedad.Aliniado = SelectEmi.EMI.EMI_ENFERMEDAD.APFISICA_ALINADO.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_ENFERMEDAD.DISCAPACIDAD != null)
                            enfermedad.Discapacidad = SelectEmi.EMI.EMI_ENFERMEDAD.DISCAPACIDAD.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_ENFERMEDAD.DESCR_ENFERMEDAD != null)
                            enfermedad.DescrDiscapacidad = SelectEmi.EMI.EMI_ENFERMEDAD.DESCR_ENFERMEDAD;
                        if (SelectEmi.EMI.EMI_ENFERMEDAD.ENFERMO_MENTAL != null)
                            enfermedad.EnfermoMental = SelectEmi.EMI.EMI_ENFERMEDAD.ENFERMO_MENTAL.Equals("S") ? "SI" : "NO";
                        if (SelectEmi.EMI.EMI_ENFERMEDAD.DESCR_ENFERMO_MENTAL != null)
                            enfermedad.DescrEM = SelectEmi.EMI.EMI_ENFERMEDAD.DESCR_ENFERMO_MENTAL;
                        if (SelectEmi.EMI.EMI_ENFERMEDAD.VIH_HEPATITIS != null)
                            enfermedad.VIH = SelectEmi.EMI.EMI_ENFERMEDAD.VIH_HEPATITIS.Equals("S") ? "SI" : "NO";

                        enfermedad.EnTratamiento = !string.IsNullOrEmpty(SelectEmi.EMI.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO) ? SelectEmi.EMI.EMI_ENFERMEDAD.EN_TRATAMIENTO_FARMACO.Equals("S") ? "SI" : "NO" : "NO";
                        enfermedad.DiagnosticoFormal = !string.IsNullOrEmpty(SelectEmi.EMI.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL) ? SelectEmi.EMI.EMI_ENFERMEDAD.DIAGNOSTICO_FORMAL.Equals("S") ? "SI" : "NO" : "NO";
                    }
                    var clasificacion = new cClasificacionCriminologicaEMI();
                    if (SelectEmi.EMI.EMI_CLAS_CRIMINOLOGICA != null)
                    {
                        clasificacion.CCPorAntecedentes = SelectEmi.EMI.EMI_CLAS_CRIMINOLOGICA.CLASIFICACION_CRIMINOLOGICA != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_CLAS_CRIMINOLOGICA.CLASIFICACION_CRIMINOLOGICA.DESCR) ? SelectEmi.EMI.EMI_CLAS_CRIMINOLOGICA.CLASIFICACION_CRIMINOLOGICA.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.PerteneceCrimenOrganizado = SelectEmi.EMI.EMI_CLAS_CRIMINOLOGICA.PERTENECE_CRIMEN_ORG != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_CLAS_CRIMINOLOGICA.PERTENECE_CRIMEN_ORG.DESCR) ? SelectEmi.EMI.EMI_CLAS_CRIMINOLOGICA.PERTENECE_CRIMEN_ORG.DESCR.Trim() : string.Empty : string.Empty;
                    }

                    if (SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO != null)
                    {
                        clasificacion.Egocentrismo = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.Agresividad = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL1 != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL1.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL1.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.IndiferenciaAfectiva = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL2 != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL2.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL2.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.LabilidadAfectiva = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL3 != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL3.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL3.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.AdaptabilidadSocial = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL4 != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL4.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL4.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.AdaptabilidadSocial = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL5 != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL5.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL5.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.Liderazgo = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL6 != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL6.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL6.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.ToleranciaFrustracion = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL7 != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL7.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL7.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.ControlImpulsos = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL8 != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL8.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL8.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.PronosticoIntraInstitucional = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL9 != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL9.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_NIVEL9.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.UbicacionPorCC = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_UBICACION != null ? !string.IsNullOrEmpty(SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_UBICACION.DESCR) ? SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.EMI_FACTOR_UBICACION.DESCR.Trim() : string.Empty : string.Empty;
                        clasificacion.Dictamen = SelectEmi.EMI.EMI_FACTOR_CRIMINODIAGNOSTICO.DICTAMEN;
                    }
                    var sanciones = new List<cSancionesDiciplinariasEMI>();
                    foreach (var sanc in SelectEmi.EMI.EMI_SANCION_DISCIPLINARIA)
                    {
                        sanciones.Add(new cSancionesDiciplinariasEMI()
                        {
                            MotivoProceso = sanc.MOTIVO_PROCESO,
                            Cantidad = sanc.CANTIDAD_PARTICIPACION.HasValue ? sanc.CANTIDAD_PARTICIPACION.ToString() : string.Empty,
                            NuevoProceso = sanc.NUEVO_PROCESO.Equals("N") ? "NO" : "SI",
                            TiempoProceso = sanc.TIEMPO_CASTIGO_SANCION_PROCESO
                        });
                    }
                    //TATUAJES
                    var tatuajes = new List<cTatuajesEMI>();
                    if (SelectEmi.EMI.EMI_TATUAJE != null)
                    {
                        var tatu = new cTatuajesEMI();
                        //ANTES DE ENTRAR
                        tatu.AntisocialAE = (SelectEmi.EMI.EMI_TATUAJE.ANTISOCIAL_AI != null ? SelectEmi.EMI.EMI_TATUAJE.ANTISOCIAL_AI : 0).ToString();
                        tatu.EroticoAE = (SelectEmi.EMI.EMI_TATUAJE.EROTICO_AI != null ? SelectEmi.EMI.EMI_TATUAJE.EROTICO_AI : 0).ToString();
                        tatu.ReligiosolAE = (SelectEmi.EMI.EMI_TATUAJE.RELIGIOSO_AI != null ? SelectEmi.EMI.EMI_TATUAJE.RELIGIOSO_AI : 0).ToString();
                        tatu.IdentificacionAE = (SelectEmi.EMI.EMI_TATUAJE.IDENTIFICACION_AI != null ? SelectEmi.EMI.EMI_TATUAJE.IDENTIFICACION_AI : 0).ToString();
                        tatu.DecorativoAE = (SelectEmi.EMI.EMI_TATUAJE.DECORATIVO_AI != null ? SelectEmi.EMI.EMI_TATUAJE.DECORATIVO_AI : 0).ToString();
                        tatu.SentimentalAE = (SelectEmi.EMI.EMI_TATUAJE.SENTIMENTAL_AI != null ? SelectEmi.EMI.EMI_TATUAJE.SENTIMENTAL_AI : 0).ToString();
                        //INTRAMUROS
                        tatu.AntisocialI = (SelectEmi.EMI.EMI_TATUAJE.ANTISOCIAL_I != null ? SelectEmi.EMI.EMI_TATUAJE.ANTISOCIAL_I : 0).ToString();
                        tatu.EroticoI = (SelectEmi.EMI.EMI_TATUAJE.EROTICO_I != null ? SelectEmi.EMI.EMI_TATUAJE.EROTICO_I : 0).ToString();
                        tatu.ReligiosolI = (SelectEmi.EMI.EMI_TATUAJE.RELIGIOSO_I != null ? SelectEmi.EMI.EMI_TATUAJE.RELIGIOSO_I : 0).ToString();
                        tatu.IdentificacionI = (SelectEmi.EMI.EMI_TATUAJE.IDENTIFICACION_I != null ? SelectEmi.EMI.EMI_TATUAJE.IDENTIFICACION_I : 0).ToString();
                        tatu.DecorativoI = (SelectEmi.EMI.EMI_TATUAJE.DECORATIVO_I != null ? SelectEmi.EMI.EMI_TATUAJE.DECORATIVO_I : 0).ToString();
                        tatu.SentimentalI = (SelectEmi.EMI.EMI_TATUAJE.SENTIMENTAL_I != null ? SelectEmi.EMI.EMI_TATUAJE.SENTIMENTAL_I : 0).ToString();
                        //TOTAL
                        tatu.TotalTatuajes = (SelectEmi.EMI.EMI_TATUAJE.TOTAL_TATUAJES != null ? SelectEmi.EMI.EMI_TATUAJE.TOTAL_TATUAJES : 0).ToString();
                        //DESCRIPCION
                        tatu.DescripcionTatuajes = SelectEmi.EMI.EMI_TATUAJE.DESCR;

                        tatuajes.Add(tatu);
                    }
                    var tatuiajes = new List<cSancionesDiciplinariasEMI>();

                    //USO DE DROGAS IMPUTADO
                    var usoDrogas = new List<UsoDrogasEMI>();
                    string fecha_ultima_dosis = "";
                    if (SelectEmi.EMI.EMI_USO_DROGA != null)
                    {
                        foreach (var ud in SelectEmi.EMI.EMI_USO_DROGA)
                        {
                            string tc = string.Empty;
                            switch (ud.TIEMPO_CONSUMO)
                            {
                                case 1:
                                    tc = "NINGUNO";
                                    break;
                                case 2:
                                    tc = "MENOR A 1 AÑO";
                                    break;
                                case 3:
                                    tc = "DE 1 A 5 AÑOS";
                                    break;
                                case 4:
                                    tc = "DE 5 A 10 AÑOS";
                                    break;
                                case 5:
                                    tc = "MAYOR A 10 AÑOS";
                                    break;
                            }
                            if (ud.FEC_ULTIMA_DOSIS != null)
                                fecha_ultima_dosis = ud.FEC_ULTIMA_DOSIS.Value.ToString("dd/MM/yyyy");
                            usoDrogas.Add(new UsoDrogasEMI()
                            {
                                Droga = ud.DROGA != null ? !string.IsNullOrEmpty(ud.DROGA.DESCR) ? ud.DROGA.DESCR.Trim() : string.Empty : string.Empty,
                                EdadInicio = ud.EDAD_INICIO.HasValue ? ud.EDAD_INICIO.ToString() : string.Empty,
                                FecUltimaDosis = fecha_ultima_dosis,
                                FrecuenciaUso = ud.DROGA_FRECUENCIA != null ? !string.IsNullOrEmpty(ud.DROGA_FRECUENCIA.DESCR) ? ud.DROGA_FRECUENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                ConsumoActual = ud.CONSUMO_ACTUAL.Equals("S") ? "SI" : "NO",
                                TiempoConsumo = tc
                            });
                        }
                    }
                    var reporte = new List<cReporte>();
                    reporte.Add(new cReporte()
                    {
                        Encabezado1 = !string.IsNullOrEmpty(Parametro.ENCABEZADO1) ? Parametro.ENCABEZADO1.Trim() : string.Empty,
                        Encabezado2 = !string.IsNullOrEmpty(Parametro.ENCABEZADO2) ? Parametro.ENCABEZADO2.Trim() : string.Empty,
                        Encabezado3 = SelectIngreso.CENTRO != null ? !string.IsNullOrEmpty(SelectIngreso.CENTRO.DESCR) ? SelectIngreso.CENTRO.DESCR.Trim().ToUpper() : string.Empty : string.Empty,
                        Encabezado4 = "ENTREVISTA MULTIDISCIPLINARIA INICIAL",
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2
                    });

                    //ARMAMOS EL REPORTE
                    //Path.Combine(Application.StartupPath, "Reports", "report.rpt").
                    var wpf = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                    view.Report.LocalReport.ReportPath = "Reportes/rEMI.rdlc";
                    view.Report.LocalReport.DataSources.Clear();

                    //FICHA
                    var ds1 = new List<cFichaEMI>();
                    ds1.Add(ficha);
                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = ds1;
                    view.Report.LocalReport.DataSources.Add(rds1);

                    //ULTIMOS EMPLEOS
                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = empleos;
                    view.Report.LocalReport.DataSources.Add(rds2);

                    //DELITO
                    Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds3.Name = "DataSet3";
                    rds3.Value = delitos;
                    view.Report.LocalReport.DataSources.Add(rds3);

                    //SITUACION JURIDICA
                    var ds4 = new List<cSituacionJuridicaEMI>();
                    ds4.Add(juridico);
                    Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds4.Name = "DataSet4";
                    rds4.Value = ds4;
                    view.Report.LocalReport.DataSources.Add(rds4);

                    //INGRESOS CENTRO
                    Microsoft.Reporting.WinForms.ReportDataSource rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds5.Name = "DataSet5";
                    rds5.Value = ingresos;
                    view.Report.LocalReport.DataSources.Add(rds5);

                    //FACTORES SOCIOFAMILIARES
                    var ds6 = new List<cSocioFamiliaresEMI>();
                    ds6.Add(sociofamiliares);
                    Microsoft.Reporting.WinForms.ReportDataSource rds6 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds6.Name = "DataSet6";
                    rds6.Value = ds6;
                    view.Report.LocalReport.DataSources.Add(rds6);

                    //GRUPO FAMILIAR
                    Microsoft.Reporting.WinForms.ReportDataSource rds7 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds7.Name = "DataSet7";
                    rds7.Value = grupoFamiliar;
                    view.Report.LocalReport.DataSources.Add(rds7);

                    //FAMILIAR ANTECEDENTES
                    Microsoft.Reporting.WinForms.ReportDataSource rds8 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds8.Name = "DataSet8";
                    rds8.Value = familiarDelito;
                    view.Report.LocalReport.DataSources.Add(rds8);

                    //FAMILIAR DROGA
                    Microsoft.Reporting.WinForms.ReportDataSource rds9 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds9.Name = "DataSet9";
                    rds9.Value = familiarDroga;
                    view.Report.LocalReport.DataSources.Add(rds9);

                    //CONDUCTAS PARASOCIALES
                    var ds10 = new List<cConductaParasocialEMI>();
                    ds10.Add(conductaParasocial);
                    Microsoft.Reporting.WinForms.ReportDataSource rds10 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds10.Name = "DataSet10";
                    rds10.Value = ds10;
                    view.Report.LocalReport.DataSources.Add(rds10);

                    //ACTIVIDADES
                    Microsoft.Reporting.WinForms.ReportDataSource rds11 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds11.Name = "DataSet11";
                    rds11.Value = actividades;
                    view.Report.LocalReport.DataSources.Add(rds11);

                    //ENFERMEDADES
                    var ds12 = new List<cEnfermedadEMI>();
                    ds12.Add(enfermedad);
                    Microsoft.Reporting.WinForms.ReportDataSource rds12 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds12.Name = "DataSet12";
                    rds12.Value = ds12;
                    view.Report.LocalReport.DataSources.Add(rds12);

                    //CLASIFICACION CRIMINOLOGICA
                    var ds13 = new List<cClasificacionCriminologicaEMI>();
                    ds13.Add(clasificacion);
                    Microsoft.Reporting.WinForms.ReportDataSource rds13 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds13.Name = "DataSet13";
                    rds13.Value = ds13;
                    view.Report.LocalReport.DataSources.Add(rds13);

                    //SANCIONES
                    Microsoft.Reporting.WinForms.ReportDataSource rds14 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds14.Name = "DataSet14";
                    rds14.Value = sanciones;
                    view.Report.LocalReport.DataSources.Add(rds14);

                    //INGRESOS MENORES
                    Microsoft.Reporting.WinForms.ReportDataSource rds15 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds15.Name = "DataSet15";
                    rds15.Value = ingresosMenores;
                    view.Report.LocalReport.DataSources.Add(rds15);

                    //GENERALES EMI
                    var _centro = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    var ds16 = new List<cGeneralesEMI>();
                    ds16.Add(new cGeneralesEMI()
                    {
                        Centro = _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty,
                        Dictaminador = string.Empty
                    });

                    Microsoft.Reporting.WinForms.ReportDataSource rds16 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds16.Name = "DataSet16";
                    rds16.Value = ds16;
                    view.Report.LocalReport.DataSources.Add(rds16);

                    //CAUSA PENAL EMI
                    var ds17 = new List<cCausaPenalEMI>();
                    ds17.Add(new cCausaPenalEMI()
                    {
                        AniosSentencia = AniosS.HasValue ? AniosS.ToString() : string.Empty,
                        MesesSentencia = MesesS.HasValue ? MesesS.ToString() : string.Empty,
                        DiasSentencia = DiasS.HasValue ? DiasS.ToString() : string.Empty,
                        AniosCompurgados = AniosCumplidoI.ToString(),
                        MesesCompurgados = MesesCumplidoI.ToString(),
                        DiasCompurgados = DiasCumplidoI.ToString(),
                        AniosPorCompurgar = AniosRestanteI.ToString(),
                        MesesPorCompurgar = MesesRestanteI.ToString(),
                        DiasPorCompurgar = DiasRestanteI.ToString(),
                        Fuero = string.Empty,
                        Delito = Delitos
                    });

                    Microsoft.Reporting.WinForms.ReportDataSource rds17 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds17.Name = "DataSet17";
                    rds17.Value = ds17;
                    view.Report.LocalReport.DataSources.Add(rds17);

                    //TATUAJES
                    Microsoft.Reporting.WinForms.ReportDataSource rds18 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds18.Name = "DataSet18";
                    rds18.Value = tatuajes;
                    view.Report.LocalReport.DataSources.Add(rds18);

                    //USO DE DROGAS
                    //TATUAJES
                    Microsoft.Reporting.WinForms.ReportDataSource rds19 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds19.Name = "DataSet19";
                    rds19.Value = usoDrogas;
                    view.Report.LocalReport.DataSources.Add(rds19);

                    //reporte
                    Microsoft.Reporting.WinForms.ReportDataSource rds20 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds20.Name = "DataSet20";
                    rds20.Value = reporte;
                    view.Report.LocalReport.DataSources.Add(rds20);

                    view.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    view.Report.RefreshReport();
                }
                SelectEmi = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir emi", ex);
            }
        }

        #region Agenda
        private void AgendaLoaded(object sender, EventArgs e)
        {
            try
            {
                AgendaInternoView.DataContext = this;
                ObtenerAgenda();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la agenda.", ex);
            }
        }

        private async void ObtenerAgenda()
        {
            try
            {
                //await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                //{
                //IngresoSeleccionado.ATENCION_INGRESO = null;
                //IngresoSeleccionado.ATENCION_INGRESO = new cAtencionIngreso().ObtenerTodo(GlobalVar.gCentro,false,IngresoSeleccionado.ID_CENTRO, IngresoSeleccionado.ID_ANIO, IngresoSeleccionado.ID_IMPUTADO).ToList();
                //LstAgendaInterno.InsertRange(IngresoSeleccionado.ATENCION_CITA.Where(w => w.ATENCION_SOLICITUD.ATENCION_CITA.Any()).Select(s => s.ATENCION_SOLICITUD.ATENCION_CITA).Select(s => new Appointment()
                //{
                //    Subject = string.Format("{0}", "ACTIVIDAD: " + s.FirstOrDefault().ATENCION_SOLICITUD.ACTIVIDAD +
                //        "\nAREA: " + s.FirstOrDefault().ATENCION_SOLICITUD.AREA.DESCR),
                //    StartTime = s.FirstOrDefault().CITA_FECHA_HORA.HasValue ? s.FirstOrDefault().CITA_FECHA_HORA.Value : new DateTime(),
                //    EndTime = s.FirstOrDefault().CITA_HORA_TERMINA.HasValue ? s.FirstOrDefault().CITA_HORA_TERMINA.Value : new DateTime(),
                //}));
                /*LstAgendaInterno.InsertRange(IngresoSeleccionado.ATENCION_INGRESO.Where(w => !w.ATENCION_CITA.Any()).Select(s => new Appointment()
                {
                    Subject = "ACTIVIDAD: " + s.ATENCION_SOLICITUD.ACTIVIDAD +
                        "\nAREA: " + s.ATENCION_SOLICITUD.AREA.DESCR,
                    StartTime = s.ATENCION_SOLICITUD.SOLICITUD_FEC.HasValue ? s.ATENCION_SOLICITUD.SOLICITUD_FEC.Value : new DateTime(),
                    EndTime = s.ATENCION_SOLICITUD.SOLICITUD_FEC.HasValue ? s.ATENCION_SOLICITUD.SOLICITUD_FEC.Value.AddHours(1) : new DateTime(),
                     Body="_"
                }));*/
                LstAgendaInterno = new ObservableCollection<Appointment>();
                var agenda = new cAtencionCita().ObtenerPorInternoDesdeFecha(GlobalVar.gCentro, IngresoSeleccionado.ID_IMPUTADO);
                if (agenda != null)
                {
                    foreach (var a in agenda)
                    {
                        LstAgendaInterno.Add(new Appointment()
                        {
                            Subject = string.Format("ÁREA CITA: {0}", !string.IsNullOrEmpty(a.AREA.DESCR) ? a.AREA.DESCR.Trim() : ""),
                            StartTime = new DateTime(a.CITA_FECHA_HORA.Value.Year, a.CITA_FECHA_HORA.Value.Month, a.CITA_FECHA_HORA.Value.Day, a.CITA_FECHA_HORA.Value.Hour, a.CITA_FECHA_HORA.Value.Minute, a.CITA_FECHA_HORA.Value.Second),
                            EndTime = new DateTime(a.CITA_HORA_TERMINA.Value.Year, a.CITA_HORA_TERMINA.Value.Month, a.CITA_HORA_TERMINA.Value.Day, a.CITA_HORA_TERMINA.Value.Hour, a.CITA_HORA_TERMINA.Value.Minute, a.CITA_HORA_TERMINA.Value.Second),
                        });
                    }
                }

                var _CitasPersonalidadDias = new cPersonalidadDetalleDias().GetData(x => x.PERSONALIDAD_DETALLE != null && x.PERSONALIDAD_DETALLE.PERSONALIDAD != null && x.PERSONALIDAD_DETALLE.PERSONALIDAD.INGRESO != null && x.PERSONALIDAD_DETALLE.PERSONALIDAD.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro && x.ID_INGRESO == IngresoSeleccionado.ID_INGRESO && x.ID_IMPUTADO == IngresoSeleccionado.ID_IMPUTADO && x.ID_ANIO == IngresoSeleccionado.ID_ANIO);
                if (_CitasPersonalidadDias != null && _CitasPersonalidadDias.Any())
                    foreach (var item in _CitasPersonalidadDias)
                    {
                        LstAgendaInterno.Add(new Appointment()
                        {
                            Subject = string.Format("{0}, {1} ",
                            item.PERSONALIDAD_DETALLE != null ? item.PERSONALIDAD_DETALLE.PERSONALIDAD_TIPO_ESTUDIO != null ? !string.IsNullOrEmpty(item.PERSONALIDAD_DETALLE.PERSONALIDAD_TIPO_ESTUDIO.DESCR) ? item.PERSONALIDAD_DETALLE.PERSONALIDAD_TIPO_ESTUDIO.DESCR.Trim() : " " : " " : " ",
                            item.ID_AREA.HasValue ? !string.IsNullOrEmpty(item.AREA.DESCR) ? item.AREA.DESCR.Trim() : "" : ""),
                            StartTime = item.FECHA_INICIO.HasValue ? new DateTime(item.FECHA_INICIO.Value.Year, item.FECHA_INICIO.Value.Month, item.FECHA_INICIO.Value.Day, item.FECHA_INICIO.Value.Hour, item.FECHA_INICIO.Value.Minute, item.FECHA_INICIO.Value.Second) : new DateTime(),
                            EndTime = item.FECHA_FINAL.HasValue ? new DateTime(item.FECHA_FINAL.Value.Year, item.FECHA_FINAL.Value.Month, item.FECHA_FINAL.Value.Day, item.FECHA_FINAL.Value.Hour, item.FECHA_FINAL.Value.Minute, item.FECHA_FINAL.Value.Second) : new DateTime()
                        });
                    }

                RaisePropertyChanged("LstAgendaInterno");
                //});
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener agenda.", ex);
            }
        }
        #endregion

        private void HideAll()
        {
            IdentificacionVisible = Visibility.Collapsed;
            JuridicoVisible = Visibility.Collapsed;
            AdministrativoVisible = Visibility.Collapsed;
            EstudiosVisible = Visibility.Collapsed;
            VisitasVisible = Visibility.Collapsed;
            AgendaVisible = Visibility.Collapsed;
            ExpedienteFisicoVisible = Visibility.Collapsed;
            VisibleDatosMedicos = Visibility.Collapsed;
            VisibleDatosKardex = Visibility.Collapsed;
            VisibleDocumentosHistorico = Visibility.Collapsed;
        }

        private void LimpiarTodo()
        {
            try
            {
                #region Imputado

                #region DatosGenerales
                SelectEtnia = SelectEscolaridad = SelectOcupacion = SelectEstadoCivil = SelectNacionalidad = SelectReligion = -1;
                SelectSexo = "S";
                TextPeso = TextEstatura = string.Empty;
                #endregion

                #region Domicilio
                SelectPais = Parametro.PAIS;// 82;
                SelectEntidad = Parametro.ESTADO;// 2;
                SelectMunicipio = -1;
                SelectColoniaItem = ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();
                TextCalle = TextNumeroInterior = AniosEstado = MesesEstado = TextDomicilioTrabajo = string.Empty;
                FechaEstado = Fechas.GetFechaDateServer;
                TextNumeroExterior = null;
                TextTelefono = null;
                TextCodigoPostal = null;
                #endregion

                #region Nacimiento
                SelectPaisNacimiento = Parametro.PAIS;//82;
                SelectEntidadNacimiento = SelectMunicipioNacimiento = -1;
                TextFechaNacimiento = Fechas.GetFechaDateServer;
                TextLugarNacimientoExtranjero = string.Empty;
                #endregion

                #region Padre
                TextPadreMaterno = TextPadrePaterno = TextPadreNombre = string.Empty;
                CheckPadreFinado = false;

                #region Domicilio
                SelectPaisDomicilioPadre = SelectEntidadDomicilioPadre = SelectMunicipioDomicilioPadre = -1;
                SelectColoniaItemDomicilioPadre = ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();
                TextCalleDomicilioPadre = TextNumeroInteriorDomicilioPadre = string.Empty;
                TextNumeroExteriorDomicilioPadre = null;
                TextCodigoPostalDomicilioPadre = null;
                #endregion

                #endregion

                #region Madre
                TextMadreMaterno = TextMadrePaterno = TextMadreNombre = string.Empty;
                CheckMadreFinado = false;

                #region Domicilio
                SelectPaisDomicilioMadre = SelectEntidadDomicilioMadre = SelectMunicipioDomicilioMadre = -1;
                SelectColoniaItemDomicilioMadre = ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();
                TextCalleDomicilioMadre = TextNumeroInteriorDomicilioMadre = string.Empty;
                TextNumeroExteriorDomicilioMadre = null;
                TextCodigoPostalDomicilioMadre = null;
                #endregion

                #endregion

                #endregion

                #region Ingreso
                #endregion

                #region CausaPenal
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar los campos.", ex);
            }
        }

        private void BuscarInternoPopup(Object obj)
        {
            try
            {
                BuscarImputado();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private async void BuscarImputado()
        {
            try
            {
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count == 1 ? (AnioBuscar != null && FolioBuscar != null) : false)
                    SelectExpediente = ListExpediente.FirstOrDefault();
                EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA_EXPEDIENTE);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la búsqueda.", ex);
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
                    new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                Pagina = result.Any() ? Pagina + 1 : Pagina;
                SeguirCargando = result.Any();
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
                return new List<IMPUTADO>();
            }
        }

        #region Cargar Datos Imputado
        private void GetDatosImputadoSeleccionado()
        {
            try
            {
                getDatosGeneralesIdentificacion();
                //GetDatosMedicosFormatos();
                ListAlias = new ObservableCollection<ALIAS>(ImputadoSeleccionado.ALIAS);
                ListApodo = new ObservableCollection<APODO>(ImputadoSeleccionado.APODO);
                var _listaTemp = new ObservableCollection<RELACION_PERSONAL_INTERNO>(ImputadoSeleccionado.RELACION_PERSONAL_INTERNO);
                ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>();
                if (_listaTemp != null && _listaTemp.Any())
                    foreach (var item in _listaTemp)
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            ListRelacionPersonalInterno.Add(new RELACION_PERSONAL_INTERNO
                            {
                                INGRESO = item.INGRESO
                            });
                        }));

                ImputadoPandilla = new ObservableCollection<IMPUTADO_PANDILLA>(ImputadoSeleccionado.IMPUTADO_PANDILLA);
                TextNombreCompleto = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(ImputadoSeleccionado.PATERNO) ? ImputadoSeleccionado.PATERNO.Trim() : string.Empty,
                    !string.IsNullOrEmpty(ImputadoSeleccionado.MATERNO) ? ImputadoSeleccionado.MATERNO.Trim() : string.Empty,
                    !string.IsNullOrEmpty(ImputadoSeleccionado.NOMBRE) ? ImputadoSeleccionado.NOMBRE.Trim() : string.Empty);

                TextExpediente = ImputadoSeleccionado.ID_ANIO + "/" + ImputadoSeleccionado.ID_IMPUTADO;
                ListSenasParticulares = new ObservableCollection<SENAS_PARTICULARES>(ImputadoSeleccionado.SENAS_PARTICULARES);

                LstCustomizadaIngresos = new System.Collections.ObjectModel.ObservableCollection<CustomIngresos>();
                var _Ingresos = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerIngresos(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO);
                if (_Ingresos != null && _Ingresos.Any())
                    foreach (var item in _Ingresos)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            LstCustomizadaIngresos.Add(new CustomIngresos
                            {
                                DescripcionIngreso = "INGRESO " + item.ID_INGRESO,
                                IdIngreso = item.ID_INGRESO
                            });
                        }));
                    };

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstCustomizadaIngresos.Insert(0, new CustomIngresos() { DescripcionIngreso = "TODOS", IdIngreso = -1 });
                    ImprimeHCM();
                    ImprimeHistoriaClinicaDental();
                }));

                SelectedIngresoBusquedas = -1;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado.", ex);
            }
        }

        private async void GetDatosIngresoSeleccionado()
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ListVisitantes = new ObservableCollection<VISITANTE_INGRESO>(IngresoSeleccionado.VISITANTE_INGRESO.OrderBy(o => o.ID_PERSONA));
                    SeleccionaIngreso();
                    GetInformacionAdministrativa();
                    GetVisitasAgendadas();
                    GetEstudios();
                    GetEmi();
                    GetDocumentosExpediente();

                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ImprimeHCM();
                        ImprimeHistoriaClinicaDental();
                    }));
                });

                //GetDatosMedicosFormatos();
                if (GeneraReporteDatos())
                    ReportViewer_Requisicion();

                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado.", ex);
            }
        }

        private void getDatosGeneralesIdentificacion()
        {
            try
            {
                #region DatosGenerales
                SelectEtnia = ImputadoSeleccionado.ID_ETNIA ?? -1;
                SelectEscolaridad = SelectIngreso.ID_ESCOLARIDAD ?? -1;
                SelectOcupacion = SelectIngreso.ID_OCUPACION ?? -1;
                SelectEstadoCivil = SelectIngreso.ID_ESTADO_CIVIL ?? -1;
                SelectNacionalidad = ImputadoSeleccionado.ID_NACIONALIDAD ?? -1;
                SelectReligion = SelectIngreso.ID_RELIGION ?? -1;
                SelectSexo = string.IsNullOrEmpty(ImputadoSeleccionado.SEXO) ? "S" : ImputadoSeleccionado.SEXO;
                SelectedDialecto = ImputadoSeleccionado.ID_DIALECTO ?? -1;
                SelectedIdioma = ImputadoSeleccionado.ID_IDIOMA ?? -1;
                RequiereTraductor = ImputadoSeleccionado.TRADUCTOR == "S";
                TextPeso = SelectIngreso.PESO.HasValue ? SelectIngreso.PESO.ToString() : "";
                TextEstatura = SelectIngreso.ESTATURA.HasValue ? SelectIngreso.ESTATURA.ToString() : "";
                #endregion

                #region Domicilio

                TextCalle = SelectIngreso.DOMICILIO_CALLE;
                TextNumeroExterior = SelectIngreso.DOMICILIO_NUM_EXT;
                TextNumeroInterior = SelectIngreso.DOMICILIO_NUM_INT;
                if ((SelectIngreso.RESIDENCIA_ANIOS.HasValue || SelectIngreso.RESIDENCIA_ANIOS.HasValue) ?
                    (SelectIngreso.RESIDENCIA_ANIOS.Value > 0 || SelectIngreso.RESIDENCIA_ANIOS.Value > 0) : false)
                {
                    AniosEstado = SelectIngreso.RESIDENCIA_ANIOS.ToString();
                    int anio = 0;
                    int.TryParse(AniosEstado, out anio);
                    MesesEstado = SelectIngreso.RESIDENCIAS_MESES.HasValue ? SelectIngreso.RESIDENCIAS_MESES.ToString() : "";
                    int mes = 0;
                    int.TryParse(MesesEstado, out mes);
                    FechaEstado = Fechas.GetFechaDateServer.AddYears(-(anio)).AddMonths(-(mes));
                }
                else
                {
                    AniosEstado = MesesEstado = "0";
                    FechaEstado = new DateTime?();
                }

                TextTelefono = SelectIngreso.TELEFONO.HasValue ? SelectIngreso.TELEFONO.ToString() : null;
                TextCodigoPostal = SelectIngreso.DOMICILIO_CP.HasValue ? SelectIngreso.DOMICILIO_CP.Value : new int?();
                TextDomicilioTrabajo = SelectIngreso.DOMICILIO_TRABAJO;

                SelectPais = SelectIngreso.ID_PAIS == null ? 82 : SelectIngreso.ID_PAIS < 1 ? 82 : SelectIngreso.ID_PAIS;
                SelectEntidad = SelectIngreso.ID_ENTIDAD == null ? 2 : SelectIngreso.ID_ENTIDAD < 1 ? 2 : SelectIngreso.ID_ENTIDAD;

                #endregion

                #region Nacimiento
                TextEdad = ImputadoSeleccionado.NACIMIENTO_FECHA.HasValue ? new Fechas().CalculaEdad(ImputadoSeleccionado.NACIMIENTO_FECHA.Value) : 0;
                TextFechaNacimiento = ImputadoSeleccionado.NACIMIENTO_FECHA;
                TextLugarNacimientoExtranjero = ImputadoSeleccionado.NACIMIENTO_LUGAR;
                SelectMunicipioNacimiento = !ImputadoSeleccionado.NACIMIENTO_MUNICIPIO.HasValue ? -1 : ImputadoSeleccionado.NACIMIENTO_MUNICIPIO < 1 ? -1 : ImputadoSeleccionado.NACIMIENTO_MUNICIPIO;
                #endregion

                #region Padres
                getDatosDomiciliosPadres();
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }

        private async void getDatosDomiciliosPadres()
        {
            try
            {
                TextPadreMaterno = ImputadoSeleccionado.MATERNO_PADRE;
                TextPadrePaterno = ImputadoSeleccionado.PATERNO_PADRE;
                TextPadreNombre = ImputadoSeleccionado.NOMBRE_PADRE;
                CheckPadreFinado = SelectIngreso.PADRE_FINADO == "S";

                TextMadreMaterno = ImputadoSeleccionado.MATERNO_MADRE;
                TextMadrePaterno = ImputadoSeleccionado.PATERNO_MADRE;
                TextMadreNombre = ImputadoSeleccionado.NOMBRE_MADRE;
                CheckMadreFinado = SelectIngreso.MADRE_FINADO == "S";

                #region Padres
                if (ImputadoSeleccionado.IMPUTADO_PADRES.Count > 0)
                {
                    bool padreExiste = false, madreExiste = false;
                    foreach (var item in ImputadoSeleccionado.IMPUTADO_PADRES)
                    {
                        if (item.ID_PADRE == "P")
                        {
                            ListMunicipioDomicilioPadre = new ObservableCollection<MUNICIPIO>();
                            if (item.ID_MUNICIPIO.HasValue ? item.MUNICIPIO != null : false)
                                ListMunicipioDomicilioPadre.Insert(0, item.MUNICIPIO);
                            else
                                ListMunicipioDomicilioPadre.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SIN DATO" });

                            ListColoniaDomicilioPadre = new ObservableCollection<COLONIA>();
                            if (item.ID_COLONIA.HasValue)
                                ListColoniaDomicilioPadre.Insert(0, item.COLONIA);
                            else
                                ListColoniaDomicilioPadre.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SIN DATO" });

                            SelectPaisDomicilioPadre = item.ID_PAIS;//== null ? 82 : item.ID_PAIS < 1 ? 82 : item.ID_PAIS;
                            //await TaskEx.Delay(100);
                            SelectEntidadDomicilioPadre = item.ID_ENTIDAD; //== null ? 2 : item.ID_ENTIDAD < 1 ? 2 : item.ID_ENTIDAD;
                            //await TaskEx.Delay(100);
                            TextCalleDomicilioPadre = item.CALLE;
                            TextNumeroExteriorDomicilioPadre = item.NUM_EXT;
                            TextNumeroInteriorDomicilioPadre = item.NUM_INT;
                            TextCodigoPostalDomicilioPadre = item.CP;
                            padreExiste = true;
                            await TaskEx.Delay(150);
                            SelectMunicipioDomicilioPadre = item.ID_MUNICIPIO; //== null ? -1 : item.ID_MUNICIPIO < 1 ? -1 : item.ID_MUNICIPIO;
                            await TaskEx.Delay(150);
                            SelectColoniaDomicilioPadre = item.ID_COLONIA;
                            // == null ? -1 : item.ID_COLONIA < 1 ? -1 : item.ID_COLONIA;
                        }
                    }
                    MismoDomicilioPadre = ((!padreExiste) ? (!string.IsNullOrEmpty(TextPadreMaterno) || !string.IsNullOrEmpty(TextPadrePaterno) || !string.IsNullOrEmpty(TextPadreNombre)) ?
                    SelectIngreso.PADRE_FINADO == "N" : false : false);

                    foreach (var item in ImputadoSeleccionado.IMPUTADO_PADRES)
                    {
                        if (item.ID_PADRE == "M")
                        {
                            ListMunicipioDomicilioMadre = new ObservableCollection<MUNICIPIO>();
                            if (item.ID_MUNICIPIO.HasValue ? item.MUNICIPIO != null : false)
                                ListMunicipioDomicilioMadre.Insert(0, item.MUNICIPIO);
                            else
                                ListMunicipioDomicilioMadre.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SIN DATO" });
                            ListColoniaDomicilioMadre = new ObservableCollection<COLONIA>();
                            if (item.ID_COLONIA.HasValue)
                                ListColoniaDomicilioMadre.Insert(0, item.COLONIA);
                            else
                                ListColoniaDomicilioMadre.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SIN DATO" });

                            SelectPaisDomicilioMadre = item.ID_PAIS == null ? 82 : item.ID_PAIS < 1 ? 82 : item.ID_PAIS;
                            //await TaskEx.Delay(100);
                            SelectEntidadDomicilioMadre = item.ID_ENTIDAD == null ? 2 : item.ID_ENTIDAD < 1 ? 2 : item.ID_ENTIDAD;
                            //await TaskEx.Delay(100);
                            TextCalleDomicilioMadre = item.CALLE;
                            TextNumeroExteriorDomicilioMadre = item.NUM_EXT;
                            TextNumeroInteriorDomicilioMadre = item.NUM_INT;
                            TextCodigoPostalDomicilioMadre = item.CP;
                            madreExiste = true;
                            await TaskEx.Delay(150);
                            SelectMunicipioDomicilioMadre = item.ID_MUNICIPIO == null ? -1 : item.ID_MUNICIPIO < 1 ? -1 : item.ID_MUNICIPIO;
                            await TaskEx.Delay(150);
                            SelectColoniaDomicilioMadre = item.ID_COLONIA == null ? -1 : item.ID_COLONIA < 1 ? -1 : item.ID_COLONIA;
                        }
                    }
                    MismoDomicilioMadre = ((!madreExiste) ? (!string.IsNullOrEmpty(TextMadreMaterno) || !string.IsNullOrEmpty(TextMadrePaterno) || !string.IsNullOrEmpty(TextMadreNombre)) ?
                    SelectIngreso.MADRE_FINADO == "N" : false : false);
                }
                else
                {
                    SelectPaisDomicilioPadre = Parametro.PAIS; //82;
                    SelectEntidadDomicilioPadre = Parametro.ESTADO;// 2;
                    TextCalleDomicilioPadre = TextNumeroInteriorDomicilioPadre = string.Empty;
                    TextNumeroExteriorDomicilioPadre = null;
                    TextCodigoPostalDomicilioPadre = null;
                    if (!string.IsNullOrEmpty(TextPadreMaterno) || !string.IsNullOrEmpty(TextPadrePaterno) || !string.IsNullOrEmpty(TextPadreNombre))
                    {
                        if (SelectIngreso.PADRE_FINADO == "N")//SI NO ESTA FINADO TIENEN EL MISMO DOMICILIO
                        {
                            MismoDomicilioPadre = true;
                        }
                    }

                    SelectPaisDomicilioMadre = Parametro.PAIS; //82;
                    SelectEntidadDomicilioMadre = Parametro.ESTADO;// 2;
                    TextCalleDomicilioMadre = TextNumeroInteriorDomicilioMadre = string.Empty;
                    TextNumeroExteriorDomicilioMadre = null;
                    TextCodigoPostalDomicilioMadre = null;
                    MismoDomicilioMadre = ((!string.IsNullOrEmpty(TextMadreMaterno) || !string.IsNullOrEmpty(TextMadrePaterno) || !string.IsNullOrEmpty(TextMadreNombre)) ?
                        SelectIngreso.MADRE_FINADO == "N" : false);
                }
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }

        private void OnLoad(FotosHuellasDigitalesView Window = null)
        {
            try
            {
                if (ImputadoSeleccionado == null)
                    return;
                if (ImputadoSeleccionado.INGRESO.Count == 0)
                    return;

                #region [Huellas Digitales]
                var myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 0;
                myDoubleAnimation.To = 240;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.3));
                myDoubleAnimation.AutoReverse = true;
                myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

                Storyboard.SetTargetName(myDoubleAnimation, "Ln");
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
                var myStoryboard = new Storyboard();
                myStoryboard.Children.Add(myDoubleAnimation);

                if (PopUpsViewModels.MainWindow.HuellasView != null)
                    Application.Current.Dispatcher.Invoke((Action)(delegate { myStoryboard.Begin(PopUpsViewModels.MainWindow.HuellasView.Ln); }));

                if (FindVisualChildren<System.Windows.Controls.Image>(Window).ToList().Any())
                    CargarHuellas();

                #endregion

                #region [Web Cam]
                CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                Window.FrontFace.Source = new Imagenes().ConvertByteToBitmap(ImputadoSeleccionado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Any(w =>
                    w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                        ImputadoSeleccionado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.First(w =>
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO : null);
                Window.LeftFace.Source = new Imagenes().ConvertByteToBitmap(ImputadoSeleccionado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Any(w =>
                    w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_IZQ_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                        ImputadoSeleccionado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.First(w =>
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_IZQ_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO : null);
                Window.RightFace.Source = new Imagenes().ConvertByteToBitmap(ImputadoSeleccionado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.Any(w =>
                    w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                        ImputadoSeleccionado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().INGRESO_BIOMETRICO.First(w =>
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO : null);
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla para capturar fotos y huellas.", ex);
            }
        }

        private void CargarHuellas()
        {
            try
            {
                if (ImputadoSeleccionado == null)
                    return;
                var LoadHuellas = new Thread((Init) =>
                {
                    var Huellas = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == ImputadoSeleccionado.ID_ANIO && w.ID_CENTRO == ImputadoSeleccionado.ID_CENTRO &&
                        w.ID_IMPUTADO == ImputadoSeleccionado.ID_IMPUTADO && (w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)).ToList();

                    foreach (var item in Huellas)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            switch ((enumTipoBiometrico)item.ID_TIPO_BIOMETRICO)
                            {
                                case enumTipoBiometrico.PULGAR_DERECHO:
                                    PulgarDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                    break;
                                case enumTipoBiometrico.INDICE_DERECHO:
                                    IndiceDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                    break;
                                case enumTipoBiometrico.MEDIO_DERECHO:
                                    MedioDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                    break;
                                case enumTipoBiometrico.ANULAR_DERECHO:
                                    AnularDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                    break;
                                case enumTipoBiometrico.MENIQUE_DERECHO:
                                    MeñiqueDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                    break;
                                case enumTipoBiometrico.PULGAR_IZQUIERDO:
                                    PulgarIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                    break;
                                case enumTipoBiometrico.INDICE_IZQUIERDO:
                                    IndiceIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                    break;
                                case enumTipoBiometrico.MEDIO_IZQUIERDO:
                                    MedioIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                    break;
                                case enumTipoBiometrico.ANULAR_IZQUIERDO:
                                    AnularIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                    break;
                                case enumTipoBiometrico.MENIQUE_IZQUIERDO:
                                    MeñiqueIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                    break;
                                default:
                                    break;
                            }
                        }));
                    }
                });

                LoadHuellas.Start();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar las huellas del imputado seleccionado.", ex);
            }
        }

        private System.Windows.Media.Brush ObtenerCalidad(int nNFIQ)
        {
            if (nNFIQ == 0)
                return new SolidColorBrush(Colors.White);
            if (nNFIQ == 3)
                return new SolidColorBrush(Colors.Yellow);
            if (nNFIQ == 4)
                return new SolidColorBrush(Colors.Red);
            return new SolidColorBrush(Colors.LightGreen);
        }

        private void SeleccionaIngreso()
        {
            try
            {
                //DATOS GENERALES
                AnioD = IngresoSeleccionado.ID_ANIO;
                FolioD = IngresoSeleccionado.ID_IMPUTADO;
                PaternoD = IngresoSeleccionado.IMPUTADO != null ? !string.IsNullOrEmpty(IngresoSeleccionado.IMPUTADO.PATERNO) ? IngresoSeleccionado.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                MaternoD = IngresoSeleccionado.IMPUTADO != null ? !string.IsNullOrEmpty(IngresoSeleccionado.IMPUTADO.MATERNO) ? IngresoSeleccionado.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                NombreD = IngresoSeleccionado.IMPUTADO != null ? !string.IsNullOrEmpty(IngresoSeleccionado.IMPUTADO.NOMBRE) ? IngresoSeleccionado.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                IngresosD = IngresoSeleccionado.ID_INGRESO;
                //NoControlD = SelectIngreso
                if (IngresoSeleccionado.CAMA != null)
                {
                    UbicacionD = UbicacionI = string.Format("{0}-{1}{2}-{3}",
                        IngresoSeleccionado.CAMA != null ? IngresoSeleccionado.CAMA.CELDA != null ? IngresoSeleccionado.CAMA.CELDA.SECTOR != null ? IngresoSeleccionado.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(IngresoSeleccionado.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? IngresoSeleccionado.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                        IngresoSeleccionado.CAMA != null ? IngresoSeleccionado.CAMA.CELDA != null ? IngresoSeleccionado.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(IngresoSeleccionado.CAMA.CELDA.SECTOR.DESCR) ? IngresoSeleccionado.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        IngresoSeleccionado.CAMA != null ? IngresoSeleccionado.CAMA.CELDA != null ? IngresoSeleccionado.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty,
                        IngresoSeleccionado.ID_UB_CAMA);
                }
                else
                    UbicacionD = UbicacionI = string.Empty;

                TipoSeguridadD = IngresoSeleccionado.TIPO_SEGURIDAD != null ? IngresoSeleccionado.TIPO_SEGURIDAD.DESCR : string.Empty;
                FecIngresoD = IngresoSeleccionado.FEC_INGRESO_CERESO.HasValue ? IngresoSeleccionado.FEC_INGRESO_CERESO.Value : new Nullable<DateTime>();
                ClasificacionJuridicaD = IngresoSeleccionado.CLASIFICACION_JURIDICA != null ? IngresoSeleccionado.CLASIFICACION_JURIDICA.DESCR : string.Empty;
                EstatusD = IngresoSeleccionado.ESTATUS_ADMINISTRATIVO != null ? IngresoSeleccionado.ESTATUS_ADMINISTRATIVO.DESCR : string.Empty;
                //DATOS INGRESO
                FecRegistroI = IngresoSeleccionado.FEC_REGISTRO != null ? IngresoSeleccionado.FEC_REGISTRO.Value : new Nullable<DateTime>();
                FecCeresoI = IngresoSeleccionado.FEC_INGRESO_CERESO != null ? IngresoSeleccionado.FEC_INGRESO_CERESO.Value : new Nullable<DateTime>();
                TipoI = IngresoSeleccionado.ID_TIPO_INGRESO.HasValue ? IngresoSeleccionado.ID_TIPO_INGRESO.Value : (short)-1;
                EstatusAdministrativoI = IngresoSeleccionado.ID_ESTATUS_ADMINISTRATIVO.HasValue ? IngresoSeleccionado.ID_ESTATUS_ADMINISTRATIVO.Value : (short)-1;
                ClasificacionI = IngresoSeleccionado.ID_CLASIFICACION_JURIDICA;
                NoOficioI = IngresoSeleccionado.DOCINTERNACION_NUM_OFICIO;
                AutoridadInternaI = IngresoSeleccionado.ID_AUTORIDAD_INTERNA.HasValue ? IngresoSeleccionado.ID_AUTORIDAD_INTERNA.Value : (short)-1;
                TipoSeguridadI = IngresoSeleccionado.ID_TIPO_SEGURIDAD;
                QuedaDisposicionI = IngresoSeleccionado.ID_DISPOSICION.HasValue ? IngresoSeleccionado.ID_DISPOSICION.Value : (short)-1;
                //DELITO
                if (IngresoSeleccionado.DELITO != null)
                    DelitoDescrI = !string.IsNullOrEmpty(IngresoSeleccionado.DELITO.DESCR) ? IngresoSeleccionado.DELITO.DESCR.Trim() : string.Empty;

                //EXPEDIENTE GOBIERNO
                FolioGobiernoI = IngresoSeleccionado.FOLIO_GOBIERNO;
                AnioGobiernoI = IngresoSeleccionado.ANIO_GOBIERNO;
                //OCULTAMOS PANTALLA BUSCAR

                TotalAnios = 0;
                TotalMeses = 0;
                TotalDias = 0;
                int a, m, d;
                a = m = d = 0;
                SENTENCIA sen;
                LstSentenciasIngresos = new List<SentenciaIngreso>();
                foreach (var cp in IngresoSeleccionado.CAUSA_PENAL)
                {
                    sen = cp.SENTENCIA.FirstOrDefault();
                    if (sen != null)
                    {
                        LstSentenciasIngresos.Add(new SentenciaIngreso
                        {
                            CausaPenal = string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO),
                            SentenciaAnios = sen.ANIOS,
                            SentenciaMeses = sen.MESES,
                            SentenciaDias = sen.DIAS,
                            Fuero = cp.CP_FUERO
                        });
                        a = sen.ANIOS != null ? sen.ANIOS.Value : 0;
                        m = sen.MESES != null ? sen.MESES.Value : 0;
                        d = sen.DIAS != null ? sen.DIAS.Value : 0;
                    }
                    else
                    {
                        LstSentenciasIngresos.Add(new SentenciaIngreso
                        {
                            CausaPenal = string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO),
                            SentenciaAnios = 0,
                            SentenciaMeses = 0,
                            SentenciaDias = 0,
                            Fuero = cp.CP_FUERO
                        });
                    }
                }
                while (d > 29)
                {
                    m++;
                    d = d - 30;
                }
                while (m > 11)
                {
                    a++;
                    m = m - 12;
                }
                TotalAnios = a;
                TotalMeses = m;
                TotalDias = d;
                //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_CAUSAS_PENALES);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }

        private async void GetDatosCausasPenales()
        {
            //await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            //{
            //PopulateIngreso();

            CalcularSentencia();
            PopulateCausaPenal();
            //});
        }

        private void PopulateCausaPenal()
        {
            try
            {
                //LimpiarCausaPenal();
                //LimpiarCoparticipe();
                //LimpiarSentencia();
                //LimpiarRecusrsos();
                if (SelectedCausaPenal != null)
                {
                    //AP
                    AgenciaAP = SelectedCausaPenal.ID_AGENCIA.HasValue ? SelectedCausaPenal.ID_AGENCIA.Value : (short)-1;
                    AnioAP = SelectedCausaPenal.AP_ANIO;
                    FolioAP = SelectedCausaPenal.AP_FOLIO;
                    AveriguacionPreviaAP = SelectedCausaPenal.AP_FORANEA;
                    FecAveriguacionAP = SelectedCausaPenal.AP_FEC_INICIO;
                    FecConsignacionAP = SelectedCausaPenal.AP_FEC_CONSIGNACION;
                    //CAUSA PENAL
                    AnioCP = SelectedCausaPenal.CP_ANIO;
                    FolioCP = SelectedCausaPenal.CP_FOLIO;
                    BisCP = SelectedCausaPenal.CP_BIS;
                    ForaneoCP = SelectedCausaPenal.CP_FORANEO;
                    TipoOrdenCP = SelectedCausaPenal.CP_TIPO_ORDEN != null ? SelectedCausaPenal.CP_TIPO_ORDEN : -1;
                    PaisJuzgadoCP = SelectedCausaPenal.CP_PAIS_JUZGADO != null ? SelectedCausaPenal.CP_PAIS_JUZGADO : -1;
                    EstadoJuzgadoCP = SelectedCausaPenal.CP_ESTADO_JUZGADO != null ? SelectedCausaPenal.CP_ESTADO_JUZGADO : -1;
                    MunicipioJuzgadoCP = SelectedCausaPenal.CP_MUNICIPIO_JUZGADO != null ? SelectedCausaPenal.CP_MUNICIPIO_JUZGADO : -1;
                    FueroCP = !string.IsNullOrEmpty(SelectedCausaPenal.CP_FUERO) ? SelectedCausaPenal.CP_FUERO : string.Empty;
                    JuzgadoCP = SelectedCausaPenal.CP_JUZGADO != null ? SelectedCausaPenal.CP_JUZGADO : -1;
                    FecRadicacionCP = SelectedCausaPenal.CP_FEC_RADICACION;
                    AmpliacionCP = !string.IsNullOrEmpty(SelectedCausaPenal.CP_AMPLIACION) ? SelectedCausaPenal.CP_AMPLIACION : string.Empty;
                    TerminoCP = SelectedCausaPenal.CP_TERMINO != null ? SelectedCausaPenal.CP_TERMINO : -1;
                    EstatusCP = SelectedCausaPenal.ID_ESTATUS_CP != null ? SelectedCausaPenal.ID_ESTATUS_CP : -1;
                    FecVencimientoTerinoCP = SelectedCausaPenal.CP_FEC_VENCIMIENTO_TERMINO;
                    ObservacionesCP = SelectedCausaPenal.OBSERV;

                    if (SelectedCausaPenal != null)
                    { }// LstCoparticipe = new ObservableCollection<COPARTICIPE>(SelectedCausaPenal.COPARTICIPEs);
                    //DELITOS
                    PopulateDelitoCausaPenal();
                    //SENTENCIA
                    if (SelectedCausaPenal.SENTENCIA != null && SelectedCausaPenal.SENTENCIA.Any())
                    {
                        var lst = SelectedCausaPenal.SENTENCIA.Where(w => w.ESTATUS == "A");
                        if (lst.Count() > 0)
                        {
                            SelectedSentencia = lst.FirstOrDefault();
                            PopulateSentencia();
                        }
                        else
                        {
                            //CARGAMOS LA LISTA DE DELITOS CON LOS DELITOS DE LA CAUSA PENAL EN LA SENTENCIA
                            if (LstSentenciaDelitos == null)
                                LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();

                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                foreach (var d in LstCausaPenalDelitos)
                                {
                                    LstSentenciaDelitos.Add(
                                        new SENTENCIA_DELITO()
                                        {
                                            ID_DELITO = d.ID_DELITO,
                                            ID_FUERO = d.ID_FUERO,
                                            ID_MODALIDAD = d.ID_MODALIDAD,
                                            ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                                            DESCR_DELITO = d.DESCR_DELITO,
                                            CANTIDAD = d.CANTIDAD,
                                            OBJETO = d.OBJETO,
                                            MODALIDAD_DELITO = d.MODALIDAD_DELITO,
                                            TIPO_DELITO = d.TIPO_DELITO
                                        });
                                }
                            }));
                            if (LstSentenciaDelitos.Count > 0)
                                SentenciaDelitoEmpty = false;
                            else
                                SentenciaDelitoEmpty = true;
                        }
                    }
                    //NUC
                    //PopulateNUC();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer causa penal", ex);
            }
        }

        private void PopulateDelitoCausaPenal()
        {
            try
            {
                LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>(new cCausaPenalDelito().ObtenerTodos(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL));
                if (LstCausaPenalDelitos != null && LstCausaPenalDelitos.Any())
                    if (LstCausaPenalDelitos.Count > 0)
                        CausaPenalDelitoEmpty = false;
                    else
                        CausaPenalDelitoEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer delito causa penal", ex);
            }
        }

        private void PopulateSentencia()
        {
            try
            {
                var s = SelectedSentencia;
                FecS = SelectedSentencia.FEC_SENTENCIA;
                FecEjecutoriaS = SelectedSentencia.FEC_EJECUTORIA;
                FecInicioCompurgacionS = SelectedSentencia.FEC_INICIO_COMPURGACION;
                AniosS = SelectedSentencia.ANIOS;
                MesesS = SelectedSentencia.MESES;
                DiasS = SelectedSentencia.DIAS;
                MultaS = SelectedSentencia.MULTA;
                if (SelectedSentencia.MULTA_PAGADA.Equals("S"))
                    MultaSi = true;
                else
                    MultaNo = true;
                ReparacionDanioS = SelectedSentencia.REPARACION_DANIO;
                if (SelectedSentencia.REPARACION_DANIO_PAGADA.Equals("S"))
                    ReparacionSi = true;
                else
                    ReparacionNo = true;
                SustitucionPenaS = SelectedSentencia.SUSTITUCION_PENA;
                if (SelectedSentencia.SUSTITUCION_PENA_PAGADA.Equals("S"))
                    SustitucionSi = true;
                else
                    SustitucionNo = true;
                SuspensionCondicionalS = SelectedSentencia.SUSPENSION_CONDICIONAL;
                ObservacionS = SelectedSentencia.OBSERVACION;
                MotivoCancelacionAntecedenteS = SelectedSentencia.MOTIVO_CANCELACION_ANTECEDENTE;
                GradoAutoriaS = SelectedSentencia.ID_GRADO_AUTORIA;
                GradoParticipacionS = SelectedSentencia.ID_GRADO_PARTICIPACION;
                AniosAbonadosS = SelectedSentencia.ANIOS_ABONADOS;
                MesesAbonadosS = SelectedSentencia.MESES_ABONADOS;
                DiasAbonadosS = SelectedSentencia.DIAS_ABONADOS;
                FecRealCompurgacionS = SelectedSentencia.FEC_REAL_COMPURGACION;

                //DELITOS
                PopulateDelitoSentencia();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer sentencia", ex);
            }
        }


        private void PopulateDelitoSentencia()
        {
            try
            {
                if (SelectedSentencia != null)
                    LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>(new cSentenciaDelito().ObtenerTodos(SelectedSentencia.ID_CENTRO, SelectedSentencia.ID_ANIO, SelectedSentencia.ID_IMPUTADO, SelectedSentencia.ID_INGRESO, SelectedSentencia.ID_CAUSA_PENAL, SelectedSentencia.ID_SENTENCIA));
                else
                {
                    var delCP = new cCausaPenalDelito().ObtenerTodos(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL);
                    if (delCP != null)
                    {
                        if (LstSentenciaDelitos == null)
                            LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                        foreach (var d in delCP)
                        {
                            LstSentenciaDelitos.Add(
                                new SENTENCIA_DELITO()
                                {
                                    ID_DELITO = d.ID_DELITO,
                                    ID_FUERO = d.ID_FUERO,
                                    ID_MODALIDAD = d.ID_MODALIDAD,
                                    ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                                    DESCR_DELITO = d.DESCR_DELITO,
                                    CANTIDAD = d.CANTIDAD,
                                    OBJETO = d.OBJETO,
                                    MODALIDAD_DELITO = d.MODALIDAD_DELITO,
                                    TIPO_DELITO = d.TIPO_DELITO
                                });
                        }
                    }
                }
                if (LstSentenciaDelitos.Count > 0)
                    SentenciaDelitoEmpty = false;
                else
                    SentenciaDelitoEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer delito sentencia", ex);
            }
        }

        private void PopulateIngreso()
        {
            try
            {
                //DATOS GENERALES
                AnioD = IngresoSeleccionado.ID_ANIO;
                FolioD = IngresoSeleccionado.ID_IMPUTADO;
                PaternoD = IngresoSeleccionado.IMPUTADO != null ? !string.IsNullOrEmpty(IngresoSeleccionado.IMPUTADO.PATERNO) ? IngresoSeleccionado.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                MaternoD = IngresoSeleccionado.IMPUTADO != null ? !string.IsNullOrEmpty(IngresoSeleccionado.IMPUTADO.MATERNO) ? IngresoSeleccionado.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                NombreD = IngresoSeleccionado.IMPUTADO != null ? !string.IsNullOrEmpty(IngresoSeleccionado.IMPUTADO.NOMBRE) ? IngresoSeleccionado.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                IngresosD = IngresoSeleccionado.ID_INGRESO;
                //NoControlD = SelectIngreso
                if (IngresoSeleccionado.CAMA != null)
                {
                    UbicacionD = UbicacionI = string.Format("{0}-{1}{2}-{3}",
                        IngresoSeleccionado.CAMA != null ? IngresoSeleccionado.CAMA.CELDA != null ? IngresoSeleccionado.CAMA.CELDA.SECTOR != null ? IngresoSeleccionado.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(IngresoSeleccionado.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? IngresoSeleccionado.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Replace(" ", string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                        IngresoSeleccionado.CAMA != null ? IngresoSeleccionado.CAMA.CELDA != null ? IngresoSeleccionado.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(IngresoSeleccionado.CAMA.CELDA.SECTOR.DESCR) ? IngresoSeleccionado.CAMA.CELDA.SECTOR.DESCR.Replace(" ", string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty,
                        IngresoSeleccionado.CAMA != null ? IngresoSeleccionado.CAMA.CELDA != null ? !string.IsNullOrEmpty(IngresoSeleccionado.CAMA.CELDA.ID_CELDA) ? IngresoSeleccionado.CAMA.CELDA.ID_CELDA.Replace(" ", string.Empty) : string.Empty : string.Empty : string.Empty,
                                               IngresoSeleccionado.ID_UB_CAMA);
                }
                else
                    UbicacionD = UbicacionI = string.Empty;

                TipoSeguridadD = IngresoSeleccionado.TIPO_SEGURIDAD != null ? !string.IsNullOrEmpty(IngresoSeleccionado.TIPO_SEGURIDAD.DESCR) ? IngresoSeleccionado.TIPO_SEGURIDAD.DESCR.Trim() : string.Empty : string.Empty;
                FecIngresoD = IngresoSeleccionado.FEC_INGRESO_CERESO.HasValue ? IngresoSeleccionado.FEC_INGRESO_CERESO.Value : new DateTime?();
                ClasificacionJuridicaD = IngresoSeleccionado.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(IngresoSeleccionado.CLASIFICACION_JURIDICA.DESCR) ? IngresoSeleccionado.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty;

                if (IngresoSeleccionado.ESTATUS_ADMINISTRATIVO != null)
                    EstatusD = IngresoSeleccionado.ESTATUS_ADMINISTRATIVO != null ? !string.IsNullOrEmpty(IngresoSeleccionado.ESTATUS_ADMINISTRATIVO.DESCR) ? IngresoSeleccionado.ESTATUS_ADMINISTRATIVO.DESCR.Trim() : string.Empty : string.Empty;
                else
                    EstatusD = string.Empty;
                //DATOS INGRESO
                FecRegistroI = IngresoSeleccionado.FEC_REGISTRO.HasValue ? IngresoSeleccionado.FEC_REGISTRO.Value : new DateTime?();
                FecCeresoI = IngresoSeleccionado.FEC_INGRESO_CERESO.HasValue ? IngresoSeleccionado.FEC_INGRESO_CERESO.Value : new DateTime?();
                TipoI = IngresoSeleccionado.ID_TIPO_INGRESO.HasValue ? IngresoSeleccionado.ID_TIPO_INGRESO.Value : new short();
                EstatusAdministrativoI = IngresoSeleccionado.ID_ESTATUS_ADMINISTRATIVO.HasValue ? IngresoSeleccionado.ID_ESTATUS_ADMINISTRATIVO.Value : new short();
                ClasificacionI = IngresoSeleccionado.ID_CLASIFICACION_JURIDICA;
                NoOficioI = IngresoSeleccionado.DOCINTERNACION_NUM_OFICIO;
                AutoridadInternaI = IngresoSeleccionado.ID_AUTORIDAD_INTERNA.HasValue ? IngresoSeleccionado.ID_AUTORIDAD_INTERNA.Value : new short();
                TipoSeguridadI = IngresoSeleccionado.ID_TIPO_SEGURIDAD;
                QuedaDisposicionI = IngresoSeleccionado.ID_DISPOSICION.HasValue ? IngresoSeleccionado.ID_DISPOSICION.Value : new short();
                //DELITO
                if (IngresoSeleccionado.DELITO != null)
                    DelitoDescrI = IngresoSeleccionado.DELITO != null ? !string.IsNullOrEmpty(IngresoSeleccionado.DELITO.DESCR) ? IngresoSeleccionado.DELITO.DESCR.Trim() : string.Empty : string.Empty;
                //if (IngresoSeleccionado.ID_INGRESO_DELITO != null)
                //{
                //    if (IngresoSeleccionado.ID_INGRESO_DELITO == 0)
                //        IngresoDelito = -1;
                //    else
                //        IngresoDelito = IngresoSeleccionado.ID_INGRESO_DELITO.Value;
                //}
                //EXPEDIENTE GOBIERNO
                FolioGobiernoI = IngresoSeleccionado.FOLIO_GOBIERNO;
                AnioGobiernoI = IngresoSeleccionado.ANIO_GOBIERNO;
                //
                /**/
                CalcularSentencia();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ingreso", ex);
            }
        }

        private void CalcularSentencia()
        {
            try
            {
                LstSentenciasIngresos = new List<SentenciaIngreso>();
                Delitos = string.Empty;
                if (IngresoSeleccionado != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (IngresoSeleccionado.CAUSA_PENAL != null)
                    {
                        foreach (var cp in IngresoSeleccionado.CAUSA_PENAL)
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
                                        if (r.Any())
                                        {
                                            var res = r.FirstOrDefault();
                                            if (res != null)
                                            {
                                                //SENTENCIA
                                                anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
                                                meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
                                                dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);

                                                LstSentenciasIngresos.Add(
                                                new SentenciaIngreso()
                                                {
                                                    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                    Fuero = cp.CP_FUERO,
                                                    SentenciaAnios = res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS : 0,
                                                    SentenciaMeses = res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES : 0,
                                                    SentenciaDias = res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS : 0,
                                                    Instancia = "SEGUNDA INSTANCIA",
                                                    Estatus = cp.CAUSA_PENAL_ESTATUS != null ? !string.IsNullOrEmpty(cp.CAUSA_PENAL_ESTATUS.DESCR) ? cp.CAUSA_PENAL_ESTATUS.DESCR.Trim() : string.Empty : string.Empty
                                                });
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

                                        #region Delito
                                        if (s.SENTENCIA_DELITO != null)
                                        {
                                            foreach (var del in s.SENTENCIA_DELITO)
                                            {
                                                if (!string.IsNullOrEmpty(Delitos))
                                                    Delitos = string.Format("{0},", Delitos);
                                                Delitos = string.Format("{0}{1}", Delitos, del.DESCR_DELITO);
                                            }
                                        }
                                        #endregion

                                        //SENTENCIA
                                        if (!segundaInstancia)
                                        {
                                            anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
                                            meses = meses + (s.MESES != null ? s.MESES.Value : 0);
                                            dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);

                                            LstSentenciasIngresos.Add(
                                            new SentenciaIngreso()
                                            {
                                                CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                Fuero = cp.CP_FUERO,
                                                SentenciaAnios = s.ANIOS != null ? s.ANIOS : 0,
                                                SentenciaMeses = s.MESES != null ? s.MESES : 0,
                                                SentenciaDias = s.DIAS != null ? s.DIAS : 0,
                                                Instancia = "PRIMERA INSTANCIA",
                                                Estatus = cp.CAUSA_PENAL_ESTATUS != null ? cp.CAUSA_PENAL_ESTATUS.DESCR : string.Empty
                                            });
                                        }
                                        //ABONO
                                        anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
                                        meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
                                        dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
                                    }
                                }
                                else
                                {
                                    LstSentenciasIngresos.Add(
                                    new SentenciaIngreso()
                                    {
                                        CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                        Fuero = cp.CP_FUERO,
                                        SentenciaAnios = null,
                                        SentenciaMeses = null,
                                        SentenciaDias = null
                                    });
                                }
                            }
                            else
                            {
                                LstSentenciasIngresos.Add(
                                new SentenciaIngreso()
                                {
                                    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                    Fuero = cp.CP_FUERO,
                                    SentenciaAnios = null,
                                    SentenciaMeses = null,
                                    SentenciaDias = null
                                });
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
                    TotalAnios = AniosPenaI = anios;
                    TotalMeses = MesesPenaI = meses;
                    TotalDias = DiasPenaI = dias;
                    AniosAbonosI = anios_abono;
                    MesesAbonosI = meses_abono;
                    DiasAbonosI = dias_abono;

                    if (FechaInicioCompurgacion != null)
                    {
                        FechaFinCompurgacion = FechaInicioCompurgacion;
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

                        int a = 0, m = 0, d = 0;
                        new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                        AniosCumplidoI = a;
                        MesesCumplidoI = m;
                        DiasCumplidoI = d;
                        a = m = d = 0;
                        new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);
                        AniosRestanteI = a;
                        MesesRestanteI = m;
                        DiasRestanteI = d;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
        }

        private void GetInformacionAdministrativa()
        {
            try
            {
                SelectedIncidente = IngresoSeleccionado.INCIDENTE.Count > 0 ? IngresoSeleccionado.INCIDENTE.OrderBy(o => o.REGISTRO_FEC).FirstOrDefault() : null;
                ListUbicaciones = new ObservableCollection<INGRESO_UBICACION_ANT>(IngresoSeleccionado.INGRESO_UBICACION_ANT.OrderByDescending(x => x.REGISTRO_FEC));
                ListIncidentesIngreso = new ObservableCollection<INCIDENTE>(IngresoSeleccionado.INCIDENTE.OrderBy(o => o.REGISTRO_FEC));
                ListExcarcelaciones = new ObservableCollection<EXCARCELACION>(IngresoSeleccionado.EXCARCELACION.OrderByDescending(o => o.REGISTRO_FEC));
                SelectExcarcelacion = ListExcarcelaciones.Any() ? ListExcarcelaciones.FirstOrDefault() : null;
                ListTraslados = new ObservableCollection<TRASLADO>(IngresoSeleccionado.TRASLADO_DETALLE.Select(s => s.TRASLADO));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener información administrativa.", ex);
            }
        }

        private void GetExcarcelaciones(EXCARCELACION excarcelacion)
        {
            string juzgado;
            try
            {
                TextFechaHoraExcarcelacion = excarcelacion.SALIDA_FEC.HasValue ? excarcelacion.SALIDA_FEC.Value.ToString() : string.Empty;
                TextTipoExcarcelacion = excarcelacion.EXCARCELACION_TIPO != null ? excarcelacion.EXCARCELACION_TIPO.DESCR : string.Empty;
                TextOficioExcarcelacion = excarcelacion.EXCARCELACION_DESTINO.Select(w => w.FOLIO_DOC).FirstOrDefault();
                if (excarcelacion.EXCARCELACION_DESTINO.Select(s => s.JUZGADO != null).FirstOrDefault())
                    juzgado = excarcelacion.EXCARCELACION_DESTINO.Select(s => s.JUZGADO.DESCR).FirstOrDefault();
                else
                    juzgado = "";
                //var hospital = excarcelacion.EXCARCELACION_DESTINO.Select(s => s.HOSPITAL).FirstOrDefault();
                //var hospital_otro = excarcelacion.EXCARCELACION_DESTINO.Select(s => s.HOSPITAL_OTRO).FirstOrDefault();
                //TextDestinoExcarcelacion = excarcelacion.EXCARCELACION_DESTINO != null ? excarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ?
                //   juzgado : hospital != null ? hospital.DESCR : hospital_otro : string.Empty;
                TextFechaReingresoExcarcelacion = excarcelacion.RETORNO_FEC.HasValue ? excarcelacion.RETORNO_FEC.Value.ToString() : string.Empty;
                TextObservacionesExcarcelacion = excarcelacion.OBSERVACION;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener sus excarcelaciones.", ex);
            }
        }

        private void GetDatosVisita()
        {
            try
            {
                //DEJA SOLO LOS DATOS QUE DEBEN SER CALCULADOS O LOS QUE DEBAN LLEVAR UN VALOR POR DEFECTO
                SelectSexoVisitante = SelectVisitante.VISITANTE != null ? SelectVisitante.VISITANTE.PERSONA != null ? SelectVisitante.VISITANTE.PERSONA.SEXO : "S" : "S";
                TextEdadVisitante = SelectVisitante.VISITANTE != null ? SelectVisitante.VISITANTE.PERSONA != null ? new Fechas().CalculaEdad(SelectVisitante.VISITANTE.PERSONA.FEC_NACIMIENTO).ToString() : string.Empty : string.Empty;
                ImagenVisitante = SelectVisitante.VISITANTE != null ? SelectVisitante.VISITANTE.PERSONA != null ? SelectVisitante.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                    SelectVisitante.VISITANTE.PERSONA.PERSONA_BIOMETRICO.First(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO :
                    new Imagenes().getImagenPerson() : new Imagenes().getImagenPerson() : new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos de la visita.", ex);
            }
        }

        private void GetVisitasAgendadas()
        {
            try
            {
                ListProgramacionVisitaAux = new ObservableCollection<ListaVisitaAgenda>();
                var VisitaEdificio = new cVisitaEdificio().ObtenerTodosActivos(IngresoSeleccionado.ID_UB_CENTRO.HasValue ? IngresoSeleccionado.ID_UB_CENTRO.Value : (short)0,
                    new int?());

                if (VisitaEdificio != null && VisitaEdificio.Count() > 0)
                {
                    var visitaEncontrada = VisitaEdificio.Where(w => (w.CELDA_INICIO == IngresoSeleccionado.ID_UB_CELDA || w.CELDA_FINAL == IngresoSeleccionado.ID_UB_CELDA) &&
                        w.ID_SECTOR == IngresoSeleccionado.ID_UB_SECTOR && w.ID_EDIFICIO == IngresoSeleccionado.ID_UB_EDIFICIO);

                    if (!visitaEncontrada.Any())
                    {
                        var celdas = new cCelda().ObtenerPorSector(IngresoSeleccionado.ID_UB_SECTOR, IngresoSeleccionado.ID_UB_EDIFICIO, IngresoSeleccionado.ID_UB_CENTRO.Value)
                            .OrderBy(o => o.ID_CELDA);
                        foreach (var itemVisita in VisitaEdificio.Where(w => w.ID_SECTOR == IngresoSeleccionado.ID_UB_SECTOR &&
                            w.ID_EDIFICIO == IngresoSeleccionado.ID_UB_EDIFICIO))
                        {
                            if (celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == IngresoSeleccionado.ID_UB_CELDA).FirstOrDefault()) >=
                                celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == itemVisita.CELDA_INICIO).FirstOrDefault()) &&
                                celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == IngresoSeleccionado.ID_UB_CELDA).FirstOrDefault()) <=
                                celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == itemVisita.CELDA_FINAL).FirstOrDefault()))
                            {
                                ListProgramacionVisitaAux.Add(new ListaVisitaAgenda
                                {
                                    HORA_FIN = itemVisita.HORA_FIN,
                                    HORA_INI = itemVisita.HORA_INI,
                                    ID_ANIO = IngresoSeleccionado.ID_ANIO,
                                    ID_CENTRO = IngresoSeleccionado.ID_CENTRO,
                                    ID_IMPUTADO = IngresoSeleccionado.ID_IMPUTADO,
                                    ID_INGRESO = IngresoSeleccionado.ID_INGRESO,
                                    ID_AREA = itemVisita.ID_AREA.HasValue ? itemVisita.ID_AREA.Value : new Nullable<short>(),
                                    ID_DIA = itemVisita.DIA.HasValue ? itemVisita.DIA.Value : (short)-1,
                                    ID_TIPO_VISITA = itemVisita.ID_TIPO_VISITA.HasValue ? itemVisita.ID_TIPO_VISITA.Value : (short)-1,
                                    ESTATUS = itemVisita.ESTATUS == "0",
                                    AREA = itemVisita.ID_AREA.HasValue ? itemVisita.AREA.DESCR : string.Empty,
                                    DIA = itemVisita.DIA.HasValue ? Enum.GetName(typeof(DayOfWeek), itemVisita.DIA.Value) : string.Empty,
                                    HORA_SALIDA = !string.IsNullOrEmpty(itemVisita.HORA_FIN) ? itemVisita.HORA_FIN.Insert(2, ":") : string.Empty,
                                    HORA_ENTRADA = !string.IsNullOrEmpty(itemVisita.HORA_INI) ? itemVisita.HORA_INI.Insert(2, ":") : string.Empty,
                                    TIPO_VISITA = itemVisita.ID_TIPO_VISITA.HasValue ? itemVisita.TIPO_VISITA.DESCR : string.Empty,
                                    VISITA_EDIFICIO = itemVisita
                                });
                            }
                        }
                    }
                    else
                    {
                        ListProgramacionVisitaAux.Add(new ListaVisitaAgenda
                        {
                            HORA_FIN = visitaEncontrada.FirstOrDefault().HORA_FIN,
                            HORA_INI = visitaEncontrada.FirstOrDefault().HORA_INI,
                            ID_ANIO = IngresoSeleccionado.ID_ANIO,
                            ID_CENTRO = IngresoSeleccionado.ID_CENTRO,
                            ID_IMPUTADO = IngresoSeleccionado.ID_IMPUTADO,
                            ID_INGRESO = IngresoSeleccionado.ID_INGRESO,
                            ID_AREA = visitaEncontrada.FirstOrDefault().ID_AREA.HasValue ? visitaEncontrada.FirstOrDefault().ID_AREA.Value : new Nullable<short>(),
                            ID_DIA = visitaEncontrada.FirstOrDefault().DIA.HasValue ? visitaEncontrada.FirstOrDefault().DIA.Value : (short)-1,
                            ID_TIPO_VISITA = visitaEncontrada.FirstOrDefault().ID_TIPO_VISITA.HasValue ? visitaEncontrada.FirstOrDefault().ID_TIPO_VISITA.Value : (short)-1,
                            ESTATUS = visitaEncontrada.FirstOrDefault().ESTATUS == "0",
                            AREA = visitaEncontrada.FirstOrDefault().ID_AREA.HasValue ? visitaEncontrada.FirstOrDefault().AREA.DESCR : string.Empty,
                            DIA = visitaEncontrada.FirstOrDefault().VISITA_DIA.DESCR,
                            HORA_SALIDA = visitaEncontrada.FirstOrDefault().HORA_FIN.Insert(2, ":"),
                            HORA_ENTRADA = visitaEncontrada.FirstOrDefault().HORA_INI.Insert(2, ":"),
                            TIPO_VISITA = visitaEncontrada.FirstOrDefault().ID_TIPO_VISITA.HasValue ? visitaEncontrada.FirstOrDefault().TIPO_VISITA.DESCR : string.Empty,
                            VISITA_EDIFICIO = visitaEncontrada.FirstOrDefault()
                        });
                    }
                }
                foreach (var item in IngresoSeleccionado.VISITA_AGENDA.Where(w => w.ESTATUS == "0"))
                {
                    ListProgramacionVisitaAux.Add(new ListaVisitaAgenda
                    {
                        HORA_FIN = item.HORA_FIN,
                        HORA_INI = item.HORA_INI,
                        ID_ANIO = item.ID_ANIO,
                        ID_CENTRO = item.ID_CENTRO,
                        ID_IMPUTADO = item.ID_IMPUTADO,
                        ID_INGRESO = item.ID_INGRESO,
                        ID_AREA = item.ID_AREA.HasValue ? item.ID_AREA.Value : (short)-1,
                        ID_DIA = item.ID_DIA,
                        ID_TIPO_VISITA = item.ID_TIPO_VISITA,
                        ESTATUS = item.ESTATUS == "0",
                        AREA = item.ID_AREA.HasValue ? item.AREA.DESCR : string.Empty,
                        DIA = item.VISITA_DIA != null ? !string.IsNullOrEmpty(item.VISITA_DIA.DESCR) ? item.VISITA_DIA.DESCR.Trim() : string.Empty : string.Empty,
                        HORA_SALIDA = item.HORA_FIN != null ? item.HORA_FIN.Insert(2, ":") : string.Empty,
                        HORA_ENTRADA = item.HORA_INI != null ? item.HORA_INI.Insert(2, ":") : string.Empty,
                        TIPO_VISITA = item.TIPO_VISITA != null ? item.TIPO_VISITA.DESCR : string.Empty,
                        VISITA_AGENDA = item
                    });
                }

                foreach (var item in new cVisitaApellido().ObtenerTodosActivos(GlobalVar.gCentro, new Nullable<int>()))
                {
                    var letra = IngresoSeleccionado.IMPUTADO.PATERNO[0].ToString();
                    if (((item.LETRA_INICIAL == letra || item.LETRA_FINAL == letra) ||
                        (ListLetras.IndexOf(item.LETRA_INICIAL) < ListLetras.IndexOf(letra) && (ListLetras.IndexOf(item.LETRA_FINAL) > ListLetras.IndexOf(letra)))))
                    {
                        ListProgramacionVisitaAux.Add(new ListaVisitaAgenda
                        {
                            HORA_FIN = item.HORA_FIN,
                            HORA_INI = item.HORA_INI,
                            ID_ANIO = IngresoSeleccionado.ID_ANIO,
                            ID_CENTRO = IngresoSeleccionado.ID_CENTRO,
                            ID_IMPUTADO = IngresoSeleccionado.ID_IMPUTADO,
                            ID_INGRESO = IngresoSeleccionado.ID_INGRESO,
                            ID_AREA = item.ID_AREA,
                            ID_DIA = item.ID_DIA.HasValue ? item.ID_DIA.Value : (short)-1,
                            ID_TIPO_VISITA = item.ID_TIPO_VISITA.HasValue ? item.ID_TIPO_VISITA.Value : (short)-1,
                            ESTATUS = item.ESTATUS == "0",
                            AREA = item.ID_AREA.HasValue ? item.AREA.DESCR : string.Empty,
                            DIA = item.ID_DIA.HasValue ? item.VISITA_DIA.DESCR : string.Empty,
                            HORA_SALIDA = item.HORA_FIN != null ? item.HORA_FIN.Insert(2, ":") : string.Empty,
                            HORA_ENTRADA = item.HORA_INI != null ? item.HORA_INI.Insert(2, ":") : string.Empty,
                            TIPO_VISITA = item.ID_TIPO_VISITA.HasValue ? item.TIPO_VISITA.DESCR : string.Empty,
                            VISITA_APELLIDO = item
                        });
                    }
                }
                ListProgramacionVisita = new ObservableCollection<ListaVisitaAgenda>(ListProgramacionVisitaAux.Where(w => w.ESTATUS));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
            }
        }

        private void GetEstudios()
        {
            try
            {
                ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                if (IngresoSeleccionado.PERSONALIDAD != null && IngresoSeleccionado.PERSONALIDAD.Any())
                {
                    short _consecutivo = 1;//se inicializa un consecutivo para mostrar al usuario
                    ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>(IngresoSeleccionado.PERSONALIDAD.OrderBy(o => o.ID_ESTUDIO));
                    if (ListEstudiosPersonalidad != null && ListEstudiosPersonalidad.Any())
                        foreach (var item in ListEstudiosPersonalidad)
                        {
                            item.ID_AREA = _consecutivo;
                            _consecutivo++;
                        }

                    SelectEstudioPersonalidad = ListEstudiosPersonalidad.FirstOrDefault();
                }
                #region Inicia definicion de estudio socioeconomico

                LimpiaCheckBox();
                GetDatosEstudio();
                ConsultaSeleccionados(Estudio);
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
        }

        #region Estudio SOCIOECONOMICO
        private void ConsultaSeleccionados(SOCIOECONOMICO Entity)
        {
            try
            {
                if (Entity != null)
                {
                    #region Grupo Primario
                    if (Entity.SOCIOE_GPOFAMPRI != null)
                    {
                        foreach (var item in Entity.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC)
                        {
                            if (item.ID_TIPO == "M")
                            {
                                if (item.ID_CLAVE == 1) IsCartonPrimarioChecked = true;
                                if (item.ID_CLAVE == 2) IsAdobePrimarioChecked = true;
                                if (item.ID_CLAVE == 3) IsLadrilloPrimarioChecked = true;
                                if (item.ID_CLAVE == 4) IsBlockPrimarioChecked = true;
                                if (item.ID_CLAVE == 5) IsMaderaPrimarioChecked = true;
                                if (item.ID_CLAVE == 6) IsMaterialesOtrosPrimarioChecked = true;
                            }

                            if (item.ID_TIPO == "D")
                            {
                                if (item.ID_CLAVE == 1) IsSalaPrimarioChecked = true;
                                if (item.ID_CLAVE == 2) IsCocinaPrimarioChecked = true;
                                if (item.ID_CLAVE == 3) IsComedorPrimarioChecked = true;
                                if (item.ID_CLAVE == 4) IsRecamaraChecked = true;
                                if (item.ID_CLAVE == 5) IsBanioChecked = true;
                                if (item.ID_CLAVE == 6) IsDistribucionPrimariaOtrosChecked = true;
                            }

                            if (item.ID_TIPO == "S")
                            {
                                if (item.ID_CLAVE == 1) IsEnergiaElectricaPrimariaChecked = true;
                                if (item.ID_CLAVE == 2) IsAguaPrimariaChecked = true;
                                if (item.ID_CLAVE == 3) IsDrenajePrimariaChecked = true;
                                if (item.ID_CLAVE == 4) IsPavimentoPrimarioChecked = true;
                                if (item.ID_CLAVE == 5) IsTelefonoPrimarioChecked = true;
                                if (item.ID_CLAVE == 6) IsTVCableChecked = true;
                            }

                            if (item.ID_TIPO == "E")
                            {
                                if (item.ID_CLAVE == 1) IsEstufaPrimarioChecked = true;
                                if (item.ID_CLAVE == 2) IsRefrigeradorPrimarioChecked = true;
                                if (item.ID_CLAVE == 3) IsMicroOndasPrimarioChecked = true;
                                if (item.ID_CLAVE == 4) IsTVPrimarioChecked = true;
                                if (item.ID_CLAVE == 5) IsLavadoraPrimarioChecked = true;
                                if (item.ID_CLAVE == 6) IsSecadoraPrimarioChecked = true;
                                if (item.ID_CLAVE == 7) IsComputadoraPrimarioChecked = true;
                                if (item.ID_CLAVE == 8) IsOtrosElectrodomesticosPrimarioChecked = true;
                            }

                            if (item.ID_TIPO == "T")
                            {
                                if (item.ID_CLAVE == 1) IsAutomovilPrimarioChecked = true;
                                if (item.ID_CLAVE == 2) IsAutobusPrimarioChecked = true;
                                if (item.ID_CLAVE == 3) IsOtrosMediosTransportePrimarioChecked = true;
                            }
                        }
                    }

                    #endregion

                    #region Grupo Secundario
                    if (Entity.SOCIOE_GPOFAMSEC != null)
                    {
                        foreach (var item in Entity.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC)
                        {
                            if (item.ID_TIPO == "M")
                            {
                                if (item.ID_CLAVE == 1) IsCartonSecundarioChecked = true;
                                if (item.ID_CLAVE == 2) IsAdobeSecundarioChecked = true;
                                if (item.ID_CLAVE == 3) IsLadrilloSecundarioChecked = true;
                                if (item.ID_CLAVE == 4) IsBlockSecundarioChecked = true;
                                if (item.ID_CLAVE == 5) IsMaderaSecundarioChecked = true;
                                if (item.ID_CLAVE == 6) IsOtrosMaterialesSecundarioChecked = true;
                            }

                            if (item.ID_TIPO == "D")
                            {
                                if (item.ID_CLAVE == 1) IsSalaSecundarioChecked = true;
                                if (item.ID_CLAVE == 2) IsCocinaSecundarioChecked = true;
                                if (item.ID_CLAVE == 3) IsComedorSecundarioChecked = true;
                                if (item.ID_CLAVE == 4) IsRecamaraSecundarioChecked = true;
                                if (item.ID_CLAVE == 5) IsBanioSecundarioChecked = true;
                                if (item.ID_CLAVE == 6) IsOtrosDistribucionSecundarioChecked = true;
                            }

                            if (item.ID_TIPO == "S")
                            {
                                if (item.ID_CLAVE == 1) IsEnergiaElectricaSecundariaChecked = true;
                                if (item.ID_CLAVE == 2) IsAguaSecundarioChecked = true;
                                if (item.ID_CLAVE == 3) IsDrenajeSecundarioChecked = true;
                                if (item.ID_CLAVE == 4) IsPavimentoSecundarioChecked = true;
                                if (item.ID_CLAVE == 5) IsTelefonoSecundarioChecked = true;
                                if (item.ID_CLAVE == 6) IsTVCableSecundarioChecked = true;
                            }

                            if (item.ID_TIPO == "E")
                            {
                                if (item.ID_CLAVE == 1) IsEstufaSecundarioChecked = true;
                                if (item.ID_CLAVE == 2) IsRefrigeradorSecundarioChecked = true;
                                if (item.ID_CLAVE == 3) IsMicroOndasSecundarioChecked = true;
                                if (item.ID_CLAVE == 4) IsTVSecundarioChecked = true;
                                if (item.ID_CLAVE == 5) IsLavadoraSecundarioChecked = true;
                                if (item.ID_CLAVE == 6) IsSecadoraSecundarioChecked = true;
                                if (item.ID_CLAVE == 7) IsComputadoraSecundariaChecked = true;
                                if (item.ID_CLAVE == 8) IsOtrosElectrodomesticosChecked = true;
                            }

                            if (item.ID_TIPO == "T")
                            {
                                if (item.ID_CLAVE == 1) IsAutomovilSecundarioChecked = true;
                                if (item.ID_CLAVE == 2) IsAutobusSecundarioChecked = true;
                                if (item.ID_CLAVE == 3) IsOtrosMediosTransporteChecked = true;
                            }

                            if (item.ID_TIPO == "A")
                            {
                                if (item.ID_CLAVE == 1) IsGiroChecked = true;
                                if (item.ID_CLAVE == 1) IsCuentaBChecked = true;
                                if (item.ID_CLAVE == 3) IsDepositoChecked = true;
                            }
                        }
                    }


                    #endregion
                }
            }

            catch (Exception exc)
            {
            }
        }

        private void LimpiaCheckBox()
        {
            try
            {
                IsCartonPrimarioChecked = false;
                IsAdobePrimarioChecked = false;
                IsLadrilloPrimarioChecked = false;
                IsBlockPrimarioChecked = false;
                IsMaderaPrimarioChecked = false;
                IsMaterialesOtrosPrimarioChecked = false;
                IsSalaPrimarioChecked = false;
                IsCocinaPrimarioChecked = false;
                IsComedorPrimarioChecked = false;
                IsRecamaraChecked = false;
                IsBanioChecked = false;
                IsDistribucionPrimariaOtrosChecked = false;
                IsEnergiaElectricaPrimariaChecked = false;
                IsAguaPrimariaChecked = false;
                IsDrenajePrimariaChecked = false;
                IsPavimentoPrimarioChecked = false;
                IsTelefonoPrimarioChecked = false;
                IsTVCableChecked = false;
                IsEstufaPrimarioChecked = false;
                IsRefrigeradorPrimarioChecked = false;
                IsMicroOndasPrimarioChecked = false;
                IsTVPrimarioChecked = false;
                IsLavadoraPrimarioChecked = false;
                IsSecadoraPrimarioChecked = false;
                IsComputadoraPrimarioChecked = false;
                IsOtrosElectrodomesticosPrimarioChecked = false;
                IsAutomovilPrimarioChecked = false;
                IsAutobusPrimarioChecked = false;
                IsOtrosMediosTransportePrimarioChecked = false;
                IsCartonSecundarioChecked = false;
                IsAdobeSecundarioChecked = false;
                IsLadrilloSecundarioChecked = false;
                IsBlockSecundarioChecked = false;
                IsMaderaSecundarioChecked = false;
                IsOtrosMaterialesSecundarioChecked = false;
                IsSalaSecundarioChecked = false;
                IsCocinaSecundarioChecked = false;
                IsComedorSecundarioChecked = false;
                IsRecamaraSecundarioChecked = false;
                IsBanioSecundarioChecked = false;
                IsOtrosDistribucionSecundarioChecked = false;
                IsEnergiaElectricaSecundariaChecked = false;
                IsAguaSecundarioChecked = false;
                IsDrenajeSecundarioChecked = false;
                IsPavimentoSecundarioChecked = false;
                IsTelefonoSecundarioChecked = false;
                IsTVCableSecundarioChecked = false;
                IsEstufaSecundarioChecked = false;
                IsRefrigeradorSecundarioChecked = false;
                IsMicroOndasSecundarioChecked = false;
                IsTVSecundarioChecked = false;
                IsLavadoraSecundarioChecked = false;
                IsSecadoraSecundarioChecked = false;
                IsComputadoraSecundariaChecked = false;
                IsOtrosElectrodomesticosChecked = false;
                IsAutomovilSecundarioChecked = false;
                IsAutobusSecundarioChecked = false;
                IsOtrosMediosTransporteChecked = false;
                IsGiroChecked = false;
                IsCuentaBChecked = false;
                IsDepositoChecked = false;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private void GetDatosEstudio()
        {
            var EstudioAnterior = new cEstudioSocioEconomico().GetData(x => x.ID_IMPUTADO == IngresoSeleccionado.ID_IMPUTADO && x.ID_INGRESO == IngresoSeleccionado.ID_INGRESO && x.ID_ANIO == IngresoSeleccionado.ID_ANIO).FirstOrDefault();
            DatosGrupoFamiliarPrimarioEnabled = DatosGrupoFamiliarSecundarioEnabled = DictamenSocioEconomicoEnabled = true;

            if (EstudioAnterior == null)//No cuenta con un estudio previo
            {
                Estudio = null;
                GrupoFamiliarPrimario = GrupoFamiliarSecundario = RelacionIntroFamiliarPrimario = RelacionIntroFamiliarSecundario = ViviendaZonaPrimario =
                NivelSocioCulturalSecundario = ViviendaZonaSecundario = ViviendaCondicionesPrimario = ViviendaCondicionesSecundario = NivelSocioCulturalPrimario = DictamenDescripcion = AntecedenteSecundario = DeQuien = Frecuencia = RazonNoRecibeVisita = AntecedentePrimario = string.Empty;
                FamiliarAntecedentePrimario = FamiliarAntecedenteSecundario = RecibeVisita = ApoyoEconomico = -1;
                FechaEstudio = null;
                PersonasLaboranSecundario = 0;
                EgresoMensualSecundario = IngresoMensualSecundario = 0;
                NoPersonasVivenHogar = NoPersonasTrabajanPrimario = new short?();
                IngresoMensualPrimario = EgresoMensualPrimario = new int?();
                Salario = new decimal?();
            }

            else
            {
                Estudio = new SOCIOECONOMICO();
                Estudio = EstudioAnterior;

                #region Datos del grupo familiar primario
                GrupoFamiliarPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR : string.Empty;
                RelacionIntroFamiliarPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR : string.Empty;
                NoPersonasVivenHogar = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.PERSONAS_VIVEN_HOGAR : new short?();
                NoPersonasTrabajanPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.PERSONAS_LABORAN : new short?();
                IngresoMensualPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.INGRESO_MENSUAL : new int?();
                EgresoMensualPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.EGRESO_MENSUAL : new int?();
                FamiliarAntecedentePrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE : new decimal?();
                AntecedentePrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.ANTECEDENTE : string.Empty;
                ViviendaZonaPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA : string.Empty;
                ViviendaCondicionesPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES : string.Empty;
                NivelSocioCulturalPrimario = EstudioAnterior.SOCIOE_GPOFAMPRI != null ? EstudioAnterior.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL : string.Empty;
                #endregion

                #region Datos del grupo familiar secundario
                GrupoFamiliarSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR : string.Empty;
                RelacionIntroFamiliarSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR : string.Empty;
                PersonasLaboranSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.PERSONAS_LABORAN : new short?();
                IngresoMensualSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.INGRESO_MENSUAL : new int?();
                EgresoMensualSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.EGRESO_MENSUAL : new int?();
                FamiliarAntecedenteSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE : new decimal?();
                AntecedenteSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.ANTECEDENTE : string.Empty;
                ViviendaZonaSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA : string.Empty;
                ViviendaCondicionesSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES : string.Empty;
                NivelSocioCulturalSecundario = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL : string.Empty;
                RecibeVisita = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.RECIBE_VISITA : new decimal?();
                DeQuien = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.VISITA : string.Empty;
                Frecuencia = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.FRECUENCIA : string.Empty;
                FechaEstudio = EstudioAnterior.DICTAMEN_FEC;
                RazonNoRecibeVisita = EstudioAnterior.SOCIOE_GPOFAMSEC.MOTIVO_NO_VISITA;
                ApoyoEconomico = EstudioAnterior.SOCIOE_GPOFAMSEC != null ? EstudioAnterior.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO : new decimal?();
                #endregion

                #region Dictamen
                DictamenDescripcion = EstudioAnterior.DICTAMEN;
                Salario = EstudioAnterior.SALARIO;
                #endregion
            }
        }
        #endregion
        private void GetEmi()
        {
            try
            {
                var emi = new cEmiIngreso().ObtenerEmiInterno(IngresoSeleccionado.ID_IMPUTADO);
                ListEmi = new ObservableCollection<EMI_INGRESO>(emi.OrderBy(o => o.FEC_CAPTURA));//new ObservableCollection<PERSONALIDAD>(IngresoSeleccionado.PERSONALIDAD.OrderBy(o => o.ID_ESTUDIO));
                //SelectEstudioPersonalidad = ListEstudiosPersonalidad.FirstOrDefault();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
        }

        private void GetDocumentosExpediente()
        {
            try
            {
                //await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                //{
                var ingreso = new cIngreso().Obtener(IngresoSeleccionado.ID_CENTRO, IngresoSeleccionado.ID_ANIO, IngresoSeleccionado.ID_IMPUTADO, IngresoSeleccionado.ID_INGRESO);
                ListExpedienteFisico = new RangeEnabledObservableCollection<DocumentoExpedienteImputado>();
                var ListExpedienteFisicoAux = new List<DocumentoExpedienteImputado>();

                ListExpedienteFisicoAux.AddRange(ingreso.VISITANTE_INGRESO.Where(w => w.VISITA_DOCUMENTO.Any()).SelectMany(s => s.VISITA_DOCUMENTO).OrderByDescending(o => o.CAPTURA_FEC)
                    .Select(s => new DocumentoExpedienteImputado
                    {
                        FECHA = s.CAPTURA_FEC,
                        VISITA_DOCUMENTO = s,
                        TIPO_DOCTO = s.TIPO_DOCUMENTO != null ? !string.IsNullOrEmpty(s.TIPO_DOCUMENTO.DESCR) ? s.TIPO_DOCUMENTO.DESCR.Trim() : string.Empty : string.Empty
                    }).GroupBy(g => new { g.VISITA_DOCUMENTO.ID_TIPO_VISITA, g.VISITA_DOCUMENTO.ID_TIPO_DOCUMENTO }).Select(s =>
                    s.First(w => w.VISITA_DOCUMENTO.ID_TIPO_DOCUMENTO == s.Key.ID_TIPO_DOCUMENTO && w.VISITA_DOCUMENTO.ID_TIPO_VISITA == s.Key.ID_TIPO_VISITA)));

                ListExpedienteFisicoAux.AddRange(ingreso.ABOGADO_INGRESO.Where(w => w.ABOGADO_ING_DOCTO.Any()).SelectMany(s => s.ABOGADO_ING_DOCTO).OrderByDescending(o => o.CAPTURA_FEC)
                    .Select(s => new DocumentoExpedienteImputado
                    {
                        FECHA = s.CAPTURA_FEC,
                        ABOGADO_ING_DOCTO = s,
                        TIPO_DOCTO = s.TIPO_DOCUMENTO != null ? !string.IsNullOrEmpty(s.TIPO_DOCUMENTO.DESCR) ? s.TIPO_DOCUMENTO.DESCR.Trim() : string.Empty : string.Empty
                    }));


                //ListExpedienteFisicoAux.AddRange(ingreso.HISTORIA_CLINICA.Where(w => w.HISTORIA_CLINICA_DOCUMENTO.Any()).SelectMany(s => s.HISTORIA_CLINICA_DOCUMENTO)
                //    .Select(s => new DocumentoExpedienteImputado
                //    {
                //        FECHA = s.HISTORIA_CLINICA.ESTUDIO_FEC,
                //        HISTORIA_CLINICA_DOCTO = s,
                //        TIPO_DOCTO = s.HC_DOCUMENTO_TIPO.ID_DOCTO
                //    }));


                ListExpedienteFisicoAux.AddRange(ingreso.ABOGADO_INGRESO.Where(w => w.ABOGADO_CAUSA_PENAL.Any()).SelectMany(s => s.ABOGADO_CAUSA_PENAL).Where(w => w.ABOGADO_CP_DOCTO.Any())
                    .SelectMany(s => s.ABOGADO_CP_DOCTO).OrderByDescending(o => o.CAPTURA_FEC).Select(s => new DocumentoExpedienteImputado
                    {
                        ABOGADO_CP_DOCTO = s,
                        FECHA = s.CAPTURA_FEC,
                        TIPO_DOCTO = s.TIPO_DOCUMENTO.DESCR
                    }));

                ListExpedienteFisicoAux.AddRange(ingreso.AMPARO_INDIRECTO.Where(w => w.AMPARO_INDIRECTO_DOCTO.Any()).SelectMany(s => s.AMPARO_INDIRECTO_DOCTO)
                    .OrderByDescending(o => o.DIGITALIZACION_FEC).Select(s => new DocumentoExpedienteImputado
                    {
                        AMPARO_INDIRECTO_DOCTO = s,
                        FECHA = s.DIGITALIZACION_FEC,
                        TIPO_DOCTO = "AMPARO INDIRECTO"
                    }));

                ListExpedienteFisicoAux.AddRange(ingreso.CAUSA_PENAL.Where(w => w.AMPARO_INCIDENTE.Any()).SelectMany(s => s.AMPARO_INCIDENTE).Where(w => w.AMPARO_INCIDENTE_DOCTO.Any())
                    .SelectMany(s => s.AMPARO_INCIDENTE_DOCTO).OrderByDescending(o => o.DIGITALIZACION_FEC).Select(s => new DocumentoExpedienteImputado
                    {
                        AMPARO_INCIDENTE_DOCTO = s,
                        FECHA = s.DIGITALIZACION_FEC,
                        TIPO_DOCTO = "AMPARO INCIDENTE"
                    }));

                ListExpedienteFisicoAux.AddRange(ingreso.CAUSA_PENAL.Where(w => w.AMPARO_DIRECTO.Any()).SelectMany(s => s.AMPARO_DIRECTO).Where(w => w.AMPARO_DIRECTO_DOCTO.Any())
                    .SelectMany(s => s.AMPARO_DIRECTO_DOCTO).OrderByDescending(o => o.DIGITALIZACION_FEC).Select(s => new DocumentoExpedienteImputado
                    {
                        AMPARO_DIRECTO_DOCTO = s,
                        TIPO_DOCTO = "AMPARO DIRECTO",
                        FECHA = s.DIGITALIZACION_FEC
                    }));

                var i = 1;
                ListExpedienteFisicoAux.AddRange(ingreso.CAUSA_PENAL.Where(w => w.CAUSA_PENAL_DOCTO.Any()).SelectMany(s => s.CAUSA_PENAL_DOCTO).OrderByDescending(o => o.DIGITALIZACION_FEC)
                    .Select(s => new DocumentoExpedienteImputado
                    {
                        CAUSA_PENAL_DOCTO = s,
                        TIPO_DOCTO = "CAUSA PENAL DOCUMENTO " + i++,
                        FECHA = s.DIGITALIZACION_FEC
                    }));

                i = 1;
                ListExpedienteFisicoAux.AddRange(ingreso.CAUSA_PENAL.Where(w => w.RECURSO.Any()).SelectMany(s => s.RECURSO).Where(w => w.RECURSO_DOCTO.Any()).SelectMany(s => s.RECURSO_DOCTO)
                    .OrderByDescending(o => o.DIGITALIZACION_FEC).Select(s => new DocumentoExpedienteImputado
                    {
                        RECURSO_DOCTO = s,
                        TIPO_DOCTO = "RECURSO DOCUMENTO " + i++,
                        FECHA = s.DIGITALIZACION_FEC
                    }));


                ListExpedienteFisicoAux.AddRange(ingreso.CAUSA_PENAL.Where(w => w.EXCARCELACION_DESTINO.Any()).SelectMany(s => s.EXCARCELACION_DESTINO).Select(s => new DocumentoExpedienteImputado
                {
                    EXCARCELACION_DESTINO = s,
                    FECHA = s.EXCARCELACION.REGISTRO_FEC,
                    TIPO_DOCTO = s.EXCARCELACION_TIPO_DOCTO.DESCR
                }));

                //ListExpedienteFisicoAux.AddRange(ingreso.IMPUTADO.IMPUTADO_DOCUMENTO.Select(s => new DocumentoExpedienteImputado
                //{
                //    IMPUTADO_DOCUMENTO = s,
                //    TIPO_DOCTO = s.IMPUTADO_TIPO_DOCUMENTO.DESCR,
                //    FECHA = s.FEC_CREACION
                //}));

                ListExpedienteFisicoAuxiliar = new ObservableCollection<DocumentoExpedienteImputado>(ListExpedienteFisicoAux);
                ListExpedienteFisico = new ObservableCollection<DocumentoExpedienteImputado>(ListExpedienteFisicoAux);
                //});
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar documentos.", ex);
            }
        }

        private async void SeleccionarExpediente(Object obj)
        {
            try
            {
                if (obj == null) return;
                if ((Object[])obj == null) return;
                if (!(((Object[])obj)[0] is DataGrid)) return;
                if (!(((Object[])obj)[1] is PdfViewer)) return;
                if (((Object[])obj)[0] == null) return;
                if (((DataGrid)((Object[])obj)[0]).SelectedItem == null) return;
                if (((DataGrid)((Object[])obj)[0]).SelectedItem is DocumentoExpedienteImputado)
                {
                    var docto = (DocumentoExpedienteImputado)(((DataGrid)((Object[])obj)[0])).SelectedItem;
                    pdfViewer = (PdfViewer)((Object[])obj)[1];
                    if (docto.ABOGADO_ING_DOCTO != null ? docto.ABOGADO_ING_DOCTO.DOCTO != null ? docto.ABOGADO_ING_DOCTO.DOCTO.Length > 0 : false : false)
                        DocumentoDigitalizado = docto.ABOGADO_ING_DOCTO.DOCTO;
                    else if (docto.ABOGADO_CP_DOCTO != null ? docto.ABOGADO_CP_DOCTO.DOCTO != null ? docto.ABOGADO_CP_DOCTO.DOCTO.Length > 0 : false : false)
                        DocumentoDigitalizado = docto.ABOGADO_CP_DOCTO.DOCTO;
                    else if (docto.AMPARO_DIRECTO_DOCTO != null ? docto.AMPARO_DIRECTO_DOCTO.DOCUMENTO != null ? docto.AMPARO_DIRECTO_DOCTO.DOCUMENTO.Length > 0 : false : false)
                        DocumentoDigitalizado = docto.AMPARO_DIRECTO_DOCTO.DOCUMENTO;
                    else if (docto.AMPARO_INCIDENTE_DOCTO != null ? docto.AMPARO_INCIDENTE_DOCTO.DOCUMENTO != null ? docto.AMPARO_INCIDENTE_DOCTO.DOCUMENTO.Length > 0 : false : false)
                        DocumentoDigitalizado = docto.AMPARO_INCIDENTE_DOCTO.DOCUMENTO;
                    else if (docto.AMPARO_INDIRECTO_DOCTO != null ? docto.AMPARO_INDIRECTO_DOCTO.DOCUMENTO != null ? docto.AMPARO_INDIRECTO_DOCTO.DOCUMENTO.Length > 0 : false : false)
                        DocumentoDigitalizado = docto.AMPARO_INDIRECTO_DOCTO.DOCUMENTO;
                    else if (docto.CAUSA_PENAL_DOCTO != null ? docto.CAUSA_PENAL_DOCTO.DOCUMENTO != null ? docto.CAUSA_PENAL_DOCTO.DOCUMENTO.Length > 0 : false : false)
                        DocumentoDigitalizado = docto.CAUSA_PENAL_DOCTO.DOCUMENTO;
                    else if (docto.IMPUTADO_DOCUMENTO != null ? docto.IMPUTADO_DOCUMENTO.DOCUMENTO != null ? docto.IMPUTADO_DOCUMENTO.DOCUMENTO.Length > 0 : false : false)
                    {
                        if (docto.IMPUTADO_DOCUMENTO.IMPUTADO_TIPO_DOCUMENTO.ID_FORMATO == (short)enumFormatoDocumento.DOC ||
                            docto.IMPUTADO_DOCUMENTO.IMPUTADO_TIPO_DOCUMENTO.ID_FORMATO == (short)enumFormatoDocumento.DOCX)
                        {
                            DocumentoDigitalizado = docto.IMPUTADO_DOCUMENTO.DOCUMENTO;
                            Formato_Documentacion_Excarcelacion = docto.IMPUTADO_DOCUMENTO.IMPUTADO_TIPO_DOCUMENTO.ID_FORMATO;
                            tc2 = new TextControlView();
                            tc2.editor.Loaded += Loaded;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc2.Owner = PopUpsViewModels.MainWindow;
                            tc2.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                            tc2.ShowDialog();
                            tc2.editor.Loaded -= Loaded;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            return;
                        }
                        else if (docto.IMPUTADO_DOCUMENTO.IMPUTADO_TIPO_DOCUMENTO.ID_FORMATO == (short)enumFormatoDocumento.SIN_TIPO ||
                            docto.IMPUTADO_DOCUMENTO.IMPUTADO_TIPO_DOCUMENTO.ID_FORMATO == (short)enumFormatoDocumento.MS_EXCEL ||
                            docto.IMPUTADO_DOCUMENTO.IMPUTADO_TIPO_DOCUMENTO.ID_FORMATO == (short)enumFormatoDocumento.JPEG)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El documento no tiene el formato correcto para ser visualizado.");
                            return;
                        }
                    }
                    else if (docto.RECURSO_DOCTO != null ? docto.RECURSO_DOCTO.DOCUMENTO != null ? docto.RECURSO_DOCTO.DOCUMENTO.Length > 0 : false : false)
                        DocumentoDigitalizado = docto.RECURSO_DOCTO.DOCUMENTO;
                    else if (docto.VISITA_DOCUMENTO != null ? docto.VISITA_DOCUMENTO.DOCUMENTO != null ? docto.VISITA_DOCUMENTO.DOCUMENTO.Length > 0 : false : false)
                        DocumentoDigitalizado = docto.VISITA_DOCUMENTO.DOCUMENTO;
                    else if (docto.EXCARCELACION_DESTINO != null ? docto.EXCARCELACION_DESTINO.DOCUMENTO != null ? docto.EXCARCELACION_DESTINO.DOCUMENTO.Length > 0 : false : false)
                    {
                        //if (docto.EXCARCELACION_DESTINO.ID_FORMATO == (short)enumFormatoDocumento.DOC || docto.EXCARCELACION_DESTINO.ID_FORMATO == (short)enumFormatoDocumento.DOCX)
                        //{
                        //    DocumentoDigitalizado = docto.EXCARCELACION_DESTINO.DOCUMENTO;
                        //    Formato_Documentacion_Excarcelacion = docto.EXCARCELACION_DESTINO.ID_FORMATO;
                        //    tc2 = new TextControlView();
                        //    tc2.editor.Loaded += Loaded;
                        //    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        //    tc2.Owner = PopUpsViewModels.MainWindow;
                        //    tc2.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        //    tc2.ShowDialog();
                        //    tc2.editor.Loaded -= Loaded;
                        //    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        //    return;
                        //}
                        //else if (docto.EXCARCELACION_DESTINO.ID_FORMATO == (short)enumFormatoDocumento.SIN_TIPO
                        //      || docto.EXCARCELACION_DESTINO.ID_FORMATO == (short)enumFormatoDocumento.JPEG
                        //      || docto.EXCARCELACION_DESTINO.ID_FORMATO == (short)enumFormatoDocumento.MS_EXCEL)
                        //{
                        //    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El documento no tiene el formato correcto para ser visualizado.");
                        //    return;
                        //}
                    }
                    else
                    {
                        DocumentoDigitalizado = null;
                        pdfViewer.Visibility = Visibility.Hidden;
                        return;
                    }
                    await Task.Factory.StartNew(() =>
                    {
                        var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                        File.WriteAllBytes(fileNamepdf, DocumentoDigitalizado);
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            pdfViewer.LoadFile(fileNamepdf);
                            pdfViewer.Visibility = Visibility.Visible;
                        }));
                    });
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar documento.", ex);
            }
        }

        private void Loaded(object s, RoutedEventArgs e)
        {
            try
            {
                pdfViewer.Visibility = Visibility.Hidden;
                switch (Formato_Documentacion_Excarcelacion)
                {
                    case (short)enumFormatoDocumento.DOCX:
                        tc2.editor.Load(DocumentoDigitalizado, TXTextControl.BinaryStreamType.WordprocessingML);
                        break;
                    case (short)enumFormatoDocumento.DOC:
                        tc2.editor.Load(DocumentoDigitalizado, TXTextControl.BinaryStreamType.MSWord);
                        break;
                    default:
                        tc2.Close();
                        new Dialogos().ConfirmacionDialogo("Notificación!", "Formato de archivo no válido");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        break;
                }
            }
            catch (Exception ex)
            {
                tc2.Close();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
            }
        }
        #endregion

        private void LimpiarDatosVisita()
        {
            TextCodigoVisitante = string.Empty;
            TextNombreVisitante = string.Empty;
            TextPaternoVisitante = string.Empty;
            TextMaternoVisitante = string.Empty;
            SelectSexoVisitante = string.Empty;
            TextEdadVisitante = string.Empty;
            SelectParentescoVisitante = new Nullable<short>();
            SelectSituacionVisitante = new Nullable<short>();
            TextFechaUltimaModificacionVisitante = string.Empty;
            TextTelefonoVisitante = string.Empty;
            TextFechaAltaVisitante = string.Empty;
            TextCalleVisitante = string.Empty;
            TextNumExtVisitante = string.Empty;
            TextNumIntVisitante = string.Empty;
            TextCodigoPostalVisitante = string.Empty;
        }

        private void SeniasFrenteLoad(SeniasFrenteView Window = null)
        {
            try
            {
                if (Window != null && TabFrente)
                {
                    ListRadioButons = new List<RadioButton>();
                    ListRadioButons = new FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridFrente"))).ToList();
                    if (SelectSenaParticular != null && RegionCodigo != null && RegionCodigo.Length > 0)
                    {
                        foreach (var item in ListRadioButons)
                            if (item.CommandParameter != null)
                                if (SelectSenaParticular.ID_REGION.HasValue)
                                {
                                    if (item.CommandParameter.ToString().Contains(SelectSenaParticular.ID_REGION.ToString() + "-" + RegionCodigo[1] + RegionCodigo[2] + RegionCodigo[3]))
                                        item.IsChecked = true;

                                    else
                                        item.IsChecked = false;
                                };
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos iniciales de las señas particulares.", ex);
            }
        }

        private void SeniasDorsoLoad(SeniasDorsoView Window = null)
        {
            try
            {
                if (Window != null && TabDorso)
                {
                    ListRadioButons = new List<RadioButton>();
                    ListRadioButons = new FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridDorso"))).ToList();
                    //CamaraWeb = null;
                    if (SelectSenaParticular != null && RegionCodigo != null && RegionCodigo.Length > 0)
                    {
                        foreach (var item in ListRadioButons)
                            if (item.CommandParameter != null)
                                if (SelectSenaParticular.ID_REGION.HasValue)
                                {
                                    if (item.CommandParameter.ToString().Contains(SelectSenaParticular.ID_REGION.ToString() + "-" + RegionCodigo[1] + RegionCodigo[2] + RegionCodigo[3]))
                                        item.IsChecked = true;

                                    else
                                        item.IsChecked = false;
                                };
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos iniciales de las señas particulares.", ex);
            }
        }

        #region Archivero
        private void InicializaLista(short? Actual)
        {
            try
            {
                if (Actual.HasValue)
                {
                    LstDocumentos = new ObservableCollection<Archivero>();
                    switch (Actual)
                    {
                        case (short)eSituacionActual.STAGE4:
                            if (SelectEstudioPersonalidad != null)
                            {
                                SelectedEstudioTerminado = SelectEstudioPersonalidad;
                                LstDocumentos.Add(new Archivero() { Disponible = "SI", NombreArchivo = "PARTIDA JURÍDICA", TipoArchivo = (short)eDocumentoMostrado.PARTIDA_JURIDICA, VisibleVerDocumentoArchivero = true });
                                LstDocumentos.Add(new Archivero() { Disponible = "SI", NombreArchivo = "FICHA SIGNALETICA", TipoArchivo = (short)eDocumentoMostrado.FICHA_SIGNALETICA, VisibleVerDocumentoArchivero = true });

                                ObservableCollection<PERSONALIDAD> lstDummy = new ObservableCollection<PERSONALIDAD>();
                                //MuestraOficioEnvio
                                short _IdFuero = 0;
                                var _EstudioPersonalidad = new cEstudioPersonalidad().GetData(c => c.ID_ESTUDIO == SelectedEstudioTerminado.ID_ESTUDIO && c.ID_IMPUTADO == SelectedEstudioTerminado.ID_IMPUTADO && c.ID_INGRESO == SelectedEstudioTerminado.ID_INGRESO && c.ID_ANIO == SelectedEstudioTerminado.ID_ANIO).FirstOrDefault();

                                if (_EstudioPersonalidad != null)
                                {
                                    if (_EstudioPersonalidad.INGRESO != null)
                                        if (_EstudioPersonalidad.INGRESO.CAUSA_PENAL != null && _EstudioPersonalidad.INGRESO.CAUSA_PENAL.Any())
                                        {
                                            ///AISLADOS DOCUMENTOS RELACIONADOS ALA REMISION DEL PROGRAMA Y JURIDICOS INDEPENDIENTES AL DESARROLLO DE LOS ESTUDIOS
                                            var _detallesCausaPenal = _EstudioPersonalidad.INGRESO.CAUSA_PENAL.FirstOrDefault(c => c.ID_ESTATUS_CP == 1 && c.CP_FUERO == "C");
                                            if (_detallesCausaPenal != null)
                                            {
                                                var _tieneFicha = new cFichasJuridicas().GetData(c => c.ID_IMPUTADO == SelectedEstudioTerminado.ID_IMPUTADO && c.ID_INGRESO == SelectedEstudioTerminado.ID_INGRESO && c.ID_ANIO == SelectedEstudioTerminado.ID_ANIO && c.ID_CENTRO == SelectedEstudioTerminado.ID_CENTRO).FirstOrDefault();

                                                LstDocumentos.Add(new Archivero() { Disponible = _tieneFicha != null ? "SI" : "NO", NombreArchivo = "FICHA DE IDENTIFICACIÓN JURÍDICA", TipoArchivo = (short)eDocumentoMostrado.FICHA_JURIDICA, VisibleVerDocumentoArchivero = _tieneFicha != null ? true : false });

                                                var CondensadoEstudiosProgramadosGeneral = new cEstudioPersonalidad().GetData(x => x.NUM_OFICIO.Trim() == _EstudioPersonalidad.NUM_OFICIO.Trim() && x.PROG_NOMBRE.Trim() == _EstudioPersonalidad.PROG_NOMBRE.Trim());
                                                if (CondensadoEstudiosProgramadosGeneral != null && CondensadoEstudiosProgramadosGeneral.Any())
                                                    foreach (var item in CondensadoEstudiosProgramadosGeneral)
                                                        lstDummy.Add(item);

                                                LstDocumentos.Add(new Archivero() { Disponible = lstDummy.Any() ? "SI" : "NO", NombreArchivo = "FORMATO DE PETICIÓN DE REALIZACIÓN DE ESTUDIOS DE PERSONALIDAD", TipoArchivo = (short)eDocumentoMostrado.OFICIO_PETICION_REALIZACION_ESTUDIOS_PERSONALIDAD, VisibleVerDocumentoArchivero = lstDummy.Any() ? true : false });

                                                LstDocumentos.Add(new Archivero() { Disponible = lstDummy.Any() ? lstDummy.All(x => x.ID_SITUACION != 5 && x.ID_SITUACION != 2 && x.ID_SITUACION != 1) ? "SI" : "NO" : "NO", NombreArchivo = "FORMATO DE REMISIÓN DE CIERRE DE ESTUDIOS DE PERSONALIDAD", TipoArchivo = (short)eDocumentoMostrado.REMISION_CIERRE, VisibleVerDocumentoArchivero = lstDummy.Any() ? lstDummy.All(x => x.ID_SITUACION != 5) ? true : false : false });

                                                var detallesBrigada = new cEstudioPersonalidad().GetData(x => x.NUM_OFICIO.Trim() == SelectedEstudioTerminado.NUM_OFICIO && x.SOLICITUD_FEC == SelectedEstudioTerminado.SOLICITUD_FEC && x.PROG_NOMBRE.Trim() == SelectedEstudioTerminado.PROG_NOMBRE && x.ID_SITUACION != 4);//consulta los estudios que fueron hechos con respecto a esta brigada

                                                if (detallesBrigada.Any())
                                                {
                                                    bool _procede = false;
                                                    if (detallesBrigada.Any(x => x.ACTA_CONSEJO_TECNICO == null))
                                                        _procede = false;
                                                    else
                                                        _procede = true;

                                                    LstDocumentos.Add(new Archivero() { Disponible = _procede ? "SI" : "NO", NombreArchivo = "FORMATO DE REMISIÓN DE CIERRE DE ESTUDIOS DE PERSONALIDAD DEPMJ", TipoArchivo = (short)eDocumentoMostrado.REMISION_DPMJ, VisibleVerDocumentoArchivero = _procede ? true : false });
                                                }
                                                else
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "FORMATO DE REMISIÓN DE CIERRE DE ESTUDIOS DE PERSONALIDAD DEPMJ", TipoArchivo = (short)eDocumentoMostrado.REMISION_DPMJ, VisibleVerDocumentoArchivero = false });

                                                var _Comunes = new cEstudioPersonalidadFueroComun().GetData(x => x.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO && x.ID_INGRESO == _EstudioPersonalidad.ID_INGRESO && x.ID_IMPUTADO == _EstudioPersonalidad.ID_IMPUTADO).FirstOrDefault();
                                                if (_Comunes != null)
                                                {
                                                    _IdFuero = 1;

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_II_MEDICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO MÉDICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_MEDICO, VisibleVerDocumentoArchivero = _Comunes.PFC_II_MEDICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_III_PSIQUIATRICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO PSIQUIÁTRICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSIQ, VisibleVerDocumentoArchivero = _Comunes.PFC_III_PSIQUIATRICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_IV_PSICOLOGICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO PSICOLÓGICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSICO, VisibleVerDocumentoArchivero = _Comunes.PFC_IV_PSICOLOGICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_IX_SEGURIDAD != null ? "SI" : "NO", NombreArchivo = "INFORME DEL ÁREA DE SEGURIDAD Y CUSTODIA DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SEGURIDAD, VisibleVerDocumentoArchivero = _Comunes.PFC_IX_SEGURIDAD != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_V_CRIMINODIAGNOSTICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO CRIMINODIAGNÓSTICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CRIMI, VisibleVerDocumentoArchivero = _Comunes.PFC_V_CRIMINODIAGNOSTICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_VI_SOCIO_FAMILIAR != null ? "SI" : "NO", NombreArchivo = "ESTUDIO SOCIO-FAMILIAR DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SOCIO_FAM, VisibleVerDocumentoArchivero = _Comunes.PFC_VI_SOCIO_FAMILIAR != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_VII_EDUCATIVO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO EDUCATIVO, CULTURAL Y DEPORTIVO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_EDUC, VisibleVerDocumentoArchivero = _Comunes.PFC_VII_EDUCATIVO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Comunes.PFC_VIII_TRABAJO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO SOBRE CAPACITACIÓN Y TRABAJO PENITENCIARIO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CAPAC, VisibleVerDocumentoArchivero = _Comunes.PFC_VIII_TRABAJO != null ? true : false });
                                                }
                                                else
                                                {
                                                    _IdFuero = 1;

                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO MÉDICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_MEDICO, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO PSIQUIÁTRICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSIQ, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO PSICOLÓGICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSICO, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "INFORME DEL ÁREA DE SEGURIDAD Y CUSTODIA DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SEGURIDAD, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO CRIMINODIAGNÓSTICO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CRIMI, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO SOCIO-FAMILIAR DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SOCIO_FAM, VisibleVerDocumentoArchivero = false });
                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO EDUCATIVO, CULTURAL Y DEPORTIVO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_EDUC, VisibleVerDocumentoArchivero = false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO SOBRE CAPACITACIÓN Y TRABAJO PENITENCIARIO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CAPAC, VisibleVerDocumentoArchivero = false });
                                                }
                                            }
                                            else
                                            {
                                                var _Federales = new cPersonalidadFueroFederal().GetData(x => x.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO && x.ID_INGRESO == _EstudioPersonalidad.ID_INGRESO && x.ID_IMPUTADO == _EstudioPersonalidad.ID_IMPUTADO).FirstOrDefault();
                                                if (_Federales != null)
                                                {
                                                    _IdFuero = 2;
                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_ACTA_CONSEJO_TECNICO != null ? "SI" : "NO", NombreArchivo = "ACTA DE CONSEJO TÉCNICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.ACTA_FEDERAL, VisibleVerDocumentoArchivero = _Federales.PFF_ACTA_CONSEJO_TECNICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_ACTIVIDAD != null ? "SI" : "NO", NombreArchivo = "INFORME DE LAS ACTIVIDADES EDUCATIVAS, CULTURALES, DEPORTIVAS, RECREATIVAS Y CÍVICAS DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_EDUCATIVAS, VisibleVerDocumentoArchivero = _Federales.PFF_ACTIVIDAD != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_CAPACITACION != null ? "SI" : "NO", NombreArchivo = "INFORME DE LAS ACTIVIDADES PRODUCTIVAS DE CAPACITACIÓN DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CAPAC, VisibleVerDocumentoArchivero = _Federales.PFF_CAPACITACION != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_CRIMINOLOGICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO CRIMINOLÓGICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CRIMINOLOGICO, VisibleVerDocumentoArchivero = _Federales.PFF_CRIMINOLOGICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_ESTUDIO_MEDICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO MÉDICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_MEDICO, VisibleVerDocumentoArchivero = _Federales.PFF_ESTUDIO_MEDICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_ESTUDIO_PSICOLOGICO != null ? "SI" : "NO", NombreArchivo = "ESTUDIO PSICOLÓGICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_PSICO, VisibleVerDocumentoArchivero = _Federales.PFF_ESTUDIO_PSICOLOGICO != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_TRABAJO_SOCIAL != null ? "SI" : "NO", NombreArchivo = "ESTUDIO DE TRABAJO SOCIAL DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_TRABAJO_SOCIAL, VisibleVerDocumentoArchivero = _Federales.PFF_TRABAJO_SOCIAL != null ? true : false });

                                                    LstDocumentos.Add(new Archivero() { Disponible = _Federales.PFF_VIGILANCIA != null ? "SI" : "NO", NombreArchivo = "INFORME DE LA SECCIÓN DE VIGILANCIA DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_VIGILANCIA, VisibleVerDocumentoArchivero = _Federales.PFF_VIGILANCIA != null ? true : false });
                                                }
                                                else
                                                {
                                                    if (_IdFuero != 1)
                                                    {
                                                        _IdFuero = 2;
                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ACTA DE CONSEJO TÉCNICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.ACTA_FEDERAL, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "INFORME DE LAS ACTIVIDADES EDUCATIVAS, CULTURALES, DEPORTIVAS, RECREATIVAS Y CÍVICAS DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_EDUCATIVAS, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "INFORME DE LAS ACTIVIDADES PRODUCTIVAS DE CAPACITACIÓN DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CAPAC, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO CRIMINOLÓGICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CRIMINOLOGICO, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO MÉDICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_MEDICO, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO PSICOLÓGICO DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_PSICO, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "ESTUDIO DE TRABAJO SOCIAL DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_TRABAJO_SOCIAL, VisibleVerDocumentoArchivero = false });

                                                        LstDocumentos.Add(new Archivero() { Disponible = "NO", NombreArchivo = "INFORME DE LA SECCIÓN DE VIGILANCIA DE FUERO FEDERAL", TipoArchivo = (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_VIGILANCIA, VisibleVerDocumentoArchivero = false });
                                                    }
                                                }
                                            }
                                        };

                                    if (_IdFuero == 1)
                                    {
                                        var _ActaComun = new cActaConsejoTecnicoComun().GetData(x => x.ID_IMPUTADO == SelectedEstudioTerminado.ID_IMPUTADO && x.ID_INGRESO == SelectedEstudioTerminado.ID_INGRESO && x.ID_ESTUDIO == SelectedEstudioTerminado.ID_ESTUDIO).FirstOrDefault();

                                        LstDocumentos.Add(new Archivero() { Disponible = _ActaComun != null ? "SI" : "NO", NombreArchivo = "FORMATO DE REMISIÓN DE ESTUDIOS DE PERSONALIDAD ", TipoArchivo = (short)eDocumentoMostrado.DICTAMEN_INDIVIDUAL, VisibleVerDocumentoArchivero = _ActaComun != null ? true : false });

                                        LstDocumentos.Add(new Archivero() { Disponible = _ActaComun != null ? "SI" : "NO", NombreArchivo = "ACTA DE CONSEJO TÉCNICO INTERDISCIPLINARIO DE FUERO COMÚN", TipoArchivo = (short)eDocumentoMostrado.ACTA_CONSEJO_TECNICO, VisibleVerDocumentoArchivero = _ActaComun != null ? true : false });
                                    };

                                    if (_EstudioPersonalidad.ID_MOTIVO == (short)eTipoSolicitudTraslado.ISLAS)
                                    {
                                        var _validacionTrasladoIslas = new cTrasladoIslasPersonalidad().GetData(c => c.ID_IMPUTADO == _EstudioPersonalidad.ID_IMPUTADO && c.ID_INGRESO == _EstudioPersonalidad.ID_INGRESO && c.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO && c.ID_CENTRO == _EstudioPersonalidad.ID_CENTRO).FirstOrDefault();

                                        LstDocumentos.Add(new Archivero() { Disponible = _validacionTrasladoIslas != null ? "SI" : "NO", NombreArchivo = " FORMATO DE SOLICITUD DE TRASLADO A ISLAS ", TipoArchivo = (short)eDocumentoMostrado.TRASLADO_ISLAS, VisibleVerDocumentoArchivero = _validacionTrasladoIslas != null ? true : false });
                                    };

                                    if (_EstudioPersonalidad.ID_MOTIVO == (short)eTipoSolicitudTraslado.INTERNACIONAL)
                                    {
                                        var _validacionTrasladoInternacional = new cTrasladoInternacionalPersonalidad().GetData(c => c.ID_IMPUTADO == _EstudioPersonalidad.ID_IMPUTADO && c.ID_INGRESO == _EstudioPersonalidad.ID_INGRESO && c.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO && c.ID_CENTRO == _EstudioPersonalidad.ID_CENTRO).FirstOrDefault();

                                        LstDocumentos.Add(new Archivero() { Disponible = _validacionTrasladoInternacional != null ? "SI" : "NO", NombreArchivo = " FORMATO DE SOLICITUD DE TRASLADO INTERNACIONAL ", TipoArchivo = (short)eDocumentoMostrado.TRASLADO_INTERNACIONAL, VisibleVerDocumentoArchivero = _validacionTrasladoInternacional != null ? true : false });
                                    };

                                    if (_EstudioPersonalidad.ID_MOTIVO == (short)eTipoSolicitudTraslado.NACIONAL)
                                    {
                                        var _validacionTrasladoNacional = new cTrasladoNacionalPersonalidad().GetData(c => c.ID_IMPUTADO == _EstudioPersonalidad.ID_IMPUTADO && c.ID_INGRESO == _EstudioPersonalidad.ID_INGRESO && c.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO && c.ID_CENTRO == _EstudioPersonalidad.ID_CENTRO).FirstOrDefault();

                                        LstDocumentos.Add(new Archivero() { Disponible = _validacionTrasladoNacional != null ? "SI" : "NO", NombreArchivo = " FORMATO DE SOLICITUD DE TRASLADO NACIONAL ", TipoArchivo = (short)eDocumentoMostrado.TRASLADO_NACIONAL, VisibleVerDocumentoArchivero = _validacionTrasladoNacional != null ? true : false });
                                    };
                                }
                            };
                            break;

                        default:
                            //no action
                            break;
                    };
                };
            }

            catch (Exception exc)
            {
                throw exc;
            }

            return;
        }

        private void ImprimirPartidaJuridica(PERSONALIDAD _dato)
        {
            if (_dato == null)
                return;

            var _CausasPenales = _dato.INGRESO.CAUSA_PENAL.Any() ? _dato.INGRESO.CAUSA_PENAL.FirstOrDefault(c => c.ID_ESTATUS_CP == 1) : null;//SOLO CAUSAS PENALES ACTIVAS
            if (_CausasPenales != null)
            {
                try
                {
                    var centro = new cCentro().Obtener(_dato.ID_CENTRO).FirstOrDefault();
                    SENTENCIA sentencia;
                    if (_CausasPenales.SENTENCIA == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "La causa penal no cuenta con una sentencia");
                        return;
                    }
                    else
                    {
                        sentencia = _CausasPenales.SENTENCIA.FirstOrDefault(w => w.ESTATUS == "A");
                        if (sentencia == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La causa penal no cuenta con una sentencia");
                            return;
                        }
                    }

                    var diccionario = new Dictionary<string, string>();
                    diccionario.Add("centro", _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.CENTRO != null ? !string.IsNullOrEmpty(_CausasPenales.INGRESO.CENTRO.DESCR) ? _CausasPenales.INGRESO.CENTRO.DESCR.Trim() : string.Empty : string.Empty : string.Empty);
                    diccionario.Add("expediente", string.Format("{0}/{1}", _CausasPenales.ID_ANIO, _CausasPenales.ID_IMPUTADO));
                    diccionario.Add("ciudad", _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.CENTRO != null ? _CausasPenales.INGRESO.CENTRO.MUNICIPIO != null ? !string.IsNullOrEmpty(_CausasPenales.INGRESO.CENTRO.MUNICIPIO.MUNICIPIO1) ? _CausasPenales.INGRESO.CENTRO.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty : string.Empty : string.Empty);
                    diccionario.Add("fecha_letra", Fechas.fechaLetra(Fechas.GetFechaDateServer, false));
                    diccionario.Add("dirigido", centro != null ? !string.IsNullOrEmpty(centro.DIRECTOR) ? centro.DIRECTOR.Trim() : string.Empty : string.Empty);
                    string nombres = string.Empty;
                    nombres = string.Format("{0} {1} {2}", _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_CausasPenales.INGRESO.IMPUTADO.NOMBRE) ? _CausasPenales.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                        _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_CausasPenales.INGRESO.IMPUTADO.PATERNO) ? _CausasPenales.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                        _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_CausasPenales.INGRESO.IMPUTADO.MATERNO) ? _CausasPenales.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty);

                    if (_CausasPenales.INGRESO != null)
                        if (_CausasPenales.INGRESO.IMPUTADO != null)
                            if (_CausasPenales.INGRESO.IMPUTADO.ALIAS != null && _CausasPenales.INGRESO.IMPUTADO.ALIAS.Any())
                                foreach (var a in _CausasPenales.INGRESO.IMPUTADO.ALIAS)
                                    nombres = nombres + string.Format("(O)\n{0} {1} {2}", !string.IsNullOrEmpty(a.NOMBRE) ? a.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(a.PATERNO) ? a.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.MATERNO) ? a.MATERNO.Trim() : string.Empty);

                    diccionario.Add("interno", nombres);
                    diccionario.Add("causa_penal", string.Format("{0}/{1}", _CausasPenales.CP_ANIO, _CausasPenales.CP_FOLIO));
                    diccionario.Add("juzgado", _CausasPenales.JUZGADO != null ? !string.IsNullOrEmpty(_CausasPenales.JUZGADO.DESCR) ? _CausasPenales.JUZGADO.DESCR.Trim() : string.Empty : string.Empty);
                    string delitos = string.Empty;
                    if (_CausasPenales.CAUSA_PENAL_DELITO != null && _CausasPenales.CAUSA_PENAL_DELITO.Any())
                        foreach (var d in _CausasPenales.CAUSA_PENAL_DELITO)
                        {
                            if (!string.IsNullOrEmpty(delitos))
                                delitos = delitos + ",";
                            delitos = delitos + d.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(d.MODALIDAD_DELITO.DELITO.DESCR) ? d.MODALIDAD_DELITO.DELITO.DESCR.Trim() : string.Empty : string.Empty;
                        }

                    diccionario.Add("delito", delitos);
                    diccionario.Add("fecha_traslado", " ");
                    diccionario.Add("cereso_procedencia", " ");
                    diccionario.Add("ingreso_origen", _CausasPenales.INGRESO != null ? _CausasPenales.INGRESO.FEC_INGRESO_CERESO.HasValue ? Fechas.fechaLetra(_CausasPenales.INGRESO.FEC_INGRESO_CERESO.Value, false) : string.Empty : string.Empty);

                    if (_CausasPenales.SENTENCIA != null)
                    {
                        if (sentencia != null)
                        {
                            string primera;
                            primera = sentencia.FEC_EJECUTORIA.HasValue ? Fechas.fechaLetra(sentencia.FEC_EJECUTORIA.Value, false) : string.Empty + ",";
                            if (sentencia.ANIOS != null && sentencia.ANIOS > 0)
                                primera = primera + string.Format(" {0} Años", sentencia.ANIOS);
                            if (sentencia.MESES != null && sentencia.MESES > 0)
                                primera = primera + string.Format(" {0} Meses", sentencia.MESES);
                            if (sentencia.DIAS != null && sentencia.DIAS > 0)
                                primera = primera + string.Format(" {0} Dias", sentencia.DIAS);
                            diccionario.Add("sentencia_primera_instancia", primera);
                            diccionario.Add("reparacion_danio", !string.IsNullOrEmpty(sentencia.REPARACION_DANIO) ? sentencia.REPARACION_DANIO.Trim() : " ");
                            //MULTAS
                            diccionario.Add("multa_primera", !string.IsNullOrEmpty(sentencia.MULTA) ? sentencia.MULTA.Trim() : " ");
                            diccionario.Add("reparacion_primera", !string.IsNullOrEmpty(sentencia.REPARACION_DANIO) ? sentencia.REPARACION_DANIO.Trim() : " ");
                            diccionario.Add("sustitucion_primera", !string.IsNullOrEmpty(sentencia.SUSTITUCION_PENA) ? sentencia.SUSTITUCION_PENA.Trim() : " ");
                            diccionario.Add("suspencion_primera", !string.IsNullOrEmpty(sentencia.SUSPENSION_CONDICIONAL) ? sentencia.SUSPENSION_CONDICIONAL.Trim() : " ");
                            //ABONOS
                            string abonos = string.Empty;
                            if (sentencia.ANIOS_ABONADOS != null && sentencia.ANIOS_ABONADOS > 0)
                                abonos = string.Format("{0} Años ", sentencia.ANIOS_ABONADOS);
                            if (sentencia.MESES_ABONADOS != null && sentencia.MESES_ABONADOS > 0)
                                abonos = abonos + string.Format("{0} Meses ", sentencia.MESES_ABONADOS);
                            if (sentencia.DIAS_ABONADOS != null && sentencia.DIAS_ABONADOS > 0)
                                abonos = abonos + string.Format("{0} Dias ", sentencia.DIAS_ABONADOS);
                            if (!string.IsNullOrEmpty(abonos))
                                diccionario.Add("abonos", abonos);
                            else
                                diccionario.Add("abonos", " ");
                        }
                        else
                        {
                            diccionario.Add("sentencia_primera_instancia", " ");
                            //MULTAS
                            diccionario.Add("multa_primera", " ");
                            diccionario.Add("reparacion_primera", " ");
                            diccionario.Add("sustitucion_primera", " ");
                            diccionario.Add("suspencion_primera", " ");
                            //ABONOS
                            diccionario.Add("abonos", " ");
                        }
                    }
                    else
                    {
                        diccionario.Add("sentencia_primera_instancia", " ");
                        //MULTAS
                        diccionario.Add("multa_primera", " ");
                        diccionario.Add("reparacion_primera", " ");
                        diccionario.Add("sustitucion_primera", " ");
                        diccionario.Add("suspencion_primera", " ");
                        //ABONOS
                        diccionario.Add("abonos", " ");
                    }


                    diccionario.Add("beneficio_ley", "No");

                    if (_CausasPenales.RECURSO != null)
                    {
                        var recurso = _CausasPenales.RECURSO.FirstOrDefault(w => w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2");
                        if (recurso != null)
                        {
                            string segunda;
                            segunda = recurso.FEC_RECURSO.HasValue ? Fechas.fechaLetra(recurso.FEC_RECURSO.Value, false) : string.Empty + ",";
                            if (recurso.SENTENCIA_ANIOS != null && recurso.SENTENCIA_ANIOS > 0)
                                segunda = segunda + string.Format(" {0} Años", recurso.SENTENCIA_ANIOS);
                            if (recurso.SENTENCIA_MESES != null && recurso.SENTENCIA_MESES > 0)
                                segunda = segunda + string.Format(" {0} Meses", recurso.SENTENCIA_MESES);
                            if (recurso.SENTENCIA_DIAS != null && recurso.SENTENCIA_DIAS > 0)
                                segunda = segunda + string.Format(" {0} Dias", recurso.SENTENCIA_DIAS);
                            diccionario.Add("sentencia_segunda_instancia", segunda);
                            //MULTA
                            diccionario.Add("multa_segunda", !string.IsNullOrEmpty(recurso.MULTA) ? recurso.MULTA.Trim() : " ");
                            diccionario.Add("reparacion_segunda", !string.IsNullOrEmpty(recurso.REPARACION_DANIO) ? recurso.REPARACION_DANIO.Trim() : " ");
                            diccionario.Add("sustitucion_segunda", !string.IsNullOrEmpty(recurso.SUSTITUCION_PENA) ? recurso.SUSTITUCION_PENA.Trim() : " ");
                            diccionario.Add("suspencion_segunda", !string.IsNullOrEmpty(recurso.MULTA_CONDICIONAL) ? recurso.MULTA_CONDICIONAL.Trim() : " ");
                        }
                        else
                        {
                            diccionario.Add("sentencia_segunda_instancia", " ");
                            diccionario.Add("multa_segunda", " ");
                            diccionario.Add("reparacion_segunda", " ");
                            diccionario.Add("sustitucion_segunda", " ");
                            diccionario.Add("suspencion_segunda", " ");
                        }
                    }
                    else
                    {
                        diccionario.Add("sentencia_segunda_instancia", " ");
                        diccionario.Add("multa_segunda", " ");
                        diccionario.Add("reparacion_segunda", " ");
                        diccionario.Add("sustitucion_segunda", " ");
                        diccionario.Add("suspencion_segunda", " ");
                    }

                    //INCIDENTE MODIFICA SENTENCIA
                    if (_CausasPenales.AMPARO_INCIDENTE != null)
                    {
                        var incidente = _CausasPenales.AMPARO_INCIDENTE.FirstOrDefault(w => w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M");
                        if (incidente != null)
                        {
                            string adecuacion = string.Empty;
                            if (incidente.MODIFICA_PENA_ANIO > 0)
                                adecuacion = string.Format("{0} Años ", incidente.MODIFICA_PENA_ANIO);
                            if (incidente.MODIFICA_PENA_MES > 0)
                                adecuacion = adecuacion + string.Format("{0} Meses ", incidente.MODIFICA_PENA_MES);
                            if (incidente.MODIFICA_PENA_DIA > 0)
                                adecuacion = adecuacion + string.Format("{0} Dias ", incidente.MODIFICA_PENA_DIA);

                            diccionario.Add("incidente_adecuacion_pena", adecuacion);
                        }
                        else
                            diccionario.Add("incidente_adecuacion_pena", " ");
                    }
                    else
                        diccionario.Add("incidente_adecuacion_pena", " ");

                    diccionario.Add("director_centro", centro != null ? !string.IsNullOrEmpty(centro.DIRECTOR) ? centro.DIRECTOR.Trim() : string.Empty : string.Empty);
                    diccionario.Add("elaboro", StaticSourcesViewModel.UsuarioLogin.Nombre);

                    var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.PARTIDA_JURIDICA); //File.ReadAllBytes(@"C:\libertades\PJ.doc");
                    if (documento == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No se encontró la plantilla del documento");
                        return;
                    }

                    var bytes = new cWord().FillFields(documento.DOCUMENTO, diccionario);
                    if (bytes == null)
                        return;

                    var tc = new TextControlView();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.editor.Loaded += (s, e) =>
                    {
                        try
                        {
                            switch (documento.ID_FORMATO)
                            {
                                case (int)enumFormatoDocumento.DOCX:
                                    tc.editor.Load(bytes, TXTextControl.BinaryStreamType.WordprocessingML);
                                    break;
                                case (int)enumFormatoDocumento.PDF:
                                    tc.editor.Load(bytes, TXTextControl.BinaryStreamType.AdobePDF);
                                    break;
                                case (int)enumFormatoDocumento.DOC:
                                    tc.editor.Load(bytes, TXTextControl.BinaryStreamType.MSWord);
                                    break;
                                default:
                                    new Dialogos().ConfirmacionDialogo("Validación", string.Format("El formato {0} del documento no es valido", documento.FORMATO_DOCUMENTO.DESCR));
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                        }
                    };

                    tc.Owner = PopUpsViewModels.MainWindow;
                    tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Show();
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir la partida jurídica.", ex);
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una causa penal.");
        }

        private void ImprimeFicha(INGRESO _dato)
        {
            try
            {
                if (_dato != null)
                {
                    var vw = new ReporteView(_dato);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    vw.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    vw.Owner = PopUpsViewModels.MainWindow;
                    vw.Show();
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar un ingreso.");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar datos para imprimir la ficha de identificacion.", ex);
            }
        }

        private void ImprimeFichaJuridica(PERSONALIDAD _dato)
        {
            try
            {
                if (_dato == null)
                    return;

                var _FichaActual = new cFichasJuridicas().GetData(x => x.ID_IMPUTADO == _dato.ID_IMPUTADO && x.ID_INGRESO == _dato.ID_INGRESO).FirstOrDefault();
                if (_FichaActual != null)
                {
                    var DatosReporte = new cFichaIdentifJuridica();
                    var View = new ReportesView();
                    #region Iniciliza el entorno para mostrar el reporte al usuario
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    View.Owner = PopUpsViewModels.MainWindow;
                    View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                    View.Show();

                    #endregion

                    #region Cuerpo del reporte
                    #region Info Basica
                    var _Foto = _FichaActual.INGRESO != null ? _FichaActual.INGRESO.INGRESO_BIOMETRICO.Any() ? _FichaActual.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : null : null;
                    if (_Foto == null)
                        DatosReporte.Foto = new Imagenes().getImagenPerson();
                    else
                        DatosReporte.Foto = _FichaActual.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;

                    DatosReporte.NombreImputado = string.Format("{0} {1} {2}", _FichaActual.INGRESO != null ?
                        _FichaActual.INGRESO.IMPUTADO != null ?
                        !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.NOMBRE) ? _FichaActual.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                        _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.PATERNO) ? _FichaActual.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                        _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.MATERNO) ? _FichaActual.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty);

                    var _aliasByImputado = new cAlias().ObtenerTodosXImputado(_FichaActual.ID_CENTRO, _FichaActual.ID_ANIO, _FichaActual.ID_IMPUTADO);
                    if (_aliasByImputado != null && _aliasByImputado.Any())
                        foreach (var item in _aliasByImputado)
                            DatosReporte.Alias += string.Format(" y/O {0} {1} {2}", !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Trim() : string.Empty);

                    DatosReporte.Edad = string.Concat(_FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? new Fechas().CalculaEdad(_FichaActual.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty : string.Empty, " AÑOS ");

                    if (_FichaActual.INGRESO != null && _FichaActual.INGRESO.ID_UB_CENTRO.HasValue)
                    {
                        var _centro = new cCentro().GetData(x => x.ID_CENTRO == _FichaActual.INGRESO.ID_UB_CENTRO).FirstOrDefault();
                        if (_centro != null)
                            DatosReporte.CeReSo = !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty;
                    }
                    else
                    {
                        var _centro = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                        if (_centro != null)
                            DatosReporte.CeReSo = !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty;
                    }

                    DatosReporte.Sentencia = _FichaActual.P2_SENTENCIA;
                    DatosReporte.Procesos = _FichaActual.P2_PROCESOS;
                    DatosReporte.Juzgados = _FichaActual.P2_JUZGADOS;
                    DatosReporte.DescrSituacionJuridi = _FichaActual.P2_CLAS_JURID;
                    DatosReporte.CausoEjecutoria = _FichaActual.P2_EJECUTORIA;
                    DatosReporte.PorcentPena = _FichaActual.P2_PENA_COMPURG;
                    DatosReporte.ProcedenteD = _FichaActual.P2_PROCEDENTE;
                    DatosReporte.FecIngreso = _FichaActual.P2_FEC_INGRESO.HasValue ? _FichaActual.P2_FEC_INGRESO.Value.ToString("dd/MM/yyyy") : string.Empty;
                    DatosReporte.Entidad = _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? _FichaActual.INGRESO.ID_ENTIDAD.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.MUNICIPIO.ENTIDAD.DESCR) ? _FichaActual.INGRESO.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty;
                    DatosReporte.Expediente = string.Format("{0} / {1}", _FichaActual.ID_ANIO, _FichaActual.ID_IMPUTADO);
                    DatosReporte.Fecha = _FichaActual.FICHA_FEC.HasValue ? Fechas.fechaLetra(_FichaActual.FICHA_FEC.Value, false).ToUpper() : string.Empty;
                    DatosReporte.EdoCivil = _FichaActual.INGRESO != null ? _FichaActual.INGRESO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.ESTADO_CIVIL.DESCR) ? _FichaActual.INGRESO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                    if (_FichaActual.INGRESO != null && _FichaActual.INGRESO.IMPUTADO != null)
                    {
                        var _EstadoNac =
                            new cEntidad().GetData(x => x.ID_ENTIDAD == _FichaActual.INGRESO.IMPUTADO.NACIMIENTO_ESTADO)
                                .FirstOrDefault();
                        var _MunicipioNac =
                            new cMunicipio().GetData(
                                x =>
                                    x.ID_MUNICIPIO == _FichaActual.INGRESO.IMPUTADO.NACIMIENTO_MUNICIPIO &&
                                    x.ID_ENTIDAD == _FichaActual.INGRESO.IMPUTADO.NACIMIENTO_ESTADO).FirstOrDefault();
                        DatosReporte.LugarOrigen = string.Format("{0}, {1}",
                            _EstadoNac != null
                                ? !string.IsNullOrEmpty(_EstadoNac.DESCR) ? _EstadoNac.DESCR.Trim() : string.Empty
                                : string.Empty,
                            _MunicipioNac != null
                                ? !string.IsNullOrEmpty(_MunicipioNac.MUNICIPIO1)
                                    ? _MunicipioNac.MUNICIPIO1.Trim()
                                    : string.Empty
                                : string.Empty);
                    }
                    else
                        DatosReporte.LugarOrigen = string.Empty;

                    DatosReporte.FecNac = _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? _FichaActual.INGRESO.IMPUTADO.NACIMIENTO_FECHA.HasValue ? _FichaActual.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty : string.Empty;
                    if (_FichaActual.INGRESO != null && _FichaActual.INGRESO.CAMA != null)
                        DatosReporte.Ubicado = string.Format("{0}-{1}{2}-{3}",
                                                   _FichaActual.INGRESO.CAMA.CELDA != null ? _FichaActual.INGRESO.CAMA.CELDA.SECTOR != null ? _FichaActual.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? _FichaActual.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                   _FichaActual.INGRESO.CAMA.CELDA != null ? _FichaActual.INGRESO.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.CAMA.CELDA.SECTOR.DESCR) ? _FichaActual.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                                                   _FichaActual.INGRESO.CAMA.CELDA != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.CAMA.CELDA.ID_CELDA) ? _FichaActual.INGRESO.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty, _FichaActual.INGRESO.ID_UB_CAMA);

                    DatosReporte.Ocupacion = _FichaActual.INGRESO != null ? _FichaActual.INGRESO.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.OCUPACION.DESCR) ? _FichaActual.INGRESO.OCUPACION.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                    DatosReporte.Nacionalidad = _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO.ID_NACIONALIDAD.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD) ? _FichaActual.INGRESO.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD.Trim() : string.Empty : string.Empty : string.Empty;

                    DatosReporte.Escolaridad = _FichaActual.INGRESO != null ? _FichaActual.INGRESO.ID_ESCOLARIDAD.HasValue ? !string.IsNullOrEmpty(_FichaActual.INGRESO.ESCOLARIDAD.DESCR) ? _FichaActual.INGRESO.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty;

                    DatosReporte.DomicilioExt = string.Format("{0} {1} {2}", _FichaActual.INGRESO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.DOMICILIO_CALLE) ? _FichaActual.INGRESO.DOMICILIO_CALLE.Trim() : string.Empty : string.Empty,
                        _FichaActual.INGRESO != null ? _FichaActual.INGRESO.DOMICILIO_NUM_EXT.HasValue ? string.Concat("No. ", _FichaActual.INGRESO.DOMICILIO_NUM_EXT.Value.ToString()) : string.Empty : string.Empty,
                        _FichaActual.INGRESO != null ? _FichaActual.INGRESO.DOMICILIO_CP.HasValue ? _FichaActual.INGRESO.DOMICILIO_CP.Value.ToString() : string.Empty : string.Empty);

                    DatosReporte.ProcesosPendientes = string.Format("SI ( {0} ) \t NO ( {1} ) ", _FichaActual.P3_PROCESOS_PENDIENTES == "S" ? "XX" : string.Empty, _FichaActual.P3_PROCESOS_PENDIENTES == "N" ? "XX" : string.Empty);
                    DatosReporte.FecUltimosExamenes = _FichaActual.P4_ULTIMO_EXAMEN_FEC.HasValue ? Fechas.fechaLetra(_FichaActual.P4_ULTIMO_EXAMEN_FEC.Value, false).ToUpper() : string.Empty;
                    DatosReporte.Aprob = string.Format("( {0} ) APROBADO \n ( {1} ) APLAZADO \n ( {2} ) MAYORÍA  \n ( {3} ) UNANIMIDAD ", _FichaActual.P5_RESOLUCION_APROBADO == "A" ? "XX" : string.Empty, _FichaActual.P5_RESOLUCION_APLAZADO == "B" ? "XX" : string.Empty,
                        _FichaActual.P5_RESOLUCION_MAYORIA == "A" ? "XX" : string.Empty, _FichaActual.P5_RESOLUCION_UNANIMIDAD == "B" ? "XX" : string.Empty);

                    DatosReporte.APartirD = _FichaActual.P2_PARTIR;

                    DatosReporte.TramiteDescripcion = string.Format("{0} {1} {2} {3}",
                        _FichaActual != null ? _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.NOMBRE) ? _FichaActual.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        _FichaActual != null ? _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.PATERNO) ? _FichaActual.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        _FichaActual != null ? _FichaActual.INGRESO != null ? _FichaActual.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_FichaActual.INGRESO.IMPUTADO.MATERNO) ? _FichaActual.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        DatosReporte.Alias);
                    DatosReporte.Criminod = _FichaActual.P6_CRIMINODINAMIA;
                    DatosReporte.Elaboro = _FichaActual.ELABORO;
                    DatosReporte.JefeJurid = _FichaActual.JEFE_DEPARTAMENTO;
                    DatosReporte.Delito = _FichaActual.P2_DELITO;
                    DatosReporte.Tr1 = _FichaActual.TRAMITE_DIAGNOSTICO == "D" ? "XX" : string.Empty;
                    DatosReporte.Tr2 = _FichaActual.P7_TRAMITE_TRASLADO == "C" ? "XX" : string.Empty;
                    DatosReporte.TR3 = _FichaActual.P7_TRAMITE_LIBERTAD == "A" ? "XX" : string.Empty;
                    DatosReporte.Tr4 = _FichaActual.P7_TRAMITE_MODIFICACION == "B" ? "XX" : string.Empty;
                    DatosReporte.Tr5 = _FichaActual.TRAMITE_TRASLADO_VOLUNTARIO == "E" ? "XX" : string.Empty;
                    DatosReporte.DetalleTram = string.Format("LIBERTAD ANTICIPADA ( {0} ) \n MODIFICACIÓN DE LA PENA ( {1} ) \n TRASLADO ( {2} )",
                        _FichaActual.P7_TRAMITE_LIBERTAD == "A" ? "XX" : string.Empty,
                        _FichaActual.P7_TRAMITE_MODIFICACION == "B" ? "XX" : string.Empty,
                        _FichaActual.P7_TRAMITE_TRASLADO == "C" ? "XX" : string.Empty);
                    DatosReporte.Oficio = !string.IsNullOrEmpty(_FichaActual.OFICIO_ESTUDIO_SOLICITADO) ? string.Format("( ESTUDIOS SOLICITADOS MEDIANTE OFICIO {0} )", _FichaActual.OFICIO_ESTUDIO_SOLICITADO) : string.Empty;
                    #endregion


                    #endregion

                    cEncabezado Encabezado = new cEncabezado();
                    Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                    Encabezado.TituloDos = Parametro.ENCABEZADO2;
                    Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                    Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;
                    Encabezado.NombreReporte = Parametro.ENCABEZADO_FUERO_COMUN;
                    Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                    Encabezado.PieUno = Parametro.DESCR_ISO_1;
                    Encabezado.PieDos = Parametro.DESCR_ISO_2;
                    Encabezado.PieTres = Parametro.DESCR_ISO_3;


                    #region Inicializacion de reporte
                    View.Report.LocalReport.ReportPath = "Reportes/rFichaIdentificacionJuridica.rdlc";
                    View.Report.LocalReport.DataSources.Clear();
                    #endregion


                    #region Definicion de origenes de datos


                    var ds1 = new List<cEncabezado>();
                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    ds1.Add(Encabezado);
                    rds1.Name = "DataSet1";
                    rds1.Value = ds1;
                    View.Report.LocalReport.DataSources.Add(rds1);

                    //datasource dos
                    var ds2 = new List<cFichaIdentifJuridica>();
                    ds2.Add(DatosReporte);
                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = ds2;
                    View.Report.LocalReport.DataSources.Add(rds2);

                    #endregion

                    View.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    View.Report.RefreshReport();
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private void MuestraFormatoActaConsejoTecnicoComun(PERSONALIDAD _Entity)
        {
            try
            {
                if (_Entity == null)
                    return;

                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                cActaConsejoTecnicoComunReporte DatosReporte = new cActaConsejoTecnicoComunReporte();
                var _datos = new cActaConsejoTecnicoComun().GetData(x => x.ID_IMPUTADO == _Entity.ID_IMPUTADO && x.ID_INGRESO == _Entity.ID_INGRESO && x.ID_ESTUDIO == _Entity.ID_ESTUDIO && x.ID_CENTRO == _Entity.ID_CENTRO).FirstOrDefault();
                string _alia = string.Empty;
                var _aliasByImputado = new cAlias().ObtenerTodosXImputado(_Entity.ID_CENTRO, _Entity.ID_ANIO, _Entity.ID_IMPUTADO);
                if (_aliasByImputado != null && _aliasByImputado.Any())
                    foreach (var item in _aliasByImputado)
                        _alia += string.Format(" y/O {0} {1} {2}", !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(item.PATERNO) ? item.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(item.MATERNO) ? item.MATERNO.Trim() : string.Empty);

                if (_datos != null)
                {
                    string OpinionCompuesta = string.Empty;
                    if (_datos.OPINION == "S")
                        OpinionCompuesta = string.Format("OBTENIENDO ASÍ UNA OPINIÓN FAVORABLE DE REINSERCIÓN SOCIAL, POR LO QUE ES CONVENIENTE RECOMENDARLO PARA EL OTORGAMIENTO DE ALGÚN BENEFICIO DE LIBERTAD ANTICIPADA. \n POR LO ANTERIOR, EL SUB DIRECTOR JURÍDICO, EN SU CALIDAD DE SECRETARIO DEL CONSEJO TÉCNICO INTERDISCIPLINARIO, HACE CONSTAR QUE LA PRESENTE ACTUACIÓN FUE DE APROBAR Y EN EL MISMO ACTO GIRA SUS ÓRDENES PARA QUE SE REALICEN LOS TRÁMITES CORRESPONDIENTES PARA QUE EL PRESENTE ACUERDO SE REMITA A LA DIRECCIÓN DE EJECUCIÓN DE PENAS Y MEDIDAS JUDICIALES, ASÍ COMO LOS ESTUDIOS TÉCNICOS DE PERSONALIDAD, PARTIDA INTEGRA DE ANTECEDENTES PENALES DEL INTERNO {0} CON NÚMERO DE EXPEDIENTE {1}",
                           string.Format("{0} {1}", _datos.INTERNO, _alia), string.Format("{0} / {1}", _datos.ID_ANIO, _datos.ID_IMPUTADO));
                    else
                        OpinionCompuesta = string.Format("OBTENIENDO ASÍ UNA OPINIÓN FAVORABLE POR MAYORÍA DE LOS INTEGRANTES DE ESTE ÓRGANO COLEGIADO, YA QUE DENTRO DEL ABANICO DE OPORTUNIDADES QUE EN RECLUSIÓN SE OFRECE A LOS INTERNOS CONFORME AL MANDATO CONSTITUCIONAL, EL SENTENCIADO HA PARTICIPADO ACTIVA Y POSITIVAMENTE EN LOS DIVERSOS PROGRAMAS DE REINSERCIÓN SOCIAL ASÍ MISMO JURÍDICAMENTE ESTÁ EN LOS PORCENTAJES DE TIEMPO QUE MARCAN LAS LEYES RESPECTIVAS PARA QUE UN SENTENCIADO PUEDA TENER ACCESO A SOLICITAR UN BENEFICIO DE LIBERTAD ANTICIPADA, AUNADO A LO ANTERIOR CUENTA CON DÍAS LABORADOS QUE SOPORTE LO ANTERIOR, POR LO QUE ESTE ÓRGANO COLEGIADO CONSIDERA Y DETERMINA POR MAYORÍA DE LAS ÁREAS RECOMENDARLO PARA EL OTORGAMIENTO DE ALGÚN BENEFICIO DE LIBERTAD ANTICIPADA CORRESPONDIENTE. \n POR LO ANTERIOR, EL SUB DIRECTOR JURÍDICO, EN SU CALIDAD DE SECRETARIO DEL CONSEJO TÉCNICO INTERDISCIPLINARIO, HACE CONSTAR QUE LA PRESENTE ACTUACIÓN FUE DE APROBAR Y EN EL MISMO ACTO GIRA SUS ÓRDENES PARA QUE SE REALICEN LOS TRÁMITES CORRESPONDIENTES PARA QUE EL PRESENTE ACUERDO SE REMITA A LA DIRECCIÓN DE EJECUCIÓN DE PENAS Y MEDIDAS JUDICIALES, ASÍ COMO LOS ESTUDIOS TÉCNICOS DE PERSONALIDAD, PARTIDA INTEGRA DE ANTECEDENTES PENALES DEL INTERNO {0} CON NÚMERO DE EXPEDIENTE {1}",
                           string.Format("{0} {1}", _datos.INTERNO, _alia), string.Format("{0} / {1}", _datos.ID_ANIO, _datos.ID_IMPUTADO));

                    DatosReporte = new cActaConsejoTecnicoComunReporte()
                    {
                        Lugar = _datos.LUGAR,
                        Interno = string.Format("{0} {1}", _datos.INTERNO, _alia),
                        Presidente = _datos.PRESIDENTE,
                        Secretario = _datos.SECRETARIO,
                        Juridico = _datos.JURIDICO,
                        Medico = _datos.MEDICO,
                        Psicologia = _datos.PSICOLOGIA,
                        Criminologia = _datos.CRIMINOLOGIA,
                        TrabajoSocial = _datos.TRABAJO_SOCIAL,
                        Educativo = _datos.EDUCATIVO,
                        Laboral = _datos.AREA_LABORAL,
                        SeguridadCustodia = _datos.SEGURIDAD_CUSTODIA,
                        Actuacion = _datos.ACTUACION,
                        Acuerdo = _datos.ACUERDO,
                        OpinionMedico = _datos.OPINION_MEDICO == "S" ? "APROBADO" : "APLAZADO",
                        OpinionPsicologia = _datos.OPINION_PSICOLOGICA == "S" ? "APROBADO" : "APLAZADO",
                        OpinionTrabajoSocial = _datos.OPINION_TRABAJO_SOCIAL == "S" ? "APROBADO" : "APLAZADO",
                        OpinionSeguridad = _datos.OPINION_SEGURIDAD == "S" ? "APROBADO" : "APLAZADO",
                        OpinionLaboral = _datos.OPINION_LABORAL == "S" ? "APROBADO" : "APLAZADO",
                        OpinionEscolar = _datos.OPINION_ESCOLAR == "S" ? "APROBADO" : "APLAZADO",
                        OpinionCriminologia = _datos.OPINION_CRIMINOLOGIA == "S" ? "APROBADO" : "APLAZADO",
                        Manifestaron = _datos.MANIFESTARON,
                        Opinion = OpinionCompuesta
                    };
                };


                cEncabezado Encabezado = new cEncabezado();
                Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.ImagenIzquierda = Parametro.LOGO_BC_ACTA_COMUN;
                Encabezado.NombreReporte = Parametro.ENCABEZADO_FUERO_COMUN;

                #region Inicializacion de reporte
                View.Report.LocalReport.ReportPath = "Reportes/rActaConsejoTecnicoFueroComun.rdlc";
                View.Report.LocalReport.DataSources.Clear();
                #endregion

                #region Definicion d origenes de datos
                var ds1 = new List<cActaConsejoTecnicoComunReporte>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(DatosReporte);
                rds1.Name = "DataSet2";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                //datasource dos
                var ds2 = new List<cEncabezado>();
                ds2.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet1";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                #endregion

                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void MuestraOficioRemisionDictamenEstudioPersonalidad(PERSONALIDAD _Entity)
        {
            try
            {
                if (_Entity == null)
                    return;

                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();
                var NombreAreasTecnicas = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 1).FirstOrDefault();
                cFormatoDictamenEstudiosPersonalidad DatosReporte = new cFormatoDictamenEstudiosPersonalidad();
                var _datos = new cActaConsejoTecnicoComun().GetData(x => x.ID_IMPUTADO == _Entity.ID_IMPUTADO && x.ID_INGRESO == _Entity.ID_INGRESO && x.ID_ESTUDIO == _Entity.ID_ESTUDIO && x.ID_CENTRO == _Entity.ID_CENTRO).FirstOrDefault();
                var _centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                if (_datos != null)
                {
                    DatosReporte = new cFormatoDictamenEstudiosPersonalidad()
                    {
                        Expediente = string.Format("Expediente: {0} / {1}", _Entity.ID_ANIO, _Entity.ID_IMPUTADO),
                        NombreInterno = string.Format("{0} \n ({1})",
                            _datos.INGRESO != null ? _datos.INGRESO.IMPUTADO != null ?
                                string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_datos.INGRESO.IMPUTADO.NOMBRE) ? _datos.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                             !string.IsNullOrEmpty(_datos.INGRESO.IMPUTADO.PATERNO) ? _datos.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                             !string.IsNullOrEmpty(_datos.INGRESO.IMPUTADO.MATERNO) ? _datos.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty,
                        _datos.OPINION == "S" ? "APROBADO" : "APROBADO POR MAYORÍA"),
                        NombreDirectorCERESO = _centro != null ? !string.IsNullOrEmpty(_centro.DIRECTOR) ? _centro.DIRECTOR.Trim() : string.Empty : string.Empty,
                        DirectorPenas = NombreAreasTecnicas != null ? NombreAreasTecnicas.USUARIO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                                        !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                                        !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty)
                                                         : string.Empty : string.Empty : string.Empty : string.Empty,
                        Fecha = string.Format("{0} a {1}", string.Format("{0} {1}", _centro.ID_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(_centro.MUNICIPIO.MUNICIPIO1) ? _centro.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                                                                                  _centro.MUNICIPIO != null ? _centro.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(_centro.MUNICIPIO.ENTIDAD.DESCR) ? _centro.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty),
                                                                                  Fechas.fechaLetra(Fechas.GetFechaDateServer, false).ToUpper()),
                        Dictamen = string.Format("DIRECTOR DEL {0}", !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty)
                    };
                };

                cEncabezado Encabezado = new cEncabezado();
                Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                Encabezado.NombreReporte = !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                Encabezado.PieUno = Parametro.DESCR_ISO_1.Replace(";", ";\n");


                #region Inicializacion de reporte
                View.Report.LocalReport.ReportPath = "Reportes/rDictamenIndividualEstudiosPersonalidad.rdlc";
                View.Report.LocalReport.DataSources.Clear();
                #endregion

                #region Definicion d origenes de datos
                var ds1 = new List<cFormatoDictamenEstudiosPersonalidad>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(DatosReporte);
                rds1.Name = "DataSet2";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                //datasource dos
                var ds2 = new List<cEncabezado>();
                ds2.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet1";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                #endregion

                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource EncabezadoReportesFueroComun(INGRESO _ingreso)
        {
            try
            {
                string NombreImputado = _ingreso != null ? _ingreso.IMPUTADO != null ?
                    string.Format("NOMBRE DEL INTERNO: {0} {1} {2}", !string.IsNullOrEmpty(_ingreso.IMPUTADO.NOMBRE) ? _ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                  !string.IsNullOrEmpty(_ingreso.IMPUTADO.PATERNO) ? _ingreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                  !string.IsNullOrEmpty(_ingreso.IMPUTADO.MATERNO) ? _ingreso.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty;

                var ds1 = new List<cEncabezado>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                cEncabezado Encabezado = new cEncabezado();
                Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;
                Encabezado.NoImputado = NombreImputado;
                Encabezado.NombreReporte = Parametro.ENCABEZADO_FUERO_COMUN;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                Encabezado.PieUno = Parametro.DESCR_ISO_1;
                Encabezado.PieDos = Parametro.DESCR_ISO_2;
                Encabezado.PieTres = Parametro.DESCR_ISO_3;
                ds1.Add(Encabezado);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                return rds1;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void MedicoComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioMedicoFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioMedicoFueroComun.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioMedicoFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                if (_ingreso == null)
                    new Microsoft.Reporting.WinForms.ReportDataSource();

                var _Datos = new List<cRealizacionEstudios>();
                var _DatosEstudioMedicoFrueroComun = new cPersonalidadEstudioMedicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                CamposBase.EdadInterno = _ingreso.IMPUTADO != null ? new Fechas().CalculaEdad(_ingreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty;
                CamposBase.SexoInterno = _ingreso.IMPUTADO != null ? _ingreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO" : string.Empty;
                if (_DatosEstudioMedicoFrueroComun != null)
                {
                    CamposBase.AntecedentesHeredoFamiliares = _DatosEstudioMedicoFrueroComun.P2_HEREDO_FAMILIARES;
                    CamposBase.AntecedentesPersonalesNoPatologicos = _DatosEstudioMedicoFrueroComun.P3_ANTPER_NOPATO;
                    CamposBase.AntecedentesConsumoToxicosEstadoActual = _DatosEstudioMedicoFrueroComun.P31_CONSUMO_TOXICO;
                    CamposBase.DescrTatuajesCicatricesRecAntiguasMalformaciones = _DatosEstudioMedicoFrueroComun.P32_TATUAJES_CICATRICES;
                    CamposBase.AntecedentesPatologicos = _DatosEstudioMedicoFrueroComun.P4_PATOLOGICOS;
                    CamposBase.PadecimientoActual = _DatosEstudioMedicoFrueroComun.P5_PADECIMIENTOS;
                    CamposBase.TensionArterial = _DatosEstudioMedicoFrueroComun.SIGNOS_TA;
                    CamposBase.Teperatura = _DatosEstudioMedicoFrueroComun.SIGNOS_TEMPERATURA;
                    CamposBase.Pulso = _DatosEstudioMedicoFrueroComun.SIGNOS_PULSO;
                    CamposBase.Respiracion = _DatosEstudioMedicoFrueroComun.SIGNOS_RESPIRACION;
                    CamposBase.Peso = _DatosEstudioMedicoFrueroComun.SIGNOS_PESO;
                    CamposBase.Estatura = _DatosEstudioMedicoFrueroComun.SIGNOS_ESTATURA;
                    CamposBase.Abdomen = _DatosEstudioMedicoFrueroComun.COORDINADOR;
                    CamposBase.Actitud = _DatosEstudioMedicoFrueroComun.ELABORO;
                    CamposBase.ImpresionDiagnostica = _DatosEstudioMedicoFrueroComun.P7_IMPRESION_DIAGNOSTICA;
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", _DatosEstudioMedicoFrueroComun.P8_DICTAMEN_MEDICO == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty,
                        _DatosEstudioMedicoFrueroComun.P8_DICTAMEN_MEDICO == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);//estructura provisional del dictamen
                    CamposBase.FechaRealizacionEstudio = _DatosEstudioMedicoFrueroComun.ESTUDIO_FEC.HasValue ? _DatosEstudioMedicoFrueroComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }

            catch (Exception exc)
            {

                throw;
            }
        }

        #region PSIQUIATRICO COMUN
        private void PsiquiatricoComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioPsiquiatricoFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioPsiquiatricoFueroComun.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioPsiquiatricoFueroComun(INGRESO _ing, short idEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosEstudioPsiquatricoComun = new cPersonalidadEstudioPsiquiatricoComun().GetData(x => x.ID_ESTUDIO == idEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosEstudioPsiquatricoComun != null)
                {
                    CamposBase.AspectoFisico = _DatosEstudioPsiquatricoComun.A1_ASPECTO_FISICO;
                    CamposBase.ConductaMotora = _DatosEstudioPsiquatricoComun.B1_CONDUCTA_MOTORA;
                    CamposBase.Habla = _DatosEstudioPsiquatricoComun.C1_HABLA;
                    CamposBase.Actitud = _DatosEstudioPsiquatricoComun.D1_ACTITUD;
                    CamposBase.EstadoAnimo = _DatosEstudioPsiquatricoComun.A2_ESTADO_ANIMO;
                    CamposBase.ExpresAfectiva = _DatosEstudioPsiquatricoComun.B2_EXPRESION_AFECTIVA;
                    CamposBase.Adecuacion = _DatosEstudioPsiquatricoComun.C2_ADECUACION;
                    CamposBase.Alucinaciones = _DatosEstudioPsiquatricoComun.A3_ALUCINACIONES;
                    CamposBase.Ilusiones = _DatosEstudioPsiquatricoComun.B3_ILUSIONES;
                    CamposBase.Despersonalizacion = _DatosEstudioPsiquatricoComun.C3_DESPERSONALIZACION;
                    CamposBase.Desrealizacion = _DatosEstudioPsiquatricoComun.D3_DESREALIZACION;
                    CamposBase.CursoPensamiento = _DatosEstudioPsiquatricoComun.A4_CURSO;
                    CamposBase.ContinuidadPensamiento = _DatosEstudioPsiquatricoComun.B4_CONTINUIDAD;
                    CamposBase.ContenidoPensamiento = _DatosEstudioPsiquatricoComun.C4_CONTENIDO;
                    CamposBase.PensamientoAbstracto = _DatosEstudioPsiquatricoComun.D4_ABASTRACTO;
                    CamposBase.Concentracion = _DatosEstudioPsiquatricoComun.E4_CONCENTRACION;
                    CamposBase.Orientacion = _DatosEstudioPsiquatricoComun.P5_ORIENTACION;
                    CamposBase.Memoria = _DatosEstudioPsiquatricoComun.P6_MEMORIA;
                    CamposBase.BajaToleranciaFrust = _DatosEstudioPsiquatricoComun.A7_BAJA_TOLERANCIA;
                    CamposBase.ExpresionDesadapt = _DatosEstudioPsiquatricoComun.B7_EXPRESION;
                    CamposBase.Adecuada = _DatosEstudioPsiquatricoComun.C7_ADECUADA;
                    CamposBase.CapacJuicio = _DatosEstudioPsiquatricoComun.P8_CAPACIDAD_JUICIO;
                    CamposBase.Introspeccion = _DatosEstudioPsiquatricoComun.P9_INTROSPECCION;
                    CamposBase.Fiabilidad = _DatosEstudioPsiquatricoComun.P10_FIANILIDAD;
                    CamposBase.ImpresionDiagnostica = _DatosEstudioPsiquatricoComun.P11_IMPRESION;
                    CamposBase.Abdomen = _DatosEstudioPsiquatricoComun.COORDINADOR;
                    CamposBase.AliasInterno = _DatosEstudioPsiquatricoComun.MEDICO_PSIQUIATRA;
                    CamposBase.FechaRealizacionEstudio = _DatosEstudioPsiquatricoComun.ESTUDIO_FEC.HasValue ? _DatosEstudioPsiquatricoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Dictamen = string.Format("FAVORABLE ( {0} ) \t DESFAVORABLE ( {1} )", _DatosEstudioPsiquatricoComun.P12_DICTAMEN_PSIQUIATRICO == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty,
                        _DatosEstudioPsiquatricoComun.P12_DICTAMEN_PSIQUIATRICO == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }

            catch (Exception exc)
            {
                throw;
            }
        }
        #endregion

        #region PSICOLOGICO COMUN
        private void PsicologicoComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioPsicologicoFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgUno(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgDos(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgTres(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgCuatro(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioPsicologicoFueroComun.rdlc";
                View.Report.RefreshReport();

                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                string fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);

                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);

                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;

                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioPsicologicoFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosPsicologicoComun = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosPsicologicoComun != null)
                {
                    CamposBase.CondicionesGralesInterno = _DatosPsicologicoComun.P1_CONDICIONES_GRALES;
                    CamposBase.ExamenMental = _DatosPsicologicoComun.P2_EXAMEN_MENTAL;
                    CamposBase.DescripcionPrincRasgosIngresoRelComDelito = _DatosPsicologicoComun.P3_PRINCIPALES_RASGOS;
                    CamposBase.LauretaBenderTexto = string.Format("( {0} ) TEST GUESTALTICO VISOMOTOR DE LAURETTA BENDER", _DatosPsicologicoComun.P4_TEST_GUALTICO == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.MatricesRavenTexto = string.Format("( {0} ) TEST DE MATRICES PROGRESIVAS DE RAVEN", _DatosPsicologicoComun.P4_TEST_MATRICES == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.HTPTexto = string.Format("( {0} ) TEST (HTP) CASA, ARBOL, PERSONA", _DatosPsicologicoComun.P4_TEST_HTP == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.MinnessotaTexto = string.Format("( {0} ) INVENTARIO MULTIFÁSICO DE LA PERSONALIDAD MINESOTA (MMPI 1 o 2).", _DatosPsicologicoComun.P4_INVENTARIO_MULTIFASICO == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.OtroTestTexto = string.Format("( {0} ) OTRA (S) {1}", _DatosPsicologicoComun.P4_OTRAS == (short)eSINO.SI ? "X" : string.Empty, _DatosPsicologicoComun.P4_OTRA_MENCIONAR);
                    CamposBase.NivelIntelectualTextro = string.Format(
                        " ( {0} ) SUPERIOR \n ( {1} ) SUPERIOR AL TÉRMINO MEDIO \n ( {2} ) MEDIO \n ( {3} ) INFERIOR AL TÉRMINO MEDIO \n ( {4} ) INFERIOR \n ( {5} ) DEFICIENTE",
                        _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.SUPERIOR ? "X" : string.Empty, _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.SUPERIOR_TERMINO_MEDIO ? "X" : string.Empty,
                        _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.MEDIO ? "X" : string.Empty, _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.INFERIOR_TERMINO_MEDIO ? "X" : string.Empty,
                        _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.INFERIOR ? "X" : string.Empty, _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.DEFICIENTE ? "X" : string.Empty);
                    CamposBase.DatosDisfuncionNeuroTexto = string.Format(" ( {0} ) NO PRESENTA \n ( {1} ) SE SOSPECHA \n ( {2} ) CON DATOS CLÍNICOS EVIDENTES",
                        _DatosPsicologicoComun.P52_DISFUNCION_NEUROLOGICA == (short)eDisfuncionNeurologica.NO_PRESENTA ? "X" : string.Empty, _DatosPsicologicoComun.P52_DISFUNCION_NEUROLOGICA == (short)eDisfuncionNeurologica.SE_SOSPECHA ? "X" : string.Empty
                        , _DatosPsicologicoComun.P52_DISFUNCION_NEUROLOGICA == (short)eDisfuncionNeurologica.DATOS_CLINICOS_EVIDENTES ? "X" : string.Empty);
                    CamposBase.IntegracionDinamica = _DatosPsicologicoComun.P6_INTEGRACION;
                    CamposBase.RasgosPersonalidadRelComisionDelitoLogradoModificarInternamiento = _DatosPsicologicoComun.P8_RASGOS_PERSONALIDAD;
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE ", _DatosPsicologicoComun.P9_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty, _DatosPsicologicoComun.P9_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosPsicologicoComun.P10_MOTIVACION_DICTAMEN;
                    CamposBase.CasoNegativoSenialeProgramasARemitir = _DatosPsicologicoComun.P11_CASO_NEGATIVO;
                    CamposBase.RequiereTratExtraMurosTexto = string.Format("SI ( {0} ) NO ( {1} )", _DatosPsicologicoComun.P12_REQUIERE_TRATAMIENTO == "S" ? "X" : string.Empty, _DatosPsicologicoComun.P12_REQUIERE_TRATAMIENTO == "N" ? "X" : string.Empty);
                    CamposBase.CualExtramuros = _DatosPsicologicoComun.P12_CUAL;
                    CamposBase.FechaRealizacionEstudio = _DatosPsicologicoComun.ESTUDIO_FEC.HasValue ? _DatosPsicologicoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = _DatosPsicologicoComun.COORDINADOR;
                    CamposBase.Actitud = _DatosPsicologicoComun.ELABORO;
                };

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgUno(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cProgramasFueroComun>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _Padre = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio).FirstOrDefault();
                if (_Padre != null)
                {
                    var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_ESTUDIO == _Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                    {
                        short _con = 1;
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_DESHABITUAMIENTO)
                            {
                                CamposBase = new cProgramasFueroComun();
                                CamposBase.Duraci = item.DURACION;
                                CamposBase.Observaciones = item.OBSERVACION;
                                CamposBase._Consecutivo = _con;
                                CamposBase.Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty;
                                _Datos.Add(CamposBase);
                                _con++;
                            };
                        };
                    }
                    else
                    {
                        CamposBase = new cProgramasFueroComun();
                        CamposBase.Duraci = string.Empty;
                        CamposBase.Observaciones = string.Empty;
                        CamposBase._Consecutivo = 1;
                        CamposBase.Programa = string.Empty;
                        _Datos.Add(CamposBase);
                    }
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgDos(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Padre = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _Datos = new List<cProgramasFueroComun>();

                if (_Padre != null)
                {
                    var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_ESTUDIO == _Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                    {
                        short _con = 1;
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_MODIFIC_CONDUCTA)
                            {
                                CamposBase = new cProgramasFueroComun();
                                CamposBase.Duraci = item.CONCLUYO == "S" ? "SI" : "NO";
                                CamposBase.Observaciones = item.OBSERVACION;
                                CamposBase._Consecutivo = _con;
                                CamposBase.Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty;
                                _Datos.Add(CamposBase);
                                _con++;
                            };
                        };
                    }
                    else
                    {
                        CamposBase = new cProgramasFueroComun();
                        CamposBase.Duraci = string.Empty;
                        CamposBase.Observaciones = string.Empty;
                        CamposBase._Consecutivo = 1;
                        CamposBase.Programa = string.Empty;
                        _Datos.Add(CamposBase);
                    }
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgTres(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Padre = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _Datos = new List<cProgramasFueroComun>();

                cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                if (_Padre != null)
                {
                    var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_ESTUDIO == _Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);

                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                    {
                        short _con = 1;
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.COMPLEMENTARIO)
                            {
                                CamposBase = new cProgramasFueroComun()
                                {
                                    Duraci = item.CONCLUYO == "S" ? "SI" : "NO",
                                    Observaciones = item.OBSERVACION,
                                    _Consecutivo = _con,
                                    Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty,
                                };

                                _Datos.Add(CamposBase);
                                _con++;
                            };
                        };
                    }
                    else
                    {
                        CamposBase = new cProgramasFueroComun()
                        {
                            Duraci = string.Empty,
                            Observaciones = string.Empty,
                            _Consecutivo = 1,
                            Programa = string.Empty
                        };

                        _Datos.Add(CamposBase);
                    }
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet5";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgCuatro(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cProgramasFueroComun>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _Padre = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio).FirstOrDefault();
                if (_Padre != null)
                {
                    var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_ESTUDIO == _Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                    {
                        short _con = 1;
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.ID_TIPO_PROGRAMA != (short)eGrupos.COMPLEMENTARIO && item.ID_TIPO_PROGRAMA != (short)eGrupos.PROGRAMAS_DESHABITUAMIENTO && item.ID_TIPO_PROGRAMA != (short)eGrupos.PROGRAMAS_MODIFIC_CONDUCTA)
                            {
                                CamposBase = new cProgramasFueroComun()
                                {
                                    Duraci = item.CONCLUYO == "S" ? "SI" : "NO",
                                    Observaciones = item.OBSERVACION,
                                    _Consecutivo = _con,
                                    Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty
                                };

                                _Datos.Add(CamposBase);
                                _con++;
                            };
                        };
                    }
                    else
                    {
                        CamposBase = new cProgramasFueroComun()
                        {
                            Duraci = string.Empty,
                            Observaciones = string.Empty,
                            _Consecutivo = 1,
                            Programa = string.Empty
                        };
                        _Datos.Add(CamposBase);
                    }
                };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet6";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CRIMINOLOGICO COMUN
        private void CriminologicoComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioCriminodiagnosticoFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rCriminoDiagnosticoFC.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioCriminodiagnosticoFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosCriminodiagnosticoComun = new cPersonalidadEstudioCriminodiagnosticoComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO && x.ID_INGRESO == _ingreso.ID_INGRESO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosCriminodiagnosticoComun != null)
                {
                    CamposBase.VersionDelitoSegunInterno = _DatosCriminodiagnosticoComun.P1_VERSION_DELITO;
                    CamposBase.MomentoCometerDelitoEncontrabaInfluenciaDrogaTexto = string.Format(" ( {0} ) NO \n ( {1} ) SI", _DatosCriminodiagnosticoComun.P1_DROGADO == "N" ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P1_DROGADO == "S" ? "X" : string.Empty);
                    CamposBase.DescripcionDrogasTexto = string.Format(" ( {0} ) ALCOHOL \n ( {1} ) DROGAS ILEGALES \n ( {2} ) OTRA ", _DatosCriminodiagnosticoComun.P1_ALCOHOL == (short)eSINO.SI ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P1_DROGRA_ILEGAL == (short)eSINO.SI ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P1_OTRA == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.Criminogenesis = _DatosCriminodiagnosticoComun.P2_CRIMINOGENESIS;
                    CamposBase.AntecedentesEvolucionConductasParaAntiSociales = _DatosCriminodiagnosticoComun.P3_CONDUCTA_ANTISOCIAL;
                    CamposBase.ClasificCriminologTexto = string.Format(" ( {0} ) PRIMO DELINCUENTE \n ( {1} ) REINCIDENTE ESPECIFICO \n ( {2} ) REINCIDENTE GENÉRICO \n ( {3} ) HABITUAL \n ( {4} ) PROFESIONAL ", _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.PRIMO_DELINCUENTE ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.REINCIDENTE_ESPECIFICO ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.REINCIDENTE_GENERICO ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.HABITUAL ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.PROFESIONAL ? "X" : string.Empty);
                    CamposBase.IntimidacionPenaImpuestaTexto = string.Format("SI ( {0} ) NO ( {1} )", _DatosCriminodiagnosticoComun.P5_INTIMIDACION == "S" ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P5_INTIMIDACION == "N" ? "X" : string.Empty);
                    CamposBase.PorqueIntimidacion = _DatosCriminodiagnosticoComun.P5_PORQUE;
                    CamposBase.CapacidadCriminalActualTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.EgocentrismoTexto = string.Format("ALTO ( {0} ) MEDIO ( {1} ) MEDIO BAJO ( {2} ) BAJO ( {3} )", _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.LabilidadAfectivaTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIO BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.AgresividadTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.IndiferenciaAfectTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.AdaptabSocialTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) BAJA ( {2} ) ", _DatosCriminodiagnosticoComun.P7_ADAPTACION_SOCIAL == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P7_ADAPTACION_SOCIAL == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P7_ADAPTACION_SOCIAL == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.IndicePeligrosidadCriminActualTexto = string.Format("MÁXIMA ( {0} ) MEDIA-MÁXIMA ( {1} ) MEDIA ( {2} ) MEDIA-MÍNIMA ( {3} ) MÍNIMA ( {4} )", _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MAXIMA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MEDIA_MAXIMA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MEDIA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MEDIA_MINIMA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MINIMA ? "X" : string.Empty);
                    CamposBase.PronosticoReincidenciaTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE ", _DatosCriminodiagnosticoComun.P10_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P10_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosCriminodiagnosticoComun.P10_MOTIVACION_DICTAMEN;
                    CamposBase.CasoNegativoSenialeProgramasARemitir = _DatosCriminodiagnosticoComun.P11_PROGRAMAS_REMITIRSE;
                    CamposBase.RequiereTratExtraMurosTexto = string.Format("Si ( {0} ) NO ( {1} )", _DatosCriminodiagnosticoComun.P12_TRATAMIENTO_EXTRAMUROS == "S" ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P12_TRATAMIENTO_EXTRAMUROS == "N" ? "X" : string.Empty);
                    CamposBase.CualExtramuros = _DatosCriminodiagnosticoComun.P12_CUAL;
                    CamposBase.FechaRealizacionEstudio = _DatosCriminodiagnosticoComun.ESTUDIO_FEC.HasValue ? _DatosCriminodiagnosticoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Actitud = _DatosCriminodiagnosticoComun.ELABORO;
                    CamposBase.Abdomen = _DatosCriminodiagnosticoComun.COORDINADOR;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {

                throw;
            }
        }
        #endregion

        #region SOCIOFAMILIAR COMUN
        private void SocioFamiliarComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioSocioFamiliarFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosVisitas(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgramasFortalecimientoComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosProgramasReligiososComun(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioSocioFamFC.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioSocioFamiliarFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosEstudioSocioEconomicoComun = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                CamposBase.NombreInterno = string.Format("{0} {1} {2} ", _ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(_ingreso.IMPUTADO.NOMBRE) ? _ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                    _ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(_ingreso.IMPUTADO.PATERNO) ? _ingreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                    _ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(_ingreso.IMPUTADO.MATERNO) ? _ingreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);

                if (_ingreso.IMPUTADO.NACIMIENTO_ESTADO.HasValue && _ingreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue)
                {
                    var municipio = new cMunicipio().Obtener(_ingreso.IMPUTADO.NACIMIENTO_ESTADO.Value, _ingreso.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                    CamposBase.LugarFecNacInterno = string.Format("{0} {1}",
                        _ingreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(municipio.MUNICIPIO1) ? municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                        _ingreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? _ingreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty);
                }
                else
                    CamposBase.LugarFecNacInterno = string.Empty;

                CamposBase.EstadoCivilInterno = _ingreso != null ? _ingreso.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(_ingreso.ESTADO_CIVIL.DESCR) ? _ingreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                CamposBase.DomicilioInterno = string.Format("{0} {1}", _ingreso != null ? !string.IsNullOrEmpty(_ingreso.DOMICILIO_CALLE) ? _ingreso.DOMICILIO_CALLE.Trim() : string.Empty : string.Empty,
                    _ingreso != null ? _ingreso.DOMICILIO_NUM_EXT.HasValue ? _ingreso.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty : string.Empty);
                CamposBase.TelefonoInterno = _ingreso != null ? _ingreso.TELEFONO.HasValue ? _ingreso.TELEFONO.Value.ToString() : string.Empty : string.Empty;

                if (_DatosEstudioSocioEconomicoComun != null)
                {
                    CamposBase.FamiliaPrimaria = _DatosEstudioSocioEconomicoComun.P21_FAMILIA_PRIMARIA;
                    CamposBase.FamiliaSecundaria = _DatosEstudioSocioEconomicoComun.P22_FAMILIA_SECUNDARIA;
                    CamposBase.AdultoMayorProgramaEspecial = string.Format(" ( {0} ) SI \n ( {1} ) NO", _DatosEstudioSocioEconomicoComun.P3_TERCERA_EDAD == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P3_TERCERA_EDAD == "N" ? "X" : string.Empty);
                    CamposBase.RecibeVisText = string.Format(" ( {0} ) NO \n ( {1} ) SI", _DatosEstudioSocioEconomicoComun.P4_RECIBE_VISITA == "N" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_RECIBE_VISITA == "S" ? "X" : string.Empty);
                    CamposBase.QuienesVisitaG = string.Format(" PADRE ( {0} ) MADRE ( {1} ) ESPOSA(O)/CONCUBINA(O) ( {2} ) HERMANOS ( {3} ) HIJOS ( {4} ) OTROS FAMILIARES ( {5} ) \n ",
                        _DatosEstudioSocioEconomicoComun.P4_PADRE == 1 ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_MADRE == 1 ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_ESPOSOA == 1 ? "X" : string.Empty,
                        _DatosEstudioSocioEconomicoComun.P4_HERMANOS == 1 ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_HIJOS == 1 ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_OTROS == 1 ? "X" : string.Empty);
                    CamposBase.TextoGenerico1 = string.Format("ESPECIFICAR QUIEN:  {0}", _DatosEstudioSocioEconomicoComun.P4_OTROS_EPECIFICAR);
                    CamposBase.FrecuenciaV = _DatosEstudioSocioEconomicoComun.P4_FRECUENCIA;
                    CamposBase.RazonNoRecibeVisitasTexto = _DatosEstudioSocioEconomicoComun.P4_MOTIVO_NO_VISITA;
                    CamposBase.MantieneComunicTelefonicaTexto = string.Format(" ( {0} ) NO, \t ESPECIFICAR POR QUE: {1} \n \n ( {2} ) SI, ESPECIFICAR QUIEN: \t {3}", _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "S" ? _DatosEstudioSocioEconomicoComun.P5_NO_POR_QUE : string.Empty,
                        _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "N" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "N" ? _DatosEstudioSocioEconomicoComun.P5_NO_POR_QUE : string.Empty);
                    CamposBase.CuentaApoyoFamiliaAlgunaPersona = _DatosEstudioSocioEconomicoComun.P6_APOYO_EXTERIOR;
                    CamposBase.PlanesSerExternado = _DatosEstudioSocioEconomicoComun.P7_PLANES_INTERNO;
                    CamposBase.QuienViviraSerExternado = _DatosEstudioSocioEconomicoComun.P7_VIVIRA;
                    CamposBase.CuentaOfertaTrabajoTexto = string.Format(" ( {0} ) SI, \t ESPECIFICAR: {1} \n\n ( {2} ) NO", _DatosEstudioSocioEconomicoComun.P8_OFERTA_TRABAJO == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P8_OFERTA_ESPECIFICAR, _DatosEstudioSocioEconomicoComun.P8_OFERTA_TRABAJO == "N" ? "X" : string.Empty);
                    CamposBase.CuentaAvalMoralTexto = string.Format(" ( {0} ) SI, \t ESPECIFICAR: {1} \n\n ( {2} ) NO", _DatosEstudioSocioEconomicoComun.P9_AVAL_MORAL == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P9_AVAL_ESPECIFICAR, _DatosEstudioSocioEconomicoComun.P9_AVAL_MORAL == "N" ? "X" : string.Empty);
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", _DatosEstudioSocioEconomicoComun.P10_DICTAMEN == (short)eSINO.SI ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P10_DICTAMEN == (short)eSINO.NO ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosEstudioSocioEconomicoComun.P11_MOTIVACION_DICTAMEN;
                    CamposBase.Abdomen = _DatosEstudioSocioEconomicoComun.ELABORO;
                    CamposBase.Actitud = _DatosEstudioSocioEconomicoComun.COORDINADOR;
                    CamposBase.FechaRealizacionEstudio = _DatosEstudioSocioEconomicoComun.ESTUDIO_FEC.HasValue ? _DatosEstudioSocioEconomicoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }

            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosVisitas(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleVisitas = new List<cPadronVisitantesRealizacionEstudios>();

                var Padre = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cComunicacionComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleVisitas.Add(new cPadronVisitantesRealizacionEstudios
                            {
                                Frecuencia = !string.IsNullOrEmpty(item.FRECUENCIA) ? item.FRECUENCIA.Trim() : string.Empty,
                                NombreTelefono = !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Concat(string.Empty, " / ", !string.IsNullOrEmpty(item.TELEFONO) ? item.TELEFONO.Trim() : string.Empty),
                                Parentesco = item.ID_TIPO_REFERENCIA.HasValue ? !string.IsNullOrEmpty(item.TIPO_REFERENCIA.DESCR) ? item.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty
                            });
                        };
                    };
                }

                else
                {
                    var _DatoVacio = new cPadronVisitantesRealizacionEstudios()
                    {
                        Frecuencia = string.Empty,
                        NombreTelefono = string.Empty,
                        Parentesco = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleVisitas.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleVisitas;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }

            catch (System.Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgramasFortalecimientoComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleVisitas = new List<cProgramasSocioEconomicoComunReporte>();
                var Padre = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cGruposSocioEconomicoComun().GetData(x => x.ID_INGRESO == Padre.ID_INGRESO && x.ID_IMPUTADO == Padre.ID_IMPUTADO && x.ID_CENTRO == Padre.ID_CENTRO && x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_ANIO == Padre.ID_ANIO);
                    if (Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            if (item.ID_TIPO_PROGRAMA == 9)
                            {
                                _DetalleVisitas.Add(new cProgramasSocioEconomicoComunReporte
                                {
                                    Generico = item.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.TIPO_PROGRAMA.NOMBRE) ? item.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    Generico2 = item.CONGREGACION,
                                    Generico3 = item.PERIODO,
                                    Generico4 = item.OBSERVACIONES
                                });
                            };
                        };
                    }

                    else
                    {
                        var _DatoVacio = new cProgramasSocioEconomicoComunReporte()
                        {
                            Generico = string.Empty,
                            Generico2 = string.Empty,
                            Generico3 = string.Empty,
                            Generico4 = string.Empty
                        };//se crea un registro sin nada, el report data source espera un valor
                        _DetalleVisitas.Add(_DatoVacio);
                    }
                }

                _respuesta.Value = _DetalleVisitas;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgramasReligiososComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleVisitas = new List<cProgramasSocioEconomicoComunReporte>();
                var Padre = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cGruposSocioEconomicoComun().GetData(x => x.ID_INGRESO == Padre.ID_INGRESO && x.ID_IMPUTADO == Padre.ID_IMPUTADO && x.ID_CENTRO == Padre.ID_CENTRO && x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_ANIO == Padre.ID_ANIO);
                    if (Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            if (item.ID_TIPO_PROGRAMA == 11)
                            {
                                _DetalleVisitas.Add(new cProgramasSocioEconomicoComunReporte
                                {
                                    Generico = item.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.TIPO_PROGRAMA.NOMBRE) ? item.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    Generico2 = item.CONGREGACION,
                                    Generico3 = item.PERIODO,
                                    Generico4 = item.OBSERVACIONES
                                });
                            }
                            else
                                continue;
                        };
                    }

                    else
                    {
                        var _DatoVacio = new cProgramasSocioEconomicoComunReporte()
                        {
                            Generico = string.Empty,
                            Generico2 = string.Empty,
                            Generico3 = string.Empty,
                            Generico4 = string.Empty
                        };//se crea un registro sin nada, el report data source espera un valor
                        _DetalleVisitas.Add(_DatoVacio);
                    }
                }

                _respuesta.Value = _DetalleVisitas;
                _respuesta.Name = "DataSet5";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        #endregion

        #region EDUCATIVO COMUN
        private void EducativoComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioEducativoCulturalDeportivoFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosEscolaridadAnterior(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosActividadesEscolares(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosActividadesCulturales(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosActividadesDeportivas(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEStudiodioEducCultDepFC.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioEducativoCulturalDeportivoFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var dato = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();

                if (dato != null)
                {
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", dato.P3_DICTAMEN == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty, dato.P3_DICTAMEN == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = dato.P4_MOTIVACION_DICTAMEN;
                    CamposBase.FechaRealizacionEstudio = dato.ESTUDIO_FEC.HasValue ? dato.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = dato.COORDINADOR;
                    CamposBase.Actitud = dato.ELABORO;
                }

                else
                {
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", string.Empty, string.Empty);
                    CamposBase.MotivacionDictamen = string.Empty;
                    CamposBase.FechaRealizacionEstudio = string.Empty;
                    CamposBase.Abdomen = string.Empty;
                    CamposBase.Actitud = string.Empty;
                }


                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEscolaridadAnterior(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cEscolaridadAnterior>();
                var Padre = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cEscolaridadesAnterioresIngreso().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleEscolaridad.Add(new cEscolaridadAnterior
                            {
                                DescripcionConcluida = item.CONCLUIDA == "S" ? "SI" : "NO",
                                NivelEducativo = !string.IsNullOrEmpty(item.EDUCACION_GRADO.DESCR) ? item.EDUCACION_GRADO.DESCR.Trim() : string.Empty,
                                ObservacionesEscolaridadAnterior = !string.IsNullOrEmpty(item.OBSERVACION) ? item.OBSERVACION.Trim() : string.Empty
                            });
                        };
                    }
                }
                else
                {
                    var _DatoVacio = new cEscolaridadAnterior()
                    {
                        DescripcionConcluida = string.Empty,
                        NivelEducativo = string.Empty,
                        ObservacionesEscolaridadAnterior = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActividadesEscolares(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cActividadesEducativas>();
                var Padre = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cEscolaridadesAnterioresIngreso().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO && !string.IsNullOrEmpty(x.RENDIMIENTO) && !string.IsNullOrEmpty(x.INTERES));
                    if (Datos != null && Datos.Any())
                    {
                        var _Act = new cActividad().GetData(x => x.ID_ACTIVIDAD == 2).FirstOrDefault();
                        foreach (var item in Datos)
                        {
                            _DetalleEscolaridad.Add(new cActividadesEducativas
                            {
                                Concluida = item.CONCLUIDA == "S" ? "SI" : "NO",
                                Nivel = _Act != null ? _Act.DESCR : string.Empty, //item.PFC_VII_EDUCATIVO != null ? item.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD.Any() ? item.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD.FirstOrDefault().ACTIVIDAD : string.Empty : string.Empty,//  item.EDUCACION_GRADO != null ? !string.IsNullOrEmpty(item.EDUCACION_GRADO.DESCR) ? item.EDUCACION_GRADO.DESCR.Trim() : string.Empty : string.Empty,
                                Observaciones = !string.IsNullOrEmpty(item.OBSERVACION) ? item.OBSERVACION.Trim() : string.Empty,
                                Interes = item.INTERES,
                                RendimientoEscolar = item.RENDIMIENTO
                            });
                        };
                    };
                }

                else
                {
                    var _DatoVacio = new cActividadesEducativas()
                    {
                        Concluida = string.Empty,
                        Interes = string.Empty,
                        Nivel = string.Empty,
                        Observaciones = string.Empty,
                        RendimientoEscolar = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActividadesCulturales(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cActividadesCulturales>();
                var Padre = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cActividadesEstudioEducativoFueroComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleEscolaridad.Add(new cActividadesCulturales
                            {
                                Actividad = item.ACTIVIDAD,
                                Duracion = item.DURACION,
                                Observaciones = item.OBSERVACION,
                                Programa = item.PFC_VII_PROGRAMA != null ? !string.IsNullOrEmpty(item.PFC_VII_PROGRAMA.DESCR) ? item.PFC_VII_PROGRAMA.DESCR.Trim() : string.Empty : string.Empty
                            });
                        };
                    }
                }

                else
                {
                    var _DatoVacio = new cActividadesCulturales()
                    {
                        Actividad = string.Empty,
                        Duracion = string.Empty,
                        Observaciones = string.Empty,
                        Programa = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet5";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActividadesDeportivas(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cActividadesCulturales>();
                var Padre = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cActividadesEstudioEducativoFueroComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleEscolaridad.Add(new cActividadesCulturales
                            {
                                Actividad = string.Empty,//item.ACTIVIDAD,
                                Duracion = string.Empty,//item.DURACION,
                                Observaciones = string.Empty,//item.OBSERVACION,
                                Programa = string.Empty//item.PFC_VII_PROGRAMA != null ? !string.IsNullOrEmpty(item.PFC_VII_PROGRAMA.DESCR) ? item.PFC_VII_PROGRAMA.DESCR.Trim() : string.Empty : string.Empty
                            });
                        };
                    };
                }

                else
                {
                    var _DatoVacio = new cActividadesCulturales()
                    {
                        Actividad = string.Empty,
                        Duracion = string.Empty,
                        Observaciones = string.Empty,
                        Programa = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet6";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        #endregion

        #region CAPACITACION COMUN
        private void CapacitacionComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosEstudioCapacitacionTrabajoPenitenciarioFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosCapacitacionLaboral(_ing, _IdEstudio));//Capacitacion laboral
                View.Report.LocalReport.DataSources.Add(DatosActivNoGratificadas(_ing, _IdEstudio));//Actividades no gratificadas
                View.Report.LocalReport.DataSources.Add(DatosActivGratificadas(_ing, _IdEstudio));//Actividades gratificadas
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioCapTrabPenitFC.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioCapacitacionTrabajoPenitenciarioFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosTrabajo = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosTrabajo != null)
                {
                    CamposBase.OficioActivDesempenadaAntesReclucion = _DatosTrabajo.P1_TRABAJO_ANTES;
                    CamposBase.ResponsabilidadTexto = string.Format("BUENA ( {0} ) \t REGULAR ( {1} ) \t MALA ( {2} ) ", _DatosTrabajo.P3_RESPONSABILIDAD == "B" ? "X" : string.Empty, _DatosTrabajo.P3_RESPONSABILIDAD == "R" ? "X" : string.Empty, _DatosTrabajo.P3_RESPONSABILIDAD == "M" ? "X" : string.Empty);
                    CamposBase.CalidadTrabajoTexto = string.Format("BUENA ( {0} ) \t REGULAR ( {1} ) \t MALA ( {2} ) ", _DatosTrabajo.P3_CALIDAD == "B" ? "X" : string.Empty, _DatosTrabajo.P3_CALIDAD == "R" ? "X" : string.Empty, _DatosTrabajo.P3_CALIDAD == "M" ? "X" : string.Empty);
                    CamposBase.PerseveranciaTexto = string.Format("BUENA ( {0} ) \t REGULAR ( {1} ) \t MALA ( {2} ) ", _DatosTrabajo.P3_PERSEVERANCIA == "B" ? "X" : string.Empty, _DatosTrabajo.P3_PERSEVERANCIA == "R" ? "X" : string.Empty, _DatosTrabajo.P3_PERSEVERANCIA == "M" ? "X" : string.Empty);
                    CamposBase.CuentaFondoAhorroTexto = string.Format(" SI ( {0} ) \n NO ( {1} )", _DatosTrabajo.P4_FONDO_AHORRO == "S" ? "X" : string.Empty, _DatosTrabajo.P4_FONDO_AHORRO == "N" ? "X" : string.Empty);
                    CamposBase.DiasOtrosCentros = _DatosTrabajo.P5_DIAS_OTROS_CENTROS.HasValue ? _DatosTrabajo.P5_DIAS_OTROS_CENTROS.Value.ToString() : string.Empty;
                    CamposBase.DiasCentroActual = _DatosTrabajo.P5_DIAS_CENTRO_ACTUAL.HasValue ? _DatosTrabajo.P5_DIAS_CENTRO_ACTUAL.Value.ToString() : string.Empty;
                    CamposBase.DiasTotalLaborados = _DatosTrabajo.P5_DIAS_LABORADOS.HasValue ? _DatosTrabajo.P5_DIAS_LABORADOS.Value.ToString() : string.Empty;
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", _DatosTrabajo.P6_DICTAMEN == "F" ? "X" : string.Empty, _DatosTrabajo.P6_DICTAMEN == "D" ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosTrabajo.P7_MOTIVACION;
                    CamposBase.FechaRealizacionEstudio = _DatosTrabajo.ESTUDIO_FEC.HasValue ? _DatosTrabajo.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = _DatosTrabajo.COORDINADOR;
                    CamposBase.Actitud = _DatosTrabajo.ELABORO;
                    CamposBase.TextoGenerico11 = _DatosTrabajo.P5_PERIODO_LABORAL.HasValue ? Fechas.fechaLetra(_DatosTrabajo.P5_PERIODO_LABORAL.Value, false).ToUpper() : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCapacitacionLaboral(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cCapacitacionLaboral>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var Padre = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var _DatosCapacitacion = new cActrividadesEstudioCapacitacionTrabajoFueroComun().GetData(x => x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    cCapacitacionLaboral CamposBase = new cCapacitacionLaboral();
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.PFC_VIII_CAPACITACION != null)
                            {
                                if (item.PFC_VIII_CAPACITACION.TIPO == "L")//Capacitacion Laboral
                                {
                                    CamposBase = new cCapacitacionLaboral()
                                    {
                                        Concluyo = item.CONCLUYO == "S" ? "SI" : "NO",
                                        Observaciones = item.OBSERVACION,
                                        Periodo = item.PERIODO,
                                        Taller = item.PFC_VIII_CAPACITACION != null ? !string.IsNullOrEmpty(item.PFC_VIII_CAPACITACION.DESCR) ? item.PFC_VIII_CAPACITACION.DESCR.Trim() : string.Empty : string.Empty
                                    };

                                    _Datos.Add(CamposBase);
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        };
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActivNoGratificadas(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cCapacitacionLaboral>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var Padre = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var _DatosCapacitacion = new cActrividadesEstudioCapacitacionTrabajoFueroComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    cCapacitacionLaboral CamposBase = new cCapacitacionLaboral();
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.PFC_VIII_CAPACITACION != null)
                            {
                                if (item.PFC_VIII_CAPACITACION.TIPO == "N")//Activ. no gratificadas
                                {
                                    CamposBase = new cCapacitacionLaboral()
                                    {
                                        Concluyo = item.CONCLUYO,
                                        Observaciones = item.OBSERVACION,
                                        Periodo = item.PERIODO,
                                        Taller = item.PFC_VIII_CAPACITACION != null ? !string.IsNullOrEmpty(item.PFC_VIII_CAPACITACION.DESCR) ? item.PFC_VIII_CAPACITACION.DESCR.Trim() : string.Empty : string.Empty
                                    };

                                    _Datos.Add(CamposBase);
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        };
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet4";
                return _respuesta;

            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActivGratificadas(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cCapacitacionLaboral>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cCapacitacionLaboral CamposBase = new cCapacitacionLaboral();
                var Padre = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var _DatosCapacitacion = new cActrividadesEstudioCapacitacionTrabajoFueroComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            if (item.PFC_VIII_CAPACITACION != null)
                            {
                                if (item.PFC_VIII_CAPACITACION.TIPO == "G")//Activ. gratificadas
                                {
                                    CamposBase = new cCapacitacionLaboral()
                                    {
                                        Concluyo = item.CONCLUYO,
                                        Observaciones = item.OBSERVACION,
                                        Periodo = item.PERIODO,
                                        Taller = item.PFC_VIII_CAPACITACION != null ? !string.IsNullOrEmpty(item.PFC_VIII_CAPACITACION.DESCR) ? item.PFC_VIII_CAPACITACION.DESCR.Trim() : string.Empty : string.Empty
                                    };

                                    _Datos.Add(CamposBase);
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        };
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet5";
                return _respuesta;

            }
            catch (Exception exc)
            {
                throw;
            }
        }
        #endregion

        #region SEGURIDAD COMUN
        private void SeguridadComun(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun(_ing));
                View.Report.LocalReport.DataSources.Add(DatosInformeAreaSeguridadCustodiaFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(SancionesInformeAreaSeguridadCustodiaFueroComun(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/RInformeAreaSeguridadCustodiaFC.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };


                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosInformeAreaSeguridadCustodiaFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                var datosReporte = new cInformeAreaSeguridadCustodiaComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (datosReporte != null)
                {
                    CamposBase.ConductaObservadaCentroTexto = string.Format(" ( {0} ) BUENA \t ( {1} ) REGULAR \t ( {2} ) MALA ", datosReporte.P1_CONDUCTA_CENTRO == "B" ? "X" : string.Empty, datosReporte.P1_CONDUCTA_CENTRO == "R" ? "X" : string.Empty, datosReporte.P1_CONDUCTA_CENTRO == "M" ? "X" : string.Empty);
                    CamposBase.ConductaAutoridadTexto = string.Format(" ( {0} ) BUENA \t ( {1} ) REGULAR \t ( {2} ) MALA ", datosReporte.P2_CONDUCTA_AUTORIDAD == "B" ? "X" : string.Empty, datosReporte.P2_CONDUCTA_AUTORIDAD == "R" ? "X" : string.Empty, datosReporte.P2_CONDUCTA_AUTORIDAD == "M" ? "X" : string.Empty);
                    CamposBase.ConductaGralTexto = string.Format(" ( {0} ) REBELDE ( {1} ) AGRESIVO ( {2} ) DISCIPLINADO ( {3} ) SE ADAPTA SIN CONFLICTOS ", datosReporte.P3_CONDUCTA_GENERAL == 1 ? "X" : string.Empty, datosReporte.P3_CONDUCTA_GENERAL == 2 ? "X" : string.Empty, datosReporte.P3_CONDUCTA_GENERAL == 3 ? "X" : string.Empty, datosReporte.P3_CONDUCTA_GENERAL == 4 ? "X" : string.Empty);
                    CamposBase.RelacionCompanerosTexto = string.Format(" ( {0} ) AISLAMIENTO ( {1} ) AGRESIVIDAD ( {2} ) CAMARADERIA ( {3} ) DOMINANTE ( {4} ) INDIFERENTE ", datosReporte.P4_RELACION_COMPANEROS == 1 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 2 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 3 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 4 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 5 ? "X" : string.Empty);
                    CamposBase.RegistraCorrectivosDiscTexto = string.Format(" SI ( {0} ) NO ( {1} )", datosReporte.P5_CORRECTIVOS == "S" ? "X" : string.Empty, datosReporte.P5_CORRECTIVOS == "N" ? "X" : string.Empty);
                    CamposBase.OpinionConductaGralInterno = string.Format(" ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ", datosReporte.P6_OPINION_CONDUCTA == "B" ? "X" : string.Empty, datosReporte.P6_OPINION_CONDUCTA == "R" ? "X" : string.Empty, datosReporte.P6_OPINION_CONDUCTA == "M" ? "X" : string.Empty);
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", datosReporte.P7_DICTAMEN == "F" ? "X" : string.Empty, datosReporte.P7_DICTAMEN == "D" ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = datosReporte.P8_MOTIVACION;
                    CamposBase.FechaRealizacionEstudio = datosReporte.ESTUDIO_FEC.HasValue ? datosReporte.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = datosReporte.COMANDANTE;
                    CamposBase.Actitud = datosReporte.ELABORO;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource SancionesInformeAreaSeguridadCustodiaFueroComun(INGRESO _ingreso, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _Datos = new List<cCorrectivosDisc>();
                cCorrectivosDisc CamposBase = new cCorrectivosDisc();
                var Padre = new cInformeAreaSeguridadCustodiaComun().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var datosReporte = new cCorrectivosSeguridadFueroComun().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ingreso.ID_INGRESO && x.ID_IMPUTADO == _ingreso.ID_IMPUTADO);
                    if (datosReporte != null && datosReporte.Any())
                        foreach (var item in datosReporte)
                        {
                            CamposBase = new cCorrectivosDisc()
                            {
                                Fecha = item.CORRECTIVO_FEC.HasValue ? item.CORRECTIVO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                                Motivo = item.MOTIVO,
                                Sancion = item.SANCION
                            };

                            _Datos.Add(CamposBase);
                        };
                };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
            }

            catch (System.Exception ex)
            {
                throw ex;
            }

            return _respuesta;
        }
        #endregion

        #region federal
        #region ACTA FEDERAL
        private void ActaFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosAreasTecnicasActaInterd(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosActaConsejoTecnicoInteridsciplinarioFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rActaConsejoTecInterdFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource EncabezadoReportesFueroFederal()
        {
            try
            {
                var ds1 = new List<cReporte>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                cReporte Encabezado = new cReporte();
                Encabezado.Encabezado1 = Parametro.ENCABEZADO_FUERO_FEDERAL_1;
                Encabezado.Encabezado2 = Parametro.ENCABEZADO_FUERO_FEDERAL_2;
                Encabezado.Encabezado3 = Parametro.ENCABEZADO_FUERO_FEDERAL_3;
                Encabezado.Encabezado4 = Parametro.ENCABEZADO_FUERO_FEDERAL_4;
                Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO_FUERO_FEDERAL_2;
                Encabezado.ImagenMedio = Parametro.REPORTE_LOGO_FUERO_FEDERAL_1;
                Encabezado.ImagenDerecha = Parametro.REPORTE_LOGO_FUERO_FEDERAL_3;
                ds1.Add(Encabezado);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                return rds1;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosAreasTecnicasActaInterd(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var _Datos = new List<cActaDeterminoRealizacionEstudiosPersonalidad>();
                var _DatosCapacitacion = new cActaDeterminoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_ANIO == _ing.ID_ANIO && x.ID_CENTRO == _ing.ID_CENTRO);
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cActaDeterminoRealizacionEstudiosPersonalidad CamposBase = new cActaDeterminoRealizacionEstudiosPersonalidad();
                if (_DatosCapacitacion != null)
                    if (_DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            CamposBase = new cActaDeterminoRealizacionEstudiosPersonalidad()
                            {
                                Nombre = item.NOMBRE,
                                NombreArea = item.AREA_TECNICA != null ? !string.IsNullOrEmpty(item.AREA_TECNICA.DESCR) ? item.AREA_TECNICA.DESCR.Trim() : string.Empty : string.Empty,
                                Opinion = !string.IsNullOrEmpty(item.OPINION) ? item.OPINION == "F" ? "FAVORABLE" : "DESFAVORABLE" : string.Empty
                            };

                            _Datos.Add(CamposBase);
                        };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;

            }
            catch (Exception exc)
            {
                throw;
            }
        }

        public string CalcularSentencia(ICollection<CAUSA_PENAL> CausaPenal)
        {
            try
            {
                if (CausaPenal != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (CausaPenal != null)
                    {
                        foreach (var cp in CausaPenal)
                        {
                            var segundaInstancia = false;
                            if (cp.SENTENCIA != null)
                            {
                                if (cp.SENTENCIA.Count > 0)
                                {
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
                                            FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        else
                                            if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;

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

                        return a + (a == 1 ? " Año " : " Años ") + m + (m == 1 ? " Mes " : " Meses ") + d + (d == 1 ? " Dia " : " Dias ");
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
            return string.Empty;
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActaConsejoTecnicoInteridsciplinarioFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                string _Del = string.Empty;
                string PartirDe = string.Empty;
                var CausaPenal = new cCausaPenal().GetData(x => x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO).FirstOrDefault();
                if (CausaPenal != null)
                {
                    var _delitos = new cCausaPenalDelito().Obtener(Convert.ToInt32(_ing.ID_CENTRO), Convert.ToInt32(_ing.ID_ANIO), Convert.ToInt32(_ing.ID_IMPUTADO), Convert.ToInt32(_ing.ID_INGRESO));
                    if (_delitos.Any())
                        foreach (var item in _delitos)
                        {
                            if (_delitos.Count > 1)
                                _Del += string.Format("{0} y ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                            else
                                _Del += string.Format("{0} ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                        };

                    var _Sentencia = new cSentencia().GetData(c => c.ID_INGRESO == _ing.ID_INGRESO && c.ID_IMPUTADO == _ing.ID_IMPUTADO && c.ID_CAUSA_PENAL == CausaPenal.ID_CAUSA_PENAL).FirstOrDefault();
                    if (_Sentencia != null)
                    {
                        var InicioComp = new cSentencia().GetData(x => x.ID_SENTENCIA == _Sentencia.ID_SENTENCIA && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_INGRESO == _ing.ID_INGRESO).FirstOrDefault();
                        if (InicioComp != null)
                            PartirDe = InicioComp.FEC_INICIO_COMPURGACION.HasValue ? InicioComp.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty;
                    };
                };

                var _Datos = new List<cRealizacionEstudios>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                CamposBase.NombreInterno = string.Format("{0} {1} {2}", _ing.IMPUTADO != null ? !string.IsNullOrEmpty(_ing.IMPUTADO.NOMBRE) ? _ing.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                    _ing.IMPUTADO != null ? !string.IsNullOrEmpty(_ing.IMPUTADO.PATERNO) ? _ing.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                    _ing.IMPUTADO != null ? !string.IsNullOrEmpty(_ing.IMPUTADO.MATERNO) ? _ing.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);

                var _DatosActHecha = new cActaConsejoTecnicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (_DatosActHecha != null)
                {
                    CamposBase.ExpInterno = _DatosActHecha.EXPEDIENTE;
                    CamposBase.NombreInterno = string.Format("{0} {1} {2}", _ing.IMPUTADO != null ? !string.IsNullOrEmpty(_ing.IMPUTADO.NOMBRE) ? _ing.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                    _ing.IMPUTADO != null ? !string.IsNullOrEmpty(_ing.IMPUTADO.PATERNO) ? _ing.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                    _ing.IMPUTADO != null ? !string.IsNullOrEmpty(_ing.IMPUTADO.MATERNO) ? _ing.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);
                    CamposBase.DelitoInterno = _Del;
                    CamposBase.SentenciaInterno = CalcularSentenciaString().ToUpper();
                    CamposBase.APartirDe = PartirDe;
                    CamposBase.EnSesionFecha = _DatosActHecha.SESION_FEC.HasValue ? _DatosActHecha.SESION_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;

                    var _Edo = new cEntidad().GetData(x => x.ID_ENTIDAD == Parametro.ESTADO).FirstOrDefault();
                    if (_Edo != null)
                        CamposBase.Estado = !string.IsNullOrEmpty(_Edo.DESCR) ? _Edo.DESCR.Trim() : string.Empty;

                    CamposBase.NombbreCentro = _DatosActHecha.CENTRO != null ? !string.IsNullOrEmpty(_DatosActHecha.CENTRO.DESCR) ? _DatosActHecha.CENTRO.DESCR.Trim() : string.Empty : string.Empty;
                    CamposBase.ActuacionTexto = _DatosActHecha.APROBADO_APLAZADO == "S" ? "APROBADO" : "APLAZADO";
                    CamposBase.VotosTexto = _DatosActHecha.APROBADO_POR == "M" ? "MAYORIA" : "UNANIMIDAD";
                    CamposBase.LugarDesc = _DatosActHecha.LUGAR;
                    CamposBase.TramiteTexto = !string.IsNullOrEmpty(_DatosActHecha.TRAMITE) ? _DatosActHecha.TRAMITE.Trim() : string.Empty;
                    CamposBase.DirectorCRS = !string.IsNullOrEmpty(_DatosActHecha.DIRECTOR) ? _DatosActHecha.DIRECTOR.Trim() : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region MEDIFCO FEDERAL
        private void MedicoFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoMedicoFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosToxicosMedicoFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioMedicoFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {

                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoMedicoFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioMedicoFederal = new cEstudioMedicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioMedicoFederal != null)
                    {
                        CamposBase.NombreInterno = !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.NOMBRE) ? _DatosEstudioMedicoFederal.NOMBRE.Trim() : string.Empty;
                        CamposBase.AliasInterno = !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.ALIAS) ? _DatosEstudioMedicoFederal.ALIAS.Trim() : string.Empty;
                        CamposBase.EdadInterno = _DatosEstudioMedicoFederal.EDAD.HasValue ? _DatosEstudioMedicoFederal.EDAD.Value.ToString() : string.Empty;
                        if (!string.IsNullOrEmpty(_DatosEstudioMedicoFederal.EDO_CIVIL))
                        {
                            short _EstInterno = short.Parse(_DatosEstudioMedicoFederal.EDO_CIVIL);
                            var _EdoCivil = new cEstadoCivil().GetData(x => x.ID_ESTADO_CIVIL == _EstInterno).FirstOrDefault();
                            if (_EdoCivil != null)
                                CamposBase.EstadoCivilInterno = !string.IsNullOrEmpty(_EdoCivil.DESCR) ? _EdoCivil.DESCR.Trim() : string.Empty;
                        };

                        CamposBase.OriginarioDeInterno = _DatosEstudioMedicoFederal.ORIGINARIO_DE;
                        CamposBase.OcupacionAnteriorInterno = _DatosEstudioMedicoFederal.OCUPACION_ANT.HasValue ? !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.OCUPACION.DESCR) ? _DatosEstudioMedicoFederal.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.OcupacionActualInterno = _DatosEstudioMedicoFederal.OCUPACION_ACT.HasValue ? !string.IsNullOrEmpty(_DatosEstudioMedicoFederal.OCUPACION1.DESCR) ? _DatosEstudioMedicoFederal.OCUPACION1.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.DelitoInterno = _DatosEstudioMedicoFederal.DELITO;
                        CamposBase.SentenciaInterno = _DatosEstudioMedicoFederal.SENTENCIA;
                        CamposBase.AntecedentesHeredoFamFederal = _DatosEstudioMedicoFederal.ANTE_HEREDO_FAM;
                        CamposBase.AntecedentesPersonalesNoPatFederal = _DatosEstudioMedicoFederal.ANTE_PERSONAL_NO_PATOLOGICOS;
                        CamposBase.AntecedentesPatoFederal = _DatosEstudioMedicoFederal.ANTE_PATOLOGICOS;
                        CamposBase.PadecimientoActual = _DatosEstudioMedicoFederal.PADECIMIENTO_ACTUAL;
                        CamposBase.InterrogAparatosSistFederal = _DatosEstudioMedicoFederal.INTERROGATORIO_APARATOS;
                        CamposBase.ExploracionFisicaCabezCuello = _DatosEstudioMedicoFederal.EXP_FIS_CABEZA_CUELLO;
                        CamposBase.Torax = _DatosEstudioMedicoFederal.EXP_FIS_TORAX;
                        CamposBase.Abdomen = _DatosEstudioMedicoFederal.EXP_FIS_ABDOMEN;
                        CamposBase.OrganosGenit = _DatosEstudioMedicoFederal.EXP_FIS_GENITALES;
                        CamposBase.Extremidades = _DatosEstudioMedicoFederal.EXP_FIS_EXTREMIDADES;
                        CamposBase.TensionArterial = _DatosEstudioMedicoFederal.TA;
                        CamposBase.Teperatura = _DatosEstudioMedicoFederal.TEMPERATURA;
                        CamposBase.Pulso = _DatosEstudioMedicoFederal.PULSO;
                        CamposBase.Respiracion = _DatosEstudioMedicoFederal.RESPIRACION;
                        CamposBase.Estatura = _DatosEstudioMedicoFederal.ESTATURA;
                        CamposBase.DescripcionTatuajesCicatrRecAntiguasMalformacionesFederal = _DatosEstudioMedicoFederal.TATUAJES;
                        CamposBase.Diagnostico = _DatosEstudioMedicoFederal.DIAGNOSTICO;
                        CamposBase.TerpeuticaImpl = _DatosEstudioMedicoFederal.RESULTADOS_OBTENIDOS;
                        CamposBase.Conclusion = _DatosEstudioMedicoFederal.CONCLUSION;
                        CamposBase.DirectorCRS = _DatosEstudioMedicoFederal.DIRECTOR_CENTRO;
                        CamposBase.LugarDesc = _DatosEstudioMedicoFederal.LUGAR;
                        CamposBase.MatricesRavenTexto = _DatosEstudioMedicoFederal.MEDICO;
                        CamposBase.TextoGenerico1 = string.Format("( {0} ) ", _DatosEstudioMedicoFederal.ASIST_FARMACODEPENDENCIA == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico2 = string.Format("( {0} ) ", _DatosEstudioMedicoFederal.ASIST_AA == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico3 = string.Format("( {0} ) ", _DatosEstudioMedicoFederal.ASIST_OTROS == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico4 = _DatosEstudioMedicoFederal.ASIST_OTROS_ESPECIF;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosToxicosMedicoFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleToxicos = new List<cAntecedentesConsumoToxicosMedicoFederal>();
                var Padre = new cEstudioMedicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cSustanciaToxicaFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cAntecedentesConsumoToxicosMedicoFederal
                            {
                                Cantidad = item.CANTIDAD.ToString(),
                                EdadInicio = item.EDAD_INICIO.HasValue ? item.EDAD_INICIO.Value.ToString() : string.Empty,
                                Periodicidad = item.PERIODICIDAD,
                                Tipo = item.DROGA != null ? !string.IsNullOrEmpty(item.DROGA.DESCR) ? item.DROGA.DESCR.Trim() : string.Empty : string.Empty
                            });
                        };
                    }
                    else
                    {
                        _DetalleToxicos.Add(new cAntecedentesConsumoToxicosMedicoFederal
                        {
                            Cantidad = string.Empty,
                            EdadInicio = string.Empty,
                            Periodicidad = string.Empty,
                            Tipo = string.Empty
                        });
                    }
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        #endregion

        #region PSICOLOGICO FEDERAL
        private void PsicologicoFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoPsicologicoFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioPsicologicoFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoPsicologicoFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioMedicoFederal = new cEstudioPsicologicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioMedicoFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioMedicoFederal.NOMBRE;
                        CamposBase.AliasInterno = _DatosEstudioMedicoFederal.SOBRENOMBRE;
                        CamposBase.EdadInterno = _DatosEstudioMedicoFederal.EDAD.HasValue ? _DatosEstudioMedicoFederal.EDAD.ToString() : string.Empty;
                        CamposBase.DelitoInterno = _DatosEstudioMedicoFederal.DELITO;
                        CamposBase.ActitudTomadaAntesEntrevista = string.Concat("ACTITUD TOMADA ANTE LA ENTREVISTA: ", _DatosEstudioMedicoFederal.ACTITUD);
                        CamposBase.ExamenMentalFF = string.Concat("EXAMEN MENTAL: ", _DatosEstudioMedicoFederal.EXAMEN_MENTAL);
                        CamposBase.PruebasAplicadas = string.Concat("PRUEBAS APLICADAS: ", _DatosEstudioMedicoFederal.PRUEBAS_APLICADAS);
                        CamposBase.NivelInt = _DatosEstudioMedicoFederal.NIVEL_INTELECTUAL;
                        CamposBase.CI = _DatosEstudioMedicoFederal.CI;
                        CamposBase.IndiceLesionOrganica = _DatosEstudioMedicoFederal.INDICE_LESION_ORGANICA;
                        CamposBase.DinamicaPersonalidadIngreso = _DatosEstudioMedicoFederal.DINAM_PERSON_INGRESO;
                        CamposBase.DinamicaPersonalidadActual = _DatosEstudioMedicoFederal.DINAM_PERSON_ACTUAL;
                        CamposBase.ResultadosTratamientoProp = _DatosEstudioMedicoFederal.RESULT_TRATAMIENTO;
                        CamposBase.RequiereTratExtraMurosTexto = string.Format("REQUERIMIENTOS DE CONTINUACIÓN DE TRATAMIENTO: SI ({0})  NO ({1})", _DatosEstudioMedicoFederal.REQ_CONT_TRATAMIENTO == "S" ? "X" : string.Empty, _DatosEstudioMedicoFederal.REQ_CONT_TRATAMIENTO == "N" ? "X" : string.Empty);
                        CamposBase.Abdomen = _DatosEstudioMedicoFederal.INTERNO;
                        CamposBase.Actitud = _DatosEstudioMedicoFederal.EXTERNO;
                        CamposBase.EspecifiqueExtraM = string.Format("ESPECIFIQUE: {0}", _DatosEstudioMedicoFederal.ESPECIFIQUE);
                        CamposBase.DirectorCRS = _DatosEstudioMedicoFederal.DIRECTOR_DENTRO;
                        CamposBase.LugarDesc = _DatosEstudioMedicoFederal.LUGAR;
                        CamposBase.Pronostico = string.Format("PRONÓSTICO DE REINTEGRACIÓN SOCIAL: {0}", _DatosEstudioMedicoFederal.PRONOSTICO_REINTEGRACION);
                        CamposBase.OpinionSobreOtorgamientoBeneficio = string.Format("OPINIÓN SOBRE EL OTORGAMIENTO DEL BENEFICIO: {0}", _DatosEstudioMedicoFederal.OPINION);
                        CamposBase.MatricesRavenTexto = _DatosEstudioMedicoFederal.PSICOLOGO;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region TRABAJO SOCIAL FEDERAL
        private void SocioFamiliarFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoTSFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosGrupoFamiliarPrimarioFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosGrupoFamiliarSecundarioFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioTrabajoSocFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoTSFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cEstudioTrabajoSocialFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.DialectoInterno = _DatosEstudioTSFederal.DIALECTO.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.DIALECTO1.DESCR) ? _DatosEstudioTSFederal.DIALECTO1.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.LugarFecNacInterno = _DatosEstudioTSFederal.LUGAR_NAC;
                        CamposBase.EscolaridadAnteriorIngreso = _DatosEstudioTSFederal.ESCOLARIDAD_CENTRO.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.ESCOLARIDAD1.DESCR) ? _DatosEstudioTSFederal.ESCOLARIDAD1.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.EscolaridadAct = _DatosEstudioTSFederal.ESCOLARIDAD_ACTUAL.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.ESCOLARIDAD.DESCR) ? _DatosEstudioTSFederal.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.OcupacionAnteriorInterno = _DatosEstudioTSFederal.TRABAJO_DESEMP_ANTES;
                        CamposBase.DomicilioInterno = _DatosEstudioTSFederal.DOMICILIO;
                        CamposBase.CaractZona = string.Format("URBANA ( {0} )  SUB - URBANA ( {1} )  RURAL ( {2} )  CRIMINOGENA (Existencia de bandas o pandillas, sobrepoblación, prostíbulos, cantinas, billares, etc.) ( {3} )",
                            _DatosEstudioTSFederal.ECO_FP_ZONA == "U" ? "X" : string.Empty, _DatosEstudioTSFederal.ECO_FP_ZONA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.ECO_FP_ZONA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.ECO_FP_ZONA == "N" ? "X" : string.Empty);
                        CamposBase.ResponsManutencionHogar = string.Format("RESPONSABLE(S) DE LA MANUTENCIÓN DEL HOGAR: {0}", _DatosEstudioTSFederal.ECO_FP_RESPONSABLE);
                        CamposBase.TotalIngresosMensuales = _DatosEstudioTSFederal.ECO_FP_TOTAL_INGRESOS_MEN.HasValue ? _DatosEstudioTSFederal.ECO_FP_TOTAL_INGRESOS_MEN.Value.ToString("c") : string.Empty;
                        CamposBase.TotalEgresosMensuales = _DatosEstudioTSFederal.ECO_FP_TOTAL_EGRESOS_MEN.HasValue ? _DatosEstudioTSFederal.ECO_FP_TOTAL_EGRESOS_MEN.Value.ToString("c") : string.Empty;
                        CamposBase.ActualmenteInternoCooperaEcon = _DatosEstudioTSFederal.ECO_FP_COOPERA_ACTUALMENTE;
                        CamposBase.TieneFondoAhorro = _DatosEstudioTSFederal.ECO_FP_FONDOS_AHORRO;
                        CamposBase.GrupoFamPrimarioTexto = string.Format(" GRUPO FAMILIAR: FUNCIONAL ( {0} ) DISFUNCIONAL ( {1} )", _DatosEstudioTSFederal.CARACT_FP_GRUPO == "F" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_GRUPO == "D" ? "X" : string.Empty);
                        CamposBase.RelacionesInterfamiliaresTexto = string.Format("RELACIONES INTERFAMILIARES: ADECUADAS ( {0} ) INADECUADAS ( {1} )", _DatosEstudioTSFederal.CARACT_FP_RELAC_INTERFAM == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_RELAC_INTERFAM == "I" ? "X" : string.Empty);
                        CamposBase.HuboViolenciaIntrafamTexto = string.Format("{0}", _DatosEstudioTSFederal.CARACT_FP_VIOLENCIA_FAM == "S" ? "SI" : "NO");
                        CamposBase.EspecificarViolenciaIntrafam = _DatosEstudioTSFederal.CARACT_FP_VIOLENCIA_FAM_ESPEFI;
                        CamposBase.NivelSocioEconomicoCultPrimario = string.Format("NIVEL SOCIO-ECONÓMICO Y CULTURAL: ALTO ( {0} ) MEDIO ( {1} ) BAJO ( {2} )", _DatosEstudioTSFederal.CARACT_FP_NIVEL_SOCIO_CULTURAL == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_NIVEL_SOCIO_CULTURAL == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FP_NIVEL_SOCIO_CULTURAL == "B" ? "X" : string.Empty);
                        CamposBase.AlgunIntegTieneAntecedPenales = string.Format("ALGÚN INTEGRANTE DE LA FAMILIA TIENE ANTECEDENTES PENALES O ADICCIÓN A ALGÚN ESTUPEFACIENTE O CUALQUIER TIPO DE TÓXICOS: {0}. \n\n ESPECIFIQUE: {1}", _DatosEstudioTSFederal.CARACT_FP_ANTECE_PENALES_ADIC == "S" ? "SI" : "NO", _DatosEstudioTSFederal.CARACT_FP_ANTECEDENTES_PENALES);
                        CamposBase.ConceptoTieneFamInterno = string.Format("CONCEPTO QUE TIENE LA FAMILIA DEL INTERNO: {0} ", _DatosEstudioTSFederal.CARACT_FP_CONCEPTO);
                        CamposBase.HijosUnionesAnt = _DatosEstudioTSFederal.CARACT_FS_HIJOS_ANT;
                        CamposBase.GrupoFamSec = string.Format("FUNCIONAL ( {0} )  DISFUNCIONAL ( {1} ) ", _DatosEstudioTSFederal.CARACT_FS_GRUPO == "F" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_GRUPO == "D" ? "X" : string.Empty);
                        CamposBase.RelacionesInterfamSecundario = string.Format("ADECUADAS ( {0} ) INADECUADAS ( {1} )", _DatosEstudioTSFederal.CARACT_FS_RELACIONES_INTERFAM == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_RELACIONES_INTERFAM == "I" ? "X" : string.Empty);
                        CamposBase.ViolenciaIntraFamGrupoSecundario = _DatosEstudioTSFederal.CARACT_FS_VIOLENCIA_INTRAFAM == "S" ? "SI" : "NO";
                        CamposBase.EspecificViolenciaGrupoSecundario = _DatosEstudioTSFederal.CARACT_FS_VIOLENCIA_INTRAFAM_E;
                        CamposBase.NivelSocioEconomicoCulturalGrupoSecundario = string.Format("ALTO ( {0} ) MEDIO ( {1} ) BAJO ( {2} )", _DatosEstudioTSFederal.CARACT_FS_NIVEL_SOCIO_CULTURAL == "A" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_NIVEL_SOCIO_CULTURAL == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_NIVEL_SOCIO_CULTURAL == "B" ? "X" : string.Empty);

                        CamposBase.NumHabitacionesTotal = string.Format("NÚMERO DE HABITACIONES EN TOTAL (SALA, COMEDOR, COCINA, RECAMARAS, BAÑO, CUARTO DE SERVICIO, ETC.): {0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_NUM_HABITACIO.HasValue ? _DatosEstudioTSFederal.CARACT_FS_VIVIEN_NUM_HABITACIO.Value.ToString() : string.Empty);
                        CamposBase.DescripcionVivienda = string.Format("CÓMO ES SU VIVIENDA (DESCRIPCIÓN, MATERIALES DE LOS QUE ESTÁ CONSTRUIDA): \n\n {0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_DESCRIPCION);
                        CamposBase.TransporteCercaVivienda = string.Format("EL TRANSPORTE ESTÁ CERCA DE SU VIVIENDA O TIENE QUE CAMINAR PARA TOMARLO:\n\n {0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_TRANSPORTE);
                        CamposBase.EnseresMobiliario = string.Format("{0}", _DatosEstudioTSFederal.CARACT_FS_VIVIEN_MOBILIARIO);
                        CamposBase.CaractZonaGrupoSec = string.Format("URBANA ( {0} )  SUB - URBANA ( {1} )  RURAL ( {2} )  CRIMINOGENA (Existencia de bandas o pandillas, sobrepoblación, prostíbulos, cantinas, billares, etc.) ( {3} )",
                            _DatosEstudioTSFederal.CARACT_FS_ZONA == "U" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_ZONA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_ZONA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CARACT_FS_ZONA == "N" ? "X" : string.Empty);
                        CamposBase.RelacionMedioExterno = string.Format("RELACIÓN CON MEDIO EXTERNO: {0}", _DatosEstudioTSFederal.CARACT_FS_RELACION_MEDIO_EXT);

                        CamposBase.AlgunMiembroPresentaProbConductaPara = string.Format("ALGÚN MIEMBRO DE LA FAMILIA PRESENTA PROBLEMAS DE CONDUCTA PARA Ó ANTISOCIAL: {0} ", _DatosEstudioTSFederal.CARACT_FS_PROBLEMAS_CONDUCTA == "S" ? "SI" : "NO");
                        CamposBase.DescrConducta = string.Format("ESPECIFIQUE: {0}", _DatosEstudioTSFederal.CARACT_FS_PROBLEMAS_CONDUCTA_E);
                        CamposBase.NoPersonasVividoManeraEstable = _DatosEstudioTSFederal.NUM_PAREJAS_ESTABLE;
                        CamposBase.TrabajoAntesReclusion = _DatosEstudioTSFederal.TRABAJO_DESEMP_ANTES;
                        CamposBase.TiempoLaborar = _DatosEstudioTSFederal.TIEMPO_LABORAR;
                        CamposBase.SueldoPercibidoGrupoSecundario = _DatosEstudioTSFederal.SUELDO_PERCIBIDO.HasValue ? _DatosEstudioTSFederal.SUELDO_PERCIBIDO.Value.ToString("c") : string.Empty;
                        CamposBase.OtrasAportacionesDeLaFamilia = string.Format("APARTE DEL INTERNO, SEÑALE OTRAS APORTACIONES ECONÓMICAS DE LA FAMILIA, QUIEN LAS REALIZA Y A CUÁNTO ASCIENDEN: {0}", _DatosEstudioTSFederal.APORTACIONES_FAM);
                        CamposBase.DistribucionGastoFamiliar = string.Format("DISTRIBUCIÓN DEL GASTO FAMILIAR: {0}", _DatosEstudioTSFederal.DISTRIBUCION_GASTO_FAM);
                        CamposBase.AlimentacionFamiliar = string.Format("LA ALIMENTACIÓN FAMILIAR EN QUE CONSISTE: {0}", _DatosEstudioTSFederal.ALIMENTACION_FAM);
                        CamposBase.ServiciosCuenta = string.Format("CON QUE SERVICIOS PÚBLICOS CUENTA (LUZ, AGUA, DRENAJE, ETC.): {0}", _DatosEstudioTSFederal.SERVICIOS_PUBLICOS);
                        CamposBase.CuentaOfertaTrabajoTexto = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.OFERTA_TRABAJO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.OFERTA_TRABAJO == "N" ? "X" : string.Empty);
                        CamposBase.EspecifiqueOfertaG = string.Format("EN QUÉ CONSISTE: {0}", _DatosEstudioTSFederal.OFERTA_TRABAJO_CONSISTE);
                        CamposBase.CuentaApoyoFamiliaAlgunaPersona = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.APOYO_FAM_OTROS == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.APOYO_FAM_OTROS == "N" ? "X" : string.Empty);
                        CamposBase.RecibeVisitaFam = string.Format("RECIBE VISITAS DE FAMILIARES: \t SI ( {0} )  NO ( {1} ) \n RADICAN EN EL ESTADO: SI( {2} )  NO( {3} )", _DatosEstudioTSFederal.VISITA_FAMILIARES == "S" ? "X" : string.Empty,
                          _DatosEstudioTSFederal.VISITA_FAMILIARES == "N" ? "X" : string.Empty,
                          _DatosEstudioTSFederal.RADICAN_ESTADO == "S" ? "X" : string.Empty,
                          _DatosEstudioTSFederal.RADICAN_ESTADO == "N" ? "X" : string.Empty);
                        CamposBase.FrecuenciaG = _DatosEstudioTSFederal.VISITA_FRECUENCIA;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                        CamposBase.VisitadoOtrasPersonas = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.VISITAS_OTROS == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.VISITAS_OTROS == "S" ? "X" : string.Empty);
                        CamposBase.QuienesVisitasTexto = string.Format("QUIENES: {0}", _DatosEstudioTSFederal.VISITA_OTROS_QUIIEN);
                        CamposBase.CuentaAvalMoralTexto = string.Format("AVAL MORAL (Nombre): {0}", _DatosEstudioTSFederal.AVAL_MORAL);
                        CamposBase.ConQuienViviraAlSerExternado = string.Format("CON QUIEN VA A VIVIR AL SER EXTERNADO: {0}", _DatosEstudioTSFederal.EXTERNADO_VIVIR_NOMBRE);
                        CamposBase.OpinionInternamiento = string.Format("CUÁL ES SU OPINIÓN ACERCA DE SU INTERNAMIENTO: {0}", _DatosEstudioTSFederal.OPINION_INTERNAMIENTO);
                        CamposBase.DeQueFormaInfluenciaEstarPrision = string.Format("DE QUÉ MANERA LE HA INFLUENCIADO SU ESTANCIA EN PRISIÓN: {0}", _DatosEstudioTSFederal.INFLUENCIADO_ESTANCIA_PRISION);
                        CamposBase.Diagnostico = string.Format("DIAGNÓSTICO SOCIAL Y PRONÓSTICO DE EXTERNACIÓN: {0}", _DatosEstudioTSFederal.DIAG_SOCIAL_PRONOS);
                        CamposBase.OpinionSobreOtorgamientoBeneficio = string.Format("ANOTE SU OPINIÓN SOBRE LA CONCESIÓN DE BENEFICIOS AL INTERNO EN ESTUDIO: {0}", _DatosEstudioTSFederal.OPINION_CONCESION_BENEFICIOS);
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.MatricesRavenTexto = _DatosEstudioTSFederal.TRABAJADORA_SOCIAL;
                        CamposBase.TextoGenerico2 = _DatosEstudioTSFederal.EXTERNADO_CALLE;
                        CamposBase.TextoGenerico3 = _DatosEstudioTSFederal.EXTERNADO_NUMERO;
                        CamposBase.TextoGenerico5 = _DatosEstudioTSFederal.EXTERNADO_CP;
                        if (_DatosEstudioTSFederal.EXTERNADO_PARENTESCO.HasValue)
                        {
                            var _parentescoExternado = new cTipoReferencia().GetData(x => x.ID_TIPO_REFERENCIA == _DatosEstudioTSFederal.EXTERNADO_PARENTESCO).FirstOrDefault();
                            if (_parentescoExternado != null)
                                CamposBase.TextoGenerico9 = !string.IsNullOrEmpty(_parentescoExternado.DESCR) ? _parentescoExternado.DESCR.Trim() : string.Empty;
                        };

                        if (_DatosEstudioTSFederal.VISTA_PARENTESCO.HasValue)
                        {
                            var _parentescoExternado = new cTipoReferencia().GetData(x => x.ID_TIPO_REFERENCIA == _DatosEstudioTSFederal.VISTA_PARENTESCO).FirstOrDefault();
                            if (_parentescoExternado != null)
                                CamposBase.TextoGenerico1 = !string.IsNullOrEmpty(_parentescoExternado.DESCR) ? _parentescoExternado.DESCR.Trim() : string.Empty;
                        };

                        if (_DatosEstudioTSFederal.EXTERNADO_ENTIDAD.HasValue)
                        {
                            var _Entidad = new cEntidad().GetData(x => x.ID_ENTIDAD == _DatosEstudioTSFederal.EXTERNADO_ENTIDAD).FirstOrDefault();
                            CamposBase.TextoGenerico8 = CamposBase.TextoGenerico7 = _Entidad != null ? !string.IsNullOrEmpty(_Entidad.DESCR) ? _Entidad.DESCR.Trim() : string.Empty : string.Empty;
                            if (_Entidad != null)
                            {
                                var _Municipio = new cMunicipio().GetData(x => x.ID_ENTIDAD == _Entidad.ID_ENTIDAD && x.ID_MUNICIPIO == _DatosEstudioTSFederal.EXTERNADO_MUNICIPIO).FirstOrDefault();
                                CamposBase.TextoGenerico6 = _Municipio != null ? !string.IsNullOrEmpty(_Municipio.MUNICIPIO1) ? _Municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty;

                                if (_Municipio != null)
                                {
                                    var _Colonia = new cColonia().GetData(x => x.ID_ENTIDAD == _Entidad.ID_ENTIDAD && x.ID_MUNICIPIO == _Municipio.ID_MUNICIPIO).FirstOrDefault();
                                    CamposBase.TextoGenerico4 = _Colonia != null ? !string.IsNullOrEmpty(_Colonia.DESCR) ? _Colonia.DESCR.Trim() : string.Empty : string.Empty;
                                };
                            };
                        };
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosGrupoFamiliarPrimarioFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var Padre = new cEstudioTrabajoSocialFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleToxicos = new List<cGrupoFamiliarPrimarioDatos>();
                if (Padre != null)
                {
                    var Datos = new cGrupoFamiliarFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_GRUPO_FAMILIAR == (short)eTipopGrupoTrabajoSocial.PRIMARIO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cGrupoFamiliarPrimarioDatos
                            {
                                Edad = item.EDAD,
                                EdoCivil = item.ESTADO_CIVIL,
                                Nombre = item.NOMBRE,
                                Ocupacion = item.OCUPACION,
                                PArentesco = item.PARENTESCO
                            });
                        };
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosGrupoFamiliarSecundarioFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var Padre = new cEstudioTrabajoSocialFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleToxicos = new List<cGrupoFamiliarPrimarioDatos>();
                if (Padre != null)
                {
                    var Datos = new cGrupoFamiliarFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO && x.ID_GRUPO_FAMILIAR == (short)eTipopGrupoTrabajoSocial.SECUNDARIO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cGrupoFamiliarPrimarioDatos
                            {
                                Edad = item.EDAD,
                                EdoCivil = item.ESTADO_CIVIL,
                                Nombre = item.NOMBRE,
                                Ocupacion = item.OCUPACION,
                                PArentesco = item.PARENTESCO
                            });
                        };
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private string MesString(short Mes)
        {
            try
            {
                string[] meses = { "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE" };
                int d = int.Parse(Mes.ToString());
                return meses[d];
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        #endregion

        #region CAPACITACION FEDERAL
        private void CapacitacionFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoActividadesProductCapacFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosDiasLaboradosCapacitacionFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosCursosCapacitacionFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rInformeActivProducCapFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoActividadesProductCapacFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cCapacitacionFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.NoD = string.Format("{0}-{1}{2}-{3}", _ing.CAMA != null ?
                            _ing.CAMA.CELDA != null ? _ing.CAMA.CELDA.SECTOR != null ? _ing.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(_ing.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? _ing.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            _ing.CAMA != null ? _ing.CAMA.CELDA != null ? _ing.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(_ing.CAMA.CELDA.SECTOR.DESCR) ? _ing.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                            _ing.CAMA != null ? _ing.CAMA.CELDA != null ? !string.IsNullOrEmpty(_ing.CAMA.CELDA.ID_CELDA) ? _ing.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty, _ing.ID_UB_CAMA);
                        CamposBase.OficioActivDesempenadaAntesReclucion = _DatosEstudioTSFederal.OFICIO_ANTES_RECLUSION.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.OCUPACION.DESCR) ? _DatosEstudioTSFederal.OCUPACION.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.SueldoPercibidoGrupoSecundario = _DatosEstudioTSFederal.SALARIO_DEVENGABA_DETENCION.HasValue ? _DatosEstudioTSFederal.SALARIO_DEVENGABA_DETENCION.Value.ToString("c") : string.Empty;
                        CamposBase.ActividadProductivaActualDentroCentro = _DatosEstudioTSFederal.ACTIVIDAD_PRODUC_ACTUAL;
                        CamposBase.AtiendeIndicacionesSuperiores = _DatosEstudioTSFederal.ATIENDE_INDICACIONES == "S" ? "SI" : "NO";
                        CamposBase.SatisfaceActividad = _DatosEstudioTSFederal.SATISFACE_ACTIVIDAD == "S" ? "SI" : "NO";
                        CamposBase.DescuidadoCumplimientoLabores = _DatosEstudioTSFederal.DESCUIDADO_LABORES == "S" ? "SI" : "NO";
                        CamposBase.MotivosTiempoInterrupcionActiv = string.Format("MOTIVOS Y TIEMPO DE LAS INTERRUPCIONES EN LA ACTIVIDAD: {0}", _DatosEstudioTSFederal.MOTIVO_TIEMPO_INTERRUP_ACT);
                        CamposBase.RecibioConstancia = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.RECIBIO_CONSTANCIA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.RECIBIO_CONSTANCIA == "N" ? "X" : string.Empty);
                        CamposBase.EspecifiqueAsistenciaGruposFederal = string.Format("EN CASO DE NO HABER ASISTIDO A CURSOS, ESPECIFIQUE EL MOTIVO: {0}", _DatosEstudioTSFederal.NO_CURSOS_MOTIVO);
                        CamposBase.CambiadoActividades = string.Format("¿HA CAMBIADO DE ACTIVIDAD? {0}. \t ¿POR QUÉ? {1} ", _DatosEstudioTSFederal.CAMBIO_ACTIVIDAD == "S" ? "SI" : "NO", _DatosEstudioTSFederal.CAMBIO_ACTIVIDAD_POR_QUE);
                        CamposBase.ActitudesHaciaDesempenoActivProduct = string.Format("ACTITUDES HACIA EL DESEMPEÑO DE ACTIVIDADES PRODUCTIVAS: {0}", _DatosEstudioTSFederal.ACTITUDES_DESEMPENO_ACT);
                        CamposBase.CuentaFondoAhorroTexto = string.Format("SI ( {0} )\t NO ( {1} )", _DatosEstudioTSFederal.FONDO_AHORRO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.FONDO_AHORRO == "N" ? "X" : string.Empty);
                        CamposBase.CompensacionRecibeActualmen = _DatosEstudioTSFederal.FONDO_AHORRO_COMPESACION_ACTUA;
                        CamposBase.TotalDiasLaboradosEfect = _DatosEstudioTSFederal.A_TOTAL_DIAS_LABORADOS.HasValue ? _DatosEstudioTSFederal.A_TOTAL_DIAS_LABORADOS.Value.ToString() : string.Empty;
                        CamposBase.DiasOtrosCentros = _DatosEstudioTSFederal.B_DIAS_LABORADOS_OTROS_CERESOS.HasValue ? _DatosEstudioTSFederal.B_DIAS_LABORADOS_OTROS_CERESOS.Value.ToString() : string.Empty;
                        CamposBase.DiasTotalLaborados = _DatosEstudioTSFederal.TOTAL_A_B.HasValue ? _DatosEstudioTSFederal.TOTAL_A_B.Value.ToString() : string.Empty;
                        CamposBase.Conclusion = _DatosEstudioTSFederal.CONCLUSIONES;
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                        CamposBase.TextoGenerico1 = _DatosEstudioTSFederal.JEFE_SECC_INDUSTRIAL;
                        CamposBase.HaProgresadoOficio = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.HA_PROGRESADO_OFICIO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.HA_PROGRESADO_OFICIO == "N" ? "X" : string.Empty);
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosCursosCapacitacionFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cCursoCapacitacionFueroFederal>();
                var Padre = new cCapacitacionFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cCapacitacionCursoFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cCursoCapacitacionFueroFederal
                            {
                                Curso = item.CURSO,
                                FechaFin = item.FECHA_TERMINO.HasValue ? item.FECHA_TERMINO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                FechaInicio = item.FECHA_INICIO.HasValue ? item.FECHA_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty
                            });
                        };
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet4";
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return _respuesta;
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosDiasLaboradosCapacitacionFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cDiasLaboradosFueroFed>();
                var Padre = new cCapacitacionFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cDiasLaboradosFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                            {
                                Mes = MesString(item.MES),
                                Anio = item.ANIO.ToString(),
                                DiasLab = item.DIAS_TRABAJADOS.ToString()
                            });
                        };
                    }
                    else
                    {
                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "ENERO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "FEBRERO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "MARZO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "ABRIL",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "MAYO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "JUNIO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "JULIO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "AGOSTO",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "SEPTIEMBRE",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "OCTUBRE",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "NOVIEMBRE",
                            DiasLab = decimal.Zero.ToString()
                        });

                        _DetalleToxicos.Add(new cDiasLaboradosFueroFed
                        {
                            Mes = "DICIEMBRE",
                            DiasLab = decimal.Zero.ToString()
                        });
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _respuesta;
        }

        private string DiaStringByDayOfWeek(int Dia)
        {
            try
            {
                string[] semana = { "DOMINGO", "LUNES", "MARTES", "MIERCOLES", "JUEVES", "VIERNES", "SABADO" };
                return semana[Dia];
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        #endregion

        #region EDUCATIVAS FEDERAL
        private void EducativasFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoInformeActivEducCultFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosParticipacionesFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rInformeActEducCultDepRecCivFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoInformeActivEducCultFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cActividadesFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.EscolaridadAnteriorIngreso = _DatosEstudioTSFederal.ESCOLARIDAD_MOMENTO.HasValue ? !string.IsNullOrEmpty(_DatosEstudioTSFederal.ESCOLARIDAD.DESCR) ? _DatosEstudioTSFederal.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                        CamposBase.EstudiosHaRealizadoInternamiento = _DatosEstudioTSFederal.ESTUDIOS_EN_INTERNAMIENTO;
                        CamposBase.EstudiosCursaActualmente = _DatosEstudioTSFederal.ESTUDIOS_ACTUALES;
                        CamposBase.AsisteEscuelaVoluntPuntualidadAsist = string.Format("SI ( {0} ) \t NO ( {1} ) \t  EN CASO NEGATIVO ¿PORQUE? {2}", _DatosEstudioTSFederal.ASISTE_PUNTUAL == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.ASISTE_PUNTUAL == "N" ? "X" : string.Empty, _DatosEstudioTSFederal.ASISTE_PUNTUAL_NO_POR_QUE);
                        CamposBase.AvanceRendimientoAcademico = string.Format("¿CUÁL ES SU AVANCE Y RENDIMIENTO ACADÉMICO? {0}", _DatosEstudioTSFederal.AVANCE_RENDIMIENTO_ACADEMINCO);
                        CamposBase.HaSidoPromovido = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.PROMOVIDO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.PROMOVIDO == "N" ? "X" : string.Empty);
                        CamposBase.Abdomen = string.Format("( {0} ) ", _DatosEstudioTSFederal.ALFABE_PRIMARIA == "S" ? "X" : string.Empty);
                        CamposBase.Actitud = string.Format("( {0} ) ", _DatosEstudioTSFederal.PRIMARIA_SECU == "S" ? "X" : string.Empty);
                        CamposBase.ActitudesHaciaDesempenoActivProduct = string.Format("( {0} ) ", _DatosEstudioTSFederal.SECU_BACHILLER == "S" ? "X" : string.Empty);
                        CamposBase.ActitudTomadaAntesEntrevista = string.Format("( {0} ) ", _DatosEstudioTSFederal.BACHILLER_UNI == "S" ? "X" : string.Empty);
                        CamposBase.ActividadProductivaActualDentroCentro = string.Format("( {0} ) ", _DatosEstudioTSFederal.OTRO == "S" ? "X" : string.Empty);
                        CamposBase.EspecifiqueOtrasPromociones = _DatosEstudioTSFederal.ESPECIFIQUE;
                        CamposBase.EnsenanzaRecibe = _DatosEstudioTSFederal.OTRA_ENSENANZA;
                        CamposBase.HaImpartidoEnsenanza = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA == "N" ? "X" : string.Empty);
                        CamposBase.DeQueTipoEnsenanza = _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA_TIPO;
                        CamposBase.CuantoTiempoEnsenanzaImpartida = _DatosEstudioTSFederal.IMPARTIDO_ENSENANZA_TIEMPO;
                        CamposBase.Conclusion = _DatosEstudioTSFederal.CONCLUSIONES;
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.MatricesRavenTexto = _DatosEstudioTSFederal.JEFE_SECC_EDUCATIVA;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                };
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return _respuesta;

        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosParticipacionesFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cParticipacionFueroFederal>();
                var Padre = new cActividadesFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cActividadParticipacionFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cParticipacionFueroFederal
                            {
                                AnioD = item.FECHA_1.HasValue ? item.FECHA_1.Value.Year.ToString() : string.Empty,
                                AnioE = item.FECHA_2.HasValue ? item.FECHA_2.Value.Year.ToString() : string.Empty,
                                Programa = item.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.TIPO_PROGRAMA.NOMBRE) ? item.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty,
                                Particip = string.Format("SI({0})  NO ({1})", item.PARTICIPACION == "S" ? "X" : string.Empty, item.PARTICIPACION == "N" ? "X" : string.Empty),
                                FecInicio = item.FECHA_1.HasValue ? item.FECHA_1.Value.ToString("dd/MM/yyyy") : string.Empty,
                                FecFin = item.FECHA_2.HasValue ? item.FECHA_2.Value.ToString("dd/MM/yyyy") : string.Empty,
                                MesD = item.FECHA_1.HasValue ? MesString(short.Parse(item.FECHA_1.Value.Month.ToString())) : string.Empty,
                                MesE = item.FECHA_2.HasValue ? MesString(short.Parse(item.FECHA_2.Value.Month.ToString())) : string.Empty,
                                DiaD = item.FECHA_1.HasValue ? DiaStringByDayOfWeek((int)(item.FECHA_1.Value.DayOfWeek)) : string.Empty,
                                DiaE = item.FECHA_2.HasValue ? DiaStringByDayOfWeek((int)(item.FECHA_2.Value.DayOfWeek)) : string.Empty
                            });
                        };
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
            }
            catch (Exception ex)
            {
            }

            return _respuesta;
        }
        #endregion

        #region VIGILANCIA FEDERAL
        private void VigilanciaFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoInformeVigilanciaFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.DataSources.Add(DatosCorrectivosVigiFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rInformeSeccVigilanciaFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoInformeVigilanciaFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cVigilanciaFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.FechaIngreso = _DatosEstudioTSFederal.FECHA_INGRESO.HasValue ? _DatosEstudioTSFederal.FECHA_INGRESO.Value.ToString("dd/MM/yyyy") : string.Empty;
                        CamposBase.CeresoProcede = _DatosEstudioTSFederal.CENTRO_DONDE_PROCEDE;
                        CamposBase.ConductaObservoEnElMismo = string.Format("EXCELENTE ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ( {3} )", _DatosEstudioTSFederal.CONDUCTA == "E" ? "X" : string.Empty
                            , _DatosEstudioTSFederal.CONDUCTA == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA == "M" ? "X" : string.Empty);
                        CamposBase.MotivoTraslado = _DatosEstudioTSFederal.MOTIVO_TRASLADO;
                        CamposBase.ConductaSuperioresTexto = string.Format("EXCELENTE ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ( {3} )", _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "E" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "B" ? "X" : string.Empty
                            , _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_SUPERIORES == "M" ? "X" : string.Empty);
                        CamposBase.RelacionCompanieros = _DatosEstudioTSFederal.RELACION_COMPANEROS;
                        CamposBase.DescrConducta = _DatosEstudioTSFederal.DESCRIPCION_CONDUCTA;
                        CamposBase.HigienePersonalTexto = string.Format("BUENA ( {0} ) REGULAR ( {1} ) MALA ( {2} )", _DatosEstudioTSFederal.HIGIENE_PERSONAL == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_PERSONAL == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_PERSONAL == "M" ? "X" : string.Empty);
                        CamposBase.HigieneCeldaTexto = string.Format("BUENA ( {0} ) REGULAR ( {1} ) MALA ( {2} )", _DatosEstudioTSFederal.HIGIENE_CELDA == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_CELDA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.HIGIENE_CELDA == "M" ? "X" : string.Empty);
                        CamposBase.RecibeVisText = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.VISITA_RECIBE == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.VISITA_RECIBE == "N" ? "X" : string.Empty);
                        CamposBase.FrecuenciaG = string.Format("FRECUENCIA: {0}", _DatosEstudioTSFederal.VISITA_FRECUENCIA);
                        CamposBase.DeQuienesVisita = string.Format("DE QUIÉNES : {0}", _DatosEstudioTSFederal.VISITA_QUIENES);
                        CamposBase.ConductaFamilia = string.Format("BUENA ( {0} ) REGULAR ( {1} ) MALA ( {2} )", _DatosEstudioTSFederal.CONDUCTA_FAMILIA == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_FAMILIA == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_FAMILIA == "M" ? "X" : string.Empty);
                        CamposBase.EstimulosBuenaCond = _DatosEstudioTSFederal.ESTIMULOS_BUENA_CONDUCTA;
                        CamposBase.ClasificConsudtaGral = string.Format("EXCELENTE ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ( {3} )", _DatosEstudioTSFederal.CONDUCTA_GENERAL == "E" ? "X" : string.Empty
                            , _DatosEstudioTSFederal.CONDUCTA_GENERAL == "B" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_GENERAL == "R" ? "X" : string.Empty, _DatosEstudioTSFederal.CONDUCTA_GENERAL == "M" ? "X" : string.Empty);
                        CamposBase.Conclusion = string.Format("CONCLUSIONES: {0}", _DatosEstudioTSFederal.CONCLUSIONES);
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.TextoGenerico1 = _DatosEstudioTSFederal.JEFE_VIGILANCIA;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                        CamposBase.RecibeVisitasTexto = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.VISITA_RECIBE == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.VISITA_RECIBE == "N" ? "X" : string.Empty);
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource DatosCorrectivosVigiFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _DetalleToxicos = new List<cCorrectivosVigilanciaFF>();
                var Padre = new cVigilanciaFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cCorrectivoFueroFederal().GetData(x => x.ID_ESTUDIO == Padre.ID_ESTUDIO && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleToxicos.Add(new cCorrectivosVigilanciaFF
                            {
                                Anio = item.FECHA.HasValue ? item.FECHA.Value.Year.ToString() : string.Empty,
                                Motivo = item.MOTIVO,
                                ResolucionH = item.RESOLUCION,
                                Mes = item.FECHA.HasValue ? MesString(short.Parse(item.FECHA.Value.Month.ToString())) : string.Empty,
                                Dia = item.FECHA.HasValue ? DiaStringByDayOfWeek((int)(item.FECHA.Value.DayOfWeek)) : string.Empty
                            });
                        };
                    };
                };

                _respuesta.Value = _DetalleToxicos;
                _respuesta.Name = "DataSet3";
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return _respuesta;
        }
        #endregion

        #region CRIMINOLOICO FEDERAL
        private void CriminologicoFederal(INGRESO _ing, short _IdEstudio)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                View.Report.LocalReport.DataSources.Add(DatosCuerpoCriminologicoFueroFederal(_ing, _IdEstudio));
                View.Report.LocalReport.ReportPath = "Reportes/rEstudioCriminologicoFF.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                TextControlView tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;

                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                        foreach (var item in tc.editor.Paragraphs)
                        {
                            var _parrafo = item as TXTextControl.Paragraph;
                            if (!string.IsNullOrEmpty(_parrafo.Text))
                            {
                                if (_parrafo.Text.Contains(".:."))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;
                                    _parrafo.Text.Replace(".:.", " ");
                                };

                                if (_parrafo.Text.Contains("~.:.~"))
                                {
                                    _parrafo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                                    _parrafo.Text.Replace("~.:.~", " ");
                                };
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosCuerpoCriminologicoFueroFederal(INGRESO _ing, short _IdEstudio)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
            try
            {
                if (_ing != null && _ing.IMPUTADO != null)
                {
                    var _Datos = new List<cRealizacionEstudios>();
                    var _DatosEstudioTSFederal = new cCriminologicoFueroFederal().GetData(x => x.ID_ESTUDIO == _IdEstudio && x.ID_INGRESO == _ing.ID_INGRESO && x.ID_IMPUTADO == _ing.ID_IMPUTADO).FirstOrDefault();
                    cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                    if (_DatosEstudioTSFederal != null)
                    {
                        CamposBase.NombreInterno = _DatosEstudioTSFederal.NOMBRE;
                        CamposBase.AliasInterno = _DatosEstudioTSFederal.SOBRENOMBRE;
                        CamposBase.VersionDelitoSegunInterno = _DatosEstudioTSFederal.P1_VERSION_INTERNO;
                        CamposBase.CaractPersonalidadRelDel = _DatosEstudioTSFederal.P2_PERSONALIDAD;
                        CamposBase.ReqValoracionVictim = string.Format("SI ( {0} )  NO ( {1} )", _DatosEstudioTSFederal.P3_VALORACION == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.P3_VALORACION == "N" ? "X" : string.Empty);
                        CamposBase.AntecedentesParasoc = _DatosEstudioTSFederal.ANTECEDENTES_PARA_ANTI_SOCIALE;
                        CamposBase.ClasificCriminologTexto = string.Format("PRIMODELINCUENTE: {0} \n REINCIDENTE ESPECÍFICO: {1} \t\t HABITUAL: {2} \n REINCIDENTE GENÉRICO: {3} \t\t PROFESIONAL: {4}",
                            _DatosEstudioTSFederal.P5_PRIMODELINCUENTE == "S" ? "X" : string.Empty,
                            _DatosEstudioTSFederal.P5_ESPECIFICO == "S" ? "X" : string.Empty,
                            _DatosEstudioTSFederal.P5_HABITUAL == "S" ? "X" : string.Empty,
                            _DatosEstudioTSFederal.P5_GENERICO == "S" ? "X" : string.Empty,
                            _DatosEstudioTSFederal.P5_PROFESIONAL == "S" ? "X" : string.Empty);
                        CamposBase.TextoGenerico1 = _DatosEstudioTSFederal.P5_PRIMODELINCUENTE == "S" ? "X" : string.Empty;
                        CamposBase.TextoGenerico2 = _DatosEstudioTSFederal.P5_ESPECIFICO == "S" ? "X" : string.Empty;
                        CamposBase.TextoGenerico3 = _DatosEstudioTSFederal.P5_HABITUAL == "S" ? "X" : string.Empty;
                        CamposBase.TextoGenerico4 = _DatosEstudioTSFederal.P5_GENERICO == "S" ? "X" : string.Empty;
                        CamposBase.TextoGenerico5 = _DatosEstudioTSFederal.P5_PROFESIONAL == "S" ? "X" : string.Empty;
                        CamposBase.Criminogenesis = _DatosEstudioTSFederal.P6_CRIMINOGENESIS;
                        CamposBase.EgocentrismoTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_EGOCENTRISMO == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_EGOCENTRISMO == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_EGOCENTRISMO == "B" ? "X" : string.Empty);
                        CamposBase.LabilidadAfectivaTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_LABILIDAD == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_LABILIDAD == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_LABILIDAD == "B" ? "X" : string.Empty);
                        CamposBase.AgresividadTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_AGRESIVIDAD == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_AGRESIVIDAD == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_AGRESIVIDAD == "B" ? "X" : string.Empty);
                        CamposBase.IndiferenciaAfectTexto = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _DatosEstudioTSFederal.P7_INDIFERENCIA == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P7_INDIFERENCIA == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P7_INDIFERENCIA == "B" ? "X" : string.Empty);
                        CamposBase.ResultTratamInstitucional = _DatosEstudioTSFederal.P8_RESULTADO_TRATAMIENTO;
                        CamposBase.EstadoPeligrosidadActual = string.Format("MINIMO ( {0} )  MÍNIMO MEDIO ( {1} )  MEDIO ( {2} )  MEDIO ALTO ( {3} )  ALTO ( {4} )", _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("1") ? "X" : string.Empty
                            , _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("2") ? "X" : string.Empty, _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("3") ? "X" : string.Empty, _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("4") ? "X" : string.Empty
                            , _DatosEstudioTSFederal.P8_ESTADO_PELIGRO == Convert.ToString("5") ? "X" : string.Empty);
                        CamposBase.OpinionSobreOtorgamientoBeneficio = _DatosEstudioTSFederal.P10_OPINION;
                        CamposBase.ReqContinuacionTratTexto = string.Format("SI ( {0} ) \t NO ( {1} )", _DatosEstudioTSFederal.P10_CONTINUAR_TRATAMIENTO == "S" ? "X" : string.Empty, _DatosEstudioTSFederal.P10_CONTINUAR_TRATAMIENTO == "N" ? "X" : string.Empty);
                        CamposBase.Abdomen = string.Concat("EN CASO AFIRMATIVO ESPECIFICAR: ", _DatosEstudioTSFederal.P10_CONTINUAR_SI_ESPECIFICAR);
                        CamposBase.Actitud = string.Concat("EN CASO NEGATIVO ESPECIFICAR: ", _DatosEstudioTSFederal.P10_CONTINUAR_NO_ESPECIFICAR);
                        CamposBase.DirectorCRS = _DatosEstudioTSFederal.DIRECTOR_CENTRO;
                        CamposBase.MatricesRavenTexto = _DatosEstudioTSFederal.CRIMINOLOGO;
                        CamposBase.LugarDesc = _DatosEstudioTSFederal.LUGAR;
                        CamposBase.ProbabilidadReincidencia = string.Format("ALTA ( {0} )  MEDIA ( {1} )  BAJA ( {2} )", _DatosEstudioTSFederal.P9_PRONOSTICO == "A" ? "X" : string.Empty,
                        _DatosEstudioTSFederal.P9_PRONOSTICO == "M" ? "X" : string.Empty, _DatosEstudioTSFederal.P9_PRONOSTICO == "B" ? "X" : string.Empty);
                    };

                    _Datos.Add(CamposBase);
                    _respuesta.Value = _Datos;
                    _respuesta.Name = "DataSet2";
                }

                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #endregion

        //private void MuestraOficioRemisionDictamenEstudioPersonalidad(PERSONALIDAD _Entity)
        //{
        //    try
        //    {
        //        if (_Entity == null)
        //            return;

        //        var View = new ReportesView();
        //        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
        //        View.Owner = PopUpsViewModels.MainWindow;
        //        View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
        //        View.Show();
        //        var NombreAreasTecnicas = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 1).FirstOrDefault();
        //        cFormatoDictamenEstudiosPersonalidad DatosReporte = new cFormatoDictamenEstudiosPersonalidad();
        //        var _datos = new cActaConsejoTecnicoComun().GetData(x => x.ID_IMPUTADO == _Entity.ID_IMPUTADO && x.ID_INGRESO == _Entity.ID_INGRESO).FirstOrDefault();
        //        var _centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
        //        if (_datos != null)
        //        {
        //            DatosReporte = new cFormatoDictamenEstudiosPersonalidad()
        //            {
        //                Expediente = string.Format("Expediente: {0} / {1}", _Entity.ID_ANIO, _Entity.ID_IMPUTADO),
        //                NombreInterno = string.Format("{0} \n ({1})",
        //                    _datos.INGRESO != null ? _datos.INGRESO.IMPUTADO != null ?
        //                        string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_datos.INGRESO.IMPUTADO.NOMBRE) ? _datos.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
        //                                                     !string.IsNullOrEmpty(_datos.INGRESO.IMPUTADO.PATERNO) ? _datos.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
        //                                                     !string.IsNullOrEmpty(_datos.INGRESO.IMPUTADO.MATERNO) ? _datos.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty,
        //                _datos.OPINION == "S" ? "APROBADO" : "APROBADO POR MAYORIA"),
        //                NombreDirectorCERESO = _centro != null ? !string.IsNullOrEmpty(_centro.DIRECTOR) ? _centro.DIRECTOR.Trim() : string.Empty : string.Empty,
        //                DirectorPenas = NombreAreasTecnicas != null ? NombreAreasTecnicas.USUARIO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA != null ?
        //                    string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
        //                                                !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
        //                                                !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty)
        //                                                 : string.Empty : string.Empty : string.Empty : string.Empty,
        //                Fecha = string.Format("{0} a {1}", string.Format("{0} {1}", _centro.ID_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(_centro.MUNICIPIO.MUNICIPIO1) ? _centro.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty,
        //                                                                          _centro.MUNICIPIO != null ? _centro.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(_centro.MUNICIPIO.ENTIDAD.DESCR) ? _centro.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty),
        //                                                                          Fechas.fechaLetra(Fechas.GetFechaDateServer, false).ToUpper()),
        //                Dictamen = string.Format("DIRECTOR DEL {0}", !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty)
        //            };
        //        };

        //        cEncabezado Encabezado = new cEncabezado();
        //        Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
        //        Encabezado.TituloDos = Parametro.ENCABEZADO2;
        //        Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
        //        Encabezado.NombreReporte = !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty;
        //        Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
        //        Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
        //        Encabezado.PieUno = Parametro.DESCR_ISO_1.Replace(";", ";\n");


        //        #region Inicializacion de reporte
        //        View.Report.LocalReport.ReportPath = "../../Reportes/rDictamenIndividualEstudiosPersonalidad.rdlc";
        //        View.Report.LocalReport.DataSources.Clear();
        //        #endregion

        //        #region Definicion d origenes de datos
        //        var ds1 = new List<cFormatoDictamenEstudiosPersonalidad>();
        //        Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
        //        ds1.Add(DatosReporte);
        //        rds1.Name = "DataSet2";
        //        rds1.Value = ds1;
        //        View.Report.LocalReport.DataSources.Add(rds1);

        //        //datasource dos
        //        var ds2 = new List<cEncabezado>();
        //        ds2.Add(Encabezado);
        //        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
        //        rds2.Name = "DataSet1";
        //        rds2.Value = ds2;
        //        View.Report.LocalReport.DataSources.Add(rds2);

        //        #endregion

        //        View.Report.RefreshReport();
        //    }
        //    catch (Exception exc)
        //    {
        //        throw exc;
        //    }

        //}

        private void MuestraOficioEnvio(ObservableCollection<PERSONALIDAD> _listaEstudios)
        {
            try
            {
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();

                string NombreCentro = string.Empty;
                string NombreMunicipio = string.Empty;
                var Centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                if (Centro != null)
                {
                    NombreCentro = !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty;
                    NombreMunicipio = string.Format("{0} {1}", Centro.ID_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(Centro.MUNICIPIO.MUNICIPIO1) ? Centro.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                        Centro.ID_MUNICIPIO.HasValue ? Centro.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(Centro.MUNICIPIO.ENTIDAD.DESCR) ? Centro.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty);
                };

                string ListaImp = string.Empty;
                short _dummy = 0;
                if (_listaEstudios != null && _listaEstudios.Any())
                    foreach (var item in _listaEstudios)
                    {
                        var _FichaAnterior = new cFichasJuridicas().GetData(x => x.ID_INGRESO == item.ID_INGRESO && x.ID_IMPUTADO == item.ID_IMPUTADO && x.ID_ANIO == item.ID_ANIO && x.ID_CENTRO == item.ID_CENTRO).FirstOrDefault();

                        string _alia = string.Empty;
                        _dummy++;
                        var _aliasImputado = new cAlias().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                        if (_aliasImputado != null && _aliasImputado.Any())
                            foreach (var item2 in _aliasImputado)
                                _alia += string.Format(" y/o {0}", string.Concat(!string.IsNullOrEmpty(item2.NOMBRE) ? item2.NOMBRE.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.PATERNO) ? item2.PATERNO.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.MATERNO) ? item2.MATERNO.Trim() : string.Empty));

                        ListaImp += _dummy + ". " + string.Format("{0} {1} {2} {3} ({4})", item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,//0
                                                        item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,//1
                                                        item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,//2
                                                        !string.IsNullOrEmpty(_alia) ? _alia.Trim() : string.Empty,//3
                                                        _FichaAnterior != null ? !string.IsNullOrEmpty(_FichaAnterior.P2_CLAS_JURID) ? _FichaAnterior.P2_CLAS_JURID.Trim() : string.Empty : string.Empty)//4 
                                                        + "\n";
                    };

                string _NombreAreasTecnicas = string.Empty;
                var NombreAreasTecnicas = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == (short)eDatosRolesProcesosPersonalidad.COORDINACION_TECNICA).FirstOrDefault();
                if (NombreAreasTecnicas != null)
                    _NombreAreasTecnicas = NombreAreasTecnicas.USUARIO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                string NombreJ = string.Empty;
                var NombreJuridico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == (short)eDatosRolesProcesosPersonalidad.JURIDICO).FirstOrDefault();
                if (NombreJuridico != null)
                    NombreJ = NombreJuridico.USUARIO != null ? NombreJuridico.USUARIO.EMPLEADO != null ? NombreJuridico.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;


                string NombreC = string.Empty;
                var NombreComandante = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == (short)eDatosRolesProcesosPersonalidad.COMANDANCIA).FirstOrDefault();
                if (NombreComandante != null)
                    NombreC = NombreComandante.USUARIO != null ? NombreComandante.USUARIO.EMPLEADO != null ? NombreComandante.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreComandante.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreComandante.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreComandante.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreComandante.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreComandante.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreComandante.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;


                string _NombreP = string.Concat("         En seguimiento a la ruta crítica establecida para el <b>", !string.IsNullOrEmpty(NombrePrograma) ? NombrePrograma.ToUpper() : NombrePrograma, "</b> ,remito a usted ", _CantidadFichas, " Fichas de Identificación Jurídica del Fuero Común de los privados de la libertad que se relacionan, los cuales se encuentran actualmente recluidos en este Centro en calidad de sentenciados, a efecto de que se les realice el Estudio de Personalidad correspondiente:");

                var _fecha = _listaEstudios.Any() ? _listaEstudios.FirstOrDefault().SOLICITUD_FEC.HasValue ? _listaEstudios.FirstOrDefault().SOLICITUD_FEC.Value : Fechas.GetFechaDateServer : Fechas.GetFechaDateServer;
                string Fecha = string.Format("{0} A {1} ", NombreMunicipio, Fechas.fechaLetra(_fecha, false).ToUpper());
                NoOficio = _listaEstudios.Any() ? _listaEstudios.FirstOrDefault().NUM_OFICIO : string.Empty;
                #region Inicia definicion de datos para el reporte
                var DatosReporte = new cPeticionRealizacionEstudiosPersonalidad()
                {
                    CCP1 = string.Format("c.c.p.C. {0} .- Comandante del Centro de Reinserción Social.- Para la elaboración de estudios", NombreC),
                    CCP2 = string.Format("c.c.p.C. {0} .- Comandante del Área Femenil.- Mismo fin.", NombreC),
                    Fecha = Fecha,
                    NombreCentro = NombreCentro,
                    NombreCoordinador = _NombreAreasTecnicas,
                    NombreJefeDptoJuridico = NombreJ,
                    NoOficio = string.Format("Se remiten Resúmenes Sociales Oficio número {0}", !string.IsNullOrEmpty(NoOficio) ? NoOficio.ToUpper() : NoOficio),
                    NombrePrograma = _NombreP,
                    ListaImputadosEstudios = ListaImp
                };

                #endregion


                cEncabezado Encabezado = new cEncabezado();
                Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                Encabezado.NombreReporte = NombreCentro;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                Encabezado.PieUno = Parametro.DESCR_ISO_1.Replace(";", ";\n");

                #region Inicializacion de reporte
                View.Report.LocalReport.ReportPath = "Reportes/rPeticionRealizacionEstudiosPersonalidad.rdlc";
                View.Report.LocalReport.DataSources.Clear();
                #endregion

                #region Definicion d origenes de datos
                var ds1 = new List<cPeticionRealizacionEstudiosPersonalidad>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(DatosReporte);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                //datasource dos
                var ds2 = new List<cEncabezado>();
                ds2.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                #endregion

                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void MuestraFormato(Archivero _Documento, PERSONALIDAD _ActualPersonalidad)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    if (_Documento == null || _ActualPersonalidad == null) return;
                    switch (_Documento.TipoArchivo)
                    {
                        case (short)eDocumentoMostrado.PARTIDA_JURIDICA:
                            ImprimirPartidaJuridica(_ActualPersonalidad);
                            break;
                        case (short)eDocumentoMostrado.FICHA_SIGNALETICA:
                            if (_ActualPersonalidad.INGRESO != null)
                                ImprimeFicha(_ActualPersonalidad.INGRESO);
                            break;
                        case (short)eDocumentoMostrado.FICHA_JURIDICA:
                            ImprimeFichaJuridica(_ActualPersonalidad);
                            break;
                        case (short)eDocumentoMostrado.ACTA_CONSEJO_TECNICO:
                            MuestraFormatoActaConsejoTecnicoComun(_ActualPersonalidad);
                            break;
                        case (short)eDocumentoMostrado.DICTAMEN_INDIVIDUAL:
                            MuestraOficioRemisionDictamenEstudioPersonalidad(_ActualPersonalidad);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_MEDICO:
                            if (_ActualPersonalidad.INGRESO != null)
                                MedicoComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSIQ:
                            PsiquiatricoComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_PSICO:
                            PsicologicoComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CRIMI:
                            CriminologicoComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SOCIO_FAM:
                            SocioFamiliarComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_EDUC:
                            EducativoComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_CAPAC:
                            CapacitacionComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_COMUN_SEGURIDAD:
                            SeguridadComun(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.ACTA_FEDERAL:
                            ActaFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_MEDICO:
                            MedicoFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_PSICO:
                            PsicologicoFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_TRABAJO_SOCIAL:
                            SocioFamiliarFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CAPAC:
                            CapacitacionFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_EDUCATIVAS:
                            EducativasFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_VIGILANCIA:
                            VigilanciaFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.PERSONALIDAD_FEDERAL_CRIMINOLOGICO:
                            CriminologicoFederal(_ActualPersonalidad.INGRESO, _ActualPersonalidad.ID_ESTUDIO);
                            break;
                        case (short)eDocumentoMostrado.OFICIO_PETICION_REALIZACION_ESTUDIOS_PERSONALIDAD:
                            ObservableCollection<PERSONALIDAD> lstDummy = new ObservableCollection<PERSONALIDAD>();
                            var CondensadoEstudiosProgramadosGeneral = new cEstudioPersonalidad().GetData(x => x.NUM_OFICIO.Contains(_ActualPersonalidad.NUM_OFICIO) && x.PROG_NOMBRE.Contains(_ActualPersonalidad.PROG_NOMBRE));
                            _CantidadFichas = CondensadoEstudiosProgramadosGeneral.Any() ? CondensadoEstudiosProgramadosGeneral.Count() : 0;
                            NombrePrograma = CondensadoEstudiosProgramadosGeneral.Any() ? CondensadoEstudiosProgramadosGeneral.FirstOrDefault().PROG_NOMBRE : string.Empty;

                            if (CondensadoEstudiosProgramadosGeneral != null && CondensadoEstudiosProgramadosGeneral.Any())
                                foreach (var item in CondensadoEstudiosProgramadosGeneral)
                                    lstDummy.Add(item);

                            MuestraOficioEnvio(lstDummy);
                            break;

                        case (short)eDocumentoMostrado.REMISION_DPMJ:
                            ObservableCollection<PERSONALIDAD> lstEstudios = new ObservableCollection<PERSONALIDAD>();
                            var Condensado = new cEstudioPersonalidad().GetData(x => x.NUM_OFICIO.Trim() == _ActualPersonalidad.NUM_OFICIO && x.PROG_NOMBRE.Trim() == _ActualPersonalidad.PROG_NOMBRE && x.ID_SITUACION != 4);

                            if (Condensado != null && Condensado.Any())
                                foreach (var item in Condensado)
                                    lstEstudios.Add(item);

                            MuestraOficioRemisionDEPMJ(lstEstudios);
                            break;

                        case (short)eDocumentoMostrado.REMISION_CIERRE:
                            MuestraFormatoCierreEstudiosPersonalidadArchivero(_ActualPersonalidad);
                            break;

                        case (short)eDocumentoMostrado.TRASLADO_NACIONAL:
                            ImprimeTrasladoNacional(_ActualPersonalidad);
                            break;

                        case (short)eDocumentoMostrado.TRASLADO_ISLAS:
                            ImprimeTrasladoIslas(_ActualPersonalidad);
                            break;

                        case (short)eDocumentoMostrado.TRASLADO_INTERNACIONAL:
                            ImprimeTrasladoInternacional(_ActualPersonalidad);
                            break;

                        default:
                            break;
                    };
                });
            }
            catch (Exception exc)
            {

                throw;
            }

            return;
        }

        private void ImprimeTrasladoNacional(PERSONALIDAD Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                View.Show();

                var _centro = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                cTrasladoNacionalPersonalidadRep DatosReporte = new cTrasladoNacionalPersonalidadRep();
                var _trasladoHecho = new cTrasladoNacionalPersonalidad().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();
                if (_trasladoHecho != null)
                    DatosReporte = new cTrasladoNacionalPersonalidadRep()
                    {
                        Generico1 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_PSICOLOGICO) ? _trasladoHecho.CONTINUAR_PSICOLOGICO == "S" ? "X" : string.Empty : string.Empty,
                        Generico2 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_PSICOLOGICO) ? _trasladoHecho.CONTINUAR_PSICOLOGICO == "N" ? "X" : string.Empty : string.Empty,
                        Generico3 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_EDUCATIVO) ? _trasladoHecho.CONTINUAR_EDUCATIVO == "S" ? "X" : string.Empty : string.Empty,
                        Generico4 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_EDUCATIVO) ? _trasladoHecho.CONTINUAR_EDUCATIVO == "N" ? "X" : string.Empty : string.Empty,
                        Generico5 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_LABORAL) ? _trasladoHecho.CONTINUAR_LABORAL == "S" ? "X" : string.Empty : string.Empty,
                        Generico6 = !string.IsNullOrEmpty(_trasladoHecho.CONTINUAR_LABORAL) ? _trasladoHecho.CONTINUAR_LABORAL == "N" ? "X" : string.Empty : string.Empty,

                        IndicePeligrosidad = string.Format("MÍNIMA ( {0} )  MÍNIMA MEDIA ( {1} )  MEDIA ( {2} ) SUPERIOR A LA MEDIA ( {3} )  MÁXIMA ( {4} )",
                        _trasladoHecho.ID_PELIGROSIDAD.HasValue ? _trasladoHecho.ID_PELIGROSIDAD.Value == 5 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD.HasValue ? _trasladoHecho.ID_PELIGROSIDAD.Value == 4 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD.HasValue ? _trasladoHecho.ID_PELIGROSIDAD.Value == 3 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD.HasValue ? _trasladoHecho.ID_PELIGROSIDAD.Value == 2 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD.HasValue ? _trasladoHecho.ID_PELIGROSIDAD.Value == 1 ? "X" : string.Empty : string.Empty
                        ),
                        ContinuaTratamientoEduc = _trasladoHecho.CONTINUAR_EDUCATIVO == "S" ? "X" : string.Empty,
                        ContinuaTratamientoLab = _trasladoHecho.CONTINUAR_LABORAL == "S" ? "X" : string.Empty,
                        ContinuaTratamientoPsico = _trasladoHecho.CONTINUAR_PSICOLOGICO == "S" ? "X" : string.Empty,
                        CoordCrimi = _trasladoHecho.COORDINADOR_CRIMINOLOGIA,
                        Elaboro = _trasladoHecho.ELABORO,
                        FechaElaboracion = _trasladoHecho.ESTUDIO_FEC.HasValue ? _trasladoHecho.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                        OtrosAspectosRelevantes = _trasladoHecho.OTROS_ASPECTOS_OPINION,
                        CualesToxicos = _trasladoHecho.CUALES,
                        NombreDirector = string.Format("DIRECTOR DEL CERESO \" {0} \" \n {1}", _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty, _trasladoHecho.DIRECTOR_CERESO),
                        OtrosContinuar = _trasladoHecho.CONTINUAR_OTROS == "S" ? "X" : string.Empty,
                        PresentaAdiccionToxicos = string.Format("SI ( {0} ) \t NO ( {1} )", _trasladoHecho.ADICCION_TOXICOS == "S" ? "X" : string.Empty, _trasladoHecho.ADICCION_TOXICOS == "N" ? "X" : string.Empty),
                        Generico7 = string.Empty,
                        Generico8 = _trasladoHecho.CONTINUAR_OTROS
                    };

                var ds1 = new List<cTrasladoNacionalPersonalidadRep>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(DatosReporte);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                View.Report.LocalReport.ReportPath = "Reportes/rTrasladoNacional.rdlc";
                View.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                View.Report.RefreshReport();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void ImprimeTrasladoIslas(PERSONALIDAD Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                View.Show();

                cTrasladoIslasPerson DatosReporte = new cTrasladoIslasPerson();
                var _trasladoHecho = new cTrasladoIslasPersonalidad().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO).FirstOrDefault();
                if (_trasladoHecho != null)
                    DatosReporte = new cTrasladoIslasPerson()
                    {
                        Crimino = _trasladoHecho.CRIMINOGENESIS,
                        Egocentrismo = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _trasladoHecho.EGOCENTRISMO == 1 ? "X" : string.Empty, _trasladoHecho.EGOCENTRISMO == 2 ? "X" : string.Empty, _trasladoHecho.EGOCENTRISMO == 4 ? "X" : string.Empty),
                        LabAfec = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _trasladoHecho.LABILIDAD_AFECTIVA == 1 ? "X" : string.Empty, _trasladoHecho.LABILIDAD_AFECTIVA == 2 ? "X" : string.Empty, _trasladoHecho.LABILIDAD_AFECTIVA == 4 ? "X" : string.Empty),
                        AdiccionNarco = string.Format("REMISIÓN ( {0} )  EXTINGUIDA ( {1} )  EN ENTORNO CONTROLADA ( {2} )",
                        _trasladoHecho.EN_CASO_ADICCION == "R" ? "X" : string.Empty,
                        _trasladoHecho.EN_CASO_ADICCION == "E" ? "X" : string.Empty,
                        _trasladoHecho.EN_CASO_ADICCION == "C" ? "X" : string.Empty),
                        IntimidacionPena = string.Format("SI ( {0} )   NO ( {1} )", _trasladoHecho.INTIMIDACION_PENA == "S" ? "X" : string.Empty, _trasladoHecho.INTIMIDACION_PENA == "N" ? "X" : string.Empty),
                        AgresividadIntram = string.Format("ALTO ( {0} )  MEDIO ( {1} )  BAJO ( {2} )", _trasladoHecho.AGRESIVIDAD_INTRAMUROS == 1 ? "X" : string.Empty, _trasladoHecho.AGRESIVIDAD_INTRAMUROS == 2 ? "X" : string.Empty, _trasladoHecho.AGRESIVIDAD_INTRAMUROS == 4 ? "X" : string.Empty),
                        IndicePeligrosidadCrimiActual = string.Format("MÍNIMA ( {0} )  MÍNIMA-MEDIA ( {1} )  MEDIA ( {2} )  MEDIA-MÁXIMA ( {3} )  MÁXIMA ( {4} )",
                        _trasladoHecho.ID_PELIGROSIDAD == 5 ? "X" : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD == 4 ? "X" : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD == 3 ? "X" : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD == 2 ? "X" : string.Empty,
                        _trasladoHecho.ID_PELIGROSIDAD == 1 ? "X" : string.Empty),
                        Continue = string.Format(" ( {0} )", _trasladoHecho.CONTINUE_TRATAMIENTO == "S" ? "X" : string.Empty),
                        AnuenciaFirmada = string.Format("SI ( {0} )   NO ( {1} )", _trasladoHecho.ANUENCIA_FIRMADA == "S" ? "X" : string.Empty, _trasladoHecho.ANUENCIA_FIRMADA == "N" ? "X" : string.Empty),
                        DictamenActual = string.Format("FAVORABLE ( {0} )  DESFAVORABLE ( {1} )", _trasladoHecho.DICTAMEN_TRASLADO == "F" ? "X" : string.Empty, _trasladoHecho.DICTAMEN_TRASLADO == "D" ? "X" : string.Empty),
                        LugarFc = _trasladoHecho.ESTUDIO_FEC.HasValue ? string.Format("A {0}", Fechas.fechaLetra(_trasladoHecho.ESTUDIO_FEC.Value, false).ToUpper()) : string.Empty,
                        DirectorCentro = _trasladoHecho.DIRECTOR,
                        Responsable = _trasladoHecho.RESPONSABLE,
                        TratamExtram = _trasladoHecho.TRATAMIENTO_SUGERIDO
                    };


                var ds1 = new List<cTrasladoIslasPerson>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(DatosReporte);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                View.Report.LocalReport.ReportPath = "Reportes/rTrasladoIslas.rdlc";
                View.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                View.Report.RefreshReport();
            }

            catch (Exception)
            {
                throw;
            }
        }

        private void ImprimeTrasladoInternacional(PERSONALIDAD Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                View.Show();

                string _Origi = string.Empty;
                string _resiPadres = string.Empty;
                if (Entity.INGRESO.IMPUTADO != null)
                {
                    var _EstadoNac = new cEntidad().GetData(x => x.ID_ENTIDAD == Entity.INGRESO.IMPUTADO.NACIMIENTO_ESTADO).FirstOrDefault();
                    var _MunicipioNac = new cMunicipio().GetData(x => x.ID_MUNICIPIO == Entity.INGRESO.IMPUTADO.NACIMIENTO_MUNICIPIO && x.ID_ENTIDAD == _EstadoNac.ID_ENTIDAD).FirstOrDefault();
                    _Origi = string.Format("{0}, {1}", _EstadoNac != null ? !string.IsNullOrEmpty(_EstadoNac.DESCR) ? _EstadoNac.DESCR.Trim() : string.Empty : string.Empty, _MunicipioNac != null ? !string.IsNullOrEmpty(_MunicipioNac.MUNICIPIO1) ? _MunicipioNac.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                    if (Entity.INGRESO.IMPUTADO.IMPUTADO_PADRES.Any())
                        foreach (var item in Entity.INGRESO.IMPUTADO.IMPUTADO_PADRES)
                            _resiPadres = string.Format("{0} {1} ", !string.IsNullOrEmpty(item.CALLE) ? item.CALLE.Trim() : string.Empty, item.NUM_EXT.HasValue ? item.NUM_EXT.Value.ToString() : string.Empty);
                };

                System.Collections.Generic.List<cDetallesJuridicosReporte> ListaDelitos = new List<cDetallesJuridicosReporte>();
                System.Collections.Generic.List<cDetallesJuridicosReporte> ListaAntecedentesPenales = new List<cDetallesJuridicosReporte>();

                var _causasPenales = Entity.INGRESO != null ? Entity.INGRESO.CAUSA_PENAL : null;
                if (_causasPenales != null && _causasPenales.Any())
                    foreach (var item in _causasPenales)
                        foreach (var item2 in item.SENTENCIA)
                            foreach (var item3 in item2.SENTENCIA_DELITO)
                                ListaDelitos.Add(new cDetallesJuridicosReporte
                                {
                                    Delito = item3.DESCR_DELITO,
                                    Apartir = item2.FEC_INICIO_COMPURGACION.HasValue ? item2.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty,
                                    FueroC = item3.ID_FUERO == "C" ? "X" : string.Empty,
                                    FueroF = item3.ID_FUERO == "F" ? "X" : string.Empty,
                                    Pena = string.Format("{0} , {1} , {2} ",
                                    item2.ANIOS.HasValue ? item2.ANIOS.Value > 0 ? string.Concat(item2.ANIOS.Value + " AÑOS") : "0 AÑOS" : "0 AÑOS",
                                    item2.MESES.HasValue ? item2.MESES.Value > 0 ? string.Concat(item2.MESES.Value + " MESES") : "0 MESES" : "0 MESES",
                                    item2.DIAS.HasValue ? item2.DIAS.Value > 0 ? string.Concat(item2.DIAS.Value + " DÍAS") : "0 DÍAS" : "0 DÍAS"),
                                    Proceso = string.Format("{0} / {1} ", item.CP_ANIO.HasValue ? item.CP_ANIO.Value.ToString() : string.Empty, item.CP_FOLIO.HasValue ? item.CP_FOLIO.Value.ToString() : string.Empty)
                                });

                var Imputado = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO : null;
                if (Imputado != null)
                    if (Imputado.INGRESO != null && Imputado.INGRESO.Any())
                        foreach (var item in Imputado.INGRESO)
                            foreach (var item2 in item.CAUSA_PENAL)
                                foreach (var item3 in item2.SENTENCIA)
                                    foreach (var item4 in item3.SENTENCIA_DELITO)
                                        ListaAntecedentesPenales.Add(new cDetallesJuridicosReporte
                                        {
                                            Delito = !string.IsNullOrEmpty(item4.DESCR_DELITO) ? item4.DESCR_DELITO.Trim() : string.Empty,
                                            Apartir = item4.SENTENCIA != null ? item4.SENTENCIA.FEC_INICIO_COMPURGACION.HasValue ? item4.SENTENCIA.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty,
                                            FueroC = item4.ID_FUERO == "C" ? "X" : string.Empty,
                                            FueroF = item4.ID_FUERO == "F" ? "X" : string.Empty,
                                            Pena = string.Format("{0} , {1} , {2} ",
                                                                    item3.ANIOS.HasValue ? item3.ANIOS.Value > 0 ? string.Concat(item3.ANIOS.Value + " AÑOS") : "0 AÑOS" : "0 AÑOS",
                                                                    item3.MESES.HasValue ? item3.MESES.Value > 0 ? string.Concat(item3.MESES.Value + " MESES") : "0 MESES" : "0 MESES",
                                                                    item3.DIAS.HasValue ? item3.DIAS.Value > 0 ? string.Concat(item3.DIAS.Value + " DÍAS") : "0 DÍAS" : "0 DÍAS"),
                                            Proceso = string.Format("{0} / {1} ", item2.CP_ANIO.HasValue ? item2.CP_ANIO.Value.ToString() : string.Empty, item2.CP_FOLIO.HasValue ? item2.CP_FOLIO.Value.ToString() : string.Empty)
                                        });

                string Antece = "N";
                if (LstDelitosDos != null && LstDelitosDos.Any())
                    Antece = "S";

                var _Centro = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                cTrasladoInterPersonalidad DatosReporte = new cTrasladoInterPersonalidad();
                List<cTrasladoIntSancionReporte> lstSanciones = new List<cTrasladoIntSancionReporte>();
                List<cTrasladoIntVisitaReporte> lstVisitas = new List<cTrasladoIntVisitaReporte>();

                var _trasladoHecho = new cTrasladoInternacionalPersonalidad().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_trasladoHecho != null)
                {
                    DatosReporte = new cTrasladoInterPersonalidad()
                    {
                        CentroOrigen = _Centro != null ? !string.IsNullOrEmpty(_Centro.DESCR) ? _Centro.DESCR.Trim() : string.Empty : string.Empty,
                        CentroD = string.Empty,
                        NombreImp = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.NOMBRE) ? Entity.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                         !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.PATERNO) ? Entity.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                         !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.MATERNO) ? Entity.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty,
                        EdadImp = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO != null ? new Fechas().CalculaEdad(Entity.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty : string.Empty,
                        //EdoCivilImp = Entity.INGRESO != null ? Entity.INGRESO.IMPUTADO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(Entity.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR) ? Entity.INGRESO.IMPUTADO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                        EdoCivilImp = Entity.INGRESO != null ? Entity.INGRESO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(Entity.INGRESO.ESTADO_CIVIL.DESCR) ? Entity.INGRESO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                        OriginaImp = _Origi,
                        LugarResid = _resiPadres,
                        EscolaridadImp = _trasladoHecho.ESCOLARIDAD,
                        OcupacionPreviaImp = _trasladoHecho.OCUPACION_PREVIA,
                        AntecPenales = string.Format("NO ( {0} ) \t SI ( {1} )", string.Empty, "X"),
                        VersionDel = _trasladoHecho.VERSION_DELITO,
                        ClinicSano = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.CLINICAMENTE_SANO == "S" ? "X" : string.Empty, _trasladoHecho.CLINICAMENTE_SANO == "N" ? "X" : string.Empty),
                        CasoNegEsp = _trasladoHecho.PADECIMIENTO,
                        TratamActual = _trasladoHecho.TRATAMIENTO_ACTUAL,
                        CoeficienteInt = _trasladoHecho.COEFICIENTE_INTELECTUAL,
                        DanioOrganoCere = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.DANIO_CEREBRAL == "S" ? "X" : string.Empty, _trasladoHecho.DANIO_CEREBRAL == "N" ? "X" : string.Empty),
                        OtrosAspectos = _trasladoHecho.OTROS_ASPECTOS_OPINION,
                        ApoyoPadres = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.APOYO_PADRES == "S" ? "X" : string.Empty, _trasladoHecho.APOYO_PADRES == "N" ? "X" : string.Empty),
                        Conyuge = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.CONYUGE == "S" ? "X" : string.Empty, _trasladoHecho.CONYUGE == "N" ? "X" : string.Empty),
                        Frecuenciavisitas = _trasladoHecho.FRECUENCIA_VISITAS,
                        NoRecibVisitas = _trasladoHecho.CAUSA_NO_VISITAS,
                        CartaArraigoDom = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.CARTA_ARRAIGO == "S" ? "X" : string.Empty, _trasladoHecho.CARTA_ARRAIGO == "N" ? "X" : string.Empty),
                        Domic = _trasladoHecho.DOMICILIO,
                        AnuenciaCupo = string.Format("SE DESCONOCE ( {0} )   SI( {1} )   NO( {2} )",
                        _trasladoHecho.ANUENCIA_CUPO == "D" ? "X" : string.Empty,
                        _trasladoHecho.ANUENCIA_CUPO == "S" ? "X" : string.Empty,
                        _trasladoHecho.ANUENCIA_CUPO == "N" ? "X" : string.Empty),
                        Fecha1 = _trasladoHecho.ANUENCIA_FEC.HasValue ? _trasladoHecho.ANUENCIA_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                        NivelSocioE = _trasladoHecho.NIVEL_SOCIOECONOMICO,
                        EstudiaActu = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.ESTUDIA_ACTUALMENTE == "S" ? "X" : string.Empty, _trasladoHecho.ESTUDIA_ACTUALMENTE == "N" ? "X" : string.Empty),
                        CasoNoEstudia = _trasladoHecho.CAUSA_NO_ESTUDIA,
                        TrabajaInst = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.TRABAJA_ACTUALMENTE == "S" ? "X" : string.Empty, _trasladoHecho.TRABAJA_ACTUALMENTE == "N" ? "X" : string.Empty),
                        Ocupaciones = _trasladoHecho.OCUPACION_ACTUAL,
                        NegativoTrabaja = _trasladoHecho.CAUSA_NO_TRABAJA,
                        DiasEfectLab = _trasladoHecho.DIAS_EFECTIVOS_TRABAJO.HasValue ? _trasladoHecho.DIAS_EFECTIVOS_TRABAJO.Value.ToString() : string.Empty,
                        ConductaDentroReclusion = string.Format("MÍNIMA ( {0} )  MÍNIMA MEDIA ( {1} )  MEDIA ( {2} )  SUPERIOR A LA MEDIA ( {3} )  MÁXIMA ( {4} ) ",
                        _trasladoHecho.CONDUCTA_RECLUSION.HasValue ? _trasladoHecho.CONDUCTA_RECLUSION == 5 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.CONDUCTA_RECLUSION.HasValue ? _trasladoHecho.CONDUCTA_RECLUSION == 4 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.CONDUCTA_RECLUSION.HasValue ? _trasladoHecho.CONDUCTA_RECLUSION == 3 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.CONDUCTA_RECLUSION.HasValue ? _trasladoHecho.CONDUCTA_RECLUSION == 2 ? "X" : string.Empty : string.Empty,
                        _trasladoHecho.CONDUCTA_RECLUSION.HasValue ? _trasladoHecho.CONDUCTA_RECLUSION == 1 ? "X" : string.Empty : string.Empty),
                        EsAdictoToxicos = string.Format("SI ( {0} ) \t  NO ( {1} )", _trasladoHecho.ADICCION_TOXICOS == "S" ? "X" : string.Empty, _trasladoHecho.ADICCION_TOXICOS == "N" ? "X" : string.Empty),
                        AdictoEspecifica = _trasladoHecho.ADICCION_CUALES,
                        ContinuaTratamPsico = _trasladoHecho.CONTINUAR_PSICOLOGICO == "S" ? "X" : string.Empty,
                        ContinuaTratamEduc = _trasladoHecho.CONTINUAR_EDUCATIVO == "S" ? "X" : string.Empty,
                        ContinuaTratamLaboral = _trasladoHecho.CONTINUAR_LABORAL == "S" ? "X" : string.Empty,
                        OtrosAspectosP = _trasladoHecho.OTROS_ASPECTOS,
                        NombreDirector = _trasladoHecho.DIRECTOR,
                        Responsable = _trasladoHecho.RESPONSABLE,
                        FecElaboracion = _trasladoHecho.ESTUDIO_FEC,
                        Generico = _trasladoHecho.AGRESIVIDAD,
                        Generico2 = _trasladoHecho.CONTINUAR_PSICOLOGICO == "N" ? "X" : string.Empty,
                        Generico3 = _trasladoHecho.CONTINUAR_EDUCATIVO == "N" ? "X" : string.Empty,
                        Generico4 = _trasladoHecho.CONTINUAR_LABORAL == "N" ? "X" : string.Empty,
                        Generico5 = string.Format("SI ({0})   NO ({1})", Antece == "S" ? "X" : string.Empty, Antece == "N" ? "X" : string.Empty),
                        Generico6 = _Centro != null ? !string.IsNullOrEmpty(_Centro.DESCR) ? _Centro.DESCR.Trim() : string.Empty : string.Empty,
                        Cursos = _trasladoHecho.OTROS_CURSOS,
                        Desconoc = _trasladoHecho.CONTINUAR_OTRO
                    };

                    if (_trasladoHecho.TRASLADO_INTERNACIONAL_SANCION != null && _trasladoHecho.TRASLADO_INTERNACIONAL_SANCION.Any())
                        foreach (var item in _trasladoHecho.TRASLADO_INTERNACIONAL_SANCION)
                        {
                            lstSanciones.Add(new cTrasladoIntSancionReporte()
                            {
                                Motivo = item.MOTIVO,
                                Sancion = item.SANCION,
                                SancionFecha = item.SANCION_FEC.HasValue ? item.SANCION_FEC.Value.ToString("dd/MM/yyyy") : string.Empty
                            });
                        };

                    if (_trasladoHecho.TRASLADO_INTERNACIONAL_VISITA != null && _trasladoHecho.TRASLADO_INTERNACIONAL_VISITA.Any())
                        foreach (var item in _trasladoHecho.TRASLADO_INTERNACIONAL_VISITA)
                        {
                            lstVisitas.Add(new cTrasladoIntVisitaReporte
                            {
                                Materno = item.MATERNO,
                                Nombre = item.NOMBRE,
                                Parentesco = item.ID_TIPO_REFERENCIA.HasValue ? !string.IsNullOrEmpty(item.TIPO_REFERENCIA.DESCR) ? item.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                Paterno = item.PATERNO
                            });
                        };
                };


                cEncabezado Encabezado = new cEncabezado();
                Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.NombreReporte = !string.IsNullOrEmpty(_Centro.DESCR) ? _Centro.DESCR.Trim() : string.Empty;
                Encabezado.ImagenDerecha = Parametro.LOGO_BC_ACTA_COMUN;

                var ds1 = new List<cEncabezado>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds1.Add(Encabezado);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);


                var ds2 = new List<cTrasladoInterPersonalidad>();
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds2.Add(DatosReporte);
                rds2.Name = "DataSet2";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                var ds3 = new List<cTrasladoIntSancionReporte>();
                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds3 = lstSanciones;
                rds3.Name = "DataSet3";
                rds3.Value = ds3;
                View.Report.LocalReport.DataSources.Add(rds3);

                var ds4 = new List<cTrasladoIntVisitaReporte>();
                Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                ds4 = lstVisitas;
                rds4.Name = "DataSet4";
                rds4.Value = ds4;
                View.Report.LocalReport.DataSources.Add(rds4);


                Microsoft.Reporting.WinForms.ReportDataSource rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = ListaDelitos;
                View.Report.LocalReport.DataSources.Add(rds5);


                Microsoft.Reporting.WinForms.ReportDataSource rds6 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds6.Name = "DataSet6";
                rds6.Value = ListaAntecedentesPenales;
                View.Report.LocalReport.DataSources.Add(rds6);


                View.Report.LocalReport.ReportPath = "Reportes/rTrasladoInternacional.rdlc";
                View.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                View.Report.RefreshReport();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void MuestraFormatoCierreEstudiosPersonalidadArchivero(PERSONALIDAD Entity)
        {
            try
            {
                if (Entity != null)
                {
                    var View = new ReportesView();
                    #region Iniciliza el entorno para mostrar el reporte al usuario
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    View.Owner = PopUpsViewModels.MainWindow;
                    View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    //View.Show();

                    #endregion

                    string NombreCentro = string.Empty;
                    string NombreMunicipio = string.Empty;
                    var Centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (Centro != null)
                    {
                        NombreCentro = !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty;
                        NombreMunicipio = string.Format("{0} {1}", Centro.ID_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(Centro.MUNICIPIO.MUNICIPIO1) ? Centro.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                            Centro.ID_MUNICIPIO.HasValue ? Centro.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(Centro.MUNICIPIO.ENTIDAD.DESCR) ? Centro.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty);
                    };

                    string ListaFavorable = string.Empty;
                    string ListaAplazados = string.Empty;
                    short _dummy = 0;
                    short _dummy2 = 0;
                    IQueryable<PERSONALIDAD> lstBrig;
                    string _alia = string.Empty;
                    var detalleByMuestra = new cEstudioPersonalidad().GetData(x => x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                    if (detalleByMuestra != null)
                    {
                        string NombreBrigada = string.Empty;
                        string NombreFolio = string.Empty;

                        NombreBrigada = detalleByMuestra.PROG_NOMBRE;
                        NombreFolio = detalleByMuestra.NUM_OFICIO;

                        lstBrig = new cEstudioPersonalidad().ObtenerDatosBrigada(NombreBrigada, NombreFolio);
                        foreach (var item in lstBrig)
                        {
                            _alia = string.Empty;
                            if (item.RESULT_ESTUDIO == "S")
                            {
                                _dummy++;
                                var _aliasImputado = new cApodo().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                                if (_aliasImputado != null && _aliasImputado.Any())
                                    foreach (var item2 in _aliasImputado)
                                        _alia += string.Format(" (a) {0}", !string.IsNullOrEmpty(item2.APODO1) ? item2.APODO1.Trim() : string.Empty);

                                ListaFavorable += _dummy + ". " + string.Format("{0} {1} {2} {3}", item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                                !string.IsNullOrEmpty(_alia) ? _alia.Trim() : string.Empty) + "\n";
                            }
                            else
                            {
                                _dummy2++;
                                var _aliasImputado = new cApodo().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                                if (_aliasImputado != null && _aliasImputado.Any())
                                    foreach (var item2 in _aliasImputado)
                                        _alia += string.Format(" (a) {0}", !string.IsNullOrEmpty(item2.APODO1) ? item2.APODO1.Trim() : string.Empty);

                                ListaAplazados += _dummy2 + ". " + string.Format("{0} {1} {2} {3}", item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                                !string.IsNullOrEmpty(_alia) ? _alia.Trim() : string.Empty) + "\n";
                            };
                        };

                        string NombreJ = string.Empty;
                        var NombreJuridico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 30).FirstOrDefault();
                        if (NombreJuridico != null)
                            NombreJ = NombreJuridico.USUARIO != null ? NombreJuridico.USUARIO.EMPLEADO != null ? NombreJuridico.USUARIO.EMPLEADO.PERSONA != null ?
                                string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                        string _NombreAreasTecnicas = string.Empty;
                        var NombreAreasTecnicas = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 1).FirstOrDefault();
                        if (NombreAreasTecnicas != null)
                            _NombreAreasTecnicas = NombreAreasTecnicas.USUARIO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA != null ?
                                string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;


                        string CuerpoDescripcion = string.Concat("  En atención al oficio " + Entity.NUM_OFICIO + " de fecha " + (Entity.SOLICITUD_FEC.HasValue ? Fechas.fechaLetra(Entity.SOLICITUD_FEC.Value, false) : string.Empty) + " remito a usted " + lstBrig.Count() + " estudios de personalidad del fuero común con su respectivo dictamen, para el trámite de beneficio que corresponda, los cuales comprenden de las semanas ");
                        string Fecha = string.Format("{0} a {1} ", NombreMunicipio, Fechas.fechaLetra(Fechas.GetFechaDateServer, false));
                        var _dato = new cFormatoRemisionEstudiosPersonalidad()
                        {
                            Descripcion = CuerpoDescripcion,
                            Fecha = Fecha,
                            InternosAplazados = ListaAplazados,
                            InternosFavorables = ListaFavorable,
                            NombreCereso = NombreCentro,
                            NombreEntidad = NombreMunicipio,
                            NombreJefeAreasTecnicas = _NombreAreasTecnicas,
                            NombreJefeJuridico = NombreJ,
                            Generico1 = detalleByMuestra != null ? detalleByMuestra.NUM_OFICIO1 : string.Empty
                        };

                        cEncabezado Encabezado = new cEncabezado();
                        Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                        Encabezado.TituloDos = Parametro.ENCABEZADO2;
                        Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                        Encabezado.NombreReporte = NombreCentro;
                        Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                        Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                        Encabezado.PieUno = Parametro.DESCR_ISO_1.Replace(";", ";\n");

                        #region Inicializacion de reporte
                        View.Report.LocalReport.ReportPath = "Reportes/rFormatoRemisionEstudiosPersonalidad.rdlc";
                        View.Report.LocalReport.DataSources.Clear();
                        #endregion


                        #region Definicion d origenes de datos

                        var ds2 = new List<cEncabezado>();
                        ds2.Add(Encabezado);
                        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds2.Name = "DataSet1";
                        rds2.Value = ds2;
                        View.Report.LocalReport.DataSources.Add(rds2);

                        var ds1 = new List<cFormatoRemisionEstudiosPersonalidad>();
                        Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        ds1.Add(_dato);
                        rds1.Name = "DataSet2";
                        rds1.Value = ds1;
                        View.Report.LocalReport.DataSources.Add(rds1);

                        #endregion
                        View.Report.RefreshReport();
                        byte[] renderedBytes;

                        Microsoft.Reporting.WinForms.Warning[] warnings;
                        string[] streamids;
                        string mimeType;
                        string encoding;
                        string extension;

                        renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                        var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                        System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                        renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                        var tc = new TextControlView();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                                tc.editor.IsSpellCheckingEnabled = true;
                                tc.editor.TextFrameMarkerLines = false;
                                tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                                tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                            }
                        };

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
                    };
                };
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private void MuestraOficioRemisionDEPMJ(ObservableCollection<PERSONALIDAD> _lstEstudios)
        {
            try
            {
                if (_lstEstudios == null)
                    return;

                if (_lstEstudios.Any())
                {
                    var View = new ReportesView();
                    #region Iniciliza el entorno para mostrar el reporte al usuario
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    View.Owner = PopUpsViewModels.MainWindow;
                    View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    //View.Show();
                    #endregion

                    var _muestra = _lstEstudios.FirstOrDefault();
                    string NombreCentro = string.Empty;
                    string EntitdadActual = string.Empty;
                    var Centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (Centro != null)
                        NombreCentro = !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty;

                    var _entidad = Centro.ID_ENTIDAD.HasValue ? Centro.ID_MUNICIPIO.HasValue ? new cMunicipio().GetData().FirstOrDefault(x => x.ID_ENTIDAD == Centro.ID_ENTIDAD && x.ID_MUNICIPIO == Centro.ID_MUNICIPIO) : null : null;

                    var NombreEjecucion = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 20).FirstOrDefault();

                    string lstAprobados = string.Empty;
                    string lstAprobadosMayoria = string.Empty;
                    short _cont = 0;
                    short _cont1 = 0;

                    foreach (var item in _lstEstudios)
                    {
                        if (item.ACTA_CONSEJO_TECNICO == null)
                            continue;

                        string _alia = string.Empty;
                        if (item.ACTA_CONSEJO_TECNICO.OPINION == "S")
                        {
                            _cont++;
                            var _aliasImputado = new cAlias().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                            if (_aliasImputado != null && _aliasImputado.Any())
                                foreach (var item2 in _aliasImputado)
                                    _alia += string.Format(" y/o {0}", string.Concat(!string.IsNullOrEmpty(item2.NOMBRE) ? item2.NOMBRE.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.PATERNO) ? item2.PATERNO.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.MATERNO) ? item2.MATERNO.Trim() : string.Empty));

                            lstAprobados += string.Format("{0}. {1} {2} \n", _cont,
                                string.Format("{0} {1} {2}",
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty),
                                _alia
                                );
                        }

                        if (item.ACTA_CONSEJO_TECNICO.OPINION == "N")
                        {
                            _cont1++;
                            var _aliasImputado = new cAlias().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                            if (_aliasImputado != null && _aliasImputado.Any())
                                foreach (var item2 in _aliasImputado)
                                    _alia += string.Format(" y/o {0}", string.Concat(!string.IsNullOrEmpty(item2.NOMBRE) ? item2.NOMBRE.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.PATERNO) ? item2.PATERNO.Trim() : string.Empty, " ", !string.IsNullOrEmpty(item2.MATERNO) ? item2.MATERNO.Trim() : string.Empty));

                            lstAprobadosMayoria += string.Format("{0}. {1} {2}  \n", _cont1,
                                string.Format("{0} {1} {2}",
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty),
                                _alia
                                );
                        }
                    }


                    cFormatoRemisionDPMJ DatosReporte = new cFormatoRemisionDPMJ()
                    {
                        NoOficio = string.Format("OFICIO: {0}", _muestra.NUM_OFICIO1),
                        NombreCentro = Centro != null ? !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty : string.Empty,
                        Fecha = string.Format("{0} a {1} ", _entidad != null ? !string.IsNullOrEmpty(_entidad.MUNICIPIO1) ? _entidad.MUNICIPIO1.Trim() : string.Empty : string.Empty, Fechas.fechaLetra(Fechas.GetFechaDateServer, false)),
                        NombreDirector = NombreEjecucion != null ? NombreEjecucion.USUARIO != null ? NombreEjecucion.USUARIO.EMPLEADO != null ? NombreEjecucion.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreEjecucion.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreEjecucion.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreEjecucion.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreEjecucion.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreEjecucion.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreEjecucion.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty,
                        NombrePrograma = _muestra != null ? !string.IsNullOrEmpty(_muestra.PROG_NOMBRE) ? _muestra.PROG_NOMBRE.Trim() : string.Empty : string.Empty,
                        CantidadEstudios = _lstEstudios.Count.ToString(),
                        NombreAprobados = lstAprobados,
                        NombreAprobadosMayoria = lstAprobadosMayoria,
                        NombreDirectorCereso = Centro != null ? !string.IsNullOrEmpty(Centro.DIRECTOR) ? Centro.DIRECTOR.Trim() : string.Empty : string.Empty
                    };

                    cEncabezado Encabezado = new cEncabezado();
                    Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA DEL ESTADO DE BAJA CALIFORNIA";
                    Encabezado.TituloDos = Parametro.ENCABEZADO2;
                    Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                    Encabezado.NombreReporte = !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty;
                    Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                    Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                    Encabezado.PieUno = Parametro.DESCR_ISO_1.Replace(";", ";\n");


                    #region Inicializacion de reporte
                    View.Report.LocalReport.ReportPath = "Reportes/rFormatoRemisionEstudiosDPMJ.rdlc";
                    View.Report.LocalReport.DataSources.Clear();
                    #endregion

                    #region Definicion d origenes de datos
                    var ds1 = new List<cFormatoRemisionDPMJ>();
                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    ds1.Add(DatosReporte);
                    rds1.Name = "DataSet2";
                    rds1.Value = ds1;
                    View.Report.LocalReport.DataSources.Add(rds1);

                    //datasource dos
                    var ds2 = new List<cEncabezado>();
                    ds2.Add(Encabezado);
                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet1";
                    rds2.Value = ds2;
                    View.Report.LocalReport.DataSources.Add(rds2);

                    #endregion

                    View.Report.RefreshReport();
                    byte[] renderedBytes;

                    Microsoft.Reporting.WinForms.Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;

                    renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                    System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                    renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                    var tc = new TextControlView();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.editor.Loaded += (s, e) =>
                    {
                        try
                        {
                            tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                            tc.editor.IsSpellCheckingEnabled = true;
                            tc.editor.TextFrameMarkerLines = false;
                            tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                            tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                        }
                    };

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Owner = PopUpsViewModels.MainWindow;
                    tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Show();

                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private string CalcularSentenciaString()
        {
            try
            {
                LstSentenciasIngresos = new List<SentenciaIngreso>();
                if (IngresoSeleccionado != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (IngresoSeleccionado.CAUSA_PENAL != null)
                    {
                        foreach (var cp in IngresoSeleccionado.CAUSA_PENAL)
                        {
                            bool segundaInstancia = false, Incidente = false;
                            if (cp.SENTENCIA != null)
                            {
                                if (cp.SENTENCIA.Count > 0)
                                {

                                    #region Incidente
                                    if (cp.AMPARO_INCIDENTE != null)
                                    {
                                        var i = cp.AMPARO_INCIDENTE.Where(w => w.MODIFICA_PENA_ANIO != null && w.MODIFICA_PENA_MES != null && w.MODIFICA_PENA_DIA != null);
                                        if (i != null)
                                        {
                                            var res = i.OrderByDescending(w => w.ID_AMPARO_INCIDENTE).FirstOrDefault();// SingleOrDefault();
                                            if (res != null)
                                            {

                                                anios = anios + (res.MODIFICA_PENA_ANIO != null ? res.MODIFICA_PENA_ANIO.Value : 0);
                                                meses = meses + (res.MODIFICA_PENA_MES != null ? res.MODIFICA_PENA_MES.Value : 0);
                                                dias = dias + (res.MODIFICA_PENA_DIA != null ? res.MODIFICA_PENA_DIA.Value : 0);

                                                LstSentenciasIngresos.Add(
                                                new SentenciaIngreso()
                                                {
                                                    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                    Fuero = cp.CP_FUERO,
                                                    SentenciaAnios = res.MODIFICA_PENA_ANIO != null ? res.MODIFICA_PENA_ANIO : 0,
                                                    SentenciaMeses = res.MODIFICA_PENA_MES != null ? res.MODIFICA_PENA_MES : 0,
                                                    SentenciaDias = res.MODIFICA_PENA_DIA != null ? res.MODIFICA_PENA_DIA : 0,
                                                    Instancia = "INCIDENCIA",
                                                    Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
                                                });
                                                Incidente = true;
                                            }
                                        }

                                        //ABONOS
                                        var dr = cp.AMPARO_INCIDENTE.Where(w => w.DIAS_REMISION != null);
                                        if (i != null)
                                        {
                                            foreach (var x in dr)
                                            {
                                                //ABONO
                                                dias_abono = dias_abono + (x.DIAS_REMISION != null ? (int)x.DIAS_REMISION : 0);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region BUSCAMOS SI TIENE 2DA INSTANCIA
                                    if (cp.RECURSO.Count > 0)
                                    {
                                        var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
                                        if (r != null && Incidente == false)
                                        {
                                            var res = r.OrderByDescending(w => w.ID_RECURSO).FirstOrDefault();
                                            if (res != null)
                                            {
                                                //SENTENCIA
                                                anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
                                                meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
                                                dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);

                                                LstSentenciasIngresos.Add(
                                                new SentenciaIngreso()
                                                {
                                                    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                    Fuero = cp.CP_FUERO,
                                                    SentenciaAnios = res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS : 0,
                                                    SentenciaMeses = res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES : 0,
                                                    SentenciaDias = res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS : 0,
                                                    Instancia = "SEGUNDA INSTANCIA",
                                                    Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
                                                });
                                                segundaInstancia = true;
                                            }
                                        }
                                    }
                                    #endregion

                                    var s = cp.SENTENCIA.Where(w => w.ESTATUS == "A").FirstOrDefault();
                                    if (s != null)
                                    {
                                        if (FechaInicioCompurgacion == null)
                                            FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        else
                                            if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;

                                        #region Delito
                                        if (s.SENTENCIA_DELITO != null)
                                        {
                                            foreach (var del in s.SENTENCIA_DELITO)
                                            {
                                                if (!string.IsNullOrEmpty(Delitos))
                                                    Delitos = string.Format("{0},", Delitos);
                                                Delitos = string.Format("{0}{1}", Delitos, del.DESCR_DELITO);
                                            }

                                        }
                                        #endregion

                                        //SENTENCIA
                                        if (!segundaInstancia && !Incidente)
                                        {
                                            anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
                                            meses = meses + (s.MESES != null ? s.MESES.Value : 0);
                                            dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);

                                            LstSentenciasIngresos.Add(
                                            new SentenciaIngreso()
                                            {
                                                CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                                Fuero = cp.CP_FUERO,
                                                SentenciaAnios = s.ANIOS != null ? s.ANIOS : 0,
                                                SentenciaMeses = s.MESES != null ? s.MESES : 0,
                                                SentenciaDias = s.DIAS != null ? s.DIAS : 0,
                                                Instancia = "PRIMERA INSTANCIA",
                                                Estatus = cp.CAUSA_PENAL_ESTATUS.DESCR
                                            });
                                        }

                                        //ABONO
                                        anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
                                        meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
                                        dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
                                    }
                                }
                                else
                                {
                                    LstSentenciasIngresos.Add(
                                    new SentenciaIngreso()
                                    {
                                        CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                        Fuero = cp.CP_FUERO,
                                        SentenciaAnios = null,
                                        SentenciaMeses = null,
                                        SentenciaDias = null
                                    });
                                }
                            }
                            else
                            {
                                LstSentenciasIngresos.Add(
                                new SentenciaIngreso()
                                {
                                    CausaPenal = string.IsNullOrEmpty(cp.CP_FORANEO) ? string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO) : cp.CP_FORANEO,
                                    Fuero = cp.CP_FUERO,
                                    SentenciaAnios = null,
                                    SentenciaMeses = null,
                                    SentenciaDias = null
                                });
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
                        FechaFinCompurgacion = FechaInicioCompurgacion.Value.Date;
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
                    };

                    return anios + (anios == 1 ? " AÑO " : " AÑOS ") + meses + (meses == 1 ? " MES " : " MESES ") + dias + (dias == 1 ? " DÍA  " : " DÍAS ");
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }

            return string.Empty;
        }


        //private void ImprimeFicha(INGRESO _dato)
        //{
        //    try
        //    {
        //        if (_dato != null)
        //        {
        //            var vw = new ReporteView(_dato);
        //            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
        //            vw.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
        //            vw.Owner = PopUpsViewModels.MainWindow;
        //            vw.Show();
        //        }
        //        else
        //        {
        //            new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar un ingreso.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar datos para imprimir la ficha de identificacion.", ex);
        //    }
        //}
        #endregion

        #region Media Filiacion
        private void PopulateMediaFiliacion()
        {
            try
            {
                //string[] valores = new string[] { RaizNariz, DorsoNariz, AnchoNariz };

                var mf = IngresoSeleccionado.IMPUTADO.IMPUTADO_FILIACION;
                if (mf != null && mf.Any())
                {
                    foreach (var f in mf)
                    {
                        switch (f.ID_MEDIA_FILIACION)
                        {
                            case (short)enumMediaFilicacion.NARIZ_RAIZ:
                                RaizNariz = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.NARIZ_DORSO:
                                DorsoNariz = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.NARIZ_ANCHO:
                                AnchoNariz = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.NARIZ_BASE:
                                BaseNariz = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.NARIZ_ALTURA:
                                AlturaNariz = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.CARA:
                                CaraSenias = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.CABELLO_CANTIDAD:
                                CantidadCabello = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.CABELLO_COLOR:
                                ColorCabello = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.CABELLO_CALVICIE:
                                CalvicieCabello = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.CABELLO_FORMA:
                                FormaCabello = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.CEJAS_DIRECCION:
                                DireccionCejas = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.CEJAS_IMPLANTACION:
                                ImplantacionCejas = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.CEJAS_FORMA:
                                FormaCejas = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.CEJAS_TAMANO:
                                TamanioCejas = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OJOS_COLOR:
                                ColorOjos = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OJOS_FORMA:
                                FormaOjos = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OJOS_TAMANO:
                                TamanioOjos = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.BOCA_TAMANO:
                                TamanioBoca = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.BOCA_COMISURAS:
                                ComisurasBoca = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.LABIOS_ESPESOR:
                                EspesorLabios = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.SANGRE_TIPO:
                                TipoSangre = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.SANGRE_FACTOR_RH:
                                FactorSangre = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.MENTON_TIPO:
                                TipoMenton = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.MENTON_FORMA:
                                FormaMenton = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.MENTON_INCLINACION:
                                InclinacionMenton = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.FRENTE_ALTURA:
                                AlturaFrente = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.FRENTE_INCLINACION:
                                InclinacionFrente = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.FRENTE_ANCHO:
                                AnchoFrente = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.COLOR_PIEL:
                                ColorPielSenias = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.CABELLO_IMPLANTACION:
                                ImplantacionCabello = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.LABIOS_ALTURA_NASO_LABIAL:
                                AlturaLabios = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.LABIOS_PROMINENCIA:
                                ProminenciaLabios = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OREJA_BORDE_FORMA:
                                FormaOreja = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.COMPLEXION:
                                ComplexionSenias = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OREJA_HELIX_ORIGINAL:
                                OriginalHelix = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OREJA_HELIX_SUPERIOR:
                                SuperiorHelix = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OREJA_HELIX_POSTERIOR:
                                PosteriorHelix = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OREJA_HELIX_ADHERENCIA:
                                AdherenciaHelix = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OREJA_LOBULO_CONTORNO:
                                ContornoLobulo = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OREJA_LOBULO_ADHERENCIA:
                                AdherenciaLobulo = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OREJA_LOBULO_PARTICULARIDAD:
                                ParticularidadLobulo = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                            case (short)enumMediaFilicacion.OREJA_LOBULO_DIMENSION:
                                DimensionLobulo = f.TIPO_FILIACION != null ? !string.IsNullOrEmpty(f.TIPO_FILIACION.DESCR) ? f.TIPO_FILIACION.DESCR.Trim() : string.Empty : string.Empty;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }
        #endregion

        #region Datos Medicos

        private void GetDatosMedicosFormatos()
        {
            try
            {
                ImprimeHCM();
                ImprimeHistoriaClinicaDental();
                //ImprimeHojaEnfermeria();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void ImprimeHCM()
        {
            try
            {
                ReporteHCM.Reset();

                ListArchivosHCM = new ObservableCollection<CustomArchivosSimple>();
                if (SelectedIngreso != null)
                {
                    var detalleHistoria = new cHistoriaClinica().GetData(x => x.ID_INGRESO == SelectedIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectedIngreso.ID_IMPUTADO && x.ID_ANIO == SelectedIngreso.ID_ANIO).FirstOrDefault();

                    TieneHistoriaClinicaMedica = detalleHistoria != null;
                    if (detalleHistoria != null)
                    {

                        VisibleReporteHistoriaClinicaMedica = Visibility.Visible;
                        if (detalleHistoria.HISTORIA_CLINICA_DOCUMENTO != null && detalleHistoria.HISTORIA_CLINICA_DOCUMENTO.Any())
                            foreach (var item in detalleHistoria.HISTORIA_CLINICA_DOCUMENTO)
                            {
                                ListArchivosHCM.Add(new CustomArchivosSimple()
                                {
                                    Consecutivo = item.ID_NODOCTO,
                                    Formato = item.ID_FORMATO.HasValue ? !string.IsNullOrEmpty(item.FORMATO_DOCUMENTO.DESCR) ? item.FORMATO_DOCUMENTO.DESCR.Trim() : string.Empty : string.Empty,
                                    TipoDocumento = item.HC_DOCUMENTO_TIPO != null ? !string.IsNullOrEmpty(item.HC_DOCUMENTO_TIPO.DESCR) ? item.HC_DOCUMENTO_TIPO.DESCR.Trim() : string.Empty : string.Empty
                                });
                            };

                        var DatosReporte = new cHistoriaClinicaReporte();
                        #region Definicion de datos base hacia el reporte
                        DatosReporte.FechaEstudio = detalleHistoria.ESTUDIO_FEC.HasValue ? detalleHistoria.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                        var _centro = new cCentro().GetData(x => x.ID_CENTRO == SelectedIngreso.ID_CENTRO).FirstOrDefault();
                        if (_centro != null)
                            DatosReporte.CeresoReclusion = !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty;

                        DatosReporte.Nombre = string.Format("{0} {1} {2}", SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.NOMBRE) ? SelectedIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                                                                          SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.PATERNO) ? SelectedIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                                                                          SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? SelectedIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);

                        DatosReporte.Edad = new Fechas().CalculaEdad(SelectedIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString();
                        DatosReporte.EdoCivil = SelectedIngreso != null ? SelectedIngreso.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectedIngreso.ESTADO_CIVIL.DESCR) ? SelectedIngreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        var municipio = new cMunicipio().Obtener(SelectedIngreso.IMPUTADO.NACIMIENTO_ESTADO.Value, SelectedIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                        DatosReporte.LugarFecNac = string.Format("{0} {1} {2} {3} {4}",
                            SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.NACIMIENTO_LUGAR) ? SelectedIngreso.IMPUTADO.NACIMIENTO_LUGAR.Trim() : string.Empty : string.Empty,
                            SelectedIngreso.IMPUTADO != null ? municipio != null ? !string.IsNullOrEmpty(municipio.MUNICIPIO1) ? municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty : string.Empty,
                            SelectedIngreso.IMPUTADO != null ? municipio != null ? municipio.ENTIDAD != null ? !string.IsNullOrEmpty(municipio.ENTIDAD.DESCR) ? municipio.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectedIngreso.IMPUTADO != null ? municipio != null ? municipio.ENTIDAD != null ? municipio.ENTIDAD.PAIS_NACIONALIDAD != null ? !string.IsNullOrEmpty(municipio.ENTIDAD.PAIS_NACIONALIDAD.PAIS) ? municipio.ENTIDAD.PAIS_NACIONALIDAD.PAIS.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectedIngreso.IMPUTADO != null ? SelectedIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectedIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty);

                        DatosReporte.APartirD = SelectedIngreso.FEC_INGRESO_CERESO.HasValue ? SelectedIngreso.FEC_INGRESO_CERESO.Value.ToString("dd/MM/yyyy") : string.Empty;
                        DatosReporte.Sexo = SelectedIngreso.IMPUTADO != null ? SelectedIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO" : string.Empty;
                        DatosReporte.Escolaridad = SelectedIngreso != null ? SelectedIngreso.ID_ESCOLARIDAD.HasValue ? !string.IsNullOrEmpty(SelectedIngreso.ESCOLARIDAD.DESCR) ? SelectedIngreso.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        DatosReporte.Ocupacion = SelectedIngreso != null ? SelectedIngreso.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(SelectedIngreso.OCUPACION.DESCR) ? SelectedIngreso.OCUPACION.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        var _delitos = new cCausaPenalDelito().Obtener(Convert.ToInt32(SelectedIngreso.ID_CENTRO), Convert.ToInt32(SelectedIngreso.ID_ANIO), Convert.ToInt32(SelectedIngreso.ID_IMPUTADO), Convert.ToInt32(SelectedIngreso.ID_INGRESO));
                        if (_delitos.Any())
                            foreach (var item in _delitos)
                            {
                                if (_delitos.Count > 1)
                                    DatosReporte.Delito += string.Format("{0} y ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                                else
                                    DatosReporte.Delito += string.Format("{0} ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                            };

                        DatosReporte.MedicamentosActuales = detalleHistoria.APP_MEDICAMENTOS_ACTIVOS;
                        DatosReporte.Sentencia = CalcularSentenciaString().ToUpper();

                        #endregion
                        #region Datos Reporte
                        DatosReporte.Directo = detalleHistoria.DIRECTO == (short)eSINO.SI ? "X" : string.Empty;
                        DatosReporte.ConstanciasDoc = detalleHistoria.POR_CONSTANCIAS_DOCUMENTALES == (short)eSINO.SI ? "X" : string.Empty;
                        DatosReporte.Nacimiento = detalleHistoria.APNP_NACIMIENTO;
                        DatosReporte.Aliment = detalleHistoria.APNP_ALIMENTACION;
                        DatosReporte.ExpFisica = detalleHistoria.EF_DESCRIPCION;
                        DatosReporte.Habitacion = detalleHistoria.APNP_HABITACION;
                        DatosReporte.Alcoh = string.Format("{0} {1}", detalleHistoria.APNP_ALCOHOLISMO == "S" ? "SI" : "NO", string.Concat(" Especifique: " + detalleHistoria.APNP_ALCOHOLISMO_OBSERV));
                        DatosReporte.Tabaq = string.Format("{0} {1}", detalleHistoria.APNP_TABAQUISMO == "S" ? "SI" : "NO", string.Concat(" Especifique: " + detalleHistoria.APNP_TABAQUISMO_OBSERV));
                        DatosReporte.Toxic = string.Format("{0} {1}", detalleHistoria.APNP_TOXICOMANIAS == "S" ? "SI" : "NO", string.Concat(" Especifique: " + detalleHistoria.APNP_TOXICOMANIAS_OBSERV));
                        DatosReporte.Menarquia = detalleHistoria.MU_MENARQUIA.HasValue ? detalleHistoria.MU_MENARQUIA.Value.ToString() : string.Empty;
                        DatosReporte.PadecimientoActual = detalleHistoria.PADECIMIENTO_ACTUAL;
                        DatosReporte.Repiratorio = detalleHistoria.RESPIRATORIO;
                        DatosReporte.CardioVas = detalleHistoria.CARDIOVASCULAR;
                        DatosReporte.Digestion = detalleHistoria.DIGESTIVO;
                        DatosReporte.Urinario = detalleHistoria.URINARIO;
                        DatosReporte.GenitalM = detalleHistoria.GENITAL_MUJERES;
                        DatosReporte.Genitall = detalleHistoria.GENITAL_HOMBRES;
                        DatosReporte.Endocrino = detalleHistoria.ENDOCRINO;
                        DatosReporte.MusculoEsq = detalleHistoria.MUSCULO_ESQUELETICO;
                        DatosReporte.Hematico = detalleHistoria.HEMATICO_LINFACTICO;
                        DatosReporte.Nervioso = detalleHistoria.NERVIOSO;
                        DatosReporte.PielAnexos = detalleHistoria.PIEL_ANEXOS;
                        DatosReporte.SintGrales = detalleHistoria.SINTOMAS_GENERALES;
                        DatosReporte.TerapePrevia = detalleHistoria.TERAPEUTICA_PREVIA;
                        DatosReporte.Peso = detalleHistoria.EF_PESO;
                        DatosReporte.Estatura = detalleHistoria.EF_ESTATURA;
                        DatosReporte.Cbeza = detalleHistoria.EF_CABEZA;
                        DatosReporte.Cuello = detalleHistoria.EF_CUELLO;
                        DatosReporte.Torax = detalleHistoria.EF_TORAX;
                        DatosReporte.Abdomen = detalleHistoria.EF_ABDOMEN;
                        DatosReporte.Recto = detalleHistoria.EF_RECTO;
                        DatosReporte.Genital = detalleHistoria.EF_GENITALES;
                        DatosReporte.Extremi = detalleHistoria.EF_EXTREMIDADES;
                        DatosReporte.PresArt = detalleHistoria.EF_PRESION_ARTERIAL;
                        DatosReporte.Puls = detalleHistoria.EF_PULSO;
                        DatosReporte.Repir = detalleHistoria.EF_RESPIRACION;
                        DatosReporte.Tempe = detalleHistoria.EF_TEMPERATURA;
                        DatosReporte.ResultadosAnalisisClin = detalleHistoria.EF_RESULTADO_ANALISIS;
                        DatosReporte.ResultadosEstudiosGani = detalleHistoria.EF_RESULTADO_GABINETE;
                        DatosReporte.ImpresDiagn = detalleHistoria.EF_IMPRESION_DIAGNOSTICA;
                        DatosReporte.AlterVisual = string.Format("Presbicia ({0}) Cataratas ({1}) Pterigion ({2}) Otros ({3}) {4}", detalleHistoria.M65_ALTERACION_VISUAL == 0 ? "X" : string.Empty,
                            detalleHistoria.M65_ALTERACION_VISUAL == 1 ? "X" : string.Empty, detalleHistoria.M65_ALTERACION_VISUAL == 2 ? "X" : string.Empty, detalleHistoria.M65_ALTERACION_VISUAL == 3 ? "X" : string.Empty, detalleHistoria.M65_OTROS);
                        DatosReporte.AltAgudAud = detalleHistoria.M65_ALTERACION_AUDITIVA == "S" ? "SI" : "NO";
                        DatosReporte.AltOlfacion = detalleHistoria.M65_ALTERACION_OLFACION == "S" ? "SI" : "NO";
                        DatosReporte.AlterVisom = detalleHistoria.M65_ALTERACION_VISOMOTRIZ == "S" ? "SI" : "NO";
                        DatosReporte.TransMem = string.Format("ninguno ({0})  superficiales ({1})  moderados ({2})  graves ({3})", detalleHistoria.M65_TRAS_MEMORIA == 0 ? "X" : string.Empty
                            , detalleHistoria.M65_TRAS_MEMORIA == 1 ? "X" : string.Empty, detalleHistoria.M65_TRAS_MEMORIA == 2 ? "X" : string.Empty, detalleHistoria.M65_TRAS_MEMORIA == 3 ? "X" : string.Empty);
                        DatosReporte.TransAten = string.Format("ninguno ({0})  superficiales ({1})  moderados ({2})  graves ({3})", detalleHistoria.M65_TRAS_ATENCION == 0 ? "X" : string.Empty
                            , detalleHistoria.M65_TRAS_ATENCION == 1 ? "X" : string.Empty, detalleHistoria.M65_TRAS_ATENCION == 2 ? "X" : string.Empty, detalleHistoria.M65_TRAS_ATENCION == 3 ? "X" : string.Empty);
                        DatosReporte.TransCompre = string.Format("ninguno ({0})  superficiales ({1})  moderados ({2})  graves ({3})", detalleHistoria.M65_TRAS_COMPRENSION == 0 ? "X" : string.Empty
                            , detalleHistoria.M65_TRAS_COMPRENSION == 1 ? "X" : string.Empty, detalleHistoria.M65_TRAS_COMPRENSION == 2 ? "X" : string.Empty, detalleHistoria.M65_TRAS_COMPRENSION == 3 ? "X" : string.Empty);
                        DatosReporte.TransCompre = string.Format("ninguno ({0})  superficiales ({1})  moderados ({2})  graves ({3})", detalleHistoria.M65_TRAS_COMPRENSION == 0 ? "X" : string.Empty
                            , detalleHistoria.M65_TRAS_COMPRENSION == 1 ? "X" : string.Empty, detalleHistoria.M65_TRAS_COMPRENSION == 2 ? "X" : string.Empty, detalleHistoria.M65_TRAS_COMPRENSION == 3 ? "X" : string.Empty);
                        DatosReporte.TransOrien = string.Format("ninguno ({0})  superficiales ({1})  moderados ({2})  graves ({3})", detalleHistoria.M65_TRAS_ORIENTACION == 0 ? "X" : string.Empty
                            , detalleHistoria.M65_TRAS_ORIENTACION == 1 ? "X" : string.Empty, detalleHistoria.M65_TRAS_ORIENTACION == 2 ? "X" : string.Empty, detalleHistoria.M65_TRAS_ORIENTACION == 3 ? "X" : string.Empty);
                        DatosReporte.TransDemenciales = string.Format("SI ({0})  NO ({1})", detalleHistoria.M65_TRAS_DEMENCIAL == "S" ? "X" : string.Empty, detalleHistoria.M65_TRAS_DEMENCIAL == "N" ? "X" : string.Empty);
                        DatosReporte.TransEdoAnimo = string.Format("Disforia ({0})  Incontinencia afectiva ({1})  Depresión ({2}) ", detalleHistoria.M65_TRAS_ANIMO == 0 ? "X" : string.Empty
                            , detalleHistoria.M65_TRAS_ANIMO == 1 ? "X" : string.Empty, detalleHistoria.M65_TRAS_ANIMO == 2 ? "X" : string.Empty);
                        DatosReporte.CapacActivProRead = string.Format("Integra, ya que puede participar en todas las actividades ({0}) \n Limitada, solo es posible que pueda participar en algunas ({1}) \n Nula, a pesar de su voluntad de participar, no cuenta con las habilidades para beneficiarse de ellas ({2})",
                            detalleHistoria.M65_PARTICIPACION == 0 ? "X" : string.Empty, detalleHistoria.M65_PARTICIPACION == 1 ? "X" : string.Empty, detalleHistoria.M65_PARTICIPACION == 2 ? "X" : string.Empty);
                        DatosReporte.GravedadEnfermedad = string.Format("leve ({0})  moderada ({1})  severa ({2})", detalleHistoria.CP_GRAVEDAD == 0 ? "X" : string.Empty, detalleHistoria.CP_GRAVEDAD == 1 ? "X" : string.Empty,
                            detalleHistoria.CP_GRAVEDAD == 2 ? "X" : string.Empty);
                        DatosReporte.EtapaEvol = string.Format("inicial ({0})  media ({1})  terminal ({2})", detalleHistoria.CP_ETAPA_EVOLUTIVA == 0 ? "X" : string.Empty, detalleHistoria.CP_ETAPA_EVOLUTIVA == 1 ? "X" : string.Empty
                            , detalleHistoria.CP_ETAPA_EVOLUTIVA == 2 ? "X" : string.Empty);
                        DatosReporte.PosibRemision = string.Format("Reversibles con tratamiento adecuado ({0}) \n  Irreversibles a pesar de tratamiento -sólo paliativo - ({1})", detalleHistoria.CP_REMISION == 0 ? "X" : string.Empty,
                            detalleHistoria.CP_REMISION == 1 ? "X" : string.Empty);
                        DatosReporte.GradoAfectacion = detalleHistoria.CP_GRADO_AFECTACION;
                        DatosReporte.Pronostico = detalleHistoria.CP_PRONOSTICO;
                        DatosReporte.CapacBrindTratam = string.Format("Suficiente ({0})  Mediana ({1})  Escasa ({2})  Nula ({3})", detalleHistoria.CP_CAPACIDAD_TRATAMIENTO == 0 ? "X" : string.Empty, detalleHistoria.CP_CAPACIDAD_TRATAMIENTO == 1 ? "X" : string.Empty
                            , detalleHistoria.CP_CAPACIDAD_TRATAMIENTO == 2 ? "X" : string.Empty, detalleHistoria.CP_CAPACIDAD_TRATAMIENTO == 3 ? "X" : string.Empty);
                        DatosReporte.NivelAtencionReq = string.Format("I Nivel ({0})   II Nivel ({1})   III Nivel ({2})", detalleHistoria.CP_NIVEL_ATENCION == 0 ? "X" : string.Empty, detalleHistoria.CP_NIVEL_ATENCION == 1 ? "X" : string.Empty
                            , detalleHistoria.CP_NIVEL_ATENCION == 2 ? "X" : string.Empty);
                        DatosReporte.Conclusiones = detalleHistoria.CONCLUSIONES;
                        DatosReporte.NomDoctor = detalleHistoria.DOCTOR;
                        DatosReporte.CedulaProf = detalleHistoria.CEDULA_PROFESIONAL;
                        DatosReporte.AniosRitmo = detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE != null ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().ANIOS_RITMO : string.Empty : string.Empty;
                        DatosReporte.Embarazos = detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE != null ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().EMBARAZO.HasValue ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().EMBARAZO.Value.ToString() : string.Empty : string.Empty : string.Empty;
                        DatosReporte.Partos = detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE != null ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().PARTO.HasValue ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().PARTO.Value.ToString() : string.Empty : string.Empty : string.Empty;
                        DatosReporte.Abortos = detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE != null ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().ABORTO.HasValue ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().ABORTO.Value.ToString() : string.Empty : string.Empty : string.Empty;
                        DatosReporte.Cesareas = detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE != null ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().CESAREA.HasValue ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().CESAREA.Value.ToString() : string.Empty : string.Empty : string.Empty;
                        DatosReporte.FecUltimaMenstr = detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE != null ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().ULTIMA_MENSTRUACION_FEC.HasValue ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().ULTIMA_MENSTRUACION_FEC.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty : string.Empty;
                        DatosReporte.FecProbParto = detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE != null ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().FECHA_PROBABLE_PARTO.HasValue ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().FECHA_PROBABLE_PARTO.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty : string.Empty;
                        DatosReporte.DeformaOrga = detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE != null ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().DEFORMACION : string.Empty : string.Empty;
                        DatosReporte.ControlP = detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE != null ?
                                                detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ?
                                                detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().CONTROL_PRENATAL == "S" ? "SI" :
                                                detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().CONTROL_PRENATAL == "N" ? "NO" :
                                                    string.Empty : string.Empty : string.Empty;
                        DatosReporte.CualControlP = detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE != null ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().CONTROL_PRENATAL1 != null ? !string.IsNullOrEmpty(detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().CONTROL_PRENATAL1.DESCR) ? detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault().CONTROL_PRENATAL1.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty;

                        #endregion

                        #region Inicializacion de reporte
                        ReporteHCM.LocalReport.ReportPath = "Reportes/rHistoriaClinica.rdlc";
                        ReporteHCM.LocalReport.DataSources.Clear();
                        #endregion

                        #region Encabezado
                        var Encabezado = new cEncabezado();
                        Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                        Encabezado.TituloDos = Parametro.ENCABEZADO2;
                        Encabezado.NombreReporte = "HISTORIA CLÍNICA";
                        Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                        Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;
                        Encabezado.PieUno = string.Concat("NOMBRE DEL INTERNO: ", string.Format("{0}/{1} ", SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO), string.Format("{0} {1} {2}",
                                                    !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.NOMBRE) ? SelectedIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.PATERNO) ? SelectedIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? SelectedIngreso.IMPUTADO.MATERNO.Trim() : string.Empty), ", UBICACIÓN ACTUAL: ", string.Format("{0}-{1}{2}-{3}",
                                                    SelectedIngreso != null ? SelectedIngreso.CAMA != null ? SelectedIngreso.CAMA.CELDA != null ? SelectedIngreso.CAMA.CELDA.SECTOR != null ? SelectedIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectedIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectedIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectedIngreso != null ? SelectedIngreso.CAMA != null ? SelectedIngreso.CAMA.CELDA != null ? SelectedIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectedIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectedIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectedIngreso != null ? SelectedIngreso.CAMA != null ? SelectedIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectedIngreso.CAMA.CELDA.ID_CELDA) ? SelectedIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectedIngreso != null ? SelectedIngreso.ID_UB_CAMA.HasValue ? SelectedIngreso.ID_UB_CAMA.Value.ToString() : string.Empty : string.Empty));
                        #endregion
                        #region Definicion de datos patologicos
                        ObservableCollection<cPatologicosHistoriaFamiliar> lstPatologicos = new ObservableCollection<cPatologicosHistoriaFamiliar>();
                        cPatologicosHistoriaFamiliar _datoIndividual = new cPatologicosHistoriaFamiliar();
                        if (detalleHistoria.HISTORIA_CLINICA_PATOLOGICOS != null && detalleHistoria.HISTORIA_CLINICA_PATOLOGICOS.Any())
                            foreach (var item in detalleHistoria.HISTORIA_CLINICA_PATOLOGICOS)
                            {
                                _datoIndividual = new cPatologicosHistoriaFamiliar()
                                {
                                    MomentoDeteccion = item.MOMENTO_DETECCION == "EI" ? "EXAMEN INICIAL" : item.MOMENTO_DETECCION == "DI" ? "EN INTERNAMIENTO" : string.Empty,
                                    OtrosDescripcion = item.OBSERVACIONES,
                                    Patologico = item.PATOLOGICO_CAT != null ? !string.IsNullOrEmpty(item.PATOLOGICO_CAT.DESCR) ? item.PATOLOGICO_CAT.DESCR.Trim() : string.Empty : string.Empty,
                                    Recuperado = item.RECUPERADO == "S" ? "SI" : item.RECUPERADO == "N" ? "NO" : string.Empty,
                                    RegistroFecha = item.REGISTRO_FEC.HasValue ? item.REGISTRO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty
                                };

                                lstPatologicos.Add(_datoIndividual);
                            };

                        List<cPatologicosGralesHistoriaClinica> lstPatos = new List<cPatologicosGralesHistoriaClinica>();
                        var _PatologicosActuales = new cPatologicoCat().GetData(y => y.ESTATUS == "S", z => z.DESCR);
                        if (_PatologicosActuales != null && _PatologicosActuales.Any())
                            foreach (var item in _PatologicosActuales)
                                if (detalleHistoria.HISTORIA_CLINICA_PATOLOGICOS.Any(x => x.ID_PATOLOGICO == item.ID_PATOLOGICO))
                                    lstPatos.Add(new cPatologicosGralesHistoriaClinica { NombrePatologico = string.Format("{0} {1}", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty, "SI DETECTADO") });
                                else
                                    lstPatos.Add(new cPatologicosGralesHistoriaClinica { NombrePatologico = string.Format("{0} {1}", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty, "NO DETECTADO") });

                        #region Definicion de arreglo para el condensado de patologicos
                        string _Padece = string.Empty;
                        List<cPatologicosGralesHistoriaClinica> patos1 = new List<cPatologicosGralesHistoriaClinica>();
                        List<cPatologicosGralesHistoriaClinica> patos2 = new List<cPatologicosGralesHistoriaClinica>();
                        List<cPatologicosGralesHistoriaClinica> patos3 = new List<cPatologicosGralesHistoriaClinica>();
                        List<cPatologicosGralesHistoriaClinica> patos4 = new List<cPatologicosGralesHistoriaClinica>();
                        if (lstPatos != null && lstPatos.Any())
                        {
                            short _dummy = 1;
                            foreach (var item in lstPatos)
                            {
                                if (_dummy == 5)
                                    _dummy = 1;

                                switch (_dummy)
                                {
                                    case 1:
                                        patos1.Add(item);
                                        _dummy++;
                                        break;
                                    case 2:
                                        patos2.Add(item);
                                        _dummy++;
                                        break;
                                    case 3:
                                        patos3.Add(item);
                                        _dummy++;
                                        break;
                                    case 4:
                                        patos4.Add(item);
                                        _dummy++;
                                        break;
                                    default:
                                        ///no case
                                        break;
                                }
                            };
                        };

                        #endregion
                        cPatologicosGralesHistoriaClinica _d = new cPatologicosGralesHistoriaClinica();
                        //_d.ImagenPatos = CaptureWebPageBytesP(_Padece, null, null);
                        lstPatos.Clear();
                        lstPatos.Add(_d);

                        Microsoft.Reporting.WinForms.ReportDataSource rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds5.Name = "DataSet5";
                        rds5.Value = lstPatos;
                        ReporteHCM.LocalReport.DataSources.Add(rds5);


                        Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds4.Name = "DataSet4";
                        rds4.Value = lstPatologicos;
                        ReporteHCM.LocalReport.DataSources.Add(rds4);


                        Microsoft.Reporting.WinForms.ReportDataSource rds7 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds7.Name = "DataSet7";
                        rds7.Value = patos1;
                        ReporteHCM.LocalReport.DataSources.Add(rds7);

                        Microsoft.Reporting.WinForms.ReportDataSource rds8 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds8.Name = "DataSet8";
                        rds8.Value = patos2;
                        ReporteHCM.LocalReport.DataSources.Add(rds8);

                        Microsoft.Reporting.WinForms.ReportDataSource rds9 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds9.Name = "DataSet9";
                        rds9.Value = patos3;
                        ReporteHCM.LocalReport.DataSources.Add(rds9);

                        Microsoft.Reporting.WinForms.ReportDataSource rds10 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds10.Name = "DataSet10";
                        rds10.Value = patos4;
                        ReporteHCM.LocalReport.DataSources.Add(rds10);

                        #endregion
                        #region Detalle de familiares
                        ObservableCollection<cFamiliaresHistoriaClinica> lstFamiliares = new ObservableCollection<cFamiliaresHistoriaClinica>();
                        cFamiliaresHistoriaClinica _dato = new cFamiliaresHistoriaClinica();
                        if (detalleHistoria.HISTORIA_CLINICA_FAMILIAR != null && detalleHistoria.HISTORIA_CLINICA_FAMILIAR.Any())
                            foreach (var item in detalleHistoria.HISTORIA_CLINICA_FAMILIAR)
                            {
                                _dato = new cFamiliaresHistoriaClinica()
                                {
                                    Edad = item.AHF_EDAD.HasValue ? item.AHF_EDAD.Value.ToString() : string.Empty,
                                    Fallecio = item.AHF_VIVE == "N" ? "SI" : "NO",
                                    CausaMuerte = !string.IsNullOrEmpty(item.AHF_FALLECIMIENTO_CAUSA) ? item.AHF_FALLECIMIENTO_CAUSA.Trim() : string.Empty,
                                    FechaMuerte = item.AHF_FALLECIMIENTO_FEC.HasValue ? item.AHF_FALLECIMIENTO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                                    Vive = item.AHF_VIVE == "S" ? "SI" : "NO",
                                    Padecimientos = item.AHF_SANO == "S" ? "SI" : "NO",
                                    Familiar = item.ID_TIPO_REFERENCIA.HasValue ? !string.IsNullOrEmpty(item.TIPO_REFERENCIA.DESCR) ? item.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                    DescripcionPadecimientos = string.Format("ALERGIAS: {0}, CA: {1}, CARDIACOS: {2}, DIABETES: {3}, EPILEPSIA: {4}, HIPERTENSO: {5}, PROBLEMAS MENTALES: {6}, TB: {7}", item.AHF_ALERGIAS == "S" ? "SI" : "NO", item.AHF_CA == "S" ? "SI" : "NO",
                                    item.AHF_CARDIACOS == "S" ? "SI" : "NO", item.AHF_DIABETES == "S" ? "SI" : "NO", item.AHF_EPILEPSIA == "S" ? "SI" : "NO", item.AHF_HIPERTENSIVO == "S" ? "SI" : "NO", item.AHF_MENTALES == "S" ? "SI" : "NO", item.AHF_TB == "S" ? "SI" : "NO"),
                                };

                                lstFamiliares.Add(_dato);
                            };

                        Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds3.Name = "DataSet3";
                        rds3.Value = lstFamiliares;
                        ReporteHCM.LocalReport.DataSources.Add(rds3);

                        #endregion
                        #region Detalle de Gineco obstetricos
                        ObservableCollection<cGinecoObstetricosReporte> lstGinecoObstetricos = new ObservableCollection<cGinecoObstetricosReporte>();
                        if (detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE.Any())
                            foreach (var item in detalleHistoria.HISTORIA_CLINICA_GINECO_OBSTRE)
                            {
                                lstGinecoObstetricos.Add(new cGinecoObstetricosReporte
                                {
                                    AniosRitmo = item.ANIOS_RITMO,
                                    Cesarea = item.CESAREA.HasValue ? item.CESAREA.Value.ToString() : string.Empty,
                                    Embarazo = item.EMBARAZO.HasValue ? item.EMBARAZO.Value.ToString() : string.Empty,
                                    FechaRegistro = item.REGISTRO_FEC.HasValue ? item.REGISTRO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                                    Parto = item.PARTO.HasValue ? item.PARTO.Value.ToString() : string.Empty,
                                    UltimaMenst = item.ULTIMA_MENSTRUACION_FEC.HasValue ? item.ULTIMA_MENSTRUACION_FEC.Value.ToString("dd/MM/yyyy") : string.Empty
                                });
                            };

                        Microsoft.Reporting.WinForms.ReportDataSource rds6 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds6.Name = "DataSet6";
                        rds6.Value = lstGinecoObstetricos;
                        ReporteHCM.LocalReport.DataSources.Add(rds6);
                        #endregion
                        #region Definicion de origenes de dato

                        var ds1 = new List<cEncabezado>();
                        ds1.Add(Encabezado);
                        Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds1.Name = "DataSet2";
                        rds1.Value = ds1;
                        ReporteHCM.LocalReport.DataSources.Add(rds1);

                        var ds2 = new List<cHistoriaClinicaReporte>();
                        ds2.Add(DatosReporte);
                        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds2.Name = "DataSet1";
                        rds2.Value = ds2;
                        ReporteHCM.LocalReport.DataSources.Add(rds2);
                        #endregion
                        ReporteHCM.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                        ReporteHCM.RefreshReport();
                    }
                    else
                        VisibleReporteHistoriaClinicaMedica = Visibility.Collapsed;
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private void ImprimeHistoriaClinicaDental()
        {
            try
            {
                ReporteHCD.Reset();
                ListArchivosHCD = new ObservableCollection<CustomArchivosSimple>();

                if (SelectedIngreso != null)
                {
                    VisibleReporteHistoriaClinicaDental = Visibility.Collapsed;

                    var detalleDental = new cHistoriaClinicaDental().GetData(x => x.ID_INGRESO == SelectedIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectedIngreso.ID_IMPUTADO && x.ID_ANIO == SelectedIngreso.ID_ANIO).FirstOrDefault();

                    TieneHistoriaClinicaDental = detalleDental != null;
                    if (detalleDental == null)
                        return;

                    VisibleReporteHistoriaClinicaDental = Visibility.Visible;

                    if (detalleDental.HISTORIA_CLINICA_DENTAL_DOCUME != null && detalleDental.HISTORIA_CLINICA_DENTAL_DOCUME.Any())
                        foreach (var item in detalleDental.HISTORIA_CLINICA_DENTAL_DOCUME)
                        {
                            ListArchivosHCD.Add(new CustomArchivosSimple()
                            {
                                ConsecutivoDental = item.ID_HCDDOCTO,
                                TipoDocumento = item.HC_DOCUMENTO_TIPO != null ? !string.IsNullOrEmpty(item.HC_DOCUMENTO_TIPO.DESCR) ? item.HC_DOCUMENTO_TIPO.DESCR.Trim() : string.Empty : string.Empty
                            });
                        };

                    cHistoriaClinicaDentalReporte DatosReporte = new cHistoriaClinicaDentalReporte();
                    ReportesView View = new ReportesView();
                    List<cDientesReporte> lstDetalleDientes = new List<cDientesReporte>();
                    List<cDientesReporte> lstSeguimientos = new List<cDientesReporte>();
                    var _centro = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (detalleDental != null)
                    {
                        string dientes = Convert.ToBase64String(new Imagenes().getImagen("miniDiente.png"));
                        string _htmlDiente = "<html><table><tr><td>";
                        for (int i = 0; i < 16; i++)
                            _htmlDiente += "<image id='ImagenDiente' style='width:20px;height:50px;' src = \" data:image/png;base64, " + dientes + " alt=\"diente" + i + "\" /></td><td>";

                        _htmlDiente += "</td></tr></table></html>";

                        DatosReporte.Generico18 = CaptureWebPageBytesP(_htmlDiente, 400, 100);
                        string _Padece = string.Empty;
                        var HistoriaClinicaMedica = new cHistoriaClinica().GetData(x => x.ID_INGRESO == SelectedIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectedIngreso.ID_IMPUTADO && x.ID_ANIO == SelectedIngreso.ID_ANIO).FirstOrDefault();
                        if (HistoriaClinicaMedica != null)
                        {
                            var patos = HistoriaClinicaMedica.HISTORIA_CLINICA_PATOLOGICOS;
                            if (patos != null && patos.Any())
                            {
                                #region Definicion de arreglo para el condensado
                                if (patos != null && patos.Any())
                                {
                                    _Padece += "<html><table><tr><td><font face=\"Arial\" size=1>";

                                    short _dummy = 0;//sirve para determinar la cantidad de patologicos por linea
                                    foreach (var item in patos)
                                    {
                                        _dummy++;
                                        if (_dummy == 3)
                                        {
                                            _Padece += (item.PATOLOGICO_CAT != null ? !string.IsNullOrEmpty(item.PATOLOGICO_CAT.DESCR) ? item.PATOLOGICO_CAT.DESCR.Trim() : string.Empty : string.Empty) + (": ") + ("<strong>") + (item.RECUPERADO == "S" ? "SI" : "NO") + ("</strong>") + "</font></td></tr><tr><td><font face=\"Arial\" size=1>";
                                            _dummy = 0;
                                        }
                                        else
                                            _Padece += (item.PATOLOGICO_CAT != null ? !string.IsNullOrEmpty(item.PATOLOGICO_CAT.DESCR) ? item.PATOLOGICO_CAT.DESCR.Trim() : string.Empty : string.Empty) + (": ") + ("<strong>") + (item.RECUPERADO == "S" ? "SI" : "NO") + ("</strong>") + "</font></td><td><font face=\"Arial\" size=1>";
                                    };

                                    _Padece += "</tr></table></html>";
                                };

                                #endregion
                            };

                            #region Complementarios
                            DatosReporte.Generico11 = string.Format("{0} {1} {2}",
                                detalleDental.INGRESO != null ? detalleDental.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(detalleDental.INGRESO.IMPUTADO.NOMBRE) ? detalleDental.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                detalleDental.INGRESO != null ? detalleDental.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(detalleDental.INGRESO.IMPUTADO.PATERNO) ? detalleDental.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                detalleDental.INGRESO != null ? detalleDental.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(detalleDental.INGRESO.IMPUTADO.MATERNO) ? detalleDental.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty);

                            DatosReporte.Generico12 = string.Format("{0} {1} {2}",
                                detalleDental.USUARIO != null ? detalleDental.USUARIO.EMPLEADO != null ? detalleDental.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(detalleDental.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? detalleDental.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                detalleDental.USUARIO != null ? detalleDental.USUARIO.EMPLEADO != null ? detalleDental.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(detalleDental.USUARIO.EMPLEADO.PERSONA.PATERNO) ? detalleDental.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                                                                detalleDental.USUARIO != null ? detalleDental.USUARIO.EMPLEADO != null ? detalleDental.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(detalleDental.USUARIO.EMPLEADO.PERSONA.MATERNO) ? detalleDental.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty);

                            #endregion

                            DatosReporte.ImagenPatologisoDentales = CaptureWebPageBytesP(_Padece, 400, 100);
                            DatosReporte.Alcohlismo = string.Format("SI [ {0} ]   NO [ {1} ]", HistoriaClinicaMedica.APNP_ALCOHOLISMO == "S" ? "X" : string.Empty, HistoriaClinicaMedica.APNP_ALCOHOLISMO == "N" ? "X" : string.Empty);
                            DatosReporte.Tabaquismo = string.Format("SI [ {0} ]   NO [ {1} ]", HistoriaClinicaMedica.APNP_TABAQUISMO == "S" ? "X" : string.Empty, HistoriaClinicaMedica.APNP_TABAQUISMO == "N" ? "X" : string.Empty);
                            DatosReporte.Toxicomanias = string.Format("SI [ {0} ]   NO [ {1} ]", HistoriaClinicaMedica.APNP_TOXICOMANIAS == "S" ? "X" : string.Empty, HistoriaClinicaMedica.APNP_TOXICOMANIAS == "N" ? "X" : string.Empty);
                            DatosReporte.TomandoMedicamento = string.Format("SI [ {0} ]   NO [ {1} ]", !string.IsNullOrEmpty(HistoriaClinicaMedica.APP_MEDICAMENTOS_ACTIVOS) ? "X" : string.Empty, string.IsNullOrEmpty(HistoriaClinicaMedica.APP_MEDICAMENTOS_ACTIVOS) ? "X" : string.Empty);
                            DatosReporte.CualMedicamentoToma = HistoriaClinicaMedica.APP_MEDICAMENTOS_ACTIVOS;
                            DatosReporte.EstaEmbarazada = string.Format("SI [ {0} ]   NO [ {1} ]", HistoriaClinicaMedica.HISTORIA_CLINICA_GINECO_OBSTRE != null ? HistoriaClinicaMedica.HISTORIA_CLINICA_GINECO_OBSTRE.Any(x => x.EMBARAZO > 0) == true ? "X" : string.Empty : string.Empty, HistoriaClinicaMedica.HISTORIA_CLINICA_GINECO_OBSTRE != null ? HistoriaClinicaMedica.HISTORIA_CLINICA_GINECO_OBSTRE.Any(x => x.EMBARAZO == 0) == true ? "X" : string.Empty : string.Empty);

                            var _familiares = HistoriaClinicaMedica.HISTORIA_CLINICA_FAMILIAR;
                            if (_familiares != null && _familiares.Any())
                            {
                                DatosReporte.Generico2 = _familiares.Any(x => x.AHF_ALERGIAS == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico3 = _familiares.Any(x => x.AHF_CA == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico4 = _familiares.Any(x => x.AHF_CARDIACOS == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico5 = _familiares.Any(x => x.AHF_DIABETES == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico6 = _familiares.Any(x => x.AHF_EPILEPSIA == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico7 = _familiares.Any(x => x.AHF_HIPERTENSIVO == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico8 = _familiares.Any(x => x.AHF_MENTALES == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                                DatosReporte.Generico9 = _familiares.Any(x => x.AHF_TB == "S") ? string.Format(" SI [ {0} ]   NO [ {1} ]", "X", string.Empty) : string.Format(" SI [ {0} ]   NO [ {1} ]", string.Empty, "X");
                            };
                        }
                        else
                            DatosReporte.Alcohlismo = DatosReporte.Tabaquismo = DatosReporte.Toxicomanias = DatosReporte.TomandoMedicamento = DatosReporte.EstaEmbarazada = DatosReporte.Generico2 = DatosReporte.Generico3 = DatosReporte.Generico4 = DatosReporte.Generico5 = DatosReporte.Generico6 = DatosReporte.Generico7 = DatosReporte.Generico8 = DatosReporte.Generico9 = "SI [  ]   NO [  ]";

                        DatosReporte.Generico10 = string.Empty;//dato limpio a proposito
                        DatosReporte.Generico = string.Format("HOMBRE [ {0} ]   MUJER [ {1} ]", SelectedIngreso.IMPUTADO != null ? SelectedIngreso.IMPUTADO.SEXO == "M" ? "X" : string.Empty : string.Empty, SelectedIngreso.IMPUTADO != null ? SelectedIngreso.IMPUTADO.SEXO == "F" ? "X" : string.Empty : string.Empty);
                        DatosReporte.NombrePaciente = string.Format("{0} {1} {2}",
                            SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.NOMBRE) ? SelectedIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                            SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.PATERNO) ? SelectedIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                            SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? SelectedIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);
                        DatosReporte.NoExpediente = string.Format("{0} / {1}", SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO);
                        DatosReporte.Hora = detalleDental.REGISTRO_FEC.HasValue ? detalleDental.REGISTRO_FEC.Value.ToString("HH : mm") : string.Empty;
                        DatosReporte.AlergicoMedicamento = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.ALERGICO_MEDICAMENTO == "S" ? "X" : string.Empty, detalleDental.ALERGICO_MEDICAMENTO == "N" ? "X" : string.Empty);
                        DatosReporte.ReaccionNegativa = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.REACCION_ANESTESICO == "S" ? "X" : string.Empty, detalleDental.REACCION_ANESTESICO == "N" ? "X" : string.Empty);
                        DatosReporte.CualMedicamentoAlergico = detalleDental.ALERGICO_MEDICAMENTO_CUAL;
                        DatosReporte.AmenazaAborto = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.AMENAZA_ABORTO == "S" ? "X" : string.Empty, detalleDental.AMENAZA_ABORTO == "N" ? "X" : string.Empty);
                        DatosReporte.Amigdalas = detalleDental.EXP_BUC_AMIGDALAS;
                        DatosReporte.AnomaliasForma = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.DIENTES_ANOM_FORMA == "S" ? "X" : string.Empty, detalleDental.DIENTES_ANOM_FORMA == "N" ? "X" : string.Empty);
                        DatosReporte.AnomaliasTamanio = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.DIENTES_ANOM_TAMANO == "S" ? "X" : string.Empty, detalleDental.DIENTES_ANOM_TAMANO == "N" ? "X" : string.Empty);
                        DatosReporte.Bruxismo = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.BRUXISMO == "S" ? "X" : string.Empty, detalleDental.BRUXISMO == "N" ? "X" : string.Empty);
                        DatosReporte.CansancioMusculosCaraCuello = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.ART_TEMP_CANSANCIO == "S" ? "X" : string.Empty, detalleDental.ART_TEMP_CANSANCIO == "N" ? "X" : string.Empty);
                        DatosReporte.Caries = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.DIENTES_CARIES == "S" ? "X" : string.Empty, detalleDental.DIENTES_CARIES == "N" ? "X" : string.Empty);
                        DatosReporte.Carrillos = detalleDental.EXP_BUC_CARRILLOS;
                        DatosReporte.Chasquidos = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.ART_TEMP_CHASQUIDOS == "S" ? "X" : string.Empty, detalleDental.ART_TEMP_CHASQUIDOS == "N" ? "X" : string.Empty);
                        DatosReporte.Color = detalleDental.ENCIAS_COLORACION;
                        DatosReporte.Complicaciones = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.COMPLICACIONES == "S" ? "X" : string.Empty, detalleDental.COMPLICACIONES == "N" ? "X" : string.Empty);
                        DatosReporte.Dolor = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.ART_TEMP_DOLOR == "S" ? "X" : string.Empty, detalleDental.ART_TEMP_DOLOR == "N" ? "X" : string.Empty);
                        DatosReporte.DolorEspecifique = detalleDental.ART_TEMP_DOLOR_OBS;
                        DatosReporte.Edad = SelectedIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectedIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty;
                        DatosReporte.EspecifiqueBruxismo = string.Format("DOLOR [ {0} ]   ASINTOMÁTICO [ {1} ]", detalleDental.BRUXISMO_DOLOR == "S" ? "X" : string.Empty, detalleDental.BRUXISMO_DOLOR == "N" ? "X" : string.Empty);
                        DatosReporte.EspecifiqueCansancioMusculosCaraCuello = detalleDental.ART_TEMP_CANSANCIO_OBS;
                        DatosReporte.EspecifiqueChasquidos = detalleDental.ART_TEMP_CHASQUIDOS_OBS;
                        DatosReporte.EspecifiqueRigidezMuscMand = detalleDental.ART_TEMP_RIGIDEZ_OBS;
                        DatosReporte.Estatura = detalleDental.ESTATURA;
                        DatosReporte.Fecha = detalleDental.REGISTRO_FEC.HasValue ? detalleDental.REGISTRO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                        DatosReporte.Fluorosis = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.DIENTES_FLUOROSIS == "S" ? "X" : string.Empty, detalleDental.DIENTES_FLUOROSIS == "N" ? "X" : string.Empty);
                        DatosReporte.Forma = detalleDental.ENCIAS_FORMA;
                        DatosReporte.FrecuenciaCard = detalleDental.FRECUENCIA_CARDIAC;
                        DatosReporte.FrecuenciaResp = detalleDental.FRECUENCIA_RESPIRA;
                        DatosReporte.Frenillos = detalleDental.EXP_BUC_FRENILLOS;
                        DatosReporte.Glicemia = detalleDental.GLICEMIA;
                        DatosReporte.Hemorragia = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.HEMORRAGIA == "S" ? "X" : string.Empty, detalleDental.HEMORRAGIA == "N" ? "X" : string.Empty);
                        DatosReporte.Hipoplasias = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.DIENTES_HIPOPLASIA == "S" ? "X" : string.Empty, detalleDental.DIENTES_HIPOPLASIA == "N" ? "X" : string.Empty);
                        DatosReporte.Labios = detalleDental.EXP_BUC_LABIOS;
                        DatosReporte.MucosaNasal = detalleDental.EXP_BUC_MUCOSA_NASAL;
                        DatosReporte.Lengua = detalleDental.EXP_BUC_LENGUA;
                        DatosReporte.PisoBoca = detalleDental.EXP_BUC_PISO_BOCA;
                        DatosReporte.PaladarDuro = detalleDental.EXP_BUC_PALADAR_DURO;
                        DatosReporte.PaladarBlanco = detalleDental.EXP_BUC_PALADAR_BLANCO;
                        DatosReporte.OtrosExploracionBucod = detalleDental.EXP_BUC_OTROS;
                        DatosReporte.OtrosDientes = detalleDental.DIENTES_OTROS;
                        DatosReporte.RigidezMusculsMand = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.ART_TEMP_RIGIDEZ == "S" ? "X" : string.Empty, detalleDental.ART_TEMP_RIGIDEZ == "N" ? "X" : string.Empty);
                        DatosReporte.Textura = detalleDental.ENCIAS_TEXTURA;
                        DatosReporte.TensionArt = detalleDental.TENSION_ARTERIAL;
                        DatosReporte.Temperatura = detalleDental.TEMPERATURA;
                        DatosReporte.Peso = detalleDental.PESO;
                        DatosReporte.Lactando = string.Format("SI [ {0} ]   NO [ {1} ]", detalleDental.LACTANDO == "S" ? "X" : string.Empty, detalleDental.LACTANDO == "N" ? "X" : string.Empty);
                    };

                    var _dientes = detalleDental.ODONTOGRAMA_INICIAL;
                    if (_dientes != null && _dientes.Any())
                    {
                        foreach (var item in _dientes)
                        {
                            var _Imagen = new Imagenes().getImagen("imageNotFound.jpg");
                            string _padec = string.Empty;
                            if (item.ID_ENFERMEDAD.HasValue)
                            {
                                _padec = !string.IsNullOrEmpty(item.ENFERMEDAD.NOMBRE) ? item.ENFERMEDAD.NOMBRE.Trim() : string.Empty;
                                var _ImagenExistente = new cOdontogramaSimbologias().GetData(x => x.ID_ENFERMEDAD == item.ID_ENFERMEDAD).FirstOrDefault();
                                if (_ImagenExistente != null)
                                    _Imagen = _ImagenExistente.IMAGEN;
                            };

                            if (item.ID_NOMENCLATURA.HasValue)
                            {
                                _padec = !string.IsNullOrEmpty(item.DENTAL_NOMENCLATURA.DESCR) ? item.DENTAL_NOMENCLATURA.DESCR.Trim() : string.Empty;
                                var _ImagenExistente = new cOdontogramaSimbologias().GetData(x => x.ID_NOMENCLATURA == item.ID_NOMENCLATURA).FirstOrDefault();
                                if (_ImagenExistente != null)
                                    _Imagen = _ImagenExistente.IMAGEN;
                            };

                            lstDetalleDientes.Add(new cDientesReporte
                            {
                                DienteNombre = string.Format("{0} EN {1} DEL {2} . POSICIÓN {3}",
                                _padec,
                                item.ODONTOGRAMA_TIPO != null ? !string.IsNullOrEmpty(item.ODONTOGRAMA_TIPO.DESCR) ? item.ODONTOGRAMA_TIPO.DESCR.Trim() : string.Empty : string.Empty,
                                item.ODONTOGRAMA != null ? !string.IsNullOrEmpty(item.ODONTOGRAMA.DESCR) ? item.ODONTOGRAMA.DESCR.Trim() : string.Empty : string.Empty,
                                item.ODONTOGRAMA != null ? item.ODONTOGRAMA.ID_POSICION.ToString() : string.Empty),
                                ImagenDiente = _Imagen
                            });
                        };
                    };

                    //FALTA CAMBIO POR ODONTOGRAMA2

                    //var _dientesSeguimiento = detalleDental.ODONTOGRAMA_SEGUIMIENTO2;
                    //if (_dientesSeguimiento != null && _dientesSeguimiento.Any())
                    //{
                    //    var _Imagen = new Imagenes().getImagen("imageNotFound.jpg");
                    //    foreach (var item in _dientesSeguimiento)
                    //    {
                    //        lstSeguimientos.Add(new cDientesReporte
                    //            {
                    //                DienteNombre = item.PROGRAMACION_FEC.HasValue ? item.PROGRAMACION_FEC.Value.ToString("dd/MM/yyyy") : string.Empty,
                    //                Padece = string.Format("{0} EN {1} DEL {2} . POSICIÓN {3}",
                    //                    item.ID_TRATA.HasValue ? !string.IsNullOrEmpty(item.DENTAL_TRATAMIENTO.DESCR) ? item.DENTAL_TRATAMIENTO.DESCR.Trim() : string.Empty : string.Empty,
                    //                    item.ODONTOGRAMA_TIPO != null ? !string.IsNullOrEmpty(item.ODONTOGRAMA_TIPO.DESCR) ? item.ODONTOGRAMA_TIPO.DESCR.Trim() : string.Empty : string.Empty,
                    //                    item.ODONTOGRAMA != null ? !string.IsNullOrEmpty(item.ODONTOGRAMA.DESCR) ? item.ODONTOGRAMA.DESCR.Trim() : string.Empty : string.Empty,
                    //                    item.ODONTOGRAMA != null ? item.ODONTOGRAMA.ID_POSICION.ToString() : string.Empty),
                    //                ImagenDiente = _Imagen
                    //            });
                    //    };
                    //};

                    cEncabezado Encabezado = new cEncabezado();
                    Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                    Encabezado.TituloDos = Parametro.ENCABEZADO2;
                    Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                    Encabezado.NombreReporte = _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty;
                    Encabezado.ImagenFondo = Parametro.LOGO_BC_ACTA_COMUN;
                    Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                    #region Inicializacion de reporte
                    ReporteHCD.LocalReport.ReportPath = "Reportes/rHistoriaClinicaDental.rdlc";
                    ReporteHCD.LocalReport.DataSources.Clear();
                    #endregion

                    #region Definicion d origenes de datos
                    var ds1 = new List<cHistoriaClinicaDentalReporte>();
                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    ds1.Add(DatosReporte);
                    rds1.Name = "DataSet2";
                    rds1.Value = ds1;
                    ReporteHCD.LocalReport.DataSources.Add(rds1);

                    //datasource dos
                    var ds2 = new List<cEncabezado>();
                    ds2.Add(Encabezado);
                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet1";
                    rds2.Value = ds2;
                    ReporteHCD.LocalReport.DataSources.Add(rds2);

                    Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds3.Name = "DataSet3";
                    rds3.Value = lstDetalleDientes;
                    ReporteHCD.LocalReport.DataSources.Add(rds3);

                    Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds4.Name = "DataSet4";
                    rds4.Value = lstSeguimientos;
                    ReporteHCD.LocalReport.DataSources.Add(rds4);
                    #endregion

                    ReporteHCD.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    ReporteHCD.RefreshReport();
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void ImprimeReporteHojaControlLiquidos()
        {
            try
            {
                ReportesView View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Show();

                var _validacionHospitalizado = new SSP.Controlador.Catalogo.Justicia.Medico.cHospitalizacion().ObtenerHospitalizacionesPorIngreso(SelectedIngreso);
                var _UnicaH = _validacionHospitalizado != null ? _validacionHospitalizado.Any() ? _validacionHospitalizado.FirstOrDefault(x => x.ID_HOSPITA == SeletedHojasLiquidos.ID_HOSPITA) : null : null;
                string Enfermedades = string.Empty;
                string Dietas = string.Empty;

                var _Enfermedades = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD : null : null;
                if (_Enfermedades != null && _Enfermedades.Any())
                    foreach (var item in _Enfermedades)
                        Enfermedades += string.Format("{0}, ", item.ENFERMEDAD != null ? !string.IsNullOrEmpty(item.ENFERMEDAD.NOMBRE) ? item.ENFERMEDAD.NOMBRE.Trim() : string.Empty : string.Empty);

                var _Dietas = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.NOTA_MEDICA_DIETA : null : null;
                if (_Dietas != null && _Dietas.Any())
                    foreach (var item in _Dietas)
                        Dietas += string.Format("{0}, ", item.DIETA != null ? !string.IsNullOrEmpty(item.DIETA.DESCR) ? item.DIETA.DESCR.Trim() : string.Empty : string.Empty);

                var datos = new System.Collections.Generic.List<cHojaControlLiquidosReporte>();
                var _detallesEnfermo = new cHojaControlLiquidosEncabezadoReporte()
                {
                    Nombre = string.Format("{0} {1} {2}",
                    SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.NOMBRE) ? SelectedIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                    SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.PATERNO) ? SelectedIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                    SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? SelectedIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty),
                    Sexo = SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.SEXO) ? SelectedIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : SelectedIngreso.IMPUTADO.SEXO == "F" ? "FEMENINO" : string.Empty : string.Empty : string.Empty,
                    Edad = SelectedIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectedIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty,
                    Fecha = FechaInicioBusqueda != null ? FechaInicioBusqueda.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Cama = _UnicaH != null ? _UnicaH.CAMA_HOSPITAL != null ? !string.IsNullOrEmpty(_UnicaH.CAMA_HOSPITAL.DESCR) ? _UnicaH.CAMA_HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                    Peso = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Talla = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TALLA) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TALLA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Diagnostico = !string.IsNullOrEmpty(Enfermedades) ? Enfermedades.TrimEnd(',') : string.Empty,
                    Dieta = !string.IsNullOrEmpty(Dietas) ? Dietas.TrimEnd(',') : string.Empty,
                    Exped = string.Format("{0} / {1}", SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO),
                    Glucemia = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.GLUCEMIA) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.GLUCEMIA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty
                };

                var _LiquidosIngreso = new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "1" && x.ID_LIQ_FORMATO == 1);
                var _LiquidosEreso = new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "2" && x.ID_LIQ_FORMATO == 1);
                string _detalleLiquidosIngreso = "INGRESOS \n";
                string _detalleLiquidosEgreso = "EGRESOS \n";

                if (_LiquidosIngreso.Any())
                    foreach (var item in _LiquidosIngreso)
                        _detalleLiquidosIngreso += string.Format("   {0}\n", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty);

                if (_LiquidosEreso.Any())
                    foreach (var item in _LiquidosEreso)
                        _detalleLiquidosEgreso += string.Format("   {0}\n", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty);

                var _HorasDisponibles = new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S", y => y.ID_LIQHORA.ToString());
                var _DetallesHojaLiquidos = new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrl().ObtenerHojaLiquidos(SeletedHojasLiquidos.ID_HOSPITA, SeletedHojasLiquidos.FECHA);
                if (_HorasDisponibles.Any())
                {
                    foreach (var item in _HorasDisponibles)
                    {
                        string CondensadoIngreso = "\n";
                        string CondensadoEgreso = "\n";
                        SSP.Servidor.LIQUIDO_HOJA_CTRL _HojaCapturada = _DetallesHojaLiquidos.Any() ? _DetallesHojaLiquidos.FirstOrDefault(x => x.ID_LIQHORA == item.ID_LIQHORA) : null;
                        if (_LiquidosIngreso.Any())
                        {
                            foreach (var itemX in _LiquidosIngreso)
                            {
                                if (_HojaCapturada != null)
                                    if (_HojaCapturada.LIQUIDO_HOJA_CTRL_DETALLE.Any())
                                    {
                                        var _elegida = _HojaCapturada.LIQUIDO_HOJA_CTRL_DETALLE.FirstOrDefault(x => x.ID_LIQ == itemX.ID_LIQ);
                                        if (_elegida != null)
                                            CondensadoIngreso += string.Format("{0} \n", _elegida.CANT);
                                        else
                                            CondensadoIngreso += "*\n";
                                    }
                                    else
                                        CondensadoIngreso += "*\n";
                                else
                                    CondensadoIngreso += "*\n";
                            };
                        };

                        if (_LiquidosEreso.Any())
                        {
                            foreach (var itemX in _LiquidosEreso)
                            {
                                if (_HojaCapturada != null)
                                    if (_HojaCapturada.LIQUIDO_HOJA_CTRL_DETALLE.Any())
                                    {
                                        var _elegida = _HojaCapturada.LIQUIDO_HOJA_CTRL_DETALLE.FirstOrDefault(x => x.ID_LIQ == itemX.ID_LIQ);
                                        if (_elegida != null)
                                            CondensadoEgreso += string.Format("{0} \n", _elegida.CANT);
                                        else
                                            CondensadoEgreso += "*\n";
                                    }
                                    else
                                        CondensadoEgreso += "*\n";
                                else
                                    CondensadoEgreso += "*\n";
                            };
                        };

                        _detalleLiquidosIngreso = !string.IsNullOrEmpty(_detalleLiquidosIngreso) ? _detalleLiquidosIngreso.TrimEnd('\n') : string.Empty;
                        _detalleLiquidosEgreso = !string.IsNullOrEmpty(_detalleLiquidosEgreso) ? _detalleLiquidosEgreso.TrimEnd('\n') : string.Empty;
                        CondensadoIngreso = !string.IsNullOrEmpty(CondensadoIngreso) ? CondensadoIngreso.TrimEnd('\n') : string.Empty;
                        CondensadoEgreso = !string.IsNullOrEmpty(CondensadoEgreso) ? CondensadoEgreso.TrimEnd('\n') : string.Empty;

                        datos.Add(new cHojaControlLiquidosReporte
                        {
                            Hora = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                            Ingresos = _detalleLiquidosIngreso,
                            Egresos = _detalleLiquidosEgreso,
                            FrecCard = "FREC. CARD.",
                            FrecResp = "FREC. RESP.",
                            Temp = "TEMP.",
                            TensionArt = "TENS. ART.",
                            Generico = _HojaCapturada != null ? !string.IsNullOrEmpty(_HojaCapturada.TENSION_ARTERIAL) ? _HojaCapturada.TENSION_ARTERIAL.Trim() : string.Empty : string.Empty,
                            Generico1 = _HojaCapturada != null ? !string.IsNullOrEmpty(_HojaCapturada.FRECUENCIA_CARDIACA) ? _HojaCapturada.FRECUENCIA_CARDIACA.Trim() : string.Empty : string.Empty,
                            Generico2 = _HojaCapturada != null ? !string.IsNullOrEmpty(_HojaCapturada.FRECUENCIA_RESPIRATORIA) ? _HojaCapturada.FRECUENCIA_RESPIRATORIA.Trim() : string.Empty : string.Empty,
                            Generico3 = _HojaCapturada != null ? !string.IsNullOrEmpty(_HojaCapturada.TEMPERATURA) ? _HojaCapturada.TEMPERATURA.Trim() : string.Empty : string.Empty,
                            Generico4 = CondensadoIngreso,
                            Generico5 = CondensadoEgreso
                        });
                    };
                };

                cControlLiquidosConcentrados Concentrados = new cControlLiquidosConcentrados();
                var _Concentrados = new SSP.Controlador.Catalogo.Justicia.cLiquidoHojaCtrlConcen().GetData(x => x.ID_HOSPITA == SeletedHojasLiquidos.ID_HOSPITA && x.ID_CENTRO_UBI == GlobalVar.gCentro);
                if (_Concentrados.Any())
                {
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _ConcentradoMatutino = _Concentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.MATUTINO);
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _ConcentradoVespertino = _Concentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.VESPERTINO);
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _ConcentradoNocturno = _Concentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.NOCTURNO);
                    SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN _ConcentradoTotal = _Concentrados.FirstOrDefault(x => x.ID_CONCENTIPO == (decimal)eTipoConcentrados.TOTAL);
                    SSP.Servidor.USUARIO _Usuario = null;
                    if (_ConcentradoMatutino != null)
                    {
                        _Usuario = null;//SE ASEGURA DE LIMPIAR EL USUARIO PARA QUE NO SE TOME EL MISMO O SE SOBREESCRIBA
                        if (!string.IsNullOrEmpty(_ConcentradoMatutino.REGISTRO_USUARIO))
                            _Usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _ConcentradoMatutino.REGISTRO_USUARIO.Trim()).FirstOrDefault();

                        Concentrados.BalanceTM = _ConcentradoMatutino.BALANCE;
                        Concentrados.EnfermeroTM = _Usuario != null ? string.Format("{0} {1} {2}",
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.NOMBRE) ? _Usuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.PATERNO) ? _Usuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.MATERNO) ? _Usuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                        Concentrados.EntradaTM = _ConcentradoMatutino.ENTRADA;
                        Concentrados.SalidasTM = _ConcentradoMatutino.SALIDA;
                    };


                    if (_ConcentradoVespertino != null)
                    {
                        _Usuario = null;
                        if (!string.IsNullOrEmpty(_ConcentradoVespertino.REGISTRO_USUARIO))
                            _Usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _ConcentradoVespertino.REGISTRO_USUARIO.Trim()).FirstOrDefault();

                        Concentrados.BalanceTV = _ConcentradoVespertino.BALANCE;
                        Concentrados.EnfermeroTV = _Usuario != null ? string.Format("{0} {1} {2}",
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.NOMBRE) ? _Usuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.PATERNO) ? _Usuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.MATERNO) ? _Usuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                        Concentrados.EntradaTV = _ConcentradoVespertino.ENTRADA;
                        Concentrados.SalidasTV = _ConcentradoVespertino.SALIDA;
                    };


                    if (_ConcentradoNocturno != null)
                    {
                        _Usuario = null;
                        if (!string.IsNullOrEmpty(_ConcentradoNocturno.REGISTRO_USUARIO))
                            _Usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _ConcentradoNocturno.REGISTRO_USUARIO.Trim()).FirstOrDefault();

                        Concentrados.BalanceTN = _ConcentradoNocturno.BALANCE;
                        Concentrados.EnfermeroTN = _Usuario != null ? string.Format("{0} {1} {2}",
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.NOMBRE) ? _Usuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.PATERNO) ? _Usuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.MATERNO) ? _Usuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                        Concentrados.EntradaTN = _ConcentradoNocturno.ENTRADA;
                        Concentrados.SalidasTN = _ConcentradoNocturno.SALIDA;
                    };

                    if (_ConcentradoTotal != null)
                    {
                        _Usuario = null;
                        if (!string.IsNullOrEmpty(_ConcentradoTotal.REGISTRO_USUARIO))
                            _Usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == _ConcentradoTotal.REGISTRO_USUARIO.Trim()).FirstOrDefault();

                        Concentrados.BalanceTTL = _ConcentradoTotal.BALANCE;
                        Concentrados.EnfermeroTTL = _Usuario != null ? string.Format("{0} {1} {2}",
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.NOMBRE) ? _Usuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.PATERNO) ? _Usuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            _Usuario.EMPLEADO != null ? _Usuario.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_Usuario.EMPLEADO.PERSONA.MATERNO) ? _Usuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;
                        Concentrados.EntradasTTL = _ConcentradoTotal.ENTRADA;
                        Concentrados.SalidasTTL = _ConcentradoTotal.SALIDA;
                    };
                }

                var Encabezado = new cEncabezado();
                Encabezado = new cEncabezado()
                {
                    ImagenDerecha = Parametro.LOGO_ESTADO,
                    ImagenIzquierda = Parametro.REPORTE_LOGO2,
                    TituloDos = Parametro.ENCABEZADO2
                };

                View.Report.LocalReport.ReportPath = "Reportes/rReporteHojaControlLiquidos.rdlc";
                View.Report.LocalReport.DataSources.Clear();

                System.Collections.Generic.List<cEncabezado> ds1 = new System.Collections.Generic.List<cEncabezado>();
                ds1.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "DataSet1", Value = ds1 };
                View.Report.LocalReport.DataSources.Add(rds1);

                var ds2 = new System.Collections.Generic.List<cHojaControlLiquidosEncabezadoReporte>();
                ds2.Add(_detallesEnfermo);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "DataSet2", Value = ds2 };
                View.Report.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "DataSet3", Value = datos };
                View.Report.LocalReport.DataSources.Add(rds3);

                var ds4 = new System.Collections.Generic.List<cControlLiquidosConcentrados>();
                ds4.Add(Concentrados);
                Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "DataSet4", Value = ds4 };
                View.Report.LocalReport.DataSources.Add(rds4);

                View.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                View.Report.RefreshReport();
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        #region Documento

        private void ProcesaDefuncion()
        {
            try
            {
                ListaDecesoBusqueda = new ObservableCollection<NOTA_DEFUNCION>(new cNota_Defuncion().Buscar(GlobalVar.gCentro, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.PATERNO : string.Empty, SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.MATERNO : string.Empty, SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.NOMBRE : string.Empty, new System.DateTime?(), new System.DateTime?()));

                if (ListaDecesoBusqueda != null && ListaDecesoBusqueda.Any())
                {
                    SelectedDecesoBusqueda = ListaDecesoBusqueda.FirstOrDefault();
                    var tc = new TextControlView();
                    tc.Closed += (s, e) =>
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    };

                    tc.editor.Loaded += (s, e) =>
                    {
                        //DOCX
                        tc.editor.EditMode = TXTextControl.EditMode.ReadOnly;
                        TXTextControl.LoadSettings _settings = new TXTextControl.LoadSettings();
                        tc.editor.Load(SelectedDecesoBusqueda.TARJETA_DECESO, TXTextControl.BinaryStreamType.WordprocessingML, _settings);
                    };
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Owner = PopUpsViewModels.MainWindow;
                    tc.Show();
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no cuenta con una hoja de defunción.");
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private byte[] CrearTarjetaInformativaDeceso(NOTA_DEFUNCION _nota_defuncion, INGRESO _ingreso)
        {
            MemoryStream stream = new MemoryStream();
            using (DocX document = DocX.Create(stream))
            {
                var font_documento = new System.Drawing.FontFamily("Arial");
                const double font_size_body = 10D;
                const double font_size_ccp = 7D;
                const float indentation_left = 1.5F;
                const float indentation_right = 1.5F;
                #region Configuracion del Documento
                document.MarginLeft = 40;
                document.MarginRight = 40;
                document.AddHeaders();
                #endregion
                #region Header
                Header header_default = document.Headers.odd;
                // Insert a Paragraph into the default Header.
                Novacode.Table th = header_default.InsertTable(1, 3);
                th.Design = TableDesign.None;
                float[] _witdhs = { 100, 500, 100 };
                th.SetWidths(_witdhs);
                MemoryStream ms_imagen = new MemoryStream(Parametro.REPORTE_LOGO2);
                Novacode.Image _logo2 = document.AddImage(ms_imagen);
                var pic1 = _logo2.CreatePicture();
                pic1.Width = 97;
                pic1.Height = 97;
                th.Rows[0].Cells[0].Paragraphs.First().InsertPicture(pic1);
                Novacode.Paragraph p1 = th.Rows[0].Cells[1].Paragraphs.First();
                p1.Alignment = Alignment.center;
                p1.AppendLine("Secretaria de Seguridad Pública").Bold().FontSize(11D).Font(font_documento);
                p1.AppendLine("Subsecretaria del Sistema Estatal Penitenciario").Bold().FontSize(11D).Font(font_documento);
                p1.AppendLine("Dirección de Programas de Reinserción Social").Bold().FontSize(11D).Font(font_documento);
                p1.AppendLine(_ingreso.CENTRO != null ? !string.IsNullOrEmpty(_ingreso.CENTRO.DESCR) ? _ingreso.CENTRO.DESCR.Trim() : " " : " ").Bold().FontSize(11D).Font(font_documento);
                p1.AppendLine("TARJETA INFORMATIVA DE DECESOS").Bold().FontSize(11D).Font(font_documento);
                ms_imagen.Close();
                ms_imagen = new MemoryStream(Parametro.REPORTE_LOGO1);
                Novacode.Image _logo1 = document.AddImage(ms_imagen);
                p1 = th.Rows[0].Cells[2].Paragraphs.First();
                p1.Alignment = Alignment.right;
                p1.InsertPicture(_logo1.CreatePicture());
                ms_imagen.Close();
                #endregion
                Novacode.Paragraph pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.right;
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.AppendLine();
                pbody.InsertText("FECHA Y LUGAR: ", false, new Formatting()
                {
                    Size = 9D,
                    Bold = true,
                    FontFamily = font_documento
                });

                pbody.InsertText(string.Format("{0}, {1}, A {2}",
                    _ingreso.CENTRO != null ? _ingreso.CENTRO.MUNICIPIO != null ? !string.IsNullOrEmpty(_ingreso.CENTRO.MUNICIPIO.MUNICIPIO1) ? _ingreso.CENTRO.MUNICIPIO.MUNICIPIO1.Trim() : " " : " " : " ",
                    _ingreso.CENTRO != null ? _ingreso.CENTRO.MUNICIPIO != null ? _ingreso.CENTRO.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(_ingreso.CENTRO.MUNICIPIO.ENTIDAD.DESCR) ? _ingreso.CENTRO.MUNICIPIO.ENTIDAD.DESCR.Trim() : " " : " " : " " : " ",
                    Fechas.fechaLetra(Fechas.GetFechaDateServer, false).ToUpper()), false, new Formatting()
                    {
                        Size = 9D,
                        Bold = true,
                        FontFamily = font_documento
                    });

                pbody.AppendLine();
                pbody = document.InsertParagraph();
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.AppendLine(_ingreso.CENTRO != null ? !string.IsNullOrEmpty(_ingreso.CENTRO.DIRECTOR) ? _ingreso.CENTRO.DIRECTOR.Trim() : " " : " ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.AppendLine("DIRECTOR DEL CENTRO DE REINSERCIÓN SOCIAL").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.AppendLine("BC. SECRETARIA DE SEGURIDAD PÚBLICA").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.AppendLine("P R E S E N T E.").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.AppendLine();
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.left;
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Append("NOMBRE DEL INTERNO: ").Bold().UnderlineStyle(UnderlineStyle.singleLine).FontSize(font_size_body).Font(font_documento);
                System.Text.StringBuilder _strbuilder = new System.Text.StringBuilder();

                if (!string.IsNullOrWhiteSpace(_ingreso.IMPUTADO.NOMBRE))
                    _strbuilder.Append(_ingreso.IMPUTADO.NOMBRE.Trim());
                if (!string.IsNullOrWhiteSpace(_ingreso.IMPUTADO.PATERNO))
                    _strbuilder.Append(" ").Append(_ingreso.IMPUTADO.PATERNO.Trim());
                if (!string.IsNullOrWhiteSpace(_ingreso.IMPUTADO.MATERNO))
                    _strbuilder.Append(" ").Append(_ingreso.IMPUTADO.MATERNO.Trim());

                pbody.Append(_strbuilder.ToString()).FontSize(font_size_body).Font(font_documento);
                _strbuilder.Clear();

                if (_ingreso.IMPUTADO != null)
                    if (_ingreso.IMPUTADO.ALIAS != null && _ingreso.IMPUTADO.ALIAS.Any())
                        foreach (var item in _ingreso.IMPUTADO.ALIAS)
                        {
                            if (_strbuilder.Length != 0)
                                _strbuilder.Append(", ");
                            _strbuilder.Append(item.NOMBRE.Trim());
                            if (!string.IsNullOrWhiteSpace(item.PATERNO))
                                _strbuilder.Append(" ").Append(item.PATERNO.Trim());
                            if (!string.IsNullOrWhiteSpace(item.MATERNO))
                                _strbuilder.Append(" ").Append(item.MATERNO.Trim());
                        }

                if (_ingreso.IMPUTADO != null)
                    if (_ingreso.IMPUTADO.APODO != null && _ingreso.IMPUTADO.APODO.Any())
                        foreach (var item in _ingreso.IMPUTADO.APODO)
                        {
                            if (_strbuilder.Length != 0)
                                _strbuilder.Append(", ");
                            _strbuilder.Append(item.APODO1.Trim());
                        }

                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.both;
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Append("ALIAS: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(_strbuilder.ToString()).FontSize(font_size_body).Font(font_documento);
                int contador = 1;

                lstCausasPenales = new ObservableCollection<EXT_DELITOS>(SelectIngreso.CAUSA_PENAL.Select(s => new EXT_DELITOS
                {
                    CP_ANIO = s.CP_ANIO.HasValue ? s.CP_ANIO.ToString() : string.Empty,
                    CP_FOLIO = s.CP_ANIO.HasValue ? s.CP_ANIO.ToString() : string.Empty,
                    CP_BIS = s.CP_BIS,
                    DELITOS = s.CAUSA_PENAL_DELITO.ToList()
                }));

                foreach (var item in lstCausasPenales)
                {
                    pbody = document.InsertParagraph();
                    pbody.IndentationBefore = indentation_left;
                    pbody.IndentationAfter = indentation_right;
                    pbody.Append(string.Format("{0}.- DELITO: ", contador.ToString())).Bold().FontSize(8D).Font(font_documento);
                    pbody.Append(item.DELITOS_DESCR).FontSize(8D).Font(font_documento);
                    pbody = document.InsertParagraph();
                    pbody.IndentationBefore = indentation_left;
                    pbody.IndentationAfter = indentation_right;
                    pbody.Append("CAUSA PENAL: ").Bold().FontSize(8D).Font(font_documento);
                    pbody.Append(item.CAUSA_PENAL).FontSize(8D).Font(font_documento);
                    contador += 1;
                }

                Novacode.Table tbody = document.InsertTable(1, 2);
                tbody.Design = TableDesign.None;
                tbody.Alignment = Alignment.center;
                tbody.SetWidths(new float[] { 414, 200 });
                var doc_cell = tbody.Rows[0].Cells[0];
                doc_cell.MarginRight = 0;
                doc_cell.MarginRight = 0;
                pbody = doc_cell.Paragraphs.First();
                pbody.Alignment = Alignment.left;
                pbody.Append("INGRESO AL CERESO: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(Fechas.fechaLetra(_ingreso.FEC_INGRESO_CERESO.Value, false).ToUpper()).FontSize(font_size_body).Font(font_documento);
                pbody = tbody.Rows[0].Cells[1].Paragraphs.First();
                pbody.Alignment = Alignment.left;
                pbody.Append("A LAS: ").Bold().FontSize(font_size_body).Font(font_documento); ;
                pbody.Append(_ingreso.FEC_INGRESO_CERESO.Value.TimeOfDay.ToString(@"hh\:mm\:ss").ToUpper()).FontSize(font_size_body).Font(font_documento);
                pbody.Append(" HRS").FontSize(font_size_body).Font(font_documento);
                tbody = document.InsertTable(1, 3);
                tbody.Alignment = Alignment.center;
                tbody.Design = TableDesign.None;
                tbody.SetWidths(new float[] { 110, 130, 374 });
                doc_cell = tbody.Rows[0].Cells[0];
                doc_cell.MarginRight = 0;
                doc_cell.MarginRight = 0;
                pbody = doc_cell.Paragraphs.First();
                pbody.Alignment = Alignment.left;
                pbody.Append("EDAD: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(new Fechas().CalculaEdad(_ingreso.IMPUTADO.NACIMIENTO_FECHA.Value).ToString()).FontSize(font_size_body).Font(font_documento);
                pbody.Append(" AÑOS").FontSize(font_size_body).Font(font_documento);
                doc_cell = tbody.Rows[0].Cells[1];
                doc_cell.MarginRight = 0;
                doc_cell.MarginRight = 0;
                pbody = doc_cell.Paragraphs.First();
                pbody.Alignment = Alignment.left;
                pbody.Append("SEXO: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(_ingreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO").FontSize(font_size_body).Font(font_documento);
                doc_cell = tbody.Rows[0].Cells[2];
                doc_cell.MarginRight = 0;
                doc_cell.MarginRight = 0;
                pbody = doc_cell.Paragraphs.First();
                pbody.Alignment = Alignment.left;
                pbody.Append("ORIGINARIO: ").Bold().FontSize(font_size_body).Font(font_documento);
                _strbuilder.Clear();
                _strbuilder.Append(_ingreso.IMPUTADO.MUNICIPIO.MUNICIPIO1.Trim()).Append(", ").Append(_ingreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR.Trim()).Append(", ")
                    .Append(_ingreso.IMPUTADO.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.PAIS.Trim());
                pbody.Append(_strbuilder.ToString()).FontSize(font_size_body).Font(font_documento);
                pbody = document.InsertParagraph();
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Alignment = Alignment.left;
                pbody.Append("VISITA FAMILIAR: ").Bold().FontSize(font_size_body).Font(font_documento);
                var _ultima_visita = _ingreso.ADUANA_INGRESO.Where(w => w.ADUANA.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_VISITA).OrderByDescending(o => o.ID_ADUANA).FirstOrDefault();
                if (_ultima_visita != null)
                {
                    _strbuilder.Clear();
                    _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.NOMBRE.Trim());
                    if (!string.IsNullOrWhiteSpace(_ultima_visita.ADUANA.PERSONA.PATERNO))
                    {
                        _strbuilder.Append(" ");
                        _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.PATERNO.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(_ultima_visita.ADUANA.PERSONA.MATERNO))
                    {
                        _strbuilder.Append(" ");
                        _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.MATERNO.Trim());
                    }
                    pbody = document.InsertParagraph();
                    pbody.IndentationBefore = indentation_left;
                    pbody.IndentationAfter = indentation_right;
                    pbody.Alignment = Alignment.left;
                    pbody.Append(_strbuilder.ToString()).Bold().FontSize(font_size_body).Font(font_documento);
                    _strbuilder.Clear();
                    _strbuilder.Append(" (");
                    _strbuilder.Append(_ultima_visita.ADUANA.PERSONA.VISITANTE.VISITANTE_INGRESO.FirstOrDefault(w => w.ID_ANIO == _ingreso.ID_ANIO && w.ID_CENTRO == _ingreso.ID_CENTRO
                        && w.ID_IMPUTADO == _ingreso.ID_IMPUTADO && w.ID_INGRESO == _ingreso.ID_INGRESO).TIPO_REFERENCIA.DESCR.Trim());
                    _strbuilder.Append(")");
                    pbody.Append(_strbuilder.ToString()).FontSize(font_size_body).Font(font_documento);
                    pbody.Append(" ÚLTIMA VISITA: ").Bold().FontSize(font_size_body).Font(font_documento);
                    pbody.Append("EL ").FontSize(font_size_body).Font(font_documento);
                    pbody.Append(Fechas.fechaLetra(_ultima_visita.ENTRADA_FEC.Value)).FontSize(font_size_body).Font(font_documento);
                }

                pbody = document.InsertParagraph();
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Alignment = Alignment.left;
                pbody.Append("CERTIFICADO MÉDICO DE NUEVO INGRESO REPORTA: ").Bold().FontSize(font_size_body).Font(font_documento);
                _strbuilder.Clear();

                if (_ingreso.ATENCION_MEDICA.Any(w => w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO))
                    foreach (var item in _ingreso.ATENCION_MEDICA.First(w => w.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO).NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD)
                    {
                        if (_strbuilder.Length > 0)
                            _strbuilder.Append(", ");
                        _strbuilder.Append(item.ENFERMEDAD.NOMBRE.Trim());
                    }

                if (_strbuilder.Length > 0)
                {
                    pbody.Append(_strbuilder.ToString()).FontSize(font_size_body).Font(font_documento);
                }

                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.left;
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Append("FECHA Y LUGAR DEL DECESO: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(Fechas.fechaLetra(_nota_defuncion.FECHA_DECESO.Value, false).ToUpper()).FontSize(font_size_body).Font(font_documento);
                pbody.Append(" EN ").FontSize(font_size_body).Font(font_documento);
                pbody.Append(_nota_defuncion.LUGAR).FontSize(font_size_body).Font(font_documento);
                pbody = document.InsertParagraph();
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.Alignment = Alignment.left;
                pbody.Append("CAUSA APARENTE DEL DECESO: ").Bold().FontSize(font_size_body).Font(font_documento);
                pbody.Append(_nota_defuncion.ENFERMEDAD.NOMBRE).FontSize(font_size_body).Font(font_documento);
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.center;
                pbody.AppendLine();
                pbody.AppendLine();
                pbody.AppendLine();
                pbody.AppendLine("HECHOS:").Bold().FontSize(font_size_body).Font(font_documento);
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.both;
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                pbody.AppendLine(_nota_defuncion.HECHOS).FontSize(font_size_body).Font(font_documento);
                document.InsertParagraph();
                document.InsertParagraph();
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.center;
                pbody.Append("ATENTAMENTE").Bold().FontSize(11D).Font(font_documento);
                document.InsertParagraph();
                document.InsertParagraph();
                pbody = document.InsertParagraph();
                pbody.Alignment = Alignment.center;
                var _coordinador = new cPersona().Obtener(_nota_defuncion.ID_EMPLEADO_COORDINADOR_MED.Value).FirstOrDefault();
                _strbuilder.Clear();
                _strbuilder.Append(_coordinador.NOMBRE.Trim());
                if (!string.IsNullOrWhiteSpace(_coordinador.PATERNO))
                    _strbuilder.Append(" ").Append(_coordinador.PATERNO.Trim());
                if (!string.IsNullOrWhiteSpace(_coordinador.MATERNO))
                    _strbuilder.Append(" ").Append(_coordinador.MATERNO.Trim());
                pbody.AppendLine(string.Format("DR. {0}", _strbuilder.ToString())).Bold().FontSize(11D).Font(font_documento);
                pbody.AppendLine("COORDINADOR MEDICO").Bold().FontSize(11D).Font(font_documento);
                pbody.AppendLine(_ingreso.CENTRO.DESCR.Trim()).Bold().FontSize(11D).Font(font_documento);
                var cedula = !string.IsNullOrWhiteSpace(_coordinador.EMPLEADO.CEDULA) ? _coordinador.EMPLEADO.CEDULA : string.Empty;
                pbody.AppendLine(string.Format("CED. PROF. {0}", cedula)).Bold().FontSize(11D).Font(font_documento);
                document.InsertParagraph();
                pbody = document.InsertParagraph();
                pbody.IndentationBefore = indentation_left;
                pbody.IndentationAfter = indentation_right;
                var algo = pbody.AppendLine(string.Format("C.C.P.- {0}.- DIRECTOR DE {1}. PARA SU SUPERIOR CONOCIMIENTO.", _ingreso.CENTRO.DIRECTOR.Trim(), _ingreso.CENTRO.DESCR.Trim())).FontSize(font_size_ccp).Font(font_documento);
                pbody.AppendLine(string.Format("C.C.P.- {0}.- DIRECTOR DE PROGRAMAS DE REINSERCIÓN SOCIAL. PARA SU SUPERIOR CONOCIMIENTO.", Parametro.DIR_PROGRAMAS_ESTATAL.Trim())).FontSize(font_size_ccp).Font(font_documento);
                pbody.AppendLine(string.Format("C.C.P.- {0}.- JEFE DE PROGRAMAS DE REINSERCIÓN. PARA SU CONOCIMIENTO", Parametro.JEFE_PROGRAMAS_ESTATAL.Trim())).FontSize(font_size_ccp).Font(font_documento);
                pbody.AppendLine(string.Format("C.C.P.- {0}.- COORDINADOR MÉDICO ESTATAL. PARA SU CONOCIMIENTO", Parametro.JEFE_PROGRAMAS_ESTATAL.Trim())).FontSize(font_size_ccp).Font(font_documento);
                var departamento_acceso = new cDepartamentosAcceso().ObtenerCoordinadorPorCentro((short)enumDepartamentos.COORDINACION_TECNICA, _ingreso.ID_UB_CENTRO.Value).FirstOrDefault();
                _strbuilder.Clear();
                _strbuilder.Append(departamento_acceso.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim());
                if (!string.IsNullOrWhiteSpace(departamento_acceso.USUARIO.EMPLEADO.PERSONA.PATERNO))
                    _strbuilder.Append(" ").Append(departamento_acceso.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim());
                if (!string.IsNullOrWhiteSpace(departamento_acceso.USUARIO.EMPLEADO.PERSONA.MATERNO))
                    _strbuilder.Append(" ").Append(departamento_acceso.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim());
                pbody.AppendLine(string.Format("C.C.P.- {0}.- COORDINADOR DE ÁREAS TÉCNICAS DEL {1}. PARA SU CONOCIMIENTO", _strbuilder.ToString(), _ingreso.CENTRO.DESCR.Trim())).FontSize(font_size_ccp).Font(font_documento);
                pbody.AppendLine("C.C.P.- ARCHIVO").FontSize(font_size_ccp).Font(font_documento);
                document.Save();
            }

            var _bytes = stream.ToArray();
            stream.Close();
            return _bytes;
        }
        #endregion



        private void CargarReporteCertificado()
        {
            try
            {
                if (SelectNotaMedica == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una nota medica para cargar el reporte.");
                    return;
                }

                #region Iniciliza el entorno para mostrar el reporte al usuario
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                ReportesView View = new ReportesView();
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Show();
                #endregion


                SelectNotaMedicaAuxiliar = new cAtencionMedica().GetData().First(g => g.ID_ATENCION_MEDICA == SelectNotaMedica.ID_ATENCION_MEDICA);
                ListLesiones = SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.LESION != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.LESION.Any() ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.LESION.Select(s => new LesionesCustom { DESCR = s.DESCR, REGION = s.ANATOMIA_TOPOGRAFICA }).ToList() : new List<LesionesCustom>() : new List<LesionesCustom>() : new List<LesionesCustom>();

                var medico = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault().EMPLEADO;
                var hoy = Fechas.GetFechaDateServer;
                var hora = hoy.ToShortTimeString() + " hrs. Del dia ";
                var dia = hoy.Day + " de " + hoy.ToString("MMMM").ToUpper() + " del " + hoy.Year + ".";
                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                var lesiones = string.Empty;
                var i = 0;
                if (SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null)
                    if (SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.LESION != null && SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.LESION.Any())
                        foreach (var item in SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.LESION)
                        {
                            lesiones = lesiones + item != null ? !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty : string.Empty + " EN " + item.ANATOMIA_TOPOGRAFICA != null ? !string.IsNullOrEmpty(item.ANATOMIA_TOPOGRAFICA.DESCR) ? item.ANATOMIA_TOPOGRAFICA.DESCR.Trim() : string.Empty : string.Empty + (i == SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.LESION.Count() ? string.Empty : ", \n");
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

                var dientes = Convert.ToBase64String(new Imagenes().getImagen("miniDiente.png"));
                var _htmlDiente = "<html><table><tr><td>";
                var j = 0;
                for (j = 0; j < 16; j++)
                    _htmlDiente += "<image id='ImagenDiente' style='width:20px;height:50px;' src = \" data:image/png;base64, " + dientes + " alt=\"diente" + j + "\" /></td><td>";

                _htmlDiente += "</td></tr></table></html>";

                var odonto = new HistoriaClinicaViewModel(new Nullable<short>(), new Nullable<short>(), new Nullable<short>(), new Nullable<short>()).CaptureWebPageBytesP(_htmlDiente, 400, 100);

                #region OBJETO
                var _centro = SelectIngreso.ID_UB_CENTRO != null ? new cCentro().GetData(x => x.ID_CENTRO == SelectIngreso.ID_UB_CENTRO).FirstOrDefault() : null;
                rds1.Value = new List<cReporteCertificadoNuevoIngreso>()
                {
                    new cReporteCertificadoNuevoIngreso
                    { 
                        OdontogramaImagen = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? odonto : null, 
                        DENTAL = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? "S" : "N",
                        Titulo = SelectNotaMedicaAuxiliar.ATENCION_SERVICIO != null ? SelectNotaMedicaAuxiliar.ATENCION_SERVICIO.DESCR : string.Empty,
                        Centro = _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty,
                        Ciudad = SelectIngreso.CENTRO1 != null ? SelectIngreso.CENTRO1.MUNICIPIO != null ? (string.IsNullOrEmpty(SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1) ? string.Empty : SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1.Trim()) : string.Empty : string.Empty,
                        Director = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DIRECTOR) ? string.Empty : SelectIngreso.CENTRO1.DIRECTOR.Trim() : "",
                        Cedula = medico.CEDULA != null ? medico.CEDULA.Trim() : "",
                        Edad = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value : new DateTime()).ToString(),
                        FechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        Fecha = hora + dia,
                        Escolaridad = SelectIngreso.ESCOLARIDAD != null ? (string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? string.Empty : SelectIngreso.ESCOLARIDAD.DESCR.Trim()) : "",
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
                        NombreImputado = SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim()),
                        TipoDelito = string.IsNullOrEmpty(delitos) ? "N/A" : delitos,
                        NombreMedico = medico.PERSONA.NOMBRE.Trim() + " " + medico.PERSONA.PATERNO.Trim() + " " + (string.IsNullOrEmpty(medico.PERSONA.MATERNO) ? string.Empty : medico.PERSONA.MATERNO.Trim()),
                        Sexo = medico.PERSONA.SEXO == "M" ? "MASCULINO" : medico.PERSONA.SEXO == "F" ? "FEMENINO" : string.Empty,
                        SignosVitales_FC = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC : string.Empty : string.Empty,
                        SignosVitales_FR = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA : string.Empty : string.Empty,
                        SignosVitales_T = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TEMPERATURA : string.Empty : string.Empty,
                        SignosVitales_TA = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL : string.Empty : string.Empty,
                        Logo1 = Parametro.LOGO_ESTADO,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Dorso = Parametro.CUERPO_DORSO,
                        Frente = Parametro.CUERPO_FRENTE,
                        Check = Parametro.IMAGEN_ZONA_CORPORAL,
                        AntecedentesPatologicos = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.ANTECEDENTES_PATOLOGICOS : "" : "",
                        Interrogatorio = lesiones,
                        PadecimientoTratamiento = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PADECIMIENTO : "" : "",
                        Toxicomanias = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TOXICOMANIAS : "" : "",
                        AmeritaHospitalizacion = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.AMERITA_HOSPITALIZACION == "S" ? "SI" : "NO" : "NO" : "NO",
                        Diagnostico = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.DIAGNOSTICO : string.Empty : string.Empty,
                        DiasEnSanar = SelectNotaMedicaAuxiliar != null?SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TARDA_SANAR_15 == "S" ? "SI" : "NO" : "NO" : "NO",
                        Observaciones = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.OBSERVACIONES : string.Empty : string.Empty,
                        PeligraVida = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PELIGRA_VIDA == "S" ? "SI" : "NO" : "NO" : "NO",
                        PlanTerapeutico = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PLAN_TERAPEUTICO : string.Empty : string.Empty,

                        Seguimiento = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.ID_CITA_SEGUIMIENTO.HasValue ? "SI" : "NO" : "NO",
                        F_FRONTAL = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FRONTAL) : true,
                        D_ANTEBRAZO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_DERECHO) : true,
                        D_ANTEBRAZO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_IZQUIERDO) : true,
                        D_BRAZO_POSTERIOR_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_DERECHO) : true,
                        D_BRAZO_POSTERIOR_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_IZQUIERDO) : true,
                        D_CALCANEO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_DERECHO) : true,
                        D_CALCANEO_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_IZQUIERDA) : true,
                        D_CODO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_DERECHO) : true,
                        D_CODO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_IZQUIERDO) : true,
                        D_COSTILLAS_COSTADO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_DERECHO) : true,
                        D_COSTILLAS_COSTADO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_IZQUIERDO) : true,
                        D_DORSAL_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_DERECHA) : true,
                        D_DORSAL_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_IZQUIERDA) : true,
                        D_ESCAPULAR_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_DERECHA) : true,
                        D_ESCAPULAR_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_IZQUIERDA) : true,
                        D_FALANGES_POSTERIORES_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_DERECHA) : true,
                        D_FALANGES_POSTERIORES_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_IZQUIERDA) : true,
                        D_GLUTEA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_DERECHA) : true,
                        D_GLUTEA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_IZQUIERDA) : true,
                        D_LUMBAR_RENAL_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_LUMBAR_RENAL_DERECHA) : true,
                        D_LUMBAR_RENAL_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                        D_METATARZIANA_PLANTA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                        D_METATARZIANA_PLANTA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_IZQUIERDA) : true,
                        D_MUÑECA_POSTERIOR_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_DERECHA) : true,
                        D_MUÑECA_POSTERIOR_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_IZQUIERDA) : true,
                        D_MUSLO_POSTERIOR_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_DERECHO) : true,
                        D_MUSLO_POSTERIOR_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_IZQUIERDO) : true,
                        D_OCCIPITAL_NUCA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OCCIPITAL_NUCA) : true,
                        D_OREJA_POSTERIOR_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_DERECHA) : true,
                        D_OREJA_POSTERIOR_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_IZQUIERDA) : true,
                        D_PANTORRILLA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_DERECHA) : true,
                        D_PANTORRILLA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_IZQUIERDA) : true,
                        D_POPLITEA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_DERECHA) : true,
                        D_POPLITEA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_IZQUIERDA) : true,
                        D_POSTERIOR_CABEZA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CABEZA) : true,
                        D_POSTERIOR_CUELLO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CUELLO) : true,
                        D_POSTERIOR_ENTREPIERNA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_DERECHA) : true,
                        D_POSTERIOR_ENTREPIERNA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_IZQUIERDA) : true,
                        D_POSTERIOR_HOMBRO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_DERECHO) : true,
                        D_POSTERIOR_HOMBRO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_IZQUIERDO) : true,
                        D_SACRA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_SACRA) : true,
                        D_TOBILLO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_DERECHO) : true,
                        D_TOBILLO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_IZQUIERDO) : true,
                        D_TORACICA_DORSAL = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TORACICA_DORSAL) : true,
                        D_VERTEBRAL_CERVICAL = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_CERVICAL) : true,
                        D_VERTEBRAL_LUMBAR = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_LUMBAR) : true,
                        D_VERTEBRAL_TORACICA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_TORACICA) : true,
                        F_ANTEBRAZO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_DERECHO) : true,
                        F_ANTEBRAZO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_IZQUIERDO) : true,
                        F_ANTERIOR_CUELLO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTERIOR_CUELLO) : true,
                        F_MUÑECA_ANTERIOR_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_DERECHA) : true,
                        F_MUÑECA_ANTERIOR_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_IZQUIERDA) : true,
                        F_AXILA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_DERECHA) : true,
                        F_AXILA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_IZQUIERDA) : true,
                        F_BAJO_VIENTRE_HIPOGASTRIO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BAJO_VIENTRE_HIPOGASTRIO) : true,
                        F_BRAZO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_DERECHO) : true,
                        F_BRAZO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_IZQUIERDO) : true,
                        F_CARA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                        F_CARA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CARA_IZQUIERDA) : true,
                        F_CLAVICULAR_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                        F_CLAVICULAR_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_IZQUIERDA) : true,
                        F_CODO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_DERECHO) : true,
                        F_CODO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_IZQUIERDO) : true,
                        F_ENTREPIERNA_ANTERIOR_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_DERECHA) : true,
                        F_ENTREPIERNA_ANTERIOR_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_IZQUIERDA) : true,
                        F_EPIGASTRIO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_EPIGASTRIO) : true,
                        F_ESCROTO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESCROTO) : true,
                        F_ESPINILLA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_DERECHA) : true,
                        F_ESPINILLA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_IZQUIERDA) : true,
                        F_FALANGES_MANO_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_DERECHA) : true,
                        F_FALANGES_MANO_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_IZQUIERDA) : true,
                        F_FALANGES_PIE_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_DERECHO) : true,
                        F_FALANGES_PIE_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_IZQUIERDO) : true,
                        F_HIPOCONDRIA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_DERECHA) : true,
                        F_HIPOCONDRIA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_IZQUIERDA) : true,
                        F_HOMBRO_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_DERECHO) : true,
                        F_HOMBRO_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_IZQUIERDO) : true,
                        F_INGUINAL_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_DERECHA) : true,
                        F_INGUINAL_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_IZQUIERDA) : true,
                        F_LATERAL_CABEZA_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_DERECHO) : true,
                        F_LATERAL_CABEZA_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_IZQUIERDO) : true,
                        F_MANDIBULA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MANDIBULA) : true,
                        F_METACARPIANOS_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_DERECHA) : true,
                        F_METACARPIANOS_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_IZQUIERDA) : true,
                        F_METATARZIANA_DORSAL_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_DERECHO) : true,
                        F_METATARZIANA_DORSAL_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_IZQUIERDO) : true,
                        F_MUSLO_ANTERIOR_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_DERECHO) : true,
                        F_MUSLO_ANTERIOR_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_IZQUIERDO) : true,
                        F_NARIZ = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_NARIZ) : true,
                        F_ORBITAL_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_DERECHO) : true,
                        F_ORBITAL_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_IZQUIERDO) : true,
                        F_OREJA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_DERECHA) : true,
                        F_OREJA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_IZQUIERDA) : true,
                        F_PENE_VAGINA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PENE_VAGINA) : true,
                        F_PEZON_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_DERECHO) : true,
                        F_PEZON_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_IZQUIERDO) : true,
                        F_RODILLA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_DERECHA) : true,
                        F_RODILLA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_IZQUIERDA) : true,
                        F_TOBILLO_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_DERECHO) : true,
                        F_TOBILLO_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_IZQUIERDO) : true,
                        F_TORAX_CENTRAL = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_CENTRAL) : true,
                        F_TORAX_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_DERECHO) : true,
                        F_TORAX_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_IZQUIERDO) : true,
                        F_UMBILICAL_MESOGASTIO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_UMBILICAL_MESOGASTIO) : true,
                        F_VACIO_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_DERECHA) : true,
                        F_VACIO_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_IZQUIERDA) : true,
                        F_VERTICE_CABEZA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VERTICE_CABEZA) : true
                    }
                };
                #endregion

                #region DENTAL
                ListaOdontogramaSeguimiento = new List<cReporteOdontogramaSeguimiento>();
                foreach (var item in SelectNotaMedicaAuxiliar.ODONTOGRAMA_SEGUIMIENTO2)
                {
                    ListaOdontogramaSeguimiento.Add(new cReporteOdontogramaSeguimiento
                    {
                        DESCR = string.Format("{0} EN {1} DEL {2}. POSICIÓN {3}", item.DENTAL_TRATAMIENTO == null ? item.ENFERMEDAD == null ? "" : item.ENFERMEDAD.NOMBRE.Trim() : item.DENTAL_TRATAMIENTO.DESCR.Trim(),
                            item.ODONTOGRAMA_TIPO.DESCR.Trim(), item.ODONTOGRAMA.DESCR.Trim(), item.ID_POSICION.ToString()),
                        ID = item.ID_CONSECUTIVO,
                    });
                }
                #endregion

                View.Report.LocalReport.ReportPath = "Reportes/rCertificadoNuevoIngreso.rdlc";
                View.Report.LocalReport.DataSources.Clear();
                View.Report.LocalReport.DataSources.Add(rds1);
                View.Report.ShowExportButton = false;
                View.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                #region Subreporte
                #endregion
                View.Report.RefreshReport();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la informacion del reporte.", ex);
            }
        }

        private void CargarReporteProdecimientoMedico()
        {
            try
            {
                if (SelectNotaMedica == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una nota medica para cargar el reporte.");
                    return;
                }

                #region Iniciliza el entorno para mostrar el reporte al usuario
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                ReportesView View = new ReportesView();
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Show();
                #endregion

                SelectNotaMedicaAuxiliar = new cAtencionMedica().GetData().First(g => g.ID_ATENCION_MEDICA == SelectNotaMedica.ID_ATENCION_MEDICA);
                var medico = new SSP.Servidor.PERSONA();
                medico = null;
                if (SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.PERSONA != null : false)
                    medico = SelectNotaMedicaAuxiliar.NOTA_MEDICA.PERSONA;
                else if (SelectNotaMedicaAuxiliar.ATENCION_CITA.Any() ? SelectNotaMedicaAuxiliar.ATENCION_CITA.Any(a => a.PERSONA != null) : false)
                    medico = SelectNotaMedicaAuxiliar.ATENCION_CITA.First(f => f.PERSONA != null).PERSONA;
                var hoy = Fechas.GetFechaDateServer;
                var hora = hoy.ToShortTimeString() + " hrs. Del dia ";
                var dia = hoy.Day + " de " + hoy.ToString("MMMM").ToUpper() + " del " + hoy.Year + ".";
                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                var delitos = string.Empty;
                var i = 1;
                if (SelectIngreso.CAUSA_PENAL.Any(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO))
                {
                    foreach (var item in SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO)
                    {
                        delitos = delitos + item.DESCR_DELITO + (i == SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO.Count ? "" : ",");
                        i++;
                    }
                }
                var dientes = Convert.ToBase64String(new Imagenes().getImagen("miniDiente.png"));
                var _htmlDiente = "<html><table><tr><td>";
                var j = 0;
                for (j = 0; j < 16; j++)
                    _htmlDiente += "<image id='ImagenDiente' style='width:20px;height:50px;' src = \" data:image/png;base64, " + dientes + " alt=\"diente" + j + "\" /></td><td>";

                _htmlDiente += "</td></tr></table></html>";

                var odonto = new HistoriaClinicaViewModel(new Nullable<short>(), new Nullable<short>(), new Nullable<short>(), new Nullable<short>()).CaptureWebPageBytesP(_htmlDiente, 400, 100);

                #region PROC MED
                var procedimientosmedicos = new List<string>();
                var insumos = new List<string>();
                ListSubreporteInsumos = new List<cReporteInsumos>();
                i = 0;

                ListSubreporteProcedimientos = new List<cReporteProcsMeds>();
                if (SelectNotaMedicaAuxiliar.PROC_ATENCION_MEDICA.Any(a => a.PROC_ATENCION_MEDICA_PROG.Any(an => an.PROC_MEDICO_PROG_DET.Any())))
                    foreach (var item in SelectNotaMedicaAuxiliar.PROC_ATENCION_MEDICA.Where(a => a.PROC_ATENCION_MEDICA_PROG.Any(an => an.PROC_MEDICO_PROG_DET.Any())))
                        if (item.PROC_ATENCION_MEDICA_PROG.Any())
                        {
                            ListSubreporteProcedimientos.Add(new cReporteProcsMeds
                            {
                                ID_ATENCION_MEDICA = SelectNotaMedicaAuxiliar.ID_ATENCION_MEDICA,
                                ID_PROC_MED = item.ID_PROCMED,
                                ProcedimientoMedico = item.PROC_MED.DESCR,
                            });
                            foreach (var itm in item.PROC_ATENCION_MEDICA_PROG.SelectMany(s => s.PROC_MEDICO_PROG_DET))
                            {
                                ListSubreporteInsumos.Add(new cReporteInsumos
                                {
                                    Insumo = itm.CANTIDAD_UTILIZADA.HasValue ? itm.CANTIDAD_UTILIZADA.Value.ToString() : "0",
                                    Producto = itm.PRODUCTO.NOMBRE,
                                    ID_ATENCION_MEDICA = SelectNotaMedicaAuxiliar.ID_ATENCION_MEDICA,
                                    ID_PROC_MED = item.ID_PROCMED,
                                    Estatus = itm.PROC_ATENCION_MEDICA_PROG.REALIZACION_FEC.HasValue ? itm.PROC_ATENCION_MEDICA_PROG.REALIZACION_FEC.Value.ToString() : "N/A"
                                });
                            }
                        }

                if (SelectNotaMedicaAuxiliar.ATENCION_CITA.Any(a => a.PROC_ATENCION_MEDICA_PROG.Any(b => b.PROC_MEDICO_PROG_DET.Any())))
                {
                    foreach (var item in SelectNotaMedicaAuxiliar.ATENCION_CITA.SelectMany(s => s.PROC_ATENCION_MEDICA_PROG))
                    {
                        ListSubreporteProcedimientos.Add(new cReporteProcsMeds
                        {
                            ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                            ID_PROC_MED = item.ID_PROCMED,
                            ProcedimientoMedico = item.PROC_ATENCION_MEDICA.PROC_MED.DESCR,
                        });
                        foreach (var itm in item.PROC_MEDICO_PROG_DET)
                        {
                            ListSubreporteInsumos.Add(new cReporteInsumos
                            {
                                Insumo = itm.CANTIDAD_UTILIZADA.HasValue ? itm.CANTIDAD_UTILIZADA.Value.ToString() : string.Empty,
                                Producto = itm.PRODUCTO.NOMBRE,
                                ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                ID_PROC_MED = itm.ID_PROCMED,
                                Estatus = itm.PROC_ATENCION_MEDICA_PROG.REALIZACION_FEC.HasValue ? itm.PROC_ATENCION_MEDICA_PROG.REALIZACION_FEC.Value.ToString() : "N/A"
                            });
                        }
                    }
                }

                #endregion

                #region OBJETO
                rds1.Value = new List<cReporteCertificadoNuevoIngreso>()
                {
                    new cReporteCertificadoNuevoIngreso
                    { 
                        OdontogramaImagen = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? odonto : null,
                        DENTAL = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? "S" : "N",
                        ID_ATENCION_MEDICA = SelectNotaMedicaAuxiliar.ID_ATENCION_MEDICA,
                        Titulo = SelectNotaMedicaAuxiliar.ATENCION_SERVICIO != null ? SelectNotaMedicaAuxiliar.ATENCION_SERVICIO.DESCR : string.Empty,
                        Centro = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DESCR) ? string.Empty : SelectIngreso.CENTRO1.DESCR.Trim() : "",
                        Ciudad = SelectIngreso.CENTRO1 != null ? SelectIngreso.CENTRO1.MUNICIPIO != null ? (string.IsNullOrEmpty(SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1) ? string.Empty : SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1.Trim()) : string.Empty : string.Empty,
                        Director = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DIRECTOR) ? string.Empty : SelectIngreso.CENTRO1.DIRECTOR.Trim() : "",
                        Cedula = medico != null ? medico.EMPLEADO != null ? medico.EMPLEADO.CEDULA != null ? medico.EMPLEADO.CEDULA.Trim() : "N/A" : "N/A" : "N/A",
                        Edad = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value : new DateTime()).ToString(),
                        FechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        Fecha = hora + dia,
                        Escolaridad = SelectIngreso.ESCOLARIDAD != null ? (string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? string.Empty : SelectIngreso.ESCOLARIDAD.DESCR.Trim()) : "",
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
                        NombreImputado = SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim()),
                        TipoDelito = string.IsNullOrEmpty(delitos) ? "N/A" : delitos,
                        NombreMedico = medico != null ? (medico.NOMBRE.Trim() + " " + medico.PATERNO.Trim() + " " + (string.IsNullOrEmpty(medico.MATERNO) ? string.Empty : medico.MATERNO.Trim())) : string.Empty,
                        Sexo = medico != null ? medico.SEXO == "M" ? "MASCULINO" : medico.SEXO == "F" ? "FEMENINO" : string.Empty : string.Empty,
                        SignosVitales_FC = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC : string.Empty : string.Empty,
                        SignosVitales_FR = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA : string.Empty : string.Empty,
                        SignosVitales_T = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TEMPERATURA : string.Empty : string.Empty,
                        SignosVitales_TA = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL : string.Empty : string.Empty,
                        Logo1 = Parametro.LOGO_ESTADO,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Dorso = Parametro.CUERPO_DORSO,
                        Frente = Parametro.CUERPO_FRENTE,
                        Check = Parametro.IMAGEN_ZONA_CORPORAL,
                        AntecedentesPatologicos = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.ANTECEDENTES_PATOLOGICOS : "" : "",
                        PadecimientoTratamiento = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PADECIMIENTO : "" : "",
                        Toxicomanias = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TOXICOMANIAS : "" : "",
                        AmeritaHospitalizacion = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.AMERITA_HOSPITALIZACION == "S" ? "SI" : "NO" : "NO" : "NO",
                        Diagnostico = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.DIAGNOSTICO : string.Empty : string.Empty,
                        DiasEnSanar = SelectNotaMedicaAuxiliar != null?SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TARDA_SANAR_15 == "S" ? "SI" : "NO" : "NO" : "NO",
                        Observaciones = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.OBSERVACIONES : string.Empty : string.Empty,
                        PeligraVida = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PELIGRA_VIDA == "S" ? "SI" : "NO" : "NO" : "NO",
                        PlanTerapeutico = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PLAN_TERAPEUTICO : string.Empty : string.Empty,
                        Seguimiento = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.ID_CITA_SEGUIMIENTO.HasValue ? "SI" : "NO" : "NO"
                    }
                };
                #endregion

                #region DENTAL
                ListaOdontogramaSeguimiento = new List<cReporteOdontogramaSeguimiento>();
                foreach (var item in SelectNotaMedicaAuxiliar.ODONTOGRAMA_SEGUIMIENTO2)
                {
                    ListaOdontogramaSeguimiento.Add(new cReporteOdontogramaSeguimiento
                    {
                        DESCR = string.Format("{0} EN {1} DEL {2}. POSICIÓN {3}", item.DENTAL_TRATAMIENTO == null ? item.ENFERMEDAD == null ? "" : item.ENFERMEDAD.NOMBRE.Trim() : item.DENTAL_TRATAMIENTO.DESCR.Trim(),
                            item.ODONTOGRAMA_TIPO.DESCR.Trim(), item.ODONTOGRAMA.DESCR.Trim(), item.ID_POSICION.ToString()),
                        ID = item.ID_CONSECUTIVO,
                    });
                }
                #endregion

                View.Report.LocalReport.ReportPath = "Reportes/rProcedimientosMedicos.rdlc";
                View.Report.LocalReport.DataSources.Clear();
                View.Report.LocalReport.DataSources.Add(rds1);
                #region Subreporte
                View.Report.LocalReport.SubreportProcessing += (s, e) =>
                {
                    if (e.ReportPath.Equals("cSubreporteProcedimientos", StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.DataSources.Clear();
                        e.DataSources.Add(new ReportDataSource("DataSet1", ListSubreporteProcedimientos));
                    }
                    else if (e.ReportPath.Equals("cSubreporteInsumoDetalle", StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.DataSources.Clear();
                        e.DataSources.Add(new ReportDataSource("DataSet1", ListSubreporteInsumos));
                    }
                    else if (e.ReportPath.Equals("cSubreporteOdontograma", StringComparison.InvariantCultureIgnoreCase) && SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL)
                    {
                        e.DataSources.Clear();
                        e.DataSources.Add(new ReportDataSource("DataSet1", ListaOdontogramaSeguimiento));
                    }
                };

                View.Report.LocalReport.SubreportProcessing += ProcesandoSubreporte;
                #endregion

                View.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                View.Report.Refresh();
                View.Report.RefreshReport();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del reporte.", ex);
            }
        }

        private void ProcesandoSubreporte(object s, SubreportProcessingEventArgs e)
        {
            if (e.ReportPath.Equals("cSubreporteProcedimientos", StringComparison.InvariantCultureIgnoreCase))
                e.DataSources.Add(new ReportDataSource("DataSet1", ListSubreporteProcedimientos));
            else if (e.ReportPath.Equals("cSubreporteInsumoDetalle", StringComparison.InvariantCultureIgnoreCase))
                e.DataSources.Add(new ReportDataSource("DataSet1", ListSubreporteInsumos));
            else if (e.ReportPath.Equals("cSubreporteOdontograma", StringComparison.InvariantCultureIgnoreCase) && SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL)
            {
                e.DataSources.Clear();
                e.DataSources.Add(new ReportDataSource("DataSet1", ListaOdontogramaSeguimiento));
            }
        }

        private void CargarReporteConsulta()
        {
            try
            {
                if (SelectNotaMedica == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una nota medica para cargar el reporte.");
                    return;
                }

                #region Iniciliza el entorno para mostrar el reporte al usuario
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                ReportesView View = new ReportesView();
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Show();
                #endregion

                SelectNotaMedicaAuxiliar = new cAtencionMedica().GetData().First(g => g.ID_ATENCION_MEDICA == SelectNotaMedica.ID_ATENCION_MEDICA);
                var medico = SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.PERSONA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.PERSONA : null : null; //new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault().EMPLEADO;
                var hoy = Fechas.GetFechaDateServer;
                var hora = hoy.ToShortTimeString() + " hrs. Del día ";
                var dia = hoy.Day + " de " + hoy.ToString("MMMM").ToUpper() + " del " + hoy.Year + ".";
                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";

                #region DELITOS
                var delitos = string.Empty;
                var i = 1;
                if (SelectIngreso.CAUSA_PENAL != null && SelectIngreso.CAUSA_PENAL.Any(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO))
                {
                    foreach (var item in SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO)
                    {
                        delitos = delitos + item.DESCR_DELITO + (i == SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO.Count ? "" : ",");
                        i++;
                    }
                }

                #endregion

                #region DIETAS
                var dietas = string.Empty;
                i = 0;
                if (SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(a => a.ESTATUS == "S") : false)
                    foreach (var item in SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_DIETA.Where(w => w.ESTATUS == "S"))
                    {
                        dietas = dietas + item.DIETA.DESCR + (i == SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_DIETA.Count(c => c.ESTATUS == "S") ? string.Empty : ", ");
                        i++;
                    }
                #endregion

                #region MEDICAMENTOS
                var medicamentos = string.Empty;
                i = 0;
                if (SelectNotaMedicaAuxiliar.RECETA_MEDICA.Any(a => a.RECETA_MEDICA_DETALLE.Any()))
                    foreach (var item in SelectNotaMedicaAuxiliar.RECETA_MEDICA.SelectMany(s => s.RECETA_MEDICA_DETALLE))
                    {
                        medicamentos = medicamentos + item.DOSIS + " " + item.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE.Trim() + " DE " + item.PRODUCTO.NOMBRE.Trim() + " " +
                            (item.DESAYUNO == 1 ? (item.COMIDA == 1 ? (item.CENA == 1 ? "EN LA MAÑANA, TARDE Y NOCHE" : "EN LA MAÑANA Y TARDE") :
                            (item.CENA == 1 ? "EN LA MAÑANA Y NOCHE" : "EN LA MAÑANA")) :
                            (item.COMIDA == 1 ? (item.CENA == 1 ? "EN LA TARDE Y NOCHE" : "EN LA TARDE") :
                            (item.CENA == 1 ? "EN LA NOCHE" : ""))) +
                            " POR " + (item.DURACION) + " " + "DIA" + (item.DURACION > 1 ? "S" : "") + (i == SelectNotaMedicaAuxiliar.RECETA_MEDICA.SelectMany(s => s.RECETA_MEDICA_DETALLE).Count() ? ", " : " ");
                        i++;
                    }
                #endregion

                #region ENFERMEDADES
                var enfermedades = string.Empty;
                i = 0;
                if (SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any() : false)
                    foreach (var item in SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD)
                    {
                        enfermedades = enfermedades + item.ENFERMEDAD.NOMBRE.Trim() + (i == SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Count() ? ", " : " ");
                        i++;
                    }
                #endregion

                var dientes = Convert.ToBase64String(new Imagenes().getImagen("miniDiente.png"));
                var _htmlDiente = "<html><table><tr><td>";
                var j = 0;
                for (j = 0; j < 16; j++)
                    _htmlDiente += "<image id='ImagenDiente' style='width:20px;height:50px;' src = \" data:image/png;base64, " + dientes + " alt=\"diente" + j + "\" /></td><td>";

                _htmlDiente += "</td></tr></table></html>";

                var odonto = new HistoriaClinicaViewModel(new Nullable<short>(), new Nullable<short>(), new Nullable<short>(), new Nullable<short>()).CaptureWebPageBytesP(_htmlDiente, 400, 100);

                #region ANTECEDENTES PAOTOLOGICOS
                var antecedentesPatologicos = string.Empty;
                if (SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null : false)
                {
                    var k = 1;
                    foreach (var item in SelectNotaMedicaAuxiliar.INGRESO.HISTORIA_CLINICA.SelectMany(s => s.HISTORIA_CLINICA_PATOLOGICOS))
                    {
                        antecedentesPatologicos = antecedentesPatologicos + item.PATOLOGICO_CAT.DESCR + (k <= SelectNotaMedicaAuxiliar.INGRESO.HISTORIA_CLINICA.SelectMany(s => s.HISTORIA_CLINICA_PATOLOGICOS).Count() ? ", " : "");
                        k++;
                    }
                }
                #endregion

                #region TOXICOMANIAS
                var toxicomanias = SelectNotaMedicaAuxiliar != null ?
                    SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ?
                        string.IsNullOrEmpty(SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TOXICOMANIAS) ? "" : "SI, " + SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TOXICOMANIAS
                    : ""
                : "";
                #endregion

                #region OBJETO
                rds1.Value = new List<cReporteCertificadoNuevoIngreso>()
                {
                    new cReporteCertificadoNuevoIngreso
                    { 
                        OdontogramaImagen = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? odonto : null,
                        DENTAL = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? "S" : "N",
                        ID_ATENCION_MEDICA = SelectNotaMedicaAuxiliar.ID_ATENCION_MEDICA,
                        Titulo = SelectNotaMedicaAuxiliar.ATENCION_SERVICIO != null ? SelectNotaMedicaAuxiliar.ATENCION_SERVICIO.DESCR : string.Empty,
                        EstudiosRealizados=SelectNotaMedicaAuxiliar.ID_TIPO_SERVICIO == (short)enumAtencionServicio.NOTA_EVOLUCION ? 
                            SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? 
                                SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_EVOLUCION != null ? 
                                    SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_EVOLUCION.ESTUDIO_RESULTADO
                                : string.Empty
                            : string.Empty
                        : string.Empty,
                        Pronostico = SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ?
                            SelectNotaMedicaAuxiliar.NOTA_MEDICA.PRONOSTICO != null ?
                                SelectNotaMedicaAuxiliar.NOTA_MEDICA.PRONOSTICO.DESCR
                            : string.Empty
                        : string.Empty,
                        SignosVitales_Observaciones = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.OBSERVACIONES: string.Empty : string.Empty,
                        SignosVitales_Glucemia = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.GLUCEMIA : string.Empty : string.Empty,
                        SignosVitales_Peso = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.PESO : string.Empty : string.Empty,
                        SignosVitales_Talla = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TALLA : string.Empty : string.Empty,
                        SignosVitales_FC = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC : string.Empty : string.Empty,
                        SignosVitales_FR = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA : string.Empty : string.Empty,
                        SignosVitales_T = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TEMPERATURA : string.Empty : string.Empty,
                        SignosVitales_TA = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL : string.Empty : string.Empty,
                        Centro = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DESCR) ? string.Empty:SelectIngreso.CENTRO1.DESCR.Trim() : "",
                        Ciudad = SelectIngreso.CENTRO1 != null ? SelectIngreso.CENTRO1.MUNICIPIO != null ? (string.IsNullOrEmpty(SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1) ? string.Empty : SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1.Trim()) : string.Empty : string.Empty,
                        Director = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DIRECTOR) ? string.Empty : SelectIngreso.CENTRO1.DIRECTOR.Trim() : "",
                        Cedula = medico != null ? medico.EMPLEADO != null ? medico.EMPLEADO.CEDULA != null ? medico.EMPLEADO.CEDULA.Trim() : "" : "" : "",
                        Edad = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value : new DateTime()).ToString(),
                        FechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        Fecha = hora + dia,
                        Folio = SelectIngreso.ID_ANIO + "/" + SelectIngreso.ID_IMPUTADO,
                        Escolaridad = SelectIngreso.ESCOLARIDAD != null ? (string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? string.Empty : SelectIngreso.ESCOLARIDAD.DESCR.Trim()) : string.Empty,
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
                        NombreImputado = SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim()),
                        TipoDelito = string.IsNullOrEmpty(delitos) ? "N/A" : delitos,
                        NombreMedico = medico != null ? (medico.NOMBRE.Trim() + " " + medico.PATERNO.Trim() + " " + (string.IsNullOrEmpty(medico.MATERNO) ? string.Empty : medico.MATERNO.Trim())) : string.Empty,
                        Sexo = medico != null ? medico.SEXO == "M" ? "MASCULINO" : medico.SEXO == "F" ? "FEMENINO" : string.Empty : string.Empty,
                        Logo1 = Parametro.LOGO_ESTADO,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Dorso = Parametro.CUERPO_DORSO,
                        Frente = Parametro.CUERPO_FRENTE,
                        Check = Parametro.IMAGEN_ZONA_CORPORAL,
                        AntecedentesPatologicos = antecedentesPatologicos,
                        Toxicomanias = toxicomanias,
                        //Interrogatorio = lesiones,//SelectNotaMedicaAuxiliar.NOTA_MEDICA!=null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.EXPLORACION_FISICA : "",
                        ExploracionFisica = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.EXPLORACION_FISICA : string.Empty : string.Empty,
                        PadecimientoTratamiento = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PADECIMIENTO : "" : "",
                        AmeritaHospitalizacion = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.AMERITA_HOSPITALIZACION == "S" ? "SI" : "NO" : "NO" : "NO",
                        Diagnostico = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.DIAGNOSTICO : string.Empty : string.Empty,
                        DiasEnSanar = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TARDA_SANAR_15 == "S" ? "SI" : "NO" : "NO" : "NO",
                        Observaciones = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.OBSERVACIONES : string.Empty : string.Empty,
                        PeligraVida = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PELIGRA_VIDA == "S" ? "SI" : "NO" : "NO" : "NO",
                        PlanTerapeutico = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PLAN_TERAPEUTICO : string.Empty : string.Empty,
                        Seguimiento = SelectNotaMedicaAuxiliar != null ? 
                            SelectNotaMedicaAuxiliar.ID_CITA_SEGUIMIENTO.HasValue ? 
                                SelectNotaMedicaAuxiliar.ATENCION_CITA1 != null ? 
                                    SelectNotaMedicaAuxiliar.ATENCION_CITA1.CITA_FECHA_HORA.HasValue ?
                                        "SI, " + SelectNotaMedicaAuxiliar.ATENCION_CITA1.CITA_FECHA_HORA.Value.ToString()
                                    : "SI" 
                                : "SI" 
                            : "NO" 
                        : "NO",
                        SolicitaInterconsulta = SelectNotaMedicaAuxiliar.NOTA_INTERCONSULTA != null ? "SI" : "NO",
                        Dietas = dietas, 
                        Medicamentos = medicamentos, 
                        Enfermedades = enfermedades,
                        Interrogatorio = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.RESUMEN_INTERROGAT : string.Empty : string.Empty
                    }
                };
                #endregion

                #region DENTAL
                ListaOdontogramaSeguimiento = new List<cReporteOdontogramaSeguimiento>();
                foreach (var item in SelectNotaMedicaAuxiliar.ODONTOGRAMA_SEGUIMIENTO2)
                {
                    ListaOdontogramaSeguimiento.Add(new cReporteOdontogramaSeguimiento
                    {
                        DESCR = string.Format("{0} EN {1} DEL {2}. POSICIÓN {3}", item.DENTAL_TRATAMIENTO == null ? item.ENFERMEDAD == null ? "" : item.ENFERMEDAD.NOMBRE.Trim() : item.DENTAL_TRATAMIENTO.DESCR.Trim(),
                            item.ODONTOGRAMA_TIPO.DESCR.Trim(), item.ODONTOGRAMA.DESCR.Trim(), item.ID_POSICION.ToString()),
                        ID = item.ID_CONSECUTIVO,
                    });
                }
                #endregion

                if (SelectNotaMedicaAuxiliar.ID_TIPO_SERVICIO == (short)enumAtencionServicio.NOTA_EVOLUCION)
                    View.Report.LocalReport.ReportPath = "Reportes/rNotaEvolucion.rdlc";
                else
                    View.Report.LocalReport.ReportPath = "Reportes/rConsultaMedica.rdlc";

                View.Report.LocalReport.DataSources.Clear();
                View.Report.LocalReport.DataSources.Add(rds1);
                //Reporte.LocalReport.DataSources.Add(rds2);
                View.Report.ShowExportButton = false;
                View.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                View.Report.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                View.Report.ZoomPercent = 100;
                View.Report.Margin = new System.Windows.Forms.Padding(5);
                View.Report.LocalReport.SubreportProcessing += ProcesandoSubreporte;

                View.Report.Refresh();
                View.Report.RefreshReport();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del reporte.", ex);
            }
        }

        private void CargarTipo_Servicios_Auxiliares()
        {
            try
            {
                //limpia la sub lista antes de llenar de nuevo
                lstSubtipoServAux = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT>();
                lstSubtipoServAux.Insert(0, new SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT { ID_SUBTIPO_SADT = -1, DESCR = "SELECCIONE" });
                RaisePropertyChanged("LstSubtipoServAux");

                lstTipoServAux = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT>(new SSP.Controlador.Catalogo.Justicia.cTipo_Serv_Aux_Diag_Trat().ObtenerTodos("", "S"));
                lstTipoServAux.Insert(0, new SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT { ID_TIPO_SADT = -1, DESCR = "SELECCIONE" });
                RaisePropertyChanged("LstTipoServAux");

                LstDiagnosticosPrincipal = new System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>();
                LstDiagnosticosPrincipal.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "SELECCIONE", ID_SERV_AUX = -1 });
                RaisePropertyChanged("LstDiagnosticosPrincipal");

                LstCustomizadaIngresos = new System.Collections.ObjectModel.ObservableCollection<CustomIngresos>();
                LstCustomizadaIngresos.Insert(0, new CustomIngresos { DescripcionIngreso = "TODOS", IdIngreso = -1 });
                RaisePropertyChanged("LstCustomizadaIngresos");

                SelectedIngresoBusquedas = -1;
                SelectedDiagnosticoEdicion = -1;
                SelectedDiagnPrincipal = -1;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de servicios auxiliares", ex);
            }
        }

        public byte[] CaptureWebPageBytesP(string body, int? width, int? height)
        {
            byte[] data;
            using (System.Windows.Forms.WebBrowser web = new System.Windows.Forms.WebBrowser())
            {
                if (width == null)
                    width = 400;
                else
                    width = width.Value;

                if (height == null)
                    height = 300;
                else
                    height = height.Value;

                web.ScrollBarsEnabled = false;//QUITA LOS SCROLL, PARA QUE NO SE MUESTREN EN LA IMAGEN
                web.ScriptErrorsSuppressed = true;//NO CARGUES LOS SCRIPTS
                web.Navigate("about:blank");//INICIALIZA UNA PAGINA EN BLANCO
                while (web.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)//HASTA QUE HALLA CARGADO POR COMPLETO
                    System.Windows.Forms.Application.DoEvents();

                web.Document.Body.InnerHtml = body;//INYECTA EL CODIGO HTML QUE SE CREO EN BASE A LOS PATOLOGICOS

                web.Width = width.Value;
                web.Height = height.Value;
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width.Value, height.Value))//CREA UNA CAPTURA DEL CODIGO QUE SE INYECTO ANTERIORMENTE EN EL NAVEGADOR
                {
                    web.DrawToBitmap(bmp, new System.Drawing.Rectangle(web.Location.X, web.Location.Y, 400, web.Height));//DEFINE LOS MARGENES DE LA CAPTURA
                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())//LEE Y GUARDA EL BUFFER
                    {
                        System.Drawing.Imaging.EncoderParameter qualityParam = null;
                        System.Drawing.Imaging.EncoderParameters encoderParams = null;
                        try
                        {
                            //DEFINE PARAMETROS PARA LA IMAGEN (CALIDAD, MIME TYPE, ETC)
                            System.Drawing.Imaging.ImageCodecInfo imageCodec = null;
                            imageCodec = GetEncoderInfo(System.Drawing.Imaging.ImageFormat.Jpeg);
                            qualityParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                            encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                            encoderParams.Param[0] = qualityParam;
                            bmp.Save(stream, imageCodec, encoderParams);
                        }
                        catch (Exception)
                        {
                            throw new Exception();
                        }

                        finally
                        {
                            //LIBERA LOS PARAMETROS UNA VEZ TERMINA EL PROCESO
                            if (encoderParams != null)
                                encoderParams.Dispose();
                            if (qualityParam != null)
                                qualityParam.Dispose();
                        }

                        //GUARDA LA IMAGEN, PROCESA Y LEE EL ARREGLO DE BYTES
                        bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        stream.Position = 0;
                        data = new byte[stream.Length];
                        stream.Read(data, 0, (int)stream.Length);
                    }
                }
            }
            return data;
        }

        private bool GeneraReporteDatos()
        {
            try
            {
                VisibleReporteBitacoraHospitalizaciones = Visibility.Collapsed;
                var _lista = new cHospitalizacion().ObtenerHospitalizacionesPorIngresoSinEstatus(SelectedIngreso);

                ds_detalle = new List<EXT_REPORTE_BITACORA_HOSPITALIZACION_DETALLE>();
                if (_lista != null && _lista.Any())
                {
                    foreach (var item in _lista)
                    {
                        ds_detalle.Add(new EXT_REPORTE_BITACORA_HOSPITALIZACION_DETALLE
                        {
                            FECHA_ALTA = item.ALTA_FEC,
                            FECHA_INGRESO = item.INGRESO_FEC,
                            FechaNac = item.NOTA_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA.HasValue ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value : new DateTime?() : new DateTime?() : new DateTime?() : new DateTime?() : new DateTime?(),
                            ID_ANIO = item.NOTA_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_ANIO : new short() : new short() : new short(),
                            ID_IMPUTADO = item.NOTA_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_IMPUTADO : new int() : new int() : new int() : new int(),
                            HOSPITALIZACION_TIPO = item.ID_INGHOSTIP,
                            MATERNO = item.NOTA_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            MATERNO_MEDICO_ALTA = item.EMPLEADO1 != null ? item.EMPLEADO1.PERSONA != null ? !string.IsNullOrEmpty(item.EMPLEADO1.PERSONA.MATERNO) ? item.EMPLEADO1.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            MATERNO_MEDICO_INGRESO = item.EMPLEADO != null ? item.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(item.EMPLEADO.PERSONA.MATERNO) ? item.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            NOCAMA = item.CAMA_HOSPITAL != null ? !string.IsNullOrEmpty(item.CAMA_HOSPITAL.DESCR) ? item.CAMA_HOSPITAL.DESCR.Trim() : string.Empty : string.Empty,
                            NOMBRE = string.Format("{0} {1} {2}", item.NOTA_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE) ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            item.NOTA_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty, item.NOTA_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty),
                            NOMBRE_MEDICO_ALTA = item.EMPLEADO1 != null ? item.EMPLEADO1.PERSONA != null ? !string.IsNullOrEmpty(item.EMPLEADO1.PERSONA.NOMBRE) ? item.EMPLEADO1.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            NOMBRE_MEDICO_INGRESO = item.EMPLEADO != null ? item.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(item.EMPLEADO.PERSONA.NOMBRE) ? item.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            PATERNO = item.NOTA_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? item.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            PATERNO_MEDICO_ALTA = item.EMPLEADO1 != null ? item.EMPLEADO1.PERSONA != null ? !string.IsNullOrEmpty(item.EMPLEADO1.PERSONA.PATERNO) ? item.EMPLEADO1.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                            PATERNO_MEDICO_INGRESO = item.EMPLEADO != null ? item.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(item.EMPLEADO.PERSONA.PATERNO) ? item.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty
                        });
                    }
                }

                var logo1 = Parametro.REPORTE_LOGO2;
                var logo2 = Parametro.LOGO_ESTADO;
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault().DESCR;
                ds_encabezado = new List<EXT_REPORTE_BITACORA_HOSPITALIZACION_ENCABEZADO>() {
                    new EXT_REPORTE_BITACORA_HOSPITALIZACION_ENCABEZADO{
                        LOGO1=logo1,
                        LOGO2=logo2,
                        CERESO=!string.IsNullOrEmpty(centro) ? centro.Trim() : string.Empty
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
                VisibleReporteBitacoraHospitalizaciones = Visibility.Visible;
                ReporteBH.LocalReport.ReportPath = "Reportes/rBitacoraIngresosEgresosHospitalizacionGeneral.rdlc";
                ReporteBH.LocalReport.DataSources.Clear();
                Microsoft.Reporting.WinForms.ReportDataSource rsd1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rsd1.Name = "DataSet1";
                rsd1.Value = ds_encabezado;
                Microsoft.Reporting.WinForms.ReportDataSource rsd2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rsd2.Name = "DataSet2";
                rsd2.Value = ds_detalle;
                ReporteBH.LocalReport.DataSources.Add(rsd1);
                ReporteBH.LocalReport.DataSources.Add(rsd2);
                ReporteBH.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                ReporteBH.RefreshReport();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el reporte", ex);
            }
        }

        private async void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_tipo_serv_aux":
                    CargarSubTipo_Servicios_Auxiliares(selectedTipoServAux);
                    selectedSubtipoServAux = -1;
                    SelectedDiagnPrincipal = -1;
                    RaisePropertyChanged("SelectedSubtipoServAux");
                    OnPropertyChanged("SelectedDiagnPrincipal");
                    break;

                case "cambio_subtipo_serv_aux":
                    CargarDiagnosticosPrincipal(SelectedSubtipoServAux);
                    SelectedDiagnPrincipal = -1;
                    RaisePropertyChanged("SelectedDiagnPrincipal");
                    break;

                case "cambio_tipo_serv_aux_edicion":
                    CargarSubTipoServiciosAuxiliaresEdicion(SelectedTipoServAuxEdicion);
                    SelectedSubTipoServAuxEdicion = -1;
                    SelectedDiagnosticoEdicion = -1;
                    RaisePropertyChanged("SelectedSubTipoServAuxEdicion");
                    break;

                case "cambio_subtipo_serv_aux_edicion":
                    SelectedSubtipoServAux = -1;
                    CargarTipoTratamientoEdicion(SelectedSubTipoServAuxEdicion);
                    SelectedDiagnosticoEdicion = -1;
                    RaisePropertyChanged("SelectedSubtipoServAux");
                    OnPropertyChanged("SelectedDiagnosticoEdicion");
                    break;
                #region Cambio SelectedItem Grid Expediente
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
                #endregion
            }
        }

        private void CargarSubTipo_Servicios_Auxiliares(short? tipo_serv_aux)
        {
            try
            {
                lstSubtipoServAux = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new SSP.Controlador.Catalogo.Justicia.cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(tipo_serv_aux, "S"));
                lstSubtipoServAux.Insert(0, new SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT { ID_SUBTIPO_SADT = -1, DESCR = "SELECCIONE" });
                RaisePropertyChanged("LstSubtipoServAux");

                LstDiagnosticosPrincipal = new System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>();
                LstDiagnosticosPrincipal.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "SELECCIONE", ID_SERV_AUX = -1 });
                RaisePropertyChanged("LstDiagnosticosPrincipal");
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", ex);
            }
        }

        private void CargarSubTipoServiciosAuxiliaresEdicion(short? tipo_serv_aux)
        {
            try
            {
                LstSubTipoServAuxEdicion = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new SSP.Controlador.Catalogo.Justicia.cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(tipo_serv_aux, "S"));
                LstSubTipoServAuxEdicion.Insert(0, new SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT { ID_SUBTIPO_SADT = -1, DESCR = "SELECCIONE" });
                RaisePropertyChanged("LstSubTipoServAuxEdicion");
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", ex);
            }
        }

        private void CargarTipoTratamientoEdicion(short? subTipo)
        {
            try
            {
                LstServAux = new System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(new SSP.Controlador.Catalogo.Justicia.cServ_Aux_Diag_Trat().ObtenerTodos(SelectedTipoServAuxEdicion, subTipo, "S").Select(s => new EXT_SERV_AUX_DIAGNOSTICO
                {
                    DESCR = s.DESCR,
                    ESTATUS = s.ESTATUS,
                    ID_SERV_AUX = s.ID_SERV_AUX,
                    ID_SUBTIPO_SADT = s.ID_SUBTIPO_SADT,
                    SUBTIPO_DESCR = s.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR
                }));
                LstServAux.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "SELECCIONE", ID_SERV_AUX = -1 });
                OnPropertyChanged("LstServAux");
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
                LstDiagnosticosPrincipal = new System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(new SSP.Controlador.Catalogo.Justicia.cServ_Aux_Diag_Trat().ObtenerTodos(SelectedTipoServAux, subTipo, "S").Select(s => new EXT_SERV_AUX_DIAGNOSTICO
                {
                    DESCR = s.DESCR,
                    ESTATUS = s.ESTATUS,
                    ID_SERV_AUX = s.ID_SERV_AUX,
                    ID_SUBTIPO_SADT = s.ID_SUBTIPO_SADT,
                    SUBTIPO_DESCR = s.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR
                }));

                LstDiagnosticosPrincipal.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "SELECCIONE", ID_SERV_AUX = -1 });
                OnPropertyChanged("LstDiagnosticosPrincipal");
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
                if (SelectIngreso == null)
                    return;

                System.DateTime f1, f2;
                if (FechaInicioBusquedaResultServ.HasValue)
                    f1 = FechaInicioBusquedaResultServ.Value;
                else
                    f1 = Fechas.GetFechaDateServer;

                if (FechaFinBusquedaResultServ.HasValue)
                    f2 = FechaFinBusquedaResultServ.Value;
                else
                    f2 = Fechas.GetFechaDateServer;

                f1 = new System.DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new System.DateTime(f2.Year, f2.Month, f2.Day);

                LstCustomizadaSinArchivos = new System.Collections.ObjectModel.ObservableCollection<CustomGridSinBytes>();
                var _datos = new SSP.Controlador.Catalogo.Justicia.cServicioAuxiliarResultado().BuscarResultados(f1, f2, SelectedTipoServAux, SelectedSubtipoServAux, SelectedDiagnPrincipal, SelectIngreso, SelectedIngresoBusquedas);
                if (_datos.Any())
                {
                    foreach (var item in _datos)
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
                    };

                    if (LstCustomizadaSinArchivos != null && LstCustomizadaSinArchivos.Count > 0)
                        EmptyResultados = false;
                    else
                        EmptyResultados = true;
                }
                else
                {
                    LstCustomizadaSinArchivos = new System.Collections.ObjectModel.ObservableCollection<CustomGridSinBytes>();
                    EmptyResultados = true;
                }
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", exc);
            }
        }

        public static System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(System.Drawing.Imaging.ImageFormat format)
        {
            return System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders().ToList().Find(delegate(System.Drawing.Imaging.ImageCodecInfo codec)
            {
                return codec.FormatID == format.Guid;
            });
        }

        private async void DescargaArchivo(CustomGridSinBytes Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                var _detallesArchivo = new SSP.Controlador.Catalogo.Justicia.cServicioAuxiliarResultado().GetData(z => z.ID_SA_RESULTADO == SeletedResultadoSinArchivo.IdResult).FirstOrDefault();
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
                        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
                        break;

                    case 2://xls
                        break;

                    case 3://pdf
                        var _file = _detallesArchivo.CAMPO_BLOB;
                        await System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".pdf";
                            System.IO.File.WriteAllBytes(fileNamepdf, _file);
                            System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                PDFViewer.LoadFile(fileNamepdf);
                                PDFViewer.Visibility = System.Windows.Visibility.Visible;
                            }));
                        });

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
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
                        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
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
                        tc.Show();
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

        private void ProcesaBusquedaHojasEnfermeria()
        {
            try
            {
                if (SelectedIngreso == null)
                    return;

                LstHojasenfermeria = new ObservableCollection<HOJA_ENFERMERIA>();
                LstHojasenfermeria = new ObservableCollection<HOJA_ENFERMERIA>(new cHojaEnfermeria().ObtenerHojasEnfermeriaBusqueda(FechaInicioBusquedaHojasEnfermeria, FechaFinBusquedaHojasEnfermeria, SelectedIngreso));
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void ProcesaBusquedaHojasControlLiquidos()
        {
            try
            {
                if (SelectedIngreso == null)
                    return;

                LstHojasLiquidos = new ObservableCollection<LIQUIDO_HOJA_CTRL>();
                LstHojasLiquidos = new ObservableCollection<LIQUIDO_HOJA_CTRL>(new cLiquidoHojaCtrl().ObtenerHojasLiquidosBusqueda(FechaInicioBusquedaHojasLiquidos, FechaFinBusquedaHojasLiquidos, SelectedIngreso, SelectTurnosHCL.HasValue ? SelectTurnosHCL.Value != -1 ? SelectTurnosHCL : new decimal?() : new decimal?()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ImprimeHojaEnfermeria()
        {
            try
            {
                if (SelectedIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar un ingreso valido.");
                    return;
                };

                ParametroCuerpoDorso = Parametro.CUERPO_DORSO;
                ParametroCuerpoFrente = Parametro.CUERPO_FRENTE;
                ParametroImagenZonaCorporal = Parametro.IMAGEN_ZONA_CORPORAL;

                ReportesView View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Show();

                var _UnicaH = new cHospitalizacion().GetData().FirstOrDefault(x => x.ID_HOSPITA == SeletedHojaEnfermeria.ID_HOSPITA);
                var _Enfermedades = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD : null : null;
                string _DatosDiagnotico = string.Empty;
                if (_Enfermedades != null && _Enfermedades.Any())
                    foreach (var item in _Enfermedades)
                        _DatosDiagnotico += string.Format("{0}, ", item.ENFERMEDAD != null ? !string.IsNullOrEmpty(item.ENFERMEDAD.NOMBRE) ? item.ENFERMEDAD.NOMBRE.Trim() : string.Empty : string.Empty);

                string DiagnosticoSinComa = !string.IsNullOrEmpty(_DatosDiagnotico) ? _DatosDiagnotico.TrimEnd(',') : string.Empty;
                cHojaEnfermeriaEncabezadoResporte _DatosCabezera = new cHojaEnfermeriaEncabezadoResporte()
                {
                    Cama = _UnicaH != null ? _UnicaH.CAMA_HOSPITAL != null ? !string.IsNullOrEmpty(_UnicaH.CAMA_HOSPITAL.DESCR) ? _UnicaH.CAMA_HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                    Diagnostico = DiagnosticoSinComa,
                    Edad = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? new Fechas().CalculaEdad(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Exp = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? string.Format("{0} / {1}", _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO, _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO) : string.Empty : string.Empty : string.Empty,
                    NombrePaciente = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ?
                        string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty)
                                 : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    FechaNacimiento = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA.HasValue ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Sexo = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.SEXO == "M" ? "MASCULINO" : _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.SEXO == "F" ? "FEMENINO" : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Peso = _UnicaH != null ? _UnicaH.NOTA_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA != null ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(_UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO) ? _UnicaH.NOTA_MEDICA.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.PESO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                    Fecha = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy"),
                    FechaIngreso = _UnicaH != null ? _UnicaH.INGRESO_FEC.HasValue ? _UnicaH.INGRESO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty,
                };

                System.Collections.Generic.List<cMedicamentosHojaEnfermeriaReporte> listaMedicamentosHojaEnfermeria = new System.Collections.Generic.List<cMedicamentosHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cHojaEnfermeriaSignosVitalesReporte> lstSignosVitales = new System.Collections.Generic.List<cHojaEnfermeriaSignosVitalesReporte>();
                System.Collections.Generic.List<cNotasEnfermeriaHojaEnfermeriaReporte> lstNotasEnfermeria = new System.Collections.Generic.List<cNotasEnfermeriaHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cSolucionesHojaEnfermeriaReporte> lstSolucionesHojaEnfermeria = new System.Collections.Generic.List<cSolucionesHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte> lstIngresosHojaEnfermeria = new System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte> lstIngresosHojaEnfermeriaFinal = new System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte> lstEgresosHojaEnfermeriaFinal = new System.Collections.Generic.List<cIngresosHojaEnfermeriaReporte>();
                System.Collections.Generic.List<cReporteCertificadoNuevoIngreso> listaUlceras = new System.Collections.Generic.List<cReporteCertificadoNuevoIngreso>();
                System.Collections.Generic.List<cReporteCertificadoNuevoIngreso> listaL = new System.Collections.Generic.List<cReporteCertificadoNuevoIngreso>();
                System.Collections.Generic.List<LesionesCustom> Lesiones = new System.Collections.Generic.List<LesionesCustom>();
                System.Collections.Generic.List<cCateteresReporteHojaEnfermeria> lstCateteresReporte = new System.Collections.Generic.List<cCateteresReporteHojaEnfermeria>();
                System.Collections.Generic.List<cSondaReporteHojaEnfermeria> lstSondasReporte = new System.Collections.Generic.List<cSondaReporteHojaEnfermeria>();
                System.Collections.Generic.List<cRayosXHojaenfermeriaReporte> lstRayosXReporte = new System.Collections.Generic.List<cRayosXHojaenfermeriaReporte>();
                System.Collections.Generic.List<cLaboratoriosHojaenfermeriaReporte> listLaboratoriosReporte = new System.Collections.Generic.List<cLaboratoriosHojaenfermeriaReporte>();

                var _TurnosDisponibles = new SSP.Controlador.Catalogo.Justicia.cLiquidoTurno().GetData(x => x.ESTATUS == "S");
                var _DatosHoja = new SSP.Controlador.Catalogo.Justicia.cHojaEnfermeria().GetData(x => x.FECHA_REGISTRO.Value.Year == SeletedHojaEnfermeria.FECHA_REGISTRO.Value.Year && x.FECHA_REGISTRO.Value.Month == SeletedHojaEnfermeria.FECHA_REGISTRO.Value.Month && x.FECHA_REGISTRO.Value.Day == SeletedHojaEnfermeria.FECHA_REGISTRO.Value.Day && x.ID_HOSPITA == SeletedHojaEnfermeria.ID_HOSPITA);
                if (_DatosHoja != null && _DatosHoja.Any())
                {
                    foreach (var item in _DatosHoja)
                    {
                        if (item.HOJA_ENFERMERIA_LECTURA != null && item.HOJA_ENFERMERIA_LECTURA.Any())
                            foreach (var item2 in item.HOJA_ENFERMERIA_LECTURA)
                                lstSignosVitales.Add(new cHojaEnfermeriaSignosVitalesReporte
                                {
                                    CambioPosicion = item2.CAMBIO_POSICION == "S" ? "SI" : item2.CAMBIO_POSICION == "N" ? "NO" : string.Empty,
                                    FC = !string.IsNullOrEmpty(item2.PC) ? item2.PC.Trim() : string.Empty,
                                    FR = !string.IsNullOrEmpty(item2.PR) ? item2.PR.Trim() : string.Empty,
                                    SAO = !string.IsNullOrEmpty(item2.SAO) ? item2.SAO.Trim() : string.Empty,
                                    NEB = !string.IsNullOrEmpty(item2.NEB) ? item2.NEB.Trim() : string.Empty,
                                    RiesgoEscara = item2.CAMBIO_ESCARAS == "S" ? "SI" : item2.CAMBIO_ESCARAS == "N" ? "NO" : string.Empty,
                                    Dextr = !string.IsNullOrEmpty(item2.DEXTROXTIX) ? item2.DEXTROXTIX.Trim() : string.Empty,
                                    PVC = !string.IsNullOrEmpty(item2.PVC) ? item2.PVC.Trim() : string.Empty,
                                    TA = !string.IsNullOrEmpty(item2.TA) ? item2.TA.Trim() : string.Empty,
                                    RiesgoCaidas = item2.RIESGO_CAIDAS == "S" ? "SI" : item2.RIESGO_CAIDAS == "N" ? "NO" : string.Empty,
                                    TAMedia = !string.IsNullOrEmpty(item2.TA_MEDIA) ? item2.TA_MEDIA.Trim() : string.Empty,
                                    Temp = !string.IsNullOrEmpty(item2.TEMP) ? item2.TEMP.Trim() : string.Empty,
                                    Generico = item2.FECHA_LECTURA.HasValue ? item2.FECHA_LECTURA.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty
                                });

                        if (item.HOJA_ENFERMERIA_ULCERA != null && item.HOJA_ENFERMERIA_ULCERA.Any())
                            foreach (var item2 in item.HOJA_ENFERMERIA_ULCERA)
                            {
                                Lesiones.Add(new LesionesCustom
                                {
                                    DESCR = item2.DESC,
                                    REGION = item2.ANATOMIA_TOPOGRAFICA
                                });
                            };

                        if (!string.IsNullOrEmpty(item.RX))
                            lstRayosXReporte.Add(new cRayosXHojaenfermeriaReporte
                            {
                                Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                RayosX = item.RX
                            });

                        if (!string.IsNullOrEmpty(item.LABORATORIO))
                            listLaboratoriosReporte.Add(new cLaboratoriosHojaenfermeriaReporte
                            {
                                Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                Laboratorio = item.LABORATORIO
                            });

                        if (item.HOJA_ENFERMERIA_CATETER != null && item.HOJA_ENFERMERIA_CATETER.Any())
                            foreach (var item2 in item.HOJA_ENFERMERIA_CATETER)
                            {
                                lstCateteresReporte.Add(new cCateteresReporteHojaEnfermeria
                                {
                                    FechaInstalado = item2.INSTALACION_FEC.HasValue ? item2.INSTALACION_FEC.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                    Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                    FechaRetirado = item2.REGISTRO_FEC.HasValue ? item2.REGISTRO_FEC.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                    Retirado = item2.RETIRO == "S" ? "SI" : item2.RETIRO == "N" ? "NO" : string.Empty,
                                    TipoCatater = item2.CATETER_TIPO != null ? !string.IsNullOrEmpty(item2.CATETER_TIPO.DESCR) ? item2.CATETER_TIPO.DESCR.Trim() : string.Empty : string.Empty
                                });
                            };

                        if (item.HOJA_ENFERMERIA_SONDA_GASOGAS != null && item.HOJA_ENFERMERIA_SONDA_GASOGAS.Any())
                            foreach (var item2 in item.HOJA_ENFERMERIA_SONDA_GASOGAS)
                            {
                                lstSondasReporte.Add(new cSondaReporteHojaEnfermeria
                                {
                                    FechaInstalacion = item2.INSTALACION_FEC.HasValue ? item2.INSTALACION_FEC.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                    Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                    Retirado = item2.RETIRO == "S" ? "SI" : item2.RETIRO == "N" ? "NO" : string.Empty
                                });
                            };

                        if (item.HOJA_CONTROL_ENFERMERIA != null && item.HOJA_CONTROL_ENFERMERIA.Any())
                            foreach (var item2 in item.HOJA_CONTROL_ENFERMERIA)
                            {
                                lstIngresosHojaEnfermeria.Add(new cIngresosHojaEnfermeriaReporte
                                {
                                    Nombre = item2.LIQUIDO != null ? !string.IsNullOrEmpty(item2.LIQUIDO.DESCR) ? item2.LIQUIDO.DESCR.Trim() : string.Empty : string.Empty,
                                    Horas = item2.ID_LIQ,
                                    Generico3 = item2.CANT.HasValue ? item2.CANT.Value.ToString() : string.Empty,
                                    Hora = item2.LIQUIDO_HORA != null ? !string.IsNullOrEmpty(item2.LIQUIDO_HORA.DESCR) ? item2.LIQUIDO_HORA.DESCR.Trim() : string.Empty : string.Empty,
                                    Generico = item2.LIQUIDO != null ? item2.LIQUIDO.ID_LIQTIPO : string.Empty,
                                    IdTurn = item2.ID_LIQHORA.HasValue ? item2.LIQUIDO_HORA.ID_LIQTURNO : new decimal?()
                                });
                            };

                        if (!string.IsNullOrEmpty(item.OBSERVACION))
                            lstNotasEnfermeria.Add(new cNotasEnfermeriaHojaEnfermeriaReporte
                            {
                                Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                Nota = !string.IsNullOrEmpty(item.OBSERVACION) ? item.OBSERVACION.Trim() : string.Empty
                            });

                        int _Consecutivo = new int();
                        if (_TurnosDisponibles.Any())
                            foreach (var item2 in _TurnosDisponibles)
                                if (item.HOJA_ENFERMERIA_SOLUCION != null && item.HOJA_ENFERMERIA_SOLUCION.Any())
                                {
                                    var SolucionesTurno = item.HOJA_ENFERMERIA_SOLUCION.Where(x => x.ID_LIQTURNO_INICIO == item2.ID_LIQTURNO);
                                    foreach (var item3 in SolucionesTurno)
                                    {
                                        _Consecutivo++;
                                        lstSolucionesHojaEnfermeria.Add(new cSolucionesHojaEnfermeriaReporte
                                        {
                                            Nombre = item3.ID_PRODUCTO.HasValue ? !string.IsNullOrEmpty(item3.PRODUCTO.NOMBRE) ? item3.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty,
                                            Generico = item3.ID_LIQTURNO_INICIO.HasValue ? !string.IsNullOrEmpty(item3.LIQUIDO_TURNO.DESCR) ? item3.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                            Consecutivo = _Consecutivo,
                                            Generico2 = item3.TERMINO == "S" ? "SI" : item3.TERMINO == "N" ? "NO" : string.Empty
                                        });
                                    };
                                };

                        if (item.HOJA_ENFERMERIA_MEDICAMENTO != null && item.HOJA_ENFERMERIA_MEDICAMENTO.Any())
                        {
                            var _refinados = item.HOJA_ENFERMERIA_MEDICAMENTO.OrderBy(x => x.FECHA_SUMINISTRO);
                            foreach (var item2 in _refinados)
                                listaMedicamentosHojaEnfermeria.Add(new cMedicamentosHojaEnfermeriaReporte
                                {
                                    Generico = item.ID_LIQTURNO.HasValue ? !string.IsNullOrEmpty(item.LIQUIDO_TURNO.DESCR) ? item.LIQUIDO_TURNO.DESCR.Trim() : string.Empty : string.Empty,
                                    Cantidad = item2.CANTIDAD.HasValue ? item2.CANTIDAD.Value.ToString() : string.Empty,
                                    Cena = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.CENA == 1 ? "SI" : item2.RECETA_MEDICA_DETALLE.CENA == 2 ? "NO" : string.Empty : string.Empty,
                                    Comida = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.COMIDA == 1 ? "SI" : item2.RECETA_MEDICA_DETALLE.COMIDA == 2 ? "NO" : string.Empty : string.Empty,
                                    Desayuno = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.DESAYUNO == 1 ? "SI" : item2.RECETA_MEDICA_DETALLE.DESAYUNO == 2 ? "NO" : string.Empty : string.Empty,
                                    DuracionTratam = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.DURACION.HasValue ? item2.RECETA_MEDICA_DETALLE.DURACION.Value.ToString() : string.Empty : string.Empty,
                                    FecSuministro = item2.FECHA_SUMINISTRO.HasValue ? item2.FECHA_SUMINISTRO.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                    FechaReceto = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.RECETA_MEDICA != null ? item2.RECETA_MEDICA_DETALLE.RECETA_MEDICA.RECETA_FEC.HasValue ? item2.RECETA_MEDICA_DETALLE.RECETA_MEDICA.RECETA_FEC.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty : string.Empty,
                                    UnidadMedida = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.PRODUCTO != null ? item2.RECETA_MEDICA_DETALLE.PRODUCTO.ID_UNIDAD_MEDIDA.HasValue ? !string.IsNullOrEmpty(item2.RECETA_MEDICA_DETALLE.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.DESCR) ? item2.RECETA_MEDICA_DETALLE.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                    PresentacionMedicamento = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.ID_PRESENTACION_MEDICAMENTO.HasValue ? !string.IsNullOrEmpty(item2.RECETA_MEDICA_DETALLE.PRESENTACION_MEDICAMENTO.DESCRIPCION) ? item2.RECETA_MEDICA_DETALLE.PRESENTACION_MEDICAMENTO.DESCRIPCION.Trim() : string.Empty : string.Empty : string.Empty,
                                    Nombre = item2.RECETA_MEDICA_DETALLE != null ? item2.RECETA_MEDICA_DETALLE.PRODUCTO != null ? !string.IsNullOrEmpty(item2.RECETA_MEDICA_DETALLE.PRODUCTO.NOMBRE) ? item2.RECETA_MEDICA_DETALLE.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty
                                });
                        };
                    };

                    //AQUI SE COMPARA CON RESPECTO A LOS TURNOS QUE NO SE CAPTURARON PARA AGREGARLOS AL FORMATO DEL REPORTE Y VISUALIZAR LOS 3 TURNOS
                    int _Cons = 1;
                    if (_TurnosDisponibles != null && _TurnosDisponibles.Any())
                        foreach (var item in _TurnosDisponibles)
                        {
                            if (!string.IsNullOrEmpty(item.DESCR))
                            {
                                if (lstNotasEnfermeria != null && lstNotasEnfermeria.Any())
                                    if (!lstNotasEnfermeria.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        lstNotasEnfermeria.Add(new cNotasEnfermeriaHojaEnfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });
                                    else { }
                                else///NUNCA HA EXISTIDO, HAY QUE AGREGARLO AL ESQUELETO DEL REPORTE
                                    lstNotasEnfermeria.Add(new cNotasEnfermeriaHojaEnfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });

                                if (lstCateteresReporte != null && lstCateteresReporte.Any())
                                    if (!lstCateteresReporte.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        lstCateteresReporte.Add(new cCateteresReporteHojaEnfermeria { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });
                                    else { /*no action */}
                                else
                                    lstCateteresReporte.Add(new cCateteresReporteHojaEnfermeria { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });

                                if (lstRayosXReporte != null && lstRayosXReporte.Any())
                                    if (!lstRayosXReporte.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        lstRayosXReporte.Add(new cRayosXHojaenfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });
                                    else { }
                                else
                                    lstRayosXReporte.Add(new cRayosXHojaenfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });

                                if (listLaboratoriosReporte != null && listLaboratoriosReporte.Any())
                                    if (!listLaboratoriosReporte.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        listLaboratoriosReporte.Add(new cLaboratoriosHojaenfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });
                                    else { }
                                else
                                    listLaboratoriosReporte.Add(new cLaboratoriosHojaenfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });

                                if (lstSondasReporte != null && lstSondasReporte.Any())
                                    if (!lstSondasReporte.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        lstSondasReporte.Add(new cSondaReporteHojaEnfermeria { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });
                                    else { }
                                else
                                    lstSondasReporte.Add(new cSondaReporteHojaEnfermeria { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty });

                                if (lstSolucionesHojaEnfermeria != null && lstSolucionesHojaEnfermeria.Any())
                                    if (!lstSolucionesHojaEnfermeria.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                    {
                                        lstSolucionesHojaEnfermeria.Add(new cSolucionesHojaEnfermeriaReporte
                                        {
                                            Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                                            //Consecutivo = _Cons
                                        });
                                    }
                                    else { }
                                else
                                    lstSolucionesHojaEnfermeria.Add(new cSolucionesHojaEnfermeriaReporte
                                    {
                                        Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                                        //Consecutivo = _Cons
                                    });

                                if (listaMedicamentosHojaEnfermeria != null && listaMedicamentosHojaEnfermeria.Any())
                                    if (!listaMedicamentosHojaEnfermeria.Any(x => x.Generico.Trim() == item.DESCR.Trim()))
                                        listaMedicamentosHojaEnfermeria.Add(new cMedicamentosHojaEnfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty/*, Nombre = string.Empty, Cantidad = string.Empty*/ });
                                    else { }
                                else
                                    listaMedicamentosHojaEnfermeria.Add(new cMedicamentosHojaEnfermeriaReporte { Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty/*, Nombre = string.Empty, Cantidad = string.Empty*/ });
                            };
                            _Cons++;
                        };
                }
                else
                {
                    //SI ENTRA AQUI ES PORQUE NO TIENE UNA HOJA DE ENFERMERIA REGISTRADA, CREA EL ESQUELETO BASE DEL REPORTE PARA DEFINIR LOS TURNOS
                    int _Cons = 1;
                    if (_TurnosDisponibles != null && _TurnosDisponibles.Any())
                        foreach (var item in _TurnosDisponibles)
                        {
                            lstSolucionesHojaEnfermeria.Add(new cSolucionesHojaEnfermeriaReporte
                            {
                                Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                            });

                            listaMedicamentosHojaEnfermeria.Add(new cMedicamentosHojaEnfermeriaReporte
                            {
                                Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                                //Nombre = string.Empty,
                                //Cantidad = string.Empty
                            });

                            lstNotasEnfermeria.Add(new cNotasEnfermeriaHojaEnfermeriaReporte
                            {
                                Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                            });

                            lstCateteresReporte.Add(new cCateteresReporteHojaEnfermeria
                            {
                                Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                            });

                            lstRayosXReporte.Add(new cRayosXHojaenfermeriaReporte
                            {
                                Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                            });

                            listLaboratoriosReporte.Add(new cLaboratoriosHojaenfermeriaReporte
                            {
                                Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                            });

                            lstSondasReporte.Add(new cSondaReporteHojaEnfermeria
                            {
                                Generico = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty
                            });

                            _Cons++;
                        };
                }

                var _HorasDisponibles = new SSP.Controlador.Catalogo.Justicia.cLiquidoHora().GetData(x => x.ESTATUS == "S", y => y.ID_LIQHORA.ToString());//ESTAS SON LAS HORAS QUE TENGO PARA TRABAJAR
                var _LiquidosIngreso = new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "1" && x.ID_LIQ_FORMATO == 2);//ESTOS SON LOS LIQUIDOS QUE TNGO PARA TRABAJAR
                var _LiquidosEgreso = new SSP.Controlador.Catalogo.Justicia.cLiquido().GetData(x => x.ID_LIQTIPO == "2" && x.ID_LIQ_FORMATO == 2);
                var IngresosEncontrados = lstIngresosHojaEnfermeria != null ? lstIngresosHojaEnfermeria.Any() ? lstIngresosHojaEnfermeria.Where(x => x.Generico == "1") : null : null;
                var EgresosEncontrados = lstIngresosHojaEnfermeria != null ? lstIngresosHojaEnfermeria.Any() ? lstIngresosHojaEnfermeria.Where(x => x.Generico == "2") : null : null;
                #region Balances de liquidos de ingreso y egreso
                decimal? BalanceM, BalanceV, BalanceN = new decimal?();
                decimal? BalanceEM, BalanceEV, BalanceEN = new decimal?();
                BalanceM = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.MATUTUNO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.MATUTUNO).ESTATUS == "N" ? IngresosEncontrados != null ? IngresosEncontrados.Any() ? IngresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.MATUTUNO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                BalanceV = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.VESPERTINO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.VESPERTINO).ESTATUS == "N" ? IngresosEncontrados != null ? IngresosEncontrados.Any() ? IngresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.VESPERTINO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                BalanceN = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.NOCTURNO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.NOCTURNO).ESTATUS == "N" ? IngresosEncontrados != null ? IngresosEncontrados.Any() ? IngresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.NOCTURNO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                BalanceEM = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.MATUTUNO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.MATUTUNO).ESTATUS == "N" ? EgresosEncontrados != null ? EgresosEncontrados.Any() ? EgresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.MATUTUNO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                BalanceEV = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.VESPERTINO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.VESPERTINO).ESTATUS == "N" ? EgresosEncontrados != null ? EgresosEncontrados.Any() ? EgresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.VESPERTINO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                BalanceEN = _DatosHoja != null ? _DatosHoja.Any() ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.NOCTURNO) != null ? _DatosHoja.FirstOrDefault(x => x.ID_LIQTURNO == (decimal)eTurnosLiqudos.NOCTURNO).ESTATUS == "N" ? EgresosEncontrados != null ? EgresosEncontrados.Any() ? EgresosEncontrados.Where(y => y.IdTurn == (decimal)eTurnosLiqudos.NOCTURNO).Sum(x => System.Convert.ToDecimal(x.Generico3)) : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?() : new decimal?();
                _DatosCabezera.Generico = BalanceM.HasValue ? BalanceM.Value.ToString() : string.Empty;
                _DatosCabezera.Generico2 = BalanceV.HasValue ? BalanceV.Value.ToString() : string.Empty;
                _DatosCabezera.Generico3 = BalanceN.HasValue ? BalanceN.Value.ToString() : string.Empty;
                _DatosCabezera.Generico4 = BalanceEM.HasValue ? BalanceEM.Value.ToString() : string.Empty;
                _DatosCabezera.Generico5 = BalanceEV.HasValue ? BalanceEV.Value.ToString() : string.Empty;
                _DatosCabezera.Generico6 = BalanceEN.HasValue ? BalanceEN.Value.ToString() : string.Empty;
                #endregion

                if (_HorasDisponibles != null && _HorasDisponibles.Any())
                {
                    string Gen = string.Empty;
                    if (_LiquidosIngreso.Any())
                        foreach (var item in _LiquidosIngreso)
                            Gen += string.Format("{0}\n", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty);//SIRVE COMO BASE DE LA MATRIZ

                    string Da = string.Empty;
                    if (_LiquidosEgreso.Any())
                        foreach (var item in _LiquidosEgreso)
                            Da += string.Format("{0}\n", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty);//SIRVE COMO BASE DE LA MATRIZ

                    foreach (var item in _HorasDisponibles)
                    {
                        #region INGRESOS
                        string Dat = string.Empty;
                        if (_LiquidosIngreso != null && _LiquidosIngreso.Any())
                        {
                            foreach (var item2 in _LiquidosIngreso)
                            {
                                if (IngresosEncontrados != null && IngresosEncontrados.Any())
                                {
                                    if (IngresosEncontrados.Any(x => x.Horas == item2.ID_LIQ && x.Hora == item.DESCR))
                                        Dat += string.Format("{0} \n", IngresosEncontrados.FirstOrDefault(x => x.Horas == item2.ID_LIQ).Generico3);
                                    else
                                        Dat += string.Format("{0} \n", "*");
                                }
                                else
                                    Dat += string.Format("{0} \n", "*");
                            };
                        };

                        lstIngresosHojaEnfermeriaFinal.Add(new cIngresosHojaEnfermeriaReporte
                        {
                            Generico3 = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                            Generico2 = !string.IsNullOrEmpty(Gen) ? Gen.TrimEnd('\n') : string.Empty,
                            Hora = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                            Generico = !string.IsNullOrEmpty(Dat) ? Dat.TrimEnd('\n') : string.Empty
                        });

                        #endregion
                        #region EGRESOS
                        string Egr = string.Empty;
                        if (_LiquidosEgreso != null && _LiquidosEgreso.Any())
                        {
                            foreach (var item3 in _LiquidosEgreso)
                            {
                                if (EgresosEncontrados != null && EgresosEncontrados.Any())
                                {
                                    if (EgresosEncontrados.Any(x => x.Horas == item3.ID_LIQ && x.Hora == item.DESCR))
                                        Egr += string.Format("{0} \n", EgresosEncontrados.FirstOrDefault(x => x.Horas == item3.ID_LIQ).Generico3);
                                    else
                                        Egr += string.Format("{0} \n", "*");
                                }
                                else
                                    Egr += string.Format("{0} \n", "*");
                            };
                        };

                        lstEgresosHojaEnfermeriaFinal.Add(new cIngresosHojaEnfermeriaReporte
                        {
                            Generico3 = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                            Generico2 = !string.IsNullOrEmpty(Da) ? Da.TrimEnd('\n') : string.Empty,
                            Hora = !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Trim() : string.Empty,
                            Generico = !string.IsNullOrEmpty(Egr) ? Egr.TrimEnd('\n') : string.Empty
                        });
                        #endregion
                    };
                };

                if (Lesiones != null && Lesiones.Any())
                {
                    foreach (var item in Lesiones)
                    {
                        listaL.Add(new cReporteCertificadoNuevoIngreso
                        {
                            Cedula = item.REGION != null ? !string.IsNullOrEmpty(item.REGION.DESCR) ? item.REGION.DESCR.Trim() : string.Empty : string.Empty,
                            NombreMedico = item.DESCR
                        });
                    };

                    listaUlceras.Add(new cReporteCertificadoNuevoIngreso
                    {
                        Dorso = ParametroCuerpoDorso,
                        Frente = ParametroCuerpoFrente,
                        Check = ParametroImagenZonaCorporal,
                        F_FRONTAL = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FRONTAL) : true,
                        D_ANTEBRAZO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_DERECHO) : true,
                        D_ANTEBRAZO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_IZQUIERDO) : true,
                        D_BRAZO_POSTERIOR_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_DERECHO) : true,
                        D_BRAZO_POSTERIOR_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_IZQUIERDO) : true,
                        D_CALCANEO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_DERECHO) : true,
                        D_CALCANEO_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_IZQUIERDA) : true,
                        D_CODO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_DERECHO) : true,
                        D_CODO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_IZQUIERDO) : true,
                        D_COSTILLAS_COSTADO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_DERECHO) : true,
                        D_COSTILLAS_COSTADO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_IZQUIERDO) : true,
                        D_DORSAL_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_DERECHA) : true,
                        D_DORSAL_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_IZQUIERDA) : true,
                        D_ESCAPULAR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_DERECHA) : true,
                        D_ESCAPULAR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_IZQUIERDA) : true,
                        D_FALANGES_POSTERIORES_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_DERECHA) : true,
                        D_FALANGES_POSTERIORES_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_IZQUIERDA) : true,
                        D_GLUTEA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_DERECHA) : true,
                        D_GLUTEA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_IZQUIERDA) : true,
                        D_LUMBAR_RENAL_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_LUMBAR_RENAL_DERECHA) : true,
                        D_LUMBAR_RENAL_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                        D_METATARZIANA_PLANTA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                        D_METATARZIANA_PLANTA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_IZQUIERDA) : true,
                        D_MUÑECA_POSTERIOR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_DERECHA) : true,
                        D_MUÑECA_POSTERIOR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_IZQUIERDA) : true,
                        D_MUSLO_POSTERIOR_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_DERECHO) : true,
                        D_MUSLO_POSTERIOR_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_IZQUIERDO) : true,
                        D_OCCIPITAL_NUCA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OCCIPITAL_NUCA) : true,
                        D_OREJA_POSTERIOR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_DERECHA) : true,
                        D_OREJA_POSTERIOR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_IZQUIERDA) : true,
                        D_PANTORRILLA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_DERECHA) : true,
                        D_PANTORRILLA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_IZQUIERDA) : true,
                        D_POPLITEA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_DERECHA) : true,
                        D_POPLITEA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_IZQUIERDA) : true,
                        D_POSTERIOR_CABEZA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CABEZA) : true,
                        D_POSTERIOR_CUELLO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CUELLO) : true,
                        D_POSTERIOR_ENTREPIERNA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_DERECHA) : true,
                        D_POSTERIOR_ENTREPIERNA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_IZQUIERDA) : true,
                        D_POSTERIOR_HOMBRO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_DERECHO) : true,
                        D_POSTERIOR_HOMBRO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_IZQUIERDO) : true,
                        D_SACRA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_SACRA) : true,
                        D_TOBILLO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_DERECHO) : true,
                        D_TOBILLO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_IZQUIERDO) : true,
                        D_TORACICA_DORSAL = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TORACICA_DORSAL) : true,
                        D_VERTEBRAL_CERVICAL = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_CERVICAL) : true,
                        D_VERTEBRAL_LUMBAR = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_LUMBAR) : true,
                        D_VERTEBRAL_TORACICA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_TORACICA) : true,
                        F_ANTEBRAZO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_DERECHO) : true,
                        F_ANTEBRAZO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_IZQUIERDO) : true,
                        F_ANTERIOR_CUELLO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTERIOR_CUELLO) : true,
                        F_MUÑECA_ANTERIOR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_DERECHA) : true,
                        F_MUÑECA_ANTERIOR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_IZQUIERDA) : true,
                        F_AXILA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_DERECHA) : true,
                        F_AXILA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_IZQUIERDA) : true,
                        F_BAJO_VIENTRE_HIPOGASTRIO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BAJO_VIENTRE_HIPOGASTRIO) : true,
                        F_BRAZO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_DERECHO) : true,
                        F_BRAZO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_IZQUIERDO) : true,
                        F_CARA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                        F_CARA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CARA_IZQUIERDA) : true,
                        F_CLAVICULAR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                        F_CLAVICULAR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_IZQUIERDA) : true,
                        F_CODO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_DERECHO) : true,
                        F_CODO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_IZQUIERDO) : true,
                        F_ENTREPIERNA_ANTERIOR_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_DERECHA) : true,
                        F_ENTREPIERNA_ANTERIOR_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_IZQUIERDA) : true,
                        F_EPIGASTRIO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_EPIGASTRIO) : true,
                        F_ESCROTO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESCROTO) : true,
                        F_ESPINILLA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_DERECHA) : true,
                        F_ESPINILLA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_IZQUIERDA) : true,
                        F_FALANGES_MANO_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_DERECHA) : true,
                        F_FALANGES_MANO_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_IZQUIERDA) : true,
                        F_FALANGES_PIE_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_DERECHO) : true,
                        F_FALANGES_PIE_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_IZQUIERDO) : true,
                        F_HIPOCONDRIA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_DERECHA) : true,
                        F_HIPOCONDRIA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_IZQUIERDA) : true,
                        F_HOMBRO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_DERECHO) : true,
                        F_HOMBRO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_IZQUIERDO) : true,
                        F_INGUINAL_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_DERECHA) : true,
                        F_INGUINAL_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_IZQUIERDA) : true,
                        F_LATERAL_CABEZA_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_DERECHO) : true,
                        F_LATERAL_CABEZA_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_IZQUIERDO) : true,
                        F_MANDIBULA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MANDIBULA) : true,
                        F_METACARPIANOS_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_DERECHA) : true,
                        F_METACARPIANOS_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_IZQUIERDA) : true,
                        F_METATARZIANA_DORSAL_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_DERECHO) : true,
                        F_METATARZIANA_DORSAL_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_IZQUIERDO) : true,
                        F_MUSLO_ANTERIOR_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_DERECHO) : true,
                        F_MUSLO_ANTERIOR_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_IZQUIERDO) : true,
                        F_NARIZ = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_NARIZ) : true,
                        F_ORBITAL_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_DERECHO) : true,
                        F_ORBITAL_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_IZQUIERDO) : true,
                        F_OREJA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_DERECHA) : true,
                        F_OREJA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_IZQUIERDA) : true,
                        F_PENE_VAGINA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PENE_VAGINA) : true,
                        F_PEZON_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_DERECHO) : true,
                        F_PEZON_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_IZQUIERDO) : true,
                        F_RODILLA_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_DERECHA) : true,
                        F_RODILLA_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_IZQUIERDA) : true,
                        F_TOBILLO_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_DERECHO) : true,
                        F_TOBILLO_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_IZQUIERDO) : true,
                        F_TORAX_CENTRAL = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_CENTRAL) : true,
                        F_TORAX_DERECHO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_DERECHO) : true,
                        F_TORAX_IZQUIERDO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_IZQUIERDO) : true,
                        F_UMBILICAL_MESOGASTIO = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_UMBILICAL_MESOGASTIO) : true,
                        F_VACIO_DERECHA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_DERECHA) : true,
                        F_VACIO_IZQUIERDA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_IZQUIERDA) : true,
                        F_VERTICE_CABEZA = Lesiones != null ? !Lesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VERTICE_CABEZA) : true
                    });
                }
                else
                {
                    #region Ulcera Vacia
                    listaUlceras.Add(new cReporteCertificadoNuevoIngreso
                    {
                        Dorso = ParametroCuerpoDorso,
                        Frente = ParametroCuerpoFrente,
                        Check = ParametroImagenZonaCorporal,
                        F_FRONTAL = true,
                        D_ANTEBRAZO_DERECHO = true,
                        D_ANTEBRAZO_IZQUIERDO = true,
                        D_BRAZO_POSTERIOR_DERECHO = true,
                        D_BRAZO_POSTERIOR_IZQUIERDO = true,
                        D_CALCANEO_DERECHO = true,
                        D_CALCANEO_IZQUIERDA = true,
                        D_CODO_DERECHO = true,
                        D_CODO_IZQUIERDO = true,
                        D_COSTILLAS_COSTADO_DERECHO = true,
                        D_COSTILLAS_COSTADO_IZQUIERDO = true,
                        D_DORSAL_DERECHA = true,
                        D_DORSAL_IZQUIERDA = true,
                        D_ESCAPULAR_DERECHA = true,
                        D_ESCAPULAR_IZQUIERDA = true,
                        D_FALANGES_POSTERIORES_DERECHA = true,
                        D_FALANGES_POSTERIORES_IZQUIERDA = true,
                        D_GLUTEA_DERECHA = true,
                        D_GLUTEA_IZQUIERDA = true,
                        D_LUMBAR_RENAL_DERECHA = true,
                        D_LUMBAR_RENAL_IZQUIERDA = true,
                        D_METATARZIANA_PLANTA_DERECHA = true,
                        D_METATARZIANA_PLANTA_IZQUIERDA = true,
                        D_MUÑECA_POSTERIOR_DERECHA = true,
                        D_MUÑECA_POSTERIOR_IZQUIERDA = true,
                        D_MUSLO_POSTERIOR_DERECHO = true,
                        D_MUSLO_POSTERIOR_IZQUIERDO = true,
                        D_OCCIPITAL_NUCA = true,
                        D_OREJA_POSTERIOR_DERECHA = true,
                        D_OREJA_POSTERIOR_IZQUIERDA = true,
                        D_PANTORRILLA_DERECHA = true,
                        D_PANTORRILLA_IZQUIERDA = true,
                        D_POPLITEA_DERECHA = true,
                        D_POPLITEA_IZQUIERDA = true,
                        D_POSTERIOR_CABEZA = true,
                        D_POSTERIOR_CUELLO = true,
                        D_POSTERIOR_ENTREPIERNA_DERECHA = true,
                        D_POSTERIOR_ENTREPIERNA_IZQUIERDA = true,
                        D_POSTERIOR_HOMBRO_DERECHO = true,
                        D_POSTERIOR_HOMBRO_IZQUIERDO = true,
                        D_SACRA = true,
                        D_TOBILLO_DERECHO = true,
                        D_TOBILLO_IZQUIERDO = true,
                        D_TORACICA_DORSAL = true,
                        D_VERTEBRAL_CERVICAL = true,
                        D_VERTEBRAL_LUMBAR = true,
                        D_VERTEBRAL_TORACICA = true,
                        F_ANTEBRAZO_DERECHO = true,
                        F_ANTEBRAZO_IZQUIERDO = true,
                        F_ANTERIOR_CUELLO = true,
                        F_MUÑECA_ANTERIOR_DERECHA = true,
                        F_MUÑECA_ANTERIOR_IZQUIERDA = true,
                        F_AXILA_DERECHA = true,
                        F_AXILA_IZQUIERDA = true,
                        F_BAJO_VIENTRE_HIPOGASTRIO = true,
                        F_BRAZO_DERECHO = true,
                        F_BRAZO_IZQUIERDO = true,
                        F_CARA_DERECHA = true,
                        F_CARA_IZQUIERDA = true,
                        F_CLAVICULAR_DERECHA = true,
                        F_CLAVICULAR_IZQUIERDA = true,
                        F_CODO_DERECHO = true,
                        F_CODO_IZQUIERDO = true,
                        F_ENTREPIERNA_ANTERIOR_DERECHA = true,
                        F_ENTREPIERNA_ANTERIOR_IZQUIERDA = true,
                        F_EPIGASTRIO = true,
                        F_ESCROTO = true,
                        F_ESPINILLA_DERECHA = true,
                        F_ESPINILLA_IZQUIERDA = true,
                        F_FALANGES_MANO_DERECHA = true,
                        F_FALANGES_MANO_IZQUIERDA = true,
                        F_FALANGES_PIE_DERECHO = true,
                        F_FALANGES_PIE_IZQUIERDO = true,
                        F_HIPOCONDRIA_DERECHA = true,
                        F_HIPOCONDRIA_IZQUIERDA = true,
                        F_HOMBRO_DERECHO = true,
                        F_HOMBRO_IZQUIERDO = true,
                        F_INGUINAL_DERECHA = true,
                        F_INGUINAL_IZQUIERDA = true,
                        F_LATERAL_CABEZA_DERECHO = true,
                        F_LATERAL_CABEZA_IZQUIERDO = true,
                        F_MANDIBULA = true,
                        F_METACARPIANOS_DERECHA = true,
                        F_METACARPIANOS_IZQUIERDA = true,
                        F_METATARZIANA_DORSAL_DERECHO = true,
                        F_METATARZIANA_DORSAL_IZQUIERDO = true,
                        F_MUSLO_ANTERIOR_DERECHO = true,
                        F_MUSLO_ANTERIOR_IZQUIERDO = true,
                        F_NARIZ = true,
                        F_ORBITAL_DERECHO = true,
                        F_ORBITAL_IZQUIERDO = true,
                        F_OREJA_DERECHA = true,
                        F_OREJA_IZQUIERDA = true,
                        F_PENE_VAGINA = true,
                        F_PEZON_DERECHO = true,
                        F_PEZON_IZQUIERDO = true,
                        F_RODILLA_DERECHA = true,
                        F_RODILLA_IZQUIERDA = true,
                        F_TOBILLO_DERECHO = true,
                        F_TOBILLO_IZQUIERDO = true,
                        F_TORAX_CENTRAL = true,
                        F_TORAX_DERECHO = true,
                        F_TORAX_IZQUIERDO = true,
                        F_UMBILICAL_MESOGASTIO = true,
                        F_VACIO_DERECHA = true,
                        F_VACIO_IZQUIERDA = true,
                        F_VERTICE_CABEZA = true
                    });

                    #endregion
                }

                var _DetalleCentroActual = new SSP.Controlador.Catalogo.Justicia.cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                View.Report.LocalReport.ReportPath = "Reportes/rHojaEnfemeria.rdlc";
                View.Report.LocalReport.DataSources.Clear();

                #region Encabezado
                var Encabezado = new cEncabezado();
                Encabezado.TituloDos = _DetalleCentroActual != null ? !string.IsNullOrEmpty(_DetalleCentroActual.DESCR) ? _DetalleCentroActual.DESCR.Trim() : string.Empty : string.Empty;
                Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                Encabezado.ImagenFondo = Parametro.LOGO_ESTADO;
                Encabezado.PieDos = string.Format("{0} {1} {2} {3} / {4}",
                    SelectedIngreso != null ? SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.NOMBRE) ? SelectedIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                    SelectedIngreso != null ? SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.PATERNO) ? SelectedIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                    SelectedIngreso != null ? SelectedIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? SelectedIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                    SelectedIngreso.ID_ANIO,
                    SelectedIngreso.ID_IMPUTADO);
                #endregion

                if (lstSolucionesHojaEnfermeria != null && lstSolucionesHojaEnfermeria.Any())
                    if (!lstSolucionesHojaEnfermeria.TrueForAll(x => string.IsNullOrEmpty(x.Nombre)))
                        foreach (var item in lstSolucionesHojaEnfermeria)
                            if (item.Consecutivo == new int?())
                                item.Consecutivo = 1;//se usa un consecutivo como comodin, evita generar espacios vacios por la agrupacion de los elementos de la matriz

                var ds1 = new System.Collections.Generic.List<cEncabezado>();
                ds1.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                var ds2 = new System.Collections.Generic.List<cHojaEnfermeriaEncabezadoResporte>();
                ds2.Add(_DatosCabezera);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = lstSignosVitales != null ? lstSignosVitales.Any() ? lstSignosVitales.OrderBy(x => x.Generico).ToList() : new System.Collections.Generic.List<cHojaEnfermeriaSignosVitalesReporte>() : new System.Collections.Generic.List<cHojaEnfermeriaSignosVitalesReporte>();
                View.Report.LocalReport.DataSources.Add(rds3);

                Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = lstSolucionesHojaEnfermeria;
                View.Report.LocalReport.DataSources.Add(rds4);

                Microsoft.Reporting.WinForms.ReportDataSource rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = lstIngresosHojaEnfermeriaFinal;
                View.Report.LocalReport.DataSources.Add(rds5);

                Microsoft.Reporting.WinForms.ReportDataSource rds6 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds6.Name = "DataSet6";
                rds6.Value = lstEgresosHojaEnfermeriaFinal;
                View.Report.LocalReport.DataSources.Add(rds6);

                Microsoft.Reporting.WinForms.ReportDataSource rds7 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds7.Name = "DataSet7";
                rds7.Value = listaMedicamentosHojaEnfermeria;
                View.Report.LocalReport.DataSources.Add(rds7);

                Microsoft.Reporting.WinForms.ReportDataSource rds8 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds8.Name = "DataSet8";
                rds8.Value = lstNotasEnfermeria;
                View.Report.LocalReport.DataSources.Add(rds8);

                Microsoft.Reporting.WinForms.ReportDataSource rds9 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds9.Name = "DataSet9";
                rds9.Value = listaUlceras;
                View.Report.LocalReport.DataSources.Add(rds9);

                Microsoft.Reporting.WinForms.ReportDataSource rds10 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds10.Name = "DataSet10";
                rds10.Value = listaL;
                View.Report.LocalReport.DataSources.Add(rds10);

                Microsoft.Reporting.WinForms.ReportDataSource rds11 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds11.Name = "DataSet11";
                rds11.Value = lstCateteresReporte;
                View.Report.LocalReport.DataSources.Add(rds11);

                Microsoft.Reporting.WinForms.ReportDataSource rds12 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds12.Name = "DataSet12";
                rds12.Value = lstSondasReporte;
                View.Report.LocalReport.DataSources.Add(rds12);

                Microsoft.Reporting.WinForms.ReportDataSource rds13 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds13.Name = "DataSet13";
                rds13.Value = lstRayosXReporte;
                View.Report.LocalReport.DataSources.Add(rds13);

                Microsoft.Reporting.WinForms.ReportDataSource rds14 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds14.Name = "DataSet14";
                rds14.Value = listLaboratoriosReporte;
                View.Report.LocalReport.DataSources.Add(rds14);

                View.Report.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                View.Report.RefreshReport();
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }


        #region Kardex
        private void GenerarReporte(string reporte)
        {
            try
            {
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                listaemp = new List<EmpalmeParticipante>();
                #region Reporte
                #region [encabezado]
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = !string.IsNullOrEmpty(Parametro.ENCABEZADO1) ? Parametro.ENCABEZADO1.Trim() : string.Empty,
                    Encabezado2 = !string.IsNullOrEmpty(Parametro.ENCABEZADO2) ? Parametro.ENCABEZADO2.Trim() : string.Empty,
                    Encabezado3 = !string.IsNullOrEmpty(Parametro.ENCABEZADO3) ? Parametro.ENCABEZADO3.Trim() : string.Empty,
                    Titulo = "KARDEX INTERNO ",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Centro = centro != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty
                });

                ReporteKdx.LocalReport.ReportPath = "Reportes/rKardexInterno.rdlc";
                ReporteKdx.LocalReport.DataSources.Clear();

                var rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                ReporteKdx.LocalReport.DataSources.Add(rds2);
                #endregion
                #region [primera parte]
                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = new List<ReporteKardexInterno>() { new ReporteKardexInterno(){ 
                 Interno= (SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.PATERNO != null ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty) + " " + (SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.MATERNO != null ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty) + " " +
                 (SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.NOMBRE != null ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty),
                  Expediente = SelectIngreso.ID_ANIO + "\\" + SelectIngreso.ID_IMPUTADO,
                  Ingreso = SelectIngreso.ID_INGRESO.ToString(),
                  Ubicacion = string.Format("{0}-{1}-{2}-{3}",
                                                SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,//0
                                                SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,//1
                                                SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,//2
                                                SelectIngreso.CAMA != null ? SelectIngreso.CAMA.ID_CAMA.ToString().Trim() : string.Empty),//3
                  FechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString() : string.Empty,
                  Planimetria = SelectIngreso.CAMA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty,
                  Avance = AvanceTratamiento,
                  HorasTratamiento = HorasTratamiento,
                  Estatus = SelectIngreso.ESTATUS_ADMINISTRATIVO != null ? !string.IsNullOrEmpty(SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR) ? SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR.Trim() : string.Empty : string.Empty,
                  HistorialSanciones = (reporte.Equals("KARDEX") || reporte.Equals("SANCIONES")) ?SelectIngreso.GRUPO_PARTICIPANTE.Where(w => w.GRUPO_PARTICIPANTE_CANCELADO.Any()).Any() ? "Tiene Historial": string.Empty: string.Empty
                }};

                ReporteKdx.LocalReport.DataSources.Add(rds1);
                #endregion
                var rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = new List<ReporteKardexInternoActividades>();
                if (reporte.Equals("KARDEX") || reporte.Equals("ACTIVIDADES"))
                {
                    #region [segunda parte]
                    var preNotaTecnica = new List<ReporteKardexInternoActividades>();
                    string NotaTecnica, Acredita;
                    foreach (var item in SelectIngreso.GRUPO_PARTICIPANTE.OrderBy(o => o.GRUPO == null).ThenBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ThenBy(t => t.GRUPO != null ? t.GRUPO.DESCR : string.Empty))
                    {
                        NotaTecnica = Acredita = string.Empty;
                        var nt = item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault();
                        if (nt != null)
                        {
                            NotaTecnica = nt.NOTA;
                            Acredita = nt.NOTA_TECNICA_ESTATUS != null ? nt.NOTA_TECNICA_ESTATUS.DESCR : string.Empty;
                        }
                        else
                        {
                            NotaTecnica = Acredita = "NO CAPTURADA";
                        }

                        preNotaTecnica.Add(new ReporteKardexInternoActividades()
                        {
                            ID_Grupo = item.GRUPO != null ? item.GRUPO.ID_GRUPO.ToString() : string.Empty,
                            Eje = item.EJE1 != null ? item.EJE1.DESCR : string.Empty,
                            Programa = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE) ? item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            Actividad = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty,
                            Grupo = item.GRUPO != null ? item.GRUPO.DESCR : item.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                            //EntityGRUPO = item.GRUPO,
                            Inicio = item.GRUPO != null ? item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.ToShortDateString() : string.Empty : string.Empty,
                            Fin = item.GRUPO != null ? item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value.ToShortDateString() : string.Empty : string.Empty,
                            Asistencia = ObtenerPorcentajeAsistencia(item, SelectIngreso.GRUPO_PARTICIPANTE),
                            Nota_Tecnica = NotaTecnica,
                            Acreditado = Acredita,
                            //Nota_Tecnica = item.NOTA_TECNICA.Count != 0 ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().NOTA : "NO CAPTURADA",
                            //Acreditado = item.NOTA_TECNICA.Count != 0 ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().NOTA_TECNICA_ESTATUS.DESCR : "NO CAPTURADA"
                        });
                    }
                    rds3.Value = preNotaTecnica;

                    #endregion
                }

                ReporteKdx.LocalReport.DataSources.Add(rds3);
                var rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = new List<ReporteKardexInternoActividades>();
                if (reporte.Equals("KARDEX") || reporte.Equals("HORARIO"))
                {
                    #region [tercera parte]
                    //var rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    //rds4.Name = "DataSet4";
                    var preNotaTecnica2 = new List<ReporteKardexInternoActividades>();
                    string NotaTecnica, Acredita;
                    foreach (var item in SelectIngreso.GRUPO_PARTICIPANTE.OrderBy(o => o.GRUPO == null).ThenBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ThenBy(t => t.GRUPO != null ? t.GRUPO.DESCR : string.Empty))
                    {
                        NotaTecnica = Acredita = string.Empty;
                        var nt = item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault();
                        if (nt != null)
                        {
                            NotaTecnica = nt.NOTA;
                            Acredita = nt.NOTA_TECNICA_ESTATUS != null ? nt.NOTA_TECNICA_ESTATUS.DESCR : string.Empty;
                        }
                        else
                        {
                            NotaTecnica = Acredita = "NO CAPTURADA";
                        }

                        preNotaTecnica2.Add(new ReporteKardexInternoActividades()
                        {
                            ID_Grupo = item.GRUPO != null ? item.GRUPO.ID_GRUPO.ToString() : string.Empty,
                            Eje = item.EJE1 != null ? item.EJE1.DESCR : string.Empty,
                            Programa = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE) ? item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                            Actividad = item.ACTIVIDAD != null ? item.ACTIVIDAD.DESCR : string.Empty,
                            Grupo = item.GRUPO != null ? item.GRUPO.DESCR : item.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                            //EntityGRUPO = item.GRUPO,
                            Inicio = item.GRUPO != null ? item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.ToShortDateString() : string.Empty : string.Empty,
                            Fin = item.GRUPO != null ? item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value.ToShortDateString() : string.Empty : string.Empty,
                            Asistencia = ObtenerPorcentajeAsistencia(item, SelectIngreso.GRUPO_PARTICIPANTE),
                            Nota_Tecnica = NotaTecnica,
                            Acreditado = Acredita,
                            //Nota_Tecnica = item.NOTA_TECNICA.Count != 0 ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().NOTA : "NO CAPTURADA",
                            //Acreditado = item.NOTA_TECNICA.Count != 0 ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().NOTA_TECNICA_ESTATUS.DESCR : "NO CAPTURADA"
                        });
                    }
                    rds4.Value = preNotaTecnica2;

                    //ReporteKdx.LocalReport.DataSources.Add(rds4);
                    #endregion
                }

                ReporteKdx.LocalReport.DataSources.Add(rds4);

                var rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = new List<ReporteKardexHorasEmpalmadas>();
                if (reporte.Equals("KARDEX") || reporte.Equals("EMPALMES"))
                {
                    #region [cuarta parte]


                    var IdGH = new Nullable<DateTime>();
                    var cont = -1;
                    var horariolist = new cGrupoAsistencia().GetData().Where(w => w.GRUPO_PARTICIPANTE.ING_ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.GRUPO_PARTICIPANTE.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.GRUPO_PARTICIPANTE.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).OrderByDescending(o => o.GRUPO_HORARIO.HORA_INICIO).ToList();

                    if (horariolist.Count > 1)
                        foreach (var item in horariolist)
                        {
                            if (item == null)
                                continue;
                            if (IdGH == item.GRUPO_HORARIO.HORA_INICIO.Value.Date)
                            {
                                listaemp[cont].ListHorario.Add(new ListaEmpalmes()
                                {
                                    ACTIVIDAD = item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.GRUPO != null ? item.GRUPO_HORARIO.GRUPO.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR) ? item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR : string.Empty : string.Empty : string.Empty : string.Empty,
                                    EJE = item.GRUPO_PARTICIPANTE != null ? item.GRUPO_PARTICIPANTE.EJE1 != null ? !string.IsNullOrEmpty(item.GRUPO_PARTICIPANTE.EJE1.DESCR) ? item.GRUPO_PARTICIPANTE.EJE1.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                                    ELEGIDA = item.EMP_APROBADO == 1,
                                    GRUPO = item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.GRUPO != null ? !string.IsNullOrEmpty(item.GRUPO_HORARIO.GRUPO.DESCR) ? item.GRUPO_HORARIO.GRUPO.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                                    HORARIO = item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.HORA_INICIO.HasValue ? item.GRUPO_HORARIO.HORA_INICIO.Value.ToShortTimeString() : string.Empty : string.Empty
                                    + " - " +
                                    item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.HORA_TERMINO.HasValue ? item.GRUPO_HORARIO.HORA_TERMINO.Value.ToShortTimeString() : string.Empty : string.Empty,
                                    PROGRAMA = item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.GRUPO != null ? item.GRUPO_HORARIO.GRUPO.ACTIVIDAD != null ? item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.TIPO_PROGRAMA != null ? item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE : string.Empty : string.Empty : string.Empty : string.Empty
                                });

                                listaemp[cont].ListHorario = listaemp[cont].ListHorario.OrderByDescending(o => o.ELEGIDA).ThenBy(t => t.HORARIO).ToList();
                                continue;
                            }

                            IdGH = item.GRUPO_HORARIO.HORA_INICIO.Value.Date;

                            listaemp.Add(new EmpalmeParticipante()
                            {
                                HEADEREXPANDER = item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.HORA_INICIO.HasValue ? item.GRUPO_HORARIO.HORA_INICIO.Value.Date.ToShortDateString() : string.Empty : string.Empty,
                                ListHorario = new List<ListaEmpalmes>() { new ListaEmpalmes()
                                    {
                                        ACTIVIDAD = item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.GRUPO != null ? item.GRUPO_HORARIO.GRUPO.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR) ? item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR : string.Empty : string.Empty : string.Empty : string.Empty,
                                        EJE = item.GRUPO_PARTICIPANTE != null ? item.GRUPO_PARTICIPANTE.EJE1 != null ? !string.IsNullOrEmpty(item.GRUPO_PARTICIPANTE.EJE1.DESCR) ? item.GRUPO_PARTICIPANTE.EJE1.DESCR : string.Empty : string.Empty : string.Empty,
                                        ELEGIDA = item.EMP_APROBADO == 1,
                                        GRUPO = item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.GRUPO != null ? !string.IsNullOrEmpty(item.GRUPO_HORARIO.GRUPO.DESCR) ? item.GRUPO_HORARIO.GRUPO.DESCR : string.Empty : string.Empty : string.Empty,
                                        HORARIO = item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.HORA_INICIO.HasValue ? item.GRUPO_HORARIO.HORA_INICIO.Value.ToShortTimeString() : string.Empty : string.Empty 
                                        + " - " +
                                        item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.HORA_TERMINO.HasValue ? item.GRUPO_HORARIO.HORA_TERMINO.Value.ToShortTimeString() : string.Empty : string.Empty,
                                        PROGRAMA = item.GRUPO_HORARIO != null ? item.GRUPO_HORARIO.GRUPO != null ? item.GRUPO_HORARIO.GRUPO.ACTIVIDAD != null ? item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE) ? item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty
                                    }}
                            });

                            cont++;
                        }

                    rds5.Value = listaemp.Where(w => w.ListHorario.Count > 1).Select(s => new ReporteKardexHorasEmpalmadas { Dia_Header = s.HEADEREXPANDER }).OrderBy(o => Convert.ToDateTime(o.Dia_Header)).ToList();
                    #endregion
                }

                ReporteKdx.LocalReport.DataSources.Add(rds5);

                ReporteKdx.LocalReport.SubreportProcessing += (s, e) =>
                {
                    #region [rKardexHorarioInterno]
                    if (e.ReportPath.Equals("rKardexHorarioInterno", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var intid = Convert.ToInt16(e.Parameters[0].Values[0]);
                        var preHorarioParticipante = new List<ReporteKardexInternoHorario>();
                        //var grupo = new cGrupo().GetData().Where(w => w.ID_GRUPO == intid).FirstOrDefault();
                        foreach (var item in SelectIngreso.GRUPO_PARTICIPANTE.Where(w => w.GRUPO != null && w.GRUPO.ID_GRUPO == intid).OrderBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ThenBy(t => t.GRUPO != null ? t.GRUPO.DESCR : string.Empty))
                        {
                            if (item.GRUPO != null)
                                foreach (var subitem in item.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO))
                                    preHorarioParticipante.Add(new ReporteKardexInternoHorario()
                                    {
                                        ID_Grupo = item.GRUPO != null ? item.GRUPO.ID_GRUPO.ToString() : string.Empty,
                                        EJE = item.EJE1 != null ? item.EJE1.DESCR : string.Empty,
                                        PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE) ? item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE : string.Empty : string.Empty : string.Empty,
                                        ACTIVIDAD = item.ACTIVIDAD != null ? item.ACTIVIDAD.DESCR : string.Empty,
                                        FECHA = subitem.HORA_INICIO.HasValue ? subitem.HORA_INICIO.Value.ToShortDateString() : string.Empty,
                                        HORARIO = string.Format("{0} - {1}", subitem.HORA_INICIO.HasValue ? subitem.HORA_INICIO.Value.ToShortTimeString() : string.Empty,
                                        subitem.HORA_TERMINO.HasValue ? subitem.HORA_TERMINO.Value.ToShortTimeString() : string.Empty),
                                        GRUPO = item.GRUPO != null ? item.GRUPO.DESCR : item.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                                        ASISTENCIA = subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ASISTENCIA == 1,
                                        INICIO = item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.ToShortDateString() : string.Empty,
                                        FIN = item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value.ToShortDateString() : string.Empty,
                                        ESTATUS = subitem.ESTATUS != 1 ? subitem.GRUPO_HORARIO_ESTATUS.DESCR : subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ESTATUS != 1 ? subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().GRUPO_ASISTENCIA_ESTATUS.DESCR : string.Empty,
                                        FechaHorario = subitem.HORA_INICIO,
                                        ShowCheck = subitem.ESTATUS != 1 ? Visibility.Collapsed : subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ESTATUS != 1 ? Visibility.Collapsed : Visibility.Visible,
                                        ShowLabel = subitem.ESTATUS != 1 ? Visibility.Visible : subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ESTATUS != 1 ? Visibility.Visible : Visibility.Collapsed
                                    });
                        }

                        //var ds = new ReportDataSource("DataSet1", lSector);
                        e.DataSources.Add(new ReportDataSource("DataSet1", preHorarioParticipante));

                        var hp = preHorarioParticipante.FirstOrDefault();
                        if (hp != null)
                        {
                            e.DataSources.Add(new ReportDataSource("DataSet2", new List<ReporteKardexInternoHorario>() {
                       new ReporteKardexInternoHorario()
                                    {
                                        ACTIVIDAD = preHorarioParticipante.FirstOrDefault().ACTIVIDAD,
                                        GRUPO = preHorarioParticipante.FirstOrDefault().GRUPO,
                                        INICIO = preHorarioParticipante.FirstOrDefault().INICIO,
                                        FIN = preHorarioParticipante.FirstOrDefault().FIN,
                                        nASISTENCIA = preHorarioParticipante.Any() ? preHorarioParticipante.Where(w => w.ASISTENCIA.Value).Count().ToString() : string.Empty,
                                        FALTAS = preHorarioParticipante.Any() ? preHorarioParticipante.Where(w => !w.ASISTENCIA.Value && w.FechaHorario < Fechas.GetFechaDateServer).Count().ToString() : string.Empty,
                                        JUSTIFICADAS = preHorarioParticipante.Any() ? preHorarioParticipante.Where(w => w.ShowLabel == Visibility.Visible).Count().ToString() : string.Empty
                                    }
                        }));
                        }
                    }
                    #endregion
                    else
                        #region [srEmpalme]
                        if (e.ReportPath.Equals("srEmpalme", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var dateid = e.Parameters[0].Values[0];

                            e.DataSources.Add(new ReportDataSource("DataSet1", listaemp.Where(w => w.HEADEREXPANDER.Equals(dateid) && w.ListHorario.Count > 1).SelectMany(se => se.ListHorario).Select(se => new ReporteKardexHorasEmpalmadas
                            {
                                EJE = se.EJE,
                                PROGRAMA = se.PROGRAMA,
                                ACTIVIDAD = se.ACTIVIDAD,
                                GRUPO = se.GRUPO,
                                Horario = se.HORARIO,
                                Elegida = se.ELEGIDA
                            }).OrderByDescending(o => o.Elegida).ToList()));

                        }
                        #endregion
                        else
                            #region [srSanciones]
                            if (e.ReportPath.Equals("srSanciones", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var listsanciones = new cGrupoParticipanteCancelado().GetData().Where(w => w.GRUPO_PARTICIPANTE.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.GRUPO_PARTICIPANTE.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.GRUPO_PARTICIPANTE.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).ToList();

                                e.DataSources.Add(new ReportDataSource("DataSet1", listsanciones.Select(se => new ReporteKardexSanciones
                                {
                                    EJE = se.GRUPO_PARTICIPANTE != null ? se.GRUPO_PARTICIPANTE.EJE1 != null ? se.GRUPO_PARTICIPANTE.EJE1.DESCR : string.Empty : string.Empty,
                                    PROGRAMA = se.GRUPO_PARTICIPANTE != null ? se.GRUPO_PARTICIPANTE.ACTIVIDAD != null ? se.GRUPO_PARTICIPANTE.ACTIVIDAD.TIPO_PROGRAMA != null ? se.GRUPO_PARTICIPANTE.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE : string.Empty : string.Empty : string.Empty,
                                    ACTIVIDAD = se.GRUPO_PARTICIPANTE != null ? se.GRUPO_PARTICIPANTE.ACTIVIDAD != null ? se.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR : string.Empty : string.Empty,
                                    GRUPO = getNombreGrupo(se.ID_GRUPO),
                                    RESPONSABLE = getNombreResponsable(se.ID_GRUPO),
                                    SOLICITUD_FECHA = se.SOLICITUD_FEC.HasValue ? se.SOLICITUD_FEC.Value.ToShortDateString() : string.Empty,
                                    RESPUESTA_FECHA = se.RESPUESTA_FEC.HasValue ? se.RESPUESTA_FEC.Value.ToShortDateString() : string.Empty,
                                    MOTIVO = se.MOTIVO,
                                    RESPUESTA = se.RESPUESTA,
                                    ESTATUS = se.GRUPO_PARTICIPANTE_ESTATUS != null ? se.GRUPO_PARTICIPANTE_ESTATUS.DESCR : string.Empty
                                }).ToList()));
                            }
                            #endregion
                };

                #region Parametros
                var listgroup = new cGrupo().GetData().Select(s => s.ID_GRUPO).Cast<String>().ToArray();
                ReporteKdx.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("ID_GRUPOParam", listgroup));
                #endregion

                VisibleDatosKardexReporte = Visibility.Visible;

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReporteKdx.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    ReporteKdx.RefreshReport();
                }));
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar reporte", ex);
            }
        }

        private string getNombreResponsable(short ID_GRUPO)
        {
            var grupodesc = new cGrupo().GetData().Where(w => w.ID_GRUPO == ID_GRUPO).FirstOrDefault();
            return grupodesc != null ? (string.IsNullOrEmpty(grupodesc.PERSONA.NOMBRE) ? string.Empty : grupodesc.PERSONA.NOMBRE.Trim()) + " " + (string.IsNullOrEmpty(grupodesc.PERSONA.PATERNO) ? string.Empty : grupodesc.PERSONA.PATERNO.Trim()) + " " + (string.IsNullOrEmpty(grupodesc.PERSONA.MATERNO) ? string.Empty : grupodesc.PERSONA.MATERNO.Trim()) : string.Empty;
        }

        private string getNombreGrupo(short ID_GRUPO)
        {
            var grupodesc = new cGrupo().GetData().Where(w => w.ID_GRUPO == ID_GRUPO).FirstOrDefault();
            return grupodesc != null ? grupodesc.DESCR : string.Empty;
        }

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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del participante.", ex);
                return string.Empty;
            }
        }

        private void ImprimirKardex()
        {
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var datosReporte = new List<cReporteDatos>();
                var GeneralesKardex = new List<cKardex>();
                var JuridicoKardex = new List<cKardexJuridico>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = centro != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty,
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2,
                    Titulo = "Curricula de Tratamiento de Reinserción Social",
                    Centro = centro != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty
                });

                #region Apodo
                var apodo = string.Empty;
                if (SelectExpediente.APODO != null)
                {
                    foreach (var a in SelectExpediente.APODO)
                    {
                        if (!string.IsNullOrEmpty(apodo))
                            apodo = apodo + ", ";
                        apodo = apodo + a.APODO1.Trim();
                    }
                }
                #endregion

                #region Ubicacion
                var ubicacion = string.Empty;
                if (SelectIngreso.CAMA != null)
                {
                    ubicacion = string.Format("{0}-{1}-{2}-{3}",
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty,
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.ID_CAMA : new short());
                }
                #endregion

                #region Causa Penal
                var causa_penal = string.Empty;
                if (SelectIngreso.CAUSA_PENAL != null)
                {
                    int[] estatus = { 0, 1, 4, 6 };
                    foreach (var cp in SelectIngreso.CAUSA_PENAL.Where(w => estatus.Contains(w.ID_ESTATUS_CP.Value)))
                    {
                        if (!string.IsNullOrEmpty(causa_penal))
                            causa_penal = causa_penal + ", ";
                        causa_penal = causa_penal + string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO);
                    }
                }
                #endregion

                #region foto
                byte[] foto = new Imagenes().getImagenPerson();
                if (SelectIngreso.INGRESO_BIOMETRICO != null)
                {
                    var b = SelectIngreso.INGRESO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO);
                    if (b != null)
                    {
                        foto = b.BIOMETRICO;
                    }
                }
                #endregion

                GeneralesKardex.Add(new cKardex()
                {
                    EXPEDIENTE = string.Format("{0}/{1}", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO),
                    NOMBRE = string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(SelectExpediente.NOMBRE) ? SelectExpediente.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectExpediente.PATERNO) ? SelectExpediente.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectExpediente.MATERNO) ? SelectExpediente.MATERNO.Trim() : string.Empty),
                    EDAD = new Fechas().CalculaEdad(SelectExpediente.NACIMIENTO_FECHA, hoy),
                    APODO = apodo,
                    FECHA_NACIMIENTO = SelectExpediente.NACIMIENTO_FECHA.HasValue ? SelectExpediente.NACIMIENTO_FECHA.Value : Fechas.GetFechaDateServer,
                    SEXO = SelectExpediente.SEXO == "F" ? "FEMENINO" : "MASCULINO",
                    ESTADO_CIVIL = SelectIngreso.ESTADO_CIVIL != null ? !string.IsNullOrEmpty(SelectIngreso.ESTADO_CIVIL.DESCR) ? SelectIngreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty,
                    CENTRO = centro != null ? !string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim() : string.Empty : string.Empty,
                    ESTATUS_JURIDICO = SelectIngreso.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(SelectIngreso.CLASIFICACION_JURIDICA.DESCR) ? SelectIngreso.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty,
                    UBICACION = ubicacion,
                    NO_INGRESO = SelectIngreso.ID_INGRESO,
                    CAUSA_PENAL = causa_penal,
                    CENTRO_PROCEDENCIA = "NINGUNO",
                    FOTO = foto,
                    FECHA_IMPRESION = hoy
                });

                var sentencia = new cSentencia().ObtenerSentenciaIngreso(
                    SelectIngreso.ID_CENTRO,
                    SelectIngreso.ID_ANIO,
                    SelectIngreso.ID_IMPUTADO,
                    SelectIngreso.ID_INGRESO,
                    hoy);

                if (sentencia != null)
                {
                    foreach (var s in sentencia)
                    {
                        #region 3 / 5 partes
                        DateTime? fecha_cumple = null;
                        var cumple = "NO";
                        int dias = (((s.S_ANIO * 365) + (s.S_MES * 30) + s.S_DIA) / 5) * 3;
                        int dias_cumplidos = (s.C_ANIO * 365) + (s.C_MES * 30) + s.C_DIA;
                        if (dias_cumplidos > dias)
                        {
                            cumple = "SI";
                        }
                        else
                        {
                            fecha_cumple = hoy.Date;
                            fecha_cumple = fecha_cumple.Value.AddDays(dias - dias_cumplidos);
                        }
                        #endregion

                        #region Fecha Libertad
                        var fecha_libertad = hoy;
                        if (s.PC_ANIO > 0)
                            fecha_libertad = fecha_libertad.AddYears(s.PC_ANIO);
                        if (s.PC_MES > 0)
                            fecha_libertad = fecha_libertad.AddMonths(s.PC_MES);
                        if (s.PC_DIA > 0)
                            fecha_libertad = fecha_libertad.AddDays(s.PC_DIA);

                        #endregion

                        JuridicoKardex.Add(new cKardexJuridico()
                        {
                            SENTENCIA_ANIOS = s.S_ANIO,
                            SENTENCIA_MESES = s.S_MES,
                            SENTENCIA_DIAS = s.S_DIA,
                            COMPURGADO_ANIOS = s.C_ANIO,
                            COMPURGADO_MESES = s.C_MES,
                            COMPURGADO_DIAS = s.C_DIA,
                            POR_COMPURGADO_ANIOS = s.PC_ANIO,
                            POR_COMPURGADO_MESES = s.PC_MES,
                            POR_COMPURGADO_DIAS = s.PC_DIA,
                            CUMPLE_3_5_PARTES = cumple,
                            FECHA_3_5_PARTES = fecha_cumple,
                            FECHA_LIBERTAD = fecha_libertad
                        });
                    }
                }

                var datos = new cGrupoParticipante().ObtenerReporteKardex(
                    SelectIngreso.ID_CENTRO,
                    SelectIngreso.ID_ANIO,
                    SelectIngreso.ID_IMPUTADO,
                    SelectIngreso.ID_INGRESO).Select(w => new cKardexContenido()
                    {
                        ID_CONSEC = w.ID_CONSEC,
                        ID_CENTRO = w.ID_CENTRO,
                        ID_ANIO = w.ID_ANIO,
                        ID_IMPUTADO = w.ID_IMPUTADO,
                        ID_INGRESO = w.ID_INGRESO,
                        ID_DEPARTAMENTO = w.ID_DEPARTAMENTO,
                        DEPARTAMENTO = w.DEPARTAMENTO,
                        ID_TIPO_PROGRAMA = w.ID_TIPO_PROGRAMA,
                        TIPO_PROGRAMA = w.TIPO_PROGRAMA,
                        ESTATUS = w.ESTATUS,
                        ID_EJE = w.ID_EJE,
                        EJE = w.EJE,
                        GRUPO = w.GRUPO,
                        FEC_INICIO = w.FEC_INICIO,
                        CALIFICACION = w.CALIFICACION,
                        FEC_REGISTRO = w.FEC_REGISTRO,
                        FEC_FIN = w.FEC_FIN,
                        RECURRENCIA = w.RECURRENCIA,
                        //HORA_INICIO =w.HORA_INICIO,
                        //HORA_TERMINO =w.HORA_TERMINO
                        ID_ACTIVIDAD = w.ID_ACTIVIDAD,
                        ACTIVIDAD = w.ACTIVIDAD,
                        ID_GRUPO = w.ID_GRUPO
                    });

                var horario = new cGrupoParticipante().ObtenerReporteKardexHorario(
                    SelectIngreso.ID_CENTRO,
                    SelectIngreso.ID_ANIO,
                    SelectIngreso.ID_IMPUTADO,
                    SelectIngreso.ID_INGRESO).Select(w => new cKardexHorario()
                    {
                        ID_CENTRO = w.ID_CENTRO,
                        ID_TIPO_PROGRAMA = w.ID_TIPO_PROGRAMA,
                        ID_ACTIVIDAD = w.ID_ACTIVIDAD,
                        ID_GRUPO = w.ID_GRUPO,
                        ID_DIA = w.ID_DIA,
                        DIA = w.DIA,
                        ID_GRUPO_HORARIO = w.ID_GRUPO_HORARIO,
                        HORA_INICIO = w.HORA_INICIO,
                        HORA_TERMINO = w.HORA_TERMINO
                    });

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReporteKdx.Reset();
                }));

                ReporteKdx.LocalReport.ReportPath = "Reportes/rKardex.rdlc";
                ReporteKdx.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet2";
                rds1.Value = datosReporte;
                ReporteKdx.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet1";
                rds2.Value = GeneralesKardex;
                ReporteKdx.LocalReport.DataSources.Add(rds2);

                ReportDataSource rds3 = new ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = JuridicoKardex;
                ReporteKdx.LocalReport.DataSources.Add(rds3);

                ReportDataSource rds4 = new ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = datos;
                ReporteKdx.LocalReport.DataSources.Add(rds4);

                #region Subreporte
                ReporteKdx.LocalReport.SubreportProcessing += (s, e) =>
                {
                    ReportDataSource sr = new ReportDataSource("DataSet1", horario);
                    e.DataSources.Add(sr);
                };
                #endregion

                ReporteKdx.ShowExportButton = false;
                ReporteKdx.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                ReporteKdx.RefreshReport();

                VisibleDatosKardexReporte = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir el kardex.", ex);
            }
        }
        #endregion

        #endregion

    }

    public class DocumentoExpedienteImputado
    {
        public DateTime? FECHA { get; set; }
        public string TIPO_DOCTO { get; set; }
        public ABOGADO_ING_DOCTO ABOGADO_ING_DOCTO { get; set; }
        public ABOGADO_CP_DOCTO ABOGADO_CP_DOCTO { get; set; }
        public AMPARO_DIRECTO_DOCTO AMPARO_DIRECTO_DOCTO { get; set; }
        public AMPARO_INCIDENTE_DOCTO AMPARO_INCIDENTE_DOCTO { get; set; }
        public AMPARO_INDIRECTO_DOCTO AMPARO_INDIRECTO_DOCTO { get; set; }
        public CAUSA_PENAL_DOCTO CAUSA_PENAL_DOCTO { get; set; }
        public IMPUTADO_DOCUMENTO IMPUTADO_DOCUMENTO { get; set; }
        public RECURSO_DOCTO RECURSO_DOCTO { get; set; }
        public VISITA_DOCUMENTO VISITA_DOCUMENTO { get; set; }
        public EXCARCELACION_DESTINO EXCARCELACION_DESTINO { get; set; }
        public EXCARCELACION EXCARCELACION { get; set; }
        public HISTORIA_CLINICA_DOCUMENTO HISTORIA_CLINICA_DOCTO { get; set; }
        public PERSONALIDAD_DETALLE PERSONALIDAD_DETALLE { get; set; }
        public INGRESO_PERTENENCIA INGRESO_PERTENENCIA { get; set; }
        //TRASLADO
    }
}
