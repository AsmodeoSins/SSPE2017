using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    class CatalogoEquiposViewModel : ValidationViewModelBase, IPageViewModel
    {
        
        #region constructor
        public CatalogoEquiposViewModel()
        {
            CatalogoHeader = "Equipos";
            HeaderAgregar = "Agregar Equipo";
            NewItem = new Persona();
            UpdateItem = new Persona();
            //LLENAR 
            EditarVisible = false;
            NuevoVisible = false;
            ListItems = new ObservableCollection<Persona>();
            ListItems.Add(new Persona() { Nombre = "10.31.208.45", Materno = "98:90:96:AB:9D:23", Paterno = "B48EF40A", Edad = "Activo", Sexo = "Desktop", Estatura = "Descripcion de ejemplo 1" });
            ListItems.Add(new Persona() { Nombre = "10.31.6.106", Materno = "84:2B:2B:A7:97:C0", Paterno = "FC1C03E2", Edad = "Inactivo", Sexo = "Desktop", Estatura = "Descripcion de ejemplo 2" });
            ListaEjemplo = new List<string>() { "ACTIVO", "INACTIVO" };
            ListaEjemplo2 = new List<string>() { "LAPTOP", "DESKTOP" };
            GeneralEnabled = true;

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
                return "catalogo_guardian_equipos";
            }
        }

        #region Visibles
        private bool agregarVisible;
        public bool AgregarVisible
        {
            get { return agregarVisible; }
            set { agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }
        private bool editarVisible;
        public bool EditarVisible
        {
            get { return editarVisible; }
            set { editarVisible = value; OnPropertyChanged("EditarVisible"); }
        }
        private bool nuevoVisible;
        public bool NuevoVisible
        {
            get { return nuevoVisible; }
            set { nuevoVisible = value; OnPropertyChanged("NuevoVisible"); }
        }
        #endregion

        #region CAMBIOS
        private string tipo;
        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; OnPropertyChanged("Tipo"); }
        }
        private string activo;
        public string Activo
        {
            get { return activo; }
            set { activo = value; OnPropertyChanged("Activo"); }
        }
        private string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; OnPropertyChanged("Descripcion"); }
        }
        private string _hd;
        public string HD
        {
            get { return _hd; }
            set { _hd = value; OnPropertyChanged("HD"); }
        }
        private string _mac;
        public string MAC
        {
            get { return _mac; }
            set { _mac = value; OnPropertyChanged("MAC"); }
        }
        private string _ip;
        public string IP
        {
            get { return _ip; }
            set { _ip = value; OnPropertyChanged("IP"); }
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

        #region Otros
        private bool generalEnabled;
        public bool GeneralEnabled
        {
            get { return generalEnabled; }
            set { generalEnabled = value; OnPropertyChanged("GeneralEnabled"); }
        }
        private List<string> listaEjemplo2;
        public List<string> ListaEjemplo2
        {
            get { return listaEjemplo2; }
            set { listaEjemplo2 = value; OnPropertyChanged("ListaEjemplo2"); }
        }
        private List<string> listaEjemplo;
        public List<string> ListaEjemplo
        {
            get { return listaEjemplo; }
            set { listaEjemplo = value; OnPropertyChanged("ListaEjemplo"); }
        }
        private string catalogHeader;
        public string CatalogoHeader
        {
            get { return catalogHeader; }
            set { catalogHeader = value; }
        }
        private bool focusText;
        public bool FocusText
        {
            get { return focusText; }
            set { focusText = value; OnPropertyChanged("FocusText"); }
        }
        public bool bandera_editar = false;
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
        private Persona updateItem;
        public Persona UpdateItem
        {
            get { return updateItem; }
            set { updateItem = value; }
        }
        private Persona newItem;
        public Persona NewItem
        {
            get { return newItem; }
            set { newItem = value; OnPropertyChanged("NewItem"); }
        }
        private ObservableCollection<Persona> listItems;
        public ObservableCollection<Persona> ListItems
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
                case "buscar_articulo_medicamento":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    IP = string.Empty;
                    HD = string.Empty;
                    MAC = string.Empty;
                    Tipo = string.Empty;
                    Activo = string.Empty;
                    Descripcion = string.Empty;
                    break;
                case "menu_editar":
                    if (_selectedItem != null)
                    {
                        EditarVisible = true;
                        NuevoVisible = false;
                        IP = SelectedItem.Nombre;
                        MAC = SelectedItem.Paterno;
                        HD = SelectedItem.Materno;
                        Tipo = SelectedItem.Sexo;
                        Activo = SelectedItem.Edad;
                        Descripcion = SelectedItem.Estatura;

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
                    break;
                case "menu_guardar":
                    if (bandera_editar == false)
                    {
                        ListItems.Add(new Persona()
                        {
                            Nombre = IP,
                            Paterno = MAC,
                            Materno = HD,
                            Sexo = Tipo,
                            Edad = Activo,
                            Estatura = Descripcion
                        });
                    }
                    else
                    {
                        SelectedItem.Nombre = IP;
                        SelectedItem.Paterno = MAC;
                        SelectedItem.Materno = HD;
                        SelectedItem.Sexo = Tipo;
                        SelectedItem.Edad = Activo;
                        SelectedItem.Estatura = Descripcion;
                    }

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

                    SelectedItem = null;
                    SeleccionIndice = -1;
                    IP = string.Empty;
                    HD = string.Empty;
                    MAC = string.Empty;
                    Tipo = string.Empty;
                    Activo = string.Empty;
                    Descripcion = string.Empty;
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

                    SelectedItem = null;
                    SeleccionIndice = -1;
                    IP = string.Empty;
                    HD = string.Empty;
                    MAC = string.Empty;
                    Tipo = string.Empty;
                    Activo = string.Empty;
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
                        var result = await metro.ShowMessageAsync("Borrar", "¿Está seguro que desea borrar esto? [ " + SelectedItem.Nombre + " " + SelectedItem.Paterno + " " + SelectedItem.Materno + " ]", MessageDialogStyle.AffirmativeAndNegative, mySettings);
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
                    IP = string.Empty;
                    HD = string.Empty;
                    MAC = string.Empty;
                    Tipo = string.Empty;
                    Activo = string.Empty;
                    Descripcion = string.Empty;
                    break;
                case "menu_exportar":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    IP = string.Empty;
                    HD = string.Empty;
                    MAC = string.Empty;
                    Tipo = string.Empty;
                    Activo = string.Empty;
                    Descripcion = string.Empty;
                    break;
                case "menu_ayuda":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    IP = string.Empty;
                    HD = string.Empty;
                    MAC = string.Empty;
                    Tipo = string.Empty;
                    Activo = string.Empty;
                    Descripcion = string.Empty;
                    break;
                case "menu_salir":
                    PrincipalViewModel.SalirMenu();
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
