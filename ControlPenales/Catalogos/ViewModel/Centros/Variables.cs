using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Linq;
using ControlPenales.Clases.Estatus;
namespace ControlPenales
{
    partial class CatalogoCentrosViewModel
    {
        public string Name
        {
            get
            {
                return "catalogo_centros";
            }
        }
        private string _busqueda;
        public string Busqueda
        {
            get { return _busqueda; }
            set { _busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private int _maxLength;
        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; OnPropertyChanged("MaxLength"); }
        }

        private bool _focusText;
        public bool FocusText
        {
            get { return _focusText; }
            set { _focusText = value; OnPropertyChanged("FocusText"); }
        }

        public bool bandera_editar = false;

        private string _cambio;
        public string Cambio
        {
            get { return _cambio; }
            set { _cambio = value; OnPropertyChanged("Cambio"); }
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
            set { _headerAgregar = value; OnPropertyChanged("HeaderAgregar"); }
        }

        private int _seleccionIndice;
        public int SeleccionIndice
        {
            get { return _seleccionIndice; }
            set { _seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }

        private bool _emptyVisible;
        public bool EmptyVisible
        {
            get { return _emptyVisible; }
            set { _emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }

        private bool _guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return _guardarMenuEnabled; }
            set { _guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }

        #region [CONFIGURACION PERMISOS]
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

        private bool _editarEnabled;
        public bool EditarEnabled
        {
            get { return _editarEnabled; }
            set { _editarEnabled = value; OnPropertyChanged("EditarEnabled"); }
        }

        private bool _eliminarMenuEnabled;
        public bool EliminarMenuEnabled
        {
            get { return _eliminarMenuEnabled; }
            set { _eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
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

        private bool _textoHabilitado;
        public bool TextoHabilitado
        {
            get { return _textoHabilitado; }
            set { _textoHabilitado = value; OnPropertyChanged("TextoHabilitado"); }
        }

        private bool _estadoHabilitado;
        public bool EstadoHabilitado
        {
            get { return _estadoHabilitado; }
            set { _estadoHabilitado = value; OnPropertyChanged("EstadoHabilitado"); }
        }

        private bool _municipioHabilitado;
        public bool MunicipioHabilitado
        {
            get { return _municipioHabilitado; }
            set { _municipioHabilitado = value; OnPropertyChanged("MunicipioHabilitado"); }
        }

        private bool _buscarHabilitado;
        public bool BuscarHabilitado
        {
            get { return _buscarHabilitado; }
            set { _buscarHabilitado = value; OnPropertyChanged("BuscarHabilitado"); }
        }
        #endregion

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

        private bool _nuevoVisible;
        public bool NuevoVisible
        {
            get { return _nuevoVisible; }
            set { _nuevoVisible = value; OnPropertyChanged("NuevoVisible"); }

        }

        private string _nombre;
        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; OnPropertyChanged("Nombre"); }
        }

        private string _colonia;
        public string Colonia
        {
            get { return _colonia; }
            set { _colonia = value; OnPropertyChanged("Colonia"); }
        }

        private string _calle;
        public string Calle
        {
            get { return _calle; }
            set { _calle = value; OnPropertyChanged("Calle"); }

        }

        private string _no_exterior;
        public string No_exterior
        {
            get { return _no_exterior; }
            set { _no_exterior = value; OnPropertyChanged("No_exterior"); }
        }
        private string _no_interior;
        public string No_interior
        {
            get { return _no_interior; }
            set { _no_interior = value; OnPropertyChanged("No_interior"); }
        }

        private string _codigo_postal;
        public string Codigo_postal
        {
            get { return _codigo_postal; }
            set { _codigo_postal = value; OnPropertyChanged("Codigo_postal"); }
        }

        private string _telefono;
        public string Telefono
        {
            get { return _telefono; }
            set { _telefono = value; OnPropertyChanged("Telefono"); }
        }

        private string _fax;
        public string Fax
        {
            get { return _fax; }
            set { _fax = value; OnPropertyChanged("Fax"); }
        }

        private string _director;
        public string Director
        {
            get { return _director; }
            set { _director = value; OnPropertyChanged("Director"); }
        }

        private Estatus _selectedEstatus = null;
        public Estatus SelectedEstatus
        {
            get { return _selectedEstatus; }
            set { _selectedEstatus = value; OnPropertyChanged("SelectedEstatus"); }
        }

        private EstatusControl _lista_Estatus = new EstatusControl();
        public EstatusControl Lista_Estatus
        {
            get { return _lista_Estatus; }
            set { _lista_Estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private ObservableCollection<CENTRO> _listItems;
        public ObservableCollection<CENTRO> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private ObservableCollection<ENTIDAD> _listEntidad;
        public ObservableCollection<ENTIDAD> ListEntidad
        {
            get { return _listEntidad; }
            set { _listEntidad = value; OnPropertyChanged("ListEntidad"); }
        }

        private ObservableCollection<ENTIDAD> _listEntidadFiltro;
        public ObservableCollection<ENTIDAD> ListEntidadFiltro
        {
            get { return _listEntidadFiltro; }
            set { _listEntidadFiltro = value; OnPropertyChanged("ListEntidadFiltro"); }
        }

        private ObservableCollection<MUNICIPIO> _listMunicipio;
        public ObservableCollection<MUNICIPIO> ListMunicipio
        {
            get { return _listMunicipio; }
            set { _listMunicipio = value; OnPropertyChanged("ListMunicipio"); }
        }

        private ObservableCollection<MUNICIPIO> _listMunicipios;
        public ObservableCollection<MUNICIPIO> ListMunicipios
        {
            get { return _listMunicipios; }
            set { _listMunicipios = value; OnPropertyChanged("ListMunicipios"); }
        }

        private ENTIDAD _entidad;
        public ENTIDAD Entidad
        {
            get { return _entidad; }
            set
            {
                _entidad = value;

                ListMunicipio = null;
                if (_entidad != null)
                    ListMunicipio = new ObservableCollection<MUNICIPIO>(_entidad.MUNICIPIO);
                OnPropertyChanged("Entidad");
            }
        }

        private ENTIDAD _selectEntidad;
        public ENTIDAD SelectEntidad
        {
            get { return _selectEntidad; }
            set
            {
                _selectEntidad = value;
                ListMunicipio = null;
                if (_selectEntidad != null)
                {
                    ListMunicipio = new ObservableCollection<MUNICIPIO>(objCiudades.ObtenerTodos(string.Empty, _selectEntidad.ID_ENTIDAD));
                    ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    SelectMunicipio = ListMunicipio.Where(w => w.ID_MUNICIPIO == -1).FirstOrDefault();
                }
                OnPropertyChanged("SelectEntidad");
            }
        }

        private MUNICIPIO _selectMunicipio;
        public MUNICIPIO SelectMunicipio
        {
            get { return _selectMunicipio; }
            set { _selectMunicipio = value; OnPropertyChanged("SelectMunicipio"); }
        }

        private MUNICIPIO _selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return _selectedMunicipio; }
            set { _selectedMunicipio = value; OnPropertyChanged("SelectedMunicipio"); }
        }

        private ENTIDAD _selectedEntidad;
        public ENTIDAD SelectedEntidad
        {
            get { return _selectedEntidad; }
            set
            {
                _selectedEntidad = value;
                ListMunicipios = null;
                ListMunicipios = new ObservableCollection<MUNICIPIO>();
                if (_selectedEntidad.ID_ENTIDAD > 0)
                {
                    ListMunicipios = new ObservableCollection<MUNICIPIO>(objCiudades.ObtenerTodos(string.Empty, _selectedEntidad.ID_ENTIDAD));
                    SelectedMunicipio = ListMunicipios.Where(w => w.ID_MUNICIPIO == 0).FirstOrDefault();
                }
                else
                {
                }
                ListMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = 0, MUNICIPIO1 = "SELECCIONAR" });
                SelectedMunicipio = ListMunicipios.Where(w => w.ID_MUNICIPIO == 0).FirstOrDefault();
                OnPropertyChanged("SelectedEntidad");
            }
        }

        private CENTRO _selectedItem;
        public CENTRO SelectedItem
        {
            get { return _selectedItem; }
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
                    if (EditarEnabled)
                    {
                        EditarMenuEnabled = EditarEnabled;
                    }
                }
                OnPropertyChanged("SelectedItem");
            }
        }
    }
}