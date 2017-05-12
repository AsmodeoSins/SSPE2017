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
    class ProveedoresViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public ProveedoresViewModel()
        {
            ListItems = new ObservableCollection<Persona>();
            ListItems.Add(new Persona() { Edad = "TEST1", Paterno = "PRUEBA 1", Nombre = "SIDH870523TC8", Sexo = "RIO CHAMPOTON 2743 GONZALEZ ORTEGA", Materno = "PROVEEDOR NUMERO UNO", IsSelected = false, Estatura = "(686)-561-36-21" });
            ListItems.Add(new Persona() { Edad = "TEST2", Paterno = "PRUEBA 2", Nombre = "PEGJ801122LL3", Sexo = "CALLE CUARTA 1000 1RO DE MAYO", Materno = "PROVEEDOR NUMERO DOS", IsSelected = false, Estatura = "(686)-580-23-56" });

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
                return "catalogo_proveedores";
            }
        }

        #region Otros
        private bool focusText;
        public bool FocusText
        {
            get { return focusText; }
            set { focusText = value; OnPropertyChanged("FocusText"); }
        }
        public bool bandera_editar = false;
        #endregion

        #region Listas
        private List<string> listaEjemplo;
        public List<string> ListaEjemplo
        {
            get { return listaEjemplo; }
            set { listaEjemplo = value; OnPropertyChanged("ListaEjemplo"); }
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
        private ObservableCollection<Persona> listItems;
        public ObservableCollection<Persona> ListItems
        {
            get { return listItems; }
            set { listItems = value; OnPropertyChanged("ListItems"); }
        }
        #endregion

        #region Cambios
        private string rfc;
        public string RFC
        {
            get { return rfc; }
            set { rfc = value; OnPropertyChanged("RFC"); }
        }
        private string homoClave;
        public string HomoClave
        {
            get { return homoClave; }
            set { homoClave = value; OnPropertyChanged("HomoClave"); }
        }
        private string nombre;
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; OnPropertyChanged("Nombre"); }
        }
        private string razonSocial;
        public string RazonSocial
        {
            get { return razonSocial; }
            set { razonSocial = value; OnPropertyChanged("RazonSocial"); }
        }
        private string direccion;
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; OnPropertyChanged("Direccion"); }
        }
        private string telefono;
        public string Telefono
        {
            get { return telefono; }
            set { telefono = value; OnPropertyChanged("Telefono"); }
        }
        private string contacto;
        public string Contacto
        {
            get { return contacto; }
            set { contacto = value; OnPropertyChanged("Contacto"); }
        }
        #endregion

        #region Indices
        private int seleccionIndice;
        public int SeleccionIndice
        {
            get { return seleccionIndice; }
            set { seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }
        #endregion

        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private void LimpiarCampos()
        {
            Nombre = "";
            Direccion = "";
            RazonSocial = "";
            HomoClave = "";
            Contacto = "";
            Telefono = "";
            RFC = "";
        }
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "menu_editar":
                    if (_selectedItem != null)
                    {
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
                        RFC = _selectedItem.Nombre;
                        Nombre = _selectedItem.Materno;
                        Direccion = _selectedItem.Sexo;
                        RazonSocial = _selectedItem.Paterno;
                        HomoClave = _selectedItem.Edad;
                        Contacto = _selectedItem.IsSelected.ToString();
                        Telefono = _selectedItem.Estatura;

                    }
                    else
                    {
                        bandera_editar = false;
                        var met = Application.Current.Windows[0] as MetroWindow;
                        await met.ShowMessageAsync("Validación", "Debe seleccionar una opcion.");
                    }
                    break;
                case "menu_agregar":
                    AgregarVisible = true;

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
                    LimpiarCampos();
                    break;
                case "menu_guardar":
                    bool cont;
                    if (Contacto == "")
                        cont = true;
                    else
                        cont = false;
                    if (bandera_editar == false)
                    {
                        ListItems.Add(new Persona()
                        {
                            Edad = HomoClave,
                            Estatura = Telefono,
                            IsSelected = cont,
                            Materno = Nombre,
                            Paterno = RazonSocial,
                            Nombre = RFC,
                            Sexo = Direccion
                        });
                    }
                    else
                    {
                        SelectedItem.Edad = HomoClave;
                        SelectedItem.Estatura = Telefono;
                        SelectedItem.IsSelected = cont;
                        SelectedItem.Materno = Nombre;
                        SelectedItem.Paterno = RazonSocial;
                        SelectedItem.Nombre = RFC;
                        SelectedItem.Sexo = Direccion;
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

                    LimpiarCampos();
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

                    LimpiarCampos();
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
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + 1 + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
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
                    LimpiarCampos();
                    break;
                case "menu_exportar":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    LimpiarCampos();
                    break;
                case "menu_ayuda":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    LimpiarCampos();
                    break;
                case "menu_salir":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    LimpiarCampos();
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
        Persona _selectedItem = null;
        public Persona SelectedItem
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