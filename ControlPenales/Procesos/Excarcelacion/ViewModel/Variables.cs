using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using SSP.Servidor.ModelosExtendidos;

namespace ControlPenales
{
    partial class ExcarcelacionViewModel
    {

        #region Variables Internas
        private enum MODO_DESTINO
        {
            INSERTAR,
            EDICION
        }
        private MODO_DESTINO _modo_destino=MODO_DESTINO.INSERTAR;

        private DateTime? _fecha_programada_Old = null;

        private enum MODO_CANCELACION
        {
            GLOBAL=1,
            INDIVIDUAL=2
        }

        private MODO_CANCELACION _modo_cancelacion;
        #endregion

        #region Variables de Validaciones
        private bool isFechaIniValida = true;
        public bool IsFechaIniValida
        {
            get { return isFechaIniValida; }
            set { isFechaIniValida = value; OnPropertyChanged("IsFechaIniValida"); }
        }

        private bool isFechaIniBusquedaValida = true;
        public bool IsFechaIniBusquedaValida
        {
            get { return isFechaIniBusquedaValida; }
            set { isFechaIniBusquedaValida = value; OnPropertyChanged("IsFechaIniBusquedaValida"); }
        }

        private bool isDocumentoAgregado = false;
        public bool IsDocumentoAgregado
        {
            get { return isDocumentoAgregado; }
            set { isDocumentoAgregado = value; OnPropertyChanged("IsDocumentoAgregado"); }
        }
        #endregion

        #region Variables para Habilitar/Deshabilitar Controles
        private bool isDatosExcarcelacionEnabled=true;
        public bool IsDatosExcarcelacionEnabled
        {
            get { return isDatosExcarcelacionEnabled; }
            set { isDatosExcarcelacionEnabled = value; OnPropertyChanged("IsDatosExcarcelacionEnabled"); }
        }

        private bool isBuscarDocumentoEnabled = true;
        public bool IsBuscarDocumentoEnabled
        {
            get { return isBuscarDocumentoEnabled; }
            set { isBuscarDocumentoEnabled = value; OnPropertyChanged("IsBuscarDocumentoEnabled"); }
        }
        private bool isSeleccionarDocumentoEnabled;
        public bool IsSeleccionarDocumentoEnabled
        {
            get { return isSeleccionarDocumentoEnabled; }
            set { isSeleccionarDocumentoEnabled = value; OnPropertyChanged("IsSeleccionarDocumentoEnabled"); }
        }
        private bool isDocumentoSistemaEnabled = false;
        public bool IsDocumentoSistemaEnabled
        {
            get { return isDocumentoSistemaEnabled; }
            set { isDocumentoSistemaEnabled = value; OnPropertyChanged("IsDocumentoSistemaEnabled"); }
        }
        private bool cancelarMenuEnabled = false;
        public bool CancelarMenuEnabled
        {
            get { return cancelarMenuEnabled; }
            set { cancelarMenuEnabled = value; OnPropertyChanged("CancelarMenuEnabled"); }
        }

        private bool eliminarMenuEnabled = false;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }

        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
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

