using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class NotaMedicaViewModel
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
            OnPropertyChanged("Glucemia");
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

        private void ValidacionesEnfermero()
        {
            base.ClearRules();
            if (SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS : false) return;
            var tension = new short();
            base.AddRule(() => TensionArterial1, () => !string.IsNullOrEmpty(TensionArterial1) && short.TryParse(TensionArterial1, out tension), "TENSIÓN ARTERIAL ES REQUERIDO!");
            base.AddRule(() => TensionArterial2, () => !string.IsNullOrEmpty(TensionArterial2) && short.TryParse(TensionArterial2, out tension), "TENSIÓN ARTERIAL ES REQUERIDO!");
            base.AddRule(() => TextFrecuenciaCardiaca, () => !string.IsNullOrEmpty(TextFrecuenciaCardiaca), "FRECUENCIA CARDIACA ES REQUERIDO!");
            base.AddRule(() => Temperatura, () => !string.IsNullOrEmpty(Temperatura), "TEMPERATURA ES REQUERIDO!");
            base.AddRule(() => Peso, () => !string.IsNullOrEmpty(Peso), "PESO ES REQUERIDO!");
            base.AddRule(() => Talla, () => !string.IsNullOrEmpty(Talla), (SelectTipoServicio != null ?
                SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                    SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.HISTORIA_CLINICA_MEDICA ? 
                        "ESTATURA"
                    : "TALLA"
                : SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ?
                    SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO ?
                        "ESTATURA"
                    : "TALLA"
                : "TALLA"
            : "TALLA") + " ES REQUERIDO!");
            //base.AddRule(() => Glucemia, () => !string.IsNullOrEmpty(Glucemia), "GLUCEMIA ES REQUERIDO!");
            base.AddRule(() => FrecuenciaRespira, () => !string.IsNullOrEmpty(FrecuenciaRespira), "FRECUENCIA RESPIRATORIA ES REQUERIDO!");
            base.AddRule(() => SelectTipoAtencion, () => SelectTipoAtencion != null ? SelectTipoAtencion.ID_TIPO_ATENCION != -1 : true, "TIPO DE ATENCION ES REQUERIDO!");
            base.AddRule(() => SelectTipoServicio, () => SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO != -1 : true, "TIPO DE SERVICIO ES REQUERIDO!");

            OnPropertyChanged("TensionArterial1");
            OnPropertyChanged("TensionArterial2");
            OnPropertyChanged("TextFrecuenciaCardiaca");
            OnPropertyChanged("Temperatura");
            OnPropertyChanged("Peso");
            OnPropertyChanged("Talla");
            //OnPropertyChanged("Glucemia");
            OnPropertyChanged("FrecuenciaRespira");
            OnPropertyChanged("SelectTipoAtencion");
            OnPropertyChanged("SelectTipoServicio");
        }

        private void ValidacionesDentista()
        {
            base.ClearRules();
            //base.AddRule(() => TensionArterial1, () => !string.IsNullOrEmpty(TensionArterial1), "TENSIÓN ARTERIAL ES REQUERIDO!");
            //OnPropertyChanged("TensionArterial1");
        }

        private void ValidacionesMedico()
        {
            base.ClearRules();
            if (SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS : false) return;
            var tension = new short();
            base.AddRule(() => TensionArterial1, () => !string.IsNullOrEmpty(TensionArterial1) && short.TryParse(TensionArterial1, out tension), "TENSIÓN ARTERIAL ES REQUERIDO!");
            base.AddRule(() => TensionArterial2, () => !string.IsNullOrEmpty(TensionArterial2) && short.TryParse(TensionArterial2, out tension), "TENSIÓN ARTERIAL ES REQUERIDO!");
            base.AddRule(() => TextFrecuenciaCardiaca, () => !string.IsNullOrEmpty(TextFrecuenciaCardiaca), "FRECUENCIA CARDIACA ES REQUERIDO!");
            base.AddRule(() => Temperatura, () => !string.IsNullOrEmpty(Temperatura), "TEMPERATURA ES REQUERIDO!");
            base.AddRule(() => Peso, () => !string.IsNullOrEmpty(Peso), "PESO ES REQUERIDO!");
            base.AddRule(() => Talla, () => !string.IsNullOrEmpty(Talla), "TALLA ES REQUERIDO!");
            base.AddRule(() => Glucemia, () => !string.IsNullOrEmpty(Glucemia), "GLUCEMIA ES REQUERIDO!");
            base.AddRule(() => FrecuenciaRespira, () => !string.IsNullOrEmpty(FrecuenciaRespira), "FRECUENCIA RESPIRATORIA ES REQUERIDO!");
            base.AddRule(() => SelectedPronostico, () => SelectedPronostico.HasValue ? SelectedPronostico != -1 : false, "PRONOSTICO ES REQUERIDO!");
            if (SelectTipoServicio != null ?
                SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA ?
                    SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_INTEGRIDAD_FISICA || SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO
                : SelectTipoServicio.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ?
                    SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTISTA_INTEGRIDAD_FISICA || SelectTipoServicio.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO
                : false
            : false)
            {
                base.AddRule(() => ExploracionFisica, () => !string.IsNullOrEmpty(ExploracionFisica), "EXPLORACIÓN FÍSICA ES REQUERIDO!");
            }
            //base.AddRule(() => ResumenInterr, () => !string.IsNullOrEmpty(ResumenInterr), "RESUMEN DE INTERROGATORIO ES REQUERIDO!");
            //base.AddRule(() => ObservacionesSignosVitales, () => !string.IsNullOrEmpty(ObservacionesSignosVitales), "OBSERVACIONES DE SIGNOS VITALES ES REQUERIDO!");
            base.AddRule(() => SelectTipoAtencion, () => SelectTipoAtencion != null ? SelectTipoAtencion.ID_TIPO_ATENCION != -1 : false, "TIPO DE ATENCION ES REQUERIDO!");
            base.AddRule(() => SelectTipoServicio, () => SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO != -1 : false, "TIPO DE SERVICIO ES REQUERIDO!");
            OnPropertyChanged("TensionArterial1");
            OnPropertyChanged("TensionArterial2");
            OnPropertyChanged("TextFrecuenciaCardiaca");
            OnPropertyChanged("Temperatura");
            OnPropertyChanged("Peso");
            OnPropertyChanged("Talla");
            OnPropertyChanged("Glucemia");
            OnPropertyChanged("FrecuenciaRespira");
            //OnPropertyChanged("ObservacionesSignosVitales");
            OnPropertyChanged("SelectTipoAtencion");
            OnPropertyChanged("SelectTipoServicio");
            OnPropertyChanged("SelectedPronostico");
            OnPropertyChanged("ExploracionFisica");
            OnPropertyChanged("ResumenInterr");
        }

        private void ValidacionesAmbosPerfiles()
        {
            base.ClearRules();
            var tension = new short();
            base.AddRule(() => TensionArterial1, () => !string.IsNullOrEmpty(TensionArterial1) && short.TryParse(TensionArterial1, out tension), "TENSIÓN ARTERIAL ES REQUERIDO!");
            base.AddRule(() => TensionArterial2, () => !string.IsNullOrEmpty(TensionArterial2) && short.TryParse(TensionArterial2, out tension), "TENSIÓN ARTERIAL ES REQUERIDO!");
            base.AddRule(() => TextFrecuenciaCardiaca, () => !string.IsNullOrEmpty(TextFrecuenciaCardiaca), "FRECUENCIA CARDIACA ES REQUERIDO!");
            base.AddRule(() => Temperatura, () => !string.IsNullOrEmpty(Temperatura), "TEMPERATURA ES REQUERIDO!");
            base.AddRule(() => Peso, () => !string.IsNullOrEmpty(Peso), "PESO ES REQUERIDO!");
            base.AddRule(() => Talla, () => !string.IsNullOrEmpty(Talla), "TALLA ES REQUERIDO!");
            base.AddRule(() => FrecuenciaRespira, () => !string.IsNullOrEmpty(FrecuenciaRespira), "FRECUENCIA RESPIRATORIA ES REQUERIDO!");
            //base.AddRule(() => ObservacionesSignosVitales, () => !string.IsNullOrEmpty(ObservacionesSignosVitales), "OBSERVACIONES DE SIGNOS VITALES ES REQUERIDO!");
            base.AddRule(() => SelectedPronostico, () => SelectedPronostico.HasValue ? SelectedPronostico != -1 : false, "PRONOSTICO ES REQUERIDO!");
            //base.AddRule(() => ResultadoServTrat, () => !string.IsNullOrEmpty(ResultadoServTrat), "RESULTADO DE SERVICIOS DE TRATAMIENTO ES REQUERIDO!");
            //base.AddRule(() => Diagnostico, () => !string.IsNullOrEmpty(Diagnostico), "DIAGNÓSTICO ES REQUERIDO!");
            //base.AddRule(() => PlanEstudioTrat, () => !string.IsNullOrEmpty(PlanEstudioTrat), "PLAN DE ESTUDIO DE TRATAMIENTO ES REQUERIDO!");
            base.AddRule(() => ExploracionFisica, () => !string.IsNullOrEmpty(ExploracionFisica), "EXPLORACIÓN FÍSICA ES REQUERIDO!");
            //base.AddRule(() => ResumenInterr, () => !string.IsNullOrEmpty(ResumenInterr), "RESUMEN DE INTERROGATORIO ES REQUERIDO!");
            //base.AddRule(() => ResultadoServAux, () => !string.IsNullOrEmpty(ResultadoServAux), "RESULTADO DE SERVICIOS AUXILIARES ES REQUERIDO!");

            OnPropertyChanged("SelectedPronostico");
            //OnPropertyChanged("ResultadoServTrat");
            //OnPropertyChanged("Diagnostico");
            //OnPropertyChanged("PlanEstudioTrat");
            OnPropertyChanged("ExploracionFisica");
            OnPropertyChanged("ResumenInterr");
            //OnPropertyChanged("ResultadoServAux");
            OnPropertyChanged("TensionArterial1");
            OnPropertyChanged("TensionArterial2");
            OnPropertyChanged("TextFrecuenciaCardiaca");
            OnPropertyChanged("Temperatura");
            OnPropertyChanged("Peso");
            OnPropertyChanged("Talla");
            OnPropertyChanged("FrecuenciaRespira");
            //OnPropertyChanged("ObservacionesSignosVitales");
        }

        private void ValidacionesUrgencia()
        {
            base.AddRule(() => TextUrgenciaDestino, () => !string.IsNullOrEmpty(TextUrgenciaDestino), "DESTINO ES REQUERIDO!");
            base.AddRule(() => TextUrgenciaEstadoMental, () => !string.IsNullOrEmpty(TextUrgenciaEstadoMental), "ESTADO MENTAL ES REQUERIDO!");
            base.AddRule(() => TextUrgenciaMotivo, () => !string.IsNullOrEmpty(TextUrgenciaMotivo), "MOTIVO ES REQUERIDO!");
            base.AddRule(() => TextUrgenciaProcedimiento, () => !string.IsNullOrEmpty(TextUrgenciaProcedimiento), "PROCEDIMIENTO ES REQUERIDO!");
            OnPropertyChanged("TextUrgenciaDestino");
            OnPropertyChanged("TextUrgenciaEstadoMental");
            OnPropertyChanged("TextUrgenciaMotivo");
            OnPropertyChanged("TextUrgenciaProcedimiento");
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

        private void ValidacionesInterconsulta()
        {
            base.AddRule(() => TextInterconsultaCriterio, () => !string.IsNullOrEmpty(TextInterconsultaCriterio), "CRITERIO ES REQUERIDO!");
            base.AddRule(() => TextInterconsultaMotivo, () => !string.IsNullOrEmpty(TextInterconsultaMotivo), "MOTIVO ES REQUERIDO!");
            base.AddRule(() => TextInterconsultaSugerencias, () => !string.IsNullOrEmpty(TextInterconsultaSugerencias), "SUGERENCIAS ES REQUERIDO!");
            base.AddRule(() => TextInterconsultaTratamiento, () => !string.IsNullOrEmpty(TextInterconsultaTratamiento), "TRATAMIENTO ES REQUERIDO!");
            OnPropertyChanged("TextInterconsultaCriterio");
            OnPropertyChanged("TextInterconsultaMotivo");
            OnPropertyChanged("TextInterconsultaSugerencias");
            OnPropertyChanged("TextInterconsultaTratamiento");
        }

        private void ValidacionesTraslado()
        {
            base.AddRule(() => TextTrasladoEnvia, () => !string.IsNullOrEmpty(TextTrasladoEnvia), "QUIEN ENVIA ES REQUERIDO!");
            base.AddRule(() => TextTrasladoMotivo, () => !string.IsNullOrEmpty(TextTrasladoMotivo), "MOTIVO ES REQUERIDO!");
            base.AddRule(() => TextTrasladoReceptor, () => !string.IsNullOrEmpty(TextTrasladoReceptor), "RECEPTOR ES REQUERIDO!");
            OnPropertyChanged("TextTrasladoEnvia");
            OnPropertyChanged("TextTrasladoMotivo");
            OnPropertyChanged("TextTrasladoReceptor");
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
