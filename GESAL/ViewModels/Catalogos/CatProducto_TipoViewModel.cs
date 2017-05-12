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
    class CatProducto_TipoViewModel:ValidationViewModelBase,IDataErrorInfo
    {

        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;
        public CatProducto_TipoViewModel(IDialogCoordinator dialogCoordinator, Usuario usuario)
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

        private ObservableCollection<ALMACEN_GRUPO> almacen_Grupos;

        public ObservableCollection<ALMACEN_GRUPO> Almacen_Grupos
        {
            get { return almacen_Grupos; }
            set { almacen_Grupos = value; RaisePropertyChanged("Almacen_Grupos"); }
        }

        private ALMACEN_GRUPO selectedAlmacen_Grupo=null;
        public ALMACEN_GRUPO SelectedAlmacen_Grupo
        {
            get { return selectedAlmacen_Grupo; }
            set { selectedAlmacen_Grupo = value; RaisePropertyChanged("SelectedAlmacen_Grupo"); }
        }
       
        private ObservableCollection<ALMACEN_TIPO_CAT> almacen_Tipos;

        public ObservableCollection<ALMACEN_TIPO_CAT> Almacen_Tipos
        {
            get { return almacen_Tipos; }
            set { almacen_Tipos = value; RaisePropertyChanged("Almacen_Tipos"); }
        }

        private ALMACEN_TIPO_CAT selectedAlmacen_Tipo;

        public ALMACEN_TIPO_CAT SelectedAlmacen_Tipo
        {
            get { return selectedAlmacen_Tipo; }
            set
            {
                selectedAlmacen_Tipo = value;
                if (value != null)
                {

                    EliminarHabilitado = true;
                    EditarHabilitado = true;
                    CancelarHabilitado = true;
                }
                else
                {
                    CondicionesIniciales();
                }
                RaisePropertyChanged("SelectedAlmacen_Tipo");
            }
        }

        private string busquedaParametro = string.Empty;

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
            base.AddRule(() => Descripcion, () => !string.IsNullOrEmpty(Descripcion), "DESCRIPCION ES REQUERIDA!");
            base.AddRule(() => SelectedAlmacen_Grupo, () => !(SelectedAlmacen_Grupo==null), "ES NECESARIO DEFINIR QUE GRUPO DE ALMACEN ES");
            RaisePropertyChanged("Descripcion");
            RaisePropertyChanged("SelectedAlmacen_Grupo");
            
        }
        #endregion

        public void CargaCatalogo(string estatus, bool isExceptionManaged=false)
        {
            try
            {
                almacen_Tipos = new ObservableCollection<ALMACEN_TIPO_CAT>(new cAlmacen_Tipo_Cat().Seleccionar(_usuario.Almacen_Grupo,estatus).ToList());
                RaisePropertyChanged("Almacen_Tipos");
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
            var isExceptionManaged = params1[1].ToString();
            try
            {
                Almacen_Tipos = new ObservableCollection<ALMACEN_TIPO_CAT>(new cAlmacen_Tipo_Cat().Seleccionar(busqueda.ToString(), _usuario.Almacen_Grupo,SelectedEstatus.CLAVE).ToList());
            }
            catch (Exception ex)
            {
                if (isExceptionManaged!="False")
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public void CargarAlmacen_Grupo(string id_almacen_grupo, bool isExceptionManaged = false)
        {
            try
            {
                almacen_Grupos = new ObservableCollection<ALMACEN_GRUPO>(new cAlmacen_Grupo().SeleccionarTodos(string.Empty,id_almacen_grupo,"S").ToList());
                RaisePropertyChanged("Almacen_Grupos");
            }catch(Exception ex)
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
            if (string.IsNullOrWhiteSpace(_usuario.Almacen_Grupo))
                SelectedAlmacen_Grupo = Almacen_Grupos.FirstOrDefault(w => w.ID_ALMACEN_GRUPO == "");
            else
                SelectedAlmacen_Grupo = Almacen_Grupos.FirstOrDefault(w => w.ID_ALMACEN_GRUPO == _usuario.Almacen_Grupo);
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
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALMACEN_TIPOPOPUP);
                    setValidationRules();
                    RealizarAccion = AccionSalvar.Salvar;
                    break;
                case "editar":
                    if (!String.IsNullOrEmpty(SelectedAlmacen_Tipo.DESCR))
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALMACEN_TIPOPOPUP);
                        setValidationRules();
                        ID = SelectedAlmacen_Tipo.ID_ALMACEN_TIPO;
                        Descripcion = SelectedAlmacen_Tipo.DESCR;
                        Activo = (SelectedAlmacen_Tipo.ACTIVO == "S") ? true : false;
                        SelectedAlmacen_Grupo = Almacen_Grupos.FirstOrDefault(w => w.ID_ALMACEN_GRUPO == SelectedAlmacen_Tipo.ID_ALMACEN_GRUPO);
                        RealizarAccion = AccionSalvar.Actualizar;
                        SalvarHabilitado = true;
                    }
                    break;
                case "salvar":
                    GuardarAlmacenTipo();
                    CargaCatalogo(SelectedEstatus.CLAVE);
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALMACEN_TIPOPOPUP);
                    break;
                case "cancelar":
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALMACEN_TIPOPOPUP);
                    break;
                case "eliminar":
                    await EliminarAlmacenTipo();
                    CargaCatalogo(SelectedEstatus.CLAVE);
                    CondicionesIniciales();
                    break;
            }
        }

        private void CondicionesIniciales()
        {
            //AgregarHabilitado = true;
            SalvarHabilitado = false;
            CancelarHabilitado = false;
            EditarHabilitado = false;
            EliminarHabilitado = false;
            ClearRules();
            limpiarCampos();
        }

        private void GuardarAlmacenTipo()
        {
            try
            {
                var _almacen_tipo_cat = new ALMACEN_TIPO_CAT
                {
                    ACTIVO = (Activo) ? "S" : "N",
                    DESCR = Descripcion,
                    ID_ALMACEN_GRUPO = SelectedAlmacen_Grupo.ID_ALMACEN_GRUPO
                };

                if (RealizarAccion == AccionSalvar.Salvar)
                {
                    new cAlmacen_Tipo_Cat().Insertar(_almacen_tipo_cat);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se inserto el tipo de almacen con exito");
                }
                else if (RealizarAccion == AccionSalvar.Actualizar)
                {
                    _almacen_tipo_cat.ID_ALMACEN_TIPO = ID.Value;
                    new cAlmacen_Tipo_Cat().Actualizar(_almacen_tipo_cat);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", string.Format("Se actualizo el tipo de almacen {0} con exito", _almacen_tipo_cat.DESCR));

                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
            }
        }

        private async Task EliminarAlmacenTipo()
        {
            var _error = false;
            try
            {
                if (await _dialogCoordinator.ShowMessageAsync(this, "Confirmación",String.Format("¿Esta seguro de cambiar el estatus a INACTIVO del tipo de almacen {0}?",SelectedAlmacen_Tipo.DESCR), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {

                    new cAlmacen_Tipo_Cat().Actualizar(new ALMACEN_TIPO_CAT
                    {
                        ACTIVO = "N",
                        DESCR = SelectedAlmacen_Tipo.DESCR,
                        ID_ALMACEN_TIPO = SelectedAlmacen_Tipo.ID_ALMACEN_TIPO,
                        ID_ALMACEN_GRUPO = SelectedAlmacen_Tipo.ID_ALMACEN_GRUPO
                    });
                    await _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se cambio el estatus del tipo de almacen con exito");
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
                        CargarAlmacen_Grupo(_usuario.Almacen_Grupo,true);
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
