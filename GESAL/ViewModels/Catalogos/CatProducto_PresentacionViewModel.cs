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
    public class CatProducto_PresentacionViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;

        public CatProducto_PresentacionViewModel(IDialogCoordinator dialogCoordinator)
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

        private short? id;
        public short? ID
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

        private ObservableCollection<PRODUCTO_PRESENTACION> producto_Presentaciones;

        public ObservableCollection<PRODUCTO_PRESENTACION> Producto_Presentaciones
        {
            get { return producto_Presentaciones; }
            set { producto_Presentaciones = value; RaisePropertyChanged("Producto_Presentaciones"); }
        }

        private PRODUCTO_PRESENTACION selectedProducto_Presentiacion;

        public PRODUCTO_PRESENTACION SelectedProducto_Presentacion
        {
            get { return selectedProducto_Presentiacion; }
            set
            {
                selectedProducto_Presentiacion = value;
                if (value != null)
                {
                    EliminarHabilitado = true;
                    EditarHabilitado = true;
                    CancelarHabilitado = true;
                }
                RaisePropertyChanged("SelectedProducto_Presentacion");
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
        #endregion

        #region Metodos

        #region Validacion
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCION ES REQUERIDA!");
            RaisePropertyChanged("Descripcion");
        }
        #endregion

        public void CargaCatalogo(string estatus, bool isExceptionManaged = false)
        {
            try
            {
                producto_Presentaciones = new ObservableCollection<PRODUCTO_PRESENTACION>(new cProducto_Presentacion().Seleccionar(estatus).ToList());
                RaisePropertyChanged("Producto_Presentaciones");
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
                Producto_Presentaciones = new ObservableCollection<PRODUCTO_PRESENTACION>(new cProducto_Presentacion().Seleccionar(busqueda.ToString(),SelectedEstatus.CLAVE).ToList());
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
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PRODUCTO_PRESENTACIONPOPUP);
                    setValidationRules();
                    RealizarAccion = AccionSalvar.Salvar;
                    break;
                case "editar":
                    if (!String.IsNullOrEmpty(SelectedProducto_Presentacion.DESCR))
                    {
                        ID = SelectedProducto_Presentacion.ID_PRESENTACION;
                        Descripcion = SelectedProducto_Presentacion.DESCR;
                        Activo = (SelectedProducto_Presentacion.ACTIVO == "S") ? true : false;
                        RealizarAccion = AccionSalvar.Actualizar;
                        SalvarHabilitado = true;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PRODUCTO_PRESENTACIONPOPUP);
                        setValidationRules();
                    }
                    break;
                case "salvar":
                    GuardarProductoTipo();
                    CargaCatalogo(SelectedEstatus.CLAVE);
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTO_PRESENTACIONPOPUP);
                    break;
                case "cancelar":
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PRODUCTO_PRESENTACIONPOPUP);
                    break;
                case "eliminar":
                    await EliminarProductoTipo();
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
                var _producto_presentacion = new PRODUCTO_PRESENTACION
                {
                    ACTIVO = (Activo) ? "S" : "N",
                    DESCR = Descripcion
                };

                if (RealizarAccion == AccionSalvar.Salvar)
                {
                    new cProducto_Presentacion().Insertar(_producto_presentacion);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se inserto la presentación de producto con exito");
                }
                else if (RealizarAccion == AccionSalvar.Actualizar)
                {
                    _producto_presentacion.ID_PRESENTACION = ID.Value;
                    new cProducto_Presentacion().Actualizar(_producto_presentacion);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", string.Format("Se actualizo la presentación de producto {0} con exito", _producto_presentacion.DESCR));

                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
            }
        }

        private async Task EliminarProductoTipo()
        {
            var _error = false;
            try
            {
                if (await _dialogCoordinator.ShowMessageAsync(this, "Confirmación", String.Format("¿Esta seguro de cambiar de estatus a INACTIVO la presentación de producto {0}?", SelectedProducto_Presentacion.DESCR), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    new cProducto_Presentacion().Actualizar(new PRODUCTO_PRESENTACION
                    {
                        ACTIVO = "N",
                        DESCR = SelectedProducto_Presentacion.DESCR,
                        ID_PRESENTACION = SelectedProducto_Presentacion.ID_PRESENTACION
                    });
                    await _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se cambio de estatus la presentación de producto con exito");
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
