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
using TXTextControl;
using Microsoft.Reporting.WinForms;
using Novacode;



namespace ControlPenales
{
    partial class NotaTecnicaViewModel : ValidationViewModelBase
    {

        private short?[] estatus_inactivos= null;

        #region Constructor
        public NotaTecnicaViewModel() { }
        #endregion

        #region Funciones Eventos
        private async void clickSwitch(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }
                switch (obj.ToString())
                {
                    #region Buscar
                    case "nueva_busqueda":
                        if (ListExpediente != null)
                            ListExpediente.Clear();
                        SelectExpediente = null;
                        ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                        FolioBuscar = AnioBuscar = null;
                        ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                        break;
                    case "buscar_menu":
                        if (!PConsultar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        AuxIngreso = SelectIngreso;
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ListExpediente = null;
                        ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                        FolioBuscar = AnioBuscar = null;
                        ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "buscar_salir":
                        SelectExpediente = null;
                        SelectIngreso = null;
                        if (AuxIngreso != null)
                        {
                            SelectIngreso = AuxIngreso;
                            AuxIngreso = null;
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "buscar_seleccionar":
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar ingreso");
                            break;
                        }
                        else
                        {
                            if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                            {
                                SelectIngreso = null;
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso no pertenece a su centro");
                                break;
                            }
                            if (EstatusInactivos.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO))
                            {
                                SelectIngreso = null;
                                new Dialogos().ConfirmacionDialogo("Validación", "El estatus delingreso no esta activo en el centro");
                                break;
                            }

                            #region Datos Generales
                            AnioD = SelectIngreso.ID_ANIO;
                            FolioD = SelectIngreso.ID_IMPUTADO;
                            PaternoD = SelectIngreso.IMPUTADO.PATERNO;
                            MaternoD = SelectIngreso.IMPUTADO.MATERNO;
                            NombreD = SelectIngreso.IMPUTADO.NOMBRE;
                            NoControlD = string.Empty;
                            var cama = SelectIngreso.CAMA;
                            IngresosD = SelectIngreso.ID_INGRESO;
                            UbicacionD = string.Format("{0}-{1}-{2}-{3}",
                                                                          cama.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                                                          cama.CELDA.SECTOR.DESCR.Trim(),
                                                                          cama.ID_CELDA.Trim(),
                                                                          cama.ID_CAMA);
                            TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD.DESCR;
                            FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO.Value;
                            ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA.DESCR;
                            EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                            if (SelectIngreso.INGRESO_BIOMETRICO != null)
                            {
                                var biometrico = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                if (biometrico != null)
                                    ImagenIngreso = biometrico.BIOMETRICO;
                                else
                                    ImagenIngreso = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenIngreso = new Imagenes().getImagenPerson();
                            #endregion
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        }
                        break;
                    #endregion
                    case "ver_historico":
                        VerHistorico();
                        break;
                    case "guardar_menu":
                        Guardar();
                        break;
                    case "reporte_menu":
                        if (!PImprimir)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        ImprimirHistorico();
                        break;
                    case "limpiar_menu":
                        StaticSourcesViewModel.SourceChanged = false;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotaTecnicaView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new NotaTecnicaViewModel();
                        break;
                    case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                        break;

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionaropción", ex);
            }
        }

