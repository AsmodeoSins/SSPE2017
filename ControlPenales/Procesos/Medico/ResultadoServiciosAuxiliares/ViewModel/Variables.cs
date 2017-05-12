using System.Linq;

namespace ControlPenales
{
    partial class ResultadoTratamientoServAuxViewModel
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
        private int? ingresosD;
        public int? IngresosD
        {
            get { return ingresosD; }
            set { ingresosD = value; OnPropertyChanged("IngresosD"); }
        }
        private string noControlD;
        public string NoControlD
        {
            get { return noControlD; }
            set { noControlD = value; OnPropertyChanged("NoControlD"); }
        }
        private string ubicacionD;
        public string UbicacionD
        {
            get { return ubicacionD; }
            set { ubicacionD = value; OnPropertyChanged("UbicacionD"); }
        }
        private string tipoSeguridadD;
        public string TipoSeguridadD
        {
            get { return tipoSeguridadD; }
            set { tipoSeguridadD = value; OnPropertyChanged("TipoSeguridadD"); }
        }
        private System.DateTime? fecIngresoD;
        public System.DateTime? FecIngresoD
        {
            get { return fecIngresoD; }
            set { fecIngresoD = value; OnPropertyChanged("FecIngresoD"); }
        }
        private string clasificacionJuridicaD;
        public string ClasificacionJuridicaD
        {
            get { return clasificacionJuridicaD; }
            set { clasificacionJuridicaD = value; OnPropertyChanged("ClasificacionJuridicaD"); }
        }
        private string estatusD;
        public string EstatusD
        {
            get { return estatusD; }
            set { estatusD = value; OnPropertyChanged("EstatusD"); }
        }

        private int indexMenu;
        public int IndexMenu
        {
            get { return indexMenu; }
            set { indexMenu = value; OnPropertyChanged("IndexMenu"); }
        }


        private string tituloModal;
        public string TituloModal
        {
            get { return tituloModal; }
            set { tituloModal = value; OnPropertyChanged("TituloModal"); }
        }

        private string tituloAlias;
        public string TituloAlias
        {
            get { return tituloAlias; }
            set { tituloAlias = value; OnPropertyChanged("TituloAlias"); }
        }

