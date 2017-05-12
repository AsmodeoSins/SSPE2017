using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Transactions;
using System.Linq;
using System.Collections.Generic;

namespace ControlPenales
{
    partial class CatalogoAmparoIncidenteTipoViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public CatalogoAmparoIncidenteTipoViewModel() { }

        void IPageViewModel.inicializa() { }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetObject();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Tipo de Incidente";
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
                        Clave = SelectedItem.ID_AMP_INC_TIPO;
                        //Corta al campo descripcion los espacios restantes.
                        Descripcion = SelectedItem.DESCR.TrimEnd();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                        AplicaCausaPenal = SelectedItem.APLICA_CAUSA_PENAL.Equals("S") ? true : false;
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
                    HeaderAgregar = "Agregar Tipo de Incidente";
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
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    AplicaCausaPenal = false;
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Descripcion) && SelectedEstatus != null)
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
                        this.GuardarObject();
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
                    SelectedEstatus = null;
                    Busqueda = string.Empty;
                    this.GetObject();
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
                        var result = await metro.ShowMessageAsync("Eliminar", "¿Está seguro que desea eliminar... [ " + SelectedItem.DESCR + " ]?", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            BaseMetroDialog dialog;
                            if (this.EliminarObject())
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

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                this.GetObject();
            }
        }

        private void GetObject()
        {
            try
            {
                ListItems.Clear();
                ListItems = new List<AMPARO_INCIDENTE_TIPO>(new cAmparoIncidenteTipo().ObtenerTodos(Busqueda,string.Empty,string.Empty));
                if (ListItems.Count > 0)

                    EmptyVisible = false;

                else
                    EmptyVisible = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener tipos de amparo incidente.", ex);
            }
        }

        private void GuardarObject()
        {
            try
            {
                cAmparoIncidenteTipo incidente = new cAmparoIncidenteTipo();
                if (Clave > 0)
                {
                    //Actualizar
                    SelectedItem.DESCR = Descripcion;
                    SelectedItem.ESTATUS = SelectedEstatus.CLAVE;
                    //tatuaje.Actualizar(SelectedItem);

                    incidente.Actualizar(new AMPARO_INCIDENTE_TIPO { ID_AMP_INC_TIPO = Clave, DESCR = SelectedItem.DESCR, ESTATUS = SelectedEstatus.CLAVE, APLICA_CAUSA_PENAL = AplicaCausaPenal ? "S" : "N" });
                }
                else
                {   //Agregar
                    incidente.Insertar(new AMPARO_INCIDENTE_TIPO { ID_AMP_INC_TIPO = Clave, DESCR = Descripcion, ESTATUS = SelectedEstatus.CLAVE, APLICA_CAUSA_PENAL = AplicaCausaPenal ? "S" : "N" });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                Busqueda = string.Empty;
                SelectedEstatus = null;
                //Mostrar Listado
                GetObject();
                //SSPEntidades Context = new SSPEntidades();
                //using (var transaction = new TransactionScope())
                //{
                //try
                //{
                //    var obj = new AMPARO_INCIDENTE_TIPO();
                //    obj.DESCR = Descripcion;
                //    obj.APLICA_CAUSA_PENAL = AplicaCausaPenal ? "S" : "N";
                //    obj.ESTATUS = SelectedEstatus.CLAVE;
                //    if (SelectedItem != null)
                //    {  //Actualizar
                //        obj.ID_AMP_INC_TIPO = SelectedItem.ID_AMP_INC_TIPO;
                //        new cAmparoIncidenteTipo().Actualizar(obj);
                //    }
                //    else
                //    {   //Agregar
                //        new cAmparoIncidenteTipo().Insertar(obj);
                //    }
                //    //Limpiamos las variables
                //    Clave = 0;
                //    Descripcion = string.Empty;
                //    SelectedEstatus = null;
                //    Busqueda = string.Empty;
                //    AplicaCausaPenal = false;
                //    this.GetObject();
                //    //transaction.Complete();
                //}
                //catch (Exception ex)
                //{
                //    //transaction.Dispose();
                //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar tipo de amparo incidente.", ex);
                //}
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar tipo de amparo incidente.", ex);
            }
        }

        private bool EliminarObject()
        {
            try
            {
                if (SelectedItem != null)
                {
                    if (!new cAmparoIncidenteTipo().Eliminar(SelectedItem.ID_AMP_INC_TIPO))
                        return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    AplicaCausaPenal = false;
                    Busqueda = string.Empty;
                    this.GetObject();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar tipo de amparo incidente.", ex);
                return false;
            }
            return true;
        }

        //LOAD
        private async void AmparoIncidenteTipoLoad(AmparoIncidenteTipoView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
               {
                   EmptyVisible = false;
                   //Listado 
                   ListItems = new List<AMPARO_INCIDENTE_TIPO>();
                   CatalogoHeader = "Tipo de Incidente";
                   HeaderAgregar = "Agregar Tipo de Incidente";
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
                   //Obtenemos las Etnias
                   this.GetObject();
                   this.setValidationRules();
                   ConfiguraPermisos();
               });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de amparo incidente.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_TIPO_INCIDENTE.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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