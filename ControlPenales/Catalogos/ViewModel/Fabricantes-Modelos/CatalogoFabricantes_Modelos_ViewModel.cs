using System;
using MahApps.Metro.Controls;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
namespace ControlPenales
{
    partial class CatalogoFabricantes_Modelos_ViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        cDecomisoFabricante decomisofabricante = new cDecomisoFabricante();

        public CatalogoFabricantes_Modelos_ViewModel() { }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetFabricantesModelos();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Fabricantes-Modelos";
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
                        PopularCombo();
                        /*****************************************/
                        Clave = SelectedItem.ID_FABRICANTE;
                        Descripcion = SelectedItem.DESCR == null ? SelectedItem.DESCR : SelectedItem.DESCR.TrimEnd();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                        if (SelectedItem.ID_OBJETO_TIPO != null)
                            Tipo = SelectedItem.ID_OBJETO_TIPO.Value;
                        //SelectedObjetoTipo = ListObjetoTipo.Where(w => w.ID_OBJETO_TIPO == SelectedItem.ID_OBJETO_TIPO).SingleOrDefault();
                        //SelectedFabricante = Lista_Fabricantes.Where(W => W. == selectedItem.ID_FABRICANTE).SingleOrDefault();
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
                    HeaderAgregar = "Agregar Nuevo Fabricantes-Modelos";
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
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    Tipo = -1;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    //SelectedObjetoTipo = null;
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
                        this.GuardarFabricantesModelo();
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
                    Tipo = -1;
                    this.GetFabricantesModelos();
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
                            if (this.EliminarTipoVisita())
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

        private async void TipoFabricantesModelosLoad(CatalogoFabricantes_Modelos_View Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<DECOMISO_FABRICANTE>();
                    ListObjetoTipo = new List<OBJETO_TIPO>(new cObjetoTipo().ObtenerTodos().OrderBy(o => o.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListObjetoTipo.Insert(0, new OBJETO_TIPO() { ID_OBJETO_TIPO = -1, DESCR = "SELECCIONE" });
                    }));
                    CatalogoHeader = "Fabricantes-Modelos";
                    HeaderAgregar = "Agregar Nuevo Fabricantes-Modelos";
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
                    //PopularCombo();
                    this.GetFabricantesModelos();
                    this.setValidationRules();
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar Fabricantes-Modelos.", ex);
            }
        }

        private void PopularCombo()
        {
            ListObjetoTipo = new cObjetoTipo().ObtenerTodos().ToList();
            ListObjetoTipo.Insert(0, new OBJETO_TIPO() { ID_OBJETO_TIPO = -1, DESCR = "SELECCIONE" });
            Tipo = -1;
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetFabricantesModelos();
            }
        }

        private void GetFabricantesModelos()
        {
            try
            {
                if (string.IsNullOrEmpty(Busqueda))
                {
                    ListItems = new ObservableCollection<DECOMISO_FABRICANTE>(new cDecomisoFabricante().ObtenerTodos().ToList());
                }
                else
                {
                    ListItems = new ObservableCollection<DECOMISO_FABRICANTE>(new cDecomisoFabricante().ObtenerTodos().ToList()
                        .Where(w => w.DESCR.Contains(Busqueda) ||
                            w.ESTATUS.Contains(Busqueda) ||
                            w.OBJETO_TIPO.DESCR.Contains(Busqueda)));
                }
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

        private void GuardarFabricantesModelo()
        {
            try
            {
                if (Clave > 0)
                {  //Actualizar
                    //SelectedItem.DESCR = Descripcion;
                    //tipoEstudio.Actualizar(SelectedItem);

                    decomisofabricante.Actualizar(new DECOMISO_FABRICANTE
                    {
                        ID_FABRICANTE = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE,
                        ID_OBJETO_TIPO = Tipo//SelectedObjetoTipo.ID_OBJETO_TIPO
                    });
                }
                else
                {   //Agregar
                    decomisofabricante.Insertar(new DECOMISO_FABRICANTE
                    {
                        ID_FABRICANTE = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE,
                        ID_OBJETO_TIPO = Tipo//SelectedObjetoTipo.ID_OBJETO_TIPO
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                Busqueda = string.Empty;
                Tipo = -1;
                SelectedEstatus = null;
                //Mostrar Listado
                this.GetFabricantesModelos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool EliminarTipoVisita()
        {
            try
            {
                if (SelectedItem != null || SelectedItem.ID_FABRICANTE >= 100)
                {
                    if (!decomisofabricante.Eliminar(SelectedItem))
                        return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    Busqueda = string.Empty;
                    this.GetFabricantesModelos();
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_FABRICANTE_MODELO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
