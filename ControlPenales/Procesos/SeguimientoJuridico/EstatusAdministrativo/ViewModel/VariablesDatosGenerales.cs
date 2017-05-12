using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    partial class EstatusAdministrativoViewModel
    {
        #region Listas
        private ObservableCollection<ESTADO_CIVIL> listEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> ListEstadoCivil
        {
            get { return listEstadoCivil; }
            set { listEstadoCivil = value; OnPropertyChanged("ListEstadoCivil"); }
        }

        private ObservableCollection<OCUPACION> listOcupacion;
        public ObservableCollection<OCUPACION> ListOcupacion
        {
            get { return listOcupacion; }
            set { listOcupacion = value; OnPropertyChanged("ListOcupacion"); }
        }

        private ObservableCollection<ESCOLARIDAD> listEscolaridad;
        public ObservableCollection<ESCOLARIDAD> ListEscolaridad
        {
            get { return listEscolaridad; }
            set { listEscolaridad = value; OnPropertyChanged("ListEscolaridad"); }
        }

        private ObservableCollection<RELIGION> listReligion;
        public ObservableCollection<RELIGION> ListReligion
        {
            get { return listReligion; }
            set { listReligion = value; OnPropertyChanged("ListReligion"); }
        }

        private ObservableCollection<ETNIA> listEtnia;
        public ObservableCollection<ETNIA> ListEtnia
        {
            get { return listEtnia; }
            set { listEtnia = value; OnPropertyChanged("ListEtnia"); }
        }

        private ObservableCollection<PAIS_NACIONALIDAD> listPaisNacionalidad;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisNacionalidad
        {
            get { return listPaisNacionalidad; }
            set { listPaisNacionalidad = value; OnPropertyChanged("ListPaisNacionalidad"); }
        }

        private ObservableCollection<PAIS_NACIONALIDAD> listPaisNacimiento;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisNacimiento
        {
            get { return listPaisNacimiento; }
            set { listPaisNacimiento = value; OnPropertyChanged("ListPaisNacimiento"); }
        }

        private ObservableCollection<PAIS_NACIONALIDAD> listPaisDomicilio;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisDomicilio
        {
            get { return listPaisDomicilio; }
            set { listPaisDomicilio = value; OnPropertyChanged("ListPaisDomicilio"); }
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

        private ObservableCollection<ENTIDAD> listEntidadNacimiento;
        public ObservableCollection<ENTIDAD> ListEntidadNacimiento
        {
            get { return listEntidadNacimiento; }
            set { listEntidadNacimiento = value; OnPropertyChanged("ListEntidadNacimiento"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipioNacimiento;
        public ObservableCollection<MUNICIPIO> ListMunicipioNacimiento
        {
            get { return listMunicipioNacimiento; }
            set { listMunicipioNacimiento = value; OnPropertyChanged("ListMunicipioNacimiento"); }
        }

        private ObservableCollection<PAIS_NACIONALIDAD> listPaisDomicilioPadre;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisDomicilioPadre
        {
            get { return listPaisDomicilioPadre; }
            set { listPaisDomicilioPadre = value; OnPropertyChanged("ListPaisDomicilioPadre"); }
        }

        private ObservableCollection<ENTIDAD> listEntidadDomicilioPadre;
        public ObservableCollection<ENTIDAD> ListEntidadDomicilioPadre
        {
            get { return listEntidadDomicilioPadre; }
            set { listEntidadDomicilioPadre = value; OnPropertyChanged("ListEntidadDomicilioPadre"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipioDomicilioPadre;
        public ObservableCollection<MUNICIPIO> ListMunicipioDomicilioPadre
        {
            get { return listMunicipioDomicilioPadre; }
            set { listMunicipioDomicilioPadre = value; OnPropertyChanged("ListMunicipioDomicilioPadre"); }
        }

        private ObservableCollection<COLONIA> listColoniaDomicilioPadre;
        public ObservableCollection<COLONIA> ListColoniaDomicilioPadre
        {
            get { return listColoniaDomicilioPadre; }
            set
            {
                listColoniaDomicilioPadre = value;
                OnPropertyChanged("ListColoniaDomicilioPadre");
            }
        }

        private ObservableCollection<PAIS_NACIONALIDAD> listPaisDomicilioMadre;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisDomicilioMadre
        {
            get { return listPaisDomicilioMadre; }
            set { listPaisDomicilioMadre = value; OnPropertyChanged("ListPaisDomicilioMadre"); }
        }

        private ObservableCollection<ENTIDAD> listEntidadDomicilioMadre;
        public ObservableCollection<ENTIDAD> ListEntidadDomicilioMadre
        {
            get { return listEntidadDomicilioMadre; }
            set { listEntidadDomicilioMadre = value; OnPropertyChanged("ListEntidadDomicilioMadre"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipioDomicilioMadre;
        public ObservableCollection<MUNICIPIO> ListMunicipioDomicilioMadre
        {
            get { return listMunicipioDomicilioMadre; }
            set { listMunicipioDomicilioMadre = value; OnPropertyChanged("ListMunicipioDomicilioMadre"); }
        }

        private ObservableCollection<COLONIA> listColoniaDomicilioMadre;
        public ObservableCollection<COLONIA> ListColoniaDomicilioMadre
        {
            get { return listColoniaDomicilioMadre; }
            set
            {
                listColoniaDomicilioMadre = value;
                OnPropertyChanged("ListColoniaDomicilioMadre");
            }
        }
        #endregion

        #region SelectDatosGenerales
        private string textEstatura;
        public string TextEstatura
        {
            get { return textEstatura; }
            set
            {
                textEstatura = value;
                OnPropertyValidateChanged("TextEstatura");
                ChecarValidaciones();
            }
        }
        private string textPeso;
        public string TextPeso
        {
            get { return textPeso; }
            set
            {
                textPeso = value;
                OnPropertyValidateChanged("TextPeso");
                ChecarValidaciones();
            }
        }
        private string selectSexo;
        public string SelectSexo
        {
            get { return selectSexo; }
            set
            {
                selectSexo = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectSexo");
            }
        }
        private short? selectEstadoCivil;
        public short? SelectEstadoCivil
        {
            get { return selectEstadoCivil; }
            set
            {
                selectEstadoCivil = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectEstadoCivil");
            }
        }
        private short? selectOcupacion;
        public short? SelectOcupacion
        {
            get { return selectOcupacion; }
            set
            {
                selectOcupacion = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectOcupacion");
            }
        }
        private short? selectEscolaridad;
        public short? SelectEscolaridad
        {
            get { return selectEscolaridad; }
            set
            {
                selectEscolaridad = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectEscolaridad");
            }
        }
        private short? selectReligion;
        public short? SelectReligion
        {
            get { return selectReligion; }
            set
            {
                selectReligion = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectReligion");
            }
        }
        private short? selectEtnia;
        public short? SelectEtnia
        {
            get { return selectEtnia; }
            set
            {
                selectEtnia = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectEtnia");
            }
        }
        #endregion

        #region SelectDomicilio
        private short? selectPais;
        public short? SelectPais
        {
            get { return selectPais; }
            set
            {
                if (value == 223)
                    selectPais = Parametro.PAIS;//82;
                else
                    selectPais = value;
                if (selectPais > 0)
                    ListEntidad = new ObservableCollection<ENTIDAD>((new cEntidad()).ObtenerTodos().OrderBy(o => o.DESCR));
                else
                    ListEntidad = new ObservableCollection<ENTIDAD>();
                ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                if (selectPais == Parametro.PAIS)//82)//Mexico
                {
                    SelectEntidad = Parametro.ESTADO; //2;//Baja California
                    EntidadEnabled = true;
                    MunicipioEnabled = true;
                    ColoniaEnabled = true;
                }
                else if (selectPais == -1)
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

                ChecarValidaciones();
                OnPropertyValidateChanged("SelectPais");
            }
        }
        private short? selectEntidad;
        public short? SelectEntidad
        {
            get { return selectEntidad; }
            set
            {
                selectEntidad = value;
                if (selectEntidad > 0)
                    ListMunicipio = new ObservableCollection<MUNICIPIO>((new cMunicipio()).ObtenerTodos(string.Empty, selectEntidad).OrderBy(o => o.MUNICIPIO1));
                else
                    ListMunicipio = new ObservableCollection<MUNICIPIO>();
                ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                if (selectEntidad == 33)
                    SelectMunicipio = 1001;
                else
                    SelectMunicipio = -1;

                ChecarValidaciones();
                OnPropertyValidateChanged("SelectEntidad");
            }
        }
        private short? selectMunicipio;
        public short? SelectMunicipio
        {
            get { return selectMunicipio; }
            set
            {
                selectMunicipio = value;
                if (selectMunicipio > 0) ListColonia = new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, selectMunicipio,SelectEntidad).OrderBy(o => o.DESCR));
                else
                    ListColonia = new ObservableCollection<COLONIA>();
                ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                if (SelectEntidad == 2)
                {
                    //SelectColonia = -1;
                    ColoniaEnabled = true;
                    if (ListColonia.Count == 1)
                    {
                        ListColonia = new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, 1001).OrderBy(o => o.DESCR));
                        ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                        SelectColoniaItem = ListColonia.Where(w => w.ID_COLONIA == 102).FirstOrDefault();
                        ColoniaEnabled = false;
                    }
                    else
                    {
                        ColoniaEnabled = true;
                        SelectColoniaItem = ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();
                    }
                }
                if (SelectEntidad != 2)
                {
                    if (ListColonia.Count == 1)
                    {
                        ListColonia = new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, 1001).OrderBy(o => o.DESCR));
                        ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                        SelectColoniaItem = ListColonia.Where(w => w.ID_COLONIA == 102).FirstOrDefault();
                        ColoniaEnabled = false;
                    }
                }
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectMunicipio");
            }
        }
        private COLONIA selectColoniaItem;
        public COLONIA SelectColoniaItem
        {
            get { return selectColoniaItem; }
            set
            {
                selectColoniaItem = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectColoniaItem");
            }
        }
        #endregion

        #region SelectNacimiento
        private short? selectNacionalidad;
        public short? SelectNacionalidad
        {
            get { return selectNacionalidad; }
            set
            {
                if (value == 223)
                    selectNacionalidad = Parametro.PAIS;//82;
                else
                    selectNacionalidad = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectNacionalidad");
            }
        }
        private short? selectPaisNacimiento;
        public short? SelectPaisNacimiento
        {
            get { return selectPaisNacimiento; }
            set
            {
                if (value == 223)
                    selectPaisNacimiento = Parametro.PAIS;//82;
                else
                    selectPaisNacimiento = value;
                if (selectPaisNacimiento > 0)
                    ListEntidadNacimiento = new ObservableCollection<ENTIDAD>((new cEntidad()).ObtenerTodos(string.Empty).OrderBy(o => o.DESCR));
                else
                    ListEntidadNacimiento = new ObservableCollection<ENTIDAD>();
                ListEntidadNacimiento.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                if (selectPaisNacimiento == Parametro.PAIS)//82)//Mexico
                {
                    SelectEntidadNacimiento = Parametro.ESTADO;// 2;//Baja California
                    EntidadNacimientoEnabled = true;
                    MunicipioNacimientoEnabled = true;
                    LugarNacimientoEnabled = false;
                }
                else if (selectPaisNacimiento == -1)
                {
                    SelectEntidadNacimiento = -1;
                    EntidadNacimientoEnabled = true;
                    MunicipioNacimientoEnabled = true;
                    LugarNacimientoEnabled = true;
                }
                else
                {
                    SelectEntidadNacimiento = 33;
                    EntidadNacimientoEnabled = false;
                    MunicipioNacimientoEnabled = false;
                    LugarNacimientoEnabled = true;
                }

                ChecarValidaciones();
                OnPropertyValidateChanged("SelectPaisNacimiento");
            }
        }
        private short? selectEntidadNacimiento;
        public short? SelectEntidadNacimiento
        {
            get { return selectEntidadNacimiento; }
            set
            {
                selectEntidadNacimiento = value;
                if (selectEntidadNacimiento > 0)
                    ListMunicipioNacimiento = new ObservableCollection<MUNICIPIO>((new cMunicipio()).ObtenerTodos(string.Empty, selectEntidadNacimiento).OrderBy(o => o.MUNICIPIO1));
                else
                    ListMunicipioNacimiento = new ObservableCollection<MUNICIPIO>();
                ListMunicipioNacimiento.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                if (selectEntidadNacimiento == 33)
                    SelectMunicipioNacimiento = 1001;
                else
                    SelectMunicipioNacimiento = -1;

                ChecarValidaciones();
                OnPropertyValidateChanged("SelectEntidadNacimiento");
            }
        }
        private short? selectMunicipioNacimiento;
        public short? SelectMunicipioNacimiento
        {
            get { return selectMunicipioNacimiento; }
            set
            {
                selectMunicipioNacimiento = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectMunicipioNacimiento");
            }
        }
        #endregion

        #region TextDomicilio
        private string textCalle;
        public string TextCalle
        {
            get { return textCalle; }
            set
            {
                textCalle = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextCalle");
            }
        }
        private int? textNumeroExterior;
        public int? TextNumeroExterior
        {
            get { return textNumeroExterior; }
            set
            {
                textNumeroExterior = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextNumeroExterior");
            }
        }
        private string textNumeroInterior;
        public string TextNumeroInterior
        {
            get { return textNumeroInterior; }
            set
            {
                textNumeroInterior = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextNumeroInterior");
            }
        }
        private string textTelefono;
        public string TextTelefono
        {
            get { return textTelefono; }
            set
            {
                textTelefono = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextTelefono");
            }
        }
        private int? textCodigoPostal;
        public int? TextCodigoPostal
        {
            get { return textCodigoPostal; }
            set
            {
                textCodigoPostal = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextCodigoPostal");
            }
        }
        private string textDomicilioTrabajo;
        public string TextDomicilioTrabajo
        {
            get { return textDomicilioTrabajo; }
            set
            {
                textDomicilioTrabajo = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextDomicilioTrabajo");
            }
        }
        private DateTime fechaEstado = Fechas.GetFechaDateServer;
        public DateTime FechaEstado
        {
            get { return fechaEstado; }
            set
            {
                if (fechaEstado != value)
                {
                    fechaEstado = value;
                    TiempoEstado();
                }

                ChecarValidaciones();
                OnPropertyValidateChanged("FechaEstado");
            }
        }
        private string aniosEstado;
        public string AniosEstado
        {
            get { return aniosEstado; }
            set
            {
                aniosEstado = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("AniosEstado");
            }
        }
        private string mesesEstado;
        public string MesesEstado
        {
            get { return mesesEstado; }
            set
            {
                mesesEstado = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("MesesEstado");
            }
        }
        #endregion

        #region TextNacimiento
        private string textLugarNacimientoExtranjero;
        public string TextLugarNacimientoExtranjero
        {
            get { return textLugarNacimientoExtranjero; }
            set
            {
                textLugarNacimientoExtranjero = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextLugarNacimientoExtranjero");
            }
        }
        private DateTime? textFechaNacimiento;
        public DateTime? TextFechaNacimiento
        {
            get { return textFechaNacimiento; }
            set
            {
                textFechaNacimiento = value;
                CalcularEdad();
                ChecarValidaciones();
                OnPropertyValidateChanged("TextFechaNacimiento");
            }
        }
        private int textEdad;
        public int TextEdad
        {
            get { return textEdad; }
            set
            {
                textEdad = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextEdad");
            }
        }
        #endregion

        #region TextPadres
        private string textPadrePaterno;
        public string TextPadrePaterno
        {
            get { return textPadrePaterno; }
            set
            {
                textPadrePaterno = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextPadrePaterno");
            }
        }
        private string textMadrePaterno;
        public string TextMadrePaterno
        {
            get { return textMadrePaterno; }
            set
            {
                textMadrePaterno = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextMadrePaterno");
            }
        }
        private string textPadreMaterno;
        public string TextPadreMaterno
        {
            get { return textPadreMaterno; }
            set
            {
                textPadreMaterno = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextPadreMaterno");
            }
        }
        private string textMadreMaterno;
        public string TextMadreMaterno
        {
            get { return textMadreMaterno; }
            set
            {
                textMadreMaterno = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextMadreMaterno");
            }
        }
        private string textPadreNombre;
        public string TextPadreNombre
        {
            get { return textPadreNombre; }
            set
            {
                textPadreNombre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextPadreNombre");
            }
        }
        private string textMadreNombre;
        public string TextMadreNombre
        {
            get { return textMadreNombre; }
            set
            {
                textMadreNombre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextMadreNombre");
            }
        }
        #endregion

        #region CheckIdentificacion
        private bool checkPadreFinado;
        public bool CheckPadreFinado
        {
            get { return checkPadreFinado; }
            set
            {
                checkPadreFinado = value;
                if (value && !MismoDomicilioPadre)
                    DomicilioPadreEnabled = false;
                else if (value && MismoDomicilioPadre)
                    DomicilioPadreEnabled = false;
                else if (!value && MismoDomicilioPadre)
                    DomicilioPadreEnabled = false;
                else if (!value && !MismoDomicilioPadre)
                    DomicilioPadreEnabled = true;
                if (value)
                {
                    MismoDomicilioMadreEnabled = false;
                    MismoDomicilioMadre = false;
                }
                else
                    MismoDomicilioMadreEnabled = true;
                if (IsSelectedIdentificacion && TabDatosGenerales)
                {
                    setValidacionesIdentificacionDatosGenerales();
                    //getDatosDomiciliosPadres();
                }
                ChecarValidaciones();
                OnPropertyChanged("");
            }
        }
        private bool checkMadreFinado;
        public bool CheckMadreFinado
        {
            get { return checkMadreFinado; }
            set
            {
                checkMadreFinado = value;
                if (value && !MismoDomicilioMadre)
                    DomicilioMadreEnabled = false;
                else if (value && MismoDomicilioMadre)
                    DomicilioMadreEnabled = false;
                else if (!value && MismoDomicilioMadre)
                    DomicilioMadreEnabled = false;
                else if (!value && !MismoDomicilioMadre)
                    DomicilioMadreEnabled = true;
                if (value)
                {
                    MismoDomicilioMadreEnabled = false;
                    MismoDomicilioMadre = false;
                }
                else
                {
                    MismoDomicilioMadreEnabled = true;
                    if (Imputado != null)
                    {
                        var madre = Imputado.IMPUTADO_PADRES.Where(w => w.ID_PADRE == "M").Any() ?
                            Imputado.IMPUTADO_PADRES.Where(w => w.ID_PADRE == "M").FirstOrDefault() : null;
                        if (madre == null)
                        {
                            SelectPaisDomicilioMadre = Parametro.PAIS;//82;
                        }
                    }
                }
                if (IsSelectedIdentificacion && TabDatosGenerales)
                {
                    setValidacionesIdentificacionDatosGenerales();
                    //getDatosDomiciliosPadres();
                }
                ChecarValidaciones();
                OnPropertyValidateChanged("CheckMadreFinado");
            }
        }
        #endregion

        #region Domicilio Padre

        #region SelectDomicilio
        private short? selectPaisDomicilioPadre;
        public short? SelectPaisDomicilioPadre
        {
            get { return selectPaisDomicilioPadre; }
            set
            {
                if (value == 223)
                    selectPaisDomicilioPadre = Parametro.PAIS;//82;
                else
                    selectPaisDomicilioPadre = value;
                if (selectPaisDomicilioPadre > 0)
                    ListEntidadDomicilioPadre = new ObservableCollection<ENTIDAD>((new cEntidad()).ObtenerTodos().OrderBy(o => o.DESCR));
                else
                    ListEntidadDomicilioPadre = new ObservableCollection<ENTIDAD>();
                ListEntidadDomicilioPadre.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                if (selectPaisDomicilioPadre == Parametro.PAIS)//82)//Mexico
                {
                    SelectEntidadDomicilioPadre = Parametro.ESTADO;// 2;//Baja California
                    EntidadEnabledDomicilioPadre = true;
                    MunicipioEnabledDomicilioPadre = true;
                    ColoniaEnabledDomicilioPadre = true;
                }
                else if (selectPaisDomicilioPadre == -1)//SELECCIONE
                {
                    SelectEntidadDomicilioPadre = -1;//SELECCIONE
                    EntidadEnabledDomicilioPadre = true;
                    MunicipioEnabledDomicilioPadre = true;
                    ColoniaEnabledDomicilioPadre = true;
                }
                else
                {
                    SelectEntidadDomicilioPadre = 33;//EXTRANJERO
                    EntidadEnabledDomicilioPadre = false;
                    MunicipioEnabledDomicilioPadre = false;
                    ColoniaEnabledDomicilioPadre = false;
                }

                ChecarValidaciones();
                OnPropertyValidateChanged("SelectPaisDomicilioPadre");

            }
        }
        private short? selectEntidadDomicilioPadre;
        public short? SelectEntidadDomicilioPadre
        {
            get { return selectEntidadDomicilioPadre; }
            set
            {
                selectEntidadDomicilioPadre = value;
                if (selectEntidadDomicilioPadre > 0)
                    ListMunicipioDomicilioPadre = new ObservableCollection<MUNICIPIO>((new cMunicipio()).ObtenerTodos(string.Empty, selectEntidadDomicilioPadre).OrderBy(o => o.MUNICIPIO1));
                else
                    ListMunicipioDomicilioPadre = new ObservableCollection<MUNICIPIO>();
                ListMunicipioDomicilioPadre.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                if (selectEntidadDomicilioPadre == 33)
                {
                    SelectMunicipioDomicilioPadre = 1001;
                    MunicipioEnabledDomicilioPadre = false;
                }
                else
                {
                    SelectMunicipioDomicilioPadre = -1;
                    MunicipioEnabledDomicilioPadre = true;
                }

                ChecarValidaciones();
                OnPropertyValidateChanged("SelectEntidadDomicilioPadre");
            }
        }
        private short? selectMunicipioDomicilioPadre;
        public short? SelectMunicipioDomicilioPadre
        {
            get { return selectMunicipioDomicilioPadre; }
            set
            {
                selectMunicipioDomicilioPadre = value;
                if (selectMunicipioDomicilioPadre > 0)
                    ListColoniaDomicilioPadre = new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, selectMunicipioDomicilioPadre).OrderBy(o => o.DESCR));
                else
                    ListColoniaDomicilioPadre = new ObservableCollection<COLONIA>();
                ListColoniaDomicilioPadre.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                if (selectMunicipioDomicilioPadre == 1001)
                {
                    SelectColoniaDomicilioPadre = 102;
                    ColoniaEnabledDomicilioPadre = false;
                }
                else
                {
                    SelectColoniaDomicilioPadre = -1;
                    ColoniaEnabledDomicilioPadre = true;
                }

                ChecarValidaciones();
                OnPropertyValidateChanged("SelectMunicipioDomicilioPadre");
            }
        }
        private COLONIA selectColoniaItemDomicilioPadre;
        public COLONIA SelectColoniaItemDomicilioPadre
        {
            get { return selectColoniaItemDomicilioPadre; }
            set
            {
                selectColoniaItemDomicilioPadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectColoniaItemDomicilioPadre");
            }
        }
        private int? selectColoniaDomicilioPadre;
        public int? SelectColoniaDomicilioPadre
        {
            get { return selectColoniaDomicilioPadre; }
            set
            {
                selectColoniaDomicilioPadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectColoniaDomicilioPadre");
            }
        }
        #endregion

        #region TextDomicilio
        private string textCalleDomicilioPadre;
        public string TextCalleDomicilioPadre
        {
            get { return textCalleDomicilioPadre; }
            set
            {
                textCalleDomicilioPadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextCalleDomicilioPadre");
            }
        }
        private int? textNumeroExteriorDomicilioPadre;
        public int? TextNumeroExteriorDomicilioPadre
        {
            get { return textNumeroExteriorDomicilioPadre; }
            set
            {
                textNumeroExteriorDomicilioPadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextNumeroExteriorDomicilioPadre");
            }
        }
        private string textNumeroInteriorDomicilioPadre;
        public string TextNumeroInteriorDomicilioPadre
        {
            get { return textNumeroInteriorDomicilioPadre; }
            set
            {
                textNumeroInteriorDomicilioPadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextNumeroInteriorDomicilioPadre");
            }
        }
        private int? textCodigoPostalDomicilioPadre;
        public int? TextCodigoPostalDomicilioPadre
        {
            get { return textCodigoPostalDomicilioPadre; }
            set
            {
                textCodigoPostalDomicilioPadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextCodigoPostalDomicilioPadre");
            }
        }
        #endregion

        #region Enableds
        private bool entidadEnabledDomicilioPadre;
        public bool EntidadEnabledDomicilioPadre
        {
            get { return entidadEnabledDomicilioPadre; }
            set
            {
                entidadEnabledDomicilioPadre = value;
                ChecarValidaciones();
                OnPropertyChanged("EntidadEnabledDomicilioPadre");
            }
        }
        private bool municipioEnabledDomicilioPadre;
        public bool MunicipioEnabledDomicilioPadre
        {
            get { return municipioEnabledDomicilioPadre; }
            set
            {
                municipioEnabledDomicilioPadre = value;
                ChecarValidaciones();
                OnPropertyChanged("MunicipioEnabledDomicilioPadre");
            }
        }
        private bool coloniaEnabledDomicilioPadre;
        public bool ColoniaEnabledDomicilioPadre
        {
            get { return coloniaEnabledDomicilioPadre; }
            set
            {
                coloniaEnabledDomicilioPadre = value;
                ChecarValidaciones();
                OnPropertyChanged("ColoniaEnabledDomicilioPadre");
            }
        }
        private bool domicilioPadreEnabled;
        public bool DomicilioPadreEnabled
        {
            get { return domicilioPadreEnabled; }
            set
            {
                domicilioPadreEnabled = value;
                ChecarValidaciones();
                OnPropertyChanged("DomicilioPadreEnabled");
            }
        }
        private bool mismoDomicilioPadre;
        public bool MismoDomicilioPadre
        {
            get { return mismoDomicilioPadre; }
            set
            {
                mismoDomicilioPadre = value;
                if (value && !CheckPadreFinado)
                    DomicilioPadreEnabled = false;
                else if (value && CheckPadreFinado)
                    DomicilioPadreEnabled = false;
                else if (!value && CheckPadreFinado)
                    DomicilioPadreEnabled = false;
                else if (!value && !CheckPadreFinado)
                    DomicilioPadreEnabled = true;
                if (IsSelectedIdentificacion && TabDatosGenerales)
                {
                    setValidacionesIdentificacionDatosGenerales();
                    //getDatosDomiciliosPadres();
                }
                ChecarValidaciones();
                OnPropertyChanged("");
            }
        }
        private bool mismoDomicilioPadreEnabled = true;
        public bool MismoDomicilioPadreEnabled
        {
            get { return mismoDomicilioPadreEnabled; }
            set { mismoDomicilioPadreEnabled = value; OnPropertyChanged("MismoDomicilioPadreEnabled"); }
        }
        #endregion

        #endregion

        #region Domicilio Madre

        #region SelectDomicilio
        private short? selectPaisDomicilioMadre;
        public short? SelectPaisDomicilioMadre
        {
            get { return selectPaisDomicilioMadre; }
            set
            {
                if (value == 223)
                    selectPaisDomicilioMadre = Parametro.PAIS;//82;
                else
                    selectPaisDomicilioMadre = value;
                if (selectPaisDomicilioMadre > 0)
                    ListEntidadDomicilioMadre = new ObservableCollection<ENTIDAD>((new cEntidad()).ObtenerTodos().OrderBy(o => o.DESCR));
                else
                    ListEntidadDomicilioMadre = new ObservableCollection<ENTIDAD>();
                ListEntidadDomicilioMadre.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                if (selectPaisDomicilioMadre == Parametro.PAIS)//82)//Mexico
                {
                    SelectEntidadDomicilioMadre = Parametro.ESTADO; //2;//Baja California
                    EntidadEnabledDomicilioMadre = true;
                    MunicipioEnabledDomicilioMadre = true;
                    ColoniaEnabledDomicilioMadre = true;
                }
                else if (selectPaisDomicilioMadre == -1)//SELECCIONE
                {
                    SelectEntidadDomicilioMadre = -1;//SELECCIONE
                    EntidadEnabledDomicilioMadre = false;
                    MunicipioEnabledDomicilioMadre = true;
                    ColoniaEnabledDomicilioMadre = true;
                }
                else
                {
                    SelectEntidadDomicilioMadre = 33;//EXTRANJERO
                    EntidadEnabledDomicilioMadre = false;
                    MunicipioEnabledDomicilioMadre = false;
                    ColoniaEnabledDomicilioMadre = false;
                }

                ChecarValidaciones();
                OnPropertyValidateChanged("SelectPaisDomicilioMadre");

            }
        }
        private short? selectEntidadDomicilioMadre;
        public short? SelectEntidadDomicilioMadre
        {
            get { return selectEntidadDomicilioMadre; }
            set
            {
                selectEntidadDomicilioMadre = value;
                if (selectEntidadDomicilioMadre > 0)
                    ListMunicipioDomicilioMadre = new ObservableCollection<MUNICIPIO>((new cMunicipio()).ObtenerTodos(string.Empty, selectEntidadDomicilioMadre).OrderBy(o => o.MUNICIPIO1));
                else
                    ListMunicipioDomicilioMadre = new ObservableCollection<MUNICIPIO>();
                ListMunicipioDomicilioMadre.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                if (selectEntidadDomicilioMadre == 33)
                {
                    SelectMunicipioDomicilioMadre = 1001;
                    MunicipioEnabledDomicilioMadre = false;
                }
                else
                {
                    SelectMunicipioDomicilioMadre = -1;
                    MunicipioEnabledDomicilioMadre = true;
                }

                ChecarValidaciones();
                OnPropertyValidateChanged("SelectEntidadDomicilioMadre");
            }
        }
        private short? selectMunicipioDomicilioMadre;
        public short? SelectMunicipioDomicilioMadre
        {
            get { return selectMunicipioDomicilioMadre; }
            set
            {
                selectMunicipioDomicilioMadre = value;
                if (selectMunicipioDomicilioMadre > 0)
                    ListColoniaDomicilioMadre = new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, selectMunicipioDomicilioMadre).OrderBy(o => o.DESCR));
                else
                    ListColoniaDomicilioMadre = new ObservableCollection<COLONIA>();
                ListColoniaDomicilioMadre.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                if (selectMunicipioDomicilioMadre == 1001)
                {
                    SelectColoniaDomicilioMadre = 102;
                    ColoniaEnabledDomicilioMadre = false;
                }
                else
                {
                    SelectColoniaDomicilioMadre = -1;
                    ColoniaEnabledDomicilioMadre = true;
                }

                ChecarValidaciones();
                OnPropertyValidateChanged("SelectMunicipioDomicilioMadre");
            }
        }
        private COLONIA selectColoniaItemDomicilioMadre;
        public COLONIA SelectColoniaItemDomicilioMadre
        {
            get { return selectColoniaItemDomicilioMadre; }
            set
            {
                selectColoniaItemDomicilioMadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectColoniaItemDomicilioMadre");
            }
        }
        private int? selectColoniaDomicilioMadre;
        public int? SelectColoniaDomicilioMadre
        {
            get { return selectColoniaDomicilioMadre; }
            set
            {
                selectColoniaDomicilioMadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectColoniaDomicilioMadre");
            }
        }
        #endregion

        #region TextDomicilio
        private string textCalleDomicilioMadre;
        public string TextCalleDomicilioMadre
        {
            get { return textCalleDomicilioMadre; }
            set
            {
                textCalleDomicilioMadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextCalleDomicilioMadre");
            }
        }
        private int? textNumeroExteriorDomicilioMadre;
        public int? TextNumeroExteriorDomicilioMadre
        {
            get { return textNumeroExteriorDomicilioMadre; }
            set
            {
                textNumeroExteriorDomicilioMadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextNumeroExteriorDomicilioMadre");
            }
        }
        private string textNumeroInteriorDomicilioMadre;
        public string TextNumeroInteriorDomicilioMadre
        {
            get { return textNumeroInteriorDomicilioMadre; }
            set
            {
                textNumeroInteriorDomicilioMadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextNumeroInteriorDomicilioMadre");
            }
        }
        private int? textCodigoPostalDomicilioMadre;
        public int? TextCodigoPostalDomicilioMadre
        {
            get { return textCodigoPostalDomicilioMadre; }
            set
            {
                textCodigoPostalDomicilioMadre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("TextCodigoPostalDomicilioMadre");
            }
        }
        #endregion

        #region Enableds
        private bool entidadEnabledDomicilioMadre;
        public bool EntidadEnabledDomicilioMadre
        {
            get { return entidadEnabledDomicilioMadre; }
            set
            {
                entidadEnabledDomicilioMadre = value;
                ChecarValidaciones();
                OnPropertyChanged("EntidadEnabledDomicilioMadre");
            }
        }
        private bool municipioEnabledDomicilioMadre;
        public bool MunicipioEnabledDomicilioMadre
        {
            get { return municipioEnabledDomicilioMadre; }
            set
            {
                municipioEnabledDomicilioMadre = value;
                ChecarValidaciones();
                OnPropertyChanged("MunicipioEnabledDomicilioMadre");
            }
        }
        private bool coloniaEnabledDomicilioMadre;
        public bool ColoniaEnabledDomicilioMadre
        {
            get { return coloniaEnabledDomicilioMadre; }
            set
            {
                coloniaEnabledDomicilioMadre = value;
                ChecarValidaciones();
                OnPropertyChanged("ColoniaEnabledDomicilioMadre");
            }
        }
        private bool domicilioMadreEnabled;
        public bool DomicilioMadreEnabled
        {
            get { return domicilioMadreEnabled; }
            set
            {
                domicilioMadreEnabled = value;
                ChecarValidaciones();
                OnPropertyChanged("DomicilioMadreEnabled");
            }
        }
        private bool mismoDomicilioMadre;
        public bool MismoDomicilioMadre
        {
            get { return mismoDomicilioMadre; }
            set
            {
                mismoDomicilioMadre = value;
                if (value && !CheckMadreFinado)
                    DomicilioMadreEnabled = false;
                else if (value && CheckMadreFinado)
                    DomicilioMadreEnabled = false;
                else if (!value && CheckMadreFinado)
                    DomicilioMadreEnabled = false;
                else if (!value && !CheckMadreFinado)
                    DomicilioMadreEnabled = true;
                if (IsSelectedIdentificacion && TabDatosGenerales)
                {
                    setValidacionesIdentificacionDatosGenerales();
                    //getDatosDomiciliosPadres();
                }
                ChecarValidaciones();
                OnPropertyChanged("");
            }
        }
        private bool mismoDomicilioMadreEnabled = true;
        public bool MismoDomicilioMadreEnabled
        {
            get { return mismoDomicilioMadreEnabled; }
            set { mismoDomicilioMadreEnabled = value; OnPropertyChanged("MismoDomicilioMadreEnabled"); }
        }
        #endregion

        #endregion

        #region Otros
        private int nacionalidadLabelColumn = 1;
        public int NacionalidadLabelColumn
        {
            get { return nacionalidadLabelColumn; }
            set { nacionalidadLabelColumn = value; OnPropertyChanged("NacionalidadLabelColumn"); }
        }
        private int nacionalidadComboboxColumn = 1;
        public int NacionalidadComboboxColumn
        {
            get { return nacionalidadComboboxColumn; }
            set { nacionalidadComboboxColumn = value; OnPropertyChanged("NacionalidadComboboxColumn"); }
        }
        private int nacionalidadLabelColspan = 1;
        public int NacionalidadLabelColspan
        {
            get { return nacionalidadLabelColspan; }
            set { nacionalidadLabelColspan = value; OnPropertyChanged("NacionalidadLabelColspan"); }
        }
        private int nacionalidadComboboxColspan = 1;
        public int NacionalidadComboboxColspan
        {
            get { return nacionalidadComboboxColspan; }
            set { nacionalidadComboboxColspan = value; OnPropertyChanged("NacionalidadComboboxColspan"); }
        }
        private Visibility gridEstaturapesoVisible = Visibility.Visible;
        public Visibility GridEstaturaPesoVisible
        {
            get { return gridEstaturapesoVisible; }
            set { gridEstaturapesoVisible = value; OnPropertyChanged("GridEstaturaPesoVisible"); }
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
        private bool entidadNacimientoEnabled;
        public bool EntidadNacimientoEnabled
        {
            get { return entidadNacimientoEnabled; }
            set { entidadNacimientoEnabled = value; OnPropertyChanged("EntidadNacimientoEnabled"); }
        }
        private bool municipioNacimientoEnabled;
        public bool MunicipioNacimientoEnabled
        {
            get { return municipioNacimientoEnabled; }
            set { municipioNacimientoEnabled = value; OnPropertyChanged("MunicipioNacimientoEnabled"); }
        }
        private bool lugarNacimientoEnabled = true;
        public bool LugarNacimientoEnabled
        {
            get { return lugarNacimientoEnabled; }
            set { lugarNacimientoEnabled = value; OnPropertyChanged("LugarNacimientoEnabled"); }
        }
        #endregion

        //Nuevo
        private ObservableCollection<IDIOMA> lstIdioma;
        public ObservableCollection<IDIOMA> LstIdioma
        {
            get { return lstIdioma; }
            set { lstIdioma = value; OnPropertyChanged("LstIdioma"); }
        }

        private short? selectedIdioma;
        public short? SelectedIdioma
        {
            get { return selectedIdioma; }
            set { selectedIdioma = value; OnPropertyValidateChanged("SelectedIdioma"); }
        }

        private ObservableCollection<DIALECTO> lstDialecto;
        public ObservableCollection<DIALECTO> LstDialecto
        {
            get { return lstDialecto; }
            set { lstDialecto = value; OnPropertyChanged("LstDialecto"); }
        }

        private short? selectedDialecto;
        public short? SelectedDialecto
        {
            get { return selectedDialecto; }
            set { selectedDialecto = value; OnPropertyValidateChanged("SelectedDialecto"); }
        }

        private bool requiereTraductor;
        public bool RequiereTraductor
        {
            get { return requiereTraductor; }
            set { requiereTraductor = value; OnPropertyValidateChanged("RequiereTraductor"); }
        }

        private bool estaturaPesoVisible = true;
        public bool EstaturaPesoVisible
        {
            get { return estaturaPesoVisible; }
            set { estaturaPesoVisible = value; OnPropertyChanged("EstaturaPesoVisible"); }
        }
    }
}