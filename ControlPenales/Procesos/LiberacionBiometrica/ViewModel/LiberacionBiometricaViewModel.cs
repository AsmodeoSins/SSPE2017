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
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class LiberacionBiometricaViewModel : ValidationViewModelBase
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        #region constructor
        public LiberacionBiometricaViewModel() 
        {
           
        }
        #endregion

        #region variables
        public string Name
        {
            get
            {
                return "liberacion_biometrica";
            }
        }
        #endregion

        #region metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "tomar_fotos":
                    if (SelectedLiberacion != null)
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar a un interno.");
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
                                ImageFrontal.Add(new ImageSourceToSave { FrameName = "ImgSenaParticular", ImageCaptured = new Imagenes().ConvertByteToBitmap(ImagenEgreso) });
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
                        if (ImagenEgreso != new Imagenes().getImagenPerson() && (ImageFrontal != null ? ImageFrontal.Count == 1 : false))
                        {
                            if (ImageFrontal.FirstOrDefault().ImageCaptured == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes de tomar una foto.");
                                return;
                            };

                            FotoTomada = true;
                            ImagenEgreso = new Imagenes().ConvertBitmapToByte(ImageFrontal.FirstOrDefault().ImageCaptured);
                            SelectedLiberacion.ImagenEgreso = ImagenEgreso;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                            await CamaraWeb.ReleaseVideoDevice();
                            break;
                        }
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes de tomar una foto.");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
                    }
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new LiberacionBiometricaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new LiberacionBiometricaViewModel();
                    break;
                case "validarBiometria":
                    if (SelectedLiberacion != null)
                    {
                        if (SelectedLiberacion.ImagenEgreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de tomar la fotografia.");
                        }
                        else
                        if (await new Dialogos().ConfirmarEliminar("Validación!", "¿Esta seguro que desea autorizar la liberacion?") == 1)
                        {
                            OnBuscarPorHuella();
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar a un interno.");
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private async void OnLoad(LiberacionBiometricaView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(ObtenerTodo);//(PopulateInternos)
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar liberación biometrica", ex);
            }
        }

        //fotos
        private async void TomarFotoLoad(TomarFotoSenaParticularView Window = null)
        {
            try
            {
                if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
                if (SelectedLiberacion.ImagenEgreso != null)
                {
                    ImagenEgreso = SelectedLiberacion.ImagenEgreso;
                    CamaraWeb.AgregarImagenControl(Window.ImgSenaParticular, new Imagenes().ConvertByteToImageSource(ImagenEgreso)); 
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó..", "Ocurrió un error al cargar la pantalla para tomar foto.", ex);
            }
        }

        private async void OnTakePicture(System.Windows.Controls.Image Picture)
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
                    ImageFrontal = new List<ImageSourceToSave>();
                    ImageFrontal.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (BitmapSource)Picture.Source });
                    StaticSourcesViewModel.Mensaje("Foto de Frente", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    if (await new Dialogos().ConfirmarEliminar("Advertencia", "Esta segúro que desea cambiar la foto " +
                    (Picture.Name.Contains("ImgFrente") ? "foto de frente" : Picture.Name.Contains("ImgReverso") ? "foto trasera" : "") + "?") == 1)
                    {
                        CamaraWeb.QuitarFoto(Picture);
                        //ImageFrontal.Remove(ImagesToSave.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                        ImageFrontal = new List<ImageSourceToSave>();
                    }                    
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

        private async void OnBuscarPorHuella(string obj = "")
        {
            try
            {
                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));

                await TaskEx.Delay(400);

                var nRet = -1;
                var bandera = true;
                var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaJuridico;
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
                    StaticSourcesViewModel.Mensaje("Advertencia", "Asegurese de conectar su lector de huella digital", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
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
                        if (Imputado == null)//NO ENCONTRO LAS HUELLAS
                        {
                            ValidacionBiometrica();
                        }
                        else//SI ENCONTRO HUELLAS SE VALIDAN
                        {
                            if (SelectedLiberacion.Liberacion != null)
                            {
                                if (SelectedLiberacion.Liberacion.ID_CENTRO == Imputado.ID_CENTRO && SelectedLiberacion.Liberacion.ID_ANIO == Imputado.ID_ANIO && SelectedLiberacion.Liberacion.ID_IMPUTADO == Imputado.ID_IMPUTADO)
                                {
                                    //Guardar("N");
                                    GuardarLibertadBiometrica("N");
                                    #region Comentado
                                    //if (GuardarLiberacion("N"))
                                    //{ 
                                    //     new Dialogos().ConfirmacionDialogo("ÉXITO", "Informaci\u00F3n registrada correctamente.");
                                    //     ObtenerTodo();
                                    //}
                                    //else
                                    //    new Dialogos().ConfirmacionDialogo("ERROR", "No se registr\u00F3 la informaci\u00F3n.");
                                    #endregion
                                }
                                else
                                {
                                    ValidacionBiometrica();
                                }
                            }
                            else
                                //new Dialogos().ConfirmacionDialogo("Validación", "Las huellas del interno no conciden con las huellas interno a liberar.");
                                ValidacionBiometrica();
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar imputado por huellas.", ex);
            }
        }
        #endregion

        #region LiberacionBiometrica
        private void ObtenerTodo() 
        {
            try
            {
                bool completo = false;
                ConfiguraPermisos();
                //var m = new ObservableCollection<LIBERACION>(new cLiberacion().ObtenerTodos("N", GlobalVar.gCentro)).Select(w => new cLiberacionBiometrica { Liberacion  = w, ImagenEgreso = null});
                LstLiberacion = new ObservableCollection<cLiberacionBiometrica>();//(m);
                #region Liberacion
                var ingresos = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro, null, null, true);
                if (ingresos != null)
                {
                    foreach (var i in ingresos)
                    {
                        completo = false;
                        var cps = i.CAUSA_PENAL;
                        if (cps != null)
                        {
                            completo = true;
                            foreach (var cp in cps)
                            {
                                if (cp.ID_ESTATUS_CP != (int)enumEstatusCausaPenal.CONCLUIDO)//validacion de activos
                                {
                                    completo = false;
                                    break;
                                }
                                if (cp.LIBERACION != null)
                                {
                                    if (cp.LIBERACION.Count == 0)
                                    {
                                        completo = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    completo = false;
                                    break;
                                }
                            }
                        }
                        if (completo)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                LstLiberacion.Add(new cLiberacionBiometrica()
                                {
                                    Ingreso = i,
                                    Imputado = i.IMPUTADO,
                                    ImagenEgreso = null,
                                });    
                            }));
                        }
                     }
                }
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar liberación biometrica", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.LIBERACION_BIOMETRICA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        //if (p.INSERTAR == 1)
                        //    PInsertar = true;
                        //if (p.EDITAR == 1)
                        //    PEditar = true;
                        //if (p.CONSULTAR == 1)
                        //    PConsultar = true;
                        //if (p.IMPRIMIR == 1)
                        //    PImprimir = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        private bool GuardarLiberacion(string Incidente) 
        {
            try
            {
                var obj = new LIBERACION();
                obj.ID_CENTRO = SelectedLiberacion.Liberacion.ID_CENTRO;
                obj.ID_ANIO = SelectedLiberacion.Liberacion.ID_ANIO;
                obj.ID_IMPUTADO = SelectedLiberacion.Liberacion.ID_IMPUTADO;
                obj.ID_INGRESO = SelectedLiberacion.Liberacion.ID_INGRESO;
                obj.ID_LIBERACION = SelectedLiberacion.Liberacion.ID_LIBERACION;
                obj.LIBERACION_FEC = SelectedLiberacion.Liberacion.LIBERACION_FEC;
                obj.LIBERACION_OFICIO = SelectedLiberacion.Liberacion.LIBERACION_OFICIO;
                obj.ID_LIBERACION_AUTORIDAD = SelectedLiberacion.Liberacion.ID_LIBERACION_AUTORIDAD;
                obj.ID_LIBERACION_MOTIVO = SelectedLiberacion.Liberacion.ID_LIBERACION_MOTIVO;
                obj.LIBERADO = "S";
                obj.INCIDENTE_BIOMETRICO = Incidente;
                #region Foto Salida
                 var ib = new INGRESO_BIOMETRICO();
                ib.ID_CENTRO = SelectedLiberacion.Liberacion.ID_CENTRO;
                ib.ID_ANIO = SelectedLiberacion.Liberacion.ID_ANIO;
                ib.ID_IMPUTADO = SelectedLiberacion.Liberacion.ID_IMPUTADO;
                ib.ID_INGRESO = SelectedLiberacion.Liberacion.ID_INGRESO;
                ib.ID_TIPO_BIOMETRICO = 1;
                ib.BIOMETRICO = SelectedLiberacion.ImagenEgreso;
                #endregion
                if (new cLiberacion().Actualizar(obj,ib,true))
                { 
                    //if(GuardarFotoSalida())
                    //    if (CambioEstatusAdministrativo())
                            return true; 
                    
                }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar liberación biometrica", ex);
            }
            return false;

        }

        private bool GuardarLibertadBiometrica(string Incidente)
        {
            try
            {
                if (SelectedLiberacion == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                    return false;
                }
                var ingreso = new INGRESO();
                ingreso.ID_CENTRO = SelectedLiberacion.Ingreso.ID_CENTRO;
                ingreso.ID_ANIO = SelectedLiberacion.Ingreso.ID_ANIO;
                ingreso.ID_IMPUTADO = SelectedLiberacion.Ingreso.ID_IMPUTADO;
                ingreso.ID_INGRESO = SelectedLiberacion.Ingreso.ID_INGRESO;
                ingreso.ID_ESTATUS_ADMINISTRATIVO = (short)enumEstatusAdministrativo.LIBERADO;
                //ubicacion
                ingreso.ID_UB_CENTRO = SelectedLiberacion.Ingreso.ID_UB_CENTRO;
                ingreso.ID_UB_EDIFICIO = SelectedLiberacion.Ingreso.ID_UB_EDIFICIO;
                ingreso.ID_UB_SECTOR = SelectedLiberacion.Ingreso.ID_UB_SECTOR;
                ingreso.ID_UB_CELDA = SelectedLiberacion.Ingreso.ID_UB_CELDA;
                ingreso.ID_UB_CAMA = SelectedLiberacion.Ingreso.ID_UB_CAMA;
                ingreso.FEC_SALIDA_CERESO = Fechas.GetFechaDateServer;

                var incidente = new INCIDENTE();
                if (Incidente == "S")
                {
                    incidente.ID_CENTRO = SelectedLiberacion.Ingreso.ID_CENTRO;
                    incidente.ID_ANIO = SelectedLiberacion.Ingreso.ID_ANIO;
                    incidente.ID_IMPUTADO = SelectedLiberacion.Ingreso.ID_IMPUTADO;
                    incidente.ID_INGRESO = SelectedLiberacion.Ingreso.ID_INGRESO;
                    incidente.ID_INCIDENTE_TIPO = 1;//cambiarlo
                    incidente.REGISTRO_FEC = ingreso.FEC_SALIDA_CERESO;
                    incidente.MOTIVO = string.Format("Libertad sin validación de huellas por el usuario {0}",GlobalVar.gUsr);
                    incidente.ESTATUS = "C";
                    incidente.AUTORIZACION_FEC = ingreso.FEC_SALIDA_CERESO;
                }

                var biometria = new INGRESO_BIOMETRICO();
                biometria.ID_CENTRO = SelectedLiberacion.Ingreso.ID_CENTRO;
                biometria.ID_ANIO = SelectedLiberacion.Ingreso.ID_ANIO;
                biometria.ID_IMPUTADO = SelectedLiberacion.Ingreso.ID_IMPUTADO;
                biometria.ID_INGRESO = SelectedLiberacion.Ingreso.ID_INGRESO;
                biometria.ID_TIPO_BIOMETRICO = 1;
                biometria.BIOMETRICO = SelectedLiberacion.ImagenEgreso;

                if (new cIngreso().LibertadBiometrica(ingreso,!string.IsNullOrEmpty(Incidente) ? incidente : null,biometria))
                {
                    
                    new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                    ObtenerTodo();
                    return true;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar liberación biometrica", ex);
            }
            return false;
        }

        private bool GuardarFotoSalida() 
        {
            var obj = new INGRESO_BIOMETRICO();
            obj.ID_CENTRO = SelectedLiberacion.Liberacion.ID_CENTRO;
            obj.ID_ANIO = SelectedLiberacion.Liberacion.ID_ANIO;
            obj.ID_IMPUTADO = SelectedLiberacion.Liberacion.ID_IMPUTADO;
            obj.ID_INGRESO = SelectedLiberacion.Liberacion.ID_INGRESO;
            obj.ID_TIPO_BIOMETRICO = 1;
            obj.BIOMETRICO = SelectedLiberacion.ImagenEgreso;
            if (SelectedLiberacion.Liberacion.INGRESO.INGRESO_BIOMETRICO.Where(w =>  w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTOGRAFIA_SALIDA && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Count() > 0)//UPDATE
            {
                if (new cIngresoBiometrico().Actualizar(obj))
                        return true;
            }
            else//INSERT
            {
                if (new cIngresoBiometrico().Insertar(obj))
                    return true;
            }
            return false;
        }

        private bool CambioEstatusAdministrativo() {
            var obj = new INGRESO();
            obj.ID_CENTRO = SelectedLiberacion.Liberacion.ID_CENTRO;
            obj.ID_ANIO = SelectedLiberacion.Liberacion.ID_ANIO;
            obj.ID_IMPUTADO = SelectedLiberacion.Liberacion.ID_IMPUTADO;
            obj.ID_INGRESO = SelectedLiberacion.Liberacion.ID_INGRESO;
            #region Info Ingreso
            obj.FEC_REGISTRO = SelectedLiberacion.Liberacion.INGRESO.FEC_REGISTRO;
            obj.FEC_INGRESO_CERESO = SelectedLiberacion.Liberacion.INGRESO.FEC_INGRESO_CERESO;
            obj.ID_TIPO_INGRESO = SelectedLiberacion.Liberacion.INGRESO.ID_TIPO_INGRESO;
            obj.ID_CLASIFICACION_JURIDICA = SelectedLiberacion.Liberacion.INGRESO.ID_CLASIFICACION_JURIDICA;
            obj.DOCINTERNACION_NUM_OFICIO = SelectedLiberacion.Liberacion.INGRESO.DOCINTERNACION_NUM_OFICIO;
            obj.ID_AUTORIDAD_INTERNA = SelectedLiberacion.Liberacion.INGRESO.ID_AUTORIDAD_INTERNA;
            obj.ID_TIPO_SEGURIDAD = SelectedLiberacion.Liberacion.INGRESO.ID_TIPO_SEGURIDAD;
            obj.ID_DISPOSICION = SelectedLiberacion.Liberacion.INGRESO.ID_DISPOSICION;
            obj.ID_TIPO_DOCUMENTO_INTERNACION = SelectedLiberacion.Liberacion.INGRESO.ID_TIPO_DOCUMENTO_INTERNACION;
            obj.DOCUMENTO_INTERNACION = SelectedLiberacion.Liberacion.INGRESO.DOCUMENTO_INTERNACION;
            obj.NUC = SelectedLiberacion.Liberacion.INGRESO.NUC;
            obj.AV_PREVIA = SelectedLiberacion.Liberacion.INGRESO.AV_PREVIA;
            obj.ID_IMPUTADO_EXPEDIENTE = SelectedLiberacion.Liberacion.INGRESO.ID_IMPUTADO_EXPEDIENTE;
            obj.ID_UB_CENTRO = SelectedLiberacion.Liberacion.INGRESO.ID_UB_CENTRO;
            obj.ID_UB_EDIFICIO = SelectedLiberacion.Liberacion.INGRESO.ID_UB_EDIFICIO;
            obj.ID_UB_SECTOR = SelectedLiberacion.Liberacion.INGRESO.ID_UB_SECTOR;
            obj.ID_UB_CELDA = SelectedLiberacion.Liberacion.INGRESO.ID_UB_CELDA;
            obj.ID_UB_CAMA = SelectedLiberacion.Liberacion.INGRESO.ID_UB_CAMA;
            obj.A_DISPOSICION = SelectedLiberacion.Liberacion.INGRESO.A_DISPOSICION;
            obj.ANIO_GOBIERNO = SelectedLiberacion.Liberacion.INGRESO.ANIO_GOBIERNO;
            obj.FOLIO_GOBIERNO = SelectedLiberacion.Liberacion.INGRESO.FOLIO_GOBIERNO;
            obj.ID_INGRESO_DELITO = SelectedLiberacion.Liberacion.INGRESO.ID_INGRESO_DELITO;
            #endregion
            obj.ID_ESTATUS_ADMINISTRATIVO = 4;//SALIDA
            if (new cIngreso().Actualizar(obj))
                return true;
            return false;
        }

        private async void ValidacionBiometrica() 
        {
            var respuesta = await new Dialogos().ConfirmarSalida("VALIDACIÓN", "Las huellas no coinciden.¿Desea capturarlas nuevamente?\nSi:Capturar las huellas nuevamente\nNo:Grabar informacion sin huellas\nCancelar:Cancelar la accion");
            switch (respuesta)
            { 
                case 0://NO
                    //Dialogos().ConfirmarEliminar("AVISO", "El interno no cuenta con biometria activada, se regisra libertad con incidencia.");
                    if (await new Dialogos().ConfirmarEliminar("AVISO", "El interno no cuenta con biometria activada, se registra libertad con incidencia.") == 1)
                    {
                        //Guardar("S");
                        GuardarLibertadBiometrica("S");    
                    }
                    
                    #region Comentado    
                    //if (GuardarLiberacion("S"))
                    //{
                    //    new Dialogos().ConfirmacionDialogo("ÉXITO", "Informaci\u00F3n registrada correctamente.");
                    //    ObtenerTodo();
                    //}
                    //else
                    //    new Dialogos().ConfirmacionDialogo("ERROR", "No se registr\u00F3 la informaci\u00F3n.");
#endregion
                    break;
                case 1://SI
                    OnBuscarPorHuella();
                    break;
            }
        }

        private bool Guardar(string Incidente)
        {
            try
            {
                if (SelectedLiberacion != null)
                {
                    if(new cIngreso().LiberarIngreso(SelectedLiberacion.Ingreso.ID_CENTRO,SelectedLiberacion.Ingreso.ID_ANIO,SelectedLiberacion.Ingreso.ID_IMPUTADO,SelectedLiberacion.Ingreso.ID_INGRESO,Incidente,SelectedLiberacion.ImagenEgreso))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                        ObtenerTodo();
                        //PopulateInternos();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un ingreso");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la liberación", ex);
            }
            return false;
        }

        private void PopulateInternos() 
        {
            try
            {
                ConfiguraPermisos();
                LstLiberacion = new ObservableCollection<cLiberacionBiometrica>(new ObservableCollection<INGRESO>(new cIngreso().GetIngresoParaLiberacion(GlobalVar.gCentro)).Select(w => new cLiberacionBiometrica { Ingreso = w, ImagenEgreso = null }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar liberación biometrica", ex);
            }
        }
        #endregion
    }
}
