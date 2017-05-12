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
    public class CatProducto_Unidad_MedidaViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;
        public CatProducto_Unidad_MedidaViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
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

        private bool editarHabilitado = false ;
        public bool EditarHabilitado
        {
            get { return editarHabilitado; }
            set { editarHabilitado = value; RaisePropertyChanged("EditarHabilitado"); }
        }

        private bool cancelarHabilitado=false;
        public bool CancelarHabilitado
        {
            get { return cancelarHabilitado; }
            set { cancelarHabilitado = value; RaisePropertyChanged("CancelarHabilitado"); }
        }

        private bool eliminarHabilitado=false;
        public bool EliminarHabilitado
        {
            get { return eliminarHabilitado; }
            set { eliminarHabilitado = value; RaisePropertyChanged("EliminarHabilitado"); }
        }

        private bool salvarHabilitado=false;
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
            set { selectedProducto_Unidad_Medida = value;
                if (value!=null)
                {   
                    EliminarHabilitado = true;
                    EditarHabilitado = true;
                    CancelarHabilitado = true;
                }
                RaisePropertyChanged("SelectedProducto_Unidad_Medida"); }
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

        #endregion


        #region Metodos

        #region Validacion
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Nombre, () => !string.IsNullOrEmpty(Nombre), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCION ES REQUERIDA!");
            RaisePropertyChanged("Nombre");
            RaisePropertyChanged("Descripcion");
        }
        #endregion

        public void CargaCatalogo(string estatus, bool isExceptionManaged = false)
        {
            try
            {
                producto_Unidades_Medida = new ObservableCollection<PRODUCTO_UNIDAD_MEDIDA>(new cProducto_Unidad_Medida().Seleccionar(estatus).ToList());
                RaisePropertyChanged("Producto_Unidades_Medida");
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
                Producto_Unidades_Medida = new ObservableCollection<PRODUCTO_UNIDAD_MEDIDA>(new cProducto_Unidad_Medida().Seleccionar(busqueda.ToString(),SelectedEstatus.CLAVE).ToList());
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
            }
        }

        private void limpiarCampos()
        {
            ID = null;
            Activo = true;
            Descripcion = null;
            Nombre = null;

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
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PRODUCTO_UNIDAD_MEDIDAPOPUP);
                    setValidationRules();
                    RealizarAccion = AccionSalvar.Salvar;
                    break;
                case "editar":
                    if (!String.IsNullOrEmpty(SelectedProducto_Unidad_Medida.NOMBRE))
                    {

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PRODUCTO_UNIDAD_MEDIDAPOPUP);
                        setValidationRules();
                        ID = SelectedProducto_Unidad_Medida.ID_UNIDAD_MEDIDA;
                        Nombre = SelectedProducto_Unidad_Medida.NOMBRE;
                        Descripcion = SelectedProducto_Unidad_Medida.DESCR;
                        Activo = (SelectedProducto_Unidad_Medida.ACTIVO == "S") ? true : false;
                        RealizarAccion = AccionSalvar.Actualizar;
                        SalvarHabilitado = true;
                    }                    
                    break;
                case "salvar":
                    GuardarProductoUnidadMedida();
                    CargaCatalogo(SelectedEstatus.CLAVE);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTO_UNIDAD_MEDIDAPOPUP);
                    CondicionesIniciales();
                    
                    break;
                case "cancelar":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTO_UNIDAD_MEDIDAPOPUP);
                    CondicionesIniciales();
                    break;
                case "eliminar":
                    await EliminarProductoUnidadMedida();
                    CargaCatalogo(SelectedEstatus.CLAVE );
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

        private void GuardarProductoUnidadMedida()
        {
            try
            {
                var _producto_unidad_medida = new PRODUCTO_UNIDAD_MEDIDA
                {
                    ACTIVO = (Activo) ? "S" : "N",
                    NOMBRE = Nombre,
                    DESCR = Descripcion
                };

                if (RealizarAccion == AccionSalvar.Salvar)
                {
                    new cProducto_Unidad_Medida().Insertar(_producto_unidad_medida);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se inserto la unidad de medida con exito");
                }
                else if (RealizarAccion == AccionSalvar.Actualizar)
                {
                    _producto_unidad_medida.ID_UNIDAD_MEDIDA = ID.Value;
                    new cProducto_Unidad_Medida().Actualizar(_producto_unidad_medida);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion",string.Format("Se actualizo la unidad de medida {0} con exito",_producto_unidad_medida.NOMBRE));
                    
                }
            }catch(Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this,"Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
            }
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

        private async Task EliminarProductoUnidadMedida()
        {
            var _error = false;
            try
            {
                if (await _dialogCoordinator.ShowMessageAsync(this, "Confirmación", String.Format("¿Esta seguro de cambiar el estatus a INACTIVO de la unidad de medida {0}?", SelectedProducto_Unidad_Medida.NOMBRE), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    new cProducto_Unidad_Medida().Actualizar(new PRODUCTO_UNIDAD_MEDIDA
                    {
                        ACTIVO = "N",
                        NOMBRE = SelectedProducto_Unidad_Medida.NOMBRE,
                        DESCR = SelectedProducto_Unidad_Medida.DESCR,
                        ID_UNIDAD_MEDIDA=SelectedProducto_Unidad_Medida.ID_UNIDAD_MEDIDA
                    });
                    await _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se cambio el estatus de la unidad de medida con exito");
                }
                
            }catch(Exception ex)
            { _error = true; };
            if(_error)
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");

        }

        private bool CanDownload(object parameter) { return base.HasErrors == false; }

        private async void OnLoad(object sender)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Espere un momento...");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    LlenarEstatusDisponibles();
                    CargaCatalogo("S", true);
                })
                .ContinueWith((prevTask) => {
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.First(w => w.CLAVE == "S");
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
        private ICommand cmdLoad;
        public ICommand CmdLoad
        {
            get { return cmdLoad??(cmdLoad=new RelayCommand(OnLoad));}
        }

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
                
                return accionCatalogo ?? (accionCatalogo = new RelayCommand(AccionCatalogoSwitch,CanDownload));
            }
        }

        private ICommand buscar;
        public ICommand Buscar
        {
            get { return buscar ?? (buscar = new RelayCommand(CargaCatalogo)); }
        }
       
        #endregion

    }
}
