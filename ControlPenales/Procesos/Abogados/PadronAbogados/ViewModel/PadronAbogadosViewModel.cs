using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using LinqKit;
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
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class PadronAbogadosViewModel : FingerPrintScanner
    {
        public PadronAbogadosViewModel() { }

        private async void Load_Window(PadronAbogadosView Window)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    CargarDatos();
                    StaticSourcesViewModel.SourceChanged = false;
                });

                escaner.EscaneoCompletado += delegate
                {
                    DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                    //ObservacionDocumento = string.Empty;
                    DocumentoDigitalizado = escaner.ScannedDocument;

                    if (AutoGuardado ? DocumentoDigitalizado != null : false)
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
                if (ImagenAbogado.Length != new Imagenes().getImagenPerson().Length)
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
                    var img = ImagesToSave.Where(w => w.FrameName == Picture.Name).FirstOrDefault();
                    if (img != null)
                    {
                        ImagesToSave.Remove(img);
                    }
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

                ImageFrontal = ImageFrontal ?? new List<ImageSourceToSave>();
                if (CamaraWeb.ImageControls.Where(w => w.Name == Picture.Name).Any())
                {
                    Picture.Source = CamaraWeb.TomarFoto(Picture);
                    ImageFrontal.Add(new ImageSourceToSave
                    {
                        FrameName = Picture.Name,
                        ImageCaptured = (BitmapSource)Picture.Source
                    });
                    Processing = true;
                    StaticSourcesViewModel.Mensaje("FOTO DE FRENTE", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    CamaraWeb.QuitarFoto(Picture);
                    ImageFrontal.Remove(ImageFrontal.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                }
                BotonTomarFotoEnabled = ImageFrontal != null ? ImageFrontal.Count == 1 : false;
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
                Lista_Sources = escaner.Sources();
                if (Lista_Sources.Count > 0) SelectedSource = Lista_Sources.Where(w => w.Default).SingleOrDefault();
                HojasMaximo = string.Format("Escaneo permitido: {0} documentos máximo.", escaner.HojasMaximo);

                var estatus = new cEstatusVisita().ObtenerTodos();
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
                    ListTipoDiscapacidad.Insert(0, new TIPO_DISCAPACIDAD { ID_TIPO_DISCAPACIDAD = -1, DESCR = "SELECCIONE" });
                    SelectEstatus = -1;
                    SelectDiscapacitado = string.Empty;
                    SelectPais = Parametro.PAIS; //82;
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla para tomar foto.", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.PADRON_ABOGADOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

        private async void ActualizarAsignacion()
        {
            try
            {
                bool EsAdministrativo = false;
                ABOGADO_CAUSA_PENAL AbogadoTitularAnterior = null;
                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un interno");
                    return;
                }

                if (ListCausasPenales != null ? ListCausasPenales.Count(w => w.ELEGIDO == true) == 0 : true)
                {
                    //new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una causa penal");
                    //return;
                    /*Validar Si el interno tiene abogado con el estatus administrativo*/
                    var icp = new cAbogadoIngreso().ObtenerAdministrativosXIngreso(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, null, "T").FirstOrDefault();
                    if (icp != null)
                    {
                        if (SelectPersona.ID_PERSONA != icp.ID_ABOGADO)
                        {
                            if (await new Dialogos().ConfirmarEliminar("Advertencia!", "Este interno ya cuenta con un abogado administrativo [" +
                                            icp.ABOGADO.ID_ABOGADO + " - " +
                                            icp.ABOGADO.PERSONA.NOMBRE.Trim() + " " +
                                            icp.ABOGADO.PERSONA.PATERNO.Trim() + " " +
                                            icp.ABOGADO.PERSONA.MATERNO.Trim() + "] , desea continuar y elegirlo como el administrativo?") != 1)
                                return;
                            else
                                EsAdministrativo = true;
                        }
                    }
                    else
                        EsAdministrativo = true;
                }
                //ABOGADO_CAUSA_PENAL AbogadoTitularAnterior = null;
                //if (ListCausasPenales.Count(w => w.ELEGIDO == true) == 0)
                //    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una causa penal");
                else
                {
                    foreach (var obj in ListCausasPenales.Where(w => w.ELEGIDO == true))
                    {
                        if (obj.CAUSA_PENAL.ABOGADO_CAUSA_PENAL != null)
                        {
                            AbogadoTitularAnterior = obj.CAUSA_PENAL.ABOGADO_CAUSA_PENAL.Where(w => w.ID_ABOGADO != SelectPersona.ID_PERSONA && w.ID_ABOGADO_TITULO == "T" && w.ID_ESTATUS_VISITA == 13 /*(short)enumEstatusVisita.AUTORIZADO*/).FirstOrDefault();
                        }
                    }
                }

                if (AbogadoTitularAnterior != null)
                    if (await new Dialogos().ConfirmarEliminar("Advertencia!", "Este interno ya cuenta con un abogado titular [" +
                                            AbogadoTitularAnterior.ID_ABOGADO + " - " +
                                            AbogadoTitularAnterior.ABOGADO_INGRESO.ABOGADO.PERSONA.NOMBRE.Trim() + " " +
                                            AbogadoTitularAnterior.ABOGADO_INGRESO.ABOGADO.PERSONA.PATERNO.Trim() + " " +
                                            AbogadoTitularAnterior.ABOGADO_INGRESO.ABOGADO.PERSONA.MATERNO.Trim() + "] para una de las causas penales seleccionadas [" +
                                            AbogadoTitularAnterior.CAUSA_PENAL.CP_ANIO + "/" + AbogadoTitularAnterior.CAUSA_PENAL.CP_FOLIO
                                            + "], desea continuar y elegirlo como el titular?") != 1)
                        return;
                var respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando Asignación", () =>
                {
                    try
                    {
                        var hoy = Fechas.GetFechaDateServer;
                        #region Abogado Ingreso
                        var ai = new ABOGADO_INGRESO();
                        ai.ID_ABOGADO = SelectPersona.ID_PERSONA;
                        ai.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        ai.ID_ANIO = SelectIngreso.ID_ANIO;
                        ai.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        ai.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        ai.ID_ABOGADO_TITULO = "T";
                        ai.CAPTURA_FEC = hoy;
                        ai.OBSERV = TextObservacionesInterno;
                        ai.ADMINISTRATIVO = EsAdministrativo ? "S" : "N";
                        ai.ID_ESTATUS_VISITA = SelectEstatus;//(short)enumEstatusVisita.AUTORIZADO;
                        #endregion

                        #region AbogadoCausaPenal
                        var acp = new List<ABOGADO_CAUSA_PENAL>();
                        if (ListCausasPenales != null)
                            foreach (var obj in ListCausasPenales.Where(w => w.ELEGIDO == true))
                            {
                                acp.Add(new ABOGADO_CAUSA_PENAL()
                                {
                                    ID_ABOGADO = SelectPersona.ID_PERSONA,
                                    ID_ABOGADO_TITULO = "T",
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    ID_CAUSA_PENAL = obj.CAUSA_PENAL.ID_CAUSA_PENAL,
                                    CAPTURA_FEC = hoy,
                                    ID_ESTATUS_VISITA = SelectEstatus//(short)enumEstatusVisita.AUTORIZADO
                                });
                            }
                        #endregion

                        if (new cPersona().ActualizarAbogadoAsignacionTransaccion(ai, acp))
                            return true;
                        else
                            return false;//new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la asignación");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al asignaciar interno.", ex);
                        return false;
                    }
                });

                if (respuesta)
                {
                    LimpiarAsignacion();
                    //LimpiarCampos();
                    //SelectPersona = new cPersona().ObtenerPersonaXID(abogadoIngreso.ID_ABOGADO).FirstOrDefault();
                    SelectPersona = new cPersona().ObtenerPersonaXID(SelectPersona.ID_PERSONA).FirstOrDefault();
                    await GetDatosPersonaSeleccionada();
                    BuscarAbogadoEnabled = true;
                    new Dialogos().ConfirmacionDialogo("Éxito", "La asignación se guardo correctamente");
                    StaticSourcesViewModel.SourceChanged = false;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la asignación");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al asignaciar interno.", ex);
            }
        }

        private void GuardarAsignacion()
        {
            try
            {
                #region Guardar Asignacion
                if (ListIngresosAsignados == null)
                    ListIngresosAsignados = new ObservableCollection<ABOGADO_INGRESO>();
                if (ListCausasPenales == null)
                    ListCausasPenales = new ObservableCollection<CausaPenalAsignacion>();
                var hoy = Fechas.GetFechaDateServer;
                var abogadoIngreso = new ABOGADO_INGRESO
                {
                    CAPTURA_FEC = hoy,
                    ID_ABOGADO = SelectPersona.ID_PERSONA,
                    ID_ABOGADO_TITULO = SelectPersona.ABOGADO.ID_ABOGADO_TITULO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    OBSERV = TextObservacionesInterno,
                    ID_ESTATUS_VISITA = SelectEstatus,
                    ADMINISTRATIVO = AbogadoAdministrativo ? "S" : "N"
                };
                Application.Current.Dispatcher.Invoke((Action)(async delegate
                {
                    try
                    {
                        var causasPenales = new List<ABOGADO_CAUSA_PENAL>();
                        var causasPenalesColaboradores = new List<ABOGADO_CAUSA_PENAL>();
                        var VisitaAutorizada = Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                        if (ListCausasPenales.Count > 0)
                        {
                            if (ListCausasPenales.Where(w => w.ELEGIDO).Any())
                            {
                                var AbogadoTitular = Parametro.ID_ABOGADO_TITULAR_ABOGADO;
                                var VisitaCancelado = Parametro.ID_ESTATUS_VISITA_CANCELADO;
                                AbogadoTitularAnterior = new List<ABOGADO_CAUSA_PENAL>();
                                foreach (var item in ListCausasPenales.Where(w => w.ELEGIDO))
                                {
                                    if (item.CAUSA_PENAL.ABOGADO_CAUSA_PENAL.Where(w => w.ID_ABOGADO != SelectPersona.ABOGADO.ID_ABOGADO &&
                                        w.ID_ABOGADO_TITULO == AbogadoTitular &&
                                        (w.ID_ESTATUS_VISITA == VisitaAutorizada ?
                                        w.ABOGADO_INGRESO.ID_ESTATUS_VISITA == VisitaAutorizada : false)).Any())
                                    {
                                        AbogadoTitularAnterior.Add(item.CAUSA_PENAL.ABOGADO_CAUSA_PENAL.Where(w => w.ID_ABOGADO != SelectPersona.ABOGADO.ID_ABOGADO &&
                                            w.ID_ABOGADO_TITULO == AbogadoTitular &&
                                            (w.ID_ESTATUS_VISITA == VisitaAutorizada ?
                                            w.ABOGADO_INGRESO.ID_ESTATUS_VISITA == VisitaAutorizada : false)).FirstOrDefault());
                                    }
                                }
                                var bandera = false;
                                var existente = new ABOGADO_CAUSA_PENAL();
                                ListCancelarCausaPenalAsignacion = new ObservableCollection<ABOGADO_CAUSA_PENAL>();
                                foreach (var item in ListCausasPenales.Where(w => !w.ELEGIDO))
                                {
                                    if (SelectAbogadoIngreso != null)
                                    {
                                        existente = SelectAbogadoIngreso.ABOGADO_CAUSA_PENAL.Where(w => w.ID_CENTRO == item.CAUSA_PENAL.ID_CENTRO && w.ID_ANIO == item.CAUSA_PENAL.ID_ANIO &&
                                               w.ID_IMPUTADO == item.CAUSA_PENAL.ID_IMPUTADO && w.ID_INGRESO == item.CAUSA_PENAL.ID_INGRESO && w.ID_CAUSA_PENAL == item.CAUSA_PENAL.ID_CAUSA_PENAL).Any() ?
                                               SelectAbogadoIngreso.ABOGADO_CAUSA_PENAL.Where(w => w.ID_CENTRO == item.CAUSA_PENAL.ID_CENTRO && w.ID_ANIO == item.CAUSA_PENAL.ID_ANIO &&
                                               w.ID_IMPUTADO == item.CAUSA_PENAL.ID_IMPUTADO && w.ID_INGRESO == item.CAUSA_PENAL.ID_INGRESO && w.ID_CAUSA_PENAL == item.CAUSA_PENAL.ID_CAUSA_PENAL).FirstOrDefault() :
                                               null;
                                        if (existente != null)
                                        {
                                            causasPenales.Add(new ABOGADO_CAUSA_PENAL
                                            {
                                                ID_CENTRO = existente.ID_CENTRO,
                                                ID_ANIO = existente.ID_ANIO,
                                                ID_IMPUTADO = existente.ID_IMPUTADO,
                                                ID_INGRESO = existente.ID_INGRESO,
                                                ID_CAUSA_PENAL = existente.ID_CAUSA_PENAL,
                                                ID_ABOGADO = existente.ID_ABOGADO,
                                                ID_ABOGADO_TITULO = existente.ID_ABOGADO_TITULO,
                                                ID_ESTATUS_VISITA = VisitaCancelado,
                                                CAPTURA_FEC = existente.CAPTURA_FEC
                                            });
                                            if (!bandera)
                                                bandera = existente.ID_ESTATUS_VISITA == VisitaAutorizada;
                                        }
                                    }
                                }
                                if (bandera ? await new Dialogos().ConfirmarEliminar("Advertencia!",
                                    "Ha quitado alguna asignacion, está seguro que desea quitarla? El estatus se actualizara a CANCELADO.") != 1 : false)
                                    return;

                                if (AbogadoTitularAnterior.Count > 0 ? await new Dialogos().ConfirmarEliminar("Advertencia!", "Este interno ya cuenta con un abogado titular [" +
                                        AbogadoTitularAnterior.FirstOrDefault().ID_ABOGADO + " - " +
                                        AbogadoTitularAnterior.FirstOrDefault().ABOGADO_INGRESO.ABOGADO.PERSONA.NOMBRE.Trim() + " " +
                                        AbogadoTitularAnterior.FirstOrDefault().ABOGADO_INGRESO.ABOGADO.PERSONA.PATERNO.Trim() + " " +
                                        AbogadoTitularAnterior.FirstOrDefault().ABOGADO_INGRESO.ABOGADO.PERSONA.MATERNO.Trim() + "] para una de las causas penales seleccionadas [" +
                                        AbogadoTitularAnterior.FirstOrDefault().CAUSA_PENAL.CP_ANIO + "/" + AbogadoTitularAnterior.FirstOrDefault().CAUSA_PENAL.CP_FOLIO
                                        + "], desea continuar y elegirlo como el titular?") != 1 : false)
                                {
                                    LimpiarAsignacion();
                                    AbogadoTitularAnterior = new List<ABOGADO_CAUSA_PENAL>();
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            AbogadoTitularAnterior = null;
                        }


                        if (AbogadoTitularAnterior != null ? AbogadoTitularAnterior.Count > 0 : false)
                        {
                            var statusCancelado = Parametro.ID_ESTATUS_VISITA_CANCELADO;
                            var statusAutorizado = Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                            foreach (var itm in AbogadoTitularAnterior.Where(w => w.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ABOGADO &&
                                    (w.ID_ESTATUS_VISITA == statusAutorizado ? w.ABOGADO_INGRESO.ID_ESTATUS_VISITA == statusAutorizado : false)))
                            {
                                causasPenales.Add(new ABOGADO_CAUSA_PENAL
                                    {
                                        CAPTURA_FEC = itm.CAPTURA_FEC,
                                        ID_ABOGADO = itm.ID_ABOGADO,
                                        ID_ABOGADO_TITULO = itm.ID_ABOGADO_TITULO,
                                        ID_CENTRO = itm.ID_CENTRO,
                                        ID_ANIO = itm.ID_ANIO,
                                        ID_IMPUTADO = itm.ID_IMPUTADO,
                                        ID_INGRESO = itm.ID_INGRESO,
                                        ID_CAUSA_PENAL = itm.ID_CAUSA_PENAL,
                                        ID_ESTATUS_VISITA = statusCancelado,
                                    });
                                foreach (var item in new cAbogadoCausaPenal().ObtenerXAbogadoTitular(itm.ID_ABOGADO).Where(w => w.ID_CENTRO == itm.ID_CENTRO && w.ID_ANIO == itm.ID_ANIO &&
                                    w.ID_IMPUTADO == itm.ID_IMPUTADO && w.ID_INGRESO == itm.ID_INGRESO && w.ID_CAUSA_PENAL == itm.ID_CAUSA_PENAL && w.ID_ESTATUS_VISITA == statusAutorizado))
                                {
                                    causasPenales.Add(new ABOGADO_CAUSA_PENAL
                                    {
                                        CAPTURA_FEC = item.CAPTURA_FEC,
                                        ID_ABOGADO = item.ID_ABOGADO,
                                        ID_ABOGADO_TITULO = item.ID_ABOGADO_TITULO,
                                        ID_CENTRO = item.ID_CENTRO,
                                        ID_ANIO = item.ID_ANIO,
                                        ID_IMPUTADO = itm.ID_IMPUTADO,
                                        ID_INGRESO = item.ID_INGRESO,
                                        ID_CAUSA_PENAL = item.ID_CAUSA_PENAL,
                                        ID_ESTATUS_VISITA = statusCancelado
                                    });
                                }

                            }
                        }
                        //abogadoIngreso = new cAbogadoIngreso().ObtenerTodos(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, SelectPersona.ID_PERSONA).FirstOrDefault();

                        if (ListCausasPenales.Count > 0 ? ListCausasPenales.Where(w => w.ELEGIDO).Any() : false)
                        {
                            var ingresoAsignacion = SelectIngreso;
                            foreach (var itm in ListCausasPenales.Where(w => w.ELEGIDO).Select(s => s.CAUSA_PENAL))
                            {
                                if (SelectAbogadoIngreso != null)
                                {
                                    if (SelectAbogadoIngreso.ABOGADO_CAUSA_PENAL.Where(aboCP => aboCP.ID_CENTRO == itm.ID_CENTRO && aboCP.ID_ABOGADO_TITULO == SelectPersona.ABOGADO.ID_ABOGADO_TITULO &&
                                        aboCP.ID_CAUSA_PENAL == itm.ID_CAUSA_PENAL && aboCP.ID_ANIO == itm.ID_ANIO && aboCP.ID_IMPUTADO == itm.ID_IMPUTADO && aboCP.ID_INGRESO == itm.ID_INGRESO).Any())
                                    {
                                        causasPenales.Add(new ABOGADO_CAUSA_PENAL
                                        {
                                            CAPTURA_FEC = SelectAbogadoIngreso.ABOGADO_CAUSA_PENAL.Where(aboCP => aboCP.ID_CENTRO == itm.ID_CENTRO &&
                                                aboCP.ID_ABOGADO_TITULO == SelectPersona.ABOGADO.ID_ABOGADO_TITULO && aboCP.ID_CAUSA_PENAL == itm.ID_CAUSA_PENAL &&
                                                aboCP.ID_ANIO == itm.ID_ANIO && aboCP.ID_IMPUTADO == itm.ID_IMPUTADO && aboCP.ID_INGRESO == itm.ID_INGRESO).FirstOrDefault().CAPTURA_FEC,
                                            ID_ABOGADO = SelectPersona.ABOGADO.ID_ABOGADO,
                                            ID_ABOGADO_TITULO = SelectPersona.ABOGADO.ID_ABOGADO_TITULO,
                                            ID_ANIO = itm.ID_ANIO,
                                            ID_CAUSA_PENAL = itm.ID_CAUSA_PENAL,
                                            ID_CENTRO = itm.ID_CENTRO,
                                            ID_IMPUTADO = itm.ID_IMPUTADO,
                                            ID_INGRESO = itm.ID_INGRESO,
                                            ID_ESTATUS_VISITA = VisitaAutorizada
                                        });
                                    }
                                    else
                                    {
                                        causasPenales.Add(new ABOGADO_CAUSA_PENAL
                                        {
                                            CAPTURA_FEC = hoy,
                                            ID_ABOGADO = SelectPersona.ABOGADO.ID_ABOGADO,
                                            ID_ABOGADO_TITULO = SelectPersona.ABOGADO.ID_ABOGADO_TITULO,
                                            ID_ANIO = itm.ID_ANIO,
                                            ID_CAUSA_PENAL = itm.ID_CAUSA_PENAL,
                                            ID_CENTRO = itm.ID_CENTRO,
                                            ID_IMPUTADO = itm.ID_IMPUTADO,
                                            ID_INGRESO = itm.ID_INGRESO,
                                            ID_ESTATUS_VISITA = VisitaAutorizada
                                        });
                                    }
                                }
                                else
                                {
                                    causasPenales.Add(new ABOGADO_CAUSA_PENAL
                                    {
                                        CAPTURA_FEC = hoy,
                                        ID_ABOGADO = SelectPersona.ABOGADO.ID_ABOGADO,
                                        ID_ABOGADO_TITULO = SelectPersona.ABOGADO.ID_ABOGADO_TITULO,
                                        ID_ANIO = itm.ID_ANIO,
                                        ID_CAUSA_PENAL = itm.ID_CAUSA_PENAL,
                                        ID_CENTRO = itm.ID_CENTRO,
                                        ID_IMPUTADO = itm.ID_IMPUTADO,
                                        ID_INGRESO = itm.ID_INGRESO,
                                        ID_ESTATUS_VISITA = VisitaAutorizada
                                    });
                                }
                            }
                        }
                        if (new cPersona().InsertarAbogadoAsignacionTransaccion(abogadoIngreso, EditarOficioAsignacion, causasPenales))
                        {
                            LimpiarAsignacion();
                            LimpiarCampos();
                            SelectPersona = new cPersona().ObtenerPersonaXID(abogadoIngreso.ID_ABOGADO).FirstOrDefault();
                            await GetDatosPersonaSeleccionada();
                            BuscarAbogadoEnabled = true;
                            StaticSourcesViewModel.SourceChanged = false;
                            (new Dialogos()).ConfirmacionDialogo("Exito!", "Se ha guardado la asignacion del imputado seleccionado");
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se pudo guardar la informacion.");

                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar la asignación del interno.", ex);
                    }
                }));
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar la asignación del interno.", ex);
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
                        if (TabRegistro)
                        {
                            if (SelectPersona == null)
                                if (!Insertable)
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debes registrar un nuevo abogado o seleccionear uno existente para modificarlo.");
                                    break;
                                }

                            if (HasErrors())
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El campo " + base.Error + " es requerido.");
                                break;
                            }
                            //await StaticSourcesViewModel.CargarDatosMetodoAsync(async () =>
                            //{
                            //await GuardarAbogado();
                            GuardarAbogadoNew();
                            //StaticSourcesViewModel.SourceChanged = false;
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

                            //await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            //{
                            ActualizarAsignacion();
                            StaticSourcesViewModel.SourceChanged = false;
                            //});
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al .", ex);
                    }
                    break;
                case "insertar_menu":
                    if (TabRegistro ? StaticSourcesViewModel.SourceChanged : false)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar, esta seguro que desea insertar a uno nuevo abogado?") != 1)
                            return;
                    }
                    //Limpiamos asignacion
                    LimpiarAsignacion();
                    ListIngresosAsignados = null;
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
                case "buscar_menu":
                    if (TabRegistro)
                    {
                        SelectPersonaAuxiliar = SelectPersona;
                        TextNombre = TextMaterno = TextPaterno = string.Empty;
                        ListPersonas = null;
                        SelectPersona = null;
                        //TextNombre = TextNombreAbogado;
                        //TextMaterno = TextMaternoAbogado;
                        //TextPaterno = TextPaternoAbogado;
                        //var pers3 = SelectPersona;
                        //await StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarPersonasSinCodigo);
                        //SelectPersonaAuxiliar = pers3;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        Editable = true;
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
                    break;
                case "limpiar_menu":
                    if (TabRegistro)
                    {
                        //base.ClearRules();
                        //ValidarEnabled = NuevoAbogado = Editable = DiscapacitadoEnabled = false;
                        //LimpiarCampos();
                        StaticSourcesViewModel.SourceChanged = false;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new PadronAbogadosView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new PadronAbogadosViewModel();
                    }
                    if (TabAsignacion)
                    {
                        base.ClearRules();
                        LimpiarAsignacion();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
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
                    if (ImageFrontal != null ? ImageFrontal.Count == 1 : false)
                    {
                        if (!FotoTomada)
                            ImageFrontal = new List<ImageSourceToSave>();
                    }
                    else
                    {
                        ImageFrontal = new List<ImageSourceToSave>();
                        ImageFrontal.Add(new ImageSourceToSave { FrameName = "ImgSenaParticular", ImageCaptured = new Imagenes().ConvertByteToBitmap(ImagenAbogado) });
                        BotonTomarFotoEnabled = true;
                    }
                    if (CamaraWeb != null)
                    {
                        await CamaraWeb.ReleaseVideoDevice();
                        CamaraWeb = null;
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    break;
                case "aceptar_tomar_foto_senas":
                    try
                    {
                        if (ImageFrontal != null ? ImageFrontal.Count == 1 : false)
                        {
                            FotoTomada = true;
                            FotoActualizada = Editable ? (ImagenAbogado != new Imagenes().ConvertBitmapToByte(ImageFrontal.FirstOrDefault().ImageCaptured)) : true;
                            ImagenAbogado = new Imagenes().ConvertBitmapToByte(ImageFrontal.FirstOrDefault().ImageCaptured);
                            StaticSourcesViewModel.SourceChanged = true;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                            await CamaraWeb.ReleaseVideoDevice();
                            break;
                        }
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "DEBES DE TOMAR UNA FOTO.");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al asignar la fotografía.", ex);
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
                            if ((ImpresionGafeteBack && ImpresionGafeteFront) ? !ImpresionGafete : false)
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
                        await new Dialogos().ConfirmacionDialogoReturn("Advertencia!", "No ha seleccionado un abogado.");
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
                        if (SelectPersona == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Advertencia!", "No ha seleccionado una persona.");
                            break;
                        }
                        if (SelectIngreso == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Advertencia!", "No ha seleccionado un ingreso.");
                            break;
                        }
                        if (SelectExpediente == null)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Advertencia!", "No ha seleccionado un imputado.");
                            break;
                        }
                        if (SelectCausaAsignada == null)
                        {
                            if (SelectAbogadoIngreso == null)
                                return;

                            if (!SelectAbogadoIngreso.ABOGADO_ING_DOCTO.Any())
                            {
                                var existe = false;
                                var asignacion = Parametro.DOCUMENTO_ASIGNACION_INTERNO;
                                var visita = short.Parse(asignacion[0]);
                                var doctoAsignado = short.Parse(asignacion[1]);
                                var VisitaAutorizada = SelectIngreso.ABOGADO_INGRESO.Count > 0 ? Parametro.ID_ESTATUS_VISITA_AUTORIZADO : 0;
                                var AbogadoTitular = SelectIngreso.ABOGADO_INGRESO.Count > 0 ? Parametro.ID_ABOGADO_TITULAR_ABOGADO : string.Empty;
                                foreach (var item in SelectIngreso.ABOGADO_INGRESO)
                                {
                                    if (item.ID_ABOGADO != SelectPersona.ABOGADO.ID_ABOGADO && item.ID_ABOGADO_TITULO == AbogadoTitular &&
                                        item.ID_ESTATUS_VISITA == VisitaAutorizada && item.ADMINISTRATIVO == "S" &&
                                        item.ABOGADO_ING_DOCTO.Where(w => w.ID_TIPO_VISITA == visita && w.ID_TIPO_DOCUMENTO == doctoAsignado &&
                                            w.ABOGADO_INGRESO.ID_ESTATUS_VISITA == VisitaAutorizada).Any())
                                    {
                                        existe = true;
                                        break;
                                    }
                                }
                                if (existe)
                                {
                                    if (await new Dialogos().ConfirmarEliminar("Advertencia!", "Este interno ya cuenta con un abogado administrativo, desea continuar y elegirlo como tal?") == 1)
                                    {
                                        AbogadoAdministrativo = true;
                                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarListaTipoDocumentoIngresoDigitalizado(); });
                                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                                    }
                                    else
                                        AbogadoAdministrativo = false;
                                }
                                else
                                {
                                    if (SelectAbogadoIngreso.ADMINISTRATIVO != "S")
                                    {
                                        if (SelectIngreso.CAUSA_PENAL.Count > 0 ? (ListCausasAsignadas.Count > 0 ?
                                            (await new Dialogos().ConfirmarEliminar("Advertencia!", "El abogado sera catalogado como administrativo, esta de acuerdo?") == 1)
                                            : false) : false)
                                        {
                                            AbogadoAdministrativo = true;
                                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarListaTipoDocumentoIngresoDigitalizado(); });
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                                        }
                                        else
                                            AbogadoAdministrativo = false;
                                    }
                                    else//Abogado Administrativo
                                    {
                                        if (SelectIngreso.CAUSA_PENAL == null || SelectIngreso.CAUSA_PENAL.Count == 0)
                                        {
                                            AbogadoAdministrativo = true;
                                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarListaTipoDocumentoIngresoDigitalizado(); });
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                                        }
                                        else
                                        {
                                            AbogadoAdministrativo = true;
                                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarListaTipoDocumentoIngresoDigitalizado(); });
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                                        }
                                        //AbogadoAdministrativo = false;
                                    }
                                }
                            }
                            else
                            {
                                AbogadoAdministrativo = false;
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarListaTipoDocumentoIngresoDigitalizado(); });
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                            }
                        }
                        else
                        {
                            AbogadoAdministrativo = false;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarListaTipoDocumentoCausaPenalDigitalizado(); });
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la digitalización de documentos.", ex);
                    }
                    break;
                case "guardar_documento":
                    GuardarDocumento();
                    break;
                #endregion

                #region BUSCAR_ABOGADOS
                case "nueva_busqueda_visitante":
                    TextPaterno = TextMaterno = TextNombre = string.Empty;
                    ListPersonas = null;
                    //TextPaternoAbogado = TextMaternoAbogado = TextNombreAbogado = TextPaterno = TextMaterno = TextNombre = string.Empty;
                    //ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    //ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    break;
                case "buscar_abogado":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    //var pers1 = SelectPersona;
                    SelectPersonaAuxiliar = SelectPersona;
                    TextNombre = TextMaterno = TextPaterno = string.Empty;
                    ListPersonas = null;
                    SelectPersona = null;
                    //TextNombre = TextNombreAbogado;
                    //TextMaterno = TextMaternoAbogado;
                    //TextPaterno = TextPaternoAbogado;
                    //await StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarPersonasSinCodigo);
                    //SelectPersonaAuxiliar = pers1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    Editable = true;
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
                    if (SelectPersona != null)
                        SelectPersonaAuxiliar = SelectPersona;
                    //var pers2 = SelectPersona;
                    BuscarPersonasSinCodigo();
                    //SelectPersonaAuxiliar = pers2;
                    break;
                case "seleccionar_buscar_persona":
                    try
                    {
                        if (SelectPersona == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a una persona.");
                            break;
                        }
                        if (StaticSourcesViewModel.SourceChanged ? await new Dialogos().ConfirmarEliminar("Advertencia!",
                            "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1 : false)
                            return;
                        if (SelectPersona.ABOGADO != null)
                        {
                            if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ABOGADO)
                            {
                                LimpiarAsignacion();
                                NuevoAbogado = NuevoTitular = false;
                                Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                SetValidaciones();
                                await GetDatosPersonaSeleccionada();
                                StaticSourcesViewModel.SourceChanged = false;
                                BuscarAbogadoEnabled = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                Editable = false;
                            }
                            else
                            {
                                if (await new Dialogos().ConfirmarEliminar("Advertencia!", "La persona seleccionada no esta registrado como abogado titular,"
                                        + "¿Desea cambiarlo ahora?") == 1)
                                {
                                    LimpiarAsignacion();
                                    NuevoAbogado = false;
                                    NuevoTitular = Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                    SetValidaciones();
                                    await GetDatosPersonaSeleccionada();
                                    BuscarAbogadoEnabled = true;
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
                        }
                        else
                        {
                            if (await new Dialogos().ConfirmarEliminar("Advertencia!", "La persona seleccionada no esta registrada como abogado,"
                                    + " ¿Desea registrarla ahora?") == 1)
                            {
                                LimpiarAsignacion();
                                NuevoTitular = false;
                                NuevoAbogado = Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                SetValidaciones();
                                await GetDatosPersonaSeleccionada();
                                BuscarAbogadoEnabled = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                Editable = false;
                                StaticSourcesViewModel.SourceChanged = false;
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
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de la persona seleccionada.", ex);
                    }
                    break;
                case "cancelar_buscar_persona":
                    //var pers = SelectPersonaAuxiliar;
                    ListPersonas = null;
                    SelectPersona = SelectPersonaAuxiliar;//pers;
                    SelectPersonaAuxiliar = null;
                    TextNombre = TextMaterno = TextPaterno = string.Empty;
                    if (SelectPersona == null)
                    {
                        TextNombreAbogado = TextPaternoAbogado = TextMaternoAbogado = string.Empty;
                    }
                    else
                    {
                        TextCodigoAbogado = SelectPersona.ID_PERSONA.ToString();
                        TextNombreAbogado = SelectPersona.NOMBRE;
                        TextPaternoAbogado = SelectPersona.PATERNO;
                        TextMaternoAbogado = SelectPersona.MATERNO;
                    }
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
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un imputado.");
                            break;
                        }
                        if (SelectIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un ingreso.");
                            break;
                        }
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                                return;
                            }
                        }
                        if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no pertenece a este centro.");
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                            return;
                        }
                        var expedient = SelectExpediente;
                        var ingres = SelectIngreso;
                        var person = SelectPersona;
                        LimpiarAsignacion();
                        LimpiarCampos();
                        SelectPersonaAuxiliar = null;
                        SelectIngresoAuxiliar = null;
                        SelectExpedienteAuxiliar = null;
                        SelectExpediente = expedient;
                        SelectIngreso = ingres;
                        SelectPersona = person;
                        EstatusEnabled = AnioEnabled = FolioEnabled = PaternoEnabled = MaternoEnabled = NombreEnabled =
                            ObservacionesInternoEnabled = true;
                        if (ListIngresosAsignados.Where(wh => wh.ABOGADO_CAUSA_PENAL.Where(w => w.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                            w.ID_ABOGADO_TITULO == SelectPersona.ABOGADO.ID_ABOGADO_TITULO && w.ID_ANIO == SelectIngreso.ID_ANIO &&
                            w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO).Any()).Any())
                            SelectAbogadoIngreso = ListIngresosAsignados.Where(wh => wh.ABOGADO_CAUSA_PENAL.Where(w => w.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                                w.ID_ANIO == SelectIngreso.ID_ANIO && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO &&
                                w.ID_ABOGADO_TITULO == SelectPersona.ABOGADO.ID_ABOGADO_TITULO).Any()).FirstOrDefault();
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
            }
        }

        private async void GuardarAbogadoNew()
        {
            var respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando Abogado", () =>
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
                        abogado.ID_ABOGADO_TITULO = "T";
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
                                abogado.ID_ABOGADO = SelectPersona.ID_PERSONA;
                                if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO != "T")
                                {
                                    abogado.CREDENCIALIZADO = "N";
                                }
                                else
                                    abogado.CREDENCIALIZADO = SelectPersona.ABOGADO.CREDENCIALIZADO;

                                abogado.ID_ABOGADO_TITULO = "T";
                                abogado.NORIGINAL = SelectPersona.ABOGADO.NORIGINAL;
                                abogado.ABOGADO_TITULAR = SelectPersona.ABOGADO.ABOGADO_TITULAR;
                                abogado.ID_JUZGADO = null;//SelectPersona.ABOGADO.ID_JUZGADO;
                                abogado.ALTA_FEC = SelectPersona.ABOGADO.ALTA_FEC;
                                #endregion
                            }
                            abogado.ULTIMA_MOD_FEC = hoy;

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
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los datos del abogado.", ex);
                        return false;
                    }
                });

            if (respuesta)
            {
                new Dialogos().ConfirmacionDialogo("Éxito", "El abogado se guardo correctamente");
                StaticSourcesViewModel.SourceChanged = false;
            }
            //else
            //    new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar abogado");
        }

        private async Task GuardarAbogado()
        {
            try
            {
                if ((SelectPersona != null ? SelectPersona.ID_PERSONA <= 0 : true) ? !Insertable : false)
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes crear un nuevo abogado o seleccionar uno ya existente para modificarlo.");
                        }));
                    return;
                }

                bool edit = false;
                if (SelectPersona != null)
                {
                    if (SelectPersona.ABOGADO != null)
                        edit = true;
                }

                if (ImagesToSave == null)
                    ImagesToSave = new List<ImageSourceToSave>();
                var VisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                var AbogadoTitular = Parametro.ID_ABOGADO_TITULAR_ABOGADO;
                var persona = new SSP.Servidor.PERSONA
                {
                    ID_PERSONA = edit ? SelectPersona.ID_PERSONA : int.Parse(Fechas.GetFechaDateServer.Year + "" + new cPersona().GetSequence<int>("ID_PERSONA_SEQ")),
                    CORIGINAL = edit ? SelectPersona.CORIGINAL : null,
                    CORREO_ELECTRONICO = TextCorreo,
                    CURP = TextCurp,
                    DOMICILIO_CALLE = TextCalle,
                    DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                    DOMICILIO_NUM_EXT = TextNumExt,
                    DOMICILIO_NUM_INT = TextNumInt,
                    ESTADO_CIVIL = edit ? SelectPersona.ESTADO_CIVIL : null,
                    FEC_NACIMIENTO = SelectFechaNacimiento,
                    ID_COLONIA = SelectColonia,
                    ID_ENTIDAD = SelectEntidad,
                    ID_ETNIA = edit ? SelectPersona.ID_ETNIA : null,
                    ID_MUNICIPIO = SelectMunicipio,
                    ID_PAIS = SelectPais,
                    ID_TIPO_DISCAPACIDAD = SelectTipoDiscapacidad,
                    ID_TIPO_PERSONA = edit ? SelectPersona.ID_TIPO_PERSONA : 2,
                    IFE = TextIne,
                    LUGAR_NACIMIENTO = edit ? SelectPersona.LUGAR_NACIMIENTO : null,
                    MATERNO = TextMaternoAbogado,
                    PATERNO = TextPaternoAbogado,
                    NOMBRE = TextNombreAbogado,
                    NACIONALIDAD = SelectPais,
                    NORIGINAL = edit ? SelectPersona.NORIGINAL : new Nullable<int>(),
                    RFC = TextRfc,
                    SEXO = SelectSexo,
                    SMATERNO = edit ? SelectPersona.SMATERNO : null,
                    SPATERNO = edit ? SelectPersona.SPATERNO : null,
                    SNOMBRE = edit ? SelectPersona.SNOMBRE : null,
                    TELEFONO = TextTelefonoFijo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                    TELEFONO_MOVIL = TextTelefonoMovil.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
                };
                var hoy = Fechas.GetFechaDateServer;

                #region ABOGADO
                var abogado = new ABOGADO();
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    abogado = new ABOGADO
                    {
                        ID_ABOGADO = persona.ID_PERSONA,
                        ID_ABOGADO_TITULO = AbogadoTitular,
                        //ID_TIPO_ABOGADO = 0,
                        IFE_FRONTAL = ImagesToSave.Where(w => w.FrameName == "ImgFrente").Any() ?
                            new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgFrente").FirstOrDefault().ImageCaptured) :
                            null,
                        IFE_REVERSO = ImagesToSave.Where(w => w.FrameName == "ImgReverso").Any() ?
                            new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgReverso").FirstOrDefault().ImageCaptured) :
                            null,
                        CEDULA = TextCedulaCJF,
                        CEDULA_FRONTAL = ImagesToSave.Where(w => w.FrameName == "ImgFrenteCedula").Any() ?
                            new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgFrenteCedula").FirstOrDefault().ImageCaptured) :
                            null,
                        CEDULA_REVERSO = ImagesToSave.Where(w => w.FrameName == "ImgReversoCedula").Any() ?
                            new Imagenes().ConvertBitmapToByte(ImagesToSave.Where(w => w.FrameName == "ImgReversoCedula").FirstOrDefault().ImageCaptured) :
                            null,
                        NORIGINAL = persona.NORIGINAL,
                        ALTA_FEC = edit ? NuevoAbogado ? hoy : SelectPersona.ABOGADO.ALTA_FEC : hoy,
                        OBSERV = TextObservaciones,
                        ULTIMA_MOD_FEC = hoy,
                        ID_ESTATUS_VISITA = SelectEstatusVisita,
                        ABOGADO_TITULAR = edit ? NuevoAbogado ? new Nullable<int>() : SelectPersona.ABOGADO.ABOGADO_TITULAR : new Nullable<int>(),
                        ID_JUZGADO = edit ? NuevoAbogado ? new Nullable<short>() : SelectPersona.ABOGADO.ID_JUZGADO : new Nullable<short>(),
                        CJF = edit ? NuevoAbogado ? null : SelectPersona.ABOGADO.CJF : null,
                        CREDENCIALIZADO = edit ? NuevoAbogado ? "N" : SelectPersona.ABOGADO.CREDENCIALIZADO : "N"
                    };
                }));
                #endregion

                #region NIP
                var personaNips = new List<PERSONA_NIP>();
                if (edit ? NuevoAbogado : true)
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

                if (!new cPersona().InsertarAbogadoTransaccion(persona, edit, abogado, NuevoAbogado, NuevoTitular, Parametro.ID_ABOGADO_TITULAR_ABOGADO, Parametro.ID_ESTATUS_VISITA_CANCELADO, personaNips, personaFotos, personaHuellas, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO))
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se pudo guardar la informacion.");
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los datos del abogado.", ex);
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
                BuscarPersonasSinCodigo();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
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
                                        .ThenByDescending(t => t.ABOGADO != null ? t.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ABOGADO : t.ABOGADO != null));
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
                HeaderSortin = false;
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                EmptyExpedienteVisible = ListExpediente.Count > 0 ? false : true;
                if (ListExpediente.Count == 1 ? (AnioBuscar != null && FolioBuscar != null) : false)
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
                        if (ingreso != null)
                            if (ingreso.ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                                return;
                            }
                    }
                    if (ingreso != null)
                        if (ingreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        //if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no pertenece a este centro.");
                            return;
                        }
                    if (ingreso != null)
                    {
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (ingreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ANIO.ToString() + "/" +
                                ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                            return;
                        }
                    }
                    SelectExpediente = ListExpediente[0];
                    SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    var expedient = SelectExpediente;
                    var ingres = SelectIngreso;
                    LimpiarAsignacion();
                    SelectExpediente = expedient;
                    SelectIngreso = ingres;
                    EstatusEnabled = AnioEnabled = FolioEnabled = PaternoEnabled = MaternoEnabled = NombreEnabled = ObservacionesInternoEnabled = true;
                    if (ListIngresosAsignados != null)
                    {
                        if (ListIngresosAsignados.Where(wh => wh.ABOGADO_CAUSA_PENAL.Where(w => w.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                           w.ID_ABOGADO_TITULO == SelectPersona.ABOGADO.ID_ABOGADO_TITULO && w.ID_ANIO == SelectIngreso.ID_ANIO &&
                           w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO).Any()).Any())
                            SelectAbogadoIngreso = ListIngresosAsignados.Where(wh => wh.ABOGADO_CAUSA_PENAL.Where(w => w.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                                w.ID_ABOGADO_TITULO == SelectPersona.ABOGADO.ID_ABOGADO_TITULO && w.ID_ANIO == SelectIngreso.ID_ANIO &&
                                w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO).Any()).FirstOrDefault();
                        else
                            GetDatosInternoSeleccionado();
                    }
                    else
                        GetDatosInternoSeleccionado();
                    StaticSourcesViewModel.SourceChanged = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    return;
                }
                if (ListExpediente.Count == 0 ? (!string.IsNullOrEmpty(NombreD) || !string.IsNullOrEmpty(PaternoD) || !string.IsNullOrEmpty(MaternoD)) : false)
                    new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún imputado con esos datos.");
                EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<IMPUTADO>();
            }
        }

        private async void BuscarPersonas()
        {
            try
            {
                var p = SelectPersona;
                ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                SelectPersona = p;
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
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    Editable = false;
                }
                else
                {
                    var legal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                    //var persona = SelectPersona;
                    SelectPersonaAuxiliar = SelectPersona;
                    ListPersonas.InsertRange(await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                        /*new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodos(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, int.Parse(TextCodigoAbogado))
                              .OrderByDescending(o => o.ABOGADO != null).ThenByDescending(t => t.ID_TIPO_PERSONA == legal))));*/
                       new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodos(string.Empty, string.Empty, string.Empty, int.Parse(TextCodigoAbogado))
                             .OrderByDescending(o => o.ABOGADO != null).ThenByDescending(t => t.ID_TIPO_PERSONA == legal))));
                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                    //SelectPersonaAuxiliar = persona;
                    if (ListPersonas.Count == 1)
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await new Dialogos().ConfirmarEliminar("Advertencia!",
                                "Existen cambios sin guardar, esta segúro que desea seleccionar a otra persona?") == 1)
                                SelectPersona = ListPersonas.FirstOrDefault();
                            else
                            {
                                SelectPersona = SelectPersonaAuxiliar;
                                return;
                            }
                        }
                        else
                            SelectPersona = ListPersonas.FirstOrDefault();
                        if (SelectPersona.ABOGADO != null)
                        {
                            if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ABOGADO)
                            {
                                LimpiarAsignacion();
                                NuevoAbogado = NuevoTitular = false;
                                Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                SetValidaciones();
                                await GetDatosPersonaSeleccionada();
                                StaticSourcesViewModel.SourceChanged = false;
                                BuscarAbogadoEnabled = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                Editable = false;
                            }
                            else
                            {
                                if (await new Dialogos().ConfirmarEliminar("Advertencia!", "La persona seleccionada no esta registrado como abogado titular,"
                                        + " ¿Desea cambiarlo ahora?") == 1)
                                {
                                    LimpiarAsignacion();
                                    NuevoAbogado = AsignacionEnabled = false;
                                    NuevoTitular = Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                    SetValidaciones();
                                    await GetDatosPersonaSeleccionada();
                                    BuscarAbogadoEnabled = true;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                    Editable = false;
                                }
                                else
                                {
                                    SelectPersona = SelectPersonaAuxiliar;
                                    if (SelectPersona != null)
                                    {
                                        TextCodigoAbogado = SelectPersona.ID_PERSONA.ToString();
                                        TextNombreAbogado = SelectPersona.NOMBRE;
                                        TextPaternoAbogado = SelectPersona.PATERNO;
                                        TextMaternoAbogado = SelectPersona.MATERNO;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (await new Dialogos().ConfirmarEliminar("Advertencia!", "La persona seleccionada no esta registrada como abogado,"
                                    + " desea registrarla ahora?") == 1)
                            {
                                LimpiarAsignacion();
                                AsignacionEnabled = false;
                                NuevoTitular = false;
                                NuevoAbogado = Editable = ValidarEnabled = DiscapacitadoEnabled = true;
                                SetValidaciones();
                                await GetDatosPersonaSeleccionada();
                                BuscarAbogadoEnabled = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                Editable = false;
                            }
                            else
                            {
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
                        SelectPersonaAuxiliar = p;
                        ListPersonas = null;
                        SelectPersona = null;
                        TextNombre = TextNombreAbogado;
                        TextPaterno = TextPaternoAbogado;
                        TextMaterno = TextMaternoAbogado;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        Editable = true;
                    }
                }
                if (ListPersonas != null)
                    EmptyBuscarRelacionInternoVisible = !(ListPersonas.Count > 0);
                else
                    EmptyBuscarRelacionInternoVisible = false;
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
                TextNombreAbogado = TextNombre == null ? (TextNombre = string.Empty) : TextNombre;
                TextPaternoAbogado = TextPaterno == null ? (TextPaterno = string.Empty) : TextPaterno;
                TextMaternoAbogado = TextMaterno == null ? (TextMaterno = string.Empty) : TextMaterno;
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    var person = SelectPersona;
                    ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                    if (PopUpsViewModels.VisibleBuscarPersonasExistentes == Visibility.Collapsed)
                    {
                        SelectPersonaAuxiliar = person;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        Editable = true;
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

        private async Task GetDatosPersonaSeleccionada()
        {
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                TextCodigoAbogado = SelectPersona.ID_PERSONA.ToString();
                TextPaternoAbogado = !string.IsNullOrEmpty(SelectPersona.PATERNO) ? SelectPersona.PATERNO.Trim() : string.Empty;
                TextMaternoAbogado = !string.IsNullOrEmpty(SelectPersona.MATERNO) ? SelectPersona.MATERNO.Trim() : string.Empty;
                TextNombreAbogado = !string.IsNullOrEmpty(SelectPersona.NOMBRE) ? SelectPersona.NOMBRE.Trim() : string.Empty;
                SelectSexo = SelectPersona.SEXO;
                SelectFechaNacimiento = SelectPersona.FEC_NACIMIENTO.HasValue ? SelectPersona.FEC_NACIMIENTO.Value : hoy;
                TextCurp = SelectPersona.CURP;
                TextRfc = SelectPersona.RFC;
                TextTelefonoFijo = SelectPersona.TELEFONO;
                TextTelefonoMovil = SelectPersona.TELEFONO_MOVIL;
                TextCorreo = SelectPersona.CORREO_ELECTRONICO;
                TextIne = SelectPersona.IFE;
                SelectDiscapacitado = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value > 0 ? "S" : "N" : "N";
                SelectTipoDiscapacidad = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value : (short)0;
                Insertable = false;
                if (SelectPersona.ABOGADO != null)
                {
                    TextCedulaCJF = SelectPersona.ABOGADO.CEDULA;
                    SelectFechaAlta = SelectPersona.ABOGADO.ALTA_FEC.HasValue ? SelectPersona.ABOGADO.ALTA_FEC.Value.ToString("dd/MM/yyyy") : hoy.ToString("dd/MM/yyyy");
                    TextObservaciones = SelectPersona.ABOGADO.OBSERV;

                    ImagesToSave = new List<ImageSourceToSave>();
                    SelectEstatusVisita = SelectPersona.ABOGADO.ID_ESTATUS_VISITA.HasValue ? SelectPersona.ABOGADO.ID_ESTATUS_VISITA.Value : (short)-1;
                    Credencializado = ImpresionGafeteFront = ImpresionGafeteBack = ImpresionGafete = SelectPersona.ABOGADO.CREDENCIALIZADO == "S";
                    if (SelectPersona.ABOGADO.IFE_FRONTAL != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgFrente", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.IFE_FRONTAL) });
                    if (SelectPersona.ABOGADO.IFE_REVERSO != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgReverso", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.IFE_REVERSO) });
                    if (SelectPersona.ABOGADO.CEDULA_FRONTAL != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgFrenteCedula", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.CEDULA_FRONTAL) });
                    if (SelectPersona.ABOGADO.CEDULA_REVERSO != null)
                        ImagesToSave.Add(new ImageSourceToSave { FrameName = "ImgReversoCedula", ImageCaptured = new Imagenes().ConvertByteToBitmap(SelectPersona.ABOGADO.CEDULA_REVERSO) });
                    var parametro = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    ListIngresosAsignados = new ObservableCollection<ABOGADO_INGRESO>(SelectPersona.ABOGADO.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ABOGADO ? 
                        w.ID_ESTATUS_VISITA != (short)enumEstatusVisita.CANCELADO_A ?
                            !parametro.Contains(w.INGRESO.ID_ESTATUS_ADMINISTRATIVO)
                        : false
                    : false));
                }
                else
                {
                    ImpresionGafeteFront = ImpresionGafeteBack = ImpresionGafete = Credencializado = false;
                    TextCedulaCJF = TextObservaciones = string.Empty;
                    ImagesToSave = new List<ImageSourceToSave>();
                    SelectFechaAlta = hoy.ToString("dd/MM/yyyy");
                    SelectEstatusVisita = -1;
                    ListIngresosAsignados = new ObservableCollection<ABOGADO_INGRESO>();
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
                //    TextNip = SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_VISITA == VisitaLegal).Any() ?
                //        (SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_VISITA == VisitaLegal).FirstOrDefault().NIP.HasValue ?
                //        SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_VISITA == VisitaLegal).FirstOrDefault().NIP.Value.ToString() : string.Empty)
                //        : string.Empty;
                //}
                SelectPais = -1;
                await TaskEx.Delay(100);
                SelectPais = SelectPersona.ID_PAIS;
                await TaskEx.Delay(100);
                SelectEntidad = SelectPersona.ID_ENTIDAD;
                await TaskEx.Delay(100);
                SelectMunicipio = SelectPersona.ID_MUNICIPIO;
                await TaskEx.Delay(100);
                if (SelectPersona.ID_COLONIA.HasValue ? SelectPersona.ID_COLONIA.Value > 0 : false)
                    SelectColonia = SelectPersona.ID_COLONIA;
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
                var VisitaAutorizada = Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                SelectEstatus = VisitaAutorizada;
                if (ListIngresosAsignados != null ? ListIngresosAsignados.Count > 0 : false)
                {
                    ListCausasPenales = new ObservableCollection<CausaPenalAsignacion>();
                    /*Codigo Nuevo*******************************************************************************/
                    if (SelectIngreso.CAUSA_PENAL != null)
                    {
                        var ing = SelectPersona.ABOGADO.ABOGADO_INGRESO.Where(w => w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_ANIO == SelectIngreso.ID_ANIO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault();
                        if (ing != null)
                        {
                            var cp_a = ing.ABOGADO_CAUSA_PENAL;
                            foreach (var obj in SelectIngreso.CAUSA_PENAL.Where(w => w.ID_ESTATUS_CP != 2 && w.ID_ESTATUS_CP != 4))
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
                            foreach (var obj in SelectIngreso.CAUSA_PENAL)
                            {
                                ListCausasPenales.Add(new CausaPenalAsignacion
                                {
                                    CAUSA_PENAL = obj,
                                    ELEGIDO = false
                                });
                            }
                        }
                    }

                    /**************************************/
                    //var ingresosAsignados = ListIngresosAsignados.Where(w => w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_ANIO == SelectIngreso.ID_ANIO
                    //    && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                    //    w.ID_ABOGADO_TITULO == SelectPersona.ABOGADO.ID_ABOGADO_TITULO);
                    //ListCausasPenales = new ObservableCollection<CausaPenalAsignacion>();
                    //if (ingresosAsignados.Any())
                    //{
                    //    EditarOficioAsignacion = true;
                    //    TextObservacionesInterno = ingresosAsignados.FirstOrDefault().OBSERV;
                    //    SelectEstatus = ingresosAsignados.FirstOrDefault().ID_ESTATUS_VISITA.Value;
                    //    //var VisitaAutorizada = Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                    //    foreach (var item in SelectIngreso.CAUSA_PENAL)
                    //    {
                    //        if (ingresosAsignados.Where(wh => wh.ABOGADO_CAUSA_PENAL.Where(w => w.ID_INGRESO == item.ID_INGRESO &&
                    //                w.ID_ABOGADO_TITULO == SelectPersona.ABOGADO.ID_ABOGADO_TITULO &&
                    //                w.ID_ANIO == item.ID_ANIO && w.ID_CENTRO == item.ID_CENTRO && w.ID_IMPUTADO == item.ID_IMPUTADO &&
                    //                w.ID_CAUSA_PENAL == item.ID_CAUSA_PENAL).Any()).Any())
                    //            ListCausasPenales.Add(new CausaPenalAsignacion
                    //            {
                    //                CAUSA_PENAL = item,
                    //                ELEGIDO = item.ABOGADO_CAUSA_PENAL.Where(wh =>
                    //                        wh.ID_ESTATUS_VISITA == VisitaAutorizada &&
                    //                        wh.ID_ABOGADO == SelectPersona.ID_PERSONA &&
                    //                        wh.ID_ABOGADO_TITULO == SelectPersona.ABOGADO.ID_ABOGADO_TITULO).Any()
                    //            });
                    //        else
                    //            ListCausasPenales.Add(new CausaPenalAsignacion { CAUSA_PENAL = item, ELEGIDO = false });
                    //    }
                    //}
                    //else
                    //{
                    //    EditarOficioAsignacion = false;
                    //    ListCausasPenales = new ObservableCollection<CausaPenalAsignacion>(SelectIngreso.CAUSA_PENAL.Select(s => new CausaPenalAsignacion { CAUSA_PENAL = s, ELEGIDO = false }));
                    //}
                }
                else
                {
                    EditarOficioAsignacion = false;
                    ListCausasPenales = new ObservableCollection<CausaPenalAsignacion>(SelectIngreso.CAUSA_PENAL.Select(s => new CausaPenalAsignacion { CAUSA_PENAL = s, ELEGIDO = false }));
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del imputado.", ex);
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
                EstatusEnabled = false;
                AnioEnabled = FolioEnabled = PaternoEnabled = MaternoEnabled = NombreEnabled = ObservacionesInternoEnabled = false;
                ListCausasPenales = null;
                SelectCausaPenalAuxiliar = null;
                SelectAbogadoIngreso = null;
                ImagenIngreso = ImagenImputado = ImagenInterno = new Imagenes().getImagenPerson();
                ListCausasAsignadas = null;
                AbogadoTitularAnterior = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar pantalla.", ex);
            }
        }

        private void LimpiarCampos()
        {
            try
            {
                TextCodigoAbogado = TextPaternoAbogado = TextMaternoAbogado = TextNombreAbogado = TextCodigo = TextPaterno = TextMaterno = TextNombre =
                    TextCurp = TextRfc = TextTelefonoFijo = TextTelefonoMovil = TextCorreo = TextIne = TextCedulaCJF = TextCalle = TextNumInt =
                        TextObservaciones = SelectDiscapacitado = string.Empty;
                SelectFechaNacimiento = Fechas.GetFechaDateServer;
                SelectFechaAlta = SelectFechaNacimiento.ToString("dd/MM/yyyy");
                SelectSexo = "S";
                TextNumExt = TextCodigoPostal = new Nullable<int>();
                SelectPais = Parametro.PAIS; //82;
                SelectEstatusVisita = -1;
                ImagesToSave = ImageFrontal = null;
                ImagenAbogado = new Imagenes().getImagenPerson();
                SelectPersona = null;
                BuscarAbogadoEnabled = Credencializado = ImpresionGafeteFront = ImpresionGafeteBack = ImpresionGafete = Editable = NuevoAbogado = FotoTomada =
                AbogadoAdministrativo = StaticSourcesViewModel.SourceChanged = FotoActualizada = false;
                HuellasCapturadas = null;
                ListPersonasAuxiliar = null;
                ListPersonas = null;
                Insertable = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar pantalla.", ex);
            }
        }

        private void CrearGafete()
        {
            try
            {
                #region VALIDACIONES
                if (SelectPersona == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar o crear un abogado.");
                    return;
                }
                if (string.IsNullOrEmpty(SelectPersona.TELEFONO) && string.IsNullOrEmpty(SelectPersona.TELEFONO_MOVIL))
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Se debe registrar un teléfono del abogado.");
                    return;
                }
                var fotos = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG);
                if (!fotos.Any())
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes tomarle foto al abogado.");
                    return;
                }

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
                //var nip = SelectPersona.PERSONA_NIP.Where(w => w.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_LEGAL);
                //if (!nip.Any())
                //{
                //    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El abogado no tiene NIP registrado.");
                //    return;
                //}
                if (SelectPersona.ABOGADO == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a un abogado.");
                    return;
                }
                if (SelectPersona.ABOGADO.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_SUSPENDIDO ||
                    SelectPersona.ABOGADO.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_CANCELADO)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Este abogado se encuentra " + SelectPersona.ABOGADO.ESTATUS_VISITA.DESCR + ".");
                    return;
                }
                var TipoVisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
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
                    if (!SelectPersona.ABOGADO.ABOGADO_DOCUMENTO.Where(w => w.ID_TIPO_VISITA == TipoVisitaLegal &&
                        w.ID_TIPO_DOCUMENTO == cedula).Any())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Aun no cuenta con la digitalizacion de la cedula.");
                        return;
                    }
                }
                if (!SelectPersona.ABOGADO.ABOGADO_DOCUMENTO.Any(w => w.ID_TIPO_DOCUMENTO == Parametro.TIPO_DOCTO_TITULO_LEGAL && w.ID_TIPO_VISITA == TipoVisitaLegal))
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes digitalizar el titulo del abogado.");
                    return;
                }
                /*var doc = 0;
                var legal = Parametro.ID_TIPO_VISITA_LEGAL;
                foreach (var item in Parametro.DOCUMENTOS_ABOGADOS)
                {
                    if (short.Parse(item.Split('-')[0]) == legal)
                    {
                        doc = short.Parse(item.Split('-')[1]);
                        if (!SelectPersona.ABOGADO.ABOGADO_DOCUMENTO.Where(w => w.ID_TIPO_DOCUMENTO == doc).Any())
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Aun no cuenta con los documentos necesarios.");
                            return;
                        }
                    }
                }*/
                #endregion

                #region GAFETE
                var gafetes = new List<GafeteAbogado>();
                GafeteView = new GafetesPVCView();
                var gaf = new GafeteAbogado();
                var centro = new cCentro().Obtener(4).FirstOrDefault();
                GafeteFrente = true;
                gaf.Discapacidad = SelectPersona.ID_TIPO_DISCAPACIDAD == null || SelectPersona.ID_TIPO_DISCAPACIDAD == 0 ? "NINGUNA" : SelectPersona.TIPO_DISCAPACIDAD.DESCR;
                gaf.TipoPersona = "ABOGADO";
                gaf.Imagen = fotos.FirstOrDefault().BIOMETRICO;
                var ceresos = Parametro.ID_DESCRIPCION_CERESOS;
                var VisitaLegal = Parametro.ID_TIPO_VISITA_LEGAL;
                //gaf.NipCereso1 = nip.Where(w => w.ID_TIPO_VISITA == VisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[0].Split('-')[0])).FirstOrDefault().NIP.Value.ToString();
                //gaf.DescrCereso1 = ceresos[0].Split('-')[1];
                //gaf.NipCereso2 = nip.Where(w => w.ID_TIPO_VISITA == VisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[1].Split('-')[0])).FirstOrDefault().NIP.Value.ToString();
                //gaf.DescrCereso2 = ceresos[1].Split('-')[1];
                //gaf.NipCereso3 = nip.Where(w => w.ID_TIPO_VISITA == VisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[2].Split('-')[0])).FirstOrDefault().NIP.Value.ToString();
                //gaf.DescrCereso3 = ceresos[2].Split('-')[1];
                //gaf.NipCereso4 = nip.Where(w => w.ID_TIPO_VISITA == VisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[3].Split('-')[0])).FirstOrDefault().NIP.Value.ToString();
                //gaf.DescrCereso4 = ceresos[3].Split('-')[1];
                //gaf.NipCereso5 = nip.Where(w => w.ID_TIPO_VISITA == VisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[4].Split('-')[0])).FirstOrDefault().NIP.Value.ToString();
                //gaf.DescrCereso5 = ceresos[4].Split('-')[1];
                //gaf.NipCereso6 = ceresos.Length == 6 ? nip.Where(w => w.ID_TIPO_VISITA == VisitaLegal &&
                //    w.ID_CENTRO == short.Parse(ceresos[5].Split('-')[0])).FirstOrDefault().NIP.Value.ToString() : "0";
                //gaf.DescrCereso6 = ceresos.Length == 6 ? ceresos[5].Split('-')[1] : "HONGO III";
                gaf.RFC = SelectPersona.RFC;
                gaf.Cedula = SelectPersona.ABOGADO.CEDULA;
                ///TODO: al depurar el catalogo de municipios quitar el 'replace'
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
                if (SelectPersona.ID_TIPO_DISCAPACIDAD == null || SelectPersona.ID_TIPO_DISCAPACIDAD == 0)
                    GafeteView.GafetesPVCReport.LocalReport.ReportPath = "Reportes/" + gafeteAbogado + ".rdlc";
                else
                    GafeteView.GafetesPVCReport.LocalReport.ReportPath = "Reportes/GafeteAbogadoFrenteDiscapacidad.rdlc";
                GafeteView.GafetesPVCReport.ShowExportButton = false;
                gafetes.Add(gaf);
                var rds = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds.Name = "DataSet1";
                rds.Value = gafetes;
                Reporteador = GafeteView.GafetesPVCReport;
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear gafete.", ex);
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
                    if (SelectPersona.ID_TIPO_DISCAPACIDAD == null || SelectPersona.ID_TIPO_DISCAPACIDAD == 0)
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
                if (((object[])(SelectedItem))[0] is CausaPenalAsignacion)
                {
                    var causaPenal = (CausaPenalAsignacion)(((object[])(SelectedItem))[0]);
                    var checkBox = (CheckBox)(((object[])(SelectedItem))[1]);
                    causaPenal.ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
                }
                else if (((object[])(SelectedItem))[0] is AbogadoCausaPenalAsignacion)
                {
                    var abogadoCausaPenal = (AbogadoCausaPenalAsignacion)(((object[])(SelectedItem))[0]);
                    var checkBox = (CheckBox)(((object[])(SelectedItem))[1]);
                    abogadoCausaPenal.ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
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
                        (new Dialogos()).ConfirmacionDialogo("Error", "Revise que el escáner esté bien configurado.");
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
                if (SelectedTipoDocumento == null)
                {
                    escaner.Hide();
                    await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija El Tipo De Documento A Digitalizar");
                    return;
                }
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
                if (TipoDoctoAbogado == (short)enumTipoDocumentoAbogado.PERSONA)
                {
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
                }
                else
                {
                    if (SelectIngreso == null)
                    {
                        escaner.Hide();
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalizacion", "Debes de elegir un imputado.");
                        return;
                    }
                    if (TipoDoctoAbogado == (short)enumTipoDocumentoAbogado.ABOGADO_CAUSA_PENAL)
                    {
                        if (SelectCausaAsignada == null)
                        {
                            escaner.Hide();
                            await new Dialogos().ConfirmacionDialogoReturn("Digitalizacion", "Debes de elegir un imputado.");
                            return;
                        }
                        #region ABOGADO_CAUSA_PENAL
                        var Documentos = new cAbogadoCausaPenalDocumento().ObtenerXAbogadoYCausaPenal(SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO,
                            SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO,
                            SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CAUSA_PENAL, SelectPersona.ID_PERSONA,
                            SelectPersona.ABOGADO.ID_ABOGADO_TITULO, SelectedTipoDocumento.ID_TIPO_DOCUMENTO).FirstOrDefault();
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
                    if (TipoDoctoAbogado == (short)enumTipoDocumentoAbogado.ABOGADO_INGRESO)
                    {
                        #region ABOGADO_INGRESO
                        var Documentos = new cAbogadoIngresoDocumento().ObtenerTodos(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO,
                            SelectIngreso.ID_INGRESO, SelectPersona.ID_PERSONA, SelectPersona.ABOGADO.ID_ABOGADO_TITULO, SelectedTipoDocumento.ID_TIPO_DOCUMENTO).FirstOrDefault();
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
            Application.Current.Dispatcher.Invoke((System.Action)(async delegate
            {
                try
                {
                    escaner.Hide();
                    if (SelectedTipoDocumento == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija El Tipo De Documento A Digitalizar");
                        return;
                    }
                    if (DocumentoDigitalizado == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice Un Documento Para Guardar");
                        return;
                    }
                    if (DocumentoDigitalizado.Length <= 0)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalizacion", "Digitalice Un Documento Para Guardar");
                        return;
                    }
                    if (SelectPersona == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalizacion", "Debe seleccionar una persona.");
                        return;
                    }
                    var hoy = Fechas.GetFechaDateServer;
                    if (TipoDoctoAbogado == (short)enumTipoDocumentoAbogado.PERSONA)
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
                            await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Debes de elegir un imputado.");
                            return;
                        }
                        if (TipoDoctoAbogado == (short)enumTipoDocumentoAbogado.ABOGADO_CAUSA_PENAL)
                        {
                            if (SelectCausaAsignada == null)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Debes de elegir una causa penal.");
                                return;
                            }
                            #region ABOGADO_CP_DOCTO
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                var Documentos = new cAbogadoCausaPenalDocumento().ObtenerXAbogadoYCausaPenal(SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO,
                                SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO,
                                SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CAUSA_PENAL, SelectPersona.ID_PERSONA,
                                SelectPersona.ABOGADO.ID_ABOGADO_TITULO, SelectedTipoDocumento.ID_TIPO_DOCUMENTO)
                                .Where(w => w.ID_TIPO_DOCUMENTO == SelectedTipoDocumento.ID_TIPO_DOCUMENTO && w.ID_TIPO_VISITA == SelectedTipoDocumento.ID_TIPO_VISITA).FirstOrDefault();

                                if (Documentos == null)
                                {
                                    if (new cAbogadoCausaPenalDocumento().Insertar(new ABOGADO_CP_DOCTO
                                       {
                                           ID_ABOGADO = SelectPersona.ID_PERSONA,
                                           ID_ABOGADO_TITULO = SelectPersona.ABOGADO.ID_ABOGADO_TITULO,
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
                                           ID_ABOGADO_TITULAR = 0
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
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                                else
                                {
                                    if (new cAbogadoCausaPenalDocumento().Actualizar(new ABOGADO_CP_DOCTO
                                    {
                                        ID_ABOGADO = SelectPersona.ID_PERSONA,
                                        ID_ABOGADO_TITULO = SelectPersona.ABOGADO.ID_ABOGADO_TITULO,
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
                                        ID_ABOGADO_TITULAR = 0
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
                        if (TipoDoctoAbogado == (short)enumTipoDocumentoAbogado.ABOGADO_INGRESO)
                        {
                            #region ABOGADO_ING_DOCTO
                            var Documentos = new cAbogadoIngresoDocumento().ObtenerTodos(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO,
                                SelectIngreso.ID_INGRESO, SelectPersona.ID_PERSONA, SelectPersona.ABOGADO.ID_ABOGADO_TITULO, SelectedTipoDocumento.ID_TIPO_DOCUMENTO).FirstOrDefault();
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                if (AbogadoAdministrativo)
                                {
                                    new cAbogadoIngreso().Actualizar(new ABOGADO_INGRESO
                                    {
                                        CAPTURA_FEC = SelectAbogadoIngreso.CAPTURA_FEC,
                                        ID_ABOGADO = SelectAbogadoIngreso.ID_ABOGADO,
                                        ID_ABOGADO_TITULO = SelectAbogadoIngreso.ID_ABOGADO_TITULO,
                                        ID_CENTRO = SelectAbogadoIngreso.ID_CENTRO,
                                        ID_ANIO = SelectAbogadoIngreso.ID_ANIO,
                                        ID_IMPUTADO = SelectAbogadoIngreso.ID_IMPUTADO,
                                        ID_INGRESO = SelectAbogadoIngreso.ID_INGRESO,
                                        OBSERV = SelectAbogadoIngreso.OBSERV,
                                        ID_ESTATUS_VISITA = SelectAbogadoIngreso.ID_ESTATUS_VISITA,
                                        ADMINISTRATIVO = AbogadoAdministrativo ? "S" : "N"
                                    });
                                    if (!new cAbogadoIngreso().CancelarAdministrativosXIngreso(SelectIngreso, SelectPersona.ABOGADO))
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalizacion", "Ocurrió un Error al Actualizar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                            if (AutoGuardado)
                                                escaner.Show();
                                            return;
                                        }));
                                    }
                                }
                                if (Documentos == null)
                                {
                                    if (new cAbogadoIngresoDocumento().Insertar(new ABOGADO_ING_DOCTO
                                    {
                                        ID_ABOGADO = SelectPersona.ID_PERSONA,
                                        ID_ABOGADO_TITULO = SelectPersona.ABOGADO.ID_ABOGADO_TITULO,
                                        DOCTO = DocumentoDigitalizado,
                                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                                        ID_ANIO = SelectIngreso.ID_ANIO,
                                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                                        ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                                        ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                                        OBSERV = ObservacionDocumento,
                                        CAPTURA_FEC = hoy,
                                        ID_ABOGADO_TITULAR = 0
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
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                                else
                                {
                                    if (new cAbogadoIngresoDocumento().Actualizar(new ABOGADO_ING_DOCTO
                                    {
                                        ID_ABOGADO = SelectPersona.ID_PERSONA,
                                        ID_ABOGADO_TITULO = SelectPersona.ABOGADO.ID_ABOGADO_TITULO,
                                        DOCTO = DocumentoDigitalizado,
                                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                                        ID_ANIO = SelectIngreso.ID_ANIO,
                                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                                        ID_TIPO_DOCUMENTO = SelectedTipoDocumento.ID_TIPO_DOCUMENTO,
                                        ID_TIPO_VISITA = SelectedTipoDocumento.ID_TIPO_VISITA,
                                        OBSERV = ObservacionDocumento,
                                        CAPTURA_FEC = Documentos.CAPTURA_FEC,
                                        ID_ABOGADO_TITULAR = 0
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
                                CargarListaTipoDocumentoIngresoDigitalizado();
                            });
                            #endregion
                        }
                    }


                    /********************************************/
                    var SelAboIng = SelectAbogadoIngreso;
                    var CauPen = SelectCausaAsignada;
                    SelectPersona = new cPersona().Obtener(SelectPersona.ID_PERSONA).FirstOrDefault();
                    if (TabAsignacion)
                    {
                        ListIngresosAsignados = new ObservableCollection<ABOGADO_INGRESO>(SelectPersona.ABOGADO.ABOGADO_INGRESO.Where(w => w.ID_ABOGADO_TITULO == Parametro.ID_ABOGADO_TITULAR_ABOGADO && w.ID_ESTATUS_VISITA != (short)enumEstatusVisita.CANCELADO_A));
                        if (ListIngresosAsignados.Any(f => f.ID_ABOGADO == SelAboIng.ID_ABOGADO && f.ID_ANIO == SelAboIng.ID_ANIO && f.ID_CENTRO == SelAboIng.ID_CENTRO && f.ID_IMPUTADO == SelAboIng.ID_IMPUTADO && f.ID_INGRESO == SelAboIng.ID_INGRESO))
                            SelectAbogadoIngreso = ListIngresosAsignados.First(f => f.ID_ABOGADO == SelAboIng.ID_ABOGADO && f.ID_ANIO == SelAboIng.ID_ANIO && f.ID_CENTRO == SelAboIng.ID_CENTRO && f.ID_IMPUTADO == SelAboIng.ID_IMPUTADO && f.ID_INGRESO == SelAboIng.ID_INGRESO);
                        else
                            SelectAbogadoIngreso = SelAboIng;
                        //ListCausasAsignadas = null;
                        if (ListCausasAsignadas.Any(f => f.ABOGADO_CAUSA_PENAL != null ?
                            f.ABOGADO_CAUSA_PENAL.ID_ABOGADO == SelAboIng.ID_ABOGADO && f.ABOGADO_CAUSA_PENAL.ID_ANIO == SelAboIng.ID_ANIO && f.ABOGADO_CAUSA_PENAL.ID_CENTRO == SelAboIng.ID_CENTRO &&
                            f.ABOGADO_CAUSA_PENAL.ID_IMPUTADO == SelAboIng.ID_IMPUTADO && f.ABOGADO_CAUSA_PENAL.ID_INGRESO == SelAboIng.ID_INGRESO
                        : false))
                            SelectCausaAsignada = ListCausasAsignadas.First(f => f.ABOGADO_CAUSA_PENAL != null ?
                                f.ABOGADO_CAUSA_PENAL.ID_ABOGADO == CauPen.ABOGADO_CAUSA_PENAL.ID_ABOGADO && f.ABOGADO_CAUSA_PENAL.ID_ANIO == CauPen.ABOGADO_CAUSA_PENAL.ID_ANIO && f.ABOGADO_CAUSA_PENAL.ID_CENTRO == CauPen.ABOGADO_CAUSA_PENAL.ID_CENTRO &&
                                f.ABOGADO_CAUSA_PENAL.ID_IMPUTADO == CauPen.ABOGADO_CAUSA_PENAL.ID_IMPUTADO && f.ABOGADO_CAUSA_PENAL.ID_INGRESO == CauPen.ABOGADO_CAUSA_PENAL.ID_INGRESO
                            : false);
                        else
                            SelectCausaAsignada = CauPen;
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
                foreach (var item in Parametro.DOCUMENTOS_ABOGADOS)
                {
                    doc = short.Parse(item.Split('-')[1]);
                    if (ListTipoDocumentoAux.Where(w => w.ID_TIPO_DOCUMENTO == doc).Any())
                    {
                        doctosAux.Add(ListTipoDocumentoAux.Where(w => w.ID_TIPO_DOCUMENTO == doc).FirstOrDefault());
                    }
                }
                var ListDocumentos = new cAbogadoDocumento().ObtenerTodos(4, VisitaLegal, SelectPersona.ID_PERSONA, 0).ToList();
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
        void CargarListaTipoDocumentoIngresoDigitalizado()
        {
            try
            {
                var ListTipoDocumentoAux = new ObservableCollection<TipoDocumento>(new cTipoDocumento().ObtenerAbogadoIngreso(Parametro.ID_TIPO_VISITA_LEGAL)
                    .Where(w => Parametro.DOCUMENTOS_ABOGADO_INGRESO.Where(wh => wh.Contains(w.ID_TIPO_DOCUMENTO.ToString())).Any())
                    .Select(s => new TipoDocumento
                    {
                        DESCR = s.DESCR,
                        DIGITALIZADO = false,
                        ID_TIPO_DOCUMENTO = s.ID_TIPO_DOCUMENTO,
                        ID_TIPO_VISITA = s.ID_TIPO_VISITA
                    }).OrderBy(o => o.DESCR).ToList());
                var ListDocumentos = new cAbogadoIngresoDocumento().ObtenerTodos(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO,
                    SelectIngreso.ID_INGRESO, SelectPersona.ID_PERSONA, SelectPersona.ABOGADO.ID_ABOGADO_TITULO, 0).ToList();
                foreach (var item in ListDocumentos)
                {
                    ListTipoDocumentoAux.Where(w => w.ID_TIPO_DOCUMENTO == item.ID_TIPO_DOCUMENTO).FirstOrDefault().DIGITALIZADO = true;
                }
                ListTipoDocumento = new ObservableCollection<TipoDocumento>(ListTipoDocumentoAux.OrderBy(o => o.ID_TIPO_DOCUMENTO));
                TipoDoctoAbogado = (short)enumTipoDocumentoAbogado.ABOGADO_INGRESO;
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
                    .Where(w => Parametro.DOCUMENTOS_ABOGADO_INGRESO.Where(wh => wh.Contains(w.ID_TIPO_DOCUMENTO.ToString())).Any())
                    .Select(s => new TipoDocumento
                    {
                        DESCR = s.DESCR,
                        DIGITALIZADO = false,
                        ID_TIPO_DOCUMENTO = s.ID_TIPO_DOCUMENTO,
                        ID_TIPO_VISITA = s.ID_TIPO_VISITA
                    }).OrderBy(o => o.DESCR).ToList());
                var ListDocumentos = new cAbogadoCausaPenalDocumento().ObtenerXAbogadoYCausaPenal(SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO,
                    SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO, SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO,
                    SelectCausaAsignada.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CAUSA_PENAL, SelectPersona.ID_PERSONA, SelectPersona.ABOGADO.ID_ABOGADO_TITULO, 0).ToList();
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