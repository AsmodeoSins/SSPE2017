namespace ControlPenales
{
    public class cRealizacionEstudios
    {

        #region Genericos
        public string Dictamen { get; set; }
        public string FechaRealizacionEstudio { get; set; }
        public string ImpresionDiagnostica { get; set; }
        public string MotivacionDictamen { get; set; }
        public string CasoNegativoSenialeProgramasARemitir { get; set; }
        public string RequiereTratExtraMurosTexto { get; set; }
        public string EdadInterno { get; set; }
        public string SexoInterno { get; set; }
        public string NombreInterno { get; set; }
        public string LugarFecNacInterno { get; set; }
        public string EstadoCivilInterno { get; set; }
        public string DomicilioInterno { get; set; }
        public string TelefonoInterno { get; set; }
        public string ProgramasDuranteTratamiento { get; set; }
        public string DelitoInterno { get; set; }
        public string SentenciaInterno { get; set; }
        public string ExpInterno { get; set; }
        public string AliasInterno { get; set; }
        public string OriginarioDeInterno { get; set; }
        public string OcupacionAnteriorInterno { get; set; }
        public string OcupacionActualInterno { get; set; }
        public string TensionArterial { get; set; }
        public string Teperatura { get; set; }
        public string Pulso { get; set; }
        public string Respiracion { get; set; }
        public string Peso { get; set; }
        public string Estatura { get; set; }
        public string Conclusion { get; set; }
        public string OpinionSobreOtorgamientoBeneficio { get; set; }
        public string DialectoInterno { get; set; }
        public string EscolaridadInicio { get; set; }
        public string EscolaridadAct { get; set; }
        private string Seccion { get; set; }
        public string FechaIngreso { get; set; }
        public string CeresoProcede { get; set; }
        public string EgocentrismoTexto { get; set; }
        public string LabilidadAfectivaTexto { get; set; }
        public string AgresividadTexto { get; set; }
        public string IndiferenciaAfectTexto { get; set; }
        public string TextoGenerico1 { get; set; }
        public string TextoGenerico2 { get; set; }
        public string TextoGenerico3 { get; set; }
        public string TextoGenerico4 { get; set; }
        public string TextoGenerico5 { get; set; }
        public string TextoGenerico6 { get; set; }
        public string TextoGenerico7 { get; set; }
        public string TextoGenerico8 { get; set; }
        public string TextoGenerico9 { get; set; }
        public string TextoGenerico10 { get; set; }
        public string TextoGenerico11 { get; set; }
        public string TextoGenerico12 { get; set; }
        #endregion
        #region Fuero Comun
        #region II Estudio Medico Fuero Comun
        public string AntecedentesHeredoFamiliares { get; set; }
        public string AntecedentesPersonalesNoPatologicos { get; set; }
        public string AntecedentesConsumoToxicosEstadoActual { get; set; }
        public string DescrTatuajesCicatricesRecAntiguasMalformaciones { get; set; }
        public string AntecedentesPatologicos { get; set; }
        public string PadecimientoActual { get; set; }
        #endregion
        #region III Estudio Psiquiatrico Fuero Comun
        public string AspectoFisico { get; set; }
        public string ConductaMotora { get; set; }
        public string Habla { get; set; }
        public string Actitud { get; set; }
        public string EstadoAnimo { get; set; }
        public string ExpresAfectiva { get; set; }
        public string Adecuacion { get; set; }
        public string Alucinaciones { get; set; }
        public string Ilusiones { get; set; }
        public string Despersonalizacion { get; set; }
        public string Desrealizacion { get; set; }
        public string CursoPensamiento { get; set; }
        public string ContinuidadPensamiento { get; set; }
        public string ContenidoPensamiento { get; set; }
        public string PensamientoAbstracto { get; set; }
        public string Concentracion { get; set; }
        public string Orientacion { get; set; }
        public string Memoria { get; set; }
        public string BajaToleranciaFrust { get; set; }
        public string ExpresionDesadapt { get; set; }
        public string Adecuada { get; set; }
        public string CapacJuicio { get; set; }
        public string Introspeccion { get; set; }
        public string Fiabilidad { get; set; }
        #endregion
        #region IV Estudio Psicologico
        public string CondicionesGralesInterno { get; set; }
        public string ExamenMental { get; set; }
        public string DescripcionPrincRasgosIngresoRelComDelito { get; set; }
        public string LauretaBenderTexto { get; set; }
        public string MatricesRavenTexto { get; set; }
        public string HTPTexto { get; set; }
        public string MinnessotaTexto { get; set; }
        public string OtroTestTexto { get; set; }
        public string NivelIntelectualTextro { get; set; }
        public string DatosDisfuncionNeuroTexto { get; set; }
        public string IntegracionDinamica { get; set; }
        public string RasgosPersonalidadRelComisionDelitoLogradoModificarInternamiento { get; set; }
        #endregion
        #region V Criminodiagnostico
        public string VersionDelitoSegunInterno { get; set; }
        public string MomentoCometerDelitoEncontrabaInfluenciaDrogaTexto { get; set; }
        public string DescripcionDrogasTexto { get; set; }
        public string Criminogenesis { get; set; }
        public string AntecedentesEvolucionConductasParaAntiSociales { get; set; }
        public string IntimidacionPenaImpuestaTexto { get; set; }
        public string PorqueIntimidacion { get; set; }
        public string CapacidadCriminalActualTexto { get; set; }
        public string AdaptabSocialTexto { get; set; }
        public string IndicePeligrosidadCriminActualTexto { get; set; }
        public string PronosticoReincidenciaTexto { get; set; }
        public string CualExtramuros { get; set; }
        #endregion
        #region VI ESTUDIO SOCIO FAMILIAR
        public string FamiliaPrimaria { get; set; }
        public string FamiliaSecundaria { get; set; }
        public string AdultoMayorProgramaEspecial { get; set; }
        public string RecibeVisitasTexto { get; set; }
        public string QuienesVisitasTexto { get; set; }
        public string RazonNoRecibeVisitasTexto { get; set; }
        public string MantieneComunicTelefonicaTexto { get; set; }
        public string PadronVisitas { get; set; }
        public string PlanesSerExternado { get; set; }
        public string QuienViviraSerExternado { get; set; }
        public string CuentaOfertaTrabajoTexto { get; set; }
        public string CuentaAvalMoralTexto { get; set; }
        #endregion
        #region VII Estudio Educativo, Cultural y Deportivo
        public string EscolaridadAnteriorIngreso { get; set; }
        #endregion
        #region VIII Estudio Sobre Capacitacion y Trabajo Penitenciario
        public string OficioActivDesempenadaAntesReclucion { get; set; }
        public string ResponsabilidadTexto { get; set; }
        public string CalidadTrabajoTexto { get; set; }
        public string PerseveranciaTexto { get; set; }
        public string CuentaFondoAhorroTexto { get; set; }
        public string DiasOtrosCentros { get; set; }
        public string DiasCentroActual { get; set; }
        public string DiasTotalLaborados { get; set; }
        public string PerioroDesarrolloActivLaboral { get; set; }
        #endregion
        #region IX Informe Area Seguridad y Custodia
        public string ConductaObservadaCentroTexto { get; set; }
        public string ConductaAutoridadTexto { get; set; }
        public string ConductaGralTexto { get; set; }
        public string RelacionCompanerosTexto { get; set; }
        public string RegistraCorrectivosDiscTexto { get; set; }
        public string CorrectivosAplicados { get; set; }
        public string OpinionConductaGralInterno { get; set; }
        #endregion
        #endregion
        #region Fuero Federal
        #region Acta de Consejo Tecnico Interdisciplinario
        public string APartirDe { get; set; }
        public string EnSesionFecha { get; set; }
        public string Estado { get; set; }
        public string NombbreCentro { get; set; }
        public string CondensadoAreas { get; set; }
        public string DirectorCRS { get; set; }
        public string ActuacionTexto { get; set; }
        public string VotosTexto { get; set; }
        public string TramiteTexto { get; set; }
        public string LugarDesc { get; set; }
        #endregion
        #region Estudio Medico de Fuero Federal
        public string AntecedentesHeredoFamFederal { get; set; }
        public string AntecedentesPersonalesNoPatFederal { get; set; }
        public string AntecedentesPatoFederal { get; set; }
        public string AntecedentesConsumoSustToxicas { get; set; }
        public string AsistenciaGuposFederal { get; set; }
        public string EspecifiqueAsistenciaGruposFederal { get; set; }
        public string PadecimientoActualFederal { get; set; }
        public string InterrogAparatosSistFederal { get; set; }
        public string ExploracionFisicaCabezCuello { get; set; }
        public string Torax { get; set; }
        public string Abdomen { get; set; }
        public string OrganosGenit { get; set; }
        public string Extremidades { get; set; }
        public string DescripcionTatuajesCicatrRecAntiguasMalformacionesFederal { get; set; }
        public string Diagnostico { get; set; }
        public string TerpeuticaImpl { get; set; }
        #endregion
        #region Estudio Psicologico de Fuero Federal
        public string ActitudTomadaAntesEntrevista { get; set; }
        public string ExamenMentalFF { get; set; }
        public string PruebasAplicadas { get; set; }
        public string NivelInt { get; set; }
        public string CI { get; set; }
        public string IndiceLesionOrganica { get; set; }
        public string DinamicaPersonalidadIngreso { get; set; }
        public string DinamicaPersonalidadActual { get; set; }
        public string ResultadosTratamientoProp { get; set; }
        public string ReqContinuacionTratTexto { get; set; }
        public string EspecifiqueContinuacionTrat { get; set; }
        public string Pronostico { get; set; }
        #endregion
        #region Estudio de Trabajo Social de Fuero Federal
        public string DatosGralesGrupoPrimario { get; set; }
        public string CaractZona { get; set; }
        public string ResponsManutencionHogar { get; set; }
        public string TotalIngresosMensuales { get; set; }
        public string TotalEgresosMensuales { get; set; }
        public string ActualmenteInternoCooperaEcon { get; set; }
        public string TieneFondoAhorro { get; set; }
        public string GrupoFamPrimarioTexto { get; set; }
        public string RelacionesInterfamiliaresTexto { get; set; }
        public string HuboViolenciaIntrafamTexto { get; set; }
        public string EspecificarViolenciaIntrafam { get; set; }
        public string NivelSocioEconomicoCultPrimario { get; set; }
        public string AlgunIntegTieneAntecedPenales { get; set; }
        public string EspecifiqGrupoPrimario { get; set; }
        public string ConceptoTieneFamInterno { get; set; }
        public string DatosGralesFamSec { get; set; }
        public string HijosUnionesAnt { get; set; }
        public string GrupoFamSec { get; set; }
        public string RelacionesInterfamSecundario { get; set; }
        public string ViolenciaIntraFamGrupoSecundario { get; set; }
        public string EspecificViolenciaGrupoSecundario { get; set; }
        public string NivelSocioEconomicoCulturalGrupoSecundario { get; set; }
        public string NumHabitacionesTotal { get; set; }
        public string DescripcionVivienda { get; set; }
        public string TransporteCercaVivienda { get; set; }
        public string EnseresMobiliario { get; set; }
        public string CaractZonaGrupoSec { get; set; }
        public string RelacionMedioExterno { get; set; }
        public string AlgunMiembroPresentaProbConductaPara { get; set; }
        public string EspecifiqueConductaGrupoSecundario { get; set; }
        public string NoPersonasVividoManeraEstable { get; set; }
        public string TrabajoAntesReclusion { get; set; }
        public string TiempoLaborar { get; set; }
        public string SueldoPercibidoGrupoSecundario { get; set; }
        public string OtrasAportacionesDeLaFamilia { get; set; }
        public string DistribucionGastoFamiliar { get; set; }
        public string AlimentacionFamiliar { get; set; }
        public string ServiciosCuenta { get; set; }
        public string OfertaTrabajoCuentaG { get; set; }
        public string EspecifiqueOfertaG { get; set; }
        public string CuentaApoyoFamiliaAlgunaPersona { get; set; }
        public string RecibeVisitaFam { get; set; }
        public string RadicanEstado { get; set; }
        public string ParentescoG { get; set; }
        public string FrecuenciaG { get; set; }
        public string ParentescoGFF { get; set; }
        public string VisitadoOtrasPersonas { get; set; }
        public string QuienesVisitaG { get; set; }
        public string NombreAvalMoralG { get; set; }
        public string ParentescoAval { get; set; }
        public string ConQuienViviraAlSerExternado { get; set; }
        public string CalleD { get; set; }
        public string NoD { get; set; }
        public string ColoniaD { get; set; }
        public string CPD { get; set; }
        public string DelegacionD { get; set; }
        public string CiudadD { get; set; }
        public string EntidadFed { get; set; }
        public string ParentescoD { get; set; }
        public string OpinionInternamiento { get; set; }
        public string DeQueFormaInfluenciaEstarPrision { get; set; }
        #endregion
        #region Informe de las Actividades Productivas de Capacitacion de Fuero Federal
        public string ActividadProductivaActualDentroCentro { get; set; }
        public string AtiendeIndicacionesSuperiores { get; set; }
        public string SatisfaceActividad { get; set; }
        public string DescuidadoCumplimientoLabores { get; set; }
        public string MotivosTiempoInterrupcionActiv { get; set; }
        public string CursosCapacitacioAprendizajeOficio { get; set; }
        public string RecibioConstancia { get; set; }
        public string CambiadoActividades { get; set; }
        public string PorqueCambiadoActiv { get; set; }
        public string HaProgresadoOficio { get; set; }
        public string ActitudesHaciaDesempenoActivProduct { get; set; }
        public string FondoAh { get; set; }
        public string CompensacionRecibeActualmen { get; set; }
        public string DiasLaboradosEfect { get; set; }
        public string TotalDiasLaboradosEfect { get; set; }
        public string TtlDiasOtrosCentros { get; set; }
        public string TtlAB { get; set; }
        #endregion
        #region Informe de las Actividades Eductivas, Culturales, Deportivas, Recreativas y Civicas de Fuero Federal
        public string EstudiosHaRealizadoInternamiento { get; set; }
        public string EstudiosCursaActualmente { get; set; }
        public string AsisteEscuelaVoluntPuntualidadAsist { get; set; }
        public string EspecifiqueNoAsisteVoluntPuntAsist { get; set; }
        public string AvanceRendimientoAcademico { get; set; }
        public string HaSidoPromovido { get; set; }
        public string PromocionesTexto { get; set; }
        public string EspecifiqueOtrasPromociones { get; set; }
        public string EnsenanzaRecibe { get; set; }
        public string HaImpartidoEnsenanza { get; set; }
        public string DeQueTipoEnsenanza { get; set; }
        public string CuantoTiempoEnsenanzaImpartida { get; set; }
        #endregion
        #region Informe Seccion Vigilancia de Fuero Federal
        public string ConductaObservoEnElMismo { get; set; }
        public string MotivoTraslado { get; set; }
        public string ConductaSuperioresTexto { get; set; }
        public string RelacionCompanieros { get; set; }
        public string DescrConducta { get; set; }
        public string HigienePersonalTexto { get; set; }
        public string HigieneCeldaTexto { get; set; }
        public string RecibeVisText { get; set; }
        public string FrecuenciaV { get; set; }
        public string DeQuienesVisita { get; set; }
        public string ConductaFamilia { get; set; }
        public string CorrectivosDisc { get; set; }
        public string EstimulosBuenaCond { get; set; }
        public string ClasificConsudtaGral { get; set; }
        #endregion
        #region Estudio Criminologico de Fuero Federal
        public string VersionDel { get; set; }
        public string CaractPersonalidadRelDel { get; set; }
        public string ReqValoracionVictim { get; set; }
        public string AntecedentesParasoc { get; set; }
        public string ClasificCriminologTexto { get; set; }
        public string CriminogenesisFF { get; set; }
        public string ResultTratamInstitucional { get; set; }
        public string EstadoPeligrosidadActual { get; set; }
        public string ProbabilidadReincidencia { get; set; }
        public string EspecifiqueExtraM { get; set; }
        #endregion
        #endregion
    }
}