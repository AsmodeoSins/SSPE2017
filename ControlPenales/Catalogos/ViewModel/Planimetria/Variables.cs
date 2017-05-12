﻿using SSP.Servidor;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Justicia;
using System.Linq;
using ControlPenales.Clases.Estatus;

namespace ControlPenales
{
    partial class CatalogoSectorClasificacionViewModel
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

        cSector obj = new cSector();

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

        private string _observacion;
        public string Observacion
        {
            get { return _observacion; }
            set { _observacion = value; OnPropertyChanged("Observacion"); }
        }

        private string _color;
        public string Color
        {
            get { return _color; }
            set { _color = value; OnPropertyChanged("Color"); }
        }

        private string _colorFont;
        public string ColorFont
        {
            get { return _colorFont; }
            set { _colorFont = value; OnPropertyChanged("ColorFont"); }
        }

        private string _colorPopUp;
        public string ColorPopUp
        {
            get { return _colorPopUp; }
            set { _colorPopUp = value; OnPropertyChanged("ColorPopUp"); }
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

        private bool _grupoVulnerable = false;
        public bool GrupoVulnerable
        {
            get { return _grupoVulnerable; }
            set { _grupoVulnerable = value; OnPropertyChanged("GrupoVulnerable"); }
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

        private bool _nuevoVisible;
        public bool NuevoVisible
        {
            get { return _nuevoVisible; }
            set { _nuevoVisible = value; OnPropertyChanged("NuevoVisible"); }
        }

        private ObservableCollection<SECTOR_CLASIFICACION> _listItems;
        public ObservableCollection<SECTOR_CLASIFICACION> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private ObservableCollection<ControlPenales.Clases.Estatus.Estatus> _listEstatus;
        public ObservableCollection<ControlPenales.Clases.Estatus.Estatus> ListEstatus
        {
            get { return _listEstatus; }
            set { _listEstatus = value; OnPropertyChanged("ListEstatus"); }
        }

        private ControlPenales.Clases.Estatus.Estatus _selectedEstatus = null;
        public ControlPenales.Clases.Estatus.Estatus SelectedEstatus
        {
            get { return _selectedEstatus; }
            set { _selectedEstatus = value; OnPropertyChanged("SelectedEstatus"); }
        }

        private SECTOR_CLASIFICACION _selectedItem;
        public SECTOR_CLASIFICACION SelectedItem
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

        private EstatusControl _lista_estatus = new EstatusControl();
        public EstatusControl Lista_Estatus
        {
            get { return _lista_estatus; }
            set { _lista_estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private short _tipoColor = 1;
        public short TipoColor
        {
            get { return _tipoColor; }
            set { _tipoColor = value; OnPropertyChanged("TipoColor"); }
        }
    }
}