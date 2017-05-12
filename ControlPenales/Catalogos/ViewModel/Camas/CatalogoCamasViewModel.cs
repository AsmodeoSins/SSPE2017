using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
namespace ControlPenales
{
    partial class CatalogoCamasViewModel : ValidationViewModelBase, IPageViewModel
    {
        cCama obj = new cCama();

        public CatalogoCamasViewModel()
        {
            //SelectedMunicipio = ListMunicipios.Where(w => w.ID_MUNICIPIO == SelectedItem.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO).FirstOrDefault();
            CatalogoHeader = "Camas";
            HeaderAgregar = "Agregar Nueva Cama";
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
            AgregarVisible = false;
            /*MAXLENGTH*/
            MaxLength = 5;
            SeleccionIndice = -1;
            EmptyVisible = true;
            ListMunicipios = new ObservableCollection<MUNICIPIO>(new cMunicipio().ObtenerTodos(string.Empty, 2));
            ListMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
            SelectedMunicipio = ListMunicipios.Select(s => s).FirstOrDefault();
            GetCamas();
            this.setValidationRules();
        }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            var metro = Application.Current.Windows[0] as MetroWindow;
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetCamas();
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
                        Clave = SelectedItem.ID_CAMA;
                        Descripcion = SelectedItem.DESCR;
                        Sector = SelectedItem.ID_SECTOR;
                        Centro = SelectedItem.ID_CENTRO;
                        Edificio = SelectedItem.ID_EDIFICIO;
                        Celda = SelectedItem.ID_CELDA;

                        SelectedMunicipio = ListMunicipios.Where(w => w.ID_MUNICIPIO == SelectedItem.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO).FirstOrDefault();
                        SelectedCentro = ListCentros.Where(w => w.ID_CENTRO == SelectedItem.ID_CENTRO).FirstOrDefault();
                        SelectedEdificio = ListEdificios.Where(w => w.ID_EDIFICIO == SelectedItem.ID_EDIFICIO).FirstOrDefault();
                        SelectedSector = ListSectores.Where(w => w.ID_SECTOR == SelectedItem.ID_SECTOR).FirstOrDefault();
                        SelectedCelda = ListCeldas.Where(w => w.ID_CELDA == SelectedItem.ID_CELDA).FirstOrDefault();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
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
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    //PopulateCamaSiguiente();
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
                        this.GuardarCamas();
                        base.ClearRules();
                        /**********************************/
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe agregar el número de cama.");
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
                    if (SelectedItem != null)
                    {
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Aceptar",
                            NegativeButtonText = "Cancelar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.ID_CAMA + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
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
                                await metro.ShowMessageAsync("Algo ocurrió...", "No se puede eliminar información de la cama: Tiene dependencias.", MessageDialogStyle.Affirmative, mySettings);
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
                GetCamas();
            }
        }

