using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace ControlPenales
{
    partial class Compania2ViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public Compania2ViewModel() { }

        void IPageViewModel.inicializa() { }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    Obtener();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Compañía";
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
                        /*****************************************/
                        Clave = SelectedItem.ID_COMPANIA;
                        Descripcion = SelectedItem.DESCR.TrimEnd();
                        Estatus = string.IsNullOrEmpty(SelectedItem.ESTATUS) ? "N" : SelectedItem.ESTATUS;
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
                    HeaderAgregar = "Agregar Compañía";
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
                    Estatus = "S";
                    //SelectedEstatus = null;
                    /********************************/
                    break;
                case "menu_guardar":
                    //if (!string.IsNullOrEmpty(Descripcion) && SelectedEstatus != null)
                    if (!base.HasErrors)
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
                        Guardar();
                        /**********************************/
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar lo campos obligatorios");
                        FocusText = true;
                    }
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
                    //SelectedEstatus = null;
                    Estatus = "S";
                    Busqueda = string.Empty;
                    Obtener();
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
                        var result = await metro.ShowMessageAsync("Eliminar", "¿Está seguro que desea eliminar... [ " + SelectedItem.DESCR.Trim() + " ]?", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            Eliminar();
                            //BaseMetroDialog dialog;
                            //if (this.EliminarEtnia())
                            //{
                            //    dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                            //}
                            //else
                            //{
                            //    dialog = (BaseMetroDialog)metro.Resources["ErrorDialog"];
                            //}
                            //await metro.ShowMetroDialogAsync(dialog);
                            //await TaskEx.Delay(1500);
                            //await metro.HideMetroDialogAsync(dialog);
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
                this.Obtener();
            }
        }

        private void Obtener()
        {
            try
            {
                ListItems.Clear();
                ListItems = new List<COMPANIA>(new cCompania().ObtenerTodos(Busqueda));
                EmptyVisible = ListItems.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la compañía.", ex);
            }
        }

        private void Guardar()
        {
            try
            {
                //var obj = new COMPANIA();
                //obj.DESCR = Descripcion;
                //obj.ESTATUS = Estatus;//SelectedEstatus.CLAVE;
                //if (SelectedItem == null)
                //{
                //    if (new cCompania().Insertar(obj) > 0)
                //        Limpiar();
                //    else
                //        new Dialogos().ConfirmacionDialogo("Error","Ocurrio un problema al guardar la compañia");
                //}
                //else
                //{
                //    obj.ID_COMPANIA = SelectedItem.ID_COMPANIA;
                //    if (new cCompania().Actualizar(obj))
                //        Limpiar();
                //    else
                //        new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un problema al guardar la compañia");
                //}
                #region Comentado
                //if (Clave > 0)
                //{  //Actualizar
                //    SelectedItem.DESCR = Descripcion;
                //    SelectedItem.ESTATUS = SelectedEstatus.CLAVE;
                //    new cTipoDiscapacidad().Actualizar(new ETNIA { ID_ETNIA = Clave, DESCR = SelectedItem.DESCR, ESTATUS = SelectedEstatus.CLAVE });
                //}
                //else
                //{   //Agregar
                //    etnia.Insertar(new ETNIA { ID_ETNIA = Clave, DESCR = Descripcion, ESTATUS = SelectedEstatus.CLAVE });
                //}
                ////Limpiamos las variables
                //Clave = 0;
                //Descripcion = string.Empty;
                //Busqueda = string.Empty;
                //SelectedEstatus = null;
                ////Mostrar Listado
                //GetEtnias();
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la compañía.", ex);
            }
        }

        private void Limpiar()
        {
            //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CatalogoSimpleNewView();
            //((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.CompaniaViewModel();
        }

        private void Eliminar()
        {
            try
            {
                if (SelectedItem != null)
                {
                    if (new cCompania().Eliminar(SelectedItem.ID_COMPANIA))
                        Limpiar();
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "Ocurrió un problema al eliminar la compañía");
                    #region Comentado
                    //cEtnia etnia = new cEtnia();
                    //if (!etnia.Eliminar(SelectedItem.))
                    //    return false;
                    //Clave = 0;
                    //Descripcion = string.Empty;
                    //SelectedEstatus = null;
                    //Busqueda = string.Empty;
                    //this.GetEtnias();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar la compañía.", ex);
            }
        }

        private async void WindowLoad(CatalogoSimpleNewView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new List<COMPANIA>();
                    CatalogoHeader = "Compañía";
                    HeaderAgregar = "Agregar Compañía";
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
                    MaxLength = 14;
                    SeleccionIndice = -1;
                    //Obtenemos las Etnias
                    Obtener();
                    setValidationRules();
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }
    }
}