using System.Linq;

namespace ControlPenales
{
    partial class EstudioPersonalidadViewModel
    {
        public string Name
        {
            get
            {
                return "estudio_personalidad";
            }
        }

        private enum eTipoEstudioDetalle
        {
            ACTIVO = 1 ,
            PENDIENTE = 2,
            TERMINADO = 3,
            CANCELADO = 4,
            ASIGNADO = 5
        }
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

        private SSP.Servidor.IMPUTADO selectExpediente;
        public SSP.Servidor.IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    if (selectExpediente.INGRESO != null && selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = selectExpediente.INGRESO.OrderBy(o => o.FEC_INGRESO_CERESO).FirstOrDefault();
                    }
                    else
                        EmptyIngresoVisible = true;

                    //OBTENEMOS FOTO DE FRENTE
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

        private string _TituloPopUp;

        public string TituloPopUp
        {
            get { return _TituloPopUp; }
            set { _TituloPopUp = value; OnPropertyChanged("TituloPopUp"); }
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

                if (value != null)
                    ProcesaEstudiosByImputado(value.ID_IMPUTADO, value.ID_INGRESO, value.ID_CENTRO,value.ID_ANIO);
                OnPropertyChanged("SelectIngreso");
            }
        }


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENTE_TIPO> lstIncidenteTipo;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INCIDENTE_TIPO> LstIncidenteTipo
        {
            get { return lstIncidenteTipo; }
            set { lstIncidenteTipo = value; OnPropertyChanged("LstIncidenteTipo"); }
        }

        private SSP.Servidor.INCIDENTE_TIPO selectedIncidenteTipo;
        public SSP.Servidor.INCIDENTE_TIPO SelectedIncidenteTipo
        {
            get { return selectedIncidenteTipo; }
            set { selectedIncidenteTipo = value; OnPropertyChanged("SelectedIncidenteTipo"); }
        }

        private short? idIncidenteTipo;
        public short? IdIncidenteTipo
        {
            get { return idIncidenteTipo; }
            set { idIncidenteTipo = value; OnPropertyChanged("IdIncidenteTipo"); }
        }

