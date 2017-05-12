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
    partial class CatalogoCeldasViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        cCelda obj = new cCelda();
        public CatalogoCeldasViewModel()
        {
            CatalogoHeader = "Celdas";
            HeaderAgregar = "Agregar Nueva Celda";
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
            MaxLength = 5;
            SeleccionIndice = -1;
            EmptyVisible = false;
            //Listado 
            ListItems = new ObservableCollection<CELDA>();
            ListMunicipios = new ObservableCollection<MUNICIPIO>();
            ListCentros = new ObservableCollection<CENTRO>();
            ListSectores = new ObservableCollection<SECTOR>();
            ListEdificios = new ObservableCollection<EDIFICIO>();
            //Obtenemos los Centros
            ListMunicipios = new ObservableCollection<MUNICIPIO>(new cMunicipio().ObtenerTodos("", 2));
            SelectedMunicipio = ListMunicipios.Where(w => w.ID_MUNICIPIO == 0).FirstOrDefault();
            //this.getSectores();
            this.setValidationRules();
        }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetCeldas();
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
                        DescripcionEnabled = false;
                        #region Obtener Valores
                        Clave = SelectedItem.ID_CENTRO;
                        Descripcion = SelectedItem.ID_CELDA;
                        Sector = SelectedItem.ID_SECTOR;
                        Centro = SelectedItem.ID_CENTRO;
                        Edificio = SelectedItem.ID_EDIFICIO;
                        SelectedMunicipio = ListMunicipios.Where(w => w.ID_MUNICIPIO == SelectedItem.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO).FirstOrDefault();
                        SelectedCentro = ListCentros.Where(w => w.ID_CENTRO == SelectedItem.ID_CENTRO).FirstOrDefault();
                        SelectedEdificio = ListEdificios.Where(w => w.ID_EDIFICIO == SelectedItem.ID_EDIFICIO).FirstOrDefault();
                        SelectedSector = ListSectores.Where(w => w.ID_SECTOR == SelectedItem.ID_SECTOR).FirstOrDefault();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).FirstOrDefault();
                        #endregion Obtener Valores
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
                    DescripcionEnabled = true;
                    AgregarVisible = true;
                    bandera_editar = false;
                    FocusText = true;
                    /********************************/
                    SeleccionIndice = -1;
                    Clave = 0;
                    Descripcion = "";
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
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
                        this.GuardarCeldas();
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
                    Descripcion = "";
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
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.ID_CELDA + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
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
                                await metro.ShowMessageAsync("Algo ocurrió...", "No se puede eliminar información de la celda: Tiene dependencias.", MessageDialogStyle.Affirmative, mySettings);
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
                GetCeldas();
            }
        }

        private void GetCeldas()
        {
            try
            {
                ListItems = new ObservableCollection<CELDA>(obj.ObtenerTodos(
                                                                Busqueda,
                                                                SelectedMunicipio,
                                                                SelectedCentro,
                                                                SelectedEdificio,
                                                                SelectedSector)
                                                                .OrderBy(o => o.SECTOR.EDIFICIO.CENTRO.MUNICIPIO.MUNICIPIO1)
                                                                .ThenBy(t => t.SECTOR.EDIFICIO.CENTRO.DESCR)
                                                                .ThenBy(b => b.SECTOR.EDIFICIO.DESCR)
                                                                .ThenBy(n => n.SECTOR.DESCR));
                if (ListItems.Count > 0)
                    EmptyVisible = false;
                else
                    EmptyVisible = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener.", ex);
            }
        }

        private void GuardarCeldas()
        {
            try
            {
                if (Clave > 0)
                {  //Actualizar
                    obj.Actualizar(new CELDA()
                    {
                        ID_CENTRO = SelectedCentro.ID_CENTRO,
                        ID_EDIFICIO = SelectedEdificio.ID_EDIFICIO,
                        ID_SECTOR = SelectedSector.ID_SECTOR,
                        ID_CELDA = Descripcion,
                        ID_TIPO_CELDA = "I",
                        ID_TIPO_SEGURIDAD = "B",
                        NIVEL_EDIFICIO = 1,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                else
                {   //Agregar
                    obj.Insertar(new CELDA()
                    {
                        ID_CENTRO = SelectedCentro.ID_CENTRO,
                        ID_EDIFICIO = SelectedEdificio.ID_EDIFICIO,
                        ID_SECTOR = SelectedSector.ID_SECTOR,
                        ID_CELDA = Descripcion,
                        ID_TIPO_CELDA = "I",
                        ID_TIPO_SEGURIDAD = "B",
                        NIVEL_EDIFICIO = 1,
                        ESTATUS = SelectedEstatus.CLAVE,
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                //Mostrar Listado
                GetCeldas();
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
                if (SelectedItem != null)
                {
                    if (obj.Eliminar(SelectedItem.ID_CELDA))
                    {
                        GetCeldas();
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
        private async void CeldasLoad(CatalogoCeldasView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<CELDA>();
                    CatalogoHeader = "Celdas";
                    HeaderAgregar = "Agregar Celdas";
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
                    this.GetCeldas();
                    this.setValidationRules();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de celda.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_CELDAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                    {
                        DataGridEnabled = true;
                        AgregarMenuEnabled = true;
                    }
                    if (p.CONSULTAR == 1)
                    {
                        BuscarHabilitado = true;
                        TextoHabilitado = true;
                        MunicipioHabilitado = true;
                        CentroHabilitado = true;
                        EdificioHabilitado = true;
                        SectorHabilitado = true;
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