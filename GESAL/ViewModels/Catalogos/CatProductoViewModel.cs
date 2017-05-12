using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.ViewModels;
using System.Collections.ObjectModel;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Almacenes;
using System.Windows.Input;
using MVVMShared.Commands;
using MVVMShared.ViewModels.Interfaces;
using System.ComponentModel;
using GESAL.Clases.Enums;
using MVVMShared.Views;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using SSP.Servidor.ModelosExtendidos;
using GESAL.Models;
using MVVMShared.Manager;
using WpfControls;
using WpfControls.Editors;
using System.Windows;
namespace GESAL.ViewModels
{
    public class CatProductoViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;
        private bool isCargaInformacion = false;
        public CatProductoViewModel(IDialogCoordinator dialogCoordinator, Usuario usuario)
        {
            _dialogCoordinator = dialogCoordinator;
            _usuario = usuario;
        }

        #region Propiedades

        #region Accion del Boton Salvar
        private AccionSalvar realizarAccion;
        public AccionSalvar RealizarAccion
        {
            get { return realizarAccion; }
            set { realizarAccion = value; }
        }
        #endregion

        #region Propiedades Habilitar Acciones

        private bool editarHabilitado = false;
        public bool EditarHabilitado
        {
            get { return editarHabilitado; }
            set { editarHabilitado = value; RaisePropertyChanged("EditarHabilitado"); }
        }

        private bool cancelarHabilitado = false;
        public bool CancelarHabilitado
        {
            get { return cancelarHabilitado; }
            set { cancelarHabilitado = value; RaisePropertyChanged("CancelarHabilitado"); }
        }

       

        private bool eliminarHabilitado = false;
        public bool EliminarHabilitado
        {
            get { return eliminarHabilitado; }
            set { eliminarHabilitado = value; RaisePropertyChanged("EliminarHabilitado"); }
        }

        private bool salvarHabilitado = false;
        public bool SalvarHabilitado
        {
            get { return salvarHabilitado; }
            set { salvarHabilitado = value; RaisePropertyChanged("SalvarHabilitado"); }
        }
        #endregion

        private int? id;
        public int? ID
        {
            get { return id; }
            set { id = value; RaisePropertyChanged("ID"); }
        }

