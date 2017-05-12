using ControlPenales.Clases.Estatus;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CatalogosAgenciasViewModel
    {
        public string Name
        {
            get
            {
                return "catalogo_agencias";
            }
        }

        private string _busqueda;
        public string Busqueda
        {
            get { return _busqueda; }
            set { _busqueda = value; OnPropertyChanged("Busqueda"); }
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

        private string _domicilio;
        public string Domicilio
        {
            get { return _domicilio; }
            set { _domicilio = value; OnPropertyChanged("Domicilio"); }
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
            set { _headerAgregar = value; OnPropertyChanged("HeaderAgregar"); }
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

        private ObservableCollection<MUNICIPIO> _listMunicipio;
        public ObservableCollection<MUNICIPIO> ListMunicipio
        {
            get { return _listMunicipio; }
            set { _listMunicipio = value; OnPropertyChanged("ListMunicipio"); }
        }

        private MUNICIPIO _selectMunicipio;
        public MUNICIPIO SelectMunicipio
        {
            get { return _selectMunicipio; }
            set { _selectMunicipio = value; OnPropertyChanged("SelectMunicipio"); }
        }

        private int _selectTipoAgenciaIndex;
        public int SelectTipoAgenciaIndex
        {
            get { return _selectTipoAgenciaIndex; }
            set { _selectTipoAgenciaIndex = value; OnPropertyChanged("SelectTipoAgenciaIndex"); }
        }

        private ObservableCollection<ENTIDAD> _listEntidad;
        public ObservableCollection<ENTIDAD> ListEntidad
        {
            get { return _listEntidad; }
            set { _listEntidad = value; OnPropertyChanged("ListEntidad"); }
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

        private List<TipoAgenciaClass> _listaAgenciasTipo;
        public List<TipoAgenciaClass> ListaAgenciasTipo
        {
            get { return _listaAgenciasTipo; }
            set { _listaAgenciasTipo = value; OnPropertyChanged("ListaAgenciasTipo"); }
        }

        private TipoAgenciaClass _selectAgenciaTipo;
        public TipoAgenciaClass SelectAgenciaTipo
        {
            get { return _selectAgenciaTipo; }
            set { _selectAgenciaTipo = value; OnPropertyChanged("SelectAgenciaTipo"); }
        }

        private Estatus _selectedEstatus;
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

        private List<AGENCIA> _listItems;
        public List<AGENCIA> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private AGENCIA _selectedItem;
        public AGENCIA SelectedItem
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

        public class TipoAgenciaClass
        {
            private int _id_tipo_agencia;
            public int Id_tipo_agencia
            {
                get { return _id_tipo_agencia; }
                set { _id_tipo_agencia = value; }
            }

            private string _tipoagencia;
            public string Tipoagencia
            {
                get { return _tipoagencia; }
                set { _tipoagencia = value; }
            }
        }
    }
}