        private bool menuLimpiarEnabled = false;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set { menuLimpiarEnabled = value; OnPropertyChanged("MenuLimpiarEnabled"); }
        }

        private bool isExcarcelacion_TiposEnabled = true;
        public bool IsExcarcelacion_TiposEnabled
        {
            get { return isExcarcelacion_TiposEnabled; }
            set { isExcarcelacion_TiposEnabled = value; OnPropertyChanged("IsExcarcelacion_TiposEnabled"); }
        }

        private bool isDocumentoFisicoEnabled = true;
        public bool IsDocumentoFisicoEnabled
        {
            get { return isDocumentoFisicoEnabled; }
            set { isDocumentoFisicoEnabled = value; OnPropertyChanged("IsDocumentoFisicoEnabled"); }
        }

        private bool isBuscarCPEnabled = true;
        public bool IsBuscarCPEnabled
        {
            get { return isBuscarCPEnabled; }
            set { isBuscarCPEnabled = value; RaisePropertyChanged("IsBuscarCPEnabled"); }
        }

        private bool isCertificadoEnabled;
        public bool IsCertificadoEnabled
        {
            get { return isCertificadoEnabled; }
            set { isCertificadoEnabled = value; OnPropertyChanged("IsCertificadoEnabled"); }
        }

        #endregion

        #region Variables de Visualizacion de Controles
        private Visibility isDatosVisible = Visibility.Visible;
        public Visibility IsDatosVisible
        {
            get { return isDatosVisible; }
            set { isDatosVisible = value; OnPropertyChanged("IsDatosVisible"); }
        }
        private Visibility isJuridicaVisible = Visibility.Visible;
        public Visibility IsJuridicaVisible
        {
            get { return isJuridicaVisible; }
            set { isJuridicaVisible = value; OnPropertyChanged("IsJuridicaVisible"); }
        }
        private Visibility isMedicaVisible = Visibility.Collapsed;
        public Visibility IsMedicaVisible
        {
            get { return isMedicaVisible; }
            set { isMedicaVisible = value; OnPropertyChanged("IsMedicaVisible"); }
        }

        private Visibility isOtroHospitalVisible = Visibility.Hidden;
        public Visibility IsOtroHospitalVisible
        {
            get { return isOtroHospitalVisible; }
            set { isOtroHospitalVisible = value; OnPropertyChanged("IsOtroHospitalVisible"); }
        }

        #region Buscar Excarcelaciones
        private Visibility isDatosVisibleBuscarExc = Visibility.Collapsed;
        public Visibility IsDatosVisibleBuscarExc
        {
            get { return isDatosVisibleBuscarExc; }
            set { isDatosVisibleBuscarExc = value; OnPropertyChanged("IsDatosVisibleBuscarExc"); }
        }
        private Visibility isJuridicaVisibleBuscarExc = Visibility.Collapsed;
        public Visibility IsJuridicaVisibleBuscarExc
        {
            get { return isJuridicaVisibleBuscarExc; }
            set { isJuridicaVisibleBuscarExc = value; OnPropertyChanged("IsJuridicaVisibleBuscarExc"); }
        }
        private Visibility isMedicaVisibleBuscarExc = Visibility.Collapsed;
        public Visibility IsMedicaVisibleBuscarExc
        {
            get { return isMedicaVisibleBuscarExc; }
            set { isMedicaVisibleBuscarExc = value; OnPropertyChanged("IsMedicaVisibleBuscarExc"); }
        }

        private Visibility isOtroHospitalVisibleBuscarExc = Visibility.Hidden;
        public Visibility IsOtroHospitalVisibleBuscarExc
        {
            get { return isOtroHospitalVisibleBuscarExc; }
            set { isOtroHospitalVisibleBuscarExc = value; OnPropertyChanged("IsOtroHospitalVisibleBuscarExc"); }
        }
        #endregion
        #endregion

        #region Datos Expediente
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

        private byte[] imagenIngreso; //compartido entre datos y busqueda
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; OnPropertyChanged("ImagenIngreso"); }
        }

        private bool buscarImputadoHabilitado = true;
        public bool BuscarImputadoHabilitado
        {
            get { return buscarImputadoHabilitado; }
            set { buscarImputadoHabilitado = value; OnPropertyChanged("BuscarImputadoHabilitado"); }
        }

        private bool nombreBuscarHabilitado = true;
        public bool NombreBuscarHabilitado
        {
            get { return nombreBuscarHabilitado; }
            set { nombreBuscarHabilitado = value; OnPropertyChanged("NombreBuscarHabilitado"); }
        }

        private bool apellidoMaternoBuscarHabilitado = true;
        public bool ApellidoMaternoBuscarHabilitado
        {
            get { return apellidoMaternoBuscarHabilitado; }
            set { apellidoMaternoBuscarHabilitado = value; OnPropertyChanged("ApellidoMaternoBuscarHabilitado"); }
        }

        private bool apellidoPaternoBuscarHabilitado = true;
        public bool ApellidoPaternoBuscarHabilitado
        {
            get { return apellidoPaternoBuscarHabilitado; }
            set { apellidoPaternoBuscarHabilitado = value; OnPropertyChanged("ApellidoPaternoBuscarHabilitado"); }
        }

        private bool folioBuscarHabilitado = true;
        public bool FolioBuscarHabilitado
        {
            get { return folioBuscarHabilitado; }
            set { folioBuscarHabilitado = value; OnPropertyChanged("FolioBuscarHabilitado"); }
        }

        private bool anioBuscarHabilitado = true;
        public bool AnioBuscarHabilitado
        {
            get { return anioBuscarHabilitado; }
            set { anioBuscarHabilitado = value; OnPropertyChanged("AnioBuscarHabilitado"); }
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
                        EmptyIngresoVisible = true;
                        SelectIngreso = null;
                    }
                        

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
                    TextBotonSeleccionarIngreso = "aceptar";
                    SelectIngresoEnabled = true;

                    if (estatus_inactivos != null && SelectIngreso!=null &&  estatus_inactivos.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO))
                    {
                        TextBotonSeleccionarIngreso = "seleccionar ingreso";
                        SelectIngresoEnabled = false;
                    }
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

        #region Datos Excarcelacion
        private ObservableCollection<EXCARCELACION_TIPO> excarcelacion_tipos;
        public ObservableCollection<EXCARCELACION_TIPO> Excarcelacion_Tipos
        {
            get { return excarcelacion_tipos; }
            set { excarcelacion_tipos = value; OnPropertyChanged("Excarcelacion_Tipos"); }
        }

        private short selectedExc_tipoValue;
        public short SelectedExc_TipoValue
        {
            get { return selectedExc_tipoValue; }
            set
            {
                selectedExc_tipoOldValue = selectedExc_tipoValue;
                selectedExc_tipoValue = value;
                if (value != 0)
                    OnPropertyValidateChanged("SelectedExc_TipoValue");
                else
                    OnPropertyChanged("SelectedExc_TipoValue");
            }
            
        }

        private short selectedExc_tipoOldValue;

        private DateTime? excarcelacion_fecha;
        public DateTime? Excarcelacion_Fecha
        {
            get { return excarcelacion_fecha; }
            set 
            {
                excarcelacion_fecha = value;
                if (value != null)
                    OnPropertyValidateChanged("Excarcelacion_Fecha");
                else
                    OnPropertyChanged("Excarcelacion_Fecha"); }
        }

        private string observaciones;

        public string Observaciones
        {
            get { return observaciones; }
            set 
            { 
                observaciones = value;
                if (!string.IsNullOrWhiteSpace(observaciones))
                    OnPropertyValidateChanged("Observaciones");
                else
                    OnPropertyChanged("Observaciones");
            }
        }

        private string headerDatos;
        public string HeaderDatos
        {
            get { return headerDatos; }
            set { headerDatos = value; OnPropertyChanged("HeaderDatos"); }
        }

        private byte[] documento = null;
        public byte[] Documento
        {
            get { return documento; }
            set 
            {
                documento = value;
                if (value!=null)
                {
                    OnPropertyValidateChanged("Documento");
                    IsDocumentoAgregado = true;
                }
                else
                {
                    OnPropertyChanged("Documento");
                    IsDocumentoAgregado = false;
                }
            }
        }

        private short? tipo_documento_excarcelacion=null;
        public short? Tipo_Documento_Excarcelacion
        {
            get { return tipo_documento_excarcelacion; }
            set 
            {
                tipo_documento_excarcelacion = value;
                if (value!=null)
                    OnPropertyValidateChanged("Tipo_Documento_Excarcelacion");
                else
                    OnPropertyChanged("Tipo_Documento_Excarcelacion");
            }
        }

        private short? formato_documentacion_excarcelacion=null;
        public short? Formato_Documentacion_Excarcelacion
        {
            get{return formato_documentacion_excarcelacion;}
            set
            {
                formato_documentacion_excarcelacion=value;
                if(value!=null)
                    OnPropertyValidateChanged("Formato_Documentacion_Excarcelacion");
                else
                    OnPropertyChanged("Formato_Documentacion_Excarcelacion");
            }
        }

        private string folio_doc=string.Empty;
        public string Folio_Doc
        {
            get { return folio_doc; }
            set 
            {
                folio_doc = value;
                if (!string.IsNullOrWhiteSpace(value))
                    OnPropertyValidateChanged("Folio_Doc");
                else
                    OnPropertyChanged("Folio_Doc");
            }
        }

        private bool certMedicoSiChecked = false;
        public bool CertMedicoSiChecked
        {
            get { return certMedicoSiChecked; }
            set { certMedicoSiChecked = value; OnPropertyChanged("CertMedicoSiChecked"); }
        }

        private bool certMedicoNoChecked = true;
        public bool CertMedicoNoChecked
        {
            get { return certMedicoNoChecked; }
            set { certMedicoNoChecked = value; OnPropertyChanged("CertMedicoNoChecked"); }
        }


        #endregion

        #region agregar destino

        private string cp_folio_destino = string.Empty;
        public string CP_Folio_Destino
        {
            get { return cp_folio_destino; }
            set { cp_folio_destino = value; RaisePropertyChanged("CP_Folio_Destino"); }
        }

        #region Juzgados
        private ObservableCollection<PAIS_NACIONALIDAD> paises;
        public ObservableCollection<PAIS_NACIONALIDAD> Paises
        {
            get { return paises; }
            set { paises = value; OnPropertyChanged("Paises"); }
        }

        private short selectedPaisValue;
        public short SelectedPaisValue
        {
            get { return selectedPaisValue; }
            set
            {
                selectedPaisValue = value;
                if (value != 0)
                    OnPropertyValidateChanged("SelectedPaisValue");
                else
                    OnPropertyChanged("SelectedPaisValue");
            }
        }

        private ObservableCollection<ENTIDAD> estados;
        public ObservableCollection<ENTIDAD> Estados
        {
            get { return estados; }
            set { estados = value; OnPropertyChanged("Estados"); }
        }

        private short selectedEstadoValue;
        public short SelectedEstadoValue
        {
            get { return selectedEstadoValue; }
            set
            {
                selectedEstadoValue = value;
                if (value != 0)
                    OnPropertyValidateChanged("SelectedEstadoValue");
                else
                    OnPropertyChanged("SelectedEstadoValue");
            }
        }

        private ObservableCollection<MUNICIPIO> municipios;
        public ObservableCollection<MUNICIPIO> Municipios
        {
            get { return municipios; }
            set { municipios = value; OnPropertyChanged("Municipios"); }
        }

        private short selectedMunicipioValue;
        public short SelectedMunicipioValue
        {
            get { return selectedMunicipioValue; }
            set
            {
                selectedMunicipioValue = value;
                if (value != 0)
                    OnPropertyValidateChanged("SelectedMunicipioValue");
                else
                    OnPropertyChanged("SelectedMunicipioValue");
            }
        }

        private ObservableCollection<FUERO> fueros;
        public ObservableCollection<FUERO> Fueros
        {
            get { return fueros; }
            set { fueros = value; OnPropertyChanged("Fueros"); }
        }

        private string selectedFueroValue;
        public string SelectedFueroValue
        {
            get { return selectedFueroValue; }
            set
            {
                selectedFueroValue = value;
                if (value != "0")
                    OnPropertyValidateChanged("SelectedFueroValue");
                else
                    OnPropertyChanged("SelectedFueroValue");
            }
        }

        private ObservableCollection<JUZGADO> juzgados;
        public ObservableCollection<JUZGADO> Juzgados
        {
            get { return juzgados; }
            set { juzgados = value; OnPropertyChanged("Juzgados"); }
        }

        private short selectedJuzgadoValue;
        public short SelectedJuzgadoValue
        {
            get { return selectedJuzgadoValue; }
            set
            {
                selectedJuzgadoValue = value;
                if (value != 0)
                    OnPropertyValidateChanged("SelectedJuzgadoValue");
                else
                    OnPropertyChanged("SelectedJuzgadoValue");
            }
        }
        #endregion

        #region Hospital
        private ObservableCollection<HOSPITAL> hospitales;
        public ObservableCollection<HOSPITAL> Hospitales
        {
            get { return hospitales; }
            set { hospitales = value; OnPropertyChanged("Hospitales"); }
        }

        private short selectedHospitalValue;
        public short SelectedHospitalValue
        {
            get { return selectedHospitalValue; }
            set
            {
                selectedHospitalValue = value;
                if (value != 0)
                    OnPropertyValidateChanged("SelectedHospitalValue");
                else
                    OnPropertyChanged("SelectedHospitalValue");
            }
        }

        private string otroHospital = string.Empty;
        public string OtroHospital
        {
            get { return otroHospital; }
            set { otroHospital = value; OnPropertyValidateChanged("OtroHospital"); }
        }

        #endregion

        #region CAUSA PENAL
        private CAUSA_PENAL cp_excarcelacion_destino;
        public CAUSA_PENAL CP_Excarcelacion_Destino
        {
            get { return cp_excarcelacion_destino; }
            set { cp_excarcelacion_destino = value; OnPropertyChanged("CP_Excarcelacion_Destino"); }
        }
        #endregion

        private ObservableCollection<CT_EXCARCELACION_DESTINO> listaExcarcelacionDestinos;
        public ObservableCollection<CT_EXCARCELACION_DESTINO> ListaExcarcelacionDestinos
        {
            get { return listaExcarcelacionDestinos; }
            set { listaExcarcelacionDestinos = value; OnPropertyChanged("ListaExcarcelacionDestinos"); }
        }

        private CT_EXCARCELACION_DESTINO selectedExcarcelacionDestino;
        public CT_EXCARCELACION_DESTINO SelectedExcarcelacionDestino
        {
            get { return selectedExcarcelacionDestino; }
            set { selectedExcarcelacionDestino = value; OnPropertyChanged("SelectedExcarcelacionDestino"); }
        }

        #endregion

        #region Motivo de Cancelacion
        private ObservableCollection<EXCARCELACION_CANCELA_MOTIVO> cancelacion_Motivos;
        public ObservableCollection<EXCARCELACION_CANCELA_MOTIVO> Cancelacion_Motivos
        {
            get { return cancelacion_Motivos; }
            set { cancelacion_Motivos = value; OnPropertyChanged("Cancelacion_Motivos"); }
        }

        private short selectedCancelacion_MotivoValue;
        public short SelectedCancelacion_MotivoValue
        {
            get { return selectedCancelacion_MotivoValue; }
            set { selectedCancelacion_MotivoValue = value; OnPropertyChanged("SelectedCancelacion_MotivoValue"); }
        }

        private string cancelacion_Observacion=string.Empty;
        public string Cancelacion_Observacion
        {
            get { return cancelacion_Observacion; }
            set { cancelacion_Observacion = value; OnPropertyChanged("Cancelacion_Observacion"); }
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
        private byte[] documentoDigitalizado=null;
        public byte[] DocumentoDigitalizado
        {
            get { return documentoDigitalizado; }
            set { documentoDigitalizado = value; OnPropertyChanged("DocumentoDigitalizado"); }
        }

        private bool isObservacionesEscanerEnabled = false;
        public bool IsObservacionesEscanerEnabled
        {
            get { return isObservacionesEscanerEnabled; }
            set { isObservacionesEscanerEnabled = value; OnPropertyChanged("IsObservacionesEscanerEnabled"); }
        }

        #endregion

        #region Listado de Documentos
        private ObservableCollection<DOCUMENTO_SISTEMA> lstDocumentos;
        public ObservableCollection<DOCUMENTO_SISTEMA> LstDocumentos
        {
            get { return lstDocumentos; }
            set { lstDocumentos = value; OnPropertyChanged("LstDocumentos"); }
        }

        private DOCUMENTO_SISTEMA selectedDocumento;
        public DOCUMENTO_SISTEMA SelectedDocumento
        {
            get { return selectedDocumento; }
            set { selectedDocumento = value; OnPropertyChanged("SelectedDocumento"); }
        }

        private string buscarNUCMensaje;
        public string BuscarNUCMensaje
        {
            get { return buscarNUCMensaje; }
            set { buscarNUCMensaje = value; OnPropertyChanged("BuscarNUCMensaje"); }
        }

        private string buscarCausaPenalMensaje;
        public string BuscarCausaPenalMensaje
        {
            get 
            {
                if (buscarCausaPenalMensaje == null)
                    return buscarCausaPenalMensaje;
                return new Converters().MascaraTocaPenal(buscarCausaPenalMensaje);
            }
            set { buscarCausaPenalMensaje = value; OnPropertyChanged("BuscarCausaPenalMensaje"); }
        }

        private DateTime? fechaInicioMensaje = null;
        public DateTime? FechaInicioMensaje
        {
            get { return fechaInicioMensaje; }
            set { fechaInicioMensaje = value; OnPropertyChanged("FechaInicioMensaje"); }
        }

        private DateTime? fechaFinalMensaje = null;
        public DateTime? FechaFinalMensaje
        {
            get { return fechaFinalMensaje; }
            set { fechaFinalMensaje = value; OnPropertyChanged("FechaFinalMensaje"); }
        }

        #endregion

        #region Busqueda Excarcelacion
        private short? anioBuscarExc = null;
        public short? AnioBuscarExc
        {
            get { return anioBuscarExc; }
            set { anioBuscarExc = value; OnPropertyChanged("AnioBuscarExc"); }
        }
        private short? folioBuscarExc;
        public short? FolioBuscarExc
        {
            get { return folioBuscarExc; }
            set { folioBuscarExc = value; OnPropertyChanged("FolioBuscarExc"); }
        }

        private string nombreBuscarExc = string.Empty;
        public string NombreBuscarExc
        {
            get { return nombreBuscarExc; }
            set { nombreBuscarExc = value; OnPropertyChanged("NombreBuscarExc"); }
        }

        private string apellidoPaternoBuscarExc = string.Empty;
        public string ApellidoPaternoBuscarExc
        {
            get { return apellidoPaternoBuscarExc; }
            set { apellidoPaternoBuscarExc = value; OnPropertyChanged("ApellidoPaternoBuscarExc"); }
        }

        private string apellidoMaternoBuscarExc = string.Empty;
        public string ApellidoMaternoBuscarExc
        {
            get { return apellidoMaternoBuscarExc; }
            set { apellidoMaternoBuscarExc = value; OnPropertyChanged("ApellidoMaternoBuscarExc"); }
        }
        private DateTime? fechaInicialBuscarExc = null;
        public DateTime? FechaInicialBuscarExc
        {
            get { return fechaInicialBuscarExc; }
            set { fechaInicialBuscarExc = value; OnPropertyChanged("FechaInicialBuscarExc"); }
        }

        private DateTime? fechaFinalBuscarExc = null;
        public DateTime? FechaFinalBuscarExc
        {
            get { return fechaFinalBuscarExc; }
            set { fechaFinalBuscarExc = value; OnPropertyChanged("FechaFinalBuscarExc"); }
        }

        private ObservableCollection<EXCARCELACION_TIPO> excarcelacion_TiposBuscar;
        public ObservableCollection<EXCARCELACION_TIPO> Excarcelacion_TiposBuscar
        {
            get { return excarcelacion_TiposBuscar; }
            set { excarcelacion_TiposBuscar = value; OnPropertyChanged("Excarcelacion_TiposBuscar"); }
        }

        private short selectedExc_TipoBuscarValue;
        public short SelectedExc_TipoBuscarValue
        {
            get { return selectedExc_TipoBuscarValue;}
            set { selectedExc_TipoBuscarValue = value; OnPropertyChanged("SelectedExc_TipoBuscarValue"); }
        }

        private ObservableCollection<EXCARCELACION_DATOS> listaExcarcelacionesBusqueda=null;
        public ObservableCollection<EXCARCELACION_DATOS> ListaExcarcelacionesBusqueda
        {
            get { return listaExcarcelacionesBusqueda; }
            set { listaExcarcelacionesBusqueda = value; OnPropertyChanged("ListaExcarcelacionesBusqueda"); }
        }

        private EXCARCELACION_DATOS selectedExcarcelacionBusqueda;
        public EXCARCELACION_DATOS SelectedExcarcelacionBusqueda
        {
            get { return selectedExcarcelacionBusqueda; }
            set { selectedExcarcelacionBusqueda = value; OnPropertyChanged("SelectedExcarcelacionBusqueda"); }
        }

        private EXCARCELACION selectedExcarcelacion;
        public EXCARCELACION SelectedExcarcelacion
        {
            get { return selectedExcarcelacion; }
            set { selectedExcarcelacion = value; OnPropertyChanged("SelectedExcarcelacion"); }
        }

        private string headerDatosBuscarExc;
        public string HeaderDatosBuscarExc
        {
            get { return headerDatosBuscarExc; }
            set { headerDatosBuscarExc = value; OnPropertyChanged("HeaderDatosBuscarExc"); }
        }

        #region Juzgados
        private ObservableCollection<PAIS_NACIONALIDAD> paisesBuscarExc;
        public ObservableCollection<PAIS_NACIONALIDAD> PaisesBuscarExc
        {
            get { return paisesBuscarExc; }
            set { paisesBuscarExc = value; OnPropertyChanged("PaisesBuscarExc"); }
        }

        private short selectedPaisBuscarExcValue;
        public short SelectedPaisBuscarExcValue
        {
            get { return selectedPaisBuscarExcValue; }
            set
            {
                selectedPaisBuscarExcValue = value;
                OnPropertyChanged("SelectedPaisBuscarExcValue");
            }
        }

        private ObservableCollection<ENTIDAD> estadosBuscarExc;
        public ObservableCollection<ENTIDAD> EstadosBuscarExc
        {
            get { return estadosBuscarExc; }
            set { estadosBuscarExc = value; OnPropertyChanged("EstadosBuscarExc"); }
        }

        private short selectedEstadoBuscarExcValue;
        public short SelectedEstadoBuscarExcValue
        {
            get { return selectedEstadoBuscarExcValue; }
            set
            {
                selectedEstadoBuscarExcValue = value;
                OnPropertyChanged("SelectedEstadoBuscarExcValue");
            }
        }

        private ObservableCollection<MUNICIPIO> municipiosBuscarExc;
        public ObservableCollection<MUNICIPIO> MunicipiosBuscarExc
        {
            get { return municipiosBuscarExc; }
            set { municipiosBuscarExc = value; OnPropertyChanged("MunicipiosBuscarExc"); }
        }

        private short selectedMunicipioBuscarExcValue;
        public short SelectedMunicipioBuscarExcValue
        {
            get { return selectedMunicipioBuscarExcValue; }
            set
            {
                selectedMunicipioBuscarExcValue = value;
                OnPropertyChanged("SelectedMunicipioBuscarExcValue");
            }
        }

        private ObservableCollection<FUERO> fuerosBuscarExc;
        public ObservableCollection<FUERO> FuerosBuscarExc
        {
            get { return fuerosBuscarExc; }
            set { fuerosBuscarExc = value; OnPropertyChanged("FuerosBuscarExc"); }
        }

        private string selectedFueroBuscarExcValue;
        public string SelectedFueroBuscarExcValue
        {
            get { return selectedFueroBuscarExcValue; }
            set
            {
                selectedFueroBuscarExcValue = value;
                OnPropertyChanged("SelectedFueroBuscarExcValue");
            }
        }

        private ObservableCollection<JUZGADO> juzgadosBuscarExc;
        public ObservableCollection<JUZGADO> JuzgadosBuscarExc
        {
            get { return juzgadosBuscarExc; }
            set { juzgadosBuscarExc = value; OnPropertyChanged("JuzgadosBuscarExc"); }
        }

        private short selectedJuzgadoBuscarExcValue;
        public short SelectedJuzgadoBuscarExcValue
        {
            get { return selectedJuzgadoBuscarExcValue; }
            set
            {
                selectedJuzgadoBuscarExcValue = value;
                OnPropertyChanged("SelectedJuzgadoBuscarExcValue");
            }
        }
        #endregion

        #region Hospital
        private ObservableCollection<HOSPITAL> hospitalesBuscarExc;
        public ObservableCollection<HOSPITAL> HospitalesBuscarExc
        {
            get { return hospitalesBuscarExc; }
            set { hospitalesBuscarExc = value; OnPropertyChanged("HospitalesBuscarExc"); }
        }

        private short selectedHospitalBuscarExcValue;
        public short SelectedHospitalBuscarExcValue
        {
            get { return selectedHospitalBuscarExcValue; }
            set
            {
                selectedHospitalBuscarExcValue = value;
                OnPropertyChanged("SelectedHospitalBuscarExcValue");
            }
        }

        private string otroHospitalBuscarExc = string.Empty;
        public string OtroHospitalBuscarExc
        {
            get { return otroHospitalBuscarExc; }
            set { otroHospitalBuscarExc = value; OnPropertyValidateChanged("OtroHospitalBuscarExc"); }
        }

        #endregion
        #endregion

        #region Busqueda Causa Penales
        private string buscar_Causa_Penal;
        public string Buscar_Causa_Penal
        {
            get
            {
                if (buscar_Causa_Penal == null)
                    return string.Empty;
                
                return new Converters().MascaraTocaPenal(buscar_Causa_Penal);
            }
            set { buscar_Causa_Penal = value; RaisePropertyChanged("Buscar_Causa_Penal"); }
        }
        #endregion

        #region Busqueda Imputados por NUC
        private string headerNUC = string.Empty;
        public string HeaderNUC
        {
            get { return "Busqueda por NUC " + headerNUC; }
            set { headerNUC = value; OnPropertyChanged("HeaderNUC"); }
        }

        private ObservableCollection<CAUSA_PENAL_INGRESOS> listadoIngresoNUC;
        public ObservableCollection<CAUSA_PENAL_INGRESOS> ListadoIngresoNUC
        {
            get { return listadoIngresoNUC; }
            set { listadoIngresoNUC = value; OnPropertyChanged("ListadoIngresoNUC"); }
        }

        private CAUSA_PENAL_INGRESOS selectedIngresoNUC;
        public CAUSA_PENAL_INGRESOS SelectedIngresoNUC
        {
            get { return selectedIngresoNUC; }
            set { selectedIngresoNUC = value; OnPropertyChanged("SelectedIngresoNUC"); }
        }

        private byte[] imagenIngresoNUC;
        public byte[] ImagenIngresoNUC
        {
            get { return imagenIngresoNUC; }
            set { imagenIngresoNUC = value; OnPropertyChanged("ImagenIngresoNUC"); }
        }

        #region Variables de Visualizacion de Controles
        private Visibility isFotoVisibleNUC = Visibility.Collapsed;
        public Visibility IsFotoVisibleNUC
        {
            get { return isFotoVisibleNUC; }
            set { isFotoVisibleNUC = value; OnPropertyChanged("IsFotoVisibleNUC"); }
        }
        #endregion

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
                    OnPropertyChanged("SelectIngreso");
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

        private INGRESO selectIngresoAuxiliar=null;

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


        #region Interconsulta Medica
        private ObservableCollection<CustomGridInterconexionMedica> lstInterconsultaMedica;
        public ObservableCollection<CustomGridInterconexionMedica> LstInterconsultaMedica
        {
            get { return lstInterconsultaMedica; }
            set { lstInterconsultaMedica = value; OnPropertyChanged("LstInterconsultaMedica"); }
        }

        private CustomGridInterconexionMedica _SelectedInterconsultaExcarcelacion;
        public CustomGridInterconexionMedica SelectedInterconsultaExcarcelacion
        {
            get { return _SelectedInterconsultaExcarcelacion; }
            set { _SelectedInterconsultaExcarcelacion = value; OnPropertyChanged("SelectedInterconsultaExcarcelacion"); }
        }


        public class CustomGridInterconexionMedica
        {
            public string NombreDestino { get; set;}
            public string NombrePrioridad { get; set;}
            public string NombreTipoAtencion{ get; set;}
            public short? IdDestino{ get; set;}
            public int IdInterconsulta { get; set;}
            public string NombreMedico { get; set; }
            public DateTime? FechaInterconsulta { get; set; }
            public string FolioInterConsulta { get; set; }
        };

        private Visibility _VisibleDatosExcarcelacionDestino = Visibility.Visible;

        public Visibility VisibleDatosExcarcelacionDestino
        {
            get { return _VisibleDatosExcarcelacionDestino; }
            set { _VisibleDatosExcarcelacionDestino = value; OnPropertyChanged("VisibleDatosExcarcelacionDestino"); }
        }


        #endregion

        private short?[] estatus_inactivos = null;

        private DateTime? _FechaMinimaExcarcelacion;

        public DateTime? FechaMinimaExcarcelacion
        {
            get { return _FechaMinimaExcarcelacion; }
            set { _FechaMinimaExcarcelacion = value; OnPropertyChanged("FechaMinimaExcarcelacion"); }
        }

        private DateTime? _FechaMaximaExcarcelacion;

        public DateTime? FechaMaximaExcarcelacion
        {
            get { return _FechaMaximaExcarcelacion; }
            set { _FechaMaximaExcarcelacion = value; OnPropertyChanged("FechaMaximaExcarcelacion"); }
        }
    }
}
