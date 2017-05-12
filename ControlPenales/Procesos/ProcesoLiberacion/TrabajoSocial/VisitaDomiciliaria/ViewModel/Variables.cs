using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales
{
    partial class VisitaDomiciliariaViewModel
    {
        #region ActualWidthNombreCroquis
        private double _ActualWidthNombreCroquis;

        public double ActualWidthNombreCroquis
        {
            get { return _ActualWidthNombreCroquis; }
            set { _ActualWidthNombreCroquis = value; OnPropertyChanged("ActualWidthNombreCroquis"); }
        }
        private double _ActualWidthtelefonoCroquis;


        public double ActualWidthtelefonoCroquis
        {
            get { return _ActualWidthtelefonoCroquis; }
            set { _ActualWidthtelefonoCroquis = value; OnPropertyChanged("ActualWidthtelefonoCroquis"); }
        }
        
        #endregion

        #region MaxWidhtActual Pantalla
        private double? _MaxWidhtActualGeneric;

        public double?  MaxWidhtActualGeneric
        {
            get { return _MaxWidhtActualGeneric; }
            set { _MaxWidhtActualGeneric = value; OnPropertyChanged("MaxWidhtActualGeneric"); }
        }
        #endregion
        
        #region TabsSelect

        private bool _SelectDatosPersonaEntrev;

        public bool SelectDatosPersonaEntrev
        {
            get { return _SelectDatosPersonaEntrev; }
            set { _SelectDatosPersonaEntrev = value; OnPropertyChanged("SelectDatosPersonaEntrev"); }
        }

        private bool _SelectDatosgenerales;


        public bool SelectDatosgenerales
        {
            get { return _SelectDatosgenerales; }
            set { _SelectDatosgenerales = value; OnPropertyChanged("SelectDatosgenerales"); }
        }

        private bool _SelectDatosAnexos;


        public bool SelectDatosAnexos
        {
            get { return _SelectDatosAnexos; }
            set { _SelectDatosAnexos = value; OnPropertyChanged("SelectDatosAnexos"); }
        }

        private bool  _TabDatosPersonaEntrev;

        public bool TabDatosPersonaEntrev
        {
            get { return _TabDatosPersonaEntrev; }
            set {
                if (_TabDatosPersonaEntrev == false && SelectDatosPersonaEntrev==false)
                {
                    SelectDatosPersonaEntrev = true;
                    //ValidacionDatosGenerales();
                    ValidacionPersonaEntrevistada();
                    //ValidacionCroquis();
                 
                }
                _TabDatosPersonaEntrev = value; 
                
                OnPropertyChanged("TabDatosPersonaEntrev"); }
        }

        private bool _TabDatosGenerales;

        public bool TabDatosGenerales
        {
            get { return _TabDatosGenerales; }
            set {
                if (_TabDatosGenerales == false && SelectDatosgenerales == false)
                {
                    SelectDatosgenerales = true;
                    
                    ValidacionDatosGenerales();
                    //ValidacionPersonaEntrevistada();
                    //ValidacionCroquis();
                    
                }
                _TabDatosGenerales = value; OnPropertyChanged("TabDatosGenerales"); }
        }
        private bool _TabDatosAnexos;

        public bool TabDatosAnexos
        {
            get { return _TabDatosAnexos; }
            set {
                if (_TabDatosAnexos == false && SelectDatosAnexos==false)
                {
                    SelectDatosAnexos = true;
                    //ValidacionDatosGenerales();
                    //ValidacionPersonaEntrevistada();
                    ValidacionCroquis();
                }
                _TabDatosAnexos = value; OnPropertyChanged("TabDatosAnexos"); }
        }

        #endregion
        
        #region MaxLenght
        private decimal _LugarMax;

        public decimal LugarMax
        {
            get { return _LugarMax; }
            set { _LugarMax = value; OnPropertyChanged("LugarMax"); }
        }

        private decimal _HoraMax;

        public decimal HoraMax
        {
            get { return _HoraMax; }
            set { _HoraMax = value; OnPropertyChanged("HoraMax"); }
        }

        private decimal _MedidaCutMax;

        public decimal MedidaCutMax
        {
            get { return _MedidaCutMax; }
            set { _MedidaCutMax = value; OnPropertyChanged("MedidaCutMax"); }

        }
        private decimal _MotivoVisitaDomMax;

        public decimal MotivoVisitaDomMax
        {
            get { return _MotivoVisitaDomMax; }
            set { _MotivoVisitaDomMax = value; OnPropertyChanged("MotivoVisitaDomMax"); }
        }

        private decimal _NombreEntrevistadoMax;

        public decimal NombreEntrevistadoMax
        {
            get { return _NombreEntrevistadoMax; }
            set { _NombreEntrevistadoMax = value; OnPropertyChanged("NombreEntrevistadoMax"); }
        }

        private decimal _CalleEntrvMax;

        public decimal CalleEntrvMax
        {
            get { return _CalleEntrvMax; }
            set { _CalleEntrvMax = value; OnPropertyChanged("CalleEntrvMax"); }
        }

        private decimal _TiempoConocerMax;

        public decimal TiempoConocerMax
        {
            get { return _TiempoConocerMax; }
            set { _TiempoConocerMax = value; OnPropertyChanged("TiempoConocerMax"); }
        }


        private decimal _RelacionSentenciadoMax;

        public decimal RelacionSentenciadoMax
        {
            get { return _RelacionSentenciadoMax; }
            set { _RelacionSentenciadoMax = value; OnPropertyChanged("RelacionSentenciadoMax"); }
        }


        private decimal _ObservacionesMax;

        public decimal ObservacionesMax
        {
            get { return _ObservacionesMax; }
            set { _ObservacionesMax = value; OnPropertyChanged("ObservacionesMax"); }
        }
        private decimal _NombreCroquisMax;

        public decimal NombreCroquisMax
        {
            get { return _NombreCroquisMax; }
            set { _NombreCroquisMax = value; OnPropertyChanged("NombreCroquisMax"); }
        }

        private decimal _DireccionCroquisMax;

        public decimal DireccionCroquisMax
        {
            get { return _DireccionCroquisMax; }
            set { _DireccionCroquisMax = value; OnPropertyChanged("DireccionCroquisMax"); }
        }

        private decimal _TiempoConocerleMax;

        public decimal TiempoConocerleMax
        {
            get { return _TiempoConocerleMax; }
            set { _TiempoConocerleMax = value; OnPropertyChanged("TiempoConocerleMax"); }
        }
        #endregion      
        
        #region EntrevistaInical

        private string _TextCausaPenalEntrevista;

        public string TextCausaPenalEntrevista
        {
            get { return _TextCausaPenalEntrevista; }
            set { _TextCausaPenalEntrevista = value; OnPropertyChanged("TextCausaPenalEntrevista"); }
        }

        private DateTime? _TextFechaEntrv;

        public DateTime? TextFechaEntrv
        {
            get { return _TextFechaEntrv; }
            set { _TextFechaEntrv = value; OnPropertyChanged("TextFechaEntrv"); }
        }


        private string _TextHoraEntrevista;

        public string TextHoraEntrevista
        {
            get { return _TextHoraEntrevista; }
            set { _TextHoraEntrevista = value; OnPropertyChanged("TextHoraEntrevista"); }
        }

        //private DateTime? _HoraEntrv;
        //public DateTime? HoraEntrv
        //{
        //    get { return _HoraEntrv; }
        //    set { _HoraEntrv = value; OnPropertyChanged("HoraEntrv"); }
        //}


        private string _TextLugarEntrevista;

        public string TextLugarEntrevista
        {
            get { return _TextLugarEntrevista; }
            set { _TextLugarEntrevista = value; OnPropertyChanged("TextLugarEntrevista"); }
        }

        private string _TextTiempoAntiguedad;

        public string TextTiempoAntiguedad
        {
            get { return _TextTiempoAntiguedad; }
            set { _TextTiempoAntiguedad = value; OnPropertyChanged("TextTiempoAntiguedad"); }
        }

        #endregion

        #region Menu

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
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

        private bool _MenuAyudaEnabled = false;

        public bool MenuAyudaEnabled
        {
            get { return _MenuAyudaEnabled; }
            set { _MenuAyudaEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        #endregion
        
        #region Datos Generales
        private SOCIOECONOMICO _Estudio;
        public SOCIOECONOMICO Estudio
        {
            get { return _Estudio; }
            set { _Estudio = value; OnPropertyChanged("Estudio"); }
        }

        private IMPUTADO selectedInterno;
        public IMPUTADO SelectedInterno
        {
            get { return selectedInterno; }
            set { selectedInterno = value; OnPropertyChanged("SelectedInterno"); }
        }
        
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

        private string tituloTop = "Visita Domiciliaria";
        public string TituloTop
        {
            get { return tituloTop; }
            set { tituloTop = value; OnPropertyChanged("TituloTop"); }
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

        private ESTADO_CIVIL _SelectedEdoCivilGrupoFamiliar;
        public ESTADO_CIVIL SelectedEdoCivilGrupoFamiliar
        {
            get { return _SelectedEdoCivilGrupoFamiliar; }
            set { _SelectedEdoCivilGrupoFamiliar = value; OnPropertyChanged("SelectedEdoCivilGrupoFamiliar"); }

        }

        private ObservableCollection<PAIS_NACIONALIDAD> listPaisNacimiento;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisNacimiento
        {
            get { return listPaisNacimiento; }
            set { listPaisNacimiento = value; OnPropertyChanged("ListPaisNacimiento"); }
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

        private PAIS_NACIONALIDAD selectedPaisNacimiento;
        public PAIS_NACIONALIDAD SelectedPaisNacimiento
        {
            get { return selectedPaisNacimiento; }
            set
            {
                selectedPaisNacimiento = value;
                ListEntidadNacimiento = value != null ? new ObservableCollection<ENTIDAD>(value.ENTIDAD) : new ObservableCollection<ENTIDAD>();
                ListEntidadNacimiento.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                SelectEntidadNacimiento = -1;
                OnPropertyChanged("SelectedPaisNacimiento");
            }
        }

        private ENTIDAD selectedEntidadNacimiento;
        public ENTIDAD SelectedEntidadNacimiento
        {
            get { return selectedEntidadNacimiento; }
            set
            {
                selectedEntidadNacimiento = value;
                ListMunicipioNacimiento = value != null ? new ObservableCollection<MUNICIPIO>(value.MUNICIPIO) : new ObservableCollection<MUNICIPIO>();
                ListMunicipioNacimiento.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                SelectMunicipioNacimiento = -1;
                OnPropertyChanged("SelectedEntidadNacimiento");
            }
        }

        private short? selectNacionalidad = Parametro.PAIS;
        public short? SelectNacionalidad
        {
            get { return selectNacionalidad; }
            set
            {
                selectNacionalidad = value;
                OnPropertyValidateChanged("SelectNacionalidad");
            }
        }

        private short? selectPaisNacimiento = Parametro.PAIS;
        public short? SelectPaisNacimiento
        {
            get { return selectPaisNacimiento; }
            set
            {
                selectPaisNacimiento = value;
                //if (value == Parametro.PAIS)
                //   // LugarNacimientoEnabled = false;
                //else
                    //LugarNacimientoEnabled = true;
                OnPropertyValidateChanged("SelectPaisNacimiento");
            }
        }

        private short? selectEntidadNacimiento = -1;
        public short? SelectEntidadNacimiento
        {
            get { return selectEntidadNacimiento; }
            set
            {
                selectEntidadNacimiento = value;
                OnPropertyValidateChanged("SelectEntidadNacimiento");
            }
        }

        private short? selectMunicipioNacimiento = -1;
        public short? SelectMunicipioNacimiento
        {
            get { return selectMunicipioNacimiento; }
            set
            {
                selectMunicipioNacimiento = value;
                OnPropertyValidateChanged("SelectMunicipioNacimiento");
            }
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

        private PAIS_NACIONALIDAD selectedPais;
        public PAIS_NACIONALIDAD SelectedPais
        {
            get { return selectedPais; }
            set
            {
                selectedPais = value;
                ListEntidad = value != null ? new ObservableCollection<ENTIDAD>(value.ENTIDAD) : new ObservableCollection<ENTIDAD>();
                ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                SelectEntidad = -1;
                OnPropertyChanged("SelectedPais");
            }
        }

        private ENTIDAD selectedEntidad;
        public ENTIDAD SelectedEntidad
        {
            get { return selectedEntidad; }
            set
            {
                selectedEntidad = value;
                ListMunicipio = value != null ? new ObservableCollection<MUNICIPIO>(value.MUNICIPIO) : new ObservableCollection<MUNICIPIO>();
                ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                SelectMunicipio = -1;
                OnPropertyChanged("SelectedEntidad");
            }
        }

        private MUNICIPIO selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return selectedMunicipio; }
            set
            {
                selectedMunicipio = value;
                //ListColonia = value != null ? new ObservableCollection<COLONIA>(value.COLONIA) : new ObservableCollection<COLONIA>();
                //ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                //SelectColonia = -1;
                OnPropertyChanged("SelectedMunicipio");
            }
        }

        private short? selectPais = Parametro.PAIS;//82;
        public short? SelectPais
        {
            get { return selectPais; }
            set
            {
                selectPais = value;
                OnPropertyValidateChanged("SelectPais");
            }
        }

        private short? selectEntidad = -1;
        public short? SelectEntidad
        {
            get { return selectEntidad; }
            set
            {
                selectEntidad = value;
                OnPropertyValidateChanged("SelectEntidad");
            }
        }

        private short? selectMunicipio = -1;
        public short? SelectMunicipio
        {
            get { return selectMunicipio; }
            set
            {
                selectMunicipio = value;
                OnPropertyValidateChanged("SelectMunicipio");
            }
        }

        private string _SelectSexo;
        public string SelectSexo
        {
            get { return _SelectSexo; }
            set { _SelectSexo = value; OnPropertyChanged("SelectSexo"); }
        }

        private ObservableCollection<ESTADO_CIVIL> lstEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> LstEstadoCivil
        {
            get { return lstEstadoCivil; }
            set { lstEstadoCivil = value; OnPropertyChanged("LstEstadoCivil"); }
        }

        private string textTelefono;
        public string TextTelefono
        {
            get { return textTelefono; }
            set
            {
                textTelefono = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;
                OnPropertyValidateChanged("TextTelefono");
            }
        }

        private string _textEntidadFederativa;
        public string textEntidadFederativa
        {
            get { return _textEntidadFederativa; }
            set { _textEntidadFederativa = value; OnPropertyChanged("textEntidadFederativa"); }
        }

        private short? selectEstadoCivil = -1;
        public short? SelectEstadoCivil
        {
            get { return selectEstadoCivil; }
            set
            {
                if (value == selectEstadoCivil)
                    return;

                selectEstadoCivil = value;

                OnPropertyValidateChanged("SelectEstadoCivil");
            }
        }

        private ObservableCollection<ESCOLARIDAD> listEscolaridad;
        public ObservableCollection<ESCOLARIDAD> ListEscolaridad
        {
            get { return listEscolaridad; }
            set { listEscolaridad = value; OnPropertyChanged("ListEscolaridad"); }
        }

        private short? selectEscolaridad = -1;
        public short? SelectEscolaridad
        {
            get { return selectEscolaridad; }
            set
            {
              

                selectEscolaridad = value;

                OnPropertyValidateChanged("SelectEscolaridad");
            }
        }

        private string textLugarNacimientoExtranjero;
        public string TextLugarNacimientoExtranjero
        {
            get { return textLugarNacimientoExtranjero; }
            set
            {
                textLugarNacimientoExtranjero = value;
                OnPropertyValidateChanged("TextLugarNacimientoExtranjero");
            }
        }
        
        private DateTime? fechaEstado = Fechas.GetFechaDateServer;
        public DateTime? FechaEstado
        {
            get { return fechaEstado; }
            set
            {
                fechaEstado = value;
                if (value != null)
                {
                    int a, m, d = 0;
                    var hoy = Fechas.GetFechaDateServer;
                    new Fechas().DiferenciaFechas(hoy, value.Value, out a, out m, out d);
                    AniosEstado = a.ToString();
                    MesesEstado = m.ToString();
                }
                else
                {
                    AniosEstado = MesesEstado = "0";
                }
                OnPropertyValidateChanged("FechaEstado");
            }
        }

        private string aniosEstado = "0";
        public string AniosEstado
        {
            get { return aniosEstado; }
            set
            {
                aniosEstado = value;
                OnPropertyValidateChanged("AniosEstado");
            }
        }

        private string mesesEstado = "0";
        public string MesesEstado
        {
            get { return mesesEstado; }
            set
            {
                mesesEstado = value;
                OnPropertyValidateChanged("MesesEstado");
            }
        }

        private ObservableCollection<OCUPACION> _ListOcupacion;
        public ObservableCollection<OCUPACION> ListOcupacion
        {
            get { return _ListOcupacion; }
            set { _ListOcupacion = value; OnPropertyChanged("ListOcupacion"); }
        }

        private short? _SelectOcupacion = -1;
        public short? SelectOcupacion
        {
            get { return _SelectOcupacion; }
            set { _SelectOcupacion = value; OnPropertyChanged("SelectOcupacion"); }
        }

        private string _TextDelitoImputa;
        public string TextDelitoImputa
        {
            get { return _TextDelitoImputa; }
            set { _TextDelitoImputa = value; OnPropertyChanged("TextDelitoImputa"); }
        }

        private string _TextTiempoRdicaEstado;
        public string TextTiempoRdicaEstado
        {
            get { return _TextTiempoRdicaEstado; }
            set { _TextTiempoRdicaEstado = value; OnPropertyChanged("TextTiempoRdicaEstado"); }
        }

        private string _TextApodo;
        public string TextApodo
        {
            get { return _TextApodo; }
            set { _TextApodo = value; OnPropertyChanged("TextApodo"); }
        }

        private ObservableCollection<RELIGION> _ListReligion;
        public ObservableCollection<RELIGION> ListReligion
        {
            get { return _ListReligion; }
            set { _ListReligion = value; OnPropertyChanged("ListReligion"); }
        }

        private short? selectReligion = -1;
        public short? SelectReligion
        {
            get { return selectReligion; }
            set
            {
                if (value == selectReligion)
                    return;

                selectReligion = value;

                OnPropertyValidateChanged("SelectReligion");
            }
        }

        private ObservableCollection<IDIOMA> _LstIdioma;
        public ObservableCollection<IDIOMA> LstIdioma
        {
            get { return _LstIdioma; }
            set { _LstIdioma = value; OnPropertyChanged("LstIdioma"); }
        }

        private short? _SelectedIdioma = 8;
        public short? SelectedIdioma
        {
            get { return _SelectedIdioma; }
            set { _SelectedIdioma = value; OnPropertyChanged("SelectedIdioma"); }
        }

        private DateTime? textFechaNacimiento;
        public DateTime? TextFechaNacimiento
        {
            get { return textFechaNacimiento; }
            set
            {
                textFechaNacimiento = value;
                if (value != null)
                {
                    //Calcula Edad
                    TextEdad = new Fechas().CalculaEdad(value);
                }
                else
                    TextEdad = 0;
                OnPropertyValidateChanged("TextFechaNacimiento");
            }
        }

        private int? _TextEdad;
        public int? TextEdad
        {
            get { return _TextEdad; }
            set { _TextEdad = value; OnPropertyChanged("TextEdad"); }
        }

        private string _textOcupacion;
        public string TextOcupacion
        {
            get { return _textOcupacion; }
            set { _textOcupacion = value; OnPropertyChanged("TextOcupacion"); }
        }

        private string _TextCalle;
        public string TextCalle
        {
            get { return _TextCalle; }
            set { _TextCalle = value; OnPropertyChanged("TextCalle"); }
        }

        private string _TextNumeroInterior;
        public string TextNumeroInterior
        {
            get { return _TextNumeroInterior; }
            set { _TextNumeroInterior = value; OnPropertyChanged("TextNumeroInterior"); }
        }

        private string _TextNumeroExterior;
        public string TextNumeroExterior
        {
            get { return _TextNumeroExterior; }
            set { _TextNumeroExterior = value; OnPropertyChanged("TextNumeroExterior"); }
        }

        private string _textOficio;
        public string textOficio
        {
            get { return _textOficio; }
            set { _textOficio = value; OnPropertyChanged("_textOficio"); }
        }

        private string _TextGrupoEtnico;
        public string TextGrupoEtnico
        {
            get { return _TextGrupoEtnico; }
            set { _TextGrupoEtnico = value; OnPropertyChanged("TextGrupoEtnico"); }
        }

        private int indexMenu;
        public int IndexMenu
        {
            get { return indexMenu; }
            set { indexMenu = value; OnPropertyChanged("IndexMenu"); }
        }

        private bool nuevoProcesoEnabled = false;
        public bool NuevoProcesoEnabled
        {
            get { return nuevoProcesoEnabled; }
            set { nuevoProcesoEnabled = value; OnPropertyChanged("NuevoProcesoEnabled"); }
        }

        private bool seleccionarProcesoEnabled = false;
        public bool SeleccionarProcesoEnabled
        {
            get { return seleccionarProcesoEnabled; }
            set { seleccionarProcesoEnabled = value; OnPropertyChanged("SeleccionarProcesoEnabled"); }
        }

        private Visibility emptyProceso = Visibility.Visible;
        public Visibility EmptyProceso
        {
            get { return emptyProceso; }
            set { emptyProceso = value; OnPropertyChanged("EmptyProceso"); }
        }

        private PROCESO_LIBERTAD auxProcesoLibertad;
        
        private PRS_VISITA_DOMICILIARIA SelectedVisitaDomiciliaria;
        
        private PROCESO_LIBERTAD selectedProcesoLibertad;
        public PROCESO_LIBERTAD SelectedProcesoLibertad
        {
            get { return selectedProcesoLibertad; }
            set
            {
                selectedProcesoLibertad = value;
                if (value != null)
                    SeleccionarProcesoEnabled = true;
                else
                    SeleccionarProcesoEnabled = false;
                OnPropertyChanged("SelectedProcesoLibertad");

            }
        }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                // NuevaMJEnabled = value != null ? true : false;
                SelectMJEnabled = value != null ? true : false;
                if (value != null)
                {
                    var foto = value.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                    if (foto != null)
                        ImagenInterno = foto.BIOMETRICO;
                    else
                    {
                        if (value.INGRESO != null)
                        {
                            var ingreso = value.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                            if (ingreso != null)
                            {
                                var fotoIngreso = ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                                if (fotoIngreso != null)
                                    ImagenInterno = fotoIngreso.BIOMETRICO;
                                else
                                    ImagenInterno = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenInterno = new Imagenes().getImagenPerson();
                        }
                    }
                    if (value.PROCESO_LIBERTAD != null)
                    {
                        if (value.PROCESO_LIBERTAD.Count == 0)
                        {
                            EmptyProceso = Visibility.Visible;
                        }
                        else
                            EmptyProceso = Visibility.Collapsed;
                    }
                    else
                        EmptyProceso = Visibility.Visible;
                }
                else
                {
                    EmptyProceso = Visibility.Visible;
                    ImagenInterno = new Imagenes().getImagenPerson();
                }
                OnPropertyChanged("SelectExpediente");
            }
        }

        private ObservableCollection<LIBERADO_MEDIDA_JUDICIAL> lstLiberadoMJ;
        public ObservableCollection<LIBERADO_MEDIDA_JUDICIAL> LstLiberadoMJ
        {
            get { return lstLiberadoMJ; }
            set { lstLiberadoMJ = value; OnPropertyChanged("LstLiberadoMJ"); }
        }

        private Visibility emptyMJVisible = System.Windows.Visibility.Collapsed;
        public Visibility EmptyMJVisible
        {
            get { return emptyMJVisible; }
            set { emptyMJVisible = value; OnPropertyChanged("EmptyMJVisible"); }
        }

        private bool nuevaMJEnabled = false;
        public bool NuevaMJEnabled
        {
            get { return nuevaMJEnabled; }
            set { nuevaMJEnabled = value; OnPropertyChanged("NuevaMJEnabled"); }
        }

        private bool _EnableDatosEntrv;
        public bool EnableDatosEntrv
        {
            get { return _EnableDatosEntrv; }
            set { _EnableDatosEntrv = value; OnPropertyChanged("EnableDatosEntrv"); }
        }

        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                {
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
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
            }
        }

        private string tituloModal;
        public string TituloModal
        {
            get { return tituloModal; }
            set { tituloModal = value; OnPropertyChanged("TituloModal"); }
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

        private string _TextMedidaCautelar;
        public string TextMedidaCautelar
        {
            get { return _TextMedidaCautelar; }
            set {
                if (value!=null)
                {
                    value = value.Trim();
                }
                _TextMedidaCautelar = value; 
                
                
                OnPropertyChanged("TextMedidaCautelar"); }
        }
        #endregion

        #region Datos Personal Entrevistado
        private string _TextNombreEntrevistado;

        public string TextNombreEntrevistado
        {
            get { return _TextNombreEntrevistado; }
            set { _TextNombreEntrevistado = value; OnPropertyChanged("TextNombreEntrevistado"); }
        }

        private string _TextEdadEntrevistado;

        public string TextEdadEntrevistado
        {
            get { return _TextEdadEntrevistado; }
            set { _TextEdadEntrevistado = value; OnPropertyChanged("TextEdadEntrevistado"); }
        }

        private string _TextEntidadFederativaEntrevistado;

        public string TextEntidadFederativaEntrevistado
        {
            get { return _TextEntidadFederativaEntrevistado; }
            set { _TextEntidadFederativaEntrevistado = value; OnPropertyChanged("TextEntidadFederativaEntrevistado"); }
        }

        private string _TextCalleEntrevistado;

        public string TextCalleEntrevistado
        {
            get { return _TextCalleEntrevistado; }
            set { _TextCalleEntrevistado = value; OnPropertyChanged("TextCalleEntrevistado"); }
        }

        private string _TextNumeroInteriorEntrevistado;

        public string TextNumeroInteriorEntrevistado
        {
            get { return _TextNumeroInteriorEntrevistado; }
            set { _TextNumeroInteriorEntrevistado = value; OnPropertyChanged("TextNumeroInteriorEntrevistado"); }
        }

        private string _TextNumeroExteriorEntrevistado;

        public string TextNumeroExteriorEntrevistado
        {
            get { return _TextNumeroExteriorEntrevistado; }
            set { _TextNumeroExteriorEntrevistado = value; OnPropertyChanged("TextNumeroExteriorEntrevistado"); }
        }


        private string _TextTelefonoEntrevistado;

        public string TextTelefonoEntrevistado
        {
            get { return _TextTelefonoEntrevistado; }
            set
            {
                _TextTelefonoEntrevistado = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;
                OnPropertyChanged("TextTelefonoEntrevistado");
            }
        }


        private string _TextParentescoEntrevistado;

        public string TextParentescoEntrevistado
        {
            get { return _TextParentescoEntrevistado; }
            set { _TextParentescoEntrevistado = value; OnPropertyChanged("TextParentescoEntrevistado"); }
        }


        private string _TextTiempoConocerceEntrvistado;

        public string TextTiempoConocerceEntrvistado
        {
            get { return _TextTiempoConocerceEntrvistado; }
            set { _TextTiempoConocerceEntrvistado = value; OnPropertyChanged("TextTiempoConocerceEntrvistado"); }
        }

        private string _TextRelacionSentenciadoEntrevistado;

        public string TextRelacionSentenciadoEntrevistado
        {
            get { return _TextRelacionSentenciadoEntrevistado; }
            set { _TextRelacionSentenciadoEntrevistado = value; OnPropertyChanged("TextRelacionSentenciadoEntrevistado"); }
        }


        private string _TextObservaciones;

        public string TextObservaciones
        {
            get { return _TextObservaciones; }
            set {
                if (value!=null)
                {
                    value = value.Trim();     
                }
                _TextObservaciones = value; 
                
                OnPropertyChanged("TextObservaciones"); }
        }


        private ObservableCollection<TIPO_REFERENCIA> _ListParentesco;

        public ObservableCollection<TIPO_REFERENCIA> ListParentesco
        {
            get { return _ListParentesco; }
            set { _ListParentesco = value; OnPropertyChanged("ListParentesco"); }
        }

        private TIPO_REFERENCIA _SelectedParentesco;

        public TIPO_REFERENCIA SelectedParentesco
        {
            get { return _SelectedParentesco; }
            set { _SelectedParentesco = value; OnPropertyChanged("SelectedParentesco"); }
        }

        private short? _SelectParentesco = -1;

        public short? SelectParentesco
        {
            get { return _SelectParentesco; }
            set { _SelectParentesco = value; OnPropertyChanged("SelectParentesco"); }
        }



        private ObservableCollection<PAIS_NACIONALIDAD> _ListPaisNacimientoEntrv;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisNacimientoEntrv
        {
            get { return _ListPaisNacimientoEntrv; }
            set { _ListPaisNacimientoEntrv = value; OnPropertyChanged("ListPaisNacimientoEntrv"); }
        }

        private ObservableCollection<ENTIDAD> _ListEntidadNacimientoEntrv;
        public ObservableCollection<ENTIDAD> ListEntidadNacimientoEntrv
        {
            get { return _ListEntidadNacimientoEntrv; }
            set { _ListEntidadNacimientoEntrv = value; OnPropertyChanged("ListEntidadNacimientoEntrv"); }
        }

        private ObservableCollection<MUNICIPIO> _ListMunicipioNacimientoEntrv;
        public ObservableCollection<MUNICIPIO> ListMunicipioNacimientoEntrv
        {
            get { return _ListMunicipioNacimientoEntrv; }
            set { _ListMunicipioNacimientoEntrv = value; OnPropertyChanged("ListMunicipioNacimientoEntrv"); }
        }

        private PAIS_NACIONALIDAD _SelectPaisNacimientoEntrv;
        public PAIS_NACIONALIDAD SelectPaisNacimientoEntrv
        {
            get { return _SelectPaisNacimientoEntrv; }
            set
            {
                _SelectPaisNacimientoEntrv = value;
                ListEntidadNacimientoEntrv = value != null ? new ObservableCollection<ENTIDAD>(value.ENTIDAD) : new ObservableCollection<ENTIDAD>();
                ListEntidadNacimientoEntrv.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                SelectEntidadNacEntrv = -1;
                OnPropertyChanged("SelectPaisNacimientoEntrv");
            }
        }

        private ENTIDAD _SelectedEntidadNacimientoEntrv;
        public ENTIDAD SelectedEntidadNacimientoEntrv
        {
            get { return _SelectedEntidadNacimientoEntrv; }
            set
            {
                _SelectedEntidadNacimientoEntrv = value;
                ListMunicipioNacimientoEntrv = value != null ? new ObservableCollection<MUNICIPIO>(value.MUNICIPIO) : new ObservableCollection<MUNICIPIO>();
                ListMunicipioNacimientoEntrv.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                SelectMunicipioNacEntrv = -1;
                OnPropertyChanged("SelectedEntidadNacimientoEntrv");
            }
        }

        private MUNICIPIO _MunicipioNacimientoEnabledEntrv;
        public MUNICIPIO MunicipioNacimientoEnabledEntrv
        {
            get { return _MunicipioNacimientoEnabledEntrv; }
            set
            {
                _MunicipioNacimientoEnabledEntrv = value;
                //ListColonia = value != null ? new ObservableCollection<COLONIA>(value.COLONIA) : new ObservableCollection<COLONIA>();
                //ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                //SelectColonia = -1;
                OnPropertyChanged("MunicipioNacimientoEnabledEntrv");
            }
        }

        private short? selectMunicipioNacEntrv = Parametro.PAIS;//82;
        public short? SelectMunicipioNacEntrv
        {
            get { return selectMunicipioNacEntrv; }
            set
            {
                selectMunicipioNacEntrv = value;
                OnPropertyValidateChanged("SelectMunicipioNacEntrv");
            }
        }

        private short? selectEntidadNacEntrv = Parametro.PAIS;//82;
        public short? SelectEntidadNacEntrv
        {
            get { return selectEntidadNacEntrv; }
            set
            {
                selectEntidadNacEntrv = value;
                OnPropertyValidateChanged("SelectEntidadNacEntrv");
            }
        }



        private short? selectPaisNacEntrv = Parametro.PAIS;//82;
        public short? SelectPaisNacEntrv
        {
            get { return selectPaisNacEntrv; }
            set
            {
                selectPaisNacEntrv = value;
                OnPropertyValidateChanged("SelectPaisNacEntrv");
            }
        }

        #endregion

        #region Anexos
        private string _TextNombreCroquis;

        public string TextNombreCroquis
        {
            get { return _TextNombreCroquis; }
            set { _TextNombreCroquis = value; OnPropertyChanged("TextNombreCroquis"); }
        }

        private string _TextTelCroquis;

        public string TextTelCroquis
        {
            get { return _TextTelCroquis; }
            set {

                _TextTelCroquis = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;
                OnPropertyChanged("TextTelCroquis");
            }
        }


        private string _TextDireccionCroquis;

        public string TextDireccionCroquis
        {
            get { return _TextDireccionCroquis; }
            set { _TextDireccionCroquis = value; OnPropertyChanged("TextDireccionCroquis"); }
        }

        private byte[] _ImagenSuperiorIzquierda;

        public byte[] ImagenSuperiorIzquierda
        {
            get { return _ImagenSuperiorIzquierda; }
            set { _ImagenSuperiorIzquierda = value; OnPropertyChanged("ImagenSuperiorIzquierda"); }
        }

        private byte[] _ImagenSuperiorDerecha;

        public byte[] ImagenSuperiorDerecha
        {
            get { return _ImagenSuperiorDerecha; }
            set { _ImagenSuperiorDerecha = value; OnPropertyChanged("ImagenSuperiorDerecha"); }
        }

        private byte[] _ImagenInferiorIzquierda;

        public byte[] ImagenInferiorIzquierda
        {
            get { return _ImagenInferiorIzquierda; }
            set { _ImagenInferiorIzquierda = value; OnPropertyChanged("ImagenInferiorIzquierda"); }
        }

        private byte[] _ImagenInferiorDerecha;

        public byte[] ImagenInferiorDerecha
        {
            get { return _ImagenInferiorDerecha; }
            set { _ImagenInferiorDerecha = value; OnPropertyChanged("ImagenInferiorDerecha"); }
        }


        #endregion
        
        private string _ApellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return _ApellidoPaternoBuscar; }
            set { _ApellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
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

        private short? anioBuscar;
        public short? AnioBuscar
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

        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set
            {
                imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
            }
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

        private LIBERADO_MEDIDA_JUDICIAL selectMJ;
        public LIBERADO_MEDIDA_JUDICIAL SelectMJ
        {
            get { return selectMJ; }
            set
            {
                selectMJ = value;
              //  SelectMJEnabled = value != null ? true : false;
                OnPropertyChanged("SelectMJ");
            }
        }

        private bool selectMJEnabled = false;
        public bool SelectMJEnabled
        {
            get { return selectMJEnabled; }
            set { selectMJEnabled = value; OnPropertyChanged("SelectMJEnabled"); }
        }

        private bool _TabsEnabled;
        public bool TabsEnabled
        {
            get { return _TabsEnabled; }
            set { _TabsEnabled = value; OnPropertyChanged("TabsEnabled"); }
        }

        private System.Windows.Visibility _TabControlVisible = Visibility.Collapsed;
        public Visibility TabControlVisible
        {
            get { return _TabControlVisible; }
            set { _TabControlVisible = value; OnPropertyChanged("TabControlVisible"); }
        }

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }
        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }

        private bool SeguirCargando { get; set; }

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
                //if (value)
                // MenuBuscarEnabled = value;
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

        private byte[] imagenInterno = new Imagenes().getImagenPerson();
        public byte[] ImagenInterno
        {
            get { return imagenInterno; }
            set { imagenInterno = value; OnPropertyChanged("ImagenInterno"); }
        }

        #region MotivosVisitaDomiciliaria
        private string _TextMotivoVisita;

        public string TextMotivoVisita
        {
            get { return _TextMotivoVisita; }
            set {
                if (value!=null)
                {
                    value = value.Trim();
                }
                _TextMotivoVisita = value; 
                
                OnPropertyChanged("TextMotivoVisita"); }
        }
        #endregion

        #region Fotografias
        private ObservableCollection<PRS_VISITA_FOTOGRAFIA> _ListasFotografias;
        public ObservableCollection<PRS_VISITA_FOTOGRAFIA> ListasFotografias
        {
            get { return _ListasFotografias; }
            set { _ListasFotografias = value; OnPropertyChanged("ListasFotografias"); }
        }
        #endregion

        #region Liberados
        private string _NUCBuscar;
        public string NUCBuscar
        {
            get { return _NUCBuscar; }
            set { _NUCBuscar = value; OnPropertyChanged("NUCBuscar"); }
        }

        private RangeEnabledObservableCollection<cLiberados> lstLiberados;
        public RangeEnabledObservableCollection<cLiberados> LstLiberados
        {
            get { return lstLiberados; }
            set { lstLiberados = value; OnPropertyChanged("LstLiberados"); }
        }

        private cLiberados selectedLiberado;
        public cLiberados SelectedLiberado
        {
            get { return selectedLiberado; }
            set
            {
                selectedLiberado = value;
                if (value != null)
                {
                    SelectExpediente = new cImputado().Obtener(value.ID_IMPUTADO, value.ID_ANIO, value.ID_CENTRO).FirstOrDefault();
                }
                else
                {
                    SelectExpediente = null;
                }
                OnPropertyChanged("SelectedLiberado");
            }
        }
        #endregion

        #region Huellas
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

        #region Filtros
        private bool porNUC = true;
        public bool PorNUC
        {
            get { return porNUC; }
            set
            {
                porNUC = value;
                if (value)
                {
                    PorCP = false;
                    PorNUCVisible = Visibility.Visible;
                    PorCPVisible = Visibility.Collapsed;
                    AnioBuscar = null;
                    FolioBuscar = null;
                }
                else
                {
                    PorCP = true;
                    PorNUCVisible = Visibility.Collapsed;
                    PorCPVisible = Visibility.Visible;
                    NUCBuscar = string.Empty;
                }
                OnPropertyChanged("PorNUC");
            }
        }

        private bool porCP = false;
        public bool PorCP
        {
            get { return porCP; }
            set { porCP = value; OnPropertyChanged("PorCP"); }
        }

        private Visibility porNUCVisible = Visibility.Visible;
        public Visibility PorNUCVisible
        {
            get { return porNUCVisible; }
            set { porNUCVisible = value; OnPropertyChanged("PorNUCVisible"); }
        }

        private Visibility porCPVisible = Visibility.Collapsed;
        public Visibility PorCPVisible
        {
            get { return porCPVisible; }
            set { porCPVisible = value; OnPropertyChanged("PorCPVisible"); }
        }
        #endregion
    }
}
