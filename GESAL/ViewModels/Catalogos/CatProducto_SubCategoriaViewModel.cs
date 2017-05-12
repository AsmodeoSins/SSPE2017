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
using GESAL.Models;
namespace GESAL.ViewModels
{
    public class CatProducto_SubCategoriaViewModel:ValidationViewModelBase,IDataErrorInfo
    {

        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;
        public CatProducto_SubCategoriaViewModel(IDialogCoordinator dialogCoordinator, Usuario usuario)
        {
            _dialogCoordinator = dialogCoordinator;
            _usuario=usuario;
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

        private ObservableCollection<PRODUCTO_SUBCATEGORIA> producto_SubCategorias;

        public ObservableCollection<PRODUCTO_SUBCATEGORIA> Producto_SubCategorias
        {
            get { return producto_SubCategorias; }
            set { producto_SubCategorias = value; RaisePropertyChanged("Producto_SubCategorias"); }
        }

        private PRODUCTO_SUBCATEGORIA selectedProducto_SubCategoria;

        public PRODUCTO_SUBCATEGORIA SelectedProducto_SubCategoria
        {
            get { return selectedProducto_SubCategoria; }
            set
            {
                selectedProducto_SubCategoria = value;
                if (value != null)
                {
                    EliminarHabilitado = true;
                    EditarHabilitado = true;
                    CancelarHabilitado = true;
                }
                RaisePropertyChanged("SelectedProducto_SubCategoria");
            }
        }

        private ObservableCollection<PRODUCTO_CATEGORIA> producto_Categorias;

        public ObservableCollection<PRODUCTO_CATEGORIA> Producto_Categorias
        {
            get { return producto_Categorias; }
            set { producto_Categorias = value; RaisePropertyChanged("Producto_Categorias"); }
        }

        private ObservableCollection<PRODUCTO_CATEGORIA> pop_Up_Producto_Categorias;
        public ObservableCollection<PRODUCTO_CATEGORIA> Pop_Up_Producto_Categorias
        {
            get { return pop_Up_Producto_Categorias; }
            set { pop_Up_Producto_Categorias = value; RaisePropertyChanged("Pop_Up_Producto_Categorias"); }
        }

        private PRODUCTO_CATEGORIA selectedProducto_Categoria;
        public PRODUCTO_CATEGORIA SelectedProducto_Categoria
        {
            get { return selectedProducto_Categoria; }
            set { selectedProducto_Categoria = value; RaisePropertyChanged("SelectedProducto_Categoria"); }
        }

        private PRODUCTO_CATEGORIA selectedProducto_CategoriaVistaDetalle;
        public PRODUCTO_CATEGORIA SelectedProducto_CategoriaVistaDetalle
        {
            get { return selectedProducto_CategoriaVistaDetalle; }
            set { selectedProducto_CategoriaVistaDetalle = value; RaisePropertyChanged("SelectedProducto_CategoriaVistaDetalle"); }
        }

        private string busquedaParametro = string.Empty ;

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
        #endregion
        #region Metodos

        #region Validacion
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCION ES REQUERIDA!");
            base.AddRule(() => SelectedProducto_CategoriaVistaDetalle, () => !(SelectedProducto_CategoriaVistaDetalle.ID_CATEGORIA==-1), "CATEGORIA ES REQUERIDA!");
            RaisePropertyChanged("Descripcion");
            RaisePropertyChanged("SelectedProducto_CategoriaVistaDetalle");
        }
        #endregion

