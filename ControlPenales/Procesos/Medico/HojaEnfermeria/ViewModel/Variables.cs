using System.Linq;
namespace ControlPenales
{
    public partial class HojaEnfermeriaViewModel
    {
        #region Configuracion Permisos
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

        #region PARAMETROS
        private byte[] ParametroCuerpoDorso;
        private byte[] ParametroCuerpoFrente;
        private byte[] ParametroImagenZonaCorporal;
        #endregion

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
        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        #endregion

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

        private string _CentroImp;

        public string CentroImp
        {
            get { return _CentroImp; }
            set { _CentroImp = value; OnPropertyChanged("CentroImp"); }
        }

        private string _TallaImp;

        public string TallaImp
        {
            get { return _TallaImp; }
            set { _TallaImp = value; OnPropertyChanged("TallaImp"); }
        }

        private System.DateTime? _FechaNacimientoImputado;

        public System.DateTime? FechaNacimientoImputado
        {
            get { return _FechaNacimientoImputado; }
            set { _FechaNacimientoImputado = value; OnPropertyChanged("FechaNacimientoImputado"); }
        }

        private System.DateTime? _FechaIngresoHospitalizacion;

        public System.DateTime? FechaIngresoHospitalizacion
        {
            get { return _FechaIngresoHospitalizacion; }
            set { _FechaIngresoHospitalizacion = value; OnPropertyChanged("FechaIngresoHospitalizacion"); }
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

        private decimal SelectedHospitalizacion { get; set; }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_TURNO> _ListTurnosLiquidos;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_TURNO> ListTurnosLiquidos
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
                    ProcesaHorasTurno(value);

                OnPropertyChanged("SelectedTurnoLiquidos");
            }
        }

        private System.DateTime _FechaMaximaRegistroMedicamentos = Fechas.GetFechaDateServer;
        public System.DateTime FechaMaximaRegistroMedicamentos
        {
            get { return _FechaMaximaRegistroMedicamentos; }
            set { _FechaMaximaRegistroMedicamentos = value; OnPropertyChanged("FechaMaximaRegistroMedicamentos"); }
        }
        private System.DateTime? _fechaMinimaHE;

        public System.DateTime? FechaMinimaHE
        {
            get { return _fechaMinimaHE; }
            set { _fechaMinimaHE = value; OnPropertyChanged("FechaMinimaHE"); }
        }

        private System.DateTime? _FechaMaximaHE;

        public System.DateTime? FechaMaximaHE
        {
            get { return _FechaMaximaHE; }
            set { _FechaMaximaHE = value; OnPropertyChanged("FechaMaximaHE"); }
        }

        #region CUERPO HUMANO
        private bool _TabFrente;
        public bool TabFrente
        {
            get { return _TabFrente; }
            set { _TabFrente = value; OnPropertyChanged("TabFrente"); }
        }
        private bool _TabDorso;
        public bool TabDorso
        {
            get { return _TabDorso; }
            set { _TabDorso = value; OnPropertyChanged("TabDorso"); }
        }

        private System.Collections.Generic.List<System.Windows.Controls.RadioButton> _ListRadioButonsDorso;
        public System.Collections.Generic.List<System.Windows.Controls.RadioButton> ListRadioButonsDorso
        {
            get { return _ListRadioButonsDorso; }
            set { _ListRadioButonsDorso = value; }
        }
        private System.Collections.Generic.List<System.Windows.Controls.RadioButton> _ListRadioButonsFrente;
        public System.Collections.Generic.List<System.Windows.Controls.RadioButton> ListRadioButonsFrente
        {
            get { return _ListRadioButonsFrente; }
            set { _ListRadioButonsFrente = value; }
        }

        #region LESIONES
        private bool _BotonLesionEnabled;
        public bool BotonLesionEnabled
        {
            get { return _BotonLesionEnabled; }
            set { _BotonLesionEnabled = value; OnPropertyChanged("BotonLesionEnabled"); }
        }
        private short? _SelectRegion;
        public short? SelectRegion
        {
            get { return _SelectRegion; }
            set
            {
                _SelectRegion = value;
                // BotonLesionEnabled = !string.IsNullOrEmpty(TextDescripcionLesion) && (value.HasValue ? value.Value > 0 : false);
                OnPropertyChanged("SelectRegion");
            }
        }

        private System.DateTime? _FechaHoyMedicamentosHE = Fechas.GetFechaDateServer;

        public System.DateTime? FechaHoyMedicamentosHE
        {
            get { return _FechaHoyMedicamentosHE; }
            set { _FechaHoyMedicamentosHE = value; OnPropertyChanged("FechaHoyMedicamentosHE"); }
        }
        private System.DateTime? _FechaCapturaMedicamento = Fechas.GetFechaDateServer;

        public System.DateTime? FechaCapturaMedicamento
        {
            get { return _FechaCapturaMedicamento; }
            set { _FechaCapturaMedicamento = value; OnPropertyChanged("FechaCapturaMedicamento"); }
        }

        private System.DateTime? _FechaMinimaCapturaMedicamento;

        public System.DateTime? FechaMinimaCapturaMedicamento
        {
            get { return _FechaMinimaCapturaMedicamento; }
            set { _FechaMinimaCapturaMedicamento = value; OnPropertyChanged("FechaMinimaCapturaMedicamento"); }
        }
        private System.Collections.ObjectModel.ObservableCollection<LesionesCustom> _ListLesiones;
        public System.Collections.ObjectModel.ObservableCollection<LesionesCustom> ListLesiones
        {
            get { return _ListLesiones; }
            set { _ListLesiones = value; OnPropertyChanged("ListLesiones"); }
        }
        private LesionesCustom _SelectLesion;
        public LesionesCustom SelectLesion
        {
            get { return _SelectLesion; }
            set { _SelectLesion = value; OnPropertyChanged("SelectLesion"); }
        }

