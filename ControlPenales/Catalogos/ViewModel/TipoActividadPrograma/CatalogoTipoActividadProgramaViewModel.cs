using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;

namespace ControlPenales
{
    partial class CatalogoTipoActividadProgramaViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        cDepartamentos departamentosControlador = new cDepartamentos();

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetTiposVisitas();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Tipo Actividad Programa";
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
                        Clave = SelectedItem.ID_TIPO_PROGRAMA;
                        Descripcion = SelectedItem.DESCR == null ? SelectedItem.DESCR : SelectedItem.DESCR.TrimEnd();
                        Nombre = SelectedItem.NOMBRE == null ? SelectedItem.NOMBRE : SelectedItem.NOMBRE.TrimEnd();
                        Orden = SelectedItem.ORDEN;
                        SelectedDepartamento = ListaDepartamentos.Where(w => w.ID_DEPARTAMENTO == SelectedItem.ID_DEPARTAMENTO).FirstOrDefault();
                        //departamentosControlador.Obtener((short)SelectedItem.ID_DEPARTAMENTO).FirstOrDefault();
                        //SelectIndexDep = SelectedDepartamento.ID_DEPARTAMENTO;
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).FirstOrDefault();
                        /*****************************************/
                    }
                    else
                    {
                        bandera_editar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una opción.");
                    }
                    break;
                case "menu_agregar":
                    HeaderAgregar = "Agregar Nuevo Tipo Actividad Programa";
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
                    Nombre = string.Empty;
                    Orden = null;
                    SelectedItem = null;
                    SelectedDepartamento = ListaDepartamentos.Where(w => w.ID_DEPARTAMENTO == -1).FirstOrDefault();
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Descripcion))
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
                        this.GuardarTipoVisita();
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
                    Busqueda = string.Empty;
                    this.GetTiposVisitas();
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
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.DESCR + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            BaseMetroDialog dialog;
                            if (this.EliminarTipoVisita())
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
                    Cambio = string.Empty;
                    break;
                case "menu_ayuda":
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    break;
                case "menu_salir":
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private async void TipoActividadProgramaLoad(CatalogoTipoActividadProgramaView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<TIPO_PROGRAMA>();
                    CatalogoHeader = "Tipos de Actividad Programa";
                    HeaderAgregar = "Agregar Nuevo Tipo de Actividad Programa";
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
                    MaxLength = 10;
                    //Capacidad que tiene el cmapo en la bd
                    MaxLenghtOrden = 4;
                    SeleccionIndice = -1;
                    //Obtenemos las Etnias
                    this.GetTiposVisitas();
                    this.GetDeprtamentos();
                    setValidationRules();
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar Tipo Actividad Programa.", ex);
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetTiposVisitas();
            }
        }

        private void GetTiposVisitas()
        {
            try
            {
                ListItems.Clear();
                ListItems = new ObservableCollection<TIPO_PROGRAMA>(new cTipoPrograma().ObtenerTodosLista());
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

        private void GetDeprtamentos()
        {
            try
            {
                ListaDepartamentos = new ObservableCollection<DEPARTAMENTO>(new cDepartamentos().ObtenerTodos(string.Empty,"S"));
                ListaDepartamentos.Insert(0, new DEPARTAMENTO() { ID_DEPARTAMENTO = -1, DESCR = "SELECCIONE" });
                SelectedDepartamento = ListaDepartamentos.Where(w => w.ID_DEPARTAMENTO == -1).FirstOrDefault();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GuardarTipoVisita()
        {
            try
            {
                cTipoPrograma tipo_programa = new cTipoPrograma();
                if (Clave > 0)
                {
                    tipo_programa.Actualizar(new TIPO_PROGRAMA
                    {
                        ID_TIPO_PROGRAMA = SelectedItem.ID_TIPO_PROGRAMA,
                        ID_DEPARTAMENTO = SelectedDepartamento.ID_DEPARTAMENTO,
                        DESCR = Descripcion,
                        ORDEN = Orden,
                        NOMBRE = Nombre,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                else
                {   //Agregar
                    tipo_programa.Insertar(new TIPO_PROGRAMA
                    {
                        ID_TIPO_PROGRAMA = Clave,
                        ID_DEPARTAMENTO = SelectedDepartamento.ID_DEPARTAMENTO,
                        DESCR = Descripcion,
                        ORDEN = Orden,
                        NOMBRE = Nombre,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                Nombre = string.Empty;
                SelectedDepartamento = null;
                SelectedEstatus = null;
                Busqueda = string.Empty;
                //Mostrar Listado
                this.GetTiposVisitas();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool EliminarTipoVisita()
        {
            try
            {
                cTipoPrograma tipo_programa = new cTipoPrograma();
                if (SelectedItem != null)
                {
                    if (!tipo_programa.Eliminar(SelectedItem.ID_TIPO_PROGRAMA))
                        return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    Busqueda = string.Empty;
                    this.GetTiposVisitas();
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_TIPO_ACTIVIDAD_PROGRAMAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
                    if (p.CONSULTAR == 1)
                    {
                        BuscarHabilitado = true;
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
    }
}
