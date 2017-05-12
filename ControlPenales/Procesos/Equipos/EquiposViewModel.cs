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
    partial class EquiposViewModel : ValidationViewModelBase
    {
        #region Constructor
        public EquiposViewModel(){}
        #endregion

        #region Metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "agregar_area":
                    if (lstAreas.Any(w=>w.ID_AREA==0))
                        if (SelectedAreaValue==0)
                        {
                            await new Dialogos().ConfirmacionDialogoReturn("Validación", "Seleccionar un area es obligatorio!");
                            return;
                        }
                    if (LstAreasAsignadas == null)
                        LstAreasAsignadas = new ObservableCollection<AREA>();
                    if (!LstAreasAsignadas.Any(w=>w.ID_AREA==SelectedAreaValue))
                        LstAreasAsignadas.Add(new AREA {
                            ID_AREA=SelectedAreaValue,
                            DESCR=LstAreas.First(w=>w.ID_AREA==SelectedAreaValue).DESCR
                        });
                    else
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "No se pueden repetir areas");
                        return;
                    }
                    break;
                case "desasignar_area":
                    if (SelectedAreaAsignada==null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Validación", "Debe seleccionar una area asignada");
                        return;
                    }
                    LstAreasAsignadas.Remove(LstAreasAsignadas.FirstOrDefault(w=>w.ID_AREA==SelectedAreaAsignada.ID_AREA));
                    LstAreasAsignadas = new ObservableCollection<AREA>(LstAreasAsignadas);
                    break;
                case "buscar":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    Buscar(); 
                    break;
                case "menu_editar":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    if (SelectedItem != null)
                    {
                        IsEquiposVisible = Visibility.Collapsed;
                        HeaderAgregar = "Editar Equipo";
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
                        setValidationRules();
                        await Obtener();
                        StaticSourcesViewModel.SourceChanged = false;
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
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    IsEquiposVisible = Visibility.Collapsed;
                    HeaderAgregar = "Agregar Equipo";
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
                    FocusText = Editar = true;
                    
                    /********************************/
                    setValidationRules();
                    Limpiar();
                    SelectedItem = null;
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!base.HasErrors)
                    {
                        /**********************************/
                        if (Guardar())
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
                            IsEquiposVisible = Visibility.Visible;
                            StaticSourcesViewModel.SourceChanged = false;
                        }
                        /**********************************/
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos");

                    break;
                case "menu_cancelar":
                    IsEquiposVisible = Visibility.Visible;
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
                    Limpiar();
                    await Obtener();
                    StaticSourcesViewModel.SourceChanged = false;
                    /****************************************/
                    break;
                case "menu_eliminar":
                    //var metro = Application.Current.Windows[0] as MetroWindow;
                    //if (SelectedItem != null)
                    //{
                    //    var mySettings = new MetroDialogSettings()
                    //    {
                    //        AffirmativeButtonText = "Aceptar",
                    //        NegativeButtonText = "Cancelar",
                    //        AnimateShow = true,
                    //        AnimateHide = false
                    //    };
                    //    var result = await metro.ShowMessageAsync("Eliminar", "¿Está seguro que desea eliminar... [ " + SelectedItem.DESCR + " ]?", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                    //    if (result == MessageDialogResult.Affirmative)
                    //    {
                    //        BaseMetroDialog dialog;
                    //        if (this.EliminarEtnia())
                    //        {
                    //            dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                    //        }
                    //        else
                    //        {
                    //            dialog = (BaseMetroDialog)metro.Resources["ErrorDialog"];
                    //        }
                    //        await metro.ShowMetroDialogAsync(dialog);
                    //        await TaskEx.Delay(1500);
                    //        await metro.HideMetroDialogAsync(dialog);
                    //    }
                    //}
                    //else
                    //    await metro.ShowMessageAsync("Validación", "Debe seleccionar una opción");
                    //SeleccionIndice = -1;
                    break;
                case "menu_exportar":
                    //SeleccionIndice = -1;
                    //Cambio = string.Empty;
                    break;
                case "menu_ayuda":
                    //SeleccionIndice = -1;
                    //Cambio = string.Empty;
                    break;
                case "menu_salir":
                    //SeleccionIndice = -1;
                    //Cambio = string.Empty;
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }
        
        private void ClickEnter(Object obj)
        {
            if (!pConsultar)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                Buscar();
            }
        }
        
        private async void WindowLoad(EquiposView obj)
        {
            try
            {
                StaticSourcesViewModel.CargarDatosMetodoAsync(CargarLista);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        private void CargarLista() 
        {
            try
            {
                ConfiguraPermisos();
                LstCentros = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos());
                LstAreas = new ObservableCollection<AREA>(new cArea().ObtenerTodos());
                LstTipoEquipo = new ObservableCollection<TipoEquipo>();
                ListItems = new ObservableCollection<CATALOGO_EQUIPOS>(new cCatalogoEquipos().ObtenerTodos(Busqueda,GlobalVar.gCentro));
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    SelectedAreaValue = 0;
                    LstTipoEquipo.Add(new TipoEquipo() { ID = (short)enumTipoEquipo.DESKTOP, DESCR = enumTipoEquipo.DESKTOP.ToString() });
                    LstTipoEquipo.Add(new TipoEquipo() { ID = (short)enumTipoEquipo.PORTATIL, DESCR = enumTipoEquipo.PORTATIL.ToString() });
                    LstTipoEquipo.Add(new TipoEquipo() { ID = (short)enumTipoEquipo.MOVIL, DESCR = enumTipoEquipo.MOVIL.ToString() });
                    LstTipoEquipo.Insert(0,new TipoEquipo() {ID = -1, DESCR = "SELECCIONE" });
                    StaticSourcesViewModel.SourceChanged = false;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listas.", ex);
            }
        }
        #endregion

        #region Equipos
        private void Limpiar() 
        {
            EIP = EMacAddress = EDescripcion= ESerieVolum = string.Empty;
            ECentro = GlobalVar.gCentro;
            ETipoEquipo = -1;
            EBiometria = false;
            EEstatus = "S";
            if (LstAreas.Any(w => w.ID_AREA == 0))
                SelectedAreaValue = 0;
            LstAreasAsignadas = new ObservableCollection<AREA>();
        }

        private bool Guardar()
        {
            try
            {
                var obj = new CATALOGO_EQUIPOS();
                obj.IP = EIP;
                obj.MAC_ADDRESS = EMacAddress;
                obj.SERIE_VOLUM = ESerieVolum;
                obj.DESCRIPCION = EDescripcion;
                obj.ID_CENTRO = eCentro;
                obj.USUARIO_CREADOR = GlobalVar.gUsr;
                obj.USUARIO_BORRADOR = GlobalVar.gUsr;
                obj.ACTIVO = EEstatus;
                //obj.FECHA_BAJA = Fechas.GetFechaDateServer;
                obj.TIPO_EQUIPO = ETipoEquipo;
                obj.BIOMETRIA = EBiometria ? "S" : "N";
                var equipo_area = new List<EQUIPO_AREA>();
                foreach (var item in lstAreasAsignadas)
                    equipo_area.Add(new EQUIPO_AREA {
                        ID_AREA=item.ID_AREA,
                        IP=obj.IP,
                        MAC_ADDRESS=obj.MAC_ADDRESS,
                        REGISTRO_FEC=_FechaServer
                    });
                if (SelectedItem != null)
                {
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        return false;
                    }
                    obj.FECHA_MODIFICACION = _FechaServer;
                    if (SelectedItem.ACTIVO == "S" && eEstatus == "N")
                        obj.FECHA_BAJA = Fechas.GetFechaDateServer;
                    if (new cCatalogoEquipos().Actualizar(obj, equipo_area))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se actualizo correctamente");
                        return true;
                    }
                }
                else
                {
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        return false;
                    }
                    var equipo = new cCatalogoEquipos().Obtener(EIP, EMacAddress);
                    if (equipo == null)
                    {
                        obj.EQUIPO_AREA = equipo_area;
                        obj.FECHA_ALTA = _FechaServer;//Fechas.GetFechaDateServer;
                        if (new cCatalogoEquipos().Insertar(obj))
                        {
                            new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                            return true;
                        }
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "La IP y MAC ADDRESS ya existen");
                        return false;
                    }
                }
                new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un error al guardar la información");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar equipo.", ex);
            }
            return false;
        }
        
        private bool Eliminar()
        {
            //if (SelectedItem != null || SelectedItem.ID_ETNIA >= 100)
            //{
            //    cEtnia etnia = new cEtnia();
            //    if (!etnia.Eliminar(SelectedItem.ID_ETNIA))
            //        return false;
            //    Clave = 0;
            //    Descripcion = string.Empty;
            //    SelectedEstatus = null;
            //    Busqueda = string.Empty;
            //    this.GetEtnias();
            //}
            return true;
        }

        private async Task Obtener() {
            try
            {
                if (SelectedItem != null)
                {
                    EIP = SelectedItem.IP;
                    EMacAddress = SelectedItem.MAC_ADDRESS;
                    EDescripcion = SelectedItem.DESCRIPCION;
                    ECentro = SelectedItem.ID_CENTRO;
                    ETipoEquipo = SelectedItem.TIPO_EQUIPO.Value;
                    EBiometria = SelectedItem.BIOMETRIA == "S" ? true : false;
                    EEstatus = SelectedItem.ACTIVO;
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                        lstAreasAsignadas = new ObservableCollection<AREA>(new cEquipo_Area().Seleccionar(selectedItem.IP, selectedItem.MAC_ADDRESS).Select(s => s.AREA));
                        OnPropertyChanged("LstAreasAsignadas");
                    });
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener datos del equipo.", ex);
            }
        }
        #endregion

        #region Buscar
        private async void Buscar() {
            try
            {
                ListItems = new ObservableCollection<CATALOGO_EQUIPOS>(new cCatalogoEquipos().ObtenerTodos(Busqueda, GlobalVar.gCentro));
                EmptyVisible = ListItems.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar busqueda.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_EQUIPOS_AUTORIZADOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        pInsertar = true;
                    if (p.EDITAR == 1)
                        pEditar = true;
                    if (p.CONSULTAR == 1)
                        pConsultar = true;
                    if (p.IMPRIMIR == 1)
                        pImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
    
    public class TipoEquipo
    {
        public short ID{set; get;} 
        public string DESCR{set; get;} 
    }
}