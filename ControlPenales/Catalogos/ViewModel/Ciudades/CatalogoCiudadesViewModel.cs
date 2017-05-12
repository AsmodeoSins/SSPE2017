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
    partial class CatalogoCiudadesViewModel : ValidationViewModelBase, IPageViewModel
    {
        cMunicipio obj = new cMunicipio();
        cEntidad obj2 = new cEntidad();

        public CatalogoCiudadesViewModel()
        {
            CatalogoHeader = "Municipios";
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
            MaxLength = 30;
            SeleccionIndice = -1;
            EmptyVisible = false;
            //Listado 
            ListItems = new ObservableCollection<MUNICIPIO>();
            ListEstados = new ObservableCollection<ENTIDAD>();
            SelectedEstado = new ENTIDAD();
            //Obtenemos las Ciudades
            ListEstados = new ObservableCollection<ENTIDAD>(new cEntidad().ObtenerTodos(string.Empty));
            ListEstados.Insert(0, new ENTIDAD() { ID_ENTIDAD = 0, DESCR = "TODOS" });
            SelectedEstado = ListEstados.Where(w => w.ID_ENTIDAD == 2).FirstOrDefault(); //Por default carga BAJA CALIFORNIA
            this.GetCiudades();
        }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetCiudades();
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
                        /*****************************************/
                        Clave = SelectedItem.ID_MUNICIPIO;
                        Descripcion = SelectedItem.MUNICIPIO1;
                        /*****************************************/
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).FirstOrDefault();
                    }
                    else
                    {
                        bandera_editar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una opción.");
                    }
                    break;
                case "menu_agregar":
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
                    bandera_editar = false;
                    FocusText = true;
                    /********************************/
                    SeleccionIndice = -1;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    Clave = 0;
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Descripcion) && SelectedEstatus != null)
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
                        GuardarCiudad();
                        this.GetCiudades();
                        /**********************************/
                    }
                    else
                        FocusText = true;
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
                    Clave = 0;
                    Descripcion = string.Empty;
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
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.MUNICIPIO1 + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
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
                                await metro.ShowMessageAsync("Algo ocurrió...", "No se puede eliminar información del Municipio: Tiene dependencias.", MessageDialogStyle.Affirmative, mySettings);
                                await TaskEx.Delay(1500);
                            }
                        }
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opción");
                    // SeleccionIndice = -1;
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
                GetCiudades();
            }
        }

        private void GetCiudades()
        {
            try
            {
                //var m = ListItems.Select(s => new ENTIDAD() { ID_ENTIDAD = s.ID_ENTIDAD, MUNICIPIOs = s.MUNICIPIOs, PAIS_NACIONALIDAD = s.PAIS_NACIONALIDAD, DESCR = s.DESCR, ID_PAIS_NAC = s.ID_PAIS_NAC });
                ListItems.Clear();
                ListItems = new ObservableCollection<MUNICIPIO>(new cMunicipio().ObtenerTodos(Busqueda, SelectedEstado.ID_ENTIDAD));

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

        private void GuardarCiudad()
        {
            try
            {
                if (Clave > 0)
                {  //Actualizar
                    //SelectedItem.MUNICIPIO1 = Descripcion;
                    //SelectedItem.ESTATUS = SelectedEstatus.CLAVE;
                    obj.Actualizar(new MUNICIPIO() { ID_ENTIDAD = SelectedItem.ID_ENTIDAD, ID_MUNICIPIO = SelectedItem.ID_MUNICIPIO, MUNICIPIO1 = Descripcion, ESTATUS = SelectedEstatus.CLAVE });
                }
                else
                {   //Agregar
                    MUNICIPIO newItem = new MUNICIPIO();
                    newItem.ID_MUNICIPIO = Clave;
                    newItem.MUNICIPIO1 = Descripcion;
                    newItem.ID_ENTIDAD = SelectedEstado.ID_ENTIDAD;
                    newItem.ESTATUS = SelectedEstatus.CLAVE;
                    obj.Insertar(newItem);
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                //Mostrar Listado
                GetCiudades();
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
                if (SelectedItem != null || SelectedItem.ID_MUNICIPIO >= 100)
                {
                    if (obj.Eliminar(Convert.ToInt32(SelectedItem.ID_MUNICIPIO)))
                    {
                        GetCiudades();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        //LOAD
        private async void CiudadesLoad(CatalogoCiudadesView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<MUNICIPIO>();
                    CatalogoHeader = "Municipios";
                    HeaderAgregar = "Agregar Municipios";
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
                    this.GetCiudades();
                    this.setValidationRules();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de municipio.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_MUNICIPIO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
                    if (p.CONSULTAR == 1)
                    {
                        BuscarHabilitado = true;
                        TextoHabilitado = true;
                        ComboxHabilitado = true;
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