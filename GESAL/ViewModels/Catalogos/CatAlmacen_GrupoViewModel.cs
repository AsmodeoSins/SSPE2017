using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.ViewModels;
using System.ComponentModel;
using GESAL.Clases.Enums;
using System.Collections.ObjectModel;
using SSP.Servidor;
using GESAL.Models;
using SSP.Controlador.Catalogo.Almacenes;
using MVVMShared.Commands;
using System.Windows.Input;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
namespace GESAL.ViewModels
{
    public class CatAlmacen_GrupoViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;
        public CatAlmacen_GrupoViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
        }

        #region Propiedades

        #region Accion del Boton Salvar
        private AccionSalvar realizarAccion;
        public AccionSalvar RealizarAccion
        {
            get { return realizarAccion; }
            set { realizarAccion = value; RaisePropertyChanged("RealizarAccion"); }
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

        private string id;
        public string ID
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

        private ObservableCollection<ALMACEN_GRUPO> almacen_grupos;

        public ObservableCollection<ALMACEN_GRUPO> Almacen_Grupos
        {
            get { return almacen_grupos; }
            set { almacen_grupos = value; RaisePropertyChanged("Almacen_Grupos"); }
        }

        private ALMACEN_GRUPO  selectedAlmacen_Grupo;

        public ALMACEN_GRUPO SelectedAlmacen_Grupo
        {
            get { return selectedAlmacen_Grupo; }
            set
            {
                selectedAlmacen_Grupo = value;
                if (value != null)
                {
                    EliminarHabilitado = true;
                    EditarHabilitado = true;
                    CancelarHabilitado = true;
                }
                RaisePropertyChanged("SelectedAlmacen_Grupo");
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
            base.AddRule(() => ID, () => (!string.IsNullOrEmpty(ID) && ID.Length==1) , "ID ES REQUERIDA Y DE UN CARACTER!");
            RaisePropertyChanged("Descripcion");
            RaisePropertyChanged("ID");
        }
        #endregion

        public void CargaCatalogo(string estatus, bool isExceptionManaged = false)
        {
            try
            {
                almacen_grupos = new ObservableCollection<ALMACEN_GRUPO>(new cAlmacen_Grupo().SeleccionarTodos(string.Empty,string.Empty,estatus).ToList());
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

        public void CargaCatalogo(Object busqueda)
        {
            try
            {
                Almacen_Grupos = new ObservableCollection<ALMACEN_GRUPO>(new cAlmacen_Grupo().SeleccionarTodos(busqueda.ToString(),selectedEstatus.CLAVE).ToList());
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
            }
        }


        private void LlenarEstatusDisponibles()
        {
            lista_estatus = new Estatus_Disponibles();
            var dummy=new Estatus{
                CLAVE="",
                DESCRIPCION="TODOS"
            };
            lista_estatus.LISTA_ESTATUS.Add(dummy);
            RaisePropertyChanged("Lista_Estatus");
        }

        private void limpiarCampos()
        {
            ID = string.Empty;
            Descripcion = string.Empty;
            Activo = true;
        }

        private async void AccionCatalogoSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "agregar":
                    CondicionesIniciales();
                    SalvarHabilitado = true;
                    CancelarHabilitado = true;

                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALMACEN_GRUPOPOPUP);
                    setValidationRules();
                    RealizarAccion = AccionSalvar.Salvar;
                    break;
                case "editar":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALMACEN_GRUPOPOPUP);
                    setValidationRules();
                    ID = SelectedAlmacen_Grupo.ID_ALMACEN_GRUPO;
                    Descripcion = SelectedAlmacen_Grupo.DESCR;
                    Activo = SelectedAlmacen_Grupo.ACTIVO == "S" ? true : false;
                    RealizarAccion = AccionSalvar.Actualizar;
                    SalvarHabilitado = true;
                    break;
                case "salvar":
                    GuardarAlmacen();
                    CargaCatalogo(SelectedEstatus.CLAVE);
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALMACEN_GRUPOPOPUP);
                    break;
                case "cancelar":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALMACEN_GRUPOPOPUP);
                    CondicionesIniciales();
                    break;
                case "eliminar":
                    await EliminarAlmacen_Grupo();
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

        private void GuardarAlmacen()
        {
            try
            {
                var _almacen_grupo = new ALMACEN_GRUPO
                {
                    ACTIVO = (Activo) ? "S" : "N",
                    DESCR = Descripcion,
                    ID_ALMACEN_GRUPO=ID 
                };

                if (RealizarAccion == AccionSalvar.Salvar)
                {
                    new cAlmacen_Grupo().Insertar(_almacen_grupo);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se inserto el grupo de almacen con exito");
                }
                else if (RealizarAccion == AccionSalvar.Actualizar)
                {
                    new cAlmacen_Grupo().Actualizar(_almacen_grupo);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", string.Format("Se actualizo el grupo de almacen {0} con exito", _almacen_grupo.DESCR));

                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
            }
        }

        private async Task EliminarAlmacen_Grupo()
        {
            var _error = false;
            try
            {
                if (await _dialogCoordinator.ShowMessageAsync(this, "Confirmación", String.Format("¿Esta seguro de cambiar el estatus a INACTIVO del grupo de almacen {0}?", SelectedAlmacen_Grupo.DESCR), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    new cAlmacen_Grupo().Actualizar(new ALMACEN_GRUPO
                    {
                        ACTIVO = "N",
                        DESCR=SelectedAlmacen_Grupo.DESCR,
                        ID_ALMACEN_GRUPO=SelectedAlmacen_Grupo.ID_ALMACEN_GRUPO
                    });
                    await _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se cambio el estatus del grupo de almacen con exito");
                }
            }
            catch (Exception ex)
            { _error = true; };
            if (_error)
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
        }

        private bool CanExecute(object parameter) { return base.HasErrors == false; }

        private async void OnLoad(object sender)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Espere un momento...");
            try
            {
                SelectedAlmacen_Grupo = null;
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
