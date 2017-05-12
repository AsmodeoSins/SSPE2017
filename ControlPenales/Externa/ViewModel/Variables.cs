using Andora.UserControlLibrary;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
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
    partial class PrincipalVisitaExternaViewModel
    {

        #region Listas

        private ObservableCollection<VISITA_EXTERNA> listRegistros;
        public ObservableCollection<VISITA_EXTERNA> ListRegistros
        {
            get { return listRegistros; }
            set { listRegistros = value; OnPropertyChanged("ListRegistros"); }
        }
        private ObservableCollection<DEPARTAMENTO> listDepartamentos;
        public ObservableCollection<DEPARTAMENTO> ListDepartamentos
        {
            get { return listDepartamentos; }
            set { listDepartamentos = value; OnPropertyChanged("ListDepartamentos"); }
        }
        private ObservableCollection<SSP.Servidor.PERSONA> listPersonas;
        public ObservableCollection<SSP.Servidor.PERSONA> ListPersonas
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
        private ObservableCollection<AREA> listArea;
        public ObservableCollection<AREA> ListArea
        {
            get { return listArea; }
            set { listArea = value; OnPropertyChanged("ListArea"); }
        }
        private ObservableCollection<PUESTO> listPuestos;
        public ObservableCollection<PUESTO> ListPuestos
        {
            get { return listPuestos; }
            set { listPuestos = value; OnPropertyChanged("ListPuestos"); }
        }

        #endregion

        #region Text

        #region AccesoAduana
        private string textNIP;
        public string TextNIP
        {
            get { return textNIP; }
            set { textNIP = value; OnPropertyChanged("TextNIP"); }
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
        private string textHoraEntrada = Fechas.GetFechaDateServer.ToString("HH:mm");
        public string TextHoraEntrada
        {
            get { return textHoraEntrada; }
            set { textHoraEntrada = value; OnPropertyChanged("TextHoraEntrada"); }
        }
        private string textHoraSalida;
        public string TextHoraSalida
        {
            get { return textHoraSalida; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Length == 1)
                        textHoraSalida = value.Replace(":", string.Empty);
                    else if (value.Length == 2)
                    {
                        if (int.Parse("" + value[0] + "" + value[1] + "") >= 24)
                            textHoraSalida = "23";
                        else
                            textHoraSalida = value.Replace(":", string.Empty);
                    }
                    else if (value.Length == 3)
                    {
                        if (value.Contains(":"))
                        {
                            if (int.Parse("" + value[0] + "" + value[1] + "") >= 24)
                                textHoraSalida = "23:";
                            else
                                textHoraSalida = "" + value[0] + value[1] + ":";
                        }
                        else
                        {
                            if (int.Parse("" + value[0] + "" + value[1] + "") >= 24)
                                textHoraSalida = "23:" + value[2];
                            else
                                textHoraSalida = "" + value[0] + value[1] + ":" + value[2];
                        }
                    }
                    else if (value.Length == 4)
                        textHoraSalida = "" + value[0] + value[1] + ":" + value[3];
                    else if (value.Length == 5)
                    {
                        if (int.Parse("" + value[3] + "" + value[4] + "") >= 60)
                            textHoraSalida = "" + value[0] + value[1] + ":" + "59";
                        else
                            textHoraSalida = "" + value[0] + value[1] + ":" + value[3] + value[4];
                    }
                    else
                        textHoraSalida = value;
                }
                OnPropertyChanged("TextHoraSalida");
            }
        }
        private string textInstitucion;
        public string TextInstitucion
        {
            get { return textInstitucion; }
            set { textInstitucion = value; OnPropertyChanged("TextInstitucion"); }
        }
        private string textAsunto;
        public string TextAsunto
        {
            get { return textAsunto; }
            set { textAsunto = value; OnPropertyChanged("TextAsunto"); }
        }
        private string textObservaciones;
        public string TextObservaciones
        {
            get { return textObservaciones; }
            set { textObservaciones = value; OnPropertyChanged("TextObservaciones"); }
        }
        private string textDepartamento;
        public string TextDepartamento
        {
            get { return textDepartamento; }
            set { textDepartamento = value; OnPropertyChanged("TextDepartamento"); }
        }
        #endregion

        #region PadronExterna
        private string textPaternoNuevo;
        public string TextPaternoNuevo
        {
            get { return textPaternoNuevo; }
            set
            {
                textPaternoNuevo = value;
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
        private string textNIPNuevo;
        public string TextNIPNuevo
        {
            get { return textNIPNuevo; }
            set { textNIPNuevo = value; OnPropertyChanged("TextNIPNuevo"); }
        }
        private string textCalle;
        public string TextCalle
        {
            get { return textCalle; }
            set { textCalle = value; OnPropertyValidateChanged("TextCalle"); }
        }
        private string textNumInt;
        public string TextNumInt
        {
            get { return textNumInt; }
            set { textNumInt = value; OnPropertyValidateChanged("TextNumInt"); }
        }
        private int? textNumExt;
        public int? TextNumExt
        {
            get { return textNumExt; }
            set { textNumExt = value; OnPropertyValidateChanged("TextNumExt"); }
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
        
        private DateTime? fechaRegistro = Fechas.GetFechaDateServer;
        public DateTime? FechaRegistro
        {
            get { return fechaRegistro; }
            set
            {
                fechaRegistro = value;
                SetValidacionesVisitaExterna();
                OnPropertyChanged("FechaRegistro");
            }
        }
        private DateTime? fechaNacimientoNuevo;
        public DateTime? FechaNacimientoNuevo
        {
            get { return fechaNacimientoNuevo; }
            set
            {
                fechaNacimientoNuevo = value;
                SetValidacionesAgregarNuevaVisitaExterna();
                OnPropertyValidateChanged("FechaNacimientoNuevo");
            }
        }
        private DateTime? fechaAltaNuevo;
        public DateTime? FechaAltaNuevo
        {
            get { return fechaAltaNuevo; }
            set { fechaAltaNuevo = value; OnPropertyChanged("FechaAltaNuevo"); }
        }
        #endregion

        #endregion

        #region Select

        #region AccesoAduana
        private short? selectDepartamento;
        public short? SelectDepartamento
        {
            get { return selectDepartamento; }
            set { selectDepartamento = value; OnPropertyChanged("SelectDepartamento"); }
        }
        private CENTRO_DEPARTAMENTO selectDepartamentoItem;
        public CENTRO_DEPARTAMENTO SelectDepartamentoItem
        {
            get { return selectDepartamentoItem; }
            set { selectDepartamentoItem = value; OnPropertyChanged("SelectDepartamentoItem"); }
        }
        private string selectDiscapacitado;
        public string SelectDiscapacitado
        {
            get { return selectDiscapacitado; }
            set
            {
                selectDiscapacitado = value;
                DiscapacidadEnabled = value == "S";
                OnPropertyChanged("SelectDiscapacitado");
            }
        }
        private short? selectDiscapacidad;
        public short? SelectDiscapacidad
        {
            get { return selectDiscapacidad; }
            set { selectDiscapacidad = value; OnPropertyChanged("SelectDiscapacidad"); }
        }
        private short? selectTipoVisitante = -1;
        public short? SelectTipoVisitante
        {
            get { return selectTipoVisitante; }
            set { selectTipoVisitante = value; OnPropertyChanged("SelectTipoVisitante"); }
        }
        private short? selectArea = -1;
        public short? SelectArea
        {
            get { return selectArea; }
            set { selectArea = value; OnPropertyChanged("SelectArea"); }
        }
        private short? selectPuesto = -1;
        public short? SelectPuesto
        {
            get { return selectPuesto; }
            set { selectPuesto = value; OnPropertyChanged("SelectPuesto"); }
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
                OnPropertyChanged("SelectPersona");
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

        #region PadronExterna
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
                DiscapacitadoNuevoEnabled = value == 23;
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
                OnPropertyChanged("SelectPais");
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
                OnPropertyChanged("SelectEntidad");
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
                OnPropertyChanged("SelectMunicipio");
            }
        }
        private int? selectColonia;
        public int? SelectColonia
        {
            get { return selectColonia; }
            set
            {
                selectColonia = value;
                OnPropertyChanged("SelectColonia");
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
                OnPropertyChanged("SelectDiscapacitadoNuevo");
            }
        }
        private short? selectDiscapacidadNuevo;
        public short? SelectDiscapacidadNuevo
        {
            get { return selectDiscapacidadNuevo; }
            set
            {
                selectDiscapacidadNuevo = value;
                OnPropertyChanged("SelectDiscapacidadNuevo");
            }
        }
        private string selectFrenteDetrasFoto;
        public string SelectFrenteDetrasFoto
        {
            get { return selectFrenteDetrasFoto; }
            set
            {
                selectFrenteDetrasFoto = value;
                OnPropertyChanged("SelectFrenteDetrasFoto");
            }
        }
        #endregion

        #endregion

        #region Visible
        private bool emptyBuscarRelacionInternoVisible;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return emptyBuscarRelacionInternoVisible; }
            set { emptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }
        private Visibility comboFrontBackFotoVisible = Visibility.Visible;
        public Visibility ComboFrontBackFotoVisible
        {
            get { return comboFrontBackFotoVisible; }
            set { comboFrontBackFotoVisible = value; OnPropertyChanged("ComboFrontBackFotoVisible"); }
        }
        private Visibility lineasGuiaFoto = Visibility.Visible;
        public Visibility LineasGuiaFoto
        {
            get { return lineasGuiaFoto; }
            set { lineasGuiaFoto = value; OnPropertyChanged("LineasGuiaFoto"); }
        }
        #endregion

        #region Enabled
        private bool _NIPEnabled = true;
        public bool NIPEnabled
        {
            get { return _NIPEnabled; }
            set { _NIPEnabled = value; OnPropertyChanged("NIPEnabled"); }
        }
        private bool discapacitadoEnabled = true;
        public bool DiscapacitadoEnabled
        {
            get { return discapacitadoEnabled; }
            set { discapacitadoEnabled = value; OnPropertyChanged("DiscapacitadoEnabled"); }
        }
        private bool discapacidadEnabled;
        public bool DiscapacidadEnabled
        {
            get { return discapacidadEnabled; }
            set { discapacidadEnabled = value; OnPropertyChanged("DiscapacidadEnabled"); }
        }
        private bool discapacitadoNuevoEnabled = false;
        public bool DiscapacitadoNuevoEnabled
        {
            get { return discapacitadoNuevoEnabled; }
            set { discapacitadoNuevoEnabled = value; OnPropertyChanged("DiscapacitadoNuevoEnabled"); }
        }
        private bool discapacidadNuevoEnabled;
        public bool DiscapacidadNuevoEnabled
        {
            get { return discapacidadNuevoEnabled; }
            set { discapacidadNuevoEnabled = value; OnPropertyChanged("DiscapacidadNuevoEnabled"); }
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
        #endregion

        #region Imagenes
        private byte[] imagenPersona = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get { return imagenPersona; }
            set { imagenPersona = value; OnPropertyChanged("ImagenPersona"); }
        }
        private byte[] imagenVisitaExterna = new Imagenes().getImagenPerson();
        public byte[] ImagenVisitaExterna
        {
            get { return imagenVisitaExterna; }
            set { imagenVisitaExterna = value; OnPropertyChanged("ImagenVisitaExterna"); }
        }
        private byte[] imagenVisitanteExterno = new Imagenes().getImagenPerson();
        public byte[] ImagenVisitanteExterno
        {
            get { return imagenVisitanteExterno; }
            set { imagenVisitanteExterno = value; OnPropertyChanged("ImagenVisitanteExterno"); }
        }
        #endregion

        #region [Huellas]
        IList<PlantillaBiometrico> HuellasCapturadas;

        private enumTipoBiometrico? _DD_Dedo;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set { _DD_Dedo = value; }
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

        private Brush _PulgarDerecho;
        public Brush PulgarDerecho
        {
            get { return _PulgarDerecho; }
            set
            {
                _PulgarDerecho = value;
                RaisePropertyChanged("PulgarDerecho");
            }
        }

        private ImageSource _PulgarDerechoBMP;
        public ImageSource PulgarDerechoBMP
        {
            get { return _PulgarDerechoBMP; }
            set
            {
                _PulgarDerechoBMP = value;
                RaisePropertyChanged("PulgarDerechoBMP");
            }
        }

        private Brush _IndiceDerecho;
        public Brush IndiceDerecho
        {
            get { return _IndiceDerecho; }
            set
            {
                _IndiceDerecho = value;
                OnPropertyChanged("IndiceDerecho");
            }
        }

        private Brush _MedioDerecho;
        public Brush MedioDerecho
        {
            get { return _MedioDerecho; }
            set
            {
                _MedioDerecho = value;
                OnPropertyChanged("MedioDerecho");
            }
        }

        private Brush _AnularDerecho;
        public Brush AnularDerecho
        {
            get { return _AnularDerecho; }
            set
            {
                _AnularDerecho = value;
                OnPropertyChanged("AnularDerecho");
            }
        }

        private Brush _MeñiqueDerecho;
        public Brush MeñiqueDerecho
        {
            get { return _MeñiqueDerecho; }
            set
            {
                _MeñiqueDerecho = value;
                OnPropertyChanged("MeñiqueDerecho");
            }
        }

        private Brush _PulgarIzquierdo;
        public Brush PulgarIzquierdo
        {
            get { return _PulgarIzquierdo; }
            set
            {
                _PulgarIzquierdo = value;
                OnPropertyChanged("PulgarIzquierdo");
            }
        }

        private Brush _IndiceIzquierdo;
        public Brush IndiceIzquierdo
        {
            get { return _IndiceIzquierdo; }
            set
            {
                _IndiceIzquierdo = value;
                OnPropertyChanged("IndiceIzquierdo");
            }
        }

        private Brush _MedioIzquierdo;
        public Brush MedioIzquierdo
        {
            get { return _MedioIzquierdo; }
            set
            {
                _MedioIzquierdo = value;
                OnPropertyChanged("MedioIzquierdo");
            }
        }

        private Brush _AnularIzquierdo;
        public Brush AnularIzquierdo
        {
            get { return _AnularIzquierdo; }
            set
            {
                _AnularIzquierdo = value;
                OnPropertyChanged("AnularIzquierdo");
            }
        }

        private Brush _MeñiqueIzquierdo;
        public Brush MeñiqueIzquierdo
        {
            get { return _MeñiqueIzquierdo; }
            set
            {
                _MeñiqueIzquierdo = value;
                OnPropertyChanged("MeñiqueIzquierdo");
            }
        }
        #endregion

        #region [Fotos]
        private WebCam CamaraWeb;
        private List<ImageSourceToSave> _ImagesToSave;
        public List<ImageSourceToSave> ImagesToSave
        {
            get { return _ImagesToSave; }
            set { _ImagesToSave = value; }
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
        #endregion

        #region Otros
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
        private bool ExistenteAuxiliar;
        private bool existeNuevo = false;
        public bool ExisteNuevo
        {
            get { return existeNuevo; }
            set
            {
                existeNuevo = value;
            }
        }
        #endregion


    }

    public class RegistroEntrega
    {
        public CORRESPONDENCIA Correspondencia { get; set; }

        public short Anio { get; set; }
        public int Folio { get; set; }
        public string Interno { get; set; }
        public string Fecha_Deposito { get; set; }
        public string Hora_Deposito { get; set; }
        public string Remitente { get; set; }
        public bool Entrega { get; set; }
        public string Observaciones { get; set; }
    }
}
