using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlPenales
{
    partial class PadronActuariosViewModel
    {

        #region Text
        private string _TextCedulaLabelBack = "Cédula Trasera";
        public string TextCedulaLabelBack
        {
            get { return _TextCedulaLabelBack; }
            set { _TextCedulaLabelBack = value; OnPropertyChanged("TextCedulaLabelBack"); }
        }
        private string _TextCedulaLabelFront = "Cédula Frente";
        public string TextCedulaLabelFront
        {
            get { return _TextCedulaLabelFront; }
            set { _TextCedulaLabelFront = value; OnPropertyChanged("TextCedulaLabelFront"); }
        }
        string _ObservacionDocumento;
        public string ObservacionDocumento
        {
            get { return _ObservacionDocumento; }
            set
            {
                _ObservacionDocumento = value;
                OnPropertyChanged("ObservacionDocumento");
            }
        }
        private string textNombre;
        public string TextNombre
        {
            get { return textNombre; }
            set { textNombre = value; OnPropertyChanged("TextNombre"); }
        }
        private string textPaterno;
        public string TextPaterno
        {
            get { return textPaterno; }
            set { textPaterno = value; OnPropertyChanged("TextPaterno"); }
        }
        private string textMaterno;
        public string TextMaterno
        {
            get { return textMaterno; }
            set { textMaterno = value; OnPropertyChanged("TextMaterno"); }
        }
        private string textCodigo;
        public string TextCodigo
        {
            get { return textCodigo; }
            set { textCodigo = value; OnPropertyChanged("TextCodigo"); }
        }
        private string textNombreAbogado;
        public string TextNombreAbogado
        {
            get { return textNombreAbogado; }
            set
            {
                textNombreAbogado = value;
                if (!Editable && !string.IsNullOrEmpty(TextNombreAbogado) && !string.IsNullOrEmpty(TextPaternoAbogado)/*!&& SelectFechaNacimiento.HasValue) && string.IsNullOrEmpty(TextMaternoAbogado)*/)
                {
                    TextRfc = CURPRFC.CalcularRFC(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, SelectFechaNacimiento.ToString("yyMMdd"));
                    TextCurp = CURPRFC.CalcularCURP(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, SelectFechaNacimiento.ToString("yyMMdd"));
                }
                if (!Editable)
                    OnPropertyValidateChanged("TextNombreAbogado");
                else
                    OnPropertyChanged("TextNombreAbogado");
            }
        }
        private string textPaternoAbogado;
        public string TextPaternoAbogado
        {
            get { return textPaternoAbogado; }
            set
            {
                textPaternoAbogado = value;
                if (!Editable && !string.IsNullOrEmpty(TextNombreAbogado) && !string.IsNullOrEmpty(TextPaternoAbogado)/*!&& SelectFechaNacimiento.HasValue) && string.IsNullOrEmpty(TextMaternoAbogado)*/)
                {
                    TextRfc = CURPRFC.CalcularRFC(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, SelectFechaNacimiento.ToString("yyMMdd"));
                    TextCurp = CURPRFC.CalcularCURP(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, SelectFechaNacimiento.ToString("yyMMdd"));
                }
               
                #region Validaciones
                if (base.FindRule("TextNombreAbogado"))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        base.RemoveRule("TextMaternoAbogado");
                        OnPropertyChanged("TextMaternoAbogado");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(TextMaternoAbogado))
                        {
                            base.RemoveRule("TextPaternoAbogado");
                            base.AddRule(() => TextPaternoAbogado, () => !string.IsNullOrEmpty(TextPaternoAbogado), "APELLIDO PATERNO ES REQUERIDO!");

                            base.RemoveRule("TextMaternoAbogado");
                            base.AddRule(() => TextMaternoAbogado, () => !string.IsNullOrEmpty(TextMaternoAbogado), "APELLIDO MATERNO ES REQUERIDO!");
                            OnPropertyChanged("TextMaternoAbogado");
                        }
                    }
                }
                #endregion

                if (!Editable)
                    OnPropertyValidateChanged("TextPaternoAbogado");
                else
                    OnPropertyChanged("TextPaternoAbogado");
            }
        }
        private string textMaternoAbogado;
        public string TextMaternoAbogado
        {
            get { return textMaternoAbogado; }
            set
            {
                textMaternoAbogado = value;
                if (!Editable && !string.IsNullOrEmpty(TextNombreAbogado) && !string.IsNullOrEmpty(TextPaternoAbogado)/*!&& SelectFechaNacimiento.HasValue) && string.IsNullOrEmpty(TextMaternoAbogado)*/)
                {
                    TextRfc = CURPRFC.CalcularRFC(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, SelectFechaNacimiento.ToString("yyMMdd"));
                    TextCurp = CURPRFC.CalcularCURP(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, SelectFechaNacimiento.ToString("yyMMdd"));
                }

                #region Notificaciones
                if (base.FindRule("TextNombreAbogado"))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        base.RemoveRule("TextPaternoAbogado");
                        OnPropertyChanged("TextPaternoAbogado");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(TextPaternoAbogado))
                        {
                            base.RemoveRule("TextMaternoAbogado");
                            base.AddRule(() => TextMaternoAbogado, () => !string.IsNullOrEmpty(TextMaternoAbogado), "APELLIDO MATERNO ES REQUERIDO!");

                            base.RemoveRule("TextPaternoAbogado");
                            base.AddRule(() => TextPaternoAbogado, () => !string.IsNullOrEmpty(TextPaternoAbogado), "APELLIDO PATERNO ES REQUERIDO!");
                            OnPropertyChanged("TextPaternoAbogado");
                        }
                    }
                }
                #endregion

                if (!Editable)
                    OnPropertyValidateChanged("TextMaternoAbogado");
                else
                    OnPropertyChanged("TextMaternoAbogado");
            }
        }
        private string textCodigoAbogado;
        public string TextCodigoAbogado
        {
            get { return textCodigoAbogado; }
            set { textCodigoAbogado = value; OnPropertyChanged("TextCodigoAbogado"); }
        }
        private string textCurp;
        public string TextCurp
        {
            get { return textCurp; }
            set { textCurp = value; OnPropertyValidateChanged("TextCurp"); }
        }
        private string textRfc;
        public string TextRfc
        {
            get { return textRfc; }
            set { textRfc = value; OnPropertyValidateChanged("TextRfc"); }
        }
        private string textTelefonoFijo;
        public string TextTelefonoFijo
        {
            get
            {
                if (textTelefonoFijo == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(textTelefonoFijo);
            }
            set { textTelefonoFijo = value; OnPropertyValidateChanged("TextTelefonoFijo"); }
        }
        private string textTelefonoMovil;
        public string TextTelefonoMovil
        {
            get
            {
                if (textTelefonoMovil == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(textTelefonoMovil);
            }
            set { textTelefonoMovil = value; OnPropertyValidateChanged("TextTelefonoMovil"); }
        }
        private string textCorreo;
        public string TextCorreo
        {
            get { return textCorreo; }
            set { textCorreo = value; OnPropertyValidateChanged("TextCorreo"); }
        }
        private string textIne;
        public string TextIne
        {
            get { return textIne; }
            set { textIne = value; OnPropertyValidateChanged("TextIne"); }
        }
        private string cedulaCJF = "Credencial CJF";
        public string CedulaCJF
        {
            get { return cedulaCJF; }
            set { cedulaCJF = value; OnPropertyChanged("CedulaCJF"); }
        }
        private string textCedulaCJF;
        public string TextCedulaCJF
        {
            get { return textCedulaCJF; }
            set { textCedulaCJF = value; OnPropertyValidateChanged("TextCedulaCJF"); }
        }
        //private string textNip;
        //public string TextNip
        //{
        //    get { return textNip; }
        //    set { textNip = value; OnPropertyValidateChanged("TextNip"); }
        //}
        private string textCalle;
        public string TextCalle
        {
            get { return textCalle; }
            set { textCalle = value; OnPropertyValidateChanged("TextCalle"); }
        }
        private int? textNumExt;
        public int? TextNumExt
        {
            get { return textNumExt; }
            set { textNumExt = value; OnPropertyValidateChanged("TextNumExt"); }
        }
        private string textNumInt;
        public string TextNumInt
        {
            get { return textNumInt; }
            set { textNumInt = value; OnPropertyValidateChanged("TextNumInt"); }
        }
        private int? textCodigoPostal;
        public int? TextCodigoPostal
        {
            get { return textCodigoPostal; }
            set { textCodigoPostal = value; OnPropertyValidateChanged("TextCodigoPostal"); }
        }
        private string textObservacion;
        public string TextObservaciones
        {
            get { return textObservacion; }
            set { textObservacion = value; OnPropertyValidateChanged("TextObservaciones"); }
        }
        #endregion

        #region List
        private ObservableCollection<JUZGADO> _ListJuzgado;
        public ObservableCollection<JUZGADO> ListJuzgado
        {
            get { return _ListJuzgado; }
            set { _ListJuzgado = value; OnPropertyChanged("ListJuzgado"); }
        }
        private ObservableCollection<TipoDocumento> AuxListTipoDocumento;
        ObservableCollection<TipoDocumento> _ListTipoDocumento;
        public ObservableCollection<TipoDocumento> ListTipoDocumento
        {
            get { return _ListTipoDocumento; }
            set
            {
                _ListTipoDocumento = value;
                OnPropertyChanged("ListTipoDocumento");
            }
        }
        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> listPersonas;
        public RangeEnabledObservableCollection<SSP.Servidor.PERSONA> ListPersonas
        {
            get { return listPersonas; }
            set { listPersonas = value; OnPropertyChanged("ListPersonas"); }
        }
        private ObservableCollection<PAIS_NACIONALIDAD> listPais;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPais
        {
            get { return listPais; }
            set { listPais = value; OnPropertyChanged("ListPais"); }
        }
        private ObservableCollection<ENTIDAD> listEntidad;
        public ObservableCollection<ENTIDAD> ListEntidad
        {
            get { return listEntidad; }
            set { listEntidad = value; OnPropertyChanged("ListEntidad"); }
        }
        private ObservableCollection<MUNICIPIO> listMunicipio;
        public ObservableCollection<MUNICIPIO> ListMunicipio
        {
            get { return listMunicipio; }
            set { listMunicipio = value; OnPropertyChanged("ListMunicipio"); }
        }
        private ObservableCollection<COLONIA> listColonia;
        public ObservableCollection<COLONIA> ListColonia
        {
            get { return listColonia; }
            set { listColonia = value; OnPropertyChanged("ListColonia"); }
        }
        private ObservableCollection<TIPO_DISCAPACIDAD> listTipoDiscapacidad;
        public ObservableCollection<TIPO_DISCAPACIDAD> ListTipoDiscapacidad
        {
            get { return listTipoDiscapacidad; }
            set { listTipoDiscapacidad = value; OnPropertyChanged("ListTipoDiscapacidad"); }
        }
        private ObservableCollection<ESTATUS_VISITA> _ListEstatus;
        public ObservableCollection<ESTATUS_VISITA> ListEstatus
        {
            get { return _ListEstatus; }
            set { _ListEstatus = value; OnPropertyChanged("ListEstatus"); }
        }
        #endregion

        #region Select
        private short? selectJuzgado;
        public short? SelectJuzgado
        {
            get { return selectJuzgado; }
            set { selectJuzgado = value; OnPropertyValidateChanged("SelectJuzgado"); }
        }
        private string selectSexo;
        public string SelectSexo
        {
            get { return selectSexo; }
            set { selectSexo = value; OnPropertyValidateChanged("SelectSexo"); }
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
        private string selectDiscapacitado;
        public string SelectDiscapacitado
        {
            get { return selectDiscapacitado; }
            set
            {
                selectDiscapacitado = value;
                DiscapacidadEnabled = value == "S";
                SelectTipoDiscapacidad = (short)(value == "N" ? 0 : -1);
                if (!string.IsNullOrEmpty(value))
                    SetValidaciones();
                OnPropertyChanged("SelectDiscapacitado");
            }
        }
        private short? selectTipoDiscapacidad;
        public short? SelectTipoDiscapacidad
        {
            get { return selectTipoDiscapacidad; }
            set { selectTipoDiscapacidad = value; OnPropertyValidateChanged("SelectTipoDiscapacidad"); }
        }
        private short? selectPais;
        public short? SelectPais
        {
            get { return selectPais; }
            set
            {
                selectPais = value;
                if (value > 0)
                    ListEntidad = new ObservableCollection<ENTIDAD>((new cEntidad()).ObtenerTodos().OrderBy(o => o.DESCR));
                else
                    ListEntidad = new ObservableCollection<ENTIDAD>();
                ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                if (value == 82)//Mexico
                {
                    SelectEntidad = 2;//Baja California
                    EntidadEnabled = ValidarEnabled;
                    MunicipioEnabled = ValidarEnabled;
                    ColoniaEnabled = ValidarEnabled;
                }
                else if (value == -1)
                {
                    SelectEntidad = -1;
                    EntidadEnabled = ValidarEnabled;
                    MunicipioEnabled = ValidarEnabled;
                    ColoniaEnabled = ValidarEnabled;
                }
                else
                {
                    SelectEntidad = 33;
                    EntidadEnabled = false;
                    MunicipioEnabled = false;
                    ColoniaEnabled = false;
                }
                OnPropertyValidateChanged("SelectPais");
            }
        }
        private short? selectEntidad;
        public short? SelectEntidad
        {
            get { return selectEntidad; }
            set
            {
                if (value > 0)
                    ListMunicipio = new ObservableCollection<MUNICIPIO>((new cMunicipio()).ObtenerTodos(string.Empty, value).OrderBy(o => o.MUNICIPIO1));
                else
                    ListMunicipio = new ObservableCollection<MUNICIPIO>();
                ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                if (value == 33)
                    SelectMunicipio = 1001;
                else
                    SelectMunicipio = -1;

                selectEntidad = value;
                OnPropertyValidateChanged("SelectEntidad");
            }
        }
        private short? selectMunicipio;
        public short? SelectMunicipio
        {
            get { return selectMunicipio; }
            set
            {
                if (value > 0)
                {
                    ListColonia = new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, value, SelectEntidad).OrderBy(o => o.DESCR));
                }
                else
                    ListColonia = new ObservableCollection<COLONIA>();
                ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                if (ListColonia.Count == 1)
                {
                    ListColonia = new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, 1001).OrderBy(o => o.DESCR));
                    ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                    SelectColonia = 102;
                    ColoniaEnabled = false;
                }
                else
                {
                    if (value == 1001)
                    {
                        SelectColonia = 102;
                        ColoniaEnabled = false;
                    }
                    else
                    {
                        SelectColonia = -1;
                        ColoniaEnabled = true;
                    }
                }
                selectMunicipio = value;
                OnPropertyValidateChanged("SelectMunicipio");
            }
        }
        private int? selectColonia;
        public int? SelectColonia
        {
            get { return selectColonia; }
            set
            {
                selectColonia = value;
                OnPropertyValidateChanged("SelectColonia");
            }
        }
        private short? _SelectEstatusVisita;
        public short? SelectEstatusVisita
        {
            get { return _SelectEstatusVisita; }
            set { _SelectEstatusVisita = value; OnPropertyValidateChanged("SelectEstatusVisita"); }
        }
        private SSP.Servidor.PERSONA SelectPersonaAuxiliar;
        private SSP.Servidor.PERSONA selectPersona;
        public SSP.Servidor.PERSONA SelectPersona
        {
            get { return selectPersona; }
            set
            {
                selectPersona = value;
                AsignacionEnabled = value != null;
                ImagenPersona = value == null ?
                    new Imagenes().getImagenPerson() :
                        value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                            value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectPersona");
            }
        }
        #endregion

        #region Menu
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

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set
            {
                pInsertar = value;
                if (value)
                    MenuGuardarEnabled = value;
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
                    MenuBuscarEnabled = value;
            }
        }

        private bool PImprimir = false;
        #endregion

        #region Enableds
        private bool _MenuInsertarEnabled = true;
        public bool MenuInsertarEnabled
        {
            get { return _MenuInsertarEnabled; }
            set { _MenuInsertarEnabled = value; OnPropertyChanged("MenuInsertarEnabled"); }
        }
        private bool _BuscarAbogadoEnabled = false;
        public bool BuscarAbogadoEnabled
        {
            get { return _BuscarAbogadoEnabled; }
            set { _BuscarAbogadoEnabled = value; OnPropertyChanged("BuscarAbogadoEnabled"); }
        }
        private bool discapacitadoEnabled = false;
        public bool DiscapacitadoEnabled
        {
            get { return discapacitadoEnabled; }
            set { discapacitadoEnabled = value; OnPropertyChanged("DiscapacitadoEnabled"); }
        }
        private bool discapacidadEnabled = false;
        public bool DiscapacidadEnabled
        {
            get { return discapacidadEnabled; }
            set { discapacidadEnabled = value; OnPropertyChanged("DiscapacidadEnabled"); }
        }
        private bool entidadEnabled = false;
        public bool EntidadEnabled
        {
            get { return entidadEnabled; }
            set { entidadEnabled = value; OnPropertyChanged("EntidadEnabled"); }
        }
        private bool municipioEnabled = false;
        public bool MunicipioEnabled
        {
            get { return municipioEnabled; }
            set { municipioEnabled = value; OnPropertyChanged("MunicipioEnabled"); }
        }
        private bool coloniaEnabled = false;
        public bool ColoniaEnabled
        {
            get { return coloniaEnabled; }
            set { coloniaEnabled = value; OnPropertyChanged("ColoniaEnabled"); }
        }
        private bool validarEnabled = false;
        public bool ValidarEnabled
        {
            get { return validarEnabled; }
            set
            {
                validarEnabled = value;
                OnPropertyChanged("ValidarEnabled");
            }
        }
        #endregion

        #region Fechas
        private string fechaAlta = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy");
        public string SelectFechaAlta
        {
            get { return fechaAlta; }
            set { fechaAlta = value; OnPropertyChanged("SelectFechaAlta"); }
        }
        private DateTime fechaNacimiento = Fechas.GetFechaDateServer;
        public DateTime SelectFechaNacimiento
        {
            get { return fechaNacimiento; }
            set
            {
                fechaNacimiento = value;
                if (!Editable && !string.IsNullOrEmpty(TextNombreAbogado) && !string.IsNullOrEmpty(TextPaternoAbogado)/*!&& SelectFechaNacimiento.HasValue) && string.IsNullOrEmpty(TextMaternoAbogado)*/)
                {
                    TextRfc = CURPRFC.CalcularRFC(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, SelectFechaNacimiento.ToString("yyMMdd"));
                    TextCurp = CURPRFC.CalcularCURP(TextNombreAbogado, TextPaternoAbogado, TextMaternoAbogado, SelectFechaNacimiento.ToString("yyMMdd"));
                }
                OnPropertyValidateChanged("SelectFechaNacimiento");
            }
        }
        DateTime? _DatePickCapturaDocumento = Fechas.GetFechaDateServer;
        public DateTime? DatePickCapturaDocumento
        {
            get { return _DatePickCapturaDocumento; }
            set
            {
                _DatePickCapturaDocumento = value;
                OnPropertyChanged("DatePickCapturaDocumento");
            }
        }
        #endregion

        #region Imagenes
        private byte[] imagenAbogado = new Imagenes().getImagenPerson();
        public byte[] ImagenAbogado
        {
            get { return imagenAbogado; }
            set { imagenAbogado = value; OnPropertyValidateChanged("ImagenAbogado"); }
        }
        private byte[] ImagenAbogadoAuxiliar;
        private byte[] imagenPersona = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get { return imagenPersona; }
            set { imagenPersona = value; OnPropertyChanged("ImagenPersona"); }
        }
        public byte[] DocumentoDigitalizado { get; set; }
        #endregion

        #region Visibles
        private Visibility _JuzgadoVisible = Visibility.Visible;
        public Visibility JuzgadoVisible
        {
            get { return _JuzgadoVisible; }
            set { _JuzgadoVisible = value; OnPropertyChanged("JuzgadoVisible"); }
        }
        private Visibility comboFrontBackFotoVisible = Visibility.Collapsed;
        public Visibility ComboFrontBackFotoVisible
        {
            get { return comboFrontBackFotoVisible; }
            set { comboFrontBackFotoVisible = value; OnPropertyChanged("ComboFrontBackFotoVisible"); }
        }
        private bool emptyBuscarRelacionInternoVisible = false;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return emptyBuscarRelacionInternoVisible; }
            set { emptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }
        #endregion

        #region Tomar Fotos
        public WebCam CamaraWeb;

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
        private bool botonTomarFotoEnabled;
        public bool BotonTomarFotoEnabled
        {
            get { return botonTomarFotoEnabled; }
            set { botonTomarFotoEnabled = value; OnPropertyChanged("BotonTomarFotoEnabled"); }
        }
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
        private ImageSource _GuardaHuella;
        public ImageSource GuardaHuella
        {
            get { return _GuardaHuella; }
            set
            {
                _GuardaHuella = value;
                OnPropertyChanged("GuardaHuella");
            }
        }
        #endregion

        #region DatosImputado
        private string _AnioD;
        public string AnioD
        {
            get
            {
                return _AnioD;
            }
            set
            {
                _AnioD = value;
                OnPropertyChanged("AnioD");
            }
        }
        private string _FolioD;
        public string FolioD
        {
            get
            {
                return _FolioD;
            }
            set
            {
                _FolioD = value;
                OnPropertyChanged("FolioD");
            }
        }
        private int? _AnioBuscar;
        public int? AnioBuscar
        {
            get { return _AnioBuscar; }
            set
            {
                _AnioBuscar = value;
                OnPropertyChanged("AnioBuscar");
            }
        }
        private int? _FolioBuscar;
        public int? FolioBuscar
        {
            get { return _FolioBuscar; }
            set
            {
                _FolioBuscar = value;
                OnPropertyChanged("FolioBuscar");
            }
        }
        private string _PaternoD;
        public string PaternoD
        {
            get
            {
                return _PaternoD;
            }
            set
            {
                _PaternoD = value;
                OnPropertyChanged("PaternoD");
            }
        }
        private string _MaternoD;
        public string MaternoD
        {
            get
            {
                return _MaternoD;
            }
            set
            {
                _MaternoD = value;
                OnPropertyChanged("MaternoD");
            }
        }
        private string _NombreD;
        public string NombreD
        {
            get
            {
                return _NombreD;
            }
            set
            {
                _NombreD = value;
                OnPropertyChanged("NombreD");
            }
        }
        private string _IngresosD;
        public string IngresosD
        {
            get
            {
                return _IngresosD;
            }
            set
            {
                _IngresosD = value;
                OnPropertyChanged("IngresosD");
            }
        }
        private string _NoControlD;
        public string NoControlD
        {
            get
            {
                return _NoControlD;
            }
            set
            {
                _NoControlD = value;
                OnPropertyChanged("NoControlD");
            }
        }
        private string _UbicacionD;
        public string UbicacionD
        {
            get
            {
                return _UbicacionD;
            }
            set
            {
                _UbicacionD = value;
                OnPropertyChanged("UbicacionD");
            }
        }
        private string _TipoSeguridadD;
        public string TipoSeguridadD
        {
            get
            {
                return _TipoSeguridadD;
            }
            set
            {
                _TipoSeguridadD = value;
                OnPropertyChanged("TipoSeguridadD");
            }
        }
        private string _FecIngresoD;
        public string FecIngresoD
        {
            get
            {
                return _FecIngresoD;
            }
            set
            {
                _FecIngresoD = value;
                OnPropertyChanged("FecIngresoD");
            }
        }
        private string _ClasificacionJuridicaD;
        public string ClasificacionJuridicaD
        {
            get
            {
                return _ClasificacionJuridicaD;
            }
            set
            {
                _ClasificacionJuridicaD = value;
                OnPropertyChanged("ClasificacionJuridicaD");
            }
        }
        private string _EstatusD;
        public string EstatusD
        {
            get
            {
                return _EstatusD;
            }
            set
            {
                _EstatusD = value;
                OnPropertyChanged("EstatusD");
            }
        }
        #endregion

        #region Otros
        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> ListPersonasAuxiliar;
        private bool FotoFrenteRegistro = false;
        private bool FotoActualizada = false;
        private bool _HeaderSortin;
        public bool HeaderSortin
        {
            get { return _HeaderSortin; }
            set { _HeaderSortin = value; }
        }
        private int Pagina { get; set; }
        private bool SeguirCargandoPersonas { get; set; }
        private bool Insertable = false;
        private bool _Credencializado;
        public bool Credencializado
        {
            get { return _Credencializado; }
            set { _Credencializado = value; OnPropertyChanged("Credencializado"); }
        }
        private bool NuevoActuario;
        private bool GafeteFront;
        private bool GafeteBack;
        private bool ImpresionGafete;
        private bool ImpresionGafeteFront;
        private bool ImpresionGafeteBack;
        private bool _AsignacionEnabled;
        public bool AsignacionEnabled
        {
            get { return _AsignacionEnabled; }
            set { _AsignacionEnabled = value; OnPropertyChanged("AsignacionEnabled"); }
        }
        private bool _TabRegistro;
        public bool TabRegistro
        {
            get { return _TabRegistro; }
            set { _TabRegistro = value; OnPropertyChanged("TabRegistro"); }
        }
        private bool _TabAsignacion;
        public bool TabAsignacion
        {
            get { return _TabAsignacion; }
            set { _TabAsignacion = value; OnPropertyChanged("TabAsignacion"); }
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
        private bool editable = false;
        public bool Editable
        {
            get { return editable; }
            set
            {
                editable = value;
            }
        }
        private bool NuevoAbogado = false;
        private bool FotoTomada = false;
        private short TipoDoctoAbogado = 0;
        #endregion

        #region Gafete
        private GafetesPVCView _gafeteView;
        public GafetesPVCView GafeteView
        {
            get { return _gafeteView; }
            set { _gafeteView = value; OnPropertyChanged("GafeteView"); }
        }
        private ReportViewer reporteador;
        public ReportViewer Reporteador
        {
            get { return reporteador; }
            set { reporteador = value; OnPropertyChanged("Reporteador"); }
        }
        private string gafeteAbogado = "GafeteAbogadoFrente";
        private bool gafeteDetras;
        public bool GafeteDetras
        {
            get { return gafeteDetras; }
            set
            {
                gafeteDetras = value;
                if (value && SelectPersona != null)
                {
                    gafeteAbogado = "GafeteAbogadoDetras";

                }
                OnPropertyChanged("GafeteDetras");
            }
        }
        private bool gafeteFrente;
        public bool GafeteFrente
        {
            get { return gafeteFrente; }
            set
            {
                gafeteFrente = value;
                if (value && SelectPersona != null)
                {
                    gafeteAbogado = "GafeteAbogadoFrente";
                    //CrearGafete();
                }
                OnPropertyChanged("GafeteFrente");
            }
        }
        #endregion

    }
}
