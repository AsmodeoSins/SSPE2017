using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class CreacionListasExamenPViewModel
    {
        #region Datos Generales
        private PdfViewer PDFViewer;
        private SSP.Servidor.SOCIOECONOMICO _Estudio;
        public SSP.Servidor.SOCIOECONOMICO Estudio
        {
            get { return _Estudio; }
            set { _Estudio = value; OnPropertyChanged("Estudio"); }
        }

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
        private DateTime? fecIngresoD;
        public DateTime? FecIngresoD
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

        private RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set { listExpediente = value; OnPropertyChanged("ListExpediente"); }
        }

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }
        private bool emptyIngresoVisible;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private int indexMenu;
        public int IndexMenu
        {
            get { return indexMenu; }
            set { indexMenu = value; OnPropertyChanged("IndexMenu"); }
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
                    if (selectExpediente.INGRESO!=null && selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = selectExpediente.INGRESO.OrderBy(o => o.FEC_INGRESO_CERESO).FirstOrDefault();
                    }
                    else
                        EmptyIngresoVisible = true;

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
                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;

                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();

                OnPropertyChanged("SelectIngreso");
            }
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

        private enum eTiposAduana
        {
            VISITA = 3
        };

        private enum eFrecuencia
        {
            SEMANAL = 4,
            QUINCENAL = 3,
            MENSUAL = 2,
            ANUAL = 1,
            SIN_DATO = 0
        };

        #endregion

        #region Busqueda e Imagenes de Imputado
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
            set { nombreBuscar = value; OnPropertyChanged("NombreBuscar"); }
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

        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }
        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        #endregion

        private enum eRolesProcedimiento
        {
            COORDINADOR_JURIDICO = 30
        }

        private ObservableCollection<SSP.Servidor.IMPUTADO> lstImputados;

        public ObservableCollection<SSP.Servidor.IMPUTADO> LstImputados
        {
            get { return lstImputados; }
            set { lstImputados = value; OnPropertyChanged("LstImputados"); }
        }

        private ObservableCollection<SentenciaIngreso> lstSentenciasIngresos;
        public ObservableCollection<SentenciaIngreso> LstSentenciasIngresos
        {
            get { return lstSentenciasIngresos; }
            set { lstSentenciasIngresos = value; OnPropertyChanged("LstSentenciasIngresos"); }
        }

        private ObservableCollection<SSP.Servidor.PERSONALIDAD> lstEstudiosPersonalidad;

        public ObservableCollection<SSP.Servidor.PERSONALIDAD> LstEstudiosPersonalidad
        {
            get { return lstEstudiosPersonalidad; }
            set { lstEstudiosPersonalidad = value; OnPropertyChanged("LstEstudiosPersonalidad"); }
        }

        private int _CantidadFichas { get; set; }
        private ObservableCollection<PERSONALIDAD_MOTIVO> lstBeneficios;
        public ObservableCollection<PERSONALIDAD_MOTIVO> LstBeneficios
        {
            get { return lstBeneficios; }
            set { lstBeneficios = value; OnPropertyChanged("LstBeneficios"); }
        }

        private SSP.Servidor.PERSONALIDAD selectedEstudio;

        public SSP.Servidor.PERSONALIDAD SelectedEstudio
        {
            get { return selectedEstudio; }
            set { selectedEstudio = value; OnPropertyChanged("SelectedEstudio"); }
        }

        private short? _IdMotivoE;

        public short? IdMotivoE
        {
            get { return _IdMotivoE; }
            set { _IdMotivoE = value; OnPropertyChanged("IdMotivoE"); }
        }

        public short? _UltimoValor { get; set; }
        public string _DatoU { get; set; }
        public string _DatoD { get; set; }
        private enum eEstatusCausaPenalListas
        {
            ACTIVO = 1,
            SUSPENDIDO = 2,
            SOBRESEIDO = 3,
            CONCLUIDO = 4,
            POR_COMPURGAR = 0,
            INACTIVO = 5,
            EN_PROCESO = 6
        };



        #region Ficha de Idenfiticacion Juridica
        private string _TramiteTrasladoVoluntario;

        public string TramiteTrasladoVoluntario
        {
            get { return _TramiteTrasladoVoluntario; }
            set { _TramiteTrasladoVoluntario = value; OnPropertyChanged("TramiteTrasladoVoluntario"); }
        }

        private string _TramiteDiagnostico;

        public string TramiteDiagnostico
        {
            get { return _TramiteDiagnostico; }
            set { _TramiteDiagnostico = value; OnPropertyChanged("TramiteDiagnostico"); }
        }

        private string _ProcesosPendientes;

        public string ProcesosPendientes
        {
            get { return _ProcesosPendientes; }
            set { _ProcesosPendientes = value; OnPropertyChanged("ProcesosPendientes"); }
        }

        private string _ResolucionAprobado;

        public string ResolucionAprobado
        {
            get { return _ResolucionAprobado; }
            set { _ResolucionAprobado = value; OnPropertyChanged("ResolucionAprobado"); }
        }

        private string _TramiteDetalle;

        public string TramiteDetalle
        {
            get { return _TramiteDetalle; }
            set { _TramiteDetalle = value; OnPropertyChanged("TramiteDetalle"); }
        }
        private string _ResolucionAplazado;

        public string ResolucionAplazado
        {
            get { return _ResolucionAplazado; }
            set { _ResolucionAplazado = value; OnPropertyChanged("ResolucionAplazado"); }
        }

        private string _NoOficioEstudio;

        public string NoOficioEstudio
        {
            get { return _NoOficioEstudio; }
            set { _NoOficioEstudio = value; OnPropertyChanged("NoOficioEstudio"); }
        }

        private string _ResolucionMayoria;

        public string ResolucionMayoria
        {
            get { return _ResolucionMayoria; }
            set { _ResolucionMayoria = value; OnPropertyChanged("ResolucionMayoria"); }
        }

        private string _ResolucionUnanimidad;

        public string ResolucionUnanimidad
        {
            get { return _ResolucionUnanimidad; }
            set { _ResolucionUnanimidad = value; OnPropertyChanged("ResolucionUnanimidad"); }
        }

        private string _TramiteLibertad;

        public string TramiteLibertad
        {
            get { return _TramiteLibertad; }
            set { _TramiteLibertad = value; OnPropertyChanged("TramiteLibertad"); }
        }

        private string _TipoTramite;

        public string TipoTramite
        {
            get { return _TipoTramite; }
            set { _TipoTramite = value; OnPropertyChanged("TipoTramite"); }
        }
        private string _TramiteModificacion;

        public string TramiteModificacion
        {
            get { return _TramiteModificacion; }
            set { _TramiteModificacion = value; OnPropertyChanged("TramiteModificacion"); }
        }

        private string _TramiteTraslado;

        public string TramiteTraslado
        {
            get { return _TramiteTraslado; }
            set { _TramiteTraslado = value; OnPropertyChanged("TramiteTraslado"); }
        }

        private DateTime? _FichaFecha;

        public DateTime? FichaFecha
        {
            get { return _FichaFecha; }
            set { _FichaFecha = value; OnPropertyChanged("FichaFecha"); }
        }

        private DateTime? _FechaUltimoExamen;

        public DateTime? FechaUltimoExamen
        {
            get { return _FechaUltimoExamen; }
            set { _FechaUltimoExamen = value; OnPropertyChanged("FechaUltimoExamen"); }
        }

        private string _OficioEstudioSolicitado;

        public string OficioEstudioSolicitado
        {
            get { return _OficioEstudioSolicitado; }
            set { _OficioEstudioSolicitado = value; OnPropertyChanged("OficioEstudioSolicitado"); }
        }

        private string _JefeDepto;

        public string JefeDepto
        {
            get { return _JefeDepto; }
            set { _JefeDepto = value; OnPropertyChanged("JefeDepto"); }
        }


        private string _Elaboro;

        public string Elaboro
        {
            get { return _Elaboro; }
            set { _Elaboro = value; OnPropertyChanged("Elaboro"); }
        }

        private string _DptoJuridico;

        public string DptoJuridico
        {
            get { return _DptoJuridico; }
            set { _DptoJuridico = value; OnPropertyChanged("DptoJuridico"); }
        }

        private string _CriminoDinamia;

        public string CriminoDinamia
        {
            get { return _CriminoDinamia; }
            set { _CriminoDinamia = value; OnPropertyChanged("CriminoDinamia"); }
        }

        private string _NombreFicha;

        public string NombreFicha
        {
            get { return _NombreFicha; }
            set { _NombreFicha = value; OnPropertyChanged("NombreFicha"); }
        }

        private string _AliasFicha;

        public string AliasFicha
        {
            get { return _AliasFicha; }
            set { _AliasFicha = value; OnPropertyChanged("AliasFicha"); }
        }

        private string _EdadFicha;

        public string EdadFicha
        {
            get { return _EdadFicha; }
            set { _EdadFicha = value; OnPropertyChanged("EdadFicha"); }
        }

        private string _LugarOrigenFicha;

        public string LugarOrigenFicha
        {
            get { return _LugarOrigenFicha; }
            set { _LugarOrigenFicha = value; OnPropertyChanged("LugarOrigenFicha"); }
        }

        private DateTime? _FecNacimientoFicha;

        public DateTime? FecNacimientoFicha
        {
            get { return _FecNacimientoFicha; }
            set { _FecNacimientoFicha = value; OnPropertyChanged("FecNacimientoFicha"); }
        }

        private string _NacionalidadFicha;

        public string NacionalidadFicha
        {
            get { return _NacionalidadFicha; }
            set { _NacionalidadFicha = value; OnPropertyChanged("NacionalidadFicha"); }
        }

        private string _DomicilioFicha;

        public string DomicilioFicha
        {
            get { return _DomicilioFicha; }
            set { _DomicilioFicha = value; OnPropertyChanged("DomicilioFicha"); }
        }
        private string _EscolaridadFicha;

        public string EscolaridadFicha
        {
            get { return _EscolaridadFicha; }
            set { _EscolaridadFicha = value; OnPropertyChanged("EscolaridadFicha"); }
        }
        private string _UbicacionFicha;

        public string UbicacionFicha
        {
            get { return _UbicacionFicha; }
            set { _UbicacionFicha = value; OnPropertyChanged("UbicacionFicha"); }
        }

        private string _EdoCivilFicha;

        public string EdoCivilFicha
        {
            get { return _EdoCivilFicha; }
            set { _EdoCivilFicha = value; OnPropertyChanged("EdoCivilFicha"); }
        }

        private string _OcupacionFicha;

        public string OcupacionFicha
        {
            get { return _OcupacionFicha; }
            set { _OcupacionFicha = value; OnPropertyChanged("OcupacionFicha"); }
        }

        private string _DelitoFicha;

        public string DelitoFicha
        {
            get { return _DelitoFicha; }
            set { _DelitoFicha = value; OnPropertyChanged("DelitoFicha"); }
        }

        private string _ProcesosFicha;

        public string ProcesosFicha
        {
            get { return _ProcesosFicha; }
            set { _ProcesosFicha = value; OnPropertyChanged("ProcesosFicha"); }
        }

        private string _JuzgadoFicha;

        public string JuzgadoFicha
        {
            get { return _JuzgadoFicha; }
            set { _JuzgadoFicha = value; OnPropertyChanged("JuzgadoFicha"); }
        }

        private string _SentenciaFicha;

        public string SentenciaFicha
        {
            get { return _SentenciaFicha; }
            set { _SentenciaFicha = value; OnPropertyChanged("SentenciaFicha"); }
        }

        private string _APartirDeFicha;

        public string APartirDeFicha
        {
            get { return _APartirDeFicha; }
            set { _APartirDeFicha = value; OnPropertyChanged("APartirDeFicha"); }
        }

        private string _CausoEjecFicha;

        public string CausoEjecFicha
        {
            get { return _CausoEjecFicha; }
            set { _CausoEjecFicha = value; OnPropertyChanged("CausoEjecFicha"); }
        }

        private string _PorcentPenaCompur;

        public string PorcentPenaCompur
        {
            get { return _PorcentPenaCompur; }
            set { _PorcentPenaCompur = value; OnPropertyChanged("PorcentPenaCompur"); }
        }

        private DateTime? _FecIngresoFicha;

        public DateTime? FecIngresoFicha
        {
            get { return _FecIngresoFicha; }
            set { _FecIngresoFicha = value; OnPropertyChanged("FecIngresoFicha"); }
        }

        private string _ProcedenteDeFicha;

        public string ProcedenteDeFicha
        {
            get { return _ProcedenteDeFicha; }
            set { _ProcedenteDeFicha = value; OnPropertyChanged("ProcedenteDeFicha"); }
        }

        private string _ClasifJuridFicha;

        public string ClasifJuridFicha
        {
            get { return _ClasifJuridFicha; }
            set { _ClasifJuridFicha = value; OnPropertyChanged("ClasifJuridFicha"); }
        }

        #endregion

        private PERSONALIDAD _SelectedSolicitud;

        public PERSONALIDAD SelectedSolicitud
        {
            get { return _SelectedSolicitud; }
            set
            {
                _SelectedSolicitud = value;
                //if (value != null)
                //    ProcesaDatosImputadoSeleccionado();

                OnPropertyChanged("SelectedSolicitud");
            }
        }

        private string _NoOficio;

        public string NoOficio
        {
            get { return _NoOficio; }
            set { _NoOficio = value; OnPropertyChanged("NoOficio"); }
        }

        private string _NombrePrograma;

        public string NombrePrograma
        {
            get { return _NombrePrograma; }
            set { _NombrePrograma = value; OnPropertyChanged("NombrePrograma"); }
        }

        private bool _VisibleModalidadComun = true;

        public bool VisibleModalidadComun
        {
            get { return _VisibleModalidadComun; }
            set
            {
                _VisibleModalidadComun = value;
                if (value)
                {
                    NombreM = "Creación de listas";
                    InicializaCreacionListas();
                }
                else
                {
                    NombreM = "Dictamen Final";
                    InicializaDictamen();
                }
                OnPropertyChanged("VisibleModalidadComun");
            }
        }


        private enum eModuloActual
        {
            //COMUN
            CREACION_LISTAS = 1,
            DICTAMEN_FINAL = 2
        }

        private short _ModuloActual = 1;

        public short ModuloActual
        {
            get { return _ModuloActual; }
            set { _ModuloActual = value; OnPropertyChanged("ModuloActual"); }
        }
        private string _NombreM = "Creación de Listas";

        public string NombreM
        {
            get { return _NombreM; }
            set { _NombreM = value; OnPropertyChanged("NombreM"); }
        }

        private ObservableCollection<PERSONALIDAD> _lstEstudiosTerminados;
        public ObservableCollection<PERSONALIDAD> LstEstudiosTerminados
        {
            get { return _lstEstudiosTerminados; }
            set { _lstEstudiosTerminados = value; OnPropertyChanged("LstEstudiosTerminados"); }
        }


        private PERSONALIDAD _SelectedEstudioTerminado;

        public PERSONALIDAD SelectedEstudioTerminado
        {
            get { return _SelectedEstudioTerminado; }
            set { _SelectedEstudioTerminado = value; OnPropertyChanged("SelectedEstudioTerminado"); }
        }

        public short _TrasladoActua { get; set; }
        private enum eTipoSolicitudTraslado
        {//enumerador en base ala tabla PERSONALIDAD_MOTIVO
            ISLAS = 5,
            INTERNACIONAL = 4,
            NACIONAL = 3
        };

        private ObservableCollection<PFC_V_CAPACIDAD> lstCapacidadTraslados;

        public ObservableCollection<PFC_V_CAPACIDAD> LstCapacidadTraslados
        {
            get { return lstCapacidadTraslados; }
            set { lstCapacidadTraslados = value; OnPropertyChanged("LstCapacidadTraslados"); }
        }

        private ObservableCollection<PFC_V_PELIGROSIDAD> lstPeligrosidadTraslados;

        public ObservableCollection<PFC_V_PELIGROSIDAD> LstPeligrosidadTraslados
        {
            get { return lstPeligrosidadTraslados; }
            set { lstPeligrosidadTraslados = value; OnPropertyChanged("LstPeligrosidadTraslados"); }
        }

        private TXTextControl.WPF.TextControl editor;
        public TXTextControl.WPF.TextControl Editor
        {
            get { return editor; }
            set { editor = value; OnPropertyChanged("Editor"); }
        }

        #region Acta
        private string _LugarActaComun;

        public string LugarActaComun
        {
            get { return _LugarActaComun; }
            set { _LugarActaComun = value; OnPropertyChanged("LugarActaComun"); }
        }

        private string _NombrePresidenteActaComun;

        public string NombrePresidenteActaComun
        {
            get { return _NombrePresidenteActaComun; }
            set { _NombrePresidenteActaComun = value; OnPropertyChanged("NombrePresidenteActaComun"); }
        }

        private string _NombreSecretarioActaComun;

        public string NombreSecretarioActaComun
        {
            get { return _NombreSecretarioActaComun; }
            set { _NombreSecretarioActaComun = value; OnPropertyChanged("NombreSecretarioActaComun"); }
        }

        private string _NombreJuridicoActaComun;

        public string NombreJuridicoActaComun
        {
            get { return _NombreJuridicoActaComun; }
            set { _NombreJuridicoActaComun = value; OnPropertyChanged("NombreJuridicoActaComun"); }
        }

        private string _NombreMedicoActaComun;

        public string NombreMedicoActaComun
        {
            get { return _NombreMedicoActaComun; }
            set { _NombreMedicoActaComun = value; OnPropertyChanged("NombreMedicoActaComun"); }
        }

        private string _NombrePsiccoActaComun;

        public string NombrePsiccoActaComun
        {
            get { return _NombrePsiccoActaComun; }
            set { _NombrePsiccoActaComun = value; OnPropertyChanged("NombrePsiccoActaComun"); }
        }

        private string _NombreCriminologiaActaComun;

        public string NombreCriminologiaActaComun
        {
            get { return _NombreCriminologiaActaComun; }
            set { _NombreCriminologiaActaComun = value; OnPropertyChanged("NombreCriminologiaActaComun"); }
        }

        private string _NombreTrabajoSocialActaComun;

        public string NombreTrabajoSocialActaComun
        {
            get { return _NombreTrabajoSocialActaComun; }
            set { _NombreTrabajoSocialActaComun = value; OnPropertyChanged("NombreTrabajoSocialActaComun"); }
        }

        private string _NombreEducativoActaComun;

        public string NombreEducativoActaComun
        {
            get { return _NombreEducativoActaComun; }
            set { _NombreEducativoActaComun = value; OnPropertyChanged("NombreEducativoActaComun"); }
        }

        private string _NombreAreaLaboralActaComun;

        public string NombreAreaLaboralActaComun
        {
            get { return _NombreAreaLaboralActaComun; }
            set { _NombreAreaLaboralActaComun = value; OnPropertyChanged("NombreAreaLaboralActaComun"); }
        }

        private string _NombreSeguridadActaComun;

        public string NombreSeguridadActaComun
        {
            get { return _NombreSeguridadActaComun; }
            set { _NombreSeguridadActaComun = value; OnPropertyChanged("NombreSeguridadActaComun"); }
        }

        private string _NombreInternoActaComun;

        public string NombreInternoActaComun
        {
            get { return _NombreInternoActaComun; }
            set { _NombreInternoActaComun = value; OnPropertyChanged("NombreInternoActaComun"); }
        }

        private string _AcuerdoActaComun;

        public string AcuerdoActaComun
        {
            get { return _AcuerdoActaComun; }
            set { _AcuerdoActaComun = value; OnPropertyChanged("AcuerdoActaComun"); }
        }

        private string _OpinionMedico;

        public string OpinionMedico
        {
            get { return _OpinionMedico; }
            set { _OpinionMedico = value; OnPropertyChanged("OpinionMedico"); }
        }

        private string _OpinionPsico;

        public string OpinionPsico
        {
            get { return _OpinionPsico; }
            set { _OpinionPsico = value; OnPropertyChanged("OpinionPsico"); }
        }

        private string _OpinionTrabSocial;

        public string OpinionTrabSocial
        {
            get { return _OpinionTrabSocial; }
            set { _OpinionTrabSocial = value; OnPropertyChanged("OpinionTrabSocial"); }
        }

        private string _OpinionSeguridad;

        public string OpinionSeguridad
        {
            get { return _OpinionSeguridad; }
            set { _OpinionSeguridad = value; OnPropertyChanged("OpinionSeguridad"); }
        }

        private string _OpinionLaboral;

        public string OpinionLaboral
        {
            get { return _OpinionLaboral; }
            set { _OpinionLaboral = value; OnPropertyChanged("OpinionLaboral"); }
        }

        private string _OpinionEscolar;

        public string OpinionEscolar
        {
            get { return _OpinionEscolar; }
            set { _OpinionEscolar = value; OnPropertyChanged("OpinionEscolar"); }
        }

        private string _OpinionCrimi;

        public string OpinionCrimi
        {
            get { return _OpinionCrimi; }
            set { _OpinionCrimi = value; OnPropertyChanged("OpinionCrimi"); }
        }

        private string _ManifestaronActaComun;

        public string ManifestaronActaComun
        {
            get { return _ManifestaronActaComun; }
            set { _ManifestaronActaComun = value; OnPropertyChanged("ManifestaronActaComun"); }
        }

        private string _OpinionActaComun;

        public string OpinionActaComun
        {
            get { return _OpinionActaComun; }
            set { _OpinionActaComun = value; OnPropertyChanged("OpinionActaComun"); }
        }

        private string _ActuacionActaComun;

        public string ActuacionActaComun
        {
            get { return _ActuacionActaComun; }
            set { _ActuacionActaComun = value; OnPropertyChanged("ActuacionActaComun"); }
        }
        #endregion

        #region Archivero
        public class Archivero
        {
            public string NombreArchivo { get; set; }
            public string Disponible { get; set; }
            public short TipoArchivo { get; set; }
            public bool VisibleVerDocumentoArchivero { get; set; }
        }

        private ObservableCollection<Archivero> _LstDocumentos;

        public ObservableCollection<Archivero> LstDocumentos
        {
            get { return _LstDocumentos; }
            set { _LstDocumentos = value; OnPropertyChanged("LstDocumentos"); }
        }

        private Archivero _SelectedDocumento;

        public Archivero SelectedDocumento
        {
            get { return _SelectedDocumento; }
            set
            {
                _SelectedDocumento = value;
                OnPropertyChanged("SelectedDocumento");
            }
        }

        private enum eSituacionActual
        {
            STAGE0 = 0,//APENAS SE ESTA GENERANDO LA LISTA, SE PUEDE CONSULTAR LA FICHA SIGNALETICA Y LA PARTIDA JURIDICA
            STAGE1 = 1,//YA SE GENERO LA FICHA JURIDICA Y EL OFICIO DE PETICION DE REALIZACION DE ESTUDIOS DE PERSONALIDAD
            STAGE2 = 2,//YA SE HICIERON LOS ESTUDIOS DE PERSONALIDAD, SE INCLUYE EL OFICIO QUE HACE AREAS TECNICAS HACIA JURIDICO
            STAGE3 = 3 //SE HACE EL ACTA, EL OFICIO DE REMISION DE LOS ESTUDIOS DE PERSONALIDAD CON SU DICTAMEN, EL DICTAMEN INDIVIDUAL
        };

        /// <summary>
        /// CONDENSADO DE LOS ARCHIVOS USADOS DENTRO DEL PROCESO DE LOS ESTUDIOS DE PERSONALIDAD DE FUERO COMUN Y FUERO FEDERAL, PROCESO JURIDICO.
        /// </summary>
        private enum eDocumentoMostrado
        {
            PARTIDA_JURIDICA = 0,
            FICHA_SIGNALETICA = 1,
            FICHA_JURIDICA = 2,
            ACTA_CONSEJO_TECNICO = 3,
            DICTAMEN_INDIVIDUAL = 4,
            PERSONALIDAD_COMUN_MEDICO = 5,
            PERSONALIDAD_COMUN_PSIQ = 6,
            PERSONALIDAD_COMUN_PSICO = 7,
            PERSONALIDAD_COMUN_CRIMI = 8,
            PERSONALIDAD_COMUN_SOCIO_FAM = 9,
            PERSONALIDAD_COMUN_EDUC = 10,
            PERSONALIDAD_COMUN_CAPAC = 11,
            PERSONALIDAD_COMUN_SEGURIDAD = 12,
            ACTA_FEDERAL = 13,
            PERSONALIDAD_FEDERAL_MEDICO = 14,
            PERSONALIDAD_FEDERAL_PSICO = 15,
            PERSONALIDAD_FEDERAL_TRABAJO_SOCIAL = 16,
            PERSONALIDAD_FEDERAL_CAPAC = 17,
            PERSONALIDAD_FEDERAL_EDUCATIVAS = 18,
            PERSONALIDAD_FEDERAL_VIGILANCIA = 19,
            PERSONALIDAD_FEDERAL_CRIMINOLOGICO = 20,
            OFICIO_PETICION_REALIZACION_ESTUDIOS_PERSONALIDAD = 21,
            OFICIO_REMISION_GENERAL_DICTAMEN = 22,
            OFICIO_REMISION_APROBADO_APROBADO_MAYORIA = 23,
            TRASLADO_NACIONAL = 24,
            TRASLADO_INTERNACIONAL = 25,
            TRASLADO_ISLAS = 26,
            REMISION_CIERRE = 27,
            REMISION_DPMJ = 28
        };

        #endregion

        #region ACTA FEDERAL
        #region Acta de consejo tecnico interdisciplinario
        private string _NombreImputadoFF;
        public string NombreImputadoFF
        {
            get { return _NombreImputadoFF; }
            set { _NombreImputadoFF = value; OnPropertyChanged("NombreImputadoFF"); }
        }

        private string _ExpedienteImputadoFF;
        public string ExpedienteImputadoFF
        {
            get { return _ExpedienteImputadoFF; }
            set { _ExpedienteImputadoFF = value; OnPropertyChanged("ExpedienteImputadoFF"); }
        }

        private string _Delito;
        public string Delito
        {
            get { return _Delito; }
            set { _Delito = value; OnPropertyChanged("Delito"); }
        }
        private string _Sentencia;

        public string Sentencia
        {
            get { return _Sentencia; }
            set { _Sentencia = value; OnPropertyChanged("Sentencia"); }
        }
        private string _APartirDe;

        public string APartirDe
        {
            get { return _APartirDe; }
            set { _APartirDe = value; OnPropertyChanged("APartirDe"); }
        }
        private DateTime? _EnSesionDeFecha;

        public DateTime? EnSesionDeFecha
        {
            get { return _EnSesionDeFecha; }
            set { _EnSesionDeFecha = value; OnPropertyChanged("EnSesionDeFecha"); }
        }
        private short? _IdEstado;

        public short? IdEstado
        {
            get { return _IdEstado; }
            set { _IdEstado = value; OnPropertyChanged("IdEstado"); }
        }

        private string _EstadoActual;
        public string EstadoActual
        {
            get { return _EstadoActual; }
            set { _EstadoActual = value; OnPropertyChanged("EstadoActual"); }
        }

        private short? _IdCentro;
        public short? IdCentro
        {
            get { return _IdCentro; }
            set { _IdCentro = value; OnPropertyChanged("IdCentro"); }
        }

        private string _CentroActual;
        public string CentroActual
        {
            get { return _CentroActual; }
            set { _CentroActual = value; OnPropertyChanged("CentroActual"); }
        }

        System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.CENTRO> _lstCentros;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.CENTRO> LstCentros
        {
            get { return _lstCentros; }
            set { _lstCentros = value; OnPropertyChanged("LstCentros"); }
        }

        ObservableCollection<AREA_TECNICA> _lstAreas;
        public ObservableCollection<AREA_TECNICA> LstAreas
        {
            get { return _lstAreas; }
            set { _lstAreas = value; OnPropertyChanged("LstAreas"); }
        }


        private AREA_TECNICA _SelArea;
        public AREA_TECNICA SelArea
        {
            get { return _SelArea; }
            set { _SelArea = value; OnPropertyChanged("SelArea"); }
        }

        System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PFF_ACTA_DETERMINO> _lstAreasTec;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PFF_ACTA_DETERMINO> LstAreasTec
        {
            get { return _lstAreasTec; }
            set { _lstAreasTec = value; OnPropertyChanged("LstAreasTec"); }
        }

        private PFF_ACTA_DETERMINO _SelectedAreTec;
        public PFF_ACTA_DETERMINO SelectedAreTec
        {
            get { return _SelectedAreTec; }
            set { _SelectedAreTec = value; OnPropertyChanged("SelectedAreTec"); }
        }


        private string _DirectorCentro;
        public string DirectorCentro
        {
            get { return _DirectorCentro; }
            set { _DirectorCentro = value; OnPropertyChanged("DirectorCentro"); }
        }

        private string _ActuacionR;
        public string ActuacionR
        {
            get { return _ActuacionR; }
            set { _ActuacionR = value; OnPropertyChanged("ActuacionR"); }
        }

        private string _VotosR;
        public string VotosR
        {
            get { return _VotosR; }
            set { _VotosR = value; OnPropertyChanged("VotosR"); }
        }

        private string _LugarActa;

        public string LugarActa
        {
            get { return _LugarActa; }
            set { _LugarActa = value; OnPropertyChanged("LugarActa"); }
        }

        private string _TramiteDescripcion;
        public string TramiteDescripcion
        {
            get { return _TramiteDescripcion; }
            set { _TramiteDescripcion = value; OnPropertyChanged("TramiteDescripcion"); }
        }

        private string _NombreDir;
        public string NombreDir
        {
            get { return _NombreDir; }
            set { _NombreDir = value; OnPropertyChanged("NombreDir"); }
        }

        private DateTime? _FechaActa;
        public DateTime? FechaActa
        {
            get { return _FechaActa; }
            set { _FechaActa = value; OnPropertyChanged("FechaActa"); }
        }

        #region Datos de coordinadores de areas

        private short _IdAreaT;
        public short IdAreaT
        {
            get { return _IdAreaT; }
            set { _IdAreaT = value; OnPropertyChanged("IdAreaT"); }
        }

        private bool _IsEnabledCoordinadorArea = true;
        public bool IsEnabledCoordinadorArea
        {
            get { return _IsEnabledCoordinadorArea; }
            set { _IsEnabledCoordinadorArea = value; OnPropertyChanged("IsEnabledCoordinadorArea"); }
        }

        private string _NombreAreaMedica;

        public string NombreAreaMedica
        {
            get { return _NombreAreaMedica; }
            set { _NombreAreaMedica = value; OnPropertyChanged("NombreAreaMedica"); }
        }
        private string _OpinionAreaMedica;

        public string OpinionAreaMedica
        {
            get { return _OpinionAreaMedica; }
            set { _OpinionAreaMedica = value; OnPropertyChanged("OpinionAreaMedica"); }
        }
        private string _NombreAreaPsiquiatrica;

        public string NombreAreaPsiquiatrica
        {
            get { return _NombreAreaPsiquiatrica; }
            set { _NombreAreaPsiquiatrica = value; OnPropertyChanged("NombreAreaPsiquiatrica"); }
        }
        private string _OpinionAreaPsiquiatrica;

        public string OpinionAreaPsiquiatrica
        {
            get { return _OpinionAreaPsiquiatrica; }
            set { _OpinionAreaPsiquiatrica = value; OnPropertyChanged("OpinionAreaPsiquiatrica"); }
        }
        private string _NombreAreaPsicologica;

        public string NombreAreaPsicologica
        {
            get { return _NombreAreaPsicologica; }
            set { _NombreAreaPsicologica = value; OnPropertyChanged("NombreAreaPsicologica"); }
        }
        private string _OpinionAreaPsicologica;

        public string OpinionAreaPsicologica
        {
            get { return _OpinionAreaPsicologica; }
            set { _OpinionAreaPsicologica = value; OnPropertyChanged("OpinionAreaPsicologica"); }
        }
        private string _NombreAreaTS;

        public string NombreAreaTS
        {
            get { return _NombreAreaTS; }
            set { _NombreAreaTS = value; OnPropertyChanged("NombreAreaTS"); }
        }
        private string _OpinionAreaTS;

        public string OpinionAreaTS
        {
            get { return _OpinionAreaTS; }
            set { _OpinionAreaTS = value; OnPropertyChanged("OpinionAreaTS"); }
        }
        private string _NombreAreaEscolar;

        public string NombreAreaEscolar
        {
            get { return _NombreAreaEscolar; }
            set { _NombreAreaEscolar = value; OnPropertyChanged("NombreAreaEscolar"); }
        }
        private string _OpinionAreaEscolar;

        public string OpinionAreaEscolar
        {
            get { return _OpinionAreaEscolar; }
            set { _OpinionAreaEscolar = value; OnPropertyChanged("OpinionAreaEscolar"); }
        }
        private string _NombreAreaInd;

        public string NombreAreaInd
        {
            get { return _NombreAreaInd; }
            set { _NombreAreaInd = value; OnPropertyChanged("NombreAreaInd"); }
        }
        private string _OpinionAreaInd;

        public string OpinionAreaInd
        {
            get { return _OpinionAreaInd; }
            set { _OpinionAreaInd = value; OnPropertyChanged("OpinionAreaInd"); }
        }
        private string _NombreAreaVigDisc;

        public string NombreAreaVigDisc
        {
            get { return _NombreAreaVigDisc; }
            set { _NombreAreaVigDisc = value; OnPropertyChanged("NombreAreaVigDisc"); }
        }
        private string _OpinionAreaVigDisc;

        public string OpinionAreaVigDisc
        {
            get { return _OpinionAreaVigDisc; }
            set { _OpinionAreaVigDisc = value; OnPropertyChanged("OpinionAreaVigDisc"); }
        }
        private string _NombreAreaCrim;

        public string NombreAreaCrim
        {
            get { return _NombreAreaCrim; }
            set { _NombreAreaCrim = value; OnPropertyChanged("NombreAreaCrim"); }
        }
        private string _OpinionAreaCrim;

        public string OpinionAreaCrim
        {
            get { return _OpinionAreaCrim; }
            set { _OpinionAreaCrim = value; OnPropertyChanged("OpinionAreaCrim"); }
        }
        private string _NombreAreaJur;

        public string NombreAreaJur
        {
            get { return _NombreAreaJur; }
            set { _NombreAreaJur = value; OnPropertyChanged("NombreAreaJur"); }
        }
        private string _OpinionAreaJur;

        public string OpinionAreaJur
        {
            get { return _OpinionAreaJur; }
            set { _OpinionAreaJur = value; OnPropertyChanged("OpinionAreaJur"); }
        }
        private string _NombreCRS;

        public string NombreCRS
        {
            get { return _NombreCRS; }
            set { _NombreCRS = value; OnPropertyChanged("NombreCRS"); }
        }
        private short? _IdActuacion;

        public short? IdActuacion
        {
            get { return _IdActuacion; }
            set { _IdActuacion = value; OnPropertyChanged("IdActuacion"); }
        }
        private short? _IdVotacion;

        public short? IdVotacion
        {
            get { return _IdVotacion; }
            set { _IdVotacion = value; OnPropertyChanged("IdVotacion"); }
        }
        private string _Tramite;

        public string Tramite
        {
            get { return _Tramite; }
            set { _Tramite = value; OnPropertyChanged("Tramite"); }
        }
        private DateTime _FechaTramite;

        public DateTime FechaTramite
        {
            get { return _FechaTramite; }
            set { _FechaTramite = value; OnPropertyChanged("FechaTramite"); }
        }

        private string _LugarActaFF;

        public string LugarActaFF
        {
            get { return _LugarActaFF; }
            set { _LugarActaFF = value; OnPropertyChanged("LugarActaFF"); }
        }

        private ObservableCollection<OCUPACION> lstOcupaciones;

        public ObservableCollection<OCUPACION> LstOcupaciones
        {
            get { return lstOcupaciones; }
            set { lstOcupaciones = value; OnPropertyChanged("LstOcupaciones"); }
        }

        private ObservableCollection<DIALECTO> lstDialectos;

        public ObservableCollection<DIALECTO> LstDialectos
        {
            get { return lstDialectos; }
            set { lstDialectos = value; OnPropertyChanged("LstDialectos"); }
        }

        private ObservableCollection<TIPO_REFERENCIA> lstParentescos;
        public ObservableCollection<TIPO_REFERENCIA> LstParentescos
        {
            get { return lstParentescos; }
            set { lstParentescos = value; OnPropertyChanged("LstParentescos"); }
        }

        private ObservableCollection<COLONIA> listItems;
        public ObservableCollection<COLONIA> ListItems
        {
            get { return listItems; }
            set { listItems = value; OnPropertyChanged("ListItems"); }
        }
        private ObservableCollection<PAIS_NACIONALIDAD> lstPaises;
        public ObservableCollection<PAIS_NACIONALIDAD> LstPaises
        {
            get { return lstPaises; }
            set { lstPaises = value; OnPropertyChanged("LstPaises"); }
        }
        private ObservableCollection<ENTIDAD> lstEntidades;
        public ObservableCollection<ENTIDAD> LstEntidades
        {
            get { return lstEntidades; }
            set { lstEntidades = value; OnPropertyChanged("LstEntidades"); }
        }
        private ObservableCollection<MUNICIPIO> lstMunicipios;
        public ObservableCollection<MUNICIPIO> LstMunicipios
        {
            get { return lstMunicipios; }
            set { lstMunicipios = value; OnPropertyChanged("LstMunicipios"); }
        }

        private ObservableCollection<COLONIA> listColonia;
        public ObservableCollection<COLONIA> ListColonia
        {
            get { return listColonia; }
            set { listColonia = value; OnPropertyChanged("ListColonia"); }
        }

        private ObservableCollection<ESTADO_CIVIL> _LstEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> LstEstadoCivil
        {
            get { return _LstEstadoCivil; }
            set { _LstEstadoCivil = value; OnPropertyChanged("LstEstadoCivil"); }
        }

        private ObservableCollection<ESTADO_CIVIL> listEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> ListEstadoCivil
        {
            get { return listEstadoCivil; }
            set { listEstadoCivil = value; OnPropertyChanged("ListEstadoCivil"); }
        }
        #endregion

        #endregion

        #endregion

        #region MODIFICACION ACTA
        private bool _TramiteLibertadAntic = false;

        public bool TramiteLibertadAntic
        {
            get { return _TramiteLibertadAntic; }
            set { _TramiteLibertadAntic = value; OnPropertyChanged("TramiteLibertadAntic"); }
        }

        private bool _TramiteMod = false;

        public bool TramiteMod
        {
            get { return _TramiteMod; }
            set { _TramiteMod = value; OnPropertyChanged("TramiteMod"); }
        }
        private bool _TramiteDiagn = false;

        public bool TramiteDiagn
        {
            get { return _TramiteDiagn; }
            set { _TramiteDiagn = value; OnPropertyChanged("TramiteDiagn"); }
        }
        private bool _TramiteTr = false;

        public bool TramiteTr
        {
            get { return _TramiteTr; }
            set { _TramiteTr = value; OnPropertyChanged("TramiteTr"); OnPropertyChanged("TramiteTr"); }
        }
        private bool _TramiteTraslVol = false;

        public bool TramiteTraslVol
        {
            get { return _TramiteTraslVol; }
            set { _TramiteTraslVol = value; OnPropertyChanged("TramiteTraslVol"); }
        }

        #endregion

        #region Trasla2
        private string _CriminogTrasladoIslas;

        public string CriminogTrasladoIslas
        {
            get { return _CriminogTrasladoIslas; }
            set { _CriminogTrasladoIslas = value; OnPropertyChanged("CriminogTrasladoIslas"); }
        }

        private short? _IdEgocentrismoTrasladoIslas;

        public short? IdEgocentrismoTrasladoIslas
        {
            get { return _IdEgocentrismoTrasladoIslas; }
            set { _IdEgocentrismoTrasladoIslas = value; OnPropertyChanged("IdEgocentrismoTrasladoIslas"); }
        }
        private short? _IdLabAfecTrasladoIslas;

        public short? IdLabAfecTrasladoIslas
        {
            get { return _IdLabAfecTrasladoIslas; }
            set { _IdLabAfecTrasladoIslas = value; OnPropertyChanged("IdLabAfecTrasladoIslas"); }
        }

        private string _EstadoAdiccionTrasladoIslas;

        public string EstadoAdiccionTrasladoIslas
        {
            get { return _EstadoAdiccionTrasladoIslas; }
            set { _EstadoAdiccionTrasladoIslas = value; OnPropertyChanged("EstadoAdiccionTrasladoIslas"); }
        }

        private string _DictamenTrasladoIslas;

        public string DictamenTrasladoIslas
        {
            get { return _DictamenTrasladoIslas; }
            set { _DictamenTrasladoIslas = value; OnPropertyChanged("DictamenTrasladoIslas"); }
        }

        private short? _IdAgresivIntrTrasladoIslas;

        public short? IdAgresivIntrTrasladoIslas
        {
            get { return _IdAgresivIntrTrasladoIslas; }
            set { _IdAgresivIntrTrasladoIslas = value; OnPropertyChanged("IdAgresivIntrTrasladoIslas"); }
        }

        private short? _IdPeligroTrasladoIslas;

        public short? IdPeligroTrasladoIslas
        {
            get { return _IdPeligroTrasladoIslas; }
            set { _IdPeligroTrasladoIslas = value; OnPropertyChanged("IdPeligroTrasladoIslas"); }
        }

        private string _ContinTratamTrasladoIslas;

        public string ContinTratamTrasladoIslas
        {
            get { return _ContinTratamTrasladoIslas; }
            set { _ContinTratamTrasladoIslas = value; OnPropertyChanged("ContinTratamTrasladoIslas"); }
        }


        private string _TieneAnuenciaTrasladoIslas;

        public string TieneAnuenciaTrasladoIslas
        {
            get { return _TieneAnuenciaTrasladoIslas; }
            set { _TieneAnuenciaTrasladoIslas = value; OnPropertyChanged("TieneAnuenciaTrasladoIslas"); }
        }

        private string _TratamSugTrasladoIslas;

        public string TratamSugTrasladoIslas
        {
            get { return _TratamSugTrasladoIslas; }
            set { _TratamSugTrasladoIslas = value; OnPropertyChanged("TratamSugTrasladoIslas"); }
        }

        private DateTime? _FechaTrasladoIslas;

        public DateTime? FechaTrasladoIslas
        {
            get { return _FechaTrasladoIslas; }
            set { _FechaTrasladoIslas = value; OnPropertyChanged("FechaTrasladoIslas"); }
        }

        private string _IntimidacionPenaTrasladoIslas;

        public string IntimidacionPenaTrasladoIslas
        {
            get { return _IntimidacionPenaTrasladoIslas; }
            set { _IntimidacionPenaTrasladoIslas = value; OnPropertyChanged("IntimidacionPenaTrasladoIslas"); }
        }

        #region TRASLADO NACIONAL
        private short? _IdPeligrosidadTrasladoNacional;

        public short? IdPeligrosidadTrasladoNacional
        {
            get { return _IdPeligrosidadTrasladoNacional; }
            set { _IdPeligrosidadTrasladoNacional = value; OnPropertyChanged("IdPeligrosidadTrasladoNacional"); }
        }

        private string _AdicToxTrasladoNacional;

        public string AdicToxTrasladoNacional
        {
            get { return _AdicToxTrasladoNacional; }
            set
            {
                _AdicToxTrasladoNacional = value;
                if (string.IsNullOrEmpty(value))
                {
                    base.RemoveRule("EspecifiqueToxicosTrasladoNacional");
                    OnPropertyChanged("EspecifiqueToxicosTrasladoNacional");
                }
                else
                {
                    if (value == "S")
                    {
                        EnabledCualesTox = true;
                        base.RemoveRule("EspecifiqueToxicosTrasladoNacional");
                        base.AddRule(() => EspecifiqueToxicosTrasladoNacional, () => !string.IsNullOrEmpty(EspecifiqueToxicosTrasladoNacional), "ESPECIFIQUE TOXICOS ES REQUERIDO!");
                        OnPropertyChanged("EspecifiqueToxicosTrasladoNacional");
                    }

                    if (value == "N")
                    {
                        EnabledCualesTox = false;
                        base.RemoveRule("EspecifiqueToxicosTrasladoNacional");
                        OnPropertyChanged("EspecifiqueToxicosTrasladoNacional");
                    }
                }

                OnPropertyChanged("EspecifiqueToxicosTrasladoNacional");
                OnPropertyChanged("AdicToxTrasladoNacional");
            }
        }

        private bool _EnabledCualesTox = false;
        public bool EnabledCualesTox
        {
            get { return _EnabledCualesTox; }
            set { _EnabledCualesTox = value; OnPropertyChanged("EnabledCualesTox"); }
        }
        private string _EspecifiqueToxicosTrasladoNacional;

        public string EspecifiqueToxicosTrasladoNacional
        {
            get { return _EspecifiqueToxicosTrasladoNacional; }
            set { _EspecifiqueToxicosTrasladoNacional = value; OnPropertyChanged("EspecifiqueToxicosTrasladoNacional"); }
        }

        private bool _IsPsicoTrasladoNacionalChecked;

        public bool IsPsicoTrasladoNacionalChecked
        {
            get { return _IsPsicoTrasladoNacionalChecked; }
            set { _IsPsicoTrasladoNacionalChecked = value; OnPropertyChanged("IsPsicoTrasladoNacionalChecked"); }
        }

        private bool _IsEducTrasladoNacionalChecked;

        public bool IsEducTrasladoNacionalChecked
        {
            get { return _IsEducTrasladoNacionalChecked; }
            set { _IsEducTrasladoNacionalChecked = value; OnPropertyChanged("IsEducTrasladoNacionalChecked"); }
        }

        private bool _IsLabTrasladoNacionalChecked;

        public bool IsLabTrasladoNacionalChecked
        {
            get { return _IsLabTrasladoNacionalChecked; }
            set { _IsLabTrasladoNacionalChecked = value; OnPropertyChanged("IsLabTrasladoNacionalChecked"); }
        }

        //private bool _IsOtrosTrasladoNacionalChecked;

        //public bool IsOtrosTrasladoNacionalChecked
        //{
        //    get { return _IsOtrosTrasladoNacionalChecked; }
        //    set { _IsOtrosTrasladoNacionalChecked = value; OnPropertyChanged("IsOtrosTrasladoNacionalChecked"); }
        //}

        private string _EspecifiqueOtroTratamientoTrasladoNacional;

        public string EspecifiqueOtroTratamientoTrasladoNacional
        {
            get { return _EspecifiqueOtroTratamientoTrasladoNacional; }
            set { _EspecifiqueOtroTratamientoTrasladoNacional = value; OnPropertyChanged("EspecifiqueOtroTratamientoTrasladoNacional"); }
        }

        private string _EspecifiqueAspectosRelevantesTrasladoNacional;

        public string EspecifiqueAspectosRelevantesTrasladoNacional
        {
            get { return _EspecifiqueAspectosRelevantesTrasladoNacional; }
            set { _EspecifiqueAspectosRelevantesTrasladoNacional = value; OnPropertyChanged("EspecifiqueAspectosRelevantesTrasladoNacional"); }
        }

        private DateTime? _FechaTrasladoNacional;

        public DateTime? FechaTrasladoNacional
        {
            get { return _FechaTrasladoNacional; }
            set { _FechaTrasladoNacional = value; OnPropertyChanged("FechaTrasladoNacional"); }
        }

        #endregion

        #region TRASLADO INTERNACIONAL
        private string _NombreCentroOrigenTrasladoIntern;

        public string NombreCentroOrigenTrasladoIntern
        {
            get { return _NombreCentroOrigenTrasladoIntern; }
            set { _NombreCentroOrigenTrasladoIntern = value; OnPropertyChanged("NombreCentroOrigenTrasladoIntern"); }
        }

        private string _NombreCentroSolicitud;

        public string NombreCentroSolicitud
        {
            get { return _NombreCentroSolicitud; }
            set { _NombreCentroSolicitud = value; OnPropertyChanged("NombreCentroSolicitud"); }
        }

        private string _NombreTrasladoInternac;

        public string NombreTrasladoInternac
        {
            get { return _NombreTrasladoInternac; }
            set { _NombreTrasladoInternac = value; OnPropertyChanged("NombreTrasladoInternac"); }
        }

        private string _EdadTrasladoInternac;

        public string EdadTrasladoInternac
        {
            get { return _EdadTrasladoInternac; }
            set { _EdadTrasladoInternac = value; OnPropertyChanged("EdadTrasladoInternac"); }
        }

        private string _EdoCivilTrasladoInternac;

        public string EdoCivilTrasladoInternac
        {
            get { return _EdoCivilTrasladoInternac; }
            set { _EdoCivilTrasladoInternac = value; OnPropertyChanged("EdoCivilTrasladoInternac"); }
        }

        private string _OriginarioTrasladoInternac;

        public string OriginarioTrasladoInternac
        {
            get { return _OriginarioTrasladoInternac; }
            set { _OriginarioTrasladoInternac = value; OnPropertyChanged("OriginarioTrasladoInternac"); }
        }

        private string _LugarResidenciaTrasladoInternac;

        public string LugarResidenciaTrasladoInternac
        {
            get { return _LugarResidenciaTrasladoInternac; }
            set { _LugarResidenciaTrasladoInternac = value; OnPropertyChanged("LugarResidenciaTrasladoInternac"); }
        }

        private string _EscolaridadTrasladoInternac;

        public string EscolaridadTrasladoInternac
        {
            get { return _EscolaridadTrasladoInternac; }
            set { _EscolaridadTrasladoInternac = value; OnPropertyChanged("EscolaridadTrasladoInternac"); }
        }

        private string _OcupacionPreviaTrasladoInternac;

        public string OcupacionPreviaTrasladoInternac
        {
            get { return _OcupacionPreviaTrasladoInternac; }
            set { _OcupacionPreviaTrasladoInternac = value; OnPropertyChanged("OcupacionPreviaTrasladoInternac"); }
        }

        public class DelitosGrid
        {
            public string Delito { get; set; }
            public string Proceso { get; set; }
            public string FComun { get; set; }
            public string FFederal { get; set; }
            public string Pena { get; set; }
            public string APartir { get; set; }
        };

        private ObservableCollection<DelitosGrid> lstDelitosUno;

        public ObservableCollection<DelitosGrid> LstDelitosUno
        {
            get { return lstDelitosUno; }
            set { lstDelitosUno = value; OnPropertyChanged("LstDelitosUno"); }
        }

        private DelitosGrid _SelectedDelitoUno;

        public DelitosGrid SelectedDelitoUno
        {
            get { return _SelectedDelitoUno; }
            set { _SelectedDelitoUno = value; OnPropertyChanged("SelectedDelitoUno"); }
        }

        private string _AntecedentesPenalesTrasladoInternacional;

        public string AntecedentesPenalesTrasladoInternacional
        {
            get { return _AntecedentesPenalesTrasladoInternacional; }
            set { _AntecedentesPenalesTrasladoInternacional = value; OnPropertyChanged("AntecedentesPenalesTrasladoInternacional"); }
        }

        private ObservableCollection<DelitosGrid> lstDelitosDos;

        public ObservableCollection<DelitosGrid> LstDelitosDos
        {
            get { return lstDelitosDos; }
            set { lstDelitosDos = value; OnPropertyChanged("LstDelitosDos"); }
        }

        private DelitosGrid _SelectedDelitoDos;

        public DelitosGrid SelectedDelitoDos
        {
            get { return _SelectedDelitoDos; }
            set { _SelectedDelitoDos = value; OnPropertyChanged("SelectedDelitoDos"); }
        }

        private string _VersionDelitoTrasladoInternacional;

        public string VersionDelitoTrasladoInternacional
        {
            get { return _VersionDelitoTrasladoInternacional; }
            set { _VersionDelitoTrasladoInternacional = value; OnPropertyChanged("VersionDelitoTrasladoInternacional"); }
        }

        private bool _EnabledSanoIntern = false;

        public bool EnabledSanoIntern
        {
            get { return _EnabledSanoIntern; }
            set { _EnabledSanoIntern = value; OnPropertyChanged("EnabledSanoIntern"); }
        }

        private bool _EnabledNoVisitTrasladoInternacional = false;

        public bool EnabledNoVisitTrasladoInternacional
        {
            get { return _EnabledNoVisitTrasladoInternacional; }
            set { _EnabledNoVisitTrasladoInternacional = value; OnPropertyChanged("EnabledNoVisitTrasladoInternacional"); }
        }

        private string _ClinicSanoTrasladoInternacional;
        public string ClinicSanoTrasladoInternacional
        {
            get { return _ClinicSanoTrasladoInternacional; }
            set
            {
                _ClinicSanoTrasladoInternacional = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "N")
                    {
                        EnabledSanoIntern = true;
                        base.RemoveRule("IndicaPadecimientoTrasladoInternacional");
                        base.AddRule(() => IndicaPadecimientoTrasladoInternacional, () => !string.IsNullOrEmpty(IndicaPadecimientoTrasladoInternacional), "ESPECIFIQUE CLINICAMENTE SANO ES REQUERIDO!");
                        OnPropertyChanged("IndicaPadecimientoTrasladoInternacional");
                        OnPropertyChanged("EnabledSanoIntern");
                    }
                    else
                    {
                        base.RemoveRule("IndicaPadecimientoTrasladoInternacional");
                        OnPropertyChanged("IndicaPadecimientoTrasladoInternacional");
                        EnabledSanoIntern = false;
                        OnPropertyChanged("EnabledSanoIntern");
                    }
                }


                OnPropertyChanged("ClinicSanoTrasladoInternacional");
                OnPropertyChanged("EnabledSanoIntern");
                OnPropertyChanged("IndicaPadecimientoTrasladoInternacional");

            }
        }

        private string _IndicaPadecimientoTrasladoInternacional;

        public string IndicaPadecimientoTrasladoInternacional
        {
            get { return _IndicaPadecimientoTrasladoInternacional; }
            set { _IndicaPadecimientoTrasladoInternacional = value; OnPropertyChanged("IndicaPadecimientoTrasladoInternacional"); }
        }

        private string _TratamientoActualTrasladoInternacional;

        public string TratamientoActualTrasladoInternacional
        {
            get { return _TratamientoActualTrasladoInternacional; }
            set { _TratamientoActualTrasladoInternacional = value; OnPropertyChanged("TratamientoActualTrasladoInternacional"); }
        }

        private string _CoeficIntTrasladoInternacional;

        public string CoeficIntTrasladoInternacional
        {
            get { return _CoeficIntTrasladoInternacional; }
            set { _CoeficIntTrasladoInternacional = value; OnPropertyChanged("CoeficIntTrasladoInternacional"); }
        }

        private string _DanioCerebTrasladoInternacional;

        public string DanioCerebTrasladoInternacional
        {
            get { return _DanioCerebTrasladoInternacional; }
            set { _DanioCerebTrasladoInternacional = value; OnPropertyChanged("DanioCerebTrasladoInternacional"); }
        }

        private string _OtrosAspectosPersonalidadTrasladoInternacional;

        public string OtrosAspectosPersonalidadTrasladoInternacional
        {
            get { return _OtrosAspectosPersonalidadTrasladoInternacional; }
            set { _OtrosAspectosPersonalidadTrasladoInternacional = value; OnPropertyChanged("OtrosAspectosPersonalidadTrasladoInternacional"); }
        }

        private string _DescAgresividad;

        public string DescAgresividad
        {
            get { return _DescAgresividad; }
            set { _DescAgresividad = value; OnPropertyChanged("DescAgresividad"); }
        }
        private string _ApoyoPadresTrasladoInternacional;

        public string ApoyoPadresTrasladoInternacional
        {
            get { return _ApoyoPadresTrasladoInternacional; }
            set { _ApoyoPadresTrasladoInternacional = value; OnPropertyChanged("ApoyoPadresTrasladoInternacional"); }
        }

        private string _ConyugeTrasladoInternacional;

        public string ConyugeTrasladoInternacional
        {
            get { return _ConyugeTrasladoInternacional; }
            set { _ConyugeTrasladoInternacional = value; OnPropertyChanged("ConyugeTrasladoInternacional"); }
        }

        private ObservableCollection<TRASLADO_INTERNACIONAL_VISITA> lstVisitasTraslInt;

        public ObservableCollection<TRASLADO_INTERNACIONAL_VISITA> LstVisitasTraslInt
        {
            get { return lstVisitasTraslInt; }
            set { lstVisitasTraslInt = value; OnPropertyChanged("LstVisitasTraslInt"); }
        }

        private short? _DiasEsu;

        public short? DiasEsu
        {
            get { return _DiasEsu; }
            set { _DiasEsu = value; OnPropertyChanged("DiasEsu"); }
        }
        private TRASLADO_INTERNACIONAL_VISITA _SelectedVisitaTrasladoInt;

        public TRASLADO_INTERNACIONAL_VISITA SelectedVisitaTrasladoInt
        {
            get { return _SelectedVisitaTrasladoInt; }
            set { _SelectedVisitaTrasladoInt = value; OnPropertyChanged("SelectedVisitaTrasladoInt"); }
        }

        private string _FrecuenciaVisitaTrasladoInternacional;

        public string FrecuenciaVisitaTrasladoInternacional
        {
            get { return _FrecuenciaVisitaTrasladoInternacional; }
            set { _FrecuenciaVisitaTrasladoInternacional = value; OnPropertyChanged("FrecuenciaVisitaTrasladoInternacional"); }
        }

        private string _NoRecibeVisitaCausasTrasladoInternacional;

        public string NoRecibeVisitaCausasTrasladoInternacional
        {
            get { return _NoRecibeVisitaCausasTrasladoInternacional; }
            set { _NoRecibeVisitaCausasTrasladoInternacional = value; OnPropertyChanged("NoRecibeVisitaCausasTrasladoInternacional"); }
        }

        private bool _EnabledCarta = false;

        public bool EnabledCarta
        {
            get { return _EnabledCarta; }
            set { _EnabledCarta = value; OnPropertyChanged("EnabledCarta"); }
        }

        private string _CartaArraigoCuentaTrasladoInternacional;

        public string CartaArraigoCuentaTrasladoInternacional
        {
            get { return _CartaArraigoCuentaTrasladoInternacional; }
            set
            {
                _CartaArraigoCuentaTrasladoInternacional = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        EnabledCarta = true;
                        base.RemoveRule("DomicilioTrasladoInternacional");
                        base.AddRule(() => DomicilioTrasladoInternacional, () => !string.IsNullOrEmpty(DomicilioTrasladoInternacional), "ESPECIFIQUE DOMICILIO ES REQUERIDO!");
                        OnPropertyChanged("DomicilioTrasladoInternacional");
                        OnPropertyChanged("EnabledCarta");
                    }
                    else
                    {
                        base.RemoveRule("DomicilioTrasladoInternacional");
                        OnPropertyChanged("DomicilioTrasladoInternacional");
                        EnabledCarta = false;
                        OnPropertyChanged("EnabledCarta");
                    }
                }

                OnPropertyChanged("CartaArraigoCuentaTrasladoInternacional");
            }
        }

        private string _DomicilioTrasladoInternacional;

        public string DomicilioTrasladoInternacional
        {
            get { return _DomicilioTrasladoInternacional; }
            set { _DomicilioTrasladoInternacional = value; OnPropertyChanged("DomicilioTrasladoInternacional"); }
        }

        private string _AnuenciaCupoTrasladoInternacional;

        public string AnuenciaCupoTrasladoInternacional
        {
            get { return _AnuenciaCupoTrasladoInternacional; }
            set { _AnuenciaCupoTrasladoInternacional = value; OnPropertyChanged("AnuenciaCupoTrasladoInternacional"); }
        }

        private DateTime? _FechaAnuenciaTrasladoInternacional;

        public DateTime? FechaAnuenciaTrasladoInternacional
        {
            get { return _FechaAnuenciaTrasladoInternacional; }
            set { _FechaAnuenciaTrasladoInternacional = value; OnPropertyChanged("FechaAnuenciaTrasladoInternacional"); }
        }

        private string _NivelSocioETrasladoInternacional;

        public string NivelSocioETrasladoInternacional
        {
            get { return _NivelSocioETrasladoInternacional; }
            set { _NivelSocioETrasladoInternacional = value; OnPropertyChanged("NivelSocioETrasladoInternacional"); }
        }

        private bool _EnabledPorqueEstudia = false;

        public bool EnabledPorqueEstudia
        {
            get { return _EnabledPorqueEstudia; }
            set { _EnabledPorqueEstudia = value; OnPropertyChanged("EnabledPorqueEstudia"); }
        }

        private string _EstudiaActualTrasladoInternacional;
        public string EstudiaActualTrasladoInternacional
        {
            get { return _EstudiaActualTrasladoInternacional; }
            set
            {
                _EstudiaActualTrasladoInternacional = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "N")
                    {
                        EnabledPorqueEstudia = true;
                        base.RemoveRule("CausasNoEstudiaTrasladoInternacional");
                        base.AddRule(() => CausasNoEstudiaTrasladoInternacional, () => !string.IsNullOrEmpty(CausasNoEstudiaTrasladoInternacional), "CAUSAS DE NO TRABAJAR ES REQUERIDO!");

                        OnPropertyChanged("EnabledPorqueEstudia");
                        OnPropertyChanged("CausasNoEstudiaTrasladoInternacional");
                    }

                    if (value == "S")
                    {
                        EnabledPorqueEstudia = false;
                        base.RemoveRule("CausasNoEstudiaTrasladoInternacional");

                        OnPropertyChanged("EnabledPorqueEstudia");
                        OnPropertyChanged("CausasNoEstudiaTrasladoInternacional");
                    }
                }


                OnPropertyChanged("EstudiaActualTrasladoInternacional");
            }
        }

        private string _CausasNoEstudiaTrasladoInternacional;

        public string CausasNoEstudiaTrasladoInternacional
        {
            get { return _CausasNoEstudiaTrasladoInternacional; }
            set { _CausasNoEstudiaTrasladoInternacional = value; OnPropertyChanged("CausasNoEstudiaTrasladoInternacional"); }
        }

        private string _OtrosCursosCapacRecibTrasladoInternacional;

        public string OtrosCursosCapacRecibTrasladoInternacional
        {
            get { return _OtrosCursosCapacRecibTrasladoInternacional; }
            set { _OtrosCursosCapacRecibTrasladoInternacional = value; OnPropertyChanged("OtrosCursosCapacRecibTrasladoInternacional"); }
        }

        private string _AcualmenteTrabajaInstTrasladoInternacional;

        public string AcualmenteTrabajaInstTrasladoInternacional
        {
            get { return _AcualmenteTrabajaInstTrasladoInternacional; }
            set
            {
                _AcualmenteTrabajaInstTrasladoInternacional = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        EnabledSI = true;
                        EnabledNO = false;

                        base.RemoveRule("NegativoSenialeCausas");

                        base.RemoveRule("SenialeOcupacionTrasladoInternacional");
                        base.AddRule(() => SenialeOcupacionTrasladoInternacional, () => !string.IsNullOrEmpty(SenialeOcupacionTrasladoInternacional), "OCUPACIONES ES REQUERIDO!");

                        OnPropertyChanged("NegativoSenialeCausas");
                        OnPropertyChanged("SenialeOcupacionTrasladoInternacional");
                        OnPropertyChanged("EnabledNO");
                        OnPropertyChanged("EnabledSI");
                    }
                    else
                    {
                        EnabledSI = false;
                        EnabledNO = true;

                        base.RemoveRule("SenialeOcupacionTrasladoInternacional");

                        base.RemoveRule("NegativoSenialeCausas");
                        base.AddRule(() => NegativoSenialeCausas, () => !string.IsNullOrEmpty(NegativoSenialeCausas), "CAUSAS ES REQUERIDO!");

                        OnPropertyChanged("NegativoSenialeCausas");
                        OnPropertyChanged("SenialeOcupacionTrasladoInternacional");
                        OnPropertyChanged("EnabledNO");
                        OnPropertyChanged("EnabledSI");
                    }
                }
                else
                {
                    EnabledSI = false;
                    EnabledNO = false;
                    base.RemoveRule("NegativoSenialeCausas");
                    base.RemoveRule("SenialeOcupacionTrasladoInternacional");
                    OnPropertyChanged("NegativoSenialeCausas");
                    OnPropertyChanged("SenialeOcupacionTrasladoInternacional");
                    OnPropertyChanged("EnabledNO");
                    OnPropertyChanged("EnabledSI");
                }

                OnPropertyChanged("AcualmenteTrabajaInstTrasladoInternacional");
            }
        }

        private bool _EnabledSI = false;

        public bool EnabledSI
        {
            get { return _EnabledSI; }
            set { _EnabledSI = value; OnPropertyChanged("EnabledSI"); }
        }
        private bool _EnabledNO = false;

        public bool EnabledNO
        {
            get { return _EnabledNO; }
            set { _EnabledNO = value; OnPropertyChanged("EnabledNO"); }
        }

        private string _SenialeOcupacionTrasladoInternacional;

        public string SenialeOcupacionTrasladoInternacional
        {
            get { return _SenialeOcupacionTrasladoInternacional; }
            set { _SenialeOcupacionTrasladoInternacional = value; OnPropertyChanged("SenialeOcupacionTrasladoInternacional"); }
        }

        private string _NegativoSenialeCausas;

        public string NegativoSenialeCausas
        {
            get { return _NegativoSenialeCausas; }
            set { _NegativoSenialeCausas = value; OnPropertyChanged("NegativoSenialeCausas"); }
        }

        private short? _TotalDiasEfectivTrasladoInternacional;

        public short? TotalDiasEfectivTrasladoInternacional
        {
            get { return _TotalDiasEfectivTrasladoInternacional; }
            set { _TotalDiasEfectivTrasladoInternacional = value; OnPropertyChanged("TotalDiasEfectivTrasladoInternacional"); }
        }

        private short? _ConductaConsidTrasladoInternacional;

        public short? ConductaConsidTrasladoInternacional
        {
            get { return _ConductaConsidTrasladoInternacional; }
            set { _ConductaConsidTrasladoInternacional = value; OnPropertyChanged("ConductaConsidTrasladoInternacional"); }
        }

        private ObservableCollection<TRASLADO_INTERNACIONAL_SANCION> lstSancionesTrasladoInt;

        public ObservableCollection<TRASLADO_INTERNACIONAL_SANCION> LstSancionesTrasladoInt
        {
            get { return lstSancionesTrasladoInt; }
            set { lstSancionesTrasladoInt = value; OnPropertyChanged("LstSancionesTrasladoInt"); }
        }

        private TRASLADO_INTERNACIONAL_SANCION _SelectedSancionTrasladoIntern;

        public TRASLADO_INTERNACIONAL_SANCION SelectedSancionTrasladoIntern
        {
            get { return _SelectedSancionTrasladoIntern; }
            set { _SelectedSancionTrasladoIntern = value; OnPropertyChanged("SelectedSancionTrasladoIntern"); }
        }

        private short? _IdPeligroTrasladoInternacional;
        public short? IdPeligroTrasladoInternacional
        {
            get { return _IdPeligroTrasladoInternacional; }
            set { _IdPeligroTrasladoInternacional = value; OnPropertyChanged("IdPeligroTrasladoInternacional"); }
        }

        private string _AdiccionToxicosTrasladoInternacional;

        public string AdiccionToxicosTrasladoInternacional
        {
            get { return _AdiccionToxicosTrasladoInternacional; }
            set
            {
                _AdiccionToxicosTrasladoInternacional = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "S")
                    {
                        EnabledToxicosInt = true;
                        base.RemoveRule("CasoAfirmativoCualesTrasladoInternacional");
                        base.AddRule(() => CasoAfirmativoCualesTrasladoInternacional, () => !string.IsNullOrEmpty(CasoAfirmativoCualesTrasladoInternacional), "CUALES TOXICOS ES REQUERIDO!");
                        OnPropertyChanged("CasoAfirmativoCualesTrasladoInternacional");
                        OnPropertyChanged("EnabledToxicosInt");
                    }
                    else
                    {
                        base.RemoveRule("CasoAfirmativoCualesTrasladoInternacional");
                        OnPropertyChanged("CasoAfirmativoCualesTrasladoInternacional");
                        EnabledToxicosInt = false;
                        OnPropertyChanged("EnabledToxicosInt");
                    }
                }
                else
                {
                    EnabledToxicosInt = false;
                    base.RemoveRule("CasoAfirmativoCualesTrasladoInternacional");
                    OnPropertyChanged("CasoAfirmativoCualesTrasladoInternacional");
                    OnPropertyChanged("EnabledToxicosInt");
                }

                OnPropertyChanged("AdiccionToxicosTrasladoInternacional");
            }
        }

        private bool _EnabledToxicosInt = false;

        public bool EnabledToxicosInt
        {
            get { return _EnabledToxicosInt; }
            set { _EnabledToxicosInt = value; OnPropertyChanged("EnabledToxicosInt"); }
        }

        private string _CasoAfirmativoCualesTrasladoInternacional;

        public string CasoAfirmativoCualesTrasladoInternacional
        {
            get { return _CasoAfirmativoCualesTrasladoInternacional; }
            set { _CasoAfirmativoCualesTrasladoInternacional = value; OnPropertyChanged("CasoAfirmativoCualesTrasladoInternacional"); }
        }

        private bool _IsPsicoTrasladoInterNacionalChecked;

        public bool IsPsicoTrasladoInterNacionalChecked
        {
            get { return _IsPsicoTrasladoInterNacionalChecked; }
            set { _IsPsicoTrasladoInterNacionalChecked = value; OnPropertyChanged("IsPsicoTrasladoInterNacionalChecked"); }
        }

        private bool _IsEducTrasladoInterNacionalChecked;

        public bool IsEducTrasladoInterNacionalChecked
        {
            get { return _IsEducTrasladoInterNacionalChecked; }
            set { _IsEducTrasladoInterNacionalChecked = value; OnPropertyChanged("IsEducTrasladoInterNacionalChecked"); }
        }

        private bool _IsLabTrasladoInterNacionalChecked;

        public bool IsLabTrasladoInterNacionalChecked
        {
            get { return _IsLabTrasladoInterNacionalChecked; }
            set { _IsLabTrasladoInterNacionalChecked = value; OnPropertyChanged("IsLabTrasladoInterNacionalChecked"); }
        }

        private bool _IsOtrosTrasladoInterNacionalChecked;

        public bool IsOtrosTrasladoInterNacionalChecked
        {
            get { return _IsOtrosTrasladoInterNacionalChecked; }
            set { _IsOtrosTrasladoInterNacionalChecked = value; OnPropertyChanged("IsOtrosTrasladoInterNacionalChecked"); }
        }

        private string _EspecifiqueOtroTratamientoTrasladoInterNacional;

        public string EspecifiqueOtroTratamientoTrasladoInterNacional
        {
            get { return _EspecifiqueOtroTratamientoTrasladoInterNacional; }
            set { _EspecifiqueOtroTratamientoTrasladoInterNacional = value; OnPropertyChanged("EspecifiqueOtroTratamientoTrasladoInterNacional"); }
        }

        private string _OtrosAspectosTrasladoInternacional;

        public string OtrosAspectosTrasladoInternacional
        {
            get { return _OtrosAspectosTrasladoInternacional; }
            set { _OtrosAspectosTrasladoInternacional = value; OnPropertyChanged("OtrosAspectosTrasladoInternacional"); }
        }


        private string _NoOficioBusquedaDictamenFinal;

        public string NoOficioBusquedaDictamenFinal
        {
            get { return _NoOficioBusquedaDictamenFinal; }
            set { _NoOficioBusquedaDictamenFinal = value; OnPropertyChanged("NoOficioBusquedaDictamenFinal"); }
        }

        private DateTime? _FechaInicioBusquedaDictamenFinal = Fechas.GetFechaDateServer;

        public DateTime? FechaInicioBusquedaDictamenFinal
        {
            get { return _FechaInicioBusquedaDictamenFinal; }
            set { _FechaInicioBusquedaDictamenFinal = value; OnPropertyChanged("FechaInicioBusquedaDictamenFinal"); }
        }

        private DateTime? _FechaFinBusquedaDictamenFinal = Fechas.GetFechaDateServer;

        public DateTime? FechaFinBusquedaDictamenFinal
        {
            get { return _FechaFinBusquedaDictamenFinal; }
            set { _FechaFinBusquedaDictamenFinal = value; OnPropertyChanged("FechaFinBusquedaDictamenFinal"); }
        }
        #endregion
        #endregion


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

        private bool pConsultar = true;
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

        private bool _BuscarImputadoHabilitado = true;
        public bool BuscarImputadoHabilitado
        {
            get { return _BuscarImputadoHabilitado; }
            set { _BuscarImputadoHabilitado = value; OnPropertyChanged("BuscarImputadoHabilitado"); }
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
        private bool menuGuardarEnabled = true;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuBuscarEnabled = true;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        #endregion

        #region CAMBIO DE FUEROS
        private bool _SelectedFueroCreacionLista = false;//false es igual a comun
        public bool SelectedFueroCreacionLista
        {
            get { return _SelectedFueroCreacionLista; }
            set
            {
                _SelectedFueroCreacionLista = value;

                OnPropertyChanged("SelectedFueroCreacionLista");
                ProcesaCambioFueros();

                if (value)
                    EnabledIngresaDiasPlazoFederal = true;
                else
                    EnabledIngresaDiasPlazoFederal = false;
            }
        }

        private bool _EnabledIngresaDiasPlazoFederal;
        public bool EnabledIngresaDiasPlazoFederal
        {
            get { return _EnabledIngresaDiasPlazoFederal; }
            set { _EnabledIngresaDiasPlazoFederal = value; OnPropertyChanged("EnabledIngresaDiasPlazoFederal"); }
        }

        #endregion


        public enum eDatosRolesProcesosPersonalidad
        {
            //ESTE ENUMERADOR ES PARA AUTO DOCUMENTAR PROCESOS DE ESTUDIOS DE PERSONALIDAD, EN BASE ALA TABLA DEPARTAMENTO_ACCESO
            COORDINACION_TECNICA = 1,
            JURIDICO = 2,
            COMANDANCIA = 4
        }
    }
}