        private async void WindowLoad(NotaTecnicaView obj)
        {
            try
            {
                if (obj != null)
                {
                    estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
                    //Reporte = obj.Report;
                    //obj.editor.Loaded += (s, e) => { };
                    //Editor = obj.editor;
                    //GenerarWordAtencionesRecibidas();
                    //CargarPlantilla();
                    //CargarHistorialAtencionesRecibidas();
                    SetValidaciones();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla.", ex);
            }
        }

        private async void ModelEnter(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }
                if (obj != null)
                {
                    //if (!PConsultar)
                    //{
                    //    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    //    return;
                    //}
                    if (!obj.GetType().Name.Equals("String"))
                    {
                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    NombreBuscar = textbox.Text;
                                    NombreD = NombreBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    ApellidoPaternoBuscar = PaternoD;
                                    ApellidoMaternoBuscar = MaternoD;
                                    AnioBuscar = AnioD;
                                    FolioBuscar = FolioD;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    ApellidoPaternoBuscar = textbox.Text;
                                    PaternoD = ApellidoPaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    NombreBuscar = NombreD;
                                    ApellidoMaternoBuscar = MaternoD;
                                    AnioBuscar = AnioD;
                                    FolioBuscar = FolioD;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    ApellidoMaternoBuscar = textbox.Text;
                                    MaternoD = ApellidoMaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    NombreBuscar = NombreD;
                                    ApellidoPaternoBuscar = PaternoD;
                                    AnioBuscar = AnioD;
                                    FolioBuscar = FolioD;
                                    break;
                                case "FolioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        FolioBuscar = int.Parse(textbox.Text);
                                    else
                                        FolioBuscar = null;
                                    AnioBuscar = AnioD;
                                    NombreBuscar = NombreD;
                                    ApellidoPaternoBuscar = PaternoD;
                                    ApellidoMaternoBuscar = MaternoD;
                                    break;
                                case "AnioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        AnioBuscar = int.Parse(textbox.Text);
                                    else
                                        AnioBuscar = null;
                                    FolioBuscar = FolioD;
                                    NombreBuscar = NombreD;
                                    ApellidoPaternoBuscar = PaternoD;
                                    ApellidoMaternoBuscar = MaternoD;
                                    break;
                            }
                        }

                    }
                }

                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                if (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar) || FolioBuscar != null || AnioBuscar != null)
                {
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                }
                
                if (ListExpediente.Count == 1)
                {
                    SelectExpediente = ListExpediente.FirstOrDefault();

                    if (SelectExpediente != null)
                    {
                        if (SelectExpediente.INGRESO != null)
                        {
                            SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodosCentro(GlobalVar.gCentro, estatus_inactivos, ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
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
        #endregion

        #region Listado:
        private void CargarListas()
        {
            try
            {
                ConfiguraPermisos();
                LstAreaTecnica = new ObservableCollection<AREA_TECNICA>(new cAreaTecnica().ObtenerTodo(string.Empty, "S"));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstAreaTecnica.Insert(0, new AREA_TECNICA() { ID_TECNICA = -1, DESCR = "SELECCIONE" });
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar lista.", ex);
            }
        }

        private void ObtenerIngreso() {
            try 
            {
                if (SelectedAtencionCita != null)
                {
                    if (SelectedAtencionCita.INGRESO != null)
                    {
                        //Anio = SelectedAtencionCita.INGRESO.ID_ANIO;
                        //Folio = SelectedAtencionCita.INGRESO.ID_IMPUTADO;
                        //Paterno = SelectedAtencionCita.INGRESO.IMPUTADO.PATERNO;
                        //Materno = SelectedAtencionCita.INGRESO.IMPUTADO.MATERNO;
                        //Nombre = SelectedAtencionCita.INGRESO.IMPUTADO.NOMBRE;
                        if (SelectedAtencionCita.INGRESO.INGRESO_BIOMETRICO != null)
                        { 
                            var obj = SelectedAtencionCita.INGRESO.INGRESO_BIOMETRICO.Where((w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG)).FirstOrDefault();
                            if (obj != null)
                                ImagenIngreso = obj.BIOMETRICO;
                        }
                        
                        //Obtener Historico
                        //LstAtencionRecibidaHistorico = new ObservableCollection<ATENCION_RECIBIDA>(new cAtencionRecibida().ObtenerTodoHistorico(SelectedAtencionCita.INGRESO.ID_CENTRO, SelectedAtencionCita.INGRESO.ID_ANIO, SelectedAtencionCita.INGRESO.ID_IMPUTADO));
                        //HistoricoVisible = LstAtencionRecibidaHistorico.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    }
                    //CargarPlantilla();
                    //GenerarReporte();
                }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener ingreso.", ex);
            }
        }
        #endregion

        #region Huellas
        private async void OnBuscarPorHuella(string obj = "")
        {
            await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));

            await TaskEx.Delay(400);

            var nRet = -1;
            var bandera = true;
            var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaEstatusAdministrativo;
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

                    Imputado = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro != null ? ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro.Imputado : null;

                    if (Imputado == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de autentificar al interno por medio de huella");
                        return;
                    }
                    else
                    { 
                        if(SelectedAtencionCita != null)
                        {
                            if (SelectedAtencionCita.INGRESO != null)
                            {
                                if (Imputado.ID_CENTRO == SelectedAtencionCita.INGRESO.ID_CENTRO && Imputado.ID_ANIO == SelectedAtencionCita.INGRESO.ID_ANIO && Imputado.ID_IMPUTADO == SelectedAtencionCita.ID_IMPUTADO)
                                {
                                    TabControlEnabled = MenuGuardarEnabled = true;
                                    BHuellasEnabled = false;
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación", "El interno no coincide con el solicitado");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar busqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
            //AceptarBusquedaHuellaFocus = true;
        }
        #endregion

        #region Atencion Recibida
        private void Guardar() 
        {
            try
            {
                if (!PInsertar)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    return;
                }
                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un Ingreso");
                    return;
                }

                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos. "+base.Error);
                    return;
                }

                var hoy = Fechas.GetFechaDateServer;
                var responsable = new cUsuario().ObtenerUsuario(GlobalVar.gUsr);
                var a = new ATENCION_SOLICITUD();
                //a.ID_ATENCION = 
                a.ID_TECNICA = AreaTecnica;
                //a.ID_AREA = 
                a.SOLICITUD_FEC = hoy;
                a.ACTIVIDAD = string.Empty;
                a.ID_CENTRO = GlobalVar.gCentro;
                a.ESTATUS = 0;

                a.ATENCION_INGRESO.Add(new ATENCION_INGRESO()
                {
                    ID_CENTRO_UBI = GlobalVar.gCentro,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    REGISTRO_FEC = hoy,
                    ESTATUS = 1
                });
                
                var obj = new ATENCION_CITA();
                obj.ID_CENTRO_UBI = GlobalVar.gCentro;
                obj.ID_CENTRO_AT_SOL = GlobalVar.gCentro;
                obj.ID_CENTRO = SelectIngreso.ID_CENTRO;
                obj.ID_ANIO = SelectIngreso.ID_ANIO;
                obj.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                obj.ID_INGRESO = SelectIngreso.ID_INGRESO;
                obj.CITA_FECHA_HORA = hoy;
                obj.CITA_HORA_TERMINA = hoy;
                obj.ID_RESPONSABLE = responsable.ID_PERSONA;
                //ID_AREA
                obj.ESTATUS = "A";
                obj.ID_USUARIO = GlobalVar.gUsr;

                var ar =  new ATENCION_RECIBIDA();
                ar.ID_CENTRO_UBI = GlobalVar.gCentro;
                ar.ID_USUARIO = GlobalVar.gUsr;
                ar.ATENCION_FEC = hoy;
                //ar.ATENCION_RECIBIDA
                ar.ATENCION_RECIBIDA_TXT =AtencionTxt; 

                obj.ATENCION_RECIBIDA = ar;
                
                a.ATENCION_CITA.Add(obj);
                
                a.ID_ATENCION = new cAtencionSolicitud().Agregar(a);
                if (a.ID_ATENCION > 0)
                {
                    MenuGuardarEnabled = false;
                    new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la información");
                }

                #region Comentado
                //var obj = new ATENCION_RECIBIDA();
                    //obj.ID_CITA = SelectedAtencionCita.ID_CITA;
                    //obj.ID_USUARIO = GlobalVar.gUsr;
                    //obj.ATENCION_FEC = Fechas.GetFechaDateServer;
                    ////byte[] data;
                    ////Editor.Save(out data, BinaryStreamType.WordprocessingML);
                    ////obj.ATENCION_RECIBIDA1 = data;
                    //obj.ATENCION_RECIBIDA_TXT = AtencionTxt;

                    //if (SelectedAtencionRecibida == null)
                    //{
                    //    if (new cAtencionRecibida().Agregar(obj))
                    //    {
                    //        SelectedAtencionRecibida = obj;
                    //        MenuGuardarEnabled = false;
                    //        #region Historico
                    //        LstAtencionRecibidaHistorico = new ObservableCollection<ATENCION_RECIBIDA>(new cAtencionRecibida().ObtenerTodoHistorico(SelectedAtencionCita.INGRESO.ID_CENTRO, SelectedAtencionCita.INGRESO.ID_ANIO, SelectedAtencionCita.INGRESO.ID_IMPUTADO));
                    //        HistoricoVisible = LstAtencionRecibidaHistorico.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    //        #endregion
                    //        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                    //    }
                    //    else
                    //        new Dialogos().ConfirmacionDialogo("Error", "Ocurrio unj error al guardar la información");
                    //}
                    //else
                    //{
                    //    if (new cAtencionRecibida().Actualizar(obj))
                    //    {
                    //        SelectedAtencionRecibida = obj;
                    //        MenuGuardarEnabled = false;
                    //        #region Historico
                    //        LstAtencionRecibidaHistorico = new ObservableCollection<ATENCION_RECIBIDA>(new cAtencionRecibida().ObtenerTodoHistorico(SelectedAtencionCita.INGRESO.ID_CENTRO, SelectedAtencionCita.INGRESO.ID_ANIO, SelectedAtencionCita.INGRESO.ID_IMPUTADO));
                    //        HistoricoVisible = LstAtencionRecibidaHistorico.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    //        #endregion
                    //        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                    //    }
                    //    else
                    //        new Dialogos().ConfirmacionDialogo("Error", "Ocurrio unj error al guardar la información");
                //}
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
            }
        }
        #endregion

        #region Historico
        private void VerHistorico()
        {
            try
            {
                if (SelectedAtencionRecibidaHistorico != null)
                {
                    var tc = new TextControlView();
                    tc.Closed += (s, e) =>
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    };
                    tc.editor.Loaded += (s, e) =>
                    {
                       //DOCX
                        tc.editor.Load(SelectedAtencionRecibidaHistorico.ATENCION_RECIBIDA1, TXTextControl.BinaryStreamType.WordprocessingML);
                    };
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Owner = PopUpsViewModels.MainWindow;
                    tc.Show();
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar la atencion recibida a visualizar");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información historica.", ex);
            }
        }

        private void CargarHistorialAtencionesRecibidas() 
        {
            try
            {
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                    var reporte = new List<cReporte>();
                    reporte.Add(new cReporte()
                    {
                        Encabezado1 = Parametro.ENCABEZADO1,
                        Encabezado2 = Parametro.ENCABEZADO2,
                        Encabezado3 = centro.DESCR.Trim(),
                        Encabezado4 = "Historial de Atención de Citas del Área " + SelectedAtencionCita.ATENCION_SOLICITUD.AREA_TECNICA.DESCR.Trim(),
                        Logo1 = Parametro.REPORTE_LOGO1,
                        Logo2 = Parametro.REPORTE_LOGO2

                    });

                    var generales = new List<cHistorialAtencionCitasGenerales>();
                    if (SelectedAtencionCita.INGRESO != null)
                    {
                        generales.Add(new cHistorialAtencionCitasGenerales()
                        {
                            Expediente = string.Format("{0}/{1}", SelectedAtencionCita.INGRESO.ID_ANIO, SelectedAtencionCita.INGRESO.ID_IMPUTADO),
                            Nombre = string.Format("{0} {1} {2}", 
                            SelectedAtencionCita.INGRESO.IMPUTADO.NOMBRE.Trim(),
                            !string.IsNullOrEmpty(SelectedAtencionCita.INGRESO.IMPUTADO.PATERNO) ? SelectedAtencionCita.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(SelectedAtencionCita.INGRESO.IMPUTADO.MATERNO) ? SelectedAtencionCita.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty),
                        });
                    }

                    var historial = new List<cHistorialAtencionCitas>();
                    var historico = new cAtencionRecibida().ObtenerTodoHistorico(
                        SelectedAtencionCita.INGRESO.ID_CENTRO, 
                        SelectedAtencionCita.INGRESO.ID_ANIO,
                        SelectedAtencionCita.INGRESO.ID_IMPUTADO, 
                        SelectedAtencionCita.ATENCION_SOLICITUD.ID_TECNICA).OrderBy(w => w.ATENCION_FEC);
                    if (historico != null)
                    {
                        foreach (var h in historico)
                        {
                            historial.Add(new cHistorialAtencionCitas() { Fecha = h.ATENCION_FEC.Value.ToString("dd/MM/yyyy"), Atencion = h.ATENCION_RECIBIDA_TXT });
                        }
                    }

                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = historial;
                    Reporte.LocalReport.DataSources.Add(rds1);

                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = reporte;
                    Reporte.LocalReport.DataSources.Add(rds2);

                    Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds3.Name = "DataSet3";
                    rds3.Value = generales;
                    Reporte.LocalReport.DataSources.Add(rds3);

                    Reporte.LocalReport.ReportPath = "Reportes/rAtencionCitaHistorial.rdlc";
                    Reporte.RefreshReport();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el ingreso.", ex);
            }
        }

        private void ImprimirHistorico()
        {
            try
            {
                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                    return;
                }
                if (AreaTecnica == -1)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un área técnica");
                    return;
                }
                var v = new ReporteView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                v.Owner = PopUpsViewModels.MainWindow;
                v.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                #region Historial
                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var reporte = new List<cReporte>();
                reporte.Add(new cReporte()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = centro.DESCR.Trim(),
                    Encabezado4 = "Historial de Atención de Citas del Área " + SelectedAreaTecnica.DESCR.Trim(),
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2

                });

                var generales = new List<cHistorialAtencionCitasGenerales>();
                
                    generales.Add(new cHistorialAtencionCitasGenerales()
                    {
                        Expediente = string.Format("{0}/{1}", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO),
                        Nombre = string.Format("{0} {1} {2}",
                        SelectIngreso.IMPUTADO.NOMBRE.Trim(),
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty),
                    });
                
                var historial = new List<cHistorialAtencionCitas>();
                var historico = new cAtencionRecibida().ObtenerTodoHistorico(
                    SelectIngreso.ID_CENTRO,
                    SelectIngreso.ID_ANIO,
                    SelectIngreso.ID_IMPUTADO,
                    AreaTecnica).OrderBy(w => w.ATENCION_FEC);
                if (historico != null)
                {
                    foreach (var h in historico)
                    {
                        historial.Add(new cHistorialAtencionCitas() { Fecha = h.ATENCION_FEC.Value.ToString("dd/MM/yyyy"), Atencion = h.ATENCION_RECIBIDA_TXT });
                    }
                }

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = historial;
                v.ReporteViewer.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = reporte;
                v.ReporteViewer.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = generales;
                v.ReporteViewer.LocalReport.DataSources.Add(rds3);

                v.ReporteViewer.LocalReport.ReportPath = "Reportes/rAtencionCitaHistorial.rdlc";
                v.ReporteViewer.RefreshReport();
                #endregion
                v.Show();
            }
            catch (Exception ex)
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir historico", ex);
            }
        }
        //private void GenerarWordAtencionesRecibidas() 
        //{
        //    try
        //    {
        //        MemoryStream stream = new MemoryStream();
        //        using (DocX document = DocX.Create(stream))
        //        {
        //            #region Configuracion del Documento
        //            document.MarginLeft = 40;
        //            document.MarginRight = 40;
        //            #endregion

        //            #region Header
        //            document.AddHeaders();
        //            document.AddFooters();
        //            // Get the default Header for this document.
        //            Header header_default = document.Headers.odd;
        //            // Insert a Paragraph into the default Header.
        //            var headLineFormat = new Formatting();
        //            headLineFormat.FontFamily = new System.Drawing.FontFamily("Arial Black");

        //            Novacode.Paragraph p1 = header_default.InsertParagraph();
        //            p1.Alignment = Alignment.right;
        //            p1.Append("Secretaria de Seguridad Publica").Bold();
        //            #endregion

        //            #region Body
        //            var historico = new cAtencionRecibida().ObtenerTodoHistorico(SelectedAtencionCita.INGRESO.ID_CENTRO, SelectedAtencionCita.INGRESO.ID_ANIO, SelectedAtencionCita.INGRESO.ID_IMPUTADO).OrderBy(w => w.ATENCION_FEC);
        //            if (historico != null)
        //            {
        //                Novacode.Paragraph pb = document.InsertParagraph();
        //                pb.AppendLine();
        //                Novacode.Table t = document.AddTable(2, 1);
        //                float[] x = { 350, 350 };
        //                t.SetWidths(x);
        //                t.Alignment = Alignment.center;
        //                t.Design = TableDesign.TableNormal;
        //                int i = 0;

        //                foreach (var h in historico)
        //                {
        //                        t.Rows[i].Cells[0].Paragraphs.First().AppendLine(h.ATENCION_FEC.Value.ToString("dd/MM/yyyy")).Bold();
        //                        t.Rows[i].Cells[0].TextDirection = TextDirection.right;
        //                        i++;
        //                        t.InsertRow();
        //                        t.Rows[i].Cells[0].Paragraphs.First().AppendLine(h.ATENCION_RECIBIDA_TXT);
        //                        i++;
        //                        t.InsertRow();
        //                }
        //                document.InsertTable(t);
        //            }
        //            #endregion

        //            #region Footer

        //            Footer footer_default = document.Footers.odd;
        //            Novacode.Paragraph p7 = footer_default.InsertParagraph();
        //            //p7.Append("pie de pagina").Bold();
        //            //p6.AppendPageCount(PageNumberFormat.normal);
        //            //p6.AppendPageNumber(PageNumberFormat.normal);
        //            //p6.Append().Bold();
        //            #endregion
        //            document.Save();

        //            byte[] bytes = stream.ToArray();
        //            Editor.Load(bytes, TXTextControl.BinaryStreamType.WordprocessingML);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error generar historico de atenciones recibidas.", ex);
        //    }
        //}
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.NOTA_TECNICA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

        #region Reporte
        private void CargarPlantilla() 
        {
            try
            {
                var diccionario = new Dictionary<string, string>();
                diccionario.Add("<<encabezado1>>", encabezado1);
                diccionario.Add("<<encabezado2>>", encabezado2);
                diccionario.Add("<<encabezado3>>", "Atención de Interno");
                diccionario.Add("<<expediente>>",string.Format("{0}/{1}", SelectedAtencionCita.ID_ANIO,selectedAtencionCita.ID_IMPUTADO));
                var imputado = SelectedAtencionCita.INGRESO.IMPUTADO;
                diccionario.Add("<<nombre>>",string.Format("{0} {1} {2}",imputado.NOMBRE.Trim(),!string.IsNullOrEmpty(imputado.PATERNO) ? imputado.PATERNO.Trim() : string.Empty,!string.IsNullOrEmpty(imputado.MATERNO) ? imputado.MATERNO.Trim() : string.Empty));
                var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.ATENCION_INTERNO);
                if (documento == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la plantilla del documento");
                    return;
                }

                var d = new cWord().FillFieldsDocx(documento.DOCUMENTO, diccionario);
                //var ib = selectedAtencionCita.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                //var d = new cWord().FillFields(Parametro.PLANTILLA_ATENCION_INTERNO, diccionario);//.FillFields(Parametro.PLANTILLA_ATENCION_INTERNO, diccionario, logo1, logo2, ib.BIOMETRICO);
                Editor.Load(d, BinaryStreamType.WordprocessingML);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar reporte de atención recibida", ex);
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
