using System.Linq;
namespace ControlPenales
{
    public partial class HojaControlLiquidosViewModel
    {
        #region Datos Generales

        private SSP.Servidor.IMPUTADO selectedInterno;
        public SSP.Servidor.IMPUTADO SelectedInterno
        {
            get { return selectedInterno; }
            set { selectedInterno = value; OnPropertyChanged("SelectedInterno"); }
        }
        private int? anioD;
        public int? AnioD
        {
            get { return anioD; }
            set { anioD = value; OnPropertyChanged("AnioD"); }
        }
        private int? folioD;
        public int? FolioD
        {
            get { return folioD; }
            set { folioD = value; OnPropertyChanged("FolioD"); }
        }
        private string paternoD;
        public string PaternoD
        {
            get { return paternoD; }
            set { paternoD = value; OnPropertyChanged("PaternoD"); }
        }
        private string maternoD;
        public string MaternoD
        {
            get { return maternoD; }
            set { maternoD = value; OnPropertyChanged("MaternoD"); }
        }
        private string nombreD;
        public string NombreD
        {
            get { return nombreD; }
            set { nombreD = value; OnPropertyChanged("NombreD"); }
        }

        private string _SexoImp;

        public string SexoImp
        {
            get { return _SexoImp; }
            set { _SexoImp = value; OnPropertyChanged("SexoImp"); }
        }

        private string _EdadImp;

        public string EdadImp
        {
            get { return _EdadImp; }
            set { _EdadImp = value; OnPropertyChanged("EdadImp"); }
        }

        private string _DiagnosticoImp;

        public string DiagnosticoImp
        {
            get { return _DiagnosticoImp; }
            set { _DiagnosticoImp = value; OnPropertyChanged("DiagnosticoImp"); }
        }

        private string _CamaImp;

        public string CamaImp
        {
            get { return _CamaImp; }
            set { _CamaImp = value; OnPropertyChanged("CamaImp"); }
        }

        private string _DietaImp;

        public string DietaImp
        {
            get { return _DietaImp; }
            set { _DietaImp = value; OnPropertyChanged("DietaImp"); }
        }

        private string _PesoImp;

        public string PesoImp
        {
            get { return _PesoImp; }
            set { _PesoImp = value; OnPropertyChanged("PesoImp"); }
        }

        private string _TallaImp;

        public string TallaImp
        {
            get { return _TallaImp; }
            set { _TallaImp = value; OnPropertyChanged("TallaImp"); }
        }


        #endregion

        #region BUSQUEDA_IMPUTADO
        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private bool crearNuevoExpedienteEnabled;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }

