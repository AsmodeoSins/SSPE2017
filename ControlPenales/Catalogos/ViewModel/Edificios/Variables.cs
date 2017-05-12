using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Linq;
using ControlPenales.Clases.Estatus;
namespace ControlPenales
{
    partial class CatalogoEdificiosViewModel
    {
        public string Name
        {
            get
            {
                return "catalogo_edificios";
            }
        }

        private string _busqueda;
        public string Busqueda
        {
            get { return _busqueda; }
            set { _busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private cEdificio _obj;
        public cEdificio obj
        {
            get { return _obj; }
            set { _obj = value; }
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

        private Estatus _selectedEstatus = null;
        public Estatus SelectedEstatus
        {
            get { return _selectedEstatus; }
            set { _selectedEstatus = value; OnPropertyChanged("SelectedEstatus"); }
        }

        private EstatusControl _lista_estatus = new EstatusControl();
        public EstatusControl Lista_Estatus
        {
            get { return _lista_estatus; }
            set { _lista_estatus = value; RaisePropertyChanged("Lista_Estatus"); }
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

        private ObservableCollection<EDIFICIO> _listItems;
        public ObservableCollection<EDIFICIO> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private ObservableCollection<CENTRO> _listCentros;
        public ObservableCollection<CENTRO> ListCentros
        {
            get { return _listCentros; }
            set { _listCentros = value; OnPropertyChanged("ListCentros"); }
        }

        private ObservableCollection<CENTRO> _listCentrosAlta;
        public ObservableCollection<CENTRO> ListCentrosAlta
        {
            get { return _listCentrosAlta; }
            set { _listCentrosAlta = value; OnPropertyChanged("ListCentrosAlta"); }
        }

        private CENTRO _selectedCentro;
        public CENTRO SelectedCentro
        {
            get { return _selectedCentro; }
            set { _selectedCentro = value; OnPropertyChanged("SelectedCentro"); }
        }

        private CENTRO _selectedCentroAlta;
        public CENTRO SelectedCentroAlta
        {
            get { return _selectedCentroAlta; }
            set { _selectedCentroAlta = value; OnPropertyChanged("SelectedCentroAlta"); }
        }

        private ObservableCollection<MUNICIPIO> _listMunicipios;
        public ObservableCollection<MUNICIPIO> ListMunicipios
        {
            get { return _listMunicipios; }
            set { _listMunicipios = value; OnPropertyChanged("ListMunicipios"); }
        }

        private MUNICIPIO _selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return _selectedMunicipio; }
            set
            {
                _selectedMunicipio = value;
                OnPropertyChanged("SelectedMunicipio");
                ListCentros = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos("", 2, _selectedMunicipio.ID_MUNICIPIO));
                ListCentros.Insert(0, new CENTRO() { DESCR = "TODOS", ID_CENTRO = 0 });
                SelectedCentro = ListCentros.Where(w => w.ID_CENTRO == 0).FirstOrDefault();
            }
        }

        private MUNICIPIO _selectedMunicipioAlta;
        public MUNICIPIO SelectedMunicipioAlta
        {
            get { return _selectedMunicipioAlta; }
            set
            {
                _selectedMunicipioAlta = value;
                OnPropertyChanged("SelectedMunicipioAlta");
                ListCentrosAlta = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos("", 2, _selectedMunicipioAlta.ID_MUNICIPIO));
                ListCentrosAlta.Insert(0, new CENTRO() { DESCR = "TODOS", ID_CENTRO = 0 });
                SelectedCentroAlta = ListCentrosAlta.Where(w => w.ID_CENTRO == 0).FirstOrDefault();
            }
        }

        private EDIFICIO _selectedItem;
        public EDIFICIO SelectedItem
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