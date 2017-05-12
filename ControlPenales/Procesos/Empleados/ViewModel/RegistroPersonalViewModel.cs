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
    partial class RegistroPersonalViewModel : FingerPrintScanner
    {
        #region constructor
        public RegistroPersonalViewModel()
        {

        }
        public RegistroPersonalViewModel(bool? validar = false)
        {
            //this.validar = validar.Value;
            //ValidarEmpleado();
            Actualiza = false;
            NuevoEmpleado = true;
            EnabledCampo = true;
        }
        #endregion

        #region Metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar_menu":
                case "menu_buscar":
                    LimpiarBusqueda();
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_EMPLEADO);
                    break;
                case "buscar_empleado_pop":
                    BuscarEmpleado();
                    break;
                case "limpiar_empleado_pop":
                    LimpiarBusqueda();
                    break;
                case "seleccionar_empleado_pop":
                    if (SelectedEmpleadoPop == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Vaidación", "Favor de seleccionar a una persona");
                        return;
                    }
                    if (StaticSourcesViewModel.SourceChanged ? await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1 : false)
                        return;
                    if (SelectedEmpleadoPop.EMPLEADO != null)
                    {
                        SelectPersona = SelectedEmpleadoPop;
                        SelectedEmpleado = SelectedEmpleadoPop.EMPLEADO;
                        Actualiza = true;
                        NuevoEmpleado = false;
                        EnabledCampo = true;
                        ObtenerEmpleado();
                    }
                    else
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "LA PERSONA SELECCIONADA NO ESTA REGISTRADA COMO EMPLEADO, DESEA REGISTRARLA AHORA?") == 1)
                        {
                            SelectPersona = SelectedEmpleadoPop;
                            SelectedEmpleado = SelectedEmpleadoPop.EMPLEADO;
                            Actualiza = true;
                            NuevoEmpleado = true;
                            EnabledCampo = false;
                            ObtenerEmpleado();
                        }
                    }
                    if (PEditar)
                    {
                        EliminarMenuEnabled = true;
                        EditarMenuEnabled = true;
                    }
                    CancelarMenuEnabled = true;
                    GuardarMenuEnabled = false;
                    BuscarMenuEnabled = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EMPLEADO);
                    break;
                case "menu_editar":
                    if (SelectedEmpleado == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Vaidación", "Favor de seleccionar a una persona");
                        return;
                    }
                    EnabledCampo = true;
                    GuardarMenuEnabled = true;
                    break;
                case "salir_empleado_pop":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EMPLEADO);
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
                            if (ImageFrontal.FirstOrDefault().ImageCaptured == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "DEBES DE TOMAR UNA FOTO.");
                                return;
                            }

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
                case "guardar_menu"://"menu_guardar":
                    ValidarEmpleado();
                        GuardarEmpleado();
                    break;
                case "limpiar_menu"://"menu_limpiar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                               "Existen cambios sin guardar, esta seguro que desea limpiar la pantalla?") != 1)
                            break;
                    }
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RegistroPersonalView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new RegistroPersonalViewModel();
                    break;
                case "insertar_menu"://"menu_agregar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                               "Existen cambios sin guardar, esta seguro que desea agregar nuevo empleado?") != 1)
                            break;
                    }
                    await Limpiar();
                    EFechaNacimientoValid = false;
                    GuardarMenuEnabled = true;
                    BuscarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    SelectedEmpleado = null;
                    SelectPersona = null;
                    Actualiza = false;
                    GuardarMenuEnabled=true;
                    EnabledCampo = true;
                    ValidarEmpleado();
                    //Obliga a buscar por huella
                    await OnBuscarPorHuellaInicio(string.Empty);
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "menu_salir":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "menu_cancelar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                               "Existen cambios sin guardar, esta seguro que desea cancelar la edicion del empleado?") != 1)
                            break;
                    }
                    await Limpiar();
                    base.ClearRules();
                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    BuscarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    StaticSourcesViewModel.SourceChanged = false;
                    EnabledCampo = false;
                    break;
                case "menu_eliminar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                               "Existen cambios sin guardar, esta seguro que desea eliminar el empleado?") != 1)
                            break;
                    }
                    if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "DESEA DESHABILITAR AL EMPLEADO?") == 0)
                        return;
                    await Eliminar();
                    await Limpiar();
                    base.ClearRules();
                    EnabledCampo = false;
                    selectedEmpleado = null;
                    selectPersona = null;
                    SelectedEmpleadoPop = null;
                    break;
            }
        }

        private async Task Eliminar()
        {
            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cambiando de estatus",()=>{
                new cEmpleado().DeshabilitarEmpleado(selectedEmpleado.ID_EMPLEADO);
                return true;
            }))
            {
                new Dialogos().ConfirmacionDialogo("EXITO!", "El empleado ha sido inhabilitado con exito");
            }
        }

        private async Task Limpiar()
        {
            ECodigo = null;
            EPaterno = string.Empty;
            EMaterno = string.Empty;
            ENombre = string.Empty;
            EFechaNacimiento = null;
            ECURP = string.Empty;
            ERFC = string.Empty;
            ETelefonoFijo = string.Empty;
            ETelefonoMovil = string.Empty;
            EFechaAlta = null;
            ECalle = string.Empty;
            ENoInterior = string.Empty;
            ENoExterior = null;
            ECodigo = null;
            EObservacion = string.Empty;
            ECedulaProfesional = string.Empty;
            ECP = null;
            ECorreo = string.Empty;
            ENip = string.Empty;
            ImagenEmpleado = new Imagenes().getImagenPerson();
            EFechaNacimientoValid = true;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);

            EColonia = -1;
            EDiscapacidad = string.Empty;
            ETipoEmpleado = -1;
            EAreaTrabajo = -1;
            ESexo = string.Empty;
           
        }


        private async void OnLoad(RegistroPersonalView obj = null)
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

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REGISTRO_PERSONAL.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

                        if (!PInsertar)
                            AgregarMenuEnabled = false;
                        else
                            AgregarMenuEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        private void CargarListas()
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
               {
                   if (LstTipoVisitante == null)
                   {
                       LstTipoVisitante = new ObservableCollection<TIPO_VISITANTE>(new cTipoVisitante().ObtenerTodos());
                       LstTipoVisitante.Insert(0, new TIPO_VISITANTE() { ID_TIPO_VISITANTE = -1, DESCR = "SELECCIONE" });
                   }
                   if (LstEstatusVisita == null)
                   {
                       LstEstatusVisita = new ObservableCollection<ESTATUS_VISITA>(new cEstatusVisita().ObtenerTodos());
                       LstEstatusVisita.Insert(0, new ESTATUS_VISITA() { ID_ESTATUS_VISITA = -1, DESCR = "SELECCIONE" });
                   }
                   if (LstPais == null)
                   {
                       LstPais = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos());
                       LstPais.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
                   }
                   if (LstTipoDiscapacidad == null)
                   {
                       LstTipoDiscapacidad = new ObservableCollection<TIPO_DISCAPACIDAD>(new cTipoDiscapacidad().ObtenerTodos());
                       LstTipoDiscapacidad.Insert(0, new TIPO_DISCAPACIDAD() { ID_TIPO_DISCAPACIDAD = -1, DESCR = "SELECCIONE" });
                   }
                   if (LstTipoEmpleado == null)
                   {
                       LstTipoEmpleado = new ObservableCollection<TIPO_EMPLEADO>(new cTipoEmpleado().ObtenerTodos());
                       LstTipoEmpleado.Insert(0, new TIPO_EMPLEADO() { ID_TIPO_EMPLEADO = -1, DESCR = "SELECCIONE" });
                   }
                   if (LstAreaTrabajo == null)
                   {
                       LstAreaTrabajo = new ObservableCollection<DEPARTAMENTO>(new cDepartamentos().ObtenerTodos());
                       LstAreaTrabajo.Insert(0, new DEPARTAMENTO() { ID_DEPARTAMENTO = -1, DESCR = "SELECCIONE" });
                   }

                   if (lstCentro==null)
                   {
                       LstCentro = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos());
                       LstCentro.Insert(0, new CENTRO() {ID_CENTRO=-1,DESCR="SELECCIONE" });
                   }

                   ConfiguraPermisos();
                   //ValidarEmpleado();
                   var centro = new cCentro().Obtener(GlobalVar.gCentro).SingleOrDefault();
                   if (centro != null)
                   {
                       SelectedCentroValue = centro.ID_CENTRO;
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
                        case "NIPEmpleado":
                            if (string.IsNullOrEmpty(textbox.Text))
                                NipE = null;
                            else
                                NipE = int.Parse(textbox.Text);
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

        #region Empleados
        private void ObtenerEmpleado()
        {
            ECodigo = SelectPersona.ID_PERSONA;
            EPaterno = !string.IsNullOrEmpty(SelectPersona.PATERNO) ? SelectPersona.PATERNO.Trim() : string.Empty;
            EMaterno = !string.IsNullOrEmpty(SelectPersona.MATERNO) ? SelectPersona.MATERNO.Trim() : string.Empty;
            ENombre = !string.IsNullOrEmpty(SelectPersona.NOMBRE) ? SelectPersona.NOMBRE.Trim() : string.Empty;
            ETipoVisitante = SelectPersona.ID_TIPO_PERSONA;
            ESexo = SelectPersona.SEXO;
            EFechaNacimiento = SelectPersona.FEC_NACIMIENTO;
            //ESituacion = SelectedPersona.es
            ECURP = !string.IsNullOrEmpty(SelectPersona.CURP) ? SelectPersona.CURP.Trim() : string.Empty;
            ERFC = !string.IsNullOrEmpty(SelectPersona.RFC) ? SelectPersona.RFC.Trim() : string.Empty;
            ETelefonoFijo = !string.IsNullOrEmpty(SelectPersona.TELEFONO) ? SelectPersona.TELEFONO.Trim() : string.Empty;
            ETelefonoMovil = !string.IsNullOrEmpty(SelectPersona.TELEFONO_MOVIL) ? SelectPersona.TELEFONO_MOVIL.Trim() : string.Empty;
            ECorreo = !string.IsNullOrEmpty(SelectPersona.CORREO_ELECTRONICO) ? SelectPersona.CORREO_ELECTRONICO.Trim() : string.Empty;
            ENip = SelectPersona.PERSONA_NIP.Any(a => a.ID_CENTRO == GlobalVar.gCentro && a.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_EMPLEADO) ?
                SelectPersona.PERSONA_NIP.First(a => a.ID_CENTRO == GlobalVar.gCentro && a.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_EMPLEADO).NIP.HasValue ?
                    SelectPersona.PERSONA_NIP.First(a => a.ID_CENTRO == GlobalVar.gCentro && a.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_EMPLEADO).NIP.Value.ToString() :
                        string.Empty : string.Empty;
            //EFechaAlta = SelectPersona.EMPLEADO.REGISTRO_FEC.HasValue ? SelectPersona.EMPLEADO.REGISTRO_FEC.Value : Fechas.GetFechaDateServer;
            EPais = SelectPersona.ID_PAIS != null ? SelectPersona.ID_PAIS : -1;
            EEstado = SelectPersona.ID_ENTIDAD != null ? SelectPersona.ID_ENTIDAD : -1;
            EMunicipio = SelectPersona.ID_MUNICIPIO != null ? SelectPersona.ID_MUNICIPIO : -1;
            EColonia = SelectPersona.ID_COLONIA != null ? SelectPersona.ID_COLONIA : -1;
            ECalle = !string.IsNullOrEmpty(SelectPersona.DOMICILIO_CALLE) ? SelectPersona.DOMICILIO_CALLE.Trim() : string.Empty;
            ENoExterior = SelectPersona.DOMICILIO_NUM_EXT;
            ENoInterior = !string.IsNullOrEmpty(SelectPersona.DOMICILIO_NUM_INT) ? SelectPersona.DOMICILIO_NUM_INT : string.Empty;
            ECP = SelectPersona.DOMICILIO_CODIGO_POSTAL;
            if (SelectPersona.ID_TIPO_DISCAPACIDAD != null ? SelectPersona.ID_TIPO_DISCAPACIDAD > 0 : false)
            {
                EDiscapacidad = "S";
                ETipoDiscapacidad = SelectPersona.ID_TIPO_DISCAPACIDAD;
            }
            else
            {
                EDiscapacidad = "N";
                ETipoDiscapacidad = -1;
            }


            if (SelectPersona.EMPLEADO != null)
            {
                EObservacion = SelectPersona.EMPLEADO.OBSERV;
                EFechaAlta = SelectPersona.EMPLEADO.REGISTRO_FEC;
                ETipoEmpleado = SelectPersona.EMPLEADO.ID_TIPO_EMPLEADO;
                EAreaTrabajo = SelectPersona.EMPLEADO.ID_DEPARTAMENTO;
                ECedulaProfesional = SelectPersona.EMPLEADO.CEDULA;
                SelectedCentroValue = SelectedEmpleado.ID_CENTRO.Value;
          
            }
            else
            {
                EFechaAlta = Fechas.GetFechaDateServer;
                EObservacion = string.Empty;
                ETipoEmpleado = -1;
                EAreaTrabajo = -1;
            }
            if (SelectPersona.PERSONA_BIOMETRICO != null)
            {
                if (SelectPersona.PERSONA_BIOMETRICO.Any(a => a.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO))
                    ImagenEmpleado = SelectPersona.PERSONA_BIOMETRICO.First(f => f.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO &&
                        f.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO;
                else
                    ImagenEmpleado = new Imagenes().getImagenPerson();
            }
            StaticSourcesViewModel.SourceChanged = false;
        }

        private async void GuardarEmpleado()
        {
            if (base.HasErrors)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Debe capturar los campos obligatorios."+base.Error);
                return;
            }

            var respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("GUARDANDO EMPLEADO", () =>
            {
                try
                {
                    var persona = new SSP.Servidor.PERSONA();
                    persona.ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EMPLEADO;
                    persona.PATERNO = EPaterno;
                    persona.MATERNO = EMaterno;
                    persona.NOMBRE = ENombre;
                    persona.SEXO = ESexo;
                    persona.DOMICILIO_CALLE = ECalle;
                    persona.DOMICILIO_NUM_EXT = ENoExterior;
                    persona.DOMICILIO_NUM_INT = ENoInterior;
                    persona.ID_COLONIA = EColonia != -1 ? EColonia : null;
                    persona.ID_MUNICIPIO = EMunicipio != -1 ? EMunicipio : null;
                    persona.ID_ENTIDAD = EEstado != -1 ? EEstado : null;
                    persona.ID_PAIS = EPais != -1 ? EPais : null;
                    persona.DOMICILIO_CODIGO_POSTAL = ECP;
                    persona.CURP = ECURP;
                    persona.RFC = ERFC;
                    persona.FEC_NACIMIENTO = EFechaNacimiento;
                    persona.TELEFONO = ETelefonoFijo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                    persona.TELEFONO_MOVIL = ETelefonoMovil.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                    persona.CORREO_ELECTRONICO = ECorreo;
                    persona.ID_TIPO_DISCAPACIDAD = ETipoDiscapacidad != -1 ? ETipoDiscapacidad : null;
                    //Empleado
                    var empleado = new EMPLEADO();
                    empleado.ID_EMPLEADO = 0;
                    empleado.ID_TIPO_EMPLEADO = ETipoEmpleado;
                    empleado.ID_DEPARTAMENTO = EAreaTrabajo;
                    empleado.ID_CENTRO = SelectedCentroValue;
                    empleado.OBSERV = EObservacion;
                    empleado.ESTATUS = "S";
                    empleado.REGISTRO_FEC = Fechas.GetFechaDateServer;
                    empleado.CEDULA = ECedulaProfesional;
                    //Biometrico Foto
                    var fotos = new SSP.Servidor.PERSONA_BIOMETRICO();
                    var huellas = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                    if (ImagenEmpleado!=null && ImagenEmpleado.Length != 1882)
                    {
                        fotos = new SSP.Servidor.PERSONA_BIOMETRICO
                        {
                            BIOMETRICO = ImagenEmpleado,
                            ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                            ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                            ID_PERSONA = 0
                        };
                    }
                    //Biometrico Huellas
                    if (HuellasCapturadas != null)
                    {
                        foreach (var h in HuellasCapturadas)
                        {
                            huellas.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                            {
                                ID_PERSONA = 0,
                                BIOMETRICO = h.BIOMETRICO,
                                ID_TIPO_BIOMETRICO = (short)h.ID_TIPO_BIOMETRICO,
                                ID_FORMATO = (short)h.ID_TIPO_FORMATO
                            });
                        }
                    }
                    var TipoVisitaEmpleado = Parametro.ID_TIPO_VISITA_EMPLEADO;
                    var PersonaNIP = new PERSONA_NIP();
                    if (Actualiza)
                    {
                        persona.ID_PERSONA = SelectPersona.ID_PERSONA;
                        persona.CORIGINAL = SelectPersona.CORIGINAL;
                        persona.ID_ETNIA = SelectPersona.ID_ETNIA;
                        persona.IFE = SelectPersona.IFE;
                        persona.LUGAR_NACIMIENTO = SelectPersona.LUGAR_NACIMIENTO;
                        persona.TELEFONO_MOVIL = SelectPersona.TELEFONO_MOVIL;
                        persona.NACIONALIDAD = SelectPersona.NACIONALIDAD;
                        persona.NORIGINAL = SelectPersona.NORIGINAL;
                        persona.SMATERNO = SelectPersona.SMATERNO;
                        persona.SPATERNO = SelectPersona.SMATERNO;
                        persona.SNOMBRE = SelectPersona.SNOMBRE;
                        empleado.ID_EMPLEADO = persona.ID_PERSONA;
                        if (NuevoEmpleado)
                        {
                            //Persona NIP
                            PersonaNIP = new PERSONA_NIP()
                            {
                                NIP = new cPersona().GetSequence<int>("NIP_SEQ"),
                                ID_TIPO_VISITA = TipoVisitaEmpleado,
                                ID_PERSONA = persona.ID_PERSONA,
                                ID_CENTRO = GlobalVar.gCentro
                            };
                        }
                        else
                            PersonaNIP = null;
                    }
                    else
                    {
                        //Persona NIP
                        PersonaNIP = new PERSONA_NIP()
                        {
                            NIP = new cPersona().GetSequence<int>("NIP_SEQ"),
                            ID_TIPO_VISITA = TipoVisitaEmpleado,
                            ID_CENTRO = GlobalVar.gCentro
                        };
                    }
                    persona.ID_PERSONA = new cPersona().InsertarEmpleado(persona, Fechas.GetFechaDateServer.Year, Actualiza, NuevoEmpleado, empleado, PersonaNIP, fotos, huellas);
                    if (persona.ID_PERSONA > 0)
                    {
                        SelectedEmpleado = new cEmpleado().Obtener(persona.ID_PERSONA);
                        SelectPersona = SelectedEmpleado.PERSONA;
                        ECodigo = SelectedEmpleado.ID_EMPLEADO;
                        EFechaAlta = SelectedEmpleado.REGISTRO_FEC.HasValue ? SelectedEmpleado.REGISTRO_FEC : null;
                        Actualiza = true;
                        return true;
                    }
                    else
                        return false;


                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo paso...", "Ocurrio un error...", ex);
                    return false;
                }
            });

            if (respuesta)
            {
                StaticSourcesViewModel.SourceChanged = false;
                new Dialogos().ConfirmacionDialogo("Éxito", "Información grabada exitosamente!");
            }
            //else
              ///  new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la información");
        }

        private bool ActualizarEmpleado()
        {
            try
            {
                var empleado = new EMPLEADO();
                empleado.ID_EMPLEADO = SelectedEmpleado.ID_EMPLEADO;
                empleado.ID_TIPO_EMPLEADO = ETipoEmpleado;
                empleado.ID_DEPARTAMENTO = EAreaTrabajo;
                empleado.ID_CENTRO = GlobalVar.gCentro;
                empleado.OBSERV = EObservacion;
                empleado.ESTATUS = SelectedEmpleado.ESTATUS;
                empleado.REGISTRO_FEC = Fechas.GetFechaDateServer;
                empleado.CEDULA = ECedulaProfesional;
                if (new cEmpleado().Actualizar(empleado))
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al actualizar empleado.", ex);
            }
            return false;
        }

        private bool ActualizarImagen()
        {
            try
            {
                if (ImagenEmpleado.Length == 1882)
                    return true;
                //actualizamos la foto
                if (SelectedEmpleado.PERSONA.PERSONA_BIOMETRICO != null)
                {
                    var bio = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                    bio.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                    {
                        BIOMETRICO = ImagenEmpleado,
                        ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                        ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                        ID_PERSONA = SelectedEmpleado.PERSONA.ID_PERSONA
                    });

                    if (SelectedEmpleado.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Count() > 0)
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

                if (SelectedEmpleado.PERSONA.PERSONA_BIOMETRICO != null)
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
                                ID_PERSONA = SelectedEmpleado.PERSONA.ID_PERSONA
                            });
                        }

                        if (SelectedEmpleado.PERSONA.PERSONA_BIOMETRICO.Where(w =>
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
        private void LimpiarBusqueda()
        {
            try
            {
                PaternoE = MaternoE = NombreE = string.Empty;
                NipE = null;
                ImagenEmpleadoPop = new Imagenes().getImagenPerson();
                LstEmpleadoPop = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
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
                Application.Current.Dispatcher.Invoke((Action)(async delegate
                {
                    LstEmpleadoPop = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    LstEmpleadoPop.InsertRange(await SegmentarPersonasBusqueda());
                    //new cPersona().ObtenerTodos(NombreE, PaternoE, MaternoE).OrderByDescending(o => o.ID_TIPO_PERSONA == empleado));
                    //new cEmpleado().ObtenerTodos(GlobalVar.gCentro, NipE, PaternoE, MaternoE, NombreE).Select(s => s.PERSONA));
                    EmpleadoEmpty = LstEmpleadoPop.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar empleado.", ex);
            }
        }

        private async Task<List<SSP.Servidor.PERSONA>> SegmentarPersonasBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(PaternoE) && string.IsNullOrEmpty(MaternoE) && string.IsNullOrEmpty(NombreE) && NipE == null)
                    return new List<SSP.Servidor.PERSONA>();
                Pagina = _Pag;
                //var empleado = short.Parse(Parametro.ID_TIPO_PERSONA_EMPLEADO);
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                        new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodosXEmpleados(NombreE, PaternoE, MaternoE,NipE, _Pag,0)));
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

        private async void HeaderSort(Object obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    if (obj != null ? obj.ToString() == "Tipo visita" : false)
                    {
                        var aux = LstEmpleadoPop;
                        LstEmpleadoPop = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                        switch (HeaderSortin)
                        {
                            case true:
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    LstEmpleadoPop.InsertRange(aux.OrderByDescending(o => o.EMPLEADO != null));
                                }));
                                HeaderSortin = false;
                                break;
                            case false:
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    LstEmpleadoPop.InsertRange(aux.OrderByDescending(o => o.EMPLEADO == null));
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
        #endregion

        #region Tomar Foto
        private async void TomarFotoLoad(TomarFotoSenaParticularView Window = null)
        {
            try
            {
                if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                CamaraWeb = new ControlPenales.Clases.WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
                if (ImagenEmpleado!=null && ImagenEmpleado.Length != 1882)
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
                                    DPUruNet.Fmd FMD = null;
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
                    //ShowPopUp = Visibility.Hidden;
                    
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                    ShowPopUp = Visibility.Hidden;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    }));
                    //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
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
                windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_EMPLEADO, nRet == 0, requiereGuardarHuellas);
                windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Persona")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "TIPO VISITA"
                });
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
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    var huella = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (!huella.IsSucceed)
                        return;
                    if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_EMPLEADO);
                        return;
                    }
                    if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null)
                        return;
                    if (StaticSourcesViewModel.SourceChanged)
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1)
                            return;
                    SelectPersona = huella.SelectRegistro.Persona;
                    if (SelectPersona.EMPLEADO != null)
                    {
                        SelectedEmpleado = SelectPersona.EMPLEADO;
                        EnabledCampo = false;
                        ObtenerEmpleado();
                    }
                    else
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "LA PERSONA SELECCIONADA NO ESTA REGISTRADA COMO EMPLEADO,"
                                + " DESEA REGISTRARLA AHORA?") == 1)
                        {
                            SelectedEmpleado = SelectPersona.EMPLEADO;
                            EnabledCampo = false;
                            ObtenerEmpleado();
                        }
                    }
                    if (PEditar)
                    {
                        EliminarMenuEnabled = true;
                        EditarMenuEnabled = true;
                    }
                    CancelarMenuEnabled = true;
                    GuardarMenuEnabled = false;
                    BuscarMenuEnabled = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
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
                windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_EMPLEADO, nRet == 0, requiereGuardarHuellas);
                windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Persona")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "TIPO VISITA"
                });
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
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    var huella = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (!huella.IsSucceed)
                        return;
                    if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_EMPLEADO);
                        return;
                    }
                    if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null)
                        return;
                    if (StaticSourcesViewModel.SourceChanged)
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar, esta seguro que desea seleccionar a otra persona?") != 1)
                            return;
                    SelectPersona = huella.SelectRegistro.Persona;
                    if (SelectPersona.EMPLEADO != null)
                    {
                        SelectedEmpleado = SelectPersona.EMPLEADO;
                        EnabledCampo = false;
                        ObtenerEmpleado();
                    }
                    else
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "LA PERSONA SELECCIONADA NO ESTA REGISTRADA COMO EMPLEADO,"
                                + " DESEA REGISTRARLA AHORA?") == 1)
                        {
                            SelectedEmpleado = SelectPersona.EMPLEADO;
                            EnabledCampo = false;
                            ObtenerEmpleado();
                        }
                    }
                    if (PEditar)
                    {
                        EliminarMenuEnabled = true;
                        EditarMenuEnabled = true;
                    }
                    CancelarMenuEnabled = true;
                    GuardarMenuEnabled = false;
                    BuscarMenuEnabled = false;
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
