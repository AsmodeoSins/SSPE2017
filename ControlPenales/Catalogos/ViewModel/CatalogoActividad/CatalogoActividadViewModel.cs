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
    partial class CatalogoActividadViewModel : ValidationViewModelBase, IPageViewModel
    {
        cActividad obj = new cActividad();

        public CatalogoActividadViewModel()
        {
            CatalogoHeader = "Actividades";
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
            SeleccionIndice = -1;
            EmptyVisible = false;
            //Listado 
            ListaProgra();
            ListItems = new ObservableCollection<ACTIVIDAD>();
            this.GetActividades();
        }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetActividades();
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
                        Orden = SelectedItem.ORDEN;
                        Descripcion = SelectedItem.DESCR;
                        Prioridad = SelectedItem.PRIORIDAD;
                        OcupanteMin = SelectedItem.OCUPANTE_MIN;
                        OcupanteMax = SelectedItem.OCUPANTE_MAX;
                        IdTipoP = SelectedItem.ID_TIPO_PROGRAMA;
                        IdActiv = SelectedItem.ID_ACTIVIDAD;
                        Estatus = SelectedItem.ESTATUS;
                        Activo = SelectedItem.ACTIVO;
                        Objetivo = SelectedItem.OBJETIVO;
                        ListaProgra();
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
                    ListaProgra();
                    SeleccionIndice = IdTipoP = -1;
                    Descripcion = Objetivo = Activo = string.Empty;
                    Estatus = Orden = OcupanteMin = Prioridad = OcupanteMax = new short?();
                    IdActiv = 0;
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
                        GuardarActividades();
                        this.GetActividades();
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
                    IdTipoP = -1;
                    /****************************************/
                    Descripcion = Objetivo = Activo = string.Empty;
                    Estatus = Orden = OcupanteMin = Prioridad = OcupanteMax = new short?();
                    IdActiv = 0;
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
                                await metro.ShowMessageAsync("Algo ocurrió...", "No se puede eliminar información de la actividad: Tiene dependencias.", MessageDialogStyle.Affirmative, mySettings);
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
                GetActividades();
            }
        }

        private void ListaProgra()
        {
            try
            {
                if (ListTipoP == null)
                {
                    ListTipoP = new ObservableCollection<TIPO_PROGRAMA>(new cTipoPrograma().GetData().OrderBy(x => x.NOMBRE));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            ListTipoP.Insert(0, (new TIPO_PROGRAMA() { ID_TIPO_PROGRAMA = -1, NOMBRE = "SELECCIONE" }));
                        }));
                };
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void GetActividades()
        {
            try
            {
                ListItems.Clear();
                ListItems = new ObservableCollection<ACTIVIDAD>(new cActividad().ObtenerTodos(Busqueda));

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

        private void GuardarActividades()
        {
            try
            {
                if (IdActiv > 0)
                    obj.Actualizar(new ACTIVIDAD() { ACTIVO = Activo, ESTATUS = Estatus, ESTAPA = Etapa, ID_ACTIVIDAD = SelectedItem.ID_ACTIVIDAD, ID_TIPO_PROGRAMA = IdTipoP, OBJETIVO = Objetivo, OCUPANTE_MAX = OcupanteMax, OCUPANTE_MIN = OcupanteMin, PRIORIDAD = Prioridad, ORDEN = Orden, DESCR = Descripcion });
                else
                {   //Agregar
                    ACTIVIDAD newItem = new ACTIVIDAD();
                    newItem.ACTIVO = Activo;
                    newItem.ESTATUS = Estatus;
                    newItem.ID_TIPO_PROGRAMA = IdTipoP;
                    newItem.OBJETIVO = Objetivo;
                    newItem.ESTAPA = Etapa;
                    newItem.OCUPANTE_MAX = OcupanteMax;
                    newItem.OCUPANTE_MIN = OcupanteMin;
                    newItem.PRIORIDAD = Prioridad;
                    newItem.DESCR = Descripcion;
                    newItem.ORDEN = Orden;
                    obj.Insertar(newItem);
                }
                Descripcion = Objetivo = string.Empty;
                Estatus = Orden = OcupanteMin = Prioridad = OcupanteMax = new short?();
                IdActiv = IdTipoP = 0;
                GetActividades();
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
                if (SelectedItem != null || SelectedItem.ID_ACTIVIDAD >= 100)
                {
                    if (obj.Eliminar(Convert.ToInt32(SelectedItem.ID_ACTIVIDAD)))
                    {
                        GetActividades();
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
        private async void ActividadesLoad(ActividadesView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<ACTIVIDAD>();
                    CatalogoHeader = "Actividad";
                    HeaderAgregar = "Agregar Actividades";
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
                    SeleccionIndice = -1;
                    //Obtenemos las celdas
                    this.GetActividades();
                    this.setValidationRules();
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las actividades.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_ACTIVIDADES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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