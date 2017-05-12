using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;

namespace ControlPenales
{
    partial class CatalogoPaisesViewModel : ValidationViewModelBase, IPageViewModel
    {
        cPaises obj = new cPaises();

        public CatalogoPaisesViewModel()
        {
            CatalogoHeader = "Países";
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
            MaxLength = 20;
            SeleccionIndice = -1;
            EmptyVisible = false;
            //Listado 
            ListItems = new List<PAIS_NACIONALIDAD>();
            //Obtenemos las Etnias
            this.GetPaises();
        }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetPaises();
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
                        Clave = SelectedItem.ID_PAIS_NAC;
                        Descripcion = SelectedItem.PAIS;
                        Nacionalidad = SelectedItem.NACIONALIDAD;
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
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
                    Nacionalidad = string.Empty;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    Clave = 0;
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Descripcion) && !string.IsNullOrEmpty(Nacionalidad) && SelectedEstatus != null)
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
                        this.GuardarPais();
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
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.PAIS + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
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
                                await metro.ShowMessageAsync("Algo ocurrió...", "No se puede eliminar información del País: Tiene dependencias.", MessageDialogStyle.Affirmative, mySettings);
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
                GetPaises();
            }
        }

        private void GetPaises()
        {
            try
            {
                ListItems.Clear();
                ListItems = new List<PAIS_NACIONALIDAD>(obj.ObtenerTodos(Busqueda));
                ListItems = new List<PAIS_NACIONALIDAD>(ListItems.OrderBy(x => x.PAIS));
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

        private void GuardarPais()
        {
            try
            {
                if (Clave > 0)
                {  //Actualizar
                    SelectedItem.PAIS = Descripcion;
                    SelectedItem.ESTATUS = SelectedEstatus.CLAVE;
                    SelectedItem.NACIONALIDAD = Nacionalidad;
                    obj.Actualizar(new PAIS_NACIONALIDAD()
                    {
                        ID_PAIS_NAC = SelectedItem.ID_PAIS_NAC,
                        NACIONALIDAD = SelectedItem.NACIONALIDAD,
                        PM = SelectedItem.PM,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                else
                {   //Agregar
                    PAIS_NACIONALIDAD newItem = new PAIS_NACIONALIDAD();
                    newItem.ID_PAIS_NAC = Clave;
                    newItem.PAIS = Descripcion;
                    newItem.ESTATUS = SelectedEstatus.CLAVE;
                    newItem.NACIONALIDAD = Nacionalidad;
                    newItem.PM = 0;
                    obj.Insertar(newItem);

                    //var f = tf.MEDIA_FILIACION.DESCR;
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                //Mostrar Listado
                GetPaises();
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
                if (SelectedItem != null || SelectedItem.ID_PAIS_NAC >= 100)
                {
                    if (obj.Eliminar(Convert.ToInt32(SelectedItem.ID_PAIS_NAC)))
                    {
                        GetPaises();
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
        private async void PaisLoad(CatalogoPaisesView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
              {
                  EmptyVisible = false;
                  //Listado 
                  ListItems = new List<PAIS_NACIONALIDAD>();
                  CatalogoHeader = "Países";
                  HeaderAgregar = "Agregar País";
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
                  //Obtenemos los países
                  this.GetPaises();
                  this.setValidationRules();
                  ConfiguraPermisos();
                  StaticSourcesViewModel.SourceChanged = false;
              });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de país.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_PAISES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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