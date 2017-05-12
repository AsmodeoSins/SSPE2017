using System;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Collections.Generic;
using ControlPenales.Clases;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        public string Name
        {
            get
            {
                return "causas_penales";
            }
        }

        #region Datos Generales
        //DATOS
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

        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
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

        //ARBOL INGRESOS
        private List<TreeViewList> _TreeList;
        public List<TreeViewList> TreeList
        {
            get { return _TreeList; }
            set
            {
                _TreeList = value;
                OnPropertyChanged("TreeList");
            }
        }

        //ARBOL UBICACIONES
        private List<TreeViewList> _TreeListUbicacion;
        public List<TreeViewList> TreeListUbicacion
        {
            get { return _TreeListUbicacion; }
            set
            {
                _TreeListUbicacion = value;
                OnPropertyChanged("TreeListUbicacion");
            }
        }

        //ARBOL DELITOS
        private List<TreeViewList> _TreeListDelito;
        public List<TreeViewList> TreeListDelito
        {
            get { return _TreeListDelito; }
            set
            {
                _TreeListDelito = value;
                //OnPropertyChanged("TreeListDelito");
            }
        }

        private int indexMenu;
        public int IndexMenu
        {
            get { return indexMenu; }
            set { indexMenu = value; OnPropertyChanged("IndexMenu"); }
        }

        private ObservableCollection<INGRESO> lstBuscarIngreso;
        public ObservableCollection<INGRESO> LstBuscarIngreso
        {
            get { return lstBuscarIngreso; }
            set { lstBuscarIngreso = value; OnPropertyChanged("LstBuscarIngreso"); }
        }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                LstBuscarIngreso = new ObservableCollection<INGRESO>();
                if (selectExpediente != null)
                {
                    //OBTENERMOS LOS INGRESOS PERTENECIENTES AL CENTRO
                    if (selectExpediente.INGRESO != null)
                        LstBuscarIngreso = new ObservableCollection<INGRESO>(selectExpediente.INGRESO.Where(w => w.ID_UB_CENTRO == GlobalVar.gCentro));
                    else
                        LstBuscarIngreso = new ObservableCollection<INGRESO>();
                    //MUESTRA LOS INGRESOS
                    //if (selectExpediente.INGRESO.Count > 0)
                    if (LstBuscarIngreso!=null && LstBuscarIngreso.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = LstBuscarIngreso.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
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
                        ImagenIngreso = new Imagenes().getImagenPerson();
                }
                else
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    EmptyIngresoVisible = true;
                }
                OnPropertyChanged("SelectExpediente");
            }
        }
        
        

        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (value != null)
                    MostrarOpcion = value.ID_ESTATUS_ADMINISTRATIVO != 4 ? Visibility.Visible : Visibility.Collapsed;
                else
                    MostrarOpcion = Visibility.Collapsed;
             
                if (selectIngreso == null)
                {
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                    OnPropertyChanged("SelectIngreso");
                    return;
                }

                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    OnPropertyChanged("SelectIngreso");
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                #region comentado
                //else
                //    ImagenIngreso = new Imagenes().getImagenPerson();

                //if (selectIngreso == null)
                //    return;
                //if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                //    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                //else
                //    ImagenImputado = new Imagenes().getImagenPerson();
                //if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                //{
                //    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                //    OnPropertyChanged("SelectIngreso");
                //}
                //else
                //    ImagenIngreso = new Imagenes().getImagenPerson();
                #endregion
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

        //NO ELIMINAR SI YA ESTAN GRABADOS EN BASE DE DATOS
        private bool eliminarVisible;
        public bool EliminarVisible
        {
            get { return eliminarVisible; }
            set { eliminarVisible = value; OnPropertyChanged("EliminarVisible"); }
        }

        private bool causaPenalDelitoEmpty = true;
        public bool CausaPenalDelitoEmpty
        {
            get { return causaPenalDelitoEmpty; }
            set { causaPenalDelitoEmpty = value; OnPropertyChanged("CausaPenalDelitoEmpty"); }
        }
    
    
        //LISTADO FECHAS TRASLAPE
        private ObservableCollection<EmpalmeFechas> lstFechasTraslape;
        public ObservableCollection<EmpalmeFechas> LstFechasTraslape
        {
            get { return lstFechasTraslape; }
            set { lstFechasTraslape = value; OnPropertyChanged("LstFechasTraslape"); }
        }

        private string empalmeDescr;
        public string EmpalmeDescr
        {
            get { return empalmeDescr; }
            set { empalmeDescr = value; OnPropertyChanged("EmpalmeDescr"); }
        }

        private bool enCausaPenal = false;
        public bool EnCausaPenal
        {
            get { return enCausaPenal; }
            set { enCausaPenal = value; }
        }
        #endregion

        #region Baja
        private ObservableCollection<LIBERACION_MOTIVO> lstLiberacionMotivo;
        public ObservableCollection<LIBERACION_MOTIVO> LstLiberacionMotivo
        {
            get { return lstLiberacionMotivo; }
            set { lstLiberacionMotivo = value; OnPropertyChanged("LstLiberacionMotivo"); }
        }

        private short bliberacionMotivo = -1;
        public short BLiberacionMotivo
        {
            get { return bliberacionMotivo; }
            set { bliberacionMotivo = value; OnPropertyChanged("BLiberacionMotivo"); }
        }

        private ObservableCollection<LIBERACION_AUTORIDAD> lstLiberacionAutoridad;
        public ObservableCollection<LIBERACION_AUTORIDAD> LstLiberacionAutoridad
        {
            get { return lstLiberacionAutoridad; }
            set { lstLiberacionAutoridad = value; OnPropertyChanged("LstLiberacionAutoridad"); }
        }

        private short bliberacionAutoridad = -1;
        public short BLiberacionAutoridad
        {
            get { return bliberacionAutoridad; }
            set { bliberacionAutoridad = value; OnPropertyChanged("BLiberacionAutoridad"); }
        }

        private DateTime? bFecha = null;
        public DateTime? BFecha
        {
            get { return bFecha; }
            set { bFecha = value; OnPropertyChanged("BFecha"); }
        }


        //private ObservableCollection<CAUSA_PENAL_BAJA> lstMotivoBaja;
        //public ObservableCollection<CAUSA_PENAL_BAJA> LstMotivoBaja
        //{
        //    get { return lstMotivoBaja; }
        //    set { lstMotivoBaja = value; OnPropertyChanged("LstMotivoBaja"); }
        //}

        //private ObservableCollection<CAUSA_PENAL_AUTORIDAD_BAJA> lstAutoridadBaja;
        //public ObservableCollection<CAUSA_PENAL_AUTORIDAD_BAJA> LstAutoridadBaja
        //{
        //    get { return lstAutoridadBaja; }
        //    set { lstAutoridadBaja = value; OnPropertyChanged("LstAutoridadBaja"); }
        //}

        //private short? bMotivoBaja = -1;
        //public short? BMotivoBaja
        //{
        //    get { return bMotivoBaja; }
        //    set { bMotivoBaja = value; OnPropertyChanged("BMotivoBaja"); }
        //}

        //private short? bAutoridadBaja = -1;
        //public short? BAutoridadBaja
        //{
        //    get { return bAutoridadBaja; }
        //    set { bAutoridadBaja = value; OnPropertyChanged("BAutoridadBaja"); }
        //}
        #endregion 

        #region AmparosIndirectos
        private ObservableCollection<AMPARO_INDIRECTO> lstAmparoIndirecto;
        public ObservableCollection<AMPARO_INDIRECTO> LstAmparoIndirecto
        {
            get { return lstAmparoIndirecto; }
            set { lstAmparoIndirecto = value; OnPropertyChanged("LstAmparoIndirecto"); }
        }

        private bool amparoIndirectoEmpty = false;
        public bool AmparoIndirectoEmpty
        {
            get { return amparoIndirectoEmpty; }
            set { amparoIndirectoEmpty = value; OnPropertyChanged("AmparoIndirectoEmpty"); }
        }

        private AMPARO_INDIRECTO selectedAmparoIndirecto;
        public AMPARO_INDIRECTO SelectedAmparoIndirecto
        {
            get { return selectedAmparoIndirecto; }
            set { selectedAmparoIndirecto = value;
            if (value != null)
                DAmparoIndirecto = true;
            else
                DAmparoDirecto = false;

                OnPropertyChanged("SelectedAmparoIndirecto"); }
        }

        private ObservableCollection<AMPARO_INDIRECTO_TIPO> lstTipoAmparoIndirecto;
        public ObservableCollection<AMPARO_INDIRECTO_TIPO> LstTipoAmparoIndirecto
        {
            get { return lstTipoAmparoIndirecto; }
            set { lstTipoAmparoIndirecto = value; OnPropertyValidateChanged("LstTipoAmparoIndirecto"); }
        }

        private AMPARO_INDIRECTO_TIPO selectedTipoAmparoIndirecto;
        public AMPARO_INDIRECTO_TIPO SelectedTipoAmparoIndirecto
        {
            get { return selectedTipoAmparoIndirecto; }
            set { selectedTipoAmparoIndirecto = value; OnPropertyChanged("SelectedTipoAmparoIndirecto"); }
        }

        private ObservableCollection<JUZGADO> lstJuzgadoAmparo;
        public ObservableCollection<JUZGADO> LstJuzgadoAmparo
        {
            get { return lstJuzgadoAmparo; }
            set { lstJuzgadoAmparo = value; OnPropertyChanged("LstJuzgadoAmparo"); }
        }

        private ObservableCollection<AMPARO_INDIRECTO_SENTENCIA> lstSentenciaAI;
        public ObservableCollection<AMPARO_INDIRECTO_SENTENCIA> LstSentenciaAI
        {
            get { return lstSentenciaAI; }
            set { lstSentenciaAI = value; OnPropertyChanged("LstSentenciaAI"); }
        }

        private ObservableCollection<AMPARO_INDIRECTO_TIPOS> lstAIT;
        public ObservableCollection<AMPARO_INDIRECTO_TIPOS> LstAIT
        {
            get { return lstAIT; }
            set { lstAIT = value; OnPropertyChanged("LstAIT"); }
        }

        private AMPARO_INDIRECTO_TIPOS selectedAmparoIndirectoTipos;
        public AMPARO_INDIRECTO_TIPOS SelectedAAmparoIndirectoTipos
        {
            get { return selectedAmparoIndirectoTipos; }
            set { selectedAmparoIndirectoTipos = value; OnPropertyChanged("SelectedAAmparoIndirectoTipos"); }
        }

        private bool aIFechaDocumentoValid = false;
        public bool AIFechaDocumentoValid
        {
            get { return aIFechaDocumentoValid; }
            set { aIFechaDocumentoValid = value; OnPropertyValidateChanged("AIFechaDocumentoValid"); }
        }

        private bool aIFechaNotificacionValid = false;
        public bool AIFechaNotificacionValid
        {
            get { return aIFechaNotificacionValid; }
            set { aIFechaNotificacionValid = value; OnPropertyValidateChanged("AIFechaNotificacionValid"); }
        }

        private bool aIFechaSuspencionValid = false;
        public bool AIFechaSuspencionValid
        {
            get { return aIFechaSuspencionValid; }
            set { aIFechaSuspencionValid = value; OnPropertyValidateChanged("AIFechaSuspencionValid"); }
        }

        private short? aITipo;
        public short? AITipo
        {
            get { return aITipo; }
            set { aITipo = value; OnPropertyValidateChanged("AITipo"); }
        }

        private string aINoOficio;
        public string AINoOficio
        {
            get { return aINoOficio; }
            set { aINoOficio = value; OnPropertyValidateChanged("AINoOficio"); }
        }

        private DateTime? aIFechaDocumento;
        public DateTime? AIFechaDocumento
        {
            get { return aIFechaDocumento; }
            set { aIFechaDocumento = value;
            if (value.HasValue)
                AIFechaDocumentoValid = true;
            else
                AIFechaDocumentoValid = false;
                OnPropertyValidateChanged("AIFechaDocumento");
            }
        }

        private DateTime? aIFechaNotificacion;
        public DateTime? AIFechaNotificacion
        {
            get { return aIFechaNotificacion; }
            set { aIFechaNotificacion = value;
            if (value.HasValue)
                AIFechaNotificacionValid = true;
            else
                AIFechaNotificacionValid = false;
                OnPropertyValidateChanged("AIFechaNotificacion");
            }
        }

        private DateTime? aIFechaSuspencion;
        public DateTime? AIFechaSuspencion
        {
            get { return aIFechaSuspencion; }
            set { aIFechaSuspencion = value;
            if (value.HasValue)
                AIFechaSuspencionValid = true;
            else
                AIFechaSuspencionValid = false;
                OnPropertyValidateChanged("AIFechaSuspencion");
            }
        }

        private short? aIAutoridadInforma;
        public short? AIAutoridadInforma
        {
            get { return aIAutoridadInforma; }
            set { aIAutoridadInforma = value; OnPropertyValidateChanged("AIAutoridadInforma"); }
        }

        private string aINoAmparo;
        public string AINoAmparo
        {
            get { return aINoAmparo; }
            set { aINoAmparo = value; OnPropertyValidateChanged("AINoAmparo"); }
        }

        private short? aIAutoridadResuelve;
        public short? AIAutoridadResuelve
        {
            get { return aIAutoridadResuelve; }
            set { aIAutoridadResuelve = value; OnPropertyValidateChanged("AIAutoridadResuelve"); }
        }

        private string aINoOficioResuelve;
        public string AINoOficioResuelve
        {
            get { return aINoOficioResuelve; }
            set { aINoOficioResuelve = value; OnPropertyValidateChanged("AINoOficioResuelve"); }
        }

        private DateTime? aIFechaDocumentoResuelve;
        public DateTime? AIFechaDocumentoResuelve
        {
            get { return aIFechaDocumentoResuelve; }
            set { aIFechaDocumentoResuelve = value; OnPropertyValidateChanged("AIFechaDocumentoResuelve"); }
        }

        private DateTime? aIFechaSentenciaResuelve;
        public DateTime? AIFechaSentenciaResuelve
        {
            get { return aIFechaSentenciaResuelve; }
            set { aIFechaSentenciaResuelve = value; OnPropertyValidateChanged("AIFechaSentenciaResuelve"); }
        }

        private short? aIResultadoSentencia;
        public short? AIResultadoSentencia
        {
            get { return aIResultadoSentencia; }
            set { aIResultadoSentencia = value; OnPropertyValidateChanged("AIResultadoSentencia"); }
        }

        private DateTime? aIFechaEjecutoria;
        public DateTime? AIFechaEjecutoria
        {
            get { return aIFechaEjecutoria; }
            set { aIFechaEjecutoria = value; OnPropertyValidateChanged("AIFechaEjecutoria"); }
        }

        private DateTime? aIFechaRevision;
        public DateTime? AIFechaRevision
        {
            get { return aIFechaRevision; }
            set { aIFechaRevision = value; OnPropertyValidateChanged("AIFechaRevision"); }
        }

        private string aIActoReclamado;
        public string AIActoReclamado
        {
            get { return aIActoReclamado; }
            set { aIActoReclamado = value; OnPropertyValidateChanged("AIActoReclamado"); }
        }
        #endregion

        #region AmparoDirecto
        private ObservableCollection<AMPARO_DIRECTO> lstAmparoDirecto;
        public ObservableCollection<AMPARO_DIRECTO> LstAmparoDirecto
        {
            get { return lstAmparoDirecto; }
            set { lstAmparoDirecto = value; OnPropertyChanged("LstAmparoDirecto"); }
        }

        private bool amparoDirectoEmpty = false;
        public bool AmparoDirectoEmpty
        {
            get { return amparoDirectoEmpty; }
            set { amparoDirectoEmpty = value; OnPropertyChanged("AmparoDirectoEmpty"); }
        }

        private AMPARO_DIRECTO selectedAmparoDirecto;
        public AMPARO_DIRECTO SelectedAmparoDirecto
        {
            get { return selectedAmparoDirecto; }
            set { selectedAmparoDirecto = value;
            if (value != null)
                DAmparoDirecto = true;
            else
                DAmparoDirecto = false;
                OnPropertyChanged("SelectedAmparoDirecto"); }
        }

        private bool aDFechaDocumentoValid = false;
        public bool ADFechaDocumentoValid
        {
            get { return aDFechaDocumentoValid; }
            set { aDFechaDocumentoValid = value; OnPropertyChanged("ADFechaDocumentoValid"); }
        }

        private bool aDFechaNotificacionValid = false;
        public bool ADFechaNotificacionValid
        {
            get { return aDFechaNotificacionValid; }
            set { aDFechaNotificacionValid = value; OnPropertyChanged("ADFechaNotificacionValid"); }
        }

        private bool aDFechaSuspencionValid = false;
        public bool ADFechaSuspencionValid
        {
            get { return aDFechaSuspencionValid; }
            set { aDFechaSuspencionValid = value; OnPropertyChanged("ADFechaSuspencionValid"); }
        }

        private string aDNoOficio;
        public string ADNoOficio
        {
            get { return aDNoOficio; }
            set { aDNoOficio = value; OnPropertyValidateChanged("ADNoOficio"); }
        }

        private DateTime? aDFechaDocumento;
        public DateTime? ADFechaDocumento
        {
            get { return aDFechaDocumento; }
            set { aDFechaDocumento = value;
            if (value.HasValue)
                ADFechaDocumentoValid = true;
            else
                ADFechaDocumentoValid = false;
                OnPropertyValidateChanged("ADFechaDocumento"); }
        }

        private DateTime? aDFechaNotificacion;
        public DateTime? ADFechaNotificacion
        {
            get { return aDFechaNotificacion; }
            set { aDFechaNotificacion = value;
            if (value.HasValue)
                ADFechaNotificacionValid = true;
            else
                ADFechaNotificacionValid = false;
            OnPropertyValidateChanged("ADFechaNotificacion");
            }
        }

        private DateTime? aDFechaSuspencion;
        public DateTime? ADFechaSuspencion
        {
            get { return aDFechaSuspencion; }
            set { aDFechaSuspencion = value;
            if (value.HasValue)
                ADFechaSuspencionValid = true;
            else
                ADFechaSuspencionValid = false;
            OnPropertyValidateChanged("ADFechaSuspencion");
            }
        }

        private short? aDAutoridadInforma;
        public short? ADAutoridadInforma
        {
            get { return aDAutoridadInforma; }
            set { aDAutoridadInforma = value; OnPropertyValidateChanged("ADAutoridadInforma"); }
        }

        private string aDNoAmparo;
        public string ADNoAmparo
        {
            get { return aDNoAmparo; }
            set { aDNoAmparo = value; OnPropertyValidateChanged("ADNoAmparo"); }
        }

        private short? aDAutoridadNotifica;
        public short? ADAutoridadNotifica
        {
            get { return aDAutoridadNotifica; }
            set { aDAutoridadNotifica = value; OnPropertyValidateChanged("ADAutoridadNotifica"); }
        }

        private string aDNoOficioResuelve;
        public string ADNoOficioResuelve
        {
            get { return aDNoOficioResuelve; }
            set { aDNoOficioResuelve = value; OnPropertyValidateChanged("ADNoOficioResuelve"); }
        }

        private DateTime? aDFechaDocumentoResuelve;
        public DateTime? ADFechaDocumentoResuelve
        {
            get { return aDFechaDocumentoResuelve; }
            set { aDFechaDocumentoResuelve = value; OnPropertyValidateChanged("ADFechaDocumentoResuelve"); }
        }

        private DateTime? aDFechaSentenciaResuelve;
        public DateTime? ADFechaSentenciaResuelve
        {
            get { return aDFechaSentenciaResuelve; }
            set { aDFechaSentenciaResuelve = value; OnPropertyValidateChanged("ADFechaSentenciaResuelve"); }
        }

        private short? aDResultadoSentencia;
        public short? ADResultadoSentencia
        {
            get { return aDResultadoSentencia; }
            set { aDResultadoSentencia = value; OnPropertyValidateChanged("ADResultadoSentencia"); }
        }

        private short? aDAutoridadResuelve;
        public short? ADAutoridadResuelve
        {
            get { return aDAutoridadResuelve; }
            set { aDAutoridadResuelve = value; OnPropertyValidateChanged("ADAutoridadResuelve"); }
        }
        #endregion

        #region Incidentes
        private ObservableCollection<AMPARO_INCIDENTE> lstAmparoIncidente;
        public ObservableCollection<AMPARO_INCIDENTE> LstAmparoIncidente
        {
            get { return lstAmparoIncidente; }
            set { lstAmparoIncidente = value; OnPropertyChanged("LstAmparoIncidente"); }
        }

        private bool amparoIncidenteEmpty = false;
        public bool AmparoIncidenteEmpty
        {
            get { return amparoIncidenteEmpty; }
            set { amparoIncidenteEmpty = value; OnPropertyChanged("AmparoIncidenteEmpty"); }
        }

        private AMPARO_INCIDENTE selectedAmparoIncidente;
        public AMPARO_INCIDENTE SelectedAmparoIncidente
        {
            get { return selectedAmparoIncidente; }
            set { selectedAmparoIncidente = value;
            if (value != null)
                DIncidente = true;
            else
                DIncidente = false;
                OnPropertyChanged("SelectedAmparoIncidente"); }
        }

        private ObservableCollection<AMPARO_INCIDENTE_TIPO> lstAmparoIncidenteTipo;
        public ObservableCollection<AMPARO_INCIDENTE_TIPO> LstAmparoIncidenteTipo
        {
            get { return lstAmparoIncidenteTipo; }
            set { lstAmparoIncidenteTipo = value; OnPropertyChanged("LstAmparoIncidenteTipo"); }
        }

        private ObservableCollection<cIncidenteResultado> lstAmparoIncidenteResultado;
        public ObservableCollection<cIncidenteResultado> LstAmparoIncidenteResultado
        {
            get { return lstAmparoIncidenteResultado; }
            set { lstAmparoIncidenteResultado = value; OnPropertyChanged("LstAmparoIncidenteResultado"); }
        }

        private Visibility iDiasRevisionVisible = Visibility.Hidden;
        public Visibility IDiasRevisionVisible
        {
            get { return iDiasRevisionVisible; }
            set { iDiasRevisionVisible = value; OnPropertyChanged("IDiasRevisionVisible"); }
        }

        private Visibility iGarantiaVisible = Visibility.Hidden;
        public Visibility IGarantiaVisible
        {
            get { return iGarantiaVisible; }
            set { iGarantiaVisible = value; OnPropertyChanged("IGarantiaVisible"); }
        }

        private Visibility iSentenciaVisible = Visibility.Hidden;
        public Visibility ISentenciaVisible
        {
            get { return iSentenciaVisible; }
            set { iSentenciaVisible = value; OnPropertyChanged("ISentenciaVisible"); }
        }

        private bool iFechaDocumentoValid = false;
        public bool IFechaDocumentoValid
        {
            get { return iFechaDocumentoValid; }
            set { iFechaDocumentoValid = value; OnPropertyChanged("IFechaDocumentoValid"); }
        }

        private short? iTipo;
        public short? ITipo
        {
            get { return iTipo; }
            set { iTipo = value; 
                LstAmparoIncidenteResultado = new ObservableCollection<cIncidenteResultado>();
                if (value != null)
                {
                    if(value == 3)
                    {
                        LstAmparoIncidenteResultado.Add(new cIncidenteResultado() { Id = "M", Descr = "MODIFICA LA PENA" });
                        LstAmparoIncidenteResultado.Add(new cIncidenteResultado() { Id = "E", Descr = "EXTINGUE LA SANCION" });
                    }
                    else
                    //if (value == 1 || value == 2)
                    {
                        LstAmparoIncidenteResultado.Add(new cIncidenteResultado() { Id = "C", Descr = "SE CONCEDE" });
                        LstAmparoIncidenteResultado.Add(new cIncidenteResultado() { Id = "N", Descr = "SE NIEGA" });
                    }
                    LstAmparoIncidenteResultado.Insert(0, new cIncidenteResultado() {Id = string.Empty, Descr = "SELECCIONE" });
                    IResultado = string.Empty;
                }
                OnPropertyChanged("ITipo"); }
        }

        private string iResultado;
        public string IResultado
        {
            get { return iResultado; }
            set { iResultado = value;
            IDiasRevisionVisible = IGarantiaVisible = ISentenciaVisible = Visibility.Hidden;
            if (value != null)
            {
                IDiasRemision = null;
                IGarantia = null;
                IModificaPenaAnios = IModificaPenaMeses = IModificaPenaDias = null;
                switch (ITipo)
                {
                    case 1:
                        if (value == "C")
                        {
                            IDiasRevisionVisible = Visibility.Visible;
                        }
                        break;
                    case 2:
                        if (value == "C")
                            IGarantiaVisible = Visibility.Visible;
                        break;
                    case 3:
                        if (value == "M")
                            ISentenciaVisible = Visibility.Visible;
                        break;
                }
            }
                SetValidacionesIncidentes();
                OnPropertyValidateChanged("IResultado"); }
        }

        private decimal? iDiasRemision = null;
        public decimal? IDiasRemision
        {
            get { return iDiasRemision; }
            set { iDiasRemision = value; OnPropertyValidateChanged("IDiasRemision"); }
        }

        private string iNoOficio;
        public string INoOficio
        {
            get { return iNoOficio; }
            set { iNoOficio = value; OnPropertyValidateChanged("INoOficio"); }
        }

        private DateTime? iFechaDocumento;
        public DateTime? IFechaDocumento
        {
            get { return iFechaDocumento; }
            set { iFechaDocumento = value;
            if (value.HasValue)
                IFechaDocumentoValid = true;
            else
                IFechaDocumentoValid = false;
            OnPropertyValidateChanged("IFechaDocumento");
            }
        }

        private short? iAutoridadInforma;
        public short? IAutoridadInforma
        {
            get { return iAutoridadInforma; }
            set { iAutoridadInforma = value; OnPropertyValidateChanged("IAutoridadInforma"); }
        }

        private decimal? iGarantia;
        public decimal? IGarantia
        {
            get { return iGarantia; }
            set { iGarantia = value; OnPropertyValidateChanged("IGarantia"); }
        }

        private short? iModificaPenaAnios;
        public short? IModificaPenaAnios
        {
            get { return iModificaPenaAnios; }
            set { iModificaPenaAnios = value;
                SetValidacionesIncidentes();
                OnPropertyValidateChanged("IModificaPenaAnios"); }
        }

        private short? iModificaPenaMeses;
        public short? IModificaPenaMeses
        {
            get { return iModificaPenaMeses; }
            set { iModificaPenaMeses = value;
                SetValidacionesIncidentes();
                OnPropertyValidateChanged("IModificaPenaMeses"); }
        }

        private short? iModificaPenaDias;
        public short? IModificaPenaDias
        {
            get { return iModificaPenaDias; }
            set { iModificaPenaDias = value;
                SetValidacionesIncidentes();
                OnPropertyValidateChanged("IModificaPenaDias"); }
        }
        #endregion

        #region Recursos
        private ObservableCollection<RECURSO> lstRecursos;
        public ObservableCollection<RECURSO> LstRecursos
        {
            get { return lstRecursos; }
            set { lstRecursos = value; OnPropertyChanged("LstRecursos"); }
        }

        private bool recursoEmpty = false;
        public bool RecursoEmpty
        {
            get { return recursoEmpty; }
            set { recursoEmpty = value; OnPropertyChanged("RecursoEmpty"); }
        }

        private RECURSO selectedRecurso;
        public RECURSO SelectedRecurso
        {
            get { return selectedRecurso; }
            set { selectedRecurso = value;
            if (value != null)
                DRecursos = true;
            else
                DRecursos = false;
                OnPropertyChanged("SelectedRecurso"); }
        }

        private ObservableCollection<TIPO_RECURSO> lstTiposRecursos;
        public ObservableCollection<TIPO_RECURSO> LstTiposRecursos
        {
            get { return lstTiposRecursos; }
            set { lstTiposRecursos = value; OnPropertyChanged("LstTiposRecursos"); }
        }

        private ObservableCollection<RECURSO_RESULTADO> lstRecursoResultado;
        public ObservableCollection<RECURSO_RESULTADO> LstRecursoResultado
        {
            get { return lstRecursoResultado; }
            set { lstRecursoResultado = value; OnPropertyChanged("LstRecursoResultado"); }
        }

        private JUZGADO selectedTribunal;
        public JUZGADO SelectedTribunal
        {
            get { return selectedTribunal; }
            set
            {
                selectedTribunal = value;
                if (selectedTribunal != null)
                {
                    switch (selectedTribunal.ID_FUERO)
                    {
                        case "F":
                            RFuero = "FEDERAL";
                            break;
                        case "C":
                            RFuero = "COMUN";
                            break;
                        case "M":
                            RFuero = "MILITAR";
                            break;
                    }
                }
                else
                    FueroR = string.Empty;
                OnPropertyChanged("SelectedTribunal");
            }
        }

        private bool rFechaRecursoValid = false;
        public bool RFechaRecursoValid
        {
            get { return rFechaRecursoValid; }
            set { rFechaRecursoValid = value; OnPropertyChanged("RFechaRecursoValid"); }
        }

        private bool rFechaResolucionValid = false;
        public bool RFechaResolucionValid
        {
            get { return rFechaResolucionValid; }
            set { rFechaResolucionValid = value; OnPropertyChanged("RFechaResolucionValid"); }
        }

        private TIPO_RECURSO selectedTipoRecurso;
        public TIPO_RECURSO SelectedTipoRecurso
        {
            get { return selectedTipoRecurso; }
            set { selectedTipoRecurso = value;
            
            if (value != null)
                LstRecursoResultado = new ObservableCollection<RECURSO_RESULTADO>(value.RECURSO_RESULTADO);
            else
                LstRecursoResultado = new ObservableCollection<RECURSO_RESULTADO>();
            LstRecursoResultado.Insert(0, new RECURSO_RESULTADO() { RESULTADO = string.Empty, DESCR = "SELECCIONE" });
            RResultadoRecurso = string.Empty;
                OnPropertyChanged("SelectedTipoRecurso"); }
        }

        private RECURSO_RESULTADO selectedRecursoResultado;
        public RECURSO_RESULTADO SelectedRecursoResultado
        {
            get { return selectedRecursoResultado; }
            set
            {
                selectedRecursoResultado = value;
                if (value != null)
                {
                    //MULTA
                    if (value.MODIFICA_OTRO == "S")
                    {
                        LimpiarMultasRecurso();
                        HabilitaMulta = true;
                    }
                    else
                    {
                        LimpiarMultasRecurso();
                        HabilitaMulta = false;
                    }
                    //MODIFICA SENTENCIA
                    if (value.MODIFICA_SENTENCIA == "S")
                    {
                        LimpiarSentenciaRecurso();
                        HabilitaSentencia = true;
                    }
                    else
                    {
                        LimpiarSentenciaRecurso();
                        HabilitaSentencia = false;
                    }
                    SetValidacionesRecurso();
                }
                OnPropertyChanged("SelectedRecursoResultado");
            }
        }

        private short? rTipoRecurso = -1;
        public short? RTipoRecurso
        {
            get { return rTipoRecurso; }
            set { rTipoRecurso = value;
            SetValidacionesRecurso();
                OnPropertyValidateChanged("RTipoRecurso"); }
        }

        private short? rTribunal;
        public short? RTribunal
        {
            get { return rTribunal; }
            set { rTribunal = value; OnPropertyValidateChanged("RTribunal"); }
        }

        private string rFuero;
        public string RFuero
        {
            get { return rFuero; }
            set { rFuero = value; OnPropertyValidateChanged("RFuero"); }
        }

        private string rResultadoRecurso = null;
        public string RResultadoRecurso
        {
            get { return rResultadoRecurso; }
            set { 
                rResultadoRecurso = value;
                OnPropertyValidateChanged("RResultadoRecurso");
                SetValidacionesRecurso();
            }
        }

        private DateTime? rFechaRecurso;
        public DateTime? RFechaRecurso
        {
            get { return rFechaRecurso; }
            set { rFechaRecurso = value;
            if (value.HasValue)
                RFechaRecursoValid = true;
            else
                RFechaRecursoValid = false;
            OnPropertyValidateChanged("RFechaRecurso");
            }
        }

        private string rTocaPenal;
        public string RTocaPenal
        {
            get { 
                if (rTocaPenal == null)
                    return string.Empty;
                return new Converters().MascaraTocaPenal(rTocaPenal);
            }
            set { rTocaPenal = value; OnPropertyValidateChanged("RTocaPenal"); }
        }

        private string rNoOficio;
        public string RNoOficio
        {
            get { return rNoOficio; }
            set { rNoOficio = value; OnPropertyValidateChanged("RNoOficio"); }
        }

        private string rResolucion = string.Empty;
        public string RResolucion
        {
            get { return rResolucion; }
            set { rResolucion = value; OnPropertyValidateChanged("RResolucion"); }
        }

        private DateTime? rFechaResolucion;
        public DateTime? RFechaResolucion
        {
            get { return rFechaResolucion; }
            set { rFechaResolucion = value;
            if (value.HasValue)
                RFechaResolucionValid = true;
            else
                RFechaResolucionValid = false;
            OnPropertyValidateChanged("RFechaResolucion");
            }
        }

        private string rMulta;
        public string RMulta
        {
            get { return rMulta; }
            set { rMulta = value; OnPropertyValidateChanged("RMulta"); }
        }

        private string rReparacionDanio;
        public string RReparacionDanio
        {
            get { return rReparacionDanio; }
            set { rReparacionDanio = value; OnPropertyValidateChanged("RReparacionDanio"); }
        }

        private string rSustitucionPena;
        public string RSustitucionPena
        {
            get { return rSustitucionPena; }
            set { rSustitucionPena = value; OnPropertyValidateChanged("RSustitucionPena"); }
        }

        private string rMultaCondicional;
        public string RMultaCondicional
        {
            get { return rMultaCondicional; }
            set { rMultaCondicional = value; OnPropertyValidateChanged("RMultaCondicional"); }
        }

        private short? rAnio;
        public short? RAnio
        {
            get { return rAnio; }
            set { rAnio = value;
            SetValidacionesRecurso();
                OnPropertyValidateChanged("RAnio"); }
        }
     
        private short? rMeses;
        public short? RMeses
        {
            get { return rMeses; }
            set { rMeses = value;
            SetValidacionesRecurso();
                OnPropertyValidateChanged("RMeses"); }
        }

        private short? rDias;
        public short? RDias
        {
            get { return rDias; }
            set { rDias = value;
            SetValidacionesRecurso();
                OnPropertyValidateChanged("RDias"); }
        }
        #endregion

        #region Digitalizacion
        private ObservableCollection<TipoDocumento> listTipoDocumento;
        public ObservableCollection<TipoDocumento> ListTipoDocumento
        {
            get { return listTipoDocumento; }
            set { listTipoDocumento = value; OnPropertyChanged("ListTipoDocumento"); }
        }

        TipoDocumento _SelectedTipoDocumento;
        public TipoDocumento SelectedTipoDocumento
        {
            get { return _SelectedTipoDocumento; }
            set
            {
                DocumentoDigitalizado = null;
                ObservacionDocumento = string.Empty;
                _SelectedTipoDocumento = value;
                OnPropertyChanged("SelectedTipoDocumento");
            }
        }

        private DateTime? datePickCapturaDocumento = Fechas.GetFechaDateServer;
        public DateTime? DatePickCapturaDocumento
        {
            get { return datePickCapturaDocumento; }
            set { datePickCapturaDocumento = value; OnPropertyChanged("DatePickCapturaDocumento"); }
        }

        private string observacionDocumento;
        public string ObservacionDocumento
        {
            get { return observacionDocumento; }
            set { observacionDocumento = value; OnPropertyChanged("ObservacionDocumento"); }
        }

        private bool _AutoGuardado = true;
        public bool AutoGuardado
        {
            get { return _AutoGuardado; }
            set
            {
                _AutoGuardado = value;
                OnPropertyChanged("AutoGuardado");
            }
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

        DigitalizarDocumento escaner = new DigitalizarDocumento(Application.Current.Windows[0]);
        public byte[] DocumentoDigitalizado { get; set; }
        #endregion

        #region Mostrar Menu
        private Visibility mostrarOpcion = Visibility.Collapsed;
        public Visibility MostrarOpcion
        {
            get { return mostrarOpcion; }
            set { mostrarOpcion = value; OnPropertyChanged("MostrarOpcion"); }
        }

        private Visibility eliminarCoparticipeOpcion = Visibility.Collapsed;
        public Visibility EliminarCoparticipeOpcion
        {
            get { return eliminarCoparticipeOpcion; }
            set { eliminarCoparticipeOpcion = value; OnPropertyChanged("EliminarCoparticipeOpcion"); }
        }

        private Visibility eliminarAliasOpcion = Visibility.Collapsed;
        public Visibility EliminarAliasOpcion
        {
            get { return eliminarAliasOpcion; }
            set { eliminarAliasOpcion = value; OnPropertyChanged("EliminarAliasOpcion"); }
        }

        private Visibility eliminarApodoOpcion = Visibility.Collapsed;
        public Visibility EliminarApodoOpcion
        {
            get { return eliminarApodoOpcion; }
            set { eliminarApodoOpcion = value; OnPropertyChanged("EliminarApodoOpcion"); }
        }

        private Visibility eliminarSentenciaDelito = Visibility.Collapsed;
        public Visibility EliminarSentenciaDelito
        {
            get { return eliminarSentenciaDelito; }
            set { eliminarSentenciaDelito = value;
                  OnPropertyChanged("EliminarSentenciaDelito");
            }
        }

        

        #endregion

        #region Temporales
        private IMPUTADO tImputado;
        private INGRESO tIngreso;
        #endregion

        #region Pantalla
        private double tabWidth;
        public double TabWidth
        {
            get { return tabWidth; }
            set { tabWidth = value; OnPropertyChanged("TabWidth"); }
        }
        #endregion

        #region Validaciones
        private bool dCausaPenal = false;
        public bool DCausaPenal
        {
            get { return dCausaPenal; }
            set { dCausaPenal = value; OnPropertyChanged("DCausaPenal"); }
        }

        private bool dRecursos = false;
        public bool DRecursos
        {
            get { return dRecursos; }
            set { dRecursos = value; OnPropertyChanged("DRecursos"); }
        }

        private bool dAmparoDirecto = false;
        public bool DAmparoDirecto
        {
            get { return dAmparoDirecto; }
            set { dAmparoDirecto = value; OnPropertyChanged("DAmparoDirecto"); }
        }

        private bool dAmparoIndirecto = false;
        public bool DAmparoIndirecto
        {
            get { return dAmparoIndirecto; }
            set { dAmparoIndirecto = value; OnPropertyChanged("DAmparoIndirecto"); }
        }
       
        private bool dIncidente = false;
        public bool DIncidente
        {
            get { return dIncidente; }
            set { dIncidente = value; OnPropertyChanged("DIncidente"); }
        }
        #endregion
    }
}