        private string nombre;
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value;
            RaisePropertyChanged("Nombre");
            ValidarNombre();
            }
        }

        public string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; RaisePropertyChanged("Descripcion"); }
        }

        private bool activo;

        public bool Activo
        {
            get { return activo; }
            set { activo = value; RaisePropertyChanged("Activo"); }
        }

        private ObservableCollection<PRODUCTO> productos;
        public ObservableCollection<PRODUCTO> Productos
        {
            get { return productos; }
            set { productos = value; RaisePropertyChanged("Productos"); }
        }

        private bool isSubcategoriaVisible = false;
        public bool IsSubcategoriaVisible
        {
            get { return isSubcategoriaVisible; }
            set { isSubcategoriaVisible = value; RaisePropertyChanged("IsSubcategoriaVisible"); }
        }

        private string filename = null;
        public string Filename
        {
            get { return filename; }
            set { 
                filename = value;
                if (!string.IsNullOrWhiteSpace(value))
                    IsFilenameValid = new FileManagerProvider().isFileMaxSize(value, 500000);//5MB
                RaisePropertyChanged("Filename"); }
        }

        private bool isFilenameValid = true;
        public bool IsFilenameValid
        {
            get { return isFilenameValid; }
            set { isFilenameValid = value; RaisePropertyChanged("IsFilenameValid"); }
        }

        private byte[] imagenData = new FileManagerProvider().fileToByteArray("pack://application:,,,/GESAL;component/Resources/Images/nophoto.png");
        public byte[]ImagenData
        {
            get { return imagenData; }
            set { imagenData = value; RaisePropertyChanged("ImagenData"); }
        }

        private PRODUCTO selectedProducto;

        public PRODUCTO SelectedProducto
        {
            get { return selectedProducto; }
            set { selectedProducto = value;
            RaisePropertyChanged("SelectedProducto");
                if (value!=selectedProductoAutoComplete)
                {
                    selectedProductoAutoComplete = value;
                    RaisePropertyChanged("SelectedProductoAutoComplete");
                }
            if (value != null)
            {
                EliminarHabilitado = true;
                EditarHabilitado = true;

                CancelarHabilitado = true;
            }
            
            }
        }


        //Hay que crear una copia de SelectedProducto para poder ligarla al TextBoxAutoComplete
        //y que de la funcionalidad de que cuando cambiemos en el SelectedItem en el TextBoxAutoComplete
        //cambie el SelectedProducto y se cargue la nueva informacion, pero si el SelectedItem viene nulo
        //como en el caso de que el usuario solo cambie el nombre y no exista en el catalogo de productos
        //el SelectedProducto se mantenga.
        private PRODUCTO selectedProductoAutoComplete;

        public PRODUCTO SelectedProductoAutoComplete
        {
            get { return selectedProductoAutoComplete; }
            set
            {
               selectedProductoAutoComplete = value;
               ValidarNombre();
               RaisePropertyChanged("SelectedProductoAutoComplete");
               if (value!=null && value!=SelectedProducto)
               {
                   SelectedProducto = value;
                   CargarInformacion();
               }
            }
        }

        private ObservableCollection<PRODUCTO_CATEGORIA> producto_Categorias;

        public ObservableCollection<PRODUCTO_CATEGORIA> Producto_Categorias
        {
            get { return producto_Categorias; }
            set { producto_Categorias = value; RaisePropertyChanged("Producto_Categorias"); }
        }

        private PRODUCTO_CATEGORIA selectedProducto_Categoria;

        public PRODUCTO_CATEGORIA SelectedProducto_Categoria
        {
            get { return selectedProducto_Categoria; }
            set
            {
                selectedProducto_Categoria = value;
                RaisePropertyChanged("SelectedProducto_Categoria");
            }
        }


        private ObservableCollection<PRODUCTO_PRESENTACION> producto_Presentaciones;
        public ObservableCollection<PRODUCTO_PRESENTACION> Producto_Presentaciones
        {
            get { return producto_Presentaciones; }
            set { producto_Presentaciones=value; RaisePropertyChanged("Producto_Presentaciones"); }
        }

        private PRODUCTO_PRESENTACION selectedProducto_Presentacion;
        public PRODUCTO_PRESENTACION SelectedProducto_Presentacion
        {
            get { return selectedProducto_Presentacion; }
            set { selectedProducto_Presentacion = value; RaisePropertyChanged("SelectedProducto_Presentacion"); }
        }

        private ObservableCollection<EXT_Almacen_Tipo_Cat> almacen_Tipos_Cat;

        public ObservableCollection<EXT_Almacen_Tipo_Cat> Almacen_Tipos_Cat
        { 
            get { return almacen_Tipos_Cat; }
            set { almacen_Tipos_Cat = value; RaisePropertyChanged("Almacen_Tipos_Cat"); }
        }

        private EXT_Almacen_Tipo_Cat selectedAlmacen_Tipo_Cat=null;
        public EXT_Almacen_Tipo_Cat SelectedAlmacen_Tipo_Cat
        {
            get { return selectedAlmacen_Tipo_Cat; }
            set { selectedAlmacen_Tipo_Cat = value; RaisePropertyChanged("SelectedAlmacen_Tipo_Cat"); }
        }

        private ObservableCollection<ALMACEN_GRUPO> almacen_grupos;

        public ObservableCollection<ALMACEN_GRUPO> Almacen_Grupos
        {
            get { return almacen_grupos; }
            set { almacen_grupos = value; RaisePropertyChanged("Almacen_Grupos"); }
        }

        private ALMACEN_GRUPO selectedAlmacen_Grupo = null;
        public ALMACEN_GRUPO SelectedAlmacen_Grupo
        {
            get { return selectedAlmacen_Grupo; }
            set {
                selectedAlmacen_Grupo = value;
                RaisePropertyChanged("SelectedAlmacen_Grupo");
                if (value != null && value.ID_ALMACEN_GRUPO != "")
                {
                    IsGrupoDefinido = true;
                    CargarAlmacen_Tipos_Cat(value.ID_ALMACEN_GRUPO);
                }
                else
                    IsGrupoDefinido = false;
                    
            }
        }

        private bool isGrupoDefinido = false;
        public bool IsGrupoDefinido
        {
            get { return isGrupoDefinido; }
            set { isGrupoDefinido = value; RaisePropertyChanged("IsGrupoDefinido"); }
        }

        private ObservableCollection<PRODUCTO_UNIDAD_MEDIDA> producto_Unidades_Medida;

        public ObservableCollection<PRODUCTO_UNIDAD_MEDIDA> Producto_Unidades_Medida
        {
            get { return producto_Unidades_Medida; }
            set { producto_Unidades_Medida = value; RaisePropertyChanged("Producto_Unidades_Medida"); }
        }

        private PRODUCTO_UNIDAD_MEDIDA selectedProducto_Unidad_Medida;
        public PRODUCTO_UNIDAD_MEDIDA SelectedProducto_Unidad_Medida
        {
            get { return selectedProducto_Unidad_Medida; }
            set { selectedProducto_Unidad_Medida = value; RaisePropertyChanged("SelectedProducto_Unidad_Medida"); }
        }

        private string busquedaParametro=string.Empty ;

        public string BusquedaParametro
        {
            get { return busquedaParametro; }
            set { busquedaParametro = value; RaisePropertyChanged("BusquedaParametro"); }
        }

        private ObservableCollection<PRODUCTO_SUBCATEGORIA> producto_Subcategorias;
        public ObservableCollection<PRODUCTO_SUBCATEGORIA> Producto_Subcategorias
        {
            get { return producto_Subcategorias; }
            set { producto_Subcategorias = value; RaisePropertyChanged("Producto_Subcategorias"); }
        }
        private int selectedProducto_SubcategoriaValue=-1;
        public int SelectedProducto_SubcategoriaValue
        {
            get { return selectedProducto_SubcategoriaValue; }
            set { selectedProducto_SubcategoriaValue = value; RaisePropertyChanged("SelectedProducto_SubcategoriaValue"); }
        }

        private Estatus_Disponibles lista_estatus = new Estatus_Disponibles();
        public Estatus_Disponibles Lista_Estatus
        {
            get { return lista_estatus; }
            set { lista_estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private Estatus selectedEstatus = null;
        public Estatus SelectedEstatus
        {
            get { return selectedEstatus; }
            set { selectedEstatus = value; RaisePropertyChanged("SelectedEstatus"); }
        }

        public SuggestionProvider PosiblesProductos
        {
            get
            {
                return new SuggestionProvider(filter => {
                    var posiblesProductos = new List<PRODUCTO>();
                    if (!string.IsNullOrEmpty(filter) && filter.Length >= 3 && SelectedAlmacen_Grupo != null && SelectedAlmacen_Grupo.ID_ALMACEN_GRUPO != "" &&
                        (RealizarAccion==AccionSalvar.Salvar || (RealizarAccion==AccionSalvar.Actualizar && !isCargaInformacion)))
                        posiblesProductos = new cProducto().Seleccionar(SelectedAlmacen_Grupo.ID_ALMACEN_GRUPO, "S").Where(w => w.NOMBRE.Contains(filter)).ToList();
                    Nombre = filter;
                    isCargaInformacion = false;
                    return posiblesProductos;
                });
            }
        }

        private bool isProductoNombreCapturado=false;
        public bool IsProductoNombreCapturado
        {
            get { return isProductoNombreCapturado; }
            set { isProductoNombreCapturado = value; RaisePropertyChanged("IsProductoNombreCapturado"); }
        }

        private ObservableCollection<ALMACEN_TIPO_CAT> busquedaAlmacen_Tipos_Cat = new ObservableCollection<ALMACEN_TIPO_CAT>();
        public ObservableCollection<ALMACEN_TIPO_CAT> BusquedaAlmacen_Tipos_Cat
        {
            get { return busquedaAlmacen_Tipos_Cat; }
            set { busquedaAlmacen_Tipos_Cat = value; RaisePropertyChanged("BusquedaAlmacen_Tipos_Cat"); }
        }

        private ALMACEN_TIPO_CAT selectedBusquedaAlmacen_Tipo_Cat = null;
        public ALMACEN_TIPO_CAT SelectedBusquedaAlmacen_Tipo_Cat
        {
            get { return selectedBusquedaAlmacen_Tipo_Cat; }
            set { selectedBusquedaAlmacen_Tipo_Cat = value; RaisePropertyChanged("SelectedBusquedaAlmacen_Tipo_Cat"); }
        }

        #endregion


        #region Metodos

        #region Validacion
        private void setValidationRules(bool isProducto_SubcategoriaRequerido)
        {
            base.ClearRules();
            base.AddRule(() => IsProductoNombreCapturado, () => IsProductoNombreCapturado, "NOMBRE ES REQUERIDO!");
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCION ES REQUERIDA!");
            base.AddRule(() => SelectedProducto_Categoria, () => !(SelectedProducto_Categoria.ID_CATEGORIA == -1), "CATEGORIA ES REQUERIDA");
            base.AddRule(() => SelectedAlmacen_Grupo, () => !(SelectedAlmacen_Grupo.ID_ALMACEN_GRUPO==""), "GRUPO ES REQUERIDO");
            base.AddRule(() => Almacen_Tipos_Cat, () => Almacen_Tipos_Cat.Any(w => w.IS_SELECTED == true), "TIPO ES REQUERIDO");
            base.AddRule(() => SelectedProducto_Presentacion, () => !(SelectedProducto_Presentacion.ID_PRESENTACION == -1), "PRESENTACION ES REQUERIDA");
            base.AddRule(() => SelectedProducto_Unidad_Medida, () => !(SelectedProducto_Unidad_Medida.ID_UNIDAD_MEDIDA == -1), "UNIDAD DE MEDIDA ES REQUERIDA");
            if(!string.IsNullOrWhiteSpace(Filename))
            {
                base.AddRule(() => Filename, () => (IsFilenameValid), "EL TAMAÑO DEL ARCHIVO EXCEDE LOS 5MB");
                RaisePropertyChanged("Filename");
            }
            if (isProducto_SubcategoriaRequerido)
            {
                base.AddRule(() => SelectedProducto_SubcategoriaValue, () => !(SelectedProducto_SubcategoriaValue == -1), "SUBCATEGORIA ES REQUERIDA");
                RaisePropertyChanged("SelectedProducto_SubcategoriaValue");
            }
            RaisePropertyChanged("IsProductoNombreCapturado");
            RaisePropertyChanged("Descripcion");
            RaisePropertyChanged("SelectedProducto_Categoria");
            RaisePropertyChanged("SelectedAlmacen_Grupo");
            RaisePropertyChanged("Almacen_Tipos_Cat");
            RaisePropertyChanged("SelectedProducto_Presentacion");
            RaisePropertyChanged("SelectedProducto_Unidad_Medida");
        }

        public void ValidarNombre()
        {
            if (string.IsNullOrWhiteSpace(Nombre) && SelectedProductoAutoComplete == null)
                IsProductoNombreCapturado = false;
            else
                IsProductoNombreCapturado = true;
        }
        #endregion

        public void CargaCatalogo(short id_almacen_tipo_cat, string estatus, bool isExceptionManaged = false)
        {
            try
            {

                if (id_almacen_tipo_cat == -1)
                    productos = new ObservableCollection<PRODUCTO>(new cProducto().Seleccionar(_usuario.Almacen_Grupo,estatus).ToList());
                else
                    productos = new ObservableCollection<PRODUCTO>(new cProducto().Seleccionar(id_almacen_tipo_cat, _usuario.Almacen_Grupo, estatus).ToList());
                RaisePropertyChanged("Productos");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public void CargaCatalogo(Object busqueda)
        {
            try 
            {
                if (SelectedBusquedaAlmacen_Tipo_Cat.ID_ALMACEN_TIPO==-1)
                    Productos = new ObservableCollection<PRODUCTO>(new cProducto().Seleccionar(busqueda.ToString(), _usuario.Almacen_Grupo,SelectedEstatus.CLAVE).ToList());
                else
                    Productos = new ObservableCollection<PRODUCTO>(new cProducto().Seleccionar(SelectedBusquedaAlmacen_Tipo_Cat.ID_ALMACEN_TIPO, _usuario.Almacen_Grupo, SelectedEstatus.CLAVE).ToList());
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
            }
        }

        public void CargarProducto_Categorias(bool isExceptionManaged = false)
        {
            try
            {
                //aqui va el codigo para determinar que puede ver el usuario
                
                producto_Categorias = new ObservableCollection<PRODUCTO_CATEGORIA>(new cProducto_Categoria().Seleccionar(_usuario.Almacen_Grupo, "S").ToList());
                var dummy = new PRODUCTO_CATEGORIA() {
                    ID_CATEGORIA=-1,
                    NOMBRE="Seleccione una categoria",
                    DESCR=string.Empty,
                    ACTIVO="S"
                };
                producto_Categorias.Insert(0, dummy);
                RaisePropertyChanged("Producto_Categorias");
            }
            catch(Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar las categorias. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public void  CargarProducto_Presentaciones(bool isExceptionManaged = false)
        {
            
            try
            {
                producto_Presentaciones = new ObservableCollection<PRODUCTO_PRESENTACION>(new cProducto_Presentacion().Seleccionar("S").ToList());
                var dummy = new PRODUCTO_PRESENTACION() {
                    ID_PRESENTACION=-1,
                    DESCR="Seleccione una presentación",
                    ACTIVO="S"
                };
                producto_Presentaciones.Insert(0, dummy);
                RaisePropertyChanged("Producto_Presentaciones");
            }
            catch(Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar las presentaciones. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public async Task CargarAlmacen_Tipos_CatAwaitable(string id_almacen_grupo, List<ALMACEN_TIPO_CAT> preseleccionados=null)
        {
            var _error = false;
            
            try
            {
                StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
                var temp = new ObservableCollection<EXT_Almacen_Tipo_Cat>(new cAlmacen_Tipo_Cat().Seleccionar_EXT(id_almacen_grupo, "S").ToList());
                if (preseleccionados!=null && preseleccionados.Count>0)
                    foreach(var item in preseleccionados)
                    {
                        var encontro = temp.FirstOrDefault(f => f.ID_ALMACEN_TIPO == item.ID_ALMACEN_TIPO);
                        if (encontro != null)
                            encontro.IS_SELECTED = true;
                    }
                Almacen_Tipos_Cat = temp;
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los tipos catalogos. Favor de contactar al administrador");
            }
        }

        public async void CargarAlmacen_Tipos_Cat(string id_almacen_grupo)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Por favor espere un momento");
            try
            {

                Almacen_Tipos_Cat = new ObservableCollection<EXT_Almacen_Tipo_Cat>(new cAlmacen_Tipo_Cat().Seleccionar_EXT(id_almacen_grupo, "S").ToList());
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los tipos catalogos. Favor de contactar al administrador");
            }
        }

        public void  CargarBusquedaAlmacen_Tipos_Cat(bool isExceptionManaged = false)
        {
            try
            {

                busquedaAlmacen_Tipos_Cat = new ObservableCollection<ALMACEN_TIPO_CAT>(new cAlmacen_Tipo_Cat().Seleccionar(_usuario.Almacen_Grupo,"S").ToList());
                var dummy = new ALMACEN_TIPO_CAT {
                   ACTIVO="S",
                   DESCR="TODOS",
                   ID_ALMACEN_GRUPO="",
                   ID_ALMACEN_TIPO=-1                   
                };
                busquedaAlmacen_Tipos_Cat.Add(dummy);
                RaisePropertyChanged("BusquedaAlmacen_Tipos_Cat");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public void CargarAlmacen_Grupos(bool isExceptionManaged = false)
        {
            try
            {
                almacen_grupos = new ObservableCollection<ALMACEN_GRUPO>(new cAlmacen_Grupo().SeleccionarTodos(string.Empty,_usuario.Almacen_Grupo,"S").ToList());
                if (string.IsNullOrWhiteSpace(_usuario.Almacen_Grupo))
                {
                    var dummy = new ALMACEN_GRUPO
                    {
                        ID_ALMACEN_GRUPO = "",
                        DESCR = "Selecciona un grupo",
                        ACTIVO = "S"
                    };
                    almacen_grupos.Add(dummy);
                }
                RaisePropertyChanged("Almacen_Grupos");
            }
            catch(Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los grupos de almacen. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public void  CargarProducto_Unidad_Medida(bool isExceptionManaged = false)
        {
            try
            {
                
                producto_Unidades_Medida = new ObservableCollection<PRODUCTO_UNIDAD_MEDIDA>(new cProducto_Unidad_Medida().Seleccionar("S").ToList());
                var dummy = new PRODUCTO_UNIDAD_MEDIDA()
                {
                    ID_UNIDAD_MEDIDA=-1,
                    NOMBRE = "Seleccione una unidad de medida",
                    DESCR=string.Empty ,
                    ACTIVO="S"
                };
                producto_Unidades_Medida.Insert(0, dummy);
                RaisePropertyChanged("Producto_Unidades_Medida");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar las unidades. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        private async void CambiarSubCategoria(object parametro)
        {
            var _error = false;
            var isProductoPopUpVisible = false;
            if (PopUpsViewModels.VisibleProductoPopUp == Visibility.Visible)
            {
                isProductoPopUpVisible = true;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTOPOPUP);
            }
            try
            {
               
                StaticSourcesViewModel.ShowProgressLoading("Por favor espere...");
                await Task.Factory.StartNew(() =>
                {
                    CargarSubCategoria(selectedProducto_Categoria.ID_CATEGORIA);
                })
                .ContinueWith((prevTask) => {
                    if (Producto_Subcategorias.Count>0)
                    {
                        if (IsSubcategoriaVisible!=true)
                            IsSubcategoriaVisible = true;
                       
                        selectedProducto_SubcategoriaValue = -1;
                        RaisePropertyChanged("SelectedProducto_SubcategoriaValue");
                        //setValidationRules(true);
                    }
                    else
                    {
                        IsSubcategoriaVisible = false;
                        setValidationRules(false);
                    }
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch(Exception ex)
            {
                _error = true;
            }
            if (_error)
                await _dialogCoordinator.ShowMessageAsync(this,"Error","Hubo un error al cargar las subcategorias. Favor de contactar al administrador");
            if (isProductoPopUpVisible)
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PRODUCTOPOPUP);
        }

        private void CargarSubCategoria(int categoria)
        {
                try
                {
                        producto_Subcategorias = new ObservableCollection<PRODUCTO_SUBCATEGORIA>(new cProducto_SubCategoria().Seleccionar(string.Empty, categoria, _usuario.Almacen_Grupo, "S").ToList());
                        if (producto_Subcategorias.Count > 0)
                        {
                            var dummy = new PRODUCTO_SUBCATEGORIA()
                            {
                                ID_SUBCATEGORIA = -1,
                                ID_CATEGORIA = -1,
                                DESCR = "Seleccione una subcategoria",
                                ACTIVO = "S"
                            };
                            producto_Subcategorias.Insert(0, dummy);
                        }
                        RaisePropertyChanged("Producto_Subcategorias");
                }
                catch (Exception ex)
                {
                    throw new Exception("Hubo un error al cargar las subcategorias. Favor de contactar al administrador");
                }
        }

        private void limpiarCampos()
        {
            ID = null;
            Activo = true;
            Descripcion = null;
            Nombre = null;
            Filename = null;
            if (string.IsNullOrWhiteSpace(_usuario.Almacen_Grupo))
                SelectedAlmacen_Grupo = Almacen_Grupos.FirstOrDefault(w => w.ID_ALMACEN_GRUPO == "");
            else
                SelectedAlmacen_Grupo = Almacen_Grupos.FirstOrDefault();
            SelectedProducto_Categoria = Producto_Categorias.FirstOrDefault(w=>w.ID_CATEGORIA==-1);
            SelectedProducto_Presentacion = Producto_Presentaciones.FirstOrDefault(w => w.ID_PRESENTACION == -1); ;
            SelectedProducto_SubcategoriaValue = -1;
            SelectedProducto_Unidad_Medida = Producto_Unidades_Medida.FirstOrDefault(w=>w.ID_UNIDAD_MEDIDA==-1);
            SelectedAlmacen_Tipo_Cat = null;
            IsSubcategoriaVisible = false;
            ImagenData = new FileManagerProvider().fileToByteArray("pack://application:,,,/GESAL;component/Resources/Images/nophoto.png"); 
        }

        private async void AccionCatalogoSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "agregar":
                    CondicionesIniciales();
                    limpiarCampos();
                    
                    SalvarHabilitado = true;
                    CancelarHabilitado = true;
                   
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PRODUCTOPOPUP);
                    setValidationRules(false);
                    RealizarAccion = AccionSalvar.Salvar;
                    break;
                case "editar":
                    if (SelectedProducto!=null && !String.IsNullOrEmpty(SelectedProducto.NOMBRE))
                    {
                        isCargaInformacion = true;
                        RealizarAccion = AccionSalvar.Actualizar;
                        SalvarHabilitado = true;
                        await CargarInformacionAwaitable();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PRODUCTOPOPUP);
                    }
                    break;
                case "salvar":
                    GuardarProducto();
                    CargaCatalogo(SelectedBusquedaAlmacen_Tipo_Cat.ID_ALMACEN_TIPO ,SelectedEstatus.CLAVE);
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTOPOPUP);
                    break;
                case "cancelar":
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTOPOPUP);
                    break;
                case "eliminar":
                    await EliminarProducto();
                    CargaCatalogo(SelectedBusquedaAlmacen_Tipo_Cat.ID_ALMACEN_TIPO ,SelectedEstatus.CLAVE);
                    CondicionesIniciales();
                    break;
            }
        }

        private async Task CargarInformacionAwaitable()
        {
            //Vacia datos a propiedades
            limpiarCampos();


            ID = SelectedProducto.ID_PRODUCTO;
            Nombre = SelectedProducto.NOMBRE;
            Descripcion = SelectedProducto.DESCRIPCION;
            if (SelectedProducto.PRODUCTO_IMAGEN != null)
                ImagenData = SelectedProducto.PRODUCTO_IMAGEN.IMAGEN;
            Activo = (SelectedProducto.ACTIVO == "S") ? true : false;

            if (SelectedProducto.ID_CATEGORIA.HasValue)
                SelectedProducto_Categoria = Producto_Categorias.FirstOrDefault(f => f.ID_CATEGORIA == SelectedProducto.ID_CATEGORIA.Value);
            if (SelectedProducto.ID_SUBCATEGORIA.HasValue||SelectedProducto_Categoria.PRODUCTO_SUBCATEGORIA.Count>0)
            {
                IsSubcategoriaVisible = true;
                StaticSourcesViewModel.ShowProgressLoading("Espere un momento...");
                await Task.Factory.StartNew(() =>
                {
                    CargarSubCategoria(selectedProducto_Categoria.ID_CATEGORIA);
                })
                .ContinueWith((prevTask) => {
                    if (SelectedProducto.ID_SUBCATEGORIA.HasValue)
                         SelectedProducto_SubcategoriaValue= Producto_Subcategorias.FirstOrDefault(f => f.ID_SUBCATEGORIA == SelectedProducto.ID_SUBCATEGORIA.Value).ID_SUBCATEGORIA;
                    else
                        SelectedProducto_SubcategoriaValue = -1;
                    IsSubcategoriaVisible = true;
                    setValidationRules(true);
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            else
            {
                IsSubcategoriaVisible = false;
                setValidationRules(false);
            }
            if (SelectedProducto.ID_PRESENTACION.HasValue)
                SelectedProducto_Presentacion = Producto_Presentaciones.FirstOrDefault(f => f.ID_PRESENTACION == SelectedProducto.ID_PRESENTACION.Value);
            if (SelectedProducto.ID_UNIDAD_MEDIDA.HasValue)
                SelectedProducto_Unidad_Medida = Producto_Unidades_Medida.FirstOrDefault(f => f.ID_UNIDAD_MEDIDA == SelectedProducto.ID_UNIDAD_MEDIDA.Value);
            //hay que cambiar el tipo de llenado porque es asincrono
            selectedAlmacen_Grupo = Almacen_Grupos.FirstOrDefault(w => w.ID_ALMACEN_GRUPO == SelectedProducto.ALMACEN_TIPO_CAT.First().ID_ALMACEN_GRUPO);
            RaisePropertyChanged("SelectedAlmacen_Grupo");
            IsGrupoDefinido = true;
            await CargarAlmacen_Tipos_CatAwaitable(selectedAlmacen_Grupo.ID_ALMACEN_GRUPO, SelectedProducto.ALMACEN_TIPO_CAT.ToList());
            setValidationRules(false);

        }

        private async void CargarInformacion()
        {
            //Vacia datos a propiedades
            limpiarCampos();
            

            ID = SelectedProducto.ID_PRODUCTO;
            Nombre = SelectedProducto.NOMBRE;
            Descripcion = SelectedProducto.DESCRIPCION;
            if (SelectedProducto.PRODUCTO_IMAGEN != null)
                ImagenData = SelectedProducto.PRODUCTO_IMAGEN.IMAGEN;
            Activo = (SelectedProducto.ACTIVO == "S") ? true : false;

            if (SelectedProducto.ID_CATEGORIA.HasValue)
                SelectedProducto_Categoria = Producto_Categorias.FirstOrDefault(f => f.ID_CATEGORIA == SelectedProducto.ID_CATEGORIA.Value);
            if (SelectedProducto.ID_SUBCATEGORIA.HasValue||SelectedProducto_Categoria.PRODUCTO_SUBCATEGORIA.Count>0)
            {
                IsSubcategoriaVisible = true;
                CargarSubCategoria(SelectedProducto_Categoria.ID_CATEGORIA);
                if (SelectedProducto.ID_SUBCATEGORIA.HasValue)
                    SelectedProducto_SubcategoriaValue = Producto_Subcategorias.FirstOrDefault(f => f.ID_SUBCATEGORIA == SelectedProducto.ID_SUBCATEGORIA.Value).ID_SUBCATEGORIA;
                else
                    SelectedProducto_SubcategoriaValue = -1;
            }
            if (SelectedProducto.ID_PRESENTACION.HasValue)
                SelectedProducto_Presentacion = Producto_Presentaciones.FirstOrDefault(f => f.ID_PRESENTACION == SelectedProducto.ID_PRESENTACION.Value);
            if (SelectedProducto.ID_UNIDAD_MEDIDA.HasValue)
                SelectedProducto_Unidad_Medida = Producto_Unidades_Medida.FirstOrDefault(f => f.ID_UNIDAD_MEDIDA == SelectedProducto.ID_UNIDAD_MEDIDA.Value);
            //hay que cambiar el tipo de llenado porque es asincrono
            selectedAlmacen_Grupo = Almacen_Grupos.FirstOrDefault(w => w.ID_ALMACEN_GRUPO == SelectedProducto.ALMACEN_TIPO_CAT.First().ID_ALMACEN_GRUPO);
            RaisePropertyChanged("SelectedAlmacen_Grupo");
            IsGrupoDefinido = true;
            await CargarAlmacen_Tipos_CatAwaitable(selectedAlmacen_Grupo.ID_ALMACEN_GRUPO, SelectedProducto.ALMACEN_TIPO_CAT.ToList());
            setValidationRules(false);
           
        }


        private void CondicionesIniciales()
        {
            SelectedProducto = null;
            Almacen_Tipos_Cat = new ObservableCollection<EXT_Almacen_Tipo_Cat>();
            SalvarHabilitado = false;
            CancelarHabilitado = false;
            EditarHabilitado = false;
            EliminarHabilitado = false;
            
            ClearRules();
            limpiarCampos();
        }

        private void GuardarProducto()
        {
            try
            {
                var _nombre = string.Empty;
                //Si SelectedProductoAutoComplete viene nulo entonces el nombre capturado por el usuario no encuentra en la lista o no
                //selecciono un producto de la lista sugerida de productos
                if (SelectedProductoAutoComplete == null)
                    _nombre = Nombre;
                else
                    _nombre = SelectedProductoAutoComplete.NOMBRE;
                var _producto = new PRODUCTO
                {
                    ACTIVO = (Activo) ? "S" : "N",
                    NOMBRE = _nombre,
                    DESCRIPCION = Descripcion,
                    ID_UNIDAD_MEDIDA=SelectedProducto_Unidad_Medida.ID_UNIDAD_MEDIDA,
                    ID_CATEGORIA=SelectedProducto_Categoria.ID_CATEGORIA,
                    ID_PRESENTACION=SelectedProducto_Presentacion.ID_PRESENTACION,
                    ID_SUBCATEGORIA=(IsSubcategoriaVisible)?(int?)SelectedProducto_SubcategoriaValue:null
                };

                var _selected_Almacen_Tipo_Cat = new List<ALMACEN_TIPO_CAT>();
                foreach (var item in Almacen_Tipos_Cat)
                    if (item.IS_SELECTED)
                        _selected_Almacen_Tipo_Cat.Add(new ALMACEN_TIPO_CAT
                        {
                            ACTIVO=item.ACTIVO,
                            DESCR=item.DESCR,
                            ID_ALMACEN_TIPO=item.ID_ALMACEN_TIPO
                        });

                var img_bytes = imagenToBytes(Filename);

                if (RealizarAccion == AccionSalvar.Salvar)
                {
                     
                    new cProducto().Insertar(_producto, _selected_Almacen_Tipo_Cat, (img_bytes != null) ? new PRODUCTO_IMAGEN
                    {
                        IMAGEN=img_bytes
                    }:null);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se inserto el producto con exito");
                }
                else if (RealizarAccion == AccionSalvar.Actualizar)
                {
                    _producto.ID_PRODUCTO  = ID.Value;
                    new cProducto().Actualizar(_producto, _selected_Almacen_Tipo_Cat, img_bytes);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", string.Format("Se actualizo la el producto {0} con exito", _producto.NOMBRE));

                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
            }
        }

        private byte[] imagenToBytes(string _filename)
        {
            if (!string.IsNullOrWhiteSpace(_filename))
                return new FileManagerProvider().fileToByteArray(Filename);
            else
                return null;
        }

        private async Task EliminarProducto()
        {
            var _error = false;
            try
            {
                if (await _dialogCoordinator.ShowMessageAsync(this, "Confirmación",String.Format("¿Esta seguro de cambiar el estatus a INACTIVO del producto {0}?",SelectedProducto.NOMBRE), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    new cProducto().ActualizarEstatus(SelectedProducto.ID_PRODUCTO,"N");
                    await _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se cambio el estatus del producto con exito");
                }

            }
            catch (Exception ex)
            { _error = true; };
            if (_error)
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
        }

        private void LlenarEstatusDisponibles()
        {
            Lista_Estatus = new Estatus_Disponibles();
            var dummy = new Estatus
            {
                CLAVE = "",
                DESCRIPCION = "TODOS"
            };
            Lista_Estatus.LISTA_ESTATUS.Add(dummy);
            
        }

        private bool CanExecute(object parameter) { return base.HasErrors == false; }

        private void RaiseChangeCheckedTipo(object obj)
        {
            RaisePropertyChanged("Almacen_Tipos_Cat");
        }

        private async void OnLoad(object sender)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Espere un momento..");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    LlenarEstatusDisponibles();
                    CargarBusquedaAlmacen_Tipos_Cat(true);
                    CargaCatalogo(BusquedaAlmacen_Tipos_Cat.First(w => w.ID_ALMACEN_TIPO == -1).ID_ALMACEN_TIPO, Lista_Estatus.LISTA_ESTATUS.First(w => w.CLAVE == "S").CLAVE, true);
                    CargarAlmacen_Grupos(true);
                    CargarProducto_Categorias(true);
                    CargarProducto_Presentaciones(true);
                    CargarProducto_Unidad_Medida(true);
                }).ContinueWith((prevTask) => {
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.First(w => w.CLAVE == "S");
                    SelectedBusquedaAlmacen_Tipo_Cat = BusquedaAlmacen_Tipos_Cat.First(w=>w.ID_ALMACEN_TIPO==-1);
                    if (string.IsNullOrWhiteSpace(_usuario.Almacen_Grupo))
                    {
                        SelectedAlmacen_Grupo = Almacen_Grupos.First(w => w.ID_ALMACEN_GRUPO == "");
                    }
                    else
                    {
                        selectedAlmacen_Grupo = Almacen_Grupos.FirstOrDefault();
                        RaisePropertyChanged("SelectedAlmacen_Grupo");
                    }
                    SelectedProducto_Categoria = Producto_Categorias.First(w => w.ID_CATEGORIA == -1);
                    SelectedProducto_Presentacion = Producto_Presentaciones.First(w => w.ID_PRESENTACION == -1);
                    SelectedProducto_Unidad_Medida = Producto_Unidades_Medida.First(w => w.ID_UNIDAD_MEDIDA == -1);
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception)
            {
                _error = true;
            }
            if (_error)
            {
                StaticSourcesViewModel.CloseProgressLoading();
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los catalogos. Favor de contactar al administrador");
            }
        }

        #endregion


        #region Command
        private ICommand accionCatalogoSinValidar;
        public ICommand AccionCatalogoSinValidar
        {
            get
            {

                return accionCatalogoSinValidar ?? (accionCatalogoSinValidar = new RelayCommand(AccionCatalogoSwitch));
            }
        }

        private ICommand accionCatalogo;
        public ICommand AccionCatalogo
        {
            get
            {

                return accionCatalogo ?? (accionCatalogo = new RelayCommand(AccionCatalogoSwitch, CanExecute));
            }
        }

        private ICommand buscar;
        public ICommand Buscar
        {
            get { return buscar ?? (buscar = new RelayCommand(CargaCatalogo)); }
        }

        private ICommand onChecked;
        public ICommand OnChecked
        {
            get { return onChecked ?? (onChecked = new RelayCommand(RaiseChangeCheckedTipo)); }
        }

        private ICommand cmdLoad;
        public ICommand CmdLoad
        {
            get { return cmdLoad ?? (cmdLoad = new RelayCommand(OnLoad)); }
        }

        private ICommand cmdProducto_CategoriasChanged;

        public ICommand CmdProducto_CategoriasChanged
        {
            get { return cmdProducto_CategoriasChanged ?? (cmdProducto_CategoriasChanged = new RelayCommand(CambiarSubCategoria)); }
        }
        #endregion
    }
}
