using SSP.Servidor;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Justicia;
using System.Linq;
using ControlPenales.Clases.Estatus;

namespace ControlPenales
{
    partial class CatalogoTipoMensajeViewModel
    {
        public string Name
        {
            get
            {
                return "catalogo_sector_clasificacion";
            }
        }

        private string _busqueda;
        public string Busqueda
        {
            get { return _busqueda; }
            set { _busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        //VARIABLES
        private string _descripcion;
        public string Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; OnPropertyChanged("Descripcion"); }
        }

        private short? _prioridad;
        public short? Prioridad
        {
            get { return _prioridad; }
            set { _prioridad = value; OnPropertyChanged("Prioridad"); }
        }

        private string _color;
        public string Color
        {
            get { return _color; }
            set { _color = value; OnPropertyChanged("Color"); }
        }

        private string _colorPopUp;
        public string ColorPopUp
        {
            get { return _colorPopUp; }
            set { _colorPopUp = value; OnPropertyChanged("ColorPopUp"); }
        }

        private string _encabezado;
        public string Encabezado
        {
            get { return _encabezado; }
            set { _encabezado = value; OnPropertyChanged("Encabezado"); }
        }

        private string _contenido;
        public string Contenido
        {
            get { return _contenido; }
            set { _contenido = value; OnPropertyChanged("Contenido"); }
        }

        private short TipoColor;

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
        private bool _agregarMenuEnabled = false;
        public bool AgregarMenuEnabled
        {
            get { return _agregarMenuEnabled; }
            set { _agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }

        private bool _editarMenuEnabled = false;
        public bool EditarMenuEnabled
        {
            get { return _editarMenuEnabled; }
            set { _editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }

        private bool _editarEnabled = false;
        public bool EditarEnabled
        {
            get { return _editarEnabled; }
            set { _editarEnabled = value; OnPropertyChanged("EditarEnabled"); }
        }

        private bool _eliminarMenuEnabled = false;
        public bool EliminarMenuEnabled
        {
            get { return _eliminarMenuEnabled; }
            set { _eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }

        private bool _agregarVisible = false;
        public bool AgregarVisible
        {
            get { return _agregarVisible; }
            set { _agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }

        private bool _editarVisible = false;
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

        //LISTADO
        private bool _emptyVisible;
        public bool EmptyVisible
        {
            get { return _emptyVisible; }
            set { _emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }

        private ObservableCollection<MENSAJE_TIPO> _listItems;
        public ObservableCollection<MENSAJE_TIPO> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private MENSAJE_TIPO _selectedItem;
        public MENSAJE_TIPO SelectedItem
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

        private EstatusControl _listEstatus = new EstatusControl();
        public EstatusControl ListEstatus
        {
            get { return _listEstatus; }
            set { _listEstatus = value; OnPropertyChanged("ListEstatus"); }
        }

        private Estatus _selectedEstatus;
        public Estatus SelectedEstatus
        {
            get { return _selectedEstatus; }
            set { _selectedEstatus = value; OnPropertyChanged("SelectedEstatus"); }
        }

        #region Roles
        private ObservableCollection<SISTEMA_ROL> lstRol;
        public ObservableCollection<SISTEMA_ROL> LstRol
        {
            get { return lstRol; }
            set { lstRol = value; OnPropertyChanged("LstRol"); }
        }

        private SISTEMA_ROL selectedRol;
        public SISTEMA_ROL SelectedRol
        {
            get { return selectedRol; }
            set { selectedRol = value; OnPropertyChanged("SelectedRol"); }
        }

        private short? rol = -1;
        public short? Rol
        {
            get { return rol; }
            set { rol = value; OnPropertyChanged("Rol"); }
        }

        private ObservableCollection<MENSAJE_ROL> lstMensajeRol;
        public ObservableCollection<MENSAJE_ROL> LstMensajeRol
        {
            get { return lstMensajeRol; }
            set { lstMensajeRol = value; OnPropertyChanged("LstMensajeRol"); }
        }

        private MENSAJE_ROL selectedMensajeRol;
        public MENSAJE_ROL SelectedMensajeRol
        {
            get { return selectedMensajeRol; }
            set { selectedMensajeRol = value; OnPropertyChanged("SelectedMensajeRol"); }
        }
        #endregion
    }
}