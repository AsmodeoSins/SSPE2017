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
    partial class CatalogoSectorClasificacionViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public CatalogoSectorClasificacionViewModel() { }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    PopulateSectorClasificacion();
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
                        PopulateSectorClasificacion();
                        #endregion Obtener Valores
                    }
                    else
                    {
                        GrupoVulnerable = SelectedItem.ES_GRUPO_VULNERABLE.Equals(1) ? true : false;
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
                    /********************************/
                    LimpiarSectorClasificacion();
                    SelectedEstatus = Lista_Estatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    GrupoVulnerable = false;
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!string.IsNullOrEmpty(Descripcion))
                    {
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
                        if (GuardarSectorClasificacion())
                        {
                            PopulateSectorClasificacionListado();
                        }
                        else
                            new Dialogos().NotificacionDialog("Error", "Error al guardar información");
                        /**********************************/
                    }
                    else
                        FocusText = true;
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
                    LimpiarSectorClasificacion();
                    SelectedEstatus = null;
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
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.POBLACION.Trim() + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            EliminarSectorClasificacion();
                            var dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
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
                case "cambiar_color":
                    TipoColor = 1;//BACKGROUND
                    ColorPopUp = Color;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONA_COLOR);
                    break;
                case "cambiar_color_texto":
                    TipoColor = 2;//TEXTO
                    ColorPopUp = ColorFont;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONA_COLOR);
                    break;
                case "seleccionar_color":
                    if (TipoColor == 1)
                        Color = ColorPopUp;
                    else
                        ColorFont = ColorPopUp;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONA_COLOR);
                    break;
                case "cancelar_color":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONA_COLOR);
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                PopulateSectorClasificacionListado();
            }
        }
    }
}