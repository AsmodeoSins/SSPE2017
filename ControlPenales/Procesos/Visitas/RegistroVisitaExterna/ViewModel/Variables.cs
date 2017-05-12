using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlPenales
{
    partial class RegistroVisitaExternaViewModel
    {
        #region Text
        private string textCodigoNuevo;
        public string TextCodigoNuevo
        {
            get { return textCodigoNuevo; }
            set { textCodigoNuevo = value; OnPropertyChanged("TextCodigoNuevo"); }
        }

        private string textCodigo;
        public string TextCodigo
        {
            get { return textCodigo; }
            set { textCodigo = value; OnPropertyChanged("TextCodigo"); }
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
            set { textMaterno = value;
            OnPropertyChanged("TextMaterno");
            }
        }
        
        private string textPaternoNuevo;
        public string TextPaternoNuevo
        {
            get { return textPaternoNuevo; }
            set
            {
                textPaternoNuevo = value;
                //if (ValidacionesActivas)
                //    if (!string.IsNullOrEmpty(value))
                //    {
                //        base.RemoveRule("TextMaternoNuevo");
                //        OnPropertyChanged("TextMaternoNuevo");
                //    }
                //    else
                //    {
                //        base.RemoveRule("TextMaternoNuevo");
                //        base.AddRule(() => TextMaternoNuevo, () => !string.IsNullOrEmpty(TextMaternoNuevo), "APELLIDO MATERNO ES REQUERIDO!");
                //        OnPropertyChanged("TextMaternoNuevo");
                //    }
                #region Validaciones
                if (base.FindRule("TextNombreNuevo"))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        base.RemoveRule("TextMaternoNuevo");
                        OnPropertyChanged("TextMaternoNuevo");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(TextMaternoNuevo))
                        {
                            base.RemoveRule("TextPaternoNuevo");
                            base.AddRule(() => TextPaternoNuevo, () => !string.IsNullOrEmpty(TextPaternoNuevo), "APELLIDO PATERNO ES REQUERIDO!");

                            base.RemoveRule("TextMaternoNuevo");
                            base.AddRule(() => TextMaternoNuevo, () => !string.IsNullOrEmpty(TextMaternoNuevo), "APELLIDO MATERNO ES REQUERIDO!");
                            OnPropertyChanged("TextMaternoNuevo");
                        }
                    }
                }
                #endregion

                OnPropertyValidateChanged("TextPaternoNuevo");
            }
        }
        
        private string textMaternoNuevo;
        public string TextMaternoNuevo
        {
            get { return textMaternoNuevo; }
            set
            {
                textMaternoNuevo = value;
                //if (ValidacionesActivas)
                //    if (!string.IsNullOrEmpty(value))
                //    {
                //        base.RemoveRule("TextPaternoNuevo");
                //        OnPropertyChanged("TextPaternoNuevo");
                //    }
                //    else
                //    {
                //        base.RemoveRule("TextPaternoNuevo");
                //        base.AddRule(() => TextPaternoNuevo, () => !string.IsNullOrEmpty(TextPaternoNuevo), "APELLIDO PATERNO ES REQUERIDO!");
                //        OnPropertyChanged("TextPaternoNuevo");
                //    }
                #region Validaciones
                if (base.FindRule("TextNombreNuevo"))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        base.RemoveRule("TextPaternoNuevo");
                        OnPropertyChanged("TextPaternoNuevo");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(TextPaternoNuevo))
                        {
                            base.RemoveRule("TextMaternoNuevo");
                            base.AddRule(() => TextMaternoNuevo, () => !string.IsNullOrEmpty(TextMaternoNuevo), "APELLIDO MATERNO ES REQUERIDO!");

                            base.RemoveRule("TextPaternoNuevo");
                            base.AddRule(() => TextPaternoNuevo, () => !string.IsNullOrEmpty(TextPaternoNuevo), "APELLIDO PATERNO ES REQUERIDO!");
                            OnPropertyChanged("TextPaternoNuevo");
                        }
                    }  
                }
                #endregion
                OnPropertyValidateChanged("TextMaternoNuevo");
            }
        }
        
        private string textNombreNuevo;
        public string TextNombreNuevo
        {
            get { return textNombreNuevo; }
            set
            {
                textNombreNuevo = value;
                OnPropertyValidateChanged("TextNombreNuevo");
            }
        }
        
        private DateTime? fechaNacimientoNuevo;
        public DateTime? FechaNacimientoNuevo
        {
            get { return fechaNacimientoNuevo; }
            set
            {
                fechaNacimientoNuevo = value;
                if (ValidacionesActivas)
                    if (value != null)
                    {
                        base.RemoveRule("FechaNacimientoNuevo");
                        base.AddRule(() => FechaNacimientoNuevo, () => FechaNacimientoNuevo.Value.Date < FechaHoy.Date, "LA FECHA DEBE SER MENOR AL DIA DE HOY!");
                        OnPropertyChanged("FechaNacimientoNuevo");
                    }
                    else
                    {
                        base.RemoveRule("FechaNacimientoNuevo");
                        base.AddRule(() => FechaNacimientoNuevo, () => FechaNacimientoNuevo.HasValue, "FECHA DE NACIMIENTO ES REQUERIDA!");
                        OnPropertyChanged("FechaNacimientoNuevo");
                    }
                //SetValidacionesAgregarNuevaVisitaExterna();
                OnPropertyValidateChanged("FechaNacimientoNuevo");
            }
        }
        
        private string textTelefonoFijoNuevo;
        public string TextTelefonoFijoNuevo
        {
            get { return textTelefonoFijoNuevo; }
            set
            {
                textTelefonoFijoNuevo = value != null ? new Converters().MascaraTelefono(value) : value;
                OnPropertyValidateChanged("TextTelefonoFijoNuevo");
            }
        }
        
        private string textTelefonoMovilNuevo;
        public string TextTelefonoMovilNuevo
        {
            get { return textTelefonoMovilNuevo; }
            set
            {
                textTelefonoMovilNuevo = value != null ? new Converters().MascaraTelefono(value) : value;
                OnPropertyValidateChanged("TextTelefonoMovilNuevo");
            }
        }
        
        private string textCorreoNuevo;
        public string TextCorreoNuevo
        {
            get { return textCorreoNuevo; }
            set { textCorreoNuevo = value; OnPropertyValidateChanged("TextCorreoNuevo"); }
        }
        
        private DateTime? fechaAltaNuevo = Fechas.GetFechaDateServer;
        public DateTime? FechaAltaNuevo
        {
            get { return fechaAltaNuevo; }
            set { fechaAltaNuevo = value; OnPropertyValidateChanged("FechaAltaNuevo"); }
        }
        
        //private string textNIPNuevo;
        //public string TextNIPNuevo
        //{
        //    get { return textNIPNuevo; }
        //    set { textNIPNuevo = value; OnPropertyValidateChanged("TextNIPNuevo"); }
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
        
        private string textObservacionNuevo;
        public string TextObservacionNuevo
        {
            get { return textObservacionNuevo; }
            set { textObservacionNuevo = value; OnPropertyValidateChanged("TextObservacionNuevo"); }
        }
        #endregion

        #region Listas
        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> listPersonas;
        public RangeEnabledObservableCollection<SSP.Servidor.PERSONA> ListPersonas
        {
            get { return listPersonas; }
            set { listPersonas = value; OnPropertyChanged("ListPersonas"); }
        }
        
        private ObservableCollection<TIPO_VISITANTE> listTipoVisitante;
        public ObservableCollection<TIPO_VISITANTE> ListTipoVisitante
        {
            get { return listTipoVisitante; }
            set { listTipoVisitante = value; OnPropertyChanged("ListTipoVisitante"); }
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
        
        private ObservableCollection<TIPO_DISCAPACIDAD> listDiscapacidades;
        public ObservableCollection<TIPO_DISCAPACIDAD> ListDiscapacidades
        {
            get { return listDiscapacidades; }
            set { listDiscapacidades = value; OnPropertyChanged("ListDiscapacidades"); }
        }
        
        private List<Image> _Frames;
        public List<Image> Frames
        {
            get { return _Frames; }
            set { _Frames = value; }
        }
        
        private List<ImageSourceToSave> _ImagesToSave;
        public List<ImageSourceToSave> ImagesToSave
        {
            get { return _ImagesToSave; }
            set { _ImagesToSave = value; }
        }
        #endregion

        #region Selects
        private string selectSexoNuevo = string.Empty;
        public string SelectSexoNuevo
        {
            get { return selectSexoNuevo; }
            set { selectSexoNuevo = value; OnPropertyValidateChanged("SelectSexoNuevo"); }
        }
        
        private short? selectTipoVisitanteNuevo = -1;
        public short? SelectTipoVisitanteNuevo
        {
            get { return selectTipoVisitanteNuevo; }
            set
            {
                selectTipoVisitanteNuevo = value;
                DiscapacitadoNuevoEnabled = value == Parametro.ID_TIPO_VISITANTE_DISCAPACITADO;
                SetValidacionesAgregarNuevaVisitaExterna();
                OnPropertyValidateChanged("SelectTipoVisitanteNuevo");
            }
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
                    EntidadEnabled = true;
                    MunicipioEnabled = true;
                    ColoniaEnabled = true;
                }
                else if (value == -1)
                {
                    SelectEntidad = -1;
                    EntidadEnabled = true;
                    MunicipioEnabled = true;
                    ColoniaEnabled = true;
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
                    ListColonia = new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, value).OrderBy(o => o.DESCR));
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
        
        private string selectDiscapacitadoNuevo;
        public string SelectDiscapacitadoNuevo
        {
            get { return selectDiscapacitadoNuevo; }
            set
            {
                selectDiscapacitadoNuevo = value;
                SelectDiscapacidadNuevo = (short)(value == "N" ? 0 : -1);
                DiscapacidadNuevoEnabled = value == "S";
                SetValidacionesAgregarNuevaVisitaExterna();
                OnPropertyValidateChanged("SelectDiscapacitadoNuevo");
            }
        }
        
        private short? selectDiscapacidadNuevo;
        public short? SelectDiscapacidadNuevo
        {
            get { return selectDiscapacidadNuevo; }
            set
            {
                selectDiscapacidadNuevo = value;
                OnPropertyValidateChanged("SelectDiscapacidadNuevo");
            }
        }
        
        private string selectFrenteDetrasFoto = "F";
        public string SelectFrenteDetrasFoto
        {
            get { return selectFrenteDetrasFoto; }
            set
            {
                selectFrenteDetrasFoto = value;
                OnPropertyValidateChanged("SelectFrenteDetrasFoto");
            }
        }
        
        private SSP.Servidor.PERSONA selectPersona;
        public SSP.Servidor.PERSONA SelectPersona
        {
            get { return selectPersona; }
            set
            {
                selectPersona = value;
                ImagenPersona = value == null ?
                    new Imagenes().getImagenPerson() :
                        value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                            value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                new Imagenes().getImagenPerson();
                OnPropertyValidateChanged("SelectPersona");
            }
        }
        
        private SSP.Servidor.PERSONA selectPersonaAuxiliar;
        public SSP.Servidor.PERSONA SelectPersonaAuxiliar
        {
            get { return selectPersonaAuxiliar; }
            set
            {
                selectPersonaAuxiliar = value;
                OnPropertyChanged("SelectPersonaAuxiliar");
            }
        }
        #endregion

        #region Enableds
        private bool discapacitadoNuevoEnabled = false;
        public bool DiscapacitadoNuevoEnabled
        {
            get { return discapacitadoNuevoEnabled; }
            set { discapacitadoNuevoEnabled = value; OnPropertyChanged("DiscapacitadoNuevoEnabled"); }
        }

        private bool entidadEnabled;
        public bool EntidadEnabled
        {
            get { return entidadEnabled; }
            set { entidadEnabled = value; OnPropertyChanged("EntidadEnabled"); }
        }

        private bool municipioEnabled;
        public bool MunicipioEnabled
        {
            get { return municipioEnabled; }
            set { municipioEnabled = value; OnPropertyChanged("MunicipioEnabled"); }
        }

        private bool coloniaEnabled;
        public bool ColoniaEnabled
        {
            get { return coloniaEnabled; }
            set { coloniaEnabled = value; OnPropertyChanged("ColoniaEnabled"); }
        }

        private bool discapacidadNuevoEnabled;
        public bool DiscapacidadNuevoEnabled
        {
            get { return discapacidadNuevoEnabled; }
            set { discapacidadNuevoEnabled = value; OnPropertyChanged("DiscapacidadNuevoEnabled"); }
        }

        private bool validarEnabled = false;
        public bool ValidarEnabled
        {
            get { return validarEnabled; }
            set { validarEnabled = value; OnPropertyChanged("ValidarEnabled"); }
        }
        #endregion

        #region Visibles
        private Visibility comboFrontBackFotoVisible = Visibility.Visible;
        public Visibility ComboFrontBackFotoVisible
        {
            get { return comboFrontBackFotoVisible; }
            set { comboFrontBackFotoVisible = value; OnPropertyChanged("ComboFrontBackFotoVisible"); }
        }
        
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
        
        private bool emptyBuscarRelacionInternoVisible;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return emptyBuscarRelacionInternoVisible; }
            set { emptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }
        #endregion

        #region Imagenes
        private byte[] imagenVisitaExterna = new Imagenes().getImagenPerson();
        public byte[] ImagenVisitaExterna
        {
            get { return imagenVisitaExterna; }
            set { imagenVisitaExterna = value; OnPropertyChanged("ImagenVisitaExterna"); }
        }
       
        private byte[] fotoFrenteCredencial;
        public byte[] FotoFrenteCredencial
        {
            get { return fotoFrenteCredencial; }
            set
            {
                fotoFrenteCredencial = value;
                OnPropertyChanged("FotoFrenteCredencial");
            }
        }
        
        private byte[] fotoFrenteCredencialAuxiliar;
        public byte[] FotoFrenteCredencialAuxiliar
        {
            get { return fotoFrenteCredencialAuxiliar; }
            set
            {
                fotoFrenteCredencialAuxiliar = value;
                OnPropertyChanged("FotoFrenteCredencialAuxiliar");
            }
        }
        
        private byte[] fotoDetrasCredencial;
        public byte[] FotoDetrasCredencial
        {
            get { return fotoDetrasCredencial; }
            set
            {
                fotoDetrasCredencial = value;
                OnPropertyChanged("FotoDetrasCredencial");
            }
        }
        
        private byte[] fotoDetrasCredencialAuxiliar;
        public byte[] FotoDetrasCredencialAuxiliar
        {
            get { return fotoDetrasCredencialAuxiliar; }
            set
            {
                fotoDetrasCredencialAuxiliar = value;
                OnPropertyChanged("FotoDetrasCredencialAuxiliar");
            }
        }
        
        private byte[] fotoPersonaExterna;
        public byte[] FotoPersonaExterna
        {
            get { return fotoPersonaExterna; }
            set
            {
                fotoPersonaExterna = value;
                OnPropertyChanged("FotoPersonaExterna");
            }
        }

        private byte[] fotoPersonaExternaAuxiliar;
        public byte[] FotoPersonaExternaAuxiliar
        {
            get { return fotoPersonaExternaAuxiliar; }
            set
            {
                fotoPersonaExternaAuxiliar = value;
                OnPropertyChanged("FotoPersonaExternaAuxiliar");
            }
        }
        
        private byte[] imagenPersona = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get { return imagenPersona; }
            set { imagenPersona = value; OnPropertyChanged("ImagenPersona"); }
        }
        
        private byte[] imagenVisitanteExterno = new Imagenes().getImagenPerson();
        public byte[] ImagenVisitanteExterno
        {
            get { return imagenVisitanteExterno; }
            set { imagenVisitanteExterno = value; OnPropertyChanged("ImagenVisitanteExterno"); }
        }
        
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

        #region Auxiliares
        private DateTime FechaHoy = Fechas.GetFechaDateServer;

        private bool ValidacionesActivas = false;

        private string tituloRelacionInterno = "Buscar";
        public string TituloRelacionInterno
        {
            get { return tituloRelacionInterno; }
            set { tituloRelacionInterno = value; OnPropertyChanged("TituloRelacionInterno"); }
        }

        private bool NipExiste = false;
        
        private bool ExternoCambio = false;
        
        private bool ExistenteAuxiliar;
        
        private bool existente = false;
        public bool Existente
        {
            get { return existente; }
            set
            {
                existente = value;
                OnPropertyChanged("Existente");
            }
        }
        
        private bool existeNuevo = false;
        public bool ExisteNuevo
        {
            get { return existeNuevo; }
            set
            {
                existeNuevo = value;
            }
        }
        
        private WebCam CamaraWeb;       
        
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
        
        private bool nuevoExterno = false;
        public bool NuevoExterno
        {
            get { return nuevoExterno; }
            set { nuevoExterno = value; OnPropertyChanged("NuevoExterno"); }
        }

        private bool SeguirCargandoPersonas = false;
        
        private int Pagina = 1;
        #endregion

        IList<PlantillaBiometrico> HuellasCapturadas;

        private GafetesPVCView gafeteView;
        
        #region Gafete
        private ReportViewer reporteador;
        public ReportViewer Reporteador
        {
            get { return reporteador; }
            set { reporteador = value; OnPropertyChanged("Reporteador"); }
        }
        
        private string gafeteExterno = "GafeteExternoFrente";
        
        private bool gafeteDetras;
        public bool GafeteDetras
        {
            get { return gafeteDetras; }
            set
            {
                gafeteDetras = value;
                if (value && SelectPersona != null)
                {
                    gafeteExterno = "GafeteExternoDetras";

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
                    gafeteExterno = "GafeteExternoFrente";
                    //CrearGafete();
                }
                OnPropertyChanged("GafeteFrente");
            }
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

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set 
            {
                pImprimir = value;
                if (value)
                    MenuReporteEnabled = value;
            }
        }
        #endregion

        #region Menu
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
    }
}
