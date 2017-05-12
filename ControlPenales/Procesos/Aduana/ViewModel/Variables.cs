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
    partial class RecepcionAduanaViewModel
    {
        #region DatosGenerales

        #region Text
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
            set
            {
                textCodigo = value;
                OnPropertyChanged("TextCodigo");
            }
        }
        private short? textEdad;
        public short? TextEdad
        {
            get { return textEdad; }
            set
            {
                textEdad = value;
                OnPropertyChanged("TextEdad");
            }
        }
        private string textCurp;
        public string TextCurp
        {
            get { return textCurp; }
            set
            {
                textCurp = value;
                OnPropertyChanged("TextCurp");
            }
        }
        private string textRfc;
        public string TextRfc
        {
            get { return textRfc; }
            set
            {
                textRfc = value;
                OnPropertyChanged("TextRfc");
            }
        }
        private int? textNip;
        public int? TextNip
        {
            get { return textNip; }
            set
            {
                textNip = value;
                OnPropertyChanged("TextNip");
            }
        }
        private string textTelefono;
        public string TextTelefono
        {
            get
            {
                if (textTelefono == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(textTelefono);
            }
            set
            {
                textTelefono = value;
                OnPropertyChanged("TextTelefono");
            }
        }
        private string textCorreo;
        public string TextCorreo
        {
            get { return textCorreo; }
            set
            {
                textCorreo = value;
                OnPropertyChanged("TextCorreo");
            }
        }
        private string textCalle;
        public string TextCalle
        {
            get { return textCalle; }
            set { textCalle = value; OnPropertyChanged("TextCalle"); }
        }
        private int? textNumExt;
        public int? TextNumExt
        {
            get { return textNumExt; }
            set { textNumExt = value; OnPropertyChanged("TextNumExt"); }
        }
        private string textNumInt;
        public string TextNumInt
        {
            get { return textNumInt; }
            set { textNumInt = value; OnPropertyChanged("TextNumInt"); }
        }
        private int? textCodigoPostal;
        public int? TextCodigoPostal
        {
            get { return textCodigoPostal; }
            set { textCodigoPostal = value; OnPropertyChanged("TextCodigoPostal"); }
        }
        private string textFechaUltimaModificacion = DateTime.Now.ToString("dd/MM/yyyy");
        public string TextFechaUltimaModificacion
        {
            get { return textFechaUltimaModificacion; }
            set
            {
                textFechaUltimaModificacion = value;
                OnPropertyChanged("TextFechaUltimaModificacion");
            }
        }
        #endregion

        #region Select
        private string selectSexo;
        public string SelectSexo
        {
            get { return selectSexo; }
            set
            {
                selectSexo = value;
                OnPropertyChanged("SelectSexo");
            }
        }
        private short selectSituacion;
        public short SelectSituacion
        {
            get { return selectSituacion; }
            set
            {
                selectSituacion = value;
                OnPropertyChanged("SelectSituacion");
            }
        }
        private DateTime fechaNacimiento = DateTime.Now;
        public DateTime FechaNacimiento
        {
            get { return fechaNacimiento; }
            set
            {
                fechaNacimiento = value;
                TextEdad = value != null ? new Fechas().CalculaEdad(value) : new Nullable<short>();
                OnPropertyChanged("FechaNacimiento");
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
                {
                    //ListEntidad = ListEntidad ?? new ObservableCollection<ENTIDAD>();
                }
                //ListEntidad = new ObservableCollection<ENTIDAD>(ListEntidadesAuxiliares.Where(w=>w.ID_PAIS_NAC == value).OrderBy(o => o.DESCR)); //(new cEntidad()).ObtenerTodos();
                else
                { }// ListEntidad = new ObservableCollection<ENTIDAD>();
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
                {
                    // ListMunicipio = ListMunicipio ?? new ObservableCollection<MUNICIPIO>();
                }
                //ListMunicipio = new ObservableCollection<MUNICIPIO>(ListMunicipiosAuxiliares.Where(w=>w.ID_ENTIDAD == value).OrderBy(o => o.MUNICIPIO1));//(new cMunicipio()).ObtenerTodos(string.Empty, value));
                else
                { }// ListMunicipio = new ObservableCollection<MUNICIPIO>();
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
                    //ListColonia = ListColonia ?? new ObservableCollection<COLONIA>();
                    //ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w=> w.ID_ENTIDAD == SelectEntidad && w.ID_MUNICIPIO == value).OrderBy(o => o.DESCR));//(new cColonia()).ObtenerTodos(string.Empty, value, SelectEntidad));
                }
                else
                { }// ListColonia = new ObservableCollection<COLONIA>();
                if (ListColonia != null ? ListColonia.Count == 1 : false)
                {
                    //ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w=> w.ID_MUNICIPIO == 1001).OrderBy(o => o.DESCR));//(new cColonia()).ObtenerTodos(string.Empty, 1001).OrderBy(o => o.DESCR));
                    //ListColonia = new ObservableCollection<COLONIA>();//
                    //ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
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
                if (value == null)
                {

                }
                OnPropertyChanged("SelectColonia");
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
                //if (!string.IsNullOrEmpty(value))
                //    SetValidaciones();
                OnPropertyChanged("SelectDiscapacitado");
            }
        }
        private short? selectTipoDiscapacidad;
        public short? SelectTipoDiscapacidad
        {
            get { return selectTipoDiscapacidad; }
            set { selectTipoDiscapacidad = value; OnPropertyChanged("SelectTipoDiscapacidad"); }
        }
        private SSP.Servidor.PERSONA SelectPersonaAuxiliar;
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
        #endregion

        #region List
        private List<string> ListLetras;
        private List<TIPO_PERSONA> ListTipoPersona = new List<TIPO_PERSONA>();
        private ObservableCollection<TIPO_DISCAPACIDAD> _ListDiscapacidades;
        public ObservableCollection<TIPO_DISCAPACIDAD> ListDiscapacidades
        {
            get { return _ListDiscapacidades; }
            set { _ListDiscapacidades = value; OnPropertyChanged("ListDiscapacidades"); }
        }
        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> listPersonas;
        public RangeEnabledObservableCollection<SSP.Servidor.PERSONA> ListPersonas
        {
            get { return listPersonas; }
            set { listPersonas = value; OnPropertyChanged("ListPersonas"); }
        }
        private ObservableCollection<ESTATUS_VISITA> listSituacion;
        public ObservableCollection<ESTATUS_VISITA> ListSituacion
        {
            get { return listSituacion; }
            set { listSituacion = value; OnPropertyChanged("ListSituacion"); }
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
        #endregion

        #region Enableds
        private bool situacionEnabled = false;
        public bool SituacionEnabled
        {
            get { return situacionEnabled; }
            set { situacionEnabled = value; OnPropertyChanged("SituacionEnabled"); }
        }
        private bool pertenenciasEnabled = true;
        public bool PertenenciasEnabled
        {
            get { return pertenenciasEnabled; }
            set { pertenenciasEnabled = value; OnPropertyChanged("PertenenciasEnabled"); }
        }
        private bool entradaEnabled = false;
        public bool EntradaEnabled
        {
            get { return entradaEnabled; }
            set { entradaEnabled = value; OnPropertyChanged("EntradaEnabled"); }
        }
        private bool salidaEnabled = false;
        public bool SalidaEnabled
        {
            get { return salidaEnabled; }
            set { salidaEnabled = value; OnPropertyChanged("SalidaEnabled"); }
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
        private bool nombreReadOnly = true;
        public bool NombreReadOnly
        {
            get { return nombreReadOnly; }
            set { nombreReadOnly = value; OnPropertyChanged("NombreReadOnly"); }
        }
        private bool codigoEnabled = false;
        public bool CodigoEnabled
        {
            get { return codigoEnabled; }
            set { codigoEnabled = value; OnPropertyChanged("CodigoEnabled"); }
        }
        private bool generalEnabled = false;
        public bool GeneralEnabled
        {
            get { return generalEnabled; }
            set { generalEnabled = value; OnPropertyChanged("GeneralEnabled"); }
        }
        private bool BanderaHuella;
        private BusquedaHuella _WindowBusqueda;
        public BusquedaHuella WindowBusqueda
        {
            get { return _WindowBusqueda; }
            set { _WindowBusqueda = value; }
        }
        #endregion

        #region Otros
        private bool SeguirCargandoPersonas { get; set; }
        private bool _EmptyBuscarRelacionInternoVisible;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return _EmptyBuscarRelacionInternoVisible; }
            set { _EmptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }
        private byte[] _FotoVisita = new Imagenes().getImagenPerson();
        public byte[] FotoVisita
        {
            get { return _FotoVisita; }
            set { _FotoVisita = value; OnPropertyChanged("FotoVisita"); }
        }
        private byte[] imagenPersona = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get { return imagenPersona; }
            set { imagenPersona = value; OnPropertyChanged("ImagenPersona"); }
        }
        #endregion

        #endregion

        #region Abogados

        #region Text
        private string _TextFolioImputado;
        public string TextFolioImputado
        {
            get { return _TextFolioImputado; }
            set
            {
                _TextFolioImputado = value;
                FiltrarImputados();
                OnPropertyChanged("TextFolioImputado");
            }
        }
        private string _TextAnioImputado;
        public string TextAnioImputado
        {
            get { return _TextAnioImputado; }
            set
            {
                _TextAnioImputado = value;
                FiltrarImputados();
                OnPropertyChanged("TextAnioImputado");
            }
        }
        private string _TextPaternoImputado;
        public string TextPaternoImputado
        {
            get { return _TextPaternoImputado; }
            set
            {
                _TextPaternoImputado = value;
                FiltrarImputados();
                OnPropertyChanged("TextPaternoImputado");
            }
        }
        private string _TextMaternoImputado;
        public string TextMaternoImputado
        {
            get { return _TextMaternoImputado; }
            set
            {
                _TextMaternoImputado = value;
                FiltrarImputados();
                OnPropertyChanged("TextMaternoImputado");
            }
        }
        private string _TextNombreImputado;
        public string TextNombreImputado
        {
            get { return _TextNombreImputado; }
            set
            {
                _TextNombreImputado = value;
                FiltrarImputados();
                OnPropertyChanged("TextNombreImputado");
            }
        }
        private string _HeaderTextDatosAbogados;
        public string HeaderTextDatosAbogados
        {
            get { return _HeaderTextDatosAbogados; }
            set { _HeaderTextDatosAbogados = value; OnPropertyChanged("HeaderTextDatosAbogados"); }
        }
        private string _TextCedulaCJF;
        public string TextCedulaCJF
        {
            get { return _TextCedulaCJF; }
            set { _TextCedulaCJF = value; OnPropertyChanged("TextCedulaCJF"); }
        }
        private string _CedulaCJF;
        public string CedulaCJF
        {
            get { return _CedulaCJF; }
            set { _CedulaCJF = value; OnPropertyChanged("CedulaCJF"); }
        }
        private string _TextObservacionesAbogado;
        public string TextObservacionesAbogado
        {
            get { return _TextObservacionesAbogado; }
            set { _TextObservacionesAbogado = value; OnPropertyChanged("TextObservacionesAbogado"); }
        }
        private string _TextJuzgadoActuario;
        public string TextJuzgadoActuario
        {
            get { return _TextJuzgadoActuario; }
            set { _TextJuzgadoActuario = value; OnPropertyChanged("TextJuzgadoActuario"); }
        }
        private string _TextObservacionesColaborador;
        public string TextObservacionesColaborador
        {
            get { return _TextObservacionesColaborador; }
            set { _TextObservacionesColaborador = value; OnPropertyChanged("TextObservacionesColaborador"); }
        }
        private string _TextNumeroAbogadoTitular;
        public string TextNumeroAbogadoTitular
        {
            get { return _TextNumeroAbogadoTitular; }
            set { _TextNumeroAbogadoTitular = value; OnPropertyChanged("TextNumeroAbogadoTitular"); }
        }
        private string _TextNombreAbogadoTitular;
        public string TextNombreAbogadoTitular
        {
            get { return _TextNombreAbogadoTitular; }
            set { _TextNombreAbogadoTitular = value; OnPropertyChanged("TextNombreAbogadoTitular"); }
        }
        #endregion

        #region Select
        private AbogadoCausaPenalAsignacion _SelectCausaAsignada;
        public AbogadoCausaPenalAsignacion SelectCausaAsignada
        {
            get { return _SelectCausaAsignada; }
            set
            {
                _SelectCausaAsignada = value;
                OnPropertyChanged("SelectCausaAsignada");
            }
        }
        private AbogadoIngresoAsignacion _SelectAbogadoIngreso;
        public AbogadoIngresoAsignacion SelectAbogadoIngreso
        {
            get { return _SelectAbogadoIngreso; }
            set
            {
                _SelectAbogadoIngreso = value;
                if (value != null)
                {
                    ImagenInterno = value.ABOGADO_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO &&
                        w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ? value.ABOGADO_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w =>
                            w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO
                        : new Imagenes().getImagenPerson();
                    if (value != null)
                    {
                        ImagenInterno = value.ABOGADO_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO &&
                            w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ? value.ABOGADO_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w =>
                                w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO
                            : new Imagenes().getImagenPerson();
                        ListCausasAsignadas = new ObservableCollection<AbogadoCausaPenalAsignacion>(value.ABOGADO_INGRESO.ABOGADO_CAUSA_PENAL.Select(s =>
                            new AbogadoCausaPenalAsignacion
                            {
                                ABOGADO_CAUSA_PENAL = s,
                                DESCR = s.ESTATUS_VISITA.DESCR,
                                ID_ESTATUS_VISITA = s.ID_ESTATUS_VISITA,
                                ELEGIDO = ListCausasSeleccionadas.Count > 0 ? ListCausasSeleccionadas.Where(w => w.ID_CENTRO == s.CAUSA_PENAL.ID_CENTRO && w.ID_ANIO == s.CAUSA_PENAL.ID_ANIO &&
                                    w.ID_IMPUTADO == s.CAUSA_PENAL.ID_IMPUTADO && w.ID_INGRESO == s.CAUSA_PENAL.ID_INGRESO && w.ID_CAUSA_PENAL == s.CAUSA_PENAL.ID_CAUSA_PENAL).Any() : false,
                                DESHABILITA = s.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_AUTORIZADO
                            }));
                    }
                }
                OnPropertyChanged("SelectAbogadoIngreso");
            }
        }
        private short? _SelectEstatusColaborador;
        public short? SelectEstatusColaborador
        {
            get { return _SelectEstatusColaborador; }
            set { _SelectEstatusColaborador = value; OnPropertyChanged("SelectEstatusColaborador"); }
        }
        private short? _SelectEstatusAbogado;
        public short? SelectEstatusAbogado
        {
            get { return _SelectEstatusAbogado; }
            set { _SelectEstatusAbogado = value; OnPropertyChanged("SelectEstatusAbogado"); }
        }
        #endregion

        #region Visibles
        private Visibility _InternosAsignadosAbogadosVisible = Visibility.Collapsed;
        public Visibility InternosAsignadosAbogadosVisible
        {
            get { return _InternosAsignadosAbogadosVisible; }
            set { _InternosAsignadosAbogadosVisible = value; OnPropertyChanged("InternosAsignadosAbogadosVisible"); }
        }
        private Visibility _DatosAbogadosVisible = Visibility.Collapsed;
        public Visibility DatosAbogadosVisible
        {
            get { return _DatosAbogadosVisible; }
            set { _DatosAbogadosVisible = value; OnPropertyChanged("DatosAbogadosVisible"); }
        }
        private Visibility _ColaboradorVisible = Visibility.Collapsed;
        public Visibility ColaboradorVisible
        {
            get { return _ColaboradorVisible; }
            set { _ColaboradorVisible = value; OnPropertyChanged("ColaboradorVisible"); }
        }
        private Visibility _JuzgadoActuarioVisible = Visibility.Collapsed;
        public Visibility JuzgadoActuarioVisible
        {
            get { return _JuzgadoActuarioVisible; }
            set { _JuzgadoActuarioVisible = value; OnPropertyChanged("JuzgadoActuarioVisible"); }
        }
        private Visibility _GridAbogadoVisible = Visibility.Collapsed;
        public Visibility GridAbogadoVisible
        {
            get { return _GridAbogadoVisible; }
            set { _GridAbogadoVisible = value; OnPropertyChanged("GridAbogadoVisible"); }
        }
        #endregion

        #region Listas
        private ObservableCollection<CAUSA_PENAL> ListCausasSeleccionadas;
        private ObservableCollection<AbogadoCausaPenalAsignacion> _ListCausasAsignadas;
        public ObservableCollection<AbogadoCausaPenalAsignacion> ListCausasAsignadas
        {
            get { return _ListCausasAsignadas; }
            set { _ListCausasAsignadas = value; OnPropertyChanged("ListCausasAsignadas"); }
        }
        private ObservableCollection<AbogadoIngresoAsignacion> _ListIngresosAsignados;
        public ObservableCollection<AbogadoIngresoAsignacion> ListIngresosAsignados
        {
            get { return _ListIngresosAsignados; }
            set
            {
                //value.FirstOrDefault().ABOGADO_INGRESO.
                _ListIngresosAsignados = value; OnPropertyChanged("ListIngresosAsignados");
            }
        }
        private ObservableCollection<ESTATUS_VISITA> _ListEstatusVisita;
        public ObservableCollection<ESTATUS_VISITA> ListEstatusVisita
        {
            get { return _ListEstatusVisita; }
            set { _ListEstatusVisita = value; OnPropertyChanged("ListEstatusVisita"); }
        }
        #endregion

        #region Otros
        private bool _CredencializadoColaborador;
        public bool CredencializadoColaborador
        {
            get { return _CredencializadoColaborador; }
            set { _CredencializadoColaborador = value; OnPropertyChanged("CredencializadoColaborador"); }
        }
        private bool _CredencializadoAbogado;
        public bool CredencializadoAbogado
        {
            get { return _CredencializadoAbogado; }
            set { _CredencializadoAbogado = value; OnPropertyChanged("CredencializadoAbogado"); }
        }
        private byte[] _ImagenInternoFamiliar;
        public byte[] ImagenInternoFamiliar
        {
            get { return _ImagenInternoFamiliar; }
            set { _ImagenInternoFamiliar = value; OnPropertyChanged("ImagenInternoFamiliar"); }
        }
        private byte[] _ImagenInterno;
        public byte[] ImagenInterno
        {
            get { return _ImagenInterno; }
            set { _ImagenInterno = value; OnPropertyChanged("ImagenInterno"); }
        }
        private ObservableCollection<ActuarioIngresoAsignacion> ListInternosSeleccionadosPorNotificarAuxiliar;
        private ObservableCollection<ActuarioIngresoAsignacion> _ListInternosSeleccionadosPorNotificar;
        public ObservableCollection<ActuarioIngresoAsignacion> ListInternosSeleccionadosPorNotificar
        {
            get { return _ListInternosSeleccionadosPorNotificar; }
            set { _ListInternosSeleccionadosPorNotificar = value; OnPropertyChanged("ListInternosSeleccionadosPorNotificar"); }
        }
        private void FiltrarImputados()
        {
            if (ListInternosSeleccionadosPorNotificarAuxiliar != null)
                ListInternosSeleccionadosPorNotificar = new ObservableCollection<ActuarioIngresoAsignacion>(ListInternosSeleccionadosPorNotificarAuxiliar.Where(w =>
                    (string.IsNullOrEmpty(TextAnioImputado) ? true : w.ACTUARIO_INGRESO.INGRESO.ID_ANIO.ToString().Contains(TextAnioImputado)) &&
                    (string.IsNullOrEmpty(TextFolioImputado) ? true : w.ACTUARIO_INGRESO.INGRESO.ID_IMPUTADO.ToString().Contains(TextFolioImputado)) &&
                    (string.IsNullOrEmpty(TextPaternoImputado) ? true : w.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Contains(TextPaternoImputado)) &&
                    (string.IsNullOrEmpty(TextMaternoImputado) ? true : w.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Contains(TextMaternoImputado)) &&
                    (string.IsNullOrEmpty(TextNombreImputado) ? true : w.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Contains(TextNombreImputado))));
        }
        #endregion

        #endregion

        #region Visita Familiar
        private short SelectAreaDestinoAuxiliar;
        private AREA _SelectAreaDestino;
        public AREA SelectAreaDestino
        {
            get { return _SelectAreaDestino; }
            set { _SelectAreaDestino = value; OnPropertyChanged("SelectAreaDestino"); }
        }
        private ObservableCollection<AREA> _ListAreasDestinos;
        public ObservableCollection<AREA> ListAreasDestinos
        {
            get { return _ListAreasDestinos; }
            set { _ListAreasDestinos = value; OnPropertyChanged("ListAreasDestinos"); }
        }
        private bool IsVisitaIntima = false;
        private Visibility _VisitaFamiliarVisible = Visibility.Collapsed;
        public Visibility VisitaFamiliarVisible
        {
            get { return _VisitaFamiliarVisible; }
            set { _VisitaFamiliarVisible = value; OnPropertyChanged("VisitaFamiliarVisible"); }
        }
        private VisitanteIngresoAsignacion _SelectInternoFamiliar;
        public VisitanteIngresoAsignacion SelectInternoFamiliar
        {
            get { return _SelectInternoFamiliar; }
            set
            {
                _SelectInternoFamiliar = value;
                ImagenInternoFamiliar = value == null ? new Imagenes().getImagenPerson() :
                    value.VISITANTE_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                        value.VISITANTE_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO :
                        value.VISITANTE_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                            value.VISITANTE_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO :
                            new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectInternoFamiliar");
            }
        }
        private ObservableCollection<VisitanteIngresoAsignacion> _ListadoInternos;
        public ObservableCollection<VisitanteIngresoAsignacion> ListadoInternos
        {
            get { return _ListadoInternos; }
            set { _ListadoInternos = value; OnPropertyChanged("ListadoInternos"); }
        }
        private Visibility _AcompanantesVisibles = Visibility.Visible;
        public Visibility AcompanantesVisibles
        {
            get { return _AcompanantesVisibles; }
            set { _AcompanantesVisibles = value; OnPropertyChanged("AcompanantesVisibles"); }
        }
        private bool _IsDetalleInternosEnable;
        public bool IsDetalleInternosEnable
        {
            get { return _IsDetalleInternosEnable; }
            set { _IsDetalleInternosEnable = value; OnPropertyChanged("IsDetalleInternosEnable"); }
        }
        private ObservableCollection<TIPO_REFERENCIA> listTipoRelacion;
        public ObservableCollection<TIPO_REFERENCIA> ListTipoRelacion
        {
            get { return listTipoRelacion; }
            set { listTipoRelacion = value; OnPropertyChanged("ListTipoRelacion"); }
        }
        private bool _SelectParentescoIngresoEnabled;
        public bool SelectParentescoIngresoEnabled
        {
            get { return _SelectParentescoIngresoEnabled; }
            set { _SelectParentescoIngresoEnabled = value; }
        }
        private TIPO_REFERENCIA _SelectParentesco;
        public TIPO_REFERENCIA SelectParentesco
        {
            get { return _SelectParentesco; }
            set { _SelectParentesco = value; OnPropertyChanged("SelectParentesco"); }
        }
        private short _SelectEstatusRelacion;
        public short SelectEstatusRelacion
        {
            get { return _SelectEstatusRelacion; }
            set { _SelectEstatusRelacion = value; OnPropertyChanged("SelectEstatusRelacion"); }
        }
        private bool _SelectEstatusRelacionEnabled;
        public bool SelectEstatusRelacionEnabled
        {
            get { return _SelectEstatusRelacionEnabled; }
            set { _SelectEstatusRelacionEnabled = value; OnPropertyChanged("SelectEstatusRelacionEnabled"); }
        }
        private string _TextObservacionVisitanteIngreso;
        public string TextObservacionVisitanteIngreso
        {
            get { return _TextObservacionVisitanteIngreso; }
            set { _TextObservacionVisitanteIngreso = value; OnPropertyChanged("TextObservacionVisitanteIngreso"); }
        }
        private Visibility _InternosAsignadosVisible = Visibility.Collapsed;
        public Visibility InternosAsignadosVisible
        {
            get { return _InternosAsignadosVisible; }
            set { _InternosAsignadosVisible = value; OnPropertyChanged("InternosAsignadosVisible"); }
        }
        private Visibility _AcompananteVisible = Visibility.Collapsed;
        public Visibility AcompananteVisible
        {
            get { return _AcompananteVisible; }
            set { _AcompananteVisible = value; OnPropertyChanged("AcompananteVisible"); }
        }
        private ObservableCollection<AcompananteAsignacion> ListAcompanantesAuxiliar;
        private ObservableCollection<AcompananteAsignacion> _ListAcompanantes;
        public ObservableCollection<AcompananteAsignacion> ListAcompanantes
        {
            get { return _ListAcompanantes; }
            set { _ListAcompanantes = value; OnPropertyChanged("ListAcompanantes"); }
        }
        private AcompananteAsignacion _SelectAcompanante;
        public AcompananteAsignacion SelectAcompanante
        {
            get { return _SelectAcompanante; }
            set
            {
                _SelectAcompanante = value;
                ImagenAcompanante = value == null ? new Imagenes().getImagenPerson() :
                    value.ACOMPANANTE.VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                        value.ACOMPANANTE.VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO :
                        value.ACOMPANANTE.VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                            value.ACOMPANANTE.VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO :
                            new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectAcompanante");
            }
        }
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        private byte[] _ImagenAcompanante;
        public byte[] ImagenAcompanante
        {
            get { return _ImagenAcompanante; }
            set { _ImagenAcompanante = value; OnPropertyChanged("ImagenAcompanante"); }
        }

        #region DatosImputado
        private string anioD;
        public string AnioD
        {
            get { return anioD; }
            set { anioD = value; OnPropertyChanged("AnioD"); }
        }
        private string folioD;
        public string FolioD
        {
            get
            {
                return folioD;
            }
            set
            {
                folioD = value;
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
            get { return _ClasificacionJuridicaD; }
            set
            {
                _ClasificacionJuridicaD = value;
                OnPropertyChanged("ClasificacionJuridicaD");
            }
        }
        private string _EstatusD;
        public string EstatusD
        {
            get { return _EstatusD; }
            set
            {
                _EstatusD = value;
                OnPropertyChanged("EstatusD");
            }
        }
        private byte[] _ImagenIngreso;
        public byte[] ImagenIngreso
        {
            get { return _ImagenIngreso; }
            set { _ImagenIngreso = value; OnPropertyChanged("ImagenIngreso"); }
        }
        #endregion

        #endregion

        #region Externos
        private Visibility _ExternosVisible = Visibility.Collapsed;
        public Visibility ExternosVisible
        {
            get { return _ExternosVisible; }
            set { _ExternosVisible = value; OnPropertyChanged("ExternosVisible"); }
        }
        private string _TextDepartamentoExterno;
        public string TextDepartamentoExterno
        {
            get { return _TextDepartamentoExterno; }
            set { _TextDepartamentoExterno = value; OnPropertyChanged("TextDepartamentoExterno"); }
        }
        private string _TextInstitucionExterno;
        public string TextInstitucionExterno
        {
            get { return _TextInstitucionExterno; }
            set
            {
                _TextInstitucionExterno = value;
                OnPropertyChanged("TextInstitucionExterno");
            }
        }
        private string _TextFechaExterno;
        public string TextFechaExterno
        {
            get { return _TextFechaExterno; }
            set
            {
                _TextFechaExterno = value;
                OnPropertyChanged("TextFechaExterno");
            }
        }
        private string _TextHoraExterno;
        public string TextHoraExterno
        {
            get { return _TextHoraExterno; }
            set
            {
                _TextHoraExterno = value;
                OnPropertyChanged("TextHoraExterno");
            }
        }
        private string _TextAsuntoExterno;
        public string TextAsuntoExterno
        {
            get { return _TextAsuntoExterno; }
            set
            {
                _TextAsuntoExterno = value;
                OnPropertyChanged("TextAsuntoExterno");
            }
        }
        private string _TextObservacionExterno;
        public string TextObservacionExterno
        {
            get { return _TextObservacionExterno; }
            set
            {
                _TextObservacionExterno = value;
                OnPropertyChanged("TextObservacionExterno");
            }
        }
        private ObservableCollection<PUESTO> _ListPuestos;
        public ObservableCollection<PUESTO> ListPuestos
        {
            get { return _ListPuestos; }
            set { _ListPuestos = value; OnPropertyChanged("ListPuestos"); }
        }
        private ObservableCollection<AREA> _ListAreaExterno;
        public ObservableCollection<AREA> ListAreaExterno
        {
            get { return _ListAreaExterno; }
            set
            {
                _ListAreaExterno = value;
                OnPropertyChanged("ListAreaExterno");
            }
        }
        private short? _SelectPuestoExterno;
        public short? SelectPuestoExterno
        {
            get { return _SelectPuestoExterno; }
            set { _SelectPuestoExterno = value; OnPropertyChanged("SelectPuestoExterno"); }
        }
        private short _SelectAreaExterno;
        public short SelectAreaExterno
        {
            get { return _SelectAreaExterno; }
            set { _SelectAreaExterno = value; OnPropertyChanged("SelectAreaExterno"); }
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

        #region Aduana
        private bool BotonMenuGuardado = false;
        private short _AreaFamiliar;
        public short AreaFamiliar
        {
            get { return _AreaFamiliar; }
            set
            {
                _AreaFamiliar = value;
            }
        }
        private short AreaAbogados = Parametro.AREA_VISITA_ABOGADO;
        private List<ABOGADO_INGRESO> Administrativos = new List<ABOGADO_INGRESO>();
        private bool BanderaIntima = false;
        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> ListPersonasAuxiliar;
        private short _HeaderSortin;
        public short HeaderSortin
        {
            get { return _HeaderSortin; }
            set { _HeaderSortin = value; }
        }
        private bool BanderaSelectAll = false;
        private bool _SeleccionarTodoAcompanantesEnabled = false;
        public bool SeleccionarTodoAcompanantesEnabled
        {
            get { return _SeleccionarTodoAcompanantesEnabled; }
            set { _SeleccionarTodoAcompanantesEnabled = value; OnPropertyChanged("SeleccionarTodoAcompanantesEnabled"); }
        }
        private bool _SeleccionarTodoAcompanantes = false;
        public bool SeleccionarTodoAcompanantes
        {
            get { return _SeleccionarTodoAcompanantes; }
            set { _SeleccionarTodoAcompanantes = value; OnPropertyChanged("SeleccionarTodoAcompanantes"); }
        }
        private bool _SeleccionarTodoAbogadosEnabled = false;
        public bool SeleccionarTodoAbogadosEnabled
        {
            get { return _SeleccionarTodoAbogadosEnabled; }
            set { _SeleccionarTodoAbogadosEnabled = value; OnPropertyChanged("_SeleccionarTodoAbogadosEnabled"); }
        }
        private bool _SeleccionarTodoAbogados = false;
        public bool SeleccionarTodoAbogados
        {
            get { return _SeleccionarTodoAbogados; }
            set { _SeleccionarTodoAbogados = value; OnPropertyChanged("SeleccionarTodoAbogados"); }
        }
        private bool _SeleccionarTodoActuariosEnabled = false;
        public bool SeleccionarTodoActuariosEnabled
        {
            get { return _SeleccionarTodoActuariosEnabled; }
            set { _SeleccionarTodoActuariosEnabled = value; OnPropertyChanged("SeleccionarTodoActuariosEnabled"); }
        }
        private bool _SeleccionarTodoActuarios = false;
        public bool SeleccionarTodoActuarios
        {
            get { return _SeleccionarTodoActuarios; }
            set { _SeleccionarTodoActuarios = value; OnPropertyChanged("SeleccionarTodoActuarios"); }
        }
        private bool _SeleccionarTodoInternos = false;
        public bool SeleccionarTodoInternos
        {
            get { return _SeleccionarTodoInternos; }
            set
            {
                _SeleccionarTodoInternos = value;
                OnPropertyChanged("SeleccionarTodoInternos");
            }
        }
        private bool _SeleccionarTodosEnable = false;
        public bool SeleccionarTodosEnable
        {
            get { return _SeleccionarTodosEnable; }
            set { _SeleccionarTodosEnable = value; OnPropertyChanged("SeleccionarTodosEnable"); }
        }
        private SeleccionarTipoVisitaAduanaView _SeleccionarTipoVisitaAduana;
        public SeleccionarTipoVisitaAduanaView SeleccionarTipoVisitaAduana
        {
            get { return _SeleccionarTipoVisitaAduana; }
            set { _SeleccionarTipoVisitaAduana = value; OnPropertyChanged("SeleccionarTipoVisitaAduana"); }
        }
        private string TextIDLockerAuxiliar;
        private string _TextIDLocker;
        public string TextIDLocker
        {
            get { return _TextIDLocker; }
            set { _TextIDLocker = value; OnPropertyChanged("TextIDLocker"); }
        }
        private bool _LockerEnabled;
        public bool LockerEnabled
        {
            get { return _LockerEnabled; }
            set { _LockerEnabled = value; OnPropertyChanged("LockerEnabled"); }
        }
        private bool _VisitaLegalEnabled;
        public bool VisitaLegalEnabled
        {
            get { return _VisitaLegalEnabled; }
            set { _VisitaLegalEnabled = value; OnPropertyChanged("VisitaLegalEnabled"); }
        }
        private bool _VisitaFamiliarEnabled;
        public bool VisitaFamiliarEnabled
        {
            get { return _VisitaFamiliarEnabled; }
            set { _VisitaFamiliarEnabled = value; OnPropertyChanged("VisitaFamiliarEnabled"); }
        }
        private bool _VisitaEmpleadoEnabled;
        public bool VisitaEmpleadoEnabled
        {
            get { return _VisitaEmpleadoEnabled; }
            set { _VisitaEmpleadoEnabled = value; OnPropertyChanged("VisitaEmpleadoEnabled"); }
        }
        private bool _VisitaExternaEnabled;
        public bool VisitaExternaEnabled
        {
            get { return _VisitaExternaEnabled; }
            set { _VisitaExternaEnabled = value; OnPropertyChanged("VisitaExternaEnabled"); }
        }
        private int? SelectTipoPersona;
        private BitacoraAduana _SelectBitacoraAcceso;
        public BitacoraAduana SelectBitacoraAcceso
        {
            get { return _SelectBitacoraAcceso; }
            set { _SelectBitacoraAcceso = value; OnPropertyChanged("SelectBitacoraAcceso"); }
        }
        private Visibility _BitacoraAccesoEnabled;
        public Visibility BitacoraAccesoEnabled
        {
            get { return _BitacoraAccesoEnabled; }
            set { _BitacoraAccesoEnabled = value; OnPropertyChanged("BitacoraAccesoEnabled"); }
        }
        private ObservableCollection<BitacoraAduana> _ListBitacoraAcceso;
        public ObservableCollection<BitacoraAduana> ListBitacoraAcceso
        {
            get { return _ListBitacoraAcceso; }
            set { _ListBitacoraAcceso = value; OnPropertyChanged("ListBitacoraAcceso"); }
        }
        #endregion

        #region Empleados
        private Visibility _EmpleadosVisible = Visibility.Collapsed;
        public Visibility EmpleadosVisible
        {
            get { return _EmpleadosVisible; }
            set { _EmpleadosVisible = value; OnPropertyChanged("EmpleadosVisible"); }
        }
        //private ObservableCollection<AREA_TRABAJO> _ListDeptos;
        //public ObservableCollection<AREA_TRABAJO> ListDeptos
        //{
        //    get { return _ListDeptos; }
        //    set { _ListDeptos = value; OnPropertyChanged("ListDeptos"); }
        //}
        private short? _SelectDeptoEmpleado;
        public short? SelectDeptoEmpleado
        {
            get { return _SelectDeptoEmpleado; }
            set { _SelectDeptoEmpleado = value; OnPropertyChanged("SelectDeptoEmpleado"); }
        }
        private string _TextPuestoEmpleado;
        public string TextPuestoEmpleado
        {
            get { return _TextPuestoEmpleado; }
            set { _TextPuestoEmpleado = value; OnPropertyChanged("TextPuestoEmpleado"); }
        }
        #endregion

        #region Configuracion Permisos
        public bool _MenuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return _MenuGuardarEnabled; }
            set
            {
                _MenuGuardarEnabled = value;
                OnPropertyChanged("MenuGuardarEnabled");
            }
        }
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
            set
            {
                pEditar = value;
            }
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

        /*private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }*/

        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }

        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        #endregion

        #region Numero de pases
        private short pases_cp = Parametro.NO_PASES_CP;
        private short pases_admin = Parametro.NO_PASES_ADMIN;
        #endregion

        #region Parametros
        private short ParametroVisitaIntima;
        private string ParametroAbogadoTitular;
        private string ParametroAbogadoColaborador;
        private string ParametroAbogadoActuario;
        private short ParametroVisitaAutorizada;
        private short ParametroVisitaCancelada;
        private short ParametroVisitaSuspendido;
        private short ParametroVisitaRegistro;
        private short ParametroPersonaLegal;
        private short ParametroPersonaVisita;
        private short ParametroPersonaExterna;
        private short ParametroPersonaEmpleado;
        private short ParametroTipoVisitanteIntima;
        private short ParametroTipoVisitanteDepositante;
        private short ParametroTipoVisitaPorCentro;
        private short ParametroInternosPermitidosPorDia;
        private short ParametroVisitaAlaVes;
        private short ParametroToleranciaTraslado;
        private int ParametroTiempoDentroCentro;
        private string[] ParametroDoctoJuez;
        private string[] ParametroDoctoInterno;
        private short?[] ParametroEstatusInactivos;
        private bool ParametroRequiereGuardarHuellas;
        #endregion

        #region CADENA DOMICILIO
        private List<ENTIDAD> ListEntidadesAuxiliares;
        private List<MUNICIPIO> ListMunicipiosAuxiliares;
        private List<COLONIA> ListColoniasAuxiliares;
        #endregion
    }
    public class AbogadoIngresoAsignacion
    {
        public ABOGADO_INGRESO ABOGADO_INGRESO { get; set; }
        public bool ELIGE { get; set; }
    }
    public class ActuarioIngresoAsignacion
    {
        public ACTUARIO_INGRESO ACTUARIO_INGRESO { get; set; }
        public bool ELIGE { get; set; }
    }
    public class AcompananteAsignacion
    {
        public ACOMPANANTE ACOMPANANTE { get; set; }
        public bool ELEGIDO { get; set; }
    }
    public class VisitanteIngresoAsignacion
    {
        public VISITANTE_INGRESO VISITANTE_INGRESO { get; set; }
        public bool ELEGIDO { get; set; }
    }
}
