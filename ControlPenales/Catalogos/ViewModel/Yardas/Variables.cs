using SSP.Servidor;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Justicia;
using System.Linq;
using ControlPenales.Clases.Estatus;
using System.Collections.Generic;

namespace ControlPenales
{
    partial class YardasViewModel
    {
        #region Grid
        private ObservableCollection<YARDA> _listItems;
        public ObservableCollection<YARDA> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private YARDA _selectedItem;
        public YARDA SelectedItem
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

        #region Yarda
        private ObservableCollection<EDIFICIO> _lstEdificio;
        public ObservableCollection<EDIFICIO> LstEdificio
        {
            get { return _lstEdificio; }
            set { _lstEdificio = value; OnPropertyChanged("LstEdificio"); }
        }

        private EDIFICIO _selectedEdificio;
        public EDIFICIO SelectedEdificio
        {
            get { return _selectedEdificio; }
            set
            {
                _selectedEdificio = value;
                if (value != null)
                    LstSector = new ObservableCollection<SECTOR>(value.SECTOR);
                else
                    LstSector = new ObservableCollection<SECTOR>();
                LstSector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
                YSector = -1;
                OnPropertyValidateChanged("SelectedEdificio");
            }
        }

        private ObservableCollection<SECTOR> _lstSector;
        public ObservableCollection<SECTOR> LstSector
        {
            get { return _lstSector; }
            set { _lstSector = value; OnPropertyChanged("LstSector"); }
        }

        private SECTOR _selectedSector;
        public SECTOR SelectedSector
        {
            get { return _selectedSector; }
            set
            {
                _selectedSector = value;
                //if (value != null)
                //    LstCelda = new ObservableCollection<CELDA>(value.CELDA);
                //else
                //    LstCelda = new ObservableCollection<CELDA>();
                //LstCelda.Insert(0, new CELDA() { ID_CELDA = "SELECCIONE" });
                //yCeldaInicioS = YCeldaFinS = "SELECCIONE";
                OnPropertyValidateChanged("SelectedSector");
            }
        }

        private ObservableCollection<CELDA> _lstCelda;
        public ObservableCollection<CELDA> LstCelda
        {
            get { return _lstCelda; }
            set { _lstCelda = value; OnPropertyChanged("LstCelda"); }
        }

        private short _yEdificio = -1;
        public short YEdificio
        {
            get { return _yEdificio; }
            set { _yEdificio = value; OnPropertyValidateChanged("YEdificio"); }
        }

        private short _ySector = -1;
        public short YSector
        {
            get { return _ySector; }
            set { _ySector = value; OnPropertyValidateChanged("YSector"); }
        }

        private string _yCeldaInicioS = "SELECCIONE";
        public string YCeldaInicioS
        {
            get { return _yCeldaInicioS; }
            set
            {
                _yCeldaInicioS = value;
                if (value != "SELECCIONE")
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        YCeldaInicio = short.Parse(value);
                    }
                    else
                        YCeldaInicio = null;
                }
                else
                {
                    YCeldaInicio = null;
                }
                OnPropertyChanged("YCeldaInicioS");
            }
        }

        private string _yCeldaFinS = "SELECCIONE";
        public string YCeldaFinS
        {
            get { return _yCeldaFinS; }
            set
            {
                _yCeldaFinS = value;
                if (value != "SELECCIONE")
                {
                    if (!string.IsNullOrEmpty(value))
                        YCeldaFin = short.Parse(value);
                    else
                        YCeldaFin = null;
                }
                else
                {
                    YCeldaFin = null;
                }
                OnPropertyChanged("YCeldaFinS");
            }
        }

        private short? _yCeldaInicio;
        public short? YCeldaInicio
        {
            get { return _yCeldaInicio; }
            set
            {
                _yCeldaInicio = value;
                setValidationRules();
                OnPropertyValidateChanged("YCeldaInicio");
            }
        }

        private short? _yCeldaFin;
        public short? YCeldaFin
        {
            get { return _yCeldaFin; }
            set
            {
                _yCeldaFin = value;
                setValidationRules();
                OnPropertyValidateChanged("YCeldaFin");
            }
        }

        private short _yDiaSemana = -1;
        public short YDiaSemana
        {
            get { return _yDiaSemana; }
            set { _yDiaSemana = value; OnPropertyValidateChanged("YDiaSemana"); }
        }

        private short? _yHoraInicio;
        public short? YHoraInicio
        {
            get { return _yHoraInicio; }
            set
            {
                _yHoraInicio = value;
                setValidationRules();
                OnPropertyValidateChanged("YHoraInicio");
            }
        }

        private short? _yMinInicio;
        public short? YMinInicio
        {
            get { return _yMinInicio; }
            set
            {
                _yMinInicio = value;
                setValidationRules();
                OnPropertyValidateChanged("YMinInicio");
            }
        }

        private short? _yHoraFin;
        public short? YHoraFin
        {
            get { return _yHoraFin; }
            set
            {
                _yHoraFin = value;
                setValidationRules();
                OnPropertyValidateChanged("YHoraFin");
            }
        }

        private short? _yMinFin;
        public short? YMinFin
        {
            get { return _yMinFin; }
            set
            {
                _yMinFin = value;
                setValidationRules();
                OnPropertyValidateChanged("YMinFin");
            }
        }

        private string _yEstatus = "S";
        public string YEstatus
        {
            get { return _yEstatus; }
            set { _yEstatus = value; OnPropertyValidateChanged("YEstatus"); }
        }

        private ObservableCollection<AREA> _lstArea;
        public ObservableCollection<AREA> LstArea
        {
            get { return _lstArea; }
            set { _lstArea = value; OnPropertyChanged("LstArea"); }
        }

        private short _yArea = -1;
        public short YArea
        {
            get { return _yArea; }
            set { _yArea = value; OnPropertyValidateChanged("YArea"); }
        }

        private bool _yInsert = true;
        public bool YInsert
        {
            get { return _yInsert; }
            set { _yInsert = value; OnPropertyChanged("YInsert"); }
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
    }
}