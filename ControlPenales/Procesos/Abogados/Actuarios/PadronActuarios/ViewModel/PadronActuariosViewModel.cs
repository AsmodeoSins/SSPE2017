using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia;
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
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class PadronActuariosViewModel : FingerPrintScanner
    {
        public PadronActuariosViewModel() { }

        private async void Load_Window(PadronActuariosView Window)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarDatos);
                escaner.EscaneoCompletado += delegate 
                {
                    DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                    //ObservacionDocumento = string.Empty;
                    DocumentoDigitalizado = escaner.ScannedDocument;
                    if (AutoGuardado)
                        if (DocumentoDigitalizado != null)
                            GuardarDocumento();

                    escaner.Dispose();
                };
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        #region TOMAR FOTOS
        private async void Load_FotoIfeCedula(TomarFotoIFECedulaView Window)
        {
            try
            {
                CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgFrente, Window.ImgFrenteCedula, Window.ImgReverso, Window.ImgReversoCedula });

                if (SelectPersona != null)
                {
                    CamaraWeb.AgregarImagenControl(Window.ImgFrente, new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.IFE_FRONTAL));
                    CamaraWeb.AgregarImagenControl(Window.ImgReverso, new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.IFE_REVERSO));
                    CamaraWeb.AgregarImagenControl(Window.ImgFrenteCedula, new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.CEDULA_FRONTAL));
                    CamaraWeb.AgregarImagenControl(Window.ImgReversoCedula, new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.CEDULA_REVERSO));
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de fotografía.", ex);
            }
        }
        private async void TomarFotoLoad(TomarFotoSenaParticularView Window = null)
        {
            try
            {
                if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
                if (ImagenAbogado.Length != 1882)
                    CamaraWeb.AgregarImagenControl(Window.ImgSenaParticular, new Imagenes().ConvertByteToImageSource(ImagenAbogado));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla para tomar foto.", ex);
            }
        }
        private async void TomarFoto(System.Windows.Controls.Image Picture)
        {
            try
            {
                if (Processing)
                    return;
                Processing = true;
                ImagesToSave = ImagesToSave ?? new List<ImageSourceToSave>();
                if (CamaraWeb.ImageControls.Where(w => w.Name == Picture.Name).Any())
                {
                    Picture.Source = CamaraWeb.TomarFoto(Picture);
                    ImagesToSave.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (BitmapSource)Picture.Source });
                    StaticSourcesViewModel.Mensaje(Picture.Name.Contains("ImgFrente") ? "FOTO DE FRENTE" :
                        Picture.Name.Contains("ImgReverso") ? "FOTO TRASERA" : "FOTO", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "ESTA SEGÚRO QUE DESEA CAMBIAR LA FOTO " +
                    (Picture.Name.Contains("ImgFrente") ? "FOTO DE FRENTE" : Picture.Name.Contains("ImgReverso") ? "FOTO TRASERA" : "") + "?") == 1)
                    {
                        CamaraWeb.QuitarFoto(Picture);
                        ImagesToSave.Remove(ImagesToSave.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                    }
                }
                Processing = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar la foto.", ex);
            }
        }
        private void OnTakePicture(System.Windows.Controls.Image Picture)
        {
            try
            {
                if (Processing)
                    return;
                Processing = true;
                ImageFrontal = ImageFrontal ?? new List<ImageSourceToSave>();
                if (CamaraWeb.ImageControls.Where(w => w.Name == Picture.Name).Any())
                {
                    Picture.Source = CamaraWeb.TomarFoto(Picture);
                    ImageFrontal.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (BitmapSource)Picture.Source });
                    StaticSourcesViewModel.Mensaje("FOTO DE FRENTE", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    CamaraWeb.QuitarFoto(Picture);
                    ImageFrontal.Remove(ImageFrontal.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                }
                if (ImageFrontal != null ? ImageFrontal.Count == 1 : false)
                    BotonTomarFotoEnabled = true;
                else
                    BotonTomarFotoEnabled = false;

                Processing = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar la foto.", ex);
            }
        }
        private void OpenSetting(string obj)
        {
            CamaraWeb.AdvanceSetting();
        }
        #endregion

        private void CargarDatos()
        {
            try
            {
                ListPais = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos());
                ListTipoDiscapacidad = new ObservableCollection<TIPO_DISCAPACIDAD>(new cTipoDiscapacidad().ObtenerTodos());
                ListJuzgado = new ObservableCollection<JUZGADO>(new cJuzgado().ObtenerTodos());
                var estatus = new cEstatusVisita().ObtenerTodos();

                Lista_Sources = escaner.Sources();
                if (Lista_Sources.Count > 0) SelectedSource = Lista_Sources.Where(w => w.Default).SingleOrDefault();
                HojasMaximo = string.Format("Escaneo permitido: {0} documentos máximo.", escaner.HojasMaximo);

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListEstatus = new ObservableCollection<ESTATUS_VISITA>();
                    foreach (var item in Parametro.ESTATUS_ABOGADOS)
                    {
                        var x = short.Parse(item);
                        if (estatus.Where(w => w.ID_ESTATUS_VISITA == x).Any())
                        {
                            ListEstatus.Add(estatus.Where(w => w.ID_ESTATUS_VISITA == x).FirstOrDefault());
                        }
                    }
                    ListEstatus.Insert(0, new ESTATUS_VISITA { ID_ESTATUS_VISITA = -1, DESCR = "SELECCIONE" });
                    ListPais.Insert(0, new PAIS_NACIONALIDAD { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
                    ListJuzgado.Insert(0, new JUZGADO { ID_JUZGADO = -1, DESCR = "SELECCIONE" });
                    SelectJuzgado = SelectEstatusVisita = -1;
                    SelectDiscapacitado = string.Empty;
                    SelectPais = Parametro.PAIS; //82;
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }
        
        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.PADRON_ACTUARIOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = MenuGuardarEnabled = true;
                        if (p.EDITAR == 1)
                            PEditar = MenuGuardarEnabled = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = true;
                        if (p.IMPRIMIR == 1)
                            PImprimir = true;
                        if (!PInsertar)//a este nivel para no interferir con el valor que ya tiene establecido
                            MenuInsertarEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
        
        private bool HasErrors() { return base.HasErrors; }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                #region MENU
                case "guardar_menu":
                    try
                    {
                        if(SelectPersona == null)
                            if (!Insertable)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Debes registrar un nuevo actuario o seleccionear uno existente para modificarlo.");
                                break;
                            }
                        if (HasErrors())
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El campo " + base.Error + " es requerido.");
                            break;
                        }

                        //await StaticSourcesViewModel.CargarDatosMetodoAsync(GuardarAbogado);
                        GuardarAbogadoNew();
                        //StaticSourcesViewModel.SourceChanged = false;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar .", ex);
                    }

                    break;
                case "nuevo_menu":
                    break;
                case "insertar_menu":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("Validación",
                            "Existen cambios sin guardar, esta seguro que desea insertar a uno nuevo actuario?") != 1)
                            return;
                    }
                    SetValidaciones();
                    ValidarEnabled = DiscapacitadoEnabled = true;
                    NuevoAbogado = Editable = false;
                    LimpiarCampos();
                    BuscarAbogadoEnabled = true;
                    Insertable = true;
                    SelectEstatusVisita = (short)enumEstatusVisita.AUTORIZADO;
                    SelectDiscapacitado = "N";
                    await OnBuscarPorHuellaInicio();
                    break;
                case "borrar_menu":
                    break;
                case "buscar_menu":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    TextNombre = TextMaterno = TextPaterno = string.Empty;
                    var pers1 = SelectPersona;
                    SelectPersonaAuxiliar = pers1;
                    ImagenPersona = new Imagenes().getImagenPerson();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarPersonasSinCodigo);
                    Editable = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    break;
                case "limpiar_menu":
                    base.ClearRules();
                    ValidarEnabled = NuevoAbogado = Editable = DiscapacitadoEnabled = false;
                    LimpiarCampos();
                    break;
                case "gafete_menu":
                    break;
                case "ayuda_menu":
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                #endregion

                #region FOTOS
                case "tomar_fotos":
                    var aux = ImagenAbogado;
                    ImagenAbogadoAuxiliar = aux;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
                    break;
                case "cancelar_tomar_foto_senas":
                    try
                    {
                        if (ImageFrontal != null ? ImageFrontal.Count == 1 : false)
                        {
                            if (!FotoTomada)
                            {
                                ImageFrontal = null;
                            }
                        }
                        else
                        {
                            if (FotoTomada)
                            {
                                ImageFrontal.Add(new ImageSourceToSave { FrameName = "ImgSenaParticular", ImageCaptured = new Imagenes().ConvertByteToBitmap(ImagenAbogado) });
                                BotonTomarFotoEnabled = true;
                            }
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        if (CamaraWeb != null)
                        {
                            await CamaraWeb.ReleaseVideoDevice();
                            CamaraWeb = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
                    }
                    break;
                case "aceptar_tomar_foto_senas":
                    try
                    {
                        if (ImagenAbogado != new Imagenes().getImagenPerson() && (ImageFrontal != null ? ImageFrontal.Count == 1 : false))
                        {
                            FotoTomada = true;
                            FotoActualizada = Editable ? (ImagenAbogado != new Imagenes().ConvertBitmapToByte(ImageFrontal.FirstOrDefault().ImageCaptured)) : true;
                            ImagenAbogado = new Imagenes().ConvertBitmapToByte(ImageFrontal.FirstOrDefault().ImageCaptured);
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                            if (CamaraWeb != null)
                            {
                                await CamaraWeb.ReleaseVideoDevice();
                                CamaraWeb = null;
                            }
                            break;
                        }
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "DEBES DE TOMAR UNA FOTO.");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
                    }
                    break;
                case "tomar_foto_ine":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.TOMAR_FOTO_IFE_CEDULA);
                    Load_FotoIfeCedula(PopUpsViewModels.MainWindow.TomarFotosIfeCedulaView);
                    break;
                case "salir_tomar_foto_ife_cedula":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.TOMAR_FOTO_IFE_CEDULA);
                    if (CamaraWeb != null)
                    {
                        await CamaraWeb.ReleaseVideoDevice();
                        CamaraWeb = null;
                    }
                    break;
                #endregion

                #region HUELLAS
                case "tomar_huellas":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                    this.ShowIdentification();
                    break;
                #endregion

                #region CREDENCIAL
                case "imprimir_credencial":
                    CrearGafete();
                    break;
                case "print_pvc":
                    try
                    {
                        if (Reporteador.PrintDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            if (GafeteFront)
                                ImpresionGafeteFront = true;
                            if (GafeteBack)
                                ImpresionGafeteBack = true;
                            if (ImpresionGafeteBack && ImpresionGafeteFront)
                            {
                                if (!ImpresionGafete)
                                {
                                    new cAbogado().Actualizar(new ABOGADO
                                    {
                                        ID_ABOGADO = SelectPersona.ID_PERSONA,
                                        //ID_TIPO_ABOGADO = SelectPersona.ABOGADO.ID_TIPO_ABOGADO,
                                        ID_ABOGADO_TITULO = SelectPersona.ABOGADO.ID_ABOGADO_TITULO,
                                        IFE_FRONTAL = SelectPersona.ABOGADO.IFE_FRONTAL,
                                        IFE_REVERSO = SelectPersona.ABOGADO.IFE_REVERSO,
                                        CEDULA = SelectPersona.ABOGADO.CEDULA,
                                        CEDULA_FRONTAL = SelectPersona.ABOGADO.CEDULA_FRONTAL,
                                        CEDULA_REVERSO = SelectPersona.ABOGADO.CEDULA_REVERSO,
                                        NORIGINAL = SelectPersona.ABOGADO.NORIGINAL,
                                        ALTA_FEC = SelectPersona.ABOGADO.ALTA_FEC,
                                        OBSERV = SelectPersona.ABOGADO.OBSERV,
                                        ULTIMA_MOD_FEC = Fechas.GetFechaDateServer,
                                        ID_ESTATUS_VISITA = SelectPersona.ABOGADO.ID_ESTATUS_VISITA,
                                        CJF = SelectPersona.ABOGADO.CJF,
                                        CREDENCIALIZADO = "S",
                                        ID_JUZGADO = SelectPersona.ABOGADO.ID_JUZGADO,
                                        ABOGADO_TITULAR = SelectPersona.ABOGADO.ABOGADO_TITULAR
                                    });
                                    SelectPersona.ABOGADO.CREDENCIALIZADO = "S";
                                    Credencializado = ImpresionGafete = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar la credencialización.", ex);
                    }
                    break;
                #endregion

                #region DIGITALIZAR DOCUMENTOS
                case "digitalizar_documentos":
                    if (SelectPersona != null)
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarListaTipoDocumentoDigitalizado(); });
                    }
                    else
                        await new Dialogos().ConfirmacionDialogoReturn("Advertencia!", "No ha seleccionado un imputado.");
                    DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                    ObservacionDocumento = string.Empty;
                    break;
                case "Cancelar_digitalizar_documentos":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                    escaner.Hide();
                    DocumentoDigitalizado = null;
                    break;
                case "guardar_documento":
                    GuardarDocumento();
                    break;
                #endregion

                #region BUSCAR_ABOGADOS
                case "nueva_busqueda_visitante":
                    TextPaternoAbogado = TextMaternoAbogado = TextNombreAbogado = TextPaterno = TextMaterno = TextNombre = string.Empty;
                    ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    break;
                case "buscar_abogado":
                    //TextNombre = TextNombreAbogado;
                    //TextMaterno = TextMaternoAbogado;
                    //TextPaterno = TextPaternoAbogado;
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    TextNombre = TextMaterno = TextPaterno = string.Empty;
                    var pers2 = SelectPersona;
                    ImagenPersona = new Imagenes().getImagenPerson();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarPersonasSinCodigo);
                    SelectPersonaAuxiliar = pers2;
                    Editable = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    break;
                case "buscar_visitante":
                    if (TextPaterno == null)
                        TextPaterno = string.Empty;
                    if (TextMaterno == null)
                        TextMaterno = string.Empty;
                    if (TextNombre == null)
                        TextNombre = string.Empty;
                    if (TextCodigo == null)
                        TextCodigo = string.Empty;
                    var pers3 = SelectPersona;
                    BuscarPersonas();
                    SelectPersonaAuxiliar = pers3;
                    break;
                case "seleccionar_buscar_persona":
                    try
                    {
                        if (SelectPersona == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a una persona.");
                            break;
                        }
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1)
                            //    return;
                            //else
                            //{
                            //    SelectPersona = SelectPersonaAuxiliar;
                            //}
                            {
                                SelectPersona = SelectPersonaAuxiliar;
                                StaticSourcesViewModel.SourceChanged = false;
                                break;
                            }
                        }
                        if (SelectPersona.ABOGADO != null)
                        {
                            if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO)
                            {
                                NuevoAbogado = false;
                                Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                SetValidaciones();
                                GetDatosPersonaSeleccionada();
                                StaticSourcesViewModel.SourceChanged = false;
                                BuscarAbogadoEnabled = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                Editable = false;
                            }
                            else
                            {
                                if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR)
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Un colaborador no puede convertirse en actuario.");
                                else
                                {
                                    if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "EL ABOGADO SELECCIONADO NO ESTA REGISTRADO COMO ACTUARIO,"
                                            + " DESEA CAMBIARLO AHORA?") == 1)
                                    {
                                        NuevoAbogado = false;
                                        Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                        SetValidaciones();
                                        GetDatosPersonaSeleccionada();
                                        BuscarAbogadoEnabled = true;
                                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                        Editable = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "LA PERSONA SELECCIONADA NO ESTA REGISTRADA COMO ACTUARIO,"
                                    + " DESEA REGISTRARLA AHORA?") == 1)
                            {
                                NuevoAbogado = true;
                                Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                SetValidaciones();
                                GetDatosPersonaSeleccionada();
                                BuscarAbogadoEnabled = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                Editable = false;
                            }
                            else { }
                        }
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
                    }
                    break;
                case "cancelar_buscar_persona":
                    var pers = SelectPersonaAuxiliar;
                    SelectPersona = pers;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    Editable = false;
                    break;
                case "buscar_por_huella":
                    break;
                #endregion

            }
        }

        private async void GuardarAbogadoNew()
        {
            var respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando Actuario", () =>
            {
                try
                {
                    var hoy = Fechas.GetFechaDateServer;
                    var tvl = Parametro.ID_TIPO_VISITA_LEGAL;
                    #region Persona
                    var persona = new SSP.Servidor.PERSONA
                    {
                        CORREO_ELECTRONICO = TextCorreo,
                        ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_ABOGADO,
                        CURP = TextCurp,
                        DOMICILIO_CALLE = TextCalle,
                        DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                        DOMICILIO_NUM_EXT = TextNumExt,
                        DOMICILIO_NUM_INT = TextNumInt,
                        FEC_NACIMIENTO = SelectFechaNacimiento,
                        ID_COLONIA = SelectColonia,
                        ID_ENTIDAD = SelectEntidad,
                        ID_MUNICIPIO = SelectMunicipio,
                        ID_PAIS = SelectPais,
                        ID_TIPO_DISCAPACIDAD = SelectTipoDiscapacidad,
                        IFE = TextIne,
                        MATERNO = TextMaternoAbogado,
                        PATERNO = TextPaternoAbogado,
                        NOMBRE = TextNombreAbogado,
                        NACIONALIDAD = SelectPais,
                        RFC = TextRfc,
                        SEXO = SelectSexo,
                        TELEFONO = TextTelefonoFijo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                        TELEFONO_MOVIL = TextTelefonoMovil.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
                    };
                    #endregion

                    #region Abogado
                    var abogado = new ABOGADO();
                    abogado.ID_ABOGADO_TITULO = "A";
                    if (ImagesToSave != null)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            abogado.IFE_FRONTAL = ImagesToSave.Where(w => w.FrameName == "ImgFrente").Any() ?
                           new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgFrente").FirstOrDefault().ImageCaptured) :
                           null;
                            abogado.IFE_REVERSO = ImagesToSave.Where(w => w.FrameName == "ImgReverso").Any() ?
                                new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgReverso").FirstOrDefault().ImageCaptured) :
                                null;
                            abogado.CEDULA_FRONTAL = ImagesToSave.Where(w => w.FrameName == "ImgFrenteCedula").Any() ?
                                new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgFrenteCedula").FirstOrDefault().ImageCaptured) :
                                null;
                            abogado.CEDULA_REVERSO = ImagesToSave.Where(w => w.FrameName == "ImgReversoCedula").Any() ?
                                new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgReversoCedula").FirstOrDefault().ImageCaptured) :
                                null;
                        }));
                    }
                    abogado.CEDULA = TextCedulaCJF;
                    abogado.OBSERV = TextObservaciones;
                    abogado.ULTIMA_MOD_FEC = hoy;
                    abogado.ID_ESTATUS_VISITA = SelectEstatusVisita;
                    //abogado.ABOGADO_TITULAR = SelectPersona.ABOGADO.ABOGADO_TITULAR;
                    abogado.ID_JUZGADO = SelectJuzgado;
                    abogado.CJF = TextCedulaCJF;
                    //CREDENCIALIZADO = edit ? NuevoAbogado ? "N" : SelectPersona.ABOGADO.CREDENCIALIZADO : "N"
                    #endregion

                    #region Fotos
                    var personaFotos = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                    if (ImageFrontal != null)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            foreach (var item in ImageFrontal)
                            {
                                var encoder = new JpegBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                                encoder.QualityLevel = 100;
                                var bit = new byte[0];
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                                    encoder.Save(stream);
                                    bit = stream.ToArray();
                                    stream.Close();
                                }
                                personaFotos.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                {
                                    BIOMETRICO = bit,
                                    ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                                    ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                                    ID_PERSONA = persona.ID_PERSONA
                                });
                            }
                        }));
                    }
                    #endregion

                    #region Huellas
                    if (HuellasCapturadas != null)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            foreach (var item in HuellasCapturadas)
                            {
                                personaFotos.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                {
                                    BIOMETRICO = item.BIOMETRICO,
                                    ID_TIPO_BIOMETRICO = (short)item.ID_TIPO_BIOMETRICO,
                                    ID_FORMATO = (short)item.ID_TIPO_FORMATO,
                                    ID_PERSONA = persona.ID_PERSONA
                                });
                            }
                        }));
                    }
                    #endregion

                    if (SelectPersona == null)
                    {
                        if (!PInsertar)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            }));
                            return false;
                        }

                        #region Persona
                        persona.ID_PERSONA = int.Parse(Fechas.GetFechaDateServer.Year + "" + new cPersona().GetSequence<int>("ID_PERSONA_SEQ"));
                        persona.ABOGADO = abogado;
                        persona.PERSONA_BIOMETRICO = personaFotos;
                        #endregion

                        #region Abogado
                        abogado.ALTA_FEC = Fechas.GetFechaDateServer;
                        #endregion

                        #region NIP
                        //int nip = 0;
                        //var personaNips = new List<PERSONA_NIP>();
                        //var ceresos = Parametro.ID_DESCRIPCION_CERESOS;
                        //foreach (var item in ceresos)
                        //{
                        //    var pn = new PERSONA_NIP();
                        //    pn.NIP = new cPersona().GetSequence<int>("NIP_SEQ");
                        //    pn.ID_TIPO_VISITA = tvl;
                        //    pn.ID_CENTRO = short.Parse(item.Split('-')[0]);
                        //    if (pn.ID_CENTRO == GlobalVar.gCentro)
                        //        nip = pn.NIP.Value;
                        //    personaNips.Add(pn);
                        //}
                        //persona.PERSONA_NIP = personaNips;
                        #endregion

                        if (new cPersona().InsertarPersona(persona))
                        {
                            TextCodigoAbogado = persona.ID_PERSONA.ToString();
                            //TextNip = nip.ToString();
                            SelectPersona = persona;
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        if (!PEditar)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            }));
                            return false;
                        }

                        #region Persona
                        persona.ID_PERSONA = SelectPersona.ID_PERSONA;
                        persona.ID_TIPO_PERSONA = SelectPersona.ID_TIPO_PERSONA;
                        persona.LUGAR_NACIMIENTO = SelectPersona.LUGAR_NACIMIENTO;
                        persona.ESTADO_CIVIL = SelectPersona.ESTADO_CIVIL;
                        persona.ID_ETNIA = SelectPersona.ID_ETNIA;
                        persona.NORIGINAL = SelectPersona.NORIGINAL;
                        persona.CORIGINAL = SelectPersona.CORIGINAL;
                        #endregion
                        if (SelectPersona.ABOGADO != null)
                        {
                            #region Biometrico
                            if (personaFotos != null)
                            {
                                foreach (var f in personaFotos)
                                {
                                    f.ID_PERSONA = SelectPersona.ID_PERSONA;
                                }
                            }
                            #endregion

                            #region Abogado
                            if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO != "A")
                            {
                                abogado.CREDENCIALIZADO = "N";
                            }
                            else
                                abogado.CREDENCIALIZADO = SelectPersona.ABOGADO.CREDENCIALIZADO;
                            abogado.ID_ABOGADO = SelectPersona.ID_PERSONA;
                            abogado.ABOGADO_TITULAR = SelectPersona.ABOGADO.ABOGADO_TITULAR;
                            abogado.NORIGINAL = SelectPersona.ABOGADO.NORIGINAL;
                            #endregion
                        }
                        abogado.ULTIMA_MOD_FEC = hoy;
                        abogado.ALTA_FEC = SelectPersona.ABOGADO.ALTA_FEC;
                        if (new cPersona().ActualizarAbogado(persona, personaFotos, abogado))
                        {
                            SelectPersona = new cPersona().Obtener(persona.ID_PERSONA).FirstOrDefault();
                            return true;
                        }
                        else
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los datos del actuario.", ex);
                    return false;
                }
            });

            if (respuesta)
            {
                new Dialogos().ConfirmacionDialogo("Éxito", "El actuario se guardo correctamente");
                StaticSourcesViewModel.SourceChanged = false;
            }
            //else
            //    new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar actuario");
        }

        private async void GuardarAbogado()
        {
            try
            {
                if (SelectPersona != null ? SelectPersona.ID_PERSONA <= 0 : true)
                {
                    if (!Insertable)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes crear un nuevo registro o seleccionar uno ya existente para modificarlo.");
                        }));
                        return;
                    }
                }
                if (ImagesToSave == null)
                    ImagesToSave = new List<ImageSourceToSave>();
                var hoy = Fechas.GetFechaDateServer;
                var VisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                var ActuarioTitular = Parametro.ID_ABOGADO_TITULAR_ACTUARIO;
                var persona = new SSP.Servidor.PERSONA
                {
                    ID_PERSONA = Editable ? SelectPersona.ID_PERSONA : int.Parse(Fechas.GetFechaDateServer.Year + "" + new cPersona().GetSequence<int>("ID_PERSONA_SEQ")),
                    CORIGINAL = Editable ? SelectPersona.CORIGINAL : null,
                    CORREO_ELECTRONICO = TextCorreo,
                    CURP = TextCurp,
                    DOMICILIO_CALLE = TextCalle,
                    DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                    DOMICILIO_NUM_EXT = TextNumExt,
                    DOMICILIO_NUM_INT = TextNumInt,
                    ESTADO_CIVIL = Editable ? SelectPersona.ESTADO_CIVIL : null,
                    FEC_NACIMIENTO = SelectFechaNacimiento,
                    ID_COLONIA = SelectColonia,
                    ID_ENTIDAD = SelectEntidad,
                    ID_ETNIA = Editable ? SelectPersona.ID_ETNIA : null,
                    ID_MUNICIPIO = SelectMunicipio,
                    ID_PAIS = SelectPais,
                    ID_TIPO_DISCAPACIDAD = SelectTipoDiscapacidad,
                    ID_TIPO_PERSONA = Editable ? SelectPersona.ID_TIPO_PERSONA : 2,
                    IFE = TextIne,
                    LUGAR_NACIMIENTO = Editable ? SelectPersona.LUGAR_NACIMIENTO : null,
                    MATERNO = TextMaternoAbogado,
                    PATERNO = TextPaternoAbogado,
                    NOMBRE = TextNombreAbogado,
                    NACIONALIDAD = SelectPais,
                    NORIGINAL = Editable ? SelectPersona.NORIGINAL : new Nullable<int>(),
                    RFC = TextRfc,
                    SEXO = SelectSexo,
                    SMATERNO = Editable ? SelectPersona.SMATERNO : null,
                    SPATERNO = Editable ? SelectPersona.SPATERNO : null,
                    SNOMBRE = Editable ? SelectPersona.SNOMBRE : null,
                    TELEFONO = TextTelefonoFijo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                    TELEFONO_MOVIL = TextTelefonoMovil.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
                };

                #region ABOGADO
                var abogado = new ABOGADO();
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    try
                    {
                        abogado = new ABOGADO
                        {
                            ID_ABOGADO = persona.ID_PERSONA,
                            IFE_FRONTAL = ImagesToSave.Where(w => w.FrameName == "ImgFrente").Any() ?
                                new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgFrente").FirstOrDefault().ImageCaptured) :
                                null,
                            IFE_REVERSO = ImagesToSave.Where(w => w.FrameName == "ImgReverso").Any() ?
                                new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgReverso").FirstOrDefault().ImageCaptured) :
                                null,
                            CJF = TextCedulaCJF,
                            CEDULA_FRONTAL = ImagesToSave.Where(w => w.FrameName == "ImgFrenteCedula").Any() ?
                                new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgFrenteCedula").FirstOrDefault().ImageCaptured) :
                                null,
                            CEDULA_REVERSO = ImagesToSave.Where(w => w.FrameName == "ImgReversoCedula").Any() ?
                                new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgReversoCedula").FirstOrDefault().ImageCaptured) :
                                null,
                            NORIGINAL = persona.NORIGINAL,
                            ALTA_FEC = Editable ? NuevoAbogado ? hoy : SelectPersona.ABOGADO.ALTA_FEC : hoy,
                            OBSERV = TextObservaciones,
                            ULTIMA_MOD_FEC = hoy,
                            ID_ABOGADO_TITULO = ActuarioTitular,
                            ID_ESTATUS_VISITA = SelectEstatusVisita,
                            CEDULA = Editable ? SelectPersona.ABOGADO.CEDULA : null,
                            ABOGADO_TITULAR = Editable ? NuevoAbogado ? new Nullable<int>() : SelectPersona.ABOGADO.ABOGADO_TITULAR : new Nullable<int>(),
                            ID_JUZGADO = SelectJuzgado,
                            CREDENCIALIZADO = Editable ? NuevoAbogado ? "N" : SelectPersona.ABOGADO.CREDENCIALIZADO : "N"
                        };
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del actuario.", ex);
                    }
                }));
                #endregion

                #region NIP
                var personaNips = new List<PERSONA_NIP>();
                if (Editable ? NuevoAbogado : true)
                {
                    var ceresos = Parametro.ID_DESCRIPCION_CERESOS;
                    foreach (var item in ceresos)
                        personaNips.Add(new PERSONA_NIP()
                        {
                            NIP = new cPersona().GetSequence<int>("NIP_SEQ"),
                            ID_TIPO_VISITA = VisitaLegal,
                            ID_PERSONA = persona.ID_PERSONA,
                            ID_CENTRO = short.Parse(item.Split('-')[0])
                        });
                }
                #endregion

                #region FOTOS
                var personaFotos = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                if (FotoActualizada)
                {
                    if (ImageFrontal == null ? false : ImageFrontal.Count != 1)
                        ImageFrontal = null;
                    else if (ImageFrontal == null ? false : ImageFrontal.Where(w => w.FrameName == "ImgSenaParticular" && w.ImageCaptured != null).Count() == 0)
                        ImageFrontal = null;
                    if (ImageFrontal != null)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            foreach (var item in ImageFrontal)
                            {
                                var encoder = new JpegBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                                encoder.QualityLevel = 100;
                                var bit = new byte[0];
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                                    encoder.Save(stream);
                                    bit = stream.ToArray();
                                    stream.Close();
                                }
                                personaFotos.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                {
                                    BIOMETRICO = bit,
                                    ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                                    ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                                    ID_PERSONA = persona.ID_PERSONA
                                });
                            }
                        }));
                    }
                }
                #endregion

                #region HUELLAS
                var personaHuellas = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                if (HuellasCapturadas != null)
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        foreach (var item in HuellasCapturadas)
                        {
                            personaHuellas.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                            {
                                BIOMETRICO = item.BIOMETRICO,
                                ID_TIPO_BIOMETRICO = (short)item.ID_TIPO_BIOMETRICO,
                                ID_FORMATO = (short)item.ID_TIPO_FORMATO,
                                ID_PERSONA = persona.ID_PERSONA
                            });
                        }
                    }));
                if (HuellasCapturadas == null)
                    HuellasCapturadas = new List<PlantillaBiometrico>();
                #endregion

                if (!new cPersona().InsertarAbogadoTransaccion(persona, Editable, abogado,
                    NuevoAbogado, false, Parametro.ID_ABOGADO_TITULAR_ACTUARIO,
                    Parametro.ID_ESTATUS_VISITA_CANCELADO, personaNips, personaFotos, personaHuellas,
                    (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO))
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se pudo guardar la información.");
                    return;
                }
                SelectPersona = new cPersona().ObtenerPersonaXID(persona.ID_PERSONA).FirstOrDefault();
                Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                SetValidaciones();
                await GetDatosPersonaSeleccionada();
                StaticSourcesViewModel.SourceChanged = false;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    (new Dialogos()).ConfirmacionDialogo("Éxito!", "Información grabada exitosamente.");
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private bool GuardarFotosYHuellas(int idPersona)
        {
            try
            {
                var guardafotos = false;
                var guardahuellas = false;
                if (ImageFrontal == null && HuellasCapturadas == null)
                    return true;
                if (ImageFrontal == null ? false : ImageFrontal.Count != 1)
                    return true;
                else if (ImageFrontal == null ? false : ImageFrontal.Where(w => w.FrameName == "ImgSenaParticular" && w.ImageCaptured != null).Count() == 0)
                    return false;
                else
                {
                    var personaFoto = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                    if (ImageFrontal != null)
                    {
                        #region [Fotos]
                        foreach (var item in ImageFrontal)
                        {
                            var encoder = new JpegBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                            encoder.QualityLevel = 100;
                            var bit = new byte[0];
                            using (MemoryStream stream = new MemoryStream())
                            {
                                encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                                encoder.Save(stream);
                                bit = stream.ToArray();
                                stream.Close();
                            }
                            personaFoto.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                            {
                                BIOMETRICO = bit,
                                ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                                ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                                ID_PERSONA = idPersona
                            });
                        }
                        if (new cPersonaBiometrico().ObtenerTodos(idPersona).Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Count() == 1)
                        {
                            #region [Actualizar Fotos]
                            guardafotos = (new cPersonaBiometrico()).Actualizar(personaFoto);
                            #endregion
                        }
                        else
                        {
                            #region [Intertar Fotos]
                            guardafotos = (new cPersonaBiometrico()).EliminarFotos(idPersona);
                            guardafotos = (new cPersonaBiometrico()).Insertar(personaFoto);
                            #endregion
                        }
                        if (guardafotos) { }
                        #endregion
                    }

                    #region [Huellas]

                    var personaHuella = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                    if (HuellasCapturadas != null)
                        foreach (var item in HuellasCapturadas)
                        {
                            personaHuella.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                            {
                                BIOMETRICO = item.BIOMETRICO,
                                ID_TIPO_BIOMETRICO = (short)item.ID_TIPO_BIOMETRICO,
                                ID_FORMATO = (short)item.ID_TIPO_FORMATO,
                                //CALIDAD = item.CALIDAD.HasValue ? item.CALIDAD : null,
                                ID_PERSONA = idPersona
                            });
                        }
                    if (new cPersonaBiometrico().ObtenerTodos(idPersona).Where(w => w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP || w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ).ToList().Count() == 20)
                    {
                        #region [actualizar biometrico]
                        HuellasCapturadas = null;
                        if (personaHuella.Any())
                            guardahuellas = (new cPersonaBiometrico()).Actualizar(personaHuella);
                        else
                            guardahuellas = true;
                        #endregion
                    }
                    else
                    {
                        #region [insertar Huellas]
                        HuellasCapturadas = null;
                        guardahuellas = (new cPersonaBiometrico()).EliminarHuellas(idPersona);
                        if (personaHuella.Any())
                        {
                            guardahuellas = (new cPersonaBiometrico()).Insertar(personaHuella);
                        }
                        else
                            guardahuellas = true;
                        #endregion
                    }
                    if (guardahuellas)
                    {
                        if (ImagesToSave != null) { }
                        else
                            guardafotos = true;
                    }

                    #endregion
                }
                if (guardahuellas && guardafotos)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar las fotos/huellas.", ex);
                return false;
            }
        }

        private void EnterAbogados(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                BuscarPersonas();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }

        private void EnterPersonas(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                BuscarPersonasSinCodigo();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }

        private async void BuscarPersonas()
        {
            try
            {
                SelectPersonaAuxiliar = SelectPersona;
                ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                if (string.IsNullOrEmpty(TextCodigoAbogado))
                {
                    var person = SelectPersona;
                    TextNombre = TextNombreAbogado;
                    TextPaterno = TextPaternoAbogado;
                    TextMaterno = TextMaternoAbogado;
                    ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                    SelectPersonaAuxiliar = person;
                    Editable = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                }
                else
                {
                    var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                    //var person = SelectPersona;
                    ListPersonas.InsertRange(await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                         new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodos(TextNombre, TextPaterno, TextMaterno, int.Parse(TextCodigoAbogado))
                             .OrderByDescending(o => o.ID_TIPO_PERSONA == legal).ThenByDescending(t => t.ABOGADO != null))));
                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                    //SelectPersonaAuxiliar = person;
                    if (ListPersonas.Count == 1)
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("Validación",
                                "Existen cambios sin guardar, esta segúro que desea seleccionar a otra persona?") != 1)
                                return;
                            else
                            {
                                SelectPersona = ListPersonas.FirstOrDefault();//SelectPersonaAuxiliar;
                                //return;
                            }
                        }
                        else
                            SelectPersona = ListPersonas.FirstOrDefault();
                        if (SelectPersona.ABOGADO != null)
                        {
                            if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO)
                            {
                                NuevoActuario = NuevoAbogado = false;
                                Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                SetValidaciones();
                                GetDatosPersonaSeleccionada();
                                StaticSourcesViewModel.SourceChanged = false;
                                BuscarAbogadoEnabled = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                Editable = false;
                            }
                            else
                            {
                                if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR)
                                    (new Dialogos()).ConfirmacionDialogo("Validación", "Un colaborador no puede convertirse en actuario.");
                                else
                                {
                                    if (await new Dialogos().ConfirmarEliminar("Validación", "El abogado seleccionado no esta registrado como actuario,"
                                            + " ¿Desea cambiarlo ahora?") == 1)
                                    {
                                        NuevoAbogado = false;
                                        NuevoActuario = true;
                                        Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                        SetValidaciones();
                                        GetDatosPersonaSeleccionada();
                                        BuscarAbogadoEnabled = true;
                                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                        Editable = false;
                                    }
                                    else
                                    {
                                        SelectPersona = SelectPersonaAuxiliar;
                                        if( SelectPersona != null)
                                        {
                                            TextCodigoAbogado = SelectPersona.ID_PERSONA.ToString();
                                            TextNombreAbogado = SelectPersona.NOMBRE;
                                            TextPaternoAbogado = SelectPersona.PATERNO;
                                            TextMaternoAbogado = SelectPersona.MATERNO;
                                            TextCurp = SelectPersona.CURP;
                                            TextRfc = SelectPersona.RFC;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (await new Dialogos().ConfirmarEliminar("Validación", "La persona seleccionada no esta registrada como actuario,"
                                    + " ¿Desea registrarla ahora?") == 1)
                            {
                                NuevoAbogado = true;
                                NuevoActuario = false;
                                Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                SetValidaciones();
                                GetDatosPersonaSeleccionada();
                                BuscarAbogadoEnabled = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                Editable = false;
                            }
                            else {
                                SelectPersona = SelectPersonaAuxiliar;
                                if (SelectPersona != null)
                                {
                                    TextCodigoAbogado = SelectPersona.ID_PERSONA.ToString();
                                    TextNombreAbogado = SelectPersona.NOMBRE;
                                    TextPaternoAbogado = SelectPersona.PATERNO;
                                    TextMaternoAbogado = SelectPersona.MATERNO;
                                    TextCurp = SelectPersona.CURP;
                                    TextRfc = SelectPersona.RFC;
                                }
                            }
                        }
                    }
                    else
                    {
                        TextNombre = TextNombreAbogado;
                        TextPaterno = TextPaternoAbogado;
                        TextMaterno = TextMaternoAbogado;
                        Editable = true;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    }
                }
                EmptyBuscarRelacionInternoVisible = !(ListPersonas.Count > 0);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void BuscarPersonasSinCodigo()
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    if (TextNombre == null)
                        TextNombre = string.Empty;
                    if (TextPaterno == null)
                        TextPaterno = string.Empty;
                    if (TextMaterno == null)
                        TextMaterno = string.Empty;
                }
                else
                {
                    if (TextNombre == null)
                        TextNombre = string.Empty;
                    if (TextPaterno == null)
                        TextPaterno = string.Empty;
                    if (TextMaterno == null)
                        TextMaterno = string.Empty;
                    StaticSourcesViewModel.SourceChanged = false;
                }
                var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                Application.Current.Dispatcher.Invoke((Action)(async delegate
                {
                    ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    if (!(TextNombre == string.Empty && TextPaterno == string.Empty && TextMaterno == string.Empty))
                    {
                        ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                        ListPersonasAuxiliar.InsertRange(ListPersonas);
                    }
                    if (PopUpsViewModels.VisibleBuscarPersonasExistentes == Visibility.Collapsed)
                    {
                        var person = SelectPersona;
                        SelectPersonaAuxiliar = SelectPersona;
                        Editable = true;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    }
                    EmptyBuscarRelacionInternoVisible = !(ListPersonas.Count > 0);
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private async Task<List<SSP.Servidor.PERSONA>> SegmentarPersonasBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(TextPaterno) && string.IsNullOrEmpty(TextMaterno) && string.IsNullOrEmpty(TextNombre) && string.IsNullOrEmpty(TextCodigoAbogado))
                    return new List<SSP.Servidor.PERSONA>();
                Pagina = _Pag;
                var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                        new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodos(TextNombre, TextPaterno, TextMaterno, 0, _Pag)
                             .OrderByDescending(o => o.ABOGADO != null).ThenByDescending(t => t.ID_TIPO_PERSONA == legal)));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargandoPersonas = true;
                }
                else
                    SeguirCargandoPersonas = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<SSP.Servidor.PERSONA>();
            }
        }

        private async Task GetDatosPersonaSeleccionada()
        {
            try
            {
                TextCodigoAbogado = SelectPersona.ID_PERSONA.ToString();
                //TextPaternoAbogado = SelectPersona.PATERNO.Trim();
                //TextMaternoAbogado = SelectPersona.MATERNO.Trim();
                //TextNombreAbogado = SelectPersona.NOMBRE.Trim();
                TextPaternoAbogado = !string.IsNullOrEmpty(SelectPersona.PATERNO) ? SelectPersona.PATERNO.Trim() : string.Empty;
                TextMaternoAbogado = !string.IsNullOrEmpty(SelectPersona.MATERNO) ? SelectPersona.MATERNO.Trim() : string.Empty;
                TextNombreAbogado = !string.IsNullOrEmpty(SelectPersona.NOMBRE) ? SelectPersona.NOMBRE.Trim() : string.Empty;
                SelectSexo = SelectPersona.SEXO;
                var hoy = Fechas.GetFechaDateServer;
                SelectFechaNacimiento = SelectPersona.FEC_NACIMIENTO.HasValue ? SelectPersona.FEC_NACIMIENTO.Value : hoy;
                TextCurp = SelectPersona.CURP;
                TextRfc = SelectPersona.RFC;
                TextTelefonoFijo = SelectPersona.TELEFONO;
                TextTelefonoMovil = SelectPersona.TELEFONO_MOVIL;
                TextCorreo = SelectPersona.CORREO_ELECTRONICO;
                TextIne = SelectPersona.IFE;
                SelectDiscapacitado = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value > 0 ? "S" : "N" : "N";
                SelectTipoDiscapacidad = (short)(SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value : -1);
                ImagesToSave = new List<ImageSourceToSave>();
                Insertable = false;
                if (SelectPersona.ABOGADO != null)
                {
                    TextCedulaCJF = SelectPersona.ABOGADO.CJF;
                    SelectFechaAlta = SelectPersona.ABOGADO.ALTA_FEC.HasValue ? SelectPersona.ABOGADO.ALTA_FEC.Value.ToString("dd/MM/yyyy") : hoy.ToString("dd/MM/yyyy");
                    SelectEstatusVisita = SelectPersona.ABOGADO.ID_ESTATUS_VISITA.HasValue ? SelectPersona.ABOGADO.ID_ESTATUS_VISITA.Value : (short)-1;
                    SelectJuzgado = SelectPersona.ABOGADO.ID_JUZGADO.HasValue ? SelectPersona.ABOGADO.ID_JUZGADO.Value : (short)-1;
                    TextObservaciones = SelectPersona.ABOGADO.OBSERV;
                    ImagesToSave = new List<ImageSourceToSave>();
                    Credencializado = ImpresionGafeteFront = ImpresionGafeteBack = ImpresionGafete = SelectPersona.ABOGADO.CREDENCIALIZADO == "S";
                    if (SelectPersona.ABOGADO.IFE_FRONTAL != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgFrente", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.IFE_FRONTAL) });
                    if (SelectPersona.ABOGADO.IFE_REVERSO != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgReverso", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.IFE_REVERSO) });
                    if (SelectPersona.ABOGADO.CEDULA_FRONTAL != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgFrenteCedula", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.CEDULA_FRONTAL) });
                    if (SelectPersona.ABOGADO.CEDULA_REVERSO != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgReversoCedula", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.CEDULA_REVERSO) });
                }
                else
                {
                    ImpresionGafeteFront = ImpresionGafeteBack = ImpresionGafete = Credencializado = false;
                    TextCedulaCJF = TextObservaciones = string.Empty;
                    ImagesToSave = new List<ImageSourceToSave>();
                    SelectFechaAlta = hoy.ToString("dd/MM/yyyy");
                    SelectJuzgado = SelectEstatusVisita = -1;
                }
                TextCalle = SelectPersona.DOMICILIO_CALLE;
                TextNumInt = SelectPersona.DOMICILIO_NUM_INT;
                TextNumExt = SelectPersona.DOMICILIO_NUM_EXT;
                TextCodigoPostal = SelectPersona.DOMICILIO_CODIGO_POSTAL;
                FotoTomada = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any();
                if (FotoTomada)
                {
                    ImagenAbogado = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    ImageFrontal = new List<ImageSourceToSave>();
                    ImageFrontal.Add(new ImageSourceToSave { FrameName = "ImgSenaParticular", ImageCaptured = new Imagenes().ConvertByteToBitmap(ImagenAbogado) });
                }
                else
                {
                    ImagenAbogado = new Imagenes().getImagenPerson();
                    ImageFrontal = new List<ImageSourceToSave>();
                }
                //huellas
                HuellasCapturadas = new List<PlantillaBiometrico>();
                if (SelectPersona.PERSONA_BIOMETRICO != null)
                {
                    foreach (var h in SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9))
                    {
                        HuellasCapturadas.Add(new PlantillaBiometrico() { ID_TIPO_BIOMETRICO = (enumTipoBiometrico)h.ID_TIPO_BIOMETRICO, ID_TIPO_FORMATO = (enumTipoFormato)h.ID_FORMATO, BIOMETRICO = h.BIOMETRICO, });
                    }
                }

                var VisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                //if (SelectPersona.PERSONA_NIP != null ? SelectPersona.PERSONA_NIP.Count > 0 : false)
                //{
                //    var VisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                //    TextNip = SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_VISITA == VisitaLegal).FirstOrDefault().NIP.HasValue ?
                //        SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_VISITA == VisitaLegal).FirstOrDefault().NIP.Value.ToString() : string.Empty;
                //}
                SelectPais = -1;
                await TaskEx.Delay(50);
                SelectPais = SelectPersona.ID_PAIS;
                await TaskEx.Delay(50);
                SelectEntidad = SelectPersona.ID_ENTIDAD;
                await TaskEx.Delay(50);
                SelectMunicipio = SelectPersona.ID_MUNICIPIO;
                await TaskEx.Delay(50);
                if (SelectPersona.ID_COLONIA.HasValue ? SelectPersona.ID_COLONIA.Value > 0 : false)
                    SelectColonia = SelectPersona.ID_COLONIA;

                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void LimpiarCampos()
        {
            try
            {
                TextCodigo = TextPaterno = TextMaterno = TextNombre = TextCodigoAbogado = TextPaternoAbogado = TextMaternoAbogado = TextNombreAbogado =
                TextCurp = TextRfc = TextTelefonoFijo = TextTelefonoMovil = TextCorreo = TextIne = TextCedulaCJF = TextCalle = 
                     TextNumInt = TextObservaciones = SelectDiscapacitado = string.Empty;
                SelectFechaNacimiento = Fechas.GetFechaDateServer;
                SelectFechaAlta = SelectFechaNacimiento.ToString("dd/MM/yyyy");
                SelectSexo = "S";
                TextNumExt = TextCodigoPostal = new Nullable<int>();
                SelectPais = Parametro.PAIS; //82;
                SelectEstatusVisita = SelectJuzgado = -1;
                ImagesToSave = ImageFrontal = null;
                ImagenAbogado = new Imagenes().getImagenPerson();
                SelectPersona = null;
                BuscarAbogadoEnabled = StaticSourcesViewModel.SourceChanged = Credencializado = ImpresionGafeteFront = ImpresionGafeteBack =
                    ImpresionGafete = Editable = NuevoAbogado = FotoTomada = FotoActualizada = false;
                HuellasCapturadas = null;
                ListPersonasAuxiliar = null;
                ListPersonas = null;
                Insertable = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar la pantalla.", ex);
            }
        }

        private void CrearGafete()
        {
            try
            {
                #region VALIDACIONES
                if (SelectPersona == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar o crear un actuario.");
                    return;
                }
                if (string.IsNullOrEmpty(SelectPersona.TELEFONO) && string.IsNullOrEmpty(SelectPersona.TELEFONO_MOVIL))
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Se debe registrar un telefono del actuario.");
                    return;
                }
                var fotos = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG);
                if (!fotos.Any())
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debes tomarle foto al actuario.");
                    return;
                }
                //if (SelectPersona.PERSONA_BIOMETRICO.Count <= 1)
                //{
                //    (new Dialogos()).ConfirmacionDialogo("Validación", "No tiene huellas capturadas.");
                //    return;
                //}
                if (SelectPersona.ID_TIPO_DISCAPACIDAD != null && SelectPersona.ID_TIPO_DISCAPACIDAD > 0)
                {
                    if (SelectPersona.TIPO_DISCAPACIDAD.HUELLA == "S")
                        if (SelectPersona.PERSONA_BIOMETRICO.Count(w => w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9) == 0)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No tiene huellas capturadas.");
                            return;
                        }
                }
                else
                    if (SelectPersona.PERSONA_BIOMETRICO.Count(w => w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9) == 0)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No tiene huellas capturadas.");
                        return;
                    }
                var TipoVisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                //var nip = SelectPersona.PERSONA_NIP.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal);
                //if (!nip.Any())
                //{
                //    (new Dialogos()).ConfirmacionDialogo("Validación", "El actuario no tiene NIP registrado.");
                //    return;
                //}
                if (SelectPersona.ABOGADO == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar a un actuario.");
                    return;
                }
                if (SelectPersona.ABOGADO.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_SUSPENDIDO ||
                    SelectPersona.ABOGADO.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_CANCELADO)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Este actuario se encuentra " + SelectPersona.ABOGADO.ESTATUS_VISITA.DESCR + ".");
                    return;
                }
                if (SelectPersona.ABOGADO.IFE_FRONTAL == null || SelectPersona.ABOGADO.IFE_REVERSO == null)
                {
                    var ife = Parametro.TIPO_DOCTO_IFE_LEGAL;
                    if (!SelectPersona.ABOGADO.ABOGADO_DOCUMENTO.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal && w.ID_TIPO_DOCUMENTO == ife).Any())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Aun no cuenta con la digitalizacion de la identificacion oficial.");
                        return;
                    }
                }
                if (SelectPersona.ABOGADO.CEDULA_FRONTAL == null || SelectPersona.ABOGADO.CEDULA_REVERSO == null)
                {
                    var cedula = Parametro.TIPO_DOCTO_CEDULA_LEGAL;
                    if (!SelectPersona.ABOGADO.ABOGADO_DOCUMENTO.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal && w.ID_TIPO_DOCUMENTO == cedula).Any())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Aun no cuenta con la digitalizacion de la cedula.");
                        return;
                    }
                }
                if (!SelectPersona.ABOGADO.ABOGADO_DOCUMENTO.Where(w => w.ID_TIPO_DOCUMENTO == Parametro.TIPO_DOCTO_TITULO_LEGAL && w.ID_TIPO_VISITA == TipoVisitaLegal).Any())
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Aun no cuenta con los documentos necesarios.");
                    return;
                }
                /*
                var doc = 0;
                foreach (var item in Parametro.DOCUMENTOS_ACTUARIOS)
                {
                    if (short.Parse(item.Split('-')[0]) == Parametro.ID_TIPO_VISITA_LEGAL)
                    {
                        doc = short.Parse(item.Split('-')[1]);
                        if (!SelectPersona.ABOGADO.ABOGADO_DOCUMENTO.Where(w => w.ID_TIPO_DOCUMENTO == doc).Any())
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Aun no cuenta con los documentos necesarios.");
                            return;
                        }
                    }
                }
                */
                #endregion

                #region GAFETE
                var gafetes = new List<GafeteAbogado>();
                GafeteView = new GafetesPVCView();
                var gaf = new GafeteAbogado();
                var centro = new cCentro().Obtener(4).FirstOrDefault();
                GafeteFrente = true;
                gaf.Discapacidad = SelectPersona.ID_TIPO_DISCAPACIDAD == null || SelectPersona.ID_TIPO_DISCAPACIDAD == 0 ? "NINGUNA" : SelectPersona.TIPO_DISCAPACIDAD.DESCR;
                gaf.TipoPersona = SelectPersona.ABOGADO.ABOGADO_TITULO.DESCR;
                gaf.Imagen = fotos.FirstOrDefault().BIOMETRICO;
                var ceresos = Parametro.ID_DESCRIPCION_CERESOS;
                //gaf.NipCereso1 = nip.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[0].Split('-')[0])).FirstOrDefault().NIP.Value.ToString();
                //gaf.DescrCereso1 = ceresos[0].Split('-')[1];
                //gaf.NipCereso2 = nip.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[1].Split('-')[0])).FirstOrDefault().NIP.Value.ToString();
                //gaf.DescrCereso2 = ceresos[1].Split('-')[1];
                //gaf.NipCereso3 = nip.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[2].Split('-')[0])).FirstOrDefault().NIP.Value.ToString();
                //gaf.DescrCereso3 = ceresos[2].Split('-')[1];
                //gaf.NipCereso4 = nip.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[3].Split('-')[0])).FirstOrDefault().NIP.Value.ToString();
                //gaf.DescrCereso4 = ceresos[3].Split('-')[1];
                //gaf.NipCereso5 = nip.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[4].Split('-')[0])).FirstOrDefault().NIP.Value.ToString();
                //gaf.DescrCereso5 = ceresos[4].Split('-')[1];
                //gaf.NipCereso6 = ceresos.Length == 6 ? nip.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[5].Split('-')[0])).FirstOrDefault().NIP.Value.ToString() : "0";
                //gaf.DescrCereso6 = ceresos.Length == 6 ? ceresos[5].Split('-')[1] : "HONGO III";
                gaf.RFC = SelectPersona.RFC;
                gaf.Cedula = SelectPersona.ABOGADO.CEDULA;
                var hoy = Fechas.GetFechaDateServer;
                gaf.Fecha = centro.MUNICIPIO.MUNICIPIO1.Replace("(BCN)", "").Trim() + " B.C. A " + hoy.ToString("dd DE MMMM DE yyyy").ToUpper();
                gaf.FechaAlta = hoy.ToString("dd/MM/yyyy");
                gaf.NombreAbogado = SelectPersona.NOMBRE.Trim() + " " + SelectPersona.PATERNO.Trim() + " " + SelectPersona.MATERNO.Trim();
                gaf.NumeroCredencial = SelectPersona.ID_PERSONA.ToString();
                gaf.Telefono = string.IsNullOrEmpty(SelectPersona.TELEFONO) ? string.IsNullOrEmpty(SelectPersona.TELEFONO_MOVIL) ? "N/A" : SelectPersona.TELEFONO_MOVIL.Trim() : SelectPersona.TELEFONO.Trim();
                var tipoVisita = "V I S I T A  LEGAL";
                //foreach (var item in nip.FirstOrDefault().TIPO_VISITA.DESCR.Trim())
                //{
                //    tipoVisita = tipoVisita + item + " ";
                //}
                gaf.TipoVisita = tipoVisita;
                Reporteador = GafeteView.GafetesPVCReport;
                Reporteador.LocalReport.ReportPath = "Reportes/" + gafeteAbogado + ".rdlc";
                Reporteador.ShowExportButton = false;
                gafetes.Add(gaf);
                var rds = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds.Name = "DataSet1";
                rds.Value = gafetes;
                Reporteador.ShowExportButton = false;
                Reporteador.ShowPrintButton = false;
                Reporteador.LocalReport.DataSources.Clear();
                Reporteador.LocalReport.DataSources.Add(rds);
                Reporteador.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                Reporteador.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                Reporteador.ZoomPercent = 135;
                Reporteador.VerticalScroll.Visible = false;
                Reporteador.HorizontalScroll.Visible = false;
                Reporteador.VerticalScroll.Enabled = false;
                Reporteador.HorizontalScroll.Enabled = false;
                GafeteView.Margin = new Thickness() { Bottom = 0, Left = 0, Right = 0, Top = 0 };
                Reporteador.RefreshReport();
                GafeteView.DataContext = this;
                GafeteView.Title = "Impresion De Gafete";
                GafeteView.Owner = PopUpsViewModels.MainWindow;
                GafeteView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                GafeteFront = true;
                GafeteBack = false;
                GafeteView.rbFrente.Checked += FrenteChecked;
                GafeteView.rbDetras.Checked += DetrasChecked;
                GafeteView.Closed += GafeteViewClosed;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                GafeteView.ShowDialog();
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar la credencial.", ex);
            }

        }

        private void GafeteViewClosed(object sender, EventArgs e)
        {
            try
            {
                GafeteView.rbFrente.Checked -= FrenteChecked;
                GafeteView.rbDetras.Checked -= DetrasChecked;
                GafeteView.Hide();
                GafeteView.Close();
                Reporteador = null;
                GafeteView = null;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar la credencial.", ex);
            }
        }

        private void FrenteChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((System.Windows.Controls.Primitives.ToggleButton)(sender)).IsChecked.Value)
                {
                    GafeteView.GafetesPVCReport.LocalReport.ReportPath = "Reportes/GafeteAbogadoFrente.rdlc";
                    GafeteView.GafetesPVCReport.RefreshReport();
                    GafeteFront = true;
                    GafeteBack = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar la credencial.", ex);
            }
        }

        private void DetrasChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((System.Windows.Controls.Primitives.ToggleButton)(sender)).IsChecked.Value)
                {
                    GafeteView.GafetesPVCReport.LocalReport.ReportPath = "Reportes/GafeteAbogadoDetras.rdlc";
                    GafeteView.GafetesPVCReport.RefreshReport();
                    GafeteFront = false;
                    GafeteBack = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar la credencial.", ex);
            }
        }

        private async void HeaderSort(Object obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    if (obj != null ? obj.ToString() == "Tipo visita" : false)
                    {
                        ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                        switch (HeaderSortin)
                        {
                            case true:
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    ListPersonas.InsertRange(ListPersonasAuxiliar.OrderByDescending(o => o.ABOGADO != null)
                                        .ThenByDescending(t => t.ABOGADO != null ? t.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO : t.ABOGADO != null));
                                }));
                                HeaderSortin = false;
                                break;
                            case false:
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    ListPersonas.InsertRange(ListPersonasAuxiliar.OrderByDescending(o => o.ABOGADO == null));
                                }));
                                HeaderSortin = true;
                                break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ordenar la busqueda.", ex);
            }
        }

        #region HUELLAS DIGITALES

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
                                            break;
                                        #endregion
                                        #region [Indice Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Medio Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Anular Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Meñique Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Pulgar Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Indice Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Medio Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Anular Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Meñique Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        default:
                                            break;
                                    }
                                }
                                #endregion
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
                var dataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_TODOS, nRet == 0, requiereGuardarHuellas);
                windowBusqueda.DataContext = dataContext;
                windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Persona")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "TIPO VISITA"
                });
                dataContext.CabeceraBusqueda = string.Empty;
                dataContext.CabeceraFoto = string.Empty;

                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }

                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Escape)windowBusqueda.Close(); };
                windowBusqueda.Closed += async (s, e) =>
                {
                    BuscarAbogadoEnabled = true;
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    var huella = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (!huella.IsSucceed)
                        return;
                    if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                    {
                        var persn = SelectPersona;
                        ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                        ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                        Editable = true;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        SelectPersonaAuxiliar = persn;
                        return;
                    }
                    if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null)
                        return;
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1)
                            return;
                        else
                        {
                            SelectPersona = SelectPersonaAuxiliar;
                            return;
                        }
                    }
                    SelectPersona = huella.SelectRegistro.Persona;
                    if (SelectPersona.ABOGADO != null)
                    {
                        if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO)
                        {
                            NuevoActuario = NuevoAbogado = false;
                            Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                            SetValidaciones();
                            GetDatosPersonaSeleccionada();
                            StaticSourcesViewModel.SourceChanged = false;
                            BuscarAbogadoEnabled = true;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            Editable = false;
                        }
                        else
                        {
                            if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR)
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Un colaborador no puede convertirse en actuario.");
                            else
                            {
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "EL ABOGADO SELECCIONADO NO ESTA REGISTRADO COMO ACTUARIO,"
                                        + " DESEA CAMBIARLO AHORA?") == 1)
                                {
                                    NuevoAbogado = false;
                                    NuevoActuario = true;
                                    Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                    SetValidaciones();
                                    GetDatosPersonaSeleccionada();
                                    BuscarAbogadoEnabled = true;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                    Editable = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "LA PERSONA SELECCIONADA NO ESTA REGISTRADA COMO ACTUARIO,"
                                + " DESEA REGISTRARLA AHORA?") == 1)
                        {
                            NuevoAbogado = true;
                            NuevoActuario = false;
                            Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                            SetValidaciones();
                            GetDatosPersonaSeleccionada();
                            BuscarAbogadoEnabled = true;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            Editable = false;
                        }
                        else { }
                    }

                };
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }
       
        private async Task OnBuscarPorHuellaInicio(string obj = "")
        {
            try
            {
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
                var dataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_TODOS, nRet == 0, requiereGuardarHuellas);
                windowBusqueda.DataContext = dataContext;
                windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Persona")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "TIPO VISITA"
                });
                dataContext.CabeceraBusqueda = string.Empty;
                dataContext.CabeceraFoto = string.Empty;

                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }

                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Escape)windowBusqueda.Close(); };
                windowBusqueda.Closed += async (s, e) =>
                {
                    BuscarAbogadoEnabled = true;
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    var huella = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (!huella.IsSucceed)
                        return;
                    if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                    {
                        var persn = SelectPersona;
                        ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                        ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                        Editable = true;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        SelectPersonaAuxiliar = persn;
                        return;
                    }
                    if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null)
                        return;
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1)
                            return;
                        else
                        {
                            SelectPersona = SelectPersonaAuxiliar;
                            return;
                        }
                    }
                    SelectPersona = huella.SelectRegistro.Persona;
                    if (SelectPersona.ABOGADO != null)
                    {
                        if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO)
                        {
                            NuevoActuario = NuevoAbogado = false;
                            Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                            SetValidaciones();
                            GetDatosPersonaSeleccionada();
                            StaticSourcesViewModel.SourceChanged = false;
                            BuscarAbogadoEnabled = true;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            Editable = false;
                        }
                        else
                        {
                            if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR)
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Un colaborador no puede convertirse en actuario.");
                            else
                            {
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "EL ABOGADO SELECCIONADO NO ESTA REGISTRADO COMO ACTUARIO,"
                                        + " DESEA CAMBIARLO AHORA?") == 1)
                                {
                                    NuevoAbogado = false;
                                    NuevoActuario = true;
                                    Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                    SetValidaciones();
                                    GetDatosPersonaSeleccionada();
                                    BuscarAbogadoEnabled = true;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                    Editable = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "LA PERSONA SELECCIONADA NO ESTA REGISTRADA COMO ACTUARIO,"
                                + " DESEA REGISTRARLA AHORA?") == 1)
                        {
                            NuevoAbogado = true;
                            NuevoActuario = false;
                            Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                            SetValidaciones();
                            GetDatosPersonaSeleccionada();
                            BuscarAbogadoEnabled = true;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            Editable = false;
                        }
                        else { }
                    }

                };
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }

        #endregion

        #region DIGITALIZAR DOCUMENTOS
        private async void Scan(PdfViewer obj)
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    await escaner.Scann(Duplex, SelectedSource, obj);
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al escanear el documento.", ex);
            }
        }
        private async void AbrirDocumento(PdfViewer obj)
        {
            try
            {
                if (SelectedTipoDocumento == null)
                {
                    escaner.Hide();
                    await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija El Tipo De Documento A Digitalizar");
                    return;
                }
                if (SelectPersona == null ? false : SelectPersona.ABOGADO == null)
                {
                    escaner.Hide();
                    await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Debe seleccionar un abogado.");
                    return;
                }
                escaner.Show();

                #region PERSONA
                var Documentos = new cAbogadoDocumento().ObtenerTodos(4, SelectedTipoDocumento.ID_TIPO_VISITA, SelectPersona.ID_PERSONA,
                    SelectedTipoDocumento.ID_TIPO_DOCUMENTO).FirstOrDefault();

                if (Documentos == null)
                {
                    StaticSourcesViewModel.Mensaje("Digitalizacion", "Documento No Digitalizado", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                    obj.Visibility = Visibility.Collapsed;
                    DocumentoDigitalizado = null;
                    return;
                }

                if (Documentos.DOCUMENTO == null)
                {
                    StaticSourcesViewModel.Mensaje("Digitalizacion", "Documento No Digitalizado", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                    obj.Visibility = Visibility.Collapsed;
                    DocumentoDigitalizado = null;
                    return;
                }
                DocumentoDigitalizado = Documentos.DOCUMENTO;
                ObservacionDocumento = Documentos.OBSERVACIONES;
                DatePickCapturaDocumento = Documentos.ALTA_FEC;
                #endregion

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
        private void GuardarDocumento()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    escaner.Hide();
                    if (SelectedTipoDocumento == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija el tipo de documento a digitalizar");
                        return;
                    }
                    if (DocumentoDigitalizado == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice un documento para guardar");
                        return;
                    }
                    if (DocumentoDigitalizado.Length <= 0)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice un documento para guardar");
                        return;
                    }
                    if (SelectPersona == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Debe seleccionar una persona.");
                        return;
                    }
                    var hoy = Fechas.GetFechaDateServer;
                    try
                    {
                        #region ABOGADO
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            var Documentos = new cAbogadoDocumento().ObtenerTodos(/*4*/GlobalVar.gCentro, 3, SelectPersona.ID_PERSONA, 0).Where(w =>
                                w.ID_TIPO_DOCUMENTO == SelectedTipoDocumento.ID_TIPO_DOCUMENTO && w.ID_TIPO_VISITA == SelectedTipoDocumento.ID_TIPO_VISITA).FirstOrDefault();

                            if (Documentos == null)
                                if (new cAbogadoDocumento().Insertar(new ABOGADO_DOCUMENTO
                                {
                                    ID_ABOGADO = SelectPersona.ID_PERSONA,
                                    DOCUMENTO = DocumentoDigitalizado,
                                    ID_CENTRO = GlobalVar.gCentro,//4,
                                    ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                                    ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                                    OBSERVACIONES = ObservacionDocumento,
                                    ALTA_FEC = hoy
                                }))
                                {
                                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                    {
                                        StaticSourcesViewModel.Mensaje("Digitalización", "Documento guardado exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                    }));
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                    {
                                        StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un error al grabar el documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                    }));
                                }
                            else
                                if (new cAbogadoDocumento().Actualizar(new ABOGADO_DOCUMENTO
                                {
                                    ID_ABOGADO = SelectPersona.ID_PERSONA,
                                    DOCUMENTO = DocumentoDigitalizado,
                                    ID_CENTRO = GlobalVar.gCentro,//4,
                                    ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                                    ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                                    OBSERVACIONES = ObservacionDocumento,
                                    ALTA_FEC = hoy
                                }))
                                {
                                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                    {
                                        StaticSourcesViewModel.Mensaje("Digitalización", "Documento actualizado exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                    }));
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                    {
                                        StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un error al actualizar el documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                    }));
                                }
                            CargarListaTipoDocumentoDigitalizado();
                        });
                        #endregion

                        if (AutoGuardado)
                            escaner.Show();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al digitalizar documento.", ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al digitalizar documento.", ex);
            }
        }
        void CargarListaTipoDocumentoDigitalizado()
        {
            try
            {
                var VisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                var ListTipoDocumentoAux = new ObservableCollection<TipoDocumento>(new cTipoDocumento().ObtenerAbogadoIngreso(VisitaLegal).Select(s => new TipoDocumento
                {
                    DESCR = s.DESCR,
                    DIGITALIZADO = false,
                    ID_TIPO_DOCUMENTO = s.ID_TIPO_DOCUMENTO,
                    ID_TIPO_VISITA = s.ID_TIPO_VISITA
                }).OrderBy(o => o.DESCR).ToList());
                var doctosAux = new ObservableCollection<TipoDocumento>();
                var doc = 0;
                foreach (var item in Parametro.DOCUMENTOS_ACTUARIOS)
                {
                    doc = short.Parse(item.Split('-')[1]);
                    if (ListTipoDocumentoAux.Where(w => w.ID_TIPO_DOCUMENTO == doc).Any())
                    {
                        doctosAux.Add(ListTipoDocumentoAux.Where(w => w.ID_TIPO_DOCUMENTO == doc).FirstOrDefault());
                    }
                }
                var ListDocumentos = new cAbogadoDocumento().ObtenerTodos(/*4*/GlobalVar.gCentro, VisitaLegal, SelectPersona.ID_PERSONA, 0).ToList();
                foreach (var item in ListDocumentos)
                {
                    if (doctosAux.Where(w => w.ID_TIPO_DOCUMENTO == item.ID_TIPO_DOCUMENTO && w.ID_TIPO_VISITA == item.ID_TIPO_VISITA).Any())
                        doctosAux.Where(w => w.ID_TIPO_DOCUMENTO == item.ID_TIPO_DOCUMENTO && w.ID_TIPO_VISITA == item.ID_TIPO_VISITA).FirstOrDefault().DIGITALIZADO = true;
                }
                ListTipoDocumento = new ObservableCollection<TipoDocumento>(doctosAux.Where(w => w.ID_TIPO_VISITA == VisitaLegal).OrderBy(o => o.ID_TIPO_DOCUMENTO));
                TipoDoctoAbogado = (short)enumTipoDocumentoAbogado.PERSONA;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los tipos de documento.", ex);
            }
        }
        private bool ValidaDocumentosCompletos()
        {
            try
            {
                var lista = new ObservableCollection<TipoDocumento>(ListTipoDocumento);
                var parametros = Parametro.DOCUMENTOS_NO_NECESARIOS;
                if (parametros != null)
                {
                    foreach (var p in parametros)
                    {
                        short tipo_documento = short.Parse(p.Split('-')[1]);
                        short tipo_visita = short.Parse(p.Split('-')[0]);
                        var obj = lista.Where(w => w.ID_TIPO_DOCUMENTO == tipo_documento && w.ID_TIPO_VISITA == tipo_visita);
                        if (obj != null)
                        {
                            var o = obj.FirstOrDefault();
                            lista.Remove(o);
                        }
                    }
                }
                if (lista.Where(w => w.DIGITALIZADO == false).Count() > 0)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar los documentos.", ex);
                return false;
            }
        }
        #endregion
    }
}