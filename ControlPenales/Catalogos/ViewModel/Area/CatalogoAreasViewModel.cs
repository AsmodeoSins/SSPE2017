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
    partial class CatalogoAreasViewModel : ValidationViewModelBase, IPageViewModel, IDataErrorInfo
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        public CatalogoAreasViewModel()
        {
            CatalogoHeader = "Areas";
            HeaderAgregar = "Agregar Nueva Area";
            FiltroDisplay = "DESCR";
            //LLENAR 
            EditarVisible = false;
            NuevoVisible = false;
            FiltroVisible = true;
            //ListItems = new ObservableCollection<AREA>();
            cCentro centro = new cCentro();
            ListTipos = new ObservableCollection<CENTRO>(centro.ObtenerTodos("", 2, 0));
            ListTipos.Insert(0, new CENTRO() { ID_CENTRO = 0, DESCR = "SELECCIONE" });
            SelectedTipo = ListTipos.Where(w => w.DESCR == "SELECCIONE").FirstOrDefault();
            AgregarVisible = false;

            GuardarMenuEnabled = false;
            AgregarMenuEnabled = true;
            EliminarMenuEnabled = false;
            EditarMenuEnabled = false;
            CancelarMenuEnabled = false;
            AyudaMenuEnabled = true;
            SalirMenuEnabled = true;
            ExportarMenuEnabled = true;

            SeleccionIndice = -1;
            this.setValidationRules();
        }

        void IPageViewModel.inicializa()
        {
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar":
                    this.GetAreas();
                    break;
                case "menu_editar":
                    if (SelectedItem != null)
                    {
                        HeaderAgregar = "Editar Area";
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
                        Clave = SelectedItem.ID_AREA.ToString();
                        Descripcion = SelectedItem.DESCR;
                        Tipo = SelectedItem.ID_CENTRO;
                        SelectedTipo = ListTipos.Where(w => w.ID_CENTRO == SelectedItem.ID_CENTRO).FirstOrDefault();
                        /*****************************************/
                    }
                    else
                    {
                        bandera_editar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una opcion.");
                    }
                    break;
                case "menu_agregar":
                    HeaderAgregar = "Agregar Nueva Area";
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
                    Clave = string.Empty;
                    Descripcion = string.Empty;
                    Tipo = 0;
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
                        GuardarArea();
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
                    Clave = "";
                    Descripcion = string.Empty;
                    Tipo = 0;
                    this.GetAreas();
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
                            if (EliminarArea())
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
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opcion");
                    SeleccionIndice = -1;
                    break;
                case "menu_exportar":
                    SeleccionIndice = -1;
                    break;
                case "menu_ayuda":
                    SeleccionIndice = -1;
                    break;
                case "menu_salir":
                    SeleccionIndice = -1;
                    OnParameterChange("salir");
                    break;
            }
        }

        private void ClickEnter(Object obj)
        {
            if (obj != null)
            {
                Busqueda = ((System.Windows.Controls.TextBox)(obj)).Text;
                GetAreas();
            }
        }

        private void GetAreas()
        {
            cAreas area = new cAreas();
            ListItems.Clear();
            ListItems = new ObservableCollection<AREA>(area.ObtenerTodos(Busqueda, SelectedTipo.ID_CENTRO));
            if (ListItems.Count > 0)
                EmptyVisible = false;
            else
                EmptyVisible = true;
        }

        private void GuardarArea()
        {
            cAreas area = new cAreas();
            if (!string.IsNullOrEmpty(Clave))
            {  //Actualizar
                area.Actualizar(new AREA
                {
                    ID_AREA = SelectedItem.ID_AREA,
                    DESCR = Descripcion,
                    ID_CENTRO = SelectedTipo.ID_CENTRO
                });
            }
            else
            {   //Agregar
                area.Insertar(new AREA
                {
                    ID_AREA = 0,
                    DESCR = Descripcion,
                    ID_CENTRO = SelectedTipo.ID_CENTRO
                });
            }
            //Limpiamos las variables
            Clave = string.Empty;
            Descripcion = string.Empty;
            Tipo = 0;
            //Mostrar Listado
            this.GetAreas();
        }

        private bool EliminarArea()
        {
            if (SelectedItem != null)
            {
                cAreas area = new cAreas();
                if (!area.Eliminar(SelectedItem.ID_AREA))
                    return false;
                Clave = string.Empty;
                Descripcion = string.Empty;
                Tipo = 0;
                this.GetAreas();
            }
            return true;
        }
    }
}