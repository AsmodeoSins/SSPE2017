using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlPenales
{
    partial class TrasladoExternoViewModel
    {


        #region Variables Manejo Controles
        #region Variables para Control de Visibilidad
        private Visibility otroDestinoVisible;
        public Visibility OtroDestinoVisible
        {
            get { return otroDestinoVisible; }
            set { otroDestinoVisible = value; RaisePropertyChanged("OtroDestinoVisible"); }
        }
        #endregion
        private bool cancelarMenuEnabled = false;
        public bool CancelarMenuEnabled
        {
            get { return cancelarMenuEnabled; }
            set { cancelarMenuEnabled = value; RaisePropertyChanged("CancelarMenuEnabled"); }
        }

        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; RaisePropertyChanged("MenuGuardarEnabled"); }
        }

        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; RaisePropertyChanged("MenuBuscarEnabled"); }
        }

        private bool menuLimpiarEnabled = false;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set { menuLimpiarEnabled = value; RaisePropertyChanged("MenuLimpiarEnabled"); }
        }

        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; RaisePropertyChanged("MenuReporteEnabled"); }
        }

        private bool eliminarMenuEnabled = false;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; RaisePropertyChanged("EliminarMenuEnabled"); }
        }

        #endregion
        #region Datos Traslado
        private ObservableCollection<TRASLADO_MOTIVO> lstMotivo;
        public ObservableCollection<TRASLADO_MOTIVO> LstMotivo
        {
            get { return lstMotivo; }
            set { lstMotivo = value; OnPropertyChanged("LstMotivo"); }
        }

        private ObservableCollection<EMISOR> lstcentrodestinoforaneo;
        public ObservableCollection<EMISOR> LstCentroDestinoForaneo
        {
            get { return lstcentrodestinoforaneo; }
            set { lstcentrodestinoforaneo = value; RaisePropertyChanged("LstCentroDestinoForaneo"); }
        }

        private int selectedEmisor = -1;
        public int SelectedEmisor
        {
            get { return selectedEmisor; }
            set { selectedEmisor = value; RaisePropertyChanged("SelectedEmisor"); }
        }

        private int id_autoridad_traslado;
        private string autoridad_traslado;
        public string Autoridad_Traslado
        {
            get { return autoridad_traslado; }
            set { autoridad_traslado = value; RaisePropertyChanged("Autoridad_Traslado"); }
        }

        private bool justificacion = true;
        public bool Justificacion
        {
            get { return justificacion; }
            set { justificacion = value; }
        }

        private bool dTFechaValid = false;
        public bool DTFechaValid
        {
            get { return dTFechaValid; }
            set { dTFechaValid = value; OnPropertyChanged("DTFechaValid"); }
        }

        private DateTime? dTFecha;
        public DateTime? DTFecha
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

        private short? dTMotivo;
        public short? DTMotivo
        {
            get { return dTMotivo; }
            set
            {
                dTMotivo = value;
                if (dTMotivo.HasValue)
                    OnPropertyValidateChanged("DTMotivo");
                else
                    RaisePropertyChanged("DTMotivo");
            }
        }

        private string dTJustificacion;
        public string DTJustificacion
        {
            get { return dTJustificacion; }
            set
            {
                dTJustificacion = value;
                if (!string.IsNullOrWhiteSpace(dTJustificacion))
                    OnPropertyValidateChanged("DTJustificacion");
                else
                    RaisePropertyChanged("DTJustificacion");
            }
        }

        private string dTNoOficio;
        public string DTNoOficio
        {
            get { return dTNoOficio; }
            set
            {
                dTNoOficio = value;
                if (!string.IsNullOrWhiteSpace(dTNoOficio))
                    OnPropertyValidateChanged("DTNoOficio");
                else
                    RaisePropertyChanged("DTNoOficio");
            }
        }

        private string otroDestino = string.Empty;
        public string OtroDestino
        {
            get { return otroDestino; }
            set { otroDestino = value; RaisePropertyChanged("OtroDestino"); }
        }

        #region Ampliar Descripcion
        private string tituloHeaderExpandirDescripcion;
        public string TituloHeaderExpandirDescripcion
        {
            get { return tituloHeaderExpandirDescripcion; }
            set { tituloHeaderExpandirDescripcion = value; OnPropertyChanged("TituloHeaderExpandirDescripcion"); }
        }

        private string textAmpliarDescripcion;
        public string TextAmpliarDescripcion
        {
            get { return textAmpliarDescripcion; }
            set { textAmpliarDescripcion = value; OnPropertyChanged("TextAmpliarDescripcion"); }
        }

        private short maxLengthAmpliarDescripcion = 1000;
        public short MaxLengthAmpliarDescripcion
        {
            get { return maxLengthAmpliarDescripcion; }
            set { maxLengthAmpliarDescripcion = value; OnPropertyChanged("MaxLengthAmpliarDescripcion"); }
        }
        #endregion

        #endregion
        #region Datos Egreso

        private string dENoOficio;
        public string DENoOficio
        {
            get { return dENoOficio; }
            set
            {
                dENoOficio = value;
                if (!string.IsNullOrWhiteSpace(dENoOficio))
                    OnPropertyValidateChanged("DENoOficio");
                else
                    OnPropertyValidateChanged("DENoOficio");
            }
        }

        private List<string> autoridadesSalida = new List<string>();
        public List<string> AutoridadesSalida
        {
            get { return autoridadesSalida; }
            set { autoridadesSalida = value; RaisePropertyChanged("AutoridadesSalida"); }
        }

        private string autorizaSalida;
        public string AutorizaSalida
        {
            get { return autorizaSalida; }
            set { autorizaSalida = value; OnPropertyValidateChanged("AutorizaSalida"); }
        }

        private short? id_motivo_traslado;

        private string motivoSalida;
        public string MotivoSalida
        {
            get { return motivoSalida; }
            set
            {
                motivoSalida = value; RaisePropertyChanged("MotivoSalida");
            }
        }

        #endregion
        #region Datos Expediente
        private int? anioD;
        public int? AnioD
        {
            get { return anioD; }
            set { anioD = value; RaisePropertyChanged("AnioD"); }
        }

        private int? folioD;
        public int? FolioD
        {
            get { return folioD; }
            set { folioD = value; RaisePropertyChanged("FolioD"); }
        }

        private string paternoD;
        public string PaternoD
        {
            get { return paternoD; }
            set { paternoD = value; RaisePropertyChanged("PaternoD"); }
        }

        private string maternoD;
        public string MaternoD
        {
            get { return maternoD; }
            set { maternoD = value; RaisePropertyChanged("MaternoD"); }
        }

        private string nombreD;
        public string NombreD
        {
            get { return nombreD; }
            set { nombreD = value; RaisePropertyChanged("NombreD"); }
        }

        private int? ingresosD;
        public int? IngresosD
        {
            get { return ingresosD; }
            set { ingresosD = value; RaisePropertyChanged("IngresosD"); }
        }

        private string ubicacionD;
        public string UbicacionD
        {
            get { return ubicacionD; }
            set { ubicacionD = value; RaisePropertyChanged("UbicacionD"); }
        }

        private string tipoSeguridadD;
        public string TipoSeguridadD
        {
            get { return tipoSeguridadD; }
            set { tipoSeguridadD = value; RaisePropertyChanged("TipoSeguridadD"); }
        }

        private DateTime? fecIngresoD;
        public DateTime? FecIngresoD
        {
            get { return fecIngresoD; }
            set { fecIngresoD = value; RaisePropertyChanged("FecIngresoD"); }
        }

        private string clasificacionJuridicaD;
        public string ClasificacionJuridicaD
        {
            get { return clasificacionJuridicaD; }
            set { clasificacionJuridicaD = value; RaisePropertyChanged("ClasificacionJuridicaD"); }
        }

        private string estatusD;
        public string EstatusD
        {
            get { return estatusD; }
            set { estatusD = value; RaisePropertyChanged("EstatusD"); }
        }

        private byte[] imagenIngreso; //compartido entre datos y busqueda
        public byte[] ImagenIngreso
        { 
            get { return imagenIngreso; }
            set { imagenIngreso = value; RaisePropertyChanged("ImagenIngreso"); }
        }

        private bool buscarImputadoHabilitado=true;
        public bool BuscarImputadoHabilitado
        {
            get { return buscarImputadoHabilitado; }
            set { buscarImputadoHabilitado = value; RaisePropertyChanged("BuscarImputadoHabilitado"); }
        }

        private bool nombreBuscarHabilitado=true;
        public bool NombreBuscarHabilitado
        {
            get { return nombreBuscarHabilitado; }
            set { nombreBuscarHabilitado = value; RaisePropertyChanged("NombreBuscarHabilitado"); }
        }

        private bool apellidoMaternoBuscarHabilitado=true;
        public bool ApellidoMaternoBuscarHabilitado
        {
            get { return apellidoMaternoBuscarHabilitado; }
            set { apellidoMaternoBuscarHabilitado = value; RaisePropertyChanged("ApellidoMaternoBuscarHabilitado"); }
        }

        private bool apellidoPaternoBuscarHabilitado=true;
        public bool ApellidoPaternoBuscarHabilitado
        {
            get { return apellidoPaternoBuscarHabilitado; }
            set { apellidoPaternoBuscarHabilitado = value; RaisePropertyChanged("ApellidoPaternoBuscarHabilitado"); }
        }

        private bool folioBuscarHabilitado=true;
        public bool FolioBuscarHabilitado
        {
            get { return folioBuscarHabilitado; }
            set { folioBuscarHabilitado = value; RaisePropertyChanged("FolioBuscarHabilitado"); }
        }

        private bool anioBuscarHabilitado=true;
        public bool AnioBuscarHabilitado
        {
            get { return anioBuscarHabilitado; }
            set { anioBuscarHabilitado = value; RaisePropertyChanged("AnioBuscarHabilitado"); }
        }
        #endregion
        #region Busqueda
        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }
        private bool crearNuevoExpedienteEnabled = true;
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

        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                OnPropertyChanged("ListExpediente");
            }
        }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (selectExpediente.INGRESO!=null && selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = selectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    }
                    else
                    {
                        SelectIngreso = null;
                        EmptyIngresoVisible = true;
                    }
                        

                    //OBTENEMOS FOTO DE FRENTE
                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        else
                            ImagenImputado = new Imagenes().getImagenPerson();
                        TextBotonSeleccionarIngreso = "aceptar";
                        SelectIngresoEnabled = true;


                        if (estatus_inactivos != null && SelectIngreso != null && estatus_inactivos.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO))
                        {
                            TextBotonSeleccionarIngreso = "seleccionar ingreso";
                            SelectIngresoEnabled = false;
                        }
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

        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set { aceptarBusquedaHuellaFocus = value; OnPropertyChanged("AceptarBusquedaHuellaFocus"); }
        }

        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        private bool selectIngresoEnabled;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }


        #endregion
        #region busqueda traslados
        private List<Tipo_Traslado> tipos_traslado;
        public List<Tipo_Traslado> Tipos_Traslado
        {
            get { return tipos_traslado; }
            set { tipos_traslado = value; RaisePropertyChanged("Tipos_Traslado"); }
        }

        private Tipo_Traslado selectedTipo_Traslado;
        public Tipo_Traslado SelectedTipo_Traslado
        {
            get { return selectedTipo_Traslado; }
            set { selectedTipo_Traslado = value; RaisePropertyChanged("SelectedTipo_Traslado"); }
        }

        private bool tipo_TrasladoHabilitado = false;
        public bool Tipo_TrasladoHabilitado
        {
            get { return tipo_TrasladoHabilitado; }
            set { tipo_TrasladoHabilitado = value; RaisePropertyChanged("Tipo_TrasladoHabilitado"); }
        }

        private int? anioBuscarTraslado;
        public int? AnioBuscarTraslado
        {
            get { return anioBuscarTraslado; }
            set { anioBuscarTraslado = value; RaisePropertyChanged("AnioBuscarTraslado"); }
        }

        private int? folioBuscarTraslado;
        public int? FolioBuscarTraslado
        {
            get { return folioBuscarTraslado; }
            set { folioBuscarTraslado = value; RaisePropertyChanged("FolioBuscarTraslado"); }
        }

        private string nombreBuscarTraslado;
        public string NombreBuscarTraslado
        {
            get { return nombreBuscarTraslado; }
            set { nombreBuscarTraslado = value; RaisePropertyChanged("NombreBuscarTraslado"); }
        }

        private string apellidoPaternoBuscarTraslado;
        public string ApellidoPaternoBuscarTraslado
        {
            get { return apellidoPaternoBuscarTraslado; }
            set { apellidoPaternoBuscarTraslado = value; RaisePropertyChanged("ApellidoPaternoBuscarTraslado"); }
        }

        private string apellidoMaternoBuscarTraslado;
        public string ApellidoMaternoBuscarTraslado
        {
            get { return apellidoMaternoBuscarTraslado; }
            set { apellidoMaternoBuscarTraslado = value; RaisePropertyChanged("ApellidoMaternoBuscarTraslado"); }
        }

        private DateTime? fechaBuscarTraslado;
        public DateTime? FechaBuscarTraslado
        {
            get { return fechaBuscarTraslado; }
            set { fechaBuscarTraslado = value; RaisePropertyChanged("FechaBuscarTraslado"); }
        }

        private ObservableCollection<TRASLADO> busquedaTraslado;
        public ObservableCollection<TRASLADO> BusquedaTraslado
        {
            get { return busquedaTraslado; }
            set { busquedaTraslado = value; RaisePropertyChanged("BusquedaTraslado"); }
        }
        #endregion
        #region Traslados

        private TRASLADO selectedTraslado;
        public TRASLADO SelectedTraslado
        {
            get { return selectedTraslado; }
            set { selectedTraslado = value; OnPropertyChanged("SelectedTraslado"); }
        }
        #endregion
        #region reportes
        private List<EXT_REPORTE_TRASLADO_DETALLE> ds_detalle;
        private List<EXT_REPORTE_TRASLADO_ENCABEZADO> ds_encabezado;
        #endregion
        #region Datos Excarcelaciones
        private ObservableCollection<CT_EXCARCELACION_DESTINO> excarcelacion_destinos;
        public ObservableCollection<CT_EXCARCELACION_DESTINO> Excarcelacion_Destinos
        {
            get { return excarcelacion_destinos; }
            set { excarcelacion_destinos = value; RaisePropertyChanged("Excarcelacion_Destinos"); }
        }

        private string tituloExcarcelaciones = string.Empty;
        public string TituloExcarcelaciones
        {
            get { return tituloExcarcelaciones; }
            set { tituloExcarcelaciones = value; RaisePropertyChanged("TituloExcarcelaciones"); }
        }
        #endregion

        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                {
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

                if (selectIngreso.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_TRASLADO && selectIngreso.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                {
                    TextBotonSeleccionarIngreso = "aceptar";
                    SelectIngresoEnabled = true;
                }
                else
                {
                    TextBotonSeleccionarIngreso = "seleccionar ingreso";
                    SelectIngresoEnabled = false;
                }
                OnPropertyChanged("SelectIngreso");
            }
        }

        private INGRESO selectIngresoAuxiliar = null;

        DateTime _FechaServer = Fechas.GetFechaDateServer;
        public DateTime FechaServer
        {
            get { return _FechaServer; }
            set
            {
                _FechaServer = value;
                OnPropertyChanged("FechaServer");
            }
        }

        private short?[] estatus_inactivos = null;

    }
}