        private System.DateTime? fecIncidencia;
        public System.DateTime? FecIncidencia
        {
            get { return fecIncidencia; }
            set
            {
                fecIncidencia = value;
                SetValidacionesIncidente();
                OnPropertyChanged("FecIncidencia");
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


        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }


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
            set
            {
                folioD = value;
                OnPropertyChanged("FolioD");
            }
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
        private bool eliminarVisible;
        public bool EliminarVisible
        {
            get { return eliminarVisible; }
            set { eliminarVisible = value; OnPropertyChanged("EliminarVisible"); }
        }


        #endregion
        #region Variables usadas en pop up
        private System.DateTime? _FechaSolicitud;
        public System.DateTime? FechaSolicitud
        {
            get { return _FechaSolicitud; }
            set { _FechaSolicitud = value; OnPropertyChanged("FechaSolicitud"); }
        }

        private string _Motivo;
        public string Motivo
        {
            get { return _Motivo; }
            set { _Motivo = value; OnPropertyChanged("Motivo"); }
        }

        private string _SolicitadoPor;
        public string SolicitadoPor
        {
            get { return _SolicitadoPor; }
            set { _SolicitadoPor = value; OnPropertyChanged("SolicitadoPor"); }
        }

        private string _Situacion;
        public string Situacion
        {
            get { return _Situacion; }
            set { _Situacion = value; OnPropertyChanged("Situacion"); }
        }
        private System.DateTime? _FechaInicio;
        public System.DateTime? FechaInicio
        {
            get { return _FechaInicio; }
            set { _FechaInicio = value; OnPropertyChanged("FechaInicio"); }
        }

        private System.DateTime? _FechaFin;
        public System.DateTime? FechaFin
        {
            get { return _FechaFin; }
            set { _FechaFin = value; OnPropertyChanged("FechaFin"); }
        }


        #endregion

        #region Busqueda especifica
        private System.DateTime? _FechaInicioBusqueda = Fechas.GetFechaDateServer;

        public System.DateTime? FechaInicioBusqueda
        {
            get { return _FechaInicioBusqueda; }
            set { _FechaInicioBusqueda = value; OnPropertyChanged("FechaInicioBusqueda"); }
        }

        private System.DateTime? _FechaFinBusqueda = Fechas.GetFechaDateServer;

        public System.DateTime? FechaFinBusqueda
        {
            get { return _FechaFinBusqueda; }
            set { _FechaFinBusqueda = value; OnPropertyChanged("FechaFinBusqueda"); }
        }

        private string _NoOficioBusqueda;
        public string NoOficioBusqueda
        {
            get { return _NoOficioBusqueda; }
            set { _NoOficioBusqueda = value; OnPropertyChanged("NoOficioBusqueda"); }
        }
        #endregion

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD> _ListEstudiosPersonalidad;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD> ListEstudiosPersonalidad
        {
            get { return _ListEstudiosPersonalidad; }
            set 
            {
                _ListEstudiosPersonalidad = value;
                OnPropertyChanged("ListEstudiosPersonalidad");
            }
        }

        private SSP.Servidor.PERSONALIDAD _SelectedEstudioPersonalidad;
        public SSP.Servidor.PERSONALIDAD SelectedEstudioPersonalidad
        {
            get { return _SelectedEstudioPersonalidad; }
            set 
            {
                _SelectedEstudioPersonalidad = value; 
                OnPropertyChanged("SelectedEstudioPersonalidad");
                if (value != null)
                {
                    ConsultaDetalle();
                    OnPropertyChanged("ListEstudiosPersonalidadDetalle");
                }
            }
        }

        private SSP.Servidor.PERSONALIDAD _IdEstudioPersonalidadPdre;

        public SSP.Servidor.PERSONALIDAD IdEstudioPersonalidadPdre
        {
            get { return _IdEstudioPersonalidadPdre; }
            set 
            {
                _IdEstudioPersonalidadPdre = value;
                if (value != null)
                    ProcesaDetalleEstudios(value.ID_ESTUDIO,value.ID_IMPUTADO,value.ID_CENTRO,value.ID_INGRESO, value.ID_ANIO);

                OnPropertyChanged("IdEstudioPersonalidadPdre");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_DETALLE> _ListEstudiosPersonalidadDetalle;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_DETALLE> ListEstudiosPersonalidadDetalle
        {
            get { return _ListEstudiosPersonalidadDetalle; }
            set { _ListEstudiosPersonalidadDetalle = value; OnPropertyChanged("ListEstudiosPersonalidadDetalle"); }
        }

        private SSP.Servidor.PERSONALIDAD_DETALLE _SelectedEstudioPersonalidadDetalle;
        public SSP.Servidor.PERSONALIDAD_DETALLE SelectedEstudioPersonalidadDetalle
        {
            get { return _SelectedEstudioPersonalidadDetalle; }
            set 
            {
                _SelectedEstudioPersonalidadDetalle = value;
                //if (value != null)
                //    ProcesaFechasDesarrollo(value);

                OnPropertyChanged("SelectedEstudioPersonalidadDetalle");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_SITUACION> _ListPersonalidadSituaciones;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_SITUACION> ListPersonalidadSituaciones
        {
            get { return _ListPersonalidadSituaciones; }
            set { _ListPersonalidadSituaciones = value; OnPropertyChanged("ListPersonalidadSituaciones"); }
        }

        private short? _SituacionEstudioPadreSelected;
        public short? SituacionEstudioPadreSelected
            {
              get { return _SituacionEstudioPadreSelected; }
              set { _SituacionEstudioPadreSelected = value; OnPropertyChanged("SituacionEstudioPadreSelected"); }
            }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_MOTIVO> _ListPersonalidadMotivo;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_MOTIVO> ListPersonalidadMotivo
        {
            get { return _ListPersonalidadMotivo; }
            set { _ListPersonalidadMotivo = value; OnPropertyChanged("ListPersonalidadMotivo"); }
        }

        private short? _MotivoEstudioPadreSelected;
        public short? MotivoEstudioPadreSelected
        {
            get { return _MotivoEstudioPadreSelected; }
            set { _MotivoEstudioPadreSelected = value; OnPropertyChanged("MotivoEstudioPadreSelected"); }
        }


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_TIPO_ESTUDIO> _ListTipoEstudio;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_TIPO_ESTUDIO> ListTipoEstudio
        {
            get { return _ListTipoEstudio; }
            set { _ListTipoEstudio = value; OnPropertyChanged("ListTipoEstudio"); }
        }

        private short? _SelectedTipoEstudioHijo;
        public short? SelectedTipoEstudioHijo
        {
            get { return _SelectedTipoEstudioHijo; }
            set { _SelectedTipoEstudioHijo = value; OnPropertyChanged("SelectedTipoEstudioHijo"); }
        }


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_ESTATUS> _ListEstatusEstudio;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_ESTATUS> ListEstatusEstudio
        {
            get { return _ListEstatusEstudio; }
            set { _ListEstatusEstudio = value; OnPropertyChanged("ListEstatusEstudio"); }
        }

        private short? _SelectedPersonalidadHijo;
        public short? SelectedPersonalidadHijo
        {
            get { return _SelectedPersonalidadHijo; }
            set { _SelectedPersonalidadHijo = value; OnPropertyChanged("SelectedPersonalidadHijo"); }
        }

        private short? _SelectedEstatusEstudioPersonalidadDetalle;
        public short? SelectedEstatusEstudioPersonalidadDetalle
        {
            get { return _SelectedEstatusEstudioPersonalidadDetalle; }
            set { _SelectedEstatusEstudioPersonalidadDetalle = value; OnPropertyChanged("SelectedEstatusEstudioPersonalidadDetalle"); }
        }

        #region Datos Detalle Estudio

        private short? _TipoEstudioSelectedDetalle;
        public short? TipoEstudioSelectedDetalle
        {
            get { return _TipoEstudioSelectedDetalle; }
            set { _TipoEstudioSelectedDetalle = value; OnPropertyChanged("TipoEstudioSelectedDetalle"); }
        }

        private short? _EstatusEstudioSelectedDetalle;
        public short? EstatusEstudioSelectedDetalle
        {
            get { return _EstatusEstudioSelectedDetalle; }
            set { _EstatusEstudioSelectedDetalle = value; OnPropertyChanged("EstatusEstudioSelectedDetalle"); }
        }

        private System.DateTime? _FechaSolicitudEstudioSelectedDetalle;
        public System.DateTime? FechaSolicitudEstudioSelectedDetalle
        {
            get { return _FechaSolicitudEstudioSelectedDetalle; }
            set { _FechaSolicitudEstudioSelectedDetalle = value; OnPropertyChanged("FechaSolicitudEstudioSelectedDetalle"); }
        }

        private short? _TipoMedidaSolicitudEstudioSelectedDetalle;
        public short? TipoMedidaSolicitudEstudioSelectedDetalle
        {
            get { return _TipoMedidaSolicitudEstudioSelectedDetalle; }
            set { _TipoMedidaSolicitudEstudioSelectedDetalle = value; OnPropertyChanged("TipoMedidaSolicitudEstudioSelectedDetalle"); }
        }


        private System.DateTime? _FechaInicioSolicitudEstudioSelectedDetalle;
        public System.DateTime? FechaInicioSolicitudEstudioSelectedDetalle
        {
            get { return _FechaInicioSolicitudEstudioSelectedDetalle; }
            set { _FechaInicioSolicitudEstudioSelectedDetalle = value; OnPropertyChanged("FechaInicioSolicitudEstudioSelectedDetalle"); }
        }

        private System.DateTime? _FechaFinSolicitudEstudioSelectedDetalle;
        public System.DateTime? FechaFinSolicitudEstudioSelectedDetalle
        {
            get { return _FechaFinSolicitudEstudioSelectedDetalle; }
            set { _FechaFinSolicitudEstudioSelectedDetalle = value; OnPropertyChanged("FechaFinSolicitudEstudioSelectedDetalle"); }
        }

        private System.DateTime? _FechaServer = null;
        public System.DateTime? FechaServer
        {
            get { return _FechaServer; }
            set { _FechaServer = value; OnPropertyChanged("FechaServer"); }
        }

        private System.DateTime _FechaInicioProgramacionEstudios;

        public System.DateTime FechaInicioProgramacionEstudios
        {
            get { return _FechaInicioProgramacionEstudios; }
            set { _FechaInicioProgramacionEstudios = value; OnPropertyChanged("FechaInicioProgramacionEstudios"); }
        }

        private System.DateTime _FechaFinProgramacionEstudios;

        public System.DateTime FechaFinProgramacionEstudios
        {
            get { return _FechaFinProgramacionEstudios; }
            set { _FechaFinProgramacionEstudios = value; OnPropertyChanged("FechaFinProgramacionEstudios"); }
        }
        private string _ResultadoSolicitudEstudioSelectedDetalle;
        public string ResultadoSolicitudEstudioSelectedDetalle
        {
            get { return _ResultadoSolicitudEstudioSelectedDetalle; }
            set { _ResultadoSolicitudEstudioSelectedDetalle = value; OnPropertyChanged("ResultadoSolicitudEstudioSelectedDetalle"); }
        }

        private short? _DiasBonificadosSolicitudEstudioSelectedDetalle;
        public short? DiasBonificadosSolicitudEstudioSelectedDetalle
        {
            get { return _DiasBonificadosSolicitudEstudioSelectedDetalle; }
            set { _DiasBonificadosSolicitudEstudioSelectedDetalle = value; OnPropertyChanged("DiasBonificadosSolicitudEstudioSelectedDetalle"); }
        }

        private byte[] _EstudioDigitalizadoSolicitudEstudioSelectedDetalle;
        public byte[] EstudioDigitalizadoSolicitudEstudioSelectedDetalle
        {
            get { return _EstudioDigitalizadoSolicitudEstudioSelectedDetalle; }
            set { _EstudioDigitalizadoSolicitudEstudioSelectedDetalle = value; OnPropertyChanged("EstudioDigitalizadoSolicitudEstudioSelectedDetalle"); }
        }

        #endregion

        private int _MaxLengthCaracteres = 100;
        public int MaxLengthCaracteres
        {
            get { return _MaxLengthCaracteres; }
            set { _MaxLengthCaracteres = value; OnPropertyChanged("MaxLengthCaracteres"); }
        }

        #region Configuracion Permisos
        private bool pInsertar = true;
        public bool PInsertar
        {
            get { return pInsertar; }
            set { pInsertar = value; }
        }

        private bool pEditar = true;
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

        private bool pImprimir = true;
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

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INGRESO> lstCandidatos;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INGRESO> LstCandidatos
        {
            get { return lstCandidatos; }
            set { lstCandidatos = value; OnPropertyChanged("LstCandidatos"); }
        }

        private bool dTFechaValid = false;
        public bool DTFechaValid
        {
            get { return dTFechaValid; }
            set { dTFechaValid = value; OnPropertyChanged("DTFechaValid"); }
        }

        private System.DateTime? dTFecha;
        public System.DateTime? DTFecha
        {
            get { return dTFecha; }
            set
            {
                dTFecha = value;
                if (value != null)
                {
                    DTFechaValid = true;
                    OnPropertyValidateChanged("DTFecha");
                }
                else
                {
                    DTFechaValid = false;
                    OnPropertyValidateChanged("DTFecha");
                }
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD> lstEstudiosConfirmados;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD> LstEstudiosConfirmados
        {
            get { return lstEstudiosConfirmados; }
            set { lstEstudiosConfirmados = value; OnPropertyChanged("LstEstudiosConfirmados"); }
        }

        private SSP.Servidor.PERSONALIDAD _SelectedEstudioConfirmado;

        public SSP.Servidor.PERSONALIDAD SelectedEstudioConfirmado
        {
            get { return _SelectedEstudioConfirmado; }
            set 
            { 
                _SelectedEstudioConfirmado = value;
                if (value != null)
                    ProcesaEstudiosByPersonalidad(value);

                OnPropertyChanged("SelectedEstudioConfirmado");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.AREA> lstArea;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.AREA> LstArea
        {
            get { return lstArea; }
            set { lstArea = value; OnPropertyChanged("LstArea"); }
        }

        private SSP.Servidor.INGRESO _SelectedImpu;

        public SSP.Servidor.INGRESO SelectedImpu
        {
            get { return _SelectedImpu; }
            set
            {  //ListEstudiosPersonalidad
                _SelectedImpu = value;
                if (value != null)
                    ProcesaEstudiosByImputado(value.ID_IMPUTADO, value.ID_INGRESO, value.ID_CENTRO, value.ID_ANIO);

                OnPropertyChanged("SelectedImpu");
            }
        }

        private string _NombreM = "Estudios de Personalidad";
        public string NombreM
        {
            get { return _NombreM; }
            set { _NombreM = value; OnPropertyChanged("NombreM"); }
        }

        private bool _VisibleModalidadEstudiosPersonalidad = true;
        public bool VisibleModalidadEstudiosPersonalidad
        {
            get { return _VisibleModalidadEstudiosPersonalidad; }
            set 
            { 
                _VisibleModalidadEstudiosPersonalidad = value;
                if (value)
                {
                    NombreM = "Estudios de Personalidad";
                    InicializaListaEstudiosPersonalidad();
                }
                else
                {
                    NombreM = "Cierre de Estudios de Personalidad";
                    InicializaCierreEstudiosPersonalidad();
                };

                OnPropertyChanged("VisibleModalidadEstudiosPersonalidad");
            }
        }

        #region Cierre de Estudios de Personalidad
        private string _NoOficioBusquedaEstudiosT;

        public string NoOficioBusquedaEstudiosT
        {
            get { return _NoOficioBusquedaEstudiosT; }
            set { _NoOficioBusquedaEstudiosT = value; OnPropertyChanged("NoOficioBusquedaEstudiosT"); }
        }

        private System.DateTime? _FechaInicioBusquedaEstudiosT = Fechas.GetFechaDateServer;

        public System.DateTime? FechaInicioBusquedaEstudiosT
        {
            get { return _FechaInicioBusquedaEstudiosT; }
            set { _FechaInicioBusquedaEstudiosT = value; OnPropertyChanged("FechaInicioBusquedaEstudiosT"); }
        }

        private System.DateTime? _FechaFinBusquedaEstudiosT = Fechas.GetFechaDateServer;

        public System.DateTime? FechaFinBusquedaEstudiosT
        {
            get { return _FechaFinBusquedaEstudiosT; }
            set { _FechaFinBusquedaEstudiosT = value; OnPropertyChanged("FechaFinBusquedaEstudiosT"); }
        }
        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD> lstEstudiosCerrdaos;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD> LstEstudiosCerrdaos
        {
            get { return lstEstudiosCerrdaos; }
            set { lstEstudiosCerrdaos = value; OnPropertyChanged("LstEstudiosCerrdaos"); }
        }

        private bool _EstudioTerminado = false;

        public bool EstudioTerminado
        {
            get { return _EstudioTerminado; }
            set { _EstudioTerminado = value; OnPropertyChanged("EstudioTerminado"); }
        }


        private SSP.Servidor.PERSONALIDAD _SelectedEstudioCerrado;
        public SSP.Servidor.PERSONALIDAD SelectedEstudioCerrado
        {
            get { return _SelectedEstudioCerrado; }
            set 
            {
                _SelectedEstudioCerrado = value; 
                OnPropertyChanged("SelectedEstudioCerrado");
            }
        }


        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD> lstTemporalEstudiosCerrar { get; set; }
        private string _TituloHeaderExpandirDescripcion;

        public string TituloHeaderExpandirDescripcion
        {
            get { return _TituloHeaderExpandirDescripcion; }
            set { _TituloHeaderExpandirDescripcion = value; OnPropertyChanged("TituloHeaderExpandirDescripcion"); }
        }

        private string _TextAmpliarDescripcion;

        public string TextAmpliarDescripcion
        {
            get { return _TextAmpliarDescripcion; }
            set { _TextAmpliarDescripcion = value; OnPropertyChanged("TextAmpliarDescripcion"); }
        }

        private int _MaxLengthAmpliarDescripcion = 50;

        public int MaxLengthAmpliarDescripcion
        {
            get { return _MaxLengthAmpliarDescripcion; }
            set { _MaxLengthAmpliarDescripcion = value; OnPropertyChanged("MaxLengthAmpliarDescripcion"); }
        }
        public class DatosEstatus
        {
            public string NombreEstatus { get; set; }
            public string Opcion { get; set; }
        }

        private System.Collections.ObjectModel.ObservableCollection<DatosEstatus> _LstEstatusEstudio;
        public System.Collections.ObjectModel.ObservableCollection<DatosEstatus> LstEstatusEstudio
        {
            get { return _LstEstatusEstudio; }
            set { _LstEstatusEstudio = value; OnPropertyChanged("LstEstatusEstudio"); }
        }

        private DatosEstatus _SelectedEstatus;
        public DatosEstatus SelectedEstatus
        {
            get { return _SelectedEstatus; }
            set 
            {
                _SelectedEstatus = value; 
                OnPropertyChanged("SelectedEstatus"); 
            }
        }

        #endregion

        private System.Collections.ObjectModel.ObservableCollection<eEstatus> lstEstatus;

        public System.Collections.ObjectModel.ObservableCollection<eEstatus> LstEstatus
        {
            get { return lstEstatus; }
            set { lstEstatus = value; OnPropertyChanged("LstEstatus"); }
        }
        public class eEstatus
        {
            public string Descripcion { get; set; }
            public string Opcion { get; set; }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_DETALLE_DIAS> lstDiasProgramados;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PERSONALIDAD_DETALLE_DIAS> LstDiasProgramados
        {
            get { return lstDiasProgramados; }
            set { lstDiasProgramados = value; OnPropertyChanged("LstDiasProgramados"); }
        }

        private SSP.Servidor.PERSONALIDAD_DETALLE_DIAS _SeledtedDiaProgramado;

        public SSP.Servidor.PERSONALIDAD_DETALLE_DIAS SeledtedDiaProgramado
        {
            get { return _SeledtedDiaProgramado; }
            set { _SeledtedDiaProgramado = value; OnPropertyChanged("SeledtedDiaProgramado"); }
        }

        private SSP.Servidor.AREA _SelectedAreaEdicionFechas;

        public SSP.Servidor.AREA SelectedAreaEdicionFechas
        {
            get { return _SelectedAreaEdicionFechas; }
            set { _SelectedAreaEdicionFechas = value; OnPropertyChanged("SelectedAreaEdicionFechas"); }
        }

        #region CAMBIO A DINAMICA DE CAPTURA DE FECHAS PARA DESARROLLO DE ESTUDIOS DE PERSONALIDAD
        private string _NombreTipoEstudio;

        public string NombreTipoEstudio
        {
            get { return _NombreTipoEstudio; }
            set { _NombreTipoEstudio = value; OnPropertyChanged("NombreTipoEstudio"); }
        }

        private System.DateTime? _FechaInicioEstudioDetalle;

        public System.DateTime? FechaInicioEstudioDetalle
        {
            get { return _FechaInicioEstudioDetalle; }
            set { _FechaInicioEstudioDetalle = value; OnPropertyChanged("FechaInicioEstudioDetalle"); }
        }

        private System.DateTime? _FechaFinEstudioDetalle;

        public System.DateTime? FechaFinEstudioDetalle
        {
            get { return _FechaFinEstudioDetalle; }
            set { _FechaFinEstudioDetalle = value; OnPropertyChanged("FechaFinEstudioDetalle"); }
        }

        private System.DateTime? _fechaMinimaMargenFin;

        public System.DateTime? FechaMinimaMargenFin
        {
            get { return _fechaMinimaMargenFin; }
            set { _fechaMinimaMargenFin = value; OnPropertyChanged("FechaMinimaMargenFin"); }
        }

        private System.DateTime? _FechaMaximaMargenFin;

        public System.DateTime? FechaMaximaMargenFin
        {
            get { return _FechaMaximaMargenFin; }
            set { _FechaMaximaMargenFin = value; OnPropertyChanged("FechaMaximaMargenFin"); }
        }

        private System.DateTime? _MargenMinimoHoraFin;

        public System.DateTime? MargenMinimoHoraFin
        {
            get { return _MargenMinimoHoraFin; }
            set { _MargenMinimoHoraFin = value; OnPropertyChanged("MargenMinimoHoraFin"); }
        }

        private System.DateTime? _MargenMaximoHoraFin;

        public System.DateTime? MargenMaximoHoraFin
        {
            get { return _MargenMaximoHoraFin; }
            set { _MargenMaximoHoraFin = value; OnPropertyChanged("MargenMaximoHoraFin"); }
        }
        private System.DateTime? _FechaSeleccionadaInicioDetalleP;

        public System.DateTime? FechaSeleccionadaInicioDetalleP
        {
            get { return _FechaSeleccionadaInicioDetalleP; }
            set 
            { 
                _FechaSeleccionadaInicioDetalleP = value;
                if(value.HasValue)
                {
                    HorasSeleccionadasFechaFinDesarrolloP = FechaSeleccionadaFinDetalleP = null;//la limpia en el supuesto que se modifique el margen y esta no lo cunmpla
                    FechaMinimaMargenFin = value;
                    OnPropertyChanged("HorasSeleccionadasFechaFinDesarrolloP");
                    OnPropertyChanged("FechaSeleccionadaFinDetalleP");
                    OnPropertyChanged("FechaMinimaMargenFin");
                }

                OnPropertyChanged("FechaSeleccionadaInicioDetalleP");
            }
        }

        private System.DateTime? _FechaSeleccionadaFinDetalleP;

        public System.DateTime? FechaSeleccionadaFinDetalleP
        {
            get { return _FechaSeleccionadaFinDetalleP; }
            set { _FechaSeleccionadaFinDetalleP = value; OnPropertyChanged("FechaSeleccionadaFinDetalleP"); }
        }

        private System.DateTime? _HorasSeleccionadasFechaInicioDesarrolloP;

        public System.DateTime? HorasSeleccionadasFechaInicioDesarrolloP
        {
            get { return _HorasSeleccionadasFechaInicioDesarrolloP; }
            set 
            {
                _HorasSeleccionadasFechaInicioDesarrolloP = value; 
                if(value.HasValue)
                {
                    MargenMinimoHoraFin = value;
                    HorasSeleccionadasFechaFinDesarrolloP = null;
                    OnPropertyChanged("MargenMinimoHoraFin");
                    OnPropertyChanged("HorasSeleccionadasFechaFinDesarrolloP");
                }

                OnPropertyChanged("HorasSeleccionadasFechaInicioDesarrolloP");
            }
        }

        private System.DateTime? _HorasSeleccionadasFechaFinDesarrolloP;

        public System.DateTime? HorasSeleccionadasFechaFinDesarrolloP
        {
            get { return _HorasSeleccionadasFechaFinDesarrolloP; }
            set { _HorasSeleccionadasFechaFinDesarrolloP = value; OnPropertyChanged("HorasSeleccionadasFechaFinDesarrolloP"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.AREA> lstAreasD;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.AREA> LstAreasD
        {
            get { return lstAreasD; }
            set { lstAreasD = value; OnPropertyChanged("LstAreasD"); }
        }

        private short? _SelectedAreaDetalleP = -1;

        public short? SelectedAreaDetalleP
        {
            get { return _SelectedAreaDetalleP; }
            set { _SelectedAreaDetalleP = value; OnPropertyChanged("SelectedAreaDetalleP"); }
        }

        #endregion
    }
}