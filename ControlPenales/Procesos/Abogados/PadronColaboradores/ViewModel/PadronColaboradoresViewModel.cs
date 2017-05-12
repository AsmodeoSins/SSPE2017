using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Controlador.Catalogo.Justicia;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Windows;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Threading;
using System.Windows.Controls;
using Cogent.Biometrics;
using System.Runtime.InteropServices;
using DPUruNet;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using WPFPdfViewer;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class PadronColaboradoresViewModel : FingerPrintScanner
    {
        public PadronColaboradoresViewModel() { }

        private async void Load_Window(PadronColaboradoresView Window)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarDatos(); StaticSourcesViewModel.SourceChanged = false; });
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

                if (SelectColaborador != null)
                {
                    CamaraWeb.AgregarImagenControl(Window.ImgFrente, new Imagenes().ConvertByteToBitmap(SelectColaborador.ABOGADO.IFE_FRONTAL));
                    CamaraWeb.AgregarImagenControl(Window.ImgReverso, new Imagenes().ConvertByteToBitmap(SelectColaborador.ABOGADO.IFE_REVERSO));
                    CamaraWeb.AgregarImagenControl(Window.ImgFrenteCedula, new Imagenes().ConvertByteToBitmap(SelectColaborador.ABOGADO.CEDULA_FRONTAL));
                    CamaraWeb.AgregarImagenControl(Window.ImgReversoCedula, new Imagenes().ConvertByteToBitmap(SelectColaborador.ABOGADO.CEDULA_REVERSO));
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla para tomar foto.", ex);
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
                    if (await new Dialogos().ConfirmarEliminar("Validación", "Esta segúro que desea cambiar la " +
                    (Picture.Name.Contains("ImgFrente") ? "foto de frente" : Picture.Name.Contains("ImgReverso") ? "foto trasera" : "") + "?") == 1)
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
            try
            {
                CamaraWeb.AdvanceSetting();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la configuración de la cámara.", ex);
            }
        }
        #endregion

        private void CargarDatos()
        {
            try
            {
                ListPais = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos());
                ListTipoDiscapacidad = new ObservableCollection<TIPO_DISCAPACIDAD>(new cTipoDiscapacidad().ObtenerTodos());
                ConfiguraPermisos();
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
                    SelectEstatus = -1;
                    SelectDiscapacitado = string.Empty;
                    SelectPais = Parametro.PAIS; //82;
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.PADRON_COLABORADORES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

        private bool GuardarAsignacion(int persona)
        {
            try
            {
                bool EsAdministrativo = false;
                if (ListCausasPenales == null || ListCausasPenales.Count(w => w.ELEGIDO == true) == 0)
                    EsAdministrativo = true;
                if (SelectColaborador == null)
                    SelectColaborador = new cPersona().ObtenerPersonaXID(persona).FirstOrDefault();
                var aboIng = new cAbogadoIngreso().ObtenerXAbogado(SelectColaborador.ID_PERSONA);
                var bandera = true;
                var abogadoIngreso = new ABOGADO_INGRESO();
                var hoy = Fechas.GetFechaDateServer;
               
                #region ABOGADO_INGRESO
                foreach (var item in aboIng)
                {
                    if (
                        item.ID_CENTRO == SelectIngreso.ID_CENTRO && 
                        item.ID_ANIO == SelectIngreso.ID_ANIO &&
                        item.ID_ABOGADO_TITULO == SelectColaborador.ABOGADO.ID_ABOGADO_TITULO && 
                        item.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO &&
                        item.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                        item.ID_ABOGADO_TITULAR == SelectAbogadoTitular.ID_PERSONA
                       )
                    {
                        abogadoIngreso = new ABOGADO_INGRESO
                        {
                            ID_ABOGADO = item.ID_ABOGADO,
                            ID_ABOGADO_TITULO = SelectColaborador.ABOGADO.ID_ABOGADO_TITULO,
                            ID_CENTRO = item.ID_CENTRO,
                            ID_ANIO = item.ID_ANIO,
                            ID_IMPUTADO = item.ID_IMPUTADO,
                            ID_INGRESO = item.ID_INGRESO,
                            CAPTURA_FEC = item.CAPTURA_FEC,
                            ID_ESTATUS_VISITA = SelectEstatus,
                            OBSERV = TextObservacionesInterno,
                            ID_ABOGADO_TITULAR = SelectAbogadoTitular.ID_PERSONA,
                        };
                        //new cAbogadoIngreso().Actualizar(abogadoIngreso);
                        bandera = false;
                        break;
                    }
                    else
                        bandera = true;
                }
                if (bandera)
                {
                    abogadoIngreso = new ABOGADO_INGRESO
                    {
                        ID_ABOGADO = SelectColaborador.ABOGADO.ID_ABOGADO,
                        ID_ABOGADO_TITULO = SelectColaborador.ABOGADO.ID_ABOGADO_TITULO,
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        CAPTURA_FEC = hoy,
                        ID_ESTATUS_VISITA = SelectEstatus,
                        OBSERV = TextObservacionesInterno,
                        ID_ABOGADO_TITULAR = SelectAbogadoTitular.ID_PERSONA
                    };
                    //new cAbogadoIngreso().Insertar(abogadoIngreso);
                }
                #endregion

                #region ABOGADO_CAUSAS_PENALES
                //if (ListCausasPenales.Where(w => w.ELEGIDO).Any())
                //{
                    var ai = new ABOGADO_INGRESO();
                    var ingresoAsignacion = SelectIngreso;
                    var causasPenales = new List<ABOGADO_CAUSA_PENAL>();
                    if (ListIngresosAsignados == null)
                        ListIngresosAsignados = new ObservableCollection<ABOGADO_INGRESO>();
                    var oficios = ListIngresosAsignados.Where(wh => wh.ID_ESTATUS_VISITA != (short)enumEstatusVisita.CANCELADO_A && wh.ABOGADO_CAUSA_PENAL.Where(w => w.ID_CENTRO == abogadoIngreso.ID_CENTRO &&
                            w.ID_ANIO == abogadoIngreso.ID_ANIO && w.ID_IMPUTADO == abogadoIngreso.ID_IMPUTADO && w.ID_INGRESO == abogadoIngreso.ID_INGRESO &&
                            w.ID_ABOGADO_TITULO == SelectColaborador.ABOGADO.ID_ABOGADO_TITULO).Any()).ToList();

                    #region CAUSAS_PENALES Seleccionadas
                    if (ListIngresosAsignados.Any())
                    {
                        foreach (var itm in ListCausasPenales.Where(w => w.ELEGIDO).Select(s => s.CAUSA_PENAL))
                        {
                            if (!oficios.Any())
                            {
                                causasPenales.Add(new ABOGADO_CAUSA_PENAL
                                {
                                    CAPTURA_FEC = hoy,
                                    ID_ABOGADO = abogadoIngreso.ID_ABOGADO,
                                    ID_ABOGADO_TITULO = SelectColaborador.ABOGADO.ID_ABOGADO_TITULO,
                                    ID_ANIO = itm.ID_ANIO,
                                    ID_CAUSA_PENAL = itm.ID_CAUSA_PENAL,
                                    ID_CENTRO = itm.ID_CENTRO,
                                    ID_IMPUTADO = itm.ID_IMPUTADO,
                                    ID_INGRESO = itm.ID_INGRESO,
                                    ID_ESTATUS_VISITA = SelectEstatus,
                                    ID_ABOGADO_TITULAR = SelectAbogadoTitular.ID_PERSONA
                                });
                            }
                            else
                            {
                                if (!oficios.FirstOrDefault().ABOGADO_CAUSA_PENAL.Where(w => w.ID_CENTRO == itm.ID_CENTRO &&
                                w.ID_ANIO == itm.ID_ANIO && w.ID_IMPUTADO == itm.ID_IMPUTADO && w.ID_INGRESO == itm.ID_INGRESO &&
                                w.ID_CAUSA_PENAL == itm.ID_CAUSA_PENAL && w.ID_ABOGADO_TITULO == SelectColaborador.ABOGADO.ID_ABOGADO_TITULO).Any())
                                {
                                    causasPenales.Add(new ABOGADO_CAUSA_PENAL
                                    {
                                        CAPTURA_FEC = hoy,
                                        ID_ABOGADO = SelectColaborador.ABOGADO.ID_ABOGADO,
                                        ID_ABOGADO_TITULO = SelectColaborador.ABOGADO.ID_ABOGADO_TITULO,
                                        ID_ANIO = itm.ID_ANIO,
                                        ID_CAUSA_PENAL = itm.ID_CAUSA_PENAL,
                                        ID_CENTRO = itm.ID_CENTRO,
                                        ID_IMPUTADO = itm.ID_IMPUTADO,
                                        ID_INGRESO = itm.ID_INGRESO,
                                        ID_ESTATUS_VISITA = SelectEstatus,
                                        ID_ABOGADO_TITULAR = SelectAbogadoTitular.ID_PERSONA
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in ListCausasPenales.Where(w => w.ELEGIDO).Select(s => s.CAUSA_PENAL))
                        {
                            causasPenales.Add(new ABOGADO_CAUSA_PENAL
                            {
                                CAPTURA_FEC = hoy,
                                ID_ABOGADO = SelectColaborador.ABOGADO.ID_ABOGADO,
                                ID_ABOGADO_TITULO = SelectColaborador.ABOGADO.ID_ABOGADO_TITULO,
                                ID_ANIO = item.ID_ANIO,
                                ID_CAUSA_PENAL = item.ID_CAUSA_PENAL,
                                ID_CENTRO = item.ID_CENTRO,
                                ID_IMPUTADO = item.ID_IMPUTADO,
                                ID_INGRESO = item.ID_INGRESO,
                                ID_ESTATUS_VISITA = SelectEstatus,
                                ID_ABOGADO_TITULAR = SelectAbogadoTitular.ID_PERSONA
                            });
                        }
                    }
                    #endregion

                    #region INSERT
                    //if (causasPenales.Count > 0)
                    //{
                        //if (!new cAbogadoCausaPenal().Insertar(causasPenales))
                        if (!new cPersona().InsertarAbogadoAsignacionTransaccion(abogadoIngreso, !bandera, causasPenales))
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No se pudo guardar la información.");
                            return false;
                        }
                    //}
                    #endregion
                //}
                //else
                //{
                //    (new Dialogos()).ConfirmacionDialogo("Validación", "No has seleccionado ninguna causa penal.");
                //    return false;
                //}
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return false;
            }
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                #region MENU
                case "guardar_menu":
                    try
                    {
                        if (SelectColaborador != null ? SelectColaborador.ID_PERSONA <= 0 : true)
                        {
                            if (!Insertable)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debes registrar un nuevo colaborador o seleccionar uno existente para modificarlo.");
                                }));
                                return;
                            }
                        }
                        if (TabRegistro)
                        {
                            if (SelectAbogadoTitular == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un abogado titular.");
                                break;
                            }
                            if (HasErrors())
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "El campo " + base.Error + " es requerido.");
                                break;
                            }

                            //await StaticSourcesViewModel.CargarDatosMetodoAsync(async () =>
                            //{
                                //await GuardarAbogado();
                                GuardarAbogadoNew();
                                StaticSourcesViewModel.SourceChanged = false;
                            //});
                        }
                        else if (TabAsignacion)
                        {
                            #region Validaciones
                            if (SelectIngreso == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                                break;
                            }
                            if (SelectEstatus == -1 ? true : !ListEstatus.Where(w => w.ID_ESTATUS_VISITA == SelectEstatus).Any())
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un estatus valido.");
                                break;
                            }
                            //if (ListCausasPenales != null ? ListCausasPenales.Count <= 0 : true)
                            //{
                            //    (new Dialogos()).ConfirmacionDialogo("Validación", "Este imputado no tiene causas penales.");
                            //    break;
                            //}
                            //Valida causa penal seleccionada
                            //if (ListCausasPenales != null)
                            //{
                            //    if (ListCausasPenales.Count(w => w.ELEGIDO == true) == 0)
                            //    {
                            //        (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar una causa penal.");
                            //        break;
                            //    }
                            //}
                            #endregion

                            if (!GuardarAsignacion(SelectColaborador.ID_PERSONA))
                                break;

                            Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                            {
                                var colab = SelectColaborador;
                                LimpiarAsignacion();
                                //LimpiarCampos();
                                SelectColaborador = new cPersona().ObtenerPersonaXID(colab.ID_PERSONA).FirstOrDefault();
                                Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                SetValidaciones();
                                await GetDatosPersonaSeleccionada();
                                StaticSourcesViewModel.SourceChanged = false;
                                (new Dialogos()).ConfirmacionDialogo("Éxito!", "Información grabada exitosamente.");
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar .", ex);
                    }
                    break;
                case "nuevo_menu":
                    break;
                case "insertar_menu":
                    if (TabRegistro)
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("Validación",
                                "Existen cambios sin guardar, esta seguro que desea insertar a uno nuevo colaborador?") != 1)
                                return;
                        }
                        base.ClearRules();
                        MenuGuardarEnabled = true;
                        DiscapacitadoEnabled = NuevoAbogado = Editable = ValidarEnabled = false;
                        LimpiarAsignacion();
                        ListIngresosAsignados = null;
                        LimpiarCampos();
                        AbogadoTitularEnabled = true;
                        EditarTitularEnabled = false;
                        SetValidaciones();
                        Insertable = true;
                        SelectEstatusVisita = (short)enumEstatusVisita.AUTORIZADO;
                        SelectDiscapacitado = "N";
                        await OnBuscarPorHuellaInicio();
                    }
                    break;
                case "borrar_menu":
                    break;
                case "buscar_menu":
                    try
                    {
                        if (TabRegistro)
                        {
                            TextNombre = TextMaterno = TextPaterno = string.Empty;
                            SelectPersonaAuxiliar = SelectColaborador;
                            SelectTitularAuxiliar = SelectPersona;
                            ListPersonas = null;
                            SelectColaborador = null;
                            SelectPersona = null;
                            AbogadoTitular = false;
                            if (!PConsultar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                return;
                            }
                            Editable = true;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            #region comentado
                            //SelectPersonaAuxiliar = SelectColaborador;
                            //ListPersonas = null;
                            //SelectColaborador = null;
                            //TextNombre = TextMaterno = TextPaterno = string.Empty;
                            ////TextNombre = TextNombreAbogado;
                            ////TextMaterno = TextMaternoAbogado;
                            ////TextPaterno = TextPaternoAbogado;
                            ////var pers3 = SelectColaborador;
                            //////await StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarPersonasSinCodigo);
                            ////SelectPersonaAuxiliar = pers3;
                            //Editable = true;
                            //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            #endregion
                        }
                        else if (TabAsignacion)
                        {
                            var exp1 = SelectExpediente;
                            var ing1 = SelectIngreso;
                            SelectIngresoAuxiliar = ing1;
                            SelectExpedienteAuxiliar = exp1;
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                            EmptyExpedienteVisible = true;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla de búsqueda.", ex);
                    }
                    break;
                case "limpiar_menu":
                    base.ClearRules();
                    if (TabRegistro)
                    {
                        //ValidarEnabled = NuevoAbogado = Editable = DiscapacitadoEnabled = false;
                        //LimpiarCampos();
                        StaticSourcesViewModel.SourceChanged = false;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new PadronColaboradoresView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new PadronColaboradoresViewModel();
                    }
                    if (TabAsignacion)
                    {
                        LimpiarAsignacion();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
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
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    ImagenAbogadoAuxiliar = aux;
                    TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
                    break;
                case "cancelar_tomar_foto_senas":
                    var imgAux = ImagenAbogadoAuxiliar;
                    ImagenAbogado = imgAux;
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
                    break;
                case "aceptar_tomar_foto_senas":
                    if (ImagenAbogado != new Imagenes().getImagenPerson() && (ImageFrontal != null ? ImageFrontal.Count == 1 : false))
                    {
                        FotoTomada = true;
                        FotoActualizada = Editable ? (ImagenAbogado != new Imagenes().ConvertBitmapToByte(ImageFrontal.FirstOrDefault().ImageCaptured)) : true;
                        ImagenAbogado = new Imagenes().ConvertBitmapToByte(ImageFrontal.FirstOrDefault().ImageCaptured);
                        StaticSourcesViewModel.SourceChanged = true;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        if (CamaraWeb != null)
                        {
                            await CamaraWeb.ReleaseVideoDevice();
                            CamaraWeb = null;
                        }
                        break;
                    }
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de tomar una foto.");
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
                                    var abogado = new ABOGADO
                                    {
                                        ID_ABOGADO = SelectColaborador.ID_PERSONA,
                                        //ID_TIPO_ABOGADO = SelectColaborador.ABOGADO.ID_TIPO_ABOGADO,
                                        ID_ABOGADO_TITULO = SelectColaborador.ABOGADO.ID_ABOGADO_TITULO,
                                        IFE_FRONTAL = SelectColaborador.ABOGADO.IFE_FRONTAL,
                                        IFE_REVERSO = SelectColaborador.ABOGADO.IFE_REVERSO,
                                        CEDULA = SelectColaborador.ABOGADO.CEDULA,
                                        CEDULA_FRONTAL = SelectColaborador.ABOGADO.CEDULA_FRONTAL,
                                        CEDULA_REVERSO = SelectColaborador.ABOGADO.CEDULA_REVERSO,
                                        NORIGINAL = SelectColaborador.ABOGADO.NORIGINAL,
                                        ALTA_FEC = SelectColaborador.ABOGADO.ALTA_FEC,
                                        OBSERV = SelectColaborador.ABOGADO.OBSERV,
                                        ULTIMA_MOD_FEC = Fechas.GetFechaDateServer,
                                        ID_ESTATUS_VISITA = SelectColaborador.ABOGADO.ID_ESTATUS_VISITA,
                                        CJF = SelectColaborador.ABOGADO.CJF,
                                        CREDENCIALIZADO = "S",
                                        ID_JUZGADO = SelectColaborador.ABOGADO.ID_JUZGADO,
                                        ABOGADO_TITULAR = SelectPersona.ID_PERSONA
                                    };
                                    new cAbogado().Actualizar(abogado);
                                    SelectColaborador.ABOGADO.CREDENCIALIZADO = "S";
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
                    if (SelectColaborador != null)
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarListaTipoDocumentoDigitalizado(); });
                    }
                    else
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "No ha seleccionado un imputado.");
                    DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                    ObservacionDocumento = string.Empty;
                    break;
                case "Cancelar_digitalizar_documentos":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                    escaner.Hide();
                    DocumentoDigitalizado = null;
                    break;
                case "digitalizar_oficio_asignacion":
                    try
                    {
                        if (SelectColaborador == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "No ha seleccionado una persona.");
                            break;
                        }
                        if (SelectIngreso == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "No ha seleccionado un ingreso.");
                            break;
                        }
                        if (SelectExpediente == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "No ha seleccionado un imputado.");
                            break;
                        }
                        if (SelectCausaAsignada == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Necesita especificar la causa a digitalizar.");
                            break;
                        }

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarListaTipoDocumentoCausaPenalDigitalizado(); });
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de digitalización de documentos.", ex);
                    }
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
                case "cancelar_editar_titular":
                    if (AbogadoTitularEnabled)
                    {
                        EditarTitular = AbogadoTitularEnabled = false;
                        EditarTitularCommandParameter = "editar_titular";
                        SelectAbogadoTitular = SelectPersonaAuxiliar;
                        SelectAbogadoTitular.PATERNO = SelectAbogadoTitular.ABOGADO.PERSONA.PATERNO.Trim();
                        SelectAbogadoTitular.MATERNO = SelectAbogadoTitular.ABOGADO.PERSONA.MATERNO.Trim();
                        SelectAbogadoTitular.NOMBRE = SelectAbogadoTitular.ABOGADO.PERSONA.NOMBRE.Trim();
                        TextCodigoTitular = SelectAbogadoTitular.ID_PERSONA.ToString();
                        TextPaternoTitular = SelectAbogadoTitular.PATERNO;
                        TextMaternoTitular = SelectAbogadoTitular.MATERNO;
                        TextNombreTitular = SelectAbogadoTitular.NOMBRE;
                    }
                    break;
                case "editar_titular":
                    var persaux = SelectAbogadoTitular;
                    EditarTitular = AbogadoTitularEnabled = true;
                    EditarTitularCommandParameter = "cancelar_editar_titular";
                    TextNombreTitular = TextPaternoTitular = TextMaternoTitular = TextCodigoTitular = string.Empty;
                    SelectPersonaAuxiliar = persaux;
                    break;
                case "buscar_titular":
                    var prs = SelectAbogadoTitular;
                    SelectPersonaAuxiliar = prs;
                    AbogadoTitular = true;
                    Editable = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    break;
                case "buscar_abogado":
                    TextNombre = TextMaterno = TextPaterno = string.Empty;
                    SelectPersonaAuxiliar = SelectColaborador;
                    SelectTitularAuxiliar = SelectPersona;
                    ListPersonas = null;
                    SelectColaborador = null;
                    SelectPersona = null;
                    AbogadoTitular = false;
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    Editable = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    break;
                case "seleccionar_buscar_persona":
                    try
                    {
                        if (SelectPersona == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar a una persona.");
                            break;
                        }
                        if (!AbogadoTitular)
                        {
                            if (StaticSourcesViewModel.SourceChanged)
                            {
                                if (await new Dialogos().ConfirmarEliminar("Validación",
                                    "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1)
                                    break;
                            }
                            if (SelectPersona.ABOGADO != null)
                            {
                                if (SelectPersona.ABOGADO.ID_ESTATUS_VISITA == (short)enumEstatusVisita.CANCELADO_A)
                                {
                                    //new Dialogos().ConfirmacionDialogo("Validación","El estatus del abogado seleccionado ha sido cancelado");
                                    //break;
                                    //new Dialogos().ConfirmacionDialogo("Validación", "El estatus del colaborador seleccionado es cancelado, la información sera solo de consulta");
                                    //MenuGuardarEnabled = false;
                                    new Dialogos().ConfirmacionDialogo("Validación", "El estatus del colaborador seleccionado es cancelado");
                                    //MenuGuardarEnabled = false;
                                }
                                else
                                { 
                                    if(PInsertar || PEditar)
                                        MenuGuardarEnabled = true;
                                }
                                if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR)
                                {
                                    var colabo = SelectPersona;
                                    LimpiarCampos();
                                    LimpiarAsignacion();
                                    SelectColaborador = colabo;
                                    NuevoAbogado = false;
                                    Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                    AbogadoTitularEnabled = true;
                                    SetValidaciones();
                                    await GetDatosPersonaSeleccionada();

                                    //if (SelectColaborador.ABOGADO.ABOGADO_TITULAR1 != null)
                                    //{
                                    //    var titular = SelectColaborador.ABOGADO.ABOGADO_TITULAR1.Where(w => w.ESTATUS == "S").FirstOrDefault();
                                    //    if (titular != null)
                                    //    {
                                    //        SelectPersona = titular.ABOGADO1.PERSONA;
                                    //        TextCodigoTitular = SelectPersona.ID_PERSONA.ToString();
                                    //        TextPaternoTitular = SelectPersona.PATERNO;
                                    //        TextMaternoTitular = SelectPersona.MATERNO;
                                    //        TextNombreTitular = SelectPersona.NOMBRE;
                                    //    }
                                    //}
                                    
                                    #region Comentado
                                    //if (SelectColaborador.ABOGADO.ABOGADO_TITULAR != null && SelectColaborador.ABOGADO.ABOGADO_TITULAR != 0)
                                    //{ 
                                    //    SelectPersona = new cPersona().Obtener(SelectColaborador.ABOGADO.ABOGADO_TITULAR.Value).FirstOrDefault();
                                    //    TextCodigoTitular = SelectPersona.ID_PERSONA.ToString();
                                    //    TextPaternoTitular = SelectPersona.PATERNO;
                                    //    TextMaternoTitular = SelectPersona.MATERNO;
                                    //    TextNombreTitular = SelectPersona.NOMBRE;
                                    //}
                                    #endregion

                                    StaticSourcesViewModel.SourceChanged = false;
                                    BuscarAbogadoEnabled = true;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                    Editable = false;
                                }
                                else
                                {
                                    if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO != Parametro.ID_ABOGADO_TITULAR_ABOGADO && SelectPersona.ABOGADO.ID_ABOGADO_TITULO != Parametro.ID_ABOGADO_TITULAR_ACTUARIO)
                                    {
                                        if (await new Dialogos().ConfirmarEliminar("Validación", "El abogado seleccionado no esta registrado como colaborador,"
                                                + " ¿Desea cambiarlo ahora?") == 1)
                                        {
                                            var colabo = SelectPersona;
                                            LimpiarAsignacion();
                                            SelectColaborador = colabo;
                                            NuevoAbogado = true;
                                            Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                            AbogadoTitularEnabled = true;
                                            SetValidaciones();
                                            await GetDatosPersonaSeleccionada();
                                            BuscarAbogadoEnabled = true;
                                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                            Editable = false;
                                        }
                                        else
                                        {
                                            if (SelectColaborador != null)
                                            {
                                                TextCodigoAbogado = SelectColaborador.ID_PERSONA.ToString();
                                                TextNombreAbogado = SelectColaborador.NOMBRE;
                                                TextPaternoAbogado = SelectColaborador.PATERNO;
                                                TextMaternoAbogado = SelectColaborador.MATERNO;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        new Dialogos().ConfirmacionDialogo("Validación", "Un abogado titular o actuario no puede convertirse en colaborador.");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (await new Dialogos().ConfirmarEliminar("Validación", "La persona seleccionada no esta registrada como colaborador,"
                                        + " ¿Desea registrarla ahora?") == 1)
                                {
                                    var colabo = SelectPersona;
                                    LimpiarAsignacion();
                                    SelectColaborador = colabo;
                                    NuevoAbogado = true;
                                    Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                    AbogadoTitularEnabled = true;
                                    SetValidaciones();
                                    await GetDatosPersonaSeleccionada();
                                    BuscarAbogadoEnabled = true;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                    Editable = false;
                                }
                                else {
                                    if (SelectColaborador != null)
                                    {
                                        TextCodigoAbogado = SelectColaborador.ID_PERSONA.ToString();
                                        TextNombreAbogado = SelectColaborador.NOMBRE;
                                        TextPaternoAbogado = SelectColaborador.PATERNO;
                                        TextMaternoAbogado = SelectColaborador.MATERNO;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (SelectPersona.ABOGADO == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar a un abogado titular valido.");
                                break;
                            }
                            if (SelectPersona.ABOGADO.ID_ESTATUS_VISITA == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Este abogado no cuenta con un registro correcto.");
                                break;
                            }
                            if (SelectPersona.ABOGADO.ID_ESTATUS_VISITA != Parametro.ID_ESTATUS_VISITA_AUTORIZADO)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Este abogado se encuentra " + SelectPersona.ABOGADO.ESTATUS_VISITA.DESCR.Trim() + " y no puede ser seleccionado como titular.");
                                break;
                            }
                            SelectAbogadoTitular = SelectPersona;
                            var x = Insertable;
                            //if (!Editable)
                            //{
                            //    var abT = SelectAbogadoTitular;
                            //    SetValidaciones();
                            //    LimpiarAsignacion();
                            //    LimpiarCampos();
                            //    SelectAbogadoTitular = abT;
                            //}
                            ValidarEnabled = BuscarAbogadoEnabled = true;
                            SelectPais = Parametro.PAIS;//82;
                            SelectAbogadoTitular.PATERNO = SelectAbogadoTitular.ABOGADO.PERSONA.PATERNO.Trim();
                            SelectAbogadoTitular.MATERNO = SelectAbogadoTitular.ABOGADO.PERSONA.MATERNO.Trim();
                            SelectAbogadoTitular.NOMBRE = SelectAbogadoTitular.ABOGADO.PERSONA.NOMBRE.Trim();
                            TextCodigoTitular = SelectAbogadoTitular.ID_PERSONA.ToString();
                            TextPaternoTitular = SelectAbogadoTitular.PATERNO;
                            TextMaternoTitular = SelectAbogadoTitular.MATERNO;
                            TextNombreTitular = SelectAbogadoTitular.NOMBRE;
                            AbogadoTitularEnabled = false;
                            EditarTitularEnabled = DiscapacitadoEnabled = true;
                            EditarTitularCommandParameter = "editar_titular";
                            Insertable = x;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            Editable = false;
                        }

                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del abogado seleccionado.", ex);
                    }
                    break;
                case "cancelar_buscar_persona":
                    SelectColaborador = SelectPersonaAuxiliar;
                    if (SelectColaborador == null)
                    { 
                        TextCodigoAbogado = TextNombreAbogado = TextPaternoAbogado = TextMaternoAbogado = string.Empty;
                        ListPersonas = null;
                    }
                    else
                    {
                        TextCodigoAbogado = SelectColaborador.ID_PERSONA.ToString();
                        TextNombreAbogado = SelectColaborador.NOMBRE;
                        TextPaternoAbogado = SelectColaborador.PATERNO;
                        TextMaternoAbogado = SelectColaborador.MATERNO;
                    }
                    SelectPersona = SelectTitularAuxiliar;
                    if (SelectPersona == null)
                        TextCodigoTitular = TextNombreTitular = TextPaternoTitular = TextMaternoTitular = string.Empty;
                    else
                    {
                        TextCodigoTitular = SelectPersona.ID_PERSONA.ToString();
                        TextNombreTitular = SelectPersona.NOMBRE;
                        TextPaternoTitular = SelectPersona.PATERNO;
                        TextMaternoTitular = SelectPersona.MATERNO;
                    }
                    //if (AbogadoTitular)
                    //    SelectAbogadoTitular = SelectPersonaAuxiliar;
                    //else
                    //{
                    //    SelectColaborador = SelectPersonaAuxiliar;
                    //    ListPersonas = null;
                    //    SelectPersonaAuxiliar = null;
                    //    TextNombre = TextPaterno = TextMaterno = string.Empty;
                    //    if (SelectColaborador == null)
                    //    {
                    //        TextNombreAbogado = TextPaternoAbogado = TextMaternoAbogado = string.Empty;
                    //    }
                    //    else
                    //    {
                    //        TextCodigoAbogado = SelectColaborador.ID_PERSONA.ToString();
                    //        TextNombreAbogado = SelectColaborador.NOMBRE;
                    //        TextPaternoAbogado = SelectColaborador.PATERNO;
                    //        TextMaternoAbogado = SelectColaborador.MATERNO;

                    //        SelectPersona = SelectAbogadoTitular;
                    //    }
                    //}
                    if (EditarTitular)
                        EditarTitular = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    Editable = false;
                    break;
                case "buscar_por_huella":
                    break;
                #endregion

                #region BUSCAR_IMPUTADOS
                case "nueva_busqueda":
                    NombreD = NombreBuscar = PaternoD = ApellidoPaternoBuscar = MaternoD = ApellidoMaternoBuscar = AnioD = FolioD = string.Empty;
                    FolioBuscar = AnioBuscar = new Nullable<int>();
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>(); //new ObservableCollection<IMPUTADO>();
                    break;
                case "buscar_visible":
                    var ing = SelectIngreso;
                    SelectIngresoAuxiliar = ing;
                    var exp = SelectExpediente;
                    SelectExpedienteAuxiliar = exp;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    var ingA = SelectIngresoAuxiliar;
                    SelectIngreso = ingA;
                    var expA = SelectExpedienteAuxiliar;
                    SelectExpediente = expA;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    try
                    {
                        if (SelectExpediente == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                            break;
                        }
                        if (SelectIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un ingreso.");
                            break;
                        }
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no esta activo.");
                                return;
                            }
                        }
                        if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a este centro.");
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a =>( a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El interno [" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ANIO.ToString() + "/" +
                                ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no puede recibir visitas.");
                            return;
                        }
                        var expedient = SelectExpediente;
                        var ingres = SelectIngreso;
                        var titulars = SelectAbogadoTitular;
                        LimpiarAsignacion();
                        //LimpiarCampos();
                        SelectIngresoAuxiliar = null;
                        SelectExpedienteAuxiliar = null;
                        SelectExpediente = expedient;
                        SelectIngreso = ingres;
                        SelectAbogadoTitular = titulars;
                        EstatusEnabled = AnioEnabled = FolioEnabled = PaternoEnabled = MaternoEnabled = NombreEnabled = ObservacionesInternoEnabled = true;
                        if (ListIngresosAsignados.Where(wh => wh.ABOGADO_CAUSA_PENAL.Where(w => w.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                            w.ID_ABOGADO_TITULO == SelectColaborador.ABOGADO.ID_ABOGADO_TITULO && w.ID_ANIO == SelectIngreso.ID_ANIO &&
                            w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO).Any()).Any())
                            SelectAbogadoIngreso = ListIngresosAsignados.Where(wh => wh.ABOGADO_CAUSA_PENAL.Where(w => w.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                                w.ID_ANIO == SelectIngreso.ID_ANIO && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO &&
                                w.ID_ABOGADO_TITULO == SelectColaborador.ABOGADO.ID_ABOGADO_TITULO).Any()).FirstOrDefault();
                        else
                            GetDatosInternoSeleccionado();
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado.", ex);
                    }
                    break;
                #endregion
            
                case "buscar_visitante":
                    BuscarPersonasSinCodigo();
                    break;
            }
        }

        private async void GuardarAbogadoNew()
        {
            if (SelectPersona == null)
            {
                new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un abogado titular");
                return;
            }
            else
                if (SelectPersona.ABOGADO == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un abogado titular");
                    return;
                }
                else
                    if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO != "T")
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El abogado seleccionado no es titular");
                        return;
                    }
            var respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando Colaborador", () =>
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
                        abogado.ID_ABOGADO_TITULO = "C";
                        if(ImagesToSave != null)
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
                        abogado.OBSERV = TextObservaciones;
                        abogado.ULTIMA_MOD_FEC = hoy;
                        abogado.ID_ESTATUS_VISITA = SelectEstatusVisita;
                        //abogado.ABOGADO_TITULAR = SelectPersona.ID_PERSONA;
                        abogado.ID_JUZGADO = SelectPersona.ABOGADO.ID_JUZGADO;
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

                    #region Titular
                    var titular = new ABOGADO_TITULAR();
                    titular.ID_ABOGADO_TITULAR = SelectPersona.ID_PERSONA;
                    titular.ESTATUS = "S";
                    titular.MOVIMIENTO_FEC = Fechas.GetFechaDateServer;
                    #endregion

                    if (SelectColaborador == null)
                    {
                        #region Persona
                        persona.ID_PERSONA = int.Parse(Fechas.GetFechaDateServer.Year + "" + new cPersona().GetSequence<int>("ID_PERSONA_SEQ"));
                        abogado.ABOGADO_TITULAR1.Add(titular);
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
                            SelectColaborador = new cPersona().Obtener(persona.ID_PERSONA).FirstOrDefault();// persona;
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        bool AbogadoTitularDiferente = false;
                        #region Persona
                        persona.ID_PERSONA = SelectColaborador.ID_PERSONA;
                        persona.ID_PERSONA = SelectColaborador.ID_PERSONA;
                        persona.ID_TIPO_PERSONA = SelectColaborador.ID_TIPO_PERSONA;
                        persona.LUGAR_NACIMIENTO = SelectColaborador.LUGAR_NACIMIENTO;
                        persona.ESTADO_CIVIL = SelectColaborador.ESTADO_CIVIL;
                        persona.ID_ETNIA = SelectColaborador.ID_ETNIA;
                        persona.NORIGINAL = SelectColaborador.NORIGINAL;
                        persona.CORIGINAL = SelectColaborador.CORIGINAL;
                        #endregion
                        if (SelectColaborador.ABOGADO != null)
                        {
                            #region Biometrico
                            if (personaFotos != null)
                            {
                                foreach (var f in personaFotos)
                                {
                                    f.ID_PERSONA = SelectColaborador.ID_PERSONA;
                                }
                            }
                            #endregion

                            #region Abogado
                            abogado.ID_ABOGADO = SelectColaborador.ID_PERSONA;
                            abogado.NORIGINAL = SelectColaborador.ABOGADO.NORIGINAL;
                            abogado.CEDULA = SelectColaborador.ABOGADO.CEDULA;
                            abogado.CJF = SelectColaborador.ABOGADO.CJF;
                            abogado.CREDENCIALIZADO = SelectColaborador.ABOGADO.CREDENCIALIZADO;
                            abogado.ALTA_FEC = SelectColaborador.ABOGADO.ALTA_FEC;
                            #endregion

                            #region Titular
                            if(SelectColaborador.ABOGADO.ABOGADO_TITULAR1 != null)
                            {
                                var t = SelectColaborador.ABOGADO.ABOGADO_TITULAR1.Where(w => w.ESTATUS == "S").FirstOrDefault();
                                if (t != null)
                                {
                                    if (t.ID_ABOGADO_TITULAR != SelectPersona.ID_PERSONA)
                                        AbogadoTitularDiferente = true;
                                }
                                else
                                    AbogadoTitularDiferente = true;
                            }
                            else
                                AbogadoTitularDiferente = true;
                            #endregion
                        }
                        abogado.ULTIMA_MOD_FEC = hoy;
                        

                        if (new cPersona().ActualizarAbogado(persona, personaFotos, abogado, AbogadoTitularDiferente ? SelectPersona.ID_PERSONA : 0,Fechas.GetFechaDateServer))
                        {
                            SelectColaborador = new cPersona().Obtener(persona.ID_PERSONA).FirstOrDefault();
                            return true;
                        }
                        else
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los datos del colaborador.", ex);
                    return false;
                }
            });

            if (respuesta)
            {
                new Dialogos().ConfirmacionDialogo("Éxito", "El colaborador se guardo correctamente");
                StaticSourcesViewModel.SourceChanged = false;
            }
            else
                new Dialogos().ConfirmacionDialogo("Error", "Ocurrió un error al guardar colaborador");
        }

        private async Task GuardarAbogado()
        {
            try
            {
                if (ImagesToSave == null)
                    ImagesToSave = new List<ImageSourceToSave>();
                var hoy = Fechas.GetFechaDateServer;

                var persona = new SSP.Servidor.PERSONA
                {
                    ID_PERSONA = Editable ? SelectColaborador.ID_PERSONA : int.Parse(Fechas.GetFechaDateServer.Year + "" + new cPersona().GetSequence<int>("ID_PERSONA_SEQ")),
                    CORIGINAL = Editable ? SelectColaborador.CORIGINAL : null,
                    CORREO_ELECTRONICO = TextCorreo,
                    CURP = TextCurp,
                    DOMICILIO_CALLE = TextCalle,
                    DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                    DOMICILIO_NUM_EXT = TextNumExt,
                    DOMICILIO_NUM_INT = TextNumInt,
                    ESTADO_CIVIL = Editable ? SelectColaborador.ESTADO_CIVIL : null,
                    FEC_NACIMIENTO = SelectFechaNacimiento,
                    ID_COLONIA = SelectColonia,
                    ID_ENTIDAD = SelectEntidad,
                    ID_ETNIA = Editable ? SelectColaborador.ID_ETNIA : null,
                    ID_MUNICIPIO = SelectMunicipio,
                    ID_PAIS = SelectPais,
                    ID_TIPO_DISCAPACIDAD = SelectTipoDiscapacidad,
                    ID_TIPO_PERSONA = Editable ? SelectColaborador.ID_TIPO_PERSONA : 2,
                    IFE = TextIne,
                    LUGAR_NACIMIENTO = Editable ? SelectColaborador.LUGAR_NACIMIENTO : null,
                    MATERNO = TextMaternoAbogado,
                    PATERNO = TextPaternoAbogado,
                    NOMBRE = TextNombreAbogado,
                    NACIONALIDAD = SelectPais,
                    NORIGINAL = Editable ? SelectColaborador.NORIGINAL : new Nullable<int>(),
                    RFC = TextRfc,
                    SEXO = SelectSexo,
                    SMATERNO = Editable ? SelectColaborador.SMATERNO : null,
                    SPATERNO = Editable ? SelectColaborador.SPATERNO : null,
                    SNOMBRE = Editable ? SelectColaborador.SNOMBRE : null,
                    TELEFONO = TextTelefonoFijo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                    TELEFONO_MOVIL = TextTelefonoMovil.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
                };

                #region ABOGADO
                var abogado = new ABOGADO();
                var VisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                var AbogadoColaborador = Parametro.ID_ABOGADO_TITULAR_COLABORADOR;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    abogado = new ABOGADO
                    {
                        ID_ABOGADO = persona.ID_PERSONA,
                        ID_ABOGADO_TITULO = AbogadoColaborador,
                        //ID_TIPO_ABOGADO = 0,
                        IFE_FRONTAL = ImagesToSave.Where(w => w.FrameName == "ImgFrente").Any() ?
                            new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgFrente").FirstOrDefault().ImageCaptured) :
                            null,
                        IFE_REVERSO = ImagesToSave.Where(w => w.FrameName == "ImgReverso").Any() ?
                            new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgReverso").FirstOrDefault().ImageCaptured) :
                            null,
                        CEDULA = Editable ? NuevoAbogado ? null : SelectColaborador.ABOGADO.CEDULA : null,
                        CEDULA_FRONTAL = ImagesToSave.Where(w => w.FrameName == "ImgFrenteCedula").Any() ?
                            new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgFrenteCedula").FirstOrDefault().ImageCaptured) :
                            null,
                        CEDULA_REVERSO = ImagesToSave.Where(w => w.FrameName == "ImgReversoCedula").Any() ?
                            new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgReversoCedula").FirstOrDefault().ImageCaptured) :
                            null,
                        NORIGINAL = persona.NORIGINAL,
                        ALTA_FEC = Editable ? NuevoAbogado ? hoy : SelectColaborador.ABOGADO.ALTA_FEC : hoy,
                        OBSERV = TextObservaciones,
                        ULTIMA_MOD_FEC = hoy,
                        ID_ESTATUS_VISITA = SelectEstatusVisita,
                        ABOGADO_TITULAR = SelectAbogadoTitular.ID_PERSONA,
                        ID_JUZGADO = Editable ? NuevoAbogado ? new Nullable<short>() : SelectColaborador.ABOGADO.ID_JUZGADO : new Nullable<short>(),
                        CJF = Editable ? NuevoAbogado ? null : SelectColaborador.ABOGADO.CJF : null,
                        CREDENCIALIZADO = Editable ? NuevoAbogado ? "N" : SelectColaborador.ABOGADO.CREDENCIALIZADO : "N"
                    };
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

                #region Modificar Relaciones Colaborador
                /*if (EditarTitular)
                {
                    new cAbogadoIngreso().CancelarXAbogado(SelectColaborador.ABOGADO.ID_ABOGADO, Parametro.ID_ESTATUS_VISITA_CANCELADO, SelectColaborador.ABOGADO.ID_ABOGADO_TITULO);
                }*/
                #endregion

                if (!new cPersona().InsertarColaboradorTransaccion(persona, Editable, abogado, NuevoAbogado, false, Parametro.ID_ABOGADO_TITULAR_COLABORADOR, Parametro.ID_ESTATUS_VISITA_CANCELADO, personaNips, personaFotos, personaHuellas, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO, EditarTitular))
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No se pudo guardar la información.");
                    return;
                }

                SelectColaborador = new cPersona().ObtenerPersonaXID(persona.ID_PERSONA).FirstOrDefault();
                Editable = ValidarEnabled = DiscapacitadoEnabled = AsignacionEnabled = true;
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar datos del abogado.", ex);
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
                                personaFoto.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                {
                                    BIOMETRICO = bit,
                                    ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                                    ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                                    ID_PERSONA = idPersona
                                });
                            }
                        }));
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
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
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
                        }));
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
                if (AbogadoTitular)
                    BuscarAbogadosTitulares();
                else
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
                if (AbogadoTitular)
                {
                    var codigo = TextCodigoTitular;
                    TextCodigoTitular = null;
                    TextNombreTitular = TextNombre;
                    TextPaternoTitular = TextPaterno;
                    TextMaternoTitular = TextMaterno;
                    BuscarAbogadosTitulares();
                    TextCodigoTitular = codigo;
                }
                else
                    BuscarPersonasSinCodigo();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }

        private void AbogadosTitulares(Object obj)
        {
            try
            {
                AbogadoTitular = true;
                TextPaterno = string.IsNullOrEmpty(TextPaternoTitular) ? string.Empty : TextPaternoTitular;
                TextMaterno = string.IsNullOrEmpty(TextMaternoTitular) ? string.Empty : TextMaternoTitular;
                TextNombre = string.IsNullOrEmpty(TextNombreTitular) ? string.Empty : TextNombreTitular;
                TextCodigo = string.IsNullOrEmpty(TextCodigoTitular) ? string.Empty : TextCodigoTitular;
                BuscarAbogadosTitulares();
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
                var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                if (string.IsNullOrEmpty(TextCodigoAbogado))
                {
                    var persona = SelectPersona;
                    ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                    TextNombre = TextNombreAbogado;
                    TextPaterno = TextPaternoAbogado;
                    TextMaterno = TextMaternoAbogado;
                    //new cPersona().ObtenerAbogadosXTipoTitular(TextNombre, TextPaterno, TextMaterno, 0, Parametro.ID_ABOGADO_TITULAR_ABOGADO)));
                    SelectPersonaAuxiliar = persona;
                    Editable = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                }
                else
                {
                    SelectPersonaAuxiliar = SelectColaborador;
                    //var persona = SelectColaborador;
                    
                    ListPersonas.InsertRange(await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                       new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodos(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, int.Parse(TextCodigoAbogado))
                             .OrderByDescending(o => o.ABOGADO != null).ThenByDescending(t => t.ID_TIPO_PERSONA == legal))));
                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                    //SelectPersonaAuxiliar = persona;
                    if (ListPersonas.Count == 1)
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("Validación",
                                "Existen cambios sin guardar, esta segúro que desea seleccionar a otra persona?") == 1)
                                SelectColaborador = ListPersonas.FirstOrDefault();
                            else
                            {
                                SelectColaborador = SelectPersonaAuxiliar;
                                return;
                            }
                        }
                        else
                            SelectColaborador = ListPersonas.FirstOrDefault();

                        if (SelectColaborador.ABOGADO != null)
                        {
                            if (SelectColaborador.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR || SelectColaborador.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO)
                            {
                                var colabo = SelectColaborador;
                                var personas = ListPersonas;
                                var titulars = SelectAbogadoTitular;
                                LimpiarCampos();
                                LimpiarAsignacion();
                                SelectColaborador = colabo;
                                ListPersonas = personas;
                                SelectAbogadoTitular = titulars;
                                NuevoAbogado = false;
                                Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                AbogadoTitularEnabled = true;
                                SetValidaciones();
                                await GetDatosPersonaSeleccionada();
                                if (SelectColaborador != null)
                                    if (SelectColaborador.ABOGADO != null)
                                    {
                                        if (SelectColaborador.ABOGADO.ABOGADO_TITULAR1 != null)
                                        {
                                            var at = SelectColaborador.ABOGADO.ABOGADO_TITULAR1.Where(w => w.ESTATUS == "S").FirstOrDefault();
                                            if (at != null)
                                            {
                                                SelectPersona = new cPersona().Obtener(at.ID_ABOGADO_TITULAR).FirstOrDefault();
                                            }

                                        }
                                        #region Comentado
                                        //if (SelectColaborador.ABOGADO.ABOGADO_TITULAR != null)
                                        //{
                                        //    SelectPersona = new cPersona().Obtener(SelectColaborador.ABOGADO.ABOGADO_TITULAR.Value).FirstOrDefault();
                                        //}
                                        #endregion
                                    }
                                StaticSourcesViewModel.SourceChanged = false;
                                BuscarAbogadoEnabled = EditarTitularEnabled = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                Editable = false;
                            }
                            else
                            {
                                var AbogadoTitular = Parametro.ID_ABOGADO_TITULAR_ABOGADO;
                                if (SelectColaborador.ABOGADO.ID_ABOGADO_TITULO == AbogadoTitular || SelectColaborador.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ACTUARIO) 
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Validación", "Un abogado titular o actuario no puede convertirse en colaborador.");
                                }
                                else if (SelectColaborador.ABOGADO.ID_ABOGADO_TITULO != AbogadoTitular)
                                {
                                    if (await new Dialogos().ConfirmarEliminar("Validación", "El abogado seleccionado no esta registrado como colaborador,"
                                            + " ¿Desea cambiarlo ahora?") == 1)
                                    {
                                        LimpiarAsignacion();
                                        NuevoAbogado = true;
                                        Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                        AbogadoTitularEnabled = true;
                                        SetValidaciones();
                                        await GetDatosPersonaSeleccionada();
                                        if (SelectColaborador != null)
                                            if (SelectColaborador.ABOGADO != null)
                                            {
                                                if (SelectColaborador.ABOGADO.ABOGADO_TITULAR1 != null)
                                                {
                                                    var at = SelectColaborador.ABOGADO.ABOGADO_TITULAR1.Where(w => w.ESTATUS == "S").FirstOrDefault();
                                                    if (at != null)
                                                    {
                                                        SelectPersona = new cPersona().Obtener(at.ID_ABOGADO_TITULAR).FirstOrDefault();
                                                    }
                                                }
                                                #region Comentado
                                                //if (SelectColaborador.ABOGADO.ABOGADO_TITULAR != null)
                                                //{
                                                //    SelectPersona = new cPersona().Obtener(SelectColaborador.ABOGADO.ABOGADO_TITULAR.Value).FirstOrDefault();
                                                //}
                                                #endregion
                                            }
                                        BuscarAbogadoEnabled = true;
                                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                        Editable = false;
                                    }
                                    else
                                    {
                                        SelectColaborador = SelectPersonaAuxiliar;
                                        if(SelectColaborador != null)
                                        {
                                            TextCodigoAbogado = SelectColaborador.ID_PERSONA.ToString();
                                            TextNombreAbogado = SelectColaborador.NOMBRE;
                                            TextPaternoAbogado = SelectColaborador.PATERNO;
                                            TextMaternoAbogado = SelectColaborador.MATERNO;
                                            TextCurp = SelectColaborador.CURP;
                                            TextRfc = SelectColaborador.RFC;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (await new Dialogos().ConfirmarEliminar("Validación", "La persona seleccionada no esta registrada como colaborador,"
                                    + " ¿Desea registrarla ahora?") == 1)
                            {
                                LimpiarAsignacion();
                                NuevoAbogado = true;
                                Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                AbogadoTitularEnabled = true;
                                SetValidaciones();
                                await GetDatosPersonaSeleccionada();
                                if (SelectColaborador != null)
                                    if (SelectColaborador.ABOGADO != null)
                                    {
                                        if (SelectColaborador.ABOGADO.ABOGADO_TITULAR1 != null)
                                        {
                                            var at = SelectColaborador.ABOGADO.ABOGADO_TITULAR1.Where(w => w.ESTATUS == "S").FirstOrDefault();
                                            if (at != null)
                                            {
                                                SelectPersona = new cPersona().Obtener(at.ID_ABOGADO_TITULAR).FirstOrDefault();
                                            }
                                        }
                                        #region Comentado
                                        //if (SelectColaborador.ABOGADO.ABOGADO_TITULAR != null)
                                        //{
                                        //    SelectPersona = new cPersona().Obtener(SelectColaborador.ABOGADO.ABOGADO_TITULAR.Value).FirstOrDefault();
                                        //}
                                        #endregion

                                    }
                                BuscarAbogadoEnabled = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                Editable = false;
                            }
                            else {
                                SelectColaborador = SelectPersonaAuxiliar;
                                if (SelectColaborador != null)
                                {
                                    TextCodigoAbogado = SelectColaborador.ID_PERSONA.ToString();
                                    TextNombreAbogado = SelectColaborador.NOMBRE;
                                    TextPaternoAbogado = SelectColaborador.PATERNO;
                                    TextMaternoAbogado = SelectColaborador.MATERNO;
                                    TextCurp = SelectColaborador.CURP;
                                    TextRfc = SelectColaborador.RFC;
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
                    TextNombreAbogado = TextNombre;
                    TextMaternoAbogado = TextMaterno;
                    TextPaternoAbogado = TextPaterno;
                }
                else
                {
                    TextNombreAbogado = TextNombre;
                    TextMaternoAbogado = TextMaterno;
                    TextPaternoAbogado = TextPaterno;
                    StaticSourcesViewModel.SourceChanged = false;
                }

                Application.Current.Dispatcher.Invoke((Action)(async delegate
                {
                    var person = SelectPersona;
                    ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                    if (PopUpsViewModels.VisibleBuscarPersonasExistentes == Visibility.Collapsed)
                    {
                        SelectPersonaAuxiliar = person;
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

        private async void BuscarAbogadosTitulares()
        {
            try
            {
                if (string.IsNullOrEmpty(TextCodigo))
                {
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas.InsertRange(await SegmentarTitularesBusqueda());
                }
                else
                {
                    var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas.InsertRange(await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                       new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerAbogadosXTipoTitular(TextNombreTitular, TextPaternoTitular, TextMaternoTitular, int.Parse(string.IsNullOrEmpty(TextCodigoTitular) ? "0" : TextCodigoTitular), Parametro.ID_ABOGADO_TITULAR_ABOGADO)
                             .OrderByDescending(o => o.ABOGADO != null).ThenByDescending(t => t.ID_TIPO_PERSONA == legal))));
                }
                Editable = true;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                Editable = false;
                EmptyBuscarRelacionInternoVisible = !(ListPersonas.Count > 0);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void BuscarInternoAsignacion(Object obj)
        {
            try
            {
                var ing = SelectIngreso;
                SelectIngresoAuxiliar = ing;
                var exp = SelectExpediente;
                SelectExpedienteAuxiliar = exp;
                AnioBuscar = !string.IsNullOrEmpty(TextAnio) ? int.Parse(TextAnio) : new Nullable<int>();
                FolioBuscar = !string.IsNullOrEmpty(TextFolio) ? int.Parse(TextFolio) : new Nullable<int>();
                NombreBuscar = !string.IsNullOrEmpty(TextNombreImputado) ? TextNombreImputado : string.Empty;
                ApellidoPaternoBuscar = !string.IsNullOrEmpty(TextPaternoImputado) ? TextPaternoImputado : string.Empty;
                ApellidoMaternoBuscar = !string.IsNullOrEmpty(TextMaternoImputado) ? TextMaternoImputado : string.Empty;
                BuscarImputado();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void BuscarInternoPopup(Object obj)
        {
            try
            {
                //var ing = SelectIngreso;
                //SelectIngresoAuxiliar = ing;
                //var exp = SelectExpediente;
                //SelectExpedienteAuxiliar = exp;
                BuscarImputado();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerImputadosXAbogadoIngreso(
                    ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag, SelectAbogadoTitular));
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<IMPUTADO>();
            }
        }

        private async void BuscarImputado()
        {
            try
            {
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count == 1)
                {
                    if (AnioBuscar != null && FolioBuscar != null)
                    {
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        var ingreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        if (ingreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El imputado no cuenta con ingreso");
                            return;
                        }
                        foreach (var item in EstatusInactivos)
                        {
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "No se encontró ningun ingreso activo en este imputado.");
                                return;
                            }
                        }
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a este centro.");
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El interno [" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ANIO.ToString() + "/" +
                                ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no puede recibir visitas.");
                            return;
                        }
                        SelectExpediente = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        var expedient = SelectExpediente;
                        var ingres = SelectIngreso;
                        LimpiarAsignacion();
                        //LimpiarCampos();
                        SelectExpediente = expedient;
                        SelectIngreso = ingres;
                        EstatusEnabled = AnioEnabled = FolioEnabled = PaternoEnabled = MaternoEnabled = NombreEnabled = ObservacionesInternoEnabled = true;
                        if (ListIngresosAsignados != null)
                        {
                            if (ListIngresosAsignados.Where(wh => wh.ABOGADO_CAUSA_PENAL.Where(w => w.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                                    w.ID_ANIO == SelectIngreso.ID_ANIO && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO &&
                                        w.ID_ABOGADO_TITULO == SelectColaborador.ABOGADO.ID_ABOGADO_TITULO).Any()).Any())
                                SelectAbogadoIngreso = ListIngresosAsignados.Where(wh => wh.ABOGADO_CAUSA_PENAL.Where(w => w.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                                    w.ID_ANIO == SelectIngreso.ID_ANIO && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO &&
                                        w.ID_ABOGADO_TITULO == SelectColaborador.ABOGADO.ID_ABOGADO_TITULO).Any()).FirstOrDefault();
                            else
                                GetDatosInternoSeleccionado();
                        }
                        else
                        {
                            GetDatosInternoSeleccionado();
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        return;
                    }
                }
                if (ListExpediente.Count == 0)
                {
                    if (!string.IsNullOrEmpty(NombreD) || !string.IsNullOrEmpty(PaternoD) || !string.IsNullOrEmpty(MaternoD))
                        new Dialogos().ConfirmacionDialogo("Validación", "No se encontró ningún imputado con esos datos.");
                }
                EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de imputados.", ex);
            }
        }

        private async Task<List<SSP.Servidor.PERSONA>> SegmentarPersonasBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(TextPaternoAbogado) && string.IsNullOrEmpty(TextMaternoAbogado) && string.IsNullOrEmpty(TextNombreAbogado) && string.IsNullOrEmpty(TextCodigoAbogado))
                    return new List<SSP.Servidor.PERSONA>();
                Pagina = _Pag;
                var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                        new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodos(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, 0, _Pag)
                             .OrderByDescending(o => o.ABOGADO != null).ThenByDescending(t => t.ID_TIPO_PERSONA == legal)));
                Pagina = result.Any() ? Pagina + 1 : Pagina;
                SeguirCargandoPersonas = result.Any();
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<SSP.Servidor.PERSONA>();
            }
        }

        private async Task<List<SSP.Servidor.PERSONA>> SegmentarTitularesBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(TextPaternoTitular) && string.IsNullOrEmpty(TextMaternoTitular) && string.IsNullOrEmpty(TextNombreTitular) && string.IsNullOrEmpty(TextCodigoTitular))
                    return new List<SSP.Servidor.PERSONA>();
                Pagina = _Pag;
                var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                        new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerAbogadosXTipoTitular(TextNombreTitular, TextPaternoTitular, TextMaternoTitular, 0, Parametro.ID_ABOGADO_TITULAR_ABOGADO, _Pag)
                             .OrderByDescending(o => o.ABOGADO != null).ThenByDescending(t => t.ID_TIPO_PERSONA == legal)));
                Pagina = result.Any() ? Pagina + 1 : Pagina;
                SeguirCargandoPersonas = result.Any();
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
                    #region Titular
                    long IdTitular = 0;
                    var EstatusLiberado = Parametro.ID_ESTATUS_ADMVO_LIBERADO;
                    if (SelectColaborador.ABOGADO != null)
                        if (SelectColaborador.ABOGADO.ABOGADO_TITULAR1 != null)
                        {
                            var titular = SelectColaborador.ABOGADO.ABOGADO_TITULAR1.Where(w => w.ESTATUS == "S").FirstOrDefault();
                            if (titular != null)
                            {
                                AbogadoTitularEnabled = false;
                                EditarTitularEnabled = true;
                                SelectPersona = titular.ABOGADO1.PERSONA;
                                SelectAbogadoTitular = titular.ABOGADO1.PERSONA;
                                IdTitular = titular.ABOGADO1.PERSONA.ID_PERSONA;
                                TextCodigoTitular = titular.ABOGADO1.PERSONA.ID_PERSONA.ToString();
                                TextPaternoTitular = !string.IsNullOrEmpty(titular.ABOGADO1.PERSONA.PATERNO) ? titular.ABOGADO1.PERSONA.PATERNO.Trim() : string.Empty;
                                TextMaternoTitular = !string.IsNullOrEmpty(titular.ABOGADO1.PERSONA.MATERNO) ? titular.ABOGADO1.PERSONA.MATERNO.Trim() : string.Empty;
                                TextNombreTitular = !string.IsNullOrEmpty(titular.ABOGADO1.PERSONA.NOMBRE) ? titular.ABOGADO1.PERSONA.NOMBRE.Trim() : string.Empty;
                                
                            }
                        }
                    #endregion

                    TextCodigoAbogado = SelectColaborador.ID_PERSONA.ToString();
                    TextPaternoAbogado = !string.IsNullOrEmpty(SelectColaborador.PATERNO) ? SelectColaborador.PATERNO.Trim() : string.Empty;
                    TextMaternoAbogado = !string.IsNullOrEmpty(SelectColaborador.MATERNO) ? SelectColaborador.MATERNO.Trim() : string.Empty;
                    TextNombreAbogado = !string.IsNullOrEmpty(SelectColaborador.NOMBRE) ? SelectColaborador.NOMBRE.Trim() : string.Empty;
                    SelectSexo = SelectColaborador.SEXO;
                    var hoy = Fechas.GetFechaDateServer;
                    SelectFechaNacimiento = SelectColaborador.FEC_NACIMIENTO.HasValue ? SelectColaborador.FEC_NACIMIENTO.Value : hoy;
                    TextCurp = SelectColaborador.CURP;
                    TextRfc = SelectColaborador.RFC;
                    TextTelefonoFijo = SelectColaborador.TELEFONO;
                    TextTelefonoMovil = SelectColaborador.TELEFONO_MOVIL;
                    TextCorreo = SelectColaborador.CORREO_ELECTRONICO;
                    TextIne = SelectColaborador.IFE;
                    SelectDiscapacitado = SelectColaborador.ID_TIPO_DISCAPACIDAD.HasValue ? SelectColaborador.ID_TIPO_DISCAPACIDAD.Value > 0 ? "S" : "N" : "N";
                    SelectTipoDiscapacidad = (short)(SelectColaborador.ID_TIPO_DISCAPACIDAD.HasValue ? SelectColaborador.ID_TIPO_DISCAPACIDAD.Value : -1);
                    Insertable = false;
                if (SelectColaborador.ABOGADO != null)
                {
                    SelectFechaAlta = SelectColaborador.ABOGADO.ALTA_FEC.HasValue ? SelectColaborador.ABOGADO.ALTA_FEC.Value.ToString("dd/MM/yyyy") : hoy.ToString("dd/MM/yyyy");
                    TextObservaciones = SelectColaborador.ABOGADO.OBSERV;
                    SelectEstatusVisita = SelectColaborador.ABOGADO.ID_ESTATUS_VISITA.HasValue ? SelectColaborador.ABOGADO.ID_ESTATUS_VISITA.Value : (short)-1;
                    Credencializado = ImpresionGafeteFront = ImpresionGafeteBack = ImpresionGafete = SelectColaborador.ABOGADO.CREDENCIALIZADO == "S";
                    ImagesToSave = new List<ImageSourceToSave>();
                    if (SelectColaborador.ABOGADO.IFE_FRONTAL != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgFrente", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectColaborador.ABOGADO.IFE_FRONTAL) });
                    if (SelectColaborador.ABOGADO.IFE_REVERSO != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgReverso", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectColaborador.ABOGADO.IFE_REVERSO) });
                    if (SelectColaborador.ABOGADO.CEDULA_FRONTAL != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgFrenteCedula", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectColaborador.ABOGADO.CEDULA_FRONTAL) });
                    if (SelectColaborador.ABOGADO.CEDULA_REVERSO != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgReversoCedula", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectColaborador.ABOGADO.CEDULA_REVERSO) });
                    if (SelectColaborador.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR)
                    {
                        ListIngresosAsignados = new ObservableCollection<ABOGADO_INGRESO>();
                        if (SelectColaborador.ABOGADO != null)
                        {
                            if (SelectColaborador.ABOGADO.ABOGADO_INGRESO != null)
                            {
                                short[] Estatus = { (short)enumEstatusAdministrativo.ASIGNADO_A_CELDA, (short)enumEstatusAdministrativo.EN_AREA_TEMPORAL, (short)enumEstatusAdministrativo.EN_CLASIFICACION, (short)enumEstatusAdministrativo.INDICIADOS };
                                ListIngresosAsignados = new ObservableCollection<ABOGADO_INGRESO>(SelectColaborador.ABOGADO.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO_TITULAR == IdTitular && w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro && Estatus.Count(x => x == w.INGRESO.ID_ESTATUS_ADMINISTRATIVO) > 0 && w.ID_ESTATUS_VISITA != (short)enumEstatusVisita.CANCELADO_A));
                            }
                        }
                        #region Comentado
                        //if (SelectColaborador.ABOGADO.ABOGADO_TITULAR.HasValue ? SelectColaborador.ABOGADO.ABOGADO_TITULAR.Value > 0 : false)
                        //{
                        //    AbogadoTitularEnabled = false;
                        //    EditarTitularEnabled = true;
                        //    //AsignacionEnabled = true;
                        //    SelectAbogadoTitular = SelectColaborador.ABOGADO.PERSONA;
                        //    TextCodigoTitular = SelectColaborador.ABOGADO.PERSONA.ID_PERSONA.ToString();
                        //    TextPaternoTitular = SelectColaborador.ABOGADO.PERSONA.PATERNO.Trim();
                        //    TextMaternoTitular = SelectColaborador.ABOGADO.PERSONA.MATERNO.Trim();
                        //    TextNombreTitular = SelectColaborador.ABOGADO.PERSONA.NOMBRE.Trim();
                        //    var EstatusLiberado = Parametro.ID_ESTATUS_ADMVO_LIBERADO;

                        //    ListIngresosAsignados = new ObservableCollection<ABOGADO_INGRESO>(SelectColaborador.ABOGADO.ABOGADO_INGRESO
                        //        .Where(w => SelectColaborador.ABOGADO.ABOGADO2 == null ? false :
                        //            (SelectColaborador.ABOGADO.ABOGADO2.ABOGADO_INGRESO.Where(wh => wh.ID_ANIO == w.ID_ANIO && wh.ID_CENTRO == w.ID_CENTRO &&
                        //            wh.ID_IMPUTADO == w.ID_IMPUTADO && wh.ID_INGRESO == w.ID_INGRESO).Any() &&
                        //            w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != EstatusLiberado ?
                        //                w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro : w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != EstatusLiberado)));
                        //}
                        //else
                        //{
                        //    SelectAbogadoTitular = null;
                        //    EditarTitularEnabled = false;
                        //}
                        #endregion
                    }
                    else
                    {
                        SelectAbogadoTitular = null;
                        EditarTitularEnabled = false;
                    }
                }
                else
                {
                    SelectAbogadoTitular = null;
                    AbogadoTitularEnabled = true;
                    EditarTitularEnabled = EditarTitularEnabled = ImpresionGafeteFront = ImpresionGafeteBack = ImpresionGafete = Credencializado = false;
                    TextCodigoTitular = TextPaternoTitular = TextMaternoTitular = TextNombreTitular = TextObservaciones = string.Empty;
                    ImagesToSave = new List<ImageSourceToSave>();
                    SelectFechaAlta = hoy.ToString("dd/MM/yyyy");
                    SelectEstatusVisita = -1;
                    ListIngresosAsignados = new ObservableCollection<ABOGADO_INGRESO>();
                }
                TextCalle = SelectColaborador.DOMICILIO_CALLE;
                TextNumInt = SelectColaborador.DOMICILIO_NUM_INT;
                TextNumExt = SelectColaborador.DOMICILIO_NUM_EXT;
                TextCodigoPostal = SelectColaborador.DOMICILIO_CODIGO_POSTAL;
                FotoTomada = SelectColaborador.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any();
                if (FotoTomada)
                {
                    ImagenAbogado = SelectColaborador.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
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
                if (SelectColaborador.PERSONA_BIOMETRICO != null)
                {
                    foreach (var h in SelectColaborador.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9))
                    {
                        HuellasCapturadas.Add(new PlantillaBiometrico() { ID_TIPO_BIOMETRICO = (enumTipoBiometrico)h.ID_TIPO_BIOMETRICO, ID_TIPO_FORMATO = (enumTipoFormato)h.ID_FORMATO, BIOMETRICO = h.BIOMETRICO, });
                    }
                }
                var VisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                //if (SelectColaborador.PERSONA_NIP != null ? SelectColaborador.PERSONA_NIP.Count > 0 : false)
                //{
                //    var VisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                //    TextNip = SelectColaborador.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_VISITA == VisitaLegal).FirstOrDefault().NIP.HasValue ?
                //           SelectColaborador.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_VISITA == VisitaLegal).FirstOrDefault().NIP.Value.ToString() : string.Empty;
                //}
                SelectPais = -1;
                await TaskEx.Delay(100);
                SelectPais = SelectColaborador.ID_PAIS;
                await TaskEx.Delay(100);
                SelectEntidad = SelectColaborador.ID_ENTIDAD;
                await TaskEx.Delay(100);
                SelectMunicipio = SelectColaborador.ID_MUNICIPIO;
                await TaskEx.Delay(100);
                if (SelectColaborador.ID_COLONIA.HasValue ? SelectColaborador.ID_COLONIA.Value > 0 : false)
                    SelectColonia = SelectColaborador.ID_COLONIA;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void GetDatosInternoSeleccionado()
        {
            try
            {
                TextNombreImputado = SelectIngreso.IMPUTADO.NOMBRE;
                TextPaternoImputado = SelectIngreso.IMPUTADO.PATERNO;
                TextMaternoImputado = SelectIngreso.IMPUTADO.MATERNO;
                TextAnio = SelectIngreso.IMPUTADO.ID_ANIO.ToString();
                TextFolio = SelectIngreso.IMPUTADO.ID_IMPUTADO.ToString();
                SelectEstatus = Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                //if (ListIngresosAsignados != null ? ListIngresosAsignados.Count > 0 : false)
                //{
                     ListCausasPenales = new ObservableCollection<CausaPenalAsignacion>();
                    /*Codigo Nuevo*******************************************************************************/
                    if (SelectIngreso.CAUSA_PENAL != null)
                    {
                        var ing = SelectColaborador.ABOGADO.ABOGADO_INGRESO.Where(w => w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_ANIO == SelectIngreso.ID_ANIO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault();
                        if (ing != null)
                        {
                            var cp_a = ing.ABOGADO_CAUSA_PENAL;
                            foreach (var obj in SelectIngreso.CAUSA_PENAL.Where(w => w.ABOGADO_CAUSA_PENAL.Count(x => x.ID_ABOGADO == SelectPersona.ID_PERSONA && (x.ID_ESTATUS_VISITA == (short)enumEstatusVisita.AUTORIZADO || x.ID_ESTATUS_VISITA == (short)enumEstatusVisita.EN_REVISION)) > 0))// && w.ID_ESTATUS_CP == (short)enumEstatusVisita.AUTORIZADO))
                            {
                                if (cp_a.Count(w => w.ID_CAUSA_PENAL == obj.ID_CAUSA_PENAL && (w.ID_ESTATUS_VISITA == (short)enumEstatusVisita.AUTORIZADO || w.ID_ESTATUS_VISITA == (short)enumEstatusVisita.EN_REVISION)) == 0)
                                {
                                    ListCausasPenales.Add(new CausaPenalAsignacion
                                    {
                                        CAUSA_PENAL = obj,
                                        ELEGIDO = false
                                    });
                                }
                            }
                        }
                        else
                        {
                            foreach (var obj in SelectIngreso.CAUSA_PENAL.Where(w => w.ABOGADO_CAUSA_PENAL.Count(x => x.ID_ABOGADO == SelectPersona.ID_PERSONA && (x.ID_ESTATUS_VISITA == (short)enumEstatusVisita.AUTORIZADO || x.ID_ESTATUS_VISITA == (short)enumEstatusVisita.EN_REVISION)) > 0))
                            {
                               ListCausasPenales.Add(new CausaPenalAsignacion
                               {
                                        CAUSA_PENAL = obj,
                                        ELEGIDO = false
                               });
                            }
                        }
                        //}
                        #region comentado
                        /**************************************/
                    //var ingresosAsignados = ListIngresosAsignados.Where(w => w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_ANIO == SelectIngreso.ID_ANIO
                    //    && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectIngreso.ID_INGRESO && w.ID_ABOGADO_TITULO == SelectColaborador.ABOGADO.ID_ABOGADO_TITULO);
                    //ListCausasPenales = new ObservableCollection<CausaPenalAsignacion>();
                    //ListCausasAsignadas = new ObservableCollection<AbogadoCausaPenalAsignacion>();
                    //if (ingresosAsignados.Any())
                    //{
                    //    EditarOficioAsignacion = true;
                    //    TextObservacionesInterno = ingresosAsignados.FirstOrDefault().OBSERV;
                    //    SelectEstatus = ingresosAsignados.FirstOrDefault().ID_ESTATUS_VISITA.HasValue ? ingresosAsignados.FirstOrDefault().ID_ESTATUS_VISITA.Value : (short)-1;
                    //    foreach (var item in SelectIngreso.CAUSA_PENAL.Where(whe => whe.ABOGADO_CAUSA_PENAL.Any(wh => wh.ABOGADO_INGRESO.ID_ABOGADO == SelectColaborador.ABOGADO.ABOGADO2.ID_ABOGADO)))
                    //    {
                    //        if (ingresosAsignados.Any(wh => wh.ABOGADO_CAUSA_PENAL.Any(w => w.ID_INGRESO == item.ID_INGRESO && w.ID_ANIO == item.ID_ANIO &&
                    //                w.ID_CENTRO == item.ID_CENTRO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_CAUSA_PENAL == item.ID_CAUSA_PENAL &&
                    //                    w.ID_ABOGADO_TITULO == SelectColaborador.ABOGADO.ID_ABOGADO_TITULO)))
                    //        {
                    //            ListCausasPenales.Add(new CausaPenalAsignacion { CAUSA_PENAL = item, ELEGIDO = true });
                    //        }
                    //        else
                    //        {
                    //            //if (ingresosAsignados.Any(w => w.ABOGADO.ABOGADO2.ABOGADO_INGRESO.Any(aboing=> aboing.ID_INGRESO == item.ID_INGRESO && 
                    //            //    aboing.ID_ANIO == item.ID_ANIO && aboing.ID_CENTRO == item.ID_CENTRO && aboing.ID_IMPUTADO == item.ID_IMPUTADO && 
                    //            //    aboing.ID_ABOGADO_TITULO == SelectColaborador.ABOGADO.ID_ABOGADO_TITULO)))
                    //            ListCausasPenales.Add(new CausaPenalAsignacion { CAUSA_PENAL = item, ELEGIDO = false });
                    //        }
                    //    }
                    //    ListCausasAsignadas = new ObservableCollection<AbogadoCausaPenalAsignacion>(ListCausasPenales.Where(w => w.ELEGIDO)
                    //         .Select(sel =>
                    //         new AbogadoCausaPenalAsignacion
                    //         {
                    //             ABOGADO_CAUSA_PENAL = sel.CAUSA_PENAL.ABOGADO_CAUSA_PENAL.First(w => w.ID_INGRESO == SelectAbogadoIngreso.ID_INGRESO &&
                    //                 w.ID_ANIO == SelectAbogadoIngreso.ID_ANIO && w.ID_CENTRO == SelectAbogadoIngreso.ID_CENTRO &&
                    //                 w.ID_IMPUTADO == SelectAbogadoIngreso.ID_IMPUTADO),
                    //             ELEGIDO = !(sel.CAUSA_PENAL.ABOGADO_CAUSA_PENAL.First(w => w.ID_INGRESO == SelectAbogadoIngreso.ID_INGRESO &&
                    //                     w.ID_ANIO == SelectAbogadoIngreso.ID_ANIO && w.ID_CENTRO == SelectAbogadoIngreso.ID_CENTRO &&
                    //                     w.ID_IMPUTADO == SelectAbogadoIngreso.ID_IMPUTADO).ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_CANCELADO ||
                    //                 sel.CAUSA_PENAL.ABOGADO_CAUSA_PENAL.First(w => w.ID_INGRESO == SelectAbogadoIngreso.ID_INGRESO &&
                    //                     w.ID_ANIO == SelectAbogadoIngreso.ID_ANIO && w.ID_CENTRO == SelectAbogadoIngreso.ID_CENTRO &&
                    //                     w.ID_IMPUTADO == SelectAbogadoIngreso.ID_IMPUTADO).ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_SUSPENDIDO),
                    //             ID_ESTATUS_VISITA = sel.CAUSA_PENAL.ABOGADO_CAUSA_PENAL.First(w => w.ID_INGRESO == SelectAbogadoIngreso.ID_INGRESO &&
                    //                 w.ID_ANIO == SelectAbogadoIngreso.ID_ANIO && w.ID_CENTRO == SelectAbogadoIngreso.ID_CENTRO &&
                    //                 w.ID_IMPUTADO == SelectAbogadoIngreso.ID_IMPUTADO).ID_ESTATUS_VISITA,
                    //             DESCR = sel.CAUSA_PENAL.ABOGADO_CAUSA_PENAL.First(w => w.ID_INGRESO == SelectAbogadoIngreso.ID_INGRESO &&
                    //                 w.ID_ANIO == SelectAbogadoIngreso.ID_ANIO && w.ID_CENTRO == SelectAbogadoIngreso.ID_CENTRO &&
                    //                 w.ID_IMPUTADO == SelectAbogadoIngreso.ID_IMPUTADO).ESTATUS_VISITA.DESCR,
                    //             DESHABILITA = sel.ELEGIDO
                    //         }));
                    //}
                    //else
                    //{
                    //    EditarOficioAsignacion = false;
                    //    ListCausasPenales = new ObservableCollection<CausaPenalAsignacion>(SelectIngreso.CAUSA_PENAL.Select(s => new CausaPenalAsignacion { CAUSA_PENAL = s, ELEGIDO = false }));
                        //}
                        #endregion
                    }
                else
                {
                    EditarOficioAsignacion = false;
                    ListCausasPenales = new ObservableCollection<CausaPenalAsignacion>(SelectIngreso.CAUSA_PENAL.Select(s => new CausaPenalAsignacion { CAUSA_PENAL = s, ELEGIDO = false }));
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer los datos del imputado.", ex);
            }
        }

        private void LimpiarAsignacion()
        {
            try
            {
                AnioBuscar = FolioBuscar = new Nullable<int>();
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                SelectExpediente = null;
                SelectIngreso = null;
                NombreBuscar = ApellidoMaternoBuscar = ApellidoPaternoBuscar = TextAnio = TextFolio = TextPaternoImputado = TextMaternoImputado =
                    TextNombreImputado = TextObservacionesInterno = string.Empty;
                SelectEstatus = -1;
                SelectExpediente = null;
                SelectIngreso = null;
                EstatusEnabled = AnioEnabled = FolioEnabled = PaternoEnabled = MaternoEnabled = NombreEnabled = ObservacionesInternoEnabled = false;
                ListCausasPenales = null;
                SelectCausaPenalAuxiliar = null;
                SelectAbogadoIngreso = null;
                ImagenIngreso = ImagenImputado = ImagenInterno = new Imagenes().getImagenPerson();
                ListCausasAsignadas = null;
                AbogadoTitularAnterior = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar la pantalla.", ex);
            }
        }

        private void LimpiarCampos()
        {
            try
            {
                TextCodigoTitular = TextPaternoTitular = TextMaternoTitular = TextNombreTitular = TextCodigo = TextPaterno = TextMaterno = TextNombre =
                    TextCodigoAbogado = TextCurp = TextRfc = TextTelefonoFijo = TextTelefonoMovil = TextCorreo = TextIne = TextCalle = TextNumInt =
                        TextPaternoAbogado = TextMaternoAbogado = TextNombreAbogado = TextObservaciones = SelectDiscapacitado = string.Empty;
                SelectFechaNacimiento = Fechas.GetFechaDateServer;
                SelectFechaAlta = SelectFechaNacimiento.ToString("dd/MM/yyyy");
                SelectSexo = "S";
                TextNumExt = TextCodigoPostal = new Nullable<int>();
                SelectPais = Parametro.PAIS;//82;
                SelectEstatusVisita = -1;
                ImagesToSave = ImageFrontal = null;
                ImagenAbogado = new Imagenes().getImagenPerson();
                SelectPersona = SelectColaborador = SelectAbogadoTitular = null;
                BuscarAbogadoEnabled = Credencializado = AsignacionEnabled = AbogadoTitularEnabled = EditarTitularEnabled = ImpresionGafeteFront = ImpresionGafeteBack = ImpresionGafete =
                    Editable = NuevoAbogado = FotoTomada = FotoActualizada = false;
                HuellasCapturadas = null;
                ListPersonasAuxiliar = null;
                ListPersonas = null;
                EditarTitularCommandParameter = "editar_titular";
                StaticSourcesViewModel.SourceChanged = false;
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
                if (SelectColaborador == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar o crear un colaborador.");
                    return;
                }
                if (string.IsNullOrEmpty(SelectColaborador.TELEFONO) && string.IsNullOrEmpty(SelectColaborador.TELEFONO_MOVIL))
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Se debe registrar un teléfono del colaborador.");
                    return;
                }
                var fotos = SelectColaborador.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG);
                if (!fotos.Any())
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debes tomarle foto al colaborador.");
                    return;
                }
                if (SelectColaborador.ID_TIPO_DISCAPACIDAD != null && SelectColaborador.ID_TIPO_DISCAPACIDAD > 0)
                {
                    if(SelectColaborador.TIPO_DISCAPACIDAD.HUELLA == "S")
                        if (SelectColaborador.PERSONA_BIOMETRICO.Count(w => w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9) == 0)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No tiene huellas capturadas.");
                            return;
                        }
                }
                else
                    if (SelectColaborador.PERSONA_BIOMETRICO.Count(w => w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9) == 0)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No tiene huellas capturadas.");
                        return;
                    }

                var TipoVisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                //var nip = SelectColaborador.PERSONA_NIP.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal);
                //if (!nip.Any())
                //{
                //    (new Dialogos()).ConfirmacionDialogo("Validación", "El colaborador no tiene NIP registrado.");
                //    return;
                //}
                if (SelectColaborador.ABOGADO == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar a un colaborador.");
                    return;
                }
                if (SelectColaborador.ABOGADO.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_SUSPENDIDO ||
                    SelectColaborador.ABOGADO.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_CANCELADO)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Este colaborador se encuentra " + SelectColaborador.ABOGADO.ESTATUS_VISITA.DESCR + ".");
                    return;
                }
                if (SelectColaborador.ABOGADO.IFE_FRONTAL == null || SelectColaborador.ABOGADO.IFE_REVERSO == null)
                {
                    var ife = Parametro.TIPO_DOCTO_IFE_LEGAL;
                    if (!SelectColaborador.ABOGADO.ABOGADO_DOCUMENTO.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal && w.ID_TIPO_DOCUMENTO == ife).Any())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Aun no cuenta con la digitalización de la identificación oficial.");
                        return;
                    }
                }
                if (SelectColaborador.ABOGADO.CEDULA_FRONTAL == null || SelectColaborador.ABOGADO.CEDULA_REVERSO == null)
                {
                    var credencialEscolar = Parametro.TIPO_DOCTO_CRED_LEGAL;
                    var cartaPasante = Parametro.TIPO_DOCTO_CARTA_LEGAL;
                    if (!SelectColaborador.ABOGADO.ABOGADO_DOCUMENTO.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal &&
                        (w.ID_TIPO_DOCUMENTO == credencialEscolar || w.ID_TIPO_DOCUMENTO == cartaPasante)).Any())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Aun no cuenta con la digitalización de la constancia escolar o carta pasante.");
                        return;
                    }
                }
                if (!SelectColaborador.ABOGADO.ABOGADO_DOCUMENTO.Any(w => w.ID_TIPO_DOCUMENTO == Parametro.TIPO_DOCTO_CARTA_LEGAL && w.ID_TIPO_VISITA == TipoVisitaLegal))
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debes digitalizar la carta pasante del colaborador.");
                    return;
                }
                //foreach (var item in Parametro.DOCUMENTOS_COLABORADORES)
                //{
                //    if (short.Parse(item.Split('-')[0]) == Parametro.ID_TIPO_VISITA_LEGAL)
                //    {
                //        doc = short.Parse(item.Split('-')[1]);
                //        if (!SelectColaborador.ABOGADO.ABOGADO_DOCUMENTO.Where(w => w.ID_TIPO_DOCUMENTO == ife || w.ID_TIPO_DOCUMENTO == credencialEscolar || w.ID_TIPO_DOCUMENTO == cartaPasante).Any())
                //        {
                //            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Aun no cuenta con los documentos necesarios.");
                //            return;
                //        }
                //    }
                //}
                #endregion

                #region GAFETE
                var gafetes = new List<GafeteAbogado>();
                GafeteView = new GafetesPVCView();
                var gaf = new GafeteAbogado();
                var centro = new cCentro().Obtener(4).FirstOrDefault();
                GafeteFrente = true;
                gaf.Discapacidad = SelectColaborador.ID_TIPO_DISCAPACIDAD == null || SelectColaborador.ID_TIPO_DISCAPACIDAD == 0 ? "NINGUNA" : SelectColaborador.TIPO_DISCAPACIDAD.DESCR;
                gaf.TipoPersona = SelectColaborador.ABOGADO.ABOGADO_TITULO.DESCR;
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
                gaf.RFC = SelectColaborador.RFC;
                gaf.Cedula = SelectColaborador.ABOGADO.CEDULA;
                var hoy = Fechas.GetFechaDateServer;
                gaf.Fecha = centro.MUNICIPIO.MUNICIPIO1.Replace("(BCN)", "").Trim() + " B.C. A " + hoy.ToString("dd DE MMMM DE yyyy").ToUpper();
                gaf.FechaAlta = hoy.ToString("dd/MM/yyyy");
                gaf.NombreAbogado = SelectColaborador.NOMBRE.Trim() + " " + SelectColaborador.PATERNO.Trim() + " " + SelectColaborador.MATERNO.Trim();
                gaf.NumeroCredencial = SelectColaborador.ID_PERSONA.ToString();
                gaf.Telefono = string.IsNullOrEmpty(SelectColaborador.TELEFONO) ? string.IsNullOrEmpty(SelectColaborador.TELEFONO_MOVIL) ? "N/A" : SelectColaborador.TELEFONO_MOVIL.Trim() : SelectColaborador.TELEFONO.Trim();
                var tipoVisita = "V I S I T A  LEGAL";
                //foreach (var item in nip.FirstOrDefault().TIPO_VISITA.DESCR.Trim())
                //{
                //    tipoVisita = tipoVisita + item + " ";
                //}
                gaf.TipoVisita = tipoVisita;
                Reporteador = GafeteView.GafetesPVCReport;
                //Reporteador.LocalReport.ReportPath = "Reportes/" + gafeteAbogado + ".rdlc";
                if (SelectColaborador.ID_TIPO_DISCAPACIDAD == null || SelectColaborador.ID_TIPO_DISCAPACIDAD == 0)
                    Reporteador.LocalReport.ReportPath = "Reportes/" + gafeteAbogado + ".rdlc";
                else
                    GafeteView.GafetesPVCReport.LocalReport.ReportPath = "Reportes/GafeteAbogadoFrenteDiscapacidad.rdlc";
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
                //GafeteView.Closed -= GafeteViewClosed;
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
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
                    if (SelectColaborador.ID_TIPO_DISCAPACIDAD == null || SelectColaborador.ID_TIPO_DISCAPACIDAD == 0)
                        GafeteView.GafetesPVCReport.LocalReport.ReportPath = "Reportes/GafeteAbogadoFrente.rdlc";
                    else
                        GafeteView.GafetesPVCReport.LocalReport.ReportPath = "Reportes/GafeteAbogadoFrenteDiscapacidad.rdlc";
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

        private void CheckBoxCausaPenalSelecccionada(object SelectedItem)
        {
            try
            {
                var checkBox = (CheckBox)(((object[])(SelectedItem))[1]);
                if (((object[])(SelectedItem))[0] is CausaPenalAsignacion)
                    ((CausaPenalAsignacion)(((object[])(SelectedItem))[0])).ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
                if (((object[])(SelectedItem))[0] is AbogadoCausaPenalAsignacion)
                    ((AbogadoCausaPenalAsignacion)(((object[])(SelectedItem))[0])).ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
            }
        }

        private async void HeaderSort(Object obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    if (!AbogadoTitularEnabled)
                        if (obj != null ? obj.ToString() == "Tipo visita" : false)
                        {
                            ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                            switch (HeaderSortin)
                            {
                                case true:
                                    Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        ListPersonas.InsertRange(ListPersonasAuxiliar.OrderByDescending(o => o.ABOGADO != null)
                                            .ThenByDescending(t => t.ABOGADO != null ? t.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR : t.ABOGADO != null));
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ordenar la búsqueda.", ex);
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
                        if (await new Dialogos().ConfirmarEliminar("Validación",
                            "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1)
                            return;
                        else
                        {
                            SelectColaborador = SelectPersonaAuxiliar;
                            return;
                        }
                    }
                    SelectColaborador = huella.SelectRegistro.Persona;
                    if (SelectColaborador.ABOGADO != null)
                    {
                        if (SelectColaborador.ABOGADO.ID_ESTATUS_VISITA == (short)enumEstatusVisita.CANCELADO_A)
                        {
                            //new Dialogos().ConfirmacionDialogo("Validación", "El estatus del colaborador seleccionado es cancelado, la información sera solo de consulta");
                            new Dialogos().ConfirmacionDialogo("Validación", "El estatus del colaborador seleccionado es cancelado");
                            //MenuGuardarEnabled = false;
                            //return;
                        }
                        else
                        { 
                            if(PInsertar || PEditar)
                                MenuGuardarEnabled = true;
                        }

                        if (SelectColaborador.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR)
                        {
                            var colabo = SelectColaborador;
                            var personas = ListPersonas;
                            LimpiarCampos();
                            LimpiarAsignacion();
                            SelectColaborador = colabo;
                            ListPersonas = personas;
                            NuevoAbogado = false;
                            Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                            AbogadoTitularEnabled = true;
                            SetValidaciones();
                            await GetDatosPersonaSeleccionada();
                            StaticSourcesViewModel.SourceChanged = false;
                            BuscarAbogadoEnabled = EditarTitularEnabled = true;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            Editable = false;
                        }
                        else
                        {
                            var AbogadoTitular = Parametro.ID_ABOGADO_TITULAR_ABOGADO;
                            if (SelectColaborador.ABOGADO.ID_ABOGADO_TITULO == AbogadoTitular)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Un abogado titular no puede convertirse en colaborador.");
                            }
                            else if (SelectColaborador.ABOGADO.ID_ABOGADO_TITULO != AbogadoTitular)
                            {
                                if (await new Dialogos().ConfirmarEliminar("Validación", "El abogado seleccionado no esta registrado como colaborador,"
                                        + " ¿Desea cambiarlo ahora?") == 1)
                                {
                                    LimpiarAsignacion();
                                    NuevoAbogado = true;
                                    Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                    AbogadoTitularEnabled = true;
                                    SetValidaciones();
                                    await GetDatosPersonaSeleccionada();
                                    BuscarAbogadoEnabled = true;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                    Editable = false;
                                }
                                else
                                {
                                    if (SelectColaborador != null)
                                    {
                                        TextCodigoAbogado = SelectColaborador.ID_PERSONA.ToString();
                                        TextNombreAbogado = SelectColaborador.NOMBRE;
                                        TextPaternoAbogado = SelectColaborador.PATERNO;
                                        TextMaternoAbogado = SelectColaborador.MATERNO;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (await new Dialogos().ConfirmarEliminar("Validación", "La persona seleccionada no esta registrada como colaborador,"
                                + " ¿Desea registrarla ahora?") == 1)
                        {
                            LimpiarAsignacion();
                            NuevoAbogado = true;
                            Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                            AbogadoTitularEnabled = true;
                            SetValidaciones();
                            await GetDatosPersonaSeleccionada();
                            BuscarAbogadoEnabled = true;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            Editable = false;
                        }
                        else {
                            if (SelectColaborador != null)
                            {
                                TextCodigoAbogado = SelectColaborador.ID_PERSONA.ToString();
                                TextNombreAbogado = SelectColaborador.NOMBRE;
                                TextPaternoAbogado = SelectColaborador.PATERNO;
                                TextMaternoAbogado = SelectColaborador.MATERNO;
                            }
                        }
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
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        Editable = true;
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
                        Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                        SetValidaciones();
                        await GetDatosPersonaSeleccionada();
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        Editable = false;
                    }
                    else
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "La persona seleccionada no esta registrada como abogado,"
                                + " ¿Desea registrarla ahora?") == 1)
                        {
                            LimpiarAsignacion();
                            Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                            SetValidaciones();
                            await GetDatosPersonaSeleccionada();
                            StaticSourcesViewModel.SourceChanged = false;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            Editable = false;
                        }
                        else
                        {
                            if (SelectPersona != null)
                            {
                                TextCodigoAbogado = SelectPersona.ID_PERSONA.ToString();
                                TextNombreAbogado = SelectPersona.NOMBRE;
                                TextPaternoAbogado = SelectPersona.PATERNO;
                                TextMaternoAbogado = SelectPersona.MATERNO;
                            }
                        }
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
                    await new Dialogos().ConfirmacionDialogoReturn("Validación", "Elija el tipo de documento a digitalizar");
                    return;
                }
                if (SelectColaborador == null ? false : SelectColaborador.ABOGADO == null)
                {
                    escaner.Hide();
                    await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe seleccionar un abogado.");
                    return;
                }
                escaner.Show();
                if (TipoDoctoAbogado == (short)enumTipoDocumentoAbogado.PERSONA)
                {
                    #region PERSONA
                    var Documentos = new cAbogadoDocumento().ObtenerTodos(/*4*/GlobalVar.gCentro, SelectedTipoDocumento.ID_TIPO_VISITA, SelectColaborador.ID_PERSONA,
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
                }
                else
                {
                    if (SelectIngreso == null)
                    {
                        escaner.Hide();
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes de elegir un imputado.");
                        return;
                    }
                    if (TipoDoctoAbogado == (short)enumTipoDocumentoAbogado.ABOGADO_CAUSA_PENAL)
                    {
                        #region ABOGADO_CAUSA_PENAL
                        var Documentos = new cAbogadoCausaPenalDocumento().ObtenerXAbogadoYCausaPenal(SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO,
                            SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO,
                            SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CAUSA_PENAL,
                            SelectColaborador.ID_PERSONA, SelectColaborador.ABOGADO.ID_ABOGADO_TITULO, SelectedTipoDocumento.ID_TIPO_DOCUMENTO).FirstOrDefault();
                        if (Documentos == null)
                        {
                            StaticSourcesViewModel.Mensaje("Digitalizacion", "Documento No Digitalizado", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                            obj.Visibility = Visibility.Collapsed;
                            DocumentoDigitalizado = null;
                            return;
                        }
                        if (Documentos.DOCTO == null)
                        {
                            StaticSourcesViewModel.Mensaje("Digitalizacion", "Documento No Digitalizado", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                            obj.Visibility = Visibility.Collapsed;
                            DocumentoDigitalizado = null;
                            return;
                        }
                        DocumentoDigitalizado = Documentos.DOCTO;
                        ObservacionDocumento = Documentos.OBSERV;
                        DatePickCapturaDocumento = Documentos.CAPTURA_FEC;
                        #endregion
                    }
                }
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
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "Elija el tipo de documento a digitalizar");
                        return;
                    }
                    if (DocumentoDigitalizado == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "Digitalice un documento para guardar");
                        return;
                    }
                    if (DocumentoDigitalizado.Length <= 0)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "Digitalice un documento para guardar");
                        return;
                    }
                    if (SelectColaborador == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe seleccionar una persona.");
                        return;
                    }
                    try
                    {
                        var hoy = Fechas.GetFechaDateServer;
                        if (TipoDoctoAbogado == (short)enumTipoDocumentoAbogado.PERSONA)
                        {
                            #region ABOGADO
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                var Documentos = new cAbogadoDocumento().ObtenerTodos(/*4*/GlobalVar.gCentro, 3, SelectColaborador.ID_PERSONA, 0).Where(w =>
                                    w.ID_TIPO_DOCUMENTO == SelectedTipoDocumento.ID_TIPO_DOCUMENTO && w.ID_TIPO_VISITA == SelectedTipoDocumento.ID_TIPO_VISITA).FirstOrDefault();
                                
                                if (Documentos == null)
                                    if (new cAbogadoDocumento().Insertar(new ABOGADO_DOCUMENTO
                                    {
                                        ID_ABOGADO = SelectColaborador.ID_PERSONA,
                                        DOCUMENTO = DocumentoDigitalizado,
                                        ID_CENTRO = /*4*/GlobalVar.gCentro,
                                        ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                                        ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                                        OBSERVACIONES = ObservacionDocumento,
                                        ALTA_FEC = hoy
                                    }))
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                else
                                    if (new cAbogadoDocumento().Actualizar(new ABOGADO_DOCUMENTO
                                    {
                                        ID_ABOGADO = SelectColaborador.ID_PERSONA,
                                        DOCUMENTO = DocumentoDigitalizado,
                                        ID_CENTRO = /*4*/GlobalVar.gCentro,
                                        ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                                        ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                                        OBSERVACIONES = ObservacionDocumento,
                                        ALTA_FEC = hoy
                                    }))
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Actualizado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un error al Actualizar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                CargarListaTipoDocumentoDigitalizado();
                            });
                            #endregion
                        }
                        else
                        {
                            if (SelectIngreso == null)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes de elegir un imputado.");
                                return;
                            }
                            if (TipoDoctoAbogado == (short)enumTipoDocumentoAbogado.ABOGADO_CAUSA_PENAL)
                            {
                                if (SelectCausaAsignada == null)
                                {
                                    await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debes de elegir una causa penal.");
                                    return;
                                }
                                #region ABOGADO_CP_DOCTO
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                                {
                                    var Documentos = new cAbogadoCausaPenalDocumento().ObtenerXAbogadoYCausaPenal(SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO,
                                        SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO,
                                        SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CAUSA_PENAL,
                                        SelectColaborador.ID_PERSONA, SelectColaborador.ABOGADO.ID_ABOGADO_TITULO, SelectedTipoDocumento.ID_TIPO_DOCUMENTO)
                                    .Where(w => w.ID_TIPO_DOCUMENTO == SelectedTipoDocumento.ID_TIPO_DOCUMENTO &&
                                        w.ID_TIPO_VISITA == SelectedTipoDocumento.ID_TIPO_VISITA).FirstOrDefault();

                                    if (Documentos == null)
                                    {
                                        if (new cAbogadoCausaPenalDocumento().Insertar(new ABOGADO_CP_DOCTO
                                        {
                                            ID_ABOGADO = SelectColaborador.ID_PERSONA,
                                            ID_ABOGADO_TITULO = SelectColaborador.ABOGADO.ID_ABOGADO_TITULO,
                                            DOCTO = DocumentoDigitalizado,
                                            ID_CENTRO = SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO,
                                            ID_ANIO = SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO,
                                            ID_IMPUTADO = SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO,
                                            ID_INGRESO = SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO,
                                            ID_CAUSA_PENAL = SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CAUSA_PENAL,
                                            ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                                            ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                                            OBSERV = ObservacionDocumento,
                                            CAPTURA_FEC = hoy,
                                            ID_ABOGADO_TITULAR = SelectAbogadoTitular.ID_PERSONA
                                        }))
                                        {
                                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                            {
                                                StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                            }));
                                        }
                                        else
                                        {
                                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                            {
                                                StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                            }));
                                        }
                                    }
                                    else
                                    {
                                        if (new cAbogadoCausaPenalDocumento().Actualizar(new ABOGADO_CP_DOCTO
                                        {
                                            ID_ABOGADO = SelectColaborador.ID_PERSONA,
                                            ID_ABOGADO_TITULO = SelectColaborador.ABOGADO.ID_ABOGADO_TITULO,
                                            DOCTO = DocumentoDigitalizado,
                                            ID_CENTRO = SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO,
                                            ID_ANIO = SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO,
                                            ID_IMPUTADO = SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO,
                                            ID_INGRESO = SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO,
                                            ID_CAUSA_PENAL = SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CAUSA_PENAL,
                                            ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                                            ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                                            OBSERV = ObservacionDocumento,
                                            CAPTURA_FEC = hoy,
                                            ID_ABOGADO_TITULAR = SelectAbogadoTitular.ID_PERSONA
                                        }))
                                        {
                                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                            {
                                                StaticSourcesViewModel.Mensaje("Digitalización", "Documento Actualizado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                            }));
                                        }
                                        else
                                        {
                                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                            {
                                                StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Actualizar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                            }));
                                        }
                                    }

                                    //var tipoDocto = SelectedTipoDocumento;
                                    //var docto = DocumentoDigitalizado;
                                    //var obs = ObservacionDocumento;

                                    CargarListaTipoDocumentoCausaPenalDigitalizado();

                                    //SelectedTipoDocumento = tipoDocto;
                                    //DocumentoDigitalizado = docto;
                                    //ObservacionDocumento = obs;
                                });
                                #endregion
                            }
                        }

                        /********************************************/
                        if (SelectColaborador != null)
                        {
                            SelectColaborador = new cPersona().Obtener(SelectColaborador.ID_PERSONA).FirstOrDefault();
                            if (TabAsignacion)
                            {
                                ListIngresosAsignados = new ObservableCollection<ABOGADO_INGRESO>(SelectColaborador.ABOGADO.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_COLABORADOR && w.ID_ESTATUS_VISITA == (short)enumEstatusVisita.AUTORIZADO));
                                ListCausasAsignadas = null;
                            }
                        }
                        /********************************************/

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
                foreach (var item in Parametro.DOCUMENTOS_COLABORADORES)
                {
                    doc = short.Parse(item.Split('-')[1]);
                    if (ListTipoDocumentoAux.Where(w => w.ID_TIPO_DOCUMENTO == doc).Any())
                    {
                        doctosAux.Add(ListTipoDocumentoAux.Where(w => w.ID_TIPO_DOCUMENTO == doc).FirstOrDefault());
                    }
                }
                var ListDocumentos = new cAbogadoDocumento().ObtenerTodos(/*4*/GlobalVar.gCentro, 3, SelectColaborador.ID_PERSONA, 0).ToList();
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
        void CargarListaTipoDocumentoCausaPenalDigitalizado()
        {
            try
            {
                var ListTipoDocumentoAux = new ObservableCollection<TipoDocumento>(new cTipoDocumento().ObtenerAbogadoIngreso(Parametro.ID_TIPO_VISITA_LEGAL)
                    .Where(w => Parametro.DOCUMENTOS_COLABORADOR_INGRESO.Where(wh => wh.Contains(w.ID_TIPO_DOCUMENTO.ToString())).Any())
                    .Select(s => new TipoDocumento
                    {
                        DESCR = s.DESCR,
                        DIGITALIZADO = false,
                        ID_TIPO_DOCUMENTO = s.ID_TIPO_DOCUMENTO,
                        ID_TIPO_VISITA = s.ID_TIPO_VISITA
                    }).OrderBy(o => o.DESCR).ToList());
                var ListDocumentos = new cAbogadoCausaPenalDocumento().ObtenerXAbogadoYCausaPenal(SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO,
                    SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO,
                    SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CAUSA_PENAL,
                    SelectColaborador.ID_PERSONA, SelectColaborador.ABOGADO.ID_ABOGADO_TITULO, 0).ToList();
                foreach (var item in ListDocumentos)
                {
                    ListTipoDocumentoAux.Where(w => w.ID_TIPO_DOCUMENTO == item.ID_TIPO_DOCUMENTO).FirstOrDefault().DIGITALIZADO = true;
                }
                ListTipoDocumento = new ObservableCollection<TipoDocumento>(ListTipoDocumentoAux.OrderBy(o => o.ID_TIPO_DOCUMENTO));
                TipoDoctoAbogado = (short)enumTipoDocumentoAbogado.ABOGADO_CAUSA_PENAL;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los tipos de documento.", ex);
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