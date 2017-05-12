namespace ControlPenales
{
    partial class RealizacionEstudiosViewModel
    {
        #region Fuero Comun
        #region Estudio Medico de Fuero Comun
        void ValidacionesEstudioMedicoFueroComun()
        {
            base.ClearRules();
            base.AddRule(() => AntecedentesHeredoFamiliares, () => !string.IsNullOrEmpty(AntecedentesHeredoFamiliares), "ANTECEDENTES FAMILIARES SON REQUERIDOS!");
            base.AddRule(() => AntecedentesPersonalesNoPatologicos, () => !string.IsNullOrEmpty(AntecedentesPersonalesNoPatologicos), "ANTECEDENTES PERSONALES NO PATOLOGICOS SON REQUERIDOS!");
            base.AddRule(() => AntedecentesConsumoToxicosEstadoActual, () => !string.IsNullOrEmpty(AntedecentesConsumoToxicosEstadoActual), "ANTECEDENTES DE CONSUMO DE TOXICOS SON REQUERIDOS!");
            base.AddRule(() => DescripcionTatuajesCicatricesMalformaciones, () => !string.IsNullOrEmpty(DescripcionTatuajesCicatricesMalformaciones), "DESCR. DE TATUAJES SON REQUERIDOS!");
            base.AddRule(() => AntecedentesPatologicos, () => !string.IsNullOrEmpty(AntecedentesPatologicos), "ANTECEDENTES PATOLOGICOS SON REQUERIDOS!");
            base.AddRule(() => DescipcionPadecimientoActual, () => !string.IsNullOrEmpty(DescipcionPadecimientoActual), "PADECIMIENTO ACTUAL ES REQUERIDO!");
            base.AddRule(() => TensionArterialGenerico, () => !string.IsNullOrEmpty(TensionArterialGenerico), "TENSION ARTERIAL ES REQUERIDO!");
            base.AddRule(() => TemperaturaGenerico, () => !string.IsNullOrEmpty(TemperaturaGenerico), "TEMPERATURA ES REQUERIDO!");
            base.AddRule(() => PulsoGenerico, () => !string.IsNullOrEmpty(PulsoGenerico), "PULSO ES REQUERIDO!");
            base.AddRule(() => RespiracionGenerico, () => !string.IsNullOrEmpty(RespiracionGenerico), "RESPIRACION ES REQUERIDO!");
            base.AddRule(() => PesoGenerico, () => !string.IsNullOrEmpty(PesoGenerico), "PESO ES REQUERIDO!");
            base.AddRule(() => EstaturaGenerico, () => !string.IsNullOrEmpty(EstaturaGenerico), "ESTATURA ES REQUERIDO!");
            base.AddRule(() => IdDictamenMedicoComun, () => IdDictamenMedicoComun.HasValue ? IdDictamenMedicoComun != 0 : false, "DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FechaEstudioMedicoComun, () => FechaEstudioMedicoComun.HasValue, "FECHA DE ESTUDIO ES REQUERIDO!");
            base.AddRule(() => ImpresionDiagnosticaEstudioMedicoComun, () => !string.IsNullOrEmpty(ImpresionDiagnosticaEstudioMedicoComun), "IMPRESION DIAGNOSTICA DE ESTUDIO MEDICO ES REQUERIDO!");
            OnPropertyChanged("");
        }
        #endregion
        #region Validaciones Estudio sobre Capacitacion y Trabajo Penitenciario de Fuero Comun
        void ValidacionesEstudioCapacitacionTrabajoFueroComun()
        {
            base.ClearRules();
            base.AddRule(() => ActividadOficioAntesReclusion, () => !string.IsNullOrEmpty(ActividadOficioAntesReclusion), "ACTIVIDAD U OFICIO ANTES DE RECLUSION ES REQUERIDO!");
            base.AddRule(() => IdResponsabilidad, () => !string.IsNullOrEmpty(IdResponsabilidad), "RESPONSABILIDAD ES REQUERIDO!");
            base.AddRule(() => IdCalidadTrabajo, () => !string.IsNullOrEmpty(IdCalidadTrabajo), "CALIDAD EN EL TRABAJO ES REQUERIDO!");
            base.AddRule(() => IdPerseverancia, () => !string.IsNullOrEmpty(IdPerseverancia), "PERSEVERANCIA ES REQUERIDO!");
            base.AddRule(() => IdCuentaConFondoAhorro, () => !string.IsNullOrEmpty(IdCuentaConFondoAhorro), "CUENTA CON FONDO DE AHORRO ES REQUERIDO!");
            base.AddRule(() => DiasEfectivosLaboradosEnOtrosCentros, () => DiasEfectivosLaboradosEnOtrosCentros.HasValue, "DIAS EFECTIVOS LABORADOS EN TROS CENTROS ES REQUERIDO!");
            base.AddRule(() => DiasEfectivosLaboradosEnCentroActual, () => DiasEfectivosLaboradosEnCentroActual.HasValue, "DIAS LABORADOS EN EL CENTRO ACTUAL ES REQUERIDO!");
            base.AddRule(() => DiasEfectivosLaboradosTotalDiasLaborados, () => DiasEfectivosLaboradosTotalDiasLaborados.HasValue, "TOTAL DE DIAS ES REQUERIDO!");
            base.AddRule(() => PeriodoDondeRealizoActividadLaboral, () => PeriodoDondeRealizoActividadLaboral.HasValue, "PERIODO EN EL QUE SE REALIZO LA ACTIVIDAD ES REQUERIDO!");
            base.AddRule(() => IdDicatmenSeguridadCustodiaDict, () => !string.IsNullOrEmpty(IdDicatmenSeguridadCustodiaDict), "DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FechaSeguridadCustodiaDictamen, () => FechaSeguridadCustodiaDictamen.HasValue, "FECHA DE ESTUDIO ES REQUERIDO!");
            base.AddRule(() => MotivacionDictamenSeguridadCustodia, () => !string.IsNullOrEmpty(MotivacionDictamenSeguridadCustodia), "MOTIVACION DE DICTAMEN ES REQUERIDO!");
            OnPropertyChanged("");
        }
        #endregion
        #region Validaciones Estudio Psiquiatrico Fuero Comun
        void ValidacionesEstudioPsiquiatricoComun()
        {
            base.ClearRules();
            base.AddRule(() => AspectoFisico, () => !string.IsNullOrEmpty(AspectoFisico), "ASPECTO FISICO ES REQUERIDO!");
            base.AddRule(() => ConductaMotora, () => !string.IsNullOrEmpty(ConductaMotora), "CONDUCTA MOTORA ES REQUERIDO!");
            base.AddRule(() => Habla, () => !string.IsNullOrEmpty(Habla), "HABLA ES REQUERIDO!");
            base.AddRule(() => Actitud, () => !string.IsNullOrEmpty(Actitud), "ACTITUD ES REQUERIDO!");
            base.AddRule(() => EstadoAnimo, () => !string.IsNullOrEmpty(EstadoAnimo), "ESTADO DE ANIMO ES REQUERIDO!");
            base.AddRule(() => ExpresionAfectiva, () => !string.IsNullOrEmpty(ExpresionAfectiva), "EXPRESION AFECTIVA ES REQUERIDO!");
            base.AddRule(() => Adecuacion, () => !string.IsNullOrEmpty(Adecuacion), "ADECUACION ES REQUERIDO!");
            base.AddRule(() => Ilusiones, () => !string.IsNullOrEmpty(Ilusiones), "ILUSIONES ES REQUERIDO!");
            base.AddRule(() => Alucinaciones, () => !string.IsNullOrEmpty(Alucinaciones), "ALUCINACIONES ES REQUERIDO!");
            base.AddRule(() => Despersonalizacion, () => !string.IsNullOrEmpty(Despersonalizacion), "DESPERSONALIZACION ES REQUERIDO!");
            base.AddRule(() => Desrealizacion, () => !string.IsNullOrEmpty(Desrealizacion), "DESREALIZACION ES REQUERIDO!");
            base.AddRule(() => CursoPensamiento, () => !string.IsNullOrEmpty(CursoPensamiento), "CURSO DEL PENSAMIENTO ES REQUERIDO!");
            base.AddRule(() => ContinuidadPensamiento, () => !string.IsNullOrEmpty(ContinuidadPensamiento), "CONTINUIDAD DEL PENSAMIENTO ES REQUERIDO!");
            base.AddRule(() => ContenidoPensamiento, () => !string.IsNullOrEmpty(ContenidoPensamiento), "CONTENIDO DEL PENSAMIENTO ES REQUERIDO!");
            base.AddRule(() => PensamientoAbstracto, () => !string.IsNullOrEmpty(PensamientoAbstracto), "PENSAMIENTO ABSTRACTO ES REQUERIDO!");
            base.AddRule(() => Concentracion, () => !string.IsNullOrEmpty(Concentracion), "CONCENTRACION ES REQUERIDO!");
            base.AddRule(() => BajaToleranciaFrustr, () => !string.IsNullOrEmpty(BajaToleranciaFrustr), "BAJA TOLERANCIA ALA FRUSTRACION ES REQUERIDO!");
            base.AddRule(() => ExpresionDesadaptativa, () => !string.IsNullOrEmpty(ExpresionDesadaptativa), "EXPRESION DESADAPTIVA ES REQUERIDO!");
            base.AddRule(() => Adecuada, () => !string.IsNullOrEmpty(Adecuada), "ADECUADA ES REQUERIDO!");
            base.AddRule(() => Orientacion, () => !string.IsNullOrEmpty(Orientacion), "ORIENTACION ES REQUERIDO!");
            base.AddRule(() => Memoria, () => !string.IsNullOrEmpty(Memoria), "MEMORIA ES REQUERIDO!");
            base.AddRule(() => CapacidadJuicio, () => !string.IsNullOrEmpty(CapacidadJuicio), "CAPACIDAD DE JUICIO ES REQUERIDO!");
            base.AddRule(() => Introspeccion, () => !string.IsNullOrEmpty(Introspeccion), "INTROSPECCION ES REQUERIDO!");
            base.AddRule(() => Fiabilidad, () => !string.IsNullOrEmpty(Fiabilidad), "FIABILIDAD ES REQUERIDO!");
            base.AddRule(() => ImpresionDiagnosticaPsiquiatricoComun, () => !string.IsNullOrEmpty(ImpresionDiagnosticaPsiquiatricoComun), "IMPRESION DIAGNOSTICA ES REQUERIDO!");
            base.AddRule(() => DictamenPsiqComun, () => DictamenPsiqComun.HasValue ? DictamenPsiqComun != 0 : false, "DICTAMEN DE RESULTADO ES REQUERIDO!");
            base.AddRule(() => FecDictamenPsiqComun, () => FecDictamenPsiqComun.HasValue, "FECHA DEL DICTAMEN ES REQUERIDO!");
            OnPropertyChanged("");
        }
        #endregion
        #region Estudio Psicologico de fuero comun
        void ValidacionesEstudioPsicologicoFueroComun()
        {
            base.ClearRules();

            base.AddRule(() => IdReqExtraMurosPsicologicoComun, () => !string.IsNullOrEmpty(IdReqExtraMurosPsicologicoComun), "REQUIERE TRATAMIENTO EXTRAMUROS ES REQUERIDO!");
            base.AddRule(() => CondicionesGralesInterno, () => !string.IsNullOrEmpty(CondicionesGralesInterno), "CONDICIONES GRALES ES REQUERIDO!");
            base.AddRule(() => ExamenMental, () => !string.IsNullOrEmpty(ExamenMental), "EXAMEN MENTAL ES REQUERIDO!");
            base.AddRule(() => DescripcionPrincipalesRazgosIngreso, () => !string.IsNullOrEmpty(DescripcionPrincipalesRazgosIngreso), "PRINCIPALES RASGOS ES REQUERIDO!");
            base.AddRule(() => IntegracionDinamicaPersonalidadActual, () => !string.IsNullOrEmpty(IntegracionDinamicaPersonalidadActual), "INTEGRACION DE LA PERSONALIDAD ES REQUERIDO!");
            base.AddRule(() => IdNivelIntelectual, () => IdNivelIntelectual.HasValue ? IdNivelIntelectual != -1 : false, "NIVEL INTELECTUAL ES REQUERIDO!");
            base.AddRule(() => IdDisfuncionNeurologica, () => IdDisfuncionNeurologica.HasValue ? IdDisfuncionNeurologica != -1 : false, "DISFUNCION NEUROLOGICA ES REQUERIDO!");
            base.AddRule(() => RasgosPersonalidadRelaciondosComisionDelito, () => !string.IsNullOrEmpty(RasgosPersonalidadRelaciondosComisionDelito), "RASGOS PERSONALIDAD ES REQUERIDO!");
            base.AddRule(() => IdDictamenPsicologicoComun, () => IdDictamenPsicologicoComun.HasValue ? IdDictamenPsicologicoComun != 0 : false, "DICTAMEN ES REQUERIDO!");
            base.AddRule(() => MotivacionDictamenPsicologicoComun, () => !string.IsNullOrEmpty(MotivacionDictamenPsicologicoComun), "MOTIVACION DEL DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FechaDictamenPsicologicoComun, () => FechaDictamenPsicologicoComun.HasValue, "FECHA DEL DICTAMEN ES REQUERIDO!");
            OnPropertyChanged("");
        }
        #endregion
        #region Validaciones Estudio Criminodiagnostico
        void ValidacionesEstudioCriminodiagnosticoComun()
        {
            base.ClearRules();
            base.AddRule(() => ReqTratamExtramurosCriminod, () => !string.IsNullOrEmpty(ReqTratamExtramurosCriminod), "REQUIERE TRATAMIENTO ES REQUERIDO!");
            base.AddRule(() => VersionDelitoSegunInterno, () => !string.IsNullOrEmpty(VersionDelitoSegunInterno), "VERSION DEL DELITO ES REQUERIDO!");
            base.AddRule(() => PorqueIntimidacionAntePenaImpuesta, () => !string.IsNullOrEmpty(PorqueIntimidacionAntePenaImpuesta), "PORQUE ES REQUERIDO!");
            base.AddRule(() => CriminoGenesisEstudioCriminoFC, () => !string.IsNullOrEmpty(CriminoGenesisEstudioCriminoFC), "CRIMINOGENESIS ES REQUERIDO!");
            base.AddRule(() => AntecedentesEvolucionConductasParaSociales, () => !string.IsNullOrEmpty(AntecedentesEvolucionConductasParaSociales), "ANTECEDENTES ES REQUERIDO!");
            base.AddRule(() => MotivacionDictamenCriminodiagnosticoComun, () => !string.IsNullOrEmpty(MotivacionDictamenCriminodiagnosticoComun), "MOTIVACION DEL DICTAMEN ES REQUERIDO!");
            base.AddRule(() => SenialeProgramasDebeRemitirseInterno, () => !string.IsNullOrEmpty(SenialeProgramasDebeRemitirseInterno), "PROGRAMAS A REMITIR ES REQUERIDO!");
            base.AddRule(() => IdEncontrabaBajoInfluenciaDroga, () => !string.IsNullOrEmpty(IdEncontrabaBajoInfluenciaDroga), "ENCONTRABA BAJO INFLUCENCIA DROGA ES REQUERIDO!");
            base.AddRule(() => IdClasificacionCriminologica, () => IdClasificacionCriminologica.HasValue ? IdClasificacionCriminologica != -1 : false, "CLASIFICACION CRIMINOLOGICA ES REQUERIDO!");
            base.AddRule(() => IntimidacionAntePenaImpuesta, () => !string.IsNullOrEmpty(IntimidacionAntePenaImpuesta), "INTIMIDACION ANTE PENA ES REQUERIDO!");
            base.AddRule(() => IdEgocentrismo, () => IdEgocentrismo.HasValue ? IdEgocentrismo != -1 : false, "EGOCENTRISMO ES REQUERIDO!");
            base.AddRule(() => IdLabilidadAfectiva, () => IdLabilidadAfectiva.HasValue ? IdLabilidadAfectiva != -1 : false, "LABILIDAD AFECTIVA ES REQUERIDO!");
            base.AddRule(() => IdAgresividad, () => IdAgresividad.HasValue ? IdAgresividad != -1 : false, "AGRESIVIDAD ES REQUERIDO!");
            base.AddRule(() => IdIndiferenciaAfectiva, () => IdIndiferenciaAfectiva.HasValue ? IdIndiferenciaAfectiva != -1 : false, "INDIFERENCIA AFECTIVA ES REQUERIDO!");
            base.AddRule(() => IdCapacidadCriminologicaActual, () => IdCapacidadCriminologicaActual.HasValue ? IdCapacidadCriminologicaActual != -1 : false, "CAPACIDAD CRIMINOLOGICA ACTUAL ES REQUERIDO!");
            base.AddRule(() => IdAdaptabilidadSocial, () => IdAdaptabilidadSocial.HasValue ? IdAdaptabilidadSocial != -1 : false, "ADAPTABILIDAD SOCIAL ES REQUERIDO!");
            base.AddRule(() => IdIndicePeligrosidadCriminologicaActual, () => IdIndicePeligrosidadCriminologicaActual.HasValue ? IdIndicePeligrosidadCriminologicaActual != -1 : false, "INDICE DE PELIGROSIDAD ACTUAL ES REQUERIDO!");
            base.AddRule(() => IdPronosticoReincidencia, () => IdPronosticoReincidencia.HasValue ? IdPronosticoReincidencia != -1 : false, "PRONOSTICO DE REINCIDENCIA ES REQUERIDO!");
            base.AddRule(() => DictamenCriminod, () => DictamenCriminod.HasValue ? DictamenCriminod != -1 : false, "DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FechaDictamenCrimino, () => FechaDictamenCrimino.HasValue, "FECHA DE ESTUDIO ES REQUERIDO!");
            OnPropertyChanged("");
        }
        #endregion
        #region Validaciones Estudio Socioeconomico fuero comun
        void ValidacionesEstudioSocioEconomicoComun()
        {
            base.ClearRules();
            base.AddRule(() => FamiliaPrimaria, () => !string.IsNullOrEmpty(FamiliaPrimaria), "FAMILIA PRIMARIA ES REQUERIDO!");
            base.AddRule(() => FamiliaSecundaria, () => !string.IsNullOrEmpty(FamiliaSecundaria), "FAMILIA SECUNDARIA ES REQUERIDO!");
            base.AddRule(() => IdRecibeVisitaSocioFamComun, () => !string.IsNullOrEmpty(IdRecibeVisitaSocioFamComun), "RECIBE VISITAS ES REQUERIDO!");
            base.AddRule(() => IdComunicacionViaTelChecked, () => !string.IsNullOrEmpty(IdComunicacionViaTelChecked), "MANTIENE COMUNICACION VIA TELEFONICA ES REQUERIDO!");
            base.AddRule(() => ApoyosRecibeExterior, () => !string.IsNullOrEmpty(ApoyosRecibeExterior), "APOYOS QUE RECIBE DEL EXTERIOR ES REQUERIDO!");
            base.AddRule(() => PlanesInternoAlSerExternado, () => !string.IsNullOrEmpty(PlanesInternoAlSerExternado), "PLANES DEL INTERNO AL SER EXTERNADO ES REQUERIDO!");
            base.AddRule(() => ConQuienVivirSerExternado, () => !string.IsNullOrEmpty(ConQuienVivirSerExternado), "CON QUIEN VIVIRA AL SER EXTRENADO ES REQUERIDO!");
            base.AddRule(() => IdOfertaTrabajoChecked, () => !string.IsNullOrEmpty(IdOfertaTrabajoChecked), "CUENTA CON OFERTA DE TRABAJO ES REQUERIDO!");
            base.AddRule(() => IdAvalMoralChecked, () => !string.IsNullOrEmpty(IdAvalMoralChecked), "CUENTA CON AVAL MORAL ES REQUERIDO!");
            base.AddRule(() => IdDictamenSocioSocioFamComun, () => IdDictamenSocioSocioFamComun.HasValue ? IdDictamenSocioSocioFamComun != -1 : false, "DICTAMEN SOCIO FAMILIAR ES REQUERIDO!");
            base.AddRule(() => MotivacionDictamenSocioEconomicoComun, () => !string.IsNullOrEmpty(MotivacionDictamenSocioEconomicoComun), "MOTIVACION DEL DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FechaEstudioSocioFamiliarComun, () => FechaEstudioSocioFamiliarComun.HasValue, "FECHA DE REALIZACION DEL ESTUDIO ES REQUERIDO!");
            base.AddRule(() => IsAdultoMayorParticipoEnProgramaEspecial, () => !string.IsNullOrEmpty(IsAdultoMayorParticipoEnProgramaEspecial), "ADULTO MAYOR ES REQUERIDO!");
            base.AddRule(() => IdDictamenSocioFamComun, () => IdDictamenSocioFamComun.HasValue ? IdDictamenSocioFamComun != 0 : false, "DICTAMEN ES REQUERIDO!");
            OnPropertyChanged("");
        }

        #endregion
        #region EstudiosEducacitov Capac. laboral
        void ValidacionesEstudioEducCapLab()
        {
            base.ClearRules();
            base.AddRule(() => IdDictamenEducativoComun, () => IdDictamenEducativoComun.HasValue ? IdDictamenEducativoComun.Value != 0 : false, "DICTAMEN ES REQUERIDO!");
            base.AddRule(() => MotivacionDictamenEducativoComun, () => !string.IsNullOrEmpty(MotivacionDictamenEducativoComun), "MOTIVACION DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FechaEstudioEducativoComun, () => FechaEstudioEducativoComun.HasValue, "FECHA DICTAMEN ES REQUERIDO!");
            OnPropertyChanged("");
        }
        #endregion
        #region Estudio Seguridad
        void ValidacionInformeSeguridad()
        {
            base.ClearRules();
            base.AddRule(() => MotivacionDictamenInformeSeguridadCustodia, () => !string.IsNullOrEmpty(MotivacionDictamenInformeSeguridadCustodia), "MOTIVACION DICTAMEN ES REQUERIDO!");
            base.AddRule(() => IdDictamenInformeSeguridadCustodia, () => !string.IsNullOrEmpty(IdDictamenInformeSeguridadCustodia), "DICTAMEN ES REQUERIDO!");
            base.AddRule(() => IdConductaObservadaCentro, () => !string.IsNullOrEmpty(IdConductaObservadaCentro), "CONDUCTA ES REQUERIDO!");
            base.AddRule(() => IdConductaAutoridad, () => !string.IsNullOrEmpty(IdConductaAutoridad), "CONDUCTA AUTORIDAD ES REQUERIDO!");
            base.AddRule(() => IdRegistraCorrectivosDisciplinarios, () => !string.IsNullOrEmpty(IdRegistraCorrectivosDisciplinarios), "REGISTRA CORRECTIVOS ES REQUERIDO!");
            base.AddRule(() => IdOpinionGralConductaInterno, () => !string.IsNullOrEmpty(IdOpinionGralConductaInterno), "OPINION ES REQUERIDO!");
            base.AddRule(() => MotivacionDictamenEducativoComun, () => !string.IsNullOrEmpty(MotivacionDictamenEducativoComun), "MOTIVACION DICTAMEN ES REQUERIDO!");
            base.AddRule(() => IdConductaGral, () => IdConductaGral.HasValue ? IdConductaGral.Value != -1 : false, "CONDUCTA GENERAL ES REQUERIDO!");
            base.AddRule(() => IdRelacionCompanieros, () => IdRelacionCompanieros.HasValue ? IdRelacionCompanieros.Value != -1 : false, "RELACION COMPANEROS ES REQUERIDO!");
            base.AddRule(() => FechaDictamenInformeSeguridadCustodia, () => FechaDictamenInformeSeguridadCustodia.HasValue, "FECHA DICTAMEN ES REQUERIDO!");
            OnPropertyChanged("");
        }
        #endregion
        #endregion

        #region Validacion Superusuario
        void ValidacionesSuperUsuario()
        {
            base.ClearRules();
            base.AddRule(() => FamiliaPrimaria, () => !string.IsNullOrEmpty(FamiliaPrimaria), "FAMILIA PRIMARIA ES REQUERIDO!");
            base.AddRule(() => FamiliaSecundaria, () => !string.IsNullOrEmpty(FamiliaSecundaria), "FAMILIA SECUNDARIA ES REQUERIDO!");
            base.AddRule(() => IdRecibeVisitaSocioFamComun, () => !string.IsNullOrEmpty(IdRecibeVisitaSocioFamComun), "RECIBE VISITAS ES REQUERIDO!");
            base.AddRule(() => IdComunicacionViaTelChecked, () => !string.IsNullOrEmpty(IdComunicacionViaTelChecked), "MANTIENE COMUNICACION VIA TELEFONICA ES REQUERIDO!");
            base.AddRule(() => ApoyosRecibeExterior, () => !string.IsNullOrEmpty(ApoyosRecibeExterior), "APOYOS QUE RECIBE DEL EXTERIOR ES REQUERIDO!");
            base.AddRule(() => PlanesInternoAlSerExternado, () => !string.IsNullOrEmpty(PlanesInternoAlSerExternado), "PLANES DEL INTERNO AL SER EXTERNADO ES REQUERIDO!");
            base.AddRule(() => ConQuienVivirSerExternado, () => !string.IsNullOrEmpty(ConQuienVivirSerExternado), "CON QUIEN VIVIRA AL SER EXTRENADO ES REQUERIDO!");
            base.AddRule(() => IdOfertaTrabajoChecked, () => !string.IsNullOrEmpty(IdOfertaTrabajoChecked), "CUENTA CON OFERTA DE TRABAJO ES REQUERIDO!");
            base.AddRule(() => IdAvalMoralChecked, () => !string.IsNullOrEmpty(IdAvalMoralChecked), "CUENTA CON AVAL MORAL ES REQUERIDO!");
            base.AddRule(() => IdDictamenSocioSocioFamComun, () => IdDictamenSocioSocioFamComun.HasValue ? IdDictamenSocioSocioFamComun != -1 : false, "DICTAMEN SOCIO FAMILIAR ES REQUERIDO!");
            base.AddRule(() => MotivacionDictamenSocioEconomicoComun, () => !string.IsNullOrEmpty(MotivacionDictamenSocioEconomicoComun), "MOTIVACION DEL DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FecRealizacionEstudioSocioFamiliar, () => FecRealizacionEstudioSocioFamiliar.HasValue, "FECHA DE REALIZACION DEL ESTUDIO ES REQUERIDO!");
            base.AddRule(() => VersionDelitoSegunInterno, () => !string.IsNullOrEmpty(VersionDelitoSegunInterno), "VERSION DEL DELITO ES REQUERIDO!");
            base.AddRule(() => CriminoGenesisEstudioCriminoFC, () => !string.IsNullOrEmpty(CriminoGenesisEstudioCriminoFC), "CRIMINOGENESIS ES REQUERIDO!");
            base.AddRule(() => AntecedentesEvolucionConductasParaSociales, () => !string.IsNullOrEmpty(AntecedentesEvolucionConductasParaSociales), "ANTECEDENTES ES REQUERIDO!");
            base.AddRule(() => PorqueIntimidacionAntePenaImpuesta, () => !string.IsNullOrEmpty(PorqueIntimidacionAntePenaImpuesta), "PORQUE DE  ES REQUERIDO!");
            base.AddRule(() => MotivacionDictamenCriminodiagnosticoComun, () => !string.IsNullOrEmpty(MotivacionDictamenCriminodiagnosticoComun), "MOTIVACION DEL DICTAMEN ES REQUERIDO!");
            base.AddRule(() => SenialeProgramasDebeRemitirseInterno, () => !string.IsNullOrEmpty(SenialeProgramasDebeRemitirseInterno), "PROGRAMAS A REMITIR ES REQUERIDO!");
            base.AddRule(() => IdEncontrabaBajoInfluenciaDroga, () => !string.IsNullOrEmpty(IdEncontrabaBajoInfluenciaDroga), "ENCONTRABA BAJO INFLUCENCIA DROGA ES REQUERIDO!");
            base.AddRule(() => IdClasificacionCriminologica, () => IdClasificacionCriminologica.HasValue ? IdClasificacionCriminologica != -1 : false, "CLASIFICACION CRIMINOLOGICA ES REQUERIDO!");
            base.AddRule(() => IntimidacionAntePenaImpuesta, () => !string.IsNullOrEmpty(IntimidacionAntePenaImpuesta), "INTIMIDACION ANTE PENA ES REQUERIDO!");
            base.AddRule(() => Alucinaciones, () => !string.IsNullOrEmpty(Alucinaciones), "ALUCINACIONES ES REQUERIDO!");
            base.AddRule(() => DictamenPsiqComun, () => DictamenPsiqComun.HasValue ? DictamenPsiqComun != 0 : false, "DICTAMEN DE RESULTADO ES REQUERIDO!");

            base.AddRule(() => IdEgocentrismo, () => IdEgocentrismo.HasValue ? IdEgocentrismo != -1 : false, "EGOCENTRISMO ES REQUERIDO!");
            base.AddRule(() => IdLabilidadAfectiva, () => IdLabilidadAfectiva.HasValue ? IdLabilidadAfectiva != -1 : false, "LABILIDAD AFECTIVA ES REQUERIDO!");
            base.AddRule(() => IdAgresividad, () => IdAgresividad.HasValue ? IdAgresividad != -1 : false, "AGRESIVIDAD ES REQUERIDO!");
            base.AddRule(() => IdIndiferenciaAfectiva, () => IdIndiferenciaAfectiva.HasValue ? IdIndiferenciaAfectiva != -1 : false, "INDIFERENCIA AFECTIVA ES REQUERIDO!");
            base.AddRule(() => IdCapacidadCriminologicaActual, () => IdCapacidadCriminologicaActual.HasValue ? IdCapacidadCriminologicaActual != -1 : false, "CAPACIDAD CRIMINOLOGICA ACTUAL ES REQUERIDO!");
            base.AddRule(() => IdAdaptabilidadSocial, () => IdAdaptabilidadSocial.HasValue ? IdAdaptabilidadSocial != -1 : false, "ADAPTABILIDAD SOCIAL ES REQUERIDO!");
            base.AddRule(() => IdIndicePeligrosidadCriminologicaActual, () => IdIndicePeligrosidadCriminologicaActual.HasValue ? IdIndicePeligrosidadCriminologicaActual != -1 : false, "INDICE DE PELIGROSIDAD ACTUAL ES REQUERIDO!");
            base.AddRule(() => IdPronosticoReincidencia, () => IdPronosticoReincidencia.HasValue ? IdPronosticoReincidencia != -1 : false, "PRONOSTICO DE REINCIDENCIA ES REQUERIDO!");
            base.AddRule(() => DictamenCriminod, () => DictamenCriminod.HasValue ? DictamenCriminod != -1 : false, "DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FechaDictamenCrimino, () => FechaDictamenCrimino.HasValue, "FECHA DE ESTUDIO ES REQUERIDO!");
            base.AddRule(() => CondicionesGralesInterno, () => !string.IsNullOrEmpty(CondicionesGralesInterno), "CONDICIONES GRALES ES REQUERIDO!");
            base.AddRule(() => ExamenMental, () => !string.IsNullOrEmpty(ExamenMental), "EXAMEN MENTAL ES REQUERIDO!");
            base.AddRule(() => DescripcionPrincipalesRazgosIngreso, () => !string.IsNullOrEmpty(DescripcionPrincipalesRazgosIngreso), "PRINCIPALES RASGOS ES REQUERIDO!");
            base.AddRule(() => IntegracionDinamicaPersonalidadActual, () => !string.IsNullOrEmpty(IntegracionDinamicaPersonalidadActual), "INTEGRACION DE LA PERSONALIDAD ES REQUERIDO!");
            base.AddRule(() => IdNivelIntelectual, () => IdNivelIntelectual.HasValue ? IdNivelIntelectual != -1 : false, "NIVEL INTELECTUAL ES REQUERIDO!");
            base.AddRule(() => IdDisfuncionNeurologica, () => IdDisfuncionNeurologica.HasValue ? IdDisfuncionNeurologica != -1 : false, "DISFUNCION NEUROLOGICA ES REQUERIDO!");
            base.AddRule(() => RasgosPersonalidadRelaciondosComisionDelito, () => !string.IsNullOrEmpty(RasgosPersonalidadRelaciondosComisionDelito), "RASGOS PERSONALIDAD ES REQUERIDO!");
            base.AddRule(() => IdDictamenPsicologicoComun, () => IdDictamenPsicologicoComun.HasValue ? IdDictamenPsicologicoComun != -1 : false, "DICTAMEN ES REQUERIDO!");
            base.AddRule(() => MotivacionDictamenPsicologicoComun, () => !string.IsNullOrEmpty(MotivacionDictamenPsicologicoComun), "MOTIVACION DEL DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FechaDictamenPsicologicoComun, () => FechaDictamenPsicologicoComun.HasValue, "FECHA DEL DICTAMEN ES REQUERIDO!");
            base.AddRule(() => AspectoFisico, () => !string.IsNullOrEmpty(AspectoFisico), "ASPECTO FISICO ES REQUERIDO!");
            base.AddRule(() => ConductaMotora, () => !string.IsNullOrEmpty(ConductaMotora), "CONDUCTA MOTORA ES REQUERIDO!");
            base.AddRule(() => Habla, () => !string.IsNullOrEmpty(Habla), "HABLA ES REQUERIDO!");
            base.AddRule(() => Actitud, () => !string.IsNullOrEmpty(Actitud), "ACTITUD ES REQUERIDO!");
            base.AddRule(() => EstadoAnimo, () => !string.IsNullOrEmpty(EstadoAnimo), "ESTADO DE ANIMO ES REQUERIDO!");
            base.AddRule(() => ExpresionAfectiva, () => !string.IsNullOrEmpty(ExpresionAfectiva), "EXPRESION AFECTIVA ES REQUERIDO!");
            base.AddRule(() => Adecuacion, () => !string.IsNullOrEmpty(Adecuacion), "ADECUACION ES REQUERIDO!");
            base.AddRule(() => Ilusiones, () => !string.IsNullOrEmpty(Ilusiones), "ILUSIONES ES REQUERIDO!");
            base.AddRule(() => Despersonalizacion, () => !string.IsNullOrEmpty(Despersonalizacion), "DESPERSONALIZACION ES REQUERIDO!");
            base.AddRule(() => Desrealizacion, () => !string.IsNullOrEmpty(Desrealizacion), "DESREALIZACION ES REQUERIDO!");
            base.AddRule(() => CursoPensamiento, () => !string.IsNullOrEmpty(CursoPensamiento), "CURSO DEL PENSAMIENTO ES REQUERIDO!");
            base.AddRule(() => ContinuidadPensamiento, () => !string.IsNullOrEmpty(ContinuidadPensamiento), "CONTINUIDAD DEL PENSAMIENTO ES REQUERIDO!");
            base.AddRule(() => ContenidoPensamiento, () => !string.IsNullOrEmpty(ContenidoPensamiento), "CONTENIDO DEL PENSAMIENTO ES REQUERIDO!");
            base.AddRule(() => PensamientoAbstracto, () => !string.IsNullOrEmpty(PensamientoAbstracto), "PENSAMIENTO ABSTRACTO ES REQUERIDO!");
            base.AddRule(() => Concentracion, () => !string.IsNullOrEmpty(Concentracion), "CONCENTRACION ES REQUERIDO!");
            base.AddRule(() => BajaToleranciaFrustr, () => !string.IsNullOrEmpty(BajaToleranciaFrustr), "BAJA TOLERANCIA ALA FRUSTRACION ES REQUERIDO!");
            base.AddRule(() => ExpresionDesadaptativa, () => !string.IsNullOrEmpty(ExpresionDesadaptativa), "EXPRESION DESADAPTIVA ES REQUERIDO!");
            base.AddRule(() => Adecuada, () => !string.IsNullOrEmpty(Adecuada), "ADECUADA ES REQUERIDO!");
            base.AddRule(() => Orientacion, () => !string.IsNullOrEmpty(Orientacion), "ORIENTACION ES REQUERIDO!");
            base.AddRule(() => Memoria, () => !string.IsNullOrEmpty(Memoria), "MEMORIA ES REQUERIDO!");
            base.AddRule(() => CapacidadJuicio, () => !string.IsNullOrEmpty(CapacidadJuicio), "CAPACIDAD DE JUICIO ES REQUERIDO!");
            base.AddRule(() => Introspeccion, () => !string.IsNullOrEmpty(Introspeccion), "INTROSPECCION ES REQUERIDO!");
            base.AddRule(() => Fiabilidad, () => !string.IsNullOrEmpty(Fiabilidad), "FIABILIDAD ES REQUERIDO!");
            base.AddRule(() => ImpresionDiagnosticaPsiquiatricoComun, () => !string.IsNullOrEmpty(ImpresionDiagnosticaPsiquiatricoComun), "IMPRESION DIAGNOSTICA ES REQUERIDO!");
            base.AddRule(() => IdDictamenResultado, () => IdDictamenResultado.HasValue ? IdDictamenResultado != -1 : false, "DICTAMEN DE RESULTADO ES REQUERIDO!");
            base.AddRule(() => FecDictamenPsiqComun, () => FecDictamenPsiqComun.HasValue, "FECHA DEL DICTAMEN ES REQUERIDO!");
            base.AddRule(() => ActividadOficioAntesReclusion, () => !string.IsNullOrEmpty(ActividadOficioAntesReclusion), "ACTIVIDAD U OFICIO ANTES DE RECLUSION ES REQUERIDO!");
            base.AddRule(() => IdResponsabilidad, () => !string.IsNullOrEmpty(IdResponsabilidad), "RESPONSABILIDAD ES REQUERIDO!");
            base.AddRule(() => IdCalidadTrabajo, () => !string.IsNullOrEmpty(IdCalidadTrabajo), "CALIDAD EN EL TRABAJO ES REQUERIDO!");
            base.AddRule(() => IdPerseverancia, () => !string.IsNullOrEmpty(IdPerseverancia), "PERSEVERANCIA ES REQUERIDO!");
            base.AddRule(() => IdCuentaConFondoAhorro, () => !string.IsNullOrEmpty(IdCuentaConFondoAhorro), "CUENTA CON FONDO DE AHORRO ES REQUERIDO!");
            base.AddRule(() => DiasEfectivosLaboradosEnOtrosCentros, () => DiasEfectivosLaboradosEnOtrosCentros.HasValue, "DIAS EFECTIVOS LABORADOS EN TROS CENTROS ES REQUERIDO!");
            base.AddRule(() => DiasEfectivosLaboradosEnCentroActual, () => DiasEfectivosLaboradosEnCentroActual.HasValue, "DIAS LABORADOS EN EL CENTRO ACTUAL ES REQUERIDO!");
            base.AddRule(() => DiasEfectivosLaboradosTotalDiasLaborados, () => DiasEfectivosLaboradosTotalDiasLaborados.HasValue, "TOTAL DE DIAS ES REQUERIDO!");
            base.AddRule(() => PeriodoDondeRealizoActividadLaboral, () => PeriodoDondeRealizoActividadLaboral.HasValue, "PERIODO EN EL QUE SE REALIZO LA ACTIVIDAD ES REQUERIDO!");
            base.AddRule(() => IdDicatmenSeguridadCustodiaDict, () => !string.IsNullOrEmpty(IdDicatmenSeguridadCustodiaDict), "DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FechaSeguridadCustodiaDictamen, () => FechaSeguridadCustodiaDictamen.HasValue, "FECHA DE ESTUDIO ES REQUERIDO!");
            base.AddRule(() => MotivacionDictamenSeguridadCustodia, () => !string.IsNullOrEmpty(MotivacionDictamenSeguridadCustodia), "MOTIVACION DE DICTAMEN ES REQUERIDO!");
            base.AddRule(() => AntecedentesHeredoFamiliares, () => !string.IsNullOrEmpty(AntecedentesHeredoFamiliares), "ANTECEDENTES FAMILIARES SON REQUERIDOS!");
            base.AddRule(() => AntecedentesPersonalesNoPatologicos, () => !string.IsNullOrEmpty(AntecedentesPersonalesNoPatologicos), "ANTECEDENTES PERSONALES NO PATOLOGICOS SON REQUERIDOS!");
            base.AddRule(() => AntedecentesConsumoToxicosEstadoActual, () => !string.IsNullOrEmpty(AntedecentesConsumoToxicosEstadoActual), "ANTECEDENTES DE CONSUMO DE TOXICOS SON REQUERIDOS!");
            base.AddRule(() => DescripcionTatuajesCicatricesMalformaciones, () => !string.IsNullOrEmpty(DescripcionTatuajesCicatricesMalformaciones), "DESCR. DE TATUAJES SON REQUERIDOS!");
            base.AddRule(() => AntecedentesPatologicos, () => !string.IsNullOrEmpty(AntecedentesPatologicos), "ANTECEDENTES PATOLOGICOS SON REQUERIDOS!");
            base.AddRule(() => DescipcionPadecimientoActual, () => !string.IsNullOrEmpty(DescipcionPadecimientoActual), "PADECIMIENTO ACTUAL ES REQUERIDO!");
            base.AddRule(() => TensionArterialGenerico, () => !string.IsNullOrEmpty(TensionArterialGenerico), "TENSION ARTERIAL ES REQUERIDO!");
            base.AddRule(() => TemperaturaGenerico, () => !string.IsNullOrEmpty(TemperaturaGenerico), "TEMPERATURA ES REQUERIDO!");
            base.AddRule(() => PulsoGenerico, () => !string.IsNullOrEmpty(PulsoGenerico), "PULSO ES REQUERIDO!");
            base.AddRule(() => RespiracionGenerico, () => !string.IsNullOrEmpty(RespiracionGenerico), "RESPIRACION ES REQUERIDO!");
            base.AddRule(() => PesoGenerico, () => !string.IsNullOrEmpty(PesoGenerico), "PESO ES REQUERIDO!");
            base.AddRule(() => EstaturaGenerico, () => !string.IsNullOrEmpty(EstaturaGenerico), "ESTATURA ES REQUERIDO!");
            base.AddRule(() => IdDictamenMedicoComun, () => IdDictamenMedicoComun.HasValue ? IdDictamenMedicoComun != 0 : false, "DICTAMEN ES REQUERIDO!");
            base.AddRule(() => FechaEstudioMedicoComun, () => FechaEstudioMedicoComun.HasValue, "FECHA DE ESTUDIO ES REQUERIDO!");
            base.AddRule(() => ImpresionDiagnosticaEstudioMedicoComun, () => !string.IsNullOrEmpty(ImpresionDiagnosticaEstudioMedicoComun), "IMPRESION DIAGNOSTICA DE ESTUDIO MEDICO ES REQUERIDO!");
            OnPropertyChanged("");
        }
        #endregion
        #region Validacion Escolaridad
        void ValidacionesEscolaridad()
        {
            base.ClearRules();
            base.AddRule(() => IdEducativo, () => IdEducativo != -1, "NIVEL EDUCATIVO ES REQUERIDO!");
            base.AddRule(() => Concluida, () => !string.IsNullOrEmpty(Concluida), "CONCLUIDA ES REQUERIDO!");
            base.AddRule(() => ObservacionesEducacion, () => !string.IsNullOrEmpty(ObservacionesEducacion), "OBSERVACIONES ES REQUERIDO!");
            OnPropertyChanged("IdEducativo");
            OnPropertyChanged("Concluida");
            OnPropertyChanged("ObservacionesEducacion");
        }

        void LimpiaValidacionesEscolaridad()
        {
            try
            {
                base.RemoveRule("IdEducativo");
                base.RemoveRule("Concluida");
                base.RemoveRule("ObservacionesEducacion");
                OnPropertyChanged("IdEducativo");
                OnPropertyChanged("Concluida");
                OnPropertyChanged("ObservacionesEducacion");
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }

        void ValidacionesActividadesEducativas()
        {
            base.ClearRules();
            base.AddRule(() => IdNivelEducativoActiv, () => IdNivelEducativoActiv != -1, "NIVEL EDUCATIVO ES REQUERIDO!");
            base.AddRule(() => IdConcluidaActivEducativa, () => !string.IsNullOrEmpty(IdConcluidaActivEducativa), "CONCLUIDA ES REQUERIDO!");
            base.AddRule(() => IdRendimientoActivEducativo, () => !string.IsNullOrEmpty(IdRendimientoActivEducativo), "RENDIMIENTO ES REQUERIDO!");
            base.AddRule(() => IdInteresActivEducativo, () => !string.IsNullOrEmpty(IdInteresActivEducativo), "INTERES ES REQUERIDO!");
            base.AddRule(() => ObservacionesActivEducativa, () => !string.IsNullOrEmpty(ObservacionesActivEducativa), "OBSERVACIONES ES REQUERIDO!");
            OnPropertyChanged("IdNivelEducativoActiv");
            OnPropertyChanged("IdConcluidaActivEducativa");
            OnPropertyChanged("IdRendimientoActivEducativo");
            OnPropertyChanged("IdInteresActivEducativo");
            OnPropertyChanged("ObservacionesActivEducativa");
        }

        void LimpiarValidacionesActiviEducativas()
        {
            try
            {
                base.RemoveRule("IdNivelEducativoActiv");
                base.RemoveRule("IdConcluidaActivEducativa");
                base.RemoveRule("IdRendimientoActivEducativo");
                base.RemoveRule("IdInteresActivEducativo");
                base.RemoveRule("ObservacionesActivEducativa");
                OnPropertyChanged("IdNivelEducativoActiv");
                OnPropertyChanged("IdConcluidaActivEducativa");
                OnPropertyChanged("IdRendimientoActivEducativo");
                OnPropertyChanged("IdInteresActivEducativo");
                OnPropertyChanged("ObservacionesActivEducativa");
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }
        #endregion
        #region Validacion ACtividades Culturales Deportivas
        void ValidacionesActividadesCulturalesDeportivas()
        {
            base.ClearRules();
            base.AddRule(() => SelectedPrograma, () => SelectedPrograma != decimal.Zero, "PROGRAMA ES REQUERIDO!");
            base.AddRule(() => DescripcionActividad, () => !string.IsNullOrEmpty(DescripcionActividad), "ACTIVIDAD ES REQUERIDO!");
            base.AddRule(() => DescripcionDuracion, () => !string.IsNullOrEmpty(DescripcionDuracion), "DURACION ES REQUERIDO!");
            base.AddRule(() => DescripcionObservacionesActiv, () => !string.IsNullOrEmpty(DescripcionObservacionesActiv), "OBSERVACIONES ES REQUERIDO!");
            OnPropertyChanged("SelectedPrograma");
            OnPropertyChanged("DescripcionActividad");
            OnPropertyChanged("DescripcionDuracion");
            OnPropertyChanged("DescripcionObservacionesActiv");
        }

        void LimpiaValidacionesActividadesCulturalesDeportivas() 
        {
            base.RemoveRule("SelectedPrograma");
            base.RemoveRule("DescripcionActividad");
            base.RemoveRule("DescripcionDuracion");
            base.RemoveRule("DescripcionObservacionesActiv");
            OnPropertyChanged("SelectedPrograma");
            OnPropertyChanged("DescripcionActividad");
            OnPropertyChanged("DescripcionDuracion");
            OnPropertyChanged("DescripcionObservacionesActiv");
        }
        #endregion
        #region Validaciones Actividades Laborales
        void ValidacionesActivLaborales()
        {
            base.ClearRules();
            base.AddRule(() => ObservacionesActiv, () => !string.IsNullOrEmpty(ObservacionesActiv), "OSERVACIONES SON REQUERIDAS!");
            base.AddRule(() => DescripcionPeriodo, () => !string.IsNullOrEmpty(DescripcionPeriodo), "PERIODO ES REQUERIDO!");
            base.AddRule(() => IdCapac, () => IdCapac.HasValue ? IdCapac.Value != -1 : false, "TALLER ES REQUERIDO!");
            OnPropertyChanged("ObservacionesActiv");
            OnPropertyChanged("DescripcionPeriodo");
            OnPropertyChanged("IdCapac");
        }

        void LimpiarValidacionesLaborales()
        {
            try
            {
                base.RemoveRule("ObservacionesActiv");
                base.RemoveRule("DescripcionPeriodo");
                base.RemoveRule("IdCapac");
                base.RemoveRule("ConcluyoActiv");
                OnPropertyChanged("ObservacionesActiv");
                OnPropertyChanged("DescripcionPeriodo");
                OnPropertyChanged("IdCapac");
                OnPropertyChanged("ConcluyoActiv");
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }
        #endregion
        #region Validacion Capacitacion Laboral
        void ValidacionCapacLaboral()
        {
            base.ClearRules();
            base.AddRule(() => ObservacionesActiv, () => !string.IsNullOrEmpty(ObservacionesActiv), "OSERVACIONES SON REQUERIDAS!");
            base.AddRule(() => DescripcionPeriodo, () => !string.IsNullOrEmpty(DescripcionPeriodo), "PERIODO ES REQUERIDO!");
            base.AddRule(() => ConcluyoActiv, () => !string.IsNullOrEmpty(ConcluyoActiv), "CONCLUYO ES REQUERIDO!");
            base.AddRule(() => IdCapac, () => IdCapac.HasValue ? IdCapac.Value != -1 : false, "TALLER ES REQUERIDO!");
            OnPropertyChanged("ObservacionesActiv");
            OnPropertyChanged("DescripcionPeriodo");
            OnPropertyChanged("ConcluyoActiv");
            OnPropertyChanged("IdCapac");
        }

        void LimpiaValidacionCapacLaboral() 
        {
            base.RemoveRule("ObservacionesActiv");
            base.RemoveRule("DescripcionPeriodo");
            base.RemoveRule("ConcluyoActiv");
            base.RemoveRule("IdCapac");
            OnPropertyChanged("ObservacionesActiv");
            OnPropertyChanged("DescripcionPeriodo");
            OnPropertyChanged("ConcluyoActiv");
            OnPropertyChanged("IdCapac");
        }
        #endregion
        #region Validaciones Grupos Psicologico
        void ValidacionesGrupoIV()
        {
            base.ClearRules();
            base.AddRule(() => DuracionrupoIV, () => !string.IsNullOrEmpty(DuracionrupoIV), "DURACIONES REQUERIDA!");
            base.AddRule(() => ObservacionesGrupoIV, () => !string.IsNullOrEmpty(ObservacionesGrupoIV), "OBSERVACIONES ES REQUERIDO!");
            base.AddRule(() => ConcluidoGrupoIV, () => !string.IsNullOrEmpty(ConcluidoGrupoIV), "CONCLUIDA ES REQUERIDO!");
            OnPropertyChanged("DuracionrupoIV");
            OnPropertyChanged("ObservacionesGrupoIV");
            OnPropertyChanged("ConcluidoGrupoIV");
        }

        void LimpiaValidacionesGrupoIV()
        {
            try
            {
                base.RemoveRule("DuracionrupoIV");
                base.RemoveRule("ObservacionesGrupoIV");
                base.RemoveRule("ConcluidoGrupoIV");
                OnPropertyChanged("DuracionrupoIV");
                OnPropertyChanged("ObservacionesGrupoIV");
                OnPropertyChanged("ConcluidoGrupoIV");
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }
        #endregion
        #region Validaciones Congregacion
        void ValidacionesCongreg()
        {
            base.ClearRules();
            base.AddRule(() => CongregSocFC, () => !string.IsNullOrEmpty(CongregSocFC), "CONGREGACION REQUERIDA!");
            base.AddRule(() => PeriodoSocFC, () => !string.IsNullOrEmpty(PeriodoSocFC), "PERIODO ES REQUERIDO!");
            base.AddRule(() => ObservacionesSocFC, () => !string.IsNullOrEmpty(ObservacionesSocFC), "OBSERVACIONES ES REQUERIDO!");
            OnPropertyChanged("CongregSocFC");
            OnPropertyChanged("PeriodoSocFC");
            OnPropertyChanged("ObservacionesSocFC");
        }

        void LimpiaValidacionesCongreg() 
        {
            base.RemoveRule("CongregSocFC");
            base.RemoveRule("PeriodoSocFC");
            base.RemoveRule("ObservacionesSocFC");
            OnPropertyChanged("CongregSocFC");
            OnPropertyChanged("PeriodoSocFC");
            OnPropertyChanged("ObservacionesSocFC");

        }

        void ValidacionesFortFam()
        {
            base.ClearRules();
            base.AddRule(() => PeriodoSocFC, () => !string.IsNullOrEmpty(PeriodoSocFC), "PERIODO ES REQUERIDO!");
            base.AddRule(() => ObservacionesSocFC, () => !string.IsNullOrEmpty(ObservacionesSocFC), "OBSERVACIONES ES REQUERIDO!");
            OnPropertyChanged("PeriodoSocFC");
            OnPropertyChanged("ObservacionesSocFC");
        }
        #endregion
        #region Validaciones Opinion
        void ValidacionesOpinionArea()
        {
            base.ClearRules();
            base.AddRule(() => NombreAreaMedica, () => !string.IsNullOrEmpty(NombreAreaMedica), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => OpinionAreaMedica, () => !string.IsNullOrEmpty(OpinionAreaMedica), "OPINION REQUERIDA!");
            base.AddRule(() => IdAreaT, () => IdAreaT != -1, "AREA TECNICA REQUERIDA!");
            OnPropertyChanged("NombreAreaMedica");
            OnPropertyChanged("OpinionAreaMedica");
            OnPropertyChanged("IdAreaT");
        }
        #endregion
        #region Validaciones Grupo Familiar Trabajo Social Fuero Federal
        void ValidacionesGrupoFamFF()
        {
            base.ClearRules();
            base.AddRule(() => NombreIntegranteTSFF, () => !string.IsNullOrEmpty(NombreIntegranteTSFF), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => EdadIntegranteTSFF, () => !string.IsNullOrEmpty(EdadIntegranteTSFF), "EDAD ES REQUERIDO!");
            base.AddRule(() => ParentescoIntegranteTSFF, () => !string.IsNullOrEmpty(ParentescoIntegranteTSFF), "PARENTESCO ES REQUERIDO!");
            base.AddRule(() => EdoCivilIntegranteTSFF, () => !string.IsNullOrEmpty(EdoCivilIntegranteTSFF), "ESTADO CIVIL ES REQUERIDO!");
            base.AddRule(() => OcupacionIntegranteTSFF, () => !string.IsNullOrEmpty(OcupacionIntegranteTSFF), "OCUPACION ES REQUERIDO!");
            base.AddRule(() => IdEscIntegranteTSFF, () => IdEscIntegranteTSFF != -1, "ESCOLARIDAD ES REQUERIDO!");
            OnPropertyChanged("NombreIntegranteTSFF");
            OnPropertyChanged("EdadIntegranteTSFF");
            OnPropertyChanged("ParentescoIntegranteTSFF");
            OnPropertyChanged("EdoCivilIntegranteTSFF");
            OnPropertyChanged("OcupacionIntegranteTSFF");
            OnPropertyChanged("IdEscIntegranteTSFF");
        }
        #endregion
        #region Validaciones Dias Trabajados

        void ValidacionDiasLab()
        {
            base.ClearRules();
            base.AddRule(() => AnioDias, () => AnioDias != 0, "EL ANIO ES REQUERIDO!");
            base.AddRule(() => MesDias, () => MesDias != -1, "EL MES ES REQUERIDO!");
            OnPropertyChanged("AnioDias");
            OnPropertyChanged("MesDias");
        }

        #endregion
        #region Validaciones Educativas Fuero Federal
        void ValidacionesEducFF()
        {
            base.ClearRules();
            base.AddRule(() => IdTipoP, () => IdTipoP != -1, "PROGRAMA ES REQUERIDO!");
            base.AddRule(() => FecInicioProg, () => FecInicioProg.HasValue, "FECHA DE INICIO ES REQUERIDO!");
            base.AddRule(() => FecFinProg, () => FecFinProg.HasValue, "FECHA DE FIN ES REQUERIDO!");
            base.AddRule(() => Participo, () => !string.IsNullOrEmpty(Participo), "PARTICIPO ES REQUERIDO!");
            OnPropertyChanged("IdTipoP");
            OnPropertyChanged("FecInicioProg");
            OnPropertyChanged("FecFinProg");
            OnPropertyChanged("Participo");
        }

        void LimpiarValidacionesEducFF()
        {
            base.RemoveRule("IdTipoP");
            base.RemoveRule("FecInicioProg");
            base.RemoveRule("Participo");
            base.RemoveRule("FecFinProg");
            OnPropertyChanged("IdTipoP");
            OnPropertyChanged("FecInicioProg");
            OnPropertyChanged("FecFinProg");
            OnPropertyChanged("Participo");
        }

        #endregion
        #region Validacion Sanciones Informe Vigilancia Fuero Federal
        void ValidacionesSancionInformeFederal()
        {
            base.ClearRules();
            base.AddRule(() => ResolucionSancFF, () => !string.IsNullOrEmpty(ResolucionSancFF), "RESOLUCION ES REQUERIDO!");
            base.AddRule(() => MotivoSancFF, () => !string.IsNullOrEmpty(MotivoSancFF), "MOTIVO ES REQUERIDO!");
            base.AddRule(() => FecVigiSancFF, () => FecVigiSancFF.HasValue, "FECHA ES REQUERIDA!");
            OnPropertyChanged("ResolucionSancFF");
            OnPropertyChanged("MotivoSancFF");
            OnPropertyChanged("FecVigiSancFF");
        }

        void LimpiaValidacionesSancionesFederales() 
        {
            base.RemoveRule("ResolucionSancFF");
            base.RemoveRule("MotivoSancFF");
            base.RemoveRule("FecVigiSancFF");
            OnPropertyChanged("ResolucionSancFF");
            OnPropertyChanged("MotivoSancFF");
            OnPropertyChanged("FecVigiSancFF");
        }
        #endregion

        #region Validaciones Cursos Federales
        void ValidacionesCursosFederales() 
        {
            base.ClearRules();
            base.AddRule(() => DescripcionCursoFederal, () => !string.IsNullOrEmpty(DescripcionCursoFederal), "CURSO ES REQUERIDO!");
            base.AddRule(() => SelectedFechaInicioCursoFederal, () => SelectedFechaInicioCursoFederal.HasValue, "FECHA DE INICIO ES REQUERIDO!");
            base.AddRule(() => SelectedFechaFinCursoFederal, () => SelectedFechaFinCursoFederal.HasValue, "FECHA DE FIN ES REQUERIDO!");
            OnPropertyChanged("DescripcionCursoFederal");
            OnPropertyChanged("SelectedFechaInicioCursoFederal");
            OnPropertyChanged("SelectedFechaFinCursoFederal");
        }
        #endregion
    }
}