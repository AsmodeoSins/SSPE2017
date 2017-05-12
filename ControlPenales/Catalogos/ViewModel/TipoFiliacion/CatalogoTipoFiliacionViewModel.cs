using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;

namespace ControlPenales
{
    partial class CatalogoTipoFiliacionViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public CatalogoTipoFiliacionViewModel()
        {
            EmptyVisible = false;
            //Listado 
            ListItems = new List<TIPO_FILIACION>();
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
            //Obtenemos los TiposFiliaciones
            //ListMediaFiliacion = new ObservableCollection<MEDIA_FILIACION>();
            cMediaFiliacion mediaFiliacion = new cMediaFiliacion();
            ListMediaFiliacion = new List<MEDIA_FILIACION>(mediaFiliacion.ObtenerTodos());
            ListMediaFiliacion.Insert(0, new MEDIA_FILIACION() { ID_MEDIA_FILIACION = -1, DESCR = "SELECCIONE" });
            this.GetMediaFiliacion();
            this.setValidationRules();
        }

        void IPageViewModel.inicializa() { }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetMediaFiliacion();
                    break;
                case "menu_editar":
                    Busqueda = string.Empty;
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Tipo de Filiación";
                        #region OpcionesVisible
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
                        bandera_editar = true;
                        FocusText = true;
                        #endregion
                        /*****************************************/
                        EnableMediaFiliacion = false;
                        Clave = SelectedItem.ID_TIPO_FILIACION;
                        //Corta al campo descripcion los espacios restantes.
                        Descripcion = SelectedItem.DESCR.TrimEnd();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                        ValueMediaFiliacion = SelectedItem.ID_MEDIA_FILIACION;
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
                    HeaderAgregar = "Agregar Tipo de Filiación";
                    Busqueda = string.Empty;
                    #region OpcionesVisible
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
                    EnableMediaFiliacion = true;
                    ValueMediaFiliacion = -1;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    this.GetMediaFiliacion();
                    /********************************/
                    break;
                case "menu_guardar":
                    Busqueda = string.Empty;
                    if (!string.IsNullOrEmpty(Descripcion) && ValueMediaFiliacion != -1 && SelectedEstatus != null)
                    {
                        #region OpcionesVisible
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
                        this.GuardarTipoFiliacion();
                    }
                    else
                        FocusText = true;
                    break;
                case "menu_cancelar":
                    Busqueda = string.Empty;
                    #region OpcionesVisible
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
                    ValueMediaFiliacion = -1;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    /****************************************/
                    break;
                case "menu_eliminar":
                    Busqueda = string.Empty;
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
                            if (EliminarTipoFiliacion())
                            {
                                dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                                await metro.ShowMetroDialogAsync(dialog);
                                await TaskEx.Delay(1500);
                                await metro.HideMetroDialogAsync(dialog);
                            }
                            else
                            {
                                mySettings = new MetroDialogSettings()
                                {
                                    AffirmativeButtonText = "Aceptar"
                                };
                                // dialog = (BaseMetroDialog)metro.Resources["ErrorDialog"];
                                await metro.ShowMessageAsync("Algo ocurrió...", "No se puede eliminar Tipo Filiación: Tiene dependencias.", MessageDialogStyle.Affirmative, mySettings);
                                await TaskEx.Delay(1500);

                            }
                            //await metro.ShowMetroDialogAsync(dialog);
                            //await TaskEx.Delay(1500);
                            //await metro.HideMetroDialogAsync(dialog);
                        }
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opción");

                    break;
                case "menu_exportar":
                    Busqueda = string.Empty;
                    Cambio = string.Empty;
                    break;
                case "menu_ayuda":
                    Busqueda = string.Empty;
                    Cambio = string.Empty;
                    break;
                case "menu_salir":
                    Busqueda = string.Empty;
                    Cambio = string.Empty;
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private void clickTipo(Object obj)
        {
            SelectedItem = (TIPO_FILIACION)obj;
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetMediaFiliacion();
            }
        }

        private void GetMediaFiliacion()
        {
            try
            {
                cTipoFiliacion tipoFiliacion = new cTipoFiliacion();
                ListItems.Clear();
                ListItems = new List<TIPO_FILIACION>(tipoFiliacion.ObtenerTodos(Busqueda));
                ListItems.ForEach((item) => { item.ESTATUS = string.IsNullOrEmpty(item.ESTATUS) ? "N" : item.ESTATUS; });
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

        private void GuardarTipoFiliacion()
        {
            try
            {
                cTipoFiliacion tipoFiliacion = new cTipoFiliacion();
                if (SelectedItem != null)
                {  //Actualizar
                    tipoFiliacion.Actualizar(new TIPO_FILIACION() { ID_TIPO_FILIACION = Clave, DESCR = Descripcion, ESTATUS = SelectedEstatus.CLAVE, ID_MEDIA_FILIACION = ValueMediaFiliacion, PM = 0, ID_VIEJO = 0 });
                }
                else
                {
                    //Agregar
                    tipoFiliacion.Insertar(new TIPO_FILIACION() { DESCR = Descripcion, ID_MEDIA_FILIACION = ValueMediaFiliacion, PM = 0, ID_VIEJO = 0, ESTATUS = SelectedEstatus.CLAVE });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                SelectedEstatus = null;
                SelectedItem = null;
                //Mostrar Listado
                this.GetMediaFiliacion();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool EliminarTipoFiliacion()
        {
            try
            {
                if (SelectedItem != null)
                {
                    cTipoFiliacion tipoFiliacion = new cTipoFiliacion();
                    if (!tipoFiliacion.Eliminar(SelectedItem.ID_TIPO_FILIACION))
                        return false;
                    this.GetMediaFiliacion();
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        private async void TipoFiliacionLoad(CatalogoSimpleView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new List<TIPO_FILIACION>();
                    CatalogoHeader = "Tipo Filiación";
                    HeaderAgregar = "Agregar tipo filiación";
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
                    this.GetMediaFiliacion();
                    this.setValidationRules();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_TIPO_FILIACION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
