using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace ControlPenales
{
    class MenuRecetasViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public MenuRecetasViewModel()
        {
            IngredientesVisible = false;
            NuevaRecetaVisible = false;
            BotonesVisible = true;
            CatalogoHeader = "Menu de Recetas";
            HeaderAgregar = "Agregar Nuevo Menu";
            NewItem = new Usuario();
            UpdateItem = new Usuario();
            //LLENAR 
            ListItems = new ObservableCollection<Usuario>();
            ListItems2 = new ObservableCollection<Usuario>();
            ListItems.Add(new Usuario() { Username = "PRUEBAS", Password = "12345" });
            ListItems.Add(new Usuario() { Username = "TEST", Password = "98765" });
            
            SeleccionIndice = -1;

            GuardarMenuEnabled = false;
            AgregarMenuEnabled = true;
            EliminarMenuEnabled = false;
            EditarMenuEnabled = false;
            CancelarMenuEnabled = false;
            AyudaMenuEnabled = true;
            SalirMenuEnabled = true;
            ExportarMenuEnabled = true;
        }
        #endregion

        #region variables
        private bool nuevaRecetaVisible;
        public bool NuevaRecetaVisible
        {
            get { return nuevaRecetaVisible; }
            set { nuevaRecetaVisible = value; OnPropertyChanged("NuevaRecetaVisible"); }
        }
        private bool ingredientesVisible;
        public bool IngredientesVisible
        {
            get { return ingredientesVisible; }
            set { ingredientesVisible = value; OnPropertyChanged("IngredientesVisible"); }
        }
        private bool botonesVisible;
        public bool BotonesVisible
        {
            get { return botonesVisible; }
            set { botonesVisible = value; OnPropertyChanged("BotonesVisible"); }
        }

        #region BotonesMenuEnabled
        private bool guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return guardarMenuEnabled; }
            set { guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }
        private bool agregarMenuEnabled;
        public bool AgregarMenuEnabled
        {
            get { return agregarMenuEnabled; }
            set { agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }
        private bool editarMenuEnabled;
        public bool EditarMenuEnabled
        {
            get { return editarMenuEnabled; }
            set { editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }
        private bool eliminarMenuEnabled;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }
        private bool cancelarMenuEnabled;
        public bool CancelarMenuEnabled
        {
            get { return cancelarMenuEnabled; }
            set { cancelarMenuEnabled = value; OnPropertyChanged("CancelarMenuEnabled"); }
        }
        private bool exportarMenuEnabled;
        public bool ExportarMenuEnabled
        {
            get { return exportarMenuEnabled; }
            set { exportarMenuEnabled = value; OnPropertyChanged("ExportarMenuEnabled"); }
        }
        private bool salirMenuEnabled;
        public bool SalirMenuEnabled
        {
            get { return salirMenuEnabled; }
            set { salirMenuEnabled = value; OnPropertyChanged("SalirMenuEnabled"); }
        }
        private bool ayudaMenuEnabled;
        public bool AyudaMenuEnabled
        {
            get { return ayudaMenuEnabled; }
            set { ayudaMenuEnabled = value; OnPropertyChanged("AyudaMenuEnabled"); }
        }
        #endregion

        #region otros
        public bool bandera_editar = false;
        private bool focusText;
        public bool FocusText
        {
            get { return focusText; }
            set { focusText = value; OnPropertyChanged("FocusText"); }
        }
        private string cambio;
        public string Cambio
        {
            get { return cambio; }
            set { cambio = value; OnPropertyChanged("Cambio"); }
        }
        private string catalogHeader;
        public string CatalogoHeader
        {
            get { return catalogHeader; }
            set { catalogHeader = value; }
        }
        private string headerAgregar;
        public string HeaderAgregar
        {
            get { return headerAgregar; }
            set { headerAgregar = value; }
        }
        private int seleccionIndice;
        public int SeleccionIndice
        {
            get { return seleccionIndice; }
            set { seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }
        #endregion

        #region Items
        private Usuario updateItem;
        public Usuario UpdateItem
        {
            get { return updateItem; }
            set { updateItem = value; OnPropertyChanged("UpdateItem"); }
        }
        private Usuario newItem;
        public Usuario NewItem
        {
            get { return newItem; }
            set { newItem = value; OnPropertyChanged("NewItem"); }
        }
        private ObservableCollection<Usuario> listItems;
        public ObservableCollection<Usuario> ListItems
        {
            get { return listItems; }
            set { listItems = value; OnPropertyChanged("ListItems"); }
        }
        private ObservableCollection<Usuario> listItems2;
        public ObservableCollection<Usuario> ListItems2
        {
            get { return listItems2; }
            set { listItems2 = value; OnPropertyChanged("ListItems2"); }
        }
        #endregion
        public string Name
        {
            get
            {
                return "catalogo_menu_recetas";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private async void clickSwitch(Object obj)
        {
            var metro = Application.Current.Windows[0] as MetroWindow;
            switch (obj.ToString())
            {
                case "menu_editar":
                    if (_selectedItem != null)
                    {
                        IngredientesVisible = true;
                        NuevaRecetaVisible = true;
                        BotonesVisible = false;

                        bandera_editar = true;
                        FocusText = true;

                        Cambio = _selectedItem.Username;

                        GuardarMenuEnabled = true;
                        AgregarMenuEnabled = false;
                        EliminarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        CancelarMenuEnabled = true;
                        AyudaMenuEnabled = true;
                        SalirMenuEnabled = true;
                        ExportarMenuEnabled = true;
                        ListItems2.Clear();
                        if (_selectedItem.Password == "12345")
                        {
                            ListItems2.Add(new Usuario() { Username = "PRUEBAS1", Password = "13579" });
                            ListItems2.Add(new Usuario() { Username = "TEST1", Password = "24680" });
                        }
                        else
                        {
                            ListItems2.Add(new Usuario() { Username = "PRUEBAS2", Password = "21399" });
                            ListItems2.Add(new Usuario() { Username = "TEST2", Password = "28253" });
                        }
                    }
                    else
                    {
                        bandera_editar = false;
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opcion.");
                    }
                    break;
                case "menu_agregar":
                    IngredientesVisible = false;
                    NuevaRecetaVisible = true;
                    BotonesVisible = true;

                    bandera_editar = false;
                    FocusText = true;

                    Cambio = string.Empty;

                    GuardarMenuEnabled = true;
                    AgregarMenuEnabled = false;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = true;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    break;
                case "boton_aceptar_receta":
                    if (_selectedReceta != null)
                    {
                        IngredientesVisible = false;
                        NuevaRecetaVisible = false;

                        bandera_editar = false;
                        FocusText = false;

                        GuardarMenuEnabled = false;
                        AgregarMenuEnabled = true;
                        EliminarMenuEnabled = false;
                        EditarMenuEnabled = false;
                        CancelarMenuEnabled = false;
                        AyudaMenuEnabled = true;
                        SalirMenuEnabled = true;
                        ExportarMenuEnabled = true;
                    }
                    else
                    {
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opcion.");
                    }
                    break;
                case "boton_cancelar_receta":
                    IngredientesVisible = false;
                    NuevaRecetaVisible = false;

                    bandera_editar = false;
                    FocusText = false;

                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    break;
                case "menu_guardar":
                    if (bandera_editar == false)
                    {
                        _selectedItem = null;
                        _selectedItem = new Usuario() { Username = Cambio, Password = "12345" };
                        ListItems.Add(_selectedItem);
                        IngredientesVisible = true;
                        NuevaRecetaVisible = false;
                        AgregarMenuEnabled = false;
                        ListItems2.Clear();
                        if (_selectedItem.Password == "12345")
                        {
                            ListItems2.Add(new Usuario() { Username = "PRUEBAS1", Password = "13579" });
                            ListItems2.Add(new Usuario() { Username = "TEST1", Password = "24680" });
                        }
                        else
                        {
                            ListItems2.Add(new Usuario() { Username = "PRUEBAS2", Password = "21399" });
                            ListItems2.Add(new Usuario() { Username = "TEST2", Password = "28253" });
                        }
                    }
                    else
                    {
                        SelectedItem.Username = Cambio;
                        IngredientesVisible = false;
                        NuevaRecetaVisible = false;
                    }

                    bandera_editar = false;
                    FocusText = false;

                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Cambio = string.Empty;

                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    break;
                case "menu_cancelar":

                    IngredientesVisible = false;
                    NuevaRecetaVisible = false;

                    bandera_editar = false;
                    FocusText = false;

                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Cambio = string.Empty;

                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    break;
                case "menu_eliminar":
                    if (_selectedItem != null)
                    {
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Aceptar",
                            NegativeButtonText = "Cancelar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        //  SALTO DE LINEA
                        // string.Format("\n")
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.Username + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            var i = ListItems.IndexOf(_selectedItem);
                            if (i >= 0)
                            {
                                ListItems.RemoveAt(i);

                                var dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                                await metro.ShowMetroDialogAsync(dialog);
                                await TaskEx.Delay(1500);
                                await metro.HideMetroDialogAsync(dialog);

                                //MENSAJE EXTERNO
                                //dialog = dialog.ShowDialogExternally();
                                //await TaskEx.Delay(1500);
                                //await dialog.RequestCloseAsync();
                            }
                        }
                    }
                    else
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opcion");
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Cambio = string.Empty;

                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;
                    break;
                case "menu_exportar":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    break;
                case "menu_ayuda":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    break;
                case "menu_salir":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Cambio = string.Empty;
                    break;
            }
        }
        #endregion

        #region command
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        Usuario _selectedItem = null;
        public Usuario SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                if (_selectedItem == null)
                {
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                }
                else
                {
                    EliminarMenuEnabled = true;
                    EditarMenuEnabled = true;
                }
            }
        }
        Usuario _selectedReceta = null;
        public Usuario SelectedReceta
        {
            get
            {
                return _selectedReceta;
            }
            set
            {
                _selectedReceta = value;
            }
        }
        #endregion

    }
}