        private LesionesCustom _SelectLesionEliminar;
        public LesionesCustom SelectLesionEliminar
        {
            get { return _SelectLesionEliminar; }
            set { _SelectLesionEliminar = value; OnPropertyChanged("SelectLesionEliminar"); }
        }
        private string _TextDescripcionLesion;
        public string TextDescripcionLesion
        {
            get { return _TextDescripcionLesion; }
            set
            {
                _TextDescripcionLesion = value;
                //BotonLesionEnabled = !string.IsNullOrEmpty(value) && (SelectRegion.HasValue ? SelectRegion.Value > 0 : false);
                OnPropertyChanged("TextDescripcionLesion");
            }
        }
        #endregion

        #endregion

        private string _FrecuenciaCardiacaHE;

        public string FrecuenciaCardiacaHE
        {
            get { return _FrecuenciaCardiacaHE; }
            set { _FrecuenciaCardiacaHE = value; OnPropertyChanged("FrecuenciaCardiacaHE"); }
        }

        private string _FrecuenciaRespiratoriaHE;

        public string FrecuenciaRespiratoriaHE
        {
            get { return _FrecuenciaRespiratoriaHE; }
            set { _FrecuenciaRespiratoriaHE = value; OnPropertyChanged("FrecuenciaRespiratoriaHE"); }
        }

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

        private string _TensionArtMediaHE;

        public string TensionArtMediaHE
        {
            get { return _TensionArtMediaHE; }
            set { _TensionArtMediaHE = value; OnPropertyChanged("TensionArtMediaHE"); }
        }

        private string _TempHE;

        public string TempHE
        {
            get { return _TempHE; }
            set { _TempHE = value; OnPropertyChanged("TempHE"); }
        }

        private string _Sa02HE;

        public string Sa02HE
        {
            get { return _Sa02HE; }
            set { _Sa02HE = value; OnPropertyChanged("Sa02HE"); }
        }

        private string _DextrHE;

        public string DextrHE
        {
            get { return _DextrHE; }
            set { _DextrHE = value; OnPropertyChanged("DextrHE"); }
        }

        private string _NebHE;

        public string NebHE
        {
            get { return _NebHE; }
            set { _NebHE = value; OnPropertyChanged("NebHE"); }
        }

        private string _PVCHE;

        public string PVCHE
        {
            get { return _PVCHE; }
            set { _PVCHE = value; OnPropertyChanged("PVCHE"); }
        }

        private string _CambioPosHE;

        public string CambioPosHE
        {
            get { return _CambioPosHE; }
            set { _CambioPosHE = value; OnPropertyChanged("CambioPosHE"); }
        }

        private string _RiesgoEscHE;

        public string RiesgoEscHE
        {
            get { return _RiesgoEscHE; }
            set { _RiesgoEscHE = value; OnPropertyChanged("RiesgoEscHE"); }
        }

        private string _RiesgoCaiHE;

        public string RiesgoCaiHE
        {
            get { return _RiesgoCaiHE; }
            set { _RiesgoCaiHE = value; OnPropertyChanged("RiesgoCaiHE"); }
        }

        private System.DateTime? _FechaHojaenfermeria = Fechas.GetFechaDateServer;

        public System.DateTime? FechaHojaenfermeria
        {
            get { return _FechaHojaenfermeria; }
            set { _FechaHojaenfermeria = value; OnPropertyChanged("FechaHojaenfermeria"); }
        }

        private System.Windows.Visibility _VisiblePrincipal = System.Windows.Visibility.Hidden;

        public System.Windows.Visibility VisiblePrincipal
        {
            get { return _VisiblePrincipal; }
            set { _VisiblePrincipal = value; OnPropertyChanged("VisiblePrincipal"); }
        }

        private System.Windows.Visibility _VisiblePasoDos = System.Windows.Visibility.Hidden;

        public System.Windows.Visibility VisiblePasoDos
        {
            get { return _VisiblePasoDos; }
            set { _VisiblePasoDos = value; OnPropertyChanged("VisiblePasoDos"); }
        }

        private int Opcion { get; set; }

        private System.Collections.ObjectModel.ObservableCollection<RecetaMedica> _ListRecetas;
        public System.Collections.ObjectModel.ObservableCollection<RecetaMedica> ListRecetas
        {
            get { return _ListRecetas; }
            set { _ListRecetas = value; OnPropertyChanged("ListRecetas"); }
        }

        private string _TextoAntiguaNotaEnfermeria;

        public string TextoAntiguaNotaEnfermeria
        {
            get { return _TextoAntiguaNotaEnfermeria; }
            set { _TextoAntiguaNotaEnfermeria = value; OnPropertyChanged("TextoAntiguaNotaEnfermeria"); }
        }
        private RecetaMedica _SelectReceta;
        public RecetaMedica SelectReceta
        {
            get { return _SelectReceta; }
            set { _SelectReceta = value; OnPropertyChanged("SelectReceta"); }
        }

        private ControlPenales.Controls.AutoCompleteTextBox _AutoCompleteReceta;
        public ControlPenales.Controls.AutoCompleteTextBox AutoCompleteReceta
        {
            get { return _AutoCompleteReceta; }
            set { _AutoCompleteReceta = value; }
        }

