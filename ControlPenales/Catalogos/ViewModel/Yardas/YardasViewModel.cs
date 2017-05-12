using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    partial class YardasViewModel : ValidationViewModelBase
    {
        #region Constructor
        public YardasViewModel() { }
        #endregion

        #region Metodos
        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    Buscar();
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
                        #region Obtener Valores
                        YInsert = false;
                        HeaderAgregar = "Editar Horario de Yarda";
                        GetYarda();
                        setValidationRules();
                        StaticSourcesViewModel.SourceChanged = false;
                        #endregion
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
                    HeaderAgregar = "Agregar Nuevo Horario de Yarda";
                    YInsert = true;
                    Limpiar();
                    setValidationRules();
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "menu_guardar":
                    if (base.HasErrors)
                    {
                        new Dialogos().ConfirmacionDialogo("Vaidación", "Favor de llenar los campos obligatorios. " + base.Error);
                        break;
                    }
                    if (!ValidaHora())
                        break;
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
                    if (Guardar())
                    {
                        Buscar();
                        base.ClearRules();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    /**********************************/
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
                    /****************************************/
                    Limpiar();
                    base.ClearRules();
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
                    //    //var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.POBLACION.Trim() + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                    //    //if (result == MessageDialogResult.Affirmative)
                    //    //{
                    //    //    EliminarSectorClasificacion();
                    //    //    var dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                    //    //    await metro.ShowMetroDialogAsync(dialog);
                    //    //    await TaskEx.Delay(1500);
                    //    //    await metro.HideMetroDialogAsync(dialog);
                    //    //}
                    //}
                    //else
                    //    await metro.ShowMessageAsync("Validación", "Debe seleccionar una opción");
                    //SeleccionIndice = -1;
                    break;
                case "menu_exportar":
                    break;
                case "menu_ayuda":
                    break;
                case "menu_salir":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                Buscar();
            }
        }

        private async void PageLoad(YardasView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ListItems = new ObservableCollection<YARDA>(new cYarda().ObtenerTodos(GlobalVar.gCentro));
                    LstEdificio = new ObservableCollection<EDIFICIO>(new cEdificio().ObtenerTodos(string.Empty, 0, GlobalVar.gCentro, "S"));
                    LstSector = new ObservableCollection<SECTOR>();
                    LstArea = new ObservableCollection<AREA>(new cArea().ObtenerTodos());
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstEdificio.Insert(0, new EDIFICIO() { ID_EDIFICIO = -1, DESCR = "SELECCIONE" });
                        LstSector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
                        LstArea.Insert(0, new AREA() { ID_AREA = -1, DESCR = "SELECCIONE" });
                        YSector = -1;
                        EmptyVisible = ListItems.Count > 0 ? false : true;
                        StaticSourcesViewModel.SourceChanged = false;
                    }));

                    //CatalogoHeader = "A";
                    HeaderAgregar = "Agregar Nuevo Horario de Yarda";
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
                    ConfiguraPermisos();
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información.", ex);
            }
        }
        #endregion

        #region Horario Yarda
        private void GetYarda()
        {
            try
            {
                if (SelectedItem != null)
                {
                    YEdificio = SelectedItem.ID_EDIFICIO;
                    YSector = SelectedItem.ID_SECTOR;
                    YArea = SelectedItem.ID_AREA != null ? SelectedItem.ID_AREA.Value : (short)-1;
                    YCeldaInicio = SelectedItem.CELDA_INICIO;
                    YCeldaFin = SelectedItem.CELDA_FINAL;
                    YDiaSemana = SelectedItem.SEMANA_DIA.Value;
                    YHoraInicio = SelectedItem.HORA_INICIO;
                    YMinInicio = SelectedItem.MINUTO_INICIO;
                    YHoraFin = SelectedItem.HORA_FIN;
                    YMinFin = SelectedItem.MINUTO_FIN;
                    YEstatus = SelectedItem.ESTATUS;
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una yarda.");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar información.", ex);
            }
        }

        private bool Guardar()
        {
            try
            {
                var obj = new YARDA();
                obj.ID_CENTRO = SelectedSector.ID_CENTRO;
                obj.ID_EDIFICIO = SelectedSector.ID_EDIFICIO;
                obj.ID_SECTOR = SelectedSector.ID_SECTOR;
                obj.CELDA_INICIO = YCeldaInicio.Value;
                obj.CELDA_FINAL = YCeldaFin;
                obj.SEMANA_DIA = YDiaSemana;
                obj.HORA_INICIO = YHoraInicio;
                obj.MINUTO_INICIO = YMinInicio;
                obj.HORA_FIN = YHoraFin;
                obj.MINUTO_FIN = YMinFin;
                obj.ESTATUS = YEstatus;
                obj.ID_AREA = YArea;
                if (SelectedItem != null)//update
                {
                    obj.ID_YARDA = SelectedItem.ID_YARDA;
                    if (new cYarda().Actualizar(obj))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente.");
                        return true;
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "Ocurrió un error al guardar la información.");
                }
                else//insert
                {
                    obj.ID_YARDA = new cYarda().Insertar(obj);
                    if (obj.ID_YARDA > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente.");
                        return true;
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "Ocurrió un error al guardar la información.");
                }
                return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar horario de la yarda.", ex);
                return false;
            }
        }

        private void CambiarEstatus()
        {
            try
            {
                if (SelectedItem != null)
                {
                    var obj = new YARDA();
                    obj.ID_YARDA = SelectedItem.ID_YARDA;
                    obj.ID_CENTRO = SelectedItem.ID_CENTRO;
                    obj.ID_EDIFICIO = SelectedItem.ID_EDIFICIO;
                    obj.ID_SECTOR = SelectedItem.ID_SECTOR;
                    obj.ESTATUS = "N";
                    if (new cYarda().ActualizarEstatus(obj))
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente.");
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "Ocurrió un error al guardar la información.");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambiar estatus del horario de la yarda.", ex);
            }
        }

        private void Buscar()
        {
            try
            {
                ListItems = new ObservableCollection<YARDA>(new cYarda().ObtenerTodos(GlobalVar.gCentro));
                EmptyVisible = ListItems.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar horarios de la yarda.", ex);
            }
        }

        private void Limpiar()
        {
            try
            {
                YEdificio = YSector = YDiaSemana = YArea = -1;
                YCeldaInicio = YCeldaFin = YHoraInicio = YMinInicio = YHoraFin = YMinFin = null;
                YEstatus = "S";
                SelectedItem = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar pantalla.", ex);
            }
        }
        #endregion

        #region [PERMISOS]
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_HORARIO_YARDAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        AgregarMenuEnabled = true;
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