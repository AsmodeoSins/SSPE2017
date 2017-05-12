using SSP.Servidor;
using System;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class RealizacionEstudiosViewModel
    {
        #region Acta de consejo tecnico interdisciplinario
        private string _NombreImputadoFF;
        public string NombreImputadoFF
        {
            get { return _NombreImputadoFF; }
            set { _NombreImputadoFF = value; OnPropertyChanged("NombreImputadoFF"); }
        }

        private string _ExpedienteImputadoFF;
        public string ExpedienteImputadoFF
        {
            get { return _ExpedienteImputadoFF; }
            set { _ExpedienteImputadoFF = value; OnPropertyChanged("ExpedienteImputadoFF"); }
        }

        private PFF_SUSTANCIA_TOXICA _DrogaElegida;

        public PFF_SUSTANCIA_TOXICA DrogaElegida
        {
            get { return _DrogaElegida; }
            set { _DrogaElegida = value; OnPropertyChanged("DrogaElegida"); }
        }
        private string _Delito;
        public string Delito
        {
            get { return _Delito; }
            set { _Delito = value; OnPropertyChanged("Delito"); }
        }

        private string _DescripcionCursoFederal;

        public string DescripcionCursoFederal
        {
            get { return _DescripcionCursoFederal; }
            set { _DescripcionCursoFederal = value; OnPropertyChanged("DescripcionCursoFederal"); }
        }

        private DateTime? _SelectedFechaInicioCursoFederal;

        public DateTime? SelectedFechaInicioCursoFederal
        {
            get { return _SelectedFechaInicioCursoFederal; }
            set { _SelectedFechaInicioCursoFederal = value; OnPropertyChanged("SelectedFechaInicioCursoFederal"); }
        }

        private DateTime? _SelectedFechaFinCursoFederal;

        public DateTime? SelectedFechaFinCursoFederal
        {
            get { return _SelectedFechaFinCursoFederal; }
            set { _SelectedFechaFinCursoFederal = value; OnPropertyChanged("SelectedFechaFinCursoFederal"); }
        }

        private ObservableCollection<SentenciaIngreso> lstSentenciasIngresos;
        public ObservableCollection<SentenciaIngreso> LstSentenciasIngresos
        {
            get { return lstSentenciasIngresos; }
            set { lstSentenciasIngresos = value; OnPropertyChanged("LstSentenciasIngresos"); }
        }

        private string _Sentencia;

        public string Sentencia
        {
            get { return _Sentencia; }
            set { _Sentencia = value; OnPropertyChanged("Sentencia"); }
        }
        private string _APartirDe;

        public string APartirDe
        {
            get { return _APartirDe; }
            set { _APartirDe = value; OnPropertyChanged("APartirDe"); }
        }
        private DateTime? _EnSesionDeFecha;

        public DateTime? EnSesionDeFecha
        {
            get { return _EnSesionDeFecha; }
            set { _EnSesionDeFecha = value; OnPropertyChanged("EnSesionDeFecha"); }
        }
        private short? _IdEstado;

        public short? IdEstado
        {
            get { return _IdEstado; }
            set { _IdEstado = value; OnPropertyChanged("IdEstado"); }
        }

        private string _EstadoActual;
        public string EstadoActual
        {
            get { return _EstadoActual; }
            set { _EstadoActual = value; OnPropertyChanged("EstadoActual"); }
        }

        private short? _IdCentro;
        public short? IdCentro
        {
            get { return _IdCentro; }
            set { _IdCentro = value; OnPropertyChanged("IdCentro"); }
        }

        private string _CentroActual;
        public string CentroActual
        {
            get { return _CentroActual; }
            set { _CentroActual = value; OnPropertyChanged("CentroActual"); }
        }

        System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.CENTRO> _lstCentros;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.CENTRO> LstCentros
        {
            get { return _lstCentros; }
            set { _lstCentros = value; OnPropertyChanged("LstCentros"); }
        }

        ObservableCollection<AREA_TECNICA> _lstAreas;
        public ObservableCollection<AREA_TECNICA> LstAreas
        {
            get { return _lstAreas; }
            set { _lstAreas = value; OnPropertyChanged("LstAreas"); }
        }


        private AREA_TECNICA _SelArea;
        public AREA_TECNICA SelArea
        {
            get { return _SelArea; }
            set { _SelArea = value; OnPropertyChanged("SelArea"); }
        }

        System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PFF_ACTA_DETERMINO> _lstAreasTec;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.PFF_ACTA_DETERMINO> LstAreasTec
        {
            get { return _lstAreasTec; }
            set { _lstAreasTec = value; OnPropertyChanged("LstAreasTec"); }
        }

        private PFF_ACTA_DETERMINO _SelectedAreTec;
        public PFF_ACTA_DETERMINO SelectedAreTec
        {
            get { return _SelectedAreTec; }
            set { _SelectedAreTec = value; OnPropertyChanged("SelectedAreTec"); }
        }


        private string _DirectorCentro;
        public string DirectorCentro
        {
            get { return _DirectorCentro; }
            set { _DirectorCentro = value; OnPropertyChanged("DirectorCentro"); }
        }

        private string _ActuacionR;
        public string ActuacionR
        {
            get { return _ActuacionR; }
            set { _ActuacionR = value; OnPropertyChanged("ActuacionR"); }
        }

        private string _VotosR;
        public string VotosR
        {
            get { return _VotosR; }
            set { _VotosR = value; OnPropertyChanged("VotosR"); }
        }

        private string _TramiteDescripcion;
        public string TramiteDescripcion
        {
            get { return _TramiteDescripcion; }
            set { _TramiteDescripcion = value; OnPropertyChanged("TramiteDescripcion"); }
        }

        private string _NombreDir;
        public string NombreDir
        {
            get { return _NombreDir; }
            set { _NombreDir = value; OnPropertyChanged("NombreDir"); }
        }

        private DateTime? _FechaActa;
        public DateTime? FechaActa
        {
            get { return _FechaActa; }
            set { _FechaActa = value; OnPropertyChanged("FechaActa"); }
        }

        #region Datos de coordinadores de areas

        private short _IdAreaT;
        public short IdAreaT
        {
            get { return _IdAreaT; }
            set { _IdAreaT = value; OnPropertyChanged("IdAreaT"); }
        }

        private bool _IsEnabledCoordinadorArea = true;
        public bool IsEnabledCoordinadorArea
        {
            get { return _IsEnabledCoordinadorArea; }
            set { _IsEnabledCoordinadorArea = value; OnPropertyChanged("IsEnabledCoordinadorArea"); }
        }

        private string _NombreAreaMedica;

        public string NombreAreaMedica
        {
            get { return _NombreAreaMedica; }
            set { _NombreAreaMedica = value; OnPropertyChanged("NombreAreaMedica"); }
        }
        private string _OpinionAreaMedica;

        public string OpinionAreaMedica
        {
            get { return _OpinionAreaMedica; }
            set { _OpinionAreaMedica = value; OnPropertyChanged("OpinionAreaMedica"); }
        }
        private string _NombreAreaPsiquiatrica;

        public string NombreAreaPsiquiatrica
        {
            get { return _NombreAreaPsiquiatrica; }
            set { _NombreAreaPsiquiatrica = value; OnPropertyChanged("NombreAreaPsiquiatrica"); }
        }
        private string _OpinionAreaPsiquiatrica;

        public string OpinionAreaPsiquiatrica
        {
            get { return _OpinionAreaPsiquiatrica; }
            set { _OpinionAreaPsiquiatrica = value; OnPropertyChanged("OpinionAreaPsiquiatrica"); }
        }
        private string _NombreAreaPsicologica;

        public string NombreAreaPsicologica
        {
            get { return _NombreAreaPsicologica; }
            set { _NombreAreaPsicologica = value; OnPropertyChanged("NombreAreaPsicologica"); }
        }
        private string _OpinionAreaPsicologica;

        public string OpinionAreaPsicologica
        {
            get { return _OpinionAreaPsicologica; }
            set { _OpinionAreaPsicologica = value; OnPropertyChanged("OpinionAreaPsicologica"); }
        }
        private string _NombreAreaTS;

        public string NombreAreaTS
        {
            get { return _NombreAreaTS; }
            set { _NombreAreaTS = value; OnPropertyChanged("NombreAreaTS"); }
        }
        private string _OpinionAreaTS;

        public string OpinionAreaTS
        {
            get { return _OpinionAreaTS; }
            set { _OpinionAreaTS = value; OnPropertyChanged("OpinionAreaTS"); }
        }
        private string _NombreAreaEscolar;

        public string NombreAreaEscolar
        {
            get { return _NombreAreaEscolar; }
            set { _NombreAreaEscolar = value; OnPropertyChanged("NombreAreaEscolar"); }
        }
        private string _OpinionAreaEscolar;

        public string OpinionAreaEscolar
        {
            get { return _OpinionAreaEscolar; }
            set { _OpinionAreaEscolar = value; OnPropertyChanged("OpinionAreaEscolar"); }
        }
        private string _NombreAreaInd;

        public string NombreAreaInd
        {
            get { return _NombreAreaInd; }
            set { _NombreAreaInd = value; OnPropertyChanged("NombreAreaInd"); }
        }
        private string _OpinionAreaInd;

        public string OpinionAreaInd
        {
            get { return _OpinionAreaInd; }
            set { _OpinionAreaInd = value; OnPropertyChanged("OpinionAreaInd"); }
        }
        private string _NombreAreaVigDisc;

        public string NombreAreaVigDisc
        {
            get { return _NombreAreaVigDisc; }
            set { _NombreAreaVigDisc = value; OnPropertyChanged("NombreAreaVigDisc"); }
        }
        private string _OpinionAreaVigDisc;

        public string OpinionAreaVigDisc
        {
            get { return _OpinionAreaVigDisc; }
            set { _OpinionAreaVigDisc = value; OnPropertyChanged("OpinionAreaVigDisc"); }
        }
        private string _NombreAreaCrim;

        public string NombreAreaCrim
        {
            get { return _NombreAreaCrim; }
            set { _NombreAreaCrim = value; OnPropertyChanged("NombreAreaCrim"); }
        }
        private string _OpinionAreaCrim;

        public string OpinionAreaCrim
        {
            get { return _OpinionAreaCrim; }
            set { _OpinionAreaCrim = value; OnPropertyChanged("OpinionAreaCrim"); }
        }
        private string _NombreAreaJur;

        public string NombreAreaJur
        {
            get { return _NombreAreaJur; }
            set { _NombreAreaJur = value; OnPropertyChanged("NombreAreaJur"); }
        }
        private string _OpinionAreaJur;

        public string OpinionAreaJur
        {
            get { return _OpinionAreaJur; }
            set { _OpinionAreaJur = value; OnPropertyChanged("OpinionAreaJur"); }
        }
        private string _NombreCRS;

        public string NombreCRS
        {
            get { return _NombreCRS; }
            set { _NombreCRS = value; OnPropertyChanged("NombreCRS"); }
        }
        private short? _IdActuacion;

        public short? IdActuacion
        {
            get { return _IdActuacion; }
            set { _IdActuacion = value; OnPropertyChanged("IdActuacion"); }
        }
        private short? _IdVotacion;

        public short? IdVotacion
        {
            get { return _IdVotacion; }
            set { _IdVotacion = value; OnPropertyChanged("IdVotacion"); }
        }
        private string _Tramite;

        public string Tramite
        {
            get { return _Tramite; }
            set { _Tramite = value; OnPropertyChanged("Tramite"); }
        }
        private DateTime _FechaTramite;

        public DateTime FechaTramite
        {
            get { return _FechaTramite; }
            set { _FechaTramite = value; OnPropertyChanged("FechaTramite"); }
        }

        #endregion

        #endregion

        #region Estudio medico de fuero federal

        private string _LugarMedico;

        public string LugarMedico
        {
            get { return _LugarMedico; }
            set { _LugarMedico = value; OnPropertyChanged("LugarMedico"); }
        }


        private string _TensionFederalUno;
        public string TensionFederalUno
        {
            get { return _TensionFederalUno; }
            set
            { _TensionFederalUno = value; OnPropertyChanged("TensionFederalUno"); }
        }

        private string _TensionFederalDos;
        public string TensionFederalDos
        {
            get { return _TensionFederalDos; }
            set { _TensionFederalDos = value; OnPropertyChanged("TensionFederalDos"); }
        }

        private ObservableCollection<ESTADO_CIVIL> _LstEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> LstEstadoCivil
        {
            get { return _LstEstadoCivil; }
            set { _LstEstadoCivil = value; OnPropertyChanged("LstEstadoCivil"); }
        }

        private string _NombreImputadoMedicoFederal;
        public string NombreImputadoMedicoFederal
        {
            get { return _NombreImputadoMedicoFederal; }
            set { _NombreImputadoMedicoFederal = value; OnPropertyChanged("NombreImputadoMedicoFederal"); }
        }

        private string _AliasImputadoMedicoFederal;
        public string AliasImputadoMedicoFederal
        {
            get { return _AliasImputadoMedicoFederal; }
            set { _AliasImputadoMedicoFederal = value; OnPropertyChanged("AliasImputadoMedicoFederal"); }
        }

        private short? _EdadImputadoMedicoFederal;
        public short? EdadImputadoMedicoFederal
        {
            get { return _EdadImputadoMedicoFederal; }
            set { _EdadImputadoMedicoFederal = value; OnPropertyChanged("EdadImputadoMedicoFederal"); }
        }

        private string _EstatusCivilImputado;
        public string EstatusCivilImputado
        {
            get { return _EstatusCivilImputado; }
            set { _EstatusCivilImputado = value; OnPropertyChanged("EstatusCivilImputado"); }
        }
        private string _OriginarioImputado;

        public string OriginarioImputado
        {
            get { return _OriginarioImputado; }
            set { _OriginarioImputado = value; OnPropertyChanged("OriginarioImputado"); }
        }
        private short? _OcupacionAnteriorImputado;

        public short? OcupacionAnteriorImputado
        {
            get { return _OcupacionAnteriorImputado; }
            set { _OcupacionAnteriorImputado = value; OnPropertyChanged("OcupacionAnteriorImputado"); }
        }
        private short? _OcupacionActualImputado;

        public short? OcupacionActualImputado
        {
            get { return _OcupacionActualImputado; }
            set { _OcupacionActualImputado = value; OnPropertyChanged("OcupacionActualImputado"); }
        }

        private string _DescripcionDelitoMedicoFederal;
        public string DescripcionDelitoMedicoFederal
        {
            get { return _DescripcionDelitoMedicoFederal; }
            set { _DescripcionDelitoMedicoFederal = value; OnPropertyChanged("DescripcionDelitoMedicoFederal"); }
        }

        private string _SentenciaDelito;

        public string SentenciaDelito
        {
            get { return _SentenciaDelito; }
            set { _SentenciaDelito = value; OnPropertyChanged("SentenciaDelito"); }
        }
        private string _AntecedentesHeredoFam;

        public string AntecedentesHeredoFam
        {
            get { return _AntecedentesHeredoFam; }
            set { _AntecedentesHeredoFam = value; OnPropertyChanged("AntecedentesHeredoFam"); }
        }
        private string _AntecedenterPersonalesNoPato;

        public string AntecedenterPersonalesNoPato
        {
            get { return _AntecedenterPersonalesNoPato; }
            set { _AntecedenterPersonalesNoPato = value; OnPropertyChanged("AntecedenterPersonalesNoPato"); }
        }

        private string _DescripcionPadecMedicoFederal;

        public string DescripcionPadecMedicoFederal
        {
            get { return _DescripcionPadecMedicoFederal; }
            set { _DescripcionPadecMedicoFederal = value; OnPropertyChanged("DescripcionPadecMedicoFederal"); }
        }

        private string _DescripcionAntecedentesPatoMedicoFederal;
        public string DescripcionAntecedentesPatoMedicoFederal
        {
            get { return _DescripcionAntecedentesPatoMedicoFederal; }
            set { _DescripcionAntecedentesPatoMedicoFederal = value; OnPropertyChanged("DescripcionAntecedentesPatoMedicoFederal"); }
        }

        private string _InterrogatorioAparatosSist;

        public string InterrogatorioAparatosSist
        {
            get { return _InterrogatorioAparatosSist; }
            set { _InterrogatorioAparatosSist = value; OnPropertyChanged("InterrogatorioAparatosSist"); }
        }
        private string _ExploracionFisicaCabezaCuello;

        public string ExploracionFisicaCabezaCuello
        {
            get { return _ExploracionFisicaCabezaCuello; }
            set { _ExploracionFisicaCabezaCuello = value; OnPropertyChanged("ExploracionFisicaCabezaCuello"); }
        }

        private string _Extremidades;

        private string _Torax;

        public string Torax
        {
            get { return _Torax; }
            set { _Torax = value; OnPropertyChanged("Torax"); }
        }
        private string _Abdomen;

        public string Abdomen
        {
            get { return _Abdomen; }
            set { _Abdomen = value; OnPropertyChanged("Abdomen"); }
        }
        private string _OrganosGenit;

        public string OrganosGenit
        {
            get { return _OrganosGenit; }
            set { _OrganosGenit = value; OnPropertyChanged("OrganosGenit"); }
        }
        private string _Extemidades;

        public string Extemidades
        {
            get { return _Extemidades; }
            set { _Extemidades = value; OnPropertyChanged("Extemidades"); }
        }
        private string _DescripcionTatuajesCicatrRecientes;

        public string DescripcionTatuajesCicatrRecientes
        {
            get { return _DescripcionTatuajesCicatrRecientes; }
            set { _DescripcionTatuajesCicatrRecientes = value; OnPropertyChanged("DescripcionTatuajesCicatrRecientes"); }
        }
        private string _Diagnostico;

        public string Diagnostico
        {
            get { return _Diagnostico; }
            set { _Diagnostico = value; OnPropertyChanged("Diagnostico"); }
        }
        private string _TerapeuticaImplementadaYResult;

        public string TerapeuticaImplementadaYResult
        {
            get { return _TerapeuticaImplementadaYResult; }
            set { _TerapeuticaImplementadaYResult = value; OnPropertyChanged("TerapeuticaImplementadaYResult"); }
        }
        private string _Conclusion;

        public string Conclusion
        {
            get { return _Conclusion; }
            set { _Conclusion = value; OnPropertyChanged("Conclusion"); }
        }

        private DateTime? _FechaEstudioMedicoFederal;
        public DateTime? FechaEstudioMedicoFederal
        {
            get { return _FechaEstudioMedicoFederal; }
            set { _FechaEstudioMedicoFederal = value; OnPropertyChanged("FechaEstudioMedicoFederal"); }
        }

        #region Signos Vitales Federales
        private string _TensionArterialFed;
        public string TensionArterialFed
        {
            get { return _TensionArterialFed; }
            set { _TensionArterialFed = value; OnPropertyChanged("TensionArterialFed"); }
        }

        private string _TemperaturaFed;
        public string TemperaturaFed
        {
            get { return _TemperaturaFed; }
            set { _TemperaturaFed = value; OnPropertyChanged("TemperaturaFed"); }
        }

        private string _PulsoFed;
        public string PulsoFed
        {
            get { return _PulsoFed; }
            set { _PulsoFed = value; OnPropertyChanged("PulsoFed"); }
        }

        private string _RespiracionFed;
        public string RespiracionFed
        {
            get { return _RespiracionFed; }
            set { _RespiracionFed = value; OnPropertyChanged("RespiracionFed"); }
        }

        private string _EstaturaFed;
        public string EstaturaFed
        {
            get { return _EstaturaFed; }
            set { _EstaturaFed = value; OnPropertyChanged("EstaturaFed"); }
        }


        private ObservableCollection<PFF_SUSTANCIA_TOXICA> lstSustToxicas;
        public ObservableCollection<PFF_SUSTANCIA_TOXICA> LstSustToxicas
        {
            get { return lstSustToxicas; }
            set { lstSustToxicas = value; OnPropertyChanged("LstSustToxicas"); }
        }

        #endregion


        private bool _IsFarmacoDepenChecked = false;

        public bool IsFarmacoDepenChecked
        {
            get { return _IsFarmacoDepenChecked; }
            set { _IsFarmacoDepenChecked = value; OnPropertyChanged("IsFarmacoDepenChecked"); }
        }
        private bool _IsAlcohlicosAnonChecked = false;

        public bool IsAlcohlicosAnonChecked
        {
            get { return _IsAlcohlicosAnonChecked; }
            set { _IsAlcohlicosAnonChecked = value; OnPropertyChanged("IsAlcohlicosAnonChecked"); }
        }

        private bool _IsEnabledOtrosAsistenciaFederal = false;

        public bool IsEnabledOtrosAsistenciaFederal
        {
            get { return _IsEnabledOtrosAsistenciaFederal; }
            set { _IsEnabledOtrosAsistenciaFederal = value; OnPropertyChanged("IsEnabledOtrosAsistenciaFederal"); }
        }

        private bool _IsOtrosGruChecked = false;

        public bool IsOtrosGruChecked
        {
            get { return _IsOtrosGruChecked; }
            set
            {
                _IsOtrosGruChecked = value;
                if (value)
                {
                    base.RemoveRule("EspecifiqueOtraDependencia");
                    IsEnabledOtrosAsistenciaFederal = true;
                    base.AddRule(() => EspecifiqueOtraDependencia, () => !string.IsNullOrEmpty(EspecifiqueOtraDependencia), "OTRAS DEPENDENCIAS ES REQUERIDO!");
                }
                else
                {
                    IsEnabledOtrosAsistenciaFederal = false;
                    base.RemoveRule("EspecifiqueOtraDependencia");
                }

                OnPropertyChanged("IsEnabledOtrosAsistenciaFederal");
                OnPropertyChanged("IsOtrosGruChecked");
                OnPropertyChanged("EspecifiqueOtraDependencia");
            }
        }

        private string _EspecifiqueOtraDependencia;

        public string EspecifiqueOtraDependencia
        {
            get { return _EspecifiqueOtraDependencia; }
            set { _EspecifiqueOtraDependencia = value; OnPropertyChanged("EspecifiqueOtraDependencia"); }
        }

        #endregion

        #region Estudio Psicologico de fuero federal

        private string _LugarPsicoF;

        public string LugarPsicoF
        {
            get { return _LugarPsicoF; }
            set { _LugarPsicoF = value; OnPropertyChanged("LugarPsicoF"); }
        }

        private string _NombreImpFF;

        public string NombreImpFF
        {
            get { return _NombreImpFF; }
            set { _NombreImpFF = value; OnPropertyChanged("NombreImpFF"); }
        }

        private string _SobreNombFF;

        public string SobreNombFF
        {
            get { return _SobreNombFF; }
            set { _SobreNombFF = value; OnPropertyChanged("SobreNombFF"); }
        }

        private short? _EdadImpFF;

        public short? EdadImpFF
        {
            get { return _EdadImpFF; }
            set { _EdadImpFF = value; OnPropertyChanged("EdadImpFF"); }
        }

        private string _DelifoFFl;

        public string DelifoFFl
        {
            get { return _DelifoFFl; }
            set { _DelifoFFl = value; OnPropertyChanged("DelifoFFl"); }
        }

        private string _ExMentFF;

        public string ExMentFF
        {
            get { return _ExMentFF; }
            set { _ExMentFF = value; OnPropertyChanged("ExMentFF"); }
        }


        private string _ActitudTomadaEntrev;

        public string ActitudTomadaEntrev
        {
            get { return _ActitudTomadaEntrev; }
            set { _ActitudTomadaEntrev = value; OnPropertyChanged("ActitudTomadaEntrev"); }
        }
        private string _PruebasAplic;

        public string PruebasAplic
        {
            get { return _PruebasAplic; }
            set { _PruebasAplic = value; OnPropertyChanged("PruebasAplic"); }
        }
        private string _NivelIntelec;

        public string NivelIntelec
        {
            get { return _NivelIntelec; }
            set { _NivelIntelec = value; OnPropertyChanged("NivelIntelec"); }
        }
        private string _CoeficienteInt;

        public string CoeficienteInt
        {
            get { return _CoeficienteInt; }
            set { _CoeficienteInt = value; OnPropertyChanged("CoeficienteInt"); }
        }
        private string _IndiceLesionOrg;

        public string IndiceLesionOrg
        {
            get { return _IndiceLesionOrg; }
            set { _IndiceLesionOrg = value; OnPropertyChanged("IndiceLesionOrg"); }
        }
        private string _DinamicaPersonIngreso;

        public string DinamicaPersonIngreso
        {
            get { return _DinamicaPersonIngreso; }
            set { _DinamicaPersonIngreso = value; OnPropertyChanged("DinamicaPersonIngreso"); }
        }
        private string _DinamicaPersActual;

        public string DinamicaPersActual
        {
            get { return _DinamicaPersActual; }
            set { _DinamicaPersActual = value; OnPropertyChanged("DinamicaPersActual"); }
        }
        private string _resultTratPropArea;

        public string ResultTratPropArea
        {
            get { return _resultTratPropArea; }
            set { _resultTratPropArea = value; OnPropertyChanged("ResultTratPropArea"); }
        }

        private string _InternoFF;

        public string InternoFF
        {
            get { return _InternoFF; }
            set { _InternoFF = value; OnPropertyChanged("InternoFF"); }
        }

        private string _ExternoFF;

        public string ExternoFF
        {
            get { return _ExternoFF; }
            set { _ExternoFF = value; OnPropertyChanged("ExternoFF"); }
        }


        private string _EspecifiqueContTrat;

        public string EspecifiqueContTrat
        {
            get { return _EspecifiqueContTrat; }
            set { _EspecifiqueContTrat = value; OnPropertyChanged("EspecifiqueContTrat"); }
        }
        private string _PronosticoReinsercionSocial;

        public string PronosticoReinsercionSocial
        {
            get { return _PronosticoReinsercionSocial; }
            set { _PronosticoReinsercionSocial = value; OnPropertyChanged("PronosticoReinsercionSocial"); }
        }
        private string _OpinionSobreBeneficio;

        public string OpinionSobreBeneficio
        {
            get { return _OpinionSobreBeneficio; }
            set { _OpinionSobreBeneficio = value; OnPropertyChanged("OpinionSobreBeneficio"); }
        }

        private DateTime? _FecEstudioPsicoFF;

        public DateTime? FecEstudioPsicoFF
        {
            get { return _FecEstudioPsicoFF; }
            set { _FecEstudioPsicoFF = value; OnPropertyChanged("FecEstudioPsicoFF"); }
        }

        #endregion

        #region EstudioTrabajoSocial

        private ObservableCollection<ESCOLARIDAD> _LstEscolaridadesGrupoFamTSFF;

        public ObservableCollection<ESCOLARIDAD> LstEscolaridadesGrupoFamTSFF
        {
            get { return _LstEscolaridadesGrupoFamTSFF; }
            set { _LstEscolaridadesGrupoFamTSFF = value; OnPropertyChanged("LstEscolaridadesGrupoFamTSFF"); }
        }



        private string _NombreIntegranteTSFF;

        public string NombreIntegranteTSFF
        {
            get { return _NombreIntegranteTSFF; }
            set { _NombreIntegranteTSFF = value; OnPropertyChanged("NombreIntegranteTSFF"); }
        }
        private string _EdadIntegranteTSFF;

        public string EdadIntegranteTSFF
        {
            get { return _EdadIntegranteTSFF; }
            set { _EdadIntegranteTSFF = value; OnPropertyChanged("EdadIntegranteTSFF"); }
        }
        private string _ParentescoIntegranteTSFF;

        public string ParentescoIntegranteTSFF
        {
            get { return _ParentescoIntegranteTSFF; }
            set { _ParentescoIntegranteTSFF = value; OnPropertyChanged("ParentescoIntegranteTSFF"); }
        }
        private string _EdoCivilIntegranteTSFF;

        public string EdoCivilIntegranteTSFF
        {
            get { return _EdoCivilIntegranteTSFF; }
            set { _EdoCivilIntegranteTSFF = value; OnPropertyChanged("EdoCivilIntegranteTSFF"); }
        }
        private string _OcupacionIntegranteTSFF;

        public string OcupacionIntegranteTSFF
        {
            get { return _OcupacionIntegranteTSFF; }
            set { _OcupacionIntegranteTSFF = value; OnPropertyChanged("OcupacionIntegranteTSFF"); }
        }
        private short _IdEscIntegranteTSFF;

        public short IdEscIntegranteTSFF
        {
            get { return _IdEscIntegranteTSFF; }
            set { _IdEscIntegranteTSFF = value; OnPropertyChanged("IdEscIntegranteTSFF"); }
        }

        private short _IdTipoGrupo { get; set; }
        private ObservableCollection<PFF_GRUPO_FAMILIAR> lstGrupoFam;
        public ObservableCollection<PFF_GRUPO_FAMILIAR> LstGrupoFam
        {
            get { return lstGrupoFam; }
            set { lstGrupoFam = value; OnPropertyChanged("LstGrupoFam"); }
        }

        private ObservableCollection<PFF_GRUPO_FAMILIAR> lstGrupoFamSecu;
        public ObservableCollection<PFF_GRUPO_FAMILIAR> LstGrupoFamSecu
        {
            get { return lstGrupoFamSecu; }
            set { lstGrupoFamSecu = value; OnPropertyChanged("LstGrupoFamSecu"); }
        }
        private PFF_GRUPO_FAMILIAR _SelectedGrupoFamFFSec;

        public PFF_GRUPO_FAMILIAR SelectedGrupoFamFFSec
        {
            get { return _SelectedGrupoFamFFSec; }
            set { _SelectedGrupoFamFFSec = value; OnPropertyChanged("SelectedGrupoFamFFSec"); }
        }

        private PFF_GRUPO_FAMILIAR _SelectedGrupoFamTSFF;
        public PFF_GRUPO_FAMILIAR SelectedGrupoFamTSFF
        {
            get { return _SelectedGrupoFamTSFF; }
            set { _SelectedGrupoFamTSFF = value; OnPropertyChanged("SelectedGrupoFamTSFF"); }
        }

        private ObservableCollection<DIALECTO> lstDialectos;

        public ObservableCollection<DIALECTO> LstDialectos
        {
            get { return lstDialectos; }
            set { lstDialectos = value; OnPropertyChanged("LstDialectos"); }
        }

        private string _NombbreImptSocioFF;

        public string NombbreImptSocioFF
        {
            get { return _NombbreImptSocioFF; }
            set { _NombbreImptSocioFF = value; OnPropertyChanged("NombbreImptSocioFF"); }
        }

        private short? _Dialecto;

        public short? Dialecto
        {
            get { return _Dialecto; }
            set { _Dialecto = value; OnPropertyChanged("Dialecto"); }
        }
        private string _LugarFechaNac;

        public string LugarFechaNac
        {
            get { return _LugarFechaNac; }
            set { _LugarFechaNac = value; OnPropertyChanged("LugarFechaNac"); }
        }
        private short? _EscolaridadIngreso;

        public short? EscolaridadIngreso
        {
            get { return _EscolaridadIngreso; }
            set { _EscolaridadIngreso = value; OnPropertyChanged("EscolaridadIngreso"); }
        }

        private string _EdoCivilTrabajoSocFF;

        public string EdoCivilTrabajoSocFF
        {
            get { return _EdoCivilTrabajoSocFF; }
            set { _EdoCivilTrabajoSocFF = value; OnPropertyChanged("EdoCivilTrabajoSocFF"); }
        }

        private short? _IdEdoCivilTSFF;
        public short? IdEdoCivilTSFF
        {
            get { return _IdEdoCivilTSFF; }
            set { _IdEdoCivilTSFF = value; OnPropertyChanged("IdEdoCivilTSFF"); }
        }

        private ObservableCollection<ESTADO_CIVIL> listEstadoCivil;
        public ObservableCollection<ESTADO_CIVIL> ListEstadoCivil
        {
            get { return listEstadoCivil; }
            set { listEstadoCivil = value; OnPropertyChanged("ListEstadoCivil"); }
        }



        private short? _EscolaridadActual;

        public short? EscolaridadActual
        {
            get { return _EscolaridadActual; }
            set { _EscolaridadActual = value; OnPropertyChanged("EscolaridadActual"); }
        }
        private short? _OcupacionAntesIngreso;

        public short? OcupacionAntesIngreso
        {
            get { return _OcupacionAntesIngreso; }
            set { _OcupacionAntesIngreso = value; OnPropertyChanged("OcupacionAntesIngreso"); }
        }
        private string _Domicilio;

        public string Domicilio
        {
            get { return _Domicilio; }
            set { _Domicilio = value; OnPropertyChanged("Domicilio"); }
        }
        private string _IdCaractZona;

        public string IdCaractZona
        {
            get { return _IdCaractZona; }
            set { _IdCaractZona = value; OnPropertyChanged("IdCaractZona"); }
        }
        private string _ResponsableManutHogar;

        public string ResponsableManutHogar
        {
            get { return _ResponsableManutHogar; }
            set { _ResponsableManutHogar = value; OnPropertyChanged("ResponsableManutHogar"); }
        }
        private int? _TotalIngresosMensuales;

        public int? TotalIngresosMensuales
        {
            get { return _TotalIngresosMensuales; }
            set { _TotalIngresosMensuales = value; OnPropertyChanged("TotalIngresosMensuales"); }
        }
        private int? _TotalEgresosMensuales;

        public int? TotalEgresosMensuales
        {
            get { return _TotalEgresosMensuales; }
            set { _TotalEgresosMensuales = value; OnPropertyChanged("TotalEgresosMensuales"); }
        }
        private string _IdActualmenteCooperaEconoConFamilia;

        public string IdActualmenteCooperaEconoConFamilia
        {
            get { return _IdActualmenteCooperaEconoConFamilia; }
            set { _IdActualmenteCooperaEconoConFamilia = value; OnPropertyChanged("IdActualmenteCooperaEconoConFamilia"); }
        }
        private string _TieneFondosAhorro;

        public string TieneFondosAhorro
        {
            get { return _TieneFondosAhorro; }
            set { _TieneFondosAhorro = value; OnPropertyChanged("TieneFondosAhorro"); }
        }
        private string _IdGrupoFamiliarPrim;

        public string IdGrupoFamiliarPrim
        {
            get { return _IdGrupoFamiliarPrim; }
            set { _IdGrupoFamiliarPrim = value; OnPropertyChanged("IdGrupoFamiliarPrim"); }
        }
        private string _IdRelacionesInterf;

        public string IdRelacionesInterf
        {
            get { return _IdRelacionesInterf; }
            set { _IdRelacionesInterf = value; OnPropertyChanged("IdRelacionesInterf"); }
        }

        private string _EspecifiqueViolenciaIntro;

        public string EspecifiqueViolenciaIntro
        {
            get { return _EspecifiqueViolenciaIntro; }
            set { _EspecifiqueViolenciaIntro = value; OnPropertyChanged("EspecifiqueViolenciaIntro"); }
        }
        private string _IdNivelSocioEconoPrim;

        public string IdNivelSocioEconoPrim
        {
            get { return _IdNivelSocioEconoPrim; }
            set { _IdNivelSocioEconoPrim = value; OnPropertyChanged("IdNivelSocioEconoPrim"); }
        }
        private string _ConceptoTieneFamiliaInterno;

        public string ConceptoTieneFamiliaInterno
        {
            get { return _ConceptoTieneFamiliaInterno; }
            set { _ConceptoTieneFamiliaInterno = value; OnPropertyChanged("ConceptoTieneFamiliaInterno"); }
        }
        private string _CantidadHijosUnionesAnteriores;

        public string CantidadHijosUnionesAnteriores
        {
            get { return _CantidadHijosUnionesAnteriores; }
            set { _CantidadHijosUnionesAnteriores = value; OnPropertyChanged("CantidadHijosUnionesAnteriores"); }
        }
        private string _IdGrupoFamiliarSecundario;

        public string IdGrupoFamiliarSecundario
        {
            get { return _IdGrupoFamiliarSecundario; }
            set { _IdGrupoFamiliarSecundario = value; OnPropertyChanged("IdGrupoFamiliarSecundario"); }
        }
        private string _IdRelacionInterFamSec;

        public string IdRelacionInterFamSec
        {
            get { return _IdRelacionInterFamSec; }
            set { _IdRelacionInterFamSec = value; OnPropertyChanged("IdRelacionInterFamSec"); }
        }

        private string _IdNivelSocioEconCulturalSec;

        public string IdNivelSocioEconCulturalSec
        {
            get { return _IdNivelSocioEconCulturalSec; }
            set { _IdNivelSocioEconCulturalSec = value; OnPropertyChanged("IdNivelSocioEconCulturalSec"); }
        }
        private short? _CantidadHabitacionesTotal;

        public short? CantidadHabitacionesTotal
        {
            get { return _CantidadHabitacionesTotal; }
            set { _CantidadHabitacionesTotal = value; OnPropertyChanged("CantidadHabitacionesTotal"); }
        }
        private string _DescripcionVivienda;

        public string DescripcionVivienda
        {
            get { return _DescripcionVivienda; }
            set { _DescripcionVivienda = value; OnPropertyChanged("DescripcionVivienda"); }
        }
        private string _TransporteCercaVivienda;

        public string TransporteCercaVivienda
        {
            get { return _TransporteCercaVivienda; }
            set { _TransporteCercaVivienda = value; OnPropertyChanged("TransporteCercaVivienda"); }
        }
        private string _EnseresMobiliarioDomestico;

        public string EnseresMobiliarioDomestico
        {
            get { return _EnseresMobiliarioDomestico; }
            set { _EnseresMobiliarioDomestico = value; OnPropertyChanged("EnseresMobiliarioDomestico"); }
        }
        private string _IdCaracZona;

        public string IdCaracZona
        {
            get { return _IdCaracZona; }
            set { _IdCaracZona = value; OnPropertyChanged("IdCaracZona"); }
        }

        private string _IdCaracZonaSec;

        public string IdCaracZonaSec
        {
            get { return _IdCaracZonaSec; }
            set { _IdCaracZonaSec = value; OnPropertyChanged("IdCaracZonaSec"); }
        }

        private string _RelacionMedioExterno;

        public string RelacionMedioExterno
        {
            get { return _RelacionMedioExterno; }
            set { _RelacionMedioExterno = value; OnPropertyChanged("RelacionMedioExterno"); }
        }
        private string _NumeroParejasVividoManeraEstable;

        public string NumeroParejasVividoManeraEstable
        {
            get { return _NumeroParejasVividoManeraEstable; }
            set { _NumeroParejasVividoManeraEstable = value; OnPropertyChanged("NumeroParejasVividoManeraEstable"); }
        }
        private string _TrabajoDesempeniadoAntesReclusion;

        public string TrabajoDesempeniadoAntesReclusion
        {
            get { return _TrabajoDesempeniadoAntesReclusion; }
            set { _TrabajoDesempeniadoAntesReclusion = value; OnPropertyChanged("TrabajoDesempeniadoAntesReclusion"); }
        }
        private string _TiempoLaborar;

        public string TiempoLaborar
        {
            get { return _TiempoLaborar; }
            set { _TiempoLaborar = value; OnPropertyChanged("TiempoLaborar"); }
        }
        private int? _SueldoPercibido;

        public int? SueldoPercibido
        {
            get { return _SueldoPercibido; }
            set { _SueldoPercibido = value; OnPropertyChanged("SueldoPercibido"); }
        }
        private string _DescripcionAportacionesEconomicasQuienCuanto;

        public string DescripcionAportacionesEconomicasQuienCuanto
        {
            get { return _DescripcionAportacionesEconomicasQuienCuanto; }
            set { _DescripcionAportacionesEconomicasQuienCuanto = value; OnPropertyChanged("DescripcionAportacionesEconomicasQuienCuanto"); }
        }
        private string _DsitribucionGastoFamiliar;

        public string DsitribucionGastoFamiliar
        {
            get { return _DsitribucionGastoFamiliar; }
            set { _DsitribucionGastoFamiliar = value; OnPropertyChanged("DsitribucionGastoFamiliar"); }
        }
        private string _DescripcionGastoFamiliar;

        public string DescripcionGastoFamiliar
        {
            get { return _DescripcionGastoFamiliar; }
            set { _DescripcionGastoFamiliar = value; OnPropertyChanged("DescripcionGastoFamiliar"); }
        }
        private string _DescripcionAlimentacionFamiliar;

        public string DescripcionAlimentacionFamiliar
        {
            get { return _DescripcionAlimentacionFamiliar; }
            set { _DescripcionAlimentacionFamiliar = value; OnPropertyChanged("DescripcionAlimentacionFamiliar"); }
        }
        private string _DescripcionServiciosCuenta;

        public string DescripcionServiciosCuenta
        {
            get { return _DescripcionServiciosCuenta; }
            set { _DescripcionServiciosCuenta = value; OnPropertyChanged("DescripcionServiciosCuenta"); }
        }

        private string _IdCuentaApoyoFamiliaOtraPersona;

        public string IdCuentaApoyoFamiliaOtraPersona
        {
            get { return _IdCuentaApoyoFamiliaOtraPersona; }
            set { _IdCuentaApoyoFamiliaOtraPersona = value; OnPropertyChanged("IdCuentaApoyoFamiliaOtraPersona"); }
        }
        private string _NombreAvalMoral;

        public string NombreAvalMoral
        {
            get { return _NombreAvalMoral; }
            set { _NombreAvalMoral = value; OnPropertyChanged("NombreAvalMoral"); }
        }
        private string _IdParentescoAval;

        public string IdParentescoAval
        {
            get { return _IdParentescoAval; }
            set { _IdParentescoAval = value; OnPropertyChanged("IdParentescoAval"); }
        }
        private string _ConQuienViviraSerExternado;

        public string ConQuienViviraSerExternado
        {
            get { return _ConQuienViviraSerExternado; }
            set { _ConQuienViviraSerExternado = value; OnPropertyChanged("ConQuienViviraSerExternado"); }
        }
        private string _CalleQuienViviraSerExternado;

        public string CalleQuienViviraSerExternado
        {
            get { return _CalleQuienViviraSerExternado; }
            set { _CalleQuienViviraSerExternado = value; OnPropertyChanged("CalleQuienViviraSerExternado"); }
        }
        private string _NoQuienViviraSerExternado;

        public string NoQuienViviraSerExternado
        {
            get { return _NoQuienViviraSerExternado; }
            set { _NoQuienViviraSerExternado = value; OnPropertyChanged("NoQuienViviraSerExternado"); }
        }
        private short _IdColoniaQuienViviraSerExternado;

        public short IdColoniaQuienViviraSerExternado
        {
            get { return _IdColoniaQuienViviraSerExternado; }
            set { _IdColoniaQuienViviraSerExternado = value; OnPropertyChanged("IdColoniaQuienViviraSerExternado"); }
        }
        private string _CodPosQuienViviraSerExternado;

        public string CodPosQuienViviraSerExternado
        {
            get { return _CodPosQuienViviraSerExternado; }
            set { _CodPosQuienViviraSerExternado = value; OnPropertyChanged("CodPosQuienViviraSerExternado"); }
        }
        private short _IdMunicipioQuienViviraSerExternado;

        public short IdMunicipioQuienViviraSerExternado
        {
            get { return _IdMunicipioQuienViviraSerExternado; }
            set { _IdMunicipioQuienViviraSerExternado = value; OnPropertyChanged("IdMunicipioQuienViviraSerExternado"); }
        }
        private short _IdCiudadQuienViviraSerExternado;

        public short IdCiudadQuienViviraSerExternado
        {
            get { return _IdCiudadQuienViviraSerExternado; }
            set { _IdCiudadQuienViviraSerExternado = value; OnPropertyChanged("IdCiudadQuienViviraSerExternado"); }
        }
        private short _IdEntidadFedQuienViviraSerExternado;

        public short IdEntidadFedQuienViviraSerExternado
        {
            get { return _IdEntidadFedQuienViviraSerExternado; }
            set { _IdEntidadFedQuienViviraSerExternado = value; OnPropertyChanged("IdEntidadFedQuienViviraSerExternado"); }
        }
        private short? _IdParentescoQuienViviraSerExternado;

        public short? IdParentescoQuienViviraSerExternado
        {
            get { return _IdParentescoQuienViviraSerExternado; }
            set { _IdParentescoQuienViviraSerExternado = value; OnPropertyChanged("IdParentescoQuienViviraSerExternado"); }
        }
        private string _OpicionAcercaInternamiento;

        public string OpicionAcercaInternamiento
        {
            get { return _OpicionAcercaInternamiento; }
            set { _OpicionAcercaInternamiento = value; OnPropertyChanged("OpicionAcercaInternamiento"); }
        }
        private string _DeQueManeraLeInfluenciaEstanciaPrision;

        public string DeQueManeraLeInfluenciaEstanciaPrision
        {
            get { return _DeQueManeraLeInfluenciaEstanciaPrision; }
            set { _DeQueManeraLeInfluenciaEstanciaPrision = value; OnPropertyChanged("DeQueManeraLeInfluenciaEstanciaPrision"); }
        }
        private string _DiagnosticoPronosticoExternacion;

        public string DiagnosticoPronosticoExternacion
        {
            get { return _DiagnosticoPronosticoExternacion; }
            set { _DiagnosticoPronosticoExternacion = value; OnPropertyChanged("DiagnosticoPronosticoExternacion"); }
        }
        private string _OpinionConcesionBeneficio;

        public string OpinionConcesionBeneficio
        {
            get { return _OpinionConcesionBeneficio; }
            set { _OpinionConcesionBeneficio = value; OnPropertyChanged("OpinionConcesionBeneficio"); }
        }

        private string _NombreCalleExternado;

        public string NombreCalleExternado
        {
            get { return _NombreCalleExternado; }
            set { _NombreCalleExternado = value; OnPropertyChanged("NombreCalleExternado"); }
        }

        private DateTime? _FecEstudioSocioFF;

        public DateTime? FecEstudioSocioFF
        {
            get { return _FecEstudioSocioFF; }
            set { _FecEstudioSocioFF = value; OnPropertyChanged("FecEstudioSocioFF"); }
        }


        private ObservableCollection<TIPO_REFERENCIA> lstParentescos;
        public ObservableCollection<TIPO_REFERENCIA> LstParentescos
        {
            get { return lstParentescos; }
            set { lstParentescos = value; OnPropertyChanged("LstParentescos"); }
        }

        private ObservableCollection<COLONIA> listItems;
        public ObservableCollection<COLONIA> ListItems
        {
            get { return listItems; }
            set { listItems = value; OnPropertyChanged("ListItems"); }
        }
        private ObservableCollection<PAIS_NACIONALIDAD> lstPaises;
        public ObservableCollection<PAIS_NACIONALIDAD> LstPaises
        {
            get { return lstPaises; }
            set { lstPaises = value; OnPropertyChanged("LstPaises"); }
        }
        private ObservableCollection<ENTIDAD> lstEntidades;
        public ObservableCollection<ENTIDAD> LstEntidades
        {
            get { return lstEntidades; }
            set { lstEntidades = value; OnPropertyChanged("LstEntidades"); }
        }
        private ObservableCollection<MUNICIPIO> lstMunicipios;
        public ObservableCollection<MUNICIPIO> LstMunicipios
        {
            get { return lstMunicipios; }
            set { lstMunicipios = value; OnPropertyChanged("LstMunicipios"); }
        }

        private ObservableCollection<COLONIA> listColonia;
        public ObservableCollection<COLONIA> ListColonia
        {
            get { return listColonia; }
            set { listColonia = value; OnPropertyChanged("ListColonia"); }
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

        private MUNICIPIO selectedMunicipio;
        public MUNICIPIO SelectedMunicipio
        {
            get { return selectedMunicipio; }
            set { selectedMunicipio = value; OnPropertyChanged("SelectedMunicipio"); }
        }

        private short? _IdParentescoVisTSFF;

        public short? IdParentescoVisTSFF
        {
            get { return _IdParentescoVisTSFF; }
            set { _IdParentescoVisTSFF = value; OnPropertyChanged("IdParentescoVisTSFF"); }
        }

        private short? _IdParentAvalTSFF;
        public short? IdParentAvalTSFF
        {
            get { return _IdParentAvalTSFF; }
            set { _IdParentAvalTSFF = value; OnPropertyChanged("IdParentAvalTSFF"); }
        }

        private short? _IdParentExternadoTSFF;

        public short? IdParentExternadoTSFF
        {
            get { return _IdParentExternadoTSFF; }
            set { _IdParentExternadoTSFF = value; OnPropertyChanged("IdParentExternadoTSFF"); }
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
                    {
                        if (value.ID_PAIS_NAC == 82)//MEXICO
                        {
                            LstEntidades = new ObservableCollection<ENTIDAD>(value.ENTIDAD);
                            LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                            EEstado = -1;
                            OnPropertyChanged("EEstado");
                            LstMunicipios = new ObservableCollection<MUNICIPIO>();
                            LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                            EMunicipio = -1;
                            OnPropertyChanged("EMunicipio");
                        }
                        else//EXTRANJERO
                        {
                            LstEntidades = new ObservableCollection<ENTIDAD>();
                            LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "EXTRANJERO" });
                            EEstado = -1;
                            OnPropertyChanged("EEstado");
                            LstMunicipios = new ObservableCollection<MUNICIPIO>();
                            LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "EXTRANJERO" });
                            EMunicipio = -1;
                            OnPropertyChanged("EMunicipio");
                        }
                    }
                    else
                    {
                        LstEntidades = new ObservableCollection<ENTIDAD>();
                        LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                        EEstado = -1;
                        OnPropertyChanged("EEstado");
                        LstMunicipios = new ObservableCollection<MUNICIPIO>();
                        LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                        EMunicipio = -1;
                        OnPropertyChanged("EMunicipio");
                    }
                }
                else
                {
                    LstEntidades = new ObservableCollection<ENTIDAD>();
                    LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                    EEstado = -1;
                    OnPropertyChanged("EEstado");
                    LstMunicipios = new ObservableCollection<MUNICIPIO>();
                    LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    EMunicipio = -1;
                    OnPropertyChanged("EMunicipio");
                }

                OnPropertyChanged("SelectedPais");
            }
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
                    LstMunicipios = new ObservableCollection<MUNICIPIO>(value.MUNICIPIO);
                    LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    //EMunicipio = -1;
                }
                else
                {
                    LstMunicipios = new ObservableCollection<MUNICIPIO>();
                    LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    EMunicipio = -1;
                }

                OnPropertyChanged("SelectedEstado");
            }
        }

        #endregion

        #region Informe de Actividades Productivas de Capacitacion
        private string _NombreInternoCpaFF;

        public string NombreInternoCpaFF
        {
            get { return _NombreInternoCpaFF; }
            set { _NombreInternoCpaFF = value; OnPropertyChanged("NombreInternoCpaFF"); }
        }

        private string _SeccionInterno;
        public string SeccionInterno
        {
            get { return _SeccionInterno; }
            set { _SeccionInterno = value; OnPropertyChanged("SeccionInterno"); }
        }
        private short? _OficioActivDesempenadaAntesR;

        public short? OficioActivDesempenadaAntesR
        {
            get { return _OficioActivDesempenadaAntesR; }
            set { _OficioActivDesempenadaAntesR = value; OnPropertyChanged("OficioActivDesempenadaAntesR"); }
        }

        private ObservableCollection<PFF_CAPACITACION_CURSO> lstCursosCapacitacionFederal;

        public ObservableCollection<PFF_CAPACITACION_CURSO> LstCursosCapacitacionFederal
        {
            get { return lstCursosCapacitacionFederal; }
            set { lstCursosCapacitacionFederal = value; OnPropertyChanged("LstCursosCapacitacionFederal"); }
        }

        private PFF_CAPACITACION_CURSO _SelectedCursoCapacitacionFederal;

        public PFF_CAPACITACION_CURSO SelectedCursoCapacitacionFederal
        {
            get { return _SelectedCursoCapacitacionFederal; }
            set { _SelectedCursoCapacitacionFederal = value; OnPropertyChanged("SelectedCursoCapacitacionFederal"); }
        }

        private short TotalPreliminar { get; set; }

        private int? _SalarioPercib;

        public int? SalarioPercib
        {
            get { return _SalarioPercib; }
            set { _SalarioPercib = value; OnPropertyChanged("SalarioPercib"); }
        }
        private string _ActivEnCentroActual;

        public string ActivEnCentroActual
        {
            get { return _ActivEnCentroActual; }
            set { _ActivEnCentroActual = value; OnPropertyChanged("ActivEnCentroActual"); }
        }
        private string _IdAtiendeIndicacionesSup;

        public string IdAtiendeIndicacionesSup
        {
            get { return _IdAtiendeIndicacionesSup; }
            set { _IdAtiendeIndicacionesSup = value; OnPropertyChanged("IdAtiendeIndicacionesSup"); }
        }
        private string _IdSatisfaceActiv;

        public string IdSatisfaceActiv
        {
            get { return _IdSatisfaceActiv; }
            set { _IdSatisfaceActiv = value; OnPropertyChanged("IdSatisfaceActiv"); }
        }
        private string _IdDescuidadoCumplimientoLab;

        public string IdDescuidadoCumplimientoLab
        {
            get { return _IdDescuidadoCumplimientoLab; }
            set { _IdDescuidadoCumplimientoLab = value; OnPropertyChanged("IdDescuidadoCumplimientoLab"); }
        }
        private string _MotivosTiempoInterrupcionesActividad;

        public string MotivosTiempoInterrupcionesActividad
        {
            get { return _MotivosTiempoInterrupcionesActividad; }
            set { _MotivosTiempoInterrupcionesActividad = value; OnPropertyChanged("MotivosTiempoInterrupcionesActividad"); }
        }
        private string _IdRecibioConstancia;

        public string IdRecibioConstancia
        {
            get { return _IdRecibioConstancia; }
            set { _IdRecibioConstancia = value; OnPropertyChanged("IdRecibioConstancia"); }
        }
        private string _EspecifiqueNoCursos;

        public string EspecifiqueNoCursos
        {
            get { return _EspecifiqueNoCursos; }
            set { _EspecifiqueNoCursos = value; OnPropertyChanged("EspecifiqueNoCursos"); }
        }
        private string _IdCambiadoActiv;

        public string IdCambiadoActiv
        {
            get { return _IdCambiadoActiv; }
            set { _IdCambiadoActiv = value; OnPropertyChanged("IdCambiadoActiv"); }
        }
        private string _ExpecifiqueCambioAct;

        public string ExpecifiqueCambioAct
        {
            get { return _ExpecifiqueCambioAct; }
            set { _ExpecifiqueCambioAct = value; OnPropertyChanged("ExpecifiqueCambioAct"); }
        }
        private string _IdProgresoOficio;

        public string IdProgresoOficio
        {
            get { return _IdProgresoOficio; }
            set { _IdProgresoOficio = value; OnPropertyChanged("IdProgresoOficio"); }
        }
        private string _ActitudesHaciaDesempenioActivProd;

        public string ActitudesHaciaDesempenioActivProd
        {
            get { return _ActitudesHaciaDesempenioActivProd; }
            set { _ActitudesHaciaDesempenioActivProd = value; OnPropertyChanged("ActitudesHaciaDesempenioActivProd"); }
        }
        private string _IdCuentaFondoA;

        public string IdCuentaFondoA
        {
            get { return _IdCuentaFondoA; }
            set 
            {
                _IdCuentaFondoA = value; 
                if(!string.IsNullOrEmpty(value))
                    if (value == "S")
                    {
                        base.RemoveRule("EspecifiqueCompensacion");
                        EnabledEspecifiqueCompensacionFederal = true;
                        base.AddRule(() => EspecifiqueCompensacion, () => !string.IsNullOrEmpty(EspecifiqueCompensacion), "ESPECIFIQUE COMPENSACION ES REQUERIDO!");
                    }
                    else
                    {
                        EnabledEspecifiqueCompensacionFederal = false;
                        base.RemoveRule("EspecifiqueCompensacion");
                    }

                OnPropertyChanged("EnabledEspecifiqueCompensacionFederal");
                OnPropertyChanged("EspecifiqueCompensacion");
                OnPropertyChanged("IdCuentaFondoA");
            }
        }

        private bool _EnabledEspecifiqueCompensacionFederal = false;

        public bool EnabledEspecifiqueCompensacionFederal
        {
            get { return _EnabledEspecifiqueCompensacionFederal; }
            set { _EnabledEspecifiqueCompensacionFederal = value; OnPropertyChanged("EnabledEspecifiqueCompensacionFederal"); }
        }
        private string _EspecifiqueCompensacion;

        public string EspecifiqueCompensacion
        {
            get { return _EspecifiqueCompensacion; }
            set { _EspecifiqueCompensacion = value; OnPropertyChanged("EspecifiqueCompensacion"); }
        }
        private short _DiasLaboradosEfectivos;

        public short DiasLaboradosEfectivos
        {
            get { return _DiasLaboradosEfectivos; }
            set { _DiasLaboradosEfectivos = value; OnPropertyChanged("DiasLaboradosEfectivos"); }
        }
        private int? _DiasOtrosCentros;

        public int? DiasOtrosCentros
        {
            get { return _DiasOtrosCentros; }
            set { _DiasOtrosCentros = value; OnPropertyChanged("DiasOtrosCentros"); }
        }
        private int? _DiasTotalAB;

        public int? DiasTotalAB
        {
            get { return _DiasTotalAB; }
            set { _DiasTotalAB = value; OnPropertyChanged("DiasTotalAB"); }
        }
        private string _ConclusionesActivProdCapac;

        public string ConclusionesActivProdCapac
        {
            get { return _ConclusionesActivProdCapac; }
            set { _ConclusionesActivProdCapac = value; OnPropertyChanged("ConclusionesActivProdCapac"); }
        }

        private DateTime? _FecCapacitFF;

        public DateTime? FecCapacitFF
        {
            get { return _FecCapacitFF; }
            set { _FecCapacitFF = value; OnPropertyChanged("FecCapacitFF"); }
        }

        #endregion

        #region Informe Actividades educativas, culturales, deportivas, recreativas y civicas
        private string _NombreImpInfActivFF;
        public string NombreImpInfActivFF
        {
            get { return _NombreImpInfActivFF; }
            set { _NombreImpInfActivFF = value; OnPropertyChanged("NombreImpInfActivFF"); }
        }

        private short? _EscolaridadMomentoDetencion;

        public short? EscolaridadMomentoDetencion
        {
            get { return _EscolaridadMomentoDetencion; }
            set { _EscolaridadMomentoDetencion = value; OnPropertyChanged("EscolaridadMomentoDetencion"); }
        }
        private string _EstudiosRealizadosInternamiento;

        public string EstudiosRealizadosInternamiento
        {
            get { return _EstudiosRealizadosInternamiento; }
            set { _EstudiosRealizadosInternamiento = value; OnPropertyChanged("EstudiosRealizadosInternamiento"); }
        }
        private string _EstudiosCursaActualmente;

        public string EstudiosCursaActualmente
        {
            get { return _EstudiosCursaActualmente; }
            set { _EstudiosCursaActualmente = value; OnPropertyChanged("EstudiosCursaActualmente"); }
        }
        private string _IdAsisteEscuelaVountariamente;

        public string IdAsisteEscuelaVountariamente
        {
            get { return _IdAsisteEscuelaVountariamente; }
            set 
            {
                _IdAsisteEscuelaVountariamente = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "N")
                    {
                        base.RemoveRule("EspecifiqueNoAsisteEscuelaVoluntariamente");
                        EnabledEspecificaVoluntad = true;
                        base.AddRule(() => EspecifiqueNoAsisteEscuelaVoluntariamente, () => !string.IsNullOrEmpty(EspecifiqueNoAsisteEscuelaVoluntariamente), "ESPECIFIQUE PORQUE NO ASISTE ES REQUERIDO!");
                        OnPropertyChanged("EspecifiqueNoAsisteEscuelaVoluntariamente");
                    }
                    else
                    {
                        base.RemoveRule("EspecifiqueNoAsisteEscuelaVoluntariamente");
                        EnabledEspecificaVoluntad = false;
                        OnPropertyChanged("EspecifiqueNoAsisteEscuelaVoluntariamente");
                    }
                }

                OnPropertyChanged("EspecifiqueNoAsisteEscuelaVoluntariamente");
                OnPropertyChanged("EnabledEspecificaVoluntad");
                OnPropertyChanged("IdAsisteEscuelaVountariamente");
            }
        }

        private bool _EnabledEspecificaVoluntad = false;

        public bool EnabledEspecificaVoluntad
        {
            get { return _EnabledEspecificaVoluntad; }
            set { _EnabledEspecificaVoluntad = value; OnPropertyChanged("EnabledEspecificaVoluntad"); }
        }
        private string _EspecifiqueNoAsisteEscuelaVoluntariamente;

        public string EspecifiqueNoAsisteEscuelaVoluntariamente
        {
            get { return _EspecifiqueNoAsisteEscuelaVoluntariamente; }
            set { _EspecifiqueNoAsisteEscuelaVoluntariamente = value; OnPropertyChanged("EspecifiqueNoAsisteEscuelaVoluntariamente"); }
        }
        private string _AvanceYRendimientoAcademico;

        public string AvanceYRendimientoAcademico
        {
            get { return _AvanceYRendimientoAcademico; }
            set { _AvanceYRendimientoAcademico = value; OnPropertyChanged("AvanceYRendimientoAcademico"); }
        }
        private string _QueOtraEnsenanaRecibe;

        public string QueOtraEnsenanaRecibe
        {
            get { return _QueOtraEnsenanaRecibe; }
            set { _QueOtraEnsenanaRecibe = value; OnPropertyChanged("QueOtraEnsenanaRecibe"); }
        }

        private string _ConclusionAcadem;

        public string ConclusionAcadem
        {
            get { return _ConclusionAcadem; }
            set { _ConclusionAcadem = value; OnPropertyChanged("ConclusionAcadem"); }
        }

        private DateTime? _FecEstudioInformeActivFF;

        public DateTime? FecEstudioInformeActivFF
        {
            get { return _FecEstudioInformeActivFF; }
            set { _FecEstudioInformeActivFF = value; OnPropertyChanged("FecEstudioInformeActivFF"); }
        }

        #endregion

        #region Informe Seccion Vigilancia
        private DateTime? _FecVigiSancFF;

        public DateTime? FecVigiSancFF
        {
            get { return _FecVigiSancFF; }
            set { _FecVigiSancFF = value; }
        }

        private string _MotivoSancFF;

        public string MotivoSancFF
        {
            get { return _MotivoSancFF; }
            set { _MotivoSancFF = value; OnPropertyChanged("MotivoSancFF"); }
        }
        private string _ResolucionSancFF;

        public string ResolucionSancFF
        {
            get { return _ResolucionSancFF; }
            set { _ResolucionSancFF = value; OnPropertyChanged("ResolucionSancFF"); }
        }

        private ObservableCollection<SANCION> _LstSancFF;

        public ObservableCollection<SANCION> LstSancFF
        {
            get { return _LstSancFF; }
            set { _LstSancFF = value; OnPropertyChanged("LstSancFF"); }
        }

        private SANCION _SelectedSancFF;

        public SANCION SelectedSancFF
        {
            get { return _SelectedSancFF; }
            set { _SelectedSancFF = value; OnPropertyChanged("SelectedSancFF"); }
        }

        private ObservableCollection<PFF_CORRECTIVO> lstCorrectivosFF;

        public ObservableCollection<PFF_CORRECTIVO> LstCorrectivosFF
        {
            get { return lstCorrectivosFF; }
            set { lstCorrectivosFF = value; OnPropertyChanged("LstCorrectivosFF"); }
        }

        private PFF_CORRECTIVO _SeleCorrecFF;

        public PFF_CORRECTIVO SeleCorrecFF
        {
            get { return _SeleCorrecFF; }
            set { _SeleCorrecFF = value; OnPropertyChanged("SeleCorrecFF"); }
        }

        private string _NombreImpVigilanciaFF;

        public string NombreImpVigilanciaFF
        {
            get { return _NombreImpVigilanciaFF; }
            set { _NombreImpVigilanciaFF = value; OnPropertyChanged("NombreImpVigilanciaFF"); }
        }

        private DateTime? _FecIngresoImputado;

        public DateTime? FecIngresoImputado
        {
            get { return _FecIngresoImputado; }
            set { _FecIngresoImputado = value; OnPropertyChanged("FecIngresoImputado"); }
        }
        private string _NombreCentroProcede;

        public string NombreCentroProcede
        {
            get { return _NombreCentroProcede; }
            set { _NombreCentroProcede = value; OnPropertyChanged("NombreCentroProcede"); }
        }
        private string _IdConducta;

        public string IdConducta
        {
            get { return _IdConducta; }
            set { _IdConducta = value; OnPropertyChanged("IdConducta"); }
        }
        private string _MotivoTraslado;

        public string MotivoTraslado
        {
            get { return _MotivoTraslado; }
            set { _MotivoTraslado = value; OnPropertyChanged("MotivoTraslado"); }
        }
        private string _IdConductaSuperiores;

        public string IdConductaSuperiores
        {
            get { return _IdConductaSuperiores; }
            set { _IdConductaSuperiores = value; OnPropertyChanged("IdConductaSuperiores"); }
        }
        private string _RelacionCompaneros;

        public string RelacionCompaneros
        {
            get { return _RelacionCompaneros; }
            set { _RelacionCompaneros = value; OnPropertyChanged("RelacionCompaneros"); }
        }
        private string _DescripcionConducta;

        public string DescripcionConducta
        {
            get { return _DescripcionConducta; }
            set { _DescripcionConducta = value; OnPropertyChanged("DescripcionConducta"); }
        }
        private string _IdHigienePersonal;

        public string IdHigienePersonal
        {
            get { return _IdHigienePersonal; }
            set { _IdHigienePersonal = value; OnPropertyChanged("IdHigienePersonal"); }
        }
        private string _IdHigieneEnCelda;

        public string IdHigieneEnCelda
        {
            get { return _IdHigieneEnCelda; }
            set { _IdHigieneEnCelda = value; OnPropertyChanged("IdHigieneEnCelda"); }
        }

        private string _IdConductaConFam;

        public string IdConductaConFam
        {
            get { return _IdConductaConFam; }
            set { _IdConductaConFam = value; OnPropertyChanged("IdConductaConFam"); }
        }
        private string _CorrectivosDisc;

        public string CorrectivosDisc
        {
            get { return _CorrectivosDisc; }
            set { _CorrectivosDisc = value; OnPropertyChanged("CorrectivosDisc"); }
        }
        private string _EstimulosBuenaConducta;

        public string EstimulosBuenaConducta
        {
            get { return _EstimulosBuenaConducta; }
            set { _EstimulosBuenaConducta = value; OnPropertyChanged("EstimulosBuenaConducta"); }
        }
        private string _IdClasificConductaGral;

        public string IdClasificConductaGral
        {
            get { return _IdClasificConductaGral; }
            set { _IdClasificConductaGral = value; OnPropertyChanged("IdClasificConductaGral"); }
        }
        private string _ConclusionesGrales;

        public string ConclusionesGrales
        {
            get { return _ConclusionesGrales; }
            set { _ConclusionesGrales = value; OnPropertyChanged("ConclusionesGrales"); }
        }

        private DateTime? _FecVigiFF;

        public DateTime? FecVigiFF
        {
            get { return _FecVigiFF; }
            set { _FecVigiFF = value; OnPropertyChanged("FecVigiFF"); }
        }

        #endregion

        #region Estudio criminologico

        private string _NombreImpCriminFF;

        public string NombreImpCriminFF
        {
            get { return _NombreImpCriminFF; }
            set { _NombreImpCriminFF = value; OnPropertyChanged("NombreImpCriminFF"); }
        }

        private string _SobreN;

        public string SobreN
        {
            get { return _SobreN; }
            set { _SobreN = value; OnPropertyChanged("SobreN"); }
        }

        private string _VersionDelitoCriminFF;

        public string VersionDelitoCriminFF
        {
            get { return _VersionDelitoCriminFF; }
            set { _VersionDelitoCriminFF = value; OnPropertyChanged("VersionDelitoCriminFF"); }
        }

        private string _CaractPersonalesRelacionadasDelito;

        public string CaractPersonalesRelacionadasDelito
        {
            get { return _CaractPersonalesRelacionadasDelito; }
            set { _CaractPersonalesRelacionadasDelito = value; OnPropertyChanged("CaractPersonalesRelacionadasDelito"); }
        }
        private string _IdRequiereValoracionCrimin;

        public string IdRequiereValoracionCrimin
        {
            get { return _IdRequiereValoracionCrimin; }
            set { _IdRequiereValoracionCrimin = value; OnPropertyChanged("IdRequiereValoracionCrimin"); }
        }
        private string _AntecedentesParaSocialesAntisociales;

        public string AntecedentesParaSocialesAntisociales
        {
            get { return _AntecedentesParaSocialesAntisociales; }
            set { _AntecedentesParaSocialesAntisociales = value; OnPropertyChanged("AntecedentesParaSocialesAntisociales"); }
        }
        private bool _IsPrimoDChecked = false;

        public bool IsPrimoDChecked
        {
            get { return _IsPrimoDChecked; }
            set { _IsPrimoDChecked = value; OnPropertyChanged("IsPrimoDChecked"); }
        }
        private bool _IsReincidenteEspecifChecked = false;

        public bool IsReincidenteEspecifChecked
        {
            get { return _IsReincidenteEspecifChecked; }
            set { _IsReincidenteEspecifChecked = value; OnPropertyChanged("IsReincidenteEspecifChecked"); }
        }
        private bool _IsReincidenteGenericoChecked = false;

        public bool IsReincidenteGenericoChecked
        {
            get { return _IsReincidenteGenericoChecked; }
            set { _IsReincidenteGenericoChecked = value; OnPropertyChanged("IsReincidenteGenericoChecked"); }
        }
        private bool _IsHabitualChecked = false;

        public bool IsHabitualChecked
        {
            get { return _IsHabitualChecked; }
            set { _IsHabitualChecked = value; OnPropertyChanged("IsHabitualChecked"); }
        }
        private bool _IsProfesionalChecked = false;

        public bool IsProfesionalChecked
        {
            get { return _IsProfesionalChecked; }
            set { _IsProfesionalChecked = value; OnPropertyChanged("IsProfesionalChecked"); }
        }
        private string _CriminogenesisCrimFF;

        public string CriminogenesisCrimFF
        {
            get { return _CriminogenesisCrimFF; }
            set { _CriminogenesisCrimFF = value; OnPropertyChanged("CriminogenesisCrimFF"); }
        }
        private string _Egocentrismo;

        public string Egocentrismo
        {
            get { return _Egocentrismo; }
            set { _Egocentrismo = value; OnPropertyChanged("Egocentrismo"); }
        }
        private string _LabAfectiva;

        public string LabAfectiva
        {
            get { return _LabAfectiva; }
            set { _LabAfectiva = value; OnPropertyChanged("LabAfectiva"); }
        }
        private string _Agresividad;

        public string Agresividad
        {
            get { return _Agresividad; }
            set { _Agresividad = value; OnPropertyChanged("Agresividad"); }
        }
        private string _IndAfectiva;

        public string IndAfectiva
        {
            get { return _IndAfectiva; }
            set { _IndAfectiva = value; OnPropertyChanged("IndAfectiva"); }
        }
        private string _ResultadoTratamientoInst;

        public string ResultadoTratamientoInst
        {
            get { return _ResultadoTratamientoInst; }
            set { _ResultadoTratamientoInst = value; OnPropertyChanged("ResultadoTratamientoInst"); }
        }
        private string _IdEstadoPeligrosidad;

        public string IdEstadoPeligrosidad
        {
            get { return _IdEstadoPeligrosidad; }
            set { _IdEstadoPeligrosidad = value; OnPropertyChanged("IdEstadoPeligrosidad"); }
        }
        private string _OpinionSobreConBeneficio;

        public string OpinionSobreConBeneficio
        {
            get { return _OpinionSobreConBeneficio; }
            set { _OpinionSobreConBeneficio = value; OnPropertyChanged("OpinionSobreConBeneficio"); }
        }

        private string _IdPronReinciFF;

        public string IdPronReinciFF
        {
            get { return _IdPronReinciFF; }
            set { _IdPronReinciFF = value; OnPropertyChanged("IdPronReinciFF"); }
        }
        private DateTime? _FecCriminFF;

        public DateTime? FecCriminFF
        {
            get { return _FecCriminFF; }
            set { _FecCriminFF = value; OnPropertyChanged("FecCriminFF"); }
        }

        private short? _IdClasificacionCri;

        public short? IdClasificacionCri
        {
            get { return _IdClasificacionCri; }
            set { _IdClasificacionCri = value; OnPropertyChanged("IdClasificacionCri"); }
        }

        private DateTime? _FechaNacimientoTSFederal;
        public DateTime? FechaNacimientoTSFederal
        {
            get { return _FechaNacimientoTSFederal; }
            set { _FechaNacimientoTSFederal = value; OnPropertyChanged("FechaNacimientoTSFederal"); }
        }

        private bool _PrimoCheckedFederal = false;

        public bool PrimoCheckedFederal
        {
            get { return _PrimoCheckedFederal; }
            set { _PrimoCheckedFederal = value; OnPropertyChanged("PrimoCheckedFederal"); }
        }

        private bool _EspecificoCheckedFederal = false;

        public bool EspecificoCheckedFederal
        {
            get { return _EspecificoCheckedFederal; }
            set { _EspecificoCheckedFederal = value; OnPropertyChanged("EspecificoCheckedFederal"); }
        }

        private bool _GenericoCheckedFederal = false;

        public bool GenericoCheckedFederal
        {
            get { return _GenericoCheckedFederal; }
            set { _GenericoCheckedFederal = value; OnPropertyChanged("GenericoCheckedFederal"); }
        }

        private bool _HabitualCheckedFederal = false;

        public bool HabitualCheckedFederal
        {
            get { return _HabitualCheckedFederal; }
            set { _HabitualCheckedFederal = value; OnPropertyChanged("HabitualCheckedFederal"); }
        }

        private bool _ProfesionalCheckedFederal;

        public bool ProfesionalCheckedFederal
        {
            get { return _ProfesionalCheckedFederal; }
            set { _ProfesionalCheckedFederal = value; OnPropertyChanged("ProfesionalCheckedFederal"); }
        }
        #endregion

        private string _LugarActaFF;

        public string LugarActaFF
        {
            get { return _LugarActaFF; }
            set { _LugarActaFF = value; OnPropertyChanged("LugarActaFF"); }
        }

        private short _AnioDias;

        public short AnioDias
        {
            get { return _AnioDias; }
            set { _AnioDias = value; OnPropertyChanged("AnioDias"); }
        }
        private short _EneroDias;

        public short EneroDias
        {
            get { return _EneroDias; }
            set { _EneroDias = value; OnPropertyChanged("EneroDias"); }
        }
        private short _FebreroDias;

        public short FebreroDias
        {
            get { return _FebreroDias; }
            set { _FebreroDias = value; OnPropertyChanged("FebreroDias"); }
        }
        private short _MarzoDias;

        public short MarzoDias
        {
            get { return _MarzoDias; }
            set { _MarzoDias = value; OnPropertyChanged("MarzoDias"); }
        }
        private short _AbrilDias;

        public short AbrilDias
        {
            get { return _AbrilDias; }
            set { _AbrilDias = value; OnPropertyChanged("AbrilDias"); }
        }
        private short _MayoDias;

        public short MayoDias
        {
            get { return _MayoDias; }
            set { _MayoDias = value; OnPropertyChanged("MayoDias"); }
        }
        private short _JunioDias;

        public short JunioDias
        {
            get { return _JunioDias; }
            set { _JunioDias = value; OnPropertyChanged("JunioDias"); }
        }
        private short _JulioDias;

        public short JulioDias
        {
            get { return _JulioDias; }
            set { _JulioDias = value; OnPropertyChanged("JulioDias"); }
        }
        private short _AgostoDias;

        public short AgostoDias
        {
            get { return _AgostoDias; }
            set { _AgostoDias = value; OnPropertyChanged("AgostoDias"); }
        }
        private short _SeptiemDias;

        public short SeptiemDias
        {
            get { return _SeptiemDias; }
            set { _SeptiemDias = value; OnPropertyChanged("SeptiemDias"); }
        }

        private short _MesDias;

        public short MesDias
        {
            get { return _MesDias; }
            set { _MesDias = value; OnPropertyChanged("MesDias"); }
        }

        private short _DiasL;

        public short DiasL
        {
            get { return _DiasL; }
            set { _DiasL = value; OnPropertyChanged("DiasL"); }
        }

        private short _NovDias;

        public short NovDias
        {
            get { return _NovDias; }
            set { _NovDias = value; OnPropertyChanged("NovDias"); }
        }
        private short _DicDias;

        public short DicDias
        {
            get { return _DicDias; }
            set { _DicDias = value; OnPropertyChanged("DicDias"); }
        }

        private ObservableCollection<PFF_DIAS_LABORADO> lstDiasLaborados;

        public ObservableCollection<PFF_DIAS_LABORADO> LstDiasLaborados
        {
            get { return lstDiasLaborados; }
            set { lstDiasLaborados = value; OnPropertyChanged("LstDiasLaborados"); }
        }

        private PFF_DIAS_LABORADO _SelectedDiaLab;

        public PFF_DIAS_LABORADO SelectedDiaLab
        {
            get { return _SelectedDiaLab; }
            set { _SelectedDiaLab = value; OnPropertyChanged("SelectedDiaLab"); }
        }


        private DateTime? _FecGruposRelig;

        public DateTime? FecGruposRelig
        {
            get { return _FecGruposRelig; }
            set { _FecGruposRelig = value; OnPropertyChanged("FecGruposRelig"); }
        }
        private DateTime? _FecGruposCult;

        public DateTime? FecGruposCult
        {
            get { return _FecGruposCult; }
            set { _FecGruposCult = value; OnPropertyChanged("FecGruposCult"); }
        }
        private DateTime? _FecActivDepor;

        public DateTime? FecActivDepor
        {
            get { return _FecActivDepor; }
            set { _FecActivDepor = value; OnPropertyChanged("FecActivDepor"); }
        }

        private ObservableCollection<PFF_ACTIVIDAD_PARTICIPACION> lstActividadPart;

        public ObservableCollection<PFF_ACTIVIDAD_PARTICIPACION> LstActividadPart
        {
            get { return lstActividadPart; }
            set { lstActividadPart = value; OnPropertyChanged("LstActividadPart"); }
        }

        private PFF_ACTIVIDAD_PARTICIPACION _SelectedActivPart;

        public PFF_ACTIVIDAD_PARTICIPACION SelectedActivPart
        {
            get { return _SelectedActivPart; }
            set { _SelectedActivPart = value; OnPropertyChanged("SelectedActivPart"); }
        }

        private ObservableCollection<TIPO_PROGRAMA> listProgramas;
        public ObservableCollection<TIPO_PROGRAMA> ListProgramas
        {
            get { return listProgramas; }
            set { listProgramas = value; OnPropertyChanged("ListProgramas"); }
        }

        private bool _EnabledActividadesTipoPFederal = true;

        public bool EnabledActividadesTipoPFederal
        {
            get { return _EnabledActividadesTipoPFederal; }
            set { _EnabledActividadesTipoPFederal = value; OnPropertyChanged("EnabledActividadesTipoPFederal"); }
        }

        private DateTime? _FecInicioProg;

        public DateTime? FecInicioProg
        {
            get { return _FecInicioProg; }
            set { _FecInicioProg = value; OnPropertyChanged("FecInicioProg"); }
        }
        private DateTime? _FecFinProg;

        public DateTime? FecFinProg
        {
            get { return _FecFinProg; }
            set { _FecFinProg = value; OnPropertyChanged("FecFinProg"); }
        }

        private string _Participo;

        public string Participo
        {
            get { return _Participo; }
            set { _Participo = value; OnPropertyChanged("Participo"); }
        }

        private short _IdTipoP = -1;

        public short IdTipoP
        {
            get { return _IdTipoP; }
            set { _IdTipoP = value; OnPropertyChanged("IdTipoP"); }
        }

        private ObservableCollection<OCUPACION> lstOcupaciones;

        public ObservableCollection<OCUPACION> LstOcupaciones
        {
            get { return lstOcupaciones; }
            set { lstOcupaciones = value; OnPropertyChanged("LstOcupaciones"); }
        }

        #region Lugares
        private string _LugarActa;

        public string LugarActa
        {
            get { return _LugarActa; }
            set { _LugarActa = value; OnPropertyChanged("LugarActa"); }
        }
        private string _LugarCrimi;

        public string LugarCrimi
        {
            get { return _LugarCrimi; }
            set { _LugarCrimi = value; OnPropertyChanged("LugarCrimi"); }
        }
        private string _LugarPsico;

        public string LugarPsico
        {
            get { return _LugarPsico; }
            set { _LugarPsico = value; OnPropertyChanged("LugarPsico"); }
        }
        private string _LugarTS;

        public string LugarTS
        {
            get { return _LugarTS; }
            set { _LugarTS = value; OnPropertyChanged("LugarTS"); }
        }
        private string _LugarEduc;

        public string LugarEduc
        {
            get { return _LugarEduc; }
            set { _LugarEduc = value; OnPropertyChanged("LugarEduc"); }
        }
        private string _LugarProd;

        public string LugarProd
        {
            get { return _LugarProd; }
            set { _LugarProd = value; OnPropertyChanged("LugarProd"); }
        }
        private string _LugarVigi;

        public string LugarVigi
        {
            get { return _LugarVigi; }
            set { _LugarVigi = value; OnPropertyChanged("LugarVigi"); }
        }
        #endregion


        private bool _IsEnabledEspecifiCont = false;

        public bool IsEnabledEspecifiCont
        {
            get { return _IsEnabledEspecifiCont; }
            set { _IsEnabledEspecifiCont = value; OnPropertyChanged("IsEnabledEspecifiCont"); }
        }

        private string _IdRequiereContTratam;

        public string IdRequiereContTratam
        {
            get { return _IdRequiereContTratam; }
            set
            {
                _IdRequiereContTratam = value;
                if (value == "S")
                {
                    base.RemoveRule("EspecifiqueContTrat");
                    IsEnabledEspecifiCont = true;
                    base.AddRule(() => EspecifiqueContTrat, () => !string.IsNullOrEmpty(EspecifiqueContTrat), "ESPECIFIQUE TRATAMIENTO ES REQUERIDO!");
                }
                else
                {
                    IsEnabledEspecifiCont = false;
                    base.RemoveRule("EspecifiqueContTrat");
                }

                OnPropertyChanged("EspecifiqueContTrat");
                OnPropertyChanged("IsEnabledEspecifiCont");
                OnPropertyChanged("IdRequiereContTratam");
            }
        }


        private string _IdHuboViolenciaIntro;

        public string IdHuboViolenciaIntro
        {
            get { return _IdHuboViolenciaIntro; }
            set
            {
                _IdHuboViolenciaIntro = value;
                if (value == "S")
                {
                    base.RemoveRule("EspecifiqueViolenciaIntro");
                    EnabledEspecifiViole1 = true;
                    base.AddRule(() => EspecifiqueViolenciaIntro, () => !string.IsNullOrEmpty(EspecifiqueViolenciaIntro), "ESPECIFIQUE VIOLENCIA ES REQUERIDO!");
                }
                else
                {
                    EnabledEspecifiViole1 = false;
                    base.RemoveRule("EspecifiqueViolenciaIntro");
                }

                OnPropertyChanged("EspecifiqueViolenciaIntro");
                OnPropertyChanged("EnabledEspecifiViole1");
                OnPropertyChanged("IdHuboViolenciaIntro");
            }
        }

        private bool _EnabledEspecifiViole1 = false;
        public bool EnabledEspecifiViole1
        {
            get { return _EnabledEspecifiViole1; }
            set { _EnabledEspecifiViole1 = value; OnPropertyChanged("EnabledEspecifiViole1"); }
        }

        private string _IdAlgunIntegranteCuentaAntecedentesAdiccion;

        public string IdAlgunIntegranteCuentaAntecedentesAdiccion
        {
            get { return _IdAlgunIntegranteCuentaAntecedentesAdiccion; }
            set
            {
                _IdAlgunIntegranteCuentaAntecedentesAdiccion = value;
                if (value == "S")
                {
                    base.RemoveRule("EspecifiqueAdiccion");
                    EnabledAdicto1 = true;
                    base.AddRule(() => EspecifiqueAdiccion, () => !string.IsNullOrEmpty(EspecifiqueAdiccion), "ESPECIFIQUE ADICCION ES REQUERIDO!");
                }
                else
                {
                    EnabledAdicto1 = false;
                    base.RemoveRule("EspecifiqueAdiccion");
                }

                OnPropertyChanged("EspecifiqueAdiccion");
                OnPropertyChanged("EnabledAdicto1");
                OnPropertyChanged("IdAlgunIntegranteCuentaAntecedentesAdiccion");
            }
        }
        private string _EspecifiqueAdiccion;

        public string EspecifiqueAdiccion
        {
            get { return _EspecifiqueAdiccion; }
            set { _EspecifiqueAdiccion = value; OnPropertyChanged("EspecifiqueAdiccion"); }
        }

        private bool _EnabledAdicto1 = false;

        public bool EnabledAdicto1
        {
            get { return _EnabledAdicto1; }
            set { _EnabledAdicto1 = value; OnPropertyChanged("EnabledAdicto1"); }
        }


        private string _IDHuboViolenciaIntraFam;

        public string IDHuboViolenciaIntraFam
        {
            get { return _IDHuboViolenciaIntraFam; }
            set
            {
                _IDHuboViolenciaIntraFam = value;
                if (value == "S")
                {
                    base.RemoveRule("EspecifiqueViolenciaIntraFam");
                    IsEnabledViolenciaNo2 = true;
                    base.AddRule(() => EspecifiqueViolenciaIntraFam, () => !string.IsNullOrEmpty(EspecifiqueViolenciaIntraFam), "ESPECIFIQUE VIOLENCIA ES REQUERIDO!");
                }

                else
                {
                    IsEnabledViolenciaNo2 = false;
                    base.RemoveRule("EspecifiqueViolenciaIntraFam");
                }

                OnPropertyChanged("EspecifiqueViolenciaIntraFam");
                OnPropertyChanged("IsEnabledViolenciaNo2");
                OnPropertyChanged("IDHuboViolenciaIntraFam");
            }
        }
        private string _EspecifiqueViolenciaIntraFam;

        public string EspecifiqueViolenciaIntraFam
        {
            get { return _EspecifiqueViolenciaIntraFam; }
            set { _EspecifiqueViolenciaIntraFam = value; OnPropertyChanged("EspecifiqueViolenciaIntraFam"); }
        }

        private bool _IsEnabledViolenciaNo2 = false;

        public bool IsEnabledViolenciaNo2
        {
            get { return _IsEnabledViolenciaNo2; }
            set { _IsEnabledViolenciaNo2 = value; OnPropertyChanged("IsEnabledViolenciaNo2"); }
        }

        private bool _IsEnabledConductaPara = false;

        public bool IsEnabledConductaPara
        {
            get { return _IsEnabledConductaPara; }
            set { _IsEnabledConductaPara = value; OnPropertyChanged("IsEnabledConductaPara"); }
        }

        private string _IdMiembroProblemasConductaAntisocial;

        public string IdMiembroProblemasConductaAntisocial
        {
            get { return _IdMiembroProblemasConductaAntisocial; }
            set
            {
                if (value == "S")
                {
                    base.RemoveRule("EspecifiqueProblemasConductaAntisocial");
                    IsEnabledConductaPara = true;
                    base.AddRule(() => EspecifiqueProblemasConductaAntisocial, () => !string.IsNullOrEmpty(EspecifiqueProblemasConductaAntisocial), "ESPECIFIQUE CONDUCTA ES REQUERIDO!");
                }

                else
                {
                    IsEnabledConductaPara = false;
                    base.RemoveRule("EspecifiqueProblemasConductaAntisocial");
                }

                _IdMiembroProblemasConductaAntisocial = value;

                OnPropertyChanged("IdMiembroProblemasConductaAntisocial");
                OnPropertyChanged("EspecifiqueProblemasConductaAntisocial");
                OnPropertyChanged("IsEnabledConductaPara");
            }
        }
        private string _EspecifiqueProblemasConductaAntisocial;

        public string EspecifiqueProblemasConductaAntisocial
        {
            get { return _EspecifiqueProblemasConductaAntisocial; }
            set { _EspecifiqueProblemasConductaAntisocial = value; OnPropertyChanged("EspecifiqueProblemasConductaAntisocial"); }
        }




        private string _IdCuentaOfertaTrabajo;

        public string IdCuentaOfertaTrabajo
        {
            get { return _IdCuentaOfertaTrabajo; }
            set
            {
                _IdCuentaOfertaTrabajo = value;
                if (value == "S")
                {
                    base.RemoveRule("ConsisteOfertaFF");
                    IsEnabledOfertaT = true;
                    base.AddRule(() => ConsisteOfertaFF, () => !string.IsNullOrEmpty(ConsisteOfertaFF), "ESPECIFIQUE OFERTA TRABAJO ES REQUERIDO!");
                }

                else
                {
                    IsEnabledOfertaT = false;
                    base.RemoveRule("ConsisteOfertaFF");
                }

                OnPropertyChanged("ConsisteOfertaFF");
                OnPropertyChanged("IsEnabledOfertaT");
                OnPropertyChanged("IdCuentaOfertaTrabajo");
            }
        }

        private string _ConsisteOfertaFF;

        public string ConsisteOfertaFF
        {
            get { return _ConsisteOfertaFF; }
            set { _ConsisteOfertaFF = value; OnPropertyChanged("ConsisteOfertaFF"); }
        }

        private bool _IsEnabledOfertaT = false;

        public bool IsEnabledOfertaT
        {
            get { return _IsEnabledOfertaT; }
            set { _IsEnabledOfertaT = value; OnPropertyChanged("IsEnabledOfertaT"); }
        }


        private string _IdRecibeVisitasFamiliares;

        public string IdRecibeVisitasFamiliares
        {
            get { return _IdRecibeVisitasFamiliares; }
            set
            {
                _IdRecibeVisitasFamiliares = value;
                if (value == "S")
                {
                    base.RemoveRule("IdRadicanEstado");
                    base.RemoveRule("IdParentescoVisTSFF");
                    base.RemoveRule("FrecuenciaVisitas");

                    IsEnabledVisitasCampos = true;

                    base.AddRule(() => IdRadicanEstado, () => !string.IsNullOrEmpty(IdRadicanEstado), "RADICAN ESTADO ES REQUERIDO!");
                    base.AddRule(() => IdParentescoVisTSFF, () => IdParentescoVisTSFF.HasValue ? IdParentescoVisTSFF != -1 : false, "PARENTESCO ES REQUERIDO!");
                    base.AddRule(() => FrecuenciaVisitas, () => !string.IsNullOrEmpty(FrecuenciaVisitas), "FRECUENCIA VISITA ES REQUERIDO!");
                    OnPropertyChanged("IdRadicanEstado");
                    OnPropertyChanged("IdParentescoVisTSFF");// IdParentesco
                    OnPropertyChanged("FrecuenciaVisitas");
                }

                else
                {
                    IsEnabledVisitasCampos = false;
                    base.RemoveRule("IdRadicanEstado");
                    base.RemoveRule("IdParentescoVisTSFF");
                    base.RemoveRule("FrecuenciaVisitas");
                    OnPropertyChanged("IdRadicanEstado");
                    OnPropertyChanged("IdParentesco");
                    OnPropertyChanged("FrecuenciaVisitas");
                }

                OnPropertyChanged("IdRadicanEstado");
                OnPropertyChanged("IdParentescoVisTSFF");
                OnPropertyChanged("FrecuenciaVisitas");
                OnPropertyChanged("IsEnabledVisitasCampos");
                OnPropertyChanged("IdRecibeVisitasFamiliares");
            }
        }
        private string _IdRadicanEstado;

        public string IdRadicanEstado
        {
            get { return _IdRadicanEstado; }
            set { _IdRadicanEstado = value; OnPropertyChanged("IdRadicanEstado"); }
        }
        private short? _IdParentesco;

        public short? IdParentesco
        {
            get { return _IdParentesco; }
            set { _IdParentesco = value; OnPropertyChanged("IdParentesco"); }
        }
        private string _FrecuenciaVisitas;

        public string FrecuenciaVisitas
        {
            get { return _FrecuenciaVisitas; }
            set { _FrecuenciaVisitas = value; OnPropertyChanged("FrecuenciaVisitas"); }
        }

        private bool _IsEnabledVisitasCampos = false;

        public bool IsEnabledVisitasCampos
        {
            get { return _IsEnabledVisitasCampos; }
            set { _IsEnabledVisitasCampos = value; OnPropertyChanged("IsEnabledVisitasCampos"); }
        }


        private string _IsVisitadoPorOtrasPersonas;
        public string IsVisitadoPorOtrasPersonas
        {
            get { return _IsVisitadoPorOtrasPersonas; }
            set
            {
                _IsVisitadoPorOtrasPersonas = value;
                if (value == "S")
                {
                    base.RemoveRule("QuienesVisitanOtrasPersonas");
                    IsEnabledVisiOtrasP = true;
                    base.AddRule(() => QuienesVisitanOtrasPersonas, () => !string.IsNullOrEmpty(QuienesVisitanOtrasPersonas), "QUIENES VISITA ES REQUERIDO!");
                }

                else
                {
                    base.RemoveRule("QuienesVisitanOtrasPersonas");
                    IsEnabledVisiOtrasP = false;
                }

                OnPropertyChanged("QuienesVisitanOtrasPersonas");
                OnPropertyChanged("IsEnabledVisiOtrasP");
                OnPropertyChanged("IsVisitadoPorOtrasPersonas");
            }
        }

        private string _QuienesVisitanOtrasPersonas;
        public string QuienesVisitanOtrasPersonas
        {
            get { return _QuienesVisitanOtrasPersonas; }
            set { _QuienesVisitanOtrasPersonas = value; OnPropertyChanged("QuienesVisitanOtrasPersonas"); }
        }

        private bool _IsEnabledVisiOtrasP = false;

        public bool IsEnabledVisiOtrasP
        {
            get { return _IsEnabledVisiOtrasP; }
            set { _IsEnabledVisiOtrasP = value; OnPropertyChanged("IsEnabledVisiOtrasP"); }
        }

        private bool _IsEnabledEspecificaReqTrataExtra = false;//SI

        public bool IsEnabledEspecificaReqTrataExtra
        {
            get { return _IsEnabledEspecificaReqTrataExtra; }
            set { _IsEnabledEspecificaReqTrataExtra = value; OnPropertyChanged("IsEnabledEspecificaReqTrataExtra"); }
        }

        private bool _IsEnabledEspecificaReqTrataExtra2 = false;//NO

        public bool IsEnabledEspecificaReqTrataExtra2
        {
            get { return _IsEnabledEspecificaReqTrataExtra2; }
            set { _IsEnabledEspecificaReqTrataExtra2 = value; OnPropertyChanged("IsEnabledEspecificaReqTrataExtra2"); }
        }
        private string _ReqTrataExtraMurosCriminFF;

        public string ReqTrataExtraMurosCriminFF
        {
            get { return _ReqTrataExtraMurosCriminFF; }
            set
            {
                _ReqTrataExtraMurosCriminFF = value;
                if (string.IsNullOrEmpty(value))
                {
                    OnPropertyChanged("ReqTrataExtraMurosCriminFF");
                    return;
                };

                if (value == "S")
                {
                    base.RemoveRule("AfirmaEspecifFF");
                    base.RemoveRule("NegatEspecifFF");

                    IsEnabledEspecificaReqTrataExtra = true;
                    IsEnabledEspecificaReqTrataExtra2 = false;

                    base.AddRule(() => AfirmaEspecifFF, () => !string.IsNullOrEmpty(AfirmaEspecifFF), "CASO AFIRMATIVO ES REQUERIDO!");
                    OnPropertyChanged("AfirmaEspecifFF");
                    OnPropertyChanged("NegatEspecifFF");
                    OnPropertyChanged("IsEnabledEspecificaReqTrataExtra2");
                    OnPropertyChanged("IsEnabledEspecificaReqTrataExtra");
                }

                else
                {
                    base.RemoveRule("AfirmaEspecifFF");
                    base.RemoveRule("NegatEspecifFF");

                    IsEnabledEspecificaReqTrataExtra = false;
                    IsEnabledEspecificaReqTrataExtra2 = true;

                    base.AddRule(() => NegatEspecifFF, () => !string.IsNullOrEmpty(NegatEspecifFF), "CASO NEGATIVO ES REQUERIDO!");
                    OnPropertyChanged("AfirmaEspecifFF");
                    OnPropertyChanged("NegatEspecifFF");
                    OnPropertyChanged("IsEnabledEspecificaReqTrataExtra2");
                    OnPropertyChanged("IsEnabledEspecificaReqTrataExtra");
                }

                OnPropertyChanged("ReqTrataExtraMurosCriminFF");
                OnPropertyChanged("AfirmaEspecifFF");
                OnPropertyChanged("NegatEspecifFF");
                OnPropertyChanged("IsEnabledEspecificaReqTrataExtra");
                OnPropertyChanged("IsEnabledEspecificaReqTrataExtra2");
            }
        }
        private string _AfirmaEspecifFF;

        public string AfirmaEspecifFF
        {
            get { return _AfirmaEspecifFF; }
            set { _AfirmaEspecifFF = value; OnPropertyChanged("AfirmaEspecifFF"); }
        }
        private string _NegatEspecifFF;

        public string NegatEspecifFF
        {
            get { return _NegatEspecifFF; }
            set { _NegatEspecifFF = value; OnPropertyChanged("NegatEspecifFF"); }
        }

        private bool _IsEnabledCamposV2 = false;

        public bool IsEnabledCamposV2
        {
            get { return _IsEnabledCamposV2; }
            set { _IsEnabledCamposV2 = value; OnPropertyChanged("IsEnabledCamposV2"); }
        }

        private string _IdRecibeVisita;
        public string IdRecibeVisita
        {
            get { return _IdRecibeVisita; }
            set
            {
                _IdRecibeVisita = value;
                if (value == "S")
                {
                    base.RemoveRule("RecibeVisitaFrecuencia");
                    base.RemoveRule("QuienesRecibeVisita");
                    IsEnabledCamposV2 = true;
                    base.AddRule(() => RecibeVisitaFrecuencia, () => !string.IsNullOrEmpty(RecibeVisitaFrecuencia), "FRECUENCIA VISITA ES REQUERIDO!");
                    base.AddRule(() => QuienesRecibeVisita, () => !string.IsNullOrEmpty(QuienesRecibeVisita), "QUIENES VISITA ES REQUERIDO!");
                }

                else
                {
                    base.RemoveRule("RecibeVisitaFrecuencia");
                    base.RemoveRule("QuienesRecibeVisita");
                    IsEnabledCamposV2 = false;
                }

                OnPropertyChanged("QuienesRecibeVisita");
                OnPropertyChanged("RecibeVisitaFrecuencia");
                OnPropertyChanged("IsEnabledCamposV2");
                OnPropertyChanged("IdRecibeVisita");
            }
        }
        private string _RecibeVisitaFrecuencia;

        public string RecibeVisitaFrecuencia
        {
            get { return _RecibeVisitaFrecuencia; }
            set { _RecibeVisitaFrecuencia = value; OnPropertyChanged("RecibeVisitaFrecuencia"); }
        }
        private string _QuienesRecibeVisita;

        public string QuienesRecibeVisita
        {
            get { return _QuienesRecibeVisita; }
            set { _QuienesRecibeVisita = value; OnPropertyChanged("QuienesRecibeVisita"); }
        }

        private bool _IsEnabledDatosPromocionEscolar = false;

        public bool IsEnabledDatosPromocionEscolar
        {
            get { return _IsEnabledDatosPromocionEscolar; }
            set { _IsEnabledDatosPromocionEscolar = value; OnPropertyChanged("IsEnabledDatosPromocionEscolar"); }
        }

        private string _HaSidoPromovido;

        public string HaSidoPromovido
        {
            get { return _HaSidoPromovido; }
            set
            {
                if (value == "S")
                    IsEnabledDatosPromocionEscolar = true;
                else
                    IsEnabledDatosPromocionEscolar = false;

                _HaSidoPromovido = value;
                OnPropertyChanged("HaSidoPromovido");
                OnPropertyChanged("IsEnabledDatosPromocionEscolar");
            }
        }
        private bool _IsAlfabAPrimariaChecked = false;

        public bool IsAlfabAPrimariaChecked
        {
            get { return _IsAlfabAPrimariaChecked; }
            set { _IsAlfabAPrimariaChecked = value; OnPropertyChanged("IsAlfabAPrimariaChecked"); }
        }
        private bool _IsPrimaASecChecked = false;

        public bool IsPrimaASecChecked
        {
            get { return _IsPrimaASecChecked; }
            set { _IsPrimaASecChecked = value; OnPropertyChanged("IsPrimaASecChecked"); }
        }
        private bool _IsSecABacChecked = false;

        public bool IsSecABacChecked
        {
            get { return _IsSecABacChecked; }
            set { _IsSecABacChecked = value; OnPropertyChanged("IsSecABacChecked"); }
        }
        private bool _IsBacAUnivChecked = false;

        public bool IsBacAUnivChecked
        {
            get { return _IsBacAUnivChecked; }
            set { _IsBacAUnivChecked = value; OnPropertyChanged("IsBacAUnivChecked"); }
        }

        private bool _IsEnabledOtroPromocion = false;
        public bool IsEnabledOtroPromocion
        {
            get { return _IsEnabledOtroPromocion; }
            set { _IsEnabledOtroPromocion = value; OnPropertyChanged("IsEnabledOtroPromocion"); }
        }

        private bool _IsOtroAcademChecked = false;
        public bool IsOtroAcademChecked
        {
            get { return _IsOtroAcademChecked; }
            set
            {
                if (value)
                {
                    base.RemoveRule("EspecifiqueOtroAcademico");
                    IsEnabledOtroPromocion = true;
                    base.AddRule(() => EspecifiqueOtroAcademico, () => !string.IsNullOrEmpty(EspecifiqueOtroAcademico), "OTRA PROMOCION ES REQUERIDO!");
                }
                else
                {
                    base.RemoveRule("EspecifiqueOtroAcademico");
                    IsEnabledOtroPromocion = false;
                }

                _IsOtroAcademChecked = value;
                OnPropertyChanged("EspecifiqueOtroAcademico");
                OnPropertyChanged("IsEnabledOtroPromocion");
                OnPropertyChanged("IsOtroAcademChecked");
            }
        }

        private string _EspecifiqueOtroAcademico;

        public string EspecifiqueOtroAcademico
        {
            get { return _EspecifiqueOtroAcademico; }
            set { _EspecifiqueOtroAcademico = value; OnPropertyChanged("EspecifiqueOtroAcademico"); }
        }


        private bool _IsEnabledEnsenianza = false;

        public bool IsEnabledEnsenianza
        {
            get { return _IsEnabledEnsenianza; }
            set { _IsEnabledEnsenianza = value; OnPropertyChanged("IsEnabledEnsenianza"); }
        }

        private string _HaImpartidoAlgunaEnsenanza;

        public string HaImpartidoAlgunaEnsenanza
        {
            get { return _HaImpartidoAlgunaEnsenanza; }
            set
            {
                if (value == "S")
                {
                    base.RemoveRule("TipoEnsenanza");
                    base.RemoveRule("CuantoTiempo");
                    IsEnabledEnsenianza = true;
                    base.AddRule(() => TipoEnsenanza, () => !string.IsNullOrEmpty(TipoEnsenanza), "TIPO ENSENANZA ES REQUERIDO!");
                    base.AddRule(() => CuantoTiempo, () => !string.IsNullOrEmpty(CuantoTiempo), "CUANTO TIEMPO ES REQUERIDO!");
                }
                else
                {
                    base.RemoveRule("TipoEnsenanza");
                    base.RemoveRule("CuantoTiempo");
                    IsEnabledEnsenianza = false;
                }

                OnPropertyChanged("TipoEnsenanza");
                OnPropertyChanged("CuantoTiempo");
                OnPropertyChanged("IsEnabledEnsenianza");
                _HaImpartidoAlgunaEnsenanza = value;
                OnPropertyChanged("HaImpartidoAlgunaEnsenanza");
            }
        }
        private string _TipoEnsenanza;

        public string TipoEnsenanza
        {
            get { return _TipoEnsenanza; }
            set { _TipoEnsenanza = value; OnPropertyChanged("TipoEnsenanza"); }
        }
        private string _CuantoTiempo;

        public string CuantoTiempo
        {
            get { return _CuantoTiempo; }
            set { _CuantoTiempo = value; OnPropertyChanged("CuantoTiempo"); }
        }


        private ObservableCollection<ControlPenales.Clases.GrupoFamiliarPV> lstPV;
        public ObservableCollection<ControlPenales.Clases.GrupoFamiliarPV> LstPV
        {
            get { return lstPV; }
            set { lstPV = value; OnPropertyChanged("LstPV"); }
        }

        private bool emptyPadronVisita;
        public bool EmptyPadronVisita
        {
            get { return emptyPadronVisita; }
            set { emptyPadronVisita = value; OnPropertyChanged("EmptyPadronVisita"); }
        }

        private string _DetalleOtrosProgAss;
        public string DetalleOtrosProgAss
        {
            get { return _DetalleOtrosProgAss; }
            set { _DetalleOtrosProgAss = value; OnPropertyChanged("DetalleOtrosProgAss"); }
        }

        /*
         * 
         * 
         * 
         *                 if (value)
                {
                    base.RemoveRule("EspecifiqueOtraDependencia");
                    IsEnabledOtrosAsistenciaFederal = true;
                    base.AddRule(() => EspecifiqueOtraDependencia, () => !string.IsNullOrEmpty(EspecifiqueOtraDependencia), "OTRAS DEPENDENCIAS ES REQUERIDO!");
                }
                else
                {
                    IsEnabledOtrosAsistenciaFederal = false;
                    base.RemoveRule("EspecifiqueOtraDependencia");
                }

                OnPropertyChanged("IsEnabledOtrosAsistenciaFederal");
                OnPropertyChanged("IsOtrosGruChecked");
                OnPropertyChanged("EspecifiqueOtraDependencia");
         */
    }
}