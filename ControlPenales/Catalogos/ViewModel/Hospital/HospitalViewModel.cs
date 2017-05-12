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
    partial class HospitalViewModel : ValidationViewModelBase
    {
        public HospitalViewModel() { }

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
                        HeaderAgregar = "Editar Hospital";
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
                        Clave = SelectedItem.ID_HOSPITAL;
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
                    HeaderAgregar = "Agregar Hospital";
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
                if(pConsultar)
                    this.Obtener();
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
            }
        }

        private void Obtener()
        {
            try
            {
                ListItems.Clear();
                if (pConsultar)
                    ListItems = new List<HOSPITAL>(new cHospitales().ObtenerTodo(Busqueda));
                else
                    ListItems = new List<HOSPITAL>();
                EmptyVisible = ListItems.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void Guardar()
        {
            try
            {
                var obj = new HOSPITAL();
                obj.DESCR = Descripcion;
                obj.ESTATUS = Estatus;
                if (SelectedItem == null)
                {
                    if (pInsertar)
                    {
                        if (new cHospitales().Insertar(obj) > 0)
                            Limpiar();
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un problema al guardar");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                    
                }
                else
                {
                    if (pEditar)
                    {
                        obj.ID_HOSPITAL = SelectedItem.ID_HOSPITAL;
                        if (new cHospitales().Actualizar(obj))
                            Limpiar();
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un problema al guardar");
                    }
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                }
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private void Limpiar()
        {
            try
            {
                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CatalogoSimpleNewView();
                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.HospitalViewModel();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar la pantalla.", ex);
            }
        }

        private void Eliminar()
        {
            try
            {
                if (SelectedItem != null)
                {
                    if (pEditar)
                    {
                        if (new cHospitales().Eliminar(SelectedItem.ID_HOSPITAL))
                            Limpiar();
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un problema al eliminar");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
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
                    ListItems = new List<HOSPITAL>();
                    CatalogoHeader = "Hospitales";
                    HeaderAgregar = "Agregar Hospital";
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
                    MaxLength = 100;
                    SeleccionIndice = -1;
                    //Obtenemos las Etnias
                    ConfiguraPermisos();
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


        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_INSTITUCIONES_MEDICAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                    { 
                        pInsertar = true;
                    }
                    if (p.CONSULTAR == 1)
                    {
                        pConsultar = true;
                    }
                    if (p.EDITAR == 1)
                    { 
                        pEditar = true;
                    }
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