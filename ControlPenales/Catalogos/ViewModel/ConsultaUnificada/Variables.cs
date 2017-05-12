using SSP.Servidor;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Justicia;
using System.Linq;
using ControlPenales.Clases.Estatus;
using System.Collections.Generic;
using System;
using System.Windows;

namespace ControlPenales
{
    partial class ConsultaUnificadaAdminViewModel
    {
        #region Grid
        private ObservableCollection<CONSULTA_UNIFICADA> _listItems;
        public ObservableCollection<CONSULTA_UNIFICADA> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private CONSULTA_UNIFICADA _selectedItem;
        public CONSULTA_UNIFICADA SelectedItem
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
        #endregion

        #region Consulta Unificada
        private string _cuNombre;
        public string CuNombre
        {
            get { return _cuNombre; }
            set { _cuNombre = value; OnPropertyValidateChanged("CuNombre"); }
        }

        private short _cuClasificacion = -1;
        public short CuClasificacion
        {
            get { return _cuClasificacion; }
            set { _cuClasificacion = value; OnPropertyValidateChanged("CuClasificacion"); }
        }

        private bool _cuTipo = false;
        public bool CuTipo
        {
            get { return _cuTipo; }
            set { _cuTipo = value; OnPropertyValidateChanged("CuTipo"); }
        }

        private string _cuEstatus = "A";
        public string CuEstatus
        {
            get { return _cuEstatus; }
            set { _cuEstatus = value; OnPropertyValidateChanged("CuEstatus"); }
        }

        private byte[] _cuDocumentoArchivo;
        public byte[] CuDocumentoArchivo
        {
            get { return _cuDocumentoArchivo; }
            set { _cuDocumentoArchivo = value; OnPropertyValidateChanged("CuDocumentoArchivo"); }
        }

        private byte[] _cuDocumentoScanner;
        public byte[] CuDocumentoScanner
        {
            get { return _cuDocumentoScanner; }
            set { _cuDocumentoScanner = value; OnPropertyValidateChanged("CuDocumentoScanner"); }
        }

        private List<CLASIFICACION_DOCUMENTO> _lstClasificacion;
        public List<CLASIFICACION_DOCUMENTO> LstClasificacion
        {
            get { return _lstClasificacion; }
            set { _lstClasificacion = value; OnPropertyChanged("LstClasificacion"); }
        }
        #endregion

        #region Scanner
        DigitalizarDocumento escaner = new DigitalizarDocumento(Application.Current.Windows[0]);

        public byte[] DocumentoDigitalizado { get; set; }

        private string _observacionDocumento;
        public string ObservacionDocumento
        {
            get { return _observacionDocumento; }
            set { _observacionDocumento = value; OnPropertyChanged("ObservacionDocumento"); }
        }

        private DateTime? _datePickCapturaDocumento = Fechas.GetFechaDateServer;
        public DateTime? DatePickCapturaDocumento
        {
            get { return _datePickCapturaDocumento; }
            set { _datePickCapturaDocumento = value; OnPropertyChanged("DatePickCapturaDocumento"); }
        }

        private bool _isObservacionesEscanerEnabled = false;
        public bool IsObservacionesEscanerEnabled
        {
            get { return _isObservacionesEscanerEnabled; }
            set { _isObservacionesEscanerEnabled = value; OnPropertyChanged("IsObservacionesEscanerEnabled"); }
        }

        private TipoDocumento _selectedTipoDocumento;
        public TipoDocumento SelectedTipoDocumento
        {
            get { return _selectedTipoDocumento; }
            set
            {
                DocumentoDigitalizado = null;
                ObservacionDocumento = string.Empty;
                _selectedTipoDocumento = value;
                OnPropertyChanged("SelectedTipoDocumento");
            }
        }

        private ObservableCollection<TipoDocumento> _listTipoDocumento;
        public ObservableCollection<TipoDocumento> ListTipoDocumento
        {
            get { return _listTipoDocumento; }
            set { _listTipoDocumento = value; OnPropertyChanged("ListTipoDocumento"); }
        }

        private bool _autoGuardado = true;
        public bool AutoGuardado
        {
            get { return _autoGuardado; }
            set { _autoGuardado = value; OnPropertyChanged("AutoGuardado"); }
        }

        private bool _Duplex = true;
        public bool Duplex
        {
            get { return _Duplex; }
            set
            {
                _Duplex = value;
                OnPropertyChanged("Duplex");
            }
        }

        private EscanerSources selectedSource = null;
        public EscanerSources SelectedSource
        {
            get { return selectedSource; }
            set { selectedSource = value; RaisePropertyChanged("SelectedSource"); }
        }

        private List<EscanerSources> lista_Sources = null;
        public List<EscanerSources> Lista_Sources
        {
            get { return lista_Sources; }
            set { lista_Sources = value; RaisePropertyChanged("Lista_Sources"); }
        }

        private string hojasMaximo;
        public string HojasMaximo
        {
            get { return hojasMaximo; }
            set { hojasMaximo = value; RaisePropertyChanged("HojasMaximo"); }
        }
        #endregion

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

        private EstatusControl _lista_estatus = new EstatusControl();
        public EstatusControl Lista_Estatus
        {
            get { return _lista_estatus; }
            set { _lista_estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private bool _nuevo;
        public bool Nuevo
        {
            get { return _nuevo; }
            set { _nuevo = value; OnPropertyChanged("Nuevo"); }
        }
    }
}