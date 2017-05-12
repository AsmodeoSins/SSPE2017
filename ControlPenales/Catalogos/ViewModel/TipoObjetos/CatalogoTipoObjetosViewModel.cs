using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using ControlPenales;

namespace ControlPenales
{
    partial class CatalogoTipoObjetosViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        cObjetoTipo objetotipo = new cObjetoTipo();

        void IPageViewModel.inicializa() { }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetTipoObjetosTipos();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Tipo Objeto";
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
                        Clave = SelectedItem.ID_OBJETO_TIPO;
                        Descripcion = SelectedItem.DESCR;
                        Configuracion = SelectedItem.CONFIGURACION;
                        Permitido = SelectedItem.PERMITIDO;
                        /*****************************************/
                        PopularEstatusCombo();
                        var EstatusSeleccionado = SelectedItem.ESTATUS == null ? "N" : SelectedItem.ESTATUS;
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == EstatusSeleccionado).SingleOrDefault();
                        setValidationRules();
                        //TODO:Falta Agregar campo estatus
                        //SelectedEstatus=ListEstatus.Where(w=>w.CLAVE==selectedItem.Id_Estatus)
                    }
                    else
                    {
                        bandera_editar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una opción.");
                    }
                    break;
                case "menu_agregar":
                    HeaderAgregar = "Agregar Nuevo Tipo de Objeto";
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
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    SelectedItem = null;
                    Descripcion = string.Empty;
                    Permitido = "N";
                    Configuracion = null;
                    PopularEstatusCombo();
                    setValidationRules();
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
                        this.GuardarObjetoTipo();
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
                    Permitido = "N";
                    this.GetTipoObjetosTipos();
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
                            if (EliminarTipoObjeto())
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

        private void PopularEstatusCombo()
        {
            //var ListaEstatus_ = new Clases.Estatus.EstatusControl().LISTA_ESTATUS;
            //ListEstatus = new ObservableCollection<ControlPenales.Clases.Estatus.Estatus>(ListaEstatus_);
        }

        private async void TipoObjetosLoad(CatalogoTipoObjetoView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new List<OBJETO_TIPO>();
                    CatalogoHeader = "Tipo de Objeto";
                    HeaderAgregar = "Agregar Nuevo Tipo de Objeto";
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
                    MaxLength = 100;
                    MaxLengthConfig = 1;
                    SeleccionIndice = -1;
                    //Obtenemos las Etnias
                    this.GetTipoObjetosTipos();
                    this.setValidationRules();
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de Objeto.", ex);
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetTipoObjetosTipos();
            }
        }

        private void GetTipoObjetosTipos()
        {
            try
            {
                ListItems.Clear();
                ListItems = new cObjetoTipo().ObtenerTodos(Busqueda).ToList();
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

        private void GuardarObjetoTipo()
        {
            try
            {
                if (Clave > 0)
                {  //Actualizar
                    //SelectedItem.DESCR = Descripcion;
                    //tipoEstudio.Actualizar(SelectedItem);

                    objetotipo.Actualizar(new OBJETO_TIPO
                    {
                        ID_OBJETO_TIPO = Clave,
                        CONFIGURACION = Configuracion,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE,
                        PERMITIDO = Permitido
                    });
                }
                else
                {
                    //Agregar
                    objetotipo.Insertar(new OBJETO_TIPO
                    {
                        ID_OBJETO_TIPO = Clave,
                        DESCR = Descripcion,
                        CONFIGURACION = Configuracion,
                        ESTATUS = SelectedEstatus.CLAVE,
                        PERMITIDO = Permitido
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                Busqueda = string.Empty;
                Permitido = "N";
                //  Abreviatura = string.Empty;
                SelectedEstatus = null;
                //Mostrar Listado
                this.GetTipoObjetosTipos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool EliminarTipoObjeto()
        {
            try
            {
                //if (SelectedItem != null || SelectedItem.ID_TIPO >= 100)
                if (SelectedItem != null)
                {
                    //cDrogaUM unidadmedida = new cDrogaUM();
                    // if (!unidadmedida.Eliminar(SelectedItem.ID_UNIDAD_MEDIDA))
                    return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    Busqueda = string.Empty;
                    Permitido = "N";
                    // Abreviatura = string.Empty;
                    this.GetTipoObjetosTipos();
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_OBJETOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
