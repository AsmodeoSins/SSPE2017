using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases.ControlInternos;
using ControlPenales.Clases.ControlProgramas;
using SSP.Controlador.Catalogo.Justicia.Actividades;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace ControlPenales
{
    partial class ControlInternosViewModel
    {
        public string Name
        {
            get { return "control_internos"; }
        }

        #region [VARIABLES]
        private DateTime FechaActualizacion;
        private string hora;
        private string minuto;
        private string segundo;
        private DispatcherTimer timer;
        cIngresoUbicacion ingreso_ubicacion = new cIngresoUbicacion();
        List<InternosRequeridos> internosSelect = new List<InternosRequeridos>();
        #endregion

        #region [SELECCION REQUERIDOS]
        private List<InternosRequeridos> LstInternosSeleccionados;
        #endregion

        #region [SELECCION AUSENTES]
        private List<InternosAusentes> LstInternosSeleccionadosAusentes;
        #endregion

        #region [PROPIEDADES PARA EL MENU]
        private bool _menuGuardarEnabled;
        public bool MenuGuardarEnabled
        {
            get { return _menuGuardarEnabled; }
            set
            {
                _menuGuardarEnabled = value;
                OnPropertyChanged("MenuGuardarEnabled");
            }
        }

        private bool _menuLimpiarEnabled = true;
        public bool MenuLimpiarEnabled
        {
            get { return _menuLimpiarEnabled; }
            set
            {
                _menuLimpiarEnabled = value;
                OnPropertyChanged("MenuLimpiarEnabled");
            }
        }

        private bool _menuSalirEnabled = true;
        public bool MenuSalirEnabled
        {
            get { return _menuSalirEnabled; }
            set
            {
                _menuSalirEnabled = value;
                OnPropertyChanged("MenuSalirEnabled");
            }
        }

        private bool _menuAyudaEnabled = true;
        public bool MenuAyudaEnabled
        {
            get { return _menuAyudaEnabled; }
            set
            {
                _menuAyudaEnabled = value;
                OnPropertyChanged("MenuAyudaEnabled");
            }
        }
        #endregion

        private int _seleccionIndice;
        public int SeleccionIndice
        {
            get { return _seleccionIndice; }
            set
            {
                _seleccionIndice = value;
                OnPropertyChanged("SeleccionIndice");
            }
        }

        private string _cambio;
        public string Cambio
        {
            get { return _cambio; }
            set
            {
                _cambio = value;
                OnPropertyChanged("Cambio");
            }
        }

        private List<EDIFICIO> _listaEdificio;
        public List<EDIFICIO> ListaEdificio
        {
            get { return _listaEdificio; }
            set
            {
                _listaEdificio = value;
                OnPropertyChanged("ListaEdificio");
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
                LlenarSector();
            }
        }

        private List<SECTOR> _listaSector;
        public List<SECTOR> ListaSector
        {
            get { return _listaSector; }
            set
            {
                _listaSector = value;
                OnPropertyChanged("ListaSector");
            }
        }

        private SECTOR _selectedSector;
        public SECTOR SelectedSector
        {
            get { return _selectedSector; }
            set
            {
                _selectedSector = value;
                OnPropertyChanged("SelectedSector");
            }
        }

        //LISTA QUE CONTIENE LOS INTERNOS REQUERIDOS DEL EDIFICIO-ACTIVIDAD
        private List<InternosRequeridos> _listaInternosRequeridos = new List<InternosRequeridos>();
        public List<InternosRequeridos> ListaInternosRequeridos
        {
            get { return _listaInternosRequeridos; }
            set
            {
                _listaInternosRequeridos = value;
                OnPropertyChanged("ListaInternosRequeridos");
            }
        }

        private int _countList;
        public int CountList
        {
            get { return _countList; }
            set
            {
                _countList = value;
                OnPropertyChanged("CountList");
            }
        }

        private InternosRequeridos _selectedInternoRequerido;
        public InternosRequeridos SelectedInternoRequerido
        {
            get { return _selectedInternoRequerido; }
            set
            {
                _selectedInternoRequerido = value;
                OnPropertyChanged("SelectedInternoRequerido");
            }
        }

        //LISTA NECESARIA PARA CHECKBOX EN INTERNOS AUSENTES
        private List<InternosAusentes> _listaInternosAusentes = new List<InternosAusentes>();
        public List<InternosAusentes> ListaInternosAusentes
        {
            get { return _listaInternosAusentes; }
            set
            {
                _listaInternosAusentes = value;
                OnPropertyChanged("ListaInternosAusentes");
            }
        }

        //LISTA QUE CONTIENE INTERNOS CON ENTRADA AL EDIFICIO
        private List<InternosAusentes> _listaAusenteEntrada = new List<InternosAusentes>();
        public List<InternosAusentes> ListaAusenteEntrada
        {
            get { return _listaAusenteEntrada; }
            set
            {
                _listaAusenteEntrada = value;
                OnPropertyChanged("ListaAusenteEntrada");
            }
        }

        //LISTA QUE CONTIENE LOS INTERNOS QUE ESTAN FUERA DEL EDIFICIO
        private ObservableCollection<InternosAusentes> _listaInternosSeleccionados;
        public ObservableCollection<InternosAusentes> ListaInternosSeleccionados
        {
            get { return _listaInternosSeleccionados; }
            set
            {
                _listaInternosSeleccionados = value;
                OnPropertyChanged("ListaInternosSeleccionados");
            }
        }

        private InternosAusentes _selectInternosSeleccionados;
        public InternosAusentes SelectInternosSeleccionados
        {
            get { return _selectInternosSeleccionados; }
            set
            {
                _selectInternosSeleccionados = value;
                OnPropertyChanged("SelectInternosSeleccionados");
            }
        }

        //LISTA QUE CONTIENE LAS ACTIVIDADES DEL DIA DE CADA INTERNO
        private ObservableCollection<InternosAusentes> _listaActividadesInternos;
        public ObservableCollection<InternosAusentes> ListaActividadesInternos
        {
            get { return _listaActividadesInternos; }
            set
            {
                _listaActividadesInternos = value;
                OnPropertyChanged("ListaActividadesInternos");
            }
        }

        //HUELLAS En LeerInternosEdificioViewModel
        IList<PlantillaBiometrico> HuellasCapturadas;

        private RangeEnabledObservableCollection<IMPUTADO> _listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return _listExpediente; }
            set
            {
                _listExpediente = value;
                OnPropertyChanged("ListExpediente");
            }
        }

        private DateTime? _fechaInicio;
        public DateTime? FechaInicio
        {
            get { return _fechaInicio; }
            set
            {
                _fechaInicio = value;
                OnPropertyChanged("FechaInicio");
            }
        }

        private DateTime? _fechaFin;
        public DateTime? FechaFin
        {
            get { return _fechaFin; }
            set
            {
                _fechaFin = value;
                OnPropertyChanged("FechaFin");
            }
        }

        private string _actualizacion;
        public string Actualizacion
        {
            get { return _actualizacion; }
            set
            {
                if (value != null)
                {
                    _actualizacion = value;
                    OnPropertyChanged("Actualizacion");
                }
            }
        }

        private bool _tipoRegistroEnabled;
        public bool TipoRegistroEnabled
        {
            get { return _tipoRegistroEnabled; }
            set
            {
                _tipoRegistroEnabled = value;
                OnPropertyChanged("TipoRegistroEnabled");
            }
        }

        private bool _tipoRegistroChecked = true;
        public bool TipoRegistroChecked
        {
            get { return _tipoRegistroChecked; }
            set
            {
                _tipoRegistroChecked = value;
                RaisePropertyChanged("TipoRegistroChecked");
            }
        }

        private bool _autorizarBtnEnabled = false;
        public bool AutorizarBtnEnabled
        {
            get { return _autorizarBtnEnabled; }
            set
            {
                _autorizarBtnEnabled = value;
                OnPropertyChanged("AutorizarBtnEnabled");
            }
        }

        private bool _enrolarInternosEnabled;
        public bool EnrolarInternosEnabled
        {
            get { return _enrolarInternosEnabled; }
            set
            {
                _enrolarInternosEnabled = value;
                OnPropertyChanged("EnrolarInternosEnabled");
            }
        }

        private bool BanderaSelect = false;

        private bool _seleccionarTodoInternos;
        public bool SeleccionarTodoInternos
        {
            get { return _seleccionarTodoInternos; }
            set
            {
                _seleccionarTodoInternos = value;
                OnPropertyChanged("SeleccionarTodoInternos");
            }
        }

        private bool _custodioVisible;
        public bool CustodioVisible
        {
            get { return _custodioVisible; }
            set
            {
                _custodioVisible = value;
                OnPropertyChanged("CustodioVisible");
            }
        }

        private string _tipoTexto = "Salida";
        public string TipoTexto
        {
            get { return _tipoTexto; }
            set
            {
                _tipoTexto = value;
                OnPropertyChanged("TipoTexto");
            }
        }

        private bool _selectAusente;
        public bool SelectAusente
        {
            get { return _selectAusente; }
            set
            {
                if (value == true)
                {
                    LstInternosSeleccionados = new List<InternosRequeridos>();
                    InternosSeleccionados = string.Format("Internos Seleccionados: {0}", LstInternosSeleccionados.Count);
                    TextoActividadVisible = Visibility.Visible;
                    ModoAlternativo = false;
                    ModoAlternativoHabilitado = true;
                    AutorizarBtnEnabled = true;
                    AsistenciaHabilitado = true;
                    TipoTexto = "Entrada";
                    EdificioHablititado = false;
                    SectorHabilitado = false;
                    TodosHabilitado = false;
                    FechaInicioHabilitado = false;
                    FechaFinalHabilitado = false;
                    BuscarHabilitado = false;
                    ToggleHabilitado = true;
                    TipoRegistroChecked = true;
                    TipoRegistroEnabled = false;
                    VisibleTipoRegistro = Visibility.Visible;
                    EnrolarInternosEnabled = false;
                    NombreBuscar = string.Empty;
                    ApellidoMaternoBuscar = string.Empty;
                    ApellidoPaternoBuscar = string.Empty;
                    FolioBuscar = null;
                    AsistenciaHabilitado = true;
                    ImagenPlaceHolder = new Imagenes();
                    ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
                    TotalInternos = string.Format("Total de Internos: {0}", ListaInternosSeleccionados.Count);
                }
                _selectAusente = value;
                OnPropertyChanged("SelectAusente");
            }
        }

        private bool _selectRequerido;
        public bool SelectRequerido
        {
            get { return _selectRequerido; }
            set
            {
                if (value == true)
                {
                    if (TabRequeridoHabilitado)
                    {
                        LstInternosSeleccionadosAusentes = new List<InternosAusentes>();
                        TextoActividadVisible = Visibility.Collapsed;
                        AutorizarBtnEnabled = true;
                        AsistenciaHabilitado = true;
                        TipoTexto = "Salida";
                        CustodioVisible = false;
                        EdificioHablititado = true;
                        SectorHabilitado = true;
                        TodosHabilitado = true;
                        FechaInicioHabilitado = true;
                        FechaFinalHabilitado = true;
                        BuscarHabilitado = true;
                        ToggleHabilitado = true;
                        ModoAlternativo = false;
                        TipoRegistroChecked = true;
                        TipoRegistroEnabled = false;
                        VisibleTipoRegistro = Visibility.Visible;
                        this.Buscar();
                        EnrolarInternosEnabled = false;
                        NombreBuscar = string.Empty;
                        ApellidoMaternoBuscar = string.Empty;
                        ApellidoPaternoBuscar = string.Empty;
                        FolioBuscar = null;
                        ImagenPlaceHolder = new Imagenes();
                        ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
                    }
                }
                _selectRequerido = value;
                OnPropertyChanged("SelectRequerido");
            }
        }

        private Visibility _visibleTipoRegistro;
        public Visibility VisibleTipoRegistro
        {
            get { return _visibleTipoRegistro; }
            set
            {
                _visibleTipoRegistro = value;
                OnPropertyChanged("VisibleTipoRegistro");
            }
        }

        #region [CONFIGURACION PERMISOS]
        private bool _edificioHabilitado;
        public bool EdificioHablititado
        {
            get { return _edificioHabilitado; }
            set
            {
                _edificioHabilitado = value;
                OnPropertyChanged("EdificioHablititado");
            }
        }

        private bool _fechaInicioHabilitado;
        public bool FechaInicioHabilitado
        {
            get { return _fechaInicioHabilitado; }
            set
            {
                _fechaInicioHabilitado = value;
                OnPropertyChanged("FechaInicioHabilitado");
            }
        }

        private bool _fechaFinalHabilitado;
        public bool FechaFinalHabilitado
        {
            get { return _fechaFinalHabilitado; }
            set
            {
                _fechaFinalHabilitado = value;
                OnPropertyChanged("FechaFinalHabilitado");
            }
        }

        private bool _buscarHabilitado;
        public bool BuscarHabilitado
        {
            get { return _buscarHabilitado; }
            set
            {
                _buscarHabilitado = value;
                OnPropertyChanged("BuscarHabilitado");
            }
        }

        private bool _tabRequeridoHabilitado;
        public bool TabRequeridoHabilitado
        {
            get { return _tabRequeridoHabilitado; }
            set
            {
                _tabRequeridoHabilitado = value;
                OnPropertyChanged("TabRequeridoHabilitado");
            }
        }

        private bool _tabAusenteHabilitado;
        public bool TabAusenteHabilitado
        {
            get { return _tabAusenteHabilitado; }
            set
            {
                _tabAusenteHabilitado = value;
                OnPropertyChanged("TabAusenteHabilitado");
            }
        }

        private bool _sectorHabilitado;
        public bool SectorHabilitado
        {
            get { return _sectorHabilitado; }
            set
            {
                _sectorHabilitado = value;
                OnPropertyChanged("SectorHabilitado");
            }
        }
        #endregion

        private bool _todosHabilitado;
        public bool TodosHabilitado
        {
            get { return _todosHabilitado; }
            set
            {
                _todosHabilitado = value;
                OnPropertyChanged("TodosHabilitado");
            }
        }

        private Visibility _custodioHuellaVisible = Visibility.Collapsed;
        public Visibility CustodioHuellaVisible 
        {
            get { return _custodioHuellaVisible; }
            set
            {
                _custodioHuellaVisible = value;
                OnPropertyChanged("CustodioHuellaVisible"); 
            }
        }

        private bool _modoAlternativo = false;
        public bool ModoAlternativo
        {
            get { return _modoAlternativo; }
            set
            {
                _modoAlternativo = value;
                OnPropertyChanged("ModoAlternativo");

                if (SelectRequerido)
                {
                    if (ModoAlternativo)
                    {
                        if (LstInternosSeleccionados != null)
                        {
                            if (LstInternosSeleccionados.Count() > 0)
                            {
                                StaticSourcesViewModel.Mensaje("NOTA", "La operación solicitada no se puede realizar, primero se autentifica el custodio y después se agregan internos", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                                _modoAlternativo = false;
                            }
                            else
                            {
                                EnrolarInternosEnabled = false;
                                NombreBuscar = string.Empty;
                                ApellidoMaternoBuscar = string.Empty;
                                ApellidoPaternoBuscar = string.Empty;
                                FolioBuscar = null;
                                AsistenciaHabilitado = false;
                                AutorizarBtnEnabled = false;
                                ImagenPlaceHolder = new Imagenes();
                                ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
                                CustodioVisible = true;
                                CustodioHuellaVisible = Visibility.Visible;
                                AutorizarBtnEnabled = false;
                                AsistenciaHabilitado = false;
                            }
                        }
                        else
                        {
                            CustodioVisible = true;
                            CustodioHuellaVisible = Visibility.Visible;
                            AutorizarBtnEnabled = false;
                            AsistenciaHabilitado = false;
                        }
                    }
                    else
                    {
                        AsistenciaHabilitado = true;
                        AutorizarBtnEnabled = true;
                        CustodioVisible = false;
                        CustodioHuellaVisible = Visibility.Collapsed;
                        Custodio = null;
                    }
                }
                else if (SelectAusente)
                {
                    if (_modoAlternativo)
                    {
                        if (LstInternosSeleccionadosAusentes != null)
                        {
                            if (LstInternosSeleccionadosAusentes.Count() > 0)
                            {
                                StaticSourcesViewModel.Mensaje("NOTA", "La operación solicitada no se puede realizar, primero se autentifica el custodio y después se agregan internos", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR, 5);
                                _modoAlternativo = false;
                            }
                            else
                            {
                                EnrolarInternosEnabled = false;
                                NombreBuscar = string.Empty;
                                ApellidoMaternoBuscar = string.Empty;
                                ApellidoPaternoBuscar = string.Empty;
                                FolioBuscar = null;
                                AsistenciaHabilitado = false;
                                AutorizarBtnEnabled = false;
                                ImagenPlaceHolder = new Imagenes();
                                ImagenCustodio = ImagenPlaceHolder.getImagenPerson();
                                CustodioVisible = true;
                                AutorizarBtnEnabled = false;
                                AsistenciaHabilitado = false;
                            }
                        }
                        else
                        {
                            CustodioVisible = true;
                            AutorizarBtnEnabled = false;
                            AsistenciaHabilitado = false;
                        }
                    }
                    else
                    {
                        AsistenciaHabilitado = true;
                        AutorizarBtnEnabled = true;
                        CustodioVisible = false;
                    }
                }
            }
        }

        private bool _modoAlternativoHabilitado = false;
        public bool ModoAlternativoHabilitado
        {
            get { return _modoAlternativoHabilitado; }
            set
            {
                _modoAlternativoHabilitado = value;
                OnPropertyChanged("ModoAlternativoHabilitado");
            }
        }

        private bool _toggleHabilitado = true;
        public bool ToggleHabilitado
        {
            get { return _toggleHabilitado; }
            set
            {
                _toggleHabilitado = value;
                OnPropertyChanged("ToggleHabilitado");
            }
        }

        private bool _asistenciaHabilitado = false;
        public bool AsistenciaHabilitado
        {
            get { return _asistenciaHabilitado; }
            set
            {
                _asistenciaHabilitado = value;
                OnPropertyChanged("AsistenciaHabilitado");
            }
        }

        private V_AGENDA _selectedItem;
        public V_AGENDA SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        //INTERNO
        private IMPUTADO _imputado;
        public IMPUTADO Imputado
        {
            get { return _imputado; }
            set
            {
                _imputado = value;
                OnPropertyChanged("Imputado");
            }
        }

        //CUSTODIO
        private SSP.Servidor.PERSONA _custodio;
        public SSP.Servidor.PERSONA Custodio
        {
            get { return _custodio; }
            set
            {
                _custodio = value;
                OnPropertyChanged("Custodio");
            }
        }

        private string _apellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return _apellidoPaternoBuscar; }
            set
            {
                _apellidoPaternoBuscar = value;
                OnPropertyChanged("ApellidoPaternoBuscar");
            }
        }

        private string _apellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return _apellidoMaternoBuscar; }
            set
            {
                _apellidoMaternoBuscar = value;
                OnPropertyChanged("ApellidoMaternoBuscar");
            }
        }

        private string _nombreBuscar;
        public string NombreBuscar
        {
            get { return _nombreBuscar; }
            set
            {
                _nombreBuscar = value;
                OnPropertyChanged("NombreBuscar");
            }
        }

        private int? _anioBuscar;
        public int? AnioBuscar
        {
            get { return _anioBuscar; }
            set
            {
                _anioBuscar = value;
                OnPropertyChanged("AnioBuscar");
            }
        }

        private int? _folioBuscar;
        public int? FolioBuscar
        {
            get { return _folioBuscar; }
            set
            {
                _folioBuscar = value;
                OnPropertyChanged("FolioBuscar");
            }
        }

        private bool _emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return _emptyExpedienteVisible; }
            set
            {
                _emptyExpedienteVisible = value;
                OnPropertyChanged("EmptyExpedienteVisible");
            }
        }

        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        private string _totalInternos;
        public string TotalInternos
        {
            get { return _totalInternos; }
            set
            {
                if (value != null)
                {
                    _totalInternos = value;
                    OnPropertyChanged("TotalInternos");
                }
            }
        }

        private string _internosSeleccionados;
        public string InternosSeleccionados
        {
            get { return _internosSeleccionados; }
            set
            {
                if (value != null)
                {
                    _internosSeleccionados = value;
                    OnPropertyChanged("InternosSeleccionados");
                }
            }
        }

        #region[LeerInternosEdificioViewModel]
        private string _busquedaInterno;
        public string BusquedaInterno
        {
            get { return _busquedaInterno; }
            set
            {
                _busquedaInterno = value;
                OnPropertyChanged("BusquedaInterno");
            }
        }

        private bool _progressRingVisible;
        public bool ProgressVisible
        {
            get { return _progressRingVisible; }
            set
            {
                _progressRingVisible = value;
                OnPropertyChanged("ProgressVisible");
            }
        }

        private bool _asistenciaNIPSelect;
        public bool AsistenciaNIPSelect
        {
            get { return _asistenciaNIPSelect; }
            set
            {
                _asistenciaNIPSelect = value;
                OnPropertyChanged("AsistenciaNIPSelect");
            }
        }

        private bool _asistenciaBiometricaDeshabilitado;
        public bool AsistenciaBiometricaDeshabilitado
        {
            get { return _asistenciaBiometricaDeshabilitado; }
            set
            {
                _asistenciaBiometricaDeshabilitado = value;
                OnPropertyChanged("AsistenciaBiometricaDeshabilitado");
            }
        }

        private bool _asistenciaBiometricaSelect;
        public bool AsistenciaBiometricaSelect
        {
            get { return _asistenciaBiometricaSelect; }
            set
            {
                _asistenciaBiometricaSelect = value;
                OnPropertyChanged("AsistenciaBiometricaSelect");
            }
        }

        private bool _aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return _aceptarBusquedaHuellaFocus; }
            set
            {
                _aceptarBusquedaHuellaFocus = value;
                OnPropertyChanged("AceptarBusquedaHuellaFocus");
            }
        }

        private IMPUTADO _selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return _selectExpediente; }
            set
            {
                _selectExpediente = value;
                if (_selectExpediente != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (_selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = _selectExpediente.INGRESO.OrderBy(o => o.FEC_INGRESO_CERESO).FirstOrDefault();
                    }
                    else
                        EmptyIngresoVisible = true;

                    //OBTENEMOS FOTO DE FRENTE
                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        else
                            ImagenImputado = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenImputado = new Imagenes().getImagenPerson();
                }
                else
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    EmptyIngresoVisible = true;
                }
                OnPropertyChanged("SelectExpediente");
            }
        }

        private INGRESO _selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return _selectIngreso; }
            set
            {
                _selectIngreso = value;
                if (_selectIngreso == null)
                    return;
                if (_selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (_selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = _selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    OnPropertyChanged("SelectIngreso");
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
            }
        }

        private byte[] _imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return _imagenImputado; }
            set
            {
                _imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
            }
        }

        private byte[] _imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return _imagenIngreso; }
            set
            {
                _imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
            }
        }

        private bool _emptyIngresoVisible = true;
        public bool EmptyIngresoVisible
        {
            get { return _emptyIngresoVisible; }
            set
            {
                _emptyIngresoVisible = value;
                OnPropertyChanged("EmptyIngresoVisible");
            }
        }

        private Visibility _showImagenCustodio;
        public Visibility ShowImagenCustodio
        {
            get { return _showImagenCustodio; }
            set
            {
                _showImagenCustodio = value;
                OnPropertyChanged("ShowImagenCustodio");
            }
        }

        private byte[] _imagenCustodio;
        public byte[] ImagenCustodio
        {
            get { return _imagenCustodio; }
            set
            {
                _imagenCustodio = value;
                OnPropertyChanged("ImagenCustodio");
            }
        }

        public Imagenes ImagenPlaceHolder { get; set; }

        private Visibility _textoActividadVisible = Visibility.Collapsed;
        public Visibility TextoActividadVisible
        {
            get { return _textoActividadVisible; }
            set
            {
                _textoActividadVisible = value;
                OnPropertyChanged("TextoActividadVisible");
            }
        }
        #endregion
    }
}
