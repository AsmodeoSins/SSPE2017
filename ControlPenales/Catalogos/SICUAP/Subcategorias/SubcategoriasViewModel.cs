using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace ControlPenales
{
    class SubcategoriasViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public SubcategoriasViewModel()
        {
            AgregarHeader = "Agregar Nueva Subcategoria";
            CatalogoHeader = "Subcategorias";

            ListItems = new ObservableCollection<Usuario>();
            ListItems.Add(new Usuario() { Username = "3500", Password = "TIPO 1" });
            ListItems.Add(new Usuario() { Username = "4250", Password = "TIPO 2" });

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
        }

        #endregion

        #region variables
        public string Name
        {
            get
            {
                return "catalogo_subcategorias";
            }
        }

        #region Otros
        private string catalogoHeader;
        public string CatalogoHeader
        {
            get { return catalogoHeader; }
            set { catalogoHeader = value; }
        }
        private string agregarHeader;
        public string AgregarHeader
        {
            get { return agregarHeader; }
            set { agregarHeader = value; }
        }
        private bool focusText;
        public bool FocusText
        {
            get { return focusText; }
            set { focusText = value; OnPropertyChanged("FocusText"); }
        }
        public bool bandera_editar = false;
        private string nombre;
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; OnPropertyChanged("Nombre"); }
        }
        private string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; OnPropertyChanged("Descripcion"); }
        }
        private int seleccionIndice;
        public int SeleccionIndice
        {
            get { return seleccionIndice; }
            set { seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
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

        #region Visibles
        private bool agregarVisible;
        public bool AgregarVisible
        {
            get { return agregarVisible; }
            set { agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }
        #endregion

        #region Items
        private ObservableCollection<Usuario> listItems;
        public ObservableCollection<Usuario> ListItems
        {
            get { return listItems; }
            set { listItems = value; OnPropertyChanged("ListItems"); }
        }
        #endregion

        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "menu_editar":
                    if (_selectedItem != null)
                    {
                        Descripcion = SelectedItem.Password;
                        Nombre = SelectedItem.Username;

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
                    }
                    else
                    {
                        bandera_editar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una opcion.");
                    }
                    break;
                case "menu_agregar":
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
                    break;
                case "menu_guardar":
                    if (bandera_editar == false)
                    {
                        ListItems.Add(new Usuario() { Username = Nombre, Password = Descripcion });
                    }
                    else
                    {
                        SelectedItem.Username = Nombre;
                        SelectedItem.Password = Descripcion;
                    }

                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;

                    AgregarVisible = false;

                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Nombre = string.Empty;
                    Descripcion = string.Empty;
                    break;
                case "menu_cancelar":
                    GuardarMenuEnabled = false;
                    AgregarMenuEnabled = true;
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                    AyudaMenuEnabled = true;
                    SalirMenuEnabled = true;
                    ExportarMenuEnabled = true;

                    AgregarVisible = false;

                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Nombre = string.Empty;
                    Descripcion = string.Empty;
                    break;
                case "menu_eliminar":
                    var metro = Application.Current.Windows[0] as MetroWindow;
                    if (_selectedItem != null)
                    {
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Aceptar",
                            NegativeButtonText = "Cancelar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.Password + ", " + SelectedItem.Username + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            var i = ListItems.IndexOf(_selectedItem);
                            if (i >= 0)
                            {
                                ListItems.RemoveAt(i);

                                var dialog = (BaseMetroDialog)metro.Resources["ConfirmacionDialog"];
                                await metro.ShowMetroDialogAsync(dialog);
                                await TaskEx.Delay(1250);
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
                    Nombre = string.Empty;
                    Descripcion = string.Empty;
                    break;
                case "menu_exportar":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Nombre = string.Empty;
                    Descripcion = string.Empty;
                    break;
                case "menu_ayuda":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Nombre = string.Empty;
                    Descripcion = string.Empty;
                    break;
                case "menu_salir":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    Nombre = string.Empty;
                    Descripcion = string.Empty;
                    break;
            }
            if (_selectedItem == null)
            {
                EliminarMenuEnabled = false;
                EditarMenuEnabled = false;
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
        #endregion

    }
}