        private string tituloApodo;
        public string TituloApodo
        {
            get { return tituloApodo; }
            set { tituloApodo = value; OnPropertyChanged("TituloApodo"); }
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
        private string _TextLugarNacimiento;
        public string TextLugarNacimiento
        {
            get { return _TextLugarNacimiento; }
            set { _TextLugarNacimiento = value; OnPropertyChanged("TextLugarNacimiento"); }
        }
        private string _TextEscolaridad;
        public string TextEscolaridad
        {
            get { return _TextEscolaridad; }
            set { _TextEscolaridad = value; OnPropertyChanged("TextEscolaridad"); }
        }
        private string _TextOcupacion;
        public string TextOcupacion
        {
            get { return _TextOcupacion; }
            set { _TextOcupacion = value; OnPropertyChanged("TextOcupacion"); }
        }
        private string _TextFechaIngreso;
        public string TextFechaIngreso
        {
            get { return _TextFechaIngreso; }
            set { _TextFechaIngreso = value; OnPropertyChanged("TextFechaIngreso"); }
        }
        private string _TextDelito;
        public string TextDelito
        {
            get { return _TextDelito; }
            set { _TextDelito = value; OnPropertyChanged("TextDelito"); }
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

        #region PROCESO
        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.FORMATO_DOCUMENTO> lstFormatosArchivos;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.FORMATO_DOCUMENTO> LstFormatosArchivos
        {
            get { return lstFormatosArchivos; }
            set { lstFormatosArchivos = value; OnPropertyChanged("LstFormatosArchivos"); }
        }

        private short _SelectedFormatoArchivo = -1;
        public short SelectedFormatoArchivo
        {
            get { return _SelectedFormatoArchivo; }
            set { _SelectedFormatoArchivo = value; OnPropertyChanged("SelectedFormatoArchivo"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT> lstTipoServAuxEdicion;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT> LstTipoServAuxEdicion
        {
            get { return lstTipoServAuxEdicion; }
            set { lstTipoServAuxEdicion = value; OnPropertyChanged("LstTipoServAuxEdicion"); }
        }

        private short _SelectedTipoServAuxEdicion = -1;

        public short SelectedTipoServAuxEdicion
        {
            get { return _SelectedTipoServAuxEdicion; }
            set { _SelectedTipoServAuxEdicion = value; OnPropertyChanged("SelectedTipoServAuxEdicion"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT> _LstSubTipoServAuxEdicion;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT> LstSubTipoServAuxEdicion
        {
            get { return _LstSubTipoServAuxEdicion; }
            set { _LstSubTipoServAuxEdicion = value; OnPropertyChanged("LstSubTipoServAuxEdicion"); }
        }

        private short _SelectedSubTipoServAuxEdicion = -1;

        public short SelectedSubTipoServAuxEdicion
        {
            get { return _SelectedSubTipoServAuxEdicion; }
            set { _SelectedSubTipoServAuxEdicion = value; OnPropertyChanged("SelectedSubTipoServAuxEdicion"); }
        }

        private bool _EnabledSubirArchivo = true;

        public bool EnabledSubirArchivo
        {
            get { return _EnabledSubirArchivo; }
            set { _EnabledSubirArchivo = value; OnPropertyChanged("EnabledSubirArchivo"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT> lstTipoServAux;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT> LstTipoServAux
        {
            get { return lstTipoServAux; }
            set { lstTipoServAux = value; RaisePropertyChanged("LstTipoServAux"); }
        }

        private short selectedTipoServAux = -1;
        public short SelectedTipoServAux
        {
            get { return selectedTipoServAux; }
            set { selectedTipoServAux = value; OnPropertyValidateChanged("SelectedTipoServAux"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT> lstSubtipoServAux;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT> LstSubtipoServAux
        {
            get { return lstSubtipoServAux; }
            set { lstSubtipoServAux = value; RaisePropertyChanged("LstSubtipoServAux"); }
        }

        private short selectedSubtipoServAux = -1;
        public short SelectedSubtipoServAux
        {
            get { return selectedSubtipoServAux; }
            set { selectedSubtipoServAux = value; OnPropertyValidateChanged("SelectedSubtipoServAux"); }
        }

        private System.DateTime? _FechaInicioBusquedaResultServ = Fechas.GetFechaDateServer;

        public System.DateTime? FechaInicioBusquedaResultServ
        {
            get { return _FechaInicioBusquedaResultServ; }
            set 
            {
                _FechaInicioBusquedaResultServ = value;
                if (value.HasValue)//avisa ala fecha de fin que ha cambiado
                {
                    FechaFinBusquedaResultServ = value;
                    OnPropertyChanged("FechaFinBusquedaResultServ");
                }
                
                OnPropertyChanged("FechaInicioBusquedaResultServ");
            }
        }

        private System.DateTime? _FechaFinBusquedaResultServ = Fechas.GetFechaDateServer;
        public System.DateTime? FechaFinBusquedaResultServ
        {
            get { return _FechaFinBusquedaResultServ; }
            set { _FechaFinBusquedaResultServ = value; OnPropertyChanged("FechaFinBusquedaResultServ"); }
        }


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SERVICIO_AUX_DIAG_TRAT> _LstDiagnosticos;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SERVICIO_AUX_DIAG_TRAT> LstDiagnosticos
        {
            get { return _LstDiagnosticos; }
            set { _LstDiagnosticos = value; OnPropertyChanged("LstDiagnosticos"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> lstServAux;
        public System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> LstServAux
        {
            get { return lstServAux; }
            set { lstServAux = value; RaisePropertyChanged("LstServAux"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> lstDiagnosticosPrincipal;

        public System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> LstDiagnosticosPrincipal
        {
            get { return lstDiagnosticosPrincipal; }
            set { lstDiagnosticosPrincipal = value; OnPropertyChanged("LstDiagnosticosPrincipal"); }
        }
        private int _SelectedDiagnosticoEdicion = -1;

        public int SelectedDiagnosticoEdicion
        {
            get { return _SelectedDiagnosticoEdicion; }
            set { _SelectedDiagnosticoEdicion = value; OnPropertyChanged("SelectedDiagnosticoEdicion"); }
        }

        private int _SelectedDiagnPrincipal = -1;
        public int SelectedDiagnPrincipal
        {
            get { return _SelectedDiagnPrincipal; }
            set { _SelectedDiagnPrincipal = value; OnPropertyChanged("SelectedDiagnPrincipal"); }
        }
        public byte[] _ArchivoSubido { get; set; }
        public string _NombreArchivoSubido { get; set; }
        public short ExtensionArchivoElegido { get; set; }

        private System.Collections.ObjectModel.ObservableCollection<CustomGridSinBytes> lstCustomizadaSinArchivos;//Lista que solo carga los datos SIN LOS BYTES (para ahorrar memoria)

        public System.Collections.ObjectModel.ObservableCollection<CustomGridSinBytes> LstCustomizadaSinArchivos
        {
            get { return lstCustomizadaSinArchivos; }
            set { lstCustomizadaSinArchivos = value; OnPropertyChanged("LstCustomizadaSinArchivos"); }
        }

        private CustomGridSinBytes _SeletedResultadoSinArchivo;

        public CustomGridSinBytes SeletedResultadoSinArchivo
        {
            get { return _SeletedResultadoSinArchivo; }
            set { _SeletedResultadoSinArchivo = value; OnPropertyChanged("SeletedResultadoSinArchivo"); }
        }
        #endregion


        private bool _EmptyResultados = true;

        public bool EmptyResultados
        {
            get { return _EmptyResultados; }
            set { _EmptyResultados = value; OnPropertyChanged("EmptyResultados"); }
        }

        private enum eformatosPermitidos
        {
            PDF = 3,
            JPEG = 5,
            JPG = 15
        };

        private System.Collections.ObjectModel.ObservableCollection<CustomIngresos> lstCustomizadaIngresos;
        public System.Collections.ObjectModel.ObservableCollection<CustomIngresos> LstCustomizadaIngresos
        {
            get { return lstCustomizadaIngresos; }
            set { lstCustomizadaIngresos = value; OnPropertyChanged("LstCustomizadaIngresos"); }
        }

        private short? _SelectedIngresoBusquedas;
        public short? SelectedIngresoBusquedas
        {
            get { return _SelectedIngresoBusquedas; }
            set { _SelectedIngresoBusquedas = value; OnPropertyChanged("SelectedIngresoBusquedas"); }
        }
        public class CustomIngresos
        {
            public string DescripcionIngreso { get; set; }
            public short IdIngreso { get; set; }
        }
    }
}