        private System.Windows.Controls.ListBox _AutoCompleteRecetaLB;
        public System.Windows.Controls.ListBox AutoCompleteRecetaLB
        {
            get { return _AutoCompleteRecetaLB; }
            set { _AutoCompleteRecetaLB = value; }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> lstLiquidosIngresoHE;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> LstLiquidosIngresoHE
        {
            get { return lstLiquidosIngresoHE; }
            set { lstLiquidosIngresoHE = value; OnPropertyChanged("LstLiquidosIngresoHE"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> lstLiquidosEgresoHE;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO> LstLiquidosEgresoHE
        {
            get { return lstLiquidosEgresoHE; }
            set { lstLiquidosEgresoHE = value; OnPropertyChanged("LstLiquidosEgresoHE"); }
        }

        private decimal? _SelectedLiquidoIngreso = -1;

        public decimal? SelectedLiquidoIngreso
        {
            get { return _SelectedLiquidoIngreso; }
            set
            {
                _SelectedLiquidoIngreso = value;
                if (value.HasValue && value != -1)
                {
                    base.RemoveRule("SelectedHoraIngresosHE");
                    base.RemoveRule("CantidadLiqIngresoHE");
                    base.AddRule(() => SelectedHoraIngresosHE, () => SelectedHoraIngresosHE != null ? SelectedHoraIngresosHE != -1 : false, "HORA EN LIQUIDOS DE INGRESO ES REQUERIDO!");
                    base.AddRule(() => CantidadLiqIngresoHE, () => CantidadLiqIngresoHE != null, "CANTIDAD EN LIQUIDOS DE INGRESO ES REQUERIDO!");

                    if (value == (decimal)eOpcionOtros.OTROS)
                    {
                        EnabledOtrosIngresosHE = true;
                        base.RemoveRule("EspecifiqueOtroLiquido");
                        base.AddRule(() => EspecifiqueOtroLiquido, () => !string.IsNullOrEmpty(EspecifiqueOtroLiquido), "OTROS LÍQUIDOS EN LIQUIDOS DE INGRESO  ES REQUERIDO!");
                        OnPropertyChanged("EspecifiqueOtroLiquido");
                        OnPropertyChanged("EnabledOtrosIngresosHE");
                    }
                    else
                    {
                        EnabledOtrosIngresosHE = false;
                        base.RemoveRule("EspecifiqueOtroLiquido");
                        OnPropertyChanged("EspecifiqueOtroLiquido");
                        OnPropertyChanged("EnabledOtrosIngresosHE");
                    }
                }
                else
                {
                    base.RemoveRule("SelectedHoraIngresosHE");
                    base.RemoveRule("CantidadLiqIngresoHE");
                    base.RemoveRule("EspecifiqueOtroLiquido");
                }

                OnPropertyChanged("SelectedHoraIngresosHE");
                OnPropertyChanged("CantidadLiqIngresoHE");
                OnPropertyChanged("EspecifiqueOtroLiquido");
                OnPropertyChanged("SelectedLiquidoIngreso");
            }
        }


        private SSP.Servidor.LIQUIDO _SelectedLiquidoIngresoMostrar;
        public SSP.Servidor.LIQUIDO SelectedLiquidoIngresoMostrar
        {
            get { return _SelectedLiquidoIngresoMostrar; }
            set { _SelectedLiquidoIngresoMostrar = value; OnPropertyChanged("SelectedLiquidoIngresoMostrar"); }
        }

        private SSP.Servidor.LIQUIDO_HORA selectedLiquidoHoraMostrar;
        public SSP.Servidor.LIQUIDO_HORA SelectedLiquidoHoraMostrar
        {
            get { return selectedLiquidoHoraMostrar; }
            set { selectedLiquidoHoraMostrar = value; OnPropertyChanged("SelectedLiquidoHoraMostrar"); }
        }

        private SSP.Servidor.LIQUIDO_HORA selectedHoraLiquidoEgresoMostrar;
        public SSP.Servidor.LIQUIDO_HORA SelectedHoraLiquidoEgresoMostrar
        {
            get { return selectedHoraLiquidoEgresoMostrar; }
            set { selectedHoraLiquidoEgresoMostrar = value; OnPropertyChanged("SelectedHoraLiquidoEgresoMostrar"); }
        }

        private SSP.Servidor.LIQUIDO selectedLiquidoEgresoMostrar;
        public SSP.Servidor.LIQUIDO SelectedLiquidoEgresoMostrar
        {
            get { return selectedLiquidoEgresoMostrar; }
            set { selectedLiquidoEgresoMostrar = value; OnPropertyChanged("SelectedLiquidoEgresoMostrar"); }
        }
        private decimal? _SelectedLiquidoEgreso = -1;

        public decimal? SelectedLiquidoEgreso
        {
            get { return _SelectedLiquidoEgreso; }
            set
            {
                _SelectedLiquidoEgreso = value;
                if (value.HasValue && value != -1)
                {
                    base.RemoveRule("SelectedHoraEgreso");
                    base.RemoveRule("CantidadLiqHEEgresos");
                    base.AddRule(() => SelectedHoraEgreso, () => SelectedHoraEgreso != null ? SelectedHoraEgreso != -1 : false, "HORA EN LIQUIDOS DE EGRESO ES REQUERIDO!");
                    base.AddRule(() => CantidadLiqHEEgresos, () => CantidadLiqHEEgresos != null, "CANTIDAD EN LIQUIDOS DE EGRESO ES REQUERIDO!");
                }
                else
                {
                    base.RemoveRule("SelectedHoraEgreso");
                    base.RemoveRule("CantidadLiqHEEgresos");
                }

                OnPropertyChanged("SelectedHoraEgreso");
                OnPropertyChanged("CantidadLiqHEEgresos");
                OnPropertyChanged("SelectedLiquidoEgreso");
            }
        }


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA> lstLiquidosHorasHE;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA> LstLiquidosHorasHE
        {
            get { return lstLiquidosHorasHE; }
            set { lstLiquidosHorasHE = value; OnPropertyChanged("LstLiquidosHorasHE"); }
        }

        private decimal? _SelectedHoraIngresosHE = -1;

        public decimal? SelectedHoraIngresosHE
        {
            get { return _SelectedHoraIngresosHE; }
            set { _SelectedHoraIngresosHE = value; OnPropertyChanged("SelectedHoraIngresosHE"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA> lstLiquidosHorasEgresoHE;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.LIQUIDO_HORA> LstLiquidosHorasEgresoHE
        {
            get { return lstLiquidosHorasEgresoHE; }
            set { lstLiquidosHorasEgresoHE = value; OnPropertyChanged("LstLiquidosHorasEgresoHE"); }
        }

        private decimal? _SelectedHoraEgreso = -1;
        public decimal? SelectedHoraEgreso
        {
            get { return _SelectedHoraEgreso; }
            set { _SelectedHoraEgreso = value; OnPropertyChanged("SelectedHoraEgreso"); }
        }

        private short? _CantidadLiqIngresoHE;

        public short? CantidadLiqIngresoHE
        {
            get { return _CantidadLiqIngresoHE; }
            set { _CantidadLiqIngresoHE = value; OnPropertyChanged("CantidadLiqIngresoHE"); }
        }

        private short? _CantidadLiqHEEgresos;

        public short? CantidadLiqHEEgresos
        {
            get { return _CantidadLiqHEEgresos; }
            set { _CantidadLiqHEEgresos = value; OnPropertyChanged("CantidadLiqHEEgresos"); }
        }
        private string _EspecifiqueOtroLiquido;

        public string EspecifiqueOtroLiquido
        {
            get { return _EspecifiqueOtroLiquido; }
            set { _EspecifiqueOtroLiquido = value; OnPropertyChanged("EspecifiqueOtroLiquido"); }
        }

        private bool _EnabledOtrosIngresosHE = false;
        public bool EnabledOtrosIngresosHE
        {
            get { return _EnabledOtrosIngresosHE; }
            set { _EnabledOtrosIngresosHE = value; OnPropertyChanged("EnabledOtrosIngresosHE"); }
        }

        private enum eOpcionOtros
        {
            OTROS = 15
        };

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

        private enum ePosicionActualHojaEnfermeria
        {
            INGRESOS = 2,
            EGRESOS = 3,
            CATETER = 7,
            SONDAS = 8
        };

        private enum eTurnosLiqudos
        {
            MATUTUNO = 1,
            VESPERTINO = 2,
            NOCTURNO = 3
        };

        private short _PosicionActual;
        public short PosicionActual
        {
            get { return _PosicionActual; }
            set { _PosicionActual = value; OnPropertyChanged("PosicionActual"); }
        }

        private string _AnotacionNuevaHojaEnfermeria;

        public string AnotacionNuevaHojaEnfermeria
        {
            get { return _AnotacionNuevaHojaEnfermeria; }
            set { _AnotacionNuevaHojaEnfermeria = value; OnPropertyChanged("AnotacionNuevaHojaEnfermeria"); }
        }

        private string _NotaEnfermeriaExistente;

        public string NotaEnfermeriaExistente
        {
            get { return _NotaEnfermeriaExistente; }
            set { _NotaEnfermeriaExistente = value; OnPropertyChanged("NotaEnfermeriaExistente"); }
        }

        private bool _EnabledHojas = true;

        public bool EnabledHojas
        {
            get { return _EnabledHojas; }
            set { _EnabledHojas = value; OnPropertyChanged("EnabledHojas"); }
        }

        private decimal _selectedHojaId;

        public decimal SelectedHojaId
        {
            get { return _selectedHojaId; }
            set { _selectedHojaId = value; OnPropertyChanged("SelectedHojaId"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_LECTURA> lstSignosVitalesHojaEnfermeria;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_LECTURA> LstSignosVitalesHojaEnfermeria
        {
            get { return lstSignosVitalesHojaEnfermeria; }
            set { lstSignosVitalesHojaEnfermeria = value; OnPropertyChanged("LstSignosVitalesHojaEnfermeria"); }
        }

        private SSP.Servidor.HOJA_ENFERMERIA_LECTURA _SelectedSignosVitalesHojaEnfermeria;

        public SSP.Servidor.HOJA_ENFERMERIA_LECTURA SelectedSignosVitalesHojaEnfermeria
        {
            get { return _SelectedSignosVitalesHojaEnfermeria; }
            set { _SelectedSignosVitalesHojaEnfermeria = value; OnPropertyChanged("SelectedSignosVitalesHojaEnfermeria"); }
        }

        private System.DateTime? _FechaHoraCaptura;

        public System.DateTime? FechaHoraCaptura
        {
            get { return _FechaHoraCaptura; }
            set { _FechaHoraCaptura = value; OnPropertyChanged("FechaHoraCaptura"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_CONTROL_ENFERMERIA> lstLiquidosIngresoHojaEnfermeria;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_CONTROL_ENFERMERIA> LstLiquidosIngresoHojaEnfermeria
        {
            get { return lstLiquidosIngresoHojaEnfermeria; }
            set { lstLiquidosIngresoHojaEnfermeria = value; OnPropertyChanged("LstLiquidosIngresoHojaEnfermeria"); }
        }

        private SSP.Servidor.HOJA_CONTROL_ENFERMERIA _SelectedLiquidoIngresoHE;

        public SSP.Servidor.HOJA_CONTROL_ENFERMERIA SelectedLiquidoIngresoHE
        {
            get { return _SelectedLiquidoIngresoHE; }
            set { _SelectedLiquidoIngresoHE = value; OnPropertyChanged("SelectedLiquidoIngresoHE"); }
        }


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_CONTROL_ENFERMERIA> lstLiquidosEgresoHojaEnfermeria;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_CONTROL_ENFERMERIA> LstLiquidosEgresoHojaEnfermeria
        {
            get { return lstLiquidosEgresoHojaEnfermeria; }
            set { lstLiquidosEgresoHojaEnfermeria = value; OnPropertyChanged("LstLiquidosEgresoHojaEnfermeria"); }
        }

        private SSP.Servidor.HOJA_CONTROL_ENFERMERIA selectedEgresoHE;

        public SSP.Servidor.HOJA_CONTROL_ENFERMERIA SelectedEgresoHE
        {
            get { return selectedEgresoHE; }
            set { selectedEgresoHE = value; OnPropertyChanged("SelectedEgresoHE"); }
        }

        private string _NombreMedicamentoEditar;
        public string NombreMedicamentoEditar
        {
            get { return _NombreMedicamentoEditar; }
            set { _NombreMedicamentoEditar = value; OnPropertyChanged("NombreMedicamentoEditar"); }
        }

        private string _UnidadMedidaMedicamentoEditar;

        public string UnidadMedidaMedicamentoEditar
        {
            get { return _UnidadMedidaMedicamentoEditar; }
            set { _UnidadMedidaMedicamentoEditar = value; OnPropertyChanged("UnidadMedidaMedicamentoEditar"); }
        }

        private string _NombrePresentacionMedicaMedicamentoEditar;

        public string NombrePresentacionMedicaMedicamentoEditar
        {
            get { return _NombrePresentacionMedicaMedicamentoEditar; }
            set { _NombrePresentacionMedicaMedicamentoEditar = value; OnPropertyChanged("NombrePresentacionMedicaMedicamentoEditar"); }
        }

        private string _CantidadMedicamentoEditar;

        public string CantidadMedicamentoEditar
        {
            get { return _CantidadMedicamentoEditar; }
            set { _CantidadMedicamentoEditar = value; OnPropertyChanged("CantidadMedicamentoEditar"); }
        }

        private System.DateTime? _FechaSuministroMedicamentoEditar;

        public System.DateTime? FechaSuministroMedicamentoEditar
        {
            get { return _FechaSuministroMedicamentoEditar; }
            set { _FechaSuministroMedicamentoEditar = value; OnPropertyChanged("FechaSuministroMedicamentoEditar"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_MEDICAMENTO> lstMedicamentosHE;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_MEDICAMENTO> LstMedicamentosHE
        {
            get { return lstMedicamentosHE; }
            set { lstMedicamentosHE = value; OnPropertyChanged("LstMedicamentosHE"); }
        }

        private SSP.Servidor.HOJA_ENFERMERIA_MEDICAMENTO _selectedMedicamento;

        public SSP.Servidor.HOJA_ENFERMERIA_MEDICAMENTO SelectedMedicamento
        {
            get { return _selectedMedicamento; }
            set { _selectedMedicamento = value; OnPropertyChanged("SelectedMedicamento"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_ULCERA> lstUlcerasHE;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_ULCERA> LstUlcerasHE
        {
            get { return lstUlcerasHE; }
            set { lstUlcerasHE = value; OnPropertyChanged("LstUlcerasHE"); }
        }

        private SSP.Servidor.HOJA_ENFERMERIA_ULCERA _SelectedUlceraHE;

        public SSP.Servidor.HOJA_ENFERMERIA_ULCERA SelectedUlceraHE
        {
            get { return _SelectedUlceraHE; }
            set { _SelectedUlceraHE = value; OnPropertyChanged("SelectedUlceraHE"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas> lstCustmoMedi;

        public System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosNotas> LstCustmoMedi
        {
            get { return lstCustmoMedi; }
            set { lstCustmoMedi = value; OnPropertyChanged("LstCustmoMedi"); }
        }


        private decimal? _SelectedTipoCataterHE = -1;

        public decimal? SelectedTipoCataterHE
        {
            get { return _SelectedTipoCataterHE; }
            set { _SelectedTipoCataterHE = value; OnPropertyChanged("SelectedTipoCataterHE"); }
        }

        private int ConsecutivoInterno { get; set; }
        private int ConsecutivoSondas { get; set; }
        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_CATETER> lstCateteres;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_CATETER> LstCateteres
        {
            get { return lstCateteres; }
            set { lstCateteres = value; OnPropertyChanged("LstCateteres"); }
        }

        private SSP.Servidor.HOJA_ENFERMERIA_CATETER _SelectedHojaCatater;

        public SSP.Servidor.HOJA_ENFERMERIA_CATETER SelectedHojaCatater
        {
            get { return _SelectedHojaCatater; }
            set
            {
                _SelectedHojaCatater = value;
                if (value != null)
                    VisualizarInformacionCateterSeleccionado(value);

                OnPropertyChanged("SelectedHojaCatater");
            }
        }

        private SSP.Servidor.HOJA_ENFERMERIA_CATETER _CateterTemp;
        public SSP.Servidor.HOJA_ENFERMERIA_CATETER CateterTemp
        {
            get { return _CateterTemp; }
            set { _CateterTemp = value; OnPropertyChanged("CateterTemp"); }
        }

        private SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS _SondaTemp;
        public SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS SondaTemp
        {
            get { return _SondaTemp; }
            set { _SondaTemp = value; OnPropertyChanged("SondaTemp"); }
        }

        private SSP.Servidor.CATETER_TIPO _SelectedTipoC;

        public SSP.Servidor.CATETER_TIPO SelectedTipoC
        {
            get { return _SelectedTipoC; }
            set { _SelectedTipoC = value; OnPropertyChanged("SelectedTipoC"); }
        }

        private bool _SoloLecturaDatosCateter = false;

        public bool SoloLecturaDatosCateter
        {
            get { return _SoloLecturaDatosCateter; }
            set { _SoloLecturaDatosCateter = value; OnPropertyChanged("SoloLecturaDatosCateter"); }
        }
        private System.Collections.Generic.List<SSP.Servidor.CATETER_TIPO> lstTiposCatater;

        public System.Collections.Generic.List<SSP.Servidor.CATETER_TIPO> LstTiposCatater
        {
            get { return lstTiposCatater; }
            set { lstTiposCatater = value; OnPropertyChanged("LstTiposCatater"); }
        }
        private System.DateTime? _FechaInstalacionCatHE;

        public System.DateTime? FechaInstalacionCatHE
        {
            get { return _FechaInstalacionCatHE; }
            set { _FechaInstalacionCatHE = value; OnPropertyChanged("FechaInstalacionCatHE"); }
        }

        private System.DateTime? _FechaRetiroSondaHE;
        public System.DateTime? FechaRetiroSondaHE
        {
            get { return _FechaRetiroSondaHE; }
            set { _FechaRetiroSondaHE = value; OnPropertyChanged("FechaRetiroSondaHE"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS> lstSondas;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS> LstSondas
        {
            get { return lstSondas; }
            set { lstSondas = value; OnPropertyChanged("LstSondas"); }
        }

        private bool _IsFechaIniBusquedaValida = false;
        public bool IsFechaIniBusquedaValida
        {
            get { return _IsFechaIniBusquedaValida; }
            set { _IsFechaIniBusquedaValida = value; OnPropertyChanged("IsFechaIniBusquedaValida"); }
        }

        private bool _IsFechaSondaValida = false;
        public bool IsFechaSondaValida
        {
            get { return _IsFechaSondaValida; }
            set { _IsFechaSondaValida = value; OnPropertyChanged("IsFechaSondaValida"); }
        }

        private System.DateTime? _FechaRetiroCateterHE;

        public System.DateTime? FechaRetiroCateterHE
        {
            get { return _FechaRetiroCateterHE; }
            set { _FechaRetiroCateterHE = value; OnPropertyChanged("FechaRetiroCateterHE"); }
        }
        private SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS _SelectedSonda;
        public SSP.Servidor.HOJA_ENFERMERIA_SONDA_GASOGAS SelectedSonda
        {
            get { return _SelectedSonda; }
            set
            {
                _SelectedSonda = value;
                if (value != null)
                    VisualizarInformacionSondaSeleccionada(value);

                OnPropertyChanged("SelectedSonda");
            }
        }

        private string _NuevoLaboratorioAgregarHEView;

        public string NuevoLaboratorioAgregarHEView
        {
            get { return _NuevoLaboratorioAgregarHEView; }
            set { _NuevoLaboratorioAgregarHEView = value; OnPropertyChanged("NuevoLaboratorioAgregarHEView"); }
        }

        private string _LaboratorioExistenteHE;

        public string LaboratorioExistenteHE
        {
            get { return _LaboratorioExistenteHE; }
            set { _LaboratorioExistenteHE = value; OnPropertyChanged("LaboratorioExistenteHE"); }
        }

        private string _NuevoRayosXAgregarHEView;

        public string NuevoRayosXAgregarHEView
        {
            get { return _NuevoRayosXAgregarHEView; }
            set { _NuevoRayosXAgregarHEView = value; OnPropertyChanged("NuevoRayosXAgregarHEView"); }
        }

        private string _RayosXExistenteHE;

        public string RayosXExistenteHE
        {
            get { return _RayosXExistenteHE; }
            set { _RayosXExistenteHE = value; OnPropertyChanged("RayosXExistenteHE"); }
        }

        private string _TextoAntiguaRayosX;

        public string TextoAntiguaRayosX
        {
            get { return _TextoAntiguaRayosX; }
            set { _TextoAntiguaRayosX = value; OnPropertyChanged("TextoAntiguaRayosX"); }
        }

        private string _TextoAntiguaLaboratorio;

        public string TextoAntiguaLaboratorio
        {
            get { return _TextoAntiguaLaboratorio; }
            set { _TextoAntiguaLaboratorio = value; OnPropertyChanged("TextoAntiguaLaboratorio"); }
        }

        private System.DateTime? _FechavencimientoCatHE;

        public System.DateTime? FechavencimientoCatHE
        {
            get { return _FechavencimientoCatHE; }
            set { _FechavencimientoCatHE = value; OnPropertyChanged("FechavencimientoCatHE"); }
        }

        private string _SelectedRetiroCateterHE;

        public string SelectedRetiroCateterHE
        {
            get { return _SelectedRetiroCateterHE; }
            set
            {
                _SelectedRetiroCateterHE = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        EnabledRetirosCateterHE = true;
                        //EnabledPermiteQuitarCataterHe = true;
                        base.RemoveRule("FechaRetiroCateterHE");
                        base.RemoveRule("DatosIngeccionCateterHE");
                        base.RemoveRule("MotivoRetiroCateterHE");
                        base.AddRule(() => FechaRetiroCateterHE, () => FechaRetiroCateterHE != null, "FECHA DE RETIRO DEL CATÉTER ES REQUERIDA!");
                        base.AddRule(() => DatosIngeccionCateterHE, () => !string.IsNullOrEmpty(DatosIngeccionCateterHE), "DATOS DE INFECCION DEL CATÉTER ES REQUERIDA!");
                        base.AddRule(() => MotivoRetiroCateterHE, () => !string.IsNullOrEmpty(MotivoRetiroCateterHE), "MOTIVO DE RETIRO DEL CATÉTER ES REQUERIDA!");
                        OnPropertyChanged("EnabledRetirosCateterHE");
                        OnPropertyChanged("FechaRetiroCateterHE");
                        OnPropertyChanged("DatosIngeccionCateterHE");
                        OnPropertyChanged("MotivoRetiroCateterHE");
                        //OnPropertyChanged("EnabledPermiteQuitarCataterHe");
                    }
                    else
                    {
                        EnabledRetirosCateterHE = false;
                        //EnabledPermiteQuitarCataterHe = false;
                        base.RemoveRule("FechaRetiroCateterHE");
                        base.RemoveRule("DatosIngeccionCateterHE");
                        base.RemoveRule("MotivoRetiroCateterHE");
                        FechaRetiroCateterHE = null;
                        MotivoRetiroCateterHE = DatosIngeccionCateterHE = string.Empty;
                        base.RemoveRule("FechaRetiroCateterHE");
                        OnPropertyChanged("FechaRetiroCateterHE");
                        OnPropertyChanged("MotivoRetiroCateterHE");
                        OnPropertyChanged("DatosIngeccionCateterHE");
                        OnPropertyChanged("EnabledRetirosCateterHE");
                        OnPropertyChanged("FechaRetiroCateterHE");
                        //OnPropertyChanged("EnabledPermiteQuitarCataterHe");
                    }
                }
                else
                {
                    EnabledRetirosCateterHE = false;
                    //EnabledPermiteQuitarCataterHe = false;
                    FechaRetiroCateterHE = null;
                    MotivoRetiroCateterHE = string.Empty;
                    DatosIngeccionCateterHE = string.Empty;
                    base.RemoveRule("FechaRetiroCateterHE");
                    base.RemoveRule("DatosIngeccionCateterHE");
                    base.RemoveRule("MotivoRetiroCateterHE");
                    OnPropertyChanged("FechaRetiroCateterHE");
                    OnPropertyChanged("MotivoRetiroCateterHE");
                    OnPropertyChanged("EnabledRetirosCateterHE");
                    OnPropertyChanged("FechaRetiroCateterHE");
                    //OnPropertyChanged("EnabledPermiteQuitarCataterHe");
                }

                //OnPropertyChanged("EnabledPermiteQuitarCataterHe");
                OnPropertyChanged("FechaRetiroCateterHE");
                OnPropertyChanged("MotivoRetiroCateterHE");
                OnPropertyChanged("EnabledRetirosCateterHE");
                OnPropertyChanged("FechaRetiroCateterHE");
                OnPropertyChanged("SelectedRetiroCateterHE");
                OnPropertyChanged("DatosIngeccionCateterHE");
            }
        }

        private bool _EnabledPermiteQuitarCataterHe = false;
        public bool EnabledPermiteQuitarCataterHe
        {
            get { return _EnabledPermiteQuitarCataterHe; }
            set { _EnabledPermiteQuitarCataterHe = value; OnPropertyChanged("EnabledPermiteQuitarCataterHe"); }
        }

        private bool _EnabledEdicionSonda = true;
        public bool EnabledEdicionSonda
        {
            get { return _EnabledEdicionSonda; }
            set { _EnabledEdicionSonda = value; OnPropertyChanged("EnabledEdicionSonda"); }
        }

        private bool _EnabledretirarSondasHE = true;
        public bool EnabledretirarSondasHE
        {
            get { return _EnabledretirarSondasHE; }
            set { _EnabledretirarSondasHE = value; OnPropertyChanged("EnabledretirarSondasHE"); }
        }

        private bool _VisualizandoCateteres = false;
        public bool VisualizandoCateteres
        {
            get { return _VisualizandoCateteres; }
            set { _VisualizandoCateteres = value; OnPropertyChanged("VisualizandoCateteres"); }
        }

        public bool VisualizandoSondas { get; set; }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA_IN_MOTIVO> _LstIncidenteMotivo;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA_IN_MOTIVO> LstIncidenteMotivo
        {
            get { return _LstIncidenteMotivo; }
            set { _LstIncidenteMotivo = value; OnPropertyChanged("LstIncidenteMotivo"); }
        }

        private short? _SelectedIncidenteMotivoValue = -1;
        public short? SelectedIncidenteMotivoValue
        {
            get { return _SelectedIncidenteMotivoValue; }
            set { _SelectedIncidenteMotivoValue = value; OnPropertyChanged("SelectedIncidenteMotivoValue"); }
        }

        private string observacionesIncidente;
        public string ObservacionesIncidente
        {
            get { return observacionesIncidente; }
            set { observacionesIncidente = value; OnPropertyChanged("ObservacionesIncidente"); }
        }

        private string _Observacion;
        public string Observacion
        {
            get { return _Observacion; }
            set { _Observacion = value; OnPropertyChanged("Observacion"); }
        }

        private System.DateTime? _FechaIncidenteMotivo;
        public System.DateTime? FechaIncidenteMotivo
        {
            get { return _FechaIncidenteMotivo; }
            set { _FechaIncidenteMotivo = value; OnPropertyChanged("FechaIncidenteMotivo"); }
        }

        private bool _EnabledRetirosCateterHE = false;
        public bool EnabledRetirosCateterHE
        {
            get { return _EnabledRetirosCateterHE; }
            set { _EnabledRetirosCateterHE = value; OnPropertyChanged("EnabledRetirosCateterHE"); }
        }

        private string _DatosIngeccionCateterHE;

        public string DatosIngeccionCateterHE
        {
            get { return _DatosIngeccionCateterHE; }
            set { _DatosIngeccionCateterHE = value; OnPropertyChanged("DatosIngeccionCateterHE"); }
        }

        private System.DateTime? _FechaInstalacionSondaHE;

        public System.DateTime? FechaInstalacionSondaHE
        {
            get { return _FechaInstalacionSondaHE; }
            set { _FechaInstalacionSondaHE = value; OnPropertyChanged("FechaInstalacionSondaHE"); }
        }

        private string _SelectedRetiroSondaNHE;

        public string SelectedRetiroSondaNHE
        {
            get { return _SelectedRetiroSondaNHE; }
            set
            {
                _SelectedRetiroSondaNHE = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        IsEnabledFechaSonda = true;
                        base.AddRule(() => FechaRetiroSondaHE, () => FechaRetiroSondaHE != null, "FECHA DE RETIRO DE LA SONDA ES REQUERIDA!");
                        OnPropertyChanged("IsEnabledFechaSonda");
                        OnPropertyChanged("FechaRetiroSondaHE");
                    }
                    else
                    {
                        IsEnabledFechaSonda = false;
                        base.RemoveRule("FechaRetiroSondaHE");
                        OnPropertyChanged("IsEnabledFechaSonda");
                        OnPropertyChanged("FechaRetiroSondaHE");
                    }
                }
                else
                {
                    IsEnabledFechaSonda = false;
                    base.RemoveRule("FechaRetiroSondaHE");
                    OnPropertyChanged("IsEnabledFechaSonda");
                    OnPropertyChanged("FechaRetiroSondaHE");
                }

                OnPropertyChanged("SelectedRetiroSondaNHE");
            }
        }

        private bool _EstaEditando;
        public bool EstaEditando
        {
            get { return _EstaEditando; }
            set
            {
                _EstaEditando = value;
                if (value)
                {
                    NombreLeyendaMenuCateter = "Terminar Edición";
                    OnPropertyChanged("NombreLeyendaMenuCateter");
                }
                else
                {
                    NombreLeyendaMenuCateter = "Agregar";
                    OnPropertyChanged("NombreLeyendaMenuCateter");
                }
            }
        }

        private string _NombreLeyendaMenuCateter = "Agregar";
        public string NombreLeyendaMenuCateter
        {
            get { return _NombreLeyendaMenuCateter; }
            set { _NombreLeyendaMenuCateter = value; OnPropertyChanged("NombreLeyendaMenuCateter"); }
        }

        private bool _IsEnabledFechaSonda = false;
        public bool IsEnabledFechaSonda
        {
            get { return _IsEnabledFechaSonda; }
            set { _IsEnabledFechaSonda = value; OnPropertyChanged("IsEnabledFechaSonda"); }
        }
        private string _ObservacionesSondaN;

        public string ObservacionesSondaN
        {
            get { return _ObservacionesSondaN; }
            set { _ObservacionesSondaN = value; OnPropertyChanged("ObservacionesSondaN"); }
        }

        private string _NombreLeyendaMenuSondas = "Agregar";
        public string NombreLeyendaMenuSondas
        {
            get { return _NombreLeyendaMenuSondas; }
            set { _NombreLeyendaMenuSondas = value; OnPropertyChanged("NombreLeyendaMenuSondas"); }
        }


        private bool _EstaEditandoSondas;
        public bool EstaEditandoSondas
        {
            get { return _EstaEditandoSondas; }
            set 
            {
                _EstaEditandoSondas = value;
                if (value)
                {
                    NombreLeyendaMenuSondas = "Terminar Edición";
                    OnPropertyChanged("NombreLeyendaMenuSondas");
                }
                else
                {
                    NombreLeyendaMenuSondas = "Agregar";
                    OnPropertyChanged("NombreLeyendaMenuSondas");
                }

                OnPropertyChanged("EstaEditandoSondas");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENCIAS_CATETER> lstIncidenciasCateterHE;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENCIAS_CATETER> LstIncidenciasCateterHE
        {
            get { return lstIncidenciasCateterHE; }
            set { lstIncidenciasCateterHE = value; OnPropertyChanged("LstIncidenciasCateterHE"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENCIAS_SONDA_GAS> lstIncidenciassonda;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENCIAS_SONDA_GAS> LstIncidenciassonda
        {
            get { return lstIncidenciassonda; }
            set { lstIncidenciassonda = value; OnPropertyChanged("LstIncidenciassonda"); }
        }

        private string _MotivoRetiroCateterHE;

        public string MotivoRetiroCateterHE
        {
            get { return _MotivoRetiroCateterHE; }
            set { _MotivoRetiroCateterHE = value; OnPropertyChanged("MotivoRetiroCateterHE"); }
        }

        private bool _EnabledQuitarCateter = true;
        public bool EnabledQuitarCateter
        {
            get { return _EnabledQuitarCateter; }
            set { _EnabledQuitarCateter = value; OnPropertyChanged("EnabledQuitarCateter"); }
        }

        private cCustomMedicamentosNotas _SelectedMedicCustom;

        public cCustomMedicamentosNotas SelectedMedicCustom
        {
            get { return _SelectedMedicCustom; }
            set { _SelectedMedicCustom = value; OnPropertyChanged("SelectedMedicCustom"); }
        }
        public class cCustomMedicamentosNotas
        {
            public string NombreMedicamento { get; set; }
            public string UnidadMedida { get; set; }
            public string Presentacion { get; set; }
            public decimal? Cantidad { get; set; }
            public short? Duracion { get; set; }
            public string Desayuno { get; set; }
            public string Comida { get; set; }
            public string Cena { get; set; }
            public int? IdProducto { get; set; }
            public short? IdPResentacionProducto { get; set; }
            public string Obsertvaciones { get; set; }
            public System.DateTime? FechaReceto { get; set; }
            public System.DateTime? FechaSuministro { get; set; }
            public string UltimaFecha { get; set; }
            public int? IdFolio { get; set; }
            public int? IdAtencionMedica { get; set; }
            public decimal? Id { get; set; }
        }
    }
}