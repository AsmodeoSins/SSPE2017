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
using ControlPenales.BiometricoServiceReference;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel : ValidationViewModelBase
    {

        #region constructor
        public RegistroDecomisoViewModel()
        { }
        #endregion

        #region variables

        public string banderaTab;
        private System.Windows.Controls.TabItem tabSelected;
        public System.Windows.Controls.TabItem TabSelected
        {
            get { return tabSelected; }
            set
            {
                tabSelected = value; OnPropertyChanged("TabSelected");
                banderaTab = tabSelected.ToString().Split(':')[1].Split(' ')[0];
            }
        }

        #region Enableds
        private bool oficialesEnabled;
        public bool OficialesEnabled
        {
            get { return oficialesEnabled; }
            set { oficialesEnabled = value; OnPropertyChanged("OficialesEnabled"); }
        }
        private bool objetosEnabled;
        public bool ObjetosEnabled
        {
            get { return objetosEnabled; }
            set { objetosEnabled = value; OnPropertyChanged("ObjetosEnabled"); }
        }
        private bool internosEnabled;
        public bool InternosEnabled
        {
            get { return internosEnabled; }
            set { internosEnabled = value; OnPropertyChanged("InternosEnabled"); }
        }
        private bool proveedoresEnabled;
        public bool ProveedoresEnabled
        {
            get { return proveedoresEnabled; }
            set { proveedoresEnabled = value; OnPropertyChanged("ProveedoresEnabled"); }
        }
        private bool resumenEnabled;
        public bool ResumenEnabled
        {
            get { return resumenEnabled; }
            set { resumenEnabled = value; OnPropertyChanged("ResumenEnabled"); }
        }
        private bool empleadosEnabled;
        public bool EmpleadosEnabled
        {
            get { return empleadosEnabled; }
            set { empleadosEnabled = value; OnPropertyChanged("EmpleadosEnabled"); }
        }
        private bool visitasEnabled;
        public bool VisitasEnabled
        {
            get { return visitasEnabled; }
            set { visitasEnabled = value; OnPropertyChanged("VisitasEnabled"); }
        }
        #endregion

        #region Visibles
        private bool objetoValesVisible;
        public bool ObjetoValesVisible
        {
            get { return objetoValesVisible; }
            set { objetoValesVisible = value; OnPropertyChanged("ObjetoValesVisible"); }
        }
        private bool objetoCelularVisible;
        public bool ObjetoCelularVisible
        {
            get { return objetoCelularVisible; }
            set { objetoCelularVisible = value; OnPropertyChanged("ObjetoCelularVisible"); }
        }
        private bool objetoTarjetaTelefonicaVisible;
        public bool ObjetoTarjetaTelefonicaVisible
        {
            get { return objetoTarjetaTelefonicaVisible; }
            set { objetoTarjetaTelefonicaVisible = value; OnPropertyChanged("ObjetoTarjetaTelefonicaVisible"); }
        }
        private bool objetoTarjetaSimVisible;
        public bool ObjetoTarjetaSimVisible
        {
            get { return objetoTarjetaSimVisible; }
            set { objetoTarjetaSimVisible = value; OnPropertyChanged("ObjetoTarjetaSimVisible"); }
        }
        private bool objetoReproductorVisible;
        public bool ObjetoReproductorVisible
        {
            get { return objetoReproductorVisible; }
            set { objetoReproductorVisible = value; OnPropertyChanged("ObjetoReproductorVisible"); }
        }
        private bool objetoRadioVisible;
        public bool ObjetoRadioVisible
        {
            get { return objetoRadioVisible; }
            set { objetoRadioVisible = value; OnPropertyChanged("ObjetoRadioVisible"); }
        }
        private bool objetoControlVisible;
        public bool ObjetoControlVisible
        {
            get { return objetoControlVisible; }
            set { objetoControlVisible = value; OnPropertyChanged("ObjetoControlVisible"); }
        }
        private bool objetoDrogaVisible;
        public bool ObjetoDrogaVisible
        {
            get { return objetoDrogaVisible; }
            set { objetoDrogaVisible = value; OnPropertyChanged("ObjetoDrogaVisible"); }
        }
        private bool objetoSinInformacionVisible;
        public bool ObjetoSinInformacionVisible
        {
            get { return objetoSinInformacionVisible; }
            set { objetoSinInformacionVisible = value; OnPropertyChanged("ObjetoSinInformacionVisible"); }
        }
        private bool buscarActivadoVisible;
        public bool BuscarActivadoVisible
        {
            get { return buscarActivadoVisible; }
            set { buscarActivadoVisible = value; OnPropertyChanged("BuscarActivadoVisible"); }
        }
        private bool buscarVisible;
        public bool BuscarVisible
        {
            get { return buscarVisible; }
            set { buscarVisible = value; OnPropertyChanged("BuscarVisible"); }
        }
        private bool popupBuscarCeldaVisible;
        public bool PopupBuscarCeldaVisible
        {
            get { return popupBuscarCeldaVisible; }
            set { popupBuscarCeldaVisible = value; OnPropertyChanged("PopupBuscarCeldaVisible"); }
        }
        #endregion

        #region Otros
        List<string> _source = new List<string> { "Item 1", "Item 2", "Item 3" };
        public List<string> Source { get { return _source; } }
        string _selectedItem = null;
        public string SelectedItem
        {
            get
            {
                if (_selectedItem == null)
                { PopupBuscarCeldaVisible = false; }
                else
                { PopupBuscarCeldaVisible = true; }
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                //NotifyPropertyChanged
            }
        }
        List<string> _source2 = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public List<string> Source2 { get { return _source2; } }
        string _selectedItem2 = null;
        public string SelectedItem2
        {
            get
            {
                ObjetoCelularVisible = false;
                ObjetoControlVisible = false;
                ObjetoDrogaVisible = false;
                ObjetoRadioVisible = false;
                ObjetoReproductorVisible = false;
                ObjetoSinInformacionVisible = false;
                ObjetoTarjetaSimVisible = false;
                ObjetoTarjetaTelefonicaVisible = false;
                ObjetoValesVisible = false;
                if (_selectedItem2 == "1")
                {
                    ObjetoSinInformacionVisible = true;
                }
                else if (_selectedItem2 == "2")
                {
                    ObjetoControlVisible = true;
                }
                else if (_selectedItem2 == "3")
                {
                    ObjetoDrogaVisible = true;
                }
                else if (_selectedItem2 == "4")
                {
                    ObjetoRadioVisible = true;
                }
                else if (_selectedItem2 == "5")
                {
                    ObjetoReproductorVisible = true;
                }
                else if (_selectedItem2 == "6")
                {
                    ObjetoTarjetaSimVisible = true;
                }
                else if (_selectedItem2 == "7")
                {
                    ObjetoTarjetaTelefonicaVisible = true;
                }
                else if (_selectedItem2 == "8")
                {
                    ObjetoCelularVisible = true;
                }
                else if (_selectedItem2 == "9")
                {
                    ObjetoValesVisible = true;
                }
                return _selectedItem2;
            }
            set
            {
                _selectedItem2 = value;
                //NotifyPropertyChanged
            }
        }
        public string Name
        {
            get
            {
                return "registro_decomiso";
            }
        }
        #endregion
        #endregion

        #region metodos
        private async void clickSwitch(Object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "seleccionar_delito_buscar":
                        PopupBuscarCeldaVisible = false;
                        break;
                    case "cancelar_seleccionar_delito":
                        PopupBuscarCeldaVisible = false;
                        break;
                    case "buscar_menu":
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_DECOMISO_EVENTO);
                        break;
                    case "buscar":
                        //await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateListado);
                        LstDecomisos = new RangeEnabledObservableCollection<DECOMISO>();
                        LstDecomisos.InsertRange(await SegmentarDecomisoBusqueda());
                        DecomisosEmpty = LstDecomisos.Count > 0 ? Visibility.Collapsed : Visibility.Visible; 
                        break;
                    case "buscar_seleccionar":
                        if (IndexTab == 1)
                        {
                            if (SelectIngreso != null)
                            {
                                if (LstInternoInvolucrado == null)
                                    LstInternoInvolucrado = new ObservableCollection<DECOMISO_INGRESO>();
                                int[] estatus = { (short)enumEstatusAdministrativo.LIBERADO, (short)enumEstatusAdministrativo.TRASLADADO, (short)enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD, (short)enumEstatusAdministrativo.DISCRECIONAL };
                                if (estatus.Count(w => w == SelectIngreso.ID_ESTATUS_ADMINISTRATIVO.Value) > 0)
                                { 
                                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un ingreso activo");
                                    break;
                                }
                                else
                                    if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                                    {
                                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado pertenece a otro centro");
                                        break;
                                    }
                                    else
                                        if (LstInternoInvolucrado.Count(w => w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_ANIO == SelectIngreso.ID_ANIO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ID_INGRESO == SelectIngreso.ID_INGRESO) > 0)
                                        {
                                            new Dialogos().ConfirmacionDialogo("Validación", "El interno ya se encuentra registrado");
                                            break;
                                        }
                          
                                LstInternoInvolucrado.Add(new DECOMISO_INGRESO()
                                {
                                    ID_DECOMISO = SelectedDecomiso != null ? SelectedDecomiso.ID_DECOMISO : 0,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    INGRESO = SelectIngreso
                                });
                                InternoInvolucradoVisible = Visibility.Collapsed;
                                LimpiarBusquedaInterno();
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            }
                        }
                        else
                        {
                            BuscarActivadoVisible = true;
                            BuscarVisible = false;
                            setTab();
                        }
                        break;
                    case "buscar_salir":
                        if (IndexTab == 1)
                        {
                            LimpiarBusquedaInterno();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        }
                        else
                        { 
                            BuscarActivadoVisible = true;
                            BuscarVisible = false;
                            setTab();
                        }
                        break;
                    case "seleccionar_ubicacion":
                        ViewModelArbolUbicacion();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONA_UBICACION_CELDA);
                        break;
                    case "cancelar_ubicacion":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONA_UBICACION_CELDA);
                        break;
                    case "salir_decomiso":
                        LimpiarBusquedaDecomiso();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_DECOMISO_EVENTO);
                        break;
                    case "seleccionar_decomiso":
                        if (SelectedDecomiso != null)
                        {
                            if (StaticSourcesViewModel.SourceChanged)
                            {
                                var respuesta = await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Seguro que desea salir sin guardar?");
                                if (respuesta == 1)
                                {
                                    ObtenerDecomiso();
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_DECOMISO_EVENTO);
                                    LimpiarBusquedaDecomiso();
                                    StaticSourcesViewModel.SourceChanged = false;
                                }
                            }
                            else
                            {
                                ObtenerDecomiso();
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_DECOMISO_EVENTO);
                                LimpiarBusquedaDecomiso();
                                StaticSourcesViewModel.SourceChanged = false;
                            }
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un decomiso");
                        break;
                    case "limpiar_busqueda_decomiso":
                        LimpiarBusquedaDecomiso();
                        break;
                    case "guardar_menu":
                        SetValidacionesDecomiso();
                        if (!base.HasErrors)
                        {
                            if (ValidarGuardarDecomiso())
                            {
                                if (GuardarDecomiso())
                                {
                                    new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                                    StaticSourcesViewModel.SourceChanged = false;
                                }
                                //else
                                //    new Dialogos().ConfirmacionDialogo("ERROR", "No se registr\u00F3 la informaci\u00F3n.");
                            }
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos. "+base.Error);
                            //ValidarResumen();
                        break;
                    //OFICIAL CARGO
                    case "addOficialCargo":
                        LimpiarOficialPop();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_OFICIALACARGO);
                        break;
                    case "delOficialCargo":
                        EliminarOficialACargo();
                        break;
                    case "buscar_oficial_pop":
                        //await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateBuscarOficial);
                        PopulateBuscarOficial();
                        break;
                    case "limpiar_oficial_pop":
                        LimpiarOficial();
                        break;
                    case "seleccionar_oficial_pop":
                        EsOficial = true;
                        if (AgregarPersona((short)enumTipoPersona.PERSONA_EMPLEADO))
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_OFICIALACARGO);
                        break;
                    case "salir_oficial_pop":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_OFICIALACARGO);
                        break;
                    //INTERNO
                    //case "buscar_interno":
                    //    LimpiarBusquedaInterno();
                    //    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO);
                    //    break;
                    //case "buscar_interno_pop":
                    //    await StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarInterno);

                    //    break;
                    //case "seleccionar_interno_pop":
                    //    if (AgregarInternoInvolucrado())
                    //    {
                    //        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO);
                    //        LimpiarBusquedaInterno();
                    //    }
                    //    break;
                    //case "salir_interno_pop":
                    //    LimpiarBusquedaInterno();
                    //    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO);
                    //    break;
                    case "nueva_busqueda":
                        LimpiarBusquedaInterno();
                        break;
                    //case "buscar_seleccionar":
                    //    break;
                    //case "buscar_salir":
                    //    break;
                    case "addInterno":
                        //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "delInterno":
                        EliminarInternoInvolucrado();
                        break;
                    //VISITANTE
                    case "addVisitante":
                        LimpiarVisitante();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITANTE);
                        break;
                    case "delVisitante":
                        EliminarVisitante();
                        break;
                    case "buscar_visitante_pop":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateBuscarVisita);
                        break;
                    case "limpiar_visitante_pop":
                        LimpiarVisitante();
                        break;
                    case "seleccionar_visitante_pop":
                        if (AgregarPersona((short)enumTipoPersona.PERSONA_VISITA))
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_VISITANTE);
                            LimpiarBusquedaVisitante();
                        }
                        break;
                    case "salir_visitante_pop":
                        LimpiarBusquedaVisitante();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_VISITANTE);
                        break;
                        //ABOGADO
                    case "addAbogado":
                        //LimpiarVisitante();
                        //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITANTE);
                        Pagina = 1;
                        SeguirCargandoPersonas = true;
                        TextNombre = TextMaterno = TextPaterno = string.Empty;
                        ListPersonas = null;
                        SelectPersona = null;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        break;
                    case "delAbogado":
                        EliminarAbogado();
                        break;
                    case "buscar_Abogado_pop":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateBuscarVisita);
                        break;
                    case "limpiar_Abogado_pop":
                        LimpiarVisitante();
                        break;
                    case "seleccionar_buscar_persona":
                        if (SelectPersona == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación!", "Debes seleccionar a una persona.");
                            break;
                        }
                        else
                        if (SelectPersona.ABOGADO != null)
                        {
                            if (LstAbogadoInvolucrada == null)
                                LstAbogadoInvolucrada = new ObservableCollection<DECOMISO_PERSONA>();

                            if (LstAbogadoInvolucrada.Count(w => w.ID_PERSONA == SelectPersona.ID_PERSONA) == 0)
                            { 
                            LstAbogadoInvolucrada.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = SelectPersona.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_ABOGADO,
                                PERSONA = SelectPersona
                            });
                            AbogadoInvolucradoVisible = LstAbogadoInvolucrada.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Validación!", "El abogado ya se encuentra en la lista.");
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación!", "La persona seleccionada no es abogado.");
                            
                        break;
                    case "cancelar_buscar_persona":
                         ListPersonas = null;
                         SelectPersona =  null;
                         TextNombre = TextMaterno = TextPaterno = string.Empty;
                         PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                        break;
                    case "nueva_busqueda_visitante":
                        TextPaterno = TextMaterno = TextNombre = string.Empty;
                        ListPersonas = null;
                        SelectPersona = null;
                        break;
                    case "buscar_visitante":
                        if (TextPaterno == null)
                            TextPaterno = string.Empty;
                        if (TextMaterno == null)
                            TextMaterno = string.Empty;
                        if (TextNombre == null)
                            TextNombre = string.Empty;
                        BuscarPersonasSinCodigo();
                        break;
                    //EMPLEADOS
                    case "addEmpleado":
                        LimpiarEmpleado();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_EMPLEADO);
                        break;
                    case "delEmpleado":
                        EliminarEmpleadoInvolucrado();
                        break;
                    case "buscar_empleado_pop":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateBuscarEmpleado);
                        break;
                    case "limpiar_empleado_pop":
                        LimpiarEmpleado();
                        break;
                    case "seleccionar_empleado_pop":
                        EsOficial = false;
                        if (AgregarPersona((short)enumTipoPersona.PERSONA_EMPLEADO))
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EMPLEADO);
                            LimpiarBusquedaEmpleado();
                        }
                        break;
                    case "salir_empleado_pop":
                        LimpiarBusquedaEmpleado();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EMPLEADO);
                        break;
                    //EXTERNO
                    case "addExterno":
                        LimpiarExterno();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_EXTERNO);
                        break;
                    case "delExterno":
                        EliminarExterno();
                        break;
                    case "buscar_externo_pop":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(PopulateBuscarExterno);
                        break;
                    case "limpiar_externo_pop":
                        LimpiarExterno();
                        break;
                    case "seleccionar_externo_pop":
                        if (AgregarPersona((short)enumTipoPersona.PERSONA_EXTERNA))
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EXTERNO);
                            LimpiarBusquedaVisitanteExterno();
                        }
                        break;
                    case "salir_externo_pop":
                        LimpiarBusquedaVisitanteExterno();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EXTERNO);
                        break;
                    //OBJETOS
                    case "addObjeto":
                        LimpiarObjeto();
                        break;
                    case "editObjeto":
                        if (SelectedObjeto != null)
                        {
                            BtnLimpiarText = "Cancelar";
                            ObtenerObjeto();
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un objeto.");
                        break;
                    case "delObjeto":
                        if (SelectedObjeto != null)
                        {
                            EliminarObjeto();
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un objeto.");
                        break;
                    case "guardar_objeto":
                        AgregarObjeto();
                        break;
                    //IMAGEN OBJETO
                    case "delImagenObjeto":
                        if (SelectedImagen != null)
                        {
                            EliminarImagen();
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar imagen a eliminar");
                        break;
                    case "imagen_objeto":
                        // if (SelectedObjeto != null)
                        //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        //else
                        //    new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar un ingreso");
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
                        break;
                    case "tomar_foto":
                        //var aux = ImagenObjeto;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        //ImagenObjeto = aux;
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
                                    ImageFrontal.Add(new ImageSourceToSave { FrameName = "ImgSenaParticular", ImageCaptured = new Imagenes().ConvertByteToBitmap(ImagenObjeto) });
                                    BotonTomarFotoEnabled = true;
                                }
                            }
                            if (CamaraWeb != null)
                            {
                                await CamaraWeb.ReleaseVideoDevice();
                                CamaraWeb = null;
                            }
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
                        }
                        break;
                    case "aceptar_tomar_foto_senas":
                        try
                        {
                            if (ImagenObjeto != new Imagenes().getImagenPerson() && (ImageFrontal != null ? ImageFrontal.Count != 0 : false))
                            {
                                FotoTomada = true;
                                if (ImageFrontal.FirstOrDefault().ImageCaptured == null)
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "DEBES DE TOMAR UNA FOTO.");
                                    return;
                                }
                                ImagenObjeto = new Imagenes().ConvertBitmapToByte(ImageFrontal.FirstOrDefault().ImageCaptured);
                                if (LstImagenes == null)
                                    LstImagenes = new ObservableCollection<DECOMISO_IMAGEN>();
                                LstImagenes.Add(new DECOMISO_IMAGEN()
                                {
                                    IMAGEN = ImagenObjeto
                                });
                                ImagenVisible = LstImagenes.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
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
                    case "limpiar_busqueda":
                        AnioI = null;
                        FolioI = null;
                        NombreI = PaternoI = MaternoI = string.Empty;
                        //LstIngreso = null;
                        SelectedIngreso = null;
                        break;
                    case "limpiar_menu":
                         StaticSourcesViewModel.SourceChanged = false;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RegistroDecomisoView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new RegistroDecomisoViewModel();
                        break;
                    case "reporte_menu":
                        if (SelectedDecomiso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un decomiso");
                            break;
                        }
                        //ValidacionImpresionDocumento();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.IMPRESION_DECOMISO);
                        break;
                    case "imprimir_decomiso":
                        //if (GuardarImpresionDecomiso())
                        //{
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.IMPRESION_DECOMISO);
                            //SetValidacionesDecomiso();
                            ImpresionFormatoDecomiso();
                        //}
                        //else
                        //    new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un problema al guardar la información.");
                        break;
                    case "cancelar_decomiso":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.IMPRESION_DECOMISO);
                        SetValidacionesDecomiso();
                        break;
                    case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar opción", ex);
            }
        }

        private async void RegistroDecomisoLoad(RegistroDecomisoView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(inicializa);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        private void inicializa()
        {
            try
            {
                LstGrupoTactico = new ObservableCollection<GRUPO_TACTICO>(new cGrupoTactico().ObtenerTodos().OrderBy(w => w.DESCR));
                LstArea = new ObservableCollection<AREA>(new cArea().ObtenerTodos(string.Empty).OrderBy(w => w.DESCR));
                LstTurno = new ObservableCollection<TURNO>(new cTurno().ObtenerTodos().OrderBy(w => w.DESCR));
                LstObjetoTipo = new ObservableCollection<OBJETO_TIPO>(new cObjetoTipo().ObtenerTodos().OrderBy(w => w.DESCR));
                LstCompania = new ObservableCollection<COMPANIA>(new cCompania().ObtenerTodos().OrderBy(w => w.DESCR));
                LstDroga = new ObservableCollection<DROGA>(new cDrogas().ObtenerTodos().OrderBy(w => w.DESCR));
                LstUnidadMedida = new ObservableCollection<DROGA_UNIDAD_MEDIDA>(new cDrogaUM().ObtenerTodos("S").OrderBy(w => w.DESCR));
                LstFabricante = new ObservableCollection<DECOMISO_FABRICANTE>(new cDecomisoFabricante().ObtenerTodos().OrderBy(w => w.DESCR));
                //MODELO
                LstModelo = new ObservableCollection<DECOMISO_MODELO>();

                Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstGrupoTactico.Insert(0, new GRUPO_TACTICO() { ID_GRUPO_TACTICO = -1, DESCR = "SELECCIONE" });
                        LstArea.Insert(0, new AREA() { ID_AREA = -1, DESCR = "SELECCIONE" });
                        LstTurno.Insert(0, new TURNO() { ID_TURNO = -1, DESCR = "SELECCIONE" });
                        LstObjetoTipo.Insert(0, new OBJETO_TIPO() { ID_OBJETO_TIPO = -1, DESCR = "SELECCIONE" });
                        SelectedObjetoTipo = LstObjetoTipo.Where(w => w.ID_OBJETO_TIPO == -1).FirstOrDefault();
                        LstCompania.Insert(0, new COMPANIA() { ID_COMPANIA = -1, DESCR = "SELECCIONE" });
                        LstDroga.Insert(0, new DROGA() { ID_DROGA = -1, DESCR = "SELECCIONE" });
                        LstUnidadMedida.Insert(0, new DROGA_UNIDAD_MEDIDA() { ID_UNIDAD_MEDIDA = 0, DESCR = "SELECCIONE" });
                        LstFabricante.Insert(0, new DECOMISO_FABRICANTE() { ID_FABRICANTE = -1, DESCR = "SELECCIONE" });
                        LstModelo.Insert(0, new DECOMISO_MODELO() { ID_MODELO = -1, DESCR = "SELECCIONE" });


                        BuscarActivadoVisible = true;
                        PopupBuscarCeldaVisible = false;
                        BuscarVisible = false;
                        ObjetoCelularVisible = false;
                        ObjetoControlVisible = false;
                        ObjetoDrogaVisible = false;
                        ObjetoRadioVisible = false;
                        ObjetoReproductorVisible = false;
                        ObjetoSinInformacionVisible = false;
                        ObjetoTarjetaSimVisible = false;
                        ObjetoTarjetaTelefonicaVisible = false;
                        ObjetoValesVisible = false;
                        OficialesEnabled = true;
                        ObjetosEnabled = true;
                        InternosEnabled = true;
                        ProveedoresEnabled = true;
                        ResumenEnabled = true;
                        EmpleadosEnabled = true;
                        VisitasEnabled = true;
                        TabSelected = new System.Windows.Controls.TabItem();
                        TabSelected.IsSelected = true;
                        //InfoInternoVisible = false;
                        // CargarListas();
                        ConfiguraPermisos();
                        SetValidacionesDecomiso();
                        StaticSourcesViewModel.SourceChanged = false;
                    }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al inicializar pantalla", ex);
            }
        }

        private async void TomarFotoLoad(TomarFotoSenaParticularView Window = null)
        {
            try
            {
                if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
                BotonTomarFotoEnabled = false;
                ImageFrontal = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla para tomar foto.", ex);
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
                    StaticSourcesViewModel.SourceChanged = true;
                    StaticSourcesViewModel.Mensaje("FOTO DECOMISO", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    CamaraWeb.QuitarFoto(Picture);
                    ImageFrontal.Remove(ImageFrontal.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                }
                if (ImageFrontal != null ? ImageFrontal.Count != 0 : false)
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

        #region Busqueda
        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;

                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "AnioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    AnioBuscar = int.Parse(textbox.Text);
                                else
                                    AnioBuscar = null;
                                break;
                            case "FolioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    FolioBuscar = int.Parse(textbox.Text);
                                else
                                    FolioBuscar = null;
                                break;
                        }
                    }
                }
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;
                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async void ClickDecomisoEnter(Object obj)
        {
            if (obj != null)
            {
                var textbox = (System.Windows.Controls.TextBox)obj;
                switch (textbox.Name)
                {
                    case "PaternoBuscarDecomiso":
                        PaternoB = textbox.Text;
                        break;
                    case "MaternoBuscarDecomiso":
                        MaternoB = textbox.Text;
                        break;
                    case "NombreBuscarDecomiso":
                        NombreB = textbox.Text;
                        break;
                }
                //PopulateListado();
                LstDecomisos = new RangeEnabledObservableCollection<DECOMISO>();
                LstDecomisos.InsertRange(await SegmentarDecomisoBusqueda());
                DecomisosEmpty = LstDecomisos.Count > 0 ? Visibility.Collapsed : Visibility.Visible; 
            }
        }
        
        private void ClickOficialEnter(Object obj)
        {
            if (obj != null)
            {
                var textbox = (System.Windows.Controls.TextBox)obj;
                switch (textbox.Name)
                {
                    case "NoControl":
                        ONoControl = textbox.Text;
                        break;
                    case "PaternoOficial":
                        OPaterno = textbox.Text;
                        break;
                    case "MaternoOficial":
                        OMaterno = textbox.Text;
                        break;
                    case "NombreOficial":
                        ONombre = textbox.Text;
                        break;
                }
                PopulateBuscarOficial();
            }
        }

        //private void ClickEnterInterno(Object obj)
        //{
        //    if (obj != null)
        //    {
        //        var textbox = (System.Windows.Controls.TextBox)obj;
        //        switch (textbox.Name)
        //        {
        //            case "AnioBuscar":
        //                AnioI = short.Parse(textbox.Text);
        //                break;
        //            case "FolioBuscar":
        //                FolioI = short.Parse(textbox.Text);
        //                break;
        //            case "PaternoBuscar":
        //                PaternoI = textbox.Text;
        //                break;
        //            case "MaternoBuscar":
        //                MaternoI = textbox.Text;
        //                break;
        //            case "NombreBuscar":
        //                NombreI = textbox.Text;
        //                break;
        //        }
        //        BuscarInterno();
        //    }
        //}

        private void ClickVisitaEnter(Object obj)
        {
            if (obj != null)
            {
                var textbox = (System.Windows.Controls.TextBox)obj;
                switch (textbox.Name)
                {
                    case "NoVisita":
                        if (!string.IsNullOrEmpty(textbox.Text))
                            NoV = int.Parse(textbox.Text);
                        break;
                    case "PaternoVisita":
                        PaternoV = textbox.Text;
                        break;
                    case "MaternoVisita":
                        MaternoV = textbox.Text;
                        break;
                    case "NombreVisita":
                        NombreV = textbox.Text;
                        break;
                }
                PopulateBuscarVisita();
                
            }
        }

        private void ClickEmpleadoEnter(Object obj)
        {
            if (obj != null)
            {
                var textbox = (System.Windows.Controls.TextBox)obj;
                switch (textbox.Name)
                {
                    case "NIPEmpleado":
                        NipE = textbox.Text;
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
                PopulateBuscarEmpleado();
            }
        }

        private void ClickProveedorEnter(Object obj)
        {
            if (obj != null)
            {
                var textbox = (System.Windows.Controls.TextBox)obj;
                switch (textbox.Name)
                {
                    case "NIPExterno":
                        NipEx = textbox.Text;
                        break;
                    case "PaternoExterno":
                        PaternoEx = textbox.Text;
                        break;
                    case "MaternoExterno":
                        MaternoEx = textbox.Text;
                        break;
                    case "NombreExterno":
                        NombreEx = textbox.Text;
                        break;
                }
                PopulateBuscarExterno();
            }
        }

        private void LimpiarBusquedaVisitante()
        {
            NoV = null;
            PaternoV = MaternoV = NombreV = string.Empty;
            LstVisitantePop = null;
            LstVisitantePop = null;
        }

        private void LimpiarBusquedaEmpleado()
        {
            NipE = PaternoE = MaternoE = NombreE = string.Empty;
            LstEmpleadoPop = null;
            SelectedEmpleadoPop = null;
        }

        private void LimpiarBusquedaVisitanteExterno()
        {
            NipEx = PaternoEx = MaternoEx = NombreEx = string.Empty;
            LstExternoPop = null;
            SelectedExternoPop = null;
        }

        private void LimpiarBusquedaDecomiso()
        {
            TipoB = 0;
            NombreB = PaternoB = MaternoB = string.Empty;
            ImagenIngresoB = new Imagenes().getImagenPerson();
            var obj = SelectedDecomiso;
            LstDecomisos = null;//new ObservableCollection<cDecomisos>();
            SelectedDecomiso = obj;
        }

        //INTERNO
        //private void LimpiarBusquedaInterno()
        //{
        //    AnioI = null;
        //    FolioI = null;
        //    NombreI = PaternoI = MaternoI = UbicacionInterno = string.Empty;
        //    LstIngreso = null;//new ObservableCollection<INGRESO>();
        //    InternosEmpty = true;
        //    SelectedIngreso = null;
        //}

        //private async Task<List<INGRESO>> SegmentarResultadoBusquedaIngreso(int _Pag = 1)
        //{
        //    try
        //    {
        //        PaginaIngreso = _Pag;
        //        var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO>>(() => new ObservableCollection<INGRESO>((
        //            new cIngreso().ObtenerTodos(/*4*/GlobalVar.gCentro, AnioI, FolioI, NombreB, PaternoB, MaternoB, _Pag))));
        //        if (result.Any())
        //        {
        //            PaginaIngreso++;
        //            SeguirCargandoIngreso = true;
        //        }
        //        else
        //            SeguirCargandoIngreso = false;
        //        return result.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de internos.", ex);
        //        return new List<INGRESO>();
        //    }
        //}

        //private void BuscarInterno()
        //{
        //    try
        //    {
        //        System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
        //        {
        //            //LstIngreso = new ObservableCollection<INGRESO>(new cIngreso().ObtenerTodos(GlobalVar.gCentro, AnioI, FolioI, NombreI, PaternoI, MaternoI));
        //            //InternosEmpty = !(LstIngreso.Count > 0);
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar interno", ex);
        //    }
        //}
        #endregion

        #region Decomiso
        private bool GuardarDecomiso()
        {
            try
            {
                var obj = new DECOMISO();
                obj.ID_CENTRO = GlobalVar.gCentro;
                obj.EVENTO_FEC = FechaEventoD;
                obj.INFORME_FEC = FechaInformeD;
                obj.OFICIO = FolioD;
                obj.ID_TURNO = SelectedTurno;
                obj.ID_GRUPO_TACTICO = SelectedGpoTactico;
                if (SelectedArea != -1)
                {
                    obj.ID_AREA = SelectedArea;
                    obj.ID_EDIFICIO = obj.ID_SECTOR = null;
                    obj.ID_CELDA = null;
                }
                else
                {
                    obj.ID_AREA = null;
                    obj.ID_EDIFICIO = SelectedCelda.ID_EDIFICIO;
                    obj.ID_SECTOR = SelectedCelda.ID_SECTOR;
                    obj.ID_CELDA = SelectedCelda.ID_CELDA;
                }
                obj.ID_CENTRO = GlobalVar.gCentro;
                obj.RESUMEN = ResumenD;
                
                //Informacion Reporte
                obj.OFICIO_SEGURIDAD = IOficioSeguridad; 
                obj.JEFE_TURNO = IJefeTurno;
                obj.COMANDANTE = IComandante;
                obj.OFICIO_COMAN1 = IOficioComandancia1;
                obj.OFICIO_COMAN2 = IOficioComandancia2;

                if (SelectedDecomiso == null)//AGREGAR
                {
                    obj.ID_USUARIO = GlobalVar.gUsr;
                    obj.SYSDATE_FEC = Fechas.GetFechaDateServer;
                    
                    #region Ingreso
                    if (LstInternoInvolucrado != null)
                    {
                        var ingresos = new List<DECOMISO_INGRESO>(LstInternoInvolucrado.Select(w => new DECOMISO_INGRESO()
                        {
                            ID_CENTRO = w.INGRESO.ID_CENTRO,
                            ID_ANIO = w.INGRESO.ID_ANIO,
                            ID_IMPUTADO = w.INGRESO.ID_IMPUTADO,
                            ID_INGRESO = w.INGRESO.ID_INGRESO
                        }));
                        obj.DECOMISO_INGRESO = ingresos;
                    }
                    #endregion

                    #region Persona
                    List<DECOMISO_PERSONA> personas = new List<DECOMISO_PERSONA>();
                    if (LstOficialesACargo != null)
                    {
                        foreach (var o in LstOficialesACargo)
                        {

                            personas.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = o.PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EMPLEADO,
                                OFICIAL_A_CARGO = "S"
                            });
                        }
                    }

                    if (LstVisitaInvolucrada != null)
                    {
                        foreach (var dp in LstVisitaInvolucrada)
                        {
                            personas.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = dp.PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_VISITA,
                                OFICIAL_A_CARGO = "N"
                            });
                        }
                    }

                    if (LstAbogadoInvolucrada != null)
                    {
                        foreach (var a in LstAbogadoInvolucrada)
                        {
                            personas.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = a.PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_ABOGADO,
                                OFICIAL_A_CARGO = "N"
                            });
                        }
                    }

                    if (LstEmpleadoInvolucrado != null)
                    {
                        foreach (var dp in LstEmpleadoInvolucrado)
                        {
                            personas.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = dp.PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EMPLEADO,
                                OFICIAL_A_CARGO = "N"
                            });
                        }
                    }

                    if (LstProveedoresInvolucrados != null)
                    {
                        foreach (var dp in LstProveedoresInvolucrados)
                        {
                            personas.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = dp.PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EXTERNA,
                                OFICIAL_A_CARGO = "N"
                            });
                        }
                    }

                    obj.DECOMISO_PERSONA = personas;
                    #endregion

                    #region Objetos
                    short i = 1;
                    List<DECOMISO_OBJETO> objetos = new List<DECOMISO_OBJETO>();

                    if (LstObjetos != null)
                    {
                        foreach (var o in LstObjetos)
                        {
                            short i2 = 1;
                            //IMAGENES
                            List<DECOMISO_IMAGEN> imagenes = new List<DECOMISO_IMAGEN>();
                            if (o.DECOMISO_IMAGEN != null)
                                foreach (var img in o.DECOMISO_IMAGEN)
                                {
                                    imagenes.Add(new DECOMISO_IMAGEN()
                                    {
                                        //ID_DECOMISO = obj.ID_DECOMISO,
                                        ID_OBJETO_TIPO = o.ID_OBJETO_TIPO,
                                        ID_CONSEC = i,
                                        ID_IMAGEN = i2,
                                        IMAGEN = img.IMAGEN
                                    });
                                    i2++;
                                }

                            objetos.Add(new DECOMISO_OBJETO()
                            {
                                //ID_DECOMISO = obj.ID_DECOMISO,
                                ID_OBJETO_TIPO = o.ID_OBJETO_TIPO,
                                ID_CONSEC = i,
                                DESCR = o.DESCR,
                                CANTIDAD = o.CANTIDAD,
                                COMENTARIO = o.COMENTARIO,
                                ID_FABRICANTE = o.ID_FABRICANTE != -1 ? o.ID_FABRICANTE : null,
                                ID_MODELO = o.ID_MODELO != -1 ? o.ID_MODELO : null,
                                SERIE = o.SERIE,
                                ID_TIPO_DROGA = o.ID_TIPO_DROGA != -1 ? o.ID_TIPO_DROGA : null,
                                ID_UNIDAD_MEDIDA = o.ID_UNIDAD_MEDIDA != 0 ? o.ID_UNIDAD_MEDIDA : null,
                                DOSIS = o.DOSIS,
                                ENVOLTORIOS = o.ENVOLTORIOS,
                                ID_COMPANIA = o.ID_COMPANIA != -1 ? o.ID_COMPANIA : null,
                                TELEFONO = o.TELEFONO.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                                IMEI = o.IMEI,
                                SIM_SERIE = o.SIM_SERIE,
                                CAPACIDAD = o.CAPACIDAD,
                                DECOMISO_IMAGEN = imagenes
                            });
                            i++;
                        }
                    }
                    obj.DECOMISO_OBJETO = objetos;
                    #endregion

                    obj.ID_DECOMISO = new cDecomiso().Insertar(obj);
                    if (obj.ID_DECOMISO > 0)
                    {
                        //SelectedDecomiso = obj;//new cDecomisos() { Decomiso = obj };
                        GenerarNotificacion();//Notificamos
                        SelectedDecomiso = new cDecomiso().Obtener(obj.ID_DECOMISO).FirstOrDefault();
                        return true;
                    }
                }
                else//ACTUALIZAR
                {
                    obj.ID_USUARIO = SelectedDecomiso.ID_USUARIO;
                    obj.SYSDATE_FEC = SelectedDecomiso.SYSDATE_FEC;
                    obj.ID_DECOMISO = SelectedDecomiso.ID_DECOMISO;

                    #region Ingreso
                    var ingresos = new List<DECOMISO_INGRESO>();
                    if (LstInternoInvolucrado != null)
                    {
                        ingresos = new List<DECOMISO_INGRESO>(LstInternoInvolucrado.Select(w => new DECOMISO_INGRESO()
                        {
                            ID_CENTRO = w.INGRESO.ID_CENTRO,
                            ID_ANIO = w.INGRESO.ID_ANIO,
                            ID_IMPUTADO = w.INGRESO.ID_IMPUTADO,
                            ID_INGRESO = w.INGRESO.ID_INGRESO,
                            ID_DECOMISO = obj.ID_DECOMISO
                        }));
                    }
                    #endregion

                    #region Persona
                    List<DECOMISO_PERSONA> personas = new List<DECOMISO_PERSONA>();
                    if (LstOficialesACargo != null)
                    {
                        foreach (var o in LstOficialesACargo)
                        {

                            personas.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = o.PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EMPLEADO,
                                OFICIAL_A_CARGO = "S",
                                ID_DECOMISO = obj.ID_DECOMISO
                            });
                        }
                    }

                    if (LstVisitaInvolucrada != null)
                    {
                        foreach (var dp in LstVisitaInvolucrada)
                        {
                            personas.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = dp.PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_VISITA,
                                OFICIAL_A_CARGO = "N",
                                ID_DECOMISO = obj.ID_DECOMISO
                            });
                        }
                    }

                    if (LstAbogadoInvolucrada != null)
                    {
                        foreach (var a in LstAbogadoInvolucrada)
                        {
                            personas.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = a.PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_ABOGADO,
                                OFICIAL_A_CARGO = "N",
                                ID_DECOMISO = obj.ID_DECOMISO
                            });
                        }
                    }

                    if (LstEmpleadoInvolucrado != null)
                    {
                        foreach (var dp in LstEmpleadoInvolucrado)
                        {
                            personas.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = dp.PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EMPLEADO,
                                OFICIAL_A_CARGO = "N",
                                ID_DECOMISO = obj.ID_DECOMISO
                            });
                        }
                    }

                    if (LstProveedoresInvolucrados != null)
                    {
                        foreach (var dp in LstProveedoresInvolucrados)
                        {
                            personas.Add(new DECOMISO_PERSONA()
                            {
                                ID_PERSONA = dp.PERSONA.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EXTERNA,
                                OFICIAL_A_CARGO = "N",
                                ID_DECOMISO = obj.ID_DECOMISO
                            });
                        }
                    }
                    #endregion

                    #region Objetos
                    short i = 1;
                    List<DECOMISO_OBJETO> objetos = new List<DECOMISO_OBJETO>();

                    if (LstObjetos != null)
                    {
                        foreach (var o in LstObjetos)
                        {
                            short i2 = 1;
                            //IMAGENES
                            List<DECOMISO_IMAGEN> imagenes = new List<DECOMISO_IMAGEN>();
                            if (o.DECOMISO_IMAGEN != null)
                                foreach (var img in o.DECOMISO_IMAGEN)
                                {
                                    imagenes.Add(new DECOMISO_IMAGEN()
                                    {
                                        ID_DECOMISO = obj.ID_DECOMISO,
                                        ID_OBJETO_TIPO = o.ID_OBJETO_TIPO,
                                        ID_CONSEC = i,
                                        ID_IMAGEN = i2,
                                        IMAGEN = img.IMAGEN
                                    });
                                    i2++;
                                }

                            #region Objetos
                                var x = new DECOMISO_OBJETO();
                                x.ID_DECOMISO = obj.ID_DECOMISO;
                                x.ID_OBJETO_TIPO = o.ID_OBJETO_TIPO;
                                x.ID_CONSEC = i;
                                x.DESCR = o.DESCR;
                                x.CANTIDAD = o.CANTIDAD;
                                x.COMENTARIO = o.COMENTARIO;
                                x.ID_FABRICANTE = o.ID_FABRICANTE != -1 ? o.ID_FABRICANTE : null;
                                x.ID_MODELO = o.ID_MODELO != -1 ? o.ID_MODELO : null;
                                x.SERIE = o.SERIE;
                                x.ID_TIPO_DROGA = o.ID_TIPO_DROGA != -1 ? o.ID_TIPO_DROGA : null;
                                x.ID_UNIDAD_MEDIDA = o.ID_UNIDAD_MEDIDA != 0 ? o.ID_UNIDAD_MEDIDA : null;
                                x.DOSIS = o.DOSIS;
                                x.ENVOLTORIOS = o.ENVOLTORIOS;
                                x.ID_COMPANIA = o.ID_COMPANIA != -1 ? o.ID_COMPANIA : null;
                                if (!string.IsNullOrEmpty(o.TELEFONO))
                                    x.TELEFONO = o.TELEFONO.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                                else
                                    x.TELEFONO = string.Empty;
                                x.IMEI = o.IMEI;
                                x.SIM_SERIE = o.SIM_SERIE;
                                x.CAPACIDAD = o.CAPACIDAD;
                                x.DECOMISO_IMAGEN = imagenes;
                                objetos.Add(x);
                            #endregion
                            /*objetos.Add(new DECOMISO_OBJETO()
                            {
                                ID_DECOMISO = obj.ID_DECOMISO,
                                ID_OBJETO_TIPO = o.ID_OBJETO_TIPO,
                                ID_CONSEC = i,
                                DESCR = o.DESCR,
                                CANTIDAD = o.CANTIDAD,
                                COMENTARIO = o.COMENTARIO,
                                ID_FABRICANTE = o.ID_FABRICANTE != -1 ? o.ID_FABRICANTE : null,
                                ID_MODELO = o.ID_MODELO != -1 ? o.ID_MODELO : null,
                                SERIE = o.SERIE,
                                ID_TIPO_DROGA = o.ID_TIPO_DROGA != -1 ? o.ID_TIPO_DROGA : null,
                                ID_UNIDAD_MEDIDA = o.ID_UNIDAD_MEDIDA != 0 ? o.ID_UNIDAD_MEDIDA : null,
                                DOSIS = o.DOSIS,
                                ENVOLTORIOS = o.ENVOLTORIOS,
                                ID_COMPANIA = o.ID_COMPANIA != -1 ? o.ID_COMPANIA : null,
                                TELEFONO = o.TELEFONO.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                                IMEI = o.IMEI,
                                SIM_SERIE = o.SIM_SERIE,
                                CAPACIDAD = o.CAPACIDAD,
                                DECOMISO_IMAGEN = imagenes
                            });*/
                            i++;
                        }
                    }
                    #endregion

                    if (new cDecomiso().Actualizar(obj, ingresos, personas, objetos))
                    {
                        SelectedDecomiso = new cDecomiso().Obtener(SelectedDecomiso.ID_DECOMISO).FirstOrDefault();
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el decomiso", ex);
            }
            return false;
        }

        private void ObtenerDecomiso()
        {
            if (SelectedDecomiso != null)
            {
                FechaEventoD = SelectedDecomiso.EVENTO_FEC;
                FechaInformeD = SelectedDecomiso.INFORME_FEC;
                FolioD = SelectedDecomiso.OFICIO;
                SelectedGpoTactico = SelectedDecomiso.ID_GRUPO_TACTICO;
                SelectedTurno = SelectedDecomiso.ID_TURNO;
                ResumenD = SelectedDecomiso.RESUMEN;
           
                if (SelectedDecomiso.ID_AREA == null)//BUSCARMOS CELDA
                {

                    Celda = string.Format("{0}-{1}-{2}", SelectedDecomiso.CELDA.SECTOR.EDIFICIO.DESCR, SelectedDecomiso.CELDA.SECTOR.DESCR, SelectedDecomiso.CELDA.ID_CELDA);
                    Celda = Celda.Replace(" ", string.Empty);
                    SelectedArea = -1;
                    SelectedCelda = SelectedDecomiso.CELDA;
                }
                else
                {
                    SelectedArea = SelectedDecomiso.ID_AREA;
                }
                //INTERNOS
                LstInternoInvolucrado = new ObservableCollection<DECOMISO_INGRESO>(SelectedDecomiso.DECOMISO_INGRESO);
                InternoInvolucradoVisible = LstInternoInvolucrado.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                if (SelectedDecomiso.DECOMISO_PERSONA != null)
                {
                    //OFICIAL A CARGO
                    LstOficialesACargo = new ObservableCollection<DECOMISO_PERSONA>(SelectedDecomiso.DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_EMPLEADO && w.OFICIAL_A_CARGO == "S"));
                    OficialInvolucradoVisible = LstOficialesACargo.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    //VISITANTES
                    LstVisitaInvolucrada = new ObservableCollection<DECOMISO_PERSONA>(SelectedDecomiso.DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_VISITA));
                    VisitaInvolucradoVisible = LstVisitaInvolucrada.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    //Abogados
                    LstAbogadoInvolucrada = new ObservableCollection<DECOMISO_PERSONA>(SelectedDecomiso.DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_ABOGADO));
                    AbogadoInvolucradoVisible = LstAbogadoInvolucrada.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    //EMPLEADOS
                    LstEmpleadoInvolucrado = new ObservableCollection<DECOMISO_PERSONA>(SelectedDecomiso.DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_EMPLEADO && w.OFICIAL_A_CARGO == "N"));
                    EmpleadoInvolucradoVisible = LstEmpleadoInvolucrado.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    //PROVEEDORES
                    LstProveedoresInvolucrados = new ObservableCollection<DECOMISO_PERSONA>(SelectedDecomiso.DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_EXTERNA));
                    ProveedorInvolucradoVisible = LstProveedoresInvolucrados.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }
                //OBJETOS
                LstObjetos = new ObservableCollection<DECOMISO_OBJETO>(SelectedDecomiso.DECOMISO_OBJETO.OrderBy(w => w.ID_DECOMISO));
                ObjetoVisible = LstObjetos.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

                //DECOMISOS impresion
                IOficioSeguridad = SelectedDecomiso.OFICIO_SEGURIDAD;
                IJefeTurno = SelectedDecomiso.JEFE_TURNO;
                IComandante = SelectedDecomiso.COMANDANTE;
                IOficioComandancia1 = SelectedDecomiso.OFICIO_COMAN1;
                IOficioComandancia2 = SelectedDecomiso.OFICIO_COMAN2;
            }
        }

        private async Task<List<DECOMISO>> SegmentarDecomisoBusqueda(int _Pag = 1)
        {
            try
            {
                PaginaDecomiso = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<DECOMISO>>(() =>
                             new ObservableCollection<DECOMISO>(new SSP.Controlador.Catalogo.Justicia.cDecomiso().ObtenerDecomisoEvento(TipoB.Value,NombreB,PaternoB,MaternoB,string.Empty,_Pag)));
                if (result.Any())
                {
                    PaginaDecomiso++;
                    SeguirCargandoDecomisos = true;
                }
                else
                    SeguirCargandoDecomisos = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar internos.", ex);
                return new List<DECOMISO>();
            }
        }
        #endregion

        #region Oficiales a Cargo
        private void LimpiarOficial()
        {
            ONoControl = OPaterno = OMaterno = ONombre = string.Empty;
            LstOficialPop = null;
            SelectedOficialPop = null;
        }

        private void LimpiarOficialPop()
        {
            ONoControl = OPaterno = OMaterno = ONombre = string.Empty;
            LstOficialPop = null; // new ObservableCollection<EMPLEADO>();
            SelectedOficialPop = null;
            OficialEmpty = true;
        }

        //private void PopulateBuscarOficial()
        //{
        //    try
        //    {
        //        System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
        //        {
        //            int? x = !string.IsNullOrEmpty(ONoControl) ? int.Parse(ONoControl) : 0;
        //            LstOficialPop = new ObservableCollection<EMPLEADO>(new cEmpleado().ObtenerTodos(GlobalVar.gCentro, x > 0 ? x : null, OPaterno, OMaterno, ONombre, true));
        //            OficialEmpty = LstOficialPop.Count > 0 ? false : true;
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al  buscar oficial", ex);
        //    }
        //}

        private async void PopulateBuscarOficial()
        {
            try
            {
                PaginaEmpleado = 1;
                SeguirCargandoEmpleado = true;
                SelectedEmpleadoPop = null;
                LstOficialPop = new RangeEnabledObservableCollection<EMPLEADO>();
                LstOficialPop.InsertRange(await SegmentarEmpleadoBusqueda());
                OficialEmpty = LstOficialPop.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al  buscar oficial", ex);
            }
        }

        private async Task<List<EMPLEADO>> SegmentarEmpleadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(OPaterno) && string.IsNullOrEmpty(OMaterno) && string.IsNullOrEmpty(ONombre) && string.IsNullOrEmpty(ONoControl))
                    return new List<EMPLEADO>();
                PaginaEmpleado = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EMPLEADO>>(() =>
                        new ObservableCollection<EMPLEADO>(new cEmpleado().ObtenerTodos(GlobalVar.gCentro, !string.IsNullOrEmpty(ONoControl) ? (int?)int.Parse(ONoControl) : null, OPaterno, OMaterno, ONombre,true, _Pag)));

                if (result.Any())
                {
                    PaginaEmpleado++;
                    SeguirCargandoEmpleado = true;
                }
                else
                    SeguirCargandoEmpleado = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar.", ex);
                return new List<EMPLEADO>();
            }
        }
        private async void EliminarOficialACargo()
        {
            if (SelectedOficialACargo != null)
            {
                if (LstOficialesACargo != null)
                {
                    var respuesta = await new Dialogos().ConfirmarEliminar("Validación", "¿Desea eliminar el oficial a cargo seleccionado?");
                    if (respuesta == 1)
                        LstOficialesACargo.Remove(SelectedOficialACargo);
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un oficial a cargo");
        }
        #endregion

        #region Internos Involucrados
        private void ObtenerTodosInternosInvolucrados()
        {
            if (SelectedDecomiso != null)
                LstInternoInvolucrado = new ObservableCollection<DECOMISO_INGRESO>(SelectedDecomiso.DECOMISO_INGRESO);
        }

        private void ObtenerInternoInvolucrado()
        {
        }

        //private bool AgregarInternoInvolucrado()
        //{
        //    try
        //    {
        //        if (SelectedIngreso == null)
        //        {
        //            new Dialogos().ConfirmacionDialogo("Validación!", "Favor de seleccionar un interno");
        //            return false;
        //        }
        //        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
        //        foreach (var item in EstatusInactivos)
        //        {
        //            if (SelectedIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
        //            {
        //                new Dialogos().ConfirmacionDialogo("Validación!", "El ingreso seleccionado no esta activo.");
        //                return false;
        //            }
        //        }
        //        if (SelectedIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
        //        {
        //            new Dialogos().ConfirmacionDialogo("Validación!", "El ingreso seleccionado no pertenece a este centro.");
        //            return false;
        //        }
        //        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
        //        if (SelectedIngreso.TRASLADO_DETALLE.Any(w => (w.ID_ESTATUS != "CA" ? w.TRASLADO.ORIGEN_TIPO != "F" : false) && w.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
        //        {
        //            new Dialogos().ConfirmacionDialogo("Validación!", "El interno [" + SelectedIngreso.ID_ANIO.ToString() + "/" +
        //                    SelectedIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
        //            return false;
        //        }
        //        if (LstInternoInvolucrado == null)
        //            LstInternoInvolucrado = new ObservableCollection<DECOMISO_INGRESO>();
        //        LstInternoInvolucrado.Add(new DECOMISO_INGRESO()
        //        {
        //            ID_DECOMISO = SelectedDecomiso != null ? SelectedDecomiso.ID_DECOMISO : 0,
        //            ID_CENTRO = SelectedIngreso.ID_CENTRO,
        //            ID_ANIO = SelectedIngreso.ID_ANIO,
        //            ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
        //            ID_INGRESO = SelectedIngreso.ID_INGRESO,
        //            INGRESO = SelectedIngreso
        //        });
        //        InternoInvolucradoVisible = LstInternoInvolucrado.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar un interno involucrado", ex);
        //    }
        //    return false;
        //}

        private bool GuardarInternoInvolucrado(int Id)
        {
            List<DECOMISO_INGRESO> list = new List<DECOMISO_INGRESO>();
            if (LstInternoInvolucrado != null)
                foreach (var obj in LstInternoInvolucrado)
                {
                    list.Add(new DECOMISO_INGRESO()
                    {
                        ID_DECOMISO = Id,
                        ID_CENTRO = obj.ID_CENTRO,
                        ID_ANIO = obj.ID_ANIO,
                        ID_IMPUTADO = obj.ID_IMPUTADO,
                        ID_INGRESO = obj.ID_INGRESO
                    });
                }
            if (new cDecomisoIngreso().Insertar(list, Id))
                return true;
            return false;
        }
        
        private async void EliminarInternoInvolucrado()
        {
            if (SelectedInternoInvolucrado != null)
            {
                if (LstInternoInvolucrado != null)
                {
                    var respuesta = await new Dialogos().ConfirmarEliminar("Validación", "¿Desea eliminar el interno seleccionado?");
                    if (respuesta == 1)
                        LstInternoInvolucrado.Remove(SelectedInternoInvolucrado);
                    InternoInvolucradoVisible = LstInternoInvolucrado.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un interno involucrado");
        }
        #endregion

        #region Visitas/Empleados/Proveedores
        private void ObtenerTodoPersona()
        {
            if (SelectedDecomiso != null)
            {
                LstEmpleadoInvolucrado = new ObservableCollection<DECOMISO_PERSONA>(SelectedDecomiso.DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_EMPLEADO));
                LstVisitaInvolucrada = new ObservableCollection<DECOMISO_PERSONA>(SelectedDecomiso.DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_VISITA));
                LstAbogadoInvolucrada = new ObservableCollection<DECOMISO_PERSONA>(SelectedDecomiso.DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_ABOGADO));
                LstExterno = new ObservableCollection<DECOMISO_PERSONA>(SelectedDecomiso.DECOMISO_PERSONA.Where(w => w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_EXTERNA));
            }
        }

        private bool AgregarPersona(short tipo)
        {
            try
            {
                switch (tipo)
                {
                    case (short)enumTipoPersona.PERSONA_EMPLEADO:

                        if (EsOficial)
                        {
                            if (SelectedOficialPop != null)
                            {
                                if (LstOficialesACargo == null)
                                    LstOficialesACargo = new ObservableCollection<DECOMISO_PERSONA>();

                                if (LstOficialesACargo.Count(w => w.ID_PERSONA == SelectedOficialPop.ID_EMPLEADO) > 0)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "El oficial ya se encuentra registrado.");
                                    return false;
                                }
                                LstOficialesACargo.Add(new DECOMISO_PERSONA()
                                {
                                    ID_DECOMISO = SelectedDecomiso != null ? SelectedDecomiso.ID_DECOMISO : 0,
                                    ID_PERSONA = SelectedOficialPop.PERSONA.ID_PERSONA,
                                    ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EMPLEADO,
                                    PERSONA = SelectedOficialPop.PERSONA
                                });
                                OficialInvolucradoVisible = LstOficialesACargo.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                            }
                            else
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un oficial a cargo");
                                return false;
                            }
                        }
                        else
                        {
                            if (SelectedEmpleadoPop != null)
                            {
                                if (LstEmpleadoInvolucrado == null)
                                    LstEmpleadoInvolucrado = new ObservableCollection<DECOMISO_PERSONA>();

                                if (LstEmpleadoInvolucrado.Count(w => w.ID_PERSONA == SelectedEmpleadoPop.ID_PERSONA) > 0)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "El empleado ya se encuentra registrado.");
                                    return false;
                                }
                                LstEmpleadoInvolucrado.Add(new DECOMISO_PERSONA()
                                {
                                    ID_DECOMISO = SelectedDecomiso != null ? SelectedDecomiso.ID_DECOMISO : 0,
                                    ID_PERSONA = SelectedEmpleadoPop.ID_PERSONA,
                                    ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EMPLEADO,
                                    PERSONA = SelectedEmpleadoPop
                                });
                                EmpleadoInvolucradoVisible = LstEmpleadoInvolucrado.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                            }
                            else
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un empleado");
                                return false;
                            }
                        }

                        break;
                    case (short)enumTipoPersona.PERSONA_VISITA:
                        if (SelectedVisitantePop != null)
                        {
                            if (LstVisitaInvolucrada == null)
                                LstVisitaInvolucrada = new ObservableCollection<DECOMISO_PERSONA>();
                            if (LstVisitaInvolucrada.Count(w => w.ID_PERSONA == SelectedVisitantePop.ID_PERSONA) > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "La visita ya se encuentra registrada.");
                                return false;
                            }
                            LstVisitaInvolucrada.Add(new DECOMISO_PERSONA()
                            {
                                ID_DECOMISO = SelectedDecomiso != null ? SelectedDecomiso.ID_DECOMISO : 0,
                                ID_PERSONA = SelectedVisitantePop.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_VISITA,
                                PERSONA = SelectedVisitantePop.PERSONA
                            });
                            VisitaInvolucradoVisible = LstVisitaInvolucrada.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un visitante");
                            return false;
                        }
                        break;
                    case (short)enumTipoPersona.PERSONA_ABOGADO:
                        if (SelectedVisitantePop != null)
                        {
                            if (LstAbogadoInvolucrada == null)
                                LstAbogadoInvolucrada = new ObservableCollection<DECOMISO_PERSONA>();
                            if (LstAbogadoInvolucrada.Count(w => w.ID_PERSONA == SelectedVisitantePop.ID_PERSONA) > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El abogado ya se encuentra registrado.");
                                return false;
                            }
                            LstAbogadoInvolucrada.Add(new DECOMISO_PERSONA()
                            {
                                ID_DECOMISO = SelectedDecomiso != null ? SelectedDecomiso.ID_DECOMISO : 0,
                                ID_PERSONA = SelectedVisitantePop.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_ABOGADO,
                                PERSONA = SelectedVisitantePop.PERSONA
                            });
                            AbogadoInvolucradoVisible = LstAbogadoInvolucrada.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un abogado");
                            return false;
                        }
                        break;
                    case (short)enumTipoPersona.PERSONA_EXTERNA:
                        if (SelectedExternoPop != null)
                        {
                            if (LstProveedoresInvolucrados == null)
                                LstProveedoresInvolucrados = new ObservableCollection<DECOMISO_PERSONA>();
                            if (LstProveedoresInvolucrados.Count(w => w.ID_PERSONA == SelectedExternoPop.ID_PERSONA) > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "La visita externa ya se encuentra registrada.");
                                return false;
                            }
                            LstProveedoresInvolucrados.Add(new DECOMISO_PERSONA()
                            {
                                ID_DECOMISO = SelectedDecomiso != null ? SelectedDecomiso.ID_DECOMISO : 0,
                                ID_PERSONA = SelectedExternoPop.ID_PERSONA,
                                ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EXTERNA,
                                PERSONA = SelectedExternoPop.PERSONA
                            });
                            ProveedorInvolucradoVisible = LstProveedoresInvolucrados.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proveedor");
                            return false;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar una persona", ex);
            }
            return true;
        }

        private bool GuardarPersona(int Id)
        {
            List<DECOMISO_PERSONA> list = new List<DECOMISO_PERSONA>();
            #region Oficiales a Cargo
            if (LstOficialesACargo != null)
                foreach (var oc in LstOficialesACargo)
                {
                    list.Add(new DECOMISO_PERSONA()
                    {
                        ID_DECOMISO = Id,
                        ID_PERSONA = oc.PERSONA.ID_PERSONA,
                        ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EMPLEADO,
                        OFICIAL_A_CARGO = "S"
                    });
                }
            #endregion

            #region Empleado
            if (LstEmpleadoInvolucrado != null)
                foreach (var ei in LstEmpleadoInvolucrado)
                {
                    list.Add(new DECOMISO_PERSONA()
                    {
                        ID_DECOMISO = Id,
                        ID_PERSONA = ei.PERSONA.ID_PERSONA,
                        ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EMPLEADO,
                        OFICIAL_A_CARGO = "N"
                    });
                }
            #endregion

            #region Visitante
            if (LstVisitaInvolucrada != null)
                foreach (var vi in LstVisitaInvolucrada)
                {
                    list.Add(new DECOMISO_PERSONA()
                    {
                        ID_DECOMISO = Id,
                        ID_PERSONA = vi.PERSONA.ID_PERSONA,
                        ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_VISITA,
                        OFICIAL_A_CARGO = "N"
                    });
                }
            #endregion

            #region Abogado
            if (LstVisitaInvolucrada != null)
                foreach (var a in LstAbogadoInvolucrada)
                {
                    list.Add(new DECOMISO_PERSONA()
                    {
                        ID_DECOMISO = Id,
                        ID_PERSONA = a.PERSONA.ID_PERSONA,
                        ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_ABOGADO,
                        OFICIAL_A_CARGO = "N"
                    });
                }
            #endregion

            #region Visita Externa
            if (LstProveedoresInvolucrados != null)
                foreach (var pi in LstProveedoresInvolucrados)
                {
                    list.Add(new DECOMISO_PERSONA()
                    {
                        ID_DECOMISO = Id,
                        ID_PERSONA = pi.PERSONA.ID_PERSONA,
                        ID_TIPO_PERSONA = (short)enumTipoPersona.PERSONA_EXTERNA,
                        OFICIAL_A_CARGO = "N"
                    });
                }
            #endregion

            if (new cDecomisoPersona().Insertar(list, Id))
                return true;
            return false;
        }

        private void LimpiarEmpleadoPop()
        {
            NipE = PaternoE = MaternoE = NombreE = string.Empty;
            LstEmpleadoPop = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
            EmpleadoEmpty = false;

        }

        private void PopulateBuscarEmpleado()
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(async delegate
                {
                    int? x = !string.IsNullOrEmpty(NipE) ? int.Parse(NipE) : 0;
                    HeaderSortin = true;
                    LstEmpleadoPop = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    LstEmpleadoPop.InsertRange(await SegmentarPersonasBusqueda());
                    EmpleadoEmpty = LstEmpleadoPop.Count > 0 ? false : true;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar empleado", ex);
            }
        }

        private async Task<List<SSP.Servidor.PERSONA>> SegmentarPersonasBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(PaternoE) && string.IsNullOrEmpty(MaternoE) && string.IsNullOrEmpty(NombreE))
                    return new List<SSP.Servidor.PERSONA>();
                Pagina = _Pag;
                //var empleado = short.Parse(Parametro.ID_TIPO_PERSONA_EMPLEADO);
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                        new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodosXEmpleados(NombreE, PaternoE, MaternoE, 0, _Pag, GlobalVar.gCentro)));
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
                                    //Modificacion de modelo, PENDIENTE
                                    LstEmpleadoPop.InsertRange(aux.OrderByDescending(o => o.ABOGADO != null)
                                        .ThenByDescending(t => t.PERSONA_EXTERNO != null).ThenByDescending(t => t.VISITANTE != null));
                                }));
                                HeaderSortin = false;
                                break;
                            case false:
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    //Modificacion de modelo, PENDIENTE
                                    LstEmpleadoPop.InsertRange(aux.OrderByDescending(o => o.VISITANTE == null)
                                        .ThenByDescending(t => t.PERSONA_EXTERNO == null).ThenByDescending(t => t.ABOGADO == null));
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

        private async void EliminarEmpleadoInvolucrado()
        {
            if (SelectedEmpleadoInvolucrado != null)
            {
                if (LstEmpleadoInvolucrado != null)
                {
                    var respuesta = await new Dialogos().ConfirmarEliminar("Validación", "¿Desea eliminar el empleado seleccionado?");
                    if (respuesta == 1)
                    {
                        LstEmpleadoInvolucrado.Remove(SelectedEmpleadoInvolucrado);
                        EmpleadoInvolucradoVisible = LstEmpleadoInvolucrado.Any() ? Visibility.Collapsed : Visibility.Visible;
                    }

                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un empleado involucrado");
        }
        #endregion

        #region Objetos
        private void LimpiarObjeto()
        {
            OUnidadMedida = 0;
            OTipoDroga = OCompania = OFabricante = OModelo = OTipo = -1;
            ODescripcion = OComentario = OSerie = OTelefono = OIMEI = OSIMSerie = OCapacidad = string.Empty;
            OCantidad = null;
            ODosis = OEnvoltorios = null;
            SelectedObjeto = null;
            BtnLimpiarText = "Limpiar";
            LstImagenes = new ObservableCollection<DECOMISO_IMAGEN>();
        }
        private void ObtenerTodoObjeto()
        {
            LstObjetos = new ObservableCollection<DECOMISO_OBJETO>(SelectedDecomiso.DECOMISO_OBJETO);
        }
        private void ObtenerObjeto()
        {
            if (SelectedObjeto != null)
            {
                var obj = SelectedObjeto;
                OTipo = SelectedObjeto.ID_OBJETO_TIPO;
                SelectedObjeto = obj;
                ODescripcion = SelectedObjeto.DESCR;
                OCantidad = SelectedObjeto.CANTIDAD;
                OComentario = SelectedObjeto.COMENTARIO;
                OFabricante = SelectedObjeto.ID_FABRICANTE != null ? SelectedObjeto.ID_FABRICANTE : -1;
                OModelo = SelectedObjeto.ID_MODELO != null ? SelectedObjeto.ID_MODELO : -1;
                OSerie = SelectedObjeto.SERIE;
                OTipoDroga = SelectedObjeto.ID_TIPO_DROGA != null ? SelectedObjeto.ID_TIPO_DROGA : -1;
                OUnidadMedida = SelectedObjeto.ID_UNIDAD_MEDIDA != null ? SelectedObjeto.ID_UNIDAD_MEDIDA : -1;
                ODosis = SelectedObjeto.DOSIS;
                OEnvoltorios = SelectedObjeto.ENVOLTORIOS;
                OCompania = SelectedObjeto.ID_COMPANIA;
                OTelefono = SelectedObjeto.TELEFONO;
                OIMEI = SelectedObjeto.IMEI;
                OSIMSerie = SelectedObjeto.SIM_SERIE;
                OCapacidad = SelectedObjeto.CAPACIDAD;
                //IMAGENES
                if (SelectedObjeto.DECOMISO_IMAGEN != null)
                    LstImagenes = new ObservableCollection<DECOMISO_IMAGEN>(SelectedObjeto.DECOMISO_IMAGEN);
                else
                    LstImagenes = new ObservableCollection<DECOMISO_IMAGEN>();
                ImagenVisible = LstImagenes.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        private void AgregarObjeto()
        {
            if (OTipo == -1)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar tipo de objeto");
                return;
            }
            else
            {
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios");
                    return;
                }
            }
            if (LstObjetos == null)
                LstObjetos = new ObservableCollection<DECOMISO_OBJETO>();

            if (SelectedObjeto == null)
            {
                var obj = new DECOMISO_OBJETO();
                obj.ID_DECOMISO = SelectedDecomiso == null ? 0 : SelectedDecomiso.ID_DECOMISO;

                obj.ID_OBJETO_TIPO = OTipo.Value;
                obj.OBJETO_TIPO = SelectedObjetoTipo;
                obj.DESCR = ODescripcion;
                obj.CANTIDAD = OCantidad == null ? 1 : OCantidad;
                obj.COMENTARIO = OComentario;
                obj.ID_FABRICANTE = OFabricante;
                obj.ID_MODELO = OModelo;
                obj.SERIE = OSerie;
                obj.ID_TIPO_DROGA = OTipoDroga;
                obj.ID_UNIDAD_MEDIDA = OUnidadMedida;
                obj.DOSIS = ODosis;
                obj.ENVOLTORIOS = OEnvoltorios;
                obj.ID_COMPANIA = OCompania;
                obj.TELEFONO = OTelefono;
                obj.IMEI = OIMEI;
                obj.SIM_SERIE = OSIMSerie;
                obj.CAPACIDAD = OCapacidad;

                //Imagenes
                obj.DECOMISO_IMAGEN = LstImagenes;

                LstObjetos.Add(obj);
                ObjetoVisible = LstObjetos.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                SelectedObjeto.ID_OBJETO_TIPO = OTipo.Value;
                SelectedObjeto.OBJETO_TIPO = SelectedObjetoTipo;
                SelectedObjeto.DESCR = ODescripcion;
                SelectedObjeto.CANTIDAD = OCantidad == null ? 1 : OCantidad;
                SelectedObjeto.COMENTARIO = OComentario;
                SelectedObjeto.ID_FABRICANTE = OFabricante;
                SelectedObjeto.ID_MODELO = OModelo;
                SelectedObjeto.SERIE = OSerie;
                SelectedObjeto.ID_TIPO_DROGA = OTipoDroga;
                SelectedObjeto.ID_UNIDAD_MEDIDA = OUnidadMedida;
                SelectedObjeto.DOSIS = ODosis;
                SelectedObjeto.ENVOLTORIOS = OEnvoltorios;
                SelectedObjeto.ID_COMPANIA = OCompania;
                SelectedObjeto.TELEFONO = OTelefono;
                SelectedObjeto.IMEI = OIMEI;
                SelectedObjeto.SIM_SERIE = OSIMSerie;
                SelectedObjeto.CAPACIDAD = OCapacidad;
                //Imagenes
                SelectedObjeto.DECOMISO_IMAGEN = LstImagenes;
                LstObjetos = new ObservableCollection<DECOMISO_OBJETO>(LstObjetos);
            }
            LimpiarObjeto();
            OTipo = -1;
        }
        private bool GuardarObjeto(int Id)
        {
            try
            {
                short i = 1;
                List<DECOMISO_OBJETO> objetos = new List<DECOMISO_OBJETO>();
                if (LstObjetos != null)
                    foreach (var o in LstObjetos)
                    {
                        short i2 = 1;
                        //IMAGENES
                        List<DECOMISO_IMAGEN> imagenes = new List<DECOMISO_IMAGEN>();
                        foreach (var img in o.DECOMISO_IMAGEN)
                        {
                            imagenes.Add(new DECOMISO_IMAGEN()
                            {
                                ID_DECOMISO = Id,
                                ID_OBJETO_TIPO = o.ID_OBJETO_TIPO,
                                ID_CONSEC = i,
                                ID_IMAGEN = i2,
                                IMAGEN = img.IMAGEN
                            });
                            i2++;
                        }

                        objetos.Add(new DECOMISO_OBJETO()
                        {
                            ID_DECOMISO = Id,
                            ID_OBJETO_TIPO = o.ID_OBJETO_TIPO,
                            ID_CONSEC = i,
                            DESCR = o.DESCR,
                            CANTIDAD = o.CANTIDAD != null ? o.CANTIDAD : 1,
                            COMENTARIO = o.COMENTARIO,
                            ID_FABRICANTE = o.ID_FABRICANTE != -1 ? o.ID_FABRICANTE : null,
                            ID_MODELO = o.ID_MODELO != -1 ? o.ID_MODELO : null,
                            SERIE = o.SERIE,
                            ID_TIPO_DROGA = o.ID_TIPO_DROGA != -1 ? o.ID_TIPO_DROGA : null,
                            ID_UNIDAD_MEDIDA = o.ID_UNIDAD_MEDIDA != -1 ? o.ID_UNIDAD_MEDIDA : null,
                            DOSIS = o.DOSIS,
                            ENVOLTORIOS = o.ENVOLTORIOS,
                            ID_COMPANIA = o.ID_COMPANIA != -1 ? o.ID_COMPANIA : null,
                            TELEFONO = o.TELEFONO,
                            IMEI = o.IMEI,
                            SIM_SERIE = o.SIM_SERIE,
                            CAPACIDAD = o.CAPACIDAD,
                            DECOMISO_IMAGEN = imagenes
                        });
                        i++;
                    }

                if (new cDecomisoObjeto().Insertar(objetos, Id))
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el objecto", ex);
            }
            return false;
        }
        private async void EliminarObjeto()
        {
            if (SelectedObjeto != null)
            {
                if (LstObjetos != null)
                {
                    var respuesta = await new Dialogos().ConfirmarEliminar("Validación", "¿Desea eliminar el objeto seleccionado?");
                    if (respuesta == 1)
                        LstObjetos.Remove(SelectedObjeto);
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un objeto");
        }
        #endregion

        #region Imagenes
        private bool EliminarImagen()
        {
            if (LstImagenes != null)
            {
                if (!LstImagenes.Remove(SelectedImagen))
                    return false;
            }
            return true;
        }
        #endregion

        #region Notificacion
        private void GenerarNotificacion()
        {
            try
            {
                if (SelectedDecomiso != null)
                {
                    var mt = new cTipoMensaje().Obtener((short)enumMensajeTipo.DECOMISO).FirstOrDefault();
                    if (mt != null)
                    {
                        var obj = new MENSAJE();
                        obj.ID_MEN_TIPO = mt.ID_MEN_TIPO;
                        obj.ENCABEZADO = mt.ENCABEZADO;
                        obj.CONTENIDO = ResumenD;
                        obj.REGISTRO_FEC = Fechas.GetFechaDateServer;
                        obj.ID_CENTRO = GlobalVar.gCentro;
                        new cMensaje().Insertar(obj);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "No se envio notificación");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar notificación", ex);
            }

        }
        #endregion

        #region Impresion
        private bool GuardarImpresionDecomiso()
        {
            if (!FParteInformativo)
                return true;
            if (SelectedDecomiso != null)
            {
                if (!base.HasErrors)
                {
                    var obj = new DECOMISO();
                    #region informacion
                    obj.ID_DECOMISO = SelectedDecomiso.ID_DECOMISO;
                    #endregion
                    obj.OFICIO_SEGURIDAD = IOficioSeguridad;
                    obj.JEFE_TURNO = IJefeTurno;
                    obj.COMANDANTE = IComandante;
                    obj.OFICIO_COMAN1 = IOficioComandancia1;
                    obj.OFICIO_COMAN2 = IOficioComandancia2;
                    if (new cDecomiso().ActualizarParteInformativo(obj))
                        return true;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios");
            }
            return false;
        }

        private void ImpresionFormatoDecomiso()
        {
            var view = new ReportesView();
            view.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

            var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
            var reporte = new List<cReporte>();
            reporte.Add(new cReporte()
            {
                Logo1 = Parametro.LOGO_ESTADO,
                Encabezado1 = Parametro.ENCABEZADO1,
                Encabezado2 = Parametro.ENCABEZADO2,
                Encabezado3 = centro.DESCR.ToUpper()
            });

            var lst = new List<cReporteDecomiso>();
            var r = new cReporteDecomiso();
            r.LogoBC = Parametro.LOGO_ESTADO;
            if (FComandante)
            {
                //var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                if (centro != null)
                {
                    r.Centro = centro.DESCR.Trim();
                    r.MunicipioFecha = string.Format("{0},{1} a {2}", centro.MUNICIPIO.MUNICIPIO1.Trim(), centro.MUNICIPIO.ENTIDAD.DESCR.Trim(), Fechas.fechaLetra(Fechas.GetFechaDateServer, false));
                    r.DirigidoA = IComandante;//centro.DIRECTOR.Trim();
                    r.DirigidoPuesto = "COMANDANTE";//string.Format("DIRECTOR DEL {0}", centro.DESCR.Trim());
                }

                r.Folio = string.Format("OFICIO No.{0}", IOficioSeguridad);
                //decomiso
                r.DecomisoFecha = string.Format("{0:dd/MM/yyyy}", FechaEventoD);
                r.DecomisoHora = string.Format("{0:hh:mm tt}", FechaEventoD);
                r.DecmosioTurno = SelectedDecomiso.TURNO.DESCR.Trim();
                r.DecomisoGpoTactico = SelectedDecomiso.GRUPO_TACTICO.DESCR.Trim();
                if (SelectedDecomiso.AREA == null)
                    r.DecomisoLugar = Celda;
                else
                    r.DecomisoLugar = SelectedDecomiso.AREA.DESCR.Trim();
                r.DecomisoResumen = ResumenD.Trim();
                //oficiales
                var lstO = new List<cDecomisoOficial>();
                if (lstOficialesACargo != null)
                foreach (var o in lstOficialesACargo)
                {
                    lstO.Add(new cDecomisoOficial() { OficialACargo = string.Format("{0} {1} {2}", string.IsNullOrEmpty(o.PERSONA.NOMBRE) ? string.Empty : o.PERSONA.NOMBRE.Trim(), string.IsNullOrEmpty(o.PERSONA.PATERNO) ? string.Empty : o.PERSONA.PATERNO.Trim(), string.IsNullOrEmpty(o.PERSONA.MATERNO) ? string.Empty : o.PERSONA.MATERNO.Trim()) });
                }
                //internos
                var lstI = new List<cDecomisoInterno>();
                if (LstInternoInvolucrado != null)
                foreach (var o in LstInternoInvolucrado)
                {
                    var ubicacion = string.Format("{0}-{1}-{2}-{3}", o.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                                   o.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                                   o.INGRESO.CAMA.CELDA.ID_CELDA.Trim(),
                                                   o.INGRESO.ID_UB_CAMA);
                    ubicacion = Celda.Replace(" ", string.Empty);

                    byte[] img;
                    if (o.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        img = o.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        img = new Imagenes().getImagenPerson();

                    lstI.Add(new cDecomisoInterno() { InternoNombre = string.Format("{0} {1} {2}", string.IsNullOrEmpty(o.INGRESO.IMPUTADO.NOMBRE) ? string.Empty : o.INGRESO.IMPUTADO.NOMBRE.Trim(), string.IsNullOrEmpty(o.INGRESO.IMPUTADO.PATERNO) ? string.Empty : o.INGRESO.IMPUTADO.PATERNO.Trim(), string.IsNullOrEmpty(o.INGRESO.IMPUTADO.MATERNO) ? string.Empty : o.INGRESO.IMPUTADO.MATERNO.Trim()), InternoExpediente = string.Format("{0}/{1}", o.INGRESO.ID_ANIO, o.INGRESO.ID_IMPUTADO), InternoUbicacion = ubicacion, InternoFoto = img });
                }
                //visitante
                var lstV = new List<cDecomisoVisita>();
                if (LstVisitaInvolucrada != null)
                foreach (var o in LstVisitaInvolucrada)
                {
                    var nombre = string.Format("{0} {1} {2} {3}", o.PERSONA.ID_PERSONA, string.IsNullOrEmpty(o.PERSONA.NOMBRE) ? string.Empty : o.PERSONA.NOMBRE.Trim(), string.IsNullOrEmpty(o.PERSONA.PATERNO) ? string.Empty : o.PERSONA.PATERNO.Trim(), string.IsNullOrEmpty(o.PERSONA.MATERNO) ? string.Empty : o.PERSONA.MATERNO.Trim());
                    var registro = string.Format("{0:dd/MM/yyyy}", o.PERSONA.VISITANTE.FEC_ALTA);
                    var sexo = o.PERSONA.SEXO == "M" ? "MASCULINO" : "FEMENINO";
                    var discapacidad = o.PERSONA.ID_TIPO_DISCAPACIDAD != null ? "SI" : "NO";
                    lstV.Add(new cDecomisoVisita() { VisitaNombre = nombre, VisitaRegistro = registro, VisitaSexo = sexo, VisitaTipo = string.Empty, VisitaEstatus = o.PERSONA.VISITANTE.ESTATUS_VISITA.DESCR.Trim(), VisitaDiscapacitado = discapacidad });
                }

                r.Remitente = IJefeTurno;
                r.RemitentePuesto = string.Empty;//"CMTE. DE COMANDANCIA DEL COMPLEJO PENITENCIARIO CERESO MEXICALI";
                view.Report.LocalReport.ReportPath = "Reportes/rDecomisoComandante.rdlc";
                view.Report.LocalReport.DataSources.Clear();
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = lstO;
                view.Report.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = lstI;
                view.Report.LocalReport.DataSources.Add(rds3);

                Microsoft.Reporting.WinForms.ReportDataSource rds4 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = lstV;
                view.Report.LocalReport.DataSources.Add(rds4);

                Microsoft.Reporting.WinForms.ReportDataSource rds5 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = reporte;
                view.Report.LocalReport.DataSources.Add(rds5);
            }
            else
                if (FDirector)
                {
                    if (centro != null)
                    {
                        r.Centro = centro.DESCR.Trim();
                        r.MunicipioFecha = string.Format("{0},{1} a {2}", centro.MUNICIPIO.MUNICIPIO1.Trim(), centro.MUNICIPIO.ENTIDAD.DESCR.Trim(), Fechas.fechaLetra(Fechas.GetFechaDateServer, false));
                        r.DirigidoA = centro.DIRECTOR.Trim();
                        r.DirigidoPuesto = string.Format("DIRECTOR DEL {0}", centro.DESCR.Trim());
                    }

                    r.Folio = string.Format("OFICIO No.{0}", IOficioComandancia1);
                    r.DecomisoResumen = ResumenD.Trim();

                    r.Remitente = IComandante;
                    r.RemitentePuesto = "COMANDANTE";

                    view.Report.LocalReport.ReportPath = "Reportes/rDecomisoDirector.rdlc";
                    view.Report.LocalReport.DataSources.Clear();

                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = reporte;
                    view.Report.LocalReport.DataSources.Add(rds2);
                }
                else
                    if (FParteInformativo)
                    {
                        //var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                        if (centro != null)
                        {
                            r.Centro = centro.DESCR.Trim();
                            r.MunicipioFecha = string.Format("{0},{1} a {2}", centro.MUNICIPIO.MUNICIPIO1.Trim(), centro.MUNICIPIO.ENTIDAD.DESCR.Trim(), Fechas.fechaLetra(Fechas.GetFechaDateServer, false));
                            r.DirigidoA = Parametro.SUB_SECRETARIO_SSP;//centro.DIRECTOR.Trim();
                            r.DirigidoPuesto = "SUBDIRECTOR CENTROS DE REINSERCION SOCIAL Y JEFE DE LA UNIDAD DE INVESTIGACIÓN PENITENCIARIA";//string.Format("DIRECTOR DEL {0}", centro.DESCR.Trim());
                            r.Remitente = centro.DIRECTOR.Trim();
                            r.RemitentePuesto = string.Format("DIRECTOR DEL {0}",centro.DESCR.Trim());
                        
                        }

                        r.Folio = string.Format("OFICIO No.{0}", IOficioComandancia2);
                        r.DecomisoResumen = ResumenD.Trim();

                        view.Report.LocalReport.ReportPath = "Reportes/rDecomisoParteInformativo.rdlc";
                        view.Report.LocalReport.DataSources.Clear();

                        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds2.Name = "DataSet2";
                        rds2.Value = reporte;
                        view.Report.LocalReport.DataSources.Add(rds2);

                        view.Report.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("Coordinador", Parametro.COORD_UNIDAD_INVESTIGACION));

                    }

            lst.Add(r);
            Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            rds1.Name = "DataSet1";
            rds1.Value = lst;
            view.Report.LocalReport.DataSources.Add(rds1);
            view.Report.RefreshReport();
            view.Owner = PopUpsViewModels.MainWindow;
            view.Show();
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.DECOMISO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

        #region Buscar Interno
        private void LimpiarBusquedaInterno() 
        {
            AnioBuscar = FolioBuscar = null;
            NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
            ListExpediente = null;
            SelectExpediente = null;
            EmptyExpedienteVisible = EmptyIngresoVisible = false;
            SelectIngreso = null;
            ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                PaginaInterno = _Pag;
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new List<IMPUTADO>();
            }
        }
        
        #endregion

        #region Buscar Abogado
        private void LimpiarAbogado() {
            TextNombre = TextPaterno = TextMaterno = string.Empty;
            ListPersonas = null;
            SelectPersona = null; 
            EmptyBuscarRelacionInternoVisible = false;
            ImagenPersona = new Imagenes().getImagenPerson();
        }

        private void BuscarPersonasSinCodigo()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    var person = SelectPersona;
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas.InsertRange(await SegmentarAbogadoBusqueda());
                    if (PopUpsViewModels.VisibleBuscarPersonasExistentes == Visibility.Collapsed)
                    {
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

        private async Task<List<SSP.Servidor.PERSONA>> SegmentarAbogadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(TextNombre) && string.IsNullOrEmpty(TextPaterno) && string.IsNullOrEmpty(TextMaterno))
                    return new List<SSP.Servidor.PERSONA>();
                Pagina = _Pag;
                //var empleado = short.Parse(Parametro.ID_TIPO_PERSONA_EMPLEADO);
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                        new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerAbogados(TextNombre, TextPaterno, TextMaterno, _Pag)));
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
