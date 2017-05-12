using ControlPenales;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;

namespace ControlPenales
{
    partial class RegistroPersonalViewModel
    {
        #region Empleados
        private bool Actualiza = false;
        private bool NuevoEmpleado = false;
        private SSP.Servidor.PERSONA selectPersona;
        public SSP.Servidor.PERSONA SelectPersona
        {
            get { return selectPersona; }
            set { selectPersona = value; }
        }
        private EMPLEADO selectedEmpleado;
        public EMPLEADO SelectedEmpleado
        {
            get { return selectedEmpleado; }
            set
            {
                selectedEmpleado = value;
                if (value != null)
                {
                    if (value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        ImagenEmpleadoPop = value.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        ImagenEmpleadoPop = new Imagenes().getImagenPerson();
                }
                else
                    ImagenEmpleadoPop = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedEmpleado");
            }
        }

        private ObservableCollection<TIPO_VISITANTE> lstTipoVisitante;
        public ObservableCollection<TIPO_VISITANTE> LstTipoVisitante
        {
            get { return lstTipoVisitante; }
            set { lstTipoVisitante = value; OnPropertyChanged("LstTipoVisitante"); }
        }

        private ObservableCollection<ESTATUS_VISITA> lstEstatusVisita;
        public ObservableCollection<ESTATUS_VISITA> LstEstatusVisita
        {
            get { return lstEstatusVisita; }
            set { lstEstatusVisita = value; OnPropertyChanged("LstEstatusVisita"); }
        }

        private ObservableCollection<PAIS_NACIONALIDAD> lstPais;
        public ObservableCollection<PAIS_NACIONALIDAD> LstPais
        {
            get { return lstPais; }
            set
            {
                lstPais = value;
                OnPropertyChanged("LstPais");
            }
        }

        private PAIS_NACIONALIDAD selectedPais;
        public PAIS_NACIONALIDAD SelectedPais
        {
            get { return selectedPais; }
            set
            {
                selectedPais = value;
                if (value != null)
                {
                    if (value.ID_PAIS_NAC != -1)
                        LstEstado = new ObservableCollection<ENTIDAD>(value.ENTIDAD);
                    else
                        LstEstado = new ObservableCollection<ENTIDAD>();
                }
                else
                    LstEstado = new ObservableCollection<ENTIDAD>();
                LstEstado.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                EEstado = -1;
                //ValidarEmpleado();
                OnPropertyChanged("SelectedPais");
            }
        }

        private ObservableCollection<ENTIDAD> lstEstado;
        public ObservableCollection<ENTIDAD> LstEstado
        {
            get { return lstEstado; }
            set { lstEstado = value; OnPropertyChanged("LstEstado"); }
        }

        private ENTIDAD selectedEstado;
        public ENTIDAD SelectedEstado
        {
            get { return selectedEstado; }
            set
            {
                selectedEstado = value;
                if (value != null)
                {
                    if (value.ID_ENTIDAD != -1)
                        LstMunicipio = new ObservableCollection<MUNICIPIO>(value.MUNICIPIO);
                    else
                        LstMunicipio = new ObservableCollection<MUNICIPIO>();
                }
                else
                    LstMunicipio = new ObservableCollection<MUNICIPIO>();
                LstMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                EMunicipio = -1;
                //ValidarEmpleado();
                OnPropertyChanged("SelectedEstado");
            }
        }

        private ObservableCollection<MUNICIPIO> lstMunicipio;
        public ObservableCollection<MUNICIPIO> LstMunicipio
        {
            get { return lstMunicipio; }
            set { lstMunicipio = value; OnPropertyChanged("LstMunicipio"); }
        }

        private MUNICIPIO selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return selectedMunicipio; }
            set
            {
                selectedMunicipio = value;
                if (value != null)
                {
                    if (value.ID_MUNICIPIO != -1)
                        LstColonia = new ObservableCollection<COLONIA>(value.COLONIA);
                    else
                        LstColonia = new ObservableCollection<COLONIA>();
                }
                else
                    LstColonia = new ObservableCollection<COLONIA>();
                LstColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                EColonia = -1;
                OnPropertyChanged("SelectedMunicipio");
            }
        }

