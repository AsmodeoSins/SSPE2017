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
    partial class ProcedimientosSubtipoViewModel : ValidationViewModelBase
    {
        public ProcedimientosSubtipoViewModel()
        {
            //EmptyVisible = false;
            ////Listado 
            //ListItems = new List<PROC_MED_SUBTIPO>();
            //CatalogoHeader = "Procedimientos Medicos Subtipos";
            //HeaderAgregar = "Agregar Procedimiento Medico Subtipo";
            ////LLENAR 
            //EditarVisible = false;
            //NuevoVisible = false;
            //AgregarVisible = false;
            //GuardarMenuEnabled = false;
            //EliminarMenuEnabled = false;
            //EditarMenuEnabled = false;
            //CancelarMenuEnabled = false;
            //AyudaMenuEnabled = true;
            //SalirMenuEnabled = true;
            //ExportarMenuEnabled = true;
            ///*MAXLENGTH*/
            //MaxLength = 20;
            //SeleccionIndice = -1;
            ////Obtenemos las Etnias
            //this.GetProcedimientosSubtipo();
            //this.setValidationRules();
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetProcedimientosSubtipo();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Procedimiento Medico Subtipo";
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
                        Clave = SelectedItem.ID_PROCMED_SUBTIPO;
                        SelectTipoAtencionAgregar = ListTipoAtencion.Any(f => SelectedItem.ID_TIPO_ATENCION.HasValue ? f.ID_TIPO_ATENCION == SelectedItem.ID_TIPO_ATENCION.Value : false) ?
                            ListTipoAtencion.First(f => SelectedItem.ID_TIPO_ATENCION.HasValue ? f.ID_TIPO_ATENCION == SelectedItem.ID_TIPO_ATENCION.Value : false) : ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == -1);
                        Descripcion = SelectedItem.DESCR.TrimEnd();
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
                    HeaderAgregar = "Agregar Procedimiento Medico Subtipo";
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
                    SelectTipoAtencionAgregar = ListTipoAtencion!=null ? ListTipoAtencion.Any() ?
                        ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == -1) : ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == -1) : ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == -1);
                    OnPropertyChanged("SelectTipoAtencionAgregar");
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
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
                        this.GuardarProcedimientoSubtipo();
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
                    SelectTipoAtencionAgregar = ListTipoAtencion.Any(f => SelectedItem.ID_TIPO_ATENCION.HasValue ? f.ID_TIPO_ATENCION == SelectedItem.ID_TIPO_ATENCION.Value : false) ?
                        ListTipoAtencion.First(f => SelectedItem.ID_TIPO_ATENCION.HasValue ? f.ID_TIPO_ATENCION == SelectedItem.ID_TIPO_ATENCION.Value : false) : ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == -1);
                    SelectedEstatus = null;
                    Busqueda = string.Empty;
                    //this.GuardarTipoLunar();
                    GetProcedimientosSubtipo();
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
                            if (this.EliminarProcedimientoSubtipo())
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
                GuardarProcedimientoSubtipo();
            }
        }

        private void GetProcedimientosSubtipo()
        {
            try
            {
                var procSubtipo = new cProcedimientosSubtipo();
                ListItems.Clear();
                var list = procSubtipo.ObtenerXBusquedaYTipoAtencion(Busqueda, SelectTipoAtencion);
                ListItems = list.Any() ? list.ToList() : new List<PROC_MED_SUBTIPO>();
                EmptyVisible = !(ListItems.Count > 0);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GuardarProcedimientoSubtipo()
        {
            try
            {
                var procSubtipo = new cProcedimientosSubtipo();
                if (Clave > 0)
                {  //Actualizar
                    SelectedItem.DESCR = Descripcion;
                    SelectedItem.ESTATUS = SelectedEstatus.CLAVE;
                    procSubtipo.Actualizar(new PROC_MED_SUBTIPO
                    {
                        ID_PROCMED_SUBTIPO = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE,
                        ID_TIPO_ATENCION = SelectTipoAtencionAgregar.ID_TIPO_ATENCION
                    });
                }
                else
                {   //Agregar
                    procSubtipo.Insertar(new PROC_MED_SUBTIPO
                    {
                        ID_PROCMED_SUBTIPO = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE,
                        ID_TIPO_ATENCION = SelectTipoAtencionAgregar.ID_TIPO_ATENCION
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                SelectedEstatus = null;
                Busqueda = string.Empty;
                SelectTipoAtencionAgregar = ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == -1);
                SelectTipoAtencion = -1;
                //Mostrar Listado
                this.GetProcedimientosSubtipo();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool EliminarProcedimientoSubtipo()
        {
            try
            {
                if (SelectedItem != null || SelectedItem.ID_PROCMED_SUBTIPO > 0)
                {
                    var procSubtipo = new cProcedimientosSubtipo();
                    SelectedItem.ESTATUS = "N";
                    if (!procSubtipo.Actualizar(SelectedItem))
                        return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    SelectTipoAtencionAgregar = ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == -1);
                    Busqueda = string.Empty;
                    SelectTipoAtencion = -1;
                    //this.GuardarTipoLunar();
                    this.GetProcedimientosSubtipo();
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        //LOAD
        private async void ProcedimientoSubtipoLoad(ProcedimientosSubtipoView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new List<PROC_MED_SUBTIPO>();
                    CatalogoHeader = "Procedimientos Medicos Subtipos";
                    HeaderAgregar = "Agregar Procedimiento Medico Subtipo";
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
                    ListTipoAtencion = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListTipoAtencion.Insert(0, new ATENCION_TIPO { DESCR = "SELECCIONE", ID_TIPO_ATENCION = -1 });
                        SelectTipoAtencion = -1;
                        SelectTipoAtencionAgregar = ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == -1);
                    }));
                    GetProcedimientosSubtipo();
                    setValidationRules();
                    ConfiguraPermisos();
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_PROCEDIMIENTO_SUBTIPO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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