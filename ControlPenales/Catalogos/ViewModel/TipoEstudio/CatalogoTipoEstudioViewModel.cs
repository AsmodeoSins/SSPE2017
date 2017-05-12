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

namespace ControlPenales
{
    partial class CatalogoTipoEstudioViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public CatalogoTipoEstudioViewModel()
        {
            //EmptyVisible = false;
            ////Listado 
            //ListItems = new List<TIPO_DELITO>();

            //CatalogoHeader = "Tipo de Delito";
            //HeaderAgregar = "Agregar Nuevo Tipo de Delito";
            ////LLENAR 
            //EditarVisible = false;
            //NuevoVisible = false;
            //AgregarVisible = false;
            //GuardarMenuEnabled = false;
            //AgregarMenuEnabled = true;
            //EliminarMenuEnabled = false;
            //EditarMenuEnabled = false;
            //CancelarMenuEnabled = false;
            //AyudaMenuEnabled = true;
            //SalirMenuEnabled = true;
            //ExportarMenuEnabled = true;

            ///*MAXLENGTH*/
            //MaxLength = 100;
            //SeleccionIndice = -1;

            ////Obtenemos las Etnias
            //this.GetTiposDelitos();
            //this.setValidationRules();
        }

        void IPageViewModel.inicializa() { }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetTiposEstudio();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Tipo de Estudio";
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
                        Clave = SelectedItem.ID_TIPO;
                        Descripcion = SelectedItem.DESCR;
                        /*****************************************/
                        PopularEstatusCombo();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
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
                    HeaderAgregar = "Agregar Nuevo Tipo de Estudio";
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
                        this.GuardarTipoEstudio();
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
                    this.GetTiposEstudio();
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
                            if (EliminarTipoEstudio())
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

        private async void TipoEstudioLoad(CatalogoSimpleView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new List<PERSONALIDAD_TIPO_ESTUDIO>();
                    CatalogoHeader = "Tipo de Estudio";
                    HeaderAgregar = "Agregar Nuevo Tipo de Estudio";
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
                    SeleccionIndice = -1;
                    //Obtenemos las Etnias
                    this.GetTiposEstudio();
                    this.setValidationRules();
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de Estudio.", ex);
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetTiposEstudio();
            }
        }

        private void GetTiposEstudio()
        {
            try
            {
                cPersonalidadTipoEstudio tipoEstudio = new cPersonalidadTipoEstudio();
                ListItems.Clear();
                ListItems = tipoEstudio.ObtenerTodos(Busqueda).ToList();
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

        private void GuardarTipoEstudio()
        {
            try
            {
                cPersonalidadTipoEstudio tipoEstudio = new cPersonalidadTipoEstudio();
                if (Clave > 0)
                {  //Actualizar
                    //SelectedItem.DESCR = Descripcion;
                    //tipoEstudio.Actualizar(SelectedItem);

                    tipoEstudio.Actualizar(new PERSONALIDAD_TIPO_ESTUDIO
                    {
                        ID_TIPO = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                else
                {   //Agregar
                    tipoEstudio.Insertar(new PERSONALIDAD_TIPO_ESTUDIO
                    {
                        ID_TIPO = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                Busqueda = string.Empty;
                SelectedEstatus = null;
                //Mostrar Listado
                this.GetTiposEstudio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool EliminarTipoEstudio()
        {
            try
            {
                //if (SelectedItem != null || SelectedItem.ID_TIPO >= 100)
                if (SelectedItem != null)
                {
                    cPersonalidadTipoEstudio tipoEstudio = new cPersonalidadTipoEstudio();
                    if (!tipoEstudio.Eliminar(SelectedItem.ID_TIPO))
                        return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    Busqueda = string.Empty;
                    this.GetTiposEstudio();
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_TIPO_ESTUDIO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
