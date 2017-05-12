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
    partial class TrabajoSocialViewModel
    {

        #region MaxWidthViews
        private double _WidhtTotalTrabajoSocialView;

        public double WidhtTotalTrabajoSocialView
        {
            get { return _WidhtTotalTrabajoSocialView; }
            set { _WidhtTotalTrabajoSocialView = value; OnPropertyChanged("WidhtTotalTrabajoSocialView"); }
        }

        private string _TextDomicilioReferencia;

        public string TextDomicilioReferencia
        {
            get { return _TextDomicilioReferencia; }
            set { _TextDomicilioReferencia = value; OnPropertyChanged("TextDomicilioReferencia"); }
        }

        private double _WidhtEntrevistainicial;
        public double WidhtEntrevistainicial
        {
            get { return _WidhtEntrevistainicial; }
            set { _WidhtEntrevistainicial = value; OnPropertyChanged("WidhtEntrevistainicial"); }
        }

        private double _WidhtEstudioSocioEconomico;
        public double WidhtEstudioSocioEconomico
        {
            get { return _WidhtEstudioSocioEconomico; }
            set { _WidhtEstudioSocioEconomico = value; OnPropertyChanged("WidhtEstudioSocioEconomico"); }
        }

        private double _WidhtEstructuraDinamicaFamiliar;
        public double WidhtEstructuraDinamicaFamiliar
        {
            get { return _WidhtEstructuraDinamicaFamiliar; }
            set { _WidhtEstructuraDinamicaFamiliar = value; OnPropertyChanged("WidhtEstructuraDinamicaFamiliar"); }
        }

        private double _WidhtSituacionActual;
        public double WidhtSituacionActual
        {
            get { return _WidhtSituacionActual; }
            set { _WidhtSituacionActual = value; OnPropertyChanged("WidhtSituacionActual"); }
        }
        #endregion

        #region Maxlenght


        private decimal _LugarMax;

        public decimal LugarMax
        {
            get { return _LugarMax; }
            set { _LugarMax = value; OnPropertyChanged("LugarMax"); }
        }
        private decimal _DelitoImputaMax;

        public decimal DelitoImputaMax
        {
            get { return _DelitoImputaMax; }
            set { _DelitoImputaMax = value; OnPropertyChanged("DelitoImputaMax"); }
        }

        private decimal _NombreApoyoMax;

        public decimal NombreApoyoMax
        {
            get { return _NombreApoyoMax; }
            set { _NombreApoyoMax = value; OnPropertyChanged("NombreApoyoMax"); }
        }
        private decimal _CalleApoyoMax;

        public decimal CalleApoyoMax
        {
            get { return _CalleApoyoMax; }
            set { _CalleApoyoMax = value; OnPropertyChanged("CalleApoyoMax"); }
        }

        private decimal _NucMax;

        public decimal NucMax
        {
            get { return _NucMax; }
            set { _NucMax = value; OnPropertyChanged("NucMax"); }
        }
        private decimal _CausaPenalMax;

        public decimal CausaPenalMax
        {
            get { return _CausaPenalMax; }
            set { _CausaPenalMax = value; OnPropertyChanged("CausaPenalMax"); }
        }

        private decimal _ObservacioneEstructuraFamMax;

        public decimal ObservacioneEstructuraFamMax
        {
            get { return _ObservacioneEstructuraFamMax; }
            set { _ObservacioneEstructuraFamMax = value; OnPropertyChanged("ObservacioneEstructuraFamMax"); }
        }

        private decimal _CondicionesZonaMax;


        public decimal CondicionesZonaMax
        {
            get { return _CondicionesZonaMax; }
            set { _CondicionesZonaMax = value; OnPropertyChanged("CondicionesZonaMax"); }
        }

        private decimal _SituacionEconMax;

        public decimal SituacionEconMax
        {
            get { return _SituacionEconMax; }
            set { _SituacionEconMax = value; OnPropertyChanged("SituacionEconMax"); }
        }

        private decimal _DesdeCuandoVivenJuntosMax;

        public decimal DesdeCuandoVivenJuntosMax
        {
            get { return _DesdeCuandoVivenJuntosMax; }
            set { _DesdeCuandoVivenJuntosMax = value; OnPropertyChanged("DesdeCuandoVivenJuntosMax"); }
        }

        private decimal _TiempoLibreMax;

        public decimal TiempoLibreMax
        {
            get { return _TiempoLibreMax; }
            set { _TiempoLibreMax = value; OnPropertyChanged("TiempoLibreMax"); }
        }
        private decimal _OtroTiempoLibrMax;

        public decimal OtroTiempoLibrMax
        {
            get { return _OtroTiempoLibrMax; }
            set { _OtroTiempoLibrMax = value; OnPropertyChanged("OtroTiempoLibrMax"); }
        }
        private decimal _EpecifiqueEnfermedadMax;

        public decimal EpecifiqueEnfermedadMax
        {
            get { return _EpecifiqueEnfermedadMax; }
            set { _EpecifiqueEnfermedadMax = value; OnPropertyChanged("EpecifiqueEnfermedadMax"); }
        }

        private decimal _TipoTratamientoRecibidoMax;

        public decimal TipoTratamientoRecibidoMax
        {
            get { return _TipoTratamientoRecibidoMax; }
            set { _TipoTratamientoRecibidoMax = value; OnPropertyChanged("TipoTratamientoRecibidoMax"); }
        }
        private decimal _DiagnosticoMax;

        public decimal DiagnosticoMax
        {
            get { return _DiagnosticoMax; }
            set { _DiagnosticoMax = value; OnPropertyChanged("DiagnosticoMax"); }
        }

        private decimal _OtroDocumMax;

        public decimal OtroDocumMax
        {
            get { return _OtroDocumMax; }
            set { _OtroDocumMax = value; OnPropertyChanged("OtroDocumMax"); }
        }
        private decimal _DescrDinamicaFamiliarMax;

        public decimal DescrDinamicaFamiliarMax
        {
            get { return _DescrDinamicaFamiliarMax; }
            set { _DescrDinamicaFamiliarMax = value; OnPropertyChanged("DescrDinamicaFamiliarMax"); }
        }

        private decimal _FomraPorqueApoyoProcesoJudMax;


        public decimal FomraPorqueApoyoProcesoJudMax
        {
            get { return _FomraPorqueApoyoProcesoJudMax; }
            set { _FomraPorqueApoyoProcesoJudMax = value; OnPropertyChanged("FomraPorqueApoyoProcesoJudMax"); }
        }
        private decimal _DeQuienDuranteInternamientoMax;

        public decimal DeQuienDuranteInternamientoMax
        {
            get { return _DeQuienDuranteInternamientoMax; }
            set { _DeQuienDuranteInternamientoMax = value; OnPropertyChanged("DeQuienDuranteInternamientoMax"); }
        }

        private decimal _FrecuenciaRecibApoyoInternmMax;

        public decimal FrecuenciaRecibApoyoInternmMax
        {
            get { return _FrecuenciaRecibApoyoInternmMax; }
            set { _FrecuenciaRecibApoyoInternmMax = value; OnPropertyChanged("FrecuenciaRecibApoyoInternmMax"); }
        }
        private decimal _TipoViviendaMax;

        public decimal TipoViviendaMax
        {
            get { return _TipoViviendaMax; }
            set { _TipoViviendaMax = value; OnPropertyChanged("TipoViviendaMax"); }
        }
        private decimal _TiempoConocerleMax;

        public decimal TiempoConocerleMax
        {
            get { return _TiempoConocerleMax; }
            set { _TiempoConocerleMax = value; OnPropertyChanged("TiempoConocerleMax"); }
        }

        private decimal _ViviaAntesMax;

        public decimal ViviaAntesMax
        {
            get { return _ViviaAntesMax; }
            set { _ViviaAntesMax = value; OnPropertyChanged("ViviaAntesMax"); }
        }
        private decimal _OtrosPersonaViviaAntesMax;

        public decimal OtrosPersonaViviaAntesMax
        {
            get { return _OtrosPersonaViviaAntesMax; }
            set { _OtrosPersonaViviaAntesMax = value; OnPropertyChanged("OtrosPersonaViviaAntesMax"); }
        }

        private decimal _TipoMatViviendaMax;

        public decimal TipoMatViviendaMax
        {
            get { return _TipoMatViviendaMax; }
            set { _TipoMatViviendaMax = value; OnPropertyChanged("TipoMatViviendaMax"); }
        }
        private decimal _TipoMatViviendaOtroMax;

        public decimal TipoMatViviendaOtroMax
        {
            get { return _TipoMatViviendaOtroMax; }
            set { _TipoMatViviendaOtroMax = value; OnPropertyChanged("TipoMatViviendaOtroMax"); }
        }

        #endregion

        #region EntrevistaInical

        private string _TextCausaPenalEntrevista;

        public string TextCausaPenalEntrevista
        {
            get { return _TextCausaPenalEntrevista; }
            set { _TextCausaPenalEntrevista = value.Trim(); OnPropertyChanged("TextCausaPenalEntrevista"); }
        }

        private DateTime? _TextFechaEntrv;

        public DateTime? TextFechaEntrv
        {
            get { return _TextFechaEntrv; }
            set { _TextFechaEntrv = value; OnPropertyChanged("TextFechaEntrv"); }
        }

        private string _TextNucEntrevista;

        public string TextNucEntrevista
        {
            get { return _TextNucEntrevista; }
            set { _TextNucEntrevista = value.Trim(); OnPropertyChanged("TextNucEntrevista"); }
        }

        private string _TextLugarEntrevista;

        public string TextLugarEntrevista
        {
            get { return _TextLugarEntrevista; }
            set { _TextLugarEntrevista = value.Trim(); OnPropertyChanged("TextLugarEntrevista"); }
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

        #region TabsSeleccion
        private bool _EntroDatosGenerales;

        public bool EntroDatosGenerales
        {
            get { return _EntroDatosGenerales; }
            set { _EntroDatosGenerales = value; OnPropertyChanged("EntroDatosGenerales"); }
        }
        private bool _EntroEstudioSocioEconomico;

        public bool EntroEstudioSocioEconomico
        {
            get { return _EntroEstudioSocioEconomico; }
            set { _EntroEstudioSocioEconomico = value; OnPropertyChanged("EntroEstudioSocioEconomico"); }
        }
        private bool _EntroEstructuraDinamicaFamiliar;

        public bool EntroEstructuraDinamicaFamiliar
        {
            get { return _EntroEstructuraDinamicaFamiliar; }
            set { _EntroEstructuraDinamicaFamiliar = value; OnPropertyChanged("EntroEstructuraDinamicaFamiliar"); }
        }

        private bool _EntroSituacionActual;

        public bool EntroSituacionActual
        {
            get { return _EntroSituacionActual; }
            set { _EntroSituacionActual = value; OnPropertyChanged("EntroSituacionActual"); }
        }

        private bool _EntroValidacionApoyoEconomicoFamiliar;

        public bool EntroValidacionApoyoEconomicoFamiliar
        {
            get { return _EntroValidacionApoyoEconomicoFamiliar; }
            set { _EntroValidacionApoyoEconomicoFamiliar = value; OnPropertyChanged("EntroValidacionApoyoEconomicoFamiliar"); }
        }

        private bool _EntroValidacionDrogafrecuencia;

        public bool EntroValidacionDrogafrecuencia
        {
            get { return _EntroValidacionDrogafrecuencia; }
            set { _EntroValidacionDrogafrecuencia = value; OnPropertyChanged("EntroValidacionDrogafrecuencia"); }
        }

        private bool _EntroValidarNuceloFamiliar;

        public bool EntroValidarNuceloFamiliar
        {
            get { return _EntroValidarNuceloFamiliar; }
            set { _EntroValidarNuceloFamiliar = value; OnPropertyChanged("EntroValidarNuceloFamiliar"); }
        }


        private bool _TabDatosGenerales;

        public bool TabDatosGenerales
        {
            get { return _TabDatosGenerales; }
            set
            {
                if (EntroDatosGenerales==false)
                {
                    EntroDatosGenerales = true;
                    ValidacionesDatosTrabajoSocial();
                }
                
                _TabDatosGenerales = value;

                OnPropertyChanged("TabDatosGenerales");
            }
        }
        private bool _TabEstudioSocioEconomico;

        public bool TabEstudioSocioEconomico
        {
            get { return _TabEstudioSocioEconomico; }
            set
            {
                if (EntroEstudioSocioEconomico==false)
                {
                    EntroEstudioSocioEconomico = true;
                    ValidacionEstudioSocioEconomico();
                }
                
                _TabEstudioSocioEconomico = value;

                OnPropertyChanged("TabEstudioSocioEconomico");
            }
        }

        private bool _TabEstructuraDinamicaFamiliar;

        public bool TabEstructuraDinamicaFamiliar
        {
            get { return _TabEstructuraDinamicaFamiliar; }
            set
            {
                if (EntroEstructuraDinamicaFamiliar==false)
                {
                    EntroEstructuraDinamicaFamiliar = true;
                    ValidacionEstructuraDinamica();
                }
                
                _TabEstructuraDinamicaFamiliar = value;

                OnPropertyChanged("TabEstructuraDinamicaFamiliar");
            }
        }

        private bool _TabSituacionActual;

        public bool TabSituacionActual
        {
            get { return _TabSituacionActual; }
            set
            {
                if (EntroSituacionActual==false)
                {
                    ValidacionSituacionActual();
                    EntroSituacionActual = true;
                }
                
                _TabSituacionActual = value;
                OnPropertyChanged("TabSituacionActual");
            }
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
        private string tituloTop = "Entrevista Inicial de Trabajo Social";
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


        private bool _IdentificacionDatosGeneralesEnabled;

        public bool IdentificacionDatosGeneralesEnabled
        {
            get { return _IdentificacionDatosGeneralesEnabled; }
            set { _IdentificacionDatosGeneralesEnabled = value; OnPropertyChanged("IdentificacionDatosGeneralesEnabled"); }
        }

        private ESTADO_CIVIL _SelectedEdoCivilGrupoFamiliar;

        public ESTADO_CIVIL SelectedEdoCivilGrupoFamiliar
        {
            get { return _SelectedEdoCivilGrupoFamiliar; }
            set { _SelectedEdoCivilGrupoFamiliar = value; OnPropertyChanged("SelectedEdoCivilGrupoFamiliar"); }

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
               // if (value == selectEscolaridad)
                 //   return;
                if (value==null)
                {
                    selectEscolaridad = -1;
                }
                else
                {
                    selectEscolaridad = value;        
                }
                

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
            set {
                if (value == null)
                {
                    _SelectOcupacion = -1;
                }
                else
                {
                    _SelectOcupacion = value;
                }



                    OnPropertyChanged("SelectOcupacion");
            }
        }


        private ObservableCollection<ALIAS> listAlias;
        public ObservableCollection<ALIAS> ListAlias
        {
            get { return listAlias; }
            set { listAlias = value; OnPropertyValidateChanged("ListAlias"); }
        }

        private ObservableCollection<APODO> listApodo;
        public ObservableCollection<APODO> ListApodo
        {
            get { return listApodo; }
            set { listApodo = value; OnPropertyValidateChanged("ListApodo"); }
        }

        private System.Windows.Visibility relacionesPersonalesVisible = Visibility.Collapsed;
        public Visibility RelacionesPersonalesVisible
        {
            get { return relacionesPersonalesVisible; }
            set { relacionesPersonalesVisible = value; OnPropertyChanged("RelacionesPersonalesVisible"); }
        }

        private System.Windows.Visibility _TabControlVisible = Visibility.Collapsed;
        public Visibility TabControlVisible
        {
            get { return _TabControlVisible; }
            set { _TabControlVisible = value; OnPropertyChanged("TabControlVisible"); }
        }

        private string _TextDelitoImputa;
        public string TextDelitoImputa
        {
            get { return _TextDelitoImputa; }
            set { 
                if (!string.IsNullOrEmpty(value))
                    _TextDelitoImputa = value.Trim();
                else
                    _TextDelitoImputa = value;
                OnPropertyChanged("TextDelitoImputa"); }
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

        private string _TextAlias;

        public string TextAlias
        {
            get { return _TextAlias; }
            set { _TextAlias = value; OnPropertyChanged("TextAlias"); }
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
                
                    if (value==null)
                    {
                        selectReligion = -1;
                    }
                    else
                    {
                        selectReligion = value;
                    }
                OnPropertyValidateChanged("SelectReligion");
            }
        }







        private ObservableCollection<IDIOMA> _LstIdioma;


        public ObservableCollection<IDIOMA> LstIdioma
        {
            get { return _LstIdioma; }
            set { _LstIdioma = value; OnPropertyChanged("LstIdioma"); }
        }

        private short? _SelectedIdioma = -1;

        public short? SelectedIdioma
        {
            get { return _SelectedIdioma; }
            set { 
            if (value==null)
            {
                _SelectedIdioma = -1;
            }
            else
            {
                _SelectedIdioma = value;
            }
            OnPropertyChanged("SelectedIdioma");
            }
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


        private int? _TextNumeroInterior;

        public int? TextNumeroInterior
        {
            get { return _TextNumeroInterior; }
            set { _TextNumeroInterior = value; OnPropertyChanged("TextNumeroInterior"); }
        }

        private int? _TextNumeroExterior;

        public int? TextNumeroExterior
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
        private ObservableCollection<ETNIA> _LstGrupoEtnico;

        public ObservableCollection<ETNIA> LstGrupoEtnico
        {
            get { return _LstGrupoEtnico; }
            set { _LstGrupoEtnico = value; OnPropertyChanged("LstGrupoEtnico"); }
        }

        private short? _SelectGrupoEtnico=-1;

        public short? SelectGrupoEtnico
        {
            get { return _SelectGrupoEtnico; }
            set {

                if (value==null)
                {
                    _SelectGrupoEtnico = -1;
                }
                else
                {
                    _SelectGrupoEtnico = value; 
                }
                OnPropertyChanged("SelectGrupoEtnico"); }
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

        private PRS_ENTREVISTA_INICIAL SelectedEntrevistaInicial;

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




        //private IMPUTADO selectExpediente;
        //public IMPUTADO SelectExpediente
        //{
        //    get { return selectExpediente; }
        //    set
        //    {
                
                
                
        //        selectExpediente = value;
        //         SelectMJEnabled = value != null ? true : false;
        //        if (value != null)
        //        {
        //            var foto = value.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
        //            if (foto != null)
        //                ImagenInterno = foto.BIOMETRICO;
        //            else
        //            {
        //                if (value.INGRESO != null)
        //                {
        //                    var ingreso = value.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
        //                    if (ingreso != null)
        //                    {
        //                        var fotoIngreso = ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
        //                        if (fotoIngreso != null)
        //                            ImagenInterno = fotoIngreso.BIOMETRICO;
        //                        else
        //                            ImagenInterno = new Imagenes().getImagenPerson();
        //                    }
        //                }
        //            }
        //            var l = new cLiberado().Obtener(value.ID_CENTRO, value.ID_ANIO, value.ID_IMPUTADO);
        //            if (l != null)
        //            {
        //                LstLiberadoMJ = new ObservableCollection<LIBERADO_MEDIDA_JUDICIAL>(l.LIBERADO_MEDIDA_JUDICIAL);
        //            }
        //            else
        //                LstLiberadoMJ = null;
        //        }
        //        else
        //        {
        //            LstLiberadoMJ = null;
        //            ImagenInterno = new Imagenes().getImagenPerson();
        //        }
        //        SelectMJ = null;
        //        EmptyMJVisible = LstLiberadoMJ != null ? LstLiberadoMJ.Count > 0 ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
        //        OnPropertyChanged("SelectExpediente");
        //    }
        //}


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


      

        private string _TituloModalDrogaConsumo = "Droga Consumo";

        public string TituloModalDrogaConsumo
        {
            get { return _TituloModalDrogaConsumo; }
            set { _TituloModalDrogaConsumo = value; OnPropertyChanged("TituloModalDrogaConsumo"); }
        }

        private string tituloAlias;
        public string TituloAlias
        {
            get { return tituloAlias; }
            set { tituloAlias = value; OnPropertyChanged("TituloAlias"); }
        }

        private string tituloApodo;
        public string TituloApodo
        {
            get { return tituloApodo; }
            set { tituloApodo = value; OnPropertyChanged("TituloApodo"); }
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

        #endregion

        #region PersonasApoyo
        private string _TextNombreApoyo;

        public string TextNombreApoyo
        {
            get { return _TextNombreApoyo; }
            set { _TextNombreApoyo = value.Trim(); OnPropertyChanged("TextNombreApoyo"); }
        }
        private string _TituloApoyoEconomico="Apoyo Economico";

        public string TituloApoyoEconomico
        {
            get { return _TituloApoyoEconomico; }
            set { _TituloApoyoEconomico = value; OnPropertyChanged("TituloApoyoEconomico"); }
        }

        private ObservableCollection<OCUPACION> _ListOcupacionApoyo;

        public ObservableCollection<OCUPACION> ListOcupacionApoyo
        {
            get { return _ListOcupacionApoyo; }
            set { _ListOcupacion = value; OnPropertyChanged("ListOcupacionApoyo"); }
        }

        private ObservableCollection<OCUPACION> _ListOcupacionesApoyo;


        public ObservableCollection<OCUPACION> ListOcupacionesApoyo
        {
            get { return _ListOcupacionesApoyo; }
            set { _ListOcupacionesApoyo = value; OnPropertyChanged("ListOcupacionesApoyo"); }
        }


        //private short? _SelectOcupacionesApoyo = -1;

        //public short? SelectOcupacionesApoyo
        //{
        //    get { return _SelectOcupacionesApoyo; }
        //    set {
        //        if (value==null)
        //        {
        //            _SelectOcupacionesApoyo = -1;
        //        }
        //        else
        //        {
        //            _SelectOcupacionesApoyo = value; 
        //        }
        //        OnPropertyChanged("SelectOcupacionesApoyo"); }
        //}



        private short? _SelectOcupacionApoyo = -1;

        public short? SelectOcupacionApoyo
        {
            get { return _SelectOcupacionApoyo; }
            set { _SelectOcupacionApoyo = value; OnPropertyChanged("SelectOcupacionApoyo"); }
        }


        private string _TextCalleApoyo;

        public string TextCalleApoyo
        {
            get { return _TextCalleApoyo; }
            set { _TextCalleApoyo = value.Trim(); OnPropertyChanged("TextCalleApoyo"); }
        }

        private int? _TextNumeroExteriorApoyo;
        public int? TextNumeroExteriorApoyo
        {
            get { return _TextNumeroExteriorApoyo; }
            set
            {
                _TextNumeroExteriorApoyo = value;
                OnPropertyValidateChanged("TextNumeroExteriorApoyo");
            }
        }

        private int? _TextEdadApoyo;

        public int? TextEdadApoyo
        {
            get { return _TextEdadApoyo; }
            set { _TextEdadApoyo = value; OnPropertyChanged("TextEdadApoyo"); }
        }

        private string _TextNumeroInteriorApoyo;
        public  string TextNumeroInteriorApoyo
        {
            get { return _TextNumeroInteriorApoyo; }
            set
            {
                _TextNumeroInteriorApoyo = value;
                OnPropertyValidateChanged("TextNumeroInteriorApoyo");
            }
        }


        private string _TextTelefonoApoyo;
        public string TextTelefonoApoyo
        {
            get { return _TextTelefonoApoyo; }
            set
            {
                _TextTelefonoApoyo = !string.IsNullOrEmpty(value) ? new Converters().MascaraTelefono(value) : value;
                OnPropertyValidateChanged("TextTelefonoApoyo");
            }
        }

        private string _TituloNucleoFamiliar="Nucleo Familiar";

        public string TituloNucleoFamiliar
        {
            get { return _TituloNucleoFamiliar; }
            set { _TituloNucleoFamiliar = value; OnPropertyChanged("TituloNucleoFamiliar"); }
        }

        private ObservableCollection<PRS_NUCLEO_FAMILIAR> _ListNuceloPrimarioFam;

        public ObservableCollection<PRS_NUCLEO_FAMILIAR> ListNuceloPrimarioFam
        {
            get { return _ListNuceloPrimarioFam; }
            set { _ListNuceloPrimarioFam = value; OnPropertyChanged("ListNuceloPrimarioFam"); }
        }

        private PRS_NUCLEO_FAMILIAR _SelectNuceloPrimarioFam;
        public PRS_NUCLEO_FAMILIAR SelectNuceloPrimarioFam
        {
            get { return _SelectNuceloPrimarioFam; }
            set { _SelectNuceloPrimarioFam = value; OnPropertyChanged("SelectNuceloPrimarioFam"); }
        }

        private ObservableCollection<PRS_NUCLEO_FAMILIAR> _ListSecundarioFam;
        public ObservableCollection<PRS_NUCLEO_FAMILIAR> ListSecundarioFam
        {
            get { return _ListSecundarioFam; }
            set { _ListSecundarioFam = value; OnPropertyChanged("ListSecundarioFam"); }
        }

        private PRS_NUCLEO_FAMILIAR _SelectSecundarioFam;
        public PRS_NUCLEO_FAMILIAR SelectSecundarioFam
        {
            get { return _SelectSecundarioFam; }
            set { _SelectSecundarioFam = value; OnPropertyChanged("SelectSecundarioFam"); }
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
            set {
                if (value==null)
                {
                    _SelectParentesco = -1;
                }
                else
                {
                    _SelectParentesco = value;
                }
                OnPropertyChanged("SelectParentesco"); }
        }


        private string _TextTiempoConocerceApoyo;
        public string TextTiempoConocerceApoyo
        {
            get { return _TextTiempoConocerceApoyo; }
            set
            {
                _TextTiempoConocerceApoyo = value.Trim();
                OnPropertyValidateChanged("TextTiempoConocerceApoyo");
            }
        }
        private string _TextOtrasPersonasViviaAntes;

        public string TextOtrasPersonasViviaAntes
        {
            get { return _TextOtrasPersonasViviaAntes; }
            set { _TextOtrasPersonasViviaAntes = value; OnPropertyChanged("TextOtrasPersonasViviaAntes"); }
        }
        private string _TextOtroTipoMaterialVivienda;

        public string TextOtroTipoMaterialVivienda
        {
            get { return _TextOtroTipoMaterialVivienda; }
            set { _TextOtroTipoMaterialVivienda = value; OnPropertyChanged("TextOtroTipoMaterialVivienda"); }
        }

        private string _TextTipoMaterialVivienda;


        public string TextTipoMaterialVivienda
        {
            get { return _TextTipoMaterialVivienda; }
            set { _TextTipoMaterialVivienda = value; OnPropertyChanged("TextTipoMaterialVivienda"); }
        }

        private string _TextTipoMaterialViviendaOtro;

        public string TextTipoMaterialViviendaOtro
        {
            get { return _TextTipoMaterialViviendaOtro; }
            set { _TextTipoMaterialViviendaOtro = value; OnPropertyChanged("TextTipoMaterialViviendaOtro"); }
        }

        #region Estructura de la Vivivenda


        private short? _TextComedorNum=null;

        public short? TextComedorNum
        {
            get { return _TextComedorNum; }
            set { _TextComedorNum = value; OnPropertyChanged("TextComedorNum"); }
        }

        private string _TextComedorObserv;

        public string TextComedorObserv
        {
            get { return _TextComedorObserv; }
            set { _TextComedorObserv = value; OnPropertyChanged("TextComedorObserv"); }
        }


        private short? _TextRecamaraNum=null;

        public short? TextRecamaraNum  
        {
            get { return _TextRecamaraNum; }
            set { _TextRecamaraNum = value; OnPropertyChanged("TextRecamaraNum"); }
        }

        private string _TextRecamaraObserv;

        public string TextRecamaraObserv
        {
            get { return _TextRecamaraObserv; }
            set { _TextRecamaraObserv = value; OnPropertyChanged("TextRecamaraObserv"); }
        }
        private short? _TextSalaNum=null;

        public short? TextSalaNum
        {
            get { return _TextSalaNum; }
            set { _TextSalaNum = value; OnPropertyChanged("TextSalaNum"); }
        }
        private string _TextSalaObserv;

        public string TextSalaObserv
        {
            get { return _TextSalaObserv; }
            set { _TextSalaObserv = value; OnPropertyChanged("TextSalaObserv"); }
        }

        private short? _TextCocinaNum=null;

        public short? TextCocinaNum
        {
            get { return _TextCocinaNum; }
            set { _TextCocinaNum = value; OnPropertyChanged("TextCocinaNum"); }
        }
        private string _TextCocinaObserv;

        public string TextCocinaObserv
        {
            get { return _TextCocinaObserv; }
            set { _TextCocinaObserv = value; OnPropertyChanged("TextCocinaObserv"); }
        }

        private short? _TextBañoNum=null;

        public short? TextBañoNum
        {
            get { return _TextBañoNum; }
            set { _TextBañoNum = value; OnPropertyChanged("TextBañoNum"); }
        }

        private string _TextBañoObserv;

        public string TextBañoObserv
        {
            get { return _TextBañoObserv; }
            set { _TextBañoObserv = value; OnPropertyChanged("TextBañoObserv"); }
        }

        private short? _TextVentanasNum=null;

        public short? TextVentanasNum
        {
            get { return _TextVentanasNum; }
            set { _TextVentanasNum = value; OnPropertyChanged("TextVentanasNum"); }
        }

        private string _TextVentanasObserv;

        public string TextVentanasObserv
        {
            get { return _TextVentanasObserv; }
            set { _TextVentanasObserv = value; OnPropertyChanged("TextVentanasObserv"); }
        }

        private short? _TextPatioNum=null;

        public short? TextPatioNum
        {
            get { return _TextPatioNum; }
            set { _TextPatioNum = value; OnPropertyChanged("TextPatioNum"); }
        }

        private string _TextPatioObserv;

        public string TextPatioObserv
        {
            get { return _TextPatioObserv; }
            set { _TextPatioObserv = value; OnPropertyChanged("TextPatioObserv"); }
        }


        #endregion

        #endregion

        #region EstudioSocioEconomico
        private string _TextTipoVivienda;

        public string TextTipoVivienda
        {
            get { return _TextTipoVivienda; }
            set { _TextTipoVivienda = value; OnPropertyChanged("TextTipoVivienda"); }
        }

        private int? _TextNoPersonasResiden;

        public int? TextNoPersonasResiden
        {
            get { return _TextNoPersonasResiden; }
            set { _TextNoPersonasResiden = value; OnPropertyChanged("TextNoPersonasResiden"); }
        }

        private string _TextPersonaViviaAntes;

        public string TextPersonaViviaAntes
        {
            get { return _TextPersonaViviaAntes; }
            set { _TextPersonaViviaAntes = value; OnPropertyChanged("TextPersonaViviaAntes"); }
        }





        private string _TextCondicionesZona;

        public string TextCondicionesZona
        {
            get { return _TextCondicionesZona; }
            set { _TextCondicionesZona = value; OnPropertyChanged("TextCondicionesZona"); }
        }

        private string _TextSituacionEconomica;

        public string TextSituacionEconomica
        {
            get { return _TextSituacionEconomica; }
            set { _TextSituacionEconomica = value; OnPropertyChanged("TextSituacionEconomica"); }
        }
        

        private decimal? _TextEgresoLuz;

        public decimal? TextEgresoLuz
        {
            get { return _TextEgresoLuz; }
            set
            {
                _TextEgresoLuz = value;
                OnPropertyChanged("TextEgresoLuz");
                ObtenerTotalEgresos();
            }
        }
        private decimal? _TextEgresoEducacion;

        public decimal? TextEgresoEducacion
        {
            get { return _TextEgresoEducacion; }
            set
            {
                _TextEgresoEducacion = value;
                OnPropertyChanged("TextEgresoEducacion");
                ObtenerTotalEgresos();
            }

        }

        private decimal? _TextEgresoCombustible;
        public decimal? TextEgresoCombustible
        {
            get { return _TextEgresoCombustible; }
            set
            {
                _TextEgresoCombustible = value;
                OnPropertyChanged("TextEgresoCombustible");
                ObtenerTotalEgresos();

            }

        }

        private decimal? _TextEgresoRenta;
        public decimal? TextEgresoRenta
        {
            get { return _TextEgresoRenta; }
            set
            {
                _TextEgresoRenta = value;
                OnPropertyChanged("TextEgresoRenta");
                ObtenerTotalEgresos();
            }

        }

        private decimal? _TextEgresoAgua;
        public decimal? TextEgresoAgua
        {
            get { return _TextEgresoAgua; }
            set
            {
                _TextEgresoAgua = value;
                OnPropertyChanged("TextEgresoAgua");
                ObtenerTotalEgresos();
            }

        }

        private decimal? _TextEgresoGas;

        public decimal? TextEgresoGas
        {
            get { return _TextEgresoGas; }
            set
            {
                _TextEgresoGas = value;
                OnPropertyChanged("TextEgresoGas");
                ObtenerTotalEgresos();
            }
        }


        private decimal? _TextEgresoTelefono;
        public decimal? TextEgresoTelefono
        {
            get { return _TextEgresoTelefono; }
            set
            {
                _TextEgresoTelefono = value;
                OnPropertyChanged("TextEgresoTelefono");
                ObtenerTotalEgresos();
            }

        }

        private decimal? _TextEgresoVestimenta;
        public decimal? TextEgresoVestimenta
        {
            get { return _TextEgresoVestimenta; }
            set
            {
                _TextEgresoVestimenta = value;
                OnPropertyChanged("TextEgresoVestimenta");
                ObtenerTotalEgresos();
            }

        }

        private decimal? _TextEgresoGatosMedicos;
        public decimal? TextEgresoGatosMedicos
        {
            get { return _TextEgresoGatosMedicos; }
            set
            {
                _TextEgresoGatosMedicos = value;
                OnPropertyChanged("TextEgresoGatosMedicos");
                ObtenerTotalEgresos();
            }

        }
        private decimal? _TextEgresoDespensa;
        public decimal? TextEgresoDespensa
        {
            get { return _TextEgresoDespensa; }
            set
            {
                _TextEgresoDespensa = value;
                OnPropertyChanged("TextEgresoDespensa");
                ObtenerTotalEgresos();
            }

        }
        private decimal? _TextEgresoLeche;
        public decimal? TextEgresoLeche
        {
            get { return _TextEgresoLeche; }
            set
            {
                _TextEgresoLeche = value;
                OnPropertyChanged("TextEgresoLeche");
                ObtenerTotalEgresos();
            }

        }
        private decimal? _TextEgresoLeguiminosas;
        public decimal? TextEgresoLeguiminosas
        {
            get { return _TextEgresoLeguiminosas; }
            set
            {
                _TextEgresoLeguiminosas = value;
                OnPropertyChanged("TextEgresoLeguiminosas");
                ObtenerTotalEgresos();


            }

        }
        private decimal? _TextEgresoFrijol;
        public decimal? TextEgresoFrijol
        {
            get { return _TextEgresoFrijol; }
            set
            {
                _TextEgresoFrijol = value;
                OnPropertyChanged("TextEgresoFrijol");
                ObtenerTotalEgresos();

            }

        }
        private decimal? _TextEgresoPolllo;
        public decimal? TextEgresoPolllo
        {
            get { return _TextEgresoPolllo; }
            set
            {
                _TextEgresoPolllo = value;
                OnPropertyChanged("TextEgresoPolllo");
                ObtenerTotalEgresos();
            }

        }

        private decimal? _TextEgresoTortillas;

        public decimal? TextEgresoTortillas
        {
            get { return _TextEgresoTortillas; }
            set
            {
                _TextEgresoTortillas = value;
                OnPropertyChanged("TextEgresoTortillas");
                ObtenerTotalEgresos();
            }
        }

        private decimal? _TextEgresoCereales;
        public decimal? TextEgresoCereales
        {
            get { return _TextEgresoCereales; }
            set
            {
                _TextEgresoCereales = value;
                OnPropertyChanged("TextEgresoCereales");
                ObtenerTotalEgresos();
            }

        }

        private decimal? _TextEgresoPastas;
        public decimal? TextEgresoPastas
        {
            get { return _TextEgresoPastas; }
            set
            {
                _TextEgresoPastas = value;
                OnPropertyChanged("TextEgresoPastas");
                ObtenerTotalEgresos();
            }

        }

        private decimal? _TextEgresoCarneRoja;
        public decimal? TextEgresoCarneRoja
        {
            get { return _TextEgresoCarneRoja; }
            set
            {
                _TextEgresoCarneRoja = value;
                OnPropertyChanged("TextEgresoCarneRoja");
                ObtenerTotalEgresos();

            }

        }

        private decimal? _TextEgresoverduras;
        public decimal? TextEgresoverduras
        {
            get { return _TextEgresoverduras; }
            set
            {
                _TextEgresoverduras = value;
                OnPropertyChanged("TextEgresoverduras");
                ObtenerTotalEgresos();
            }

        }


        private decimal? _TextEgresoGolosinas;
        public decimal? TextEgresoGolosinas
        {
            get { return _TextEgresoGolosinas; }
            set
            {
                _TextEgresoGolosinas = value;
                OnPropertyChanged("TextEgresoGolosinas");
                ObtenerTotalEgresos();
            }

        }

        private string _TextEgresoDescrOtros;

        public string TextEgresoDescrOtros
        {
            get { return _TextEgresoDescrOtros; }
            set { _TextEgresoDescrOtros = value; OnPropertyChanged("TextEgresoDescrOtros"); }
        }

        private decimal? ObtenerTotalEgresos()
        {


            decimal? agua = TextEgresoAgua != null ? TextEgresoAgua : 0;
            decimal? cereales = TextEgresoCereales != null ? TextEgresoCereales : 0;
            decimal? combustible = TextEgresoCombustible != null ? TextEgresoCombustible : 0;
            decimal? despensa = TextEgresoDespensa != null ? TextEgresoDespensa : 0;
            decimal? educacion = TextEgresoEducacion != null ? TextEgresoEducacion : 0;
            decimal? frijol = TextEgresoFrijol != null ? TextEgresoFrijol : 0;
            decimal? gas = TextEgresoGas != null ? TextEgresoGas : 0;
            decimal? GatosMedicos = TextEgresoGatosMedicos != null ? TextEgresoGatosMedicos : 0;
            decimal? Golosinas = TextEgresoGolosinas != null ? TextEgresoGolosinas : 0;
            decimal? leche = TextEgresoLeche != null ? TextEgresoLeche : 0;
            decimal? leguiminosas = TextEgresoLeguiminosas != null ? TextEgresoLeguiminosas : 0;
            decimal? pastas = TextEgresoPastas != null ? TextEgresoPastas : 0;
            decimal? pollo = TextEgresoPolllo != null ? TextEgresoPolllo : 0;
            decimal? renta = TextEgresoRenta != null ? TextEgresoRenta : 0;
            decimal? telefono = TextEgresoTelefono != null ? TextEgresoTelefono : 0;
            decimal? tortillas = TextEgresoTortillas != null ? TextEgresoTortillas : 0;
            decimal? verduras = TextEgresoverduras != null ? TextEgresoverduras : 0;
            decimal? vestimenta = TextEgresoVestimenta != null ? TextEgresoVestimenta : 0;
            decimal? carneroja = TextEgresoCarneRoja != null ? TextEgresoCarneRoja : 0;
            decimal? egresosoOtros = TextEgresoOtros != null ? TextEgresoOtros : 0;
            decimal? Luz = TextEgresoLuz != null ? TextEgresoLuz : 0;
            TextEgresoTotal = agua + cereales + combustible + despensa + educacion + frijol + gas + GatosMedicos + Golosinas + leche + leguiminosas + pastas + pollo + renta + telefono + tortillas + verduras + vestimenta + carneroja + egresosoOtros + Luz;
            return TextEgresoTotal;
        }

        private decimal? _TextEgresoOtros;
        public decimal? TextEgresoOtros
        {
            get { return _TextEgresoOtros; }
            set
            {
                _TextEgresoOtros = value;
                ObtenerTotalEgresos();
                OnPropertyChanged("TextEgresoOtros");
            }

        }
        private int? _TextComidasAlDia;

        public int? TextComidasAlDia
        {
            get { return _TextComidasAlDia; }
            set { _TextComidasAlDia = value; OnPropertyChanged("TextComidasAlDia"); }
        }


        #endregion

        #region EstructuraDinamicaFamiliar

        private string apellidoPaternoBuscar;

        private bool _ViveConPadres;
        public bool ViveConPadres
        {
            get { return _ViveConPadres; }
            set
            {
                if (value)
                {
                    // ValidacionEstructuraDinamica();
                    base.AddRule(() => DesdeCuandoVivePadres, () => !string.IsNullOrEmpty(DesdeCuandoVivePadres), "DESDE CUANDO VIVI CON PADRES ES REQUERIDO!");
                    OnPropertyChanged("DesdeCuandoVivePadres");
                    EnabledPadresJuntos = true;
                }
                else
                {
                    EnabledPadresJuntos = false;
                    DesdeCuandoVivePadres = string.Empty;
                }
                _ViveConPadres = value;
                PadresVivenJuntos = "Si";
                OnPropertyChanged("ViveConPadres");
                OnPropertyChanged("PadresVivenJuntos");
            }
        }


        private bool _ViveConPadresNo;
        public bool ViveConPadresNo
        {
            get { return _ViveConPadresNo; }
            set
            {
                if (value)
                {
                    base.RemoveRule("DesdeCuandoVivePadres"); OnPropertyChanged("DesdeCuandoVivePadres");
                }
                _ViveConPadresNo = value; OnPropertyChanged("ViveConPadresNo");
                PadresVivenJuntos = "No";
                OnPropertyChanged("PadresVivenJuntos");

            }
        }

        private string _DesdeCuandoVivePadres;

        public string DesdeCuandoVivePadres
        {
            get { return _DesdeCuandoVivePadres; }
            set { _DesdeCuandoVivePadres = value; OnPropertyChanged("DesdeCuandoVivePadres"); }
        }

        private string _DescrDinamicaFamiliar;


        public string DescrDinamicaFamiliar
        {
            get { return _DescrDinamicaFamiliar; }
            set { _DescrDinamicaFamiliar = value; OnPropertyChanged("DescrDinamicaFamiliar"); }
        }


        private string _ProblemaFamiliar;

        public string ProblemaFamiliar
        {
            get { return _ProblemaFamiliar; }
            set { _ProblemaFamiliar = value; OnPropertyChanged("ProblemaFamiliar"); }
        }

        private bool _ProblemaFamiliarSi;


        public bool ProblemaFamiliarSi
        {
            get { return _ProblemaFamiliarSi; }
            set
            {
                _ProblemaFamiliarSi = value;
                ProblemaFamiliar = "Si";
                OnPropertyChanged("ProblemaFamiliar");
                OnPropertyChanged("ProblemaFamiliarSi");
            }
        }

        private bool _ProblemaFamiliarNo;

        public bool ProblemaFamiliarNo
        {
            get { return _ProblemaFamiliarNo; }
            set
            {
                _ProblemaFamiliarNo = value;

                ProblemaFamiliar = "No";
                OnPropertyChanged("ProblemaFamiliar");
                OnPropertyChanged("ProblemaFamiliarNo");
            }
        }


        private string _MiembroFamiliaAbandono;

        public string MiembroFamiliaAbandono
        {
            get { return _MiembroFamiliaAbandono; }
            set { _MiembroFamiliaAbandono = value; OnPropertyChanged("MiembroFamiliaAbandono"); }
        }

        private bool _MiembroFamiliaAbandonoHogarSi;

        public bool MiembroFamiliaAbandonoHogarSi
        {
            get { return _MiembroFamiliaAbandonoHogarSi; }
            set
            {
                _MiembroFamiliaAbandonoHogarSi = value;
                MiembroFamiliaAbandono = "Si";
                OnPropertyChanged("MiembroFamiliaAbandono");
                OnPropertyChanged("MiembroFamiliaAbandonoHogarSi");
            }
        }

        private bool _MiembroFamiliaAbandonoHogarNo;

        public bool MiembroFamiliaAbandonoHogarNo
        {
            get { return _MiembroFamiliaAbandonoHogarNo; }
            set
            {
                _MiembroFamiliaAbandonoHogarNo = value;
                MiembroFamiliaAbandono = "No";
                OnPropertyChanged("MiembroFamiliaAbandonoHogarNo");
            }
        }

        private string _TextFormaPorqueApoyo;


        public string TextFormaPorqueApoyo
        {
            get { return _TextFormaPorqueApoyo; }
            set { _TextFormaPorqueApoyo = value; OnPropertyChanged("TextFormaPorqueApoyo"); }
        }

        private string _RecibioApoyoprocesoJuducal;

        public string RecibioApoyoprocesoJuducal
        {
            get { return _RecibioApoyoprocesoJuducal; }
            set { _RecibioApoyoprocesoJuducal = value; OnPropertyChanged("RecibioApoyoprocesoJuducal"); }
        }

        private bool _RecibioApoyoEconomicoEnProcesojudicialSi;
        public bool RecibioApoyoEconomicoEnProcesojudicialSi
        {
            get { return _RecibioApoyoEconomicoEnProcesojudicialSi; }
            set
            {

                if (value)
                {
                    base.AddRule(() => TextFormaPorqueApoyo, () => !string.IsNullOrEmpty(TextFormaPorqueApoyo), "FORMA APOYO ES REQUERIDO!");
                    OnPropertyChanged("TextFormaPorqueApoyo");
                    EnabledApoyoFamiliar = true;
                }
                else
                {
                    EnabledApoyoFamiliar = false;
                    TextFormaPorqueApoyo = string.Empty;
                }
                _RecibioApoyoEconomicoEnProcesojudicialSi = value;
                RecibioApoyoprocesoJuducal = "Si";
                OnPropertyChanged("RecibioApoyoprocesoJuducal");
                OnPropertyChanged("RecibioApoyoEconomicoEnProcesojudicialSi");
            }
        }

        private bool _RecibioApoyoEconomicoEnProcesojudicialNo;

        public bool RecibioApoyoEconomicoEnProcesojudicialNo
        {
            get { return _RecibioApoyoEconomicoEnProcesojudicialNo; }
            set
            {
                if (value)
                {
                    base.RemoveRule("TextFormaPorqueApoyo"); OnPropertyChanged("TextFormaPorqueApoyo");
                }
                _RecibioApoyoEconomicoEnProcesojudicialNo = value;

                RecibioApoyoprocesoJuducal = "No";
                OnPropertyChanged("RecibioApoyoprocesoJuducal");
                OnPropertyChanged("RecibioApoyoEconomicoEnProcesojudicialNo");
            }
        }

        private string _AntecedentesPenales;


        public string AntecedentesPenales
        {
            get { return _AntecedentesPenales; }
            set { _AntecedentesPenales = value; OnPropertyChanged("AntecedentesPenales"); }
        }


        private bool _ExitenAntecedentespenalesFamiiarSi;


        public bool ExitenAntecedentespenalesFamiiarSi
        {
            get { return _ExitenAntecedentespenalesFamiiarSi; }
            set
            {
                _ExitenAntecedentespenalesFamiiarSi = value;

                AntecedentesPenales = "Si";
                OnPropertyChanged("AntecedentesPenales");
                OnPropertyChanged("ExitenAntecedentespenalesFamiiarSi");
            }
        }


        private bool _ExitenAntecedentespenalesFamiiarNo;


        public bool ExitenAntecedentespenalesFamiiarNo
        {
            get { return _ExitenAntecedentespenalesFamiiarNo; }
            set
            {
                _ExitenAntecedentespenalesFamiiarNo = value;
                AntecedentesPenales = "No";
                OnPropertyChanged("AntecedentesPenales");
                OnPropertyChanged("ExitenAntecedentespenalesFamiiarNo");
            }
        }
        private string _SustanciasToxicas;

        public string SustanciasToxicas
        {
            get { return _SustanciasToxicas; }
            set { _SustanciasToxicas = value; OnPropertyChanged("SustanciasToxicas"); }
        }

        private bool _FamiliarConsumeSustanciaSi;

        public bool FamiliarConsumeSustanciaSi
        {
            get { return _FamiliarConsumeSustanciaSi; }
            set
            {
                _FamiliarConsumeSustanciaSi = value;

                SustanciasToxicas = "Si";
                OnPropertyChanged("FamiliarConsumeSustanciaSi");
            }
        }

        private bool _FamiliarConsumeSustanciaNo;

        public bool FamiliarConsumeSustanciaNo
        {
            get { return _FamiliarConsumeSustanciaNo; }
            set
            {
                _FamiliarConsumeSustanciaNo = value;
                SustanciasToxicas = "No";
                OnPropertyChanged("FamiliarConsumeSustanciaNo");
            }
        }

        private string _ConsumidoDrogas;

        public string ConsumidoDrogas
        {
            get { return _ConsumidoDrogas; }
            set { _ConsumidoDrogas = value; OnPropertyChanged("ConsumidoDrogas"); }
        }





        private bool _ConsumidoAlgunTipoDrogaSi;


        public bool ConsumidoAlgunTipoDrogaSi
        {
            get { return _ConsumidoAlgunTipoDrogaSi; }
            set
            {
                _ConsumidoAlgunTipoDrogaSi = value;
                ConsumidoDrogas = "Si";
                OnPropertyChanged("ConsumidoDrogas");
                OnPropertyChanged("ConsumidoAlgunTipoDrogaSi");
            }
        }

        private bool _ConsumidoAlgunTipoDrogaNo;


        public bool ConsumidoAlgunTipoDrogaNo
        {
            get { return _ConsumidoAlgunTipoDrogaNo; }
            set
            {
                _ConsumidoAlgunTipoDrogaNo = value;

                ConsumidoDrogas = "No";
                OnPropertyChanged("ConsumidoDrogas");
                OnPropertyChanged("ConsumidoAlgunTipoDrogaNo");
            }
        }
        private string _AntecedentesAnteriores;



        public string AntecedentesAnteriores
        {
            get { return _AntecedentesAnteriores; }
            set { _AntecedentesAnteriores = value; OnPropertyChanged("AntecedentesAnteriores"); }
        }


        private bool _AntecedentesPernalesSi;

        public bool AntecedentesPernalesSi
        {
            get { return _AntecedentesPernalesSi; }
            set
            {
                _AntecedentesPernalesSi = value;
                AntecedentesAnteriores = "Si";
                OnPropertyChanged("AntecedentesAnteriores");
                OnPropertyChanged("AntecedentesPernalesSi");
            }
        }

        private bool _AntecedentesPernalesNo;

        public bool AntecedentesPernalesNo
        {
            get { return _AntecedentesPernalesNo; }
            set
            {
                _AntecedentesPernalesNo = value;
                AntecedentesAnteriores = "No";
                OnPropertyChanged("AntecedentesAnteriores");

                OnPropertyChanged("AntecedentesPernalesNo");
            }
        }


        private string _ApoyoDuranteInternamiento;


        public string ApoyoDuranteInternamiento
        {
            get { return _ApoyoDuranteInternamiento; }
            set
            {
                _ApoyoDuranteInternamiento = value;


                OnPropertyChanged("ApoyoDuranteInternamiento");
            }
        }

        private bool _RecibioApoyoInternamientoSi;
        public bool RecibioApoyoInternamientoSi
        {
            get { return _RecibioApoyoInternamientoSi; }
            set
            {
                if (value)
                {
                    base.AddRule(() => TextDeQuienRecibioApoyoInternamiento, () => !string.IsNullOrEmpty(TextDeQuienRecibioApoyoInternamiento), "RECIBIO APOYO  INTERNAMIENTO ES REQUERIDO!");
                    OnPropertyChanged("TextDeQuienRecibioApoyoInternamiento");
                    base.AddRule(() => TextFrecuencia, () => !string.IsNullOrEmpty(TextFrecuencia), "FRECUENCIA ES REQUERIDO!");
                    OnPropertyChanged("TextFrecuencia");
                    EnabledApoyoInternamiento = true;
                }
                else 
                {
                    EnabledApoyoInternamiento = false;
                    TextDeQuienRecibioApoyoInternamiento = TextFrecuencia = string.Empty;
                }
                _RecibioApoyoInternamientoSi = value;
                ApoyoDuranteInternamiento = "Si";
                OnPropertyChanged("ApoyoDuranteInternamiento");
                OnPropertyChanged("RecibioApoyoInternamientoSi");
            }
        }

        private bool _RecibioApoyoInternamientoNo;

        public bool RecibioApoyoInternamientoNo
        {
            get { return _RecibioApoyoInternamientoNo; }
            set
            {
                if (value)
                {
                    base.RemoveRule("TextDeQuienRecibioApoyoInternamiento"); OnPropertyChanged("TextDeQuienRecibioApoyoInternamiento");
                    base.RemoveRule("TextFrecuencia"); OnPropertyChanged("TextFrecuencia");
                }
                _RecibioApoyoInternamientoNo = value;
                ApoyoDuranteInternamiento = "No";
                OnPropertyChanged("ApoyoDuranteInternamiento");
                OnPropertyChanged("RecibioApoyoInternamientoNo");
            }
        }

        private string _TextDeQuienRecibioApoyoInternamiento;

        public string TextDeQuienRecibioApoyoInternamiento
        {
            get { return _TextDeQuienRecibioApoyoInternamiento; }
            set { _TextDeQuienRecibioApoyoInternamiento = value; OnPropertyChanged("TextDeQuienRecibioApoyoInternamiento"); }
        }


        private string _TextFrecuencia;


        public string TextFrecuencia
        {
            get { return _TextFrecuencia; }
            set { _TextFrecuencia = value; OnPropertyChanged("TextFrecuencia"); }
        }

        private string _TextunionesAnteriores;


        public string TextunionesAnteriores
        {
            get { return _TextunionesAnteriores; }
            set { _TextunionesAnteriores = value; OnPropertyChanged("TextunionesAnteriores"); }
        }

        private int? _NoHijos;

        public int? NoHijos
        {
            get { return _NoHijos; }
            set { _NoHijos = value; OnPropertyChanged("NoHijos"); }
        }

        private string _ProblemaPareja;

        public string ProblemaPareja
        {
            get { return _ProblemaPareja; }
            set { _ProblemaPareja = value; OnPropertyChanged("ProblemaPareja"); }
        }


        private bool _ProblemaParejaSi;

        public bool ProblemaParejaSi
        {
            get { return _ProblemaParejaSi; }
            set
            {
                _ProblemaParejaSi = value;
                ProblemaPareja = "Si";
                OnPropertyChanged("ProblemaParejaSi");
            }
        }

        private bool _ProblemaParejaNo;

        public bool ProblemaParejaNo
        {
            get { return _ProblemaParejaNo; }
            set
            {
                _ProblemaParejaNo = value;
                ProblemaPareja = "No";
                OnPropertyChanged("ProblemaParejaNo");
            }
        }



        private string _PadresVivenjuntosHeader = "PADRES VIVIEN JUNTOS";

        public string PadresVivenjuntosHeader
        {
            get { return _PadresVivenjuntosHeader; }
            set { _PadresVivenjuntosHeader = value; OnPropertyChanged("PadresVivenjuntosHeader"); }
        }

        private string _PadresVivenJuntos;
        public string PadresVivenJuntos
        {
            get { return _PadresVivenJuntos; }
            set { _PadresVivenJuntos = value; OnPropertyChanged("PadresVivenJuntos"); }

        }

        private string _VivePadresSi = "Si";

        public string VivePadresSi
        {
            get { return _VivePadresSi; }
            set
            {
                _VivePadresSi = value; OnPropertyChanged("VivePadresSi");
            }
        }

        private bool _MiembroFamiliaAbandonoFamSi;

        public bool MiembroFamiliaAbandonoFamSi
        {
            get { return _MiembroFamiliaAbandonoFamSi; }
            set { _MiembroFamiliaAbandonoFamSi = value; OnPropertyChanged("MiembroFamiliaAbandonoFamSi"); }
        }

        private decimal? _TextEgresoTotal;

        public decimal? TextEgresoTotal
        {
            get { return _TextEgresoTotal; }
            set { _TextEgresoTotal = value; OnPropertyChanged("TextEgresoTotal"); }
        }

        private ObservableCollection<EstructuraViviendaClass> _ListaEstructuraVivienda;

        public ObservableCollection<EstructuraViviendaClass> ListaEstructuraVivienda
        {
            get { return _ListaEstructuraVivienda; }
            set { _ListaEstructuraVivienda = value; OnPropertyChanged("ListaEstructuraVivienda"); }
        }
        #endregion

        #region Busqueda e Imagenes de Imputado

        #region SituacionActual

        private string _ConoceVecinos;

        public string ConoceVecinos
        {
            get { return _ConoceVecinos; }
            set { _ConoceVecinos = value; OnPropertyChanged("ConoceVecinos"); }
        }

        private bool _ConoceVecinosSi;

        public bool ConoceVecinosSi
        {
            get { return _ConoceVecinosSi; }
            set
            {
                _ConoceVecinosSi = value;
                ConoceVecinos = "Si";
                OnPropertyChanged("ConoceVecinos");
                OnPropertyChanged("ConoceVecinosSi");
            }
        }

        private bool _ConoceVecinosNo;

        public bool ConoceVecinosNo
        {
            get { return _ConoceVecinosNo; }
            set
            {
                _ConoceVecinosNo = value;

                ConoceVecinos = "No";
                OnPropertyChanged("ConoceVecinos");
                OnPropertyChanged("ConoceVecinosNo");
            }
        }

        private string _ProblemaAlguno;


        public string ProblemaAlguno
        {
            get { return _ProblemaAlguno; }
            set { _ProblemaAlguno = value; OnPropertyChanged("ProblemaAlguno"); }
        }

        private bool _ProblemasConVecinosSi;

        public bool ProblemasConVecinosSi
        {
            get { return _ProblemasConVecinosSi; }
            set
            {
                _ProblemasConVecinosSi = value;
                ProblemaAlguno = "Si";
                OnPropertyChanged("ProblemaAlguno");
                OnPropertyChanged("ProblemasConVecinosSi");
            }
        }

        private bool _ProblemasConVecinosNo;

        public bool ProblemasConVecinosNo
        {
            get { return _ProblemasConVecinosNo; }
            set
            {
                _ProblemasConVecinosNo = value;
                ProblemaAlguno = "No";
                OnPropertyChanged("ProblemaAlguno");

                OnPropertyChanged("ProblemasConVecinosNo");
            }
        }


        private string _TextTiempoLibre;

        public string TextTiempoLibre
        {
            get { return _TextTiempoLibre; }
            set { _TextTiempoLibre = value.Trim(); OnPropertyChanged("TextTiempoLibre"); }
        }

        private string _TextTiempoLibreOtro;

        public string TextTiempoLibreOtro
        {
            get { return _TextTiempoLibreOtro; }
            set { _TextTiempoLibreOtro = value.Trim(); OnPropertyChanged("TextTiempoLibreOtro"); }
        }

        private byte[] imagenInterno = new Imagenes().getImagenPerson();
        public byte[] ImagenInterno
        {
            get { return imagenInterno; }
            set { imagenInterno = value; OnPropertyChanged("ImagenInterno"); }
        }

        private bool _chkActaNac;

        public bool chkActaNac
        {
            get { return _chkActaNac; }
            set { _chkActaNac = value; OnPropertyChanged("chkActaNac"); }
        }

        private bool _chkCurp;

        public bool chkCurp
        {
            get { return _chkCurp; }
            set { _chkCurp = value; OnPropertyChanged("chkCurp"); }
        }

        private bool _chkComprobanteEstudio;

        public bool chkComprobanteEstudio
        {
            get { return _chkComprobanteEstudio; }
            set { _chkComprobanteEstudio = value; OnPropertyChanged("chkComprobanteEstudio"); }
        }
        private bool _chkCartilla;

        public bool chkCartilla
        {
            get { return _chkCartilla; }
            set { _chkCartilla = value; OnPropertyChanged("chkCartilla"); }
        }
        private bool _chkVisaLaser;

        public bool chkVisaLaser
        {
            get { return _chkVisaLaser; }
            set { _chkVisaLaser = value; OnPropertyChanged("chkVisaLaser"); }
        }


        private bool _chkLiciencia;

        public bool chkLiciencia
        {
            get { return _chkLiciencia; }
            set { _chkLiciencia = value; OnPropertyChanged("chkLiciencia"); }
        }

        private bool _chkActaMatrimonio;

        public bool chkActaMatrimonio
        {
            get { return _chkActaMatrimonio; }
            set { _chkActaMatrimonio = value; OnPropertyChanged("chkActaMatrimonio"); }
        }
        private bool _chkPasaporteMex;

        public bool chkPasaporteMex
        {
            get { return _chkPasaporteMex; }
            set { _chkPasaporteMex = value; OnPropertyChanged("chkPasaporteMex"); }
        }

        private bool _chkIFE;

        public bool chkIFE
        {
            get { return _chkIFE; }
            set { _chkIFE = value; OnPropertyChanged("chkIFE"); }
        }

        private string _TextDocumentosOtro;

        public string TextDocumentosOtro
        {
            get { return _TextDocumentosOtro; }
            set { _TextDocumentosOtro = value.Trim(); OnPropertyChanged("TextDocumentosOtro"); }
        }

        private string _PadecioEnfermedad;

        public string PadecioEnfermedad
        {
            get { return _PadecioEnfermedad; }
            set { _PadecioEnfermedad = value; OnPropertyChanged("PadecioEnfermedad"); }
        }

        private bool _PadeceEnfermedadSi;
        public bool PadeceEnfermedadSi
        {
            get { return _PadeceEnfermedadSi; }
            set
            {
                if (value)
                {
                    base.AddRule(() => TextEspecifiqueOtraEnfermedad, () => !string.IsNullOrEmpty(TextEspecifiqueOtraEnfermedad), "ESPECIFIQUE OTRA ENFERMEDAD ES REQUERIDO!");
                    OnPropertyChanged("TextEspecifiqueOtraEnfermedad");
                    base.AddRule(() => TextTipoTratamientoRecibido, () => !string.IsNullOrEmpty(TextTipoTratamientoRecibido), "TIPO DE TRATAMIENTO RECIBIDO ES REQUERIDO!");
                    OnPropertyChanged("TextTipoTratamientoRecibido");
                    EnabledPadeceEnfermedad = true;
                }
                else
                {
                    TextEspecifiqueOtraEnfermedad = TextTipoTratamientoRecibido = string.Empty;
                    EnabledPadeceEnfermedad = false; 
                }
                _PadeceEnfermedadSi = value;

                PadecioEnfermedad = "Si";
                OnPropertyChanged("PadecioEnfermedad");
                OnPropertyChanged("PadeceEnfermedadSi");
            }
        }
        private bool _PadeceEnfermedadNo;

        public bool PadeceEnfermedadNo
        {
            get { return _PadeceEnfermedadNo; }
            set
            {
                if (value)
                {
                   base.RemoveRule("TextEspecifiqueOtraEnfermedad"); OnPropertyChanged("TextEspecifiqueOtraEnfermedad");
                   base.RemoveRule("TextTipoTratamientoRecibido"); OnPropertyChanged("TextTipoTratamientoRecibido");  
                }
                _PadeceEnfermedadNo = value; PadecioEnfermedad = "No";
                OnPropertyChanged("PadeceEnfermedadNo");
            }
        }

        private string _textDiagnostico;


        public string textDiagnostico
        {
            get { return _textDiagnostico; }
            set { _textDiagnostico = value.Trim(); OnPropertyChanged("textDiagnostico"); }
        }

        private string _TextEspecifiqueOtraEnfermedad;

        public string TextEspecifiqueOtraEnfermedad
        {
            get { return _TextEspecifiqueOtraEnfermedad; }
            set { _TextEspecifiqueOtraEnfermedad = value.Trim(); OnPropertyChanged("TextEspecifiqueOtraEnfermedad"); }
        }

        private string _TextTipoTratamientoRecibido;

        public string TextTipoTratamientoRecibido
        {
            get { return _TextTipoTratamientoRecibido; }
            set { _TextTipoTratamientoRecibido = value.Trim(); OnPropertyChanged("TextTipoTratamientoRecibido"); }
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
             //   SelectMJEnabled = value != null ? true : false;
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


        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }
        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }

           
        private bool SeguirCargando { get; set; }

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

        #region ApoyoEconomicoPopup
        private string _TextNombreFamiliar;

        public string TextNombreFamiliar
        {
            get { return _TextNombreFamiliar; }
            set { _TextNombreFamiliar = value; OnPropertyChanged("TextNombreFamiliar"); }
        }

        private string _TextOficio;

        public string TextOficio
        {
            get { return _TextOficio; }
            set { _TextOficio = value; OnPropertyChanged("TextOficio"); }
        }


        private ObservableCollection<OCUPACION> _ListOcupacionApoyoEconomic;

        public ObservableCollection<OCUPACION> ListOcupacionApoyoEconomic
        {
            get { return _ListOcupacionApoyoEconomic; }
            set { _ListOcupacionApoyoEconomic = value; OnPropertyChanged("ListOcupacionApoyoEconomic"); }
        }



        private short? _SelectOcupacionEconomic = -1;

        public short? SelectOcupacionApoyoEconomic
        {
            get { return _SelectOcupacionEconomic; }
            set { _SelectOcupacionEconomic = value; OnPropertyChanged("SelectOcupacionApoyoEconomic"); }
        }


        private bool _ValidacionApoyoEconomicoRules;
        public bool ValidacionApoyoEconomicoRules
        {
            get { return _ValidacionApoyoEconomicoRules; }
            set { _ValidacionApoyoEconomicoRules = value; OnPropertyChanged("ValidacionApoyoEconomicoRules"); }
        }
        private string _TextAportaciones;

        public string TextAportaciones
        {
            get { return _TextAportaciones; }
            set { _TextAportaciones = value; OnPropertyChanged("TextAportaciones"); }
        }

        //PRS_APOYO_ECONOMICO
        private ObservableCollection<PRS_APOYO_ECONOMICO> _ListPersonasApoyo;

        public ObservableCollection<PRS_APOYO_ECONOMICO> ListPersonasApoyo
        {
            get { return _ListPersonasApoyo; }
            set { _ListPersonasApoyo = value; OnPropertyChanged("ListPersonasApoyo"); }
        }

        private PRS_APOYO_ECONOMICO _SelectPersonasApoyo;

        public PRS_APOYO_ECONOMICO SelectPersonasApoyo
        {
            get { return _SelectPersonasApoyo; }
            set { _SelectPersonasApoyo = value; OnPropertyChanged("SelectPersonasApoyo"); }
        }

        private bool _EditarApoyoEconomico;



        public bool EditarApoyoEconomico
        {
            get { return _EditarApoyoEconomico; }
            set { _EditarApoyoEconomico = value; OnPropertyChanged("EditarApoyoEconomico"); }
        }


        #endregion

        #region ModalConsumo Droga

        private ObservableCollection<PRS_DROGA> _ListDrogaConsumo;

        public ObservableCollection<PRS_DROGA> ListDrogaConsumo
        {
            get { return _ListDrogaConsumo; }
            set { _ListDrogaConsumo = value; OnPropertyChanged("ListDrogaConsumo"); }
        }

        private PRS_DROGA _SelectDrogaConsumo;
        public PRS_DROGA SelectDrogaConsumo
        {
            get { return _SelectDrogaConsumo; }
            set { _SelectDrogaConsumo = value; OnPropertyChanged("SelectDrogaConsumo"); }
        }

        private ObservableCollection<DROGA> _LstDrogas;

        public ObservableCollection<DROGA> LstDrogas
        {
            get { return _LstDrogas; }
            set { _LstDrogas = value; OnPropertyChanged("LstDrogas"); }
        }

        private short _popUpDrogaId = -1;
        public short popUpDrogaId
        {
            get { return _popUpDrogaId; }
            set { _popUpDrogaId = value; OnPropertyChanged("popUpDrogaId"); }
        }

        private ObservableCollection<DROGA_FRECUENCIA> _LstFrecuenciasUsoDrogas;


        public ObservableCollection<DROGA_FRECUENCIA> LstFrecuenciasUsoDrogas
        {
            get { return _LstFrecuenciasUsoDrogas; }
            set { _LstFrecuenciasUsoDrogas = value; OnPropertyChanged("LstFrecuenciasUsoDrogas"); }
        }

        private DateTime? _popUpFechaUltDosis;


        public DateTime? popUpFechaUltDosis
        {
            get { return _popUpFechaUltDosis; }
            set { _popUpFechaUltDosis = value; OnPropertyChanged("popUpFechaUltDosis"); }
        }

        private short? _popUpFrecuenciaUso = -1;

        public short? popUpFrecuenciaUso
        {
            get { return _popUpFrecuenciaUso; }
            set { _popUpFrecuenciaUso = value; OnPropertyChanged("popUpFrecuenciaUso"); }
        }


        #endregion

        #region Nucelo FAmiliar Popup


        private string _Tipo_Nucelo_Familiar;

        public string Tipo_Nucelo_Familiar
        {
            get { return _Tipo_Nucelo_Familiar; }
            set { _Tipo_Nucelo_Familiar = value; OnPropertyChanged("Tipo_Nucelo_Familiar"); }
        }

        private string _TextNombreNuceloFamiliar;

        public string TextNombreNuceloFamiliar
        {
            get { return _TextNombreNuceloFamiliar; }
            set { _TextNombreNuceloFamiliar = value; OnPropertyChanged("TextNombreNuceloFamiliar"); }
        }

        private int? _TextEdadNuceloFamiliar;

        public int? TextEdadNuceloFamiliar
        {
            get { return _TextEdadNuceloFamiliar; }
            set { _TextEdadNuceloFamiliar = value; OnPropertyChanged("TextEdadNuceloFamiliar"); }
        }

        private ObservableCollection<TIPO_REFERENCIA> _ListParentescoNucleoFamiliar;

        public ObservableCollection<TIPO_REFERENCIA> ListParentescoNucleoFamiliar 
        {
            get { return _ListParentescoNucleoFamiliar; }
            set { _ListParentescoNucleoFamiliar = value; OnPropertyChanged("ListParentescoNucleoFamiliar"); }
        }

        private TIPO_REFERENCIA _SelectedParentescoNuceloFamiliar;

        public TIPO_REFERENCIA SelectedParentescoNuceloFamiliar
        {
            get { return _SelectedParentescoNuceloFamiliar; }
            set { _SelectedParentescoNuceloFamiliar = value; OnPropertyChanged("SelectedParentescoNuceloFamiliar"); }
        }

        private short? _SelectParentescoNuceloFamiliar = -1;

        public short? SelectParentescoNuceloFamiliar
        {
            get { return _SelectParentescoNuceloFamiliar; }
            set { _SelectParentescoNuceloFamiliar = value; OnPropertyChanged("SelectParentescoNuceloFamiliar"); }
        }

        private ObservableCollection<ESTADO_CIVIL> _ListEstadoCivilNuceloFamiliar;
        public ObservableCollection<ESTADO_CIVIL> ListEstadoCivilNuceloFamiliar
        {
            get { return _ListEstadoCivilNuceloFamiliar; }
            set { _ListEstadoCivilNuceloFamiliar = value; OnPropertyChanged("ListEstadoCivilNuceloFamiliar"); }
        }


        private short? _SelectEstadoCivilNuceloFamiliar = -1;
        public short? SelectEstadoCivilNuceloFamiliar
        {
            get { return _SelectEstadoCivilNuceloFamiliar; }
            set
            {
                if (value == _SelectEstadoCivilNuceloFamiliar)
                    return;

                _SelectEstadoCivilNuceloFamiliar = value;

                OnPropertyValidateChanged("SelectEstadoCivilNuceloFamiliar");
            }
        }

        private ObservableCollection<ESCOLARIDAD> _ListEscolaridadNuceloFamiliar;
        public ObservableCollection<ESCOLARIDAD> ListEscolaridadNuceloFamiliar
        {
            get { return _ListEscolaridadNuceloFamiliar; }
            set { _ListEscolaridadNuceloFamiliar = value; OnPropertyChanged("ListEscolaridadNuceloFamiliar"); }
        }


        private short? _SelectEscolaridadNuceloFamiliar = -1;
        public short? SelectEscolaridadNuceloFamiliar
        {
            get { return _SelectEscolaridadNuceloFamiliar; }
            set
            {

                if (value==null)
                {
                    _SelectEscolaridadNuceloFamiliar = -1;
                }
                else
                {
                    _SelectEscolaridadNuceloFamiliar = value;
                }

                OnPropertyValidateChanged("SelectEscolaridadNuceloFamiliar");
            }
        }


        private ObservableCollection<OCUPACION> _ListOcupacionNucleoFamiliar;

        public ObservableCollection<OCUPACION> ListOcupacionNucleoFamiliar
        {
            get { return _ListOcupacionNucleoFamiliar; }
            set { _ListOcupacionNucleoFamiliar = value; OnPropertyChanged("ListOcupacionNucleoFamiliar"); }
        }



        private short? _SelectOcupacionNuceloFamiliar = -1;

        public short? SelectOcupacionNuceloFamiliar
        {
            get { return _SelectOcupacionNuceloFamiliar; }
            set { _SelectOcupacionNuceloFamiliar = value; OnPropertyChanged("SelectOcupacionNuceloFamiliar"); }
        }


      
        #endregion

        public class EstructuraViviendaClass : TrabajoSocialViewModel
        {
            private string _EstructuraVivienda;

            public string EstructuraVivienda
            {
                get { return _EstructuraVivienda; }
                set { _EstructuraVivienda = value; OnPropertyChanged("EstructuraVivienda"); }
            }

             //   private int? _NoEstructura;

            //public int? NoEstructura
            //{
            //    get { return _NoEstructura; }
            //    set { _NoEstructura = value; OnPropertyChanged("NoEstructura"); }
            //}

            //private string _Observaciones;

            //public string Observaciones
            //{
            //    get { return _Observaciones; }
            //    set { _Observaciones = value; OnPropertyChanged("Observaciones"); }
            //}

            //public EstructuraViviendaClass(string _EstructuraVivienda, int? _NoEstructura, string _Observaciones)
            //{
            //    EstructuraVivienda = _EstructuraVivienda;
            //    NoEstructura = _NoEstructura;
            //    Observaciones = _Observaciones;
            //}
        }

        #region Enabled
        private bool enabledPadeceEnfermedad = false;
        public bool EnabledPadeceEnfermedad
        {
            get { return enabledPadeceEnfermedad; }
            set { enabledPadeceEnfermedad = value; OnPropertyChanged("EnabledPadeceEnfermedad"); }
        }

        private bool enabledPadresJuntos = false;
        public bool EnabledPadresJuntos
        {
            get { return enabledPadresJuntos; }
            set { enabledPadresJuntos = value; OnPropertyChanged("EnabledPadresJuntos"); }
        }

        private bool enabledApoyoFamiliar = false;
        public bool EnabledApoyoFamiliar
        {
            get { return enabledApoyoFamiliar; }
            set { enabledApoyoFamiliar = value; OnPropertyChanged("EnabledApoyoFamiliar"); }
        }

        private bool enabledApoyoInternamiento = false;
        public bool EnabledApoyoInternamiento
        {
            get { return enabledApoyoInternamiento; }
            set { enabledApoyoInternamiento = value; OnPropertyChanged("EnabledApoyoInternamiento"); }
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
    }

    #region Nucelo Familiar Popup

    #endregion

    #region Datos Aoyo Economico Popup
    public class ApoyoFamiliarClase
    {
        public int? id_persona { get; set; }
        public string NOMBRE { get; set; }
        public string OCUPACION { get; set; }
        public int id_ocupacion { get; set; }
        public decimal? APORTACION { get; set; }
    }
    #endregion
}
