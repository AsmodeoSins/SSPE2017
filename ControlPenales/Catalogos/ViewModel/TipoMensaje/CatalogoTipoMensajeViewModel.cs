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
    partial class CatalogoTipoMensajeViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public CatalogoTipoMensajeViewModel() { }

        void IPageViewModel.inicializa() { }

        private async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    PopulateListado();
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
                        PopulateDetalle();
                        #endregion Obtener Valores
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
                    /********************************/
                    Limpiar();
                    SelectedEstatus = ListEstatus.LISTA_ESTATUS.Where(w => w.CLAVE == "S").FirstOrDefault();
                    Rol = -1;
                    LstMensajeRol = new ObservableCollection<MENSAJE_ROL>();
                    SelectedMensajeRol = null;
                    setValidationRules();
                    /********************************/
                    break;
                case "menu_guardar":
                    if (!base.HasErrors)
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
                        if (Guardar())
                        {
                            PopulateListado();
                        }
                        else
                            new Dialogos().NotificacionDialog("Error", "Al guardar información");
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
                    Limpiar();
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
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.DESCR.Trim() + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            if (Eliminar())
                                PopulateListado();
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

                case "seleccionar_color":
                    Color = ColorPopUp;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONA_COLOR);
                    break;
                case "cancelar_color":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONA_COLOR);
                    break;
                case "addRol":
                    AgregarRol();
                    break;
                case "delRol":
                    EliminarMensajeRol();
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                PopulateListado();
            }
        }

        #region Roles
        private void AgregarRol()
        {
            try
            {
                if (Rol != -1)
                {
                    if (LstMensajeRol == null)
                        LstMensajeRol = new ObservableCollection<MENSAJE_ROL>();
                    if (LstMensajeRol.Count(w => w.ID_ROL == Rol) == 0)
                    {
                        LstMensajeRol.Add(new MENSAJE_ROL() { 
                            ID_ROL = Rol.Value,
                            SISTEMA_ROL = SelectedRol
                        });
                        Rol = -1;
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación","El rol seleccionado ya se encuentra en la lista");
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un rol");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar rol.", ex);
            }
        }

        private void EliminarMensajeRol() {
            try
            {
                if (SelectedMensajeRol != null)
                {
                    LstMensajeRol.Remove(SelectedMensajeRol);
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar el rol a eliminar");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar rol.", ex);
            }
        }
        #endregion
    }
}