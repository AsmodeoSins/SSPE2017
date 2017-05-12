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
    class RecetasAlimentacionInternosViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public RecetasAlimentacionInternosViewModel()
        {
            IngredientesVisible = false;
            NuevaRecetaVisible = false;
            BotonesVisible = true;
            CatalogoHeader = "Recetas de Alimentacion de Internos";
            HeaderAgregar = "Agregar Nueva Receta";
            NewItem = new Usuario();
            UpdateItem = new Usuario();
            //LLENAR 
            ListItems = new ObservableCollection<Usuario>();
            ListItems.Add(new Usuario() { Username = "PRUEBAS", Password = "12345" });
            ListItems.Add(new Usuario() { Username = "TEST", Password = "98765" });
            ListItems2 = new ObservableCollection<DetalleTraspaso>();
            ListItems2.Add(new DetalleTraspaso() { Cantidad = "3", IsSelected = true, UnidadMedida = "KILOS", Producto = "TOMATE" });
            ListItems2.Add(new DetalleTraspaso() { Cantidad = "2", IsSelected = false, UnidadMedida = "KILOS", Producto = "CEBOLLA" });
            ListItems2.Add(new DetalleTraspaso() { Cantidad = "5", IsSelected = true, UnidadMedida = "KILOS", Producto = "CHILE" });

            SeleccionIndice = -1;

            GuardarMenuEnabled = false;
            AgregarMenuEnabled = true;
            EliminarMenuEnabled = false;
            EditarMenuEnabled = false;
            CancelarMenuEnabled = false;
            AyudaMenuEnabled = true;
            SalirMenuEnabled = true;
            ExportarMenuEnabled = true;
            AgregarIngredienteEnabled = false;
            CheckVisible = false;
        }
        #endregion

        #region variables

        #region Visibles
        private bool checkVisible;
        public bool CheckVisible
        {
            get { return checkVisible; }
            set { checkVisible = value; OnPropertyChanged("CheckVisible"); }
        }
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
        #endregion

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
        private bool agregarIngredienteEnabled;
        public bool AgregarIngredienteEnabled
        {
            get { return agregarIngredienteEnabled; }
            set { agregarIngredienteEnabled = value; OnPropertyChanged("AgregarIngredienteEnabled"); }
        }
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
        private string producto;
        public string Producto
        {
            get { return producto; }
            set { producto = value; OnPropertyChanged("Producto"); }
        }
        private string unidadMedida;
        public string UnidadMedida
        {
            get { return unidadMedida; }
            set { unidadMedida = value; OnPropertyChanged("UnidadMedida"); }
        }
        private string cantidad;
        public string Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; OnPropertyChanged("Cantidad"); }
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
        private ObservableCollection<DetalleTraspaso> listItems2;
        public ObservableCollection<DetalleTraspaso> ListItems2
        {
            get { return listItems2; }
            set { listItems2 = value; OnPropertyChanged("ListItems2"); }
        }
        #endregion

        public string Name
        {
            get
            {
                return "recetas_alimentacion_internos";
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
                        CheckVisible = true;
                        int i = 0;
                        if (_selectedItem.Password == "12345")
                        {
                            foreach (var item in ListItems2)
                            {
                                if (i % 2 == 0)
                                {
                                    item.IsSelected = true;
                                }
                                else
                                {
                                    item.IsSelected = false;
                                }
                                i = i + 1;
                            }
                        }
                        else
                        {
                            foreach (var item in ListItems2)
                            {
                                if (i % 2 == 0)
                                {
                                    item.IsSelected = false;
                                }
                                else
                                {
                                    item.IsSelected = true;
                                }
                                i = i + 1;
                            }
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
                    //CheckVisible = false;
                    break;
                case "boton_agregar_ingrediente":
                    if (_ingredienteSeleccionado != null)
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
                        CheckVisible = false;
                        _ingredienteSeleccionado.Cantidad = Cantidad;
                    }
                    else
                    {
                        await metro.ShowMessageAsync("Validación", "Debe seleccionar una opcion.");
                    }
                    break;
                case "boton_cancelar_ingrediente":
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
                    CheckVisible = false;
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
                            ListItems2.Add(new DetalleTraspaso() { Cantidad = "3", IsSelected = true, UnidadMedida = "KILOS", Producto = "TOMATE" });
                            ListItems2.Add(new DetalleTraspaso() { Cantidad = "2", IsSelected = false, UnidadMedida = "KILOS", Producto = "CEBOLLA" });
                            ListItems2.Add(new DetalleTraspaso() { Cantidad = "5", IsSelected = true, UnidadMedida = "KILOS", Producto = "CHILE" });
                        }
                        else
                        {
                            ListItems2.Add(new DetalleTraspaso() { Cantidad = "4", IsSelected = true, UnidadMedida = "KILOS", Producto = "CARNE MOLIDA" });
                            ListItems2.Add(new DetalleTraspaso() { Cantidad = "8", IsSelected = false, UnidadMedida = "KILOS", Producto = "TOMATE" });
                            ListItems2.Add(new DetalleTraspaso() { Cantidad = "6", IsSelected = false, UnidadMedida = "KILOS", Producto = "CEBOLLA" });
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
                    //CheckVisible = false;
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
                    CheckVisible = false;
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
                    CheckVisible = false;
                    break;
                case "menu_exportar":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    break;
                case "menu_ayuda":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    break;
                case "menu_salir":
                    SelectedItem = null;
                    SeleccionIndice = -1;
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
        DetalleTraspaso _ingredienteSeleccionado = null;
        public DetalleTraspaso IngredienteSeleccionado
        {
            get
            {
                return _ingredienteSeleccionado;
            }
            set
            {
                _ingredienteSeleccionado = value;
                if (_ingredienteSeleccionado == null)
                {
                    AgregarIngredienteEnabled = false;
                    Cantidad = "";
                }
                else
                {
                    if (_ingredienteSeleccionado.IsSelected || CheckVisible == false)
                    {
                        AgregarIngredienteEnabled = true;
                        Cantidad = _ingredienteSeleccionado.Cantidad;
                        Producto = _ingredienteSeleccionado.Producto;
                        UnidadMedida = _ingredienteSeleccionado.UnidadMedida;
                    }
                    else
                    {
                        AgregarIngredienteEnabled = false;
                        Cantidad = "";
                        Producto = "";
                        UnidadMedida = "";
                    }
                }
            }
        }
        #endregion
    }
}