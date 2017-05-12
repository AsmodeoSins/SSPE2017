using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class HistoriaClinicaViewModel : FingerPrintScanner
    {
        private int? _IdImputado;
        private short? _IdAnio;
        private short? _IdIngreso;
        private short? _IdCentro;
        private bool bScan;
        private bool bScanAislado;

        public HistoriaClinicaViewModel(int? Imputado, short? _Anio, short? _Ingreso, short? _Centro)
        {
            _IdImputado = Imputado;
            _IdAnio = _Anio;
            _IdIngreso = _Ingreso;
            _IdCentro = _Centro;
            if (_IdImputado.HasValue && _IdAnio.HasValue || _IdIngreso.HasValue || _IdCentro.HasValue)
            {
                SelectIngreso = new cIngreso().GetData().FirstOrDefault(x => x.ID_INGRESO == _IdIngreso && x.ID_IMPUTADO == _IdImputado && x.ID_ANIO == _IdAnio && x.ID_CENTRO == _IdCentro);
                SelectExpediente = SelectIngreso.IMPUTADO;
                clickSwitch("buscar_seleccionar");
            };
        }

        private PdfViewer PDFViewer;
        private async void Load_Window(HistoriaClinicaView Window)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ConfiguraPermisos();
                    PrepararListas();
                    Lista_Sources = escaner.Sources();
                    if (Lista_Sources.Count > 0) SelectedSource = Lista_Sources.Where(w => w.Default).SingleOrDefault();
                    HojasMaximo = string.Format("Escaneo permitido: {0} documentos máximo.", escaner.HojasMaximo);
                    PDFViewer = PopUpsViewModels.MainWindow.MostrarPDFView.pdfViewer;
                    StaticSourcesViewModel.SourceChanged = false;
                });

                escaner.EscaneoCompletado += delegate
                {
                    if (bScan)
                    {
                        DocumentoDigitalizado = escaner.ScannedDocument;

                        if (AutoGuardado)
                            if (DocumentoDigitalizado != null)
                                GuardarDocumento();
                            else
                            {
                                if (SelectedTipoDocumento != null && ListTipoDocumento.FirstOrDefault(w => w.ID_DOCTO == SelectedTipoDocumento.ID_DOCTO) != null)
                                    ListTipoDocumento = new ObservableCollection<HC_DOCUMENTO_TIPO>();
                            }

                        escaner.Dispose();
                    }

                    if (bScanAislado)
                    {
                        DocumentoDigitalizado = escaner.ScannedDocument;

                        if (AutoGuardado)
                            if (DocumentoDigitalizado != null)
                                GuardarDocumentoAislado();

                        escaner.Dispose();
                    }
                };
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
                case "limpiar_odonto_seguimiento":
                    LimpiarDientesSeguimiento();
                    break;
                case "seleccionar_archivo_dental":
                    SeleccionaImagenDental();
                    break;

                case "guardar_imagenes_dentales":
                    IdTipoImagenDental = -1;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_IMAGENES_HISTORIA_CLINICA_DENTAL);
                    break;

                case "cancelar_imagen_dental":
                    IdTipoImagenDental = -1;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_IMAGENES_HISTORIA_CLINICA_DENTAL);
                    break;

                case "agregar_odonto_seguimiento":
                    if (SelectIngreso != null)
                        GuardaDientesSeguimiento();
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }
                    break;

                case "subir_imagenes_dentales":
                    if (SelectIngreso != null)
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_IMAGENES_HISTORIA_CLINICA_DENTAL);
                        IdTipoImagenDental = -1;
                        //SeleccionaImagenDental();
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }

                    break;
                #endregion
                #region MENU
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "ficha_menu":
                    break;
                case "ayuda_menu":
                    break;
                case "reporte_menu":
                    if (SelectIngreso != null)
                        if (!EsDentista)
                            Imprime();
                        else
                            ImprimeHistoriaClinicaDental();
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }

                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new HistoriaClinicaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.HistoriaClinicaViewModel(null, null, null, null);
                    break;
                case "guardar_menu":
                    if (SelectExpediente == null)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar un imputado para guardar la historia clínica.");
                        }));
                        return;
                    }
                    if (SelectIngreso == null)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar un imputado con un ingreso vigente para guardar la historia clínica.");
                        }));
                        return;
                    }

                    if (!EsDentista)
                    {
                        SetValidacionesTodas();
                        if (base.HasErrors)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación!", base.Error);
                            }));
                            return;
                        }

                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Ingresando Historia Clínica Médica", () => GuardarHistoriaClinica()))
                        {
                            StaticSourcesViewModel.SourceChanged = false;
                            (new Dialogos()).ConfirmacionDialogo("Éxito!", "Se ha guardado la historia clínica médica con éxito");
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error!", "Surgió un error al ingresar la historia clínica medica.");
                        };
                    }
                    else
                    {
                        ValidacionesHistoriaClinicaDental();
                        if (base.HasErrors)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación!", base.Error);
                            }));
                            return;
                        }

                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Ingresando Historia Clínica Dental", () => GuardarHistoriaClinicaDental()))
                        {
                            StaticSourcesViewModel.SourceChanged = false;
                            (new Dialogos()).ConfirmacionDialogo("Éxito!", "Se ha guardado la historia clínica dental con éxito");
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error!", "Surgió un error al ingresar la historia clínica dental.");
                        };
                    }
                    break;
                case "buscar_menu":
                    NombreD = NombreBuscar = PaternoD = ApellidoPaternoBuscar = MaternoD = ApellidoMaternoBuscar = string.Empty;
                    AnioD = FolioD = FolioBuscar = AnioBuscar = new int?();
                    ImagenIngreso = new Imagenes().getImagenPerson();
                    var ing = SelectIngreso;
                    SelectIngresoAuxiliar = ing;
                    var exp = SelectExpediente;
                    SelectExpedienteAuxiliar = exp;
                    SelectExpediente = null;
                    EmptyIngresoVisible = true;
                    EmptyExpedienteVisible = true;
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>(); //new ObservableCollection<IMPUTADO>();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                #endregion

                #region BUSCAR IMPUTADO
                case "nueva_busqueda":
                    NombreD = NombreBuscar = PaternoD = ApellidoPaternoBuscar = MaternoD = ApellidoMaternoBuscar = string.Empty;
                    AnioD = FolioD = FolioBuscar = AnioBuscar = new int?();
                    ImagenIngreso = new Imagenes().getImagenPerson();
                    SelectExpediente = null;
                    EmptyIngresoVisible = true;
                    EmptyExpedienteVisible = true;
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>(); //new ObservableCollection<IMPUTADO>();
                    break;
                case "buscar_visible":
                    NombreD = NombreBuscar = PaternoD = ApellidoPaternoBuscar = MaternoD = ApellidoMaternoBuscar = string.Empty;
                    AnioD = FolioD = FolioBuscar = AnioBuscar = new int?();
                    ImagenIngreso = new Imagenes().getImagenPerson();
                    var ing2 = SelectIngreso;
                    SelectIngresoAuxiliar = ing2;
                    var exp2 = SelectExpediente;
                    SelectExpedienteAuxiliar = exp2;
                    SelectExpediente = null;
                    EmptyIngresoVisible = true;
                    EmptyExpedienteVisible = true;
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>(); //new ObservableCollection<IMPUTADO>();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    var expA = SelectExpedienteAuxiliar;
                    SelectExpediente = expA;
                    var ingA = SelectIngresoAuxiliar;
                    SelectIngreso = ingA;
                    //ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    try
                    {
                        if (SelectExpediente == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un expediente.");
                            return;
                        }
                        if (SelectIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un ingreso.");
                            return;
                        }
                        if (StaticSourcesViewModel.SourceChanged ? await new Dialogos().ConfirmarEliminar("Advertencia!",
                            "Existen cambios sin guardar, esta seguro que desea seleccionar a otro imputado?") != 1 : false)
                            return;
                        MenuGuardarEnabled = !PInsertar ? PEditar : true;
                        if (Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(a => a.HasValue ? a.Value == SelectIngreso.ID_ESTATUS_ADMINISTRATIVO : false))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no es uno activo.");
                            }));
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                            return;
                        }
                        if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no pertenece a este centro.");
                            return;
                        }

                        if (EsDentista)
                        {
                            if (SelectIngreso.HISTORIA_CLINICA == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación!", "Es necesario capturar historia clínica médica al imputado");
                                return;
                            }

                            if (SelectIngreso.HISTORIA_CLINICA.Count == 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación!", "Es necesario capturar historia clínica médica al imputado");
                                return;
                            }
                        }

                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            TabsEnabled = true;
                            GetDatosImputadoSeleccionado();
                        });

                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado.", ex);
                    }
                    break;
                case "aceptar_huella":
                    try
                    {
                        HuellaWindow.Hide();
                        if (ScannerMessage.Contains("Procesando..."))
                            return;
                        CancelKeepSearching = true;
                        isKeepSearching = true;
                        //await WaitForFingerPrints();
                        _IsSucceed = true;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                if (SelectRegistro == null)
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(async delegate
                                    {
                                        var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "Debes seleccionar un imputado.");
                                        HuellaWindow.Show();
                                    }));
                                    return;
                                }
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
                                    Application.Current.Dispatcher.Invoke((Action)(async delegate
                                    {
                                        var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                                        HuellaWindow.Show();
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
                                    Application.Current.Dispatcher.Invoke((Action)(async delegate
                                    {
                                        var respuesta = await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "El ingreso seleccionado no pertenece a su centro.");
                                        HuellaWindow.Show();
                                        //new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                                    }));
                                    return;
                                }
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                                if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado).TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay))
                                {
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        HuellaWindow.Close();
                                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                    }));

                                    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                        SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");

                                    return;
                                }

                                GetDatosImputadoSeleccionado();
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    HuellaWindow.Close();
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                }));
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
                    }
                    break;
                case "buscar_nip":
                    ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                    if (string.IsNullOrEmpty(TextNipBusqueda)) return;
                    HuellaWindow.Hide();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        try
                        {
                            //var tipo = (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO;
                            var auxiliar = new List<ResultadoBusquedaBiometrico>();
                            ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                            foreach (var s in new cImputado().ObtenerXNip(TextNipBusqueda))
                            {
                                var ingresobiometrico = s.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
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
                                    AMaterno = s.MATERNO.Trim(),
                                    APaterno = s.PATERNO.Trim(),
                                    Expediente = s.ID_ANIO + "/" + s.ID_IMPUTADO,
                                    Foto = FotoBusquedaHuella,
                                    Imputado = s,
                                    NIP = !string.IsNullOrEmpty(s.NIP) ? s.NIP : string.Empty,
                                    Nombre = s.NOMBRE.Trim(),
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

                case "agregar_docto":
                    PDFViewer = PopUpsViewModels.MainWindow.MostrarPDFView.pdfViewer;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZACION_SIMPLE);
                    break;

                case "cerrar_agregar_doctos":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DOCUMENTOS_HISTORIA_CLINICA);
                    break;
                case "guardar_documento_aislado":
                    GuardarDocumentoAislado();
                    break;

                case "documento_directo":
                    if (SelectExpediente == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un expediente.");
                        return;
                    }
                    if (SelectIngreso == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un ingreso.");
                        return;
                    }

                    TipoDocto = -1;
                    TipoArchivo = string.Empty;
                    PrepararListas();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DOCUMENTOS_HISTORIA_CLINICA);
                    break;

                case "agregar_documento_grid":
                    if (!string.IsNullOrEmpty(TipoArchivo) && TipoDocto != -1)
                    {
                        if (LstDocumentosActuales == null)
                            LstDocumentosActuales = new ObservableCollection<HISTORIA_CLINICA_DOCUMENTO>();

                        if (TipoArchivo == "F")
                        {
                            var _detalleArchivo = new SSP.Controlador.Catalogo.Justicia.cTipoDocumentosHistoriaClinica().GetData(x => x.ID_DOCTO == TipoDocto).FirstOrDefault();
                            LstDocumentosActuales.Add(new HISTORIA_CLINICA_DOCUMENTO
                                {
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_CENTRO,
                                    DOCUMENTO = null,
                                    FISICO = (short)eFisicoDigital.FISICO,
                                    ID_DOCTO = TipoDocto,
                                    HC_DOCUMENTO_TIPO = _detalleArchivo != null ? _detalleArchivo : null,
                                    ID_FORMATO = null
                                });
                        }
                        else
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DOCUMENTOS_HISTORIA_CLINICA);
                            PDFViewer = PopUpsViewModels.MainWindow.MostrarPDFView.pdfViewer;
                            DocumentoDigitalizado = null;
                            escaner.Hide();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZACION_SIMPLE);
                        }
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "Es necesario seleccione el tipo de archivo.");
                        return;
                    }

                    break;

                case "seleccionar_archivo":
                    SeleccionarArchivo();
                    break;

                case "cancelar_digitalizar_documentos_aislado":
                    CierraDigitalizacionIndividual();
                    break;

                #endregion
                #region Digitalizacion

                case "digitalizar_directo":
                    if (SelectIngreso == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar un ingreso valido para scanear.");
                        return;
                    };
                    ObtenerTipoDocumento();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                    _Directo = true;
                    break;
                case "digitalizar_const":
                    if (SelectIngreso == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar un ingreso valido para scanear.");
                        return;
                    };

                    ObtenerTipoDocumento();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                    _ConstanciaDoc = true;
                    break;

                case "Cancelar_digitalizar_documentos":
                    escaner.Hide();
                    DocumentoDigitalizado = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                    break;
                case "guardar_documento":
                    GuardarDocumento();
                    break;
                #endregion
                #region Proceso de botones
                case "agregar_patol":
                    AgregarPatologicos();
                    //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;

                case "remove_patol":
                    QuitarPatologico();
                    break;

                #endregion
                #region Patologicos
                case "guardar_ampliar_descripcion":
                    AgregarPatologicos(TextAmpliarDescripcion);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    TextAmpliarDescripcion = string.Empty;
                    break;

                case "cancelar_ampliar_descripcion":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    TextAmpliarDescripcion = string.Empty;
                    break;
                #endregion
                #region Documentos y visualizacion de documentos
                case "ver_documento_historia_clinica":
                    if (SelectedDocumentoActual != null)
                        MuestraDocumento(SelectedDocumentoActual);
                    break;

                case "eliminar_archivo_lista":
                    if (SelectedDocumentoActual != null)
                        if (LstDocumentosActuales.Remove(SelectedDocumentoActual))
                            SelectedDocumentoActual = null;
                        else
                            return;
                    break;
                case "cerrar_visualizador_documentos":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                    break;
                #endregion
            }
        }

        private async void MuestraDocumento(HISTORIA_CLINICA_DOCUMENTO Entity)
        {
            try
            {
                if (Entity.ID_FORMATO == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación!", "No existe documento adjunto.");
                    return;
                };

                var tc = new TextControlView();

                switch (Entity.ID_FORMATO)
                {
                    case (short)eFormatosDigitalizacion.DOCX: // docx
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                tc.editor.Load(Entity.DOCUMENTO, TXTextControl.BinaryStreamType.WordprocessingML);
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
                        break;

                    case (short)eFormatosDigitalizacion.XLS://xls
                        break;

                    case 3://pdf
                        //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        var _file = Entity.DOCUMENTO;
                        await Task.Factory.StartNew(() =>
                        {
                            var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                            File.WriteAllBytes(fileNamepdf, _file);
                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                PDFViewer.LoadFile(fileNamepdf);
                                PDFViewer.Visibility = Visibility.Visible;
                            }));
                        });

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                        //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        //tc.Owner = PopUpsViewModels.MainWindow;
                        //tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        //tc.Show();
                        break;

                    case (short)eFormatosDigitalizacion.DOC://doc
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                tc.editor.Load(Entity.DOCUMENTO, TXTextControl.BinaryStreamType.MSWord);
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
                        break;

                    case (short)eFormatosDigitalizacion.JPEG://jpeg
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                System.IO.MemoryStream ms = new System.IO.MemoryStream(Entity.DOCUMENTO);
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

                    case (short)eformatosPermitidos.JPG:
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                System.IO.MemoryStream ms = new System.IO.MemoryStream(Entity.DOCUMENTO);
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

                    case (short)eFormatosDigitalizacion.XLSX://xlsx
                        break;

                    default:
                        break;
                }
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }


        private void CierraDigitalizacionIndividual()
        {
            try
            {
                DocumentoDigitalizado = null;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZACION_SIMPLE);
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DOCUMENTOS_HISTORIA_CLINICA);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private bool GuardarHistoriaClinica()
        {
            try
            {
                var usr = new cUsuario().ObtenerTodos(GlobalVar.gUsr);
                if (!usr.Any())
                    return false;

                var fHoy = Fechas.GetFechaDateServer;
                var user = usr.FirstOrDefault();
                var historia = new HISTORIA_CLINICA
                {
                    AHF_HERMANOS_F = TextHermanosMujeres,
                    AHF_HERMANOS_M = TextHermanosHombres,
                    AHF_NOMBRE = string.Empty,
                    APP_MEDICAMENTOS_ACTIVOS = MedicamentosActivos,
                    ESTUDIO_FEC = fHoy,
                    APNP_ALCOHOLISMO = TextAlcoholismoNoPatologicos,
                    APNP_ALIMENTACION = TextAlimentacionNoPatologicos,
                    APNP_HABITACION = TextHabitacionNoPatologicos,
                    APNP_NACIMIENTO = TextNacimientoNoPatologicos,
                    APNP_TABAQUISMO = TextTabaquismoNoPatologicos,
                    APNP_TOXICOMANIAS = TextToxicomaniasNoPatologicos,
                    CARDIOVASCULAR = TextCardiovascular,
                    CEDULA_PROFESIONAL = user.EMPLEADO != null ? user.EMPLEADO.PERSONA != null ? user.EMPLEADO.PERSONA.EMPLEADO != null ? !string.IsNullOrEmpty(user.EMPLEADO.PERSONA.EMPLEADO.CEDULA) ? user.EMPLEADO.PERSONA.EMPLEADO.CEDULA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                    CONCLUSIONES = TextConclusiones,
                    CP_CAPACIDAD_TRATAMIENTO = IdCapacTrata.HasValue ? IdCapacTrata.Value != -1 ? IdCapacTrata : new short?() : new short?(),
                    CP_ETAPA_EVOLUTIVA = IdEtapaEvo.HasValue ? IdEtapaEvo.Value != -1 ? IdEtapaEvo : new short?() : new short?(),
                    CP_GRADO_AFECTACION = TextGradoAfectacion,
                    CP_GRAVEDAD = IdComplica.HasValue ? IdComplica.Value != -1 ? IdComplica : new short?() : new short?(),
                    CP_NIVEL_ATENCION = IdNivelReq.HasValue ? IdNivelReq.Value != -1 ? IdNivelReq : new short?() : new short?(),
                    CP_PRONOSTICO = TextPronostico,
                    CP_REMISION = IdPosibRemis.HasValue ? IdPosibRemis.Value != -1 ? IdPosibRemis : new short?() : new short?(),
                    DIGESTIVO = TextDigestivo,
                    DIRECTO = (short)eSINO.SI,
                    DOCTOR = user.EMPLEADO != null ? user.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2} ", user.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(user.EMPLEADO.PERSONA.NOMBRE) ? user.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty,
                             user.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(user.EMPLEADO.PERSONA.PATERNO) ? user.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty,
                             user.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(user.EMPLEADO.PERSONA.MATERNO) ? user.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty) : string.Empty : string.Empty,
                    EF_ABDOMEN = TextAbdomen,
                    EF_CABEZA = TextCabeza,
                    EF_CUELLO = TextCuello,
                    EF_DESCRIPCION = TextExploracionfisica,
                    ESTATUS = "T",
                    EF_ESTATURA = TextEstatura,
                    EF_EXTREMIDADES = TextExtremidades,
                    EF_GENITALES = TextGenitales,
                    EF_IMPRESION_DIAGNOSTICA = TextImpresionDiagnostica,
                    EF_PESO = TextPeso,
                    EF_PRESION_ARTERIAL = string.Format("{0} / {1}", Arterial1, Arterial2),
                    EF_PULSO = TextPulso,
                    EF_RECTO = TextRecto,
                    EF_RESPIRACION = TextRespiracion,
                    EF_RESULTADO_ANALISIS = TextResultadosAnalisisClinicos,
                    EF_RESULTADO_GABINETE = TextResultadosestudiosGabinete,
                    EF_TEMPERATURA = TextTemperatura,
                    EF_TORAX = TextTorax,
                    ENDOCRINO = TextEndocrino,
                    GENITAL_MUJERES = SelectIngreso.IMPUTADO.SEXO == "M" ? null : TextGenital,
                    GENITAL_HOMBRES = SelectIngreso.IMPUTADO.SEXO == "F" ? null : TextGenital,
                    HEMATICO_LINFACTICO = TextHematicoLinfatico,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    ID_RESPONSABLE = user.ID_PERSONA,
                    M65_ALTERACION_AUDITIVA = CheckAgudezaAuditiva,
                    M65_ALTERACION_OLFACION = CheckOlfacion,
                    M65_ALTERACION_VISOMOTRIZ = CheckCapacidadVisomotriz,
                    M65_ALTERACION_VISUAL = IdDisminucionVisua.HasValue ? IdDisminucionVisua.Value != -1 ? IdDisminucionVisua : new short?() : new short?(),
                    M65_OTROS = TextOtroDisminucionVisual,
                    M65_PARTICIPACION = IdCapacidadParticipacion.HasValue ? IdCapacidadParticipacion.Value != -1 ? IdCapacidadParticipacion : new short?() : new short?(),
                    M65_TRAS_ANIMO = IdEstadoAnimo.HasValue ? IdEstadoAnimo.Value != -1 ? IdEstadoAnimo : new short?() : new short?(),
                    M65_TRAS_ATENCION = IdTranstornosAtencion.HasValue ? IdTranstornosAtencion.Value != -1 ? IdTranstornosAtencion : new short?() : new short?(),
                    M65_TRAS_COMPRENSION = IdTranstornosComprension.HasValue ? IdTranstornosComprension.Value != -1 ? IdTranstornosComprension : new short?() : new short?(),
                    M65_TRAS_DEMENCIAL = CheckDemencial,
                    M65_TRAS_MEMORIA = IdTranstornosMemoria.HasValue ? IdTranstornosMemoria.Value != -1 ? IdTranstornosMemoria : new short?() : new short?(),
                    M65_TRAS_ORIENTACION = IdTranstornosOrientacion.HasValue ? IdTranstornosOrientacion.Value != -1 ? IdTranstornosOrientacion : new short?() : new short?(),


                    MU_MENARQUIA = SelectIngreso.IMPUTADO.SEXO == "M" ? null : CheckMenarquia,

                    MUSCULO_ESQUELETICO = TextMusculoEsqueletico,
                    NERVIOSO = TextNervioso,
                    PADECIMIENTO_ACTUAL = TextPadecimientoActual,
                    PIEL_ANEXOS = TextPielAnexos,
                    RES_NOMBRE = string.Empty,
                    RESPIRATORIO = TextRespiratorio,
                    SINTOMAS_GENERALES = TextSintomasGenerales,
                    TERAPEUTICA_PREVIA = TextTerapeuticaPrevia,
                    URINARIO = TextUrinario,
                    APNP_ALCOHOLISMO_OBSERV = ObservacionesAlcohlismo,
                    APNP_TABAQUISMO_OBSERV = ObservacionesTabaquismo,
                    APNP_TOXICOMANIAS_OBSERV = ObservacionesToxicomanias
                };

                var _ginecoObstetricos = new HISTORIA_CLINICA_GINECO_OBSTRE()
                {
                    ABORTO = TextAbortos,
                    ABORTO_MODIFICADO = string.Empty,
                    ANIOS_RITMO = TextAniosRitmo,
                    ANIOS_RITMO_MODIFICADO = string.Empty,
                    CESAREA = TextCesareas,
                    CESAREA_MODIFICADO = string.Empty,
                    CONTROL_PRENATAL = IdControlPreN,
                    CONTROL_PRENATAL_MODIFICADO = string.Empty,
                    DEFORMACION = TextDeformacionesOrganicas,
                    DEFORMACION_MODIFICADO = string.Empty,
                    EMBARAZO = TextEmbarazos,
                    EMBARAZO_MODIFICADO = string.Empty,
                    FECHA_PROBABLE_PARTO = FechaProbParto,
                    FECHA_PROBABLE_PARTO_MOD = string.Empty,
                    FUENTE = "HC",
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    MOMENTO_REGISTRO = "EI",
                    PARTO = TextPartos,
                    PARTO_MODIFICADO = string.Empty,
                    REGISTRO_FEC = Fechas.GetFechaDateServer,
                    ULTIMA_MENS_MODIFICADO = string.Empty,
                    ID_CONTROL_PRENATAL = IdSelectedControlP,
                    ULTIMA_MENSTRUACION_FEC = FechaUltimaMenstruacion,
                    ID_CONSEC = 0,//Invalidos a proposito
                    ID_GINECO = 0//Invalidos a proposito
                };

                //Pendiente de modificar
                historia.HISTORIA_CLINICA_GINECO_OBSTRE.Add(_ginecoObstetricos);

                ICollection<HISTORIA_CLINICA_PATOLOGICOS> _detallePatos = new Collection<HISTORIA_CLINICA_PATOLOGICOS>();
                if (LstCondensadoPatologicos != null && LstCondensadoPatologicos.Any())
                {
                    short _consecutivo = 1;
                    foreach (var item in LstCondensadoPatologicos)
                    {
                        HISTORIA_CLINICA_PATOLOGICOS pato = new HISTORIA_CLINICA_PATOLOGICOS();
                        pato.ID_ANIO = SelectIngreso.ID_ANIO;
                        pato.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        pato.ID_CONSEC = _consecutivo;
                        pato.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        pato.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        pato.ID_PATOLOGICO = item.ID_PATOLOGICO;
                        pato.MOMENTO_DETECCION = item.MOMENTO_DETECCION;
                        pato.OTROS_DESCRIPCION = item.OTROS_DESCRIPCION;
                        pato.RECUPERADO = item.RECUPERADO == "True" ? "S" : "N";
                        pato.REGISTRO_FEC = item.REGISTRO_FEC;
                        pato.OBSERVACIONES = item.OBSERVACIONES;
                        _detallePatos.Add(pato);
                        _consecutivo++;
                    };

                    historia.HISTORIA_CLINICA_PATOLOGICOS.Clear();
                    historia.HISTORIA_CLINICA_PATOLOGICOS = _detallePatos;
                };

                ICollection<HISTORIA_CLINICA_DOCUMENTO> _detalleDocumentos = new Collection<HISTORIA_CLINICA_DOCUMENTO>();
                if (LstDocumentosActuales != null && LstDocumentosActuales.Any())
                {
                    foreach (var item in LstDocumentosActuales)
                    {
                        var Documento = new HISTORIA_CLINICA_DOCUMENTO()
                        {
                            DOCUMENTO = item.DOCUMENTO,
                            FISICO = item.FISICO,
                            ID_ANIO = item.ID_ANIO,
                            ID_CENTRO = item.ID_CENTRO,
                            ID_DOCTO = item.ID_DOCTO,
                            ID_IMPUTADO = item.ID_IMPUTADO,
                            ID_INGRESO = item.ID_INGRESO,
                            ID_FORMATO = item.ID_FORMATO
                        };

                        _detalleDocumentos.Add(Documento);
                    };

                    historia.HISTORIA_CLINICA_DOCUMENTO.Clear();
                    historia.HISTORIA_CLINICA_DOCUMENTO = _detalleDocumentos;
                };

                #region detalle patologico
                ObservableCollection<HISTORIA_CLINICA_FAMILIAR> lstFamiliares = new ObservableCollection<HISTORIA_CLINICA_FAMILIAR>();
                HISTORIA_CLINICA_FAMILIAR Padre = new HISTORIA_CLINICA_FAMILIAR()
                {
                    AHF_ALERGIAS = IsCheckedAlergiPadre,
                    AHF_CA = IsCheckedCAPadre,
                    AHF_CARDIACOS = IsCheckedCardiPadre,
                    AHF_DIABETES = IsCheckedDiabPadre,
                    AHF_EDAD = TextEdadPadre,
                    AHF_EPILEPSIA = IsCheckedEpiPadre,
                    AHF_FALLECIMIENTO_CAUSA = CausaMuertePadre,
                    AHF_FALLECIMIENTO_FEC = CuandoMuertePadre,
                    AHF_HIPERTENSIVO = IsCheckedHipertPadre,
                    AHF_MENTALES = IsCheckedMentPadre,
                    AHF_SANO = CheckPadrePadece,
                    AHF_TB = IsCheckedTBPadre,
                    AHF_VIVE = CheckPadreVive,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    ID_TIPO_REFERENCIA = (short)eFamiliares.PADRE
                };

                lstFamiliares.Add(Padre);

                HISTORIA_CLINICA_FAMILIAR Madre = new HISTORIA_CLINICA_FAMILIAR()
                {
                    AHF_ALERGIAS = IsCheckedAlergiMadre,
                    AHF_CA = IsCheckedCAMadre,
                    AHF_CARDIACOS = IsCheckedCardiMadre,
                    AHF_DIABETES = IsCheckedDiabMadre,
                    AHF_EDAD = TextEdadMadre,
                    AHF_EPILEPSIA = IsCheckedEpiMadre,
                    AHF_FALLECIMIENTO_FEC = CuandoMuerteMadre,
                    AHF_HIPERTENSIVO = IsCheckedHipertMadre,
                    AHF_FALLECIMIENTO_CAUSA = CausaMuerteMadre,
                    AHF_MENTALES = IsCheckedMentMadre,
                    AHF_SANO = CheckMadrePadece,
                    AHF_TB = IsCheckedTBMadre,
                    AHF_VIVE = CheckMadreVive,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    ID_TIPO_REFERENCIA = (short)eFamiliares.MADRE
                };

                lstFamiliares.Add(Madre);
                HISTORIA_CLINICA_FAMILIAR Hermanos = new HISTORIA_CLINICA_FAMILIAR()
                {
                    AHF_ALERGIAS = IsCheckedAlergiHnos,
                    AHF_CA = IsCheckedCAHnos,
                    AHF_CARDIACOS = IsCheckedCardiHnos,
                    AHF_DIABETES = IsCheckedDiabHnos,
                    AHF_EPILEPSIA = IsCheckedEpiHnos,
                    AHF_FALLECIMIENTO_CAUSA = CausaMuerteHnos,
                    AHF_FALLECIMIENTO_FEC = CuandoMuerteHnos,
                    AHF_HIPERTENSIVO = IsCheckedHipertHnos,
                    AHF_MENTALES = IsCheckedMentHnos,
                    AHF_SANO = CheckHermanosSanos,
                    AHF_TB = IsCheckedTBHnos,
                    AHF_VIVE = CheckHermanosVivos,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    ID_TIPO_REFERENCIA = (short)eFamiliares.HERMANOS
                };

                lstFamiliares.Add(Hermanos);

                HISTORIA_CLINICA_FAMILIAR Conyuge = new HISTORIA_CLINICA_FAMILIAR()
                {
                    AHF_ALERGIAS = IsCheckedAlergiCony,
                    AHF_CA = IsCheckedCACony,
                    AHF_CARDIACOS = IsCheckedCardiCony,
                    AHF_DIABETES = IsCheckedDiabCony,
                    AHF_EDAD = TextEdadConyuge,
                    AHF_EPILEPSIA = IsCheckedEpiCony,
                    AHF_FALLECIMIENTO_CAUSA = CausaMuerteCony,
                    AHF_FALLECIMIENTO_FEC = CuandoMuerteCony,
                    AHF_HIPERTENSIVO = IsCheckedHipertCony,
                    AHF_MENTALES = IsCheckedMentCony,
                    AHF_SANO = CheckConyugePadece,
                    AHF_TB = IsCheckedTBCony,
                    AHF_VIVE = CheckConyugeVive,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    ID_TIPO_REFERENCIA = (short)eFamiliares.CONYUGE
                };

                lstFamiliares.Add(Conyuge);

                HISTORIA_CLINICA_FAMILIAR Hijos = new HISTORIA_CLINICA_FAMILIAR()
                {
                    AHF_ALERGIAS = IsCheckedAlergiHijos,
                    AHF_CA = IsCheckedCAHijos,
                    AHF_CARDIACOS = IsCheckedCardiHijos,
                    AHF_DIABETES = IsCheckedDiabHijos,
                    AHF_EDAD = TextEdadesHijos,
                    AHF_EPILEPSIA = IsCheckedEpiHijos,
                    AHF_FALLECIMIENTO_CAUSA = CausaMuerteHijos,
                    AHF_FALLECIMIENTO_FEC = CuandoMuerteHijos,
                    AHF_HIPERTENSIVO = IsCheckedHipertHijos,
                    AHF_MENTALES = IsCheckedMentHijos,
                    AHF_SANO = CheckHijosPadece,
                    AHF_TB = IsCheckedTBHijos,
                    AHF_VIVE = CheckHijosVive,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    ID_TIPO_REFERENCIA = (short)eFamiliares.HIJOS
                };

                lstFamiliares.Add(Hijos);
                #endregion

                historia.HISTORIA_CLINICA_FAMILIAR.Clear();
                historia.HISTORIA_CLINICA_FAMILIAR = lstFamiliares;

                LstGruposVulnerables = new ObservableCollection<GRUPO_VULNERABLE>();
                DateTime _fecha = Fechas.GetFechaDateServer;
                if (LstSectoresVulnerbles != null && LstSectoresVulnerbles.Any())
                    foreach (var item in LstSectoresVulnerbles)
                    {
                        if (item.ES_GRUPO_VULNERABLE == 1)
                        {
                            LstGruposVulnerables.Add(new GRUPO_VULNERABLE
                                {
                                    BAJA_FEC = null,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    REGISTRO_FEC = _fecha,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    MOMENTO_DETECCION = "EI",
                                    ID_SECTOR_CLAS = item.ID_SECTOR_CLAS
                                });
                        };
                    };

                historia.GRUPO_VULNERABLE.Clear();
                historia.GRUPO_VULNERABLE = LstGruposVulnerables;

                historia.POR_CONSTANCIAS_DOCUMENTALES = LstDocumentosActuales != null ? LstDocumentosActuales.Any() ? (short)eSINO.SI : (short)eSINO.NO : (short)eSINO.NO;
                bool Resultado = false;
                if (new cHistoriaClinica().InsertarTransaccion(historia, Fechas.GetFechaDateServer) == null)
                    Resultado = false;
                else
                {
                    StaticSourcesViewModel.SourceChanged = false;
                    IsDocumentoFisicoEnabled = DigitalizacionDirecta = HabilitaImputados = MenuGuardarEnabled = IsReadOnlyDatos = false;
                    Resultado = true;
                }

                return Resultado;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la historia clínica médica.", ex);
            }

            return false;
        }

        private void GetDatosImputadoSeleccionado()
        {
            try
            {

                if (SelectIngreso == null)
                    return;

                if (SelectExpediente != null)
                {
                    PaternoD = !string.IsNullOrEmpty(SelectExpediente.PATERNO) ? SelectExpediente.PATERNO.Trim() : string.Empty;
                    MaternoD = !string.IsNullOrEmpty(SelectExpediente.MATERNO) ? SelectExpediente.MATERNO.Trim() : string.Empty;
                    NombreD = !string.IsNullOrEmpty(SelectExpediente.NOMBRE) ? SelectExpediente.NOMBRE.Trim() : string.Empty;
                    AnioD = SelectExpediente.ID_ANIO;
                    FolioD = SelectExpediente.ID_IMPUTADO;
                    TextPaternoImputado = !string.IsNullOrEmpty(SelectExpediente.PATERNO) ? SelectExpediente.PATERNO.Trim() : string.Empty;
                    TextMaternoImputado = !string.IsNullOrEmpty(SelectExpediente.MATERNO) ? SelectExpediente.MATERNO.Trim() : string.Empty;
                    TextNombreImputado = !string.IsNullOrEmpty(SelectExpediente.NOMBRE) ? SelectExpediente.NOMBRE.Trim() : string.Empty;
                    TextAnioImputado = SelectExpediente.ID_ANIO;
                    TextFolioImputado = SelectExpediente.ID_IMPUTADO;
                    AnioBuscar = SelectExpediente.ID_ANIO;
                    FolioBuscar = SelectExpediente.ID_IMPUTADO;
                };

                IngresosD = SelectIngreso.ID_INGRESO;
                if (SelectIngreso.CAMA != null)
                    UbicacionD = string.Format("{0}-{1}{2}-{3}",
                        SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                        SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty, SelectIngreso.ID_UB_CAMA);
                else
                    UbicacionD = string.Empty;

                TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.TIPO_SEGURIDAD.DESCR) ? SelectIngreso.TIPO_SEGURIDAD.DESCR.Trim() : string.Empty : string.Empty;
                FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO;
                ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(SelectIngreso.CLASIFICACION_JURIDICA.DESCR) ? SelectIngreso.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty;
                EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO != null ? !string.IsNullOrEmpty(SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR) ? SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR.Trim() : string.Empty : string.Empty;
                TextEdad = SelectIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA) : new short?();
                SelectSexo = !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.SEXO) ? SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO" : string.Empty;
                MujeresEnabled = SelectIngreso.IMPUTADO.SEXO == "F";
                var municipio = new cMunicipio().Obtener(SelectIngreso.IMPUTADO.NACIMIENTO_ESTADO.Value, SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                TextLugarNacimiento = string.Format("{0} {1} {2}", SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NACIMIENTO_LUGAR) ? SelectIngreso.IMPUTADO.NACIMIENTO_LUGAR.Trim() : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? municipio != null ? !string.IsNullOrEmpty(municipio.MUNICIPIO1) ? municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? municipio != null ? municipio.ENTIDAD != null ? !string.IsNullOrEmpty(municipio.ENTIDAD.DESCR) ? municipio.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? municipio != null ? municipio.ENTIDAD != null ? municipio.ENTIDAD.PAIS_NACIONALIDAD != null ? !string.IsNullOrEmpty(municipio.ENTIDAD.PAIS_NACIONALIDAD.PAIS) ? municipio.ENTIDAD.PAIS_NACIONALIDAD.PAIS.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty);

                var _cent = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();

                CentroReclusion = _cent != null ? !string.IsNullOrEmpty(_cent.DESCR) ? _cent.DESCR.Trim() : string.Empty : string.Empty;
                NombreImputado = string.Format("{0} {1} {2}", SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                                                              SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                                                              SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);

                SelectFechaNacimiento = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA : new DateTime?();
                TextEscolaridad = SelectIngreso.ESCOLARIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? SelectIngreso.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                TextOcupacion = SelectIngreso != null ? SelectIngreso.OCUPACION != null ? !string.IsNullOrEmpty(SelectIngreso.OCUPACION.DESCR) ? SelectIngreso.OCUPACION.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                TextEstadoCivil = SelectIngreso != null ? SelectIngreso.ESTADO_CIVIL != null ? !string.IsNullOrEmpty(SelectIngreso.ESTADO_CIVIL.DESCR) ? SelectIngreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                var _delitos = new cCausaPenalDelito().Obtener(Convert.ToInt32(SelectIngreso.ID_CENTRO), Convert.ToInt32(SelectIngreso.ID_ANIO), Convert.ToInt32(SelectIngreso.ID_IMPUTADO), Convert.ToInt32(SelectIngreso.ID_INGRESO));

                if (_delitos.Any())
                    foreach (var item in _delitos)
                    {
                        if (_delitos.Count > 1)
                            TextDelito += string.Format("{0} y ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                        else
                            TextDelito += string.Format("{0} ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                    };

                TextSentencia = CalcularSentencia().ToUpper();
                TextAPartir = SelectIngreso.FEC_INGRESO_CERESO;
                FotoIngreso = SelectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                        SelectIngreso.INGRESO_BIOMETRICO.First(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO :
                            SelectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG) ?
                                SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                        ImagenImputado = new Imagenes().getImagenPerson();

                GenitalMujer = SelectIngreso.IMPUTADO.SEXO == "F" ? Visibility.Visible : Visibility.Collapsed;
                GenitalHombre = SelectIngreso.IMPUTADO.SEXO == "M" ? Visibility.Visible : Visibility.Collapsed;
                ConfiguraPermisos();
                if (EsDentista == false)
                {
                    ArbolViewM();
                    GetDatosHistoriaClinicaExistente();
                    SetValidacionesTodas();
                }
                else
                {
                    ArbolViewDental();
                    InicializaDatosHistoriaClinicaDental();
                    ValidacionesHistoriaClinicaDental();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado.", ex);
            }
        }


        private void PrepararListas()
        {
            try
            {
                if (LstControlPrenatal == null)
                {
                    LstControlPrenatal = new ObservableCollection<CONTROL_PRENATAL>(new cControlPrenatal().GetData());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstControlPrenatal.Insert(0, (new CONTROL_PRENATAL() { ID_CONTROL_PRENATAL = -1, DESCR = "SELECCIONE" }));
                    }));
                };

                ListTipoDocumento = new ObservableCollection<HC_DOCUMENTO_TIPO>();
                ListTipoDocumento = new ObservableCollection<HC_DOCUMENTO_TIPO>(new cTipoDocumentosHistoriaClinica().GetData(x => x.ESTATUS == "S"));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListTipoDocumento.Insert(0, (new HC_DOCUMENTO_TIPO() { ID_DOCTO = -1, DESCR = "SELECCIONE" }));
                }));

                TipoDocto = -1;
                OnPropertyChanged("TipoDocto");
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void Imprime()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    var detalleHistoria = new cHistoriaClinica().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (detalleHistoria != null)
                    {
                        var DatosReporte = new cHistoriaClinicaReporte();

                        var View = new ReportesView();
                        #region Iniciliza el entorno para mostrar el reporte al usuario
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        View.Owner = PopUpsViewModels.MainWindow;
                        View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                        View.Show();

                        #endregion

                        #region Definicion de datos base hacia el reporte
                        DatosReporte.FechaEstudio = detalleHistoria.ESTUDIO_FEC.HasValue ? detalleHistoria.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                        var _centro = SelectIngreso.ID_UB_CENTRO.HasValue ? new cCentro().GetData(x => x.ID_CENTRO == SelectIngreso.ID_UB_CENTRO).FirstOrDefault() : null;
                        if (_centro != null)
                            DatosReporte.CeresoReclusion = !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty;

                        DatosReporte.Nombre = string.Format("{0} {1} {2}", SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                                                                          SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                                                                          SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);

                        DatosReporte.Edad = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString();
                        //DatosReporte.EdoCivil = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.ESTADO_CIVIL.DESCR) ? SelectIngreso.IMPUTADO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        DatosReporte.EdoCivil = SelectIngreso != null ? SelectIngreso.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectIngreso.ESTADO_CIVIL.DESCR) ? SelectIngreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        var municipio = new cMunicipio().Obtener(SelectIngreso.IMPUTADO.NACIMIENTO_ESTADO.Value, SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                        DatosReporte.LugarFecNac = string.Format("{0} {1} {2} {3} {4}",
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NACIMIENTO_LUGAR) ? SelectIngreso.IMPUTADO.NACIMIENTO_LUGAR.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? municipio != null ? !string.IsNullOrEmpty(municipio.MUNICIPIO1) ? municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? municipio != null ? municipio.ENTIDAD != null ? !string.IsNullOrEmpty(municipio.ENTIDAD.DESCR) ? municipio.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? municipio != null ? municipio.ENTIDAD != null ? municipio.ENTIDAD.PAIS_NACIONALIDAD != null ? !string.IsNullOrEmpty(municipio.ENTIDAD.PAIS_NACIONALIDAD.PAIS) ? municipio.ENTIDAD.PAIS_NACIONALIDAD.PAIS.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty);

                        DatosReporte.APartirD = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToString("dd/MM/yyyy") : string.Empty;
                        DatosReporte.Sexo = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO" : string.Empty;
                        //DatosReporte.Escolaridad = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.ID_ESCOLARIDAD.HasValue ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.ESCOLARIDAD.DESCR) ? SelectIngreso.IMPUTADO.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        DatosReporte.Escolaridad = SelectIngreso != null ? SelectIngreso.ID_ESCOLARIDAD.HasValue ? !string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? SelectIngreso.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        //DatosReporte.Ocupacion = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.OCUPACION.DESCR) ? SelectIngreso.IMPUTADO.OCUPACION.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        DatosReporte.Ocupacion = SelectIngreso != null ? SelectIngreso.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(SelectIngreso.OCUPACION.DESCR) ? SelectIngreso.OCUPACION.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                        var _delitos = new cCausaPenalDelito().Obtener(Convert.ToInt32(SelectIngreso.ID_CENTRO), Convert.ToInt32(SelectIngreso.ID_ANIO), Convert.ToInt32(SelectIngreso.ID_IMPUTADO), Convert.ToInt32(SelectIngreso.ID_INGRESO));
                        if (_delitos.Any())
                            foreach (var item in _delitos)
                            {
                                if (_delitos.Count > 1)
                                    DatosReporte.Delito += string.Format("{0} y ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                                else
                                    DatosReporte.Delito += string.Format("{0} ", item.MODALIDAD_DELITO != null ? !string.IsNullOrEmpty(item.MODALIDAD_DELITO.DESCR) ? item.MODALIDAD_DELITO.DESCR.Trim() : string.Empty : string.Empty);
                            };

                        DatosReporte.MedicamentosActuales = detalleHistoria.APP_MEDICAMENTOS_ACTIVOS;
                        DatosReporte.Sentencia = CalcularSentencia().ToUpper();

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
                        View.Report.LocalReport.ReportPath = "Reportes/rHistoriaClinica.rdlc";
                        View.Report.LocalReport.DataSources.Clear();
                        #endregion


                        #region Encabezado
                        var Encabezado = new cEncabezado();
                        Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                        Encabezado.TituloDos = Parametro.ENCABEZADO2;
                        Encabezado.NombreReporte = "HISTORIA CLÍNICA";
                        Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                        Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;
                        Encabezado.PieUno = string.Concat("NOMBRE DEL INTERNO: ", string.Format("{0}/{1} ", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO), string.Format("{0} {1} {2}",
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                                    !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty), ", UBICACIÓN ACTUAL: ", string.Format("{0}-{1}{2}-{3}",
                                                    SelectIngreso != null ? SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectIngreso != null ? SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectIngreso != null ? SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    SelectIngreso != null ? SelectIngreso.ID_UB_CAMA.HasValue ? SelectIngreso.ID_UB_CAMA.Value.ToString() : string.Empty : string.Empty));
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
                        View.Report.LocalReport.DataSources.Add(rds5);


                        Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds4.Name = "DataSet4";
                        rds4.Value = lstPatologicos;
                        View.Report.LocalReport.DataSources.Add(rds4);


                        Microsoft.Reporting.WinForms.ReportDataSource rds7 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds7.Name = "DataSet7";
                        rds7.Value = patos1;
                        View.Report.LocalReport.DataSources.Add(rds7);

                        Microsoft.Reporting.WinForms.ReportDataSource rds8 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds8.Name = "DataSet8";
                        rds8.Value = patos2;
                        View.Report.LocalReport.DataSources.Add(rds8);

                        Microsoft.Reporting.WinForms.ReportDataSource rds9 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds9.Name = "DataSet9";
                        rds9.Value = patos3;
                        View.Report.LocalReport.DataSources.Add(rds9);

                        Microsoft.Reporting.WinForms.ReportDataSource rds10 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds10.Name = "DataSet10";
                        rds10.Value = patos4;
                        View.Report.LocalReport.DataSources.Add(rds10);

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
                        View.Report.LocalReport.DataSources.Add(rds3);

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
                        View.Report.LocalReport.DataSources.Add(rds6);
                        #endregion
                        #region Definicion de origenes de dato


                        var ds1 = new List<cEncabezado>();
                        ds1.Add(Encabezado);
                        Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds1.Name = "DataSet2";
                        rds1.Value = ds1;
                        View.Report.LocalReport.DataSources.Add(rds1);


                        var ds2 = new List<cHistoriaClinicaReporte>();
                        ds2.Add(DatosReporte);
                        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds2.Name = "DataSet1";
                        rds2.Value = ds2;
                        View.Report.LocalReport.DataSources.Add(rds2);
                        #endregion
                        View.Report.RefreshReport();
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El necesario capturar la historia clínica antes de imprimir el reporte");
                        return;
                    }
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Notificación!", "Es necesario seleccionar un ingreso valido.");
                    return;
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// Metodo que recibe un texto en html, lo lee y toma una captura de pantalla
        /// </summary>
        /// <param name="body"> Inicializa un entorno con el html que se le envia, lee el codigo html, toma una imagen y la regresa</param>
        /// <returns> Imgen de la captura de pantalla, esto para dar formato de tabla a un texto que se forma de manera dinamica</returns>
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

        public static System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(System.Drawing.Imaging.ImageFormat format)
        {
            return System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders().ToList().Find(delegate(System.Drawing.Imaging.ImageCodecInfo codec)
            {
                return codec.FormatID == format.Guid;
            });
        }


        private void GetDatosHistoriaClinicaExistente()
        {
            try
            {
                PrepararListas();
                var historia = new cHistoriaClinica().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                var _GrposActivos = new cSectorClasificacion().GetData(x => x.ESTATUS == "S" && x.ES_GRUPO_VULNERABLE == (short)eVulnerablesActivos.SI);

                LstSectoresVulnerbles = new ObservableCollection<SECTOR_CLASIFICACION>();
                DateTime Fecha = Fechas.GetFechaDateServer;
                #region Ya tiene historia clinica
                if (historia != null)
                {
                    var _validaHistoriaCliinica = historia != null ? historia.ESTATUS == "I" ? new cHistoriaClinica().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.INGRESO.ID_UB_CENTRO == SelectIngreso.ID_CENTRO && x.ID_INGRESO != SelectIngreso.ID_INGRESO).OrderByDescending(x => x.ESTUDIO_FEC).FirstOrDefault() : null : null;

                    if (_validaHistoriaCliinica != null)
                    {
                        #region AHF
                        var _familiares = _validaHistoriaCliinica.HISTORIA_CLINICA_FAMILIAR;
                        if (_familiares != null && _familiares.Any())
                        {
                            foreach (var item in _familiares)
                            {
                                if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.PADRE)
                                {
                                    CheckPadreVive = item.AHF_VIVE ?? string.Empty;
                                    TextEdadPadre = item.AHF_EDAD ?? new short?();
                                    CheckPadrePadece = item.AHF_SANO ?? string.Empty;
                                    IsCheckedDiabPadre = item.AHF_DIABETES ?? string.Empty;
                                    IsCheckedTBPadre = item.AHF_TB ?? string.Empty;
                                    IsCheckedCAPadre = item.AHF_CA ?? string.Empty;
                                    IsCheckedCardiPadre = item.AHF_CARDIACOS ?? string.Empty;
                                    IsCheckedEpiPadre = item.AHF_EPILEPSIA ?? string.Empty;
                                    IsCheckedMentPadre = item.AHF_MENTALES ?? string.Empty;
                                    IsCheckedAlergiPadre = item.AHF_ALERGIAS ?? string.Empty;
                                    IsCheckedHipertPadre = item.AHF_HIPERTENSIVO ?? string.Empty;
                                    CuandoMuertePadre = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                    CausaMuertePadre = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                };

                                if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.MADRE)
                                {
                                    CheckMadreVive = item.AHF_VIVE ?? string.Empty;
                                    TextEdadMadre = item.AHF_EDAD ?? new short?();
                                    CheckMadrePadece = item.AHF_SANO ?? string.Empty;
                                    IsCheckedDiabMadre = item.AHF_DIABETES ?? string.Empty;
                                    IsCheckedTBMadre = item.AHF_TB ?? string.Empty;
                                    IsCheckedCAMadre = item.AHF_CA ?? string.Empty;
                                    IsCheckedCardiMadre = item.AHF_CARDIACOS ?? string.Empty;
                                    IsCheckedEpiMadre = item.AHF_EPILEPSIA ?? string.Empty;
                                    IsCheckedMentMadre = item.AHF_MENTALES ?? string.Empty;
                                    IsCheckedAlergiMadre = item.AHF_ALERGIAS ?? string.Empty;
                                    IsCheckedHipertMadre = item.AHF_HIPERTENSIVO ?? string.Empty;
                                    CuandoMuerteMadre = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                    CausaMuerteMadre = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                };

                                if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.HERMANOS)
                                {
                                    CheckHermanosVivos = item.AHF_VIVE ?? string.Empty;
                                    CheckHermanosSanos = item.AHF_SANO ?? string.Empty;
                                    IsCheckedDiabHnos = item.AHF_DIABETES ?? string.Empty;
                                    IsCheckedTBHnos = item.AHF_TB ?? string.Empty;
                                    IsCheckedCAHnos = item.AHF_CA ?? string.Empty;
                                    IsCheckedCardiHnos = item.AHF_CARDIACOS ?? string.Empty;
                                    IsCheckedEpiHnos = item.AHF_EPILEPSIA ?? string.Empty;
                                    IsCheckedMentHnos = item.AHF_MENTALES ?? string.Empty;
                                    IsCheckedAlergiHnos = item.AHF_ALERGIAS ?? string.Empty;
                                    IsCheckedHipertHnos = item.AHF_HIPERTENSIVO ?? string.Empty;
                                    CuandoMuerteHnos = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                    CausaMuerteHnos = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                    TextHermanosMujeres = _validaHistoriaCliinica.AHF_HERMANOS_F ?? new short?();
                                    TextHermanosHombres = _validaHistoriaCliinica.AHF_HERMANOS_M ?? new short?();
                                };

                                if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.CONYUGE)
                                {
                                    CheckConyugeVive = item.AHF_VIVE ?? string.Empty;
                                    TextEdadConyuge = item.AHF_EDAD ?? new short?();
                                    CheckConyugePadece = item.AHF_SANO ?? string.Empty;
                                    IsCheckedDiabCony = item.AHF_DIABETES ?? string.Empty;
                                    IsCheckedTBCony = item.AHF_TB ?? string.Empty;
                                    IsCheckedCACony = item.AHF_CA ?? string.Empty;
                                    IsCheckedCardiCony = item.AHF_CARDIACOS ?? string.Empty;
                                    IsCheckedEpiCony = item.AHF_EPILEPSIA ?? string.Empty;
                                    IsCheckedMentCony = item.AHF_MENTALES ?? string.Empty;
                                    IsCheckedAlergiCony = item.AHF_ALERGIAS ?? string.Empty;
                                    IsCheckedHipertCony = item.AHF_HIPERTENSIVO ?? string.Empty;
                                    CuandoMuerteCony = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                    CausaMuerteCony = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                };

                                if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.HIJOS)
                                {
                                    CheckHijosVive = item.AHF_VIVE ?? string.Empty;
                                    TextEdadesHijos = item.AHF_EDAD ?? new short?();
                                    CheckHijosPadece = item.AHF_SANO ?? string.Empty;
                                    IsCheckedDiabHijos = item.AHF_DIABETES ?? string.Empty;
                                    IsCheckedTBHijos = item.AHF_TB ?? string.Empty;
                                    IsCheckedCAHijos = item.AHF_CA ?? string.Empty;
                                    IsCheckedCardiHijos = item.AHF_CARDIACOS ?? string.Empty;
                                    IsCheckedEpiHijos = item.AHF_EPILEPSIA ?? string.Empty;
                                    IsCheckedMentHijos = item.AHF_MENTALES ?? string.Empty;
                                    IsCheckedAlergiHijos = item.AHF_ALERGIAS ?? string.Empty;
                                    IsCheckedHipertHijos = item.AHF_HIPERTENSIVO ?? string.Empty;
                                    CuandoMuerteHijos = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                    CausaMuerteHijos = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                };
                            };
                        }
                        else
                        {
                            TextEdadConyuge = TextHermanosMujeres = TextHermanosHombres = TextEdadesHijos = TextEdadPadre = TextEdadMadre = new short?();
                            CheckPadreVive = CheckPadrePadece = IsCheckedDiabPadre = IsCheckedTBPadre = IsCheckedCAPadre = IsCheckedCardiPadre = IsCheckedEpiPadre = IsCheckedMentPadre = IsCheckedAlergiPadre = IsCheckedHipertPadre = CausaMuertePadre = string.Empty;
                            CuandoMuerteHijos = CuandoMuerteCony = CuandoMuerteHnos = CuandoMuertePadre = CuandoMuerteMadre = new DateTime?();
                            CheckMadreVive = CheckMadrePadece = IsCheckedDiabMadre = IsCheckedTBMadre = IsCheckedCAMadre = IsCheckedCardiMadre = IsCheckedEpiMadre = IsCheckedMentMadre = IsCheckedAlergiMadre = IsCheckedHipertMadre = CausaMuerteMadre = string.Empty;
                            CausaMuerteHnos = CheckHermanosVivos = CheckHermanosSanos = IsCheckedDiabHnos = IsCheckedTBHnos = IsCheckedCAHnos = IsCheckedCardiHnos = IsCheckedEpiHnos = IsCheckedMentHnos = IsCheckedAlergiHnos = IsCheckedHipertHnos = string.Empty;
                            CheckConyugeVive = CheckConyugePadece = IsCheckedDiabCony = IsCheckedTBCony = IsCheckedCACony = IsCheckedCardiCony = IsCheckedEpiCony = IsCheckedMentCony = IsCheckedAlergiCony = IsCheckedHipertCony = CausaMuerteCony = string.Empty;
                            CheckHijosPadece = IsCheckedDiabHijos = IsCheckedTBHijos = IsCheckedCAHijos = IsCheckedCardiHijos = IsCheckedEpiHijos = IsCheckedMentHijos = IsCheckedAlergiHijos = IsCheckedHipertHijos = CausaMuerteHijos = CheckHijosVive = string.Empty;
                        }

                        #endregion

                        #region APNP
                        TextNacimientoNoPatologicos = _validaHistoriaCliinica.APNP_NACIMIENTO;
                        TextAlimentacionNoPatologicos = _validaHistoriaCliinica.APNP_ALIMENTACION;
                        TextHabitacionNoPatologicos = _validaHistoriaCliinica.APNP_HABITACION;
                        TextAlcoholismoNoPatologicos = _validaHistoriaCliinica.APNP_ALCOHOLISMO ?? string.Empty;
                        TextTabaquismoNoPatologicos = _validaHistoriaCliinica.APNP_TABAQUISMO ?? string.Empty;
                        TextToxicomaniasNoPatologicos = _validaHistoriaCliinica.APNP_TOXICOMANIAS ?? string.Empty;
                        ObservacionesAlcohlismo = _validaHistoriaCliinica.APNP_ALCOHOLISMO_OBSERV;
                        ObservacionesTabaquismo = _validaHistoriaCliinica.APNP_TABAQUISMO_OBSERV;

                        #endregion

                        #region MUJERES
                        CheckMenarquia = _validaHistoriaCliinica.MU_MENARQUIA;
                        var detallesGinecoObstetricos = _validaHistoriaCliinica.HISTORIA_CLINICA_GINECO_OBSTRE != null ? _validaHistoriaCliinica.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? _validaHistoriaCliinica.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault(x => x.MOMENTO_REGISTRO == "EI") : null : null;
                        IdControlPreN = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.CONTROL_PRENATAL : string.Empty;
                        TextAniosRitmo = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.ANIOS_RITMO : string.Empty;
                        TextEmbarazos = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.EMBARAZO : new short?();
                        TextPartos = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.PARTO : new short?();
                        TextAbortos = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.ABORTO : new short?();
                        TextCesareas = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.CESAREA : new short?();
                        FechaUltimaMenstruacion = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.ULTIMA_MENSTRUACION_FEC : new DateTime?();
                        TextDeformacionesOrganicas = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.DEFORMACION : string.Empty;
                        IdSelectedControlP = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.ID_CONTROL_PRENATAL : -1;
                        FechaProbParto = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.FECHA_PROBABLE_PARTO : new DateTime?();

                        #endregion

                        #region PADECIMIENTO_ACTUAL
                        TextPadecimientoActual = _validaHistoriaCliinica.PADECIMIENTO_ACTUAL;
                        #endregion

                        #region APARATOS_Y_SISTEMAS
                        TextRespiratorio = _validaHistoriaCliinica.RESPIRATORIO;
                        TextCardiovascular = _validaHistoriaCliinica.CARDIOVASCULAR;
                        TextDigestivo = _validaHistoriaCliinica.DIGESTIVO;
                        TextUrinario = _validaHistoriaCliinica.URINARIO;
                        TextGenitalMujeres = _validaHistoriaCliinica.GENITAL_MUJERES;
                        TextGenitalHombres = _validaHistoriaCliinica.GENITAL_HOMBRES;
                        TextEndocrino = _validaHistoriaCliinica.ENDOCRINO;
                        TextMusculoEsqueletico = _validaHistoriaCliinica.MUSCULO_ESQUELETICO;
                        TextHematicoLinfatico = _validaHistoriaCliinica.HEMATICO_LINFACTICO;
                        TextNervioso = _validaHistoriaCliinica.NERVIOSO;
                        TextPielAnexos = _validaHistoriaCliinica.PIEL_ANEXOS;
                        TextTerapeuticaPrevia = _validaHistoriaCliinica.TERAPEUTICA_PREVIA;
                        TextSintomasGenerales = _validaHistoriaCliinica.SINTOMAS_GENERALES;
                        #endregion

                        #region EXPLORACION_FISICA

                        TextExploracionfisica = _validaHistoriaCliinica.EF_DESCRIPCION;
                        var _signosVitales = new cHistoriaClinica().GetData(x => x.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault();
                        TextPeso = _signosVitales != null ? _signosVitales.EF_PESO : _validaHistoriaCliinica.EF_PESO;
                        TextEstatura = _signosVitales != null ? _signosVitales.EF_ESTATURA : _validaHistoriaCliinica.EF_ESTATURA;
                        TextCabeza = _validaHistoriaCliinica.EF_CABEZA;
                        TextCuello = _validaHistoriaCliinica.EF_CUELLO;
                        TextTorax = _validaHistoriaCliinica.EF_TORAX;
                        TextAbdomen = _validaHistoriaCliinica.EF_ABDOMEN;
                        TextRecto = _validaHistoriaCliinica.EF_RECTO;
                        TextGenitales = _validaHistoriaCliinica.EF_GENITALES;
                        TextExtremidades = _validaHistoriaCliinica.EF_EXTREMIDADES;
                        if (!string.IsNullOrEmpty(_validaHistoriaCliinica.EF_PRESION_ARTERIAL) ? _validaHistoriaCliinica.EF_PRESION_ARTERIAL.Contains("/") : false)
                        {
                            Arterial1 = _signosVitales != null ? _signosVitales.EF_PRESION_ARTERIAL.Split('/')[0] : _validaHistoriaCliinica != null ? _validaHistoriaCliinica.EF_PRESION_ARTERIAL.Split('/')[0] : string.Empty;
                            Arterial2 = _signosVitales != null ? _signosVitales.EF_PRESION_ARTERIAL.Split('/')[1] : _validaHistoriaCliinica != null ? _validaHistoriaCliinica.EF_PRESION_ARTERIAL.Split('/')[1] : string.Empty;
                        }

                        TextPulso = _signosVitales != null ? _signosVitales.EF_PULSO : _validaHistoriaCliinica.EF_PULSO;
                        TextGenital = SelectIngreso.IMPUTADO.SEXO == "M" ? _validaHistoriaCliinica.GENITAL_HOMBRES : _validaHistoriaCliinica.GENITAL_MUJERES;
                        TextRespiracion = _signosVitales != null ? _signosVitales.EF_RESPIRACION : _validaHistoriaCliinica.EF_RESPIRACION;
                        TextTemperatura = _signosVitales != null ? _signosVitales.EF_TEMPERATURA : _validaHistoriaCliinica.EF_TEMPERATURA;
                        TextResultadosAnalisisClinicos = _validaHistoriaCliinica.EF_RESULTADO_ANALISIS;
                        TextResultadosestudiosGabinete = _validaHistoriaCliinica.EF_RESULTADO_GABINETE;
                        TextImpresionDiagnostica = _validaHistoriaCliinica.EF_IMPRESION_DIAGNOSTICA;

                        #endregion

                        #region MAYORES 65
                        IdDisminucionVisua = _validaHistoriaCliinica.M65_ALTERACION_VISUAL ?? -1;
                        CheckAgudezaAuditiva = _validaHistoriaCliinica.M65_ALTERACION_AUDITIVA ?? string.Empty;
                        CheckOlfacion = _validaHistoriaCliinica.M65_ALTERACION_OLFACION ?? string.Empty;
                        CheckCapacidadVisomotriz = _validaHistoriaCliinica.M65_ALTERACION_VISOMOTRIZ ?? string.Empty;
                        IdTranstornosMemoria = _validaHistoriaCliinica.M65_TRAS_MEMORIA ?? -1;
                        IdTranstornosAtencion = _validaHistoriaCliinica.M65_TRAS_ATENCION ?? -1;
                        IdTranstornosComprension = _validaHistoriaCliinica.M65_TRAS_COMPRENSION ?? -1;
                        IdTranstornosOrientacion = _validaHistoriaCliinica.M65_TRAS_ORIENTACION ?? -1;
                        CheckDemencial = _validaHistoriaCliinica.M65_TRAS_DEMENCIAL ?? string.Empty;
                        IdEstadoAnimo = _validaHistoriaCliinica.M65_TRAS_ANIMO ?? -1;
                        IdCapacidadParticipacion = _validaHistoriaCliinica.M65_PARTICIPACION ?? -1;
                        TextOtroDisminucionVisual = _validaHistoriaCliinica.M65_OTROS ?? string.Empty;
                        #endregion

                        #region CONSIDERACIONES_FINALES
                        IdComplica = _validaHistoriaCliinica.CP_GRAVEDAD ?? -1;
                        IdEtapaEvo = _validaHistoriaCliinica.CP_ETAPA_EVOLUTIVA ?? -1;
                        IdPosibRemis = _validaHistoriaCliinica.CP_REMISION ?? -1;
                        IdCapacTrata = _validaHistoriaCliinica.CP_CAPACIDAD_TRATAMIENTO ?? -1;
                        IdNivelReq = _validaHistoriaCliinica.CP_NIVEL_ATENCION ?? -1;
                        TextGradoAfectacion = _validaHistoriaCliinica.CP_GRADO_AFECTACION;
                        TextPronostico = _validaHistoriaCliinica.CP_PRONOSTICO;
                        #endregion


                        MedicamentosActivos = _validaHistoriaCliinica.APP_MEDICAMENTOS_ACTIVOS;

                        #region CONCLUSIONES
                        TextConclusiones = _validaHistoriaCliinica.CONCLUSIONES;
                        #endregion


                        if (_IdImputado == null || _IdAnio == null || _IdIngreso == null || _IdCentro == null)//NO TIENE UNA HISTORIA CLINICA Y NO VIENE DEL CONSTRUCTOR
                            IsDocumentoFisicoEnabled = DigitalizacionDirecta = HabilitaImputados = MenuGuardarEnabled = IsReadOnlyDatos = IsEnabledSignosVitales = false;

                        if (string.IsNullOrEmpty(_validaHistoriaCliinica.EF_PESO) || string.IsNullOrEmpty(_validaHistoriaCliinica.EF_PRESION_ARTERIAL) || string.IsNullOrEmpty(_validaHistoriaCliinica.EF_PULSO) || string.IsNullOrEmpty(_validaHistoriaCliinica.EF_RESPIRACION) || string.IsNullOrEmpty(_validaHistoriaCliinica.EF_TEMPERATURA))
                            IsEnabledSignosVitales = true;//al menos un signo vital esta vacio, permitele capturar de nuevo
                        else
                            IsEnabledSignosVitales = false;

                        #region Patologicos
                        System.DateTime _fechaAct = Fechas.GetFechaDateServer;
                        LstCondensadoPatologicos = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>(_validaHistoriaCliinica.HISTORIA_CLINICA_PATOLOGICOS.Select(x => new HISTORIA_CLINICA_PATOLOGICOS
                        {
                            FUENTE = "X",
                            ID_ANIO = x.ID_ANIO,
                            ID_CENTRO = x.ID_CENTRO,
                            ID_CONSEC = x.ID_CONSEC,
                            ID_IMPUTADO = x.ID_IMPUTADO,
                            ID_INGRESO = x.ID_INGRESO,
                            ID_NOPATOLOGICO = x.ID_NOPATOLOGICO,
                            MOMENTO_DETECCION = "EI",
                            ID_PATOLOGICO = x.ID_PATOLOGICO,
                            OBSERVACIONES = x.OBSERVACIONES,
                            OTROS_DESCRIPCION = x.OTROS_DESCRIPCION,
                            RECUPERADO = x.RECUPERADO,
                            REGISTRO_FEC = _fechaAct,
                            PATOLOGICO_CAT = x.PATOLOGICO_CAT
                        }));

                        LstPatologicos = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>();

                        var _Enfermedades = new cPatologicoCat().GetData(y => y.ESTATUS == "S", z => z.DESCR);
                        if (_Enfermedades.Any() && _Enfermedades != null)
                            foreach (var item in _Enfermedades)
                                if (LstCondensadoPatologicos.Any() && LstCondensadoPatologicos != null)
                                    if (LstCondensadoPatologicos.FirstOrDefault(x => x.ID_PATOLOGICO == item.ID_PATOLOGICO) != null)
                                        continue;
                                    else
                                        Application.Current.Dispatcher.Invoke((Action)delegate
                                        {
                                            LstPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS() { OBSERVACIONES = string.Empty, MOMENTO_DETECCION = "EI", PATOLOGICO_CAT = item, REGISTRO_FEC = Fecha, ID_PATOLOGICO = item.ID_PATOLOGICO, ID_ANIO = SelectIngreso.ID_ANIO, ID_CENTRO = SelectIngreso.ID_CENTRO, ID_IMPUTADO = SelectIngreso.ID_IMPUTADO, ID_INGRESO = SelectIngreso.ID_INGRESO, OTROS_DESCRIPCION = string.Empty, RECUPERADO = "N" });
                                        });

                                else
                                    Application.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        LstPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS() { OBSERVACIONES = string.Empty, MOMENTO_DETECCION = "EI", PATOLOGICO_CAT = item, REGISTRO_FEC = Fecha, ID_PATOLOGICO = item.ID_PATOLOGICO, ID_ANIO = SelectIngreso.ID_ANIO, ID_CENTRO = SelectIngreso.ID_CENTRO, ID_IMPUTADO = SelectIngreso.ID_IMPUTADO, ID_INGRESO = SelectIngreso.ID_INGRESO, OTROS_DESCRIPCION = string.Empty, RECUPERADO = "N" });
                                    });
                        #endregion

                        #region Vulnerables
                        if (_GrposActivos != null && _GrposActivos.Any())
                            foreach (var item in _GrposActivos)
                            {
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    LstSectoresVulnerbles.Add(new SECTOR_CLASIFICACION() { COLOR = item.COLOR, COLOR_TEXTO = item.COLOR_TEXTO, OBSERV = item.OBSERV, ID_SECTOR_CLAS = item.ID_SECTOR_CLAS, ESTATUS = item.ESTATUS, ES_GRUPO_VULNERABLE = 0, POBLACION = item.POBLACION, SECTOR_OBSERVACION = item.SECTOR_OBSERVACION });
                                });
                            }
                        #endregion
                        return;
                    }

                    #region AHF
                    var familiares = historia.HISTORIA_CLINICA_FAMILIAR;
                    if (familiares != null && familiares.Any())
                    {
                        foreach (var item in familiares)
                        {
                            if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.PADRE)
                            {
                                CheckPadreVive = item.AHF_VIVE ?? string.Empty;
                                TextEdadPadre = item.AHF_EDAD ?? new short?();
                                CheckPadrePadece = item.AHF_SANO ?? string.Empty;
                                IsCheckedDiabPadre = item.AHF_DIABETES ?? string.Empty;
                                IsCheckedTBPadre = item.AHF_TB ?? string.Empty;
                                IsCheckedCAPadre = item.AHF_CA ?? string.Empty;
                                IsCheckedCardiPadre = item.AHF_CARDIACOS ?? string.Empty;
                                IsCheckedEpiPadre = item.AHF_EPILEPSIA ?? string.Empty;
                                IsCheckedMentPadre = item.AHF_MENTALES ?? string.Empty;
                                IsCheckedAlergiPadre = item.AHF_ALERGIAS ?? string.Empty;
                                IsCheckedHipertPadre = item.AHF_HIPERTENSIVO ?? string.Empty;
                                CuandoMuertePadre = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                CausaMuertePadre = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                            };

                            if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.MADRE)
                            {
                                CheckMadreVive = item.AHF_VIVE ?? string.Empty;
                                TextEdadMadre = item.AHF_EDAD ?? new short?();
                                CheckMadrePadece = item.AHF_SANO ?? string.Empty;
                                IsCheckedDiabMadre = item.AHF_DIABETES ?? string.Empty;
                                IsCheckedTBMadre = item.AHF_TB ?? string.Empty;
                                IsCheckedCAMadre = item.AHF_CA ?? string.Empty;
                                IsCheckedCardiMadre = item.AHF_CARDIACOS ?? string.Empty;
                                IsCheckedEpiMadre = item.AHF_EPILEPSIA ?? string.Empty;
                                IsCheckedMentMadre = item.AHF_MENTALES ?? string.Empty;
                                IsCheckedAlergiMadre = item.AHF_ALERGIAS ?? string.Empty;
                                IsCheckedHipertMadre = item.AHF_HIPERTENSIVO ?? string.Empty;
                                CuandoMuerteMadre = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                CausaMuerteMadre = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                            };

                            if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.HERMANOS)
                            {
                                CheckHermanosVivos = item.AHF_VIVE ?? string.Empty;
                                CheckHermanosSanos = item.AHF_SANO ?? string.Empty;
                                IsCheckedDiabHnos = item.AHF_DIABETES ?? string.Empty;
                                IsCheckedTBHnos = item.AHF_TB ?? string.Empty;
                                IsCheckedCAHnos = item.AHF_CA ?? string.Empty;
                                IsCheckedCardiHnos = item.AHF_CARDIACOS ?? string.Empty;
                                IsCheckedEpiHnos = item.AHF_EPILEPSIA ?? string.Empty;
                                IsCheckedMentHnos = item.AHF_MENTALES ?? string.Empty;
                                IsCheckedAlergiHnos = item.AHF_ALERGIAS ?? string.Empty;
                                IsCheckedHipertHnos = item.AHF_HIPERTENSIVO ?? string.Empty;
                                CuandoMuerteHnos = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                CausaMuerteHnos = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                TextHermanosMujeres = historia.AHF_HERMANOS_F ?? new short?();
                                TextHermanosHombres = historia.AHF_HERMANOS_M ?? new short?();
                            };

                            if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.CONYUGE)
                            {
                                CheckConyugeVive = item.AHF_VIVE ?? string.Empty;
                                TextEdadConyuge = item.AHF_EDAD ?? new short?();
                                CheckConyugePadece = item.AHF_SANO ?? string.Empty;
                                IsCheckedDiabCony = item.AHF_DIABETES ?? string.Empty;
                                IsCheckedTBCony = item.AHF_TB ?? string.Empty;
                                IsCheckedCACony = item.AHF_CA ?? string.Empty;
                                IsCheckedCardiCony = item.AHF_CARDIACOS ?? string.Empty;
                                IsCheckedEpiCony = item.AHF_EPILEPSIA ?? string.Empty;
                                IsCheckedMentCony = item.AHF_MENTALES ?? string.Empty;
                                IsCheckedAlergiCony = item.AHF_ALERGIAS ?? string.Empty;
                                IsCheckedHipertCony = item.AHF_HIPERTENSIVO ?? string.Empty;
                                CuandoMuerteCony = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                CausaMuerteCony = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                            };

                            if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.HIJOS)
                            {
                                CheckHijosVive = item.AHF_VIVE ?? string.Empty;
                                TextEdadesHijos = item.AHF_EDAD ?? new short?();
                                CheckHijosPadece = item.AHF_SANO ?? string.Empty;
                                IsCheckedDiabHijos = item.AHF_DIABETES ?? string.Empty;
                                IsCheckedTBHijos = item.AHF_TB ?? string.Empty;
                                IsCheckedCAHijos = item.AHF_CA ?? string.Empty;
                                IsCheckedCardiHijos = item.AHF_CARDIACOS ?? string.Empty;
                                IsCheckedEpiHijos = item.AHF_EPILEPSIA ?? string.Empty;
                                IsCheckedMentHijos = item.AHF_MENTALES ?? string.Empty;
                                IsCheckedAlergiHijos = item.AHF_ALERGIAS ?? string.Empty;
                                IsCheckedHipertHijos = item.AHF_HIPERTENSIVO ?? string.Empty;
                                CuandoMuerteHijos = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                CausaMuerteHijos = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                            };
                        };
                    }
                    else
                    {
                        TextEdadConyuge = TextHermanosMujeres = TextHermanosHombres = TextEdadesHijos = TextEdadPadre = TextEdadMadre = new short?();
                        CheckPadreVive = CheckPadrePadece = IsCheckedDiabPadre = IsCheckedTBPadre = IsCheckedCAPadre = IsCheckedCardiPadre = IsCheckedEpiPadre = IsCheckedMentPadre = IsCheckedAlergiPadre = IsCheckedHipertPadre = CausaMuertePadre = string.Empty;
                        CuandoMuerteHijos = CuandoMuerteCony = CuandoMuerteHnos = CuandoMuertePadre = CuandoMuerteMadre = new DateTime?();
                        CheckMadreVive = CheckMadrePadece = IsCheckedDiabMadre = IsCheckedTBMadre = IsCheckedCAMadre = IsCheckedCardiMadre = IsCheckedEpiMadre = IsCheckedMentMadre = IsCheckedAlergiMadre = IsCheckedHipertMadre = CausaMuerteMadre = string.Empty;
                        CausaMuerteHnos = CheckHermanosVivos = CheckHermanosSanos = IsCheckedDiabHnos = IsCheckedTBHnos = IsCheckedCAHnos = IsCheckedCardiHnos = IsCheckedEpiHnos = IsCheckedMentHnos = IsCheckedAlergiHnos = IsCheckedHipertHnos = string.Empty;
                        CheckConyugeVive = CheckConyugePadece = IsCheckedDiabCony = IsCheckedTBCony = IsCheckedCACony = IsCheckedCardiCony = IsCheckedEpiCony = IsCheckedMentCony = IsCheckedAlergiCony = IsCheckedHipertCony = CausaMuerteCony = string.Empty;
                        CheckHijosPadece = IsCheckedDiabHijos = IsCheckedTBHijos = IsCheckedCAHijos = IsCheckedCardiHijos = IsCheckedEpiHijos = IsCheckedMentHijos = IsCheckedAlergiHijos = IsCheckedHipertHijos = CausaMuerteHijos = CheckHijosVive = string.Empty;
                    }

                    #endregion

                    #region APNP
                    TextNacimientoNoPatologicos = historia.APNP_NACIMIENTO;
                    TextAlimentacionNoPatologicos = historia.APNP_ALIMENTACION;
                    TextHabitacionNoPatologicos = historia.APNP_HABITACION;
                    TextAlcoholismoNoPatologicos = historia.APNP_ALCOHOLISMO ?? string.Empty;
                    TextTabaquismoNoPatologicos = historia.APNP_TABAQUISMO ?? string.Empty;
                    TextToxicomaniasNoPatologicos = historia.APNP_TOXICOMANIAS ?? string.Empty;
                    #endregion

                    #region MUJERES
                    CheckMenarquia = historia.MU_MENARQUIA;
                    var _detallesGinecoObstetricos = historia.HISTORIA_CLINICA_GINECO_OBSTRE != null ? historia.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? historia.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault(x => x.MOMENTO_REGISTRO == "EI") : null : null;
                    IdControlPreN = _detallesGinecoObstetricos != null ? _detallesGinecoObstetricos.CONTROL_PRENATAL : string.Empty;
                    TextAniosRitmo = _detallesGinecoObstetricos != null ? _detallesGinecoObstetricos.ANIOS_RITMO : string.Empty;
                    TextEmbarazos = _detallesGinecoObstetricos != null ? _detallesGinecoObstetricos.EMBARAZO : new short?();
                    TextPartos = _detallesGinecoObstetricos != null ? _detallesGinecoObstetricos.PARTO : new short?();
                    TextAbortos = _detallesGinecoObstetricos != null ? _detallesGinecoObstetricos.ABORTO : new short?();
                    TextCesareas = _detallesGinecoObstetricos != null ? _detallesGinecoObstetricos.CESAREA : new short?();
                    FechaUltimaMenstruacion = _detallesGinecoObstetricos != null ? _detallesGinecoObstetricos.ULTIMA_MENSTRUACION_FEC : new DateTime?();
                    TextDeformacionesOrganicas = _detallesGinecoObstetricos != null ? _detallesGinecoObstetricos.DEFORMACION : string.Empty;
                    IdSelectedControlP = _detallesGinecoObstetricos != null ? _detallesGinecoObstetricos.ID_CONTROL_PRENATAL : -1;
                    FechaProbParto = _detallesGinecoObstetricos != null ? _detallesGinecoObstetricos.FECHA_PROBABLE_PARTO : new DateTime?();

                    #endregion

                    MedicamentosActivos = historia.APP_MEDICAMENTOS_ACTIVOS;
                    TextExploracionfisica = historia.EF_DESCRIPCION;
                    #region PADECIMIENTO_ACTUAL
                    TextPadecimientoActual = historia.PADECIMIENTO_ACTUAL;
                    #endregion

                    #region APARATOS_Y_SISTEMAS
                    TextRespiratorio = historia.RESPIRATORIO;
                    TextCardiovascular = historia.CARDIOVASCULAR;
                    TextDigestivo = historia.DIGESTIVO;
                    TextUrinario = historia.URINARIO;
                    TextGenitalMujeres = historia.GENITAL_MUJERES;
                    TextGenitalHombres = historia.GENITAL_HOMBRES;
                    TextEndocrino = historia.ENDOCRINO;
                    TextMusculoEsqueletico = historia.MUSCULO_ESQUELETICO;
                    TextHematicoLinfatico = historia.HEMATICO_LINFACTICO;
                    TextNervioso = historia.NERVIOSO;
                    TextPielAnexos = historia.PIEL_ANEXOS;
                    TextTerapeuticaPrevia = historia.TERAPEUTICA_PREVIA;
                    TextSintomasGenerales = historia.SINTOMAS_GENERALES;
                    #endregion

                    #region EXPLORACION_FISICA
                    TextPeso = historia.EF_PESO;
                    TextEstatura = historia.EF_ESTATURA;
                    TextCabeza = historia.EF_CABEZA;
                    TextCuello = historia.EF_CUELLO;
                    TextTorax = historia.EF_TORAX;
                    TextAbdomen = historia.EF_ABDOMEN;
                    TextRecto = historia.EF_RECTO;
                    TextGenitales = historia.EF_GENITALES;
                    TextExtremidades = historia.EF_EXTREMIDADES;
                    if (!string.IsNullOrEmpty(historia.EF_PRESION_ARTERIAL) ? historia.EF_PRESION_ARTERIAL.Contains("/") : false)
                    {
                        Arterial1 = historia.EF_PRESION_ARTERIAL.Split('/')[0];
                        Arterial2 = historia.EF_PRESION_ARTERIAL.Split('/')[1];
                    }
                    TextPulso = historia.EF_PULSO;
                    TextGenital = SelectIngreso.IMPUTADO.SEXO == "M" ? historia.GENITAL_HOMBRES : historia.GENITAL_MUJERES;
                    TextRespiracion = historia.EF_RESPIRACION;
                    TextTemperatura = historia.EF_TEMPERATURA;
                    TextResultadosAnalisisClinicos = historia.EF_RESULTADO_ANALISIS;
                    TextResultadosestudiosGabinete = historia.EF_RESULTADO_GABINETE;
                    TextImpresionDiagnostica = historia.EF_IMPRESION_DIAGNOSTICA;

                    #endregion

                    #region MAYORES 65
                    IdDisminucionVisua = historia.M65_ALTERACION_VISUAL ?? -1;
                    CheckAgudezaAuditiva = historia.M65_ALTERACION_AUDITIVA ?? string.Empty;
                    CheckOlfacion = historia.M65_ALTERACION_OLFACION ?? string.Empty;
                    CheckCapacidadVisomotriz = historia.M65_ALTERACION_VISOMOTRIZ ?? string.Empty;
                    IdTranstornosMemoria = historia.M65_TRAS_MEMORIA ?? -1;
                    IdTranstornosAtencion = historia.M65_TRAS_ATENCION ?? -1;
                    IdTranstornosComprension = historia.M65_TRAS_COMPRENSION ?? -1;
                    IdTranstornosOrientacion = historia.M65_TRAS_ORIENTACION ?? -1;
                    CheckDemencial = historia.M65_TRAS_DEMENCIAL ?? string.Empty;
                    IdEstadoAnimo = historia.M65_TRAS_ANIMO ?? -1;
                    IdCapacidadParticipacion = historia.M65_PARTICIPACION ?? -1;
                    TextOtroDisminucionVisual = historia.M65_OTROS ?? string.Empty;
                    #endregion

                    #region CONSIDERACIONES_FINALES
                    IdComplica = historia.CP_GRAVEDAD ?? -1;
                    IdEtapaEvo = historia.CP_ETAPA_EVOLUTIVA ?? -1;
                    IdPosibRemis = historia.CP_REMISION ?? -1;
                    IdCapacTrata = historia.CP_CAPACIDAD_TRATAMIENTO ?? -1;
                    IdNivelReq = historia.CP_NIVEL_ATENCION ?? -1;
                    TextGradoAfectacion = historia.CP_GRADO_AFECTACION;
                    TextPronostico = historia.CP_PRONOSTICO;
                    #endregion

                    #region CONCLUSIONES
                    TextConclusiones = historia.CONCLUSIONES;
                    #endregion

                    LstCondensadoPatologicos = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>();

                    LstPatologicos = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>();

                    #region Patologicos
                    if (historia.HISTORIA_CLINICA_PATOLOGICOS.Any() && historia.HISTORIA_CLINICA_PATOLOGICOS != null)
                        foreach (var item in historia.HISTORIA_CLINICA_PATOLOGICOS)
                        {
                            if (item.RECUPERADO == "S")
                            {//Ya se curo. Pero puede enfermarse de nuevo, se muestra dentro de las enfermedades del lazo izq.
                                if (LstPatologicos.Any(x => x.ID_PATOLOGICO == item.ID_PATOLOGICO))
                                    continue;

                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    LstPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS() { OBSERVACIONES = string.Empty, MOMENTO_DETECCION = "EI", PATOLOGICO_CAT = item.PATOLOGICO_CAT, REGISTRO_FEC = Fecha, ID_PATOLOGICO = item.ID_PATOLOGICO, ID_ANIO = SelectIngreso.ID_ANIO, ID_CENTRO = SelectIngreso.ID_CENTRO, ID_IMPUTADO = SelectIngreso.ID_IMPUTADO, ID_INGRESO = SelectIngreso.ID_INGRESO, OTROS_DESCRIPCION = string.Empty, RECUPERADO = "N" });
                                });
                            }

                            else
                            {//NO se curo, aparece del lado derecho

                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    LstCondensadoPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS() { OBSERVACIONES = item.OBSERVACIONES, MOMENTO_DETECCION = "EI", PATOLOGICO_CAT = item.PATOLOGICO_CAT, REGISTRO_FEC = Fecha, ID_PATOLOGICO = item.ID_PATOLOGICO, ID_ANIO = SelectIngreso.ID_ANIO, ID_CENTRO = SelectIngreso.ID_CENTRO, ID_IMPUTADO = SelectIngreso.ID_IMPUTADO, ID_INGRESO = SelectIngreso.ID_INGRESO, OTROS_DESCRIPCION = string.Empty, RECUPERADO = "N" });
                                });
                            };
                        };

                    var Enfermedades = new cPatologicoCat().GetData(y => y.ESTATUS == "S", z => z.DESCR);
                    if (Enfermedades.Any() && Enfermedades != null)
                        foreach (var item in Enfermedades)
                            if (LstCondensadoPatologicos.Any() && LstCondensadoPatologicos != null)
                            {
                                if (LstCondensadoPatologicos.FirstOrDefault(x => x.ID_PATOLOGICO == item.ID_PATOLOGICO) != null)
                                    continue;
                                else
                                {
                                    if (LstPatologicos.Any(x => x.ID_PATOLOGICO == item.ID_PATOLOGICO))
                                        continue;

                                    Application.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        LstPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS() { OBSERVACIONES = string.Empty, MOMENTO_DETECCION = "EI", PATOLOGICO_CAT = item, REGISTRO_FEC = Fecha, ID_PATOLOGICO = item.ID_PATOLOGICO, ID_ANIO = SelectIngreso.ID_ANIO, ID_CENTRO = SelectIngreso.ID_CENTRO, ID_IMPUTADO = SelectIngreso.ID_IMPUTADO, ID_INGRESO = SelectIngreso.ID_INGRESO, OTROS_DESCRIPCION = string.Empty, RECUPERADO = "N" });
                                    });
                                }
                            }
                            else
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    LstPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS() { OBSERVACIONES = string.Empty, MOMENTO_DETECCION = "EI", PATOLOGICO_CAT = item, REGISTRO_FEC = Fecha, ID_PATOLOGICO = item.ID_PATOLOGICO, ID_ANIO = SelectIngreso.ID_ANIO, ID_CENTRO = SelectIngreso.ID_CENTRO, ID_IMPUTADO = SelectIngreso.ID_IMPUTADO, ID_INGRESO = SelectIngreso.ID_INGRESO, OTROS_DESCRIPCION = string.Empty, RECUPERADO = "N" });
                                });

                    #endregion
                    #region Grupos Vulnerables
                    if (historia.GRUPO_VULNERABLE != null && historia.GRUPO_VULNERABLE.Any())
                    {
                        foreach (var item in historia.GRUPO_VULNERABLE.AsQueryable())
                        {
                            if (LstSectoresVulnerbles.Any(z => z.ID_SECTOR_CLAS == item.ID_SECTOR_CLAS))
                                continue;

                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                LstSectoresVulnerbles.Add(new SECTOR_CLASIFICACION
                                {
                                    ID_SECTOR_CLAS = item.ID_SECTOR_CLAS,
                                    POBLACION = item.SECTOR_CLASIFICACION != null ? item.SECTOR_CLASIFICACION.POBLACION : string.Empty,
                                    COLOR = item.SECTOR_CLASIFICACION != null ? item.SECTOR_CLASIFICACION.COLOR : string.Empty,
                                    COLOR_TEXTO = item.SECTOR_CLASIFICACION != null ? item.SECTOR_CLASIFICACION.COLOR_TEXTO : string.Empty,
                                    ES_GRUPO_VULNERABLE = (short)eVulnerablesActivos.SI,
                                    ESTATUS = item.SECTOR_CLASIFICACION != null ? item.SECTOR_CLASIFICACION.ESTATUS : string.Empty,
                                    OBSERV = item.SECTOR_CLASIFICACION != null ? item.SECTOR_CLASIFICACION.OBSERV : string.Empty
                                });
                            });
                        };
                    }
                    else
                    {
                        foreach (var item in _GrposActivos)
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                LstSectoresVulnerbles.Add(new SECTOR_CLASIFICACION() { COLOR = item.COLOR, COLOR_TEXTO = item.COLOR_TEXTO, OBSERV = item.OBSERV, ID_SECTOR_CLAS = item.ID_SECTOR_CLAS, ESTATUS = item.ESTATUS, ES_GRUPO_VULNERABLE = 0, POBLACION = item.POBLACION, SECTOR_OBSERVACION = item.SECTOR_OBSERVACION });
                            });
                        }
                    };

                    #endregion
                    ObservacionesAlcohlismo = historia.APNP_ALCOHOLISMO_OBSERV;
                    ObservacionesTabaquismo = historia.APNP_TABAQUISMO_OBSERV;
                    ObservacionesToxicomanias = historia.APNP_TOXICOMANIAS_OBSERV;
                    MedicamentosActivos = historia.APP_MEDICAMENTOS_ACTIVOS;
                    TextExploracionfisica = historia.EF_DESCRIPCION;

                    if (_IdImputado == null || _IdAnio == null || _IdIngreso == null || _IdCentro == null)
                    {
                        IsDocumentoFisicoEnabled = DigitalizacionDirecta = HabilitaImputados = MenuGuardarEnabled = IsReadOnlyDatos = IsEnabledSignosVitales = false;
                        return;
                    }

                    if (historia.ESTATUS == "T")
                        IsDocumentoFisicoEnabled = DigitalizacionDirecta = HabilitaImputados = MenuGuardarEnabled = IsReadOnlyDatos = IsEnabledSignosVitales = false;
                    else
                        if (string.IsNullOrEmpty(historia.EF_PESO) || string.IsNullOrEmpty(historia.EF_PRESION_ARTERIAL) || string.IsNullOrEmpty(historia.EF_PULSO) || string.IsNullOrEmpty(historia.EF_RESPIRACION) || string.IsNullOrEmpty(historia.EF_TEMPERATURA))
                            IsEnabledSignosVitales = true;//al menos un signo vital esta vacio, permitele capturar de nuevo
                        else
                            IsEnabledSignosVitales = false;
                }
                else
                {
                    var _validaHistoriaAnterior = new cHistoriaClinica().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.INGRESO.ID_UB_CENTRO == SelectIngreso.ID_CENTRO && x.ID_INGRESO != SelectIngreso.ID_INGRESO).OrderByDescending(x => x.ESTUDIO_FEC).FirstOrDefault();

                    if (_validaHistoriaAnterior != null)
                    {
                        #region AHF
                        var familiares = _validaHistoriaAnterior.HISTORIA_CLINICA_FAMILIAR;
                        if (familiares != null && familiares.Any())
                        {
                            foreach (var item in familiares)
                            {
                                if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.PADRE)
                                {
                                    CheckPadreVive = item.AHF_VIVE ?? string.Empty;
                                    TextEdadPadre = item.AHF_EDAD ?? new short?();
                                    CheckPadrePadece = item.AHF_SANO ?? string.Empty;
                                    IsCheckedDiabPadre = item.AHF_DIABETES ?? string.Empty;
                                    IsCheckedTBPadre = item.AHF_TB ?? string.Empty;
                                    IsCheckedCAPadre = item.AHF_CA ?? string.Empty;
                                    IsCheckedCardiPadre = item.AHF_CARDIACOS ?? string.Empty;
                                    IsCheckedEpiPadre = item.AHF_EPILEPSIA ?? string.Empty;
                                    IsCheckedMentPadre = item.AHF_MENTALES ?? string.Empty;
                                    IsCheckedAlergiPadre = item.AHF_ALERGIAS ?? string.Empty;
                                    IsCheckedHipertPadre = item.AHF_HIPERTENSIVO ?? string.Empty;
                                    CuandoMuertePadre = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                    CausaMuertePadre = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                };

                                if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.MADRE)
                                {
                                    CheckMadreVive = item.AHF_VIVE ?? string.Empty;
                                    TextEdadMadre = item.AHF_EDAD ?? new short?();
                                    CheckMadrePadece = item.AHF_SANO ?? string.Empty;
                                    IsCheckedDiabMadre = item.AHF_DIABETES ?? string.Empty;
                                    IsCheckedTBMadre = item.AHF_TB ?? string.Empty;
                                    IsCheckedCAMadre = item.AHF_CA ?? string.Empty;
                                    IsCheckedCardiMadre = item.AHF_CARDIACOS ?? string.Empty;
                                    IsCheckedEpiMadre = item.AHF_EPILEPSIA ?? string.Empty;
                                    IsCheckedMentMadre = item.AHF_MENTALES ?? string.Empty;
                                    IsCheckedAlergiMadre = item.AHF_ALERGIAS ?? string.Empty;
                                    IsCheckedHipertMadre = item.AHF_HIPERTENSIVO ?? string.Empty;
                                    CuandoMuerteMadre = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                    CausaMuerteMadre = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                };

                                if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.HERMANOS)
                                {
                                    CheckHermanosVivos = item.AHF_VIVE ?? string.Empty;
                                    CheckHermanosSanos = item.AHF_SANO ?? string.Empty;
                                    IsCheckedDiabHnos = item.AHF_DIABETES ?? string.Empty;
                                    IsCheckedTBHnos = item.AHF_TB ?? string.Empty;
                                    IsCheckedCAHnos = item.AHF_CA ?? string.Empty;
                                    IsCheckedCardiHnos = item.AHF_CARDIACOS ?? string.Empty;
                                    IsCheckedEpiHnos = item.AHF_EPILEPSIA ?? string.Empty;
                                    IsCheckedMentHnos = item.AHF_MENTALES ?? string.Empty;
                                    IsCheckedAlergiHnos = item.AHF_ALERGIAS ?? string.Empty;
                                    IsCheckedHipertHnos = item.AHF_HIPERTENSIVO ?? string.Empty;
                                    CuandoMuerteHnos = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                    CausaMuerteHnos = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                    TextHermanosMujeres = _validaHistoriaAnterior.AHF_HERMANOS_F ?? new short?();
                                    TextHermanosHombres = _validaHistoriaAnterior.AHF_HERMANOS_M ?? new short?();
                                };

                                if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.CONYUGE)
                                {
                                    CheckConyugeVive = item.AHF_VIVE ?? string.Empty;
                                    TextEdadConyuge = item.AHF_EDAD ?? new short?();
                                    CheckConyugePadece = item.AHF_SANO ?? string.Empty;
                                    IsCheckedDiabCony = item.AHF_DIABETES ?? string.Empty;
                                    IsCheckedTBCony = item.AHF_TB ?? string.Empty;
                                    IsCheckedCACony = item.AHF_CA ?? string.Empty;
                                    IsCheckedCardiCony = item.AHF_CARDIACOS ?? string.Empty;
                                    IsCheckedEpiCony = item.AHF_EPILEPSIA ?? string.Empty;
                                    IsCheckedMentCony = item.AHF_MENTALES ?? string.Empty;
                                    IsCheckedAlergiCony = item.AHF_ALERGIAS ?? string.Empty;
                                    IsCheckedHipertCony = item.AHF_HIPERTENSIVO ?? string.Empty;
                                    CuandoMuerteCony = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                    CausaMuerteCony = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                };

                                if (item.ID_TIPO_REFERENCIA == (short)eFamiliares.HIJOS)
                                {
                                    CheckHijosVive = item.AHF_VIVE ?? string.Empty;
                                    TextEdadesHijos = item.AHF_EDAD ?? new short?();
                                    CheckHijosPadece = item.AHF_SANO ?? string.Empty;
                                    IsCheckedDiabHijos = item.AHF_DIABETES ?? string.Empty;
                                    IsCheckedTBHijos = item.AHF_TB ?? string.Empty;
                                    IsCheckedCAHijos = item.AHF_CA ?? string.Empty;
                                    IsCheckedCardiHijos = item.AHF_CARDIACOS ?? string.Empty;
                                    IsCheckedEpiHijos = item.AHF_EPILEPSIA ?? string.Empty;
                                    IsCheckedMentHijos = item.AHF_MENTALES ?? string.Empty;
                                    IsCheckedAlergiHijos = item.AHF_ALERGIAS ?? string.Empty;
                                    IsCheckedHipertHijos = item.AHF_HIPERTENSIVO ?? string.Empty;
                                    CuandoMuerteHijos = item.AHF_FALLECIMIENTO_FEC ?? new DateTime?();
                                    CausaMuerteHijos = item.AHF_FALLECIMIENTO_CAUSA ?? string.Empty;
                                };
                            };
                        }
                        else
                        {
                            TextEdadConyuge = TextHermanosMujeres = TextHermanosHombres = TextEdadesHijos = TextEdadPadre = TextEdadMadre = new short?();
                            CheckPadreVive = CheckPadrePadece = IsCheckedDiabPadre = IsCheckedTBPadre = IsCheckedCAPadre = IsCheckedCardiPadre = IsCheckedEpiPadre = IsCheckedMentPadre = IsCheckedAlergiPadre = IsCheckedHipertPadre = CausaMuertePadre = string.Empty;
                            CuandoMuerteHijos = CuandoMuerteCony = CuandoMuerteHnos = CuandoMuertePadre = CuandoMuerteMadre = new DateTime?();
                            CheckMadreVive = CheckMadrePadece = IsCheckedDiabMadre = IsCheckedTBMadre = IsCheckedCAMadre = IsCheckedCardiMadre = IsCheckedEpiMadre = IsCheckedMentMadre = IsCheckedAlergiMadre = IsCheckedHipertMadre = CausaMuerteMadre = string.Empty;
                            CausaMuerteHnos = CheckHermanosVivos = CheckHermanosSanos = IsCheckedDiabHnos = IsCheckedTBHnos = IsCheckedCAHnos = IsCheckedCardiHnos = IsCheckedEpiHnos = IsCheckedMentHnos = IsCheckedAlergiHnos = IsCheckedHipertHnos = string.Empty;
                            CheckConyugeVive = CheckConyugePadece = IsCheckedDiabCony = IsCheckedTBCony = IsCheckedCACony = IsCheckedCardiCony = IsCheckedEpiCony = IsCheckedMentCony = IsCheckedAlergiCony = IsCheckedHipertCony = CausaMuerteCony = string.Empty;
                            CheckHijosPadece = IsCheckedDiabHijos = IsCheckedTBHijos = IsCheckedCAHijos = IsCheckedCardiHijos = IsCheckedEpiHijos = IsCheckedMentHijos = IsCheckedAlergiHijos = IsCheckedHipertHijos = CausaMuerteHijos = CheckHijosVive = string.Empty;
                        }

                        #endregion

                        #region APNP
                        TextNacimientoNoPatologicos = _validaHistoriaAnterior.APNP_NACIMIENTO;
                        TextAlimentacionNoPatologicos = _validaHistoriaAnterior.APNP_ALIMENTACION;
                        TextHabitacionNoPatologicos = _validaHistoriaAnterior.APNP_HABITACION;
                        TextAlcoholismoNoPatologicos = _validaHistoriaAnterior.APNP_ALCOHOLISMO ?? string.Empty;
                        TextTabaquismoNoPatologicos = _validaHistoriaAnterior.APNP_TABAQUISMO ?? string.Empty;
                        TextToxicomaniasNoPatologicos = _validaHistoriaAnterior.APNP_TOXICOMANIAS ?? string.Empty;
                        #endregion

                        #region MUJERES
                        CheckMenarquia = _validaHistoriaAnterior.MU_MENARQUIA;
                        var detallesGinecoObstetricos = _validaHistoriaAnterior.HISTORIA_CLINICA_GINECO_OBSTRE != null ? _validaHistoriaAnterior.HISTORIA_CLINICA_GINECO_OBSTRE.Any() ? _validaHistoriaAnterior.HISTORIA_CLINICA_GINECO_OBSTRE.FirstOrDefault(x => x.MOMENTO_REGISTRO == "EI") : null : null;
                        IdControlPreN = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.CONTROL_PRENATAL : string.Empty;
                        TextAniosRitmo = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.ANIOS_RITMO : string.Empty;
                        TextEmbarazos = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.EMBARAZO : new short?();
                        TextPartos = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.PARTO : new short?();
                        TextAbortos = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.ABORTO : new short?();
                        TextCesareas = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.CESAREA : new short?();
                        FechaUltimaMenstruacion = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.ULTIMA_MENSTRUACION_FEC : new DateTime?();
                        TextDeformacionesOrganicas = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.DEFORMACION : string.Empty;
                        IdSelectedControlP = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.ID_CONTROL_PRENATAL : -1;
                        FechaProbParto = detallesGinecoObstetricos != null ? detallesGinecoObstetricos.FECHA_PROBABLE_PARTO : new DateTime?();

                        #endregion

                        #region PADECIMIENTO_ACTUAL
                        TextPadecimientoActual = _validaHistoriaAnterior.PADECIMIENTO_ACTUAL;
                        #endregion

                        #region APARATOS_Y_SISTEMAS
                        TextRespiratorio = _validaHistoriaAnterior.RESPIRATORIO;
                        TextCardiovascular = _validaHistoriaAnterior.CARDIOVASCULAR;
                        TextDigestivo = _validaHistoriaAnterior.DIGESTIVO;
                        TextUrinario = _validaHistoriaAnterior.URINARIO;
                        TextGenitalMujeres = _validaHistoriaAnterior.GENITAL_MUJERES;
                        TextGenitalHombres = _validaHistoriaAnterior.GENITAL_HOMBRES;
                        TextEndocrino = _validaHistoriaAnterior.ENDOCRINO;
                        TextMusculoEsqueletico = _validaHistoriaAnterior.MUSCULO_ESQUELETICO;
                        TextHematicoLinfatico = _validaHistoriaAnterior.HEMATICO_LINFACTICO;
                        TextNervioso = _validaHistoriaAnterior.NERVIOSO;
                        TextPielAnexos = _validaHistoriaAnterior.PIEL_ANEXOS;
                        TextTerapeuticaPrevia = _validaHistoriaAnterior.TERAPEUTICA_PREVIA;
                        TextSintomasGenerales = _validaHistoriaAnterior.SINTOMAS_GENERALES;
                        #endregion

                        #region EXPLORACION_FISICA
                        var _signosVitales = new cHistoriaClinica().GetData(x => x.INGRESO.ID_UB_CENTRO == SelectIngreso.ID_CENTRO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault();
                        TextPeso = _signosVitales != null ? _signosVitales.EF_PESO : _validaHistoriaAnterior.EF_PESO;
                        TextEstatura = _signosVitales != null ? _signosVitales.EF_ESTATURA : _validaHistoriaAnterior.EF_ESTATURA;
                        TextCabeza = _validaHistoriaAnterior.EF_CABEZA;
                        TextCuello = _validaHistoriaAnterior.EF_CUELLO;
                        TextTorax = _validaHistoriaAnterior.EF_TORAX;
                        TextAbdomen = _validaHistoriaAnterior.EF_ABDOMEN;
                        TextRecto = _validaHistoriaAnterior.EF_RECTO;
                        TextGenitales = _validaHistoriaAnterior.EF_GENITALES;
                        TextExtremidades = _validaHistoriaAnterior.EF_EXTREMIDADES;
                        if (!string.IsNullOrEmpty(_validaHistoriaAnterior.EF_PRESION_ARTERIAL) ? _validaHistoriaAnterior.EF_PRESION_ARTERIAL.Contains("/") : false)
                        {
                            Arterial1 = _signosVitales != null ? _signosVitales.EF_PRESION_ARTERIAL.Split('/')[0] : _validaHistoriaAnterior != null ? _validaHistoriaAnterior.EF_PRESION_ARTERIAL.Split('/')[0] : string.Empty;
                            Arterial2 = _signosVitales != null ? _signosVitales.EF_PRESION_ARTERIAL.Split('/')[1] : _validaHistoriaAnterior != null ? _validaHistoriaAnterior.EF_PRESION_ARTERIAL.Split('/')[1] : string.Empty;
                        }

                        TextPulso = _signosVitales != null ? _signosVitales.EF_PULSO : _validaHistoriaAnterior.EF_PULSO;
                        TextGenital = SelectIngreso.IMPUTADO.SEXO == "M" ? _validaHistoriaAnterior.GENITAL_HOMBRES : _validaHistoriaAnterior.GENITAL_MUJERES;
                        TextRespiracion = _signosVitales != null ? _signosVitales.EF_RESPIRACION : _validaHistoriaAnterior.EF_RESPIRACION;
                        TextTemperatura = _signosVitales != null ? _signosVitales.EF_TEMPERATURA : _validaHistoriaAnterior.EF_TEMPERATURA;
                        TextResultadosAnalisisClinicos = _validaHistoriaAnterior.EF_RESULTADO_ANALISIS;
                        TextResultadosestudiosGabinete = _validaHistoriaAnterior.EF_RESULTADO_GABINETE;
                        TextImpresionDiagnostica = _validaHistoriaAnterior.EF_IMPRESION_DIAGNOSTICA;

                        #endregion

                        MedicamentosActivos = _validaHistoriaAnterior.APP_MEDICAMENTOS_ACTIVOS;

                        #region MAYORES 65
                        IdDisminucionVisua = _validaHistoriaAnterior.M65_ALTERACION_VISUAL ?? -1;
                        CheckAgudezaAuditiva = _validaHistoriaAnterior.M65_ALTERACION_AUDITIVA ?? string.Empty;
                        CheckOlfacion = _validaHistoriaAnterior.M65_ALTERACION_OLFACION ?? string.Empty;
                        CheckCapacidadVisomotriz = _validaHistoriaAnterior.M65_ALTERACION_VISOMOTRIZ ?? string.Empty;
                        IdTranstornosMemoria = _validaHistoriaAnterior.M65_TRAS_MEMORIA ?? -1;
                        IdTranstornosAtencion = _validaHistoriaAnterior.M65_TRAS_ATENCION ?? -1;
                        IdTranstornosComprension = _validaHistoriaAnterior.M65_TRAS_COMPRENSION ?? -1;
                        IdTranstornosOrientacion = _validaHistoriaAnterior.M65_TRAS_ORIENTACION ?? -1;
                        CheckDemencial = _validaHistoriaAnterior.M65_TRAS_DEMENCIAL ?? string.Empty;
                        IdEstadoAnimo = _validaHistoriaAnterior.M65_TRAS_ANIMO ?? -1;
                        IdCapacidadParticipacion = _validaHistoriaAnterior.M65_PARTICIPACION ?? -1;
                        TextOtroDisminucionVisual = _validaHistoriaAnterior.M65_OTROS ?? string.Empty;
                        #endregion

                        #region CONSIDERACIONES_FINALES
                        IdComplica = _validaHistoriaAnterior.CP_GRAVEDAD ?? -1;
                        IdEtapaEvo = _validaHistoriaAnterior.CP_ETAPA_EVOLUTIVA ?? -1;
                        IdPosibRemis = _validaHistoriaAnterior.CP_REMISION ?? -1;
                        IdCapacTrata = _validaHistoriaAnterior.CP_CAPACIDAD_TRATAMIENTO ?? -1;
                        IdNivelReq = _validaHistoriaAnterior.CP_NIVEL_ATENCION ?? -1;
                        TextGradoAfectacion = _validaHistoriaAnterior.CP_GRADO_AFECTACION;
                        TextPronostico = _validaHistoriaAnterior.CP_PRONOSTICO;
                        #endregion

                        #region CONCLUSIONES
                        TextConclusiones = _validaHistoriaAnterior.CONCLUSIONES;
                        #endregion

                        TextExploracionfisica = _validaHistoriaAnterior.EF_DESCRIPCION;

                        if (_IdImputado == null || _IdAnio == null || _IdIngreso == null || _IdCentro == null)//NO TIENE UNA HISTORIA CLINICA Y NO VIENE DEL CONSTRUCTOR
                            IsDocumentoFisicoEnabled = DigitalizacionDirecta = HabilitaImputados = MenuGuardarEnabled = IsReadOnlyDatos = IsEnabledSignosVitales = false;

                        if (string.IsNullOrEmpty(_validaHistoriaAnterior.EF_PESO) || string.IsNullOrEmpty(_validaHistoriaAnterior.EF_PRESION_ARTERIAL) || string.IsNullOrEmpty(_validaHistoriaAnterior.EF_PULSO) || string.IsNullOrEmpty(_validaHistoriaAnterior.EF_RESPIRACION) || string.IsNullOrEmpty(_validaHistoriaAnterior.EF_TEMPERATURA))
                            IsEnabledSignosVitales = true;//al menos un signo vital esta vacio, permitele capturar de nuevo
                        else
                            IsEnabledSignosVitales = false;

                        #region Patologicos
                        System.DateTime _fechaAct = Fechas.GetFechaDateServer;
                        LstCondensadoPatologicos = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>(_validaHistoriaAnterior.HISTORIA_CLINICA_PATOLOGICOS.Select(x => new HISTORIA_CLINICA_PATOLOGICOS
                        {
                            FUENTE = "X",
                            ID_ANIO = x.ID_ANIO,
                            ID_CENTRO = x.ID_CENTRO,
                            ID_CONSEC = x.ID_CONSEC,
                            ID_IMPUTADO = x.ID_IMPUTADO,
                            ID_INGRESO = x.ID_INGRESO,
                            ID_NOPATOLOGICO = x.ID_NOPATOLOGICO,
                            MOMENTO_DETECCION = "EI",
                            ID_PATOLOGICO = x.ID_PATOLOGICO,
                            OBSERVACIONES = x.OBSERVACIONES,
                            OTROS_DESCRIPCION = x.OTROS_DESCRIPCION,
                            RECUPERADO = x.RECUPERADO,
                            REGISTRO_FEC = _fechaAct,
                            PATOLOGICO_CAT = x.PATOLOGICO_CAT
                        }));

                        LstPatologicos = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>();

                        var Enfermedades = new cPatologicoCat().GetData(y => y.ESTATUS == "S", z => z.DESCR);
                        if (Enfermedades.Any() && Enfermedades != null)
                            foreach (var item in Enfermedades)
                                if (LstCondensadoPatologicos.Any() && LstCondensadoPatologicos != null)
                                    if (LstCondensadoPatologicos.FirstOrDefault(x => x.ID_PATOLOGICO == item.ID_PATOLOGICO) != null)
                                        continue;
                                    else
                                        Application.Current.Dispatcher.Invoke((Action)delegate
                                        {
                                            LstPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS() { OBSERVACIONES = string.Empty, MOMENTO_DETECCION = "EI", PATOLOGICO_CAT = item, REGISTRO_FEC = Fecha, ID_PATOLOGICO = item.ID_PATOLOGICO, ID_ANIO = SelectIngreso.ID_ANIO, ID_CENTRO = SelectIngreso.ID_CENTRO, ID_IMPUTADO = SelectIngreso.ID_IMPUTADO, ID_INGRESO = SelectIngreso.ID_INGRESO, OTROS_DESCRIPCION = string.Empty, RECUPERADO = "N" });
                                        });

                                else
                                    Application.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        LstPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS() { OBSERVACIONES = string.Empty, MOMENTO_DETECCION = "EI", PATOLOGICO_CAT = item, REGISTRO_FEC = Fecha, ID_PATOLOGICO = item.ID_PATOLOGICO, ID_ANIO = SelectIngreso.ID_ANIO, ID_CENTRO = SelectIngreso.ID_CENTRO, ID_IMPUTADO = SelectIngreso.ID_IMPUTADO, ID_INGRESO = SelectIngreso.ID_INGRESO, OTROS_DESCRIPCION = string.Empty, RECUPERADO = "N" });
                                    });
                        #endregion

                        #region Vulnerables
                        if (_GrposActivos != null && _GrposActivos.Any())
                            foreach (var item in _GrposActivos)
                            {
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    LstSectoresVulnerbles.Add(new SECTOR_CLASIFICACION() { COLOR = item.COLOR, COLOR_TEXTO = item.COLOR_TEXTO, OBSERV = item.OBSERV, ID_SECTOR_CLAS = item.ID_SECTOR_CLAS, ESTATUS = item.ESTATUS, ES_GRUPO_VULNERABLE = 0, POBLACION = item.POBLACION, SECTOR_OBSERVACION = item.SECTOR_OBSERVACION });
                                });
                            }
                        #endregion
                        return;
                    }

                    TextNacimientoNoPatologicos = TextAlimentacionNoPatologicos = TextHabitacionNoPatologicos = MedicamentosActivos = TextAlcoholismoNoPatologicos = TextTabaquismoNoPatologicos = TextToxicomaniasNoPatologicos = IdControlPreN = string.Empty;
                    IdSelectedControlP = -1;
                    TextEmbarazos = TextPartos = TextAbortos = TextCesareas = 0;
                    TextAniosRitmo = TextDeformacionesOrganicas = TextIntegridadFisica = TextPadecimientoActual = TextRespiratorio = TextCardiovascular = TextDigestivo = TextUrinario = TextGenitalMujeres = TextGenitalHombres = TextEndocrino = TextMusculoEsqueletico = TextHematicoLinfatico = TextNervioso = TextPielAnexos = TextTerapeuticaPrevia = TextSintomasGenerales = string.Empty;
                    TextPeso = TextEstatura = TextCabeza = TextCuello = TextTorax = TextExploracionfisica = TextAbdomen = TextRecto = TextGenitales = TextExtremidades = Arterial1 = Arterial2 = TextPresionArterial = TextPulso = TextRespiracion = TextTemperatura = TextResultadosAnalisisClinicos = TextResultadosestudiosGabinete = TextImpresionDiagnostica = string.Empty;
                    IdDisminucionVisua = IdTranstornosMemoria = IdTranstornosAtencion = IdTranstornosComprension = IdTranstornosOrientacion = IdEstadoAnimo = IdCapacidadParticipacion = -1;
                    CheckAgudezaAuditiva = CheckOlfacion = CheckCapacidadVisomotriz = CheckDemencial = TextOtroDisminucionVisual = ObservacionesAlcohlismo = ObservacionesTabaquismo = ObservacionesToxicomanias = TextGradoAfectacion = TextPronostico = TextConclusiones = string.Empty;
                    IdComplica = IdEtapaEvo = IdPosibRemis = IdCapacTrata = IdNivelReq = -1;
                    CheckMenarquia = 0;

                    FechaUltimaMenstruacion = FechaProbParto = new DateTime?();
                    #region Patologicos
                    LstCondensadoPatologicos = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>();
                    LstPatologicos = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>();

                    var _Enfermedades = new cPatologicoCat().GetData(y => y.ESTATUS == "S", z => z.DESCR);
                    if (_Enfermedades.Any() && _Enfermedades != null)
                        foreach (var item in _Enfermedades)
                            if (LstCondensadoPatologicos.Any() && LstCondensadoPatologicos != null)
                                if (LstCondensadoPatologicos.FirstOrDefault(x => x.ID_PATOLOGICO == item.ID_PATOLOGICO) != null)
                                    continue;
                                else
                                    Application.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        LstPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS() { OBSERVACIONES = string.Empty, MOMENTO_DETECCION = "EI", PATOLOGICO_CAT = item, REGISTRO_FEC = Fecha, ID_PATOLOGICO = item.ID_PATOLOGICO, ID_ANIO = SelectIngreso.ID_ANIO, ID_CENTRO = SelectIngreso.ID_CENTRO, ID_IMPUTADO = SelectIngreso.ID_IMPUTADO, ID_INGRESO = SelectIngreso.ID_INGRESO, OTROS_DESCRIPCION = string.Empty, RECUPERADO = "N" });
                                    });

                            else
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    LstPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS() { OBSERVACIONES = string.Empty, MOMENTO_DETECCION = "EI", PATOLOGICO_CAT = item, REGISTRO_FEC = Fecha, ID_PATOLOGICO = item.ID_PATOLOGICO, ID_ANIO = SelectIngreso.ID_ANIO, ID_CENTRO = SelectIngreso.ID_CENTRO, ID_IMPUTADO = SelectIngreso.ID_IMPUTADO, ID_INGRESO = SelectIngreso.ID_INGRESO, OTROS_DESCRIPCION = string.Empty, RECUPERADO = "N" });
                                });
                    #endregion
                    #region AHF
                    TextEdadesHijos = TextEdadPadre = TextEdadMadre = new short?();
                    CheckPadreVive = CheckPadrePadece = IsCheckedDiabPadre = IsCheckedTBPadre = IsCheckedCAPadre = IsCheckedCardiPadre = IsCheckedEpiPadre = IsCheckedMentPadre = IsCheckedAlergiPadre = IsCheckedHipertPadre = CausaMuertePadre = string.Empty;
                    CuandoMuerteHijos = CuandoMuerteCony = CuandoMuerteHnos = CuandoMuertePadre = CuandoMuerteMadre = new DateTime?();
                    CheckMadreVive = CheckMadrePadece = IsCheckedDiabMadre = IsCheckedTBMadre = IsCheckedCAMadre = IsCheckedCardiMadre = IsCheckedEpiMadre = IsCheckedMentMadre = IsCheckedAlergiMadre = IsCheckedHipertMadre = CausaMuerteMadre = string.Empty;
                    CausaMuerteHnos = CheckHermanosVivos = CheckHermanosSanos = IsCheckedDiabHnos = IsCheckedTBHnos = IsCheckedCAHnos = IsCheckedCardiHnos = IsCheckedEpiHnos = IsCheckedMentHnos = IsCheckedAlergiHnos = IsCheckedHipertHnos = string.Empty;
                    TextEdadConyuge = TextHermanosMujeres = TextHermanosHombres = new short?();
                    CheckConyugeVive = CheckConyugePadece = IsCheckedDiabCony = IsCheckedTBCony = IsCheckedCACony = IsCheckedCardiCony = IsCheckedEpiCony = IsCheckedMentCony = IsCheckedAlergiCony = IsCheckedHipertCony = CausaMuerteCony = string.Empty;
                    CheckHijosPadece = IsCheckedDiabHijos = IsCheckedTBHijos = IsCheckedCAHijos = IsCheckedCardiHijos = IsCheckedEpiHijos = IsCheckedMentHijos = IsCheckedAlergiHijos = IsCheckedHipertHijos = CausaMuerteHijos = CheckHijosVive = string.Empty;
                    #endregion

                    #region Vulnerables
                    if (_GrposActivos != null && _GrposActivos.Any())
                        foreach (var item in _GrposActivos)
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                LstSectoresVulnerbles.Add(new SECTOR_CLASIFICACION() { COLOR = item.COLOR, COLOR_TEXTO = item.COLOR_TEXTO, OBSERV = item.OBSERV, ID_SECTOR_CLAS = item.ID_SECTOR_CLAS, ESTATUS = item.ESTATUS, ES_GRUPO_VULNERABLE = 0, POBLACION = item.POBLACION, SECTOR_OBSERVACION = item.SECTOR_OBSERVACION });
                            });
                        }
                    #endregion

                    if (_IdImputado == null || _IdAnio == null || _IdIngreso == null || _IdCentro == null)//NO TIENE UNA HISTORIA CLINICA Y NO VIENE DEL CONSTRUCTOR
                        IsDocumentoFisicoEnabled = DigitalizacionDirecta = HabilitaImputados = MenuGuardarEnabled = IsReadOnlyDatos = IsEnabledSignosVitales = false;
                }
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar historia clínica del imputado.", ex);
            }
        }

        private void LimpiarDatosImputado()
        {
            try
            {
                IngresosD = FolioBuscar = AnioBuscar = FolioD = AnioD = new int?();
                TextSentencia = TextDelito = TextEstadoCivil = TextOcupacion = TextEscolaridad = TextLugarNacimiento = SelectSexo = EstatusD = ClasificacionJuridicaD = TipoSeguridadD = UbicacionD = NombreD = MaternoD = PaternoD = string.Empty;
                FecIngresoD = TextAPartir = SelectFechaNacimiento = new DateTime?();
                TextEdad = new short?();
                FotoIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar datos en pantalla.", ex);
            }
        }

        private void LimpiarHistoriaClinica()
        {
            try
            {
                #region AHF
                CheckHijosVive = CheckHijosPadece = CheckPadreVive = CheckPadrePadece = CheckMadreVive = CheckMadrePadece = CheckHermanosVivos = CheckHermanosSanos = CheckConyugeVive = string.Empty;
                TextEdadPadre = TextEdadMadre = TextHermanosHombres = TextHermanosMujeres = TextEdadesHijos = TextEdadConyuge = new short?();
                CheckDiabetes = CheckTB = CheckCA = CheckCardiacos = CheckEpilepsia = CheckMentales = CheckAlergias = CheckHipertensivo = false;
                #endregion

                #region APNP
                TextToxicomaniasNoPatologicos = TextTabaquismoNoPatologicos = TextAlcoholismoNoPatologicos = TextHabitacionNoPatologicos = TextAlimentacionNoPatologicos = TextNacimientoNoPatologicos = string.Empty;
                #endregion

                #region APP
                TextIntolerancias = TextTransfusionales = TextParasitarios = TextAmigdalitis = TextParotiditis = TextTosferina = TextPaludicos = TextQuirurgicos = TextTraumaticos = TextAlergicos = TextFimicos = TextLueticos = TextRubeola = TextViruela = TextSarampion = string.Empty;
                #endregion

                #region MUJERES
                TextCesareas = TextAbortos = TextPartos = TextEmbarazos = CheckMenarquia = new short?();
                FechaUltimaMenstruacion = new DateTime?();
                TextAniosRitmo = TextDeformacionesOrganicas = TextIntegridadFisica = string.Empty;
                #endregion

                #region PADECIMIENTO_ACTUAL
                TextPadecimientoActual = string.Empty;
                #endregion

                #region APARATOS_Y_SISTEMAS
                TextRespiratorio = string.Empty;
                TextCardiovascular = string.Empty;
                TextDigestivo = string.Empty;
                TextUrinario = string.Empty;
                GenitalMujer = Visibility.Collapsed;
                GenitalHombre = Visibility.Collapsed;
                TextGenitalMujeres = string.Empty;
                TextGenitalHombres = string.Empty;
                TextEndocrino = string.Empty;
                TextMusculoEsqueletico = string.Empty;
                TextHematicoLinfatico = string.Empty;
                TextNervioso = string.Empty;
                TextPielAnexos = string.Empty;
                TextTerapeuticaPrevia = string.Empty;
                #endregion

                #region EXPLORACION_FISICA
                TextPeso = string.Empty;
                TextEstatura = string.Empty;
                TextCabeza = string.Empty;
                TextCuello = string.Empty;
                TextTorax = string.Empty;
                TextAbdomen = string.Empty;
                TextRecto = string.Empty;
                TextGenitales = string.Empty;
                TextExtremidades = string.Empty;
                TextPresionArterial = string.Empty;
                TextPulso = string.Empty;
                TextRespiracion = string.Empty;
                TextTemperatura = string.Empty;
                TextResultadosAnalisisClinicos = string.Empty;
                TextResultadosestudiosGabinete = string.Empty;
                TextImpresionDiagnostica = string.Empty;
                #endregion

                #region MAYORES 65
                CheckPresbicia = false;
                CheckCataratas = false;
                CheckPterigion = false;
                CheckOtros = false;
                TextOtroDisminucionVisual = null;
                CheckAgudezaAuditiva = string.Empty;
                CheckOlfacion = string.Empty;
                CheckCapacidadVisomotriz = string.Empty;
                CheckMemoriaNinguno = false;
                CheckMemoriaSuperficiales = false;
                CheckMemoriaModerados = false;
                CheckMemoriaGraves = false;
                CheckAtencionNinguno = false;
                CheckAtencionSuperficiales = false;
                CheckAtencionModerados = false;
                CheckAtencionGraves = false;
                CheckComprensionNinguno = false;
                CheckComprensionSuperficiales = false;
                CheckComprensionModerados = false;
                CheckComprensionGraves = false;
                CheckOrientacionNinguno = false;
                CheckOrientacionSuperficiales = false;
                CheckOrientacionModerados = false;
                CheckOrientacionGraves = false;
                CheckEstadoAnimoDisforia = false;
                CheckEstadoAnimoIncontinencia = false;
                CheckEstadoAnimoDepresion = false;
                CheckDemencial = string.Empty;
                CheckReadaptacionIntegra = false;
                CheckReadaptacionLimitada = false;
                CheckReadaptacionNula = false;
                #endregion

                #region CONSIDERACIONES_FINALES
                CheckComplicacionesLeve = false;
                CheckComplicacionesModerada = false;
                CheckComplicacionesSevera = false;
                CheckEtapaEvolutivaInicial = false;
                CheckEtapaEvolutivaMedia = false;
                CheckEtapaEvolutivaTerminal = false;
                CheckRemisionReversible = false;
                CheckRemisionIrreversible = false;
                TextGradoAfectacion = string.Empty;
                TextPronostico = string.Empty;
                CheckCapacidadTratamientoSuficiente = false;
                CheckCapacidadTratamientoMediana = false;
                CheckCapacidadTratamientoEscasa = false;
                CheckCapacidadTratamientoNula = false;
                CheckAtencionNivel1 = false;
                CheckAtencionNivel2 = false;
                CheckAtencionNivel3 = false;
                #endregion

                #region CONCLUSIONES
                TextConclusiones = string.Empty;
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar datos en pantalla.", ex);
            }
        }

        private void EnterBuscar(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                    return;
                }
                if (obj != null)
                {
                    if (obj.ToString() != "buscar_visible")
                    {
                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    NombreD = NombreBuscar = textbox.Text;
                                    AnioD = AnioBuscar;
                                    FolioD = FolioBuscar;
                                    MaternoD = ApellidoMaternoBuscar;
                                    PaternoD = ApellidoPaternoBuscar;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    PaternoD = ApellidoPaternoBuscar = textbox.Text;
                                    AnioD = AnioBuscar;
                                    FolioD = FolioBuscar;
                                    MaternoD = ApellidoMaternoBuscar;
                                    NombreD = NombreBuscar;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    MaternoD = ApellidoMaternoBuscar = textbox.Text;
                                    AnioD = AnioBuscar;
                                    FolioD = FolioBuscar;
                                    PaternoD = ApellidoPaternoBuscar;
                                    NombreD = NombreBuscar;
                                    break;
                                case "FolioBuscar":
                                    FolioD = string.IsNullOrEmpty(textbox.Text) ? new int?() : int.Parse(textbox.Text);
                                    FolioBuscar = string.IsNullOrEmpty(textbox.Text) ? new int?() : int.Parse(textbox.Text);
                                    AnioD = AnioBuscar;
                                    NombreD = NombreBuscar;
                                    PaternoD = ApellidoPaternoBuscar;
                                    MaternoD = ApellidoMaternoBuscar;
                                    break;
                                case "AnioBuscar":
                                    AnioD = string.IsNullOrEmpty(textbox.Text) ? new int?() : int.Parse(textbox.Text);
                                    AnioBuscar = string.IsNullOrEmpty(textbox.Text) ? new int?() : int.Parse(textbox.Text);
                                    FolioD = FolioBuscar;
                                    NombreD = NombreBuscar;
                                    PaternoD = ApellidoPaternoBuscar;
                                    MaternoD = ApellidoMaternoBuscar;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        NombreBuscar = NombreD;
                        ApellidoPaternoBuscar = PaternoD;
                        ApellidoMaternoBuscar = MaternoD;
                    }
                    var ing = SelectIngreso;
                    SelectIngresoAuxiliar = ing;
                    var exp = SelectExpediente;
                    SelectExpedienteAuxiliar = exp;
                    BuscarImputado(NombreD, PaternoD, MaternoD);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de imputados.", ex);
            }
        }

        private async void BuscarExpediente(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                    return;
                }
                if (obj != null)
                {
                    if (obj.ToString() != "buscar_visible")
                    {
                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    NombreBuscar = NombreD = textbox.Text;
                                    AnioBuscar = AnioD = TextAnioImputado;
                                    FolioBuscar = FolioD = TextFolioImputado;
                                    ApellidoMaternoBuscar = MaternoD = TextMaternoImputado;
                                    ApellidoPaternoBuscar = PaternoD = TextPaternoImputado;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    ApellidoPaternoBuscar = PaternoD = textbox.Text;
                                    AnioBuscar = AnioD = TextAnioImputado;
                                    FolioBuscar = FolioD = TextFolioImputado;
                                    ApellidoMaternoBuscar = MaternoD = TextMaternoImputado;
                                    NombreBuscar = NombreD = TextNombreImputado;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    ApellidoMaternoBuscar = MaternoD = textbox.Text;
                                    AnioBuscar = AnioD = TextAnioImputado;
                                    FolioBuscar = FolioD = TextFolioImputado;
                                    ApellidoPaternoBuscar = PaternoD = TextPaternoImputado;
                                    NombreBuscar = NombreD = TextNombreImputado;
                                    break;
                                case "FolioBuscar":
                                    FolioBuscar = !string.IsNullOrEmpty(textbox.Text) ? int.Parse(textbox.Text) : new int?();
                                    AnioBuscar = AnioD = TextAnioImputado;
                                    ApellidoMaternoBuscar = MaternoD = TextMaternoImputado;
                                    NombreBuscar = NombreD = TextNombreImputado;
                                    ApellidoPaternoBuscar = PaternoD = TextPaternoImputado;
                                    break;
                                case "AnioBuscar":
                                    AnioBuscar = !string.IsNullOrEmpty(textbox.Text) ? int.Parse(textbox.Text) : new int?();
                                    FolioBuscar = FolioD = TextFolioImputado;
                                    ApellidoMaternoBuscar = MaternoD = TextMaternoImputado;
                                    NombreBuscar = NombreD = TextNombreImputado;
                                    ApellidoPaternoBuscar = PaternoD = TextPaternoImputado;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        TextNombreImputado = TextMaternoImputado = TextPaternoImputado = string.Empty;
                        TextAnioImputado = TextFolioImputado = new int?();
                        NombreD = NombreBuscar = PaternoD = ApellidoPaternoBuscar = MaternoD = ApellidoMaternoBuscar = string.Empty;
                        AnioD = FolioD = FolioBuscar = AnioBuscar = new int?();
                        ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                        ImagenIngreso = new Imagenes().getImagenPerson();
                        var ing = SelectIngreso;
                        SelectIngresoAuxiliar = ing;
                        var exp = SelectExpediente;
                        SelectExpedienteAuxiliar = exp;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)
                    {
                        SelectExpediente = ListExpediente.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).FirstOrDefault();
                    }
                    if (ListExpediente.Count == 1 && AnioBuscar.HasValue && FolioBuscar.HasValue)
                    {
                        MenuGuardarEnabled = !PInsertar ? PEditar : true;
                        if (StaticSourcesViewModel.SourceChanged ? await new Dialogos().ConfirmarEliminar("Advertencia!",
                            "Existen cambios sin guardar, está seguro que desea seleccionar a otro imputado?") != 1 : false)
                            return;

                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;

                        if (Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(a => a.HasValue ? a.Value == SelectIngreso.ID_ESTATUS_ADMINISTRATIVO : false))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                            }));
                            return;
                        }

                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                    SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no puede recibir visitas.");
                            }));
                            return;
                        }

                        if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no pertenece a este centro.");
                            }));
                            return;
                        }

                        if (EsDentista)
                        {
                            if (SelectIngreso.HISTORIA_CLINICA == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación!", "Es necesario capturar historia clínica médica al imputado");
                                SelectExpediente = null;
                                SelectIngreso = null;
                                return;
                            }

                            if (SelectIngreso.HISTORIA_CLINICA.Count == 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación!", "Es necesario capturar historia clínica médica al imputado");
                                SelectExpediente = null;
                                SelectIngreso = null;
                                return;
                            }
                        }

                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            TabsEnabled = true;
                            GetDatosImputadoSeleccionado();
                            StaticSourcesViewModel.SourceChanged = false;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        });

                        return;
                    }
                    else if (ListExpediente.Count == 0 && (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar)))
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun imputado con esos datos.");
                    EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de imputados.", ex);
            }
        }

        private async void BuscarImputado(string nombre, string paterno, string materno)
        {
            try
            {
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)
                {
                    SelectExpediente = ListExpediente.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).FirstOrDefault();
                }
                if (ListExpediente.Count == 1)
                {
                    ImagenIngreso = new Imagenes().getImagenPerson();
                    var EstatusLiberado = Parametro.ID_ESTATUS_ADMVO_LIBERADO;
                    if (ListExpediente[0].INGRESO.Any(a => a.ID_ESTATUS_ADMINISTRATIVO != EstatusLiberado))
                    {
                        if (ListExpediente[0].INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != EstatusLiberado).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no pertenece a este centro.");
                            return;
                        }

                        SelectIngreso = ListExpediente[0].INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != EstatusLiberado).FirstOrDefault();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                }
                else if (ListExpediente.Count == 0)
                {
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    if (!string.IsNullOrEmpty(NombreD) || !string.IsNullOrEmpty(PaternoD) || !string.IsNullOrEmpty(MaternoD))
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun imputado con esos datos.");
                }

                EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al hacer la búsqueda.", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar los imputados.", ex);
                return new List<IMPUTADO>();
            }
        }

        private void ConfiguraPermisos()
        {
            try
            {
                var _detalleUsuarioActual = new cUsuario().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr).FirstOrDefault();

                if (_detalleUsuarioActual != null)
                {
                    foreach (var item in _detalleUsuarioActual.USUARIO_ROL)
                        if (item.ID_ROL == (short)eTiposMedicos.DENTISTA)
                            EsDentista = true;

                    if (EsDentista)
                    {
                        DigitalizacionDirecta = BuscarImputadoHabilitado = false;
                        var permisos = new cProcesoUsuario().Obtener(enumProcesos.HISTORIA_CLINICA_DENTAL.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
                    else
                    {
                        SubirImagenesDental = false;
                        var permisos = new cProcesoUsuario().Obtener(enumProcesos.HISTORIA_CLINICA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }

        #region Arbol
        private void ArbolViewM()
        {
            try
            {
                INGRESO Ingre = SelectIngreso;

                if (Ingre != null)
                {
                    HISTORIA_CLINICA HistClinica = new cHistoriaClinica().GetData(x => x.ID_INGRESO == Ingre.ID_INGRESO && x.ID_IMPUTADO == Ingre.ID_IMPUTADO && x.ID_ANIO == Ingre.ID_ANIO).FirstOrDefault();
                    var TreeNodeExamen = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeAntecedentesHeredoFam = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeAntecedentesNoPato = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeAntecedentesPato = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeMujeres = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodePadecimientoActual = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeAparaSist = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeExplorFisica = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNode65Mas = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeConsideraciones = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeConclusiones = new List<ControlPenales.Clases.TreeViewList>();
                    var ItemTreeRaiz = new ControlPenales.Clases.TreeViewList();
                    var ItemTreeIngreso = new List<ControlPenales.Clases.TreeViewList>();
                    var ItemTreeCausaPenal = new List<ControlPenales.Clases.TreeViewList>();


                    List<ControlPenales.Clases.TreeViewList> nodeX = null;
                    if (ItemTreeCausaPenal != null)
                        if (ItemTreeCausaPenal.Count > 0)
                            nodeX = ItemTreeCausaPenal;

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "FICHA DE IDENTIFICACIÓN",
                        Value = HistClinica,
                        Node = nodeX,
                        IsNodeExpanded = true

                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "ANTECEDENTES HEREDO-FAMILIARES",
                        Value = HistClinica,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "ANTECEDENTES PERSONALES NO PATOLÓGICOS",
                        Value = HistClinica,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "ANTECEDENTES PERSONALES PATOLÓGICOS",
                        Value = HistClinica,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                    {
                        if (SelectIngreso.IMPUTADO.SEXO == "F")
                        {
                            LimpiaValidacionesMujeres();
                            SetValidacionesMujeres();
                            ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                            {
                                IsRoot = false,
                                IsCheck = false,
                                Text = "ANTECEDENTES GINECO-OBSTÉTRICOS",
                                Value = HistClinica,
                                Node = nodeX,
                                IsNodeExpanded = true
                            });
                        }
                        else
                            LimpiaValidacionesMujeres();
                    };

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "PADECIMIENTO ACTUAL",
                        Value = HistClinica,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "APARATOS Y SISTEMAS",
                        Value = HistClinica,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "EXPLORACIÓN FÍSICA",
                        Value = HistClinica,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    if (SelectIngreso != null && SelectIngreso.IMPUTADO != null)
                    {
                        if (new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA) >= 65)
                        {
                            LimpiarValidacionesAdultosMayores();
                            ValidacionesAdultosMayores();
                            ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                            {
                                IsRoot = false,
                                IsCheck = false,
                                Text = "MAYORES DE 65 AÑOS",
                                Value = HistClinica,
                                Node = nodeX,
                                IsNodeExpanded = true
                            });
                        }
                        else
                            LimpiarValidacionesAdultosMayores();
                    }
                    else
                        LimpiarValidacionesAdultosMayores();

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "CONSIDERACIONES FINALES",
                        Value = HistClinica,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "CONCLUSIONES",
                        Value = HistClinica,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });


                    TreeList = new List<Clases.TreeViewList>();

                    ItemTreeRaiz.IsRoot = true;
                    ItemTreeRaiz.IsCheck = false;
                    ItemTreeRaiz.IsNodeExpanded = true;
                    ItemTreeRaiz.Text = "HISTORIA CLÍNICA";
                    ItemTreeRaiz.Node = ItemTreeIngreso;
                    TreeList.Add(ItemTreeRaiz);
                    RaisePropertyChanged("TreeList");

                    DatosIngresoVisible = true;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void ArbolViewDental()
        {
            try
            {
                INGRESO Ingre = SelectIngreso;

                if (Ingre != null)
                {
                    var _validacionHistoriaClinica = new cHistoriaClinica().GetData(x => x.ID_IMPUTADO == Ingre.ID_IMPUTADO && x.ID_INGRESO == Ingre.ID_INGRESO && x.ID_CENTRO == Ingre.ID_CENTRO && x.ID_ANIO == Ingre.ID_ANIO).FirstOrDefault();

                    if (_validacionHistoriaClinica == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validaación!", "Es necesario capturar historia clínica médica al imputado antes de continuar.");
                        return;
                    };

                    HISTORIA_CLINICA_DENTAL HistClinicaDental = new cHistoriaClinicaDental().GetData(x => x.ID_INGRESO == Ingre.ID_INGRESO && x.ID_IMPUTADO == Ingre.ID_IMPUTADO && x.ID_ANIO == Ingre.ID_ANIO && x.ID_CENTRO == Ingre.ID_CENTRO).FirstOrDefault();

                    var TreeNodeHeredoFamDental = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodePatosDental = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeInterrogatorio = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeBucoDental = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeDientes = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeArticulacionTempDental = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeEncias = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeBruxismo = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeSignosVitales = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeOdontogramaInicial = new List<ControlPenales.Clases.TreeViewList>();
                    var TreeNodeOdontogramaSeguimiento = new List<ControlPenales.Clases.TreeViewList>();
                    var ItemTreeRaiz = new ControlPenales.Clases.TreeViewList();
                    var ItemTreeIngreso = new List<ControlPenales.Clases.TreeViewList>();
                    var ItemTreeCausaPenal = new List<ControlPenales.Clases.TreeViewList>();


                    List<ControlPenales.Clases.TreeViewList> nodeX = null;
                    if (ItemTreeCausaPenal != null)
                        if (ItemTreeCausaPenal.Count > 0)
                            nodeX = ItemTreeCausaPenal;

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "ANTECEDENTES HEREDO-FAMILIARES (DENTAL)",
                        Value = HistClinicaDental,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "ANTECEDENTES PERSONALES PATOLÓGICOS (DENTAL)",
                        Value = HistClinicaDental,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "INTERROGATORIO",
                        Value = HistClinicaDental,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "EXPLORACIÓN BUCODENTAL",
                        Value = HistClinicaDental,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "DIENTES",
                        Value = HistClinicaDental,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "ARTICULACIÓN TEMPOROMANDIBULAR",
                        Value = HistClinicaDental,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "ENCÍAS",
                        Value = HistClinicaDental,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "BRUXISMO",
                        Value = HistClinicaDental,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "SIGNOS VITALES",
                        Value = HistClinicaDental,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    {
                        IsRoot = false,
                        IsCheck = false,
                        Text = "ODONTOGRAMA INICIAL",
                        Value = HistClinicaDental,
                        Node = nodeX,
                        IsNodeExpanded = true
                    });

                    //ItemTreeIngreso.Add(new ControlPenales.Clases.TreeViewList
                    //{
                    //    IsRoot = false,
                    //    IsCheck = false,
                    //    Text = "ODONTOGRAMA DE SEGUIMIENTO",
                    //    Value = HistClinicaDental,
                    //    Node = nodeX,
                    //    IsNodeExpanded = true
                    //});

                    if (TreeList == null)
                        TreeList = new List<Clases.TreeViewList>();

                    ItemTreeRaiz.IsRoot = true;
                    ItemTreeRaiz.IsCheck = false;
                    ItemTreeRaiz.IsNodeExpanded = true;
                    ItemTreeRaiz.Text = "HISTORIA CLÍNICA";
                    ItemTreeRaiz.Node = ItemTreeIngreso;
                    TreeList.Add(ItemTreeRaiz);
                    OnPropertyChanged("TreeList");

                    DatosIngresoVisible = true;
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion

        #region Procesamiento de Datos Individuales
        private void SeleccionaIngresoArbol(Object obj)
        {
            try
            {
                if (obj == null)
                    return;

                var arbol = (TreeView)obj;
                var x = (ControlPenales.Clases.TreeViewList)arbol.SelectedValue;

                if (x == null)
                    return;

                if (x.IsRoot)//es la raiz
                {
                    //IngresosVisible = true;
                    //DatosIngresoVisible = false;
                }
                else
                {
                    #region DENTAL

                    if (x.Text.Equals("ANTECEDENTES HEREDO-FAMILIARES (DENTAL)"))
                    {
                        //limpia lo medico para asegurarse que no exista algun cambio pendiente.
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;

                        //limpia las otras ventanas que no corresponden ala que elegi
                        IsEnabledPatologicosDental = IsEnabledOdontoSeguimientoDental = IsEnabledInterrogatorioDental = IsEnabledExploracionBucoDental = IsEnabledDientesDental = IsEnabledArticulacionDental = IsEnabledEnciasDental = IsEnabledBruxismoDental = IsEnabledSignosVitalesDental = IsEnabledOdontoInicialDental = false;

                        SelectTab = (short)enumHistoriaClinica.HEREDO_FAMILIARES_DENTAL;
                        IsEnabledHeredoFamiliaresDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("ANTECEDENTES PERSONALES PATOLÓGICOS (DENTAL)"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;

                        IsEnabledHeredoFamiliaresDental = IsEnabledOdontoSeguimientoDental = IsEnabledInterrogatorioDental = IsEnabledExploracionBucoDental = IsEnabledDientesDental = IsEnabledArticulacionDental = IsEnabledEnciasDental = IsEnabledBruxismoDental = IsEnabledSignosVitalesDental = IsEnabledOdontoInicialDental = false;

                        SelectTab = (short)enumHistoriaClinica.PATOLOGICOS_DENTAL;
                        IsEnabledPatologicosDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("INTERROGATORIO"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;

                        IsEnabledPatologicosDental = IsEnabledOdontoSeguimientoDental = IsEnabledHeredoFamiliaresDental = IsEnabledExploracionBucoDental = IsEnabledDientesDental = IsEnabledArticulacionDental = IsEnabledEnciasDental = IsEnabledBruxismoDental = IsEnabledSignosVitalesDental = IsEnabledOdontoInicialDental = false;

                        SelectTab = (short)enumHistoriaClinica.INTERROGATORIO;
                        IsEnabledInterrogatorioDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("EXPLORACIÓN BUCODENTAL"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;

                        IsEnabledHeredoFamiliaresDental = IsEnabledOdontoSeguimientoDental = IsEnabledPatologicosDental = IsEnabledInterrogatorioDental = IsEnabledDientesDental = IsEnabledArticulacionDental = IsEnabledEnciasDental = IsEnabledBruxismoDental = IsEnabledSignosVitalesDental = IsEnabledOdontoInicialDental = false;

                        SelectTab = (short)enumHistoriaClinica.EPLORACION_BUCODENTAL;
                        IsEnabledExploracionBucoDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("DIENTES"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;

                        IsEnabledHeredoFamiliaresDental = IsEnabledOdontoSeguimientoDental = IsEnabledPatologicosDental = IsEnabledInterrogatorioDental = IsEnabledExploracionBucoDental = IsEnabledArticulacionDental = IsEnabledEnciasDental = IsEnabledBruxismoDental = IsEnabledSignosVitalesDental = IsEnabledOdontoInicialDental = false;

                        SelectTab = (short)enumHistoriaClinica.DIENTES;
                        IsEnabledDientesDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("ARTICULACIÓN TEMPOROMANDIBULAR"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;

                        IsEnabledHeredoFamiliaresDental = IsEnabledOdontoSeguimientoDental = IsEnabledPatologicosDental = IsEnabledInterrogatorioDental = IsEnabledExploracionBucoDental = IsEnabledDientesDental = IsEnabledEnciasDental = IsEnabledBruxismoDental = IsEnabledSignosVitalesDental = IsEnabledOdontoInicialDental = false;

                        SelectTab = (short)enumHistoriaClinica.ARTICULACION_TERMO;
                        IsEnabledArticulacionDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("ENCÍAS"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;

                        IsEnabledHeredoFamiliaresDental = IsEnabledOdontoSeguimientoDental = IsEnabledPatologicosDental = IsEnabledInterrogatorioDental = IsEnabledExploracionBucoDental = IsEnabledDientesDental = IsEnabledBruxismoDental = IsEnabledSignosVitalesDental = IsEnabledOdontoInicialDental = false;

                        SelectTab = (short)enumHistoriaClinica.ENCIAS;
                        IsEnabledEnciasDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("BRUXISMO"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;

                        IsEnabledHeredoFamiliaresDental = IsEnabledOdontoSeguimientoDental = IsEnabledPatologicosDental = IsEnabledInterrogatorioDental = IsEnabledExploracionBucoDental = IsEnabledDientesDental = IsEnabledArticulacionDental = IsEnabledEnciasDental = IsEnabledSignosVitalesDental = IsEnabledOdontoInicialDental = false;

                        SelectTab = (short)enumHistoriaClinica.BRUXISMO;
                        IsEnabledBruxismoDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("SIGNOS VITALES"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;

                        IsEnabledHeredoFamiliaresDental = IsEnabledOdontoSeguimientoDental = IsEnabledPatologicosDental = IsEnabledInterrogatorioDental = IsEnabledExploracionBucoDental = IsEnabledDientesDental = IsEnabledArticulacionDental = IsEnabledEnciasDental = IsEnabledBruxismoDental = IsEnabledOdontoInicialDental = false;

                        SelectTab = (short)enumHistoriaClinica.SIGNOS_VITALES;
                        IsEnabledSignosVitalesDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("ODONTOGRAMA INICIAL"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = IsEnabledSignosVitalesDental = false;

                        IsEnabledHeredoFamiliaresDental = IsEnabledOdontoSeguimientoDental = IsEnabledPatologicosDental = IsEnabledInterrogatorioDental = IsEnabledExploracionBucoDental = IsEnabledDientesDental = IsEnabledArticulacionDental = IsEnabledEnciasDental = IsEnabledBruxismoDental = false;

                        SelectTab = (short)enumHistoriaClinica.ODONTOGRAMA_INICIAL;
                        IsEnabledOdontoInicialDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("ODONTOGRAMA DE SEGUIMIENTO"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = IsEnabledSignosVitalesDental = false;

                        IsEnabledHeredoFamiliaresDental = IsEnabledOdontoInicialDental = IsEnabledPatologicosDental = IsEnabledInterrogatorioDental = IsEnabledExploracionBucoDental = IsEnabledDientesDental = IsEnabledArticulacionDental = IsEnabledEnciasDental = IsEnabledBruxismoDental = false;

                        SelectTab = (short)enumHistoriaClinica.ODONTOGRAMA_SEGUIMIENTO;
                        IsEnabledOdontoSeguimientoDental = true;
                        SelctedHistoriaClinicaDental = (HISTORIA_CLINICA_DENTAL)x.Value;
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    #endregion
                    if (x.Text.Equals("FICHA DE IDENTIFICACIÓN"))
                    {
                        IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;

                        SelectTab = 0;
                        IsEnabledTabExamen = true;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;
                    }


                    if (x.Text.Equals("ANTECEDENTES HEREDO-FAMILIARES"))
                    {
                        IsEnabledTabExamen = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;
                        SelectTab = 1;
                        IsEnabledTabHF = true;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("ANTECEDENTES PERSONALES NO PATOLÓGICOS"))
                    {
                        IsEnabledTabExamen = IsEnabledTabHF = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;
                        IsEnabledTabANP = true;
                        SelectTab = 2;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;

                    }

                    if (x.Text.Equals("ANTECEDENTES PERSONALES PATOLÓGICOS"))
                    {
                        IsEnabledTabExamen = IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;
                        SelectTab = 3;
                        IsEnabledTabAP = true;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;

                    }

                    if (x.Text.Equals("ANTECEDENTES GINECO-OBSTÉTRICOS"))
                    {
                        IsEnabledTabExamen = IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;
                        SelectTab = 4;
                        IsEnabledTabMujeres = true;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;

                    }

                    if (x.Text.Equals("PADECIMIENTO ACTUAL"))
                    {
                        IsEnabledTabExamen = IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;
                        SelectTab = 5;
                        IsEnabledTabPadAct = true;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("APARATOS Y SISTEMAS"))
                    {
                        IsEnabledTabExamen = IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;
                        SelectTab = 6;
                        IsEnabledTabAparatosSist = true;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;

                    }

                    if (x.Text.Equals("EXPLORACIÓN FÍSICA"))
                    {
                        IsEnabledTabExamen = IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;
                        SelectTab = 7;
                        IsENabledTabExplorFisica = true;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;

                    }

                    if (x.Text.Equals("MAYORES DE 65 AÑOS"))
                    {
                        IsEnabledTabExamen = IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTabConsideracionesFinales = IsEnabledTabConclusiones = false;
                        SelectTab = 8;
                        IsEnabledTab65Mas = true;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;

                    }

                    if (x.Text.Equals("CONSIDERACIONES FINALES"))
                    {
                        IsEnabledTabExamen = IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConclusiones = false;
                        SelectTab = 9;
                        IsEnabledTabConsideracionesFinales = true;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;
                    }

                    if (x.Text.Equals("CONCLUSIONES"))
                    {
                        IsEnabledTabExamen = IsEnabledTabHF = IsEnabledTabANP = IsEnabledTabAP = IsEnabledTabMujeres = IsEnabledTabPadAct = IsEnabledTabAparatosSist = IsENabledTabExplorFisica = IsEnabledTab65Mas = IsEnabledTabConsideracionesFinales = false;
                        SelectTab = 10;
                        IsEnabledTabConclusiones = true;
                        SelectedHistoriaC = (HISTORIA_CLINICA)x.Value;

                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion

        #region Complementarios
        private string CalcularSentencia()
        {
            try
            {
                LstSentenciasIngresos = new ObservableCollection<SentenciaIngreso>();
                if (SelectIngreso != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (SelectIngreso.CAUSA_PENAL != null)
                    {
                        foreach (var cp in SelectIngreso.CAUSA_PENAL)
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

        ///METODO VIEJO
        //private string CalcularSentencia(ICollection<CAUSA_PENAL> CausaPenal)
        //{
        //    try
        //    {
        //        if (CausaPenal != null)
        //        {
        //            int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
        //            DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
        //            if (CausaPenal != null)
        //            {
        //                foreach (var cp in CausaPenal)
        //                {
        //                    var segundaInstancia = false;
        //                    if (cp.SENTENCIA != null && cp.SENTENCIA.Count > 0)
        //                    {
        //                        if (cp.RECURSO.Any() && cp.RECURSO.Count > 0)
        //                        {
        //                            var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
        //                            if (r.Any() && r != null)
        //                            {
        //                                var res = r.FirstOrDefault();
        //                                if (res != null)
        //                                {
        //                                    //SENTENCIA
        //                                    anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
        //                                    meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
        //                                    dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);

        //                                    segundaInstancia = true;
        //                                }
        //                            }
        //                        }
        //                        var s = cp.SENTENCIA.FirstOrDefault();
        //                        if (s != null)
        //                        {
        //                            if (FechaInicioCompurgacion == null)
        //                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
        //                            else
        //                                if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
        //                                    FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;

        //                            //SENTENCIA
        //                            if (!segundaInstancia)
        //                            {
        //                                anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
        //                                meses = meses + (s.MESES != null ? s.MESES.Value : 0);
        //                                dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);
        //                            }

        //                            //ABONO
        //                            anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
        //                            meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
        //                            dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
        //                        }
        //                    }
        //                }
        //            }

        //            while (dias > 29)
        //            {
        //                meses++;
        //                dias = dias - 30;
        //            }
        //            while (meses > 11)
        //            {
        //                anios++;
        //                meses = meses - 12;
        //            }

        //            if (FechaInicioCompurgacion != null)
        //            {
        //                FechaFinCompurgacion = FechaInicioCompurgacion;
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
        //                //
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
        //                FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

        //                int a = 0, m = 0, d = 0;
        //                new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
        //                a = m = d = 0;
        //                new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);

        //                return a + (a == 1 ? " AÑO " : " AÑOS ") + m + (m == 1 ? " MES " : " MESES ") + d + (d == 1 ? " DÍA " : " DÍAS ");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
        //    }
        //    return string.Empty;
        //}

        #endregion

        #region Scanner
        private void ObtenerTipoDocumento()
        {
            try
            {
                DocumentoDigitalizado = null;
                ObservacionDocumento = string.Empty;
                DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                listTipoDocumento = new ObservableCollection<HC_DOCUMENTO_TIPO>(new cTipoDocumentosHistoriaClinica().GetData());
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los tipos de documento.", ex);
            }

        }

        //private async void Scan(PdfViewer obj)
        //{
        //    try
        //    {
        //        await Task.Factory.StartNew(async () =>
        //        {
        //            bScan = true;
        //            bScanAislado = false;
        //            await escaner.Scann(Duplex, SelectedSource, obj);

        //        });
        //        PopUpsViewModels.EnabledMenu = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al escanear el documento.", ex);
        //    }
        //}


        private async void ScanAislado(PdfViewer obj)
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    bScan = false;
                    bScanAislado = true;
                    await escaner.Scann(Duplex, SelectedSource, obj);
                });
                PopUpsViewModels.EnabledMenu = true;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al escanear el documento.", ex);
            }
        }

        private void GuardarDocumentoAislado()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    escaner.Hide();
                    if (DocumentoDigitalizado == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice un documento para guardar");
                        return;
                    }
                    if (DocumentoDigitalizado.Length <= 0)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice al menos un documento para guardar");
                        return;
                    }

                    Documento = new byte[DocumentoDigitalizado.Count()];
                    DocumentoDigitalizado.CopyTo(Documento, 0);

                    if (LstDocumentosActuales == null)
                        LstDocumentosActuales = new ObservableCollection<HISTORIA_CLINICA_DOCUMENTO>();

                    var _detalleArchivo = new SSP.Controlador.Catalogo.Justicia.cTipoDocumentosHistoriaClinica().GetData(x => x.ID_DOCTO == TipoDocto).FirstOrDefault();

                    LstDocumentosActuales.Add(new HISTORIA_CLINICA_DOCUMENTO
                        {
                            DOCUMENTO = DocumentoDigitalizado,
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_DOCTO = TipoDocto,
                            HC_DOCUMENTO_TIPO = _detalleArchivo,
                            FISICO = (short)eFisicoDigital.DIGITAL,
                            ID_FORMATO = (short)eFormatosDigitalizacion.PDF
                        });

                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        StaticSourcesViewModel.Mensaje("Digitalización", "Documento guardado exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                    }));

                    if (AutoGuardado)
                        escaner.Show();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al digitalizar documento.", ex);
            }
        }

        private void GuardarDocumento()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    escaner.Hide();
                    if (DocumentoDigitalizado == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice un documento para guardar");
                        return;
                    }
                    if (DocumentoDigitalizado.Length <= 0)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice al menos un documento para guardar");
                        return;
                    }
                    Documento = new byte[DocumentoDigitalizado.Count()];
                    DocumentoDigitalizado.CopyTo(Documento, 0);
                    IdTipoDocumento = _Directo == true ? (int)eTipoDocumentoHistoriaClinica.DIRECTO : _ConstanciaDoc == true ? (int)eTipoDocumentoHistoriaClinica.CONSTANCIA_DOCUMENTAL : new int();
                    IdTipoFormatoDocumento = Parametro.ID_FORMATO_IMAGEN;
                    if (LstDocumentos == null)
                        LstDocumentos = new ObservableCollection<HISTORIA_CLINICA_DOCUMENTO>();

                    LstDocumentos.Add(new HISTORIA_CLINICA_DOCUMENTO
                        {
                            DOCUMENTO = Documento,
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_DOCTO = IdTipoDocumento,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            FISICO = _ConstanciaDoc ? (short)eFisico.SI : (short)eFisico.NO
                        });

                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        StaticSourcesViewModel.Mensaje("Digitalización", "Documento guardado exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                    }));

                    if (AutoGuardado)
                        escaner.Show();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al digitalizar documento.", ex);
            }
        }

        private async void AbrirDocumento(PdfViewer obj)
        {
            try
            {
                if (Documento == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "NO HA SIDO DIGITALIZADO UN DOCUMENTO!");
                    return;
                }

                if (IdTipoFormatoDocumento != Parametro.ID_FORMATO_IMAGEN)
                {
                    new Dialogos().ConfirmacionDialogo("FORMATO DE DOCUMENTO!", "EL DOCUMENTO ESTA EN FORMATO DOCX. NO PUEDE SER MOSTRADO EN EL VISOR DEL ESCANER");
                    return;
                }

                documentoDigitalizado = new byte[Documento.Count()];
                Documento.CopyTo(documentoDigitalizado, 0);


                if (DocumentoDigitalizado == null)
                    return;

                await Task.Factory.StartNew(() =>
                {
                    var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                    File.WriteAllBytes(fileNamepdf, DocumentoDigitalizado);
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        obj.LoadFile(fileNamepdf);
                        obj.Visibility = Visibility.Visible;
                    }));
                });


            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir documento.", ex);
            }
        }

        #endregion

        #region Acciones en botones de agregar y quitar
        private void AgregarPatologicos(string Observaciones = "")
        {
            try
            {
                if (SelectIngreso != null && SelectedPatologico != null)
                {
                    HISTORIA_CLINICA_PATOLOGICOS _temporal = SelectedPatologico;
                    if (LstCondensadoPatologicos == null)
                        LstCondensadoPatologicos = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>();

                    if (!LstCondensadoPatologicos.Any(a => a.ID_PATOLOGICO == SelectedPatologico.ID_PATOLOGICO))
                    {
                        if (LstPatologicos.Remove(SelectedPatologico))
                        {
                            LstCondensadoPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS
                                {
                                    PATOLOGICO_CAT = _temporal.PATOLOGICO_CAT,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    ID_PATOLOGICO = _temporal.ID_PATOLOGICO,
                                    MOMENTO_DETECCION = "EI",
                                    OTROS_DESCRIPCION = string.Empty,
                                    RECUPERADO = string.Empty,
                                    REGISTRO_FEC = Fechas.GetFechaDateServer,
                                    OBSERVACIONES = Observaciones,
                                    FUENTE = _temporal.FUENTE
                                });
                        };
                    }
                    else
                    {

                        if (LstCondensadoPatologicos.Remove(SelectedPatologico))
                        {
                            LstCondensadoPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS
                            {
                                PATOLOGICO_CAT = _temporal.PATOLOGICO_CAT,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_PATOLOGICO = _temporal.ID_PATOLOGICO,
                                MOMENTO_DETECCION = "EI",
                                OTROS_DESCRIPCION = string.Empty,
                                RECUPERADO = string.Empty,
                                REGISTRO_FEC = Fechas.GetFechaDateServer,
                                OBSERVACIONES = Observaciones
                            });
                        };
                    }
                    if (_temporal.PATOLOGICO_CAT.SECTOR_CLASIFICACION.Any())
                        if (LstSectoresVulnerbles != null && LstSectoresVulnerbles.Count > 0 && _temporal.PATOLOGICO_CAT.SECTOR_CLASIFICACION != null & _temporal.PATOLOGICO_CAT.SECTOR_CLASIFICACION.Count > 0)
                        {
                            foreach (var _sector_clasif in _temporal.PATOLOGICO_CAT.SECTOR_CLASIFICACION)
                            {
                                if (LstSectoresVulnerbles.Any(a => a.ID_SECTOR_CLAS == _sector_clasif.ID_SECTOR_CLAS && (!a.ES_GRUPO_VULNERABLE.HasValue || a.ES_GRUPO_VULNERABLE.HasValue && a.ES_GRUPO_VULNERABLE.Value != 1)))
                                    LstSectoresVulnerbles.First(a => a.ID_SECTOR_CLAS == _sector_clasif.ID_SECTOR_CLAS).ES_GRUPO_VULNERABLE = 1;
                            }
                            LstSectoresVulnerbles = new ObservableCollection<SECTOR_CLASIFICACION>(LstSectoresVulnerbles);
                        }
                    SelectedPatologico = _temporal = null;
                }
                else
                    new Dialogos().ConfirmacionDialogo("VALIDACIÓN!", "FAVOR DE SELECCIONAR UN ANTECEDENTE PATOLÓGICO.");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("ERROR!", "OCURRIÓ UN ERROR AL AGREGAR ELANTECEDENTE PATOLÓGICO.", ex);
            }
        }

        private void QuitarPatologico()
        {
            try
            {
                if (LstCondensadoPatologicos == null)
                    return;

                if (LstPatologicos == null)
                    return;

                if (SelectedCondensadoPato != null)
                {
                    if (SelectedCondensadoPato.FUENTE == "X")
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "El antecedente no se puede retirar.");
                        return;
                    }

                    HISTORIA_CLINICA_PATOLOGICOS _temporal = SelectedCondensadoPato;
                    var _temporalInverso = SelectedCondensadoPato;
                    if (LstCondensadoPatologicos.Remove(SelectedCondensadoPato))
                    {
                        LstPatologicos.Add(new HISTORIA_CLINICA_PATOLOGICOS
                            {
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                MOMENTO_DETECCION = "EI",
                                OTROS_DESCRIPCION = _temporalInverso.OTROS_DESCRIPCION,
                                RECUPERADO = _temporalInverso.RECUPERADO,
                                REGISTRO_FEC = _temporalInverso.REGISTRO_FEC,
                                OBSERVACIONES = _temporalInverso.OBSERVACIONES,
                                PATOLOGICO_CAT = _temporalInverso.PATOLOGICO_CAT,
                                ID_CONSEC = _temporalInverso.ID_CONSEC,
                                ID_PATOLOGICO = _temporalInverso.ID_PATOLOGICO,
                                FUENTE = _temporalInverso.FUENTE,
                                ID_NOPATOLOGICO = _temporalInverso.ID_NOPATOLOGICO
                            });
                    };
                    if (LstSectoresVulnerbles != null && LstSectoresVulnerbles.Count > 0 && _temporal.PATOLOGICO_CAT.SECTOR_CLASIFICACION != null &&
                        _temporal.PATOLOGICO_CAT.SECTOR_CLASIFICACION.Count > 0)
                    {
                        foreach (var _sector_clasif in _temporal.PATOLOGICO_CAT.SECTOR_CLASIFICACION)
                        {
                            if (!LstCondensadoPatologicos.Any(a => a.PATOLOGICO_CAT.SECTOR_CLASIFICACION.Any(a2 => a2.ID_SECTOR_CLAS == _sector_clasif.ID_SECTOR_CLAS))
                                && LstSectoresVulnerbles.Any(a => a.ID_SECTOR_CLAS == _sector_clasif.ID_SECTOR_CLAS && a.ES_GRUPO_VULNERABLE == 1))
                            {
                                LstSectoresVulnerbles.First(w => w.ID_SECTOR_CLAS == _sector_clasif.ID_SECTOR_CLAS).ES_GRUPO_VULNERABLE = 0;
                                LstSectoresVulnerbles = new ObservableCollection<SECTOR_CLASIFICACION>(LstSectoresVulnerbles);
                            }
                        }
                    }
                    SelectedCondensadoPato = _temporalInverso = null;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("ERROR!", "OCURRIÓ UN ERROR AL AGREGAR ELANTECEDENTE PATOLÓGICO.", ex);
            }
        }

        #endregion

        #region ATTACHMENT DE ARCHIVOS

        private void SeleccionaImagenDental()
        {
            try
            {
                if (IdTipoImagenDental == null || IdTipoImagenDental == -1)
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "Seleccione la categoría a contemplar!");
                    return;
                }

                if (LstDocumentosDentales == null)
                    LstDocumentosDentales = new ObservableCollection<HISTORIA_CLINICA_DENTAL_DOCUME>();

                var op = new System.Windows.Forms.OpenFileDialog();
                op.Title = "Seleccione una imagen";
                string formatos = string.Empty;
                var _formatosPermitidos = Parametro.ID_FORMATO_IMAGENES;
                if (_formatosPermitidos != null)
                    if (_formatosPermitidos.Any())
                        foreach (var item in _formatosPermitidos)
                        {
                            short _dato = short.Parse(item);
                            var _detalleArchivo = new cFormatoDocumento().GetData(x => x.ID_FORMATO == _dato).FirstOrDefault();
                            if (_detalleArchivo != null)
                                formatos += string.Format("Archivo Tipo {0}  (*{1})|*{2}|", !string.IsNullOrEmpty(_detalleArchivo.DESCR) ? _detalleArchivo.DESCR.Replace('.', ' ') : string.Empty, _detalleArchivo.DESCR.ToLower(), _detalleArchivo.DESCR.ToLower());
                        };

                op.Filter = !string.IsNullOrEmpty(formatos) ? formatos.TrimEnd('|') : string.Empty;
                if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    if (new System.IO.FileInfo(op.FileName).Length > Parametro.MAXIMO_BYTES_IMAGENES)
                        StaticSourcesViewModel.Mensaje("Imagen no soportada", "El archivo debe ser de máximo" + Parametro.MAXIMO_BYTES_IMAGENES.ToString() + "bytes", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                    else
                    {
                        LstDocumentosDentales.Add(new HISTORIA_CLINICA_DENTAL_DOCUME
                            {
                                DOCUMENTO = System.IO.File.ReadAllBytes(op.FileName),
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_DOCTO = IdTipoImagenDental
                            });

                        IdTipoImagenDental = -1;
                    }
            }
            catch (System.IO.IOException exArc)
            {
                if (!string.IsNullOrEmpty(exArc.Message))
                    if (exArc.Message.Contains("The process cannot access the file"))
                        new Dialogos().ConfirmacionDialogo("Validación!", "EL ARCHIVO SELECCIONADO ESTA EN USO, CIERRE EL ARCHIVO PARA INGRESARLO");
                    else
                        new Dialogos().ConfirmacionDialogo("Validación!", exArc.Message.ToUpper());

                return;
            }

            catch (Exception exc)
            {
                new Dialogos().ConfirmacionDialogo("Validación!", !string.IsNullOrEmpty(exc.Message) ? exc.Message.ToUpper() : string.Empty);
                return;
            }
        }
        private void SeleccionarArchivo()
        {
            if (string.IsNullOrEmpty(TipoArchivo) && TipoDocto == null)
            {
                new Dialogos().ConfirmacionDialogo("Validación!", "SELECCIONE EL TIPO DE DOCUMENTO A ANEXAR!");
                return;
            }

            if (TipoDocto == -1)
            {
                new Dialogos().ConfirmacionDialogo("Validación!", "SELECCIONE EL TIPO DE DOCUMENTO A ANEXAR!");
                return;
            }

            var op = new System.Windows.Forms.OpenFileDialog();
            op.Title = "Seleccione un archivo";
            var _ImagenesPermitidasParametro = new short[] { (short)eformatosPermitidos.JPEG, (short)eformatosPermitidos.JPG }; //Parametro.ID_FORMATO_IMAGENES;
            var _DocumentosPermitidosParametros = new short[] { (short)eformatosPermitidos.PDF }; //Parametro.ID_FORMATO_DOCUMENTOS;
            string formatosP = string.Empty;
            string formatosDisponibles = string.Empty;
            string _todosArchivosCatalogo = "Todos los archivos permitidos | ";
            string FormatosImagenes = "Todas las imágenes permitidas |";
            string FormatosDocumentos = "Todos los documentos permitidos |";
            var _archivosCatalogo = new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData(x => x.ID_FORMATO == (short)eformatosPermitidos.JPEG || x.ID_FORMATO == (short)eformatosPermitidos.JPG || x.ID_FORMATO == (short)eformatosPermitidos.PDF);
            if (_archivosCatalogo.Any())
                foreach (var item in _archivosCatalogo)
                    _todosArchivosCatalogo += string.Format("*{0};", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.ToLower() : string.Empty);

            if (_ImagenesPermitidasParametro.Any())
                foreach (var item in _ImagenesPermitidasParametro)
                {
                    var _ImagenIndividual = new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData().FirstOrDefault(x => x.ID_FORMATO == item);
                    if (_ImagenIndividual != null)
                        FormatosImagenes += string.Format("*{0};", !string.IsNullOrEmpty(_ImagenIndividual.DESCR) ? _ImagenIndividual.DESCR.ToLower() : string.Empty);
                };

            if (_DocumentosPermitidosParametros.Any())
                foreach (var item in _DocumentosPermitidosParametros)
                {
                    var _DocumentoPermitido = new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData().FirstOrDefault(x => x.ID_FORMATO == item);
                    if (_DocumentoPermitido != null)
                        FormatosDocumentos += string.Format("*{0};", !string.IsNullOrEmpty(_DocumentoPermitido.DESCR) ? _DocumentoPermitido.DESCR.ToLower() : string.Empty);
                };

            var _archivosPermitidos = new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData(x => x.ID_FORMATO == (short)eformatosPermitidos.JPEG || x.ID_FORMATO == (short)eformatosPermitidos.JPG || x.ID_FORMATO == (short)eformatosPermitidos.PDF);//new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData(x => x.ID_FORMATO != 0);
            if (_archivosPermitidos.Any())
                foreach (var item in _archivosPermitidos)
                    formatosDisponibles += string.Format("Archivo Tipo {0}  (*{1})|*{2}|", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Replace('.', ' ') : string.Empty, item.DESCR.ToLower(), item.DESCR.ToLower());

            string _refinados = !string.IsNullOrEmpty(formatosDisponibles) ? formatosDisponibles.TrimEnd('|') : string.Empty;
            op.Filter = _todosArchivosCatalogo + '|' + FormatosImagenes + '|' + FormatosDocumentos + '|' + _refinados;
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                short? _formato = new short?();

                try
                {
                    if (LstDocumentosActuales == null)
                        LstDocumentosActuales = new ObservableCollection<HISTORIA_CLINICA_DOCUMENTO>();

                    if (!string.IsNullOrEmpty(op.SafeFileName))
                    {
                        string _extension = System.IO.Path.GetExtension(op.SafeFileName).TrimStart('.').ToUpper();
                        var _detalleFormatoElegido = new cFormatoDocumento().GetData(c => c.DESCR.Contains(_extension)).FirstOrDefault();
                        if (_detalleFormatoElegido != null)
                            _formato = _detalleFormatoElegido.ID_FORMATO;
                    };

                    var _detalleArchivo = new SSP.Controlador.Catalogo.Justicia.cTipoDocumentosHistoriaClinica().GetData(x => x.ID_DOCTO == TipoDocto).FirstOrDefault();
                    LstDocumentosActuales.Add(new HISTORIA_CLINICA_DOCUMENTO
                    {
                        DOCUMENTO = System.IO.File.ReadAllBytes(op.FileName),
                        FISICO = (short)eFisicoDigital.DIGITAL,
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        ID_DOCTO = TipoDocto,
                        HC_DOCUMENTO_TIPO = _detalleArchivo,
                        ID_FORMATO = _formato
                    });
                }

                catch (System.IO.IOException exArc)
                {
                    if (!string.IsNullOrEmpty(exArc.Message))
                    {
                        if (exArc.Message.Contains("The process cannot access the file"))
                            new Dialogos().ConfirmacionDialogo("Validación!", "EL ARCHIVO SELECCIONADO ESTA EN USO, GUARDE SU PROGRESO Y CIERRE EL ARCHIVO PARA INGRESARLO");
                        else
                            new Dialogos().ConfirmacionDialogo("Validación!", exArc.Message.ToUpper());
                    };

                    return;
                }

                catch (Exception exc)
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", !string.IsNullOrEmpty(exc.Message) ? exc.Message.ToUpper() : string.Empty);
                    return;
                }
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
/* * * * * * * * * * * * * * * * * * * * * * * * 
HACEN FALTA LOS CAMPOS DE: 
AHF_NOMBRE, CEDULA_PROFESIONAL, DOCTOR, PAG3_NOMBRE, PAG4_NOMBRE, PAG5_NOMBRE, RES_NOMBRE
 * CREO QUE NO SON NECESARIOS PORQUE SE GUARDA EL USUARIO LOGUEADO EN EL CAMPO ID_RESPONSABLE
 * A MENOS QUE ESTE REGISTRO SEA GENERADO POR VARIAS PERSONAS EN DIFERENTES TIEMPOS

  
 
 * TAMBIEN TE DEJO ESTOS PARAMETROS QUE UTILICE PARA LO DE ADUANA

INSERT INTO PARAMETRO_CLAVE (ID_CLAVE,DESCR,SISTEMA)
VALUES ('TIPO_VISITA_POR_CENTRO','Indica si la visita se agenda por apellido o por orden alfabetico. 1 = AP, 2 = ED','J');

INSERT INTO PARAMETRO_CLAVE (ID_CLAVE,DESCR,SISTEMA)
VALUES ('INTERNOS_PERMITIDOS_POR_DIA','Indica cuantos interno puede visitar por dia cada visitante.','J');

INSERT INTO PARAMETRO (ID_CENTRO,ID_CLAVE,VALOR)
VALUES (4,'TIPO_VISITA_POR_CENTRO',1);
INSERT INTO PARAMETRO (ID_CENTRO,ID_CLAVE,VALOR)
VALUES (2,'TIPO_VISITA_POR_CENTRO',2);

INSERT INTO PARAMETRO (ID_CENTRO,ID_CLAVE,VALOR)
VALUES (4,'INTERNOS_PERMITIDOS_POR_DIA',12);
INSERT INTO PARAMETRO (ID_CENTRO,ID_CLAVE,VALOR)
VALUES (2,'INTERNOS_PERMITIDOS_POR_DIA',1);
 * 
/////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 * Observaciones Nota Clínica (revisar los max lenght)
Tab Antecedentes Heredo Familiares, acomodar mejor la distribucion de la pantalla 
los textbox de edad hacerlos mas pequeños
los textbox de hermanos hombres ymujeres hacer los textbox mas chicos

Tab Antecedentes No Patólogicos veo un espacio entre los texbox por ejemplo entre alcoholismo y tabaquismo
,en las demas pantallas no se deja el espacio y se ve diferente

Tab Antecedentes Patológicos
Espacio entre textbox

Tab Mujer
Ver como acomodar mejor el checkbox de menarquía, para no dejar un espacio tan grande en blanco
Checar en el control de datetime para cambiar el "select a date" por "Selecciona fecha"

Tab Exploracion fisica => General
Dejar un espacio para que no queden pegados los textbox con la seccion de tabs
Peso me deja capturar letras y minusculas(solo numeros), no tiene max lenght
Estatura me deja capturar letras minusculas(solo numeros), no tiene max lenght

Tab Exp[loración Fisica => Signos Vitales
Presión arterial no se esta validando el max lenght 
Pulso no se esta validando el max lenght
Respiración no se esta validando el max lenght
Temperatura no se esta validando el max lenght

Tab Mayores 65 años
Disminucion y/o alteración visual solo activar de texto al seleccionar opcion otros, esta capturando en minusciulas,


/////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 * RESPUESTAS
 * 
TAB ANTECEDENTES HEREDO FAMILIARES
 * NO SE ME OCURRIERON IDEAS PARA ESTE TAB, PUSE DOS OPCIONES PARA QUE ME DIGAS CUAL ESTA MEJOR	
TAB ANTECEDENTES NO PAOTOLOGICOS
 * ES A CAUSA DEL WIDTH A BASE DEL LABEL PARA QUE NO SE VAYA ABRIENDO MAS CADA VEZ QUE ESCRIBES,
LO RESOLVI PONIENDO x-0 EN EL ConverterParameter
TAB ANTECEDENTES PAOTOLOGICOS
 * LO MISMO QUE EL ANTERIOR
TAB EXPLORACION FISICA
 * TAB GENERAL
     * LOS CAMPOS PESO Y ESTATURA SON STRINGS SUPONGO PORQUE SE LE PUEDE PONER KG O MTS
     * YA LE PUSE EN MAYUSCULAS
     * CUANTO LE PONGO DE MAX LENGTH? EN LA BD SON VARCHAR2 DE 500
 * TAB SIGNOS VITALES
     * LISTO, MAX LENGTH=20
TAB MAYORES 65
 * LISTO, MAX LENGTH=500 

PD. SE VAN A QUITAR LOS CAMPOS DE NOMBRES, CEDULA Y DOCTOR?
SI ES ASI, SE TENDRA QUE ACTUALIZAR EL MODELO
SI NO, NECESITO SABER SI PONGO UN CAMPO DE TEXTO POR TAB O QUE ROLLO
 * 
 * 
* * * * * * * * * * * * * * * * * * * * * * * * */