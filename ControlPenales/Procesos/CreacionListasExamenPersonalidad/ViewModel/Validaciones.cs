using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class CreacionListasExamenPViewModel
    {
        void ValidacionesFichaJuridica() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => ProcesosPendientes, () => !string.IsNullOrEmpty(ProcesosPendientes), "PROCESOS PENDIENTES ES REQUERIDO!");
                base.AddRule(() => CriminoDinamia, () => !string.IsNullOrEmpty(CriminoDinamia), "CRIMINODINAMIA ES REQUERIDO!");
                base.AddRule(() => ClasifJuridFicha, () => !string.IsNullOrEmpty(ClasifJuridFicha), "CLASIFICACIÓN JURÍDICA ES REQUERIDA!");
                base.AddRule(() => DelitoFicha, () => !string.IsNullOrEmpty(DelitoFicha), "DELITO ES REQUERIDO!");
                base.AddRule(() => ProcesosFicha, () => !string.IsNullOrEmpty(ProcesosFicha), "PROCESOS ES REQUERIDO!");
                base.AddRule(() => JuzgadoFicha, () => !string.IsNullOrEmpty(JuzgadoFicha), "JUZGADO ES REQUERIDO!");
                base.AddRule(() => SentenciaFicha, () => !string.IsNullOrEmpty(SentenciaFicha), "SENTENCIA ES REQUERIDO!");
                base.AddRule(() => APartirDeFicha, () => !string.IsNullOrEmpty(APartirDeFicha), "PARTIR DE ES REQUERIDO!");
                base.AddRule(() => CausoEjecFicha, () => !string.IsNullOrEmpty(CausoEjecFicha), "CAUSO EJECUTORIA ES REQUERIDO!");
                base.AddRule(() => PorcentPenaCompur, () => !string.IsNullOrEmpty(PorcentPenaCompur), "% PENA COMPURGADA ES REQUERIDA!");
                base.AddRule(() => ProcedenteDeFicha, () => !string.IsNullOrEmpty(ProcedenteDeFicha), "PROCEDENTE DE ES REQUERIDO!");
                base.AddRule(() => FecIngresoFicha, () => FecIngresoFicha.HasValue, "FECHA DE INGRESO ES REQUERIDO!");

                OnPropertyChanged("ProcesosPendientes");
                OnPropertyChanged("CriminoDinamia");
                OnPropertyChanged("ClasifJuridFicha");
                OnPropertyChanged("DelitoFicha");
                OnPropertyChanged("JuzgadoFicha");
                OnPropertyChanged("APartirDeFicha");
                OnPropertyChanged("PorcentPenaCompur");
                OnPropertyChanged("ProcedenteDeFicha");
                OnPropertyChanged("FecIngresoFicha");
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        void ValidacionesCreacionLista()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => NoOficio, () => !string.IsNullOrEmpty(NoOficio), "NÚMERO DE OFICIO ES REQUERIDO!");
                base.AddRule(() => IdMotivoE, () => IdMotivoE.HasValue ? IdMotivoE.Value != -1 : false, "MOTIVO DEL ESTUDIO ES REQUERIDO!");
                base.AddRule(() => NombrePrograma, () => !string.IsNullOrEmpty(NombrePrograma), "NOMBRE DEL PROGRAMA ES REQUERIDO!");

                OnPropertyChanged("NoOficio");
                OnPropertyChanged("IdMotivoE");
                OnPropertyChanged("NombrePrograma");
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        void ValidacionesTrasladoIslasPersonalidad() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => CriminogTrasladoIslas, () => !string.IsNullOrEmpty(CriminogTrasladoIslas), "CRIMINOGENESIS ES REQUERIDO!");
                base.AddRule(() => EstadoAdiccionTrasladoIslas, () => !string.IsNullOrEmpty(EstadoAdiccionTrasladoIslas), "ESTADO ADICCIÓN ES REQUERIDO!");
                base.AddRule(() => IntimidacionPenaTrasladoIslas, () => !string.IsNullOrEmpty(IntimidacionPenaTrasladoIslas), "INTIMIDACIÓN ANTE PENA ES REQUERIDA!");
                base.AddRule(() => DictamenTrasladoIslas, () => !string.IsNullOrEmpty(DictamenTrasladoIslas), "DICTAMEN DE TRASLADO ES REQUERIDO!");
                base.AddRule(() => ContinTratamTrasladoIslas, () => !string.IsNullOrEmpty(ContinTratamTrasladoIslas), "CONTINÚE CON TRATAMIENTO ES REQUERIDO!");
                base.AddRule(() => TieneAnuenciaTrasladoIslas, () => !string.IsNullOrEmpty(TieneAnuenciaTrasladoIslas), "ANUENCIA TRASLADO ES REQUERIDO!");
                base.AddRule(() => TratamSugTrasladoIslas, () => !string.IsNullOrEmpty(TratamSugTrasladoIslas), "TRATAMIENTO SUGERIDO ES REQUERIDO!");
                base.AddRule(() => FechaTrasladoIslas, () => FechaTrasladoIslas.HasValue, "FECHA ES REQUERIDO!");

                //-1
                base.AddRule(() => IdEgocentrismoTrasladoIslas, () => IdEgocentrismoTrasladoIslas.HasValue ? IdEgocentrismoTrasladoIslas.Value != -1 : false, "EGOCENTRISMO ES REQUERIDO!");
                base.AddRule(() => IdLabAfecTrasladoIslas, () => IdLabAfecTrasladoIslas.HasValue ? IdLabAfecTrasladoIslas.Value != -1 : false, "LABILIDAD AFECTIVA ES REQUERIDA!");
                base.AddRule(() => IdAgresivIntrTrasladoIslas, () => IdAgresivIntrTrasladoIslas.HasValue ? IdAgresivIntrTrasladoIslas.Value != -1 : false, "AGRESIVIDAD ES REQUERIDO!");
                base.AddRule(() => IdPeligroTrasladoIslas, () => IdPeligroTrasladoIslas.HasValue ? IdPeligroTrasladoIslas.Value != -1 : false, "PELIGROSIDAD ES REQUERIDO!");

                OnPropertyChanged("CriminogTrasladoIslas");
                OnPropertyChanged("EstadoAdiccionTrasladoIslas");
                OnPropertyChanged("IntimidacionPenaTrasladoIslas");
                OnPropertyChanged("DictamenTrasladoIslas");
                OnPropertyChanged("ContinTratamTrasladoIslas");
                OnPropertyChanged("TieneAnuenciaTrasladoIslas");
                OnPropertyChanged("TratamSugTrasladoIslas");
                OnPropertyChanged("FechaTrasladoIslas");
                OnPropertyChanged("IdEgocentrismoTrasladoIslas");
                OnPropertyChanged("IdLabAfecTrasladoIslas");
                OnPropertyChanged("IdAgresivIntrTrasladoIslas");
                OnPropertyChanged("IdPeligroTrasladoIslas");
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        void ValidacionesTrasladoInternacionalPersonalidad() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => EscolaridadTrasladoInternac, () => !string.IsNullOrEmpty(EscolaridadTrasladoInternac), "ESCOLARIDAD ES REQUERIDO!");
                base.AddRule(() => OcupacionPreviaTrasladoInternac, () => !string.IsNullOrEmpty(OcupacionPreviaTrasladoInternac), "OCUPACIÓN PREVIA ES REQUERIDA!");
                base.AddRule(() => VersionDelitoTrasladoInternacional, () => !string.IsNullOrEmpty(VersionDelitoTrasladoInternacional), "VERSIÓN DEL DELITO ES REQUERIDO!");
                base.AddRule(() => ClinicSanoTrasladoInternacional, () => !string.IsNullOrEmpty(ClinicSanoTrasladoInternacional), "CLÍNICAMENTE SANO ES REQUERIDO!");
                base.AddRule(() => TratamientoActualTrasladoInternacional, () => !string.IsNullOrEmpty(TratamientoActualTrasladoInternacional), "TRATAMIENTO ACTUAL ES REQUERIDO!");
                base.AddRule(() => CoeficIntTrasladoInternacional, () => !string.IsNullOrEmpty(CoeficIntTrasladoInternacional), "COEFICIENTE INTELECTUAL ES REQUERIDO!");
                base.AddRule(() => DanioCerebTrasladoInternacional, () => !string.IsNullOrEmpty(DanioCerebTrasladoInternacional), "DAÑO ÓRGANO CEREBRAL ES REQUERIDO!");
                base.AddRule(() => OtrosAspectosPersonalidadTrasladoInternacional, () => !string.IsNullOrEmpty(OtrosAspectosPersonalidadTrasladoInternacional), "ASPECTOS PERSONALIDAD ES REQUERIDO!");
                base.AddRule(() => ApoyoPadresTrasladoInternacional, () => !string.IsNullOrEmpty(ApoyoPadresTrasladoInternacional), "APOYO PADRES ES REQUERIDO!");
                base.AddRule(() => ConyugeTrasladoInternacional, () => !string.IsNullOrEmpty(ConyugeTrasladoInternacional), "APOYO CÓNYUGE ES REQUERIDO!");
                base.AddRule(() => FrecuenciaVisitaTrasladoInternacional, () => !string.IsNullOrEmpty(FrecuenciaVisitaTrasladoInternacional), "FRECUENCIA ES REQUERIDO!");
                base.AddRule(() => CartaArraigoCuentaTrasladoInternacional, () => !string.IsNullOrEmpty(CartaArraigoCuentaTrasladoInternacional), "CARTA DE ARRAIGO ES REQUERIDO!");
                base.AddRule(() => DomicilioTrasladoInternacional, () => !string.IsNullOrEmpty(DomicilioTrasladoInternacional), "DOMICILIO ES REQUERIDO!");
                base.AddRule(() => AnuenciaCupoTrasladoInternacional, () => !string.IsNullOrEmpty(AnuenciaCupoTrasladoInternacional), "ANUENCIA ES REQUERIDO!");
                base.AddRule(() => FechaAnuenciaTrasladoInternacional, () => FechaAnuenciaTrasladoInternacional.HasValue, "FECHA ES REQUERIDO!");
                base.AddRule(() => NivelSocioETrasladoInternacional, () => !string.IsNullOrEmpty(NivelSocioETrasladoInternacional), "NIVEL SOCIOECONÓMICO ES REQUERIDO!");
                base.AddRule(() => EstudiaActualTrasladoInternacional, () => !string.IsNullOrEmpty(EstudiaActualTrasladoInternacional), "ESTUDIA ACTUALMENTE ES REQUERIDO!");
                base.AddRule(() => OtrosCursosCapacRecibTrasladoInternacional, () => !string.IsNullOrEmpty(OtrosCursosCapacRecibTrasladoInternacional), "OTROS CURSOS ES REQUERIDO!");
                base.AddRule(() => AcualmenteTrabajaInstTrasladoInternacional, () => !string.IsNullOrEmpty(AcualmenteTrabajaInstTrasladoInternacional), "ACTUALMENTE TRABAJA ES REQUERIDO!");
                base.AddRule(() => TotalDiasEfectivTrasladoInternacional, () => TotalDiasEfectivTrasladoInternacional.HasValue, "TOTAL DE DÍAS ES REQUERIDO!");
                base.AddRule(() => DescAgresividad, () => !string.IsNullOrEmpty(DescAgresividad), "AGRESIVIDAD ES REQUERIDO!");
                base.AddRule(() => ConductaConsidTrasladoInternacional, () => ConductaConsidTrasladoInternacional.HasValue ? ConductaConsidTrasladoInternacional != -1 : false, "CONDUCTA ES REQUERIDO!");
                base.AddRule(() => IdPeligroTrasladoInternacional, () => IdPeligroTrasladoInternacional.HasValue ? IdPeligroTrasladoInternacional.Value != -1 : false, "PELIGROSIDAD ES REQUERIDO!");
                base.AddRule(() => AdiccionToxicosTrasladoInternacional, () => !string.IsNullOrEmpty(AdiccionToxicosTrasladoInternacional), "ADICCIÓN A TÓXICOS ES REQUERIDA!");
                base.AddRule(() => EspecifiqueOtroTratamientoTrasladoInterNacional, () => !string.IsNullOrEmpty(EspecifiqueOtroTratamientoTrasladoInterNacional), "ACTUALMENTE TRABAJA ES REQUERIDO!");
                base.AddRule(() => OtrosAspectosTrasladoInternacional, () => !string.IsNullOrEmpty(OtrosAspectosTrasladoInternacional), "OTROS ASPECTOS ES REQUERIDO!");

                OnPropertyChanged("EscolaridadTrasladoInternac");
                OnPropertyChanged("OcupacionPreviaTrasladoInternac");
                OnPropertyChanged("VersionDelitoTrasladoInternacional");
                OnPropertyChanged("ClinicSanoTrasladoInternacional");
                OnPropertyChanged("TratamientoActualTrasladoInternacional");
                OnPropertyChanged("CoeficIntTrasladoInternacional");
                OnPropertyChanged("DanioCerebTrasladoInternacional");
                OnPropertyChanged("OtrosAspectosPersonalidadTrasladoInternacional");
                OnPropertyChanged("ApoyoPadresTrasladoInternacional");
                OnPropertyChanged("ConyugeTrasladoInternacional");
                OnPropertyChanged("FrecuenciaVisitaTrasladoInternacional");
                OnPropertyChanged("CartaArraigoCuentaTrasladoInternacional");
                OnPropertyChanged("DomicilioTrasladoInternacional");
                OnPropertyChanged("AnuenciaCupoTrasladoInternacional");
                OnPropertyChanged("FechaAnuenciaTrasladoInternacional");
                OnPropertyChanged("NivelSocioETrasladoInternacional");
                OnPropertyChanged("EstudiaActualTrasladoInternacional");
                OnPropertyChanged("OtrosCursosCapacRecibTrasladoInternacional");
                OnPropertyChanged("AcualmenteTrabajaInstTrasladoInternacional");
                OnPropertyChanged("TotalDiasEfectivTrasladoInternacional");
                OnPropertyChanged("DescAgresividad");
                OnPropertyChanged("ConductaConsidTrasladoInternacional");
                OnPropertyChanged("IdPeligroTrasladoInternacional");
                OnPropertyChanged("AdiccionToxicosTrasladoInternacional");
                OnPropertyChanged("EspecifiqueOtroTratamientoTrasladoInterNacional");
                OnPropertyChanged("OtrosAspectosTrasladoInternacional");
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        void ValidacionesActaConsejoTecnicoComun() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => LugarActaComun, () => !string.IsNullOrEmpty(LugarActaComun), "LUGAR ES REQUERIDO!");
                base.AddRule(() => NombrePresidenteActaComun, () => !string.IsNullOrEmpty(NombrePresidenteActaComun), "NOMBRE DE PRESIDENTE ES REQUERIDO!");
                base.AddRule(() => NombreSecretarioActaComun, () => !string.IsNullOrEmpty(NombreSecretarioActaComun), "NOMBRE DE SECRETARIO ES REQUERIDO!");
                base.AddRule(() => NombreJuridicoActaComun, () => !string.IsNullOrEmpty(NombreJuridicoActaComun), "NOMBRE DE JURÍDICO ES REQUERIDO!");
                base.AddRule(() => NombreMedicoActaComun, () => !string.IsNullOrEmpty(NombreMedicoActaComun), "NOMBRE DE MEDICO ES REQUERIDO!");
                base.AddRule(() => NombrePsiccoActaComun, () => !string.IsNullOrEmpty(NombrePsiccoActaComun), "NOMBRE DE PSICÓLOGO ES REQUERIDO!");
                base.AddRule(() => NombreCriminologiaActaComun, () => !string.IsNullOrEmpty(NombreCriminologiaActaComun), "NOMBRE DE CRIMINOLOGÍA ES REQUERIDO!");
                base.AddRule(() => NombreTrabajoSocialActaComun, () => !string.IsNullOrEmpty(NombreTrabajoSocialActaComun), "NOMBRE DE TRABAJO SOCIAL ES REQUERIDO!");
                base.AddRule(() => NombreEducativoActaComun, () => !string.IsNullOrEmpty(NombreEducativoActaComun), "NOMBRE DE EDUCATIVO ES REQUERIDO!");
                base.AddRule(() => NombreAreaLaboralActaComun, () => !string.IsNullOrEmpty(NombreAreaLaboralActaComun), "NOMBRE DE ÁREA LABORAL ES REQUERIDO!");
                base.AddRule(() => NombreSeguridadActaComun, () => !string.IsNullOrEmpty(NombreSeguridadActaComun), "NOMBRE DE SEGURIDAD ES REQUERIDO!");
                base.AddRule(() => OpinionMedico, () => !string.IsNullOrEmpty(OpinionMedico), "OPINIÓN MÉDICA ES REQUERIDO!");
                base.AddRule(() => OpinionPsico, () => !string.IsNullOrEmpty(OpinionPsico), "OPINIÓN PSICOLÓGICA  ES REQUERIDO!");
                base.AddRule(() => OpinionCrimi, () => !string.IsNullOrEmpty(OpinionCrimi), "OPINIÓN CRIMINOLOGICA ES REQUERIDO!");
                base.AddRule(() => OpinionTrabSocial, () => !string.IsNullOrEmpty(OpinionTrabSocial), "OPINIÓN DE TRABAJO SOCIAL ES REQUERIDO!");
                base.AddRule(() => OpinionEscolar, () => !string.IsNullOrEmpty(OpinionEscolar), "OPINIÓN ESCOLAR ES REQUERIDO!");
                base.AddRule(() => OpinionLaboral, () => !string.IsNullOrEmpty(OpinionLaboral), "OPINIÓN LABORAL ES REQUERIDO!");
                base.AddRule(() => OpinionSeguridad, () => !string.IsNullOrEmpty(OpinionSeguridad), "OPINIÓN DE SEGURIDAD ES REQUERIDO!");
                base.AddRule(() => NombreInternoActaComun, () => !string.IsNullOrEmpty(NombreInternoActaComun), "NOMBRE DEL INTERNO ES REQUERIDO!");
                base.AddRule(() => ManifestaronActaComun, () => !string.IsNullOrEmpty(ManifestaronActaComun), "MANIFIESTO ES REQUERIDO!");
                base.AddRule(() => ActuacionActaComun, () => !string.IsNullOrEmpty(ActuacionActaComun), "ACTUACION ES REQUERIDO!");
                base.AddRule(() => AcuerdoActaComun, () => !string.IsNullOrEmpty(AcuerdoActaComun), "ACUERDO ES REQUERIDO!");
                base.AddRule(() => OpinionActaComun, () => !string.IsNullOrEmpty(OpinionActaComun), "OPINIÓN ES REQUERIDO!");

                OnPropertyChanged("LugarActaComun");
                OnPropertyChanged("NombrePresidenteActaComun");
                OnPropertyChanged("NombreSecretarioActaComun");
                OnPropertyChanged("NombreJuridicoActaComun");
                OnPropertyChanged("NombreMedicoActaComun");
                OnPropertyChanged("NombrePsiccoActaComun");
                OnPropertyChanged("NombreCriminologiaActaComun");
                OnPropertyChanged("NombreTrabajoSocialActaComun");
                OnPropertyChanged("NombreEducativoActaComun");
                OnPropertyChanged("NombreAreaLaboralActaComun");
                OnPropertyChanged("NombreSeguridadActaComun");
                OnPropertyChanged("OpinionMedico");
                OnPropertyChanged("OpinionPsico");
                OnPropertyChanged("OpinionCrimi");
                OnPropertyChanged("OpinionTrabSocial");
                OnPropertyChanged("OpinionEscolar");
                OnPropertyChanged("OpinionLaboral");
                OnPropertyChanged("OpinionSeguridad");
                OnPropertyChanged("NombreInternoActaComun");
                OnPropertyChanged("ManifestaronActaComun");
                OnPropertyChanged("ActuacionActaComun");
                OnPropertyChanged("AcuerdoActaComun");
                OnPropertyChanged("OpinionActaComun");
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        void ValidacionesTrasladoIslas() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => IdPeligrosidadTrasladoNacional, () => IdPeligrosidadTrasladoNacional.HasValue ? IdPeligrosidadTrasladoNacional.Value != -1 : false, "PELIGROSIDAD ES REQUERIDO!");
                base.AddRule(() => AdicToxTrasladoNacional, () => !string.IsNullOrEmpty(AdicToxTrasladoNacional), "ADICCION A TOXICOS ES REQUERIDO!");
                base.AddRule(() => EspecifiqueAspectosRelevantesTrasladoNacional, () => !string.IsNullOrEmpty(EspecifiqueAspectosRelevantesTrasladoNacional), "ASPECTOS RELEVANTES ES REQUERIDO!");
                base.AddRule(() => FechaTrasladoNacional, () => FechaTrasladoNacional.HasValue, "FECHA ES REQUERIDO!");

                OnPropertyChanged("IdPeligrosidadTrasladoNacional");
                OnPropertyChanged("AdicToxTrasladoNacional");
                OnPropertyChanged("EspecifiqueAspectosRelevantesTrasladoNacional");
                OnPropertyChanged("FechaTrasladoNacional");
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
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

        void ValidacionesActaFederal() 
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => DirectorCentro, () => !string.IsNullOrEmpty(DirectorCentro), "DIRECTOR DEL CENTRO ES REQUERIDO!");
                base.AddRule(() => ActuacionR, () => !string.IsNullOrEmpty(ActuacionR), "ACTUACION ES REQUERIDO!");
                base.AddRule(() => VotosR, () => !string.IsNullOrEmpty(VotosR), "VOTOS ES REQUERIDO!");
                base.AddRule(() => TramiteDescripcion, () => !string.IsNullOrEmpty(TramiteDescripcion), "TRAMITE ES REQUERIDO!");
                base.AddRule(() => LugarActa, () => !string.IsNullOrEmpty(LugarActa), "LUGAR ES REQUERIDO!");
                base.AddRule(() => FechaActa, () => FechaActa.HasValue, "FECHA ES REQUERIDO!");

                OnPropertyChanged("DirectorCentro");
                OnPropertyChanged("ActuacionR");
                OnPropertyChanged("VotosR");
                OnPropertyChanged("TramiteDescripcion");
                OnPropertyChanged("LugarActa");
                OnPropertyChanged("FechaActa");
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

    }
}