        private ObservableCollection<COLONIA> lstColonia;
        public ObservableCollection<COLONIA> LstColonia
        {
            get { return lstColonia; }
            set { lstColonia = value; OnPropertyChanged("LstColonia"); }
        }

        private ObservableCollection<TIPO_DISCAPACIDAD> lstTipoDiscapacidad;
        public ObservableCollection<TIPO_DISCAPACIDAD> LstTipoDiscapacidad
        {
            get { return lstTipoDiscapacidad; }
            set { lstTipoDiscapacidad = value; OnPropertyChanged("LstTipoDiscapacidad"); }
        }

        private ObservableCollection<TIPO_EMPLEADO> lstTipoEmpleado;
        public ObservableCollection<TIPO_EMPLEADO> LstTipoEmpleado
        {
            get { return lstTipoEmpleado; }
            set { lstTipoEmpleado = value; OnPropertyChanged("LstTipoEmpleado"); }
        }
        private ObservableCollection<DEPARTAMENTO> lstAreaTrabajo;
        public ObservableCollection<DEPARTAMENTO> LstAreaTrabajo
        {
            get { return lstAreaTrabajo; }
            set { lstAreaTrabajo = value; OnPropertyChanged("LstAreaTrabajo"); }
        }

        private int? eCodigo;
        public int? ECodigo
        {
            get { return eCodigo; }
            set { eCodigo = value; OnPropertyValidateChanged("ECodigo"); }
        }

