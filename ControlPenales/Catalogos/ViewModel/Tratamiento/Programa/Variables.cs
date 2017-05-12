using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;


namespace ControlPenales
{
    partial class CatalogoProgramasViewModel
    {
        private Visibility _generalVisible;
        public Visibility GeneralVisible
        {
            get { return _generalVisible; }
            set { _generalVisible = value; OnPropertyChanged("GeneralVisible"); }
        }

        //public string CatalogoHeader
        //{
        //    get { return "Programas"; }
        //}

        private string _busqueda;
        public string Busqueda
        {
            get { return _busqueda; }
            set { _busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private ObservableCollection<DEPARTAMENTO> _catBusqueda;
        public ObservableCollection<DEPARTAMENTO> CatBusqueda
        {
            get { return _catBusqueda; }
            set { _catBusqueda = value; OnPropertyChanged("CatBusqueda"); }
        }

        private short _selectedcatTipo = -1;
        public short SelectedcatTipo
        {
            get { return _selectedcatTipo; }
            set { _selectedcatTipo = value; OnPropertyChanged("SelectedcatTipo"); }
        }

        private List<TIPO_PROGRAMA> _listItems;
        public List<TIPO_PROGRAMA> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private TIPO_PROGRAMA _selectedItem;
        public TIPO_PROGRAMA SelectedItem
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

        private bool _emptyVisible;
        public bool EmptyVisible
        {
            get { return _emptyVisible; }
            set { _emptyVisible = value; OnPropertyChanged("EmptyVisible"); }
        }

        private bool _panelVisible;
        public bool PanelVisible
        {
            get { return _panelVisible; }
            set { _panelVisible = value; OnPropertyChanged("PanelVisible"); }
        }

        private string _accionHeader;
        public string AccionHeader
        {
            get { return _accionHeader; }
            set { _accionHeader = value; OnPropertyChanged("AccionHeader"); }
        }

        private List<DEPARTAMENTO> _listTipos;
        public List<DEPARTAMENTO> ListTipos
        {
            get { return _listTipos; }
            set { _listTipos = value; OnPropertyChanged("ListTipos"); }
        }

        private List<DEPARTAMENTO> _listTiposFiltros;
        public List<DEPARTAMENTO> ListTiposFiltros
        {
            get { return _listTiposFiltros; }
            set { _listTiposFiltros = value; OnPropertyChanged("ListTiposFiltros"); }
        }

        private short? _selectedTipo;
        public short? SelectedTipo
        {
            get { return _selectedTipo; }
            set { _selectedTipo = value; OnPropertyChanged("SelectedTipo"); }
        }

        private string _nombre;
        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; OnPropertyChanged("Nombre"); }
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

        private EstatusControl _lista_estatus = new EstatusControl();
        public EstatusControl Lista_Estatus
        {
            get { return _lista_estatus; }
            set { _lista_estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private Estatus _selectedEstatus = null;
        public Estatus SelectedEstatus
        {
            get { return _selectedEstatus; }
            set { _selectedEstatus = value; RaisePropertyChanged("SelectedEstatus"); }
        }

        private bool _guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return _guardarMenuEnabled; }
            set { _guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
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

        private bool _nuevoVisible;
        public bool NuevoVisible
        {
            get { return _nuevoVisible; }
            set { _nuevoVisible = value; OnPropertyChanged("NuevoVisible"); }
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

        private bool _departamentoHabilitado;
        public bool DepartamentoHabilitado
        {
            get { return _departamentoHabilitado; }
            set { _departamentoHabilitado = value; OnPropertyChanged("DepartamentoHabilitado"); }
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

        private bool _ayudaMenuEnabled = true;
        public bool AyudaMenuEnabled
        {
            get { return _ayudaMenuEnabled; }
            set { _ayudaMenuEnabled = value; OnPropertyChanged("AyudaMenuEnabled"); }
        }

        private bool _salirMenuEnabled = true;
        public bool SalirMenuEnabled
        {
            get { return _salirMenuEnabled; }
            set { _salirMenuEnabled = value; OnPropertyChanged("SalirMenuEnabled"); }
        }

        private bool _exportarMenuEnabled;
        public bool ExportarMenuEnabled
        {
            get { return _exportarMenuEnabled; }
            set { _exportarMenuEnabled = value; OnPropertyChanged("ExportarMenuEnabled"); }
        }

        private int _maxLength;
        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; OnPropertyChanged("MaxLength"); }
        }

        private int _seleccionIndice;
        public int SeleccionIndice
        {
            get { return _seleccionIndice; }
            set { _seleccionIndice = value; OnPropertyChanged("SeleccionIndice"); }
        }

        private short _selectedIndex;
        public short SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; OnPropertyChanged("SelectedIndex"); }
        }
    }
}