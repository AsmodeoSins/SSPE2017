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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlPenales
{
    partial class ProcedimientosMaterialesViewModel : ValidationViewModelBase
    {
        public ProcedimientosMaterialesViewModel()
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
                    ExportarMenuEnabled = false;
                    AgregarVisible = false;
                    #endregion
                    this.GetProcedimientosMedicos();
                    break;
                case "menu_editar":
                    if (SelectProcMed == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento medico.");
                        return;
                    }
                    if (SelectProcMat == null)
                    {
                        bandera_editar = false;
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un material.");
                        return;
                    }
                    HeaderAgregar = "Editar Procedimiento Material";
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
                    MaterialesEnabled = false;
                    bandera_editar = true;
                    FocusText = true;
                    /*****************************************/
                    Clave = SelectProcMat.ID_PROCMED;
                    SelectSubtipoAgregar = null;
                    SelectSubtipoAgregar = ListSubtipos.First(f => f.ID_PROCMED_SUBTIPO == SelectProcMed.ID_PROCMED_SUBTIPO);
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.First(w => w.CLAVE == SelectProcMat.ESTATUS);
                    SelectProducto = SelectProcMat.PRODUCTO;
                    ProductoSeleccionado = SelectProcMat.PRODUCTO.NOMBRE.Trim();
                    /*****************************************/
                    break;
                case "menu_agregar":
                    if (SelectProcMed == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un procedimiento medico.");
                        return;
                    }
                    HeaderAgregar = "Agregar Procedimiento Material";
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
                    MaterialesEnabled = true;
                    //Descripcion = string.Empty;
                    //SelectSubtipoAgregar = null;
                    //SelectSubtipoAgregar = ListSubtipos.First(f => f.ID_PROCMED_SUBTIPO == SelectProcMed.ID_PROCMED_SUBTIPO);
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    SelectProducto = null;
                    ProductoSeleccionado = string.Empty;
                    SelectProcMat = null;
                    /********************************/
                    break;
                case "menu_guardar":
                    if (SelectedEstatus != null ? SelectedEstatus.CLAVE != "S" && SelectedEstatus.CLAVE != "N" : true)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar el estatus.");
                        return;
                    }
                    if (SelectProducto == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un material.");
                        return;
                    }
                    #region visiblePantalla
                    EditarVisible = false;
                    NuevoVisible = false;
                    GuardarMenuEnabled = false;
                    //AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    //CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    //AgregarVisible = false;
                    #endregion
                    /**********************************/
                    this.GuardarProcedimientoMateriales();
                    /**********************************/
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
                    SelectProcMat = null;
                    SelectProcMed = null;
                    #endregion
                    /****************************************/
                    SeleccionIndice = -1;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    Busqueda = string.Empty;
                    //this.GuardarTipoLunar();
                    GetProcedimientosMedicos();
                    /****************************************/
                    break;
                case "menu_eliminar":
                    /*
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
                            if (this.EliminarProcedimientoMateriales())
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
                    SeleccionIndice = -1;*/
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
                GetProcedimientosMedicos();
            }
        }

        private void GetProcedimientosMedicos()
        {
            try
            {
                var procMed = new cProcedimientosMedicos();
                if (ListProcMeds != null)
                    ListProcMeds.Clear();
                var list = procMed.ObtenerXBusquedaYSubtipo(Busqueda, SelectSubtipo);
                ListProcMeds = list.Any() ? list.ToList() : new List<PROC_MED>();
                SelectProcMed = null;
                ListProcMats = new ObservableCollection<PROC_MATERIAL>();
                EmptyProcMedsVisible = ListProcMeds.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                EmptyProcMatsVisible = ListProcMats != null ? ListProcMats.Count > 0 ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GetProcedimientosMateriales()
        {
            try
            {
                var procMat = new cProcedimientosMateriales();
                if (ListProcMeds != null)
                    ListProcMeds.Clear();
                var list = procMat.ObtenerXBusquedaYSubtipo(Busqueda, SelectSubtipo);
                ListProcMats = list.Any() ? new ObservableCollection<PROC_MATERIAL>(list.ToList()) : new ObservableCollection<PROC_MATERIAL>();
                EmptyProcMatsVisible = ListProcMats.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GuardarProcedimientoMateriales()
        {
            try
            {
                var procMed = new cProcedimientosMateriales();
                var hoy = Fechas.GetFechaDateServer;
                if (Clave > 0)
                {  //Actualizar
                    //SelectedItem.DESCR = Descripcion;
                    SelectProcMat.ESTATUS = SelectedEstatus.CLAVE;
                    procMed.Actualizar(new PROC_MATERIAL
                    {
                        ID_PROCMED = SelectProcMed.ID_PROCMED,
                        ESTATUS = SelectedEstatus.CLAVE,
                        REGISTRO_FEC = SelectProcMat.REGISTRO_FEC,
                        ID_PRODUCTO = SelectProcMat.ID_PRODUCTO,
                        INACTIVO_FEC = SelectProcMat.ESTATUS == "N" ? SelectProcMat.INACTIVO_FEC : SelectedEstatus.CLAVE == "N" ? hoy : new Nullable<DateTime>()
                    });
                }
                else
                {   //Agregar
                    procMed.Insertar(new PROC_MATERIAL
                    {
                        ID_PROCMED = SelectProcMed.ID_PROCMED,
                        ESTATUS = SelectedEstatus.CLAVE,
                        REGISTRO_FEC = hoy,
                        ID_PRODUCTO = SelectProducto != null ? SelectProducto.ID_PRODUCTO : 0,
                        INACTIVO_FEC = SelectedEstatus.CLAVE == "N" ? hoy : new Nullable<DateTime>()
                    });
                }
                //Limpiamos las variables
                var procmed = SelectProcMed;
                Clave = 0;
                Descripcion = string.Empty;
                SelectedEstatus = null;
                Busqueda = string.Empty;
                SelectSubtipoAgregar = ListSubtipos.First(f => f.ID_PROCMED_SUBTIPO == -1);
                SelectSubtipo = -1;
                //Mostrar Listado
                this.GetProcedimientosMedicos();
                ListProcMeds = new cProcedimientosMedicos().ObtenerTodosActivos().ToList();
                SelectProcMed = ListProcMeds.First(f=>f.ID_PROCMED == procmed.ID_PROCMED && f.ID_PROCMED_SUBTIPO == procmed.ID_PROCMED_SUBTIPO);
                SelectProducto = null;
                ProductoSeleccionado = string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool EliminarProcedimientoMateriales()
        {
            try
            {/*
                if (SelectedItem != null || SelectedItem.ID_PROCMED_SUBTIPO > 0)
                {
                    var procMed = new cProcedimientosMedicos();
                    SelectedItem.ESTATUS = "N";
                    if (!procMed.Actualizar(SelectedItem))
                        return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    SelectSubtipoAgregar = ListSubtipos.First(f => f.ID_PROCMED_SUBTIPO == -1);
                    Busqueda = string.Empty;
                    SelectSubtipo = -1;
                    //this.GuardarTipoLunar();
                    this.GetProcedimientosMateriales();
                }*/
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        //LOAD
        private async void ProcedimientoMaterialLoad(ProcedimientosMaterialesView obj)
        {
            try
            {
                if (obj == null) return;
                AutoCompleteReceta = obj.AutoCompleteReceta;
                AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", obj.AutoCompleteReceta) as ListBox;
                AutoCompleteReceta.PreviewMouseDown += new MouseButtonEventHandler(MouseUpReceta);
                AutoCompleteReceta.KeyDown += KeyDownReceta;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    //Listado 
                    //ListItems = new List<PROC_MED>();
                    CatalogoHeader = "Procedimientos Medicos";
                    HeaderAgregar = "Agregar Procedimiento Material";
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
                    ListSubtipos = new ObservableCollection<PROC_MED_SUBTIPO>(new cProcedimientosSubtipo().ObtenerTodosActivos());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListSubtipos.Insert(0, new PROC_MED_SUBTIPO { DESCR = "TODOS", ID_PROCMED_SUBTIPO = -1 });
                        SelectSubtipo = -1;
                        SelectSubtipoAgregar = ListSubtipos.First(f => f.ID_PROCMED_SUBTIPO == -1);
                    }));
                    GetProcedimientosMedicos();
                    setValidationRules();
                    ConfiguraPermisos();
                    AgregarVisible = false;
                    StaticSourcesViewModel.SourceChanged = false;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        private void MouseUpReceta(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", AutoCompleteReceta) as ListBox;
                var dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is ListBoxItem))
                    dep = VisualTreeHelper.GetParent(dep);
                if (dep == null) return;
                var item = AutoCompleteRecetaLB.ItemContainerGenerator.ItemFromContainer(dep);
                if (item == null) return;
                if (item is RecetaMedica)
                {
                    ProductoSeleccionado = ((RecetaMedica)item).PRODUCTO.NOMBRE.Trim();
                    SelectProducto = ((RecetaMedica)item).PRODUCTO;
                    AutoCompleteReceta.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        private void KeyDownReceta(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter || e.Key == Key.Return)
                {
                    var popup = AutoCompleteReceta.Template.FindName("PART_Popup", AutoCompleteReceta) as System.Windows.Controls.Primitives.Popup;
                    AutoCompleteRecetaLB = AutoCompleteReceta.Template.FindName("PART_ListBox", AutoCompleteReceta) as ListBox;
                    var dep = (DependencyObject)e.OriginalSource;
                    while ((dep != null) && !(dep is ListBoxItem))
                        dep = VisualTreeHelper.GetParent(dep);
                    if (dep == null) return;
                    var item = AutoCompleteRecetaLB.ItemContainerGenerator.ItemFromContainer(dep);
                    if (item == null) return;
                    if (item is RecetaMedica)
                    {
                        ProductoSeleccionado = ((RecetaMedica)item).PRODUCTO.NOMBRE.Trim();
                        SelectProducto = ((RecetaMedica)item).PRODUCTO;
                        AutoCompleteReceta.Text = string.Empty;
                        popup.IsOpen = false;
                    }
                }
                else if (e.Key == Key.Tab) { }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar una enfermedad.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_PROCEDIMIENTO_MATERIAL.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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