using SSP.Servidor;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Justicia;
using System.Linq;
using ControlPenales.Clases.Estatus;

namespace ControlPenales
{
    partial class CatalogoCamasViewModel
    {
        public string Name
        {
            get
            {
                return "catalogo_camas";
            }
        }
        private string _busqueda;
        public string Busqueda
        {
            get { return _busqueda; }
            set { _busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private bool _celdasEnabled;
        public bool CeldasEnabled
        {
            get { return _celdasEnabled; }
            set { _celdasEnabled = value; OnPropertyChanged("CeldasEnabled"); }
        }

        private bool _descripcionEnabled;
        public bool DescripcionEnabled
        {
            get { return _descripcionEnabled; }
            set { _descripcionEnabled = value; OnPropertyChanged("DescripcionEnabled"); }
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

        private int _municipio;
        public int Municipio
        {
            get { return _municipio; }
            set { _municipio = value; OnPropertyChanged("Municipio"); }
        }

        private int _centro;
        public int Centro
        {
            get { return _centro; }
            set { _centro = value; OnPropertyChanged("Centro"); }
        }

        private int _edificio;
        public int Edificio
        {
            get { return _edificio; }
            set { _edificio = value; OnPropertyChanged("Edificio"); }
        }

        private string _celda;
        public string Celda
        {
            get { return _celda; }
            set { _celda = value; OnPropertyChanged("Celda"); }
        }

        private int _sector;
        public int Sector
        {
            get { return _sector; }
            set { _sector = value; OnPropertyChanged("Sector"); }
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

        private bool _emptyVisible;
        public bool EmptyVisible
        {
            get { return _emptyVisible; }
            set { _emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
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
            set { _headerAgregar = value; }
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

        #region [CONFIGURACION PERMISOS]
        private bool _agregarMenuEnabled;
        public bool AgregarMenuEnabled
        {
            get { return _agregarMenuEnabled; }
            set { _agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }

        private bool _dataGridEnabled;
        public bool DataGridEnabled 
        {
            get { return _dataGridEnabled; }
            set { _dataGridEnabled = value; OnPropertyChanged("DataGridEnabled"); }
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

        private bool _municipioHabilitado;
        public bool MunicipioHabilitado
        {
            get { return _municipioHabilitado; }
            set { _municipioHabilitado = value; OnPropertyChanged("MunicipioHabilitado"); }
        }

        private bool _centroHabilitado;
        public bool CentroHabilitado
        {
            get { return _centroHabilitado; }
            set { _centroHabilitado = value; OnPropertyChanged("CentroHabilitado"); }
        }

        private bool _edificioHabilitado;
        public bool EdificioHabilitado
        {
            get { return _edificioHabilitado; }
            set { _edificioHabilitado = value; OnPropertyChanged("EdificioHabilitado"); }
        }

        private bool _sectorHabilitado;
        public bool SectorHabilitado
        {
            get { return _sectorHabilitado; }
            set { _sectorHabilitado = value; OnPropertyChanged("SectorHabilitado"); }
        }

        private bool _celdaEnabled;
        public bool CeldaEnabled
        {
            get { return _celdaEnabled; }
            set { _celdaEnabled = value; OnPropertyChanged("CeldaEnabled"); }
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

        private ObservableCollection<CAMA> _listItems;
        public ObservableCollection<CAMA> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private ObservableCollection<SECTOR> _listSectores;
        public ObservableCollection<SECTOR> ListSectores
        {
            get { return _listSectores; }
            set { _listSectores = value; OnPropertyChanged("ListSectores"); }
        }

        private ObservableCollection<EDIFICIO> _listEdificios;
        public ObservableCollection<EDIFICIO> ListEdificios
        {
            get { return _listEdificios; }
            set { _listEdificios = value; OnPropertyChanged("ListEdificios"); }
        }

        private ObservableCollection<CENTRO> _listCentros;
        public ObservableCollection<CENTRO> ListCentros
        {
            get { return _listCentros; }
            set
            { _listCentros = value; OnPropertyChanged("ListCentros"); }
        }

        private ObservableCollection<MUNICIPIO> _listMunicipios;
        public ObservableCollection<MUNICIPIO> ListMunicipios
        {
            get { return _listMunicipios; }
            set { _listMunicipios = value; OnPropertyChanged("ListMunicipios"); }
        }

        private ObservableCollection<CELDA> _listCeldas;
        public ObservableCollection<CELDA> ListCeldas
        {
            get { return _listCeldas; }
            set { _listCeldas = value; OnPropertyChanged("ListCeldas"); }
        }

        private CELDA _selectedCelda;
        public CELDA SelectedCelda
        {
            get { return _selectedCelda; }
            set { _selectedCelda = value; OnPropertyChanged("SelectedCelda"); }

        }

        private SECTOR _selectedSector;
        public SECTOR SelectedSector
        {
            get { return _selectedSector; }
            set
            {
                _selectedSector = value;
                OnPropertyChanged("SelectedSector");
                if (value != null)
                {
                    if (_selectedSector.ID_SECTOR == -1)
                    {
                        CeldasEnabled = false;
                        ListCeldas = new ObservableCollection<CELDA>();
                    }
                    else
                    {
                        CeldasEnabled = true;
                        ListCeldas = new ObservableCollection<CELDA>(SelectedSector.CELDA); //new ObservableCollection<CELDA>(new cCelda().ObtenerTodos(string.Empty, selectedMunicipio, selectedCentro, selectedEdificio, selectedSector));
                    }
                    ListCeldas.Insert(0, new CELDA() { ID_CELDA = "SELECCIONE" });
                    SelectedCelda = ListCeldas.Where(w => w.ID_CELDA == "SELECCIONE").FirstOrDefault();
                    Celda = SelectedCelda.ID_CELDA;
                }
            }
        }

        private EDIFICIO _selectedEdificio;
        public EDIFICIO SelectedEdificio
        {
            get { return _selectedEdificio; }
            set
            {
                _selectedEdificio = value;
                OnPropertyChanged("SelectedEdificio");
                if (value != null)
                {
                    if (_selectedEdificio.ID_EDIFICIO == -1)
                        ListSectores = new ObservableCollection<SECTOR>();
                    else
                        //ListSectores = new ObservableCollection<SECTOR>(new cSector().ObtenerTodos(string.Empty, selectedMunicipio.ID_MUNICIPIO, selectedCentro.ID_CENTRO, selectedEdificio.ID_EDIFICIO));
                        ListSectores = new ObservableCollection<SECTOR>(_selectedEdificio.SECTOR);
                    ListSectores.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
                    SelectedSector = ListSectores.Where(w => w.ID_SECTOR == -1).FirstOrDefault();
                }
            }
        }

        private CENTRO _selectedCentro;
        public CENTRO SelectedCentro
        {
            get { return _selectedCentro; }
            set
            {
                _selectedCentro = value;
                OnPropertyChanged("SelectedCentro");
                if (value != null)
                {
                    if (_selectedCentro.ID_CENTRO == -1)
                        ListEdificios = new ObservableCollection<EDIFICIO>();
                    else
                        //ListEdificios = new ObservableCollection<EDIFICIO>(new cEdificio().ObtenerTodos(string.Empty, selectedMunicipio.ID_MUNICIPIO, selectedCentro.ID_CENTRO));
                        ListEdificios = new ObservableCollection<EDIFICIO>(_selectedCentro.EDIFICIO);
                    ListEdificios.Insert(0, new EDIFICIO() { ID_EDIFICIO = -1, DESCR = "SELECCIONE" });
                    SelectedEdificio = ListEdificios.Where(w => w.ID_EDIFICIO == -1).FirstOrDefault();
                }
            }
        }

        private MUNICIPIO _selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return _selectedMunicipio; }
            set
            {
                _selectedMunicipio = value;
                OnPropertyChanged("SelectedMunicipio");
                if (value != null)
                {
                    if (_selectedMunicipio.ID_MUNICIPIO == -1)
                        ListCentros = new ObservableCollection<CENTRO>();
                    else
                        ListCentros = new ObservableCollection<CENTRO>(_selectedMunicipio.CENTRO);//; new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos(string.Empty, 2, selectedMunicipio.ID_MUNICIPIO));
                    ListCentros.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = "SELECCIONE" });
                    SelectedCentro = ListCentros.Where(w => w.ID_CENTRO == -1).FirstOrDefault();
                }
            }
        }

        private CAMA _selectedItem;
        public CAMA SelectedItem
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