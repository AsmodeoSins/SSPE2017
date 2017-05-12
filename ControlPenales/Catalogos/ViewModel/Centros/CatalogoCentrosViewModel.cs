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
    partial class CatalogoCentrosViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        cCentro objCentro = new cCentro();
        cEntidad objEstado = new cEntidad();
        cMunicipio objCiudades = new cMunicipio();

        public CatalogoCentrosViewModel()
        {
            CatalogoHeader = "Centros";
            HeaderAgregar = "Agregar Centro";
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
            SeleccionIndice = -1;
            EmptyVisible = false;
            //Listado 
            ListItems = new ObservableCollection<CENTRO>(objCentro.ObtenerTodos());
            ListEntidad = new ObservableCollection<ENTIDAD>();
            ListMunicipio = new ObservableCollection<MUNICIPIO>();
            ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = 0, MUNICIPIO1 = "SELECCIONAR" });
            ListEntidadFiltro = new ObservableCollection<ENTIDAD>();
            Entidad = new ENTIDAD();
            //Obtenemos las Entidades Federativas
            this.GetEntidades();
            //Obtenemos los Centros
            this.GetCentros();
            this.setValidationRules();
        }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetCentros();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Centro";
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
                        #region Obtener Valores
                        Nombre = string.IsNullOrEmpty(SelectedItem.DESCR) ? string.Empty : SelectedItem.DESCR.TrimEnd();
                        Calle = string.IsNullOrEmpty(SelectedItem.CALLE) ? string.Empty : SelectedItem.CALLE.TrimEnd();
                        Colonia = string.IsNullOrEmpty(SelectedItem.COLONIA) ? string.Empty : SelectedItem.CALLE.TrimEnd();
                        No_exterior = string.IsNullOrEmpty(SelectedItem.NUM_EXT.ToString()) ? string.Empty : SelectedItem.NUM_EXT.ToString();
                        No_interior = string.IsNullOrEmpty(SelectedItem.NUM_INT) ? string.Empty : SelectedItem.NUM_INT.TrimEnd();
                        Codigo_postal = string.IsNullOrEmpty(SelectedItem.CP.ToString()) ? string.Empty : SelectedItem.CP.ToString();
                        Telefono = string.IsNullOrEmpty(SelectedItem.TELEFONO.ToString()) ? string.Empty : SelectedItem.TELEFONO.ToString();
                        Fax = string.IsNullOrEmpty(SelectedItem.FAX.ToString()) ? string.Empty : SelectedItem.FAX.ToString();
                        Director = string.IsNullOrEmpty(SelectedItem.DIRECTOR) ? string.Empty : SelectedItem.DIRECTOR.TrimEnd();

                        SelectEntidad = ListEntidad.Where(w => w.ID_ENTIDAD == SelectedItem.ID_ENTIDAD).FirstOrDefault();
                        SelectMunicipio = ListMunicipio.Where(w => w.ID_MUNICIPIO == SelectedItem.ID_MUNICIPIO).FirstOrDefault();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
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
                    HeaderAgregar = "Agregar Centro";
                    //Limpiamos el objeto de tipo centro
                    SeleccionIndice = -1;
                    Nombre = string.Empty;
                    Calle = string.Empty;
                    Colonia = string.Empty;
                    No_exterior = string.Empty;
                    No_interior = string.Empty;
                    Codigo_postal = string.Empty;
                    Telefono = string.Empty;
                    Fax = string.Empty;
                    Director = string.Empty;
                    SelectEntidad = ListEntidad.Where(w => w.ID_ENTIDAD == -1).FirstOrDefault();
                    SelectMunicipio = ListMunicipio.Where(w => w.ID_MUNICIPIO == -1).FirstOrDefault();
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    /****************************************/
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
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Nombre))
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
                        this.GuardarCentros();
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
                    //Limpiamos las variables
                    SeleccionIndice = -1;
                    Nombre = string.Empty;
                    Calle = string.Empty;
                    Colonia = string.Empty;
                    No_exterior = string.Empty;
                    No_interior = string.Empty;
                    Codigo_postal = string.Empty;
                    Telefono = string.Empty;
                    Fax = string.Empty;
                    Director = string.Empty;
                    SelectEntidad = ListEntidad.Where(w => w.ID_ENTIDAD == -1).FirstOrDefault();
                    SelectMunicipio = ListMunicipio.Where(w => w.ID_MUNICIPIO == -1).FirstOrDefault();
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
                                await metro.ShowMessageAsync("Algo ocurrió...", "No se puede eliminar información del centro: Tiene dependencias.", MessageDialogStyle.Affirmative, mySettings);
                                await TaskEx.Delay(1500);
                            }
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

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetCentros();
            }
        }

        private void GetEntidades()
        {
            try
            {
                ListEntidad.Clear();
                ListEntidad = new ObservableCollection<ENTIDAD>(objEstado.ObtenerTodos());
                ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                ListEntidadFiltro.Clear();
                ListEntidadFiltro = new ObservableCollection<ENTIDAD>(objEstado.ObtenerTodos());
                ListEntidadFiltro.Insert(0, new ENTIDAD() { ID_ENTIDAD = 0, DESCR = "TODOS" });
                SelectedEntidad = ListEntidad.Where(w => w.ID_ENTIDAD == 0).FirstOrDefault();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GetCentros()
        {
            try
            {
                cCentro centro = new cCentro();
                ListItems = new ObservableCollection<CENTRO>(centro.ObtenerTodos(Busqueda, SelectedEntidad.ID_ENTIDAD, SelectedMunicipio.ID_MUNICIPIO));
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

        private void GuardarCentros()
        {
            try
            {
                if (SelectedItem != null) //Editar
                {  //Actualizar
                    var centro = new CENTRO();
                    centro.ID_CENTRO = SelectedItem.ID_CENTRO;
                    centro.DESCR = Nombre;
                    centro.CALLE = Calle;
                    centro.COLONIA = Colonia;
                    centro.ESTATUS = SelectedEstatus.CLAVE;
                    if (!string.IsNullOrEmpty(Codigo_postal))
                        centro.CP = short.Parse(Codigo_postal);
                    if (!string.IsNullOrEmpty(No_exterior))
                        centro.NUM_EXT = int.Parse(No_exterior);
                    centro.NUM_INT = No_interior;
                    if (!string.IsNullOrEmpty(Telefono))
                        centro.TELEFONO = long.Parse(Telefono);
                    if (!string.IsNullOrEmpty(Telefono))
                        centro.FAX = long.Parse(Fax);
                    centro.DIRECTOR = Director;
                    centro.ID_MUNICIPIO = SelectMunicipio.ID_MUNICIPIO;
                    centro.ID_ENTIDAD = SelectEntidad.ID_ENTIDAD;
                    centro.ID_TIPO_CENTRO = SelectedItem.ID_TIPO_CENTRO;
                    objCentro.Actualizar(centro);
                }
                else
                {   //Agregar
                    var centro = new CENTRO();
                    centro.DESCR = Nombre;
                    centro.CALLE = Calle;
                    centro.COLONIA = Colonia;
                    centro.ESTATUS = SelectedEstatus.CLAVE;
                    if (!string.IsNullOrEmpty(Codigo_postal))
                        centro.CP = string.IsNullOrEmpty(Codigo_postal) ? new Nullable<int>() : int.Parse(Codigo_postal);
                    if (!string.IsNullOrEmpty(No_exterior))
                        centro.NUM_EXT = string.IsNullOrEmpty(No_exterior) ? new Nullable<int>() : int.Parse(No_exterior);
                    centro.NUM_INT = No_interior;
                    if (!string.IsNullOrEmpty(Telefono))
                        centro.TELEFONO = string.IsNullOrEmpty(Telefono) ? new Nullable<long>() : long.Parse(Telefono);
                    if (!string.IsNullOrEmpty(Telefono))
                        centro.FAX = string.IsNullOrEmpty(Fax) ? new Nullable<long>() : long.Parse(Fax);
                    centro.DIRECTOR = Director;
                    centro.ID_MUNICIPIO = SelectMunicipio.ID_MUNICIPIO;
                    centro.ID_ENTIDAD = SelectEntidad.ID_ENTIDAD;
                    centro.ID_TIPO_CENTRO = 0;

                    objCentro.Insertar(centro);
                }
                //Limpiamos las variables
                SeleccionIndice = -1;
                Nombre = string.Empty;
                Calle = string.Empty;
                Colonia = string.Empty;
                No_exterior = string.Empty;
                No_interior = string.Empty;
                Codigo_postal = string.Empty;
                Telefono = string.Empty;
                Fax = string.Empty;
                Director = string.Empty;
                SelectEntidad = ListEntidad.Where(w => w.ID_ENTIDAD == -1).FirstOrDefault();
                SelectMunicipio = ListMunicipio.Where(w => w.ID_MUNICIPIO == -1).FirstOrDefault();
                //Mostrar Listado
                GetCentros();
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
                    if (objCentro.Eliminar(SelectedItem.ID_CENTRO))
                    {
                        SeleccionIndice = -1;
                        Nombre = string.Empty;
                        Calle = string.Empty;
                        Colonia = string.Empty;
                        No_exterior = string.Empty;
                        No_interior = string.Empty;
                        Codigo_postal = string.Empty;
                        Telefono = string.Empty;
                        Fax = string.Empty;
                        Director = string.Empty;
                        SelectEntidad = ListEntidad.Where(w => w.ID_ENTIDAD == -1).FirstOrDefault();
                        SelectMunicipio = ListMunicipio.Where(w => w.ID_MUNICIPIO == -1).FirstOrDefault();
                        GetCentros();
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
        private async void CentroLoad(CatalogoCentrosView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<CENTRO>();
                    CatalogoHeader = "Centros";
                    HeaderAgregar = "Agregar Centros";
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
                    //Obtenemos los centros
                    this.GetCentros();
                    this.setValidationRules();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de centro.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_CENTROS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
                    if (p.CONSULTAR == 1)
                    {
                        BuscarHabilitado = true;
                        TextoHabilitado = true;
                        EstadoHabilitado = true;
                        MunicipioHabilitado = true;
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