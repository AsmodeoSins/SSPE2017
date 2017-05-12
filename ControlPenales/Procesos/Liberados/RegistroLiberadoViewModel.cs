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
using ControlPenales.BiometricoServiceReference;
using System.Windows.Controls;
using System.Windows.Interop;
using ControlPenales.Clases;
using System.Threading;
using Cogent.Biometrics;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;


namespace ControlPenales
{
    partial class RegistroLiberadoViewModel : FingerPrintScanner
    {
        #region constructor
        public RegistroLiberadoViewModel() 
        {
            
        }
        public RegistroLiberadoViewModel(bool? validar = false)
        {
            this.validar = validar.Value;
            EnabledCampo = true;
        }
        #endregion

        #region Metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar_menu":
                    LimpiarBusqueda();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONA);
                    break;
                case "buscar_empleado_pop":
                    BuscarEmpleado();
                    break;
                case "limpiar_empleado_pop":
                    LimpiarBusqueda();
                    break;
                case "seleccionar_empleado_pop":
                    if (SelectedEmpleadoPop != null)
                    {
                        if(StaticSourcesViewModel.SourceChanged == true)
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "Existen cambios sin guardar, esta seguro que desea agregar nuevo empleado?") != 1)
                                break;
                        SelectedLiberado = SelectedEmpleadoPop;
                        ObtenerLiberado();
                        LimpiarBusqueda();
                        EnabledCampo = true;
                        BotonEMIEnabled = true;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONA);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Vaidación","Favor de seleccionar a una persona");
                    break;
                case "salir_empleado_pop":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONA);
                    break;
                case "camara_visitante":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                     TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
                    break;
                case "aceptar_tomar_foto_senas":
                    try
                    {
                        if (ImageFrontal != null ? ImageFrontal.Count == 1 : false)
                        {
                            FotoTomada = true;
                            ImagenEmpleado = new Imagenes().ConvertBitmapToByte(ImageFrontal.FirstOrDefault().ImageCaptured);
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
                case "cancelar_tomar_foto_senas":
                     if (ImageFrontal != null ? ImageFrontal.Count == 1 : false)
                    {
                        if (!FotoTomada)
                            ImageFrontal = new List<ImageSourceToSave>();
                    }
                    else
                    {
                        ImageFrontal = new List<ImageSourceToSave>();
                        ImageFrontal.Add(new ImageSourceToSave { FrameName = "ImgSenaParticular", ImageCaptured = new Imagenes().ConvertByteToBitmap(ImagenEmpleado) });
                        BotonTomarFotoEnabled = true;
                    }
                    if (CamaraWeb != null)
                    {
                        await CamaraWeb.ReleaseVideoDevice();
                        CamaraWeb = null;
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    break;
                case "huellas_visitante":
                     PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                     this.ShowIdentification();
                    break;
                case "guardar_menu":
                    if (!base.HasErrors)
                        GuardarLiberado();
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Debe capturar los campos obligatorios.");
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RegistroLiberadoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new RegistroLiberadoViewModel();
                    break;
                case "insertar_menu":
                    if (StaticSourcesViewModel.SourceChanged)
                    { 
                     if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar, esta seguro que desea agregar nueva persona?") != 1)
                         break;
                    }
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RegistroLiberadoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new RegistroLiberadoViewModel(true);
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "enviar_emi":
                    //if (StaticSourcesViewModel.SourceChanged)
                    //{ 
                    // if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                    //        "Existen cambios sin guardar, esta seguro que desea agregar nueva persona?") != 1)
                    //     break;
                    //}
                    //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EMILiberadoView();
                    //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new EMILiberadoViewModel(SelectedLiberado);
                    break;
            }
        }

        private async void OnLoad(RegistroLiberadoView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }

        }

        private void CargarListas() 
        {
            try 
            {

                LstTipoVisitante = new ObservableCollection<TIPO_VISITANTE>(new cTipoVisitante().ObtenerTodos());
                LstPais = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos());
                LstTipoDiscapacidad = new ObservableCollection<TIPO_DISCAPACIDAD>(new cTipoDiscapacidad().ObtenerTodos());
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
               {
                   LstTipoVisitante.Insert(0, new TIPO_VISITANTE() { ID_TIPO_VISITANTE = -1, DESCR = "SELECCIONE" });
                   LstPais.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
                   LstTipoDiscapacidad.Insert(0, new TIPO_DISCAPACIDAD() { ID_TIPO_DISCAPACIDAD = -1, DESCR = "SELECCIONE" });
                   var centro = new cCentro().Obtener(GlobalVar.gCentro).SingleOrDefault();
                   if (centro != null)
                   {
                       
                       EPais = centro.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.ID_PAIS_NAC;
                       EEstado = centro.ID_ENTIDAD;
                       EMunicipio = centro.ID_MUNICIPIO;
                   }
                   StaticSourcesViewModel.SourceChanged = false;
               }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listados.", ex);
            }
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    var textbox = (TextBox)obj;
                    switch (textbox.Name)
                    {
                        case "NIP":
                            if (string.IsNullOrEmpty(textbox.Text))
                                Nip = null;
                            else
                                Nip = int.Parse(textbox.Text);
                            break;
                        case "PaternoEmpleado":
                            PaternoE = textbox.Text;
                            break;
                        case "MaternoEmpleado":
                            MaternoE = textbox.Text;
                            break;
                        case "NombreEmpleado":
                            NombreE = textbox.Text;
                            break;
                    }
                    BuscarEmpleado();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar busqueda", ex);
            }
        }
        #endregion

        #region Liberados
        private void ObtenerLiberado() {
            if (SelectedLiberado != null)
            {
                ECodigo = SelectedLiberado.ID_PERSONA;
                EPaterno = !string.IsNullOrEmpty(SelectedLiberado.PATERNO) ? SelectedLiberado.PATERNO.Trim() : string.Empty;
                EMaterno = !string.IsNullOrEmpty(SelectedLiberado.MATERNO) ? SelectedLiberado.MATERNO.Trim() : string.Empty;
                ENombre = !string.IsNullOrEmpty(SelectedLiberado.NOMBRE) ? SelectedLiberado.NOMBRE.Trim() : string.Empty;
                ESexo = SelectedLiberado.SEXO;
                EFechaNacimiento = SelectedLiberado.FEC_NACIMIENTO;
                //ESituacion = SelectedPersona.es
                ECURP = !string.IsNullOrEmpty(SelectedLiberado.CURP) ? SelectedLiberado.CURP.Trim() : string.Empty;
                ERFC = !string.IsNullOrEmpty(SelectedLiberado.RFC) ? SelectedLiberado.RFC.Trim() : string.Empty;
                ETelefonoFijo = !string.IsNullOrEmpty(SelectedLiberado.TELEFONO) ? SelectedLiberado.TELEFONO.Trim() : string.Empty;
                ETelefonoMovil = !string.IsNullOrEmpty(SelectedLiberado.TELEFONO_MOVIL) ? SelectedLiberado.TELEFONO_MOVIL.Trim() : string.Empty;
                ECorreo = !string.IsNullOrEmpty(SelectedLiberado.CORREO_ELECTRONICO) ? SelectedLiberado.CORREO_ELECTRONICO.Trim() : string.Empty;
                //ENip = 
                //EFechaAlta = SelectedLiberado.REGISTRO_FEC;
                EPais = SelectedLiberado.ID_PAIS != null ? SelectedLiberado.ID_PAIS : -1;
                EEstado = SelectedLiberado.ID_ENTIDAD != null ? SelectedLiberado.ID_ENTIDAD : -1;
                EMunicipio = SelectedLiberado.ID_MUNICIPIO != null ? SelectedLiberado.ID_MUNICIPIO : -1;
                EColonia = SelectedLiberado.ID_COLONIA != null ? SelectedLiberado.ID_COLONIA : -1;
                ECalle = !string.IsNullOrEmpty(SelectedLiberado.DOMICILIO_CALLE) ? SelectedLiberado.DOMICILIO_CALLE.Trim() : string.Empty;
                ENoExterior = SelectedLiberado.DOMICILIO_NUM_EXT;
                ENoInterior = !string.IsNullOrEmpty(SelectedLiberado.DOMICILIO_NUM_INT) ? SelectedLiberado.DOMICILIO_NUM_INT : string.Empty;
                ECP = SelectedLiberado.DOMICILIO_CODIGO_POSTAL;
                if (SelectedLiberado.ID_TIPO_DISCAPACIDAD != null)
                {
                    if (SelectedLiberado.ID_TIPO_DISCAPACIDAD > 0)
                    {
                        EDiscapacidad = "S";
                        ETipoDiscapacidad = SelectedLiberado.ID_TIPO_DISCAPACIDAD;
                    }
                    else
                    {
                        EDiscapacidad = "N";
                        ETipoDiscapacidad = -1;
                    }
                }
                else
                {
                    EDiscapacidad = "N";
                    ETipoDiscapacidad = -1;
                }


                //EObservacion = SelectedLiberado.OBSERV;
                //EFechaAlta = SelectedEmpleado.REGISTRO_FEC;
                //if (SelectedEmpleado.PERSONA.EMPLEADO != null)
                //{
                //    ETipoEmpleado = SelectedEmpleado.PERSONA.EMPLEADO.ID_TIPO_EMPLEADO;
                //    EAreaTrabajo = SelectedEmpleado.PERSONA.EMPLEADO.ID_AREA_EMP;
                //}

                //Imagenes
                if (SelectedLiberado.PERSONA_BIOMETRICO != null)
                {
                    var bio = SelectedLiberado.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                    if (bio != null)
                    {
                        ImagenEmpleado = bio.BIOMETRICO;
                    }
                }

                StaticSourcesViewModel.SourceChanged = false;
            }
        }

        private async void GuardarLiberado()
        {
            try
            {
                var respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando liberado", () =>
                {
                    try
                    {
                        var obj = new SSP.Servidor.PERSONA();
                        //obj.ID_TIPO_PERSONA = (short)enumTipoPersona.LIBERADO;
                        obj.PATERNO = EPaterno;
                        obj.MATERNO = EMaterno;
                        obj.NOMBRE = ENombre;
                        obj.SEXO = ESexo;
                        obj.CURP = ECURP;
                        obj.RFC = ERFC;
                        obj.FEC_NACIMIENTO = EFechaNacimiento;

                        obj.DOMICILIO_CALLE = ECalle;
                        obj.DOMICILIO_NUM_EXT = ENoExterior;
                        obj.DOMICILIO_NUM_INT = ENoInterior;
                        obj.ID_COLONIA = EColonia != -1 ? EColonia : null;
                        obj.ID_MUNICIPIO = EMunicipio != -1 ? EMunicipio : null;
                        obj.ID_ENTIDAD = EEstado != -1 ? EEstado : null;
                        obj.ID_PAIS = EPais != -1 ? EPais : null;
                        obj.DOMICILIO_CODIGO_POSTAL = ECP;

                        obj.TELEFONO = ETelefonoFijo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                        obj.TELEFONO_MOVIL = ETelefonoMovil.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                        obj.CORREO_ELECTRONICO = ECorreo;
                        obj.ID_TIPO_DISCAPACIDAD = ETipoDiscapacidad != -1 ? ETipoDiscapacidad : null;

                        if (SelectedLiberado == null)
                        {
                            obj.ID_PERSONA = 0;
                            //Biometrico Foto
                            if (ImagenEmpleado.Length != 1882)
                            {
                                var bio = new SSP.Servidor.PERSONA_BIOMETRICO();
                                bio.BIOMETRICO = ImagenEmpleado;
                                bio.ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO;
                                bio.ID_FORMATO = (short)enumTipoFormato.FMTO_JPG;
                                bio.ID_PERSONA = 0;
                                obj.PERSONA_BIOMETRICO.Add(bio);
                            }
                            //Biometrico Foto
                            if (HuellasCapturadas != null)
                            {
                                foreach (var h in HuellasCapturadas)
                                {
                                    obj.PERSONA_BIOMETRICO.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                    {
                                        ID_PERSONA = 0,
                                        BIOMETRICO = h.BIOMETRICO,
                                        ID_TIPO_BIOMETRICO = (short)h.ID_TIPO_BIOMETRICO,
                                        ID_FORMATO = (short)h.ID_TIPO_FORMATO
                                    });
                                }
                            }

                            obj.ID_PERSONA = new cPersona().InsertarP(obj,Fechas.GetFechaDateServer.Year);
                            if (obj.ID_PERSONA > 0)
                            {
                                SelectedLiberado = new cPersona().Obtener(obj.ID_PERSONA).SingleOrDefault();
                                ECodigo = SelectedLiberado.ID_PERSONA;
                                //EFechaAlta = SelectedEmpleado.REGISTRO_FEC.HasValue ? SelectedEmpleado.REGISTRO_FEC : null;
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                        {
                            obj.ID_PERSONA = SelectedLiberado.ID_PERSONA;
                            //Fotos
                            var foto = new SSP.Servidor.PERSONA_BIOMETRICO();
                            if (ImagenEmpleado.Length != 1882)
                            {
                                foto.BIOMETRICO = ImagenEmpleado;
                                foto.ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO;
                                foto.ID_FORMATO = (short)enumTipoFormato.FMTO_JPG;
                                foto.ID_PERSONA = SelectedLiberado.ID_PERSONA;
                            }
                            else
                                foto = null;

                            //Huellas
                            var huellas = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                            if (HuellasCapturadas != null)
                            {
                                foreach (var h in HuellasCapturadas)
                                {
                                    huellas.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                    {
                                        ID_PERSONA = obj.ID_PERSONA,
                                        BIOMETRICO = h.BIOMETRICO,
                                        ID_TIPO_BIOMETRICO = (short)h.ID_TIPO_BIOMETRICO,
                                        ID_FORMATO = (short)h.ID_TIPO_FORMATO,
                                    });
                                }
                            }
                            else
                                huellas = null;
                            if (new cPersona().Actualizar(obj, foto, huellas))
                            {
                                return true;
                                //if (ActualizarEmpleado())
                                //{
                                //    if (ActualizarImagen())
                                //    {
                                //        if (ActualizarHuellas())
                                //            return true;
                                //        else
                                //            return false;
                                //    }
                                //    else
                                //        return false;
                                //}
                                //else
                                //    return false;
                            }
                            else
                                return false;
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }
                    return false;
                });

                if (respuesta)
                {
                    StaticSourcesViewModel.SourceChanged = false;
                    BotonEMIEnabled = true;
                    new Dialogos().ConfirmacionDialogo("Éxito", "Información grabada exitosamente!");
                }
                else
                    new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la información");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo paso...", "Ocurrio un error...", ex);
            }
        }

        //private bool ActualizarEmpleado() 
        //{
        //    try
        //    {
        //        var empleado = new EMPLEADO();
        //        empleado.ID_EMPLEADO = SelectedEmpleado.ID_EMPLEADO;
        //        empleado.ID_TIPO_EMPLEADO = ETipoEmpleado;
        //        empleado.ID_AREA_EMP = EAreaTrabajo;
        //        empleado.ID_CENTRO = GlobalVar.gCentro;
        //        empleado.OBSERV = EObservacion;
        //        empleado.ESTATUS = SelectedEmpleado.ESTATUS;
        //        if (new cEmpleado().Actualizar(empleado))
        //            return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al actualizar empleado.", ex);
        //    }
        //    return false;
        //}

        private bool ActualizarImagen()
        {
            try
            {
                if (ImagenEmpleado.Length == 1882)
                    return true;
                //actualizamos la foto
                if (SelectedLiberado.PERSONA_BIOMETRICO != null)
                {
                    var bio = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                    bio.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                    {
                        BIOMETRICO = ImagenEmpleado,
                        ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                        ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                        ID_PERSONA = SelectedLiberado.ID_PERSONA
                    });

                    if (SelectedLiberado.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Count() > 0)
                    {
                        if (new cPersonaBiometrico().Actualizar(bio))
                            return true;
                    }
                    else
                    {
                        if (new cPersonaBiometrico().Insertar(bio))
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al actualizar foto.", ex);
            }
            return false;
        }

        private bool ActualizarHuellas()
        {
            try 
            {
                if (HuellasCapturadas == null)
                    return true;

                if (SelectedLiberado.PERSONA_BIOMETRICO != null)
                {
                    if (HuellasCapturadas != null)
                    {
                        var bio = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                        foreach (var h in HuellasCapturadas)
                        {
                            bio.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                            {
                                BIOMETRICO = h.BIOMETRICO,
                                ID_TIPO_BIOMETRICO = (short)h.ID_TIPO_BIOMETRICO,
                                ID_FORMATO = (short)h.ID_TIPO_FORMATO,
                                ID_PERSONA = SelectedLiberado.ID_PERSONA
                            });
                        }

                        if (SelectedLiberado.PERSONA_BIOMETRICO.Where(w =>
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.MENIQUE_DERECHO ||
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.ANULAR_DERECHO ||
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.MEDIO_DERECHO ||
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.INDICE_DERECHO ||
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.PULGAR_DERECHO ||
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.MENIQUE_IZQUIERDO ||
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.ANULAR_IZQUIERDO ||
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.MEDIO_IZQUIERDO ||
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.INDICE_IZQUIERDO ||
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.PULGAR_IZQUIERDO).Count() > 0)
                        {
                            if (new cPersonaBiometrico().Actualizar(bio))
                                return true;
                        }
                        else
                        {
                            if (new cPersonaBiometrico().Insertar(bio))
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al actualizar huellas.", ex);
            }
            return false;
        }
        #endregion

        #region Buscar
        private void LimpiarBusqueda() {
            try 
            {
                PaternoE = MaternoE = NombreE = string.Empty;
                Nip = null;
                ImagenEmpleadoPop = new Imagenes().getImagenPerson();
                LstEmpleadoPop = new ObservableCollection<SSP.Servidor.PERSONA>();
                EmpleadoEmpty = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar busqueda.", ex);
            }
        }

        private void BuscarEmpleado() 
        {
            try 
            {
                LstEmpleadoPop = new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodos(NombreE, PaternoE, MaternoE, Nip, 1));
                EmpleadoEmpty = LstEmpleadoPop.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar empleado.", ex);
            }
        }
        #endregion

        #region Tomar Foto
        private async void TomarFotoLoad(TomarFotoSenaParticularView Window = null)
        {
            try
            {
                if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                CamaraWeb = new ControlPenales.Clases.WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
                if (ImagenEmpleado.Length != 1882)
                    CamaraWeb.AgregarImagenControl(Window.ImgSenaParticular, new Imagenes().ConvertByteToImageSource(ImagenEmpleado));
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
                    ImagesToSave.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (System.Windows.Media.Imaging.BitmapSource)Picture.Source });
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
                    ImageFrontal.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (System.Windows.Media.Imaging.BitmapSource)Picture.Source });
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
                                //for (short i = 1; i <= 10; i++)
                                //{
                                //    var pBuffer = IntPtr.Zero;
                                //    var nBufferLength = 0;
                                //    var nNFIQ = 0;
                                //    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                                //    var bufferBMP = new byte[nBufferLength];
                                //    if (pBuffer != IntPtr.Zero)
                                //        Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);
                                //    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                                //    var bufferWSQ = new byte[nBufferLength];
                                //    if (pBuffer != IntPtr.Zero)
                                //        Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);
                                //    CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);
                                //    DPUruNet.Fmd FMD = null;
                                //    if (bufferBMP.Length != 0)
                                //    {
                                //        GuardaHuella = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                //        FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                                //    }
                                //    Thread.Sleep(200);
                                //    switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                                //    {
                                //        #region [Pulgar Derecho]
                                //        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                //            break;
                                //        #endregion
                                //        #region [Indice Derecho]
                                //        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                //            break;
                                //        #endregion
                                //        #region [Medio Derecho]
                                //        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                //            break;
                                //        #endregion
                                //        #region [Anular Derecho]
                                //        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                //            break;
                                //        #endregion
                                //        #region [Meñique Derecho]
                                //        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                //            break;
                                //        #endregion
                                //        #region [Pulgar Izquierdo]
                                //        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                //            break;
                                //        #endregion
                                //        #region [Indice Izquierdo]
                                //        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                //            break;
                                //        #endregion
                                //        #region [Medio Izquierdo]
                                //        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                //            break;
                                //        #endregion
                                //        #region [Anular Izquierdo]
                                //        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                //            break;
                                //        #endregion
                                //        #region [Meñique Izquierdo]
                                //        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                //            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                //            break;
                                //        #endregion
                                //        default:
                                //            break;
                                //    }
                                //}
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
              
                if (nRet != 0)
                    if (((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0)
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
                    var resultado = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (resultado != null)
                    {
                        if (resultado.SelectRegistro != null)
                        {
                            SelectedLiberado = resultado.SelectRegistro.Persona;
                            ObtenerLiberado();
                        }
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                };
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }
        #endregion
    }
}
