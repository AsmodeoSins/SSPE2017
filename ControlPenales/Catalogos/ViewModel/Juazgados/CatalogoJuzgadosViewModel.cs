using ControlPenales;
using ControlPenales.Clases.Estatus;
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
    partial class CatalogoJuzgadosViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        cJuzgado juzgadoControlador = new cJuzgado();
        cEntidad EntidadesControlador = new cEntidad();
        cMunicipio municipioControlador = new cMunicipio();

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetTipoRelacion();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Juzgados";
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
                        Clave = SelectedItem.ID_JUZGADO;
                        Descripcion = SelectedItem.DESCR == null ? SelectedItem.DESCR : SelectedItem.DESCR.TrimEnd();
                        Domicilio = SelectedItem.DOMICILIO == null ? SelectedItem.DOMICILIO : SelectedItem.DOMICILIO.TrimEnd();
                        Pais = ListPais.Where(w => w.ID_PAIS_NAC == SelectedItem.ID_PAIS).FirstOrDefault();
                        Entidad = ListEntidad.Where(w => w.ID_ENTIDAD == SelectedItem.ID_ENTIDAD).ToList().FirstOrDefault() ?? new ENTIDAD();
                        SelectMunicipio = ListMunicipio.Where(w => w.ID_MUNICIPIO == SelectedItem.ID_MUNICIPIO).ToList().FirstOrDefault() ?? new MUNICIPIO();
                        SelectedEstatus = SelectedItem.ESTATUS == null ? Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "N").SingleOrDefault() : Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == SelectedItem.ESTATUS).SingleOrDefault();
                        SelectedFuero = ListaFuero.Where(w => w.ID_FUERO == SelectedItem.ID_FUERO).FirstOrDefault();
                        SelectJuzgadoTipo = ListaJuzgadoTipo.Where(w => w.ID_TIPO_JUZGADO == SelectedItem.ID_TIPO_JUZGADO).FirstOrDefault();
                        Tribunal = SelectedItem.TRIBUNAL == "S" ? true : false;
                        //  setValidationRules();
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
                    HeaderAgregar = "Agregar Nuevo Juzgado";
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
                    SelectMunicipio = null;
                    SelectedFuero = null;
                    SelectJuzgadoTipo = null;
                    Tribunal = false;
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
                    this.GetTipoRelacion();
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

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetJuzgados();
            }
        }
        private async void CatalogojuzgadoLoad(CatalogoJuzgadosView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    EmptyVisible = false;
                    //Listado 
                    ListItems = new ObservableCollection<JUZGADO>();
                    ListEntidad = new ObservableCollection<ENTIDAD>();
                    CatalogoHeader = "Juzgados";
                    HeaderAgregar = "Agregar Juzgados";
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
                    this.GetJuzgados();
                    this.GetTipoJuzgados();
                    this.Getpaises();
                    this.GetFueros();
                    //GetTipoAgencias();
                    GetEntidades();
                    Lista_Estatus.LISTA_ESTATUS.Insert(0, new Estatus() { CLAVE = "V", DESCRIPCION = "SELECCIONE" });
                    setValidationRules();
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar Juzgados.", ex);
            }
        }

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_JUZGADOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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

        private void GetJuzgados()
        {
            try
            {
                ListItems = new ObservableCollection<JUZGADO>(new cJuzgado().Obtener().ToList());
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

        private void Getpaises()
        {
            try
            {
                cPaises paises = new cPaises();
                ListPais = new ObservableCollection<PAIS_NACIONALIDAD>(paises.ObtenerTodos().ToList());
                ListPais.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GetFueros()
        {
            try
            {
                cFuero fueroControlador = new cFuero();
                ListaFuero = new ObservableCollection<FUERO>(fueroControlador.ObtenerTodos().ToList());
                ListaFuero.Insert(0, new FUERO() { ID_FUERO = "-1", DESCR = "SELECCIONE" });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GetTipoJuzgados()
        {
            try
            {
                ListaJuzgadoTipo = new ObservableCollection<TIPO_JUZGADO>(new cJuzgadoTipo().Obtener().ToList());
                ListaJuzgadoTipo.Insert(0, new TIPO_JUZGADO() { ID_TIPO_JUZGADO = -1, DESCR = "SELECCIONE" });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GuardarAgencia()
        {
            try
            {
                cJuzgado _juzgadoControlador = new cJuzgado();
                if (SelectedItem != null)
                {
                    //Validacion
                    if (!string.IsNullOrEmpty(Descripcion) && !string.IsNullOrEmpty(Domicilio) && SelectJuzgadoTipo.ID_TIPO_JUZGADO > 0 && SelectMunicipio.ID_MUNICIPIO > 0 && Entidad.ID_ENTIDAD > 0)
                    {
                        var actualizo = _juzgadoControlador.Actualizar(new JUZGADO()
                        {
                            ID_JUZGADO = short.Parse(Clave.ToString()),
                            ID_TIPO_JUZGADO = short.Parse(SelectJuzgadoTipo.ID_TIPO_JUZGADO.ToString()),
                            ID_FUERO = SelectedFuero.ID_FUERO,
                            ID_PAIS = Pais.ID_PAIS_NAC,
                            DESCR = Descripcion,
                            DOMICILIO = Domicilio,
                            ID_ENTIDAD = short.Parse(Entidad.ID_ENTIDAD.ToString()),
                            ID_MUNICIPIO = short.Parse(SelectMunicipio.ID_MUNICIPIO.ToString()),
                            TRIBUNAL = Tribunal == true ? "S" : "N",
                            ESTATUS = SelectedEstatus.CLAVE
                        });
                    }
                }
                else
                {
                    //Validacion
                    if (!string.IsNullOrEmpty(Descripcion) && !string.IsNullOrEmpty(Domicilio) && SelectJuzgadoTipo.ID_TIPO_JUZGADO > 0 && SelectMunicipio.ID_MUNICIPIO > 0 && Entidad.ID_ENTIDAD > 0)
                    {
                        var inserto = _juzgadoControlador.Insertar(new JUZGADO()
                        {
                            ID_JUZGADO = short.Parse(Clave.ToString()),
                            ID_TIPO_JUZGADO = short.Parse(SelectJuzgadoTipo.ID_TIPO_JUZGADO.ToString()),
                            ID_FUERO = SelectedFuero.ID_FUERO,
                            ID_PAIS = Pais.ID_PAIS_NAC,
                            DESCR = Descripcion,
                            DOMICILIO = Descripcion,
                            ID_ENTIDAD = short.Parse(Entidad.ID_ENTIDAD.ToString()),
                            ID_MUNICIPIO = short.Parse(SelectMunicipio.ID_MUNICIPIO.ToString()),
                            TRIBUNAL = Tribunal == true ? "S" : "N",
                            ESTATUS = SelectedEstatus.CLAVE
                        });
                    }
                }
                GetJuzgados();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
            }
        }

        private void GetEntidades()
        {
            try
            {
                ListEntidad = new ObservableCollection<ENTIDAD>(EntidadesControlador.ObtenerTodos());
                ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos.", ex);
            }
        }

        private void GetTipoRelacion()
        {
            try
            {
                ListItems.Clear();
                ListItems = new ObservableCollection<JUZGADO>(juzgadoControlador.ObtenerTodos().ToList());
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
                    this.GetTipoRelacion();
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar.", ex);
                return false;
            }
        }
    }
}
