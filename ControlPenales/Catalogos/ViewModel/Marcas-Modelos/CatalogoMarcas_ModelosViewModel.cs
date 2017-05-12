using MahApps.Metro.Controls;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace ControlPenales
{
    partial class CatalogoMarcas_ModelosViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public delegate bool comprobacion(System.Windows.Controls.UserControl d);
        cDecomisoModelo decomisomodelo = new cDecomisoModelo();
        cDecomisoFabricante decomisofabricante = new cDecomisoFabricante();

        public CatalogoMarcas_ModelosViewModel() { }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetMarcasModelo();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Modelos-Marcas";
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
                        Clave = SelectedItem.ID_MODELO;
                        Descripcion = SelectedItem.DESCR == null ? SelectedItem.DESCR : SelectedItem.DESCR.TrimEnd();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                        SelectedFabricante = Lista_Fabricantes.Where(W => W.ID_FABRICANTE == SelectedItem.ID_FABRICANTE).SingleOrDefault();
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
                    HeaderAgregar = "Agregar Nuevo Marcas-Modelo";
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
                    PopularCombo();
                    /********************************/
                    SeleccionIndice = -1;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    SelectedFabricante = null;
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
                        this.GuardarMarcaModelo();
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
                    this.GetMarcasModelo();
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

        public ObservableCollection<DECOMISO_MODELO> CargarLoading()
        {
            EmptyVisible = false;
            //Listado 
            ListItems = new ObservableCollection<DECOMISO_MODELO>();
            CatalogoHeader = "Modelos-Marcas";
            HeaderAgregar = "Agregar Nuevo Modelos-Marcas";
            //LLENAR 
            EditarVisible = false;
            NuevoVisible = false;
            AgregarVisible = false;
            GuardarMenuEnabled = false;
            AgregarMenuEnabled = true;
            EliminarMenuEnabled = false;
            EditarMenuEnabled = false;
            CancelarMenuEnabled = false;
            AyudaMenuEnabled = true;
            SalirMenuEnabled = true;
            ExportarMenuEnabled = true;
            /*MAXLENGTH*/
            MaxLength = 10;
            SeleccionIndice = -1;
            //Obtenemos las Etnias
            if (string.IsNullOrEmpty(Busqueda))
            {
                ListItems = new ObservableCollection<DECOMISO_MODELO>(new cDecomisoModelo().ObtenerTodos().ToList());
            }
            else
            {
                ListItems = new ObservableCollection<DECOMISO_MODELO>(new cDecomisoModelo().ObtenerTodos().ToList()
                    .Where(w => w.DESCR.Contains(Busqueda) ||
                        w.ESTATUS.Contains(Busqueda) ||
                        w.DECOMISO_FABRICANTE.DESCR.Contains(Busqueda)));
            }
            if (ListItems.Count > 0)
                EmptyVisible = false;
            else
                EmptyVisible = true;
            setValidationRules();
            return ListItems;
        }

        public bool Comprobar(System.Windows.Controls.UserControl ventana)
        {
            var usercontr = (System.Windows.Controls.UserControl)ventana;
            var Lista = (System.Windows.Controls.ListView)usercontr.FindName("lvMedia");
            if (Lista.HasItems)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //private async void ModelosMarcasLoad(CatalogoMarcas_ModelosView obj)
        //{
        //    try
        //    {
        //        var lista = await StaticSourcesViewModel.CargarDatosMetodo<ObservableCollection<DECOMISO_MODELO>>(() => CargarLoading());
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar Modelos-Marcas.", ex);
        //    }


        //}

        private async void ModelosMarcasLoad(CatalogoMarcas_ModelosView Window)
        {
            try
            {
                CatalogoHeader = "Marcas-Modelo";
                HeaderAgregar = "Agregar Nuevo Marcas-Modelo";
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    // ListItems = new ObservableCollection<DECOMISO_MODELO>();
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
                    // this.GetMarcasModelo();
                    this.setValidationRules();
                    ConfiguraPermisos();
                    GetMarcasModelo();
                    //var Res= Window.Dispatcher.Invoke(new System.Action(() =>
                    // {
                    //     bool Salir=false;
                    //     while(Salir==false){
                    //         var usercontr = (System.Windows.Controls.UserControl)Window;
                    //         var Lista = (System.Windows.Controls.ListView)usercontr.FindName("lvMedia");
                    //         if (Lista.HasItems)
                    //     {
                    //     Salir=true;
                    //     }
                    //     }

                    //}), null);
                    //return Res;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar Marcas-Modelo.", ex);
            }
        }

        private void PopularCombo()
        {
            Lista_Fabricantes = new cDecomisoFabricante().ObtenerTodos().ToList();
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetMarcasModelo();
            }
        }

        private void GetMarcasModelo()
        {
            try
            {
                ListItems = new ObservableCollection<DECOMISO_MODELO>(new cDecomisoModelo().ObtenerTodos(Busqueda).ToList());
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

        private void GuardarMarcaModelo()
        {
            try
            {
                if (Clave > 0)
                {
                    //Actualizar
                    //SelectedItem.DESCR = Descripcion;
                    //SelectedItem.ESTATUS = SelectedEstatus.CLAVE;
                    //SelectedItem.ID_FABRICANTE = SelectedFabricante.ID_FABRICANTE;
                    //SelectedItem.ID_MODELO = Clave;
                    //tatuaje.Actualizar(SelectedItem);
                    decomisomodelo.Actualizar(new DECOMISO_MODELO
                    {
                        ID_FABRICANTE = SelectedFabricante.ID_FABRICANTE,//.ID_FABRICANTE,
                        ID_MODELO = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                else
                {   //Agregar
                    decomisomodelo.Insertar(new DECOMISO_MODELO
                    {
                        ID_FABRICANTE = SelectedFabricante.ID_FABRICANTE,
                        ID_MODELO = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                Busqueda = string.Empty;
                //Mostrar Listado
                this.GetMarcasModelo();
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
                if (SelectedItem != null || SelectedItem.ID_MODELO >= 100)
                {
                    if (!decomisomodelo.Eliminar(SelectedItem))
                        return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    Busqueda = string.Empty;
                    this.GetMarcasModelo();
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_MARCA_MODELO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
