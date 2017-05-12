using SSP.Servidor;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class CatalogoModalidadDelitoViewModel
    {
        public string Name
        {
            get
            {
                return "catalogo_modalidad_delito";
            }
        }

        private string _busqueda;
        public string Busqueda
        {
            get { return _busqueda; }
            set { _busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private bool _focusText;
        public bool FocusText
        {
            get { return _focusText; }
            set { _focusText = value; OnPropertyChanged("FocusText"); }
        }

        public bool bandera_editar = false;

        private string _filtroDisplay;
        public string FiltroDisplay
        {
            get { return _filtroDisplay; }
            set { _filtroDisplay = value; OnPropertyChanged("FiltroDisplay"); }
        }

        private short _clave;
        public short Clave
        {
            get { return _clave; }
            set { _clave = value; OnPropertyChanged("Clave"); }
        }

        private string _descripcion;
        public string Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; OnPropertyChanged("Descripcion"); }
        }

        private short _tipo;
        public short Tipo
        {
            get { return _tipo; }
            set { _tipo = value; OnPropertyChanged("Tipo"); }
        }

        private string _catalogHeader;
        public string CatalogoHeader
        {
            get { return _catalogHeader; }
            set { _catalogHeader = value; OnPropertyChanged("CatalogoHeader"); }
        }

        private string _headerAgregar;
        public string HeaderAgregar
        {
            get { return _headerAgregar; }
            set { _headerAgregar = value; }
        }

        private string _comboBoxLigado;
        public string ComboBoxLigado
        {
            get { return _comboBoxLigado; }
            set { _comboBoxLigado = value; }
        }

        private int _seleccionIndice;
        public int SeleccionIndice
        {
            get { return _seleccionIndice; }
            set { _seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }

        private bool _guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return _guardarMenuEnabled; }
            set { _guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }

        private bool _agregarMenuEnabled;
        public bool AgregarMenuEnabled
        {
            get { return _agregarMenuEnabled; }
            set { _agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }

        private bool _editarMenuEnabled;
        public bool EditarMenuEnabled
        {
            get { return _editarMenuEnabled; }
            set { _editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }

        private bool _eliminarMenuEnabled;
        public bool EliminarMenuEnabled
        {
            get { return _eliminarMenuEnabled; }
            set { _eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }

        private bool _cancelarMenuEnabled;
        public bool CancelarMenuEnabled
        {
            get { return _cancelarMenuEnabled; }
            set { _cancelarMenuEnabled = value; OnPropertyChanged("CancelarMenuEnabled"); }
        }

        private bool _exportarMenuEnabled;
        public bool ExportarMenuEnabled
        {
            get { return _exportarMenuEnabled; }
            set { _exportarMenuEnabled = value; OnPropertyChanged("ExportarMenuEnabled"); }
        }

        private bool _salirMenuEnabled;
        public bool SalirMenuEnabled
        {
            get { return _salirMenuEnabled; }
            set { _salirMenuEnabled = value; OnPropertyChanged("SalirMenuEnabled"); }
        }

        private bool _ayudaMenuEnabled;
        public bool AyudaMenuEnabled
        {
            get { return _ayudaMenuEnabled; }
            set { _ayudaMenuEnabled = value; OnPropertyChanged("AyudaMenuEnabled"); }
        }

        private bool _emptyVisible;
        public bool EmptyVisible
        {
            get { return _emptyVisible; }
            set { _emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }

        private bool _filtroVisible;
        public bool FiltroVisible
        {
            get { return _filtroVisible; }
            set { _filtroVisible = value; OnPropertyChanged("FiltroVisible"); }
        }

        private bool _agregarVisible;
        public bool AgregarVisible
        {
            get { return _agregarVisible; }
            set { _agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }

        private bool _editarVisible;
        public bool EditarVisible
        {
            get { return _editarVisible; }
            set { _editarVisible = value; OnPropertyChanged("EditarVisible"); }
        }

        private bool _nuevoVisible;
        public bool NuevoVisible
        {
            get { return _nuevoVisible; }
            set { _nuevoVisible = value; OnPropertyChanged("NuevoVisible"); }
        }

        private ObservableCollection<TIPO_DELITO> _listTipos;
        public ObservableCollection<TIPO_DELITO> ListTipos
        {
            get { return _listTipos; }
            set { _listTipos = value; OnPropertyChanged("ListTipos"); }
        }

        private ObservableCollection<MODALIDAD_DELITO> _listItems;
        public ObservableCollection<MODALIDAD_DELITO> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private MODALIDAD_DELITO _selectedItem;
        public MODALIDAD_DELITO SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
                if (SelectedItem == null)
                {
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                }
                else
                {
                    //EliminarMenuEnabled = true;
                    EditarMenuEnabled = true;
                }
            }
        }

        private TIPO_DELITO _selectedTipo;
        public TIPO_DELITO SelectedTipo
        {
            get { return _selectedTipo; }
            set { _selectedTipo = value; OnPropertyChanged("SelectedTipo"); }
        }
    }
}