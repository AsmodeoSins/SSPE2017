using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        #region Listas
        private ObservableCollection<ESTADO_CIVIL> listEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> ListEstadoCivil
        {
            get { return listEstadoCivil; }
            set { listEstadoCivil = value; OnPropertyChanged("ListEstadoCivil"); }
        }

        private ObservableCollection<IDIOMA> lstIdioma;
        public ObservableCollection<IDIOMA> LstIdioma
        {
            get { return lstIdioma; }
            set { lstIdioma = value; OnPropertyChanged("LstIdioma"); }
        }

        private ObservableCollection<DIALECTO> lstDialecto;
        public ObservableCollection<DIALECTO> LstDialecto
        {
            get { return lstDialecto; }
            set { lstDialecto = value; OnPropertyChanged("LstDialecto"); }
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
                OnPropertyChanged("TextEstatura");

            }
        }
        private string textPeso;
        public string TextPeso
        {
            get { return textPeso; }
            set
            {
                textPeso = value;
                OnPropertyChanged("TextPeso");

            }
        }
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
        private short? selectEstadoCivil;
        public short? SelectEstadoCivil
        {
            get { return selectEstadoCivil; }
            set
            {
                selectEstadoCivil = value;

                OnPropertyChanged("SelectEstadoCivil");
            }
        }
        private short? selectOcupacion;
        public short? SelectOcupacion
        {
            get { return selectOcupacion; }
            set
            {
                selectOcupacion = value;

                OnPropertyChanged("SelectOcupacion");
            }
        }
        private short? selectEscolaridad;
        public short? SelectEscolaridad
        {
            get { return selectEscolaridad; }
            set
            {
                selectEscolaridad = value;

                OnPropertyChanged("SelectEscolaridad");
            }
        }
        private short? selectReligion;
        public short? SelectReligion
        {
            get { return selectReligion; }
            set
            {
                selectReligion = value;

                OnPropertyChanged("SelectReligion");
            }
        }
        private short? selectEtnia;
        public short? SelectEtnia
        {
            get { return selectEtnia; }
            set
            {
                selectEtnia = value;

                OnPropertyChanged("SelectEtnia");
            }
        }
        private short? selectedIdioma;
        public short? SelectedIdioma
        {
            get { return selectedIdioma; }
            set { selectedIdioma = value; OnPropertyChanged("SelectedIdioma"); }
        }
        private short? selectedDialecto;
        public short? SelectedDialecto
        {
            get { return selectedDialecto; }
            set { selectedDialecto = value; OnPropertyChanged("SelectedDialecto"); }
        }
        private bool requiereTraductor;
        public bool RequiereTraductor
        {
            get { return requiereTraductor; }
            set { requiereTraductor = value; OnPropertyChanged("RequiereTraductor"); }
        }
        private bool estaturaPesoVisible = true;
        public bool EstaturaPesoVisible
        {
            get { return estaturaPesoVisible; }
            set { estaturaPesoVisible = value; OnPropertyChanged("EstaturaPesoVisible"); }
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
                {
                    //llenarEntidades();
                    //ListEntidad = new ObservableCollection<ENTIDAD>((new cEntidad()).ObtenerTodos().OrderBy(o => o.DESCR));
                }
                else
                { }// ListEntidad = new ObservableCollection<ENTIDAD>();
                //ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                if (selectPais == Parametro.PAIS)//82)//Mexico
                {
                    SelectEntidad = 2;//Baja California
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


                OnPropertyValidateChanged("SelectPais");
            }
        }
        public async void llenarEntidades()
        {
            ListEntidad = await StaticSourcesViewModel.CargarDatos<ObservableCollection<ENTIDAD>>(() =>
                new ObservableCollection<ENTIDAD>((new cEntidad()).ObtenerTodos().OrderBy(o => o.DESCR)));
        }
        private short? selectEntidad;
        public short? SelectEntidad
        {
            get { return selectEntidad; }
            set
            {
                selectEntidad = value;
                if (selectEntidad > 0)
                {
                    //llenarMunicipios(value);
                    //ListMunicipio = new ObservableCollection<MUNICIPIO>((new cMunicipio()).ObtenerTodos(string.Empty, value).OrderBy(o => o.MUNICIPIO1));
                }
                else
                { }// ListMunicipio = new ObservableCollection<MUNICIPIO>();
                //ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                if (selectEntidad == 33)
                    SelectMunicipio = 1001;
                else
                    SelectMunicipio = -1;


                OnPropertyChanged("SelectEntidad");
            }
        }
        public async void llenarMunicipios(short? entidad)
        {
            ListMunicipio = await StaticSourcesViewModel.CargarDatos<ObservableCollection<MUNICIPIO>>(() =>
                new ObservableCollection<MUNICIPIO>((new cMunicipio()).ObtenerTodos(string.Empty, entidad).OrderBy(o => o.MUNICIPIO1)));
        }
        private short? selectMunicipio;
        public short? SelectMunicipio
        {
            get { return selectMunicipio; }
            set
            {
                selectMunicipio = value;
                OnPropertyChanged("SelectMunicipio");
            }
        }
        public async void llenarColonias(short? municipio, short? entidad)
        {
            ListColonia = await StaticSourcesViewModel.CargarDatos<ObservableCollection<COLONIA>>(() =>
                new ObservableCollection<COLONIA>((new cColonia()).ObtenerTodos(string.Empty, municipio, entidad).OrderBy(o => o.DESCR)));
        }
        private COLONIA selectColoniaItem;
        public COLONIA SelectColoniaItem
        {
            get { return selectColoniaItem; }
            set
            {
                selectColoniaItem = value;

                OnPropertyChanged("SelectColoniaItem");
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

                OnPropertyChanged("SelectNacionalidad");
            }
        }
        private short? selectPaisNacimiento;
        public short? SelectPaisNacimiento
        {
            get { return selectPaisNacimiento; }
            set
            {
                selectPaisNacimiento = value;
                OnPropertyChanged("SelectPaisNacimiento");
            }
        }
        public async void llenarEntidadesNacimiento()
        {
            ListEntidadNacimiento = await StaticSourcesViewModel.CargarDatos<ObservableCollection<ENTIDAD>>(() =>
                new ObservableCollection<ENTIDAD>((new cEntidad()).ObtenerTodos().OrderBy(o => o.DESCR)));
        }
        private short? selectEntidadNacimiento;
        public short? SelectEntidadNacimiento
        {
            get { return selectEntidadNacimiento; }
            set
            {
                selectEntidadNacimiento = value;
                OnPropertyChanged("SelectEntidadNacimiento");
            }
        }
        public async void llenarMunicipiosNacimiento(short? entidad)
        {
            ListMunicipioNacimiento = await StaticSourcesViewModel.CargarDatos<ObservableCollection<MUNICIPIO>>(() =>
                new ObservableCollection<MUNICIPIO>((new cMunicipio()).ObtenerTodos(string.Empty, entidad).OrderBy(o => o.MUNICIPIO1)));
        }
        private short? selectMunicipioNacimiento;
        public short? SelectMunicipioNacimiento
        {
            get { return selectMunicipioNacimiento; }
            set
            {
                selectMunicipioNacimiento = value;

                OnPropertyChanged("SelectMunicipioNacimiento");
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

                OnPropertyChanged("TextCalle");
            }
        }
        private int? textNumeroExterior;
        public int? TextNumeroExterior
        {
            get { return textNumeroExterior; }
            set
            {
                textNumeroExterior = value;

                OnPropertyChanged("TextNumeroExterior");
            }
        }
        private string textNumeroInterior;
        public string TextNumeroInterior
        {
            get { return textNumeroInterior; }
            set
            {
                textNumeroInterior = value;

                OnPropertyChanged("TextNumeroInterior");
            }
        }
        private string textTelefono;
        public string TextTelefono
        {
            get { return textTelefono; }
            set
            {
                textTelefono = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;

                OnPropertyChanged("TextTelefono");
            }
        }
        private int? textCodigoPostal;
        public int? TextCodigoPostal
        {
            get { return textCodigoPostal; }
            set
            {
                textCodigoPostal = value;

                OnPropertyChanged("TextCodigoPostal");
            }
        }
        private string textDomicilioTrabajo;
        public string TextDomicilioTrabajo
        {
            get { return textDomicilioTrabajo; }
            set
            {
                textDomicilioTrabajo = value;

                OnPropertyChanged("TextDomicilioTrabajo");
            }
        }
        private DateTime? fechaEstado = Fechas.GetFechaDateServer;
        public DateTime? FechaEstado
        {
            get { return fechaEstado; }
            set
            {
                if (fechaEstado != value)
                {
                    fechaEstado = value;
                }


                OnPropertyChanged("FechaEstado");
            }
        }
        private string aniosEstado;
        public string AniosEstado
        {
            get { return aniosEstado; }
            set
            {
                aniosEstado = value;

                OnPropertyChanged("AniosEstado");
            }
        }
        private string mesesEstado;
        public string MesesEstado
        {
            get { return mesesEstado; }
            set
            {
                mesesEstado = value;

                OnPropertyChanged("MesesEstado");
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

                OnPropertyChanged("TextLugarNacimientoExtranjero");
            }
        }
        private DateTime? textFechaNacimiento;
        public DateTime? TextFechaNacimiento
        {
            get { return textFechaNacimiento; }
            set
            {
                textFechaNacimiento = value;
                OnPropertyChanged("TextFechaNacimiento");
            }
        }
        private int textEdad;
        public int TextEdad
        {
            get { return textEdad; }
            set
            {
                textEdad = value;

                OnPropertyChanged("TextEdad");
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

                OnPropertyChanged("TextPadrePaterno");
            }
        }
        private string textMadrePaterno;
        public string TextMadrePaterno
        {
            get { return textMadrePaterno; }
            set
            {
                textMadrePaterno = value;

                OnPropertyChanged("TextMadrePaterno");
            }
        }
        private string textPadreMaterno;
        public string TextPadreMaterno
        {
            get { return textPadreMaterno; }
            set
            {
                textPadreMaterno = value;

                OnPropertyChanged("TextPadreMaterno");
            }
        }
        private string textMadreMaterno;
        public string TextMadreMaterno
        {
            get { return textMadreMaterno; }
            set
            {
                textMadreMaterno = value;

                OnPropertyChanged("TextMadreMaterno");
            }
        }
        private string textPadreNombre;
        public string TextPadreNombre
        {
            get { return textPadreNombre; }
            set
            {
                textPadreNombre = value;

                OnPropertyChanged("TextPadreNombre");
            }
        }
        private string textMadreNombre;
        public string TextMadreNombre
        {
            get { return textMadreNombre; }
            set
            {
                textMadreNombre = value;

                OnPropertyChanged("TextMadreNombre");
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
                OnPropertyChanged("");
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
                selectPaisDomicilioPadre = value;
                OnPropertyChanged("SelectPaisDomicilioPadre");

            }
        }
        private short? selectEntidadDomicilioPadre;
        public short? SelectEntidadDomicilioPadre
        {
            get { return selectEntidadDomicilioPadre; }
            set
            {
                selectEntidadDomicilioPadre = value;
                OnPropertyChanged("SelectEntidadDomicilioPadre");
            }
        }
        private short? selectMunicipioDomicilioPadre;
        public short? SelectMunicipioDomicilioPadre
        {
            get { return selectMunicipioDomicilioPadre; }
            set
            {
                selectMunicipioDomicilioPadre = value;
                OnPropertyChanged("SelectMunicipioDomicilioPadre");
            }
        }
        private COLONIA selectColoniaItemDomicilioPadre;
        public COLONIA SelectColoniaItemDomicilioPadre
        {
            get { return selectColoniaItemDomicilioPadre; }
            set
            {
                selectColoniaItemDomicilioPadre = value;

                OnPropertyChanged("SelectColoniaItemDomicilioPadre");
            }
        }
        private int? selectColoniaDomicilioPadre;
        public int? SelectColoniaDomicilioPadre
        {
            get { return selectColoniaDomicilioPadre; }
            set
            {
                selectColoniaDomicilioPadre = value;

                OnPropertyChanged("SelectColoniaDomicilioPadre");
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

                OnPropertyChanged("TextCalleDomicilioPadre");
            }
        }
        private int? textNumeroExteriorDomicilioPadre;
        public int? TextNumeroExteriorDomicilioPadre
        {
            get { return textNumeroExteriorDomicilioPadre; }
            set
            {
                textNumeroExteriorDomicilioPadre = value;

                OnPropertyChanged("TextNumeroExteriorDomicilioPadre");
            }
        }
        private string textNumeroInteriorDomicilioPadre;
        public string TextNumeroInteriorDomicilioPadre
        {
            get { return textNumeroInteriorDomicilioPadre; }
            set
            {
                textNumeroInteriorDomicilioPadre = value;

                OnPropertyChanged("TextNumeroInteriorDomicilioPadre");
            }
        }
        private int? textCodigoPostalDomicilioPadre;
        public int? TextCodigoPostalDomicilioPadre
        {
            get { return textCodigoPostalDomicilioPadre; }
            set
            {
                textCodigoPostalDomicilioPadre = value;

                OnPropertyChanged("TextCodigoPostalDomicilioPadre");
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
                selectPaisDomicilioMadre = value;
                OnPropertyChanged("SelectPaisDomicilioMadre");

            }
        }
        private short? selectEntidadDomicilioMadre;
        public short? SelectEntidadDomicilioMadre
        {
            get { return selectEntidadDomicilioMadre; }
            set
            {
                selectEntidadDomicilioMadre = value;
                OnPropertyChanged("SelectEntidadDomicilioMadre");
            }
        }
        private short? selectMunicipioDomicilioMadre;
        public short? SelectMunicipioDomicilioMadre
        {
            get { return selectMunicipioDomicilioMadre; }
            set
            {
                selectMunicipioDomicilioMadre = value;
                OnPropertyChanged("SelectMunicipioDomicilioMadre");
            }
        }
        private COLONIA selectColoniaItemDomicilioMadre;
        public COLONIA SelectColoniaItemDomicilioMadre
        {
            get { return selectColoniaItemDomicilioMadre; }
            set
            {
                selectColoniaItemDomicilioMadre = value;

                OnPropertyChanged("SelectColoniaItemDomicilioMadre");
            }
        }
        private int? selectColoniaDomicilioMadre;
        public int? SelectColoniaDomicilioMadre
        {
            get { return selectColoniaDomicilioMadre; }
            set
            {
                selectColoniaDomicilioMadre = value;

                OnPropertyChanged("SelectColoniaDomicilioMadre");
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

                OnPropertyChanged("TextCalleDomicilioMadre");
            }
        }
        private int? textNumeroExteriorDomicilioMadre;
        public int? TextNumeroExteriorDomicilioMadre
        {
            get { return textNumeroExteriorDomicilioMadre; }
            set
            {
                textNumeroExteriorDomicilioMadre = value;

                OnPropertyChanged("TextNumeroExteriorDomicilioMadre");
            }
        }
        private string textNumeroInteriorDomicilioMadre;
        public string TextNumeroInteriorDomicilioMadre
        {
            get { return textNumeroInteriorDomicilioMadre; }
            set
            {
                textNumeroInteriorDomicilioMadre = value;

                OnPropertyChanged("TextNumeroInteriorDomicilioMadre");
            }
        }
        private int? textCodigoPostalDomicilioMadre;
        public int? TextCodigoPostalDomicilioMadre
        {
            get { return textCodigoPostalDomicilioMadre; }
            set
            {
                textCodigoPostalDomicilioMadre = value;

                OnPropertyChanged("TextCodigoPostalDomicilioMadre");
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
        private System.Windows.Visibility gridEstaturapesoVisible = Visibility.Visible;
        public System.Windows.Visibility GridEstaturaPesoVisible
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
    }
}