using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    partial class CatalogoDepartamentosViewModel : ValidationViewModelBase
    {
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetDepartamentos();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        ModoEdicion = TipoEdicion.EDITAR;
                        HeaderAgregar = "Editar Departamento";
                        #region visiblePantalla
                        EditarVisible = true;
                        NuevoVisible = false;
                        GuardarMenuEnabled = true;
                        AgregarMenuEnabled = false;
                        EliminarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        CancelarMenuEnabled = true;
                        AyudaMenuEnabled = true;
                        SalirMenuEnabled = true;
                        ExportarMenuEnabled = false;
                        AgregarVisible = true;
                        #endregion
                        bandera_editar = true;
                        FocusText = true;
                        /*****************************************/
                        Clave = SelectedItem.ID_DEPARTAMENTO;
                        Descripcion = SelectedItem.DESCR.TrimEnd();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                        if (SelectedItem.ID_ROL.HasValue)
                            SelectedTipoValue = SelectedItem.ID_ROL.Value;
                        else
                            SelectedTipoValue = -1;
                        /*****************************************/
                        this.setValidationRules();
                    }
                    else
                    {
                        bandera_editar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una opción.");
                    }
                    break;
                case "menu_agregar":
                    ModoEdicion = TipoEdicion.NUEVO;
                    HeaderAgregar = "Agregar Nuevo Departamento";
                    #region visiblePantalla
                    EditarVisible = false;
                    NuevoVisible = true;
                    GuardarMenuEnabled = true;
                    AgregarMenuEnabled = false;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = true;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = false;
                    AgregarVisible = true;
                    #endregion
                    bandera_editar = false;
                    FocusText = true;
                    /********************************/
                    SeleccionIndice = -1;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    SelectedTipoValue = -1;
                    this.setValidationRules();
                    Tipo = 0;
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Descripcion) && SelectedEstatus != null)
                    {
                        #region visiblePantalla
                        EditarVisible = false;
                        NuevoVisible = false;
                        GuardarMenuEnabled = false;
                        AgregarMenuEnabled = true;
                        EliminarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        AyudaMenuEnabled = true;
                        SalirMenuEnabled = true;
                        ExportarMenuEnabled = true;
                        AgregarVisible = false;
                        #endregion
                        /**********************************/
                        this.GuardarDepartamento();
                        /**********************************/
                    }
                    else
                        FocusText = true;
                    break;
                case "menu_cancelar":
                    #region visiblePantalla
                    NuevoVisible = false;
                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    AgregarVisible = false;
                    #endregion
                    /****************************************/
                    SeleccionIndice = -1;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    Tipo = 0;
                    this.GetDepartamentos();
                    /****************************************/
                    break;
                case "menu_eliminar":
                    var metro = Application.Current.Windows[0] as MetroWindow;
                    if (SelectedItem != null)
                    {
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Aceptar",
                            NegativeButtonText = "Cancelar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        var result = await metro.ShowMessageAsync("Eliminar", "¿Está seguro que desea eliminar... [ " + SelectedItem.DESCR + " ]?", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            BaseMetroDialog dialog;
                            if (this.eliminarDepartamento())
                            {
                                dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                            }
                            else
                            {
                                dialog = (BaseMetroDialog)metro.Resources["ErrorDialog"];
                            }
                            await metro.ShowMetroDialogAsync(dialog);
                            await TaskEx.Delay(1500);
                            await metro.HideMetroDialogAsync(dialog);
                        }
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opción");
                    SeleccionIndice = -1;
                    break;
                case "menu_exportar":
                    SeleccionIndice = -1;
                    break;
                case "menu_ayuda":
                    SeleccionIndice = -1;
                    break;
                case "menu_salir":
                    SeleccionIndice = -1;
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        //private void CargarEstatus()
        //{
        //    Lista_Estatus.LISTA_ESTATUS.Insert(0, new Clases.Estatus.Estatus { CLAVE = "-1", DESCRIPCION = "SELECCIONAR" });
        //    RaisePropertyChanged("Lista_Estatus");
        //}

        private void CargarRoles(bool isExceptionManaged)
        {
            try
            {
                ListTipos = new ObservableCollection<SISTEMA_ROL>(new cSistemaRol().ObtenerTodos());
                ListTipos.Add(new SISTEMA_ROL
                {
                    ID_ROL = -1,
                    DESCR = "SELECCIONAR"
                });
                RaisePropertyChanged("ListTipos");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de departamentos", ex);
                else
                    throw ex;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetDepartamentos();
            }
        }

        private void GetDepartamentos()
        {
            try
            {
                cDepartamentos departamento = new cDepartamentos();
                ListItems.Clear();
                ListItems = new ObservableCollection<DEPARTAMENTO>(departamento.ObtenerTodos(Busqueda,string.Empty));
                if (ListItems.Count > 0)
                    EmptyVisible = false;
                else
                    EmptyVisible = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GuardarDepartamento()
        {
            try
            {
                cDepartamentos departamento = new cDepartamentos();
                if (Clave > 0)
                {
                    //Actualizar
                    departamento.Actualizar(new DEPARTAMENTO
                    {
                        ID_DEPARTAMENTO = SelectedItem.ID_DEPARTAMENTO,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE,
                        ID_ROL = SelectedTipoValue
                    });
                }
                else
                {   //Agregar
                    departamento.Insertar(new DEPARTAMENTO
                    {
                        ID_DEPARTAMENTO = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE,
                        ID_ROL = SelectedTipoValue
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                SelectedEstatus = null;
                Tipo = 0;
                //Mostrar Listado
                this.GetDepartamentos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool eliminarDepartamento()
        {
            try
            {
                if (SelectedItem != null)
                {
                    cDepartamentos departamento = new cDepartamentos();
                    if (!departamento.Eliminar(SelectedItem.ID_DEPARTAMENTO))
                        return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    Tipo = 0;
                    this.GetDepartamentos();
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        //LOAD
        private async void DepartamentosLoad(CatalogoDepartamentosView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<DEPARTAMENTO>();
                    CatalogoHeader = "Departamentos";
                    HeaderAgregar = "Agregar Departamentos";
                    //LLENAR 
                    EditarVisible = false;
                    NuevoVisible = false;
                    AgregarVisible = false;
                    GuardarMenuEnabled = false;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    /*MAXLENGTH*/
                    MaxLength = 14;
                    SeleccionIndice = -1;
                    //Obtenemos las celdas
                    this.GetDepartamentos();
                    ConfiguraPermisos();
                    //CargarEstatus();
                    CargarRoles(true);
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de departamento.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_DEPARTAMENTOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
                    if (p.CONSULTAR == 1)
                    {
                        BuscarHabilitado = true;
                        CentroHabilitado = true;
                        TextoHabilitado = true;
                    }
                    if (p.EDITAR == 1)
                        EditarEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        private enum TipoEdicion
        {
            NUEVO = 1,
            EDITAR = 2
        }
    }
}