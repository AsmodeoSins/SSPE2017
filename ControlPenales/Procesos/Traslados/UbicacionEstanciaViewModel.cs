using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Threading.Tasks;
using ControlPenales.Clases;
using System.Windows.Controls;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class UbicacionEstanciaViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public UbicacionEstanciaViewModel(){ }
        #endregion

        #region variables


        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set { pInsertar = value; }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set { pEditar = value; }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
                if (value)
                    MenuBuscarEnabled = value;
            }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set { pImprimir = value;}
        }
        #endregion


        #region Menu y Enabled
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        private bool enabledReubicar = false;
        public bool EnabledReubicar
        {
            get { return enabledReubicar; }
            set { enabledReubicar = value; OnPropertyChanged("EnabledReubicar"); }
        }
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        #endregion


        private Visibility mostrarOpcion = Visibility.Collapsed;
        public Visibility MostrarOpcion
        {
            get { return mostrarOpcion; }
            set { mostrarOpcion = value; OnPropertyChanged("MostrarOpcion"); }
        }

        private List<TreeViewList> _TreeListUbicacion;
        public List<TreeViewList> TreeListUbicacion
        {
            get { return _TreeListUbicacion; }
            set
            {
                _TreeListUbicacion = value;
                OnPropertyChanged("TreeListUbicacion");
            }
        }


        private INGRESO_UBICACION_ANT nuevoIngreso;
        public INGRESO_UBICACION_ANT NuevoIngreso
        {
            get { return nuevoIngreso; }
            set
            {
                nuevoIngreso = value;
                OnPropertyChanged("NuevoIngreso");
            }
        }

        private bool ubicacionVisible;
        public bool UbicacionVisible
        {
            get { return ubicacionVisible; }
            set { ubicacionVisible = value; OnPropertyChanged("UbicacionVisible"); }
        }

        private CENTRO ubicaciones;
        public CENTRO Ubicaciones
        {
            get { return ubicaciones; }
            set { ubicaciones = value; OnPropertyChanged("Ubicaciones"); }
        }

        private bool popupBuscarCeldaVisible;
        public bool PopupBuscarCeldaVisible
        {
            get { return popupBuscarCeldaVisible; }
            set { popupBuscarCeldaVisible = value; OnPropertyChanged("PopupBuscarCeldaVisible"); }
        }
        public string Name
        {
            get
            {
                return "ubicacion_estancia";
            }
        }

        private bool emptyIngresoVisible;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private IMPUTADO _Imputado;
        public IMPUTADO Imputado
        {
            get { return _Imputado; }
            set
            {
                _Imputado = value;
                OnPropertyChanged();
            }
        }

        private string textAmpliarDescripcion;
        public string TextAmpliarDescripcion
        {
            get { return textAmpliarDescripcion; }
            set { textAmpliarDescripcion = value; OnPropertyChanged("TextAmpliarDescripcion"); }
        }


        private ObservableCollection<SECTOR> listSector;
        public ObservableCollection<SECTOR> ListSector
        {
            get { return listSector; }
            set { listSector = value; OnPropertyChanged("ListSector"); }
        }

        private List<INGRESO_UBICACION_ANT> listIngresos;
        public List<INGRESO_UBICACION_ANT> ListIngresos
        {
            get { return listIngresos; }
            set { listIngresos = value; OnPropertyChanged("ListIngresos"); }
        }


        private string Resumen { get; set; }
        private INGRESO_UBICACION_ANT ingresoSeleccionado;
        public INGRESO_UBICACION_ANT IngresoSeleccionado
        {
            get { return ingresoSeleccionado; }
            set { ingresoSeleccionado = value; OnPropertyChanged("IngresoSeleccionado"); }
        }

        private ObservableCollection<CELDA> listCelda;
        public ObservableCollection<CELDA> ListCelda
        {
            get { return listCelda; }
            set { listCelda = value; OnPropertyChanged("ListCelda"); }
        }

        private string ubicacionI;
        public string UbicacionI
        {
            get { return ubicacionI; }
            set { ubicacionI = value; OnPropertyValidateChanged("UbicacionI"); }
        }

        private List<TreeViewList> _TreeList;
        public List<TreeViewList> TreeList
        {
            get { return _TreeList; }
            set { _TreeList = value; OnPropertyChanged("TreeList"); }
        }

        private CENTRO centro;
        public CENTRO Centro
        {
            get { return centro; }
            set { centro = value; OnPropertyChanged("Centro"); }
        }

        private CAMA selectedCama;
        public CAMA SelectedCama
        {
            get { return selectedCama; }
            set
            {
                selectedCama = value;

                OnPropertyChanged("SelectedCama");
            }
        }


        private bool SeguirCargandoIngreso { get; set; }
        private byte[] imagenIngresoPop = new Imagenes().getImagenPerson();
        public byte[] ImagenIngresoPop
        {
            get { return imagenIngresoPop; }
            set
            {
                imagenIngresoPop = value;
                OnPropertyChanged("ImagenIngresoPop");
            }
        }

        private string ubicacionInterno;
        public string UbicacionInterno
        {
            get { return ubicacionInterno; }
            set { ubicacionInterno = value; OnPropertyChanged("UbicacionInterno"); }
        }

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }


        private RangeEnabledObservableCollection<IMPUTADO> lstIngreso;
        public RangeEnabledObservableCollection<IMPUTADO> LstIngreso
        {
            get { return lstIngreso; }
            set { lstIngreso = value; OnPropertyChanged("LstIngreso"); }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            var result = new ObservableCollection<IMPUTADO>();
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar la búsqueda", ex);
            }
            return result.ToList();
        }


        private short? anioI;
        public short? AnioI
        {
            get { return anioI; }
            set { anioI = value; OnPropertyChanged("AnioI"); }
        }

        private int? folioI;
        public int? FolioI
        {
            get { return folioI; }
            set { folioI = value; OnPropertyChanged("FolioI"); }
        }

        private string nombreI;
        public string NombreI
        {
            get { return nombreI; }
            set { nombreI = value; OnPropertyChanged("NombreI"); }
        }

        private string paternoI;
        public string PaternoI
        {
            get { return paternoI; }
            set { paternoI = value; OnPropertyChanged("PaternoI"); }
        }

        private string maternoI;
        public string MaternoI
        {
            get { return maternoI; }
            set { maternoI = value; OnPropertyChanged("MaternoI"); }
        }


        private bool internosEmpty;
        public bool InternosEmpty
        {
            get { return internosEmpty; }
            set { internosEmpty = value; OnPropertyChanged("InternosEmpty"); }
        }

        private string tituloModal;
        public string TituloModal
        {
            get { return tituloModal; }
            set { tituloModal = value; OnPropertyChanged("TituloModal"); }
        }

        private string tituloAlias;
        public string TituloAlias
        {
            get { return tituloAlias; }
            set { tituloAlias = value; OnPropertyChanged("TituloAlias"); }
        }

        private string tituloApodo;
        public string TituloApodo
        {
            get { return tituloApodo; }
            set { tituloApodo = value; OnPropertyChanged("TituloApodo"); }
        }

        private string tituloHeaderExpandirDescripcion = "INGRESE EL MOTIVO DE LA REUBICACIÓN ";
        public string TituloHeaderExpandirDescripcion
        {
            get { return tituloHeaderExpandirDescripcion; }
            set { tituloHeaderExpandirDescripcion = value; OnPropertyChanged("TituloHeaderExpandirDescripcion"); }
        }

        private TreeViewList _SelectedItem;
        public TreeViewList SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; }
        }

        #endregion

        #region metodos


        void IPageViewModel.inicializa()
        { }
        
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                #region Eventos desde menu principal

                case "buscar_menu":
                    SelectedIngreso = null;
                    LimpiarCampos();
                    SelectedIngreso = SelectIngreso;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "ayuda_menu":
                    break;

                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;

                case "guardar_menu":
                    if (SelectedIngreso == null)
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un interno.");
                    break;

                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new UbicacionEstanciaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new UbicacionEstanciaViewModel();
                    break;

                #endregion

                case "buscar_visible":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "buscar_interno_pop":
                    LimpiarCampos();
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarImputados);
                    break;

                case "buscar_seleccionar":
                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO.HasValue && SelectIngreso.ID_ESTATUS_ADMINISTRATIVO.Value != 4)
                        {
                            if (SelectIngreso.ID_UB_CENTRO.HasValue && SelectIngreso.ID_UB_CENTRO == GlobalVar.gCentro)
                            {
                                SelectedIngreso = null;
                                ObtenerIngreso();
                                StaticSourcesViewModel.SourceChanged = false;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                    }
                    else
                    {
                        AvisoImputadoVacio();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                        break;

                case "buscar_salir":
                    SelectIngreso = SelectedIngreso;
                    SelectedIngreso = null;
                    if (SelectIngreso == null) ImagenIngreso = new Imagenes().getImagenPerson();//si ya se habia hecho una busqueda satisfactoria, se desa salir. Hay que conservar la imagen de la persona resultante de la busqueda anterior
                    StaticSourcesViewModel.SourceChanged = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "limpiar_busqueda":
                    LimpiarCampos();
                    break;

                case "salir_interno_pop":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "seleccionar_interno_pop":
                    if (SelectedIngreso == null)
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un interno.");
                    else
                    {
                        AsignarInformacion();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                    break;

                case "modificar_ingreso_real":
                    if (!PEditar)
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    else
                    {
                        if (SelectIngreso != null)
                        {
                            TextAmpliarDescripcion = string.Empty;
                            ViewModelArbolUbicacion();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONA_UBICACION);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                    }
                    break;

                case "guardar_ubicacion":
                    break;

                case "cancelar_ubicacion":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONA_UBICACION);
                    break;

                case "guardar_ampliar_descripcion":
                   if (await new Dialogos().ConfirmarEliminar("Advertencia", "Una vez guardado, el registro no podrá ser modificado,¿Desea continuar?") != 1)
                       break;

                   Guardar();
                   break;

                case "cancelar_ampliar_descripcion":
                   PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                   break;

                case "nueva_busqueda":
                   LimpiarBusqueda();
                   break;

                case "addUbicacionEstancia":
                  ViewModelArbolUbicacion();
                  PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONA_UBICACION);
                  break;

                default:
                    //no action (EN BASE A GUIA DE ESTANDARES DE CODIFICACION)
                  break;

            }
        }



        private void AvisoImputadoVacio()
        {
            try
            {
                StaticSourcesViewModel.Mensaje("Aviso", "Debe seleccionar un imputado antes de capturar información.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al dar aviso imputado", ex);
            }
        }

        /// <summary>
        /// Metodo que se asegura de limpiar los datos para una busqueda nueva de imputados
        /// </summary>
        private void LimpiarCampos() 
        {
            try
            {
                AnioI = null;
                FolioI = null;
                NombreI = PaternoI = MaternoI = UbicacionInterno = string.Empty;
                LstIngreso = null;
                LstIngreso = new RangeEnabledObservableCollection<IMPUTADO>();
                ImagenIngresoPop = new Imagenes().getImagenPerson();
            }

            catch (Exception exc)
            {
                throw exc;
            }

            return;
        }


        /// <summary>
        /// Metodo que busca desde la ventana emergente de Busqueda
        /// </summary>
        private async void BuscarImputados()
        {
            try
            {
                LstIngreso = new RangeEnabledObservableCollection<IMPUTADO>();
                LstIngreso.InsertRange(await SegmentarResultadoBusqueda());
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());

                if (ListExpediente != null)
                {
                    if (ListExpediente.Count == 0)
                        EmptyExpedienteVisible = true;
                    else
                        emptyExpedienteVisible = false;
                }

                if (LstIngreso != null)
                {
                    if (LstIngreso.Count == 0)
                        EmptyExpedienteVisible = true;
                    else
                        emptyExpedienteVisible = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar interno", ex);
            }
            return;
        }

        private async void LoadSelectedTreeViewItem(INGRESO Item, IMPUTADO Item2)
        {
            if (Item == null || Item2 == null)
                return;
            TreeList.UnCheckAll();
            var TreeViewItem = await StaticSourcesViewModel.CargarDatosAsync<INGRESO>(() => (new cIngreso()).GetData().Where(w => w.ID_INGRESO == Item.ID_INGRESO && w.ID_IMPUTADO == Item2.ID_IMPUTADO && w.ID_ANIO == Item.ID_ANIO
                                                                && w.ID_CENTRO == Item.ID_CENTRO).FirstOrDefault());

            if (TreeViewItem == null)
                return;
            TreeList.Collapse();
            TreeList.FindElement(TreeViewItem);
            TreeList.Restart().Expand();
        }

        private void AsignarInformacion()
        {
            if (SelectedIngreso == null)
                return;

            try
            {
                ObtenerHistorial();
            }

            catch (Exception exc)
            {
                throw exc;
            }

            return;
        }

        private void ObtenerHistorial()
        {
            try
            {
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ubicación en árbol", ex);
            }

        }

        #endregion

        #region commands
        
        private ICommand buscarInternoClick;
        public ICommand BuscarInternoClick
        {
            get
            {
                return buscarInternoClick ?? (buscarInternoClick = new RelayCommand(ClickEnter));
            }
        }

        public ICommand UbicacionEstanciaLoad
        {
            get { return new DelegateCommand<UbicacionEstanciaView>(LoadUbicacionEstancia); }
        }

        private async void LoadUbicacionEstancia(UbicacionEstanciaView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(InicializaPrivilegios);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las ubicaciones de estancia", ex);
            }

        }

        private void InicializaPrivilegios()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al inicializar pantalla", ex);
            }
        }


        #endregion

        #region[ARBOL UBICACION]
        private void ViewModelArbolUbicacion()
        {
            var UbicacionCentro = (new cCentro()).Obtener(GlobalVar.gCentro).FirstOrDefault();
            if (UbicacionCentro != null)
            {
                if (UbicacionCentro.EDIFICIO != null)
                    foreach (var UbicacionEdificio in UbicacionCentro.EDIFICIO)
                    {
                        foreach (var UbicacionSector in UbicacionEdificio.SECTOR)
                        {
                            foreach (var UbicacionCelda in UbicacionSector.CELDA)
                            {
                                var iCama = new List<CAMA>();
                                foreach (var UbicacionCama in UbicacionCelda.CAMA.Where(w => w.ESTATUS == "S"))
                                {
                                    if (UbicacionCama.ESTATUS == "S")
                                        iCama.Add(UbicacionCama);
                                }

                                UbicacionCelda.CAMA = iCama.OrderBy(w => w.ID_CAMA).ToList();
                            }
                        }
                    }
                Ubicaciones = UbicacionCentro;
            }
        }

        private void SeleccionaUbicacionArbol(Object obj)
        {
            try
            {
                var arbol = (TreeView)obj;
                var x = arbol.SelectedItem;
                var t = x.GetType();
                if (t.BaseType.Name.ToString().Equals("CAMA"))
                {
                    SelectedUbicacion = (CAMA)x;
                    if (new cCama().GetEstatusCama(SelectedUbicacion.ID_CAMA, SelectedUbicacion.ID_CELDA, SelectedUbicacion.ID_SECTOR, SelectedUbicacion.ID_EDIFICIO))
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONA_UBICACION);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    }

                    else
                        (new Dialogos()).ConfirmacionDialogo("Validación", "La cama seleccionada ya se encuentra asignada, selecciona una diferente para continuar.");

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ubicación en árbol", ex);
            }
        }
        #endregion
       
        #region Buscar
        private void LimpiarBusqueda()
        {
            ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
            FolioBuscar = AnioBuscar = null;
            ListExpediente = null;
            ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
        }

        private void ObtenerIngreso()
        {
            try
            {
                //DATOS GENERALES
                AnioD = SelectIngreso.ID_ANIO;
                FolioD = SelectIngreso.ID_IMPUTADO;
                PaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                MaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                NombreD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                IngresosD = SelectIngreso.ID_INGRESO;
                if (SelectIngreso.CAMA != null)
                {
                    UbicacionD = UbicacionI = string.Format("{0}-{1}-{2}-{3}",
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty, SelectIngreso.ID_UB_CAMA);
                }
                else
                    UbicacionD = UbicacionI = string.Empty;

                TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.TIPO_SEGURIDAD.DESCR) ? SelectIngreso.TIPO_SEGURIDAD.DESCR.Trim() : string.Empty : string.Empty;
                FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO ?? new DateTime?();
                ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(SelectIngreso.CLASIFICACION_JURIDICA.DESCR) ? SelectIngreso.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty;
                EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO != null ? !string.IsNullOrEmpty(SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR) ? SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR.Trim() : string.Empty : string.Empty;

                //Historial
                ListIngresos = new List<INGRESO_UBICACION_ANT>(SelectIngreso.INGRESO_UBICACION_ANT.OrderByDescending(x => x.REGISTRO_FEC));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener información del ingreso", ex);
            }
        }
       
        private async void ModelEnter(Object obj)
        {
            try
            {
                #region Validacion de Privilegios
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }

                #endregion
                #region Validacion Tipo de Dato
                if (obj != null)
                {
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
                                    break;
                                case "ApellidoPaternoBuscar":
                                    ApellidoPaternoBuscar = textbox.Text;
                                    PaternoD = ApellidoPaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    ApellidoMaternoBuscar = textbox.Text;
                                    MaternoD = ApellidoMaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "FolioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        FolioBuscar = int.Parse(textbox.Text);
                                    else
                                        FolioBuscar = null;
                                    AnioBuscar = AnioD;
                                    break;
                                case "AnioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        AnioBuscar = int.Parse(textbox.Text);
                                    else
                                        AnioBuscar = null;
                                    FolioBuscar = FolioD;
                                    break;
                            }
                        }
                    }
                }
                #endregion

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

                #region Validacion Nombre y Apellidos
                if (string.IsNullOrEmpty(NombreD))
                    NombreBuscar = string.Empty;
                else
                    NombreBuscar = NombreD;

                if (string.IsNullOrEmpty(PaternoD))
                    ApellidoPaternoBuscar = string.Empty;
                else
                    ApellidoPaternoBuscar = PaternoD;

                if (string.IsNullOrEmpty(MaternoD))
                    ApellidoMaternoBuscar = string.Empty;
                else
                    ApellidoMaternoBuscar = MaternoD;

                #endregion

                if (AnioBuscar != null && FolioBuscar != null)
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        //if (ListExpediente[0].INGRESO.Count > 0)
                        //{
                        //    foreach (var item in ListExpediente[0].INGRESO)
                        //    {
                        //        if (item.ID_ESTATUS_ADMINISTRATIVO.HasValue)
                        //        {
                        //            if (!Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(x => x.Value == item.ID_ESTATUS_ADMINISTRATIVO) && item.ID_UB_CENTRO.HasValue && item.ID_UB_CENTRO == GlobalVar.gCentro)
                        //            {
                        //                SelectExpediente = ListExpediente[0];
                        //                SelectIngreso = item;
                        //                ObtenerIngreso();
                        //                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        //                break;
                        //            }
                        //            else
                        //            {
                        //                SelectExpediente = null;
                        //                SelectIngreso = null;
                        //                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        //                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        //                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        //                break;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            SelectExpediente = null;
                        //            SelectIngreso = null;
                        //            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        //            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    SelectExpediente = null;
                        //    SelectIngreso = null;
                        //    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        //    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        //}

                        if (ListExpediente[0].INGRESO != null && !ListExpediente[0].INGRESO.Any())
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                            return;
                        };

                        foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                        {
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                SelectExpediente = null;
                                SelectIngreso = null;
                                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                                return;
                            };
                        };

                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        };

                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                                ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        };

                        SelectExpediente = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        //SelectedInterno = SelectExpediente;
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        }

                        ObtenerIngreso();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                    {
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                }
                else
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)//Empty row
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                var textbox = obj as System.Windows.Controls.TextBox;
                if (textbox != null)
                {
                    switch (textbox.Name)
                    {
                        case "AnioBuscar":
                            if (!string.IsNullOrWhiteSpace(textbox.Text))
                                AnioI = short.Parse(textbox.Text);
                            else
                                AnioI = new short?();
                            break;
                        case "FolioBuscar":
                            if (!string.IsNullOrWhiteSpace(textbox.Text))
                                FolioI = short.Parse(textbox.Text);
                            else
                                FolioI = new short?();
                            break;
                        case "PaternoBuscar":
                            PaternoI = textbox.Text;
                            break;
                        case "MaternoBuscar":
                            MaternoI = textbox.Text;
                            break;
                        case "NombreBuscar":
                            NombreI = textbox.Text;
                            break;
                    }
                }

                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;
            }

            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            LstIngreso = new RangeEnabledObservableCollection<IMPUTADO>();
            LstIngreso.InsertRange(await SegmentarResultadoBusqueda());
            if (LstIngreso.Count > 0)
                EmptyExpedienteVisible = false;
            else
                EmptyExpedienteVisible = true;
        }

        #endregion

        #region Cambio Ubicacion
        private void Guardar() {

            string DiaVisita = string.Empty;
            StringBuilder Mensaje = new StringBuilder();
            string UbicacionImputadoAnterior = string.Empty;

            try
            {
                if (IngresoSeleccionado == null)
                {
                    var obj = new INGRESO_UBICACION_ANT();
                    obj.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    obj.ID_ANIO = SelectIngreso.ID_ANIO;
                    obj.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    obj.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    obj.ID_CONSEC = 0;
                    obj.ID_UB_CENTRO = SelectedUbicacion.ID_CENTRO;
                    obj.ID_UB_EDIFICIO = SelectedUbicacion.ID_EDIFICIO;
                    obj.ID_UB_SECTOR = SelectedUbicacion.ID_SECTOR;
                    obj.ID_UB_CELDA = SelectedUbicacion.ID_CELDA;
                    obj.ID_UB_CAMA = SelectedUbicacion.ID_CAMA;
                    obj.REGISTRO_FEC = Fechas.GetFechaDateServer;
                    obj.MOTIVO_CAMBIO = TextAmpliarDescripcion;
                    //Ingreso
                    INGRESO ingreso = null;
                    if (SelectedUbicacion != null)
                    {
                        ingreso = new INGRESO();
                        ingreso.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        ingreso.ID_ANIO = SelectIngreso.ID_ANIO;
                        ingreso.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        ingreso.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        ingreso.ID_UB_CENTRO = SelectedUbicacion.ID_CENTRO;
                        ingreso.ID_UB_EDIFICIO = SelectedUbicacion.ID_EDIFICIO;
                        ingreso.ID_UB_SECTOR = SelectedUbicacion.ID_SECTOR;
                        ingreso.ID_UB_CELDA = SelectedUbicacion.ID_CELDA;
                        ingreso.ID_UB_CAMA = SelectedUbicacion.ID_CAMA;
                    }
                    //Cama Nueva
                    CAMA camaNueva = null;
                    if (SelectedUbicacion != null)
                    {
                        camaNueva = new CAMA();
                        camaNueva.ID_CENTRO = SelectedUbicacion.ID_CENTRO;
                        camaNueva.ID_EDIFICIO = SelectedUbicacion.ID_EDIFICIO;
                        camaNueva.ID_SECTOR = SelectedUbicacion.ID_SECTOR;
                        camaNueva.ID_CELDA = SelectedUbicacion.ID_CELDA;
                        camaNueva.ID_CAMA = SelectedUbicacion.ID_CAMA;
                        camaNueva.ESTATUS = "N";
                    }
                    //Cama Vieja
                    CAMA camaVieja = null;
                    if (SelectedUbicacion != null)
                    {
                        camaVieja = new CAMA();
                        camaVieja.ID_CENTRO = SelectIngreso.ID_UB_CENTRO.HasValue ? SelectIngreso.ID_UB_CENTRO.Value : new short();
                        camaVieja.ID_EDIFICIO = SelectIngreso.ID_UB_EDIFICIO.HasValue ? SelectIngreso.ID_UB_EDIFICIO.Value : new short();
                        camaVieja.ID_SECTOR = SelectIngreso.ID_UB_SECTOR.HasValue ? SelectIngreso.ID_UB_SECTOR.Value : new short();
                        camaVieja.ID_CELDA = !string.IsNullOrEmpty(SelectIngreso.ID_UB_CELDA) ? SelectIngreso.ID_UB_CELDA : string.Empty;
                        camaVieja.ID_CAMA = SelectIngreso.ID_UB_CAMA.HasValue ? SelectIngreso.ID_UB_CAMA.Value : new short();
                        camaVieja.ESTATUS = "S";
                    }

                    #region Validacion Traslado
                    if (camaVieja != null && camaVieja.ID_CENTRO.Equals(0)) camaVieja = null;
                    if (camaVieja != null && camaVieja.ID_EDIFICIO.Equals(0)) camaVieja = null;
                    if (camaVieja != null && camaVieja.ID_SECTOR.Equals(0)) camaVieja = null;
                    if (camaVieja != null && string.IsNullOrEmpty(camaVieja.ID_CELDA)) camaVieja = null;
                    if (camaVieja != null && camaVieja.ID_CAMA.Equals(0)) camaVieja = null;
                    #endregion


                    #region Guardar la ubicacion inicial cuando no se le ha generado un historial
                    if (ListIngresos.Count == 0)//No tiene un historial de reubicaciones, es la primera vez que es reubicado en otra celda
                    {
                        var CamaAntesMover = new INGRESO_UBICACION_ANT();
                        CamaAntesMover.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        CamaAntesMover.ID_ANIO = SelectIngreso.ID_ANIO;
                        CamaAntesMover.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        CamaAntesMover.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        CamaAntesMover.ID_CONSEC = 0;
                        CamaAntesMover.ID_UB_CENTRO = SelectIngreso.ID_UB_CENTRO;
                        CamaAntesMover.ID_UB_EDIFICIO = SelectIngreso.ID_UB_EDIFICIO;
                        CamaAntesMover.ID_UB_SECTOR = SelectIngreso.ID_UB_SECTOR;
                        CamaAntesMover.ID_UB_CELDA = SelectIngreso.ID_UB_CELDA;
                        CamaAntesMover.ID_UB_CAMA = SelectIngreso.ID_UB_CAMA;
                        CamaAntesMover.REGISTRO_FEC = Fechas.GetFechaDateServer;
                        CamaAntesMover.MOTIVO_CAMBIO = "INGRESO";
                        new cIngresoUbicacionAnterior().Insert(CamaAntesMover);

                        UbicacionImputadoAnterior = string.Format("{0}-{1}{2}-{3}",//se genera el mensaje de ubicacion anterior
                            SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA != null ? SelectIngreso.CAMA.ID_CAMA.ToString() : string.Empty 
                            );
                    }

                    #endregion

                    if (new cIngresoUbicacionAnterior().Insertar(obj, ingreso, camaNueva, camaVieja))
                    {
                        if (SelectedUbicacion != null)
                        {
                            UbicacionD = UbicacionI = string.Format("{0}-{1}-{2}-{3}",
                                SelectedUbicacion.CELDA != null ? SelectedUbicacion.CELDA.SECTOR != null ? SelectedUbicacion.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectedUbicacion.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectedUbicacion.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                SelectedUbicacion.CELDA != null ? SelectedUbicacion.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectedUbicacion.CELDA.SECTOR.DESCR) ? SelectedUbicacion.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                                !string.IsNullOrEmpty(SelectedUbicacion.ID_CELDA) ? SelectedUbicacion.ID_CELDA.Trim() : string.Empty, SelectedUbicacion.ID_CAMA);
                        }

                        if (ListIngresos != null && ListIngresos.Any())//TOMA EL ULTIMO REGISTRO DEL HISTORIAL QUE SE LE GENERO PARA MANDARLO CON EL MENSAJE COMO UBICACION DE INICIO UNA VEZ QUE SE CARGO LA VENTANA Y ANTES DE QUE SE GENERE EL NUEVO REGISTRO
                        {
                           var UltimoDato = ListIngresos.AsQueryable().LastOrDefault();
                           if (UltimoDato != null)
                               UbicacionImputadoAnterior = string.Format("{0}-{1}{2}-{3}",
                                   UltimoDato.CAMA != null ? UltimoDato.CAMA.CELDA != null ? UltimoDato.CAMA.CELDA.SECTOR != null ? UltimoDato.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(UltimoDato.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? UltimoDato.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                   UltimoDato.CAMA != null ? UltimoDato.CAMA.CELDA != null ? UltimoDato.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(UltimoDato.CAMA.CELDA.SECTOR.DESCR) ? UltimoDato.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                   UltimoDato.CAMA != null ? UltimoDato.CAMA.CELDA != null ? !string.IsNullOrEmpty(UltimoDato.CAMA.CELDA.ID_CELDA) ? UltimoDato.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                                   UltimoDato.CAMA != null ? UltimoDato.CAMA.ID_CAMA.ToString() : string.Empty);
                        }

                        if (camaVieja == null)//si la cama vieja no existe es producto de un traslado, no tiene una ubicacion anterior dentro de este centro.
                            UbicacionImputadoAnterior = string.Empty;

                        //Notificacion Inicio

                        #region Notificacion a Trabajo Social si aplica
                        var visitasPorCentro = new cVisitaEdificio().GetData(x => x.ID_CENTRO == (short)GlobalVar.gCentro && x.ID_EDIFICIO == camaNueva.ID_EDIFICIO && x.ID_SECTOR == camaNueva.ID_SECTOR && !string.IsNullOrEmpty(x.ESTATUS) ? x.ESTATUS.Equals("0") : false);
                        if (visitasPorCentro.Any() && visitasPorCentro.Where(w => w.CELDA_INICIO.Contains(camaNueva.ID_CELDA.Trim()) && w.ID_TIPO_VISITA.HasValue && w.ID_TIPO_VISITA.Value == 2).Any()) //Solo considero las visitas familiares
                            foreach (var item in visitasPorCentro)
                                DiaVisita += string.Format("{0} ,",item.DIA.HasValue ? !string.IsNullOrEmpty(item.VISITA_DIA.DESCR) ? item.VISITA_DIA.DESCR.Trim() : string.Empty : string.Empty);
                        #endregion

                        Mensaje.Append(string.Concat("SE LE NOTIFICA QUE EL INTERNO ", string.Format("{0}/{1} ", SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.ID_ANIO.ToString() : string.Empty, SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.ID_IMPUTADO.ToString() : string.Empty),
                            string.Format("{0} {1} {2}",  SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty), !string.IsNullOrEmpty(UbicacionImputadoAnterior) ? string.Concat(" CON UBICACIÓN  ", UbicacionImputadoAnterior, " ") : string.Empty,
                            " HA SIDO REUBICADO EN LA ESTANCIA ", UbicacionD, !string.IsNullOrEmpty(DiaVisita.ToString()) ? string.Concat(" ASÍ MISMO SE LE INDICA QUE EL DÍA DE VISITA SERÁ EL DÍA: ", DiaVisita) : string.Empty));

                        var msj = new MENSAJE();
                        msj.ID_MEN_TIPO = (short)enumMensajeTipo.CAMBIO_ESTANCIA;
                        msj.ENCABEZADO = "SE HA REALIZADO UN CAMBIO DE ESTANCIA";
                        msj.CONTENIDO = Mensaje.ToString();
                        msj.REGISTRO_FEC = Fechas.GetFechaDateServer;
                        msj.ID_CENTRO = GlobalVar.gCentro;
                        new cMensaje().Insertar(msj);
                        //Notificacion Fin


                        ListIngresos = new cIngresoUbicacionAnterior().ObtenerTodos(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO).ToList();

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                        #region Limpiar Campos 
                        Mensaje.Clear();
                        UbicacionImputadoAnterior = string.Empty;
                        #endregion

                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardó correctamente");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "Ocurrió un error al cambiar la ubicación del interno");
                }
                else {
                    var obj = new INGRESO_UBICACION_ANT();
                    obj.ID_CENTRO = IngresoSeleccionado.ID_CENTRO;
                    obj.ID_ANIO = IngresoSeleccionado.ID_ANIO;
                    obj.ID_IMPUTADO = IngresoSeleccionado.ID_IMPUTADO;
                    obj.ID_INGRESO = IngresoSeleccionado.ID_INGRESO;
                    obj.ID_CONSEC = IngresoSeleccionado.ID_CONSEC;
                    obj.MOTIVO_CAMBIO = TextAmpliarDescripcion;
                    if (new cIngresoUbicacionAnterior().Actualiza(obj))
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                        ListIngresos = new List<INGRESO_UBICACION_ANT>(new cIngresoUbicacionAnterior().ObtenerTodos(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO));
                        IngresoSeleccionado = null;
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardó correctamente");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "Ocurrió un error al cambiar la ubicación del interno");
                    }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener información del ingreso", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.UBICACION_ESTANCIAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = EnabledReubicar = true;
                        if (p.EDITAR == 1)
                            PEditar = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = true;
                        //if (p.IMPRIMIR == 1)
                        //    PImprimir = MenuFichaEnabled = MenuReporteEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
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