        private void GetCamas()
        {
            try
            {
                ListItems = new ObservableCollection<CAMA>(new cCama().ObtenerTodos(Busqueda, SelectedMunicipio, SelectedCentro, SelectedEdificio, SelectedSector, SelectedCelda)
                                                            .OrderBy(o => o.CELDA.SECTOR.EDIFICIO.CENTRO.MUNICIPIO.MUNICIPIO1).ThenBy(t => t.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR)
                                                            .ThenBy(h => h.CELDA.SECTOR.EDIFICIO.DESCR).ThenBy(n => n.CELDA.SECTOR.DESCR)
                                                            .ThenBy(b => b.CELDA.ID_CELDA).ThenBy(y => y.ID_CAMA));
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

        //private void GetCamas()
        //{

        //    ListItems = new ObservableCollection<CAMA>(obj.ObtenerTodos(Busqueda,SelectedMunicipio,SelectedCentro,SelectedEdificio,SelectedSector,SelectedCelda)
        //                                                    .OrderBy(o => o.CELDA.SECTOR.EDIFICIO.CENTRO.MUNICIPIO.MUNICIPIO1).ThenBy(t => t.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR)
        //                                                    .ThenBy(h => h.CELDA.SECTOR.EDIFICIO.DESCR).ThenBy(n => n.CELDA.SECTOR.DESCR)
        //                                                    .ThenBy(b => b.CELDA.ID_CELDA).ThenBy(y => y.ID_CAMA));
        //    if (ListItems.Count > 0)
        //        EmptyVisible = false;
        //    else
        //        EmptyVisible = true;
        //}

        private void GuardarCamas()
        {
            try
            {
                if (Clave > 0)
                {  //Actualizar
                    obj.Actualizar(new CAMA()
                    {
                        ID_CENTRO = SelectedCentro.ID_CENTRO,
                        ID_EDIFICIO = SelectedEdificio.ID_EDIFICIO,
                        ID_SECTOR = SelectedItem.ID_SECTOR,
                        ID_CELDA = SelectedCelda.ID_CELDA,
                        ESTATUS = SelectedEstatus.CLAVE,
                        ID_CAMA = SelectedItem.ID_CAMA,
                        DESCR = Descripcion
                    });
                }
                else
                {   //Agregar
                    obj.Insertar(new CAMA()
                    {
                        ID_CENTRO = SelectedCentro.ID_CENTRO,
                        ID_EDIFICIO = SelectedEdificio.ID_EDIFICIO,
                        ID_SECTOR = SelectedSector.ID_SECTOR,
                        ID_CELDA = SelectedCelda.ID_CELDA,
                        ESTATUS = SelectedEstatus.CLAVE,
                        DESCR = Descripcion
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                //Mostrar Listado
                GetCamas();
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
                    if (obj.Eliminar(SelectedItem.ID_CAMA))
                    {
                        GetCamas();
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
        private async void CatalogoCamaLoad(CatalogoCamasView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<CAMA>();
                    CatalogoHeader = "Camas";
                    HeaderAgregar = "Agregar Camas";
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
                    this.GetCamas();
                    this.setValidationRules();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de cama.", ex);
            }
        }

        ////LOAD
        //private void CatalogoCamaLoad(CatalogoCamasView Window = null)
        //{
        //    CatalogoHeader = "Camas";
        //    HeaderAgregar = "Agregar Nueva Cama";
        //    //LLENAR 
        //    EditarVisible = false;
        //    NuevoVisible = false;

        //    AgregarVisible = false;

        //    GuardarMenuEnabled = false;
        //    AgregarMenuEnabled = true;
        //    EliminarMenuEnabled = false;
        //    EditarMenuEnabled = false;
        //    CancelarMenuEnabled = false;
        //    AyudaMenuEnabled = true;
        //    SalirMenuEnabled = true;
        //    ExportarMenuEnabled = true;

        //    /*MAXLENGTH*/
        //    MaxLength = 4;
        //    SeleccionIndice = -1;
        //    Descripcion = 0;
        //    CeldasEnabled = false;
        //    EmptyVisible = false;
        //    //Listado 
        //    ListItems = new ObservableCollection<CAMA>();
        //    PrepararLista();
        //    //ListCeldas = new ObservableCollection<CELDA>();
        //    //ListMunicipios = new ObservableCollection<MUNICIPIO>();
        //    //ListCentros = new ObservableCollection<CENTRO>();
        //    //ListSectores = new ObservableCollection<SECTOR>();
        //    //ListEdificios = new ObservableCollection<EDIFICIO>();
        //    //Obtenemos los Centros
        //    //ListMunicipios = new ObservableCollection<MUNICIPIO>(new cMunicipio().ObtenerTodos(string.Empty, 2));
        //    SelectedMunicipio = ListMunicipios.Where(w => w.ID_MUNICIPIO == -1).FirstOrDefault();
        //    //this.getSectores();
        //    //this.setValidationRules();
        //}

        private void PrepararLista()
        {
            if (ListMunicipios == null)
            {
                ListMunicipios = new ObservableCollection<MUNICIPIO>(new cMunicipio().ObtenerTodos(string.Empty, 2));
                ListMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
            }
            if (ListCentros == null)
            {
                ListCentros = new ObservableCollection<CENTRO>();
                ListCentros.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = "SELECCIONE" });
            }
            if (ListEdificios == null)
            {
                ListEdificios = new ObservableCollection<EDIFICIO>();
                ListEdificios.Insert(0, new EDIFICIO() { ID_EDIFICIO = -1, DESCR = "SELECCIONE" });
            }
            if (ListSectores == null)
            {
                ListSectores = new ObservableCollection<SECTOR>();
                ListSectores.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
            }
            if (ListCeldas == null)
            {
                ListCeldas = new ObservableCollection<CELDA>();
                ListCeldas.Insert(0, new CELDA() { ID_CELDA = "SELECCIONE" });
            }
        }

        private void PopulateCamaSiguiente()
        {
            if (Clave != -1)
            {
                if (!SelectedCelda.ID_CELDA.Equals("SELECCIONE"))
                    Clave = obj.CamaSiguiete(SelectedCelda.ID_CENTRO, SelectedCelda.ID_EDIFICIO, SelectedCelda.ID_SECTOR, SelectedCelda.ID_CELDA);
                else
                    Clave = -1;
            }
            else
            {
                Clave = -1;
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_CAMAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
                        MunicipioHabilitado = true;
                        CentroHabilitado = true;
                        EdificioHabilitado = true;
                        SectorHabilitado = true;
                        CeldaEnabled = true;
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