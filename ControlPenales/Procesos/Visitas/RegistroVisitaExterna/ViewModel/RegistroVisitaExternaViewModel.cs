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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ControlPenales
{
    partial class RegistroVisitaExternaViewModel : FingerPrintScanner
    {
        #region Constructor
        public RegistroVisitaExternaViewModel() { }
        #endregion

        private async void Load_Window(PadronVisitaExternaView Window)
        {
            try
            {
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
                myStoryboard.Begin(PopUpsViewModels.MainWindow.HuellasView.Ln);

                #endregion

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { llenarCombos(); });
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { ConfiguraPermisos(); });

                if (PopUpsViewModels.VisibleTomarFotoSenasParticulares == Visibility.Visible)
                    TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
            }
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                #region BUSCAR_PERSONA
                case "nueva_busqueda_visitante":
                    TextPaterno = TextMaterno = TextNombre = string.Empty;
                    ListPersonas = null;
                    SelectPersona = null;
                    break;
                case "seleccionar_buscar_persona":
                    try
                    {
                        if (SelectPersona != null)
                        {
                            //Modificacion de modelo, PENDIENTE
                            if (SelectPersona.PERSONA_EXTERNO != null)
                            {
                                ValidacionesActivas = true;
                                SetValidacionesAgregarNuevaVisitaExterna();
                                FotoDetrasCredencial = FotoDetrasCredencialAuxiliar = FotoFrenteCredencial = FotoFrenteCredencialAuxiliar =
                                    FotoPersonaExterna = null;
                                GetDatosNuevoPadronVisitanteSeleccionado();
                                ValidarEnabled = ExisteNuevo = true;
                                NuevoExterno = false;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                StaticSourcesViewModel.SourceChanged = false;
                            }
                            else
                            {
                                var respuesta = await new Dialogos().ConfirmarEliminar("Validación", "La persona seleccionada no esta registrada como externa. ¿Desea registrarla ahora?");
                                if (respuesta == 1)
                                {
                                    SetValidacionesAgregarNuevaVisitaExterna();
                                    GetDatosNuevoPadronVisitanteSeleccionado();
                                    ValidarEnabled = ExisteNuevo = NuevoExterno = true;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                }
                            }
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación", "favor de seleccionar a una persona.");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar la persona.", ex);
                    }
                    break;
                case "cancelar_buscar_persona":
                    try
                    {
                        SelectPersona = SelectPersonaAuxiliar;
                        SelectPersonaAuxiliar = null;
                        if (SelectPersona != null)
                        {
                            TextCodigoNuevo = SelectPersona.ID_PERSONA.ToString();
                            TextPaternoNuevo = SelectPersona.PATERNO;
                            TextMaternoNuevo = SelectPersona.MATERNO;
                            TextNombreNuevo = SelectPersona.NOMBRE;
                        }
                        else
                        {
                            TextCodigoNuevo = TextPaternoNuevo = TextMaternoNuevo = TextNombreNuevo = string.Empty;
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cancelar.", ex);
                    }
                    break;
                case "buscar_visitante":
                    //BuscarPersonasNuevo();
                    TextCodigo = string.Empty;
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                    EmptyBuscarRelacionInternoVisible = ListPersonas.Count > 0 ? false : true;
                    break;
                #endregion

                #region Foto_Credencial
                case "capturar_credencial_visita_externa":
                    try
                    {
                        ComboFrontBackFotoVisible = Visibility.Visible;
                        if (FotoFrenteCredencial != null)
                        {
                            var foto = FotoFrenteCredencial;
                            FotoFrenteCredencialAuxiliar = foto;
                        }
                        if (FotoDetrasCredencial != null)
                        {
                            var foto = FotoDetrasCredencial;
                            FotoDetrasCredencialAuxiliar = foto;
                        }
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir la ventana para tomar fotos.", ex);
                    }
                    break;
                #endregion

                #region Foto_Persona
                case "abrir_camara_visita_externa"://PERSONA
                    try
                    {
                        ComboFrontBackFotoVisible = Visibility.Collapsed;
                        SelectFrenteDetrasFoto = "F";
                        if (FotoPersonaExterna != null)
                        {
                            var foto = FotoPersonaExterna;
                            FotoPersonaExternaAuxiliar = foto;
                        }
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir la ventana para tomar fotos.", ex);
                    }
                    break;
                #endregion

                #region Fotos_Generales
                case "aceptar_tomar_foto_senas":
                    try
                    {
                        if (ComboFrontBackFotoVisible != Visibility.Visible)
                        {
                            if (FotoPersonaExterna != null)
                            {
                                //SelectFrenteDetrasFoto = string.Empty;
                                ImagenVisitaExterna = FotoPersonaExterna;
                                if (CamaraWeb != null)
                                {
                                    await CamaraWeb.ReleaseVideoDevice();
                                    CamaraWeb = null;
                                }
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                                break;
                            }
                            else
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Debes tomar la foto del visitante.");
                                break;
                            }
                        }
                        if (FotoFrenteCredencial == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes tomar la foto frontal.");
                            break;
                        }
                        if (FotoDetrasCredencial == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes tomar la foto trasera.");
                            break;
                        }
                        //SelectFrenteDetrasFoto = string.Empty;
                        ExternoCambio = true;
                        if (CamaraWeb != null)
                        {
                            await CamaraWeb.ReleaseVideoDevice();
                            CamaraWeb = null;
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar las fotos.", ex);
                    }
                    break;
                case "cancelar_tomar_foto_senas":
                    try
                    {
                        if (ComboFrontBackFotoVisible == Visibility.Visible)
                        {
                            if (FotoFrenteCredencialAuxiliar != null)
                            {
                                var foto = FotoFrenteCredencialAuxiliar;
                                FotoFrenteCredencial = foto;
                            }
                            if (FotoDetrasCredencialAuxiliar != null)
                            {
                                var foto = FotoDetrasCredencialAuxiliar;
                                FotoDetrasCredencial = foto;
                            }
                        }
                        else
                        {
                            if (FotoPersonaExternaAuxiliar != null)
                            {
                                var foto = FotoPersonaExternaAuxiliar;
                                FotoPersonaExterna = foto;
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
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la ventana.", ex);
                    }
                    break;
                #endregion

                #region MENU
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "ayuda_menu":
                    break;
                case "buscar_menu":
                    SelectPersonaAuxiliar = SelectPersona;
                    TextNombre = TextPaterno = TextMaterno = string.Empty;
                    ListPersonas = null;
                    SelectPersona = null;
                    //var personn = new SSP.Servidor.PERSONA();
                    //personn = SelectPersona;
                    //SelectPersonaAuxiliar = personn;
                    //ListPersonas = new ObservableCollection<SSP.Servidor.PERSONA>();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    break;
                case "guardar_menu":
                    try
                    {
                        SetValidacionesAgregarNuevaVisitaExterna();
                        //if (base.HasErrors)
                        //{
                        //    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos.");
                        //    break;
                        //}
                        if (base.HasErrors)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación!", string.Format("Validar el campo: {0}.", base.Error));
                            break;
                        }
                        Guardar();
                        #region Comentado
                        //var persona = new SSP.Servidor.PERSONA()
                        //{
                        //    ID_PERSONA = ExisteNuevo ? SelectPersona.ID_PERSONA : int.Parse(Fechas.GetFechaDateServer.Year + "" + new cPersona().GetSequence<int>("ID_PERSONA_SEQ")),
                        //    NOMBRE = TextNombreNuevo,
                        //    PATERNO = TextPaternoNuevo,
                        //    MATERNO = TextMaternoNuevo,
                        //    CORREO_ELECTRONICO = TextCorreoNuevo,
                        //    DOMICILIO_CALLE = TextCalle,
                        //    DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                        //    DOMICILIO_NUM_EXT = TextNumExt,
                        //    DOMICILIO_NUM_INT = TextNumInt,
                        //    FEC_NACIMIENTO = FechaNacimientoNuevo,
                        //    ID_PAIS = SelectPais,
                        //    ID_ENTIDAD = SelectEntidad,
                        //    ID_MUNICIPIO = SelectMunicipio,
                        //    ID_COLONIA = SelectColonia,
                        //    ID_TIPO_PERSONA = 4,
                        //    ID_TIPO_DISCAPACIDAD = SelectDiscapacitadoNuevo == "S" ? SelectDiscapacidadNuevo : 0,
                        //    SEXO = SelectSexoNuevo,
                        //    TELEFONO = TextTelefonoFijoNuevo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                        //    TELEFONO_MOVIL = TextTelefonoMovilNuevo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),

                        //    CORIGINAL = ExisteNuevo ? SelectPersona.CORIGINAL : null,
                        //    CURP = ExisteNuevo ? SelectPersona.CURP : null,
                        //    ID_ETNIA = ExisteNuevo ? SelectPersona.ID_ETNIA : new Nullable<short>(),
                        //    IFE = ExisteNuevo ? SelectPersona.IFE : null,
                        //    LUGAR_NACIMIENTO = ExisteNuevo ? SelectPersona.LUGAR_NACIMIENTO : null,
                        //    NACIONALIDAD = ExisteNuevo ? SelectPersona.NACIONALIDAD : new Nullable<short>(),
                        //    NORIGINAL = ExisteNuevo ? SelectPersona.NORIGINAL : null,
                        //    RFC = ExisteNuevo ? SelectPersona.RFC : null,
                        //    SMATERNO = ExisteNuevo ? SelectPersona.SMATERNO : null,
                        //    SNOMBRE = ExisteNuevo ? SelectPersona.SNOMBRE : null,
                        //    SPATERNO = ExisteNuevo ? SelectPersona.SPATERNO : null
                        //};

                        //#region NIP
                        //var personaNIP = new PERSONA_NIP();
                        //if (ExisteNuevo)
                        //{
                        //    var VisitaExterna = Parametro.ID_TIPO_VISITA_EXTERNA;
                        //    if (!SelectPersona.PERSONA_NIP.Any(w => w.ID_TIPO_VISITA == VisitaExterna && w.ID_CENTRO == GlobalVar.gCentro))
                        //    {
                        //        personaNIP = new PERSONA_NIP()
                        //        {
                        //            NIP = new cPersona().GetSequence<int>("NIP_SEQ"),
                        //            ID_TIPO_VISITA = VisitaExterna,
                        //            ID_PERSONA = persona.ID_PERSONA,
                        //            ID_CENTRO = GlobalVar.gCentro
                        //        };
                        //    }
                        //    else
                        //        NipExiste = true;
                        //}

                        //#endregion

                        //#region Externa
                        //var personExterna = new PERSONA_EXTERNO();
                        //if (!SelectPersona.PERSONA_EXTERNO.Any(a => a.ID_CENTRO == GlobalVar.gCentro))
                        //{
                        //    personExterna = new PERSONA_EXTERNO()
                        //    {
                        //        CREDENCIAL_DETRAS = FotoDetrasCredencial,
                        //        CREDENCIAL_FRENTE = FotoFrenteCredencial,
                        //        ID_PERSONA = persona.ID_PERSONA,
                        //        ID_CENTRO = GlobalVar.gCentro,
                        //        ESTATUS = "S",
                        //        ID_TIPO_VISITANTE = SelectTipoVisitanteNuevo,
                        //        OBSERV = TextObservacionNuevo
                        //    };
                        //}
                        //else
                        //{
                        //    if (ExternoCambio)
                        //    {
                        //        personExterna = SelectPersona.PERSONA_EXTERNO.Where(a => a.ID_CENTRO == GlobalVar.gCentro).Select(s => new PERSONA_EXTERNO()
                        //        {
                        //            CREDENCIAL_DETRAS = FotoDetrasCredencial,
                        //            CREDENCIAL_FRENTE = FotoFrenteCredencial,
                        //            ID_PERSONA = s.ID_PERSONA,
                        //            ID_CENTRO = s.ID_CENTRO,
                        //            ESTATUS = s.ESTATUS,
                        //            ID_TIPO_VISITANTE = SelectTipoVisitanteNuevo,
                        //            OBSERV = TextObservacionNuevo
                        //        }).FirstOrDefault();
                        //    }
                        //}
                        //#endregion

                        //#region FOTOS
                        //var personaFotos = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                        //if (ImagesToSave == null ? false : ImagesToSave.Count != 1)
                        //    ImagesToSave = null;
                        //if (ImagesToSave != null)
                        //{
                        //    Application.Current.Dispatcher.Invoke((Action)(delegate
                        //    {
                        //        foreach (var item in ImagesToSave)
                        //        {
                        //            var encoder = new JpegBitmapEncoder();
                        //            encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                        //            encoder.QualityLevel = 100;
                        //            var bit = new byte[0];
                        //            using (MemoryStream stream = new MemoryStream())
                        //            {
                        //                encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                        //                encoder.Save(stream);
                        //                bit = stream.ToArray();
                        //                stream.Close();
                        //            }
                        //            personaFotos.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                        //            {
                        //                BIOMETRICO = bit,
                        //                ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                        //                ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                        //                ID_PERSONA = persona.ID_PERSONA
                        //            });
                        //        }
                        //    }));
                        //}
                        //#endregion

                        //#region HUELLAS
                        //var personaHuellas = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                        //if (HuellasCapturadas != null)
                        //    Application.Current.Dispatcher.Invoke((Action)(delegate
                        //    {
                        //        foreach (var item in HuellasCapturadas)
                        //        {
                        //            personaHuellas.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                        //            {
                        //                BIOMETRICO = item.BIOMETRICO,
                        //                ID_TIPO_BIOMETRICO = (short)item.ID_TIPO_BIOMETRICO,
                        //                ID_FORMATO = (short)item.ID_TIPO_FORMATO,
                        //                ID_PERSONA = persona.ID_PERSONA
                        //            });
                        //        }
                        //    }));
                        //if (HuellasCapturadas == null)
                        //    HuellasCapturadas = new List<PlantillaBiometrico>();
                        //#endregion

                        //if (!new cPersona().InsertarVisitaExternaTransaccion(persona, personaNIP, NipExiste, personExterna, personaFotos, personaHuellas, GlobalVar.gCentro,
                        //    Parametro.ID_TIPO_VISITA_EXTERNA, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO))
                        //{
                        //    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se pudo guardar la informacion.");
                        //    return;
                        //}
                        //await StaticSourcesViewModel.CargarDatosMetodoAsync(async () =>
                        //{
                        //    await GetPersona(persona.ID_PERSONA);
                        //    ImagenVisitanteExterno = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO &&
                        //        w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                        //            SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO &&
                        //                w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();
                        //});
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
                    }
                    break;
                case "nuevo_menu":
                    ValidacionesActivas = true;
                    LimpiarAgregarVisitaExterna();
                    SetValidacionesAgregarNuevaVisitaExterna();
                    //OnPropertyChanged();
                    SelectFrenteDetrasFoto = "F";
                    //Existente = false;
                    ImagenVisitaExterna = new Imagenes().getImagenPerson();
                    FotoFrenteCredencial = FotoDetrasCredencial = FotoPersonaExterna = null;
                    SelectPersona = null;
                    ValidarEnabled = true;
                    await OnBuscarPorHuellaInicio();
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "imprimir_menu":

                    #region Validacion
                    if (SelectPersona == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un visitante externo");
                        break;
                    }
                    else 
                    {
                        var x = new cPersona().Obtener(SelectPersona.ID_PERSONA).FirstOrDefault();
                        if (x != null)
                        {
                            if (x.ID_TIPO_DISCAPACIDAD != null)
                            {
                                //Foto
                                if (x.PERSONA_BIOMETRICO.Count(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO) == 0)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar fotografía antes de imprimir gafete");
                                    break;
                                }
                                if (x.TIPO_DISCAPACIDAD.HUELLA == "S")
                                {
                                    if (x.PERSONA_BIOMETRICO == null)
                                    {
                                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar sus huellas antes de imprimir gafete");
                                        break;
                                    }
                                    else
                                    {
                                        //Huellas
                                        int[] f = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                                        if (x.PERSONA_BIOMETRICO.Count(w => f.Contains(w.ID_TIPO_BIOMETRICO)) == 0)
                                        {
                                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar sus huellas antes de imprimir gafete");
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {

                                //Foto
                                if (x.PERSONA_BIOMETRICO.Count(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO) == 0)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar fotografía antes de imprimir gafete");
                                    break;
                                }

                                //Huellas
                                int[] f = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                                if (x.PERSONA_BIOMETRICO.Count(w => f.Contains(w.ID_TIPO_BIOMETRICO)) == 0)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar sus huellas antes de imprimir gafete");
                                    break;
                                }
                                
                            }
                        }
                    }
                    #endregion

                    if (!GafeteFrente)
                    {
                        GafeteFrente = true;
                    }
                    CrearGafete();
                    break;
                case "limpiar_menu":
                    StaticSourcesViewModel.SourceChanged = false;
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new PadronVisitaExternaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.RegistroVisitaExternaViewModel();
                    break;
                #endregion

                #region Huellas
                case "capturar_huellas_visita_externa":
                    SelectSexoNuevo = "M";
                    SelectTipoVisitanteNuevo = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                    this.ShowIdentification();
                    break;
                #endregion

                case "print_pvc":
                    Reporteador.PrintDialog();
                    break;

                //case "Registro_Correspondencia":
                //    PopUpsViewModels.ShowPopUp(new CorrespondenciasViewModel(), PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                //    break;
            }
        }

        private void CrearGafete()
        {

            #region VALIDACIONES
            if (SelectPersona == null)
            {
                (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar o crear un visitante.");
                return;
            }
            if (string.IsNullOrEmpty(SelectPersona.TELEFONO) && string.IsNullOrEmpty(SelectPersona.TELEFONO_MOVIL))
            {
                (new Dialogos()).ConfirmacionDialogo("Validación", "Se debe registrar un telefono del visitante.");
                return;
            }
            var fotos = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG);
            if (!fotos.Any())
            {
                (new Dialogos()).ConfirmacionDialogo("Validación", "Debes tomarle foto al visitante.");
                return;
            }
            //Modificacion de modelo, PENDIENTE
            if (SelectPersona.PERSONA_EXTERNO == null)
            {
                (new Dialogos()).ConfirmacionDialogo("Validación", "El visitante no tiene registro.");
                return;
            }
            #endregion

            #region GAFETE
            List<GafeteVisitaExterna> gafetes = new List<GafeteVisitaExterna>();
            gafeteView = new GafetesPVCView();
            var gaf = new GafeteVisitaExterna();
            var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
            gaf.CERESO = centro.DESCR.Trim();
            gaf.DirectorCentro = centro.DIRECTOR;
            gaf.Discapacidad = SelectPersona.ID_TIPO_DISCAPACIDAD == null || SelectPersona.ID_TIPO_DISCAPACIDAD == 0 ? "NINGUNA" : SelectPersona.TIPO_DISCAPACIDAD.DESCR;

            gaf.Fecha = centro.MUNICIPIO.ENTIDAD.DESCR + " A " + Fechas.GetFechaDateServer.ToString("dd DE MMM de yyyy");
            gaf.ImagenVisitante = fotos.FirstOrDefault().BIOMETRICO;
            //gaf.NIP = nip.FirstOrDefault().NIP.ToString();
            gaf.Fecha = Fechas.GetFechaDateServer.ToString("B.C. A dd DE MMMM DE yyyy").ToUpper();
            gaf.NombreVisitante = SelectPersona.NOMBRE.Trim() + " " + SelectPersona.PATERNO.Trim() + " " + SelectPersona.MATERNO.Trim();
            gaf.NumeroCredencial = SelectPersona.ID_PERSONA.ToString();
            gaf.Telefono = string.IsNullOrEmpty(SelectPersona.TELEFONO) ? string.IsNullOrEmpty(SelectPersona.TELEFONO_MOVIL) ? "N/A" : SelectPersona.TELEFONO_MOVIL.Trim() : SelectPersona.TELEFONO.Trim();
            var tipoVisita = "V I S I T A  EXTERNA";
            //foreach (var item in nip.FirstOrDefault().TIPO_VISITA.DESCR.Trim())
            //{
            //    tipoVisita = tipoVisita + item + " ";
            //}
            gaf.TipoVisita = tipoVisita;
            //Modificacion de modelo, PENDIENTE
            if (SelectPersona.PERSONA_EXTERNO.TIPO_VISITANTE != null)
                gaf.TipoVisitante = SelectPersona.PERSONA_EXTERNO.TIPO_VISITANTE.DESCR;
            else
                gaf.TipoVisitante = string.Empty;

            gaf.Titulo = "SISTEMA ESTATAL PENITENCIARIO";
            Reporteador = gafeteView.GafetesPVCReport;
            Reporteador.LocalReport.ReportPath = "Reportes/" + gafeteExterno + ".rdlc";

            Reporteador.ShowExportButton = false;

            gafetes.Add(gaf);
            var rds = new Microsoft.Reporting.WinForms.ReportDataSource();
            rds.Name = "DataSet1";
            rds.Value = gafetes;

            Reporteador.ShowExportButton = false;

            Reporteador.LocalReport.DataSources.Clear();

            Reporteador.LocalReport.DataSources.Add(rds);
            Reporteador.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            Reporteador.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
            Reporteador.ZoomPercent = 135;
            Reporteador.VerticalScroll.Visible = false;
            Reporteador.HorizontalScroll.Visible = false;
            Reporteador.VerticalScroll.Enabled = false;
            Reporteador.HorizontalScroll.Enabled = false;
            gafeteView.Margin = new Thickness() { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            Reporteador.RefreshReport();
            gafeteView.DataContext = this;
            gafeteView.rbFrente.Checked += GafeteViewChecked;
            gafeteView.rbDetras.Checked += GafeteViewChecked;
            gafeteView.Closed -= GafeteViewClosed;
            gafeteView.Closed += GafeteViewClosed;
            gafeteView.Title = "Impresion De Gafete";
            gafeteView.Owner = PopUpsViewModels.MainWindow;
            gafeteView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            gafeteView.ShowDialog();
            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            #endregion

        }

        private void GafeteViewClosed(object sender, EventArgs e)
        {
            try
            {
                gafeteView.rbFrente.Checked -= GafeteViewChecked;
                gafeteView.rbDetras.Checked -= GafeteViewChecked;
                gafeteView.Hide();
                gafeteView.Close();
                Reporteador = null;
                gafeteView = null;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar la credencial.", ex);
            }
        }

        private void GafeteViewChecked(object sender, EventArgs e)
        {
            try
            {
                if (((System.Windows.Controls.Primitives.ToggleButton)(sender)).IsChecked.Value)
                {
                    var x = ((System.Windows.Controls.Primitives.ToggleButton)(sender)).Name;
                    if (x == "rbDetras")
                        gafeteExterno = "GafeteExternoDetras";
                    else
                        gafeteExterno = "GafeteExternoFrente";
                    gafeteView.GafetesPVCReport.LocalReport.ReportPath = "Reportes/" + gafeteExterno + ".rdlc";
                    gafeteView.GafetesPVCReport.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al marcar", ex);
            }
        }

        private void llenarCombos()
        {
            try
            {
               ListTipoVisitante = new ObservableCollection<TIPO_VISITANTE>(new cTipoVisitante().ObtenerTodos().OrderBy(o => o.DESCR));
               ListDiscapacidades = new ObservableCollection<TIPO_DISCAPACIDAD>(new cTipoDiscapacidad().ObtenerTodos().OrderBy(o => o.DESCR));
               ListPais = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos().OrderBy(o => o.PAIS));
               
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                   ListTipoVisitante.Insert(0, new TIPO_VISITANTE() { DESCR = "SELECCIONE", ID_TIPO_VISITANTE = -1 });
                   ListDiscapacidades.Insert(0, new TIPO_DISCAPACIDAD() { DESCR = "SELECCIONE", ID_TIPO_DISCAPACIDAD = -1 });
                   ListPais.Insert(0, new PAIS_NACIONALIDAD() { PAIS = "SELECCIONE", ID_PAIS_NAC = -1 });
                }));
                #region comentado
                //if (ListTipoVisitante == null ? true : ListTipoVisitante.Count <= 0)
                //{
                //    ListTipoVisitante = new ObservableCollection<TIPO_VISITANTE>(new cTipoVisitante().ObtenerTodos().OrderBy(o => o.DESCR));
                //    ListTipoVisitante.Insert(0, new TIPO_VISITANTE() { DESCR = "SELECCIONE", ID_TIPO_VISITANTE = -1 });
                //}
                //if (ListDiscapacidades == null ? true : ListDiscapacidades.Count <= 0)
                //{
                //    ListDiscapacidades = new ObservableCollection<TIPO_DISCAPACIDAD>(new cTipoDiscapacidad().ObtenerTodos().OrderBy(o => o.DESCR));
                //    ListDiscapacidades.Insert(0, new TIPO_DISCAPACIDAD() { DESCR = "SELECCIONE", ID_TIPO_DISCAPACIDAD = -1 });
                //}
                //if (ListPais == null ? true : ListPais.Count <= 0)
                //{
                //    ListPais = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos().OrderBy(o => o.PAIS));
                //    ListPais.Insert(0, new PAIS_NACIONALIDAD() { PAIS = "SELECCIONE", ID_PAIS_NAC = -1 });
                //}
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla.", ex);
            }
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                base.ClearRules();

                if (obj != null)
                {
                    var textbox = (TextBox)obj;
                    switch (textbox.Name)
                    {
                        case "NombreBuscar":
                            TextNombre = textbox.Text;
                            break;
                        case "PaternoBuscar":
                            TextPaterno = textbox.Text;
                            break;
                        case "MaternoBuscar":
                            TextMaterno = textbox.Text;
                            break;
                    }
                }

                //BuscarPersonasNuevo();
                ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                EmptyBuscarRelacionInternoVisible = ListPersonas.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }

        }

        private async void ClickEnterNuevo(Object obj)
        {
            try
            {
                base.ClearRules();
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }

                if (obj != null)
                {
                    var textbox = (TextBox)obj;
                    switch (textbox.Name)
                    {
                        case "Codigo":
                            TextCodigoNuevo = textbox.Text;
                            break;
                        case "NombreBuscarNuevo":
                            TextNombreNuevo = textbox.Text;
                            break;
                        case "PaternoBuscarNuevo":
                            TextPaternoNuevo = textbox.Text;
                            break;
                        case "MaternoBuscarNuevo":
                            TextMaternoNuevo = textbox.Text;
                            break;
                    }
                }
                SelectPersonaAuxiliar = SelectPersona;
                TextCodigo = TextCodigoNuevo;
                if (string.IsNullOrEmpty(TextCodigo))
                {
                    TextNombre = TextNombreNuevo;
                    TextPaterno = TextPaternoNuevo;
                    TextMaterno = TextMaternoNuevo;
                }
                else
                { 
                    TextNombre = TextPaterno = TextMaterno = string.Empty; 
                }
                //BuscarPersonasNuevo();
                ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                EmptyBuscarRelacionInternoVisible = ListPersonas.Count > 0 ? false : true;
                PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqeuda.", ex);
            }
        }

        private bool HasErrors()
        {
            return base.HasErrors;
        }

        private Task<bool> GetPersona(int pers)
        {
            try
            {
                SelectPersona = new cPersona().Obtener(pers).FirstOrDefault();

                return TaskEx.FromResult(true);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
            return TaskEx.FromResult(true);
        }

        private void LimpiarAgregarVisitaExterna()
        {
            TextNombreNuevo = TextPaternoNuevo = TextMaternoNuevo = TextCorreoNuevo = TextObservacionNuevo = TextCalle =
                TextNumInt = /*TextNIPNuevo =*/ SelectDiscapacitadoNuevo = string.Empty;
            TextTelefonoFijoNuevo = TextTelefonoMovilNuevo = null;// new cCentro().Obtener(4).FirstOrDefault().MUNICIPIO.LADA.ToString();
            TextNumExt = TextCodigoPostal = new Nullable<int>();
            SelectSexoNuevo = string.Empty;
            SelectDiscapacidadNuevo = -1;
            FechaNacimientoNuevo = new Nullable<DateTime>();
            FechaAltaNuevo = Fechas.GetFechaDateServer;
            SelectPersona = null;
            SelectPais = Parametro.PAIS; //82;
            SelectEntidad = Parametro.ESTADO; //2;
            SelectMunicipio = -1;
            ImagenPersona = new Imagenes().getImagenPerson();
            ValidarEnabled = ExisteNuevo = NuevoExterno = ExternoCambio = NipExiste = false;
            base.ClearRules();
            OnPropertyChanged();
        }

        private async void TomarFotoLoad(TomarFotoSenaParticularView Window = null)
        {
            try
            {
                if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible)
                    return;
                if (ComboFrontBackFotoVisible == Visibility.Visible)
                {
                    if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                    CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                    await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });

                    if (FotoFrenteCredencial != null && SelectFrenteDetrasFoto == "F")
                    {
                        CamaraWeb.AgregarImagenControl(Window.ImgSenaParticular, new Imagenes().ConvertByteToImageSource(FotoFrenteCredencial));
                        return;
                    }
                    else if (FotoDetrasCredencial != null && SelectFrenteDetrasFoto == "D")
                    {
                        CamaraWeb.AgregarImagenControl(Window.ImgSenaParticular, new Imagenes().ConvertByteToImageSource(FotoDetrasCredencial));
                        return;
                    }
                    return;
                }
                else
                {
                    if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                    CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                    await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
                    if (FotoPersonaExterna == null)
                        return;
                    else
                    {
                        CamaraWeb.AgregarImagenControl(Window.ImgSenaParticular, new Imagenes().ConvertByteToImageSource(FotoPersonaExterna));
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private async void CapturarFoto(System.Windows.Controls.Image Picture)
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
                    ImagesToSave.Add(new ImageSourceToSave
                    {
                        FrameName = ComboFrontBackFotoVisible == Visibility.Collapsed
                        ?
                           "Persona"
                        :
                            SelectFrenteDetrasFoto == "F"
                            ?
                                "Frontal"
                            :
                                SelectFrenteDetrasFoto == "D"
                                ?
                                    "Trasera"
                                :
                                    "Error",
                        ImageCaptured = (BitmapSource)Picture.Source
                    });
                    if (ComboFrontBackFotoVisible == Visibility.Collapsed)
                        FotoPersonaExterna = new Imagenes().ConvertBitmapToByte((BitmapSource)Picture.Source);
                    else if (SelectFrenteDetrasFoto == "F")
                        FotoFrenteCredencial = new Imagenes().ConvertBitmapToByte((BitmapSource)Picture.Source);
                    else if (SelectFrenteDetrasFoto == "D")
                        FotoDetrasCredencial = new Imagenes().ConvertBitmapToByte((BitmapSource)Picture.Source);
                    ImagesToSave = new List<ImageSourceToSave>();
                    StaticSourcesViewModel.Mensaje(Picture.Name == "Persona"
                        ?
                            "FOTO PERSONA"
                        :
                            SelectFrenteDetrasFoto == "F"
                            ?
                                "FOTO FRONTAL"
                            :
                                SelectFrenteDetrasFoto == "D"
                                ?
                                    "FOTO TRASERA"
                                :
                                    "ERROR",
                    "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    //if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "ESTA SEGÚRO QUE DESEA CAMBIAR LA " +
                    //    (SelectFrenteDetrasFoto.Contains("F") ? "FOTO FRONTAL" : "FOTO TRASERA") + " DE LA IFE?") == 1)
                    if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "¿ESTA SEGÚRO QUE DESEA CAMBIAR LA FOTO?") == 1)
                    {
                        CamaraWeb.QuitarFoto(Picture);
                        ImagesToSave.Remove(ImagesToSave.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                    }
                }
                Processing = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar fotografía.", ex);
            }
        }

        private async void FrenteDetrasImages(TomarFotoSenaParticularView Window)
        {
            if (Window == null)
                return;
            if (string.IsNullOrEmpty(SelectFrenteDetrasFoto))
                return;
            if (CamaraWeb != null)
            {
                await CamaraWeb.ReleaseVideoDevice();
                CamaraWeb = null;
            }
            TomarFotoLoad(Window);
        }

        //private async void BuscarPersonasNuevo()
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(TextNombre))
        //            TextNombre = string.Empty;
        //        if (string.IsNullOrEmpty(TextPaterno))
        //            TextPaterno = string.Empty;
        //        if (string.IsNullOrEmpty(TextMaterno))
        //            TextMaterno = string.Empty;

        //        ListPersonas = new ObservableCollection<SSP.Servidor.PERSONA>(await StaticSourcesViewModel.CargarDatosAsync<IQueryable<SSP.Servidor.PERSONA>>(() =>
        //            new cPersona().ObtenerXNombreYNIP(TextNombre, TextPaterno, TextMaterno, 0)));
        //        if (ListPersonas.Count > 0)
        //            EmptyBuscarRelacionInternoVisible = false;
        //        else
        //            EmptyBuscarRelacionInternoVisible = true;
        //        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
        //        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
        //    }
        //}

        private void GetDatosNuevoPadronVisitanteSeleccionado()
        {
            try
            {
                TextCodigoNuevo = SelectPersona.ID_PERSONA.ToString();
                TextNombreNuevo = !string.IsNullOrEmpty(SelectPersona.NOMBRE) ? SelectPersona.NOMBRE.Trim() : string.Empty;
                TextPaternoNuevo = !string.IsNullOrEmpty(SelectPersona.PATERNO) ? SelectPersona.PATERNO.Trim() : string.Empty;
                TextMaternoNuevo = !string.IsNullOrEmpty(SelectPersona.MATERNO) ? SelectPersona.MATERNO.Trim() : string.Empty;
                SelectSexoNuevo = SelectPersona.SEXO;
                FechaNacimientoNuevo = SelectPersona.FEC_NACIMIENTO;
                TextTelefonoFijoNuevo =  !string.IsNullOrEmpty(SelectPersona.TELEFONO) ? SelectPersona.TELEFONO.Trim() : string.Empty;
                TextTelefonoMovilNuevo = !string.IsNullOrEmpty(SelectPersona.TELEFONO_MOVIL) ? SelectPersona.TELEFONO_MOVIL.Trim() : string.Empty;
                TextCorreoNuevo = !string.IsNullOrEmpty(SelectPersona.CORREO_ELECTRONICO) ? SelectPersona.CORREO_ELECTRONICO.Trim() : string.Empty;
                //Modificacion de modelo, PENDIENTE
                if (SelectPersona.PERSONA_EXTERNO != null)
                {
                    SelectTipoVisitanteNuevo = SelectPersona.PERSONA_EXTERNO.ID_TIPO_VISITANTE != null ? SelectPersona.PERSONA_EXTERNO.ID_TIPO_VISITANTE : -1;
                    TextObservacionNuevo = SelectPersona.PERSONA_EXTERNO.OBSERV;
                    FotoDetrasCredencial = SelectPersona.PERSONA_EXTERNO.CREDENCIAL_DETRAS;
                    FotoFrenteCredencial = SelectPersona.PERSONA_EXTERNO.CREDENCIAL_FRENTE;
                }
                var VisitaExterna = Parametro.ID_TIPO_VISITA_EXTERNA;
                SelectPais = SelectPersona.ID_PAIS;
                SelectEntidad = SelectPersona.ID_ENTIDAD;
                SelectMunicipio = SelectPersona.ID_MUNICIPIO;
                SelectColonia = SelectPersona.ID_COLONIA;
                TextCalle = !string.IsNullOrEmpty(SelectPersona.DOMICILIO_CALLE) ? SelectPersona.DOMICILIO_CALLE.Trim() : string.Empty;
                TextNumExt = SelectPersona.DOMICILIO_NUM_EXT;
                TextNumInt = !string.IsNullOrEmpty(SelectPersona.DOMICILIO_NUM_INT) ? SelectPersona.DOMICILIO_NUM_INT.Trim() : string.Empty;
                TextCodigoPostal = SelectPersona.DOMICILIO_CODIGO_POSTAL;
                SelectDiscapacitadoNuevo = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value > 0 ? "S" : "N" : "N";
                SelectDiscapacidadNuevo = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value : (short?)0;
                FotoPersonaExterna = ImagenVisitaExterna = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                    SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                        new Imagenes().getImagenPerson();
                //Modificacion de modelo, PENDIENTE
                //FotoDetrasCredencial = SelectPersona.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == GlobalVar.gCentro).Any() ?
                //    SelectPersona.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().CREDENCIAL_DETRAS : null;
                //FotoFrenteCredencial = SelectPersona.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == GlobalVar.gCentro).Any() ?
                //    SelectPersona.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().CREDENCIAL_FRENTE : null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private async void Guardar() 
        {
            try
            {
                var respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando Visita Externa", () =>
                {
                    #region Persona Externa
                    var p = new SSP.Servidor.PERSONA();
                    p.NOMBRE = TextNombreNuevo;
                    p.PATERNO = TextPaternoNuevo;
                    p.MATERNO = TextMaternoNuevo;
                    p.CORREO_ELECTRONICO = TextCorreoNuevo;
                    p.DOMICILIO_CALLE = TextCalle;
                    p.DOMICILIO_CODIGO_POSTAL = TextCodigoPostal;
                    p.DOMICILIO_NUM_EXT = TextNumExt;
                    p.DOMICILIO_NUM_INT = TextNumInt;
                    p.FEC_NACIMIENTO = FechaNacimientoNuevo;
                    p.ID_PAIS = SelectPais;
                    p.ID_ENTIDAD = SelectEntidad;
                    p.ID_MUNICIPIO = SelectMunicipio;
                    p.ID_COLONIA = SelectColonia;
                    p.ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EXTERNA;
                    p.ID_TIPO_DISCAPACIDAD = SelectDiscapacitadoNuevo == "S" ? SelectDiscapacidadNuevo : null;
                    p.SEXO = SelectSexoNuevo;
                    p.TELEFONO = TextTelefonoFijoNuevo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                    p.TELEFONO_MOVIL = TextTelefonoMovilNuevo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                    #endregion

                    #region PersonaExterno
                    var pe = new PERSONA_EXTERNO();
                    pe.ID_CENTRO = GlobalVar.gCentro;
                    pe.ESTATUS = "S";
                    pe.CREDENCIAL_FRENTE = FotoFrenteCredencial;
                    pe.CREDENCIAL_DETRAS = FotoDetrasCredencial;
                    pe.OBSERV = TextObservacionNuevo;
                    pe.ID_TIPO_VISITANTE = SelectTipoVisitanteNuevo;
                    #endregion

                    #region Fotos
                    var personaFotos = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            if (FotoPersonaExterna != null)
                            {
                                personaFotos.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                {
                                    BIOMETRICO = FotoPersonaExterna,
                                    ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                                    ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                                    ID_PERSONA = p.ID_PERSONA
                                });
                            }
                            //foreach (var item in ImagesToSave)
                            //{
                            //    var encoder = new JpegBitmapEncoder();
                            //    encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                            //    encoder.QualityLevel = 100;
                            //    var bit = new byte[0];
                            //    using (MemoryStream stream = new MemoryStream())
                            //    {
                            //        encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                            //        encoder.Save(stream);
                            //        bit = stream.ToArray();
                            //        stream.Close();
                            //    }
                            //    personaFotos.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                            //    {
                            //        BIOMETRICO = bit,
                            //        ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                            //        ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                            //        ID_PERSONA = p.ID_PERSONA
                            //    });
                            //}
                        }));
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
                                    ID_PERSONA = p.ID_PERSONA
                                });
                            }
                        }));
                    }
                    #endregion

                    if (SelectPersona == null)//insert
                    {
                        #region Persona
                        p.ID_PERSONA = int.Parse(Fechas.GetFechaDateServer.Year + "" + new cPersona().GetSequence<int>("ID_PERSONA_SEQ"));
                        //Modificacion de modelo, PENDIENTE
                        p.PERSONA_EXTERNO = pe;
                        p.PERSONA_BIOMETRICO = personaFotos;
                        #endregion

                        #region NIP
                        //var pn = new List<PERSONA_NIP>();
                        //var nip = new cPersona().GetSequence<int>("NIP_SEQ");
                        //pn.Add(new PERSONA_NIP()
                        //{
                        //    ID_CENTRO = GlobalVar.gCentro,
                        //    ID_TIPO_VISITA = 4,
                        //    NIP = nip,
                        //});
                        //p.PERSONA_NIP = pn;
                        #endregion

                        if (new cPersona().InsertarPersona(p))
                        {
                            SelectPersona = new cPersona().Obtener(p.ID_PERSONA).FirstOrDefault();
                            //TextNIPNuevo = nip.ToString();
                            TextCodigoNuevo = p.ID_PERSONA.ToString();
                            return true;
                        }
                    }
                    else//update
                    {
                        #region Persona
                        p.ID_PERSONA = SelectPersona.ID_PERSONA;
                        p.ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EXTERNA;
                        p.CURP = SelectPersona.CURP;
                        p.RFC = SelectPersona.RFC;
                        p.LUGAR_NACIMIENTO = SelectPersona.LUGAR_NACIMIENTO;
                        p.NACIONALIDAD = SelectPersona.NACIONALIDAD;
                        p.ESTADO_CIVIL = SelectPersona.ESTADO_CIVIL;
                        p.ID_ETNIA = SelectPersona.ID_ETNIA;
                        p.IFE = SelectPersona.IFE;
                        p.NORIGINAL = SelectPersona.NORIGINAL;
                        p.CORIGINAL = SelectPersona.CORIGINAL;

                        if (personaFotos != null)
                        {
                            foreach (var f in personaFotos)
                            {
                                f.ID_PERSONA = SelectPersona.ID_PERSONA;
                            }
                        }
                        #endregion

                        #region Persona Externo
                        //var externo = new PERSONA_EXTERNO();
                        //if (pe != null)
                        //{
                        //    foreach (var x in pe)
                        //        x.ID_PERSONA = p.ID_PERSONA;
                        //    externo = pe[0];
                        //}
                        pe.ID_PERSONA = p.ID_PERSONA;
                        #endregion

                        if (new cPersona().ActualizarVisitaExterno(p, personaFotos, pe))
                        {
                            SelectPersona = new cPersona().Obtener(p.ID_PERSONA).FirstOrDefault();
                            return true;
                        }
                    }
                    return false;
                });
                if (respuesta)
                {
                    new Dialogos().ConfirmacionDialogo("Éxito", "La visita externa se guardo correctamente");
                    StaticSourcesViewModel.SourceChanged = false;
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar visita externa");
                }
              
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private async Task<List<SSP.Servidor.PERSONA>> SegmentarPersonasBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                        new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerXNombreYNIP(TextNombre, TextPaterno, TextMaterno, !string.IsNullOrEmpty(TextCodigo) ? int.Parse(TextCodigo) : 0)));
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

        #region [Huellas Digitales]
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

                                //if (new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && w.ID_TIPO_BIOMETRICO >= 11 && w.ID_TIPO_BIOMETRICO <= 20).ToList().Any())
                                //    ScannerMessage = "Huellas Actualizadas Correctamente";
                                //else
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
                windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_TODOS, nRet == 0, requiereGuardarHuellas);
                windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Persona")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "TIPO VISITA"
                });
                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLAS DIGITALES", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }

                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Escape)windowBusqueda.Close(); };
                windowBusqueda.Closed += async (s, e) =>
                {
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    var huella = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (!huella.IsSucceed)
                        return;

                    if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                    {
                        //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                        ValidacionesActivas = true;
                        LimpiarAgregarVisitaExterna();
                        SetValidacionesAgregarNuevaVisitaExterna();
                        //OnPropertyChanged();
                        SelectFrenteDetrasFoto = "F";
                        //Existente = false;
                        ImagenVisitaExterna = new Imagenes().getImagenPerson();
                        FotoFrenteCredencial = FotoDetrasCredencial = FotoPersonaExterna = null;
                        SelectPersona = null;
                        ValidarEnabled = true;
                        StaticSourcesViewModel.SourceChanged = false;
                        return;
                    }

                    if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null)
                        return;

                    SelectPersona = huella.SelectRegistro.Persona;
                    //Modificacion de modelo, PENDIENTE
                    if (SelectPersona.PERSONA_EXTERNO != null)
                    {
                        SetValidacionesAgregarNuevaVisitaExterna();
                        FotoDetrasCredencial = FotoDetrasCredencialAuxiliar = FotoFrenteCredencial = FotoFrenteCredencialAuxiliar =
                            FotoPersonaExterna = null;
                        GetDatosNuevoPadronVisitanteSeleccionado();
                        ValidarEnabled = ExisteNuevo = true;
                        NuevoExterno = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    }
                    else
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "LA PERSONA SELECCIONADA NO ESTA REGISTRADA COMO EXTERNA,"
                                + " DESEA REGISTRARLA AHORA?") == 1)
                        {
                            SetValidacionesAgregarNuevaVisitaExterna();
                            GetDatosNuevoPadronVisitanteSeleccionado();
                            ValidarEnabled = ExisteNuevo = NuevoExterno = true;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
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
                windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_TODOS, nRet == 0, requiereGuardarHuellas);
                windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Persona")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "TIPO VISITA"
                });
                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLAS DIGITALES", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }

                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Escape)windowBusqueda.Close(); };
                windowBusqueda.Closed += async (s, e) =>
                {
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    var huella = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (!huella.IsSucceed)
                        return;

                    if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                    {
                        //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                        ValidacionesActivas = true;
                        LimpiarAgregarVisitaExterna();
                        SetValidacionesAgregarNuevaVisitaExterna();
                        //OnPropertyChanged();
                        SelectFrenteDetrasFoto = "F";
                        //Existente = false;
                        ImagenVisitaExterna = new Imagenes().getImagenPerson();
                        FotoFrenteCredencial = FotoDetrasCredencial = FotoPersonaExterna = null;
                        SelectPersona = null;
                        ValidarEnabled = true;
                        StaticSourcesViewModel.SourceChanged = false;
                        return;
                    }

                    if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null)
                        return;

                    SelectPersona = huella.SelectRegistro.Persona;
                    //Modificacion de modelo, PENDIENTE
                    if (SelectPersona.PERSONA_EXTERNO != null)
                    {
                        SetValidacionesAgregarNuevaVisitaExterna();
                        FotoDetrasCredencial = FotoDetrasCredencialAuxiliar = FotoFrenteCredencial = FotoFrenteCredencialAuxiliar =
                            FotoPersonaExterna = null;
                        GetDatosNuevoPadronVisitanteSeleccionado();
                        ValidarEnabled = ExisteNuevo = true;
                        NuevoExterno = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    }
                    else
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "LA PERSONA SELECCIONADA NO ESTA REGISTRADA COMO EXTERNA,"
                                + " DESEA REGISTRARLA AHORA?") == 1)
                        {
                            SetValidacionesAgregarNuevaVisitaExterna();
                            GetDatosNuevoPadronVisitanteSeleccionado();
                            ValidarEnabled = ExisteNuevo = NuevoExterno = true;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        }
                        //else { }
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

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.VISITA_EXTERNA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

    }
}
