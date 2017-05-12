using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMShared.ViewModels;
using System.Collections.ObjectModel;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Almacenes;
using SSP.Controlador.Catalogo.Justicia;
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
    public class CatAlmacenViewModel:ValidationViewModelBase,IDataErrorInfo
    {
        private IDialogCoordinator _dialogCoordinator;
        private Usuario _usuario;
        public CatAlmacenViewModel(IDialogCoordinator dialogCoordinator,Usuario usuario)
        {
            _usuario = usuario;
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

        private ObservableCollection<ALMACEN> almacenes;

        public ObservableCollection<ALMACEN> Almacenes
        {
            get { return almacenes; }
            set { almacenes = value; RaisePropertyChanged("Almacenes"); }
        }

        private ALMACEN  selectedAlmacen;

        public ALMACEN SelectedAlmacen
        {
            get { return selectedAlmacen; }
            set
            {
                selectedAlmacen = value;
                if (value != null)
                {
                    EliminarHabilitado = true;
                    EditarHabilitado = true;
                   
                    CancelarHabilitado = true;

                   
                }
                RaisePropertyChanged("SelectedAlmacen");
            }
        }

        private ObservableCollection<CENTRO> centros;

        public ObservableCollection<CENTRO> Centros
        {
            get { return centros; }
            set { centros = value; RaisePropertyChanged("Centros"); }
        }

        private CENTRO selectedCentro;
        public CENTRO SelectedCentro
        {
            get { return selectedCentro; }
            set { selectedCentro = value; RaisePropertyChanged("SelectedCentro"); }
        }

        private ObservableCollection<CENTRO> centrosVistaDetalle;

        public ObservableCollection<CENTRO> CentrosVistaDetalle
        {
            get { return centrosVistaDetalle; }
            set { centrosVistaDetalle = value; RaisePropertyChanged("CentrosVistaDetalle"); }
        }

        private CENTRO selectedCentroVistaDetalle;
        public CENTRO SelectedCentroVistaDetalle
        {
            get { return selectedCentroVistaDetalle; }
            set { selectedCentroVistaDetalle = value; RaisePropertyChanged("SelectedCentroVistaDetalle"); }
        }

        private ObservableCollection<ALMACEN_TIPO_CAT> almacen_Tipos_Cat;

        public ObservableCollection<ALMACEN_TIPO_CAT> Almacen_Tipos_Cat
        {
            get { return almacen_Tipos_Cat; }
            set { almacen_Tipos_Cat = value; RaisePropertyChanged("Almacen_Tipos_Cat"); }
        }

        private ALMACEN_TIPO_CAT selectedAlmacen_Tipo_Cat;
        public ALMACEN_TIPO_CAT SelectedAlmacen_Tipo_Cat
        {
            get { return selectedAlmacen_Tipo_Cat; }
            set { selectedAlmacen_Tipo_Cat = value; RaisePropertyChanged("SelectedAlmacen_Tipo_Cat"); }
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
            base.AddRule(() => SelectedCentroVistaDetalle, () => !(SelectedCentroVistaDetalle.ID_CENTRO==-1), "SELECCIONE UN CENTRO!");
            base.AddRule(() => SelectedAlmacen_Tipo_Cat, () => !(SelectedAlmacen_Tipo_Cat.ID_ALMACEN_TIPO == -1), "SELECCIONE UN TIPO!");
            RaisePropertyChanged("Descripcion");
            RaisePropertyChanged("SelectedCentroVistaDetalle");
            RaisePropertyChanged("SelectedAlmacen_Tipo_Cat");
        }
        #endregion

        public void CargaCatalogo(string estatus, bool isExceptionManaged = false)
        {
            try
            {
                almacenes = new ObservableCollection<ALMACEN>(new cAlmacen().Seleccionar(estatus,_usuario.Almacen_Grupo).ToList());
                RaisePropertyChanged("Almacenes");
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
                Almacenes = new ObservableCollection<ALMACEN>(new cAlmacen().Seleccionar(busqueda.ToString(),SelectedCentro.ID_CENTRO,_usuario.Almacen_Grupo,selectedEstatus.CLAVE).ToList());
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar el catalogo. Favor de contactar al administrador");
            }
        }

        public void CargarCentros(bool isExceptionManaged = false)
        {
            try
            {
                centros = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos().ToList());
                var dummy = new CENTRO
                {
                    ID_CENTRO = -1,
                    DESCR = "TODOS"
                };
                centros.Insert(0, dummy);
                RaisePropertyChanged("Centros");                
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los centros. Favor de contactar al administrador");
                else
                    throw ex;
            }
        }

        public void CargarCentrosVistaDetalle()
        {
            try
            {
                CentrosVistaDetalle = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos().ToList());
                var dummy = new CENTRO
                {
                    ID_CENTRO = -1,
                    DESCR = "Selecciona uno"
                };
                CentrosVistaDetalle.Insert(0, dummy);
                SelectedCentroVistaDetalle = dummy;

            }catch(Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los centros. Favor de contactar al administrador");
            }
        }

        public void CargarAlmacen_Tipo_Cat()
        {
            try
            {
                
                var almacen_grupo = _usuario.Almacen_Grupo;
                Almacen_Tipos_Cat = new ObservableCollection<ALMACEN_TIPO_CAT>(new cAlmacen_Tipo_Cat().Seleccionar(almacen_grupo, "S").ToList());
                var dummy = new ALMACEN_TIPO_CAT
                {
                    ID_ALMACEN_TIPO=-1,
                    DESCR="Selecciona uno",
                    ID_ALMACEN_GRUPO = ""
                };
                Almacen_Tipos_Cat.Insert(0, dummy);
                SelectedAlmacen_Tipo_Cat = dummy;
            }catch(Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Hubo un error al cargar los tipos. Favor de contactar al administrador");
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
                    CargarCentrosVistaDetalle();
                    CargarAlmacen_Tipo_Cat();
                    SalvarHabilitado = true;
                    CancelarHabilitado = true;

                    //VistaDetalleEsVisible = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALMACEN_POPUP);
                    setValidationRules();
                    RealizarAccion = AccionSalvar.Salvar;
                    break;
                case "editar":
                    if (!String.IsNullOrEmpty(SelectedAlmacen.DESCRIPCION))
                    {
                        CargarCentrosVistaDetalle();
                        CargarAlmacen_Tipo_Cat();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALMACEN_POPUP);
                        setValidationRules();
                        ID = SelectedAlmacen.ID_ALMACEN;
                        Descripcion = SelectedAlmacen.DESCRIPCION;
                        Activo = (SelectedAlmacen.ACTIVO == "S") ? true : false;
                        SelectedCentroVistaDetalle = CentrosVistaDetalle.FirstOrDefault(f => f.ID_CENTRO == SelectedAlmacen.ID_CENTRO);
                        if (SelectedAlmacen.ID_PRODUCTO_TIPO.HasValue)
                            SelectedAlmacen_Tipo_Cat = Almacen_Tipos_Cat.FirstOrDefault(f => f.ID_ALMACEN_TIPO == SelectedAlmacen.ID_PRODUCTO_TIPO.Value);
                        RealizarAccion = AccionSalvar.Actualizar;
                        SalvarHabilitado = true;
                    }
                    break;
                case "salvar":
                    GuardarAlmacen();
                    CargaCatalogo(SelectedEstatus.CLAVE);
                    CondicionesIniciales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALMACEN_POPUP);
                    break;
                case "cancelar":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALMACEN_POPUP);
                    CondicionesIniciales();
                    break;
                case "eliminar":
                    await EliminarAlmacen();
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
                var _almacen = new ALMACEN
                {
                    ACTIVO = (Activo) ? "S" : "N",
                    DESCRIPCION = Descripcion,
                    ID_CENTRO=SelectedCentroVistaDetalle.ID_CENTRO,
                    ID_PRODUCTO_TIPO=SelectedAlmacen_Tipo_Cat.ID_ALMACEN_TIPO
                };

                if (RealizarAccion == AccionSalvar.Salvar)
                {
                    new cAlmacen().Insertar(_almacen);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se inserto el almacen con exito");
                }
                else if (RealizarAccion == AccionSalvar.Actualizar)
                {
                    _almacen.ID_ALMACEN = ID.Value;
                    new cAlmacen().Actualizar(_almacen);
                    _dialogCoordinator.ShowMessageAsync(this, "Notificacion", string.Format("Se actualizo el almacen {0} con exito", _almacen.DESCRIPCION));

                }
            }
            catch (Exception ex)
            {
                _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
            }
        }

        private async Task EliminarAlmacen()
        {
            var _error = false;
            try
            {
                if (await _dialogCoordinator.ShowMessageAsync(this, "Confirmación", String.Format("¿Esta seguro de cambiar el estatus a INACTIVO del almacen {0}?", SelectedAlmacen.DESCRIPCION), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    new cAlmacen().Actualizar(new ALMACEN {
                        ACTIVO="N",
                        DESCRIPCION=selectedAlmacen.DESCRIPCION,
                        ID_ALMACEN=selectedAlmacen.ID_ALMACEN,                        
                        ID_CENTRO=selectedAlmacen.ID_CENTRO,
                        ID_PRODUCTO_TIPO=selectedAlmacen.ID_PRODUCTO_TIPO
                    });
                    await _dialogCoordinator.ShowMessageAsync(this, "Notificacion", "Se elimino el almacen con exito");
                }
            }
            catch (Exception ex)
            { _error = true; };
            if (_error)
                await _dialogCoordinator.ShowMessageAsync(this, "Error", "Ocurrió un error en la operacion. Favor de notificar al administrador");
        }

        private bool CanDownload(object parameter) { return base.HasErrors == false; }

        private async void OnLoad(object sender)
        {
            var _error = false;
            StaticSourcesViewModel.ShowProgressLoading("Espere un momento...");
            try
            {
                await Task.Factory.StartNew(() => {
                    CargarCentros(true);
                    LlenarEstatusDisponibles();
                    CargaCatalogo("S",true);
                })
                .ContinueWith((prevTask) => {
                    SelectedCentro = Centros.First(w=>w.ID_CENTRO==-1);
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

                return accionCatalogo ?? (accionCatalogo = new RelayCommand(AccionCatalogoSwitch, CanDownload));
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
