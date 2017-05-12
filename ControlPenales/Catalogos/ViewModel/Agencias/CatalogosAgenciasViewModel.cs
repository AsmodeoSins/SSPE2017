using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    partial class CatalogosAgenciasViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        cAgencia agencia_controlador = new cAgencia();
        cCentro objCentro = new cCentro();
        cEntidad objEstado = new cEntidad();
        cMunicipio objCiudades = new cMunicipio();

        public CatalogosAgenciasViewModel() { }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetAgencias();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Agencias";
                        #region visiblePantalla
                        EditarVisible = true;
                        NuevoVisible = false;
                        GuardarMenuEnabled = true;
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
                        Clave = SelectedItem.ID_AGENCIA;
                        Descripcion = SelectedItem.DESCR == null ? SelectedItem.DESCR : SelectedItem.DESCR.TrimEnd();
                        Domicilio = SelectedItem.DOMICILIO == null ? SelectedItem.DOMICILIO : SelectedItem.DOMICILIO.TrimEnd();
                        Entidad = ListEntidad.Where(w => w.ID_ENTIDAD == SelectedItem.ID_ENTIDAD).FirstOrDefault();
                        SelectMunicipio = ListMunicipio.Where(w => w.ID_MUNICIPIO == SelectedItem.ID_MUNICIPIO).FirstOrDefault();
                        SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                        string TipoAgencia = SelectedItem.TIPO_AGENCIA == "E" ? "ESTATAL" : "FEDERAL";
                        var _SelectAgenciaTipo = ListaAgenciasTipo.Where(w => w.Tipoagencia == TipoAgencia).SingleOrDefault();
                        SelectTipoAgenciaIndex = _SelectAgenciaTipo.Id_tipo_agencia;

                        setValidationRules();
                        //PopularEstatusCombo();
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
                    HeaderAgregar = "Agregar Nueva Agencia";
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
                    Domicilio = string.Empty;
                    SelectMunicipio = null;
                    SelectedItem = null;
                    SelectedEstatus = null;
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    SelectAgenciaTipo = null;
                    SelectMunicipio = null;
                    setValidationRules();
                    //LimpiarTipoVisita();
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
                        //   this.Guardar();
                        GuardarAgencia();
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
                    this.GetAgencias();
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
                            if (this.Eliminar())
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

        private async void AgenciasLoad(CatalogoAgenciasView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new List<AGENCIA>();
                    ListEntidad = new ObservableCollection<ENTIDAD>();
                    CatalogoHeader = "Agencias";
                    HeaderAgregar = "Agregar Nueva Agencia";
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
                    this.GetAgencias();
                    GetTipoAgencias();
                    GetEntidades();
                    setValidationRules();
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar agencias.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_AGENCIAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

        private void GetAgencias()
        {
            try
            {
                //cAgencia agenciasControlador = new cAgencia();
                //ListItems = new ObservableCollection<AGENCIA>(agenciasControlador.ObtenerTodos());
                //if (ListItems.Count > 0)
                //    EmptyVisible = false;
                //else
                //    EmptyVisible = true;
                cAgencia agencias = new cAgencia();
                ListItems.Clear();
                ListItems = agencias.ObtenerTodos().ToList();
                if (ListItems.Count > 0)
                    EmptyVisible = false;
                else
                    EmptyVisible = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener agencias.", ex);
            }
        }

        private void GuardarAgencia()
        {
            try
            {
                var d = Clave;
                var des = Descripcion;
                var dom = Domicilio;
                var ident = Entidad.ID_ENTIDAD;
                var idmun = SelectMunicipio.ID_MUNICIPIO;
                var idagen = SelectTipoAgenciaIndex == 1 ? "E" : "F";
                var _ESTATUS = SelectedEstatus.CLAVE;
                cAgencia _agencia_controlador = new cAgencia();

                if (SelectedItem != null)
                {
                    if (!string.IsNullOrEmpty(Descripcion) && !string.IsNullOrEmpty(Domicilio) && Entidad.ID_ENTIDAD > 0 && SelectTipoAgenciaIndex > 0)
                    {
                        _agencia_controlador.Actualizar(new AGENCIA()
                        {
                            ID_AGENCIA = short.Parse(Clave.ToString()),
                            DESCR = Descripcion,
                            DOMICILIO = Domicilio,
                            ID_ENTIDAD = short.Parse(Entidad.ID_ENTIDAD.ToString()),
                            ID_MUNICIPIO = short.Parse(SelectMunicipio.ID_MUNICIPIO.ToString()),
                            TIPO_AGENCIA = SelectTipoAgenciaIndex == 1 ? "E" : "F",
                            ESTATUS = SelectedEstatus.CLAVE
                        });
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Descripcion) && !string.IsNullOrEmpty(Domicilio) && Entidad.ID_ENTIDAD > 0 && SelectTipoAgenciaIndex > 0)
                    {
                        _agencia_controlador.Insertar(new AGENCIA()
                        {
                            ID_AGENCIA = Clave,
                            DESCR = Descripcion,
                            DOMICILIO = Domicilio,
                            ID_ENTIDAD = Entidad.ID_ENTIDAD,
                            ID_MUNICIPIO = SelectMunicipio.ID_MUNICIPIO,
                            TIPO_AGENCIA = SelectTipoAgenciaIndex == 1 ? "E" : "F",
                            ESTATUS = SelectedEstatus.CLAVE
                        });
                    }
                }
                GetAgencias();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private void GetTipoAgencias()
        {
            try
            {
                var ListaTiposAgencia = new List<TipoAgenciaClass> { new TipoAgenciaClass() { Id_tipo_agencia = -1, Tipoagencia = "SELECCIONE" }, new TipoAgenciaClass() { Id_tipo_agencia = 1, Tipoagencia = "ESTATAL" }, new TipoAgenciaClass() { Id_tipo_agencia = 2, Tipoagencia = "FEDERAL" } };
                ListaAgenciasTipo = ListaTiposAgencia.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GetEntidades()
        {
            try
            {
                ListEntidad = new ObservableCollection<ENTIDAD>(objEstado.ObtenerTodos());
                ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        #region [CODIGO COMENTADO]
        //private void Guardar()
        //{
        //    if (Clave != 0)
        //    {  //Actualizar
        //        //obj.Actualizar(new TIPO_RELACION
        //        //{
        //        //    ID_RELACION = SelectedItem.ID_RELACION,
        //        //    DESCR = Descripcion,
        //        //    ESTATUS=SelectedEstatus.CLAVE
        //        //});
        //        var ObjPrueba = obj.Obtener(SelectedItem.ID_RELACION);
        //        SelectedItem.DESCR = descripcion;
        //        SelectedItem.ESTATUS = SelectedEstatus.CLAVE;
        //        obj.Actualizar(SelectedItem);
        //    }
        //    else
        //    {   //Agregar
        //        obj.Insertar(new TIPO_RELACION
        //        {
        //            ID_RELACION = 0,
        //            DESCR = Descripcion,
        //            ESTATUS=SelectedEstatus.CLAVE
        //        });
        //    }
        //    //Limpiamos las variables
        //    Clave = 0;
        //    Descripcion = string.Empty;
        //    Busqueda = string.Empty;
        //    //Mostrar Listado
        //    this.GetTipoRelacion();
        //}
        #endregion

        private bool Eliminar()
        {
            try
            {
                if (SelectedItem != null)
                {
                    // if (!agencia_controlador.Eliminar(SelectedItem.ID_AGENCIA))
                    return false;
                    Clave = 0;
                    Descripcion = string.Empty;
                    Busqueda = string.Empty;
                    this.GetAgencias();
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetAgencias();
            }
        }
    }
}
