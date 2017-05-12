using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    partial class CatalogoCampañiasViewModel : ValidationViewModelBase, IPageViewModel
    {
        cCompania obj = new cCompania();

        public CatalogoCampañiasViewModel()
        {
            CatalogoHeader = "Compañias";
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
            MaxLength = 25;
            SeleccionIndice = -1;
            EmptyVisible = false;
            //Listado 
            ListItems = new ObservableCollection<COMPANIA>();
            LstObjetoTipo = new ObservableCollection<OBJETO_TIPO>(new cObjetoTipo().ObtenerTodos());
            LstObjetoTipo.Insert(0, new OBJETO_TIPO() { ID_OBJETO_TIPO = -1, DESCR = "SELECCIONE" });
            this.GetCompanias();
        }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            var metro = Application.Current.Windows[0] as MetroWindow;
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetCompanias();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
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
                        DescripcionEnabled = true;
                        #region Obtener Valores
                        Descripcion = SelectedItem.DESCR;
                        Estatus = SelectedItem.ESTATUS;
                        IdCompania = SelectedItem.ID_COMPANIA;
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                        if(SelectedItem.ID_OBJETO_TIPO != null)
                            Tipo = SelectedItem.ID_OBJETO_TIPO.Value;
                        #endregion Obtener Valores
                    }
                    else
                    {
                        bandera_editar = false;
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opción.");
                    }
                    break;
                case "menu_agregar":
                    setValidationRules();
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
                    DescripcionEnabled = true;
                    AgregarVisible = true;
                    bandera_editar = false;
                    FocusText = true;
                    /********************************/
                    SeleccionIndice = -1;
                    Descripcion = Estatus = string.Empty;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    Tipo = -1;
                    //PopulateCamaSiguiente();
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Descripcion))
                    {
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
                        /**********************************/
                        this.GuardarCompanias();
                        base.ClearRules();
                        /**********************************/
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe agregar el número de compañía ");
                    break;
                case "menu_cancelar":
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
                    SeleccionIndice = -1;
                    /****************************************/
                    Descripcion = Estatus = string.Empty;
                    SelectedEstatus = null;
                    /****************************************/
                    break;
                case "menu_eliminar":
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
                            if (Eliminar())
                            {
                                var dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
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
                                await metro.ShowMessageAsync("Algo ocurrió...", "No se puede eliminar información de la compañía: Tiene dependencias.", MessageDialogStyle.Affirmative, mySettings);
                                await TaskEx.Delay(1500);
                            }
                        }
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opción");
                    //SeleccionIndice = -1;
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

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetCompanias();
            }
        }

        private void GetCompanias()
        {
            try
            {
                ListItems.Clear();
                ListItems = new ObservableCollection<COMPANIA>(new cCompania().ObtenerTodos(Busqueda));
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

        private void GuardarCompanias()
        {
            try
            {
                COMPANIA obj = new COMPANIA();
                obj.DESCR = Descripcion;
                obj.ESTATUS = SelectedEstatus.CLAVE;
                if (Tipo != -1)
                    obj.ID_OBJETO_TIPO = Tipo;
                if (IdCompania > 0)
                {
                    obj.ID_COMPANIA = IdCompania.Value;
                    new cCompania().Actualizar(obj);
                }
                else
                {   //Agregar
                    new cCompania().Insertar(obj);
                }

                Descripcion = string.Empty;
                Tipo = -1;
                SelectedEstatus = null;
                GetCompanias();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool Eliminar()
        {
            try
            {
                if (SelectedItem != null || SelectedItem.ID_COMPANIA >= 100)
                {
                    if (obj.Eliminar(Convert.ToInt32(SelectedItem.ID_COMPANIA)))
                    {
                        GetCompanias();
                        return true;
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        //LOAD
        private async void CompaniasLoad(CompaniaView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<COMPANIA>();
                    CatalogoHeader = "Compañías";
                    HeaderAgregar = "Agregar Compañía";
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
                    MaxLength = 25;
                    SeleccionIndice = -1;
                    //Obtenemos las companias
                    this.setValidationRules();
                    this.GetCompanias();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la Compañía.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_COMPANIAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
