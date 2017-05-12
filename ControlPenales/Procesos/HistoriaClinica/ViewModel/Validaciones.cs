using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace ControlPenales
{
    partial class HistoriaClinicaViewModel
    {

        #region VALIDACIONES DENTALES
        void ValidacionesHistoriaClinicaDental()
        {
            base.ClearRules();
            base.AddRule(() => ComplicacionesDespuesTratamDental, () => !string.IsNullOrEmpty(ComplicacionesDespuesTratamDental), "COMPLICACIONES DESPUÉS DE TRATAMIENTO ES REQUERIDO");
            OnPropertyChanged("ComplicacionesDespuesTratamDental");
            base.AddRule(() => HemorragiaDespuesExtracDental, () => !string.IsNullOrEmpty(HemorragiaDespuesExtracDental), "HEMORRAGIA DESPUÉS DE EXTRACCIÓN ES REQUERIDA");
            OnPropertyChanged("HemorragiaDespuesExtracDental");
            base.AddRule(() => TenidoReaccionNegativaDental, () => !string.IsNullOrEmpty(TenidoReaccionNegativaDental), "REACCIÓN NEGATIVA ES REQUERIDA");
            OnPropertyChanged("TenidoReaccionNegativaDental");
            base.AddRule(() => TomandoAlgunMedicamento, () => !string.IsNullOrEmpty(TomandoAlgunMedicamento), "TOMANDO ALGÚN MEDICAMENTO ES REQUERIDO");
            OnPropertyChanged("TomandoAlgunMedicamento");
            base.AddRule(() => AlergicoAlgunMedicamento, () => !string.IsNullOrEmpty(AlergicoAlgunMedicamento), "ALÉRGICO ALGÚN MEDICAMENTO ES REQUERIDO");
            OnPropertyChanged("AlergicoAlgunMedicamento");
            base.AddRule(() => EspecifiqueLabiosDental, () => !string.IsNullOrEmpty(EspecifiqueLabiosDental), "LABIOS ES REQUERIDO");
            OnPropertyChanged("EspecifiqueLabiosDental");
            base.AddRule(() => EspecifiqueLenguaDental, () => !string.IsNullOrEmpty(EspecifiqueLenguaDental), "LENGUA ES REQUERIDO");
            OnPropertyChanged("EspecifiqueLenguaDental");
            base.AddRule(() => EspecifiqueMucosaDental, () => !string.IsNullOrEmpty(EspecifiqueMucosaDental), "MUCOSA ES REQUERIDO");
            OnPropertyChanged("EspecifiqueMucosaDental");
            base.AddRule(() => EspecifiqueAmigdalasDental, () => !string.IsNullOrEmpty(EspecifiqueAmigdalasDental), "AMÍGDALAS ES REQUERIDO");
            OnPropertyChanged("EspecifiqueAmigdalasDental");
            base.AddRule(() => EspecifiquePisoBocaDental, () => !string.IsNullOrEmpty(EspecifiquePisoBocaDental), "PISO DE BOCA ES REQUERIDO");
            OnPropertyChanged("EspecifiquePisoBocaDental");
            base.AddRule(() => EspecifiquePaladarDuroDental, () => !string.IsNullOrEmpty(EspecifiquePaladarDuroDental), "PALADAR DURO ES REQUERIDO");
            OnPropertyChanged("EspecifiquePaladarDuroDental");
            base.AddRule(() => EspecifiquePaladarBlancoDental, () => !string.IsNullOrEmpty(EspecifiquePaladarBlancoDental), "PALADAR BLANCO ES REQUERIDO");
            OnPropertyChanged("EspecifiquePaladarBlancoDental");
            base.AddRule(() => EspecifiqueCarrillosDental, () => !string.IsNullOrEmpty(EspecifiqueCarrillosDental), "CARRILLOS ES REQUERIDO");
            OnPropertyChanged("EspecifiqueCarrillosDental");
            base.AddRule(() => EspecifiqueFrenillosDental, () => !string.IsNullOrEmpty(EspecifiqueFrenillosDental), "FRENILLOS ES REQUERIDO");
            OnPropertyChanged("EspecifiqueFrenillosDental");
            base.AddRule(() => EspecifiqueOtrosDental, () => !string.IsNullOrEmpty(EspecifiqueOtrosDental), "OTROS ES REQUERIDO");
            OnPropertyChanged("EspecifiqueOtrosDental");
            base.AddRule(() => CariesDental, () => !string.IsNullOrEmpty(CariesDental), "CARIES ES REQUERIDO");
            OnPropertyChanged("CariesDental");
            base.AddRule(() => FluoroDental, () => !string.IsNullOrEmpty(FluoroDental), "FLUOROSIS ES REQUERIDO");
            OnPropertyChanged("FluoroDental");
            base.AddRule(() => AnomForma, () => !string.IsNullOrEmpty(AnomForma), "ANOMALÍAS FORMA ES REQUERIDO");
            OnPropertyChanged("AnomForma");
            base.AddRule(() => AnomTamanio, () => !string.IsNullOrEmpty(AnomTamanio), "ANOMALÍAS TAMAÑO ES REQUERIDO");
            OnPropertyChanged("AnomTamanio");
            base.AddRule(() => HipoPlastDental, () => !string.IsNullOrEmpty(HipoPlastDental), "HIPOPLASIA ES REQUERIDO");
            OnPropertyChanged("HipoPlastDental");
            base.AddRule(() => OtrosHipoDental, () => !string.IsNullOrEmpty(OtrosHipoDental), "OTROS ES REQUERIDO");
            OnPropertyChanged("OtrosHipoDental");
            base.AddRule(() => DolorDental, () => !string.IsNullOrEmpty(DolorDental), "DOLOR ES REQUERIDO");
            OnPropertyChanged("DolorDental");
            base.AddRule(() => RigidezDental, () => !string.IsNullOrEmpty(RigidezDental), "RIGIDEZ ES REQUERIDO");
            OnPropertyChanged("RigidezDental");
            base.AddRule(() => ChasidosDental, () => !string.IsNullOrEmpty(ChasidosDental), "CHASQUIDOS ES REQUERIDO");
            OnPropertyChanged("ChasidosDental");
            base.AddRule(() => CansancioDental, () => !string.IsNullOrEmpty(CansancioDental), "CANSANCIO ES REQUERIDO");
            OnPropertyChanged("CansancioDental");
            base.AddRule(() => EnciasColorDental, () => !string.IsNullOrEmpty(EnciasColorDental), "COLOR ENCÍAS ES REQUERIDO");
            OnPropertyChanged("EnciasColorDental");
            base.AddRule(() => EnciasFormaDental, () => !string.IsNullOrEmpty(EnciasFormaDental), "FORMA ENCÍAS ES REQUERIDO");
            OnPropertyChanged("EnciasFormaDental");
            base.AddRule(() => EnciasTexturaDental, () => !string.IsNullOrEmpty(EnciasTexturaDental), "TEXTURA ENCÍAS ES REQUERIDO");
            OnPropertyChanged("EnciasTexturaDental");
            base.AddRule(() => BruxismoEstatus, () => !string.IsNullOrEmpty(BruxismoEstatus), "BRUXISMO ES REQUERIDO");
            OnPropertyChanged("BruxismoEstatus");
            base.AddRule(() => TemperaturaSignosVitalesDental, () => !string.IsNullOrEmpty(TemperaturaSignosVitalesDental), "TEMPERATURA ES REQUERIDO");
            OnPropertyChanged("TemperaturaSignosVitalesDental");
            base.AddRule(() => FrecuenciaCardSignosVitalesDental, () => !string.IsNullOrEmpty(FrecuenciaCardSignosVitalesDental), "FRECUENCIA CARDIACA ES REQUERIDA");
            OnPropertyChanged("FrecuenciaCardSignosVitalesDental");
            base.AddRule(() => FrecuenciaRespSignosVitalesDental, () => !string.IsNullOrEmpty(FrecuenciaRespSignosVitalesDental), "FRECUENCIA RESPIRATORIA ES REQUERIDA");
            OnPropertyChanged("FrecuenciaRespSignosVitalesDental");
            base.AddRule(() => GlicemiaSignosVitalesDental, () => !string.IsNullOrEmpty(GlicemiaSignosVitalesDental), "GLICEMIA ES REQUERIDO");
            OnPropertyChanged("GlicemiaSignosVitalesDental");
            base.AddRule(() => PesoSignosVitalesDental, () => !string.IsNullOrEmpty(PesoSignosVitalesDental), "PESO ES REQUERIDO");
            OnPropertyChanged("PesoSignosVitalesDental");
            base.AddRule(() => EstaturaSignosVitalesDental, () => !string.IsNullOrEmpty(EstaturaSignosVitalesDental), "ESTATURA ES REQUERIDO");
            OnPropertyChanged("EstaturaSignosVitalesDental");
            base.AddRule(() => Arterial1, () => !string.IsNullOrEmpty(Arterial1), "TENSION ARTERIAL ES REQUERIDO");
            OnPropertyChanged("Arterial1");
            base.AddRule(() => Arterial2, () => !string.IsNullOrEmpty(Arterial2), "TENSION ARTERIAL ES REQUERIDO");
            OnPropertyChanged("Arterial2");
        }

        #endregion
        private void SetValidacionesTodas()
        {
            base.ClearRules();
            //PERSONALES NO PATOLOGICOS INICIO
            base.AddRule(() => TextNacimientoNoPatologicos, () => TextNacimientoNoPatologicos != null ? !string.IsNullOrEmpty(TextNacimientoNoPatologicos) : false, "NACIMIENTO EN EL APARTADO DE ANTECEDENTES PERSONALES NO PATOLÓGICOS ES REQUERIDO");
            OnPropertyChanged("TextNacimientoNoPatologicos");
            base.AddRule(() => TextAlimentacionNoPatologicos, () => !string.IsNullOrEmpty(TextAlimentacionNoPatologicos), "ALIMENTACIÓN EN EL APARTADO DE ANTECEDENTES PERSONALES NO PATOLÓGICOS ES REQUERIDA");
            OnPropertyChanged("TextAlimentacionNoPatologicos");
            base.AddRule(() => TextHabitacionNoPatologicos, () => !string.IsNullOrEmpty(TextHabitacionNoPatologicos), "HABITACIÓN EN EL APARTADO DE ANTECEDENTES PERSONALES NO PATOLÓGICOS ES REQUERIDA");
            OnPropertyChanged("TextHabitacionNoPatologicos");
            base.AddRule(() => TextAlcoholismoNoPatologicos, () => !string.IsNullOrEmpty(TextAlcoholismoNoPatologicos), "ALCOHOLISMO EN EL APARTADO DE ANTECEDENTES PERSONALES NO PATOLÓGICOS ES REQUERIDO");
            OnPropertyChanged("TextAlcoholismoNoPatologicos");
            base.AddRule(() => TextTabaquismoNoPatologicos, () => !string.IsNullOrEmpty(TextTabaquismoNoPatologicos), "TABAQUISMO EN EL APARTADO DE ANTECEDENTES PERSONALES NO PATOLÓGICOS ES REQUERIDO");
            OnPropertyChanged("TextTabaquismoNoPatologicos");
            base.AddRule(() => TextToxicomaniasNoPatologicos, () => !string.IsNullOrEmpty(TextToxicomaniasNoPatologicos), "TOXICOMANÍAS EN EL APARTADO DE ANTECEDENTES PERSONALES NO PATOLÓGICOS ES REQUERIDA");
            OnPropertyChanged("TextToxicomaniasNoPatologicos");


            //PERSONALES NO PATOLOGICOS FIN
            //
            base.AddRule(() => MedicamentosActivos, () => !string.IsNullOrEmpty(MedicamentosActivos), "MEDICAMENTOS ACTIVOS EN EL APARTADO DE ANTECEDENTES PATOLÓGICOS ES REQUERIDO");
            OnPropertyChanged("MedicamentosActivos");

            base.AddRule(() => TextRespiratorio, () => !string.IsNullOrEmpty(TextRespiratorio), "RESPIRATORIO EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextRespiratorio");
            base.AddRule(() => TextCardiovascular, () => !string.IsNullOrEmpty(TextCardiovascular), "CARDIOVASCULAR EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextCardiovascular");
            base.AddRule(() => TextDigestivo, () => !string.IsNullOrEmpty(TextDigestivo), "DIGESTIVO EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextDigestivo");
            base.AddRule(() => TextUrinario, () => !string.IsNullOrEmpty(TextUrinario), "URINARIO EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextUrinario");
            base.AddRule(() => TextGenital, () => !string.IsNullOrEmpty(TextGenital), "GENITAL EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextGenital");


            base.AddRule(() => TextEndocrino, () => !string.IsNullOrEmpty(TextEndocrino), "ENDOCRINO EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextEndocrino");
            base.AddRule(() => TextMusculoEsqueletico, () => !string.IsNullOrEmpty(TextMusculoEsqueletico), "MUSCULO ESQUELÉTICO EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextMusculoEsqueletico");
            base.AddRule(() => TextHematicoLinfatico, () => !string.IsNullOrEmpty(TextHematicoLinfatico), "HEMÁTICO LINFÁTICO EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextHematicoLinfatico");
            base.AddRule(() => TextNervioso, () => !string.IsNullOrEmpty(TextNervioso), "NERVIOSO EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextNervioso");
            base.AddRule(() => TextPielAnexos, () => !string.IsNullOrEmpty(TextPielAnexos), "PIEL Y ANEXOS EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextPielAnexos");
            base.AddRule(() => TextTerapeuticaPrevia, () => !string.IsNullOrEmpty(TextTerapeuticaPrevia), "TERAPÉUTICA PREVIA EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextTerapeuticaPrevia");
            base.AddRule(() => TextSintomasGenerales, () => !string.IsNullOrEmpty(TextSintomasGenerales), "SÍNTOMAS GRALES. EN EL APARTADO DE APARATOS Y SISTEMAS ES REQUERIDO");
            OnPropertyChanged("TextSintomasGenerales");

            base.AddRule(() => TextConclusiones, () => !string.IsNullOrEmpty(TextConclusiones), "CONCLUSIÓN EN EL APARTADO DE CONCLUSIONES ES REQUERIDA");
            OnPropertyChanged("TextConclusiones");

            //CONSIDERACIONES FINALES INICIO
            base.AddRule(() => IdComplica, () => IdComplica.HasValue ? IdComplica.Value != -1 : false, "GRAVEDAD DE ENFERMEDAD EN EL APARTADO DE CONSIDERACIONES FINALES ES REQUERIDA");
            OnPropertyChanged("IdComplica");
            base.AddRule(() => IdEtapaEvo, () => IdEtapaEvo.HasValue ? IdEtapaEvo.Value != -1 : false, "ETAPA EVOLUTIVA EN EL APARTADO DE CONSIDERACIONES FINALES ES REQUERIDA");
            OnPropertyChanged("IdEtapaEvo");
            base.AddRule(() => IdPosibRemis, () => IdPosibRemis.HasValue ? IdPosibRemis.Value != -1 : false, "POSIBILIDADES DE REMISIÓN EN EL APARTADO DE CONSIDERACIONES FINALES ES REQUERIDO");
            OnPropertyChanged("IdPosibRemis");
            base.AddRule(() => TextGradoAfectacion, () => !string.IsNullOrEmpty(TextGradoAfectacion), "GRADO AFECTACIÓN EN EL APARTADO DE CONSIDERACIONES FINALES ES REQUERIDO");
            OnPropertyChanged("TextGradoAfectacion");
            base.AddRule(() => TextPronostico, () => !string.IsNullOrEmpty(TextPronostico), "PRONOSTICO EN EL APARTADO DE CONSIDERACIONES FINALES ES REQUERIDO");
            OnPropertyChanged("TextPronostico");
            base.AddRule(() => IdCapacTrata, () => IdCapacTrata.HasValue ? IdCapacTrata.Value != -1 : false, "CAPACIDAD DE TRATAMIENTO EN EL APARTADO DE CONSIDERACIONES FINALES ES REQUERIDA");
            OnPropertyChanged("IdCapacTrata");
            base.AddRule(() => IdNivelReq, () => IdNivelReq.HasValue ? IdNivelReq.Value != -1 : false, "NIVEL DE ATENCIÓN MÉDICA QUE REQUIERE EN EL APARTADO DE CONSIDERACIONES FINALES ES REQUERIDO");
            OnPropertyChanged("IdNivelReq");
            //CONSIDERACIONES FINALES FIN

            //EXPLORACION FISICA INICIO
            base.AddRule(() => TextExploracionfisica, () => !string.IsNullOrEmpty(TextExploracionfisica), "EXPLORACIÓN FÍSICA EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDA");
            OnPropertyChanged("TextExploracionfisica");
            base.AddRule(() => TextCabeza, () => !string.IsNullOrEmpty(TextCabeza), "CABEZA EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextCabeza");
            base.AddRule(() => TextCuello, () => !string.IsNullOrEmpty(TextCuello), "CUELLO EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextCuello");
            base.AddRule(() => TextTorax, () => !string.IsNullOrEmpty(TextTorax), "TÓRAX EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextTorax");
            base.AddRule(() => TextAbdomen, () => !string.IsNullOrEmpty(TextAbdomen), "ABDOMEN EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextAbdomen");
            base.AddRule(() => TextRecto, () => !string.IsNullOrEmpty(TextRecto), "RECTO EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextRecto");
            base.AddRule(() => TextGenitales, () => !string.IsNullOrEmpty(TextGenitales), "GENITALES EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextGenitales");
            base.AddRule(() => TextExtremidades, () => !string.IsNullOrEmpty(TextExtremidades), "EXTREMIDADES EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextExtremidades");
            base.AddRule(() => Arterial1, () => !string.IsNullOrEmpty(Arterial1), "PRESIÓN SISTÓLICA EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDA");
            OnPropertyChanged("Arterial1");
            base.AddRule(() => Arterial2, () => !string.IsNullOrEmpty(Arterial2), "PRESIÓN DIASTÓLICA EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDA");
            OnPropertyChanged("Arterial2");
            base.AddRule(() => TextPulso, () => !string.IsNullOrEmpty(TextPulso), "PULSO EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextPulso");
            base.AddRule(() => TextRespiracion, () => !string.IsNullOrEmpty(TextRespiracion), "RESPIRACIÓN EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDA");
            OnPropertyChanged("TextRespiracion");
            base.AddRule(() => TextTemperatura, () => !string.IsNullOrEmpty(TextTemperatura), "TEMPERATURA EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDA");
            OnPropertyChanged("TextTemperatura");
            base.AddRule(() => TextPeso, () => !string.IsNullOrEmpty(TextPeso), "PESO EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextPeso");
            base.AddRule(() => TextEstatura, () => !string.IsNullOrEmpty(TextEstatura), "ESTATURA EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDA");
            OnPropertyChanged("TextEstatura");
            base.AddRule(() => TextResultadosAnalisisClinicos, () => !string.IsNullOrEmpty(TextResultadosAnalisisClinicos), "RESULTADOS DE ANÁLISIS CLÍNICOS EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextResultadosAnalisisClinicos");
            base.AddRule(() => TextResultadosestudiosGabinete, () => !string.IsNullOrEmpty(TextResultadosestudiosGabinete), "RESULTADO DE ESTUDIOS DE GABINETE EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextResultadosestudiosGabinete");
            base.AddRule(() => TextImpresionDiagnostica, () => !string.IsNullOrEmpty(TextImpresionDiagnostica), "IMPRESIÓN DIAGNOSTICA EN EL APARTADO DE EXPLORACIÓN FÍSICA ES REQUERIDO");
            OnPropertyChanged("TextImpresionDiagnostica");
            //EXPLORACION FISICA FIN

            //PADECIMIENTO ACTUAL INICIO
            base.AddRule(() => TextPadecimientoActual, () => !string.IsNullOrEmpty(TextPadecimientoActual), "PADECIMIENTO ACTUAL EN EL APARTADO DE PADECIMIENTO ACTUAL ES REQUERIDO");
            OnPropertyChanged("TextPadecimientoActual");
            //PADECIMIENTO ACTUAL FIN
        }


        private void SetValidacionesAHF()
        {
            base.ClearRules();
            //base.AddRule(() => TextEdadPadre, () =>
            //    CheckPadreVive ? TextEdadPadre.HasValue ? TextEdadPadre.Value > 0 : TextEdadPadre.HasValue : !CheckPadreVive, "LA EDAD DEL PADRE ES REQUERIDA");
            //OnPropertyChanged("CheckPadreVive");
            //OnPropertyChanged("TextEdadPadre");
            //base.AddRule(() => TextEdadMadre, () =>
            //    CheckMadreVive ? TextEdadMadre.HasValue ? TextEdadMadre.Value > 0 : TextEdadMadre.HasValue : !CheckMadreVive, "LA EDAD DE LA MADRE ES REQUERIDA");
            //OnPropertyChanged("CheckMadreVive");
            //OnPropertyChanged("TextEdadMadre");
            //base.AddRule(() => TextEdadConyuge, () =>
            //    CheckConyugeVive ? TextEdadConyuge.HasValue ? TextEdadConyuge.Value > 0 : TextEdadConyuge.HasValue : !CheckConyugeVive, "LA EDAD DEL CONYUGE ES REQUERIDA");
            //OnPropertyChanged("CheckConyugeVive");
            //OnPropertyChanged("TextEdadConyuge");
            //base.AddRule(() => TextEdadesHijos, () =>
            //    CheckHijosVive ? !string.IsNullOrEmpty(TextEdadesHijos) : !CheckHijosVive, "LAS EDADES DE LOS HIJOS SON REQUERIDAS");
            //OnPropertyChanged("CheckHijosVive");
            //OnPropertyChanged("TextEdadesHijos");
            //base.AddRule(() => TextHermanosHombres, () =>
            //    CheckHermanosVivos ? TextHermanosHombres.HasValue ? TextHermanosHombres.Value >= 0 : TextHermanosHombres.HasValue : !CheckHermanosVivos, "LAS EDADES DE LOS HERMANOS SON REQUERIDAS");
            //OnPropertyChanged("CheckHermanosVivos");
            //OnPropertyChanged("TextHermanosHombres");
            //base.AddRule(() => TextHermanosMujeres, () =>
            //    CheckHermanosVivos ? TextHermanosMujeres.HasValue ? TextHermanosMujeres.Value >= 0 : TextHermanosMujeres.HasValue : !CheckHermanosVivos, "LAS EDADES DE LAS HERMANAS SON REQUERIDAS");
            //OnPropertyChanged("TextHermanosMujeres");
        }
        private void SetValidacionesPadecimientoActual()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextPadecimientoActual), "PADECIMIENTO ACTUAL ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextPadecimientoActual");
        }
        private void SetValidacionesASRespiratorio()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextRespiratorio), "RESPIRATORIO ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextRespiratorio");
        }
        private void SetValidacionesASCardiovascular()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextCardiovascular), "CARDIOVASCULAR ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextCardiovascular");
        }
        private void SetValidacionesASDigestivo()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextDigestivo), "DIGESTIVO ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextDigestivo");
        }
        private void SetValidacionesASEndocrino()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextEndocrino), "ENDOCRINO ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextEndocrino");
        }
        private void SetValidacionesASGenital()
        {
            base.ClearRules();
            if (GenitalMujer == Visibility.Visible)
                base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextGenitalMujeres), "GENITALES ES REQUERIDO");
            if (GenitalHombre == Visibility.Visible)
                base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextGenitalHombres), "GENITALES ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextGenitalMujeres");
            OnPropertyChanged("TextGenitalHombres");
        }
        private void SetValidacionesASHematico()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextHematicoLinfatico), "HEMATICO LINFATICO ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextHematicoLinfatico");
        }
        private void SetValidacionesASMusculoEsqueletico()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextMusculoEsqueletico), "MUSCULO ESQUELETICO ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextMusculoEsqueletico");
        }
        private void SetValidacionesASNervioso()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextNervioso), "NERVIOSO ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextNervioso");
        }
        private void SetValidacionesASPielAnexos()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextPielAnexos), "PIEL Y ANEXOS ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextPielAnexos");
        }
        private void SetValidacionesASSintomasGenerales()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextSintomasGenerales), "SINTOMAS GENERALES ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextSintomasGenerales");
        }
        private void SetValidacionesASTerapeuticaPrevia()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextTerapeuticaPrevia), "TERAPEUTICA PREVIA ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextTerapeuticaPrevia");
        }
        private void SetValidacionesASUrinario()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextUrinario), "URINARIO ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextUrinario");
        }

        private void SetValidacionesEFAbdomen()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextAbdomen), "ABDOMEN ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextAbdomen");
        }
        private void SetValidacionesEFCabeza()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextCabeza), "CABEZA ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextCabeza");
        }
        private void SetValidacionesEFCuello()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextCuello), "CUELLO ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextCuello");
        }
        private void SetValidacionesEFExtremidades()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextExtremidades), "EXTREMIDADES ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextExtremidades");
        }
        private void SetValidacionesEFGeneral()
        {
            base.ClearRules();
            base.AddRule(() => TextPeso, () => !string.IsNullOrEmpty(TextPeso), "PESO ES REQUERIDO");
            base.AddRule(() => TextEstatura, () => !string.IsNullOrEmpty(TextEstatura), "ESTATURA ES REQUERIDO");
            OnPropertyChanged("TextPeso");
            OnPropertyChanged("TextEstatura");
        }
        private void SetValidacionesEFGenitales()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextGenitales), "GENITALES ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextGenitales");
        }
        private void SetValidacionesEFImpresionDiagnostica()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextImpresionDiagnostica), "IMPRESION DIAGNOSTICA ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextImpresionDiagnostica");
        }
        private void SetValidacionesEFRecto()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextRecto), "RECTO ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextRecto");
        }
        private void SetValidacionesEFResultadosClinicos()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextResultadosAnalisisClinicos), "RESULTADOS DE ANALISIS CLINICOS ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextResultadosAnalisisClinicos");
        }
        private void SetValidacionesEFResultadosGabinete()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextResultadosestudiosGabinete), "RESULTADOS DE ESTUDIO DE GABINETE ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextResultadosestudiosGabinete");
        }
        private void SetValidacionesEFSignosVitales()
        {
            base.ClearRules();
            base.AddRule(() => TextPresionArterial, () => !string.IsNullOrEmpty(TextPresionArterial), "PRESION ARTERIAL ES REQUERIDO");
            base.AddRule(() => TextPulso, () => !string.IsNullOrEmpty(TextPulso), "PULSO ES REQUERIDO");
            base.AddRule(() => TextRespiracion, () => !string.IsNullOrEmpty(TextRespiracion), "RESPIRACION ES REQUERIDO");
            base.AddRule(() => TextTemperatura, () => !string.IsNullOrEmpty(TextTemperatura), "TEMPERATURA ES REQUERIDO");
            OnPropertyChanged("TextPresionArterial");
            OnPropertyChanged("TextPulso");
            OnPropertyChanged("TextRespiracion");
            OnPropertyChanged("TextTemperatura");
        }
        private void SetValidacionesEFTorax()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextTorax), "TORAX ES REQUERIDO");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextTorax");
        }
        private void SetValidacionesConclusiones()
        {
            base.ClearRules();
            base.AddRule(() => TextGenerico, () => !string.IsNullOrEmpty(TextConclusiones), "CONCLUSIONES SON REQUERIDAS");
            OnPropertyChanged("TextGenerico");
            OnPropertyChanged("TextConclusiones");
        }
        private void SetValidacionesMujeres()
        {
            base.AddRule(() => CheckMenarquia, () => CheckMenarquia.HasValue, "MENARQUIA ES REQUERIDO EN EL APARTADO DE ANTECEDENTES GINECO OBSTÉTRICOS");
            base.AddRule(() => FechaUltimaMenstruacion, () => FechaUltimaMenstruacion.HasValue, "FECHA DE ÚLTIMA MENSTRUACIÓN ES REQUERIDO EN EL APARTADO DE ANTECEDENTES GINECO OBSTÉTRICOS");
            base.AddRule(() => TextPartos, () => TextPartos.HasValue, "PARTOS ES REQUERIDO EN EL APARTADO DE ANTECEDENTES GINECO OBSTÉTRICOS");
            base.AddRule(() => TextCesareas, () => TextCesareas.HasValue, "CESÁREAS ES REQUERIDO EN EL APARTADO DE ANTECEDENTES GINECO OBSTÉTRICOS");
            base.AddRule(() => TextAbortos, () => TextAbortos.HasValue, "ABORTOS ES REQUERIDO EN EL APARTADO DE ANTECEDENTES GINECO OBSTÉTRICOS");
            base.AddRule(() => TextEmbarazos, () => TextEmbarazos.HasValue, "EMBARAZOS ES REQUERIDO EN EL APARTADO DE ANTECEDENTES GINECO OBSTÉTRICOS");
            base.AddRule(() => TextAniosRitmo, () => !string.IsNullOrEmpty(TextAniosRitmo), "AÑOS RITMO ES REQUERIDO EN EL APARTADO DE ANTECEDENTES GINECO OBSTÉTRICOS");
            base.AddRule(() => TextDeformacionesOrganicas, () => !string.IsNullOrEmpty(TextDeformacionesOrganicas), "DEFORMACIONES ORGÁNICAS ES REQUERIDO EN EL APARTADO DE ANTECEDENTES GINECO OBSTÉTRICOS");
            base.AddRule(() => TextIntegridadFisica, () => !string.IsNullOrEmpty(TextIntegridadFisica), "INTEGRIDAD FÍSICA ES REQUERIDA EN EL APARTADO DE ANTECEDENTES GINECO OBSTÉTRICOS");
            base.AddRule(() => IdControlPreN, () => !string.IsNullOrEmpty(IdControlPreN), "CONTROL PRENATAL ES REQUERIDO EN EL APARTADO DE ANTECEDENTES GINECO OBSTÉTRICOS");
            OnPropertyChanged("CheckMenarquia");
            OnPropertyChanged("FechaUltimaMenstruacion");
            OnPropertyChanged("TextPartos");
            OnPropertyChanged("TextCesareas");
            OnPropertyChanged("TextAbortos");
            OnPropertyChanged("TextEmbarazos");
            OnPropertyChanged("TextAniosRitmo");
            OnPropertyChanged("TextDeformacionesOrganicas");
            OnPropertyChanged("TextIntegridadFisica");
            OnPropertyChanged("IdControlPreN");
        }

        private void LimpiaValidacionesMujeres()
        {
            base.RemoveRule("CheckMenarquia");
            base.RemoveRule("FechaUltimaMenstruacion");
            base.RemoveRule("TextPartos");
            base.RemoveRule("TextCesareas");
            base.RemoveRule("TextAbortos");
            base.RemoveRule("TextEmbarazos");
            base.RemoveRule("TextAniosRitmo");
            base.RemoveRule("TextDeformacionesOrganicas");
            base.RemoveRule("TextIntegridadFisica");
            base.RemoveRule("IdControlPreN");
            OnPropertyChanged("CheckMenarquia");
            OnPropertyChanged("FechaUltimaMenstruacion");
            OnPropertyChanged("TextPartos");
            OnPropertyChanged("TextCesareas");
            OnPropertyChanged("TextAbortos");
            OnPropertyChanged("TextEmbarazos");
            OnPropertyChanged("TextAniosRitmo");
            OnPropertyChanged("TextDeformacionesOrganicas");
            OnPropertyChanged("TextIntegridadFisica");
            OnPropertyChanged("IdControlPreN");
        }

        private void ValidacionesAdultosMayores()
        {
            base.AddRule(() => IdDisminucionVisua, () => IdDisminucionVisua.HasValue ? IdDisminucionVisua != -1 : false, "DISMINUCIÓN AUDIO VISUAL ES REQUERIDO EN EL APARTADO DE MAYORES DE 65");
            base.AddRule(() => IdTranstornosMemoria, () => IdTranstornosMemoria.HasValue ? IdTranstornosMemoria != -1 : false, "TRASTORNOS DE LA MEMORIA ES REQUERIDO EN EL APARTADO DE MAYORES DE 65");
            base.AddRule(() => IdTranstornosAtencion, () => IdTranstornosAtencion.HasValue ? IdTranstornosAtencion != -1 : false, "TRASTORNOS DE ATENCIÓN EN EL APARTADO DE MAYORES DE 65");
            base.AddRule(() => IdTranstornosComprension, () => IdTranstornosComprension.HasValue ? IdTranstornosComprension != -1 : false, "TRASTORNOS DE COMPRENSIÓN ES REQUERIDO EN EL APARTADO DE MAYORES DE 65");
            base.AddRule(() => IdTranstornosOrientacion, () => IdTranstornosOrientacion.HasValue ? IdTranstornosOrientacion != -1 : false, "TRASTORNOS DE ORIENTACIÓN ES REQUERIDO EN EL APARTADO DE MAYORES DE 65");
            base.AddRule(() => IdEstadoAnimo, () => IdEstadoAnimo.HasValue ? IdEstadoAnimo != -1 : false, "TRASTORNOS DE ESTADO DE ÁNIMO ES REQUERIDO EN EL APARTADO DE MAYORES DE 65");
            base.AddRule(() => CheckAgudezaAuditiva, () => !string.IsNullOrEmpty(CheckAgudezaAuditiva), "ALTERACIONES EN AGUDEZA AUDITIVA ES REQUERIDO EN EL APARTADO DE MAYORES DE 65");
            base.AddRule(() => CheckOlfacion, () => !string.IsNullOrEmpty(CheckOlfacion), "ALTERACIONES EN LA OLFACCIÓN ES REQUERIDO EN EL APARTADO DE MAYORES DE 65");
            base.AddRule(() => CheckCapacidadVisomotriz, () => !string.IsNullOrEmpty(CheckCapacidadVisomotriz), "CAPACIDAD VISOMOTRIZ ES REQUERIDO EN EL APARTADO DE MAYORES DE 65");
            base.AddRule(() => CheckDemencial, () => !string.IsNullOrEmpty(CheckDemencial), "TRASTORNOS DEMENCIALES ES REQUERIDO EN EL APARTADO DE MAYORES DE 65");
            base.AddRule(() => IdCapacidadParticipacion, () => IdCapacidadParticipacion.HasValue ? IdCapacidadParticipacion != -1 : false, "CAPACIDAD DE PARTICIPACIÓN ES REQUERIDO EN EL APARTADO DE MAYORES DE 65");
            OnPropertyChanged("IdDisminucionVisua");
            OnPropertyChanged("IdTranstornosMemoria");
            OnPropertyChanged("IdTranstornosAtencion");
            OnPropertyChanged("IdTranstornosComprension");
            OnPropertyChanged("IdTranstornosOrientacion");
            OnPropertyChanged("IdEstadoAnimo");
            OnPropertyChanged("CheckAgudezaAuditiva");
            OnPropertyChanged("CheckOlfacion");
            OnPropertyChanged("CheckCapacidadVisomotriz");
            OnPropertyChanged("CheckDemencial");
            OnPropertyChanged("IdCapacidadParticipacion");
        }

        private void LimpiarValidacionesAdultosMayores()
        {
            base.RemoveRule("IdCapacidadParticipacion");
            base.RemoveRule("CheckDemencial");
            base.RemoveRule("IdDisminucionVisua");
            base.RemoveRule("IdTranstornosMemoria");
            base.RemoveRule("IdTranstornosAtencion");
            base.RemoveRule("IdTranstornosComprension");
            base.RemoveRule("IdTranstornosOrientacion");
            base.RemoveRule("IdEstadoAnimo");
            base.RemoveRule("CheckAgudezaAuditiva");
            base.RemoveRule("CheckOlfacion");
            base.RemoveRule("CheckCapacidadVisomotriz");
            OnPropertyChanged("IdCapacidadParticipacion");
            OnPropertyChanged("CheckDemencial");
            OnPropertyChanged("IdDisminucionVisua");
            OnPropertyChanged("IdTranstornosMemoria");
            OnPropertyChanged("IdTranstornosAtencion");
            OnPropertyChanged("IdTranstornosComprension");
            OnPropertyChanged("IdTranstornosOrientacion");
            OnPropertyChanged("IdEstadoAnimo");
            OnPropertyChanged("CheckAgudezaAuditiva");
            OnPropertyChanged("CheckOlfacion");
            OnPropertyChanged("CheckCapacidadVisomotriz");
        }

        private void ValidacionesCamposMujeresDental() 
        {
            try
            {
                base.RemoveRule("AmenazaAbortoDental");
                base.RemoveRule("LactandoDental");
                base.AddRule(() => AmenazaAbortoDental, () => !string.IsNullOrEmpty(AmenazaAbortoDental), "EMBARAZADA ES REQUERIDO EN EL APARTADO DE INTERROGATORIO");
                base.AddRule(() => LactandoDental, () => !string.IsNullOrEmpty(LactandoDental), "LACTANDO ES REQUERIDO EN EL APARTADO DE INTERROGATORIO");
                OnPropertyChanged("AmenazaAbortoDental");
                OnPropertyChanged("LactandoDental");
            }
            catch (System.Exception exc)
            {
                throw exc; ;
            }
        }

        private void LimpiaValidacionesCamposMujeres() 
        {
            try
            {
                base.RemoveRule("AmenazaAbortoDental");
                base.RemoveRule("LactandoDental");
                OnPropertyChanged("AmenazaAbortoDental");
                OnPropertyChanged("LactandoDental");
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
    }
}