        private string ePaterno;
        public string EPaterno
        {
            get { return ePaterno; }
            set
            {
                ePaterno = value;

                if (!string.IsNullOrEmpty(ENombre) && !string.IsNullOrEmpty(EPaterno) && !string.IsNullOrEmpty(EMaterno) && EFechaNacimiento != null)
                {
                    ERFC = CURPRFC.CalcularRFC(ENombre, EPaterno, EMaterno, string.Format("{0:yyMMdd}", EFechaNacimiento));
                    ECURP = CURPRFC.CalcularCURP(ENombre, EPaterno, EMaterno, string.Format("{0:yyMMdd}", EFechaNacimiento));
                }

                #region Validaciones
                if (base.FindRule("ENombre"))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        base.RemoveRule("EMaterno");
                        OnPropertyChanged("EMaterno");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(EMaterno))
                        {
                            base.RemoveRule("EPaterno");
                            base.AddRule(() => EPaterno, () => !string.IsNullOrEmpty(EPaterno), "APELLIDO PATERNO ES REQUERIDO!");

                            base.RemoveRule("EMaterno");
                            base.AddRule(() => EMaterno, () => !string.IsNullOrEmpty(EMaterno), "APELLIDO MATERNO ES REQUERIDO!");
                            OnPropertyChanged("EMaterno");
                        }
                    }
                }
                #endregion
                OnPropertyValidateChanged("EPaterno");
            }
        }

        private string eMaterno;
        public string EMaterno
        {
            get { return eMaterno; }
            set
            {
                eMaterno = value;
                if (!string.IsNullOrEmpty(ENombre) && !string.IsNullOrEmpty(EPaterno) && !string.IsNullOrEmpty(EMaterno) && EFechaNacimiento != null)
                {
                    ERFC = CURPRFC.CalcularRFC(ENombre, EPaterno, EMaterno, string.Format("{0:yyMMdd}", EFechaNacimiento));
                    ECURP = CURPRFC.CalcularCURP(ENombre, EPaterno, EMaterno, string.Format("{0:yyMMdd}", EFechaNacimiento));
                }

                #region Validaciones
                if (base.FindRule("ENombre"))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        base.RemoveRule("EPaterno");
                        OnPropertyChanged("EPaterno");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(EPaterno))
                        {
                            base.RemoveRule("EMaterno");
                            base.AddRule(() => EMaterno, () => !string.IsNullOrEmpty(EMaterno), "APELLIDO MATERNO ES REQUERIDO!");

                            base.RemoveRule("EPaterno");
                            base.AddRule(() => EPaterno, () => !string.IsNullOrEmpty(EPaterno), "APELLIDO PATERNO ES REQUERIDO!");
                            OnPropertyChanged("EPaterno");
                        }
                    }   
                }
                #endregion

                OnPropertyValidateChanged("EMaterno");
            }
        }

        private string eNombre;
        public string ENombre
        {
            get { return eNombre; }
            set
            {
                eNombre = value;
                if (!string.IsNullOrEmpty(ENombre) && !string.IsNullOrEmpty(EPaterno) && !string.IsNullOrEmpty(EMaterno) && EFechaNacimiento != null)
                {
                    ERFC = CURPRFC.CalcularRFC(ENombre, EPaterno, EMaterno, string.Format("{0:yyMMdd}", EFechaNacimiento));
                    ECURP = CURPRFC.CalcularCURP(ENombre, EPaterno, EMaterno, string.Format("{0:yyMMdd}", EFechaNacimiento));
                }
                OnPropertyValidateChanged("ENombre");
            }
        }

        private short? eTipoVisitante = -1;
        public short? ETipoVisitante
        {
            get { return eTipoVisitante; }
            set { eTipoVisitante = value; OnPropertyValidateChanged("ETipoVisitante"); }
        }

        private string eSexo = string.Empty;
        public string ESexo
        {
            get { return eSexo; }
            set { eSexo = value; OnPropertyValidateChanged("ESexo"); }
        }

        private bool eFechaNacimientoValid = true;
        public bool EFechaNacimientoValid
        {
            get { return eFechaNacimientoValid; }
            set { eFechaNacimientoValid = value; OnPropertyChanged("EFechaNacimientoValid"); }
        }

        private string eFechaNacimientoMensaje = "Fecha de nacimiento es requerida!";
        public string EFechaNacimientoMensaje
        {
            get { return eFechaNacimientoMensaje; }
            set { eFechaNacimientoMensaje = value; OnPropertyChanged("EFechaNacimientoMensaje"); }
        }

        private DateTime? eFechaNacimiento;
        public DateTime? EFechaNacimiento
        {
            get { return eFechaNacimiento; }
            set
            {
                eFechaNacimiento = value;
                if (value != null)
                {
                    DateTime hoy = Fechas.GetFechaDateServer;
                    if (value.Value.Date > hoy.Date.Date)
                    {
                        EFechaNacimientoMensaje = "La fecha debe ser menor al dia de hoy!";
                        EFechaNacimientoValid = false;
                    }
                    else
                    {
                        EFechaNacimientoValid = true;
                    }


                }
                else
                {
                    EFechaNacimientoMensaje = "Fecha de nacmiento es requerida!";
                    EFechaNacimientoValid = false;
                }

                if (!string.IsNullOrEmpty(ENombre) && !string.IsNullOrEmpty(EPaterno) && !string.IsNullOrEmpty(EMaterno) && EFechaNacimiento != null)
                {
                    ERFC = CURPRFC.CalcularRFC(ENombre, EPaterno, EMaterno, string.Format("{0:yyMMdd}", EFechaNacimiento));
                    ECURP = CURPRFC.CalcularCURP(ENombre, EPaterno, EMaterno, string.Format("{0:yyMMdd}", EFechaNacimiento));
                }

                OnPropertyValidateChanged("EFechaNacimiento");
            }
        }

        private short? eSituacion = -1;
        public short? ESituacion
        {
            get { return eSituacion; }
            set { eSituacion = value; OnPropertyValidateChanged("ESituacion"); }
        }

        private string eCURP;
        public string ECURP
        {
            get { return eCURP; }
            set { eCURP = value; OnPropertyValidateChanged("ECURP"); }
        }

        private string eRFC;
        public string ERFC
        {
            get { return eRFC; }
            set { eRFC = value; OnPropertyValidateChanged("ERFC"); }
        }

        private string eTelefonoFijo;
        public string ETelefonoFijo
        {
            get
            {
                if (eTelefonoFijo == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(eTelefonoFijo);
            }
            set { eTelefonoFijo = value; OnPropertyValidateChanged("ETelefonoFijo"); }
        }

        private string eTelefonoMovil;
        public string ETelefonoMovil
        {
            get
            {
                if (eTelefonoMovil == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(eTelefonoMovil);
            }
            set { eTelefonoMovil = value; OnPropertyValidateChanged("ETelefonoMovil"); }
        }

        private string eCorreo;
        public string ECorreo
        {
            get { return eCorreo; }
            set { eCorreo = value; OnPropertyValidateChanged("ECorreo"); }
        }

        private string eNip;
        public string ENip
        {
            get { return eNip; }
            set { eNip = value; OnPropertyChanged("ENip"); }
        }

        private DateTime? eFechaAlta;
        public DateTime? EFechaAlta
        {
            get { return eFechaAlta; }
            set { eFechaAlta = value; OnPropertyChanged("EFechaAlta"); }
        }
        //Domicilio
        private short? ePais;
        public short? EPais
        {
            get { return ePais; }
            set
            {
                ePais = value;
                //ValidarEmpleado();
                OnPropertyValidateChanged("EPais");
            }
        }

        private short? eEstado;
        public short? EEstado
        {
            get { return eEstado; }
            set
            {
                eEstado = value;
                //ValidarEmpleado();
                OnPropertyValidateChanged("EEstado");
            }
        }

        private short? eMunicipio;
        public short? EMunicipio
        {
            get { return eMunicipio; }
            set
            {
                eMunicipio = value;
                //ValidarEmpleado();
                OnPropertyValidateChanged("EMunicipio");
            }
        }

        private int? eColonia;
        public int? EColonia
        {
            get { return eColonia; }
            set { eColonia = value; OnPropertyValidateChanged("EColonia"); }
        }

        private string eCalle;
        public string ECalle
        {
            get { return eCalle; }
            set { eCalle = value; OnPropertyValidateChanged("ECalle"); }
        }

        private int? eNoExterior;
        public int? ENoExterior
        {
            get { return eNoExterior; }
            set { eNoExterior = value; OnPropertyValidateChanged("ENoExterior"); }
        }

        private string eNoInterior;
        public string ENoInterior
        {
            get { return eNoInterior; }
            set { eNoInterior = value; OnPropertyValidateChanged("ENoInterior"); }
        }

        private int? eCP;
        public int? ECP
        {
            get { return eCP; }
            set { eCP = value; OnPropertyValidateChanged("ECP"); }
        }
        //Discapacidad
        private string eDiscapacidad = string.Empty;
        public string EDiscapacidad
        {
            get { return eDiscapacidad; }
            set
            {
                eDiscapacidad = value;
                //ValidarEmpleado();
                if (value == "S")
                {
                    ETipoDiscapacidadEnabled = true;
                }
                else
                {
                    ETipoDiscapacidadEnabled = false;
                    ETipoDiscapacidad = -1;
                }
                OnPropertyValidateChanged("EDiscapacidad");
            }
        }

        private short? eTipoDiscapacidad = -1;
        public short? ETipoDiscapacidad
        {
            get { return eTipoDiscapacidad; }
            set { eTipoDiscapacidad = value; OnPropertyValidateChanged("ETipoDiscapacidad"); }
        }
        //Observacion
        private string eObservacion;
        public string EObservacion
        {
            get { return eObservacion; }
            set { eObservacion = value; OnPropertyValidateChanged("EObservacion"); }
        }
        //Empleado
        private short? eTipoEmpleado = -1;
        public short? ETipoEmpleado
        {
            get { return eTipoEmpleado; }
            set { eTipoEmpleado = value; OnPropertyValidateChanged("ETipoEmpleado"); }
        }

        private short? eAreaTrabajo = -1;
        public short? EAreaTrabajo
        {
            get { return eAreaTrabajo; }
            set { eAreaTrabajo = value; OnPropertyValidateChanged("EAreaTrabajo"); }
        }

        private bool eTipoDiscapacidadEnabled = false;
        public bool ETipoDiscapacidadEnabled
        {
            get { return eTipoDiscapacidadEnabled; }
            set { eTipoDiscapacidadEnabled = value; OnPropertyChanged("ETipoDiscapacidadEnabled"); }
        }

        private string eCedulaProfesional;
        public string ECedulaProfesional
        {
            get { return eCedulaProfesional; }
            set { eCedulaProfesional = value; OnPropertyValidateChanged("ECedulaProfesional"); }
        }

        private ObservableCollection<CENTRO> lstCentro;
        public ObservableCollection<CENTRO> LstCentro
        {
            get { return lstCentro; }
            set { lstCentro = value; RaisePropertyChanged("LstCentro"); }
        }

        private short selectedCentroValue;
        public short SelectedCentroValue
        {
            get { return selectedCentroValue; }
            set { selectedCentroValue = value; RaisePropertyChanged("SelectedCentroValue"); }
        }
        #endregion

        #region Menu
        private bool guardarMenuEnabled = false;
        public bool GuardarMenuEnabled
        {
            get { return guardarMenuEnabled; }
            set { guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }
        
        private bool agregarMenuEnabled = false;
        public bool AgregarMenuEnabled
        {
            get { return agregarMenuEnabled; }
            set { agregarMenuEnabled = value; RaisePropertyChanged("AgregarMenuEnabled"); }
        }
        private bool buscarMenuEnabled = false;
        public bool BuscarMenuEnabled
        {
            get { return buscarMenuEnabled; }
            set { buscarMenuEnabled = value; OnPropertyChanged("BuscarMenuEnabled"); }
        }
        private bool eliminarMenuEnabled = false;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; RaisePropertyChanged("EliminarMenuEnabled"); }
        }

        private bool editarMenuEnabled = false;
        public bool EditarMenuEnabled
        {
            get { return editarMenuEnabled; }
            set { editarMenuEnabled = value; RaisePropertyChanged("EditarMenuEnabled"); }
        }

        private bool cancelarMenuEnabled;
        public bool CancelarMenuEnabled
        {
            get { return cancelarMenuEnabled; }
            set { cancelarMenuEnabled = value; RaisePropertyChanged("CancelarMenuEnabled"); }
        }
        #endregion

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set
            {
                pInsertar = value;
                if (value)
                    AgregarMenuEnabled = value;
            }
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
                    BuscarMenuEnabled = value;
            }
        }
        #endregion

        #region Buscar
        private int Pagina { get; set; }
        private bool SeguirCargandoPersonas { get; set; }
        private bool _HeaderSortin;
        public bool HeaderSortin
        {
            get { return _HeaderSortin; }
            set { _HeaderSortin = value; }
        }
        private int? nipE;
        public int? NipE
        {
            get { return nipE; }
            set { nipE = value; OnPropertyChanged("NipE"); }
        }

        private string paternoE;
        public string PaternoE
        {
            get { return paternoE; }
            set { paternoE = value; OnPropertyChanged("PaternoE"); }
        }

        private string maternoE;
        public string MaternoE
        {
            get { return maternoE; }
            set { maternoE = value; OnPropertyChanged("MaternoE"); }
        }

        private string nombreE;
        public string NombreE
        {
            get { return nombreE; }
            set { nombreE = value; OnPropertyChanged("NombreE"); }
        }

        private byte[] imagenEmpleadoPop = new Imagenes().getImagenPerson();
        public byte[] ImagenEmpleadoPop
        {
            get { return imagenEmpleadoPop; }
            set { imagenEmpleadoPop = value; OnPropertyChanged("ImagenEmpleadoPop"); }
        }

        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> lstEmpleadoPopAux;
        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> lstEmpleadoPop;
        public RangeEnabledObservableCollection<SSP.Servidor.PERSONA> LstEmpleadoPop
        {
            get { return lstEmpleadoPop; }
            set { lstEmpleadoPop = value; OnPropertyChanged("LstEmpleadoPop"); }
        }

        private SSP.Servidor.PERSONA selectedEmpleadoPop;
        public SSP.Servidor.PERSONA SelectedEmpleadoPop
        {
            get { return selectedEmpleadoPop; }
            set
            {
                selectedEmpleadoPop = value;
                if (value != null)
                {
                    if (value.PERSONA_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                        ImagenEmpleadoPop = value.PERSONA_BIOMETRICO.First(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).BIOMETRICO;
                    else
                        ImagenEmpleadoPop = new Imagenes().getImagenPerson();
                }
                else
                    ImagenEmpleadoPop = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedEmpleadoPop");
            }
        }

        private Visibility empleadoEmpty = Visibility.Collapsed;
        public Visibility EmpleadoEmpty
        {
            get { return empleadoEmpty; }
            set { empleadoEmpty = value; OnPropertyChanged("EmpleadoEmpty"); }
        }
        #endregion

        #region Fotos
        private ControlPenales.Clases.WebCam CamaraWeb;

        private ImageSourceToSave _FotoFrente;
        public ImageSourceToSave FotoFrente
        {
            get { return _FotoFrente; }
            set { _FotoFrente = value; }
        }

        private List<ImageSourceToSave> _ImagesToSave;
        public List<ImageSourceToSave> ImagesToSave
        {
            get { return _ImagesToSave; }
            set { _ImagesToSave = value; }
        }

        private List<ImageSourceToSave> _ImageFrontal;
        public List<ImageSourceToSave> ImageFrontal
        {
            get { return _ImageFrontal; }
            set { _ImageFrontal = value; }
        }

        private bool _Processing = false;
        public bool Processing
        {
            get { return _Processing; }
            set
            {
                _Processing = value;
                OnPropertyChanged("Processing");
            }
        }

        private bool botonTomarFotoEnabled = false;
        public bool BotonTomarFotoEnabled
        {
            get { return botonTomarFotoEnabled; }
            set { botonTomarFotoEnabled = value; OnPropertyChanged("BotonTomarFotoEnabled"); }
        }

        private Visibility comboFrontBackFotoVisible = Visibility.Collapsed;
        public Visibility ComboFrontBackFotoVisible
        {
            get { return comboFrontBackFotoVisible; }
            set { comboFrontBackFotoVisible = value; OnPropertyChanged("ComboFrontBackFotoVisible"); }
        }

        private byte[] imagenEmpleado = new Imagenes().getImagenPerson();
        public byte[] ImagenEmpleado
        {
            get { return imagenEmpleado; }
            set { imagenEmpleado = value; OnPropertyValidateChanged("ImagenEmpleado"); }
        }

        private bool FotoTomada = false;
        #endregion

        #region Huellas
        private Visibility _ShowPopUp = Visibility.Hidden;
        public Visibility ShowPopUp
        {
            get { return _ShowPopUp; }
            set
            {
                _ShowPopUp = value;
                OnPropertyChanged("ShowPopUp");
            }
        }

        private Visibility _ShowFingerPrint = Visibility.Hidden;
        public Visibility ShowFingerPrint
        {
            get { return _ShowFingerPrint; }
            set
            {
                _ShowFingerPrint = value;
                OnPropertyChanged("ShowFingerPrint");
            }
        }

        private Visibility _ShowLine = Visibility.Visible;
        public Visibility ShowLine
        {
            get { return _ShowLine; }
            set
            {
                _ShowLine = value;
                OnPropertyChanged("ShowLine");
            }
        }

        private Visibility _ShowOk = Visibility.Hidden;
        public Visibility ShowOk
        {
            get { return _ShowOk; }
            set
            {
                _ShowOk = value;
                OnPropertyChanged("ShowOk");
            }
        }

        IList<PlantillaBiometrico> HuellasCapturadas;

        private System.Windows.Media.ImageSource _GuardaHuella;
        public System.Windows.Media.ImageSource GuardaHuella
        {
            get { return _GuardaHuella; }
            set
            {
                _GuardaHuella = value;
                OnPropertyChanged("GuardaHuella");
            }
        }
        #endregion

        #region Pantalla
        private bool enabledCampo = false;
        public bool EnabledCampo
        {
            get { return enabledCampo; }
            set { enabledCampo = value; OnPropertyChanged("EnabledCampo"); }
        }

        private bool validar = false;
        #endregion
    }
}
