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
using System.Collections.Generic;
namespace ControlPenales
{
    partial class CatalogoPandillaViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public CatalogoPandillaViewModel()
        {
            EmptyVisible = false;
            //Objeto Pandilla
            Pandilla = new PANDILLA();
            //Listado 
            ListItems = new List<PANDILLA>();
            CatalogoHeader = "Pandilla";
            HeaderAgregar = "Agregar Pandilla";
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
            MaxLength = 50;
            SeleccionIndice = -1;
            //Obtenemos las pandilla
            this.GetPandilla();
            this.setValidationRules();
        }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetPandilla();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Pandilla";
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
                        Pandilla = SelectedItem;
                        Clave = SelectedItem.ID_PANDILLA;
                        Nombre = Pandilla.NOMBRE.TrimEnd();
                        Ubicacion = string.IsNullOrEmpty(Pandilla.UBICACION) ? string.Empty : Pandilla.UBICACION.TrimEnd();
                        Observaciones = string.IsNullOrEmpty(Pandilla.OBSERV) ? string.Empty : Pandilla.OBSERV.TrimEnd();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
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
                    HeaderAgregar = "Agregar Pandilla";
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
                    Pandilla = null;
                    Clave = 0;
                    Pandilla = new PANDILLA();
                    Nombre = Ubicacion = string.Empty;
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Observaciones) && !string.IsNullOrEmpty(Nombre) && !string.IsNullOrEmpty(Ubicacion) && SelectedEstatus != null)
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
                        this.GuardarPandilla();
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
                    Pandilla = null;
                    Clave = 0;
                    Pandilla = new PANDILLA();
                    Nombre = Ubicacion = string.Empty;
                    SelectedEstatus = null;
                    this.GetPandilla();
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
                        var result = await metro.ShowMessageAsync("Eliminar", "¿Está seguro que desea eliminar... [ " + SelectedItem.NOMBRE + " ]?", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            BaseMetroDialog dialog;
                            if (this.EliminarPandilla())
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
                    Nombre = Ubicacion = string.Empty;
                    Cambio = string.Empty;
                    break;
                case "menu_salir":
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    Nombre = Ubicacion = string.Empty;
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetPandilla();
            }
        }

        private void GetPandilla()
        {
            try
            {
                ListItems.Clear();
                ListItems = new List<PANDILLA>(new cPandilla().ObtenerTodos(Busqueda));
                if (ListItems.Count > 0)
                    EmptyVisible = false;
                else
                    EmptyVisible = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener tipos de pandilla.", ex);
            }
        }

        private void GuardarPandilla()
        {
            try
            {
                cPandilla pandilla = new cPandilla();
                if (SelectedItem != null)
                {  //Actualizar
                    SelectedItem.ID_PANDILLA = Clave;
                    SelectedItem.NOMBRE = Nombre;
                    SelectedItem.UBICACION = Ubicacion;
                    SelectedItem.OBSERV = Observaciones;
                    SelectedItem.ESTATUS = SelectedEstatus.DESCRIPCION == "ACTIVO" ? "S" : "N";
                    pandilla.Actualizar(new PANDILLA
                    {
                        ID_PANDILLA = Clave,
                        NOMBRE = Nombre,
                        UBICACION = Ubicacion,
                        OBSERV = Observaciones,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                else
                {   //Agregar
                    pandilla.Insertar(new PANDILLA
                    {
                        ID_PANDILLA = Clave,
                        NOMBRE = Nombre,
                        UBICACION = Ubicacion,
                        OBSERV = Observaciones,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                //Limpiamos las variables
                //Clave = 0;
                Nombre = string.Empty;
                Clave = 0;
                Ubicacion = string.Empty;
                Observaciones = string.Empty;
                SelectedEstatus = null;
                Busqueda = string.Empty;
                //Mostrar Listado
                this.GetPandilla();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool EliminarPandilla()
        {
            try
            {
                if (SelectedItem != null)
                {
                    cPandilla pandilla = new cPandilla();
                    if (!pandilla.Eliminar(SelectedItem.ID_PANDILLA))
                        return false;
                    Pandilla = null;
                    Clave = 0;
                    Pandilla = new PANDILLA();
                    Busqueda = string.Empty;
                    Nombre = Ubicacion = string.Empty;
                    SelectedEstatus = null;
                    this.GetPandilla();
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        private async void PandillasTipoLoad(CatalogoPandillaView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new List<PANDILLA>();
                    CatalogoHeader = "Tipo de Pandilla";
                    HeaderAgregar = "Agregar Tipo de Pandilla";
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
                    //Obtenemos las Etnias
                    this.GetPandilla();
                    this.setValidationRules();
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de pandilla.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_PANDILLAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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