        public void CargaCatalogo(string estatus, bool isExceptionManaged = false)
        {
            try
            {
                producto_SubCategorias = new ObservableCollection<PRODUCTO_SUBCATEGORIA>(new cProducto_SubCategoria().Seleccionar(_usuario.Almacen_Grupo,estatus).ToList());
                RaisePropertyChanged("Producto_SubCategorias");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public void CargaCatalogoParametrizada(Object busqueda)
        {
            try
            {
                Producto_SubCategorias = new ObservableCollection<PRODUCTO_SUBCATEGORIA>(new cProducto_SubCategoria().Seleccionar(BusquedaParametro, SelectedProducto_Categoria.ID_CATEGORIA, _usuario.Almacen_Grupo,SelectedEstatus.CLAVE).ToList());
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
                producto_Categorias = new ObservableCollection<PRODUCTO_CATEGORIA>(new cProducto_Categoria().Seleccionar(_usuario.Almacen_Grupo,"S").ToList());
                var dummy=new PRODUCTO_CATEGORIA
                {
                    ACTIVO = "S",
                    DESCR = "",
                    ID_PROD_GRUPO = "",
                    ID_CATEGORIA = -1,
                    NOMBRE = "Todas",
                };
                producto_Categorias.Insert(0,dummy);
                RaisePropertyChanged("Producto_Categorias");
                
            }catch(Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar las categorias. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public void CargarProducto_CategoriasVistaDetalle(bool isExceptionManaged = false)
        {
            try
            {
                pop_Up_Producto_Categorias = new ObservableCollection<PRODUCTO_CATEGORIA>(new cProducto_Categoria().Seleccionar(_usuario.Almacen_Grupo, "S").ToList());
                var dummy= new PRODUCTO_CATEGORIA
                {
                    ACTIVO = "S",
                    DESCR = "",
                    ID_PROD_GRUPO = "",
                    ID_CATEGORIA = -1,
                    NOMBRE = "Selecciona una",
                };
                pop_Up_Producto_Categorias.Insert(0, dummy);
                RaisePropertyChanged("Pop_Up_Producto_Categorias");

            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar las categorias. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        private void limpiarCampos()
        {
            ID = null;
            Activo = true;
            Descripcion = null;

        }

        private async void AccionCatalogoSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "agregar":
                    CondicionesIniciales();
                    limpiarCampos();                    
                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.PRODUCTO_SUBCATEGORIAPOPUP);
                    setValidationRules();
                    SelectedProducto_CategoriaVistaDetalle = Pop_Up_Producto_Categorias.First(w => w.ID_CATEGORIA == -1);
                    SalvarHabilitado = true;
                    CancelarHabilitado = true;
                    RealizarAccion = AccionSalvar.Salvar;
                    break;
                case "editar":
                    if (!String.IsNullOrEmpty(SelectedProducto_SubCategoria.DESCR))
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PRODUCTO_SUBCATEGORIAPOPUP);
                        setValidationRules();
                        CargarProducto_CategoriasVistaDetalle();
                        ID = SelectedProducto_SubCategoria.ID_SUBCATEGORIA;
                        Descripcion = SelectedProducto_SubCategoria.DESCR;
                        Activo = (SelectedProducto_SubCategoria.ACTIVO == "S") ? true : false;
                        SelectedProducto_CategoriaVistaDetalle = Pop_Up_Producto_Categorias.FirstOrDefault(f => f.ID_CATEGORIA == SelectedProducto_SubCategoria.PRODUCTO_CATEGORIA.ID_CATEGORIA);
                        RealizarAccion = AccionSalvar.Actualizar;
                        SalvarHabilitado = true;
                    }
                    break;
                case "salvar":
                    GuardarProductoTipo();
                    CargaCatalogo(SelectedEstatus.CLAVE);
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTO_SUBCATEGORIAPOPUP);
                    break;
                case "cancelar":

                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTO_SUBCATEGORIAPOPUP);
                    break;
                case "eliminar":
                    await EliminarProductoSubCategoria();
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

        private void GuardarProductoTipo()
        {
            try
            {
                var _producto_subcategoria = new PRODUCTO_SUBCATEGORIA
                {
                    ACTIVO = (Activo) ? "S" : "N",
                    DESCR = Descripcion,
                    ID_CATEGORIA=SelectedProducto_CategoriaVistaDetalle.ID_CATEGORIA
                };

                if (RealizarAccion == AccionSalvar.Salvar)
                {
                    new cProducto_SubCategoria().Insertar(_producto_subcategoria);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se inserto la subcategoria de producto con exito");
                }
                else if (RealizarAccion == AccionSalvar.Actualizar)
                {
                    _producto_subcategoria.ID_SUBCATEGORIA = ID.Value;
                    new cProducto_SubCategoria().Actualizar(_producto_subcategoria);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", string.Format("Se actualizo la subcategoria de producto {0} con exito", _producto_subcategoria.DESCR));

                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
            }
        }

        private async Task EliminarProductoSubCategoria()
        {
            var _error = false;
            try
            {
                if (await _dialogCoordinator.ShowMessageAsync(this, "Confirmación", String.Format("¿Esta seguro de cambiar el estatus a INACTIVO de la subcategoria de producto {0}?", SelectedProducto_SubCategoria.DESCR), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    new cProducto_SubCategoria().Actualizar(new PRODUCTO_SUBCATEGORIA
                    {
                        ACTIVO = "N",
                        DESCR = SelectedProducto_SubCategoria.DESCR,
                        ID_SUBCATEGORIA=SelectedProducto_SubCategoria.ID_SUBCATEGORIA,
                        ID_CATEGORIA=SelectedProducto_SubCategoria.ID_CATEGORIA
                    });
                    await _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se cambio el estatus de la subcategoria de producto con exito");
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
                    LlenarEstatusDisponibles();
                    CargaCatalogo("S",true);
                    CargarProducto_Categorias(true);
                    CargarProducto_CategoriasVistaDetalle(true);
                })
                .ContinueWith((prevTask) => {
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.First(w => w.CLAVE == "S");
                    SelectedProducto_Categoria = Producto_Categorias.First(w=>w.ID_CATEGORIA==-1);
                    SelectedProducto_CategoriaVistaDetalle = Pop_Up_Producto_Categorias.First(w=>w.ID_CATEGORIA==-1);
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
            get { return buscar ?? (buscar = new RelayCommand(CargaCatalogoParametrizada)); }
        }

        private ICommand cmdLoad;
        public ICommand CmdLoad
        {
            get { return cmdLoad ?? (cmdLoad = new RelayCommand(OnLoad)); }
        }

        #endregion
    }
}
