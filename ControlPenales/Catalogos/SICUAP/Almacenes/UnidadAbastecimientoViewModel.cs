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
    class UnidadAbastecimientoViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public UnidadAbastecimientoViewModel()
        {
            CatalogoHeader = "Unidad de Abastecimiento";
            HeaderAgregar = "Agregar Nueva Unidad de Abastecimiento";
            NewItem = new Persona();
            UpdateItem = new Persona();
            //LLENAR 
            EditarVisible = false;
            NuevoVisible = false;
            ListItems = new ObservableCollection<Persona>();
            ListItems.Add(new Persona() { Edad = "CERESO MEXICALI", Nombre = "MODULOS", Sexo = "MOVIL", Paterno = "PRUEBA 1", Materno = "DESCRIPCION DE PRUEBA", IsSelected = true, Estatura = "UNO A UNO" });
            ListItems.Add(new Persona() { Edad = "EL HONGO", Nombre = "ALMACEN TIENDA", Sexo = "FIJA", Paterno = "PRUEBA 2", Materno = "DESCRIPCION DE PRUEBA NUMERO DOS", IsSelected = true, Estatura = "UNO A UNO" });
            ListaEjemplo = new List<string>() { "TIPO 1", "TIPO 2" };
            Cereso = new List<string>() { "CERESO MEXICALI", "CERESO TIJUANA", "EL HONGO" };
            TipoBusqueda = new List<string>() { "UNO A UNO" };
            Verificacion = new List<string>() { "N", "S" };
            Almacen = new List<string>() { "FEMENIL", "MODULOS", "ALMACEN TIENDA", "UNIDAD DE ABASTECIMIENTO" };

            //AlmacenSelected = _selectedItem.Nombre;
            //Nombre = _selectedItem.Paterno;
            //Descripcion = _selectedItem.Materno;
            //TipoBusquedaSelected = _selectedItem.Estatura;
            //CeresoSelected = _selectedItem.Edad;
            //VerificacionSelected = _selectedItem.IsSelected;

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
            VerificacionIndice = -1;
            CeresoIndice = -1;
            TipoBusquedaIndice = -1;
            AlmacenIndice = -1;
        }

        #endregion

        #region variables
        public string Name
        {
            get
            {
                return "catalogo_unidades_abastecimiento";
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
        private string comboBoxLigado;
        public string ComboBoxLigado
        {
            get { return comboBoxLigado; }
            set { comboBoxLigado = value; }
        }
        #endregion

        #region Listas
        private List<string> listaEjemplo;
        public List<string> ListaEjemplo
        {
            get { return listaEjemplo; }
            set { listaEjemplo = value; OnPropertyChanged("ListaEjemplo"); }
        }
        private List<string> cereso;
        public List<string> Cereso
        {
            get { return cereso; }
            set { cereso = value; OnPropertyChanged("Cereso"); }
        }
        private List<string> tipoBusqueda;
        public List<string> TipoBusqueda
        {
            get { return tipoBusqueda; }
            set { tipoBusqueda = value; OnPropertyChanged("TipoBusqueda"); }
        }
        private List<string> verificacion;
        public List<string> Verificacion
        {
            get { return verificacion; }
            set { verificacion = value; OnPropertyChanged("Verificacion"); }
        }
        private List<string> almacen;
        public List<string> Almacen
        {
            get { return almacen; }
            set { almacen = value; OnPropertyChanged("Almacen"); }
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

        #region Items
        private Persona updateItem;
        public Persona UpdateItem
        {
            get { return updateItem; }
            set { updateItem = value; OnPropertyChanged("UpdateItem"); }
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

        #region Cambios
        private string tipoBusquedaSelected;
        public string TipoBusquedaSelected
        {
            get { return tipoBusquedaSelected; }
            set { tipoBusquedaSelected = value; OnPropertyChanged("TipoBusquedaSelected"); }
        }
        private string ceresoSelected;
        public string CeresoSelected
        {
            get { return ceresoSelected; }
            set { ceresoSelected = value; OnPropertyChanged("CeresoSelected"); }
        }
        private string verificacionSelected;
        public string VerificacionSelected
        {
            get { return verificacionSelected; }
            set { verificacionSelected = value; OnPropertyChanged("VerificacionSelected"); }
        }
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
        private bool fijaSelected;
        public bool FijaSelected
        {
            get { return fijaSelected; }
            set { fijaSelected = value; OnPropertyChanged("FijaSelected"); }
        }
        private bool movilSelected;
        public bool MovilSelected
        {
            get { return movilSelected; }
            set { movilSelected = value; OnPropertyChanged("MovilSelected"); }
        }
        private string almacenSelected;
        public string AlmacenSelected
        {
            get { return almacenSelected; }
            set { almacenSelected = value; OnPropertyChanged("AlmacenSelected"); }
        }
        #endregion

        #region Indices
        private int seleccionIndice;
        public int SeleccionIndice
        {
            get { return seleccionIndice; }
            set { seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }
        private int ceresoIndice;
        public int CeresoIndice
        {
            get { return ceresoIndice; }
            set { ceresoIndice = value; OnPropertyChanged("CeresoIndice"); }
        }
        private int almacenIndice;
        public int AlmacenIndice
        {
            get { return almacenIndice; }
            set { almacenIndice = value; OnPropertyChanged("AlmacenIndice"); }
        }
        private int tipoBusquedaIndice;
        public int TipoBusquedaIndice
        {
            get { return tipoBusquedaIndice; }
            set { tipoBusquedaIndice = value; OnPropertyChanged("TipoBusquedaIndice"); }
        }
        private int verificacionIndice;
        public int VerificacionIndice
        {
            get { return verificacionIndice; }
            set { verificacionIndice = value; OnPropertyChanged("VerificacionIndice"); }
        }

        #endregion

        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private void LimpiarCampos()
        {
            AlmacenSelected = "";
            Nombre = "";
            Descripcion = "";
            TipoBusquedaSelected = "";
            CeresoSelected = "";
            FijaSelected = false;
            MovilSelected = false;
        }
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "menu_editar":
                    if (_selectedItem != null)
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
                        Nombre = _selectedItem.Paterno;
                        Descripcion = _selectedItem.Materno;
                        TipoBusquedaSelected = _selectedItem.Estatura;
                        CeresoSelected = _selectedItem.Edad;
                        if (_selectedItem.IsSelected)
                        {
                            FijaSelected = true;
                        }
                        else
                        {
                            MovilSelected = true;
                        }
                        if (_selectedItem.IsSelected)
                            VerificacionSelected = "S";
                        else
                            VerificacionSelected = "N";
                        AlmacenSelected = _selectedItem.Nombre;

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
                    LimpiarCampos();
                    break;
                case "menu_guardar":
                    string tipoUA = "";
                    bool verif;
                    if (bandera_editar == false)
                    {
                        if (FijaSelected)
                        {
                            tipoUA = "FIJA";
                        }
                        else if (MovilSelected)
                        {
                            tipoUA = "MOVIL";
                        }
                        if (VerificacionSelected == "S")
                            verif = true;
                        else
                            verif = false;
                        ListItems.Add(new Persona()
                        {
                            Edad = CeresoSelected,
                            Estatura = TipoBusquedaSelected,
                            IsSelected = verif,
                            Materno = Descripcion,
                            Paterno = Nombre,
                            Nombre = AlmacenSelected,
                            Sexo = tipoUA
                        });
                    }
                    else
                    {
                        if (FijaSelected)
                        {
                            tipoUA = "FIJA";
                        }
                        else if (MovilSelected)
                        {
                            tipoUA = "MOVIL";
                        }
                        if (VerificacionSelected == "S")
                            verif = true;
                        else
                            verif = false;
                        SelectedItem.Edad = CeresoSelected;
                        SelectedItem.Estatura = TipoBusquedaSelected;
                        SelectedItem.IsSelected = verif;
                        SelectedItem.Materno = Descripcion;
                        SelectedItem.Paterno = Nombre;
                        SelectedItem.Nombre = AlmacenSelected;
                        SelectedItem.Sexo = tipoUA;
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
                    VerificacionIndice = -1;
                    CeresoIndice = -1;
                    TipoBusquedaIndice = -1;
                    AlmacenIndice = -1;

                    LimpiarCampos();
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
                    VerificacionIndice = -1;
                    CeresoIndice = -1;
                    TipoBusquedaIndice = -1;
                    AlmacenIndice = -1;

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
                    VerificacionIndice = -1;
                    CeresoIndice = -1;
                    TipoBusquedaIndice = -1;
                    AlmacenIndice = -1;
                    LimpiarCampos();
                    break;
                case "menu_exportar":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    VerificacionIndice = -1;
                    CeresoIndice = -1;
                    TipoBusquedaIndice = -1;
                    AlmacenIndice = -1;
                    LimpiarCampos();
                    break;
                case "menu_ayuda":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    VerificacionIndice = -1;
                    CeresoIndice = -1;
                    TipoBusquedaIndice = -1;
                    AlmacenIndice = -1;
                    LimpiarCampos();
                    break;
                case "menu_salir":
                    SelectedItem = null;
                    SeleccionIndice = -1;
                    VerificacionIndice = -1;
                    CeresoIndice = -1;
                    TipoBusquedaIndice = -1;
                    AlmacenIndice = -1;
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