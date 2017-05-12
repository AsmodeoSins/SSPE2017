using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class NotaEvolucionViewModel
    {
        private void LimpiarValidaciones()
        {
            base.ClearRules();
            OnPropertyChanged("TensionArterial1");
            OnPropertyChanged("TensionArterial2");
            OnPropertyChanged("TextFrecuenciaCardiaca");
            OnPropertyChanged("Temperatura");
            OnPropertyChanged("Peso");
            OnPropertyChanged("Talla");
            OnPropertyChanged("FrecuenciaRespira");
            //OnPropertyChanged("ObservacionesSignosVitales");
            OnPropertyChanged("SelectedValueTipoAtencion");
            OnPropertyChanged("SelectedPronostico");
            OnPropertyChanged("ExploracionFisica");
            OnPropertyChanged("ResumenInterr");
            OnPropertyChanged("TextUrgenciaDestino");
            OnPropertyChanged("TextUrgenciaEstadoMental");
            OnPropertyChanged("TextUrgenciaMotivo");
            OnPropertyChanged("TextUrgenciaProcedimiento");
            OnPropertyChanged("TextEvolucion");
            OnPropertyChanged("TextExistencia");
            OnPropertyChanged("TextActualizacion");
            OnPropertyChanged("TextInterconsultaCriterio");
            OnPropertyChanged("TextInterconsultaMotivo");
            OnPropertyChanged("TextInterconsultaSugerencias");
            OnPropertyChanged("TextInterconsultaTratamiento");
            OnPropertyChanged("TextTrasladoEnvia");
            OnPropertyChanged("TextTrasladoMotivo");
            OnPropertyChanged("TextTrasladoReceptor");
            OnPropertyChanged("TextPreOpCuidados");
            OnPropertyChanged("TextPreOpDiagnostico");
            OnPropertyChanged("TextPreOpFecha");
            OnPropertyChanged("TextPreOpPlan");
            OnPropertyChanged("TextPreOpPlanTerapeutico");
            OnPropertyChanged("TextPreOpRiesgo");
            OnPropertyChanged("TextPreAnestEvolucion");
            OnPropertyChanged("TextPreAnestRiesgo");
            OnPropertyChanged("TextPreAnestTipo");
            OnPropertyChanged("TextPostOpAccidentes");
            OnPropertyChanged("TextPostOpDiagnosticoPostOp");
            OnPropertyChanged("TextPostOpEstadoInmediato");
            OnPropertyChanged("TextPostOpGasasCompresas");
            OnPropertyChanged("TextPreAnestRiesgo");
            OnPropertyChanged("TextPostOpIncidentes");
            OnPropertyChanged("TextPostOpInterpretacion");
            OnPropertyChanged("TextPostOpPiezasBiopsias");
            OnPropertyChanged("TextPostOpPlaneada");
            OnPropertyChanged("TextPostOpRealizada");
            OnPropertyChanged("TextPostOpSangrado");
            OnPropertyChanged("TextPostOpTecnicaQuirurgica");
            OnPropertyChanged("TextPostOpTratamientoInmediato");
        }

        private void ValidacionSolicitud()
        {
            base.ClearRules();
            base.AddRule(() => AFechaValid, () => AFechaValid, "FECHA ES REQUERIDA");
            base.AddRule(() => AHoraI, () => AHoraI.HasValue, "HORA INICIO ES REQUERIDA");
            base.AddRule(() => AHoraF, () => AHoraF.HasValue, "HORA FIN REQUERIDA");
            base.AddRule(() => AHorasValid, () => AHorasValid, "LA HORA DE INICIO DEBE SER MAYOR A LA HORA FIN");

            OnPropertyChanged("AFechaValid");
            OnPropertyChanged("AHoraI");
            OnPropertyChanged("AHoraF");
            OnPropertyChanged("AHorasValid");
        }

        private void ValidacionesMedico()
        {
            base.ClearRules();
            //if (SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS : false) return;
            base.AddRule(() => TextTensionArterial1, () => !string.IsNullOrEmpty(TextTensionArterial1), "TENSIÓN ARTERIAL ES REQUERIDO!");
            base.AddRule(() => TextTensionArterial2, () => !string.IsNullOrEmpty(TextTensionArterial2), "TENSIÓN ARTERIAL ES REQUERIDO!");
            base.AddRule(() => TextFrecuenciaCardiaca, () => !string.IsNullOrEmpty(TextFrecuenciaCardiaca), "FRECUENCIA CARDIACA ES REQUERIDO!");
            base.AddRule(() => TextTemperatura, () => !string.IsNullOrEmpty(TextTemperatura), "TEMPERATURA ES REQUERIDO!");
            base.AddRule(() => TextPeso, () => !string.IsNullOrEmpty(TextPeso), "PESO ES REQUERIDO!");
            base.AddRule(() => TextTalla, () => !string.IsNullOrEmpty(TextTalla), "TALLA ES REQUERIDO!");
            base.AddRule(() => TextGlucemia, () => !string.IsNullOrEmpty(TextGlucemia), "GLUCEMIA ES REQUERIDO!");
            base.AddRule(() => TextFrecuenciaRespira, () => !string.IsNullOrEmpty(TextFrecuenciaRespira), "FRECUENCIA RESPIRATORIA ES REQUERIDO!");
            base.AddRule(() => SelectPronostico, () => SelectPronostico.HasValue ? SelectPronostico != -1 : false, "PRONOSTICO ES REQUERIDO!");
            //if (SelectTipoServicio != null ?
            //    SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
            //        SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_INTEGRIDAD_FISICA || SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO
            //    : SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ?
            //        SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTISTA_INTEGRIDAD_FISICA || SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO
            //    : false
            //: false)
            //{
            base.AddRule(() => TextExploracionFisica, () => !string.IsNullOrEmpty(TextExploracionFisica), "EXPLORACIÓN FÍSICA ES REQUERIDO!");
            //}
            //base.AddRule(() => ResumenInterr, () => !string.IsNullOrEmpty(ResumenInterr), "RESUMEN DE INTERROGATORIO ES REQUERIDO!");
            //base.AddRule(() => ObservacionesSignosVitales, () => !string.IsNullOrEmpty(ObservacionesSignosVitales), "OBSERVACIONES DE SIGNOS VITALES ES REQUERIDO!");
            //base.AddRule(() => SelectTipoAtencion, () => SelectTipoAtencion != null ? SelectTipoAtencion.ID_TIPO_ATENCION != -1 : false, "TIPO DE ATENCION ES REQUERIDO!");
            //base.AddRule(() => SelectTipoServicio, () => SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO != -1 : false, "TIPO DE SERVICIO ES REQUERIDO!");
            OnPropertyChanged("TextTensionArterial1");
            OnPropertyChanged("TextTensionArterial2");
            OnPropertyChanged("TextFrecuenciaCardiaca");
            OnPropertyChanged("TextTemperatura");
            OnPropertyChanged("TextPeso");
            OnPropertyChanged("TextTalla");
            OnPropertyChanged("TextGlucemia");
            OnPropertyChanged("TextFrecuenciaRespira");
            OnPropertyChanged("TextExploracionFisica");
            //OnPropertyChanged("ObservacionesSignosVitales");
            //OnPropertyChanged("SelectTipoAtencion");
            //OnPropertyChanged("SelectTipoServicio");
            OnPropertyChanged("SelectPronostico");
            //OnPropertyChanged("ResumenInterr");
        }

        private void ValidacionesEvolucion()
        {
            base.AddRule(() => TextEvolucion, () => !string.IsNullOrEmpty(TextEvolucion), "EVOLUCION ES REQUERIDO!");
            base.AddRule(() => TextExistencia, () => !string.IsNullOrEmpty(TextExistencia), "EXISTENCIA ES REQUERIDO!");
            base.AddRule(() => TextActualizacion, () => !string.IsNullOrEmpty(TextActualizacion), "ACTUALIZACION ES REQUERIDO!");
            OnPropertyChanged("TextEvolucion");
            OnPropertyChanged("TextExistencia");
            OnPropertyChanged("TextActualizacion");
        }

        private void ValidacionesPreOp()
        {
            base.AddRule(() => TextPreOpCuidados, () => !string.IsNullOrEmpty(TextPreOpCuidados), "CUIDADOS ES REQUERIDO!");
            base.AddRule(() => TextPreOpDiagnostico, () => !string.IsNullOrEmpty(TextPreOpDiagnostico), "DIAGNOSTICO ES REQUERIDO!");
            base.AddRule(() => TextPreOpFecha, () => TextPreOpFecha.HasValue, "FECHA DE LA CIRUGIA ES REQUERIDA!");
            base.AddRule(() => TextPreOpPlan, () => !string.IsNullOrEmpty(TextPreOpPlan), "PLAN QUIRURGICO ES REQUERIDO!");
            base.AddRule(() => TextPreOpPlanTerapeutico, () => !string.IsNullOrEmpty(TextPreOpPlanTerapeutico), "PLAN TERAPEUTICO ES REQUERIDO!");
            base.AddRule(() => TextPreOpRiesgo, () => !string.IsNullOrEmpty(TextPreOpRiesgo), "RIESGO ES REQUERIDO!");
            OnPropertyChanged("TextPreOpCuidados");
            OnPropertyChanged("TextPreOpDiagnostico");
            OnPropertyChanged("TextPreOpFecha");
            OnPropertyChanged("TextPreOpPlan");
            OnPropertyChanged("TextPreOpPlanTerapeutico");
            OnPropertyChanged("TextPreOpRiesgo");
        }

        private void ValidacionesPreAn()
        {
            base.AddRule(() => TextPreAnestEvolucion, () => !string.IsNullOrEmpty(TextPreAnestEvolucion), "EVOLUCION ES REQUERIDO!");
            base.AddRule(() => TextPreAnestRiesgo, () => !string.IsNullOrEmpty(TextPreAnestRiesgo), "RIESGO ES REQUERIDO!");
            base.AddRule(() => TextPreAnestTipo, () => !string.IsNullOrEmpty(TextPreAnestTipo), "TIPO DE ANESTESIA ES REQUERIDA!");
            OnPropertyChanged("TextPreAnestEvolucion");
            OnPropertyChanged("TextPreAnestRiesgo");
            OnPropertyChanged("TextPreAnestTipo");
        }

        private void ValidacionesPostOp()
        {
            base.AddRule(() => TextPostOpAccidentes, () => !string.IsNullOrEmpty(TextPostOpAccidentes), "ACCIDENTES SON REQUERIDOS!");
            base.AddRule(() => TextPostOpDiagnosticoPostOp, () => !string.IsNullOrEmpty(TextPostOpDiagnosticoPostOp), "DIAGNOSTICO ES REQUERIDO!");
            base.AddRule(() => TextPostOpEstadoInmediato, () => !string.IsNullOrEmpty(TextPostOpEstadoInmediato), "ESTADO POST-QUIRURGICO INMEDIATO ES REQUERIDA!");
            base.AddRule(() => TextPostOpGasasCompresas, () => !string.IsNullOrEmpty(TextPostOpGasasCompresas), "REPORTE DE GASAS ES REQUERIDO!");
            base.AddRule(() => TextPreAnestRiesgo, () => !string.IsNullOrEmpty(TextPostOpHallazgosTransoperatorios), "RIESGO ES REQUERIDO!");
            base.AddRule(() => TextPostOpIncidentes, () => !string.IsNullOrEmpty(TextPostOpIncidentes), "INCIDENTES SON REQUERIDOS!");
            base.AddRule(() => TextPostOpInterpretacion, () => !string.IsNullOrEmpty(TextPostOpInterpretacion), "INTERPRETACION ES REQUERIDO!");
            base.AddRule(() => TextPostOpPiezasBiopsias, () => !string.IsNullOrEmpty(TextPostOpPiezasBiopsias), "PIEZAS Y BIOPSIAS SON REQUERIDOAS!");
            base.AddRule(() => TextPostOpPlaneada, () => !string.IsNullOrEmpty(TextPostOpPlaneada), "OPERACION PLANEADA ES REQUERIDA!");
            base.AddRule(() => TextPostOpRealizada, () => !string.IsNullOrEmpty(TextPostOpRealizada), "OPERACION REALIZADA ES REQUERIDA!");
            base.AddRule(() => TextPostOpSangrado, () => !string.IsNullOrEmpty(TextPostOpSangrado), "SANGRADO ES REQUERIDO!");
            base.AddRule(() => TextPostOpTecnicaQuirurgica, () => !string.IsNullOrEmpty(TextPostOpTecnicaQuirurgica), "TECNICA QUIRURGICA ES REQUERIDA!");
            base.AddRule(() => TextPostOpTratamientoInmediato, () => !string.IsNullOrEmpty(TextPostOpTratamientoInmediato), "TRATAMIENTO INMEDIATO ES REQUERIDO!");
            OnPropertyChanged("TextPostOpAccidentes");
            OnPropertyChanged("TextPostOpDiagnosticoPostOp");
            OnPropertyChanged("TextPostOpEstadoInmediato");
            OnPropertyChanged("TextPostOpGasasCompresas");
            OnPropertyChanged("TextPreAnestRiesgo");
            OnPropertyChanged("TextPostOpIncidentes");
            OnPropertyChanged("TextPostOpInterpretacion");
            OnPropertyChanged("TextPostOpPiezasBiopsias");
            OnPropertyChanged("TextPostOpPlaneada");
            OnPropertyChanged("TextPostOpRealizada");
            OnPropertyChanged("TextPostOpSangrado");
            OnPropertyChanged("TextPostOpTecnicaQuirurgica");
            OnPropertyChanged("TextPostOpTratamientoInmediato");
        }

    }
}
