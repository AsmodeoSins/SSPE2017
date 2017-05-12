﻿using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class CatalogoTipoDelitoViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public CatalogoTipoDelitoViewModel()
        {
            //EmptyVisible = false;
            ////Listado 
            //ListItems = new List<TIPO_DELITO>();

            //CatalogoHeader = "Tipo de Delito";
            //HeaderAgregar = "Agregar Nuevo Tipo de Delito";
            ////LLENAR 
            //EditarVisible = false;
            //NuevoVisible = false;
            //AgregarVisible = false;
            //GuardarMenuEnabled = false;
            //AgregarMenuEnabled = true;
            //EliminarMenuEnabled = false;
            //EditarMenuEnabled = false;
            //CancelarMenuEnabled = false;
            //AyudaMenuEnabled = true;
            //SalirMenuEnabled = true;
            //ExportarMenuEnabled = true;

            ///*MAXLENGTH*/
            //MaxLength = 100;
            //SeleccionIndice = -1;

            ////Obtenemos las Etnias
            //this.GetTiposDelitos();
            //this.setValidationRules();
        }

        void IPageViewModel.inicializa() { }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetTiposDelitos();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Tipo de Delito";
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
                        Clave = SelectedItem.ID_TIPO_DELITO;
                        Descripcion = SelectedItem.DESCR;
                        /*****************************************/
                        //PopularEstatusCombo();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                    }
                    else
                    {
                        bandera_editar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una opción.");
                    }
                    break;
                case "menu_agregar":
                    HeaderAgregar = "Agregar Nuevo Tipo de Delito";
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
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    SelectedItem = null;
                    //PopularEstatusCombo();
                    Descripcion = string.Empty;
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
                        this.GuardarTipoDelito();
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
                    this.GetTiposDelitos();
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
                            if (EliminarTipoDelito())
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

        //private void PopularEstatusCombo()
        //{
        //    var ListaEstatus_ = new Clases.Estatus.EstatusControl().LISTA_ESTATUS;
        //    ListEstatus = new ObservableCollection<ControlPenales.Clases.Estatus.Estatus>(ListaEstatus_);
        //}

        private async void TipoDelitosLoad(CatalogoSimpleView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new List<TIPO_DELITO>();
                    CatalogoHeader = "Tipo de Delito";
                    HeaderAgregar = "Agregar Nuevo Tipo de Delito";
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
                    this.GetTiposDelitos();
                    this.setValidationRules();
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de delito.", ex);
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetTiposDelitos();
            }
        }

        private void GetTiposDelitos()
        {
            try
            {
                cTipoDelito tipoDelito = new cTipoDelito();
                ListItems.Clear();
                ListItems = tipoDelito.ObtenerTodos().ToList();
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

        private void GuardarTipoDelito()
        {
            try
            {
                cTipoDelito tipoDelito = new cTipoDelito();
                if (Clave > 0)
                {  //Actualizar
                    SelectedItem.ID_TIPO_DELITO = Clave;
                    SelectedItem.DESCR = Descripcion;
                    SelectedItem.ESTATUS = SelectedEstatus.CLAVE;
                    tipoDelito.Actualizar(new TIPO_DELITO
                    {
                        ID_TIPO_DELITO = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                else
                {   //Agregar
                    tipoDelito.Insertar(new TIPO_DELITO
                    {
                        ID_TIPO_DELITO = Clave,
                        DESCR = Descripcion,
                        ESTATUS = SelectedEstatus.CLAVE
                    });
                }
                //Limpiamos las variables
                Clave = 0;
                Descripcion = string.Empty;
                SelectedEstatus = null;
                Busqueda = string.Empty;
                //Mostrar Listado
                this.GetTiposDelitos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private bool EliminarTipoDelito()
        {
            try
            {
                if (SelectedItem != null || SelectedItem.ID_TIPO_DELITO >= 100)
                {
                    cTipoDelito tipoDelito = new cTipoDelito();
                    if (!tipoDelito.Eliminar(SelectedItem.ID_TIPO_DELITO))
                        return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    SelectedEstatus = null;
                    Busqueda = string.Empty;
                    this.GetTiposDelitos();
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
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_DELITOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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