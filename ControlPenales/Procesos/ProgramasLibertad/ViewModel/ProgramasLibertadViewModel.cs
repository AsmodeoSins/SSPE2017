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
using Cogent.Biometrics;
using System.Runtime.InteropServices;
using DPUruNet;
using System.Drawing;
using ControlPenales.BiometricoServiceReference;
using System.Windows.Media;

namespace ControlPenales
{
    partial class ProgramasLibertadViewModel : FingerPrintScanner
    {
        #region constructor
        public ProgramasLibertadViewModel() { }
        #endregion

        #region metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_menu":
                    if (!pConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    auxProcesoLibertad = SelectedProcesoLibertad;
                    SelectExpediente = null;
                    SelectedProcesoLibertad = null;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                    break;
                case "nueva_busqueda":
                    LimpiarBusqueda();
                    break;
                case "buscar_salir":
                    SelectedProcesoLibertad = auxProcesoLibertad;
                    if (SelectedProcesoLibertad != null)
                    {
                        SelectExpediente = SelectedProcesoLibertad.IMPUTADO;
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                    break;
                case "buscar_visible":
                    if (!pConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    ClickEnter();
                    break;
                case "nuevo_expediente":
                    break;
                case "buscar_nueva_medida":
                    //SelectMJ = null; //Se agrega nueva medida
                    //Obtener();
                    break;
                case "buscar_seleccionar":
                    if (!pConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    //if (SelectMJ != null)
                    //{
                    //   ValidacionesLiberado();
                    if (SelectedProcesoLibertad != null)
                    {
                          Obtener();
                    //    ValidacionDatosFamiliar();
                    //    OnPropertyChanged("TextLugarEntrevista");
                    //    OnPropertyChanged("TextFechaEntrv");
                    //    OnPropertyChanged("InicioDiaDomingo");
                    //    OnPropertyChanged("TextNombreFamiliar");
                    //    OnPropertyChanged("SelectParentesco");
                    //    OnPropertyChanged("TextTelefonoFamiliar");
                    //    OnPropertyChanged("TextCalleFamiliar");
                    //    OnPropertyChanged("TextNumExteriorFamiliar");
                    //    OnPropertyChanged("TextDescripcionEntrv");
                    //    OnPropertyChanged("TextTecnicasUtilizadas");
                    //    OnPropertyChanged("TextExamenMental");
                    //    OnPropertyChanged("TextPersonalidad");
                    //    OnPropertyChanged("TextNuceloFamPrimario");
                    //    OnPropertyChanged("TextNuceloFamSecundario");
                    //    OnPropertyChanged("TextObsrv");
                    //    OnPropertyChanged("TextSugerencia");
                    //    StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso en libertad.");
                    }
                    break;
                case "reporte_menu":
                    //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGENDA_LIBERTAD);
                    
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ProgramasLibertadView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ProgramasLibertadViewModel();
                    break;           
                case "guardar_menu":
                    Save();
                    break;
                case "Open442":
                    if (!pConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                    this.ShowIdentification();
                    break;
                case "addSeguimiento":
                    VisibleDetalle = Visibility.Visible;
                    ValidacionAgenda();
                    AgendaLibertadDetalle = null;
                    GetEntidadAgendaSeguimiento();
                    break;
                case "editSeguimiento":
                    VisibleDetalle = Visibility.Visible;
                    ValidacionAgenda();
                    GetEntidadAgendaSeguimiento();
                    break;
                case "viewSeguimiento":
                    var v = new AgendasView();
                    v.DataContext = new AgendasViewModel(AgendaLibertadDetalle);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    v.Owner = PopUpsViewModels.MainWindow;
                    v.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    v.Show();
                    break;
                case "cerrar_agenda":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGENDA_LIBERTAD);
                    break;
                case "asignacion":
                    LimpiarOficios();
                    if (SelectedProcesoLibertad != null)
                    {
                        OANUC = SelectedProcesoLibertad.NUC;
                        OACPAnio = SelectedProcesoLibertad.CP_ANIO;
                        OACPFolio = SelectedProcesoLibertad.CP_FOLIO;
                        if (AgendaLibertadDetalle != null)
                        {
                            if (AgendaLibertadDetalle.AGENDA_ACT_LIB_DETALLE != null)
                            {
                                OANoJornadasLetra = string.Format("{0} JORNADAS", new cNumLetra().Convertir(AgendaLibertadDetalle.AGENDA_ACT_LIB_DETALLE.Count().ToString(), true));
                            }
                        }
                    }
                    ValidacionOficioAsignacion();
                    PopulateInformacionJuridica(1);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OFICIO_ASIGNACION_NSJP);
                    break;
                case "agregar_oficio_asignacion":
                    AgregarOficioAsignacion(); 
                    break;
                case "ver_oficio_asignacion":
                    Oficio_Asignacion = true;
                    Oficio_Conclusion = false;
                    Oficio_Baja = false;
                    ImprimirReportes();
                    break;
                case "cancelar_oficio_asignacion":
                    LimpiarOficios();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OFICIO_ASIGNACION_NSJP);
                    break;
                case "conclusion":
                    LimpiarOficios();
                    if (SelectedProcesoLibertad != null)
                    {
                        OCCPAnio = SelectedProcesoLibertad.CP_ANIO;
                        OCCPFolio = SelectedProcesoLibertad.CP_FOLIO;
                        if (AgendaLibertadDetalle != null)
                        {
                            if (AgendaLibertadDetalle.AGENDA_ACT_LIB_DETALLE != null)
                            {
                                var x = AgendaLibertadDetalle.AGENDA_ACT_LIB_DETALLE.Count();
                            }
                        }
                    }
                    ValidacionOficioConclusion();
                    PopulateInformacionJuridica(2);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OFICIO_CONCLUSION);
                    break;
                case "agregar_oficio_conclusion":
                    AgregarOficioConclusion();
                    break;
                case "ver_oficio_conclusion":
                    Oficio_Asignacion = false;
                    Oficio_Conclusion = true;
                    Oficio_Baja = false;
                    ImprimirReportes();
                    break;
                case "cancelar_oficio_conclusion":
                    LimpiarOficios();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OFICIO_CONCLUSION);
                    break;
                case "viewOficio":
                    if (SelectedOficio != null)
                    {
                        var tc = new TextControlView();
                        tc.Closed += (s, e) =>
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        };
                        tc.editor.Loaded += (s, e) =>
                        {

                            tc.editor.Load(SelectedOficio.DOCUMENTO, TXTextControl.BinaryStreamType.WordprocessingML);
                        };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Show();
                   
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar el oficio a ver");
                    break;
                case "buscar_cp_asignacion":