        private string apellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return apellidoPaternoBuscar; }
            set { apellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
        }

        private string apellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return apellidoMaternoBuscar; }
            set { apellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscar"); }
        }

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set
            {
                nombreBuscar = value; OnPropertyChanged("NombreBuscar");
            }
        }

        private int? anioBuscar;
        public int? AnioBuscar
        {
            get { return anioBuscar; }
            set { anioBuscar = value; OnPropertyChanged("AnioBuscar"); }
        }

        private int? folioBuscar;
        public int? FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value; OnPropertyChanged("FolioBuscar"); }
        }

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set
            {
                imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
            }
        }

        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set
            {
                imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
            }
        }

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private bool emptyIngresoVisible = true;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                OnPropertyChanged("ListExpediente");
            }
        }

        private SSP.Servidor.IMPUTADO InputadoInterno { get; set; }

        private SSP.Servidor.IMPUTADO selectExpediente;
        public SSP.Servidor.IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    if (selectExpediente.INGRESO!=null && selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = selectExpediente.INGRESO.OrderBy(o => o.FEC_INGRESO_CERESO).FirstOrDefault();
                    }
                    else
                        EmptyIngresoVisible = true;

                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).Any())
                            ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
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

        private SSP.Servidor.INGRESO selectIngreso;
        public SSP.Servidor.INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                {
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                    OnPropertyChanged("SelectIngreso");
                    return;
                }
                if (selectIngreso.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                    SelectIngresoEnabled = true;
                else
                    SelectIngresoEnabled = false;
                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectIngreso");
            }
        }

        private bool selectIngresoEnabled;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }

        private SSP.Servidor.INGRESO selectedIngreso;
        public SSP.Servidor.INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value; OnPropertyChanged("SelectedIngreso"); }
        }

        private bool _ElementosDisponibles = true;
        public bool ElementosDisponibles
        {
            get { return _ElementosDisponibles; }
            set { _ElementosDisponibles = value; OnPropertyChanged("ElementosDisponibles"); }
        }

        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        private int? _TextAnioImputado;
        public int? TextAnioImputado
        {
            get { return _TextAnioImputado; }
            set { _TextAnioImputado = value; OnPropertyChanged("TextAnioImputado"); }
        }
        private int? _TextFolioImputado;
        public int? TextFolioImputado
        {
            get { return _TextFolioImputado; }
            set { _TextFolioImputado = value; OnPropertyChanged("TextFolioImputado"); }
        }
        private string _TextPaternoImputado;
        public string TextPaternoImputado
        {
            get { return _TextPaternoImputado; }
            set { _TextPaternoImputado = value; OnPropertyChanged("TextPaternoImputado"); }
        }
        private string _TextMaternoImputado;
        public string TextMaternoImputado
        {
            get { return _TextMaternoImputado; }
            set { _TextMaternoImputado = value; OnPropertyChanged("TextMaternoImputado"); }
        }
        private string _TextNombreImputado;
        public string TextNombreImputado
        {
            get { return _TextNombreImputado; }
            set { _TextNombreImputado = value; OnPropertyChanged("TextNombreImputado"); }
        }
        private byte[] _FotoIngreso = new Imagenes().getImagenPerson();
        public byte[] FotoIngreso
        {
            get { return _FotoIngreso; }
            set { _FotoIngreso = value; OnPropertyChanged("FotoIngreso"); }
        }
        private string _TextEdad;
        public string TextEdad
        {
            get { return _TextEdad; }
            set { _TextEdad = value; OnPropertyChanged("TextEdad"); }
        }
        private string _SelectSexo;
        public string SelectSexo
        {
            get { return _SelectSexo; }
            set { _SelectSexo = value; OnPropertyChanged("SelectSexo"); }
        }
        private string _SelectFechaNacimiento;
        public string SelectFechaNacimiento
        {
            get { return _SelectFechaNacimiento; }
            set { _SelectFechaNacimiento = value; OnPropertyChanged("SelectFechaNacimiento"); }
        }
        #endregion

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set { pInsertar = value; }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set { pEditar = value; }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
                if (value)
                    MenuBuscarEnabled = value;
            }
        }

        private bool _BuscarImputadoHabilitado = false;
        public bool BuscarImputadoHabilitado
        {
            get { return _BuscarImputadoHabilitado; }
            set { _BuscarImputadoHabilitado = value; OnPropertyChanged("BuscarImputadoHabilitado"); }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set { pImprimir = value; }
        }
        #endregion


        #region Menu y Enabled
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        #endregion

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


        private string _Arterial1;
        public string Arterial1
        {
            get { return _Arterial1; }
            set
            {
                _Arterial1 = value;
                if (!string.IsNullOrEmpty(value))
                    TextPresionArterial = value + "/" + Arterial2;

                OnPropertyChanged("Arterial1");
            }
        }

        private string _Arterial2;
        public string Arterial2
        {
            get { return _Arterial2; }
            set
            {
                _Arterial2 = value;
                if (!string.IsNullOrEmpty(value))
                    TextPresionArterial = Arterial1 + "/" + value;

                OnPropertyChanged("Arterial2");
            }
        }

        private string _TextPresionArterial;
        public string TextPresionArterial
        {
            get { return _TextPresionArterial; }
            set { _TextPresionArterial = value; OnPropertyChanged("TextPresionArterial"); }
        }

        private string _FrecuenciaCardiaca;

        public string FrecuenciaCardiaca
        {
            get { return _FrecuenciaCardiaca; }
            set { _FrecuenciaCardiaca = value; OnPropertyChanged("FrecuenciaCardiaca"); }
        }

        private string _FrecuenciaRespiratoria;

        public string FrecuenciaRespiratoria
        {
            get { return _FrecuenciaRespiratoria; }
            set { _FrecuenciaRespiratoria = value; OnPropertyChanged("FrecuenciaRespiratoria"); }
        }

        private string _Temperatura;

        public string Temperatura
        {
            get { return _Temperatura; }
            set { _Temperatura = value; OnPropertyChanged("Temperatura"); }
        }

        private decimal SelectedHospitalizacion { get; set; }

        private string _Glucemia;

        public string Glucemia
        {
            get { return _Glucemia; }
            set { _Glucemia = value; OnPropertyChanged("Glucemia"); }
        }

        private System.DateTime? _FecSeleccionadaregistro;
        public System.DateTime? FecSeleccionadaregistro
        {
            get { return _FecSeleccionadaregistro; }
            set
            {
                _FecSeleccionadaregistro = value;
                if (value.HasValue)
                {
                    FechaMinima = value.Value.Date.AddDays(-1);
                    FechaMaxima = value.Value;
                    OnPropertyChanged("FechaMinima");
                    OnPropertyChanged("FechaMaxima");
                }
                OnPropertyChanged("FecSeleccionadaregistro");
            }
        }

        private System.DateTime _FechaMaxima;

        public System.DateTime FechaMaxima
        {
            get { return _FechaMaxima; }
            set { _FechaMaxima = value; OnPropertyChanged("FechaMaxima"); }
        }

        private System.DateTime _FechaMinima;

        public System.DateTime FechaMinima
        {
            get { return _FechaMinima; }
            set { _FechaMinima = value; OnPropertyChanged("FechaMinima"); }
        }

        private System.DateTime? _FechaGeneraConcentrado = Fechas.GetFechaDateServer;

        public System.DateTime? FechaGeneraConcentrado
        {
            get { return _FechaGeneraConcentrado; }
            set
            {
                _FechaGeneraConcentrado = value;
                if (value.HasValue)
                {
                    FechaMinimaConcentrado = value.Value.Date.AddDays(-1);
                    FechaMaximaConcentrado = value.Value;
                    OnPropertyChanged("FechaMinimaConcentrado");
                    OnPropertyChanged("FechaMaximaConcentrado");
                }
                OnPropertyChanged("FechaGeneraConcentrado");
            }
        }

        private System.DateTime? _FechaMinimaConcentrado;

        public System.DateTime? FechaMinimaConcentrado
        {
            get { return _FechaMinimaConcentrado; }
            set { _FechaMinimaConcentrado = value; OnPropertyChanged("FechaMinimaConcentrado"); }
        }

        private System.DateTime? _FechaMaximaConcentrado;

        public System.DateTime? FechaMaximaConcentrado
        {
            get { return _FechaMaximaConcentrado; }
            set { _FechaMaximaConcentrado = value; OnPropertyChanged("FechaMaximaConcentrado"); }
        }
        private System.DateTime? _FechaInicioBusqueda = Fechas.GetFechaDateServer;

        public System.DateTime? FechaInicioBusqueda
        {
            get { return _FechaInicioBusqueda; }
            set { _FechaInicioBusqueda = value; OnPropertyChanged("FechaInicioBusqueda"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA> _ListHorasLiquidos;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA> ListHorasLiquidos
        {
            get { return _ListHorasLiquidos; }
            set { _ListHorasLiquidos = value; OnPropertyChanged("ListHorasLiquidos"); }
        }

        private decimal? _SelectedHoraLiquidos = -1;
        public decimal? SelectedHoraLiquidos
        {
            get { return _SelectedHoraLiquidos; }
            set { _SelectedHoraLiquidos = value; OnPropertyChanged("SelectedHoraLiquidos"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> _ListLiquidosIngresos;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> ListLiquidosIngresos
        {
            get { return _ListLiquidosIngresos; }
            set { _ListLiquidosIngresos = value; OnPropertyChanged("ListLiquidosIngresos"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN_TIPO> _ListTurnosLiquidos;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HOJA_CTRL_CONCEN_TIPO> ListTurnosLiquidos
        {
            get { return _ListTurnosLiquidos; }
            set { _ListTurnosLiquidos = value; OnPropertyChanged("ListTurnosLiquidos"); }
        }

        private decimal? _SelectedTurnoLiquidos = -1;

        public decimal? SelectedTurnoLiquidos
        {
            get { return _SelectedTurnoLiquidos; }
            set
            {
                _SelectedTurnoLiquidos = value;
                if (value.HasValue && value != -1)
                    ConsultaCocentrados();

                OnPropertyChanged("SelectedTurnoLiquidos");
            }
        }

        private decimal? _TotalBalance;

        public decimal? TotalBalance
        {
            get { return _TotalBalance; }
            set { _TotalBalance = value; OnPropertyChanged("TotalBalance"); }
        }

        private decimal? _TotalEntradas;

        public decimal? TotalEntradas
        {
            get { return _TotalEntradas; }
            set { _TotalEntradas = value; OnPropertyChanged("TotalEntradas"); }
        }

        private enum eTiposLiquidos
        {
            INGRESO = 1,
            EGRESO = 2
        };

        private enum eTiposFormatos
        {
            HOJA_CONTROL_LIQUIDOS = 1,
            HOJA_ENFERMERIA = 2
        };

        private decimal? _TotalSalidas;

        public decimal? TotalSalidas
        {
            get { return _TotalSalidas; }
            set { _TotalSalidas = value; OnPropertyChanged("TotalSalidas"); }
        }

        private decimal? _SelectedTipoLiquido = -1;

        public decimal? SelectedTipoLiquido
        {
            get { return _SelectedTipoLiquido; }
            set { _SelectedTipoLiquido = value; OnPropertyChanged("SelectedTipoLiquido"); }
        }
        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_TIPO> _ListTipoLiquido;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_TIPO> ListTipoLiquido
        {
            get { return _ListTipoLiquido; }
            set { _ListTipoLiquido = value; OnPropertyChanged("ListTipoLiquido"); }
        }
        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> _ListLiquidosEgresos;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> ListLiquidosEgresos
        {
            get { return _ListLiquidosEgresos; }
            set { _ListLiquidosEgresos = value; OnPropertyChanged("ListLiquidosEgresos"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE> _ListLiquidosIngresoEditar;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE> ListLiquidosIngresoEditar
        {
            get { return _ListLiquidosIngresoEditar; }
            set { _ListLiquidosIngresoEditar = value; OnPropertyChanged("ListLiquidosIngresoEditar"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE> _ListLiquidosEgresoEditar;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE> ListLiquidosEgresoEditar
        {
            get { return _ListLiquidosEgresoEditar; }
            set { _ListLiquidosEgresoEditar = value; OnPropertyChanged("ListLiquidosEgresoEditar"); }
        }
        private SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE _SelectedLiquido;
        public SSP.Servidor.LIQUIDO_HOJA_CTRL_DETALLE SelectedLiquido
        {
            get { return _SelectedLiquido; }
            set { _SelectedLiquido = value; OnPropertyChanged("SelectedLiquido"); }
        }


        private decimal? _selectedLiqIngreso = -1;
        public decimal? SelectedLiqIngreso
        {
            get { return _selectedLiqIngreso; }
            set { _selectedLiqIngreso = value; OnPropertyChanged("SelectedLiqIngreso"); }
        }

        private enum eTipoLiquidos
        {
            INGRESOS = 1,
            EGRESOS = 2
        };

        private enum eTipoConcentrados
        {
            MATUTINO = 1,
            VESPERTINO = 2,
            NOCTURNO = 3,
            TOTAL = 4
        };

        private decimal? _TxtCantidad;
        public decimal? TxtCantidad
        {
            get { return _TxtCantidad; }
            set { _TxtCantidad = value; OnPropertyChanged("TxtCantidad"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> _ListaLiquidos;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> ListaLiquidos
        {
            get { return _ListaLiquidos; }
            set { _ListaLiquidos = value; OnPropertyChanged("ListaLiquidos"); }
        }

        public class GridEspecialconsultaHojaLiquidos
        {
            public short Hora { get; set; }
            public string TensionArt { get; set; }
            public string FrecuenciaCard { get; set; }
            public string FrecuenciaRespiratoria { get; set; }
            public string Temperatura { get; set; }
            public string Detalles1 { get; set; }
            public string Detalles2 { get; set; }
        };

        private GridEspecialconsultaHojaLiquidos _SelectedConsultaEspecial;
        public GridEspecialconsultaHojaLiquidos SelectedConsultaEspecial
        {
            get { return _SelectedConsultaEspecial; }
            set { _SelectedConsultaEspecial = value; OnPropertyChanged("SelectedConsultaEspecial"); }
        }
        public class ComplementoiquidosEspecial
        {
            public string NombreLiquido { get; set; }
        };

        private System.Collections.ObjectModel.ObservableCollection<GridEspecialconsultaHojaLiquidos> _LstEspecialConsultaHojaLiquidos;
        public System.Collections.ObjectModel.ObservableCollection<GridEspecialconsultaHojaLiquidos> LstEspecialConsultaHojaLiquidos
        {
            get { return _LstEspecialConsultaHojaLiquidos; }
            set { _LstEspecialConsultaHojaLiquidos = value; OnPropertyChanged("LstEspecialConsultaHojaLiquidos"); }
        }

        private string _LeyendaIngresos = "INGRESOS";

        public string LeyendaIngresos
        {
            get { return _LeyendaIngresos; }
            set { _LeyendaIngresos = value; OnPropertyChanged("LeyendaIngresos"); }
        }

        private string _LeyendaEgresos = "EGRESOS";
        public string LeyendaEgresos
        {
            get { return _LeyendaEgresos; }
            set { _LeyendaEgresos = value; OnPropertyChanged("LeyendaEgresos"); }
        }

        #region COCNENTRADOS
        private decimal? _EntradasMatutino;

        public decimal? EntradasMatutino
        {
            get { return _EntradasMatutino; }
            set { _EntradasMatutino = value; OnPropertyChanged("EntradasMatutino"); }
        }

        private decimal? _SalidasMatutino;

        public decimal? SalidasMatutino
        {
            get { return _SalidasMatutino; }
            set { _SalidasMatutino = value; OnPropertyChanged("SalidasMatutino"); }
        }

        private decimal? _BalanceMatutino;

        public decimal? BalanceMatutino
        {
            get { return _BalanceMatutino; }
            set { _BalanceMatutino = value; OnPropertyChanged("BalanceMatutino"); }
        }

        private string _NombreMatutino;

        public string NombreMatutino
        {
            get { return _NombreMatutino; }
            set { _NombreMatutino = value; OnPropertyChanged("NombreMatutino"); }
        }

        private decimal? _EntradasVespertino;

        public decimal? EntradasVespertino
        {
            get { return _EntradasVespertino; }
            set { _EntradasVespertino = value; OnPropertyChanged("EntradasVespertino"); }
        }

        private decimal? _SalidasVespertino;

        public decimal? SalidasVespertino
        {
            get { return _SalidasVespertino; }
            set { _SalidasVespertino = value; OnPropertyChanged("SalidasVespertino"); }
        }

        private decimal? _BalanceVespertino;

        public decimal? BalanceVespertino
        {
            get { return _BalanceVespertino; }
            set { _BalanceVespertino = value; OnPropertyChanged("BalanceVespertino"); }
        }
        private string _NombreVespertino;

        public string NombreVespertino
        {
            get { return _NombreVespertino; }
            set { _NombreVespertino = value; OnPropertyChanged("NombreVespertino"); }
        }

        private decimal? _EntradasNocturno;

        public decimal? EntradasNocturno
        {
            get { return _EntradasNocturno; }
            set { _EntradasNocturno = value; OnPropertyChanged("EntradasNocturno"); }
        }

        private decimal? _SalidasNocturno;

        public decimal? SalidasNocturno
        {
            get { return _SalidasNocturno; }
            set { _SalidasNocturno = value; OnPropertyChanged("SalidasNocturno"); }
        }

        private decimal? _BalanceNocturno;

        public decimal? BalanceNocturno
        {
            get { return _BalanceNocturno; }
            set { _BalanceNocturno = value; OnPropertyChanged("BalanceNocturno"); }
        }

        private string _NombreNocturno;

        public string NombreNocturno
        {
            get { return _NombreNocturno; }
            set { _NombreNocturno = value; OnPropertyChanged("NombreNocturno"); }
        }

        private decimal? _EntradasTotal;

        public decimal? EntradasTotal
        {
            get { return _EntradasTotal; }
            set { _EntradasTotal = value; OnPropertyChanged("EntradasTotal"); }
        }

        private decimal? _SalidasTotal;

        public decimal? SalidasTotal
        {
            get { return _SalidasTotal; }
            set { _SalidasTotal = value; OnPropertyChanged("SalidasTotal"); }
        }

        private decimal? _BalanceTotal;

        public decimal? BalanceTotal
        {
            get { return _BalanceTotal; }
            set { _BalanceTotal = value; OnPropertyChanged("BalanceTotal"); }
        }

        private string _NombreTotal;

        public string NombreTotal
        {
            get { return _NombreTotal; }
            set { _NombreTotal = value; OnPropertyChanged("NombreTotal"); }
        }

        #endregion

        private short? _Opcion = -1;
        public short? Opcion
        {
            get { return _Opcion; }
            set
            {
                _Opcion = value;
                OnPropertyChanged("Opcion");
            }
        }

        private System.DateTime? _FechaMaxConsultas = Fechas.GetFechaDateServer;

        public System.DateTime? FechaMaxConsultas
        {
            get { return _FechaMaxConsultas; }
            set { _FechaMaxConsultas = value; OnPropertyChanged("FechaMaxConsultas"); }
        }
        private enum ePosicionActual
        {
            CAPTURA = 0,
            CONSULTA = 1
        };

        private bool esEnfermero = false;

        public bool EsEnfermero
        {
            get { return esEnfermero; }
            set { esEnfermero = value; OnPropertyChanged("EsEnfermero"); }
        }

        private enum eRolesMedi
        {
            MEDICO = 30,
            ENFERMERO = 31,
            COORDINADOR_MEDICO = 29,
            COORDINACION_ESTATAL_MEDICA = 25,
        };
    }
}