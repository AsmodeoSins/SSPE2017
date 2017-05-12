using System.Linq;

namespace ControlPenales
{
    partial class NotificacionTrabajoSocialViewModel
    {
        public string Name
        {
            get
            {
                return "notificacion_trabajo_social";
            }
        }

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


        private enum eRolesNotificacionTrabajoSocial
        {
            MEDICO = 30,
            TRABAJADOR_SOCIAL = 34,
            TRABAJO_SOCIAL = 4,
            COORDINADOR_MEDICO = 29,
            COORDINACION_ESTATAL_MEDICA = 25,
            DENTISTA = 33
        };

        private bool _EsMedico = false;

        public bool EsMedico
        {
            get { return _EsMedico; }
            set { _EsMedico = value; OnPropertyChanged("EsMedico"); }
        }

        private bool _EsTrabajoSocial = false;

        public bool EsTrabajoSocial
        {
            get { return _EsTrabajoSocial; }
            set { _EsTrabajoSocial = value; OnPropertyChanged("EsTrabajoSocial"); }
        }

        private bool _EnabledCamposMedicos = true;

        public bool EnabledCamposMedicos
        {
            get { return _EnabledCamposMedicos; }
            set { _EnabledCamposMedicos = value; OnPropertyChanged("EnabledCamposMedicos"); }
        }

        private string _NombrePerfilActual;

        public string NombrePerfilActual
        {
            get { return _NombrePerfilActual; }
            set { _NombrePerfilActual = value; OnPropertyChanged("NombrePerfilActual"); }
        }

        private System.DateTime? _FechaInicioBusqueda;

        public System.DateTime? FechaInicioBusqueda
        {
            get { return _FechaInicioBusqueda; }
            set { _FechaInicioBusqueda = value; OnPropertyChanged("FechaInicioBusqueda"); }
        }

        private string _NombreMotivo = string.Empty;

        public string NombreMotivo
        {
            get { return _NombreMotivo; }
            set { _NombreMotivo = value; OnPropertyChanged("NombreMotivo"); }
        }

        private string _NombreTituloCapturaNotificacion = string.Empty;

        public string NombreTituloCapturaNotificacion
        {
            get { return _NombreTituloCapturaNotificacion; }
            set { _NombreTituloCapturaNotificacion = value; OnPropertyChanged("NombreTituloCapturaNotificacion"); }
        }

        private string _TipoNotificacionDescripcion;

        public string TipoNotificacionDescripcion
        {
            get { return _TipoNotificacionDescripcion; }
            set { _TipoNotificacionDescripcion = value; OnPropertyChanged("TipoNotificacionDescripcion"); }
        }

        private string _NombreImputado;

        public string NombreImputado
        {
            get { return _NombreImputado; }
            set { _NombreImputado = value; OnPropertyChanged("NombreImputado"); }
        }

        private string _EstanciaImputadp;

        public string EstanciaImputadp
        {
            get { return _EstanciaImputadp; }
            set { _EstanciaImputadp = value; OnPropertyChanged("EstanciaImputadp"); }
        }

        private System.DateTime? _FechaSolicitud;

        public System.DateTime? FechaSolicitud
        {
            get { return _FechaSolicitud; }
            set { _FechaSolicitud = value; OnPropertyChanged("FechaSolicitud"); }
        }

        private string _Expediente;

        public string Expediente
        {
            get { return _Expediente; }
            set { _Expediente = value; OnPropertyChanged("Expediente"); }
        }

        private string _OtroRiesgoEspecifique;

        public string OtroRiesgoEspecifique
        {
            get { return _OtroRiesgoEspecifique; }
            set { _OtroRiesgoEspecifique = value; OnPropertyChanged("OtroRiesgoEspecifique"); }
        }

        private string _MotivoNotificacion;

        public string MotivoNotificacion
        {
            get { return _MotivoNotificacion; }
            set { _MotivoNotificacion = value; OnPropertyChanged("MotivoNotificacion"); }
        }

        private decimal? _CaracterNotificacion;

        public decimal? CaracterNotificacion
        {
            get { return _CaracterNotificacion; }
            set { _CaracterNotificacion = value; OnPropertyChanged("CaracterNotificacion"); }
        }
        
        private bool _RequeridoOtrosEspecifique = false;
        public bool RequeridoOtrosEspecifique
        {
            get { return _RequeridoOtrosEspecifique; }
            set { _RequeridoOtrosEspecifique = value; OnPropertyChanged("RequeridoOtrosEspecifique"); }
        }

        private decimal? _RiesgosNotificacionTS;
        public decimal? RiesgosNotificacionTS
        {
            get { return _RiesgosNotificacionTS; }
            set 
            {
                _RiesgosNotificacionTS = value;
                if (value.HasValue)
                {
                    if (value == (decimal)eRiesgosOtros.OTROS)
                    {
                        RequeridoOtrosEspecifique = true;
                        base.RemoveRule("OtroRiesgoEspecifique");
                        base.AddRule(() => OtroRiesgoEspecifique, () => !string.IsNullOrEmpty(OtroRiesgoEspecifique), "ESPECIFIQUE OTROS RIESGOS ES REQUERIDO!");
                        OnPropertyChanged("OtroRiesgoEspecifique");
                    }
                    else
                    {
                        RequeridoOtrosEspecifique = false;
                        base.RemoveRule("OtroRiesgoEspecifique");
                        OnPropertyChanged("OtroRiesgoEspecifique");
                    }
                };

                OnPropertyChanged("RiesgosNotificacionTS");
            }
        }

        private enum eRiesgosOtros
        {
            OTROS = 5//ENUMERADOR PARA IDENTIFICAR EL ID CORRESPONDIENTE AL CAMPO OTROS Y ACTIVAR LA VALIDACION DINAMICA
        }