                    if (OACPAnio.HasValue && OACPFolio.HasValue)
                    {
                        PopulateInformacionJuridica(1, OACPAnio, OACPFolio, OANUC);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar la causa penal");
                    break;
                case "buscar_cp_conclusion":
                    if (OCCPAnio.HasValue && OCCPFolio.HasValue)
                    {
                        PopulateInformacionJuridica(2, OCCPAnio, OCCPFolio);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar la causa penal");
                    break;
                case "baja":
                    LimpiarOficios();
                    if (SelectedProcesoLibertad != null)
                    {
                        OBCPAnio = SelectedProcesoLibertad.CP_ANIO;
                        OBCPFolio = SelectedProcesoLibertad.CP_FOLIO;
                        if (AgendaLibertadDetalle != null)
                        {
                            if (AgendaLibertadDetalle.AGENDA_ACT_LIB_DETALLE != null)
                            {
                                OBDias = AgendaLibertadDetalle.AGENDA_ACT_LIB_DETALLE.Count();
                                
                            }
                            if (AgendaLibertadDetalle.ACTIVIDAD_PROGRAMA != null)
                            {
                                OBPrograma = AgendaLibertadDetalle.ACTIVIDAD_PROGRAMA.PROGRAMA_LIBERTAD.DESCR;
                            }
                        }
                    }
                    ValidacionOficioBaja();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OFICIO_BAJA);
                    break;
                case "agregar_oficio_baja":
                    AgregarOficioBaja();
                    break;
                case "ver_oficio_baja":
                    Oficio_Asignacion = false;
                    Oficio_Conclusion = false;
                    Oficio_Baja = true;
                    ImprimirReportes();
                    break;
                case "cancelar_oficio_baja":
                    LimpiarOficios();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OFICIO_BAJA);
                    break;
            }
        }

        private async void OnLoad(ProgramasLibertadView obj = null)
        {
            try
            {
                ConfiguraPermisos();
                //Calendario = obj.Calendario;
               // var x = PopUpsViewModels.MainWindow.EditarFechaView.DG_Horario;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }

        }

        private async void ClickEnter(Object obj = null)
        {
            try
            {
                if (!pConsultar)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NUCBuscar":
                                NUCBuscar = textbox.Text;
                                break;
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "FolioBuscar":
                                if (!string.IsNullOrWhiteSpace(textbox.Text))
                                    FolioBuscar = Convert.ToInt32(textbox.Text);
                                else
                                    FolioBuscar = null;
                                break;
                            case "AnioBuscar":
                                if (!string.IsNullOrWhiteSpace(textbox.Text))
                                    AnioBuscar = short.Parse(textbox.Text);
                                else
                                    AnioBuscar = null;
                                break;
                        }
                    }
                }

                #region comentado
                //ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                //ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                //ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                //if (ListExpediente != null)
                //    EmptyExpedienteVisible = ListExpediente.Count < 0;
                //else
                //    EmptyExpedienteVisible = true;
                #endregion
                #region nuevo
                LstLiberados = new RangeEnabledObservableCollection<cLiberados>();
                LstLiberados.InsertRange(await SegmentarResultadoBusquedaLiberados());
                EmptyExpedienteVisible = LstLiberados.Count > 0 ? false : true;
                #endregion
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al imputado.", ex);
            }
        }

        private void CargarListas()
        {
            try
            {
                LstUnidadreceptora = new ObservableCollection<UNIDAD_RECEPTORA>(new cUnidadReceptora().ObtenerTodos());
                LstPrograma = new ObservableCollection<PROGRAMA_LIBERTAD>(new cProgramaLibertad().ObtenerTodos());

                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstUnidadreceptora.Insert(0, new UNIDAD_RECEPTORA() { ID_UNIDAD_RECEPTORA = -1, NOMBRE = "SELECCIONE" });
                    LstPrograma.Insert(0, new PROGRAMA_LIBERTAD() { ID_PROGRAMA_LIBERTAD = -1, DESCR = "SELECCIONE" });
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listados", ex);
            }
        }
        #endregion

        #region Agenda Seguimiento
        private void GetEntidadAgendaSeguimiento() 
        {
            try
            {
                if (AgendaLibertadDetalle == null)
                {
                    EstatusAL = -1;
                    UnidadReceptoraAL = -1;
                    ProgramaLibertadAL = ActividadProgramadaAL = -1;
                    FechaInicioAL = FechaFinalAL = null;
                    RemuneradaAL = FirmaInicioAL = FirmaFinalAL = false;
                    LstSeguimientoDetalle = new ObservableCollection<cSeguimientoDetalle>();
                    LstSeguimientoDetalleTodo = new ObservableCollection<AGENDA_ACT_LIB_DETALLE>();

                    Domingo = Lunes = Martes = Miercoles = Jueves = Viernes = Sabado = Domingo = false;
                    DomingoInicio = DomingoFin = LunesInicio = LunesFin = MartesInicio = MartesFin = MiercolesInicio = MiercolesFin = JuevesInicio = JuevesFin = ViernesInicio = ViernesFin = SabadoInicio = SabadoFin = null;

                    LstOficios = new ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO>();
                }
                else
                {
                    EstatusAL = AgendaLibertadDetalle.ID_ESTATUS;
                    UnidadReceptoraAL = AgendaLibertadDetalle.ID_UNIDAD_RECEPTORA;
                    ProgramaLibertadAL = AgendaLibertadDetalle.ID_PROGRAMA_LIBERTAD;
                    ActividadProgramadaAL = AgendaLibertadDetalle.ID_ACTIVIDAD_PROGRAMA;
                    FechaInicioAL = AgendaLibertadDetalle.FECHA_INICIO;
                    FechaFinalAL = AgendaLibertadDetalle.FECHA_FINAL;
                    RemuneradaAL = AgendaLibertadDetalle.REMUNERADA == "S" ? true : false;
                    //FirmaInicioAL = FirmaFinalAL = false;
                    LstSeguimientoDetalleTodo = new ObservableCollection<AGENDA_ACT_LIB_DETALLE>(AgendaLibertadDetalle.AGENDA_ACT_LIB_DETALLE);
                    PopulateRecurrencia();
                    LstOficios = new ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO>(AgendaLibertadDetalle.AGENDA_LIBERTAD_DOCUMENTO);
                }
               
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error", ex);
            }
        }

        private void Populate()
        {
            try
            {
                if (SelectedProcesoLibertad != null)
                {
                    LstAgenda = new ObservableCollection<AGENDA_ACTIVIDAD_LIBERTAD>(new cAgendaActividadLibertad().ObtenerTodos(
                         SelectedProcesoLibertad.ID_CENTRO,
                         SelectedProcesoLibertad.ID_ANIO,
                         SelectedProcesoLibertad.ID_IMPUTADO, 
                         SelectedProcesoLibertad.ID_PROCESO_LIBERTAD));                
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error", ex);
            }
        }

        private void Save() 
        {
            try
            {
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los datos obligatorios");
                    return;
                }
                if (AgendaLibertadDetalle == null)
                {
                    if (!pInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    var ald = new AGENDA_ACTIVIDAD_LIBERTAD();
                    ald.ID_CENTRO = selectedProcesoLibertad.ID_CENTRO;
                    ald.ID_ANIO = selectedProcesoLibertad.ID_ANIO;
                    ald.ID_IMPUTADO = selectedProcesoLibertad.ID_IMPUTADO;
                    ald.ID_PROCESO_LIBERTAD = selectedProcesoLibertad.ID_PROCESO_LIBERTAD;

                    ald.ID_ESTATUS = EstatusAL;
                    ald.ID_UNIDAD_RECEPTORA = UnidadReceptoraAL;
                    ald.ID_PROGRAMA_LIBERTAD = ProgramaLibertadAL;
                    ald.ID_ACTIVIDAD_PROGRAMA = ActividadProgramadaAL;
                    ald.FECHA_INICIO = FechaInicioAL;
                    ald.FECHA_FINAL = FechaFinalAL;
                    ald.REMUNERADA = RemuneradaAL ? "S" : "N"; 

                    ProgramarActividades();
                    ald.AGENDA_ACT_LIB_DETALLE = LstSeguimientoDetalleTodo;

                    var documentos = new List<AGENDA_LIBERTAD_DOCUMENTO>();
                    if (LstOficios != null)
                    {
                        short i = 1;
                        foreach (var d in LstOficios)
                        {
                            documentos.Add(new AGENDA_LIBERTAD_DOCUMENTO()
                            {
                                ID_DOCUMENTO = i,
                                ID_AGENDA_ACTIVIDAD_LIBERTAD = 0,
                                ID_IM_TIPO_DOCUMENTO = d.ID_IM_TIPO_DOCUMENTO,
                                DOCUMENTO = d.DOCUMENTO,
                                FECHA = d.FECHA,
                                OBSERVACION = d.OBSERVACION
                            });
                            i++;
                        }
                    }

                    ald.AGENDA_LIBERTAD_DOCUMENTO = documentos; 
                    ald.ID_AGENDA_ACTIVIDAD_LIBERTAD = new cAgendaActividadLibertad().Insertar(ald);
                    if (ald.ID_AGENDA_ACTIVIDAD_LIBERTAD > 0)
                    {
                        var x = ald;
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        Populate();
                        AgendaLibertadDetalle = LstAgenda.FirstOrDefault(w => w.ID_CENTRO == x.ID_CENTRO && w.ID_ANIO == x.ID_ANIO && w.ID_IMPUTADO == x.ID_IMPUTADO && w.ID_PROCESO_LIBERTAD == x.ID_PROCESO_LIBERTAD && w.ID_AGENDA_ACTIVIDAD_LIBERTAD == x.ID_AGENDA_ACTIVIDAD_LIBERTAD);
                    }
                }
                else
                {
                    if (!pEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    var fechas = new List<AGENDA_ACT_LIB_DETALLE>();
                    var obj = new AGENDA_ACTIVIDAD_LIBERTAD();
                    obj.ID_CENTRO = AgendaLibertadDetalle.ID_CENTRO;
                    obj.ID_ANIO = AgendaLibertadDetalle.ID_ANIO;
                    obj.ID_IMPUTADO = AgendaLibertadDetalle.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = AgendaLibertadDetalle.ID_PROCESO_LIBERTAD;
                    obj.ID_AGENDA_ACTIVIDAD_LIBERTAD = AgendaLibertadDetalle.ID_AGENDA_ACTIVIDAD_LIBERTAD;
                    
                    //obj.ID_ESTATUS = AgendaLibertadDetalle.ID_ESTATUS;
                    //obj.ID_UNIDAD_RECEPTORA = AgendaLibertadDetalle.ID_UNIDAD_RECEPTORA;
                    //obj.ID_PROGRAMA_LIBERTAD = AgendaLibertadDetalle.ID_PROGRAMA_LIBERTAD;
                    //obj.ID_ACTIVIDAD_PROGRAMA = AgendaLibertadDetalle.ID_ACTIVIDAD_PROGRAMA;
                    //obj.FECHA_INICIO = AgendaLibertadDetalle.FECHA_INICIO;
                    //obj.FECHA_FINAL = AgendaLibertadDetalle.FECHA_FINAL;
                    //obj.REMUNERADA = AgendaLibertadDetalle.REMUNERADA;
                    //obj.FIRMA_INICIO = AgendaLibertadDetalle.FIRMA_INICIO;
                    //obj.FIRMA_FINAL = AgendaLibertadDetalle.FIRMA_FINAL;

                    obj.ID_ESTATUS = EstatusAL;
                    obj.ID_UNIDAD_RECEPTORA = UnidadReceptoraAL;
                    obj.ID_PROGRAMA_LIBERTAD = ProgramaLibertadAL;
                    obj.ID_ACTIVIDAD_PROGRAMA = ActividadProgramadaAL;
                    obj.FECHA_INICIO = FechaInicioAL;
                    obj.FECHA_FINAL = FechaFinalAL;
                    obj.REMUNERADA = RemuneradaAL ? "S" : "N"; 

                    ProgramarActividades();
                    if (LstSeguimientoDetalleTodo != null)
                        fechas = new List<AGENDA_ACT_LIB_DETALLE>( LstSeguimientoDetalleTodo.ToList());
                    var oficios = new List<AGENDA_LIBERTAD_DOCUMENTO>();
                    if (LstOficios != null)
                    {
                        foreach (var o in LstOficios)
                        {
                            oficios.Add(new AGENDA_LIBERTAD_DOCUMENTO()
                            {
                               //ID_DOCUMENTO
                               ID_AGENDA_ACTIVIDAD_LIBERTAD = AgendaLibertadDetalle.ID_AGENDA_ACTIVIDAD_LIBERTAD,
                               ID_IM_TIPO_DOCUMENTO = o.ID_IM_TIPO_DOCUMENTO,
                               DOCUMENTO = o.DOCUMENTO,
                               FECHA = o.FECHA,
                               OBSERVACION = o.OBSERVACION
                            });
                        }
                    }
                    
                    if (new cAgendaActividadLibertad().Actualizar(obj, fechas,oficios))
                    {
                        var x = AgendaLibertadDetalle;
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        
                        Populate();
                        AgendaLibertadDetalle = LstAgenda.FirstOrDefault(w => w.ID_CENTRO == x.ID_CENTRO && w.ID_ANIO == x.ID_ANIO && w.ID_IMPUTADO == x.ID_IMPUTADO && w.ID_PROCESO_LIBERTAD == x.ID_PROCESO_LIBERTAD && w.ID_AGENDA_ACTIVIDAD_LIBERTAD == x.ID_AGENDA_ACTIVIDAD_LIBERTAD);
                    }
                }

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error", ex);
            }
        }
        #endregion

        #region Agenda Seguimiento Detalle
        private void ProgramarActividades()
        
        {
            try
            {
                LstSeguimientoDetalleTodo = new ObservableCollection<AGENDA_ACT_LIB_DETALLE>();
                int i = 1;
                var Fecha = FechaInicioAL;
                while (Fecha.Value.Date <= FechaFinalAL.Value.Date)
                {
                    switch ((short)Fecha.Value.DayOfWeek)
                    { 
                        case (short)enumSemana.Domingo:
                            if(Domingo)
                            LstSeguimientoDetalleTodo.Add(new AGENDA_ACT_LIB_DETALLE()
                            {
                                ID_DETALLE = i,
                                FECHA = Fecha,
                                HORA_INICIO = new DateTime(Fecha.Value.Year,Fecha.Value.Month,Fecha.Value.Day,DomingoInicio.Value.Hour,DomingoInicio.Value.Minute,0),
                                HORA_FIN = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, DomingoFin.Value.Hour, DomingoFin.Value.Minute, 0),
                                ASISTENCIA = 0
                            });
                        break;
                        case (short)enumSemana.Lunes:
                            if(Lunes)
                        LstSeguimientoDetalleTodo.Add(new AGENDA_ACT_LIB_DETALLE()
                        {
                            ID_DETALLE = i,
                            FECHA = Fecha,
                            HORA_INICIO = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, LunesInicio.Value.Hour, LunesInicio.Value.Minute, 0),
                            HORA_FIN = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, LunesFin.Value.Hour, LunesFin.Value.Minute, 0),
                            ASISTENCIA = 0
                        });
                        break;
                        case (short)enumSemana.Martes:
                            if(Martes)
                        LstSeguimientoDetalleTodo.Add(new AGENDA_ACT_LIB_DETALLE()
                        {
                            ID_DETALLE = i,
                            FECHA = Fecha,
                            HORA_INICIO = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, MartesInicio.Value.Hour, MartesInicio.Value.Minute, 0),
                            HORA_FIN = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, MartesFin.Value.Hour, MartesFin.Value.Minute, 0),
                            ASISTENCIA = 0
                        });
                        break;
                        case (short)enumSemana.Miercoles:
                            if(Miercoles)
                        LstSeguimientoDetalleTodo.Add(new AGENDA_ACT_LIB_DETALLE()
                        {
                            ID_DETALLE = i,
                            FECHA = Fecha,
                            HORA_INICIO = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, MiercolesInicio.Value.Hour, MiercolesInicio.Value.Minute, 0),
                            HORA_FIN = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, MiercolesFin.Value.Hour, MiercolesFin.Value.Minute, 0),
                            ASISTENCIA = 0
                        });
                        break;
                        case (short)enumSemana.Jueves:
                            if(Jueves)
                        LstSeguimientoDetalleTodo.Add(new AGENDA_ACT_LIB_DETALLE()
                        {
                            ID_DETALLE = i,
                            FECHA = Fecha,
                            HORA_INICIO = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, JuevesInicio.Value.Hour, JuevesInicio.Value.Minute, 0),
                            HORA_FIN = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, JuevesFin.Value.Hour, JuevesFin.Value.Minute, 0),
                            ASISTENCIA = 0
                        });
                        break;
                        case (short)enumSemana.Viernes:
                            if(Viernes)
                        LstSeguimientoDetalleTodo.Add(new AGENDA_ACT_LIB_DETALLE()
                        {
                            ID_DETALLE = i,
                            FECHA = Fecha,
                            HORA_INICIO = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, ViernesInicio.Value.Hour, ViernesInicio.Value.Minute, 0),
                            HORA_FIN = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, ViernesFin.Value.Hour, ViernesFin.Value.Minute, 0),
                            ASISTENCIA = 0
                        });
                        break;
                        case (short)enumSemana.Sabado:
                            if(Sabado)
                        LstSeguimientoDetalleTodo.Add(new AGENDA_ACT_LIB_DETALLE()
                        {
                            ID_DETALLE = i,
                            FECHA = Fecha,
                            HORA_INICIO = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, SabadoInicio.Value.Hour, SabadoInicio.Value.Minute, 0),
                            HORA_FIN = new DateTime(Fecha.Value.Year, Fecha.Value.Month, Fecha.Value.Day, SabadoFin.Value.Hour, SabadoFin.Value.Minute, 0),
                            ASISTENCIA = 0
                        });
                        break;
                    }
                    i++;
                    Fecha = Fecha.Value.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error..", ex);
            }
        }

        private void PopulateRecurrencia()
        {
            try
            {
                if (AgendaLibertadDetalle!= null)
                { 
                    if(AgendaLibertadDetalle.AGENDA_ACT_LIB_DETALLE != null)
                    {
                        var dias = AgendaLibertadDetalle.AGENDA_ACT_LIB_DETALLE.OrderBy(w => w.ID_DETALLE).Take(7);
                        if (dias != null)
                        {
                            foreach (var d in dias)
                            { 
                                switch((short)d.FECHA.Value.DayOfWeek)
                                {
                                    case (short)enumSemana.Domingo:
                                        if (!Domingo)
                                        {
                                            Domingo = true;
                                            DomingoInicio = d.HORA_INICIO;
                                            DomingoFin = d.HORA_FIN;
                                        }
                                        break;
                                    case (short)enumSemana.Lunes:
                                        if (!Lunes)
                                        {
                                            Lunes = true;
                                            LunesInicio = d.HORA_INICIO;
                                            LunesFin = d.HORA_FIN;
                                        }
                                        break;
                                    case (short)enumSemana.Martes:
                                        if (!Martes)
                                        {
                                            Martes = true;
                                            MartesInicio = d.HORA_INICIO;
                                            MartesFin = d.HORA_FIN;
                                        }
                                        break;
                                    case (short)enumSemana.Miercoles:
                                        if (!Miercoles)
                                        {
                                            Miercoles = true;
                                            MiercolesInicio = d.HORA_INICIO;
                                            MiercolesFin = d.HORA_FIN;
                                        }
                                        break;
                                    case (short)enumSemana.Jueves:
                                        if (!Jueves)
                                        {
                                            Jueves = true;
                                            JuevesInicio = d.HORA_INICIO;
                                            JuevesFin = d.HORA_FIN;
                                        }
                                        break;
                                    case (short)enumSemana.Viernes:
                                        if (!Viernes)
                                        {
                                            Viernes = true;
                                            ViernesInicio = d.HORA_INICIO;
                                            ViernesFin = d.HORA_FIN;
                                        }
                                        break;
                                    case (short)enumSemana.Sabado:
                                        if (!Sabado)
                                        {
                                            Sabado = true;
                                            SabadoInicio = d.HORA_INICIO;
                                            SabadoFin = d.HORA_FIN;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error..", ex);
            }
        }
        #endregion 

        #region Huellas Digitales
        private void ShowIdentification(object obj = null)
        {
            ShowPopUp = Visibility.Visible;
            ShowFingerPrint = Visibility.Hidden;
            var Initial442 = new Thread((Init) =>
            {
                try
                {
                    var nRet = 0;

                    CLSFPCaptureDllWrapper.CLS_Initialize();
                    CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                    nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);

                    if (nRet == 0)
                    {
                        ScannerMessage = "Procesando...";
                        ShowFingerPrint = Visibility.Visible;
                        ShowLine = Visibility.Visible;
                        ShowOk = Visibility.Hidden;
                        Thread.Sleep(300);
                        HuellasCapturadas = new List<PlantillaBiometrico>();

                        var SaveFingerPrints = new Thread((saver) =>
                        {
                            try
                            {
                                #region [Huellas]
                                for (short i = 1; i <= 10; i++)
                                {
                                    var pBuffer = IntPtr.Zero;
                                    var nBufferLength = 0;
                                    var nNFIQ = 0;

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
                                        GuardaHuella = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                        FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                                    }

                                    Thread.Sleep(200);
                                    switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                                    {
                                        #region [Pulgar Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                PulgarDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Indice Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                IndiceDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Medio Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MedioDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Anular Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                AnularDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Meñique Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MeñiqueDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Pulgar Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                PulgarIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Indice Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                IndiceIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Medio Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MedioIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Anular Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                AnularIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Meñique Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MeñiqueIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        default:
                                            break;
                                    }
                                }
                                #endregion
                                if (SelectExpediente == null)
                                    ScannerMessage = "Huellas Capturadas Correctamente";
                                else
                                    if (new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == SelectExpediente.ID_ANIO && w.ID_CENTRO == SelectExpediente.ID_CENTRO && w.ID_IMPUTADO == SelectExpediente.ID_IMPUTADO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP).ToList().Any())
                                        ScannerMessage = "Huellas Actualizadas Correctamente";
                                    else
                                        ScannerMessage = "Huellas Capturadas Correctamente";
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al procesar huellas.", ex);
                            }
                        });

                        SaveFingerPrints.Start();
                        SaveFingerPrints.Join();

                        ShowLine = Visibility.Hidden;
                        Thread.Sleep(1500);
                    }
                    ShowPopUp = Visibility.Hidden;
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ShowPopUp = Visibility.Hidden;
                        (new Dialogos()).ConfirmacionDialogo("Error", "Revise que el escanner este bien configurado.");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    }));
                }
            });

            Initial442.Start();
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

        private void OkClick(object ImgPrint = null)
        {
            ShowPopUp = Visibility.Hidden;
        }

        private async void OnBuscarPorHuella(string obj = "")
        {
            await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));

            await TaskEx.Delay(400);

            var nRet = -1;
            var bandera = true;
            var requiereGuardarHuellas = false;// Parametro.GuardarHuellaEnBusquedaRegistro;
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
            windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.IMPUTADO, nRet == 0, requiereGuardarHuellas);
            if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                return;
            }
            windowBusqueda.Owner = PopUpsViewModels.MainWindow;
            windowBusqueda.KeyDown += (s, e) =>
            {
                try
                {
                    if (e.Key == System.Windows.Input.Key.Escape) windowBusqueda.Close();
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar", ex);
                }
            };
            windowBusqueda.Closed += (s, e) =>
            {
                try
                {
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    if (!((BusquedaHuellaViewModel)windowBusqueda.DataContext).IsSucceed)
                        return;

                    SelectExpediente = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro != null ? ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro.Imputado : null;


                    if (SelectExpediente == null)
                    {
                        //CamposBusquedaEnabled = true;
                        MenuBuscarEnabled = pConsultar;
                    }
                    else
                    {
                        AnioBuscar = SelectExpediente.ID_ANIO;
                        FolioBuscar = SelectExpediente.ID_IMPUTADO;
                        ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                        ApellidoMaternoBuscar = SelectExpediente.MATERNO;
                        NombreBuscar = SelectExpediente.NOMBRE;
                        clickSwitch("buscar_visible");
                        MenuBuscarEnabled = pConsultar;
                        MenuGuardarEnabled = pEditar;
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
            //AceptarBusquedaHuellaFocus = true;
        }

        private void CargarHuellas()
        {
            try
            {
                //if (SelectExpediente == null)
                //    return;
                var LoadHuellas = new Thread((Init) =>
                {
                    //if (SelectExpediente!=null)
                    //{
                    //    var Huellas = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == SelectExpediente.ID_ANIO && w.ID_CENTRO == SelectExpediente.ID_CENTRO && w.ID_IMPUTADO == SelectExpediente.ID_IMPUTADO && w.ID_TIPO_BIOMETRICO >= 11 && w.ID_TIPO_BIOMETRICO <= 30).ToList();
                    //    //HuellasCapturadas = Huellas;
                    //}


                    if (HuellasCapturadas != null)
                        foreach (var item in HuellasCapturadas.Where(w => w.ID_TIPO_FORMATO == enumTipoFormato.FMTO_DP))
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

                if (SelectExpediente != null)
                {
                    if (SelectExpediente.IMPUTADO_BIOMETRICO != null ? SelectExpediente.IMPUTADO_BIOMETRICO.Count > 0 : true)
                    {
                        HuellasCapturadas = new List<PlantillaBiometrico>();
                        foreach (var biometrico in SelectExpediente.IMPUTADO_BIOMETRICO)
                        {
                            switch (biometrico.ID_TIPO_BIOMETRICO)
                            {
                                case 0:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 1:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 2:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 3:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 4:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 5:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 6:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 7:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 8:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 9:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;

                            }
                        }
                    }
                }

                LoadHuellas.Start();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Buscar 
        private void LimpiarBusqueda()
        {
            ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = NUCBuscar = string.Empty;
            AnioBuscar = null;
            FolioBuscar = null;
            SelectExpediente = null;
            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            ListExpediente = null;
            LstLiberados = null;
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
        
        private async Task<List<cLiberados>> SegmentarResultadoBusquedaLiberados(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && string.IsNullOrEmpty(NUCBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                return new List<cLiberados>();

            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<IEnumerable<cLiberados>>(() => new cImputado().ObtenerLiberados(NUCBuscar,AnioBuscar,FolioBuscar, NombreBuscar, ApellidoPaternoBuscar, ApellidoMaternoBuscar, _Pag).Select(w => new cLiberados()
            {
                ID_CENTRO = w.ID_CENTRO,
                ID_ANIO = w.ID_ANIO,
                ID_IMPUTADO = w.ID_IMPUTADO,
                CENTRO = w.CENTRO,
                NOMBRE = string.Format("{0}{1}", w.NOMBRE, !string.IsNullOrEmpty(w.APODO_NOMBRE) ? "(" + w.APODO_NOMBRE + ")" : string.Empty),
                PATERNO = string.Format("{0}{1}", w.PATERNO, !string.IsNullOrEmpty(w.PATERNO_A) ? "(" + w.PATERNO_A + ")" : string.Empty),
                MATERNO = string.Format("{0}{1}", w.MATERNO, !string.IsNullOrEmpty(w.MATERNO_A) ? "(" + w.MATERNO_A + ")" : string.Empty)
            }));
            if (result.Any())
            {
                Pagina++;
                SeguirCargando = true;
            }
            else
                SeguirCargando = false;

            return result.ToList();
        }

        private void Obtener()
        {
            try
            {
                #region BusquedaDatos
                NUCBuscar = SelectedProcesoLibertad.NUC;
                AnioBuscar = SelectExpediente.ID_ANIO;
                FolioBuscar = SelectExpediente.ID_IMPUTADO;
                NombreBuscar = SelectExpediente.NOMBRE;
                ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                ApellidoMaternoBuscar = SelectExpediente.MATERNO;
                #endregion

                #region Seguimiento
                LstAgenda = new ObservableCollection<AGENDA_ACTIVIDAD_LIBERTAD>(SelectedProcesoLibertad.AGENDA_ACTIVIDAD_LIBERTAD);
                #endregion
                
                MenuReporteEnabled = true;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener información del imputado.", ex);
            }

        }
        #endregion

        #region Unidad Receptora
        private void SetUnidadReceptora()
        {
            try
            {
                if (SelectedUnidadReceptora != null)
                {
                      DireccionUR = string.Format("{0} {1} {2} {3}",
                          !string.IsNullOrEmpty(SelectedUnidadReceptora.CALLE_DIRECCION) ? SelectedUnidadReceptora.CALLE_DIRECCION : string.Empty,
                          !string.IsNullOrEmpty(SelectedUnidadReceptora.NUM_EXT_DIRECCION) ? "NUM."+ SelectedUnidadReceptora.NUM_EXT_DIRECCION : string.Empty,
                          !string.IsNullOrEmpty(SelectedUnidadReceptora.NUM_INT_DIRECCION)? "NUM.EXT."+SelectedUnidadReceptora.NUM_INT_DIRECCION : string.Empty,
                          SelectedUnidadReceptora.COLONIA != null ? SelectedUnidadReceptora.COLONIA.DESCR.Trim() : string.Empty,
                          SelectedUnidadReceptora.COLONIA != null ? SelectedUnidadReceptora.COLONIA.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty);
                        TelefonoUR = SelectedUnidadReceptora.TELEFONO.ToString();
                  
                }
                else
                {
                    DireccionUR = TelefonoUR =  string.Empty;
                }

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }
        #endregion

        #region Reportes
        private void LimpiarOficios() 
        {
            //Asignacion
            OAFuero = "C";
            OAFEcha = null;
            OACPAnio = null;
            OACPFolio = null;
            OANUC = string.Empty;
            OAJuzgado = string.Empty;
            OADelito = string.Empty;
            OASustitucionPena = string.Empty;
            OANoJornadasLetra = string.Empty;
            //Conclusion
            OCFecha = null;
            OCCPAnio = null;
            OCCPFolio = null;
            OCJuzgado = string.Empty;
            OCDelito = string.Empty;
            OCJornadasCumplidas = string.Empty;
            OCOficioConclusion = string.Empty;
            OCFechaConclusion = null;
            //Baja
            OBFecha = null;
            OBDias = null;
            OBCPAnio = null;
            OBCPFolio = null;
            OBDiasRegistrados = null;
            OBPrograma = string.Empty;
            OBMesBaja = -1;
            OBDiasPendientes = null;
            OBNumeroBaja = -1;
        }
        
        private void PopulateInformacionJuridica(int Tipo = 1,short? Anio = null, int? Folio = null,string NUC = "") 
        {
            try
            {
                if (SelectedProcesoLibertad != null)
                {
                    //Obtenemos la ultima causa penal
                    var cp = new cCausaPenal().ObtenerCausaPenalProgramaLibertad(
                        SelectedProcesoLibertad.ID_CENTRO,
                        SelectedProcesoLibertad.ID_ANIO,
                        SelectedProcesoLibertad.ID_IMPUTADO,
                        Anio.HasValue ? Anio : SelectedProcesoLibertad.CP_ANIO,
                        Folio.HasValue ? Folio : SelectedProcesoLibertad.CP_FOLIO,
                        !string.IsNullOrEmpty(NUC) ? NUC : SelectedProcesoLibertad.NUC);
                    if (cp != null)
                    {
                        if (Tipo == 1)
                        {
                            var delitos = string.Empty;
                            if (cp.CAUSA_PENAL_DELITO != null)
                            {
                                foreach (var d in cp.CAUSA_PENAL_DELITO)
                                {
                                    if (!string.IsNullOrEmpty(delitos))
                                        delitos = delitos + ", ";
                                    delitos = delitos + d.DESCR_DELITO;
                                }
                            }

                            OAFuero = cp.CP_FUERO;
                            OAFEcha = Fechas.GetFechaDateServer;
                            OACPAnio = cp.CP_ANIO;
                            OACPFolio = cp.CP_FOLIO;
                            OANUC = cp.NUC != null ? cp.NUC.ID_NUC : string.Empty;
                            OAJuzgado = cp.JUZGADO.DESCR;
                            OADelito = delitos;
                            OASustitucionPena = string.Empty;
                            OANoJornadasLetra = string.Empty;
                        }
                        else
                        {
                            OCFecha = Fechas.GetFechaDateServer;
                            OCCPAnio = cp.CP_ANIO;
                            OCCPFolio = cp.CP_FOLIO;
                            OCJuzgado = cp.JUZGADO.DESCR;
                            var delitos = string.Empty;
                            if (cp.CAUSA_PENAL_DELITO != null)
                            {
                                foreach (var d in cp.CAUSA_PENAL_DELITO)
                                {
                                    if (!string.IsNullOrEmpty(delitos))
                                        delitos = delitos + ", ";
                                    delitos = delitos + d.DESCR_DELITO;
                                }
                            }
                            OCDelito = delitos;
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void AgregarOficioAsignacion() 
        {
            try
            {
                if (!base.HasErrors)
                {
                    if (LstOficios == null)
                        LstOficios = new ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO>();
                    IMPUTADO_TIPO_DOCUMENTO itd = new IMPUTADO_TIPO_DOCUMENTO();
                    if (!string.IsNullOrEmpty(OANUC))
                    {
                        itd = new cImputadoTipoDocumento().Obtener(345);
                    }
                    else
                    {
                        itd = new cImputadoTipoDocumento().Obtener(346);
                    }

                    var diccionario = new Dictionary<string, string>();
                    diccionario.Add("<<responsable_liberados>>", Parametro.RESPONSABLE_LIBERADOS);
                    diccionario.Add("<<direccion_liberados>>", Parametro.DIRECCION_LIBERADOS);
                    diccionario.Add("<<jefe_evaluacion>>", Parametro.JEFE_EVALUACION);
                    diccionario.Add("<<nombre>>",string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(ApellidoPaternoBuscar) ? ApellidoPaternoBuscar.Trim() : string.Empty,
                        !string.IsNullOrEmpty(ApellidoMaternoBuscar) ? ApellidoMaternoBuscar.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreBuscar) ? NombreBuscar.Trim() : string.Empty));
                    diccionario.Add("<<jefe_atencion_seguimiento>>", Parametro.JEFE_ATENCION_SEGUIMIENTO);
                    diccionario.Add("<<fuero>>", OAFuero == "C" ? "COMUN" : "FEDERAL");
                    diccionario.Add("<<fecha>>", Fechas.fechaLetra(OAFEcha));
                    diccionario.Add("<<causa_penal>>", string.Format("{0}/{1}", OACPAnio, OACPFolio));
                    diccionario.Add("<<nuc>>", OANUC);
                    diccionario.Add("<<juzgado>>", OAJuzgado);
                    diccionario.Add("<<delito>>", OADelito);
                    diccionario.Add("<<sustitucion_pena>>", OASustitucionPena);
                    diccionario.Add("<<no_jornadas>>", OANoJornadasLetra);
                    var d = new cWord().FillFieldsDocx(itd.DOCUMENTO, diccionario);      

                    LstOficios.Add(new AGENDA_LIBERTAD_DOCUMENTO()
                    {
                        ID_IM_TIPO_DOCUMENTO = itd.ID_IM_TIPO_DOCTO,
                        FECHA = Fechas.GetFechaDateServer,
                        DOCUMENTO = d,
                        IMPUTADO_TIPO_DOCUMENTO = itd,
                        OBSERVACION = !string.IsNullOrEmpty(OAObseervacion) ? OAObseervacion : string.Format("{0} {1:dd/MM/yyyy}", itd.DESCR, Fechas.GetFechaDateServer)
                    });
                    LstOficios = new ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO>(LstOficios);
                    base.ClearRules();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OFICIO_ASIGNACION_NSJP);
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar los campos requeridos. \n" + base.Error);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void AgregarOficioConclusion()
        {
            try
            {
                if (!base.HasErrors)
                {
                    if (LstOficios == null)
                        LstOficios = new ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO>();
                    IMPUTADO_TIPO_DOCUMENTO itd = new IMPUTADO_TIPO_DOCUMENTO();
                    itd = new cImputadoTipoDocumento().Obtener(347);

                    var diccionario = new Dictionary<string, string>();
                    diccionario.Add("<<jefe_evaluacion>>", Parametro.JEFE_EVALUACION);
                    diccionario.Add("<<jefe_atencion_seguimiento>>", Parametro.JEFE_ATENCION_SEGUIMIENTO);
                    diccionario.Add("<<nombre>>", string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(ApellidoPaternoBuscar) ? ApellidoPaternoBuscar.Trim() : string.Empty,
                        !string.IsNullOrEmpty(ApellidoMaternoBuscar) ? ApellidoMaternoBuscar.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreBuscar) ? NombreBuscar.Trim() : string.Empty));
                    diccionario.Add("<<fecha>>", Fechas.fechaLetra(OCFecha));
                    diccionario.Add("<<causa_penal>>", string.Format("{0}/{1}", OCCPAnio, OCCPFolio));
                    diccionario.Add("<<juzgado>>", OCJuzgado);
                    diccionario.Add("<<delito>>", OCDelito);
                    diccionario.Add("<<jornadas_cumplidas>>", OCJornadasCumplidas);
                    diccionario.Add("<<folio_conclusion>>", OCOficioConclusion);
                    diccionario.Add("<<fecha_conclusion>>", OCFechaConclusion.Value.ToString("dd/MM/yyyy"));
                    var d = new cWord().FillFieldsDocx(itd.DOCUMENTO, diccionario);      

                    LstOficios.Add(new AGENDA_LIBERTAD_DOCUMENTO() {
                     ID_IM_TIPO_DOCUMENTO = itd.ID_IM_TIPO_DOCTO,
                     FECHA = Fechas.GetFechaDateServer,
                     DOCUMENTO = d,
                     IMPUTADO_TIPO_DOCUMENTO = itd,
                     OBSERVACION = !string.IsNullOrEmpty(OCObservacion) ? OCObservacion : string.Format("{0} {1:dd/MM/yyyy}", itd.DESCR, Fechas.GetFechaDateServer)
                    });
                    LstOficios = new ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO>(LstOficios);
                    base.ClearRules();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OFICIO_CONCLUSION);
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar los campos requeridos. \n" + base.Error);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void AgregarOficioBaja() 
        {
            try
            {
                if (!base.HasErrors)
                {
                    if (LstOficios == null)
                        LstOficios = new ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO>();
                    IMPUTADO_TIPO_DOCUMENTO itd = new IMPUTADO_TIPO_DOCUMENTO();
                    itd = new cImputadoTipoDocumento().Obtener(348);
                    string[] meses = { "", "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE" };
                    string[] numero = { "", "PRIMERA", "SEGUNDA", "TERCERA", "CUARTA", "QUINTA", "SEXTA", "SEPTIMA", "OCTAVA", "NOVENA", "DECIMA" };
                    var diccionario = new Dictionary<string, string>();
                    diccionario.Add("<<jefe_evaluacion>>", Parametro.JEFE_EVALUACION);
                    diccionario.Add("<<jefe_atencion_seguimiento>>", Parametro.JEFE_ATENCION_SEGUIMIENTO);
                    diccionario.Add("<<nombre>>", string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(ApellidoPaternoBuscar) ? ApellidoPaternoBuscar.Trim() : string.Empty,
                        !string.IsNullOrEmpty(ApellidoMaternoBuscar) ? ApellidoMaternoBuscar.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreBuscar) ? NombreBuscar.Trim() : string.Empty));
                    diccionario.Add("<<fecha>>", Fechas.fechaLetra(OBFecha));
                    diccionario.Add("<<causa_penal>>", string.Format("{0}/{1}", OBCPAnio, OBCPFolio));
                    diccionario.Add("<<dias>>", OBDias.ToString());
                    diccionario.Add("<<dias_registrados>>", OBDiasRegistrados.ToString());
                    diccionario.Add("<<programa>>", OBPrograma);
                    diccionario.Add("<<mes_baja>>", meses[OBMesBaja.Value]);
                    diccionario.Add("<<dias_pendientes>>", OBDiasPendientes.ToString());
                    diccionario.Add("<<numero_baja>>", numero[OBNumeroBaja.Value]);
                    var d = new cWord().FillFieldsDocx(itd.DOCUMENTO, diccionario);
                    LstOficios.Add(new AGENDA_LIBERTAD_DOCUMENTO()
                    {
                        ID_IM_TIPO_DOCUMENTO = itd.ID_IM_TIPO_DOCTO,
                        FECHA = Fechas.GetFechaDateServer,
                        DOCUMENTO = d,
                        IMPUTADO_TIPO_DOCUMENTO = itd,
                        OBSERVACION = !string.IsNullOrEmpty(OBObservacion) ? OBObservacion : string.Format("{0} {1:dd/MM/yyyy}", itd.DESCR, Fechas.GetFechaDateServer)
                    });
                    LstOficios = new ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO>(LstOficios);
                    base.ClearRules();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OFICIO_BAJA);
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar los campos requeridos. \n" + base.Error);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void ImprimirReportes()
        {
            try
            {
                var diccionario = new Dictionary<string, string>();
                diccionario.Add("<<responsable_liberados>>", Parametro.RESPONSABLE_LIBERADOS);
                diccionario.Add("<<direccion_liberados>>", Parametro.DIRECCION_LIBERADOS);
                diccionario.Add("<<jefe_evaluacion>>", Parametro.JEFE_EVALUACION);
                diccionario.Add("<<jefe_atencion_seguimiento>>", Parametro.JEFE_ATENCION_SEGUIMIENTO);
                diccionario.Add("<<nombre>>", string.Format("{0} {1} {2}",
                       !string.IsNullOrEmpty(ApellidoPaternoBuscar) ? ApellidoPaternoBuscar.Trim() : string.Empty,
                       !string.IsNullOrEmpty(ApellidoMaternoBuscar) ? ApellidoMaternoBuscar.Trim() : string.Empty,
                       !string.IsNullOrEmpty(NombreBuscar) ? NombreBuscar.Trim() : string.Empty));

                IMPUTADO_TIPO_DOCUMENTO itd = new IMPUTADO_TIPO_DOCUMENTO();
                var tc = new TextControlGuardarView();
                if (Oficio_Asignacion)
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OFICIO_ASIGNACION_NSJP);
                if (Oficio_Conclusion)
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OFICIO_CONCLUSION);
                if (Oficio_Baja)
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OFICIO_BAJA);

                tc.Closed += (s, e) =>
                {
                    
                    if (LstOficios == null)
                        LstOficios = new ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO>();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    if (Oficio_Asignacion)
                    {
                        if (!string.IsNullOrEmpty(OANUC))
                        {
                            itd = new cImputadoTipoDocumento().Obtener(345);
                        }
                        else
                        {
                            itd = new cImputadoTipoDocumento().Obtener(346);
                        }
                        LstOficios.Add(new AGENDA_LIBERTAD_DOCUMENTO()
                        {
                            ID_IM_TIPO_DOCUMENTO = itd.ID_IM_TIPO_DOCTO,
                            FECHA = Fechas.GetFechaDateServer,
                            DOCUMENTO = tc.Documento,
                            IMPUTADO_TIPO_DOCUMENTO = itd,
                            OBSERVACION = !string.IsNullOrEmpty(OAObseervacion) ? OAObseervacion : string.Format("{0} {1:dd/MM/yyyy}",itd.DESCR,Fechas.GetFechaDateServer)
                        });
                    }
                    
                    if (Oficio_Conclusion)
                    {
                        itd = new cImputadoTipoDocumento().Obtener(347);
                        LstOficios.Add(new AGENDA_LIBERTAD_DOCUMENTO()
                        {
                            ID_IM_TIPO_DOCUMENTO = itd.ID_IM_TIPO_DOCTO,
                            FECHA = Fechas.GetFechaDateServer,
                            DOCUMENTO = tc.Documento,
                            IMPUTADO_TIPO_DOCUMENTO = itd,
                            OBSERVACION = !string.IsNullOrEmpty(OCObservacion) ? OCObservacion : string.Format("{0} {1:dd/MM/yyyy}",itd.DESCR,Fechas.GetFechaDateServer)                      
                        });
                    }
                    
                    if (Oficio_Baja)
                    {  
                        itd = new cImputadoTipoDocumento().Obtener(348);
                        LstOficios.Add(new AGENDA_LIBERTAD_DOCUMENTO()
                        {
                            ID_IM_TIPO_DOCUMENTO = itd.ID_IM_TIPO_DOCTO,
                            FECHA = Fechas.GetFechaDateServer,
                            DOCUMENTO = tc.Documento,
                            IMPUTADO_TIPO_DOCUMENTO = itd,
                            OBSERVACION = !string.IsNullOrEmpty(OBObservacion) ? OBObservacion : string.Format("{0} {1:dd/MM/yyyy}",itd.DESCR,Fechas.GetFechaDateServer)                      
                        });
                    }
                };

                if (Oficio_Asignacion)
                {
                    diccionario.Add("<<fuero>>", OAFuero == "C" ? "COMUN" : "FEDERAL");
                    diccionario.Add("<<fecha>>", Fechas.fechaLetra(OAFEcha));
                    diccionario.Add("<<causa_penal>>", string.Format("{0}/{1}", OACPAnio, OACPFolio));
                    diccionario.Add("<<nuc>>", OANUC);
                    diccionario.Add("<<juzgado>>", OAJuzgado);
                    diccionario.Add("<<delito>>", OADelito);
                    diccionario.Add("<<sustitucion_pena>>", OASustitucionPena);
                    diccionario.Add("<<no_jornadas>>", OANoJornadasLetra);
                    if (OAFuero == "F" || (OAFuero == "C" && !string.IsNullOrEmpty(OANUC)))
                    {
                        var oficio = new cImputadoTipoDocumento().Obtener(345);//(short)enumTipoDocumentoImputado.OFICIO_ASIGNACION_NSJ);
                        if (oficio != null)
                        {
                            tc.editor.Loaded += (s, e) =>
                            {
                                var d = new cWord().FillFieldsDocx(oficio.DOCUMENTO, diccionario);
                                tc.editor.Load(d, TXTextControl.BinaryStreamType.WordprocessingML);
                            };
                        }
                    }
                    else
                    {
                        var oficio = new cImputadoTipoDocumento().Obtener(346);
                        if (oficio != null)
                        {
                            tc.editor.Loaded += (s, e) =>
                            {
                                var d = new cWord().FillFieldsDocx(oficio.DOCUMENTO, diccionario);
                                tc.editor.Load(d, TXTextControl.BinaryStreamType.WordprocessingML);
                            };
                        }
                    }
                }
                else
                    if (Oficio_Conclusion)
                    {

                        diccionario.Add("<<fecha>>", Fechas.fechaLetra(OCFecha));
                        diccionario.Add("<<causa_penal>>", string.Format("{0}/{1}", OCCPAnio, OCCPFolio));
                        diccionario.Add("<<juzgado>>", OCJuzgado);
                        diccionario.Add("<<delito>>", OCDelito);
                        diccionario.Add("<<jornadas_cumplidas>>", OCJornadasCumplidas);
                        diccionario.Add("<<folio_conclusion>>", OCOficioConclusion);
                        diccionario.Add("<<fecha_conclusion>>", OCFechaConclusion.Value.ToString("dd/MM/yyyy"));
                        var oficio = new cImputadoTipoDocumento().Obtener(347);
                        if (oficio != null)
                        {
                            tc.editor.Loaded += (s, e) =>
                            {
                                var d = new cWord().FillFieldsDocx(oficio.DOCUMENTO, diccionario);
                                tc.editor.Load(d, TXTextControl.BinaryStreamType.WordprocessingML);
                            };
                        }
                    }
                    else
                        if (Oficio_Baja)
                        {

                            string[] meses = { "", "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE" };
                            string[] numero = { "", "PRIMERA", "SEGUNDA", "TERCERA", "CUARTA", "QUINTA", "SEXTA", "SEPTIMA", "OCTAVA", "NOVENA", "DECIMA" };
                            diccionario.Add("<<fecha>>", Fechas.fechaLetra(OBFecha));
                            diccionario.Add("<<causa_penal>>", string.Format("{0}/{1}", OBCPAnio, OBCPFolio));
                            diccionario.Add("<<dias>>", OBDias.ToString());
                            diccionario.Add("<<dias_registrados>>", OBDiasRegistrados.ToString());
                            diccionario.Add("<<programa>>", OBPrograma);
                            diccionario.Add("<<mes_baja>>", meses[OBMesBaja.Value]);
                            diccionario.Add("<<dias_pendientes>>", OBDiasPendientes.ToString());
                            diccionario.Add("<<numero_baja>>", numero[OBNumeroBaja.Value]);
                            var oficio = new cImputadoTipoDocumento().Obtener(348);
                            if (oficio != null)
                            {
                                tc.editor.Loaded += (s, e) =>
                                {
                                    var d = new cWord().FillFieldsDocx(oficio.DOCUMENTO, diccionario);
                                    tc.editor.Load(d, TXTextControl.BinaryStreamType.WordprocessingML);
                                };
                            }
                        }

                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Show();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }
        #endregion

        #region Privilegios
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.PROGRAMA_LIBERTAD.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        pInsertar = true;
                    if (p.EDITAR == 1)
                        pEditar = true;
                    if (p.CONSULTAR == 1)
                        pConsultar = true;
                    if (p.IMPRIMIR == 1)
                        pImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }

    public enum enumSemana 
    { 
        Domingo = 0,
        Lunes = 1,
        Martes = 2, 
        Miercoles = 3,
        Jueves = 4,
        Viernes = 5,
        Sabado = 6
    }

    public enum enumFormatosLiberados
    {
        Oficio_Asignacion = 1,
        Oficio_Conclusion = 2
    }
}
