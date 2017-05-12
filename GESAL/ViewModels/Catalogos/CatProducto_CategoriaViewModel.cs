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
using MVVMShared.Views;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using GESAL.Models;
using GESAL.Clases.Enums;
namespace GESAL.ViewModels
{
    public class CatProducto_CategoriaViewModel : ValidationViewModelBase, IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;
        public CatProducto_CategoriaViewModel(IDialogCoordinator dialogCoordinator, Usuario usuario)
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
            set { nombre = value; RaisePropertyChanged("Nombre"); }
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


        private ObservableCollection<ALMACEN_GRUPO> almacen_Grupos;

        public ObservableCollection<ALMACEN_GRUPO> Almacen_Grupos
        {
            get { return almacen_Grupos; }
            set { almacen_Grupos = value; RaisePropertyChanged("Almacen_Grupos"); }
        }

        private ALMACEN_GRUPO selectedAlmacen_Grupo = null;
        public ALMACEN_GRUPO SelectedAlmacen_Grupo
        {
            get { return selectedAlmacen_Grupo; }
            set { selectedAlmacen_Grupo = value; RaisePropertyChanged("SelectedAlmacen_Grupo"); }
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
                if (value != null)
                {
                    EliminarHabilitado = true;
                    EditarHabilitado = true;
                   
                    CancelarHabilitado = true;

                   
                }
                RaisePropertyChanged("SelectedProducto_Categoria");
            }
        }

        private string busquedaParametro=string.Empty ;

        public string BusquedaParametro
        {
            get { return busquedaParametro; }
            set { busquedaParametro = value; RaisePropertyChanged("BusquedaParametro"); }
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

        private bool isGrupoEnabled = false;
        public bool IsGrupoEnabled
        {
            get { return isGrupoEnabled; }
            set { isGrupoEnabled = value; RaisePropertyChanged("IsGrupoEnabled"); }
        }
        #endregion


        #region Metodos

        #region Validacion
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Nombre, () => !string.IsNullOrEmpty(Nombre), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCION ES REQUERIDA!");
            base.AddRule(() => SelectedAlmacen_Grupo, () => !(SelectedAlmacen_Grupo==null), "ES NECESARIO DEFINIR QUE GRUPO DE ALMACEN ES");
            RaisePropertyChanged("Nombre");
            RaisePropertyChanged("Descripcion");
            RaisePropertyChanged("SelectedAlmacen_Grupo");
        }
        #endregion

        public void CargaCatalogo(string estatus,bool isExceptionManaged=false)
        {
            try
            {
                producto_Categorias = new ObservableCollection<PRODUCTO_CATEGORIA>(new cProducto_Categoria().Seleccionar(_usuario.Almacen_Grupo,estatus).ToList());
                RaisePropertyChanged("Producto_Categorias");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public void CargaCatalogo(Object param)
        {
            var params1 = (object[])param;
            var busqueda = params1[0].ToString();
            var isExceptionManaged = Convert.ToBoolean(params1[1]);
            try
            {
                Producto_Categorias = new ObservableCollection<PRODUCTO_CATEGORIA>(new cProducto_Categoria().Seleccionar(busqueda.ToString(),_usuario.Almacen_Grupo,SelectedEstatus.CLAVE).ToList());
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public void CargarAlmacen_Grupo(string id_almacen_grupo, bool isExceptionManaged = false)
        {
            try
            {
                almacen_Grupos = new ObservableCollection<ALMACEN_GRUPO>(new cAlmacen_Grupo().SeleccionarTodos(string.Empty, id_almacen_grupo,"S").ToList());
                RaisePropertyChanged("Almacen_Grupos");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        private void limpiarCampos()
        {
            ID = null;
            Activo = true;
            Descripcion = null;
            Nombre = null;
            if (string.IsNullOrWhiteSpace(_usuario.Almacen_Grupo))
                SelectedAlmacen_Grupo = null;
            else
                SelectedAlmacen_Grupo = Almacen_Grupos.FirstOrDefault(w => w.ID_ALMACEN_GRUPO == _usuario.Almacen_Grupo);

        }

        private async void AccionCatalogoSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "agregar":
                    CondicionesIniciales();
                    SalvarHabilitado = true;
                    CancelarHabilitado = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PRODUCTO_CATEGORIAPOPUP);
                    setValidationRules();
                    RealizarAccion = AccionSalvar.Salvar;
                    break;
                case "editar":
                    if (!String.IsNullOrEmpty(SelectedProducto_Categoria.NOMBRE))
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PRODUCTO_CATEGORIAPOPUP);
                        setValidationRules();
                        ID = SelectedProducto_Categoria.ID_CATEGORIA;
                        Nombre = SelectedProducto_Categoria.NOMBRE;
                        Descripcion = SelectedProducto_Categoria.DESCR;
                        Activo = (SelectedProducto_Categoria.ACTIVO == "S") ? true : false;
                        SelectedAlmacen_Grupo = Almacen_Grupos.FirstOrDefault(w => w.ID_ALMACEN_GRUPO == SelectedProducto_Categoria.ID_PROD_GRUPO);
                        RealizarAccion = AccionSalvar.Actualizar;
                        SalvarHabilitado = true;
                    }
                    break;
                case "salvar":
                    GuardarProductoCategoria();
                    CargaCatalogo(SelectedEstatus.CLAVE);
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTO_CATEGORIAPOPUP);
                    break;
                case "cancelar":
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTO_CATEGORIAPOPUP);
                    break;
                case "eliminar":
                    await EliminarProductoCategoria();
                    CargaCatalogo(SelectedEstatus.CLAVE);
                    CondicionesIniciales();
                    break;
            }
        }

        private void CondicionesIniciales()
        {
            SalvarHabilitado = false;
            CancelarHabilitado = false;
            EditarHabilitado = false;
            EliminarHabilitado = false;
            
            ClearRules();
            limpiarCampos();
        }

        private void GuardarProductoCategoria()
        {
            try
            {
                var _producto_categoria = new PRODUCTO_CATEGORIA
                {
                    ACTIVO = (Activo) ? "S" : "N",
                    NOMBRE = Nombre,
                    DESCR = Descripcion,
                    ID_PROD_GRUPO = SelectedAlmacen_Grupo.ID_ALMACEN_GRUPO
                };

                if (RealizarAccion == AccionSalvar.Salvar)
                {
                    new cProducto_Categoria().Insertar(_producto_categoria);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se inserto la categoria del producto con exito");
                }
                else if (RealizarAccion == AccionSalvar.Actualizar)
                {
                    _producto_categoria.ID_CATEGORIA = ID.Value;
                    new cProducto_Categoria().Actualizar(_producto_categoria);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion",string.Format("Se actualizo la categoria del producto {0} con exito",_producto_categoria.NOMBRE));

                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
            }
        }

        private async Task EliminarProductoCategoria()
        {
            var _error = false;
            try
            {
                if (await _dialogCoordinator.ShowMessageAsync(this, "Confirmación", String.Format("¿Esta seguro de cambiar el estatus a INACTIVO de la categoria del producto {0}?", SelectedProducto_Categoria.NOMBRE), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    new cProducto_Categoria().Actualizar(new PRODUCTO_CATEGORIA
                    {
                        ACTIVO = "N",
                        NOMBRE = SelectedProducto_Categoria.NOMBRE,
                        DESCR = SelectedProducto_Categoria.DESCR,
                        ID_CATEGORIA = SelectedProducto_Categoria.ID_CATEGORIA,
                        ID_PROD_GRUPO = SelectedProducto_Categoria.ID_PROD_GRUPO
                    });
                    await _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se cambio el estatus de la categoria del producto con exito");
                }

            }
            catch (Exception ex)
            { _error = true; };
            if (_error)
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");

        }

        private void LlenarEstatusDisponibles()
        {
            lista_estatus = new Estatus_Disponibles();
            var dummy = new Estatus
            {
                CLAVE = "",
                DESCRIPCION = "TODOS"
            };
            lista_estatus.LISTA_ESTATUS.Add(dummy);
            RaisePropertyChanged("Lista_Estatus");
        }

        private bool CanExecute(object parameter) { return base.HasErrors == false; }

        private async void OnLoad(object sender)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Espere un momento...");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CargarAlmacen_Grupo(_usuario.Almacen_Grupo, true);
                    LlenarEstatusDisponibles();
                    CargaCatalogo("S", true);
                })
                .ContinueWith((prevTask) => {
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.First(w => w.CLAVE == "S");
                    if (string.IsNullOrWhiteSpace(_usuario.Almacen_Grupo))
                        IsGrupoEnabled = true;
                });
                StaticSourcesViewModel.CloseProgressLoading();
            }
            catch (Exception ex)
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

        private ICommand cmdLoad;
        public ICommand CmdLoad
        {
            get { return cmdLoad ?? (cmdLoad = new RelayCommand(OnLoad)); }
        }

        #endregion
    }
}