        private string _NombreCentro;
        public string NombreCentro
        {
            get { return _NombreCentro; }
            set { _NombreCentro = value; OnPropertyChanged("NombreCentro"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ENFERMEDAD> _ListEnfermedades;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ENFERMEDAD> ListEnfermedades
        {
            get { return _ListEnfermedades; }
            set { _ListEnfermedades = value; OnPropertyChanged("ListEnfermedades"); }
        }

        private SSP.Servidor.ENFERMEDAD _SelectEnfermedad;
        public SSP.Servidor.ENFERMEDAD SelectEnfermedad
        {
            get { return _SelectEnfermedad; }
            set { _SelectEnfermedad = value; OnPropertyChanged("SelectEnfermedad"); }
        }

        private System.Windows.Controls.ListBox _AutoComplete;
        public System.Windows.Controls.ListBox AutoCompleteLB
        {
            get { return _AutoComplete; }
            set { _AutoComplete = value; }
        }

        private ControlPenales.Controls.AutoCompleteTextBox _AutoCompleteTB;
        public ControlPenales.Controls.AutoCompleteTextBox AutoCompleteTB
        {
            get { return _AutoCompleteTB; }
            set { _AutoCompleteTB = value; }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.CARACTER_NOTIFICACION_TS> lstCaracterNotificacionTS;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.CARACTER_NOTIFICACION_TS> LstCaracterNotificacionTS
        {
            get { return lstCaracterNotificacionTS; }
            set { lstCaracterNotificacionTS = value; OnPropertyChanged("LstCaracterNotificacionTS"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.RIESGO_NOTIFICACION_TS> lstRiesgosNotificacionTS;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.RIESGO_NOTIFICACION_TS> LstRiesgosNotificacionTS
        {
            get { return lstRiesgosNotificacionTS; }
            set { lstRiesgosNotificacionTS = value; OnPropertyChanged("LstRiesgosNotificacionTS"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.DIAGNOSTICO_NOTIFICA_TS_TIPO> lstTipoNotificacionTS;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.DIAGNOSTICO_NOTIFICA_TS_TIPO> LstTipoNotificacionTS
        {
            get { return lstTipoNotificacionTS; }
            set { lstTipoNotificacionTS = value; OnPropertyChanged("LstTipoNotificacionTS"); }
        }


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.NOTIFICACION_TS> lstNotificacionesNuevas;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.NOTIFICACION_TS> LstNotificacionesNuevas
        {
            get { return lstNotificacionesNuevas; }
            set { lstNotificacionesNuevas = value; OnPropertyChanged("LstNotificacionesNuevas"); }
        }

        private SSP.Servidor.NOTIFICACION_TS _SelectedNotificacionNueva;

        public SSP.Servidor.NOTIFICACION_TS SelectedNotificacionNueva
        {
            get { return _SelectedNotificacionNueva; }
            set { _SelectedNotificacionNueva = value; OnPropertyChanged("SelectedNotificacionNueva"); }
        }

        private bool _IsReadOnlyOtrosTS = false;

        public bool IsReadOnlyOtrosTS
        {
            get { return _IsReadOnlyOtrosTS; }
            set { _IsReadOnlyOtrosTS = value; OnPropertyChanged("IsReadOnlyOtrosTS"); }
        }

        private enum eTiposRiesgos
        {
            HOSPITALIZADO_HOSPITAL_GENERAL = 1,
            HOSPITALIZADO_CENTRO = 2,
            DECESO = 3,
            ESTUDIOS = 4,
            OTROS = 5
        };

        private System.Windows.Visibility _VisibleMensajeTS = System.Windows.Visibility.Collapsed;

        public System.Windows.Visibility VisibleMensajeTS
        {
            get { return _VisibleMensajeTS; }
            set { _VisibleMensajeTS = value; OnPropertyChanged("VisibleMensajeTS"); }
        }

        private string _MensajeRespuestaTS;

        public string MensajeRespuestaTS
        {
            get { return _MensajeRespuestaTS; }
            set { _MensajeRespuestaTS = value; OnPropertyChanged("MensajeRespuestaTS"); }
        }

        private System.DateTime? _FechaInicioBusquedaNotificaciones = Fechas.GetFechaDateServer;

        public System.DateTime? FechaInicioBusquedaNotificaciones
        {
            get { return _FechaInicioBusquedaNotificaciones; }
            set { _FechaInicioBusquedaNotificaciones = value; OnPropertyChanged("FechaInicioBusquedaNotificaciones"); }
        }

        private System.DateTime? _FechaFinBusquedaNotificaciones = Fechas.GetFechaDateServer;

        public System.DateTime? FechaFinBusquedaNotificaciones
        {
            get { return _FechaFinBusquedaNotificaciones; }
            set { _FechaFinBusquedaNotificaciones = value; OnPropertyChanged("FechaFinBusquedaNotificaciones"); }
        }
        
        private bool _BorrarEnfermedadEnabled = true;
        public bool BorrarEnfermedadEnabled
        {
            get { return _BorrarEnfermedadEnabled; }
            set { _BorrarEnfermedadEnabled = value; OnPropertyChanged("BorrarEnfermedadEnabled"); }
        }

        private bool _EmptyResultados = true;

        public bool EmptyResultados
        {
            get { return _EmptyResultados; }
            set { _EmptyResultados = value; OnPropertyChanged("EmptyResultados"); }
        }

        private System.DateTime _FechaServer = Fechas.GetFechaDateServer;

        public System.DateTime FechaServer
        {
            get { return _FechaServer; }
            set { _FechaServer = value; OnPropertyChanged("FechaServer"); }
        }
    }
}